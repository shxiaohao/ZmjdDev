using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HJD.HotelServices.Contracts;
using HJDAPI.Common.Helpers;
using HJDAPI.Controllers;
using HJDAPI.Models;
using Newtonsoft.Json;
using HJDAPI.Controllers.Adapter;
using HJD.HotelManagementCenter.Domain;
using HJD.OtaCrawlerService.Contract.Ctrip.Hotel;
using HJD.OtaCrawlerService.Contract.Hotel;
using HJD.CommentService.Contract;
using HJDAPI.Models.ResponseModel;
using HJDAPI.Models.RequestParams;

namespace HJDAPI.APIProxy
{
    public class Hotel : BaseProxy
    {

        public GetTravelPersonResponseModel AddTravelPerson(GetTravelPersonParams param)
        {
            string url = APISiteUrl + "api/hotel/AddTravelPerson";
            //GetTravelPersonParams TravelParams = new GetTravelPersonParams();
            //TravelParams.travelPerson = param.travelPerson;
            CookieContainer cc = new CookieContainer();

            string json = HttpRequestHelper.PostJson(url, param, ref cc);
            return JsonConvert.DeserializeObject<GetTravelPersonResponseModel>(json);

        }

        public GetTravelPersonResponseModel UpdateTravelPerson(GetTravelPersonParams param)
        {
            string url = APISiteUrl + "api/hotel/UpdateTravelPerson";
            //GetTravelPersonParams TravelParams = new GetTravelPersonParams();
            //TravelParams.travelPerson = param.travelPerson;
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.PostJson(url, param, ref cc);
            return JsonConvert.DeserializeObject<GetTravelPersonResponseModel>(json);
        }

        public GetTravelPersonByUserIdResponseModel GetTravelPersonByUserId(GetTravelPersonByUserIdParams param)
        {
            string url = APISiteUrl + "api/hotel/GetTravelPersonByUserId";
            //GetTravelPersonByUserIdParams GetTravelList = new GetTravelPersonByUserIdParams();
            //GetTravelList.userID = param.userID;
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.PostJson(url, param, ref cc);
            return JsonConvert.DeserializeObject<GetTravelPersonByUserIdResponseModel>(json);
        }

        public GetTravelPersonResponseModel DeleteTravelPerson(DeleteTravePersonById param)
        {
            string url = APISiteUrl + "api/hotel/GetTravelPersonByUserId";
            //DeleteTravePersonById DelTravel = new DeleteTravePersonById();
            //DelTravel.ID = param.ID;
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.PostJson(url, param, ref cc);
            return JsonConvert.DeserializeObject<GetTravelPersonResponseModel>(json);
        }


        public static CityList GetZMJDCityData2()
        {
            if (IsProductEvn)
                return HotelAdapter.GetZMJDCityData2();
            else
            {
                string url = APISiteUrl + "api/hotel/GetZMJDCityData2";
                //string postDataStr = string.Format("districtid={0}"
                //  , districtid);

                CookieContainer cc = new CookieContainer();

                string json = HttpRequestHelper.Get(url, null, ref cc);
                return JsonConvert.DeserializeObject<CityList>(json);
            }
        }

        public static List<HJD.HotelServices.Contracts.ChannelInfoEntity> GetAllChannelInfoList()
        {
            if (IsProductEvn)
                return HotelAdapter.GetAllChannelInfoList();
            else
            {
                string url = APISiteUrl + "api/hotel/GetAllChannelInfoList";
                //string postDataStr = string.Format("districtid={0}"
                //  , districtid);

                CookieContainer cc = new CookieContainer();

                string json = HttpRequestHelper.Get(url, null, ref cc);
                return JsonConvert.DeserializeObject<List<HJD.HotelServices.Contracts.ChannelInfoEntity>>(json);
            }
        }

        public static List<SpecialDealPackageEntity> GetSpecialDealPackage()
        {
            if (IsProductEvn)
                return PackageAdapter.GetSpecialDealPackage();
            else
            {
                string url = APISiteUrl + "api/hotel/GetSpecialDealPackage";
                string postDataStr = "";

                CookieContainer cc = new CookieContainer();

                string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
                return JsonConvert.DeserializeObject<List<SpecialDealPackageEntity>>(json);
            }
        }

        public static List<CanSaleHotelInfoEntity> GetAllCanSellPackage(DateTime startDate, DateTime endDate, string tag = "")
        {
            if (IsProductEvn)
                return PackageAdapter.GetAllCanSellPackage(startDate, endDate, tag);
            else
            {
                string url = APISiteUrl + "api/hotel/GetAllCanSellPackage";
                string postDataStr = string.Format("startDate={0}&endDate={1}&tag={2}"
                   , startDate.Date.ToString("yyyy-MM-dd"), endDate.Date.ToString("yyyy-MM-dd"), tag);

                CookieContainer cc = new CookieContainer();

                string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
                return JsonConvert.DeserializeObject<List<CanSaleHotelInfoEntity>>(json);
            }
        }

        public static List<QuickSearchSuggestItem> SuggestHotel(string keyword, int count)
        {

            if (IsProductEvn)
                return HotelAdapter.SuggestHotel(keyword, count);
            else
            {
                string url = APISiteUrl + "api/hotel/SuggestHotel";
                string postDataStr = string.Format("keyword={0}&count={1}"
                  , keyword, count);

                CookieContainer cc = new CookieContainer();

                string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
                return JsonConvert.DeserializeObject<List<QuickSearchSuggestItem>>(json);
            }
        }

