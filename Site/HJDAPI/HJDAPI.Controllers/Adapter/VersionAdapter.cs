using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HJD.Framework.WCF;
using HJD.HotelServices.Contracts;
using HJD.Framework.Interface;

namespace HJDAPI.Controllers.Adapter
{
    public class VersionAdapter 
    {

        public static ICacheProvider LocalCache = CacheManagerFactory.Create("DynamicCacheForType2");

        static IHotelService hotelService = ServiceProxyFactory.Create<IHotelService>("BasicHttpBinding_IHotelService");
          
        /// <summary>
        /// 景区酒店数据版本
        /// </summary>
        /// <returns></returns>
       public Dictionary<string, string> GetLatestAppVersion()
        {
            return LocalCache.GetData<Dictionary<string, string>>("LatestAppVersion", () =>
            {
                List<CommDictEntity> cdeList = hotelService.GetCommDictList(601);
                Dictionary<string, string> dic = new Dictionary<string, string>();
                foreach (CommDictEntity cde in cdeList)
                {
                    dic.Add(cde.DicValue, cde.Descript);
                }

                return dic;
            });
        }

          /// <summary>
        /// 周末酒店数据版本
        /// </summary>
        /// <returns></returns>
       public Dictionary<string, string> GetLatestZMJDDataVersion()
        {
            return LocalCache.GetData<Dictionary<string, string>>("LatestZMJDDataVersion", () =>
            {
                List<CommDictEntity> cdeList = hotelService.GetCommDictList(604);
                Dictionary<string, string> dic = new Dictionary<string, string>();
                foreach (CommDictEntity cde in cdeList)
                {
                    dic.Add(cde.DicValue, cde.Descript);
                }

                return dic;
            });
        }

        /// <summary>
        /// 周末酒店Android版本信息
        /// </summary>
        /// <returns></returns>
       public Dictionary<string, string> GetZMJDAndroidVersionInfo()
       {
           return LocalCache.GetData<Dictionary<string, string>>("ZMJDAndroidVersionInfo", () =>
           {
               List<CommDictEntity> cdeList = hotelService.GetCommDictList(602);
               Dictionary<string, string> dic = new Dictionary<string, string>();
               foreach (CommDictEntity cde in cdeList)
               {
                   dic.Add(cde.DicValue, cde.Descript);
                   if(cde.DicValue=="note")
                   {
                       dic.Add("node", cde.Descript);
                   }
               }

               return dic;
           });
       }

       /// <summary>
       /// 情侣酒店数据版本
       /// </summary>
       /// <returns></returns>
       public Dictionary<string, string> GetLatestZMJDLoveDataVersion()
       {
           return GetCommDict(606);
       }

       /// <summary>
       /// 情侣酒店Android版本信息
       /// </summary>
       /// <returns></returns>
       public Dictionary<string, string> GetZMJDLoveAndroidVersionInfo()
       {
           return GetCommDict(605);
       }
        
        /// <summary>
        /// 景区酒店Android版本信息
        /// </summary>
        /// <returns></returns>
       public Dictionary<string, string> GetAttractionHotelAndroidVersionInfo()
       {
           return LocalCache.GetData<Dictionary<string, string>>("GetAttractionHotelAndroidVersionInfo", () =>
           {
               List<CommDictEntity> cdeList = hotelService.GetCommDictList(603);
               Dictionary<string, string> dic = new Dictionary<string, string>();
               foreach (CommDictEntity cde in cdeList)
               {
                   dic.Add(cde.DicValue, cde.Descript);
               }

               return dic;
           });
       }


       public Dictionary<string, string> GetCommDict(int typeid)
       {
           return LocalCache.GetData<Dictionary<string, string>>("CommDict" + typeid, () =>
           {
               List<CommDictEntity> cdeList = hotelService.GetCommDictList(typeid);
               Dictionary<string, string> dic = new Dictionary<string, string>();
               foreach (CommDictEntity cde in cdeList)
               {
                   dic.Add(cde.DicValue, cde.Descript);
               }

               return dic;
           });
       }

    }
}
