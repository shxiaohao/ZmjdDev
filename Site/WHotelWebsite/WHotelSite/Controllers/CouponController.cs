using HJD.AccountServices.Entity;
using HJD.CouponService.Contracts.Entity;
using HJD.HotelManagementCenter.Domain;
using HJD.HotelManagementCenter.Domain.Fund;
using HJD.HotelPrice.Contract.DataContract.Order;
using HJD.HotelServices.Contracts;
using HJD.WeixinService.Contract;
using HJD.WeixinServices.Contracts;
using HJDAPI.APIProxy;
using HJDAPI.Common.Security;
using HJDAPI.Models;
using Newtonsoft.Json;
using ProductService.Contracts.Entity;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using WHotelSite.Adapter;
using WHotelSite.Common;
using WHotelSite.Models;

namespace WHotelSite.Controllers
{
    public class CouponController : BaseController
    {
        //
        // GET: /Coupon/

        public const string AccessProtocal_IsApp = "whotelapp://www.zmjiudian.com/";
        public const string AccessProtocalPage_IsApp = "whotelapp://gotopage?url=http://www.zmjiudian.com/";

        public const string AccessProtocal_UnApp = "/inspector/jump?jumpurl=whotelapp://www.zmjiudian.com/";  //"http://www.zmjiudian.com/hotel/";  //"http://m1.zmjiudian.com/";
        public const string AccessProtocalPage_UnApp = "/inspector/jump?jumpurl=whotelapp://www.zmjiudian.com/gotopage?url=http://www.zmjiudian.com/";
        //public const string AccessProtocalPage_UnApp = "/inspector/jump?jumpurl=whotelapp://www.zmjiudian.com/gotopage?url=http://192.168.1.22:8081/";
 
        #region App 我的钱包

