﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HJD.HotelServices.Contracts
{
    [Serializable]
    [DataContract]
    public class QueryReviewResult
    {
        [DataMember]
        public int TotalCount { get; set; }
        [DataMember]
        public Dictionary<long, int> FilterCount { get; set; }
        [DataMember]
        public List<HotelReviewExEntity> ReviewList { get; set; }

        public QueryReviewResult()
        {
            FilterCount = new Dictionary<long, int>();
            ReviewList = new List<HotelReviewExEntity>();
        }
    }
}
