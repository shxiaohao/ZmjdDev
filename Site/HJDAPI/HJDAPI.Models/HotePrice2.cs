using HJD.HotelServices.Contracts;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace HJDAPI.Models
{
    public class HotelPrice2
    {
        public int HotelID { get; set; }

        public string Name { get; set; }

        public bool InChina { get; set; }

        public int DayLimitMin { get; set; } //套餐可选天数限止，0：无限止， 1：只能选一天 2：只能选两天 3：。。。。

        public int DayLimitMax { get; set; } //套餐可选天数限止，0：无限止， 1：只能选一天 2：只能选两天 3：。。。。

        public DateTime CheckIn { get; set; }

        public DateTime CheckOut { get; set; }

        public List<PackageInfoEntity> Packages { get; set; }

        public List<PackageInfoEntity> JLPackages { get; set; }

        public List<OTAInfo2> OTAList { get; set; }

        /// <summary>
        /// 客户身份
        /// </summary>
        public int CustomerType { get; set; }

        //public int MinHotelPeople { get; set; }

        //public int MaxHotelPeople { get; set; }

        //public List<string> CardType { get; set; }
    }

    public class HotelPrice3
    {
        public HotelPrice3()
        {
            Name = "";
            PackageGroups = new List<PackageGroupItem>();
            Packages = new List<PackageInfoEntity>();
            JLPackages = new List<PackageInfoEntity>();
            OTAList = new List<OTAInfo2>();
            //Suggest = new BuyMembershipSuggest();
            HotelBenefitPolicy = new CommentTextAndUrl();
        }

        public int HotelID { get; set; }

        public string Name { get; set; }

        public int DayLimitMin { get; set; } //套餐可选天数限止，0：无限止， 1：只能选一天 2：只能选两天 3：。。。。

        public int DayLimitMax { get; set; } //套餐可选天数限止，0：无限止， 1：只能选一天 2：只能选两天 3：。。。。

        public DateTime CheckIn { get; set; }

        public DateTime CheckOut { get; set; }
        /// <summary>
        /// 控制前端展示个数
        /// </summary>
        public int ShowPackageGroupCount { get; set; }

        public List<PackageGroupItem> PackageGroups { get; set; }

        public List<PackageInfoEntity> Packages { get; set; }

        public List<PackageInfoEntity> JLPackages { get; set; }

        public List<OTAInfo2> OTAList { get; set; }

        /// <summary>
        /// 客户身份
        /// </summary>
        public int CustomerType { get; set; }

        ///// <summary>
        ///// 用户身份
        ///// </summary>
        //public BuyMembershipSuggest Suggest { get; set; }

        /// <summary>
        /// 是否有非周末酒店的套餐
        /// </summary>
        public bool HaveNotZMJDPackages { get; set; }

        /// <summary>
        /// 酒店可用券政策
        /// </summary>
        public CommentTextAndUrl HotelBenefitPolicy { get; set; }


        /// <summary>
        /// 入住天数
        /// </summary>
        public int NightCount { get; set; }
        ///// <summary>
        ///// 最少入住人数
        ///// </summary>
        //public int MinHotelPeople { get; set; }

        ///// <summary>
        ///// 最多入住人数
        ///// </summary>
        //public int MaxHotelPeople { get; set; }

        ///// <summary>
        ///// 证件类型
        ///// </summary>
        //public List<string> CardType { get; set; }
    }

    //public class BuyMembershipSuggest{

    //    public BuyMembershipSuggest()
    //    {
    //        ActionUrl = "";
    //        Text = "";
    //    }

    //    /// <summary>
    //    /// 跳转链接
    //    /// </summary>
    //    public string ActionUrl { get; set; }

    //    /// <summary>
    //    /// 建议购买会员的内容
    //    /// </summary>
    //    public string Text { get; set; }
    //}

    public class PackageGroupItem
    {
        public PackageGroupItem()
        {
            Brief = "";
            SerialNo = "";
            SeriaNoDesc = "";
            GroupItemLables = new List<string>();
        }
        public int ShowPackageGroupCount { get; set; }
        public string SerialNo { get; set; }

        public string Brief { get; set; }
        public List<string> RoomCodes { get; set; }

        public List<string> GroupItemLables { get; set; }
        public bool HasForVIPFirstBuy { get; set; }
        //系列号下套餐销售状态 0 表示可售 1 查看可售日  2 售完
        public int SeriaNoState { get; set; }
        public string SeriaNoDesc { get; set; }
        public Dictionary<string, PackageInfoEntity> dicRoomTypePackage { get; set; }
    }

    public class HotelPackageCalendar
    {
        public HotelPackageCalendar()
        {
            DayItems = new List<PDayItem>();
        }

        /// <summary>
        /// 最小值
        /// </summary>
        public int DayLimitMin { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int DayLimitMax { get; set; }

        public List<PDayItem> DayItems { get; set; }

    }

    /// <summary>
    /// 
    /// </summary>
    public class HotelPackagePriceCalendarEntity
    {
        /// <summary>
        /// 日历对象
        /// </summary>
        public HotelPackageCalendar Calendar { get; set; }

        public DateTime CheckIn { get; set; }

        public DateTime CheckOut { get; set; }

        public int NightCount { get; set; }

        public int TotalPrice { get; set; }

        public int TotalVipPrice { get; set; }

        public int TotalNormalPrice { get; set; }

    }
}