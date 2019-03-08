//using CommLib;
//using HJD.AccessService.Contract.Model;
//using HJD.AccessService.Contract.Params;
//using Lucene.Net.Analysis;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Net;
//using System.Text;
//using System.Text.RegularExpressions;

//namespace HJD.AccessService.Implement.Helper
//{
//    public class HmmHelper
//    {
//        /// <summary>
//        /// 搜索
//        /// </summary>
//        /// <param name="searchParams"></param>
//        /// <returns></returns>
//        public static SearchResult Search(SearchParams searchParams)
//        {
//            var result = new SearchResult();

//            //得到搜索类目（用户想干嘛）
//            var searchType = GetSearchType(searchParams);

//            //根据搜索类目创建搜索引擎
//            var searchEngine = CreateSearchEngine(searchType);

//            //搜索，得到结果
//            result = searchEngine.Search(searchParams);

//            //可能需要记录本次搜索历史
//            //。。。

//            //最后返回时间
//            result.returnTime = DateTime.Now;

//            return result;
//        }

//        /// <summary>
//        /// 根据搜索参数判断搜索类目
//        /// </summary>
//        /// <param name="value"></param>
//        /// <returns></returns>
//        public static SearchType GetSearchType(SearchParams searchParams)
//        {
//            /*
//                优先级：
//             * 订单 > 酒店 > 问题
//             * 
//            */

//            //得到分词
//            searchParams.keyList = SearchSource.AnalyzerKeywordArr(searchParams.keywords);
//            foreach (var key in searchParams.keyList)
//            {
//                if (SearchSource.OrderSource().Exists(o => o.Contains(key))) searchParams.orders.Add(key);          //订单
//                else if (SearchSource.HotelSource().Exists(o => o.Contains(key))) searchParams.hotels.Add(key);     //酒店
//                else if (SearchSource.AnswerSource().Exists(o => o.Contains(key))) searchParams.answers.Add(key);   //问答
//            }

//            //判断问题类目
//            if (searchParams.orders.Count > 0) return SearchType.Order;
//            if (searchParams.hotels.Count > 0) return SearchType.Hotel;
//            if (searchParams.answers.Count > 0) return SearchType.Answer;
//            return SearchType.Unknown;
//        }

//        public static ISearchEngine CreateSearchEngine(SearchType searchType)
//        {
//            switch (searchType)
//            {
//                case SearchType.Hotel:
//                    return HotelSearchEngine;
//                case SearchType.Order:
//                    return OrderSearchEngine;
//                case SearchType.Answer:
//                    return AnswerSearchEngine;
//                case SearchType.Unknown:
//                    return BaseSearchEngine;
//            }

//            return BaseSearchEngine;
//        }
//        private static readonly ISearchEngine BaseSearchEngine = new BaseSearchEngine();
//        private static readonly ISearchEngine HotelSearchEngine = new HotelSearchEngine();
//        private static readonly ISearchEngine OrderSearchEngine = new OrderSearchEngine();
//        private static readonly ISearchEngine AnswerSearchEngine = new AnswerSearchEngine();
//    }

//    #region 语言分类

//    /// <summary>
//    /// 基础搜索引擎
//    /// </summary>
//    public class BaseSearchEngine : ISearchEngine
//    {
//        public virtual SearchResult Search(SearchParams searchParams)
//        {
//            var result = new SearchResult();
//            result.resultType = SearchType.Unknown;
//            result.resultTitle = "如需进一步帮助，请联系人工客服";

//            return result;
//        }
//    }

//    public class HotelSearchEngine : BaseSearchEngine
//    {
//        public override SearchResult Search(SearchParams searchParams)
//        {
//            var result = new SearchResult();
//            result.resultType = SearchType.Hotel;
//            result.resultTitle = "您正在查询酒店信息";

//            //检索搜索内容中的关键字
//            foreach (var key in searchParams.keyList)
//            {
//                if (SearchSource.CitySource().Exists(city => city.Contains(key))) searchParams.citys.Add(key);
//                else if (SearchSource.ThemeSource().Exists(theme => theme.Contains(key))) searchParams.themes.Add(key);
//                else if (SearchSource.SightSource().Exists(sight => sight.Contains(key))) searchParams.sights.Add(key);
//                else if (SearchSource.TimeSource().Exists(time => time.Contains(key))) searchParams.times.Add(key);
//                else if (SearchSource.RoomStateSource().Exists(rstate => rstate.Contains(key))) searchParams.roomstates.Add(key);
//            }

//            //得到所有类目的关键字，然后返回对应类目的信息
//            //PAST: 我猜您要查找周末苏州有房的酒店？
//            result.resultValue += "我猜您要查找";
//            if (searchParams.times.Count > 0) result.resultValue += searchParams.times[0];
//            if (searchParams.citys.Count > 0) result.resultValue += searchParams.citys[0];
//            if (searchParams.sights.Count > 0) result.resultValue += searchParams.sights[0];
//            if (searchParams.roomstates.Count > 0) result.resultValue += searchParams.roomstates[0];
//            result.resultValue += "的酒店?";

//            //查询所有类目关键字相关的搜索结果
//            //比如，如果地点房态主题等关键字齐全，可以查询一个相关的酒店进行推荐
//            //举例：
//            if (searchParams.citys.Count > 0 && searchParams.citys[0].Contains("苏州"))
//            {
//                result.relationResult.Add(new SearchResultEntity
//                {
//                    title = "苏州金鸡湖凯宾斯基大酒店",
//                    url = "http://www.zmjiudian.com/hotel/188660"
//                });
//            }

//            return result;
//        }
//    }

