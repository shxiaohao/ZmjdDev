using HJD.AccountServices.Entity;
using HJD.CouponService.Contracts.Entity;
using HJD.HotelManagementCenter.Domain;
using HJD.HotelManagementCenter.Domain.Fund;
using HJD.HotelPrice.Contract.DataContract.Order;
using HJD.HotelServices.Contracts;
using HJD.WeixinServices.Contracts;
using HJDAPI.APIProxy;
using HJDAPI.Common.Helpers;
using HJDAPI.Common.Security;
using HJDAPI.Models;
using Newtonsoft.Json;
using ProductService.Contracts.Entity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WHotelSite.Common;
using WHotelSite.Models;

namespace WHotelSite.Controllers
{
    /// <summary>
    /// 渠道处理
    /// </summary>
    public class ChannelController : BaseController
    {
        /// <summary>
        /// 店铺明细
        /// </summary>
        /// <param name="cid"></param>
        /// <returns></returns>
        public ActionResult ShopDetail(int cid = 0)
        {
            //isMobile
            var isMobile = Utils.IsMobile();
            ViewBag.IsMobile = isMobile;

            ViewBag.CID = cid;

            //获取产品店铺信息
            var productShopInfo = new RetailerShopEntity();
            if (cid > 0)
            {
                productShopInfo = Shop.GetRetailerShopByCID(cid);
            }
            ViewBag.ProductShopInfo = productShopInfo;

            return View();
        }

