using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using HJD.Framework.Entity;

namespace HJD.HotelServices.Contracts
{
    /// <summary>
    /// 品牌筛选条件
    /// </summary>
    [Serializable]
    [DataContract]
    [DefaultColumn]
    public class ClassBrandEntity
    {
        /// <summary>
        /// 品牌编号
        /// </summary>
        [DataMember]
        public int Brand { get; set; }

        /// <summary>
        /// 品牌名称
        /// </summary>
        [DataMember]
        public string BrandName { get; set; }
    }

    //[Serializable]
    //[DataContract]
    //public enum BrandType
    //{
    //    [EnumMember]
    //    Ramada = 1,
    //    [EnumMember]
    //    HowardJohnson = 2,
    //    [EnumMember]
    //    Days = 3,
    //    [EnumMember]
    //    Super8 = 4,
    //    [EnumMember]
    //    Novotel = 5,
    //    [EnumMember]
    //    Sofitel = 6,
    //    [EnumMember]
    //    Pullman = 7,
    //    [EnumMember]
    //    Ibis = 8,
    //    [EnumMember]
    //    Mercure = 9,
    //    [EnumMember]
    //    Hilton = 10,
    //    [EnumMember]
    //    Conrad = 11,
    //    [EnumMember]
    //    Sheraton = 12,
    //    [EnumMember]
    //    FourPoints = 13,
    //    [EnumMember]
    //    Regis = 14,
    //    [EnumMember]
    //    Westin = 15,
    //    [EnumMember]
    //    JWMarriott = 16,
    //    [EnumMember]
    //    RitzCarlton = 17,
    //    [EnumMember]
    //    Marriott = 18,
    //    [EnumMember]
    //    Renaissance = 19,
    //    [EnumMember]
    //    Courtyard = 20,
    //    [EnumMember]
    //    Holiday = 21,
    //    [EnumMember]
    //    HolidayInnExpress = 22,
    //    [EnumMember]
    //    CrownePlaza = 23,
    //    [EnumMember]
    //    Intercontinental = 24,
    //    [EnumMember]
    //    GrandHyatt = 25,
    //    [EnumMember]
    //    HyattRegency = 26,
    //    [EnumMember]
    //    ParkHyatt = 27,
    //    [EnumMember]
    //    Wyndham = 28,
    //    [EnumMember]
    //    Aloft = 29,
    //    [EnumMember]
    //    Amy = 30,
    //    [EnumMember]
    //    HomeInns = 31,
    //    [EnumMember]
    //    YiLin = 32
    //}

    //public static class BrandTypeExtensions
    //{
    //    public static string[] BrandName = new string[] { "", "华美达", "豪生", "戴斯", "速8", "诺富特"
    //        , "索菲特", "铂尔曼", "宜必思", "美居", "希尔顿"
    //        , "康莱德", "喜来登", "福朋", "瑞吉", "威斯汀"
    //        , "JW万豪", "丽思卡尔顿", "万豪", "万丽", "万怡"
    //        , "假日", "智选假日", "皇冠假日", "洲际", "君悦"
    //        , "凯悦", "柏悦", "温德姆", "雅乐轩", "艾美"
    //        , "如家快捷", "逸林" };
    //    public static string[] BrandEnName = new string[] { "", "Ramada", "Howard Johnson", "Days", "Super 8", "Novotel"
    //        , "Sofitel", "Pullman", "Ibis", "Mercure", "Hilton"
    //        , "Conrad", "Sheraton", "Four Points", "Regis", "Westin"
    //        , "JW Marriott", "Ritz-Carlton", "Marriott", "Renaissance", "Courtyard"
    //        , "Holiday", "Holiday Inn Express", "Crowne Plaza", "Intercontinental", "Grand Hyatt"
    //        , "Hyatt Regency", "Park Hyatt", "Wyndham", "Aloft", "Amy"
    //        , "Home Inns", "Yi Lin" };

    //    public static string GetDisplayName(this BrandType brandType)
    //    {
    //        return BrandName[(int)brandType];
    //    }

    //    public static string GetDisplayEnName(this BrandType brandType)
    //    {
    //        return BrandEnName[(int)brandType];
    //    }
    //}
}
