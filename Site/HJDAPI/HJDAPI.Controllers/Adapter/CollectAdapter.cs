using HJD.Framework.WCF;
using HJDAPI.Models;
using PersonalServices.Contract;
using PersonalServices.Contracts.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.Controllers
{
    public class CollectAdapter
    {
        public static IPersonalService personalService = ServiceProxyFactory.Create<IPersonalService>("basicHttpBinding_IPersonalService");

        public static CollectOptionResult Add(CollectParam param)
        {
            return personalService.AddCollect(param);
        }

        //remove collect 支持批量取消收藏
        public static CollectOptionResult Remove(CollectParam param)
        {
            return personalService.RemoveCollect(param);
        }

        //get collect 由搜索条件返回需要的收藏列表
        public static List<long> GetCollectIdList(long userID)
        {
            CollectOptionResult result = personalService.GetCollectIdList(new CollectParam() { UserID = userID });
            if (result.CollectHotelIDs != null && result.CollectHotelIDs.Count != 0)
            {
                return result.CollectHotelIDs;
            }
            else {
                return new List<long>();
            }
        }

        public static List<FavouriteHotel> GetCollectList(CollectParam param)
        {
            return personalService.GetCollectList(param);
        }
        public static List<FavouriteHotel> GetPageCollectList(CollectParam param)
        {
            return personalService.GetPageCollectList(param); 
        }

        public static bool IsCollect(long UserID,int HotelID)
        {
            List<long> hotelIds = GetCollectIdList(UserID);
            if (hotelIds != null && hotelIds.Count != 0)
            {
                return hotelIds.Contains(HotelID);
            }
            else
            {
                return false;
            }
        }
    }
}