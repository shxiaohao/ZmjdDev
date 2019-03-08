using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HJD.Framework.Interface;

namespace HJD.HotelServices
{
    internal class MongodbHotelDAL
    {
        private static IMongodbProvider mongodb = MongodbManagerFactory.Create("Hotel");
        private const string collectionName = "Hotel";

        internal static long Gen_id(int pDistrictID, int pHotelID)
        {
            return HotelFilterPrefix.BaseOffset * pDistrictID + pHotelID;
        }

        internal static HotelSignatureForMongoDB GetHotelSignatureByHotel(long id)
        {
            return mongodb.SelectOne<HotelSignatureForMongoDB>(collectionName, Query.EQ("_id", id));
        }

        internal static void SetHotelByHotel(HotelForMongodb pm)
        {
            mongodb.Update<HotelForMongodb>(collectionName, Query.EQ("_id", pm._id), pm, true);
        }

        internal static HotelForMongodb GetHotelInfo(long id)
        {
            return mongodb.SelectOne<HotelForMongodb>(collectionName, Query.EQ("_id", id));
        }
    }
}