        // public static HomePageData GetHomePageData(int districtid = 2)
        // {
        //    if (IsProductEvn){
        //        return  HotelAdapter.GetHomePageData(districtid);
        //    }
        //    else
        //    {
        //        string url = APISiteUrl + "api/hotel/GetHomePageData";
        //        string postDataStr = string.Format("districtid={0}"
        //          , districtid);

        //        CookieContainer cc = new CookieContainer();

        //        string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
        //        return JsonConvert.DeserializeObject<HomePageData>(json);
        //    }
        //}

        public static HomePageData GetHomePageData(int districtid = 2, int geoScopeType = 2)
        {
            if (IsProductEvn)
            {
                return HotelAdapter.GetHomePageData(districtid, geoScopeType);
            }
            else
            {
                string url = APISiteUrl + "api/hotel/GetHomePageData";
                string postDataStr = string.Format("districtid={0}&geoScopeType={1}"
                  , districtid, geoScopeType);

                CookieContainer cc = new CookieContainer();

                string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
                return JsonConvert.DeserializeObject<HomePageData>(json);
            }
        }
        public static HotelItem GetHotelBasicInfo(int HotelID)
        {
            if (IsProductEvn)
                return new HotelAdapter().GetHotelBasicInfo(HotelID);
            else
            {
                string url = APISiteUrl + "api/hotel/GetHotelBasicInfo";
                string postDataStr = string.Format("HotelID={0}"
                  , HotelID);

                CookieContainer cc = new CookieContainer();

                string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
                return JsonConvert.DeserializeObject<HotelItem>(json);
            }
        }
        public static HotelItem3 Get3(int id, string checkIn, string checkOut, string sType, int interestid)
        {
            if (IsProductEvn)
                return new HotelAdapter().Get3(id, checkIn, checkOut, sType, interestid);
            else
            {
                string url = APISiteUrl + "api/hotel/Get3";
                string postDataStr = string.Format("id={0}&checkIn={1}&checkOut={2}&sType={3}&interestid={4}"
                  , id, checkIn, checkOut, sType, interestid);

                CookieContainer cc = new CookieContainer();

                string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
                return JsonConvert.DeserializeObject<HotelItem3>(json);
            }
        }

        //http://api.zmjiudian.com/api/hotel/searchThemeHotel?sort=2&order=0&start=0&count=5&hoteltheme=1&lat=30&lng=120
        //未定位：
        //http://api.zmjiudian.com/api/hotel/searchThemeHotel?sort=0&order=0&start=0&count=5&hoteltheme=1

        public static WapThemeHotelsResult searchThemeHotel(HotelListQueryParam p)
        {
            if (IsProductEvn)
                return new HotelAdapter().SearchThemeHotel(p);
            else
            {
                string url = APISiteUrl + "api/hotel/searchThemeHotel";
                string postDataStr = string.Format("districtid={0}&lat={1}&lng={2}&distance={3}&hotelid={4}&type={5}&sort={6}&order={7}&start={8}&count={9}&checkin={10}&checkout={11}&nLat={12}&nLng={13}&sLat={14}&sLng={15}&valued={16}&tag={17}&minPrice={18}&maxPrice={19}&location={20}&zone={21}&brand={22}&attraction={23}&featured={24}&hoteltheme={25}"
                  , p.districtid
                    , p.lat
                    , p.lng
                    , p.distance
                    , p.hotelid
                    , p.type
                    , p.sort
                    , p.order
                    , p.start
                    , p.count
                    , p.checkin
                    , p.checkout
                    , p.nLat
                    , p.nLng
                    , p.sLat
                    , p.sLng
                    , p.valued
                    , p.tag
                    , p.minPrice
                    , p.maxPrice
                    , p.location
                    , p.zone
                    , p.brand
                    , p.attraction
                    , p.featured
                    , p.hotelTheme
                    );

                CookieContainer cc = new CookieContainer();

                string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
                return JsonConvert.DeserializeObject<WapThemeHotelsResult>(json);
            }
        }

        public static WapInterestHotelsResult2 SearchInterestHotel20(HotelListQueryParam p)
        {
            if (IsProductEvn)
                return new HotelAdapter().SearchInterestHotel20(p);
            else
            {
                string url = APISiteUrl + "api/hotel/SearchInterestHotel20";
                string postDataStr = string.Format("districtid={0}&lat={1}&lng={2}&distance={3}&hotelid={4}&type={5}&sort={6}&order={7}&start={8}&count={9}&checkin={10}&checkout={11}&nLat={12}&nLng={13}&sLat={14}&sLng={15}&valued={16}&tag={17}&minPrice={18}&maxPrice={19}&location={20}&zone={21}&brand={22}&attraction={23}&featured={24}"
                  , p.districtid
 , p.lat
 , p.lng
 , p.distance
 , p.hotelid
 , p.type
 , p.sort
 , p.order
 , p.start
 , p.count
 , p.checkin
 , p.checkout
 , p.nLat
 , p.nLng
 , p.sLat
 , p.sLng
 , p.valued
 , p.tag
 , p.minPrice
 , p.maxPrice
 , p.location
 , p.zone
 , p.brand
 , p.attraction
 , p.featured);

                CookieContainer cc = new CookieContainer();

                string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
                return JsonConvert.DeserializeObject<WapInterestHotelsResult2>(json);
            }
        }

