using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace HJD.HotelServices.Contracts
{
    /// <summary>
    /// 酒店类别
    /// </summary>
    [Serializable]
    [DataContract]
    public enum HotelType
    {
        /// <summary>
        /// 酒店
        /// </summary>
        [EnumMember]
        HHotel = 0,

        /// <summary>
        /// 经济型酒店
        /// </summary>
        [EnumMember]
        SHotel = 1,

        /// <summary>
        /// 客栈/青年旅社
        /// </summary>
        [EnumMember]
        VHotel = 2
    }

    // Define an extension method in a non-nested static class.
    public static class HotelTypeExtensions
    {
        public static string[] ns = new string[] { "酒店", "经济型酒店", "客栈/青年旅社" };
        public static string GetDisplayName(this HotelType hotelType)
        {
            return ns[(int)hotelType];
        }

        public static string[] ens = new string[] { "h", "s", "v" };
        public static string GetDisplayEnName(this HotelType hotelType)
        {
            return ens[(int)hotelType];
        }

        public static string[] Sns = new string[] { "H", "M", "L" };
        public static string GetDisplaySName(this HotelType hotelType)
        {
            return Sns[(int)hotelType];
        }
    }
}
