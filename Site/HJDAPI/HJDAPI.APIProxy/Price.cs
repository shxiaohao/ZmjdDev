using HJD.HotelPrice.Contract.DataContract.Order;
using HJD.HotelServices.Contracts;
using HJDAPI.Common.Helpers;
using HJDAPI.Controllers.Adapter;
using HJDAPI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Serialization;

namespace HJDAPI.APIProxy
{
    public class Price : BaseProxy
    {
        public static Models.HotelPrice Get(int id, string checkIn, string checkOut, string sType)
        {

            if (IsProductEvn)
                return PriceAdapter.Get(id, checkIn, checkOut, sType);
            else
            {
                string url = APISiteUrl + "api/Price/Get";
                string postDataStr = string.Format("id={0}&checkIn={1}&checkOut={2}&sType={3}"
                  , id, checkIn, checkOut, sType);

                CookieContainer cc = new CookieContainer();

                string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
                return JsonConvert.DeserializeObject<Models.HotelPrice>(json);
            }

        }
        
        public static Models.HotelPrice2 Get2(int id, string checkIn, string checkOut, string sType)
        {

            if (IsProductEvn)
                return PriceAdapter.Get2(id, checkIn, checkOut, sType);
            else
            {
                string url = APISiteUrl + "api/Price/Get2";
                string postDataStr = string.Format("id={0}&checkIn={1}&checkOut={2}&sType={3}"
                  , id, checkIn, checkOut, sType);

                CookieContainer cc = new CookieContainer();

                string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
                return JsonConvert.DeserializeObject<Models.HotelPrice2>(json);
            }

        }

        public static Models.HotelPrice2 Get3(int id, string checkIn, string checkOut, string sType, bool needNotSalePackage = false)
        {
            if (IsProductEvn)
                return PriceAdapter.GetHotelPackageList(id, checkIn, checkOut, sType, needNotSalePackage: needNotSalePackage);
            else
            {
                string url = APISiteUrl + "api/Price/Get3";
                string postDataStr = string.Format("id={0}&checkIn={1}&checkOut={2}&sType={3}"
                  , id, checkIn, checkOut, sType);

                CookieContainer cc = new CookieContainer();

                string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
                return JsonConvert.DeserializeObject<Models.HotelPrice2>(json);
            }
        }

        /// <summary>
        /// App4.2版本 用来获得新的套餐
        /// </summary>
        /// <param name="id"></param>
        /// <param name="checkIn"></param>
        /// <param name="checkOut"></param>
        /// <param name="sType"></param>
        /// <returns></returns>
        public static Models.HotelPrice2 GetV42(int id, string checkIn, string checkOut, string sType, long userID = 0, bool needNotSalePackage = false)
        {
            if (IsProductEvn)
                return PriceAdapter.GetHotelPackageList(id, checkIn, checkOut, sType, "4.2", needNotSalePackage: needNotSalePackage);
            else
            {
                string url = APISiteUrl + "api/Price/GetV42";
                string postDataStr = string.Format("id={0}&checkIn={1}&checkOut={2}&sType={3}&userID={4}&needNotSalePackage={5}"
                  , id, checkIn, checkOut, sType, userID, needNotSalePackage);

                CookieContainer cc = new CookieContainer();
                string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
                return JsonConvert.DeserializeObject<Models.HotelPrice2>(json);
            }
        }

        public static Models.HotelPrice2 Get32(int id, string checkIn, string checkOut, string sType, int updatePrice)
        {
            if (IsProductEvn)
                return PriceAdapter.GetHotelPackageList(id, checkIn, checkOut, sType);
            else
            {
                string url = APISiteUrl + "api/Price/Get32";
                string postDataStr = string.Format("id={0}&checkIn={1}&checkOut={2}&sType={3}&updatePrice={4}"
                  , id, checkIn, checkOut, sType, updatePrice);

                CookieContainer cc = new CookieContainer();

                string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
                return JsonConvert.DeserializeObject<Models.HotelPrice2>(json);
            }

        }

        public static Models.HotelPrice2 Get4(int hotelId, string code, string sType, int pid)
        {
            if (IsProductEvn)
                return PriceAdapter.Get4(hotelId, code, sType, pid);
            else
            {
                string url = APISiteUrl + "api/Price/Get4";
                string postDataStr = string.Format("hotelid={0}&code={1}&sType={2}&pid={3}"
                  , hotelId, code, sType, pid);

                CookieContainer cc = new CookieContainer();

                string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
                return JsonConvert.DeserializeObject<Models.HotelPrice2>(json);
            }
        }

        public static HotelPrice3 Get6(int hotelid, string checkIn, string checkOut, string sType, long userId = 0)
        {
            string url = APISiteUrl + "api/Price/Get6";
            string postDataStr = string.Format("hotelid={0}&checkIn={1}&checkOut={2}&sType={3}&userId={4}"
              , hotelid, checkIn, checkOut, sType, userId);

            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<HotelPrice3>(json);
        }

