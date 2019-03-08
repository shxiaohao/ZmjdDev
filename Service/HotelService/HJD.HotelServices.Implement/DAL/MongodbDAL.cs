using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HJD.Framework.Interface;
using HJD.HotelServices.Contracts;
using System.Diagnostics;
using HJD.Framework.WCF;

namespace HJD.HotelServices
{
    internal class MongodbDAL
    {
        //private static IMongodbProvider mongodb = MongodbManagerFactory.Create("Hotel");
        //private const string collectionName = "Review";
        //private const string collectionHotelList = "Hotel";

        //internal static HotelReviewSignatureForMongoDB GetHotelReviewSignatureByWriting(int p)
        //{
        //    return mongodb.SelectOne<HotelReviewSignatureForMongoDB>(collectionName, Query.EQ("_id", p));
        //}
        //internal static HotelListInfoForMongoDB GetHotelListInfoSignature(int p)
        //{
        //    return mongodb.SelectOne<HotelListInfoForMongoDB>(collectionHotelList, Query.EQ("_id", p));
        //}
        //internal static HotelReviewForMongodb GetHotelReviewInfoByWriting(int p)
        //{
        //    return mongodb.SelectOne<HotelReviewForMongodb>(collectionName, Query.EQ("_id", p));
        //}

        //internal static void SetHotelReviewByWriting(HotelReviewForMongodb pm)
        //{
        //    mongodb.Update<HotelReviewForMongodb>(collectionName, Query.EQ("_id", pm._id), pm, true);
        //}
        ////获取用户的酒店点评
        //internal static List<HotelReviewSignatureForMongoDB> GetUserHotelReviewList(long userid, bool isOwner, bool isHide)
        //{
        //    QueryCondition qc = Query.EQ("UserID", userid);



        //    if (isOwner)//管理员
        //    {
        //        qc = Query.And(qc, Query.NE("Status", 1));
        //    }
        //    else if (isHide) //隐发布
        //    {
        //        qc = Query.And(qc, Query.And(Query.EQ("Deleted", 0)), Query.And(Query.EQ("Status", 2)));

        //    }
        //    else
        //    {
        //        qc = Query.And(qc, Query.EQ("Deleted", 0));
        //    }

        //    //排序
        //    Dictionary<string, OrderBy> od = new Dictionary<string, OrderBy>();
        //    od.Add("OrderColTime", OrderBy.Desc);


        //    //调试代码
        //    System.Diagnostics.Debug.Print(mongodb.QueryHelper(qc));


        //    return mongodb.SelectTop<HotelReviewSignatureForMongoDB>(collectionName, qc, 100000, od);


        //}

        //internal static List<HotelReviewSignatureForMongoDB> GetHotelReviewList(ArguHotelReview argu)
        //{
        //    QueryCondition qc = Query.EQ("Hotel", argu.Hotel);

        //    bool hasFilterCol = false;
        //    if (argu.UserIdentityType != UserIdentityType.All)
        //    {
        //        qc = Query.And(qc, Query.EQ("FilterCol", HotelReviewFilterPrefix.UserIdentity + (int)argu.UserIdentityType));
        //        hasFilterCol = true;
        //    }

        //    if (argu.RatingType != RatingType.All)
        //    {
        //        qc = Query.And(qc, Query.EQ("FilterCol", HotelReviewFilterPrefix.Rate + (int)argu.RatingType));
        //        hasFilterCol = true;
        //    }

        //    if (!hasFilterCol)
        //        qc = Query.And(qc, Query.EQ("FilterCol", 0));

        //    if (argu.HideUserID > 0)//隐发布用户
        //    {
        //        qc = Query.And(qc, Query.Or(Query.EQ("UserID", argu.HideUserID), Query.EQ("Deleted", 0)));
        //    }
        //    else if (argu.UserID > 0) //登录用户
        //    {
        //        qc = Query.And(qc, Query.Or(Query.And(Query.EQ("UserID", argu.UserID),
        //                                               Query.NE("Status", 1)),
        //                                    Query.EQ("Deleted", 0)));
        //    }
        //    else  //未登录用户
        //    {
        //        qc = Query.And(qc, Query.EQ("Deleted", 0));
        //    }

