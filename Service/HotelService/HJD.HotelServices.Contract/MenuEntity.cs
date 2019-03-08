using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using HJD.Framework.Entity;

namespace HJD.HotelServices.Contracts
{
    /// <summary>
    /// 菜单用
    /// </summary>
    [Serializable]
    [DataContract]
    [DefaultColumn]
    public class MenuEntity
    {
        [DataMemberAttribute()]
        public List<FilterDicEntity> Brands{ get; set; }

        [DataMemberAttribute()]         
        public List<FilterDicEntity> Zones { get; set; }

        [DataMemberAttribute()]
        public List<FilterDicEntity> Locations { get; set; }

        [DataMemberAttribute()]
        public List<FilterDicEntity> Tags { get; set; }

        [DataMemberAttribute()]
        public List<FilterDicEntity> Stars { get; set; }

        [DataMemberAttribute()]
        public List<FilterDicEntity> Facilitys { get; set; }

        [DataMemberAttribute()]
        public List<FilterDicEntity> Classes { get; set; }

        [DataMemberAttribute()]
        public List<FilterDicEntity> DistrictIDs { get; set; }

        [DataMemberAttribute()]
        public List<string> FilterPrice { get; set; }

        public MenuEntity()
        {
            Brands = new List<FilterDicEntity>();
            Zones = new List<FilterDicEntity>();
            Locations = new List<FilterDicEntity>();
            Tags = new List<FilterDicEntity>();
            Stars = new List<FilterDicEntity>();
            Facilitys = new List<FilterDicEntity>();
        }

    }
}
