using HJD.DestServices.Contract;
using HJD.ElasticSearch.Contracts.Entity;
using HJD.ElasticSearch.Contracts.Interface;
using HJD.Framework.WCF;
using HJDAPI.Common.Helpers;
using HJDAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.Controllers.Adapter
{
    public class SearchAdapter
    {
        private static IElasticSearchService searchSvc = ServiceProxyFactory.Create<IElasticSearchService>("IElasticSearchService");
        public static IDestService destService = ServiceProxyFactory.Create<IDestService>("BasicHttpBinding_IDestService");

        private const  string couponUrlFormat = "http://www.zmjiudian.com/coupon/product/{0}?userid={{userid}}&_newpage=1&_dorpdown=1&_newtitle=1&&_sourcekey={1}";
        
        /// <summary>
        /// 为APP提交的搜索样式
        /// 有高亮、默认搜索数量、城市周边等
        /// </summary>
        /// <param name="p"></param>
        /// <param name="isApp"></param>
        /// <returns></returns>
        public static List<QuickSearchSuggestItem> searchForApp(SearchParams p, bool isApp)
        {
            if (p.keyword == null || p.keyword.Trim().Length == 0)
            {
                return new List<QuickSearchSuggestItem>();
            }

            string URIPrex = isApp ? "whotelapp" : "http";
          

            var result = new List<QuickSearchSuggestItem>();

            QueryParams queryParams = new QueryParams();

            queryParams.querySentence = p.keyword;


            if (p.cityCount != 0)
            {
                queryParams.queryTypeList.Add(new QueryTypeParam { queryType = HJD.ElasticSearch.Contracts.QueryTypeEnum.city, needHighlight = p.needHighlight, from = 0, size = p.cityCount < 0 ? 4 : p.cityCount }); 
            }
            if (p.playCount != 0)
            {
                queryParams.queryTypeList.Add(new QueryTypeParam { queryType = HJD.ElasticSearch.Contracts.QueryTypeEnum.play, needHighlight = p.needHighlight, from = 0, size = p.playCount < 0 ? 3 : p.playCount }); 
            }
            if (p.foodCount != 0)
            {
                queryParams.queryTypeList.Add(new QueryTypeParam { queryType = HJD.ElasticSearch.Contracts.QueryTypeEnum.foods, needHighlight = p.needHighlight, from = 0, size = p.foodCount < 0 ? 3 : p.foodCount });
            }
            if (p.hotelCount != 0)
            {
                queryParams.queryTypeList.Add(new QueryTypeParam { queryType = HJD.ElasticSearch.Contracts.QueryTypeEnum.hotel, needHighlight = p.needHighlight, from = 0, size = p.hotelCount < 0 ? 9 : p.hotelCount });
            }

            var queryRest = searchSvc.Query(queryParams);

            if (queryRest.cityList.Count > 0)
            {
                bool bHasAddAroundCity = false;  //只加一次
                queryRest.cityList.ForEach(_ =>
               {
                   // 命中字数小于查询字数的目的地不显示
                   if (_.NameHighlight.Split("<".ToCharArray()).Count() / 2 < p.keyword.Trim().Length)
                   {
                       _.DistrictID = 0;
                   }
                   else
                   {
                       var item = new QuickSearchSuggestItem()
                       {
                           EName = _.EName,
                           ShowEName = StringHelper.SetHighlight(p.needHighlight, _.ENameHighlight, _.EName),
                           Icon = Configs.searchIcon4Lab,
                           Id = _.DistrictID,
                           Name = _.Name,
                           ShowName = StringHelper.SetHighlight(p.needHighlight, _.NameHighlight, _.Name),
                           ParentName = _.RootName,
                           Tag = "",
                           Type = "D",
                           Lat = _.Lat,
                           Lon = _.Lon,
                           ActionUrl = string.Format("http://www.zmjiudian.com/App/Around?userid={{userid}}&districtId={0}&districtName={1}&lat=0&lng=0&geoScopeType=1&_newpage=1&_headSearch=1&_searchType=1", _.DistrictID, _.Name)
                       };

                       result.Add(item);

                       //加周边
                       var newName = string.Format("{0}及周边", item.ShowName);
                       var newNameNoHighlisgh = string.Format("{0}及周边", _.Name);
                       var copy = new QuickSearchSuggestItem()
                       {
                           EName = newName,
                           ShowEName = newName,
                           Icon = item.Icon,
                           Id = _.DistrictID,
                           Name = newNameNoHighlisgh,
                           ShowName = newName,
                           ParentName = "",
                           Tag = "",
                           Type = "D",
                           GeoScopeType = 3,
                           Lat = _.Lat,
                           Lon = _.Lon,
                           ActionUrl = string.Format("http://www.zmjiudian.com/App/Around?userid={{userid}}&districtName={0}&lat={1}&lng={2}&geoScopeType=3&_newpage=1&_headSearch=1&_searchType=1", newNameNoHighlisgh, _.Lat, _.Lon)
                       };

                       result.Add(copy);

                   }
               });
            }

            if (queryRest.hotelList.Count > 0)
            {
                int hotelMaxTakeCount = 9 - queryRest.foodList.Count - queryRest.playList.Count;
                result.AddRange(
                queryRest.hotelList.Take(hotelMaxTakeCount).Select(_ => new QuickSearchSuggestItem()
                {
                    EName = _.EName,
                    ShowEName = StringHelper.SetHighlight(p.needHighlight, _.ENameHighlight, _.EName),
                    Icon = Configs.searchIcon4Hotel,
                    Id = _.Hotelid,
                    Name = _.Name,
                    ShowName = StringHelper.SetHighlight(p.needHighlight, _.NameHighlight, _.Name),
                    ParentName ="",
                    Tag = "",
                    Type = "H",
                    Lat = _.Lat,
                    Lon = _.Lon,
                    ActionUrl = string.Format("{0}://www.zmjiudian.com/hotel/{1}",URIPrex, _.Hotelid)
                }));
            }
          
            if (queryRest.foodList.Count > 0)
            {
                result.AddRange(
                queryRest.foodList.Select(_ => new QuickSearchSuggestItem()
                {
                    EName = "",
                    Icon = Configs.searchIcon4Foods,
                    Id = _.SKUID,
                    Name = _.PageTitle,
                    ShowName = StringHelper.SetHighlight(p.needHighlight, _.PageTitleHighlight, _.PageTitle),
                    ParentName = "",
                    Tag = "",
                    Type = "F",
                    Lat = _.Lat,
                    Lon = _.Lon,
                    ActionUrl = string.Format(couponUrlFormat, _.SKUID, _.SKUID, GetSKUSearchTag(isApp))
                }));
            }

            if (queryRest.playList.Count > 0)
            {
                result.AddRange(
                queryRest.playList.Select(_ => new QuickSearchSuggestItem()
                {
                    EName = "",
                    Icon =  Configs.searchIcon4Play,
                    Id = _.SKUID,
                    Name = _.PageTitle,
                    ShowName = StringHelper.SetHighlight(p.needHighlight, _.PageTitleHighlight, _.PageTitle),
                    ParentName = "",
                    Tag = "",
                    Type = "P",
                    Lat = _.Lat,
                    Lon = _.Lon,
                    ActionUrl = string.Format(couponUrlFormat, _.SKUID, GetSKUSearchTag(isApp))
                }));
            }

            return  CalScoreAndSort( result );
        }


        /// <summary>
        /// 为APP提交的搜索样式
        /// 有高亮、默认搜索数量、城市周边等
        /// </summary>
        /// <param name="p"></param>
        /// <param name="isApp"></param>
        /// <returns></returns>
        public static List<QuickSearchSuggestItem> searchForCity(SearchParamsForCity p )
        {
            if (p.keyword == null || p.keyword.Trim().Length == 0)
            {
                return new List<QuickSearchSuggestItem>();
            }             
            var result = new List<QuickSearchSuggestItem>();             

            var queryRest = searchSvc.QueryCity(query: p.keyword, from: 0, size: p.cityCount, needHighlight:p.needHighlight, onlyInChina:p.onlyInChina );

            if (queryRest.cityList.Count > 0)
            {
                queryRest.cityList.ForEach(_ =>
                {
                    // 命中字数小于查询字数的目的地不显示
                    
                        var item = new QuickSearchSuggestItem()
                        {
                            EName = _.EName,
                            ShowEName = StringHelper.SetHighlight(p.needHighlight, _.ENameHighlight, _.EName),
                            Icon = Configs.searchIcon4Lab,
                            Id = _.DistrictID,
                            Name = _.Name,
                            ShowName = StringHelper.SetHighlight(p.needHighlight, _.NameHighlight, _.Name),
                            ParentName = _.RootName,
                            Tag = "",
                            Type = "D",
                            Lat = _.Lat,
                            Lon = _.Lon,
                            ActionUrl = string.Format("http://www.zmjiudian.com/App/Around?userid={{userid}}&districtId={0}&districtName={1}&lat=0&lng=0&geoScopeType=1&_newpage=1&_headSearch=1&_searchType=1", _.DistrictID, _.Name)
                        };

                        result.Add(item);
                         
                     
                });
            }
             
            return CalScoreAndSort(result);
        }

        /// <summary>
        /// 为APP提交的搜索样式
        /// 有高亮、默认搜索数量、城市周边等
        /// </summary>
        /// <param name="p"></param>
        /// <param name="isApp"></param>
        /// <returns></returns>
        public static List<QuickSearchSuggestItem> SearchCoupon(SearchParamsForCoupon p)
        {
            if (p.keyword == null || p.keyword.Trim().Length == 0)
            {
                return new List<QuickSearchSuggestItem>();
            }
            var result = new List<QuickSearchSuggestItem>();

            var queryRest = searchSvc.QueryAllCoupon(new AllCouponQueryParams { KeyWords = p.keyword, from = 0, size = p.count, needHighlight = p.needHighlight, OnlyDistributable = p.OnlyDistributable });

            if (queryRest.Count > 0)
            {
                queryRest.ForEach(_ =>
                {
                    // 命中字数小于查询字数的目的地不显示

                    var item = new QuickSearchSuggestItem()
                    {
                        EName = "",
                        Icon = Configs.searchIcon4Lab,
                        Id = _.SKUID,
                        Name = _.PageTitle,
                        ShowName = StringHelper.SetHighlight(p.needHighlight, _.PageTitleHighlight, _.PageTitle),
                        ParentName = "",
                        Tag = "",
                        Type = "F",
                        Lat = _.Lat,
                        Lon = _.Lon,
                        ActionUrl = ""
                    };

                    result.Add(item);


                });
            }

            return CalScoreAndSort(result);
        }

        private static string GetSKUSearchTag(bool isApp)
        {
            return isApp ? "sku_search_app" : "sku_search_h5";
        }

        static Dictionary<string, double> dicEntityWeight = new Dictionary<string, double> { 
        { "D", 5 },
        {"H",1.2},
        {"P",1.1},
        {"F",1.0} 
        };

        private static List<QuickSearchSuggestItem> CalScoreAndSort(List<QuickSearchSuggestItem> result)
        {
            result.ForEach(r => r.Score = (r.Name.Split("<".ToCharArray()).Count() * dicEntityWeight[r.Type]));

            return result.OrderByDescending(_ => _.Score).ToList();

        }

    }
}
