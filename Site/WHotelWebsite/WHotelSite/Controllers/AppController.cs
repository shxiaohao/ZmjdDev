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
using WHotelSite.Models;
using HJD.CommentService.Contract;
using HJDAPI.Common.Security;

namespace WHotelSite.Controllers
{
    public class AppController : BaseController
    {
        public const string AccessProtocal_IsApp = "whotelapp://www.zmjiudian.com/";
        public const string AccessProtocalPage_IsApp = "whotelapp://gotopage?url=http://www.zmjiudian.com/";

        public const string AccessProtocal_UnApp = "/inspector/jump?jumpurl=whotelapp://www.zmjiudian.com/";  //"http://www.zmjiudian.com/hotel/";  //"http://m1.zmjiudian.com/";
        public const string AccessProtocalPage_UnApp = "/inspector/jump?jumpurl=whotelapp://www.zmjiudian.com/gotopage?url=http://www.zmjiudian.com/";
        //public const string AccessProtocalPage_UnApp = "/inspector/jump?jumpurl=whotelapp://www.zmjiudian.com/gotopage?url=http://192.168.1.22:8081/";

        #region 首页

        //首页 old
        public ActionResult Home2(int userid = 0, float userlat = 0, float userlng = 0, int v = 1)
        {
            if (v == 1)
            {
                return Redirect(string.Format("/app/home?userid={0}&userlat={1}&userlng={2}", userid, userlat, userlng));
            }

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

            #region 读取首页腰部banner

            var adInfo = new HomeDataModel20();
            var adList = new List<ADItem>();
            try
            {
                adInfo = new Hotel().GetUserHomeData20(userid);
                if (adInfo != null && adInfo.AD != null)
                {
                    adList = adInfo.AD.ADList;
                }
            }
            catch (Exception ex)
            {
                
            }
            ViewBag.AdInfo = adInfo;
            ViewBag.AdList = adList;

            //获取首页弹窗配置
            var adInfo2 = new Hotel().GetHomeBoxData(userid);
            ViewBag.BoxData = adInfo2.BoxData;

            #endregion

            #region 首页头部banner

            var selectedResort = new Advertise();
            if (adInfo != null && adInfo.SelectedResort != null)
            {
                selectedResort = adInfo.SelectedResort;   
            }
            ViewBag.SelectedList = selectedResort.ADList;

            #endregion

            #region 读取首页推荐点评

            var rcmParm = new RecommendCommentListQueryParam
            {
                start = 0,
                count = 4
            };

            RecommendCommentListModel recmComments = Comment.GetRecommendCommentListModel(rcmParm);
            ViewBag.RecmComments = recmComments;

            #endregion

            #region 读取首页闪购&亲子团

            HomePageData30 hotResult = new Hotel().GetAppHomePageData(userid);
            ViewBag.HotResult = hotResult;

            #endregion

            #region 读取首页精选

            RecommendHotelResult recmHotelResult = new Hotel().GetRecommendHotelResult(Convert.ToInt64(userid));
            ViewBag.RecmHotelResult = recmHotelResult;

            #endregion

            ViewBag.UserId = userid;
            return View();
        }

        //首页 new
        public ActionResult Home(int userid = 0, float userlat = 0, float userlng = 0)
        {
            var isApp = IsApp();
            if (isApp)
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

            //Weixin
            bool isInWeixin = Request.UserAgent.IndexOf("MicroMessenger") > 0 ? true : false;
            ViewBag.isInWeixin = isInWeixin;

            //检测是否大于等于4.6版本（4.6之后app支持h5跳转至原生预订页面）
            var isThanVer46 = IsThanVer4_6();
            ViewBag.IsThanVer46 = isThanVer46;

            //是否大于等于4.7
            var isThanVer47 = IsThanVer4_7();
            ViewBag.IsThanVer47 = isThanVer47;

            var isMobile = Utils.IsMobile();
            ViewBag.IsMobile = isMobile;

            ViewBag.UserId = userid;
            ViewBag.Userlat = userlat;
            ViewBag.Userlng = userlng;

            //当前用户是否VIP
            var isVip = false; if (userid > 0) isVip = account.IsVIPCustomer(new AccountInfoItem { Uid = userid.ToString() });
            ViewBag.IsVip = isVip;

            //获取首页弹窗配置
            var adInfo = new Hotel().GetHomeBoxData(userid);
            ViewBag.BoxData = adInfo.BoxData;

            return View();
        }

        /// <summary>
        /// 首页头（目的地轮播）
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public ActionResult HomeTop(int userid = 0)
        {
            var isApp = IsApp();
            ViewBag.IsApp = isApp;

            //Weixin
            bool isInWeixin = Request.UserAgent.IndexOf("MicroMessenger") > 0 ? true : false;
            ViewBag.isInWeixin = isInWeixin;

            //当前系统环境（ios | android）
            var appType = AppType();
            ViewBag.AppType = appType;

            //检测是否大于等于4.6版本（4.6之后app支持h5跳转至原生预订页面）
            var isThanVer46 = IsThanVer4_6();
            ViewBag.IsThanVer46 = isThanVer46;

            ViewBag.UserId = userid;

            #region 读取首页腰部banner

            var adInfo = new HomeDataModel20();
            var adList = new List<ADItem>();
            try
            {
                adInfo = new Hotel().GetUserHomeData20(userid);
                if (adInfo != null && adInfo.AD != null)
                {
                    adList = adInfo.AD.ADList;
                }
            }
            catch (Exception ex)
            {

            }
            ViewBag.AdInfo = adInfo;

            //微信过滤掉“推荐朋友”banner
            if (!isApp)
            {
                adList = adList.Where(a => !a.ActionURL.ToLower().Contains("magicall")
                    && !a.ActionURL.ToLower().Contains("magicallclient")
                    && !a.ActionURL.ToLower().Contains("fund")
                    && !a.ActionURL.ToLower().Contains("userrecommend")).ToList();
            }
            ViewBag.AdList = adList;

            #endregion

            #region 首页头部banner

            var selectedResort = new Advertise();
            if (adInfo != null && adInfo.SelectedResort != null)
            {
                selectedResort = adInfo.SelectedResort;
            }

            //对android做一个特殊处理，如果是android，则图片使用WebP格式
            if (appType == "android" && selectedResort.ADList != null && selectedResort.ADList.Count > 0)
            {
                foreach (var aditem in selectedResort.ADList)
                {
                    if (!string.IsNullOrEmpty(aditem.ADURL) && (aditem.ADURL.Contains("upaiyun") || aditem.ADURL.Contains("p1.zmjiudian")))
                    {
                        aditem.ADURL = aditem.ADURL + "/format/webp";
                    }
                }
            }

            ViewBag.SelectedList = selectedResort.ADList;

            #endregion

            return View();
        }

