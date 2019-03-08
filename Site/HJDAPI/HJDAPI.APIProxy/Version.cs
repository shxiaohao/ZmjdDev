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
    public class Version : BaseProxy
    {
     
        public Dictionary<string, string> GetLatestAppVersion()
        {
            if (IsProductEvn)
                return new VersionAdapter().GetLatestAppVersion();
            else
            {
                string url = APISiteUrl + "api/Version/GetLatestAppVersion";
                string postDataStr = "";

                CookieContainer cc = new CookieContainer();
                string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
                return JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            }
        }
    }
}
