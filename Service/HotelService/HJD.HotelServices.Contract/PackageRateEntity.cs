using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HJD.HotelServices.Contracts
{
    [Serializable]
    [DataContract]
    public class PackageRateEntity
    {
        public PackageRateEntity() { DailyList = new List<PackageDailyRateEntity>(); }

        [DataMember]
        public decimal AutoCommission { get; set; }

        [DataMember]
        public decimal ManualCommission { get; set; }

        [DataMember]
        public int HotelID { get; set; }
        [DataMember]
        public int PID { get; set; }
        [DataMember]
        public int Price { get; set; }

        [DataMember]
        public int VIPPrice { get; set; }

        [DataMember]
        public int ManaualVIPPrice { get; set; }

        [DataMember]
        public int NotVIPPrice { get; set; }
        [DataMember]
        public int SettlePrice { get; set; }

        [DataMember]
        public int Rebate { get; set; }

        [DataMember]
        public int ActiveRebate { get; set; }

        [DataMember]
        public int CanUseCashCoupon { get; set; }

        [DataMember]
        public int CanUseCashCouponForBoTao { get; set; }

        [DataMember]
        public int CashCoupon { get; set; }

        [DataMember]
        public int PayType { get; set; }

         [DataMember]
        public DateTime startDate { get; set; }
         [DataMember]
         public DateTime endDate { get; set; }

         [DataMember]
         public List<PackageDailyRateEntity> DailyList { get; set; }


    }
    [Serializable]
    [DataContract]
    public class PackageDailyRateEntity
    {
        public PackageDailyRateEntity() { ItemList = new List<PackageRateItemEntity>(); }

        [DataMember]
        public decimal AutoCommission { get; set; }

        [DataMember]
        public decimal ManualCommission { get; set; }

        [DataMember]
        public DateTime Day { get; set; }

        [DataMember]
        public int Price { get; set; }

        [DataMember]
        public int VIPPrice { get; set; }


        [DataMember]
        public int NotVIPPrice { get; set; }

        [DataMember]
        public int SettlePrice { get; set; }

        [DataMember]
        public int PayType { get; set; }

        [DataMember]
        public int Rebate { get; set; }

        [DataMember]
        public int ActiveRebate { get; set; }

        [DataMember]
        public int CanUseCashCoupon { get; set; }

        [DataMember]
        public int CanUseCashCouponForBoTao { get; set; }

        [DataMember]
        public int CashCoupon { get; set; }

        [DataMember]
        public List<PackageRateItemEntity> ItemList { get; set; }
    }

    [Serializable]
    [DataContract]
    public class PackageRateItemEntity
    {
        /// <summary>
        /// 房间供应商类型 包房 酒店
        /// </summary>
        [DataMember]
        public int RoomSupplierType { get; set; }
        /// <summary>
        /// 供应商类型 产品or房间
        /// </summary>
        [DataMember]
        public int SupplierType { get; set; }
        [DataMember]
        public int SupplierID { get; set; }
        [DataMember]
        public int ProductID { get; set; }
        [DataMember]
        public int Price { get; set; }


        [DataMember]
        public int VIPPrice { get; set; }

        [DataMember]
        public int NotVIPPrice { get; set; }

        [DataMember]
        public int SettlePrice { get; set; }

        [DataMember]
        public int PayType { get; set; }

        [DataMember]
        public int Rebate { get; set; }

        [DataMember]
        public int ActiveRebate { get; set; }

        [DataMember]
        public int CanUseCashCoupon { get; set; }

        [DataMember]
        public int CanUseCashCouponForBoTao { get; set; }

        [DataMember]
        public int CashCoupon { get; set; }

        [DataMember]
        public decimal AutoCommission { get; set; }

        [DataMember]
        public decimal ManualCommission { get; set; }
    }
}
