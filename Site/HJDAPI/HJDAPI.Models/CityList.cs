using HJD.HotelServices.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.Models
{
    public class CityList
    {
        public List<CityEntity> HotOverseaArea { get; set; }
        public List<CityEntity> HotArea { get; set; }
        public List<CityEntity> Citys { get; set; }
        public List<CityEntity> HMTCitys { get; set; }
        public List<CityEntity> SouthEastAsiaCitys { get; set; }
        public List<CityEntity> BoutiqueCity { get; set; }
        /// <summary>
        /// iOS获取字典的顺序随机 次数组用于控制出现顺序
        /// </summary>
        public List<string> HotAreaKeys { get; set; }
        /// <summary>
        /// 多个热门地区的数组字典
        /// </summary>
        public Dictionary<string, List<CityEntity>> HotAreas { get; set; }
    }

    [Serializable]
    [DataContract]
    public class AroundCityList
    {
        [DataMember]
        public string currentCityName { get; set; }

        [DataMember]
        public int aroundCityCount { get; set; }

        //[DataMember]
        //public List<CityEntity> citys { get; set; }

        [DataMember]
        public List<ArounDistrictEntity> cityList { get; set; }
    }

    [Serializable]
    [DataContract]
    public class ShowCityInfo
    {
        [DataMember]
        public List<UserDefinedCity> UserDefinedList { get; set; }

        [DataMember]
        public List<AroundCityList> AroundCityList { get; set; }
    }

    [Serializable]
    [DataContract]
    public class UserDefinedCity
    {
        [DataMember]
        public string Title { get; set; }
        [DataMember]
        public string Link { get; set; }
    }


}