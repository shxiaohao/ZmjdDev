using HJD.OtaCrawlerService.Contract.Crawl;
using HJD.OtaCrawlerService.Contract.Ctrip.Hotel;
using HJD.OtaCrawlerService.Contract.Ctrip.Report;
using HJD.OtaCrawlerService.Contract.Hotel;
using HJD.OtaCrawlerService.Contract.Params;
using HJD.OtaCrawlerService.Contract.Proxy;
using HJDAPI.Common.Helpers;
using HJDAPI.Controllers;
using HJDAPI.Controllers.Adapter;
using HJDAPI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.APIProxy
{
    public class OtaCrawler : BaseProxy
    {
        public CrawlerHotel GetCtripCanSellHotel(HotelRoomParams hotelRoomParams)
        {
            if (IsProductEvn)
            {
                return new OtaCrawlerAdapter().GetCtripCanSellHotel(hotelRoomParams);
            }
            else
            {
                string url = APISiteUrl + "api/OtaCrawler/GetCtripCanSellHotel";
                string postDataStr = string.Format("hotelid={0}&checkin={1}&checkout={2}&roomid={3}", hotelRoomParams.HotelID, hotelRoomParams.CheckIn, hotelRoomParams.CheckOut, hotelRoomParams.RoomID, hotelRoomParams.OtaType);

                CookieContainer cc = new CookieContainer();

                string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
                return JsonConvert.DeserializeObject<CrawlerHotel>(json);
            }
        }

        public Hotels GetPreHotel(int groupId)
        {
            string url = APISiteUrl + "api/OtaCrawler/GetPreHotel";
            string postDataStr = string.Format("groupId={0}",groupId);

            CookieContainer cc = new CookieContainer();

            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<Hotels>(json);
        }

        public Hotels GetFirstErrorHotel()
        {
            string url = APISiteUrl + "api/OtaCrawler/GetFirstErrorHotel";
            string postDataStr = "";

            CookieContainer cc = new CookieContainer();

            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<Hotels>(json);
        }

        public int UpdateHotelStatus(Hotels hotels)
        {
            string url = APISiteUrl + "api/Hotel/UpdateHotelStatus";
            string postDataStr = string.Format("hid={0}&realhotelid={1}&status={2}&crawlercount={3}",
                hotels.HID, hotels.RealHotelID, hotels.Status, hotels.CrawlerCount);

            CookieContainer cc = new CookieContainer();

            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<int>(json);
        }

        public int InsertHistorys(Historys historys)
        {
            string url = APISiteUrl + "api/Hotel/InsertHistorys";
            string postDataStr = string.Format("hid={0}&status={1}&proxy={2}&message={3}",
                historys.HID, historys.Status, historys.Proxy, historys.Message);

            CookieContainer cc = new CookieContainer();

            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<int>(json);
        }

        public string InsertCtripRoom(CrawlerHotelBaseRoom baseRoom)
        {
            string url = APISiteUrl + "api/Hotel/InsertCtripRoom";
            CookieContainer cc = new CookieContainer();

            string json = HttpRequestHelper.PostJson(url, baseRoom, ref cc);
            return JsonConvert.DeserializeObject<string>(json);
        }

        public int InsertCtripPriceRate(CrawlerHotelRoom room)
        {
            string url = APISiteUrl + "api/Hotel/InsertCtripPriceRate";
            CookieContainer cc = new CookieContainer();

            string json = HttpRequestHelper.PostJson(url, room, ref cc);
            return JsonConvert.DeserializeObject<int>(json);
        }

        public List<ProxyData> GetProxyDataList()
        {
            string url = APISiteUrl + "api/OtaCrawler/GetProxyDataList";
            string postDataStr = "";

            CookieContainer cc = new CookieContainer();

            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<List<ProxyData>>(json);
        }

        public List<ProxyData> GetOptimizeProxyDataList()
        {
            string url = APISiteUrl + "api/OtaCrawler/GetOptimizeProxyDataList";
            string postDataStr = "";

            CookieContainer cc = new CookieContainer();

            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<List<ProxyData>>(json);
        }

        public int InsertOptimizeProxyData(ProxyData proxy)
        {
            string url = APISiteUrl + "api/Hotel/InsertOptimizeProxyData";
            string postDataStr = string.Format("ipAddress={0}&ip={1}&port={2}",
                proxy.IpAddress, proxy.Ip, proxy.Port);

            CookieContainer cc = new CookieContainer();

            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<int>(json);
        }

        public int InsertProxyData(ProxyData proxy)
        {
            string url = APISiteUrl + "api/Hotel/InsertProxyData";
            string postDataStr = string.Format("ipAddress={0}&ip={1}&port={2}",
                proxy.IpAddress, proxy.Ip, proxy.Port);

            CookieContainer cc = new CookieContainer();

            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<int>(json);
        }

        public int DeleteProxyData()
        {
            string url = APISiteUrl + "api/Hotel/DeleteProxyData";
            string postDataStr = "";

            CookieContainer cc = new CookieContainer();

            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<int>(json);
        }

        public int PreHotelsData(DateTime night, int groupId)
        {
            string url = APISiteUrl + "api/Hotel/PreHotelsData";
            string postDataStr = string.Format("night={0}&groupId={1}", night.ToString("yyyy-MM-dd"), groupId);

            CookieContainer cc = new CookieContainer();

            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<int>(json);
        }

        public List<HotelBase> GetUnCheckHotelBase()
        {
            string url = APISiteUrl + "api/OtaCrawler/GetUnCheckHotelBase";
            string postDataStr = "";

            CookieContainer cc = new CookieContainer();

            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<List<HotelBase>>(json);
        }

        public int UpdateRealHotelBase(string hotelID, string realHotelID)
        {
            string url = APISiteUrl + "api/Hotel/UpdateRealHotelBase";
            string postDataStr = string.Format("hid={0}&realhotelid={1}", hotelID, realHotelID);

            CookieContainer cc = new CookieContainer();

            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<int>(json);
        }

        public List<CrawlerHotelBaseRoom> GetUpdatedRoomList()
        {
            string url = APISiteUrl + "api/OtaCrawler/GetUpdatedRoomList";
            string postDataStr = "";

            CookieContainer cc = new CookieContainer();

            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<List<CrawlerHotelBaseRoom>>(json);
        }

        public List<CrawlerHotelRoom> GetUpdatedPriceRateList()
        {
            string url = APISiteUrl + "api/OtaCrawler/GetUpdatedPriceRateList";
            string postDataStr = "";

            CookieContainer cc = new CookieContainer();

            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<List<CrawlerHotelRoom>>(json);
        }

        public int InsertProductCtripRoom(CrawlerHotelBaseRoom baseRoom)
        {
            string url = APISiteUrl + "api/Hotel/InsertProductCtripRoom";
            CookieContainer cc = new CookieContainer();

            string json = HttpRequestHelper.PostJson(url, baseRoom, ref cc);
            return JsonConvert.DeserializeObject<int>(json);
        }

        public int InsertProductCtripPriceRate(CrawlerHotelRoom room)
        {
            string url = APISiteUrl + "api/Hotel/InsertProductCtripPriceRate";

            CookieContainer cc = new CookieContainer();

            string json = HttpRequestHelper.PostJson(url, room, ref cc);
            return JsonConvert.DeserializeObject<int>(json);
        }

        public List<PRoomInfoEntity> GetPRoomInfo()
        {
            string url = APISiteUrl + "api/OtaCrawler/GetPRoomInfo";
            string postDataStr = "";

            CookieContainer cc = new CookieContainer();

            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<List<PRoomInfoEntity>>(json);
        }

        public List<CrawlerHotelBaseRoom> GetBaseRoomInfo()
        {
            string url = APISiteUrl + "api/OtaCrawler/GetBaseRoomInfo";
            string postDataStr = "";

            CookieContainer cc = new CookieContainer();

            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<List<CrawlerHotelBaseRoom>>(json);
        }

        public HotelOTAEntity GetHotelOriId(long hotelid)
        {
            string url = APISiteUrl + "api/OtaCrawler/GetHotelOriId";
            string postDataStr = string.Format("hotelid={0}", hotelid);

            CookieContainer cc = new CookieContainer();

            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<HotelOTAEntity>(json);
        }

        public int InsertRoomMatch(RoomMatch roomMatch)
        {
            string url = APISiteUrl + "api/Hotel/InsertRoomMatch";
            string postDataStr = string.Format("baseRoomID={0}&otaRoomName={1}&proomID={2}&pRoomCode={3}&hotelID={4}&hotelOriID={5}&matchRate={6}&matched={7}&matchTime={8}",
                roomMatch.BaseRoomID, roomMatch.OtaRoomName, roomMatch.PRoomID, roomMatch.PRoomCode, roomMatch.HotelID, roomMatch.HotelOriID, roomMatch.MatchRate, roomMatch.Matched, roomMatch.MatchTime);

            CookieContainer cc = new CookieContainer();

            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<int>(json);
        }

        public List<RoomMatch> GetRoomMatchList()
        {
            string url = APISiteUrl + "api/OtaCrawler/GetRoomMatchList";
            string postDataStr = "";

            CookieContainer cc = new CookieContainer();

            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<List<RoomMatch>>(json);
        }

        public List<CrawlerHotelRoom> GetMatchPriceRateList(long baseRoomID, string breakfast, DateTime night)
        {
            string url = APISiteUrl + "api/OtaCrawler/GetMatchPriceRateList";
            string postDataStr = string.Format("baseRoomID={0}&breakfast={1}&night={2}", baseRoomID, breakfast, night);

            CookieContainer cc = new CookieContainer();

            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<List<CrawlerHotelRoom>>(json);
        }

        public int InsertPackageMatch(PackageMatch packageMatch)
        {
            string url = APISiteUrl + "api/Hotel/InsertPackageMatch";

            CookieContainer cc = new CookieContainer();

            string json = HttpRequestHelper.PostJson(url, packageMatch, ref cc);
            return JsonConvert.DeserializeObject<int>(json);
        }

        public HotelBase GetHotelBaseByOldId(long hotelid)
        {
            string url = APISiteUrl + "api/OtaCrawler/GetHotelBaseByOldId";
            string postDataStr = string.Format("hotelid={0}", hotelid);

            CookieContainer cc = new CookieContainer();

            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<HotelBase>(json);
        }

        public int BindRoomMatchRate()
        {
            string url = APISiteUrl + "api/Hotel/BindRoomMatchRate";
            string postDataStr = "";
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<int>(json);
        }

        public RoomMatchRate GetRoomMatchRate(long hotelId, string pRoomCode, long hotelOriId, string otaRoomName)
        {
            string url = APISiteUrl + "api/OtaCrawler/GetRoomMatchRate";
            string postDataStr = string.Format("hotelId={0}&pRoomCode={1}&hotelOriId={2}&otaRoomName={3}", hotelId, pRoomCode, hotelOriId, otaRoomName);

            CookieContainer cc = new CookieContainer();

            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<RoomMatchRate>(json);
        }

        public int BakTodayHotels()
        {
            string url = APISiteUrl + "api/Hotel/BakTodayHotels";
            string postDataStr = "";
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<int>(json);
        }

        public List<PackageMatchReport> GetPackageMatchReport()
        {
            string url = APISiteUrl + "api/OtaCrawler/GetPackageMatchReport";
            string postDataStr = "";
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<List<PackageMatchReport>>(json);
        }

        public List<RoomMatchRateReport> GetRoomMatchRateReport()
        {
            string url = APISiteUrl + "api/OtaCrawler/GetRoomMatchRateReport";
            string postDataStr = "";
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<List<RoomMatchRateReport>>(json);
        }

        public Config GetCrawlerConfig(string code)
        {
            string url = APISiteUrl + "api/OtaCrawler/GetCrawlerConfig";
            string postDataStr = string.Format("code={0}", code);
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<Config>(json);
        }

        public int UpdateCrawlerConfig(string code, string value)
        {
            string url = APISiteUrl + "api/Hotel/UpdateCrawlerConfig";
            string postDataStr = string.Format("code={0}&value={1}", code, value);

            CookieContainer cc = new CookieContainer();

            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<int>(json);
        }

        public int InsertPricePlan(PricePlan pricePlan)
        {
            string url = APISiteUrl + "api/Hotel/InsertPricePlan";

            CookieContainer cc = new CookieContainer();

            string json = HttpRequestHelper.PostJson(url, pricePlan, ref cc);
            return JsonConvert.DeserializeObject<int>(json);
        }

        public int UpdateOldPriceRate(DateTime night)
        {
            string url = APISiteUrl + "api/Hotel/UpdateOldPriceRate";
            string postDataStr = string.Format("night={0}", night.ToString("yyyy-MM-dd"));

            CookieContainer cc = new CookieContainer();

            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<int>(json);
        }

        public string GetCtripApiAccessToken(int type)
        {
            string url = APISiteUrl + "api/OtaCrawler/GetCtripApiAccessToken";
            string postDataStr = string.Format("type={0}", type);

            CookieContainer cc = new CookieContainer();

            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<string>(json);
        }

        #region 抓取与业务数据处理层相关操作

        public OTAPackageSourceConfig GetOnePackageSourceConfig(int channelId, int isValid)
        {
            string url = APISiteUrl + "api/OtaCrawler/GetOnePackageSourceConfig";
            string postDataStr = string.Format("channelId={0}&isValid={1}", channelId, isValid);
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<OTAPackageSourceConfig>(json);
        }

        public int InsertUpdatePackageSourceConfig(OTAPackageSourceConfig packageSourceConfig)
        {
            string url = APISiteUrl + "api/OtaCrawler/InsertUpdatePackageSourceConfig";

            CookieContainer cc = new CookieContainer();

            string json = HttpRequestHelper.PostJson(url, packageSourceConfig, ref cc);
            return JsonConvert.DeserializeObject<int>(json);
        }

        public OTAPackageSourceConfig GetPackageSourceConfigByHotelId(int channelId, Int64 hotelId)
        {
            string url = APISiteUrl + "api/OtaCrawler/GetPackageSourceConfigByHotelId";
            string postDataStr = string.Format("channelId={0}&hotelId={1}", channelId, hotelId);
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<OTAPackageSourceConfig>(json);
        }

        public int UpdatePackageSourceConfigValid(Int64 id, int valid)
        {
            string url = APISiteUrl + "api/Hotel/UpdatePackageSourceConfigValid";
            string postDataStr = string.Format("id={0}&valid={1}", id, valid);
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<int>(json);
        }

        public CrawlerHotel GetPackagesForCtripApi(Int64 hotelId, DateTime checkIn, DateTime checkOut)
        {
            string url = APISiteUrl + "api/OtaCrawler/GetPackagesForCtripApi";
            string postDataStr = string.Format("hotelId={0}&checkIn={1}&checkOut={2}", hotelId, checkIn.ToString("yyyy-MM-dd"), checkOut.ToString("yyyy-MM-dd"));
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<CrawlerHotel>(json);
        }

        public CrawlerHotel GetAllPackagesForCtripApi(Int64 hotelId, DateTime checkIn, DateTime checkOut)
        {
            string url = APISiteUrl + "api/OtaCrawler/GetAllPackagesForCtripApi";
            string postDataStr = string.Format("hotelId={0}&checkIn={1}&checkOut={2}", hotelId, checkIn.ToString("yyyy-MM-dd"), checkOut.ToString("yyyy-MM-dd"));
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<CrawlerHotel>(json);
        }

        /// <summary>
        /// 是否为中国酒店
        /// </summary>
        /// <param name="hotelId"></param>
        /// <returns></returns>
        public bool HotelInChina(int hotelId)
        {
            string url = APISiteUrl + "api/OtaCrawler/HotelInChina";
            string postDataStr = string.Format("hotelId={0}", hotelId);
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<bool>(json);
        }

        #endregion

        #region 抓取任务管理

        public int SavePluginFile(PluginInfo plugin)
        {
            string url = APISiteUrl + "api/OtaCrawler/SavePluginFile";

            CookieContainer cc = new CookieContainer();

            string json = HttpRequestHelper.PostJson(url, plugin, ref cc);
            return JsonConvert.DeserializeObject<int>(json);
        }

        public string GetPluginFileTxt(string pluginName, string localPath)
        {
            string url = APISiteUrl + "api/OtaCrawler/GetPluginFileTxt";
            string postDataStr = string.Format("pluginName={0}&localPath={1}", pluginName, localPath);

            CookieContainer cc = new CookieContainer();

            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<string>(json);
        }

        #endregion

        #region 酒店套餐价格报表相关

        /// <summary>
        /// Zmjd套餐价格报表统计插入
        /// </summary>
        /// <param name="pp"></param>
        /// <returns></returns>
        public int InsertPackagePriceReportZmjd(PackagePriceV001 pp)
        {
            string url = APISiteUrl + "api/OtaCrawler/InsertPackagePriceReportZmjd";

            CookieContainer cc = new CookieContainer();

            string json = HttpRequestHelper.PostJson(url, pp, ref cc);
            return JsonConvert.DeserializeObject<int>(json);
        }

        /// <summary>
        /// XC套餐价格报表统计插入
        /// </summary>
        /// <param name="pp"></param>
        /// <returns></returns>
        public int InsertPackagePriceReportCtrip(PackagePriceV001 pp)
        {
            string url = APISiteUrl + "api/OtaCrawler/InsertPackagePriceReportCtrip";

            CookieContainer cc = new CookieContainer();

            string json = HttpRequestHelper.PostJson(url, pp, ref cc);
            return JsonConvert.DeserializeObject<int>(json);
        }

        #endregion
    }
}
