using System;
using System.Linq;
using System.Collections.Generic;
using HJDAPI.Models;
using HJD.Framework.WCF;
using HJD.PhotoServices.Contracts;
using HJD.PhotoServices.Entity;
using HJDAPI.Common.Helpers;
using HJD.PhotoSyncServices.Contracts;
//using HJD.PhotoSyncServices.Contracts;

namespace HJDAPI.Controllers
{
    public class PhotoAdapter
    {
        //public static IPhotoSyncService PhotoSyncService = ServiceProxyFactory.Create<IPhotoSyncService>("BasicHttpBinding_IHJDPhotoSyncService");
        public static IPhotoService PhotoService = ServiceProxyFactory.Create<IPhotoService>("BasicHttpBinding_IPhotoService");
        public static IPhotoSyncService photoSyncService = ServiceProxyFactory.Create<IPhotoSyncService>("IPhotoSyncService");

        public static int AddPHSUploadInfoWithYupooInfo(int appID, int typeID, int size, string YPURL, string YPSecret, string type, int width = 0, int height = 0)
        {
            return PhotoService.AddPHSUploadInfoWithYupooInfo(appID, typeID, size, YPURL, YPSecret, type, width, height);
        }
        public static int PhotoUpload(byte[] picdata, int TypeID, int AppID, bool bSync)
        {
          
            return photoSyncService.PhotoUpload(picdata, TypeID, AppID, bSync);
        }

        public static  List<PHSPhotoInfoEntity> GetWHHotelCommentPhotoByWritings(IEnumerable<int> writings)
        {
            return PhotoService.GetWHHotelCommentPhotoByWritings(writings.ToList());
        }

        public static string GenHotelPicUrl(string surl, Enums.AppPhotoSize type)
        {
            //string photoSize = 
            switch (type)
            {
                case Enums.AppPhotoSize.appdetail:
                    return string.Format(Configs.HotelListPhotoUrl, surl, "appdetail1s");
                case Enums.AppPhotoSize.applist2:
                    return string.Format(Configs.HotelListPhotoUrl, surl, "600x220s");
                case Enums.AppPhotoSize.share:
                    return string.Format(Configs.HotelListPhotoUrl, surl, "140X140");
                case Enums.AppPhotoSize.interestHotelList:
                    return string.Format(Configs.HotelListPhotoUrl, surl, "InterestHotelList");
                case Enums.AppPhotoSize.w320h230:
                    return string.Format(Configs.HotelListPhotoUrl, surl, "w320h230");
                case Enums.AppPhotoSize.theme:
                    return string.Format(Configs.HotelListPhotoUrl, surl, "theme");
                case Enums.AppPhotoSize.appdetail1:
                    return string.Format(Configs.HotelListPhotoUrl, surl, "appdetail1");
                case Enums.AppPhotoSize.shop:
                    return string.Format(Configs.HotelListPhotoUrl, surl, "290x290");
                default:
                    return string.Format(Configs.HotelListPhotoUrl, surl, type.ToString());
            }
        }

        //public static int UploadPhoto(string picdata, int typeId, int appId)
        //{
        //    int result;
        //    var bytes = Convert.FromBase64String(picdata);
        //    if (bytes.Length == 0)
        //    {
        //        result = -1;
        //    }
        //    else
        //    {
        //        result = PhotoSyncService.LPPhotoUpload(Convert.FromBase64String(picdata), typeId, appId, true);
        //    }
        //    return result;
        //}

        public static void UploadPhotoInfo(int phsid, int type, long userid, int districtId, int resourceId, int bussinessId, string picDesc)
        {
            PhotoService.UpdatePHSPhotoInfoExt(new PHSPhotoInfoExt
                                                   {
                                                       BusinessID = bussinessId,
                                                       Deleted = "F",
                                                       DistrictID = districtId,
                                                       Type = 5,
                                                       PHSID = phsid,
                                                       Picked = 0,
                                                       ResourceID = resourceId,
                                                       Status = "W",
                                                       UserID = userid,
                                                       Title = picDesc,
                                                       UploadTime = DateTime.Now
                                                   });
        }
        
        public static void InsertPhotoInfo(int phsid, int type, int TagBlockCategory, long userid, int districtId, int resourceId, int bussinessId, string picDesc)
        {
            PhotoService.InsertPHSPhotoInfoExt(new PHSPhotoInfoExt{
                                                       BusinessID = bussinessId,
                                                       Deleted = "F",
                                                       DistrictID = districtId,
                                                       Type = type,
                                                       PHSID = phsid,
                                                       Picked = 0,
                                                       ResourceID = resourceId,
                                                       Status = "W",
                                                       UserID = userid,
                                                       Title = picDesc,
                                                       UploadTime = DateTime.Now,
                                                       SubType = TagBlockCategory
                                                   });
        }

