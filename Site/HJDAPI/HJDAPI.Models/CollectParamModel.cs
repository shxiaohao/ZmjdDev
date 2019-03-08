using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.Models
{
   public class CollectParamModel
    {
        public CollectParamModel() { Items = new List<CollectParamItemModel>(); }
        //[DataMember]
        //public DateTime EndAddTime { get; set; }
         public bool IsCollect { get; set; }
        public List<CollectParamItemModel> Items { get; set; }
       public int Method { get; set; }
        //[DataMember]
        //public DateTime StartAddTime { get; set; }
       public long UserID { get; set; }

       public int start { get; set; }

       public int count { get; set; }
    }
}
