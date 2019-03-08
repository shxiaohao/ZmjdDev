using HJD.CouponService.Contracts.Entity;
using HJD.HotelServices.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.Models
{
    [Serializable]
    [DataContract]
    public class CouponActivityDetailModel
    {
        [DataMember]
        public TopNPackageItem package { get; set; }

        [DataMember]
        public CouponActivityEntity activity { get; set; }

        /// <summary>
        /// 0 已结束；1 进行中
        /// </summary>
        [DataMember]
        public int activityOpenState { get; set; }

        /// <summary>
        /// 地区名称
        /// </summary>
        [DataMember]
        public string districtName { get; set; }

        [DataMember]
        public List<HJD.HotelManagementCenter.Domain.PItemEntity> DailyItems { get; set; }

        [DataMember]
        public List<HJD.HotelManagementCenter.Domain.PItemEntity> NoticeItems { get; set; }

        [DataMember]
        public List<ExchangeCouponAddInfoItem> HasBoughtItems { get; set; }
    }

    [Serializable]
    [DataContract]
    public class ExchangeCouponAddInfoItem
    {
        /// <summary>
        /// 真名
        /// </summary>
        [DataMember]
        public string TrueName { get; set; }
        /// <summary>
        /// 人员构成 几大几小
        /// </summary>
        [DataMember]
        public string PersonnelStructure { get; set; }
        /// <summary>
        /// 儿童几个几岁
        /// </summary>
        [DataMember]
        public string ChildrenAge { get; set; }

        /// <summary>
        /// 券数量(待用)
        /// </summary>
        [DataMember]
        public int CouponCount { get; set; }
    }

    [Serializable]
    [DataContract]
    public class ExchangeCouponPackageInfo
    {
        public ExchangeCouponPackageInfo()
        {
            exchangeInfo = new ExchangeCouponEntity();
            packageInfo = new PackageInfoEntity();
            exchangeDateType = 0;
            canExchangeDates = new List<DateTime>();
            isAllowMultiRoom = false;
        }

        [DataMember]
        public PackageInfoEntity packageInfo { get; set; }

        [DataMember]
        public ExchangeCouponEntity exchangeInfo { get; set; }

        /// <summary>
        /// 0: 周末和平日 1: 平日 2: 周末
        /// </summary>
        [DataMember]
        public int exchangeDateType { get; set; }

        /// <summary>
        /// 是否允许选多间
        /// </summary>
        [DataMember]
        public bool isAllowMultiRoom { get; set; }
        
        /// <summary>
        /// 指定房券兑换的日期 只有这些日期可用
        /// </summary>
        [DataMember]
        public List<DateTime> canExchangeDates { get; set; }
    }

    [Serializable]
    [DataContract]
    public class ExchangeRoomOrderConfirmResult
    {
        [DataMember]
        public string Message { get; set; }

        /// <summary>
        /// 单位元  -1：需要提示有问题，不能提交兑换
        /// </summary>
        [DataMember]
        public decimal AddMoneyAmount { get; set; }

        /// <summary>
        /// 是否需要补汇款  是否需要提示
        /// </summary>
        [DataMember]
        public bool IsNeedAddMoney { get; set; }

        /// <summary>
        /// 使用兑换券数量
        /// </summary>
        [DataMember]
        public int UseCouponCount { get; set; }


    }
    
    #region 博涛房券信息model
    [Serializable]
    [DataContract]
    public class ThirdPartyProductItem
    {
        /// <summary>
        /// 产品唯一标识
        /// </summary>
        [DataMember]
        public string ProductCode { get; set; }
        /// <summary>
        /// 最后更新时间 周末酒店产品数据有变动时会更新该时间，初值是创建时间
        /// </summary>
        [DataMember]
        public string LastEditTime { get; set; }
        /// <summary>
        /// 产品图片链接
        /// </summary>
        [DataMember]
        public string PicUrl { get; set; }
        /// <summary>
        /// 产品详情页链接
        /// </summary>
        [DataMember]
        public string DetailUrl { get; set; }
        /// <summary>
        /// 酒店名称
        /// </summary>
        [DataMember]
        public string HotelName { get; set; }
        /// <summary>
        /// 套餐名称
        /// </summary>
        [DataMember]
        public string PackageName { get; set; }
        /// <summary>
        /// 酒店所在城市
        /// </summary>
        [DataMember]
        public string CityName { get; set; }
        /// <summary>
        /// 酒店地址
        /// </summary>
        [DataMember]
        public string Address { get; set; }
        /// <summary>
        /// 总数
        /// </summary>
        [DataMember]
        public int TotalCount { get; set; }
        /// <summary>
        /// 可买数量
        /// </summary>
        [DataMember]
        public int AvailableCount { get; set; }
        /// <summary>
        /// 过期日期
        /// </summary>
        [DataMember]
        public DateTime ExpirationDate { get; set; }
        /// <summary>
        /// 原价
        /// </summary>
        [DataMember]
        public int MarketPrice { get; set; }
        /// <summary>
        /// 折扣价
        /// </summary>
        [DataMember]
        public int DiscountPrice { get; set; }
        /// <summary>
        /// 产品类型
        /// </summary>
        [DataMember]
        public ThirdPartyProductType ProductType { get; set; }
        /// <summary>
        /// 套餐简介
        /// </summary>
        [DataMember]
        public string PackageBrief { get; set; }
        /// <summary>
        /// 产品详情介绍
        /// </summary>
        [DataMember]
        public string ProdcutDetail { get; set; }
        /// <summary>
        /// 产品当前状态 0: 表示未删除 1: 表示已删除
        /// </summary>
        [DataMember]
        public bool IsDel { get; set; }
    }

    [Serializable]
    [DataContract]
    public class ThirdPartyProductDataResult
    {
        public ThirdPartyProductDataResult()
        {
            ErrorInfo = new BasePostResponse(){
                data = "",
                errorcode = "",
                errormsg = ""
            };
            Items = new List<ThirdPartyProductItem>();
        }

        [DataMember]
        public List<ThirdPartyProductItem> Items { get; set; }

        [DataMember]
        public BasePostResponse ErrorInfo { get; set; }
    }

    public enum ThirdPartyProductType
    {
        hotel = 1,
        roomcoupon = 2
    }

    #endregion

}