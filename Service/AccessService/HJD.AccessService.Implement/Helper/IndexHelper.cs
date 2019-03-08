//using CommLib;
//using HJD.AccessService.Contract.Model;
//using HJD.AccessService.Contract.Params;
//using HJD.AccessService.Implement.Entity;
//using Lucene.Net.Analysis;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Net;
//using System.Text;
//using System.Text.RegularExpressions;
//using LN = Lucene.Net;

//namespace HJD.AccessService.Implement.Helper
//{
//    public class IndexHelper
//    {
//        #region 单例对象

//        private static IndexHelper m_Instance = null;
//        internal static IndexHelper GetInstance(QuickSearchSuggestType t)
//        {
//            if (m_Instance == null)
//                m_Instance = new IndexHelper(t);
//            return m_Instance;
//        }

//        //对象函数
//        private IndexHelper(QuickSearchSuggestType t)
//        {
//            InitIndexParams(t);
//        }

//        #endregion

//        #region 成员

//        public QuickSearchSuggestType type;

//        public string indexPath;

//        public List<object> indexDocList;

//        public string[] searchField;    //new string[] { "Title", "Summary" }

//        public string keywords;

//        #endregion

//        #region 索引操作

//        /// <summary>
//        /// 初始索引
//        /// </summary>
//        public void InitIndexParams(QuickSearchSuggestType t)
//        {
//            type = t;

//            //根据不同的索引类型，来获取相关参数（索引库地址）
//            switch (type)
//            {
//                case QuickSearchSuggestType.Hotel:
//                    indexPath = @"D:\LPSearchEngine\Index\Hotel";
//                    break;
//                case QuickSearchSuggestType.Spot:
//                    indexPath = @"D:\LPSearchEngine\Index\Spot";
//                    break;
//                case QuickSearchSuggestType.City:
//                    indexPath = @"D:\LPSearchEngine\Index\City";
//                    break;
//            }
//        }

//        /// <summary>
//        /// 创建索引
//        /// </summary>
//        public void CreateIndex()
//        {
//            if (!string.IsNullOrEmpty(indexPath))
//            {
//                //判断是创建索引还是追加索引
//                bool isNew = false;
//                if (!LN.Index.IndexReader.IndexExists(indexPath))
//                {
//                    isNew = true;
//                }

//                LN.Index.IndexWriter iw = new LN.Index.IndexWriter(indexPath, new IKAnalyzer(), isNew);//使用PanGuAnalyzer初始化IndexWriter，参数create为true表示创建，为false表示添加。
//                //iw.SetMergeFactor(1000);

//                LN.Documents.Document document = new LN.Documents.Document();//创建Document

//                //var testField = new LN.Documents.Field("Uri", uri, LN.Documents.Field.Store.YES, LN.Documents.Field.Index.TOKENIZED);
//                ////testField.SetBoost

//                ////添加Field
//                //document.Add(new LN.Documents.Field("Uri", uri, LN.Documents.Field.Store.YES, LN.Documents.Field.Index.NO));
//                //document.Add(new LN.Documents.Field("Title", title, LN.Documents.Field.Store.YES, LN.Documents.Field.Index.ANALYZED));
//                //document.Add(new LN.Documents.Field("CreateTime", DateTime.Now.ToString("yyyy-MM-dd"), LN.Documents.Field.Store.YES, LN.Documents.Field.Index.NOT_ANALYZED));
//                //document.Add(new LN.Documents.Field("Summary", summary, LN.Documents.Field.Store.YES, LN.Documents.Field.Index.ANALYZED));
//                ////document.SetBoost


//                iw.AddDocument(document);//向索引添加文档

//                iw.Optimize();//优化索引

//                iw.Close();//关闭索引
//            }
//        }

//        /// <summary>
//        /// 更新索引
//        /// </summary>
//        public void UpdateIndex()
//        {

//        }

//        /// <summary>
//        /// 删除索引
//        /// </summary>
//        public void DeleteIndex()
//        {

//        }

//        /// <summary>
//        /// 设置高亮
//        /// </summary>
//        public void SetHighlighter()
//        {

//        }

//        #endregion

//        #region 搜索操作

//        /// <summary>
//        /// 搜索主方法
//        /// </summary>
//        public void Search()
//        {
//            //// 建立搜索分析对象
//            //var analyzer = new StandardAnalyzer();

//            //// 指定索引文档创建位置
//            //// Server.MapPath("Index")为索引文件存放地址
//            //var indexWriter = new IndexWriter(Server.MapPath("Index"), analyzer, true);

//            List<object> results = new List<object>();

//            LN.Search.IndexSearcher searcher = new LN.Search.IndexSearcher(indexPath);//初始化IndexSearcher

//            LN.QueryParsers.MultiFieldQueryParser parser = new LN.QueryParsers.MultiFieldQueryParser(searchField, new IKAnalyzer());    //初始化MultiFieldQueryParser以便同时查询多列

//            LN.Search.Query query = parser.Parse(keywords);//初始化Query

//            LN.Search.Hits hits = searcher.Search(query);//搜索

//            //遍历结果集
//            for (int i = 0; i < hits.Length(); i++)
//            {
//                LN.Documents.Document doc = hits.Doc(i);
//                //results.Add(new Item() { Title = doc.Get("Title"), Summary = doc.Get("Summary"), CreateTime = doc.Get("CreateTime"), Uri = doc.Get("Uri") });
//            }
//            searcher.Close();
//        }

//        #endregion
//    }
//}
