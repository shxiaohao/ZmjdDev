using HJD.ADServices.Contract;
using HJD.CouponService.Contracts.Entity;
using HJDAPI.Common.Helpers;
using HJDAPI.Controllers.Adapter;
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
    public class Activity : BaseProxy
    {
        public static string GetInvitedCode(ActivityParam p)
        {
            if (IsProductEvn)
                return ActivityAdapter.GetInvitedCode(p);
            else
            {
                string url = APISiteUrl + "api/Activity/GetInvitedCode";
                CookieContainer cc = new CookieContainer();
                string json = HttpRequestHelper.PostJson(url, p, ref cc);
                return JsonConvert.DeserializeObject<string>(json);
            }
        }

       
        public static ActivePageEntity GetActivePageByIDX(int idx)
        {
            if (IsProductEvn)
                return ActivityAdapter.GetActivePageByIDX(idx);
            else
            {
                string url = APISiteUrl + "api/Activity/GetActivePageByIDX";
                CookieContainer cc = new CookieContainer();
                string pars = "idx=" + idx.ToString();
                string json = HttpRequestHelper.Get(url, pars, ref cc);
                return JsonConvert.DeserializeObject<ActivePageEntity>(json);
            }
        }

        public static ActivePageTemplateEntity GetActivePageTemplateByID(int id)
        {
            if (IsProductEvn)
                return ActivityAdapter.GetActivePageTemplateByID(id);
            else
            {
                string url = APISiteUrl + "api/Activity/GetActivePageTemplateByID";
                CookieContainer cc = new CookieContainer();
                string pars = "id=" + id.ToString();
                string json = HttpRequestHelper.Get(url, pars, ref cc);
                return JsonConvert.DeserializeObject<ActivePageTemplateEntity>(json);
            }
        }
        public static int JoinZmjdGetCoupon(string phone, long sourceId = 0)
        {
            string url = APISiteUrl + "api/Activity/JoinZmjdGetCoupon";
            CookieContainer cc = new CookieContainer();
            string pars = string.Format("phone={0}&sourceId={1}", phone, sourceId);
            string json = HttpRequestHelper.Get(url, pars, ref cc);
            return JsonConvert.DeserializeObject<int>(json);
        }

        public static int GetOrgCouponCount(long userid, CouponActivityCode type)
        {
            string url = APISiteUrl + "api/Activity/GetOrgCouponCount";
            CookieContainer cc = new CookieContainer();
            string pars = string.Format("phone={0}&sourceId={1}", userid, type);
            string json = HttpRequestHelper.Get(url, pars, ref cc);
            return JsonConvert.DeserializeObject<int>(json);
        }
    }
}