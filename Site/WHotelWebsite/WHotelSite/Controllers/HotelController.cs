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
using WHotelSite.Params.Hotel;
using PersonalServices.Contract;
using WHotelSite.Common;
using HJD.AccountServices.Entity;
using HJD.AccessService.Contract.Model;
using HJDAPI.Models.RequestParams;
using HJDAPI.Models.ResponseModel;
using HJD.CouponService.Contracts.Entity;
using WHotelSite.Models;
using HJD.WeixinServices.Contracts;
using HJD.HotelManagementCenter.Domain;

namespace WHotelSite.Controllers
{
    public class HotelController : BaseController
    {
        public ActionResult Transfer(int channelid, string channelHotelID, DateTime startdate, DateTime depdate)
        {
            string url = string.Format("http://hotels.ctrip.com/hotel/{0}.html?allianceid=731899&sid=1273259&ouid={0}&startdate={1}&depdate={2}",
                channelHotelID, startdate.ToString("yyyy-MM-dd"), depdate.ToString("yyyy-MM-dd"));
            ViewBag.TransUrl = url;

            return View();
        }

        public ActionResult OtaTransfer(int hotelId, string checkIn, string checkOut, string sType = "wap")
        {
            var hotelPrice = Price.GetOtaList(hotelId, checkIn, checkOut, sType) ?? new HotelPrice2();

            ViewBag.HotelPrice = hotelPrice;
            ViewBag.SType = sType;
            return View();
        }