        //    //排序
        //    Dictionary<string, OrderBy> od = new Dictionary<string, OrderBy>();
        //    switch (argu.HotelReviewOrderType)
        //    {
        //        case HotelReviewOrderType.CheckIn_Down:
        //            od.Add("OrderColCheckIn", OrderBy.Desc);
        //            break;
        //        case HotelReviewOrderType.CheckIn_Up:
        //            od.Add("OrderColCheckIn", OrderBy.Asc);
        //            break;
        //        case HotelReviewOrderType.Friend:
        //            od.Add("OrderColTime", OrderBy.Desc);
        //            break;
        //        case HotelReviewOrderType.Rating_Down:
        //            od.Add("OrderColRating", OrderBy.Desc);
        //            break;
        //        case HotelReviewOrderType.Rating_Up:
        //            od.Add("OrderColRating", OrderBy.Asc);
        //            break;
        //        case HotelReviewOrderType.Time_Up:
        //            od.Add("OrderColTime", OrderBy.Asc);
        //            break;
        //        case HotelReviewOrderType.Time_Down:
        //        default:
        //            od.Add("OrderColTime", OrderBy.Desc);
        //            break;
        //    }

        //    if (argu.HotelReviewOrderType == HotelReviewOrderType.Friend
        //        && argu.FriendUserID != null && argu.FriendUserID.Count > 0)
        //    {
        //        return GetHotelReviewOrderByFriend(argu, qc, od);
        //    }
        //    else
        //    {
        //        //调试代码
        //        System.Diagnostics.Debug.Print(mongodb.QueryHelper(qc));

        //        if (argu.Start == 0)
        //        {
        //            return mongodb.SelectTop<HotelReviewSignatureForMongoDB>(collectionName, qc, argu.Count, od);
        //        }
        //        else
        //        {
        //            return mongodb.Select<HotelReviewSignatureForMongoDB>(collectionName, qc, argu.Start, argu.Count, od);
        //        }
        //    }
        //}

        //private static List<HotelReviewSignatureForMongoDB> GetHotelReviewOrderByFriend(ArguHotelReview argu, QueryCondition qc, Dictionary<string, OrderBy> od)
        //{
        //    QueryCondition fqc;

        //    //先取朋友的点评
        //    if (argu.FriendUserID.Count == 1)
        //    {
        //        fqc = Query.And(qc, Query.EQ("UserID", argu.FriendUserID[0]));
        //    }
        //    else
        //    {
        //        fqc = Query.And(qc, Query.In("UserID", argu.FriendUserID));
        //    }
        //    //调试代码
        //    System.Diagnostics.Debug.Print(mongodb.QueryHelper(fqc));

        //    List<HotelReviewSignatureForMongoDB> l1;
        //    int iCountFriend = 0;

        //    if (argu.Start == 0)
        //    {
        //        l1 = mongodb.SelectTop<HotelReviewSignatureForMongoDB>(collectionName, fqc, argu.Count, od);
        //    }
        //    else
        //    {
        //        //计算朋友点评数
        //        iCountFriend = (int)mongodb.Count(collectionName, fqc);
        //        if (argu.Start < iCountFriend)
        //        {
        //            //取朋友点评
        //            l1 = mongodb.Select<HotelReviewSignatureForMongoDB>(collectionName, fqc, argu.Start, argu.Count, od);
        //        }
        //        else
        //            l1 = new List<HotelReviewSignatureForMongoDB>();
        //    }

        //    int ileft = argu.Count - l1.Count;
        //    if (ileft > 0)
        //    {
        //        //补非朋友的点评
        //        if (argu.FriendUserID.Count == 1)
        //        {
        //            fqc = Query.And(qc, Query.NE("UserID", argu.FriendUserID[0]));
        //        }
        //        else
        //        {
        //            fqc = Query.And(qc, Query.NotIn("UserID", argu.FriendUserID));
        //        }

        //        //调试代码
        //        System.Diagnostics.Debug.Print(mongodb.QueryHelper(fqc));

        //        if (argu.Start == 0 || argu.Start <= iCountFriend)
        //            l1.AddRange(mongodb.SelectTop<HotelReviewSignatureForMongoDB>(collectionName, fqc, ileft, od));
        //        else
        //        {
        //            l1.AddRange(mongodb.Select<HotelReviewSignatureForMongoDB>(collectionName, fqc, argu.Start - iCountFriend, ileft, od));
        //        }
        //    }
        //    return l1;
        //}

        //internal static long GetIndexHotelReview(int hotelID, int writing, long userID, long hideUserID)
        //{
        //    QueryCondition qc = Query.EQ("Hotel", hotelID);

        //    //使用索引
        //    qc = Query.And(qc, Query.EQ("FilterCol", 0));

        //    if (hideUserID > 0)//隐发布用户
        //    {
        //        qc = Query.And(qc, Query.Or(Query.EQ("UserID", hideUserID), Query.EQ("Deleted", 0)));
        //    }
        //    else if (userID > 0) //登录用户
        //    {
        //        qc = Query.And(qc, Query.Or(Query.And(Query.EQ("UserID", userID),
        //                                               Query.NE("Status", 1)),
        //                                    Query.EQ("Deleted", 0)));
        //    }
        //    else  //未登录用户
        //    {
        //        qc = Query.And(qc, Query.EQ("Deleted", 0));
        //    }

