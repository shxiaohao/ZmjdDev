
using HJDAPI.Common.Helpers;
using HJDAPI.Controllers.Adapter;
using HJDAPI.Controllers.Common;
using HJDAPI.Models;
using MagiCallService.Contracts.Model.Dialog;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.APIProxy
{
    public  class MagiCall: BaseProxy
    {
        public string GetGreeting(long userID, bool isVIPUser)
        {
            string url = APISiteUrl + "api/MagiCall/GetGreeting";
            CookieContainer cc = new CookieContainer();
            string pars = string.Format("userId={0}&isVIPUser={1}", userID, isVIPUser);
            return HttpRequestHelper.Get(url, pars, ref cc);
        }


        public string MagiCallClientHeart(MessageEntity message)
        {
           string url = APISiteUrl + "api/MagiCall/MagiCallClientHeart";
           CookieContainer cc = new CookieContainer();
           return HttpRequestHelper.PostJson(url, message, ref cc);
        }

        public string MagiCallClientMessage(MessageEntity message)
        {
            string url = APISiteUrl + "api/MagiCall/MagiCallClientMessage";
            CookieContainer cc = new CookieContainer();
            return HttpRequestHelper.PostJson(url, message, ref cc);
        }

        public bool IsMagiCallUser(long userId)
        {
            string url = APISiteUrl + "api/MagiCall/IsMagiCallUser";
            CookieContainer cc = new CookieContainer();
            string postData = string.Format("userId={0}", userId);
            string json = HttpRequestHelper.Get(url, postData, ref cc);
            return JsonConvert.DeserializeObject<bool>(json);
        }

        public UserCustomerCareInfoEntity GetLastUserCustomerCareInfo(long userID)
        {
            if (IsProductEvn)
            {
                return new MagiCallAdapter().GetLastUserCustomerCareInfo(userID);
            }
            else
            {
                string url = APISiteUrl + "api/MagiCall/GetLastUserCustomerCareInfo";
                CookieContainer cc = new CookieContainer();
                string postDataStr = string.Format("UserID={0}", userID);
                string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
                return JsonConvert.DeserializeObject<UserCustomerCareInfoEntity>(json);
            }
        }
        public CustomerCareEntity GetCustomerCareByUserID(long UserID)
        {
            if (IsProductEvn)
            {
              return   new MagiCallAdapter().GetCustomerCareByUserID(UserID); 
            }
            else
            {
                string url = APISiteUrl + "api/MagiCall/GetCustomerCareByUserID";
                CookieContainer cc = new CookieContainer();
                string postDataStr = string.Format("UserID={0}", UserID);
                string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
                return JsonConvert.DeserializeObject<CustomerCareEntity>(json);
            }
        }

    }
}
