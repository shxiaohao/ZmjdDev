using HJD.AccessServer.DAL;
using HJD.AccessService.Contract.Model;
using HJD.AccessService.Contract.Model.Dialog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJD.Search.CommonLibrary.Parses
{
    //解析城市信息
    //如果有附近、周围、周边等词，那么需要解析出相应的城市范围
    public class CityParse
    {

        public static void GenCityAroundOptionParam(UserWordOptionItem option, int districtID, int distance)
        {
            List<DistrictInfoAroundEntity> list = DestDBDAL.GetAroundDistrict(districtID, distance);

            option.ActionParam = string.Join("|", list.Select(d => string.Format("ID:{0},Name:{1}", d.DistrictID, d.Name)));
        }

    }
}