        /// <summary>
        /// 最近搜索目的地
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public ActionResult HomeDistrictSearchHistory(int userid = 0)
        {
            var isApp = IsApp();
            ViewBag.IsApp = isApp;

            //当前系统环境（ios | android）
            var appType = AppType();
            ViewBag.AppType = appType;

            ViewBag.UserId = userid;

            var s = 0;
            var c = 1;

            var bp = new CommonRecordQueryParam { userId = userid, start = s, count = c, businessType = 0 };//仅显示首页搜索的地区 businessType等于3;仅显示发现页搜索的地区 businessType等于4;全部显示 businessType等于0;
            var result = Hotel.GetSearchRecordList(bp);
            foreach (var item in result.HotelList)
            {
                item.HotelPicUrl = item.HotelPicUrl.Replace("_theme", "_640x360");
                if (appType == "android" && (item.HotelPicUrl.Contains("upaiyun") || item.HotelPicUrl.Contains("p1.zmjiudian")))
                {
                    item.HotelPicUrl = item.HotelPicUrl + "/format/webp";
                }
            }

            return View(result);
        }

        /// <summary>
        /// 首页专辑套餐通用页面
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="albumId">专辑ID</param>
        /// <returns></returns>
        public ActionResult HomeAlbumPackage(int userid = 0, int albumId = 0)
        {
            var isApp = IsApp();
            ViewBag.IsApp = isApp;

            //当前系统环境（ios | android）
            var appType = AppType();
            ViewBag.AppType = appType;

            ViewBag.UserId = userid;

            //专辑ID
            ViewBag.AlbumId = albumId;

            var s = 0;
            var c = 6;

            RecommendHotelResult result = Hotel.GetRecommendHotelResultByAlbumId(albumId, s, c, (long)userid);
            foreach (var item in result.HotelList)
            {
                item.HotelPicUrl = item.HotelPicUrl.Replace("_theme", "_640x360");
                if (appType == "android" && (item.HotelPicUrl.Contains("upaiyun") || item.HotelPicUrl.Contains("p1.zmjiudian")))
                {
                    item.HotelPicUrl = item.HotelPicUrl + "/format/webp";
                }
            }

            return View(result);
        }

        /// <summary>
        /// 首页特惠套餐（4.7在非登录状态下显示，4.7之前默认显示）
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public ActionResult HomeHotelPackageList(int userid = 0)
        {
            var isApp = IsApp();
            ViewBag.IsApp = isApp;

            //当前系统环境（ios | android）
            var appType = AppType();
            ViewBag.AppType = appType;

            ViewBag.UserId = userid;

            RecommendHotelResult recmHotelResult = new Hotel().GetRecommendHotelResult(Convert.ToInt64(userid));
            foreach (var item in recmHotelResult.HotelList)
            {
                item.HotelPicUrl = item.HotelPicUrl.Replace("_theme", "_640x360");
                if (appType == "android" && (item.HotelPicUrl.Contains("upaiyun") || item.HotelPicUrl.Contains("p1.zmjiudian")))
                {
                    item.HotelPicUrl = item.HotelPicUrl + "/format/webp";
                }
            }

            return View(recmHotelResult);
        }

        /// <summary>
        /// 限时闪购
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public ActionResult HomeFlashDeals(int userid = 0)
        {
            var isApp = IsApp();
            ViewBag.IsApp = isApp;

            //当前系统环境（ios | android）
            var appType = AppType();
            ViewBag.AppType = appType;

            ViewBag.UserId = userid;

            HomePageData30 hotResult = new Hotel().GetAppHomeFlashData(userid);
            foreach (var item in hotResult.FlashDeals)
            {
                item.hotelPicUrl = item.hotelPicUrl.Replace("_theme", "_640x360");
                if (appType == "android" && (item.hotelPicUrl.Contains("upaiyun") || item.hotelPicUrl.Contains("p1.zmjiudian")))
                {
                    item.hotelPicUrl = item.hotelPicUrl + "/format/webp";
                }
            }

            return View(hotResult);
        }

        /// <summary>
        /// 团购产品列表/酒+机套餐
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public ActionResult HomeGroupDeals(int userid = 0)
        {
            var isApp = IsApp();
            ViewBag.IsApp = isApp;

            //当前系统环境（ios | android）
            var appType = AppType();
            ViewBag.AppType = appType;

            ViewBag.UserId = userid;

            HomePageData30 hotResult = new Hotel().GetAppHomeGroupData(userid);
            foreach (var item in hotResult.GroupDeals)
            {
                item.hotelPicUrl = item.hotelPicUrl.Replace("_theme", "_640x360");
                if (appType == "android" && (item.hotelPicUrl.Contains("upaiyun") || item.hotelPicUrl.Contains("p1.zmjiudian")))
                {
                    item.hotelPicUrl = item.hotelPicUrl + "/format/webp";
                }
            }

            return View(hotResult);
        }

