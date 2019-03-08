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
    public class BookInspectorHotelParam:BaseParam
    {
        [DataMember]
        public long id { get; set; }
        [DataMember]
        public long userid { get; set; }
        [DataMember]
        public string checkin { get; set; }
        [DataMember]
        public string checkout { get; set; }
        [DataMember]
        public Int32 hotelid { get; set; }

        /// <summary>
        /// 房券ID
        /// </summary>
        [DataMember]
        public Int32 HVID { get; set; }
        /// <summary>
        /// 房券编号ID
        /// </summary>
        [DataMember]
        public Int32 VID { get; set; }
    }
}