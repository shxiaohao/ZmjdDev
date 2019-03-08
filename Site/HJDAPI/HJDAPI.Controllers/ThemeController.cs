using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using HJD.DestServices.Contract;
using HJD.HotelServices.Contracts;
using HJDAPI.Models;
using HJDAPI.Controllers.Adapter;
using Newtonsoft.Json;
using HJDAPI.Controllers.Common;
using HJD.HotelPrice.Contract.DataContract;
using HJD.Framework.WCF;
using HJD.HotelPrice.Contract;
using HJD.CouponService.Contracts;
using HJDAPI.Common.Helpers;
using ProductService.Contracts.Entity;

namespace HJDAPI.Controllers
{
    public class HotelThemeController : BaseApiController
    {
        public static HJD.HotelServices.Contracts.IHotelService HotelService = ServiceProxyFactory.Create<HJD.HotelServices.Contracts.IHotelService>("BasicHttpBinding_IHotelService");
        public static IHotelPriceService PriceService = ServiceProxyFactory.Create<IHotelPriceService>("BasicHttpBinding_IHotelPriceService");
        public static ICouponService couponService = ServiceProxyFactory.Create<ICouponService>("ICouponService");

        [HttpGet]
        public List<HotelThemeEntity> GetAllHotelTheme()
        {
            return ResourceAdapter.GetAllHotelTheme();
        }

        [HttpGet]
        public List<HotelThemeEntity> ThrowTestException()
        {
            short i = Convert.ToInt16("111111111111111");
            return null;
        }

        [HttpPost]
        public List<HotelThemeEntity> ThrowTestException(int stat = 0)
        {
            short i = Convert.ToInt16("0.223444");
            return null;
        }

