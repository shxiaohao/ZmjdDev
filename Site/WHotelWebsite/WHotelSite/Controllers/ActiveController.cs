using HJDAPI.APIProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using WHotelSite.Models;
using HJD.ADServices.Contract;
using HJD.HotelServices.Contracts;
using HJDAPI.Common.Helpers;
using System.Net;
using Newtonsoft.Json;
using HJD.WeixinServices.Contracts;
using HJDAPI.Models;
using WHotelSite.Common;
using HJDAPI.Controllers.Common;
using System.Web.Security;
using System.Text.RegularExpressions;
using HJD.AccountServices.Entity;
using HJD.CouponService.Contracts.Entity;
using HJDAPI.Controllers;
using HJDAPI.Controllers.Adapter;
using HJDAPI.Common.Security;
using System.Xml;
using HJD.HotelPrice.Contract;
using HJD.HotelManagementCenter.Domain.Fund;
using HJD.WeixinService.Contract;
using GeetestSDK;
using Com.Ctrip.Framework.Apollo;

namespace WHotelSite.Controllers
{
    public class ActiveController : BaseController
    {
        //
        // GET: /Active/

        public const string AccessProtocal_IsApp = "whotelapp://www.zmjiudian.com/";
        public const string AccessProtocalPage_IsApp = "whotelapp://gotopage?url=http://www.zmjiudian.com/";

        public const string AccessProtocal_UnApp = "/inspector/jump?jumpurl=whotelapp://www.zmjiudian.com/";  //"http://www.zmjiudian.com/hotel/";  //"http://m1.zmjiudian.com/";
        public const string AccessProtocalPage_UnApp = "/inspector/jump?jumpurl=whotelapp://www.zmjiudian.com/gotopage?url=http://www.zmjiudian.com/";
        //public const string AccessProtocalPage_UnApp = "/inspector/jump?jumpurl=whotelapp://www.zmjiudian.com/gotopage?url=http://192.168.1.22:8081/";


        public ActionResult GotoURL(string title, string gotoURL)
        {
            GotoURLModel m = new GotoURLModel();
            m.Title = title;
            m.GotoURL = gotoURL;
            return View(m);
        }