        /// <summary>
        /// 最近浏览的酒店
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public ActionResult HomeRecentSeeHotels(int userid = 0)
        {
            var isApp = IsApp();
            ViewBag.IsApp = isApp;

            //当前系统环境（ios | android）
            var appType = AppType();
            ViewBag.AppType = appType;

            ViewBag.UserId = userid;

            var s = 0;
            var c = 6;

            BsicSearchParam bp = new BsicSearchParam { userId = userid, start = s, count = c };
            RecommendHotelResult result = Hotel.GetHotelBrowsingRecordList(bp);
            if (result != null)
            {
                foreach (var item in result.HotelList)
                {
                    item.HotelPicUrl = item.HotelPicUrl.Replace("_theme", "_640x360").Replace("_290x290", "_640x360");
                    if (appType == "android" && (item.HotelPicUrl.Contains("upaiyun") || item.HotelPicUrl.Contains("p1.zmjiudian")))
                    {
                        item.HotelPicUrl = item.HotelPicUrl + "/format/webp";
                    }
                }
            }

            return View(result);
        }

        /// <summary>
        /// 朋友推荐的酒店
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public ActionResult HomeFriendRecHotels(int userid = 0)
        {
            var isApp = IsApp();
            ViewBag.IsApp = isApp;

            //当前系统环境（ios | android）
            var appType = AppType();
            ViewBag.AppType = appType;

            ViewBag.UserId = userid;

            var s = 0;
            var c = 6;

            RecommendCommentParam rp = new RecommendCommentParam { userId = userid, start = s, count = c };
            RecommendHotelResult result = Hotel.GetRecommendHotelListByFollowing(rp);
            if (result != null)
            {
                foreach (var item in result.HotelList)
                {
                    item.HotelPicUrl = item.HotelPicUrl.Replace("_theme", "_640x360").Replace("_290x290", "_640x360");
                    if (appType == "android" && (item.HotelPicUrl.Contains("upaiyun") || item.HotelPicUrl.Contains("p1.zmjiudian")))
                    {
                        item.HotelPicUrl = item.HotelPicUrl + "/format/webp";
                    }

                    item.AvatarUrl = item.AvatarUrl.Replace("_jupiter", "_120x120");
                    if (appType == "android" && (item.AvatarUrl.Contains("upaiyun") || item.AvatarUrl.Contains("p1.zmjiudian")))
                    {
                        item.AvatarUrl = item.AvatarUrl + "/format/webp";
                    }
                }   
            }

            return View(result);
        }


        /// <summary>
        /// 全部网友推荐(旧版本使用)
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public ActionResult RecComments(int userid = 0, int s = 0, int c = 8)
        {
            var isApp = IsApp();
            if (isApp)
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

            #region 读取首页推荐点评

            var rcmParm = new RecommendCommentListQueryParam
            {
                start = s,
                count = c
            };

            RecommendCommentListModel recmComments = Comment.GetRecommendCommentListModel(rcmParm);
            ViewBag.RecmComments = recmComments;

            #endregion

            ViewBag.UserId = userid;
            return View();
        }

        public ActionResult LoadRecComments(int userid = 0, int s = 0, int c = 8)
        {
            var rcmParm = new RecommendCommentListQueryParam
            {
                start = s,
                count = c
            };

            var comments = new List<RecommendCommentModel>();

            try
            {
                RecommendCommentListModel recmComments = Comment.GetRecommendCommentListModel(rcmParm);
                comments = recmComments.CommentList ?? new List<RecommendCommentModel>();
            }
            catch (Exception ex)
            {

            }

            return Json(comments, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 关于周末酒店(In App)
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="appname"></param>
        /// <returns></returns>
        public ActionResult AboutApp(int userid = 0, string appname = "周末酒店")
        {
            var isApp = IsApp();
            if (isApp)
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

            //Weixin
            bool isInWeixin = Request.UserAgent.IndexOf("MicroMessenger") > 0 ? true : false;
            ViewBag.isInWeixin = isInWeixin;

            //当前APP版本号（如4.x）
            var appVerCode = AppVersionCode();
            ViewBag.AppVerCode = appVerCode;

            //APP Bundle Ver
            ViewBag.AppBundleVer = _ContextBasicInfo.AppBundleVer;

            ViewBag.AppName = HttpUtility.UrlDecode(appname);

            var isMobile = Utils.IsMobile();
            ViewBag.IsMobile = isMobile;

            ViewBag.UserId = userid;

            return View();
        }

        #endregion

        #region 首页-更多-统一页面

        /// <summary>
        /// 更多套餐列表
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="albumId">1其他特惠套餐 10VIP精选 12机酒 21元旦VIP专场</param>
        /// <param name="t">1限时闪购 2酒+机套餐 3最近浏览酒店 4朋友推荐酒店 5本周特惠 6周末好去处</param>
        /// <param name="userlat"></param>
        /// <param name="userlng"></param>
        /// <param name="usercity"></param>
        /// <param name="shownewvip">是否显示新VIP专享头</param>
        /// <param name="areas">套餐的城市区域（如机+酒套餐会被区分成国内/东南亚/欧美澳等tabs）</param>
        /// <param name="airhoteldate"></param>
        /// <param name="startdid"></param>
        /// <param name="stopdid"></param>
        /// <param name="grid">是否grid方式展示（默认0）</param>
        /// <returns></returns>
        public ActionResult MorePackageList(int userid = 0, int albumId = 0, int t = 0, float userlat = 31.2303925f, float userlng = 121.4737f, string usercity = "上海", int shownewvip = 0, string areas = "", string airhoteldate = "", int startdid = 0, int stopdid = 0, int grid = 0)
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
            ViewBag.TypeNum = t;
            ViewBag.Userlat = userlat;
            ViewBag.Userlng = userlng;
            ViewBag.UserCity = usercity;
            ViewBag.ShowNewVip = shownewvip == 1;
            ViewBag.Areas = areas;
            ViewBag.AirHotelDate = airhoteldate;
            ViewBag.StartDistrictId = startdid;
            ViewBag.StopDistrictId = stopdid;
            ViewBag.Grid = grid;
            ViewBag.IsGridStyle = grid == 1;

            #region 行为记录

            var value = string.Format("{{\"albumId\":\"{0}\",\"userNo\":\"{1}\",\"userlat\":\"{2}\",\"userlng\":\"{3}\",\"usercity\":\"{4}\",\"shownewvip\":\"{5}\",\"areas\":\"{6}\",\"t\":\"{7}\"}}",
                albumId, userid, userlat, userlng, usercity, shownewvip, areas, t);
            RecordBehavior("MorePackageListLoad", value);

            #endregion

            return View();
        }

        /// <summary>
        /// 专辑套餐（Item）
        /// </summary>
        /// <returns></returns>
        public ActionResult More_AlbumPackages(int userid = 0, int s = 0, int c = 6, int albumId = 0, float userlat = 0, float userlng = 0, int geoScopeType = 0, int districtid = 0, string airhoteldate = "", int startdid = 0, int stopdid = 0, int grid = 0)
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

            ViewBag.IsGridStyle = grid == 1;

            RecommendHotelResult result = new RecommendHotelResult();
            
            //VIP专享套餐专用接口
            if (isApp && isThanVer5_1 && albumId == 10 && userlat >= 0 && userlng >= 0)
            {
                result = new Hotel().GetRecommendHotelResultByAlbumIdAddSearch(albumId, s, c, (long)userid, userlat, userlng, geoScopeType, districtid, 0);
            }
            else
            {
                result = Hotel.GetRecommendHotelResultByAlbumId(albumId, s, c, (long)userid, 0, dateStr: airhoteldate, startDistrictID: startdid, gotoDistrictID: stopdid);    
            }

            if (result != null && result.HotelList != null)
            {
                foreach (var item in result.HotelList)
                {
                    item.HotelPicUrl = item.HotelPicUrl.Replace("_theme", "_640x426");
                    if (appType == "android" && (item.HotelPicUrl.Contains("upaiyun") || item.HotelPicUrl.Contains("p1.zmjiudian")))
                    {
                        item.HotelPicUrl = item.HotelPicUrl + "/format/webp";
                    }
                }   
            }

            return View(result);
        }

