﻿using HJD.Framework.Entity;
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
    [DefaultColumn]
   public  class CityEntity
    {
        [DataMember]
        public int ID { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string EName { get; set; }

        [DataMember]
        public string pinyin { get; set; }

        [DataMember]
        public float lat { get; set; }

        [DataMember]
        public float lon { get; set; }
    }
}