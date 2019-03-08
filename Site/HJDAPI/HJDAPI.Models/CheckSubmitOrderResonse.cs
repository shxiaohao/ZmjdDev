using HJD.HotelPrice.Contract.DataContract.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.Models
{
    [DataContract]
    public class CheckSubmitOrderResonse 
    {
        public CheckSubmitOrderResonse()
        {
            ResponseResult = new TextAndUrl();
        }
        [DataMember]
        public int ResultCode { get; set; }
        //[DataMember]
        //public string ActionUrl { get; set; }
        [DataMember]
        public TextAndUrl ResponseResult { get; set; }
    }
}