        // 酒店列表
        public ActionResult List(float userLat = 0, float userLng = 0, int city = 0, int sctype = 1, int interest = 0, int aroundcity = 0, int zoneid = 0, string districtName = "")
        {
            if (city == 0 && interest == 0 && aroundcity == 0 && zoneid == 0 && sctype == 1)
            {
                return HttpNotFound();
            }

            if (city == 0 && interest != 0 && sctype != 3)
            {
                var interests = Interest.GetAllInterest();
                var intr = interests.Find(p => p.ID == interest);
                if (intr == null || intr.Districtid == null)
                {
                    city = 2;
                }
                else
                {
                    city = intr.Districtid.Value;
                }
            }

            var isApp = IsApp();
            ViewBag.IsApp = isApp;

            //Weixin
            bool isInWeixin = Request.UserAgent.IndexOf("MicroMessenger") > 0 ? true : false;
            ViewBag.isInWeixin = isInWeixin;

            var isMobile = Utils.IsMobile();
            ViewBag.isMobile = isMobile;

            #region 左侧筛选项

            //查询当前城市的所有主题
            if (aroundcity > 0)
            {
                var homePageData30 = new InterestModel3();
                if (!isMobile)
                {
                    homePageData30 = new Hotel().GetHomePageData30(userLat, userLng, sctype, aroundcity);   
                }
                ViewBag.HomePageData30 = homePageData30;
            }
            else
            {
                var homePageData30 = new InterestModel3();
                if (!isMobile)
                {
                    homePageData30 = new Hotel().GetHomePageData30(userLat, userLng, sctype, city);
                }
                ViewBag.HomePageData30 = homePageData30;
            }

            //价格、星级筛选
            var distanceFilters = new List<FilterItem>(){
            new FilterItem(){ Name ="100公里以内", Value="0,100"},
            new FilterItem(){ Name ="100-200公里", Value="100,200"},
            new FilterItem(){ Name ="200-300公里", Value="200,300"}};
            ViewBag.DistanceFilters = distanceFilters;

            var priceFilters = new List<FilterItem>(){
            new FilterItem(){ Name ="400元以下", Value="0,400"},
            new FilterItem(){ Name ="400-600元", Value="400,600"},
            new FilterItem(){ Name ="600-800元", Value="600,800"},
            new FilterItem(){ Name ="800-1000元", Value="800,1000"},
            new FilterItem(){ Name ="1000-1500元", Value="1000,1500"},
            new FilterItem(){ Name ="1500-2000元", Value="1500,2000"},
            new FilterItem(){ Name ="2000元以上", Value="2000,150000"}};
            ViewBag.PriceFilters = priceFilters;

            var starFilters = new List<FilterItem>(){
                new FilterItem(){ Name ="五星", Value="5"},
                new FilterItem(){ Name ="四星", Value="4"},
                new FilterItem(){ Name ="三星及以下", Value="3"}
            };
            ViewBag.StarFilters = starFilters;

            #endregion

            //params
            ListParam param = new ListParam(this);
            param.City = city > 0 ? city : aroundcity;
            param.ScType = sctype;
            param.Interest = interest;

            var queryParam = new HotelListQueryParam20();
            queryParam.aroundCityId = aroundcity;
            queryParam.districtid = city;
            queryParam.start = param.Start;
            queryParam.count = param.Count;
            queryParam.districtLat = userLat;
            queryParam.districtLng = userLng;
            queryParam.lat = userLat;
            queryParam.lng = userLng;
            queryParam.geoScopeType = sctype;
            queryParam.zoneId = zoneid;
            if (param.Sort > 0) { queryParam.sort = param.Sort; }
            if (param.Interest > 0) { queryParam.interest = param.Interest; }

            if (param.Price != null)
            {
                queryParam.minPrice = param.MinPrice;
                queryParam.maxPrice = param.MaxPrice;

                //如果价格的条件不合理，则重定向至首页
                if ((!param.MinPrice.Equals(0) && !param.MinPrice.Equals(400) && !param.MinPrice.Equals(600) && !param.MinPrice.Equals(800) && !param.MinPrice.Equals(1000) && !param.MinPrice.Equals(1500) && !param.MinPrice.Equals(2000))
                    || (!param.MaxPrice.Equals(400) && !param.MaxPrice.Equals(600) && !param.MaxPrice.Equals(800) && !param.MaxPrice.Equals(1000) && !param.MaxPrice.Equals(1500) && !param.MaxPrice.Equals(2000) && !param.MaxPrice.Equals(150000)))
                {
                    return Redirect("/");
                }
            }
            if (param.Star > 0)
            {
                queryParam.star = param.Star.ToString();
            }

            ViewBag.param = param;

            ViewBag.userLat = userLat;
            ViewBag.userLng = userLng;
            ViewBag.aroundcity = aroundcity;
            ViewBag.zoneid = zoneid;

            //search
            var ret = new SearchHotelListResult();
            if (!isMobile)
            {
                ret = new Hotel().SearchHotelList30(queryParam);   
            }

            //如果是周边查询，可能没有cityid，那么通过坐标反查询出地理位置
            var disInfoEntity = new HJD.DestServices.Contract.DistrictInfoEntity { DistrictID = 0, Name = districtName, EName = "" };
            if (isMobile)
            {
                if (param.City > 0)
                {
                    disInfoEntity = Utils.GetDistrictInfo(param.City);
                }
            }
            else 
            {
                if (param.City == 0 && userLat > 0 && userLng > 0 && zoneid == 0)
                {
                    //根据坐标获取地理位置
                    var simpList = new Dest().GetAroundCityInfo(userLat, userLng);
                    if (simpList != null && simpList.Count > 0)
                    {
                        var simpObj = simpList.First();
                        disInfoEntity.DistrictID = simpObj.DistrictId;
                        disInfoEntity.Name = simpObj.Name;
                        disInfoEntity.EName = simpObj.EName;
                    }
                }
                else
                {
                    if (param.City == 0 && zoneid > 0)
                    {
                        if (ret != null && ret.Result20 != null && ret.Result20.Count > 0)
                        {
                            disInfoEntity.Name = ret.Result20[0].DistrictName;
                        }
                    }
                    else
                    {
                        disInfoEntity = Utils.GetDistrictInfo(param.City);
                    }
                }
            }
            ViewBag.city = disInfoEntity;

            //酒店列表页的筛选日历数据
            var calendar = new HotelPackageCalendar 
            { 
                DayItems = new List<HJD.HotelServices.Contracts.PDayItem>(), 
                DayLimitMax = 0, 
                DayLimitMin = 0 
            };

            //默认日历
            calendar.DayItems = GetDefaultCalendar(DateTime.Now.Date, 90, 1);
            
            ViewBag.Calendar = calendar;

            if (Request.IsAjaxRequest())
            {
                return View("ListContent", ret != null ? ret.Result20 : new List<ListHotelItemV43>());
            }

            //记录酒店列表页加载行为
            var value = string.Format("{{\"zone\":\"{0}\",\"theme\":\"{1}\",\"sight\":\"{2}\",\"price\":\"{3}\",\"star\":\"{4}\",\"sort\":\"{5}\",\"distance\":\"{6}\"}}",
                city, (param.Interest > 0 ? param.Interest.ToString() : ""), (param.ScType > 0 ? param.ScType.ToString() : ""), (param.Price != null ? param.Price : ""), (param.Star > 0 ? param.Star.ToString() : ""), (param.Sort > 0 ? param.Sort.ToString() : ""), (param.Distance != null ? param.Distance : ""));
            RecordBehavior("HotelListLoad", value);
            ViewBag.PageTag = "HotelList";

            return View(ret);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult ListMobile(int city = 0, string i = "12")
        {
            if (city == 0 && (string.IsNullOrEmpty(i) || i == "0"))
            {
                return HttpNotFound();
            }

            var isApp = IsApp();
            ViewBag.IsApp = isApp;

            //Weixin
            bool isInWeixin = Request.UserAgent.IndexOf("MicroMessenger") > 0 ? true : false;
            ViewBag.isInWeixin = isInWeixin;

            //酒店列表
            var hotelList = new List<ListHotelItem3>();

            //获取当前所有的主题Id
            var interestIds = i.Split(',');
            foreach (var interest in interestIds)
            {
                var ret = new Hotel().SearchInterestHotelWeixin(city, Convert.ToInt32(interest));
                if (ret != null && ret.Result.Count() > 0)
                {
                    foreach (var hotel in ret.Result)
                    {
                        if (!hotelList.Exists(h => h.Id == hotel.Id))
                        {
                            hotelList.Add(hotel);
                        }
                    }
                }
            }

            hotelList = hotelList.OrderByDescending(h => h.Score).ToList();

            //获取当前主题下有酒店的省份
            var pDic = new Dictionary<string, string>();
            foreach (var interest in interestIds)
            {
                var pList = new Hotel().GetProvinceListByInterest(Convert.ToInt32(interest));
                foreach (var pinfo in pList)
                {
                    if (!string.IsNullOrEmpty(pinfo) && pinfo.Contains(","))
                    {
                        var pid = pinfo.Split(',')[0];
                        var pname = pinfo.Split(',')[1];
                        pDic[pid] = pname;
                    }
                }
            }
            ViewBag.PDic = pDic;

            //热门省份列表
            var hotProvinceDic = new Dictionary<string, string>() 
            {
                {"2", "上海"},{"19570", "江苏"},{"5501", "浙江"},{"54586", "安徽"}
            };
            ViewBag.HotProvinceDic = hotProvinceDic;

            //当前主题
            var interestName = "";
            var interests = Interest.GetAllInterest();
            foreach (var interest in interestIds)
            {
                var intr = interests.Find(p => p.ID == Convert.ToInt32(interest));
                if (intr != null)
                {
                    if (!string.IsNullOrEmpty(interestName)) interestName += "、";
                    interestName += intr.Name;
                }
            }
            ViewBag.InterestName = interestName;
            ViewBag.Interest = i;

            //当前城市
            var pName = "全部";
            if (pDic.ContainsKey(city.ToString()))
            {
                pName = pDic[city.ToString()];
            }
            ViewBag.PName = pName;
            ViewBag.CityId = city;

            return View(hotelList);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inchina"></param>
        /// <param name="zone"></param>
        /// <returns></returns>
        public ActionResult ListZoneMobile(int inchina = 1, string zone = "inlall", int city = 0)
        {
            ViewBag.InChina = inchina;
            ViewBag.Zone = zone;

            #region 国内地域初始

            var inchinaDic = new Dictionary<string, Dictionary<string, string>>();

            //江
            inchinaDic["江苏"] = new Dictionary<string, string> 
            {
                {"19570", "江苏"}
            };

            //浙
            inchinaDic["浙江"] = new Dictionary<string, string> 
            {
                {"5501", "浙江"}
            };

            //沪
            inchinaDic["上海"] = new Dictionary<string, string> 
            {
                {"2", "上海"}
            };

            //皖
            inchinaDic["安徽"] = new Dictionary<string, string> 
            {
                {"54586", "安徽"}
            };

            //华东
            inchinaDic["华东"] = new Dictionary<string, string> 
            {
                {"19570", "江苏"},{"5501", "浙江"},{"2", "上海"},{"54586", "安徽"},{"24981", "山东"},{"36423", "福建"},{"56933", "江西"}
            };

            //华南
            inchinaDic["华南"] = new Dictionary<string, string> 
            {
                {"47085", "广东"},{"10167", "广西"},{"7926", "海南"}
            };

            //华中
            inchinaDic["华中"] = new Dictionary<string, string> 
            {
                {"58681", "湖北"},{"59568", "湖南"},{"35090", "河南"}
            };

            //华北
            inchinaDic["华北"] = new Dictionary<string, string> 
            {
                {"1", "北京"},{"5446", "天津"},{"72823", "河北"},{"19429", "山西"},{"31874", "内蒙古"}
            };

            //东北
            inchinaDic["东北"] = new Dictionary<string, string> 
            {
                {"53084", "辽宁"},{"324", "吉林"},{"77493", "黑龙江"}
            };

            //西北
            inchinaDic["西北"] = new Dictionary<string, string> 
            {
                {"7098", "宁夏"},{"5169", "新疆"},{"74554", "青海"},{"33013", "陕西"},{"15400", "甘肃"}
            };

            //西南
            inchinaDic["西南"] = new Dictionary<string, string> 
            {
                {"61253", "四川"},{"48245", "云南"},{"48253", "贵州"},{"49687", "西藏"},{"15729", "重庆"}
            };

            ViewBag.InChinaDic = inchinaDic;

            #endregion

            #region 国外地域初始

            var unchinaDic = new Dictionary<string, Dictionary<string, string>>();

            //all
            unchinaDic["inlall"] = new Dictionary<string, string> 
            {
                {"47720", "香港"},{"35447", "澳门"},{"24138", "台湾"},{"27882", "日本"},{"5831", "韩国"}
                ,{"21648", "新加坡"},{"28757", "泰国"},{"44459", "马来西亚"},{"75387", "英国"}
                ,{"14862", "法国"},{"54565", "美国"},{"20126", "加拿大"},{"54936", "澳大利亚"}
            };

            //日韩
            unchinaDic["日韩"] = new Dictionary<string, string> 
            {
                {"27882", "日本"},{"5831", "韩国"}
            };

            //东南亚
            unchinaDic["东南亚"] = new Dictionary<string, string> 
            {
                {"10488", "菲律宾"},{"21648", "新加坡"},{"28757", "泰国"},{"40141", "缅甸"},{"43627", "印度尼西亚"},{"44459", "马来西亚"},{"50688", "文莱"},{"61398", "柬埔寨"}
            };

            //欧洲
            unchinaDic["欧洲"] = new Dictionary<string, string> 
            {
                {"2246", "奥地利"},{"7242", "荷兰"},{"10939", "意大利"},{"12302", "挪威"},{"13088", "冰岛"},
                {"14057", "瑞士"},{"14862", "法国"},{"43007", "丹麦"},{"47652", "西班牙"},{"50637", "芬兰"},
                {"56781", "希腊"},{"60021", "罗马尼亚"},{"63759", "德国"},{"64606", "葡萄牙"},{"70503", "波兰"},
                {"75387", "英国"},{"76152", "俄罗斯"},{"77670", "瑞典"}
            };

            //北美
            unchinaDic["北美"] = new Dictionary<string, string> 
            {
                {"54565", "美国"},{"20126", "加拿大"}
            };

            //澳新
            unchinaDic["澳新"] = new Dictionary<string, string> 
            {
                {"54936", "澳大利亚"},{"21648", "新加坡"}
            };

            //港澳
            unchinaDic["港澳"] = new Dictionary<string, string> 
            {
                {"47720", "香港"},{"35447", "澳门"}
            };

            //台湾
            unchinaDic["台湾"] = new Dictionary<string, string> 
            {
                {"24138", "台湾"}
            };

            ViewBag.UnChinaDic = unchinaDic;

            #endregion


            #region 统一处理

            //默认值
            if (city == 0)
            {
                city = inchina == 1 ? 2 : 32781; //32781,新加坡
            }

            //获取当前主题下有酒店的省份
            var pDic = new Dictionary<string, string>();
            var pList = inchina == 1 ? new Hotel().GetProvinceListByInterest(0) : new Hotel().GetProvinceListByInterest(-1);
            foreach (var pinfo in pList)
            {
                if (!string.IsNullOrEmpty(pinfo) && pinfo.Contains(","))
                {
                    var pid = pinfo.Split(',')[0];
                    var pname = pinfo.Split(',')[1];
                    pDic[pid] = pname;
                }
            }
            ViewBag.PDic = pDic;

            //热门省份列表
            var hotProvinceDic = new Dictionary<string, string>() 
            {
                {"2", "上海"},{"19570", "江苏"},{"5501", "浙江"},{"54586", "安徽"}
            };
            ViewBag.HotProvinceDic = hotProvinceDic;

            //当前城市
            var pName = "全部";
            if (pDic.ContainsKey(city.ToString()))
            {
                pName = pDic[city.ToString()];
            }
            ViewBag.PName = pName;
            ViewBag.CityId = city;

            //酒店列表
            var hotelList = new List<ListHotelItem3>();
            var ret = new Hotel().SearchInterestHotelWeixin(city, 0);
            if (ret != null && ret.Result.Count() > 0)
            {
                foreach (var hotel in ret.Result)
                {
                    if (!hotelList.Exists(h => h.Id == hotel.Id))
                    {
                        hotelList.Add(hotel);
                    }
                }
            }

            #endregion

            #region 国外相关处理(旧的方式)

            ////当前地区
            //var thisZoneDic = new Dictionary<string, string>();
            //if (inchina == 1 && inchinaDic.ContainsKey(zone))
            //{
            //    thisZoneDic = inchinaDic[zone];
            //}
            //else if (inchina == 0 && unchinaDic.ContainsKey(zone))
            //{
            //    thisZoneDic = unchinaDic[zone];
            //}

            //酒店列表
            var hotelListDic = new Dictionary<string, List<ListHotelItem3>>();
            //if (inchina == 0)
            //{
            //    foreach (var cityKey in thisZoneDic.Keys)
            //    {
            //        var cityName = thisZoneDic[cityKey];
            //        var hlist = new List<ListHotelItem3>();
            //        var ret = new Hotel().SearchInterestHotelWeixin(Convert.ToInt32(cityKey), 0);
            //        if (ret != null && ret.Result.Count() > 0)
            //        {
            //            foreach (var hotel in ret.Result)
            //            {
            //                if (!hlist.Exists(h => h.Id == hotel.Id))
            //                {
            //                    hlist.Add(hotel);
            //                }
            //            }
            //            hlist = hlist.OrderByDescending(h => h.Score).ToList();
            //            hotelListDic[cityName] = hlist;
            //        }
            //    }   
            //}
            ViewBag.HotelListDic = hotelListDic;

            #endregion


            return View(hotelList);
        }

        /// <summary>
        /// 2016-09-29
        /// </summary>
        /// <param name="userLat"></param>
        /// <param name="userLng"></param>
        /// <param name="city"></param>
        /// <param name="sctype"></param>
        /// <param name="interest"></param>
        /// <param name="aroundcity"></param>
        /// <param name="zoneid"></param>
        /// <returns></returns>
        public ActionResult List2(float userLat = 0, float userLng = 0, int city = 0, int sctype = 1, int interest = 0, int aroundcity = 0, int zoneid = 0)
        {
            var isApp = IsApp();
            ViewBag.IsApp = isApp;

            if (city == 0 && interest == 0 && aroundcity == 0 && zoneid == 0)
            {
                return HttpNotFound();
            }

            if (city == 0 && interest != 0 && sctype != 3)
            {
                var interests = Interest.GetAllInterest();
                var intr = interests.Find(p => p.ID == interest);
                if (intr == null || intr.Districtid == null)
                {
                    city = 2;
                }
                else
                {
                    city = intr.Districtid.Value;
                }
            }

            ViewBag.isMobile = Utils.IsMobile();

            #region 左侧筛选项

            //查询当前城市的所有主题
            if (aroundcity > 0)
            {
                var homePageData30 = new Hotel().GetHomePageData30(userLat, userLng, sctype, aroundcity);
                ViewBag.HomePageData30 = homePageData30;
            }
            else
            {
                var homePageData30 = new Hotel().GetHomePageData30(userLat, userLng, sctype, city);
                ViewBag.HomePageData30 = homePageData30;
            }

            //价格、星级筛选
            var distanceFilters = new List<FilterItem>(){
            new FilterItem(){ Name ="100公里以内", Value="0,100"},
            new FilterItem(){ Name ="100-200公里", Value="100,200"},
            new FilterItem(){ Name ="200-300公里", Value="200,300"}};
            ViewBag.DistanceFilters = distanceFilters;

            var priceFilters = new List<FilterItem>(){
            new FilterItem(){ Name ="400元以下", Value="0,400"},
            new FilterItem(){ Name ="400-600元", Value="400,600"},
            new FilterItem(){ Name ="600-800元", Value="600,800"},
            new FilterItem(){ Name ="800-1000元", Value="800,1000"},
            new FilterItem(){ Name ="1000-1500元", Value="1000,1500"},
            new FilterItem(){ Name ="1500-2000元", Value="1500,2000"},
            new FilterItem(){ Name ="2000元以上", Value="2000,150000"}};
            ViewBag.PriceFilters = priceFilters;

            var starFilters = new List<FilterItem>(){
                new FilterItem(){ Name ="五星", Value="5"},
                new FilterItem(){ Name ="四星", Value="4"},
                new FilterItem(){ Name ="三星及以下", Value="3"}
            };
            ViewBag.StarFilters = starFilters;

            #endregion

            //params
            ListParam param = new ListParam(this);
            param.City = city > 0 ? city : aroundcity;
            param.ScType = sctype;
            param.Interest = interest;

            var queryParam = new HotelListQueryParam20();
            queryParam.aroundCityId = aroundcity;
            queryParam.districtid = city;
            queryParam.start = param.Start;
            queryParam.count = param.Count;
            queryParam.districtLat = userLat;
            queryParam.districtLng = userLng;
            queryParam.lat = userLat;
            queryParam.lng = userLng;
            queryParam.geoScopeType = sctype;
            queryParam.zoneId = zoneid;
            if (param.Sort > 0) { queryParam.sort = param.Sort; }
            if (param.Interest > 0) { queryParam.interest = param.Interest; }

            if (param.Price != null)
            {
                queryParam.minPrice = param.MinPrice;
                queryParam.maxPrice = param.MaxPrice;

                //如果价格的条件不合理，则重定向至首页
                if ((!param.MinPrice.Equals(0) && !param.MinPrice.Equals(400) && !param.MinPrice.Equals(600) && !param.MinPrice.Equals(800) && !param.MinPrice.Equals(1000) && !param.MinPrice.Equals(1500) && !param.MinPrice.Equals(2000))
                    || (!param.MaxPrice.Equals(400) && !param.MaxPrice.Equals(600) && !param.MaxPrice.Equals(800) && !param.MaxPrice.Equals(1000) && !param.MaxPrice.Equals(1500) && !param.MaxPrice.Equals(2000) && !param.MaxPrice.Equals(150000)))
                {
                    return Redirect("/");
                }
            }
            if (param.Star > 0)
            {
                queryParam.star = param.Star.ToString();
            }

            ViewBag.param = param;

            //search
            var ret = new Hotel().SearchHotelList20(queryParam);

            //如果是周边查询，可能没有cityid，那么通过坐标反查询出地理位置
            var disInfoEntity = new HJD.DestServices.Contract.DistrictInfoEntity { DistrictID = 0, Name = "", EName = "" };
            if (param.City == 0 && userLat > 0 && userLng > 0 && zoneid == 0)
            {
                //根据坐标获取地理位置
                var simpList = new Dest().GetAroundCityInfo(userLat, userLng);
                if (simpList != null && simpList.Count > 0)
                {
                    var simpObj = simpList.First();
                    disInfoEntity.DistrictID = simpObj.DistrictId;
                    disInfoEntity.Name = simpObj.Name;
                    disInfoEntity.EName = simpObj.EName;
                }
            }
            else
            {
                if (param.City == 0 && zoneid > 0)
                {
                    if (ret != null && ret.Result20 != null && ret.Result20.Count > 0)
                    {
                        disInfoEntity.Name = ret.Result20[0].DistrictName;
                    }
                }
                else
                {
                    disInfoEntity = Utils.GetDistrictInfo(param.City);
                }
            }
            ViewBag.city = disInfoEntity;

            if (Request.IsAjaxRequest())
            {
                return View("ListContent2", ret.Result20);
            }

            ViewBag.PageTag = "HotelList";

            return View(ret);
        }

        private string GetQuery(HotelListQueryParam p)
        {
            string url = string.Format("http://api.zmjiudian.com/api/hotel/QueryInterestHotel?districtid={0}&lat={1}&lng={2}&distance={3}&hotelid={4}&type={5}&sort={6}&order={7}&start={8}&count={9}&checkin={10}&checkout={11}&nLat={12}&nLng={13}&sLat={14}&sLng={15}&valued={16}&tag={17}&minPrice={18}&maxPrice={19}&location={20}&zone={21}&brand={22}&attraction={23}&featured={24}&star={25}&geoScopeType={26}&fromDistance={27}&Interest={28}", (object)p.districtid, (object)p.lat, (object)p.lng, (object)p.distance, (object)p.hotelid, (object)p.type, (object)p.sort, (object)p.order, (object)p.start, (object)p.count, (object)p.checkin, (object)p.checkout, (object)p.nLat, (object)p.nLng, (object)p.sLat, (object)p.sLng, (object)p.valued, (object)p.tag, (object)p.minPrice, (object)p.maxPrice, (object)p.location, (object)p.zone, (object)p.brand, (object)p.attraction, (object)p.featured, (object)p.star, (object)p.geoScopeType, (object)p.fromDistance, (object)p.Interest);
            return url;

        }

        /// <summary>
        /// 获取一个指定日期量的空日历列表
        /// </summary>
        private static List<HJD.HotelServices.Contracts.PDayItem> GetDefaultCalendar(DateTime startDate, int canlendarLength = 90, int defSellState = 0)
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
                d.SellState = defSellState;

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
        /// 酒店详情页
        /// </summary>
        /// <param name="sign"></param>
        /// <param name="pid"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public ActionResult Detail(string sign = "", int pid = 0, int userid = 0)
        {
            if (HttpContext.Request.Url != null && !string.IsNullOrEmpty(HttpContext.Request.Url.AbsoluteUri))
            {
                var _rUrl = "";
                var _absUri = HttpContext.Request.Url.AbsoluteUri;
                if (_absUri.ToLower().Contains("www.shangjiudian.com"))
                {
                    _rUrl = _absUri.ToLower().Replace("www.shangjiudian.com", "www.zmjiudian.com").Replace("https://", "http://");
                    return Redirect(_rUrl);
                }
                else if (_absUri.ToLower().Contains("https://"))
                {
                    _rUrl = _absUri.ToLower().Replace("https://", "http://");
                    return Redirect(_rUrl);
                }
            }

            var isApp = IsApp();
            ViewBag.IsApp = isApp;

            var isMobile = Utils.IsMobile();
            ViewBag.IsMobile = isMobile;

            //Weixin
            bool isInWeixin = Request.UserAgent.IndexOf("MicroMessenger") > 0 ? true : false;
            ViewBag.isInWeixin = isInWeixin;

            var curUserID = UserState.UserID > 0 ? UserState.UserID : 0;// ClientHelper.GetBotaoUserIdFromCookie(HttpContext.Request.Cookies);
            if (curUserID <= 0 && userid > 0)
            {
                curUserID = userid;
            }

            ViewBag.UserId = curUserID;

            DetailParam param = new DetailParam(this);

            //当前用户是否VIP
            var isVip = false; if (curUserID > 0) isVip = account.IsVIPCustomer(new AccountInfoItem { Uid = curUserID.ToString() });
            ViewBag.IsVip = isVip;

            //CID
            ViewBag.CID = GetCurCID();

            //获取请求参数中的CID
            var requestCID = GetCurCIDForRequest();
            ViewBag.RequestCID = requestCID;

            #region 铂韬信息验证

            var isBotaoUser = false;
            //var bohuijinrongStr = HJDAPI.Common.Helpers.Enums.ThirdPartyMerchantCode.bohuijinrong.ToString();
            //var bohuijinronguser = "bohuijinronguserid";
            //if (!string.IsNullOrWhiteSpace(sign))
            //{
            //    sign = sign.Replace(" ", "+");
            //    var desStr = HJDAPI.Common.Security.DES.Decrypt(sign, HJDAPI.Common.Security.DES.bohuijinrongDESKey);
            //    if (!string.IsNullOrWhiteSpace(desStr))
            //    {
            //        var desArray = desStr.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            //        if (desArray != null && desArray.Length > 1 && desArray[1].Equals(bohuijinrongStr))
            //        {
            //            var operationResult = account.RegisterWithoutPassword(new RegistPhoneUserItem()
            //            {
            //                Phone = desArray[0],
            //                Sign = "",
            //                SourceID = 0,
            //                TimeStamp = 0,
            //                ConfirmPassword = "",
            //                Password = "",
            //                RequestType = "RegisterWithoutPassword"
            //            });

            //            if (operationResult.Success && operationResult.UserID > 0)
            //            {
            //                ViewBag.UserId = operationResult.UserID;
            //                if (HttpContext.Request.Cookies[bohuijinronguser] == null)
            //                {
            //                    HttpCookie cookie = new HttpCookie(bohuijinronguser);
            //                    cookie.Value = HJDAPI.Common.Security.DES.Encrypt(operationResult.UserID.ToString(), HJDAPI.Common.Security.DES.bohuijinrongDESKey);
            //                    cookie.Expires = DateTime.Now.AddDays(1);
            //                    cookie.HttpOnly = true;
            //                    HttpContext.Response.Cookies.Add(cookie);
            //                }
            //                else
            //                {
            //                    HttpCookie cookie = new HttpCookie(bohuijinronguser);
            //                    cookie.Value = HJDAPI.Common.Security.DES.Encrypt(operationResult.UserID.ToString(), HJDAPI.Common.Security.DES.bohuijinrongDESKey);
            //                    cookie.Expires = DateTime.Now.AddDays(1);
            //                    cookie.HttpOnly = true;
            //                    HttpContext.Response.Cookies.Set(cookie);
            //                }

            //                if (HttpContext.Request.Cookies[bohuijinrongStr] == null)
            //                {
            //                    HttpCookie cookie = new HttpCookie(bohuijinrongStr);
            //                    cookie.Value = HJDAPI.Common.Security.DES.Encrypt(desArray[0], HJDAPI.Common.Security.DES.bohuijinrongDESKey);
            //                    cookie.Expires = DateTime.Now.AddDays(1);
            //                    cookie.HttpOnly = true;
            //                    HttpContext.Response.Cookies.Add(cookie);
            //                }
            //                else
            //                {
            //                    HttpCookie cookie = new HttpCookie(bohuijinrongStr);
            //                    cookie.Value = HJDAPI.Common.Security.DES.Encrypt(desArray[0], HJDAPI.Common.Security.DES.bohuijinrongDESKey);
            //                    cookie.Expires = DateTime.Now.AddDays(1);
            //                    cookie.HttpOnly = true;
            //                    HttpContext.Response.Cookies.Set(cookie);
            //                }

            //                account.InsertOrDeleteUserPrivilegeRel(new UserPriviledgeInsertParam()
            //                {
            //                    IsAdd = true,
            //                    PriviledgeId = 2001,
            //                    UserId = operationResult.UserID
            //                });
            //            }
            //            isBotaoUser = true;
            //        }
            //        else
            //        {
            //            LogHelper.WriteLog(string.Format("签名验证失败，酒店ID:{0}，参数sign:{1}", param.HotelId, sign));
            //        }
            //    }
            //}
            //else
            //{
            //    if (HttpContext.Request.Cookies[bohuijinronguser] != null)
            //    {
            //        HttpCookie cookie = new HttpCookie(bohuijinronguser);
            //        cookie.Expires = DateTime.Now.AddDays(-10);
            //        HttpContext.Response.Cookies.Set(cookie);
            //    }

            //    if (HttpContext.Request.Cookies[bohuijinrongStr] != null)
            //    {
            //        HttpCookie cookie = new HttpCookie(bohuijinrongStr);
            //        cookie.Expires = DateTime.Now.AddDays(-10);
            //        HttpContext.Response.Cookies.Set(cookie);
            //    }
            //}

            #endregion

            //原始h5版本的酒店详情页对象
            HotelItem5 hotelEntity = new HotelItem5();

            //app同步版本的酒店详情页对象
            HotelItem6 hotelEntityNew = new HotelItem6();

            var hotelName = "";

            //如果移动版访问，则直接使用app同步版本的api
            if (isMobile)
            {
                hotelEntityNew = new Hotel().GetHotelDetail(param.HotelId, param.CheckIn, param.CheckOut, Utils.ClientType(), 0, curUserID, "", pid: pid);
                hotelName = hotelEntityNew.HotelName;
            }
            else
            {
                hotelEntity = new Hotel().Get50(param.HotelId, param.CheckIn, param.CheckOut, Utils.ClientType(), 0, curUserID, "", pid: pid);
                hotelName = hotelEntity.HotelName;
            }

            ViewBag.HotelEntity = hotelEntity;
            ViewBag.HotelEntityNew = hotelEntityNew;

            param.CheckInDate = hotelEntity.PriceCinDate;
            param.CheckOutDate = hotelEntity.PriceCouDate;

            if (hotelEntity.PackageList != null && hotelEntity.PackageList.Count > 0)
            {
                ViewBag.calendar = HJDAPI.APIProxy.Price.GetHotelPackageCalendar30(param.HotelId, DateTime.Now.Date);
            }

            ViewBag.showFullHeart = false;
            ViewBag.showAirHeart = true;
            if (UserState.UserID != 0)
            {
                CollectHotelResult result = Collect.GetCollectHotelList(UserState.UserID);
                if (result != null && result.TotalCount != 0 && result.Hotels.Count<CollectListItem>(i => i.Id == param.HotelId) > 0)
                {
                    ViewBag.showFullHeart = true;
                    ViewBag.showAirHeart = false;
                }
            }

            //记录酒店详情页加载
            var value = string.Format("{{\"hotelid\":\"{0}\",\"checkin\":\"{1}\",\"checkout\":\"{2}\",\"hotelname\":\"{3}\",\"userType\":\"{4}\",\"userNo\":\"{5}\"}}", param.HotelId, param.CheckIn, param.CheckOut, hotelName, isBotaoUser ? "botao" : "zmjd", curUserID);
            RecordBehavior("HotelLoad", value);
            ViewBag.PageTag = "Hotel";

            ViewBag.param = param;

            //获取产品店铺信息
            var productShopInfo = new HJD.HotelManagementCenter.Domain.RetailerShopEntity();
            if (requestCID > 0)
            {
                //查询指定CID的店铺信息
                productShopInfo = Shop.GetRetailerShopByCID(requestCID);
            }
            ViewBag.ProductShopInfo = productShopInfo;

            return View();
        }

        // 酒店地图
        public ActionResult Map()
        {
            DetailParam param = new DetailParam(this);

            //酒店地图页加载
            var value = string.Format("{{\"hotelid\":\"{0}\"}}", param.HotelId);
            RecordBehavior("MapLoad", value);
            ViewBag.PageTag = "Map";

            return View(new Hotel().GetHotelMapInfo(param.HotelId));//2015-09-28 wwb修改酒店地图信息
            //return View((new Hotel()).Get2(param.HotelId, "0", "0", Utils.ClientType(), 0));
        }

        // 酒店照片
        public ActionResult Photos()
        {
            DetailParam param = new DetailParam(this);
            return View(new Hotel().GetHotelPhotos(param.HotelId));
        }

        // 酒店点评
        public ActionResult Reviews(int feature = 0, int pid = 0)
        {
            ReviewsParam param = new ReviewsParam(this);
            param.Feature = feature;
            HotelItem3 hotel = HJDAPI.APIProxy.Hotel.Get3(param.HotelId, "0", "0", Utils.ClientType(), 0);
            if (param.Interest == 0)
            {
                param.Interest = hotel.InterestID;
            }
            ReviewQueryParam queryParam = new ReviewQueryParam();
            queryParam.Hotel = param.HotelId;
            queryParam.Start = param.Start;
            queryParam.Count = param.Count;
            if (param.Rating > 0)
            {
                queryParam.RatingType = param.Rating;
            }
            else if (param.Feature > 0)
            {
                queryParam.InterestID = param.Interest;
                queryParam.TFTType = param.TFTType == 0 ? 1 : param.TFTType;
                queryParam.TFTID = param.Feature;
            }
            else if (param.TFTType > 0)
            {
                queryParam.InterestID = param.Interest;
                queryParam.TFTType = param.TFTType;
            }

            ViewBag.param = param;
            ViewBag.hotel = hotel;
            ViewBag.Pid = pid;

            //查询出点评数据
            var ret = new ReviewResult40();

            if (pid > 0)
            {
                ret = Comment.GetCommentInfosByPId(pid, param.Start, param.Count);
            }
            else
            {
                //ret = new Hotel().GetComments(queryParam);
                ret = new Hotel().GetComments50(queryParam);
            }

            if (Request.IsAjaxRequest())
            {
                return View("ReviewsContent", ret.Result);
            }

            //记录酒店点评页加载
            var value = string.Format("{{\"hotelid\":\"{0}\",\"rating\":\"{1}\",\"feature\":\"{2}\",\"tftType\":\"{3}\"}}",
                param.HotelId, (param.Rating > 0 ? param.Rating.ToString() : ""), (param.Feature > 0 ? param.Feature.ToString() : ""), (param.TFTType > 0 ? param.TFTType.ToString() : ""));
            RecordBehavior("ReviewsLoad", value);
            ViewBag.PageTag = "Reviews";

            return View(ret);
        }

        /// <summary>
        /// 单个套餐详情页
        /// </summary>
        /// <param name="pid"></param>
        /// <param name="userid"></param>
        /// <param name="albumid"></param>
        /// <param name="showcld"></param>
        /// <param name="checkInStr"></param>
        /// <param name="checkOutStr"></param>
        /// <param name="distributioncid"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public ActionResult Package(int pid = 0, int userid = 0, int albumid = 0, int showcld = 0, string checkInStr = "", string checkOutStr = "", long distributioncid = 0, string code = "")
        {
            var isApp = IsApp();
            var curUserID = Convert.ToInt32(UserState.UserID > 0 ? UserState.UserID : (isApp ? Convert.ToInt64(userid) : 0));

            //获取请求参数中的CID
            var requestCID = GetCurCIDForRequest();
            ViewBag.RequestCID = requestCID;

            //Weixin
            bool isInWeixin = Request.UserAgent.IndexOf("MicroMessenger") > 0 ? true : false;
            ViewBag.isInWeixin = isInWeixin;

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
                            weixinGoUrl = WeiXinHelper.GenSilenceAuthorUrlForHaoYi(HttpUtility.UrlEncode(string.Format("http://www.zmjiudian.com/Hotel/Package/{0}?userid={1}&albumid={2}&showcld={3}&checkInStr={4}&checkOutStr={5}&CID={6}&distributioncid={7}", pid, userid, albumid, showcld, checkInStr, checkOutStr, requestCID, distributioncid)));
                        }
                        else
                        {
                            weixinGoUrl = WeiXinHelper.GenSilenceAuthorUrlForHaoYi(HttpUtility.UrlEncode(string.Format("http://www.zmjiudian.com/Hotel/Package/{0}?userid={1}&albumid={2}&showcld={3}&checkInStr={4}&checkOutStr={5}&distributioncid={6}", pid, userid, albumid, showcld, checkInStr, checkOutStr, distributioncid)));
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
                        weixinGoUrl = WeiXinHelper.GenSilenceAuthorUrlForHaoYi(HttpUtility.UrlEncode(string.Format("http://www.zmjiudian.com/Hotel/Package/{0}?userid={1}&albumid={2}&showcld={3}&checkInStr={4}&checkOutStr={5}&CID={6}&distributioncid={7}", pid, userid, albumid, showcld, checkInStr, checkOutStr, requestCID, distributioncid)));
                    }
                    else
                    {
                        weixinGoUrl = WeiXinHelper.GenSilenceAuthorUrlForHaoYi(HttpUtility.UrlEncode(string.Format("http://www.zmjiudian.com/Hotel/Package/{0}?userid={1}&albumid={2}&showcld={3}&checkInStr={4}&checkOutStr={5}&distributioncid={6}", pid, userid, albumid, showcld, checkInStr, checkOutStr, distributioncid)));
                    }
                    return Redirect(weixinGoUrl);
                }
            }
            ViewBag.Unionid = weixinUserInfo.Unionid;

