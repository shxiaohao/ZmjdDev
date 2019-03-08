using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using HJD.HotelServices.Contracts;

namespace HJDAPI.Models
{
     /// <summary>
    /// wap 菜单用
    /// </summary>
    [Serializable]
    [DataContract]
    public class AppMenuEntity
    {  

        [DataMemberAttribute()]
        public List<FilterDicEntity> Tags { get; set; }

        [DataMemberAttribute()]
        public List<TypeItem> TypeItems { get; set; }

        [DataMemberAttribute()]
        public int TotalNum { get; set; }

        [DataMemberAttribute()]
        public List<FilterDicEntity> Attractions { get; set; }


        public AppMenuEntity()
        {
             Tags = new  List<FilterDicEntity>();
             TypeItems = new List<TypeItem>();
             Attractions = new List<FilterDicEntity>();
        }
    }

    [Serializable]
    [DataContract]
    public class TypeItem
    {
      
        [DataMemberAttribute()]
        public int TypeID{ get; set; } 

        [DataMemberAttribute()]
        public string TypeName{ get; set; }
 
        [DataMemberAttribute()]
        public int TotalNum { get; set; }

        [DataMemberAttribute()]
        public List<FilterDicEntity> FeaturedList { get; set; }
    }
}
