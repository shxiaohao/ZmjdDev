using HJD.Framework.WCF;
using HJD.HotelManagementCenter.IServices;
using HJD.HotelPrice.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.Controllers.Adapter
{
    public  class ServiceList
    {

        public static HJD.HotelServices.Contracts.IHotelService HotelService = ServiceProxyFactory.Create<HJD.HotelServices.Contracts.IHotelService>("BasicHttpBinding_IHotelService");

        public static IHotelPriceService PriceService = ServiceProxyFactory.Create<IHotelPriceService>("BasicHttpBinding_IHotelPriceService");

        public static IActivityService ActivityService = ServiceProxyFactory.Create<IActivityService>("BasicHttpBinding_IActivityService");
    }
}