        /// <summary>
        /// 活动浏览页面（目前只是解析ActivePage和WeixinMaterial）
        /// </summary>
        /// <param name="pageid">ActivePage的ID</param>
        /// <param name="defbg">设置1的时候，自定义页面为白色</param>
        /// <param name="nopadding">自定义控制页面的padding</param>
        /// <param name="midx">WeixinMaterial的IDX</param>
        /// <returns></returns>
        public string ActivePage(int pageid = 0, int defbg = 0, int nopadding = 0, int midx = 0)
        {
            //IsApp
            var isApp = IsApp();

            ActivePageEntity ap = new ActivePageEntity();
            WeixinMaterialEntity wm = new WeixinMaterialEntity();

            //活动基础信息
            var _ActiveTitle = "";
            var _ActiveContent = "";
            var _ActiveType = 1;

            //获取数据
            if (pageid > 0)
            {
                ap = Activity.GetActivePageByIDX(pageid);

                _ActiveTitle = ap.Name;
                _ActiveContent = ap.HTML;
                _ActiveType = ap.Type;
            }
            else if (midx > 0)
            {
                wm = new Weixin().GetWeixinMaterialByIdx(midx);

                _ActiveTitle = wm.Title;
                _ActiveContent = wm.Content;
                _ActiveType = wm.Type;
            }

            //组织追加自定义样式等
            var tableAddStyle = @"
<style>
    table{ border-top:1px solid #ddd;border-left:1px solid #ddd; }
        table tr td{ border-right:1px solid #ddd;border-bottom:1px solid #ddd; }
</style>";

            //素材页面默认不追加这些代码
            if (midx > 0)
            {
                tableAddStyle = "";
            }

            if (nopadding == 1)
            {
                tableAddStyle = @"
<style>
    .rich_media_inner{
        padding:0 0 0 0;
    }
</style>" + tableAddStyle;
            }

            if (defbg == 1)
            {
                tableAddStyle = @"
<style>
    body{background-color:#fff;}
</style>" + tableAddStyle;
            }

            _ActiveContent = tableAddStyle + _ActiveContent;

            _ActiveContent = SetUrlProtocol(_ActiveContent);

            //获取模板html
            ActivePageTemplateEntity apt = Activity.GetActivePageTemplateByID(_ActiveType);
            string HTML = apt.Template;

            HTML = HTML.Replace("/content/css/activepage.css", string.Format("/content/css/activepage.css?v={0}", curAppVer()));

            string[] Paras = apt.Params.Split(',');
            foreach (string par in Paras)
            {
                string key = par.Split(':')[0];
                string value = par.Split(':')[1];

                switch (key.ToLower())
                {
                    case "title":
                        HTML = HTML.Replace(value, _ActiveTitle);
                        break;
                    case "html":
                        HTML = HTML.Replace(value, _ActiveContent);
                        break;
                }
            }
            //ap.HTML = HTML;

            try
            {
                //追加基本脚本
                var baseJs = @"
<script src='/Content/js/jquery-1.10.2-min.js'></script>
<script src='/Content/js/framework/jquery.lazyload.min.js'></script>
<script src='/Content/js/framework/config.js'></script>
<script src='/Content/js/framework/zmjd.swiper.min.js'></script>
";

                HTML = HTML + baseJs;

                //对图片src做处理，为支持懒加载
                HTML = Regex.Replace(HTML, "\" src=", "\" data-original=");

                //追加懒加载js
                var lazyloadJs = @"
<script>
$(function(){

$('img').lazyload({
    threshold: 20,
    placeholder: '/Content/images/seat/img-home-seat-banner2.png',
    effect: 'show'
});

});
</script>";
                HTML = HTML + lazyloadJs;

                //替换微信的图片地址为不可用
                HTML = HTML.Replace("mmbiz.qpic.cn", "zmjiudian");

                if (isApp && false)
                {
                    var _appShareUrl = "http://www.zmjiudian.com/active/activepage?pageid=" + pageid + "&midx=" + midx + "&defbg=1";

                    //追加分享脚本
                    var shareJs = @"
<script>
try {
    onAppShareCall = function () {
        try { whotel.appShareData('" + _ActiveTitle + @"-周末酒店', '', 'http://whfront.b0.upaiyun.com/www/img/Logo/logo_50.png', '" + _appShareUrl + @"'); } catch (e) { }
        var returnStr = ""{'title':'" + _ActiveTitle + @"-周末酒店','Content':'','photoUrl':'http://whfront.b0.upaiyun.com/www/img/Logo/logo_50.png','shareLink':'" + _appShareUrl + @"'}"";
        return returnStr;
    }
    onAppShareCall();
}
catch (e) { }
</script>
";

                    HTML = HTML + shareJs;   
                }
            }
            catch (Exception ex)
            {
                Log.WriteLog("ActivePage ERROR：" + ex);
            }

            return HTML;
        }


        public ActionResult Active101()
        {
            DateTime EndDate = DateTime.Parse("2014-10-7").Date;
           
                List<HJD.HotelServices.Contracts.CanSaleHotelInfoEntity> list = Hotel.GetAllCanSellPackage(DateTime.Now.Date, EndDate);
            
                return View(list);
        }

        public ActionResult SpecialDeal()
        {
            List<HJD.HotelServices.Contracts.SpecialDealPackageEntity> sl = Hotel.GetSpecialDealPackage();
            SetUrlProtocol();
            return View(sl);
        }
        public ActionResult WeekendPackages(string strCheckDate = "2014-1-1")
        {
            DateTime FirstSat = DateTime.Now.AddDays(6 - (int)DateTime.Now.DayOfWeek).Date;
            DateTime checkDate = FirstSat;
            if (strCheckDate != "2014-1-1")
            {
                checkDate = DateTime.Parse(strCheckDate).Date;
            }

            List<HJD.HotelServices.Contracts.CanSaleHotelInfoEntity> list = Hotel.GetAllCanSellPackage(checkDate, checkDate);

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < 4; i++)
            {
                DateTime Sat = FirstSat.AddDays( i* 7);

                sb.AppendFormat(@" <div class=""col-xs-3 {2}"">
                    <a href=""/active/WeekendPackages?strCheckDate={0}"">{1}</a>
                </div>", Sat.ToShortDateString(), Sat.ToString("MM.dd"), Sat == checkDate ? "cur" : "");

            }

            ViewBag.SatList = sb.ToString();

            return View(list);
        }

        public ActionResult SpringFestival2015(string strCheckDate = "2014-1-1")
        {
             if( IsApp ())
             {
                 ViewBag.AccessProtocal = "whotelapp://www.zmjiudian.com/hotel/";
             }
             else
             {
                 ViewBag.AccessProtocal = "http://m1.zmjiudian.com/";  //"http://www.zmjiudian.com/hotel/";// "http://m1.zmjiudian.com/";
             }

            DateTime FirstSat = DateTime.Now.Date;
            if( FirstSat < DateTime.Parse("2015-2-18").Date)
            {
                FirstSat = DateTime.Parse("2015-2-18").Date;
            }

            DateTime checkDate = FirstSat;
            if (strCheckDate != "2014-1-1")
            {
                checkDate = DateTime.Parse(strCheckDate).Date;
            }

            List<HJD.HotelServices.Contracts.CanSaleHotelInfoEntity> list = Hotel.GetAllCanSellPackage(checkDate, checkDate,"1");//"1" 表示春节有房

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < 7; i++)
            {
                DateTime Sat = FirstSat.AddDays( i* 1);

                if (Sat < DateTime.Parse("2015-2-24"))
                {
                    sb.AppendFormat(@" <div class=""col-xs-4 {2}"">
                    <a href=""/active/SpringFestival2015?strCheckDate={0}"">{1}</a>
                </div>", Sat.ToShortDateString(), Sat.ToString("M.dd"), Sat == checkDate ? "cur" : "");
                }

            }

            ViewBag.SatList = sb.ToString();

            return View(list);
        }

        public ActionResult Qingming(string strCheckDate = "2014-1-1")
        {
            if (IsApp())
            {
                ViewBag.AccessProtocal = "whotelapp://www.zmjiudian.com/hotel/";
            }
            else
            {
                ViewBag.AccessProtocal = "http://m1.zmjiudian.com/";  //"http://www.zmjiudian.com/hotel/";// "http://m1.zmjiudian.com/";
            }

            DateTime FirstSat = DateTime.Now.Date;
            if (FirstSat < DateTime.Parse("2015-6-19").Date)
            {
                FirstSat = DateTime.Parse("2015-6-19").Date;
            }

            DateTime checkDate = FirstSat;
            if (strCheckDate != "2014-1-1")
            {
                checkDate = DateTime.Parse(strCheckDate).Date;
            }

            List<HJD.HotelServices.Contracts.CanSaleHotelInfoEntity> list = Hotel.GetAllCanSellPackage(checkDate, checkDate, "1");//"1" 表示春节有房

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < 3; i++)
            {
                DateTime Sat = FirstSat.AddDays(i * 1);

                if (Sat < DateTime.Parse("2015-6-22"))
                {
                    sb.AppendFormat(@" <div class=""col-xs-4 {2}"">
                    <a href=""/active/Qingming?strCheckDate={0}"">{1}</a>
                </div>", Sat.ToShortDateString(), Sat.ToString("M.d"), Sat == checkDate ? "cur" : "");
                }

            }

            ViewBag.SatList = sb.ToString();

            return View(list);
        }
        /// <summary>
        /// 节日有房
        /// </summary>
        /// <returns></returns>
        public ActionResult Holiday(string strCheckDate = "2014-1-1", string userid = "0", string sid = "", int swvip = 0)
        {
            var isApp = IsApp();
            ViewBag.IsApp = isApp;

            //当前系统环境（ios | android）
            ViewBag.AppType = AppType();

            ViewBag.UserId = userid;

            ViewBag.PageTitle = Apollo.Get("HolidayTitle");

            return HolidayContent(strCheckDate, userid, sid, swvip);
        }

        /// <summary>
        /// 承载各个节日有房的页面
        /// </summary>
        /// <param name="strCheckDate"></param>
        /// <param name="userid"></param>
        /// <param name="sid"></param>
        /// <param name="swvip"></param>
        /// <returns></returns>
        public ActionResult HolidayContent(string strCheckDate = "2014-1-1", string userid = "0", string sid = "", int swvip = 0)
        {
            if (IsApp())
            {
                ViewBag.AccessProtocal = "whotelapp://www.zmjiudian.com/hotel/";
            }
            else
            {
                ViewBag.AccessProtocal = "http://www.zmjiudian.com/hotel/";  //"http://www.zmjiudian.com/hotel/";// "http://m1.zmjiudian.com/";
            }

            //当前系统环境（ios | android）
            ViewBag.AppType = AppType();

            //isMobile
            var isMobile = Utils.IsMobile();
            ViewBag.IsMobile = isMobile;

            //Weixin
            bool isInWeixin = Request.UserAgent.IndexOf("MicroMessenger") > 0 ? true : false;
            ViewBag.isInWeixin = isInWeixin;

            //页面头banner
            var topBanner = Apollo.Get("HolidayBannerUrl");
            ViewBag.TopBanner = topBanner;

            //节假日开始日期（通常会设置为节假日开始的前一天，从酒店层面看周五的价格算是周末价）
            DateTime dateFrom = DateTime.Parse(Apollo.Get("HolidayFromDate")).Date;

            //结束日不显示
            DateTime dateEnd = DateTime.Parse(Apollo.Get("HolidayEndDate")).Date;

            //日期名称配置
            var dateNames = Apollo.Get("HolidayDateNames").Split(',').ToList();
            ViewBag.DateNames = dateNames;

            DateTime FirstSat = DateTime.Now.Date;
            if (FirstSat < dateFrom)
            {
                FirstSat = dateFrom;
            }

            DateTime checkDate = FirstSat;
            if (strCheckDate != "2014-1-1")
            {
                checkDate = DateTime.Parse(strCheckDate).Date;
            }

            //"1" 表示春节有房 
            List<CanSaleHotelInfoEntity> list = Hotel.GetAllCanSellPackage(checkDate, checkDate.AddDays(1), "1");

            //读取需要排除的日期配置
            var removeDatesStr = Apollo.Get("HolidayRemoveDates");
            var removeDates = new List<DateTime>();
            if (!string.IsNullOrEmpty(removeDatesStr))
            {
                var _removelist = removeDatesStr.Split(',');
                for (int _rdnum = 0; _rdnum < _removelist.Length; _rdnum++)
                {
                    removeDates.Add(DateTime.Parse(_removelist[_rdnum]));
                }
            }

            var dateList = new List<DateTime>();
            int dayCount = (dateEnd.Date - dateFrom.Date).Days;
            for (int i = 0; i < dayCount; i++)
            {
                DateTime Sat = FirstSat.AddDays(i * 1);

                if (Sat < dateEnd && !removeDates.Contains(Sat))
                {
                    dateList.Add(Sat);
                }

            }
            ViewBag.DateList = dateList;
            ViewBag.CheckIn = checkDate;
            ViewBag.CheckOut = checkDate.AddDays(1);

            //节日有房酒店数据分组处理
            var cityOrderListStr = Apollo.Get("HolidayCityOrderStr");
            cityOrderListStr = cityOrderListStr == "0" ? "" : cityOrderListStr;
            var proOrderListStr = Apollo.Get("HolidayProvinceOrderStr");
            proOrderListStr = proOrderListStr == "0" ? "" : proOrderListStr;
            List<string> cityOrderList = cityOrderListStr.Split(',').ToList();
            List<string> disOrderList = proOrderListStr.Split(',').ToList();

            //分组集合
            Dictionary<string, List<CanSaleHotelInfoEntity>> d1 = new Dictionary<string, List<CanSaleHotelInfoEntity>>();
            Dictionary<string, List<CanSaleHotelInfoEntity>> d2 = new Dictionary<string, List<CanSaleHotelInfoEntity>>();
            if (list != null && list.Count > 0)
            {
                list = list.Where(h => h.Night == checkDate).ToList();

                //城市list
                foreach (var cityName in cityOrderList)
                {
                    if (!string.IsNullOrEmpty(cityName))
                    {
                        d1[cityName] = list.Where(p => p.DistrictName == cityName).OrderBy(c => c.HotelName).ToList();   
                    }
                }

                //其他list
                var otherList = list.Where(p => !cityOrderList.Contains(p.DistrictName)).OrderBy(c => c.HotelName).ToList();

                //将套餐列表按照其酒店的目的地分组
                var g1 = otherList.GroupBy(_ => _.ProvinceName).OrderBy(_ => _.Key);
                foreach (var disName in disOrderList)
                {
                    try
                    {
                        var _listObj = g1.First(g => g.Key == disName);
                        if (_listObj != null)
                        {
                            var _list = _listObj.ToList();
                            d1[disName] = _list.OrderBy(c => c.HotelName).ToList();
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }

                foreach (var key in g1)
                {
                    if (!disOrderList.Contains(key.Key))
                    {
                        d1[key.Key] = key.OrderBy(c => c.HotelName).ToList();
                    }
                }
            }

            ////匹配所有包房酒店的信息
            //var packRoomHotelIdList = Hotel.GetPackRoomHotelIdList(checkDate);
            //var packHotelList = list.FindAll(_ => packRoomHotelIdList.Contains(_.HotelId));

            //价格优势的套餐靠前
            var packHotelList = list.Where(_ => _.PackageIsInBenefitArea).OrderBy(ph => ph.HotelName).ToList();
            ViewBag.packHotelList = packHotelList;

            //将对应地区的包房酒店放置列表顶部
            foreach (var dKey in d1.Keys)
            {
                var _list = d1[dKey];
                var _packList = packHotelList.Where(ph => ph.DataPathName.Contains(dKey)).OrderBy(ph => ph.HotelName).ToList();
                if (_list != null && _list.Count > 0) _list = _list.Where(h => !_packList.Exists(ph => ph.HotelId == h.HotelId)).ToList();

                d2[dKey] = new List<CanSaleHotelInfoEntity>();
                d2[dKey].AddRange(_packList);
                d2[dKey].AddRange(_list);
            }

            ViewBag.D1 = d2;

            #region 分享配置

            var shareLink = "";
            MemberProfileInfo shareUserInfo = new HJD.AccountServices.Entity.MemberProfileInfo();

            if (!string.IsNullOrEmpty(sid))
            {
                //解析分享跟踪参数
                try
                {
                    long sourceId = 0;
                  
                    var originObj = base.DesSID(sid); 
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
                scp.BizType = ZMJDShareTrackBizType.hoteldetail;
                var shareCode = Comment.GenTrackCodeResult4Share(scp);

                //分享链接生成
                shareLink = string.Format("http://www.zmjiudian.com/Active/holiday?sid={0}", shareCode.EncodeStr);
            }

            var shareTitle = Apollo.Get("HolidayShareTitle");
            ViewBag.ShareTitle = shareTitle;

            var shareContent = Apollo.Get("HolidayShareContent");
            ViewBag.ShareContent = shareContent;

            var shareImg = Apollo.Get("HolidayShareImgUrl");
            ViewBag.ShareImg = shareImg.Replace("_jupiter", "_290x290s").Replace("_appdetail1s", "_290x290s");

            ViewBag.ShareLink = shareLink.Replace("shangjiudian", "zmjiudian");

            ViewBag.ShareUserInfo = shareUserInfo;

            #endregion

            //当前用户是否VIP
            var isVip = false; if (Convert.ToInt32(userid) > 0) isVip = account.IsVIPCustomer(new AccountInfoItem { Uid = userid });
            ViewBag.IsVip = isVip;

            //是否显示VIP标识
            ViewBag.SwVip = (swvip == 1);

            ViewBag.UserId = userid;

            return View(list);
        }

        public ActionResult NewYearDay2015(string strCheckDate = "2014-1-1")
        {
            DateTime FirstSat = DateTime.Now.Date;
            if( FirstSat < DateTime.Parse("2014-12-31").Date)
            {
                FirstSat = DateTime.Parse("2014-12-31").Date;
            }

            DateTime checkDate = FirstSat;
            if (strCheckDate != "2014-1-1")
            {
                checkDate = DateTime.Parse(strCheckDate).Date;
            }

            List<HJD.HotelServices.Contracts.CanSaleHotelInfoEntity> list = Hotel.GetAllCanSellPackage(checkDate, checkDate);

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < 3; i++)
            {
                DateTime Sat = FirstSat.AddDays( i* 1);

                if (Sat < DateTime.Parse("2015-1-3"))
                {
                    sb.AppendFormat(@" <div class=""col-xs-4 {2}"">
                    <a href=""/active/NewYearDay2015?strCheckDate={0}"">{1}</a>
                </div>", Sat.ToShortDateString(), Sat.ToString("yyyy.MM.dd"), Sat == checkDate ? "cur" : "");
                }

            }

            ViewBag.SatList = sb.ToString();

            return View(list);
        }

        public string SetUrlProtocol(string HTML)
        {
            string apptype = Request.Headers["apptype"] != null ? Request.Headers["apptype"] : "";
            string appver = Request.Headers["appver"] != null ? Request.Headers["appver"] : "";

            if (apptype != "")
            {
                //HTML = HTML.Replace("http://www.zmjiudian.com", "whotelapp://www.zmjiudian.com").Replace("tp=webp","");
            }
            else
            {
                HTML = HTML.Replace("{userid}", "0"); 
            }

            return HTML;
        }

        /// <summary>
        /// 设置连接访问协议，以便在APP中直接打开相应界面
        /// </summary>
        public void SetUrlProtocol()
        {
           string  apptype = Request.Headers["apptype"] != null ? Request.Headers["apptype"] : "";
           string  appver = Request.Headers["appver"] != null ? Request.Headers["appver"] : "";

            if (apptype != "")
                ViewBag.urlProtocol = "whotelapp";
            else
                ViewBag.urlProtocol = "http";
        }

        public ActionResult Test()
        {
            return View();
        }

        public ActionResult Top20Deals()
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

            var list = new Hotel().GetTop20Package();

            return View(list);
        }

        public ActionResult Top20DealsForMail()
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

            var list = new Hotel().GetTop20Package();

            return View(list);
        }

        #region 微信报名活动【通用】

        /// <summary>
        /// 【活动分享页】
        /// </summary>
        /// <returns></returns>
        public ActionResult Weixin_SignupActive(string aidsharer = "0123450", string code = "", string state = "")
        {
            ViewBag.IsApp = IsApp();

            var activeid = "0";
            var sharer = "0";
            if (!string.IsNullOrEmpty(aidsharer) && aidsharer.Contains("12345"))
            {
                activeid = Regex.Split(aidsharer, "12345")[0];
                sharer = Regex.Split(aidsharer, "12345")[1];
            }

            var weixinActiveEntity = GetWeixinActiveEntity(Convert.ToInt32(activeid));

            //分享页面和分享图片
            ViewBag.ShareTitle = weixinActiveEntity.WeixinSignUpShareTitle;
            ViewBag.ShareLink = weixinActiveEntity.WeixinSignUpShareLink;
            ViewBag.ShareImgUrl = weixinActiveEntity.WeixinSignUpShareImgUrl;
            ViewBag.TopBannerUrl = weixinActiveEntity.WeixinSignUpTopBannerUrl;
            ViewBag.ActiveId = activeid;

            if (!string.IsNullOrEmpty(code))
            {
                //首先拿到分享者的openid
                var shareWeixinUser = new Weixin().GetActiveWeixinUserById(Convert.ToInt32(sharer));
                if (shareWeixinUser != null && weixinActiveEntity != null && weixinActiveEntity.ActiveEndTime > DateTime.Now)
                {
                    //通过code换取网页授权access_token
                    var accessToken = WeiXinHelper.GetWeixinAccessToken(code);

                    if (shareWeixinUser.Openid.ToLower().Trim() != accessToken.Openid.ToLower().Trim())
                    {
                        //有了分享者openid，又拿到了当前阅读者的openid，则插入一条阅读记录
                        var activeRead = new ActiveWeixinShareRead();
                        activeRead.ActiveID = Convert.ToInt32(activeid);
                        activeRead.SharerOpenid = shareWeixinUser.Openid;
                        activeRead.Openid = accessToken.Openid;
                        activeRead.LastReadTime = DateTime.Now;
                        activeRead.ReadCount = 1;

                        var add = new Weixin().AddActiveWeixinShareRead(activeRead);
                    }
                }
            }
            else
            {
                //授权页面
                var weixinGoUrl = WeiXinHelper.GenSilenceAuthorUrl(HttpUtility.UrlEncode(string.Format("http://www.zmjiudian.com/active/Weixin_SignupActive/{0}12345{1}", activeid, sharer)));
                return Redirect(weixinGoUrl);
            }

            //走到这里，说明已经静默授权了，则直接跳到活动页
            return Redirect(weixinActiveEntity.WeixinSignUpShareLink);
        }

        /// <summary>
        /// 【活动报名页】
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public ActionResult Weixin_SignupActive_Reg(int activeid = 0, string code = "", string state = "")
        {
            var openid = "";

            ViewBag.IsApp = IsApp();
            ViewBag.AppId = WeiXinHelper.WeixinServiceAppId;
            ViewBag.AppSecret = WeiXinHelper.WeixinServiceSecret;
            ViewBag.Snsapi_base = WeiXinHelper.Snsapi_base;
            ViewBag.Snsapi_userinfo = WeiXinHelper.Snsapi_userinfo;

            var weixinActiveEntity = GetWeixinActiveEntity(activeid);
            ViewBag.WeixinActiveEntity = weixinActiveEntity;

            //分享页面和分享图片
            ViewBag.ShareTitle = weixinActiveEntity.WeixinSignUpShareTitle;
            ViewBag.ShareLink = weixinActiveEntity.WeixinSignUpShareLink;
            ViewBag.ShareImgUrl = weixinActiveEntity.WeixinSignUpShareImgUrl;
            ViewBag.TopBannerUrl = weixinActiveEntity.WeixinSignUpTopBannerUrl;
            ViewBag.ActiveId = activeid;

            if (!string.IsNullOrEmpty(code))
            {
                //通过code换取网页授权access_token
                var accessToken = WeiXinHelper.GetWeixinAccessToken(code);
                //var accessToken = new WeixinAccessToken { Openid = "okg6-uBqijun__Frr_dJfV8m6Tjs" };

                //通过access_token拉取用户信息(需scope为 snsapi_userinfo)（这是通过微信api获取的真实用户信息）
                var weeixinUserInfo = WeiXinHelper.GetWeixinUserInfo(accessToken.AccessToken, accessToken.Openid);

                //先查询当前openid是否已经存储微信用户信息
                var weixinUser = new Weixin().GetActiveWeixinUser(accessToken.Openid);
                if (weixinUser != null && weixinUser.ID > 0)
                {
                    //如果已经存储该用户信息，再获取下用户信息，主要更新其头像和昵称相关
                    try
                    {
                        if (weeixinUserInfo != null)
                        {
                            var weixinNewUser = new ActiveWeixinUser();
                            weixinNewUser.Openid = accessToken.Openid;
                            weixinNewUser.Nickname = weeixinUserInfo.Nickname;
                            weixinNewUser.Sex = weeixinUserInfo.Sex.ToString();
                            weixinNewUser.Province = weeixinUserInfo.Province;
                            weixinNewUser.City = weeixinUserInfo.City;
                            weixinNewUser.Country = weeixinUserInfo.Country;
                            weixinNewUser.Headimgurl = weeixinUserInfo.Headimgurl;
                            weixinNewUser.Privilege = "";
                            weixinNewUser.Unionid = weeixinUserInfo.Unionid;
                            weixinNewUser.Phone = "";
                            weixinNewUser.ActiveID = activeid;
                            var add = new Weixin().AddActiveWeixinUser(weixinNewUser);
                        }
                    }
                    catch (Exception ex)
                    {
                        
                    }

                    //检查报名记录
                    var weixinDraw = new Weixin().GetActiveWeixinDraw(activeid, accessToken.Openid);
                    if (weixinDraw != null)
                    {
                        //是否分享？已经分享则直接跳至分享成功页面
                        if (weixinDraw.IsShare == 1)
                        {
                            return Redirect(string.Format("/Active/Weixin_SignupActive_ShareDone/{0}?openid={1}", activeid, accessToken.Openid));
                        }
                        //授权，但是没有分享，则跳转至报名待分享页面
                        else
                        {
                            //当前活动已经下线的时候，跳至活动结束页面
                            if (weixinActiveEntity != null && weixinActiveEntity.ActiveEndTime < DateTime.Now)
                            {
                                return Redirect(string.Format("/Active/Weixin_SignupActive_End/{0}?openid={1}", activeid, accessToken.Openid));
                            }
                            return Redirect(string.Format("/Active/Weixin_SignupActive_RegDone/{0}?openid={1}", activeid, accessToken.Openid));
                        }
                    }
                    //没有报名，则停留在此页进行报名
                    else
                    {
                        //当前活动已经下线的时候，跳至活动结束页面
                        if (weixinActiveEntity != null && weixinActiveEntity.ActiveEndTime < DateTime.Now)
                        {
                            return Redirect(string.Format("/Active/Weixin_SignupActive_End/{0}?openid={1}", activeid, accessToken.Openid));
                        }
                    }
                }
                else
                {
                    //当前活动已经下线的时候，跳至活动结束页面
                    if (weixinActiveEntity != null && weixinActiveEntity.ActiveEndTime < DateTime.Now)
                    {
                        return Redirect(string.Format("/Active/Weixin_SignupActive_End/{0}?openid={1}", activeid, accessToken.Openid));
                    }

                    //-----当没有记录微信用户信息的时候，重新授权操作-----

                    if (weeixinUserInfo == null || string.IsNullOrEmpty(weeixinUserInfo.Openid))
                    {
                        //授权页面
                        var weixinGoUrl = WeiXinHelper.GenConfrimAuthorUrl(HttpUtility.UrlEncode(string.Format("http://www.zmjiudian.com/Active/Weixin_SignupActive_Reg/{0}", activeid)));
                        return Redirect(weixinGoUrl);
                    }
                    else
                    {
                        #region 存储当前用户授权信息

                        var weixinNewUser = new ActiveWeixinUser();
                        weixinNewUser.Openid = accessToken.Openid;
                        weixinNewUser.Nickname = weeixinUserInfo.Nickname;
                        weixinNewUser.Sex = weeixinUserInfo.Sex.ToString();
                        weixinNewUser.Province = weeixinUserInfo.Province;
                        weixinNewUser.City = weeixinUserInfo.City;
                        weixinNewUser.Country = weeixinUserInfo.Country;
                        weixinNewUser.Headimgurl = weeixinUserInfo.Headimgurl;
                        weixinNewUser.Privilege = "";
                        weixinNewUser.Unionid = weeixinUserInfo.Unionid;
                        weixinNewUser.Phone = "";
                        weixinNewUser.ActiveID = activeid;
                        var add = new Weixin().AddActiveWeixinUser(weixinNewUser);

                        #endregion
                    }
                }

                openid = accessToken.Openid;
            }
            else
            {
                //授权页面
                var weixinGoUrl = WeiXinHelper.GenSilenceAuthorUrl(HttpUtility.UrlEncode(string.Format("http://www.zmjiudian.com/Active/Weixin_SignupActive_Reg/{0}", activeid)));
                return Redirect(weixinGoUrl);
            }

            ViewBag.Code = code; //code = "00129702e695ee6927c73d7e99ccb48k";
            ViewBag.State = state;
            ViewBag.Openid = openid;

            return View();
        }

        /// <summary>
        /// 报名方法
        /// </summary>
        /// <param name="openid"></param>
        /// <param name="username"></param>
        /// <param name="phone"></param>
        /// <returns></returns>
        public ActionResult SignUpWeixinActive(string openid = "", string username = "", string phone = "", int activeid = 0)
        {
            var activeDraw = new ActiveWeixinDraw();
            activeDraw.ActiveID = activeid;
            activeDraw.PartnerId = 0;
            activeDraw.Openid = openid;
            activeDraw.UserName = username;
            activeDraw.Phone = phone;
            activeDraw.IsShare = 0;
            activeDraw.ShareTime = DateTime.Now;
            activeDraw.SendFriendCount = 0;
            activeDraw.LastSendFriendTime = DateTime.Now;
            activeDraw.CreateTime = DateTime.Now;

            try
            {
                var add = new Weixin().AddActiveWeixinDraw(activeDraw);
            }
            catch (Exception ex)
            {
                return Json("0", JsonRequestBehavior.AllowGet);
            }

            return Json("1", JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 【报名成功页】
        /// </summary>
        /// <param name="openid"></param>
        /// <param name="code"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public ActionResult Weixin_SignupActive_RegDone(int activeid = 0, string openid = "", string code = "", string state = "")
        {
            ViewBag.IsApp = IsApp();
            ViewBag.Code = code;
            ViewBag.State = state;
            ViewBag.Openid = openid;

            var weixinActiveEntity = GetWeixinActiveEntity(activeid);
            ViewBag.WeixinActiveEntity = weixinActiveEntity;

            //分享页面和分享图片
            ViewBag.ShareTitle = weixinActiveEntity.WeixinSignUpShareTitle;
            //ViewBag.ShareLink = weixinActiveEntity.WeixinSignUpShareLink;
            ViewBag.ShareImgUrl = weixinActiveEntity.WeixinSignUpShareImgUrl;
            ViewBag.TopBannerUrl = weixinActiveEntity.WeixinSignUpTopBannerUrl;
            ViewBag.ShareTip = weixinActiveEntity.WeixinSignUpShareTip;
            ViewBag.ActiveId = activeid;

            var uid = "";
            var weixinUser = new Weixin().GetActiveWeixinUser(openid);
            if (weixinUser != null)
            {
                uid = weixinUser.ID.ToString();
            }
            ViewBag.Uid = uid;

            //分享链接（需生成短链接）
            var shareLink = string.Format("http://www.zmjiudian.com/active/Weixin_SignupActive/{0}12345{1}", activeid, uid);

            shareLink = new Access().GenShortUrl(1, shareLink);
            switch (weixinActiveEntity.WeixinAcountId)
            {
                case 1: { shareLink = new Access().GenShortUrl(1, shareLink); break; }  //www.shangjiudian.com
                case 3: { shareLink = new Access().GenShortUrl(1, shareLink); break; }  //www.shangjiudian.com
                case 4: { shareLink = new Access().GenShortUrl(4, shareLink); break; }  //www.zmjd123.com
                case 5: { shareLink = new Access().GenShortUrl(5, shareLink); break; }  //www.shangclub.com
            }

            ViewBag.ShareLink = shareLink;

            return View();
        }

        /// <summary>
        /// 【分享成功页】
        /// </summary>
        /// <param name="openid"></param>
        /// <param name="code"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public ActionResult Weixin_SignupActive_ShareDone(int activeid = 0, string openid = "", string code = "", string state = "", string shared = "0", string sendfriendd = "0")
        {
            ViewBag.IsApp = IsApp();
            ViewBag.Code = code;
            ViewBag.State = state;

            var weixinActiveEntity = GetWeixinActiveEntity(activeid);
            ViewBag.WeixinActiveEntity = weixinActiveEntity;

            //分享页面和分享图片
            ViewBag.ShareTitle = weixinActiveEntity.WeixinSignUpShareTitle;
            ViewBag.ShareImgUrl = weixinActiveEntity.WeixinSignUpShareImgUrl;
            ViewBag.TopBannerUrl = weixinActiveEntity.WeixinSignUpTopBannerUrl;
            ViewBag.ActiveEndTime = weixinActiveEntity.ActiveEndTime;
            ViewBag.WeixinSignUpResultLink = weixinActiveEntity.WeixinSignUpResultLink ?? "";
            ViewBag.ActiveId = activeid;

            try
            {
                #region 是否更新分享状态

                if (shared == "1")
                {
                    var updateShare = new Weixin().UpdateActiveWeixinDrawIsShare(activeid, openid);
                }

                #endregion

                #region 更新发送给朋友次数

                if (sendfriendd == "1")
                {
                    var updateSendShare = new Weixin().UpdateActiveWeixinDrawSendCount(activeid, openid);
                }

                #endregion
            }
            catch (Exception ex)
            {

            }

            var weixinUser = new Weixin().GetActiveWeixinUser(openid);
            ViewBag.WeixinUser = weixinUser;

            //根据openid查询当前微信用户的报名信息
            var weixinDraw = new Weixin().GetActiveWeixinDraw(activeid, openid);
            ViewBag.WeixinDraw = weixinDraw;

            //获取该微信用户分享的活动页阅读记录
            var shareReadList = new Weixin().GetActiveWeixinShareReadList(activeid, openid) ?? new List<ActiveWeixinShareRead>();
            ViewBag.ShareReadList = shareReadList;

            var readCount = shareReadList.Count;
            ViewBag.ReadCount = readCount;
            ViewBag.OpenId = openid;
            ViewBag.Uid = weixinUser.ID;

            //获取该活动的统计数据
            var statResult = new Weixin().GetWeixinAtvStatResult(activeid);
            ViewBag.StatResult = statResult;

            //分享链接（需生成短链接）
            var shareLink = string.Format("http://www.zmjiudian.com/active/Weixin_SignupActive/{0}12345{1}", activeid, weixinUser.ID);

            shareLink = new Access().GenShortUrl(1, shareLink);
            switch (weixinActiveEntity.WeixinAcountId)
            {
                case 1: { shareLink = new Access().GenShortUrl(1, shareLink); break; }  //www.shangjiudian.com
                case 3: { shareLink = new Access().GenShortUrl(1, shareLink); break; }  //www.shangjiudian.com
                case 4: { shareLink = new Access().GenShortUrl(4, shareLink); break; }  //www.zmjd123.com
                case 5: { shareLink = new Access().GenShortUrl(5, shareLink); break; }  //www.shangclub.com
            }

            ViewBag.ShareLink = shareLink;

            return View();
        }

        /// <summary>
        /// 【邀请更多朋友页】
        /// </summary>
        /// <param name="openid"></param>
        /// <param name="code"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public ActionResult Weixin_SignupActive_SendMoreFd(int activeid = 0, string openid = "", string code = "", string state = "")
        {
            ViewBag.IsApp = IsApp();
            ViewBag.Code = code;
            ViewBag.State = state;
            ViewBag.Openid = openid;

            var weixinActiveEntity = GetWeixinActiveEntity(activeid);
            ViewBag.WeixinActiveEntity = weixinActiveEntity;

            //分享页面和分享图片
            ViewBag.ShareTitle = weixinActiveEntity.WeixinSignUpShareTitle;
            //ViewBag.ShareLink = weixinActiveEntity.WeixinSignUpShareLink;
            ViewBag.ShareImgUrl = weixinActiveEntity.WeixinSignUpShareImgUrl;
            ViewBag.TopBannerUrl = weixinActiveEntity.WeixinSignUpTopBannerUrl;
            ViewBag.ActiveId = activeid;

            var uid = "";
            var weixinUser = new Weixin().GetActiveWeixinUser(openid);
            if (weixinUser != null)
            {
                uid = weixinUser.ID.ToString();
            }
            ViewBag.Uid = uid;

            //分享链接（需生成短链接）
            var shareLink = string.Format("http://www.zmjiudian.com/active/Weixin_SignupActive/{0}12345{1}", activeid, uid);

            shareLink = new Access().GenShortUrl(1, shareLink);
            switch (weixinActiveEntity.WeixinAcountId)
            {
                case 1: { shareLink = new Access().GenShortUrl(1, shareLink); break; }  //www.shangjiudian.com
                case 3: { shareLink = new Access().GenShortUrl(1, shareLink); break; }  //www.shangjiudian.com
                case 4: { shareLink = new Access().GenShortUrl(4, shareLink); break; }  //www.zmjd123.com
                case 5: { shareLink = new Access().GenShortUrl(5, shareLink); break; }  //www.shangclub.com
            }

            ViewBag.ShareLink = shareLink;

            return View();
        }

        /// <summary>
        /// 【活动结束】
        /// </summary>
        /// <param name="activeid"></param>
        /// <returns></returns>
        public ActionResult Weixin_SignupActive_End(int activeid = 0, string openid = "")
        {
            ViewBag.IsApp = IsApp();
            ViewBag.ActiveId = activeid;

            return View();
        }

        /// <summary>
        /// 获取微信活动对象
        /// </summary>
        /// <param name="activeId"></param>
        /// <returns></returns>
        public static WeixinActivityEntity GetWeixinActiveEntity(int activeId)
        {
            try
            {
                var weixinActiveEntity = new Weixin().GetOneWeixinActive(activeId);
                if (weixinActiveEntity != null && weixinActiveEntity.HaveSignUp == 1)
                {
                    return weixinActiveEntity;
                }

                //var weixinActiveList = new Weixin().GetWeixinActives();
                //if (weixinActiveList != null && weixinActiveList.Count > 0 && weixinActiveList.Exists(w => w.ActivityID == activeId))
                //{
                //    var weixinActiveEntity = weixinActiveList.Find(w => w.ActivityID == activeId);
                //    if (weixinActiveEntity != null && weixinActiveEntity.HaveSignUp == 1)
                //    {
                //        return weixinActiveEntity;
                //    }
                //}
            }
            catch (Exception ex)
            {

            }

            return new WeixinActivityEntity();
        }

        /// <summary>
        /// 根据指定微信活动ID，初始相关配置参数
        /// </summary>
        /// <param name="activeId"></param>
        /// <param name="weixinAtvShareTitle"></param>
        /// <param name="weixinAtvShareLink"></param>
        /// <param name="weixinAtvShareImgUrl"></param>
        /// <param name="weixinAtvShareTopBanner"></param>
        public void LoadWeixinActiveConfig(int activeId, ref string weixinAtvShareTitle, ref string weixinAtvShareLink, ref string weixinAtvShareImgUrl, ref string weixinAtvShareTopBanner)
        {
            try
            {
                var weixinActiveEntity = GetWeixinActiveEntity(activeId);

                if (!string.IsNullOrEmpty(weixinActiveEntity.WeixinSignUpShareTitle)) weixinAtvShareTitle = weixinActiveEntity.WeixinSignUpShareTitle;
                if (!string.IsNullOrEmpty(weixinActiveEntity.WeixinSignUpShareLink)) weixinAtvShareLink = weixinActiveEntity.WeixinSignUpShareLink;
                if (!string.IsNullOrEmpty(weixinActiveEntity.WeixinSignUpShareImgUrl)) weixinAtvShareImgUrl = weixinActiveEntity.WeixinSignUpShareImgUrl;
                if (!string.IsNullOrEmpty(weixinActiveEntity.WeixinSignUpTopBannerUrl)) weixinAtvShareTopBanner = weixinActiveEntity.WeixinSignUpTopBannerUrl;
            }
            catch (Exception ex)
            {

            }
        }

        #endregion

        #region 微信报名活动【通用|最新】

        /// <summary>
        /// 【报名页】
        /// </summary>
        /// <param name="partnerid"></param>
        /// <param name="activeid"></param>
        /// <param name="code"></param>
        /// <param name="state"></param>
        /// <param name="ispay">标识已支付完成</param>
        /// <returns></returns>
        public ActionResult Weixin_LuckActive_Reg(int partnerid = 0, int activeid = 0, string code = "", string state = "", int ip = 0)
        {
            //return Redirect(string.Format("/Active/Weixin_LuckActive_End?activeid={0}&openid={1}", 0, 0));

            var openid = "";

            ViewBag.IsApp = IsApp();
            ViewBag.AppId = WeiXinHelper.WeixinServiceAppId;
            ViewBag.AppSecret = WeiXinHelper.WeixinServiceSecret;
            ViewBag.Snsapi_base = WeiXinHelper.Snsapi_base;
            ViewBag.Snsapi_userinfo = WeiXinHelper.Snsapi_userinfo;

            var weixinActiveEntity = GetWeixinActiveEntity(activeid);
            ViewBag.WeixinActiveEntity = weixinActiveEntity;

            //截取当前活动要回复的关键字
            var showKeyword = weixinActiveEntity.ActivityKeyWord.Split('|')[0];
            ViewBag.ShowKeyword = showKeyword;
              
            //分享页面和分享图片
            ViewBag.ShareTitle = weixinActiveEntity.WeixinSignUpShareTitle;
            ViewBag.ShareLink = weixinActiveEntity.WeixinSignUpShareLink;
            ViewBag.ShareImgUrl = weixinActiveEntity.WeixinSignUpShareImgUrl;
            ViewBag.TopBannerUrl = weixinActiveEntity.WeixinSignUpTopBannerUrl;
            ViewBag.ActiveId = activeid;
            ViewBag.PartnerId = partnerid;

            //用户昵称，用作报名的名称使用
            var nickName = "";

            //当前用户是否关注周末酒店 1关注 0没有关注
            var subscribe = "1";
            var unid = "";

            //报名页
            var regPageUrl = string.Format("http://www.shang-ke.cn/wx/active/reg/{0}/{1}", partnerid, activeid);
            if (ip == 200)
            {
                //支付完成
                regPageUrl = string.Format("http://www.shang-ke.cn/wx/active/regpayok/{0}/{1}", partnerid, activeid);
            }

            //tip：目前活动需要服务号的功能，现在只有周末酒店服务号支持，所以暂时统一使用 www.shang-ke.cn 域名 haoy 2018-07-17
            regPageUrl = new Access().GenShortUrl(3, regPageUrl);
            switch (weixinActiveEntity.WeixinAcountId)
            {
                case 1: { regPageUrl = new Access().GenShortUrl(3, regPageUrl); break; }  //www.shang-ke.cn
                case 3: { regPageUrl = new Access().GenShortUrl(3, regPageUrl); break; }  //www.shang-ke.cn
                case 4: { regPageUrl = new Access().GenShortUrl(3, regPageUrl); break; }  //www.shang-ke.cn
                case 5: { regPageUrl = new Access().GenShortUrl(3, regPageUrl); break; }  //www.shang-ke.cn
                case 6: { regPageUrl = new Access().GenShortUrl(3, regPageUrl); break; }  //www.shang-ke.cn
            }

            if (!string.IsNullOrEmpty(code))
            {
                //通过code换取网页授权access_token
                var accessToken = WeiXinHelper.GetWeixinAccessToken(code);
                //var accessToken = new WeixinAccessToken { Openid = "okg6-uBqijun__Frr_dJfV8m6Tjs" };

                //通过access_token拉取用户信息(需scope为 snsapi_userinfo)（这是通过微信api获取的真实用户信息）
                var weeixinUserInfo = WeiXinHelper.GetWeixinUserInfo(accessToken.AccessToken, accessToken.Openid);
                
                try
                {
                    //检查当前用户是否关注当前订阅号
                    //var getJson = "";
                    //var weeixinUserSubscribeInfo = WeiXinHelper.GetWeixinUserSubscribeInfo(accessToken.Unionid, ref getJson);
                    var weeixinUserSubscribeInfo = new Weixin().GetWeixinUserByUnionid("zmjiudian", accessToken.Unionid);
                    if (weeixinUserSubscribeInfo == null)
                    {
                        subscribe = "0";
                    }
                    else if (!string.IsNullOrEmpty(weeixinUserSubscribeInfo.Openid) && !string.IsNullOrEmpty(weeixinUserSubscribeInfo.Unionid))
                    {
                        subscribe = weeixinUserSubscribeInfo.Subscribe.ToString();   
                    }
                }
                catch (Exception ex)
                {
                    subscribe = "-1";
                }

                //先查询当前openid是否已经存储微信用户信息
                var weixinUser = new Weixin().GetActiveWeixinUser(accessToken.Openid);
                if (weixinUser != null && weixinUser.ID > 0)
                {
                    //如果已经存储该用户信息，再获取下用户信息，主要更新其头像和昵称相关
                    try
                    {
                        if (weeixinUserInfo != null)
                        {
                            var weixinNewUser = new ActiveWeixinUser();
                            weixinNewUser.Openid = accessToken.Openid;
                            weixinNewUser.Nickname = weeixinUserInfo.Nickname;
                            weixinNewUser.Sex = weeixinUserInfo.Sex.ToString();
                            weixinNewUser.Province = weeixinUserInfo.Province;
                            weixinNewUser.City = weeixinUserInfo.City;
                            weixinNewUser.Country = weeixinUserInfo.Country;
                            weixinNewUser.Headimgurl = weeixinUserInfo.Headimgurl;
                            weixinNewUser.Privilege = "";
                            weixinNewUser.Unionid = weeixinUserInfo.Unionid;
                            weixinNewUser.Phone = "";
                            weixinNewUser.ActiveID = activeid;
                            var add = new Weixin().AddActiveWeixinUser(weixinNewUser);

                            nickName = weixinNewUser.Nickname;
                        }
                    }
                    catch (Exception ex)
                    {

                    }

                    //检查报名记录
                    var weixinDraw = new Weixin().GetActiveWeixinDraw(activeid, accessToken.Openid);
                    if (weixinDraw != null)
                    {
                        //是否分享？已经分享则直接跳至分享成功页面
                        if (weixinDraw.IsShare == 1)
                        {
                            //如果是“限时抢购”的报名活动，分享成功后，则跳转至抢购活动页面
                            switch (weixinActiveEntity.Type)
                            {
                                case 1:
                                case 3:
                                    {
                                        return Redirect(string.Format("/Active/Weixin_LuckActive_ShareDone/{0}?openid={1}", activeid, accessToken.Openid));
                                    }
                                case 2:
                                    {
                                        return Redirect(GetWxCounponShopUrl(weixinActiveEntity.WeixinSignUpResultLink, weixinDraw.Phone));
                                    }
                            }
                        }
                        //授权，但是没有分享，则跳转至报名待分享页面
                        else
                        {
                            //当前活动已经下线的时候
                            if (weixinActiveEntity != null && weixinActiveEntity.ActiveEndTime < DateTime.Now)
                            {
                                switch (weixinActiveEntity.Type)
                                {
                                    case 1:
                                    case 3:
                                        {
                                            //跳至活动结束页面
                                            return Redirect(string.Format("/Active/Weixin_LuckActive_End?activeid={0}&openid={1}", activeid, accessToken.Openid));
                                        }
                                    case 2:
                                        {
                                            //跳转至关联抢购页
                                            return Redirect(GetWxCounponShopUrl(weixinActiveEntity.WeixinSignUpResultLink, weixinDraw.Phone));
                                        }
                                }
                            }

                            //需要支付报名
                            if (weixinActiveEntity.NeedPaySign == 1)
                            {
                                //报过名 & 并且ip=200，说明当前用户已经支付完成
                                if (weixinDraw.IsPay == 1)
                                {
                                    if (weixinActiveEntity.IsInvite == 1)
                                    {
                                        //邀请制报名后直接跳转至分享成功页面
                                        return Redirect(string.Format("/Active/Weixin_LuckActive_ShareDone/{0}?openid={1}", activeid, accessToken.Openid));
                                    }
                                    else
                                    {
                                        //跳转至报名成功页面
                                        return Redirect(string.Format("/Active/Weixin_LuckActive_RegDone/{0}?openid={1}", activeid, accessToken.Openid));   
                                    }
                                }
                                else
                                {
                                    if (ip == 200)
                                    {
                                        //更新为已完成支付
                                        var updatePayState = new Weixin().UpdateActiveWeixinDrawIsPay(activeid, accessToken.Openid);

                                        //奖励住基金
                                        var uinfo = account.GetUserInfoByMobile(weixinDraw.Phone);
                                        if (uinfo != null && uinfo.UserId > 0)
                                        {
                                            var userFundDetail = new UserFundIncomeDetail();
                                            userFundDetail.RelationUserId = 0;
                                            userFundDetail.OriginOrderId = 0;
                                            userFundDetail.OriginAmount = 0;
                                            userFundDetail.OriCreateTime = DateTime.Now;
                                            userFundDetail.UserId = uinfo.UserId;
                                            userFundDetail.TypeId = 6;
                                            userFundDetail.Label = "1来5往";
                                            userFundDetail.Fund = weixinActiveEntity.ReturnPrice;
                                            userFundDetail.CreateTime = DateTime.Now;
                                            var addUserFund = new Fund().AddUserFund(userFundDetail);
                                        }

                                        //奖励一个抽奖码
                                        var aLuck = new ActiveWeixinLuckCode();
                                        aLuck.ActiveId = activeid;
                                        aLuck.Openid = accessToken.Openid;
                                        aLuck.PartnerId = 0;
                                        aLuck.TagName = "众筹奖励";
                                        aLuck.CreateTime = DateTime.Now;
                                        var genluckcode = new Weixin().AddActiveWeixinLuckCode(aLuck);

                                        //判断当前活动是否为邀请制活动，邀请制活动在报名支付成功后，需奖励推荐者一个 邀请奖励 抽奖码
                                        if (weixinActiveEntity.IsInvite == 1)
                                        {
                                            //获取当前报名者的推荐者
                                            var shareUsers = new Weixin().GetActiveWeixinDrawByReadUser(activeid, accessToken.Openid);
                                            if (shareUsers != null && shareUsers.Count > 0)
                                            {
                                                foreach (var shareItem in shareUsers)
                                                {
                                                    //奖励一个 邀请奖励 抽奖码
                                                    aLuck = new ActiveWeixinLuckCode();
                                                    aLuck.ActiveId = activeid;
                                                    aLuck.Openid = shareItem.Openid;
                                                    aLuck.PartnerId = 0;
                                                    aLuck.TagName = string.Format("邀请奖励-{0}", weixinDraw.UserName);
                                                    aLuck.CreateTime = DateTime.Now;
                                                    genluckcode = new Weixin().AddActiveWeixinLuckCode(aLuck);
                                                }
                                            }

                                            //邀请制报名后直接跳转至分享成功页面
                                            return Redirect(string.Format("/Active/Weixin_LuckActive_ShareDone/{0}?openid={1}", activeid, accessToken.Openid));
                                        }
                                        else 
                                        {
                                            //跳转至报名成功页面
                                            return Redirect(string.Format("/Active/Weixin_LuckActive_RegDone/{0}?openid={1}", activeid, accessToken.Openid));
                                        }
                                    }
                                }
                            }
                            else 
                            {
                                if (weixinActiveEntity.IsInvite == 1)
                                {
                                    //邀请制报名后直接跳转至分享成功页面
                                    return Redirect(string.Format("/Active/Weixin_LuckActive_ShareDone/{0}?openid={1}", activeid, accessToken.Openid));
                                }
                                else
                                {
                                    //if (weixinDraw.PartnerId == 17 || weixinDraw.PartnerId == 44 || weixinDraw.PartnerId == 18 || weixinDraw.PartnerId == 45)
                                    
                                    //1周末酒店 3尚旅游 4尚旅游成都 5美味至尚 6尚旅游北京 11遛娃指南服务号
                                    //如果是 4/5 则需要分享才能报名成功 2017.05.12
                                    if (weixinDraw.PartnerId == 17 || weixinActiveEntity.WeixinAcountId == 4 || weixinActiveEntity.WeixinAcountId == 5 || weixinActiveEntity.WeixinAcountId == 6 || weixinActiveEntity.WeixinAcountId == 11)
                                    {
                                        //跳转至报名成功页面
                                        return Redirect(string.Format("/Active/Weixin_LuckActive_RegDone/{0}?openid={1}", activeid, accessToken.Openid));   
                                    }
                                    else
                                    {
                                        //现在都默认跳转至分享成功页面（不需要在分享） 2017-01-12 haoy
                                        return Redirect(string.Format("/Active/Weixin_LuckActive_ShareDone/{0}?openid={1}", activeid, accessToken.Openid));   
                                    }
                                }
                            }
                        }

                        ViewBag.WeixinDraw = weixinDraw;
                    }
                    //没有报名，则停留在此页进行报名
                    else
                    {
                        //当前活动已经下线的时候
                        if (weixinActiveEntity != null && weixinActiveEntity.ActiveEndTime < DateTime.Now)
                        {
                            switch (weixinActiveEntity.Type)
                            {
                                case 1:
                                case 3:
                                    {
                                        //跳至活动结束页面
                                        return Redirect(string.Format("/Active/Weixin_LuckActive_End?activeid={0}&openid={1}", activeid, accessToken.Openid));
                                    }
                                case 2:
                                    {
                                        //跳转至关联抢购页
                                        return Redirect(GetWxCounponShopUrl(weixinActiveEntity.WeixinSignUpResultLink, weixinDraw.Phone));
                                    }
                            }
                        }
                    }
                }
                else
                {
                    //当前活动已经下线的时候
                    if (weixinActiveEntity != null && weixinActiveEntity.ActiveEndTime < DateTime.Now)
                    {
                        switch (weixinActiveEntity.Type)
                        {
                            case 1:
                            case 3:
                                {
                                    //跳至活动结束页面
                                    return Redirect(string.Format("/Active/Weixin_LuckActive_End?activeid={0}&openid={1}", activeid, accessToken.Openid));
                                }
                            case 2:
                                {
                                    //跳转至关联抢购页
                                    return Redirect(GetWxCounponShopUrl(weixinActiveEntity.WeixinSignUpResultLink, ""));
                                }
                        }
                    }

                    //-----当没有记录微信用户信息的时候，重新授权操作-----

                    if (weeixinUserInfo == null || string.IsNullOrEmpty(weeixinUserInfo.Openid))
                    {
                        //授权页面
                        var weixinGoUrl = WeiXinHelper.GenConfrimAuthorUrl(HttpUtility.UrlEncode(regPageUrl));
                        return Redirect(weixinGoUrl);
                    }
                    else
                    {
                        #region 存储当前用户授权信息

                        var weixinNewUser = new ActiveWeixinUser();
                        weixinNewUser.Openid = accessToken.Openid;
                        weixinNewUser.Nickname = weeixinUserInfo.Nickname;
                        weixinNewUser.Sex = weeixinUserInfo.Sex.ToString();
                        weixinNewUser.Province = weeixinUserInfo.Province;
                        weixinNewUser.City = weeixinUserInfo.City;
                        weixinNewUser.Country = weeixinUserInfo.Country;
                        weixinNewUser.Headimgurl = weeixinUserInfo.Headimgurl;
                        weixinNewUser.Privilege = "";
                        weixinNewUser.Unionid = weeixinUserInfo.Unionid;
                        weixinNewUser.Phone = "";
                        weixinNewUser.ActiveID = activeid;
                        var add = new Weixin().AddActiveWeixinUser(weixinNewUser);

                        nickName = weixinNewUser.Nickname;

                        #endregion
                    }
                }

                unid = accessToken.Unionid;
                openid = accessToken.Openid;
            }
            else
            {
                //授权页面
                var weixinGoUrl = WeiXinHelper.GenSilenceAuthorUrl(HttpUtility.UrlEncode(regPageUrl));
                return Redirect(weixinGoUrl);
            }

            ViewBag.Subscribe = subscribe;
            ViewBag.NickName = nickName;
            ViewBag.Code = code; //code = "00129702e695ee6927c73d7e99ccb48k";
            ViewBag.State = state;
            ViewBag.Openid = openid;
            ViewBag.Unid = unid;

            return View();
        }

        /// <summary>
        /// 报名方法
        /// </summary>
        /// <param name="openid"></param>
        /// <param name="username"></param>
        /// <param name="phone"></param>
        /// <param name="activeid"></param>
        /// <returns></returns>
        public ActionResult SignUpWeixinLuckActive(string openid = "", string unionid = "", string username = "", string phone = "", int activeid = 0, int partnerid = 0, int needpaysign = 0, decimal payprice = 1, decimal returnprice = 5)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();

            #region 注释

            //暂时将强制关注的处理放在分享成功查看抽奖码的页面 2016-05-29 haoy
            ////检查当前用户是否已经关注zmjiudian订阅号
            //if (!string.IsNullOrEmpty(unionid))
            //{
            //    try
            //    {
            //        var weeixinUserSubscribeInfo = new Weixin().GetWeixinUserByUnionid("zmjiudian", unionid);
            //        if (weeixinUserSubscribeInfo != null && !string.IsNullOrEmpty(weeixinUserSubscribeInfo.Openid) && !string.IsNullOrEmpty(weeixinUserSubscribeInfo.Unionid))
            //        {
            //            if (weeixinUserSubscribeInfo.Subscribe == 0)
            //            {
            //                return Json("-1", JsonRequestBehavior.AllowGet);
            //            }
            //        }   
            //    }
            //    catch (Exception)
            //    {
                    
            //    }
            //}

            #endregion

            //检查报名记录
            var weixinDraw = new Weixin().GetActiveWeixinDraw(activeid, openid);
            
            //已报名
            if (weixinDraw != null)
            {
                if (needpaysign < 1 || (needpaysign == 1 && weixinDraw.IsPay == 1))
                {
                    dict["state"] = "1";
                    dict["payurl"] = "";
                    return Json(dict, JsonRequestBehavior.AllowGet);
                }
            }
            //未报名
            else
            {
                var activeDraw = new ActiveWeixinDraw();
                activeDraw.ActiveID = activeid;
                activeDraw.PartnerId = partnerid;
                activeDraw.Openid = openid;
                activeDraw.UserName = username;
                activeDraw.Phone = phone;
                //activeDraw.IsShare = 0;

                /*
                 * 17 尚旅游
                 * 18 尚旅游成都
                 * 45 美味至尚
                 * 47 尚旅游北京
                 * 48 遛娃指南 订阅号
                 * 50 遛娃指南 服务号
                 */
                if (partnerid == 17 || partnerid == 18 || partnerid == 45 || partnerid == 47 || partnerid == 49 || partnerid == 50) //(partnerid == 17 || partnerid == 44))// || partnerid == 18 || partnerid == 45)
                {
                    activeDraw.IsShare = 0;
                }
                else
                {
                    //只要不是通过尚旅入口报名，现在填写手机号后就认为报名成功了，不需要再分享一次了（2017-01-12 haoy）
                    activeDraw.IsShare = 1;
                }

                activeDraw.ShareTime = DateTime.Now;
                activeDraw.SendFriendCount = 0;
                activeDraw.LastSendFriendTime = DateTime.Now;
                activeDraw.IsPay = 0;
                activeDraw.PayTime = DateTime.Now;
                activeDraw.CreateTime = DateTime.Now;

                try
                {
                    var add = new Weixin().AddActiveWeixinDraw(activeDraw);

                    //现在报名成功后即默认奖励一个抽奖码
                    var aLuck = new ActiveWeixinLuckCode();
                    aLuck.ActiveId = activeid;
                    aLuck.Openid = openid;
                    aLuck.PartnerId = partnerid;
                    aLuck.TagName = "报名奖励";
                    aLuck.CreateTime = DateTime.Now;
                    var genluckcode = new Weixin().PublishWeixinLuckCodeTask(aLuck);
                }
                catch (Exception ex)
                {
                    dict["state"] = "0";
                    dict["payurl"] = "";
                    return Json(dict, JsonRequestBehavior.AllowGet);
                }
            }
            
            //需要支付报名
            if (needpaysign == 1)
            {
                //需要支付报名的活动，当前用户未报名或者报名未支付
                HJD.HotelPrice.Contract.DataContract.Order.CommOrderEntity orderEntity = new HJD.HotelPrice.Contract.DataContract.Order.CommOrderEntity();
                orderEntity.CustomID = activeid;
                orderEntity.Name = "度假众筹";
                orderEntity.OpNotice = "";
                orderEntity.PhoneNum = phone;
                orderEntity.Price = (int)payprice;
                orderEntity.TypeID = Convert.ToInt32(CommOrderType.Active_OneToFive);
                orderEntity.State = 1;
                orderEntity.CreateTime = DateTime.Now;

                //创建订单
                var orderId = Order.AddCommOrders(orderEntity);

                dict["state"] = "2";
                dict["payurl"] = Url.Action("Pay", "Order", new { order = orderId, payChannels = "tenpay" });
                return Json(dict, JsonRequestBehavior.AllowGet);
            }

            dict["state"] = "1";
            dict["payurl"] = "";
            return Json(dict, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 【分享页】
        /// </summary>
        /// <param name="aidsharer"></param>
        /// <param name="code"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public ActionResult Weixin_LuckActive(string aidsharer = "0123450", string code = "", string state = "")
        {

            //return Redirect(string.Format("/Active/Weixin_LuckActive_End?activeid={0}&openid={1}", 0, 0));

            ViewBag.IsApp = IsApp();

            var activeid = "0";
            var sharer = "0";
            if (!string.IsNullOrEmpty(aidsharer) && aidsharer.Contains("12345"))
            {
                activeid = Regex.Split(aidsharer, "12345")[0];
                sharer = Regex.Split(aidsharer, "12345")[1];
            }

            var weixinActiveEntity = GetWeixinActiveEntity(Convert.ToInt32(activeid));

            //分享页面和分享图片
            ViewBag.ShareTitle = weixinActiveEntity.WeixinSignUpShareTitle;
            ViewBag.ShareLink = weixinActiveEntity.WeixinSignUpShareLink;
            ViewBag.ShareImgUrl = weixinActiveEntity.WeixinSignUpShareImgUrl;
            ViewBag.TopBannerUrl = weixinActiveEntity.WeixinSignUpTopBannerUrl;
            ViewBag.ActiveId = activeid;

            if (!string.IsNullOrEmpty(code))
            {
                //首先拿到分享者的openid
                var shareWeixinUser = new Weixin().GetActiveWeixinUserById(Convert.ToInt32(sharer));
                if (shareWeixinUser != null && weixinActiveEntity != null && weixinActiveEntity.ActiveEndTime > DateTime.Now)
                {
                    //通过code换取网页授权access_token
                    var accessToken = WeiXinHelper.GetWeixinAccessToken(code);

                    if (shareWeixinUser.Openid.ToLower().Trim() != accessToken.Openid.ToLower().Trim())
                    {
                        //有了分享者openid，又拿到了当前阅读者的openid，则插入一条阅读记录
                        var activeRead = new ActiveWeixinShareRead();
                        activeRead.ActiveID = Convert.ToInt32(activeid);
                        activeRead.SharerOpenid = shareWeixinUser.Openid;
                        activeRead.Openid = accessToken.Openid;
                        activeRead.LastReadTime = DateTime.Now;
                        activeRead.ReadCount = 1;

                        //add
                        var add = new Weixin().AddActiveWeixinShareRead_Luck(activeRead);

                        //邀请制活动不需要分享奖励
                        if (weixinActiveEntity.IsInvite == 0)
                        {
                            //阅读记录完成，触发生成抽奖码队列任务
                            var aLuck = new ActiveWeixinLuckCode();
                            aLuck.ActiveId = activeRead.ActiveID;
                            aLuck.Openid = activeRead.SharerOpenid;
                            aLuck.PartnerId = 0;
                            aLuck.TagName = "分享奖励";
                            aLuck.CreateTime = DateTime.Now;
                            var genluckcode = new Weixin().PublishWeixinLuckCodeTask(aLuck);   
                        }
                    }
                }
            }
            else
            {
                //授权页面
                var weixinGoUrl = WeiXinHelper.GenSilenceAuthorUrl(HttpUtility.UrlEncode(string.Format("http://www.shang-ke.cn/active/Weixin_LuckActive/{0}12345{1}", activeid, sharer)));
                //switch (weixinActiveEntity.WeixinAcountId)
                //{
                //    case 1: { weixinGoUrl = new Access().GenShortUrl(1, weixinGoUrl); break; }  //www.shang-ke.cn
                //    case 3: { weixinGoUrl = new Access().GenShortUrl(1, weixinGoUrl); break; }  //www.shang-ke.cn
                //    case 4: { weixinGoUrl = new Access().GenShortUrl(4, weixinGoUrl); break; }  //www.shang-ke.cn
                //    case 5: { weixinGoUrl = new Access().GenShortUrl(5, weixinGoUrl); break; }  //www.shang-ke.cn
                //}
                return Redirect(weixinGoUrl);
            }

            //走到这里，说明已经静默授权了，则直接跳到活动页
            return Redirect(weixinActiveEntity.WeixinSignUpShareLink);
        }

        /// <summary>
        /// 【报名成功页】
        /// </summary>
        /// <param name="activeid"></param>
        /// <param name="openid"></param>
        /// <param name="code"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public ActionResult Weixin_LuckActive_RegDone(int activeid = 0, string openid = "", string code = "", string state = "")
        {

            //return Redirect(string.Format("/Active/Weixin_LuckActive_End?activeid={0}&openid={1}", 0, 0));

            ViewBag.IsApp = IsApp();
            ViewBag.Code = code;
            ViewBag.State = state;
            ViewBag.Openid = openid;

            var weixinActiveEntity = GetWeixinActiveEntity(activeid);
            ViewBag.WeixinActiveEntity = weixinActiveEntity;

            //分享页面和分享图片
            ViewBag.ShareTitle = weixinActiveEntity.WeixinSignUpShareTitle;
            //ViewBag.ShareLink = weixinActiveEntity.WeixinSignUpShareLink;
            ViewBag.ShareImgUrl = weixinActiveEntity.WeixinSignUpShareImgUrl;
            ViewBag.TopBannerUrl = weixinActiveEntity.WeixinSignUpTopBannerUrl;
            ViewBag.ShareTip = weixinActiveEntity.WeixinSignUpShareTip;
            ViewBag.ActiveId = activeid;

            var uid = "";
            var weixinUser = new Weixin().GetActiveWeixinUser(openid);
            if (weixinUser != null)
            {
                uid = weixinUser.ID.ToString();
            }
            ViewBag.Uid = uid;

            //分享链接（需生成短链接）
            var shareLink = string.Format("http://www.shang-ke.cn/active/Weixin_LuckActive/{0}12345{1}", activeid, uid);

            //tip：目前活动需要服务号的功能，现在只有周末酒店服务号支持，所以暂时统一使用 www.shang-ke.cn，www.shangjiudian.com被屏蔽分享了... 域名 haoy 2018-01-16
            shareLink = new Access().GenShortUrl(1, shareLink);
            switch (weixinActiveEntity.WeixinAcountId)
            {
                case 1: { shareLink = new Access().GenShortUrl(1, shareLink); break; }  //www.shang-ke.cn
                case 3: { shareLink = new Access().GenShortUrl(1, shareLink); break; }  //www.shang-ke.cn
                case 4: { shareLink = new Access().GenShortUrl(1, shareLink); break; }  //www.shang-ke.cn
                case 5: { shareLink = new Access().GenShortUrl(1, shareLink); break; }  //www.shang-ke.cn
                case 6: { shareLink = new Access().GenShortUrl(1, shareLink); break; }  //www.shang-ke.cn
                case 11: { shareLink = new Access().GenShortUrl(1, shareLink); break; }  //www.shang-ke.cn
            }

            ViewBag.ShareLink = shareLink;

            return View();
        }

        /// <summary>
        /// 【分享成功页面】
        /// </summary>
        /// <param name="activeid"></param>
        /// <param name="openid"></param>
        /// <param name="code"></param>
        /// <param name="state"></param>
        /// <param name="shared"></param>
        /// <param name="sendfriendd"></param>
        /// <returns></returns>
        public ActionResult Weixin_LuckActive_ShareDone(int activeid = 0, string openid = "", string code = "", string state = "", string shared = "0", string sendfriendd = "0", int swfund = 0)
        {

            //return Redirect(string.Format("/Active/Weixin_LuckActive_End?activeid={0}&openid={1}", 0, 0));

            ViewBag.IsApp = IsApp();
            ViewBag.Code = code;
            ViewBag.State = state;

            var weixinActiveEntity = GetWeixinActiveEntity(activeid);
            ViewBag.WeixinActiveEntity = weixinActiveEntity;

            //支付报名活动
            if (weixinActiveEntity.NeedPaySign == 1)
            {
                ViewBag.ShowFundAlter = swfund == 1;
            }
            else
            {
                ViewBag.ShowFundAlter = false;   
            }

            //截取当前活动要回复的关键字
            var showKeyword = weixinActiveEntity.ActivityKeyWord.Split('|')[0];
            ViewBag.ShowKeyword = showKeyword;

            //分享页面和分享图片
            ViewBag.ShareTitle = weixinActiveEntity.WeixinSignUpShareTitle;
            ViewBag.ShareImgUrl = weixinActiveEntity.WeixinSignUpShareImgUrl;
            ViewBag.TopBannerUrl = weixinActiveEntity.WeixinSignUpTopBannerUrl;
            ViewBag.ActiveEndTime = weixinActiveEntity.ActiveEndTime;
            ViewBag.WeixinSignUpResultLink = weixinActiveEntity.WeixinSignUpResultLink ?? "";
            ViewBag.ActiveId = activeid;

            //根据openid查询当前微信用户的报名信息
            var weixinDraw = new Weixin().GetActiveWeixinDraw(activeid, openid);
            ViewBag.WeixinDraw = weixinDraw;

            try
            {
                #region 是否更新分享状态

                if (shared == "1")
                {
                    var updateShare = new Weixin().UpdateActiveWeixinDrawIsShare(activeid, openid);

                    //分享成功后，判断如果是“限时抢购”活动报名，则跳转至抢购页
                    if (weixinActiveEntity.Type == 2)
                    {
                        if (weixinDraw != null)
                        {
                            return Redirect(GetWxCounponShopUrl(weixinActiveEntity.WeixinSignUpResultLink, weixinDraw.Phone, true));
                        }
                    }
                }

                #endregion

                #region 更新发送给朋友次数

                if (sendfriendd == "1")
                {
                    var updateSendShare = new Weixin().UpdateActiveWeixinDrawSendCount(activeid, openid);
                }

                #endregion
            }
            catch (Exception ex)
            {

            }

            var weixinUser = new Weixin().GetActiveWeixinUser(openid);
            ViewBag.WeixinUser = weixinUser;

            #region 当前用户是否关注周末酒店 1关注 0没有关注

            var subscribe = "1";
            if (false && (weixinActiveEntity.WeixinAcountId == 1 || weixinActiveEntity.WeixinAcountId == 2) && weixinDraw.PartnerId != 42)
            {
                try
                {
                    //检查当前用户是否关注当前订阅号
                    var weeixinUserSubscribeInfo = new Weixin().GetWeixinUserByUnionid("zmjiudian", weixinUser.Unionid);
                    if (weeixinUserSubscribeInfo == null)
                    {
                        subscribe = "0";

                        //如果用户没有记录，则增加一条记录(可能总量下载关注用户时有遗漏)
                        //try
                        //{
                        //    var w = new WeixinUser 
                        //    { 
                        //        Openid = openid, 
                        //        Unionid = weixinUser.Unionid, 
                        //        Nickname = weixinUser.Nickname, 
                        //        Sex = weixinUser.Sex, 
                        //        Province = weixinUser.Province, 
                        //        City = weixinUser.City, 
                        //        Country = weixinUser.Country, 
                        //        Headimgurl = weixinUser.Headimgurl, 
                        //        Privilege = weixinUser.Privilege, 
                        //        Phone = weixinUser.Phone,
                        //        Remark = "",
                        //        GroupId = "0",
                        //        Subscribe = 1,
                        //        WeixinAcount = "zmjiudian", 
                        //        Language = "zh_CN",
                        //        SubscribeTime = DateTime.Now, 
                        //        CreateTime = DateTime.Now 
                        //    };
                        //    var update = new Weixin().UpdateWeixinUserSubscribe(w);
                        //}
                        //catch (Exception ex)
                        //{

                        //}
                    }
                    else if (!string.IsNullOrEmpty(weeixinUserSubscribeInfo.Openid) && !string.IsNullOrEmpty(weeixinUserSubscribeInfo.Unionid))
                    {
                        subscribe = weeixinUserSubscribeInfo.Subscribe.ToString();
                    }
                }
                catch (Exception ex)
                {
                    subscribe = "-1";
                }
            }
            ViewBag.Subscribe = subscribe;

            #endregion

            //获取该微信用户分享的活动页阅读记录
            var shareReadList = new Weixin().GetActiveWeixinShareReadList(activeid, openid) ?? new List<ActiveWeixinShareRead>();
            ViewBag.ShareReadList = shareReadList;

            //获取该微信用户的获奖码记录
            var luckCodeList = new Weixin().GetActiveWeixinLuckCodeInfo(activeid, openid) ?? new List<ActiveWeixinLuckCode>();
            ViewBag.LuckCodeList = luckCodeList;

            //获取当前活动关联的导粉数据
            var havePartner = false;
            var relPartnerIds = weixinActiveEntity.RelPartnerIds;
            if (!string.IsNullOrEmpty(relPartnerIds))
            {
                try
                {
                    var relPidsArray = relPartnerIds.Split(',').ToList().Where(_i => _i != "7").ToList();
                    havePartner = relPidsArray.Count > 0;
                }
                catch (Exception)
                {
                    
                }
            }
            ViewBag.HavePartner = havePartner;

            var readCount = shareReadList.Count;
            ViewBag.ReadCount = readCount;
            ViewBag.OpenId = openid;
            ViewBag.Uid = weixinUser.ID;

            //获取该活动的统计数据
            var statResult = new Weixin().GetActiveWeixinLuckCodeCount(activeid);
            ViewBag.StatResult = statResult;

            //获取自定义随机海报背景图片
            ViewBag.PosterBgPic = GetLuckPosterBgPicByActiveId(weixinActiveEntity.ActivityID);

            //翻倍卡海报随机文案
            ViewBag.PosterSloganPic = GetLuckPosterSloganPic();

            //随机得出当前用户是否要弹出翻倍卡(如果之前弹过，通过js缓存来判断，这里只得出本次获得与否)
            var showThreePosterCord = false;

            //不同活动的自定义功能处理
            switch (weixinActiveEntity.ActivityID)
            {
                    //君澜团建免费住
                case 829:
                    {

                        showThreePosterCord = false;
                        break;

                        //一个1一个0，50%概率
                        var _luckShowList = new List<int>();
                        _luckShowList.Add(1);
                        _luckShowList.Add(1);
                        _luckShowList.Add(1);
                        _luckShowList.Add(1);
                        _luckShowList.Add(1);
                        _luckShowList.Add(1);
                        _luckShowList.Add(0);
                        //_luckShowList.Add(0);
                        //_luckShowList.Add(0);
                        //_luckShowList.Add(0);
                        //_luckShowList.Add(0);

                        //按照guid打乱
                        _luckShowList = _luckShowList.OrderBy(_ => Guid.NewGuid()).ToList();

                        //再随机取出一个
                        Random _ran = new Random();
                        int _ranNum = _ran.Next(1, _luckShowList.Count);

                        //选出值
                        var _selectItem = _luckShowList[_ranNum-1];//_luckShowList[0]; //
                        if (_selectItem > 0)
                        {
                            showThreePosterCord = true;
                        }

                        break;
                    }
            }

            //是否弹出翻倍卡
            ViewBag.ShowThreePosterCord = showThreePosterCord;

            //分享链接（需生成短链接）
            var shareLink = string.Format("http://www.shang-ke.cn/active/Weixin_LuckActive/{0}12345{1}", activeid, weixinUser.ID);

            //tip：目前活动需要服务号的功能，现在只有周末酒店服务号支持，所以暂时统一使用 www.shang-ke.cn，www.shangjiudian.com被屏蔽分享了...域名 haoy 2018-01-16
            shareLink = new Access().GenShortUrl(1, shareLink);
            switch (weixinActiveEntity.WeixinAcountId)
            {
                case 1: { shareLink = new Access().GenShortUrl(1, shareLink); break; }  //www.shang-ke.cn
                case 3: { shareLink = new Access().GenShortUrl(1, shareLink); break; }  //www.shang-ke.cn
                case 4: { shareLink = new Access().GenShortUrl(1, shareLink); break; }  //www.shang-ke.cn
                case 5: { shareLink = new Access().GenShortUrl(1, shareLink); break; }  //www.shang-ke.cn
                case 6: { shareLink = new Access().GenShortUrl(1, shareLink); break; }  //www.shang-ke.cn
                case 11: { shareLink = new Access().GenShortUrl(1, shareLink); break; }  //www.shang-ke.cn
            }

            ViewBag.ShareLink = shareLink;

            return View();
        }

        /// <summary>
        /// 随机获取一个免费住翻倍卡海报slogan背景图
        /// </summary>
        /// <returns></returns>
        private string GetLuckPosterSloganPic()
        {
            var _pic = "";

            var _list = new List<string>();
            _list.Add("http://whphoto.b0.upaiyun.com/118aK1e5_jupiter");    //freeluck-一场有品质还免费的outing.png
            _list.Add("http://whphoto.b0.upaiyun.com/118aK1e4_jupiter");    //freeluck-要想员工卖力点.png
            _list.Add("http://whphoto.b0.upaiyun.com/118aK1e1_jupiter");    //freeluck-拼命工作放肆玩耍.png
            _list.Add("http://whphoto.b0.upaiyun.com/118aK1e2_jupiter");    //freeluck-团建搞得好员工不跳槽.png
            _list.Add("http://whphoto.b0.upaiyun.com/118aK1e0_jupiter");    //freeluck-年会办得好来年生意好.png
            _list.Add("http://whphoto.b0.upaiyun.com/118aK1e3_jupiter");    //freeluck-要想财神跟你跑.png

            //随机一个
            _pic = _list.OrderBy(_ => Guid.NewGuid()).ToList().First();

            return _pic;
        }

        /// <summary>
        /// 随机获取一个分享海报背景图
        /// </summary>
        /// <returns></returns>
        private string GetLuckPosterBgPicByActiveId(int activeId)
        {
            var _pic = "";

            var _list = new List<string>();

            switch (activeId)
            {
                    //猪鼓励活动
                case 842:
                    {
                        _list.Add("http://whphoto.b0.upaiyun.com/119HQ6J4_jupiter");
                        _list.Add("http://whphoto.b0.upaiyun.com/119HQ6J0_jupiter");
                        _list.Add("http://whphoto.b0.upaiyun.com/119HQ6J5_jupiter");
                        _list.Add("http://whphoto.b0.upaiyun.com/119HQ6J3_jupiter");
                        _list.Add("http://whphoto.b0.upaiyun.com/119HQ6J1_jupiter");
                        _list.Add("http://whphoto.b0.upaiyun.com/119HQ6J2_jupiter");
                        break;
                    }
            }

            //随机一个
            if (_list.Count > 0)
            {
                _pic = _list.OrderBy(_ => Guid.NewGuid()).ToList().First();
            }

            return _pic;
        }

        /// <summary>
        /// 根据原始抢购链接，追加userid参数后返回
        /// </summary>
        /// <param name="link"></param>
        /// <param name="phone"></param>
        /// <returns></returns>
        private string GetWxCounponShopUrl(string link, string phone, bool first = false)
        {
            var shopLink = link;
            var _userInfo = account.GetUserInfoByMobile(phone);
            if (_userInfo != null)
            {
                if (shopLink.Contains("?")) shopLink = shopLink + "&userid=" + _userInfo.UserId;
                else shopLink = shopLink + "?userid=" + _userInfo.UserId;

                if (first)
                {
                    shopLink = shopLink + "&wxsignfirst=1";
                }
            }
            return shopLink;
        }

        /// <summary>
        /// 【微信合作伙伴列表页】
        /// </summary>
        /// <param name="activeid"></param>
        /// <param name="openid"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public ActionResult Weixin_LuckActive_SharePartner(int activeid = 0, string openid = "", string code = "")
        {
            //return Redirect(string.Format("/Active/Weixin_LuckActive_End?activeid={0}&openid={1}", 0, 0));   

            ViewBag.IsApp = IsApp();
            ViewBag.ActiveId = activeid;
            
            //获取当前活动的详细信息
            var weixinActiveEntity = GetWeixinActiveEntity(activeid);
            ViewBag.WeixinActiveEntity = weixinActiveEntity;
            
            #region 如果没有openid，则通过静默授权获取

            if (string.IsNullOrEmpty(openid))
            {
                if (!string.IsNullOrEmpty(code))
                {
                    //通过code换取网页授权access_token
                    var accessToken = WeiXinHelper.GetWeixinAccessToken(code);
                    if (accessToken != null)
                    {
                        openid = accessToken.Openid;
                    }
                }
                else
                {
                    //授权页面
                    var weixinGoUrl = WeiXinHelper.GenSilenceAuthorUrl(HttpUtility.UrlEncode(string.Format("http://www.shang-ke.cn/active/Weixin_LuckActive_SharePartner/{0}", activeid)));
                    //switch (weixinActiveEntity.WeixinAcountId)
                    //{
                    //    case 1: { weixinGoUrl = new Access().GenShortUrl(1, weixinGoUrl); break; }  //www.shang-ke.cn
                    //    case 3: { weixinGoUrl = new Access().GenShortUrl(1, weixinGoUrl); break; }  //www.shang-ke.cn
                    //    case 4: { weixinGoUrl = new Access().GenShortUrl(4, weixinGoUrl); break; }  //www.shang-ke.cn
                    //    case 5: { weixinGoUrl = new Access().GenShortUrl(5, weixinGoUrl); break; }  //www.shang-ke.cn
                    //}
                    return Redirect(weixinGoUrl);
                }   
            }

            #endregion

            //截取当前活动要回复的关键字
            var showKeyword = weixinActiveEntity.ActivityKeyWord.Split('|')[0];
            ViewBag.ShowKeyword = showKeyword;

            //获取所有的微信合作伙伴信息
            var allPartnerList = new Weixin().GetAllWeixinPartners();
            ViewBag.AllPartnerList = allPartnerList;

            //筛选出当前活动关联的微信合作伙伴
            var relPartnerList = new List<ActiveWeixinPartner>();
            var relPartnerIds = weixinActiveEntity.RelPartnerIds;
            if (!string.IsNullOrEmpty(relPartnerIds))
            {
                var relPidsArray = relPartnerIds.Split(',').ToList();
                relPartnerList = allPartnerList.Where(p => relPidsArray.Contains(p.Id.ToString())).ToList();
            }

            switch (weixinActiveEntity.WeixinAcountId)
            {
                case 1: 
                case 2:
                    {
                        //将订阅号的顺序随机显示 (过滤掉zmjiudian，因为zmjiudian是报名强制关注的 2016-05-13 haoy)
                        relPartnerList = relPartnerList.Where(rp => rp.PartnerCode.ToLower().Trim() != "zmjiudian").ToList();   
                        break;
                    }
                case 3:
                    {
                        //如果当前活动是尚旅游主办的，则过滤掉iloveshanglvyou微信号二维码
                        relPartnerList = relPartnerList.Where(rp => rp.PartnerCode.ToLower().Trim() != "iloveshanglvyou").ToList();   
                        break;
                    }
            }

            //随机排序
            relPartnerList = relPartnerList.OrderBy(x => Guid.NewGuid()).ToList();
            
            ViewBag.RelPartnerList = relPartnerList;

            //获取当前用户openid的所有获奖数据（用于筛选出哪些合作号已经领取）
            var luckCodeList = new Weixin().GetActiveWeixinLuckCodeInfo(activeid, openid) ?? new List<ActiveWeixinLuckCode>();
            ViewBag.LuckCodeList = luckCodeList;

            //筛选出已经参与过的合作号
            var hasPartnerIdList = luckCodeList.Select(l => l.PartnerId).Distinct().ToList();
            ViewBag.HasPartnerIdList = hasPartnerIdList;

            return View();
        }

        /// <summary>
        /// 【微信活动酒店列表页 - 指定分组】
        /// </summary>
        /// <param name="partnerid"></param>
        /// <param name="groupid"></param>
        /// <param name="openid"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public ActionResult Weixin_LuckActive_HotelList(int partnerid = 0, int groupid = 0, string openid = "", string code = "")
        {

            return Redirect(string.Format("/Active/Weixin_LuckActive_End?activeid={0}&openid={1}", 0, 0));

            ViewBag.IsApp = IsApp();
            ViewBag.GroupId = groupid;
            ViewBag.PartnerId = partnerid;

            #region 如果没有openid，则通过静默授权获取

            if (string.IsNullOrEmpty(openid))
            {
                if (!string.IsNullOrEmpty(code))
                {
                    //通过code换取网页授权access_token
                    var accessToken = WeiXinHelper.GetWeixinAccessToken(code);
                    if (accessToken != null)
                    {
                        openid = accessToken.Openid;
                    }
                }
                else
                {
                    //授权页面
                    var weixinGoUrl = WeiXinHelper.GenSilenceAuthorUrl(HttpUtility.UrlEncode(string.Format("http://www.zmjiudian.com/wx/active/hotellist/{0}/{1}", partnerid, groupid)));
                    return Redirect(weixinGoUrl);
                }
            }

            #endregion

            //获取指定组下面的微信酒店列表
            var groupInfo = Hotel.GetActiveRuleList(groupid);

            //将所有活动分成 未报名 已报名 两组
            var listtop0 = new List<ActiveRuleExEntity>();
            var list0 = new List<ActiveRuleExEntity>();
            var listtop1 = new List<ActiveRuleExEntity>();
            var list1 = new List<ActiveRuleExEntity>();

            if (groupInfo != null && groupInfo.ActiveRuleEx != null && groupInfo.ActiveRuleEx.Count > 0)
            {
                for (int i = 0; i < groupInfo.ActiveRuleEx.Count; i++)
                {
                    var _ex = groupInfo.ActiveRuleEx[i];

                    //获取当前用户该活动的报名状态
                    var _weixinDraw = new Weixin().GetActiveWeixinDraw(_ex.ActiveId, openid);
                    if (_weixinDraw != null)
                    {
                        list1.Add(_ex);
                    }
                    else
                    {
                        list0.Add(_ex);
                    }
                }

                //未参与

                //获取置顶的酒店
                listtop0 = list0.Where(ae => ae.OrderNum < 0).OrderBy(ae => ae.OrderNum).ToList();
                list0 = list0.Where(ae => ae.OrderNum >= 0).OrderBy(x => Guid.NewGuid()).ToList();
                listtop0.AddRange(list0);
                list0 = listtop0;

                //已参与

                //获取置顶的酒店
                listtop1 = list1.Where(ae => ae.OrderNum < 0).OrderBy(ae => ae.OrderNum).ToList();
                list1 = list1.Where(ae => ae.OrderNum >= 0).OrderBy(x => Guid.NewGuid()).ToList();
                listtop1.AddRange(list1);
                list1 = listtop1;
            }

            ViewBag.GroupInfo = groupInfo;
            ViewBag.List0 = list0;
            ViewBag.List1 = list1;

            var weixinActiveEntity = GetWeixinActiveEntity((groupInfo != null ? groupInfo.ActiveId : 0));
            ViewBag.WeixinActiveEntity = weixinActiveEntity;

            //分享页面和分享图片
            ViewBag.ShareTitle = weixinActiveEntity.WeixinSignUpShareTitle;
            ViewBag.ShareImgUrl = weixinActiveEntity.WeixinSignUpShareImgUrl;

            //分享链接（需生成短链接）
            var shareLink = string.Format("http://www.zmjiudian.com/wx/active/hotellist/{0}/{1}", partnerid, groupid);
            if (groupInfo != null && groupInfo.ActiveId > 0)
            {
                shareLink = string.Format("http://www.zmjiudian.com/active/Weixin_LuckActive/{0}12345{1}", groupInfo.ActiveId, "2");
            }

            //tip：目前活动需要服务号的功能，现在只有周末酒店服务号支持，所以暂时统一使用 www.shangjiudian.com 域名 haoy 2017-07-17
            shareLink = new Access().GenShortUrl(1, shareLink);
            switch (weixinActiveEntity.WeixinAcountId)
            {
                case 1: { shareLink = new Access().GenShortUrl(1, shareLink); break; }  //www.shangjiudian.com
                case 3: { shareLink = new Access().GenShortUrl(1, shareLink); break; }  //www.shangjiudian.com
                case 4: { shareLink = new Access().GenShortUrl(1, shareLink); break; }  //www.zmjd123.com
                case 5: { shareLink = new Access().GenShortUrl(1, shareLink); break; }  //www.shangclub.com
                case 6: { shareLink = new Access().GenShortUrl(1, shareLink); break; }  //www.shangjiudian.com
            }

            ViewBag.ShareLink = shareLink;

            return View();
        }

        /// <summary>
        /// 【微信活动伙伴关注奖励页】
        /// </summary>
        /// <param name="partnerid"></param>
        /// <param name="activeid"></param>
        /// <param name="openid"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public ActionResult Weixin_LuckActive_PartnerLuck(int partnerid = 0, int activeid = 0, string openid = "", string code = "")
        {

            //return Redirect(string.Format("/Active/Weixin_LuckActive_End?activeid={0}&openid={1}", 0, 0));

            if (partnerid <= 0 || activeid <= 0)
            {
                return View();
            }

            //特殊处理
            if (activeid == 81) activeid = 84;  //第一期微信合作活动特殊处理，将“小豪测试”=>“母亲节礼盒”
            
            //获取当前活动的详细信息
            var weixinActiveEntity = GetWeixinActiveEntity(activeid);
            if (weixinActiveEntity == null && weixinActiveEntity.ID <= 0)
            {
                return View();
            }

            #region 如果没有openid，则通过静默授权获取

            if (string.IsNullOrEmpty(openid))
            {
                if (!string.IsNullOrEmpty(code))
                {
                    //通过code换取网页授权access_token
                    var accessToken = WeiXinHelper.GetWeixinAccessToken(code);
                    if (accessToken != null)
                    {
                        openid = accessToken.Openid;
                    }

                    //当前活动已经下线的时候
                    if (weixinActiveEntity != null && weixinActiveEntity.ActiveEndTime < DateTime.Now)
                    {
                        //跳至活动结束页面
                        return Redirect(string.Format("/Active/Weixin_LuckActive_End?activeid={0}&openid={1}", activeid, accessToken.Openid));
                    }

                }
                else
                {
                    //授权页面
                    var weixinGoUrl = WeiXinHelper.GenSilenceAuthorUrl(HttpUtility.UrlEncode(string.Format("http://www.shang-ke.cn/wx/active/partner/{0}/{1}", partnerid, activeid)));
                    //switch (weixinActiveEntity.WeixinAcountId)
                    //{
                    //    case 1: { weixinGoUrl = new Access().GenShortUrl(1, weixinGoUrl); break; }  //www.shang-ke.cn
                    //    case 3: { weixinGoUrl = new Access().GenShortUrl(1, weixinGoUrl); break; }  //www.shang-ke.cn
                    //    case 4: { weixinGoUrl = new Access().GenShortUrl(4, weixinGoUrl); break; }  //www.shang-ke.cn
                    //    case 5: { weixinGoUrl = new Access().GenShortUrl(5, weixinGoUrl); break; }  //www.shang-ke.cn
                    //}
                    return Redirect(weixinGoUrl);
                }
            }

            #endregion

            //获取所有的微信合作伙伴信息
            var allPartnerList = new Weixin().GetAllWeixinPartners();

            //筛选出当前活动关联的微信合作伙伴
            var relPartnerList = new List<ActiveWeixinPartner>();
            var relPartnerIds = weixinActiveEntity.RelPartnerIds;
            if (!string.IsNullOrEmpty(relPartnerIds))
            {
                var relPidsArray = relPartnerIds.Split(',').ToList();
                relPartnerList = allPartnerList.Where(p => relPidsArray.Contains(p.Id.ToString())).ToList();
            }
            if (partnerid != 49 && partnerid != 48 && partnerid != 47 && partnerid != 45 && partnerid != 18 && partnerid != 17 && !relPartnerList.Exists(p => p.Id == partnerid))
            {
                return View();
            }

            //检查报名记录
            var weixinDraw = new Weixin().GetActiveWeixinDraw(activeid, openid);
            if (weixinDraw == null || weixinDraw.ID <= 0)
            {
                //没有报名跳转至报名页
                var weixinGoUrl = WeiXinHelper.GenConfrimAuthorUrl(HttpUtility.UrlEncode(string.Format("http://www.shang-ke.cn/wx/active/reg/{0}/{1}", partnerid, activeid)));
                //switch (weixinActiveEntity.WeixinAcountId)
                //{
                //    case 1: { weixinGoUrl = new Access().GenShortUrl(1, weixinGoUrl); break; }  //www.shang-ke.cn
                //    case 3: { weixinGoUrl = new Access().GenShortUrl(1, weixinGoUrl); break; }  //www.shang-ke.cn
                //    case 4: { weixinGoUrl = new Access().GenShortUrl(4, weixinGoUrl); break; }  //www.shang-ke.cn
                //    case 5: { weixinGoUrl = new Access().GenShortUrl(5, weixinGoUrl); break; }  //www.shang-ke.cn
                //}
                return Redirect(weixinGoUrl);

                ////没有报名直接跳到活动页
                //return Redirect(weixinActiveEntity.WeixinSignUpShareLink);
            }

            //获取当前用户openid的所有获奖数据（用于筛选出哪些合作号已经领取）
            var luckCodeList = new Weixin().GetActiveWeixinLuckCodeInfo(activeid, openid) ?? new List<ActiveWeixinLuckCode>();

            //如果当前用户还没有领取过当前合作伙伴的奖励，则执行奖励
            if (!luckCodeList.Exists(lc => lc.PartnerId == partnerid))
            {
                //当前合作伙伴信息
                var partnerObj = allPartnerList.Find(p => p.Id == partnerid);

                //给当前用户openid生成该合作伙伴指定奖励数目的抽奖码
                for (int i = 0; i < partnerObj.LuckCodeCount; i++)
                {
                    var aLuck = new ActiveWeixinLuckCode();
                    aLuck.ActiveId = activeid;
                    aLuck.Openid = openid;
                    aLuck.PartnerId = partnerObj.Id;
                    aLuck.TagName = partnerObj.Name;
                    aLuck.CreateTime = DateTime.Now;
                    var genluckcode = new Weixin().PublishWeixinLuckCodeTask(aLuck);
                }   
            }

            //重定向个人抽奖页面
            return Redirect(string.Format("/Active/Weixin_LuckActive_ShareDone/{0}?openid={1}", activeid, openid));
            //return View();
        }

        //【活动结束页】
        public ActionResult Weixin_LuckActive_End(int activeid = 0, string openid = "")
        {
            ViewBag.IsApp = IsApp();
            ViewBag.ActiveId = activeid;

            return View();
        }

        #endregion

        #region 微信自定义活动

        /// <summary>
        /// 活动报名页面
        /// </summary>
        /// <param name="aid">活动ID</param>
        /// <param name="phone">参与者手机号</param>
        /// <param name="showtype">0报名 1查看报名信息</param>
        /// <returns></returns>
        public ActionResult CustomActive_Reg(int aid = 0, string phone = "", int showtype = 0)
        {
            ViewBag.ActiveId = aid;
            ViewBag.Phone = phone;
            ViewBag.ShowType = showtype;

            //查询当前活动当前手机的用户信息
            if (aid > 0 && !string.IsNullOrEmpty(phone) && phone.Length == 11)
            {
                var customUser = new Weixin().GetCustomActiveUser(aid, phone);
                ViewBag.CustomUser = customUser;
            }
            else
            {
                ViewBag.CustomUser = null;
            }

            return View();
        }

        /// <summary>
        /// 加入方法
        /// </summary>
        /// <param name="aid"></param>
        /// <param name="userName"></param>
        /// <param name="userPhone"></param>
        /// <returns></returns>
        public ActionResult RegCustomActiveUser(int aid = 0, string userName = "", string userPhone = "")
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();

            var customUser = new CustomActiveUser();
            customUser.OpenId = "";
            customUser.Unionid = "";
            customUser.UserName = userName;
            customUser.Phone = userPhone;
            customUser.Point = 2000;
            customUser.ActiveId = aid;
            customUser.State = 1;
            customUser.CreateTime = DateTime.Now;

            try
            {
                var add = new Weixin().AddCustomActiveUser(customUser);
                dict["Message"] = "恭喜您，提交成功";
                dict["Success"] = "1";
                dict["Url"] = "";
                return Json(dict, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                dict["Message"] = "提交失败，请重试";
                dict["Success"] = "0";
                dict["Url"] = "";
                return Json(dict, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region 微信支付 Demo

     

        /// <summary>
        /// 去微信支付
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="tit"></param>
        /// <param name="price"></param>
        /// <returns></returns>
        public ActionResult GoWeixinPay(int userid = 0, string ono = "", string tit = "", string price = "1", string openid = "")
        {
            var result = new WeixinPayReturnParam();
            try
            {
                //
                var clientIP = Request.UserHostAddress;

                string pre_pay_url = string.Format("{3}?orderid={0}&clientIP={1}&openid={2}", ono, clientIP, openid, WHotelSite.Common.Config.TenPay_pre_pay_url);

                LogHelper.WriteLog("pre_pay_url:" + pre_pay_url);

                string xml = HttpHelper.Get(pre_pay_url, "utf-8"); //<WeixinPayReturnParam_ForService xmlns:i="http://www.w3.org/2001/XMLSchema-instance" xmlns="http://schemas.datacontract.org/2004/07/Tenpay.Models"><err_code i:nil="true" /><err_code_des i:nil="true" /><prepay_id i:nil="true" /><result_code i:nil="true" /><return_code i:nil="true" /><return_msg i:nil="true" /><nonceStr>eBftKN0EdNnd6nuS</nonceStr><package>prepay_id=wx201605241917307bea7f635e0023591484</package><paySign>DA0947B61BC3F06AF99A79C547266D3C</paySign><signType>MD5</signType><timeStamp>1464117451</timeStamp></WeixinPayReturnParam_ForService>
                string ns = @"xmlns:i=""http://www.w3.org/2001/XMLSchema-instance"" xmlns=""http://schemas.datacontract.org/2004/07/Tenpay.Models""";
                string nil = @"i:nil=""true""";

                xml = xml.Replace(ns, "").Replace(nil, "");
                LogHelper.WriteLog("pre_pay_url Resykt:" + xml);



                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(xml);
                XmlElement rootElement = xmlDoc.DocumentElement;


                result = new WeixinPayReturnParam
                {
                    err_code = WeiXinHelper.GetXMLNodeValue(rootElement, "err_code"),
                    err_code_des = WeiXinHelper.GetXMLNodeValue(rootElement, "err_code_des"),
                    nonceStr = WeiXinHelper.GetXMLNodeValue(rootElement, "nonceStr"),
                    package = WeiXinHelper.GetXMLNodeValue(rootElement, "package"),
                    paySign = WeiXinHelper.GetXMLNodeValue(rootElement, "paySign"),
                    prepay_id = WeiXinHelper.GetXMLNodeValue(rootElement, "prepay_id"),
                    result_code = WeiXinHelper.GetXMLNodeValue(rootElement, "result_code"),
                    return_code = WeiXinHelper.GetXMLNodeValue(rootElement, "return_code"),
                    return_msg = WeiXinHelper.GetXMLNodeValue(rootElement, "return_msg"),
                    signType = WeiXinHelper.GetXMLNodeValue(rootElement, "signType"),
                    timeStamp = int.Parse(WeiXinHelper.GetXMLNodeValue(rootElement, "timeStamp"))
                };

                LogHelper.WriteLog("WeixinPayReturnParam:" + result.package);



               
            }
            catch(Exception err)
            {
                LogHelper.WriteLog("WeixinPayReturnParam err:" + err.Message + err.StackTrace);
               
            }

             return Json(result, JsonRequestBehavior.AllowGet);
            //var notify_url = "http://www.zmjiudian.com";
            //var order_result = WeiXinHelper.GetWeixinUnifiedOrder(tit, "", ono, Convert.ToInt32(price), clientIP, notify_url, "JSAPI", openid);
            //if (string.IsNullOrEmpty(order_result.prepay_id))
            //{
            //    result = new WeixinPayReturnParam() { return_code = order_result.return_code, return_msg = order_result.return_msg, result_code = order_result.result_code, err_code = order_result.err_code, err_code_des = order_result.err_code_des };
            //}
            //else
            //{
            //    //完成接口的签名及返回签名参数
            //    string non_str = WeiXinHelper.GenerateRandomStr();
            //    int timeStamp = WeiXinHelper.getSecondCountSince19700101();
            //    string package = "prepay_id=" + order_result.prepay_id;
            //    string signType = "MD5";
            //    string paySign = Signature.WeixinPaySignSignature(WeiXinHelper.WeixinServiceAppId, WeiXinHelper.WeixinServiceApiSecret, non_str, package, signType, timeStamp);

            //    result = new WeixinPayReturnParam() {  timeStamp = timeStamp, nonceStr = non_str, package = package, signType = signType, paySign = paySign };
            //}

            //LogHelper.WriteLog("GoWeixinPay:" + JsonConvert.SerializeObject(result));

            //return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetWeixinServiceConfigInfo()
        {
            //ToDo 传调用js-sdk页面url参数进来 得到使用注册信息
            string url = this.Request.Form["url"];
            if (!string.IsNullOrEmpty(url))
            {
                WeixinConfig config = new Weixin().GetWeixinConfig(url);
                config.appId = WeiXinHelper.WeixinServiceAppId;
                return Json(config);
            }
            else
            {
                return Json(new { Message = "缺少页面url", Success = 1 });
            }
        }

        public ActionResult GetHaoYiWeixinServiceConfigInfo()
        {
            //ToDo 传调用js-sdk页面url参数进来 得到使用注册信息
            string url = this.Request.Form["url"];
            if (!string.IsNullOrEmpty(url))
            {
                WeixinConfig config = new Weixin().GetWeixinConfig(url);
                config.appId = WeiXinHelper.HaoYiWeixinServiceAppId;
                return Json(config);
            }
            else
            {
                return Json(new { Message = "缺少页面url", Success = 1 });
            }
        }

        #endregion

        #region Demo

        public ActionResult ScopeDemo1(string code = "", string state = "")
        {

            ViewBag.Code = code;

            return View();
        }

        #endregion

        #region Coupon

        /// <summary>
        /// 加入ZMJD，立获现金券
        /// </summary>
        /// <returns></returns>
        public ActionResult GenCashCoupon(long sourceId = 0, string sid = "")
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

            //解析跟踪参数
            if (!string.IsNullOrEmpty(sid))
            {
                try
                {
                    string originStr = HJDAPI.Common.Security.DES.Decrypt(sid.Replace(" ", "+"));
                    var originObj = JsonConvert.DeserializeObject<GenTrackCodeParam>(originStr);
                    if (sourceId <= 0 && originObj.UserID > 0)
                    {
                        sourceId = originObj.UserID;
                    }
                }
                catch (Exception ex)
                {
                    
                }
            }
            ViewBag.SourceId = sourceId;

            //解析跟踪分享者信息
            HJD.AccountServices.Entity.MemberProfileInfo shareUserInfo = HJDAPI.APIProxy.account.GetCurrentUserInfo(sourceId) ?? new HJD.AccountServices.Entity.MemberProfileInfo();
            ViewBag.ShareUserInfo = shareUserInfo;

            return View();
        }

        /// <summary>
        /// 邀请好友注册页面
        /// </summary>
        /// <param name="sourceId"></param>
        /// <param name="sid"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public ActionResult GenCashCouponForInvitation(long sourceId = 0, string sid = "", string code = "")
        {
            if (HttpContext.Request.Url != null && !string.IsNullOrEmpty(HttpContext.Request.Url.AbsoluteUri))
            {
                var _rUrl = "";
                var _absUri = HttpContext.Request.Url.AbsoluteUri;
                if (_absUri.Contains("www.shangjiudian.com"))
                {
                    _rUrl = _absUri.Replace("www.shangjiudian.com", "www.zmjiudian.com").Replace("https://", "http://");
                    //return Redirect(_rUrl);
                }
                else if (_absUri.Contains("https://"))
                {
                    _rUrl = _absUri.Replace("https://", "http://");
                    //return Redirect(_rUrl);
                }

                if (!string.IsNullOrEmpty(_rUrl))
                {
                    //临时处理 2017.03.14 haoy 不需要了
                    //_rUrl = _rUrl.Replace("GenCashCouponForInvitation", "GenCashCoupon");

                    return Redirect(_rUrl);
                }
            }

            //Weixin
            bool isInWeixin = Request.UserAgent.IndexOf("MicroMessenger") > 0 ? true : false;
            ViewBag.isInWeixin = isInWeixin;

            #region 微信下做微信授权

            //微信环境下，做微信静默授权
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
                        }
                    }
                    else
                    {
                        //如果code有值，但不能获取，一般说明code过期了，那么重新获取
                        var weixinGoUrl = WeiXinHelper.GenSilenceAuthorUrlForHaoYi(HttpUtility.UrlEncode(string.Format("http://www.zmjiudian.com/Active/GenCashCouponForInvitation?sourceId={0}&sid={1}", sourceId, sid)));
                        return Redirect(weixinGoUrl);
                    }
                }
                else
                {
                    //授权页面
                    var weixinGoUrl = WeiXinHelper.GenSilenceAuthorUrlForHaoYi(HttpUtility.UrlEncode(string.Format("http://www.zmjiudian.com/Active/GenCashCouponForInvitation?sourceId={0}&sid={1}", sourceId, sid)));
                    return Redirect(weixinGoUrl);
                }
            }
            ViewBag.Openid = openid;
            ViewBag.Unionid = unionid;
            ViewBag.WeixinUid = wxuid;

            #endregion

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

            //解析跟踪参数
            if (!string.IsNullOrEmpty(sid))
            {
                try
                {
                    string originStr = HJDAPI.Common.Security.DES.Decrypt(sid.Replace(" ", "+"));
                    var originObj = JsonConvert.DeserializeObject<GenTrackCodeParam>(originStr);
                    if (sourceId <= 0 && originObj.UserID > 0)
                    {
                        sourceId = originObj.UserID;
                    }
                }
                catch (Exception ex)
                {

                }
            }
            ViewBag.SourceId = sourceId;

            //解析跟踪分享者信息
            HJD.AccountServices.Entity.MemberProfileInfo shareUserInfo = HJDAPI.APIProxy.account.GetCurrentUserInfo(sourceId) ?? new HJD.AccountServices.Entity.MemberProfileInfo();
            ViewBag.ShareUserInfo = shareUserInfo;

            //获取分享者的更多用户信息
            var shareUserInfoEx = account.GetUserInfoByUserID(sourceId);
            ViewBag.ShareUserInfoEx = shareUserInfoEx ?? new UserInfoResult();

            return View();
        }


        public ActionResult JoinZmjdGetCoupon(string phone = "", long sourceId = 0)
        {
            Dictionary<string, string> re = new Dictionary<string, string>();

            //首先验证当前手机号码是否已经注册
            try
            {
                var uinfo = account.GetUserInfoByMobile(phone);
                if (uinfo != null && uinfo.MobileState == 2)
                {
                    re["Message"] = "抱歉，您已经是周末酒店会员了";
                    re["Success"] = "4";
                    return Json(re, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                
            }

            //0 成功 1 失败 2 手机号已存在 3 手机号错误
            var result = Activity.JoinZmjdGetCoupon(phone, sourceId);
            switch (result)
            {
                case 0:
                    {
                        re["Message"] = "您已成功领取现金券";
                        re["Success"] = "0";
                        break;
                    }
                case 1:
                    {
                        re["Message"] = "领取失败，请重试";
                        re["Success"] = "1";
                        break;
                    }
                case 2:
                    {
                        re["Message"] = "该手机号已注册";
                        re["Success"] = "2";
                        break;
                    }
                case 3:
                    {
                        re["Message"] = "手机号输入有误";
                        re["Success"] = "3";
                        break;
                    }
            }

            return Json(re, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 抢红包
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public ActionResult GetRedPackets(int tag = 0)
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

            return View();
        }

        #endregion

        #region 短信页面的活动页面

        public ActionResult SmsForWeixinUser()
        {
            ViewBag.IsApp = IsApp();

            return View();
        }

        #endregion

        #region VIP相关活动

        /// <summary>
        /// vip活动之赠送礼品
        /// </summary>
        /// <returns></returns>
        public ActionResult VipActiveForGift(int userid = 0)
        {
            var isApp = IsApp();
            ViewBag.IsApp = isApp;

            //isMobile
            var isMobile = Utils.IsMobile();
            ViewBag.IsMobile = isMobile;

            //Weixin
            bool isInWeixin = Request.UserAgent.IndexOf("MicroMessenger") > 0 ? true : false;
            ViewBag.isInWeixin = isInWeixin;

            var curUserID = UserState.UserID > 0 ? UserState.UserID : userid;
            ViewBag.UserId = curUserID;

            //获取分享跟踪加密参数
            var scp = new GenTrackCodeParam();
            scp.UserID = curUserID;
            scp.BizType = ZMJDShareTrackBizType.recommendfriend;
            var shareCode = Comment.GenTrackCodeResult4Share(scp);
            ViewBag.SID = shareCode.EncodeStr;

            //获取当前用户信息
            HJD.AccountServices.Entity.MemberProfileInfo shareUserInfo = HJDAPI.APIProxy.account.GetCurrentUserInfo(curUserID) ?? new HJD.AccountServices.Entity.MemberProfileInfo();
            ViewBag.ShareUserInfo = shareUserInfo;

            return View();
        }

        /// <summary>
        /// 受邀成为VIP
        /// </summary>
        /// <param name="sourceId"></param>
        /// <param name="sid"></param>
        /// <returns></returns>
        public ActionResult GenVipForInvitation(long sourceId = 0, string sid = "")
        {
            var isApp = IsApp();
            ViewBag.IsApp = isApp;

            //isMobile
            var isMobile = Utils.IsMobile();
            ViewBag.IsMobile = isMobile;

            //Weixin
            bool isInWeixin = Request.UserAgent.IndexOf("MicroMessenger") > 0 ? true : false;
            ViewBag.isInWeixin = isInWeixin;

            //解析跟踪参数
            if (!string.IsNullOrEmpty(sid))
            {
                try
                {
                    string originStr = HJDAPI.Common.Security.DES.Decrypt(sid.Replace(" ", "+"));
                    var originObj = JsonConvert.DeserializeObject<GenTrackCodeParam>(originStr);
                    if (sourceId <= 0 && originObj.UserID > 0)
                    {
                        sourceId = originObj.UserID;
                    }
                }
                catch (Exception ex)
                {

                }
            }
            ViewBag.SourceId = sourceId;

            //解析跟踪分享者信息
            HJD.AccountServices.Entity.MemberProfileInfo shareUserInfo = HJDAPI.APIProxy.account.GetCurrentUserInfo(sourceId) ?? new HJD.AccountServices.Entity.MemberProfileInfo();
            ViewBag.ShareUserInfo = shareUserInfo;

            //获取分享者的更多用户信息
            var shareUserInfoEx = account.GetUserInfoByUserID(sourceId);
            ViewBag.ShareUserInfoEx = shareUserInfoEx ?? new UserInfoResult();

            if (sourceId > 0)
            {
                SetIsVIPInvatation(true);
            }

            return View();
        }

        #endregion

        #region 红包联合推广活动

        /// <summary>
        /// 领取落地页
        /// </summary>
        /// <param name="activeid"></param>
        /// <param name="partnerid">联合伙伴ID</param>
        /// <param name="goget">是否自动滚动到领取红包区域</param>
        /// <param name="code"></param>
        /// <returns></returns>
        public ActionResult RedpackUnionHome(int activeid = 0, int partnerid = 0, int goget = 0, string code = "")
        {
            var isApp = IsApp();
            ViewBag.IsApp = isApp;

            ViewBag.Goget = goget == 1;

            //Weixin
            bool isInWeixin = Request.UserAgent.IndexOf("MicroMessenger") > 0 ? true : false;
            ViewBag.isInWeixin = isInWeixin;

            #region 微信环境下，做微信授权

            var openid = "";
            if (isInWeixin)
            //var openid = "oHGzlw64Od16EpBke0PUojcPJEC0"; // "oHGzlw-sdix9G__-S4IzfTsYRqC8"; // "oHGzlw8ME0-5fwbBN-TJExJuM9qM";
            //if (isInWeixin && false)
            {
                if (!string.IsNullOrEmpty(code))
                {
                    //通过code换取网页授权access_token
                    var accessToken = WeiXinHelper.GetWeixinAccessToken(code);
                    if (accessToken != null && !string.IsNullOrEmpty(accessToken.Openid))
                    {
                        //通过access_token拉取用户信息(需scope为 snsapi_userinfo)（这是通过微信api获取的真实用户信息）
                        var weeixinUserInfo = WeiXinHelper.GetWeixinUserInfo(accessToken.AccessToken, accessToken.Openid);

                        openid = accessToken.Openid;

                        //存储微信用户
                        try
                        {
                            if (weeixinUserInfo != null)
                            {
                                #region insert 微信用户信息

                                var weixinNewUser = new ActiveWeixinUser();
                                weixinNewUser.Openid = accessToken.Openid;
                                weixinNewUser.Nickname = weeixinUserInfo.Nickname;
                                weixinNewUser.Sex = weeixinUserInfo.Sex.ToString();
                                weixinNewUser.Province = weeixinUserInfo.Province;
                                weixinNewUser.City = weeixinUserInfo.City;
                                weixinNewUser.Country = weeixinUserInfo.Country;
                                weixinNewUser.Headimgurl = weeixinUserInfo.Headimgurl;
                                weixinNewUser.Privilege = "";
                                weixinNewUser.Unionid = weeixinUserInfo.Unionid;
                                weixinNewUser.Phone = "";
                                weixinNewUser.ActiveID = activeid;
                                var add = new Weixin().AddActiveWeixinUser(weixinNewUser);

                                #endregion

                                #region insert 活动报名记录

                                //检查报名记录
                                var weixinDraw = new Weixin().GetActiveWeixinDraw(activeid, openid);

                                //未报名
                                if (weixinDraw == null)
                                {
                                    var activeDraw = new ActiveWeixinDraw();
                                    activeDraw.ActiveID = activeid;
                                    activeDraw.PartnerId = partnerid;
                                    activeDraw.Openid = openid;
                                    activeDraw.UserName = weeixinUserInfo.Nickname;
                                    activeDraw.Phone = "";
                                    activeDraw.IsShare = 0;
                                    activeDraw.ShareTime = DateTime.Now;
                                    activeDraw.SendFriendCount = 0;
                                    activeDraw.LastSendFriendTime = DateTime.Now;
                                    activeDraw.IsPay = 0;
                                    activeDraw.PayTime = DateTime.Now;
                                    activeDraw.CreateTime = DateTime.Now;
                                    var addDraw = new Weixin().AddActiveWeixinDraw(activeDraw);

                                    //现在报名成功后即默认奖励一个宝石
                                    var aFic = new ActiveWeixinFicMoney();
                                    aFic.ActiveId = activeid;
                                    aFic.Openid = openid;
                                    aFic.PartnerId = partnerid;
                                    aFic.Remark = "参与奖励";
                                    aFic.Value = 3;
                                    aFic.CreateTime = DateTime.Now;
                                    var genluckcode = new Weixin().AddActiveWeixinFicMoney(aFic);   //直接奖励，不是队列（队列难免存在延迟）
                                }

                                #endregion
                            }
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                    else
                    {
                        //如果code有值，但不能获取，一般说明code过期了，那么重新获取
                        //var weixinGoUrl = WeiXinHelper.GenConfrimAuthorUrlForHaoYi(HttpUtility.UrlEncode(string.Format("http://www.zmjiudian.com/wx/active/redpackunionhome/{0}/{1}?goget={2}", partnerid, activeid, goget)));
                        var weixinGoUrl = WeiXinHelper.GenConfrimAuthorUrl(HttpUtility.UrlEncode(string.Format("http://www.shang-ke.cn/wx/active/redpackunionhome/{0}/{1}?goget={2}", partnerid, activeid, goget)));
                        return Redirect(weixinGoUrl);
                    }
                }
                else
                {
                    //授权页面
                    //var weixinGoUrl = WeiXinHelper.GenConfrimAuthorUrlForHaoYi(HttpUtility.UrlEncode(string.Format("http://www.zmjiudian.com/wx/active/redpackunionhome/{0}/{1}?goget={2}", partnerid, activeid, goget)));
                    var weixinGoUrl = WeiXinHelper.GenConfrimAuthorUrl(HttpUtility.UrlEncode(string.Format("http://www.shang-ke.cn/wx/active/redpackunionhome/{0}/{1}?goget={2}", partnerid, activeid, goget)));
                    return Redirect(weixinGoUrl);
                }
            }
            else { 
            
                //非微信环境打开，无效
                //....
            }
            ViewBag.Openid = openid;
            ViewBag.Code = code;

            #endregion

            //活动基本信息
            var weixinActiveEntity = GetWeixinActiveEntity(activeid);
            ViewBag.WeixinActiveEntity = weixinActiveEntity;

            //活动关键字
            var showKeyword = weixinActiveEntity.ActivityKeyWord.Split('|')[0];
            ViewBag.ShowKeyword = showKeyword;

            //根据openid查询当前微信用户的报名信息
            var weixinDrawBak = new Weixin().GetActiveWeixinDraw(activeid, openid);
            ViewBag.WeixinDraw = weixinDrawBak;

            //当前微信用户信息
            var weixinUser = new Weixin().GetActiveWeixinUser(openid);
            ViewBag.WeixinUser = weixinUser;

            //获取该微信用户分享的活动页阅读记录
            var shareReadList = new Weixin().GetActiveWeixinShareReadList(activeid, openid) ?? new List<ActiveWeixinShareRead>();
            ViewBag.ShareReadList = shareReadList;

            //获取当前宝石获取记录
            var gemList = new Weixin().GetActiveWeixinFicMoneyInfo(activeid, openid) ?? new List<ActiveWeixinFicMoney>();
            ViewBag.GemList = gemList;

            //宝石扣除记录
            var deGemList = gemList.Where(_ => _.Value < 0).ToList();
            ViewBag.DeGemList = deGemList;

            //当前宝石总值
            var gemCount = gemList.Sum(_ => _.Value);
            ViewBag.GemCount = gemCount;

            //合作推广伙伴
            //获取所有的微信合作伙伴信息
            var allPartnerList = new Weixin().GetAllWeixinPartners();
            ViewBag.AllPartnerList = allPartnerList;

            //筛选出当前活动关联的微信合作伙伴
            var relPartnerList = new List<ActiveWeixinPartner>();
            var relPartnerIds = weixinActiveEntity.RelPartnerIds;
            if (!string.IsNullOrEmpty(relPartnerIds))
            {
                var relPidsArray = relPartnerIds.Split(',').ToList();
                relPartnerList = allPartnerList.Where(p => relPidsArray.Contains(p.Id.ToString())).ToList();
            }

            switch (weixinActiveEntity.WeixinAcountId)
            {
                case 1:
                case 2:
                    {
                        //将订阅号的顺序随机显示 (过滤掉zmjiudian，因为zmjiudian是报名强制关注的 2016-05-13 haoy)
                        relPartnerList = relPartnerList.Where(rp => rp.PartnerCode.ToLower().Trim() != "zmjiudian").ToList();
                        break;
                    }
            }

            //随机排序
            relPartnerList = relPartnerList.OrderBy(x => Guid.NewGuid()).ToList();

            ViewBag.RelPartnerList = relPartnerList;

            #region 合作伙伴页面发红包相关处理

            var activeRedpackActiveName = "红包大联欢";
            var activeRedpackWishing = "红包大联欢";
            var activeRedpackRemark = "红包大联欢";

            switch (activeid)
            {
                case 498: { break; }
                case 515: 
                    {
                        activeRedpackActiveName = "红包闹元宵";
                        activeRedpackWishing = "元宵节快乐";
                        activeRedpackRemark = "红包闹元宵";
                        break; 
                    }
                case 524:
                    {
                        activeRedpackActiveName = "女王驾到";
                        activeRedpackWishing = "女王节快乐";
                        activeRedpackRemark = "女王驾到";
                        break;
                    }
            }

            //是否领取过当前账户
            var isGetThisRedpack = false;

            var thisPartnerEntity = new ActiveWeixinPartner();
            if (partnerid > 0 && partnerid != 7)
            {
                //获取当前合作伙伴状态
                thisPartnerEntity = relPartnerList.Find(_ => _.Id == partnerid);
                if (thisPartnerEntity != null && thisPartnerEntity.Id > 0)
                {
                    //活动资金>0
                    if (thisPartnerEntity.ActiveFund > 0)
                    {
                        //验证当前用户是否领取过该合作伙伴的红包
                        if (!deGemList.Exists(_ => _.PartnerId == partnerid))
                        {
                            //验证当前用户是否有宝石 & 宝石数>=合作伙伴参与成本
                            if (gemCount > 0 && gemCount >= thisPartnerEntity.ActiveJoinCost)
                            {
                                ////扣宝石
                                //var deLuck = new ActiveWeixinLuckCode();
                                //deLuck.ActiveId = activeid;
                                //deLuck.Openid = openid;
                                //deLuck.PartnerId = partnerid;
                                //deLuck.TagName = "领红包扣除";
                                //deLuck.LuckCode = (0 - thisPartnerEntity.ActiveJoinCost);
                                //deLuck.CreateTime = DateTime.Now;
                                //var deluckcode = new Weixin().PublishWeixinGemTask(deLuck);

                                //扣宝石
                                var aFic = new ActiveWeixinFicMoney();
                                aFic.ActiveId = activeid;
                                aFic.Openid = openid;
                                aFic.PartnerId = partnerid;
                                aFic.Remark = "领红包扣除";
                                aFic.Value = (0 - thisPartnerEntity.ActiveJoinCost);
                                aFic.CreateTime = DateTime.Now;
                                var genluckcode = new Weixin().AddActiveWeixinFicMoney(aFic);   //直接奖励，不是队列（队列难免存在延迟）

                                //生成随机红包金额
                                var redpackAmount = 100;
                                if (thisPartnerEntity.ActiveMinReward > 0 && thisPartnerEntity.ActiveMaxReward > 0 && thisPartnerEntity.ActiveMinReward < thisPartnerEntity.ActiveMaxReward)
                                {
                                    var _partnerFund = thisPartnerEntity.ActiveFund * 100;
                                    int _min = Convert.ToInt32(thisPartnerEntity.ActiveMinReward * 100);
                                    int _max = Convert.ToInt32(thisPartnerEntity.ActiveMaxReward * 100);

                                    //如果当前合作伙伴剩余资金在区间内，那么直接发完
                                    if (_partnerFund >= _min && _partnerFund <= _max)
                                    {
                                        redpackAmount = (int)_partnerFund;
                                    }
                                    else
                                    {
                                        //取随机
                                        redpackAmount = new Random().Next(_min, _max);

                                        //循环检查
                                        var _ck = 0;
                                        while (redpackAmount > _partnerFund && _ck < 20)
                                        {
                                            redpackAmount = new Random().Next(_min, _max);
                                            _ck++;
                                        }

                                        //如果红包金额还是过大，则设为最小值发送
                                        if (redpackAmount > _partnerFund)
                                        {
                                            redpackAmount = _min;
                                        }
                                    }
                                }

                                var _sendName = thisPartnerEntity.ShortName;
                                if (string.IsNullOrEmpty(_sendName))
                                {
                                    _sendName = (thisPartnerEntity.Name.Length > 7 ? thisPartnerEntity.Name.Substring(0, 7) : thisPartnerEntity.Name) + "..";
                                }

                                //发红包
                                var addEntity = new WeixinRewardRecord();
                                addEntity.SourceId = partnerid;
                                addEntity.SourceType = 3;       //1酒店订单 2酒店订单 3活动合作伙伴ID 4微信活动用户报名记录ID
                                addEntity.ReOpenid = openid;
                                addEntity.ReWxUid = activeid;     //ReWxUid存储当前微信活动ID
                                addEntity.Wishing = activeRedpackWishing; //(thisPartnerEntity.Name.Length > 32 ? thisPartnerEntity.Name.Substring(0, 32) : thisPartnerEntity.Name);
                                addEntity.Amount = redpackAmount; //单位 分
                                addEntity.Number = 1;
                                addEntity.ActiveId = activeid;
                                addEntity.ActiveName = activeRedpackActiveName;
                                addEntity.Remark = activeRedpackRemark;
                                addEntity.SceneId = 3;            //1:商品促销 2:抽奖 3:虚拟物品兑奖 4:企业内部福利 5:渠道分润 6:保险回馈 7:彩票派奖 8:税务刮奖
                                addEntity.SendName = _sendName;   //"周末酒店";
                                addEntity.WillSendTime = DateTime.Now;
                                addEntity.RealSendTime = DateTime.Now;
                                addEntity.State = 0;
                                var sendRedpack = new Weixin().GenActiveRedPackRewardForUnion(addEntity);

                                ////红包发送成功后，真实扣除合作伙伴的活动资金（这里可能存在一些问题，当并发领取红包时）
                                decimal _updateAmount = Convert.ToDecimal(Convert.ToDecimal(addEntity.Amount) / Convert.ToDecimal(100));
                                var updatePartnerAmount = new Weixin().UpdateWxPartnerActiveFund(partnerid, _updateAmount);

                                isGetThisRedpack = true;
                            }
                        }
                        else
                        {
                            isGetThisRedpack = true;
                        }
                    }
                }
                else
                {
                    return Redirect(string.Format("/Active/Weixin_LuckActive_End?activeid={0}&openid={1}", 0, 0));   
                }
            }
            ViewBag.ThisPartnerEntity = thisPartnerEntity;
            ViewBag.ThisPartnerId = partnerid;
            ViewBag.IsGetThisRedpack = isGetThisRedpack;


            #endregion

            //获取当前用户已获得的红包 all
            var redpackGetedList = new Weixin().GetWeixinRewardRecordByWxActive(3, activeid, openid);
            ViewBag.RedpackGetedList = redpackGetedList;

            //成功以及待发以及发放失败的红包
            var redpackGetedList_1 = redpackGetedList.Where(_ => _.State == 0 || _.State == 1 || _.State == -1).ToList();
            ViewBag.RedpackGetedList_1 = redpackGetedList_1;

            //当前合作伙伴的红包
            var redpackGetedList_this = redpackGetedList_1.Where(_ => _.SourceId == partnerid).ToList();
            ViewBag.RedpackGetedList_this = redpackGetedList_this;

            //分享页面和分享图片
            ViewBag.ShareTitle = weixinActiveEntity.WeixinSignUpShareTitle;
            ViewBag.ShareImgUrl = weixinActiveEntity.WeixinSignUpShareImgUrl;
            ViewBag.TopBannerUrl = weixinActiveEntity.WeixinSignUpTopBannerUrl;
            ViewBag.ActiveEndTime = weixinActiveEntity.ActiveEndTime;
            ViewBag.WeixinSignUpResultLink = weixinActiveEntity.WeixinSignUpResultLink ?? "";
            ViewBag.ActiveId = activeid;

            var readCount = shareReadList.Count;
            ViewBag.ReadCount = readCount;
            ViewBag.OpenId = openid;
            ViewBag.Uid = weixinUser.ID;

            ////获取该活动的统计数据
            //var statResult = new Weixin().GetWeixinAtvStatResult(activeid);
            //ViewBag.StatResult = statResult;

            //分享链接（需生成短链接）
            //var shareLink = string.Format("http://www.zmjiudian.com/wx/active/redpackunionshare/{0}12345{1}", activeid, weixinUser.ID);
            var shareLink = string.Format("http://www.shang-ke.cn/wx/active/redpackunionshare/{0}12345{1}", activeid, weixinUser.ID);

            ////tip：目前活动需要服务号的功能，现在只有周末酒店服务号支持，所以暂时统一使用 www.shang-ke.cn，www.shangjiudian.com被屏蔽分享了...域名 haoy 2018-01-16
            //shareLink = new Access().GenShortUrl(1, shareLink);
            //switch (weixinActiveEntity.WeixinAcountId)
            //{
            //    case 1: { shareLink = new Access().GenShortUrl(1, shareLink); break; }  //www.shang-ke.cn
            //    case 3: { shareLink = new Access().GenShortUrl(1, shareLink); break; }  //www.shang-ke.cn
            //    case 4: { shareLink = new Access().GenShortUrl(1, shareLink); break; }  //www.shang-ke.cn
            //    case 5: { shareLink = new Access().GenShortUrl(1, shareLink); break; }  //www.shang-ke.cn
            //    case 6: { shareLink = new Access().GenShortUrl(1, shareLink); break; }  //www.shang-ke.cn
            //    case 11: { shareLink = new Access().GenShortUrl(1, shareLink); break; }  //www.shang-ke.cn
            //}
            ViewBag.ShareLink = shareLink;

            return View();
        }

        /// <summary>
        /// 【分享页】
        /// </summary>
        /// <param name="aidsharer"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public ActionResult RedpackUnionShare(string aidsharer = "0123450", string code = "")
        {
            ViewBag.IsApp = IsApp();

            //Weixin
            bool isInWeixin = Request.UserAgent.IndexOf("MicroMessenger") > 0 ? true : false;
            ViewBag.isInWeixin = isInWeixin;

            var activeid = "0";
            var sharer = "0";
            if (!string.IsNullOrEmpty(aidsharer) && aidsharer.Contains("12345"))
            {
                activeid = Regex.Split(aidsharer, "12345")[0];
                sharer = Regex.Split(aidsharer, "12345")[1];
            }

            var weixinActiveEntity = GetWeixinActiveEntity(Convert.ToInt32(activeid));

            //分享页面和分享图片
            ViewBag.ShareTitle = weixinActiveEntity.WeixinSignUpShareTitle;
            ViewBag.ShareLink = weixinActiveEntity.WeixinSignUpShareLink;
            ViewBag.ShareImgUrl = weixinActiveEntity.WeixinSignUpShareImgUrl;
            ViewBag.TopBannerUrl = weixinActiveEntity.WeixinSignUpTopBannerUrl;
            ViewBag.ActiveId = activeid;

            if (!string.IsNullOrEmpty(code))
            {
                //首先拿到分享者的openid
                var shareWeixinUser = new Weixin().GetActiveWeixinUserById(Convert.ToInt32(sharer));
                if (shareWeixinUser != null && weixinActiveEntity != null && weixinActiveEntity.ActiveEndTime > DateTime.Now)
                {
                    //通过code换取网页授权access_token
                    var accessToken = WeiXinHelper.GetWeixinAccessToken(code);

                    if (shareWeixinUser.Openid.ToLower().Trim() != accessToken.Openid.ToLower().Trim())
                    {
                        //有了分享者openid，又拿到了当前阅读者的openid，则插入一条阅读记录
                        var activeRead = new ActiveWeixinShareRead();
                        activeRead.ActiveID = Convert.ToInt32(activeid);
                        activeRead.SharerOpenid = shareWeixinUser.Openid;
                        activeRead.Openid = accessToken.Openid;
                        activeRead.LastReadTime = DateTime.Now;
                        activeRead.ReadCount = 1;

                        //add
                        var add = new Weixin().AddActiveWeixinShareRead_Luck(activeRead);

                        //阅读记录完成，触发生成抽奖码/宝石队列任务
                        var aLuck = new ActiveWeixinLuckCode();
                        aLuck.ActiveId = activeRead.ActiveID;
                        aLuck.Openid = activeRead.SharerOpenid;
                        aLuck.PartnerId = 0;
                        aLuck.TagName = "分享奖励";
                        aLuck.LuckCode = 1;
                        aLuck.CreateTime = DateTime.Now;
                        var genluckcode = new Weixin().PublishWeixinGemTask(aLuck); 
                    }
                }
            }
            else
            {
                //授权页面
                //var weixinGoUrl = WeiXinHelper.GenSilenceAuthorUrlForHaoYi(HttpUtility.UrlEncode(string.Format("http://www.zmjiudian.com/wx/active/redpackunionshare/{0}12345{1}", activeid, sharer)));
                var weixinGoUrl = WeiXinHelper.GenSilenceAuthorUrl(HttpUtility.UrlEncode(string.Format("http://www.shang-ke.cn/wx/active/redpackunionshare/{0}12345{1}", activeid, sharer)));
                return Redirect(weixinGoUrl);
            }

            //走到这里，说明已经静默授权了，则直接跳到活动页
            if (!string.IsNullOrEmpty(weixinActiveEntity.WeixinSignUpShareLink))
            {
                return Redirect(weixinActiveEntity.WeixinSignUpShareLink);
            }
            return View();
        }

        #endregion

        #region 套餐专辑活动（如 航空里程赠送）

        /// <summary>
        /// 套餐专辑活动（如 航空里程赠送）
        /// </summary>
        /// <param name="activeId"></param>
        /// <param name="userId"></param>
        /// <param name="toget">是否直接领取券(主要为app的分享回调使用)</param>
        /// <returns></returns>
        public ActionResult ProductAlbumActive(int activeId = 0, long userId = 0, int toget = 0)
        {
            var isApp = IsApp();
            ViewBag.IsApp = isApp;

            //isMobile
            var isMobile = Utils.IsMobile();
            ViewBag.IsMobile = isMobile;

            //Weixin
            bool isInWeixin = Request.UserAgent.IndexOf("MicroMessenger") > 0 ? true : false;
            ViewBag.isInWeixin = isInWeixin;

            var curUserID = UserState.UserID > 0 ? UserState.UserID : userId;
            ViewBag.UserId = curUserID;

            ViewBag.ActiveId = activeId;

            ViewBag.ToGet = toget;

            return View();
        }

        #endregion

        #region 微信投票活动【公众号回复投票机制】

        /// <summary>
        /// 【投票活动结果页】
        /// </summary>
        /// <param name="activeId">当前主活动ID</param>
        /// <param name="voteId">当前被投票者的编号（目前存储的是HotelId字段）</param>
        /// <param name="code"></param>
        /// <returns></returns>
        public ActionResult VoteResult(int activeId = 0, int voteId = 0, string code = "")
        {
            //是否app环境
            ViewBag.IsApp = IsApp();

            //Weixin
            bool isInWeixin = Request.UserAgent.IndexOf("MicroMessenger") > 0 ? true : false;
            ViewBag.isInWeixin = isInWeixin;

            //当前活动ID
            ViewBag.ActiveId = activeId;

            //当前被投票者ID
            ViewBag.VoteId = voteId;

            //当前zmjd登录用户
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
            var openid = "";
            var unionid = "";
            var wxuid = 0;
            if (isInWeixin)
            {
                //周末酒店服务号_浩颐
                var weixinAcount = Convert.ToInt32(WeiXinChannelCode.周末酒店服务号_皓颐);

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
                        var weixinGoUrl = WeiXinHelper.GenConfrimAuthorUrlByWeixinAcount((WeiXinChannelCode)weixinAcount, HttpUtility.UrlEncode(string.Format("http://www.zmjiudian.com/wx/active/voteresult/{0}/{1}", activeId, voteId)));
                        return Redirect(weixinGoUrl);
                    }
                }
                else
                {
                    //授权页面
                    var weixinGoUrl = WeiXinHelper.GenConfrimAuthorUrlByWeixinAcount((WeiXinChannelCode)weixinAcount, HttpUtility.UrlEncode(string.Format("http://www.zmjiudian.com/wx/active/voteresult/{0}/{1}", activeId, voteId)));
                    return Redirect(weixinGoUrl);
                }
            }

            #endregion

            ViewBag.Openid = openid;
            ViewBag.Unionid = unionid;
            ViewBag.WeixinUid = wxuid;

            return View();
        }

        #endregion

        #region 微信大投票活动【大使代言投票机制】

        /// <summary>
        /// 【大投票活动结果页】
        /// </summary>
        /// <param name="activeId">当前主活动ID</param>
        /// <param name="urlfrommine"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public ActionResult SuperVote(int activeId = 0, int urlfrommine = 0, string code = "")
        {
            //是否app环境
            ViewBag.IsApp = IsApp();

            //Weixin
            bool isInWeixin = Request.UserAgent.IndexOf("MicroMessenger") > 0 ? true : false;
            ViewBag.isInWeixin = isInWeixin;

            //当前活动ID
            ViewBag.ActiveId = activeId;

            //当前zmjd登录用户
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
            var openid = "";
            var unionid = "";
            var wxuid = 0;
            if (isInWeixin)
            //if (isInWeixin && false)
            {
                //周末酒店服务号_浩颐
                var weixinAcount = Convert.ToInt32(WeiXinChannelCode.周末酒店服务号_皓颐);

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
                        }
                    }
                    else
                    {
                        //如果code有值，但不能获取，一般说明code过期了，那么重新获取
                        var weixinGoUrl = WeiXinHelper.GenConfrimAuthorUrlByWeixinAcount((WeiXinChannelCode)weixinAcount, HttpUtility.UrlEncode(string.Format("http://www.zmjiudian.com/wx/active/supervote/{0}/{1}", activeId, urlfrommine)));
                        return Redirect(weixinGoUrl);
                    }
                }
                else
                {
                    //授权页面
                    var weixinGoUrl = WeiXinHelper.GenConfrimAuthorUrlByWeixinAcount((WeiXinChannelCode)weixinAcount, HttpUtility.UrlEncode(string.Format("http://www.zmjiudian.com/wx/active/supervote/{0}/{1}", activeId, urlfrommine)));
                    return Redirect(weixinGoUrl);
                }
            }

            #endregion

            //test
            if (HttpContext.Request.Url != null && !string.IsNullOrEmpty(HttpContext.Request.Url.AbsoluteUri))
            {
                var _rUrl = "";
                var _absUri = HttpContext.Request.Url.AbsoluteUri;
                if (!_absUri.ToLower().Contains("www."))
                {
                    if (string.IsNullOrEmpty(openid))
                    {
                        openid = "oHGzlw-sdix9G__-S4IzfTsYRqC8";
                    }
                }
            }

            #region 查询当前用户报名信息

            var weixinDraw = new Weixin().GetActiveWeixinDraw(activeId, openid);
            if (weixinDraw == null || weixinDraw.ActiveID <= 0 )
            {
                //注册报名
                var activeDraw = new ActiveWeixinDraw();
                activeDraw.ActiveID = activeId;
                activeDraw.PartnerId = 0;
                activeDraw.Openid = openid;
                activeDraw.UserName = weixinUserInfo.Nickname;
                activeDraw.IsShare = 0;
                activeDraw.ShareTime = DateTime.Now;
                activeDraw.SendFriendCount = 0;
                activeDraw.LastSendFriendTime = DateTime.Now;
                activeDraw.IsPay = 0;
                activeDraw.PayTime = DateTime.Now;
                activeDraw.CreateTime = DateTime.Now;

                //默认报名没有手机号
                activeDraw.Phone = "";

                var _add = new Weixin().AddActiveWeixinDraw(activeDraw);

                //重新获取
                weixinDraw = new Weixin().GetActiveWeixinDraw(activeId, openid);
            }
            ViewBag.WeixinDraw = weixinDraw;

            #endregion

            ViewBag.WeixinUser = weixinUserInfo;
            ViewBag.Openid = openid;
            ViewBag.Unionid = unionid;
            ViewBag.WeixinUid = wxuid;

            //是否来自自己
            if (urlfrommine > 0)
            {
                urlfrommine = new Random().Next(1, 999);
            }
            ViewBag.Urlfrommine = urlfrommine;

            return View();
        }

        /// <summary>
        /// 【大投票活动的被投票者主页】
        /// </summary>
        /// <param name="activeId"></param>
        /// <param name="id"></param>
        /// <param name="sourceDrawid">来源报名用户ID</param>
        /// <param name="urlfrommine">跳转是否来自自己主页</param>
        /// <param name="code"></param>
        /// <returns></returns>
        public ActionResult SuperVoteItem(int activeId, int id, int sourceDrawid = 0, int urlfrommine = 0, string code = "")
        {
            //是否app环境
            ViewBag.IsApp = IsApp();

            //Weixin
            bool isInWeixin = Request.UserAgent.IndexOf("MicroMessenger") > 0 ? true : false;
            ViewBag.isInWeixin = isInWeixin;

            //当前活动ID
            ViewBag.ActiveId = activeId;

            //当前item ID
            ViewBag.Id = id;

            //当前zmjd登录用户
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
            var openid = "";
            var unionid = "";
            var wxuid = 0;
            if (isInWeixin)
            //if (isInWeixin && false)
            {
                //周末酒店服务号_浩颐
                var weixinAcount = Convert.ToInt32(WeiXinChannelCode.周末酒店服务号_皓颐);

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
                        }
                    }
                    else
                    {
                        //如果code有值，但不能获取，一般说明code过期了，那么重新获取
                        var weixinGoUrl = WeiXinHelper.GenConfrimAuthorUrlByWeixinAcount((WeiXinChannelCode)weixinAcount, HttpUtility.UrlEncode(string.Format("http://www.zmjiudian.com/wx/active/supervoteitem/{0}/{1}/{2}/{3}", activeId, id, sourceDrawid, urlfrommine)));
                        return Redirect(weixinGoUrl);
                    }
                }
                else
                {
                    //授权页面
                    var weixinGoUrl = WeiXinHelper.GenConfrimAuthorUrlByWeixinAcount((WeiXinChannelCode)weixinAcount, HttpUtility.UrlEncode(string.Format("http://www.zmjiudian.com/wx/active/supervoteitem/{0}/{1}/{2}/{3}", activeId, id, sourceDrawid, urlfrommine)));
                    return Redirect(weixinGoUrl);
                }
            }

            #endregion

            //test
            if (HttpContext.Request.Url != null && !string.IsNullOrEmpty(HttpContext.Request.Url.AbsoluteUri))
            {
                var _rUrl = "";
                var _absUri = HttpContext.Request.Url.AbsoluteUri;
                if (!_absUri.ToLower().Contains("www."))
                {
                    if (string.IsNullOrEmpty(openid))
                    {
                        openid = "oHGzlw-sdix9G__-S4IzfTsYRqC8";
                    }
                }
            }

            #region 查询当前用户报名信息

            var weixinDraw = new Weixin().GetActiveWeixinDraw(activeId, openid);
            if (weixinDraw == null || weixinDraw.ActiveID <= 0)
            {
                //注册报名
                var activeDraw = new ActiveWeixinDraw();
                activeDraw.ActiveID = activeId;
                activeDraw.PartnerId = 0;
                activeDraw.Openid = openid;
                activeDraw.UserName = weixinUserInfo.Nickname;
                activeDraw.IsShare = 0;
                activeDraw.ShareTime = DateTime.Now;
                activeDraw.SendFriendCount = 0;
                activeDraw.LastSendFriendTime = DateTime.Now;
                activeDraw.IsPay = 0;
                activeDraw.PayTime = DateTime.Now;
                activeDraw.CreateTime = DateTime.Now;

                //默认报名没有手机号
                activeDraw.Phone = "";

                var _add = new Weixin().AddActiveWeixinDraw(activeDraw);

                //重新获取
                weixinDraw = new Weixin().GetActiveWeixinDraw(activeId, openid);
            }
            ViewBag.WeixinDraw = weixinDraw;

            //如果是自己识别自己海报，则不算数
            if (weixinDraw != null && weixinDraw.ID == sourceDrawid)
            {
                sourceDrawid = 0;
            }

            #endregion

            ViewBag.WeixinUser = weixinUserInfo;
            ViewBag.Openid = openid;
            ViewBag.Unionid = unionid;
            ViewBag.WeixinUid = wxuid;
            ViewBag.SourceDrawId = sourceDrawid;
            ViewBag.PosterSloganPic = GetVotePosterSloganPic();

            //是否来自自己
            if (urlfrommine > 0 )
            {
                urlfrommine = new Random().Next(1, 999);
            }
            ViewBag.Urlfrommine = urlfrommine;

            return View();
        }

        /// <summary>
        /// 【大投票活动的大使用户主页】
        /// </summary>
        /// <param name="activeId">活动Id</param>
        /// <param name="code"></param>
        /// <param name="ruleExId"></param>
        /// <returns></returns>
        public ActionResult SuperVoteUser(int activeId = 0, int ruleExId = 0, string code = "")
        {
            //是否app环境
            ViewBag.IsApp = IsApp();

            //Weixin
            bool isInWeixin = Request.UserAgent.IndexOf("MicroMessenger") > 0 ? true : false;
            ViewBag.isInWeixin = isInWeixin;

            //当前活动ID
            ViewBag.ActiveId = activeId;

            //当前zmjd登录用户
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
            var openid = "";
            var unionid = "";
            var wxuid = 0;
            if (isInWeixin)
            //if (isInWeixin && false)
            {
                //周末酒店服务号_浩颐
                var weixinAcount = Convert.ToInt32(WeiXinChannelCode.周末酒店服务号_皓颐);

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
                        }
                    }
                    else
                    {
                        //如果code有值，但不能获取，一般说明code过期了，那么重新获取
                        var weixinGoUrl = WeiXinHelper.GenConfrimAuthorUrlByWeixinAcount((WeiXinChannelCode)weixinAcount, HttpUtility.UrlEncode(string.Format("http://www.zmjiudian.com/wx/active/supervoteuser/{0}/{1}", activeId, ruleExId)));
                        return Redirect(weixinGoUrl);
                    }
                }
                else
                {
                    //授权页面
                    var weixinGoUrl = WeiXinHelper.GenConfrimAuthorUrlByWeixinAcount((WeiXinChannelCode)weixinAcount, HttpUtility.UrlEncode(string.Format("http://www.zmjiudian.com/wx/active/supervoteuser/{0}/{1}", activeId, ruleExId)));
                    return Redirect(weixinGoUrl);
                }
            }

            #endregion

            //test
            if (HttpContext.Request.Url != null && !string.IsNullOrEmpty(HttpContext.Request.Url.AbsoluteUri))
            {
                var _rUrl = "";
                var _absUri = HttpContext.Request.Url.AbsoluteUri;
                if (!_absUri.ToLower().Contains("www."))
                {
                    if (string.IsNullOrEmpty(openid))
                    {
                        openid = "oHGzlw0o44j7HJUn5h2nNC7UdkHo";
                    }
                }
            }

            #region 查询当前用户报名信息

            var weixinDraw = new Weixin().GetActiveWeixinDraw(activeId, openid);
            if (weixinDraw == null || weixinDraw.ID <= 0 || string.IsNullOrEmpty(weixinDraw.Phone))
            {
                return Redirect("http://www.shang-ke.cn/active/supervoteend?err=1");
            }
            ViewBag.WeixinDraw = weixinDraw;

            #endregion

            ViewBag.WeixinUser = weixinUserInfo;
            ViewBag.Openid = openid;
            ViewBag.Unionid = unionid;
            ViewBag.WeixinUid = wxuid;
            ViewBag.RuleExId = ruleExId;
            ViewBag.PosterSloganPic = GetVotePosterSloganPic();

            return View();
        }

