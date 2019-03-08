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
    public class CouponProductEntity
    {

        [DataMember]
        public string PhotoUrl { get; set; }

        [DataMember]
        public bool NeedPhoto { get; set; }

        [DataMember]
        public List<string> PackageInfoList { get; set; }

        [DataMember]
        public decimal Price { get; set; }

        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public int State { get; set; }

        [DataMember]
        public string ExchangeNo { get; set; }

        [DataMember]
        public int SupplierId { get; set; }

        [DataMember]
        public string SupplierName { get; set; }

        [DataMember]
        public string SKUName { get; set; }

        [DataMember]
        public string SPUName { get; set; }

        [DataMember]
        public string ShopName { get; set; }

        [DataMember]
        public DateTime ConsumeTime { get; set; }
        
        [DataMember]
        public string Msg { get; set; }

        [DataMember]
        public string CouponNote { get; set; }

        [DataMember]
        public int RefundState { get; set; }

        [DataMember]
        public bool IsShowPrice { get; set; }

        [DataMember]
        public int OperationState { get; set; }

        [DataMember]
        public long UserID { get; set; }

        [DataMember]
        public DateTime ExpireTime { get; set; }
    }
}
