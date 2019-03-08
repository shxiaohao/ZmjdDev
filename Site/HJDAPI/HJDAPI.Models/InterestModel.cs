using HJD.HotelServices.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.Models
{
    public class InterestModel
    {
        public string Name { get; set; }
        public int districtid { get; set; }
        public float GLat { get; set; }
        public float GLon { get; set; }
        public List<InterestEntity> InterestList;
    }

    public class InterestModel2
    {
        public string Name { get; set; }
        public int districtid { get; set; }
        public float GLat { get; set; }
        public float GLon { get; set; }
        public int TotalHotelNum { get; set; }
        public List<InterestEntity> ThemeInterestList;
        public List<InterestEntity> SightInterestList;
        public List<InterestEntity> HotInterestList;

        public List<SightCategory> SightCategoryList;

        public Advertise AD;
    }

    public class InterestModel3
    {
        public string Name { get; set; }
        public int districtid { get; set; }
        public float GLat { get; set; }
        public float GLon { get; set; }
        public int TotalICONNum { get; set; }
        public int TotalHotelNum { get; set; }
        public Advertise AD { get; set; }
        public List<InterestAlbumICON> ICONList { get; set; }
        public List<InterestEntity> ThemeInterestList { get; set; }
        public List<RecommendedInspectorModel> InspectorList { get; set; }
    }

    [Serializable]
    [DataContract]
    public class InterestAlbumICON : InterestEntity
    {
        [DataMember]
        public string ActionUrl { get; set; }

        [DataMember]
        public SearchHotelListResult HotelListResult { get; set; }
    }

    public class InterestHotelResult
    {
        /// <summary>
        /// 主题相关数据
        /// </summary>
        public List<InterestEntity> interests { get; set; }
        
        /// <summary>
        /// 酒店总数量
        /// </summary>
        public int totalHotelCount { get; set; }
    }

    public class HomeDataModel
    {
        public HomeDataModel()
        {
            AD = new Advertise();
            HotelResult = new RecommendHotelResult();
            InspectorResult = new RecommendInspectorResult();
            //CommentResult = new RecommendCommentResult();
        }

        public Advertise AD { get; set; }

        public RecommendHotelResult HotelResult { get; set; }

        public RecommendInspectorResult InspectorResult { get; set; }

        //public RecommendCommentResult CommentResult { get; set; }
    }

    public class HomeDataModel20
    {
        public HomeDataModel20()
        {
            AD = new Advertise();
            SelectedResort = new Advertise();
            loadH5Url = "";
            BoxData = new PopupBoxData();
        }

        public Advertise AD { get; set; }
        public Advertise SelectedResort { get; set; }

        public string loadH5Url { get; set; }

        public PopupBoxData BoxData { get; set; }
    }

    public class HomePageData30
    {
        public HomePageData30()
        {
            FlashDeals = new List<RoomCouponActivityModel>();
            GroupDeals = new List<RoomCouponActivityModel>();
        }

        /// <summary>
        /// 闪购活动
        /// </summary>
        public List<RoomCouponActivityModel> FlashDeals { get; set; }

        /// <summary>
        /// 组团活动
        /// </summary>
        public List<RoomCouponActivityModel> GroupDeals { get; set; }
    }

    public class RoomCouponActivityModel
    {
        public int activityId { get; set; }
        public string activityTitle { get; set; }
        public int activityType { get; set; }
        public int activityState { get; set; }
        public string packageBrief { get; set; }
        public string hotelName { get; set; }
        public string hotelPicUrl { get; set; }
        public int marketPrice { get; set; }
        public int totalNum { get; set; }
        public int leaveNum { get; set; }
        public string[] labelPics { get; set; }
        public DateTime startSellTime { get; set; }
        public DateTime endSellTime { get; set; }
        public int activityPrice { get; set; }
        public string activityTypeName { get; set; }
    }

    public class RecommendCommentResult
    {
        /// <summary>
        /// 推荐点评区域 宽高比
        /// </summary>
        public double Ratio { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string CommentBlockTitle { get; set; }

        /// <summary>
        /// show slide-comment page Url
        /// </summary>
        public string SlideCommentsUrl { get; set; }

        /// <summary>
        /// show all-comment page url
        /// </summary>
        public string AllCommentsUrl { get; set; }
    }

    public class RecommendCommentListModel
    {
        public RecommendCommentListModel()
        {
            CommentList = new List<RecommendCommentModel>();
        }

        /// <summary>
        /// 块标题
        /// </summary>
        public string BlockTitle { get; set; }

        /// <summary>
        /// 点评数目
        /// </summary>
        public int CommentTotalCount { get; set; }

        /// <summary>
        /// 关注数量
        /// </summary>
        public int FollowingsCount { get; set; }

        /// <summary>
        /// 粉丝数量
        /// </summary>
        public int FollowersCount { get; set; }

        /// <summary>
        /// 推荐酒店的问题
        /// </summary>
        public List<RecommendCommentModel> CommentList { get; set; }
    }

    public class RecommendCommentListQueryParam
    {
        public int start { get; set; }

        public int count { get; set; }
    }

    /// <summary>
    /// 推荐点评数据模型
    /// </summary>
    public class RecommendCommentModel
    {
        /// <summary>
        /// 点评ID
        /// </summary>
        public int CommentID { get; set; }

        /// <summary>
        /// 点评标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 点评内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 点评内容
        /// </summary>
        public float Score { get; set; }

        /// <summary>
        /// 酒店名称
        /// </summary>
        public string HotelName { get; set; }

        /// <summary>
        /// 酒店Id
        /// </summary>
        public int HotelId { get; set; }

        /// <summary>
        /// 酒店照片
        /// </summary>
        public string HotelPicUrl { get; set; }

        /// <summary>
        /// 酒店评分
        /// </summary>
        public float HotelScore { get; set; }

        /// <summary>
        /// 酒店价格
        /// </summary>
        public int HotelPrice { get; set; }

        /// <summary>
        /// 酒店推荐人数
        /// </summary>
        public int HotelRecommendedCount { get; set; }

        /// <summary>
        /// 点评照片
        /// </summary>
        public string PhotoUrl { get; set; }

        /// <summary>
        /// 点评照片数组
        /// </summary>
        public List<string> PhotoUrls { get; set; }

        /// <summary>
        /// 作者昵称
        /// </summary>
        public string NickName { get; set; }

        /// <summary>
        /// 作者userID
        /// </summary>
        public long AuthorUserID { get; set; }

        /// <summary>
        /// 作者头像链接
        /// </summary>
        public string AvatarUrl { get; set; }

        /// <summary>
        /// 身份 品鉴师 候选品鉴师 普通用户
        /// </summary>
        public string RoleDesc { get; set; }

        /// <summary>
        /// 推荐描述
        /// </summary>
        public string RecommendDesc { get; set; }

        /// <summary>
        /// 发表时间描述
        /// </summary>
        public string TimeDesc { get; set; }

        /// <summary>
        /// general = 0, botao = 1, vip = 2, inspector = 3
        /// </summary>
        public int CustomerType { get; set; }

        /// <summary>
        /// 关注按钮 需要知道当前关系
        /// </summary>
        public int followState { get; set; }
    }

    /// <summary>
    /// 4.1版本新首页 推荐酒店
    /// </summary>
    public class RecommendHotelResult
    {
        public RecommendHotelResult()
        {
            DistrictName = "";
            HotelBlockTitle = "";
            HotelList = new List<RecommendHotelItem>();
        }

        public string DistrictName { get; set; }

        public string HotelBlockTitle { get; set; }

        public int HotelTotalCount { get; set; }

        public List<RecommendHotelItem> HotelList { get; set; }
    }

    /// <summary>
    /// 4.1版本新首页 品鉴师块
    /// </summary>
    public class RecommendInspectorResult
    {
        public string InspectorBlockTitle { get; set; }
        public int InspectorTotalCount { get; set; }
        public List<RecommendedInspectorModel> InspectorList { get; set; }
    }

    public class RecommendHotelItem
    {
        /// <summary>
        /// 酒店套餐状态
        /// </summary>
        public int PackageState { get; set; }
        public int Id { get; set; }
        /// <summary>
        /// 酒店名称
        /// </summary>
        public string HotelName { get; set; }
        /// <summary>
        /// 酒店ID
        /// </summary>
        public int HotelID { get; set; }
        /// <summary>
        /// 酒店点评分
        /// </summary>
        public decimal HotelScore { get; set; }
        /// <summary>
        /// 酒店点评数量
        /// </summary>
        public int HotelReviewCount { get; set; }
        /// <summary>
        /// 市场价
        /// </summary>
        public int MarketPrice { get; set; }
        /// <summary>
        /// 套餐优惠价
        /// </summary>
        public int HotelPrice { get; set; }
        /// <summary>
        /// 酒店照片
        /// </summary>
        public string HotelPicUrl { get; set; }
        /// <summary>
        /// 酒店描述语句
        /// </summary>
        public string ADDescription { get; set; }
        /// <summary>
        /// 套餐简介
        /// </summary>
        public string PackageBrief { get; set; }
        /// <summary>
        /// 套餐名称
        /// </summary>
        public string PackageName { get; set; }
        /// <summary>
        /// 套餐ID
        /// </summary>
        public int PID { get; set; }
        /// <summary>
        /// 推荐照片
        /// </summary>
        public string RecommendPicUrl { get; set; }
        /// <summary>
        /// 推荐照片2
        /// </summary>
        public string RecommendPicUrl2 { get; set; }
        /// <summary>
        /// 推荐理由
        /// </summary>
        public string RecomemndWord { get; set; }
        /// <summary>
        /// 推荐理由2
        /// </summary>
        public string RecomemndWord2 { get; set; }
        /// <summary>
        /// 套餐内容
        /// </summary>
        public List<string> packageContent { get; set; }
        /// <summary>
        /// 注意事项
        /// </summary>
        public List<string> packageNotice { get; set; }
        /// <summary>
        /// VIP价格
        /// </summary>
        public int VIPPrice { get; set; }
        /// <summary>
        /// 非VIP价格
        /// </summary>
        public int NotVIPPrice { get; set; }
        /// <summary>
        /// VIP价总价
        /// </summary>
        public int TotalVIPPrice { get; set; }
        /// <summary>
        /// 套餐优惠价总价
        /// </summary>
        public int TotalHotelPrice { get; set; }
        /// <summary>
        /// 最少入住天数
        /// </summary>
        public int DayLimitMin { get; set; }
        /// <summary>
        /// 客户身份 等于0 普通用户； 等于1 铂韬用户；等于2则是会员； 等于3 品鉴师
        /// </summary>
        public int CustomerType { get; set; }
        /// <summary>
        /// 酒店推荐数
        /// </summary>
        public int RecommendCount { get; set; }
        /// <summary>
        /// 头像Url
        /// </summary>
        public string AvatarUrl { get; set; }
        /// <summary>
        /// 跳转链接
        /// </summary>
        public string ActionURL { get; set; }
        /// <summary>
        /// 所在城市ID
        /// </summary>
        public int DistrictId { get; set; }

        /// <summary>
        /// 所在城市名称
        /// </summary>
        public string DistrictName { get; set; }

        /// <summary>
        /// 所在城市英文名称
        /// </summary>
        public string DistrictEName { get; set; }

        /// <summary>
        /// RootName
        /// </summary>
        public string ProvinceName { get; set; }

        /// <summary>
        /// 是否属于中国
        /// </summary>
        public bool InChina { get; set; }

        /// <summary>
        /// 新VIP专享套餐
        /// </summary>
        public bool ForVIPFirstBuy { get; set; }

        /// <summary>
        /// 套餐销售数量
        /// </summary>
        public int PackageOrderCount { get; set; }

        /// <summary>
        /// 是否售完
        /// </summary>
        public bool IsSellOut { get; set; }

        /// <summary>
        /// 最近vip售卖价
        /// </summary>
        public int SellVIPPrice { get; set; }

        /// <summary>
        /// 最近非vip售卖价
        /// </summary>
        public int SellNotVIPPrice { get; set; }
        /// <summary>
        /// 最近售日期
        /// </summary>
        public DateTime SellDate { get; set; }
        /// <summary>
        /// 推荐理由
        /// </summary>
        public CommentTextAndUrl_ex Intro { get; set; }
        /// <summary>
        /// 间夜数
        /// </summary>
        public int NightCount { get; set; }

        public double Lat { get; set; }

        public double Lon { get; set; }

        /// <summary>
        /// 是否分销套餐
        /// </summary>
        public bool IsDistributable { get; set; }

        public int PackageType { get; set; }

        public int DateSelectType { get; set; }

        public PackageAndProductCouponDefineEntity CouponInfo { get; set; }

        /// <summary>
        /// 出发地ID
        /// </summary>
        public int StartDistrictId { get; set; }

        /// <summary>
        /// 出发地
        /// </summary>
        public string StartDistrictName { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int Rank { get; set; }

        /// <summary>
        /// 专辑中套餐标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 分享内容
        /// </summary>
        public string ShareDescription { get; set; }

        /// <summary>
        /// 分享标题
        /// </summary>
        public string ShareTitle { get; set; }


        /// <summary>
        /// 套餐分组号
        /// </summary>
        public string PackageGroupName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal AutoCommission { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public decimal ManualCommission { get; set; }
        /// <summary>
        /// 是否引导成为VIP
        /// </summary>
        public bool NeedVIPGuide { get; set; } 

    }

    public class RecommendPackageDetailResult
    {
        public RecommendPackageDetailResult()
        {
            packageItem = new RecommendHotelItem();
            AddressInfo = new ArrivalDepartureAndAddressInfo();
            CouponInfo = new PackageAndProductCouponDefineEntity();
            GroupSerialList = new List<GroupSerialItem>();
            HotelMapUrl = "";
            HotelTel = "";
        }
        public string ActiviyInfo { get; set; }
        public RecommendHotelItem packageItem { get; set; }

        public List<SameSerialPackageItem> serialPackageList { get; set; }

        public ArrivalDepartureAndAddressInfo AddressInfo { get; set; }

        public PackageAndProductCouponDefineEntity CouponInfo { get; set; }

        public List<GroupSerialItem> GroupSerialList { get; set; }

        public string HotelMapUrl { get; set; }

        public string HotelTel { get; set; }
        public double GLat { get; set; }
        public double GLon { get; set; }
    }

    public class SameSerialPackageItem
    {
        /// <summary>
        /// 套餐Id
        /// </summary>
        public int pId { get; set; }
        /// <summary>
        /// 房型Id
        /// </summary>
        public int roomTypeId { get; set; }
        /// <summary>
        /// 房型名称
        /// </summary>
        public string roomTypeName { get; set; }
        /// <summary>
        /// 房型描述
        /// </summary>
        public string roomDesc { get; set; }
        /// <summary>
        ///序列号
        /// </summary>
        public string serialNo { get; set; }

        public decimal NoVipPrice { get; set; }

        public decimal VipPrice { get; set; }
    }

    [DataContract]
    public class UserHomeDataParam:BaseParam
    {
        [DataMember]
        public float userLat { get; set; }
        [DataMember]
        public float userLng { get; set; }
        [DataMember]
        public int geoStype { get; set; }
        [DataMember]
        public int districtID { get; set; }
        [DataMember]
        public int curUserID { get; set; }
    }

    [DataContract]
    public class RecommendedInspectorModel
    {
        [DataMember]
        public string NickName {get;set;}
        [DataMember]
        public string AvatarUrl { get; set; }
        [DataMember]
        public string CoverPicUrl { get; set; }
        [DataMember]
        public int FollowFollowingState { get; set; }
        [DataMember]
        public string Brief { get; set; }
        [DataMember]
        public long UserID { get; set; }
        [DataMember]
        public int FollowersCount { get; set; }
        [DataMember]
        public int FollowingsCount { get; set; }
    }
    
    [DataContract]
    public class RecommendedInspectorParam
    {
        [DataMember]
        public int start { get; set; }
        [DataMember]
        public int count { get; set; }
        [DataMember]
        public int curUserID { get; set; }
    }

    public class AttractionModel
    {    
        public List<InterestEntity> SightInterestList;

        public List<SightCategory> SightCategoryList;
    }

    public class ThemeModel
    {
        public ThemeModel() { RowList = new List<RowItem>(); }
        public string Name { get; set; }
        public int districtid { get; set; }
        public float GLat { get; set; }
        public float GLon { get; set; }

        public List<RowItem> RowList;
    }

    public class RowItem
    {
        public RowItem() { themes = new List<ThemeItem>(); AD = new Advertise(); }
        public List<ThemeItem> themes { get; set; }
        public Advertise AD { get; set; }
    }

    public class ThemeItem
    {
       public   InterestEntity theme { get; set; }
       public int colspan { get; set; }
    }
    
    [Serializable]
    [DataContract]
    public class Advertise
    {
        public Advertise() { ADList = new List<ADItem>(); }

        /// <summary>
        /// banner显示宽高比
        /// </summary>
        [DataMember]
        public double Ratio { get; set; }
        
        /// <summary>
        /// banner列表
        /// </summary>
        [DataMember]
        public List<ADItem> ADList { get; set; }
    }

    [Serializable]
    [DataContract]
    public class ADItem
    {
        public ADItem()
        {
            ADTitle = "";
            ADURL = "";
            ActionURL = "";
        }

        /// <summary>
        /// 1.图片 2.网页
        /// </summary>
        [DataMember]
        public int ADShowType { get; set; }


        /// <summary>
        /// 0：所有人可见 1：普通用户可见 2：VIP可见
        /// </summary>
        [DataMember]
        public int ADUserShowType { get; set; }
        /// <summary>
        /// 广告图片链接 banner显示用
        /// </summary>
        [DataMember]
        public string ADURL { get; set; }

        /// <summary>
        /// 点击banner后 App跳转还是打开url
        /// </summary>
        [DataMember]
        public string ActionURL { get; set; }

        /// <summary>
        /// ADTitle
        /// </summary>
        [DataMember]
        public string ADTitle { get; set; }
    }

    [Serializable]
    [DataContract]
    public class StrategyADItem:ADItem
    {
        /// <summary>
        /// 离当前位置的距离 单位米
        /// </summary>
        [DataMember]
        public int Distance { get; set; }

        /// <summary>
        /// 攻略地区的名称
        /// </summary>
        [DataMember]
        public string DistrictName { get; set; }

        /// <summary>
        /// 攻略地区ID
        /// </summary>
        [DataMember]
        public int DistrictID { get; set; }
    }

    [Serializable]
    [DataContract]
    public class StrategyADResult
    {
        public StrategyADResult() { ADList = new List<StrategyADItem>(); }

        /// <summary>
        /// XX及周边 周边攻略
        /// </summary>
        [DataMember]
        public string AroundDistrictName { get; set; }

        /// <summary>
        /// banner列表
        /// </summary>
        [DataMember]
        public List<StrategyADItem> ADList { get; set; }
    }

    [Serializable]
    [DataContract]
    public class SightCategory
    {
        public SightCategory() { }

        [DataMember]
        public int ID { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public List<int> InterestID { get; set; }
    }

    [Serializable]
    [DataContract]
    public class DistrictSearchParam
    {
        [DataMember]
        public int DistrictID { get; set; }

        [DataMember]
        public string DistrictName { get; set; }

        [DataMember]
        public float Lon { get; set; }

        [DataMember]
        public float Lat { get; set; }
    }

    [Serializable]
    [DataContract]
    public class PreLoadAppDataParam : DistrictSearchParam
    {
        [DataMember]
        public long UserID { get; set; }
    }

    [Serializable]
    [DataContract]
    public class PreLoadAppData
    {
        [DataMember]
        public SearchHotelAdv SearchHotelAdv { get; set; }
    }

    [Serializable]
    [DataContract]
    public class SearchHotelAdv
    {
        /// <summary>
        /// 酒店HotelID
        /// </summary>
        [DataMember]
        public int HotelID { get; set; }
        /// <summary>
        /// 酒店名称
        /// </summary>
        [DataMember]
        public string HotelName { get; set; }
        /// <summary>
        /// 酒店背景图片
        /// </summary>
        [DataMember]
        public string HotelBGP { get; set; }
        /// <summary>
        /// 跳转链接
        /// </summary>
        [DataMember]
        public string ActionUrl { get; set; }
    }
        
    [Serializable]
    [DataContract]
    public class PackageAlbumDetail
    {
        /// <summary>
        /// 专辑详情
        /// </summary>
        [DataMember]
        public PackageAlbumsEntity albumEntity { get; set; }
        
        /// <summary>
        /// 专辑下套餐列表
        /// </summary>
        [DataMember]
        public List<RecommendHotelItem> packageList { get; set; }

        [DataMember]
        public CommentShareModel shareModel { get; set; }
    }

    //[Serializable]
    //[DataContract]
    //public class IntroEntity : CommentTextAndUrl
    //{
    //    public IntroEntity()
    //    {
    //        Item = new List<string>();
    //    }
    //    [DataMember]
    //    public List<string> Item { get; set; }
    //}

}