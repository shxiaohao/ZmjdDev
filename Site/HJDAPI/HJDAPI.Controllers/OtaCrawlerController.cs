using HJD.Framework.Interface;
using HJD.Framework.WCF;
using HJD.HotelManagementCenter.IServices;
using HJD.OtaCrawlerService.Contract;
using HJD.OtaCrawlerService.Contract.Crawl;
using HJD.OtaCrawlerService.Contract.Ctrip.Hotel;
using HJD.OtaCrawlerService.Contract.Ctrip.Report;
using HJD.OtaCrawlerService.Contract.Hotel;
using HJD.OtaCrawlerService.Contract.Params;
using HJD.OtaCrawlerService.Contract.Proxy;
using HJD.OtaCrawlerServices.Contract;
using HJDAPI.Controllers.Activity;
using HJDAPI.Controllers.Adapter;
using HJDAPI.Controllers.Common;
using HJDAPI.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Security;
using System.Xml;

namespace HJDAPI.Controllers
{
    public class OtaCrawlerController : BaseApiController
    {
        public static ICacheProvider LocalCache5Min = CacheManagerFactory.Create("DynamicCacheFor5Min");
        //public static ICommService commService = ServiceProxyFactory.Create<ICommService>("ICommService");
        public static IOtaCrawlerService otaCrawlerService = ServiceProxyFactory.Create<IOtaCrawlerService>("IOtaCrawlerService");