        /// <summary>
        /// 度假伙伴主页
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public ActionResult Partners(long userid = 0)
        {
            //获取请求参数中的CID
            var requestCID = GetCurCIDForRequest();
            ViewBag.RequestCID = requestCID;

            //isMobile
            var isMobile = Utils.IsMobile();
            ViewBag.IsMobile = isMobile;

            //Weixin
            bool isInWeixin = Request.UserAgent.IndexOf("MicroMessenger") > 0 ? true : false;
            ViewBag.isInWeixin = isInWeixin;

            var isApp = IsApp();
            ViewBag.IsApp = isApp;

            var curUserID = Convert.ToInt64(UserState.UserID > 0 ? UserState.UserID : userid);
            if (curUserID <= 0)
            {
                curUserID = _ContextBasicInfo.AppUserID;
            }
            if (curUserID >= 0 && _ContextBasicInfo.AppUserID <= 0)
            {
                _ContextBasicInfo.AppUserID = curUserID;
            }
            ViewBag.UserId = curUserID;
            userid = curUserID;

            //是否VIP
            var isVip = account.IsVIPCustomer(new AccountInfoItem { Uid = userid.ToString() });
            ViewBag.IsVip = isVip;

            //查询当前用户的度假伙伴状态
            var partnerResult = Partner.GetRetailerInvate(userid);
            ViewBag.PartnerResult = partnerResult;


            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public ActionResult ApplyPartner(long userid = 0)
        {
            //获取请求参数中的CID
            var requestCID = GetCurCIDForRequest();
            ViewBag.RequestCID = requestCID;

            //isMobile
            var isMobile = Utils.IsMobile();
            ViewBag.IsMobile = isMobile;

            //Weixin
            bool isInWeixin = Request.UserAgent.IndexOf("MicroMessenger") > 0 ? true : false;
            ViewBag.isInWeixin = isInWeixin;

            var isApp = IsApp();
            ViewBag.IsApp = isApp;

            var curUserID = Convert.ToInt64(UserState.UserID > 0 ? UserState.UserID : userid);
            if (curUserID <= 0)
            {
                curUserID = _ContextBasicInfo.AppUserID;
            }
            if (curUserID >= 0 && _ContextBasicInfo.AppUserID <= 0)
            {
                _ContextBasicInfo.AppUserID = curUserID;
            }
            ViewBag.UserId = curUserID;
            userid = curUserID;

            //是否VIP
            var isVip = account.IsVIPCustomer(new AccountInfoItem { Uid = userid.ToString() });
            ViewBag.IsVip = isVip;

            //获取当前用户信息
            var userInfo = account.GetUserInfoByUserID(userid);
            ViewBag.UserInfo = userInfo ?? new UserInfoResult();

            //查询当前用户的度假伙伴状态
            var partnerResult = Partner.GetRetailerInvate(userid);
            ViewBag.PartnerResult = partnerResult;

            return View();
        }


        #region 针对度假伙伴，通过尚旅周末获取微信信息的中转页面

        /// <summary>
        /// 分销尚旅周末微信中转页面
        /// </summary>
        /// <param name="redurl"></param>
        /// <param name="userId"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public ActionResult ChannelTransferForBg(string redurl = "", long userId = 0, string code = "")
        {
            //Weixin
            bool isInWeixin = Request.UserAgent.IndexOf("MicroMessenger") > 0 ? true : false;
            ViewBag.isInWeixin = isInWeixin;

            #region 微信环境下，做微信静默授权

            WeixinUserInfo weixinUserInfo = new WeixinUserInfo();
            var _openid = "";
            var _unionid = "";
            if (isInWeixin)
            {
                if (!string.IsNullOrEmpty(code))
                {
                    var weixinAcountName = "shanglvzhoumo";

                    //通过code换取网页授权access_token
                    var accessToken = WeiXinHelper.GetWeixinAccessToken(code);
                    if (accessToken != null && !string.IsNullOrEmpty(accessToken.Openid))
                    {
                        //获取用户信息
                        //通过access_token拉取用户信息(需scope为 snsapi_userinfo)（这是通过微信api获取的真实用户信息）
                        weixinUserInfo = WeiXinHelper.GetWeixinUserInfo(accessToken.AccessToken, accessToken.Openid);
                        if (weixinUserInfo != null && !string.IsNullOrEmpty(weixinUserInfo.Openid))
                        {
                            _openid = weixinUserInfo.Openid;
                            _unionid = weixinUserInfo.Unionid;
                            if (string.IsNullOrEmpty(_unionid) && !string.IsNullOrEmpty(accessToken.Unionid))
                            {
                                _unionid = accessToken.Unionid;
                            }

                            //存储微信用户
                            try
                            {
                                //insert
                                var w = new WeixinUser
                                {
                                    Openid = weixinUserInfo.Openid,
                                    Unionid = weixinUserInfo.Unionid,
                                    Nickname = weixinUserInfo.Nickname,
                                    Sex = weixinUserInfo.Sex.ToString(),
                                    Province = weixinUserInfo.Province,
                                    City = weixinUserInfo.City,
                                    Country = weixinUserInfo.Country,
                                    Headimgurl = weixinUserInfo.Headimgurl,
                                    Privilege = "",
                                    Phone = "",
                                    Remark = "",
                                    GroupId = "0",
                                    Subscribe = 0,
                                    WeixinAcount = weixinAcountName,   //WeiXinChannelCode.周末酒店服务号
                                    Language = "zh_CN",
                                    SubscribeTime = DateTime.Now,
                                    CreateTime = DateTime.Now
                                };
                                var update = new Weixin().UpdateWeixinUserSubscribe(w);
                            }
                            catch (Exception ex)
                            {

                            }

                            #region 处理微信号绑定用户信息相关操作

                            //微信环境下，如果当前已经登录，则直接绑定
                            if (userId > 0)
                            {
                                WeiXinHelper.BindingWxAndUid(userId, weixinUserInfo.Unionid);
                            }

                            #endregion
                        }
                    }
                    else
                    {
                        //如果code有值，但不能获取，一般说明code过期了，那么重新获取
                        var weixinGoUrl = WeiXinHelper.GenConfrimAuthorUrlByWeixinAcount(HJD.WeixinService.Contract.WeiXinChannelCode.周末酒店服务号, HttpUtility.UrlEncode(string.Format("http://www.shang-ke.cn/channel/ChannelTransferForBg?redurl={0}&userid={1}", redurl, userId)));
                        return Redirect(weixinGoUrl);
                    }
                }
                else
                {
                    //授权页面
                    var weixinGoUrl = WeiXinHelper.GenConfrimAuthorUrlByWeixinAcount(HJD.WeixinService.Contract.WeiXinChannelCode.周末酒店服务号, HttpUtility.UrlEncode(string.Format("http://www.shang-ke.cn/channel/ChannelTransferForBg?redurl={0}&userid={1}", redurl, userId)));
                    return Redirect(weixinGoUrl);
                }
            }

            #endregion
            
            if (redurl.Contains("?"))
            {
                redurl = string.Format("{0}&_openid={1}&_unionid={2}", redurl, _openid, _unionid);
            }
            else
            {
                redurl = string.Format("{0}?_openid={1}&_unionid={2}", redurl, _openid, _unionid);
            }

            return Redirect(redurl);
        }

        #endregion
    }
}