        public static InterestHotelsResult QueryInterestHotel(HotelListQueryParam p)
        {
            if (p.geoScopeType == 0) p.geoScopeType = 2;//查看目的周边酒店
            if (IsProductEvn)
                return HotelAdapter.QueryInterestHotel(p);
            else
            {
                string url = APISiteUrl + "api/hotel/QueryInterestHotel";
                string postDataStr = string.Format("districtid={0}&lat={1}&lng={2}&distance={3}&hotelid={4}&type={5}&sort={6}&order={7}&start={8}&count={9}&checkin={10}&checkout={11}&nLat={12}&nLng={13}&sLat={14}&sLng={15}&valued={16}&tag={17}&minPrice={18}&maxPrice={19}&location={20}&zone={21}&brand={22}&attraction={23}&featured={24}&star={25}&geoScopeType={26}&fromDistance={27}&Interest={28}"
                  , p.districtid
 , p.lat
 , p.lng
 , p.distance
 , p.hotelid
 , p.type
 , p.sort
 , p.order
 , p.start
 , p.count
 , p.checkin
 , p.checkout
 , p.nLat
 , p.nLng
 , p.sLat
 , p.sLng
 , p.valued
 , p.tag
 , p.minPrice
 , p.maxPrice
 , p.location
 , p.zone
 , p.brand
 , p.attraction
 , p.featured
 , p.star
 , p.geoScopeType
 , p.fromDistance
 , p.Interest);

                CookieContainer cc = new CookieContainer();

                string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
                return JsonConvert.DeserializeObject<InterestHotelsResult>(json);
            }
        }

        public static WapHotelsResult Search(HotelListQueryParam p)
        {
            if (IsProductEvn)
                return new HotelAdapter().Search(p);
            else
            {
                string url = APISiteUrl + "api/hotel/search";
                string postDataStr = string.Format("districtid={0}&lat={1}&lng={2}&distance={3}&hotelid={4}&type={5}&sort={6}&order={7}&start={8}&count={9}&checkin={10}&checkout={11}&nLat={12}&nLng={13}&sLat={14}&sLng={15}&valued={16}&tag={17}&minPrice={18}&maxPrice={19}&location={20}&zone={21}&brand={22}&attraction={23}&featured={24}"
                  , p.districtid
, p.lat
, p.lng
, p.distance
, p.hotelid
, p.type
, p.sort
, p.order
, p.start
, p.count
, p.checkin
, p.checkout
, p.nLat
, p.nLng
, p.sLat
, p.sLng
, p.valued
, p.tag
, p.minPrice
, p.maxPrice
, p.location
, p.zone
, p.brand
, p.attraction
, p.featured);

                CookieContainer cc = new CookieContainer();

                string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
                return JsonConvert.DeserializeObject<WapHotelsResult>(json);
            }
        }

        public HJDAPI.Controllers.HotelAdapter.HotelItemResult Get(int id, string checkIn, string checkOut, string sType)
        {


            if (IsProductEvn)
                return new HotelAdapter().Get(id, checkIn, checkOut, sType);

            else
            {
                string url = APISiteUrl + "api/hotel/get";
                string postDataStr = string.Format("id={0}&checkin={1}&checkout={2}&sType={3}"
                  , id, checkIn, checkOut, sType);

                CookieContainer cc = new CookieContainer();
                string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
                return JsonConvert.DeserializeObject<HJDAPI.Controllers.HotelAdapter.HotelItemResult>(json);
            }
        }

        public HJDAPI.Controllers.HotelAdapter.HotelItem2Result Get2(int id, string checkIn, string checkOut, string sType, int themeid)
        {


            if (IsProductEvn)
                return new HotelAdapter().Get2(id, checkIn, checkOut, sType, themeid);

            else
            {
                string url = APISiteUrl + "api/hotel/get2";
                string postDataStr = string.Format("id={0}&checkin={1}&checkout={2}&sType={3}&themeid={4}"
                  , id, checkIn, checkOut, sType, themeid);

                CookieContainer cc = new CookieContainer();
                string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
                return JsonConvert.DeserializeObject<HJDAPI.Controllers.HotelAdapter.HotelItem2Result>(json);
            }
        }

        public ReviewResult GetComments(ArguHotelReview p)
        {
            if (IsProductEvn)
                return new HotelAdapter().GetHotelReviews(p);
            else
            {
                string url = APISiteUrl + "api/hotel/GetComments";
                string postDataStr = string.Format("hotel={0}&start={1}&count={2}&ratingtype={3}&featured={4}&TFTType={5}&TFTID={6}"
                  , p.Hotel, p.Start, p.Count, p.RatingType, p.Featured, p.TFTType, p.TFTID);

                CookieContainer cc = new CookieContainer();
                string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
                return JsonConvert.DeserializeObject<ReviewResult>(json);
            }
        }

        public ReviewResult2 GetComments(ReviewQueryParam p)
        {
            if (IsProductEvn)
                return new HotelAdapter().GetHotelReviews2(p);
            else
            {
                string url = APISiteUrl + "api/hotel/GetComments2";
                string postDataStr = string.Format("hotel={0}&start={1}&count={2}&ratingtype={3}&interestid={4}&TFTType={5}&TFTID={6}"
                  , p.Hotel, p.Start, p.Count, p.RatingType, p.InterestID, p.TFTType, p.TFTID);

                CookieContainer cc = new CookieContainer();
                string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
                return JsonConvert.DeserializeObject<ReviewResult2>(json);
            }
        }