        /// <summary>
        /// 限时闪购（Item）
        /// </summary>
        public ActionResult More_Flash(int userid = 0, int s = 0, int c = 6)
        {
            var isApp = IsApp();
            ViewBag.IsApp = isApp;

            //当前系统环境（ios | android）
            var appType = AppType();
            ViewBag.AppType = appType;

            ViewBag.UserId = userid;

            HomePageData30 hotResult = new HomePageData30();

            //目前闪购不做分页，所以只在第一页读取数据
            if (s == 0)
            {
                hotResult = new Hotel().GetAppHomeFlashData(userid);
                foreach (var item in hotResult.FlashDeals)
                {
                    item.hotelPicUrl = item.hotelPicUrl.Replace("_theme", "_640x426");
                    if (appType == "android" && (item.hotelPicUrl.Contains("upaiyun") || item.hotelPicUrl.Contains("p1.zmjiudian")))
                    {
                        item.hotelPicUrl = item.hotelPicUrl + "/format/webp";
                    }
                }
            }

            return View(hotResult);
        }

        /// <summary>
        /// 酒+机套餐（Item）
        /// </summary>
        public ActionResult More_Group(int userid = 0, int s = 0, int c = 6)
        {
            var isApp = IsApp();
            ViewBag.IsApp = isApp;

            //当前系统环境（ios | android）
            var appType = AppType();
            ViewBag.AppType = appType;

            ViewBag.UserId = userid;

            HomePageData30 hotResult = new HomePageData30();

            //目前酒机不做分页，所以只在第一页读取数据
            if (s == 0)
            {
                hotResult = new Hotel().GetAppHomeGroupData(userid);
                foreach (var item in hotResult.GroupDeals)
                {
                    item.hotelPicUrl = item.hotelPicUrl.Replace("_theme", "_640x426");
                    if (appType == "android" && (item.hotelPicUrl.Contains("upaiyun") || item.hotelPicUrl.Contains("p1.zmjiudian")))
                    {
                        item.hotelPicUrl = item.hotelPicUrl + "/format/webp";
                    }
                }
            }

            return View(hotResult);
        }

        /// <summary>
        /// 最近浏览的酒店
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="s"></param>
        /// <param name="c"></param>
        /// <param name="t">数据类型 3最近浏览酒店 6周末好去处</param>
        /// <returns></returns>
        public ActionResult More_RecentSee(int userid = 0, int s = 0, int c = 6, int t = 3, float userlat = 0, float userlng = 0)
        {
            var isApp = IsApp();
            ViewBag.IsApp = isApp;

            //当前系统环境（ios | android）
            var appType = AppType();
            ViewBag.AppType = appType;
            ViewBag.UserId = userid;

            BrowsingRecordResult result = null;
            switch (t)
            {
                case 3:
                    {
                        BsicSearchParam bp = new BsicSearchParam { userId = userid, start = s, count = c };
                        result = hoteltheme.GetBrowsingRecordList(bp);

                        break;
                    }
            }

            if (result != null)
            {
                foreach (var item in result.BorwsRecordList)
                {
                    item.BrowRecordPicUrl = item.BrowRecordPicUrl.Replace("_theme", "_640x426").Replace("_290x290", "_640x426");
                    if (appType == "android" && (item.BrowRecordPicUrl.Contains("upaiyun") || item.BrowRecordPicUrl.Contains("p1.zmjiudian")))
                    {
                        item.BrowRecordPicUrl = item.BrowRecordPicUrl + "/format/webp";
                    }
                }
            }
            #region 老版本 浏览记录

            //RecommendHotelResult result = null;
            //switch (t)
            //{
            //    case 3: 
            //        {
            //            BsicSearchParam bp = new BsicSearchParam { userId = userid, start = s, count = c };
            //            result = Hotel.GetHotelBrowsingRecordList(bp);

            //            //new hoteltheme().

            //            break; 
            //        }
            //    case 6:
            //        {
            //            result = Hotel.GetHotelWithInDistance(c, s, userlat, userlng);
            //            break;
            //        }
            //}

            //if (result != null)
            //{
            //    foreach (var item in result.HotelList)
            //    {
            //        item.HotelPicUrl = item.HotelPicUrl.Replace("_theme", "_640x426").Replace("_290x290", "_640x426");
            //        if (appType == "android" && (item.HotelPicUrl.Contains("upaiyun") || item.HotelPicUrl.Contains("p1.zmjiudian")))
            //        {
            //            item.HotelPicUrl = item.HotelPicUrl + "/format/webp";
            //        }

            //        if (!string.IsNullOrEmpty(item.AvatarUrl))
            //        {
            //            item.AvatarUrl = item.AvatarUrl.Replace("_jupiter", "_120x120");
            //            if (appType == "android" && (item.AvatarUrl.Contains("upaiyun") || item.AvatarUrl.Contains("p1.zmjiudian")))
            //            {
            //                item.AvatarUrl = item.AvatarUrl + "/format/webp";
            //            }   
            //        }
            //    }
            //} 
            #endregion

            return View(result);
        }

