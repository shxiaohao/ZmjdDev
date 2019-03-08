using System;
using System.Collections.Generic;
using System.Linq;
using HJDAPI.Models;
using HJD.DestServices.Contract;
using HJD.Framework.WCF;
using HJD.HotelServices.Contracts;
using HJD.PhotoServices.Entity;
using HJD.Framework.Interface;
using HJDAPI.Common.Helpers;
using HJD.HotelServices;
using HJDAPI.Controllers.Adapter;

namespace HJDAPI.Controllers
{
    public class ResourceAdapter
    {
        public static ICacheProvider LocalCache = CacheManagerFactory.Create("DynamicCacheForType2");
        public static ICacheProvider LocalCache30Min = CacheManagerFactory.Create("DynamicCacheForADV");
        //public static ICacheProvider LocalCache6Hour = CacheManagerFactory.Create("DynamicCacheForStrategyDistrict");
        public static IHotelService HotelService = ServiceProxyFactory.Create<IHotelService>("BasicHttpBinding_IHotelService");
        public static IDestService DestService = ServiceProxyFactory.Create<IDestService>("BasicHttpBinding_IDestService");
        
        public static Dictionary<int,string> dicHotelTheme = new Dictionary<int,string>();
        public static Dictionary<int, string> dicInterest = new Dictionary<int, string>();

        /// <summary>
        /// 是否是有套餐的酒店 
        /// </summary>
        /// <param name="hotelid"></param>
        public static bool IsPackageHotel(int hotelid )
        {
           return   PackageHotelIDList.Contains(hotelid);
        }

        public static HashSet<int> mPackageHotelIDList ;
        public static HashSet<int> PackageHotelIDList
        {
            get
            {
                if (mPackageHotelIDList == null)
                {
                    mPackageHotelIDList = new HashSet<int>();
                    foreach (int h in HotelService.GetPackageHotelList())
                    {
                        mPackageHotelIDList.Add(h);
                    }
                }

                return mPackageHotelIDList;
            }
        }

        public static HotelItem GetHotel(int id)
        {
            return GetHotel(id, 0);
        }

        public static HotelItem GetHotel(int id, long userId)
        {
            var model = HotelService.GetHotel(id);

            if (model != null && model.Id > 0)
            {
                HotelPhotosEntity phe = HotelService.GetHotelPhotos(id);
                 model.PictureCount = phe.HPList.Count();
                 if (model.PictureCount > 0)
                     model.Picture = PhotoAdapter.GenHotelPicUrl(phe.HPList[0].SURL, Enums.AppPhotoSize.appdetail);
                 else
                     model.Picture = "";
              
                //var pics = PhotoAdapter.GetTopHotelPhotos(new List<int> { id });
                //model.PictureCount = 0;// PhotoAdapter.GetHotelPhotoCount(id, userId);
                //if (pics != null && pics.Count > 0)
                //    model.Picture = pics.ContainsKey(id) ? pics[id] : string.Empty;
                //model.OfficalPictureCount = 0;// PhotoAdapter.GetHotelOfficalPhotoCount(id, userId);
            }
            else
            {
                model = new HotelItem()
                {
                    Name = "酒店不存在",
                    Description = "",
                    Score = 0,
                    Star = 0,
                    Picture = ""
                };
            }
            return model;
        }

        public static SightItem GetSight(int id, long userId)
        {
            var model = DestService.GetSight(id);
            var pics = PhotoAdapter.GetTopSightPhotos(new List<int> { id });
            model.PictureCount = PhotoAdapter.GetSightPhotoCount(id, userId);
            model.Picture = pics.ContainsKey(id) ? pics[id] : string.Empty;
            return model;
        }

        public static List<DistrictInfoEntity> GetDistrictInfo(List<int> ids)
        {
           return DestService.GetDistrictInfo(ids);
        }

        public static string GetDistrictName(List<int> ids)
        {
            if (ids != null && ids.Count > 0)
            {
                var di = DestService.GetDistrictInfo(ids).FirstOrDefault();


                return di != null && di.DistrictID > 0 ? di.Name:"" ;
            }
            return "";
        }

        private static int GetRate(string str)
        {
            var arr = str.Split('/');
            if (arr.Length != 2)
            {
                return 0;
            }
            var a = arr[0];
            var b = arr[1];
            double ba;
            double.TryParse(a, out ba);
            double bb;
            double.TryParse(b, out bb);
            if (bb == 0 || bb < ba)
            {
                return 0;
            }


            var r = Math.Floor(ba / bb * 100);
            return (int)r;
        }

