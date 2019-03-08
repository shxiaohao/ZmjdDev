using HJD.AccountServices.Entity;
using HJD.CouponService.Contracts.Entity;
using HJD.HotelManagementCenter.Domain;
using HJD.HotelManagementCenter.Domain.Fund;
using HJD.HotelPrice.Contract.DataContract.Order;
using HJD.HotelServices.Contracts;
using HJDAPI.APIProxy;
using HJDAPI.Common.Security;
using HJDAPI.Models;
using Newtonsoft.Json;
using ProductService.Contracts.Entity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WHotelSite.Common;
using WHotelSite.Models;

namespace WHotelSite.Controllers
{
    public class ProductController : BaseController
    {
        #region 机加酒相关

        /// <summary>
        /// 套餐产品（机加酒）
        /// </summary>
        /// <param name="skuid"></param>
        /// <param name="userid"></param>
        /// <param name="sid"></param>
        /// <returns></returns>
        public ActionResult PackageProduct(int skuid, string userid = "0", string sid = "")
        {
            var isApp = IsApp();
            ViewBag.IsApp = isApp;
            ViewBag.SKUID = skuid;

            var curUserID = UserState.UserID > 0 ? UserState.UserID.ToString(): userid;
            ViewBag.UserId = curUserID;

            //调试使用
            ViewBag.AppType = AppType();
            if (AppType() == "iosapp") ViewBag.AppVerForIOS = AppVerForIOS();
            else ViewBag.AppVerForIOS = "";
            if (AppType() == "android") ViewBag.AppVerForAndroid = AppVerForAndroid();
            else ViewBag.AppVerForAndroid = "";
            ViewBag.IsLatestVerApp = IsLatestVerApp();

            //Weixin
            bool isInWeixin = Request.UserAgent.IndexOf("MicroMessenger") > 0 ? true : false;
            ViewBag.isInWeixin = isInWeixin;

            //获取券SKU信息
            var couponSkuInfo = Coupon.GetSKUCouponActivityDetail(skuid);
            ViewBag.CouponSkuInfo = couponSkuInfo;

            //如果当前抢购VIP专享，则判断当前用户是否为VIP
            var isVip = account.IsVIPCustomer(new AccountInfoItem { Uid = curUserID });
            ViewBag.IsVip = isVip;

            //isMobile
            var isMobile = Utils.IsMobile();
            ViewBag.IsMobile = isMobile;

            //获取当前用户信息
            var userInfo = account.GetUserInfoByUserID(Convert.ToInt64(curUserID));
            ViewBag.UserInfo = userInfo ?? new UserInfoResult();

            ////查询当前券和用户的可售状态
            //var buyResult = new Coupon().IsExchangeCouponSoldOut(couponSkuInfo.activity.ID, Convert.ToInt64(curUserID));
            //ViewBag.BuyResult = buyResult;

            //stype (1wap/2web/3app/4weixin)
            var stype = ProductServiceEnums.SceneType.WEB;
            if (isInWeixin)
            {
                stype = ProductServiceEnums.SceneType.WEIXIN;
            }
            else if (isApp)
            {
                stype = ProductServiceEnums.SceneType.APP;
            }
            else if (isMobile)
            {
                stype = ProductServiceEnums.SceneType.WAP;
            }
            ViewBag.SType = stype;

            #region 分享配置

            var shareLink = "";
            MemberProfileInfo shareUserInfo = new HJD.AccountServices.Entity.MemberProfileInfo();

            if (!string.IsNullOrEmpty(sid))
            {
                //解析分享跟踪参数
                try
                {
                    long sourceId = 0;
                    string originStr = HJDAPI.Common.Security.DES.Decrypt(sid.Replace(" ", "+"));
                    var originObj = JsonConvert.DeserializeObject<GenTrackCodeParam>(originStr);
                    if (sourceId <= 0 && originObj.UserID > 0)
                    {
                        sourceId = originObj.UserID;
                    }

                    shareUserInfo = HJDAPI.APIProxy.account.GetCurrentUserInfo(sourceId) ?? new HJD.AccountServices.Entity.MemberProfileInfo();
                }
                catch (Exception ex)
                {

                }
            }
            else
            {
                //分享跟踪参数
                var scp = new GenTrackCodeParam();
                scp.UserID = Convert.ToInt64(curUserID);
                scp.CouponActivityID = skuid;
                scp.BizType = ZMJDShareTrackBizType.roomcoupondetail;
                var shareCode = Comment.GenTrackCodeResult4Share(scp);

                //分享链接生成
                shareLink = string.Format("http://www.zmjiudian.com/coupon/product/{0}?sid={1}", skuid, shareCode.EncodeStr);
            }

            ViewBag.ShareLink = shareLink;
            ViewBag.ShareUserInfo = shareUserInfo;

            #endregion           

            return View();
        }