//    public class OrderSearchEngine : BaseSearchEngine
//    {
//        public override SearchResult Search(SearchParams searchParams)
//        {
//            var result = new SearchResult();
//            result.resultType = SearchType.Order;
//            result.resultTitle = "您正在查询订单信息";

//            return result;
//        }
//    }

//    public class AnswerSearchEngine : BaseSearchEngine
//    {

//    }

//    /// <summary>
//    /// 搜索类目
//    /// </summary>
//    public enum SearchType
//    {
//        Unknown = -1,

//        //酒店搜索类目
//        Hotel = 1001,

//        //订单搜索类目
//        Order = 2001,

//        //咨询类目
//        Answer = 3001
//    }

//    public interface ISearchEngine
//    {
//        SearchResult Search(SearchParams searchParams);
//    }

//    #endregion

//    #region 源

//    public class SearchSource
//    {
//        public static List<string> AnalyzerKeywordArr(string SearchKeyword)
//        {
//            SearchKeyword = SearchKeyword.ToLower();
//            List<string> analyzerKeywords = new List<string>();
//            Analyzer analyzer = new IKAnalyzer();
//            TokenStream ts = analyzer.TokenStream("", new StringReader(SearchKeyword));
//            Token t = ts.Next();
//            while (t != null)
//            {
//                string strTermText = t.TermText();
//                analyzerKeywords.Add(strTermText);
//                t = ts.Next();
//            }
//            return analyzerKeywords;
//        }

//        public static List<string> HotelSource()
//        {
//            var list = new List<string>();

//            list.Add("酒店");
//            list.Add("客栈");
//            list.Add("宾馆");
//            list.Add("旅社");
//            list.Add("住哪");
//            list.Add("无家可归");

//            return list;
//        }

//        public static List<string> OrderSource()
//        {
//            var list = new List<string>();

//            list.Add("订单");
//            list.Add("下单");

//            return list;
//        }

//        public static List<string> AnswerSource()
//        {
//            var list = new List<string>();

//            return list;
//        }


//        public static List<string> CitySource()
//        {
//            var list = new List<string>();

//            list.Add("上海");
//            list.Add("苏州");
//            list.Add("南京");
//            list.Add("无锡");
//            list.Add("宁波");
//            list.Add("北京");
//            list.Add("常州");
//            list.Add("广州");
//            list.Add("成都");

//            return list;
//        }

//        public static List<string> ThemeSource()
//        {
//            var list = new List<string>();

//            list.Add("亲子度假");
//            list.Add("湖光山色");
//            list.Add("温泉");
//            list.Add("浪漫情侣");
//            list.Add("禅意生活");
//            list.Add("滑雪");
//            list.Add("避世仙境");
//            list.Add("海边");
//            list.Add("带宠物");

//            return list;
//        }

//        public static List<string> SightSource()
//        {
//            var list = new List<string>();

//            list.Add("佘山");
//            list.Add("上海欢乐谷");
//            list.Add("阳澄湖");
//            list.Add("金鸡湖");
//            list.Add("阳澄湖");
//            list.Add("旺山");
//            list.Add("苏州乐园");
//            list.Add("恐龙园");
//            list.Add("无锡太湖");
//            list.Add("莫干山");
//            list.Add("黄山");

//            return list;
//        }

//        public static List<string> PriceSource()
//        {
//            var list = new List<string>();

//            return list;
//        }

//        public static List<string> StarSource()
//        {
//            var list = new List<string>();

//            return list;
//        }

//        public static List<string> RoomStateSource()
//        {
//            var list = new List<string>();
//            list.Add("有房间");
//            list.Add("可入住");
//            list.Add("可以入住");

//            return list;
//        }        
        
//        public static List<string> TimeSource()
//        {
//            var list = new List<string>();
//            list.Add("周1");
//            list.Add("周2");
//            list.Add("周3");
//            list.Add("周4");
//            list.Add("周5");
//            list.Add("周6");
//            list.Add("周一");
//            list.Add("周二");
//            list.Add("周三");
//            list.Add("周四");
//            list.Add("周五");
//            list.Add("周六");
//            list.Add("周日");
//            list.Add("周天");
//            list.Add("周末");

//            return list;
//        }
//    }

//    #endregion

//    #region 搜索参数和搜索结果

//    /// <summary>
//    /// 搜索参数
//    /// </summary>
//    public class SearchParams
//    {
//        public DateTime searchTime = DateTime.Now;

//        public string keywords = "";
//        public List<string> keyList = new List<string>();
//        public SearchType searchType = SearchType.Unknown;

//        public List<string> hotels = new List<string>();
//        public List<string> orders = new List<string>();
//        public List<string> answers = new List<string>();

//        public List<string> citys = new List<string>();
//        public List<string> themes = new List<string>();
//        public List<string> sights = new List<string>();
//        public List<string> prices = new List<string>();
//        public List<string> stars = new List<string>();
//        public List<string> times = new List<string>();
//        public List<string> roomstates = new List<string>();

//    }

//    /// <summary>
//    /// 搜索结果
//    /// </summary>
//    public class SearchResult
//    {
//        public SearchResult() { searchTime = DateTime.Now; }

//        public DateTime searchTime = DateTime.Now;

//        public DateTime returnTime = DateTime.Now;

//        public SearchType resultType = SearchType.Unknown;

//        public string resultTitle { set; get; }

//        public string resultValue { set; get; }

//        public List<SearchResultEntity> relationResult = new List<SearchResultEntity>();
//    }

//    public class SearchResultEntity 
//    {
//        public string title { set; get; }
//        public string url { set; get; }

//        public List<SearchResultEntity> relationResult = new List<SearchResultEntity>();
//    }

//    #endregion
//}