        public static List<HotelTopItem> GetHotelTopList(int id)
        {
            var list = DestService.GetHJDWordListByKindID(id);
            var hotelIds = list.Select(p => p.KeyID).ToList();
            var hotels = HotelService.GetHotels(hotelIds);
            var hotelDict = new Dictionary<int, HotelItem>();
            foreach (var item in hotels)
            {
                if (!hotelDict.ContainsKey(item.Id))
                {
                    hotelDict.Add(item.Id, item);
                }
            }
            var phsIds = list.Select(p => p.PHSID).ToList();
            var phs = PhotoAdapter.GetPhotos(phsIds);
            var photoDict = new Dictionary<int, string>();
            foreach (var item in phs)
            {
                if (!photoDict.ContainsKey(item.PHSID))
                {
                    photoDict.Add(item.PHSID, item.PhotoUrl[PhotoSizeType.thumb]);
                }
            }

            var res = list.Select(p => new HotelTopItem
                               {
                                   Id = p.KeyID,
                                   CommentContent = p.Comment,
                                   ReviewCount = p.Comments,
                                   Name = hotelDict.ContainsKey(p.KeyID) ? hotelDict[p.KeyID].Name : string.Empty,
                                   CommentRate = GetRate(p.RecommendDegree),
                                   Rank = p.Rank,
                                   PicUrl = photoDict.ContainsKey(p.PHSID) ? photoDict[p.PHSID] : string.Empty,
                                   Score = hotelDict.ContainsKey(p.KeyID) ? (int)Math.Round(hotelDict[p.KeyID].Score) : 0,
                               }).ToList();

            return res;
        }

        public static HJDWordKindsEntity GetHotelTopKind(string name)
        {
            var res = DestService.GetHJDWordKindsInfo(name);
            return res;
        }

        public static string GetOtaHotelUrl(string channel, int otaHotelID, string sourcePageType, string sType)
        {
            //实现方法移动至HotelService
            return HotelService.GetOtaHotelUrl(channel,  otaHotelID,  sourcePageType,  sType);           
        }
        private static string AddZeroFront(int otaHotelID)
        {
            List<int> ugly = new List<int> { 1101002, 1102002, 1104002, 1105002, 1801002, 1915003, 1915004, 1915005, 2301002 };
            return ugly.Contains(otaHotelID) ? otaHotelID.ToString() : otaHotelID.ToString().PadLeft(8, '0');
        }

        private static string GetBookingUrl(int hotelID)
        {
            return LocalCache.GetData<string>("bookingUrl:" + hotelID.ToString(), () =>
            {
                return HotelService.GetBookingUrlByHotel(hotelID).Replace("\t", "");
            });
        }

        private static string GetAgodaUrl(int hotelID)
        {
            return LocalCache.GetData<string>("agodaUrl:" + hotelID.ToString(), () =>
            {
                return HotelService.GetAgodaUrlByHotel(hotelID).Replace("\t", "");
            });
        }

        private static OTACodeEntity GetOTAHotelCode(int channelid, int otahotelid)
        {
            return LocalCache.GetData<OTACodeEntity>(String.Format("OTAHotelCode:{0}:{1}", channelid, otahotelid), () =>
            {
                return HotelService.GetOTAHotelCode(channelid,new List<int>{ otahotelid}).FirstOrDefault();
            });
        }

         private static string GetQunarHotelInfo(int hotelID)
        {
            return LocalCache.GetData<string>("QunerHotel:" + hotelID.ToString(), () =>
            {
                return "";// TODO HotelService.GetQunarHotelInfo(hotelID);
            });
        }

        static Dictionary<int, string> dicAttractionNames;
        public static String GetAttractionName(int aid)
        {
            if (dicAttractionNames == null)
            {
                dicAttractionNames = GetAllAttraction().ToDictionary(K => K.ID, V => V.Name);
            }
            return dicAttractionNames.ContainsKey(aid) ? dicAttractionNames[aid] : "";
        }

        public static List<HotelThemeEntity> GetAllHotelTheme()
        {
            return HotelService.GetAllHotelTheme();
        }

