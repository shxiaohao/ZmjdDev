using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HJD.HotelServices.Contracts
{
    public class HotelServiceEnums
    {
         public enum    HotelCacheType
         {
             Hotel,
             Hotel3,
             HotelInfoExCacheKey
         }
        public enum ConsumeUserPointsBizType
        {
            WriteComment = 1, //	写点评奖励
            FreeInspectHotel = 2, //代表的是免费品鉴需要消耗的积分
            RefundFreeInspectHotel = 3,  //取消兑换

            InviteReward = 4,//邀请奖励积分
            Guess = 5,//	猜一猜酒店积分
            suggest = 6,//	用户反馈
            activitypass = 7,//	品鉴师活动通过
            pointsexpiration = 8,//	积分过期
            toprecommend = 9,//	首页推荐
            uploaduseravatar = 10,//上传头像
            share = 11,//分享
            regist = 12,//	注册
            orders = 13,//消费获得积分
            Modify2015 = 14,//	更正2015过期项 
            Coupon = 15, //券使用积分
            NewVIP = 16,   //新VIP
            UpgradeVIP = 17   //VIP升级
        }

       public enum SellState
       {
           canSell = 1,
           cannotSell = 2,
           notFitForDayLimited = 3  //不符合售卖天数限止，所以不可售。 如一些套餐需要连续三天购买
       }

        public enum PackageSellSate
        {
            canSell = 1,
            cannotSell = 2
        }

        /// <summary>
        /// 套餐状态
        /// </summary>
        public enum PackageSate
        {
            Pending = 0 , //待发布
            Publish = 1,  // 已发布
            Offline = 2 //下线
        }

        public enum PackageType
        {
            HotelPackage = 1,
            OTA = 2,
            JLPackage = 3,
            CtripPackage = 4,
            CtripPackageForHotel = 5,
            CtripPackageByApi = 6,
            TopTownPackage = 7,
        }

        /// <summary>
        /// 周末酒店自己打包的套餐的套餐类型
        /// </summary>
        public enum WHPackageType
        {
            Normal = 0, //普通套餐
            AirHotel = 1 //机酒套餐
        }

        /// <summary>
        /// 供应商类型
        /// </summary>
        public enum SupplierType
        {
            RoomSupplier = 1,  //房间供应商
            ProductSupplier =2  //其它产品供应商 如：门票、晚餐等
        }


        public enum PricePolicyType
        {
            Default = 0,
            VIP = 1,
            Botao = 2
        }


        public enum PackagePayType
        {
            FrontPay = 1, //前台现付
            Warrant = 2, //担保
            PrePay = 3 //预付
        }

        [Serializable]
        [DataContract]
        public enum HotelDistrictRange
        {
            [EnumMember]
            JZH = 1, //江浙沪
            [EnumMember]
            NotJZHInChina = 2, //国内非江浙沪
            [EnumMember]
            NotInChina = 3, //国外
        }

        public enum EnumCardType
        {
            身份证 = 1,
            护照 = 2,
            户口簿 = 3,
            港澳通行证 = 4,
            台胞证 = 5,
            其他 = 10
        }
    }
}