        /// <summary>
        /// 确认订单配置信息
        /// </summary>
        /// <param name="skuid"></param>
        /// <param name="date"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public ActionResult PackageConfirm(int skuid, string date, string userid = "0") 
        {
            var isApp = IsApp();
            ViewBag.IsApp = isApp;
            ViewBag.SKUID = skuid;

            var curUserID = UserState.UserID > 0 ? UserState.UserID.ToString() : userid;
            ViewBag.UserId = curUserID;

            //调试使用
            ViewBag.AppType = AppType();
            if (AppType() == "iosapp") ViewBag.AppVerForIOS = AppVerForIOS();
            else ViewBag.AppVerForIOS = "";
            if (AppType() == "android") ViewBag.AppVerForAndroid = AppVerForAndroid();
            else ViewBag.AppVerForAndroid = "";
            ViewBag.IsLatestVerApp = IsLatestVerApp();

            //Weixin
            bool isInWeixin = Request.UserAgent.IndexOf("MicroMessenger") > 0 ? true : false;
            ViewBag.isInWeixin = isInWeixin;

            //isMobile
            var isMobile = Utils.IsMobile();
            ViewBag.IsMobile = isMobile;

            //stype (1wap/2web/3app/4weixin)
            var stype = ProductServiceEnums.SceneType.WEB;
            if (isInWeixin)
            {
                stype = ProductServiceEnums.SceneType.WEIXIN;
            }
            else if (isApp)
            {
                stype = ProductServiceEnums.SceneType.APP;
            }
            else if (isMobile)
            {
                stype = ProductServiceEnums.SceneType.WAP;
            }
            ViewBag.SType = stype;



            return View();
        }

        /// <summary>
        /// 订单确认提交页
        /// </summary>
        /// <param name="skuid"></param>
        /// <param name="date"></param>
        /// <param name="bnum"></param>
        /// <param name="cnum"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public ActionResult PackageBook(int skuid, string date, int bnum, int cnum, string userid = "0")
        {
            var isApp = IsApp();
            ViewBag.IsApp = isApp;
            ViewBag.SKUID = skuid;

            var curUserID = UserState.UserID > 0 ? UserState.UserID.ToString() : userid;
            ViewBag.UserId = curUserID;

            //调试使用
            ViewBag.AppType = AppType();
            if (AppType() == "iosapp") ViewBag.AppVerForIOS = AppVerForIOS();
            else ViewBag.AppVerForIOS = "";
            if (AppType() == "android") ViewBag.AppVerForAndroid = AppVerForAndroid();
            else ViewBag.AppVerForAndroid = "";
            ViewBag.IsLatestVerApp = IsLatestVerApp();

            //Weixin
            bool isInWeixin = Request.UserAgent.IndexOf("MicroMessenger") > 0 ? true : false;
            ViewBag.isInWeixin = isInWeixin;

            //isMobile
            var isMobile = Utils.IsMobile();
            ViewBag.IsMobile = isMobile;

            //stype (1wap/2web/3app/4weixin)
            var stype = ProductServiceEnums.SceneType.WEB;
            if (isInWeixin)
            {
                stype = ProductServiceEnums.SceneType.WEIXIN;
            }
            else if (isApp)
            {
                stype = ProductServiceEnums.SceneType.APP;
            }
            else if (isMobile)
            {
                stype = ProductServiceEnums.SceneType.WAP;
            }
            ViewBag.SType = stype;



            return View();
        }

