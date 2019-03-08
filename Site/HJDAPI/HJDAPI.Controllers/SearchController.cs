using HJDAPI.Common.Helpers;
using HJDAPI.Controllers.Adapter;
using HJDAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace HJDAPI.Controllers
{
    public class SearchController : BaseApiController
    {

        /// <summary>
        /// 搜索方法（目前该接口仅支持城市和酒店搜索）
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="cityCount"></param>
        /// <param name="hotelCount"></param>
        /// <returns></returns>
        [HttpGet]
        public List<QuickSearchSuggestItem> Search([FromUri]  SearchParams p)
        {

            var rest = SearchAdapter.searchForApp(p, base.IsApp);
           #region 行为记录

           try
           {
               var value = string.Format("{{\"keyword\":\"{0}\",\"cityCount\":\"{1}\",\"hotelCount\":\"{2}\"}}", p.keyword.Replace("|", ""), p.cityCount, p.hotelCount);
               RecordBehavior("Search", value);
           }
           catch (Exception ex) { }

           #endregion

           return rest;

        }

        /// <summary>
        /// 搜索城市
        /// </summary> 
        /// <returns></returns>
        [HttpGet]
        public List<QuickSearchSuggestItem> SearchCity([FromUri]  SearchParamsForCity p)
        {
            var rest = SearchAdapter.searchForCity(p);

            return rest;
        }



        /// <summary>
        /// 搜索城市
        /// </summary> 
        /// <returns></returns>
        [HttpGet]
        public List<QuickSearchSuggestItem> SearchCoupon([FromUri]  SearchParamsForCoupon p)
        {
            var rest = SearchAdapter.SearchCoupon(p);

            return rest;
        }

    }
}