        public static HotelPrice3 Get7(int hotelid, string checkIn, string checkOut, string sType, HJD.HotelServices.Contracts.HotelServiceEnums.PackageType packageType = HJD.HotelServices.Contracts.HotelServiceEnums.PackageType.HotelPackage, long userId = 0, bool isNotSale = false)
        {
            string url = APISiteUrl + "api/Price/Get7";
            string postDataStr = string.Format("hotelid={0}&checkIn={1}&checkOut={2}&sType={3}&packageType={4}&userId={5}&isNotSale={6}"
              , hotelid, checkIn, checkOut, sType, Convert.ToInt32(packageType), userId, isNotSale);

            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<HotelPrice3>(json);
        }

        public static List<HJD.HotelServices.Contracts.PDayItem> GetHotelPackageCalendar30(int hotelid, DateTime startDate)
        {
            if (IsProductEvn)
                return PackageAdapter.GetHotelPackageCalendar30(hotelid, startDate);
            else
            {
                string url = APISiteUrl + "api/Price/GetHotelPackageCalendar30";
                string postDataStr = string.Format("hotelid={0}&startDate={1}"
                  , hotelid, startDate.ToString("yyyy-MM-dd"));

                CookieContainer cc = new CookieContainer();

                string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
                return JsonConvert.DeserializeObject<List<HJD.HotelServices.Contracts.PDayItem>>(json);
            }
        }

        public static List<HJD.HotelServices.Contracts.PDayItem> GetOneHotelPackageCalendar30(int hotelid, DateTime startDate, int pid = 0)
        {
            if (IsProductEvn)
                return PackageAdapter.GetHotelPackageCalendar30(hotelid, startDate, pid);
            else
            {
                string url = APISiteUrl + "api/Price/GetOneHotelPackageCalendar30";
                string postDataStr = string.Format("hotelid={0}&startDate={1}&pid={2}"
                  , hotelid, startDate.ToString("yyyy-MM-dd"), pid);

                CookieContainer cc = new CookieContainer();

                string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
                return JsonConvert.DeserializeObject<List<HJD.HotelServices.Contracts.PDayItem>>(json);
            }
        }

        public static Models.HotelPrice2 GetOtaList(int id, string checkIn, string checkOut, string sType)
        {

            if (IsProductEvn)
                return PriceAdapter.GetOtaList(id, checkIn, checkOut, sType);
            else
            {
                string url = APISiteUrl + "api/Price/GetOtaList";
                string postDataStr = string.Format("id={0}&checkIn={1}&checkOut={2}&sType={3}"
                  , id, checkIn, checkOut, sType);

                CookieContainer cc = new CookieContainer();

                string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
                return JsonConvert.DeserializeObject<Models.HotelPrice2>(json);
            }

        }

        public static HotelPackageCalendar GetOneHotelPackageCalendar(int hotelid, DateTime startDate, int pid, long userId = 0)
        {
            string url = APISiteUrl + "api/Price/GetOneHotelPackageCalendar";
            string postDataStr = string.Format("hotelid={0}&startDate={1}&pid={2}&userId={3}"
              , hotelid, startDate.ToString("yyyy-MM-dd"), pid, userId);

            CookieContainer cc = new CookieContainer();

            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<HotelPackageCalendar>(json);
        }
        public static HotelPackageCalendar GetOneHotelPackageCalendarWithCustomerType(int hotelid, DateTime startDate, int pid, HJDAPI.Common.Helpers.Enums.CustomerType customerType)
        {
            string url = APISiteUrl + "api/Price/GetOneHotelPackageCalendarWithCustomerType";
            string postDataStr = string.Format("hotelid={0}&startDate={1}&pid={2}&customerType={3}"
              , hotelid, startDate.ToString("yyyy-MM-dd"), pid, (int)customerType);

            CookieContainer cc = new CookieContainer();

            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<HotelPackageCalendar>(json);
        }

        public static PackageInfoEntity GetFirstVIPPackageByPackageId(int pid, string checkIn, string checkOut)
        {
            string url = APISiteUrl + "api/Price/GetFirstVIPPackageByPackageId";
            string postDataStr = string.Format("pid={0}&checkIn={1}&checkOut={2}", pid, checkIn, checkOut);

            CookieContainer cc = new CookieContainer();

            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<PackageInfoEntity>(json);
        } 
        
        
        public static PackageInfoEntity GetOnePackageInfoEntity(int pId, int hotelId, DateTime checkIn, DateTime checkOut, long userId = 0)
        {
            string url = APISiteUrl + "api/Price/GetOnePackageInfoEntity";
            string postDataStr = string.Format("pId={0}&hotelId={1}&checkIn={2}&checkOut={3}&userId={4}",  pId,  hotelId,  checkIn.ToString("yyyy-MM-dd"),  checkOut.ToString("yyyy-MM-dd"),  userId);

            CookieContainer cc = new CookieContainer();

            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<PackageInfoEntity>(json);
        }
    }
}
