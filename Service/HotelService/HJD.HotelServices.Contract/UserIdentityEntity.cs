using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace HJD.HotelServices.Contracts
{
    //[Serializable]
    //[DataContract]
    //public class UserIdentityEntity
    //{
    //    [DataMember]
    //    public UserIdentityType UserIdentity { get; set; }

    //    [DataMember]
    //    public int Count { get; set; }
    //}

    [Serializable]
    [DataContract]
    public enum UserIdentityType
    {
        [EnumMember]
        All = 0,
        [EnumMember]
        Business = 1,
        [EnumMember]
        Couple = 2,
        [EnumMember]
        Family = 3,
        [EnumMember]
        Friend = 4,
        [EnumMember]
        Single = 5,
        [EnumMember]
        Child = 6,
    }

    // Define an extension method in a non-nested static class.
    public static class UserIdentityTypeExtensions
    {
        public static string[] ns = new string[] { "全部", "商务", "夫妻/情侣", "家庭游", "朋友/同事", "独自", "带孩子游" };
        public static string GetDisplayName(this UserIdentityType type)
        {
            return ns[(int)type];
        }
    }
}
