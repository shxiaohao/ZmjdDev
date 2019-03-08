using HJD.HotelServices.Contracts;
using System.Collections.Generic;
using System.Runtime.Serialization;
namespace HJDAPI.Models
{
    public class HotelPrice
    {
       
        public int HotelID { get; set; }
        public string Name { get; set; }

        public List<OTAInfo> OTAList { get; set; }
       
    }



}