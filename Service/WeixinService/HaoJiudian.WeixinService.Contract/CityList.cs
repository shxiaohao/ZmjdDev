using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using HJD.Framework.Entity;

namespace HJD.WeixinServices.Contracts
{
    [Serializable]
    [DataContract]
    [DefaultColumn]
    public class CityList
    {
        [DataMember]
        public List<CityEntity> HotArea { get; set; }
        [DataMember]
        public List<CityEntity> Citys { get; set; }
    }
}
