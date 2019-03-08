using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using HJD.Framework.Entity;

namespace HJD.HotelServices.Implement.Entity
{
    [Serializable]
    [DataContract]
    [DefaultColumn]
    public class HotelReview4TagKeywordEntity
    {
        /// <summary>
        /// 酒店id
        /// </summary>
        [DataMemberAttribute()]
        public int Resource { get; set; }

        /// <summary>
        /// writing
        /// </summary>
        [DataMemberAttribute()]
        public int Writing { get; set; }

        /// <summary>
        /// content
        /// </summary>
        [DataMemberAttribute()]
        public string Content { get; set; }
    }
}