        public HotelPhotosEntity GetHotelPhotos(int HotelID)
        {
            if (IsProductEvn)
                return HotelAdapter.GetHotelPhotos(HotelID, 0);
            else
            {
                string url = APISiteUrl + "api/hotel/gethotelphotos";
                string postDataStr = string.Format("hotelid={0}"
                  , HotelID);

                CookieContainer cc = new CookieContainer();
                string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
                return JsonConvert.DeserializeObject<HotelPhotosEntity>(json);
            }
        }

        public WapInterestHotelsResult3 SearchInterestHotel40(HotelListQueryParam p)
        {
            if (IsProductEvn)
                return HotelAdapter.SearchInterestHotel40(p);
            else
            {
                string url = APISiteUrl + "api/hotel/SearchInterestHotel40";
                string postDataStr = string.Format("districtid={0}&lat={1}&lng={2}&distance={3}&hotelid={4}&type={5}&sort={6}&order={7}&start={8}&count={9}&checkin={10}&checkout={11}&nLat={12}&nLng={13}&sLat={14}&sLng={15}&valued={16}&tag={17}&minPrice={18}&maxPrice={19}&location={20}&zone={21}&brand={22}&attraction={23}&featured={24}&Interest={25}&geoScopeType={26}"
                , p.districtid
                , p.lat
                , p.lng
                , p.distance
                , p.hotelid
                , p.type
                , p.sort
                , p.order
                , p.start
                , p.count
                , p.checkin
                , p.checkout
                , p.nLat
                , p.nLng
                , p.sLat
                , p.sLng
                , p.valued
                , p.tag
                , p.minPrice
                , p.maxPrice
                , p.location
                , p.zone
                , p.brand
                , p.attraction
                , p.featured
                , p.Interest
                , p.geoScopeType);

                CookieContainer cc = new CookieContainer();

                string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
                return JsonConvert.DeserializeObject<WapInterestHotelsResult3>(json);
            }
        }

        public SearchHotelListResult SearchHotelList20(HotelListQueryParam20 p)
        {
            string url = APISiteUrl + "api/Hotel/SearchHotelList20";
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.PostJson(url, p, ref cc);
            return JsonConvert.DeserializeObject<SearchHotelListResult>(json);
        }

        public SearchHotelListResult SearchHotelList30(HotelListQueryParam20 p)
        {
            string url = APISiteUrl + "api/Hotel/SearchHotelList30";
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.PostJson(url, p, ref cc);
            return JsonConvert.DeserializeObject<SearchHotelListResult>(json);
        }

        public HotelItem4 Get40(int id, string checkIn, string checkOut, string sType, int interestid)
        {
            if (IsProductEvn)
            {
                return new HotelAdapter().Get40(id, checkIn, checkOut, sType, interestid);
            }
            else
            {
                string url = APISiteUrl + "api/hotel/Get40";
                string postDataStr = string.Format("id={0}&checkIn={1}&checkOut={2}&sType={3}&interestid={4}"
                  , id, checkIn, checkOut, sType, interestid);

                CookieContainer cc = new CookieContainer();

                string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
                return JsonConvert.DeserializeObject<HotelItem4>(json);
            }
        }

        public HotelItem5 Get50(int id, string checkIn, string checkOut, string sType, int interestid, long userID = 0, string appVer = "", int pid = 0)
        {
            string url = APISiteUrl + "api/hotel/Get50";
            string postDataStr = string.Format("id={0}&checkIn={1}&checkOut={2}&sType={3}&interestid={4}&userID={5}&appVer={6}&pid={7}"
              , id, checkIn, checkOut, sType, interestid, userID, appVer, pid);

            CookieContainer cc = new CookieContainer();

            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<HotelItem5>(json);
        }

        public HotelItem6 GetHotelDetail(int id, string checkIn, string checkOut, string sType, int interestid, long userID = 0, string machineNo = "", int pid = 0)
        {
            string url = APISiteUrl + "api/hotel/GetHotelDetail";
            string postDataStr = string.Format("id={0}&checkIn={1}&checkOut={2}&sType={3}&interestid={4}&userID={5}&machineNo={6}&pid={7}"
              , id, checkIn, checkOut, sType, interestid, userID, machineNo, pid);

            CookieContainer cc = new CookieContainer();

            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<HotelItem6>(json);
        }
        public HotelItem6 GetSimpleHotelDetail(int id, string checkIn, string checkOut, string sType, int interestid, long userID = 0, string machineNo = "", int pid = 0)
        {
            string url = APISiteUrl + "api/hotel/GetSimpleHotelDetail";
            string postDataStr = string.Format("id={0}&checkIn={1}&checkOut={2}&sType={3}&interestid={4}&userID={5}&machineNo={6}&pid={7}"
              , id, checkIn, checkOut, sType, interestid, userID, machineNo, pid);

            CookieContainer cc = new CookieContainer();

            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<HotelItem6>(json);
        }
        public ReviewResult40 GetComments40(ReviewQueryParam p)
        {
            if (IsProductEvn)
            {
                return new HotelAdapter().GetHotelReviews40(p);
            }
            else
            {
                string url = APISiteUrl + "api/hotel/GetComments40";
                string postDataStr = string.Format("Hotel={0}&Start={1}&Count={2}&RatingType={3}&InterestID={4}&TFTType={5}&TFTID={6}"
                  , p.Hotel, p.Start, p.Count, p.RatingType, p.InterestID, p.TFTType, p.TFTID);

                CookieContainer cc = new CookieContainer();

                string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
                return JsonConvert.DeserializeObject<ReviewResult40>(json);
            }
        }

