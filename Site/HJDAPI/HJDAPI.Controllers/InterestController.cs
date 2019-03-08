using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using HJD.DestServices.Contract;
using HJD.HotelServices.Contracts;
using HJDAPI.Models;
using System.Runtime.Serialization;

namespace HJDAPI.Controllers
{

    public class InterestController : BaseApiController
    {
        [HttpGet]
        public InterestModel QueryInterest(int districtid, float lat, float lng, int distance = 300000)
        {
            return ResourceAdapter.QueryInterest(districtid, lat, lng, distance);
        }

        [HttpGet]
        public InterestModel QueryInterest2(int districtid, float lat, float lng, int distance = 300000)
        {
            return ResourceAdapter.QueryInterest2(districtid, lat, lng, distance);
        }

        [HttpGet]
        public InterestModel2 QueryInterest20(int districtid, float lat, float lng, int distance = 300000)
        {
            return ResourceAdapter.QueryInterest20(districtid, lat, lng, distance);
        }

        [HttpGet]
        public InterestModel2 QueryInterest30(int districtid, float userLat, float userLng,int geoScopeType , int distance = 300000)
        {
            return ResourceAdapter.QueryInterest30(districtid, userLat, userLng, geoScopeType, distance);
        }   
        
        [HttpGet]
        public ThemeModel QueryTheme(int districtid, float userLat, float userLng, int geoScopeType, int distance = 300000)
        {
            return ResourceAdapter.QueryTheme(districtid, userLat, userLng, geoScopeType, distance);
        }

        [HttpGet]
        public AttractionModel QueryAttraction(int districtid, float userLat, float userLng, int geoScopeType, int distance = 300000)
        {
            return ResourceAdapter.QueryAttraction(districtid, userLat, userLng, geoScopeType, distance);
        }
        
        [HttpGet]
        public List<InterestEntity2> GetAllInterest()
        {
            return ResourceAdapter.GetAllInterest();
        }
    }
 
   
}