            #endregion

            return PackageContent(pid, userid, albumid, showcld, checkInStr, checkOutStr, distributioncid);
        }

        /// <summary>
        /// 套餐着陆页内容部分
        /// </summary>
        /// <param name="pid"></param>
        /// <param name="userid"></param>
        /// <param name="albumid"></param>
        /// <param name="showcld"></param>
        /// <param name="checkInStr"></param>
        /// <param name="checkOutStr"></param>
        /// <param name="distributioncid"></param>
        /// <returns></returns>
        public ActionResult PackageContent(int pid = 0, int userid = 0, int albumid = 0, int showcld = 0, string checkInStr = "", string checkOutStr = "", long distributioncid = 0)
        {
            var isApp = IsApp();
            ViewBag.IsApp = isApp;
            ViewBag.Pid = pid;
            ViewBag.AlbumId = albumid;
            ViewBag.ShowCalendar = showcld;
             
            //HJDAPI.Common.Helpers.TimeLog log = new HJDAPI.Common.Helpers.TimeLog(string.Format("PackageContent:{0}", pid), 3 * 1000);
            //Weixin
            bool isInWeixin = Request.UserAgent.IndexOf("MicroMessenger") > 0 ? true : false;
            ViewBag.isInWeixin = isInWeixin;
            //log.AddLog("isInWeixin");
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
            //log.AddLog("调试使用");

            //检测是否大于等于4.6版本（4.6之后app支持h5跳转至原生预订页面）
            var isThanVer46 = IsThanVer4_6();
            ViewBag.IsThanVer46 = isThanVer46;

            //是否大于等于4.7
            var isThanVer47 = IsThanVer4_7();
            ViewBag.IsThanVer47 = isThanVer47;

            var curUserID = Convert.ToInt32(UserState.UserID > 0 ? UserState.UserID : (isApp ? Convert.ToInt64(userid) : 0));
            ViewBag.UserId = curUserID;
            userid = curUserID;
            //log.AddLog("curUserID");

            //如果当前抢购VIP专享，则判断当前用户是否为VIP
            var isVip = account.IsVIPCustomer(new AccountInfoItem { Uid = curUserID.ToString() });
            ViewBag.IsVip = isVip;

            //获取当前套餐信息
            var startTime = DateTime.Now.Date.AddDays(1);
            //RecommendPackageDetailResult packageEntity = Hotel.GetPackageDetailResult(pid, userid, startTime.ToString("yyyy-MM-dd"), "");
            RecommendPackageDetailResult packageEntity = Hotel.GetPackageDetailInfo(pid, userid, startTime.ToString("yyyy-MM-dd"), "");
            
            ViewBag.PackageEntity = packageEntity;
            //log.AddLog("GetPackageDetailResult");

            //当前套餐房型
            SameSerialPackageItem thisPackageRoomInfo = new SameSerialPackageItem();
            if (packageEntity.serialPackageList != null && packageEntity.serialPackageList.Count > 0)
            {
                thisPackageRoomInfo = packageEntity.serialPackageList.Find(p => p.pId == pid);
            }
            ViewBag.ThisPackageRoomInfo = thisPackageRoomInfo;
            //log.AddLog("packageEntity.serialPackageList");

            //默认日期
            var addNightCount = 1;
            var checkIn = DateTime.Now.Date;
            var checkOut = checkIn.AddDays(addNightCount);
            var defPrice = packageEntity.packageItem.HotelPrice;

            //是否使用日期参数
            var useDateParam = false;

            //当前套餐的可用日期
            var calendar = new HotelPackageCalendar { DayItems = new List<HJD.HotelServices.Contracts.PDayItem>(), DayLimitMax = 0, DayLimitMin = 0 };
            try
            {
                calendar = HJDAPI.APIProxy.Price.GetOneHotelPackageCalendar(packageEntity.packageItem.HotelID, DateTime.Now.Date, packageEntity.packageItem.PID, Convert.ToInt64(userid));
            }
            catch (Exception ex)
            {

            }
            //log.AddLog("GetOneHotelPackageCalendar");

            //默认总价
            var totalPrice = packageEntity.packageItem.NotVIPPrice * (calendar.DayLimitMin < 1 ? 1 : calendar.DayLimitMin);
            var totalVipPrice = packageEntity.packageItem.VIPPrice * (calendar.DayLimitMin < 1 ? 1 : calendar.DayLimitMin);


            if (calendar != null && calendar.DayItems != null && calendar.DayItems.Count > 0 && calendar.DayItems.Exists(d => d.SellState == 1))
            {
                //如果指定了日期，并且该日期可售，那么以指定日期为准
                if (!string.IsNullOrEmpty(checkInStr) && !string.IsNullOrEmpty(checkOutStr))
                {
                    var _checkInDate = DateTime.Parse(checkInStr);
                    var _checkOutDate = DateTime.Parse(checkOutStr);
                    if (calendar.DayItems.Exists(_ => _.Day == _checkInDate) && calendar.DayItems.Exists(_ => _.Day == _checkOutDate) && !calendar.DayItems.Exists(_ => _.Day >= _checkInDate && _.Day < _checkOutDate && _.SellState != 1))
                    {
                        checkIn = _checkInDate;
                        checkOut = _checkOutDate;

                        //所有范围内的日期
                        var allSeledDates = calendar.DayItems.Where(_ => _.Day >= _checkInDate && _.Day < _checkOutDate);
                        totalPrice = allSeledDates.Sum(_ => _.NormalPrice);
                        totalVipPrice = allSeledDates.Sum(_ => _.VipPrice);
                        defPrice = totalVipPrice;

                        useDateParam = true;
                    }
                }
                //log.AddLog("string.IsNullOrEmpty(checkInStr)");

                if (!useDateParam)
                {
                    var firstDayObj = calendar.DayItems.OrderBy(_=>_.VipPrice).First(d => d.SellState == 1);

                    checkIn = firstDayObj.Day;
                    //最低价格
                    totalPrice = firstDayObj.NormalPrice;
                    totalVipPrice = firstDayObj.VipPrice;

                    if (calendar.DayLimitMin > 0)
                    {
                        addNightCount = calendar.DayLimitMin;
                    }
                    checkOut = checkIn.AddDays(addNightCount);
                    defPrice = firstDayObj.SellPrice;

                    //如果是多天的套餐，则累加计算出多天总价
                    if (calendar.DayLimitMin > 1)
                    {
                        totalPrice = 0;
                        totalVipPrice = 0;

                        var _startDate = checkIn;
                        for (int i = 0; i < calendar.DayLimitMin; i++)
                        {
                            var _calendarItem = calendar.DayItems.Find(_ => _.Day == _startDate.AddDays(i));
                            if (_calendarItem != null)
                            {
                                totalPrice += _calendarItem.NormalPrice;
                                totalVipPrice += _calendarItem.VipPrice;
                            }
                        }
                    }
                }
                //log.AddLog("useDateParam");
            }

            //如果当前套餐不是新VIP专享，则检查当前套餐是否有关联的新VIP套餐
            var _relPackageEntity = new PackageInfoEntity();
            if (packageEntity.packageItem != null && !packageEntity.packageItem.ForVIPFirstBuy)
            {
                //不查了，目前不推新VIP概念 2018.04.26 haoy
                //_relPackageEntity = Price.GetFirstVIPPackageByPackageId(pid, checkIn.ToString("yyyy-MM-dd"), checkOut.ToString("yyyy-MM-dd"));
            }
            ViewBag.RelPackageEntity = _relPackageEntity;

            //获取产品店铺信息
            var productShopInfo = new HJD.HotelManagementCenter.Domain.RetailerShopEntity();
            if (requestCID > 0)
            {
                //查询指定CID的店铺信息
                productShopInfo = Shop.GetRetailerShopByCID(requestCID);
            }
            //log.AddLog("GetRetailerShopByCID");
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

            ViewBag.TotalPrice = totalPrice;
            ViewBag.TotalVipPrice = totalVipPrice;

            var subPriceTip = string.Format("已选{0}月{1}号入住{2}月{3}日离店，共{4}晚", checkIn.Month, checkIn.Day, checkOut.Month, checkOut.Day, addNightCount);
            ViewBag.SubPriceTip = subPriceTip;
            ViewBag.DefPrice = defPrice;
            ViewBag.CheckIn = checkIn;
            ViewBag.CheckOut = checkOut;
            ViewBag.Calendar = calendar.DayItems;
            ViewBag.DayLimitMin = calendar.DayLimitMin;
            ViewBag.DayLimitMax = calendar.DayLimitMax;
            ViewBag.NightCount = addNightCount;

            #region 分享配置

            var shareLink = "";
            if (requestCID > 0)
            {
                //分享链接生成
                shareLink = string.Format("http://www.zmjiudian.com/Hotel/Package/{0}?CID={1}", pid, requestCID);
            }
            else
            {
                //暂时只有在APP中登录分享才会加sid&cid等参数
                if (isApp)
                {
                    //分享链接生成
                    shareLink = string.Format("http://www.zmjiudian.com/Hotel/Package/{0}?CID={1}", pid, userid);
                }
                else
                {
                    shareLink = string.Format("http://www.zmjiudian.com/Hotel/Package/{0}?CID={1}", pid, userid);
                }
            }

            ViewBag.ShareLink = shareLink;

            #endregion

            #region 行为记录

            var value = string.Format("{{\"pid\":\"{0}\",\"userNo\":\"{1}\",\"PageName\":\"{2}\",\"HotelName\":\"{3}\",\"albumid\":\"{4}\"}}", pid, curUserID, packageEntity.packageItem.PackageName, packageEntity.packageItem.HotelName, albumid);
            RecordBehavior("PackageLoad", value);

            #endregion

            //log.AddLog("other");
            //log.Finish();
            return View();
        }