        /// <summary>
        /// 钱包页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Wallet()
        {
            var isApp = IsApp();
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
            ViewBag.IsApp = isApp;

            //当前系统环境（ios | android）
            ViewBag.AppType = AppType();

            //是否大于等于5.0版本
            ViewBag.IsThanVer5_0 = IsThanVer5_0();

            //是否大于等于5.2版本
            ViewBag.IsThanVer5_2 = IsThanVer5_2();

            //是否大于等于5.4版本
            ViewBag.IsThanVer5_4 = IsThanVer5_4();

            //url加密  优先设计 通过则继续；不通过返回验证不通过信息
            /*
            1.	URL中除签名部分外，所有的参数都补齐
            2.	取URL中除去“&sign={sign}”部分的URL做为签名算法中的RequestType
            3.	调用签名算法签名，用签名结果规则URL中的{sign} 
            */
            string url = this.Request.Url.AbsoluteUri.Replace("&_newtitle=1", "").Replace("&_newpage=1", "").Replace("&_newpage=0", "").Replace("&_dorpdown=1", "").Replace("&_dorpdown=0", "");
            string url_Sign = this.Request.QueryString["Sign"];//url请求的sign
            long url_TimeStamp = 0;
            url_TimeStamp = long.TryParse(this.Request.QueryString["TimeStamp"], out url_TimeStamp) ? url_TimeStamp : 0;
            int sublength = url.LastIndexOf("&");
            string url_RequestType = sublength > 0 ? url.Substring(0, sublength) : url;
            int url_SourceID = 0;
            url_SourceID = int.TryParse(this.Request.QueryString["SourceID"], out url_SourceID) ? url_SourceID : 0;
            string sscode = HJDAPI.Common.Security.Signature.GenSignature(url_TimeStamp, url_SourceID, WHotelSite.Common.Config.MD5Key, url_RequestType);

            string final = url_Sign.Replace(" ", "+");

            string userAgent = Request.UserAgent;
            string terminalName = Utils.GetTerminalName(userAgent, _ContextBasicInfo);
            if (!(sscode == final))
            {
                LogHelper.WriteLog(url + ":" + url_RequestType + "  验证失败  " + terminalName + " ");
                return Json(new { Success = 1, Message = "验证失败" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                //LogHelper.WriteLog(url + "  验证成功  " + terminalName + " ");
            }

            ViewBag.CashIntroductionURL = "whotelapp://www.zmjiudian.com/gotopage?url=" + "http://www.zmjiudian.com/coupon/CashIntroduction";
            ViewBag.CouponIntroductionURL = "whotelapp://www.zmjiudian.com/gotopage?url=" + "http://www.zmjiudian.com/coupon/CouponIntroduction";

            long userid = long.TryParse((string)this.RouteData.Values["id"], out userid) ? userid : 0;
            var tag = this.RouteData.Values["tag"]; tag = (tag != null ? tag.ToString() : "");
            var mode = this.RouteData.Values["mode"]; mode = (mode != null ? mode.ToString() : "");

            if (userid != 0)
            {
                //通用参数
                GetInspectorHotelsListParam lp = new GetInspectorHotelsListParam { userid = userid };

                //空积分相关对象
                var PointResult = new PointResult();

                //空基金相关对象
                var userFundInfo = new UserFundEntity();
                var userFundIncomeStatList = new List<UserFundIncomeStatEntity>();
                var userFundExpendDetailList = new List<UserFundExpendDetailEntity>();

                //空现金券相关对象
                var CouponResult = new CouponResult();
                var cashCouponCount = 0;

                //代金券默认数量
                var voucherCount = 0;

                //空房券相关对象
                var RoomcouponResult = new RoomCouponResult();// List<RoomCouponOrderEntity>();

                //空返现相关对象
                var HadRebateAmount = 0;
                var WaitingRebateAmount = 0;
                var RebateList = new List<UserRebateInfoEntity>();

                //空航空里程对象
                var airMilesResult = new EasternAirLinesCardEntity();
                var airMilesDetailResult = new List<EasternAirLinesRecordEntity>();

                //空消费券相关对象
                var productCouponCount = 0;
                var productCouponDetailResult = new ProductCouponResult();

                //菜单链接
                var pointUrl = "";
                var pointDetailUrl = "";
                var fundUrl = "";
                var fundDetailUrl = "";
                var couponUrl = "";
                var couponDetailUrl = "";
                var cashCouponUrl = "";
                var voucherUrl = "";
                var roomcouponUrl = "";
                var cashUrl = "";

                //航空里程相关url
                var airmilesUrl = "";
                var airmilesDetailUrl = "";

                //消费券相关url
                var productCouponUrl = "";
                var productCouponDetailUrl = "";

                //得到领取的现金券和抵扣的现金券信息
                switch (tag.ToString())
                {
                    #region 积分

                    case "point":
                        {
                            //获取积分
                            PointResult = Coupon.GetPersonalPoint(lp);

                            //积分url
                            pointDetailUrl = GetUserWalletUrl(userid, "point", "detail", newpage: true);

                            //积分明细
                            var detailList = PointResult.pointList ?? new List<PointsEntity>();
                            if (PointResult.pointConsumeList != null && PointResult.pointConsumeList.Count > 0)
                            {
                                foreach (var _item in PointResult.pointConsumeList)
                                {
                                    detailList.Add(new PointsEntity { TypeName = _item.TypeName, ObjectName = _item.ObjectName, TotalPoint = (_item.State == 2 ? _item.ConsumePoint : (0 - _item.ConsumePoint)), CreateTime = _item.CreateTime });
                                }
                            }
                            ViewBag.PointDetailList = detailList.OrderByDescending(_ => _.CreateTime).ToList();

                            break;
                        }

                    #endregion

                    #region 住基金

                    case "fund":
                        {
                            //获取住基金
                            userFundInfo = new Fund().GetUserFundInfo(userid);

                            //住基金url
                            fundDetailUrl = GetUserWalletUrl(userid, "fund", "detail", newpage: true);

                            //住基金明细
                            if (mode.ToString() == "detail")
                            {
                                userFundIncomeStatList = new Fund().GetUserFundIncomeStat(userid);
                                userFundExpendDetailList = new Fund().GetUserFundExpendDetail(userid);

                                var detailList = userFundIncomeStatList ?? new List<UserFundIncomeStatEntity>();
                                if (userFundExpendDetailList != null && userFundExpendDetailList.Count > 0)
                                {
                                    foreach (var _item in userFundExpendDetailList)
                                    {
                                        detailList.Add(new UserFundIncomeStatEntity { Label = _item.Label, Day = _item.CreateTime, Fund = (0 - _item.Fund) });
                                    }
                                }
                                ViewBag.FundDetailList = detailList.OrderByDescending(_ => _.Day).ToList();
                            }
                            break;
                        }

                    #endregion

                    #region 现金券

                    case "coupon":
                        {
                            //获取现金券
                            CouponResult = Coupon.GetPersonalCoupon(lp);

                            //现金券url
                            couponDetailUrl = GetUserWalletUrl(userid, "coupon", "detail", newpage: true);

                            //现金券明细
                            var detailList = CouponResult.couponList ?? new List<AcquiredCoupon>();
                            if (CouponResult.couponRecordList != null && CouponResult.couponRecordList.Count > 0)
                            {
                                foreach (var _item in CouponResult.couponRecordList)
                                {
                                    detailList.Add(new AcquiredCoupon { TypeName = _item.TypeName, ObjectName = _item.ObjectName, AcquiredTime = _item.createTime, TotalMoney = (0 - _item.discountMoney) });
                                }
                            }
                            ViewBag.CouponDetailList = detailList.OrderByDescending(_ => (_.AcquiredTime.HasValue ? _.AcquiredTime : _.CreateTime)).ToList();
                            break;
                        }

                    #endregion

                    #region 房券

                    case "roomcoupon":
                        {
                            //获取房券
                            RoomcouponResult = new Coupon().GetPersonalRoomCoupon(lp);
                            if (RoomcouponResult != null && RoomcouponResult.couponList != null)
                            {
                                RoomcouponResult.couponList = RoomcouponResult.couponList.Where(_ => _.State != 4).ToList();
                            }

                            RoomcouponResult.couponList = RoomcouponResult.couponList.Where(r => r.State != 4).ToList();
                            break;
                        }

                    #endregion

                    #region 航空里程

                    case "airmiles":
                        {
                            //获取航空里程
                            airMilesResult = new EasternAirLinesPoints().GetPointsPreview(userid.ToString()) ?? new EasternAirLinesCardEntity();

                            //航空里程url
                            airmilesDetailUrl = GetUserWalletUrl(userid, "airmiles", "detail", newpage: true);

                            if (mode.ToString() == "detail")
                            {
                                //获取里程明细
                                airMilesDetailResult = new EasternAirLinesPoints().GetPointsList(userid.ToString()) ?? new List<EasternAirLinesRecordEntity>();

                            }

                            break;
                        }

                    #endregion

                    #region 消费券

                    case "productcoupon":
                        {
                            //获取消费券
                            productCouponDetailResult = new Coupon().GetPersonalProductCoupon(userid);

                            //消费券url
                            productCouponDetailUrl = GetUserWalletUrl(userid, "productcoupon", "detail", newpage: true);

                            if (mode.ToString() == "detail")
                            {
                                ////获取里程明细
                                //productCouponDetailResult = new Coupon().GetPersonalProductCoupon(userid);

                            }

                            //获取可重新上传照片的券ID
                            var canUpdatePhotoExid = new Coupon().GetUserCanReUpdatePhotoListByUserID(userid);
                            ViewBag.CanUpdatePhotoExid = canUpdatePhotoExid;

                            break;
                        }

                    #endregion

                    #region 返现

                    case "cash":
                        {
                            long timeStamp = (long)Math.Round((DateTime.Now - new DateTime(1971, 1, 1, 8, 0, 0)).TotalSeconds, 0);
                            int sourceID = 3;
                            string requestType = "GetUserRebateList";
                            string sign = HJDAPI.Common.Security.Signature.GenSignature(timeStamp, sourceID, WHotelSite.Common.Config.MD5Key, requestType);
                            GetUserRebateListRequestParams param = new GetUserRebateListRequestParams() { userid = userid, TimeStamp = timeStamp, SourceID = sourceID, RequestType = requestType, Sign = sign };
                            GetUserRebateListResponse ss = new Order().GetUserRebateList(param);

                            HadRebateAmount = ss.HadRebateAmount;
                            WaitingRebateAmount = ss.WaitingRebateAmount;
                            RebateList = ss.RebateList;
                            break;
                        }

                    #endregion

                    //默认，我的钱包（基本什么都不需要取）
                    default:
                        {
                            //获取积分
                            PointResult = Coupon.GetPersonalPoint(lp);

                            //获取住基金
                            userFundInfo = new Fund().GetUserFundInfo(userid);

                            ////获取现金券
                            //CouponResult = Coupon.GetPersonalCoupon(lp);

                            //【最新】获取现金券数量
                            cashCouponCount = Coupon.GetUserCouponInfoCountByUserId(Convert.ToInt32(userid), UserCouponState.log);

                            //【最新】获取代金券数量
                            voucherCount = Coupon.GetUserCouponInfoCountByUserIdAndType(Convert.ToInt32(userid), UserCouponState.log, UserCouponType.DiscountVoucher);

                            //获取房券
                            RoomcouponResult = new Coupon().GetPersonalRoomCoupon(lp);
                            if (RoomcouponResult != null && RoomcouponResult.couponList != null)
                            {
                                RoomcouponResult.couponList = RoomcouponResult.couponList.Where(_ => _.State != 4).ToList();
                            }

                            //获取航空里程
                            airMilesResult = new EasternAirLinesPoints().GetPointsPreview(userid.ToString()) ?? new EasternAirLinesCardEntity();

                            //获取消费券
                            productCouponCount = new Coupon().GetPersonalProductCouponCount(userid);

                            pointUrl = GetUserWalletUrl(userid, "point", newtitle: true);
                            fundUrl = GetUserWalletUrl(userid, "fund", newtitle: true);
                            couponUrl = GetUserWalletUrl(userid, "coupon", newtitle: true);
                            roomcouponUrl = GetUserWalletUrl(userid, "roomcoupon");
                            cashUrl = GetUserWalletUrl(userid, "cash");
                            airmilesUrl = GetUserWalletUrl(userid, "airmiles", newtitle: false, dorpdown: true);
                            productCouponUrl = GetUserWalletUrl(userid, "productcoupon", newtitle: false, dorpdown: true);
                            productCouponDetailUrl = GetUserWalletUrl(userid, "productcoupon", "detail", newpage: false, dorpdown: true);

                            cashCouponUrl = string.Format("/coupon/WalletCashCoupon?userid={0}&select=0&orderid=0&issection=0", userid);
                            voucherUrl = string.Format("/coupon/WalletVoucher?userid={0}&select=0&orderid=0&issection=0", userid);
                            break;
                        }
                }

                ViewBag.tag = tag;
                ViewBag.Mode = mode;
                ViewBag.userid = userid;
                ViewBag.HadRebateAmount = HadRebateAmount;
                ViewBag.WaitingRebateAmount = WaitingRebateAmount;
                ViewBag.RebateList = RebateList;

                ViewBag.PointResult = PointResult;
                ViewBag.CouponResult = CouponResult;
                ViewBag.CashCouponCount = cashCouponCount;
                ViewBag.VoucherCount = voucherCount;
                ViewBag.RoomcouponResult = RoomcouponResult;

                //基金相关
                ViewBag.UserFundInfo = userFundInfo;
                ViewBag.UserFundIncomeStatList = userFundIncomeStatList;
                ViewBag.UserFundExpendDetailList = userFundExpendDetailList;

                //航空里程相关
                ViewBag.AirMilesResult = airMilesResult;
                ViewBag.AirMilesDetailResult = airMilesDetailResult;

                //消费券相关
                ViewBag.ProductCouponCount = productCouponCount;
                ViewBag.ProductCouponDetailResult = productCouponDetailResult;

                ViewBag.PointUrl = pointUrl;
                ViewBag.PointDetailUrl = pointDetailUrl;
                ViewBag.FundUrl = fundUrl;
                ViewBag.FundDetailUrl = fundDetailUrl;
                ViewBag.CashCouponUrl = cashCouponUrl;
                ViewBag.VoucherUrl = voucherUrl;
                ViewBag.CouponUrl = couponUrl;
                ViewBag.CouponDetailUrl = couponDetailUrl;
                ViewBag.RoomCouponUrl = roomcouponUrl;
                ViewBag.CashUrl = cashUrl;
                ViewBag.AirMilesUrl = airmilesUrl;
                ViewBag.AirMilesDetailUrl = airmilesDetailUrl;
                ViewBag.ProductCouponUrl = productCouponUrl;
                ViewBag.ProductCouponDetailUrl = productCouponDetailUrl;
                return View();
            }
            else
            {
                return Json(new { Message = "缺少用户Id", Success = 1 }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 获取钱包下的各个功能的URL
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="tag"></param>
        /// <param name="mode"></param>
        /// <param name="newtitle">url是否要追加 _newtitle=1 参数</param>
        /// <param name="newpage">url是否要追加 _newpage=1 参数</param>
        /// <param name="dorpdown">url是否要追加 _dorpdown=1 参数</param>
        /// <returns></returns>
        public string GetUserWalletUrl(long userId, string tag, string mode = "", bool newtitle = false, bool newpage = false, bool dorpdown = false)
        {
            Int64 url_TimeStamp = Signature.GenTimeStamp();
            int url_SourceID = 100;
            string webSiteUrl = WHotelSite.Common.Config.WebSiteUrl;
            if (string.IsNullOrWhiteSpace(webSiteUrl))
            {
                webSiteUrl = "http://www.zmjiudian.com/";
            }
            //webSiteUrl = "http://192.168.1.22:8081/";
            //webSiteUrl = "http://localhost:8780/";
            //webSiteUrl = "http://www.tst.zmjd001.com/";
            string url_RequestType = String.Format("{4}personal/wallet/{0}/{3}{5}?TimeStamp={1}&SourceID={2}", userId, url_TimeStamp, url_SourceID, tag, webSiteUrl, (string.IsNullOrEmpty(mode) ? "" : "/" + mode));
            string MD5Key = WHotelSite.Common.Config.MD5Key;
            string Sign = Signature.GenSignature(url_TimeStamp, url_SourceID, MD5Key, url_RequestType);
            string UserInfoUrl = String.Format("{0}&Sign={1}{2}{3}{4}", url_RequestType, Sign, (newtitle ? "&_newtitle=1" : ""), (newpage ? "&_newpage=1" : ""), (dorpdown ? "&_dorpdown=1" : ""));
            return UserInfoUrl;
        }

        /// <summary>
        /// 消费券申请退款
        /// </summary>
        /// <param name="exid"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public ActionResult ProductCouponReturnPolicy(int exid, string userid)
        {
            Dictionary<string, string> re = new Dictionary<string, string>();

            var addresult = Coupon.Add2RefundCoupon(exid, userid);

            return Json(addresult, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 消费券重新上传照片
        /// </summary>
        /// <param name="exid"></param>
        /// <param name="photoUrl"></param>
        /// <returns></returns>
        public ActionResult UpdateCoupnPhoto(int exid, string photoUrl)
        {
            var updateObj = new UpdateCouponPhotoReqParms();
            updateObj.CouponID = exid;
            updateObj.PhotoUrl = photoUrl;

            Int64 url_TimeStamp = Signature.GenTimeStamp();
            int url_SourceID = 100;
            string MD5Key = WHotelSite.Common.Config.MD5Key;
            string url_RequestType = "UpdateCoupnPhoto";

            updateObj.Sign = Signature.GenSignature(url_TimeStamp, url_SourceID, MD5Key, url_RequestType);
            updateObj.SourceID = url_SourceID;
            updateObj.RequestType = url_RequestType;
            updateObj.TimeStamp = Signature.GenTimeStamp();

            //update photo
            var update = new Coupon().UpdateCoupnPhoto(updateObj);

            return Json(update, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 钱包-现金券
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="couponid">已选券ID</param>
        /// <param name="buycount">购买数量/套数</param>
        /// <param name="totalprice">订单总额</param>
        /// <param name="from">酒店套餐预订checkin</param>
        /// <param name="to">酒店套餐预订checkout</param>
        /// <param name="sourceid">产品ID（SKUID或PID）</param>
        /// <param name="sourcetype">产品类型（1酒店套餐 2消费券SKU）</param>
        /// <param name="select">是否选择模式</param>
        /// <param name="issection">是否弹窗模式</param>
        /// <param name="canNotUseCashcoupon">不能使用满减券</param>
        /// <returns></returns>
        public ActionResult WalletCashCoupon(long userid = 0, int couponid = 0, int buycount = 0, decimal totalprice = 0, string from = "2017-10-01", string to = "2017-10-02", int sourceid = 0, int sourcetype = 0, int select = 0, int issection = 0, int canNotUseCashcoupon = 0)
        {
            var isApp = IsApp();
            ViewBag.IsApp = isApp;

            //Weixin
            bool isInWeixin = Request.UserAgent.IndexOf("MicroMessenger") > 0 ? true : false;
            ViewBag.IsInWeixin = isInWeixin;

            var curUserID = UserState.UserID > 0 ? UserState.UserID : userid;
            ViewBag.UserId = curUserID;

            //是否现金券选择模式（一般是下单过程中使用）
            var isSelect = select == 1;
            ViewBag.IsSelect = isSelect;

            //是否为模块调用
            var isSection = issection == 1;
            ViewBag.IsSection = isSection;

            ViewBag.CouponId = couponid;
            ViewBag.BuyCount = buycount;
            ViewBag.TotalPrice = totalprice;

            var fromDate = DateTime.Parse(from);
            ViewBag.FromDate = fromDate;
            var toDate = DateTime.Parse(to);
            ViewBag.ToDate = toDate;

            ViewBag.SourceId = sourceid;
            ViewBag.SourceType = sourcetype;

            //未使用列表
            var logList = new List<UserCouponItemInfoEntity>();

            //已使用列表
            var usedList = new List<UserCouponItemInfoEntity>();

            //已过期列表
            var expiredList = new List<UserCouponItemInfoEntity>();

            //可用列表
            var canList = new List<UserCouponItemInfoEntity>();

            //不可用列表
            var nocanList = new List<UserCouponItemInfoEntity>();

            if (isSelect)
            {
                //选择模式
                canList = new List<UserCouponItemInfoEntity>();
                nocanList = new List<UserCouponItemInfoEntity>();

                var couponParamObj = new OrderUserCouponRequestParams();
                couponParamObj.BuyCount = buycount;
                couponParamObj.TotalOrderPrice = totalprice;
                couponParamObj.SelectedCashCouponID = couponid;
                couponParamObj.OrderSourceID = sourceid;
                couponParamObj.OrderTypeID = (CashCouponOrderSorceType)sourcetype;
                couponParamObj.SelectedDateFrom = fromDate;
                couponParamObj.SelectedDateTo = toDate;
                couponParamObj.UserID = userid;
                couponParamObj.CanNotUseDiscountOverPrice = canNotUseCashcoupon;
                canList = new Coupon().GetCanUseCouponInfoListForOrder(couponParamObj);
                nocanList = new Coupon().GetCannotUseCouponInfoListForOrder(couponParamObj);

            }
            else
            { 
                //钱包模式
                logList = Coupon.GetUserCouponInfoListByUserId(Convert.ToInt32(userid), UserCouponState.log);
                usedList = Coupon.GetUserCouponInfoListByUserId(Convert.ToInt32(userid), UserCouponState.used);
                expiredList = Coupon.GetUserCouponInfoListByUserId(Convert.ToInt32(userid), UserCouponState.expired);
            }

            ViewBag.LogList = logList;
            ViewBag.UsedList = usedList;
            ViewBag.ExpiredList = expiredList;
            ViewBag.CanList = canList;
            ViewBag.NoCanList = nocanList;

            return View();
        }

        /// <summary>
        /// 钱包-代金券
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="couponid"></param>
        /// <param name="buycount"></param>
        /// <param name="totalprice"></param>
        /// <param name="skuVoucherPrice"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="sourceid"></param>
        /// <param name="sourcetype"></param>
        /// <param name="select"></param>
        /// <param name="issection"></param>
        /// <param name="canNotUseCashcoupon">不能使用代金券</param>
        /// <returns></returns>
        public ActionResult WalletVoucher(long userid = 0, string couponid = "", int buycount = 0, decimal totalprice = 0, decimal skuVoucherPrice = 0, string from = "2017-10-01", string to = "2017-10-02", int sourceid = 0, int sourcetype = 0, int select = 0, int issection = 0,int canNotUseCashcoupon = 0)
        {
            var isApp = IsApp();
            ViewBag.IsApp = isApp;

            //Weixin
            bool isInWeixin = Request.UserAgent.IndexOf("MicroMessenger") > 0 ? true : false;
            ViewBag.IsInWeixin = isInWeixin;

            var curUserID = UserState.UserID > 0 ? UserState.UserID : userid;
            ViewBag.UserId = curUserID;

            //是否现金券选择模式（一般是下单过程中使用）
            var isSelect = select == 1;
            ViewBag.IsSelect = isSelect;

            //是否为模块调用
            var isSection = issection == 1;
            ViewBag.IsSection = isSection;

            var selectedCouponIds = new List<int>();
            if (!string.IsNullOrEmpty(couponid))
            {
                selectedCouponIds = couponid.Split(',').Select(_ => Convert.ToInt32(_)).ToList();
            }

            ViewBag.SelectedCouponIds = selectedCouponIds;
            ViewBag.BuyCount = buycount;
            ViewBag.TotalPrice = totalprice;
            ViewBag.SkuVoucherPrice = skuVoucherPrice;

            var fromDate = DateTime.Parse(from);
            ViewBag.FromDate = fromDate;
            var toDate = DateTime.Parse(to);
            ViewBag.ToDate = toDate;

            ViewBag.SourceId = sourceid;
            ViewBag.SourceType = sourcetype;

            //未使用列表
            var logList = new List<UserCouponItemInfoEntity>();

            //已使用列表
            var usedList = new List<UserCouponItemInfoEntity>();

            //已过期列表
            var expiredList = new List<UserCouponItemInfoEntity>();

            //可用列表
            var canList = new List<UserCouponItemInfoEntity>();

            //不可用列表
            var nocanList = new List<UserCouponItemInfoEntity>();

            if (isSelect)
            {
                //选择模式
                canList = new List<UserCouponItemInfoEntity>();
                nocanList = new List<UserCouponItemInfoEntity>();

                var couponParamObj = new OrderUserCouponRequestParams();
                couponParamObj.BuyCount = buycount;
                couponParamObj.TotalOrderPrice = totalprice;
                //couponParamObj.SelectedCashCouponID = couponid;
                couponParamObj.SelectedCashCouponID = 0;
                couponParamObj.OrderSourceID = sourceid;
                couponParamObj.OrderTypeID = (CashCouponOrderSorceType)sourcetype;
                couponParamObj.SelectedDateFrom = fromDate;
                couponParamObj.SelectedDateTo = toDate;
                couponParamObj.UserID = userid;
                couponParamObj.CanNotUseDiscountOverPrice = canNotUseCashcoupon;
                canList = Coupon.GetCanUseVoucherInfoListForOrder(couponParamObj);
                nocanList = Coupon.GetCannotUseVoucherInfoListForOrder(couponParamObj);

                //OrderVoucherRequestParams
            }
            else
            {
                //钱包模式
                logList = Coupon.GetUserCouponInfoListByUserIdAndType(Convert.ToInt32(userid), UserCouponState.log, UserCouponType.DiscountVoucher);
                usedList = Coupon.GetUserCouponInfoListByUserIdAndType(Convert.ToInt32(userid), UserCouponState.used, UserCouponType.DiscountVoucher);
                expiredList = Coupon.GetUserCouponInfoListByUserIdAndType(Convert.ToInt32(userid), UserCouponState.expired, UserCouponType.DiscountVoucher);
            }

            ViewBag.LogList = logList;
            ViewBag.UsedList = usedList;
            ViewBag.ExpiredList = expiredList;
            ViewBag.CanList = canList;
            ViewBag.NoCanList = nocanList;

            return View();
        }

        #endregion

        #region 航空里程相关处理 2017.03.09 haoy

        public ActionResult FlyerBuild(int userid = 0)
        {
            var isApp = IsApp();
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
            ViewBag.IsApp = isApp;
            ViewBag.UserId = userid;

            //调试使用
            ViewBag.AppType = AppType();
            if (AppType() == "iosapp") ViewBag.AppVerForIOS = AppVerForIOS();
            else ViewBag.AppVerForIOS = "";
            if (AppType() == "android") ViewBag.AppVerForAndroid = AppVerForAndroid();
            else ViewBag.AppVerForAndroid = "";
            ViewBag.IsLatestVerApp = IsLatestVerApp();

            return View();
        }

        /// <summary>
        /// 提交添加常旅客信息
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="acountNo"></param>
        /// <param name="otherName"></param>
        /// <param name="preName"></param>
        /// <returns></returns>
        public ActionResult AddFlyerInfo(int userid, string acountNo, string otherName, string preName)
        {
            var _addInfo = new EasternAirLinesCardParam();
            _addInfo.UserID = userid;
            _addInfo.AccountNO = acountNo;
            _addInfo.OtherName = otherName;
            _addInfo.PreName = preName;

            //add
            var _add = new EasternAirLinesPoints().AddEACard(_addInfo);

            return Json(_add, JsonRequestBehavior.AllowGet);

            ////验证会员号的正确性
            //var _check = new EasternAirLinesPoints().CheckCardNO(_addInfo.AccountNO);
            //if (_check.Success == 1)
            //{
            //    //add
            //    var _add = new EasternAirLinesPoints().AddEACard(_addInfo);

            //    return Json(_add, JsonRequestBehavior.AllowGet);
            //}
            //else
            //{
            //    return Json(_check, JsonRequestBehavior.AllowGet);
            //}
        }

        /// <summary>
        /// 删除指定的东方航空常旅客卡
        /// </summary>
        /// <param name="cardid"></param>
        /// <returns></returns>
        public ActionResult DeleteEaCard(string cardid)
        {
            //del
            var _del = new EasternAirLinesPoints().ChangeEACard(cardid);

            return Json(_del, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region 抢购列表/限时抢购/房券购买 等

        /// <summary>
        /// 活动抢购列表页面【原始】
        /// </summary>
        /// <param name="advID"></param>
        /// <param name="groupNo"></param>
        /// <param name="onlyVip">是否只显示VIP专享的抢购</param>
        /// <param name="userid"></param>
        /// <param name="count">显示数量 -1默认全部</param>
        /// <param name="activetag">是否显示活动标识</param>
        /// <returns></returns>
        public ActionResult CouponShopList(int advID = 0, int groupNo = 0, int onlyVip = 0, int userid = 0, int count = -1, int activetag = 1)
        {
            var isApp = IsApp();
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
            ViewBag.IsApp = isApp;

            var list = new Coupon().GetSpeciallyCheapRoomCouponActivityList(advID, groupNo, onlyVip);

            if (list.Items != null && list.Items.Count > 0)
            {
                list.Items = list.Items.Where(i => i.EndSellTime > DateTime.Now).OrderBy(i => i.Rank).ThenBy(i => i.StartSellTime).ToList();  //过滤掉已结束的   
                //   LogHelper.WriteLog(string.Format("CouponShopList1:isVIP{0}  Ranks:{1} ", onlyVip, string.Join(",", list.Items.Select(i => i.ActivityID + ":" + i.Rank))));
            }

            ViewBag.ShowList = list;

            //当前用户是否VIP
            var isVip = false; if (userid > 0) isVip = account.IsVIPCustomer(new AccountInfoItem { Uid = userid.ToString() });
            ViewBag.IsVip = isVip;
            ViewBag.UserId = userid;
            ViewBag.ActiveTag = activetag;
            ViewBag.OnlyVip = onlyVip;
            ViewBag.GroupNo = groupNo;

            return View();
        }

        /// <summary>
        /// 限时抢购
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userid"></param>
        /// <param name="sid"></param>
        /// <param name="sign"></param>
        /// <returns></returns>
        public ActionResult CouponShop(int id, string userid = "0", string sid = "", string sign = "", int wxsignfirst = 0)
        {
            var isApp = IsApp();
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
            ViewBag.IsApp = isApp;
            ViewBag.Aid = id;

            var curUserID = UserState.UserID > 0 ? UserState.UserID.ToString() : userid;
            ViewBag.UserId = curUserID;
            userid = curUserID;

            //获取请求参数中的CID
            var requestCID = GetCurCIDForRequest();
            ViewBag.RequestCID = requestCID;

            //CID
            //ViewBag.CID = GetCurCID();

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

            var couponDetail = Coupon.GetCouponActivityDetail(id);
            ViewBag.CouponDetail = couponDetail;

            //如果当前抢购VIP专享，则判断当前用户是否为VIP
            var isVip = false;
            var showVipAlert = false;
            if (couponDetail.activity.IsVipExclusive)
            {
                if (userid != "0") isVip = account.IsVIPCustomer(new AccountInfoItem { Uid = userid });
                if (!isVip) showVipAlert = true;
            }
            ViewBag.IsVip = isVip;
            ViewBag.ShowVipAlert = showVipAlert;

            #region 检查当前抢购的活动规则关联

            //是否需要弹出去微信报名的提示
            var showSignWeixin = false;

            //couponDetail.activity.RelationId = 212;
            if (couponDetail.activity.RelationId > 0)
            {
                var relActiveId = couponDetail.activity.RelationId;
                var weixinActiveEntity = ActiveController.GetWeixinActiveEntity(relActiveId);
                if (weixinActiveEntity != null)
                {
                    ViewBag.WeixinActiveEntity = weixinActiveEntity;

                    var userPhone = "";
                    try
                    {
                        var userEntity = account.GetCurrentUserInfo(Convert.ToInt64(userid));
                        if (userEntity != null) { userPhone = userEntity.MobileAccount; }
                    }
                    catch (Exception ex) { }

                    //根据微信关联活动和手机号，检查报名信息
                    var weixinDraw = new Weixin().GetActiveWeixinDrawByPhone(relActiveId, userPhone);
                    if (weixinDraw == null || weixinDraw.IsShare == 0)
                    {
                        //没有报名或者没有分享，如果是微信则跳转报名页 | App则弹出提示（暂时处理方案）
                        if (isInWeixin)
                        {
                            if (weixinActiveEntity.ActiveEndTime > DateTime.Now)
                            {
                                return Redirect(string.Format("http://www.zmjiudian.com/wx/active/reg/{0}/{1}", "7", relActiveId));
                            }
                        }
                        else
                        {
                            showSignWeixin = true;
                        }
                    }
                }
            }

            ViewBag.ShowSignWeixin = showSignWeixin;
            ViewBag.IsFirstWxsign = wxsignfirst == 1;

            #endregion

            //查询当前券和用户的可售状态
            var buyResult = new Coupon().IsExchangeCouponSoldOut(id, Convert.ToInt64(userid));
            ViewBag.BuyResult = buyResult;

            #region 分享配置

            var shareLink = "";
            if (requestCID > 0)
            {
                //分享链接生成
                shareLink = string.Format("http://www.zmjiudian.com/coupon/shop/{0}?CID={1}", id, requestCID);
            }
            else
            {
                //暂时只有在APP中登录分享才会加sid&cid等参数
                if (isApp)
                {
                    //分享跟踪参数
                    var scp = new GenTrackCodeParam();
                    scp.UserID = Convert.ToInt64(userid);
                    scp.CouponActivityID = id;
                    scp.BizType = ZMJDShareTrackBizType.roomcoupondetail;
                    var shareCode = Comment.GenTrackCodeResult4Share(scp);

                    //分享链接生成
                    shareLink = string.Format("http://www.zmjiudian.com/coupon/shop/{0}?sid={1}", id, shareCode.EncodeStr);
                }
                else 
                {
                    //分享跟踪参数
                    var scp = new GenTrackCodeParam();
                    scp.UserID = Convert.ToInt64(userid);
                    scp.CouponActivityID = id;
                    scp.BizType = ZMJDShareTrackBizType.roomcoupondetail;
                    var shareCode = Comment.GenTrackCodeResult4Share(scp);

                    //分享链接生成
                    shareLink = string.Format("http://www.zmjiudian.com/coupon/shop/{0}?sid={1}", id, shareCode.EncodeStr);
                }
            }
            
            //解析SID
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

            ViewBag.ShareLink = shareLink;
            ViewBag.ShareUserInfo = shareUserInfo;

            #endregion

            #region 行为记录

            var value = string.Format("{{\"id\":\"{0}\",\"userid\":\"{1}\",\"sid\":\"{2}\",\"sign\":\"{3}\",\"wxsignfirst\":\"{4}\"}}", id, userid, sid, sign, wxsignfirst);
            RecordBehavior("CouponShopLoad", value);

            #endregion        

            return View();
        }

        /// <summary>
        /// 团购
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userid"></param>
        /// <param name="sid"></param>
        /// <returns></returns>
        public ActionResult CouponShopForGroup(int id, string userid = "0", string sid = "")
        {
            var isApp = IsApp();
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
            ViewBag.IsApp = isApp;
            ViewBag.Aid = id;
            ViewBag.UserId = userid;

            //CID
           // ViewBag.CID = GetCurCID();

            //调试使用
            ViewBag.AppType = AppType();
            if (AppType() == "iosapp") ViewBag.AppVerForIOS = AppVerForIOS();
            else ViewBag.AppVerForIOS = "";
            if (AppType() == "android") ViewBag.AppVerForAndroid = AppVerForAndroid();
            else ViewBag.AppVerForAndroid = "";
            ViewBag.IsLatestVerApp = IsLatestVerApp();

            //if (userid == "0" && HttpContext.Session["USERID"] != null) { userid = HttpContext.Session["USERID"].ToString(); }
            //ViewBag.UserId = userid;
            //HttpContext.Session["USERID"] = userid;

            //产品信息
            var coupondetail = Coupon.GetGroupCouponActivityDetail(id);

            //已购买者
            var hasBoughtItems = coupondetail.HasBoughtItems;
            if (hasBoughtItems != null && hasBoughtItems.Count > 0)
            {
                hasBoughtItems = hasBoughtItems.Where(h => !string.IsNullOrEmpty(h.TrueName)).ToList();
            }

            ViewBag.HasBoughtItems = hasBoughtItems;
            ViewBag.CouponDetail = coupondetail;

            //查询当前券和用户的可售状态
            var buyResult = new Coupon().IsExchangeCouponSoldOut(id, Convert.ToInt64(userid));
            ViewBag.BuyResult = buyResult;

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
                scp.UserID = Convert.ToInt64(userid);
                scp.CouponActivityID = id;
                scp.BizType = ZMJDShareTrackBizType.roomgroupcoupondetail;
                var shareCode = Comment.GenTrackCodeResult4Share(scp);

                //分享链接生成
                shareLink = string.Format("http://www.zmjiudian.com/coupon/shop/group/{0}?sid={1}", id, shareCode.EncodeStr);
            }

            ViewBag.ShareLink = shareLink;
            ViewBag.ShareUserInfo = shareUserInfo;

            #endregion

            return View();
        }

        /// <summary>
        /// 房券购买检测
        /// </summary>
        /// <param name="id"></param>
        /// <param name="buynum"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public ActionResult CheckBuyNumber(int id, int buynum, string userid = "0")
        {
            Dictionary<string, string> re = new Dictionary<string, string>();

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
        /// 普通房券抢购提交
        /// </summary>
        /// <param name="aid"></param>
        /// <param name="atype"></param>
        /// <param name="pid"></param>
        /// <param name="pricetype"></param>
        /// <param name="paynum"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public ActionResult SubmitConpon(int aid, int atype, int pid, int pricetype, int paynum, string userid = "0")
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();

            SubmitCouponOrderModel sub = new SubmitCouponOrderModel();
            sub.ActivityID = aid;
            sub.ActivityType = atype;
            sub.UserID = Convert.ToInt64(userid);
            sub.CID = GetCurCID();
            sub.OrderItems = new List<ProductAndNum> 
            { 
                new ProductAndNum
                {
                    SourceID = pid,
                    Number = paynum,
                    Type = pricetype
                }
            };

            //是否大于等于5.4版本
            var isThanVer5_4 = IsThanVer5_4();
            ViewBag.IsThanVer5_4 = isThanVer5_4;

            CouponOrderResult order = new Coupon().SubmitExchangeOrder(sub);
            if (order != null && order.Success == 0)
            {
                if (IsLatestVerApp())
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
                    dict["Exids"] = string.Join(",", order.ExchangeIdList);
                    dict["Url"] = string.Format("whotelapp://orderPay?orderid={0}&finishurl={1}&paytype={2}", order.OrderID, completeUrl, "all");
                }
                else
                {
                    dict["Message"] = "订单已提交";
                    dict["Success"] = "0";
                    dict["Exids"] = string.Join(",", order.ExchangeIdList);
                    dict["Url"] = Url.Action("Pay", "Order", new { order = order.OrderID, payChannels = WHotelSite.Common.Config.DefaultPayChannelList });
                }
            }
            else
            {
                dict["Message"] = (order != null ? order.Message : "订单提交失败");
                dict["Success"] = "1";
                dict["Url"] = "";
                dict["Exids"] = "";
            }

            return Json(dict, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 团购房券提交
        /// </summary>
        /// <param name="aid"></param>
        /// <param name="atype"></param>
        /// <param name="pid"></param>
        /// <param name="pricetype"></param>
        /// <param name="paynum"></param>
        /// <param name="userid"></param>
        /// <param name="trueName"></param>
        /// <param name="personnelStatus"></param>
        /// <param name="childOldInfo"></param>
        /// <returns></returns>
        public ActionResult SubmitGroupConpon(int aid, int atype, int pid, int pricetype, int paynum, string userid = "0", string trueName = "", string personnelStatus = "", string childOldInfo = "")
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();

            SubmitCouponOrderModel sub = new SubmitCouponOrderModel();
            sub.ActivityID = aid;
            sub.ActivityType = atype;
            sub.UserID = Convert.ToInt64(userid);
            sub.CID = GetCurCID();
            sub.OrderItems = new List<ProductAndNum> 
            { 
                new ProductAndNum
                {
                    SourceID = pid,
                    Number = paynum,
                    Type = pricetype
                }
            };

            //是否大于等于5.4版本
            var isThanVer5_4 = IsThanVer5_4();
            ViewBag.IsThanVer5_4 = isThanVer5_4;

            //主题团购购买的附加信息(如几大几小，年龄等)
            sub.AddInfo = new ExchangeCouponAddInfoItem();
            sub.AddInfo.TrueName = trueName;
            sub.AddInfo.ChildrenAge = childOldInfo;
            sub.AddInfo.PersonnelStructure = personnelStatus;

            CouponOrderResult order = new Coupon().SubmitExchangeOrder(sub);
            if (order != null && order.Success == 0)
            {
                if (IsLatestVerApp())
                {
                    var completeUrl = "";
                    if (isThanVer5_4)
                    {
                        completeUrl = Utils.GetAbsoluteUrl(Url, Url.Action("PayComplete", "Coupon", new { channel = "all", order = order.OrderID.ToString(), _isoneoff = 1, _clearoneoff = 1 }));
                    }
                    else
                    {
                        completeUrl = Utils.GetAbsoluteUrl(Url, Url.Action("PayComplete", "Coupon", new { channel = "all", order = order.OrderID.ToString() }));   
                    }
                    completeUrl = Server.UrlEncode(completeUrl);
                    dict["Message"] = "订单已提交";
                    dict["Success"] = "0";
                    dict["Exids"] = string.Join(",", order.ExchangeIdList);
                    dict["Url"] = string.Format("whotelapp://orderPay?orderid={0}&finishurl={1}&paytype={2}", order.OrderID, completeUrl, "all");
                }
                else
                {
                    dict["Message"] = "订单已提交";
                    dict["Success"] = "0";
                    dict["Exids"] = string.Join(",", order.ExchangeIdList);
                    dict["Url"] = Url.Action("Pay", "Order", new { order = order.OrderID, payChannels = WHotelSite.Common.Config.DefaultPayChannelList  });
                }
            }
            else
            {
                dict["Message"] = (order != null ? order.Message : "订单提交失败");
                dict["Success"] = "1";
                dict["Url"] = "";
                dict["Exids"] = "";
            }

            return Json(dict, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 券支付完成页面
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="order"></param>
        /// <param name="_t">支付时间戳，用于判断非法进入支付完成页面</param>
        /// <param name="code"></param>
        /// <returns></returns>
        public ActionResult PayComplete(string channel, string order = "0", long _t = 0, string code = "")
        {
            var isApp = IsApp();
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
            ViewBag.IsApp = isApp;
            ViewBag.Channel = channel;

            //Weixin
            bool isInWeixin = Request.UserAgent.IndexOf("MicroMessenger") > 0 ? true : false;
            ViewBag.isInWeixin = isInWeixin;

            //微信环境下，做微信静默授权
            var openid = "";
            var wxuid = 0;
            if (isInWeixin)
            {
                if (!string.IsNullOrEmpty(code))
                {
                    //通过code换取网页授权access_token
                    var accessToken = WeiXinHelper.GetWeixinAccessTokenForHaoYi(code);
                    if (accessToken != null && !string.IsNullOrEmpty(accessToken.Openid))
                    {
                        openid = accessToken.Openid;

                        //存储微信用户
                        try
                        {
                            //insert
                            var w = new WeixinUser
                            {
                                Openid = openid,
                                Unionid = accessToken.Unionid,
                                Nickname = "",
                                Sex = "",
                                Province = "",
                                City = "",
                                Country = "",
                                Headimgurl = "",
                                Privilege = "",
                                Phone = "",
                                Remark = "",
                                GroupId = "0",
                                Subscribe = 0,
                                WeixinAcount = "weixinservice_haoyi",   //WeiXinChannelCode.周末酒店服务号_皓颐
                                Language = "zh_CN",
                                SubscribeTime = DateTime.Now,
                                CreateTime = DateTime.Now
                            };
                            var update = new Weixin().UpdateWeixinUserSubscribe(w);

                            //get weixin user
                            var weixinUser = new Weixin().GetWeixinUser(openid);
                            if (weixinUser != null && weixinUser.ID > 0)
                            {
                                wxuid = weixinUser.ID;
                            }
                        }
                        catch (Exception ex)
                        {

                        }

                    }
                    else
                    {
                        //如果code有值，但不能获取，一般说明code过期了，那么重新获取
                        var weixinGoUrl = WeiXinHelper.GenSilenceAuthorUrlForHaoYi(HttpUtility.UrlEncode(string.Format("http://www.zmjiudian.com/coupon/paycomplete/{0}/{1}/{2}", channel, order, _t)));
                        return Redirect(weixinGoUrl);
                    }
                }
                else
                {
                    //授权页面
                    var weixinGoUrl = WeiXinHelper.GenSilenceAuthorUrlForHaoYi(HttpUtility.UrlEncode(string.Format("http://www.zmjiudian.com/coupon/paycomplete/{0}/{1}/{2}", channel, order, _t)));
                    return Redirect(weixinGoUrl);
                }
            }
            ViewBag.Openid = openid;
            ViewBag.WeixinUid = wxuid; 

            RoomCouponOrderEntity couponOrder = new Coupon().GenCouponPayCompleteResult(Convert.ToInt32(order));


            //if (couponOrder == null)
            //{
            //    HJD.CouponService.Contracts.Entity.CommOrderEntity commOrderEntity = new Coupon().GetOneCommOrderEntity(Convert.ToInt32(order));
            //    ViewBag.CommOrderEntity = commOrderEntity;
            //}


            //var couponOrderKe=couponOrder.ExchangeCouponList.Where(_ => _.IsPackage == true).ToList();//取出可产品
            //SKUCouponActivityDetailModel couponSkuInfoKe = Coupon.GetSKUCouponActivityDetail(couponOrderKe[0].SKUID);

            //ViewBag.couponKeName = couponSkuInfoKe.activity.PageTitle;
            //ViewBag.couponKeSPUName = couponSkuInfoKe.SKUInfo.SKU.Name;



            long userid = 0;
            var skuid = 0;
            var groupid = 0;
            if (couponOrder != null)
	        {
                if (couponOrder.ExchangeCouponList != null && couponOrder.ExchangeCouponList.Count > 0)
                {
                    var _exchangeCouponList = couponOrder.ExchangeCouponList.OrderByDescending(_ => _.Price).ToList();
                    userid = _exchangeCouponList[0].UserID;
                    skuid = _exchangeCouponList[0].SKUID;
                    groupid = _exchangeCouponList[0].GroupId;
                }
                else
                {
                    userid = couponOrder.UserId;
                    skuid = couponOrder.SKUID;
                    groupid = couponOrder.GroupId;
                }
	        }


            couponOrder.ExchangeCouponList = couponOrder.ExchangeCouponList.Where(_ => _.IsPackage == false).ToList();
            ViewBag.CouponOrder = couponOrder;


            ViewBag.WalletUrl = GetUserWalletUrl(Convert.ToInt64(userid), "wallet");

            ViewBag.UserId = userid;
            ViewBag.SKUID = skuid;
            ViewBag.GroupId = groupid;
            ViewBag.OrderId = Convert.ToInt32(order);

            //获取券SKU信息
            var couponSkuInfo = new SKUCouponActivityDetailModel();
            try
            {
                
                couponSkuInfo = Coupon.GetSKUCouponActivityDetail(skuid);
            }
            catch (Exception ex)
            {
                couponSkuInfo = new SKUCouponActivityDetailModel();
            }

            ViewBag.CouponSkuInfo = couponSkuInfo;

            //是否积攒拼团类型
            var isLikeGroup = couponSkuInfo != null && couponSkuInfo.SKUInfo != null && couponSkuInfo.SKUInfo.Category != null && couponSkuInfo.SKUInfo.Category.ID == 21;
            ViewBag.IsLikeGroup = isLikeGroup;

            //是否全员分销产品
            var isRetail = couponSkuInfo != null && couponSkuInfo.SKUInfo != null && couponSkuInfo.SKUInfo.SKU != null && couponSkuInfo.SKUInfo.SKU.IsRetail;
            ViewBag.IsRetail = isRetail;

            //单买/手拉手SKU
            var singleBuySKU = new SKUEntity();
            var groupBuySKU = new SKUEntity();
            if (couponSkuInfo.SKUInfo != null && couponSkuInfo.SKUInfo.SKUList != null && couponSkuInfo.SKUInfo.SKUList.Count > 0)
            {
                if (couponSkuInfo.SKUInfo.SKUList.Exists(_ => !_.IsGroupSKU))
                {
                    singleBuySKU = couponSkuInfo.SKUInfo.SKUList.Find(_ => !_.IsGroupSKU);    
                }

                if (couponSkuInfo.SKUInfo.SKUList.Exists(_ => _.IsGroupSKU))
                {
                    groupBuySKU = couponSkuInfo.SKUInfo.SKUList.Find(_ => _.IsGroupSKU);
                }
            }
            ViewBag.SingleBuySKU = singleBuySKU;
            ViewBag.GroupBuySKU = groupBuySKU;

            #region 微信环境下，提供关注的公众号信息（公众号名称、二维码等）

            var wxAccountName = "周末酒店服务号";
            var wxAccountQrcodeImg = "http://whfront.b0.upaiyun.com/app/img/qrcode-wx-zmjdservice-346x346.png";
            if (couponSkuInfo != null && couponSkuInfo.activity != null && couponSkuInfo.activity.WeixinAcountId > 0)
            {
                switch ((WeiXinChannelCode)couponSkuInfo.activity.WeixinAcountId)
                {
                    case WeiXinChannelCode.周末酒店服务号_皓颐:
                        wxAccountName = "周末酒店服务号";
                        wxAccountQrcodeImg = "http://whfront.b0.upaiyun.com/app/img/qrcode-wx-zmjdservice-346x346.png";
                        break;
                    case WeiXinChannelCode.遛娃指南:
                    case WeiXinChannelCode.遛娃指南服务号_皓颐:
                        wxAccountName = "遛娃指南服务号";
                        wxAccountQrcodeImg = "http://whfront.b0.upaiyun.com/app/img/qrcode-wx-lwzn-service-258x258.jpg";
                        break;
                    case WeiXinChannelCode.周末酒店苏州服务号_皓颐:
                        wxAccountName = "遛娃指南苏州服务号";
                        wxAccountQrcodeImg = "http://whfront.b0.upaiyun.com/app/img/qrcode-wx-lwzn-sz-service-258x258.jpg";
                        break;
                    case WeiXinChannelCode.尚旅游成都订阅号:
                    case WeiXinChannelCode.周末酒店成都服务号_皓颐: //二维码未配置 2018.08.28 haoy
                        wxAccountName = "遛娃指南成都服务号";
                        wxAccountQrcodeImg = "http://whfront.b0.upaiyun.com/app/img/qrcode-wx-lwzn-chd-service-258x258.jpg";
                        break;
                    case WeiXinChannelCode.周末酒店深圳服务号_皓颐:
                        wxAccountName = "遛娃指南深圳服务号";
                        wxAccountQrcodeImg = "http://whfront.b0.upaiyun.com/app/img/qrcode-wx-lwzn-shz-service-258x258.jpg";
                        break;
                    case WeiXinChannelCode.遛娃指南南京服务号_皓颐:
                        wxAccountName = "遛娃指南南京服务号";
                        wxAccountQrcodeImg = "http://whfront.b0.upaiyun.com/app/img/qrcode-wx-lwzn-nj-service-258x258.jpg";
                        break;
                    case WeiXinChannelCode.遛娃指南无锡服务号_皓颐:
                        wxAccountName = "遛娃指南无锡服务号";
                        wxAccountQrcodeImg = "http://whfront.b0.upaiyun.com/app/img/qrcode-wx-lwzn-wx-service-258x258.jpg";
                        break;
                    case WeiXinChannelCode.遛娃指南广州服务号_皓颐: //二维码未配置 2018.08.28 haoy
                        wxAccountName = "遛娃指南广州服务号";
                        wxAccountQrcodeImg = "http://whfront.b0.upaiyun.com/app/img/qrcode-wx-lwzn-gz-service-258x258.jpg";
                        break;
                    case WeiXinChannelCode.遛娃指南杭州服务号_皓颐:
                        wxAccountName = "遛娃指南杭州服务号";
                        wxAccountQrcodeImg = "http://whfront.b0.upaiyun.com/app/img/qrcode-wx-lwzn-hz-service-258x258.jpg";
                        break;
                }
            }

            ViewBag.WXAccountName = wxAccountName;
            ViewBag.WXAccountQrcodeImg = wxAccountQrcodeImg;

            #endregion

            //获取当前时间戳和提交的时间戳
            var nowTimeStamp = Signature.GenTimeStamp();
            ViewBag.NowTimeStamp = nowTimeStamp;
            ViewBag.TimeStamp = _t;

            //计算出提交-支付完成的时间差
            var _tDate = StampToDateTime(_t);
            var _nowStampDate = StampToDateTime(nowTimeStamp);
            var _difTotalSeconds = _nowStampDate.Subtract(_tDate).TotalSeconds;
            ViewBag.DifTotalSeconds = _difTotalSeconds;

            //购买成功后，获取当前订单可用的红包信息（非团购、非积分、非免费领取的产品，参会需要弹出分享红包功能）
            var canShareRedCoupon = false;
            RedOrderInfoEntity _shareRedEntity = new RedOrderInfoEntity();
            if (Convert.ToInt64(order) > 0)
            {
                try
                {
                    _shareRedEntity = Coupon.GetOrderRed(userid, Convert.ToInt64(order), HJDAPI.Common.Helpers.Enums.OrdersType.ExchangeOrder, couponOrder.TotalPrice);
                    if (_shareRedEntity != null && _shareRedEntity.RedState == 1)
                    {
                        canShareRedCoupon = true;
                    }
                }
                catch (Exception ex)
                {
                    
                }
            }

            //分销产品不弹出红包
            if (couponSkuInfo != null && couponSkuInfo.activity != null && !string.IsNullOrEmpty(couponSkuInfo.activity.MerchantCode) && couponSkuInfo.activity.MerchantCode.ToLower().Trim().Contains("retailer"))
            {
                canShareRedCoupon = false;
            }
            
            //全员分销产品，不弹红包
            if (isRetail)
            {
                canShareRedCoupon = false;
            }

            ViewBag.CanShareRedCoupon = canShareRedCoupon;
            ViewBag.ShareRedEntity = _shareRedEntity;

            //房券url
            var roomCouponUrl = GetUserWalletUrl(userid, "roomcoupon", newpage: true, dorpdown: true);
            ViewBag.RoomCouponUrl = roomCouponUrl;

            //消费券url
            var productCouponUrl = GetUserWalletUrl(userid, "productcoupon", "detail", newpage: true, dorpdown: true);
            ViewBag.ProductCouponUrl = productCouponUrl;

            #region 行为记录

            var _code = "CouponShopPaymentComplete";
            if (groupid > 0)
            {
                _code = "CouponProductForGroupPaymentComplete";
            }
            else if (couponOrder != null && couponOrder.ActivityType == 600)
            {
                _code = "CouponProductPaymentComplete";
            }

            var value = string.Format("{{\"order\":\"{0}\",\"channel\":\"{1}\",\"userid\":\"{2}\"}}", order, channel, userid);
            RecordBehavior(_code, value);

            #endregion

            return View();
        }

        /// <summary>
        /// 发票支付完成页
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="order">payid</param>
        /// <param name="_t">支付时间戳，用于判断非法进入支付完成页面</param>
        /// <param name="code"></param>
        /// <returns></returns>
        public ActionResult PayCompleteInvoice(string channel, string order = "0", long _t = 0, string code = "")
        {
            return View();
        }

        public ActionResult CheckCanPay(string orderid)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();

            //是否大于等于5.4版本
            var isThanVer5_4 = IsThanVer5_4();
            ViewBag.IsThanVer5_4 = isThanVer5_4;

            //房券订单首先要验证是否过期了
            var canpay = new Coupon().IsExchangeOrderCanPay(Convert.ToInt32(orderid));
            if (canpay > 0)
            {
                dict["Success"] = "1";
            }
            else
            {
                dict["Success"] = "0";
            }

            if (IsLatestVerApp())
            {
                var completeUrl = "";
                if (isThanVer5_4)
                {
                    completeUrl = Utils.GetAbsoluteUrl(Url, Url.Action("PayComplete", "Coupon", new { channel = "all", order = orderid, _isoneoff = 1, _clearoneoff = 1 }));
                }
                else
                {
                    completeUrl = Utils.GetAbsoluteUrl(Url, Url.Action("PayComplete", "Coupon", new { channel = "all", order = orderid }));   
                }
                completeUrl = Server.UrlEncode(completeUrl);
                dict["IsLatestVerApp"] = "1";
                dict["Url"] = string.Format("whotelapp://orderPay?orderid={0}&finishurl={1}&paytype={2}", orderid, completeUrl, "all");
            }
            else
            {
                dict["IsLatestVerApp"] = "0";
                dict["Url"] = Url.Action("Pay", "Order", new { order = orderid, payChannels = WHotelSite.Common.Config.DefaultPayChannelList  });
            }

            return Json(dict, JsonRequestBehavior.AllowGet);
        }

        public static string GetCouponActivityUrl(UrlHelper Url, RoomCouponOrderEntity order)
        {
            if (order == null || order.ExchangeCouponList == null || order.ExchangeCouponList.Count == 0)
            {
                return Url.Action("CouponShop", "Coupon", new
                {
                    id = 0,
                    userid = 0,
                });
            }
            return Url.Action("CouponShop", "Coupon", new
            {
                id = order.ExchangeCouponList[0].ActivityID,
                userid = order.ExchangeCouponList[0].UserID.ToString(),
            });
        }

        public static string GetCommCouponActivityUrl(UrlHelper Url, HJD.CouponService.Contracts.Entity.CommOrderEntity order)
        {
            if (order == null || order.RelExchangeCoupon == null || order.RelExchangeCoupon.Count == 0)
            {
                return Url.Action("CouponShop", "Coupon", new
                {
                    id = 0,
                    userid = 0,
                });
            }

            //限时抢购 or 团购 or VIP
            var pageType = "CouponShop";
            if (order.RelExchangeCoupon[0].ActivityType == 300)
            {
                pageType = "CouponShopForGroup";
            }
            else if (order.RelExchangeCoupon[0].ActivityType == 400)
            {
                pageType = "CouponShopForVip";
            }
            else if (order.RelExchangeCoupon[0].ActivityType == 500)
            {
                pageType = "CouponShopForFruitDay";
            }

            return Url.Action(pageType, "Coupon", new
            {
                id = order.RelExchangeCoupon[0].ActivityID,
                userid = order.RelExchangeCoupon[0].UserID.ToString(),
            });
        }

        /// <summary>
        /// 活动抢购列表页【邮件版】
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult CouponShopForMail(int id)
        {
            var isApp = IsApp();
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
            ViewBag.IsApp = isApp;
            ViewBag.Aid = id;

            //调试使用
            ViewBag.AppType = AppType();
            if (AppType() == "iosapp") ViewBag.AppVerForIOS = AppVerForIOS();
            else ViewBag.AppVerForIOS = "";
            if (AppType() == "android") ViewBag.AppVerForAndroid = AppVerForAndroid();
            else ViewBag.AppVerForAndroid = "";
            ViewBag.IsLatestVerApp = IsLatestVerApp();

            ViewBag.CouponDetail = Coupon.GetCouponActivityDetail(id);

            return View();
        }

        /// <summary>
        /// 活动抢购列表页【双12版】
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult CouponShopListFor1212(int advID = 0, int groupNo = 0, string userid = "0")
        {
            var isApp = IsApp();
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
            ViewBag.IsApp = isApp;
            ViewBag.UserId = userid;

            var list = new Coupon().GetSpeciallyCheapRoomCouponActivityList(advID, groupNo);
            ViewBag.ShowList = list;

            return View();
        }

        /// <summary>
        /// 验证指定用户有没有参与双12抢购活动的报名，报过名才能进入抢购
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public ActionResult CheckUserPartActiveStateFor1212(int userid)
        {
            Dictionary<string, string> back = new Dictionary<string, string>();

            var activeCode1212 = 29;
            var phone = "";
            if (userid > 0)
            {
                MemberProfileInfo minfo = account.GetCurrentUserInfo((long)userid);
                if (minfo != null)
                {
                    phone = minfo.MobileAccount;
                }
            }

            if (!string.IsNullOrEmpty(phone))
            {
                if (Weixin.IsPartWeixinActivity(activeCode1212, phone))
                {
                    back["Message"] = "验证通过";
                    back["Success"] = "1";
                    return Json(back, JsonRequestBehavior.AllowGet);
                }
            }

            back["Message"] = "<center>抱歉，您尚未报名！<br />1、关注“周末酒店”微信号<br />2、回复“双十二+手机号码”进行报名</center>";
            back["Success"] = "0";
            return Json(back, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region 兑换房券相关

        /// <summary>
        /// 房券兑换
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userid"></param>
        /// <param name="hotelid"></param>
        /// <param name="pid"></param>
        /// <returns></returns>
        public ActionResult CouponExchange(int id, string userid = "0", int hotelid = 0, int pid = 0)
        {
             try
            {
                var isApp = IsApp();
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
                ViewBag.IsApp = isApp;

                //当前系统环境（ios | android）
                var appType = AppType();
                ViewBag.AppType = appType;

                ViewBag.UserId = userid;

                //房券套餐信息
                ExchangeCouponPackageInfo couponPackage = Coupon.GetCouponPackageInfo(id);

                //手机号过滤
                couponPackage.exchangeInfo.PhoneNum = couponPackage.exchangeInfo.PhoneNum.Trim();

                var hotelName = couponPackage.exchangeInfo.ObjectName;
                var packageEntity = new PackageInfoEntity();
                var calendar = new List<HJD.HotelServices.Contracts.PDayItem>();

                //如果是500的券，则套餐单独获取，正常来说，如果是兑换500的券，pid参数必须要传递
                if (couponPackage.exchangeInfo.ActivityType == 500)
                {
                    //获取当前套餐信息
                    var startTime = DateTime.Now.Date;
                    HotelPrice2 price = HJDAPI.APIProxy.Price.GetV42(hotelid, startTime.ToString("yyyy-MM-dd"), startTime.AddDays(1).ToString("yyyy-MM-dd"), Utils.ClientType(), needNotSalePackage:true);
                    foreach (PackageInfoEntity entity in price.Packages)
                    {
                        if (entity.packageBase.ID == pid)
                        {
                            packageEntity = entity;
                        }
                    }

                    hotelName = price.Name;

                    //当前套餐的可用日期
                    var calendarEntity = HJDAPI.APIProxy.Price.GetOneHotelPackageCalendar(packageEntity.packageBase.HotelID, DateTime.Now.Date, packageEntity.packageBase.ID, Convert.ToInt64(userid));
                    calendar = calendarEntity.DayItems;

                    ViewBag.DayLimitMin = calendarEntity.DayLimitMin;
                    ViewBag.DayLimitMax = calendarEntity.DayLimitMax;

                    //当前券的价值
                    var oriCouponPrice = couponPackage.exchangeInfo.Price;

                    //对日历做补差价过滤
                    for (int i = 0; i < calendar.Count; i++)
                    {
                        var citem = calendar[i];
                        if (citem.SellPrice <= oriCouponPrice)
                        {
                            citem.SellPrice = 0;
                        }
                        else
                        {
                            citem.SellPrice = (citem.SellPrice - (int)oriCouponPrice);
                        }
                    }

                    ViewBag.Pid = pid;
                }
                else
                {
                    //根据当前免费品鉴酒店的酒店 以及 其对应的“免费品鉴”套餐 获取日历
                    var hotelPackageCalendar = HJDAPI.APIProxy.Price.GetOneHotelPackageCalendarWithCustomerType(couponPackage.packageInfo.packageBase.HotelID, DateTime.Now.Date, couponPackage.packageInfo.packageBase.ID, (HJDAPI.Common.Helpers.Enums.CustomerType)couponPackage.exchangeInfo.CustomerType);
                    if (hotelPackageCalendar != null && hotelPackageCalendar.DayItems != null)
                    {
                        calendar = hotelPackageCalendar.DayItems;
                    }

                    packageEntity = couponPackage.packageInfo;

                    ViewBag.DayLimitMin = couponPackage.packageInfo.packageBase.DayLimitMin;
                    ViewBag.DayLimitMax = couponPackage.packageInfo.packageBase.DayLimitMax;

                    ViewBag.Pid = couponPackage.packageInfo.packageBase.ID;
                }

                //券套餐信息
                ViewBag.HotelName = hotelName;
                ViewBag.PackageEntity = packageEntity;

                //根据总的间晚数，以及默认入住一天，得出需要显示的入住人数目
                var nightRoomCount = (couponPackage.exchangeInfo.NightRoomCount > 0 ? couponPackage.exchangeInfo.NightRoomCount : 1);
                var defaultRoomCount = nightRoomCount;

                //设置默认选择的入住到离店的天数
                var checkOutAddDays = 1;
                if (!couponPackage.isAllowMultiRoom)
                {
                    checkOutAddDays = nightRoomCount;
                    defaultRoomCount = 1;
                }

                //现调整为，默认始终是1（2018.07.20 haoy）
                defaultRoomCount = 1;

                ViewBag.NightRoomCount = nightRoomCount;
                ViewBag.DefaultRoomCount = defaultRoomCount;

                //入住日期&离店日期

                var checkIn = DateTime.Now.AddDays(1).Date;
                var checkOut = checkIn.AddDays(checkOutAddDays);

                //过滤日期
                //calendar = FilterCalendar(calendar, couponPackage.exchangeInfo.IsFestivalCanUse == 1, couponPackage.exchangeDateType, couponPackage.canExchangeDates);
                ViewBag.calendar = calendar;

                //根据日历得出最近一个可以入住的日期
                if (calendar != null && calendar.Count > 0 && calendar.Exists(d => d.SellState == 1))
                {
                    checkIn = calendar.Where(d => d.SellState == 1).OrderBy(d => d.Day).ToList()[0].Day.Date;
                    checkOut = checkIn.AddDays(checkOutAddDays);
                }

                ViewBag.CheckIn = checkIn;
                ViewBag.CheckOut = checkOut;

                ViewBag.CouponPackage = couponPackage;

                List<string> notes = packageEntity.Room.DefaultOption.Split(new char[] { ',' }).ToList();
                ViewBag.Notes = notes;
            }
            catch( Exception err)
            {
                LogHelper.WriteLog("CouponExchange:" + err.Message + err.StackTrace);
            }

             return View();
        }

        /// <summary>
        /// 房券兑换.获取所选日期的套餐房间满房信息更新
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userid"></param>
        /// <param name="hotelid"></param>
        /// <param name="pid"></param>
        /// <returns></returns>
        public string GetOnePackageRoomOptions(int hotelid, int pid, DateTime checkIn, DateTime checkOut)
        {
            PackageInfoEntity pinfo = Price.GetOnePackageInfoEntity(pid, hotelid, checkIn, checkOut, 0);

            return pinfo.Room.Options;
        }


        /// <summary>
        /// 获取一个指定日期量的空日历列表
        /// </summary>
        private static List<HJD.HotelServices.Contracts.PDayItem> GetDefaultCalendar(DateTime startDate, int canlendarLength = 90)
        {
            DateTime packageEndDate = startDate.AddDays(canlendarLength);
            DateTime endDate = startDate.AddDays(canlendarLength) < packageEndDate ? startDate.AddDays(canlendarLength) : packageEndDate;

            List<HJD.HotelServices.Contracts.PDayItem> ds = new List<HJD.HotelServices.Contracts.PDayItem>();
            for (int i = 0; i < canlendarLength; i++)
            {
                if (startDate.AddDays(i) > endDate) break;

                HJD.HotelServices.Contracts.PDayItem d = new HJD.HotelServices.Contracts.PDayItem();
                d.Day = startDate.AddDays(i).Date;
                d.MaxSealCount = 5;
                d.SellState = 0;

                //FillDailyItems
                var pdayPItem = new HJD.HotelServices.Contracts.PDayPItem
                {
                    Day = d.Day.Date,
                    MaxSealCount = 5,
                    SoldCount = 0,
                    PID = 0
                };

                d.PItems = new List<HJD.HotelServices.Contracts.PDayPItem>();
                d.PItems.Add(pdayPItem);

                ds.Add(d);
            }
            return ds;
        }

        /// <summary>
        /// 筛选日历
        /// </summary>
        private static List<HJD.HotelServices.Contracts.PDayItem> FilterCalendar(List<HJD.HotelServices.Contracts.PDayItem> calendar, bool canHoliday, int priceType, List<DateTime> canExchangeDates)
        {
            foreach (var item in calendar)
            {
                if (canExchangeDates != null && canExchangeDates.Count > 0)
                {
                    if (item.SellState == 1 && !canExchangeDates.Exists(hd => hd.Date == item.Day.Date))
                    {
                        item.SellState = 0;
                    }
                }
            }

            return calendar;
        }

        /// <summary>
        /// 获取指定日期需要补的差价
        /// </summary>
        /// <param name="hotelid"></param>
        /// <param name="pid"></param>
        /// <param name="cin"></param>
        /// <param name="cout"></param>
        /// <param name="oriCouponPrice"></param>
        /// <returns></returns>
        public ActionResult GetExchangeDiffPrice(int hotelid, int pid, string cin, string cout, int oriCouponPrice)
        {
            Dictionary<string, string> back = new Dictionary<string, string>();

            //获取当前套餐信息
            var startTime = DateTime.Now.Date;
            HotelPrice2 price = HJDAPI.APIProxy.Price.GetV42(hotelid, startTime.ToString("yyyy-MM-dd"), startTime.AddDays(1).ToString("yyyy-MM-dd"), Utils.ClientType());
            PackageInfoEntity packageEntity = null;
            foreach (PackageInfoEntity entity in price.Packages)
            {
                if (entity.packageBase.ID == pid)
                {
                    packageEntity = entity;
                }
            }
            ViewBag.PackageEntity = packageEntity;

            //当前套餐的可用日期
            var calendarEntity = HJDAPI.APIProxy.Price.GetOneHotelPackageCalendar(packageEntity.packageBase.HotelID, DateTime.Now.Date, packageEntity.packageBase.ID, Convert.ToInt64(0));
            var calendar = calendarEntity.DayItems;

            var checkIn = DateTime.Parse(cin);
            var checkOut = DateTime.Parse(cout);

            //统计差价
            var diffPrice = 0;
            for (int i = 0; i < calendar.Count; i++)
            {
                var citem = calendar[i];
                if (citem.SellPrice <= oriCouponPrice)
                {
                    citem.SellPrice = 0;
                }
                else
                {
                    citem.SellPrice = (citem.SellPrice - oriCouponPrice);
                }

                //sum diff price
                if (citem.Day >= checkIn && citem.Day < checkOut)
                {
                    diffPrice += citem.SellPrice;
                }
            }

            back["Price"] = diffPrice.ToString();
            return Json(back, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 房券兑换前检测
        /// </summary>
        /// <returns></returns>
        public ActionResult CheckExChange()
        {
            Dictionary<string, string> back = new Dictionary<string, string>();
            SubmitExchangeRoomOrderParam sp = new SubmitExchangeRoomOrderParam();
            sp.contact = this.Request.Params["contact"].ToString();
            sp.contactPhone = this.Request.Params["contactPhone"].ToString();
            sp.exchangeNo = this.Request.Params["exchangeNo"].ToString();
            sp.checkIn = DateTime.Parse(this.Request.Params["checkIn"].ToString());
            sp.nightCount = Convert.ToInt32(this.Request.Params["nightCount"].ToString());
            sp.roomCount = Convert.ToInt32(this.Request.Params["roomCount"].ToString());
            sp.packageID = Convert.ToInt32(this.Request.Params["packageID"].ToString());
            sp.packageType = Convert.ToInt32(this.Request.Params["packageType"].ToString());
            sp.note = this.Request.Params["note"].ToString();
            sp.note = sp.note.Replace("\n\n", " ").Replace("\r\n", " ");
            sp.userID = Convert.ToInt32(this.Request.Params["userId"].ToString());
            sp.hotelID = Convert.ToInt32(this.Request.Params["hotelId"].ToString());
            sp.terminalID = Utils.GetTerminalId(Request.UserAgent);
            sp.channelID = Utils.GetChannelId(Request.UserAgent, Session["ChannelID"]);

            //check
            ExchangeRoomOrderConfirmResult result = new Coupon().IsExchangeNeedAddMoney(sp);
            back["Message"] = result.Message;
            back["Success"] = result.AddMoneyAmount < 0 ? ((int)result.AddMoneyAmount).ToString() : (result.IsNeedAddMoney ? "1" : "0");
            back["useCouponNum"] = result.UseCouponCount.ToString();//使用的券的数量
            return Json(back, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 提交房券兑换
        /// </summary>
        /// <returns></returns>
        public ActionResult SubmitExChange()
        {
            Dictionary<string, string> back = new Dictionary<string, string>();

            SubmitExchangeRoomOrderParam sp = new SubmitExchangeRoomOrderParam();
            sp.contact = this.Request.Params["contact"].ToString();
            sp.contactPhone = this.Request.Params["contactPhone"].ToString();
            sp.exchangeNo = this.Request.Params["exchangeNo"].ToString();
            sp.checkIn = DateTime.Parse(this.Request.Params["checkIn"].ToString());
            sp.nightCount = Convert.ToInt32(this.Request.Params["nightCount"].ToString());
            sp.roomCount = Convert.ToInt32(this.Request.Params["roomCount"].ToString());
            sp.packageID = Convert.ToInt32(this.Request.Params["packageID"].ToString());
            sp.packageType = Convert.ToInt32(this.Request.Params["packageType"].ToString());
            sp.note = this.Request.Params["note"].ToString();
            sp.note = sp.note.Replace("\n\n", " ").Replace("\r\n", " ");
            sp.userID = Convert.ToInt32(this.Request.Params["userId"].ToString());
            sp.hotelID = Convert.ToInt32(this.Request.Params["hotelId"].ToString());
            sp.terminalID = Utils.GetTerminalId(Request.UserAgent);
            sp.channelID = Utils.GetChannelId(Request.UserAgent, Session["ChannelID"]);

            //出行人
            var _travelPersonIdList = new List<int>();
            if (this.Request.Params["travelPersons"] != null && !string.IsNullOrEmpty(this.Request.Params["travelPersons"].ToString()))
            {
                try
                {
                    var _travelPersons = this.Request.Params["travelPersons"].ToString();
                    _travelPersonIdList = _travelPersons.Split(',').ToList().Select(_id => Convert.ToInt32(_id)).ToList();
                }
                catch (Exception ex)
                {
                    //return Content("出行人读取错误，请重试");
                }
            }
            sp.travelId = _travelPersonIdList;

            //submit
            SubmitExchangeRoomOrderResult result = Coupon.SubmitExchangeRoomOrder(sp);
            back["Message"] = result.Message;
            back["Success"] = result.Success.ToString();
            return Json(back, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region VIP购买/VIP说明页 等

        public RedirectToRouteResult VipShopInfoForInvatation(int CID = 0)
        {
            SetIsVIPInvatation(true);
            return RedirectToAction("VipShopInfo", "Coupon", new { CID = CID, isVIPInvatation = 1 });
        }

        /// <summary>
        /// VIP会员专区
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="CID"></param>
        /// <param name="isVIPInvatation"></param>
        /// <param name="Activity"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public ActionResult VipAreaInfo(long userid = 0, int CID = 0, int isVIPInvatation = 0, int Activity = 0, string code = "")
        {
            var isApp = IsApp();
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

            //获取请求参数中的CID
            var requestCID = GetCurCIDForRequest();
            ViewBag.RequestCID = requestCID;

            ////如果当前用户VIP，则直接重定向VIP权益页面
            //var isVip = account.IsVIPCustomer(new AccountInfoItem { Uid = userid.ToString() });
            //if (isVip)
            //{
            //    return Redirect(string.Format("/Account/VipRights?userid={0}", userid));
            //}

            if (isVIPInvatation == 1)
            {
                SetIsVIPInvatation(true);
            }

            string agent = HttpContext.Request.UserAgent;
            string[] keywords = { "Android", "iPhone", "iPod", "iPad", "Windows Phone", "MQQBrowser" };
            // agent = "Mozilla/5.0 (iPhone; CPU iPhone OS 8_1_2 like Mac OS X) AppleWebKit/600.1.4 (KHTML, like Gecko) Mobile/12B440 MicroMessenger/6.1 NetType/WIFI";
            var isInWeixin = agent.IndexOf("MicroMessenger") > 0;
            ViewBag.IsInWeixin = isInWeixin;

            #region 微信环境下，做微信静默授权

            WeixinUserInfo weixinUserInfo = new WeixinUserInfo();
            var openid = "";
            var unionid = "";
            var wxuid = 0;
            if (isInWeixin)
            {
                if (!string.IsNullOrEmpty(code))
                {
                    var weixinAcountName = "weixinservice_haoyi";

                    //通过code换取网页授权access_token
                    var accessToken = WeiXinHelper.GetWeixinAccessTokenForHaoYi(code);
                    if (accessToken != null && !string.IsNullOrEmpty(accessToken.Openid))
                    {
                        openid = accessToken.Openid;

                        //获取用户信息
                        //通过access_token拉取用户信息(需scope为 snsapi_userinfo)（这是通过微信api获取的真实用户信息）
                        weixinUserInfo = WeiXinHelper.GetWeixinUserInfo(accessToken.AccessToken, accessToken.Openid);
                        if (weixinUserInfo != null && !string.IsNullOrEmpty(weixinUserInfo.Openid))
                        {
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
                                    WeixinAcount = weixinAcountName,   //WeiXinChannelCode.周末酒店服务号_皓颐
                                    Language = "zh_CN",
                                    SubscribeTime = DateTime.Now,
                                    CreateTime = DateTime.Now
                                };
                                var update = new Weixin().UpdateWeixinUserSubscribe(w);

                                //get weixin user
                                var weixinUser = new Weixin().GetWeixinUser(openid);
                                if (weixinUser != null && weixinUser.ID > 0)
                                {
                                    wxuid = weixinUser.ID;
                                }
                            }
                            catch (Exception ex)
                            {

                            }

                            #region 处理微信号绑定用户信息相关操作

                            //微信环境下，如果当前已经登录，则直接绑定
                            if (userid > 0)
                            {
                                WeiXinHelper.BindingWxAndUid(userid, weixinUserInfo.Unionid);
                            }

                            #endregion

                            #region 更新Unionid缓存并更新CID信息

                            SetCurWXUnionid(weixinUserInfo.Unionid);
                            CheckCID();

                            #endregion
                        }
                    }
                    else
                    {
                        //如果code有值，但不能获取，一般说明code过期了，那么重新获取
                        var weixinGoUrl = WeiXinHelper.GenSilenceAuthorUrlForHaoYi(HttpUtility.UrlEncode(string.Format("http://www.zmjiudian.com/coupon/VipAreaInfo?userid={0}&CID={1}&isVIPInvatation={2}&Activity={3}", userid, CID, isVIPInvatation, Activity)));
                        return Redirect(weixinGoUrl);
                    }
                }
                else
                {
                    //授权页面
                    var weixinGoUrl = WeiXinHelper.GenSilenceAuthorUrlForHaoYi(HttpUtility.UrlEncode(string.Format("http://www.zmjiudian.com/coupon/VipAreaInfo?userid={0}&CID={1}&isVIPInvatation={2}&Activity={3}", userid, CID, isVIPInvatation, Activity)));
                    return Redirect(weixinGoUrl);
                }
            }

            #endregion

            ViewBag.Openid = openid;
            ViewBag.Unionid = unionid;
            ViewBag.WeixinUid = wxuid;

            //获取当前用户信息
            var userInfo = account.GetUserInfoByUserID(userid);
            ViewBag.UserInfo = userInfo ?? new UserInfoResult();

            //如果当前抢购VIP专享，则判断当前用户是否为VIP
            var isVip = account.IsVIPCustomer(new AccountInfoItem { Uid = userid.ToString() });
            ViewBag.IsVip = isVip;

            //获取用户权限
            var isOldVip = true;
            if (isVip)
            {
                var _userInfoPrivileges = account.GetAllPrivilegeByUserId(new AccountInfoItem { Uid = userid.ToString() });
                if (_userInfoPrivileges.Exists(_p => _p.PrivID == 2010))
                {
                    isOldVip = false;
                }
            }
            else 
            {
                isOldVip = false;
            }
            ViewBag.IsOldVip = isOldVip;

            //单独拿出手机号使用
            var userPhone = "";
            if (userInfo != null && !string.IsNullOrEmpty(userInfo.Mobile))
            {
                userPhone = userInfo.Mobile;
            }
            ViewBag.UserPhone = userPhone;

            ViewBag.MagiCallUrl = GetMagicallUrl(Convert.ToInt64(userid));

            //现金券URL
            var cashCouponUrl = GetUserWalletUrl(Convert.ToInt64(curUserID), "coupon", "", newpage: true, dorpdown: true);
            ViewBag.CashCouponLink = cashCouponUrl;

            //积分URL
            var pointsUrl = GetUserWalletUrl(Convert.ToInt64(curUserID), "point", "", newpage: true, dorpdown: true);
            ViewBag.PointsUrl = pointsUrl;

            #region 分享配置

            var shareLink = "";
            if (requestCID > 0)
            {
                //分享链接生成
                if (Activity > 0)
                {
                    shareLink = string.Format("http://www.zmjiudian.com/Coupon/VipAreaInfo?CID={0}&Activity={1}", requestCID, Activity);
                }
                else 
                {
                    shareLink = string.Format("http://www.zmjiudian.com/Coupon/VipAreaInfo?CID={0}", requestCID);
                }
            }
            else
            {
                //暂时只有在APP中登录分享才会加sid&cid等参数
                if (isApp)
                {
                    //分享链接生成
                    if (Activity > 0)
                    {
                        shareLink = string.Format("http://www.zmjiudian.com/Coupon/VipAreaInfo?CID={0}&Activity={1}", userid, Activity);
                    }
                    else
                    {
                        shareLink = string.Format("http://www.zmjiudian.com/Coupon/VipAreaInfo?CID={0}", userid);   
                    }
                }
                else
                {
                    if (Activity > 0)
                    {
                        shareLink = string.Format("http://www.zmjiudian.com/Coupon/VipAreaInfo?CID={0}&Activity={1}", userid, Activity);
                    }
                    else
                    {
                        shareLink = string.Format("http://www.zmjiudian.com/Coupon/VipAreaInfo?CID={0}", userid);   
                    }
                }
            }

            ViewBag.ShareLink = shareLink;

            #endregion

            #region 行为记录

            var value = string.Format("{{\"userid\":\"{0}\",\"CID\":\"{1}\",\"isVIPInvatation\":\"{2}\",\"Activity\":\"{3}\"}}", userid, CID, isVIPInvatation, Activity);
            RecordBehavior("VipShopInfoLoad", value);

            #endregion

            return View();
        }

        /// <summary>
        /// 成为VIP介绍页面
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="isVIPInvatation"></param>
        /// <returns></returns>
        public ActionResult VipShopInfo(long userid = 0, int CID = 0, int isVIPInvatation = 0, int Activity = 0)
        {
            //2017-09-13晚开始，之前旧的购买VIP页面，全部转向新的 VIP专区 页面VipAreaInfo
            return Redirect(string.Format("/Coupon/VipAreaInfo?userid={0}&CID={1}&isVIPInvatation={2}&Activity={3}", userid, CID, isVIPInvatation, Activity));

            var isApp = IsApp();
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
            ViewBag.IsApp = isApp;

            var curUserID = Convert.ToInt32(UserState.UserID > 0 ? UserState.UserID : userid);
            ViewBag.UserId = curUserID;
            userid = curUserID;

            //获取请求参数中的CID
            var requestCID = GetCurCIDForRequest();
            ViewBag.RequestCID = requestCID;

            //如果当前用户VIP，则直接重定向VIP权益页面
            var isVip = account.IsVIPCustomer(new AccountInfoItem { Uid = userid.ToString() });
            if (isVip)
            {
                return Redirect(string.Format("/Account/VipRights?userid={0}", userid));
            }

            if (isVIPInvatation == 1)
            {
                SetIsVIPInvatation(true);
            }

            string agent = HttpContext.Request.UserAgent;
            string[] keywords = { "Android", "iPhone", "iPod", "iPad", "Windows Phone", "MQQBrowser" };
            // agent = "Mozilla/5.0 (iPhone; CPU iPhone OS 8_1_2 like Mac OS X) AppleWebKit/600.1.4 (KHTML, like Gecko) Mobile/12B440 MicroMessenger/6.1 NetType/WIFI";
            ViewBag.IsInWeixin = agent.IndexOf("MicroMessenger") > 0;

            //获取当前用户信息
            var userPhone = "";
            if (userid > 0)
            {
                try
                {
                    var userEntity = account.GetCurrentUserInfo(userid);
                    if (userEntity != null)
                    {
                        userPhone = userEntity.MobileAccount;
                    }
                }
                catch (Exception ex)
                {

                }
            }
            ViewBag.UserPhone = userPhone;

            ViewBag.MagiCallUrl = GetMagicallUrl(Convert.ToInt64(userid));

            #region 分享配置

            var shareLink = "";
            if (requestCID > 0)
            {
                //分享链接生成
                if (Activity > 0)
                {
                    shareLink = string.Format("http://www.zmjiudian.com/Coupon/VipShopInfo?CID={0}&Activity={1}", requestCID, Activity);
                }
                else
                {
                    shareLink = string.Format("http://www.zmjiudian.com/Coupon/VipShopInfo?CID={0}", requestCID);
                }
            }
            else
            {
                //暂时只有在APP中登录分享才会加sid&cid等参数
                if (isApp)
                {
                    //分享链接生成
                    if (Activity > 0)
                    {
                        shareLink = string.Format("http://www.zmjiudian.com/Coupon/VipShopInfo?CID={0}&Activity={1}", userid, Activity);
                    }
                    else
                    {
                        shareLink = string.Format("http://www.zmjiudian.com/Coupon/VipShopInfo?CID={0}", userid);
                    }
                }
                else
                {
                    if (Activity > 0)
                    {
                        shareLink = string.Format("http://www.zmjiudian.com/Coupon/VipShopInfo?CID={0}&Activity={1}", userid, Activity);
                    }
                    else
                    {
                        shareLink = string.Format("http://www.zmjiudian.com/Coupon/VipShopInfo?CID={0}", userid);
                    }
                }
            }

            ViewBag.ShareLink = shareLink;

            #endregion

            #region 行为记录

            var value = string.Format("{{\"userid\":\"{0}\",\"CID\":\"{1}\",\"isVIPInvatation\":\"{2}\",\"Activity\":\"{3}\"}}", userid, CID, isVIPInvatation, Activity);
            RecordBehavior("VipShopInfoLoad", value);

            #endregion

            return View();
        }


        public ActionResult GetUserPhone(int userid)
        {
            //获取当前用户信息
            var userPhone = "";
            if (userid > 0)
            {
                try
                {
                    var userEntity = account.GetCurrentUserInfo(userid);
                    if (userEntity != null)
                    {
                        userPhone = userEntity.MobileAccount;
                    }
                }
                catch (Exception ex)
                {

                }
            }
            return Json(userPhone, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 赠送积分
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="phone"></param>
        /// <returns></returns>
        public ActionResult GiftCashCoupon(int userid, string phone)
        {
            Dictionary<string, string> re = new Dictionary<string, string>();

            var _param = new PresentCashCouponParam();
            _param.userID = userid;
            _param.typeID = CouponActivityCode.newVIPPresent;
            _param.phoneNo = phone;
            var _result = CouponAdapter.PresentCashCoupon(_param);

            return Json(_result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// VIP卡片购买列表页面
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public ActionResult VipCartList(int userid = 0)
        {
            var isApp = IsApp();
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
            ViewBag.IsApp = isApp;
            ViewBag.UserId = userid;

            //调试使用
            ViewBag.AppType = AppType();
            if (AppType() == "iosapp") ViewBag.AppVerForIOS = AppVerForIOS();
            else ViewBag.AppVerForIOS = "";
            if (AppType() == "android") ViewBag.AppVerForAndroid = AppVerForAndroid();
            else ViewBag.AppVerForAndroid = "";
            ViewBag.IsLatestVerApp = IsLatestVerApp();

            return View();
        }

        /// <summary>
        /// 199VIP会员【购买】
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userid"></param>
        /// <param name="sid"></param>
        /// <returns></returns>
        public ActionResult CouponShopForVip(int id, string userid = "0", string sid = "")
        {
            var isApp = IsApp();
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
            ViewBag.IsApp = isApp;

            var curUserID = UserState.UserID > 0 ? UserState.UserID.ToString() : userid;
            ViewBag.UserId = curUserID;
            userid = curUserID;

            //暂时避免用户通过旧的链接进来购买“老199会员”，这里将100399转换为100929 haoy 2017.02.23
            if (id == 100399) id = 100929;
            ViewBag.Aid = id;

            //获取当前用户信息
            var userPhone = "";
            if (!string.IsNullOrEmpty(userid) && userid != "0")
            {
                try
                {
                    var userEntity = account.GetCurrentUserInfo(Convert.ToInt64(userid));
                    if (userEntity != null)
                    {
                        userPhone = userEntity.MobileAccount;
                    }
                }
                catch (Exception ex)
                {

                }
            }
            ViewBag.UserPhone = userPhone;

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

            //获取当前用户信息
            var userInfo = account.GetUserInfoByUserID(string.IsNullOrEmpty(userid) ? 0 : Convert.ToInt64(userid));
            ViewBag.UserInfo = userInfo ?? new UserInfoResult();

            //产品信息
            var coupondetail = Coupon.GetGroupCouponActivityDetail(id);

            ViewBag.CouponDetail = coupondetail;

            //查询当前券和用户的可售状态
            var buyResult = new Coupon().IsExchangeCouponSoldOut(id, Convert.ToInt64(userid));
            ViewBag.BuyResult = buyResult;

            #region 分享配置

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
                scp.UserID = Convert.ToInt64(userid);
                scp.CouponActivityID = id;
                scp.BizType = ZMJDShareTrackBizType.roomgroupcoupondetail;
                var shareCode = Comment.GenTrackCodeResult4Share(scp);
            }

            ViewBag.ShareUserInfo = shareUserInfo;

            #endregion

            return View();
        }

        /// <summary>
        /// 199VIP会员【预约】
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userid"></param>
        /// <param name="sid"></param>
        /// <param name="canreserve">是否可预约</param>
        /// <returns></returns>
        public ActionResult CouponShopForVip2(int id, string userid = "0", string sid = "", int canreserve = 0)
        {
            var isApp = IsApp();
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
            ViewBag.IsApp = isApp;
            ViewBag.Aid = id;
            ViewBag.UserId = userid;

            //检查当前用户是否属于【预约 & [品鉴师|候选|有预订记录] & 非会员】的用户，是的话则直接跳转至支付页面
            var isOkUser = new Coupon().IsVipNoPayReserveUser(userid.ToString());
            if (isOkUser > 0)
            {
                var toPayUrl = string.Format("http://www.zmjiudian.com/custom/shop/vip/{0}?userid={1}", id, userid);
                return Redirect(toPayUrl);
            }

            //调试使用
            ViewBag.AppType = AppType();
            if (AppType() == "iosapp") ViewBag.AppVerForIOS = AppVerForIOS();
            else ViewBag.AppVerForIOS = "";
            if (AppType() == "android") ViewBag.AppVerForAndroid = AppVerForAndroid();
            else ViewBag.AppVerForAndroid = "";
            ViewBag.IsLatestVerApp = IsLatestVerApp();

            //产品信息
            var coupondetail = Coupon.GetGroupCouponActivityDetail(id);

            ViewBag.CouponDetail = coupondetail;

            //查询当前券和用户的可售状态
            var buyResult = new Coupon().IsExchangeCouponSoldOut(id, Convert.ToInt64(userid));
            ViewBag.BuyResult = buyResult;

            #region 分享配置

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
                scp.UserID = Convert.ToInt64(userid);
                scp.CouponActivityID = id;
                scp.BizType = ZMJDShareTrackBizType.roomgroupcoupondetail;
                var shareCode = Comment.GenTrackCodeResult4Share(scp);
            }

            ViewBag.ShareUserInfo = shareUserInfo;

            #endregion

            //是否可以预约 0预约结束 1可预约
            ViewBag.CanReserve = canreserve;

            return View();
        }

        public ActionResult CouponShopForVip2Result(int aid = 0)
        {
            var isApp = IsApp();
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
            ViewBag.IsApp = isApp;
            ViewBag.Aid = aid;

            //获取指定活动的预约情况
            var allList = new Weixin().GetLuckyDrawByActiveCode(aid);
            if (allList != null) allList = allList.OrderByDescending(a => a.ID).ToList();
            ViewBag.AllList = allList;

            return View();
        }

        /// <summary>
        /// 检查VIP券购买状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="buynum"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public ActionResult CheckBuyNumberForVip(int id, int buynum, string userid = "0")
        {
            Dictionary<string, string> re = new Dictionary<string, string>();

            //获取当前券的信息
            var coupon = Coupon.GetCouponActivityDetail(id);

            //查询当前券和用户的可售状态
            var buyResult = new Coupon().IsExchangeCouponSoldOut(id, Convert.ToInt64(userid));

            //首先检查当前券是否已经售完
            if (buyResult.ActivityState == 0)// || true
            {
                re["Message"] = "名额已满";
                re["Success"] = "0";
                re["CanSell"] = "0";
                return Json(re, JsonRequestBehavior.AllowGet);
            }

            //检查当前用户购买量是否超过了当前券规定单人购买限额
            //已经彻底不能购买了
            if (buyResult.UserBuyNum >= coupon.activity.SingleBuyNum)
            {
                re["Message"] = string.Format("您已经成功购买周末酒店VIP会员");
                re["Success"] = "1";
                re["CanSell"] = "1";
                return Json(re, JsonRequestBehavior.AllowGet);
            }

            //还可以购买，但不能买那么多了
            if (buynum + buyResult.UserBuyNum > coupon.activity.SingleBuyNum)
            {
                re["Message"] = string.Format("您已经成功购买周末酒店VIP会员");
                re["Success"] = "2";
                re["CanSell"] = (coupon.activity.SingleBuyNum - buyResult.UserBuyNum).ToString();
                return Json(re, JsonRequestBehavior.AllowGet);
            }

            //检查当前购买量是否超过了券的剩余购买量
            if (buynum > buyResult.ActivityNoLockNum)
            {
                if (buyResult.ActivityNoLockNum > 0)
                {
                    re["Message"] = string.Format("限购{0}名，部分用户未支付完成，稍后可能有名额放出，请耐心等待", buyResult.ActivityNoLockNum);
                }
                else
                {
                    re["Message"] = string.Format("名额已满，部分用户未支付完成，稍后可能有名额放出，请耐心等待", buyResult.ActivityNoLockNum);
                }
                re["Success"] = "3";
                re["CanSell"] = buyResult.ActivityNoLockNum.ToString();
                return Json(re, JsonRequestBehavior.AllowGet);
            }

            return Json(re, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// VIP购买提交
        /// </summary>
        /// <param name="aid"></param>
        /// <param name="atype"></param>
        /// <param name="pid"></param>
        /// <param name="pricetype"></param>
        /// <param name="paynum"></param>
        /// <param name="userid"></param>
        /// <param name="realname"></param>
        /// <returns></returns>
        public ActionResult SubmitVipConpon(int aid, int atype, int pid, int pricetype, int paynum, string userid = "0", string realname = "")
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict["Message"] = "";
            dict["Success"] = "";
            dict["Url"] = "";

            SubmitCouponOrderModel sub = new SubmitCouponOrderModel();
            sub.ActivityID = aid;
            sub.ActivityType = atype;
            sub.UserID = Convert.ToInt64(userid);
            sub.CID = GetCurCID();
            int SKUID = 0;
            int.TryParse(GetCurActivity(), out SKUID);
            sub.SKUID = SKUID;
            sub.IsVIPInvatation = GetIsVIPInvatation();
            sub.AddInfo = new ExchangeCouponAddInfoItem();
            sub.OrderItems = new List<ProductAndNum> 
            { 
                new ProductAndNum
                {
                    SourceID = pid,
                    Number = paynum,
                    Type = pricetype
                }
            };
            sub.RealName = realname;

            //是否大于等于5.4版本
            var isThanVer5_4 = IsThanVer5_4();
            ViewBag.IsThanVer5_4 = isThanVer5_4;

            //将
            ////增加验证，目前支付的用户必须是
            //var isOkUser = new Coupon().IsVipNoPayReserveUser(userid.ToString());
            //if (isOkUser <= 0)
            //{
            //    dict["Message"] = "抱歉，您没有预约或不符合购买资格！";
            //    dict["Success"] = "-1";
            //    dict["Url"] = "";
            //    return Json(dict, JsonRequestBehavior.AllowGet);
            //}

            //验证当前用户是否已经购买会员
            //CheckZMJDMemberResponse checkMemberResult = account.IsZMJDMember(Convert.ToInt64(userid), "");
            //if (checkMemberResult.isMember)
            //{
            //    dict["Message"] = "您已经是周末酒店VIP会员了！";
            //    dict["Success"] = "-1";
            //    dict["Url"] = "";
            //    return Json(dict, JsonRequestBehavior.AllowGet);
            //}

            CouponOrderResult order = Coupon.SubmitMemberOrder(sub);
            if (order != null)
            {
                switch (order.Success)
                {
                    case 0:
                    case 5:
                        {
                            if (IsLatestVerApp())
                            {
                                var completeUrl = "";
                                if (isThanVer5_4)
                                {
                                    completeUrl = Utils.GetAbsoluteUrl(Url, Url.Action("PayCompleteForVip", "Coupon", new { channel = WHotelSite.Common.Config.DefaultPayChannelList, order = order.OrderID.ToString(), _isoneoff = 1, _clearoneoff = 1 }));
                                }
                                else
                                {
                                    completeUrl = Utils.GetAbsoluteUrl(Url, Url.Action("PayCompleteForVip", "Coupon", new { channel = WHotelSite.Common.Config.DefaultPayChannelList, order = order.OrderID.ToString() }));   
                                }
                                completeUrl = Server.UrlEncode(completeUrl);
                                dict["Message"] = "已提交购买";
                                dict["Success"] = "0";
                                dict["Url"] = string.Format("whotelapp://orderPay?orderid={0}&finishurl={1}&paytype={2}", order.OrderID, completeUrl, WHotelSite.Common.Config.DefaultPayChannelList);
                            }
                            else
                            {
                                dict["Message"] = "已提交购买";
                                dict["Success"] = "0";
                                dict["Url"] = Url.Action("Pay", "Order", new { order = order.OrderID, payChannels = WHotelSite.Common.Config.DefaultPayChannelList  });
                            }
                            break;
                        }
                    case 6:
                        {
                            dict["Message"] = (order != null ? order.Message : "您已购买成功");
                            dict["Success"] = order.Success.ToString();
                            dict["Url"] = "";
                            break;
                        }
                    default:
                        {
                            dict["Message"] = (order != null ? order.Message : "购买失败");
                            dict["Success"] = order.Success.ToString();
                            dict["Url"] = "";
                            break;
                        }
                }
            }
            else
            {
                dict["Message"] = (order != null ? order.Message : "购买失败");
                dict["Success"] = "1";
                dict["Url"] = "";
            }

            #region 行为记录

            var value = string.Format("{{\"userid\":\"{0}\",\"Message\":\"{1}\",\"CID\":\"{2}\",\"RequestCID\":\"{3}\"}}", userid, dict["Message"], GetCurCID(), GetCurCIDForRequest());
            RecordBehavior("SubmitVipConpon", value);

            #endregion

            return Json(dict, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 提交VIP会员预约信息
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public ActionResult ReserveVipConpon(string userid = "0")
        {
            var aid = 83;

            Dictionary<string, string> dict = new Dictionary<string, string>();

            //获取用户信息（主要是手机号）
            var minfo = account.GetCurrentUserInfo(Convert.ToInt64(userid)) ?? new MemberProfileInfo() { UserID = Convert.ToInt64(userid), MobileAccount = "0" };

            //提交预约信息
            var addResult = new Weixin().AddActiveLuckyDraw(aid, minfo.MobileAccount, minfo.MobileAccount, minfo.UserID.ToString(), "199VIP会员预约");
            if (addResult.ToLower().Trim().Contains("sign"))
            {
                dict["Message"] = "恭喜预约成功，我们将在一周之内联系您";
                dict["Success"] = "0";
                dict["Url"] = "";
            }
            else
            {
                dict["Message"] = "您已经预约成功";
                dict["Success"] = "-1";
                dict["Url"] = "";
            }

            return Json(dict, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// VIP支付完成页面
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public ActionResult PayCompleteForVip(string channel, string order = "0")
        {
            var isApp = IsApp();
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
            ViewBag.IsApp = isApp;
            ViewBag.Channel = channel;

            //是否登录
            var isLogin = UserState.IsLogin;
            ViewBag.IsLogin = isLogin;

            string[] keywords = new string[] { "Android", "iPhone", "iPod", "iPad", "Windows Phone", "MicroMessenger" };

            bool isShowHeader = true;
            foreach (var item in keywords)
            {
                if (Request.UserAgent.IndexOf(item) > -1)
                {
                    isShowHeader = false;
                    break;
                }
            }
            bool isInWeixin = Request.UserAgent.IndexOf("MicroMessenger") > 0 ? true : false;
            ViewBag.isShowHeader = isShowHeader;//如果是App里或微信里打开页面  则不显示
            ViewBag.isInWeixin = isInWeixin;

            RoomCouponOrderEntity couponOrder = new Coupon().GenCouponPayCompleteResult(Convert.ToInt32(order));
            ViewBag.CouponOrder = couponOrder;

            var userid = couponOrder != null && couponOrder.ExchangeCouponList != null && couponOrder.ExchangeCouponList.Count > 0 ?
                couponOrder.ExchangeCouponList[0].UserID : 0;
            ViewBag.MagiCallUrl = GetMagicallUrl(Convert.ToInt64(userid));

            ViewBag.UserId = userid;

            #region 行为记录

            var value = string.Format("{{\"order\":\"{0}\",\"channel\":\"{1}\",\"userid\":\"{2}\"}}", order, channel, userid);
            RecordBehavior("PayCompleteForVip", value);

            #endregion

            return View();
        }

        #endregion

        #region 天天果园398购买

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userid"></param>
        /// <param name="sid"></param>
        /// <returns></returns>
        public ActionResult CouponShopForFruitDay(int id, string userid = "0", string sid = "")
        {
            var isApp = IsApp();
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
            ViewBag.IsApp = isApp;
            ViewBag.UserId = userid;

            //暂时避免用户通过旧的链接进来购买“老199会员”，这里将100399转换为100929 haoy 2017.02.23
            if (id == 100399) id = 100929;
            ViewBag.Aid = id;

            //获取当前用户信息
            var userPhone = "";
            if (!string.IsNullOrEmpty(userid) && userid != "0")
            {
                try
                {
                    var userEntity = account.GetCurrentUserInfo(Convert.ToInt64(userid));
                    if (userEntity != null)
                    {
                        userPhone = userEntity.MobileAccount;
                    }
                }
                catch (Exception ex)
                {

                }
            }
            ViewBag.UserPhone = userPhone;

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

            //获取当前用户信息
            var userInfo = account.GetUserInfoByUserID(string.IsNullOrEmpty(userid) ? 0 : Convert.ToInt64(userid));
            ViewBag.UserInfo = userInfo ?? new UserInfoResult();

            //产品信息
            var coupondetail = Coupon.GetGroupCouponActivityDetail(id);

            ViewBag.CouponDetail = coupondetail;

            //查询当前券和用户的可售状态
            var buyResult = new Coupon().IsExchangeCouponSoldOut(id, Convert.ToInt64(userid));
            ViewBag.BuyResult = buyResult;

            #region 分享配置

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
                scp.UserID = Convert.ToInt64(userid);
                scp.CouponActivityID = id;
                scp.BizType = ZMJDShareTrackBizType.roomgroupcoupondetail;
                var shareCode = Comment.GenTrackCodeResult4Share(scp);
            }

            ViewBag.ShareUserInfo = shareUserInfo;

            #endregion

            return View();
        }

        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="aid"></param>
        /// <param name="atype"></param>
        /// <param name="pid"></param>
        /// <param name="pricetype"></param>
        /// <param name="paynum"></param>
        /// <param name="userid"></param>
        /// <param name="realname"></param>
        /// <returns></returns>
        public ActionResult SubmitFruitDayConpon(int aid, int atype, int pid, int pricetype, int paynum, string userid = "0", string realname = "")
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();

            SubmitCouponOrderModel sub = new SubmitCouponOrderModel();
            sub.ActivityID = aid;
            sub.ActivityType = atype;
            sub.UserID = Convert.ToInt64(userid);
            sub.CID = GetCurCID();
            sub.AddInfo = new ExchangeCouponAddInfoItem();
            sub.OrderItems = new List<ProductAndNum> 
            { 
                new ProductAndNum
                {
                    SourceID = pid,
                    Number = paynum,
                    Type = pricetype
                }
            };
            sub.RealName = realname;

            //是否大于等于5.4版本
            var isThanVer5_4 = IsThanVer5_4();
            ViewBag.IsThanVer5_4 = isThanVer5_4;

            CouponOrderResult order = Coupon.SubmitMemberOrder(sub);
            if (order != null)
            {
                switch (order.Success)
                {
                    case 0:
                    case 5:
                        {
                            if (IsLatestVerApp())
                            {
                                var completeUrl = "";
                                if (isThanVer5_4)
                                {
                                    completeUrl = Utils.GetAbsoluteUrl(Url, Url.Action("PayCompleteForFruitDay", "Coupon", new { channel = "all", order = order.OrderID.ToString(), _isoneoff = 1, _clearoneoff = 1 }));
                                }
                                else
                                {
                                    completeUrl = Utils.GetAbsoluteUrl(Url, Url.Action("PayCompleteForFruitDay", "Coupon", new { channel = "all", order = order.OrderID.ToString() }));
                                }
                                completeUrl = Server.UrlEncode(completeUrl);
                                dict["Message"] = "已提交购买";
                                dict["Success"] = "0";
                                dict["Url"] = string.Format("whotelapp://orderPay?orderid={0}&finishurl={1}&paytype={2}", order.OrderID, completeUrl, "all");
                            }
                            else
                            {
                                dict["Message"] = "已提交购买";
                                dict["Success"] = "0";
                                dict["Url"] = Url.Action("Pay", "Order", new { order = order.OrderID, payChannels = WHotelSite.Common.Config.DefaultPayChannelList  });
                            }
                            break;
                        }
                    case 6:
                        {
                            dict["Message"] = (order != null ? order.Message : "您已购买成功");
                            dict["Success"] = order.Success.ToString();
                            dict["Url"] = "";
                            break;
                        }
                    default:
                        {
                            dict["Message"] = (order != null ? order.Message : "购买失败");
                            dict["Success"] = order.Success.ToString();
                            dict["Url"] = "";
                            break;
                        }
                }
            }
            else
            {
                dict["Message"] = (order != null ? order.Message : "购买失败");
                dict["Success"] = "1";
                dict["Url"] = "";
            }

            return Json(dict, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 购买成功
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public ActionResult PayCompleteForFruitDay(string channel, string order = "0")
        {
            var isApp = IsApp();
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
            ViewBag.IsApp = isApp;
            ViewBag.Channel = channel;

            string[] keywords = new string[] { "Android", "iPhone", "iPod", "iPad", "Windows Phone", "MicroMessenger" };

            bool isShowHeader = true;
            foreach (var item in keywords)
            {
                if (Request.UserAgent.IndexOf(item) > -1)
                {
                    isShowHeader = false;
                    break;
                }
            }
            bool isInWeixin = Request.UserAgent.IndexOf("MicroMessenger") > 0 ? true : false;
            ViewBag.isShowHeader = isShowHeader;//如果是App里或微信里打开页面  则不显示
            ViewBag.isInWeixin = isInWeixin;

            RoomCouponOrderEntity couponOrder = new Coupon().GenCouponPayCompleteResult(Convert.ToInt32(order));
            ViewBag.CouponOrder = couponOrder;

            var userid = couponOrder != null && couponOrder.ExchangeCouponList != null && couponOrder.ExchangeCouponList.Count > 0 ?
                couponOrder.ExchangeCouponList[0].UserID : 0;
            ViewBag.MagiCallUrl = GetMagicallUrl(Convert.ToInt64(userid));

            ViewBag.UserId = userid;

            return View();
        }

        #endregion

        #region 平日券兑换列表

        /// <summary>
        /// 可兑换套餐专辑列表
        /// </summary>
        /// <param name="cid"></param>
        /// <param name="userid"></param>
        /// <param name="exchangeid"></param>
        /// <returns></returns>
        public ActionResult ExchangePackages(int cid = 0, int userid = 0, int exchangeid = 0)
        {
            var isApp = IsApp();
            ViewBag.IsApp = isApp;
            ViewBag.Cid = cid;
            ViewBag.UserId = userid;
            ViewBag.ExchangeId = exchangeid;

            //当前系统环境（ios | android）
            var appType = AppType();
            ViewBag.AppType = appType;

            //Weixin
            bool isInWeixin = Request.UserAgent.IndexOf("MicroMessenger") > 0 ? true : false;
            ViewBag.isInWeixin = isInWeixin;
            bool isNeedNotSale = true;//标记为不可售的套餐也列出来
            PackageAlbumDetail albumDetail = Hotel.GetPackageAlbumDetail(cid, isNeedNotSale);

            //if (albumDetail != null && albumDetail.packageList != null && albumDetail.packageList.Count > 0)
            //{
            //    albumDetail.packageList = albumDetail.packageList.OrderBy(p => p.);
            //}
            //无锡,苏州,三亚,桂林,厦门,莫干山,安吉,杭州,常州,都江堰,峨眉山,成都,长白山

            //分享信息设置
            albumDetail.shareModel.shareLink = string.Format("http://www.zmjiudian.com/package/collection/{0}?userid={1}", cid, userid);
            albumDetail.shareModel.photoUrl = albumDetail.shareModel.photoUrl.Replace("_jupiter", "_290x290s").Replace("_theme", "_290x290s").Replace("_appdetail1s", "_290x290s");

            ViewBag.AlbumDetail = albumDetail;

            return View();
        }

        /// <summary>
        /// 单个套餐详情页
        /// </summary>
        /// <param name="pid"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public ActionResult ExchangePackage(int pid = 0, int userid = 0, int albumid = 0, int exchangeid = 0)
        {
            return ExchangePackageContent(pid, userid, albumid, exchangeid);
        }

        /// <summary>
        /// 套餐着陆页内容部分
        /// </summary>
        /// <param name="pid"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public ActionResult ExchangePackageContent(int pid = 0, int userid = 0, int albumid = 0, int exchangeid = 0)
        {
            var isApp = IsApp();
            ViewBag.IsApp = isApp;
            ViewBag.Pid = pid;
            ViewBag.UserId = userid;
            ViewBag.AlbumId = albumid;
            ViewBag.ExchangeId = exchangeid;

            //调试使用
            ViewBag.AppType = AppType();
            if (AppType() == "iosapp") ViewBag.AppVerForIOS = AppVerForIOS();
            else ViewBag.AppVerForIOS = "";
            if (AppType() == "android") ViewBag.AppVerForAndroid = AppVerForAndroid();
            else ViewBag.AppVerForAndroid = "";
            ViewBag.IsLatestVerApp = IsLatestVerApp();

            //检测是否大于等于4.6版本（4.6之后app支持h5跳转至原生预订页面）
            var isThanVer46 = IsThanVer4_6();
            ViewBag.IsThanVer46 = isThanVer46;

            //是否大于等于4.7
            var isThanVer47 = IsThanVer4_7();
            ViewBag.IsThanVer47 = isThanVer47;

            //获取当前套餐信息
            var startTime = DateTime.Now.Date;
            if (albumid == 13)
            {
                startTime = DateTime.Parse("2016-10-01");
            }
            RecommendPackageDetailResult packageEntity = Hotel.GetPackageDetailResult(pid, userid, startTime.ToString("yyyy-MM-dd"), "");
            ViewBag.PackageEntity = packageEntity;

            //当前套餐房型
            SameSerialPackageItem thisPackageRoomInfo = new SameSerialPackageItem();
            if (packageEntity.serialPackageList != null && packageEntity.serialPackageList.Count > 0)
            {
                thisPackageRoomInfo = packageEntity.serialPackageList.Find(p => p.pId == pid);
            }
            ViewBag.ThisPackageRoomInfo = thisPackageRoomInfo;

            //默认日期
            var addNightCount = 1;
            var checkIn = DateTime.Now.Date;
            var checkOut = checkIn.AddDays(addNightCount);
            var defPrice = packageEntity.packageItem.HotelPrice;

            //当前套餐的可用日期
            var calendar = HJDAPI.APIProxy.Price.GetOneHotelPackageCalendar(packageEntity.packageItem.HotelID, DateTime.Now.Date, packageEntity.packageItem.PID, Convert.ToInt64(userid));
            if (calendar != null && calendar.DayItems != null && calendar.DayItems.Count > 0 && calendar.DayItems.Exists(d => d.SellState == 1))
            {
                var firstDayObj = calendar.DayItems.First(d => d.SellState == 1);

                #region 对专辑13特殊处理

                if (albumid == 13)
                {
                    if (firstDayObj.Day < DateTime.Parse("2016-10-01"))
                    {
                        if (calendar.DayItems.Exists(d => d.SellState == 1 && d.Day == DateTime.Parse("2016-10-01")) &&
                            (calendar.DayLimitMin > 2 ? calendar.DayItems.Exists(d => d.SellState == 1 && d.Day == DateTime.Parse("2016-10-02")) && calendar.DayItems.Exists(d => d.SellState == 1 && d.Day == DateTime.Parse("2016-10-03")) : true))
                        {
                            firstDayObj = calendar.DayItems.First(d => d.SellState == 1 && d.Day == DateTime.Parse("2016-10-01"));
                        }
                        else if (calendar.DayItems.Exists(d => d.SellState == 1 && d.Day == DateTime.Parse("2016-10-02")) &&
                        (calendar.DayLimitMin > 2 ? calendar.DayItems.Exists(d => d.SellState == 1 && d.Day == DateTime.Parse("2016-10-03")) && calendar.DayItems.Exists(d => d.SellState == 1 && d.Day == DateTime.Parse("2016-10-04")) : true))
                        {
                            firstDayObj = calendar.DayItems.First(d => d.SellState == 1 && d.Day == DateTime.Parse("2016-10-02"));
                        }
                        else if (calendar.DayItems.Exists(d => d.SellState == 1 && d.Day == DateTime.Parse("2016-10-03")) &&
                        (calendar.DayLimitMin > 2 ? calendar.DayItems.Exists(d => d.SellState == 1 && d.Day == DateTime.Parse("2016-10-04")) && calendar.DayItems.Exists(d => d.SellState == 1 && d.Day == DateTime.Parse("2016-10-05")) : true))
                        {
                            firstDayObj = calendar.DayItems.First(d => d.SellState == 1 && d.Day == DateTime.Parse("2016-10-03"));
                        }
                        else if (calendar.DayItems.Exists(d => d.SellState == 1 && d.Day == DateTime.Parse("2016-10-04")) &&
                        (calendar.DayLimitMin > 2 ? calendar.DayItems.Exists(d => d.SellState == 1 && d.Day == DateTime.Parse("2016-10-05")) && calendar.DayItems.Exists(d => d.SellState == 1 && d.Day == DateTime.Parse("2016-10-06")) : true))
                        {
                            firstDayObj = calendar.DayItems.First(d => d.SellState == 1 && d.Day == DateTime.Parse("2016-10-04"));
                        }
                        else if (calendar.DayItems.Exists(d => d.SellState == 1 && d.Day == DateTime.Parse("2016-10-01")) &&
                        (calendar.DayLimitMin > 1 ? calendar.DayItems.Exists(d => d.SellState == 1 && d.Day == DateTime.Parse("2016-10-02")) : true))
                        {
                            firstDayObj = calendar.DayItems.First(d => d.SellState == 1 && d.Day == DateTime.Parse("2016-10-01"));
                        }
                        else if (calendar.DayItems.Exists(d => d.SellState == 1 && d.Day == DateTime.Parse("2016-10-02")) &&
                            (calendar.DayLimitMin > 1 ? calendar.DayItems.Exists(d => d.SellState == 1 && d.Day == DateTime.Parse("2016-10-03")) : true))
                        {
                            firstDayObj = calendar.DayItems.First(d => d.SellState == 1 && d.Day == DateTime.Parse("2016-10-02"));
                        }
                        else if (calendar.DayItems.Exists(d => d.SellState == 1 && d.Day == DateTime.Parse("2016-10-03")) &&
                            (calendar.DayLimitMin > 1 ? calendar.DayItems.Exists(d => d.SellState == 1 && d.Day == DateTime.Parse("2016-10-04")) : true))
                        {
                            firstDayObj = calendar.DayItems.First(d => d.SellState == 1 && d.Day == DateTime.Parse("2016-10-03"));
                        }
                        else if (calendar.DayItems.Exists(d => d.SellState == 1 && d.Day == DateTime.Parse("2016-10-04")) &&
                            (calendar.DayLimitMin > 1 ? calendar.DayItems.Exists(d => d.SellState == 1 && d.Day == DateTime.Parse("2016-10-05")) : true))
                        {
                            firstDayObj = calendar.DayItems.First(d => d.SellState == 1 && d.Day == DateTime.Parse("2016-10-04"));
                        }
                    }
                }

                #endregion

                checkIn = firstDayObj.Day;

                if (calendar.DayLimitMin > 0)
                {
                    addNightCount = calendar.DayLimitMin;
                }
                checkOut = checkIn.AddDays(addNightCount);
                defPrice = firstDayObj.SellPrice;
            }

            var totalPrice = packageEntity.packageItem.NotVIPPrice * (calendar.DayLimitMin < 1 ? 1 : calendar.DayLimitMin);
            var totalVipPrice = packageEntity.packageItem.VIPPrice * (calendar.DayLimitMin < 1 ? 1 : calendar.DayLimitMin);

            //如果是多天的套餐，则累加计算出多天总价
            if (calendar.DayLimitMin > 1)
            {
                totalPrice = 0;
                totalVipPrice = 0;

                var _startDate = checkIn;
                for (int i = 0; i < calendar.DayLimitMin; i++)
                {
                    var _calendarItem = calendar.DayItems.Find(_ => _.Day == _startDate.AddDays(i));
                    totalPrice += _calendarItem.NormalPrice;
                    totalVipPrice += _calendarItem.VipPrice;
                }
            }

            ViewBag.TotalPrice = totalPrice;
            ViewBag.TotalVipPrice = totalVipPrice;

            var subPriceTip = string.Format("已选{0}月{1}号入住{2}月{3}日离店共{4}天", checkIn.Month, checkIn.Day, checkOut.Month, checkOut.Day, addNightCount);
            ViewBag.SubPriceTip = subPriceTip;
            ViewBag.DefPrice = defPrice;
            ViewBag.CheckIn = checkIn;
            ViewBag.CheckOut = checkOut;
            ViewBag.Calendar = calendar.DayItems;
            ViewBag.DayLimitMin = calendar.DayLimitMin;
            ViewBag.DayLimitMax = calendar.DayLimitMax;
            ViewBag.NightCount = addNightCount;

            return View();
        }


        #endregion

        #region 消费券购买相关

        /// <summary>
        /// 消费券购买页
        /// </summary>
        /// <param name="skuid"></param>
        /// <param name="userid"></param>
        /// <param name="sid"></param>
        /// <param name="fromwxno">是否来自微信公众号，只有微信公众号里放的链接才会传1</param>
        /// <param name="fromwxuid">来自哪一个微信号的分享推荐（全员分销使用）</param>
        /// <param name="distributioncid"></param>
        /// <param name="_sourcekey"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public ActionResult CouponShopForProduct(int skuid, string userid = "0", string sid = "", int fromwxno = 0, int fromwxuid = 0, long distributioncid = 0, string _sourcekey = "", string code = "")
        {
            var curUserID = UserState.UserID > 0 ? UserState.UserID : Convert.ToInt64( userid);
            if (curUserID <= 0)
            {
                curUserID = _ContextBasicInfo.AppUserID;
            }
            ViewBag.UserId = curUserID;
            ViewBag.SourceKey = _sourcekey;

            return CouponShopForProductContent(skuid, curUserID.ToString(), sid, fromwxno, fromwxuid, distributioncid, _sourcekey, code);
        }

        /// <summary>
        /// 消费券购买页Content
        /// </summary>
        /// <param name="skuid"></param>
        /// <param name="userid"></param>
        /// <param name="sid"></param>
        /// <param name="fromwxno">是否来自微信公众号，只有微信公众号里放的链接才会传1</param>
        /// <param name="fromwxuid">来自哪一个微信号的分享推荐（全员分销使用）</param>
        /// <param name="distributioncid"></param>
        /// <param name="_sourcekey"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public ActionResult CouponShopForProductContent(int skuid, string userid = "0", string sid = "", int fromwxno = 0, int fromwxuid = 0, long distributioncid = 0, string _sourcekey = "", string code = "")
        {
            var isApp = IsApp();
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
            ViewBag.IsApp = isApp;
            ViewBag.SKUID = skuid;

            var curUserID = UserState.UserID > 0 ? UserState.UserID : (isApp ? Convert.ToInt64(userid) : 0);
            if (curUserID <= 0)
            {
                curUserID = _ContextBasicInfo.AppUserID;
            }
            if (curUserID >= 0 && _ContextBasicInfo.AppUserID <= 0)
            {
                _ContextBasicInfo.AppUserID = curUserID;   
            }
            ViewBag.UserId = curUserID;

            //获取请求参数中的CID
            var requestCID = GetCurCIDForRequest();
            ViewBag.RequestCID = requestCID;

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

            //当前连接来自哪一个微信用户分销(全员分销时使用)
            ViewBag.FromWxUid = fromwxuid;

            #region 微信环境下，做微信静默授权

            WeixinUserInfo weixinUserInfo = new WeixinUserInfo();
            var openid = "";
            var unionid = "";
            var wxuid = 0;
            if (isInWeixin)
            {
                if (!string.IsNullOrEmpty(code))
                {
                    var weixinAcountName = "weixinservice_haoyi";

                    //通过code换取网页授权access_token
                    var accessToken = WeiXinHelper.GetWeixinAccessTokenForHaoYi(code);
                    if (accessToken != null && !string.IsNullOrEmpty(accessToken.Openid))
                    {
                        openid = accessToken.Openid;

                        //获取用户信息
                        //通过access_token拉取用户信息(需scope为 snsapi_userinfo)（这是通过微信api获取的真实用户信息）
                        weixinUserInfo = WeiXinHelper.GetWeixinUserInfo(accessToken.AccessToken, accessToken.Openid);
                        if (weixinUserInfo != null && !string.IsNullOrEmpty(weixinUserInfo.Openid))
                        {
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
                                    WeixinAcount = weixinAcountName,   //WeiXinChannelCode.周末酒店服务号_皓颐
                                    Language = "zh_CN",
                                    SubscribeTime = DateTime.Now,
                                    CreateTime = DateTime.Now
                                };
                                var update = new Weixin().UpdateWeixinUserSubscribe(w);

                                //get weixin user
                                var weixinUser = new Weixin().GetWeixinUser(openid);
                                if (weixinUser != null && weixinUser.ID > 0)
                                {
                                    wxuid = weixinUser.ID;
                                }
                            }
                            catch (Exception ex)
                            {

                            }

                            #region 处理微信号绑定用户信息相关操作

                            //微信环境下，如果当前已经登录，则直接绑定
                            if (curUserID > 0)
                            {
                                WeiXinHelper.BindingWxAndUid(curUserID, weixinUserInfo.Unionid);
                            }

                            #endregion

                            #region 更新Unionid缓存并更新CID信息

                            SetCurWXUnionid(weixinUserInfo.Unionid);
                            CheckCID();

                            #endregion
                        }
                    }
                    else
                    {
                        //如果code有值，但不能获取，一般说明code过期了，那么重新获取
                        var weixinGoUrl = "";
                        if (requestCID > 0)
                        {
                            weixinGoUrl = WeiXinHelper.GenSilenceAuthorUrlForHaoYi(HttpUtility.UrlEncode(string.Format("http://www.zmjiudian.com/coupon/product/{0}?sid={2}&fromwxno={3}&fromwxuid={4}&CID={5}&distributioncid={6}&_sourcekey={7}", skuid, userid, sid, fromwxno, fromwxuid, requestCID, distributioncid, _sourcekey)));
                        }
                        else
                        {
                            weixinGoUrl = WeiXinHelper.GenSilenceAuthorUrlForHaoYi(HttpUtility.UrlEncode(string.Format("http://www.zmjiudian.com/coupon/product/{0}?sid={2}&fromwxno={3}&fromwxuid={4}&distributioncid={5}&_sourcekey={6}", skuid, userid, sid, fromwxno, fromwxuid, distributioncid, _sourcekey)));
                        }
                        return Redirect(weixinGoUrl);
                    }
                }
                else
                {
                    //授权页面
                    var weixinGoUrl = "";
                    if (requestCID > 0)
                    {
                        weixinGoUrl = WeiXinHelper.GenSilenceAuthorUrlForHaoYi(HttpUtility.UrlEncode(string.Format("http://www.zmjiudian.com/coupon/product/{0}?sid={2}&fromwxno={3}&fromwxuid={4}&CID={5}&distributioncid={6}&_sourcekey={7}", skuid, userid, sid, fromwxno, fromwxuid, requestCID, distributioncid, _sourcekey)));
                    }
                    else
                    {
                        weixinGoUrl = WeiXinHelper.GenSilenceAuthorUrlForHaoYi(HttpUtility.UrlEncode(string.Format("http://www.zmjiudian.com/coupon/product/{0}?sid={2}&fromwxno={3}&fromwxuid={4}&distributioncid={5}&_sourcekey={6}", skuid, userid, sid, fromwxno, fromwxuid, distributioncid, _sourcekey)));
                    }
                    return Redirect(weixinGoUrl);
                }
            }
            ViewBag.Openid = openid;
            ViewBag.Unionid = weixinUserInfo.Unionid;
            ViewBag.WeixinUid = wxuid;

            #endregion

            //如果fromwxuid与当前wxuid一致，则清楚fromwxuid（自己分享给自己不算）
            if (wxuid == fromwxuid)
            {
                fromwxuid = 0;
            }

            //获取产品店铺信息
            var productShopInfo = new RetailerShopEntity();
            if (requestCID > 0)
            {
                //查询指定CID的店铺信息
                productShopInfo = Shop.GetRetailerShopByCID(requestCID);   
            }
            ViewBag.ProductShopInfo = productShopInfo;

            //查询当前用户的度假伙伴状态
            var partnerResult = Partner.GetRetailerInvate(curUserID);
            ViewBag.PartnerResult = partnerResult;

            //分销查看（如果distributioncid>0，说明是从分销后台跳转过来打开的，那么优先显示该cid的分销状态信息；如果没有传，但是当前用户是分销，则显示当前用户的分销状态信息）
            var pcid = distributioncid;
            if (pcid <= 0 && curUserID > 0 && (RetailerInvateState)partnerResult.State == RetailerInvateState.Pass)
            {
                pcid = curUserID;
            }
            ViewBag.PCID = pcid;

            //获取券SKU信息
            var couponSkuInfo = Coupon.GetSKUCouponActivityDetail(skuid);
            ViewBag.CouponSkuInfo = couponSkuInfo;

            #region 检查当前sku是不是手拉手的产品，是的话则重定向到手拉手产品

            if (couponSkuInfo.activity != null && couponSkuInfo.activity.GroupCount > 0 && couponSkuInfo.activity.GroupDay > 0)
            {
                var _groupRdUrl = string.Format("/coupon/group/product/{0}/0?cid={1}&userid={2}&distributioncid={3}", skuid, requestCID, curUserID, distributioncid);
                return Redirect(_groupRdUrl);
            }

            #endregion

            #region 检查当前sku是不是大团购产品，是的话则重定向到大团购产品

            if (couponSkuInfo.SKUInfo != null && couponSkuInfo.SKUInfo.SPU != null 
                && couponSkuInfo.SKUInfo.SPU.PropertyList != null && couponSkuInfo.SKUInfo.SPU.PropertyList.Exists(_ => _.Type == 6))
            {
                //获取当前大团购的定金SKU
                var _dingjinSKU = couponSkuInfo.SKUInfo.SKUList.Find(_ => _.IsDepositSKU);
                var _stepgroupRdUrl = string.Format("/coupon/stepgroup/product/{0}?cid={1}&userid={2}&distributioncid={3}", _dingjinSKU.ID, requestCID, curUserID, distributioncid);
                return Redirect(_stepgroupRdUrl);
            }

            #endregion

            //如果当前抢购VIP专享，则判断当前用户是否为VIP
            var isVip = account.IsVIPCustomer(new AccountInfoItem { Uid = curUserID.ToString() });
            ViewBag.IsVip = isVip;

            //isMobile
            var isMobile = Utils.IsMobile();
            ViewBag.IsMobile = isMobile;

            //获取当前用户信息
            var userInfo = account.GetUserInfoByUserID(curUserID);
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

            //标识是否来自微信公众号
            var _fromWxno = fromwxno == 1;
            ViewBag.FromWxno = _fromWxno;

            #region 检查当前抢购的活动规则关联

            //是否需要弹出去微信报名的提示
            var showSignWeixin = false;

            //couponSkuInfo.activity.RelationId = 401;
            if (couponSkuInfo.activity != null && couponSkuInfo.activity.RelationId > 0)
            {
                var relActiveId = couponSkuInfo.activity.RelationId;
                var weixinActiveEntity = ActiveController.GetWeixinActiveEntity(relActiveId);
                if (weixinActiveEntity != null)
                {
                    ViewBag.WeixinActiveEntity = weixinActiveEntity;

                    var userPhone = "";
                    try
                    {
                        var userEntity = account.GetCurrentUserInfo(Convert.ToInt64(userid));
                        if (userEntity != null) { userPhone = userEntity.MobileAccount; }
                    }
                    catch (Exception ex) { }

                    //根据微信关联活动和手机号，检查报名信息
                    var weixinDraw = new Weixin().GetActiveWeixinDrawByPhone(relActiveId, userPhone);

                    //没有报名或者没有分享
                    if (weixinDraw == null || weixinDraw.IsShare == 0)
                    {
                        //如果是从微信公众号过来的，直接跳转到预约页面
                        if (_fromWxno)
                        {
                            if (weixinActiveEntity.ActiveEndTime > DateTime.Now)
                            {
                                //合作伙伴ID
                                var _ptid = 7;
                                switch (weixinActiveEntity.WeixinAcountId)
                                {
                                    case 3: _ptid = 17; break;   //尚旅游
                                    case 4: _ptid = 18; break;   //尚旅游成都
                                    case 5: _ptid = 45; break;   //美味至尚
                                    case 6: _ptid = 47; break;   //尚旅游北京
                                }
                                return Redirect(string.Format("http://www.zmjiudian.com/wx/active/reg/{0}/{1}", _ptid, relActiveId));
                            }
                        }
                        //如果非微信公众号过来的，则弹出去预约的提示
                        else 
                        {
                            showSignWeixin = true;
                        }
                    }
                }
            }

            ViewBag.ShowSignWeixin = showSignWeixin;

            #endregion

            #region 分享配置

            var shareLink = "";
            //分享链接生成
            if (requestCID > 0)
            {
                shareLink = string.Format("http://www.zmjiudian.com/coupon/product/{0}?CID={1}&fromwxuid={2}", skuid, requestCID, wxuid);
            }
            else
            {
                //暂时只有在APP中登录分享才会加sid&cid等参数
                if (isApp)
                {
                    //分享跟踪参数
                    var scp = new GenTrackCodeParam();
                    scp.UserID = Convert.ToInt64(curUserID);
                    scp.CouponActivityID = skuid;
                    scp.BizType = ZMJDShareTrackBizType.roomcoupondetail;
                    var shareCode = Comment.GenTrackCodeResult4Share(scp);

                    shareLink = string.Format("http://www.zmjiudian.com/coupon/product/{0}?sid={1}", skuid, shareCode.EncodeStr);
                }
                else
                {
                    //分享跟踪参数
                    var scp = new GenTrackCodeParam();
                    scp.UserID = Convert.ToInt64(curUserID);
                    scp.CouponActivityID = skuid;
                    scp.BizType = ZMJDShareTrackBizType.roomcoupondetail;
                    var shareCode = Comment.GenTrackCodeResult4Share(scp);

                    shareLink = string.Format("http://www.zmjiudian.com/coupon/product/{0}?fromwxuid={1}", skuid, wxuid);
                }
            }

            //解析SID
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

            ViewBag.ShareLink = shareLink;
            ViewBag.ShareUserInfo = shareUserInfo;

            #endregion           

            #region 行为记录

            var value = string.Format("{{\"skuid\":\"{0}\",\"sid\":\"{1}\",\"userNo\":\"{2}\",\"title\":\"{3}\",\"CID\":\"{4}\",\"RequestCID\":\"{5}\",\"fromwxno\":\"{6}\"}}", skuid, sid, curUserID, (couponSkuInfo.activity != null && !string.IsNullOrEmpty(couponSkuInfo.activity.PageTitle) ? couponSkuInfo.activity.PageTitle : ""), GetCurCID(), GetCurCIDForRequest(), fromwxno);
            RecordBehavior("CouponProductLoad", value);

            #endregion

            return View();
        }

        /// <summary>
        /// 检测产品价格策略
        /// </summary>
        /// <param name="skuid"></param>
        /// <param name="buynum"></param>
        /// <param name="userid"></param>
        /// <param name="stype"></param>
        /// <returns></returns>
        public ActionResult CheckProductPromotionForCoupon(int skuid, int buynum, int userid = 0, int stype = 1)
        {
            Dictionary<string, string> re = new Dictionary<string, string>();

            var promotionResult = Coupon.CheckProductPromotionForCoupon(skuid, buynum, userid, stype);

            return Json(promotionResult, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 消费券购买检测
        /// </summary>
        /// <param name="id"></param>
        /// <param name="buynum"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public ActionResult CheckBuyNumberForProduct(int id, int buynum, string userid = "0")
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
        /// 消费券购买提交
        /// </summary>
        /// <param name="aid"></param>
        /// <param name="atype"></param>
        /// <param name="pid"></param>
        /// <param name="pricetype"></param>
        /// <param name="paynum"></param>
        /// <param name="userid"></param>
        /// <param name="skuid"></param>
        /// <param name="stype"></param>
        /// <param name="cashCouponIdx"></param>
        /// <param name="cashCouponType"></param>
        /// <param name="cashCouponAmount"></param>
        /// <param name="useFundAmount"></param>
        /// <param name="selectedVoucherIDs"></param>
        /// <param name="voucherAmount"></param>
        /// <param name="bookTempId"></param>
        /// <param name="bookTempDataString"></param>
        /// <param name="bookTempDescription"></param>
        /// <param name="travelPersons"></param>
        /// <param name="fromwxuid"></param>
        /// <param name="otherPhotoUrl">券订单照片上传信息</param>
        /// <param name="coid">大团购补尾款的定金关联CouponOrderId</param>
        /// <returns></returns>
        public ActionResult SubmitConponForProduct(int aid, int atype, int skuid, int paynum, string userid, int stype, int cashCouponIdx = 0, int cashCouponType = 0, decimal cashCouponAmount = 0, decimal useFundAmount = 0, string selectedVoucherIDs = "", decimal voucherAmount = 0, int bookTempId = 0, string bookTempDataString = "", string bookTempDescription = "", string travelPersons = "", int fromwxuid = 0, string otherPhotoUrl = "", long coid = 0)
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
            sub.CouponOrderID = coid;

            //现金券使用
            if (cashCouponIdx > 0 && cashCouponAmount > 0)
            {
                sub.UseCashCouponInfo = new HJD.CouponService.Contracts.Entity.UseCashCouponItem
                {
                    CashCouponID = cashCouponIdx,
                    CashCouponType = cashCouponType,
                    UseCashAmount = cashCouponAmount
                };
            }

            //代金券使用
            var selectedCouponIds = new List<int>();
            if (!string.IsNullOrEmpty(selectedVoucherIDs))
            {
                selectedCouponIds = selectedVoucherIDs.Split(',').Select(_ => Convert.ToInt32(_)).ToList();
            }

            if (selectedCouponIds.Count > 0 && voucherAmount > 0)
            {
                sub.UseVoucherInfo = new UseVoucherInfoEntity
                {
                    UseVoucherIDList = selectedCouponIds.ToList(),
                    UseVoucherAmount = voucherAmount
                };
            }

            //住基金使用
            if (useFundAmount > 0)
            {
                sub.UserUseHousingFundAmount = useFundAmount;
            }

            //提交的“其他信息”
            if (!string.IsNullOrEmpty(bookTempDataString))
            {
                try
                {
                    sub.TemplateData = new ProductService.Contracts.Entity.TemplateDataEntity 
                    {
                        TemplateID = bookTempId,
                        TemplateItem = bookTempDataString,
                        Description = bookTempDescription
                    };
                }
                catch (Exception ex)
                {
                    
                }
            }

            //出行人
            var _travelPersonIdList = new List<int>();
            if (!string.IsNullOrEmpty(travelPersons))
            {
                try
                {
                    _travelPersonIdList = travelPersons.Split(',').ToList().Select(_id => Convert.ToInt32(_id)).ToList();
                }
                catch (Exception ex)
                {
                    //return Content("出行人读取错误，请重试");
                }
            }
            sub.TravelId = _travelPersonIdList;

            //from wx uid
            sub.FromWeixinUid = fromwxuid;

            //照片信息
            if (!string.IsNullOrEmpty(otherPhotoUrl))
            {
                //otherPhotoUrl
                sub.PhotoUrl = otherPhotoUrl;
            }

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
                    dict["Message"] = "该商品仅限VIP会员购买";
                    dict["Success"] = "2";
                    dict["Exids"] = "";
                    dict["Url"] = "";

                    #region 行为记录

                    //var value = string.Format("{{\"skuid\":\"{0}\",\"aid\":\"{1}\",\"atype\":\"{2}\",\"paynum\":\"{3}\",\"userid\":\"{4}\",\"stype\":\"{5}\",\"return\":\"该商品仅限VIP会员购买\"}}", skuid, aid, atype, paynum, userid, stype);
                    //RecordBehavior("CouponProductSubmit", value);

                    #endregion

                    return Json(dict, JsonRequestBehavior.AllowGet);
                }
            }

            //获取券SKU信息
            var couponSkuInfo = Coupon.GetSKUCouponActivityDetail(skuid);

            //是否新VIP才能购买的产品
            if (couponSkuInfo.SKUInfo.SKU.TagsList != null)
            {
                //获取用户是否VIP
                var isVip = account.IsVIPCustomer(new AccountInfoItem { Uid = userid });

                #region 是否首单 or 是否新VIP专享（不扣首单权限）

                if (couponSkuInfo.SKUInfo.SKU.TagsList.Exists(_ => _.TagID == 1 || _.TagID == 5))
                {
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
                                if (couponSkuInfo.SKUInfo.SKU.TagsList.Exists(_ => _.TagID == 1))
                                {
                                    dict["Message"] = "抱歉，此专享优惠套餐每位VIP会员限购一套哦，您之前已经购买过啦～";
                                }
                                else
                                {
                                    dict["Message"] = "抱歉，该产品仅供新VIP会员专享哦～";
                                }
                                dict["Success"] = "1";
                                dict["Exids"] = "";
                                dict["Url"] = "";

                                #region 行为记录

                                //var value = string.Format("{{\"skuid\":\"{0}\",\"aid\":\"{1}\",\"atype\":\"{2}\",\"paynum\":\"{3}\",\"userid\":\"{4}\",\"stype\":\"{5}\",\"return\":\"抱歉，此专享优惠套餐每位VIP会员限购一套哦\"}}", skuid, aid, atype, paynum, userid, stype);
                                //RecordBehavior("CouponProductSubmit", value);

                                #endregion

                                return Json(dict, JsonRequestBehavior.AllowGet);
                            }
                        }
                        else
                        {
                            if (couponSkuInfo.SKUInfo.SKU.TagsList.Exists(_ => _.TagID == 1))
                            {
                                dict["Message"] = "抱歉，此价格仅供新VIP会员专享哦～";
                            }
                            else
                            {
                                dict["Message"] = "抱歉，该产品仅供新VIP会员专享哦～";
                            }
                            dict["Success"] = "1";
                            dict["Exids"] = "";
                            dict["Url"] = "";

                            #region 行为记录

                            //var value = string.Format("{{\"skuid\":\"{0}\",\"aid\":\"{1}\",\"atype\":\"{2}\",\"paynum\":\"{3}\",\"userid\":\"{4}\",\"stype\":\"{5}\",\"return\":\"抱歉，此价格仅供新VIP会员专享哦\"}}", skuid, aid, atype, paynum, userid, stype);
                            //RecordBehavior("CouponProductSubmit", value);

                            #endregion

                            return Json(dict, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        if (couponSkuInfo.SKUInfo.SKU.TagsList.Exists(_ => _.TagID == 1))
                        {
                            dict["Message"] = "抱歉，此价格为新VIP会员专享，成为VIP会员才能购买哦～";
                        }
                        else
                        {
                            if (couponSkuInfo.SKUInfo.Category.PayType == 1)
                            {
                                dict["Message"] = "抱歉，该产品为新VIP会员专享，成为VIP会员才能兑换/领取哦～";
                            }
                            else
                            {
                                dict["Message"] = "抱歉，此价格为新VIP会员专享，成为VIP会员才能购买哦～";
                            }
                        }
                        dict["Success"] = "2";
                        dict["Exids"] = "";
                        dict["Url"] = "";

                        #region 行为记录

                        //var value = string.Format("{{\"skuid\":\"{0}\",\"aid\":\"{1}\",\"atype\":\"{2}\",\"paynum\":\"{3}\",\"userid\":\"{4}\",\"stype\":\"{5}\",\"return\":\"抱歉，此价格为新VIP会员专享，成为VIP会员才能购买哦\"}}", skuid, aid, atype, paynum, userid, stype);
                        //RecordBehavior("CouponProductSubmit", value);

                        #endregion

                        return Json(dict, JsonRequestBehavior.AllowGet);
                    }

                    //首单的新VIP专享的套餐用户只能买一件
                    if (couponSkuInfo.SKUInfo.SKU.TagsList.Exists(_ => _.TagID == 1))
                    {
                        if (paynum > 1)
                        {
                            dict["Message"] = "抱歉，此专享优惠套餐每位VIP会员限购一套哦～";
                            dict["Success"] = "1";
                            dict["Exids"] = "";
                            dict["Url"] = "";

                            #region 行为记录

                            //var value = string.Format("{{\"skuid\":\"{0}\",\"aid\":\"{1}\",\"atype\":\"{2}\",\"paynum\":\"{3}\",\"userid\":\"{4}\",\"stype\":\"{5}\",\"return\":\"抱歉，此专享优惠套餐每位VIP会员限购一套哦\"}}", skuid, aid, atype, paynum, userid, stype);
                            //RecordBehavior("CouponProductSubmit", value);

                            #endregion

                            return Json(dict, JsonRequestBehavior.AllowGet);
                        }
                    }
                }

                #endregion

                #region 是否只限普通会员专享

                else if (couponSkuInfo.SKUInfo.SKU.TagsList.Exists(_ => _.TagID == 4))
                {
                    if (isVip)
                    {
                        dict["Message"] = "抱歉，此套餐仅限普通用户购买";
                        dict["Success"] = "1";
                        dict["Exids"] = "";
                        dict["Url"] = "";

                        #region 行为记录

                        //var value = string.Format("{{\"skuid\":\"{0}\",\"aid\":\"{1}\",\"atype\":\"{2}\",\"paynum\":\"{3}\",\"userid\":\"{4}\",\"stype\":\"{5}\",\"return\":\"抱歉，此套餐仅限普通用户购买\"}}", skuid, aid, atype, paynum, userid, stype);
                        //RecordBehavior("CouponProductSubmit", value);

                        #endregion

                        return Json(dict, JsonRequestBehavior.AllowGet);
                    }
                }

                #endregion
            }

            CouponOrderResult order = new Coupon().SubmitExchangeOrderForProduct(sub);
            if (order != null && order.Success == 0)
            {
                //铂涛的跳转页面 发送短信验证码
                var botaoCode = HJDAPI.Common.Helpers.Enums.ThirdPartyMerchantCode.bohuijinrong.ToString();
                var botaophone = ClientHelper.GetBotaoPhoneNumFromCookie(HttpContext.Request.Cookies);

                //如果是积分产品/免费领取的产品,则跳转到h5的支付方式页面
                if (!_ContextBasicInfo.IsThanVer5_7 &&
                    ((couponSkuInfo != null && couponSkuInfo.SKUInfo != null && couponSkuInfo.SKUInfo.SKU.Price == 0 && couponSkuInfo.SKUInfo.SKU.VIPPrice == 0 && couponSkuInfo.SKUInfo.SKU.Points == 0) ||
                    (couponSkuInfo != null && couponSkuInfo.SKUInfo != null && couponSkuInfo.SKUInfo.Category != null && couponSkuInfo.SKUInfo.Category.PayType == 1)))
                {
                    dict["Message"] = "订单已提交";
                    dict["Success"] = "0";
                    dict["Exids"] = string.Join(",", order.ExchangeIdList);
                    dict["Url"] = Url.Action("Pay", "Order", new { order = order.OrderID, payChannels = WHotelSite.Common.Config.DefaultPayChannelList, _newpage = 1 });
                }
                else if (!string.IsNullOrWhiteSpace(botaophone))
                {
                    var phoneNum = botaophone;
                    var amount = (int)(order.Amount * 100);// order.Amount * 100;//元转分
                    var goodsInfo = order.GoodsInfo;
                    var orderId = order.OrderID;

                    var payWebSiteUrl = (string)System.Configuration.ConfigurationManager.AppSettings["PayWebSiteUrl"];
                    var completeUrl = string.Format("{0}Pay/BoTaoPayComplete/{1}/{2}", payWebSiteUrl, "coupon", orderId);

                    var sign = EncryptMethod.GenSignature4Pay(botaoCode, phoneNum, amount, order.OrderID, completeUrl);
                    dict["Message"] = "订单已提交";
                    dict["Success"] = "0";
                    dict["Exids"] = string.Join(",", order.ExchangeIdList);
                    var jumpUrl = string.Format("{7}pay/botao?mobileId={0}&merName={1}&goodsInf={2}&amount={3}&orderId={4}&retUrl={5}&sign={6}", phoneNum, botaoCode, goodsInfo, amount, order.OrderID, completeUrl, sign, payWebSiteUrl);
                    dict["Url"] = jumpUrl;
                }
                else if (IsLatestVerApp())
                {
                    var _timeStamp = Signature.GenTimeStamp();

                    var completeUrl = "";
                    if (isThanVer5_4)
                    {
                        completeUrl = Utils.GetAbsoluteUrl(Url, Url.Action("PayComplete", "Coupon", new { channel = WHotelSite.Common.Config.DefaultPayChannelList, order = order.OrderID.ToString(), _t = _timeStamp, _isoneoff = 1, _clearoneoff = 1 }));
                    }
                    else
                    {
                        completeUrl = Utils.GetAbsoluteUrl(Url, Url.Action("PayComplete", "Coupon", new { channel = WHotelSite.Common.Config.DefaultPayChannelList, order = order.OrderID.ToString(), _t = _timeStamp }));
                    }
                    completeUrl = Server.UrlEncode(completeUrl);
                    dict["Message"] = "订单已提交";
                    dict["Success"] = "0";
                    dict["Exids"] = string.Join(",", order.ExchangeIdList);
                    dict["Url"] = string.Format("whotelapp://orderPay?orderid={0}&finishurl={1}&paytype={2}", order.OrderID, completeUrl, WHotelSite.Common.Config.DefaultPayChannelList);
                }
                else
                {
                    dict["Message"] = "订单已提交";
                    dict["Success"] = "0";
                    dict["Exids"] = string.Join(",", order.ExchangeIdList);
                    dict["Url"] = Url.Action("Pay", "Order", new { order = order.OrderID, payChannels = WHotelSite.Common.Config.DefaultPayChannelList });
                }

                #region 行为记录

                //var value = string.Format("{{\"skuid\":\"{0}\",\"aid\":\"{1}\",\"atype\":\"{2}\",\"paynum\":\"{3}\",\"userid\":\"{4}\",\"stype\":\"{5}\",\"return\":\"订单已提交\"}}", skuid, aid, atype, paynum, userid, stype);
                //RecordBehavior("CouponProductSubmit", value);

                #endregion
            }
            else
            {
                dict["Message"] = (order != null ? order.Message : "订单提交失败");
                dict["Success"] = "1";
                dict["Exids"] = "";
                dict["Url"] = "";

                #region 行为记录

                //var value = string.Format("{{\"skuid\":\"{0}\",\"aid\":\"{1}\",\"atype\":\"{2}\",\"paynum\":\"{3}\",\"userid\":\"{4}\",\"stype\":\"{5}\",\"return\":\"订单提交失败\"}}", skuid, aid, atype, paynum, userid, stype);
                //RecordBehavior("CouponProductSubmit", value);

                #endregion
            }

            return Json(dict, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 大团购补尾款
        /// </summary>
        /// <param name="depositskuid"></param>
        /// <param name="tailskuid"></param>
        /// <param name="tailprice"></param>
        /// <param name="couponorderid"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public ActionResult SubmitConponForStepGroup(int depositskuid, int tailskuid, decimal tailprice, long couponorderid, long userid = 0)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();

            //是否大于等于5.4版本
            var isThanVer5_4 = IsThanVer5_4();
            ViewBag.IsThanVer5_4 = isThanVer5_4;

            CouponOrderResult order = new Coupon().SubmitStepGroupOrder(depositskuid, tailskuid, userid, tailprice, couponorderid);
            if (order != null && order.Success == 0)
            {
                if (IsLatestVerApp())
                {
                    var _timeStamp = Signature.GenTimeStamp();

                    var completeUrl = "";
                    if (isThanVer5_4)
                    {
                        completeUrl = Utils.GetAbsoluteUrl(Url, Url.Action("PayComplete", "Coupon", new { channel = WHotelSite.Common.Config.DefaultPayChannelList, order = order.OrderID.ToString(), _t = _timeStamp, _isoneoff = 1, _clearoneoff = 1 }));
                    }
                    else
                    {
                        completeUrl = Utils.GetAbsoluteUrl(Url, Url.Action("PayComplete", "Coupon", new { channel = WHotelSite.Common.Config.DefaultPayChannelList, order = order.OrderID.ToString(), _t = _timeStamp }));
                    }
                    completeUrl = Server.UrlEncode(completeUrl);
                    dict["Message"] = "订单已提交";
                    dict["Success"] = "0";
                    dict["Exids"] = string.Join(",", order.ExchangeIdList);
                    dict["Url"] = string.Format("whotelapp://orderPay?orderid={0}&finishurl={1}&paytype={2}", order.OrderID, completeUrl, WHotelSite.Common.Config.DefaultPayChannelList);
                }
                else
                {
                    dict["Message"] = "订单已提交";
                    dict["Success"] = "0";
                    dict["Exids"] = string.Join(",", order.ExchangeIdList);
                    dict["Url"] = Url.Action("Pay", "Order", new { order = order.OrderID, payChannels = WHotelSite.Common.Config.DefaultPayChannelList });
                }
            }
            else
            {
                dict["Message"] = (order != null ? order.Message : "订单提交失败");
                dict["Success"] = "1";
                dict["Exids"] = "";
                dict["Url"] = "";
            }

            return Json(dict, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 更多消费券列表
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="albumId">专辑ID：2美食 3玩乐 4促销/限时 5超值团</param>
        /// <param name="category">产品分类类型ID：14餐券 20玩乐券 15房券 0其它</param>
        /// <param name="grid">是否grid方式展示（默认0）</param>
        /// <param name="isdouble11">是否双11</param>
        /// <param name="cid"></param>
        /// <param name="districtId"></param>
        /// <param name="districtName"></param>
        /// <param name="geoScopeType"></param>
        /// <returns></returns>
        public ActionResult MoreProductList(int userid = 0, string albumId = "0", int category = 0, int grid = 0, int isdouble11 = 0, int cid = 0, int districtId = 0, string districtName = "", int geoScopeType = 1, bool ischild = false)
        {
            var isApp = IsApp();
            ViewBag.IsApp = isApp;

            //当前系统环境（ios | android）
            var appType = AppType();
            ViewBag.AppType = appType;

            //是否大于等于5.1版本
            var isThanVer5_1 = IsThanVer5_1();
            ViewBag.IsThanVer5_1 = isThanVer5_1;

            //手动赋值cid
            if (cid > 0)
            {
                System.Web.HttpContext.Current.Session["ChannelID"] = cid;   
            }

            //Weixin
            bool isInWeixin = Request.UserAgent.IndexOf("MicroMessenger") > 0 ? true : false;
            ViewBag.isInWeixin = isInWeixin;

            var curUserID = UserState.UserID > 0 ? UserState.UserID : Convert.ToInt64(userid);
            if (curUserID <= 0)
            {
                curUserID = _ContextBasicInfo.AppUserID;
            }
            ViewBag.UserId = curUserID;

            //当前用户是否VIP
            var isVip = false; if (curUserID > 0) isVip = account.IsVIPCustomer(new AccountInfoItem { Uid = curUserID.ToString() });
            ViewBag.IsVip = isVip;

            ViewBag.AlbumId = albumId;
            ViewBag.CategoryId = category;
            ViewBag.DistrictId = districtId;
            ViewBag.DistrictName = districtName;
            ViewBag.GeoScopeType = geoScopeType;

            ViewBag.IsGridStyle = grid == 1;
            ViewBag.IsDouble11 = isdouble11 == 1;


            ViewBag.IsChild = ischild;
            #region 行为记录

            var value = string.Format("{{\"albumId\":\"{0}\",\"userNo\":\"{1}\"}}", albumId, curUserID);
            RecordBehavior("MoreProductListLoad", value);

            #endregion

            return View();
        }

        /// <summary>
        /// 亲子玩乐着陆页（可指定其他分类，目前只是亲子玩乐在用 2018.07.20 haoy）
        /// </summary>
        /// <param name="categoryId"></param>
        /// <param name="userId"></param>
        /// <param name="did">目的地ID</param>
        /// <param name="dName">目的地名称</param>
        /// <returns></returns>
        public ActionResult AlbumProductInfo(int categoryId = 0, long userId = 0, int did = 0,string dName = "全部城市")
        {
            var curUserID = UserState.UserID > 0 ? UserState.UserID : Convert.ToInt64(userId);
            if (curUserID <= 0)
            {
                curUserID = _ContextBasicInfo.AppUserID;
            }

            //Weixin
            bool isInWeixin = Request.UserAgent.IndexOf("MicroMessenger") > 0 ? true : false;
            ViewBag.isInWeixin = isInWeixin;

            ViewBag.UserId = curUserID;
            ViewBag.CategoryId = categoryId;
            var isApp = IsApp();
            ViewBag.IsApp = isApp;

            ViewBag.DId = did;
            ViewBag.DName = dName;


            return View();
        }

        /// <summary>
        /// 更多消费券项目（Item）
        /// </summary>
        /// <returns></returns>
        public ActionResult MoreProduct_SKU(int userid = 0, int s = 0, int c = 6, int albumId = 0)
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

        /// <summary>
        /// 验证指定条件下的代金券的合法性
        /// </summary>
        /// <param name="_paramObj"></param>
        /// <param name="selectedVoucherIDs"></param>
        /// <returns></returns>
        public ActionResult CheckSelectedVoucherInfoForOrder(int buyCount, decimal totalOrderPrice, int orderSourceID, int orderTypeID, decimal maxOrderCanUseVoucherAmount, string selectedVoucherIDs, int userID)
        {
            var selectedCouponIds = new List<int>();
            if (!string.IsNullOrEmpty(selectedVoucherIDs))
            {
                selectedCouponIds = selectedVoucherIDs.Split(',').Select(_ => Convert.ToInt32(_)).ToList();
            }

            var couponParamObj = new OrderVoucherRequestParams();
            couponParamObj.BuyCount = buyCount;
            couponParamObj.TotalOrderPrice = totalOrderPrice;
            couponParamObj.OrderSourceID = orderSourceID;
            couponParamObj.OrderTypeID = (CashCouponOrderSorceType)orderTypeID;
            couponParamObj.UserID = userID;
            couponParamObj.SelectedDateFrom = DateTime.Parse("2017-10-01");
            couponParamObj.SelectedDateTo = DateTime.Parse("2017-10-02");
            couponParamObj.MaxOrderCanUseVoucherAmount = maxOrderCanUseVoucherAmount;
            couponParamObj.SelectedVoucherIDs = selectedCouponIds;

            var _result = Coupon.CheckSelectedVoucherInfoForOrder(couponParamObj);

            return Json(_result, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region 手拉手相关

        /// <summary>
        /// 消费券购买页
        /// </summary>
        /// <param name="skuid"></param>
        /// <param name="groupid"></param>
        /// <param name="userid"></param>
        /// <param name="sid"></param>
        /// <param name="distributioncid"></param>
        /// <param name="_sourcekey"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public ActionResult CouponShopForGroupProduct(int skuid, int groupid = 0, string userid = "0", string sid = "", long distributioncid = 0, string _sourcekey = "", string code = "")
        {
            var isApp = IsApp();
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
            ViewBag.IsApp = isApp;
            ViewBag.SKUID = skuid;
            ViewBag.GROUPID = groupid;
          //  ViewBag.CID = GetCurCID();

            var curUserID = UserState.UserID > 0 ? UserState.UserID.ToString() : (isApp ? userid : "0");
            ViewBag.UserId = curUserID;

            //获取请求参数中的CID
            var requestCID = GetCurCIDForRequest();
            ViewBag.RequestCID = requestCID;

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

            //获取产品店铺信息
            var productShopInfo = new RetailerShopEntity();
            if (requestCID > 0)
            {
                //查询指定CID的店铺信息
                productShopInfo = Shop.GetRetailerShopByCID(requestCID);
            }
            ViewBag.ProductShopInfo = productShopInfo;

            //查询当前用户的度假伙伴状态
            var partnerResult = Partner.GetRetailerInvate(Convert.ToInt64(curUserID));
            ViewBag.PartnerResult = partnerResult;

            //分销查看（如果distributioncid>0，说明是从分销后台跳转过来打开的，那么优先显示该cid的分销状态信息；如果没有传，但是当前用户是分销，则显示当前用户的分销状态信息）
            var pcid = distributioncid;
            if (pcid <= 0 && Convert.ToInt64(curUserID) > 0 && (RetailerInvateState)partnerResult.State == RetailerInvateState.Pass)
            {
                pcid = Convert.ToInt64(curUserID);
            }
            ViewBag.PCID = pcid;

            //获取券SKU信息
            var couponSkuInfo = new Coupon().GetGroupSKUCouponActivityModel(skuid, Convert.ToInt64(userid), groupid);
            ViewBag.CouponSkuInfo = couponSkuInfo;

            //是否积攒拼团类型
            var isLikeGroup = couponSkuInfo.SKUInfo.Category.ID == 21;
            ViewBag.IsLikeGroup = isLikeGroup;

            //如果是积攒拼团，并且当前有小团，但当前用户不是发起者，则跳转至tree活动页
            if (isLikeGroup && groupid > 0 && !couponSkuInfo.IsCreator && curUserID != "0")
            {
                //return Redirect(string.Format("/coupon/group/tree/{0}/{1}", skuid, groupid));
            }

            //微信用户信息
            WeixinUserInfo weixinUserInfo = new WeixinUserInfo();

            //如果当前产品是积攒拼团，则需要用户微信授权
            if (isLikeGroup)
            {
                //获取当前产品的归属微信账号
                var weixinAcount = couponSkuInfo.activity.WeixinAcountId > 0 ? couponSkuInfo.activity.WeixinAcountId : 7;
                ViewBag.WeixinAcount = weixinAcount;

                if (isInWeixin)
                {
                    #region 有code，说明微信已回调

                    if (!string.IsNullOrEmpty(code))
                    {
                        var weixinAcountName = "weixinservice_haoyi";

                        //通过code换取网页授权access_token
                        WeixinAccessToken accessToken = new WeixinAccessToken();
                        switch ((WeiXinChannelCode)weixinAcount)
                        {
                            case WeiXinChannelCode.周末酒店服务号_皓颐:
                                weixinAcountName = "weixinservice_haoyi";
                                accessToken = WeiXinHelper.GetWeixinAccessTokenForHaoYi(code);
                                break;
                            case WeiXinChannelCode.周末酒店苏州服务号_皓颐:
                                weixinAcountName = "weixinservice_haoyi_sz";
                                accessToken = WeiXinHelper.GetWeixinAccessTokenForHaoYiSZ(code);
                                break;
                            case WeiXinChannelCode.周末酒店成都服务号_皓颐:
                                weixinAcountName = "weixinservice_haoyi_cd";
                                accessToken = WeiXinHelper.GetWeixinAccessTokenForHaoYiCD(code);
                                break;
                            case WeiXinChannelCode.周末酒店深圳服务号_皓颐:
                                weixinAcountName = "weixinservice_haoyi_shenz";
                                accessToken = WeiXinHelper.GetWeixinAccessTokenForHaoYiShenZ(code);
                                break;
                            case WeiXinChannelCode.遛娃指南服务号_皓颐:
                                weixinAcountName = "weixinservice_haoyi_liuwa";
                                accessToken = WeiXinHelper.GetWeixinAccessTokenForHaoYiLiuwa(code);
                                break;
                            case WeiXinChannelCode.遛娃指南南京服务号_皓颐:
                                weixinAcountName = "weixinservice_haoyi_liuwa_nj";
                                accessToken = WeiXinHelper.GetWeixinAccessTokenForHaoYiLiuwaNJ(code);
                                break;
                            case WeiXinChannelCode.遛娃指南无锡服务号_皓颐:
                                weixinAcountName = "weixinservice_haoyi_liuwa_wx";
                                accessToken = WeiXinHelper.GetWeixinAccessTokenForHaoYiLiuwaWX(code);
                                break;
                            case WeiXinChannelCode.遛娃指南广州服务号_皓颐:
                                weixinAcountName = "weixinservice_haoyi_liuwa_gz";
                                accessToken = WeiXinHelper.GetWeixinAccessTokenForHaoYiLiuwaGZH(code);
                                break;
                            case WeiXinChannelCode.遛娃指南杭州服务号_皓颐:
                                weixinAcountName = "weixinservice_haoyi_liuwa_hzh";
                                accessToken = WeiXinHelper.GetWeixinAccessTokenForHaoYiLiuwaHZH(code);
                                break;
                        }
                        //var accessToken = new WeixinAccessToken { Openid = "okg6-uBqijun__Frr_dJfV8m6Tjs" };

                        //通过access_token拉取用户信息(需scope为 snsapi_userinfo)（这是通过微信api获取的真实用户信息）
                        weixinUserInfo = WeiXinHelper.GetWeixinUserInfo(accessToken.AccessToken, accessToken.Openid);
                        if (weixinUserInfo != null && !string.IsNullOrEmpty(weixinUserInfo.Openid))
                        {
                            //存储微信用户
                            try
                            {
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
                                    WeixinAcount = weixinAcountName,
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
                            if (Convert.ToInt64(curUserID) > 0)
                            {
                                WeiXinHelper.BindingWxAndUid(Convert.ToInt64(curUserID), weixinUserInfo.Unionid);
                            }

                            #endregion

                            #region 更新Unionid缓存并更新CID信息

                            SetCurWXUnionid(weixinUserInfo.Unionid);
                            CheckCID();

                            #endregion
                        }
                        else
                        {
                            //如果code有值，但不能获取，一般说明code过期了，那么重新获取
                            var weixinGoUrl = "";
                            if (requestCID > 0)
                            {
                                weixinGoUrl = WeiXinHelper.GenConfrimAuthorUrlByWeixinAcount((WeiXinChannelCode)weixinAcount, HttpUtility.UrlEncode(string.Format("http://www.zmjiudian.com/coupon/group/product/{0}/{1}?CID={2}&distributioncid={3}&_sourcekey={4}", skuid, groupid, requestCID, distributioncid, _sourcekey)));
                            }
                            else
                            {
                                weixinGoUrl = WeiXinHelper.GenConfrimAuthorUrlByWeixinAcount((WeiXinChannelCode)weixinAcount, HttpUtility.UrlEncode(string.Format("http://www.zmjiudian.com/coupon/group/product/{0}/{1}?distributioncid={2}&_sourcekey={3}", skuid, groupid, distributioncid, _sourcekey)));
                            }
                            if (isInWeixin)
                            {
                                return Redirect(weixinGoUrl);
                            }
                        }
                    }

                    #endregion

                    #region 没有code，需要微信授权回调

                    else
                    {
                        //没有授权的跳转至授权页面
                        var weixinGoUrl = "";
                        if (requestCID > 0)
                        {
                            weixinGoUrl = WeiXinHelper.GenConfrimAuthorUrlByWeixinAcount((WeiXinChannelCode)weixinAcount, HttpUtility.UrlEncode(string.Format("http://www.zmjiudian.com/coupon/group/product/{0}/{1}?CID={2}&distributioncid={3}&_sourcekey={4}", skuid, groupid, requestCID, distributioncid, _sourcekey)));
                        }
                        else
                        {
                            weixinGoUrl = WeiXinHelper.GenConfrimAuthorUrlByWeixinAcount((WeiXinChannelCode)weixinAcount, HttpUtility.UrlEncode(string.Format("http://www.zmjiudian.com/coupon/group/product/{0}/{1}?distributioncid={2}&_sourcekey={3}", skuid, groupid, distributioncid, _sourcekey)));
                        }
                        if (isInWeixin)
                        {
                            return Redirect(weixinGoUrl);
                        }
                    }

                    #endregion   
                }
            }
            ViewBag.WeixinUserInfo = weixinUserInfo;
            ViewBag.Unionid = weixinUserInfo.Unionid;

            //微信授权code
            ViewBag.Code = code;

            //单买/手拉手SKU
            var singleBuySKU = new SKUEntity();
            var groupBuySKU = new SKUEntity();
            if (couponSkuInfo.SKUInfo != null && couponSkuInfo.SKUInfo.SKUList != null && couponSkuInfo.SKUInfo.SKUList.Count > 0)
            {
                singleBuySKU = couponSkuInfo.SKUInfo.SKUList.Find(_ => !_.IsGroupSKU);
                groupBuySKU = couponSkuInfo.SKUInfo.SKUList.Find(_ => _.IsGroupSKU);
            }
            ViewBag.SingleBuySKU = singleBuySKU;
            ViewBag.GroupBuySKU = groupBuySKU;

            //如果当前抢购VIP专享，则判断当前用户是否为VIP
            var isVip = account.IsVIPCustomer(new AccountInfoItem { Uid = curUserID });
            ViewBag.IsVip = isVip;

            //是否是团发起人可以享受首单
            ViewBag.GroupSponsorCanBuyVIPFirstCoupon = couponSkuInfo.activity.GroupSponsorCanBuyVIPFirstCoupon;

            //isMobile
            var isMobile = Utils.IsMobile();
            ViewBag.IsMobile = isMobile;

            //获取当前用户信息
            var userInfo = account.GetUserInfoByUserID(Convert.ToInt64(curUserID));
            ViewBag.UserInfo = userInfo ?? new UserInfoResult();

            //房券URL
            var roomcouponUrl = GetUserWalletUrl(Convert.ToInt64(curUserID), "roomcoupon");
            ViewBag.WalletRoomCouponLink = roomcouponUrl;

            //消费券URL
            var productCouponUrl = GetUserWalletUrl(Convert.ToInt64(curUserID), "productcoupon", "detail", newpage: false, dorpdown: true);
            ViewBag.WalletProductCouponLink = productCouponUrl;

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

            ViewBag.SourceKey = _sourcekey;

            return View();
        }

        /// <summary>
        /// 手拉手购买提交
        /// </summary>
        /// <param name="aid"></param>
        /// <param name="atype"></param>
        /// <param name="pid"></param>
        /// <param name="pricetype"></param>
        /// <param name="paynum"></param>
        /// <param name="userid"></param>
        /// <param name="skuid"></param>
        /// <param name="stype"></param>
        /// <param name="groupId">手拉手</param>
        /// <param name="openid"></param>
        /// <param name="cashCouponIdx"></param>
        /// <param name="cashCouponType"></param>
        /// <param name="cashCouponAmount"></param>
        /// <param name="useFundAmount"></param>
        /// <param name="selectedVoucherIDs"></param>
        /// <param name="voucherAmount"></param>
        /// <param name="bookTempId"></param>
        /// <param name="bookTempDataString"></param>
        /// <param name="bookTempDescription"></param>
        /// <param name="travelPersons"></param>
        /// <returns></returns>
        public ActionResult SubmitConponForGroupProduct(int aid, int atype, int skuid, int paynum, string userid, int stype, int groupId = 0, string openid = "", int cashCouponIdx = 0, int cashCouponType = 0, decimal cashCouponAmount = 0, decimal useFundAmount = 0, string selectedVoucherIDs = "", decimal voucherAmount = 0, int bookTempId = 0, string bookTempDataString = "", string bookTempDescription = "", string travelPersons = "")
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();

            SubmitCouponOrderModel sub = new SubmitCouponOrderModel();
            sub.ActivityID = aid;
            sub.ActivityType = atype;
            sub.SKUID = skuid;
            sub.GroupId = groupId;
            sub.UserID = Convert.ToInt64(userid);
            sub.OpenId = openid;
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

            //现金券使用
            if (cashCouponIdx > 0 && cashCouponAmount > 0)
            {
                sub.UseCashCouponInfo = new HJD.CouponService.Contracts.Entity.UseCashCouponItem
                {
                    CashCouponID = cashCouponIdx,
                    CashCouponType = cashCouponType,
                    UseCashAmount = cashCouponAmount
                };
            }

            //代金券使用
            var selectedCouponIds = new List<int>();
            if (!string.IsNullOrEmpty(selectedVoucherIDs))
            {
                selectedCouponIds = selectedVoucherIDs.Split(',').Select(_ => Convert.ToInt32(_)).ToList();
            }

            if (selectedCouponIds.Count > 0 && voucherAmount > 0)
            {
                sub.UseVoucherInfo = new UseVoucherInfoEntity
                {
                    UseVoucherIDList = selectedCouponIds.ToList(),
                    UseVoucherAmount = voucherAmount
                };
            }

            //住基金使用
            if (useFundAmount > 0)
            {
                sub.UserUseHousingFundAmount = useFundAmount;
            }

            //提交的“其他信息”
            if (!string.IsNullOrEmpty(bookTempDataString))
            {
                try
                {
                    sub.TemplateData = new ProductService.Contracts.Entity.TemplateDataEntity
                    {
                        TemplateID = bookTempId,
                        TemplateItem = bookTempDataString,
                        Description = bookTempDescription
                    };
                }
                catch (Exception ex)
                {

                }
            }

            //出行人
            var _travelPersonIdList = new List<int>();
            if (!string.IsNullOrEmpty(travelPersons))
            {
                try
                {
                    _travelPersonIdList = travelPersons.Split(',').ToList().Select(_id => Convert.ToInt32(_id)).ToList();
                }
                catch (Exception ex)
                {
                    //return Content("出行人读取错误，请重试");
                }
            }
            sub.TravelId = _travelPersonIdList;

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
                    dict["Message"] = "抱歉，您还不是VIP。该团购活动需VIP身份用户可参与开团／参团，现在去成为VIP吧。";
                    dict["Success"] = "2";
                    dict["Exids"] = "";
                    dict["Url"] = "";
                    return Json(dict, JsonRequestBehavior.AllowGet);
                }
            }

            //获取券SKU信息
            var couponSkuInfo = Coupon.GetSKUCouponActivityDetail(skuid);

            //是否新VIP才能购买的产品
            if (couponSkuInfo.SKUInfo.SKU.TagsList != null)
            {
                //获取用户是否VIP
                var isVip = account.IsVIPCustomer(new AccountInfoItem { Uid = userid });

                #region 首单验证

                if (couponSkuInfo.SKUInfo.SKU.TagsList.Exists(_ => _.TagID == 1))
                {
                    //获取当前用户信息
                    var cusType = account.GetVIPTypeNum(new AccountInfoItem { Uid = userid });

                    if (isVip)
                    {
                        if ((HJDAPI.Common.Helpers.Enums.CustomerType)cusType == HJDAPI.Common.Helpers.Enums.CustomerType.vip199nr
                            || (HJDAPI.Common.Helpers.Enums.CustomerType)cusType == HJDAPI.Common.Helpers.Enums.CustomerType.vip599
                            || (HJDAPI.Common.Helpers.Enums.CustomerType)cusType == HJDAPI.Common.Helpers.Enums.CustomerType.vip199
                            || (HJDAPI.Common.Helpers.Enums.CustomerType)cusType == HJDAPI.Common.Helpers.Enums.CustomerType.vip)
                        {
                            // 如果手拉手套餐设置了 “发起人享受首单优惠”，那么用户可以没有首单权限 
                            if (groupId == 0 && coupon.activity.GroupSponsorCanBuyVIPFirstCoupon == true)
                            {
                                //do nothing
                            }
                            else if (AccountHelper.UserHasVIPFirstBuyPriviledge(userid) == false)//2010 首单爆款 
                            {
                                if (groupId > 0 && coupon.activity.GroupSponsorCanBuyVIPFirstCoupon == true)
                                {
                                    dict["Message"] = "抱歉，此次活动限新VIP会员参团，您已经是VIP了，可以去发起拼团哦。";
                                    dict["Success"] = "3";
                                    dict["Exids"] = "";
                                    dict["Url"] = "";
                                    return Json(dict, JsonRequestBehavior.AllowGet);
                                }
                                else
                                {
                                    dict["Message"] = "抱歉，此专享优惠套餐每位VIP会员限购一套哦，您之前已经购买过啦～";
                                    dict["Success"] = "1";
                                    dict["Exids"] = "";
                                    dict["Url"] = "";
                                    return Json(dict, JsonRequestBehavior.AllowGet);
                                }
                            }
                        }
                        else
                        {
                            dict["Message"] = "抱歉，此价格仅供新VIP会员专享哦～";
                            dict["Success"] = "1";
                            dict["Exids"] = "";
                            dict["Url"] = "";
                            return Json(dict, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        dict["Message"] = "抱歉，此价格为新VIP会员专享，成为VIP会员才能购买哦～";
                        dict["Success"] = "2";
                        dict["Exids"] = "";
                        dict["Url"] = "";
                        return Json(dict, JsonRequestBehavior.AllowGet);
                    }

                    //新VIP专享的套餐用户只能买一件
                    if (paynum > 1)
                    {
                        dict["Message"] = "抱歉，此专享优惠套餐每位VIP会员限购一套哦～";
                        dict["Success"] = "1";
                        dict["Exids"] = "";
                        dict["Url"] = "";
                        return Json(dict, JsonRequestBehavior.AllowGet);
                    }
                }

                #endregion

                #region 是否只限普通会员专享

                else if (couponSkuInfo.SKUInfo.SKU.TagsList.Exists(_ => _.TagID == 4))
                {
                    if (isVip)
                    {
                        dict["Message"] = "抱歉，此套餐仅限普通用户购买";
                        dict["Success"] = "1";
                        dict["Exids"] = "";
                        dict["Url"] = "";

                        return Json(dict, JsonRequestBehavior.AllowGet);
                    }
                }

                #endregion
   
            }

            CouponOrderResult order = new Coupon().SubmitGroupOrderForProduct(sub);
            if (order != null && order.Success == 0)
            {
                //铂涛的跳转页面 发送短信验证码
                var botaoCode = HJDAPI.Common.Helpers.Enums.ThirdPartyMerchantCode.bohuijinrong.ToString();
                var botaophone = ClientHelper.GetBotaoPhoneNumFromCookie(HttpContext.Request.Cookies);

                if (!string.IsNullOrWhiteSpace(botaophone))
                {
                    var phoneNum = botaophone;
                    var amount = (int)(order.Amount * 100);// order.Amount * 100;//元转分
                    var goodsInfo = order.GoodsInfo;
                    var orderId = order.OrderID;

                    var payWebSiteUrl = (string)System.Configuration.ConfigurationManager.AppSettings["PayWebSiteUrl"];
                    var completeUrl = string.Format("{0}Pay/BoTaoPayComplete/{1}/{2}", payWebSiteUrl, "coupon", orderId);

                    var sign = EncryptMethod.GenSignature4Pay(botaoCode, phoneNum, amount, order.OrderID, completeUrl);
                    dict["Message"] = "订单已提交";
                    dict["Success"] = "0";
                    dict["Exids"] = string.Join(",", order.ExchangeIdList);
                    var jumpUrl = string.Format("{7}pay/botao?mobileId={0}&merName={1}&goodsInf={2}&amount={3}&orderId={4}&retUrl={5}&sign={6}", phoneNum, botaoCode, goodsInfo, amount, order.OrderID, completeUrl, sign, payWebSiteUrl);
                    dict["Url"] = jumpUrl;
                }
                else if (IsLatestVerApp())
                {
                    var _timeStamp = Signature.GenTimeStamp();

                    var completeUrl = "";
                    if (isThanVer5_4)
                    {
                        completeUrl = Utils.GetAbsoluteUrl(Url, Url.Action("PayComplete", "Coupon", new { channel = WHotelSite.Common.Config.DefaultPayChannelList, order = order.OrderID.ToString(), _t = _timeStamp, _isoneoff = 1, _clearoneoff = 1 }));
                    }
                    else
                    {
                        completeUrl = Utils.GetAbsoluteUrl(Url, Url.Action("PayComplete", "Coupon", new { channel = WHotelSite.Common.Config.DefaultPayChannelList, order = order.OrderID.ToString(), _t = _timeStamp }));  
                    }
                    completeUrl = Server.UrlEncode(completeUrl);
                    dict["Message"] = "订单已提交";
                    dict["Success"] = "0";
                    dict["Exids"] = string.Join(",", order.ExchangeIdList);
                    dict["Url"] = string.Format("whotelapp://orderPay?orderid={0}&finishurl={1}&paytype={2}", order.OrderID, completeUrl, WHotelSite.Common.Config.DefaultPayChannelList);
                }
                else
                {
                    dict["Message"] = "订单已提交";
                    dict["Success"] = "0";
                    dict["Exids"] = string.Join(",", order.ExchangeIdList);
                    dict["Url"] = Url.Action("Pay", "Order", new { order = order.OrderID, payChannels = WHotelSite.Common.Config.DefaultPayChannelList  });
                }
            }
            else
            {
                dict["Message"] = (order != null ? order.Message : "订单提交失败");
                dict["Success"] = "1";
                dict["Exids"] = "";
                dict["Url"] = "";
            }

            return Json(dict, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region 大团购（阶梯团）相关

        /// <summary>
        /// 大团购详情购买页面
        /// </summary>
        /// <param name="skuid"></param>
        /// <param name="userid"></param>
        /// <param name="sid"></param>
        /// <param name="fromwxno">是否来自微信公众号，只有微信公众号里放的链接才会传1</param>
        /// <param name="fromwxuid">来自哪一个微信号的分享推荐（全员分销使用）</param>
        /// <param name="distributioncid"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public ActionResult CouponShopForStepGroup(int skuid, string userid = "0", string sid = "", int fromwxno = 0, int fromwxuid = 0, long distributioncid = 0, string code = "")
        {
            var isApp = IsApp();
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
            ViewBag.IsApp = isApp;
            ViewBag.SKUID = skuid;

            var curUserID = UserState.UserID > 0 ? UserState.UserID : (isApp ? Convert.ToInt64(userid) : 0);
            if (curUserID <= 0)
            {
                curUserID = _ContextBasicInfo.AppUserID;
            }
            if (curUserID >= 0 && _ContextBasicInfo.AppUserID <= 0)
            {
                _ContextBasicInfo.AppUserID = curUserID;
            }
            ViewBag.UserId = curUserID;

            //获取请求参数中的CID
            var requestCID = GetCurCIDForRequest();
            ViewBag.RequestCID = requestCID;

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

            //当前连接来自哪一个微信用户分销(全员分销时使用)
            ViewBag.FromWxUid = fromwxuid;

            #region 微信环境下，做微信静默授权

            WeixinUserInfo weixinUserInfo = new WeixinUserInfo();
            var openid = "";
            var unionid = "";
            var wxuid = 0;
            if (isInWeixin)
            {
                if (!string.IsNullOrEmpty(code))
                {
                    var weixinAcountName = "weixinservice_haoyi";

                    //通过code换取网页授权access_token
                    var accessToken = WeiXinHelper.GetWeixinAccessTokenForHaoYi(code);
                    if (accessToken != null && !string.IsNullOrEmpty(accessToken.Openid))
                    {
                        openid = accessToken.Openid;

                        //获取用户信息
                        //通过access_token拉取用户信息(需scope为 snsapi_userinfo)（这是通过微信api获取的真实用户信息）
                        weixinUserInfo = WeiXinHelper.GetWeixinUserInfo(accessToken.AccessToken, accessToken.Openid);
                        if (weixinUserInfo != null && !string.IsNullOrEmpty(weixinUserInfo.Openid))
                        {
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
                                    WeixinAcount = weixinAcountName,   //WeiXinChannelCode.周末酒店服务号_皓颐
                                    Language = "zh_CN",
                                    SubscribeTime = DateTime.Now,
                                    CreateTime = DateTime.Now
                                };
                                var update = new Weixin().UpdateWeixinUserSubscribe(w);

                                //get weixin user
                                var weixinUser = new Weixin().GetWeixinUser(openid);
                                if (weixinUser != null && weixinUser.ID > 0)
                                {
                                    wxuid = weixinUser.ID;
                                }
                            }
                            catch (Exception ex)
                            {

                            }

                            #region 处理微信号绑定用户信息相关操作

                            //微信环境下，如果当前已经登录，则直接绑定
                            if (curUserID > 0)
                            {
                                WeiXinHelper.BindingWxAndUid(curUserID, weixinUserInfo.Unionid);
                            }

                            #endregion

                            #region 更新Unionid缓存并更新CID信息

                            SetCurWXUnionid(weixinUserInfo.Unionid);
                            CheckCID();

                            #endregion
                        }
                    }
                    else
                    {
                        //如果code有值，但不能获取，一般说明code过期了，那么重新获取
                        var weixinGoUrl = "";
                        if (requestCID > 0)
                        {
                            weixinGoUrl = WeiXinHelper.GenSilenceAuthorUrlForHaoYi(HttpUtility.UrlEncode(string.Format("http://www.zmjiudian.com/coupon/stepgroup/product/{0}?sid={2}&fromwxno={3}&fromwxuid={4}&CID={5}&distributioncid={6}", skuid, userid, sid, fromwxno, fromwxuid, requestCID, distributioncid)));
                        }
                        else
                        {
                            weixinGoUrl = WeiXinHelper.GenSilenceAuthorUrlForHaoYi(HttpUtility.UrlEncode(string.Format("http://www.zmjiudian.com/coupon/stepgroup/product/{0}?sid={2}&fromwxno={3}&fromwxuid={4}&distributioncid={5}", skuid, userid, sid, fromwxno, fromwxuid, distributioncid)));
                        }
                        return Redirect(weixinGoUrl);
                    }
                }
                else
                {
                    //授权页面
                    var weixinGoUrl = "";
                    if (requestCID > 0)
                    {
                        weixinGoUrl = WeiXinHelper.GenSilenceAuthorUrlForHaoYi(HttpUtility.UrlEncode(string.Format("http://www.zmjiudian.com/coupon/stepgroup/product/{0}?sid={2}&fromwxno={3}&fromwxuid={4}&CID={5}&distributioncid={6}", skuid, userid, sid, fromwxno, fromwxuid, requestCID, distributioncid)));
                    }
                    else
                    {
                        weixinGoUrl = WeiXinHelper.GenSilenceAuthorUrlForHaoYi(HttpUtility.UrlEncode(string.Format("http://www.zmjiudian.com/coupon/stepgroup/product/{0}?sid={2}&fromwxno={3}&fromwxuid={4}&distributioncid={5}", skuid, userid, sid, fromwxno, fromwxuid, distributioncid)));
                    }
                    return Redirect(weixinGoUrl);
                }
            }
            ViewBag.Openid = openid;
            ViewBag.Unionid = weixinUserInfo.Unionid;
            ViewBag.WeixinUid = wxuid;

            #endregion

            //如果fromwxuid与当前wxuid一致，则清楚fromwxuid（自己分享给自己不算）
            if (wxuid == fromwxuid)
            {
                fromwxuid = 0;
            }

            //获取产品店铺信息
            var productShopInfo = new RetailerShopEntity();
            if (requestCID > 0)
            {
                //查询指定CID的店铺信息
                productShopInfo = Shop.GetRetailerShopByCID(requestCID);
            }
            ViewBag.ProductShopInfo = productShopInfo;

            //查询当前用户的度假伙伴状态
            var partnerResult = Partner.GetRetailerInvate(curUserID);
            ViewBag.PartnerResult = partnerResult;

            //分销查看（如果distributioncid>0，说明是从分销后台跳转过来打开的，那么优先显示该cid的分销状态信息；如果没有传，但是当前用户是分销，则显示当前用户的分销状态信息）
            var pcid = distributioncid;
            if (pcid <= 0 && curUserID > 0 && (RetailerInvateState)partnerResult.State == RetailerInvateState.Pass)
            {
                pcid = curUserID;
            }
            ViewBag.PCID = pcid;

            //获取券SKU信息
            var couponSkuInfo = Coupon.GetSKUCouponActivityDetail(skuid);
            ViewBag.CouponSkuInfo = couponSkuInfo;

            #region 检查当前sku是不是手拉手的产品，是的话则重定向到手拉手产品

            if (couponSkuInfo.activity != null && couponSkuInfo.activity.GroupCount > 0 && couponSkuInfo.activity.GroupDay > 0)
            {
                var _groupRdUrl = string.Format("/coupon/group/product/{0}/0?cid={1}&distributioncid={2}", skuid, requestCID, distributioncid);
                return Redirect(_groupRdUrl);
            }

            #endregion

            //如果当前抢购VIP专享，则判断当前用户是否为VIP
            var isVip = account.IsVIPCustomer(new AccountInfoItem { Uid = curUserID.ToString() });
            ViewBag.IsVip = isVip;

            //isMobile
            var isMobile = Utils.IsMobile();
            ViewBag.IsMobile = isMobile;

            //获取当前用户信息
            var userInfo = account.GetUserInfoByUserID(curUserID);
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

            //标识是否来自微信公众号
            var _fromWxno = fromwxno == 1;
            ViewBag.FromWxno = _fromWxno;

            #region 检查当前抢购的活动规则关联

            //是否需要弹出去微信报名的提示
            var showSignWeixin = false;

            //couponSkuInfo.activity.RelationId = 401;
            if (couponSkuInfo.activity != null && couponSkuInfo.activity.RelationId > 0)
            {
                var relActiveId = couponSkuInfo.activity.RelationId;
                var weixinActiveEntity = ActiveController.GetWeixinActiveEntity(relActiveId);
                if (weixinActiveEntity != null)
                {
                    ViewBag.WeixinActiveEntity = weixinActiveEntity;

                    var userPhone = "";
                    try
                    {
                        var userEntity = account.GetCurrentUserInfo(Convert.ToInt64(userid));
                        if (userEntity != null) { userPhone = userEntity.MobileAccount; }
                    }
                    catch (Exception ex) { }

                    //根据微信关联活动和手机号，检查报名信息
                    var weixinDraw = new Weixin().GetActiveWeixinDrawByPhone(relActiveId, userPhone);

                    //没有报名或者没有分享
                    if (weixinDraw == null || weixinDraw.IsShare == 0)
                    {
                        //如果是从微信公众号过来的，直接跳转到预约页面
                        if (_fromWxno)
                        {
                            if (weixinActiveEntity.ActiveEndTime > DateTime.Now)
                            {
                                //合作伙伴ID
                                var _ptid = 7;
                                switch (weixinActiveEntity.WeixinAcountId)
                                {
                                    case 3: _ptid = 17; break;   //尚旅游
                                    case 4: _ptid = 18; break;   //尚旅游成都
                                    case 5: _ptid = 45; break;   //美味至尚
                                    case 6: _ptid = 47; break;   //尚旅游北京
                                }
                                return Redirect(string.Format("http://www.zmjiudian.com/wx/active/reg/{0}/{1}", _ptid, relActiveId));
                            }
                        }
                        //如果非微信公众号过来的，则弹出去预约的提示
                        else
                        {
                            showSignWeixin = true;
                        }
                    }
                }
            }

            ViewBag.ShowSignWeixin = showSignWeixin;

            #endregion

            #region 分享配置

            var shareLink = "";
            //分享链接生成
            if (requestCID > 0)
            {
                shareLink = string.Format("http://www.zmjiudian.com/coupon/product/{0}?CID={1}&fromwxuid={2}", skuid, requestCID, wxuid);
            }
            else
            {
                //暂时只有在APP中登录分享才会加sid&cid等参数
                if (isApp)
                {
                    //分享跟踪参数
                    var scp = new GenTrackCodeParam();
                    scp.UserID = Convert.ToInt64(curUserID);
                    scp.CouponActivityID = skuid;
                    scp.BizType = ZMJDShareTrackBizType.roomcoupondetail;
                    var shareCode = Comment.GenTrackCodeResult4Share(scp);

                    shareLink = string.Format("http://www.zmjiudian.com/coupon/product/{0}?sid={1}", skuid, shareCode.EncodeStr);
                }
                else
                {
                    //分享跟踪参数
                    var scp = new GenTrackCodeParam();
                    scp.UserID = Convert.ToInt64(curUserID);
                    scp.CouponActivityID = skuid;
                    scp.BizType = ZMJDShareTrackBizType.roomcoupondetail;
                    var shareCode = Comment.GenTrackCodeResult4Share(scp);

                    shareLink = string.Format("http://www.zmjiudian.com/coupon/product/{0}?fromwxuid={1}", skuid, wxuid);
                }
            }

            //解析SID
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

            ViewBag.ShareLink = shareLink;
            ViewBag.ShareUserInfo = shareUserInfo;

            #endregion

            #region 行为记录

            var value = string.Format("{{\"skuid\":\"{0}\",\"sid\":\"{1}\",\"userNo\":\"{2}\",\"title\":\"{3}\",\"CID\":\"{4}\",\"RequestCID\":\"{5}\",\"fromwxno\":\"{6}\"}}", skuid, sid, curUserID, (couponSkuInfo.activity != null && !string.IsNullOrEmpty(couponSkuInfo.activity.PageTitle) ? couponSkuInfo.activity.PageTitle : ""), GetCurCID(), GetCurCIDForRequest(), fromwxno);
            RecordBehavior("CouponProductStepGroupLoad", value);

            #endregion

            return View();
        }

        #endregion

        #region 拼团-点亮/种树

        /// <summary>
        /// Convert Byte[] to Image
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static Image BytesToImage(byte[] buffer)
        {
            MemoryStream ms = new MemoryStream(buffer);
            Image image = System.Drawing.Image.FromStream(ms);
            return image;
        }

        /// <summary>
        /// Convert Image to Byte[]
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static byte[] ImageToBytes(Image image)
        {
            ImageFormat format = image.RawFormat;
            using (MemoryStream ms = new MemoryStream())
            {
                if (format.Equals(ImageFormat.Jpeg))
                {
                    image.Save(ms, ImageFormat.Jpeg);
                }
                else if (format.Equals(ImageFormat.Png))
                {
                    image.Save(ms, ImageFormat.Png);
                }
                else if (format.Equals(ImageFormat.Bmp))
                {
                    image.Save(ms, ImageFormat.Bmp);
                }
                else if (format.Equals(ImageFormat.Gif))
                {
                    image.Save(ms, ImageFormat.Gif);
                }
                else if (format.Equals(ImageFormat.Icon))
                {
                    image.Save(ms, ImageFormat.Icon);
                }
                byte[] buffer = new byte[ms.Length];
                //Image.Save()会改变MemoryStream的Position，需要重新Seek到Begin
                ms.Seek(0, SeekOrigin.Begin);
                ms.Read(buffer, 0, buffer.Length);
                return buffer;
            }
        }

        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                { return codec; }
            }
            return null;
        }  

        /// <summary>
        /// 壓縮圖片 /// </summary>
        /// <param name="fileStream">圖片流</param>
        /// <param name="quality">壓縮質量0-100之間 數值越大質量越高</param>
        /// <returns></returns>
        private byte[] CompressionImage(Stream fileStream, long quality)
        {
            using (System.Drawing.Image img = System.Drawing.Image.FromStream(fileStream))
            {
                using (Bitmap bitmap = new Bitmap(img))
                {
                    ImageCodecInfo CodecInfo = GetEncoder(img.RawFormat);
                    System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;
                    EncoderParameters myEncoderParameters = new EncoderParameters(1);
                    EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, quality);
                    myEncoderParameters.Param[0] = myEncoderParameter;
                    using (MemoryStream ms = new MemoryStream())
                    {
                        bitmap.Save(ms, CodecInfo, myEncoderParameters);
                        myEncoderParameters.Dispose();
                        myEncoderParameter.Dispose();
                        return ms.ToArray();
                    }
                }
            }
        }

        //首先给出一个函数；用来将图片保存到内存中  
        private Image ReturnImage(string strPhotoPath)
        {
            WebRequest myrequest = WebRequest.Create(strPhotoPath);
            WebResponse myresponse = myrequest.GetResponse();
            Stream imgstream = myresponse.GetResponseStream();

            System.Drawing.Image image = BytesToImage(CompressionImage(imgstream, 50L));

            return image;

            //stream.Write(byData, 0, byData.Length);//此句可省略，上一句构造函数已经将字节流塞进流了
            Bitmap bmp = new Bitmap(imgstream);

            return bmp;
        }

        private void Fold_Click()
        {
            //产品名称
            var productName = "价值¥500BlueBubbleBS亲子游用课程免费体验";
            productName = "我领取了“" + productName + "”";

            //产品价格1
            var salePrice = "0";
            salePrice = "¥" + salePrice;

            //产品价格2
            var defPrice = "500";
            defPrice = "原价¥" + defPrice;

            string str0 = "http://whfront.b0.upaiyun.com/app/img/coupon/groupproductfortree/canvas-zmjd.png";//像框文件的路径
            string str1 = "http://p1.zmjiudian.com/117IQAW105l_640x360";//照片文件的全路径
            string str2 = "https://mp.weixin.qq.com/cgi-bin/showqrcode?ticket=gQGk8TwAAAAAAAAAAS5odHRwOi8vd2VpeGluLnFxLmNvbS9xLzAyY05PTGhZc0lmQjMxMDAwMDAwNzkAAgSQ6U1aAwQAAAAA";//照片文件的全路径
            Image img0 = ReturnImage(str0);
            Image img1 = ReturnImage(str1);
            Image img2 = ReturnImage(str2);
            //System.Drawing.Image newImage = img0.GetThumbnailImage(640, 848, null, new IntPtr());

            //Graphics g = Graphics.FromImage(img0);这句话改成如下，以控制背景图片尺寸
            Image bmap = new Bitmap(640, 848);
            Graphics gra = Graphics.FromImage(bmap);

            gra.DrawImage(img0, 0, 0, 640, 848);
            gra.DrawImage(img1, 0, 0, 640, 360);
            gra.DrawImage(img2, (640 - 200) / 2, 290, 200, 200);

            Font font = new Font("幼圆", 22, FontStyle.Bold);
            Brush bush = new SolidBrush(Color.Black);
            gra.DrawString(productName, font, bush, 90, 500);

            font = new Font("幼圆", 24, FontStyle.Bold);
            bush = new SolidBrush(Color.Red);
            gra.DrawString(salePrice, font, bush, ((640 - 200) / 2) + 10, 550);

            font = new Font("幼圆", 24, FontStyle.Regular);
            bush = new SolidBrush(Color.LightGray);
            gra.DrawString(defPrice, font, bush, ((640 - 200) / 2) + 40 + (salePrice.Length * 12), 550);

            bmap.Save(@"D:\Me\Task\2018-01-04\result pic.png");

            gra.Dispose();
        }

        /// <summary>
        /// 助力拼团
        /// </summary>
        /// <param name="skuid"></param>
        /// <param name="groupid"></param>
        /// <param name="userid"></param>
        /// <param name="sid"></param>
        /// <param name="_sourcekey"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public ActionResult GroupProductForTree(int skuid, int groupid = 0, long userid = 0, string sid = "", string _sourcekey = "", string code = "")
        {
            var isApp = IsApp();
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
            ViewBag.IsApp = isApp;
            ViewBag.SKUID = skuid;
            ViewBag.GROUPID = groupid;
            //  ViewBag.CID = GetCurCID();

            //Weixin
            bool isInWeixin = Request.UserAgent.IndexOf("MicroMessenger") > 0 ? true : false;
            ViewBag.isInWeixin = isInWeixin;

            var curUserID = UserState.UserID > 0 ? UserState.UserID : (isApp ? userid : 0);
            ViewBag.UserId = curUserID;

            //如果没有登录，暂时该页面全部重定向产品页面（该页面助力活动被举报 2018.08.09 haoy）
            if (curUserID <= 0)
            {
                return Redirect(string.Format("/coupon/group/product/{0}/{1}", skuid, 0));   
            }

            //如果没有GroupId，则直接跳转至产品页
            if (groupid == 0)
            {
                return Redirect(string.Format("/coupon/group/product/{0}/{1}", skuid, groupid));
            }

            //调试使用
            ViewBag.AppType = AppType();
            if (AppType() == "iosapp") ViewBag.AppVerForIOS = AppVerForIOS();
            else ViewBag.AppVerForIOS = "";
            if (AppType() == "android") ViewBag.AppVerForAndroid = AppVerForAndroid();
            else ViewBag.AppVerForAndroid = "";
            ViewBag.IsLatestVerApp = IsLatestVerApp();

            //微信用户信息
            WeixinUserInfo weixinUserInfo = new WeixinUserInfo();

            //获取券SKU信息
            var couponSkuInfo = new Coupon().GetGroupSKUCouponActivityModel(skuid, Convert.ToInt64(userid), groupid, weixinUserInfo.Openid);
            ViewBag.CouponSkuInfo = couponSkuInfo;

            //获取当前产品的归属微信账号
            var weixinAcount = couponSkuInfo.activity.WeixinAcountId > 0 ? couponSkuInfo.activity.WeixinAcountId : 7;
            ViewBag.WeixinAcount = weixinAcount;

            #region 有code，说明微信已回调

            if (!string.IsNullOrEmpty(code))
            {
                var weixinAcountName = "weixinservice_haoyi";

                //通过code换取网页授权access_token
                WeixinAccessToken accessToken = new WeixinAccessToken();
                switch ((WeiXinChannelCode)weixinAcount)
                {
                    case WeiXinChannelCode.周末酒店服务号_皓颐:
                        weixinAcountName = "weixinservice_haoyi";
                        accessToken = WeiXinHelper.GetWeixinAccessTokenForHaoYi(code);
                        break;
                    case WeiXinChannelCode.周末酒店苏州服务号_皓颐:
                        weixinAcountName = "weixinservice_haoyi_sz";
                        accessToken = WeiXinHelper.GetWeixinAccessTokenForHaoYiSZ(code);
                        break;
                    case WeiXinChannelCode.周末酒店成都服务号_皓颐:
                        weixinAcountName = "weixinservice_haoyi_cd";
                        accessToken = WeiXinHelper.GetWeixinAccessTokenForHaoYiCD(code);
                        break;
                    case WeiXinChannelCode.周末酒店深圳服务号_皓颐:
                        weixinAcountName = "weixinservice_haoyi_shenz";
                        accessToken = WeiXinHelper.GetWeixinAccessTokenForHaoYiShenZ(code);
                        break;
                    case WeiXinChannelCode.遛娃指南服务号_皓颐:
                        weixinAcountName = "weixinservice_haoyi_liuwa";
                        accessToken = WeiXinHelper.GetWeixinAccessTokenForHaoYiLiuwa(code);
                        break;
                    case WeiXinChannelCode.遛娃指南南京服务号_皓颐:
                        weixinAcountName = "weixinservice_haoyi_liuwa_nj";
                        accessToken = WeiXinHelper.GetWeixinAccessTokenForHaoYiLiuwaNJ(code);
                        break;
                    case WeiXinChannelCode.遛娃指南无锡服务号_皓颐:
                        weixinAcountName = "weixinservice_haoyi_liuwa_wx";
                        accessToken = WeiXinHelper.GetWeixinAccessTokenForHaoYiLiuwaWX(code);
                        break;
                    case WeiXinChannelCode.遛娃指南广州服务号_皓颐:
                        weixinAcountName = "weixinservice_haoyi_liuwa_gz";
                        accessToken = WeiXinHelper.GetWeixinAccessTokenForHaoYiLiuwaGZH(code);
                        break;
                    case WeiXinChannelCode.遛娃指南杭州服务号_皓颐:
                        weixinAcountName = "weixinservice_haoyi_liuwa_hzh";
                        accessToken = WeiXinHelper.GetWeixinAccessTokenForHaoYiLiuwaHZH(code);
                        break;
                }
                //var accessToken = new WeixinAccessToken { Openid = "okg6-uBqijun__Frr_dJfV8m6Tjs" };

                //通过access_token拉取用户信息(需scope为 snsapi_userinfo)（这是通过微信api获取的真实用户信息）
                weixinUserInfo = WeiXinHelper.GetWeixinUserInfo(accessToken.AccessToken, accessToken.Openid);
                if (weixinUserInfo != null && !string.IsNullOrEmpty(weixinUserInfo.Openid))
                {
                    //存储微信用户
                    try
                    {
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
                            WeixinAcount = weixinAcountName,
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
                    if (curUserID > 0)
                    {
                        WeiXinHelper.BindingWxAndUid(curUserID, weixinUserInfo.Unionid);
                    }

                    #endregion

                    #region 更新Unionid缓存并更新CID信息

                    SetCurWXUnionid(weixinUserInfo.Unionid);
                    CheckCID();

                    #endregion
                }
                else
                {
                    //如果code有值，但不能获取，一般说明code过期了，那么重新获取
                    var weixinGoUrl = WeiXinHelper.GenConfrimAuthorUrlByWeixinAcount((WeiXinChannelCode)weixinAcount, HttpUtility.UrlEncode(string.Format("http://www.zmjiudian.com/coupon/group/tree/{0}/{1}?_sourcekey={2}", skuid, groupid, _sourcekey)));
                    if (isInWeixin)
                    {
                        return Redirect(weixinGoUrl);
                    }
                }
            }

            #endregion

            #region 没有code，需要微信授权回调

            else
            {
                //没有授权的跳转至授权页面
                var weixinGoUrl = WeiXinHelper.GenConfrimAuthorUrlByWeixinAcount((WeiXinChannelCode)weixinAcount, HttpUtility.UrlEncode(string.Format("http://www.zmjiudian.com/coupon/group/tree/{0}/{1}?_sourcekey={2}", skuid, groupid, _sourcekey)));
                if (isInWeixin)
                {
                    return Redirect(weixinGoUrl);
                }
            }

            #endregion

            //都授权通过，才能走到这里，继续...
            ViewBag.WeixinUserInfo = weixinUserInfo;
            ViewBag.Unionid = weixinUserInfo.Unionid;

            //微信授权code
            ViewBag.Code = code;

            //单买/手拉手SKU
            var singleBuySKU = new SKUEntity();
            var groupBuySKU = new SKUEntity();
            if (couponSkuInfo.SKUInfo != null && couponSkuInfo.SKUInfo.SKUList != null && couponSkuInfo.SKUInfo.SKUList.Count > 0)
            {
                singleBuySKU = couponSkuInfo.SKUInfo.SKUList.Find(_ => !_.IsGroupSKU);
                groupBuySKU = couponSkuInfo.SKUInfo.SKUList.Find(_ => _.IsGroupSKU);
            }
            ViewBag.SingleBuySKU = singleBuySKU;
            ViewBag.GroupBuySKU = groupBuySKU;

            //如果当前抢购VIP专享，则判断当前用户是否为VIP
            var isVip = account.IsVIPCustomer(new AccountInfoItem { Uid = curUserID.ToString() });
            ViewBag.IsVip = isVip;

            //是否是团发起人可以享受首单
            ViewBag.GroupSponsorCanBuyVIPFirstCoupon = couponSkuInfo.activity.GroupSponsorCanBuyVIPFirstCoupon;

            //isMobile
            var isMobile = Utils.IsMobile();
            ViewBag.IsMobile = isMobile;

            //获取当前用户信息
            var userInfo = account.GetUserInfoByUserID(Convert.ToInt64(curUserID));
            ViewBag.UserInfo = userInfo ?? new UserInfoResult();

            //房券URL
            var roomcouponUrl = GetUserWalletUrl(Convert.ToInt64(curUserID), "roomcoupon");
            ViewBag.WalletRoomCouponLink = roomcouponUrl;

            //消费券URL
            var productCouponUrl = GetUserWalletUrl(Convert.ToInt64(curUserID), "productcoupon", "detail", newpage: false, dorpdown: true);
            ViewBag.WalletProductCouponLink = productCouponUrl;

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

            return View();
        }

        #endregion

        #region 券确认页面相关

        /// <summary>
        /// 券订单确认页
        /// </summary>
        /// <param name="skuid"></param>
        /// <param name="paynum"></param>
        /// <param name="userid"></param>
        /// <param name="groupid"></param>
        /// <param name="openid"></param>
        /// <param name="fromwxuid">来自哪一个微信号的分享推荐（全员分销使用）</param>
        /// <param name="coid">大团购产品的尾款SKU提交支付的时候会带过来</param>
        /// <returns></returns>
        public ActionResult CouponBook(int skuid, int paynum = 1, string userid = "0", int groupid = 0, string openid = "", int fromwxuid = 0, long coid = 0)
        {
            var isApp = IsApp();
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
            ViewBag.IsApp = isApp;
            ViewBag.SKUID = skuid;
            ViewBag.PayNum = paynum;
            ViewBag.GroupId = groupid;
            ViewBag.OpenId = openid;
            ViewBag.FromWxUid = fromwxuid;
            ViewBag.CouponOrderId = coid;

            var curUserID = UserState.UserID > 0 ? UserState.UserID : Convert.ToInt32(userid);
            if (curUserID <= 0)
            {
                curUserID = _ContextBasicInfo.AppUserID;
            }
            if (curUserID >= 0 && _ContextBasicInfo.AppUserID <= 0)
            {
                _ContextBasicInfo.AppUserID = curUserID;
            }
            ViewBag.UserId = curUserID;

            //获取请求参数中的CID
            var requestCID = GetCurCIDForRequest();
            ViewBag.RequestCID = requestCID;

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
            var couponSkuInfo = Coupon.GetSKUCouponActivityDetail(skuid, curUserID, coid);
            ViewBag.CouponSkuInfo = couponSkuInfo;

            //判断当前用户是否为VIP
            var isVip = account.IsVIPCustomer(new AccountInfoItem { Uid = curUserID.ToString() });
            ViewBag.IsVip = isVip;

            //isMobile
            var isMobile = Utils.IsMobile();
            ViewBag.IsMobile = isMobile;

            //获取当前用户信息
            var userInfo = account.GetUserInfoByUserID(curUserID);
            ViewBag.UserInfo = userInfo ?? new UserInfoResult();

            //检查当前用户的住基金
            var userFundInfo = new Fund().GetUserFundInfo(curUserID);
            ViewBag.UserFundInfo = userFundInfo;

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

            #region 行为记录

            //var value = string.Format("{{\"skuid\":\"{0}\",\"sid\":\"{1}\",\"userNo\":\"{2}\",\"title\":\"{3}\",\"CID\":\"{4}\",\"RequestCID\":\"{5}\",\"fromwxno\":\"{6}\"}}", skuid, sid, curUserID, (couponSkuInfo.activity != null && !string.IsNullOrEmpty(couponSkuInfo.activity.PageTitle) ? couponSkuInfo.activity.PageTitle : ""), GetCurCID(), GetCurCIDForRequest(), fromwxno);
            //RecordBehavior("CouponBooktLoad", value);

            #endregion

            return View();
        }

        #endregion

        #region 券预约相关

        /// <summary>
        /// 券预约
        /// </summary>
        /// <param name="skuid"></param>
        /// <param name="exid"></param>
        /// <param name="userid"></param>
        /// <param name="prereserve">是否提前预约</param>
        /// <param name="paynum">【prereserve时使用】下一步提交使用</param>
        /// <param name="fromwxuid">【prereserve时使用】下一步提交使用</param>
        /// <param name="groupid">【prereserve时使用】下一步提交使用</param>
        /// <param name="openid">【prereserve时使用】下一步提交使用</param>
        /// <param name="coid">【prereserve时使用】下一步提交使用</param>
        /// <returns></returns>
        public ActionResult CouponReserve(int skuid, int exid, int userid = 0, int prereserve = 0, int paynum = 0, int fromwxuid = 0, int groupid = 0, string openid = "", long coid = 0)
        {
            var isApp = IsApp();
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
            ViewBag.IsApp = isApp;

            //当前系统环境（ios | android）
            var appType = AppType();
            ViewBag.AppType = appType;

            ViewBag.UserId = userid;

            //获取当前用户信息
            var userInfo = account.GetUserInfoByUserID(userid);
            ViewBag.UserInfo = userInfo ?? new UserInfoResult();

            //单独拿出手机号使用
            var userPhone = "";
            if (userInfo != null && !string.IsNullOrEmpty(userInfo.Mobile))
            {
                userPhone = userInfo.Mobile;
            }
            ViewBag.UserPhone = userPhone;

            //获取券SKU信息
            var couponSkuInfo = Coupon.GetSKUCouponActivityDetail(skuid);
            ViewBag.CouponSkuInfo = couponSkuInfo;

            var packageEntity = new PackageInfoEntity();
            var calendar = new List<HJD.HotelServices.Contracts.PDayItem>();

            //当前套餐的可用日期
            calendar = Coupon.GetBookDateList(skuid);
            if (calendar != null && calendar.Count > 0)
            {
                calendar = calendar.Where(_ => _.Day >= DateTime.Now.Date).ToList();

                for (int dnum = 0; dnum < calendar.Count; dnum++)
                {
                    var ditem = calendar[dnum];
                    if (ditem.Day < DateTime.Now.Date)
                    {
                        ditem.SellState = 0;
                    }
                }
            }
            //var calendarEntity = HJDAPI.APIProxy.Price.GetOneHotelPackageCalendar(188660, DateTime.Now.Date, 5660, Convert.ToInt64(userid));

            var checkIn = DateTime.Now.AddDays(1).Date;
            var checkOut = checkIn.AddDays(1).Date;
            ViewBag.CheckIn = checkIn;
            ViewBag.CheckOut = checkOut;
            ViewBag.DayLimitMin = 1;
            ViewBag.DayLimitMax = 1;
            ViewBag.calendar = calendar;

            ViewBag.ExchangeId = exid;
            ViewBag.SKUID = skuid;
            ViewBag.PayNum = paynum;
            ViewBag.FromWxuid = fromwxuid;
            ViewBag.GroupId = groupid;
            ViewBag.OpenId = openid;
            ViewBag.CouponOrderId = coid;
            ViewBag.PreReserve = prereserve == 1;

            return View();
        }

        /// <summary>
        /// 房券兑换前检测
        /// </summary>
        /// <returns></returns>
        public ActionResult CheckCouponReserve()
        {
            Dictionary<string, string> back = new Dictionary<string, string>();

            SubmitExchangeRoomOrderParam sp = new SubmitExchangeRoomOrderParam();
            sp.contact = this.Request.Params["contact"].ToString();
            sp.contactPhone = this.Request.Params["contactPhone"].ToString();
            sp.exchangeNo = this.Request.Params["exchangeNo"].ToString();
            sp.checkIn = DateTime.Parse(this.Request.Params["checkIn"].ToString());
            sp.nightCount = Convert.ToInt32(this.Request.Params["nightCount"].ToString());
            sp.roomCount = Convert.ToInt32(this.Request.Params["roomCount"].ToString());
            sp.packageID = Convert.ToInt32(this.Request.Params["packageID"].ToString());
            sp.packageType = Convert.ToInt32(this.Request.Params["packageType"].ToString());
            sp.note = this.Request.Params["note"].ToString();
            sp.note = sp.note.Replace("\n\n", " ").Replace("\r\n", " ");
            sp.userID = Convert.ToInt32(this.Request.Params["userId"].ToString());
            sp.hotelID = Convert.ToInt32(this.Request.Params["hotelId"].ToString());
            sp.terminalID = Utils.GetTerminalId(Request.UserAgent);
            sp.channelID = Utils.GetChannelId(Request.UserAgent, Session["ChannelID"]);

            //check
            ExchangeRoomOrderConfirmResult result = new Coupon().IsExchangeNeedAddMoney(sp);
            back["Message"] = result.Message;
            back["Success"] = result.AddMoneyAmount < 0 ? ((int)result.AddMoneyAmount).ToString() : (result.IsNeedAddMoney ? "1" : "0");
            return Json(back, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 提交预约（暂时不用）
        /// </summary>
        /// <returns></returns>
        public ActionResult SubmitCouponReserve()
        {
            Dictionary<string, string> back = new Dictionary<string, string>();

            SubmitExchangeRoomOrderParam sp = new SubmitExchangeRoomOrderParam();
            sp.contact = this.Request.Params["contact"].ToString();
            sp.contactPhone = this.Request.Params["contactPhone"].ToString();
            sp.exchangeNo = this.Request.Params["exchangeNo"].ToString();
            sp.checkIn = DateTime.Parse(this.Request.Params["checkIn"].ToString());
            sp.nightCount = Convert.ToInt32(this.Request.Params["nightCount"].ToString());
            sp.roomCount = Convert.ToInt32(this.Request.Params["roomCount"].ToString());
            sp.packageID = Convert.ToInt32(this.Request.Params["packageID"].ToString());
            sp.packageType = Convert.ToInt32(this.Request.Params["packageType"].ToString());
            sp.note = this.Request.Params["note"].ToString();
            sp.note = sp.note.Replace("\n\n", " ").Replace("\r\n", " ");
            sp.userID = Convert.ToInt32(this.Request.Params["userId"].ToString());
            sp.hotelID = Convert.ToInt32(this.Request.Params["hotelId"].ToString());
            sp.terminalID = Utils.GetTerminalId(Request.UserAgent);
            sp.channelID = Utils.GetChannelId(Request.UserAgent, Session["ChannelID"]);

            //出行人
            var _travelPersonIdList = new List<int>();
            if (this.Request.Params["travelPersons"] != null && !string.IsNullOrEmpty(this.Request.Params["travelPersons"].ToString()))
            {
                try
                {
                    var _travelPersons = this.Request.Params["travelPersons"].ToString();
                    _travelPersonIdList = _travelPersons.Split(',').ToList().Select(_id => Convert.ToInt32(_id)).ToList();
                }
                catch (Exception ex)
                {
                    //return Content("出行人读取错误，请重试");
                }
            }
            sp.travelId = _travelPersonIdList;

            //submit
            SubmitExchangeRoomOrderResult result = Coupon.SubmitExchangeRoomOrder(sp);
            back["Message"] = result.Message;
            back["Success"] = result.Success.ToString();
            return Json(back, JsonRequestBehavior.AllowGet);
        }

        ///// <summary>
        ///// 提交预约（在用）
        ///// </summary>
        ///// <returns></returns>
        //public ActionResult SubmitBookInfo(int skuid, int exid, long userid, int bookdateid, int bookdetailid)
        //{
        //    Dictionary<string, string> back = new Dictionary<string, string>();

            


        //    back["Message"] = "";
        //    back["Success"] = "";
        //    return Json(back, JsonRequestBehavior.AllowGet);
        //}

        #endregion

        #region

        /// <summary>
        /// 节假日活动领券中心（如暑期大促领500现金券）
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="albumId"></param>
        /// <param name="grid"></param>
        /// <param name="cid"></param>
        /// <param name="districtId"></param>
        /// <param name="districtName"></param>
        /// <param name="geoScopeType"></param>
        /// <param name="tempId"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public ActionResult CouponActiveCenter(int userid = 0, string albumId = "0", int grid = 0, int cid = 0, int districtId = 0, string districtName = "", int geoScopeType = 1, int tempId = 3026, string code = "")
        {
            var isApp = IsApp();
            ViewBag.IsApp = isApp;

            //当前系统环境（ios | android）
            var appType = AppType();
            ViewBag.AppType = appType;

            //是否大于等于5.1版本
            var isThanVer5_1 = IsThanVer5_1();
            ViewBag.IsThanVer5_1 = isThanVer5_1;

            //手动赋值cid
            if (cid > 0)
            {
                System.Web.HttpContext.Current.Session["ChannelID"] = cid;
            }

            //Weixin
            bool isInWeixin = Request.UserAgent.IndexOf("MicroMessenger") > 0 ? true : false;
            ViewBag.isInWeixin = isInWeixin;

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

            #region 微信环境下，做微信静默授权

            WeixinUserInfo weixinUserInfo = new WeixinUserInfo();
            var openid = "";
            var unionid = "";
            var wxuid = 0;
            if (isInWeixin)
            {
                if (!string.IsNullOrEmpty(code))
                {
                    var weixinAcountName = "weixinservice_haoyi";

                    //通过code换取网页授权access_token
                    var accessToken = WeiXinHelper.GetWeixinAccessTokenForHaoYi(code);
                    if (accessToken != null && !string.IsNullOrEmpty(accessToken.Openid))
                    {
                        openid = accessToken.Openid;

                        //获取用户信息
                        //通过access_token拉取用户信息(需scope为 snsapi_userinfo)（这是通过微信api获取的真实用户信息）
                        weixinUserInfo = WeiXinHelper.GetWeixinUserInfo(accessToken.AccessToken, accessToken.Openid);
                        if (weixinUserInfo != null && !string.IsNullOrEmpty(weixinUserInfo.Openid))
                        {
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
                                    WeixinAcount = weixinAcountName,   //WeiXinChannelCode.周末酒店服务号_皓颐
                                    Language = "zh_CN",
                                    SubscribeTime = DateTime.Now,
                                    CreateTime = DateTime.Now
                                };
                                var update = new Weixin().UpdateWeixinUserSubscribe(w);

                                //get weixin user
                                var weixinUser = new Weixin().GetWeixinUser(openid);
                                if (weixinUser != null && weixinUser.ID > 0)
                                {
                                    wxuid = weixinUser.ID;
                                }
                            }
                            catch (Exception ex)
                            {

                            }

                            #region 处理微信号绑定用户信息相关操作

                            //微信环境下，如果当前已经登录，则直接绑定
                            if (curUserID > 0)
                            {
                                WeiXinHelper.BindingWxAndUid(curUserID, weixinUserInfo.Unionid);
                            }

                            #endregion

                            #region 更新Unionid缓存并更新CID信息

                            SetCurWXUnionid(weixinUserInfo.Unionid);
                            CheckCID();

                            #endregion
                        }
                    }
                    else
                    {
                        //如果code有值，但不能获取，一般说明code过期了，那么重新获取
                        var weixinGoUrl = WeiXinHelper.GenSilenceAuthorUrlForHaoYi(HttpUtility.UrlEncode(string.Format("http://www.zmjiudian.com/Coupon/ActiveCenter/{0}/{1}/{7}?albumid={2}&userid={3}&districtId={4}&districtName={5}&geoScopeType={6}", grid, cid, albumId, userid, districtId, districtName, geoScopeType, tempId)));
                        return Redirect(weixinGoUrl);
                    }
                }
                else
                {
                    //授权页面
                    var weixinGoUrl = WeiXinHelper.GenSilenceAuthorUrlForHaoYi(HttpUtility.UrlEncode(string.Format("http://www.zmjiudian.com/Coupon/ActiveCenter/{0}/{1}/{7}?albumid={2}&userid={3}&districtId={4}&districtName={5}&geoScopeType={6}", grid, cid, albumId, userid, districtId, districtName, geoScopeType, tempId)));
                    return Redirect(weixinGoUrl);
                }
            }

            #endregion

            ViewBag.Openid = openid;
            ViewBag.Unionid = unionid;
            ViewBag.WeixinUid = wxuid;

            //当前用户是否VIP
            var isVip = false; if (curUserID > 0) isVip = account.IsVIPCustomer(new AccountInfoItem { Uid = curUserID.ToString() });
            ViewBag.IsVip = isVip;

            ViewBag.TempId = tempId;
            ViewBag.AlbumId = albumId;
            ViewBag.DistrictId = districtId;
            ViewBag.DistrictName = districtName;
            ViewBag.GeoScopeType = geoScopeType;

            ViewBag.IsGridStyle = grid == 1;

            #region 行为记录

            var value = string.Format("{{\"albumId\":\"{0}\",\"userNo\":\"{1}\"}}", albumId, curUserID);
            RecordBehavior("CouponActiveCenterLoad", value);

            #endregion

            return View();
        }

        #endregion

        #region 扫码核销订单列表页

        /// <summary>
        /// 扫码核销，扫码打开的订单列表页
        /// </summary>
        /// <param name="supplierId">指定供应商ID</param>
        /// <param name="userId"></param>
        /// <param name="qrcodeId">二维码ID</param>
        /// <returns></returns>
        public ActionResult OrderListForSupplier(int supplierId = 0, long userId = 0, int qrcodeId = 0, string code = "")
        {
            var isApp = IsApp();
            ViewBag.IsApp = isApp;

            //isMobile
            var isMobile = Utils.IsMobile();
            ViewBag.IsMobile = isMobile;

            //Weixin
            bool isInWeixin = Request.UserAgent.IndexOf("MicroMessenger") > 0 ? true : false;
            ViewBag.isInWeixin = isInWeixin;

            var curUserID = Convert.ToInt64(UserState.UserID > 0 ? UserState.UserID : 0);
            if (curUserID <= 0)
            {
                curUserID = _ContextBasicInfo.AppUserID;
            }
            if (curUserID >= 0 && _ContextBasicInfo.AppUserID <= 0)
            {
                _ContextBasicInfo.AppUserID = curUserID;
            }
            ViewBag.UserId = curUserID;

            #region 微信环境下，做微信静默授权

            WeixinUserInfo weixinUserInfo = new WeixinUserInfo();
            if (isInWeixin)
            {
                if (!string.IsNullOrEmpty(code))
                {
                    var weixinAcountName = "weixinservice_haoyi";

                    //通过code换取网页授权access_token
                    var accessToken = WeiXinHelper.GetWeixinAccessTokenForHaoYi(code);
                    if (accessToken != null && !string.IsNullOrEmpty(accessToken.Openid))
                    {
                        //获取用户信息
                        //通过access_token拉取用户信息(需scope为 snsapi_userinfo)（这是通过微信api获取的真实用户信息）
                        weixinUserInfo = WeiXinHelper.GetWeixinUserInfo(accessToken.AccessToken, accessToken.Openid);
                        if (weixinUserInfo != null && !string.IsNullOrEmpty(weixinUserInfo.Openid))
                        {
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
                                    WeixinAcount = weixinAcountName,   //WeiXinChannelCode.周末酒店服务号_皓颐
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
                            if (curUserID > 0)
                            {
                                WeiXinHelper.BindingWxAndUid(curUserID, weixinUserInfo.Unionid);
                            }

                            #endregion

                            #region 更新Unionid缓存并更新CID信息

                            SetCurWXUnionid(weixinUserInfo.Unionid);
                            CheckCID();

                            #endregion
                        }
                    }
                    else
                    {
                        //如果code有值，但不能获取，一般说明code过期了，那么重新获取
                        var weixinGoUrl = WeiXinHelper.GenSilenceAuthorUrlForHaoYi(HttpUtility.UrlEncode(string.Format("http://www.zmjiudian.com/Coupon/OrderListForSupplier?supplierid={0}&qrcodeid={1}", supplierId, qrcodeId)));
                        return Redirect(weixinGoUrl);
                    }
                }
                else
                {
                    //授权页面
                    var weixinGoUrl = WeiXinHelper.GenSilenceAuthorUrlForHaoYi(HttpUtility.UrlEncode(string.Format("http://www.zmjiudian.com/Coupon/OrderListForSupplier?supplierid={0}&qrcodeid={1}", supplierId, qrcodeId)));
                    return Redirect(weixinGoUrl);
                }
            }
            ViewBag.Unionid = weixinUserInfo.Unionid;

            #endregion

            //供应商ID
            ViewBag.SupplierId = supplierId;

            return View();
        }

        #endregion

        #region 券特价列表页面

        /// <summary>
        /// 券特价页面（今日特价）
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="albumId">主产品list使用的券专辑ID</param>
        /// <param name="topid">头banner使用的券专辑ID</param>
        /// <param name="districtId">目的地ID</param>
        /// <param name="districtName">目的地名称</param>
        /// <param name="geoScopeType">是否周边</param>
        /// <param name="isgrid">是否矩阵布局</param>
        /// <param name="tagId">特色标签ID</param>
        /// <param name="tagName">特色标签名称</param>
        /// <returns></returns>
        public ActionResult CouponSales(int userid = 0, string albumId = "0", int topid = 0, int districtId = 0, string districtName = "", int geoScopeType = 1, int isgrid = 1, int tagId = 0, string tagName = "")
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

            var curUserID = UserState.UserID > 0 ? UserState.UserID : Convert.ToInt64(userid);
            if (curUserID <= 0)
            {
                curUserID = _ContextBasicInfo.AppUserID;
            }
            ViewBag.UserId = curUserID;

            //当前用户是否VIP
            var isVip = false; if (curUserID > 0) isVip = account.IsVIPCustomer(new AccountInfoItem { Uid = curUserID.ToString() });
            ViewBag.IsVip = isVip;

            ViewBag.AlbumId = albumId;
            ViewBag.TopId = topid;
            ViewBag.DistrictId = districtId;
            ViewBag.DistrictName = districtName;
            ViewBag.TagId = tagId;
            ViewBag.TagName = tagName;
            ViewBag.GeoScopeType = geoScopeType;
            ViewBag.IsGrid = isgrid == 1;

            return View();
        }


        #endregion

        /// <summary>
        /// 获取服务器时间
        /// </summary>
        /// <param name="timerFormat"></param>
        /// <returns></returns>
        public ActionResult GetNowtime(string timerFormat = "yyyy-MM-dd")
        {
            Dictionary<string, string> _back = new Dictionary<string, string>();

            var _now = DateTime.Now;
            _back["Time"] = _now.ToString(timerFormat);    //"yyyy-MM-dd HH:mm:ss"
            _back["Year"] = _now.Year.ToString();
            _back["Month"] = _now.Month.ToString();
            _back["Day"] = _now.Day.ToString();
            _back["Hour"] = _now.Hour.ToString();
            _back["Minute"] = _now.Minute.ToString();
            _back["Second"] = _now.Second.ToString();

            return Json(_back, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Detail()
        {
            string orderid = this.Request.QueryString["orderid"];
            long orderId = 0;
            orderId = long.TryParse(orderid, out orderId) ? orderId : 0;

            string guid = "";
            if (orderId != 0)
            {
                guid = Coupon.GetOriginGUID(orderId, 100);//测试环境 typeid暂用100 ToDo
            }
            else
            {
                guid = this.Request.QueryString["key"];
            }
            if (string.IsNullOrEmpty(guid))
            {
                return Json(new { Message = "没有guid值", Success = 1 }, JsonRequestBehavior.AllowGet);
            }
            string userAgent = Request.UserAgent.ToString().ToLower();
            ViewBag.isMicro = userAgent.Contains("MicroMessenger") ? true : false;

            string apptype = Request.Headers["apptype"] != null ? Request.Headers["apptype"] : "";
            //string appver = Request.Headers["appver"] != null ? Request.Headers["appver"] : "";

            if (apptype != "" || !string.IsNullOrEmpty((string)Session["apptype"]))
            {
                ViewBag.IsApp = true;
                //HTML = HTML.Replace("http://www.zmjiudian.com", "whotelapp://www.zmjiudian.com").Replace("tp=webp", "");
            }
            else
            {
                ViewBag.IsApp = false;
            }

            OriginCoupon coupon = Coupon.GetCashCoupon(0, guid);
            //ViewBag.Money = coupon.CashMoney; 2015-06-01 由于改发现金券 而非现金红包
            ViewBag.Money = Math.Round(coupon.TotalMoney / 10, 0);//取整 单位分
            //string totalMoney = Math.Round(coupon.TotalMoney / 100, 0).ToString();//嵌入文字中 需要转成元

            ViewBag.guid = guid;
            bool isProduct = bool.Parse(System.Configuration.ConfigurationManager.AppSettings["APIProxy_IsProductEvn"]);
            string currentDomin = WHotelSite.Common.Config.WebSiteUrl;
            string enviroment = isProduct ? "http://www.zmjiudian.com/" : currentDomin;
            string originShareUrl = string.Format("{0}Coupon/Grab?key={1}", enviroment, guid);
            originShareUrl = new Access().GenShortUrl(0, originShareUrl);
            ViewBag.originShareUrl = originShareUrl;
            string shareLink = Server.UrlEncode(originShareUrl);
            //ToDo 获得分享内容 本地new一个
            ActivityResult result = new ActivityResult() { title = @"这个大礼包不错，周末节假日出行订酒店又好又便宜！", Content = @"这个大礼包不错，周末节假日出行订酒店又好又便宜！", photoUrl = "http://whphoto.b0.upaiyun.com/116HOQY0XH_140X140", shareLink = shareLink };
            ViewBag.share = result;
            string originNextUrl = string.Format("{0}Coupon/ShareResult?key={1}", enviroment, guid);
            //ViewBag.originNextUrl = originNextUrl;
            ViewBag.nextUrl = Server.UrlEncode(originNextUrl);

            return View();
        }

        #region 获取微信js-SDK 使用注册信息

        /// <summary>
        /// 【周末酒店订阅号】获取微信公众号配置
        /// </summary>
        /// <returns></returns>
        public ActionResult GetWeixinConfigInfo()
        {
            //ToDo 传调用js-sdk页面url参数进来 得到使用注册信息
            string url = this.Request.Form["url"];
            if (!string.IsNullOrEmpty(url))
            {
                WeixinConfig config = new Weixin().GetWeixinConfig(url);
                return Json(config);
            }
            else
            {
                return Json(new { Message = "缺少页面url", Success = 1 });
            }
        }

        /// <summary>
        /// 【尚旅游订阅号】获取微信公众号配置
        /// </summary>
        /// <returns></returns>
        public ActionResult GetWeixinConfigInfoSly()
        {
            //ToDo 传调用js-sdk页面url参数进来 得到使用注册信息
            string url = this.Request.Form["url"];
            if (!string.IsNullOrEmpty(url))
            {
                WeixinConfig config = new Weixin().GetWeixinConfigSly(url);
                return Json(config);
            }
            else
            {
                return Json(new { Message = "缺少页面url", Success = 1 });
            }
        }

        /// <summary>
        /// 【尚旅游成都订阅号】获取微信公众号配置
        /// </summary>
        /// <returns></returns>
        public ActionResult GetWeixinConfigSlycd()
        {
            //ToDo 传调用js-sdk页面url参数进来 得到使用注册信息
            string url = this.Request.Form["url"];
            if (!string.IsNullOrEmpty(url))
            {
                WeixinConfig config = new Weixin().GetWeixinConfigSlycd(url);
                return Json(config);
            }
            else
            {
                return Json(new { Message = "缺少页面url", Success = 1 });
            }
        }

        /// <summary>
        /// 【美味至尚订阅号】获取微信公众号配置
        /// </summary>
        /// <returns></returns>
        public ActionResult GetWeixinConfigMwzs()
        {
            //ToDo 传调用js-sdk页面url参数进来 得到使用注册信息
            string url = this.Request.Form["url"];
            if (!string.IsNullOrEmpty(url))
            {
                WeixinConfig config = new Weixin().GetWeixinConfigMwzs(url);
                return Json(config);
            }
            else
            {
                return Json(new { Message = "缺少页面url", Success = 1 });
            }
        }

        /// <summary>
        /// 【尚旅游北京订阅号】获取微信公众号配置
        /// </summary>
        /// <returns></returns>
        public ActionResult GetWeixinConfigSlybj()
        {
            //ToDo 传调用js-sdk页面url参数进来 得到使用注册信息
            string url = this.Request.Form["url"];
            if (!string.IsNullOrEmpty(url))
            {
                WeixinConfig config = new Weixin().GetWeixinConfigSlycd(url);
                return Json(config);
            }
            else
            {
                return Json(new { Message = "缺少页面url", Success = 1 });
            }
        }

        #endregion

        //public ActionResult GetCash()
        //{
        //    //ToDo 更新现金红包表记录 为已领取
        //    string guid = this.Request.Form["key"];
        //    OriginCouponResult result = Coupon.UpdateCashCoupon20(0, guid, 1);
        //    return Json(result);
        //}

        public ActionResult Grab()
        {
            string guid = this.Request.QueryString["key"];
            ViewBag.guid = guid;

            if (!Coupon.IsAllAcquired(guid))
            {
                return View();
            }
            else
            {
                //List<AcquiredCoupon> list = Coupon.GetCurrentAcquiredCouponList(guid);
                //return View("Late",list);
                return View("Late");
            }
        }

        public ActionResult Result()
        {
            long phoneNum = 0;
            long.TryParse(this.Request.Form["phoneNum"], out phoneNum);
            string guid = this.Request.QueryString["key"];

            if (phoneNum == 0 || string.IsNullOrEmpty(guid))
            {
                return Json(new { Message = "网络发生异常，请重新提交", Success = 2 });
            }

            long userId = 0;
            bool isExists = Coupon.HasParticipateActivity(guid, phoneNum.ToString());
            if (!isExists)
            {
                AcquiredCoupon ac = Coupon.GetAcquiredCoupon(userId, guid, phoneNum.ToString());
                //此处没抢到钱 再次跳转入 来晚了\
                string redirectUrl = "";
                if (ac.ID == 0)
                {
                    //List<AcquiredCoupon> list = new List<AcquiredCoupon>();//new Coupon().GetCurrentAcquiredCouponList(guid);
                    //return View("Late", list);
                    redirectUrl = "/Coupon/Late";
                    LogHelper.WriteLog(string.Format("{0} {1} {2} 没抢到重定向", userId, phoneNum, guid));
                }
                else
                {
                    redirectUrl = "/Coupon/Result2?key=" + guid + "&cid=" + ac.ID.ToString();
                }
                return Json(new { url = redirectUrl, Success = 0 });
            }
            else
            {
                return Json(new { Message = "您已参加过活动，转发让更多朋友来参与吧", Success = 1 });
            }
        }

        public ActionResult Result2()
        {
            AppDownloadViewModel m = new AppDownloadViewModel();

            string agent = HttpContext.Request.UserAgent;
            string[] keywords = { "Android", "iPhone", "iPod", "iPad", "Windows Phone", "MQQBrowser" };

            // agent = "Mozilla/5.0 (iPhone; CPU iPhone OS 8_1_2 like Mac OS X) AppleWebKit/600.1.4 (KHTML, like Gecko) Mobile/12B440 MicroMessenger/6.1 NetType/WIFI";

            m.isInWeixin = agent.IndexOf("MicroMessenger") > 0;

            //排除 Windows 桌面系统            
            if (!agent.Contains("Windows NT") || (agent.Contains("Windows NT") && agent.Contains("compatible; MSIE 9.0;")))
            {
                //排除 苹果桌面系统                
                if (!agent.Contains("Windows NT") && !agent.Contains("Macintosh"))
                {
                    foreach (string item in keywords)
                    {
                        if (agent.Contains(item))
                        {
                            if (item.StartsWith("i"))
                            {
                                m.appDownloadURL = m.appleAppURL;
                                m.isIPhone = true;
                            }
                            else
                            {
                                m.appDownloadURL = m.androidAppURL;
                            }
                            break;
                        }
                    }
                }
            }
            else
            {
                m.appDownloadURL = m.androidAppURL;
            }

            long acquiredId = 0;
            long.TryParse(this.Request.QueryString["cid"], out acquiredId);

            string guid = this.Request.QueryString["key"];

            if (acquiredId != 0 && !string.IsNullOrEmpty(guid))
            {
                List<AcquiredCoupon> list = Coupon.GetCurrentAcquiredCouponList(guid);
                AcquiredCoupon ac = Coupon.GetAcquiredCouponById(acquiredId);
                if (list != null && list.Count != 0)
                {
                    ViewBag.CouponNum = ac.TotalMoney;
                    ViewBag.PhoneNo = ac.PhoneNo.ToString();
                    ViewBag.CouponList = list;
                }
                else
                {
                    ViewBag.CouponNum = 0;
                    ViewBag.PhoneNo = null;
                    ViewBag.CouponList = new List<AcquiredCoupon>();
                }
            }
            else
            {
                ViewBag.CouponNum = 0;
                ViewBag.PhoneNo = null;
                ViewBag.CouponList = new List<AcquiredCoupon>();
            }

            return View(m);
        }

        public ActionResult Download()
        {
            AppDownloadViewModel m = new AppDownloadViewModel();

            string agent = HttpContext.Request.UserAgent;
            string[] keywords = { "Android", "iPhone", "iPod", "iPad", "Windows Phone", "MQQBrowser" };

            // agent = "Mozilla/5.0 (iPhone; CPU iPhone OS 8_1_2 like Mac OS X) AppleWebKit/600.1.4 (KHTML, like Gecko) Mobile/12B440 MicroMessenger/6.1 NetType/WIFI";

            m.isInWeixin = agent.IndexOf("MicroMessenger") > 0;

            //排除 Windows 桌面系统            
            if (!agent.Contains("Windows NT") || (agent.Contains("Windows NT") && agent.Contains("compatible; MSIE 9.0;")))
            {
                //排除 苹果桌面系统                
                if (!agent.Contains("Windows NT") && !agent.Contains("Macintosh"))
                {
                    foreach (string item in keywords)
                    {
                        if (agent.Contains(item))
                        {
                            if (item.StartsWith("i"))
                            {
                                m.appDownloadURL = m.appleAppURL;
                                m.isIPhone = true;
                                // Response.Redirect(appleAppURL);
                            }
                            else
                            {
                                m.appDownloadURL = m.androidAppURL;
                                // Response.Redirect(androidAppURL);
                            }
                            break;
                        }
                    }
                }
            }
            else
            {
                m.appDownloadURL = m.androidAppURL;
                //  Response.Redirect("https://itunes.apple.com/cn/app/zhou-mo-jiu-dian/id763264901");
            }

            return View(m);
        }

        public ActionResult Rebate()
        {
            long orderid = 0;
            orderid = long.TryParse((string)this.Request.Form["orderid"], out orderid) ? orderid : 0;
            if (orderid != 0)
            {
                BaseResponse response = new Order().RequestOrderRebate(orderid, 1);//1 返现
                return Json(response);
            }
            else
            {
                return Json(new { Message = "缺少订单Id", Success = 1 });
            }
        }

        public ActionResult ShareResult()
        {
            string guid = this.Request.QueryString["key"];
            if (string.IsNullOrEmpty(guid))
            {
                return Json(new { Message = "没有guid值", Success = 1 }, JsonRequestBehavior.AllowGet);
            }
            OriginCouponResult result = Coupon.UpdateCashCoupon20(0, guid, 1);
            return View();
        }

        public ActionResult Late()
        {
            return View();
        }

        public ActionResult CashIntroduction()
        {
            return View();
        }

        public ActionResult CouponIntroduction()
        {
            return View();
        }

        public ActionResult DashList()
        {
            //url加密  优先设计 通过则继续；不通过返回验证不通过信息
            /*
            1.	URL中除签名部分外，所有的参数都补齐
            2.	取URL中除去“&sign={sign}”部分的URL做为签名算法中的RequestType
            3.	调用签名算法签名，用签名结果规则URL中的{sign} 
            */
            string url = this.Request.Url.AbsoluteUri;
            string url_Sign = this.Request.QueryString["Sign"];//url请求的sign
            long url_TimeStamp = 0;
            url_TimeStamp = long.TryParse(this.Request.QueryString["TimeStamp"], out url_TimeStamp) ? url_TimeStamp : 0;
            int sublength = url.LastIndexOf("&");
            string url_RequestType = sublength > 0 ? url.Substring(0, sublength) : url;
            int url_SourceID = 0;
            url_SourceID = int.TryParse(this.Request.QueryString["SourceID"], out url_SourceID) ? url_SourceID : 0;
            string sscode = HJDAPI.Common.Security.Signature.GenSignature(url_TimeStamp, url_SourceID, WHotelSite.Common.Config.MD5Key, url_RequestType);
            //bool isSignRight = HJDAPI.Common.Security.Signature.IsRightSignature(url_TimeStamp, url_SourceID, url_RequestType, url_Sign);

            string final = url_Sign.Replace(" ", "+");

            string userAgent = Request.UserAgent;
            string terminalName = Utils.GetTerminalName(userAgent, _ContextBasicInfo);
            if (!(sscode == final))
            {
                //LogHelper.WriteLog(url + "  验证失败  " + terminalName + " ");
                return Json(new { Success = 1, Message = "验证失败" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                //LogHelper.WriteLog(url + "  验证成功  " + terminalName + " ");
            }

            //this.Request.Form["userId"];//post
            //this.Request.QueryString["userId"];//get
            long userId = 0;
            long.TryParse(this.Request.QueryString["userId"], out userId);
            List<OriginCoupon> list = Coupon.GetCashCouponList(userId);//userID

            string apptype = Request.Headers["apptype"] != null ? Request.Headers["apptype"] : "";
            Session["apptype"] = apptype;
            //List<int> list = new List<int>();
            return View(list);
        }

        public ActionResult GetLocalUser()
        {
            string userid = "0";
            if (HttpContext.Session["USERID"] != null)
            {
                userid = HttpContext.Session["USERID"].ToString();
            }
            return Json(userid, JsonRequestBehavior.AllowGet);
        }

        //时间戳转为时间
        private DateTime StampToDateTime(long timeStamp)
        {
            DateTime dateTimeStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(timeStamp + "0000000");
            TimeSpan toNow = new TimeSpan(lTime);
            return dateTimeStart.Add(toNow);
        }

        /// <summary>
        /// 获取指定UserId的Magicall Url
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public string GetMagicallUrl(long userId)
        {
            var url = "";

            var magicallurl = string.Format("http://www.zmjiudian.com/MagiCall/MagiCallClient?userid={0}&realuserid=1", userId);
            magicallurl = HttpUtility.UrlEncode(magicallurl);

            url = string.Format("whotelapp://www.zmjiudian.com/gotopage?url={0}", magicallurl);

            return url;
        }

        public ActionResult ExchangeCouponSearchAndBook()
        {
            var curUserID = UserState.UserID;
            ViewBag.UserID = curUserID;
            var isApp = IsApp();
            ViewBag.IsApp = isApp;
            return View();
        }

        #region 分享发红包

        /// <summary>
        /// 领取红包页
        /// </summary>
        /// <returns></returns>
        public ActionResult RedCashCoupon(string key = "", string userid = "0", string code = "", int redRecordId = 0)
        {
            //Weixin
            bool isInWeixin = Request.UserAgent.IndexOf("MicroMessenger") > 0 ? true : false;
            ViewBag.isInWeixin = isInWeixin;

            var curUserID = UserState.UserID > 0 ? UserState.UserID.ToString() : userid;
            ViewBag.UserId = curUserID;

            if (!string.IsNullOrEmpty(key) && key.Contains("_"))
            {
                key = Regex.Replace(key, "_", "-");
            }
            ViewBag.Key = key;

            //分享使用的key
            var shareKey = Regex.Replace(key, "-", "_");

            //微信用户信息
            WeixinUserInfo weixinUserInfo = new WeixinUserInfo();

            //指定红包领取记录（如果有指定领取记录ID的话..）
            RedRecordEntity redRecordInfo = new RedRecordEntity();

            //如果有领取记录ID，则直接显示该领取信息
            if (redRecordId > 0)
            {
                redRecordInfo = Coupon.GetUserRedRecord(redRecordId);
            }
            else
            {
                #region 有code，说明微信已回调

                if (!string.IsNullOrEmpty(code))
                {
                    var weixinAcountName = "weixinservice_haoyi";

                    //通过code换取网页授权access_token
                    WeixinAccessToken accessToken = WeiXinHelper.GetWeixinAccessTokenForHaoYi(code);
                    //var accessToken = new WeixinAccessToken { Openid = "okg6-uBqijun__Frr_dJfV8m6Tjs" };

                    //通过access_token拉取用户信息(需scope为 snsapi_userinfo)（这是通过微信api获取的真实用户信息）
                    weixinUserInfo = WeiXinHelper.GetWeixinUserInfo(accessToken.AccessToken, accessToken.Openid);
                    if (weixinUserInfo != null && !string.IsNullOrEmpty(weixinUserInfo.Openid))
                    {
                        //存储微信用户
                        try
                        {
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
                                WeixinAcount = weixinAcountName,
                                Language = "zh_CN",
                                SubscribeTime = DateTime.Now,
                                CreateTime = DateTime.Now
                            };
                            var update = new Weixin().UpdateWeixinUserSubscribe(w);
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                    else
                    {
                        //如果code有值，但不能获取，一般说明code过期了，那么重新获取
                        var weixinGoUrl = WeiXinHelper.GenConfrimAuthorUrlByWeixinAcount(WeiXinChannelCode.周末酒店服务号_皓颐, HttpUtility.UrlEncode(string.Format("http://www.zmjiudian.com/coupon/redcashcoupon/{0}/0", shareKey)));
                        if (isInWeixin)
                        {
                            return Redirect(weixinGoUrl);
                        }
                    }
                }

                #endregion

                #region 没有code，需要微信授权回调

                else
                {
                    //没有授权的跳转至授权页面
                    var weixinGoUrl = WeiXinHelper.GenConfrimAuthorUrlByWeixinAcount(WeiXinChannelCode.周末酒店服务号_皓颐, HttpUtility.UrlEncode(string.Format("http://www.zmjiudian.com/coupon/redcashcoupon/{0}/0", shareKey)));
                    if (isInWeixin)
                    {
                        return Redirect(weixinGoUrl);
                    }
                }

                #endregion   
            }

            //都授权通过，才能走到这里，继续...
            ViewBag.WeixinUserInfo = weixinUserInfo;

            //微信授权code
            ViewBag.Code = code;

            //指定领取记录ID
            ViewBag.RedRecordId = redRecordId;

            //指定领取记录
            ViewBag.RedRecordInfo = redRecordInfo;

            #region 处理微信号绑定用户信息相关操作

            //微信环境下，如果当前已经登录，则直接绑定
            if (isInWeixin && Convert.ToInt64(curUserID) > 0)
            {
                //weixinUserInfo.Openid = "oHGzlw-sdix9G__-S4IzfTsYRqC8";
                WeiXinHelper.BindingWxAndUid(Convert.ToInt64(curUserID), weixinUserInfo.Unionid);
            }

            #endregion

            #region 更新Unionid缓存并更新CID信息

            SetCurWXUnionid(weixinUserInfo.Unionid);
            CheckCID();

            #endregion

            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult RedCashCouponChecked()
        {
            long phoneNum = 0;
            long.TryParse(this.Request.Form["phoneNum"], out phoneNum);
            string guid = this.Request.QueryString["key"];
            string openid = this.Request.QueryString["openid"];
            if (phoneNum == 0 || string.IsNullOrEmpty(guid))
            {
                return Json(new { Message = "网络发生异常，请重新提交", Success = 2 });
            }

            if (!string.IsNullOrEmpty(guid) && guid.Contains("_"))
            {
                guid = Regex.Replace(guid, "_", "-");
            }

            string redirectUrl = "";
            long userId = 0;
            RedRecordEntity redRecord = Coupon.GetRedRecordByGuidAndPhone(guid, phoneNum.ToString(), openid);
            if (redRecord != null)
            {
                //redirectUrl = "/Coupon/RedCashCouponResult?key=" + guid + "&cid=" + model.ID.ToString();
                return Json(new { url = redirectUrl, Success = 0 });
            }

            //redirectUrl = "/Coupon/RedCashCouponResult?key=" + guid + "&cid=" + model.ID.ToString();
            return Json(new { url = redirectUrl, Success = 0 });
        }

        /// <summary>
        /// 红包结果页
        /// </summary>
        /// <param name="key"></param>
        /// <param name="cid"></param>
        /// <returns></returns>
        public ActionResult RedCashCouponResult(string key = "", long cid = 0)
        {
            if (!string.IsNullOrEmpty(key) && key.Contains("_"))
            {
                key = Regex.Replace(key, "_", "-");
            }

            List<AcquiredCoupon> list = new List<AcquiredCoupon>();
            AcquiredCoupon ac = new AcquiredCoupon();
            int count = 0;
            List<AcquiredCoupon> allList = Coupon.GetAcquiredCouponList(key);
            if (allList.Count > 0)
            {
                RedShareEntity redModel = Coupon.GetRedShareEntityByGUID(key).First();
                ViewBag.RedShare = redModel;
                list = Coupon.GetCurrentAcquiredCouponList(key);
                count = list.Count;

                ac = Coupon.GetAcquiredCouponById(cid);
                if (!string.IsNullOrEmpty(ac.PhoneNo))
                {
                    ac.PhoneNo = ac.PhoneNo.Remove(3, 4).Insert(3, "****");
                }
                if (allList.Count == count)
                {
                    ViewBag.MaxAmountEntity = allList.OrderByDescending(_ => _.TotalMoney).First();
                }
            }
            ViewBag.AC = ac;
            ViewBag.GUID = key;
            ViewBag.CID = cid;
            //ViewBag.All = all;
            ViewBag.Count = count;
            ViewBag.AllCount = allList.Count;
            return View(list);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult RedNonExistent()
        {
            return View();
        }

        /// <summary>
        /// 领取红包集合活动页面
        /// </summary>
        /// <returns></returns>
        public ActionResult RedCashCouponList(long userId = 0, int tempdataid = 3287)
        {
            var curUserID = Convert.ToInt64(UserState.UserID > 0 ? UserState.UserID : userId);
            if (curUserID <= 0)
            {
                curUserID = _ContextBasicInfo.AppUserID;
            }
            if (curUserID >= 0 && _ContextBasicInfo.AppUserID <= 0)
            {
                _ContextBasicInfo.AppUserID = curUserID;
            }
            ViewBag.UserId = curUserID;

            var isApp = IsApp();
            ViewBag.IsApp = isApp;

            //Weixin
            bool isInWeixin = Request.UserAgent.IndexOf("MicroMessenger") > 0 ? true : false;
            ViewBag.IsInWeixin = isInWeixin;

            ViewBag.TempDataId = tempdataid;

            return View();
        }

        #endregion
    }
}