        public static string GetItineraryCover(int phsId)
        {
            if (phsId <= 0) return "";
            var res = PhotoService.GetPhotoInfo(new List<int> { phsId }).FirstOrDefault();
            if (res == null)
            {
                return string.Empty;
            }
            return (res.PhotoUrl[PhotoSizeType.small]!="" ? res.PhotoUrl[PhotoSizeType.small] : "");
        }

        public static string GetTopDistrictPhoto(int district)
        {
            if (district == 0)
            {
                return string.Empty;
            }

            var res = PhotoService.GetDistrictPhotoByDistrict(district, 0, 1, 0, 0).FirstOrDefault();
            if (res == null)
            {
                return string.Empty;
            }

            return res.PhotoUrl[PhotoSizeType.small];
        }

        public static IDictionary<int, string> GetTopHotelPhotos(List<int> hotelIds)
        {
            if (hotelIds.Count == 0)
            {
                return new Dictionary<int, string>();
            }

            var res = PhotoService.GetHotelsListPhoto(hotelIds, 1).ToDictionary(p=>p.HotelID,p=>p.PHSID);
            if (res.Values.Count == 0)
            {
                return new Dictionary<int, string>();
            }
            var photos = PhotoService.GetPhotoInfo(res.Values.ToList()).ToDictionary(p => p.PHSID, p => p.PhotoUrl[PhotoSizeType.wood]);
            return res.ToDictionary(p=>p.Key,p=>photos.ContainsKey(p.Value)?photos[p.Value]:string.Empty);
        }

        //public static int GetHotelPhotoCount(int hotelId, long userid)
        //{
        //    return (int)PhotoService.CountHotelPhotoByResource(hotelId, userid, userid);
        //}

        //public static int GetHotelOfficalPhotoCount(int hotelId, long userid)
        //{
        //    return (int)PhotoService.CountHotelOfficalPhotoByResource(hotelId, userid, userid);
        //}

        public static IDictionary<int, string> GetTopSightPhotos(List<int> sightIds)
        {
            if (sightIds.Count == 0)
            {
                return new Dictionary<int, string>();
            }

            var res = PhotoService.GetSightCommentPhotoInfo(sightIds);
            return res.ToDictionary(p => p.ResourceID, p => p.PhotoUrl[PhotoSizeType.square]);
        }

        public static int GetSightPhotoCount(int sightId, long userid)
        {
            return (int)PhotoService.CountSightPhotoByResource(sightId, userid, userid);
        }
        
        public static List<PictureItem> GetHotelPhotos(int hotel, long userId, int photoType, int start, int count)
        {
            var pt = GetPhotoType(photoType);
            var res = PhotoService.GetHotelPhotoByResource(hotel, start, count, userId, userId)
                .Select(p => new PictureItem
                                 {
                                     Description = p.Title,
                                     Nickname = AccountAdapter.GetNickName(p.UserID),
                                     OrgUrl = p.PhotoUrl[PhotoSizeType.custom],
                                     PhotoTime = p.UploadTime.ToString("yyyy-MM-dd"),
                                     Url = p.PhotoUrl[pt]
                                 });
            return res.ToList();
        }

        public static List<PHSPhotoInfoEntity> GetPhotos(List<int> phsIds)
        {
            return PhotoService.GetPhotoInfo(phsIds);
        }
               
        public static List<PictureItem> GetHotelOfficalPhotos(int hotel, long userId, int photoType, int start, int count)
        {
            var pt = GetPhotoType(photoType);
            var res = PhotoService.GetHotelOfficalPhotoByResource(hotel, start, count, userId, userId)
                .Select(p => new PictureItem
                {
                    Description = p.Title,
                    Nickname = AccountAdapter.GetNickName(p.UserID),
                    OrgUrl = p.PhotoUrl[PhotoSizeType.custom],
                    PhotoTime = p.UploadTime.ToString("yyyy-MM-dd"),
                    Url = p.PhotoUrl[pt]
                });
            return res.ToList();
        }

        public static List<PictureItem> GetSightPhotos(int sight, long userId, int photoType, int start, int count)
        {
            var pt = GetPhotoType(photoType);
            var res = PhotoService.GetSightPhotoByResource(sight, start, count, userId, userId)
                .Select(p => new PictureItem
                {
                    Description = p.Title,
                    Nickname = AccountAdapter.GetNickName(p.UserID),
                    OrgUrl = p.PhotoUrl[PhotoSizeType.custom],
                    PhotoTime = p.UploadTime.ToString("yyyy-MM-dd"),
                    Url = p.PhotoUrl[pt]
                });
            return res.ToList();
        }

        private static PhotoSizeType GetPhotoType(int photoType)
        {
            PhotoSizeType pt;
            Enum.TryParse(photoType.ToString(), out pt);
            return pt;
        }

        /// <summary>
        /// 更新酒店标题
        /// </summary>
        /// <param name="photoId"></param>
        /// <param name="newTitle"></param>
        /// <returns></returns>
        public static int UpdateHotelPhotoTitle(int photoId, string newTitle)
        {
            return PhotoService.UpdateHotelPhotoTitle(photoId, newTitle);
        }
    }
}