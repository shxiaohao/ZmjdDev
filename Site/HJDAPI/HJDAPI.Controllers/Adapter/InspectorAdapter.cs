using HJD.AccountServices.Entity;
using HJD.Framework.Interface;
using HJD.Framework.WCF;
using HJD.HotelServices.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.Controllers.Adapter
{
    public class InspectorAdapter
    {
        public static IHotelService HotelService = ServiceProxyFactory.Create<IHotelService>("BasicHttpBinding_IHotelService");
        public static ICacheProvider LocalCache10Min = CacheManagerFactory.Create("DynamicCacheFor10Min");//抢券活动缓存 十分钟

        public int InsertOrUpdateInspectorHotel(InspectorHotel eh)
        {
            return HotelService.InsertOrUpdateInspectorHotel(eh);
        }

        public int InsertOrUpdateInspectorRefHotel(InspectorRefHotel eh)
        {
            return HotelService.InsertOrUpdateInspectorRefHotel(eh);
        }

        public List<InspectorRefHotel> GetEvaluationerHotelList(long evaHotelID, long userID)
        {
            return HotelService.GetInspectorRefHotelList(evaHotelID, userID);
        }

        public List<InspectorHotel> GetEvaluationHotelsList(InspectorHotelSearchParam param, out int count)
        {
            return HotelService.GetInspectorHotelList(param, out count);
        }

        public static InspectorHotel GetInspectorHotelById(long inspectorHotelId)
        {
            return HotelService.GetInspectorHotelById(inspectorHotelId);
        }

        /// <summary>
        /// 获取品鉴酒店的缓存数据
        /// </summary>
        /// <param name="inspectorHotelId"></param>
        /// <returns></returns>
        public static int GetInspectorHotelLeftNum(int inspectorHotelId)
        {
            return LocalCache10Min.GetData<InspectorHotel>(string.Format("Inspector_GetInspectorHotelLeftNum:{0}", inspectorHotelId), () =>
            {
                return GetInspectorHotelById(inspectorHotelId);//
            }).LeaveCount;
        }

      
    }
}