        /// <summary>
        /// 机酒购买检测
        /// </summary>
        /// <param name="id"></param>
        /// <param name="buynum"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public ActionResult CheckBuyNumberForPackageProduct(int id, int buynum, string userid = "0")
        {
            Dictionary<string, string> re = new Dictionary<string, string>();

            //test
            return Json(re, JsonRequestBehavior.AllowGet);

            //获取当前券的信息
            var coupon = Coupon.GetCouponActivityDetail(id);

            //如果当前抢购VIP专享，则判断当前用户是否为VIP
            if (coupon.activity.IsVipExclusive)
            {
                var isVip = false;
                if (userid != "0") isVip = account.IsVIPCustomer(new AccountInfoItem { Uid = userid });
                if (!isVip)
                {
                    re["Message"] = "该抢购只限VIP会员购买";
                    re["Success"] = "-1";
                    re["CanSell"] = "0";
                    return Json(re, JsonRequestBehavior.AllowGet);
                }
            }

            //查询当前券和用户的可售状态
            var buyResult = new Coupon().IsExchangeCouponSoldOut(id, Convert.ToInt64(userid));

            //首先检查当前券是否已经售完
            if (buyResult.ActivityState == 0)// || true
            {
                re["Message"] = "此兑换券已售完";
                re["Success"] = "0";
                re["CanSell"] = "0";
                return Json(re, JsonRequestBehavior.AllowGet);
            }

            //检查当前用户购买量是否超过了当前券规定单人购买限额
            //已经彻底不能购买了
            if (buyResult.UserBuyNum >= coupon.activity.SingleBuyNum)
            {
                re["Message"] = string.Format("此兑换券每人限购{0}套，您已购买过{1}套<br />请到\"我的钱包\"中打开\"房券\"进行支付", coupon.activity.SingleBuyNum, buyResult.UserBuyNum);
                re["Success"] = "1";
                re["CanSell"] = "1";
                return Json(re, JsonRequestBehavior.AllowGet);
            }

            //还可以购买，但不能买那么多了
            if (buynum + buyResult.UserBuyNum > coupon.activity.SingleBuyNum)
            {
                re["Message"] = string.Format("目前可订 {0} 套，此兑换券每人限购{1}套<br />请到\"我的钱包\"中打开\"房券\"进行支付", (coupon.activity.SingleBuyNum - buyResult.UserBuyNum), coupon.activity.SingleBuyNum);
                re["Success"] = "2";
                re["CanSell"] = (coupon.activity.SingleBuyNum - buyResult.UserBuyNum).ToString();
                return Json(re, JsonRequestBehavior.AllowGet);
            }

            //检查当前购买量是否超过了券的剩余购买量
            if (buynum > buyResult.ActivityNoLockNum)
            {
                if (buyResult.ActivityNoLockNum > 0)
                {
                    re["Message"] = string.Format("目前可订 {0} 套，部分订单已提交未支付，稍后可能有余单放出，请耐心等待", buyResult.ActivityNoLockNum);
                }
                else
                {
                    re["Message"] = string.Format("目前不可购买，部分订单已提交未支付，稍后可能有余单放出，请耐心等待", buyResult.ActivityNoLockNum);
                }
                re["Success"] = "3";
                re["CanSell"] = buyResult.ActivityNoLockNum.ToString();
                return Json(re, JsonRequestBehavior.AllowGet);
            }

            return Json(re, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 机酒购买提交
        /// </summary>
        /// <param name="aid"></param>
        /// <param name="atype"></param>
        /// <param name="pid"></param>
        /// <param name="pricetype"></param>
        /// <param name="paynum"></param>
        /// <param name="userid"></param>
        /// <param name="skuid"></param>
        /// <param name="stype"></param>
        /// <returns></returns>
        public ActionResult SubmitForPackageProduct(int aid, int atype, int skuid, int paynum, string userid, int stype)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();

            SubmitCouponOrderModel sub = new SubmitCouponOrderModel();
            sub.ActivityID = aid;
            sub.ActivityType = atype;
            sub.SKUID = skuid;
            sub.UserID = Convert.ToInt64(userid);
            sub.CID = GetCurCID();
            sub.SceneType = stype;
            sub.OrderItems = new List<ProductAndNum> 
            { 
                new ProductAndNum
                {
                    SourceID = 0,
                    Number = paynum,
                    Type = 1
                }
            };

            //是否大于等于5.4版本
            var isThanVer5_4 = IsThanVer5_4();
            ViewBag.IsThanVer5_4 = isThanVer5_4;

            //获取当前券的信息
            var coupon = Coupon.GetCouponActivityDetail(aid);

            //如果当前抢购VIP专享，则判断当前用户是否为VIP
            if (coupon.activity.IsVipExclusive)
            {
                var isVip = false;
                if (userid != "0") isVip = account.IsVIPCustomer(new AccountInfoItem { Uid = userid });
                if (!isVip)
                {
                    dict["Message"] = "该商品只限VIP会员购买";
                    dict["Success"] = "2";
                    dict["Url"] = "";
                    return Json(dict, JsonRequestBehavior.AllowGet);
                }
            }

            //获取券SKU信息
            var couponSkuInfo = Coupon.GetSKUCouponActivityDetail(skuid);

            //是否新VIP才能购买的产品
            if (couponSkuInfo.SKUInfo.SKU.TagsList != null
                && couponSkuInfo.SKUInfo.SKU.TagsList.Exists(_ => _.TagID == 1))
            {
                //获取用户是否VIP
                var isVip = account.IsVIPCustomer(new AccountInfoItem { Uid = userid });

                //获取当前用户信息
                var cusType = account.GetVIPTypeNum(new AccountInfoItem { Uid = userid });

                if (isVip)
                {
                    if ((HJDAPI.Common.Helpers.Enums.CustomerType)cusType == HJDAPI.Common.Helpers.Enums.CustomerType.vip199nr
                        || (HJDAPI.Common.Helpers.Enums.CustomerType)cusType == HJDAPI.Common.Helpers.Enums.CustomerType.vip599)
                    {
                        //获取用户权限
                        var _userInfoPrivileges = account.GetAllPrivilegeByUserId(new AccountInfoItem { Uid = userid });

                        //2010 首单爆款
                        if (!_userInfoPrivileges.Exists(_p => _p.PrivID == 2010))
                        {
                            dict["Message"] = "抱歉，此专享优惠套餐每位VIP会员限购一套哦，您之前已经购买过啦～";
                            dict["Success"] = "1";
                            dict["Url"] = "";
                            return Json(dict, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        dict["Message"] = "抱歉，此价格仅供新VIP会员专享哦～";
                        dict["Success"] = "1";
                        dict["Url"] = "";
                        return Json(dict, JsonRequestBehavior.AllowGet);
                    }
                }
                else 
                {
                    dict["Message"] = "抱歉，此价格为新VIP会员专享，成为VIP会员才能购买哦～";
                    dict["Success"] = "2";
                    dict["Url"] = "";
                    return Json(dict, JsonRequestBehavior.AllowGet);
                }

                //新VIP专享的套餐用户只能买一件
                if (paynum > 1)
                {
                    dict["Message"] = "抱歉，此专享优惠套餐每位VIP会员限购一套哦～";
                    dict["Success"] = "1";
                    dict["Url"] = "";
                    return Json(dict, JsonRequestBehavior.AllowGet);
                }
            }

            CouponOrderResult order = new Coupon().SubmitExchangeOrderForProduct(sub);
            if (order != null && order.Success == 0)
            {
                //铂涛的跳转页面 发送短信验证码
                var botaoCode = HJDAPI.Common.Helpers.Enums.ThirdPartyMerchantCode.bohuijinrong.ToString();
                var botaophone = ClientHelper.GetBotaoPhoneNumFromCookie(HttpContext.Request.Cookies);

                if (!string.IsNullOrWhiteSpace(botaophone))
                {
                    var phoneNum = botaophone;
                    var amount = (int)(order.Amount * 100);//元转分
                    var goodsInfo = order.GoodsInfo;
                    var orderId = order.OrderID;

                    var payWebSiteUrl = (string)System.Configuration.ConfigurationManager.AppSettings["PayWebSiteUrl"];
                    var completeUrl = string.Format("{0}Pay/BoTaoPayComplete/{1}/{2}", payWebSiteUrl, "coupon", orderId);

                    var sign = EncryptMethod.GenSignature4Pay(botaoCode, phoneNum, amount, order.OrderID, completeUrl);
                    dict["Message"] = "订单已提交";
                    dict["Success"] = "0";
                    var jumpUrl = string.Format("{7}pay/botao?mobileId={0}&merName={1}&goodsInf={2}&amount={3}&orderId={4}&retUrl={5}&sign={6}", phoneNum, botaoCode, goodsInfo, amount, order.OrderID, completeUrl, sign, payWebSiteUrl);
                    dict["Url"] = jumpUrl;
                }
                else if (IsLatestVerApp())
                {
                    var _timeStamp = Signature.GenTimeStamp();

                    var completeUrl = "";
                    if (isThanVer5_4)
                    {
                        completeUrl = Utils.GetAbsoluteUrl(Url, Url.Action("PayComplete", "Coupon", new { channel = "all", order = order.OrderID.ToString(), _t = _timeStamp, _isoneoff = 1, _clearoneoff = 1 }));
                    }
                    else
                    {
                        completeUrl = Utils.GetAbsoluteUrl(Url, Url.Action("PayComplete", "Coupon", new { channel = "all", order = order.OrderID.ToString(), _t = _timeStamp }));
                    }
                    completeUrl = Server.UrlEncode(completeUrl);
                    dict["Message"] = "订单已提交";
                    dict["Success"] = "0";
                    dict["Url"] = string.Format("whotelapp://orderPay?orderid={0}&finishurl={1}&paytype={2}", order.OrderID, completeUrl, "all");
                }
                else
                {
                    dict["Message"] = "订单已提交";
                    dict["Success"] = "0";
                    dict["Url"] = Url.Action("Pay", "Order", new { order = order.OrderID, payChannels = WHotelSite.Common.Config.DefaultPayChannelList });
                }
            }
            else
            {
                dict["Message"] = (order != null ? order.Message : "订单提交失败");
                dict["Success"] = "1";
                dict["Url"] = "";
            }

            return Json(dict, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 更多机酒列表
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="albumId">2美食 3玩乐 4促销/限时 5超值团</param>
        /// <returns></returns>
        public ActionResult MorePackageProductList(int userid = 0, int albumId = 0)
        {
            var isApp = IsApp();
            ViewBag.IsApp = isApp;

            //当前系统环境（ios | android）
            var appType = AppType();
            ViewBag.AppType = appType;

            //是否大于等于5.1版本
            var isThanVer5_1 = IsThanVer5_1();
            ViewBag.IsThanVer5_1 = isThanVer5_1;

            //Weixin
            bool isInWeixin = Request.UserAgent.IndexOf("MicroMessenger") > 0 ? true : false;
            ViewBag.isInWeixin = isInWeixin;

            //当前用户是否VIP
            var isVip = false; if (userid > 0) isVip = account.IsVIPCustomer(new AccountInfoItem { Uid = userid.ToString() });
            ViewBag.IsVip = isVip;

            ViewBag.UserId = userid;
            ViewBag.AlbumId = albumId;

            return View();
        }

        /// <summary>
        /// 更多消费券项目（Item）
        /// </summary>
        /// <returns></returns>
        public ActionResult MorePackageProduct_SKU(int userid = 0, int s = 0, int c = 6, int albumId = 0)
        {
            var isApp = IsApp();
            ViewBag.IsApp = isApp;

            //当前系统环境（ios | android）
            var appType = AppType();
            ViewBag.AppType = appType;

            //是否大于等于5.1版本
            var isThanVer5_1 = IsThanVer5_1();
            ViewBag.IsThanVer5_1 = isThanVer5_1;

            ViewBag.UserId = userid;
            ViewBag.AlbumId = albumId;

            SKUCouponActivityAlbumEntity result = Coupon.GetProductAlbumSKUCouponActivityListByAlbumID(albumId, s, c, userid);

            if (result != null && result.SKUCouponList != null)
            {
                foreach (var item in result.SKUCouponList)
                {
                    if (item.PicList != null && item.PicList.Count > 0 && !string.IsNullOrEmpty(item.PicList[0]))
                    {
                        item.PicPath = item.PicList[0];
                        item.PicPath = item.PicPath.Replace("_theme", "_640x426");
                        if (appType == "android" && (item.PicPath.Contains("upaiyun") || item.PicPath.Contains("p1.zmjiudian")))
                        {
                            item.PicPath = item.PicPath + "/format/webp";
                        }
                    }
                }
            }

            return View(result);
        }

        #endregion
   }
}