        /// <summary>
        /// 朋友推荐的酒店
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="s"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public ActionResult More_FriendRec(int userid = 0, int s = 0, int c = 6)
        {
            var isApp = IsApp();
            ViewBag.IsApp = isApp;

            //当前系统环境（ios | android）
            var appType = AppType();
            ViewBag.AppType = appType;

            ViewBag.UserId = userid;

            RecommendCommentParam rp = new RecommendCommentParam { userId = userid, start = s, count = c };
            RecommendHotelResult result = Hotel.GetRecommendHotelListByFollowing(rp);
            if (result != null)
            {
                foreach (var item in result.HotelList)
                {
                    item.HotelPicUrl = item.HotelPicUrl.Replace("_theme", "_640x426").Replace("_290x290", "_640x426");
                    if (appType == "android" && (item.HotelPicUrl.Contains("upaiyun") || item.HotelPicUrl.Contains("p1.zmjiudian")))
                    {
                        item.HotelPicUrl = item.HotelPicUrl + "/format/webp";
                    }

                    item.AvatarUrl = item.AvatarUrl.Replace("_jupiter", "_120x120");
                    if (appType == "android" && (item.AvatarUrl.Contains("upaiyun") || item.AvatarUrl.Contains("p1.zmjiudian")))
                    {
                        item.AvatarUrl = item.AvatarUrl + "/format/webp";
                    }
                }
            }

            return View(result);
        }


        #endregion

        #region 城市搜索模块

        /// <summary>
        /// 城市模块
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public ActionResult CityMenu(float userlat = 0, float userlng = 0)
        {
            var isApp = IsApp();
            ViewBag.IsApp = isApp;

            //当前系统环境（ios | android）
            ViewBag.AppType = AppType();

            //Weixin
            bool isInWeixin = Request.UserAgent.IndexOf("MicroMessenger") > 0 ? true : false;
            ViewBag.isInWeixin = isInWeixin;

            #region 获取城市列表

            var cityListInfo = Hotel.GetZMJDAllCityData();

            //继承于 CityList
            var cityListEntity = new CityListEntity();
            cityListEntity.Citys = cityListInfo.Citys;
            cityListEntity.HMTCitys = cityListInfo.HMTCitys;
            cityListEntity.HotArea = cityListInfo.HotArea;
            cityListEntity.HotOverseaArea = cityListInfo.HotOverseaArea;
            cityListEntity.SouthEastAsiaCitys = cityListInfo.SouthEastAsiaCitys;

            //对城市列表做分组处理(A~Z)
            if (cityListInfo != null && cityListInfo.Citys != null && cityListInfo.Citys.Count > 0)
            {
                var cityGroup = cityListInfo.Citys.GroupBy(cg => cg.pinyin.Substring(0, 1).ToUpper()).ToList();
                cityListEntity.CityGroupList = cityGroup.OrderBy(c => c.Key).ToList();
            }

            //根据坐标获取周边城市相关信息（这里不直接查询周边，放置alax异步处理）
            cityListEntity.AroundCityList = new AroundCityList();

            //isInWeixin
            cityListEntity.IsInWeixin = isInWeixin;

            //IsApp
            cityListEntity.IsApp = isApp;

            #endregion

            return View(cityListEntity);
        }

        /// <summary>
        /// 搜索（copy app版本）
        /// </summary>
        /// <param name="userlat"></param>
        /// <param name="userlng"></param>
        /// <returns></returns>
        public ActionResult Search(float userlat = 0, float userlng = 0)
        {
            var isApp = IsApp();
            ViewBag.IsApp = isApp;

            //当前系统环境（ios | android）
            ViewBag.AppType = AppType();

            //Weixin
            bool isInWeixin = Request.UserAgent.IndexOf("MicroMessenger") > 0 ? true : false;
            ViewBag.isInWeixin = isInWeixin;

            var curUserID = UserState.UserID > 0 ? UserState.UserID : 0;
            ViewBag.UserId = curUserID;

            return View();
        }

        /// <summary>
        /// 搜索城市
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public ActionResult SearchCityList(string keyword)
        {
            var isApp = IsApp();
            ViewBag.IsApp = isApp;

            var result = new Hotel().SuggestCityAndHotel(keyword, 3, 20);

            return View(result);

            //var cList = new List<HJD.HotelServices.Contracts.CityEntity>();

            //var cityList = Hotel.GetZmjdCityList();
            //if (cityList != null && cityList.Count > 0)
            //{
            //    var l1 = cityList.Where(c => c.Name.ToLower().StartsWith(keyword)).OrderBy(c => c.Name).ToList();
            //    if (l1 != null) cList.AddRange(l1);

            //    var l2 = cityList.Where(c => !c.Name.ToLower().StartsWith(keyword) && c.Name.ToLower().Contains(keyword)).OrderBy(c => c.Name).ToList();
            //    if (l2 != null) cList.AddRange(l2);
            //}

            //return View(cList);
        }

        /// <summary>
        /// 周边城市
        /// </summary>
        /// <param name="userlat"></param>
        /// <param name="userlng"></param>
        /// <returns></returns>
        public ActionResult AroundCityList(float userlat = 0, float userlng = 0)
        {
            var isApp = IsApp();

            //继承于 CityList
            var cityListEntity = new CityListEntity();

            //根据坐标获取周边城市相关信息
            if (userlat > 0 && userlng > 0)
            {
                cityListEntity.AroundCityList = Hotel.GetAroundCityList("", userlng, userlat, 0);
                if (cityListEntity.AroundCityList.cityList != null && cityListEntity.AroundCityList.cityList.Count > 0)
                {
                    cityListEntity.AroundCityList.cityList = cityListEntity.AroundCityList.cityList.Where(c => !c.DistrictName.ToLower().StartsWith("del_")).ToList();
                    cityListEntity.AroundCityList.aroundCityCount = cityListEntity.AroundCityList.cityList.Count;
                }
            }

            cityListEntity.IsApp = isApp;

            return View(cityListEntity);
        }