        public static InterestModel QueryInterest(int districtid, float lat, float lng, int distance= 300000)
        {
            List<HotelThemeEntity> htl = HotelService.GetHotelTheme(districtid, lat, lng);

            List<AttractionEntity> al;
            if (districtid > 0)
                al = DestService.GetAttractionByCity(districtid);
            else
            {
                AttractionQueryParam p = new AttractionQueryParam();
                p.lat = lat;
                p.lng = lng;
                p.distanceFrom = 0;
                p.distanceTo = 300000; //300Km
                p.count = 100;
                al = DestService.QueryAttraction(p).AttractionList;
            }

            var ieth = from h in htl
                     select new InterestEntity { GLat = 0, GLon = 0, HotelCount = h.HotelCount, HotelList = h.Hotels, ID = h.ID, Type = 1, ImageUrl = h.ImgUrl, Name = h.Name };

            var iea = from h in al
                     select new InterestEntity { GLat = h.Glat??0, GLon = h.Glon??0, HotelCount = h.HotelCount, HotelList = h.Hotels, ID = h.ID, Type = 2, ImageUrl = h.ImageUrl.Replace("600x280","290x290"), Name = h.ShortName };

            InterestModel model = new InterestModel();
            model.districtid = districtid;
            model.GLat = lat;
            model.GLon = lng;
            model.Name = "";
            model.InterestList = ieth.ToList();
            model.InterestList.AddRange(iea);

            return model;
        }

        public static InterestModel QueryInterest2(int districtid, float lat, float lng, int distance = 300000)
        {
            if (districtid > 0)
            {
                return LocalCache30Min.GetData<InterestModel>("QueryInterest" + districtid, () =>
                {
                    return GenQueryInterest2(districtid, lat, lng, distance);
                });
            }
            else
            {
                return GenQueryInterest2(districtid,  lat,  lng,  distance);
            }
        }
        
        public static InterestModel2 QueryInterest20(int districtid, float lat, float lng, int distance = 300000, bool withPlaceIDs = true)
        {
            if (districtid > 0)
            {
                return LocalCache30Min.GetData<InterestModel2>(string.Format("QueryInterest20:{0}:{1}:{2}" , districtid ,withPlaceIDs, distance), () =>
                {
                    return GenQueryInterest20(districtid, lat, lng, distance, withPlaceIDs);
                });
            }
            else
            {
               return LocalCache30Min.GetData<InterestModel2>(string.Format("QueryInterest20:{0}:{1}:{2}:{3}", (int)(lat * 10000), (int)(lng * 10000), withPlaceIDs, distance), () =>
               {
                   return GenQueryInterest20(districtid, lat, lng, distance, withPlaceIDs);
               });
            }
        }

        /// <summary>
        /// 景点有分类
        /// </summary>
        /// <param name="districtid"></param>
        /// <param name="lat"></param>
        /// <param name="lng"></param>
        /// <param name="distance"></param>
        /// <param name="withPlaceIDs"></param>
        /// <returns></returns>
        public static InterestModel2 QueryInterest30(int districtid, float userLat, float userLng, int geoScopeType, int distance = 300000)
        {
            string cachekey = string.Format("QueryInterest30:{0}:{1}", districtid, geoScopeType);
            if (geoScopeType == (int)EnumHelper.GeoScopeType.AroundCoordinate)
                cachekey = string.Format("QueryInterest30:{0}:{1}", (int)(userLat * 100), (int)(userLng * 100));

  //return GenQueryInterest30(districtid, userLat, userLng, geoScopeType);

            return LocalCache.GetData<InterestModel2>(cachekey, () =>
                {
                    return GenQueryInterest30(districtid, userLat, userLng, geoScopeType);
                });
        }


        public static InterestModel GenQueryInterest2(int districtid, float lat, float lng, int distance = 300000)
        {
           
            List<InterestEntity> htl = HotelService.QueryInterest(districtid, lat, lng);


            InterestModel model = new InterestModel();
            model.districtid = districtid;
            model.GLat = lat;
            model.GLon = lng;
            model.Name = "";
            model.InterestList = htl;

            return model;
        }


        public static List<InterestEntity2> GetAllInterest()
        {
            return HotelService.GetAllInterestList();
        }

