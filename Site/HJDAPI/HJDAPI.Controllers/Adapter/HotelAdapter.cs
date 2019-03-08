using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using HJDAPI.Common.Helpers;
using HJDAPI.Models;
using HJD.DestServices.Contract;
using HJD.Framework.WCF;
using HJD.HotelServices.Contracts;
using HJD.HotelServices;
using HJD.Search.Contracts;
using HJD.Search.Entity;
using System.Web;
using System.Net;
using Newtonsoft.Json;
using System.Diagnostics;
using HJD.Framework.Interface;
using HJD.HotelPrice.Contract;
using System.Xml;
using System.IO;
using System.Data;
using HJDAPI.Controllers.Common;
using HJDAPI.Controllers.Adapter;
using HJD.HotelManagementCenter.IServices;
using HJD.CommentService.Contract;
using HJD.HotelServices.Contracts.Comments;
using HJD.HotelPrice.Contract.DataContract;
using HJD.AccessService.Contract.Model;
using HJD.AccessService.Contract;
using Microsoft.Practices.EnterpriseLibrary.Common.Utility;
using System.Text.RegularExpressions;
using HJDAPI.Models.ResponseModel;
using PersonalServices.Contract;

namespace HJDAPI.Controllers
{
    public class HotelAdapter
    {
        public static HJD.CommentService.Contract.ICommentService commentService = ServiceProxyFactory.Create<HJD.CommentService.Contract.ICommentService>("ICommentService");
        public static readonly ICacheProvider LocalCache = CacheManagerFactory.Create("DynamicCacheForType2");
        public static readonly ICacheProvider LocalCache30Min = CacheManagerFactory.Create("DynamicCacheForADV");
        public static readonly ICacheProvider LocalCache10Min = CacheManagerFactory.Create("DynamicCacheFor10Min");
        public static readonly ICacheProvider LocalCache5Min = CacheManagerFactory.Create("DynamicCacheFor5Min");

        //static readonly IMemcacheProvider memcached10min = MemcacheManagerFactory.Create("memcached10min");

        public static HJD.HotelServices.Contracts.IHotelService HotelService = ServiceProxyFactory.Create<HJD.HotelServices.Contracts.IHotelService>("BasicHttpBinding_IHotelService");
        public static ICommService commService = ServiceProxyFactory.Create<ICommService>("ICommService");
        public static IHotelPriceService PriceService = ServiceProxyFactory.Create<IHotelPriceService>("BasicHttpBinding_IHotelPriceService");
        public static IDestService destService = ServiceProxyFactory.Create<IDestService>("BasicHttpBinding_IDestService");
        public static ISearchTipAPI SearchTipApiService = ServiceProxyFactory.Create<ISearchTipAPI>("wsHttpBinding_ISearchTipAPI");
        public static HJD.HotelManagementCenter.IServices.IHotelService HMC_HotelService = ServiceProxyFactory.Create<HJD.HotelManagementCenter.IServices.IHotelService>("HMC_IHotelService");
        public static HJD.HotelManagementCenter.IServices.IPackageService HMC_PackageService = ServiceProxyFactory.Create<HJD.HotelManagementCenter.IServices.IPackageService>("HMC_IPackageService");
        public static IAccessService AccessService = ServiceProxyFactory.Create<IAccessService>("IAccessService");

        private const int CtripID = 2; //携程OTA的ID
        private const int BookingID = 1; //Booking OTA的ID
        private const int needTagLength = 5; //列表中需要返回的tagNameList的长度
        private const int MaxHotelPrice = 1000000;//暂定的最大酒店价格

        private const int defaultDistance = 300 * 1000;//缺省周边的范围为300公里
        private const string keyTemplate = "Hotel:{0};Interest:{1};District:{2};GeoStype:{3};Lat:{4};Lon:{5}";//缓存被挤掉的hotelID
        const string defaultPicSUrl = "115eSkS0O6";//App及酒店列表 不存在图片时的默认图片

        public long UserId;
        public string Uid;

        public static HotelItem GetOneHotelInfo(int hotelid)
        {
            return HotelService.GetHotel(hotelid);
        }

        public static List<ChannelInfoEntity> GetAllChannelInfoList()
        {
            return HotelService.GetAllChannelInfoList();
        }

        public static int AddTravelPerson(TravelPersonEntity travelperson)
        {
            return HotelService.AddTravelPerson(travelperson);
        }

        public static int UpdateTravelPerson(TravelPersonEntity travelperson)
        {
            return HotelService.UpdateTravelPerson(travelperson);
        }

        public static TravelPersonEntity GetTravelPersonById(int Id)
        {
            return HotelService.GetTravelPersonById(Id);
        }
        public static List<TravelPersonEntity> GetTravelPersonByUserId(long userId)
        {
            List<TravelPersonEntity> travelPersonEncrypt = new List<TravelPersonEntity>();
            List<TravelPersonEntity> t = HotelService.GetTravelPersonByUserId(userId);
            if (t.Count > 0)
            {
                foreach (TravelPersonEntity tp in t)
                {
                    tp.IDNumber = HJDAPI.Common.Security.DES.Encrypt(tp.IDNumber);
                    tp.TravelPersonName = HJDAPI.Common.Security.DES.Encrypt(tp.TravelPersonName);
                    travelPersonEncrypt.Add(tp);
                }
            }
            return travelPersonEncrypt;
            //return HotelService.GetTravelPersonByUserId(userId);
        }

        public static List<TravelPersonEntity> GetTravelPersonByUserIdUnEncryption(long userId)
        {
            List<TravelPersonEntity> travelPersonEncrypt = HotelService.GetTravelPersonByUserId(userId);
            return travelPersonEncrypt;
            //return HotelService.GetTravelPersonByUserId(userId);
        }

        public static List<TravelPersonEntity> GetTravelPersonByIDS(string IDS)
        {
            List<TravelPersonEntity> travelPersonList = HotelService.GetTravelPersonByIds(IDS);
            return travelPersonList;
        }

        public static bool DeleteTravelPerson(int id)
        {
            return HotelService.DeleteTravelPerson(id);
        }

        static Dictionary<int, string> dicCard = new Dictionary<int, string>();

        public static List<CardEntity> GetCardType()
        {
            if (dicCard.Count == 0)
            {
                dicCard.Add(1, ((HJDAPI.Common.Helpers.Enums.enumCardType)(1)).ToString());
                dicCard.Add(2, ((HJDAPI.Common.Helpers.Enums.enumCardType)(2)).ToString());
                dicCard.Add(3, ((HJDAPI.Common.Helpers.Enums.enumCardType)(3)).ToString());
                dicCard.Add(4, ((HJDAPI.Common.Helpers.Enums.enumCardType)(4)).ToString());
                dicCard.Add(5, ((HJDAPI.Common.Helpers.Enums.enumCardType)(5)).ToString());
                dicCard.Add(10, ((HJDAPI.Common.Helpers.Enums.enumCardType)(10)).ToString());
            }
            List<CardEntity> cartype = new List<CardEntity>();
            foreach (var c in dicCard)
            {
                CardEntity cardentity = new CardEntity();
                cardentity.cardTypeID = c.Key.ToString();
                cardentity.cardTypeDes = c.Value.ToString();
                cartype.Add(cardentity);
            }

            return cartype;
        }
        //public static Dictionary<int, string> GetCardType()
        //{
        //    if (dicCard.Count == 0)
        //    {
        //        dicCard.Add(1, ((HJDAPI.Common.Helpers.Enums.enumCardType)(1)).ToString());
        //        dicCard.Add(2, ((HJDAPI.Common.Helpers.Enums.enumCardType)(2)).ToString());
        //        dicCard.Add(3, ((HJDAPI.Common.Helpers.Enums.enumCardType)(3)).ToString());
        //        dicCard.Add(4, ((HJDAPI.Common.Helpers.Enums.enumCardType)(4)).ToString());
        //        dicCard.Add(10, ((HJDAPI.Common.Helpers.Enums.enumCardType)(10)).ToString());
        //    }
        //    List<CardEntity> cartype = new List<CardEntity>();
        //    CardEntity cardentity = new CardEntity();
        //    foreach (var c in dicCard)
        //    {
        //        cardentity.cardTypeID = c.Key.ToString();
        //        cardentity.cardTypeDes = c.Value.ToString();
        //        cartype.Add(cardentity);
        //    }

        //    return dicCard;
        //}

        public static List<QuickSearchSuggestItem> SuggestHotel(string keyword, int count)
        {
            #region 原始搜索方式

            //if (string.IsNullOrWhiteSpace(keyword))
            //{
            //    return new List<QuickSearchSuggestItem>();
            //}
            //var list = SearchTipApiService.GetSearchTipListByType(keyword, QuickSearchSuggestType.Hotel, count);
            //var res = (from d in list
            //           select new QuickSearchSuggestItem
            //           {
            //               EName = d.EngName,
            //               ParentName = d.DisName,
            //               Name = d.CnName,
            //               Type = GetSuggestType(d.Type),
            //               Id = int.Parse(d.Id),
            //               Tag = ResourceAdapter.IsPackageHotel(int.Parse(d.Id)) ? "特惠" : ""
            //           }).ToList();
            //return res;

            #endregion

            #region 索引搜索方式

            if (string.IsNullOrWhiteSpace(keyword))
            {
                return new List<QuickSearchSuggestItem>();
            }

            var searchHotelList = AccessService.SearchHotel(keyword, count);
            if (searchHotelList != null && searchHotelList.Count > 0)
            {
                var res = (from sitem in searchHotelList
                           select new QuickSearchSuggestItem
                           {
                               EName = sitem.Ename,
                               ParentName = "",
                               Name = sitem.HotelName,
                               Type = GetSuggestType("H"),
                               Id = int.Parse(sitem.HotelId.ToString()),
                               Tag = ResourceAdapter.IsPackageHotel(int.Parse(sitem.HotelId.ToString())) ? "特惠" : ""
                           }).ToList();
                return res;
            }
            else
            {
                return new List<QuickSearchSuggestItem>();
            }

            #endregion
        }

        public static List<CitySuggestItem> SuggestCity(string keyword, int count)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return new List<CitySuggestItem>();
            }

            keyword = keyword.Trim();
            var list = SearchTipApiService.GetSearchTipListByType(keyword, QuickSearchSuggestType.City, count);
            if (!list.Any())
            {
                if (keyword.EndsWith("市") || keyword.EndsWith("县"))
                {
                    list = SearchTipApiService.GetSearchTipListByType(keyword.Remove(keyword.Length - 1),
                                                         QuickSearchSuggestType.City, count);
                }
            }

            var res = (from d in list.Where(p => int.Parse(p.Id) < 100000)
                       select new CitySuggestItem
                       {
                           EName = d.EngName,
                           ParentName = d.DisName,
                           Name = d.CnName,
                           Id = int.Parse(d.Id)
                       }).ToList();

            return res;
        }

        public static void BindUserAccountAndOrders(long userid, string phone)
        {
            HotelService.BindUserAccountAndOrders(userid, phone);
        }

        public static CityList GetZMJDCityData2()
        {
            return LocalCache.GetData<CityList>("GetZMJDCityData2", () =>
            {
                CityList cl = new CityList();

                cl.Citys = GetZMJDCityData();
                cl.HotArea = GenHotArea(cl.Citys);
                cl.HotOverseaArea = GenHotOverseaArea(cl.Citys);//2015.08.21 4.0版新加 wwb 热门海外城市 webconfig维护
                cl.HMTCitys = GenHMTCitys(cl.Citys);//2016-05-26 list
                cl.SouthEastAsiaCitys = GenSouthEastAsiaCitys(cl.Citys);//2016-05-26 list

                cl.HotAreas = GenHotAreasDic(cl.Citys);
                cl.HotAreaKeys = cl.HotAreas.Keys.ToList();

                return cl;
            });
        }

        public static CityList GetZMJDSelectedCityData()
        {
            return LocalCache.GetData<CityList>("GetZMJDSelectedCityData", () =>
            {
                CityList cl = new CityList();

                cl.Citys = GenZMJDSelectedCityData();
                cl.HotArea = GenHotArea(cl.Citys);
                cl.HotOverseaArea = GenHotOverseaArea(cl.Citys);//2015.08.21 4.0版新加 wwb 热门海外城市 webconfig维护
                cl.HMTCitys = GenHMTCitys(cl.Citys);//2016-05-26 list
                cl.SouthEastAsiaCitys = GenSouthEastAsiaCitys(cl.Citys);//2016-05-26 list

                cl.HotAreas = GenHotAreasDic(cl.Citys);
                cl.HotAreaKeys = cl.HotAreas.Keys.ToList();

                return cl;
            });
        }

        public static CityList GetZMJDAllCityData()
        {
            return LocalCache.GetData<CityList>("GetZMJDAllCityData", () =>
            {
                CityList cl = new CityList();

                cl.Citys = GenZMJDAllCityData();
                cl.HotArea = GenHotArea(cl.Citys);
                cl.HotOverseaArea = GenHotOverseaArea(cl.Citys);//2015.08.21 4.0版新加 wwb 热门海外城市 webconfig维护
                cl.HMTCitys = GenHMTCitys(cl.Citys);//2016-05-26 list
                cl.SouthEastAsiaCitys = GenSouthEastAsiaCitys(cl.Citys);//2016-05-26 list
                cl.BoutiqueCity = GenBoutiqueCity(cl.Citys);//2016-05-26 精选list

                cl.HotAreas = GenHotAreasDic(cl.Citys);
                cl.HotAreaKeys = cl.HotAreas.Keys.ToList();

                return cl;
            });
        }

        /// <summary>
        /// 生成热门地区字典
        /// </summary>
        /// <param name="citys"></param>
        /// <returns></returns>
        private static Dictionary<string, List<HJD.HotelServices.Contracts.CityEntity>> GenHotAreasDic(List<HJD.HotelServices.Contracts.CityEntity> citys)
        {
            var dic = new Dictionary<string, List<HJD.HotelServices.Contracts.CityEntity>>();
            dic.Add("国内热门", GenHotArea(citys));
            dic.Add("港澳台日韩", GenHMTCitys(citys));
            dic.Add("东南亚/海岛度假", GenSouthEastAsiaCitys(citys));
            return dic;
        }

        public static List<HJD.HotelServices.Contracts.CityEntity> GetZmjdCityList()
        {
            return LocalCache.GetData<List<HJD.HotelServices.Contracts.CityEntity>>("GetZmjdCityList", () =>
            {
                return GenZMJDAllCityData();
            });
        }

        private static List<HJD.HotelServices.Contracts.CityEntity> GenHMTCitys(List<HJD.HotelServices.Contracts.CityEntity> citys)
        {
            List<HJD.HotelServices.Contracts.CityEntity> cl = new List<HJD.HotelServices.Contracts.CityEntity>();
            //港澳台城市
            foreach (string city in Configs.HMTCitys.Split(','))
            {
                var targetCity = citys.Where(c => c.Name == city).FirstOrDefault();
                if (targetCity != null && targetCity.ID > 0)
                {
                    cl.Add(targetCity);
                }
            }
            return cl;
        }

        private static List<HJD.HotelServices.Contracts.CityEntity> GenSouthEastAsiaCitys(List<HJD.HotelServices.Contracts.CityEntity> citys)
        {
            List<HJD.HotelServices.Contracts.CityEntity> cl = new List<HJD.HotelServices.Contracts.CityEntity>();
            //东南亚/海岛
            foreach (string city in Configs.SouthEastAsiaCitys.Split(','))
            {
                var targetCity = citys.Where(c => c.Name == city).FirstOrDefault();
                if (targetCity != null && targetCity.ID > 0)
                {
                    cl.Add(targetCity);
                }
            }
            return cl;
        }

        private static List<HJD.HotelServices.Contracts.CityEntity> GenBoutiqueCity(List<HJD.HotelServices.Contracts.CityEntity> citys)
        {
            List<HJD.HotelServices.Contracts.CityEntity> cl = new List<HJD.HotelServices.Contracts.CityEntity>();
            //精选地
            foreach (string city in Configs.BoutiqueCity.Split(','))
            {
                var targetCity = citys.Where(c => c.Name == city).FirstOrDefault();
                if (targetCity != null && targetCity.ID > 0)
                {
                    cl.Add(targetCity);
                }
            }
            return cl;
        }

        private static List<HJD.HotelServices.Contracts.CityEntity> GenHotArea(List<HJD.HotelServices.Contracts.CityEntity> citys)
        {
            List<HJD.HotelServices.Contracts.CityEntity> cl = new List<HJD.HotelServices.Contracts.CityEntity>();
            //国内热门
            foreach (string city in Configs.HotCitys.Split(','))
            {
                var targetCity = citys.Where(c => c.Name == city).FirstOrDefault();
                if (targetCity != null && targetCity.ID > 0)
                {
                    cl.Add(targetCity);
                }
            }
            return cl;
        }

        private static List<HJD.HotelServices.Contracts.CityEntity> GenHotOverseaArea(List<HJD.HotelServices.Contracts.CityEntity> citys)
        {
            List<HJD.HotelServices.Contracts.CityEntity> cl = new List<HJD.HotelServices.Contracts.CityEntity>();
            //海外热门
            foreach (string city in Configs.HotOverseaCitys.Split(','))
            {
                var targetCity = citys.Where(c => c.Name == city).FirstOrDefault();
                if (targetCity != null && targetCity.ID > 0)
                {
                    cl.Add(targetCity);
                }
            }

            return cl;
        }

        public static HomePageData GetHomePageData(int districtid = 2, int geoScopeType = 2)
        {
            return LocalCache30Min.GetData<HomePageData>("HomePageData" + districtid + ":" + geoScopeType, () =>
            {
                //HJD.DestServices.Contract.DistrictInfoEntity di = destService.GetDistrictInfo(new List<int> { districtid }).First();

                HomePageData hpd = new HomePageData();
                hpd.InterestData = ResourceAdapter.QueryInterest20(districtid, 0, 0);

                List<InterestEntity> themeList = hpd.InterestData.ThemeInterestList;
                hpd.InterestData.ThemeInterestList = themeList.FindAll(_ => _.ID < 100003);

                //if (geoScopeType == 1)
                //{
                //    hpd.InterestData = ResourceAdapter.QueryInterest30(districtid, 0, 0, geoScopeType);// ResourceAdapter.QueryInterest20(0, di.lat, di.lon);
                //}
                //else{
                //    hpd.InterestData = ResourceAdapter.QueryInterest20(districtid, 0, 0);// ResourceAdapter.QueryInterest20(0, di.lat, di.lon);
                //}
                HJD.HotelManagementCenter.Domain.CommDictEntity advPackage = commService.GetCommDict(10008, 1);

                List<int> pids = new List<int>();// { 30, 36, 38, 73, 81, 95 };

                foreach (string p in advPackage.Descript.Split(new char[] { '\r', '\n' }))
                {
                    if (p == "") continue;
                    pids.Add(int.Parse(p.Split(',')[0]));
                }

                PackageAdapter pa = new PackageAdapter();

                List<SimplePackageEntity> ls = pa.GetSimplePackageInfo(string.Join(",", pids));

                IEnumerable<HotPackageInfo> lhp = ls.Select(p => new HotPackageInfo()
                {
                    PID = p.Id,
                    PicURL = PhotoAdapter.GenHotelPicUrl(p.PicSURL, Enums.AppPhotoSize.appdetail),
                    Brief = p.Brief,
                    HotelID = p.HotelId,
                    HotelName = p.HotelName,
                    MinPrice = p.MinPrice,
                    PicSUrl = p.PicSURL,
                    ReviewCount = p.ReviewCount,
                    ReviewScore = p.ReviewScore
                });

                if (lhp.Count() > 4)
                {
                    hpd.topPackage = lhp.First();

                    hpd.Preferential = lhp.Skip(1).Take(4).ToList();
                }

                return hpd;
            });
        }

        public static HotelListMenu GetHotelListMenu(int districtid, int geoScopeType, double lat, double lng, int distance = 300000)
        {
            return LocalCache.GetData<HotelListMenu>(string.Format("HotelListMenu{0}:{1}:{2}:{3}", districtid, geoScopeType, geoScopeType == 3 ? (int)(lat * 100) : 0, geoScopeType == 3 ? (int)(lng * 100) : 0), () =>
            {
                InterestModel2 im;

                switch (geoScopeType)
                {
                    case 1://目的地
                        im = ResourceAdapter.QueryInterest20(districtid, 0, 0, 0);
                        break;
                    case 2://目的地周边
                        HJD.DestServices.Contract.DistrictInfoEntity di = destService.GetDistrictInfo(new List<int>() { districtid }).FirstOrDefault();
                        im = ResourceAdapter.QueryInterest20(0, di.lat, di.lon, distance == 0 ? 300000 : distance);
                        break;
                    default:
                        im = ResourceAdapter.QueryInterest20(0, (float)lat, (float)lng, distance == 0 ? 300000 : distance);
                        break;
                }

                HotelListMenu menu = new HotelListMenu();
                menu.InitMenu();

                menu.TotalNum = im.TotalHotelNum;

                menu.ThemeInterestList = im.ThemeInterestList;
                menu.SightInterestList = im.SightInterestList;

                return menu;
            });
        }

        public static List<HJD.HotelServices.Contracts.CityEntity> GetZMJDCityData()
        {
            return LocalCache.GetData<List<HJD.HotelServices.Contracts.CityEntity>>("ZMJDCityData", () => { return HotelService.GetZMJDCityData(); });
        }

        public static List<HJD.HotelServices.Contracts.CityEntity> GenZMJDSelectedCityData()
        {
            return LocalCache.GetData<List<HJD.HotelServices.Contracts.CityEntity>>("ZMJDSelectedCityData", () => { return HotelService.GetZMJDSelectedCityData(); });
        }

        public static List<HJD.HotelServices.Contracts.CityEntity> GenZMJDAllCityData()
        {
            return LocalCache.GetData<List<HJD.HotelServices.Contracts.CityEntity>>("ZMJDAllCityData", () => { return HotelService.GetZMJDAllCityData(); });
        }

        public List<HJD.HotelServices.Contracts.CityEntity> GetZMJDLoveCityData()
        {
            return LocalCache.GetData<List<HJD.HotelServices.Contracts.CityEntity>>("ZMJDLoveCityData", () => { return HotelService.GetZMJDLoveCityData(); });
        }

        /// <summary>
        /// 酒店搜索
        /// </summary>
        /// <returns></returns>
        public WapHotelsResult Search(HotelListQueryParam p)
        {
            DateTime arrivalTime = CommMethods.GetDefaultCheckIn();
            DateTime departureTime = arrivalTime.AddDays(1);

            int OTAID = GetOTAID(p.districtid);

            try
            {
                arrivalTime = DateTime.Parse(p.checkin);
                departureTime = DateTime.Parse(p.checkout);
                if (departureTime < arrivalTime.AddDays(1)) departureTime = arrivalTime.AddDays(1);
            }
            catch { }

            HotelSearchParas argu = new HotelSearchParas();

            argu.Featured = ParseIntArray(p.featured);
            argu.Distance = p.attraction > 0 ? 0 : p.distance;// 如果是按景区筛选，则不受目的地的限止
            argu.DistrictID = p.districtid;
            argu.Lat = p.lat;
            argu.Lng = p.lng;
            argu.nLat = p.nLat;
            argu.nLng = p.nLng;
            argu.ReturnCount = p.count;
            argu.StartIndex = p.start;
            argu.SortType = p.sort;
            argu.SortDirection = p.order;
            argu.Zone = ParseIntArray(p.zone);
            argu.Location = ParseIntArray(p.location);
            argu.Brands = ParseIntArray(p.brand);
            argu.sLat = p.sLat;
            argu.sLng = p.sLng;
            argu.MinPrice = p.minPrice;
            argu.MaxPrice = p.maxPrice;
            argu.Attraction = p.attraction;

            var model = new WapHotelsResult();
            model.menu = GetWapMenuEntity(argu, needTagLength);

            if (p.type == 0)//如果没有，那么缺省取第一个类型
            {
                if (model.menu.TypeItems.Count > 0)
                    p.type = model.menu.TypeItems.First().TypeID;
                else
                    p.type = 1001;
            }

            argu.Type = p.type;
            if (argu.Type > 0)
            {
                HotelTypeDefine hc = GetHotelClassDefine().Where(c => c.TypeID == argu.Type).FirstOrDefault();
                if (hc.SubHotelClass != null)
                {
                    argu.ClassID = (from c in hc.SubHotelClass
                                    select c.Key).ToArray();
                }
            }

            argu.TagIDs = ParseIntArray(p.tag);
            argu.Valued = p.valued;
            argu.CheckInDate = arrivalTime;
            argu.CheckOutDate = departureTime;

            argu.NeedHotelID = true;
            argu.NeedFilterCol = false; //不需要返回筛选项统计结果




            model.Start = argu.StartIndex;
            model.Type = argu.Type;
            //  model.Stype = stype;

            Stopwatch sw = new Stopwatch();
            var qResult = HotelService.QueryHotel(argu, OTAID);// HotelService.SearchHotel(argu).ToList();
            var hotels = qResult.HotelList;

            model.TotalCount = qResult.TotalCount;

            // model.ValuedCount = qResult.FilterCount.ContainsKey(110000000001)?qResult.FilterCount[110000000001]:0;



            if (hotels.Count > 0)
            {
                //价格数据
                if (p.maxPrice != -1)  //-1表示不需要价格数据
                {
                    List<HotelPriceEntity> hpl = PriceService.QueryHotelListPrice(hotels.Select(h => h.Id).ToList(), arrivalTime, departureTime);

                    //   List<HotelPrice> hpl = HotelService.QueryHotelListPriceWithOTAID(hotels.Select(h => h.Id).ToList(), arrivalTime, departureTime, OTAID);
                    if (hpl.Count > 0)
                    {
                        foreach (var h in hotels)
                        {
                            var thp = hpl.Where(hp => hp.HotelId == h.Id);
                            h.MinPrice = (int)(thp.Count() == 1 && thp.First().PriceList.Count() > 0 ? thp.First().PriceList.Min(P => P.Price) : 0);
                        }
                    }
                }
                //照片
                foreach (var hotel in hotels)
                {
                    hotel.Picture = hotel.PicSURL.Length == 0 ? PhotoAdapter.GenHotelPicUrl(defaultPicSUrl, Enums.AppPhotoSize.applist) : PhotoAdapter.GenHotelPicUrl(hotel.PicSURL, Enums.AppPhotoSize.applist);

                    hotel.PictureCount = 0;
                    hotel.OfficalPictureCount = 0;
                }

            }

            model.Result = hotels;

            model.DistrictName = ResourceAdapter.GetDistrictName(new List<int> { argu.DistrictID });

            model.AttractionName = ResourceAdapter.GetAttractionName(p.attraction);

            return model;
        }

        /// <summary>
        /// 主题酒店搜索
        /// </summary>
        /// <returns></returns>
        public WapThemeHotelsResult SearchThemeHotel(HotelListQueryParam p)
        {
            DateTime arrivalTime = CommMethods.GetDefaultCheckIn();
            DateTime departureTime = arrivalTime.AddDays(1);

            int OTAID = GetOTAID(p.districtid);

            try
            {
                arrivalTime = DateTime.Parse(p.checkin);
                departureTime = DateTime.Parse(p.checkout);
                if (departureTime < arrivalTime.AddDays(1)) departureTime = arrivalTime.AddDays(1);
            }
            catch { }

            HotelSearchParas argu = new HotelSearchParas();

            argu.Featured = ParseIntArray(p.featured);
            argu.Distance = p.attraction > 0 ? 0 : p.distance;// 如果是按景区筛选，则不受目的地的限止
            argu.DistrictID = p.districtid;
            argu.Lat = p.lat;
            argu.Lng = p.lng;
            argu.nLat = p.nLat;
            argu.nLng = p.nLng;
            argu.ReturnCount = p.count;
            argu.StartIndex = p.start;
            argu.SortType = p.sort;
            argu.SortDirection = p.order;
            argu.Zone = ParseIntArray(p.zone);
            argu.Location = ParseIntArray(p.location);
            argu.Brands = ParseIntArray(p.brand);
            argu.sLat = p.sLat;
            argu.sLng = p.sLng;
            argu.MinPrice = p.minPrice;
            argu.MaxPrice = p.maxPrice;
            argu.Attraction = p.attraction;
            argu.HotelTheme = p.hotelTheme;

            if (argu.Lng < 0 || argu.Lat < 0)  //如果在南半球 或 在美国，那么只按排名排序
            {
                argu.SortType = 0;
            }

            var model = new WapThemeHotelsResult();

            argu.TagIDs = ParseIntArray(p.tag);
            argu.Valued = p.valued;
            argu.CheckInDate = arrivalTime;
            argu.CheckOutDate = departureTime;

            argu.NeedHotelID = true;
            argu.NeedFilterCol = false; //不需要返回筛选项统计结果


            model.Start = argu.StartIndex;

            Stopwatch sw = new Stopwatch();
            var qResult = HotelService.QueryHotel(argu, OTAID);// HotelService.SearchHotel(argu).ToList();
            var hotels = qResult.HotelList;

            model.TotalCount = qResult.TotalCount;

            if (hotels.Count > 0)
            {
                //价格数据
                if (p.maxPrice != -1)  //-1表示不需要价格数据
                {
                    List<HotelPriceEntity> hpl = PriceService.QueryHotelListPrice(hotels.Select(h => h.Id).ToList(), arrivalTime, departureTime);

                    //   List<HotelPrice> hpl = HotelService.QueryHotelListPriceWithOTAID(hotels.Select(h => h.Id).ToList(), arrivalTime, departureTime, OTAID);
                    if (hpl.Count > 0)
                    {
                        foreach (var h in hotels)
                        {
                            var thp = hpl.Where(hp => hp.HotelId == h.Id);
                            h.MinPrice = (int)(thp.Count() == 1 && thp.First().PriceList.Count() > 0 ? thp.First().PriceList.Min(P => P.Price) : 0);
                        }
                    }
                }
                //照片
                foreach (var hotel in hotels)
                {
                    hotel.Picture = hotel.PicSURL.Length == 0 ? PhotoAdapter.GenHotelPicUrl(defaultPicSUrl, Enums.AppPhotoSize.applist2) : PhotoAdapter.GenHotelPicUrl(hotel.PicSURL, Enums.AppPhotoSize.applist2);

                    hotel.PictureCount = 0;
                    hotel.OfficalPictureCount = 0;
                }

            }

            model.Result = hotels;

            model.ThemeID = p.hotelTheme;
            model.ThemeName = ResourceAdapter.GetHotelThemeName(model.ThemeID);


            return model;
        }

        /// <summary>
        /// 玩点酒店搜索
        /// </summary>
        /// <returns></returns>
        public WapInterestHotelsResult SearchInterestHotel(HotelListQueryParam p)
        {
            DateTime arrivalTime = CommMethods.GetDefaultCheckIn();
            DateTime departureTime = arrivalTime.AddDays(1);

            try
            {
                arrivalTime = DateTime.Parse(p.checkin);
                departureTime = DateTime.Parse(p.checkout);
                if (departureTime < arrivalTime.AddDays(1)) departureTime = arrivalTime.AddDays(1);
            }
            catch { }

            HotelSearchParas argu = new HotelSearchParas();


            argu.Featured = ParseIntArray(p.featured);
            argu.Distance = p.attraction > 0 ? 0 : p.distance;// 如果是按景区筛选，则不受目的地的限止
            argu.DistrictID = p.districtid;
            argu.Lat = p.lat;
            argu.Lng = p.lng;
            argu.nLat = p.nLat;
            argu.nLng = p.nLng;
            argu.ReturnCount = p.count;
            argu.StartIndex = p.start;
            argu.SortType = p.sort;
            argu.SortDirection = p.order;
            argu.Zone = ParseIntArray(p.zone);
            argu.Location = ParseIntArray(p.location);
            argu.Brands = ParseIntArray(p.brand);
            argu.sLat = p.sLat;
            argu.sLng = p.sLng;
            argu.MinPrice = p.minPrice;
            argu.MaxPrice = p.maxPrice;
            argu.Attraction = p.attraction;
            argu.HotelTheme = p.hotelTheme;

            if (argu.Lng < 0 || argu.Lat < 0)  //如果在南半球 或 在美国，那么只按排名排序
            {
                argu.SortType = 0;
            }

            var model = new WapInterestHotelsResult();

            argu.TagIDs = ParseIntArray(p.tag);
            argu.Valued = p.valued;
            argu.CheckInDate = arrivalTime;
            argu.CheckOutDate = departureTime;

            argu.NeedHotelID = true;
            argu.NeedFilterCol = false; //不需要返回筛选项统计结果


            model.Start = argu.StartIndex;

            Stopwatch sw = new Stopwatch();
            var qResult = HotelService.QueryHotel(argu);// HotelService.SearchHotel(argu).ToList();
            var hotels = qResult.HotelList;

            model.TotalCount = qResult.TotalCount;

            if (hotels.Count > 0)
            {
                //价格数据
                if (p.maxPrice != -1)  //-1表示不需要价格数据
                {
                    List<HotelPriceEntity> hpl = PriceService.QueryHotelListPrice(hotels.Select(h => h.Id).ToList(), arrivalTime, departureTime);

                    //   List<HotelPrice> hpl = HotelService.QueryHotelListPriceWithOTAID(hotels.Select(h => h.Id).ToList(), arrivalTime, departureTime, OTAID);
                    if (hpl.Count > 0)
                    {
                        foreach (var h in hotels)
                        {
                            var thp = hpl.Where(hp => hp.HotelId == h.Id);
                            h.MinPrice = (int)(thp.Count() == 1 && thp.First().PriceList.Count() > 0 ? thp.First().PriceList.Min(P => P.Price) : 0);
                        }
                    }
                }
                //照片 排名
                int rank = p.start + 1;
                foreach (var hotel in hotels)
                {
                    hotel.Picture = hotel.PicSURL.Length == 0 ? PhotoAdapter.GenHotelPicUrl(defaultPicSUrl, Enums.AppPhotoSize.applist2) : PhotoAdapter.GenHotelPicUrl(hotel.PicSURL, Enums.AppPhotoSize.applist2);

                    hotel.PictureCount = 0;
                    hotel.OfficalPictureCount = 0;

                    hotel.Rank = rank;
                    rank++;
                }

            }

            model.Result = hotels;
            model.InterestType = p.type;
            model.InterestID = p.type == 1 ? p.hotelTheme : p.attraction;
            model.InterestName = p.type == 1 ? ResourceAdapter.GetHotelThemeName(p.hotelTheme) : ResourceAdapter.GetAttractionName(p.attraction); ;


            return model;
        }

        /// <summary>
        /// 网站酒店列表接口
        /// 只有目的地时如何出
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static InterestHotelsResult QueryInterestHotel(HotelListQueryParam p)
        {
            InterestHotelsResult ihr = new InterestHotelsResult();
            ihr.filters = GetHotelListMenu(p.districtid, p.geoScopeType, p.lat, p.lng, p.distance);

            //switch (p.geoScopeType)
            //{
            //    case 1: //按目的地查
            //        p.InterestPlace = string.Join(",", GetInterestPlaceIDs(p.Interest, p.districtid, 0, 0, p.distance));
            //        break;
            //    case 2://查城市周边
            //       HJD.DestServices.Contract.DistrictInfoEntity di =  destService.GetDistrictInfo(new List<int>() { p.districtid }).First();
            //        p.InterestPlace = string.Join(",", GetInterestPlaceIDs(p.Interest, 0, di.lat, di.lon, p.distance));
            //        if (p.sort == 1) //按口碑排
            //            p.sort = 6; //按所有玩点的口碑排序
            //        break;
            //    case 3://用户坐标附近
            //        p.InterestPlace = string.Join(",", GetInterestPlaceIDs(p.Interest, 0, p.lat, p.lng, p.distance));
            //        if (p.sort == 1) //按口碑排
            //            p.sort = 6; //按所有玩点的口碑排序
            //        break;
            //}


            WapInterestHotelsResult2 hotels = SearchInterestHotel30(p);

            ihr.hotels = hotels;
            ihr.TotalCount = hotels.TotalCount;

            return ihr;
        }

        public static int[] GetInterestPlaceIDs(int interestID, int districtID, float lat, float lon, int distance)
        {
            InterestModel2 im = ResourceAdapter.QueryInterest20(districtID, lat, lon, distance, true);

            List<string> ips = new List<string>();

            if (interestID == 0) //显示所有酒店
            {

                foreach (InterestEntity ie in im.ThemeInterestList)
                {
                    ips.AddRange(ie.InterestPlaceIDs.Split(','));
                }
                foreach (InterestEntity ie in im.SightInterestList)
                {
                    ips.AddRange(ie.InterestPlaceIDs.Split(','));
                }
            }
            else
            {
                foreach (InterestEntity ie in im.ThemeInterestList.Where(o => o.ID == interestID))
                {
                    ips.AddRange(ie.InterestPlaceIDs.Split(','));
                }
                foreach (InterestEntity ie in im.SightInterestList.Where(o => o.ID == interestID))
                {
                    ips.AddRange(ie.InterestPlaceIDs.Split(','));
                }
            }

            return ips.Select(s => int.Parse(s)).ToArray();
        }

        /// <summary>
        /// 玩点酒店搜索
        /// </summary>
        /// <returns></returns>
        public WapInterestHotelsResult SearchInterestHotel2(HotelListQueryParam p)
        {
            DateTime arrivalTime = CommMethods.GetDefaultCheckIn();
            DateTime departureTime = arrivalTime.AddDays(1);

            try
            {
                arrivalTime = DateTime.Parse(p.checkin);
                departureTime = DateTime.Parse(p.checkout);
                if (departureTime < arrivalTime.AddDays(1)) departureTime = arrivalTime.AddDays(1);
            }
            catch { }

            HotelSearchParas argu = new HotelSearchParas();


            argu.Interest = p.Interest;
            argu.InterestPlace = GetInterestPlaceIDs(p.Interest, p.districtid, p.lat, p.lng, p.distance);


            argu.Featured = ParseIntArray(p.featured);
            argu.Distance = p.attraction > 0 ? 0 : p.distance;// 如果是按景区筛选，则不受目的地的限止
            argu.DistrictID = p.districtid;
            argu.Lat = p.lat;
            argu.Lng = p.lng;
            argu.nLat = p.nLat;
            argu.nLng = p.nLng;
            argu.ReturnCount = p.count;
            argu.StartIndex = p.start;
            argu.SortType = p.sort;
            argu.SortDirection = p.order;
            argu.Zone = ParseIntArray(p.zone);
            argu.Location = ParseIntArray(p.location);
            argu.Brands = ParseIntArray(p.brand);
            argu.sLat = p.sLat;
            argu.sLng = p.sLng;
            argu.MinPrice = p.minPrice;
            argu.MaxPrice = p.maxPrice;
            argu.Attraction = p.attraction;
            argu.HotelTheme = p.hotelTheme;

            if (argu.Lng < 0 || argu.Lat < 0)  //如果在南半球 或 在美国，那么只按排名排序
            {
                argu.SortType = 0;
            }

            var model = new WapInterestHotelsResult();

            argu.TagIDs = ParseIntArray(p.tag);
            argu.Valued = p.valued;
            argu.CheckInDate = arrivalTime;
            argu.CheckOutDate = departureTime;

            argu.NeedHotelID = true;
            argu.NeedFilterCol = false; //不需要返回筛选项统计结果


            model.Start = argu.StartIndex;

            //      Stopwatch sw = new Stopwatch();
            var qResult = HotelService.QueryHotel(argu);// HotelService.SearchHotel(argu).ToList();
            var hotels = qResult.HotelList;

            model.TotalCount = qResult.TotalCount;

            if (hotels.Count > 0)
            {
                //价格数据
                if (p.maxPrice != -1)  //-1表示不需要价格数据, 为appCheck所用
                {
                    List<HotelPriceEntity> hpl = PriceService.QueryHotelListPrice(hotels.Select(h => h.Id).ToList(), arrivalTime, departureTime);

                    //   List<HotelPrice> hpl = HotelService.QueryHotelListPriceWithOTAID(hotels.Select(h => h.Id).ToList(), arrivalTime, departureTime, OTAID);
                    if (hpl.Count > 0)
                    {
                        foreach (var h in hotels)
                        {
                            var thp = hpl.Where(hp => hp.HotelId == h.Id);
                            h.MinPrice = (int)(thp.Count() == 1 && thp.First().PriceList.Count() > 0 ? thp.First().PriceList.Min(P => P.Price) : 0);
                        }
                    }
                }

                //照片 排名
                int rank = p.start + 1;
                foreach (var hotel in hotels)
                {
                    hotel.Picture = hotel.PicSURL.Length == 0 ? PhotoAdapter.GenHotelPicUrl(defaultPicSUrl, Enums.AppPhotoSize.applist2) : PhotoAdapter.GenHotelPicUrl(hotel.PicSURL, Enums.AppPhotoSize.applist2);

                    hotel.PictureCount = 0;
                    hotel.OfficalPictureCount = 0;

                    hotel.Rank = rank;
                    rank++;
                }
            }

            model.Result = hotels;
            model.InterestType = p.type;
            model.InterestID = p.Interest;// p.type == 1 ? p.hotelTheme : p.attraction;
            model.InterestName = ResourceAdapter.GetInterestName(p.Interest);// p.type == 1 ? ResourceAdapter.GetHotelThemeName(p.hotelTheme) : ResourceAdapter.GetAttractionName(p.attraction); ;


            return model;
        }

        /// <summary>
        /// 玩点酒店搜索
        /// </summary>
        /// <returns></returns>
        public WapInterestHotelsResult2 SearchInterestHotel20(HotelListQueryParam p)
        {
            DateTime arrivalTime = CommMethods.GetDefaultCheckIn();
            DateTime departureTime = arrivalTime.AddDays(1);

            try
            {
                arrivalTime = DateTime.Parse(p.checkin);
                departureTime = DateTime.Parse(p.checkout);
                if (departureTime < arrivalTime.AddDays(1)) departureTime = arrivalTime.AddDays(1);
            }
            catch { }

            HotelSearchParas argu = new HotelSearchParas();

            argu.Interest = p.Interest == 0 ? (int)EnumHelper.InterestID.AllHotelInterest : p.Interest;// 110000全部酒店列表

            if (p.InterestPlace != null && p.InterestPlace.Length > 0)
            {
                argu.InterestPlace = p.InterestPlace.Split(',').Select(s => int.Parse(s)).ToArray();
            }
            else
            {
                argu.InterestPlace = GetInterestPlaceIDs(p.Interest, p.districtid, p.lat, p.lng, p.distance);
            }

            argu.ReturnCount = p.count;
            argu.StartIndex = p.start;

            // 排序方式：0：默认 1：口碑 2：距离 3：价格正序 4：价格倒序
            switch (p.sort)
            {
                case 0://0：默认
                    argu.SortType = 10;
                    argu.SortDirection = 0;
                    break;
                case 1://1：口碑
                    argu.SortType = 0;
                    argu.SortDirection = 0;
                    break;
                case 2://2：距离
                    argu.SortType = 2;
                    argu.SortDirection = 0;
                    break;
                case 3://3：价格正序
                    argu.SortType = 1;
                    argu.SortDirection = 0;
                    break;
                case 4://4：价格倒序
                    argu.SortType = 1;
                    argu.SortDirection = 1;
                    break;
            }

            argu.Lat = p.lat;
            argu.Lng = p.lng;

            if (p.maxPrice > 0)
            {
                argu.MaxPrice = p.maxPrice;
                argu.MinPrice = p.minPrice;
            }

            if (p.star > 0)
            {
                argu.Star = HotelHelper.TransStarArgu(p.star);
            }
            //argu.Distance = p.distance;           
            if (argu.Lng < 0 || argu.Lat < 0)  //如果在南半球 或 在美国，那么只按排名排序
            {
                argu.SortType = 0;
            }

            var model = new WapInterestHotelsResult2();

            argu.CheckInDate = arrivalTime;
            argu.CheckOutDate = departureTime;

            argu.NeedHotelID = true;
            argu.NeedFilterCol = false; //不需要返回筛选项统计结果

            model.Start = argu.StartIndex;

            var qResult = HotelService.QueryHotel2(argu);//HotelService.SearchHotel(argu).ToList();
            var hotels = qResult.HotelList;

            model.TotalCount = qResult.TotalCount;

            if (hotels.Count > 0)
            {
                //价格数据
                if (p.maxPrice != -1)  //-1表示不需要价格数据
                {
                    GenHotelListPrice(null, hotels, arrivalTime, departureTime);
                }

                //照片 排名
                int rank = p.start + 1;
                foreach (var hotel in hotels)
                {
                    hotel.Picture = hotel.PicSURL.Length == 0 ? PhotoAdapter.GenHotelPicUrl(defaultPicSUrl, Enums.AppPhotoSize.appdetail) : PhotoAdapter.GenHotelPicUrl(hotel.PicSURL, Enums.AppPhotoSize.appdetail);
                }
            }

            model.Result = hotels;
            //model.InterestType = p.type;
            model.InterestID = p.Interest;
            model.InterestName = ResourceAdapter.GetInterestName(p.Interest);
            return model;
        }

        /// <summary>
        /// 玩点酒店搜索 JLTour
        /// </summary>
        /// <returns></returns>
        public static WapInterestHotelsResult2 SearchInterestHotel30(HotelListQueryParam p)
        {
            int defaultDistance = 300000;

            DateTime arrivalTime = CommMethods.GetDefaultCheckIn();
            DateTime departureTime = arrivalTime.AddDays(1);

            try
            {
                arrivalTime = DateTime.Parse(p.checkin);
                departureTime = DateTime.Parse(p.checkout);
                if (departureTime < arrivalTime.AddDays(1)) departureTime = arrivalTime.AddDays(1);
            }
            catch { }

            HotelSearchParas argu = new HotelSearchParas();

            argu.Interest = p.Interest == 0 ? (int)EnumHelper.InterestID.AllHotelInterest : p.Interest;

            if (argu.Interest == (int)EnumHelper.InterestID.SpecialDealHotelInterest)//如果是特惠精选，那么需要特殊的解析 InterestPlacIDs的方式
            {
                if (p.InterestPlace == null || p.InterestPlace.Length == 0)
                {
                    List<string> ipids = ResourceAdapter.GetSpecialDealInterestPlaceIDs(p.districtid, p.lat, p.lng, p.geoScopeType);
                    p.InterestPlace = string.Join(",", ipids);
                }
            }


            if (p.InterestPlace == null || p.InterestPlace.Length == 0)
            {
                switch (p.geoScopeType)
                {
                    case 1: //按目的地查
                        p.InterestPlace = string.Join(",", GetInterestPlaceIDs(p.Interest, p.districtid, 0, 0, p.distance));
                        break;
                    case 2://查城市周边
                        HJD.DestServices.Contract.DistrictInfoEntity di = destService.GetDistrictInfo(new List<int>() { p.districtid }).First();
                        p.InterestPlace = string.Join(",", GetInterestPlaceIDs(p.Interest, 0, di.lat, di.lon, p.distance == 0 ? defaultDistance : p.distance));
                        if (p.sort == 1) //按口碑排
                            p.sort = 6; //按所有玩点的口碑排序
                        break;
                    case 3://用户坐标附近
                        p.InterestPlace = string.Join(",", GetInterestPlaceIDs(p.Interest, 0, p.lat, p.lng, p.distance == 0 ? defaultDistance : p.distance));
                        if (p.sort == 1) //按口碑排
                            p.sort = 6; //按所有玩点的口碑排序
                        break;
                }

            }



            if (p.InterestPlace != null && p.InterestPlace.Length > 0)
            {
                argu.InterestPlace = p.InterestPlace.Split(',').Select(i => int.Parse(i)).ToArray();
            }
            else if (p.districtid > 0)
            {
                InterestModel im = ResourceAdapter.QueryInterest2(p.districtid, 0, 0, 0);
                if (im.InterestList.Where(i => i.ID == 11).Count() > 0)
                {
                    argu.InterestPlace = im.InterestList.Where(i => i.ID == 11).First().InterestPlaceIDs.Split(',').Select(i => int.Parse(i)).ToArray();
                }
            }

            argu.ReturnCount = p.count;

            argu.StartIndex = p.start;

            // 排序方式：0：默认 1：口碑 2：距离 3：价格正序 4：价格倒序   6：按所有玩点口碑 10:套餐先排
            switch (p.sort)
            {
                case 0://0：默认
                    argu.SortType = 10;
                    argu.SortDirection = 0;
                    break;
                case 1://1：口碑
                    argu.SortType = 0;
                    argu.SortDirection = 0;
                    break;
                case 6://按所有玩点口碑
                    argu.SortType = 6;
                    argu.SortDirection = 0;
                    break;
                case 2://2：距离
                    argu.SortType = 2;
                    argu.SortDirection = 0;
                    break;
                case 3://3：价格正序
                    argu.SortType = 1;
                    argu.SortDirection = 0;
                    break;
                case 4://4：价格倒序
                    argu.SortType = 1;
                    argu.SortDirection = 1;
                    break;
            }

            argu.Lat = p.lat;
            argu.Lng = p.lng;

            if (argu.Lng < 0 || argu.Lat < 0)  //如果在南半球 或 在美国，那么只按排名排序
            {
                argu.SortType = 0;
            }

            var model = new WapInterestHotelsResult2();

            argu.CheckInDate = arrivalTime;
            argu.CheckOutDate = departureTime;

            argu.NeedHotelID = true;
            argu.NeedFilterCol = false; //不需要返回筛选项统计结果

            if (p.star > 0)
            {
                argu.Star = HotelHelper.TransStarArgu(p.star);
            }


            model.Start = argu.StartIndex;

            var qResult = HotelService.QueryHotel2(argu);// HotelService.SearchHotel(argu).ToList();
            var hotels = qResult.HotelList;

            model.TotalCount = qResult.TotalCount;

            if (hotels.Count > 0)
            {
                //价格数据
                if (p.maxPrice != -1)  //-1表示不需要价格数据
                {
                    GenHotelListPrice(null, hotels, arrivalTime, departureTime);
                }

                //照片 排名
                int rank = p.start + 1;
                foreach (var hotel in hotels)
                {
                    hotel.Picture = hotel.PicSURL.Length == 0 ? PhotoAdapter.GenHotelPicUrl(defaultPicSUrl, Enums.AppPhotoSize.appdetail) : PhotoAdapter.GenHotelPicUrl(hotel.PicSURL, Enums.AppPhotoSize.appdetail);
                }
            }

            model.Result = hotels;
            //  model.InterestType = p.type;
            model.InterestID = p.Interest;
            model.InterestName = ResourceAdapter.GetInterestName(p.Interest);
            return model;
        }

        /// <summary>
        /// 玩点酒店搜索 JLTour
        /// </summary>
        /// <returns></returns>
        public static WapInterestHotelsResult3 SearchInterestHotel40(HotelListQueryParam p)
        {
            int defaultDistance = 300000;

            DateTime arrivalTime = CommMethods.GetDefaultCheckIn();
            DateTime departureTime = arrivalTime.AddDays(1);

            try
            {
                arrivalTime = DateTime.Parse(p.checkin);
                departureTime = DateTime.Parse(p.checkout);
                if (departureTime < arrivalTime.AddDays(1)) departureTime = arrivalTime.AddDays(1);
            }
            catch { }

            HotelSearchParas argu = new HotelSearchParas();

            argu.Interest = p.Interest == 0 ? (int)EnumHelper.InterestID.AllHotelInterest : p.Interest;

            if (argu.Interest == (int)EnumHelper.InterestID.SpecialDealHotelInterest)//如果是特惠精选，那么需要特殊的解析 InterestPlacIDs的方式
            {
                if (p.InterestPlace == null || p.InterestPlace.Length == 0)
                {
                    List<string> ipids = ResourceAdapter.GetSpecialDealInterestPlaceIDs(p.districtid, p.lat, p.lng, p.geoScopeType);
                    p.InterestPlace = string.Join(",", ipids);
                }
            }


            if (p.InterestPlace == null || p.InterestPlace.Length == 0)
            {
                switch (p.geoScopeType)
                {
                    case 1: //按目的地查
                        p.InterestPlace = string.Join(",", GetInterestPlaceIDs(p.Interest, p.districtid, 0, 0, p.distance));
                        break;
                    case 2://查城市周边
                        HJD.DestServices.Contract.DistrictInfoEntity di = destService.GetDistrictInfo(new List<int>() { p.districtid }).First();
                        p.InterestPlace = string.Join(",", GetInterestPlaceIDs(p.Interest, 0, di.lat, di.lon, p.distance == 0 ? defaultDistance : p.distance));
                        if (p.sort == 1) //按口碑排
                            p.sort = 6; //按所有玩点的口碑排序
                        break;
                    case 3://用户坐标附近
                        p.InterestPlace = string.Join(",", GetInterestPlaceIDs(p.Interest, 0, p.lat, p.lng, p.distance == 0 ? defaultDistance : p.distance));
                        if (p.sort == 1) //按口碑排
                            p.sort = 6; //按所有玩点的口碑排序
                        break;
                }

            }



            if (p.InterestPlace != null && p.InterestPlace.Length > 0)
            {
                argu.InterestPlace = p.InterestPlace.Split(',').Select(i => int.Parse(i)).ToArray();
            }
            else if (p.districtid > 0)
            {
                InterestModel im = ResourceAdapter.QueryInterest2(p.districtid, 0, 0, 0);
                if (im.InterestList.Where(i => i.ID == 11).Count() > 0)
                {
                    argu.InterestPlace = im.InterestList.Where(i => i.ID == 11).First().InterestPlaceIDs.Split(',').Select(i => int.Parse(i)).ToArray();
                }
            }

            argu.ReturnCount = p.count;

            argu.StartIndex = p.start;

            // 排序方式：0：默认 1：口碑 2：距离 3：价格正序 4：价格倒序   6：按所有玩点口碑 10:套餐先排
            switch (p.sort)
            {
                case 0://0：默认
                    argu.SortType = 10;// 6;
                    argu.SortDirection = 0;
                    break;
                case 1://1：口碑
                    argu.SortType = 0;
                    argu.SortDirection = 0;
                    break;
                case 6://按所有玩点口碑
                    argu.SortType = 6;
                    argu.SortDirection = 0;
                    break;
                case 2://2：距离
                    argu.SortType = 2;
                    argu.SortDirection = 0;
                    break;
                case 3://3：价格正序
                    argu.SortType = 1;
                    argu.SortDirection = 0;
                    break;
                case 4://4：价格倒序
                    argu.SortType = 1;
                    argu.SortDirection = 1;
                    break;
            }

            argu.Lat = p.lat;
            argu.Lng = p.lng;

            if (argu.Lng < 0 || argu.Lat < 0)  //如果在南半球 或 在美国，那么只按排名排序
            {
                argu.SortType = 0;
            }

            var model = new WapInterestHotelsResult3();

            argu.CheckInDate = arrivalTime;
            argu.CheckOutDate = departureTime;

            argu.MinPrice = p.minPrice;
            argu.MaxPrice = p.maxPrice;

            argu.NeedHotelID = true;
            argu.NeedFilterCol = false; //不需要返回筛选项统计结果

            if (p.star > 0)
            {
                argu.Star = HotelHelper.TransStarArgu(p.star);
            }

            model.Start = argu.StartIndex;

            var qResult = HotelService.QueryHotel3(argu);// HotelService.SearchHotel(argu).ToList();
            var hotels = qResult.HotelList;

            model.TotalCount = qResult.TotalCount;

            if (hotels.Count > 0)
            {
                //价格数据
                if (p.maxPrice != -1)  //-1表示不需要价格数据
                {
                    //GenHotelListPrice(hotels, null,arrivalTime,departureTime);
                    GenHotelListSlotPrice(hotels, null, arrivalTime, departureTime, p.minPrice, p.maxPrice);
                }

                //照片 排名
                int rank = p.start + 1;

                List<InterestCommentEntity> hicl = CommentAdapter.GetHotelInterestComment(p.Interest, hotels.Select(h => h.Id).ToList());

                foreach (var hotel in hotels)
                {
                    hotel.InterestComment = "“";
                    if (hicl.Where(h => h.Hotelid == hotel.Id).Count() > 0)
                    {
                        InterestCommentEntity hic = hicl.Where(h => h.Hotelid == hotel.Id).First();
                        hotel.InterestComment += FormatHotelListComment(hic.Comment.Length > 0 ? hic.Comment : hic.CalComment);
                    }
                    hotel.InterestComment += "”";
                    hotel.PictureList = hotel.PictureSURLList.Count == 0 ? new List<string>() { PhotoAdapter.GenHotelPicUrl(defaultPicSUrl, Enums.AppPhotoSize.theme) } :
                        hotel.PictureSURLList.Select(s => PhotoAdapter.GenHotelPicUrl(s, Enums.AppPhotoSize.theme)).ToList();

                    //hotel.Tag = string.IsNullOrWhiteSpace(hotel.Tag) ? "" : hotel.Tag;
                }
            }

            model.Result = hotels;
            model.InterestID = p.Interest;
            model.InterestName = ResourceAdapter.GetInterestName(p.Interest);
            model.filters = GetHotelListMenu(p.districtid, p.geoScopeType, p.lat, p.lng, p.distance);

            return model;
        }

        /// <summary>
        /// 玩点酒店搜索 JLTour
        /// </summary>
        /// <returns></returns>
        public static WapInterestHotelsResult3 SearchInterestHotel50(HotelListQueryParam p)
        {
            int defaultDistance = 300000;

            DateTime arrivalTime = CommMethods.GetDefaultCheckIn();
            DateTime departureTime = arrivalTime.AddDays(1);

            try
            {
                arrivalTime = DateTime.Parse(p.checkin);
                departureTime = DateTime.Parse(p.checkout);
                if (departureTime < arrivalTime.AddDays(1)) departureTime = arrivalTime.AddDays(1);
            }
            catch { }

            HotelSearchParas argu = new HotelSearchParas();

            argu.Interest = p.Interest == 0 ? (int)EnumHelper.InterestID.AllHotelInterest : p.Interest;

            if (argu.Interest == (int)EnumHelper.InterestID.SpecialDealHotelInterest)//如果是特惠精选，那么需要特殊的解析 InterestPlacIDs的方式
            {
                if (p.InterestPlace == null || p.InterestPlace.Length == 0)
                {
                    List<string> ipids = ResourceAdapter.GetSpecialDealInterestPlaceIDs(p.districtid, p.lat, p.lng, p.geoScopeType);
                    p.InterestPlace = string.Join(",", ipids);
                }
            }

            if (p.InterestPlace == null || p.InterestPlace.Length == 0)
            {
                switch (p.geoScopeType)
                {
                    case 1: //按目的地查
                        p.InterestPlace = string.Join(",", GetInterestPlaceIDs(p.Interest, p.districtid, 0, 0, p.distance));
                        break;
                    case 2://查城市周边
                        HJD.DestServices.Contract.DistrictInfoEntity di = destService.GetDistrictInfo(new List<int>() { p.districtid }).First();
                        p.InterestPlace = string.Join(",", GetInterestPlaceIDs(p.Interest, 0, di.lat, di.lon, p.distance == 0 ? defaultDistance : p.distance));
                        if (p.sort == 1) //按口碑排
                            p.sort = 6; //按所有玩点的口碑排序
                        break;
                    case 3://用户坐标附近
                        p.InterestPlace = string.Join(",", GetInterestPlaceIDs(p.Interest, 0, p.lat, p.lng, p.distance == 0 ? defaultDistance : p.distance));
                        if (p.sort == 1) //按口碑排
                            p.sort = 6; //按所有玩点的口碑排序
                        break;
                }
            }

            if (p.InterestPlace != null && p.InterestPlace.Length > 0)
            {
                argu.InterestPlace = p.InterestPlace.Split(',').Select(i => int.Parse(i)).ToArray();
            }
            else if (p.districtid > 0)
            {
                InterestModel im = ResourceAdapter.QueryInterest2(p.districtid, 0, 0, 0);
                if (im.InterestList.Where(i => i.ID == 11).Count() > 0)
                {
                    argu.InterestPlace = im.InterestList.Where(i => i.ID == 11).First().InterestPlaceIDs.Split(',').Select(i => int.Parse(i)).ToArray();
                }
            }

            argu.ReturnCount = p.count;

            argu.StartIndex = p.start;

            // 排序方式：0：默认 1：口碑 2：距离 3：价格正序 4：价格倒序   6：按所有玩点口碑 10:套餐先排
            switch (p.sort)
            {
                case 0://0：默认
                    argu.SortType = 6;// 10;
                    argu.SortDirection = 0;
                    break;
                case 1://1：口碑
                    argu.SortType = 0;
                    argu.SortDirection = 0;
                    break;
                case 6://按所有玩点口碑
                    argu.SortType = 6;
                    argu.SortDirection = 0;
                    break;
                case 2://2：距离
                    argu.SortType = 2;
                    argu.SortDirection = 0;
                    break;
                case 3://3：价格正序
                    argu.SortType = 1;
                    argu.SortDirection = 0;
                    break;
                case 4://4：价格倒序
                    argu.SortType = 1;
                    argu.SortDirection = 1;
                    break;
            }

            argu.Lat = p.lat;
            argu.Lng = p.lng;

            if (argu.Lng < 0 || argu.Lat < 0)  //如果在南半球 或 在美国，那么只按排名排序
            {
                argu.SortType = 0;
            }

            var model = new WapInterestHotelsResult3();

            argu.CheckInDate = arrivalTime;
            argu.CheckOutDate = departureTime;

            argu.MinPrice = p.minPrice;
            argu.MaxPrice = p.maxPrice;

            argu.NeedHotelID = true;
            argu.NeedFilterCol = false; //不需要返回筛选项统计结果

            if (p.star > 0)
            {
                argu.Star = HotelHelper.TransStarArgu(p.star);
            }

            model.Start = argu.StartIndex;

            var qResult = HotelService.QueryHotel3(argu);// HotelService.SearchHotel(argu).ToList();
            var hotels = qResult.HotelList;

            model.TotalCount = qResult.TotalCount;

            //需要获取数据 start传的参数
            List<ListHotelItem3> item3s = new List<ListHotelItem3>();
            //审核过滤条件
            if (p.hotelid > 0 && p.Interest > 0)
            {
                for (int i = hotels.Count - 1; i >= 0; i--)
                {
                    int hotelID = hotels[i].Id;
                    if (hotelID == p.hotelid)
                    {
                        int cacheHotel = GetSearchHotelInterestCacheHotelId(p.hotelid, p.Interest, p.districtid, p.geoScopeType, p.lat, p.lng);

                        if (p.start == 0 || cacheHotel == 0)
                        {
                            //移除重复数据
                            hotels.RemoveAt(i);
                            break;
                        }
                        else
                        {
                            //拿首页被顶掉的酒店替换重复出现的酒店
                            ListHotelItem3 topItem = GetListHotelItem3(cacheHotel, 0);
                            hotels[i] = topItem;
                            break;
                        }
                    }
                }

                //只有第一页进去需要显示主题代表酒店;next页面 如果存在过的也要删除掉
                if (p.start == 0)
                {
                    if (hotels.Count >= p.count)
                    {
                        int cacheHotel = hotels[hotels.Count - 1].Id;
                        //移除最后一条数据
                        hotels.RemoveAt(hotels.Count - 1);

                        cacheSearchHotelInterestHotelId(p.hotelid, p.Interest, p.districtid, p.geoScopeType, p.lat, p.lng, cacheHotel);
                    }

                    ListHotelItem3 topItem = GetListHotelItem3(p.hotelid, p.Interest);
                    item3s.Add(topItem);
                    item3s.AddRange(hotels);
                }
            }
            if (item3s.Count > 0)
            {
                hotels = item3s;
            }

            if (hotels.Count > 0)
            {
                //价格数据
                if (p.maxPrice != -1)  //-1表示不需要价格数据
                {
                    GenHotelListPrice(hotels, null, arrivalTime, departureTime);
                }

                //照片 排名
                int rank = p.start + 1;

                foreach (var hotel in hotels)
                {
                    hotel.PictureList = hotel.PictureSURLList.Count == 0 ? new List<string>() { PhotoAdapter.GenHotelPicUrl(defaultPicSUrl, Enums.AppPhotoSize.theme) } :
                        hotel.PictureSURLList.Select(s => PhotoAdapter.GenHotelPicUrl(s, Enums.AppPhotoSize.theme)).ToList();

                    hotel.HotelIntro = GenHotelIntro(hotel.Id);
                }
            }

            model.Result = hotels;
            model.InterestID = p.Interest;
            model.InterestName = ResourceAdapter.GetInterestName(p.Interest);

            HotelListMenu hlm = GetHotelListMenu(p.districtid, p.geoScopeType, p.lat, p.lng, p.distance);
            model.filters = new HotelListMenu()
            {
                DistanceFilters = new List<FilterItem>(),
                PriceFilters = new List<FilterItem>(),
                SightInterestList = new List<InterestEntity>(),
                StarFilters = new List<FilterItem>(),
                ThemeInterestList = new List<InterestEntity>(),
                TotalNum = hlm.TotalNum
            };

            return model;
        }

        public static SearchHotelListResult SearchHotelList20(HotelListQueryParam20 p)
        {
            SearchHotelListResult model = new SearchHotelListResult();
            HotelSearchParas argu = TransHotelListQueryParam202HotelSearchParas(p);

            var qResult = HotelService.QueryHotel3(argu);// HotelService.SearchHotel(argu).ToList();
            var hotels = qResult.HotelList;

            if (hotels != null && hotels.Count != 0)
            {
                genHotelPicsAndPrices(p, ref hotels, argu.CheckInDate, argu.CheckOutDate);
            }

            model.Start = p.start;
            model.TotalCount = qResult.TotalCount;
            model.Result = hotels;
            model.Result20 = new List<ListHotelItemV43>();
            model.SortOptions = new List<HotelListSortOption>();

            model.FilterMenus = TransHotelListMenu(p);//网站
            model.FilterBlocks = new FilterBlockModel();
            model.FilterBlocks.TagBlocks = TransFilterCol2FilterBlock(p, argu);//p.districtid,p.interest,p.zoneId,p.geoScopeType,p.lat,p.lng);//app

            return model;
        }

        /// <summary>
        /// 酒店列表查询
        /// </summary>
        /// <param name="_contextBasicInfo"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public static SearchHotelListResult SearchHotelList30(ContextBasicInfo _contextBasicInfo, HotelListQueryParam20 p)
        {
            var model = new SearchHotelListResult();
            if (p != null)
            {
                TimeLog log = new TimeLog(string.Format("HotelAdapter SearchHotelList30:checkin{0} checkout{1} districtid{2}  ", p.checkin, p.checkout, p.districtid), 1000, null);

                HotelSearchParas argu = TransHotelListQueryParam202HotelSearchParas(p);
                var isInMap = argu.nLat != 0 || argu.n1Lat != 0 || argu.sLat != 0 || argu.s1Lat != 0 ? true : false;

                log.AddLog("HotelAdapter.TransHotelListQueryParam202HotelSearchParas");
                //查询酒店列表结果集
                var qResult = HotelService.QueryHotel3(argu);// HotelService.SearchHotel(argu).ToList();
                var hotels = qResult.HotelList;

                log.AddLog("HotelAdapter.HotelService.QueryHotel3");

                //获取所有查询酒店的列表价
                if (hotels != null && hotels.Count > 0)
                {
                    GenHotelListSlotPrice20(_contextBasicInfo.AppUserID, ref hotels, argu.CheckInDate, argu.FixDate);
                    log.AddLog("HotelAdapter.GenHotelListSlotPrice20");
                }

                model.Start = p.start;
                model.TotalCount = qResult.TotalCount;
                //model.TotalCount = hotels.Count;
                model.Result = new List<ListHotelItem3>();
                bool IsInterest = p.interest > 0 ? true : false;

                model.Result20 = hotels != null ? hotels.Select(_ => new ListHotelItemV43()
                {
                    IsInterest = IsInterest,
                    HotelStar = genHotelStarDesc(int.Parse(_.HotelStar)),
                    Score = argu.SortType != 30 ? _.Score : 0, //用于 ios android   控制app是否展示  0 不展示
                    HotelScore = _.Score,
                    ReviewCount = _.ReviewCount,
                    Currency = _.Currency,
                    DistrictName = _.DistrictName,
                    GLat = _.GLat,
                    GLon = _.GLon,
                    Id = _.Id,
                    InterestComment = "",
                    HotelIntro = "",
                    MinPrice = _.VIPPrice == 0 ? 0 : _.MinPrice,
                    VIPPrice = _.VIPPrice,
                    Name = _.Name,
                    ShortName = !string.IsNullOrWhiteSpace(_.ShortName) ? _.ShortName : _.Name,
                    PriceType = _.PriceType,
                    Rank = _.Rank,
                    Tag = "",
                    PictureSURLList = _.PictureSURLList,
                    PictureList = _.PictureList,
                    HotelRelComments = new List<HotelRelComment>(),
                    HotelRelFilterTags = new List<FilterTag>(),
                    PackageBrief = _.HotelIntro,//借用hotelIntro字段显示 套餐brief信息， ToDo同时显示hotelIntro和brief时 需要加字段
                    PackageLabel = _.Tag,
                    SaleStartTime = DateTime.MinValue.Date,
                    SaleEndTime = DateTime.MinValue.Date,
                    PackageEndDate = DateTime.MinValue.Date,
                    PackageStartDate = DateTime.MinValue.Date,
                    NightCount = _.NightCount
                }).ToList() : new List<ListHotelItemV43>();

                foreach (var hitem in model.Result20)
                {
                    try
                    {
                        if (!_contextBasicInfo.IsThanVer6_0)
                        {
                            hitem.InterestComment = GenHotelIntro(hitem.Id);
                            if (!string.IsNullOrEmpty(hitem.InterestComment)) hitem.InterestComment = FormatHotelListComment(hitem.InterestComment);
                        }

                        if (!isInMap)
                        {
                            //照片
                            hitem.PictureList = hitem.PictureSURLList.Count == 0 ? new List<string>() { PhotoAdapter.GenHotelPicUrl(defaultPicSUrl, Enums.AppPhotoSize.theme) } :
                            hitem.PictureSURLList.Select(s => PhotoAdapter.GenHotelPicUrl(s, Enums.AppPhotoSize.theme)).ToList();

                            //web格式支持
                            if (p.SupportWebP)
                            {
                                hitem.PictureList.ForEach(pic => pic = string.Format("{0}/{1}", pic, "format/webp"));
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }

                log.AddLog("HotelAdapter.model.Result20");
                #region 关联推荐套餐价格信息

                //&& false 暂不查询关联套餐价格，目前套餐中的ShowNo字段先不用了 haoy 2017-08-14
                if (!p.JustMinPricePlan && false)
                {
                    var hotelIds = hotels != null && hotels.Any() ? hotels.Select(_ => _.Id).Distinct().ToList() : new List<int>();
                    var relPackageList = GetHotelTop1PackageInfo(hotelIds, argu.CheckInDate, argu.CheckOutDate);

                    if (!isInMap)
                    {
                        model.Result20.ForEach((_) =>
                        {
                            var relPackage = relPackageList.FirstOrDefault(j => j.HotelID == _.Id);
                            if (relPackage != null)
                            {
                                _.PackageBrief = relPackage.Brief;
                                _.MinPrice = relPackage.Price;
                                _.VIPPrice = relPackage.VIPPrice;
                                _.SaleStartTime = relPackage.SaleStartTime;
                                _.SaleEndTime = DateTime.MinValue.Date;// (relPackage.SaleEndTime - DateTime.Now).TotalDays >= 7 ? DateTime.MinValue.Date : relPackage.SaleEndTime;//倒计时在7天才出现
                                _.PackageStartDate = relPackage.StartDate;
                                _.PackageEndDate = relPackage.EndDate;
                                _.PackageLabel = "优惠套餐";
                                _.PackageId = relPackage.PID;
                            }

                        });
                    }
                    else if (relPackageList.Any())
                    {
                        model.Result20.ForEach((_) =>
                        {
                            var relPackage = relPackageList.FirstOrDefault(j => j.HotelID == _.Id);
                            if (relPackage != null)
                            {
                                _.PackageBrief = relPackage.Brief;
                                _.MinPrice = relPackage.Price;
                                _.VIPPrice = relPackage.VIPPrice;
                                _.PackageLabel = "优惠套餐";
                                _.PackageId = relPackage.PID;
                            }
                        });
                    }
                }
                log.AddLog("HotelAdapter.model.关联推荐套餐价格信息");
                #endregion

                log.Finish();
            }
            return model;
        }

        private static List<FilterTag> GenerateRightFilterTag(IEnumerable<FilterTag> searchFilterTags, IEnumerable<FilterTag> hotelFilterTags)
        {
            var result = new List<FilterTag>();
            if (searchFilterTags == null || !searchFilterTags.Any())
            {
                return result;
            }
            else if (hotelFilterTags == null || !hotelFilterTags.Any())
            {
                return searchFilterTags.ToList();
            }
            else
            {
                foreach (var filterTag in searchFilterTags)
                {
                    int blockTagId = filterTag.BlockCategoryID;
                    string tagValue = filterTag.Value;
                    if (hotelFilterTags.Any(_ => _.BlockCategoryID == blockTagId && _.Value == tagValue))
                    {
                        filterTag.IsMatch = true;
                        result.Add(filterTag);
                    }
                }
            }
            return result;
        }

        private static string genHotelStarDesc(int hotelStar)
        {
            switch (hotelStar)
            {
                case 5:
                    return "五星及以上(豪华)";
                case 4:
                    return "四星(高档)";
                case 0:
                    return "";
                default:
                    return "三星及以下(舒适)";
            }
        }

        private static void genHotelPicsAndPrices(HotelListQueryParam20 p, ref List<ListHotelItem3> hotels, DateTime checkIn, DateTime checkOut)
        {
            //需要获取数据 start传的参数
            List<ListHotelItem3> item3s = new List<ListHotelItem3>();
            //审核过滤条件
            if (p.hotelId > 0 && p.interest > 0 && (p.FilterTags == null || p.FilterTags.Count == 0))
            {
                for (int i = hotels.Count - 1; i >= 0; i--)
                {
                    int hotelID = hotels[i].Id;
                    if (hotelID == p.hotelId)
                    {
                        int cacheHotel = GetSearchHotelInterestCacheHotelId(p.hotelId, p.interest, p.districtid, p.geoScopeType, p.lat, p.lng);

                        if (p.start == 0 || cacheHotel == 0)
                        {
                            //移除重复数据
                            hotels.RemoveAt(i);
                            break;
                        }
                        else
                        {
                            //拿首页被顶掉的酒店替换重复出现的酒店
                            ListHotelItem3 topItem = GetListHotelItem3(cacheHotel, 0);
                            hotels[i] = topItem;
                            break;
                        }
                    }
                }

                //只有第一页进去需要显示主题代表酒店;next页面 如果存在过的也要删除掉
                if (p.start == 0)
                {
                    if (hotels.Count >= p.count)
                    {
                        int cacheHotel = hotels[hotels.Count - 1].Id;
                        //移除最后一条数据
                        hotels.RemoveAt(hotels.Count - 1);

                        cacheSearchHotelInterestHotelId(p.hotelId, p.interest, p.districtid, p.geoScopeType, p.lat, p.lng, cacheHotel);
                    }

                    ListHotelItem3 topItem = GetListHotelItem3(p.hotelId, p.interest);
                    item3s.Add(topItem);
                    item3s.AddRange(hotels);
                }
            }
            if (item3s.Count > 0)
            {
                hotels = item3s;
            }

            if (hotels.Count > 0)
            {
                //价格数据
                if (p.maxPrice != -1)  //-1表示不需要价格数据
                {
                    //GenHotelListPrice(hotels, null, checkIn, checkOut);
                    GenHotelListSlotPrice(hotels, null, checkIn, checkOut, p.minPrice, p.maxPrice);
                }

                //照片 排名
                //int rank = p.start + 1;

                foreach (var hotel in hotels)
                {
                    hotel.PictureList = hotel.PictureSURLList.Count == 0 ? new List<string>() { PhotoAdapter.GenHotelPicUrl(defaultPicSUrl, Enums.AppPhotoSize.theme) } :
                        hotel.PictureSURLList.Select(s => PhotoAdapter.GenHotelPicUrl(s, Enums.AppPhotoSize.theme)).ToList();

                    hotel.HotelIntro = GenHotelIntro(hotel.Id);
                }
            }
        }

        /// <summary>
        /// 使用hotelIntro字段记录Brief字段信息
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="hotels"></param>
        /// <param name="arrivalTime"></param>
        /// <param name="fixDate">是否指定列表价日期</param>
        private static void GenHotelListSlotPrice20(long userId, ref List<ListHotelItem3> hotels, DateTime arrivalTime, bool fixDate)
        {
            if (hotels != null && hotels.Any())
            {
                //酒店列表集合
                var newHotels = new List<ListHotelItem3>();

                //暂存没有列表价的酒店ID列表
                var noPriceHotelIds = new List<int>();

                //根据是否指定入住日期，设置不同的列表价查询起始日期(默认CommMethods.GetDefaultCheckIn()明天)
                arrivalTime = fixDate ? arrivalTime : CommMethods.GetDefaultCheckIn();

                //select出所有酒店ID列表
                var hotelIds = hotels.Select(_ => _.Id).ToList();

                //获取当前用户遵循的列表价类型
                var minPriceTypes = PriceAdapter.GetHotelMinPriceTypeByUser(userId);

                //获取HotelMinPrice列表价
                var hotelMinPriceList = PriceService.GetHotelMinPriceList(hotelIds, arrivalTime) ?? new List<HotelMinPriceEntity>();
                var pricePlanList = new List<PricePlanEx>();

                //if (hotelMinPriceList == null || hotelMinPriceList.Count == 0)
                {
                    //获取PricePlan列表价
                    pricePlanList = PriceService.GetPricePlanExList(hotelIds, arrivalTime) ?? new List<PricePlanEx>();
                }

                ////查询列表价
                //var pricePlanList = PriceService.GetPricePlanExList(hotelIds, arrivalTime) ?? new List<PricePlanEx>();

                if ((pricePlanList != null && pricePlanList.Any()) || (hotelMinPriceList != null && hotelMinPriceList.Any()))
                {
                    //遍历所有酒店，赋值列表价
                    foreach (var h in hotels)
                    {
                        h.MinPrice = 0;
                        h.VIPPrice = 0;
                        h.HotelIntro = "";
                        h.PriceType = 0;
                        h.Tag = "";

                        //当前酒店的列表价
                        var pricePlan = PriceAdapter.GetHotelPricePlan(h.Id, pricePlanList, hotelMinPriceList, minPriceTypes, arrivalTime, fixDate);

                        //set hotel price info
                        if (pricePlan.Price > 0)
                        {
                            h.MinPrice = pricePlan.Price;
                            h.VIPPrice = pricePlan.VipPrice;
                            h.HotelIntro = pricePlan.Name + pricePlan.Brief;
                            h.PriceType = pricePlan.ChannelID == 100 ? 3 : 2;
                            h.Tag = pricePlan.ChannelID == 100 ? "优惠套餐" : "";
                            h.NightCount = pricePlan.NightCount;

                            //只显示列表价大于0的酒店
                            newHotels.Add(h);
                        }
                        else
                        {
                            noPriceHotelIds.Add(h.Id);
                        }
                    }
                }

                ////yyb 20170117 拿掉vip价格为0 的酒店
                //对没有拿到列表价的酒店
                if (noPriceHotelIds != null && noPriceHotelIds.Count > 0)
                {
                    var hotelBasePriceList = PriceAdapter.QueryHotelBasePrice(noPriceHotelIds).FindAll(_ => _.MinPrice > 0);
                    if (hotelBasePriceList != null && hotelBasePriceList.Any())
                    {
                        foreach (var basePrice in hotelBasePriceList)
                        {
                            var item = hotels.Find(_ => _.Id == basePrice.HotelId);
                            if (item != null && item.MinPrice <= 0 && basePrice.MinPrice > 0)
                            {
                                item.MinPrice = basePrice.MinPrice;
                                item.VIPPrice = 0;
                                item.HotelIntro = "";
                                item.PriceType = 0;
                                item.Tag = "";

                                //补充了酒店的basePrice以后，将该酒店放入酒店列表
                                newHotels.Add(item);
                            }
                        }
                    }
                }

                //reset hotels
                hotels = newHotels;
            }
        }

        public static HotelSearchParas TransHotelListQueryParam202HotelSearchParas(HotelListQueryParam20 param20)
        {
            var isInMap = param20.Top_Left_Lat != 0 || param20.Top_Right_Lat != 0 || param20.Bottom_Left_Lat != 0 || param20.Bottom_Right_Lat != 0 ? true : false;

            //从攻略页面进来 限定是指定地区的意思
            param20.geoScopeType = param20.zoneId > 0 ? 1 : param20.geoScopeType;
            //从攻略页面进来 如果没有带districtID 则取一次
            param20.districtid = param20.zoneId > 0 && param20.districtid == 0 ? destService.GetDistrictZoneID(param20.zoneId).DistrictID : param20.districtid;

            if (param20.districtid == 0 && param20.geoScopeType == 1 && !string.IsNullOrWhiteSpace(param20.districtName))
            {
                var districtNameParam = param20.districtName;
                districtNameParam = districtNameParam.StartsWith("%")
                    ? HttpUtility.UrlDecode(districtNameParam)
                    : districtNameParam;
                var targetCity =
                    GetZMJDCityData2().Citys.FirstOrDefault(_ => _.Name.Trim() == districtNameParam ||
                                                                 _.EName.Trim() == districtNameParam);
                param20.districtid = targetCity != null ? targetCity.ID : 0;
            }

            //从首页城市列表 跳转过来的使用aroundcityid
            if (param20.districtid == 0 && param20.geoScopeType == 3 && !string.IsNullOrWhiteSpace(param20.districtName) && param20.interest == 0)
            {
                var districtNameParam = param20.districtName;
                districtNameParam = districtNameParam.StartsWith("%")
                    ? HttpUtility.UrlDecode(districtNameParam)
                    : districtNameParam;
                var targetCity =
                    GetZMJDCityData2().Citys.FirstOrDefault(_ => _.Name.Trim() == districtNameParam ||
                                                                 _.EName.Trim() == districtNameParam);
                param20.aroundCityId = targetCity != null ? targetCity.ID : 0;//aroundCityId 可以作为firstfilter
            }

            //根据传入参数 转为搜索参数
            HotelSearchParas argu = new HotelSearchParas();

            //基本参数
            DateTime checkIn = CommMethods.GetDefaultCheckIn();
            DateTime checkOut = checkIn.AddDays(1);
            if (string.IsNullOrWhiteSpace(param20.checkin) || param20.checkin == "0")
            {
                argu.CheckInDate = checkIn;
                argu.CheckOutDate = checkOut;
            }
            else
            {
                argu.CheckInDate = DateTime.Parse(param20.checkin);
                argu.CheckOutDate = DateTime.Parse(param20.checkout);
                if (argu.CheckInDate >= argu.CheckOutDate)
                {
                    argu.CheckOutDate = argu.CheckInDate.AddDays(1);
                }
            }

            //默认不指定日期
            argu.FixDate = false;

            #region 四个点参数
            argu.nLat = param20.Top_Left_Lat;
            argu.nLng = param20.Top_Left_Lng;
            argu.n1Lat = param20.Top_Right_Lat;
            argu.n1Lng = param20.Top_Right_Lng;

            argu.sLat = param20.Bottom_Left_Lat;
            argu.sLng = param20.Bottom_Left_Lng;
            argu.s1Lat = param20.Bottom_Right_Lat;
            argu.s1Lng = param20.Bottom_Right_Lng;
            #endregion

            argu.StartIndex = param20.start;
            argu.AroundCityID = param20.aroundCityId;
            argu.DistrictID = param20.districtid;
            argu.ReturnCount = param20.count;
            argu.NeedFilterCol = param20.IsNeedFilterCol;//搜索酒店列表 根据查询是否有酒店列表来判断
            argu.NeedHotelID = param20.IsNeedHotelList;//需要返回酒店列表酒店ID
            argu.Lat = param20.lat;
            argu.Lng = param20.lng;
            argu.Distance = param20.geoScopeType == 1 ? 0 : param20.distance;
            argu.Interest = param20.interest;
            argu.HotelState = param20.interest > 0 ? new int[] { 2 } : null;

            //内部搜出interest跟地区组合的玩点数据 以及不同区域范围默认排序设置
            //2015-09-10 设置限制 必须是主题不为0 才需要生成interplace过滤条件
            if (param20.interest != 0 && !isInMap)
            {
                argu.InterestPlace = GenInterestPlaceFromDistrictAndInterest(param20);
            }
            else
            {
                argu.InterestPlace = null;
            }

            //排序方式：0：默认 1：口碑 2：距离 3：价格正序 4：价格倒序   6：按所有玩点口碑 10:套餐先排
            switch (param20.sort)
            {
                //如果存在interest则默认按6排序;如果不是interest则按0排序
                case 0://0：默认
                    argu.SortType = 10;//param20.interest > 0 ? 10 : 0;//把10改成6 再改成6和0 再改回10和0（2016.03.31）;
                    argu.SortDirection = 0;
                    break;
                case 1://1：口碑
                    argu.SortType = 0;
                    argu.SortDirection = 0;
                    break;
                case 6://按所有玩点口碑
                    argu.SortType = 6;
                    argu.SortDirection = 0;
                    break;
                case 2://2：距离
                    argu.SortType = 2;
                    argu.SortDirection = 0;
                    break;
                case 3://3：价格正序
                    argu.SortType = 1;
                    argu.SortDirection = 0;
                    break;
                case 4://4：价格倒序
                    argu.SortType = 1;
                    argu.SortDirection = 1;
                    break;
                case 20://20：按标签匹配度降序(查找页默认跳转排序)
                    argu.SortType = 20;
                    argu.SortDirection = 1;
                    break;
                case 30:
                    argu.SortType = 30;
                    argu.SortDirection = 1;
                    break;
            }

            //如果在南半球 或 在美国，那么只按排名排序
            if (argu.Lng < 0 || argu.Lat < 0)
            {
                argu.SortType = 0;
            }

            //酒店星级筛选
            int star = 0;
            if (string.IsNullOrWhiteSpace(param20.star) || param20.star.Contains("0"))
            {
            }
            else
            {
                var starList = new List<int>();
                foreach (var starItem in param20.star.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
                {
                    int.TryParse(starItem, out star);
                    starList.AddRange(HotelHelper.TransStarArgu(star)); //星级查询反馈
                }
                argu.Star = starList.ToArray();
            }

            argu.ZonePlaceID = param20.zoneId;//区域ID 自己画的圈

            argu.MinPrice = param20.minPrice;
            argu.MaxPrice = param20.minPrice != 0 && param20.maxPrice == 0 ? 100000 : param20.maxPrice;//最小值不为0 而最大值为0 表示大于minPrice的价格（maxprice传个大的值）

            //转化为hotelfilter参数
            if (param20.FilterTags != null && param20.FilterTags.Count > 0)
            {
                argu.Class2ID = param20.FilterTags.FindAll(_ => _.BlockCategoryID == 3).Select(_ => int.Parse(_.Value)).ToArray();//酒店类型 2015-09-07 改用Class2ID
                argu.HotelFacilitys = param20.FilterTags.FindAll(_ => _.BlockCategoryID == 4).Select(_ => int.Parse(_.Value)).ToArray();//酒店设施
                argu.ZonePlaces = param20.FilterTags.FindAll(_ => _.BlockCategoryID == 1).Select(_ => int.Parse(_.Value)).ToArray();//酒店区域列表
                argu.InterestArray = param20.FilterTags.FindAll(_ => _.BlockCategoryID == 2).Select(_ => int.Parse(_.Value)).ToArray();//酒店筛选主题特色

                argu.TripTypeArray = param20.FilterTags.FindAll(_ => _.BlockCategoryID == 8).Select(_ => int.Parse(_.Value)).ToArray();//出游类型
                argu.FeaturedTreeArray = param20.FilterTags.FindAll(_ => _.BlockCategoryID == 9).Select(_ => int.Parse(_.Value)).ToArray();//其他标签

                //筛选标签条件里有酒店星级的数据 如果选了不限 那么不管 选了多个那么把多个组合起来
                var starTags = param20.FilterTags.FindAll(_ => _.BlockCategoryID == 7);
                if (starTags != null && starTags.Any())
                {
                    if (!starTags.Exists(_ => _.Value.Equals("0")))
                    {
                        List<int> stars = new List<int>();
                        foreach (var starTag in starTags)
                        {
                            stars.AddRange(HotelHelper.TransStarArgu(Int32.Parse(starTag.Value)));
                        }
                        argu.Star = stars.ToArray();
                    }
                }

                //如果筛选条件里有价格数据
                FilterTag ftPrice = param20.FilterTags.FindAll(_ => _.BlockCategoryID == 5).FirstOrDefault();
                if (ftPrice != null && ftPrice.BlockCategoryID > 0)
                {
                    var priceArray = ftPrice.Value.Split(",".ToCharArray());
                    argu.MinPrice = int.Parse(priceArray[0]);//价格下限
                    argu.MaxPrice = int.Parse(priceArray[1]);//价格上限
                    argu.MaxPrice = argu.MinPrice != 0 && argu.MaxPrice == 0 ? 100000 : argu.MaxPrice;//最小值不为0 而最大值为0 表示大于minPrice的价格（maxprice传个大的值）
                }

                //如果筛选条件里有日期数据
                FilterTag ftDate = param20.FilterTags.FindAll(_ => _.BlockCategoryID == 6).FirstOrDefault();
                if (ftDate != null && ftDate.BlockCategoryID > 0)
                {
                    var dateArray = ftDate.Value.Split(",".ToCharArray());
                    try
                    {
                        DateTime startDate = DateTime.Parse(dateArray[0]);//日期下限
                        DateTime endDate = DateTime.Parse(dateArray[1]);//日期上限
                        argu.CheckInDate = startDate;
                        argu.CheckOutDate = endDate;

                        argu.FixDate = true;
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }

            return argu;
        }

        /// <summary>
        /// 处理主题ID参数 算出interestPlace
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private static int[] GenInterestPlaceFromDistrictAndInterest(HotelListQueryParam20 p)
        {
            string interestPlace = "";
            int arguInterest = p.interest == 0 ? (int)EnumHelper.InterestID.AllHotelInterest : p.interest;

            if (arguInterest == (int)EnumHelper.InterestID.SpecialDealHotelInterest)//如果是特惠精选，那么需要特殊的解析 InterestPlacIDs的方式
            {
                if (interestPlace == null || interestPlace.Length == 0)
                {
                    List<string> ipids = ResourceAdapter.GetSpecialDealInterestPlaceIDs(p.districtid, p.lat, p.lng, p.geoScopeType);
                    interestPlace = string.Join(",", ipids);
                }
            }

            if (interestPlace == null || interestPlace.Length == 0)
            {
                switch (p.geoScopeType)
                {
                    case 1: //按目的地查
                        interestPlace = string.Join(",", GetInterestPlaceIDs(p.interest, p.districtid, 0, 0, p.distance == 0 ? defaultDistance : p.distance));
                        break;
                    case 2://查城市周边
                        HJD.DestServices.Contract.DistrictInfoEntity di = destService.GetDistrictInfo(new List<int>() { p.districtid }).First();
                        interestPlace = string.Join(",", GetInterestPlaceIDs(p.interest, 0, di.lat, di.lon, p.distance == 0 ? defaultDistance : p.distance));
                        if (p.sort == 1) //按口碑排
                            p.sort = 6; //按所有玩点的口碑排序
                        break;
                    case 3://用户坐标附近
                        interestPlace = string.Join(",", GetInterestPlaceIDs(p.interest, 0, p.lat, p.lng, p.distance == 0 ? defaultDistance : p.distance));
                        if (p.sort == 1) //按口碑排
                            p.sort = 6; //按所有玩点的口碑排序
                        break;
                }
            }

            if (interestPlace != null && interestPlace.Length > 0)
            {
                return interestPlace.Split(',').Select(i => int.Parse(i)).ToArray();
            }
            else if (p.districtid > 0)
            {
                InterestModel im = ResourceAdapter.QueryInterest2(p.districtid, 0, 0, 0);
                if (im.InterestList.Where(i => i.ID == 11).Count() > 0)
                {
                    return im.InterestList.Where(i => i.ID == 11).First().InterestPlaceIDs.Split(',').Select(i => int.Parse(i)).ToArray();
                }
                else
                {
                    return new int[0];
                }
            }
            else
            {
                return new int[0];
            }
        }

        private static HotelListMenu TransHotelListMenu(HotelListQueryParam20 p)
        {
            HotelListMenu hlm = new HotelListMenu() { };
            //请求头判断是否App
            if (HttpContext.Current.Request.Headers != null && HttpContext.Current.Request.Headers["apptype"] == null)
            {
                hlm = GetHotelListMenu(p.districtid, p.geoScopeType, p.lat, p.lng, p.distance);
            }
            else
            {
                hlm.InitMenu();
            }
            return hlm;
        }

        private static List<FilterTagBlock> TransFilterCol2FilterBlock(HotelListQueryParam20 p, HotelSearchParas argu)//int districtid, int interest, int zoneID, int geoScopeType, double lat, double lon, int distance = 300000)
        {
            List<FilterTagBlock> filterFlags = new List<FilterTagBlock>();
            if (HttpContext.Current.Request.Headers != null && HttpContext.Current.Request.Headers["apptype"] != null)
            {
                int showType = argu.Interest > 0 ? 1 : argu.ZonePlaceID > 0 ? 2 : 0;
                filterFlags = InitFilterTagBlock();//日期 价格 靠近哪里 主题特色 酒店类型(出游类型) 设施服务
                //取数据filterCol
                QueryHotelResult3 qResult = HotelService.QueryHotel3(new HotelSearchParas()
                {
                    DistrictID = argu.DistrictID,
                    AroundCityID = argu.AroundCityID,
                    Lat = argu.Lat,
                    Lng = argu.Lng,
                    Distance = argu.Distance,
                    NeedFilterCol = true,
                    NeedHotelID = false,
                    Interest = argu.Interest,
                    HotelState = argu.HotelState,
                    ZonePlaceID = argu.ZonePlaceID,
                    CheckInDate = p.interest > 0 || p.zoneId > 0 ? CommMethods.GetDefaultCheckIn() : argu.CheckInDate,
                    CheckOutDate = p.interest > 0 || p.zoneId > 0 ? CommMethods.GetDefaultCheckIn().AddDays(1) : argu.CheckOutDate,
                    InterestPlace = argu.InterestPlace,
                    MaxPrice = p.interest > 0 || p.zoneId > 0 ? 0 : argu.MaxPrice,//注意首页或者攻略页面进入酒店列表 价格只作为筛选条件 不改变初次进入酒店列表时的目录内容
                    MinPrice = p.interest > 0 || p.zoneId > 0 ? 0 : argu.MinPrice//注意首页或者攻略页面进入酒店列表 价格只作为筛选条件 不改变初次进入酒店列表时的目录内容
                });

                //转换到filterTag
                Dictionary<long, int> dicFilterResult = qResult.FilterCount;
                if (dicFilterResult != null && dicFilterResult.Count > 0)
                {
                    long baseOffSet = HJDAPI.Common.Helpers.Struct.HotelListFilterPrefix.BaseOffset;
                    long themeKey = HJDAPI.Common.Helpers.Struct.HotelListFilterPrefix.Interest;
                    long zonePlaceKey = HJDAPI.Common.Helpers.Struct.HotelListFilterPrefix.ZonePlace;
                    long classKey = HJDAPI.Common.Helpers.Struct.HotelListFilterPrefix.Class2;
                    long hotelFacilityKey = HJDAPI.Common.Helpers.Struct.HotelListFilterPrefix.HotelFacility;
                    //long featuredTreeKey = HJDAPI.Common.Helpers.Struct.HotelListFilterPrefix.FeaturedTree;

                    string themePrefix = (themeKey / baseOffSet).ToString();
                    string zonePlacePrefix = (zonePlaceKey / baseOffSet).ToString();
                    string classPrefix = (classKey / baseOffSet).ToString();
                    string hotelFacilityPrefix = (hotelFacilityKey / baseOffSet).ToString();
                    //string featuredTreeKeyPrefix = (featuredTreeKey / baseOffSet).ToString();

                    List<int> zoneids = new List<int>();
                    List<int> interestids = new List<int>();
                    List<int> classids = new List<int>();
                    List<int> facilitys = new List<int>();
                    //List<int> featuredTrees = new List<int>();

                    foreach (var filterKey in dicFilterResult.Keys)
                    {
                        string keyStr = filterKey.ToString();
                        //靠近哪里
                        if (keyStr.StartsWith(zonePlacePrefix))
                        {
                            int zone = (int)(filterKey - zonePlaceKey);
                            zoneids.Add(zone);
                        }
                        //主题
                        else if (keyStr.StartsWith(themePrefix) && showType != 1)
                        {
                            int interestid = (int)(filterKey - themeKey);
                            interestids.Add(interestid);
                        }
                        //酒店类型
                        else if (keyStr.StartsWith(classPrefix))
                        {
                            int classid = (int)(filterKey - classKey);
                            classids.Add(classid);
                        }
                        //设施
                        else if (keyStr.StartsWith(hotelFacilityPrefix))
                        {
                            int facility = (int)(filterKey - hotelFacilityKey);
                            facilitys.Add(facility);
                        }
                        //查询标签
                        //else if (keyStr.StartsWith(featuredTreeKeyPrefix))
                        //{
                        //    int featuredTree = (int)(filterKey - featuredTreeKey);
                        //    featuredTrees.Add(featuredTree);
                        //}
                    }

                    List<FilterDicEntity> filterTagInfos =
                        GetHotelListFilterTagInfos(new SearchHotelListFilterTagInfoParam()
                        {
                            classids = classids,
                            interestids = interestids,
                            zoneids = zoneids,
                            facilitys = facilitys
                        });
                    //更新名称数据
                    if (filterTagInfos != null && filterTagInfos.Count > 0)
                    {
                        filterFlags[2].Tags = filterTagInfos.FindAll(_ => _.Type == 18 && !string.IsNullOrWhiteSpace(_.Name)).Select(_ => new FilterTag() { BlockCategoryID = 1, Value = _.ID.ToString(), Name = _.Name }).ToList();
                        filterFlags[2].Tags = p.zoneId > 0 && filterFlags[2].Tags.Count <= 1 ? new List<FilterTag>() : filterFlags[2].Tags;//攻略页面进来 只有一个区域 无需显示

                        //非主题页进来 才显示主题选项
                        if (showType != 1)
                        {
                            //2015-09-07 要求按照主题特色相关酒店数量降序排列
                            var interestFilters = filterTagInfos.FindAll(_ => _.Type == 16 && !string.IsNullOrWhiteSpace(_.Name));
                            if (interestFilters != null && interestFilters.Count != 0)
                            {
                                interestFilters = interestFilters.Select(_ =>
                                    new
                                    {
                                        ID = _.ID,
                                        Name = _.Name,
                                        Key = _.Key,
                                        Num = _.Num,
                                        Type = _.Type,
                                        themeHotelCount = dicFilterResult[_.Key]
                                    }).OrderByDescending(_ =>
                                            _.themeHotelCount).Select(_ =>
                                                new FilterDicEntity()
                                {
                                    ID = _.ID,
                                    Key = _.Key,
                                    Name = _.Name,
                                    Num = _.Num,
                                    Type = _.Type
                                }).ToList();
                            }

                            filterFlags[3].Tags = interestFilters.Select(_ => new FilterTag() { BlockCategoryID = 2, Value = _.ID.ToString(), Name = _.Name }).ToList();
                            filterFlags[3].Tags = p.interest > 0 && filterFlags[3].Tags.Count <= 1 ? new List<FilterTag>() : filterFlags[3].Tags;//主题页面进入 只有一个主题 也无需显示
                        }

                        //首页和攻略页面进来 非搜索页才显示日期范围和价格范围
                        if (showType != 0)
                        {
                            filterFlags[0].Tags.Add(new FilterTag()
                            {
                                BlockCategoryID = 6,
                                Value = "0,0",
                                Name = "不限日期"
                            });
                            filterFlags[1].Tags.Add(new FilterTag()
                            {
                                BlockCategoryID = 5,
                                Value = "0,0",
                                Name = "不限价格"
                            });
                        }
                        filterFlags[4].Tags = filterTagInfos.FindAll(_ => _.Type == 21 && !string.IsNullOrWhiteSpace(_.Name)).Select(_ => new FilterTag() { BlockCategoryID = 3, Value = _.ID.ToString(), Name = _.Name }).ToList();
                        filterFlags[5].Tags = filterTagInfos.FindAll(_ => _.Type == 19 && !string.IsNullOrWhiteSpace(_.Name)).Select(_ => new FilterTag() { BlockCategoryID = 4, Value = _.ID.ToString(), Name = _.Name }).ToList();
                    }
                }
            }
            return filterFlags;
        }

        public static List<FilterTagBlock> TransFilterCol2FilterTagBlock(HotelSearchParas argu)
        {
            List<FilterTagBlock> filterFlags = new List<FilterTagBlock>();

            int showType = argu.Interest > 0 ? 1 : argu.ZonePlaceID > 0 ? 2 : 0;//查找页面默认0开始

            filterFlags = InitFilterTagBlock20();
            //取数据filterCol
            QueryHotelResult3 qResult = HotelService.QueryHotel3(new HotelSearchParas()
            {
                DistrictID = argu.DistrictID,
                AroundCityID = argu.AroundCityID,
                Lat = argu.Lat,
                Lng = argu.Lng,
                Distance = argu.Distance,
                NeedFilterCol = true,
                NeedHotelID = false,
                Interest = argu.Interest,
                HotelState = argu.HotelState,
                ZonePlaceID = argu.ZonePlaceID,
                CheckInDate = argu.CheckInDate,
                CheckOutDate = argu.CheckOutDate,
                InterestPlace = argu.InterestPlace,
                MaxPrice = argu.MaxPrice,
                MinPrice = argu.MinPrice
            });

            //转换到filterTag
            Dictionary<long, int> dicFilterResult = qResult.FilterCount;
            if (dicFilterResult != null && dicFilterResult.Count > 0)
            {
                long baseOffSet = HJDAPI.Common.Helpers.Struct.HotelListFilterPrefix.BaseOffset;//基本系数
                long themeKey = HJDAPI.Common.Helpers.Struct.HotelListFilterPrefix.Interest;//主题特色
                long zonePlaceKey = HJDAPI.Common.Helpers.Struct.HotelListFilterPrefix.ZonePlace;//区域ID(位置)
                long classKey = HJDAPI.Common.Helpers.Struct.HotelListFilterPrefix.Class2;//酒店类型
                long hotelFacilityKey = HJDAPI.Common.Helpers.Struct.HotelListFilterPrefix.HotelFacility;//设施服务
                long starKey = HJDAPI.Common.Helpers.Struct.HotelListFilterPrefix.Star;//酒店星级
                long tripTypeKey = HJDAPI.Common.Helpers.Struct.HotelListFilterPrefix.TripType;//出游类型
                long featuredTreeKey = HJDAPI.Common.Helpers.Struct.HotelListFilterPrefix.FeaturedTree;

                string themePrefix = (themeKey / baseOffSet).ToString();//前缀
                string zonePlacePrefix = (zonePlaceKey / baseOffSet).ToString();
                string classPrefix = (classKey / baseOffSet).ToString();
                string hotelFacilityPrefix = (hotelFacilityKey / baseOffSet).ToString();
                string starPrefix = (starKey / baseOffSet).ToString();
                string tripTypePrefix = (tripTypeKey / baseOffSet).ToString();
                string featuredTreeKeyPrefix = (featuredTreeKey / baseOffSet).ToString();

                List<int> zoneids = new List<int>();
                List<int> interestids = new List<int>();
                List<int> classids = new List<int>();
                List<int> facilitys = new List<int>();
                List<int> starids = new List<int>();//酒店星级id集合
                List<int> tripTypeids = new List<int>();//出游类型id集合
                List<int> featuredTrees = new List<int>();

                foreach (var filterKey in dicFilterResult.Keys)
                {
                    string keyStr = filterKey.ToString();
                    //靠近哪里（位置）
                    if (keyStr.StartsWith(zonePlacePrefix))
                    {
                        int zone = (int)(filterKey - zonePlaceKey);
                        zoneids.Add(zone);
                    }
                    //主题特色
                    else if (keyStr.StartsWith(themePrefix) && showType != 1)
                    {
                        int interestid = (int)(filterKey - themeKey);
                        interestids.Add(interestid);
                    }
                    //酒店类型
                    else if (keyStr.StartsWith(classPrefix))
                    {
                        int classid = (int)(filterKey - classKey);
                        classids.Add(classid);
                    }
                    //设施服务
                    else if (keyStr.StartsWith(hotelFacilityPrefix))
                    {
                        int facility = (int)(filterKey - hotelFacilityKey);
                        facilitys.Add(facility);
                    }
                    //酒店星级
                    else if (keyStr.StartsWith(starPrefix))
                    {
                        int star = (int)(filterKey - starKey);
                        starids.Add(star);
                    }
                    //出游类型
                    else if (keyStr.StartsWith(tripTypePrefix))
                    {
                        int tripType = (int)(filterKey - tripTypeKey);
                        tripTypeids.Add(tripType);
                    }
                    //查询标签
                    else if (keyStr.StartsWith(featuredTreeKeyPrefix))
                    {
                        int featuredTree = (int)(filterKey - featuredTreeKey);
                        featuredTrees.Add(featuredTree);
                    }
                }

                List<FilterDicEntity> filterTagInfos = GetHotelListFilterTagInfos(new SearchHotelListFilterTagInfoParam() { classids = classids, interestids = interestids, zoneids = zoneids, facilitys = facilitys, triptypeids = tripTypeids, featuredtreeids = featuredTrees });
                //更新名称数据
                if (filterTagInfos != null && filterTagInfos.Count > 0)
                {
                    var zoneIndex = filterFlags.FindIndex(_ => _.BlockCategoryID == 1);//区域
                    var dateIndex = filterFlags.FindIndex(_ => _.BlockCategoryID == 6);//日期
                    var priceIndex = filterFlags.FindIndex(_ => _.BlockCategoryID == 5);//价格
                    var starIndex = filterFlags.FindIndex(_ => _.BlockCategoryID == 7);//酒店星级
                    var interestIndex = filterFlags.FindIndex(_ => _.BlockCategoryID == 2);//主题特色
                    var tripIndex = filterFlags.FindIndex(_ => _.BlockCategoryID == 8);//出游类型
                    var hotelIndex = filterFlags.FindIndex(_ => _.BlockCategoryID == 3);//酒店类型
                    var facilityIndex = filterFlags.FindIndex(_ => _.BlockCategoryID == 4);//设施服务
                    var otherIndex = filterFlags.FindIndex(_ => _.BlockCategoryID == 9);//设施服务

                    //区域筛选项
                    var zoneFilterFlag = filterFlags[zoneIndex];
                    zoneFilterFlag.Tags = filterTagInfos.FindAll(_ => _.Type == 18 && !string.IsNullOrWhiteSpace(_.Name)).Select(_ => new FilterTag() { BlockCategoryID = 1, Value = _.ID.ToString(), Name = _.Name, HotelCount = dicFilterResult[_.Key] }).ToList();
                    zoneFilterFlag.Tags = showType == 2 ? new List<FilterTag>() : zoneFilterFlag.Tags;//攻略页面进来 只有一个区域 无需显示

                    //非主题页进来 才显示主题筛选项
                    if (showType != 1)
                    {
                        //2015-09-07 要求按照主题特色相关酒店数量降序排列
                        var interestFilters = filterTagInfos.FindAll(_ => _.Type == 16 && !string.IsNullOrWhiteSpace(_.Name));
                        if (interestFilters != null && interestFilters.Count != 0)
                        {
                            interestFilters = interestFilters.Select(_ =>
                                new
                                {
                                    ID = _.ID,
                                    Name = _.Name,
                                    Key = _.Key,
                                    Num = _.Num,
                                    Type = _.Type,
                                    themeHotelCount = dicFilterResult[_.Key]
                                }).OrderByDescending(_ =>
                                        _.themeHotelCount).Select(_ =>
                                            new FilterDicEntity()
                                            {
                                                ID = _.ID,
                                                Key = _.Key,
                                                Name = _.Name,
                                                Num = _.Num,
                                                Type = _.Type
                                            }).ToList();
                        }

                        var interestFilterFlag = filterFlags[interestIndex];
                        interestFilterFlag.Tags = interestFilters.Select(_ => new FilterTag() { BlockCategoryID = 2, Value = _.ID.ToString(), Name = _.Name, HotelCount = dicFilterResult[_.Key] }).ToList();
                    }

                    //首页和攻略页面进来 非搜索页才显示日期范围和价格范围
                    //V4.6 所有页面进来都可以看到日期 价格筛选项
                    if (true || showType != 0)
                    {
                        filterFlags[dateIndex].Tags.Add(new FilterTag()
                        {
                            BlockCategoryID = 6,
                            Value = "0,0",
                            Name = "不限日期"
                        });

                        filterFlags[priceIndex].Tags.Add(new FilterTag()
                        {
                            BlockCategoryID = 5,
                            Value = "0,0",
                            Name = "不限价格"
                        });

                        filterFlags[starIndex].Tags = genHotelStarFilterTags(starids); //获取酒店星级标签块
                        filterFlags[tripIndex].Tags =
                            filterTagInfos.FindAll(_ => _.Type == 22 && !string.IsNullOrWhiteSpace(_.Name))
                                .Select(
                                    _ => new FilterTag() { BlockCategoryID = 8, Value = _.ID.ToString(), Name = _.Name, HotelCount = dicFilterResult[_.Key] })
                                .ToList();
                    }
                    //else
                    //{
                    //    filterFlags[otherIndex].Tags = filterTagInfos.FindAll(_ => _.Type == 23 && !string.IsNullOrWhiteSpace(_.Name)).Select(_ => new FilterTag() { BlockCategoryID = 9, Value = _.ID.ToString(), Name = _.Name, HotelCount = dicFilterResult[_.Key] }).ToList();
                    //}

                    //其他 筛选项
                    filterFlags[otherIndex].Tags = filterTagInfos.FindAll(_ => _.Type == 23 && !string.IsNullOrWhiteSpace(_.Name)).Select(_ => new FilterTag() { BlockCategoryID = 9, Value = _.ID.ToString(), Name = _.Name, HotelCount = dicFilterResult[_.Key] }).ToList();
                    //酒店类型 筛选项
                    var tempHotelClassFilterFlag = filterTagInfos.FindAll(_ => _.Type == 21 && !string.IsNullOrWhiteSpace(_.Name));
                    filterFlags[hotelIndex].Tags = tempHotelClassFilterFlag.Select(_ => new FilterTag() { BlockCategoryID = 3, Value = _.ID.ToString(), Name = _.Name, HotelCount = _.ID == 5 ? 10000 : dicFilterResult[_.Key] }).OrderByDescending(_ => _.HotelCount).ToList();
                    //设施筛选项
                    filterFlags[facilityIndex].Tags = filterTagInfos.FindAll(_ => _.Type == 19 && !string.IsNullOrWhiteSpace(_.Name)).Select(_ => new FilterTag() { BlockCategoryID = 4, Value = _.ID.ToString(), Name = _.Name, HotelCount = dicFilterResult[_.Key] }).ToList();
                }
            }
            return filterFlags;
        }

        public static List<FilterTag> genHotelStarFilterTags(IEnumerable<int> starids)
        {
            var result = new List<FilterTag>();
            if (starids != null && starids.Count() > 0)
            {
                foreach (var item in starids.OrderBy(_ => _))
                {
                    switch (item)
                    {
                        case 1:
                        case 2:
                        case 3:
                            result.Add(new FilterTag
                            {
                                BlockCategoryID = 7,
                                IsMatch = false,
                                Name = "三星及以下",
                                PinyinName = "",
                                Value = "3"
                            });
                            break;
                        case 4:
                            result.Add(new FilterTag
                            {
                                BlockCategoryID = 7,
                                IsMatch = false,
                                Name = "四星(高档)",
                                PinyinName = "",
                                Value = "4"
                            });
                            break;
                        case 5:
                            result.Add(new FilterTag
                            {
                                BlockCategoryID = 7,
                                IsMatch = false,
                                Name = "五星及以上(豪华)",
                                PinyinName = "",
                                Value = "5"
                            });
                            break;
                    }
                }

                result = (from i in result
                          group i by new
                          {
                              Value = i.Value
                          }
                              into g
                              select new FilterTag()
                              {
                                  BlockCategoryID = 7,
                                  IsMatch = false,
                                  Name = g.ToList().First().Name,
                                  PinyinName = "",
                                  Value = g.Key.Value
                              }).ToList();//去重

                if (result.Count() > 1)
                {
                    result.Add(new FilterTag
                    {
                        BlockCategoryID = 7,
                        IsMatch = false,
                        Name = "不限",
                        PinyinName = "",
                        Value = "0"
                    });
                }
            }
            return result;
        }

        public static List<FilterDicEntity> GetHotelListFilterTagInfos(SearchHotelListFilterTagInfoParam param)
        {
            return HotelService.GetHotelListFilterTagInfos(param);
        }

        public static List<FilterTagBlock> InitFilterTagBlock()
        {
            return new List<FilterTagBlock>() {
                    new FilterTagBlock(){
                        BlockCategoryID = 6,
                        BlockTitle = "日期筛选",
                        MinTags = 0,
                        MaxTags = 1,
                        Tags = new List<FilterTag>()
                    },
                    new FilterTagBlock(){
                        BlockCategoryID = 5,
                        BlockTitle = "价格范围",
                        MinTags = 0,
                        MaxTags = 1,
                        Tags = new List<FilterTag>()
                    },
                    new FilterTagBlock(){
                        BlockCategoryID = 1,
                        BlockTitle = "靠近哪里",
                        MinTags = 0,
                        MaxTags = 100,
                        Tags = new List<FilterTag>()
                    },
                    new FilterTagBlock(){
                        BlockCategoryID = 2,
                        BlockTitle = "主题特色",
                        MinTags = 0,
                        MaxTags = 100,
                        Tags = new List<FilterTag>()
                    },
                    new FilterTagBlock(){
                        BlockCategoryID = 3,
                        BlockTitle = "酒店类型",
                        MinTags = 0,
                        MaxTags = 100,
                        Tags = new List<FilterTag>()
                    },
                    new FilterTagBlock(){
                        BlockCategoryID = 4,
                        BlockTitle = "设施服务",
                        MinTags = 0,
                        MaxTags = 100,
                        Tags = new List<FilterTag>()
                    }
            };
        }

        /// <summary>
        /// 4.3版本默认出现的标签块
        /// </summary>
        /// <returns></returns>
        public static List<FilterTagBlock> InitFilterTagBlock20()
        {
            return new List<FilterTagBlock>() {
                    new FilterTagBlock()
                    {
                        BlockCategoryID = 6,
                        BlockTitle = "日期筛选",
                        MinTags = 0,
                        MaxTags = 1,
                        Tags = new List<FilterTag>()
                    },
                    new FilterTagBlock(){
                        BlockCategoryID = 1,
                        BlockTitle = "区域",
                        MinTags = 0,
                        MaxTags = 100,
                        Tags = new List<FilterTag>()
                    },
                    new FilterTagBlock()
                    {
                        BlockCategoryID = 2,
                        BlockTitle = "主题特色",
                        MinTags = 0,
                        MaxTags = 100,
                        Tags = new List<FilterTag>()
                    },
                    new FilterTagBlock()
                    {
                        BlockCategoryID = 5,
                        BlockTitle = "价格范围",
                        MinTags = 0,
                        MaxTags = 1,
                        Tags = new List<FilterTag>()
                    },
                    new FilterTagBlock(){
                        BlockCategoryID = 7,
                        BlockTitle = "酒店星级",
                        MinTags = 0,
                        MaxTags = 10,
                        Tags = new List<FilterTag>()
                    },
                    new FilterTagBlock(){
                        BlockCategoryID = 3,
                        BlockTitle = "酒店类型",
                        MinTags = 0,
                        MaxTags = 100,
                        Tags = new List<FilterTag>()
                    },
                    new FilterTagBlock(){
                        BlockCategoryID = 8,
                        BlockTitle = "出游类型",
                        MinTags = 0,
                        MaxTags = 100,
                        Tags = new List<FilterTag>()
                    },
                    new FilterTagBlock(){
                        BlockCategoryID = 4,
                        BlockTitle = "设施服务",
                        MinTags = 0,
                        MaxTags = 100,
                        Tags = new List<FilterTag>()
                    },
                    new FilterTagBlock(){
                        BlockCategoryID = 9,
                        BlockTitle = "其他",
                        MinTags = 0,
                        MaxTags = 100,
                        Tags = new List<FilterTag>()
                    }
            };
        }

        public static void GenHotelListPrice(List<ListHotelItem3> hotels1, List<ListHotelItem2> hotels2, DateTime arrivalTime, DateTime departureTime)
        {
            List<HotelPriceEntity> hpl = new List<HotelPriceEntity>();
            if (hotels1 != null)
            {
                hpl = PriceService.QueryHotelListPrice(hotels1.Select(h => h.Id).ToList(), arrivalTime, departureTime);
            }
            else if (hotels2 != null)
            {
                hpl = PriceService.QueryHotelListPrice(hotels2.Select(h => h.Id).ToList(), arrivalTime, departureTime);
            }
            if (hpl.Count > 0 && hotels1 != null)
            {
                foreach (var h in hotels1)
                {
                    var thp = hpl.Where(hp => hp.HotelId == h.Id);

                    if (thp.Count() == 1 && thp.First().PriceList.Count() > 0)
                    {
                        if (thp.First().PriceList.Where(pr => pr.ChannelID == 100).Count() > 0) //有套餐
                        {
                            h.MinPrice = (int)thp.First().PriceList.Where(pr => pr.ChannelID == 100).FirstOrDefault().Price;
                            h.PriceType = 3;
                            h.Tag = "特惠";
                        }
                        else if (thp.First().PriceList.Where(pr => pr.ChannelID == 102).Count() > 0) //JLTour
                        {
                            h.MinPrice = (int)thp.First().PriceList.Where(pr => pr.ChannelID == 102).FirstOrDefault().Price;
                            h.PriceType = 2;
                            h.Tag = "特惠";
                        }
                        else
                        {
                            h.MinPrice = (int)thp.First().PriceList.Min(P => P.Price);
                            h.PriceType = 1;
                            h.Tag = "";
                        }
                    }
                    else
                    {
                        h.MinPrice = 0;
                        h.PriceType = 0;
                        h.Tag = "";
                    }
                }
            }
            else if (hpl.Count > 0 && hotels2 != null)
            {
                foreach (var h in hotels2)
                {
                    var thp = hpl.Where(hp => hp.HotelId == h.Id);

                    if (thp.Count() == 1 && thp.First().PriceList.Count() > 0)
                    {
                        if (thp.First().PriceList.Where(pr => pr.ChannelID == 100).Count() > 0) //有套餐
                        {
                            h.MinPrice = (int)thp.First().PriceList.Where(pr => pr.ChannelID == 100).FirstOrDefault().Price;
                            h.PriceType = 3;
                        }
                        else if (thp.First().PriceList.Where(pr => pr.ChannelID == 102).Count() > 0) //JLTour
                        {
                            h.MinPrice = (int)thp.First().PriceList.Where(pr => pr.ChannelID == 102).FirstOrDefault().Price;
                            h.PriceType = 2;
                        }
                        else
                        {
                            h.MinPrice = (int)thp.First().PriceList.Min(P => P.Price);
                            h.PriceType = 1;
                        }
                    }
                    else
                    {
                        h.MinPrice = 0;
                        h.PriceType = 0;
                    }
                }
            }
        }

        public static void GenHotelListSlotPrice(List<ListHotelItem3> hotels1, List<ListHotelItem2> hotels2, DateTime arrivalTime, DateTime departureTime, int minPrice, int maxPrice)
        {
            var hpl = new List<HotelPriceEntity>();
            var hpl2 = new List<HotelPriceEntity>();

            var hotelIds = new List<int>();

            if (hotels1 != null)
            {
                hotelIds = hotels1.Select(h => h.Id).ToList();
                hpl = PriceService.GenHotelListSlotPrice(hotelIds, arrivalTime, minPrice, maxPrice);
            }
            else if (hotels2 != null)
            {
                hotelIds = hotels2.Select(h => h.Id).ToList();
                hpl = PriceService.GenHotelListSlotPrice(hotelIds, arrivalTime, minPrice, maxPrice);
            }

            if (hpl.Count > 0 && hotels1 != null)
            {
                foreach (var h in hotels1)
                {
                    var thp = hpl.Where(hp => hp.HotelId == h.Id);

                    if (thp.Count() == 1 && thp.First().PriceList.Count() > 0)
                    {
                        if (thp.First().PriceList.Where(pr => pr.ChannelID == 100).Count() > 0) //有套餐
                        {
                            h.MinPrice = (int)thp.First().PriceList.Where(pr => pr.ChannelID == 100).FirstOrDefault().Price;
                            h.PriceType = 3;
                            h.Tag = "特惠";
                        }
                        else if (thp.First().PriceList.Where(pr => pr.ChannelID == 102).Count() > 0) //JLTour
                        {
                            h.MinPrice = (int)thp.First().PriceList.Where(pr => pr.ChannelID == 102).FirstOrDefault().Price;
                            h.PriceType = 2;
                            h.Tag = "特惠";
                        }
                        else
                        {
                            h.MinPrice = (int)thp.First().PriceList.Min(P => P.Price);
                            h.PriceType = 1;
                            h.Tag = "";
                        }
                    }
                    else
                    {
                        h.MinPrice = 0;
                        h.PriceType = 0;
                        h.Tag = "";
                    }

                    //补救方案，当列表价拿不到的时候，使用原始的列表价读取方式
                    if (h.MinPrice == 0)
                    {
                        GetHotelMinPricePlan(h, ref hpl2, hotels1, arrivalTime, departureTime);
                    }

                    if (h.MinPrice > 0)
                    {
                        hotelIds.Remove(h.Id);
                    }
                }
            }
            else if (hpl.Count > 0 && hotels2 != null)
            {
                foreach (var h in hotels2)
                {
                    var thp = hpl.Where(hp => hp.HotelId == h.Id);

                    if (thp.Count() == 1 && thp.First().PriceList.Count() > 0)
                    {
                        if (thp.First().PriceList.Where(pr => pr.ChannelID == 100).Count() > 0) //有套餐
                        {
                            h.MinPrice = (int)thp.First().PriceList.Where(pr => pr.ChannelID == 100).FirstOrDefault().Price;
                            h.PriceType = 3;
                        }
                        else if (thp.First().PriceList.Where(pr => pr.ChannelID == 102).Count() > 0) //JLTour
                        {
                            h.MinPrice = (int)thp.First().PriceList.Where(pr => pr.ChannelID == 102).FirstOrDefault().Price;
                            h.PriceType = 2;
                        }
                        else
                        {
                            h.MinPrice = (int)thp.First().PriceList.Min(P => P.Price);
                            h.PriceType = 1;
                        }
                    }
                    else
                    {
                        h.MinPrice = 0;
                        h.PriceType = 0;
                    }

                    ////补救方案，当列表价拿不到的时候，使用原始的列表价读取方式
                    //if (h.MinPrice == 0)
                    //{
                    //    GetHotelMinPricePlan(h, ref hpl2, hotels2, arrivalTime, departureTime);
                    //}

                    if (h.MinPrice > 0)
                    {
                        hotelIds.Remove(h.Id);
                    }
                }
            }

            //新增补救措施 2016.06.06 如果维护了基本价格可以使用
            var hotelBasePriceList = hotelIds.Any() ? PriceAdapter.QueryHotelBasePrice(hotelIds).FindAll(_ => _.MinPrice > 0) : null;
            if (hotels1 != null && hotelBasePriceList != null && hotelBasePriceList.Any())
            {
                foreach (var item in hotels1)
                {
                    foreach (var basePrice in hotelBasePriceList)
                    {
                        if (basePrice.HotelId == item.Id && item.MinPrice <= 0)
                        {
                            item.MinPrice = basePrice.MinPrice;
                        }
                    }
                }
            }

            if (hotels2 != null && hotelBasePriceList != null && hotelBasePriceList.Any())
            {
                foreach (var item in hotels2)
                {
                    foreach (var basePrice in hotelBasePriceList)
                    {
                        if (basePrice.HotelId == item.Id && item.MinPrice <= 0)
                        {
                            item.MinPrice = basePrice.MinPrice;
                        }
                    }
                }
            }
        }

        private static List<HotelPriceEntity> GetHotelMinPricePlan(ListHotelItem3 h, ref List<HotelPriceEntity> hpl2, List<ListHotelItem3> hotels1, DateTime arrivalTime, DateTime departureTime)
        {
            if (hpl2 != null && hpl2.Count <= 0)
            {
                hpl2 = PriceService.QueryHotelListPrice(hotels1.Select(hotelItem => hotelItem.Id).ToList(), arrivalTime, departureTime);
                if (hpl2 != null && hpl2.Count <= 0) hpl2 = null;
            }

            if (hpl2 != null && hpl2.Count > 0)
            {
                var thpList = hpl2.Where(hp => hp.HotelId == h.Id);

                if (thpList.Count() == 1 && thpList.First().PriceList.Count() > 0)
                {
                    if (thpList.First().PriceList.Where(pr => pr.ChannelID == 100).Count() > 0) //有套餐
                    {
                        h.MinPrice = (int)thpList.First().PriceList.Where(pr => pr.ChannelID == 100).FirstOrDefault().Price;
                        h.PriceType = 3;
                        h.Tag = "特惠";
                    }
                    else if (thpList.First().PriceList.Where(pr => pr.ChannelID == 102).Count() > 0) //JLTour
                    {
                        h.MinPrice = (int)thpList.First().PriceList.Where(pr => pr.ChannelID == 102).FirstOrDefault().Price;
                        h.PriceType = 2;
                        h.Tag = "特惠";
                    }
                    else
                    {
                        h.MinPrice = (int)thpList.First().PriceList.Min(P => P.Price);
                        h.PriceType = 1;
                        h.Tag = "";
                    }
                }
                else
                {
                    h.MinPrice = 0;
                    h.PriceType = 0;
                    h.Tag = "";
                }
            }
            return hpl2;
        }

        private static ListHotelItem3 GetListHotelItem3(int hotelID, int interestID)
        {
            List<ListHotelItem3> item3List = HotelService.GetListHotelItem3List(new List<int>() { hotelID });
            if (item3List == null || item3List.Count == 0)
            {
                return new ListHotelItem3();
            }
            ListHotelItem3 lhi3 = item3List[0];
            if (interestID > 0)
            {
                HJD.HotelManagementCenter.Domain.HomeThemeHotelEntity hthe = HMC_HotelService.GetHomeThemeHotelEntityList(hotelID, interestID).FindAll(_ => !_.IsDel).FirstOrDefault();
                if (hthe != null && !string.IsNullOrWhiteSpace(hthe.ShortUrl))
                {
                    lhi3.PictureSURLList = InsertNewPic2Front(lhi3.PictureSURLList, hthe.ShortUrl);
                }
            }
            return lhi3;
        }

        private static List<string> InsertNewPic2Front(List<string> pics, string newPic)
        {
            List<string> newList = new List<string>();
            if (pics == null || pics.Count == 0)
            {
                return newList;
            }
            newList.Add(newPic);
            if (pics.Count >= 5)
            {
                pics.RemoveAt(pics.Count - 1);//超过5张 删掉最后一张
            }

            newList.AddRange(pics);
            return newList;
        }

        private static string GenHotelIntro(int hotelID)
        {
            List<Hotel3Entity> h3s = HotelService.GetHotel3(hotelID).FindAll(_ => _.Type == 1).ToList();
            HotelInfo hi = new HotelInfo();
            if (h3s != null && h3s.Count > 0)
            {
                hi = GenHotelInfo(h3s[0]);
            }
            return string.Join("", hi.Items.Select(_ => _.content));
        }

        private static string FormatHotelListComment(string p)
        {
            //酒店列表页只显示前2段
            string[] cs = p.Split(new char[] { '，', ',', '.', '。', '!', '！' });
            if (cs.Count() > 3)
            {

                p = "";
                for (int i = 0; i < 3; i++)
                {
                    p += cs[i] + "，";
                }
                p = p.Remove(p.Length - 1) + "...";
            }

            return p;
        }

        /// <summary>
        /// 玩点酒店搜索
        /// </summary>
        /// <returns></returns>
        public WapInterestHotelsResult SearchInterestHotel3(HotelListQueryParam p)
        {
            DateTime arrivalTime = CommMethods.GetDefaultCheckIn();
            DateTime departureTime = arrivalTime.AddDays(1);

            try
            {
                arrivalTime = DateTime.Parse(p.checkin);
                departureTime = DateTime.Parse(p.checkout);
                if (departureTime < arrivalTime.AddDays(1)) departureTime = arrivalTime.AddDays(1);
            }
            catch { }

            HotelSearchParas argu = new HotelSearchParas();


            argu.Interest = p.Interest;
            argu.InterestPlace = GetInterestPlaceIDs(p.Interest, p.districtid, p.lat, p.lng, p.distance);

            argu.Featured = ParseIntArray(p.featured);
            argu.Distance = p.attraction > 0 ? 0 : p.distance;// 如果是按景区筛选，则不受目的地的限止
            argu.DistrictID = p.districtid;
            argu.Lat = p.lat;
            argu.Lng = p.lng;
            argu.nLat = p.nLat;
            argu.nLng = p.nLng;
            argu.ReturnCount = p.count;
            argu.StartIndex = p.start;
            argu.SortType = p.sort;
            argu.SortDirection = p.order;
            argu.Zone = ParseIntArray(p.zone);
            argu.Location = ParseIntArray(p.location);
            argu.Brands = ParseIntArray(p.brand);
            argu.sLat = p.sLat;
            argu.sLng = p.sLng;
            argu.MinPrice = p.minPrice;
            argu.MaxPrice = p.maxPrice;
            argu.Attraction = p.attraction;
            argu.HotelTheme = p.hotelTheme;

            if (argu.Lng < 0 || argu.Lat < 0)  //如果在南半球 或 在美国，那么只按排名排序
            {
                argu.SortType = 0;
            }

            var model = new WapInterestHotelsResult();

            argu.TagIDs = ParseIntArray(p.tag);
            argu.Valued = p.valued;
            argu.CheckInDate = arrivalTime;
            argu.CheckOutDate = departureTime;

            argu.NeedHotelID = true;
            argu.NeedFilterCol = false; //不需要返回筛选项统计结果

            model.Start = argu.StartIndex;

            //Stopwatch sw = new Stopwatch();
            var qResult = HotelService.QueryHotel(argu);// HotelService.SearchHotel(argu).ToList();
            var hotels = qResult.HotelList;

            model.TotalCount = qResult.TotalCount;

            if (hotels.Count > 0)
            {
                //价格数据
                if (p.maxPrice != -1)  //-1表示不需要价格数据
                {
                    List<HotelPriceEntity> hpl = PriceService.QueryHotelListPrice(hotels.Select(h => h.Id).ToList(), arrivalTime, departureTime);

                    //   List<HotelPrice> hpl = HotelService.QueryHotelListPriceWithOTAID(hotels.Select(h => h.Id).ToList(), arrivalTime, departureTime, OTAID);
                    if (hpl.Count > 0)
                    {
                        foreach (var h in hotels)
                        {
                            var thp = hpl.Where(hp => hp.HotelId == h.Id);
                            h.MinPrice = (int)(thp.Count() == 1 && thp.First().PriceList.Count() > 0 ? thp.First().PriceList.Min(P => P.Price) : 0);
                        }
                    }
                }

                //照片 排名
                int rank = p.start + 1;
                foreach (var hotel in hotels)
                {
                    hotel.Picture = hotel.PicSURL.Length == 0 ? PhotoAdapter.GenHotelPicUrl(defaultPicSUrl, Enums.AppPhotoSize.interestHotelList) : PhotoAdapter.GenHotelPicUrl(hotel.PicSURL, Enums.AppPhotoSize.interestHotelList);

                    hotel.PictureCount = 0;
                    hotel.OfficalPictureCount = 0;

                    hotel.Rank = rank;
                    rank++;
                }
            }

            model.Result = hotels;
            model.InterestType = p.type;
            model.InterestID = p.type == 1 ? p.hotelTheme : p.attraction;
            model.InterestName = p.type == 1 ? ResourceAdapter.GetHotelThemeName(p.hotelTheme) : ResourceAdapter.GetAttractionName(p.attraction);

            return model;
        }

        /// <summary>
        /// 根据目的地ID 返回取那家OTA的价格数据
        /// </summary>
        /// <param name="districtid"></param>
        /// <returns></returns>
        private int GetOTAID(int districtid)
        {
            return 0;
            return (int)LocalCache.GetData<object>("OTAIDDistrictID" + districtid.ToString(), () =>
            {
                HJD.DestServices.Contract.DistrictInfoEntity di = destService.GetDistrictInfo(new List<int> { districtid }).FirstOrDefault();

                if (di.InChina)
                {
                    return CtripID;
                }
                else
                {
                    return BookingID;
                }
            });

        }

        private HotelItem GenNewHotelItem(string Name)
        {
            HotelItem hi = new HotelItem();
            hi.Address = "Address";
            hi.ReviewCount = 3;
            hi.Currency = "RMB";
            hi.Description = "Description";
            hi.DistrictCount = 1;
            hi.DistrictId = 2;
            hi.DistrictName = "DistrictName";
            hi.EName = "EName";
            // hi.Facilities = new string[] { "f1", "f2" };
            hi.GLat = 123.231;
            hi.GLat = 30.23;
            hi.Id = 5;
            hi.InChina = true;
            hi.MaxPrice = 1000;
            hi.MinPrice = 100;
            hi.Name = Name;
            hi.OfficalPictureCount = 30;
            hi.Picture = "Picture";
            hi.PictureCount = 5;
            hi.Rank = 4;
            hi.Score = 5;
            hi.Star = 3;
            hi.Telephone = "telephone";
            hi.Types = new string[] { "t1", "t2" };

            return hi;
        }

        /// <summary>
        /// 获取酒店详情 为APP返回数据
        /// </summary>
        /// <param name="id">酒店id</param>
        /// <returns></returns>        
        public HotelItemResult Get(int id, string checkIn, string checkOut)
        {
            return Get(id, checkIn, checkOut, "app");
        }

        public HotelItem GetHotelBasicInfo(int hotelid)
        {
            return ResourceAdapter.GetHotel(hotelid, 0);
        }

        /// <summary>
        /// 获取酒店详情
        /// </summary>
        /// <param name="id">酒店id</param>
        /// <param name="sType">访问源类型：app, m=mobile</param>
        /// <returns></returns>
        public HotelItemResult Get(int id, string checkIn, string checkOut, string sType)
        {
            HotelItem hi = ResourceAdapter.GetHotel(id, UserId);
            DateTime arrivalTime = DateTime.Now;
            DateTime departureTime = arrivalTime;

            try
            {
                arrivalTime = DateTime.Parse(checkIn);
                departureTime = DateTime.Parse(checkOut);
                if (departureTime < arrivalTime.AddDays(1)) departureTime = arrivalTime.AddDays(1);
            }
            catch { }



            List<HotelPriceEntity> hpl = PriceService.QueryHotelListPrice(new List<int> { id }, arrivalTime, departureTime);
            List<OTAInfo> otaInfos = HotelHelper.GetHotelOTAInfo(id, sType);

            if (hpl.Count > 0 && hpl[0].PriceList.Count > 0)
            {
                hi.MinPrice = Decimal.Round(hpl.First().PriceList.Min(o => o.Price), 0);
                foreach (HotelOTAPrice hpe in hpl[0].PriceList)
                {
                    OTAInfo toi = otaInfos.Where(o => o.ChannelID == hpe.ChannelID).FirstOrDefault();
                    if (toi != null && toi.ChannelID > 0) toi.Price = Decimal.Round(hpe.Price, 0);
                }
            }

            ArguHotelReview p = new ArguHotelReview();
            p.Hotel = id;
            p.Start = 0;
            p.Count = 5;
            ReviewResult cr = GetHotelReviews(p);

            HotelItemResult hr = new HotelItemResult();
            hr.hotel = hi;
            hr.reviews = cr;
            //有价格的情况下，价格最低优先
            //合作OTA有四家，同等价格下，优先显示次序为携程、艺龙、住哪儿、同程
            hr.OTAInfos = otaInfos.OrderBy(o => o.Price == 0 ? 10000 : o.Price).ThenBy(o => o.ChannelID == 2 ? 1 : (o.ChannelID == 6 ? 2 : (o.ChannelID == 11 ? 3 : (o.ChannelID == 6 ? 4 : 100)))).ToList();

            return hr;
        }

        /// <summary>
        /// 获取酒店详情
        /// </summary>
        /// <param name="id">酒店id</param>
        /// <param name="sType">访问源类型：app, m=mobile</param>
        /// <returns></returns>
        public HotelItem2Result Get2(int id, string checkIn, string checkOut, string sType, int themeid)
        {
            HotelItem hi = ResourceAdapter.GetHotel(id, UserId);

            DateTime arrivalTime = CommMethods.GetDefaultCheckIn();
            DateTime departureTime = arrivalTime.AddDays(1);

            if (checkIn != "")
            {
                try
                {
                    arrivalTime = DateTime.Parse(checkIn);
                    departureTime = DateTime.Parse(checkOut);
                    if (departureTime < arrivalTime.AddDays(1)) departureTime = arrivalTime.AddDays(1);
                }
                catch { }
            }



            List<HotelPriceEntity> hpl = PriceService.QueryHotelListPrice(new List<int> { id }, arrivalTime, departureTime);
            List<OTAInfo> otaInfos = HotelHelper.GetHotelOTAInfo(id, sType);

            if (hpl.Count > 0 && hpl[0].PriceList.Count > 0)
            {
                hi.MinPrice = Decimal.Round(hpl.First().PriceList.Min(o => o.Price), 0);
                foreach (HotelOTAPrice hpe in hpl[0].PriceList)
                {
                    OTAInfo toi = otaInfos.Where(o => o.ChannelID == hpe.ChannelID).FirstOrDefault();
                    if (toi != null && toi.ChannelID > 0) toi.Price = Decimal.Round(hpe.Price, 0);
                }
            }


            List<HotelTFTRelItemEntity> tftList = GetHotelTFTList(hi.Id, themeid);

            // List<HotelTFTReviewEntity> cr = HotelService.GetHotelTFTReview(hi.Id);

            //if (themeid > 0 && tftList.Count > 0)
            //{
            //    if( !( tftList.First().Type == 3 && tftList.First().TFTID == themeid ) ){
            //        HotelTFTRelItemEntity htft = tftList.Where(c => c.Type == 3 && c.TFTID == themeid).FirstOrDefault();
            //        if (htft != null)
            //        {
            //            tftList.Remove(htft);
            //            tftList.Insert(0, htft);
            //        }
            //    }
            //}

            ArguHotelReview rp = new ArguHotelReview();
            rp.Hotel = id;
            //rp.
            rp.Start = 0;
            rp.Count = 5;
            rp.TFTID = tftList.Count > 0 ? tftList.FirstOrDefault().TFTID : 0;
            rp.TFTType = 1;
            ReviewResult tr = GetHotelReviews(rp);


            HotelItem2Result hr = new HotelItem2Result();
            hr.hotel = hi;
            hr.reviews = new List<HotelTFTReview>();// Trans2TFTReview(cr);
            //有价格的情况下，价格最低优先
            //合作OTA有四家，同等价格下，优先显示次序为携程、艺龙、住哪儿、同程
            hr.OTAInfos = otaInfos.OrderBy(o => o.Price == 0 ? 10000 : o.Price).ThenBy(o => o.ChannelID == 2 ? 1 : (o.ChannelID == 6 ? 2 : (o.ChannelID == 11 ? 3 : (o.ChannelID == 6 ? 4 : 100)))).ToList();
            hr.Pics = GetHotelPhotos(hi.Id, 0).HPList.Take(5).Select(p => PhotoAdapter.GenHotelPicUrl(p.SURL, Enums.AppPhotoSize.appdetail)).ToList();
            hr.tftList = tftList.Count > 3 ? tftList.Take(3).ToList() : tftList;
            hr.themereviews = tr;

            return hr;
        }

        /// <summary>
        /// 获取酒店详情  for 周末酒店2.0版
        /// </summary>
        /// <param name="id">酒店id</param>
        /// <param name="sType">访问源类型：app, m=mobile</param>
        /// <returns></returns>
        public HotelItem3 Get3(int id, string checkIn, string checkOut, string sType, int interestid)
        {
            DateTime arrivalTime = CommMethods.GetDefaultCheckIn();
            DateTime departureTime = arrivalTime.AddDays(1);

            if (checkIn != "")
            {
                try
                {
                    arrivalTime = DateTime.Parse(checkIn);
                    departureTime = DateTime.Parse(checkOut);
                    if (departureTime < arrivalTime.AddDays(1)) departureTime = arrivalTime.AddDays(1);
                }
                catch { }
            }


            HotelItem3 hi = GetHotelItem3FromCache(id, interestid);

            List<HotelPriceEntity> hpl = PriceService.QueryHotelListPrice(new List<int> { id }, arrivalTime, departureTime);

            if (hpl.Count > 0 && hpl[0].PriceList.Count > 0)
            {
                if (hpl.First().PriceList.Where(pr => pr.ChannelID == 100).Count() > 0) //有套餐
                {
                    hi.MinPrice = (int)hpl.First().PriceList.Where(pr => pr.ChannelID == 100).FirstOrDefault().Price;
                    hi.PriceType = 3;
                    // hi.Tag = "特惠";
                }
                else if (hpl.First().PriceList.Where(pr => pr.ChannelID == 101).Count() > 0) //有专享
                {
                    hi.MinPrice = (int)hpl.First().PriceList.Where(pr => pr.ChannelID == 101).FirstOrDefault().Price;
                    hi.PriceType = 2;
                    //hi.Tag = "特惠";
                }
                else
                {
                    hi.MinPrice = Decimal.Round(hpl.First().PriceList.Min(P => P.Price), 0);
                    hi.PriceType = 1;
                }
            }

            hi.ShareDesc = string.Format("这家不错！￥{0}{1}", hi.MinPrice,
                hi.PriceType == 1 ? "起 " +
                (hi.Intro.Items.Count == 0 ? "" : hi.Intro.Items[0].content.Substring(0, hi.Intro.Items[0].content.Length > 15 ? 15 : hi.Intro.Items[0].content.Length) + "...")
                : " " + GetSharePckageInfoByPrice(hi.HotelID, (int)hi.MinPrice));

            return hi;
        }

        public HotelItem3 GetHotelItem3FromCache(int id, int interestid)
        {
            return LocalCache.GetData<HotelItem3>(string.Format("HI3:{0}:{1}", id, interestid), () =>
            {
                return GetHotelItem3(id, interestid);
            });
        }

        /// <summary>
        /// 清理酒店缓存信息，包括酒店照片缓存
        /// </summary>
        /// <param name="hotelid"></param>
        /// <returns></returns>
        public bool ClareHotelInfoCache(int hotelid)
        {
            List<int> il = ResourceAdapter.GetHotelInterestIDs(hotelid);

            foreach (int i in il)
            {
                LocalCache.Remove(GetHotelItem3CacheKey(hotelid, i));
            }

            //HotelService.RefreshCacheHotelInfoEx(new List<int> { hotelid });
            HotelService.RefreshSomeHotelCache(new List<int> { hotelid });
            HotelService.RefreshHotelPhotos(hotelid);

            return true;
        }

        private string GetHotelItem3CacheKey(int id, int interestid)
        {
            return string.Format("HI3:{0}:{1}", id, interestid);
        }

        private string GetHotelItem4CacheKey(int id, int interestid)
        {
            return string.Format("HI4:{0}:{1}", id, interestid);
        }

        /// <summary>
        /// 获取酒店详情  for 周末酒店2.0版
        /// </summary>
        /// <param name="id">酒店id</param>
        /// <param name="sType">访问源类型：app, m=mobile</param>
        /// <returns></returns>
        public HotelItem3 Get30(int id, string checkIn, string checkOut, string sType, int interestid)
        {
            DateTime arrivalTime = CommMethods.GetDefaultCheckIn();
            DateTime departureTime = arrivalTime.AddDays(1);

            if (checkIn != "")
            {
                try
                {
                    arrivalTime = DateTime.Parse(checkIn);
                    departureTime = DateTime.Parse(checkOut);
                    if (departureTime < arrivalTime.AddDays(1)) departureTime = arrivalTime.AddDays(1);
                }
                catch { }
            }

            HotelItem3 hi = GetHotelItem3FromCache(id, interestid);

            //LocalCache.GetData<HotelItem3>(GetHotelItem3CacheKey(id, interestid), () =>
            //    {
            //        return GetHotelItem3(id, interestid);
            //    });

            List<HotelPriceEntity> hpl = PriceService.QueryHotelListPrice(new List<int> { id }, arrivalTime, departureTime);

            if (hpl.Count > 0 && hpl[0].PriceList.Count > 0)
            {
                if (hpl.First().PriceList.Where(pr => pr.ChannelID == 100).Count() > 0) //有套餐
                {
                    hi.MinPrice = (int)hpl.First().PriceList.Where(pr => pr.ChannelID == 100).FirstOrDefault().Price;
                    hi.PriceType = 3;
                    //hi.Tag = "特惠";
                }
                else if (hpl.First().PriceList.Where(pr => pr.ChannelID == 102).Count() > 0) //有专享
                {
                    hi.MinPrice = (int)hpl.First().PriceList.Where(pr => pr.ChannelID == 102).FirstOrDefault().Price;
                    hi.PriceType = 2;
                    //hi.Tag = "特惠";
                }
                else
                {
                    hi.MinPrice = Decimal.Round(hpl.First().PriceList.Min(P => P.Price), 0);
                    hi.PriceType = 1;
                }
            }

            hi.ShareDesc = string.Format("这家不错！￥{0}{1}", hi.MinPrice,
                hi.PriceType < 3 ? "起 " +
                (hi.Intro.Items.Count == 0 ? "" : hi.Intro.Items[0].content.Substring(0, hi.Intro.Items[0].content.Length > 15 ? 15 : hi.Intro.Items[0].content.Length) + "...")
                : " " + GetSharePckageInfoByPrice(hi.HotelID, (int)hi.MinPrice));

            return hi;

        }

        /// <summary>
        /// 酒店详情页获取【APP 4.? USE】
        /// </summary>
        /// <param name="id"></param>
        /// <param name="checkIn"></param>
        /// <param name="checkOut"></param>
        /// <param name="sType"></param>
        /// <param name="interestid"></param>
        /// <returns></returns>
        public HotelItem4 Get40(int id, string checkIn, string checkOut, string sType, int interestid)
        {
            DateTime arrivalTime = CommMethods.GetDefaultCheckIn();
            DateTime departureTime = arrivalTime.AddDays(1);

            if (!(string.IsNullOrWhiteSpace(checkIn) || checkIn == "0"))
            {
                try
                {
                    arrivalTime = DateTime.Parse(checkIn);
                    departureTime = DateTime.Parse(checkOut);
                    if (departureTime < arrivalTime.AddDays(1)) departureTime = arrivalTime.AddDays(1);
                }
                catch
                {

                }
            }

            HotelItem4 h4 = GetHotelItem4(id, interestid);

            //缓存1
            PackagePriceInfo priceInfo = LocalCache5Min.GetData<PackagePriceInfo>(string.Format("HotelAdapter.Get.GetHotelPriceInfo:{0}", id), () =>
            {
                return GetHotelPriceInfo(0, id, arrivalTime, departureTime);
            });

            h4.PackageList = priceInfo.PackageList;
            h4.PriceType = priceInfo.PriceType;
            if (priceInfo.PackageList != null && priceInfo.PackageList.Count != 0)
            {
                h4.MinPrice = priceInfo.PackageList[0].Price;//已套餐可售的最低价为准  priceInfo.MinPrice;
            }
            else
            {
                h4.MinPrice = 0;//没有套餐就是0
            }
            var firstHotelPackage = priceInfo.PackageList.FirstOrDefault();
            h4.ShareDesc = string.Format("这家不错！￥{0}{1}", firstHotelPackage != null ? firstHotelPackage.Price : h4.MinPrice,
            h4.PriceType < 3 ? "起 " +
            (h4.Intro.Items.Count == 0 ? "" : h4.Intro.Items[0].content.Substring(0, h4.Intro.Items[0].content.Length > 15 ? 15 : h4.Intro.Items[0].content.Length) + "...")
            : " " + GetSharePckageInfoByPrice(h4.HotelID, (int)h4.MinPrice));

            return h4;
        }

        /// <summary>
        /// 酒店详情页获取【APP 4.5/4.6/4.7 USE】
        /// </summary>
        /// <param name="id"></param>
        /// <param name="checkIn"></param>
        /// <param name="checkOut"></param>
        /// <param name="sType"></param>
        /// <param name="interestid"></param>
        /// <param name="userID"></param>
        /// <param name="appVer"></param>
        /// <param name="pid"></param>
        /// <returns></returns>
        public HotelItem5 Get50(int id, string checkIn, string checkOut, string sType, int interestid, long userID = 0, string appVer = "", int pid = 0)
        {
            DateTime arrivalTime = CommMethods.GetDefaultCheckIn();
            DateTime departureTime = arrivalTime.AddDays(1);

            if (!(string.IsNullOrWhiteSpace(checkIn) || checkIn == "0"))
            {
                try
                {
                    arrivalTime = DateTime.Parse(checkIn);
                    departureTime = DateTime.Parse(checkOut);
                    if (departureTime < arrivalTime.AddDays(1)) departureTime = arrivalTime.AddDays(1);
                }
                catch
                {

                }
            }
            TimeLog log = new TimeLog(string.Format("Get50 hotelid:{0} checkIn:{1} checkOut:{2} sType:{3} interestid:{4} userID:{5} appVer:{6}", id, checkIn, checkOut, sType, interestid, userID, appVer)
            , 2000, null);

            HotelItem5 h5 = GetHotelItem5(id, interestid, appVer, log);


            CollectParam param = new CollectParam();

            List<FavouriteHotel> collectHotelList = CollectAdapter.GetCollectList(param);
            bool isCollect = collectHotelList.Select(_ => _.HotelID == h5.HotelID) != null ? true : false;

            log.AddLog("GetHotelItem5");

            PackagePriceInfo priceInfo = new PackagePriceInfo()
            {
                MinPrice = 0,
                OTAList = new List<OTAInfo2>(),
                PackageList = new List<PackageItem>(),
                PriceType = 0,
                Tag = ""
            };
            if (pid > 0)
            {
                var hotelPackagePrice = HotelService.GetHotelPackageFirstAvailablePriceByPIds(new int[] { pid }, arrivalTime, departureTime);
                if (hotelPackagePrice != null && hotelPackagePrice.Any())
                {
                    priceInfo.MinPrice = hotelPackagePrice.First().Price;
                    priceInfo.PriceType = 3;//默认显示
                    priceInfo.PackageList.Add(hotelPackagePrice.Select(_ => new PackageItem()
                    {
                        Brief = _.Brief,
                        HotelID = _.HotelID,
                        HotelName = _.HotelName,
                        InterestID = 0,
                        PicUrl = "",
                        PID = _.PID,
                        Price = _.Price,
                        Title = _.Code
                    }).First());
                }
            }
            if (priceInfo.MinPrice == 0)
            {
                priceInfo = GetHotelPriceInfo(userID, id, arrivalTime, departureTime, parentLog: log);
            }

            log.AddLog("GetHotelPriceInfo");

            h5.PackageList = priceInfo.PackageList;
            h5.OTAList = priceInfo.OTAList;
            GenOtaTransferURL(h5.OTAList, id, arrivalTime.ToString("yyyy-MM-dd"), departureTime.ToString("yyyy-MM-dd"));//替换OTAList里面的日期价格

            h5.PriceType = priceInfo.PriceType;
            h5.MinPrice = priceInfo.MinPrice;


            h5.RecommendHotelList = GenRecommendHotelList(id, userID, interestid, arrivalTime, departureTime, parentLog: log);
            log.AddLog("GenRecommendHotelList");

            //生成酒店分享的 标题和链接
            h5.ShareTitle = h5.HotelName;
            if (userID > 0)
            {
                //追踪码生成 点评ID + 英文半角逗号 + 分享者UserID DES加密后传输 防止url篡改
                string desBase64Str = HJDAPI.Common.Security.DES.Encrypt(string.Format("{0},{1}", h5.HotelID, userID));
                h5.ShareLink = string.Format("http://www.zmjiudian.com/hotel/{0}?CID={1}", h5.HotelID, userID);
            }
            else
            {
                h5.ShareLink = string.Format("http://www.zmjiudian.com/hotel/{0}", h5.HotelID);
            }

            h5.ShareLink = AccessAdapter.GenShortUrl(0, h5.ShareLink);

            var firstHotelPackage = priceInfo.PackageList.FirstOrDefault();

            h5.ShareDesc = string.Format("这家不错！￥{0}{1}", firstHotelPackage != null ? firstHotelPackage.Price : h5.MinPrice,
                h5.PriceType < 3 ? "起 " +
                (h5.Intro.Items.Count == 0 ? "" : h5.Intro.Items[0].content.Substring(0, h5.Intro.Items[0].content.Length > 15 ? 15 : h5.Intro.Items[0].content.Length) + "...")
                : " " + GetSharePckageInfoByPrice(h5.HotelID, (int)h5.MinPrice));

            //优点 不足
            h5.Advantages = h5.FeatureList.Select(_ => _.FeaturedName + "-" + _.Comment).ToList();

            //地图位置信息
            HotelMapBasicInfo mapInfo = HotelService.GetHotelMapInfo(h5.HotelID);
            h5.IsInChina = mapInfo.InChina;
            h5.HotelMapUrl = string.Format("http://www.zmjiudian.com/hotel/{0}/map", h5.HotelID);

            h5.districtName = GetHotelAssociatedZoneName(id, h5.districtID);//获取缓存的地区名称
            log.AddLog("GetHotelAssociatedZoneName");

            var cacheH5RelComment = GetRelComment(id, userID);

            h5.FollowingCommentCount = cacheH5RelComment.FollowingCommentCount;
            h5.InspectorCommentCount = cacheH5RelComment.InspectorCommentCount;

            h5.RelComments = cacheH5RelComment.RelComments;

            h5.PriceCinDate = arrivalTime;
            h5.PriceCouDate = departureTime;

            log.AddLog("GetRelComments");
            log.Finish();

            //对当前查询的酒店做后台列表价动态更新操作
            HotelAdapter.PublishUpdatePriceSlotTask(id, arrivalTime.ToString("yyyy-MM-dd"));

            return h5;
        }

        private static HotelItem5 GetRelComment(int hotelID, long userID)
        {
            return LocalCache10Min.GetData<HotelItem5>(string.Format("{0}:{1}", hotelID, userID), () =>
            {
                return GenRelComment(hotelID, userID);
            });

        }

        private static HotelItem5 GenRelComment(int hotelID, long userID)
        {
            TimeLog log = new TimeLog(string.Format("GenRelComment:hotelid:{0} userID:{1}", hotelID, userID), 500);
            HotelItem5 h5 = new HotelItem5();
            int followingCommentCount = 0;
            int inspectorCommentCount = 0;
            var relCommentInfos = new List<CommentInfoEntity>();
            if (userID != 0)
            {
                relCommentInfos = CommentAdapter.GetCommentInfosByFollowing(userID, hotelID, new int[] { 1 },
                    out followingCommentCount, 0, 6);
                log.AddLog("GetCommentInfosByFollowing");
            }
            if (followingCommentCount == 0)
            {
                relCommentInfos = CommentAdapter.GetCommentInfosByInspector(hotelID, new int[] { 1 },
                    out inspectorCommentCount, 0, 6);

                log.AddLog("GetCommentInfosByInspector");
            }

            h5.FollowingCommentCount = followingCommentCount;
            h5.InspectorCommentCount = inspectorCommentCount;

            var userIds = relCommentInfos.Select(_ => _.Comment.UserID);
            var userInfos = AccountAdapter.GetUserBasicInfo(userIds);

            h5.RelComments = (from i in relCommentInfos
                              join j in userInfos on i.Comment.UserID equals j.UserId
                              select new HotelRelComment()
                              {
                                  Author = j.NickName,
                                  AvatarUrl = string.IsNullOrWhiteSpace(j.AvatarUrl) ? DescriptionHelper.defaultAvatar.Replace("_jupiter", "_small") : PhotoAdapter.GenHotelPicUrl(j.AvatarUrl, Enums.AppPhotoSize.small),
                                  AuthorUserID = j.UserId,
                                  Title = ""
                              }).Distinct().ToList();

            return h5;
        }

        /// <summary>
        /// 酒店详情页获取【APP 5.0 USE】
        /// </summary>
        /// <param name="_contextBasicInfo">上下文基本信息</param>
        /// <param name="id"></param>
        /// <param name="checkIn"></param>
        /// <param name="checkOut"></param>
        /// <param name="sType"></param>
        /// <param name="interestid"></param>
        /// <param name="pid"></param>
        /// <param name="commentid"></param>
        /// <returns></returns>
        public HotelItem6 Get60(ContextBasicInfo _contextBasicInfo, int id, string checkIn, string checkOut, string sType, int interestid, int pid = 0, int commentid = 0)
        {
            var userID = _contextBasicInfo.AppUserID;
            var appVer = _contextBasicInfo.AppVer;

            //默认标识不指定入住日期
            var fixDate = false;

            //默认一个日期（明天）
            DateTime arrivalTime = CommMethods.GetDefaultCheckIn();
            DateTime departureTime = arrivalTime.AddDays(1);

            //checkIn非零，则认为指定了列表价日期
            if (!(string.IsNullOrWhiteSpace(checkIn) || checkIn == "0"))
            {
                try
                {
                    arrivalTime = DateTime.Parse(checkIn);
                    departureTime = DateTime.Parse(checkOut);
                    if (departureTime < arrivalTime.AddDays(1)) departureTime = arrivalTime.AddDays(1);

                    //标识指定了列表价日期
                    fixDate = true;
                }
                catch
                {

                }
            }

            TimeLog log = new TimeLog(string.Format("Get60 hotelid:{0} checkIn:{1} checkOut:{2} sType:{3} interestid:{4} userID:{5} appVer:{6}", id, checkIn, checkOut, sType, interestid, userID, appVer)
            , 1000, null);

            //初始获取酒店详情实例
            HotelItem6 h6 = GetHotelItem6(id, interestid, appVer, log);

            //如果详情页有传进来点评ID，则查询出该店铺的
            List<HotelReviewExEntity> commForId = HotelService.GetHotelReviewByCommentId(commentid).ToList();
            if (commForId != null && commForId.Count > 0)
            {
                FillWHotelCommentOne(commForId);
                h6.CommentForId = transReviewExToCommentItem(commForId, h6.HotelID).FirstOrDefault();
            }

            //判断当前用户是否VIP
            var isVip = AccountAdapter.IsVIPCustomer(AccountAdapter.GetCustomerType(userID));

            //非VIP用户
            if (!isVip)
            {
                h6.Suggest.ActionUrl = "http://www.zmjiudian.com/Coupon/VipShopInfo?userid={userid}&_newpage=1&_isoneoff=1";
                h6.Suggest.Text = "成为VIP>>";
                h6.Suggest.Description = "，付199元年费可";

                //5.6.2版本后，文案调整
                if (_contextBasicInfo.IsThanVer5_7)
                {
                    h6.Suggest.Description = "付199元年费可";
                }
            }

            //var customerType = AccountAdapter.GetCustomerType(userID);
            //if (string.IsNullOrWhiteSpace(appVer) || appVer.CompareTo("5.6.2") < 0)
            //{
            //    if (customerType == (int)HJDAPI.Common.Helpers.Enums.CustomerType.general)
            //    {
            //        h6.Suggest.ActionUrl = "http://www.zmjiudian.com/Coupon/VipShopInfo?userid={userid}&_newpage=1&_isoneoff=1";
            //        h6.Suggest.Text = "成为VIP>>";
            //        h6.Suggest.Description = "，付199元年费可";
            //    }
            //}
            //else
            //{
            //    if (customerType == (int)HJDAPI.Common.Helpers.Enums.CustomerType.general)
            //    {
            //        h6.Suggest.ActionUrl = "http://www.zmjiudian.com/Coupon/VipShopInfo?userid={userid}&_newpage=1&_isoneoff=1";
            //        h6.Suggest.Text = "成为VIP>>";
            //        h6.Suggest.Description = "付199元年费可";
            //    }
            //}

            //是否收藏【酒店详情页优化点：直接查询当前HotelId是否收藏即可，不要查询出所有收藏列表再where是否存在，这个方法太笨 2017-08-11 haoy】
            bool isCollect = false;
            if (userID > 0)
            {
                try
                {
                    CollectParam param = new CollectParam();
                    param.UserID = userID;
                    List<FavouriteHotel> collectHotelList = CollectAdapter.GetCollectList(param);
                    if (collectHotelList.Count > 0)
                    {
                        isCollect = collectHotelList.Where(_ => _.HotelID == h6.HotelID).FirstOrDefault() != null ? true : false;
                    }
                }
                catch (Exception e)
                {
                    Log.WriteLog("收藏" + e);
                }
            }
            h6.IsCollection = isCollect;

            log.AddLog("GetHotelItem6");

            //初始列表价价格对象
            PackagePriceInfo priceInfo = new PackagePriceInfo()
            {
                MinPrice = 0,
                OTAList = new List<OTAInfo2>(),
                PackageList = new List<PackageItem>(),
                PriceType = 0,
                Tag = "",
                CheckIn = arrivalTime,
                CheckOut = departureTime
            };

            #region 指定了套餐ID，则获取指定套餐的列表价信息

            if (pid > 0)
            {
                //获取指定套餐的信息
                var hotelPackagePrice = HotelService.GetHotelPackageFirstAvailablePriceByPIds(new int[] { pid }, arrivalTime, departureTime);
                if (hotelPackagePrice != null && hotelPackagePrice.Any())
                {
                    //获取当前酒店60天内的可售日历
                    var hotelPackageCalendar = HotelService.GenHotelPackageCalendar(id, arrivalTime, pid, 60);
                    if (hotelPackageCalendar != null && hotelPackageCalendar.Exists(_ => _.SellState == 1))
                    {
                        //找出该套餐最便宜的一个日期的信息
                        var minPriceCalendar = hotelPackageCalendar.Where(_ => _.SellState == 1).OrderBy(_ => _.SellPrice).First();

                        priceInfo.MinPrice = minPriceCalendar.SellPrice;
                        priceInfo.PriceType = 3;//默认显示
                        priceInfo.PackageList.Add(hotelPackagePrice.Select(_ => new PackageItem()
                        {
                            Brief = _.Brief,
                            HotelID = _.HotelID,
                            HotelName = _.HotelName,
                            InterestID = 0,
                            PicUrl = "",
                            PID = _.PID,
                            Price = minPriceCalendar.VipPrice == 0 ? 0 : minPriceCalendar.SellPrice,//vip价格为0 普通价强制设为0 20170106
                            VIPPrice = minPriceCalendar.VipPrice,
                            Title = _.Code
                        }).First());

                        //设置当前价格对应的日期
                        arrivalTime = minPriceCalendar.Day;
                        departureTime = arrivalTime.AddDays(1);
                    }
                }
            }

            #endregion

            #region 没有指定套餐ID，通用列表价获取

            if (priceInfo.MinPrice == 0)
            {
                priceInfo = GetHotelPriceInfo(userID, id, arrivalTime, departureTime, parentLog: log, fixDate: fixDate);

                //将入住日期和离店日期改为最终列表价的日期
                arrivalTime = priceInfo.CheckIn;
                departureTime = priceInfo.CheckOut;
            }

            #endregion

            log.AddLog("GetHotelPriceInfo");

            h6.PackageList = priceInfo.PackageList;

            ////测试代码。。。生产需要拿掉
            //if (h6.HotelID == 608031)
            //{
            //    h6.PackageList.First().NightCount = 2;
            //    h6.PackageList.First().PackageUrl = "http://www.zmjiudian.com/hotel/package/6797?_newpage=1&_newtitle=1&_dorpdown=1 ";
            //    h6.PackageList.First().PackageLables.Add("http://whfront.b0.upaiyun.com/app/img/icon/new_vip.png");
            //}

            priceInfo.OTAList.ForEach(_ => h6.OTAList.Add(new OTAInfo2()
            {
                AccessURL = _.AccessURL,
                CanSyncPrice = _.CanSyncPrice,
                ChannelID = _.ChannelID,
                Name = _.Name,
                OTAHotelID = _.OTAHotelID,
                OtaTransferURL = _.OtaTransferURL,
                Price = _.VIPPrice == 0 ? 0 : _.Price,
                PriceBrief = _.PriceBrief,
                PriceName = _.PriceName,
                RoomRates = _.RoomRates,
                VIPPrice = _.VIPPrice
            }));
            //h6.OTAList = priceInfo.OTAList;
            GenOtaTransferURL(h6.OTAList, id, arrivalTime.ToString("yyyy-MM-dd"), departureTime.ToString("yyyy-MM-dd"));//替换OTAList里面的日期价格

            h6.PriceType = priceInfo.PriceType;
            h6.MinPrice = priceInfo.MinPrice;

            h6.RecommendHotelList = GenRecommendHotelList(id, userID, interestid, arrivalTime, departureTime, parentLog: log);
            log.AddLog("GenRecommendHotelList");

            //生成酒店分享的 标题和链接
            h6.ShareTitle = h6.HotelName;
            if (userID > 0)
            {
                //追踪码生成 点评ID + 英文半角逗号 + 分享者UserID DES加密后传输 防止url篡改
                string desBase64Str = HJDAPI.Common.Security.DES.Encrypt(string.Format("{0},{1}", h6.HotelID, userID));
                h6.ShareLink = string.Format("http://www.zmjiudian.com/hotel/{0}?CID={1}", h6.HotelID, userID);
            }
            else
            {
                h6.ShareLink = string.Format("http://www.zmjiudian.com/hotel/{0}", h6.HotelID);
            }

            h6.ShareLink = AccessAdapter.GenShortUrl(0, h6.ShareLink);

            var firstHotelPackage = priceInfo.PackageList.FirstOrDefault();

            h6.ShareDesc = string.Format("这家不错！￥{0}{1}", firstHotelPackage != null ? firstHotelPackage.Price : h6.MinPrice,
                h6.PriceType < 3 ? "起 " +
                (h6.Intro.Items.Count == 0 ? "" : h6.Intro.Items[0].content.Substring(0, h6.Intro.Items[0].content.Length > 15 ? 15 : h6.Intro.Items[0].content.Length) + "...")
                : " " + GetSharePckageInfoByPrice(h6.HotelID, (int)h6.MinPrice));

            //优点&不足
            h6.Advantages = h6.FeatureList.Select(_ => _.FeaturedName + "-" + _.Comment).ToList();

            //地图位置信息
            HotelMapBasicInfo mapInfo = HotelService.GetHotelMapInfo(h6.HotelID);
            h6.IsInChina = mapInfo.InChina;
            h6.HotelMapUrl = string.Format("http://www.zmjiudian.com/hotel/{0}/map", h6.HotelID);

            //获取缓存的地区名称
            h6.districtName = GetHotelAssociatedZoneName(id, h6.districtID);
            log.AddLog("GetHotelAssociatedZoneName");


            var cacheH5RelComment = GetRelComment(id, userID);
            h6.FollowingCommentCount = cacheH5RelComment.FollowingCommentCount;
            h6.InspectorCommentCount = cacheH5RelComment.InspectorCommentCount;

            h6.RelComments = cacheH5RelComment.RelComments;

            log.AddLog("GetRelComments");

            h6.PriceCinDate = arrivalTime;
            h6.PriceCouDate = departureTime;

            //GetHotelReviewByCommentId
            ReviewResult40 comm = GetHotelReviews40(new ReviewQueryParam() { Hotel = id, RatingType = 0, TFTType = 0, Count = 10, Start = 0, TFTID = 0 }, parentLog: log);
            h6.NewComment = comm.Result.FirstOrDefault();

            log.AddLog("GetHotelReviews40");
            log.Finish();

            //对当前查询的酒店做后台列表价动态更新操作
            HotelAdapter.PublishUpdatePriceSlotTask(id, arrivalTime.ToString("yyyy-MM-dd"));

            return h6;
        }


        /// <summary>
        /// 酒店详情页获取【APP 5.9 USE】
        /// </summary>
        /// <param name="_contextBasicInfo">上下文基本信息</param>
        /// <param name="id"></param>
        /// <param name="checkIn"></param>
        /// <param name="checkOut"></param>
        /// <param name="sType"></param>
        /// <param name="interestid"></param>
        /// <param name="pid"></param>
        /// <param name="commentid"></param>
        /// <returns></returns>
        public HotelItem6 GetHotelDetailAdapter(ContextBasicInfo _contextBasicInfo, int id, string checkIn, string checkOut, string sType, int interestid, int pid = 0, int commentid = 0, long userId = 0)
        {
            var userID = userId == 0 ? _contextBasicInfo.AppUserID : userId;
            var appVer = _contextBasicInfo.AppVer;
            //默认标识不指定入住日期
            var fixDate = false;
            //默认一个日期（明天）
            DateTime arrivalTime = CommMethods.GetDefaultCheckIn();
            DateTime departureTime = arrivalTime.AddDays(1);

            //checkIn非零，则认为指定了列表价日期
            if (!(string.IsNullOrWhiteSpace(checkIn) || checkIn == "0"))
            {
                try
                {
                    arrivalTime = DateTime.Parse(checkIn);
                    departureTime = DateTime.Parse(checkOut);
                    if (departureTime < arrivalTime.AddDays(1)) departureTime = arrivalTime.AddDays(1);

                    //标识指定了列表价日期
                    fixDate = true;
                }
                catch
                {
                }
            }

            TimeLog log = new TimeLog(string.Format("GetHotelDetailAdapter hotelid:{0} checkIn:{1} checkOut:{2} sType:{3} interestid:{4} userID:{5} appVer:{6}", id, checkIn, checkOut, sType, interestid, userID, appVer)
            , 1000, null);

            //初始获取酒店详情实例
            HotelItem6 h6 = GetHotelItem6(id, interestid, appVer, log, _contextBasicInfo);


            h6.PicHeight = 9;
            h6.PicWidth = 16;

            //如果详情页有传进来点评ID，则查询出该店铺的
            List<HotelReviewExEntity> commForId = HotelService.GetHotelReviewByCommentId(commentid).ToList();
            if (commForId != null && commForId.Count > 0)
            {
                FillWHotelCommentOne(commForId);
                h6.CommentForId = transReviewExToCommentItem(commForId, h6.HotelID).FirstOrDefault();
            }

            h6.IsCollection = GenIsCollect(userID, h6.HotelID);

            log.AddLog("GetHotelItem6");

            //初始列表价价格对象
            PackagePriceInfo priceInfo = new PackagePriceInfo()
            {
                MinPrice = 0,
                OTAList = new List<OTAInfo2>(),
                PackageList = new List<PackageItem>(),
                PriceType = 0,
                Tag = "",
                CheckIn = arrivalTime,
                CheckOut = departureTime
            };

            #region 指定了套餐ID，则获取指定套餐的列表价信息

            if (pid > 0)
            {
                //获取指定套餐的信息
                var hotelPackagePrice = HotelService.GetHotelPackageFirstAvailablePriceByPIds(new int[] { pid }, arrivalTime, departureTime);
                if (hotelPackagePrice != null && hotelPackagePrice.Any())
                {
                    //获取当前酒店60天内的可售日历
                    var hotelPackageCalendar = HotelService.GenHotelPackageCalendar(id, arrivalTime, pid, 60);
                    if (hotelPackageCalendar != null && hotelPackageCalendar.Exists(_ => _.SellState == 1))
                    {
                        var minPriceCalendar = new PDayItem();
                        if (fixDate && hotelPackageCalendar.Exists(_ => _.SellState == 1 && _.Day.Date == arrivalTime.Date))
                        {
                            //指定了日期查询
                            minPriceCalendar = hotelPackageCalendar.Where(_ => _.SellState == 1 && _.Day.Date == arrivalTime.Date).First();
                        }
                        else
                        {
                            //找出该套餐最便宜的一个日期的信息
                            minPriceCalendar = hotelPackageCalendar.Where(_ => _.SellState == 1).OrderBy(_ => _.SellPrice).First();
                        }

                        priceInfo.MinPrice = minPriceCalendar.SellPrice;
                        priceInfo.PriceType = 3;//默认显示
                        priceInfo.PackageList.Add(hotelPackagePrice.Select(_ => new PackageItem()
                        {
                            Brief = _.Brief,
                            HotelID = _.HotelID,
                            HotelName = _.HotelName,
                            InterestID = 0,
                            PicUrl = "",
                            PID = _.PID,
                            Price = minPriceCalendar.VipPrice == 0 ? 0 : minPriceCalendar.SellPrice,//vip价格为0 普通价强制设为0 20170106
                            VIPPrice = minPriceCalendar.VipPrice,
                            NightCount = _.PackageType == 0? (departureTime - arrivalTime).Days:0,
                            Title = _.Code,
                            PackageUrl = string.Format("http://www.zmjiudian.com/hotel/package/{0}?userid={1}&checkInStr={2}&checkOutStr={3}&_newpage=1&_newtitle=1&_dorpdown=1", _.PID, userId, minPriceCalendar.Day.ToString("yyyy-MM-dd"), minPriceCalendar.Day.AddDays(1).ToString("yyyy-MM-dd"))
                        }).First());

                        //设置当前价格对应的日期
                        arrivalTime = minPriceCalendar.Day;
                        departureTime = arrivalTime.AddDays(1);
                    }
                }
            }

            #endregion

            #region 没有指定套餐ID，通用列表价获取

            if (priceInfo.MinPrice == 0)
            {
                priceInfo = GetHotelPriceInfo(userID, id, arrivalTime, departureTime, parentLog: log, fixDate: fixDate);
                //将入住日期和离店日期改为最终列表价的日期
                arrivalTime = priceInfo.CheckIn;
                departureTime = priceInfo.CheckOut;
            }

            #endregion

            log.AddLog("GetHotelPriceInfo");

            h6.PackageList = priceInfo.PackageList;

            #region 5.9版本 
            if ((_contextBasicInfo.IsThanVer5_9 || (!_contextBasicInfo.IsIOS && !_contextBasicInfo.IsAndroid)) && h6.PackageList.Count > 0)
            {
                //判断当前用户是否VIP
                var isVip = AccountAdapter.IsVIPCustomer(AccountAdapter.GetCustomerType(userID));
                //非VIP用户
                if (!isVip)
                {
                    h6.Suggest.ActionUrl = "http://www.zmjiudian.com/Coupon/VipShopInfo?userid={userid}&_newpage=1&_isoneoff=1";
                    h6.Suggest.Text = "成为VIP>>";
                    h6.Suggest.Description = "，付199元年费可";
                    //5.6.2版本后，文案调整
                    if (_contextBasicInfo.IsThanVer5_7)
                    {
                        h6.Suggest.Description = "付199元年费可";
                    }
                }

                #region 追加套餐内容

                foreach (PackageItem p in h6.PackageList)
                {
                    if (p.PID > 0)
                    {
                        List<SameSerialPackageItem> sameSerialList = HotelService.GetSameSerialPackageEntityList(p.PID).Select(_ => new SameSerialPackageItem()
                        {
                            pId = _.pId,
                            roomTypeId = _.roomTypeId,
                            serialNo = _.serialNo,
                            roomTypeName = _.roomTypeName,
                            roomDesc = _.roomDesc
                        }).Where(_ => _.pId == p.PID).ToList();

                        List<string> packageContent = new List<string>();
                        List<string> PackageNotice = new List<string>();
                        string strbuildContent = "";
                        if (sameSerialList.Count > 0)
                        {
                            strbuildContent = p.NightCount + "晚 " + sameSerialList.FirstOrDefault().roomDesc;
                            packageContent.Add(strbuildContent);
                        }

                        List<PackageInfoEntity> packagePrice = new PackageAdapter().GetHotelPackages(id, arrivalTime, departureTime, pid: p.PID);
                        if (packagePrice.Count > 0)
                        {
                            PriceAdapter.AddMorePackageInfo(packagePrice);
                            packageContent.AddRange(packagePrice[0].Items.FindAll(_ => _.Type == 1).Select(_ => _.Description).ToList());
                            p.packageContent = packageContent;
                            p.packageNotice = packagePrice[0].Items.FindAll(_ => _.Type == 2).Select(_ => _.Description).ToList();
                            p.NightCount = packagePrice[0].packageBase.WHPackageType == 0 ? p.NightCount : 0;//机酒、游轮套餐 间夜数显示为0
                        }   
                    }
                }

                #endregion

            } 
            #endregion

            priceInfo.OTAList.ForEach(_ => h6.OTAList.Add(new OTAInfo2()
            {
                AccessURL = _.AccessURL,
                CanSyncPrice = _.CanSyncPrice,
                ChannelID = _.ChannelID,
                Name = _.Name,
                OTAHotelID = _.OTAHotelID,
                OtaTransferURL = _.OtaTransferURL,
                Price = _.VIPPrice == 0 ? 0 : _.Price,
                PriceBrief = _.PriceBrief,
                PriceName = _.PriceName,
                RoomRates = _.RoomRates,
                VIPPrice = _.VIPPrice
            }));
            //h6.OTAList = priceInfo.OTAList;
            GenOtaTransferURL(h6.OTAList, id, arrivalTime.ToString("yyyy-MM-dd"), departureTime.ToString("yyyy-MM-dd"));//替换OTAList里面的日期价格

            h6.PriceType = priceInfo.PriceType;
            h6.MinPrice = priceInfo.MinPrice;

            h6.RecommendHotelList = GenRecommendHotelList(id, userID, interestid, arrivalTime, departureTime, parentLog: log);
            log.AddLog("GenRecommendHotelList");

            //生成酒店分享的 标题和链接
            h6.ShareTitle = h6.HotelName;
            if (userID > 0)
            {
                //追踪码生成 点评ID + 英文半角逗号 + 分享者UserID DES加密后传输 防止url篡改
                string desBase64Str = HJDAPI.Common.Security.DES.Encrypt(string.Format("{0},{1}", h6.HotelID, userID));
                h6.ShareLink = string.Format("http://www.zmjiudian.com/hotel/{0}?CID={1}", h6.HotelID, userID);
            }
            else
            {
                h6.ShareLink = string.Format("http://www.zmjiudian.com/hotel/{0}", h6.HotelID);
            }

            //h6.ShareLink = AccessAdapter.GenShortUrl(0, h6.ShareLink);

            var firstHotelPackage = priceInfo.PackageList.FirstOrDefault();

            h6.ShareDesc = string.Format("这家不错！￥{0}{1}", firstHotelPackage != null ? firstHotelPackage.Price : h6.MinPrice,
                h6.PriceType < 3 ? "起 " +
                (h6.Intro.Items.Count == 0 ? "" : h6.Intro.Items[0].content.Substring(0, h6.Intro.Items[0].content.Length > 15 ? 15 : h6.Intro.Items[0].content.Length) + "...")
                : " " + GetSharePckageInfoByPrice(h6.HotelID, (int)h6.MinPrice));

            //优点&不足
            h6.Advantages = h6.FeatureList.Select(_ => _.FeaturedName + "-" + _.Comment).ToList();

            //地图位置信息
            HotelMapBasicInfo mapInfo = HotelService.GetHotelMapInfo(h6.HotelID);
            h6.IsInChina = mapInfo.InChina;
            h6.HotelMapUrl = string.Format("http://www.zmjiudian.com/hotel/{0}/map", h6.HotelID);

            //获取缓存的地区名称
            h6.districtName = GetHotelAssociatedZoneName(id, h6.districtID);
            log.AddLog("GetHotelAssociatedZoneName");

            h6.PriceCinDate = arrivalTime;
            h6.PriceCouDate = departureTime;
            if (_contextBasicInfo.IsThanVer5_9 || (!_contextBasicInfo.IsIOS && !_contextBasicInfo.IsAndroid))
            {
                if (userID > 0)
                {
                    ///5.9版本取 关注人发表的最新一条评论
                    int totalFollowingCommentCount = 0;
                    List<CommentInfoEntity> commentList = CommentAdapter.GetCommentInfosByFollowing(userID, id,
                        new int[] { 1 }, out totalFollowingCommentCount, 0, 10);
                    if (commentList != null && commentList.Count != 0)
                    {
                        h6.FriendComment = CommentAdapter.GenCommentItems(commentList).FirstOrDefault();
                    }
                    h6.FollowingCommentCount = totalFollowingCommentCount;

                    var userIds = commentList.Select(_ => _.Comment.UserID);
                    var userInfos = AccountAdapter.GetUserBasicInfo(userIds);

                    h6.RelComments = (from i in commentList
                                      join j in userInfos on i.Comment.UserID equals j.UserId
                                      select new HotelRelComment()
                                      {
                                          Author = j.NickName,
                                          AvatarUrl = string.IsNullOrWhiteSpace(j.AvatarUrl) ? DescriptionHelper.defaultAvatar.Replace("_jupiter", "_small") : PhotoAdapter.GenHotelPicUrl(j.AvatarUrl, Enums.AppPhotoSize.small),
                                          AuthorUserID = j.UserId,
                                          Title = ""
                                      }).Distinct().ToList();
                }
            }
            else
            {
                var cacheH5RelComment = GetRelComment(id, userID);
                h6.FollowingCommentCount = cacheH5RelComment.FollowingCommentCount;
                h6.InspectorCommentCount = cacheH5RelComment.InspectorCommentCount;

                //GetHotelReviewByCommentId
                ReviewResult40 comm = GetHotelReviews40(new ReviewQueryParam() { Hotel = id, RatingType = 0, TFTType = 0, Count = 10, Start = 0, TFTID = 0 }, parentLog: log);
                h6.NewComment = comm.Result.FirstOrDefault();

                h6.RelComments = cacheH5RelComment.RelComments;

                log.AddLog("GetRelComments");
            }

            log.AddLog("GetHotelReviews40");
            log.Finish();

            //对当前查询的酒店做后台列表价动态更新操作
            HotelAdapter.PublishUpdatePriceSlotTask(id, arrivalTime.ToString("yyyy-MM-dd"));

            return h6;
        }


        public bool GenIsCollect(long userID,long hotelID)
        {
            bool isCollect = false;

            if (userID > 0)
            {
                try
                {
                    CollectParam param = new CollectParam();
                    param.UserID = userID;
                    List<FavouriteHotel> collectHotelList = CollectAdapter.GetCollectList(param);
                    if (collectHotelList.Count > 0)
                    {
                        isCollect = collectHotelList.Where(_ => _.HotelID == hotelID).FirstOrDefault() != null ? true : false;
                    }
                }
                catch (Exception e)
                {
                    Log.WriteLog("收藏 " + e);
                }
            }
            return isCollect;
        }
        /// <summary>
        /// Magicall流程中使用的酒店详情页获取
        /// </summary>
        /// <param name="id"></param>
        /// <param name="checkIn"></param>
        /// <param name="checkOut"></param>
        /// <param name="sType"></param>
        /// <param name="interestid"></param>
        /// <param name="userID"></param>
        /// <param name="appVer"></param>
        /// <param name="pid"></param>
        /// <returns></returns>
        public HotelItem6 GetSimple60(int id, string checkIn, string checkOut, string sType, int interestid, long userID = 0, string appVer = "", int pid = 0, int commentid = 0)
        {
            //默认标识不指定入住日期
            var fixDate = false;

            //默认一个日期（明天）
            DateTime arrivalTime = CommMethods.GetDefaultCheckIn();
            DateTime departureTime = arrivalTime.AddDays(1);

            //checkIn非零，则认为指定了列表价日期
            if (!(string.IsNullOrWhiteSpace(checkIn) || checkIn == "0"))
            {
                try
                {
                    arrivalTime = DateTime.Parse(checkIn);
                    departureTime = DateTime.Parse(checkOut);
                    if (departureTime < arrivalTime.AddDays(1)) departureTime = arrivalTime.AddDays(1);

                    //标识指定了列表价日期
                    fixDate = true;
                }
                catch
                {

                }
            }

            TimeLog log = new TimeLog(string.Format("Get60 hotelid:{0} checkIn:{1} checkOut:{2} sType:{3} interestid:{4} userID:{5} appVer:{6}", id, checkIn, checkOut, sType, interestid, userID, appVer)
            , 2000, null);

            //初始获取酒店详情实例
            HotelItem6 h6 = GetHotelItem6(id, interestid, appVer, log);

            //List<HotelReviewExEntity> commForId = HotelService.GetHotelReviewByCommentId(commentid).ToList();
            //if (commForId != null && commForId.Count > 0)
            //{
            //    FillWHotelCommentOne(commForId);
            //    h6.CommentForId = transReviewExToCommentItem(commForId, h6.HotelID).FirstOrDefault();
            //}


            //bool isCollect = false;
            //if (userID != 0)
            //{
            //    try
            //    {
            //        CollectParam param = new CollectParam();
            //        param.UserID = userID;
            //        List<FavouriteHotel> collectHotelList = CollectAdapter.GetCollectList(param);
            //        if (collectHotelList.Count > 0)
            //        {
            //            isCollect = collectHotelList.Where(_ => _.HotelID == h6.HotelID).FirstOrDefault() != null ? true : false;
            //        }
            //    }
            //    catch (Exception e)
            //    {
            //        Log.WriteLog("收藏" + e);
            //    }
            //}

            log.AddLog("GetHotelItem6");

            //初始列表价价格对象
            PackagePriceInfo priceInfo = new PackagePriceInfo()
            {
                MinPrice = 0,
                OTAList = new List<OTAInfo2>(),
                PackageList = new List<PackageItem>(),
                PriceType = 0,
                Tag = "",
                CheckIn = arrivalTime,
                CheckOut = departureTime
            };

            #region 没有指定套餐ID，通用列表价获取

            if (priceInfo.MinPrice == 0)
            {
                priceInfo = GetHotelPriceInfo(userID, id, arrivalTime, departureTime, parentLog: log, fixDate: fixDate);

                //将入住日期和离店日期改为最终列表价的日期
                arrivalTime = priceInfo.CheckIn;
                departureTime = priceInfo.CheckOut;
            }

            #endregion

            log.AddLog("GetHotelPriceInfo");

            h6.PackageList = priceInfo.PackageList;
            priceInfo.OTAList.ForEach(_ => h6.OTAList.Add(new OTAInfo2()
            {
                AccessURL = _.AccessURL,
                CanSyncPrice = _.CanSyncPrice,
                ChannelID = _.ChannelID,
                Name = _.Name,
                OTAHotelID = _.OTAHotelID,
                OtaTransferURL = _.OtaTransferURL,
                Price = _.VIPPrice == 0 ? 0 : _.Price,
                PriceBrief = _.PriceBrief,
                PriceName = _.PriceName,
                RoomRates = _.RoomRates,
                VIPPrice = _.VIPPrice
            }));
            //h6.OTAList = priceInfo.OTAList;
            GenOtaTransferURL(h6.OTAList, id, arrivalTime.ToString("yyyy-MM-dd"), departureTime.ToString("yyyy-MM-dd"));//替换OTAList里面的日期价格

            h6.PriceType = priceInfo.PriceType;
            h6.MinPrice = priceInfo.MinPrice;

            var firstHotelPackage = priceInfo.PackageList.FirstOrDefault();
            h6.Advantages = h6.FeatureList.Select(_ => _.FeaturedName + "-" + _.Comment).ToList();

            int followingCommentCount = 0;
            int inspectorCommentCount = 0;
            var relCommentInfos = new List<CommentInfoEntity>();
            if (userID != 0)
            {
                relCommentInfos = CommentAdapter.GetCommentInfosByFollowing(userID, id, new int[] { 1 },
                    out followingCommentCount, 0, 6);
            }
            if (followingCommentCount == 0)
            {
                relCommentInfos = CommentAdapter.GetCommentInfosByInspector(id, new int[] { 1 },
                    out inspectorCommentCount, 0, 6);
            }

            h6.FollowingCommentCount = followingCommentCount;
            h6.InspectorCommentCount = inspectorCommentCount;

            var userIds = relCommentInfos.Select(_ => _.Comment.UserID);
            var userInfos = AccountAdapter.GetUserBasicInfo(userIds);
            log.Finish();

            //对当前查询的酒店做后台列表价动态更新操作
            HotelAdapter.PublishUpdatePriceSlotTask(id, arrivalTime.ToString("yyyy-MM-dd"));

            return h6;
        }
        #region 映射酒店的hotelfilter cat 与 reviewfilter的 cat 之间的区别
        public int MapHotelFilterCat2ReviewFilter(int hotelfilterCat)
        {
            switch (hotelfilterCat)
            {
                case 1:
                    return 1;
                case 2:
                    return 2;
                case 3:
                    return 3;
                case 4:
                    return 4;
                case 5:
                    return 5;
                case 6:
                    return 6;
                default:
                    return 0;
            }
        }
        #endregion

        internal static void SetHotelAssociatedZoneName(int hotelId, string zoneName)
        {
            DistrictZoneEntity dz = new DistrictZoneEntity()
            {
                Name = zoneName
            };
            LocalCache.Set(string.Format("HotelAssociatedZone:{0}", hotelId.ToString()), dz);
        }

        internal static string GetHotelAssociatedZoneName(int hotelId, int districtId)
        {
            var result = "";
            var keyName = string.Format("HotelAssociatedZone:{0}", hotelId.ToString());
            var data = LocalCache.GetData<DistrictZoneEntity>(keyName, () =>
            {
                var districtName = "";
                if (districtId > 0)
                {
                    var allCitys = GetZMJDAllCityData().Citys;
                    districtName = allCitys != null && allCitys.Exists(_ => _.ID == districtId) ? allCitys.First(_ => _.ID == districtId).Name : "";
                }
                return new DistrictZoneEntity()
                {
                    Name = districtName
                };
            });
            return data != null ? data.Name : result;
        }

        /// <summary>
        /// 模板:http://www.zmjiudian.com/hotel/OtaTransfer?hotelId={0}&checkIn={1}&checkOut={2}
        /// </summary>
        private void GenOtaTransferURL(List<OTAInfo2> OTAList, int hotelId, string checkIn, string checkOut)
        {
            if (OTAList == null || OTAList.Count == 0)
            {
                return;
            }
            else
            {
                foreach (var ota in OTAList)
                {
                    ota.OtaTransferURL = string.Format("http://www.zmjiudian.com/hotel/OtaTransfer?hotelId={0}&checkIn={1}&checkOut={2}", hotelId, checkIn, checkOut);
                }
            }
        }

        public HotelItem5 Get50Test(int id, string checkIn, string checkOut, string sType, int interestid)
        {
            DateTime arrivalTime = CommMethods.GetDefaultCheckIn();
            DateTime departureTime = arrivalTime.AddDays(1);

            if (!(string.IsNullOrWhiteSpace(checkIn) || checkIn == "0"))
            {
                try
                {
                    arrivalTime = DateTime.Parse(checkIn);
                    departureTime = DateTime.Parse(checkOut);
                    if (departureTime < arrivalTime.AddDays(1)) departureTime = arrivalTime.AddDays(1);
                }
                catch
                {

                }
            }

            #region

            var otaList = new List<OTAInfo2>();
            PackagePriceInfo ppi = new PackagePriceInfo();
            ppi.PackageList = GenHotelPricePlanList(0, id, ref arrivalTime, ref departureTime, ref otaList);
            ppi.PackageList = ppi.PackageList.Take(1).ToList();

            #endregion

            HotelItem5 h5 = GetHotelItem5(id, interestid);

            //缓存1
            PackagePriceInfo priceInfo = GetHotelPriceInfo(0, id, arrivalTime, departureTime);

            h5.PackageList = priceInfo.PackageList;
            h5.OTAList = priceInfo.OTAList;
            h5.PriceType = priceInfo.PriceType;
            h5.MinPrice = priceInfo.MinPrice;//MinPrice和PackageList里面的最低价不是一回事

            var firstHotelPackage = priceInfo.PackageList.FirstOrDefault();
            //缓存2
            h5.RecommendHotelList = GenRecommendHotelList(id, 0, interestid, arrivalTime, departureTime);

            h5.ShareDesc = string.Format("这家不错！￥{0}{1}", firstHotelPackage != null ? firstHotelPackage.Price : h5.MinPrice,
                h5.PriceType < 3 ? "起 " +
                (h5.Intro.Items.Count == 0 ? "" : h5.Intro.Items[0].content.Substring(0, h5.Intro.Items[0].content.Length > 15 ? 15 : h5.Intro.Items[0].content.Length) + "...")
                : " " + GetSharePckageInfoByPrice(h5.HotelID, (int)h5.MinPrice));

            //优点 不足
            h5.Advantages = h5.FeatureList.Select(_ => _.FeaturedName + "-" + _.Comment).ToList();

            return h5;
        }

        /// <summary>
        /// 不同系列套餐 只出两条
        /// </summary>
        /// <param name="id"></param>
        /// <param name="arrivalTime"></param>
        /// <param name="departureTime"></param>
        /// <param name="otaList"></param>
        /// <returns></returns>
        private static List<PackageItem> GenHotelPackageList(int id, ref DateTime arrivalTime, ref DateTime departureTime, ref List<OTAInfo2> otaList)
        {
            List<PackageItem> packageList = new List<PackageItem>();

            string checkIn = arrivalTime == DateTime.MinValue ? "0" : arrivalTime.ToShortDateString();
            string checkOut = departureTime == DateTime.MinValue ? "0" : departureTime.ToShortDateString();

            HotelPrice2 hp = PriceAdapter.Get3(id, checkIn, checkOut, "ios", true);

            //一定要有可卖的套餐 没有不管了
            if (hp != null && hp.Packages != null && hp.Packages.Count != 0 && hp.Packages.Exists(_ => _.SellState == 1))
            {
                hp.Packages = hp.Packages.OrderBy(_ => _.SellState).ThenBy(_ => _.Price).ToList();//找出可售的 最便宜的价格
                PackageInfoEntity pie = hp.Packages.First();
                PackageEntity pe = pie.packageBase;
                if (string.IsNullOrWhiteSpace(pe.Brief))
                {
                    pe.Brief = hp.Packages.First().DailyItems.First().Items.FindAll(_ => _.Type == 1).First().Description;
                }
                packageList.Add(new PackageItem
                {
                    Brief = pe.Brief,
                    Title = pe.Code,
                    PID = pe.ID,
                    Price = pie.Price
                });

                //只出两条，但不同系列 string SerialNO
                //for (int i = 1; i < hp.Packages.Count(); i++)
                //{
                //    if (hp.Packages[i].packageBase.SerialNO != pe.SerialNO)
                //    {
                //        if (string.IsNullOrWhiteSpace(hp.Packages[i].packageBase.Brief))
                //        {
                //            hp.Packages[i].packageBase.Brief = hp.Packages[i].DailyItems.First().Items.FindAll(_ => _.Type == 1).First().Description;
                //        }
                //        packageList.Add(new PackageItem
                //        {
                //            Brief = hp.Packages[i].packageBase.Brief,
                //            Title = hp.Packages[i].packageBase.Code,
                //            PID = hp.Packages[i].packageBase.ID,
                //            Price = hp.Packages[i].Price
                //        });
                //        break;
                //    }
                //}

                arrivalTime = hp.CheckIn;
                departureTime = hp.CheckOut;
            }

            //otaList
            otaList = hp.OTAList ?? new List<OTAInfo2>();

            return packageList;
        }

        /// <summary>
        /// 查询酒店指定时间段的列表价及价格套餐信息
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="id"></param>
        /// <param name="arrivalTime"></param>
        /// <param name="departureTime"></param>
        /// <param name="otaList"></param>
        /// <param name="fixDate">是否指定日期（默认不指定）</param>
        /// <returns></returns>
        private static List<PackageItem> GenHotelPricePlanList(long userId, int id, ref DateTime arrivalTime, ref DateTime departureTime, ref List<OTAInfo2> otaList, bool fixDate = false)
        {
            List<PackageItem> packageList = new List<PackageItem>();

            var hotelMinPrice = new HotelMinPriceEntity();
            var refCheckIn = arrivalTime;

            //新的直接读取列表价的方式
            PriceAdapter.GetPricePlanAndOtaList(userId, id, ref refCheckIn, ref hotelMinPrice, ref otaList, fixDate);
            if (hotelMinPrice.Price > 0 && hotelMinPrice.VipPrice > 0)
            {
                var _packageItem = new PackageItem
                {
                    Brief = hotelMinPrice.Brief,
                    Title = hotelMinPrice.Name,
                    PID = 0,
                    Price = hotelMinPrice.VipPrice == 0 ? 0 : hotelMinPrice.Price, //vipPrice 为0 普通价强制改为0 2017 01 11
                    VIPPrice = hotelMinPrice.VipPrice,
                    ChannelID = hotelMinPrice.ChannelID,
                    NightCount = hotelMinPrice.NightCount,
                    ForVIPFirstBuy = false,
                    PackageUrl = "",
                    PackageLables = new List<string>(),
                    packageContent = new List<string>(),
                    packageNotice = new List<string>()
                };

                //跳转到指定套餐详情的url
                if (hotelMinPrice.PID > 0)
                {
                    _packageItem.PID = hotelMinPrice.PID;
                    _packageItem.PackageUrl = string.Format("http://www.zmjiudian.com/hotel/package/{0}?userid={1}&checkInStr={2}&checkOutStr={3}&_newpage=1&_newtitle=1&_dorpdown=1", hotelMinPrice.PID, userId, refCheckIn.ToString("yyyy-MM-dd"), refCheckIn.AddDays(hotelMinPrice.NightCount).ToString("yyyy-MM-dd"));
                }

                //如果是新VIP专享套餐，则显示一个labelicon
                if ((HotelMinPriceType)hotelMinPrice.Type == HotelMinPriceType.NewVip)
                {
                    _packageItem.ForVIPFirstBuy = true;
                    _packageItem.PackageLables.Add("http://whfront.b0.upaiyun.com/app/img/icon/new_vip.png");
                }

                packageList.Add(_packageItem);

                arrivalTime = refCheckIn;
                departureTime = refCheckIn.AddDays(hotelMinPrice.NightCount);
            }

            return packageList;
        }

        /// <summary>
        /// 获取指定酒店的列表价信息（可指定日期也可不指定）
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="id"></param>
        /// <param name="arrivalTime"></param>
        /// <param name="departureTime"></param>
        /// <param name="parentLog"></param>
        /// <param name="fixDate">是否指定入住日期（默认不指定）</param>
        /// <returns></returns>
        public static PackagePriceInfo GetHotelPriceInfo(long userId, int id, DateTime arrivalTime, DateTime departureTime, TimeLog parentLog = null, bool fixDate = false)
        {
            PackagePriceInfo ppi = new PackagePriceInfo();

            //log engine
            TimeLog log = new TimeLog("GetHotelItem5", 500, parentLog);

            if (arrivalTime == DateTime.MinValue || departureTime == DateTime.MinValue)
            {
                arrivalTime = CommMethods.GetDefaultCheckIn();
                departureTime = arrivalTime.AddDays(1);
            }

            //获取列表价方法   
            var otaList = new List<OTAInfo2>();
            ppi.PackageList = GenHotelPricePlanList(userId, id, ref arrivalTime, ref departureTime, ref otaList, fixDate);
            ppi.PackageList = ppi.PackageList.Take(1).ToList();//2015-06-26 tracy 要求50版本的详情页:特惠套餐只出最便宜的一条；房间部分:没有图片的房型不出现

            //otaList
            ppi.OTAList = otaList;

            log.AddLog("GenHotelPricePlanList");

            if (ppi.PackageList != null && ppi.PackageList.Count > 0)
            {
                var nearestDatePrice = ppi.PackageList.First();
                ppi.MinPrice = nearestDatePrice.Price;
                ppi.PriceType = nearestDatePrice.ChannelID == 100 ? 3 : 2;
            }
            else
            {
                if (ppi.OTAList != null && ppi.OTAList.Count > 0)
                {
                    ppi.MinPrice = ppi.OTAList.Min(ota => ota.Price);
                    ppi.PriceType = 1;
                    ppi.Tag = "";
                }
                else
                {
                    ppi.MinPrice = 0;
                    ppi.PriceType = 0;
                    ppi.Tag = "";
                }
            }

            //2016.06.16 add: if minPrice equals zero, then set the minPrice equals defaultPrice (baseprice)
            if (ppi.MinPrice == 0)
            {
                var defaultPrice = PriceAdapter.QueryHotelBasePrice(new List<int> { id }).FirstOrDefault();
                ppi.MinPrice = defaultPrice != null && defaultPrice.MinPrice > 0 ? defaultPrice.MinPrice : 0;
                log.AddLog(string.Format(" PriceAdapter.QueryHotelBasePrice:id:{0} ", id));
            }

            log.Finish();

            ppi.CheckIn = arrivalTime;
            ppi.CheckOut = departureTime;

            return ppi;
        }

        private string GetSharePckageInfoByPrice(int hotelid, int price)
        {
            return LocalCache.GetData<string>("ZMJDCityData:" + hotelid.ToString() + ":" + price.ToString(), () =>
            {
                PackageEntity pe = HotelService.GetSharePckageInfoByPrice(hotelid, price);
                if (pe != null)
                {
                    return pe.Brief == null ? "" : pe.Brief;
                }
                else
                {
                    return "";
                }
            });
        }

        private HotelItem3 GetHotelItem3(int id, int interestid)
        {
            HotelItem3 hi3 = new HotelItem3();
            HotelItem hi = ResourceAdapter.GetHotel(id, UserId);
            List<Hotel3Entity> h3s = HotelService.GetHotel3(id);
            hi3.RestaurentList = HotelService.GetHotelRestaurantList(id);
            hi3.SightList = HotelService.GetHotelSightList(id);
            hi3.Address = hi.Address;
            hi3.Facilities = hi.Facilities;
            hi3.GLat = hi.GLat;
            hi3.GLon = hi.GLon;
            hi3.HotelID = id;
            hi3.Name = hi.Name;
            hi3.districtID = hi.DistrictId;
            hi3.districtName = hi.DistrictName;
            hi3.Tel = hi.Telephone;
            hi3.Score = hi.Score;
            hi3.ReviewCount = hi.ReviewCount;
            HotelPhotosEntity hps = GetHotelPhotos(hi.Id, interestid);
            hi3.PicCount = hps.HPList.Count();
            hi3.Pics = hps.HPList.Take(5).Select(p => PhotoAdapter.GenHotelPicUrl(p.SURL, Enums.AppPhotoSize.appdetail)).ToList();
            hi3.Star = hi.Star;
            hi3.OpenYear = hi.OpenYear.Year > 1901 ? string.Format("{0}年", hi.OpenYear.Year) : "";
            hi3.packagePriceURL = string.Format("http://m1.zmjiudian.com/{0}/price", hi3.HotelID);
            hi3.InterestID = interestid;

            List<HotelInterestTagEntity> tftList = HotelService.GetHotelInterestTag(id, interestid);
            if (tftList.Count > 0)
            {
                for (int i = 0; i < (tftList.Count > 3 ? 3 : tftList.Count); i++)
                {
                    hi3.Featrues.Add(tftList[i].TFTName);
                }

                hi3.InterestID = tftList.First().InterestID;

            }

            hi3.InterestName = ResourceAdapter.GetInterestName(hi3.InterestID);


            foreach (Hotel3Entity h3 in h3s)
            {
                switch (h3.Type)
                {
                    case 1:
                        hi3.Intro = GenHotelInfo(h3);
                        break;
                    case 2:
                        hi3.Interest = GenHotelInfo(h3);
                        break;
                    case 3:
                        hi3.Food = GenHotelInfo(h3);
                        break;
                    case 4:
                        hi3.Environment = GenHotelInfo(h3);
                        break;
                    case 5:
                        hi3.RoomFacilities = GenHotelInfo(h3);
                        break;
                    case 6:
                        hi3.ArrivalAndDeparture = GenHotelInfo(h3);
                        break;
                }
            }

            return hi3;
        }

        private HotelItem4 GetHotelItem4(int id, int interestid)
        {
            HotelItem4 hi4 = new HotelItem4();
            HotelItem hi = ResourceAdapter.GetHotel(id, UserId);
            List<Hotel3Entity> h3s = HotelService.GetHotel3(id);
            hi4.RestaurentList = HotelService.GetHotelRestaurantList(id).Where(r => !r.IsInner).Take(3).ToList(); //不显示酒店内餐馆，只显示三条
            hi4.SightList = new List<HotelSightEntity>();// HotelService.GetHotelSightList(id);  //暂不显示
            hi4.Address = hi.Address;
            hi4.Facilities = hi.Facilities;
            hi4.GLat = hi.GLat;
            hi4.GLon = hi.GLon;
            hi4.HotelID = id;
            hi4.Name = hi.Name;
            hi4.districtID = hi.DistrictId;
            hi4.districtName = hi.DistrictName;
            hi4.Tel = hi.Telephone;
            hi4.Score = hi.Score;
            hi4.ReviewCount = hi.ReviewCount;
            HotelPhotosEntity hps = GetHotelPhotos(hi.Id, interestid);
            hi4.PicCount = hps.HPList.Count();
            hi4.Pics = hps.HPList.Take(5).Select(p => PhotoAdapter.GenHotelPicUrl(p.SURL, Enums.AppPhotoSize.appdetail)).ToList();
            hi4.Star = hi.Star;
            hi4.OpenYear = hi.OpenYear.Year > 1901 ? string.Format("{0}年", hi.OpenYear.Year) : "";
            hi4.ReBuildInfo = hi.ReBuildInfo;
            hi4.packagePriceURL = string.Format("http://m1.zmjiudian.com/{0}/price", hi4.HotelID);
            hi4.InterestID = interestid;



            List<FeaturedCommentEntity> fl = HotelService.GetHotelFeaturedCommentInfo(id);

            if (fl.Count > 0)
            {
                //如果有与玩点（interestid)相关的点评，那么这类点评排在前面                 
                if (interestid > 0 && fl.Where(t => t.RelInterestID == interestid).Count() > 0)
                {
                    fl.Where(t => t.RelInterestID == interestid).First().TagType = 0;
                }
                hi4.FeatureList = fl.Where(f => f.CategoryID == 16).OrderBy(f => f.TagType).ThenByDescending(f => f.CommentCount).Take(3).ToList();
                hi4.EntertainmentList = fl.Where(f => f.CategoryID == 17).OrderByDescending(f => f.CommentCount).Take(3).ToList();

                hi4.FoodComment = fl.Where(f => f.CategoryID == 18).OrderByDescending(f => f.CommentCount).Take(3).ToList();
                hi4.RoomComment = fl.Where(f => f.CategoryID == 19).OrderByDescending(f => f.CommentCount).Take(3).ToList();
            }

            hi4.InterestName = ResourceAdapter.GetInterestName(hi4.InterestID);


            foreach (Hotel3Entity h3 in h3s)
            {
                switch (h3.Type)
                {
                    case 1:
                        hi4.Intro = GenHotelInfo(h3);

                        break;
                    case 2:
                        hi4.Interest = new HotelInfo();//  GenHotelInfo(h3); 
                        break;
                    case 3:
                        hi4.Food = GenHotelInfo(h3);
                        break;
                    case 4:
                        hi4.Environment = GenHotelInfo(h3);
                        break;
                    case 5:
                        hi4.RoomFacilities = new HotelInfo();//GenHotelInfo(h3);
                        break;
                    case 6:
                        hi4.ArrivalAndDeparture = GenHotelInfo(h3);
                        break;
                }
            }

            return hi4;
        }

        private HotelItem5 GetHotelItem5(int id, int interestid, string appVer = "", TimeLog parentLog = null)
        {
            TimeLog log = new TimeLog("GetHotelItem5", 200, parentLog);

            HotelItem5 hi5 = GenBasicHotelItem5(id, interestid);

            log.AddLog("GenBasicHotelItem5");

            GenFeaturedCommentData(hi5, interestid, appVer, log);
            log.AddLog("GenFeaturedCommentData");

            if (string.IsNullOrWhiteSpace(appVer) || appVer.CompareTo("4.4") < 0)
            {
                List<HotelFacilityEntity> facilitys = HotelService.GetHotelFacilitysByHotelID(id);
                hi5.Facilities = facilitys != null && facilitys.Count > 0 ? facilitys.Select(_ => new HotelFacilityEntity()
                {
                    Facility = _.Facility,
                    FacilityName = _.FacilityName,
                    IsAvailable = true,
                    HotelID = _.HotelID,
                    Cat = _.Cat,
                    isValid = _.isValid
                }).ToList() : new List<HotelFacilityEntity>();

                log.AddLog("GetHotelFacilitysByHotelID");
            }
            else
            {
                var officialPicsCount = 0;
                var officialPics = GetHotelPicFromOfficial(id, 0, 20, out officialPicsCount);
                hi5.RelPicData.officialPicCount = officialPicsCount;
                hi5.RelPicData.officialPics = officialPics;
                log.AddLog("GetHotelPicFromOfficial");
                var customerPicsCount = 0;//取客户照片
                var customerPics = GetHotelPicFromCustomer(id, 0, 50, out customerPicsCount);

                hi5.RelPicData.customerPicCount = customerPicsCount;
                hi5.RelPicData.customerPics = customerPics;
                log.AddLog("GetHotelPicFromCustomer");

            }
            log.Finish();
            return hi5;
        }

        private HotelItem6 GetHotelItem6(int id, int interestid, string appVer = "", TimeLog parentLog = null, ContextBasicInfo _contextBasicInfo = null)
        {
            TimeLog log = new TimeLog("GetHotelItem6", 200, parentLog);

            HotelItem6 hi6 = GenBasicHotelItem6(id, interestid, _contextBasicInfo);

            log.AddLog("GenBasicHotelItem6");

            GenFeaturedCommentData60(hi6, interestid, appVer, log);
            log.AddLog("GenFeaturedCommentData");

            hi6.RelPicData = new HotelRelPicData();

            #region 5.9版本拿掉 酒店关联照片
            //var officialPicsCount = 0;
            //var officialPics = GetHotelPicFromOfficial(id, 0, 20, out officialPicsCount);
            //hi6.RelPicData.officialPicCount = officialPicsCount;
            //hi6.RelPicData.officialPics = officialPics;
            //log.AddLog("GetHotelPicFromOfficial");
            //var customerPicsCount = 0;//取客户照片
            //var customerPics = GetHotelPicFromCustomer(id, 0, 50, out customerPicsCount);

            //hi6.RelPicData.customerPicCount = customerPicsCount;
            //hi6.RelPicData.customerPics = customerPics;
            //log.AddLog("GetHotelPicFromCustomer"); 
            #endregion

            log.Finish();
            return hi6;
        }


        /// <summary>
        /// 获取基本酒店详情数据
        /// </summary>
        /// <param name="id"></param>
        /// <param name="UserId"></param>
        /// <param name="interestid"></param>
        /// <returns></returns>
        private HotelItem5 GenBasicHotelItem5(int id, int interestid)
        {
            var basicInfo = new HotelItem5();
            HotelItem hi = ResourceAdapter.GetHotel(id, UserId);
            List<Hotel3Entity> h3s = HotelService.GetHotel3(id);

            basicInfo.RestaurentList = new List<HotelRestaurantEntity>();//HotelService.GetHotelRestaurantList(id).Where(r => !r.IsInner).Take(3).ToList(); //不显示酒店内餐馆，只显示三条

            basicInfo.SightList = HotelService.GetHotelSightList(id);//new List<HotelSightEntity>(); //暂不显示
            if (basicInfo.SightList != null && basicInfo.SightList.Count != 0)
            {
                basicInfo.SightList.ForEach((_) =>
                {
                    _.DistanceDescription = _.Distance >= 1000 ? string.Format("距酒店{0}公里", Math.Round(_.Distance / 1000, 1)) : string.Format("距酒店{0}米", _.Distance.ToString("0"));
                });
            }

            basicInfo.GLat = hi.GLat;
            basicInfo.GLon = hi.GLon;
            basicInfo.HotelID = id;
            basicInfo.HotelName = hi.Name;
            basicInfo.districtID = hi.DistrictId;
            basicInfo.districtName = hi.DistrictName;
            basicInfo.Tel = hi.Telephone;
            basicInfo.Score = hi.Score;
            basicInfo.ReviewCount = hi.ReviewCount;

            HotelPhotosEntity hps = GetHotelPhotos(hi.Id, interestid);
            basicInfo.PicCount = hps.HPList.Count();
            basicInfo.Pics = hps.HPList.Take(5).Select(p => PhotoAdapter.GenHotelPicUrl(p.SURL, Enums.AppPhotoSize.theme)).ToList();//列表页图片尺寸与详情页一致

            basicInfo.Star = hi.Star;
            basicInfo.OpenYear = hi.OpenYear.Year > 1901 ? string.Format("{0}年", hi.OpenYear.Year) : "";
            basicInfo.ReBuildInfo = hi.ReBuildInfo;
            basicInfo.packagePriceURL = string.Format("http://m1.zmjiudian.com/{0}/price", basicInfo.HotelID);
            basicInfo.InterestID = interestid;

            List<HotelInterestTagEntity> tftList = HotelService.GetHotelInterestTag(id, interestid);
            if (tftList.Count > 0)
            {
                for (int i = 0; i < (tftList.Count > 3 ? 3 : tftList.Count); i++)
                {
                    basicInfo.Featrues.Add(tftList[i].TFTName);
                }
            }

            basicInfo.InterestName = ResourceAdapter.GetInterestName(basicInfo.InterestID);

            //交通 到达 介绍
            basicInfo.AddressInfo = new ArrivalDepartureAndAddressInfo { Address = hi.Address };
            foreach (Hotel3Entity h3 in h3s)
            {
                switch (h3.Type)
                {
                    case 1:
                        basicInfo.Intro = GenHotelInfo(h3);
                        break;
                    case 2:
                        basicInfo.Interest = new HotelInfo();//GenHotelInfo(h3);
                        break;
                    case 6:
                        basicInfo.AddressInfo.ArrivalAndDeparture = GenHotelInfo(h3);
                        break;
                }
            }

            return basicInfo;
        }

        private HotelItem6 GenBasicHotelItem6(int id, int interestid, ContextBasicInfo _contextBasicInfo = null)
        {
            var basicInfo = new HotelItem6();
            try
            {
                HotelItem hi = ResourceAdapter.GetHotel(id, UserId);
                List<Hotel3Entity> h3s = HotelService.GetHotel3(id);

                basicInfo.RestaurentList = new List<HotelRestaurantEntity>();//HotelService.GetHotelRestaurantList(id).Where(r => !r.IsInner).Take(3).ToList(); //不显示酒店内餐馆，只显示三条

                basicInfo.SightList = HotelService.GetHotelSightList(id);//new List<HotelSightEntity>(); //暂不显示
                if (basicInfo.SightList != null && basicInfo.SightList.Count != 0)
                {
                    basicInfo.SightList.ForEach((_) =>
                    {
                        _.DistanceDescription = _.Distance >= 1000 ? string.Format("距酒店{0}公里", Math.Round(_.Distance / 1000, 1)) : string.Format("距酒店{0}米", _.Distance.ToString("0"));
                    });
                }

                basicInfo.GLat = hi.GLat;
                basicInfo.GLon = hi.GLon;
                basicInfo.HotelID = id;
                basicInfo.HotelName = hi.Name;
                basicInfo.districtID = hi.DistrictId;
                basicInfo.districtName = hi.DistrictName;
                basicInfo.Tel = hi.Telephone;
                basicInfo.Score = hi.Score;
                basicInfo.ReviewCount = hi.ReviewCount;

                HotelPhotosEntity hps = GetHotelPhotos(hi.Id, interestid);
                basicInfo.PicCount = hps.HPList.Count();
                if (_contextBasicInfo != null && _contextBasicInfo.IsThanVer6_2_1)
                {
                    basicInfo.Pics = hps.HPList.Take(5).Select(p => PhotoAdapter.GenHotelPicUrl(p.SURL, Enums.AppPhotoSize.theme).Replace("_theme", "_640x360")).ToList();
                }
                else
                {
                    basicInfo.Pics = hps.HPList.Take(5).Select(p => PhotoAdapter.GenHotelPicUrl(p.SURL, Enums.AppPhotoSize.theme)).ToList();//列表页图片尺寸与详情页一致
                }

                

                basicInfo.Star = hi.Star;
                basicInfo.OpenYear = hi.OpenYear.Year > 1901 ? string.Format("{0}年", hi.OpenYear.Year) : "";
                basicInfo.ReBuildInfo = hi.ReBuildInfo;
                if (_contextBasicInfo != null && (_contextBasicInfo.IsThanVer5_9 || (!_contextBasicInfo.IsIOS && !_contextBasicInfo.IsAndroid)))
                {
                    basicInfo.OpenYear = string.IsNullOrEmpty(basicInfo.OpenYear) ? "" : basicInfo.OpenYear + "开业";
                    if (!string.IsNullOrWhiteSpace(basicInfo.ReBuildInfo) && basicInfo.ReBuildInfo.Trim().Length > 3)
                    {
                        basicInfo.ReBuildInfo = basicInfo.ReBuildInfo.Substring(0, 4) + "年重装修";
                    }
                }

                basicInfo.packagePriceURL = string.Format("http://m1.zmjiudian.com/{0}/price", basicInfo.HotelID);
                basicInfo.InterestID = interestid;

                List<HotelInterestTagEntity> tftList = HotelService.GetHotelInterestTag(id, interestid);
                if (tftList.Count > 0)
                {
                    for (int i = 0; i < (tftList.Count > 3 ? 3 : tftList.Count); i++)
                    {
                        basicInfo.Featrues.Add(tftList[i].TFTName);
                    }
                }

                basicInfo.InterestName = ResourceAdapter.GetInterestName(basicInfo.InterestID);

                //交通 到达 介绍
                basicInfo.AddressInfo = new ArrivalDepartureAndAddressInfo { Address = hi.Address };
                foreach (Hotel3Entity h3 in h3s)
                {
                    switch (h3.Type)
                    {
                        //case 1: 推荐理由
                        //    HotelInfo his = new HotelInfo();
                        //    his = GenHotel3Info(h3);
                        //    basicInfo.Intro = his;
                        //    if (!string.IsNullOrEmpty(his.Items[0].content))
                        //    {
                        //        basicInfo.IntroNew.Description = his.Description;
                        //        foreach (var item in his.Items)
                        //        {
                        //            basicInfo.IntroNew.Item.Add(item.content);
                        //        }
                        //    }
                        //    break;
                        case 2://玩点
                            if (_contextBasicInfo == null || !_contextBasicInfo.IsThanVer5_9)
                            {
                                basicInfo.Interest = GenHotelInfo(h3);//GenHotelInfo(h3);
                            }
                            break;
                        case 3://美食
                            if (_contextBasicInfo == null || !_contextBasicInfo.IsThanVer5_9)
                            {
                                basicInfo.FoodDescription = GenHotelInfo(h3);//GenHotelInfo(h3);
                                basicInfo.FoodDescription.CategoryID = 18;
                            }
                            break;
                        case 5://房间
                            if (_contextBasicInfo == null || !_contextBasicInfo.IsThanVer5_9)
                            {
                                basicInfo.RoomDescription = GenHotel3Info(h3);//GenHotelInfo(h3);
                                basicInfo.RoomDescription.CategoryID = 19;
                            }
                            break;
                        case 6:
                            if (_contextBasicInfo == null || !_contextBasicInfo.IsThanVer5_9)
                            {
                                basicInfo.AddressInfo.ArrivalAndDeparture = GenHotel3Info(h3);
                            }
                            break;
                        case 7://设施
                            if (_contextBasicInfo == null || !_contextBasicInfo.IsThanVer5_9)
                            {
                                basicInfo.FacilitieDescription = GenHotel3Info(h3);
                                basicInfo.FacilitieDescription.CategoryID = 17;
                            }
                            break;


                            #region 微信图文 取hotelcontact中的ActivePageId
                            //case 9: //微信分享全文链接
                            //case 10: //微信链接为空时 取本地全文链接
                            //    Hotel3Entity h9 = h3s.Where(_ => _.Type == 9).FirstOrDefault();
                            //    if (h9 != null)
                            //    {
                            //        HotelInfo hi9 = GenHotel3Info(h9);
                            //        //string content9 = hi9.Items[0].content;
                            //        if (hi9.Items.Count > 0)
                            //        {
                            //            string strUrl9 = hi9.Items[0].content.Contains("#") == true ? (hi9.Items[0].content.Insert(hi9.Items[0].content.LastIndexOf("#"), "&_isshare=1")) : (hi9.Items[0].content + "&_isshare=1");
                            //            basicInfo.IntroNew.ActionUrl = hi9.Items.Count == 0 ? "" : strUrl9;
                            //            basicInfo.IntroNew.Text = "全文";
                            //        }
                            //        else
                            //        {
                            //            HotelInfo h = new HotelInfo();
                            //            h = GenHotel3Info(h3);
                            //            basicInfo.IntroNew.ActionUrl = h.Items.Count == 0 ? "" : ("http://www.zmjiudian.com/active/activepage?midx=" + h.Items[0].content + "&_isshare=1");
                            //            basicInfo.IntroNew.Text = "全文";
                            //        }
                            //    }
                            //    else
                            //    {
                            //        HotelInfo h = new HotelInfo();
                            //        h = GenHotel3Info(h3);
                            //        basicInfo.IntroNew.ActionUrl = h.Items.Count == 0 ? "" : ("http://www.zmjiudian.com/active/activepage?midx=" + h.Items[0].content + "&_isshare=1");
                            //        basicInfo.IntroNew.Text = "全文";
                            //    }
                            //    break; 
                            #endregion
                    }
                }

                //获取微信图文
                HJD.HotelServices.Contracts.HotelContacts hc = HotelService.GetHotelContacts(id);
                if (hc != null & hc.ActivePageId > 0)
                {
                    basicInfo.IntroNew.ActionUrl = "http://www.zmjiudian.com/active/activepage?pageid=" + hc.ActivePageId + "&_isshare=1";
                    basicInfo.IntroNew.Text = "全文";
                }

            }
            catch (Exception err)
            {
                Log.WriteLog("GenBasicHotelItem6:" + id.ToString() + err.Message + err.StackTrace);
            }
            return basicInfo;
        }


        /// <summary>
        /// 获取特征评论标签
        /// </summary>
        /// <param name="hi5"></param>
        /// <param name="interestid"></param>
        /// <param name="isOldRoomComment"></param>
        public void GenFeaturedCommentData(HotelItem5 hi5, int interestid, string appVer = "", TimeLog parentLog = null)
        {
            TimeLog log = new TimeLog("GenFeaturedCommentData", 300, parentLog);

            var result = new List<FeaturedCommentEntity>();
            var id = hi5.HotelID;

            List<FeaturedCommentEntity> fl = GenHotelDetailDisplayList(id, appVer);//酒店展示的全部特征点评
            if (fl == null || fl.Count == 0)
            {
                return;
            }

            log.AddLog("GenHotelDetailDisplayList");

            //如果有与玩点（interestid)相关的点评，那么这类点评排在前面
            if (interestid > 0 && fl.Where(t => t.RelInterestID == interestid).Count() > 0)
            {
                fl.Where(t => t.RelInterestID == interestid).First().TagType = 0;
            }
            var isAppVerBefore4dot4 = string.IsNullOrWhiteSpace(appVer) || appVer.CompareTo("4.4") < 0;
            int maxFeaturedTagCount = isAppVerBefore4dot4 ? 3 : 8;

            hi5.FeatureList = fl.Where(f => f.CategoryID == 16).OrderBy(f => f.TagType).ThenByDescending(f => f.CommentCount).Take(maxFeaturedTagCount).ToList();//特色
            hi5.EntertainmentList = fl.Where(f => f.CategoryID == 17).OrderByDescending(f => f.CommentCount).Take(maxFeaturedTagCount).ToList();//娱乐
            hi5.FoodComment = fl.Where(f => f.CategoryID == 18).OrderByDescending(f => f.CommentCount).Take(maxFeaturedTagCount).ToList();//食物

            List<PRoomInfoEntity> pRoomInfoList = GetProomInfoList(id).OrderByDescending(_ => _.IsRecommend).ThenByDescending(_ => _.CommentCount).ToList();

            log.AddLog("GetProomInfoList");

            if (isAppVerBefore4dot4)
            {
                hi5.RoomComment = new RoomTypeComment()
                {
                    defaultCategoryID = 19,
                    singleComments = new List<FeaturedCommentEntity>(),
                    whotelComment = ""
                };

                List<FeaturedCommentEntity> roomComments = fl.Where(f => f.CategoryID == 19).Take(maxFeaturedTagCount).ToList();//内部需要找酒店房型的点评及照片，房型点评
                //先从后台套餐房型入手有用这个，没有用携程的房型，有图出图，有文字出文字，什么也没有，图和文字都没有就说酒店整体的点评
                if (pRoomInfoList != null && pRoomInfoList.Count > 0)
                {
                    foreach (var roomInfo in pRoomInfoList)
                    {
                        FeaturedCommentEntity fce = roomComments.FindAll(_ => _.FeaturedName.Equals(roomInfo.RoomCode)).FirstOrDefault();//特色名称和房型一样 则是房型对应的点评内容
                        fce = fce == null ? new FeaturedCommentEntity() : fce;
                        if (!string.IsNullOrEmpty(roomInfo.PicShortNames) && roomInfo.PicShortNames.Length > 4)
                        {
                            fce.PicUrl = genHotelRoomTypePicCompleteUrl(roomInfo.PicShortNames);
                            //以下三行代码 表示有照片的房型才加入 没有照片就不显示该房型信息
                            fce.FeaturedName = roomInfo.RoomCode;
                            fce.CategoryID = fce.CategoryID == 0 ? 19 : fce.CategoryID;
                            fce.FeaturedID = 0;
                            fce.IsRecommend = roomInfo.IsRecommend;
                            hi5.RoomComment.singleComments.Add(fce);
                        }
                    }
                    hi5.RoomComment.singleComments = hi5.RoomComment.singleComments.OrderByDescending(_ => _.IsRecommend).ToList();
                }
            }
            else
            {
                var roomTypeList = GetHotelRoomTypeFilterTagList(id);
                log.AddLog("GetHotelRoomTypeFilterTagList");
                var tftList = TransHotelRoomTypeFilterTag2FeatureComments(roomTypeList);
                //房型标签
                hi5.RoomTypeData = new List<RoomTypeCommentItem>();
                if (pRoomInfoList != null && pRoomInfoList.Count > 0)
                {
                    foreach (var item in pRoomInfoList)
                    {
                        var tftListOfRoomType = tftList.FindAll(_ => _.CategoryID == HJDAPI.Common.Helpers.Struct.HotelDetailDisplayFilterPrefix.RoomType + item.ID).Take(maxFeaturedTagCount).ToList();
                        var roomTypeEntity = new RoomTypeCommentItem()
                        {
                            DefaultCategoryID = 19,
                            Name = item.RoomCode,
                            Pics = genHotelRoomTypePicCompleteUrl(item.PicShortNames),
                            Tags = new List<FeaturedCommentEntity>(),//tftListOfRoomType,
                            CommentCount = tftListOfRoomType.Sum(_ => _.CommentCount),
                            IsRecommend = item.IsRecommend,
                            RoomArea = !string.IsNullOrWhiteSpace(item.Area) ? item.Area + "m²" : "",
                            Floor = !string.IsNullOrWhiteSpace(item.Floor) ? item.Floor + "层" : "",
                            BedSize = item.BigBedWidth > 0 ? ("大床" + item.BigBedWidth + "m" + (item.TwinBedWidth > 0 ? "或双床" + item.TwinBedWidth + "m" : "")) : (item.TwinBedWidth > 0 ? "双床" + item.TwinBedWidth + "m" : "")
                        };
                        hi5.RoomTypeData.Add(roomTypeEntity);
                    }
                }
                hi5.FacilityList = fl.Where(f => f.CategoryID == 152).OrderByDescending(f => f.CommentCount).Take(maxFeaturedTagCount).ToList();//设施标签
            }

            log.Finish();
        }

        /// <summary>
        /// 获取特征评论标签
        /// </summary>
        /// <param name="hi6"></param>
        /// <param name="interestid"></param>
        /// <param name="isOldRoomComment"></param>
        public void GenFeaturedCommentData60(HotelItem6 hi6, int interestid, string appVer = "", TimeLog parentLog = null)
        {
            TimeLog log = new TimeLog("GenFeaturedCommentData", 300, parentLog);

            var result = new List<FeaturedCommentEntity>();
            var id = hi6.HotelID;

            List<FeaturedCommentEntity> fl = GenHotelDetailDisplayList(id, appVer);//酒店展示的全部特征点评
            if (fl == null || fl.Count == 0)
            {
                return;
            }

            log.AddLog("GenHotelDetailDisplayList");

            //如果有与玩点（interestid)相关的点评，那么这类点评排在前面
            if (interestid > 0 && fl.Where(t => t.RelInterestID == interestid).Count() > 0)
            {
                fl.Where(t => t.RelInterestID == interestid).First().TagType = 0;
            }
            var isAppVerBefore4dot4 = string.IsNullOrWhiteSpace(appVer) || appVer.CompareTo("4.4") < 0;
            int maxFeaturedTagCount = isAppVerBefore4dot4 ? 3 : 8;
            ///---------旧版本
            //hi6.FeatureList 
            List<int> featureIdList = GenHotelShowFeatureDetailList(id);//展示到前端的特色id
            //Log.WriteLog("featureIdList" + string.Join(",", featureIdList));
            List<FeaturedCommentEntity> FeaturedCommentList = fl.Where(f => f.CategoryID == 16).OrderBy(f => f.TagType).ThenByDescending(f => f.CommentCount).Take(maxFeaturedTagCount).ToList();//特色

            foreach (FeaturedCommentEntity item in FeaturedCommentList)
            {
                // Log.WriteLog("featureIdList" + string.Join(",", item));
                if (featureIdList.Contains(item.FeaturedID))
                {
                    hi6.FeatureList.Add(item);
                }
            }

            //hi6.EntertainmentList = fl.Where(f => f.CategoryID == 17).OrderByDescending(f => f.CommentCount).Take(maxFeaturedTagCount).ToList();//娱乐
            //hi6.FoodComment = fl.Where(f => f.CategoryID == 18).OrderByDescending(f => f.CommentCount).Take(maxFeaturedTagCount).ToList();//食物

            //hi6.FacilitieDescription = fl.Where(f => f.CategoryID == 17).OrderBy(f => f.TagType).ThenByDescending(f => f.CommentCount).Take(maxFeaturedTagCount).FirstOrDefault();//设施
            //hi6.FoodDescription = fl.Where(f => f.CategoryID == 18).OrderByDescending(f => f.CommentCount).Take(maxFeaturedTagCount).FirstOrDefault();//食物
            //hi6.RoomDescription = fl.Where(f => f.CategoryID == 19).OrderByDescending(f => f.CommentCount).Take(maxFeaturedTagCount).FirstOrDefault();//房间

            List<PRoomInfoEntity> pRoomInfoList = GetProomInfoList(id).OrderByDescending(_ => _.IsRecommend).ThenByDescending(_ => _.CommentCount).ToList();

            log.AddLog("GetProomInfoList");

            if (isAppVerBefore4dot4)
            {
                hi6.RoomComment = new RoomTypeComment()
                {
                    defaultCategoryID = 19,
                    singleComments = new List<FeaturedCommentEntity>(),
                    whotelComment = ""
                };

                List<FeaturedCommentEntity> roomComments = fl.Where(f => f.CategoryID == 19).Take(maxFeaturedTagCount).ToList();//内部需要找酒店房型的点评及照片，房型点评
                //先从后台套餐房型入手有用这个，没有用携程的房型，有图出图，有文字出文字，什么也没有，图和文字都没有就说酒店整体的点评
                if (pRoomInfoList != null && pRoomInfoList.Count > 0)
                {
                    foreach (var roomInfo in pRoomInfoList)
                    {
                        FeaturedCommentEntity fce = roomComments.FindAll(_ => _.FeaturedName.Equals(roomInfo.RoomCode)).FirstOrDefault();//特色名称和房型一样 则是房型对应的点评内容
                        fce = fce == null ? new FeaturedCommentEntity() : fce;
                        if (!string.IsNullOrEmpty(roomInfo.PicShortNames) && roomInfo.PicShortNames.Length > 4)
                        {
                            fce.PicUrl = genHotelRoomTypePicCompleteUrl(roomInfo.PicShortNames);
                            //以下三行代码 表示有照片的房型才加入 没有照片就不显示该房型信息
                            fce.FeaturedName = roomInfo.RoomCode;
                            fce.CategoryID = fce.CategoryID == 0 ? 19 : fce.CategoryID;
                            fce.FeaturedID = 0;
                            fce.IsRecommend = roomInfo.IsRecommend;
                            hi6.RoomComment.singleComments.Add(fce);
                        }
                    }
                    hi6.RoomComment.singleComments = hi6.RoomComment.singleComments.OrderByDescending(_ => _.IsRecommend).ToList();
                }
            }
            else
            {
                var roomTypeList = GetHotelRoomTypeFilterTagList(id);
                log.AddLog("GetHotelRoomTypeFilterTagList");
                var tftList = TransHotelRoomTypeFilterTag2FeatureComments(roomTypeList);
                //房型标签
                hi6.RoomTypeData = new List<RoomTypeCommentItem>();
                if (pRoomInfoList != null && pRoomInfoList.Count > 0)
                {
                    foreach (var item in pRoomInfoList)
                    {
                        var tftListOfRoomType = tftList.FindAll(_ => _.CategoryID == HJDAPI.Common.Helpers.Struct.HotelDetailDisplayFilterPrefix.RoomType + item.ID).Take(maxFeaturedTagCount).ToList();
                        var roomTypeEntity = new RoomTypeCommentItem()
                        {
                            DefaultCategoryID = 19,
                            Name = item.RoomCode,
                            Pics = genHotelRoomTypePicCompleteUrl(item.PicShortNames),
                            Tags = new List<FeaturedCommentEntity>(),//tftListOfRoomType,
                            CommentCount = tftListOfRoomType.Sum(_ => _.CommentCount),
                            IsRecommend = item.IsRecommend,
                            RoomArea = !string.IsNullOrWhiteSpace(item.Area) ? item.Area + "m²" : "",
                            Floor = !string.IsNullOrWhiteSpace(item.Floor) ? item.Floor + "层" : "",
                            BedSize = item.BigBedWidth > 0 ? ("大床" + item.BigBedWidth + "m" + (item.TwinBedWidth > 0 ? "或双床" + item.TwinBedWidth + "m" : "")) : (item.TwinBedWidth > 0 ? "双床" + item.TwinBedWidth + "m" : "")
                        };
                        hi6.RoomTypeData.Add(roomTypeEntity);
                    }
                }
                hi6.FacilityList = fl.Where(f => f.CategoryID == 152).OrderByDescending(f => f.CommentCount).Take(maxFeaturedTagCount).ToList();//设施标签
            }

            log.Finish();
        }

        public static List<string> genHotelRoomTypePicCompleteUrl(string picShortNames)
        {
            return (!string.IsNullOrEmpty(picShortNames) && picShortNames.Length > 4) ? picShortNames.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(_ => PhotoAdapter.GenHotelPicUrl(_, Enums.AppPhotoSize.appview)).ToList() : new List<string>() { PhotoAdapter.GenHotelPicUrl("116eTZJ0ZF", Enums.AppPhotoSize.appview) };
        }

        /// <summary>
        /// 猜你喜欢的酒店
        /// </summary>
        /// <param name="hotelId"></param>
        /// <param name="userid"></param>
        /// <param name="interestId"></param>
        /// <param name="arrivalTime"></param>
        /// <param name="departureTime"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        private List<PackageItem> GenRecommendHotelList(int hotelId, long userid, int interestId, DateTime arrivalTime, DateTime departureTime, int hotelCount = 4, int distance = 300000, TimeLog parentLog = null)
        {
            List<PackageItem> packageList = new List<PackageItem>();
            TimeLog log = new TimeLog("GenFeaturedCommentData", 300, parentLog);

            //获取推荐酒店ID  分别得出各自的HotelItem
            List<ListHotelItem3> list = HotelService.GetUserRecommendHotelList(hotelId, userid, interestId, hotelCount, distance);
            log.AddLog("HotelService.GetUserRecommendHotelList");
            if (list.Count > 0)
            {
                HotelItem5 h5 = new HotelItem5();

                List<OTAInfo2> otaList = new List<OTAInfo2>();
                foreach (var item in list)
                {
                    PackageItem pi = GenHotelPricePlanList(userid, item.Id, ref arrivalTime, ref departureTime, ref otaList).FirstOrDefault();
                    //log.AddLog("GenHotelPricePlanList:"+ item.Id.ToString());
                    if (pi != null)
                    {
                        pi.HotelID = item.Id;
                        pi.HotelName = item.Name;
                        pi.PicUrl = item.PictureSURLList.Select(_ => PhotoAdapter.GenHotelPicUrl(_, Enums.AppPhotoSize.appdetail)).FirstOrDefault();
                        pi.InterestID = interestId;
                        packageList.Add(pi);
                    }
                }
            }

            log.Finish();

            return packageList;
        }

        private void GenHotelFacilities(HJD.HotelManagementCenter.Domain.HotelContactsEntity hce, HotelItem5 hi)
        {
            hi.Facilities = new List<HotelFacilityEntity>();//酒店设施  是否需要合并房间设施 ToDo 去hotelcontact表中找数据
            var petStr = (hce.WithPet == 1) ? "可带宠物" : (hce.WithPet == 3) ? "部分房型可带宠物" : "";
            if (!string.IsNullOrEmpty(petStr))
            {
                hi.Facilities.Add(new HotelFacilityEntity() { FacilityName = petStr, HotelID = hi.HotelID, Cat = "WithPet", IsAvailable = true });
            }
            var indoorPool = hce.IndoorSwimmingPool != 1 ? "" : hce.IndoorSwimmingPoolIsHengWen == 1 ? "室内恒温游泳池" : "室内游泳池";
            if (!string.IsNullOrEmpty(indoorPool))
            {
                hi.Facilities.Add(new HotelFacilityEntity() { FacilityName = indoorPool, HotelID = hi.HotelID, Cat = "IndoorSwimmingPool", IsAvailable = true });
            }
            var outdoorPool = hce.OutdoorSwimmingPool != 1 ? "" : "室外游泳池";
            if (!string.IsNullOrEmpty(outdoorPool))
            {
                hi.Facilities.Add(new HotelFacilityEntity() { FacilityName = outdoorPool, HotelID = hi.HotelID, Cat = "OutdoorSwimmingPool", IsAvailable = true });
            }
            var indoorChildPool = hce.IndoorChildrenPool != 1 ? "" : hce.IndoorChildrenPoolIsHengWen == 1 ? "室内恒温儿童游泳池" : "室内儿童游泳池";
            if (!string.IsNullOrEmpty(indoorChildPool))
            {
                hi.Facilities.Add(new HotelFacilityEntity() { FacilityName = indoorChildPool, HotelID = hi.HotelID, Cat = "IndoorChildrenPool", IsAvailable = true });
            }
            var indoorPlayground = hce.IndoorChildrenPlayground != 1 ? "" : "室内儿童游乐场";
            if (!string.IsNullOrEmpty(indoorPlayground))
            {
                hi.Facilities.Add(new HotelFacilityEntity() { FacilityName = indoorPlayground, HotelID = hi.HotelID, Cat = "IndoorChildrenPlayground", IsAvailable = true });
            }
            var outdoorPlayground = hce.OutdoorChildrenPlayground != 1 ? "" : "室外儿童游乐场";
            if (!string.IsNullOrEmpty(outdoorPlayground))
            {
                hi.Facilities.Add(new HotelFacilityEntity() { FacilityName = outdoorPlayground, HotelID = hi.HotelID, Cat = "OutdoorChildrenPlayground", IsAvailable = true });
            }
            var isHasBus = hce.HasBus != 1 ? "" : "班车接送";
            if (!string.IsNullOrEmpty(isHasBus))
            {
                hi.Facilities.Add(new HotelFacilityEntity() { FacilityName = isHasBus, HotelID = hi.HotelID, Cat = "HasBus", IsAvailable = true });
            }
            var isHasBeach = hce.Beach != 1 ? "" : "沙滩";
            if (!string.IsNullOrEmpty(isHasBeach))
            {
                hi.Facilities.Add(new HotelFacilityEntity() { FacilityName = isHasBeach, HotelID = hi.HotelID, Cat = "Beach", IsAvailable = true });
            }
            var isHasPrivateBeach = hce.PrivateBeach != 1 ? "" : "私人沙滩";
            if (!string.IsNullOrEmpty(isHasPrivateBeach))
            {
                hi.Facilities.Add(new HotelFacilityEntity() { FacilityName = isHasPrivateBeach, HotelID = hi.HotelID, Cat = "PrivateBeach", IsAvailable = true });
            }
            var isHasTennisCourt = hce.TennisCourt != 1 ? "" : "网球场";
            if (!string.IsNullOrEmpty(isHasTennisCourt))
            {
                hi.Facilities.Add(new HotelFacilityEntity() { FacilityName = isHasTennisCourt, HotelID = hi.HotelID, Cat = "TennisCourt", IsAvailable = true });
            }
            var isChildCare = hce.ChildCare != 1 ? "" : hce.ChildCareIsFree == 1 ? "免费儿童看护" : "儿童看护";
            if (!string.IsNullOrEmpty(isChildCare))
            {
                hi.Facilities.Add(new HotelFacilityEntity() { FacilityName = isChildCare, HotelID = hi.HotelID, Cat = "ChildCare", IsAvailable = true });
            }
        }

        public static HotelInfo GenHotelInfo(Hotel3Entity h3)
        {
            HotelInfo hi = new HotelInfo();
            try
            {
                if (h3 != null)
                {
                    hi.Description = h3.Description == null ? "" : h3.Description.Trim();
                    StringReader reader = new StringReader(h3.items);
                    DataSet set = new DataSet();
                    set.ReadXml(reader);
                    if (set.Tables.Count > 0)
                    {
                        foreach (DataRow dr in set.Tables[0].Rows)
                        {
                            Item item = new Item();
                            item.content = dr["content"].ToString().Trim();
                            item.pic = "";// dr["Image"].ToString();
                            hi.Items.Add(item);
                        }
                    }
                }
            }
            catch { }

            return hi;

        }

        public static HotelInfo GenHotel3Info(Hotel3Entity h3)
        {
            HotelInfo hi = new HotelInfo();
            try
            {
                if (h3 != null)
                {
                    //hi.Description = h3.Description == null ? "" : h3.Description;
                    h3.items = h3.items.Replace("&", "&amp;").Replace("<br>","");
                    StringReader reader = new StringReader(h3.items);
                    DataSet set = new DataSet();
                    set.ReadXml(reader);
                    if (set.Tables.Count > 0)
                    {
                        int i = 1;
                        foreach (DataRow dr in set.Tables[0].Rows)
                        {
                            Item item = new Item();
                            hi.Description += dr["content"].ToString().Trim() + (i == set.Tables[0].Rows.Count ? "" : Environment.NewLine);
                            item.content = dr["content"].ToString().Trim();
                            item.pic = "";// dr["Image"].ToString();
                            i++;
                            hi.Items.Add(item);
                        }
                    }
                }
            }
            catch (Exception e) { Log.WriteLog("GenHotel3Info error" + e); }

            return hi;

        }

        public List<HotelTFTReview> Trans2TFTReview(List<HotelTFTReviewEntity> cr)
        {
            return (from r in cr
                    select new HotelTFTReview
                    {
                        HotelID = r.HotelID
                        ,
                        TFTID = r.TFTID
                        ,
                        TFTName = r.TFTName
                        ,
                        Type = r.Type
                        ,
                        Result = transReviewExToCommentItem(r.ReviewList, r.HotelID)
                    }).ToList();

        }

        private int GetOTAChannelID(string otaename)
        {
            int otaChannelID = 0;
            switch (otaename.ToLower())
            {
                case "booking":
                    otaChannelID = 1;
                    break;
                case "ctrip":
                    otaChannelID = 2;
                    break;
                case "elong":
                    otaChannelID = 6;
                    break;
                case "tongcheng":
                    otaChannelID = 4;
                    break;
                case "zhuna":
                    otaChannelID = 11;
                    break;
                case "qunar":
                    otaChannelID = 26;
                    break;
            }

            return otaChannelID;
        }

        [DataContract]
        public struct HotelItemResult
        {
            [DataMember]
            public HotelItem hotel;
            [DataMember]
            public ReviewResult reviews;

            [DataMember]
            public List<OTAInfo> OTAInfos;
        }

        [DataContract]
        public struct HotelItem2Result
        {
            [DataMember]
            public HotelItem hotel;
            [DataMember]
            public List<HotelTFTReview> reviews;

            [DataMember]
            public ReviewResult themereviews;

            [DataMember]
            public List<HotelTFTRelItemEntity> tftList;

            [DataMember]
            public List<OTAInfo> OTAInfos;

            [DataMember]
            public List<String> Pics;
        }

        [DataContract]
        public struct HotelTFTReview
        {
            [DataMember]
            public int HotelID { get; set; }
            [DataMember]
            public IEnumerable<CommentItem> Result { get; set; }
            [DataMember]
            public int TFTID { get; set; }
            [DataMember]
            public string TFTName { get; set; }
            [DataMember]
            public int Type { get; set; }
        }

        public string HotelTypes()
        {
            var res = HotelService.GetHotelAllClass();
            return "{" + string.Join(",", res.Select(p => p.ClassID + ":\"" + p.ClassName + "\"")) + "}";
        }

        /// <summary>
        /// 获取酒店点评
        /// </summary>
        /// <param name="hotel">酒店id</param>
        /// <param name="review">点评id</param>
        /// <param name="start"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public ReviewResult40 GetHotelReviews40(ReviewQueryParam par, string appVer = null, TimeLog parentLog = null)
        {
            TimeLog log = new TimeLog("GetHotelReviews40", 300, parentLog);

            int originTFTType = par.TFTType;
            int originTFTID = par.TFTID;

            ArguHotelReview p = new ArguHotelReview();
            p.NeedFilter = 1;//不需要分类过滤
            p.Hotel = par.Hotel;
            p.Start = par.Start;
            p.Count = par.Count;
            p.RatingType = par.RatingType;

            p.HotelReviewOrderType = HotelReviewOrderType.Source;//HotelReviewOrderType.Time_Down;

            bool isAppBefore4dot4 = string.IsNullOrWhiteSpace(appVer) || appVer.CompareTo("4.4") < 0;
            //bool isAppBefore5dot0 = string.IsNullOrWhiteSpace(appVer) || appVer.CompareTo("5.0") < 0;

            //4.4之前版本 查看某个分类全部点评
            if (par.TFTID == 0 && par.TFTType > 0 && isAppBefore4dot4)//如果是查看一类点评, 那么取第一个标签来显示
            {
                var allHotelComment = GetHotelTFTList40(par.Hotel, par.TFTType).FirstOrDefault();
                log.AddLog("GetHotelTFTList40");
                par.TFTID = allHotelComment != null ? allHotelComment.TFTID : 0;
            }

            //4.4版本 查看房型相关点评
            int roomType = 0;
            int notValidRoomTypeID = 484;//一个无效的房型ID 用来在没有房型时强制不显示点评
            long baseOffset = HJDAPI.Common.Helpers.Struct.HotelDetailDisplayFilterPrefix.BaseOffset;
            bool isRoomTypeReviewList = par.TFTType == 19 || (par.TFTType > 19 * baseOffset && par.TFTType < 20 * baseOffset);

            List<PRoomInfoEntity> pRoomInfoList = new List<PRoomInfoEntity>();
            if (isRoomTypeReviewList && !isAppBefore4dot4)
            {
                pRoomInfoList = GetProomInfoList(par.Hotel).OrderByDescending(_ => _.IsRecommend).ThenByDescending(_ => _.CommentCount).ToList();
                log.AddLog("GetProomInfoList");
                if (par.TFTType == 19 && par.TFTID == 0)
                {
                    var roomTypeData = pRoomInfoList.FirstOrDefault();
                    roomType = roomTypeData != null ? roomTypeData.ID : notValidRoomTypeID;
                    par.TFTType = roomTypeData != null ? roomTypeData.ID + HJDAPI.Common.Helpers.Struct.HotelDetailDisplayFilterPrefix.RoomType : 19;
                }
                else if (par.TFTType > 0)
                {
                    roomType = par.TFTType - HJDAPI.Common.Helpers.Struct.HotelDetailDisplayFilterPrefix.RoomType;
                }
                else
                {
                    roomType = notValidRoomTypeID;
                }
            }

            //4.4版本 查看特色 美食 设施等点评
            p.FeaturedTreeList = new List<int>();
            if (!isAppBefore4dot4 && !isRoomTypeReviewList && par.TFTID == 0)
            {
                //非房间点评 缺少大类filtercol 需要把各个子标签的filtercol拿出来
                var tftList = GenHotelDetailDisplayList(par.Hotel, "4.4").FindAll(_ => _.CategoryID == par.TFTType);//酒店展示的全部特征点评 //GetHotelTFTList40(par.Hotel, par.TFTType);
                log.AddLog("GenHotelDetailDisplayList");
                if (tftList != null && tftList.Count != 0)
                {
                    p.FeaturedTreeList.AddRange(tftList.Select(_ => _.FeaturedID));
                }
                else if (par.TFTType > 0)
                {
                    roomType = notValidRoomTypeID;//其他美食、设施、特色等在详情页没有任何标签情况下 强制不显示任何点评
                }
            }

            p.RoomType = roomType;
            p.FeaturedTreeID = par.TFTID;

            QueryReviewResult qrr = HotelService.QueryReview(p);
            log.AddLog("QueryReview");
            FillWHotelComment(qrr, log);
            log.AddLog("FillWHotelComment");
            var model = (isRoomTypeReviewList || par.TFTType != 0) && !isAppBefore4dot4 ? new ReviewResult40() : GetHotelReviews40Model(par);
            model.Start = par.Start;
            model.TotalCount = qrr.TotalCount;
            model.HotelID = par.Hotel;

            //foreach(PRoomInfoEntity item in pRoomInfoList)
            //{
            //    RoomPicAndPicDescription pic=new RoomPicAndPicDescription();
            //    pic.PicUrl = genHotelRoomTypePicCompleteUrl(item.PicShortNames);
            //    model.PicUrlDescription = (!string.IsNullOrWhiteSpace(item.Area) ? item.Area + "平米 | " : "") + (!string.IsNullOrWhiteSpace(item.Floor) ? item.Floor + "层 | " : "") + (item.BigBedWidth > 0 ? ("大床" + item.BigBedWidth + "米" + (item.TwinBedWidth > 0 ? "或双床" + item.TwinBedWidth + "米" : "")) : (item.TwinBedWidth > 0 ? "双床" + item.TwinBedWidth + "米" : ""));
            //    model.RoomPicAndPicDescriptionList.Add(pic);
            //}

            //我们的点评排前  按ChannelType升序  ChannelType=0是我们客人写的
            model.Result = transReviewExToCommentItem(qrr.ReviewList, par.Hotel, log);//.OrderBy(_ => _.ChannelID).ToList();
            log.AddLog("transReviewExToCommentItem");

            List<int> commentIds = new List<int>();
            if (par.UserID != 0)
            {
                commentIds = commentService.GetUserClickUsefulComment(par.UserID);
                log.AddLog("GetUserClickUsefulComment");

                if (commentIds != null && commentIds.Count > 0)
                {
                    foreach (CommentItem item in model.Result)
                    {
                        item.HasClickUseful = commentIds.Contains(item.Id) ? true : false;
                    }
                }
            }

            model.RatingType = (int)par.RatingType;
            model.InterestID = par.InterestID;
            model.TFTID = par.TFTID;
            model.TFTType = par.TFTType;

            if (!isAppBefore4dot4 && isRoomTypeReviewList)
            {
                if (pRoomInfoList != null && pRoomInfoList.Count != 0)
                {
                    //PRoomInfoEntity pr= pRoomInfoList.Where(_ => _.ID == (pRoomInfoList.FirstOrDefault()!=null?pRoomInfoList.FirstOrDefault().ID:19)).FirstOrDefault();
                    long baseRoomType = HJDAPI.Common.Helpers.Struct.HotelDetailDisplayFilterPrefix.RoomType;
                    PRoomInfoEntity pr = originTFTType == 19 ? pRoomInfoList.Where(_ => _.ID == (pRoomInfoList.FirstOrDefault() != null ? pRoomInfoList.FirstOrDefault().ID : 19)).FirstOrDefault() : pRoomInfoList.Where(_ => _.ID == (originTFTType - baseRoomType)).FirstOrDefault();
                    if (pr != null)
                    {
                        model.PicUrl = genHotelRoomTypePicCompleteUrl(pr.PicShortNames);
                        log.AddLog("genHotelRoomTypePicCompleteUrl");

                        model.PicUrlDescription = (!string.IsNullOrWhiteSpace(pr.Area) ? pr.Area + "平米 | " : "") + (!string.IsNullOrWhiteSpace(pr.Floor) ? pr.Floor + "层 | " : "") + (pr.BigBedWidth > 0 ? ("大床" + pr.BigBedWidth + "米" + (pr.TwinBedWidth > 0 ? "或双床" + pr.TwinBedWidth + "米" : "")) : (pr.TwinBedWidth > 0 ? "双床" + pr.TwinBedWidth + "米" : ""));
                    }

                    model.ParentTFTList = pRoomInfoList.Select(_ => new HotelTFTRelItemEntity()
                    {
                        CommentCount = _.CommentCount,
                        HotelID = par.Hotel,
                        TagType = 0,
                        TFTID = 0,
                        TFTName = _.RoomCode,
                        Type = HJDAPI.Common.Helpers.Struct.HotelDetailDisplayFilterPrefix.RoomType + _.ID
                    }).ToList();

                    var roomTypeList = GetHotelRoomTypeFilterTagList(par.Hotel);
                    log.AddLog("GetHotelRoomTypeFilterTagList");
                    var roomTFTList = TransHotelRoomTypeFilterTag2FeatureComments(roomTypeList);
                    log.AddLog("TransHotelRoomTypeFilterTag2FeatureComments");
                    if (roomTFTList != null && roomTFTList.Count != 0)
                    {
                        List<int> feature = GenHotelShowFeatureDetailList(par.Hotel);//特色后台 设置要展示的特色标签ID集合
                        //  Log.WriteLog("featureIdList :" + string.Join(",", feature));
                        // Log.WriteLog("par.TFTType :" + par.TFTType);
                        List<HotelTFTRelItemEntity> featuredList = roomTFTList.FindAll(_ => _.CategoryID == par.TFTType).Select(_ => new HotelTFTRelItemEntity()
                        {
                            CommentCount = _.CommentCount,
                            HotelID = _.HotelID,
                            TagType = _.TagType,
                            TFTID = _.FeaturedID,
                            TFTName = _.FeaturedName,
                            Type = _.CategoryID
                        }).ToList();

                        foreach (HotelTFTRelItemEntity item in featuredList)
                        {
                            if (feature.Contains(item.TFTID))
                            {
                                model.TFTList.Add(item);
                            }
                        }

                        //model.TFTList = roomTFTList.FindAll(_ => _.CategoryID == par.TFTType).Select(_ => new HotelTFTRelItemEntity()
                        //{
                        //    CommentCount = _.CommentCount,
                        //    HotelID = _.HotelID,
                        //    TagType = _.TagType,
                        //    TFTID = _.FeaturedID,
                        //    TFTName = _.FeaturedName,
                        //    Type = _.CategoryID
                        //}).ToList();
                    }
                    else
                    {
                        model.TFTList = new List<HotelTFTRelItemEntity>();
                    }
                }
                else
                {
                    model.ParentTFTList = new List<HotelTFTRelItemEntity>();
                    model.TFTList = new List<HotelTFTRelItemEntity>();
                }

                model.GroupTotalCount = model.ParentTFTList.Sum(_ => _.CommentCount);//所有房型的点评数据                
            }
            else if (!isAppBefore4dot4 && par.TFTType != 0)
            {
                model.ParentTFTList = new List<HotelTFTRelItemEntity>();
                List<FeaturedCommentEntity> fl = GenHotelDetailDisplayList(par.Hotel, "4.4");//酒店展示的全部特征点评
                log.AddLog("GenHotelDetailDisplayList");
                var tempList = fl.Where(f => f.CategoryID == par.TFTType).OrderByDescending(f => f.CommentCount).ToList();

                if (tempList != null && tempList.Count != 0)
                {
                    List<int> feature = GenHotelShowFeatureDetailList(par.Hotel);//特色后台 设置要展示的特色标签ID集合
                    log.AddLog("GenHotelShowFeatureDetailList");
                    //Log.WriteLog("featureIdList :" + string.Join(",", feature));
                    List<HotelTFTRelItemEntity> featuredList = tempList.FindAll(_ => _.CategoryID == par.TFTType).Select(_ => new HotelTFTRelItemEntity()
                    {
                        CommentCount = _.CommentCount,
                        HotelID = _.HotelID,
                        TagType = _.TagType,
                        TFTID = _.FeaturedID,
                        TFTName = _.FeaturedName,
                        Type = _.CategoryID
                    }).ToList();

                    foreach (HotelTFTRelItemEntity item in featuredList)
                    {
                        if (feature.Contains(item.Type))
                        {
                            //Log.WriteLog("HotelTFTRelItemEntity " + item.TFTID + "," + item.TFTName + "," + item.Type);
                            model.TFTList.Add(item);
                        }
                    }
                }
                //model.TFTList = tempList != null && tempList.Count != 0 ? tempList.Select(_ => new HotelTFTRelItemEntity()
                //{
                //    CommentCount = _.CommentCount,
                //    HotelID = _.HotelID,
                //    TagType = _.TagType,
                //    TFTID = _.FeaturedID,
                //    TFTName = _.FeaturedName,
                //    Type = _.CategoryID
                //}).ToList() : new List<HotelTFTRelItemEntity>();
                model.GroupTotalCount = model.TFTList.Sum(_ => _.CommentCount);
            }
            else
            {
                model.ParentTFTList = new List<HotelTFTRelItemEntity>();
            }

            log.Finish();
            return model;
        }

        /// <summary>
        /// 获取酒店点评
        /// </summary>
        /// <param name="hotel">酒店id</param>
        /// <param name="review">点评id</param>
        /// <param name="start"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public ReviewResult2 GetHotelReviews2(ReviewQueryParam par)
        {

            ArguHotelReview p = new ArguHotelReview();

            p.NeedFilter = 1;//不需要分类过滤
            p.Hotel = par.Hotel;
            p.Start = par.Start;
            p.Count = par.Count;
            p.RatingType = par.RatingType;

            p.HotelReviewOrderType = HotelReviewOrderType.Time_Down;

            if (par.InterestID > 0)
            {
                if (par.TFTType == 0)
                {
                    List<HotelTFTRelItemEntity> tftList = GetHotelTFTList(par.Hotel, par.InterestID);

                    par.TFTID = tftList.First().TFTID;
                    par.TFTType = tftList.First().Type;

                }
                switch (par.TFTType)
                {
                    case 1:
                        p.Tag = par.TFTID;
                        break;
                    case 2:
                        p.Featured = par.TFTID;
                        break;
                    case 3:
                        p.Theme = par.TFTID;
                        break;
                }
            }

            QueryReviewResult qrr = HotelService.QueryReview(p);

            FillWHotelComment(qrr);

            var model = GetHotelReviews2Model(par);
            model.Start = par.Start;
            model.TotalCount = qrr.TotalCount;

            model.Result = transReviewExToCommentItem(qrr.ReviewList, par.Hotel).ToList();

            model.RatingType = (int)par.RatingType;
            model.InterestID = par.InterestID;
            model.TFTID = par.TFTID;
            model.TFTType = par.TFTType;



            return model;

        }

        public ReviewResult2 GetHotelReviews2Model(ReviewQueryParam par)
        {
            return LocalCache.GetData<ReviewResult2>(string.Format("{0}:{1}:{2}", "ReviewResult2Model", par.Hotel, par.InterestID), () =>
                {
                    return GenHotelReviews2Model(par);
                });
        }

        public ReviewResult40 GetHotelReviews40Model(ReviewQueryParam par)
        {
            //2015-09-15 wwb 将酒店列表的缓存时间缩短至30min 并 更改缓存过期类型
            return LocalCache30Min.GetData<ReviewResult40>(string.Format("{0}:{1}:{2}", "ReviewResult40Model", par.Hotel, par.TFTType), () =>
                {
                    return GenHotelReviews40Model(par);
                });
        }

        public ReviewResult40 GenHotelReviews40Model(ReviewQueryParam par)
        {
            ArguHotelReview p = new ArguHotelReview();
            p.Hotel = par.Hotel;
            p.NeedReview = 1;//不需要点评数据

            string RatingPercent = "Good:0,Normal:0,Bad:0";

            if (par.TFTType > 0)
            {
                p.TFTType = par.TFTType;
            }
            else
            {
                p.RatingType = HJD.HotelServices.Contracts.RatingType.All;
            }

            QueryReviewResult qrr = HotelService.QueryReview(p);
            List<HotelTFTRelItemEntity> tftList = GetHotelTFTList40(par.Hotel, par.TFTType);
            if (par.TFTType == 0)
            {
                long FilterBase = 10000000000;
                long GoodKey = FilterBase * 2 + 13;
                long NormalKey = FilterBase * 2 + 12;
                long BadKey = FilterBase * 2 + 11;

                int good = GetFilterCount(qrr.FilterCount, GoodKey);
                int normal = GetFilterCount(qrr.FilterCount, NormalKey);
                int bad = GetFilterCount(qrr.FilterCount, BadKey);
                float total = good + normal + bad;
                if (total > 0)
                {
                    RatingPercent = string.Format("Good:{0},Normal:{1},Bad:{2}",
                       CalPercent(good, total),
                       CalPercent(normal, total),
                       100 - CalPercent(good, total) - CalPercent(normal, total)
                    );
                }
            }
            else if (tftList != null && tftList.Count > 0)
            {
                tftList.ForEach(_ => _.CommentCount = GetFilterCount(qrr.FilterCount, 80000000000 + _.TFTID));//默认特色80开头
                tftList = tftList.FindAll(_ => _.CommentCount > 0);//将点评数量为0的拿走
            }

            var model = new ReviewResult40();
            model.RatingPercent = RatingPercent;
            model.HotelID = par.Hotel;
            model.GroupTotalCount = par.TFTType > 0 ? tftList.Sum(t => t.CommentCount) : qrr.TotalCount;
            HotelItem hi = ResourceAdapter.GetHotel(par.Hotel, UserId);
            model.Score = hi.Score;
            model.AllReviewCount = hi.ReviewCount;
            model.TFTList = tftList;
            return model;
        }

        public ReviewResult40 GetHotelReviews50(ReviewQueryParam par, string appVer = null)
        {
            TimeLog log = new TimeLog(string.Format("GetHotelReviews50 Hotel：{0} TFTType:{1} Count:{2} Start:{3}", par.Hotel, par.TFTType, par.Count, par.Start)
   , 1000, null);
            if (par.Start > 150 || par.Count > 50)  //防止点评抓取 kevincai 
            {
                log.AddLog("防止点评抓取");
                log.Finish();
                return new ReviewResult40();
            }

            List<HotelTFTRelItemEntity> tftList = GetHotelTFTList40(par.Hotel, par.TFTType);

            log.AddLog("GetHotelTFTList40");

            ArguHotelReview p = new ArguHotelReview();
            p.Hotel = par.Hotel;
            p.Count = par.Count == 0 ? 15 : par.Count;
            p.Start = par.Start;
            p.Featured = 0;

            p.HotelReviewOrderType = HotelReviewOrderType.Source;//HotelReviewOrderType.Time_Down;
            p.RatingType = par.RatingType;
            p.UserIdentityType = UserIdentityType.All;
            p.TFTType = par.TFTType;

            if (par.TFTID == 0 && par.TFTType > 0)//如果是查看一类点评, 那么取第一个标签来显示
            {
                var allHotelComment = tftList.FirstOrDefault();
                par.TFTID = allHotelComment != null ? allHotelComment.TFTID : 0;
            }
            p.FeaturedTreeID = par.TFTID;

            QueryReviewResult qrr = HotelService.QueryReview(p);
            log.AddLog(string.Format("QueryReview  Hotel:{0} Count:{1} Start:{2} Featured:{3} HotelReviewOrderType :{4} RatingType :{5} UserIdentityType:{6} TFTType:{7} FeaturedTreeID:{8}",
            p.Hotel,
            p.Count,
            p.Start,
            p.Featured,
            p.HotelReviewOrderType,
            p.RatingType,
            p.UserIdentityType,
            p.TFTType,
            p.FeaturedTreeID));

            string RatingPercent = "Good:0,Normal:0,Bad:0";
            if (par.TFTType == 0)
            {
                long FilterBase = 10000000000;
                long GoodKey = FilterBase * 2 + 13;
                long NormalKey = FilterBase * 2 + 12;
                long BadKey = FilterBase * 2 + 11;

                int good = GetFilterCount(qrr.FilterCount, GoodKey);
                int normal = GetFilterCount(qrr.FilterCount, NormalKey);
                int bad = GetFilterCount(qrr.FilterCount, BadKey);
                float total = good + normal + bad;
                if (total > 0)
                {
                    RatingPercent = string.Format("Good:{0},Normal:{1},Bad:{2}",
                        CalPercent(good, total),
                        CalPercent(normal, total),
                        100 - CalPercent(good, total) - CalPercent(normal, total));
                }
            }
            else if (tftList != null && tftList.Count > 0)
            {
                tftList.ForEach(_ => _.CommentCount = GetFilterCount(qrr.FilterCount, 80000000000 + _.TFTID));//默认特色80开头
                tftList = tftList.FindAll(_ => _.CommentCount > 0);//将点评数量为0的拿走
            }

            var model = new ReviewResult40();
            model.RatingPercent = RatingPercent;
            model.HotelID = par.Hotel;

            HotelItem hi = ResourceAdapter.GetHotel(par.Hotel, 0);
            model.Score = hi.Score;
            model.AllReviewCount = hi.ReviewCount;
            log.AddLog("ResourceAdapter.GetHotel");
            model.TFTList = tftList;
            model.TFTID = par.TFTID;
            model.TFTType = par.TFTType;
            model.RatingType = (int)par.RatingType;

            model.Start = par.Start;
            model.TotalCount = qrr.TotalCount;
            model.GroupTotalCount = par.TFTType > 0 ? tftList.Sum(t => t.CommentCount) : qrr.TotalCount;// 全部点评 为全部的点评数量；其他如玩点则是将对应类型的搞出来的单独求总数

            FillWHotelComment(qrr);//补全周末酒店用户填写的点评内容
            model.Result = transReviewExToCommentItem(qrr.ReviewList, par.Hotel).ToList();

            List<int> commentIds = new List<int>();
            if (par.UserID != 0)
            {
                commentIds = commentService.GetUserClickUsefulComment(par.UserID);
                if (commentIds != null && commentIds.Count > 0)
                {
                    foreach (CommentItem item in model.Result)
                    {
                        item.HasClickUseful = commentIds != null && commentIds.Contains(item.Id) ? true : false;
                    }
                }
            }

            log.Finish();

            return model;
        }

        public int GetHotelReviewCount(int hotelId)
        {
            HotelItem hi = ResourceAdapter.GetHotel(hotelId, 0);
            return hi.ReviewCount;
        }

        public void FillWHotelCommentOne(List<HotelReviewExEntity> ReviewList)
        {
            try
            {
                List<int> whWritingList = new List<int>();

                try
                {
                    whWritingList = ReviewList.Where(o => o.WritingTypeID == 3).Select(r => r.Writing).ToList();
                }
                catch
                { }

                if (whWritingList.Count > 0)
                {
                    List<CommentInfoEntity> WHCommentList = CommentAdapter.GetComments(whWritingList);

                    foreach (CommentInfoEntity c in WHCommentList)
                    {
                        try
                        {
                            HotelReviewExEntity r = ReviewList.Where(o => o.WritingTypeID == 3 && o.Writing == c.Comment.ID).First();

                            r.CommentTitle = DescriptionHelper.CommentInfoConcat(9, c);

                            if (string.IsNullOrWhiteSpace(r.CommentTitle))
                            {
                                r.Content = GetOrGenWHHotelCommentDescription(c);
                            }
                            else
                            {
                                r.Content = DescriptionHelper.CommentInfoConcat(10, c);//点评详细内容
                                
                                r.HoContent = DescriptionHelper.CommentInfoConcat(11, c);//补充点评内容
                            }
                            r.PhotoCount = c.PhotoIDs.Count();
                            r.RoomInfo = string.Join("、", c.RoomInfo.Select(o => o.TagName));

                            var tripResult = c.TagInfo.FindAll(_ => _.CategoryID == 8).FirstOrDefault();
                            if (tripResult != null && tripResult.CategoryID > 0 && tripResult.Tags.Count > 0)
                            {
                                r.TripInfo = string.Join("、", tripResult.Tags.Select(_ => _.TagName));
                            }

                            //还要小图 和 大图链接的数组
                            r.CommentPics = c.CommentPics;
                            r.BigCommentPics = c.BigCommentPics;
                            //还要有帮助数量
                            r.UsefulCount = c.Comment.UsefulCount;
                        }
                        catch (Exception e)
                        {
                            Log.WriteLog("error 生成点评列表用点评描述，不带住房信息:" + e);
                        }
                    }
                }

                //统计一下 其他渠道点评的点有用数量
                //List<int> hotelReviewIDs = qrr.ReviewList.Where(o => o.WritingTypeID == 1).Select(r => r.Writing).ToList();
                //List<int> otaHotelReviewIDs = qrr.ReviewList.Where(o => o.WritingTypeID == 2).Select(r => r.Writing).ToList();                
            }
            catch (Exception err)
            {
                Log.WriteLog(err.Message + err.StackTrace);
            }
        }


        /// <summary>
        /// 生成点评列表用点评描述，不带住房信息
        /// </summary>
        /// <param name="qrr"></param>
        public void FillWHotelComment(QueryReviewResult qrr, TimeLog parentLog = null)
        {
            TimeLog log = new TimeLog("FillWHotelComment", 300, parentLog);
            try
            {
                List<int> whWritingList = new List<int>();

                try
                {
                    whWritingList = qrr.ReviewList.Where(o => o.WritingTypeID == 3).Select(r => r.Writing).ToList();
                }
                catch
                { }

                if (whWritingList.Count > 0)
                {
                    List<CommentInfoEntity> WHCommentList = CommentAdapter.GetComments(whWritingList);
                    log.AddLog("GetComments");

                    foreach (CommentInfoEntity c in WHCommentList)
                    {
                        try
                        {
                            HotelReviewExEntity r = qrr.ReviewList.Where(o => o.WritingTypeID == 3 && o.Writing == c.Comment.ID).First();

                            r.CommentTitle = DescriptionHelper.CommentInfoConcat(9, c);

                            if (string.IsNullOrWhiteSpace(r.CommentTitle))
                            {
                                r.Content = GetOrGenWHHotelCommentDescription(c);
                                log.AddLog("GetOrGenWHHotelCommentDescription");

                            }
                            else
                            {
                                r.Content = DescriptionHelper.CommentInfoConcat(10, c);//点评详细内容
                                r.HoContent = DescriptionHelper.CommentInfoConcat(11, c);//补充点评内容
                            }
                            r.PhotoCount = c.PhotoIDs.Count();
                            r.RoomInfo = string.Join("、", c.RoomInfo.Select(o => o.TagName));

                            var tripResult = c.TagInfo.FindAll(_ => _.CategoryID == 8).FirstOrDefault();
                            if (tripResult != null && tripResult.CategoryID > 0 && tripResult.Tags.Count > 0)
                            {
                                r.TripInfo = string.Join("、", tripResult.Tags.Select(_ => _.TagName));
                            }

                            //还要小图 和 大图链接的数组
                            r.CommentPics = c.CommentPics;
                            r.BigCommentPics = c.BigCommentPics;
                            //还要有帮助数量
                            r.UsefulCount = c.Comment.UsefulCount;
                        }
                        catch (Exception e)
                        {
                            Log.WriteLog("error 生成点评列表用点评描述，不带住房信息:" + e);
                        }
                    }
                }

                //统计一下 其他渠道点评的点有用数量
                //List<int> hotelReviewIDs = qrr.ReviewList.Where(o => o.WritingTypeID == 1).Select(r => r.Writing).ToList();
                //List<int> otaHotelReviewIDs = qrr.ReviewList.Where(o => o.WritingTypeID == 2).Select(r => r.Writing).ToList();                
            }
            catch (Exception err)
            {
                Log.WriteLog(err.Message + err.StackTrace);
            }

            log.Finish();
        }

        //1	酒店的总体印象
        //2	我住的房型是
        //3	对房间设施的评价
        //4	有那些好玩的
        //5	有哪些好吃的
        public static String GetOrGenWHHotelCommentDescription(CommentInfoEntity c)
        {
            //如果数据库中Comment为空，那么重新生成这个字段内容
            if (string.IsNullOrWhiteSpace(c.Comment.Comment))
            {
                CommentInfoEntity tempC = CommentAdapter.GenWHHotelCommentDescription(c.Comment.ID);
                return tempC.Comment.Comment;
            }
            else
            {
                return c.Comment.Comment.Replace("<P/>", "</P>").Replace("<p/>", "</p>");
            }
        }

        public ReviewResult2 GenHotelReviews2Model(ReviewQueryParam par)
        {
            ArguHotelReview p = new ArguHotelReview();
            p.Hotel = par.Hotel;
            p.RatingType = HJD.HotelServices.Contracts.RatingType.All;
            List<HotelTFTRelItemEntity> tftList = GetHotelTFTList(par.Hotel, par.InterestID);
            p.NeedReview = 1;//不需要点评数据

            QueryReviewResult qrr = HotelService.QueryReview(p);


            long FilterBase = 10000000000;
            long GoodKey = FilterBase * 2 + 13;
            long NormalKey = FilterBase * 2 + 12;
            long BadKey = FilterBase * 2 + 11;

            int good = GetFilterCount(qrr.FilterCount, GoodKey);
            int normal = GetFilterCount(qrr.FilterCount, NormalKey);
            int bad = GetFilterCount(qrr.FilterCount, BadKey);
            float total = good + normal + bad;
            string RatingPercent = string.Format("Good:{0},Normal:{1},Bad:{2}",
                   CalPercent(good, total),
                   CalPercent(normal, total),
                   100 - CalPercent(good, total) - CalPercent(normal, total)
                   );

            var model = new ReviewResult2();
            model.RatingPercent = RatingPercent;
            model.HotelID = par.Hotel;
            model.TotalCount = qrr.TotalCount;
            HotelItem hi = ResourceAdapter.GetHotel(par.Hotel, UserId);
            model.Score = hi.Score;
            model.TFTList = tftList;
            return model;
        }

        private int CalPercent(int part, float total)
        {
            return (int)((part / total + 0.005) * 100);
        }

        private int GetFilterCount(Dictionary<long, int> FilterCount, long key)
        {
            return FilterCount.ContainsKey(key) ? FilterCount[key] : 0;
        }

        private List<HotelTFTRelItemEntity> GetHotelTFTList(int hotelID, int interestID)
        {

            return HotelService.GetHotelInterestTag(hotelID, interestID).Select(t => new HotelTFTRelItemEntity { HotelID = t.HotelID, TFTID = t.TFTID, TFTName = t.TFTName, Type = t.Type }).ToList();
            //List<HotelTFTRelItemEntity> tftList = HotelService.GetHotelTFTRel(hotelID);

            //return GetHotelTFTList(tftList, interestID);
        }

        private List<HotelTFTRelItemEntity> GetHotelTFTList40(int hotelID, int type)
        {
            if (type == 0)
            {
                return new List<HotelTFTRelItemEntity>();
            }
            List<HotelTagInfoEntity> tl = HotelService.GetHotelShowTags(hotelID);
            return tl.Where(t => t.CategoryID == type).
                Select(t => new HotelTFTRelItemEntity { HotelID = t.Hotelid, TFTID = t.Featuredid, TFTName = t.FeaturedName, Type = t.CategoryID, CommentCount = t.CommentCount, TagType = t.TagType })
                .OrderBy(s => s.TagType)
                .ThenByDescending(s => s.CommentCount).ToList();
        }

        private List<HotelTFTRelItemEntity> GetHotelTFTList(List<HotelTFTRelItemEntity> tftList, int InterestID)
        {
            if (tftList.Count > 0)
            {
                if (!(tftList.First().Type == 3 && tftList.First().TFTID == InterestID))
                {
                    HotelTFTRelItemEntity htft = tftList.Where(c => c.Type == 3 && c.TFTID == InterestID).FirstOrDefault();
                    if (htft != null)
                    {
                        tftList.Remove(htft);
                        tftList.Insert(0, htft);
                    }
                }
            }

            return tftList;
        }

        /// <summary>
        /// 获取酒店点评
        /// </summary>
        /// <param name="hotel">酒店id</param>
        /// <param name="review">点评id</param>
        /// <param name="start"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public ReviewResult GetHotelReviews(ArguHotelReview p)
        {
            p.HotelReviewOrderType = HotelReviewOrderType.Time_Down;
            if (p.TFTType > 0)
            {
                switch (p.TFTType)
                {
                    case 1:
                        p.Tag = p.TFTID;
                        break;
                    case 2:
                        p.Featured = p.TFTID;
                        break;
                    case 3:
                        p.Theme = p.TFTID;
                        break;
                }
            }

            QueryReviewResult qrr = HotelService.QueryReview(p);

            return GenHotelReviews(qrr.ReviewList, qrr.TotalCount, p.Start, p.Hotel, p.RatingType, p.Featured);
        }

        public ReviewResult GenHotelReviews(List<HotelReviewExEntity> ReviewList, int TotalCount, int Start, int Hotel, HJD.HotelServices.Contracts.RatingType RatingType, int Featured)
        {

            var model = new ReviewResult();

            model.TotalCount = TotalCount;
            model.Start = Start;

            model.Result = transReviewExToCommentItem(ReviewList, Hotel).ToList();
            model.DistrictName = HotelService.GetHotel(Hotel).DistrictName;
            HotelItem hi = ResourceAdapter.GetHotel(Hotel, UserId);
            model.HotelName = hi.Name;
            model.Score = hi.Score;

            model.RatingType = RatingType > 0 ? GetRatingTypeName(RatingType) : "";
            model.Featured = Featured > 0 ? GetFeaturedName(Featured) : "";
            model.OTAInfos = HotelHelper.GetHotelOTAInfo(Hotel, "app", true);


            return model;
        }

        private IEnumerable<CommentItem> transReviewExToCommentItem(List<HotelReviewExEntity> ReviewList, int hotelid, TimeLog parentLog = null)
        {
            TimeLog log = new TimeLog("transReviewExToCommentItem", 200, parentLog);
            List<CommentItem> comments = new List<CommentItem>();
            List<OTAInfo> lo = HotelHelper.GetHotelOTAInfo(hotelid, "app", true);
            try
            {
                //Log.WriteLog("transReviewExToCommentItem userList：" + string.Join(",", ReviewList.Select(_ => _.UserID).ToList()));
                
                List<long> userIds = ReviewList.Where(u => u.UserID != 0).Select(_ => _.UserID).Distinct().ToList();
                if (userIds != null && userIds.Count > 0)
                {
                    var userList = AccountAdapter.GetMultiMemberProfileInfo(userIds);
                    log.AddLog("GetHotelOTAInfo");
                    foreach (HotelReviewExEntity item in ReviewList)
                    {
                        if (item != null)
                        {
                            CommentItem comm = new CommentItem();
                            comm.Author = StringHelper.UidMask(item.Uid);// ((HJDAPI.Common.Helpers.Enums.OTAUserName)p.ChannelType).ToString() ,
                            comm.Id = item.Writing;
                            comm.Score = item.WritingTypeID == 3 ? (decimal)item.Score : Decimal.Round(item.Rating, 1);
                            comm.PhotoCount = item.PhotoCount;
                            comm.RoomInfo = item.RoomInfo;
                            comm.TripInfo = item.TripInfo;
                            comm.Text = item.Content;
                            comm.AdditionalText = string.IsNullOrWhiteSpace(item.HoContent) ? "" : item.HoContent;
                            comm.Time = item.ChannelType == 26 ? "" : item.WDate.ToString("yyyy-MM-dd");
                            comm.ChannelID = item.ChannelType;
                            comm.ChannelName = ((HJDAPI.Common.Helpers.Enums.enumOTAName)item.ChannelType).ToString();
                            comm.OTAAccessUrl = GetOTAAccessUrl(lo, item.ChannelType);
                            comm.ScoreDetail = String.Format("{0},{1},{2},{3}", item.RatingRoom, item.RatingService, item.RatingCostBenefit, item.RatingAtmosphere);
                            comm.CommentPics = item.CommentPics;
                            comm.BigCommentPics = item.BigCommentPics;
                            comm.UsefulCount = item.UsefulCount;
                            comm.HasClickUseful = false;
                            comm.AuthorUserID = item.UserID;
                            comm.CommentTitle = item.CommentTitle;
                            comm.AvatarUrl = (userList.Count == 0 || userList.Where(_ => _.UserID == item.UserID).Count() == 0 || string.IsNullOrWhiteSpace(userList.Where(_ => _.UserID == item.UserID).First().AvatarUrl)) ? DescriptionHelper.defaultAvatar :
                                PhotoAdapter.GenHotelPicUrl(userList.Where(_ => _.UserID == item.UserID).First().AvatarUrl, Enums.AppPhotoSize.jupiter);
                            comments.Add(comm);
                        }
                        //else
                        //{
                        //    Log.WriteLog("无数据" );
                        //}
                    }

                }
            }
            catch (Exception e)
            {
                Log.WriteLog("transReviewExToCommentItem " + ReviewList.Count + e);
            }
            //-------ReviewList集合中，可能会有数据为null
            //var comments = ReviewList.Select(c => new CommentItem
            //{
            //    Author = StringHelper.UidMask(c.Uid),// ((HJDAPI.Common.Helpers.Enums.OTAUserName)p.ChannelType).ToString() ,
            //    Id = c.Writing,
            //    Score = c.WritingTypeID == 3 ? (decimal)c.Score : Decimal.Round(c.Rating, 1),
            //    PhotoCount = c.PhotoCount,
            //    RoomInfo = c.RoomInfo,
            //    TripInfo = c.TripInfo,
            //    Text = c.Content,
            //    AdditionalText = string.IsNullOrWhiteSpace(c.HoContent) ? "" : c.HoContent,
            //    Time = c.ChannelType == 26 ? "" : c.WDate.ToString("yyyy-MM-dd"),
            //    ChannelID = c.ChannelType,
            //    ChannelName = ((HJDAPI.Common.Helpers.Enums.enumOTAName)c.ChannelType).ToString(),
            //    OTAAccessUrl = GetOTAAccessUrl(lo, c.ChannelType),
            //    ScoreDetail = String.Format("{0},{1},{2},{3}", c.RatingRoom, c.RatingService, c.RatingCostBenefit, c.RatingAtmosphere),
            //    CommentPics = c.CommentPics,
            //    BigCommentPics = c.BigCommentPics,
            //    UsefulCount = c.UsefulCount,
            //    HasClickUseful = false,
            //    AuthorUserID = c.UserID,
            //    CommentTitle = c.CommentTitle,
            //    AvatarUrl = AccountAdapter.GetCurrentUserInfo(c.UserID).AvatarUrl
            //});
            log.Finish();
            return comments;
        }

        private string GetOTAAccessUrl(List<OTAInfo> lo, int ChannelID)
        {
            OTAInfo otaInfo = lo.Where(o => o.ChannelID == ChannelID).FirstOrDefault();
            if (otaInfo != null)
                return otaInfo.AccessURL;
            else
                return "";

        }

        private string GetFeaturedName(int fid)
        {
            return LocalCache.GetData<List<FeaturedEntity>>("FeaturedList", () =>
             {
                 return HotelService.GetFeaturedList();
             }).Where(f => f.ID == fid).First().Name;
        }

        private string GetRatingTypeName(HJD.HotelServices.Contracts.RatingType ratingType)
        {
            switch (ratingType)
            {
                case HJD.HotelServices.Contracts.RatingType.All:
                    return "";
                    break;
                case HJD.HotelServices.Contracts.RatingType.Best:
                    return "4.5 - 5分";
                    break;
                case HJD.HotelServices.Contracts.RatingType.Better:
                    return "3.5 - 4.5分";
                    break;
                case HJD.HotelServices.Contracts.RatingType.Good:
                    return "2.5 - 3.5分";
                    break;
                case HJD.HotelServices.Contracts.RatingType.Normal:
                    return "1.5 - 2.5分";
                    break;
                case HJD.HotelServices.Contracts.RatingType.Terrible:
                    return "0 - 1.5分";
                    break;
            }

            return "";

        }

        public static HotelPhotosEntity GetHotelPhotos(int HotelID, int InterestID)
        {
            HotelPhotosEntity phe = HotelService.GetHotelPhotos(HotelID);

            List<HotelPhotoEntity> gagaList = new List<HotelPhotoEntity>();
            //if (InterestID > 0)
            //{
            //    HJD.HotelManagementCenter.Domain.HomeThemeHotelEntity hthe = HMC_HotelService.GetHomeThemeHotelEntityList(HotelID, InterestID).FindAll(_ => !_.IsDel).FirstOrDefault();
            //    if (hthe != null && !string.IsNullOrWhiteSpace(hthe.ShortUrl))
            //    {
            //        gagaList.Add(new HotelPhotoEntity() { SURL = hthe.ShortUrl, Deleted = false, HotelID = HotelID, Title = "主题" });
            //    }
            //}
            if (gagaList != null && gagaList.Count != 0)
            {
                gagaList.AddRange(phe.HPList);
                phe.HPList = gagaList;
            }
            foreach (HotelPhotoEntity hp in phe.HPList)
            {
                hp.URL = PhotoAdapter.GenHotelPicUrl(hp.SURL, Enums.AppPhotoSize.appview);
            }
            return phe;
        }

        public static List<HotelPhotosEntity> GetManyHotelPhotos(IEnumerable<int> hotelIds)
        {
            return HotelService.GetManyHotelPhotos(hotelIds);//hotelService内部有按酒店ID获取照片 并缓存酒店照片数据
        }

        public List<QuickSearchSuggestItem> SuggestCityAndHotel(string keyword, int count)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return new List<QuickSearchSuggestItem>();
            }
            var list = SearchTipApiService.GetListSearchTipAllType(keyword, count);
            var res = (from d in list
                       select new QuickSearchSuggestItem
                       {
                           EName = d.EngName,
                           ParentName = d.DisName,
                           Name = d.CnName,
                           Type = d.Type,
                           Id = int.Parse(d.Id)
                       }).ToList();
            return res;
        }

        /// <summary>
        /// 发布酒店点评
        /// </summary>
        /// <param name="id">酒店id</param>
        /// <param name="name">酒店名称</param>
        /// <param name="title">标题</param>
        /// <param name="text">内容</param>
        /// <param name="ratingRoom">房间卫生</param>
        /// <param name="ratingAtmosphere">周边环境</param>
        /// <param name="ratingService">酒店服务</param>
        /// <param name="ratingCostBenefit">设施设备</param>
        /// <param name="tripRadio">出游类型</param>
        /// <param name="identity"></param>
        /// <param name="recommend">是否推荐</param>
        /// <returns></returns>
        public OperationResult PostReview(HotelReviewItem r)
        {

            var result = new OperationResult();



            HotelReviewEntity hr = new HotelReviewEntity();

            //    rate=0&ratingRoom={0}&ratingAtmosphere={1}&ratingService={2}&ratingCostBenefit={3}&tripRadio={4}&reviewTitle={5}&reviewContent={6}&resourceName={7}&resource={8}
            //&identitytxt={9}&recRadio={10}&uid={11}&key={12}&source={13}",
            hr.RatingRoom = r.ratingRoom;
            hr.RatingAtmosphere = r.ratingAtmosphere;
            hr.RatingService = r.ratingService;
            hr.RatingCostBenefit = r.ratingCostBenefit;
            hr.CommentSubject =
                hr.Title = r.title;// HttpUtility.UrlEncode(r.title, Encoding.GetEncoding("GB2312"));
            hr.Content = r.text;// HttpUtility.UrlEncode(r.text, Encoding.GetEncoding("GB2312"));
            hr.RoomName = r.name;// HttpUtility.UrlEncode(r.name, Encoding.GetEncoding("GB2312"));
            hr.Hotel = r.id;
            hr.WholeRate = r.wholeRate;
            hr.NewisCommand = r.recommend ? "T" : "F";
            hr.Uid = Uid;
            hr.UserID = AccountAdapter.GetCurrentUserID();
            hr.Source = r.source;
            hr.Deleted = "F";
            hr.Status = "P";
            hr.StatusDetail = 1;



            int user_Identity = 0;
            string identitytxt = "";
            switch (r.tripRadio)
            {
                case "10":
                    user_Identity = 10;
                    identitytxt = "商务出差";
                    break;
                case "20":
                    user_Identity = 20;
                    identitytxt = "带小孩出游";
                    break;
                case "30":
                    user_Identity = 30;
                    identitytxt = "和家人出游";
                    break;
                case "40":
                    user_Identity = 40;
                    identitytxt = "和朋友出游";
                    break;
                case "50":
                    user_Identity = 50;
                    identitytxt = "独自出游";
                    break;
                case "60":
                    user_Identity = 60;
                    identitytxt = "代人预订";
                    break;
                case "70":
                    user_Identity = 70;
                    identitytxt = "夫妇、情侣出游";
                    break;
                default:
                    user_Identity = 0;
                    identitytxt = r.identity != null && r.identity != "" ? r.identity : "其他";
                    break;
            }
            hr.User_Identity = user_Identity;
            hr.Identitytxt = identitytxt;

            HotelReviewSaveResult hrs = HotelService.HotelReviewSubmitToHotelDB(hr);

            if (hrs.Writing > 0)
            {
                result.Success = true;
                result.Data = hrs.Writing.ToString();
            }
            return result;
        }

        /// <summary>
        /// 获取目的地中档价格区间段(最低价和最高价）
        /// </summary>
        /// <param name="districtID"></param>
        /// <returns></returns>
        public List<int> GetPriceSectionMinMax(int districtID)
        {
            return LocalCache.GetData<List<int>>("PriceSecMM" + districtID.ToString(), () =>
            {
                return HotelService.GetZhongdangPriceSection(districtID);
            });
        }
        public static List<PDayItem> GenHotelPackageCalendar(int hotelid, DateTime startDate, int pid = 0, int canlendarLength = 180, HJD.HotelServices.Contracts.HotelServiceEnums.PricePolicyType ppt = HJD.HotelServices.Contracts.HotelServiceEnums.PricePolicyType.Default)
        {
            return HotelService.GenHotelPackageCalendar(hotelid, startDate, pid, canlendarLength);
        }

        //public int[] GetPriceSection(int type , int districtID)
        //{
        //   Dictionary<int,int> pl =  LocalCache.GetData < Dictionary<int, int>>("PriceSec" + districtID.ToString(), () =>
        //    {  
        //        HotelSearchParas p = new HotelSearchParas();
        //        p.DistrictID = districtID;
        //        p.ClassID = new int[1];
        //        p.ClassID[0] = 0;
        //        p.Valued = 0;
        //        p.MinPrice = 0;
        //        p.MaxPrice = 0;

        //        Dictionary<int,string> fp =   GetWapMenuEntity( p,needTagLength).filterPrice;
        //        Dictionary<int, int> lp = new Dictionary<int, int>();
        //        foreach (int k in fp.Keys)
        //        {
        //            lp.Add(k, int.Parse(fp[k].Split('|')[0]));
        //        }
        //        lp.Add(fp.Count + 1, int.Parse(fp[fp.Count].Split('|')[1]));
        //        return lp;
        //    });
        //    int minPrice = 0;
        //    int maxPrice = 0;
        //    switch (type)
        //    {
        //        case 200:
        //            minPrice = 0;
        //            maxPrice = pl[1];
        //            break;
        //        case 201:
        //            minPrice = pl[1];
        //            maxPrice = pl[pl.Count/2 + 1];
        //            break;
        //        case 202:
        //            minPrice = pl[pl.Count / 2 + 1];
        //            maxPrice = pl[pl.Count];
        //            break;
        //        case 203:
        //            minPrice = pl[pl.Count ];
        //            maxPrice = MaxHotelPrice;
        //            break;
        //    }

        //    return new int[] { minPrice, maxPrice };

        //}

        public AppMenuEntity GetWapMenuEntity(HotelSearchParas p, int needTagLength)
        {
            return LocalCache.GetData<AppMenuEntity>(string.Format("WapMenu{0}", p.Attraction), () =>
            {
                AppMenuEntity menu = new AppMenuEntity();

                HotelSearchParas tp = new HotelSearchParas();

                tp.Attraction = p.Attraction;
                tp.Type = 0;
                tp.NeedFilterCol = true;
                tp.NeedHotelID = false; //不需要返回酒店列表
                tp.CheckInDate = DateTime.Now;
                tp.CheckOutDate = DateTime.Now;


                List<FilterDicEntity> flist = HotelService.GetQueryHotelFilters(tp);

                if (flist.Count > 0)
                {
                    menu.TotalNum = flist.Where(o => o.Key == Struct.HotelListFilterPrefix.Attraction + p.Attraction).FirstOrDefault().Num;

                    menu.Attractions = (from b in flist
                                        where b.Type == (int)(Struct.HotelListFilterPrefix.Attraction / Struct.HotelListFilterPrefix.BaseOffset)
                                        orderby b.Num descending
                                        select b).ToList();
                }


                List<HotelTypeDefine> hotelClassDefine = GetHotelClassDefine();

                foreach (HotelTypeDefine hc in hotelClassDefine)
                {
                    TypeItem item = new TypeItem();
                    tp.Type = hc.TypeID;
                    tp.ClassID = (from c in hc.SubHotelClass
                                  select c.Key).ToArray();

                    flist = HotelService.GetQueryHotelFilters(tp);


                    item.TypeName = hc.Name;
                    item.TypeID = hc.TypeID;
                    FilterDicEntity TotalNumItem = flist.Where(o => o.Key == Struct.HotelListFilterPrefix.Attraction + p.Attraction).FirstOrDefault();
                    if (TotalNumItem != null)
                    {
                        item.TotalNum = TotalNumItem.Num;

                        item.FeaturedList = (from b in flist
                                             where b.Type == (int)(Struct.HotelListFilterPrefix.Featured / Struct.HotelListFilterPrefix.BaseOffset)
                                             orderby b.Num descending
                                             select b).Take(needTagLength).ToList();

                        menu.TypeItems.Add(item);
                    }
                }

                return menu;
            });
        }

        public List<HotelTypeDefine> GetHotelClassDefine()
        {
            return LocalCache.GetData<List<HotelTypeDefine>>("HotelClassDefine", () =>
            {
                List<HotelTypeDefine> list = new List<HotelTypeDefine>();
                foreach (string c in Configs.AppHotelClass.Split('|'))
                {
                    HotelTypeDefine item = new HotelTypeDefine();
                    item.TypeID = int.Parse(c.Split(',')[0]);
                    item.Name = c.Split(',')[1];
                    item.SubHotelClass = new Dictionary<int, string>();
                    foreach (string sc in c.Split(',')[2].Split('+'))
                    {
                        item.SubHotelClass.Add(int.Parse(sc.Split(':')[0]), sc.Split(':')[1]);
                    }
                    list.Add(item);
                }
                return list;
            });
        }

        /// <summary>
        /// 转换货币代码与符号
        /// </summary>
        /// <param name="currency"></param>
        /// <returns></returns>
        private string TransCurrencyToSymbol(string currency)
        {

            return "￥";
        }

        //<summary>
        //将抓取的点评送往生产环境
        //</summary>

        public string MoveReview_17U(MoveReview17U mr)
        {
            try
            {
                HotelService.MoveReview17U(mr);
            }
            catch (Exception e)
            {
                return e.Message;
            }
            return "Success";
        }

        protected static string GetSuggestType(string type)
        {
            switch (type)
            {
                case "D":
                    return "City";
                case "S":
                    return "Sight";
                case "H":
                    return "Hotel";
                default:
                    return "City";
            }
        }

        protected static int[] ParseIntArray(string str)
        {
            if (str == null) return null;
            List<int> result = new List<int>();
            foreach (string i in str.Split(','))
            {
                if (i.Length > 0)
                {
                    int t = -1;
                    if (int.TryParse(i, out t))
                    {
                        result.Add(t);
                    }

                }
            }

            return result.ToArray();
        }

        /// <summary>
        /// 由酒店ID集合获得酒店列表清单
        /// </summary>
        /// <param name="hotelIdList"></param>
        /// <returns></returns>
        public static List<ListHotelItem2> GetCollectHotelList(List<int> hotelIDs)
        {
            return HotelService.GetCollectHotelList(hotelIDs);
        }

        /// <summary>
        /// 房券积分列表
        /// </summary>
        /// <param name="hotelIdList"></param>
        /// <returns></returns>
        public static List<VRateEntity> GetVRateByHVID(int HVID)
        {
            return HotelService.GetVRateByHVID(HVID);
        }


        public static List<HotelPriceEntity> QueryHotelListPrice(List<int> hotelIds, DateTime arrivalTime, DateTime departureTime)
        {
            return PriceService.QueryHotelListPrice(hotelIds, arrivalTime, departureTime);
        }

        public static List<TopNPackageItem> GetTop20Package(int count = 25, int albumsId = 1, int start = 0, bool isShowVipFirstBuyPackage = true, bool isNeedNotSale = false, string dateStr = "", int gotoDistrictID = 0, int startDistrictID = 0)
        {
            List<TopNPackageItem> list = HotelService.GetTopNPackageList(true, start, count, albumsId, isShowVipFirstBuyPackage, isNeedNotSale, dateStr,gotoDistrictID,startDistrictID);
            genTopNPackageItemPicURL(list);
            return list;
            //return LocalCache10Min.GetData<List<TopNPackageItem>>(string.Format("HotelAdapter:GetTop20Package_{0}_{1}_{2}_{3}", count, albumsId, start, isShowVipFirstBuyPackage), () =>
            //{
            //    List<TopNPackageItem> list = HotelService.GetTopNPackageList(true, start, count, albumsId, isShowVipFirstBuyPackage);
            //    genTopNPackageItemPicURL(list);
            //    return list;
            //});
        }
        public static List<TopNPackageItem> GetTop20GroupPackage(int count = 25, int albumsId = 1, int start = 0, bool isShowVipFirstBuyPackage = true, bool isNeedNotSale = false, string dateStr = "", int gotoDistrictID = 0, int startDistrictID = 0,int pid=0)
        {
            List<TopNPackageItem> list = HotelService.GetTopNGroupPackageList(true, start, count, albumsId, isShowVipFirstBuyPackage, isNeedNotSale, dateStr, gotoDistrictID, startDistrictID,pid);
            genTopNPackageItemPicURL(list);
            return list;
            //return LocalCache10Min.GetData<List<TopNPackageItem>>(string.Format("HotelAdapter:GetTop20Package_{0}_{1}_{2}_{3}", count, albumsId, start, isShowVipFirstBuyPackage), () =>
            //{
            //    List<TopNPackageItem> list = HotelService.GetTopNPackageList(true, start, count, albumsId, isShowVipFirstBuyPackage);
            //    genTopNPackageItemPicURL(list);
            //    return list;
            //});
        }
        public static List<TopNPackageItem> GetTop20PackageCached(int count = 25, int albumsId = 1, int start = 0, bool isShowVipFirstBuyPackage = true, bool isNeedNotSale = false, string dateStr = "", int gotoDistrictID = 0, int startDistrictID = 0)
        {
            return LocalCache10Min.GetData<List<TopNPackageItem>>(string.Format("HotelAdapter:GetTop20Package_{0}_{1}_{2}_{3}_{4}_{5}_{6}_{7}", count, albumsId, start, isShowVipFirstBuyPackage, isNeedNotSale, dateStr, gotoDistrictID, startDistrictID), () =>
            {

                return GetTop20Package(count ,  albumsId ,  start ,  isShowVipFirstBuyPackage, isNeedNotSale,dateStr , gotoDistrictID ,startDistrictID);
            });
        }

        //public static void RemoveGetTop20PackageCache(int count = 25, int albumsId = 1, int start = 0, bool isShowVipFirstBuyPackage = true)
        //{
        //    string key = string.Format(string.Format("HotelAdapter:GetTop20Package_{0}_{1}_{2}_{3}", count, albumsId, start, isShowVipFirstBuyPackage));
        //    LocalCache10Min.Remove(key);
        //}
        public static List<TopNPackageItem> GetTop20PackageByDistrictIds(int count = 25, int albumsId = 1, int start = 0, string districtIds = "", bool isShowVipFirstBuyPackage = true, string dateStr = "", int gotoDistrictID = 0)
        {
            List<TopNPackageItem> list = HotelService.GetTopNPackageListByDistrictIds(true, start, count, albumsId, districtIds, isShowVipFirstBuyPackage);
            genTopNPackageItemPicURL(list);
            return list;
            //return LocalCache10Min.GetData<List<TopNPackageItem>>(string.Format("HotelAdapter:GetTop20PackageByDistrictIds{0}_{1}_{2}_{3}_{4}", count, albumsId, start, isShowVipFirstBuyPackage, districtIds), () =>
            //{
            //    List<TopNPackageItem> list = HotelService.GetTopNPackageListByDistrictIds(true, start, count, albumsId, districtIds, isShowVipFirstBuyPackage);
            //    genTopNPackageItemPicURL(list);
            //    return list;
            //});
        }
        public static List<TopNPackageItem> GetTop20GroupPackageByDistrictIds(int count = 25, int albumsId = 1, int start = 0, string districtIds = "", bool isShowVipFirstBuyPackage = true, string dateStr = "", int gotoDistrictID = 0)
        {
            List<TopNPackageItem> list = HotelService.GetTopNGroupPackageListByDistrictIds(true, start, count, albumsId, districtIds, isShowVipFirstBuyPackage);
            genTopNPackageItemPicURL(list);
            return list;
            //return LocalCache10Min.GetData<List<TopNPackageItem>>(string.Format("HotelAdapter:GetTop20PackageByDistrictIds{0}_{1}_{2}_{3}_{4}", count, albumsId, start, isShowVipFirstBuyPackage, districtIds), () =>
            //{
            //    List<TopNPackageItem> list = HotelService.GetTopNPackageListByDistrictIds(true, start, count, albumsId, districtIds, isShowVipFirstBuyPackage);
            //    genTopNPackageItemPicURL(list);
            //    return list;
            //});
        }

        public static ScreenConditionsEntity GetAlbumFilter(int albumId = 12)
        {
            return GenAlbumFilter(albumId);
            return LocalCache.GetData<ScreenConditionsEntity>("GetAlbumScreen" + albumId, () =>
                {
                    return GenAlbumFilter(albumId);
                });
        }

        public static ScreenConditionsEntity GenAlbumFilter(int albumId = 12)
        {
            ScreenConditionsEntity model = new ScreenConditionsEntity();
            //model.StartCityList.Add(new HJD.HotelServices.Contracts.CityEntity() { ID = 2, Name = "上海" });
            List<AlbumPackageSimpleEntity> packageList = HotelService.GetTopNPackageScreenList(albumId);

            if (albumId == 12)
            {
                if (packageList != null && packageList.Count > 0)
                {
                    #region 日期遍历 已注释
                    AlbumPackageSimpleEntity firstPackage = packageList.OrderByDescending(_ => _.EndDate).First();
                    int sysYear = System.DateTime.Now.Year;
                    int sysMonth = System.DateTime.Now.Month;
                    int endYear = firstPackage.EndDate.Year;
                    int endMonth = firstPackage.EndDate.Month;
                    if (sysYear < endYear)
                    {
                        for (int i = sysYear; i <= endYear; i++)
                        {
                            YearEntity yearEntity = new YearEntity();
                            yearEntity.Year = i;
                            yearEntity.State = 0;
                            if (i == endYear)
                            {
                                for (int m = 1; m <= endMonth; m++)
                                {
                                    MonthEntity monthEntity = new MonthEntity();
                                    monthEntity.Month = m;
                                    monthEntity.State = 0;
                                    yearEntity.MonthList.Add(monthEntity);
                                }
                            }
                            else if (i == sysYear)
                            {
                                for (int m = sysMonth; m <= 12; m++)
                                {
                                    MonthEntity monthEntity = new MonthEntity();
                                    monthEntity.Month = m;
                                    monthEntity.State = 0;
                                    yearEntity.MonthList.Add(monthEntity);
                                }
                            }
                            else
                            {
                                for (int m = 1; m <= 12; m++)
                                {
                                    MonthEntity monthEntity = new MonthEntity();
                                    monthEntity.Month = m;
                                    monthEntity.State = 0;
                                    yearEntity.MonthList.Add(monthEntity);
                                }
                            }
                            model.Calendar.Add(yearEntity);
                        }
                    }
                    else
                    {
                        YearEntity yearEntity = new YearEntity();
                        yearEntity.Year = endYear;
                        yearEntity.State = 0;
                        for (int m = sysMonth; m <= endMonth; m++)
                        {
                            MonthEntity monthEntity = new MonthEntity();
                            monthEntity.Month = m;
                            monthEntity.State = 0;
                            yearEntity.MonthList.Add(monthEntity);
                        }
                        model.Calendar.Add(yearEntity);
                    }
                    #endregion
                }
            }

            foreach (var item in packageList)
            {
                HJD.HotelServices.Contracts.CityEntity cityModel = new HJD.HotelServices.Contracts.CityEntity();
                cityModel.Name = item.DistrictName;
                cityModel.ID = item.DistrictID;
                int gotoCount = model.GoToCityList.Where(_ => _.ID == item.DistrictID).Count();
                if (gotoCount == 0)
                {
                    model.GoToCityList.Add(cityModel);
                }

                if (item.StartDistrictId != 0)
                {
                    HJD.HotelServices.Contracts.CityEntity startCityModel = new HJD.HotelServices.Contracts.CityEntity();
                    startCityModel.Name = item.StartDistrictName;
                    startCityModel.ID = item.StartDistrictId;
                    int startCount = model.StartCityList.Where(_ => _.ID == item.StartDistrictId).Count();
                    if (startCount == 0)
                    {
                        model.StartCityList.Add(startCityModel);
                    }
                }
                if (albumId == 12)
                {
                    #region 日期遍历
                    int sysYear = System.DateTime.Now.Year;
                    int sysMonth = System.DateTime.Now.Month;
                    int endYear = item.EndDate.Year;
                    int endMonth = item.EndDate.Month;
                    int startYear = item.StartDate.Year;
                    int startMonth = item.StartDate.Month;
                    if (sysYear < endYear)
                    {
                        for (int i = startYear; i <= endYear; i++)
                        {
                            foreach (YearEntity itemyear in model.Calendar.Where(_ => _.Year == i).ToList())
                            {
                                itemyear.State = 1;
                                foreach (MonthEntity monthitem in itemyear.MonthList)
                                {
                                    if (i == endYear)
                                    {
                                        for (int m = 1; m <= endMonth; m++)
                                        {
                                            if (monthitem.Month == m)
                                            {
                                                monthitem.State = 1;
                                            }
                                        }
                                    }
                                    else if (i == startYear)
                                    {
                                        for (int m = startMonth; m <= 12; m++)
                                        {
                                            if (monthitem.Month == m)
                                            {
                                                monthitem.State = 1;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        for (int m = 1; m <= 12; m++)
                                        {
                                            if (monthitem.Month == m)
                                            {
                                                monthitem.State = 1;
                                            }
                                        }
                                    }
                                }
                            }
                            //YearEntity yearEntity = new YearEntity();
                            //yearEntity.Year = i;
                            //yearEntity.state = 0;
                            //if (i == endYear)
                            //{
                            //    for (int m = sysMonth; m <= endMonth; m++)
                            //    {
                            //        MonthEntity monthEntity = new MonthEntity();
                            //        monthEntity.Month = m;
                            //        monthEntity.state = 0;
                            //        yearEntity.MonthList.Add(monthEntity);
                            //    }
                            //}
                            //else if (i == startYear)
                            //{
                            //    for (int m = sysMonth; m <= 12; m++)
                            //    {
                            //        MonthEntity monthEntity = new MonthEntity();
                            //        monthEntity.Month = m;
                            //        monthEntity.state = 0;
                            //        yearEntity.MonthList.Add(monthEntity);
                            //    }
                            //}
                            //else
                            //{
                            //    for (int m = 1; m <= 12; m++)
                            //    {
                            //        MonthEntity monthEntity = new MonthEntity();
                            //        monthEntity.Month = m;
                            //        monthEntity.state = 0;
                            //        yearEntity.MonthList.Add(monthEntity);
                            //    }
                            //}
                        }
                    }
                    else
                    {
                        foreach (YearEntity itemyear in model.Calendar.Where(_ => _.Year == sysYear).ToList())
                        {
                            itemyear.State = 1;
                            foreach (MonthEntity monthitem in itemyear.MonthList)
                            {
                                for (int m = sysMonth; m <= 12; m++)
                                {
                                    if (monthitem.Month == m)
                                    {
                                        monthitem.State = 1;
                                    }
                                }
                            }
                        }
                    }
                    #endregion

                }
            }
            return model;
        }
        //public static void RemoveGetTop20PackageByDistrictIds(int count = 25, int albumsId = 1, int start = 0, string districtIds = "", bool isShowVipFirstBuyPackage = true)
        //{
        //    string key = string.Format(string.Format("HotelAdapter:GetTop20PackageByDistrictIds{0}_{1}_{2}_{3}_{4}", count, albumsId, start, isShowVipFirstBuyPackage, districtIds));
        //    LocalCache10Min.Remove(key);
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        /// <param name="albumsId"></param>
        /// <param name="start"></param>
        /// <param name="lat"></param>
        /// <param name="lng"></param>
        /// <param name="geoScopeType"></param>
        /// <returns></returns>
        public static List<TopNPackageItem> GetTop20PackageAddSearch(int count = 25, int albumsId = 1, int start = 0, float lat = 0, float lng = 0, int geoScopeType = 0, int districtID = 0, bool isShowVipFirstBuypackage = true)
        {
            return LocalCache10Min.GetData<List<TopNPackageItem>>(string.Format("HotelAdapter:GetTop20Package_{0}_{1}_{2}_{3}_{4}_{5}_{6}_{7}", count, albumsId, start, lat, lng, geoScopeType, districtID, isShowVipFirstBuypackage), () =>
            {
                List<TopNPackageItem> list = HotelService.GetTopNPackageAddSearchList(true, start, count, albumsId, lat, lng, geoScopeType, districtID, isShowVipFirstBuypackage);
                genTopNPackageItemPicURL(list);
                return list;
            });
        }

        public static List<HotelDestnInfo> GetHotelDestnInfo(int albumsID)
        {
            return LocalCache30Min.GetData<List<HotelDestnInfo>>(string.Format("HotelAdapter:GetHotelDestnInfo_{0}", albumsID), () =>
            {
                List<HotelDestnInfo> list = HotelService.GetHotelDestnInfo(albumsID);
                return list;
            });
            //List<HotelDestnInfo> list = HotelService.GetHotelDestnInfo(albumsID);
            //return list;
        }

        public static List<HotelDestnInfo> GetHotelDestnInfoWithIn(int albumsID, float lat, float lng)
        {
            return LocalCache30Min.GetData<List<HotelDestnInfo>>(string.Format("HotelAdapter:GetHotelDestnInfoWithIn_{0}_{1}_{2}", albumsID, lat, lng), () =>
            {
                List<HotelDestnInfo> list = HotelService.GetHotelDestWithIn(albumsID, lat, lng);
                return list;
            });
            //List<HotelDestnInfo> list = HotelService.GetHotelDestWithIn(albumsID, lat, lng);
            //return list;
        }

        public static PackageAlbumsEntity GetOnePackageAlbums(int albumId)
        {
            return HotelService.GetOnePackageAlbums(albumId);
        }

        public static List<TopNPackagesEntity> GetTopNPackagesEntityList4HotelAlbums(TopNPackageSearchParam param, out int count)
        {
            return HotelService.GetTopNPackagesEntityList4HotelAlbums(param, out count);
        }

        private static void genTopNPackageItemPicURL(IEnumerable<TopNPackageItem> list)
        {
            var hotelIds = list.Select(_ => _.HotelID).Distinct();

            var hotelPicsList = HotelAdapter.GetManyHotelPhotos(hotelIds);

            var finalHasPicHotel = new Dictionary<int, List<HJD.HotelServices.Contracts.HotelPhotoEntity>>();
            foreach (var item in hotelPicsList)
            {
                HJD.HotelServices.Contracts.HotelPhotoEntity photo = item.HPList.FirstOrDefault();
                if (photo != null && photo.HotelID > 0)
                {
                    finalHasPicHotel.Add(photo.HotelID, item.HPList);
                }
            }

            foreach (var temp in list)
            {
                int tempHotelId = temp.HotelID;
                if (!string.IsNullOrWhiteSpace(temp.CoverPicSUrl))
                {
                    temp.PicUrls = new List<string>() { PhotoAdapter.GenHotelPicUrl(temp.CoverPicSUrl, Enums.AppPhotoSize.appdetail1) };
                }
                else if (finalHasPicHotel.ContainsKey(tempHotelId))
                {
                    temp.PicUrls = new List<string>() { PhotoAdapter.GenHotelPicUrl(finalHasPicHotel[tempHotelId][0].SURL, Enums.AppPhotoSize.appdetail1) };
                }
                else
                {
                    temp.PicUrls = new List<string>();
                }
            }
        }

        public static TopNPackageItem GetTopNPackageItem(int pid)
        {
            return GetTopNPackageItem(pid, DateTime.Now.Date);
        }

        public static TopNPackageItem GetTopNPackageItem(int pid, DateTime currentDate)
        {
            List<TopNPackageItem> list = HotelService.GetPackageItemList2(new List<int>() { pid }, currentDate);
            genTopNPackageItemPicURL(list);
            return list != null && list.Count != 0 ? list[0] : new TopNPackageItem();
        }

        public static TopNPackageItem GetTopNPackageItemLocalCache30(int pid, DateTime currentDate)
        {
            string cacheKey = string.Format("GetTopNPackageItemLocalCache30{0}{1}", pid, currentDate);
            return LocalCache30Min.GetData<TopNPackageItem>(cacheKey, () =>
            {
                List<TopNPackageItem> list = HotelService.GetPackageItemList2(new List<int>() { pid }, currentDate);
                genTopNPackageItemPicURL(list);
                return list != null && list.Count != 0 ? list[0] : new TopNPackageItem();
            });
        }

        /// <summary>
        /// 获取套餐内容，不过滤套餐状态
        /// </summary>
        /// <param name="pid"></param>
        /// <param name="currentDate"></param>
        /// <returns></returns>
        public static TopNPackageItem GetTopNPackageItemNoFilterPackageStateLocalCache30(int pid, DateTime? currentDate)
        {
            string cacheKey = string.Format("GetTopNPackageItemNoFilterPackageStateLocalCache30{0}{1}", pid, currentDate);
            return LocalCache30Min.GetData<TopNPackageItem>(cacheKey, () =>
            {
                List<TopNPackageItem> list = HotelService.GetPackageItemList2(new List<int>() { pid }, currentDate, false);
                genTopNPackageItemPicURL(list);
                return list != null && list.Count != 0 ? list[0] : new TopNPackageItem();
            });
        }

        /// <summary>
        /// 获取套餐内容，不过滤套餐状态 无缓存
        /// </summary>
        /// <param name="pid"></param>
        /// <param name="currentDate"></param>
        /// <returns></returns>
        public static TopNPackageItem GetTopNPackageItemNoFilterPackageState(int pid, DateTime? currentDate)
        {
            List<TopNPackageItem> list = HotelService.GetPackageItemList2(new List<int>() { pid }, currentDate, false);
            genTopNPackageItemPicURL(list);
            return list != null && list.Count != 0 ? list[0] : new TopNPackageItem();
        }
        
        public static List<TypeAndPrice> GetPackageTypeAndPriceList(List<int> pids)
        {
            return HotelService.GetPackageTypeAndPriceList(pids);
        }

        public static void cacheSearchHotelInterestHotelId(int hotelID, int interest, int district, int geoStype, float lat, float lon, int cacheHotel)
        {
            string key = string.Format(keyTemplate, hotelID, interest, district, geoStype, Math.Round(lat * 1000, 0), Math.Round(lon * 1000, 0));
            LocalCache.Set(key, "SearchHotelInterest", cacheHotel);
        }

        public static int GetSearchHotelInterestCacheHotelId(int hotelID, int interest, int district, int geoStype, float lat, float lon)
        {
            string key = string.Format(keyTemplate, hotelID, interest, district, geoStype, Math.Round(lat * 1000, 0), Math.Round(lon * 1000, 0));
            return Convert.ToInt32(LocalCache.GetData<object>(key, "SearchHotelInterest", () => { return 0; }));
        }

        /// <summary>
        /// 批量获取酒店基本信息
        /// </summary>
        /// <param name="hotelIDs"></param>
        /// <returns></returns>
        public static IEnumerable<HotelBasicInfo> GetHotelBasicInfoList(IEnumerable<int> hotelIDs)
        {
            return HotelService.GetHotelBasicInfoList(hotelIDs);
        }

        public static PackageEntity GetOnePackageEntity(int pId)
        {
            return HotelService.GetOnePackageEntity(pId);
        }

        public static List<HJD.HotelManagementCenter.Domain.HolidayInfoEntity> GetHolidays()
        {
            return commService.GetHolidays();
        }

        /// <summary>
        /// 获得酒店地图信息
        /// </summary>
        /// <param name="hotelID"></param>
        /// <returns></returns>
        public static HotelMapBasicInfo GetHotelMapInfo(int hotelID)
        {
            return HotelService.GetHotelMapInfo(hotelID);
        }

        /// <summary>
        /// (缓存)根据地区ID或坐标 获取附近300km（默认）的数据  
        /// </summary>
        /// <param name="districtID"></param>
        /// <param name="lat"></param>
        /// <param name="lon"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        public static List<ArounDistrictEntity> CalculateNearDistrictByDistance(int districtID, float lat, float lon, int distance = 300000)
        {
            return LocalCache30Min.GetData<List<ArounDistrictEntity>>(string.Format("NearDistrict_{0}_{1}_{2}", districtID, districtID > 0 ? 0 : Math.Round(lat * 100, 0), districtID, districtID > 0 ? 0 : Math.Round(lon * 100, 0)), () =>
            {
                return CalculateNearDistrictByDistanceNoCache(districtID, lat, lon, distance);
            });
        }

        /// <summary>
        /// (非缓存)根据地区ID或坐标 获取附近300km（默认）的数据  
        /// </summary>
        /// <param name="districtID"></param>
        /// <param name="lat"></param>
        /// <param name="lon"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        public static List<ArounDistrictEntity> CalculateNearDistrictByDistanceNoCache(int districtID, float lat, float lon, int distance = 300000)
        {
            return HotelService.CalculateNearDistrictByDistance(districtID, lat, lon, distance);
        }

        /// <summary>
        /// 【触发更新列表价 hotel list】触发更新指定酒店指定日期的列表价队列任务
        /// </summary>
        /// <param name="result"></param>
        /// <param name="checkIn"></param>
        public static void PublishUpdatePriceSlotTask(SearchHotelListResult result, string checkIn)
        {
            if (result != null && result.Result20 != null && result.Result20.Count > 0)
            {
                try
                {
                    if (!string.IsNullOrEmpty(checkIn))
                    {
                        var hotellist = result.Result20.Select(h => h.Id).ToList();
                        var pub = HotelService.PublishUpdateMultiPriceSlotTask(hotellist, DateTime.Parse(checkIn));
                    }
                }
                catch (Exception ex)
                {

                }
            }
        }

        /// <summary>
        /// 【触发更新列表价 single hotel】触发更新指定酒店指定日期的列表价队列任务
        /// </summary>
        /// <param name="hotelId"></param>
        /// <param name="checkIn"></param>
        public static void PublishUpdatePriceSlotTask(int hotelId, string checkIn)
        {
            if (hotelId > 0 && !string.IsNullOrEmpty(checkIn))
            {
                try
                {
                    var pub = HotelService.PublishUpdatePriceSlotTask(hotelId, DateTime.Parse(checkIn));
                }
                catch (Exception ex)
                {

                }
            }
        }

        #region 微信菜单酒店列表

        public static WapInterestHotelsResult3 SearchInterestHotelWeixin(int districtId, int interestId = 0)
        {
            var model = new WapInterestHotelsResult3();

            DateTime arrivalTime = CommMethods.GetDefaultCheckIn();
            DateTime departureTime = arrivalTime.AddDays(1);

            var hids = "";
            var qResult = new QueryHotelResult3();
            if (districtId == -1 && interestId == 12)
            {
                //读取 华东亲子Top10 的酒店Id列表
                HJD.HotelManagementCenter.Domain.CommDictEntity advPackage = commService.GetCommDict(10008, 4);
                qResult = HotelService.QueryHotelByHids(advPackage.Descript);
            }
            else
            {
                qResult = HotelService.QueryHotelByDistrictInterest(districtId, interestId);
            }

            var hotels = qResult.HotelList;
            if (hotels.Count > 0)
            {
                //GenHotelListPrice(hotels, null,arrivalTime,departureTime);
                GenHotelListSlotPrice(hotels, null, arrivalTime, departureTime, 0, 0);

                interestId = (interestId == 0 ? (int)EnumHelper.InterestID.AllHotelInterest : interestId);

                List<InterestCommentEntity> hicl = CommentAdapter.GetHotelInterestComment(interestId, hotels.Select(h => h.Id).ToList());

                foreach (var hotel in hotels)
                {
                    hotel.InterestComment = "";
                    try
                    {
                        hotel.InterestComment = GenHotelIntro(hotel.Id);
                        if (!string.IsNullOrEmpty(hotel.InterestComment)) hotel.InterestComment = FormatHotelListComment(hotel.InterestComment);
                    }
                    catch (Exception ex)
                    {

                    }

                    hotel.PictureList = hotel.PictureSURLList.Count == 0 ? new List<string>() { PhotoAdapter.GenHotelPicUrl(defaultPicSUrl, Enums.AppPhotoSize.theme) } :
                        hotel.PictureSURLList.Select(s => PhotoAdapter.GenHotelPicUrl(s, Enums.AppPhotoSize.theme)).ToList();
                }
            }

            model.Result = hotels;
            model.InterestID = interestId;
            model.InterestName = ResourceAdapter.GetInterestName(interestId);

            return model;
        }

        public static List<string> GetProvinceListByInterest(int interestId)
        {
            return HotelService.GetProvinceListByInterest(interestId);
        }

        #endregion

        #region 点评、酒店、地区 分享和浏览 搜索记录

        /// <summary>
        /// 插入浏览记录
        /// </summary>
        /// <param name="browsing"></param>
        /// <returns></returns>
        public static int InsertBrowsingRecord(BrowsingRecordEntity browsing)
        {
            return HotelService.InsertBrowsingRecord(browsing);
        }

        /// <summary>
        /// 插入分享记录
        /// </summary>
        /// <param name="share"></param>
        /// <returns></returns>
        public static int InsertShareRecord(ShareRecordEntity share)
        {
            return HotelService.InsertShareRecord(share);
        }

        /// <summary>
        /// 插入搜索记录
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public static int InsertSearchRecord(SearchRecordEntity search)
        {
            return HotelService.InsertSearchRecord(search);
        }

        #endregion

        #region 获取多个酒店ID的最新点评
        public static Dictionary<int, List<HotelRelComment>> GetManyHotelRelComments(IEnumerable<int> hotelIds)
        {
            Dictionary<int, List<HotelRelComment>> dic = new Dictionary<int, List<HotelRelComment>>();
            var userIds = new List<long>();

            if (hotelIds != null && hotelIds.Any())
            {
                var commentAddInfos = commentService.GetCommentAddInfos(new CommentAddInfoParam()
                {
                    hotelIdList = hotelIds,
                    UserID = 0,
                    CatIdList = new List<int>(),
                    CommentIdList = new List<int>()
                });

                userIds = commentAddInfos.Select(_ => _.UserId).Distinct().ToList();

                foreach (var hotelId in hotelIds)
                {
                    var addInfos = commentAddInfos.FindAll(_ => _.HotelId == hotelId);
                    if (addInfos != null && addInfos.Any())
                    {
                        var titleAddInfo = addInfos.FirstOrDefault(_ => _.CategoryID == 9);
                        var advantageInfo = addInfos.FirstOrDefault(_ => _.CategoryID == 1);

                        var content = titleAddInfo == null || string.IsNullOrWhiteSpace(titleAddInfo.Content)
                            ? advantageInfo == null || string.IsNullOrWhiteSpace(advantageInfo.Content) ? "" : advantageInfo.Content
                            : titleAddInfo.Content;

                        long authorId = titleAddInfo != null
                            ? titleAddInfo.UserId
                            : advantageInfo != null ? advantageInfo.UserId : 0;

                        if (authorId != 0)
                        {
                            content = content.Length > 30 ? content.Substring(0, 30) : content;
                            dic.Add(hotelId, new List<HotelRelComment>(){ new HotelRelComment()
                            {
                                Title = content,
                                AuthorUserID = authorId
                            }});
                        }
                    }
                }

                if (dic != null && dic.Keys.Any())
                {
                    var curUserInfos = AccountAdapter.GetUserBasicInfo(userIds);
                    foreach (var hotelRelComments in dic.Values)
                    {
                        var curUserInfo = curUserInfos.FirstOrDefault(j => j.UserId == hotelRelComments[0].AuthorUserID);
                        hotelRelComments[0].Author = curUserInfo == null ? "" : "by " + curUserInfo.NickName;
                    }
                }
            }
            return dic;
        }
        #endregion

        #region 酒店列表 每个酒店匹配标签
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tags"></param>
        /// <param name="hotelIds"></param>
        /// <returns></returns>
        public static Dictionary<int, List<FilterTag>> GenHotelListItemRelFilterTags(List<FilterTag> tags, IEnumerable<int> hotelIds)
        {
            var result = new Dictionary<int, List<FilterTag>>();
            if (hotelIds == null || !hotelIds.Any()) return result;
            int tagsLength = tags != null ? tags.Count : 0;

            var hotelFilterCols = HotelService.GetManyHotelFilterCols(hotelIds);
            foreach (var hotelId in hotelIds)
            {
                var hotelTags = TransFilterCols2FilterTags(hotelFilterCols.FindAll(_ => _.HotelId == hotelId));
                if (hotelTags == null || !hotelTags.Any()) continue;

                var tempTags = (from hotelTag in hotelTags
                                join tag in tags
                                    on new { hotelTag.BlockCategoryID, hotelTag.Value } equals
                                    new { tag.BlockCategoryID, tag.Value }
                                    into datas
                                from subpet in datas.DefaultIfEmpty()
                                select
                                    new FilterTag
                                    {
                                        BlockCategoryID = hotelTag.BlockCategoryID,
                                        Name = hotelTag.Name,
                                        PinyinName = hotelTag.PinyinName,
                                        Value = hotelTag.Value,
                                        IsMatch = subpet != null
                                    }).OrderByDescending(_ => _.IsMatch).ThenBy(_ => _.BlockCategoryID)
                        .Take(tagsLength > 7 ? tagsLength : 7);

                result.Add(hotelId, tempTags.GroupBy(_ => _.Name).Select(_ => _.FirstOrDefault()).ToList());
            }
            return result;
        }

        private static List<FilterTag> TransFilterCols2FilterTags(List<HotelFilterColEntity> hotelFilterCols)
        {
            if (hotelFilterCols == null) return new List<FilterTag>();

            long baseOffset = HJDAPI.Common.Helpers.Struct.HotelListFilterPrefix.BaseOffset;//基本系数

            List<int> zoneids = new List<int>();
            List<int> interestids = new List<int>();
            List<int> classids = new List<int>();
            List<int> facilitys = new List<int>();
            List<int> starids = new List<int>();//酒店星级id集合
            List<int> tripTypeids = new List<int>();//出游类型id集合
            List<int> featuredTrees = new List<int>();

            MapFilterCol2BlockValue(hotelFilterCols, ref classids, ref interestids, ref zoneids, ref facilitys, ref tripTypeids, ref featuredTrees);

            List<FilterDicEntity> filterTagInfos = GetHotelListFilterTagInfos(new SearchHotelListFilterTagInfoParam()
            {
                classids = new List<int>(),
                interestids = interestids,
                zoneids = zoneids,
                facilitys = facilitys,
                triptypeids = tripTypeids,
                featuredtreeids = featuredTrees
            });

            return filterTagInfos.Select(_ =>
                new FilterTag()
                {
                    BlockCategoryID = MapFilterCol2BlockCategory(_.Key),
                    PinyinName = "",
                    Name = _.Name,
                    Value = _.ID.ToString(),
                    IsMatch = false
                }
            ).ToList();
        }

        private static void MapFilterCol2BlockValue(List<HotelFilterColEntity> hotelFilterCols, ref List<int> classids, ref List<int> interestids, ref List<int> zoneids, ref List<int> facilitys, ref List<int> triptypeids, ref List<int> featuredtreeids)
        {
            if (hotelFilterCols == null) return;
            long baseOffSet = HJDAPI.Common.Helpers.Struct.HotelListFilterPrefix.BaseOffset;//基本系数
            long themeKey = HJDAPI.Common.Helpers.Struct.HotelListFilterPrefix.Interest;//主题特色
            long zonePlaceKey = HJDAPI.Common.Helpers.Struct.HotelListFilterPrefix.ZonePlace;//区域ID(位置)
            long classKey = HJDAPI.Common.Helpers.Struct.HotelListFilterPrefix.Class2;//酒店类型
            long hotelFacilityKey = HJDAPI.Common.Helpers.Struct.HotelListFilterPrefix.HotelFacility;//设施服务
            long starKey = HJDAPI.Common.Helpers.Struct.HotelListFilterPrefix.Star;//酒店星级
            long tripTypeKey = HJDAPI.Common.Helpers.Struct.HotelListFilterPrefix.TripType;//出游类型
            long featuredTreeKey = HJDAPI.Common.Helpers.Struct.HotelListFilterPrefix.FeaturedTree;//标签查询

            string themePrefix = (themeKey / baseOffSet).ToString();//前缀
            string zonePlacePrefix = (zonePlaceKey / baseOffSet).ToString();
            string classPrefix = (classKey / baseOffSet).ToString();
            string hotelFacilityPrefix = (hotelFacilityKey / baseOffSet).ToString();
            string starPrefix = (starKey / baseOffSet).ToString();
            string tripTypePrefix = (tripTypeKey / baseOffSet).ToString();
            string featuredTreeKeyPrefix = (featuredTreeKey / baseOffSet).ToString();

            classids =
                hotelFilterCols.FindAll(_ => _.FilterCol.ToString().StartsWith(classPrefix))
                    .Select(_ => (int)(_.FilterCol - classKey)).ToList();

            interestids =
                hotelFilterCols.FindAll(_ => _.FilterCol.ToString().StartsWith(themePrefix))
                    .Select(_ => (int)(_.FilterCol - themeKey)).ToList();

            zoneids =
                hotelFilterCols.FindAll(_ => _.FilterCol.ToString().StartsWith(zonePlacePrefix))
                    .Select(_ => (int)(_.FilterCol - zonePlaceKey)).ToList();

            facilitys =
                hotelFilterCols.FindAll(_ => _.FilterCol.ToString().StartsWith(hotelFacilityPrefix))
                    .Select(_ => (int)(_.FilterCol - hotelFacilityKey)).ToList();

            triptypeids =
                hotelFilterCols.FindAll(_ => _.FilterCol.ToString().StartsWith(tripTypePrefix))
                    .Select(_ => (int)(_.FilterCol - tripTypeKey)).ToList();

            featuredtreeids =
                hotelFilterCols.FindAll(_ => _.FilterCol.ToString().StartsWith(featuredTreeKeyPrefix))
                    .Select(_ => (int)(_.FilterCol - featuredTreeKey)).ToList();
        }

        private static int MapFilterCol2BlockCategory(long filterCol)
        {
            long baseOffset = HJDAPI.Common.Helpers.Struct.HotelListFilterPrefix.BaseOffset;//基本系数
            int key = (int)(filterCol / baseOffset);

            return MapTagBlockCategoryId(key);
        }
        #endregion

        #region 热门标签

        /// <summary>
        /// 获取某个地区热门搜索Tag
        /// </summary>
        /// <param name="districtId"></param>
        /// <returns></returns>
        public static List<DistrictHotFilterTagEntity> GetDistrictHotFilterTagList(int districtId)
        {
            return HotelService.GetDistrictHotFilterTagList(districtId);
        }

        /// <summary>
        /// 获取某个地区热门搜索Tag
        /// </summary>
        /// <param name="districtId"></param>
        /// <returns></returns>
        public static int UpsertDistrictHotFilterTagList(IEnumerable<DistrictHotFilterTagEntity> hotTags)
        {
            return HotelService.UpsertDistrictHotFilterTagList(hotTags);
        }

        #endregion

        #region Map tagBlockCategory CategoryID

        public static int MapTagBlockCategoryId(int categoryId)
        {
            switch (categoryId)
            {
                case 18:
                    return 1;
                case 16:
                    return 2;
                case 22:
                    return 8;
                case 21:
                    return 3;
                case 19:
                    return 4;
                case 10:
                    return 7;
                case 23:
                    return 9;
                default:
                    return 0;
            }
        }

        public static int MapCategoryId(int blockTagCategoryId)
        {
            switch (blockTagCategoryId)
            {
                case 1:
                    return 18;
                case 2:
                    return 16;
                case 8:
                    return 22;
                case 3:
                    return 21;
                case 4:
                    return 19;
                case 7:
                    return 10;
                case 9:
                    return 23;
                default:
                    return 0;
            }
        }
        #endregion

        #region 查找标签优先级
        public static List<FilterTag> GenHotFilterTagsByFilterBlockCategoryId(int catId, List<FilterTag> tags)
        {
            if (catId == 9)
            {
                return new List<FilterTag>();
            }
            else if (catId == 2)
            {
                return tags.OrderByDescending(j => j.HotelCount).ToList();
            }
            else if (catId == 4)
            {
                var filterTagsWithOrder = GenHotFacilitysFilterTag();
                return (from tag1 in tags
                        join tag2 in filterTagsWithOrder
                            on new { tag1.BlockCategoryID, tag1.Name } equals
                            new { tag2.BlockCategoryID, tag2.Name }
                            into datas
                        from subpet in datas.DefaultIfEmpty()
                        select
                            new FilterTag
                            {
                                BlockCategoryID = tag1.BlockCategoryID,
                                Name = tag1.Name,
                                PinyinName = tag1.PinyinName,
                                Value = tag1.Value,
                                IsMatch = false,
                                HotelCount = subpet != null ? subpet.HotelCount : tag1.HotelCount
                            }).OrderByDescending(_ => _.HotelCount).Take(20).ToList();
            }
            else if (catId == 3)
            {
                return tags.Select(_ => new FilterTag()
                {
                    BlockCategoryID = _.BlockCategoryID,
                    HotelCount = _.Value.Equals("5") ? 10000 : _.HotelCount,
                    IsMatch = _.IsMatch,
                    Name = _.Name,
                    PinyinName = _.PinyinName,
                    Value = _.Value
                }).OrderByDescending(_ => _.HotelCount).ToList();//酒店类型 酒店放在第一顺位
            }
            else
            {
                return tags.Take(7).ToList();
            }
        }

        internal static List<FilterTag> GenHotFacilitysFilterTag()
        {
            var result = new List<FilterTag>();
            var hotFacilityStr = Configs.HotFacilitys;
            if (string.IsNullOrWhiteSpace(hotFacilityStr)) return result;

            var hotFacilityArray = hotFacilityStr.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            for (var i = 0; i < hotFacilityArray.Length; i++)
            {
                result.Add(new FilterTag()
                {
                    BlockCategoryID = 4,
                    HotelCount = 1000000 - i,
                    Name = hotFacilityArray[i]
                });
            }
            return result;
        }
        #endregion

        #region interestEntity
        /// <summary>
        /// 获取主题
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        internal static InterestEntity GetOneInterestEntity(int Id)
        {
            return HotelService.GetOneInterestEntity(Id);
        }
        #endregion

        #region 获取单个酒店的所有FilterCol及相关名称

        /// <summary>
        /// 获得酒店详情页展示的标签(房型除外)
        /// </summary>
        /// <param name="hotelId"></param>
        /// <returns></returns>
        public static List<HotelFilterColEntity> GetHotelDisplayFilterTagList(int hotelId)
        {
            return HotelService.GetHotelDisplayFilterTagList(hotelId);
        }

        /// <summary>
        /// 出所有不重复的房型
        /// 每个房型下可能有标签 可能没有
        /// </summary>
        /// <param name="hotelId"></param>
        /// <returns></returns>
        public static List<HotelRoomTypeFilterTagEntity> GetHotelRoomTypeFilterTagList(int hotelId)
        {
            return HotelService.GetHotelRoomTypeFilterTagList(hotelId);
        }

        /// <summary>
        /// 获取酒店不重复房型
        /// </summary>
        /// <param name="hotelId"></param>
        /// <returns></returns>
        public static List<PRoomInfoEntity> GetProomInfoList(int hotelId)
        {
            return HotelService.GetProomInfoList(hotelId);
        }

        public static List<FeaturedCommentEntity> TransHotelFilterColEntity2FeaturedCommentEntity(IEnumerable<HotelFilterColEntity> list)
        {
            var toList = new List<FeaturedCommentEntity>();
            if (list != null && list.Count() != 0)
            {
                foreach (var item in list)
                {
                    toList.Add(new FeaturedCommentEntity()
                    {
                        CategoryID = item.CategoryID,
                        Comment = "",
                        CommentCount = item.CommentCount,
                        FeaturedID = item.ID,
                        FeaturedName = item.Name,
                        HotelID = item.HotelId,
                        IsRecommend = false,
                        PicUrl = new List<string>(),
                        RelInterestID = 0,
                        TagType = 0
                    });
                }

                //由于设施在23和19开头的filtercol有重复 这里需要单独去重
                toList = toList.GroupBy(_ => new { _.CategoryID, _.FeaturedID }).Select(_ => _.First()).ToList();
            }
            return toList;
        }

        public static List<FeaturedCommentEntity> GenHotelDetailDisplayList(int hotelId, string appVer = null)
        {
            var featurCommentList = new List<FeaturedCommentEntity>();
            if (string.IsNullOrWhiteSpace(appVer) || appVer.CompareTo("4.4") < 0)
            {
                featurCommentList = HotelService.GetHotelFeaturedCommentInfo(hotelId);
            }
            else
            {
                var displayFilterList = GetHotelDisplayFilterTagList(hotelId);
                featurCommentList = TransHotelFilterColEntity2FeaturedCommentEntity(displayFilterList);
            }
            return featurCommentList;
        }
        #endregion

        public static List<int> GenHotelShowFeatureDetailList(int hotelId)
        {
            var featurCommentList = HotelService.GetHotelFeaturedInfo(hotelId);
            return featurCommentList;
        }

        #region 获取单个酒店房间相关的点评和数据

        public static List<FeaturedCommentEntity> TransHotelRoomTypeFilterTag2FeatureComments(IEnumerable<HotelRoomTypeFilterTagEntity> list)
        {
            var toList = new List<FeaturedCommentEntity>();
            if (list != null && list.Count() != 0)
            {
                foreach (var item in list)
                {
                    toList.Add(new FeaturedCommentEntity()
                    {
                        CategoryID = HJDAPI.Common.Helpers.Struct.HotelDetailDisplayFilterPrefix.RoomType + item.RoomID,
                        Comment = "",
                        CommentCount = item.CommentCount,
                        FeaturedID = item.ID,
                        FeaturedName = item.Name,
                        HotelID = item.HotelId,
                        IsRecommend = false,
                        PicUrl = new List<string>(),
                        RelInterestID = 0,
                        TagType = 0
                    });
                }
            }
            return toList;
        }

        #endregion

        #region 酒店相关照片的数据
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hotelId"></param>
        /// <param name="start"></param>
        /// <param name="count"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public static List<HotelPicInfo> GetHotelPicFromCustomer(int hotelId, int start, int count, out int totalCount)
        {
            var hotelCommentPics = commentService.GetHotelCommentPhotoList(hotelId, start, count, out totalCount);
            return hotelCommentPics.Select(_ => new HotelPicInfo()
            {
                author = _.NickName,
                date = _.WriteDate.ToString("yyyy/MM/dd"),
                picSmallUrl = PhotoAdapter.GenHotelPicUrl(_.PicSUrl, Enums.AppPhotoSize.small),
                picUrl = PhotoAdapter.GenHotelPicUrl(_.PicSUrl, Enums.AppPhotoSize.theme),
                roomComment = _.Brief,
                roomType = ""
            }).ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hotelId"></param>
        /// <param name="start"></param>
        /// <param name="count"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public static List<HotelPicInfo> GetHotelPicFromOfficial(int hotelId, int start, int count, out int totalCount)
        {
            HotelPhotosEntity hps = GetHotelPhotos(hotelId, 0);
            totalCount = hps.HPList.Count;
            return start < totalCount ? hps.HPList.Skip(start).Take(count).Select(_ => new HotelPicInfo()
            {
                author = "",
                date = "",
                picSmallUrl = PhotoAdapter.GenHotelPicUrl(_.SURL, Enums.AppPhotoSize.small),
                picUrl = PhotoAdapter.GenHotelPicUrl(_.SURL, Enums.AppPhotoSize.jupiter),
                roomComment = "",
                roomType = !string.IsNullOrWhiteSpace(_.Title) ? _.Title.Length >= 10 ? "" : _.Title : ""
            }).ToList() : new List<HotelPicInfo>();
        }
        #endregion

        #region 铂涛优惠套餐

        public static List<ThirdPartyProductItem> GetThirdPartyProductItemList4Botao(DateTime startTime)
        {
            var curNow = DateTime.Now;
            if (startTime > new DateTime(1990, 1, 1, 0, 0, 0))
            {
                startTime = startTime.AddDays(-1);
            }
            else
            {
                startTime = curNow;
            }

            var hotelRateList = GetCheapHotelPackage4Botao(startTime.Date);

            var result = new List<ThirdPartyProductItem>();
            if (hotelRateList != null && hotelRateList.Count != 0)
            {
                foreach (var hotelRate in hotelRateList.GroupBy(_ => _.HotelId))
                {
                    var package = hotelRate.OrderBy(_ => _.CheapPrice).First();
                    var hotelPhoto = HotelService.GetHotelPhotos(package.HotelId);
                    var firstSUrl = hotelPhoto.HPList != null && hotelPhoto.HPList.Count != 0 ? hotelPhoto.HPList.First().SURL : "defaultPicSUrl";
                    result.Add(GenThirdPartyProductItem(string.Format("http://p1.zmjiudian.com/{0}_theme", firstSUrl), string.Format("{0}/hotel/{1}?pid={2}", Configs.WWWURL, package.HotelId, package.PId), package.HotelName, package.PName, package.DistrictName, package.DistrictName, 100, 100, package.EndDate, (int)package.MarketPrice, (int)package.CheapPrice, ThirdPartyProductType.hotel, package.PBrief, package.PBrief, string.Format("hotel_{0}", package.HotelId), DateTime.Now, package.EndDate <= curNow ? true : false));
                }
            }
            return result;
        }

        public static List<CanSellCheapHotelPackageEntity> GetCheapHotelPackage4Botao(DateTime startTime)
        {
            //var keyName = string.Format("BoTaoHotelPackage_{0}", startTime.Date.ToString("yy-MM-dd"));
            //return LocalCache30Min.GetData<List<CanSellCheapHotelPackageEntity>>(keyName, () =>
            //{
            //var result = new List<CanSellCheapHotelPackageEntity>();
            //var allPRateList = HotelService.GetCheapHotelPackage4Botao(startTime.Date);
            //if(allPRateList != null && allPRateList.Count != 0){
            //    foreach (var item in allPRateList.GroupBy(_ => new { _.HotelId, _.PId }))
            //    {
            //        var pricePackage = item.OrderBy(_ => _.CheapPrice).First();
            //        result.Add(pricePackage);
            //    }
            //}
            //return result;
            //});
            return HotelService.GetCheapHotelPackage4Botao(startTime.Date);
        }

        public static ThirdPartyProductItem GenThirdPartyProductItem(string picUrl, string detailUrl, string hotelName, string packageName, string cityName, string address, int availableCount, int totalCount, DateTime expirationDate, int marketPrice, int discountPrice, ThirdPartyProductType productType, string packageBrief, string productDetail, string productcode, DateTime lastEditTime, bool IsDel)
        {
            return new ThirdPartyProductItem()
            {
                Address = address,
                AvailableCount = availableCount,
                TotalCount = totalCount,
                CityName = cityName,
                PicUrl = picUrl,
                DetailUrl = detailUrl,
                HotelName = hotelName,
                PackageName = packageName,
                PackageBrief = packageBrief,
                ExpirationDate = expirationDate,
                MarketPrice = marketPrice,
                DiscountPrice = discountPrice,
                ProdcutDetail = productDetail,
                ProductType = productType,
                ProductCode = productcode,
                LastEditTime = lastEditTime.ToString("yyyy-MM-dd hh:mm:ss"),
                IsDel = IsDel
            };
        }

        #endregion

        #region 铂韬结算订单数据

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static int InsertBotaoSettlementRecord(BotaoSettlementEntity entity)
        {
            return HotelService.InsertBotaoSettlementRecord(entity);
        }

        /// <summary>
        /// 铂涛待结算订单列表
        /// </summary>
        /// <returns></returns>
        public static List<BotaoSettleOrderEntity> GetBotaoSettleOrderList()
        {
            return HotelService.GetBotaoSettleOrderList();
        }

        #endregion

        #region 套餐着陆页结果
        public static RecommendPackageDetailResult GetPackageDetailResult(int pid, long userId, DateTime startDate, DateTime endDate)
        {
            var result = new RecommendPackageDetailResult();

            try
            {
                TimeLog log = new TimeLog(string.Format("GetPackageDetailResult pid：{0} userId:{1} startDate:{2} endDate:{3}", pid, userId, startDate, endDate)
             , 1000, null);

                if (startDate == null || startDate < DateTime.Now.Date)
                {
                    startDate = DateTime.Now.Date;
                }
                var packageItem = HotelAdapter.GetTopNPackageItem(pid, startDate);
                log.AddLog("GetTopNPackageItem");
                var dailyPrice = packageItem.PackagePrice.First(j => j.Type == 0).Price;//非vip最低价 正常售价
                var vipPrice = packageItem.PackagePrice.First(j => j.Type == -1).Price;//最低价 是vip价格

                var regex = new Regex("_.*$", RegexOptions.Compiled | RegexOptions.RightToLeft);
                var picUrl = packageItem.PicUrls.Any() ? regex.Replace(packageItem.PicUrls[0], "_theme") : "";

                var customerType = AccountAdapter.GetCustomerType(userId);

                var hotelId = packageItem.HotelID;

                var notVIPPrice = 0;

                notVIPPrice = dailyPrice;

                //var hp = new HotelPrice2();
                //var packagePrice = new List<PackageInfoEntity>() { new PackageAdapter().GetHotelPackageByCode(hotelId, "", pid) };

                //PriceAdapter.SetDayLimit(hp, packagePrice);
                var list = PackageAdapter.GetHotelPackageCalendar30(hotelId, startDate, pid, userId);
                log.AddLog("GetHotelPackageCalendar30");

                //获取当前套餐日历中最近一个可售日价格 
                PDayItem firstSell = new PDayItem() { Day = startDate };
                if (list.Exists(_ => _.SellState == 1))
                {
                    firstSell = list.Where(_ => _.SellState == 1).OrderBy(_ => _.Day).FirstOrDefault();
                }
                if (startDate == null || endDate < startDate)
                {
                    endDate = startDate.AddDays(1);
                }
                int nightcount = (endDate - startDate).Days;

                result.packageItem = new RecommendHotelItem()
                {
                    HotelID = hotelId,
                    HotelName = packageItem.HotelName.Replace("\r", " "),
                    HotelPicUrl = picUrl,
                    HotelPrice = dailyPrice,
                    ADDescription = string.IsNullOrWhiteSpace(packageItem.Title) ? "" : packageItem.Title,
                    MarketPrice = packageItem.MarketPrice,
                    PackageBrief = packageItem.PackageBrief,
                    PID = packageItem.PackageID,
                    RecommendPicUrl = string.IsNullOrWhiteSpace(packageItem.RecomendPicShortNames) ? "" : PhotoAdapter.GenHotelPicUrl(packageItem.RecomendPicShortNames, Enums.AppPhotoSize.theme),
                    RecommendPicUrl2 = string.IsNullOrWhiteSpace(packageItem.RecomendPicShortNames2) ? "" : PhotoAdapter.GenHotelPicUrl(packageItem.RecomendPicShortNames2, Enums.AppPhotoSize.theme),
                    RecomemndWord = packageItem.RecomemndWord,
                    RecomemndWord2 = packageItem.RecomemndWord2,
                    VIPPrice = vipPrice,
                    NotVIPPrice = notVIPPrice,
                    CustomerType = (int)AccountAdapter.TransforCustomerTypeForShowVIPPrice(customerType),
                    PackageName = packageItem.PackageName,
                    IsSellOut = packageItem.IsSellOut,
                    SellDate = firstSell.Day,
                    SellNotVIPPrice = firstSell.NormalPrice,
                    SellVIPPrice = firstSell.VipPrice,
                    ForVIPFirstBuy = packageItem.ForVIPFirstBuy,
                    Intro = new CommentTextAndUrl_ex(),
                    NightCount = nightcount,
                    IsDistributable = packageItem.IsDistributable,
                    PackageType = packageItem.PackageType,
                    DateSelectType = packageItem.DateSelectType,
                    ShareDescription = packageItem.ShareDescription,
                    ShareTitle = packageItem.ShareTitle
                };
                log.AddLog(" new RecommendHotelItem");


                //Log.WriteLog("endDate "+endDate+"  startDate"+startDate);
                //var pinfoEntity = new PackageAdapter().GetHotelPackageByCode(hotelId, "", pid);
                //var pinfoEntity = PackageAdapter.GetHotelPackages(hotelId, pid);
                var pinfoEntity = new PackageAdapter().GetHotelPackages(hotelId, startDate, endDate, pid: pid);
                log.AddLog("GetHotelPackageByCode");
                if (pinfoEntity != null)
                {
                    var packagePrice = new List<PackageInfoEntity>();
                    //packagePrice.Add(pinfoEntity);
                    packagePrice.AddRange(pinfoEntity);

                    PriceAdapter.AddMorePackageInfo(packagePrice);

                    //result.packageItem.AutoCommission = packagePrice[0].AutoCommission;
                    //result.packageItem.ManualCommission = packagePrice[0].ManualCommission;

                    result.packageItem.packageContent = packagePrice[0].Items.FindAll(_ => _.Type == 1).Select(_ => _.Description).ToList();
                    result.packageItem.packageNotice = packagePrice[0].Items.FindAll(_ => _.Type == 2).Select(_ => _.Description).ToList();
                }

                try
                {
                    RetailHotelInfoEntity retailresult = HotelAdapter.GetRetailPackageList(hotelId);
                    RetailPackageEntity package = retailresult.RetailPackageList.Where(_ => _.PID == pid).FirstOrDefault();
                    if (package != null)
                    {
                        //RetailPackageEntity package = retailresult.RetailPackageList.Where(_ => _.PID == pid).First();
                        result.packageItem.AutoCommission = package.AutoCommission;
                        result.packageItem.ManualCommission = package.ManualCommission;
                        if (package.NightCount > 1)
                        {
                            int _totalPrice = 0;
                            int _totalVipPrice = 0;
                            decimal _totalMamuCommission = 0m;
                            decimal _totalAutoCommission = 0m;
                            DateTime startTime = package.CheckIn;
                            PriceAdapter.GetManyDaysPackagePriceCached(retailresult.HotelId, package.PID, Enums.CustomerType.vip, package.NightCount, out _totalPrice, out _totalVipPrice, out startTime, out _totalMamuCommission, out _totalAutoCommission);

                            if (_totalAutoCommission == 0m) { result.packageItem.AutoCommission = package.AutoCommission * package.NightCount; }
                            else { result.packageItem.AutoCommission = _totalAutoCommission; }
                            if (_totalMamuCommission == 0m) { result.packageItem.ManualCommission = package.ManualCommission * package.NightCount; }
                            else { result.packageItem.ManualCommission = _totalMamuCommission; }
                        }
                    }

                }
                catch (Exception e)
                {
                    Log.WriteLog("retailresult" + e);
                }


                result.serialPackageList = HotelService.GetSameSerialPackageEntityList(pid).Select(_ => new SameSerialPackageItem()
                {
                    pId = _.pId,
                    roomTypeId = _.roomTypeId,
                    serialNo = _.serialNo,
                    roomTypeName = _.roomTypeName,
                    roomDesc = _.roomDesc
                }).ToList();
                log.AddLog("GetSameSerialPackageEntityList");

                try
                {
                    List<SameSerialPackageEntity> serialList = HotelService.GetSerialPackageItemListByPid(pid, startDate, hotelId);
                    //IEnumerable<>
                    IEnumerable<IGrouping<string, SameSerialPackageEntity>> groupList = serialList.GroupBy(_ => _.serialNo);
                    foreach (var groupItem in groupList)
                    {
                        GroupSerialItem groupSerialItem = new GroupSerialItem();
                        groupSerialItem.SerialNo = groupItem.Key;
                        foreach (var item in groupItem)
                        {
                            SameSerialPackageItem sameItem = new SameSerialPackageItem();
                            sameItem.pId = item.pId;
                            sameItem.roomDesc = item.roomDesc;
                            sameItem.roomTypeId = item.roomTypeId;
                            sameItem.roomTypeName = item.roomTypeName;
                            sameItem.NoVipPrice = item.NoVIPPrice;
                            sameItem.VipPrice = item.VIPPrice;
                            groupSerialItem.SerialPackageList.Add(sameItem);
                        }

                        result.GroupSerialList.Add(groupSerialItem);
                    }
                    log.AddLog("GetSerialPackageItemListByPid");
                }
                catch (Exception ex) {
                    Log.WriteLog("获取套餐分组报错" + ex);
                }
                

                result.HotelMapUrl = string.Format("http://www.zmjiudian.com/hotel/{0}/map", hotelId);

                HotelItem hi = ResourceAdapter.GetHotel(hotelId, userId);
                result.HotelTel = hi.Telephone;
                result.GLat = hi.GLat;
                result.GLon = hi.GLon;

                result.AddressInfo = new ArrivalDepartureAndAddressInfo { Address = hi.Address };
                List<Hotel3Entity> h3s = HotelService.GetHotel3(hotelId);
                foreach (Hotel3Entity h3 in h3s)
                {
                    switch (h3.Type)
                    {
                        case 1://推荐理由
                            HotelInfo his = new HotelInfo();
                            his = GenHotel3Info(h3);
                            if (!string.IsNullOrEmpty(his.Items[0].content))
                            {
                                result.packageItem.Intro.Description = his.Items[0].content;
                                foreach (var item in his.Items[0].content.Split("\r\n".ToCharArray()))
                                {
                                    result.packageItem.Intro.Item.Add(item);
                                }
                            }
                            break;
                        case 6:
                            result.AddressInfo.ArrivalAndDeparture = GenHotel3Info(h3);
                            break;
                            #region 微信图文 取hotelcontact中的ActivePageId
                            //case 9: //微信分享全文链接
                            //case 10: //微信链接为空时 取本地全文链接
                            //    Hotel3Entity h9 = h3s.Where(_ => _.Type == 9).FirstOrDefault();
                            //    if (h9 != null)
                            //    {
                            //        HotelInfo hi9 = GenHotel3Info(h9);

                            //        if (hi9.Items.Count > 0)
                            //        {
                            //            string strUrl9 = hi9.Items[0].content.Contains("#") == true ? (hi9.Items[0].content.Insert(hi9.Items[0].content.LastIndexOf("#"), "&_isshare=1")) : (hi9.Items[0].content + "&_isshare=1");
                            //            result.packageItem.Intro.ActionUrl = hi9.Items.Count == 0 ? "" : strUrl9;
                            //            result.packageItem.Intro.Text = "全文";
                            //        }
                            //        else
                            //        {
                            //            HotelInfo h = new HotelInfo();
                            //            h = GenHotel3Info(h3);
                            //            result.packageItem.Intro.ActionUrl = h.Items.Count == 0 ? "" : ("http://www.zmjiudian.com/active/activepage?midx=" + h.Items[0].content + "&_isshare=1");
                            //            result.packageItem.Intro.Text = "全文";
                            //        }
                            //    }
                            //    else
                            //    {
                            //        HotelInfo h = new HotelInfo();
                            //        h = GenHotel3Info(h3);
                            //        result.packageItem.Intro.ActionUrl = h.Items.Count == 0 ? "" : ("http://www.zmjiudian.com/active/activepage?midx=" + h.Items[0].content + "&_isshare=1");
                            //        result.packageItem.Intro.Text = "全文";
                            //    }
                            //    break; 
                            #endregion

                    }
                }

                try
                {
                    //获取微信图文
                    HJD.HotelServices.Contracts.HotelContacts hc = HotelService.GetHotelContacts(hotelId);
                    if (hc != null & hc.ActivePageId > 0)
                    {
                        result.packageItem.Intro.ActionUrl = "http://www.zmjiudian.com/active/activepage?pageid=" + hc.ActivePageId + "&_isshare=1";
                        result.packageItem.Intro.Text = "全文";
                    }
                    result.CouponInfo = CouponAdapter.GetCanCouponDefineByBizID(pid, 2, userId);
                }
                catch (Exception e)
                {
                    Log.WriteLog("套餐关联红包报错 ERROR：" + e);
                }



                log.Finish();
            }
            catch (Exception err)
            {
                Log.WriteLog(string.Format("GetPackageDetailResult pid：{0} userId:{1} startDate:{2} endDate:{3}", pid, userId, startDate, endDate) + err.Message + err.StackTrace);
            }

            SetPackageActivityInfo(result);

            return result;
        }


        public static RecommendPackageDetailResult GetPakcageDetailInfo(int pid, long userId, DateTime startDate, DateTime endDate)
        {
            var result = new RecommendPackageDetailResult();

            try
            {
                TimeLog log = new TimeLog(string.Format("GetPakcageDetailInfo pid：{0} userId:{1} startDate:{2} endDate:{3}", pid, userId, startDate, endDate)
             , 1000, null);

                if (startDate == null || startDate < DateTime.Now.Date)
                {
                    startDate = DateTime.Now.Date;
                }
                
                //var packageItem = HotelAdapter.GetTopNPackageItemNoFilterPackageStateLocalCache30(pid, null);//HotelAdapter.GetTopNPackageItemLocalCache30(pid, startDate);
                var packageItem = HotelAdapter.GetTopNPackageItemNoFilterPackageState(pid, null);
                log.AddLog("GetTopNPackageItem");
                var dailyPrice = 0;
                var vipPrice = 0;
                DateTime checkin = DateTime.Now;
                if (packageItem.PackageState == 1)
                {
                    dailyPrice = packageItem.PackagePrice.First(j => j.Type == 0).Price;//非vip最低价 正常售价
                    vipPrice = packageItem.PackagePrice.First(j => j.Type == -1).Price;//最低价 是vip价格

                }


                var regex = new Regex("_.*$", RegexOptions.Compiled | RegexOptions.RightToLeft);
                var picUrl = packageItem.PicUrls.Any() ? regex.Replace(packageItem.PicUrls[0], "_theme") : "";

                var customerType = AccountAdapter.GetCustomerType(userId);

                var hotelId = packageItem.HotelID;

                var notVIPPrice = 0;

                notVIPPrice = dailyPrice;

                var pinfoEntity = new PackageAdapter().GetHotelPackagesInfo(hotelId, startDate, endDate, pid: pid);
                //DailyItems
                log.AddLog("GetHotelPackageByCode");

                if (startDate == null || endDate < startDate)
                {
                    endDate = startDate.AddDays(1);
                }
                int nightcount = (endDate - startDate).Days;

                result.packageItem = new RecommendHotelItem()
                {
                    HotelID = hotelId,
                    HotelName = packageItem.HotelName.Replace("\r"," "),
                    HotelPicUrl = picUrl,
                    HotelPrice = dailyPrice,
                    ADDescription = string.IsNullOrWhiteSpace(packageItem.Title) ? "" : packageItem.Title,
                    MarketPrice = packageItem.MarketPrice,
                    PackageBrief = packageItem.PackageBrief,
                    PID = packageItem.PackageID,
                    RecommendPicUrl = string.IsNullOrWhiteSpace(packageItem.RecomendPicShortNames) ? "" : PhotoAdapter.GenHotelPicUrl(packageItem.RecomendPicShortNames, Enums.AppPhotoSize.theme),
                    RecommendPicUrl2 = string.IsNullOrWhiteSpace(packageItem.RecomendPicShortNames2) ? "" : PhotoAdapter.GenHotelPicUrl(packageItem.RecomendPicShortNames2, Enums.AppPhotoSize.theme),
                    RecomemndWord = packageItem.RecomemndWord,
                    RecomemndWord2 = packageItem.RecomemndWord2,
                    VIPPrice = vipPrice,
                    NotVIPPrice = notVIPPrice,
                    CustomerType = (int)AccountAdapter.TransforCustomerTypeForShowVIPPrice(customerType),
                    PackageName = packageItem.PackageName,
                    IsSellOut = packageItem.IsSellOut,
                    //SellDate = firstSell.Day,
                    //SellNotVIPPrice = firstSell.NormalPrice,
                    //SellVIPPrice = firstSell.VipPrice,
                    ForVIPFirstBuy = packageItem.ForVIPFirstBuy,
                    Intro = new CommentTextAndUrl_ex(),
                    NightCount = nightcount,
                    IsDistributable = packageItem.IsDistributable,
                    PackageType = packageItem.PackageType,
                    DateSelectType = packageItem.DateSelectType,
                    ShareDescription = packageItem.ShareDescription,
                    ShareTitle = packageItem.ShareTitle,
                    NeedVIPGuide =packageItem.NeedVIPGuide,
                    PackageState = packageItem.PackageState
                };

                if (pinfoEntity != null)
                {
                    var packagePrice = new List<PackageInfoEntity>();
                    //packagePrice.Add(pinfoEntity);
                    packagePrice.AddRange(pinfoEntity);

                    PriceAdapter.AddMorePackageInfo(packagePrice);


                    result.packageItem.packageContent = packagePrice[0].Items.FindAll(_ => _.Type == 1).Select(_ => _.Description).ToList();
                    result.packageItem.packageNotice = packagePrice[0].Items.FindAll(_ => _.Type == 2).Select(_ => _.Description).ToList();
                }

                try
                {
                    RetailHotelInfoEntity retailresult = HotelAdapter.GetRetailPackageList(hotelId);

                    RetailPackageEntity package = retailresult.RetailPackageList.Where(_ => _.PID == pid).FirstOrDefault();
                    if (package != null)
                    {
                        result.packageItem.AutoCommission = package.AutoCommission;
                        result.packageItem.ManualCommission = package.ManualCommission;
                        if (package.NightCount > 1)
                        {
                            int _totalPrice = 0;
                            int _totalVipPrice = 0;
                            decimal _totalMamuCommission = 0m;
                            decimal _totalAutoCommission = 0m;
                            DateTime startTime = package.CheckIn;
                            PriceAdapter.GetManyDaysPackagePriceCached(retailresult.HotelId, package.PID, Enums.CustomerType.vip, package.NightCount, out _totalPrice, out _totalVipPrice, out startTime, out _totalMamuCommission, out _totalAutoCommission);

                            if (_totalAutoCommission == 0m) { result.packageItem.AutoCommission = package.AutoCommission * package.NightCount; }
                            else { result.packageItem.AutoCommission = _totalAutoCommission; }
                            if (_totalMamuCommission == 0m) { result.packageItem.ManualCommission = package.ManualCommission * package.NightCount; }
                            else { result.packageItem.ManualCommission = _totalMamuCommission; }
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.WriteLog("GetPakcageDetailInfo  retailresult  error" + e);
                }

                result.serialPackageList = HotelService.GetSameSerialPackageEntityList(pid).Select(_ => new SameSerialPackageItem()
                {
                    pId = _.pId,
                    roomTypeId = _.roomTypeId,
                    serialNo = _.serialNo,
                    roomTypeName = _.roomTypeName,
                    roomDesc = _.roomDesc
                }).ToList();
                log.AddLog("GetSameSerialPackageEntityList");

                try
                {
                    List<SameSerialPackageEntity> serialList = HotelService.GetSerialPackageItemListByPid(pid, startDate, hotelId);
                    //IEnumerable<>
                    IEnumerable<IGrouping<string, SameSerialPackageEntity>> groupList = serialList.GroupBy(_ => _.serialNo);
                    foreach (var groupItem in groupList)
                    {
                        GroupSerialItem groupSerialItem = new GroupSerialItem();
                        groupSerialItem.SerialNo = groupItem.Key;
                        foreach (var item in groupItem)
                        {
                            SameSerialPackageItem sameItem = new SameSerialPackageItem();
                            sameItem.pId = item.pId;
                            sameItem.roomDesc = item.roomDesc;
                            sameItem.roomTypeId = item.roomTypeId;
                            sameItem.roomTypeName = item.roomTypeName;
                            sameItem.NoVipPrice = item.NoVIPPrice;
                            sameItem.VipPrice = item.VIPPrice;
                            groupSerialItem.SerialPackageList.Add(sameItem);
                        }

                        result.GroupSerialList.Add(groupSerialItem);
                    }
                    result.GroupSerialList.OrderBy(_ => _.SerialNo);
                    log.AddLog("GetSerialPackageItemListByPid");
                }
                catch (Exception ex)
                {
                    Log.WriteLog("获取套餐分组报错" + ex);
                }


                result.HotelMapUrl = string.Format("http://www.zmjiudian.com/hotel/{0}/map", hotelId);

                HotelItem hi = ResourceAdapter.GetHotel(hotelId, userId);
                result.HotelTel = hi.Telephone;
                result.GLat = hi.GLat;
                result.GLon = hi.GLon;

                result.AddressInfo = new ArrivalDepartureAndAddressInfo { Address = hi.Address };
                List<Hotel3Entity> h3s = HotelService.GetHotel3(hotelId);
                foreach (Hotel3Entity h3 in h3s)
                {
                    switch (h3.Type)
                    {
                        case 1://推荐理由
                            HotelInfo his = new HotelInfo();
                            his = GenHotel3Info(h3);
                            if (!string.IsNullOrEmpty(his.Items[0].content))
                            {
                                result.packageItem.Intro.Description = his.Items[0].content;
                                foreach (var item in his.Items[0].content.Split("\r\n".ToCharArray()))
                                {
                                    result.packageItem.Intro.Item.Add(item);
                                }
                            }
                            break;
                        case 6:
                            result.AddressInfo.ArrivalAndDeparture = GenHotel3Info(h3);
                            break;
                            #region 微信图文 取hotelcontact中的ActivePageId
                            //case 9: //微信分享全文链接
                            //case 10: //微信链接为空时 取本地全文链接
                            //    Hotel3Entity h9 = h3s.Where(_ => _.Type == 9).FirstOrDefault();
                            //    if (h9 != null)
                            //    {
                            //        HotelInfo hi9 = GenHotel3Info(h9);

                            //        if (hi9.Items.Count > 0)
                            //        {
                            //            string strUrl9 = hi9.Items[0].content.Contains("#") == true ? (hi9.Items[0].content.Insert(hi9.Items[0].content.LastIndexOf("#"), "&_isshare=1")) : (hi9.Items[0].content + "&_isshare=1");
                            //            result.packageItem.Intro.ActionUrl = hi9.Items.Count == 0 ? "" : strUrl9;
                            //            result.packageItem.Intro.Text = "全文";
                            //        }
                            //        else
                            //        {
                            //            HotelInfo h = new HotelInfo();
                            //            h = GenHotel3Info(h3);
                            //            result.packageItem.Intro.ActionUrl = h.Items.Count == 0 ? "" : ("http://www.zmjiudian.com/active/activepage?midx=" + h.Items[0].content + "&_isshare=1");
                            //            result.packageItem.Intro.Text = "全文";
                            //        }
                            //    }
                            //    else
                            //    {
                            //        HotelInfo h = new HotelInfo();
                            //        h = GenHotel3Info(h3);
                            //        result.packageItem.Intro.ActionUrl = h.Items.Count == 0 ? "" : ("http://www.zmjiudian.com/active/activepage?midx=" + h.Items[0].content + "&_isshare=1");
                            //        result.packageItem.Intro.Text = "全文";
                            //    }
                            //    break; 
                            #endregion
                    }
                }

                try
                {
                    //获取微信图文
                    HJD.HotelServices.Contracts.HotelContacts hc = HotelService.GetHotelContacts(hotelId);
                    if (hc != null & hc.ActivePageId > 0)
                    {
                        result.packageItem.Intro.ActionUrl = "http://www.zmjiudian.com/active/activepage?pageid=" + hc.ActivePageId + "&_isshare=1";
                        result.packageItem.Intro.Text = "全文";
                    }
                    result.CouponInfo = CouponAdapter.GetCanCouponDefineByBizID(pid, 2, userId);
                }
                catch (Exception e)
                {
                    Log.WriteLog("套餐关联红包报错 ERROR：" + e);
                }



                log.Finish();
            }
            catch (Exception err)
            {
                Log.WriteLog(string.Format("GetPakcageDetailInfo pid：{0} userId:{1} startDate:{2} endDate:{3}", pid, userId, startDate, endDate) + err.Message + err.StackTrace);
            }

            SetPackageActivityInfo(result);

            return result;
        }

        /// <summary>
        /// 根据套餐ID返回套餐活动信息
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        private static void  SetPackageActivityInfo(RecommendPackageDetailResult pinfo)
        {
            int curActivityID = 1; // 1: 积分送里程
            if (IsPackageInActivity(curActivityID, pinfo))
            {
                GenActivityJson(curActivityID, pinfo);
            }
        }
        /// <summary>
        /// 生成活动JSON
        /// </summary>
        /// <param name="activityID"></param>
        /// <param name="pinfo"></param>
        /// <returns></returns>
        private static void  GenActivityJson(int activityID, RecommendPackageDetailResult pinfo)
        {
             switch (activityID)
            {
                case 1:
                string strPrice = pinfo.packageItem.SellVIPPrice.ToString();
                    pinfo.ActiviyInfo = @"{""activeID"":1, ""activeIcon"":""http://whfront.b0.upaiyun.com/www/img/Active/productalbumactive/active-detail-tip-1.png"",""activeTip"":""购买本产品可获赠" + strPrice + @"航空里程"",""activeLink"":""/Active/ProductAlbumActive?_newpage=1""}";
                    break;
            }             
        }

        /// <summary>
        /// 判断套餐是否在活动中
        /// </summary>
        /// <param name="activityID"></param>
        /// <param name="pinfo"></param>
        /// <returns></returns>
        private static bool IsPackageInActivity(int activityID, RecommendPackageDetailResult pinfo)
        {
            bool result = false;
            switch(activityID)
            {
                case 1:
                     result = GetCachedAlbumsPIDs(new List<int>{73,74}).Contains(pinfo.packageItem.PID);
                    break;
            }

            return result;
            }

        private static SortedSet<int> GetCachedAlbumsPIDs(List<int> albumsIdList)
       {
        //    SortedSet<int> pids = new SortedSet<int>();
        //    albumsIdList.ForEach(id => GetTop20Package(count: 1000, albumsId: id).Select(_ => _.PackageID).ToList().ForEach(_ => pids.Add(_)));
        //    return pids;
            return LocalCache10Min.GetData<SortedSet<int>>(string.Format("GetCachedAlbumsPIDs:{0}", string.Join("_", albumsIdList)), () =>
            {
                SortedSet<int> pids = new SortedSet<int>();
                GetTop20Package(count: 100, albumsId: 73).Select(_ => _.PackageID).ToList().ForEach(_ => pids.Add(_));
                GetTop20Package(count: 100, albumsId: 74).Select(_ => _.PackageID).ToList().ForEach(_ => pids.Add(_));
                return pids;
            });
        }

        #endregion

        #region 通过酒店名称拿到酒店id
        public static int GetHotelIdByName(string hotelName)
        {
            return HotelService.GetHotelIdByName(hotelName);
        }
        #endregion

        #region 房型列表
        public static List<PRoomInfoEntity> GetPRoomInfoList(IEnumerable<int> roomIds)
        {
            if (roomIds == null || !roomIds.Any())
            {
                return new List<PRoomInfoEntity>();
            }
            return HotelService.GetPRoomInfoEntityList(roomIds);
        }
        #endregion

        #region 房型照片信息
        public static Dictionary<int, IEnumerable<string>> GenRoomAndPicDic(IEnumerable<int> roomIds)
        {
            var roomDataList = GetPRoomInfoList(roomIds);
            var dic = new Dictionary<int, IEnumerable<string>>();
            foreach (var item in roomDataList.GroupBy(_ => _.ID))
            {
                var roomData = item.First();
                var picList = !string.IsNullOrWhiteSpace(roomData.PicShortNames) ?
                    roomData.PicShortNames.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(p => PhotoAdapter.GenHotelPicUrl(p, Enums.AppPhotoSize.appview)) : new List<string>() { "" };

                dic.Add(roomData.ID, picList);
            }
            return dic;
        }
        #endregion

        #region 获得酒店套餐列表的数据
        public static List<HotelTop1PackageInfoEntity> GetHotelTop1PackageInfo(IEnumerable<int> HotelIDList, DateTime CheckIn, DateTime CheckOut)
        {
            if (HotelIDList == null || !HotelIDList.Any())
            {
                return new List<HotelTop1PackageInfoEntity>();
            }
            return HotelService.GetHotelTop1PackageInfo(HotelIDList, CheckIn, CheckOut);
        }

        #endregion

        #region 可售地区酒店
        public static List<CanSellDistrictCheapHotel> GetCanSellDistrictCheapHotelList(HJD.HotelServices.Contracts.HotelServiceEnums.HotelDistrictRange range)
        {
            return HotelService.GetCanSellDistrictCheapHotelList(range);
        }
        #endregion

        #region 大家都推荐的酒店
        public static RecommendHotelResult GetHotRecommendHotel(BsicSearchParam param)
        {
            var geoStype = 1;
            if (param.districtid > 0)
            {
                param.lat = 0;
                param.lng = 0;
            }
            else if (param.lat > 0 && param.lng > 0)
            {
                var aroundCity = CalculateNearDistrictByDistance(param.districtid, param.lat, param.lng).OrderBy(_ => _.Distance).FirstOrDefault();//距离最近的是当前城市
                param.districtid = aroundCity != null ? aroundCity.DistrictID : 0;
                geoStype = 3;
            }

            var argu = new HotelSearchParas()
            {
                NeedHotelID = true,
                NeedFilterCol = false,
                AroundCityID = geoStype == 3 ? param.districtid : 0,
                Distance = 0,
                DistrictID = geoStype == 1 ? param.districtid : 0,
                SortDirection = 0,
                SortType = 10,
                CheckInDate = CommMethods.GetDefaultCheckIn(),
                CheckOutDate = CommMethods.GetDefaultCheckIn().AddDays(1),
                Lat = 0,
                Lng = 0,
                ReturnCount = param.count
            };

            var qResult = QueryHotelList4Recommend(argu);
            var totalCount = qResult.TotalCount;
            var hotels = qResult.HotelList;

            var result = new RecommendHotelResult()
            {
                HotelBlockTitle = "大家都说好",
                HotelTotalCount = totalCount,
                HotelList = new List<RecommendHotelItem>()
            };
            //var list = HotelService.GetTopScoreHotelList(param, out totalCount);
            if (totalCount > 0 && hotels.Any())
            {
                var hotelIdList = hotels.FindAll(_ => _.RecommendCount == 0).Select(_ => _.Id);//没有推荐数的酒店ID集合
                var hotelBasicInfoList = hotelIdList.Any() ? GetHotelBasicInfoList(hotelIdList).ToList() : new List<HotelBasicInfo>();

                result.HotelList = hotels.Select(_ => new RecommendHotelItem()
                {
                    ADDescription = "",
                    CustomerType = 0,
                    HotelID = _.Id,
                    HotelName = _.Name,
                    HotelPicUrl = _.PictureSURLList.FirstOrDefault() != null ? PhotoAdapter.GenHotelPicUrl(_.PictureSURLList.FirstOrDefault(), Enums.AppPhotoSize.theme) : "",
                    HotelPrice = 0,
                    MarketPrice = 0,
                    HotelReviewCount = _.ReviewCount,
                    HotelScore = _.Score == 0 ? (decimal)4.6 : _.Score,//如果按10排序的还是没评分 先设置4.6
                    NotVIPPrice = 0,
                    PackageBrief = "",
                    packageContent = new List<string>(),
                    PackageName = "",
                    packageNotice = new List<string>(),
                    PID = 0,
                    RecomemndWord = "",
                    RecommendCount = _.RecommendCount > 0 ? _.RecommendCount : hotelBasicInfoList.First(h => h.HotelID == _.Id).RecommendCount,
                    RecommendPicUrl = "",
                    VIPPrice = 0
                }).ToList();
            }
            return result;
        }

        public static QueryHotelResult3 QueryHotelList4Recommend(HotelSearchParas argu)
        {
            string cacheKey = string.Format("QueryHotelList4Recommend{0}{1}{2}{3}{4}{5}{6}", argu.AroundCityID, argu.DistrictID, argu.NeedHotelID, argu.NeedFilterCol, argu.ReturnCount, argu.SortType, argu.SortDirection);
            return LocalCache30Min.GetData<QueryHotelResult3>(cacheKey, () =>
            {
                return HotelService.QueryHotel3(argu);
            });
        }

        #endregion

        #region 区域内的点评
        public static List<PackageAlbumsEntity> GetPackageAlbumByGeoInfo(int districtID, float lat, float lng)
        {
            string cachekey = string.Format("GetPackageAlbumByGeoInfo:{0}:{1}:{2}", districtID, districtID > 0 ? 0 : Math.Round(lat * 100, 0), districtID > 0 ? 0 : Math.Round(lng * 100, 0));

            return LocalCache30Min.GetData<List<PackageAlbumsEntity>>(cachekey, () =>
            {
                return HotelService.GetPackageAlbumByGeoInfo(districtID, lat, lng);
            });
        }
        #endregion

        #region 判断是否支持WebP
        public static bool JudgeSupportWebP(string appVer, string appType)
        {
            var result = false;
            if (string.IsNullOrWhiteSpace(appVer) || string.IsNullOrWhiteSpace(appType))
            {

            }
            else if (appType.ToLower().Contains("android") && appVer.CompareTo("4.2") >= 0)
            {
                result = true;
            }
            return result;
        }
        #endregion

        #region 最近浏览记录

        public static RecommendHotelResult GetHotelBrowsingRecordList(BsicSearchParam param)
        {
            DateTime arrivalTime = CommMethods.GetDefaultCheckIn();

            var result = new RecommendHotelResult()
            {
                HotelBlockTitle = "最近浏览记录",
                HotelList = new List<RecommendHotelItem>(),
                HotelTotalCount = 0
            };

            int totalCount = 0;
            var list = HotelService.GetHotelBrowsingRecordList(param, out totalCount);

            if (totalCount > 0 && list.Any())
            {
                var hotelIdList = list.Select(_ => _.HotelId).ToList();//酒店ID集合
                var hotelPicsList = HotelAdapter.GetManyHotelPhotos(hotelIdList);

                //获取当前用户遵循的列表价类型
                var minPriceTypes = PriceAdapter.GetHotelMinPriceTypeByUser(param.userId);

                //获取HotelMinPrice列表价
                var hotelMinPriceList = PriceService.GetHotelMinPriceList(hotelIdList, arrivalTime) ?? new List<HotelMinPriceEntity>();
                var pricePlanList = new List<PricePlanEx>();

                if (hotelMinPriceList == null || hotelMinPriceList.Count == 0)
                {
                    //获取PricePlan列表价
                    pricePlanList = PriceService.GetPricePlanExList(hotelIdList, arrivalTime) ?? new List<PricePlanEx>();
                }

                ////获取列表价
                //var pricePlanList = PriceService.GetPricePlanExList(hotelIdList, arrivalTime) ?? new List<HJD.HotelPrice.Contract.DataContract.PricePlanEx>();

                result.HotelTotalCount = totalCount;
                result.HotelList = new List<RecommendHotelItem>();
                foreach (var _ in list)
                {
                    HotelItem hi = ResourceAdapter.GetHotel(_.HotelId, 0);
                    var _item = new RecommendHotelItem
                    {
                        ADDescription = "",
                        CustomerType = 0,
                        HotelID = _.HotelId,
                        HotelName = _.HotelName,
                        HotelPicUrl = hotelPicsList.FirstOrDefault(p => p.HotelId == _.HotelId) != null && hotelPicsList.First(p => p.HotelId == _.HotelId).HPList.Any() ? PhotoAdapter.GenHotelPicUrl(hotelPicsList.First(p => p.HotelId == _.HotelId).HPList[0].SURL, Enums.AppPhotoSize.theme) : "",
                        HotelPrice = 0,
                        HotelReviewCount = hi.ReviewCount,
                        HotelScore = hi.Score,
                        MarketPrice = 0,
                        NotVIPPrice = 0,
                        PackageBrief = "",
                        packageContent = new List<string>(),
                        PackageName = "",
                        packageNotice = new List<string>(),
                        PID = 0,
                        RecomemndWord = "",
                        RecommendCount = 0,
                        RecommendPicUrl = "",
                        VIPPrice = 0,
                        AvatarUrl = ""
                    };

                    var _minPriceEntity = PriceAdapter.GetHotelPricePlan(_.HotelId, pricePlanList, hotelMinPriceList, minPriceTypes, arrivalTime, false) ?? new HotelMinPriceEntity();
                    _item.HotelPrice = _minPriceEntity.Price;
                    _item.VIPPrice = _minPriceEntity.VipPrice;
                    _item.PackageBrief = _minPriceEntity.Brief;

                    result.HotelList.Add(_item);
                }
            }
            return result;
        }

        #region 注释

        //public static BrowsingRecordResult GetBrowsingRecordList(BsicSearchParam param)
        //{
        //    DateTime arrivalTime = CommMethods.GetDefaultCheckIn();

        //    var result = new BrowsingRecordResult()
        //    {
        //        BlockTitle = "最近浏览记录",
        //        BorwsRecordList = new List<BrowsingRecordItem>(),
        //        TotalCount = 0
        //    };

        //    int totalCount = 0;
        //    var list = HotelService.GetBrowsingRecordList(param, out totalCount);

        //    if (totalCount > 0 && list.Any())
        //    {
        //        List<int> hotelIdList = list.Where(h => h.BusinessType == 2).Select(_ => Convert.ToInt32(_.BusinessID)).ToList();//酒店ID集合
        //        var hotelPicsList = HotelAdapter.GetManyHotelPhotos(hotelIdList);

        //        //获取当前用户遵循的列表价类型
        //        var minPriceTypes = PriceAdapter.GetHotelMinPriceTypeByUser(param.userId);

        //        //获取HotelMinPrice列表价
        //        var hotelMinPriceList = PriceService.GetHotelMinPriceList(hotelIdList, arrivalTime) ?? new List<HotelMinPriceEntity>();
        //        var pricePlanList = new List<PricePlanEx>();

        //        if (hotelMinPriceList == null || hotelMinPriceList.Count == 0)
        //        {
        //            //获取PricePlan列表价
        //            pricePlanList = PriceService.GetPricePlanExList(hotelIdList, arrivalTime) ?? new List<PricePlanEx>();
        //        }

        //        result.TotalCount = totalCount;
        //        result.BorwsRecordList = new List<BrowsingRecordItem>();
        //        foreach (var _ in list)
        //        {
        //            if (_.BusinessType == 2)
        //            {
        //                HotelItem hi = ResourceAdapter.GetHotel(Convert.ToInt32(_.BusinessID), 0);
        //                var _item = new BrowsingRecordItem
        //                {
        //                    BrowRecordBizID = _.BusinessID,
        //                    BrowRecordBizType = _.BusinessType,
        //                    BrowRecordPicUrl = hotelPicsList.FirstOrDefault(p => p.HotelId == Convert.ToInt32(_.BusinessID)) != null && hotelPicsList.First(p => p.HotelId == Convert.ToInt32(_.BusinessID)).HPList.Any() ? PhotoAdapter.GenHotelPicUrl(hotelPicsList.First(p => p.HotelId == Convert.ToInt32(_.BusinessID)).HPList[0].SURL, Enums.AppPhotoSize.theme) : "",
        //                    BrowRecordNotVipPrice = 0,
        //                    BrowRecordVipPrice = 0,
        //                    BrowRecordBrief = ""
        //                };

        //                var _minPriceEntity = PriceAdapter.GetHotelPricePlan(Convert.ToInt32(_.BusinessID), pricePlanList, hotelMinPriceList, minPriceTypes, arrivalTime, false) ?? new HotelMinPriceEntity();
        //                _item.BrowRecordNotVipPrice = _minPriceEntity.Price;
        //                _item.BrowRecordVipPrice = _minPriceEntity.VipPrice;
        //                _item.BrowRecordBrief = _minPriceEntity.Brief;
        //                result.BorwsRecordList.Add(_item);
        //            }

        //        }
        //    }

        //    return result;
        //} 
        #endregion

        #endregion

        #region 获取300公里之内酒店

        public static RecommendHotelResult GetHotelWithInDistance(float lat = 0, float lng = 0, int count = 20, int start = 0)
        {
            var result = new RecommendHotelResult()
            {
                HotelBlockTitle = "周边300公里",
                HotelList = new List<RecommendHotelItem>()
            };

            DateTime arrivalTime = CommMethods.GetDefaultCheckIn();
            string cachekey = string.Format("GetHotelWithInDistance:{0}:{1}:{2}:{3}", Math.Round(lat * 100, 0), Math.Round(lng * 100, 0), count, start);
            string cachekeyCount = string.Format("GetHotelWithInDistance:{0}:{1}:{2}:{3}", Math.Round(lat * 100, 0), Math.Round(lng * 100, 0), count, start);

            int totalCount = 0;
            var list = LocalCache30Min.GetData<List<TopScoreHotelEntity>>(cachekey, () =>
            {
                return HotelService.GetHotelWithInDistance(lat, lng, count, start, out totalCount);
            });
            //var list = HotelService.GetHotelWithInDistance(lat, lng, pageIndex, pageSize, out totalCount);

            totalCount = Convert.ToInt32(LocalCache30Min.GetData<string>(cachekeyCount, () => { return totalCount.ToString(); }));

            if (list.Any())//totalCount > 0 && 
            {
                var hotelIdList = list.Select(_ => _.HotelId).ToList();//酒店ID集合
                var hotelPicsList = HotelAdapter.GetManyHotelPhotos(hotelIdList);

                //获取当前用户遵循的列表价类型
                var minPriceTypes = PriceAdapter.GetHotelMinPriceTypeByUser(0);

                //获取HotelMinPrice列表价
                var hotelMinPriceList = PriceService.GetHotelMinPriceList(hotelIdList, arrivalTime) ?? new List<HotelMinPriceEntity>();
                var pricePlanList = new List<PricePlanEx>();

                if (hotelMinPriceList == null || hotelMinPriceList.Count == 0)
                {
                    //获取PricePlan列表价
                    pricePlanList = PriceService.GetPricePlanExList(hotelIdList, arrivalTime) ?? new List<PricePlanEx>();
                }

                ////查询列表价
                //var pricePlanList = PriceAdapter.PriceService.GetPricePlanExList(hotelIdList, arrivalTime) ?? new List<HJD.HotelPrice.Contract.DataContract.PricePlanEx>();

                result.HotelTotalCount = totalCount;
                result.HotelList = list.Select(_ => new RecommendHotelItem()
                {
                    ADDescription = "",
                    CustomerType = 0,
                    HotelID = _.HotelId,
                    HotelName = _.HotelName,
                    HotelPicUrl = hotelPicsList.FirstOrDefault(p => p.HotelId == _.HotelId) != null && hotelPicsList.First(p => p.HotelId == _.HotelId).HPList.Any() ? PhotoAdapter.GenHotelPicUrl(hotelPicsList.First(p => p.HotelId == _.HotelId).HPList[0].SURL, Enums.AppPhotoSize.theme) : "",
                    HotelPrice = PriceAdapter.GetHotelPricePlan(_.HotelId, pricePlanList, hotelMinPriceList, minPriceTypes, arrivalTime, false).Price,
                    HotelReviewCount = 0,
                    HotelScore = 0,
                    MarketPrice = 0,
                    NotVIPPrice = 0,
                    PackageBrief = PriceAdapter.GetHotelPricePlan(_.HotelId, pricePlanList, hotelMinPriceList, minPriceTypes, arrivalTime, false).Brief,
                    packageContent = new List<string>(),
                    PackageName = "",
                    packageNotice = new List<string>(),
                    PID = 0,
                    RecomemndWord = "",
                    RecommendCount = 0,
                    RecommendPicUrl = "",
                    VIPPrice = 0,
                    AvatarUrl = ""
                }).ToList();
            }
            return result;
            //return new RecommendHotelResult();
        }
        #endregion

        #region 新增用户记录
        public static BasePostResult InsertUserRecord(CommonRecordParam recordParam)
        {
            if (recordParam.busniessId == 0 || recordParam.businessType == 0 || recordParam.recordType == 0 || (recordParam.userID == 0 && string.IsNullOrWhiteSpace(recordParam.sessionID) && string.IsNullOrWhiteSpace(recordParam.openID)))
            {
                return new BasePostResult()
                {
                    Success = false,
                    Message = "参数不完整无法记录"
                };
            }

            int id = 0;
            //1.分享
            if (recordParam.recordType == 1)
            {
                id = HotelAdapter.InsertShareRecord(new ShareRecordEntity()
                {
                    BusinessID = recordParam.busniessId,
                    BusinessType = recordParam.businessType,
                    ShareUser = recordParam.userID,
                    TerminalType = recordParam.terminalType,
                    ClientIP = recordParam.clientIP,
                    SessionID = recordParam.sessionID,
                    OpenID = recordParam.openID,
                    AppVer = recordParam.appVersion
                });
            }
            //2.浏览
            if (recordParam.recordType == 2)
            {
                id = HotelAdapter.InsertBrowsingRecord(new BrowsingRecordEntity()
                {
                    BusinessID = recordParam.busniessId,
                    BusinessType = recordParam.businessType,
                    Visitor = recordParam.userID,
                    TerminalType = recordParam.terminalType,
                    ClientIP = recordParam.clientIP,
                    SessionID = recordParam.sessionID,
                    OpenID = recordParam.openID,
                    AppVer = recordParam.appVersion
                });
            }
            //3.搜索
            if (recordParam.recordType == 3)
            {
                id = HotelAdapter.InsertSearchRecord(new SearchRecordEntity()
                {
                    BusinessID = recordParam.busniessId,
                    BusinessType = recordParam.businessType,
                    OptionUser = recordParam.userID,
                    TerminalType = recordParam.terminalType,
                    ClientIP = recordParam.clientIP,
                    SessionID = recordParam.sessionID,
                    OpenID = recordParam.openID,
                    AppVer = recordParam.appVersion
                });
            }

            return new BasePostResult()
            {
                Success = id > 0 ? true : false,
                Message = id > 0 ? "添加记录成功！" : "添加记录失败！"
            };
        }
        #endregion

        #region 搜索记录
        public static RecommendHotelResult GetSearchRecordList(CommonRecordQueryParam param, ContextBasicInfo _contextBasicInfo = null)
        {
            var result = new RecommendHotelResult()
            {
                HotelBlockTitle = "最近搜索记录",
                HotelList = new List<RecommendHotelItem>(),
                HotelTotalCount = 0
            };
            //
            int totalCount = 0;
            var list = HotelService.GetSearchRecordList(param, out totalCount);

            string actionUrl = @"whotelapp://www.zmjiudian.com/strategy/place?districtid={0}&title={1}";
            if (_contextBasicInfo != null && (_contextBasicInfo.IsThanVer6_0 || (!_contextBasicInfo.IsAndroid && !_contextBasicInfo.IsAndroid)))
            {
                actionUrl = @"http://www.zmjiudian.com/App/Around?userid={{userid}}&districtId={0}&districtName={1}&lat=0&lng=0&geoScopeType=1&_newpage=1&_headSearch=1&_searchType=1";
            }

            //var list = HotelService.GetSearchRecordList(new CommonRecordQueryParam()
            //{
            //    businessId = 0,
            //    businessType = 0,
            //    start = param.start,
            //    count = param.count,
            //    userId = param.userId
            //}, out totalCount);

            if (totalCount > 0 && list.Any())
            {
                var districtList = destService.GetDistrictInfo(list.Select(_ => (int)_.BusinessID).ToList());//地区信息集合

                result.HotelTotalCount = totalCount;
                result.HotelList = list.Select(_ => new RecommendHotelItem()
                {
                    ADDescription = "",
                    CustomerType = 0,
                    HotelID = (int)_.BusinessID,
                    HotelName = string.Format("{0}{1}", _.Name, districtList.FirstOrDefault(d => d.DistrictID == (int)_.BusinessID) != null ? "，" + districtList.First(d => d.DistrictID == (int)_.BusinessID).RootName : ""),
                    HotelPicUrl = !string.IsNullOrWhiteSpace(_.PicUrl) ? PhotoAdapter.GenHotelPicUrl(_.PicUrl, Enums.AppPhotoSize.theme) : "",
                    HotelPrice = 0,
                    HotelReviewCount = 0,
                    HotelScore = 0,
                    MarketPrice = 0,
                    NotVIPPrice = 0,
                    PackageBrief = "",
                    packageContent = new List<string>(),
                    PackageName = "",
                    packageNotice = new List<string>(),
                    PID = 0,
                    Id = _.ID,
                    RecomemndWord = "",
                    RecommendCount = 0,
                    RecommendPicUrl = "",
                    VIPPrice = 0,
                    AvatarUrl = "",
                    ActionURL = string.Format(actionUrl, _.BusinessID, _.Name),
                    Lat = _.Lat,
                    Lon = _.Lon
                }).OrderByDescending(_ => _.Id).ToList();
            }
            return result;
        }
        #endregion

        public static ActiveRuleGroupEntity GetActiveRuleList(int id)
        {
            var ActiveRuleGroup = new ActiveRuleGroupEntity();
            ActiveRuleGroup = HotelService.GetWXActiveRuleGroupList(id).FirstOrDefault();
            if (ActiveRuleGroup != null)
            {
                ActiveRuleGroup.ActiveRuleEx = HotelService.GetWXActiveRuleExList(ActiveRuleGroup.ID);
                var hotelIdList = ActiveRuleGroup.ActiveRuleEx.Select(_ => _.HotelId).ToList();//酒店ID集合
                var hotelPicsList = HotelAdapter.GetManyHotelPhotos(hotelIdList);
                ActiveRuleGroup.ActiveRuleEx = ActiveRuleGroup.ActiveRuleEx.Select(_ => new ActiveRuleExEntity()
                {
                    ActiveId = _.ActiveId,
                    Description = _.Description,
                    ID = _.ID,
                    GroupId = _.GroupId,
                    HotelId = _.HotelId,
                    OfferCount = _.OfferCount,
                    OrderNum = _.OrderNum,
                    PicUrl = string.IsNullOrWhiteSpace(_.PicUrl) ? (hotelPicsList.FirstOrDefault(p => p.HotelId == _.HotelId) != null && hotelPicsList.First(p => p.HotelId == _.HotelId).HPList.Any()) ? PhotoAdapter.GenHotelPicUrl(hotelPicsList.First(p => p.HotelId == _.HotelId).HPList[0].SURL, Enums.AppPhotoSize.theme) : "" : _.PicUrl,
                    RoomInfo = _.RoomInfo,
                    SubTitle = _.SubTitle,
                    Title = _.Title,
                    UpdateTime = _.UpdateTime
                }).OrderBy(_ => _.OrderNum).ToList();
            }
            return ActiveRuleGroup;
        }


        public static List<PointsEntity> GetPointslistNumByUserIdAndTypeId(long userId, int typeId)
        {
            return HotelService.GetPointslistNumByUserIdAndTypeId(userId, typeId);
        }
        /// <summary>
        /// 推送消息积分
        /// </summary>
        /// <param name="typeId"></param>
        /// <param name="ffRelIds"></param>
        /// <returns></returns>
        public static List<PointsEntity> GetPointListByTypeIDAndUserID(int typeId, IEnumerable<long> ffRelIds)
        {
            return HotelService.GetPointListByTypeIDAndUserID(typeId, ffRelIds);
        }
        /// <summary>
        /// 推送消息，根据类型和BusinessId
        /// </summary>
        /// <param name="typeId"></param>
        /// <param name="ffRelIds"></param>
        /// <returns></returns>
        public static List<PointsEntity> GetPointListByTypeIDAndBusinessID(int typeId, IEnumerable<long> ffRelIds)
        {
            return HotelService.GetPointListByTypeIDAndBusinessID(typeId, ffRelIds);
        }
        public static List<PackageItemEntity> GetHotelPackageByDistrictId(int districtId, DateTime checkIn, DateTime checkOut, int pageIndex, int pageSize, out int totalCount)
        {
            //return LocalCache10Min.GetData<List<PackageItemEntity>>(string.Format("HotelAdapter:GetHotelPackageByDistrictId{0}_{1}_{2}_{3}", districtId, checkIn, checkOut, totalCount), () =>
            //{
            //    List<PackageItemEntity> list = HotelService.GetHotelPackageByDistrictId(districtId, checkIn, checkOut, pageIndex, pageSize, out totalCount = 0);
            //    genPackageItemPicURL(list);
            //    return list;
            //});
            //return HotelService.GetHotelPackageByDistrictId(districtId, checkIn, checkOut, pageIndex, pageSize, out totalCount);
            List<PackageItemEntity> list = HotelService.GetHotelPackageByDistrictId(districtId, checkIn, checkOut, pageIndex, pageSize, out totalCount);
            genPackageItemPicURL(list);
            return list;
        }

        private static void genPackageItemPicURL(IEnumerable<PackageItemEntity> list)
        {
            var hotelIds = list.Select(_ => _.HotelID).Distinct();

            var hotelPicsList = HotelAdapter.GetManyHotelPhotos(hotelIds);

            var finalHasPicHotel = new Dictionary<int, List<HJD.HotelServices.Contracts.HotelPhotoEntity>>();
            foreach (var item in hotelPicsList)
            {
                HJD.HotelServices.Contracts.HotelPhotoEntity photo = item.HPList.FirstOrDefault();
                if (photo != null && photo.HotelID > 0)
                {
                    finalHasPicHotel.Add(photo.HotelID, item.HPList);
                }
            }

            foreach (var temp in list)
            {
                int tempHotelId = temp.HotelID;
                if (finalHasPicHotel.ContainsKey(tempHotelId))
                {
                    temp.PicUrls = new List<string>() { PhotoAdapter.GenHotelPicUrl(finalHasPicHotel[tempHotelId][0].SURL, Enums.AppPhotoSize.appdetail1) };
                }
                else
                {
                    temp.PicUrls = new List<string>();
                }
            }
        }


        public static HJD.HotelManagementCenter.Domain.TemplateDataEntity GetTemplateDataByBizIdAndBizType(int bizId, int bizType)
        {
            HJD.HotelManagementCenter.Domain.TemplateDataEntity model = HMC_HotelService.GetTemplateDataList(0, bizId, bizType).FirstOrDefault();
            return model != null ? model : new HJD.HotelManagementCenter.Domain.TemplateDataEntity();
        }

        public static HJD.HotelManagementCenter.Domain.TemplateSourceEntity GetTempSourceById(int id)
        {
            HJD.HotelManagementCenter.Domain.TemplateSourceEntity model = HMC_HotelService.GetTemplateSourceList(id).FirstOrDefault();
            return model != null ? model : new HJD.HotelManagementCenter.Domain.TemplateSourceEntity();
        }

        public static int InserOrUpdateTemplateData(HJD.HotelManagementCenter.Domain.TemplateDataEntity model)
        {
            return HMC_HotelService.InserOrUpdateTemplateData(model);
        }
        public static int GetHotelDistrictID(int hotelid)
        {
            HJD.HotelManagementCenter.Domain.HotelEditEntity hotelinfo= HMC_HotelService.GetHotelEditInfo(hotelid);
            if(hotelinfo!=null)
            {
                return hotelinfo.DistrictID;
            }
            else
            {
                return 0;
            }
        }

        public static RetailHotelEntity GetRetailHotelList(int type, int sort, string searchWord, int start, int count)
        {
            RetailHotelEntity list = HotelService.GetRetailHotelList(type, sort, searchWord, start, count);
            return list; 
        }

        public static RetailHotelInfoEntity GetRetailPackageList(int hotelId)
        {
            RetailHotelInfoEntity result = new RetailHotelInfoEntity();

            result = HotelService.GetRetailHotelInfo(hotelId);

            //多天累加
            //PriceAdapter.GetManyDaysPackagePriceCached(_.HotelID, _.PackageID, userType, _nightCount, out _totalPrice, out _totalPricePrice);
            return result;
        }

        internal static RetailPackageEntity GetRetailProductByID(DateTime checkIn, DateTime checkOut, int pid, long cid)
        {
            try
            {
                RetailPackageEntity retailPackage = HotelService.GetRetailPackageInfo(checkIn,checkOut,pid, cid);


                string ResourceUrl = "";
                List<Hotel3Entity> h3s = HotelService.GetHotel3(retailPackage.HotelId);
                //Hotel3Entity h9 = h3s.Where(_ => _.Type == 9).FirstOrDefault();
                //if (h9 != null)
                //{
                //    HotelInfo hi9 = GenHotel3Info(h9);
                //    if (hi9.Items.Count > 0)
                //    {
                //        string strUrl9 = hi9.Items[0].content.Contains("#") == true ? (hi9.Items[0].content.Insert(hi9.Items[0].content.LastIndexOf("#"), "&_isshare=1&CID=" + cid.ToString())) : (hi9.Items[0].content + "&_isshare=1&CID=" + cid.ToString());
                //        ResourceUrl = hi9.Items.Count == 0 ? "" : strUrl9;
                //    }
                //}
                foreach (Hotel3Entity h3 in h3s)
                {
                    switch (h3.Type)
                    {
                        case 1://推荐理由
                            retailPackage.Intro = new CommentTextAndUrlEx();
                            HotelInfo his = new HotelInfo();
                            his = GenHotel3Info(h3);
                            if (!string.IsNullOrEmpty(his.Items[0].content))
                            {
                                retailPackage.Intro.Description = his.Items[0].content;
                                foreach (var item in his.Items[0].content.Split("\r\n".ToCharArray()))
                                {
                                    retailPackage.Intro.Item.Add(item);
                                }
                            }
                            break;
                            #region 图文连接 取hotelcontact表中ActivePageId
                            //case 9:
                            //    HotelInfo hi9 = GenHotel3Info(h3);
                            //    if (hi9.Items.Count > 0)
                            //    {
                            //        string strUrl9 = hi9.Items[0].content.Contains("#") == true ? (hi9.Items[0].content.Insert(hi9.Items[0].content.LastIndexOf("#"), "&_isshare=1&CID=" + cid.ToString())) : (hi9.Items[0].content + "&_isshare=1&CID=" + cid.ToString());
                            //        ResourceUrl = hi9.Items.Count == 0 ? "" : strUrl9;
                            //    }
                            //    break; 
                            #endregion
                    }
                }

                //获取微信图文
                HJD.HotelServices.Contracts.HotelContacts hc = HotelService.GetHotelContacts(retailPackage.HotelId);
                if (hc != null & hc.ActivePageId > 0)
                {
                    ResourceUrl = "http://www.zmjiudian.com/active/activepage?pageid=" + hc.ActivePageId + "&_isshare=1&CID=" + cid.ToString();
                }


                string date = "&checkInstr=" + checkIn.ToShortDateString() + "&checkOutstr=" + checkOut.ToShortDateString();
                string ProductUrl = string.Format(Configs.PackageURL, pid, cid, date);


                if (retailPackage.ResourceUrl != ResourceUrl || retailPackage.ProductUrl != ProductUrl) //如果没有短链生成，那么需要生成短链
                {
                    if (retailPackage.ProductUrl != ProductUrl)
                    {
                        retailPackage.ProductUrl = ProductUrl;

                        retailPackage.ShortProductUrl = CommMethods.GenShortenUrl(retailPackage.ProductUrl);
                    }

                    if (ResourceUrl.Length == 0)
                    {
                        retailPackage.ResourceUrl = "";
                        retailPackage.ShortResourceUrl = "";
                    }
                    else if (retailPackage.ResourceUrl != ResourceUrl)
                    {
                        retailPackage.ResourceUrl = ResourceUrl;
                        if (ResourceUrl.Contains("mp.weixin.qq.com"))
                        {
                            retailPackage.ShortResourceUrl = ResourceUrl;
                        }
                        else
                        {
                            retailPackage.ShortResourceUrl = CommMethods.GenShortenUrl(retailPackage.ResourceUrl);
                        }
                    }

                    CouponAdapter.AddRetailUrl(new HJD.CouponService.Contracts.Entity.RetailUrlEntity
                    {
                        CID = cid,
                        CreateTime = DateTime.Now,
                        ProductUrl = ProductUrl,
                        ResourceURL = ResourceUrl,
                        ShortProductUrl = retailPackage.ShortProductUrl,
                        ShortResourceURL = retailPackage.ShortResourceUrl,
                        SKUID = 0,
                        PID = pid
                    });

                } 

                return retailPackage;
            }
            catch (Exception err)
            {
                Log.WriteLog("GetRetailProductByID: Error:" + err.Message + err.StackTrace);

                return new RetailPackageEntity();
            }
        }

        internal static List<PackageAlbumsEntity> GetPackageAlbumsByGroupNo(string groupNo)
        {
            List<PackageAlbumsEntity> result = new List<PackageAlbumsEntity>();
            List<PackageAlbumsEntity> albumlist = HotelService.GetPackageAlbumsByGroupNo(groupNo);
            foreach(PackageAlbumsEntity item in albumlist)
            {
                if (!string.IsNullOrWhiteSpace(item.CoverPicSUrl) || !string.IsNullOrWhiteSpace(item.LabelPicSUrl))
                {
                    item.CompleteCoverPicSUrl = PhotoAdapter.GenHotelPicUrl((item.CoverPicSUrl == null || item.CoverPicSUrl == "") ? item.LabelPicSUrl : item.CoverPicSUrl, Enums.AppPhotoSize.jupiter);
                }
            }
            return albumlist.OrderBy(_=>_.Rank).ToList();
        }

        internal static List<PackageAlbumsEntity> GetAllPackageAlbums()
        {
            List<PackageAlbumsEntity> albumlist = HotelService.GetAllPackageAlbums();
            return albumlist;
        }

        public static bool UpdateAlbumRedisCache(int albumId, int pid)
        {
            return HotelHelper.UpdateAlbumRedisCache(albumId, pid);
        }
        public static PackageAlbumDetail GetPackageAlbumDetailList(int albumId, int startDistrictId=0, int count=100, bool isNeedNotSale=false)
        {
            try
            {
                //var albumEntity = HotelAdapter.GetOnePackageAlbums(albumId);
                //albumEntity.CoverPicSUrl = !string.IsNullOrWhiteSpace(albumEntity.CoverPicSUrl) ? PhotoAdapter.GenHotelPicUrl(albumEntity.CoverPicSUrl, Enums.AppPhotoSize.jupiter) : "";
                var regex = new Regex("_.*$", RegexOptions.Compiled | RegexOptions.RightToLeft);

                List<TopNPackageItem> packageList = HotelAdapter.GetTop20GroupPackage(RedisHelper.CacheCount, albumId, isNeedNotSale: isNeedNotSale, startDistrictID: startDistrictId);

                packageList = packageList.Where(_ => _.PackagePrice.Exists(j => j.Type == 0) && _.PackagePrice.Exists(j => j.Type == -1)).ToList();
               // Log.WriteLog("PackageAlbumDetail.packagelist =" + JsonConvert.SerializeObject(packageList));
                var result = new PackageAlbumDetail()
                {
                    //albumEntity = albumEntity,
                    packageList = packageList.Select(_ => new RecommendHotelItem()
                    {
                        HotelID = _.HotelID,
                        HotelName = _.HotelName,
                        HotelPicUrl = _.PicUrls.Any() ? regex.Replace(_.PicUrls.First(), "_theme") : "",
                        HotelPrice = _.PackagePrice.First(j => j.Type == 0).Price,
                        VIPPrice = _.PackagePrice.First(j => j.Type == -1).Price,
                        TotalHotelPrice = _.PackagePrice.First(j => j.Type == 0).Price * _.DayLimitMin,
                        TotalVIPPrice = _.PackagePrice.First(j => j.Type == -1).Price * _.DayLimitMin,
                        DayLimitMin = _.DayLimitMin,
                        ADDescription = string.IsNullOrWhiteSpace(_.Title) ? "" : _.Title,
                        MarketPrice = _.MarketPrice,
                        PackageBrief = _.PackageBrief,
                        PID = _.PackageID,
                        RecommendPicUrl = string.IsNullOrEmpty(_.RecomendPicShortNames) ? "" : ("http://whphoto.b0.upaiyun.com/" + _.RecomendPicShortNames + "_theme"),
                        RecommendPicUrl2 = string.IsNullOrEmpty(_.RecomendPicShortNames2) ? "" : ("http://whphoto.b0.upaiyun.com/" + _.RecomendPicShortNames2 + "_theme"),
                        HotelReviewCount = _.HotelReviewCount,
                        HotelScore = _.HotelScore,
                        RecomemndWord = _.RecomemndWord,
                        RecomemndWord2 = _.RecomemndWord2,
                        PackageName = _.PackageName,
                        DistrictId = _.DistrictId,
                        DistrictName = _.DistrictName,
                        DistrictEName = _.DistrictEName,
                        ProvinceName = _.ProvinceName,
                        InChina = _.InChina,
                        Rank = _.Rank,
                        StartDistrictId = _.StartDistrictId,
                        StartDistrictName = _.StartDistrictName,
                        Title = _.Title,
                        PackageGroupName=_.PackageGroupName
                    }).ToList()
                    //,
                    //shareModel = new CommentShareModel()
                    //{
                    //    Content = albumEntity.SubTitle,
                    //    notHotelNameTitle = "",
                    //    title = albumEntity.SubTitle,
                    //    shareLink = "",
                    //    photoUrl = regex.Replace(albumEntity.CoverPicSUrl, "_290x290s"),
                    //    returnUrl = ""
                    //}
                };

                if (result.packageList.Count > 0)
                {
                    HotelHelper.AddAlbumToRedis(albumId, result);
                }
                return result;
            }
            catch (Exception ex)
            {
                Log.WriteLog("HotelAdapter.GetPackageAlbumDetailList" + ex.Message + ex.StackTrace);
                return new PackageAlbumDetail();
            }
        }

        /// <summary>
        /// 下线过期套餐
        /// </summary>
        /// <returns></returns>
        public static bool OfflineOverTimePackage()
        {
            try
            {
                var packageList = HMC_PackageService.GetOverSaleEndTimePackageList();
                {
                    foreach (var item in packageList)
                    {
                        if (item.PackageState == 1)
                        {
                            var packageEntity = HMC_PackageService.GetPackage(item.ID, 0).FirstOrDefault();
                            if (packageEntity != null)
                            {
                                packageEntity.PackageState = 2;
                                HMC_PackageService.UpdatePackage(packageEntity);
                                HotelHelper.UpdateAlbumRedisCache(0, packageEntity.ID);
                            }
                            
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.WriteLog("过了售卖时间套餐下线失败，错误信息：" + ex.Message + ex.StackTrace);
            }
            return true;
        }
        public static HJD.HotelManagementCenter.Domain.PackageEntity GetPackage(int pid)
        {
            return HMC_PackageService.GetPackage(pid, 0, -1).FirstOrDefault();
        }
    }
}