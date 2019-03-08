using HJD.HotelManagementCenter.Domain.Settlement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.Models.RequestParams
{
    [Serializable]
    [DataContract]
    public class GetInvoiceParams : BaseParam
    {
        [DataMember]
        public InvoiceEntity invoice { get; set; }
        [DataMember]
        public long orderid { get; set; }

        /// <summary>
        /// 收货人信息ID
        /// </summary>
        [DataMember]
        public int ReceivePeopleInformationId { get; set; }
    }
}
