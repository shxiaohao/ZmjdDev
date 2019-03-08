using HJDAPI.Common.Helpers;
using HJDAPI.Models;
using MessageService.Contract;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.APIProxy
{
    public class Notice : BaseProxy
    {

        public SendAppNoticeResponse SentMagiCallClientMessage(SendNoticeEntity message)
       {                 
           return SendNoticeToApp( message);
       }

        public SendAppNoticeResponse SendNoticeToApp(SendNoticeEntity message)
        {
            string url = APISiteUrl + "api/Notice/SendAppNotice";
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.PostJson(url, message, ref cc);

            SendAppNoticeResponse result = JsonConvert.DeserializeObject<SendAppNoticeResponse>(json);

            return result;
        }

        public SendAppNoticeResponse SendNoticeToAllApp(SendNoticeEntity message)
        {
            string url = APISiteUrl + "api/Notice/SendAllAppNotice";
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.PostJson(url, message, ref cc);

            SendAppNoticeResponse result = JsonConvert.DeserializeObject<SendAppNoticeResponse>(json);

            return result;
        }
    }
}
