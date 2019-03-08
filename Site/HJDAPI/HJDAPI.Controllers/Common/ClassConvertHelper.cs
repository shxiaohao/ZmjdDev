using HJDAPI.Models;
using PersonalServices.Contract;
using PersonalServices.Contracts.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.Controllers.Common
{
    public class ClassConvertHelper
    {
        public static List<CollectParamItem> Convert(List<CollectParamItemModel> list)
        {
            List<CollectParamItem> list2 = new List<CollectParamItem>();
            if (list != null && list.Count != 0)
            {
                list.ForEach(i => list2.Add(new CollectParamItem()
                {
                    HotelID = i.HotelID,
                    InterestID = i.InterestID, 
                    Price = i.Price, 
                    ThemeID = i.ThemeID 
                }));
            }
            return list2;
        }
    }
}
