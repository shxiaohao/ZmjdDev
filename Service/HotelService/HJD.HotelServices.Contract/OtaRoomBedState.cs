using HJD.Framework.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HJD.HotelServices.Contracts
{
    [Serializable]
    [DataContract]
    public class OtaRoomBedState
    {
        [DataMemberAttribute()]
        [DBColumnAttribute]
        public Int64 Id { get; set; }

        [DataMemberAttribute()]
        [DBColumnAttribute]
        public Int64 HotelId { get; set; }

        [DataMemberAttribute()]
        [DBColumnAttribute]
        public Int32 RoomId { get; set; }

        [DataMemberAttribute()]
        [DBColumnAttribute]
        public string RoomName { get; set; }

        [DataMemberAttribute()]
        [DBColumnAttribute]
        public DateTime Date { get; set; }

        [DataMemberAttribute()]
        [DBColumnAttribute]
        public int State { get; set; }

        [DataMemberAttribute()]
        [DBColumnAttribute]
        public int HaveCount { get; set; }

        [DataMemberAttribute()]
        [DBColumnAttribute]
        public int SoldCount { get; set; }

        [DataMemberAttribute()]
        [DBColumnAttribute]
        public int ChannelId { get; set; }

        [DataMemberAttribute()]
        [DBColumnAttribute]
        public Int64 HotelOriId { get; set; }

        [DataMemberAttribute()]
        [DBColumnAttribute]
        public Int64 RoomOriId { get; set; }

        [DataMemberAttribute()]
        [DBColumnAttribute]
        public string RoomOriName { get; set; }

        [DataMemberAttribute()]
        [DBColumnAttribute]
        public DateTime UpdateTime { get; set; }
    }

    [Serializable]
    [DataContract]
    public class OtaHotelRoomState
    {
        [DataMemberAttribute()]
        [DBColumnAttribute]
        public Int64 HotelId { get; set; }

        [DataMemberAttribute()]
        [DBColumnAttribute]
        public Int64 HotelOriId { get; set; }

        [DataMemberAttribute()]
        [DBColumnAttribute]
        public string HotelName { get; set; }

        [DataMemberAttribute()]
        [DBColumnAttribute]
        public int State1 { get; set; }

        [DataMemberAttribute()]
        [DBColumnAttribute]
        public int State0 { get; set; }

        [DataMemberAttribute()]
        [DBColumnAttribute]
        public int State_1 { get; set; }
    }
}
