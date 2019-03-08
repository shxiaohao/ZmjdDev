using HJD.AccountServices.Entity;
using HJD.ADServices.Contract;
using HJD.CouponService.Contracts;
using HJD.CouponService.Contracts.Entity;
using HJD.HotelManagementCenter.Domain;
using HJD.Framework.WCF;
using HJDAPI.Common.Security;
using HJDAPI.Controllers.Adapter;
using HJDAPI.Controllers.Common;
using HJDAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;


namespace HJDAPI.Controllers
{
    public class ActivityController : BaseApiController
    {
        static ICouponService couponService = ServiceProxyFactory.Create<ICouponService>("ICouponService");

        [HttpPost]
        public string GetActivityUrl(ActivityParam p)
        {
            if (p.userID == 0)
            {
                return "";
            }
            //MemberProfileInfo info = AccountAdapter.GetCurrentUserInfo(p.userID);
            //if (info == null || string.IsNullOrEmpty(info.NickName))
            //{
            //    return "";
            //}
            if (Signature.IsRightSignature(p.TimeStamp, p.SourceID, p.RequestType, p.Sign))
            {
                return System.Configuration.ConfigurationManager.AppSettings[p.activityUrlType];
            }
            else
            {
                return "";
            }
        }

        [HttpPost]
        public ActivityResult GetActivityShareResult(ActivityParam p)
        {
            if (p.userID == 0 || p.mediaType == 0)
            {
                return new ActivityResult();
            }
            //MemberProfileInfo info = AccountAdapter.GetCurrentUserInfo(p.userID);
            //if (info == null || string.IsNullOrEmpty(info.NickName))
            //{
            //    return new ActivityResult();
            //}
            if (Signature.IsRightSignature(p.TimeStamp, p.SourceID, p.RequestType, p.Sign))
            {
                string content = "";
                string yaoqingCode = ActivityAdapter.GetInvitedCode(p);
                int price = 50;
                string activityInfoUrl = System.Configuration.ConfigurationManager.AppSettings["ActivitySite"] + "/Activity/" + p.activityID;
                //不同媒体 分享信息不一样
                switch (p.mediaType)
                {
                    case 1:
                        content = string.Format("周末酒店APP推荐的酒店很不错，适合休闲度假，推荐下载。输入邀请代码{0}，可以有{1}元返现。", yaoqingCode, price);
                        break;
                    case 2:
                        content = string.Format("周末酒店APP推荐的酒店很不错，适合休闲度假。下载注册后，输入邀请代码{0}，我们都可以有{1}元返现。", yaoqingCode, price);
                        break;
                    case 3:
                        content = string.Format("周末酒店APP推荐的酒店很不错，适合休闲度假，推荐下载。输入邀请代码{0}，可以有{1}元返现。", yaoqingCode, price);
                        break;
                    default: break;
                }
                return new ActivityResult() { Content = content, title = "邀请朋友", photoUrl = "http://www.zmjiudian.com/Content/images/logo.png", shareLink = activityInfoUrl };
            }
            else
            {
                return new ActivityResult();
            }
        }

        [HttpPost]
        public string GetInvitedCode(ActivityParam p)
        {
            if (p.userID == 0)
            {
                return "0";
            }
            //MemberProfileInfo info = AccountAdapter.GetCurrentUserInfo(p.userID);
            //if (info == null || string.IsNullOrEmpty(info.NickName))
            //{
            //    return "";
            //}
            if (Signature.IsRightSignature(p.TimeStamp, p.SourceID, p.RequestType, p.Sign))
            {
                return ActivityAdapter.GetInvitedCode(p);
            }
            else
            {
                return "0";
            }
        }

        [HttpGet]
        [HttpPost]
        public ActivePageEntity GetActivePageByIDX(int idx)
        {
            return ActivityAdapter.GetActivePageByIDX(idx);
        }

        [HttpGet]
        [HttpPost]
        public ActivePageTemplateEntity GetActivePageTemplateByID(int id)
        {
            return ActivityAdapter.GetActivePageTemplateByID(id);
        }

        //[HttpGet]
        //public ActivityResult GetCouponShare(CouponModelParam param)
        //{
        //    if (param != null && param.)
        //    {

        //    }

        //}

        /// <summary>
        /// 加入ZMJD并获取现金券
        /// </summary>
        /// <param name="phone"></param>
        /// <returns>0 成功 1 失败 2 手机号已存在 3 手机号错误</returns>
        [HttpGet]
        [HttpPost]
        public int JoinZmjdGetCoupon(string phone, long sourceId = 0, long CID = 0)
        {
            if (!CommMethods.IsPhone(phone))
            {
                //手机号错误
                return 3;
            }

            User_Info ui = AccountAdapter.GetOrRegistPhoneUser(phone, CID);
            CouponTypeDefineEntity ctd = couponService.GetCouponTypeDefineByCode(CouponActivityCode.cashcoupon50);
            List<OriginCoupon> lo = couponService.GetUserOrgCouponInfoByType(ui.UserId, ctd.Type);
            if (lo.Count > 0)
            {
                //手机号已存在
                return 2;
            }
            else
            {
                //发现金券（注册类型，50rmb）
                OriginCouponResult ocr = couponService.GenerateOriginCoupon3(ui.UserId, ctd.Type, phone, sourceId);
                if (ocr.Success == 1)
                {
                    //失败
                    return 1;
                }
            }

            //成功
            return 0;
        }

        public int GetOrgCouponCount(long userId, CouponActivityCode type)
        {
            CouponTypeDefineEntity ctd = couponService.GetCouponTypeDefineByCode(type);
            List<OriginCoupon> lo = couponService.GetUserOrgCouponInfoByType(userId, ctd.Type);
            return lo == null ? 0 : lo.Count;
        }


        /// <summary>
        /// 度假伙伴-活动海报查询
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [HttpPost]
        public List<ChannelPosterActiveEntity> GetChannelActiveQueryAll()
        {
            return CommAdapter.GetAllChannelActive();

        }


        /// <summary>
        /// 根据id查询
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [HttpPost]
        public ChannelPosterActiveEntity GetChannelActiveQueryByID(int id)
        {
            return CommAdapter.GetChannelActiveByID(id);

        }

    }
}
