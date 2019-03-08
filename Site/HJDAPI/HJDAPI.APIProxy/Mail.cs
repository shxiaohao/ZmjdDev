using HJDAPI.Common.Helpers;
using HJDAPI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.APIProxy
{
    public class Mail : BaseProxy
    {
        public int UpdateEdmUsersState(long idx, int state)
        {
            string url = APISiteUrl + "api/Mail/UpdateEdmUsersState";
            string postDataStr = string.Format("idx={0}&state={1}", idx, state);
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<int>(json);
        }
    }
}
