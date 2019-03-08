using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HJD.Search.CommonLibrary.Model;
using Lucene.Net;
using Lucene.Net.Analysis;
using Lucene.Net.Index;
using Lucene.Net.Documents;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Store;
using Lucene.Net.Search;
using Lucene.Net.QueryParsers;
using PanGu;
using HJD.Search.CommonLibrary.Config;
using PanGu.HighLight;
using HJD.AccessService.Contract.Model;
using HJD.AccessService.Contract;

namespace HJD.Search.CommonLibrary.Engine
{
    public class SearchEngine
    {
        #region 单例对象

        private static Dictionary<SearchType, SearchEngine> dic_Instance = new Dictionary<SearchType, SearchEngine>();

        public static SearchEngine GetInstance(SearchType t)
        {
            if (!dic_Instance.ContainsKey(t))
            {
                dic_Instance[t] = new SearchEngine(t);
            }

            return dic_Instance[t];
        }

        //对象函数
        public SearchEngine(SearchType t)
        {
            InitSearchParams(t);
        }

        #endregion

        #region 成员

        private Dictionary<string, IndexSearcher> IndexSearches;

        private object lockObj = new object();

        private Analyzer analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);

        private List<SearchType> searchTypes;

        private string indexPath;

        private string[] indexPaths;

        private DateTime searchStartTime;
        public DateTime SearchStartTime
        {
            get { return searchStartTime; }
            set { searchStartTime = value; }
        }

        private DateTime searchEndTime;
        public DateTime SearchEndTime
        {
            get { return searchEndTime; }
            set 
            {
                searchEndTime = value;
                searchTotalMilliseconds = (value - searchStartTime).TotalMilliseconds; 
            }
        }

        private double searchTotalMilliseconds;
        public double SearchTotalMilliseconds
        {
            get { return searchTotalMilliseconds; }
            set { searchTotalMilliseconds = value; }
        }

        #endregion

        #region 搜索参数配置

        /// <summary>
        /// 初始索引
        /// </summary>
        private void InitSearchParams(SearchType t)
        {
            searchTypes = new List<SearchType> { t };

            //根据不同的索引类型，来获取相关参数（如索引库地址，检索项等）
            indexPath = IndexConfig.GetIndexPathByType(t);
        }
        private void InitSearchParams(List<SearchType> ts)
        {
            searchTypes = ts;

            //根据不同的索引类型，来获取相关参数（如索引库地址，检索项等）
            indexPaths = IndexConfig.GetIndexPathsByTypes(ts);
        }

        #endregion

        #region 检索操作


        #region 去检索

        /// <summary>
        /// 搜索主入口
        /// </summary>
        /// <param name="keywords"></param>
        /// <param name="orderBy"></param>
        /// <param name="limitCount"></param>
        /// <param name="sType"></param>
        /// <returns></returns>
        public List<BaseSearchResult> Search(string keywords, OrderBy orderBy, int limitCount, SearcherType searcherType)
        {
            //根据搜索类型（如酒店搜索）、搜索关键字解析出针对该索引库需要搜索的列/值
            var searchDataList = NLPEngine.ExplainKeywords(searchTypes, keywords);

            switch (searcherType)
            {
                case SearcherType.Single:
                    return SingleSearch(searchDataList, orderBy, limitCount);
                case SearcherType.Multi:
                    return MultiSearch(searchDataList, orderBy, limitCount);
                case SearcherType.ParallelMulti:
                    return ParallelMultiSearcher(searchDataList, orderBy, limitCount);
            }

            return SingleSearch(searchDataList, orderBy, limitCount);
        }
        public List<BaseSearchResult> Search(string keywords, int limitCount, SearcherType searcherType)
        {
            return Search(keywords, null, limitCount, searcherType);
        }

        /// <summary>
        /// Qa 搜索入口
        /// </summary>
        /// <param name="keywords"></param>
        /// <param name="orderBy"></param>
        /// <param name="limitCount"></param>
        /// <param name="searcherType"></param>
        /// <returns></returns>
        public List<BaseSearchResult> QaSearch(List<QaParticipleEntity> questionWords, OrderBy orderBy, int limitCount, SearcherType searcherType)
        {
            //根据搜索类型（如酒店搜索）、搜索关键字解析出针对该索引库需要搜索的列/值
            var searchDataList = NLPEngine.QaExplainKeywords(searchTypes, questionWords);

            switch (searcherType)
            {
                case SearcherType.Single:
                    return SingleSearch(searchDataList, orderBy, limitCount);
                case SearcherType.Multi:
                    return MultiSearch(searchDataList, orderBy, limitCount);
                case SearcherType.ParallelMulti:
                    return ParallelMultiSearcher(searchDataList, orderBy, limitCount);
            }

            return SingleSearch(searchDataList, orderBy, limitCount);
        }
        public List<BaseSearchResult> QaSearch(List<QaParticipleEntity> questionWords, int limitCount, SearcherType searcherType)
        {
            return QaSearch(questionWords, null, limitCount, searcherType);
        }

