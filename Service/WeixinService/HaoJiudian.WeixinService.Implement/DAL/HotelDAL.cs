using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HJD.Framework.Interface;
using HJD.WeixinServices.Contract;
using System.Data;
using HJD.WeixinServices.Contracts;

namespace HJD.WeixinServices.Implement
{
    public class HotelDAL
    {
        private static IDatabaseManager HotelDB = DatabaseManagerFactory.Create("HotelDB");

        public static List<CityEntity> GetZMJDCityData()
        {
            return HotelDB.ExecuteSqlString<CityEntity>("HotelDB.GetZMJDCityData");
        }
    }
}
