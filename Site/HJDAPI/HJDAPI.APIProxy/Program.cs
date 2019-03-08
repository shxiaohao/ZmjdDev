using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HJDAPI.Models;

namespace HJDAPI.APIProxy
{
    class Program
    {
        static void Main(string[] args)
        {
           // HotelListQueryParam p = new HotelListQueryParam();

           // p.attraction = 176;
           // p.districtid = 2;
           //// p.checkin = DateTime.Now;

           // Hotel.Search(p);


          InterestHotelsResult r=  Hotel.QueryInterestHotel(new HotelListQueryParam() { districtid = 2, star = 0, count = 10 });
        }
    }
}
