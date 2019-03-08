using HJD.Framework.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJD.HotelServices
{
    public class RepositoryBase
    {
        protected static IDatabaseManager HotelBizDB = DatabaseManagerFactory.Create("HotelBizDB");
        protected static IDatabaseManager HotelDB = DatabaseManagerFactory.Create("HotelDB");
        protected static IDatabaseManager HotelReviewsDB = DatabaseManagerFactory.Create("HotelReviewsDB");
        protected static IDatabaseManager CommDB = DatabaseManagerFactory.Create("CommDB");
        protected static IDatabaseManager DestDB = DatabaseManagerFactory.Create("DestDB");
        protected static IDatabaseManager OTADataDB = DatabaseManagerFactory.Create("OTADataDB");
        protected static IDatabaseManager CtripDB = DatabaseManagerFactory.Create("CtripDB");
    }
}
