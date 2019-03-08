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
    public class SubmitExchangeRoomOrderResult
    {
        [DataMember]
        public string Message { get; set; }
        [DataMember]
        public int Success { get; set; }
        [DataMember]
        public long OrderID { get; set; }
        [DataMember]
        public string HotelName { get; set; }
        [DataMember]
        public long UserID { get; set; }
    }

    [Serializable]
    [DataContract]
    public class SubmitExchangeRoomOrderParam
    {
        [DataMember]
        public string contact { get; set; }
        [DataMember]
        public string contactPhone { get; set; }
        /// <summary>
        /// 准备使用多少张房券
        /// </summary>
        [DataMember]
        public int useCouponNum { get; set; }
        [DataMember]
        public string exchangeNo { get; set; }
        /// <summary>
        /// 第三方的凭证(可能有)
        /// </summary>
        [DataMember]
        public string otherToken { get; set; }
        [DataMember]
        public DateTime checkIn { get; set; }
        [DataMember]
        public int nightCount { get; set; }
        [DataMember]
        public int roomCount { get; set; }
        [DataMember]
        public int packageID { get; set; }
        /// <summary>
        /// 填写订单的一些补充信息 包括床型以及其他要求
        /// </summary>
        [DataMember]
        public string note { get; set; }
        /// <summary>
        /// 区分我们的套餐还是携程的套餐等
        /// </summary>
        [DataMember]
        public int packageType { get; set; }
        [DataMember]
        public int hotelID { get; set; }
        [DataMember]
        public int terminalID { get; set; }
        [DataMember]
        public int channelID { get; set; }
        [DataMember]
        public int userID { get; set; }
        [DataMember]
        public List<int> travelId { get; set; }
    }
}