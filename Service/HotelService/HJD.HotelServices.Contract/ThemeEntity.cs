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
    public class HotelThemeEntity
    {
        [DataMemberAttribute()]
        public int ID { get; set; }

        [DataMemberAttribute()]
        public string Name { get; set; }

        [DataMemberAttribute()]
        public int HotelCount { get; set; }

        [DataMemberAttribute()]
        public string ImgUrl { get; set; }      
        
        [DataMemberAttribute()]
        public string Hotels { get; set; }
    }
}
