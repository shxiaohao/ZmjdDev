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
    /// wap 菜单用
    /// </summary>
    [Serializable]
    [DataContract]
    [DefaultColumn]
    public class WapMenuEntity
    {
        //[DataMemberAttribute()]
        //public List<FilterDicEntity> Brands{ get; set; }

        //[DataMemberAttribute()]         
        //public List<FilterDicEntity> Zones { get; set; }

        //[DataMemberAttribute()]
        //public List<FilterDicEntity> Locations { get; set; }

        [DataMemberAttribute()]
        public List<FilterDicEntity> Tags { get; set; }

        [DataMemberAttribute()]
        public int TotalNum { get; set; }


        [DataMemberAttribute()]
        public int ValuedNum { get; set; }

         //<summary>
         //精品酒店
         //</summary>
        [DataMemberAttribute()]
        public int BoutiqueNum { get; set; }

         //<summary>
         //度假村
         //</summary>
        [DataMemberAttribute()]
        public int ResortNum { get; set; }

        [DataMemberAttribute()]
        public int PriceSectionMinPrice { get; set; }

        [DataMemberAttribute()]
        public int PriceSectionMaxPrice { get; set; }

        [DataMemberAttribute()]
        public Dictionary<int,string> filterPrice { get; set; }

        public WapMenuEntity()
        {
            //Brands = new List<FilterDicEntity>();
            //Zones = new List<FilterDicEntity>();
            //Locations = new List<FilterDicEntity>();
            Tags = new  List<FilterDicEntity>();
        }

    }
}
