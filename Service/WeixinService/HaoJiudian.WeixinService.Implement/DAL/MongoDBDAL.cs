using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HJD.Framework.Interface;
using HJD.WeixinServices.Contract;

namespace HJD.WeixinServices.Implement
{
    public class MongoDBDAL
    {
        private static IMongodbProvider mongodb = MongodbManagerFactory.Create("Weixin");

        private const string collectionName = "Weixin";

    }
}
