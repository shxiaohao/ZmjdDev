using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.Models
{
    [DataContract]
    public class BaseResponseEx : BaseResponse
    {
        /// <summary>
        /// 需要返回的ID
        /// </summary>
        [DataMember]
        public int BizID { get; set; }

        /// <summary>
        /// 返回ID的类型
        /// </summary>
        public enum BizTypeEnum
        {
            ReceivePeopleInformationID = 800,//收件人表（ReceivePeopleInformation）ID
            PayID = 1000                     //支付ID（CommOrders表中的IDX）
        }
    }
}