        private string logFile = Configs.LogPath + string.Format("OtaCrawlerLog_{0}{1}{2}.txt", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

        [HttpGet]
        public CrawlerHotel GetCtripCanSellHotel(string hotelid, string checkin, string checkout, string roomid)
        {
            HotelRoomParams hotelRoomParams = new HotelRoomParams {
                HotelID = hotelid,
                CheckIn = checkin,
                CheckOut = checkout,
                RoomID = roomid
            };
            //return null;
            return new OtaCrawlerAdapter().GetCtripCanSellHotel(hotelRoomParams);
        }

        [HttpGet]
        public Hotels GetPreHotel(int groupId)
        {
            Hotels hotels = new Hotels();

            hotels = otaCrawlerService.GetPreHotel(groupId);

            return hotels;
        }

        [HttpGet]
        public Hotels GetFirstErrorHotel()
        {
            Hotels hotels = new Hotels();

            hotels = otaCrawlerService.GetFirstErrorHotel();

            return hotels;
        }

        [HttpGet]
        public List<ProxyData> GetProxyDataList()
        {
            List<ProxyData> proxyDataList = new List<ProxyData>();

            proxyDataList = otaCrawlerService.GetProxyDataList();

            return proxyDataList;
        }

        [HttpGet]
        public List<ProxyData> GetOptimizeProxyDataList()
        {
            List<ProxyData> proxyDataList = new List<ProxyData>();

            proxyDataList = otaCrawlerService.GetOptimizeProxyDataList();

            return proxyDataList;
        }

        [HttpGet]
        public List<CrawlerHotelBaseRoom> GetUpdatedRoomList()
        {
            List<CrawlerHotelBaseRoom> roomList = new List<CrawlerHotelBaseRoom>();

            roomList = otaCrawlerService.GetUpdatedRoomList();

            return roomList;
        }

        [HttpGet]
        public List<CrawlerHotelRoom> GetUpdatedPriceRateList()
        {
            List<CrawlerHotelRoom> priceRateList = new List<CrawlerHotelRoom>();

            priceRateList = otaCrawlerService.GetUpdatedPriceRateList();

            return priceRateList;
        }

        [HttpGet]
        public List<HotelBase> GetUnCheckHotelBase()
        {
            List<HotelBase> list = new List<HotelBase>();

            list = otaCrawlerService.GetUnCheckHotelBase();

            return list;
        }

        [HttpGet]
        public List<PRoomInfoEntity> GetPRoomInfo()
        {
            return otaCrawlerService.GetPRoomInfo();
        }

        [HttpGet]
        public List<CrawlerHotelBaseRoom> GetBaseRoomInfo()
        {
            return otaCrawlerService.GetBaseRoomInfo();
        }

        [HttpGet]
        public HotelOTAEntity GetHotelOriId(long hotelid)
        {
            return otaCrawlerService.GetHotelOriId(hotelid);
        }

        [HttpGet]
        public List<RoomMatch> GetRoomMatchList()
        {
            return otaCrawlerService.GetRoomMatchList();
        }

        [HttpGet]
        public List<CrawlerHotelRoom> GetMatchPriceRateList(long baseRoomID, string breakfast, DateTime night)
        {
            return otaCrawlerService.GetMatchPriceRateList(baseRoomID, breakfast, night);
        }

        [HttpGet]
        public HotelBase GetHotelBaseByOldId(long hotelid)
        {
            return otaCrawlerService.GetHotelBaseByOldId(hotelid);
        }

        [HttpGet]
        public RoomMatchRate GetRoomMatchRate(long hotelId, string pRoomCode, long hotelOriId, string otaRoomName)
        {
            return otaCrawlerService.GetRoomMatchRate(hotelId, pRoomCode, hotelOriId, otaRoomName);
        }
        
        [HttpGet]
        public List<PackageMatchReport> GetPackageMatchReport()
        {
            return otaCrawlerService.GetPackageMatchReport();
        }

        [HttpGet]
        public List<RoomMatchRateReport> GetRoomMatchRateReport()
        {
            return otaCrawlerService.GetRoomMatchRateReport();
        }

        [HttpGet]
        public Config GetCrawlerConfig(string code)
        {
            return otaCrawlerService.GetCrawlerConfig(code);
        }

        [HttpGet]
        public string GetCtripApiAccessToken(int type)
        {
            return otaCrawlerService.GetCtripApiAccessToken(type);
        }

        #region 抓取与业务数据处理层相关操作

        [HttpGet]
        public OTAPackageSourceConfig GetOnePackageSourceConfig(int channelId, int isValid)
        {
            return otaCrawlerService.GetOnePackageSourceConfig(channelId, isValid);
        }

        [HttpPost]
        public int InsertUpdatePackageSourceConfig(OTAPackageSourceConfig packageSourceConfig)
        {
            return otaCrawlerService.InsertUpdatePackageSourceConfig(packageSourceConfig);
        }

        [HttpGet]
        public OTAPackageSourceConfig GetPackageSourceConfigByHotelId(int channelId, Int64 hotelId)
        {
            return otaCrawlerService.GetPackageSourceConfigByHotelId(channelId, hotelId);
        }

        /// <summary>
        /// 根据酒店、入住日期获取套餐数据【ctrip api】
        /// </summary>
        /// <param name="hotelId">zmjd hotelid</param>
        /// <param name="checkIn"></param>
        /// <param name="checkOut"></param>
        /// <returns></returns>
        [HttpGet]
        public CrawlerHotel GetPackagesForCtripApi(Int64 hotelId, DateTime checkIn, DateTime checkOut)
        {
            var channelId = 103;
            var otaPackageConfig = otaCrawlerService.GetPackageSourceConfigByHotelId(channelId, hotelId);
            if (otaPackageConfig != null)
            {
                return otaCrawlerService.Ctrip_CrawlHotelPackageForApiV42(otaPackageConfig, checkIn, checkOut, false);
            }
            else
            {
                return new CrawlerHotel();
            }
        }

        /// <summary>
        /// 获取所有OTA套餐（For Ctrip Api）
        /// </summary>
        /// <param name="hotelId"></param>
        /// <param name="checkIn"></param>
        /// <param name="checkOut"></param>
        /// <returns>所有Api返回的套餐，包括不可售的</returns>
        [HttpGet]
        public CrawlerHotel GetAllPackagesForCtripApi(Int64 hotelId, DateTime checkIn, DateTime checkOut)
        {
            var channelId = 103;
            var otaPackageConfig = otaCrawlerService.GetPackageSourceConfigByHotelId(channelId, hotelId);
            if (otaPackageConfig != null)
            {
                //查询每一个酒店的固定日期内的套餐信息
                HotelRoomParams hotelRoomParams = new HotelRoomParams
                {
                    HotelID = otaPackageConfig.HotelOriId.ToString(),
                    MineHotelID = otaPackageConfig.HotelId.ToString(),
                    OtaPackageConfig = otaPackageConfig,
                    CheckIn = checkIn.ToString("yyyy-MM-dd"),
                    CheckOut = checkOut.ToString("yyyy-MM-dd"),
                    OtaType = HJD.OtaCrawlerService.Contract.OtaType.Ctrip,
                    GetAll = true
                };

                return otaCrawlerService.GetCanSellHotelForApiV42(hotelRoomParams);
            }
            else
            {
                return new CrawlerHotel();
            }
        }

        /// <summary>
        /// 是否为中国酒店
        /// </summary>
        /// <param name="hotelId"></param>
        /// <returns></returns>
        [HttpGet]
        public bool HotelInChina(int hotelId)
        {
            return otaCrawlerService.HotelInChina(hotelId);
        }

        #endregion

        #region 抓取任务管理

        [HttpPost]
        public int SavePluginFile(PluginInfo plugin)
        {
            return otaCrawlerService.SavePluginFile(plugin.Code, plugin.LocalPath, plugin.Value);
        }

        [HttpGet]
        public string GetPluginFileTxt(string pluginName, string localPath)
        {
            return otaCrawlerService.GetPluginFileTxt(pluginName, localPath);
        }

        #endregion

        #region 酒店套餐价格报表相关

        [HttpPost]
        public int InsertPackagePriceReportZmjd(PackagePriceV001 pp)
        {
            return otaCrawlerService.InsertPackagePriceReportZmjd(pp);
        }

        [HttpPost]
        public int InsertPackagePriceReportCtrip(PackagePriceV001 pp)
        {
            return otaCrawlerService.InsertPackagePriceReportCtrip(pp);
        }

        #endregion
    }
}
