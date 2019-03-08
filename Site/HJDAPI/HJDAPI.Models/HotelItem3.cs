using HJD.HotelServices.Contracts;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
namespace HJDAPI.Models
{
    public class HotelItem3
    {
        public HotelItem3()
        {
            Pics = new List<string>();
            Featrues = new List<string>();
            Intro = new HotelInfo();
            Interest = new HotelInfo();
            SightList = new List<HotelSightEntity>();
            Food = new HotelInfo();
            RestaurentList = new List<HotelRestaurantEntity>();
            Facilities = new List<HotelFacilityEntity>();
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
    }

    public class Item
    {
        public string content;
        public string pic;
    }

    public class HotelInfo
    {
        public HotelInfo() { 
            Items = new List<Item>();
            Description = "";
        }
        public string Description;
        public List<Item> Items;

        public int CategoryID { get; set; }
    }


}