using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HJD.DestServices.Contract;
using HJDAPI.Common.Helpers;
using HJDAPI.Controllers;
using HJDAPI.Controllers.Adapter;
using Newtonsoft.Json;


namespace HJDAPI.APIProxy
{
    public class attraction : BaseProxy
    {
       

        public List<AttractionEntity> GetAttractionByCity(int cityID)
        {
            
            if (IsProductEvn)
                return ResourceAdapter.GetAttractionByCity(cityID);
            else
            {
                string url = APISiteUrl + "api/attraction/GetAttractionByCity";
                string postDataStr = string.Format("cityID={0}",cityID);

                CookieContainer cc = new CookieContainer();
                string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
                return JsonConvert.DeserializeObject<List<AttractionEntity>>(json);
            }
        }

        public List<AttractionEntity> GetAllAttraction()
        {

            if (IsProductEvn)
                return ResourceAdapter.GetAllAttraction();
            else
            {
                string url = APISiteUrl + "api/attraction/GetAllAttraction";
                string postDataStr = "";

                CookieContainer cc = new CookieContainer();
                string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
                return JsonConvert.DeserializeObject<List<AttractionEntity>>(json);
            }
        }

        public WapAttractionResult AttractionQuery(AttractionQueryParam p)
        {
            if (IsProductEvn)
            
            return ResourceAdapter.AttractionQuery(p);
            else
            {
                string url = APISiteUrl + "api/attraction/AttractionQuery";
                string postDataStr = string.Format("districtid={0}&districtuniquename={1}&lat={2}&lng={3}&distanceFrom={4}&distanceTo={5}&sort={6}&order={7}&start={8}&count={9}&themes={10}&nLat={11}&nLng={12}&sLat={13}&sLng={14}",
                    p.districtid, p.districtUniqueName, p.lat, p.lng, p.distanceFrom, p.distanceTo, p.sort, p.order, p.start, p.count, p.themes, p.nLat, p.nLng, p.sLat, p.sLng);

                CookieContainer cc = new CookieContainer();
                string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
                return JsonConvert.DeserializeObject<WapAttractionResult>(json);
            }
        }
        public List<CityEntity> GetCityList()
        {
            if (IsProductEvn)
                return ResourceAdapter.GetCityList();
            else
            {
                string url = APISiteUrl + "api/attraction/GetCityList";
              
                CookieContainer cc = new CookieContainer();
                string json = HttpRequestHelper.Get(url, "", ref cc);
                return JsonConvert.DeserializeObject<List<CityEntity>>(json);
            }
        }
    }
}
