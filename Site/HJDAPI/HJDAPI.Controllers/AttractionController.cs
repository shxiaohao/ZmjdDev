using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using HJD.DestServices.Contract;
using HJD.HotelServices.Contracts;
using HJDAPI.Models;

namespace HJDAPI.Controllers
{

    public class AttractionController : BaseApiController
    {
        [HttpGet]
        public List<AttractionEntity> GetAttractionByCity(int cityID)
        {
            return ResourceAdapter.GetAttractionByCity(cityID);
        }

        [HttpGet]
        public AttractionEntity GetOneAttraction(int attractionID)
        {
            return ResourceAdapter.GetOneAttraction(attractionID);
        }

        [HttpGet]
        public List<AttractionEntity> GetAllAttraction()
        {
            return ResourceAdapter.GetAllAttraction();
        }

          
        /// <summary>
        /// 酒店搜索
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public WapAttractionResult AttractionQuery([FromUri] AttractionQueryParam p)
        {
            return ResourceAdapter.AttractionQuery(p);          
        }

        /// <summary>
        /// 获取城市列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public List<HJD.DestServices.Contract.CityEntity> GetCityList()
        {
            return ResourceAdapter.GetCityList();
        }
    }
}
