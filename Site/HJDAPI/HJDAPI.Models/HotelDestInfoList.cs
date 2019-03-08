using HJD.HotelServices.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.Models
{
    public class HotelDestInfoList
    { 
        public HotelDestInfoList()
        {
            dicHotelDestInfoList = new List<dicHotelDestInfoList>();
        } 
        public List<dicHotelDestInfoList> dicHotelDestInfoList { get; set; }
    }
    public class dicHotelDestInfoList
    {
        public dicHotelDestInfoList()
        {
            Description = "";
            HotelDestInfoList = new List<HotelDestnInfo>();
        }
        public string Description { get; set; }
        public List<HotelDestnInfo> HotelDestInfoList { get; set; }
    }
}