        public ReviewResult40 GetComments50(ReviewQueryParam p)
        {
            if (IsProductEvn)
            {
                return new HotelAdapter().GetHotelReviews50(p);
            }
            else
            {
                string url = APISiteUrl + "api/hotel/GetComments50";
                string postDataStr = string.Format("Hotel={0}&Start={1}&Count={2}&RatingType={3}&InterestID={4}&TFTType={5}&TFTID={6}"
                  , p.Hotel, p.Start, p.Count, p.RatingType, p.InterestID, p.TFTType, p.TFTID);

                CookieContainer cc = new CookieContainer();

                string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
                return JsonConvert.DeserializeObject<ReviewResult40>(json);
            }
        }

        public List<TopNPackageItem> GetTop20Package()
        {
            if (IsProductEvn)
            {
                return HotelAdapter.GetTop20Package();
            }
            else
            {
                string url = APISiteUrl + "api/hotel/GetTop20Package";
                CookieContainer cc = new CookieContainer();
                string json = HttpRequestHelper.Get(url, "", ref cc);
                return JsonConvert.DeserializeObject<List<TopNPackageItem>>(json);
            }
        }

        public List<HolidayInfoEntity> GetHolidays()
        {
            if (IsProductEvn)
            {
                return HotelAdapter.GetHolidays();
            }
            else
            {
                string url = APISiteUrl + "api/hotel/GetHolidays";
                CookieContainer cc = new CookieContainer();
                string json = HttpRequestHelper.Get(url, "", ref cc);
                return JsonConvert.DeserializeObject<List<HolidayInfoEntity>>(json);
            }
        }

        public HotelMapBasicInfo GetHotelMapInfo(int hotelID)
        {
            if (IsProductEvn)
            {
                return HotelAdapter.GetHotelMapInfo(hotelID);
            }
            else
            {
                string url = APISiteUrl + "api/hotel/GetHotelMapInfo";
                CookieContainer cc = new CookieContainer();
                string json = HttpRequestHelper.Get(url, "hotelID=" + hotelID, ref cc);
                return JsonConvert.DeserializeObject<HotelMapBasicInfo>(json);

            }
        }

        public CrawlerHotel GetCtripHotel(string hotelId, string checkIn, string checkOut)
        {
            string url = APISiteUrl + "api/hotel/GetCtripHotel";
            string postDataStr = string.Format("hotelId={0}&checkIn={1}&checkOut={2}", hotelId, checkIn, checkOut);
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<CrawlerHotel>(json);
        }

        #region 酒店搜索相关

        public static SearchHotelListResult SearchHotelList(HotelListQueryParam20 p)
        {
            string url = APISiteUrl + "api/Hotel/SearchHotelList";
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.PostJson(url, p, ref cc);
            return JsonConvert.DeserializeObject<SearchHotelListResult>(json);
        }

        public List<QuickSearchSuggestItem> SuggestCityAndHotel(string keyword, int cityCount = 2, int hotelCount = 10)
        {
            string url = APISiteUrl + "api/hotel/SuggestCityAndHotel";
            string postDataStr = string.Format("keyword={0}&cityCount={1}&hotelCount={2}", keyword, cityCount, hotelCount);
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<List<QuickSearchSuggestItem>>(json);
        }

        #endregion

        #region

        public WapInterestHotelsResult3 SearchInterestHotelWeixin(int districtId, int interestId = 0)
        {
            if (IsProductEvn)
                return HotelAdapter.SearchInterestHotelWeixin(districtId, interestId);
            else
            {
                string url = APISiteUrl + "api/hotel/SearchInterestHotelWeixin";
                string postDataStr = string.Format("districtid={0}&interestid={1}"
                  , districtId, interestId);

                CookieContainer cc = new CookieContainer();
                string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
                return JsonConvert.DeserializeObject<WapInterestHotelsResult3>(json);
            }
        }

        public List<string> GetProvinceListByInterest(int interestId)
        {
            if (IsProductEvn)
                return HotelAdapter.GetProvinceListByInterest(interestId);
            else
            {
                string url = APISiteUrl + "api/hotel/GetProvinceListByInterest";
                string postDataStr = string.Format("interestid={0}", interestId);

                CookieContainer cc = new CookieContainer();
                string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
                return JsonConvert.DeserializeObject<List<string>>(json);
            }
        }

        #endregion

        public RecommendHotelResult GetRecommendHotelResult(long curUserID = 0)
        {
            string url = APISiteUrl + "api/hotel/GetRecommendHotelResult";
            string postDataStr = string.Format("curUserID={0}", curUserID);

            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<RecommendHotelResult>(json);
        }

        public HomeDataModel20 GetUserHomeData20(long curUserID = 0)
        {
            string url = APISiteUrl + "api/hotel/GetUserHomeData20";
            string postDataStr = string.Format("curUserID={0}", curUserID);

            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<HomeDataModel20>(json);
        }

