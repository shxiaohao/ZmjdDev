using HJD.Framework.Interface;
using HJD.Framework.WCF;
using HJD.HotelServices.Contracts;
using HJDAPI.Common.Helpers;
using HJDAPI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HJDAPI.Controllers.Common
{
    public class HotelHelper
    {
        public static ICacheProvider LocalCache = CacheManagerFactory.Create("DynamicCacheForType2");
        public static IHotelService HotelService = ServiceProxyFactory.Create<IHotelService>("BasicHttpBinding_IHotelService");
    
        /// <summary>
        /// 专辑下所有套餐，{0}用专辑ID替换
        /// </summary>
        static string AblumPIDKey = "AlbumId_PID:{0}";
        /// <summary>
        /// 专辑对象，{0}用专辑ID替换
        /// </summary>
        static string AlbumEntityKey="AlbumEntity:{0}";
        /// <summary>
        /// 套餐列表，{0}用套餐ID替换
        /// </summary>
        static string AlbumPackageKey ="AlbumPackage:{0}";
        /// <summary>
        /// 专辑分享对象，{0}用专辑ID替换
        /// </summary>
        static string AlbumShareModel="AlbumShareModel:{0}";
        /// <summary>
        /// key过期时间，单位分钟
        /// </summary>
        static double KeyExpireTime = 24;
        public static int[] TransStarArgu(int pStar)
        {
            if (pStar > 0)
            {
                switch (pStar)
                {
                    case 5:
                    case 4:
                        return new int[] { pStar };
                    default:
                        return new int[] { 1, 2, 3 };
                }
            }

            return new int[] { 1, 2, 3, 4, 5 };
        }

        public static List<OTAInfo> GetHotelOTAInfo(int hotelid, string sType, bool isForReview = false)
        {
            return LocalCache.GetData<List<OTAInfo>>(string.Format("OTAInfo{0}{1}{2}", sType, isForReview, hotelid), () =>
            {

                List<OTAInfo> otaInfos = new List<OTAInfo>();

                HotelInfoChannelExEntity chl = HotelService.GetHotelChannelList(new List<int> { hotelid }).FirstOrDefault();
                if (chl != null && chl.HotelOriIDList != null && chl.HotelOriIDList.Count > 0)
                {
                    foreach (HotelInfoChannelEntity ch in chl.HotelOriIDList)
                    {

                        if (!isForReview && (ch.ChannelID == 25 || ch.ChannelID == 26)) continue; //过滤掉部分OTA
                        if (isForReview && (ch.ChannelID == 25)) continue; //过滤掉部分OTA 点评不用过滤掉去哪
                        OTAInfo oi = new OTAInfo();
                        oi.EName = ch.Channel;
                        oi.OTAHotelID = ch.HotelOriID;
                        oi.Name = GetOTAName(ch.ChannelID);
                        oi.ChannelID = ch.ChannelID;
                        oi.AccessURL = GetBookUrl(hotelid, oi.EName, oi.OTAHotelID, sType);
                        oi.CanSyncPrice = ch.CanSyncPrice;

                        oi.IsInnerOpen = true;// oi.EName.ToLower() == "ctrip" ? true : false;

                        otaInfos.Add(oi);
                    }
                }
                return otaInfos;
            });
        }

        private static string GetOTAName(int ChannelID)
        {
            return ((HJDAPI.Common.Helpers.Enums.enumOTAName)ChannelID).ToString();
        }


        public static string GetBroadbandType(int Broadband)
        {
            string BroadbandType = "";
            switch (Broadband)
            {
                case 0:
                    BroadbandType = "无宽带";
                    break;
                case 1:
                    BroadbandType = "免费宽带";
                    break;
                case 2:
                    BroadbandType = "收费宽带";
                    break;
                case 3:
                    BroadbandType = "免费有线宽带";
                    break;
                case 4:
                    BroadbandType = "收费有线宽带";
                    break;
                case 5:
                    BroadbandType = "免费无线宽带";
                    break;
                case 6:
                    BroadbandType = "收费无线宽带";
                    break;
            }
            return BroadbandType;
        }
        public static string GetBreakfastType(int numberOfBreakfast)
        {
            string BreakfastType = "";
            switch (numberOfBreakfast)
            {
                case 0:
                    BreakfastType = "无早";
                    break;
                case 1:
                    BreakfastType = "单早";
                    break;
                case 2:
                    BreakfastType = "双早";
                    break;
            }

            return BreakfastType;
        }


        public static string GetBedTypeName(int bedType)
        {
            string bedTypeName = "";
            switch (bedType)
            {
                case 1:
                    bedTypeName ="双床";
                    break;
                case 2:
                    bedTypeName ="Futon";
                    break;
                case 3:
                    bedTypeName ="大床";
                    break;
                case 4:
                    bedTypeName ="Murphy bed";
                    break;
                case 5:
                    bedTypeName ="Queen";
                    break;
                case 6:
                    bedTypeName ="Sofa bed";
                    break;
                case 7:
                    bedTypeName ="Tatami mats";
                    break;
                case 8:
                    bedTypeName ="2张单人床";
                    break;
                case 9:
                    bedTypeName ="单人床";
                    break;
                case 10:
                    bedTypeName ="Full";
                    break;
                case 11:
                    bedTypeName ="Run of the house";
                    break;
                case 12:
                    bedTypeName ="Dorm bed";
                    break;
                case 501:
                    bedTypeName ="大床或双床";
                    break;
                case 502:
                    bedTypeName ="大床或单床";
                    break;
                case 503:
                    bedTypeName ="单床或双床";
                    break;
            }

            return bedTypeName;
        }

        private static string GetOTAName(string otaename)
        {
            string otaName = "";
            switch (otaename.ToLower())
            {
                case "booking":
                    otaName = "Booking";
                    break;
                case "ctrip":
                    otaName = "携程";
                    break;
                case "elong":
                    otaName = "艺龙";
                    break;
                case "tongcheng":
                    otaName = "同程";
                    break;
                case "zhuna":
                    otaName = "住哪";
                    break;
                case "qunar":
                    otaName = "去哪儿";
                    break;
                case "jltour":
                    otaName = "周末";
                    break;
            }

            return otaName;
        }

        private static string GetBookUrl(int hotelID, string otaEName, int otaHotelID, string sType)
        {
            return LocalCache.GetData<string>(string.Format("bookingurl:{0}:{1}:{2}", hotelID.ToString()
                , otaEName, sType), () =>
                {
                    return GenBookUrl(hotelID, otaEName, otaHotelID, sType);
                });
        }

        private static string GenBookUrl(int hotelID, string otaEName, int otaHotelID, string sType)
        {
            string bookurl = "";


            bookurl = ResourceAdapter.GetOtaHotelUrl(otaEName, otaHotelID, "", sType);


            return bookurl;
        }

        #region Album Retail
        /// <summary>
        /// 获取专辑套餐列表
        /// </summary>
        /// <param name="albumId">专辑ID</param>
        /// <param name="start">开始： 从0开始</param>
        /// <param name="count">返回条数</param>
        /// <param name="startDistrictId">按出发地过滤： 机酒用</param>
        /// <returns></returns>
        public static PackageAlbumDetail GetPackageAlbumDetailFromRedis(int albumId, int start, int count, int startDistrictId=0)
        {
            try
            {
                List<string> PIDs = RedisHelper.RedisConn.SortedSetRangeByRank<string>(string.Format(HotelHelper.AblumPIDKey, albumId), start, (start + count - 1), RedisOrderBy.Asc);
                if (PIDs.Count > 0 )
                {
                    PackageAlbumDetail result = new PackageAlbumDetail();
                    //拼接PIDs的 HashFeild
                    List<string> Album_PIDs = PIDs.Select(p => GenAlbumPackageKey(p)).ToList();
                    //result.albumEntity = RedisHelper.RedisConn.HashGetAll<PackageAlbumsEntity>(string.Format(HotelHelper.AlbumEntityKey, albumId));
                    var packageList = RedisHelper.RedisConn.HashGetAllToList<RecommendHotelItem>(Album_PIDs);
                    if (startDistrictId > 0)
                    {
                        packageList=packageList.Where(p => p.StartDistrictId == startDistrictId).ToList();
                    }
                    result.packageList = packageList;
                    result.shareModel = RedisHelper.RedisConn.HashGetAll<CommentShareModel>(string.Format(HotelHelper.AlbumShareModel, albumId));
                    if (result.packageList.Count > 0) return result;
                }
                return null;
            }
            catch (Exception ex)
            {
                 Log.WriteLog(string.Format("从Redis获取专辑套餐列表失败：albumId={0},错误信息：{1}", albumId, ex.Message + ex.StackTrace));
                 return null;
            }
        }

       /// <summary>
       /// 更新一个专辑下所有的套餐数据
       /// </summary>
       /// <param name="albumId">专辑ID</param>
       /// <param name="packageAlbumDetail">计算出来的专辑下的所有套餐</param>
       /// <returns></returns>
        public static bool AddAlbumToRedis(int albumId, PackageAlbumDetail packageAlbumDetail)
        {
            bool isSuccess = false;
            try
            {                 
                RedisHelper.RedisConn.HashSetObject<PackageAlbumsEntity>(GenAlbumEntityKey(albumId), packageAlbumDetail.albumEntity);

                RedisHelper.RedisConn.HashSetObject<CommentShareModel>(GenAlbumShareModelKey(albumId), packageAlbumDetail.shareModel);

                foreach (var item in packageAlbumDetail.packageList)
                { 
                    RedisHelper.RedisConn.SortedSetAdd(GenAlbumPIDKey(albumId), item.PID.ToString(), item.Rank);

                    RedisHelper.RedisConn.HashSetObject<RecommendHotelItem>(GenAlbumPackageKey(item.PID), item); 
                } 
                isSuccess = true;
            }
            catch (Exception ex)
            {
                Log.WriteLog(string.Format("专辑放入Redi缓存失败：albumId={0},错误信息：{1}", albumId, ex.Message + ex.StackTrace));
            }
            return isSuccess;
        }

        private static  string GenAlbumEntityKey(int albumId)
        {
            return string.Format(HotelHelper.AlbumEntityKey, albumId);
        }

        private static string GenAlbumShareModelKey(int albumId)
        {
            return string.Format(HotelHelper.AlbumShareModel, albumId);
        }

        private static  string GenAlbumPIDKey(int albumId)
        {
            return string.Format(HotelHelper.AblumPIDKey, albumId);
        }

        private static string GenAlbumPackageKey(int packageID)
        {
            return string.Format(HotelHelper.AlbumPackageKey, packageID);
        }
        private static string GenAlbumPackageKey(string strPackageID)
        {
            return string.Format(HotelHelper.AlbumPackageKey, strPackageID);
        }

        public static bool UpdateAlbumRedisCache(int albumId = 0, int pid = 0)
        {
            try
            {
                List<string> albumPackageListKeyList = new List<string>();
                var regex = new Regex("_.*$", RegexOptions.Compiled | RegexOptions.RightToLeft);
                if (albumId == 0 && pid > 0)
                {
                    var packageEntity = HotelAdapter.GetPackage(pid);
                    if (packageEntity.PackageState == 1)//发布
                    {
                        var albumList = HotelService.GetTopNPackagesListByAlbumIdOrPID(0, pid);
                        foreach (var item in albumList)
                        {
                            //根据PID获取专辑套餐列表
                            var pList = HotelAdapter.GetTop20GroupPackage(albumsId: item.AlbumsID, pid: pid);
                            //没有专辑的套餐
                            if (pList.Count == 0) continue;
                            var albumPackageEntity = pList.First(); 
                            //套餐分组号
                            var packageGroupName = albumPackageEntity.PackageGroupName;
                            //套餐价格
                            var price = albumPackageEntity.PackagePrice.Count > 0? albumPackageEntity.PackagePrice.FirstOrDefault(j => j.Type == 0).Price : 100000;
                            string ablumPIDKey = string.Format(HotelHelper.AblumPIDKey, item.AlbumsID);
                            string albumPackageListKey = GenAlbumPackageKey( item.PID);
                            var redisRecommendHotelItem = RedisHelper.RedisConn.HashGetAll<RecommendHotelItem>(albumPackageListKey);
                            //redis存在该套餐ID
                            if (redisRecommendHotelItem != null)
                            {
                                //分组号不为空 查找分组号相同比较套餐价格，删除价格高的套餐
                                if (!string.IsNullOrEmpty(packageGroupName) && redisRecommendHotelItem.PackageGroupName == packageGroupName)
                                { 
                                    if (redisRecommendHotelItem.HotelPrice > price || redisRecommendHotelItem.PackageState !=  (int) HJD.HotelServices.Contracts.HotelServiceEnums.PackageSate.Publish )
                                    {
                                        albumPackageListKeyList.AddRange(new List<string> { albumPackageListKey });
                                        if(albumPackageListKeyList.Count>0)
                                        {
                                            if (RedisHelper.RedisConn.KeyDelete(albumPackageListKeyList) > 0)
                                            {
                                                RedisHelper.RedisConn.SortedSetRemove(ablumPIDKey, pid.ToString());//移除ablumPIDKey中对应的PID
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                //redis不存在该套餐ID，比较分组号相同的套餐，删除价格高的套餐 
                                List<string> PIDs = RedisHelper.RedisConn.SortedSetRangeByRank<string>(string.Format(HotelHelper.AblumPIDKey, item.AlbumsID));
                                PIDs = PIDs.Select(p => GenAlbumPackageKey(p)).ToList();
                                var redisPackageList = RedisHelper.RedisConn.HashGetAllToList<RecommendHotelItem>(PIDs);
                                //分组号不为空 
                                if (!string.IsNullOrEmpty(packageGroupName))
                                {
                                    redisPackageList = redisPackageList.Where(p => p.PackageGroupName == packageGroupName).ToList();
                                    if (redisPackageList.Count > 0)
                                    {
                                        var redisHotePrice = redisPackageList.OrderBy(p => p.HotelPrice).ToList().FirstOrDefault().HotelPrice;
                                        if (redisHotePrice > price)
                                        {
                                            albumPackageListKeyList.AddRange(new List<string> { albumPackageListKey });
                                            if (albumPackageListKeyList.Count > 0)
                                            {
                                                if (RedisHelper.RedisConn.KeyDelete(albumPackageListKeyList) > 0)
                                                {
                                                    RedisHelper.RedisConn.SortedSetRemove(ablumPIDKey, pid.ToString());//移除ablumPIDKey中对应的PID
                                                }
                                            }
                                        }
                                    }
                                    
                                }
                            }
                            var recommendHotelItem = GetRecommendHotelItem(regex, albumPackageEntity);
                            if (RedisHelper.RedisConn.KeyExists(ablumPIDKey))
                            {
                                RedisHelper.RedisConn.SortedSetAdd(ablumPIDKey, item.PID.ToString(), item.Rank);
                                RedisHelper.RedisConn.HashSetObject<RecommendHotelItem>(albumPackageListKey, recommendHotelItem);
                            }
                          
                        }
                    }
                    else if (packageEntity.PackageState ==  (int) HJD.HotelServices.Contracts.HotelServiceEnums.PackageSate.Offline || packageEntity.PackageState ==  (int) HJD.HotelServices.Contracts.HotelServiceEnums.PackageSate.Pending)//下线、待发布
                    {
                        var albumList = HotelService.GetTopNPackagesListByAlbumIdOrPID(0, pid);
                        foreach (var item in albumList)
                        {
                            string ablumPIDKey = string.Format(HotelHelper.AblumPIDKey, item.AlbumsID);
                            albumPackageListKeyList.Add(GenAlbumPackageKey(item.PID));
                            RedisHelper.RedisConn.SortedSetRemove(ablumPIDKey, pid.ToString());//移除ablumPIDKey中对应的PID
                        }
                       
                        if (albumPackageListKeyList.Count > 0)
                        {
                            RedisHelper.RedisConn.KeyDelete(albumPackageListKeyList);
                            albumList.ForEach(album =>
                            {
                                string ablumPIDKey = string.Format(HotelHelper.AblumPIDKey, album.AlbumsID);

                                if (RedisHelper.RedisConn.KeyExists(ablumPIDKey))
                                {
                                    HotelAdapter.GetPackageAlbumDetailList(album.AlbumsID);
                                }
                            });
                        }
                    }
                }
                else if (albumId > 0 && pid > 0)
                {
                    string ablumPIDKey = string.Format(HotelHelper.AblumPIDKey, albumId);
                    string albumPackageListKey = GenAlbumPackageKey(pid);
                    albumPackageListKeyList.AddRange(new List<string> {  albumPackageListKey });
                    //根据PID获取专辑套餐列表
                    var albumPackageEntity = HotelAdapter.GetTop20GroupPackage(albumsId: albumId, pid: pid).FirstOrDefault();
                    //没有专辑套餐 或者没有发布 移除redis该套餐，重新计算
                    if (albumPackageEntity == null)
                    {
                        if (albumPackageListKeyList.Count > 0)
                        {
                            if (RedisHelper.RedisConn.KeyDelete(albumPackageListKeyList) > 0)
                            {
                                RedisHelper.RedisConn.SortedSetRemove(ablumPIDKey, pid.ToString());//移除ablumPIDKey中对应的PID
                            }
                            if (RedisHelper.RedisConn.KeyExists(ablumPIDKey))
                            {
                                HotelAdapter.GetPackageAlbumDetailList(albumId);
                            }
                        }
                    }
                    else
                    {
                        //直接修改套餐缓存
                        var recommendHotelItem = GetRecommendHotelItem(regex, albumPackageEntity);
                        if (RedisHelper.RedisConn.KeyExists(ablumPIDKey))
                        {
                            RedisHelper.RedisConn.SortedSetAdd(ablumPIDKey, pid.ToString(), recommendHotelItem.Rank);
                            RedisHelper.RedisConn.HashSetObject<RecommendHotelItem>(albumPackageListKey, recommendHotelItem);
                        }
                        
                    }
                }
                else if (albumId > 0 && pid == 0)
                {
                    string albumEntityKey = string.Format(HotelHelper.AlbumEntityKey, albumId);
                    string albumShareModel = string.Format(HotelHelper.AlbumShareModel, albumId);
                    var albumEntity = HotelAdapter.GetOnePackageAlbums(albumId);
                    albumEntity.CoverPicSUrl = !string.IsNullOrWhiteSpace(albumEntity.CoverPicSUrl) ? PhotoAdapter.GenHotelPicUrl(albumEntity.CoverPicSUrl, Enums.AppPhotoSize.jupiter) : "";
                    var shareModel = new CommentShareModel()
                    {
                        Content = albumEntity.SubTitle,
                        notHotelNameTitle = "",
                        title = albumEntity.SubTitle,
                        shareLink = "",
                        photoUrl = regex.Replace(albumEntity.CoverPicSUrl, "_290x290s"),
                        returnUrl = ""
                    };
                    if (RedisHelper.RedisConn.KeyExists(albumEntityKey))
                    {
                        RedisHelper.RedisConn.HashSetObject<PackageAlbumsEntity>(albumEntityKey, albumEntity);
                    }
                    if (RedisHelper.RedisConn.KeyExists(albumShareModel))
                    {
                        RedisHelper.RedisConn.HashSetObject<CommentShareModel>(albumShareModel, shareModel);
                    }
                }
                return true;

            }
            catch (Exception ex)
            {
                Log.WriteLog("更新分组专辑套餐缓存失败， albumId=" + albumId + "pid=" + pid + "错误信息：" + ex.Message + ex.StackTrace);
                return false;
            }
        }

        private static RecommendHotelItem GetRecommendHotelItem(Regex regex, TopNPackageItem topNPackageItem)
        {
            var item = new RecommendHotelItem() { };
            item.HotelID = topNPackageItem.HotelID;
            item.HotelName = topNPackageItem.HotelName;
            item.HotelPicUrl = topNPackageItem.PicUrls.Any() ? regex.Replace(topNPackageItem.PicUrls.First(), "_theme") : "";
            item.HotelPrice = topNPackageItem.PackagePrice.First(j => j.Type == 0).Price;
            item.VIPPrice = topNPackageItem.PackagePrice.First(j => j.Type == -1).Price;
            item.TotalHotelPrice = topNPackageItem.PackagePrice.First(j => j.Type == 0).Price * topNPackageItem.DayLimitMin;
            item.TotalVIPPrice = topNPackageItem.PackagePrice.First(j => j.Type == -1).Price * topNPackageItem.DayLimitMin;
            item.DayLimitMin = topNPackageItem.DayLimitMin;
            item.ADDescription = string.IsNullOrWhiteSpace(topNPackageItem.Title) ? "" : topNPackageItem.Title;
            item.MarketPrice = topNPackageItem.MarketPrice;
            item.PackageBrief = topNPackageItem.PackageBrief;
            item.PID = topNPackageItem.PackageID;
            item.RecommendPicUrl = string.IsNullOrEmpty(topNPackageItem.RecomendPicShortNames) ? "" : ("http://whphoto.b0.upaiyun.com/" + topNPackageItem.RecomendPicShortNames + "_theme");
            item.RecommendPicUrl2 = string.IsNullOrEmpty(topNPackageItem.RecomendPicShortNames2) ? "" : ("http://whphoto.b0.upaiyun.com/" + topNPackageItem.RecomendPicShortNames2 + "_theme");
            item.HotelReviewCount = topNPackageItem.HotelReviewCount;
            item.HotelScore = topNPackageItem.HotelScore;
            item.RecomemndWord = topNPackageItem.RecomemndWord;
            item.RecomemndWord2 = topNPackageItem.RecomemndWord2;
            item.PackageName = topNPackageItem.PackageName;
            item.DistrictId = topNPackageItem.DistrictId;
            item.DistrictName = topNPackageItem.DistrictName;
            item.DistrictEName = topNPackageItem.DistrictEName;
            item.ProvinceName = topNPackageItem.ProvinceName;
            item.InChina = topNPackageItem.InChina;
            item.Rank = topNPackageItem.Rank;
            item.StartDistrictId = topNPackageItem.StartDistrictId;
            item.StartDistrictName = topNPackageItem.StartDistrictName;
            item.Title = topNPackageItem.Title;
            item.PackageGroupName = topNPackageItem.PackageGroupName;
            return item;
        }
        //public static bool AddAlbumPackageToRedis(int albumId, int pid, PackageAlbumDetail packageAlbumDetail)
        //{
        //    bool isSuccess = false;
        //    try
        //    {
        //        List<string> PIDs = new List<string>();
        //        string albumEntityKey = string.Format(HotelHelper.AlbumEntityKey, albumId);
        //        string albumShareModel = string.Format(HotelHelper.AlbumShareModel, albumId);
        //        string ablumPIDKey = string.Format(HotelHelper.AblumPIDKey, albumId);
        //        RedisHelper.RedisConn.HashSetObject<PackageAlbumsEntity>(albumEntityKey, packageAlbumDetail.albumEntity);
        //        RedisHelper.RedisConn.HashSetObject<CommentShareModel>(albumShareModel, packageAlbumDetail.shareModel);
        //        foreach (var item in packageAlbumDetail.packageList)
        //        {
        //            PIDs.Add(item.PID.ToString());
        //        }
        //        PIDs = PIDs.Select(p => GenAlbumPackageKey(p)).ToList();
        //        var packageList = RedisHelper.RedisConn.HashGetAllToList<RecommendHotelItem>(PIDs);
        //        var list = packageList.GroupBy(p => p.PackageGroupName).ToList();
        //        foreach (var item in list)
        //        {
        //            string groupName = item.Key;
        //            var lowerList = item.OrderBy(p => p.VIPPrice).FirstOrDefault();
        //        }
        //        isSuccess = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.WriteLog(string.Format("专辑放入Redi缓存失败：albumId={0},错误信息：{1}", albumId, ex.Message + ex.StackTrace));
        //    }
        //    return isSuccess;
        //}

        #endregion
    }
}
