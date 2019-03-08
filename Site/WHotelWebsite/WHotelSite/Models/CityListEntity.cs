using HJD.HotelServices.Contracts;
using HJDAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WHotelSite.Models
{
    public class CityListEntity : CityList
    {
        /// <summary>
        /// A~Z城市组列表
        /// </summary>
        public List<IGrouping<string, CityEntity>> CityGroupList = new List<IGrouping<string, CityEntity>>();

        /// <summary>
        /// 周边城市列表
        /// </summary>
        public AroundCityList AroundCityList = new AroundCityList();

        /// <summary>
        /// 是否在微信环境下
        /// </summary>
        public bool IsInWeixin;

        /// <summary>
        /// 是否在zmjiudian App环境下
        /// </summary>
        public bool IsApp;
    }
}