using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HJD.HotelServices.Contracts;
using HJDAPI.Models;
using WHotelSite.Models;
using InterestModel = WHotelSite.Models.InterestModel;
using HJD.AccessService.Contract.Model;

namespace WHotelSite
{
    public class Mapper
    {
        public static SimpleDistrictModel ConvertCityEntityToSimpleDistrictModel(CityEntity item)
        {
            if (item == null)
            {
                return null;
            }

            var ret = new SimpleDistrictModel
            {
                DistrictId = item.ID,
                FirstLetter = string.IsNullOrEmpty(item.pinyin) ? string.Empty : item.pinyin.Substring(0, 1),
                Lat = item.lat,
                Lng = item.lon,
                Name = item.Name,
                PinYin = item.pinyin
            };
            return ret;
        }



        public static SimpleDistrictModel ConvertCityEntityToSimpleDistrictModel(HJD.DestServices.Contract.CityEntity item)
        {
            if (item == null)
            {
                return null;
            }

            var ret = new SimpleDistrictModel
            {
                DistrictId = item.DistrictID,
                FirstLetter = string.IsNullOrEmpty(item.PinYin) ? string.Empty : item.PinYin.Substring(0, 1),
                Lat = item.Lat,
                Lng = item.Lon,
                Name = item.Name,
                PinYin = item.PinYin
            };
            return ret;
        }

        public static List<SimpleDistrictModel> ConvertCityEntityListToSimpleDistrictModelList(List<CityEntity> list)
        {
            if (list == null || !list.Any())
            {
                return new List<SimpleDistrictModel>();
            }
            var ret = list.Where(p => p != null).Select(ConvertCityEntityToSimpleDistrictModel).ToList();
            return ret;
        }

        public static List<SimpleDistrictModel> ConvertCityEntityListToSimpleDistrictModelList(CityList list)
        {

            List<SimpleDistrictModel> ret = new List<SimpleDistrictModel>();
            //foreach (HJD.HotelServices.Contracts.CityEntity h in list.HotArea)
            //{
            //    h.Name += "及周边";
            //}

            ret = list.HotArea.Select(ConvertCityEntityToSimpleDistrictModel).ToList();
            ret.AddRange(list.Citys.OrderBy(c => c.Name).Where(p => p != null).Select(ConvertCityEntityToSimpleDistrictModel).ToList());

            return ret;
        }

        public static HotPackageModel ConvertHotPackageInfoToHotPackageModel(HotPackageInfo item)
        {
            if (item == null)
            {
                return null;
            }

            var ret = new HotPackageModel
            {
                Brief = item.Brief,
                HotelId = item.HotelID,
                HotelName = item.HotelName,
                MinPrice = item.MinPrice,
                PicSUrl = item.PicSUrl,
                PicUrl = item.PicURL,
                Pid = item.PID,
                ReviewCount = item.ReviewCount,
                ReviewScore = item.ReviewScore

            };
            return ret;
        }

        public static List<HotPackageModel> ConvertHotPackageInfoListToHotPackageModelList(List<HotPackageInfo> list)
        {
            if (list == null || !list.Any())
            {
                return new List<HotPackageModel>();
            }

            var ret = list.Where(p => p != null).Select(ConvertHotPackageInfoToHotPackageModel).ToList();
            return ret;
        }

        public static InterestModel ConvertInterestEntityToInterestEntityModel(InterestEntity item)
        {
            if (item == null)
            {
                return null;
            }

            var ret = new InterestModel
            {
                GLat = item.GLat,
                GLon = item.GLon,
                HotelCount = item.HotelCount,
                HotelList = item.HotelList,
                Id = item.ID,
                ImageUrl = item.ImageUrl,
                InterestPlaceIDs = item.InterestPlaceIDs,
                Name = item.Name,
                Type = item.Type,
            };
            return ret;
        }

        public static List<InterestModel> ConvertInterestEntityListToInterestEntityModelList(List<InterestEntity> list)
        {
            if (list == null || !list.Any())
            {
                return new List<InterestModel>();
            }

            var ret = list.Where(p => p != null).Select(ConvertInterestEntityToInterestEntityModel).ToList();
            return ret;
        }

        internal static List<SightCategoryModel> ConvertSightCategoryListToSightCategoryModelList(List<SightCategory> list)
        {
            if (list == null || !list.Any())
            {
                return new List<SightCategoryModel>();
            }

            var ret = list.Where(p => p != null).Select(p => new SightCategoryModel
            {
                Id = p.ID,
                InterestIds = p.InterestID,
                Name = p.Name
            }).ToList();
            return ret;
        }

        public static List<HotelModel> ConvertSuggestListToHotelModelList(List<QuickSearchSuggestItem> list)
        {
            if (list == null || !list.Any())
            {
                return new List<HotelModel>();
            }

            var ret = list.Where(p => p != null).Select(p => new HotelModel
            {
                HotelId = p.Id,
                HotelName = p.Name,

            }).ToList();
            return ret;
        }

        public static List<HotelModel> ConvertSearchHotelResultToHotelModelList(List<HotelSearchResult> list)
        {
            if (list == null || !list.Any())
            {
                return new List<HotelModel>();
            }

            var ret = list.Where(p => p != null).Select(p => new HotelModel
            {
                HotelId = p.HotelId,
                HotelName = p.HotelName,    //p.Boost + 

            }).ToList();
            return ret;
        }

        /// <summary>
        /// 转换app搜索的酒店对象到pc端搜索的酒店对象
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static List<HotelModel> ConvertAppSearchHotelResultToHotelModelList(List<QuickSearchSuggestItem> list)
        {
            if (list == null || !list.Any())
            {
                return new List<HotelModel>();
            }

            list = list.Where(_ => _.Type.ToUpper().Trim() == "H").ToList();

            var ret = list.Where(p => p != null).Select(p => new HotelModel
            {
                HotelId = p.Id,
                HotelName = p.Name,

            }).ToList();
            return ret;
        }
    }
}