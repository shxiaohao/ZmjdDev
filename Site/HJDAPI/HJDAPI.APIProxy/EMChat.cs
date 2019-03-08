using HJDAPI.Common.Helpers;
using HJDAPI.Controllers.Adapter;
using HJDAPI.Controllers.Common;
using HJDAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.APIProxy
{
    public  class EMChat: BaseProxy
    {
        public string SendTxtMessageToUser(MagiCallTxtMsgEntity m)
        {

            if (IsProductEvn)
                return EMChatHelper.SendTxtMessageToUser(m);
            else
            {
                string url = APISiteUrl + "api/EMChatApi/SendTxtMessageToUser";

                CookieContainer cc = new CookieContainer();

                string json = HttpRequestHelper.PostJson(url, m, ref cc);
                return json;
            }
        }
        public string SendPicTxtMessageToUser(MagiCallTxtMsgEntity m)
        {

            if (IsProductEvn)
                return EMChatHelper.SendPicTxtMessageToUser(m);
            else
            {
                string url = APISiteUrl + "api/EMChatApi/SendPicTxtMessageToUser";

                CookieContainer cc = new CookieContainer();

                string json = HttpRequestHelper.PostJson(url, m, ref cc);
                return json;
            }
        }
        public string AccountCreate(long userID)
        {

            if (IsProductEvn)
                return EMChatHelper.AccountCreate(userID);
            else
            {
                string url = APISiteUrl + "api/EMChatApi/AccountCreate";
                string postDataStr = string.Format("userID={0}"
                  , userID);

                CookieContainer cc = new CookieContainer();
                string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
                return json;
            }
        }
    }
}