        [HttpGet]
        public HJD.HotelManagementCenter.Domain.TemplateDataEntity GetTempSource(int id)
        {
            //HJD.HotelManagementCenter.Domain.TemplateSourceEntity tempSource = HotelAdapter.GetTempSourceById(id);
            //HJD.HotelManagementCenter.Domain.TemplateDataEntity tempData = new HJD.HotelManagementCenter.Domain.TemplateDataEntity();
            //if (tempSource != null && tempSource.ID > 0)
            //{
            //    tempData.TemplateID = tempSource.ID;
            //    tempData.TemplateItem = tempSource.TemplateContent;
            //    tempData.BizType = 3;
            //    List<HJD.HotelManagementCenter.Domain.TemplateContent>  tList = JsonConvert.DeserializeObject<List<HJD.HotelManagementCenter.Domain.TemplateContent>>(tempSource.TemplateContent);
            //    if (tList.Count > 0 && tList != null)
            //    {
            //        tempData.ContentList = tList.OrderBy(_ => _.IDX).ToList();
            //    }
            //}
            //return tempData;

            return TemplateAdapter.GetTempSource(id);
        }
        /// <summary>
        /// 浏览记录 包括酒店和sku
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpGet]
        public BrowsingRecordResult GetBrowsingRecordList([FromUri]BsicSearchParam param)
        {
            return GenBrowsingRecordList(param);
        }
        public BrowsingRecordResult GenBrowsingRecordList(BsicSearchParam param)
        {
            DateTime arrivalTime = CommMethods.GetDefaultCheckIn();

            var result = new BrowsingRecordResult()
            {
                BlockTitle = "最近浏览记录",
                BorwsRecordList = new List<BrowsingRecordItem>(),
                TotalCount = 0
            };

            TimeLog log = new TimeLog(string.Format("GenBrowsingRecordList"), 1000, null); 

            int totalCount = 0;
            var list = HotelService.GetBrowsingRecordList(param, out totalCount);
            log.AddLog("GetBrowsingRecordList");

            if (totalCount > 0 && list.Any())
            {
                List<int> hotelIdList = list.Where(h => h.BusinessType == 2).Select(_ => Convert.ToInt32(_.BusinessID)).ToList();//酒店ID集合
                var hotelPicsList = HotelAdapter.GetManyHotelPhotos(hotelIdList);

                log.AddLog("GetManyHotelPhotos");
                //获取当前用户遵循的列表价类型
                var minPriceTypes = PriceAdapter.GetHotelMinPriceTypeByUser(param.userId);

                log.AddLog("GetHotelMinPriceTypeByUser");
                //获取HotelMinPrice列表价
                var hotelMinPriceList = PriceService.GetHotelMinPriceList(hotelIdList, arrivalTime) ?? new List<HotelMinPriceEntity>();

                log.AddLog("GetHotelMinPriceList");
                var pricePlanList = new List<PricePlanEx>();

                if (hotelMinPriceList == null || hotelMinPriceList.Count == 0)
                {
                    //获取PricePlan列表价
                    pricePlanList = PriceService.GetPricePlanExList(hotelIdList, arrivalTime) ?? new List<PricePlanEx>();
                    log.AddLog("GetPricePlanExList");
                }

                result.TotalCount = totalCount;
                result.BorwsRecordList = new List<BrowsingRecordItem>();
                foreach (var _ in list)
                {
                    if (_.BusinessType == 2)
                    {
                        string picUrl = "";
                        HotelItem hi = ResourceAdapter.GetHotel(Convert.ToInt32(_.BusinessID), 0);
                        if (hotelPicsList.FirstOrDefault(p => p.HotelId == Convert.ToInt32(_.BusinessID)) != null && hotelPicsList.First(p => p.HotelId == Convert.ToInt32(_.BusinessID)).HPList.Any())
                        {
                            picUrl = string.IsNullOrWhiteSpace(hotelPicsList.First(p => p.HotelId == Convert.ToInt32(_.BusinessID)).HPList[0].SURL) ? "http://whfront.b0.upaiyun.com/app/img/pic-def-16x9.png" : PhotoAdapter.GenHotelPicUrl(hotelPicsList.First(p => p.HotelId == Convert.ToInt32(_.BusinessID)).HPList[0].SURL, Enums.AppPhotoSize.theme);
                        }

                        var _item = new BrowsingRecordItem
                        {
                            BrowRecordName = hi.Name,
                            BrowRecordBizID = _.BusinessID,
                            BrowRecordBizType = _.BusinessType,
                            BrowRecordPicUrl = picUrl, //hotelPicsList.FirstOrDefault(p => p.HotelId == Convert.ToInt32(_.BusinessID)) != null && hotelPicsList.First(p => p.HotelId == Convert.ToInt32(_.BusinessID)).HPList.Any() ? PhotoAdapter.GenHotelPicUrl(hotelPicsList.First(p => p.HotelId == Convert.ToInt32(_.BusinessID)).HPList[0].SURL, Enums.AppPhotoSize.theme) : "",
                            BrowRecordNotVipPrice = 0,
                            BrowRecordVipPrice = 0,
                            BrowRecordBrief = "",
                            ID = _.ID
                        };

                        var _minPriceEntity = PriceAdapter.GetHotelPricePlan(Convert.ToInt32(_.BusinessID), pricePlanList, hotelMinPriceList, minPriceTypes, arrivalTime, false) ?? new HotelMinPriceEntity();
                        _item.BrowRecordNotVipPrice = _minPriceEntity.Price;
                        _item.BrowRecordVipPrice = _minPriceEntity.VipPrice;
                        _item.BrowRecordBrief = _minPriceEntity.Brief;
                        result.BorwsRecordList.Add(_item);
                    }
                    else if (_.BusinessType == 3)
                    {
                        try
                        {
                            var _item = new BrowsingRecordItem();
                            HJD.CouponService.Contracts.Entity.CouponActivityEntity couponActivity = couponService.GetCouponActivityBySKUID(Convert.ToInt32(_.BusinessID));
                            if (couponActivity.IsValid == true && couponActivity.State == 1)
                            {
                                if (!string.IsNullOrEmpty(couponActivity.PicPath))
                                {
                                    var picList = couponActivity.PicPath.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
                                    _item.BrowRecordPicUrl = PhotoAdapter.GenHotelPicUrl(picList[0], Enums.AppPhotoSize.theme);
                                }
                                SKUEntity sku = ProductAdapter.GetSKUEXEntityByID(Convert.ToInt32(_.BusinessID));
                                _item.BrowRecordBizID = _.BusinessID;
                                _item.BrowRecordBizType = _.BusinessType;
                                _item.BrowRecordBrief = couponActivity.Tags;
                                _item.BrowRecordName = couponActivity.PageTitle;
                                _item.BrowRecordNotVipPrice = sku.Price;
                                _item.BrowRecordVipPrice = sku.VIPPrice;
                                _item.ID = _.ID;
                                result.BorwsRecordList.Add(_item);
                            }
                        }
                        catch (Exception e)
                        {
                            Log.WriteLog("浏览记录报错 Error  BusinessID：" + _.BusinessID + "  BusinessType：" + _.BusinessType);
                        }
                    }

                }
                log.AddLog("Foreach");
                log.Finish();
            }

            return result;
        }
    }
}