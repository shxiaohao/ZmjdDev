using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HJDAPI.Common.Helpers;
using HJDAPI.Controllers.Adapter;
using Newtonsoft.Json;

namespace HJDAPI.APIProxy
{
    public class Product : BaseProxy
    {
        /// <summary>
        /// 检查指定sku是否为分销产品
        /// </summary>
        /// <param name="SKUID"></param>
        /// <returns></returns>
        public Boolean IsRetailerProduct(int SKUID)
        {
            if (IsProductEvn)
                return ProductAdapter.IsRetailerProduct(SKUID);
            else
            {
                string url = APISiteUrl + "api/Product/IsRetailerProduct";
                string postDataStr = "SKUID=" + SKUID.ToString();

                CookieContainer cc = new CookieContainer();
                string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
                return JsonConvert.DeserializeObject<Boolean>(json);
            }
        }

        /// <summary>
        /// 更新大团购状态
        /// </summary>
        /// <param name="SKUID"></param>
        /// <returns></returns>
        public Boolean RemoveStepGroupCahceWithSKUID(int SKUID)
        {

            string url = APISiteUrl + "api/Product/RemoveStepGroupCahceWithSKUID";
            string postDataStr = "SKUID=" + SKUID.ToString();

            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<Boolean>(json);
        }
    }
}