        /// <summary>
        /// 【大投票活动的大使用户注册页面】
        /// </summary>
        /// <param name="activeId">活动Id</param>
        /// <param name="exid">ActiveRuleEx ID</param>
        /// <param name="urlfrommine"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public ActionResult SuperVoteUserReg(int activeId = 0, int exid = 0, int urlfrommine = 0, string code = "")
        {
            //是否app环境
            ViewBag.IsApp = IsApp();

            //Weixin
            bool isInWeixin = Request.UserAgent.IndexOf("MicroMessenger") > 0 ? true : false;
            ViewBag.isInWeixin = isInWeixin;

            //当前活动ID
            ViewBag.ActiveId = activeId;

            //当前zmjd登录用户
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
            var openid = "";
            var unionid = "";
            var wxuid = 0;
            if (isInWeixin)
            //if (isInWeixin && false)
            {
                //周末酒店服务号_浩颐
                var weixinAcount = Convert.ToInt32(WeiXinChannelCode.周末酒店服务号_皓颐);

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
                        }
                    }
                    else
                    {
                        //如果code有值，但不能获取，一般说明code过期了，那么重新获取
                        var weixinGoUrl = WeiXinHelper.GenConfrimAuthorUrlByWeixinAcount((WeiXinChannelCode)weixinAcount, HttpUtility.UrlEncode(string.Format("http://www.zmjiudian.com/wx/active/supervoteuserreg/{0}/{1}/{2}", activeId, exid, urlfrommine)));
                        return Redirect(weixinGoUrl);
                    }
                }
                else
                {
                    //授权页面
                    var weixinGoUrl = WeiXinHelper.GenConfrimAuthorUrlByWeixinAcount((WeiXinChannelCode)weixinAcount, HttpUtility.UrlEncode(string.Format("http://www.zmjiudian.com/wx/active/supervoteuserreg/{0}/{1}/{2}", activeId, exid, urlfrommine)));
                    return Redirect(weixinGoUrl);
                }
            }

            #endregion

            //test
            if (HttpContext.Request.Url != null && !string.IsNullOrEmpty(HttpContext.Request.Url.AbsoluteUri))
            {
                var _rUrl = "";
                var _absUri = HttpContext.Request.Url.AbsoluteUri;
                if (!_absUri.ToLower().Contains("www."))
                {
                    if (string.IsNullOrEmpty(openid))
                    {
                        openid = "oHGzlw-sdix9G__-S4IzfTsYRqC8";
                    }
                }
            }

            #region 查询当前用户报名信息

            var weixinDraw = new Weixin().GetActiveWeixinDraw(activeId, openid);
            if (weixinDraw == null || weixinDraw.ID <= 0)
            {
                return Redirect("http://www.shang-ke.cn/active/supervoteend?err=1");
            }
            ViewBag.WeixinDraw = weixinDraw;

            //微信头像上传ID
            var headImgUploadId = 0;

            try
            {
                //上传当前用户头像并生成zmjd的图片地址，更新到报名记录里
                if (weixinDraw != null && string.IsNullOrEmpty(weixinDraw.HeadImgUrl) && weixinUserInfo != null && !string.IsNullOrEmpty(weixinUserInfo.Headimgurl))
                {
                    if (!string.IsNullOrEmpty(weixinUserInfo.Headimgurl))
                    {
                        //更新当前用户的报名头像地址
                        var _update = new Weixin().UpdateActiveWeixinDrawHeadImgUrl(activeId, openid, weixinUserInfo.Headimgurl);
                    }

                    ////用户原微信头像
                    //var _weixinUserHeadImg = weixinUserInfo.Headimgurl;

                    ////获取上传后的图片地址
                    ////weixinUserInfo.Headimgurl = "http://wx.qlogo.cn/mmopen/vi_32/DYAIOgq83erMQ8uEP01pb3zpae99PsIG2GQ8EiawxahZnFVsN0JaY4wibXDQKsRA1hpq4Ar9Hdckv6PD00VmEoiaw/0";
                    //var imgs = new List<string> { _weixinUserHeadImg };

                    ////获取上传图片的ID
                    //var _getids = new Weixin().PhotoUploadWithUrls(imgs.ToArray());
                    //if (_getids != null && _getids.Count > 0)
                    //{
                    //    headImgUploadId = _getids[0];
                    //}
                }
            }
            catch (Exception ex)
            {
                
            }

            #endregion

            ViewBag.HeadImgUploadId = headImgUploadId;
            ViewBag.WeixinUser = weixinUserInfo;
            ViewBag.Openid = openid;
            ViewBag.Unionid = unionid;
            ViewBag.WeixinUid = wxuid;
            ViewBag.RuleExId = exid;
            ViewBag.PosterSloganPic = GetVotePosterSloganPic();

            //是否来自自己
            if (urlfrommine > 0)
            {
                urlfrommine = new Random().Next(1, 999);
            }
            ViewBag.Urlfrommine = urlfrommine;

            return View();
        }

        /// <summary>
        /// 上传报名头像
        /// </summary>
        /// <param name="activeId"></param>
        /// <param name="openid"></param>
        /// <param name="imgId"></param>
        /// <returns></returns>
        public ActionResult UploadDrawHeadimg(int activeId, string openid, int imgId)
        {
            Dictionary<string, string> re = new Dictionary<string, string>();
            re["State"] = "0";
            re["Message"] = "头像上传失败";
            re["ImageUrl"] = "";

            //图片ID参数
            List<int> _imgIds = new List<int>();
            _imgIds.Add(imgId);

            //获取图片信息
            List<HJD.PhotoServices.Contracts.PHSPhotoInfoEntity> _getPhotos = new Weixin().GenPhotoPaths(_imgIds);
            if (_getPhotos != null && _getPhotos.Count > 0)
            {
                var _uploadImgUrl = _getPhotos[0].PhotoUrl[HJD.PhotoServices.Entity.PhotoSizeType.jupiter];

                //更新当前用户的报名头像地址
                var _update = new Weixin().UpdateActiveWeixinDrawHeadImgUrl(activeId, openid, _uploadImgUrl);

                re["State"] = "1";
                re["Message"] = "头像已上传";
                re["ImageUrl"] = _uploadImgUrl;
            }

            return Json(re, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 随机获取一个拉票海报slogan背景图
        /// </summary>
        /// <returns></returns>
        private string GetVotePosterSloganPic()
        {
            var _pic = "";

            var _list = new List<string>();
            _list.Add("http://whphoto.b0.upaiyun.com/118HQe60_jupiter");    //supervote-大家都来帮比蜜甜.png
            _list.Add("http://whphoto.b0.upaiyun.com/118HQe66_jupiter");    //supervote-长这么好看愿意pick我.png
            _list.Add("http://whphoto.b0.upaiyun.com/118HQe63_jupiter");    //supervote-你一票我一票轻松拿到.png
            _list.Add("http://whphoto.b0.upaiyun.com/118HQe61_jupiter");    //supervote-你不投我不投何时抱走.png
            _list.Add("http://whphoto.b0.upaiyun.com/118HQe64_jupiter");    //supervote-天若有情天亦老pick一下好不好.png
            _list.Add("http://whphoto.b0.upaiyun.com/118HQe65_jupiter");    //supervote-万水千山总是情行不行.png
            _list.Add("http://whphoto.b0.upaiyun.com/118HQe62_jupiter");    //supervote-你出力我出力大奖不是奇迹.png

            //随机一个
            _pic = _list.OrderBy(_ => Guid.NewGuid()).ToList().First();

            return _pic;
        }

        /// <summary>
        /// 【大投票活动的大使抽奖记录页面】
        /// </summary>
        /// <param name="activeId"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public ActionResult SuperVoteLuckRecord(int activeId = 0, string code = "")
        {
            //是否app环境
            ViewBag.IsApp = IsApp();

            //Weixin
            bool isInWeixin = Request.UserAgent.IndexOf("MicroMessenger") > 0 ? true : false;
            ViewBag.isInWeixin = isInWeixin;

            //当前活动ID
            ViewBag.ActiveId = activeId;

            //当前zmjd登录用户
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
            var openid = "";
            var unionid = "";
            var wxuid = 0;
            if (isInWeixin)
            //if (isInWeixin && false)
            {
                //周末酒店服务号_浩颐
                var weixinAcount = Convert.ToInt32(WeiXinChannelCode.周末酒店服务号_皓颐);

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
                        }
                    }
                    else
                    {
                        //如果code有值，但不能获取，一般说明code过期了，那么重新获取
                        var weixinGoUrl = WeiXinHelper.GenConfrimAuthorUrlByWeixinAcount((WeiXinChannelCode)weixinAcount, HttpUtility.UrlEncode(string.Format("http://www.zmjiudian.com/wx/active/SuperVoteLuckRecord/{0}", activeId)));
                        return Redirect(weixinGoUrl);
                    }
                }
                else
                {
                    //授权页面
                    var weixinGoUrl = WeiXinHelper.GenConfrimAuthorUrlByWeixinAcount((WeiXinChannelCode)weixinAcount, HttpUtility.UrlEncode(string.Format("http://www.zmjiudian.com/wx/active/SuperVoteLuckRecord/{0}", activeId)));
                    return Redirect(weixinGoUrl);
                }
            }

            #endregion

            //test
            if (HttpContext.Request.Url != null && !string.IsNullOrEmpty(HttpContext.Request.Url.AbsoluteUri))
            {
                var _rUrl = "";
                var _absUri = HttpContext.Request.Url.AbsoluteUri;
                if (!_absUri.ToLower().Contains("www."))
                {
                    if (string.IsNullOrEmpty(openid))
                    {
                        openid = "oHGzlw-sdix9G__-S4IzfTsYRqC8";
                    }
                }
            }

            #region 查询当前用户报名信息

            var weixinDraw = new Weixin().GetActiveWeixinDraw(activeId, openid);
            if (weixinDraw == null || weixinDraw.ID <= 0 || string.IsNullOrEmpty(weixinDraw.Phone))
            {
                return Redirect("http://www.shang-ke.cn/active/supervoteend?err=1");
            }
            ViewBag.WeixinDraw = weixinDraw;

            #endregion

            ViewBag.WeixinUser = weixinUserInfo;
            ViewBag.Openid = openid;
            ViewBag.Unionid = unionid;
            ViewBag.WeixinUid = wxuid;

            return View();
        }

        /// <summary>
        /// 大使活动的投票方法
        /// </summary>
        /// <param name="activeId"></param>
        /// <param name="weixinAccount"></param>
        /// <param name="sourceId"></param>
        /// <param name="sourceType"></param>
        /// <param name="reltionId"></param>
        /// <param name="sourceDrawid"></param>
        /// <param name="checkkey"></param>
        /// <returns></returns>
        public ActionResult GogogoVoteRecord(int activeId, string weixinAccount, int sourceId, int sourceType, int reltionId, int sourceDrawid, string checkkey)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict["State"] = "0";
            dict["Message"] = "投票失败";

            //是否可投票
            var _canGoVote = true;

            //获取当前用户的今日投票记录
            var _todayVoteRecord = new Weixin().GetActiveVoteRecordForType1ByWxAccount(activeId, weixinAccount, 1);
            if (_todayVoteRecord != null)
            {
                //今日次数已达5次
                if (_todayVoteRecord.Count >= 10)
                {
                    dict["State"] = "0";
                    dict["Message"] = "你今天的投票次数已用完，明天再来哦~";

                    _canGoVote = false;
                }
                else
                {
                    //一家酒店一天只能投一次
                    if (_todayVoteRecord.Exists(_ => _.SourceId == sourceId))
                    {
                        //并检测今天是否通过当前drawid（包括自己）投过该酒店
                        var _todayGoVotesForDrawid = new Weixin().GetActiveVoteRecordForType2BySourceIdAndReltionId(activeId, reltionId, sourceId, weixinAccount, 1);
                        if (_todayGoVotesForDrawid != null && _todayGoVotesForDrawid.Count > 0)
                        {
                            dict["State"] = "0";
                            dict["Message"] = "你今天已帮好友的这次打榜投过票啦~";

                            _canGoVote = false;
                        }
                    }
                }
            }

            if (_canGoVote)
            {
                //查询被投用户今天是第几次投票，如果是前3次，则弹出二维码进行投票
                var _sourceTodayVotes = sourceDrawid > 0 ? new Weixin().GetActiveVoteRecordForType2BySourceId(activeId, sourceDrawid, 1) : new List<ActiveVoteRecord>();
                if (sourceDrawid > 0 && (_sourceTodayVotes == null || _sourceTodayVotes.Count < 5))
                {
                    dict["State"] = "3";
                    dict["Message"] = "还差一步，帮人帮到底嘛~识别二维码帮我投一票";
                }
                else {

                    var _goVoteEntity = new ActiveVoteRecord
                    {
                        ID = 0,
                        WeixinAccount = weixinAccount,
                        UserId = 0,
                        SourceId = sourceId,
                        SourceType = sourceType,
                        ReltionId = reltionId,
                        State = 1,
                        ActiveId = activeId
                    };

                    var _add = new Weixin().AddActiveVoteRecord(_goVoteEntity);
                    if (_add == -1)
                    {
                        dict["State"] = "-1";
                        dict["Message"] = "投票异常";
                    }
                    else
                    {

                        dict["State"] = "1";
                        dict["Message"] = "投票成功";
                    }
                }

            }

            return Json(dict, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 大使活动的抽奖方法
        /// </summary>
        /// <param name="activeId"></param>
        /// <param name="activeDrawId"></param>
        /// <param name="openid"></param>
        /// <returns></returns>
        public ActionResult SuperVoteLuckDraw(int activeId, int activeDrawId, string openid)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict["State"] = "0";
            dict["Message"] = "";
            dict["Name"] = "";
            dict["Description"] = "";
            dict["Price"] = "0";
            dict["TagName"] = "";
            dict["GetLink"] = "";

            //首先查询当前用户的抽奖记录
            var _allLuckRecord = new Weixin().GetActiveLuckRecordAndPrizeByDrawId(activeDrawId) ?? new List<ActiveLuckDrawRecordContainPrize>();

            //首先检查今天是否已经抽过奖
            if (_allLuckRecord.Count > 0 && _allLuckRecord.Exists(_ => _.RecordTime.Date == DateTime.Now.Date))
            {
                dict["State"] = "0";
                dict["Message"] = "今日已抽奖，明天再来哦～";
            }
            else
            {
                #region 抽奖逻辑计算

                //获取所有奖品
                var _allPrize = new Weixin().GetActiveRulePrizeBySourceId(activeId, 0);

                //奖品池
                var _prizePool = new List<ActiveRulePrize>();

                //已经抽过几次
                var _sumDrawCount = _allLuckRecord.Count;

                //当前第几次抽奖
                var _thisDrawIndex = _sumDrawCount + 1;

                ////测试。。。
                //_thisDrawIndex = new Random().Next(1, 10);

                //根据次数，来确定当前抽奖的获奖规则和概率
                switch (_thisDrawIndex)
                {
                    //第一次：3-4元 微信红包
                    case 1:
                        {
                            _prizePool = _allPrize.Where(_ => _.Name == "微信红包" && _.Price >= 3 && _.Price <= 4).ToList();


                            break;
                        }
                    //第二次：4-5元 全场通用立减金
                    case 2:
                        {
                            _prizePool = _allPrize.Where(_ => _.Name == "立减金" && _.Price >= 4 && _.Price <= 5).ToList();


                            break;
                        }
                    //第三次：1-2元 微信红包
                    case 3:
                        {
                            _prizePool = _allPrize.Where(_ => _.Name == "微信红包" && _.Price >= 1 && _.Price <= 2).ToList();


                            break;
                        }
                    //第四次：100元 亚特兰蒂斯优惠券
                    case 4:
                        {
                            _prizePool = _allPrize.Where(_ => _.Name == "优惠券").ToList();


                            break;
                        }
                    //第五次：1-5元立减金
                    case 5:
                        {
                            _prizePool = _allPrize.Where(_ => _.Name == "立减金" && _.Price >= 1 && _.Price <= 5).ToList();


                            break;
                        }
                    default:
                        {
                            //第4次以后...
                            if (_thisDrawIndex > 5 && _thisDrawIndex < 10)
                            {
                                _prizePool = _allPrize.Where(_ => _.Name == "立减金" && _.Price >= 1 && _.Price <= 3).ToList();

                                ////追加不中的几率
                                //_prizePool.AddRange(_allPrize.Where(_ => _.Price == 0).ToList());


                            }
                            else
                            {
                                if (_thisDrawIndex >=  10)
                                {
                                    _prizePool = _allPrize.Where(_ => _.Name == "立减金" && _.Price >= 2 && _.Price <= 4).ToList();

                                    ////追加不中的几率
                                    //_prizePool.AddRange(_allPrize.Where(_ => _.Price == 0).ToList());


                                }
                            }
                            break;
                        }
                }

                //按照guid打乱
                _prizePool = _prizePool.OrderBy(_ => Guid.NewGuid()).ToList();

                //再随机取出一个
                Random _ran = new Random();
                int _ranNum = _ran.Next(0, _prizePool.Count - 1);

                //选出当前随机的获奖奖品
                var _selectPrizeItem = _prizePool[_ranNum];

                //存储抽奖记录
                var _activeLuckDrawRecord = new ActiveLuckDrawRecord();
                _activeLuckDrawRecord.ActiveDrawId = activeDrawId;
                _activeLuckDrawRecord.PrizeId = _selectPrizeItem.ID;
                _activeLuckDrawRecord.Remark = (_selectPrizeItem.Price > 0 ? string.Format("{0}{1}元", _selectPrizeItem.TagName, _selectPrizeItem.Price) : "未中奖");
                _activeLuckDrawRecord.ActiveId = activeId;
                _activeLuckDrawRecord.CreateTime = DateTime.Now;
                new Weixin().AddActiveLuckDrawRecord(_activeLuckDrawRecord);

                //判断如果是微信红包，需要插入红包发送队列
                if (_selectPrizeItem.Name == "微信红包" && !string.IsNullOrEmpty(openid))
                {
                    var _activeRedpackActiveName = "酒店亲子大使活动";
                    var _activeRedpackWishing = "记得每天来抽奖哦！";
                    var _activeRedpackRemark = "大使活动微信红包";
                    var _redpackSendName = "周末酒店APP";

                    //金额转换成分
                    var _redpackAmout = Convert.ToInt32(_selectPrizeItem.Price * 100);

                    #region 发红包

                    //发红包
                    var addEntity = new WeixinRewardRecord();
                    addEntity.SourceId = activeDrawId;
                    addEntity.SourceType = 4;       //1酒店订单 2酒店订单 3活动合作伙伴ID 4微信活动用户报名记录ID
                    addEntity.ReOpenid = openid;
                    addEntity.ReWxUid = activeId;     //ReWxUid存储当前微信活动ID
                    addEntity.Wishing = _activeRedpackWishing; //(thisPartnerEntity.Name.Length > 32 ? thisPartnerEntity.Name.Substring(0, 32) : thisPartnerEntity.Name);
                    addEntity.Amount = _redpackAmout; //单位 分
                    addEntity.Number = 1;
                    addEntity.ActiveId = activeId;
                    addEntity.ActiveName = _activeRedpackActiveName;
                    addEntity.Remark = _activeRedpackRemark;
                    addEntity.SceneId = 2;            //1:商品促销 2:抽奖 3:虚拟物品兑奖 4:企业内部福利 5:渠道分润 6:保险回馈 7:彩票派奖 8:税务刮奖
                    addEntity.SendName = _redpackSendName;   //"周末酒店";
                    addEntity.WillSendTime = DateTime.Now;
                    addEntity.RealSendTime = DateTime.Now;
                    addEntity.State = 0;
                    var sendRedpack = new Weixin().GenActiveRedPackReward(addEntity);

                    #endregion
                }

                dict["State"] = "1";
                dict["Message"] = "已抽到";
                dict["Name"] = _selectPrizeItem.Name;
                dict["Description"] = _selectPrizeItem.Description;
                dict["Price"] = _selectPrizeItem.Price.ToString();
                dict["TagName"] = _selectPrizeItem.TagName;
                dict["GetLink"] = _selectPrizeItem.Code;

                #endregion
            }

            return Json(dict, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 活动结束页面
        /// </summary>
        /// <param name="err"></param>
        /// <returns></returns>
        public ActionResult SuperVoteEnd(int err = 0)
        {
            //是否app环境
            ViewBag.IsApp = IsApp();

            //Weixin
            bool isInWeixin = Request.UserAgent.IndexOf("MicroMessenger") > 0 ? true : false;
            ViewBag.isInWeixin = isInWeixin;

            //是否非法访问
            ViewBag.Err = err;

            return View();
        }

        /// <summary>
        /// 极验初始化
        /// </summary>
        /// <returns></returns>
        public string GetCaptcha()
        {
            string geetestID = "03ec8ffe1f67c915111afc813f73e486";// System.Configuration.ConfigurationManager.AppSettings["geetestID"];
            string geetestKey = "9cd9b0548bfe305289c11b47e466cecb";// System.Configuration.ConfigurationManager.AppSettings["geetestKey"];
            GeetestLib geetest = new GeetestLib(geetestID, geetestKey);
            string userID = Guid.NewGuid().ToString();
            Byte gtServerStatus = geetest.preProcess(userID, "web", "127.0.0.1");
            Session[GeetestLib.gtServerStatusSessionKey] = gtServerStatus;
            Session["userID"] = userID;

            var _result = geetest.getResponseStr();

            return _result;
        }

        #endregion
    }
}
