using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.Models
{
    [DataContract]
    public class ShopSearchItemEntity
    {
        public ShopSearchItemEntity()
        {
            SearchSort = new List<ShopSearchEntity>();
            SearScreen = new List<ShopSearchEntity>();
        }
        [DataMember]
        public List<ShopSearchEntity> SearchSort { get; set; }

        [DataMember]
        public List<ShopSearchEntity> SearScreen { get; set; }
    }

    [DataContract]
    public class ShopSearchEntity
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public int Values { get; set; }

        [DataMember]
        public int Type { get; set; }
    }
}