        /// <summary>
        /// 单个索引 检索
        /// </summary>
        private List<BaseSearchResult> SingleSearch(List<SearchData> searchObjectList, OrderBy orderBy, int limitCount)
        {
            var index = GetIndexSearch(indexPath);

            return GoSearch(index, searchObjectList, orderBy, limitCount);
        }

        /// <summary>
        /// 多索引 检索
        /// </summary>
        private List<BaseSearchResult> MultiSearch(List<SearchData> searchObjectList, OrderBy orderBy, int limitCount)
        {
            var allIndexSearch = GetIndexSearches(indexPaths);

            MultiSearcher multiSearch = new MultiSearcher(allIndexSearch.ToArray());

            return GoSearch(multiSearch, searchObjectList, orderBy, limitCount);
        }

        /// <summary>
        /// 多索引并行检索
        /// </summary>
        private List<BaseSearchResult> ParallelMultiSearcher(List<SearchData> searchObjectList, OrderBy orderBy, int limitCount)
        {
            var allIndexSearch = GetIndexSearches(indexPaths);

            ParallelMultiSearcher parallelMultiSearch = new ParallelMultiSearcher(allIndexSearch.ToArray());

            return GoSearch(parallelMultiSearch, searchObjectList, orderBy, limitCount);
        }

        private List<BaseSearchResult> GoSearch(Searcher searches, List<SearchData> searchObjectList, OrderBy orderBy, int limitCount)
        {
            Query query;

            if (searchObjectList == null || searchObjectList.Count <= 0)
            {
                searchObjectList = new List<SearchData>();
                SearchData searchObject = new SearchData();
                searchObject.Column = "*";
                searchObject.Value = string.Empty;
                searchObjectList.Add(searchObject);
            }

            var docs = SearchDocs(searches, searchObjectList, orderBy, limitCount, out query).ToList();

            List<BaseSearchResult> list = new List<BaseSearchResult>();
            foreach (var item in docs)
            {
                var doc = searches.Doc(item.Doc);
                //doc.Boost = 1F;

                var ex = searches.Explain(query, item.Doc);

                list.Add(new BaseSearchResult { Id = doc.Get("Id"), Boost = item.Score, Explain = ex.ToHtml() });
            }

            return list;
        }

        private ScoreDoc[] SearchDocs(Searcher searcher, List<SearchData> searchObject, OrderBy orderBy, int limitCount, out Query query)
        {
            ScoreDoc[] docs = null;

            query = BulidQuery(searchObject, analyzer, searcher);

            var total = 0;

            if (query == null) return new ScoreDoc[] { };

            if (orderBy == null || string.IsNullOrEmpty(orderBy.Coloumn))
            {
                var allDocs = searcher.Search(query, limitCount);
                total = allDocs.TotalHits;
                docs = allDocs.ScoreDocs;
            }
            else
            {
                var allDocs = searcher.Search(query, null, limitCount, new Sort(new SortField(orderBy.Coloumn + "_sort", SortField.STRING, !orderBy.IsDesc)));

                total = allDocs.TotalHits;
                docs = allDocs.ScoreDocs;
            }
            return docs;

        }

        #endregion

        #region 检索结果

        private Query BulidQuery(List<SearchData> searchObject, Analyzer analyzer, Searcher searhcer)
        {
            BooleanQuery bq = new BooleanQuery();

            foreach (var item in searchObject)
            {
                if (item.Column == "*")
                {
                    var query = BulidQuery(item.Value.ToString(), analyzer, searhcer);
                    bq.Add(query, item.Logic == Logic.and ? Lucene.Net.Search.Occur.MUST : item.Logic == Logic.or ? Occur.SHOULD : Occur.MUST_NOT);
                }
                else
                {
                    var query = ParserOperatorTypeQuery(item, analyzer);
                    if (query != null)
                        bq.Add(query, item.Logic == Logic.and ? Occur.MUST : item.Logic == Logic.or ? Occur.SHOULD : Occur.MUST_NOT);
                }
            }

            if (bq.Clauses.Count == 0) return null;

            return bq;
        }

