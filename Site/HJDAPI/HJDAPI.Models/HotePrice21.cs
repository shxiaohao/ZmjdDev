using HJD.HotelServices.Contracts;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
namespace HJDAPI.Models
{
    public class HotelPrice21
    {
       
        public int HotelID { get; set; }
        public string Name { get; set; }

        public DateTime CheckIn { get; set; }

        public DateTime CheckOut { get; set; }

        public List<PackageInfoEntity> Packages { get; set; }                

        public List<OTAInfo2> OTAList { get; set; }
       
    }



}