        public HomeDataModel20 GetHomeOnlineBanners(long curUserID = 0)
        {
            string url = APISiteUrl + "api/hotel/GetHomeOnlineBanners";
            string postDataStr = string.Format("curUserID={0}", curUserID);

            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<HomeDataModel20>(json);
        }
        public HomeDataModel20 GetHomeOnlineBannersByType(int type = 0, long curUserID = 0)
        {
            string url = APISiteUrl + "api/hotel/GetHomeOnlineBannersByType";
            string postDataStr = string.Format("type={0}&curUserID={1}", type, curUserID);

            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<HomeDataModel20>(json);
        }

        public HomeDataModel20 GetHomeBoxData(long curUserID = 0)
        {
            string url = APISiteUrl + "api/hotel/GetHomeBoxData";
            string postDataStr = string.Format("curUserID={0}", curUserID);

            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<HomeDataModel20>(json);
        }

        public HomePageData30 GetAppHomePageData(long curUserID = 0)
        {
            string url = APISiteUrl + "api/hotel/GetAppHomePageData";
            string postDataStr = string.Format("curUserID={0}", curUserID);

            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<HomePageData30>(json);
        }

        public HomePageData30 GetAppHomeFlashData(long curUserID = 0)
        {
            string url = APISiteUrl + "api/hotel/GetAppHomeFlashData";
            string postDataStr = string.Format("curUserID={0}", curUserID);

            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<HomePageData30>(json);
        }

        public HomePageData30 GetAppHomeGroupData(long curUserID = 0)
        {
            string url = APISiteUrl + "api/hotel/GetAppHomeGroupData";
            string postDataStr = string.Format("curUserID={0}", curUserID);

            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<HomePageData30>(json);
        }

        public InterestModel3 GetHomePageData30(float userLat, float userLng, int geoScopeType, int districtid = 2, int distance = 300000)
        {
            string url = APISiteUrl + "api/hotel/GetHomePageData30";
            string postDataStr = string.Format("userLat={0}&userLng={1}&geoScopeType={2}&districtid={3}&distance={4}", userLat, userLng, geoScopeType, districtid, distance);

            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<InterestModel3>(json);
        }

        public static bool CacheCrawlHotelPackage(CrawlerHotel result)
        {
            string url = APISiteUrl + "api/Hotel/CacheCrawlHotelPackage";
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.PostJson(url, result, ref cc);
            return JsonConvert.DeserializeObject<bool>(json);
        }

        #region Agoda相关处理

        public int Agoda_UploadHotelUrl(string hotelOriId, string hotelUrl, string photoUrl)
        {
            string url = APISiteUrl + "api/hotel/Agoda_UploadHotelUrl";
            string postDataStr = string.Format("hotelOriId={0}&hotelUrl={1}&photourl={2}", hotelOriId, hotelUrl, photoUrl);
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<int>(json);
        }

        #endregion

        #region 酒店列表价相关处理

        public static List<OTAPackageSourceConfig> GetOtaMinPriceHotels()
        {
            string url = APISiteUrl + "api/hotel/GetOtaMinPriceHotels";
            string postDataStr = "";

            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<List<OTAPackageSourceConfig>>(json);
        }

        public int InsertPricePlanAndEx(int hotelId, DateTime date, int channelId, decimal price, string name, string brief)
        {
            string url = APISiteUrl + "api/hotel/InsertPricePlanAndEx";
            string postDataStr = string.Format("hotelId={0}&date={1}&channelId={2}&price={3}&name={4}&brief={5}", hotelId, date.ToString("yyyy-MM-dd"), channelId, price, name, brief);

            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<int>(json);
        }

        public static int InsertHotelPriceSlot(HotelPriceSlot pslot)
        {
            string url = APISiteUrl + "api/Hotel/InsertHotelPriceSlot";
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.PostJson(url, pslot, ref cc);
            return JsonConvert.DeserializeObject<int>(json);
        }

        #endregion

        #region 更新酒店图片标题
        public static int UpdateHotelPhotoTitle(HotelPhotoEntity entity)
        {
            string url = APISiteUrl + "api/Hotel/UpdateHotelPhotoTitle";
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.PostJson(url, entity, ref cc);
            return JsonConvert.DeserializeObject<int>(json);
        }
        #endregion

        public static RecommendPackageDetailResult GetPackageDetailResult(int pid, long userId = 0, string startDate = "", string endDate = "")
        {
            string url = APISiteUrl + "api/hotel/GetPackageDetailResult";
            string postDataStr = string.Format("pid={0}&userId={1}&startDate={2}&endDate={3}", pid, userId, startDate, endDate);

            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<RecommendPackageDetailResult>(json);
        }

        public static RecommendPackageDetailResult GetPackageDetailInfo(int pid, long userId = 0, string startDate = "", string endDate = "")
        {
            //"http://api.dev.jiudian.corp/"
            string url = APISiteUrl + "api/hotel/GetPackageDetailInfo";
            string postDataStr = string.Format("pid={0}&userId={1}&startDate={2}&endDate={3}", pid, userId, startDate, endDate);

            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<RecommendPackageDetailResult>(json);
        }
        /// <summary>
        /// 获取附近城市数据
        /// </summary>
        /// <param name="districtName"></param>
        /// <param name="lon"></param>
        /// <param name="lat"></param>
        /// <param name="districtID"></param>
        /// <returns></returns>
        public static AroundCityList GetAroundCityList(string districtName, float lon = 0, float lat = 0, int districtID = 0)
        {
            string url = APISiteUrl + "api/hotel/GetAroundCityList";
            string postDataStr = string.Format("districtName={0}&lon={1}&lat={2}&districtID={3}", districtName, lon, lat, districtID);

            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<AroundCityList>(json);
        }

