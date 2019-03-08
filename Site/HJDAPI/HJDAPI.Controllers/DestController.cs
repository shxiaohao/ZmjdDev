using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using HJD.DestServices.Contract;

namespace HJDAPI.Controllers
{
    public class DestController : BaseApiController
    {
        [HttpGet]
        public SimpleCityInfo GetCityInfoByName(string cityName, float lat, float lon)
        {
            return ResourceAdapter.GetCityInfoByName(cityName,lat,  lon);
        }


        [HttpGet]
        public List<SimpleCityInfo> GetAroundCityInfo( float lat, float lon)
        {
            return ResourceAdapter.GetAroundCityInfo(lat, lon);
        }


        [HttpGet]
        public List<DistrictInfoEntity> GetDistrictInfo(string ids)
        {
            return ResourceAdapter.GetDistrictInfo(ids.Split(',').Select(s=>int.Parse(s)).ToList());
        }

        [HttpGet]
        public List<DistrictZoneEntity> GetStrategyDistrictZoneList(int districtId)
        {
            return ResourceAdapter.GetStrategyDistrictZoneList(districtId);
        }
    }
}
