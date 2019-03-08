using HJD.HotelServices.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.Models
{
    public class HotelItem5 : HotelBaseItem
    {
        public HotelItem5()
        {
            RoomComment = new RoomTypeComment();
            EntertainmentList = new List<FeaturedCommentEntity>();
            Facilities = new List<HotelFacilityEntity>();
            RoomTypeData = new List<RoomTypeCommentItem>();
        }
        /// <summary>
        /// 房间评论老字段
        /// </summary>
        public RoomTypeComment RoomComment { get; set; }
        /// <summary>
        /// 娱乐设施字段
        /// </summary>
        public List<FeaturedCommentEntity> EntertainmentList { get; set; }
        /// <summary>
        /// 设施老字段
        /// </summary>
        public List<HotelFacilityEntity> Facilities { get; set; }

        public List<RoomTypeCommentItem> RoomTypeData { get; set; }

        public DateTime PriceCinDate { get; set; }

        public DateTime PriceCouDate { get; set; }
    }

    public class RoomTypeCommentItem
    {
        /// <summary>
        /// 是否推荐
        /// </summary>
        [DataMember]
        public bool IsRecommend { get; set; }
        /// <summary>
        /// 房型默认大类ID 19
        /// 用于查看全部关于房间的点评
        /// </summary>
        [DataMember]
        public int DefaultCategoryID { get; set; }
        /// <summary>
        /// 房型相关标签数据
        /// </summary>
        public List<FeaturedCommentEntity> Tags { get; set; }
        /// <summary>
        /// 房型名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 房型照片链接
        /// </summary>
        public List<string> Pics { get; set; }
        /// <summary>
        /// 房型点评数量
        /// </summary>
        [DataMember]
        public int CommentCount { get; set; }
        /// <summary>
        /// 房间面积(平方米)
        /// </summary>
        [DataMember]
        public string RoomArea { get; set; }
        /// <summary>
        /// 楼层
        /// </summary>
        [DataMember]
        public string Floor { get; set; }
        /// <summary>
        /// 床型大小信息
        /// </summary>
        [DataMember]
        public string BedSize { get; set; }
    }

    [Serializable]
    [DataContract]
    public class ArrivalDepartureAndAddressInfo
    {
        [DataMember]
        public string Address { get; set; }
        [DataMember]
        public HotelInfo ArrivalAndDeparture { get; set; }
    }

    [Serializable]
    [DataContract]
    public class RoomTypeComment
    {
        [DataMember]
        public string whotelComment { get; set; }
        [DataMember]
        public int defaultCategoryID { get; set; }
        [DataMember]
        public List<FeaturedCommentEntity> singleComments { get; set; }
    }

    [Serializable]
    [DataContract]
    public class PackagePriceInfo
    {
        [DataMember]
        public string Tag { get; set; }

        [DataMember]
        public decimal MinPrice { get; set; }

        [DataMember]
        public decimal VipPrice { get; set; }
        
        [DataMember]
        public DateTime CheckIn { get; set; }
        
        [DataMember]
        public DateTime CheckOut { get; set; }
        
        [DataMember]
        public int PriceType { get; set; }
        
        [DataMember]
        public List<PackageItem> PackageList { get; set; }
        
        [DataMember]
        public List<OTAInfo2> OTAList { get; set; }
    }

    public class HotelBaseItem
    {
        public HotelBaseItem()
        {
            HotelName = "";
            districtName = "";
            InterestName = "";
            ShareDesc = "";
            ShareLink = "";
            ShareTitle = "";
            packagePriceURL = "";
            ReBuildInfo = "";
            OpenYear = "";
            Tel = "";
            HotelMapUrl = "";
            //RelCommentAuthorType = "";

            Pics = new List<string>();
            Featrues = new List<string>();
            Intro = new HotelInfo();
            Interest = new HotelInfo();
            PackageList = new List<PackageItem>();

            FoodComment = new List<FeaturedCommentEntity>();
            FeatureList = new List<FeaturedCommentEntity>();
            RestaurentList = new List<HotelRestaurantEntity>();

            RecommendHotelList = new List<PackageItem>();
            AddressInfo = new ArrivalDepartureAndAddressInfo();

            Advantages = new List<string>();
            Disadvantages = new List<string>();

            OTAList = new List<OTAInfo2>();

            SightList = new List<HotelSightEntity>();

            FacilityList = new List<FeaturedCommentEntity>();

            RelComments = new List<HotelRelComment>();

            RelPicData = new HotelRelPicData();
        }

        public int HotelID { get; set; }
        public string HotelName { get; set; }
        public List<string> Advantages { get; set; }
        public List<string> Disadvantages { get; set; }
        public int districtID { get; set; }
        public string districtName { get; set; }
        public int Star { get; set; }
        public int InterestID { get; set; }
        public string InterestName { get; set; }
        /// <summary>
        /// 酒店分享的描述内容
        /// </summary>
        public string ShareDesc { get; set; }
        /// <summary>
        /// 酒店分享的标题
        /// </summary>
        public string ShareTitle { get; set; }
        /// <summary>
        /// 酒店分享的链接 跳转到网页酒店详情页
        /// </summary>
        public string ShareLink { get; set; }
        public HotelInfo Intro { get; set; }
        public HotelInfo Interest { get; set; }
        public List<string> Pics;
        public int PicCount { get; set; }
        public decimal MinPrice { get; set; }
        public int PriceType { get; set; }
        public string packagePriceURL { get; set; }
        public string Currency { get { return "￥"; } }
        public List<string> Featrues;
        public decimal Score { get; set; }
        public int ReviewCount { get; set; }
        public string Tel { get; set; }
        public double GLat { get; set; }
        public double GLon { get; set; }
        public string OpenYear { get; set; }
        public string ReBuildInfo { get; set; }

        public List<HotelRestaurantEntity> RestaurentList { get; set; }
        public List<FeaturedCommentEntity> FeatureList { get; set; }
        public List<FeaturedCommentEntity> FoodComment { get; set; }

        /// <summary>
        /// 交通
        /// </summary>
        public ArrivalDepartureAndAddressInfo AddressInfo { get; set; }
        /// <summary>
        /// 推荐酒店
        /// </summary>
        public List<PackageItem> RecommendHotelList { get; set; }
        /// <summary>
        /// 套餐价格数组
        /// </summary>
        public List<PackageItem> PackageList { get; set; }
        /// <summary>
        /// OTA跳转数组
        /// </summary>
        public List<OTAInfo2> OTAList { get; set; }
        /// <summary>
        /// 酒店地图Url
        /// </summary>
        public string HotelMapUrl { get; set; }
        /// <summary>
        /// 是否在中国
        /// </summary>
        public bool IsInChina { get; set; }        
        
        ///// <summary>
        ///// 点评作者类型 品酒师or好友
        ///// </summary>
        //public string RelCommentAuthorType { get; set; }
        
        /// <summary>
        /// 品鉴师点评数量
        /// </summary>
        public int InspectorCommentCount { get; set; }
        /// <summary>
        /// 涉及该酒店的点评数量
        /// </summary>
        public int FollowingCommentCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<HotelSightEntity> SightList { get; set; }
        /// <summary>
        /// 设施点评标签块
        /// </summary>
        public List<FeaturedCommentEntity> FacilityList { get; set; }
        /// <summary>
        /// 酒店关联点评列表
        /// </summary>
        public List<HotelRelComment> RelComments { get; set; }
        /// <summary>
        /// 酒店关联照片 5.9版本废掉了
        /// </summary>
        public HotelRelPicData RelPicData { get; set; }
    }

    public class HotelRelPicData
    {
        public HotelRelPicData()
        {
            officialPics = new List<HotelPicInfo>();
            customerPics = new List<HotelPicInfo>();
        }
        /// <summary>
        /// 官方照片数量
        /// </summary>
        public int officialPicCount { get; set; }
        /// <summary>
        /// 客人照片数量
        /// </summary>
        public int customerPicCount { get; set; }
        public List<HotelPicInfo> officialPics { get; set; }
        public List<HotelPicInfo> customerPics { get; set; }
    }

    public class HotelPicInfo
    {
        public string picUrl { get; set; }
        public string picSmallUrl { get; set; }
        public string roomType{get;set;}
        public string roomComment { get; set; }
        public string author { get; set; }
        public string date { get; set; }
    }
}