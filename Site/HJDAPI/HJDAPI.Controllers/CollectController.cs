using HJD.HotelPrice.Contract;
using HJD.HotelPrice.Contract.DataContract;
using HJD.HotelServices.Contracts;
using HJDAPI.Common.Helpers;
using HJDAPI.Controllers.Adapter;
using HJDAPI.Controllers.Common;
using HJDAPI.Models;
using PersonalServices.Contract;
using PersonalServices.Contracts.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace HJDAPI.Controllers
{
    public class CollectController : BaseApiController
    {
        [HttpPost]
        public CollectOptionResult Add(CollectParamModel paramModel)
        {
            CollectOptionResult result = new CollectOptionResult();          

            if (paramModel.UserID == 0)
            {
                result.Message = "用户ID不能为空";
                result.Success = 2;
                return result;
            }
            else if (paramModel.Items == null || paramModel.Items.Count == 0)
            {
                result.Message = "没有酒店ID";
                result.Success = 3;
                return result;
            }

            CollectParam param = new CollectParam();
            param.UserID = paramModel.UserID;
            param.Items = ClassConvertHelper.Convert(paramModel.Items);
            CollectOptionResult ss = CollectAdapter.Add(param);
            ss.Message = ss.Success == 0 ? "" : "添加收藏失败";
            return ss;
        }

        //[HttpPost]
        //public CollectOptionResult Add(long UserID, List<CollectParamItemModel> Items)
        //{
        //    CollectOptionResult result = new CollectOptionResult();

        //    CollectParamModel paramModel = new CollectParamModel();
        //    paramModel.UserID = UserID;
        //    paramModel.Items = Items;

        //    if (paramModel.UserID == 0)
        //    {
        //        result.Message = "用户ID不能为空";
        //        result.Success = false;
        //        return result;
        //    }
        //    if (paramModel.Items == null || paramModel.Items.Count == 0)
        //    {
        //        result.Message = "没有收藏";
        //        result.Success = false;
        //        return result;
        //    }

        //    CollectParam param = new CollectParam();
        //    param.UserID = paramModel.UserID;
        //    param.Items = ClassConvertHelper.Convert(paramModel.Items);

        //    return CollectAdapter.Add(param);
        //}

        //remove collect 支持批量取消收藏
        [HttpPost]
        public CollectOptionResult Remove(CollectParamModel paramModel)
        {
            CollectOptionResult result = new CollectOptionResult();

            if (paramModel.UserID == 0)
            {
                result.Message = "用户ID不能为空";
                result.Success = 2;
                return result;
            }
            else if (paramModel.Items == null || paramModel.Items.Count == 0)
            {
                result.Message = "没有酒店ID";
                result.Success = 3;
                return result;
            }

            CollectParam param = new CollectParam();
            param.UserID = paramModel.UserID;
            param.Items = ClassConvertHelper.Convert(paramModel.Items);
            CollectOptionResult ss = CollectAdapter.Remove(param);
            ss.Message = ss.Success == 0 ? "" : "取消收藏失败";
            return ss;
        }

        //get collect 由搜索条件返回需要的收藏列表
        [HttpPost]
        public List<long> GetCollectIdList(CollectParamModel paramModel)
        {
            CollectParam param = new CollectParam();
            param.UserID = paramModel.UserID;

            if (param.UserID == 0)
            {
                return new List<long>();
            }
            return CollectAdapter.GetCollectIdList(param.UserID);
        }

        /// <summary>
        /// 由酒店ID集合获得酒店列表清单
        /// </summary>
        /// <param name="hotelIdList"></param>
        /// <returns></returns>
        [HttpPost]
        public CollectHotelResult GetCollectHotelList(CollectParamModel paramModel)
        {
            CollectHotelResult result = new CollectHotelResult();
            CollectParam param = new CollectParam();
            param.UserID = paramModel.UserID;
            if (paramModel.UserID == 0)
            {
                result.Message = "用户ID不能为空";
                result.Success = 2;
                return result;
            }

            List<FavouriteHotel> collectHotelList = CollectAdapter.GetCollectList(param);                      
            
            List<int> hotelIdList = null;
            Dictionary<int, int> dic = new Dictionary<int, int>();
            if (collectHotelList != null && collectHotelList.Count != 0)
            {
                hotelIdList = new List<int>();
                foreach (FavouriteHotel hotel in collectHotelList)
                {
                    hotelIdList.Add((int)hotel.HotelID);
                    dic.Add((int)hotel.HotelID,hotel.CollectInterestID);
                }
            }
            if (hotelIdList != null)
            {
                List<ListHotelItem2> hotelList = HotelAdapter.GetCollectHotelList(hotelIdList);//hotelIdList.ConvertAll<int>(i => (int)i)             

                DateTime arrivalTime = CommMethods.GetDefaultCheckIn();
                DateTime departureTime = arrivalTime.AddDays(1);

                List<HotelPriceEntity> hpl = HotelAdapter.QueryHotelListPrice(hotelList.Select(h => h.Id).ToList(), arrivalTime, departureTime);

                //绑定数据类型
                if (hpl.Count > 0)
                {
                    foreach (var h in hotelList)
                    {
                        var thp = hpl.Where(hp => hp.HotelId == h.Id);

                        if (thp.Count() == 1 && thp.First().PriceList.Count() > 0)
                        {
                            if (thp.First().PriceList.Where(pr => pr.ChannelID == 100).Count() > 0) //有套餐
                            {
                                h.MinPrice = (int)thp.First().PriceList.Where(pr => pr.ChannelID == 100).FirstOrDefault().Price;
                                h.PriceType = 3;

                            }
                            else if (thp.First().PriceList.Where(pr => pr.ChannelID == 102).Count() > 0) //JLTour
                            {
                                h.MinPrice = (int)thp.First().PriceList.Where(pr => pr.ChannelID == 102).FirstOrDefault().Price;
                                h.PriceType = 2;

                            }
                            else
                            {
                                h.MinPrice = (int)thp.First().PriceList.Min(P => P.Price);
                                h.PriceType = 1;
                            }
                        }
                        else
                        {
                            h.MinPrice = 0;
                            h.PriceType = 0;
                        }
                    }
                }

                hotelList.ForEach(h => h.Picture = PhotoAdapter.GenHotelPicUrl(h.PicSURL, Enums.AppPhotoSize.applist));//拼图片链接


                result.Hotels = new List<CollectListItem>();
                

                foreach (ListHotelItem2 temp in hotelList)
                {
                    CollectListItem item = new CollectListItem()
                    {
                        Currency = temp.Currency,
                        FeaturedList = temp.FeaturedList,
                        GLat = temp.GLat,                    
                        GLon = temp.GLon,
                        Id = temp.Id,
                        MinPrice = temp.MinPrice,
                        Name = temp.Name,
                        PicSURL = temp.PicSURL,
                        Picture = temp.Picture,
                        PriceType = temp.PriceType,
                        Rank = temp.Rank,
                        ReviewCount = temp.ReviewCount,
                        Score = temp.Score,
                        InterestID = dic[temp.Id]
                    };
                    result.Hotels.Add(item);
                }
                result.TotalCount = result.Hotels.Count;
                result.Message = "";
                result.Success = 0;
                return result;
            }
            else
            {
                result.Message = "没有酒店数据";
                result.Success = 1;
                return result;
            }
        }




        /// <summary>
        /// 由酒店ID集合获得酒店列表清单 app5.0
        /// </summary>
        /// <param name="hotelIdList"></param>
        /// <returns></returns>
        [HttpPost]
        public CollectHotelResult GetPageCollectHotelList(CollectParamModel paramModel)
        {
            try {
                CollectHotelResult result = new CollectHotelResult();
                CollectParam param = new CollectParam();
                param.UserID = paramModel.UserID;
                param.count = paramModel.count;
                param.start = paramModel.start;
                if (paramModel.UserID == 0)
                {
                    result.Message = "用户ID不能为空";
                    result.Success = 2;
                    return result;
                }

                List<FavouriteHotel> collectHotelList = CollectAdapter.GetPageCollectList(param);
                List<int> hotelIdList = null;
                Dictionary<int, int> dic = new Dictionary<int, int>();
                if (collectHotelList != null && collectHotelList.Count != 0)
                {
                    hotelIdList = new List<int>();
                    foreach (FavouriteHotel hotel in collectHotelList)
                    {
                        hotelIdList.Add((int)hotel.HotelID);
                        dic.Add((int)hotel.HotelID, hotel.CollectInterestID);
                    }
                }

                if (hotelIdList != null)
                {
                    List<ListHotelItem2> hotelList = HotelAdapter.GetCollectHotelList(hotelIdList);//hotelIdList.ConvertAll<int>(i => (int)i)             

                    DateTime arrivalTime = CommMethods.GetDefaultCheckIn();
                    DateTime departureTime = arrivalTime.AddDays(1);

                    var newHidList = hotelList.Select(h => h.Id).ToList();

                    //获取当前用户遵循的列表价类型
                    var minPriceTypes = PriceAdapter.GetHotelMinPriceTypeByUser(param.UserID);

                    //获取HotelMinPrice列表价
                    var hotelMinPriceList = PriceAdapter.PriceService.GetHotelMinPriceList(newHidList, arrivalTime) ?? new List<HotelMinPriceEntity>();
                    var pricePlanList = new List<PricePlanEx>();

                    if (hotelMinPriceList == null || hotelMinPriceList.Count == 0)
                    {
                        //获取PricePlan列表价
                        pricePlanList = PriceAdapter.PriceService.GetPricePlanExList(newHidList, arrivalTime) ?? new List<PricePlanEx>();
                    }

                    ////获取列表价
                    //var pricePlanList = HotelAdapter.PriceService.GetPricePlanExList(newHidList, arrivalTime) ?? new List<HJD.HotelPrice.Contract.DataContract.PricePlanEx>();

                    //拼图片链接
                    hotelList.ForEach(h => h.Picture = PhotoAdapter.GenHotelPicUrl(h.PicSURL, Enums.AppPhotoSize.theme));

                    result.Hotels = new List<CollectListItem>();

                    foreach (ListHotelItem2 temp in hotelList)
                    {
                        var priceType = PriceAdapter.GetHotelPricePlan(temp.Id, pricePlanList, hotelMinPriceList, minPriceTypes, arrivalTime, false);

                        CollectListItem item = new CollectListItem()
                        {
                            Currency = temp.Currency,
                            FeaturedList = temp.FeaturedList,
                            GLat = temp.GLat,
                            GLon = temp.GLon,
                            Id = temp.Id,
                            Name = temp.Name,
                            PicSURL = temp.PicSURL,
                            Picture = temp.Picture,
                            PriceType = (priceType.ChannelID == 100 ? 3 : 0),
                            Rank = temp.Rank,
                            ReviewCount = temp.ReviewCount,
                            Score = temp.Score,
                            InterestID = dic[temp.Id],
                            MinPrice = priceType.VipPrice == 0 ? 0 : priceType.Price,
                            Brief = priceType.Brief
                        };
                        result.Hotels.Add(item);
                    }
                    result.TotalCount = result.Hotels.Count;
                    result.Message = "";
                    result.Success = 0;
                    return result;
                }
                else
                {
                    result.Message = "没有酒店数据";
                    result.Success = 1;
                    return result;
                }
            }
            catch (Exception e)
            {
                CollectHotelResult result = new CollectHotelResult();
                Log.WriteLog("error pageCollectHotelList：" + e);
                result.Message = "没有酒店数据.";
                result.Success = 1;
                return result;
            }
        }





        /// <summary>
        /// 同步APP本地的收藏完成后 返回最新的收藏列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public CollectHotelResult SyncCollectList(CollectParamModel paramModel)
        {
            CollectHotelResult result = new CollectHotelResult();
            if (paramModel.UserID == 0)
            {
                result.Message = "用户ID不能为空";
                result.Success = 2;
            }
            if (paramModel.Items == null || paramModel.Items.Count == 0)
            {
                result.Message = "没有酒店ID";
                result.Success = 3;
            }

            CollectParam param = new CollectParam();
            param.UserID = paramModel.UserID;
            param.Items = ClassConvertHelper.Convert(paramModel.Items);

            CollectOptionResult r1 = CollectAdapter.Add(param);
            if (r1.Success > 0)
            {
                result.Success = r1.Success;
                result.Message = r1.Message;
                //result.Message = "同步失败";
                //result.Success = false;
            }
            else
            {
                result = GetCollectHotelList(paramModel);
            }
            return result;
        }

        [HttpGet]
        public CollectOptionResult IsCollect(long UserID, int HotelID)
        {
            CollectOptionResult result = new CollectOptionResult();
            if(UserID == 0)
            {
                result.Message = "用户ID不能为空";
                result.Success = 2;
                return result;
            }
            else if (HotelID == 0)
            {
                result.Message = "酒店ID不能为空";
                result.Success = 3;
                return result;
            }
            else
            {
                result.Success = !CollectAdapter.IsCollect(UserID, HotelID) ? 0 : 1;
                result.Message = result.Success == 0 ? "" : "已收藏";
            }
            return result;
        }
    }
}