        public static CityList GetZMJDAllCityData()
        {
            string url = APISiteUrl + "api/hotel/GetZMJDAllCityData";
            string postDataStr = "";
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<CityList>(json);
        }

        public static List<HJD.HotelServices.Contracts.CityEntity> GetZmjdCityList()
        {
            string url = APISiteUrl + "api/hotel/GetZmjdCityList";
            string postDataStr = "";
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<List<HJD.HotelServices.Contracts.CityEntity>>(json);
        }

        public static List<int> GetPackRoomHotelIdList(DateTime date)
        {
            string url = APISiteUrl + "api/hotel/GetPackRoomHotelIdList";
            string postDataStr = string.Format("date={0}", date.Date.ToString("yyyy-MM-dd"));

            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<List<int>>(json);
        }

        public static PackageAlbumDetail GetPackageAlbumDetail(int albumId, bool isNeedNotSale = false)
        {
            string url = APISiteUrl + "api/hotel/GetPackageAlbumDetail";
            string postDataStr = string.Format("albumId={0}&isNeedNotSale={1}", albumId, isNeedNotSale);
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<PackageAlbumDetail>(json);
        }

        public List<PackageAlbumsEntity> GetPackageAlbumsEntityList(int page = 1, int pageSize = 10)
        {
            string url = APISiteUrl + "api/hotel/GetPackageAlbumsEntityList";
            string postDataStr = string.Format("page={0}&pageSize={1}", page, pageSize);
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<List<PackageAlbumsEntity>>(json);
        }

        public CanSellDistrictHotelResult GetCanSellDistrictCheapHotelList(HJD.HotelServices.Contracts.HotelServiceEnums.HotelDistrictRange range)
        {
            string url = APISiteUrl + "api/hotel/GetCanSellDistrictCheapHotelList";
            string postDataStr = string.Format("range={0}", (int)range);
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<CanSellDistrictHotelResult>(json);
        }

        public HJD.HotelServices.Contracts.PackageEntity GetOnePackageEntity(int pId)
        {
            string url = APISiteUrl + "api/hotel/GetOnePackageEntity";
            string postDataStr = string.Format("pId={0}", pId);
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<HJD.HotelServices.Contracts.PackageEntity>(json);
        }

        public static RecommendHotelResult GetHotRecommendHotel(int start, int count, float lat = 0, float lng = 0, int districtid = 0)
        {
            string url = APISiteUrl + "api/hotel/GetHotRecommendHotel";
            string postDataStr = string.Format("start={0}&count={1}&lat={2}&lng={3}&districtid={4}", start, count, lat, lng, districtid);
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<RecommendHotelResult>(json);
        }

        public static InterestModel3 GetHomePageData40(float userLat, float userLng, int geoScopeType, int districtid = 2, string districtName = null, int needICONCount = 0)
        {
            string url = APISiteUrl + "api/hotel/GetHomePageData40";
            string postDataStr = string.Format("userLat={0}&userLng={1}&geoScopeType={2}&districtid={3}&districtName={4}&needICONCount={5}", userLat, userLng, geoScopeType, districtid, districtName, needICONCount);
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<InterestModel3>(json);
        }

        public static InterestModel3 GetThemeInterestList(float userLat, float userLng, int geoScopeType, int districtid = 2, string districtName = null, int needICONCount = 0)
        {
            string url = APISiteUrl + "api/hotel/GetThemeInterestList";
            string postDataStr = string.Format("userLat={0}&userLng={1}&geoScopeType={2}&districtid={3}&districtName={4}&needICONCount={5}", userLat, userLng, geoScopeType, districtid, districtName, needICONCount);
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<InterestModel3>(json);
        }

        /// <summary>
        /// 为VIP用户提供专享酒店
        /// </summary>
        /// <param name="albumId">专辑ID</param>
        /// <param name="start"></param>
        /// <param name="count"></param>
        /// <param name="curUserID"></param>
        /// <param name="ckvip">是否需要验证VIP身份 1需要 0不需要</param>
        /// <returns></returns>
        public static RecommendHotelResult GetRecommendHotelResultByAlbumId(int albumId, int start, int count, long curUserID, int ckvip = 1, int area = 0, string dateStr = "", int gotoDistrictID = 0, int startDistrictID = 0)
        {
            string url = APISiteUrl + "api/hotel/GetRecommendHotelResultByAlbumId";
            string postDataStr = string.Format("albumId={0}&start={1}&count={2}&curUserID={3}&ckvip={4}&area={5}&dateStr={6}&gotoDistrictID={7}&startDistrictID={8}", albumId, start, count, curUserID, ckvip, area, dateStr, gotoDistrictID, startDistrictID);
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<RecommendHotelResult>(json);
        }