        [HttpGet]
        public ActionResult GetCommentCount(int hotelid = 0)
        {
            var backDic = new Dictionary<string, string>();

            //往期点评
            ReviewQueryParam queryParam = new ReviewQueryParam();
            queryParam.Hotel = hotelid;
            queryParam.Start = 0;
            queryParam.Count = 0;
            queryParam.RatingType = HJD.HotelServices.Contracts.RatingType.GoodGrade;
            var reviewObj = new Hotel().GetComments50(queryParam);
            if (reviewObj != null)
            {
                backDic["count"] = reviewObj.AllReviewCount.ToString();
            }
            else
            {
                backDic["count"] = "0";
            }

            return Json(backDic, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取指定日期的套餐价格
        /// </summary>
        /// <param name="hotelid"></param>
        /// <param name="pid"></param>
        /// <param name="checkIn"></param>
        /// <param name="checkOut"></param>
        /// <param name="userid"></param>
        /// <param name="dateSelectType"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetPackageCalendarPrice(int hotelid, int pid, string checkIn, string checkOut, int userid = 0, int dateSelectType = 1)
        {
            var backDic = new Dictionary<string, string>();

            var checkInDate = DateTime.Now.Date;
            var checkOutDate = checkInDate.AddDays(1);
            try
            {
                checkInDate = DateTime.Parse(checkIn);
                checkOutDate = DateTime.Parse(checkOut);
            }
            catch (Exception ex)
            {

            }

            var price = 0;
            var vipPrice = 0;
            var normalPrice = 0;
            var nightCount = 1;

            //首先获取当前套餐的所有日期下的价格数据
            var calendar = HJDAPI.APIProxy.Price.GetOneHotelPackageCalendar(hotelid, DateTime.Now.Date, pid, userid);
            if (calendar != null && calendar.DayItems != null && calendar.DayItems.Count > 0)
            {
                var selCalendar = calendar.DayItems.Where(c => c.Day >= checkInDate && c.Day < checkOutDate).ToList();
                price = selCalendar.Sum(c => c.SellPrice);
                vipPrice = selCalendar.Sum(c => c.VipPrice);
                normalPrice = selCalendar.Sum(c => c.NormalPrice);
                nightCount = selCalendar.Count;
            }

            backDic["price"] = price.ToString();
            backDic["vipPrice"] = vipPrice.ToString();
            backDic["normalPrice"] = normalPrice.ToString();
            backDic["days"] = nightCount.ToString();

            var _tip = string.Format("已选{0}月{1}号入住{2}月{3}日离店，共{4}晚", checkInDate.Month, checkInDate.Day, checkOutDate.Month, checkOutDate.Day, nightCount);
            switch (dateSelectType)
            {
                case 1: { break; }  //入住日期
                case 2: { _tip = string.Format("已选择{0}月{1}日出发", checkInDate.Month, checkInDate.Day); break; }   //出行日期
                case 3: { break; }   //入住开始日
                case 4: { break; }   //消费日期
                default: { break; }
            }
            backDic["tip"] = _tip;

            return Json(backDic, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 酒店套餐列表页
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public ActionResult Packages(string code = "")
        {
            if (HttpContext.Request.Url != null && !string.IsNullOrEmpty(HttpContext.Request.Url.AbsoluteUri))
            {
                var _rUrl = "";
                var _absUri = HttpContext.Request.Url.AbsoluteUri;
                if (_absUri.ToLower().Contains("www.shangjiudian.com"))
                {
                    _rUrl = _absUri.ToLower().Replace("www.shangjiudian.com", "www.zmjiudian.com").Replace("https://", "http://");
                    return Redirect(_rUrl);
                }
                else if (_absUri.ToLower().Contains("https://"))
                {
                    _rUrl = _absUri.ToLower().Replace("https://", "http://");
                    return Redirect(_rUrl);
                }
            }

            string agent = HttpContext.Request.UserAgent;
            string[] keywords = { "Android", "iPhone", "iPod", "iPad", "Windows Phone", "MQQBrowser" };
            var isInWeixin = agent.IndexOf("MicroMessenger") > 0;   //agent = "Mozilla/5.0 (iPhone; CPU iPhone OS 8_1_2 like Mac OS X) AppleWebKit/600.1.4 (KHTML, like Gecko) Mobile/12B440 MicroMessenger/6.1 NetType/WIFI";
            ViewBag.IsInWeixin = isInWeixin;

            var curUserID = UserState.UserID > 0 ? UserState.UserID : ClientHelper.GetBotaoUserIdFromCookie(HttpContext.Request.Cookies);
            ViewBag.UserId = curUserID;

            //获取请求参数中的CID
            var requestCID = GetCurCIDForRequest();
            ViewBag.RequestCID = requestCID;

            PackagesParam param = new PackagesParam(this);

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
                            weixinGoUrl = WeiXinHelper.GenSilenceAuthorUrlForHaoYi(HttpUtility.UrlEncode(string.Format("http://www.zmjiudian.com/hotel/{0}/packages?checkIn={1}&checkOut={2}&pid={3}&CID={4}", param.HotelId, param.CheckIn, param.CheckOut, param.pid, requestCID)));
                        }
                        else
                        {
                            weixinGoUrl = WeiXinHelper.GenSilenceAuthorUrlForHaoYi(HttpUtility.UrlEncode(string.Format("http://www.zmjiudian.com/hotel/{0}/packages?checkIn={1}&checkOut={2}&pid={3}", param.HotelId, param.CheckIn, param.CheckOut, param.pid)));
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
                        weixinGoUrl = WeiXinHelper.GenSilenceAuthorUrlForHaoYi(HttpUtility.UrlEncode(string.Format("http://www.zmjiudian.com/hotel/{0}/packages?checkIn={1}&checkOut={2}&pid={3}&CID={4}", param.HotelId, param.CheckIn, param.CheckOut, param.pid, requestCID)));
                    }
                    else
                    {
                        weixinGoUrl = WeiXinHelper.GenSilenceAuthorUrlForHaoYi(HttpUtility.UrlEncode(string.Format("http://www.zmjiudian.com/hotel/{0}/packages?checkIn={1}&checkOut={2}&pid={3}", param.HotelId, param.CheckIn, param.CheckOut, param.pid)));
                    }
                    return Redirect(weixinGoUrl);
                }
            }
            ViewBag.Unionid = unionid;

            #endregion

            //当前用户是否VIP
            var isVip = false; if (curUserID > 0) isVip = account.IsVIPCustomer(new AccountInfoItem { Uid = curUserID.ToString() });
            ViewBag.IsVip = isVip;

            ViewBag.isMobile = Utils.IsMobile();

            ViewBag.hotel = HJDAPI.APIProxy.Hotel.Get3(param.HotelId, param.CheckIn, param.CheckOut, Utils.ClientType(), 0);

            //get package 1
            HotelPrice3 priceEntity = Price.Get7(param.HotelId, param.CheckIn, param.CheckOut, Utils.ClientType(), HotelServiceEnums.PackageType.HotelPackage, curUserID);
            ViewBag.PriceEntity = priceEntity;

            //get package 2 (from ota)
            HotelPrice3 priceEntity2 = Price.Get7(param.HotelId, param.CheckIn, param.CheckOut, Utils.ClientType(), HotelServiceEnums.PackageType.CtripPackageByApi, curUserID);
            ViewBag.PriceEntity2 = priceEntity2;

            param.CheckInDate = priceEntity.CheckIn;
            param.CheckOutDate = priceEntity.CheckOut;
            if (priceEntity.PackageGroups.Count > 0)
            {
                ViewBag.calendar = HJDAPI.APIProxy.Price.GetHotelPackageCalendar30(param.HotelId, DateTime.Now.Date);
            }
            ViewBag.firstpid = param.pid;

            //记录酒店套餐页加载
            var value = string.Format("{{\"hotelid\":\"{0}\",\"checkin\":\"{1}\",\"checkout\":\"{2}\",\"userType\":\"{3}\",\"userNo\":\"{4}\"}}", param.HotelId, param.CheckIn, param.CheckOut, curUserID > 0 && UserState.UserID == 0 ? "botao" : "zmjd", curUserID);
            RecordBehavior("PackageLoad", value);
            ViewBag.PageTag = "Package";

            var hostName = HttpContext.Request.Url.Host;//请求的服务器主机 //UserHostName;//客户的主机名
            var portNo = HttpContext.Request.Url.Port;
            var hotelID = param.HotelId;//酒店ID

            //预订URL
            bool isProduct = bool.Parse(System.Configuration.ConfigurationManager.AppSettings["APIProxy_IsProductEvn"]);
            ViewBag.HttpsBookUrlTemplate = (isProduct ? "https://" : "http://") + hostName + (portNo == 0 ? "" : ":" + portNo.ToString()) + "/hotel/" + hotelID + "/book?package={0}&checkIn={1}&checkOut={2}";

            ViewBag.param = param;
            return View();
        }

        // 套餐日历
        [HttpGet]
        public ActionResult PackageCalendar()
        {
            PackageCalendarParam param = new PackageCalendarParam(this);
            var onePackageEntity = new Hotel().GetOnePackageEntity(param.PackageId);
            var data = HJDAPI.APIProxy.Price.GetOneHotelPackageCalendar30(param.HotelId, DateTime.Today, param.PackageId);
            return Json(Utils.MakeCalendarOptions(data, onePackageEntity.DayLimitMin, onePackageEntity.DayLimitMax), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 套餐预订前验证
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="pid"></param>
        /// <returns></returns>
        public ActionResult CheckSubmitOrderBefore(long userid = 0, int pid = 0)
        {
            var _result = new Order().CheckSubmitOrderBefore(pid, userid);

            return Json(_result, JsonRequestBehavior.AllowGet);
        }

        // 酒店预订
        public ActionResult Book(long userid = 0)
        {
            //Request.UrlReferrer
            var thisUrl = Request.Url.ToString();
            if (thisUrl.Contains("shangjiudian"))
            {
                thisUrl = thisUrl.Replace("shangjiudian", "zmjiudian");
                //thisUrl = "http://www.shangjiudian.com/hotel/188660/book?package=1361&checkIn=2016-06-10&checkOut=2016-06-11";
                return Redirect(thisUrl);
            }

            var isApp = IsApp();
            ViewBag.IsApp = isApp;

            string agent = HttpContext.Request.UserAgent;
            string[] keywords = { "Android", "iPhone", "iPod", "iPad", "Windows Phone", "MQQBrowser" };
            // agent = "Mozilla/5.0 (iPhone; CPU iPhone OS 8_1_2 like Mac OS X) AppleWebKit/600.1.4 (KHTML, like Gecko) Mobile/12B440 MicroMessenger/6.1 NetType/WIFI";
            ViewBag.IsInWeixin = agent.IndexOf("MicroMessenger") > 0;

            BookParam param = new BookParam(this);
            ViewBag.param = param;
            ViewBag.hotel = HJDAPI.APIProxy.Hotel.Get3(param.HotelId, param.CheckIn, param.CheckOut, Utils.ClientType(), 0);

            //userid
            var curUserID = UserState.UserID > 0 ? UserState.UserID : ClientHelper.GetBotaoUserIdFromCookie(HttpContext.Request.Cookies);
            if (userid <= 0 && curUserID > 0) { userid = curUserID; }

            HotelPrice2 price = HJDAPI.APIProxy.Price.GetV42(param.HotelId, param.CheckIn, param.CheckOut, Utils.ClientType(), userid);
            ViewBag.price = price;
            param.CheckInDate = price.CheckIn;
            param.CheckOutDate = price.CheckOut;

            //PackageInfoEntity packageEntity = null;
            PackageInfoEntity packageEntity = Price.GetOnePackageInfoEntity(param.PackageId, param.HotelId, DateTime.Parse(param.CheckIn), DateTime.Parse(param.CheckOut), userid);
            if (packageEntity.packageBase == null)
            {
                foreach (PackageInfoEntity entity in price.Packages)
                {
                    if (entity.packageBase.ID == param.PackageId)
                    {
                        packageEntity = entity;
                        packageEntity.InChina = true;
                        break;
                    }
                }
            }
            ViewBag.package = packageEntity;

            if (ViewBag.package == null)
            {
                throw new Exception("套餐ID无效");
            }

            ViewBag.isbotaomeb = false;

            if (userid != 0)
            {
                MemberProfileInfo info = account.GetCurrentUserInfo(userid);
                ViewBag.phoneNum = info.MobileAccount;
            }
            else
            {
                var botaoCode = HJDAPI.Common.Helpers.Enums.ThirdPartyMerchantCode.bohuijinrong.ToString();
                var botaophone = ClientHelper.GetBotaoPhoneNumFromCookie(HttpContext.Request.Cookies);

                if (!string.IsNullOrWhiteSpace(botaophone))
                {
                    ViewBag.phoneNum = botaophone;

                    ViewBag.isbotaomeb = true;
                }
            }

            //获取当前用户的现金券信息
            int userCouponAmount = 0; if (userid > 0) userCouponAmount = new Coupon().GetUserCanUseCashCouponAmountByUid(userid) / 100;
            ViewBag.UserCouponAmount = userCouponAmount;

            //检查当前套餐当前用户的可用券情况
            var canUseCashCoupon = 0;
            if (packageEntity != null && packageEntity.CanUseCashCoupon > 0 && userCouponAmount > 0)
            {
                canUseCashCoupon = userCouponAmount < packageEntity.CanUseCashCoupon ? userCouponAmount : packageEntity.CanUseCashCoupon;
            }
            ViewBag.CanUseCashCoupon = canUseCashCoupon;

            //检查当前用户的住基金
            var userFundInfo = new Fund().GetUserFundInfo(userid);
            ViewBag.UserFundInfo = userFundInfo;

            ViewBag.UserId = userid;

            //如果当前抢购VIP专享，则判断当前用户是否为VIP
            var isVip = account.IsVIPCustomer(new AccountInfoItem { Uid = userid.ToString() });
            ViewBag.IsVip = isVip;

            //var sessionId = Session.SessionID;
            //var validationCodeResult = new Access().GenValidationCodeBase64Str(sessionId);
            //ViewBag.imgcode = string.Format("data:image/gif;base64,{0}", validationCodeResult.base64Url);

            //记录加载
            var value = string.Format("{{\"hotelid\":\"{0}\",\"package\":\"{1}\",\"checkin\":\"{2}\",\"checkout\":\"{3}\",\"userType\":\"{4}\",\"userNo\":\"{5}\"}}", param.HotelId, param.PackageId, param.CheckIn, param.CheckOut, curUserID > 0 && UserState.UserID == 0 ? "botao" : "zmjd", userid);
            RecordBehavior("BookLoad", value);
            ViewBag.PageTag = "Book";

            return View();
        }

        /// <summary>
        /// 提交酒店套餐订单
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="chooseCash"></param>
        /// <param name="cashCouponIdx"></param>
        /// <param name="cashCouponType"></param>
        /// <param name="cashCouponAmount"></param>
        /// <param name="useFundAmount"></param>
        /// <returns></returns>
        public ActionResult Submit(long userid = 0, int chooseCash = 0, int cashCouponIdx = 0, int cashCouponType = 0, decimal cashCouponAmount = 0, decimal useFundAmount = 0)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();

            try
            {
                SubmitParam param = new SubmitParam(this);

                HJD.HotelPrice.Contract.DataContract.Order.OrderPackageEntity package = new HJD.HotelPrice.Contract.DataContract.Order.OrderPackageEntity();
                package.CheckIn = param.CheckInDate;
                package.NightCount = (int)(param.CheckOutDate - param.CheckInDate).TotalDays;
                package.Contact = param.Contact;
                package.ContactPhone = param.ContactPhone;
                package.PID = param.PackageId;
                package.RoomCount = param.RoomCount;
                package.Note = param.Note;

                PackageInfoEntity packageInfo = null;
                var curUserID = UserState.UserID > 0 ? UserState.UserID : ClientHelper.GetBotaoUserIdFromCookie(HttpContext.Request.Cookies);
                if (userid <= 0 && curUserID > 0) { userid = curUserID; }

                HotelPrice2 price = HJDAPI.APIProxy.Price.GetV42(param.HotelId, param.CheckIn, param.CheckOut, Utils.ClientType(), userid);
                foreach (PackageInfoEntity entity in price.Packages)
                {
                    if (entity.packageBase.ID == param.PackageId)
                    {
                        packageInfo = entity;
                        break;
                    }
                }

                //计算现金券使用【老的现金券】
                if (chooseCash == 1)
                {
                    var cashAmount = packageInfo.CanUseCashCoupon * param.RoomCount;
                    int userCouponAmount = 0; if (userid > 0) userCouponAmount = new Coupon().GetUserCanUseCashCouponAmountByUid(userid) / 100;
                    var userUseCashCouponAmount = cashAmount <= userCouponAmount ? cashAmount : userCouponAmount;
                    package.UserUseCashCouponAmount = userUseCashCouponAmount;
                }
                else
                {
                    package.UserUseCashCouponAmount = 0;
                }

                //总优惠金额
                decimal _discountTotalAmount = 0;

                //最新现金券使用
                if (cashCouponIdx > 0 && cashCouponAmount > 0)
                {
                    package.UseCashCouponInfo = new HJD.HotelPrice.Contract.DataContract.Order.UseCashCouponItem 
                    {
                        CashCouponID = cashCouponIdx,
                        CashCouponType = cashCouponType,
                        UseCashAmount = cashCouponAmount
                    };

                    _discountTotalAmount += cashCouponAmount;

                }

                //住基金使用
                if (useFundAmount > 0)
                {
                    package.UserUseHousingFundAmount = (int)useFundAmount;
                    _discountTotalAmount += useFundAmount;
                }

                //支付类型
                package.PayType = packageInfo.PayType;

                if (packageInfo == null)
                {
                    return Content("套餐已下线，请选择其他");
                }
                if (packageInfo.packageBase.PackageCount == 0)
                {
                    return Content("套餐已售韾，请选择其他");
                }

                OrderMainEntity main = new OrderMainEntity();
                main.Amount = packageInfo.Price * package.RoomCount;
                main.Type = (OrderType)packageInfo.PackageType;
                main.HotelID = param.HotelId;
                main.TerminalID = Utils.GetTerminalId(Request.UserAgent);
                main.ChannelID = Utils.GetChannelId(Request.UserAgent, Session["ChannelID"]);
                main.UserID = userid;

                if (packageInfo.packageBase.PackageCount < package.RoomCount)
                {
                    return Content("套餐目前仅剩" + packageInfo.packageBase.PackageCount + "套, 请重新选择");
                }
                int canCustomBuyMin = packageInfo.CustomBuyMin;
                int canCustomBuyMax = packageInfo.CustomBuyMax;
                if (canCustomBuyMax != 0 && packageInfo.packageBase.PackageCount > canCustomBuyMax)
                {
                    if ((package.RoomCount <= 0 ? 1 : package.RoomCount) > canCustomBuyMax)
                    {
                        return Content("每位用户限购" + canCustomBuyMax + "套, 请重新选择");
                    }
                }
                OrderEntity orderEntity = new OrderEntity();
                orderEntity.main = main;
                orderEntity.package = package;

                //出行人
                var _travelPersons = new List<int>();
                if (packageInfo.MinHotelPeople > 0 && !string.IsNullOrEmpty(param.TravelPersons))
                {
                    try
                    {
                        _travelPersons = param.TravelPersons.Split(',').ToList().Select(_id => Convert.ToInt32(_id)).ToList();
                    }
                    catch (Exception ex)
                    {
                        return Content("出行人读取错误，请重试");
                    }
                }
                orderEntity.TravelId = _travelPersons;

                //机酒邮轮套餐的出行人信息
                var orderContactList = new List<OrderContactsEntity>();
                if (!string.IsNullOrEmpty(param.AirPersons))
                {
                    try
                    {
                        var _nameList = param.AirPersons.Split(',').ToList();
                        foreach (var _personName in _nameList)
                        {
                            orderContactList.Add(new OrderContactsEntity { CName = _personName });
                        }
                    }
                    catch (Exception ex)
                    {
                        return Content("机酒邮轮出行人读取错误，请重试");
                    }
                }
                orderEntity.OrderContactList = orderContactList;

                OrderSubmitResult result = HJDAPI.APIProxy.Order.SubmitOrderV42(orderEntity);
                if (result.ErrorCode == 0)
                {
                    var botaoCode = HJDAPI.Common.Helpers.Enums.ThirdPartyMerchantCode.bohuijinrong.ToString();
                    var botaophone = ClientHelper.GetBotaoPhoneNumFromCookie(HttpContext.Request.Cookies);

                    if (!string.IsNullOrWhiteSpace(botaophone))
                    {
                        var phoneNum = botaophone;
                        var brief = packageInfo.packageBase.Brief;

                        var goodsInfo = string.Format("{3}({0}间{1}晚 {2})", package.RoomCount, package.NightCount, string.IsNullOrWhiteSpace(brief) ? packageInfo.Room.Description : brief, price.Name);
                        var amount = (int)main.Amount * 100;//元转分
                        var orderID = result.OrderID;

                        var payWebSiteUrl = (string)System.Configuration.ConfigurationManager.AppSettings["PayWebSiteUrl"];
                        var completeUrl = string.Format("{0}Pay/BoTaoPayComplete/{1}/{2}", payWebSiteUrl, "hotel", orderID);

                        var sign = EncryptMethod.GenSignature4Pay(botaoCode, phoneNum, amount, orderID, completeUrl);
                        var jumpUrl = string.Format("{7}pay/botao?mobileId={0}&merName={1}&goodsInf={2}&amount={3}&orderId={4}&retUrl={5}&sign={6}", phoneNum, botaoCode, goodsInfo, amount, orderID, completeUrl, sign, payWebSiteUrl);
                        dict["url"] = jumpUrl;
                    }
                    else if (packageInfo.PayType == 1)//现付成功提交订单后，提示提交成功并显示订单详情
                    {
                        dict["url"] = Url.Action("Detail", "Order", new { order = result.OrderID, showtit = 1, key = EncryptMethod.GenMD5Key(result.OrderID.ToString()) });
                    }
                    else
                    {
                        //唯一指定了支付方式时，比如待支付金额大于0，才会跳转到支付渠道页面
                        if (!param.isChannelNull && main.Amount > _discountTotalAmount)
                        {
                            dict["url"] = Url.Action("Direct", "Payment", new { order = result.OrderID, channel = param.Channel });
                        }
                        else
                        {
                            var payChannelList = packageInfo.PayChannels ?? new List<string>();
                            var payChannelsStr = "";
                            foreach (var pitem in payChannelList)
                            {
                                if (!string.IsNullOrEmpty(payChannelsStr)) payChannelsStr += ",";
                                payChannelsStr += pitem;
                            }

                            if (IsLatestVerApp())
                            {
                                var completeUrl = HttpUtility.UrlEncode(string.Format("whotelapp://www.zmjiudian.com/personal/order/{0}", result.OrderID));
                                dict["url"] = string.Format("whotelapp://orderPay?orderid={0}&finishurl={1}&paytype={2}", result.OrderID, completeUrl, "all");
                            }
                            else
                            {
                                dict["url"] = Url.Action("Pay", "Order", new { order = result.OrderID, payType = packageInfo.PayType, payChannels = payChannelsStr });
                            }
                        }
                    }
                }
                else
                {
                    dict["code"] = Convert.ToInt32(result.ErrorCode).ToString();
                    dict["error"] = result.ErrorMessage;
                }
                AccountController.SetUserId(result.UserID);

                //提交订单行为记录
                try
                {
                    var value = string.Format("{{\"orderid\":\"{0}\",\"hotelid\":\"{1}\",\"packageid\":\"{2}\",\"checkin\":\"{3}\",\"nightcount\":\"{4}\"}}",
                    result.OrderID, main.HotelID, package.PID, package.CheckIn, package.NightCount);
                    RecordBehavior("Submit", value);
                }
                catch (Exception ex)
                {
                    RecordBehavior("Submit", ex.Message);
                }

            }
            catch (Exception err)
            {
                LogHelper.WriteLog("Submit Error:" + err.Message + err.StackTrace);
                dict["message"] = err.Message;
                return Json(dict);
            }

            return Json(dict);
        }

        public static string GetPackageReBookUrl(UrlHelper Url, PackageOrderInfo20 order)
        {
            return Url.Action("Book", "Hotel", new
            {
                hotel = order.HotelID,
                package = order.PID,
                checkIn = Utils.FormatDate(order.CheckIn),
                checkOut = Utils.FormatDate(order.CheckIn.AddDays(order.NightCount)),
                roomCount = order.RoomCount,
                contact = order.Contact,
                contactPhone = order.ContactPhone,
                note = order.Note,
            });
        }

        public static HotelItem3 GetHotel(int hotelId)
        {
            return HJDAPI.APIProxy.Hotel.Get3(hotelId, "0", "0", Utils.ClientType(), 0);
        }

        public ActionResult Strategy(int did)
        {
            var isApp = IsApp();
            ViewBag.IsApp = isApp;

            ViewBag.AppType = AppType();

            var districtList = Dest.GetStrategyDistrictZoneList(did);

            ViewBag.DistrictList = districtList;
            ViewBag.DistrictId = did;

            return View();
        }

        /// <summary>
        /// 酒店专辑列表
        /// </summary>
        /// <returns></returns>
        public ActionResult CollectionHotel(int cid = 0, int userid = 0)
        {
            var isApp = IsApp();
            ViewBag.IsApp = isApp;
            ViewBag.Cid = cid;
            ViewBag.UserId = userid;

            //Weixin
            bool isInWeixin = Request.UserAgent.IndexOf("MicroMessenger") > 0 ? true : false;
            ViewBag.isInWeixin = isInWeixin;

            PackageAlbumDetail albumDetail = Hotel.GetPackageAlbumDetail(cid);

            //分享信息设置
            albumDetail.shareModel.shareLink = string.Format("http://www.zmjiudian.com/hotel/collection/{0}?userid={1}", cid, userid);
            albumDetail.shareModel.photoUrl = albumDetail.shareModel.photoUrl.Replace("_jupiter", "_290x290s").Replace("_theme", "_290x290s").Replace("_appdetail1s", "_290x290s");

            ViewBag.AlbumDetail = albumDetail;

            return View();
        }

        /// <summary>
        /// 套餐专辑列表
        /// </summary>
        /// <param name="cid"></param>
        /// <param name="userid"></param>
        /// <param name="showType">用于控制页面展示 1是新版展示方式 </param>
        /// <returns></returns>
        public ActionResult CollectionPackage(int cid = 0, int userid = 0, int showType = 0, int startDistrictId = 0,bool isShowStartDistrict = true)
        {
            var isApp = IsApp();
            ViewBag.IsApp = isApp;
            ViewBag.Cid = cid;
            ViewBag.UserId = userid;

            //当前系统环境（ios | android）
            var appType = AppType();
            ViewBag.AppType = appType;

            //Weixin
            bool isInWeixin = Request.UserAgent.IndexOf("MicroMessenger") > 0 ? true : false;
            ViewBag.isInWeixin = isInWeixin;

            //PackageAlbumDetail albumDetail = Hotel.GetPackageAlbumDetail(cid);
            PackageAlbumDetail albumDetail = new PackageAlbumDetail();

            string districtName = "全部城市";
            if (showType == 1)
            {
                ScreenConditionsEntity cityInfoList = Hotel.GetAlbumFilter(cid);
                albumDetail = Hotel.GetPackageGroupAlbumDetail(cid, startDistrictId);
                if (startDistrictId > 0 && cityInfoList != null && cityInfoList.StartCityList != null && cityInfoList.StartCityList.Count > 0)
                {
                    int count = cityInfoList.StartCityList.Where(_ => _.ID == startDistrictId).Count();
                    if (count > 0)
                    {
                        districtName = cityInfoList.StartCityList.Where(_ => _.ID == startDistrictId).First().Name;
                    }
                }

                ViewBag.CityInfo = cityInfoList;
            }
            else
            {
                albumDetail = Hotel.GetPackageAlbumDetail(cid);
            }

            //if (albumDetail != null && albumDetail.packageList != null && albumDetail.packageList.Count > 0)
            //{
            //    albumDetail.packageList = albumDetail.packageList.OrderBy(p => p.);
            //}
            //无锡,苏州,三亚,桂林,厦门,莫干山,安吉,杭州,常州,都江堰,峨眉山,成都,长白山

            //分享信息设置
            //albumDetail.shareModel.shareLink = string.Format("http://www.zmjiudian.com/package/collection/{0}?userid={1}&showType={2}", cid, userid, showType);
            albumDetail.shareModel.shareLink = System.Web.HttpUtility.UrlEncode(string.Format("http://www.zmjiudian.com/package/collection/{0}?userid={1}&showType={2}", cid, userid, showType), System.Text.Encoding.GetEncoding("GB2312"));
            albumDetail.shareModel.photoUrl = albumDetail.shareModel.photoUrl.Replace("_jupiter", "_290x290s").Replace("_theme", "_290x290s").Replace("_appdetail1s", "_290x290s");
            albumDetail.shareModel.title = albumDetail.albumEntity.Name;

            ViewBag.AlbumDetail = albumDetail;

            ViewBag.ShowType = showType;

            ViewBag.StartDistrictId = startDistrictId;

            ViewBag.StartDistrictName = districtName;

            ViewBag.IsShowStartDistrict = isShowStartDistrict;

            return View();
        }

        /// <summary>
        /// 节假日专用套餐专辑页
        /// </summary>
        /// <param name="cid"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public ActionResult CollectionPackageForHoliday(int cid = 0, int userid = 0)
        {
            var isApp = IsApp();
            ViewBag.IsApp = isApp;
            ViewBag.Cid = cid;
            ViewBag.UserId = userid;

            //Weixin
            bool isInWeixin = Request.UserAgent.IndexOf("MicroMessenger") > 0 ? true : false;
            ViewBag.isInWeixin = isInWeixin;

            //目的地排序
            var cityOrderListStr = System.Configuration.ConfigurationManager.AppSettings["AlbumCityOrderStr"].ToString();
            var proOrderListStr = System.Configuration.ConfigurationManager.AppSettings["AlbumProvinceOrderStr"].ToString();
            List<string> cityOrderList = cityOrderListStr.Split(',').ToList(); //new List<string> { "三亚", "厦门", "桂林" };
            List<string> disOrderList = proOrderListStr.Split(',').ToList();
            //new List<string> 
            //{ 
            //    "海外",
            //    "江苏",
            //    "浙江",
            //    "四川",
            //    "广东",
            //    "北京",
            //    "安徽"
            //};


            //分组集合
            Dictionary<string, List<RecommendHotelItem>> d1 = new Dictionary<string, List<RecommendHotelItem>>();

            //获取专辑套餐数据
            PackageAlbumDetail albumDetail = Hotel.GetPackageAlbumDetail(cid);
            if (albumDetail != null && albumDetail.packageList != null && albumDetail.packageList.Count > 0)
            {
                //城市list
                foreach (var cityName in cityOrderList)
                {
                    d1[cityName] = albumDetail.packageList.Where(p => p.DistrictName == cityName).OrderBy(c => c.HotelName).ToList();
                }

                //其他list
                var chinaList = albumDetail.packageList.Where(p => !cityOrderList.Contains(p.DistrictName) && p.InChina).OrderBy(c => c.HotelName).ToList();
                var inlList = albumDetail.packageList.Where(p => !p.InChina).OrderBy(c => c.HotelName).ToList();

                //将套餐列表按照其酒店的目的地分组
                var g1 = chinaList.GroupBy(_ => _.ProvinceName).OrderBy(_ => _.Key);
                foreach (var disName in disOrderList)
                {
                    try
                    {
                        if (disName == "海外")
                        {
                            d1[disName] = inlList;
                        }
                        else
                        {
                            var _listObj = g1.First(g => g.Key == disName);
                            if (_listObj != null)
                            {
                                var _list = _listObj.ToList();
                                d1[disName] = _list.OrderBy(c => c.HotelName).ToList();
                            }
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
            ViewBag.D1 = d1;

            //分享信息设置
            albumDetail.shareModel.shareLink = string.Format("http://www.zmjiudian.com/package/collection/{0}?userid={1}", cid, userid);
            albumDetail.shareModel.photoUrl = albumDetail.shareModel.photoUrl.Replace("_jupiter", "_290x290s").Replace("_theme", "_290x290s").Replace("_appdetail1s", "_290x290s");

            ViewBag.AlbumDetail = albumDetail;

            return View();
        }


        #region 出行人相关

        /// <summary>
        /// 获取指定用户的出行人信息
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public ActionResult GetTravelPersonByUserId(long userid)
        {
            var md5Key = WHotelSite.Common.Config.MD5Key;
            var sourceId = 4;
            var requestType = "GetTravelPersonByUserId";
            var timeStamp = HJDAPI.Common.Security.Signature.GenTimeStamp();
            var sign = HJDAPI.Common.Security.Signature.GenSignature(timeStamp, sourceId, md5Key, requestType);

            var _p = new HJDAPI.Models.RequestParams.GetTravelPersonByUserIdParams() { TimeStamp = timeStamp, SourceID = sourceId, RequestType = requestType, Sign = sign, userID = userid };
            var result = new Hotel().GetTravelPersonByUserId(_p);

            //对出行人名称和证件号码解密
            if (result != null && result.TravelPersonList != null && result.TravelPersonList.Count > 0)
            {
                foreach (var _per in result.TravelPersonList)
                {
                    _per.TravelPersonName = HJDAPI.Common.Security.DES.Decrypt(_per.TravelPersonName);
                    _per.IDNumber = HJDAPI.Common.Security.DES.Decrypt(_per.IDNumber);
                }
            }

            return Json(result.TravelPersonList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SaveTravelPerson(int saveType, int id, int userid, string travelPersonName, int idType, string idnumber, string birthday)
        {
            var md5Key = WHotelSite.Common.Config.MD5Key;
            var sourceId = 4;
            var requestType = "SaveTravelPerson";
            var timeStamp = HJDAPI.Common.Security.Signature.GenTimeStamp();
            var sign = HJDAPI.Common.Security.Signature.GenSignature(timeStamp, sourceId, md5Key, requestType);

            var _p = new GetTravelPersonParams { TimeStamp = timeStamp, SourceID = sourceId, RequestType = requestType, Sign = sign };
            var result = new GetTravelPersonResponseModel { CardTypeName = "", Message = "", Success = false };

            //检查生日
            if (string.IsNullOrEmpty(birthday) || !birthday.Contains("-"))
            {
                birthday = "1990-01-01";
            }

            switch (saveType)
            {
                //add
                case 0:
                    {
                        var _addEntity = new TravelPersonEntity
                        {
                            ID = id,
                            UserID = userid,
                            TravelPersonName = HJDAPI.Common.Security.DES.Encrypt(travelPersonName),
                            IDType = idType,
                            IDNumber = HJDAPI.Common.Security.DES.Encrypt(idnumber),
                            Birthday = DateTime.Parse(birthday),
                            State = 1
                        };
                        _p.travelPerson = _addEntity;

                        result = new Hotel().AddTravelPerson(_p);
                        break;
                    }
                //edit
                case 1:
                    {
                        var _editEntity = new TravelPersonEntity
                        {
                            ID = id,
                            UserID = userid,
                            TravelPersonName = HJDAPI.Common.Security.DES.Encrypt(travelPersonName),
                            IDType = idType,
                            IDNumber = HJDAPI.Common.Security.DES.Encrypt(idnumber),
                            Birthday = DateTime.Parse(birthday),
                            State = 1
                        };
                        _p.travelPerson = _editEntity;

                        result = new Hotel().UpdateTravelPerson(_p);
                        break;
                    }
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}