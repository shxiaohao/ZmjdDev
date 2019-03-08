using HJD.AccountServices.Entity;
using HJD.Framework.Interface;
using HJD.Framework.WCF;
using HJD.CouponService.Contracts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Security;
using System.Xml;
using HJD.OtaCrawlerService.Contract.Ctrip.Hotel;
using HJD.OtaCrawlerServices.Contract;
using HJD.OtaCrawlerService.Contract.Params;
using HJD.OtaCrawlerService.Contract;

namespace HJDAPI.Controllers.Adapter
{
    public class OtaCrawlerAdapter
    {
        public static ICacheProvider LocalCache5Min = CacheManagerFactory.Create("DynamicCacheFor5Min");
        //public static ICommService commService = ServiceProxyFactory.Create<ICommService>("ICommService");
        public static IOtaCrawlerService otaCrawlerService = ServiceProxyFactory.Create<IOtaCrawlerService>("IOtaCrawlerService");

        private string logFile = Configs.LogPath + string.Format("OtaCrawlerLog_{0}{1}{2}.txt", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

        public CrawlerHotel GetCtripCanSellHotel(HotelRoomParams hotelRoomParams)
        {
            var hotel = new CrawlerHotel();
            hotelRoomParams.OtaType = OtaType.Ctrip;
            hotel = otaCrawlerService.GetCanSellHotel(hotelRoomParams);
            return hotel;
        }
    }
}