        //    var m = GetHotelReviewInfoByWriting(writing);

        //    if (m == null)
        //    {
        //        qc = Query.EQ("_id", -1);
        //    }
        //    else
        //    {
        //        qc = Query.And(qc, Query.GTE("OrderColTime", m.OrderColTime));
        //    }

        //    //调试代码
        //    System.Diagnostics.Debug.Print(mongodb.QueryHelper(qc));

        //    return mongodb.Count(collectionName, qc);
        //}
        ///// <summary>
        ///// 设置酒店到mongodb
        ///// </summary>
        ///// <param name="pm"></param>
        //internal static void SetHotelToMongoDBByHotel(HotelListInfoMongoDB pm)
        //{
        //    //mongodb.Delete(collectionHotelName, Query.EQ("DistrictID", 1));
        //    mongodb.Update<HotelListInfoMongoDB>(collectionHotelList, Query.EQ("_id", pm._id), pm, true);
        //}

        //internal static List<HotelListInfoForMongoDB> GetHotelList(int distirctID, List<long> filterCol, DateTime checkInDate, DateTime checkOutDate, decimal minPrice, decimal maxPrice, int startIndex, int returnCount, HotelListSortType sortType, OrderBy orderBy, out long count, double lat, double lng, int distance, double nLat, double nLng, double sLat, double sLng)
        //{
        //    count = 0;
        //    QueryCondition qc = GenQcHotelList(distirctID, filterCol, checkInDate, checkOutDate, lat, lng, distance, nLat, nLng, sLat, sLng);

        //    List<HotelListInfoForMongoDB> result = new List<HotelListInfoForMongoDB>();

        //    List<string> columns = new List<string> { "_id" };

        //    if (sortType == HotelListSortType.Distance)
        //        columns.Add("HotelDistances");
        //    List<HotelListInfoForMongoDB> hl = new List<HotelListInfoForMongoDB>();
        //    if (maxPrice > 0 || sortType != HotelListSortType.Rank)
        //    {
        //        if (sortType == HotelListSortType.Rank)
        //            columns.Add("Rank");
        //        hl = mongodb.SelectTop<HotelListInfoForMongoDB>(collectionHotelList, qc, int.MaxValue, null, columns);
        //    }
        //    else
        //    {
        //        count = mongodb.Count(collectionHotelList, qc);
        //        Dictionary<string, OrderBy> od = new Dictionary<string, OrderBy>() { { "Rank", orderBy } };
        //        hl = mongodb.Select<HotelListInfoForMongoDB>(collectionHotelList, qc, startIndex, returnCount, od, columns);
        //    }
        //    return hl;
        //}

        //private static QueryCondition GenQcHotelList(int District, List<long> filterCol, DateTime checkInDate, DateTime checkOutDate, double lat, double lng, int distance, double nLat, double nLng, double sLat, double sLng)
        //{
        //    QueryCondition qc = null;

        //    if (District > 0) //有目的地信息时的处理方式
        //    {
        //        qc = Query.EQ("FilterCol", HotelListFilterPrefix.DistrictID + District);
        //    }

        //    QueryCondition tqc = null;
        //     long lastf = 0;
        //    foreach (long f in filterCol.OrderBy(o => o))
        //    {
        //        if (f / 10000000000 != lastf / 10000000000)//每类内用OR, 不同类间用 AND
        //        {
        //            if (tqc != null)
        //                qc = Query.And(qc, tqc);
        //            tqc = (Query.EQ("FilterCol", f));
        //        }
        //        else
        //        {
        //            tqc = Query.Or(tqc, Query.EQ("FilterCol", f));
        //        }

        //        lastf = f;
        //    }

        //    if (tqc != null)
        //        qc = Query.And(qc, tqc);
        //     string locName = "Gloc";//目前只考虑用google坐标           
        //     if (lat > 0)
        //        qc = Query.And(qc, Query.Near(locName, lat, lng, DistanceToOffset(distance)));
        //     else if (nLat > 0)
        //         qc = Query.And(qc, Query.WithinRectangle(locName, nLat, nLng, sLat, sLng));
            
        //    //调试代码
        //    Debug.Print(mongodb.QueryHelper(qc));

        //    return qc;
        //}

        ///// <summary>
        ///// 距离转换成经纬度偏移,1米约等于0.000007
        ///// </summary>
        ///// <param name="distance">距离，单位米</param>
        ///// <returns></returns>
        //public static double DistanceToOffset(int distance)
        //{
        //    const double d = 0.000007;
        //    return d * distance;
        //}
    }
}