        /// <summary>
        /// 根据本地缓存生成搜索历史纪录
        /// </summary>
        /// <param name="history"></param>
        /// <returns></returns>
        public ActionResult GenCitySearchHistory(string history)
        {
            var isApp = IsApp();

            List<Dictionary<string, string>> back = new List<Dictionary<string, string>>();

            if (!string.IsNullOrEmpty(history))
            {
                var l1 = history.Split(';');
                var itemCount = 0;
                for (int i = 0; i < l1.Length; i++)
                {
                    var item = l1[i];
                    if (item.Contains(","))
                    {
                        var l2 = item.Split(',');
                        if (l2.Length > 1 && itemCount < 4 && !back.Exists(d => d.ContainsValue(l2[1])))
                        {
                            var dic = new Dictionary<string, string>();
                            var url = string.Format("http://www.zmjiudian.com/city{0}", l2[0]);
                            if (isApp) url = string.Format("whotelapp://www.zmjiudian.com/strategy/place?districtid={0}&title={1}", l2[0], l2[1]);
                            dic["url"] = url;
                            dic["id"] = l2[0];
                            dic["name"] = l2[1];
                            back.Add(dic);
                            itemCount++;
                        }
                    }
                }
            }

            return Json(back, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region 发现

        /// <summary>
        /// 新发现页
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="userlat"></param>
        /// <param name="userlng"></param>
        /// <returns></returns>
        public ActionResult Find(int userid = 0, int districtId = 2, string districtName = "上海", float lat = 0, float lng = 0, int geoScopeType = 2)
        {
            var isApp = IsApp();
            if (isApp)
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

            //Weixin
            bool isInWeixin = Request.UserAgent.IndexOf("MicroMessenger") > 0 ? true : false;
            ViewBag.isInWeixin = isInWeixin;

            //检测是否大于等于4.6版本（4.6之后app支持h5跳转至原生预订页面）
            var isThanVer46 = IsThanVer4_6();
            ViewBag.IsThanVer46 = isThanVer46;

            var isMobile = Utils.IsMobile();
            ViewBag.IsMobile = isMobile;

            ViewBag.UserId = userid;
            ViewBag.DistrictId = districtId;
            ViewBag.DistrictName = districtName;
            ViewBag.Userlat = lat;
            ViewBag.Userlng = lng;
            ViewBag.GeoScopeType = geoScopeType;

            return View();
        }

        /// <summary>
        /// 发现顶部专题
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="districtId"></param>
        /// <param name="districtName"></param>
        /// <param name="lat"></param>
        /// <param name="lng"></param>
        /// <param name="geoScopeType"></param>
        /// <returns></returns>
        public ActionResult FindTheme(int userid = 0, int districtId = 2, string districtName = "上海", float lat = 0, float lng = 0, int geoScopeType = 2)
        {
            var isApp = IsApp();
            ViewBag.IsApp = isApp;

            //当前系统环境（ios | android）
            var appType = AppType();
            ViewBag.AppType = appType;

            //Weixin
            bool isInWeixin = Request.UserAgent.IndexOf("MicroMessenger") > 0 ? true : false;
            ViewBag.isInWeixin = isInWeixin;

            ViewBag.UserId = userid;
            ViewBag.DistrictId = districtId;
            ViewBag.DistrictName = districtName;
            ViewBag.Userlat = lat;
            ViewBag.Userlng = lng;
            ViewBag.GeoScopeType = geoScopeType;

            //获取专题
            var themeList = Hotel.GetHomePageData40(lat, lng, geoScopeType, districtId, districtName, 6);
            ViewBag.ThemeList = themeList;

            return View();
        }

        /// <summary>
        /// 发现 大家都说好
        /// </summary>
        /// <returns></returns>
        public ActionResult FindHotRecommendHotel(int userid = 0, int districtId = 2, float lat = 0, float lng = 0)
        {
            var isApp = IsApp();
            ViewBag.IsApp = isApp;

            //当前系统环境（ios | android）
            var appType = AppType();
            ViewBag.AppType = appType;

            //Weixin
            bool isInWeixin = Request.UserAgent.IndexOf("MicroMessenger") > 0 ? true : false;
            ViewBag.isInWeixin = isInWeixin;

            var recHotels = new RecommendHotelResult();
            try
            {
                recHotels = Hotel.GetHotRecommendHotel(0, 10, lat, lng , districtId);
                if (recHotels != null && recHotels.HotelList != null && recHotels.HotelList.Count > 0)
                {
                    foreach (var item in recHotels.HotelList)
	                {
                        if (appType == "android" && (item.HotelPicUrl.Contains("upaiyun") || item.HotelPicUrl.Contains("p1.zmjiudian")))
                        {
                            item.HotelPicUrl = item.HotelPicUrl + "/format/webp";
                        }
	                }
                }
            }
            catch (Exception ex)
            {

            }
            ViewBag.RecHotels = recHotels;

            return View();
        }

        /// <summary>
        /// 正在点评
        /// </summary>
        /// <param name="s"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public ActionResult FindNewComments(int s = 0, int c = 6, int districtId = 2, float lat = 0, float lng = 0)
        {
            var isApp = IsApp();
            ViewBag.IsApp = isApp;

            //当前系统环境（ios | android）
            var appType = AppType();
            ViewBag.AppType = appType;

            //Weixin
            bool isInWeixin = Request.UserAgent.IndexOf("MicroMessenger") > 0 ? true : false;
            ViewBag.isInWeixin = isInWeixin;

            //获取点评数据
            var p = new RecommendCommentParam { start = s, count = c , districtid = districtId, lat = lat, lng = lng };
            var cmList = Comment.GetPublishedCommentList(p) ?? new RecommendCommentListModel();
            if (cmList != null && cmList.CommentList != null && cmList.CommentList.Count > 0)
            {
                if (appType == "android")
                {
                    foreach (var item in cmList.CommentList)
                    {
                        if (item.PhotoUrls != null && item.PhotoUrls.Count > 0)
                        {
                            for (int pnum = 0; pnum < item.PhotoUrls.Count; pnum++)
                            {
                                var pic = item.PhotoUrls[pnum];
                                if (pic.Contains("upaiyun") || pic.Contains("p1.zmjiudian"))
                                {
                                    pic = pic + "/format/webp";
                                }
                            }
                        }
                    }   
                }
            }

            ViewBag.CmList = cmList;

            return View();
        }

        /// <summary>
        /// 更多专题
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="districtId"></param>
        /// <param name="districtName"></param>
        /// <param name="lat"></param>
        /// <param name="lng"></param>
        /// <param name="geoScopeType"></param>
        /// <returns></returns>
        public ActionResult ThemeList(int userid = 0, int districtId = 2, string districtName = "上海", float lat = 0, float lng = 0, int geoScopeType = 2)
        {
            var isApp = IsApp();
            ViewBag.IsApp = isApp;

            //当前系统环境（ios | android）
            var appType = AppType();
            ViewBag.AppType = appType;

            //Weixin
            bool isInWeixin = Request.UserAgent.IndexOf("MicroMessenger") > 0 ? true : false;
            ViewBag.isInWeixin = isInWeixin;

            ViewBag.UserId = userid;
            ViewBag.DistrictId = districtId;
            ViewBag.DistrictName = districtName;
            ViewBag.Userlat = lat;
            ViewBag.Userlng = lng;
            ViewBag.GeoScopeType = geoScopeType;

            //获取所有专题
            var themeList = Hotel.GetHomePageData40(lat, lng, geoScopeType, districtId, districtName, 0);
            ViewBag.ThemeList = themeList;

            return View();
        }

        #endregion

        #region 推荐 & 周边好去处（m版的推荐页面）

        /// <summary>
        /// 周边好去处（目前该页面主要是面向微信环境使用，定位调用微信API）
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="userlat"></param>
        /// <param name="userlng"></param>
        /// <returns></returns>
        public ActionResult Well(int userid = 0, float userlat = 0, float userlng = 0)
        {
            var isApp = IsApp();
            if (isApp)
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

            //Weixin
            bool isInWeixin = Request.UserAgent.IndexOf("MicroMessenger") > 0 ? true : false;
            ViewBag.isInWeixin = isInWeixin;

            var isMobile = Utils.IsMobile();
            ViewBag.IsMobile = isMobile;

            ViewBag.UserId = userid;
            ViewBag.Userlat = userlat;
            ViewBag.Userlng = userlng;

            //当前用户是否VIP
            var isVip = false; if (userid > 0) isVip = account.IsVIPCustomer(new AccountInfoItem { Uid = userid.ToString() });
            ViewBag.IsVip = isVip;

            return View();
        }

        #endregion

        #region 微信入口的相关页面

        //超值精选
        public ActionResult HotPackage(int userid = 0)
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

            #region 读取闪购&亲子团

            HomePageData30 hotResult = new Hotel().GetAppHomePageData(userid);
            ViewBag.HotResult = hotResult;

            #endregion

            #region 读取精选

            RecommendHotelResult recmHotelResult = new Hotel().GetRecommendHotelResult(Convert.ToInt64(userid));
            ViewBag.RecmHotelResult = recmHotelResult;

            #endregion

            ViewBag.UserId = userid;
            return View();
        }

        /// <summary>
        /// 订优惠
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="userlat"></param>
        /// <param name="userlng"></param>
        /// <returns></returns>
        public ActionResult DiscountCollection(int userid = 0)
        {
            var isApp = IsApp();
            if (isApp)
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

            //Weixin
            bool isInWeixin = Request.UserAgent.IndexOf("MicroMessenger") > 0 ? true : false;
            ViewBag.isInWeixin = isInWeixin;

            //检测是否大于等于4.6版本（4.6之后app支持h5跳转至原生预订页面）
            var isThanVer46 = IsThanVer4_6();
            ViewBag.IsThanVer46 = isThanVer46;

            ViewBag.UserId = userid;

            //获取限时抢购信息
            HomePageData30 hotResult = new Hotel().GetAppHomePageData(userid);
            ViewBag.HotResult = hotResult;

            //获取Top 20
            RecommendHotelResult recmHotelResult = new Hotel().GetRecommendHotelResult(Convert.ToInt64(userid));
            ViewBag.RecHotelResult = recmHotelResult;

            return View();
        }

        /// <summary>
        /// 地区的酒店列表
        /// </summary>
        /// <param name="range">1江浙沪 2国内（不包括江浙沪）</param>
        /// <returns></returns>
        public ActionResult DiscountDistrictCollection(int range = 2)
        {
            var isApp = IsApp();
            ViewBag.IsApp = isApp;

            //当前系统环境（ios | android）
            var appType = AppType();
            ViewBag.AppType = appType;

            //Weixin
            bool isInWeixin = Request.UserAgent.IndexOf("MicroMessenger") > 0 ? true : false;
            ViewBag.isInWeixin = isInWeixin;

            //检测是否大于等于4.6版本（4.6之后app支持h5跳转至原生预订页面）
            var isThanVer46 = IsThanVer4_6();
            ViewBag.IsThanVer46 = isThanVer46;

            CanSellDistrictHotelResult result = new Hotel().GetCanSellDistrictCheapHotelList((HotelServiceEnums.HotelDistrictRange)range);
            ViewBag.Result = result;

            return View();
        }

        #endregion

        #region 我的订单（包含了订单、房券、消费券）

        /// <summary>
        /// 订单（包含酒店订单、房券、消费券）
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="specifyuid">是否制定userid,如果是,则已参数传递的uid为准,忽略session登录信息</param>
        /// <returns></returns>
        public ActionResult Order(int userid = 0, int specifyuid = 0)
        {
            var isApp = IsApp();
            if (isApp)
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

            //Weixin
            bool isInWeixin = Request.UserAgent.IndexOf("MicroMessenger") > 0 ? true : false;
            ViewBag.isInWeixin = isInWeixin;

            //当前APP版本号（如4.x）
            var appVerCode = AppVersionCode();
            ViewBag.AppVerCode = appVerCode;

            //APP Bundle Ver
            ViewBag.AppBundleVer = _ContextBasicInfo.AppBundleVer;

            var isMobile = Utils.IsMobile();
            ViewBag.IsMobile = isMobile;

            var curUserID = UserState.UserID > 0 ? UserState.UserID : userid;
            if (specifyuid == 1 && userid > 0)
            {
                curUserID = userid;
            }
            ViewBag.UserId = curUserID;

            //酒店订单url
            var hotelOrderUrl = "/personal/order";
            if (isApp)
            {
                hotelOrderUrl = "whotelapp://www.zmjiudian.com/personal/order";
            }
            ViewBag.HotelOrderUrl = hotelOrderUrl;

            //通用参数
            GetInspectorHotelsListParam lp = new GetInspectorHotelsListParam { userid = curUserID };

            //房券url
            var roomCouponUrl = GetUserWalletUrl(curUserID, "roomcoupon", newpage: true, dorpdown: true);
            ViewBag.RoomCouponUrl = roomCouponUrl;

            //房券list信息
            var roomcouponResult = new Coupon().GetPersonalRoomCoupon(lp);
            if (roomcouponResult != null && roomcouponResult.couponList != null)
            {
                roomcouponResult.couponList = roomcouponResult.couponList.Where(_ => _.State != 4).ToList();
            }
            ViewBag.RoomcouponResult = roomcouponResult;

            //消费券url
            var productCouponUrl = GetUserWalletUrl(curUserID, "productcoupon", "detail", newpage: true, dorpdown: true);
            ViewBag.ProductCouponUrl = productCouponUrl;

            //消费券count
            var productCouponCount = new Coupon().GetPersonalProductCouponCount(curUserID);
            ViewBag.ProductCouponCount = productCouponCount;

            return View();
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

        #endregion

        #region

        /// <summary>
        /// 城市周边页面（App6.0版本开始的城市周边页面，目前包含了周边酒店、周边美食周边玩乐三个栏目
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="userlat"></param>
        /// <param name="userlng"></param>
        /// <returns></returns>
        public ActionResult Around(int userid = 0, float userlat = 0, float userlng = 0)
        {
            var isApp = IsApp();
            if (isApp)
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

            //Weixin
            bool isInWeixin = Request.UserAgent.IndexOf("MicroMessenger") > 0 ? true : false;
            ViewBag.isInWeixin = isInWeixin;

            var isMobile = Utils.IsMobile();
            ViewBag.IsMobile = isMobile;

            ViewBag.UserId = userid;
            ViewBag.Userlat = userlat;
            ViewBag.Userlng = userlng;

            //当前用户是否VIP
            var isVip = false; if (userid > 0) isVip = account.IsVIPCustomer(new AccountInfoItem { Uid = userid.ToString() });
            ViewBag.IsVip = isVip;

            return View();
        }

        #endregion

        #region 其它

        /// <summary>
        /// 短信段发送的app首页
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public ActionResult HomeSms(int userid = 0)
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

            #region 读取首页头部banner

            var adInfo = new HomeDataModel20();
            var adList = new List<ADItem>();
            try
            {
                adInfo = new Hotel().GetUserHomeData20(userid);
                if (adInfo != null && adInfo.AD != null)
                {
                    adList = adInfo.AD.ADList;
                }
            }
            catch (Exception ex)
            {

            }
            ViewBag.AdInfo = adInfo;
            ViewBag.AdList = adList;

            #endregion

            #region 读取首页推荐点评

            var rcmParm = new RecommendCommentListQueryParam
            {
                start = 0,
                count = 4
            };

            RecommendCommentListModel recmComments = Comment.GetRecommendCommentListModel(rcmParm);
            ViewBag.RecmComments = recmComments;

            #endregion

            #region 读取首页闪购&亲子团

            HomePageData30 hotResult = new Hotel().GetAppHomePageData(userid);
            ViewBag.HotResult = hotResult;

            #endregion

            #region 读取首页精选

            RecommendHotelResult recmHotelResult = new Hotel().GetRecommendHotelResult(Convert.ToInt64(userid));
            ViewBag.RecmHotelResult = recmHotelResult;

            #endregion

            ViewBag.UserId = userid;
            return View();
        }

        /// <summary>
        /// 查找酒店页面
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public ActionResult SearchHotel(int userid = 0)
        {
            var isApp = IsApp();
            if (isApp)
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

            //获取查找酒店页面的背景相关信息
            var preData = new PreLoadAppData()
            {
                SearchHotelAdv = new SearchHotelAdv()
                {
                    ActionUrl = "whotelapp://www.zmjiudian.com/hotel/213676",
                    HotelBGP = "http://whphoto.b0.upaiyun.com/115MOsC04B_jupiter",
                    HotelID = 213676,
                    HotelName = "溧阳涵田度假村酒店"
                }
            };
            var preParam = new PreLoadAppDataParam();
            preParam.DistrictID = 0;
            preParam.DistrictName = "";
            preParam.Lon = 0;
            preParam.Lat = 0;
            preParam.UserID = userid;
            try
            {
                preData = new App().GetPreLoadAppData(preParam);
            }
            catch (Exception ex)
            {

            }

            //如果不是在app内，酒店url更改为http
            if (!isApp)
            {
                preData.SearchHotelAdv.ActionUrl = preData.SearchHotelAdv.ActionUrl.Replace("whotelapp", "http");
            }

            ViewBag.PreData = preData;

            ViewBag.UserId = userid;

            return View();
        }

        #endregion
        #region MyRegion

        public ActionResult AlbumGroup(string groupNos = "180502,180501")
        {
            ViewBag.GroupNos = groupNos;
            return View();
        } 

        #endregion
    }
}