        /// <summary>
        /// 为VIP用户提供专享酒店 加搜索功能
        /// </summary>
        /// <param name="albumId"></param>
        /// <param name="start"></param>
        /// <param name="count"></param>
        /// <param name="curUserID"></param>
        /// <param name="lat"></param>
        /// <param name="lng"></param>
        /// <param name="geoScopeType"> 等于3 查询300km之内</param>
        /// <param name="districtID">城市id</param>
        /// <param name="ckvip"></param>
        /// <returns></returns>
        public RecommendHotelResult GetRecommendHotelResultByAlbumIdAddSearch(int albumId, int start, int count, long curUserID, float lat = 0, float lng = 0, int geoScopeType = 0, int districtID = 0, int ckvip = 1)
        {
            string url = APISiteUrl + "api/hotel/GetRecommendHotelResultByAlbumIdAddSearch";
            string postDataStr = string.Format("albumId={0}&start={1}&count={2}&curUserID={3}&lat={4}&lng={5}&geoScopeType={6}&districtID={7}&ckvip={8}", albumId, start, count, curUserID, lat, lng, geoScopeType, districtID, ckvip);
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<RecommendHotelResult>(json);
        }

        public HotelDestInfoList GetDistrictInfoForAlbum(int albumsID, float lat, float lng)
        {
            string url = APISiteUrl + "api/hotel/GetDistrictInfoForAlbum";
            string postDataStr = string.Format("albumsID={0}&lat={1}&lng={2}", albumsID, lat, lng);
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<HotelDestInfoList>(json);
        }

        #region 4.7版本App 个性化首页
        /// <summary>
        /// 推荐酒店列表
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static RecommendHotelResult GetRecommendHotelListByFollowing(RecommendCommentParam param)
        {
            string url = APISiteUrl + "api/hotel/GetRecommendHotelListByFollowing";
            string postDataStr = string.Format("start={0}&count={1}&userId={2}", param.start, param.count, param.userId);
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<RecommendHotelResult>(json);
        }

        /// <summary>
        /// 酒店浏览记录
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static RecommendHotelResult GetHotelBrowsingRecordList(BsicSearchParam param)
        {
            string url = APISiteUrl + "api/hotel/GetHotelBrowsingRecordList";
            string postDataStr = string.Format("start={0}&count={1}&userId={2}", param.start, param.count, param.userId);
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<RecommendHotelResult>(json);
        }
        
        /// <summary>
        /// 300公里之内酒店
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static RecommendHotelResult GetHotelWithInDistance(int count, int start, float lat = 0, float lng = 0)
        {
            string url = APISiteUrl + "api/hotel/GetHotelWithInDistance";
            string postDataStr = string.Format("lat={0}&lng={1}&count={2}&start={3}", lat, lng, count, start);
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<RecommendHotelResult>(json);
        }
        

        public static RecommendHotelResult GetSearchRecordList(CommonRecordQueryParam param)
        {
            string url = APISiteUrl + "api/hotel/GetSearchRecordList";
            string postDataStr = string.Format("start={0}&count={1}&userId={2}&businessType={3}", param.start, param.count, param.userId, param.businessType);
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<RecommendHotelResult>(json);
        }

        #endregion

        public static ActiveRuleGroupEntity GetActiveRuleList(int id)
        {
            string url = APISiteUrl + "api/hotel/GetActiveRuleList";
            string postDataStr = string.Format("id={0}", id);
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<ActiveRuleGroupEntity>(json);
        }

        public bool QuickPublishPriceSlotTask(int hotelId, DateTime checkIn)
        {
            string url = APISiteUrl + "api/hotel/QuickPublishPriceSlotTask";
            string postDataStr = string.Format("hotelId={0}&checkIn={1}", hotelId, checkIn.ToString("yyyy-MM-dd"));

            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<bool>(json);
        }
        public static bool UpdateAlbumRedisCache(int albumId=0, int pid=0)
        {
            string url = APISiteUrl + "api/hotel/UpdateAlbumRedisCache";
            string postDataStr = string.Format("albumId={0}&pid={1}", albumId, pid);

            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<bool>(json);
        }
        public static PackageAlbumDetail GetPackageGroupAlbumDetail(int albumId, int startDistrictId = 0, int start = 0, int count = 100, bool isNeedNotSale = false)
        {
            string url = APISiteUrl + "api/hotel/GetPackageGroupAlbumDetail";
            string postDataStr = string.Format("albumId={0}&startDistrictId={1}", albumId, startDistrictId);

            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<PackageAlbumDetail>(json);
        }

        public static ScreenConditionsEntity GetAlbumFilter(int albumId)
        {
            string url = APISiteUrl + "api/hotel/GetAlbumFilter";
            string postDataStr = string.Format("albumId={0}", albumId);

            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<ScreenConditionsEntity>(json);
        }

        #region 分销产品

        public static RetailHotelEntity GetRetailHotelList(HJD.CouponService.Contracts.Entity.SearchProductRequestEntity param)
        {
            string url = APISiteUrl + "api/hotel/GetRetailHotelList";
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.PostJson(url, param, ref cc);
            return JsonConvert.DeserializeObject<RetailHotelEntity>(json);
        }
        public static RetailHotelInfoEntity GetRetailHotelDetail(int hotelId)
        {
            string url = APISiteUrl + "api/hotel/GetRetailHotelDetail";
            string postDataStr = string.Format("hotelId={0}", hotelId);

            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<RetailHotelInfoEntity>(json);
        }

        public static RetailPackageEntity GetRetailPackageInfo(DateTime checkIn, DateTime checkOut, int pid, long cid)
        {
            string url = APISiteUrl + "api/hotel/GetRetailPackageInfo";
            string postDataStr = string.Format("checkIn={0}&checkOut={1}&pid={2}&cid={3}", checkIn, checkOut, pid, cid);

            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<RetailPackageEntity>(json);
        }

        #endregion
    }
}