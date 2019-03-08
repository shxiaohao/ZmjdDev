using HJD.HotelServices.Contracts;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
namespace HJDAPI.Models
{
    public class HotelItem4
    {
        public HotelItem4()
        {
            Pics = new List<string>();
            Featrues = new List<string>();
            Intro = new HotelInfo();
            Interest = new HotelInfo();
            SightList = new List<HotelSightEntity>();
            Food = new HotelInfo();
            RestaurentList = new List<HotelRestaurantEntity>();
            Facilities = new List<HotelFacilityEntity>();
            PackageList = new List<PackageItem>();

            FeatureList = new List<FeaturedCommentEntity>();
            RoomComment = new  List<FeaturedCommentEntity>();
            EntertainmentList = new List<FeaturedCommentEntity>();
            FoodComment = new  List<FeaturedCommentEntity>();
        }
        public int HotelID { get; set; }
        public string Name { get; set; }
        public int districtID { get; set; }
        public string districtName { get; set; }
        public string InterestName { get; set; }
        public string ShareDesc { get; set; }
        public List<string> Pics;
        public int PicCount { get; set; }
        public decimal MinPrice { get; set; }
        public int PriceType { get; set; }
        public string packagePriceURL { get; set; }
        public string Currency { get { return "￥"; } }
        public List<string> Featrues;
        public decimal Score { get; set; }
        public int ReviewCount { get; set; }
        public HotelInfo Intro { get; set; }
        public HotelInfo Interest { get; set; }
        public List<HotelSightEntity> SightList { get; set; }
        public HotelInfo Food { get; set; }
        public HotelInfo Environment { get; set; }
        public HotelInfo RoomFacilities { get; set; }
        public HotelInfo ArrivalAndDeparture { get; set; }
        public List<HotelRestaurantEntity> RestaurentList { get; set; }
        public List<HotelFacilityEntity> Facilities { get; set; }
        public string Tel { get; set; }
        public string Address { get; set; }
        public double GLat { get; set; }
        public double GLon { get; set; }
        public int InterestID { get; set; }
        public int Star { get; set; }
        public string OpenYear { get; set; }
        public string ReBuildInfo { get; set; }
        public List<PackageItem> PackageList { get; set; }
        public List<FeaturedCommentEntity> FeatureList { get; set; }
        public List<FeaturedCommentEntity> RoomComment { get; set; }
        public List<FeaturedCommentEntity> EntertainmentList { get; set; }
        //public List<SightCommentItem> SightCommentList { get; set; }
        public List<FeaturedCommentEntity> FoodComment { get; set; }
    }

    public class PackageItem
    {
        public PackageItem()
        {
            PackageLables = new List<string>();
            PackageUrl = "";
        }
        public int PID{get;set;}
        public string Title { get; set; }
        public string Brief{get;set;}
        public int Price { get; set; }
        public int VIPPrice { get; set; }
        public int HotelID { get; set; }
        public string HotelName { get; set; }
        public string PicUrl { get; set; }
        public int InterestID { get; set; }
        public int ChannelID { get; set; }

        /// <summary>
        /// 套餐icon
        /// </summary>
        public List<string> PackageLables { get; set; }

        /// <summary>
        /// 套餐连接
        /// </summary>
        public string PackageUrl { get; set; }

        /// <summary>
        /// 几晚
        /// </summary>
        public int NightCount { get; set; }

        /// <summary>
        /// VIP专享
        /// </summary>
        public bool ForVIPFirstBuy { get; set; }


        /// <summary>
        /// 套餐内容
        /// </summary>
        public List<string> packageContent { get; set; }


        /// <summary>
        /// 注意事项
        /// </summary>
        public List<string> packageNotice { get; set; }



    }

    public class SightCommentItem
    {
        public int sight { get; set; }
        public string Comment { get; set; }
    }
}