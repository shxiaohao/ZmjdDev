using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

using HJD.DestServices.Contract;
using HJD.HotelServices;
using HJD.HotelServices.Contracts;
using HJDAPI.APIProxy;
using HJDAPI.Models;

using HJD.HotelPrice.Contract.DataContract.Order;
using HJD.HotelPrice.Contract;
using Newtonsoft.Json;
using WHotelSite.Params;
using PersonalServices.Contract;
using WHotelSite.Common;
using HJD.AccountServices.Entity;
using WHotelSite.Params.Account;

namespace WHotelSite.Controllers
{
    public class FundController : BaseController
    {
        public const string AccessProtocal_IsApp = "whotelapp://www.zmjiudian.com/";
        public const string AccessProtocalPage_IsApp = "whotelapp://gotopage?url=http://www.zmjiudian.com/";

        public const string AccessProtocal_UnApp = "/inspector/jump?jumpurl=whotelapp://www.zmjiudian.com/";  //"http://www.zmjiudian.com/hotel/";  //"http://m1.zmjiudian.com/";
        public const string AccessProtocalPage_UnApp = "/inspector/jump?jumpurl=whotelapp://www.zmjiudian.com/gotopage?url=http://www.zmjiudian.com/";
        //public const string AccessProtocalPage_UnApp = "/inspector/jump?jumpurl=whotelapp://www.zmjiudian.com/gotopage?url=http://192.168.1.22:8081/";

        /// <summary>
        /// 好友推荐
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public ActionResult UserRecommend(int userid = 0)
        {
            if (IsApp())
            {
                ViewBag.AccessProtocal = AccessProtocal_IsApp;
                ViewBag.AccessProtocalPage = AccessProtocalPage_IsApp;
            }
            else
            {
                ViewBag.AccessProtocal = AccessProtocal_UnApp;
                ViewBag.AccessProtocalPage = AccessProtocalPage_UnApp;
            }

            ViewBag.UserId = userid;

            //获取分享跟踪加密参数
            var scp = new GenTrackCodeParam();
            scp.UserID = userid;
            scp.BizType = ZMJDShareTrackBizType.recommendfriend;
            var shareCode = Comment.GenTrackCodeResult4Share(scp);

            //string originStr = HJDAPI.Common.Security.DES.Decrypt(shareCode.EncodeStr);

            var s_url = "http://www.zmjiudian.com/Active/GenCashCoupon?sid=" + shareCode.EncodeStr;
            s_url = new Access().GenShortUrl(0, HttpUtility.UrlEncode(s_url));

            //分享链接
            var shareLink = "whfriend://comment?title={0}&photoUrl={1}&shareLink={2}&nextUrl={3}&content={4}&shareType={5}";
            shareLink = string.Format(
                shareLink
                , HttpUtility.UrlEncode("度假订酒店，又好又划算")
                , HttpUtility.UrlEncode("http://whfront.b0.upaiyun.com/app/img/zmjd-logo-x167.png")
                , HttpUtility.UrlEncode(s_url)
                , ""
                , HttpUtility.UrlEncode("1000+精选度假酒店，更具性价比超低优惠，更有人情味的管家式客服")
                , 0);
            ViewBag.ShareLink = shareLink;

            return View();
        }

