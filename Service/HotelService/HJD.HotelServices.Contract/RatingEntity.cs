using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using HJD.Framework.Entity;

namespace HJD.HotelServices.Contracts
{
    [Serializable]
    [DataContract]
    [DefaultColumn]
    public class RatingEntity
    {
        /// <summary>
        /// 酒店ID
        /// </summary>
        [DataMemberAttribute()]
        public int HotelID { get; set; }

        [DataMember]
        public RatingType Rating { get; set; }

        [DataMember]
        public UserIdentityType UserIdentity { get; set; }

        [DataMember]
        public int Count { get; set; }
    }

    [Serializable]
    [DataContract]
    public enum RatingType
    {
        [EnumMember]
        All = 0,
        [EnumMember]
        Best = 5,
        [EnumMember]
        Better = 4,
        [EnumMember]
        Good = 3,
        [EnumMember]
        Normal = 2,
        [EnumMember]
        Terrible = 1,
        [EnumMember]
        GoodGrade = 13,
        [EnumMember]
        NormalGrade = 12,
        [EnumMember]
        BadGrade = 11
    }

    // Define an extension method in a non-nested static class.
    public static class RatingTypeExtensions
    {
        public static string[] ns = new string[] { "全部", "很差", "较差", "一般", "较好", "很好" };
        public static string GetDisplayName(this RatingType type)
        {
            return ns[(int)type];
        }
    }
}
