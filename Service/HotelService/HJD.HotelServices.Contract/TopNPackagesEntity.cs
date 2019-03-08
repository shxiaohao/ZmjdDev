using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HJD.HotelServices.Contracts
{
    [System.SerializableAttribute()]
    [System.Runtime.Serialization.DataContractAttribute()]
    [HJD.Framework.Entity.DefaultColumnAttribute()]
    public class TopNPackagesEntity
    {
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public int ID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public int HotelID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public int PID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public int Rank { get; set; }

        /// <summary>
        /// 计算出来的排序顺序
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public int CalcRank { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public long Creator { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public long Updator { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public DateTime UpdateTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public bool IsValid { get; set; }

        /// <summary>
        /// 推荐理由
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public string RecomemndWord { get; set; }

        /// <summary>
        /// 推荐理由2
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public string RecomemndWord2 { get; set; }

        /// <summary>
        /// 推荐图片
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public string RecomendPicShortNames { get; set; }
        /// <summary>
        /// 推荐图片2
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public string RecomendPicShortNames2 { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public string HotelName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public string PackageName { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public string Title { get; set; }

        /// <summary>
        /// 市场价
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public int MarketPrice { get; set; }

        /// <summary>
        /// 封面照片
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public string CoverPicSUrl { get; set; }

        /// <summary>
        /// 专辑ID
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public int AlbumsID { get; set; }

        /// <summary>
        /// 专辑名称
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public string AlbumsName { get; set; }
        /// <summary>
        /// 套餐结束时间
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public DateTime EndDate { get; set; }

        /// <summary>
        /// 套餐简介
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public string Brief { get; set; }

        /// <summary>
        /// 新VIP专享套餐
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public bool ForVIPFirstBuy { get; set; } 
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public int VIPFirstPayDiscount { get; set; } 
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public int DateSelectType { get; set; }

        /// <summary>
        /// 出发地ID
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public int StartDistrictId { get; set; }

        /// <summary>
        /// 出发地
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public string StartDistrictName { get; set; }

        /// <summary>
        /// 套餐分组号
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public string PackageGroupName { get; set; }
    }

    [System.SerializableAttribute()]
    [System.Runtime.Serialization.DataContractAttribute()]
    [HJD.Framework.Entity.DefaultColumnAttribute()]
    public class TopNPackagesResult
    {
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public string Message { get; set; }


        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public int Success { get; set; }
    }

    [System.SerializableAttribute()]
    [System.Runtime.Serialization.DataContractAttribute()]
    [HJD.Framework.Entity.DefaultColumnAttribute()]
    public class TopNPackageItem
    {
        /// <summary>
        /// 套餐状态
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public int PackageState { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public string HotelName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public Int32 HotelID { get; set; }

        /// <summary>
        /// 酒店点评分
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public decimal HotelScore { get; set; }

        /// <summary>
        /// 酒店点评数量
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public Int32 HotelReviewCount { get; set; }

        /// <summary>
        /// 所在城市ID
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public int DistrictId { get; set; }

        /// <summary>
        /// 所在城市名称
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public string DistrictName { get; set; }

        /// <summary>
        /// 所在城市英文名称
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public string DistrictEName { get; set; }

        /// <summary>
        /// RootName
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public string ProvinceName { get; set; }

        /// <summary>
        /// 是否属于中国
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public bool InChina { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public string PackageName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public Int32 PackageID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public string PackageBrief { get; set; }

        /// <summary>
        /// 套餐价格
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public List<TypeAndPrice> PackagePrice { get; set; }

        /// <summary>
        /// 市场价
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public int MarketPrice { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public string RecomemndWord { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public string RecomemndWord2 { get; set; }

        /// <summary>
        /// 推荐图片
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public string RecomendPicShortNames { get; set; }
        /// <summary>
        /// 推荐图片2
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public string RecomendPicShortNames2 { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public string Title { get; set; }

        /// <summary>
        /// 封面照片
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public string CoverPicSUrl { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public List<string> PicUrls { get; set; }

        /// <summary>
        /// 专辑ID
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public int AlbumsID { get; set; }

        /// <summary>
        /// 专辑名称
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public string AlbumsName { get; set; }

        /// <summary>
        /// 最少入住天数
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public int DayLimitMin { get; set; }

        /// <summary>
        /// 新VIP专享套餐
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public bool ForVIPFirstBuy { get; set; }


    /// <summary>
        /// 新VIP专享套餐
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public int VIPFirstPayDiscount { get; set; }
    
        /// <summary>
        /// 日历选择模式
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public int DateSelectType { get; set; }


        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public int PackageType { get; set; }

        /// <summary>
        /// 套餐是否可售
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public bool IsSellOut { get; set; }

        /// <summary>
        /// 是否是分销酒店
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public bool IsDistributable { get; set; }

        /// <summary>
        /// 出发地ID
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public int StartDistrictId { get; set; }

        /// <summary>
        /// 出发地
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public string StartDistrictName { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public int Rank { get; set; }


        /// <summary>
        /// 分享内容
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public string ShareDescription { get; set; }

        /// <summary>
        /// 分享标题
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public string ShareTitle { get; set; }

        /// <summary>
        /// 套餐分组号
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public string PackageGroupName { get; set; }

        /// <summary>
        /// 是否引导成为VIP
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        [System.ComponentModel.Description("是否引导成为VIP")]
        public bool NeedVIPGuide { get; set; } 
    }

    [System.SerializableAttribute()]
    [System.Runtime.Serialization.DataContractAttribute()]
    [HJD.Framework.Entity.DefaultColumnAttribute()]
    public class TypeAndPrice
    {
        /// <summary>
        /// 当前价格对应的日期
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public DateTime NowPriceDay { get; set; }

        /// <summary>
        /// 套餐ID
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public int PID { get; set; }

        /// <summary>
        /// 套餐价格类型 0对应平日;6和7对应周末;8指定日;1-7对应星期一到星期天  -1表示VIP价格
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public int Type { get; set; }

        /// <summary>
        /// 单位元
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public int Price { get; set; }

        /// <summary>
        /// 价格信息补充
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public string AddInfo { get; set; }

        /// <summary>
        /// 价格类型名称
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public string TypeName { get; set; }
    }

    [Serializable]
    [DataContract(IsReference = true)]
    public class TopNPackageSearchParam
    {
        [DataMember]
        public TopNPackageSearchFilter Filter { get; set; }
        [DataMember]
        public int Start { get; set; }
        [DataMember]
        public float lat { get; set; }
        [DataMember]
        public float lng { get; set; }
        [DataMember]
        public int geoScopeType { get; set; }
        [DataMember]
        public int PageSize { get; set; }
        [DataMember]
        public int districtID { get; set; }
        
        [DataMember]
        public Dictionary<string, string> Sort { get; set; }


    }

    [Serializable]
    [DataContract(IsReference = true)]
    public class TopNPackageSearchFilter
    {
        public TopNPackageSearchFilter() { NeedNotSale = false; }
        [DataMember]
        public string HotelName { get; set; }
        [DataMember]
        public string PackageName { get; set; }
        [DataMember]
        public int HotelID { get; set; }
        [DataMember]
        public int PackageID { get; set; }
        [DataMember]
        public bool? IsValid { get; set; }
        [DataMember]
        public int AlbumsID { get; set; }
        /// <summary>
        /// 等于1限制rank取值在1-20之间
        /// </summary>
        [DataMember]
        public int NeedRankRange { get; set; }
         
        /// <summary>
        /// 是否需要列出不售卖的套餐
        /// </summary>
        [DataMember]
        public bool NeedNotSale { get; set; }

        /// <summary>
        /// 组号数组
        /// 如果长度为空不传则不参与查询
        /// </summary>
        [DataMember]
        public IEnumerable<string> GroupNo { get; set; }
        /// <summary>
        /// 专辑类型
        /// </summary>
        [DataMember]
        public int Type { get; set; }

        /// <summary>
        /// 多个districtId用英文逗号分隔
        /// </summary>
        [DataMember]
        public string DistrictIds { get; set; }

        /// <summary>
        /// 展示爆款套餐（新vip只能购买一次的套餐）
        /// </summary>
        [DataMember]
        public int VipFirstBuyPackage { get; set; }

        /// <summary>
        /// 日期
        /// </summary>
        [DataMember]
        public string DateStr { get; set; }

        /// <summary>
        /// 目的地id
        /// </summary>
        [DataMember]
        public int GoToDistrictId { get; set; }
        /// <summary>
        /// 出发地id
        /// </summary>
        [DataMember]
        public int StartDistrictId { get; set; }
    }

    [System.SerializableAttribute()]
    [System.Runtime.Serialization.DataContractAttribute()]
    [HJD.Framework.Entity.DefaultColumnAttribute()]
    public class BotaoSettlementEntity
    {
        /// <summary>
        /// 订单Id
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public long orderId { get; set; }

        /// <summary>
        /// 结算类型 退款还是扣款
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public BotaoSettleType settleType { get; set; }

        /// <summary>
        /// 订单类型 房券还是酒店产品
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public BotaoOrderType orderType { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public DateTime createTime { get; set; }

        /// <summary>
        /// 单位分
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public int price { get; set; }
    }

    /// <summary>
    /// cancel为1 confirm为2 重新跳整价格绑定reconfirm 3
    /// </summary>
    [System.SerializableAttribute()]
    [System.Runtime.Serialization.DataContractAttribute()]
    public enum BotaoSettleType
    {
        [EnumMember]
        cancel = 1,
        [EnumMember]
        confirm = 2,
        [EnumMember]
        reconfirm = 3
    }

    /// <summary>
    /// 产品类型 1是酒店 2是房券
    /// </summary>
    [System.SerializableAttribute()]
    [System.Runtime.Serialization.DataContractAttribute()]
    public enum BotaoOrderType
    {
        [EnumMember]
        hotel = 1,
        [EnumMember]
        roomcoupon = 2
    }

    [System.SerializableAttribute()]
    [System.Runtime.Serialization.DataContractAttribute()]
    [HJD.Framework.Entity.DefaultColumnAttribute()]
    public class BotaoSettleOrderEntity
    {
        /// <summary>
        /// 订单ID
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public long OrderID { get; set; }

        /// <summary>
        /// 订单金额 单位元
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public decimal Amount { get; set; }

        /// <summary>
        /// 支付金额 单位分
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public float Price { get; set; }

        /// <summary>
        /// 支付凭证信息
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public string PayToken { get; set; }

        /// <summary>
        /// 结算类型
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public int SettleType { get; set; }
    }

    [System.SerializableAttribute()]
    [System.Runtime.Serialization.DataContractAttribute()]
    [HJD.Framework.Entity.DefaultColumnAttribute()]
    public class PackageAlbumsEntity
    {
        /// <summary>
        /// ID
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public int ID { get; set; }

        /// <summary>
        /// 排序号
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public int Rank { get; set; }

        /// <summary>
        /// 名称 主标题
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public string Name { get; set; }

        /// <summary>
        /// 下级标题
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public string SubTitle { get; set; }

        /// <summary>
        /// 完整封面图片
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public string CompleteCoverPicSUrl { get; set; }

        /// <summary>
        /// 封面图片
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public string CoverPicSUrl { get; set; }

        /// <summary>
        /// 标签图片
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public string LabelPicSUrl { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public string Description { get; set; }

        /// <summary>
        /// 是否发布
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public bool IsValid { get; set; }

        /// <summary>
        /// 日期
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public DateTime Date { get; set; }

        /// <summary>
        /// 作者名称
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public string Author { get; set; }

        /// <summary>
        /// 主题 类型
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public int Type { get; set; }

        /// <summary>
        /// 组号 多个以英文半角逗号相隔
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public string GroupNo { get; set; }

        /// <summary>
        /// ShowSortNo
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public int ShowSortNo { get; set; }

        /// <summary>
        /// ICON
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public string ICON { get; set; }

    }

    [System.SerializableAttribute()]
    [System.Runtime.Serialization.DataContractAttribute()]
    [HJD.Framework.Entity.DefaultColumnAttribute()]
    public class RelPackageAlbumsEntity : PackageAlbumsEntity
    {
        /// <summary>
        /// 酒店Id
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public int HotelId { get; set; }

        /// <summary>
        /// 套餐Id
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public int PId { get; set; }

        /// <summary>
        /// topNPackage表的ID
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public int TNPId { get; set; }
    }

    [System.SerializableAttribute()]
    [System.Runtime.Serialization.DataContractAttribute()]
    [HJD.Framework.Entity.DefaultColumnAttribute()]
    public class TopScoreHotelEntity
    {
        /// <summary>
        /// Id
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public int Id { get; set; }

        /// <summary>
        /// 酒店Id
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public int HotelId { get; set; }

        /// <summary>
        /// 酒店名称
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public string HotelName { get; set; }

        /// <summary>
        /// 排序分数 可人工修改
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public float RankScore { get; set; }

        /// <summary>
        /// 推荐次数
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public int RecommendCount { get; set; }

        /// <summary>
        /// 酒店打分 非zmjd点评打分
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public float HotelScore { get; set; }

        /// <summary>
        /// 计算得分
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public float CalcScore { get; set; }

        /// <summary>
        /// 酒店封面照片
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public string HotelPicUrl { get; set; }
    }

    [System.SerializableAttribute()]
    [System.Runtime.Serialization.DataContractAttribute()]
    [HJD.Framework.Entity.DefaultColumnAttribute()]
    public class BsicSearchParam
    {
        /// <summary>
        /// 开始取数据的位置
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public int start { get; set; }

        /// <summary>
        /// 需要取回的数量
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public int count { get; set; }

        /// <summary>
        /// 纬度
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public float lat { get; set; }

        /// <summary>
        /// 经度
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public float lng { get; set; }

        /// <summary>
        /// 地区ID
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public int districtid { get; set; }

        /// <summary>
        /// userId
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public long userId { get; set; }
    }
}