        public static List<Int32> GetHotelInterestIDs(Int32 hotelid )
        {
            return HotelService.GetHotelInterestIDs(hotelid);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="districtid"></param>
        /// <param name="lat"></param>
        /// <param name="lng"></param>
        /// <param name="distance"></param>
        /// <param name="withPlaceIDs">是否返回PlaceIDs</param>
        /// <returns></returns>
        public static InterestModel2 GenQueryInterest20(int districtid, float lat, float lng, int distance = 300000, bool withPlaceIDs=false)
        {

            InterestQueryEntity htl = HotelService.QueryInterestAndHotelCount(districtid, lat, lng);
                      
            InterestModel2 model = new InterestModel2();
            model.districtid = districtid;
            model.GLat = lat;
            model.GLon = lng;
            model.Name = "";
            model.ThemeInterestList = htl.interestList.Where(h => h.Type == 1).ToList();
            model.SightInterestList = htl.interestList.Where(h => h.Type == 2).ToList();
            model.HotInterestList = new List<InterestEntity>();
            model.TotalHotelNum = htl.hotelCount;            

            //准备特惠等广告内容
            List<CommDictEntity> ld = HotelService.GetCommDictList(10001);
            foreach (CommDictEntity dic in ld)
            {
                string[] l = dic.Descript.Split('|');
                InterestEntity hi = new InterestEntity();
                hi.Name = dic.DicValue;
                hi.ID = int.Parse(l[0]) ;
                hi.ImageUrl = l[2];
                hi.Type = int.Parse(l[1]);
                hi.HotelList = "";
                hi.HotelCount = 0;
                if (hi.Type == 3) //特惠精选
                {

                    int geoScopeType = districtid > 0 ? 1 : 3;
                    List<string> ipids = GetSpecialDealInterestPlaceIDs(districtid, lat, lng, geoScopeType);
                    hi.InterestPlaceIDs = withPlaceIDs? string.Join(",",ipids):"";
                    hi.DistrictID = districtid;
                    hi.GLat = lat;
                    hi.GLon = lng;

                    HotelSearchParas pars = new HotelSearchParas();

                    pars.Interest = hi.ID;
                    pars.InterestPlace =  ipids.Select(i => int.Parse(i)).ToArray();
                    pars.NeedHotelID = false;
                    pars.NeedFilterCol = false;
                    pars.StartIndex = 0;
                    pars.ReturnCount = 1;
                    pars.CheckInDate = DateTime.Now;
                    pars.CheckOutDate = DateTime.Now;
                  
                    QueryHotelResult2  qr =  HotelService.QueryHotel2(pars, 0);
                    hi.HotelCount = qr.TotalCount;

                    if (hi.HotelCount > 0)
                    {
                        model.HotInterestList.Add(hi);
                    }

                }
                else if (hi.Type == 4) //限时特惠，只投放酒店周围
                {
                    int hotelid = int.Parse(l[4]);
                    hi.InterestPlaceIDs = l[3];

                  HotelItem h =  HotelService.GetHotel(hotelid);

                  if (lat > 0)//如果是周边
                  {
                      //如果距离在周边，则显示
                      double hdistance = Common.CommMethods.CalcDistance(h.GLat, h.GLon, lat, lng);
                      if ( hdistance < distance)
                      {
                          model.HotInterestList.Add(hi);
                      }
                  }
                  else
                  {
                      //如果同目的地则显示
                      if (h.DistrictId == districtid)
                      { model.HotInterestList.Add(hi); }
                  }

                }
                else if (hi.Type == 5) //限时特惠，投放所有目的地
                {
                    hi.InterestPlaceIDs = l[3];//连接地址
                    model.HotInterestList.Add(hi);
                }

            }

            if (withPlaceIDs == false)
            {
                foreach (InterestEntity h in htl.interestList)
                {
                    h.InterestPlaceIDs = "";
                }
            }

    
            var aclist = from a in GetAttractionCategoryRel()
                         join s in model.SightInterestList
                           on a.InterestID equals s.ID
                         select a;

            model.SightCategoryList = (from a in aclist
                                  group a by a.ID into aGroup
                                  select new SightCategory() 
                                  { ID = aGroup.Key,
                                      Name = aGroup.First().Name, 
                                      InterestID = aGroup.Select(s => s.InterestID).ToList() }).ToList();


            return model;
        }

        public static InterestQueryEntity GetInterestAndHotelCount(int districtid, float userLat, float userLng, int geoScopeType)
        {
            string cachekey = string.Format("InterestAndHotelCount:{0}:{1}", districtid, geoScopeType);
            if (geoScopeType == (int)EnumHelper.GeoScopeType.AroundCoordinate)
                cachekey = string.Format("InterestAndHotelCount:{0}:{1}", (int)(userLat * 100), (int)(userLng * 100));

            return LocalCache30Min.GetData<InterestQueryEntity>(cachekey, () =>
                {
                    return GenInterestAndHotelCount(districtid, userLat, userLng, geoScopeType);
                });
        }

        public static InterestQueryEntity GenInterestAndHotelCount(int districtid, float userLat, float userLng, int geoScopeType)
        {
            InterestQueryEntity htl = new InterestQueryEntity();
             switch ((EnumHelper.GeoScopeType)geoScopeType)
            {
                case EnumHelper.GeoScopeType.District:
                    htl = HotelService.QueryInterestAndHotelCount(districtid, 0, 0);
                    break;               
                case EnumHelper.GeoScopeType.AroundDistrict:
                    DistrictInfoEntity di = GetDistrictInfo(new List<int> { districtid }).FirstOrDefault();
                    htl = HotelService.QueryInterestAndHotelCount(0, di.lat, di.lon);
                    break; 
                case EnumHelper.GeoScopeType.AroundCoordinate:
                    htl = HotelService.QueryInterestAndHotelCount(0, userLat, userLng);
                    break;
            }
            return  htl ;

        }

        /// <summary>
        /// 获取主题数据
        /// </summary>
        /// <param name="districtid"></param>
        /// <param name="userLat"></param>
        /// <param name="userLng"></param>
        /// <param name="geoScopeType"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        public static ThemeModel QueryTheme(int districtid, float userLat, float userLng, int geoScopeType, int distance = 300000)
        {
            InterestQueryEntity htl = GetInterestAndHotelCount(districtid, userLat, userLng, geoScopeType);
            List<InterestEntity> interestList = htl.interestList.Where(i => i.Type == 1 && i.ID < 100003).ToList();//超出100003的主题不显示 需要绘制主题log和配色 06-01 wwb

            ThemeModel model = new ThemeModel();
            model.districtid = districtid;
            model.GLat = userLat;
            model.GLon = userLng;
            model.Name = "";

            Advertise AD = ADAdapter.GetOnlineAdv(districtid, geoScopeType,userLat,userLng);

            int ADCount = AD.ADList.Count;
            int themeCount = interestList.Count;

            List<int> ColSpanSeq = new List<int> { 2, 1, 1, 2, 1, 1, 1 };
            int curColSpanIndex = 0;
            int curADIndex = 0;
            int curThemeIndex = 0;

            for (int i = 0; i < ADCount*3 + themeCount; i++)
            {
                if (i % 3 == 0 && curADIndex < ADCount)
                {
                    RowItem r = new RowItem();
                    Advertise mAD = new Advertise { Ratio = AD.Ratio, ADList = new List<ADItem>() };
                    mAD.ADList.Add(AD.ADList[curADIndex]);
                    curADIndex++;
                    r.AD = mAD;
                    model.RowList.Add(r);
                }

                if (i % 3 != 0 && curThemeIndex < themeCount)
                {
                    RowItem r = new RowItem();
                 
                    for (int col = 0; col < 3; )
                    {

                        ThemeItem theme = new ThemeItem();
                        theme.colspan = ColSpanSeq[curColSpanIndex];
                        theme.theme = interestList[curThemeIndex];
                        col += theme.colspan;
                        curThemeIndex++;
                        curColSpanIndex++;
                        curColSpanIndex = curColSpanIndex% ColSpanSeq.Count;

                        r.themes.Add(theme);

                        if (curThemeIndex == themeCount)
                        {
                            break;
                        }
                    }

                    int usedColSpan = r.themes.Sum(t => t.colspan);
                    if (usedColSpan == 1)
                    {
                        r.themes[0].colspan = 3;
                    }
                    else if (usedColSpan == 2)
                    {
                        if (r.themes.Count == 1)
                        {
                            r.themes[0].colspan = 3;
                        }
                        else
                        {
                            r.themes[0].colspan = 2;
                        }
                    }
                    model.RowList.Add(r);
                }
            }

            return model;
        }

        public static InterestHotelResult GetInterest4AD(int districtid, float userLat, float userLng, int geoScopeType, bool onlySelected)
        {
            string cachekey = string.Format("Interest4AD:{0}:{1}:{2}", districtid, geoScopeType, onlySelected ? 1 : 0);
            if (geoScopeType == (int)EnumHelper.GeoScopeType.AroundCoordinate)
                cachekey = string.Format("Interest4AD:{0}:{1}:{2}", (int)(userLat * 100), (int)(userLng * 100), onlySelected ? 1 : 0);

            return LocalCache30Min.GetData<InterestHotelResult>(cachekey, () =>
            {
                return GenInterest4AD(districtid, userLat, userLng, geoScopeType, onlySelected);
            });
        }

        /// <summary>
        /// ToDo数据库发布后 换上新的写法
        /// </summary>
        /// <param name="districtid"></param>
        /// <param name="userLat"></param>
        /// <param name="userLng"></param>
        /// <param name="geoScopeType"></param>
        /// <param name="onlySelected"></param>
        /// <returns></returns>
        public static InterestHotelResult GenInterest4AD(int districtid, float userLat, float userLng, int geoScopeType, bool onlySelected)
        {
            List<InterestEntity> ieList = new List<InterestEntity>();
            int totalHotelCount = 0;
            switch ((EnumHelper.GeoScopeType)geoScopeType)
            {
                case EnumHelper.GeoScopeType.District:
                    //ieList = HotelService.QueryInterest4AD(districtid, 0, 0);
                    ieList = onlySelected ? HotelService.QueryInterest4ADSelected(districtid, 0, 0)
                        : HotelService.QueryInterest4AD(districtid, 0, 0);
                    totalHotelCount = onlySelected ? HotelService.QueryInterestHotelCountSelected(districtid, 0, 0) : HotelService.QueryInterestHotelCount(districtid, 0, 0);
                    break;
                case EnumHelper.GeoScopeType.AroundDistrict:
                    DistrictInfoEntity di = GetDistrictInfo(new List<int> { districtid }).FirstOrDefault();
                    //ieList = HotelService.QueryInterest4AD(0, di.lat, di.lon);
                    ieList = onlySelected ? HotelService.QueryInterest4ADSelected(0, di.lat, di.lon)
                        : HotelService.QueryInterest4AD(0, di.lat, di.lon);
                    totalHotelCount = onlySelected ? HotelService.QueryInterestHotelCountSelected(0, di.lat, di.lon) : HotelService.QueryInterestHotelCount(0, di.lat, di.lon);
                    break;
                case EnumHelper.GeoScopeType.AroundCoordinate:
                    //ieList = HotelService.QueryInterest4AD(0, userLat, userLng);
                    ieList = onlySelected ? HotelService.QueryInterest4ADSelected(0, userLat, userLng)
                        : HotelService.QueryInterest4AD(0, userLat, userLng);
                    totalHotelCount = onlySelected ? HotelService.QueryInterestHotelCountSelected(0, userLat, userLng) : HotelService.QueryInterestHotelCount(0, userLat, userLng);
                    break;
            }

            ieList.ForEach(_ => { if (!string.IsNullOrWhiteSpace(_.ImageUrl)) { _.ImageUrl = _.ImageUrl.TrimEnd("s".ToCharArray()); } });//从290x290s变成290x290

            return new InterestHotelResult()
            {
                interests = ieList,
                totalHotelCount = totalHotelCount
            };
        }

        /// <summary>
        ///  景区数量
        /// </summary>
        /// <param name="districtid"></param>
        /// <param name="userLat"></param>
        /// <param name="userLng"></param>
        /// <param name="geoScopeType"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        public static AttractionModel QueryAttraction(int districtid, float userLat, float userLng, int geoScopeType, int distance = 300000)
        {

            InterestQueryEntity htl = GetInterestAndHotelCount(districtid, userLat, userLng, geoScopeType);

            AttractionModel model = new AttractionModel();
            model.SightInterestList = htl.hotelCount > 0 ? htl.interestList.Where(h => h.Type == 2).ToList() : new List<InterestEntity>();
           
            var aclist = from a in GetAttractionCategoryRel()
                         join s in model.SightInterestList
                           on a.InterestID equals s.ID
                         select a;

            model.SightCategoryList = (from a in aclist
                                       group a by a.ID into aGroup
                                       select new SightCategory()
                                       {
                                           ID = aGroup.Key,
                                           Name = aGroup.First().Name,
                                           InterestID = aGroup.Select(s => s.InterestID).ToList()
                                       }).ToList();


            return model;
        }
        
        public static InterestModel2 GenQueryInterest30(int districtid, float userLat, float userLng, int geoScopeType, int distance = 300000)
        {

            InterestQueryEntity htl = GetInterestAndHotelCount(districtid, userLat, userLng, geoScopeType);


            InterestModel2 model = new InterestModel2();
            model.districtid = districtid;
            model.GLat = userLat;
            model.GLon = userLng;
            model.Name = "";
            model.ThemeInterestList = htl.hotelCount >0?  htl.interestList.Where(h => h.Type == 1).ToList() : new List<InterestEntity>() ;
            model.SightInterestList = htl.hotelCount > 0 ?  htl.interestList.Where(h => h.Type == 2).ToList() : new List<InterestEntity>();
            model.HotInterestList = new List<InterestEntity>();
            model.TotalHotelNum = htl.hotelCount;


            model.AD = ADAdapter.GetOnlineAdv(districtid, geoScopeType, userLat, userLng);


            //准备特惠等广告内容
            model.HotInterestList = ADAdapter.GenOnlineAdvOld();
            //List<CommDictEntity> ld = HotelService.GetCommDictList(10001);
            //foreach (CommDictEntity dic in ld)
            //{
            //    string[] l = dic.Descript.Split('|');
            //    InterestEntity hi = new InterestEntity();
            //    hi.Name = dic.DicValue;
            //    hi.ID = int.Parse(l[0]);
            //    hi.ImageUrl = l[2];
            //    hi.Type = int.Parse(l[1]);
            //    hi.HotelList = "";
            //    hi.HotelCount = 0;
            //    if (hi.Type == 3) //特惠精选
            //    {


            //        List<string> ipids = GetSpecialDealInterestPlaceIDs(districtid, userLat, userLng, geoScopeType);
            //        hi.InterestPlaceIDs = "";
            //        hi.DistrictID = districtid;
            //        hi.GLat = userLat;
            //        hi.GLon = userLng;

            //        HotelSearchParas pars = new HotelSearchParas();

            //        pars.Interest = hi.ID;
            //        pars.InterestPlace = ipids.Select(i => int.Parse(i)).ToArray();
            //        pars.NeedHotelID = false;
            //        pars.NeedFilterCol = false;
            //        pars.StartIndex = 0;
            //        pars.ReturnCount = 1;
            //        pars.CheckInDate = DateTime.Now;
            //        pars.CheckOutDate = DateTime.Now;

            //        QueryHotelResult2 qr = HotelService.QueryHotel2(pars, 0);
            //        hi.HotelCount = qr.TotalCount;

            //        if (hi.HotelCount > 0)
            //        {
            //            model.HotInterestList.Add(hi);
            //        }

            //    }
            //    else if (hi.Type == 4) //限时特惠，只投放酒店周围
            //    {
            //        int hotelid = int.Parse(l[4]);
            //        hi.InterestPlaceIDs = l[3];

            //        HotelItem h = HotelService.GetHotel(hotelid);

            //        if (userLat > 0)//如果是周边
            //        {
            //            //如果距离在周边，则显示
            //            double hdistance = Common.CommMethods.CalcDistance(h.GLat, h.GLon, userLat, userLng);
            //            if (hdistance < distance)
            //            {
            //                model.HotInterestList.Add(hi);
            //            }
            //        }
            //        else
            //        {
            //            //如果同目的地则显示
            //            if (h.DistrictId == districtid)
            //            { model.HotInterestList.Add(hi); }
            //        }

            //    }
            //    else if (hi.Type == 5) //限时特惠，投放所有目的地
            //    {
            //        hi.InterestPlaceIDs = l[3];//连接地址
            //        model.HotInterestList.Add(hi);
            //    }

            //}


       

            var aclist = from a in GetAttractionCategoryRel()
                         join s in model.SightInterestList
                           on a.InterestID equals s.ID
                         select a;

            model.SightCategoryList = (from a in aclist
                                       group a by a.ID into aGroup
                                       select new SightCategory()
                                       {
                                           ID = aGroup.Key,
                                           Name = aGroup.First().Name,
                                           InterestID = aGroup.Select(s => s.InterestID).ToList()
                                       }).ToList();


            return model;
        }

        /// <summary>
        /// 获取特惠精选的INTEREST PLACE IDs
        /// </summary>
        /// <param name="districtid"></param>
        /// <param name="lat"></param>
        /// <param name="lng"></param>
        /// <returns></returns>
        public static List<string> GetSpecialDealInterestPlaceIDs(int districtid, float userLat, float userLng, int geoScopeType)
        {

            List<string> ipids = new List<string>();
            InterestQueryEntity htl = GetInterestAndHotelCount(districtid, userLat, userLng, geoScopeType);

            if (htl.hotelCount > 0)
            {
                ipids = string.Join(",", htl.interestList.Select(h => h.InterestPlaceIDs).ToList()).Split(',').Distinct().Where(d => d.Length > 0).ToList();

                if (ipids.Count > 50)
                {
                    for (int i = ipids.Count - 1; i > 0; i--)
                    {
                        if (!pHotels.Contains(int.Parse(ipids[i])))
                        {
                            ipids.RemoveAt(i);
                        }
                    }
                }
            }

            return  ipids;
        }

        public static List<AttractionCategoryRelEntity> GetAttractionCategoryRel()
        {
            return LocalCache.GetData<List<AttractionCategoryRelEntity>>("AttractionCategoryRel", () =>
            {
                return HotelService.GetAttractionCategoryRel();
            });

        }

        public static string GetHotelThemeName( int themeID)
        {
            if (!dicHotelTheme.ContainsKey(themeID))
            {
                 List<HotelThemeEntity> lt = GetAllHotelTheme();
                 foreach (HotelThemeEntity t in lt)
                 {
                     if (!dicHotelTheme.ContainsKey(t.ID))
                         dicHotelTheme.Add(t.ID, t.Name);
                 }
                 if (!dicHotelTheme.ContainsKey(themeID))
                 {
                     dicHotelTheme.Add(themeID, "...");
                 }
            }

            return dicHotelTheme[themeID];
        }

        public static string GetInterestName(int interestID)
        {
            try
            {
                if (!dicInterest.ContainsKey(interestID))
                {
                    List<InterestEntity2> lt = HotelService.GetAllInterestList();
                    foreach (InterestEntity2 t in lt)
                    {
                        if (!dicInterest.ContainsKey(t.ID))
                            dicInterest.Add(t.ID, t.Name);
                    }
                    if (!dicInterest.ContainsKey(interestID))
                    {
                        dicInterest.Add(interestID, "...");
                    }
                }

                return dicInterest[interestID];
            }
            catch(Exception err)
            {
                HJDAPI.Controllers.Common.Log.WriteLog("GetInterestName:" + interestID.ToString() + ":" + err.Message + err.StackTrace);
            }
            return "..";
        }

        public static List<AttractionEntity> GetAttractionByCity(int districtID)
        {
           return  DestService.GetAttractionByCity(districtID);
        }

        public static WapAttractionResult AttractionQuery( AttractionQueryParam p)
        {
            return DestService.QueryAttraction(p);
        }

        public static List<AttractionEntity> GetAllAttraction()
        {
            return DestService.GetAllAttraction();
        }
        
        public static SimpleCityInfo GetCityInfoByName(string cityName, float lat, float lon)
        {
            return DestService.GetCityInfoByName(cityName, lat, lon);
        }
        public static List<SimpleCityInfo> GetAroundCityInfo(float Lat, float Lon)
        {
            return DestService.GetAroundCityInfoByLatLonAndHotelCount(Lat, Lon);
        }

        internal static AttractionEntity GetOneAttraction(int attractionID)
        {
             List<AttractionEntity> allAtt = GetAllAttraction();
             return allAtt.Where(a => a.ID == attractionID).FirstOrDefault();
        }

        public static List<HJD.DestServices.Contract.CityEntity> GetCityList()
        {
            return DestService.GetCityList();
        }

        private static List<int> pHotels
        {
            get
            {
                return LocalCache.GetData<List<int>>("PackagedInterestPlaces", () =>
                    {
                        return HotelService.GetPackagedInterestPlace();
                    });
            }
        }

        #region 酒店攻略相关查询
        /// <summary>
        /// 缓存 地区攻略数据(此地区聚合了下级区域的所有数据)
        /// </summary>
        /// <param name="districtId"></param>
        /// <returns></returns>
        public static List<DistrictZoneEntity> GetStrategyDistrictZoneList(int districtId, bool onlyPublished = true)
        {
            List<DistrictZoneEntity> allData = LocalCache30Min.GetData<List<DistrictZoneEntity>>("StrategyDistrict_" + districtId, () =>
            {
                return GetStrategyDistrictZoneListNoCache(districtId, -1);//出某个地区所有的攻略数据 包括发布和未发布的
            });
            return onlyPublished ? allData.FindAll(_ => _.State == 1).ToList() : allData;
        }

        /// <summary>
        /// 非缓存 地区攻略数据
        /// </summary>
        /// <param name="districtId"></param>
        /// <returns></returns>
        public static List<DistrictZoneEntity> GetStrategyDistrictZoneListNoCache(int districtId, int state = -1)
        {
            var list = DestService.GetStrategyDistrictZoneList(districtId, state) ?? new List<DistrictZoneEntity>();
            foreach (var item in list)
            {
                if (!string.IsNullOrWhiteSpace(item.PicUrl)) item.PicUrl = PhotoAdapter.GenHotelPicUrl(item.PicUrl, Enums.AppPhotoSize.appview);
            }

            return list;
        }

        /// <summary>
        /// 获取多个地区攻略基本信息
        /// </summary>
        /// <param name="districtIds"></param>
        /// <param name="type"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public static IEnumerable<DistrictZoneEntity> GetManyDistrictZoneEntity(IEnumerable<int> districtIds, int type, int state)
        {
            if(districtIds == null || districtIds.Count() == 0){
                return new List<DistrictZoneEntity>();
            }
            return DestService.GetManyDistrictZoneEntity(districtIds, type == 0 ? 1 : type, state);
        }

        #endregion
    }
}