        public ActionResult UserRecommend2(int userid = 0)
        {
            if (IsApp())
            {
                ViewBag.AccessProtocal = AccessProtocal_IsApp;
                ViewBag.AccessProtocalPage = AccessProtocalPage_IsApp;
            }
            else
            {
                ViewBag.AccessProtocal = AccessProtocal_UnApp;
                ViewBag.AccessProtocalPage = AccessProtocalPage_UnApp;
            }

            ViewBag.UserId = userid;

            //获取分享跟踪加密参数
            var scp = new GenTrackCodeParam();
            scp.UserID = userid;
            scp.BizType = ZMJDShareTrackBizType.recommendfriend;
            var shareCode = Comment.GenTrackCodeResult4Share(scp);

            //获取当前用户信息
            var userInfo = account.GetUserInfoByUserID(userid);
            ViewBag.UserInfo = userInfo ?? new UserInfoResult();

            //解析跟踪分享者信息
            HJD.AccountServices.Entity.MemberProfileInfo shareUserInfo = HJDAPI.APIProxy.account.GetCurrentUserInfo(userid) ?? new HJD.AccountServices.Entity.MemberProfileInfo();
            ViewBag.ShareUserInfo = shareUserInfo;

            //string originStr = HJDAPI.Common.Security.DES.Decrypt(shareCode.EncodeStr);

            //相关分享参数
            //var s_title = string.Format("你的朋友{0}送你50积分", (shareUserInfo != null && !string.IsNullOrEmpty(shareUserInfo.NickName)) ? shareUserInfo.NickName : "");
            var s_title = string.Format("你的朋友{0}邀你注册", (shareUserInfo != null && !string.IsNullOrEmpty(shareUserInfo.NickName)) ? shareUserInfo.NickName : "");
            var s_poto_url = "http://whfront.b0.upaiyun.com/app/img/zmjd-logo-x167.png";
            if (userInfo != null && !string.IsNullOrEmpty(userInfo.Avatar) && !userInfo.Avatar.Contains("115CN1V08W"))
	        {
                s_poto_url = userInfo.Avatar.Replace("_jupiter", "_290x290s");
	        }
            var s_url = "http://www.zmjiudian.com/Active/GenCashCouponForInvitation?sid=" + shareCode.EncodeStr;
            s_url = HttpUtility.UrlEncode(s_url);
            var s_content = "1000+精选度假酒店，更具性价比超低优惠，更有人情味的管家式客服";

            //原生分享链接
            var shareLink = "whfriend://comment?title={0}&photoUrl={1}&shareLink={2}&nextUrl={3}&content={4}&shareType={5}";
            shareLink = string.Format(shareLink, HttpUtility.UrlEncode(s_title), HttpUtility.UrlEncode(s_poto_url), HttpUtility.UrlEncode(s_url), "", HttpUtility.UrlEncode(s_content), "{0}");
            ViewBag.ShareLink = string.Format(shareLink, 0);

            //微信好友
            var shareLink_WeixinFriend = string.Format(shareLink, 1);
            ViewBag.ShareLink_WeixinFriend = shareLink_WeixinFriend;

            //微信朋友圈
            var shareLink_WeixinLoop = string.Format(shareLink, 2);
            ViewBag.ShareLink_WeixinLoop = shareLink_WeixinLoop;

            //QQ好友
            var shareLink_QqFriend = string.Format(shareLink, 5);
            ViewBag.ShareLink_QqFriend = shareLink_QqFriend;

            //QQ空间
            var shareLink_QqZone = string.Format(shareLink, 3);
            ViewBag.ShareLink_QqZone = shareLink_QqZone;

            //新浪
            var shareLink_Sina = string.Format(shareLink, 4);
            ViewBag.ShareLink_Sina = shareLink_Sina;

            //复制链接
            var shareLink_CopyLink = string.Format(shareLink, 6);
            ViewBag.ShareLink_CopyLink = shareLink_CopyLink;

            //更多
            var shareLink_More = string.Format(shareLink, 7);
            ViewBag.ShareLink_More = shareLink_More;

            //打开原生分享bar
            var shareLink_Native = string.Format(shareLink, 0);
            ViewBag.ShareLink_Native = shareLink_Native;

            //当前系统环境（ios | android）
            ViewBag.AppType = AppType();

            //检查当前版本是否大于等于4.4版本
            ViewBag.IsThanVer4_4 = IsThanVer4_4();

            #region 行为记录

            var value = string.Format("{{\"userid\":\"{0}\"}}", userid);
            RecordBehavior("UserRecommend2Load", value);

            #endregion

            return View();
        }

        /// <summary>
        /// 推荐规则
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public ActionResult RecommendRule(int userid = 0)
        {
            return View();
        }
    }
}