        private Query BulidQuery(string keyword, Analyzer analyzer, Searcher searhcer)
        {
            if (string.IsNullOrEmpty(keyword))
            {
                return BulidQueryForNoKeyword();
            }

            return BulidQueryForKeyWord(keyword, analyzer, searhcer);
        }

        private Query BulidQueryForKeyWord(string keyword, Analyzer analyzer, Searcher searhcer)
        {
            List<string> keys = new List<string>();
            List<string> values = new List<string>();

            var keywordsSplit = keyword;

            foreach (var item in IndexSearches.Select(x => x.Value).FirstOrDefault().IndexReader.GetFieldNames(IndexReader.FieldOption.ALL))
            {
                if (item.ToLower() == "id" || item.ToLower().Contains("_sort"))
                    continue;
                keys.Add(item);
                values.Add(keywordsSplit);
            }

            Query query = MultiFieldQueryParser.Parse(Lucene.Net.Util.Version.LUCENE_30,
                values.ToArray(), keys.ToArray(), analyzer);

            return query;
        }

        private Query ParserOperatorTypeQuery(SearchData item, Analyzer analyzer)
        {
            Query query = null;
            if (string.IsNullOrEmpty(item.Value.ToString().Trim())) return query;
            if (item.OperatorType == Operator.Equal)
            {
                Term t = new Term(item.Column + "_sort", item.Value.ToString());

                query = new TermQuery(t);
            }
            else
            {
                QueryParser parser = new QueryParser(Lucene.Net.Util.Version.LUCENE_30, item.Column, analyzer);
                query = parser.Parse(item.Value.ToString());
            }

            if (item.Column.ToLower() == "star")
            {
                query.Boost = 2F;   
            }

            return query;
        }

        private Query BulidQueryForNoKeyword()
        {
            NumericRangeQuery<int> query = NumericRangeQuery.NewIntRange("ID", 0, 10000000, true, true);

            return query;
        }

        #endregion

        #region 进行时方法处理

        /// <summary>
        /// 获取指定的索引的检索对象（单个）
        /// </summary>
        /// <param name="indexPath"></param>
        /// <returns></returns>
        private IndexSearcher GetIndexSearch(string indexPath)
        {
            if (!System.IO.Directory.Exists(indexPath))
            {
                throw new ArgumentException("当前索引路径不存在");
            }

            lock (lockObj)
            {
                if (IndexSearches == null)
                {
                    IndexSearches = new Dictionary<string, IndexSearcher>();
                }

                //new
                IndexReader reader = LuceneFactory.GetIndexReader(indexPath);
                var searcher = LuceneFactory.GetIndexSearcher(reader);
                IndexSearches[indexPath] = searcher;

                //old
                //if (!IndexSearches.Keys.Contains(indexPath))
                //{
                //    System.IO.DirectoryInfo info = new System.IO.DirectoryInfo(indexPath);

                //    IndexSearcher searcher = new IndexSearcher(FSDirectory.Open(info), true);

                //    IndexSearches.Add(indexPath, searcher);
                //}

                return IndexSearches[indexPath];

            }

        }

        /// <summary>
        /// 获取多个索引的检索对象
        /// </summary>
        /// <param name="indexPath"></param>
        /// <returns></returns>
        private List<IndexSearcher> GetIndexSearches(string[] indexPath)
        {
            List<IndexSearcher> allIndexSearch = new List<IndexSearcher>();

            foreach (var item in indexPath)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    allIndexSearch.Add(GetIndexSearch(item));
                }
            }

            return allIndexSearch;
        }

        /// <summary>
        /// 返回分词集合的字符串
        /// </summary>
        /// <param name="word"></param>
        /// <param name="split"></param>
        /// <returns></returns>
        public string GetWord(ICollection<string> words, string split = " ")
        {
            return string.Join(split, words);

        }

        /// <summary>
        /// 获取分词集合
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public static ICollection<string> GetWords(string word)
        {
            if (NLPEngine.Segment == null) NLPEngine.Segment = new PanGuSegment();
            return NLPEngine.Segment.GetSegment(word).Select(x => x.Word).ToList();
        }

        /// <summary>
        /// 高亮处理
        /// </summary>
        /// <param name="keywords"></param>
        /// <param name="content"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string GetHightLighter(string keywords, string content, int length)
        {
            // 高亮显示设置
            SimpleHTMLFormatter simpleHTMLFormatter = new SimpleHTMLFormatter("<font color=\"red\">", "</font>");
            var highlighter = new Highlighter(simpleHTMLFormatter, new Segment());

            //关键内容显示大小设置 
            highlighter.FragmentSize = length;
            return highlighter.GetBestFragment(keywords, content);
        }

        #endregion


        #endregion
    }
}
