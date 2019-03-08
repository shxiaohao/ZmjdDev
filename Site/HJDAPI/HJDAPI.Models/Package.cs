using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.Models
{
    //用于酒店列表套餐数据
    [DataContract]
    public class Package
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public decimal Price { get; set; }
        [DataMember]
        public decimal MarketPrice { get; set; }//市场价
        //[DataMember]
        //public string BoughtCount { get; set; } //已购买人数
        //[DataMember]
        //public string Description { get; set; }
    }
}
