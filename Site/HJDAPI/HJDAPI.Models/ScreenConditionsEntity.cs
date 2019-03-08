using HJD.HotelServices.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.Models
{
    [Serializable]
    [DataContract]
    public class ScreenConditionsEntity
    {
        public ScreenConditionsEntity()
        {
            StartCityList = new List<CityEntity>();
            GoToCityList = new List<CityEntity>();
            Calendar = new List<YearEntity>();
        }
        [DataMember]
        public List<CityEntity> StartCityList { get; set; }

        [DataMember]
        public List<CityEntity> GoToCityList { get; set; }

        [DataMember]
        public List<YearEntity> Calendar { get; set; }

    }

    [Serializable]
    [DataContract]
    public class YearEntity
    {
        public YearEntity()
        {
            MonthList = new List<MonthEntity>();
        }
        [DataMember]
        public int Year { get; set; }

        [DataMember]
        public int State { get; set; }

        [DataMember]
        public List<MonthEntity> MonthList { get; set; }
    }

    [Serializable]
    [DataContract]
    public class MonthEntity
    {
        [DataMember]
        public int Month { get; set; }

        [DataMember]
        public int State { get; set; }
    }
}
