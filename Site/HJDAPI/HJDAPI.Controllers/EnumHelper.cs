using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.Controllers
{
    public class EnumHelper
    {


        public enum CommOrderState
        {
            log = 0,
            paied = 1,
            cancel = 3
        }
        public enum InterestID
        {
            SpecialDealHotelInterest = 100002, //特惠精选
            AllHotelInterest = 110000
        }

        public enum ActivityID
        {
            VIP199_EVER = 100250,
            VIP599 = 100398,
            VIP199 = 100399,
            VIP3M = 100433,//	金牌3月
            VIP6M = 100434,	//金牌6月
            VIP199_NR = 100929,//金牌不可退
            TTGY = 100944  //天天果园

        }

        public enum GeoScopeType
        {
            District = 1,   //地理范围类型 1：目的地
            AroundDistrict = 2, //2：目的地周边  
            AroundCoordinate = 3 // 3：坐标附近
        }

        //HJD.HotelServices.Contracts.HotelServiceEnums.WHPackageType
        //public enum PackageType
        //{
        //    Normal = 0, //普通套餐
        //    AirHotel  = 1 //机酒套餐
        //}

        //public enum WeiXinChannelCode
        //{
        //    UNKNOW =0,
        //    周末酒店订阅号 = 1, //  1周末酒店  
        //    周末酒店服务号 = 2,// 2周末酒店订阅号  
        //    尚旅游订阅号 = 3  // 3尚旅游
        //}

        /// <summary>
        /// 浏览记录 2：酒店，3：sku
        /// </summary>
        public enum BrowsingRecordType
        {
            hotel = 2,
            sku = 3
        }
        ///// <summary>
        ///// 酒店订单
        ///// 0：普通酒店订单
        ///// 1：机酒订单
        ///// 2：游轮订单
        ///// </summary>
        //public enum HotelOrderType
        //{
        //    HotelOrder = 0,
        //    JiJiuHotelOrder = 1,
        //    YouLunHotelOrder = 2
        //}
        /// <summary>
        /// 酒店订单
        /// 0：普通酒店订单
        /// 1：机酒订单
        /// 2：游轮订单
        /// 房券订单
        /// 14：美食
        /// 20：玩乐
        /// 15：房券
        /// 28：遛娃
        /// </summary>
        public enum OrderType
        {
            AllOrder = -1,
            HotelOrder = 0,
            JiJiuHotelOrder = 1,
            YouLunHotelOrder = 2,
            LiuWaCouponOrder = 28,
            FoodCouponOrder = 14,
            PlayCouponOrder = 20,
            RoomCouponOrder = 15
        }

      public static   List<EnumHelper.OrderType> CouponOrderTypeList = new List<EnumHelper.OrderType>() { 
                        EnumHelper.OrderType.FoodCouponOrder, 
                        EnumHelper.OrderType.LiuWaCouponOrder, 
                        EnumHelper.OrderType.PlayCouponOrder , 
                        EnumHelper.OrderType.RoomCouponOrder 
                    };

      public static List<EnumHelper.OrderType> PackageOrderTypeList = new List<EnumHelper.OrderType>() { 
                        EnumHelper.OrderType.HotelOrder, 
                        EnumHelper.OrderType.JiJiuHotelOrder, 
                        EnumHelper.OrderType.YouLunHotelOrder 
                    };

        public enum RetailProductType
        {
            HotelPackage = 0,
            JiJiuPackage = 1,
            YouLunPackage = 2,
            FoodCoupon = 14,
            PlayCoupon = 20,
            RoomCoupon = 15
        }
        
        /// <summary>
        /// 团状态
        /// </summary>
        public enum GroupState
        {
            [Description("拼团中")]
            Progress = 0,
            [Description("拼团成功")]
            Success = 1,
            [Description("拼团失败")]
            Fail = 2,
            [Description("未支付")]
            NoPay = 3,
            [Description("已取消")]
            Cancel = 4
        }
        /// <summary>
        /// 获取枚举文字描述
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="i"></param>
        /// <returns></returns>
        public static string GetEnumDescription<TEnum>(int i)
        {
            string result = "";
            Type typeFromHandle = typeof(TEnum);
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary = GetEnumDictionary(typeFromHandle);
            foreach (KeyValuePair<string, string> current in dictionary)
            {
                if (!string.IsNullOrWhiteSpace(GetEnumValue<TEnum>(current.Key, i)))
                {
                    result = current.Value;
                    break;
                }
            }
            return result;
        }
        private static Dictionary<string, string> GetEnumDictionary(Type enumType)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            FieldInfo[] fields = enumType.GetFields();
            FieldInfo[] array = fields;
            for (int i = 0; i < array.Length; i++)
            {
                FieldInfo fieldInfo = array[i];
                if (fieldInfo.FieldType.IsEnum)
                {
                    object[] customAttributes = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
                    dictionary.Add(fieldInfo.Name, ((DescriptionAttribute)customAttributes[0]).Description);
                }
            }
            return dictionary;
        }
        private static string GetEnumValue<TEnum>(string name, int i)
        {
            string result = "";
            if (name == Enum.GetName(typeof(TEnum), i))
            {
                result = i.ToString();
            }
            return result;
        }
    }
}
