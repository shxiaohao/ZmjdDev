using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using HJDAPI.Common.Helpers;
using HJDAPI.Models;
using HJD.DestServices.Contract;
using HJD.Framework.WCF;
using HJD.HotelServices.Contracts;
using HJD.HotelServices;
using HJD.Search.Contracts;
using HJD.Search.Entity;
using System.Web;
using System.Net;
using Newtonsoft.Json;
using System.Diagnostics;
using HJD.Framework.Interface;
using HJD.HotelPrice.Contract;
using HJDAPI.Controllers.Adapter;
using HJD.OtaCrawlerService.Contract.Ctrip.Hotel;
using HJD.OtaCrawlerServices.Contract;
using HJD.OtaCrawlerService.Contract.Proxy;
using HJDAPI.Controllers.Common;
using HJD.OtaCrawlerService.Contract;
using HJD.OtaCrawlerService.Contract.Params;
using System.Text.RegularExpressions;
using HJD.OtaCrawlerService.Contract.Hotel;
using HJD.CommentService.Contract;
using HJDAPI.Models.RequestParams;
using HJDAPI.Models.ResponseModel;
using HJDAPI.Common.Security;

namespace HJDAPI.Controllers
{
    public class HotelController : BaseApiController
    {
        public static ICacheProvider LocalCache = CacheManagerFactory.Create("DynamicCacheForType2");
        public static ICacheProvider LocalCache10Min = CacheManagerFactory.Create("DynamicCacheFor10Min");
        public static ICacheProvider LocalCache30Min = CacheManagerFactory.Create("DynamicCacheForADV");
        public static IHotelService HotelService = ServiceProxyFactory.Create<IHotelService>("BasicHttpBinding_IHotelService");
        public static IHotelPriceService PriceService = ServiceProxyFactory.Create<IHotelPriceService>("BasicHttpBinding_IHotelPriceService");
        public static IDestService destService = ServiceProxyFactory.Create<IDestService>("BasicHttpBinding_IDestService");
        public static ISearchTipAPI SearchTipApiService = ServiceProxyFactory.Create<ISearchTipAPI>("wsHttpBinding_ISearchTipAPI");
        public static IOtaCrawlerService otaCrawlerService = ServiceProxyFactory.Create<IOtaCrawlerService>("IOtaCrawlerService");

        private const int CtripID = 2; //携程OTA的ID
        private const int BookingID = 1; //Booking OTA的ID
        private const int needTagLength = 5; //列表中需要返回的tagNameList的长度
        private const int MaxHotelPrice = 1000000;//暂定的最大酒店价格

      

        [HttpGet]
        public List<CanSaleHotelInfoEntity> GetAllCanSellPackage(DateTime startDate, DateTime endDate, string tag = "")
        {
            return GenAllCanSellPackage(startDate, endDate, tag);
            //return LocalCache10Min.GetData<List<CanSaleHotelInfoEntity>>(string.Format("AllCanSellPackage:{0}:{1}:{2}", startDate.Date, endDate.Date, tag), () =>
            //{
            //    return GenAllCanSellPackage(startDate, endDate, tag);
            //});
        }

        private static List<CanSaleHotelInfoEntity> GenAllCanSellPackage(DateTime startDate, DateTime endDate, string tag = "")
        {
          //  Common.Log.WriteLog("GenAllCanSellPackage" + startDate.ToString());

            List<CanSaleHotelInfoEntity> hl = PackageAdapter.GetAllCanSellPackage(startDate, endDate, tag);
            List<int> didList = hl.Select(h => h.DistrictID).Distinct().ToList();
            List<HJD.DestServices.Contract.DistrictInfoEntity> dl = new List<HJD.DestServices.Contract.DistrictInfoEntity>();

            if (didList.Count > 0)
            {
                dl = destService.GetDistrictInfo(didList);
            }
            foreach (CanSaleHotelInfoEntity h in hl)
            {
                if (dl.Where(d => d.DistrictID == h.DistrictID).Count() == 0)
                {
                    h.DataPathName = h.DistrictID.ToString();
                }
                else
                {
                    h.DataPathName = dl.Where(d => d.DistrictID == h.DistrictID).First().DataPathName;
                }
            }
            return hl;
        }

        [HttpGet]
        public List<SpecialDealPackageEntity> GetSpecialDealPackage()
        {
            return PackageAdapter.GetSpecialDealPackage();
            //return LocalCache.GetData<List<SpecialDealPackageEntity>>(string.Format("SpecialDealPackage:{0}.{1}", DateTime.Now.Day, DateTime.Now.Hour), () =>
            //{
            //    return  PackageAdapter.GetSpecialDealPackage();
            //});
        }

        [HttpGet]
        public bool UpdateHotelInfo([FromUri]HotelInfoEditEntity hi)
        {
            return HotelService.UpdateHotelInfo(hi);
        }

        [HttpGet]
        public List<HJD.HotelServices.Contracts.CityEntity> GetZMJDCityData()
        {
            return HotelAdapter.GetZMJDCityData();
        }


        /// <summary>
        /// 添加出行人
        /// </summary>
        /// <param name="travelperson"></param>
        /// <returns></returns>
        [HttpPost]
        public GetTravelPersonResponseModel AddTravelPerson(GetTravelPersonParams param)
        {
            if (Signature.IsRightSignature(param.TimeStamp, param.SourceID, param.RequestType, param.Sign))
            {
                param.travelPerson.IDNumber = DES.Decrypt(param.travelPerson.IDNumber);
                param.travelPerson.TravelPersonName = DES.Decrypt(param.travelPerson.TravelPersonName);
                if (HotelAdapter.AddTravelPerson(param.travelPerson) > 0)
                {
                    return new GetTravelPersonResponseModel()
                    {
                        //CardTypeName=((HJDAPI.Common.Helpers.Enums.enumCardType)(param.travelPerson.IDType)).ToString(),
                        Success = true,
                        Message = "添加成功"
                    };
                }
                else
                {
                    return new GetTravelPersonResponseModel()
                    {
                        //CardTypeName="",
                        Success = false,
                        Message = "添加失败"
                    };
                }
            }
            else
            {
                return new GetTravelPersonResponseModel()
                {
                    //CardTypeName = "",
                    Success = false,
                    Message = "验证签名错误"
                };
            }
        }

        /// <summary>
        /// 添加出行人 不加密
        /// </summary>
        /// <param name="travelperson"></param>
        /// <returns></returns>
        [HttpPost]
        public GetTravelPersonResponseModel AddTravelPersonUnEncryption(TravelPersonEntity param)
        {
                if (HotelAdapter.AddTravelPerson(param) > 0)
                {
                    return new GetTravelPersonResponseModel()
                    {
                        Success = true,
                        Message = "添加成功"
                    };
                }
                else
                {
                    return new GetTravelPersonResponseModel()
                    {
                        //CardTypeName="",
                        Success = false,
                        Message = "添加失败"
                    };
                }
        }
        /// <summary>
        /// 更新出行人信息 不加密
        /// </summary>
        /// <param name="travelperson"></param>
        /// <returns></returns>
        [HttpPost]
        public GetTravelPersonResponseModel UpdateTravelPersonUnEncryption(TravelPersonEntity param)
        {
                TravelPersonEntity oldEntity = HotelAdapter.GetTravelPersonById(param.ID);
            if (HotelAdapter.UpdateTravelPerson(param) > 0)
            {
                CommAdapter.AddBizOpLog(new HJD.HotelManagementCenter.Domain.BizOpLogEntity
                {
                    BizID = param.ID,
                    BizIDStr = "",
                    BizType = HJD.HotelManagementCenter.Domain.OpLogBizType.TravelPerson,
                    OpType = (int)HJD.HotelManagementCenter.Domain.OpLogBizOpType.Update,
                    OpContent = string.Format("old:name:{0},type:{1} cardID:{2} birthDay:{3}  new:name:{4},type:{5} cardID:{6} birthDay:{7}", 
                    oldEntity.TravelPersonName, oldEntity.IDType, oldEntity.IDNumber, oldEntity.Birthday, param.TravelPersonName, param.IDType, param.IDNumber, param.Birthday),
                    OperatorUserID = param.UserID
                });

                return new GetTravelPersonResponseModel()
                {
                    //CardTypeName = ((HJDAPI.Common.Helpers.Enums.enumCardType)(param.travelPerson.IDType)).ToString(),
                    Success = true,
                    Message = "修改成功"
                };
            }
            return new GetTravelPersonResponseModel()
            {
                //CardTypeName = "",
                Success = false,
                Message = "修改失败"
            };
        }

        /// <summary>
        /// 根据id获取出行人
        /// </summary>
        /// <param name="travelperson"></param>
        /// <returns></returns>
        [HttpGet]
        public TravelPersonEntity GetTravelPersonById(int id)
        {
            TravelPersonEntity travelEntity = HotelAdapter.GetTravelPersonById(id);
            return travelEntity;
        }
        
        
        /// <summary>
        /// 更新出行人信息
        /// </summary>
        /// <param name="travelperson"></param>
        /// <returns></returns>
        [HttpPost]
        public GetTravelPersonResponseModel UpdateTravelPerson(GetTravelPersonParams param)
        {
            if (Signature.IsRightSignature(param.TimeStamp, param.SourceID, param.RequestType, param.Sign))
            {
                param.travelPerson.IDNumber = DES.Decrypt(param.travelPerson.IDNumber);
                param.travelPerson.TravelPersonName = DES.Decrypt(param.travelPerson.TravelPersonName);
                TravelPersonEntity oldEntity = HotelAdapter.GetTravelPersonById(param.travelPerson.ID);
                if (HotelAdapter.UpdateTravelPerson(param.travelPerson) > 0)
                {
                    CommAdapter.AddBizOpLog(new HJD.HotelManagementCenter.Domain.BizOpLogEntity
                    {
                        BizID = param.travelPerson.ID,
                        BizIDStr = "",
                        BizType = HJD.HotelManagementCenter.Domain.OpLogBizType.TravelPerson,
                        OpType = (int)HJD.HotelManagementCenter.Domain.OpLogBizOpType.Update,
                        OpContent = string.Format("old:name:{0},type:{1} cardID:{2} birthDay:{3}  new:name:{4},type:{5} cardID:{6} birthDay:{7}", oldEntity.TravelPersonName, oldEntity.IDType, oldEntity.IDNumber, oldEntity.Birthday, param.travelPerson.TravelPersonName, param.travelPerson.IDType, param.travelPerson.IDNumber, param.travelPerson.Birthday),
                        OperatorUserID = param.travelPerson.UserID
                    });

                    return new GetTravelPersonResponseModel()
                    {
                        //CardTypeName = ((HJDAPI.Common.Helpers.Enums.enumCardType)(param.travelPerson.IDType)).ToString(),
                        Success = true,
                        Message = "修改成功"
                    };
                }
                return new GetTravelPersonResponseModel()
                {
                    //CardTypeName = "",
                    Success = false,
                    Message = "修改失败"
                };
            }
            else
            {
                return new GetTravelPersonResponseModel()
                {
                    //CardTypeName = "",
                    Success = false,
                    Message = "验证签名错误"
                };
            }
        }

        /// <summary>
        /// 根据登陆人获取出行人列表
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpPost]
        public GetTravelPersonByUserIdResponseModel GetTravelPersonByUserId(GetTravelPersonByUserIdParams param)
        {
            if (Signature.IsRightSignature(param.TimeStamp, param.SourceID, param.RequestType, param.Sign))
            {

                GetTravelPersonByUserIdResponseModel m = new GetTravelPersonByUserIdResponseModel();
                m.Success = true;
                m.Message = "";

                m.TravelPersonList = HotelAdapter.GetTravelPersonByUserId(param.userID);
                m.CardTypeList = HotelAdapter.GetCardType();
                return m;
            }
            else
            {
                return new GetTravelPersonByUserIdResponseModel()
                {
                    Success = false,
                    Message = "验证签名错误"
                };
            }
        }

        /// <summary>
        /// 根据登陆人获取出行人列表  不加密
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpPost]
        public GetTravelPersonByUserIdResponseModel GetTravelPersonByUserIdUnEncryption(GetTravelPersonByUserIdParams param)
        {
            GetTravelPersonByUserIdResponseModel m = new GetTravelPersonByUserIdResponseModel();
            m.Success = true;
            m.Message = "";
            m.TravelPersonList = HotelAdapter.GetTravelPersonByUserIdUnEncryption(param.userID);
            m.CardTypeList = HotelAdapter.GetCardType();
            return m;
        }
        /// <summary>
        /// 获取证件类型
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]
        public List<CardEntity> GetCardTypeList()
        {
            return HotelAdapter.GetCardType();
        }

        static Dictionary<int, string> dicCard = new Dictionary<int, string>();

        /// <summary>
        /// 返回证件类型
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public Dictionary<int, string> GetCardType()
        {
            if (dicCard.Count == 0)
            {
                dicCard.Add(1, ((HJDAPI.Common.Helpers.Enums.enumCardType)(1)).ToString());
                dicCard.Add(2, ((HJDAPI.Common.Helpers.Enums.enumCardType)(2)).ToString());
                dicCard.Add(3, ((HJDAPI.Common.Helpers.Enums.enumCardType)(3)).ToString());
                dicCard.Add(4, ((HJDAPI.Common.Helpers.Enums.enumCardType)(4)).ToString());
                dicCard.Add(5, ((HJDAPI.Common.Helpers.Enums.enumCardType)(5)).ToString());
                dicCard.Add(10, ((HJDAPI.Common.Helpers.Enums.enumCardType)(10)).ToString());
            }
            return dicCard;
        }

        /// <summary>
        /// 删除用户state=2
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public GetTravelPersonResponseModel DeleteTravelPerson(DeleteTravePersonById param)
        {
            if (Signature.IsRightSignature(param.TimeStamp, param.SourceID, param.RequestType, param.Sign))
            {

                if (HotelAdapter.DeleteTravelPerson(param.ID))
                {
                    return new GetTravelPersonResponseModel()
                    {
                        Success = true,
                        Message = "删除成功"
                    };
                }
                else
                {
                    return new GetTravelPersonResponseModel()
                    {
                        Success = false,
                        Message = "删除失败"
                    };
                }
            }
            else
            {
                return new GetTravelPersonResponseModel()
                {
                    Success = false,
                    Message = "验证签名错误"
                };
            }
        }


        /// <summary>
        /// 获取周末酒店城市列表，包括热门区域
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public CityList GetZMJDCityData2()
        {
            return HotelAdapter.GetZMJDCityData2();
        }

        /// <summary>
        /// 获取周末酒店精选城市列表，包括热门区域
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public CityList GetZMJDSelectedCityData()
        {
            return HotelAdapter.GetZMJDSelectedCityData();
        }

        /// <summary>
        /// 获取周末酒店精选城市列表，包括热门区域
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public CityList GetZMJDAllCityData()
        {
            var cachedResult = HotelAdapter.GetZMJDAllCityData();
            var isAppVer4dot6 = !string.IsNullOrWhiteSpace(AppVer) && AppVer.CompareTo("4.6") >= 0 ? true : false;

            var result = new CityList()
            {
                HotArea = isAppVer4dot6 ? new List<HJD.HotelServices.Contracts.CityEntity>() : cachedResult.HotArea,
                HotOverseaArea = isAppVer4dot6 ? new List<HJD.HotelServices.Contracts.CityEntity>() : cachedResult.HotOverseaArea,
                SouthEastAsiaCitys = isAppVer4dot6 ? new List<HJD.HotelServices.Contracts.CityEntity>() : cachedResult.SouthEastAsiaCitys,
                HMTCitys = isAppVer4dot6 ? new List<HJD.HotelServices.Contracts.CityEntity>() : cachedResult.HMTCitys,
                Citys = cachedResult.Citys,
                HotAreaKeys = isAppVer4dot6 ? cachedResult.HotAreaKeys : new List<string>(),
                HotAreas = isAppVer4dot6 ? cachedResult.HotAreas : new Dictionary<string, List<HJD.HotelServices.Contracts.CityEntity>>(),
            };
            return result;
        }

        [HttpGet]
        public List<HJD.HotelServices.Contracts.CityEntity> GetZmjdCityList()
        {
            return HotelAdapter.GetZmjdCityList();
        }

        [HttpGet]
        public List<HJD.HotelServices.Contracts.CityEntity> GetZMJDLoveCityData()
        {
            return new HotelAdapter().GetZMJDLoveCityData();
        }

        /// <summary>
        /// 玩点酒店搜索
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public WapInterestHotelsResult SearchInterestHotel([FromUri] HotelListQueryParam p)
        {
            return new HotelAdapter().SearchInterestHotel(p);
        }

        /// <summary>
        /// 玩点酒店搜索
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public WapInterestHotelsResult SearchInterestHotel2([FromUri] HotelListQueryParam p)
        {
            return new HotelAdapter().SearchInterestHotel2(p);
        }

        /// <summary>
        /// 玩点酒店搜索
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public WapInterestHotelsResult2 SearchInterestHotel20([FromUri] HotelListQueryParam p)
        {
            return HotelAdapter.SearchInterestHotel30(p);
        }

        /// <summary>
        /// 玩点酒店搜索 JLTour 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public WapInterestHotelsResult2 SearchInterestHotel30([FromUri] HotelListQueryParam p)
        {
            return HotelAdapter.SearchInterestHotel30(p);
        }

        /// <summary>
        /// 酒店列表搜索（网站）
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public WapInterestHotelsResult3 SearchInterestHotel40([FromUri] HotelListQueryParam p)
        {
            return HotelAdapter.SearchInterestHotel40(p);
        }

        /// <summary>
        /// 玩点酒店搜索 JLTour
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public WapInterestHotelsResult3 SearchInterestHotel50([FromUri] HotelListQueryParam p)
        {
            return HotelAdapter.SearchInterestHotel50(p);
        }

        /// <summary>
        /// 根据酒店价格 入住日期范围 以及其他搜索数据得到酒店列表信息 （目前 APP 4.0 调用 2015-09-16 haoy）
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        [HttpPost]
        public SearchHotelListResult SearchHotelList(HotelListQueryParam20 p)
        {
            if (AppVer.CompareTo("4.1") >= 0)
            {
                p.hotelId = 0;//4.1版本之后 不再将精选页面某个主题排第一的酒店ID传到后台
            }
            p.IsNeedFilterCol = false;
            p.IsNeedHotelList = true;
            return HotelAdapter.SearchHotelList20(p);
        }

        /// <summary>
        /// 玩点酒店搜索 缩小照片尺寸
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public WapInterestHotelsResult SearchInterestHotel3([FromUri] HotelListQueryParam p)
        {
            return new HotelAdapter().SearchInterestHotel3(p);
        }

        /// <summary>
        /// 网站酒店列表接口
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        [HttpGet]
        public InterestHotelsResult QueryInterestHotel([FromUri] HotelListQueryParam p)
        {
            return HotelAdapter.QueryInterestHotel(p);
        }

        /// <summary>
        /// 3.0版本
        /// </summary>
        /// <param name="districtid"></param>
        /// <param name="geoScopeType"></param>
        /// <returns></returns>
        [HttpGet]
        public HomePageData GetHomePageData(int districtid = 2, int geoScopeType = 2)
        {
            return HotelAdapter.GetHomePageData(districtid, geoScopeType);
        }

        /// <summary>
        /// 4.0及之前版本使用
        /// </summary>
        /// <param name="userLat"></param>
        /// <param name="userLng"></param>
        /// <param name="geoScopeType"></param>
        /// <param name="districtid"></param>
        /// <param name="distance"></param>
        /// <param name="onlySelected"></param>
        /// <param name="districtName"></param>
        /// <returns></returns>
        [HttpGet]
        public InterestModel3 GetHomePageData20(float userLat, float userLng, int geoScopeType, int districtid = 2, int distance = 300000, bool onlySelected = false, string districtName = null)
        {
            try
            {
                int adDistrictID = districtid;
                //有地区名称找到地区ID
                if (adDistrictID == 0 && !string.IsNullOrWhiteSpace(districtName))
                {
                    districtName = HttpUtility.UrlDecode(districtName);//转一次码
                    var targetCity = GetZMJDCityData2().Citys.FirstOrDefault(_ => _.Name.Trim() == districtName.Trim());
                    adDistrictID = targetCity != null ? targetCity.ID : 0;
                }

                InterestModel3 im3 = new InterestModel3();
                im3.districtid = districtid;
                im3.Name = "";
                im3.GLat = userLat;
                im3.GLon = userLng;
                im3.TotalHotelNum = 0;

                //广告位置
                im3.AD = ADAdapter.GetOnlineAdv(adDistrictID, geoScopeType, userLat, userLng);
                im3.AD.Ratio = 0.547;
                if (im3.AD == null)
                {
                    im3.AD = new Advertise();
                }

                im3.InspectorList = new List<RecommendedInspectorModel>();

                InterestHotelResult interestHotelResult = ResourceAdapter.GetInterest4AD(districtid, userLat, userLng, geoScopeType, onlySelected);
                if (interestHotelResult.interests == null || interestHotelResult.interests.Count == 0)
                {
                    return im3;
                }

                List<InterestEntity> tempInterestDataCopyCache = interestHotelResult.interests.FindAll(_ => _.HotelCount > 0 && _.Type == 1).Select(_ => new InterestEntity()
                {
                    ADDescription = _.ADDescription,
                    DistrictID = _.DistrictID,
                    GLat = _.GLat,
                    GLon = _.GLon,
                    HotelCount = _.HotelCount,
                    HotelID = _.HotelID,
                    HotelList = _.HotelList,
                    HotelName = _.HotelName,
                    HotelPrice = _.HotelPrice,
                    ID = _.ID,
                    ImageUrl = _.ImageUrl,
                    InterestPlaceIDs = _.InterestPlaceIDs,
                    LogoBGColor = _.LogoBGColor,
                    LogoURL = _.LogoURL,
                    Name = _.Name,
                    Type = _.Type
                }).ToList();

                im3.ThemeInterestList = new List<InterestEntity>();

                List<int> chooseIIDs = RandomChooseInterestHotel(tempInterestDataCopyCache, false);

                if (chooseIIDs != null && chooseIIDs.Count != 0)
                {
                    List<InterestEntity> tempInterestList = tempInterestDataCopyCache.FindAll(_ => chooseIIDs.Contains(_.ID));
                    if (tempInterestList != null && tempInterestList.Count != 0)
                    {
                        //补充逻辑 如果只有一家酒店 对应多个主题 只显示一个主题 主题出现暂时按主题的sort升序
                        List<InterestEntity> interestList = tempInterestList.OrderByDescending(_ => _.HotelCount).ToList();
                        //以下调整 优先显示亲子,其余主题按酒店数量 从高到低排列
                        InterestEntity childInterest = interestList.FirstOrDefault(_ => _.ID == 12);//先找亲子主题 如果没有则算了

                        if (childInterest != null)
                        {
                            im3.ThemeInterestList.Add(childInterest);
                        }
                        foreach (var item in interestList)
                        {
                            if (item.ID != 12)
                            {
                                im3.ThemeInterestList.Add(item);
                            }
                        }

                        DateTime arrivalTime = CommMethods.GetDefaultCheckIn();
                        DateTime departureTime = arrivalTime.AddDays(1);

                        List<ListHotelItem3> hotelItemList = im3.ThemeInterestList.Select(_ => new ListHotelItem3()
                        {
                            Id = _.HotelID
                        }).ToList();
                        HotelAdapter.GenHotelListSlotPrice(hotelItemList, null, arrivalTime, departureTime, 0, 0);

                        im3.ThemeInterestList.ForEach((_) =>
                        {
                            _.ADDescription = string.IsNullOrWhiteSpace(_.ADDescription) ? "" : _.ADDescription;
                            _.HotelPrice = (int)hotelItemList.First(j => (int)j.Id == _.HotelID).MinPrice;
                        });
                    }
                }
                return im3;
            }
            catch (Exception ex)
            {
                Log.WriteLog("捕获异常：" + ex.Message + ex.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// 4.1版本 广告显示策略调整
        /// </summary>
        /// <param name="userLat"></param>
        /// <param name="userLng"></param>
        /// <param name="geoScopeType"></param>
        /// <param name="districtid"></param>
        /// <param name="distance"></param>
        /// <param name="onlySelected"></param>
        /// <param name="districtName"></param>
        /// <returns></returns>
        [HttpGet]
        public InterestModel3 GetHomePageData30(float userLat, float userLng, int geoScopeType, int districtid = 2, int distance = 300000, bool onlySelected = true, string districtName = null)
        {
            try
            {
                int adDistrictID = districtid;
                //有地区名称找到地区ID
                if (adDistrictID == 0 && !string.IsNullOrWhiteSpace(districtName))
                {
                    districtName = HttpUtility.UrlDecode(districtName);//转一次码
                    var targetCity = GetZMJDCityData2().Citys.FirstOrDefault(_ => _.Name.Trim() == districtName.Trim());
                    adDistrictID = targetCity != null ? targetCity.ID : 0;
                }

                InterestModel3 im3 = new InterestModel3();
                im3.districtid = districtid;
                im3.Name = "";
                im3.GLat = userLat;
                im3.GLon = userLng;

                Task<Advertise> advTask = Task.Factory.StartNew<Advertise>(() =>
                {
                    return ADAdapter.GenOnlineStrategy(adDistrictID, geoScopeType, userLat, userLng, string.IsNullOrWhiteSpace(districtName) ? "" : districtName);
                });

                Task<InterestHotelResult> interestHotelResultTask = Task.Factory.StartNew<InterestHotelResult>(() =>
                {
                    return ResourceAdapter.GetInterest4AD(districtid, userLat, userLng, geoScopeType, onlySelected);
                });

                Task<List<InterestEntity>> tempInterestDataCopyCacheTask = interestHotelResultTask.ContinueWith<List<InterestEntity>>((obj) =>
                {
                    return interestHotelResultTask.Result.interests.FindAll(_ => _.HotelCount > 0 && _.Type == 1).Select(_ => new InterestEntity()
                    {
                        ADDescription = _.ADDescription,
                        DistrictID = _.DistrictID,
                        GLat = _.GLat,
                        GLon = _.GLon,
                        HotelCount = _.HotelCount,
                        HotelID = _.HotelID,
                        HotelList = _.HotelList,
                        HotelName = _.HotelName,
                        HotelPrice = _.HotelPrice,
                        ID = _.ID,
                        ImageUrl = _.ImageUrl,
                        InterestPlaceIDs = _.InterestPlaceIDs,
                        LogoBGColor = _.LogoBGColor,
                        LogoURL = _.LogoURL,
                        Name = _.Name,
                        Type = _.Type
                    }).ToList();
                });

                Task.WaitAll();

                //广告位置
                im3.AD = advTask.Result;
                im3.AD.Ratio = 0.625;
                if (im3.AD == null)
                {
                    im3.AD = new Advertise();
                }

                im3.InspectorList = new List<RecommendedInspectorModel>();

                InterestHotelResult interestHotelResult = interestHotelResultTask.Result;
                if (interestHotelResult.interests == null || interestHotelResult.interests.Count == 0)
                {
                    return im3;
                }

                im3.TotalHotelNum = interestHotelResult.totalHotelCount;//酒店总数量
                im3.ThemeInterestList = new List<InterestEntity>();

                List<InterestEntity> tempInterestList = tempInterestDataCopyCacheTask.Result;
                if (tempInterestList != null && tempInterestList.Count != 0)
                {
                    //补充逻辑 如果只有一家酒店 对应多个主题 只显示一个主题 主题出现暂时按主题的sort升序
                    List<InterestEntity> interestList = tempInterestList.OrderByDescending(_ => _.HotelCount).ToList();
                    //以下调整 优先显示亲子,其余主题按酒店数量 从高到低排列
                    InterestEntity childInterest = interestList.FirstOrDefault(_ => _.ID == 12);//先找亲子主题 如果没有则算了

                    if (childInterest != null)
                    {
                        im3.ThemeInterestList.Add(childInterest);
                    }
                    foreach (var item in interestList)
                    {
                        if (item.ID != 12)
                        {
                            im3.ThemeInterestList.Add(item);
                        }
                    }
                }

                return im3;
            }
            catch (Exception ex)
            {
                Log.WriteLog("捕获异常：" + ex.Message + ex.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// 新版首页数据接口
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpGet]
        public HomeDataModel GetUserHomeData([FromUri]UserHomeDataParam param)
        {
            try
            {
                var taskArray = new List<Task>();

                Task<Advertise> advTask = Task.Factory.StartNew<Advertise>(() =>
                {
                    return ADAdapter.GenHomeOnlineAdv(AppUserID);
                });//广告任务

                int inspectorCount = 0;
                Task<List<RecommendedInspectorModel>> inspectorTask = Task.Factory.StartNew<List<RecommendedInspectorModel>>(() =>
                {
                    return AccountAdapter.GetAppHomeInspectorList(
                        new HJD.AccountServices.Entity.InspectorSearchParam()
                        {
                            Start = 0,
                            PageSize = 10,
                            Filter = new HJD.AccountServices.Entity.InspectorFilter()
                            {
                                State = new List<int>() { 2, 6 }
                            }
                        }, out inspectorCount, AppUserID);
                });//品鉴师任务

                Task<List<TopNPackageItem>> packageTask = Task.Factory.StartNew<List<TopNPackageItem>>(() =>
                {
                    return HotelAdapter.GetTop20Package(25);
                });//取推荐套餐任务

                taskArray.Add(advTask);
                taskArray.Add(inspectorTask);
                taskArray.Add(packageTask);

                Task.WaitAll(taskArray.ToArray());

                //广告位置 仅首页显示和全站显示
                HomeDataModel homeDataModel = new HomeDataModel();
                homeDataModel.AD = advTask.Result;//ADAdapter.GenHomeOnlineAdv();
                homeDataModel.AD.Ratio = 0.7;//0.547

                //推荐的关注品鉴师
                homeDataModel.InspectorResult = new RecommendInspectorResult()
                {
                    InspectorList = inspectorTask.Result,
                    InspectorBlockTitle = "品鉴师の选择",
                    InspectorTotalCount = inspectorCount
                };

                //推荐酒店套餐 暂时用top25系列酒店的数据
                List<TopNPackageItem> packageList = packageTask.Result;//HotelAdapter.GetTop20Package();//返回前N个推荐套餐
                homeDataModel.HotelResult = new RecommendHotelResult()
                {
                    HotelList = packageList.Select(_ => new RecommendHotelItem()
                    {
                        HotelID = _.HotelID,
                        HotelName = _.HotelName,
                        HotelPicUrl = _.PicUrls.First().Replace("_appdetail1", "_theme"),//_.PicUrls[0],
                        HotelPrice = _.PackagePrice.First(j => j.Type == 0).Price,
                        ADDescription = string.IsNullOrWhiteSpace(_.Title) ? "" : _.Title,
                        PID = _.PackageID
                    }).ToList(),
                    HotelTotalCount = packageList.Count,
                    HotelBlockTitle = "超值精选"
                };

                return homeDataModel;
            }
            catch (Exception ex)
            {
                Log.WriteLog("捕获异常：" + ex.Message + ex.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// 获取App首页广告
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public HomeDataModel20 GetUserHomeData20(long curUserID = 0)
        {
            long userId = curUserID == 0 ? AppUserID : curUserID;
            var adv = AppUserID > 0 ? new Advertise() : ADAdapter.GenHomeOnlineAdv(userId);

            var selectedResort = AppUserID > 0 ? new Advertise() : ADAdapter.GenSelectedResort();

            return new HomeDataModel20()
            {
                AD = adv,
                SelectedResort = selectedResort,
                loadH5Url = Configs.WWWURL + "/app/home?userid=" + (userId == 0 ? "{userid}" : userId.ToString()),
                BoxData = new PopupBoxData()
            };
        }

        /// <summary>
        /// 获取首页的普通广告Banners
        /// </summary>
        /// <param name="curUserID"></param>
        /// <returns></returns>
        [HttpGet]
        public HomeDataModel20 GetHomeOnlineBanners(long curUserID = 0)
        {
            long userId = curUserID == 0 ? AppUserID : curUserID;
            var adv = AppUserID > 0 ? new Advertise() : ADAdapter.GenHomeOnlineAdv(userId);

            //如果是非VIP，则追加一个购买VIPbanner
            var isvip = false;
            if (curUserID > 0)
            {
                try
                {
                    isvip = AccountAdapter.IsVIPCustomer(AccountAdapter.GetCustomerType(curUserID));
                }
                catch (Exception ex) { }
            }

            if (!isvip)
            {
                //adv.ADList.Add(new ADItem 
                //{
                //    ADURL = "http://whfront.b0.upaiyun.com/app/img/home/home-vip-banner.png?v=20161207.1",
                //    ActionURL = "http://www.zmjiudian.com/Coupon/VipShopInfo?_newpage=1",
                //    ADShowType = 1,
                //    ADTitle = "成为VIP"
                //});
            }
            
            return new HomeDataModel20()
            {
                AD = adv,
                SelectedResort = new Advertise(),
                loadH5Url = "",
                BoxData = new PopupBoxData()
            };
        }

        /// <summary>
        /// 获取广告列表Banners
        /// </summary>
        /// <param name="type">目前有：0普通广告，1度假广告，2首页头广告，3VIP专区广告，4首页导航，5首页头Banner</param>
        /// <param name="curUserID"></param>
        /// <returns></returns>
        [HttpGet]
        public HomeDataModel20 GetHomeOnlineBannersByType(int type = 0, long curUserID = 0)
        {
            long userId = curUserID == 0 ? AppUserID : curUserID;
            //var adv = AppUserID > 0 ? new Advertise() : ADAdapter.GenHomeOnlineAdvByType(_ContextBasicInfo, type, userId);

            var adv = ADAdapter.GenHomeOnlineAdvByType(_ContextBasicInfo, type, userId);

            ////如果是非VIP，则追加一个购买VIPbanner
            //var isvip = false;
            //if (curUserID > 0)
            //{
            //    try
            //    {
            //        isvip = AccountAdapter.IsVIPCustomer(AccountAdapter.GetCustomerType(curUserID));
            //    }
            //    catch (Exception ex) { }
            //}

            return new HomeDataModel20()
            {
                AD = adv,
                SelectedResort = new Advertise(),
                loadH5Url = "",
                BoxData = new PopupBoxData()
            };
        }

        [HttpGet]
        public HomeDataModel20 GetHomeOnlineBannersByTypeAndDistrictId(int type = 0, long curUserID = 0, int districtId = 0)
        {
            long userId = curUserID == 0 ? AppUserID : curUserID;
            var adv = ADAdapter.GetHomeOnlineBannersByTypeAndDistrictId(_ContextBasicInfo, type, userId, districtId);

            return new HomeDataModel20()
            {
                AD = adv,
                SelectedResort = new Advertise(),
                loadH5Url = "",
                BoxData = new PopupBoxData()
            };
        }

        /// <summary>
        /// 获取首页的弹窗信息
        /// </summary>
        /// <param name="curUserID"></param>
        /// <returns></returns>
        [HttpGet]
        public HomeDataModel20 GetHomeBoxData(long curUserID = 0)
        {
            var boxDataList = ADAdapter.GetPopBoxConfigEntityList((int)HJDAPI.Common.Helpers.Enums.PopBoxTarget.homepage);
            var boxData = boxDataList.FirstOrDefault();

            return new HomeDataModel20()
            {
                BoxData = new PopupBoxData()
                {
                    isShow = boxData != null ? boxData.isShow : false,
                    showUrl = boxData != null ? boxData.showUrl : "",
                    lazyLoadTime = boxData != null ? boxData.lazyLoadTime : 0.2f,
                    widthRatio = boxData != null ? boxData.widthRatio : 0.8f,
                    frequency = boxData != null ? boxData.frequency : 0,
                    boxId = boxData != null ? boxData.boxId : "",
                    widthHeightRatio = boxData != null ? boxData.widthHeightRatio : 0.6f
                }
            };
        }

        [HttpGet]
        public RecommendHotelResult GetRecommendHotelResult(long curUserID = 0)
        {
            //推荐酒店套餐 暂时用top25系列酒店的数据
            List<TopNPackageItem> packageList = HotelAdapter.GetTop20Package(25);

            var regex = new Regex("_.*$", RegexOptions.Compiled | RegexOptions.RightToLeft);
            var hotelList = packageList.Select(_ => new RecommendHotelItem()
            {
                HotelID = _.HotelID,
                HotelName = _.HotelName,
                HotelPicUrl = regex.Replace(_.PicUrls.First(), "_theme"),
                HotelPrice = _.PackagePrice.First(j => j.Type == 0).Price,
                ADDescription = string.IsNullOrWhiteSpace(_.Title) ? "" : _.Title,
                MarketPrice = _.MarketPrice,
                PackageBrief = _.PackageBrief,
                PID = _.PackageID,
                RecommendPicUrl = ""
            }).Where(_ => _.HotelPrice > 0).ToList();

            var result = new RecommendHotelResult()
            {
                HotelList = hotelList,
                HotelTotalCount = hotelList.Count,
                HotelBlockTitle = "超值精选"
            };
            return result;
        }

        /// <summary>
        /// 为VIP用户提供专享酒店
        /// </summary>
        /// <param name="albumId">专辑ID</param>
        /// <param name="start"></param>
        /// <param name="count"></param>
        /// <param name="curUserID"></param>
        /// <param name="area">1：国内，2港澳，3日本，4东南亚，5欧美澳</param>
        /// <param name="ckvip">是否需要验证VIP身份 1需要 0不需要</param>
        /// <returns></returns>
        [HttpGet]
        public RecommendHotelResult GetRecommendHotelResultByAlbumId(int albumId, int start, int count, long curUserID, int area = 0, int ckvip = 1, string dateStr = "", int gotoDistrictID = 0, int startDistrictID = 0)
        {
            var result = new RecommendHotelResult()
            {
                HotelList = new List<RecommendHotelItem>(),
                HotelTotalCount = 0,
                HotelBlockTitle = albumId == 1 ? "本周特惠套餐" : albumId == 10 ? "VIP专享" : ""
            };
            string districtIds = "";
            if (area == 1)
            {
                //districtIds = "15931";
                districtIds = "1,2,324,5169,5446,5501,7098,7926,10167,15400,15729,19429,19570,24981,31874,33013,35090,36423,47085,48245,48253,49687,53084,54586,56933,58681,59568,61253,72823,74554,77493";
            }
            else if (area == 2)//港澳
            {
                districtIds = "47720,35447";
            }
            else if (area == 3)//日本
            {
                districtIds = "27882";
            }
            else if (area == 4)//东南亚
            {
                districtIds = "10488,21648,28757,40141,43627,44459,50688,61398";
            }
            else if (area == 5)//欧美澳
            {
                districtIds = "2246,7242,10939,12302,13088,14057,14862,43007,47652,50637,56781,60021,63759,64606,70503,75387,76152,77670,54565,20126,54936";
            }

            TimeLog log = new TimeLog(string.Format("GetRecommendHotelResultByAlbumId，AlbumID：{0}，Area：{1}", albumId, area), 1000, null); 



            Enums.CustomerType userType = AccountAdapter.GetCustomerType(curUserID);
            bool isShowVipFirstBuypackage = true;
            bool isVip = true;
            int customerType = AccountAdapter.IsVIPCustomer(userType) ? 2 : 0;
            if (userType == Enums.CustomerType.vip199nr || userType == Enums.CustomerType.vip599)
            {
                isVip = true;
                isShowVipFirstBuypackage = AccountAdapter.HasUserPriviledge(curUserID, HJD.AccountServices.Entity.PrivilegeEnums.UserPrivilege.CanBuyVIPFirstPackage);
                //Log.WriteLog("userType1" + isShowVipFirstBuypackage);
            }
            else if (userType == Enums.CustomerType.general || curUserID == 0)
            {
                isVip = false;
                isShowVipFirstBuypackage = true;
            }
            else
            {
                isVip = true;
                isShowVipFirstBuypackage = false;
            }
            log.AddLog("GetCustomerType");

            //var isVip = AccountAdapter.IsVIPCustomer(curUserID);
            var _ok = ckvip == 1 ? (albumId != 10 || (curUserID > 0 && isVip)) : true;
            if (_ok)
            {
                List<TopNPackageItem> packageList = new List<TopNPackageItem>();
                if (area == 0)
                {
                    if (!string.IsNullOrWhiteSpace(dateStr))
                    {
                        try {
                            dateStr = Convert.ToDateTime(dateStr).ToString("yyyy-MM");
                        }
                        catch (Exception e)
                        {
                            dateStr = "";
                        }
                    }
                    //packageList = HotelAdapter.GetTop20Package(count, albumId, start, isShowVipFirstBuypackage, dateStr: dateStr, gotoDistrictID: gotoDistrictID, startDistrictID: startDistrictID);
                    packageList = HotelAdapter.GetTop20GroupPackage(count, albumId, start, isShowVipFirstBuypackage, dateStr: dateStr, gotoDistrictID: gotoDistrictID, startDistrictID: startDistrictID);
                    
                    log.AddLog("GetTop20Package");
                }
                else
                {
                    packageList = HotelAdapter.GetTop20PackageByDistrictIds(count, albumId, start, districtIds, isShowVipFirstBuypackage);
                    log.AddLog("GetTop20PackageByDistrictIds");
                }
                //List<TopNPackageItem> packageList = HotelAdapter.GetTop20Package(count, albumId, start, isShowVipFirstBuypackage);
                var regex = new Regex("_.*$", RegexOptions.Compiled | RegexOptions.RightToLeft);
                var hotelList = new List<RecommendHotelItem>();
                foreach (var _ in packageList)
                {
                    var _item = new RecommendHotelItem();
                    _item.HotelID = _.HotelID;
                    _item.HotelName = _.HotelName;
                    _item.PackageName = _.PackageName;

                    _item.HotelPicUrl = "";
                    if (_.PicUrls != null && _.PicUrls.Count > 0)
                    {
                        _item.HotelPicUrl = regex.Replace(_.PicUrls.First(), "_theme");
                    }

                    _item.HotelPrice = 0;
                    _item.VIPPrice = 0;
                    _item.TotalHotelPrice = 0;
                    _item.TotalVIPPrice = 0;

                    if (_.PackagePrice != null && _.PackagePrice.Count > 0)
                    {
                        var _singlePrice = 0;
                        var _singleVipPrice = 0;
                        var _totalPrice = 0;
                        var _totalPricePrice = 0;
                        var _nightCount = _.DayLimitMin;

                        //normal price
                        if (_.PackagePrice.Exists(j => j.Type == 0))
                        {
                            _singlePrice = _.PackagePrice.First(j => j.Type == 0).Price;
                        }

                        //vip price
                        if (_.PackagePrice.Exists(j => j.Type == -1))
                        {
                            _singleVipPrice = _.PackagePrice.First(j => j.Type == -1).Price;
                        }

                        //计算获取价格
                        if (_nightCount > 1)
                        {
                            //多天累加
                            PriceAdapter.GetManyDaysPackagePriceCached(_.HotelID, _.PackageID, userType, _nightCount, out _totalPrice, out _totalPricePrice);

                            if (_totalPrice <= 0) { _totalPrice = _singlePrice * _nightCount; }
                            if (_totalPricePrice <= 0) { _totalPricePrice = _singleVipPrice * _nightCount; }
                        }
                        else
                        {
                            _totalPrice = _singlePrice;
                            _totalPricePrice = _singleVipPrice;
                        }

                        _item.HotelPrice = _singlePrice;
                        _item.TotalHotelPrice = _totalPrice;

                        _item.VIPPrice = _singleVipPrice;
                        _item.TotalVIPPrice = _totalPricePrice;
                    }
                    log.AddLog("foreach (var _ in packageList) pid："+_.PackageID);

                    _item.DayLimitMin = _.DayLimitMin;
                    _item.ADDescription = string.IsNullOrWhiteSpace(_.Title) ? "" : _.Title;
                    _item.MarketPrice = _.MarketPrice;
                    _item.PackageBrief = _.PackageBrief;
                    _item.PID = _.PackageID;
                    _item.RecommendPicUrl = "";
                    _item.DistrictId = _.DistrictId;
                    _item.DistrictName = _.DistrictName;
                    _item.DistrictEName = _.DistrictEName;
                    _item.ProvinceName = _.ProvinceName;
                    _item.InChina = _.InChina;
                    _item.CustomerType = customerType;// (AccountAdapter.IsVIPCustomer(userType) ? 2 : 0);
                    _item.ForVIPFirstBuy = _.ForVIPFirstBuy;
                    _item.Title = string.IsNullOrWhiteSpace(_.Title) ? "" : _.Title;

                    hotelList.Add(_item);
                }

                ///yyb 2017 01 13 haoy 要取消掉排序 
                ////VIP专享页面按照省最多排序
                //if (albumId == 10)
                //{
                //    hotelList = hotelList.OrderByDescending(h => h.HotelPrice - h.VIPPrice).ToList();
                //}

                result.HotelList = hotelList;
                result.HotelTotalCount = hotelList.Count;
            }
            log.AddLog("foreach (var _ in packageList)");
            log.Finish();

            return result;
        }

        //public void RemoveGetRecommendHotelResultByAlbumIdCache(int albumsId)
        //{
        //    //int count = 25, int albumsId = 1, int start = 0, bool isShowVipFirstBuyPackage = true
        //    HotelAdapter.RemoveGetTop20PackageCache(albumsId: albumsId);
        //    HotelAdapter.RemoveGetTop20PackageByDistrictIds(albumsId: albumsId);
        //}

        [HttpGet]
        public ScreenConditionsEntity GetAlbumFilter(int albumId = 12)
        {
            ScreenConditionsEntity result = new ScreenConditionsEntity();
            result = HotelAdapter.GetAlbumFilter(albumId);
            return result;
        }


        #region 注释

        /// <summary>
        /// 为VIP用户提供专享酒店
        /// </summary>
        /// <param name="albumId">专辑ID</param>
        /// <param name="start"></param>
        /// <param name="count"></param>
        /// <param name="curUserID"></param>
        /// <param name="area">1：国内，2港澳，3日本，4东南亚，5欧美澳</param>
        /// <param name="ckvip">是否需要验证VIP身份 1需要 0不需要</param>
        /// <returns></returns>
        //[HttpGet]
        //public RecommendHotelResult GetRecommendHotelResultByAlbumIdAndDistrict(int albumId, int start, int count, long curUserID, int area = 0, int ckvip = 1)
        //{
        //    var result = new RecommendHotelResult()
        //    {
        //        HotelList = new List<RecommendHotelItem>(),
        //        HotelTotalCount = 0,
        //        HotelBlockTitle = albumId == 1 ? "本周特惠套餐" : albumId == 10 ? "VIP专享" : ""
        //    };

        //    #region 国外地域初始

        //    var unchinaDic = new Dictionary<string, Dictionary<string, string>>();

        //    #region 注释

        //    ////all
        //    //unchinaDic["inlall"] = new Dictionary<string, string> 
        //    //{
        //    //    {"47720", "香港"},{"35447", "澳门"},{"24138", "台湾"},{"27882", "日本"},{"5831", "韩国"}
        //    //    ,{"21648", "新加坡"},{"28757", "泰国"},{"44459", "马来西亚"},{"75387", "英国"}
        //    //    ,{"14862", "法国"},{"54565", "美国"},{"20126", "加拿大"},{"54936", "澳大利亚"}
        //    //};

        //    ////日韩
        //    //unchinaDic["日韩"] = new Dictionary<string, string> 
        //    //{
        //    //    {"27882", "日本"},{"5831", "韩国"}
        //    //};
        //    //日韩

        //    ////欧洲
        //    //unchinaDic["欧洲"] = new Dictionary<string, string> 
        //    //{
        //    //    {"2246", "奥地利"},{"7242", "荷兰"},{"10939", "意大利"},{"12302", "挪威"},{"13088", "冰岛"},
        //    //    {"14057", "瑞士"},{"14862", "法国"},{"43007", "丹麦"},{"47652", "西班牙"},{"50637", "芬兰"},
        //    //    {"56781", "希腊"},{"60021", "罗马尼亚"},{"63759", "德国"},{"64606", "葡萄牙"},{"70503", "波兰"},
        //    //    {"75387", "英国"},{"76152", "俄罗斯"},{"77670", "瑞典"}
        //    //};

        //    ////北美
        //    //unchinaDic["北美"] = new Dictionary<string, string> 
        //    //{
        //    //    {"54565", "美国"},{"20126", "加拿大"}
        //    //};

        //    ////澳新
        //    //unchinaDic["澳新"] = new Dictionary<string, string> 
        //    //{
        //    //    {"54936", "澳大利亚"},{"21648", "新加坡"}
        //    //};
        //    ////台湾
        //    //unchinaDic["台湾"] = new Dictionary<string, string> 
        //    //{
        //    //    {"24138", "台湾"}
        //    //}; 
        //    #endregion


        //    string districtIds = "";
        //    if (area == 1)
        //    {
        //        districtIds = "15931";
        //    }
        //    else if (area == 2)//港澳
        //    {
        //         districtIds = "47720,35447";
        //    }
        //    else if (area == 3)//日本
        //    {
        //        districtIds = "27882";
        //    }
        //    else if (area == 4)//东南亚
        //    {
        //       districtIds = "10488,21648,28757,40141,43627,44459,50688,61398";
        //    }
        //    else if (area == 5)//欧美澳
        //    {
        //        districtIds = "2246,7242,10939,12302,13088,14057,14862,43007,47652,50637,56781,60021,63759,64606,70503,75387,76152,77670,54565,20126,54936";
        //    }

        //    #endregion

        //    Enums.CustomerType userType = AccountAdapter.GetCustomerType(curUserID);
        //    bool isShowVipFirstBuypackage = true;
        //    bool isVip = true;
        //    if (userType == Enums.CustomerType.vip199nr || userType == Enums.CustomerType.vip599)
        //    {
        //        isVip = true;
        //        isShowVipFirstBuypackage = AccountAdapter.HasUserPriviledge(curUserID, HJD.AccountServices.Entity.PrivilegeEnums.UserPrivilege.CanBuyVIPFirstPackage);
        //    }
        //    else if (userType == Enums.CustomerType.general || curUserID == 0)
        //    {
        //        isVip = false;
        //        isShowVipFirstBuypackage = true;
        //    }
        //    else
        //    {
        //        isVip = true;
        //        isShowVipFirstBuypackage = false;
        //    }

        //    //var isVip = AccountAdapter.IsVIPCustomer(curUserID);
        //    var _ok = ckvip == 1 ? (albumId != 10 || (curUserID > 0 && isVip)) : true;
        //    if (_ok)
        //    {
        //        List<TopNPackageItem> packageList = new List<TopNPackageItem>();
        //        if (area == 0)
        //        {
        //            packageList = HotelAdapter.GetTop20Package(count, albumId, start, isShowVipFirstBuypackage);
        //        }
        //        else
        //        {
        //            packageList = HotelAdapter.GetTop20PackageByDistrictIds(count, albumId, start, districtIds, isShowVipFirstBuypackage);
        //        }
        //        //List<TopNPackageItem> packageList = HotelAdapter.GetTop20PackageByDistrictIds(count, albumId, start, districtIds, isShowVipFirstBuypackage);
        //        var regex = new Regex("_.*$", RegexOptions.Compiled | RegexOptions.RightToLeft);
        //        var hotelList = new List<RecommendHotelItem>();
        //        foreach (var _ in packageList)
        //        {
        //            var _item = new RecommendHotelItem();
        //            _item.HotelID = _.HotelID;
        //            _item.HotelName = _.HotelName;
        //            _item.PackageName = _.PackageName;

        //            _item.HotelPicUrl = "";
        //            if (_.PicUrls != null && _.PicUrls.Count > 0)
        //            {
        //                _item.HotelPicUrl = regex.Replace(_.PicUrls.First(), "_theme");
        //            }

        //            _item.HotelPrice = 0;
        //            _item.VIPPrice = 0;
        //            _item.TotalHotelPrice = 0;
        //            _item.TotalVIPPrice = 0;

        //            if (_.PackagePrice != null && _.PackagePrice.Count > 0)
        //            {
        //                if (_.PackagePrice.Exists(j => j.Type == 0))
        //                {
        //                    var _singlePrice = _.PackagePrice.First(j => j.Type == 0).Price;

        //                    _item.HotelPrice = _singlePrice;
        //                    _item.TotalHotelPrice = (_.DayLimitMin > 1 ? PriceAdapter.GetManyDaysPackagePrice(_.HotelID, _.PackageID, _singlePrice, _.DayLimitMin, (int)curUserID, false) : _singlePrice * _.DayLimitMin);
        //                }

        //                if (_.PackagePrice.Exists(j => j.Type == -1))
        //                {
        //                    var _singlePrice = _.PackagePrice.First(j => j.Type == -1).Price;

        //                    _item.VIPPrice = _singlePrice;
        //                    _item.TotalVIPPrice = (_.DayLimitMin > 1 ? PriceAdapter.GetManyDaysPackagePrice(_.HotelID, _.PackageID, _singlePrice, _.DayLimitMin, (int)curUserID, true) : _singlePrice * _.DayLimitMin);
        //                }
        //            }

        //            _item.DayLimitMin = _.DayLimitMin;
        //            _item.ADDescription = string.IsNullOrWhiteSpace(_.Title) ? "" : _.Title;
        //            _item.MarketPrice = _.MarketPrice;
        //            _item.PackageBrief = _.PackageBrief;
        //            _item.PID = _.PackageID;
        //            _item.RecommendPicUrl = "";
        //            _item.DistrictId = _.DistrictId;
        //            _item.DistrictName = _.DistrictName;
        //            _item.DistrictEName = _.DistrictEName;
        //            _item.ProvinceName = _.ProvinceName;
        //            _item.InChina = _.InChina;
        //            _item.CustomerType = (AccountAdapter.IsVIPCustomer(userType) ? 2 : 0);
        //            _item.ForVIPFirstBuy = _.ForVIPFirstBuy;

        //            hotelList.Add(_item);
        //        }


        //        result.HotelList = hotelList;
        //        result.HotelTotalCount = hotelList.Count;
        //    }

        //    return result;
        //}
        
        #endregion

        /// <summary>
        /// 为VIP用户提供专享酒店 加搜索功能
        /// </summary>
        /// <param name="albumId"></param>
        /// <param name="start"></param>
        /// <param name="count"></param>
        /// <param name="curUserID"></param>
        /// <param name="lat"></param>
        /// <param name="lng"></param>
        /// <param name="geoScopeType"> 等于3 查询300km之内</param>
        /// <param name="districtID">城市id</param>
        /// <param name="ckvip"></param>
        /// <returns></returns>
        [HttpGet]
        public RecommendHotelResult GetRecommendHotelResultByAlbumIdAddSearch(int albumId, int start, int count, long curUserID, float lat = 0, float lng = 0, int geoScopeType = 0, int districtID = 0, int ckvip = 1)
        {

            DestController dest = new DestController();
            SimpleCityInfo CityInfo = dest.GetCityInfoByName("", lat, lng);
            int districtid = CityInfo.DistrictId;
            //int geo = geoScopeType;
            if(districtID!=0)
            {
                districtid = districtID;
                var City = dest.GetDistrictInfo(districtID.ToString()).FirstOrDefault();
                lat = City.lat;
                lng = City.lon;
            }

            var result = new RecommendHotelResult()
            {
                HotelList = new List<RecommendHotelItem>(),
                HotelTotalCount = 0,
                HotelBlockTitle = albumId == 1 ? "本周特惠套餐" : albumId == 10 ? "VIP专享" : ""
            };

            Enums.CustomerType userType = AccountAdapter.GetCustomerType(curUserID);
            bool isShowVipFirstBuypackage = true;
            bool isVip = true;
            if (userType == Enums.CustomerType.vip199nr || userType == Enums.CustomerType.vip599)
            {
                isVip = true;
                isShowVipFirstBuypackage = AccountAdapter.HasUserPriviledge(curUserID, HJD.AccountServices.Entity.PrivilegeEnums.UserPrivilege.CanBuyVIPFirstPackage);
                //isShowVipFirstBuypackage = bool.Parse(LocalCache30Min.GetData<string>(string.Format("SearchHotelListForWellPage:{0}", curUserID), () =>
                //{
                //    return AccountAdapter.HasUserPriviledge(curUserID, HJD.AccountServices.Entity.PrivilegeEnums.UserPrivilege.CanBuyVIPFirstPackage).ToString();
                //}));
            }
            else if (userType == Enums.CustomerType.general || curUserID == 0)
            {
                isVip = false;
                isShowVipFirstBuypackage = true;
            }
            else
            {
                isVip = true;
                isShowVipFirstBuypackage = false;
            }


            //var isVip = AccountAdapter.IsVIPCustomer(curUserID);
            var _ok = ckvip == 1 ? (albumId != 10 || (curUserID > 0 && isVip)) : true;
            if (_ok)
            {
                List<TopNPackageItem> packageList = HotelAdapter.GetTop20PackageAddSearch(count, albumId, start, lat, lng, geoScopeType, districtID, isShowVipFirstBuypackage);
                var regex = new Regex("_.*$", RegexOptions.Compiled | RegexOptions.RightToLeft);
                var hotelList = new List<RecommendHotelItem>();
                foreach (var _ in packageList)
                {
                    var _item = new RecommendHotelItem();
                    _item.HotelID = _.HotelID;
                    _item.HotelName = _.HotelName;
                    _item.PackageName = _.PackageName;

                    _item.HotelPicUrl = "";
                    if (_.PicUrls != null && _.PicUrls.Count > 0)
                    {
                        _item.HotelPicUrl = regex.Replace(_.PicUrls.First(), "_theme");
                    }

                    _item.HotelPrice = 0;
                    _item.VIPPrice = 0;
                    _item.TotalHotelPrice = 0;
                    _item.TotalVIPPrice = 0;

                    if (_.PackagePrice != null && _.PackagePrice.Count > 0)
                    {
                        var _singlePrice = 0;
                        var _singleVipPrice = 0;
                        var _totalPrice = 0;
                        var _totalPricePrice = 0;
                        var _nightCount = _.DayLimitMin;

                        //normal price
                        if (_.PackagePrice.Exists(j => j.Type == 0))
                        {
                            _singlePrice = _.PackagePrice.First(j => j.Type == 0).Price;
                        }

                        //vip price
                        if (_.PackagePrice.Exists(j => j.Type == -1))
                        {
                            _singleVipPrice = _.PackagePrice.First(j => j.Type == -1).Price;
                        }

                        //计算获取价格
                        if (_nightCount > 1)
                        {
                            //多天累加
                            PriceAdapter.GetManyDaysPackagePriceCached(_.HotelID, _.PackageID, userType, _nightCount, out _totalPrice, out _totalPricePrice);

                            if (_totalPrice <= 0) { _totalPrice = _singlePrice * _nightCount; }
                            if (_totalPricePrice <= 0) { _totalPricePrice = _singleVipPrice * _nightCount; }
                        }
                        else
                        {
                            _totalPrice = _singlePrice;
                            _totalPricePrice = _singleVipPrice;
                        }

                        _item.HotelPrice = _singlePrice;
                        _item.TotalHotelPrice = _totalPrice;

                        _item.VIPPrice = _singleVipPrice;
                        _item.TotalVIPPrice = _totalPricePrice;
                    }

                    _item.DayLimitMin = _.DayLimitMin;
                    _item.ADDescription = string.IsNullOrWhiteSpace(_.Title) ? "" : _.Title;
                    _item.MarketPrice = _.MarketPrice;
                    _item.PackageBrief = _.PackageBrief;
                    _item.PID = _.PackageID;
                    _item.RecommendPicUrl = "";
                    _item.DistrictId = _.DistrictId;
                    _item.DistrictName = _.DistrictName;
                    _item.DistrictEName = _.DistrictEName;
                    _item.ProvinceName = _.ProvinceName;
                    _item.InChina = _.InChina;
                    _item.CustomerType = (AccountAdapter.IsVIPCustomer(userType) ? 2 : 0);
                    _item.ForVIPFirstBuy = _.ForVIPFirstBuy;

                    hotelList.Add(_item);
                }
                //yyb 2017 01 13 haoy 要取消掉排序 
                ////VIP专享页面按照省最多排序
                //if (albumId == 10)
                //{
                //    hotelList = hotelList.OrderByDescending(h => h.HotelPrice - h.VIPPrice).ToList();
                //}

                result.HotelList = hotelList;
                result.HotelTotalCount = hotelList.Count;
            }

            return result;
        }


        public HotelDestInfoList GetDistrictInfoForAlbum(int albumsID, float lat, float lng)
        {
            //坐标为0 默认上海
            if (lat == 0 || lng==0)
            {
                lat = 31.230393F;
                lng = 121.473704F;
            }

            List<dicHotelDestInfoList> hotelList = new List<dicHotelDestInfoList>();
            List<HotelDestnInfo> dei = HotelAdapter.GetHotelDestnInfo(albumsID);
            if (dei != null & dei.Count > 0)
            {
                dicHotelDestInfoList destnInChina = new dicHotelDestInfoList();
                destnInChina.Description = "国内";
                destnInChina.HotelDestInfoList = dei.Where(_ => _.InChina == true).ToList();
                hotelList.Add(destnInChina);
                dicHotelDestInfoList destnNoInChina = new dicHotelDestInfoList();
                destnNoInChina.Description = "海外";
                destnNoInChina.HotelDestInfoList = dei.Where(_ => _.InChina == false).ToList();
                hotelList.Add(destnNoInChina);
            }
            DestController dest = new DestController();
            SimpleCityInfo CityInfo = dest.GetCityInfoByName("", lat, lng);

            List<HotelDestnInfo> deiWithIn = HotelAdapter.GetHotelDestnInfoWithIn(albumsID, lat, lng);
            if (deiWithIn != null && dei.Count > 0)
            {
                HotelDestnInfo destInfo = deiWithIn.Where(_ => _.DistrictID == CityInfo.DistrictId).FirstOrDefault();
                deiWithIn.Remove(destInfo);
                deiWithIn.Insert(0, destInfo);
                dicHotelDestInfoList destnWithIn = new dicHotelDestInfoList();
                destnWithIn.HotelDestInfoList = deiWithIn;
                destnWithIn.Description = "周边城市";
                hotelList.Add(destnWithIn);
            }

            HotelDestInfoList hotelDestList = new HotelDestInfoList();
            hotelDestList.dicHotelDestInfoList = hotelList;

            return hotelDestList;
        }

        /// <summary>
        /// 酒店搜索
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public WapThemeHotelsResult SearchThemeHotel([FromUri] HotelListQueryParam p)
        {
            return new HotelAdapter().SearchThemeHotel(p);
        }

        /// <summary>
        /// 酒店搜索
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public WapHotelsResult Search([FromUri] HotelListQueryParam p)
        {
            return new HotelAdapter().Search(p);
        }

        /// <summary>
        /// 根据目的地ID 返回取那家OTA的价格数据
        /// </summary>
        /// <param name="districtid"></param>
        /// <returns></returns>
        private int GetOTAID(int districtid)
        {
            return 0;
            return (int)LocalCache.GetData<object>("OTAIDDistrictID" + districtid.ToString(), () =>
            {
                HJD.DestServices.Contract.DistrictInfoEntity di = destService.GetDistrictInfo(new List<int> { districtid }).FirstOrDefault();

                if (di.InChina)
                {
                    return CtripID;
                }
                else
                {
                    return BookingID;
                }
            });

        }

        private HotelItem GenNewHotelItem(string Name)
        {
            HotelItem hi = new HotelItem();
            hi.Address = "Address";
            hi.ReviewCount = 3;
            hi.Currency = "RMB";
            hi.Description = "Description";
            hi.DistrictCount = 1;
            hi.DistrictId = 2;
            hi.DistrictName = "DistrictName";
            hi.EName = "EName";
            // hi.Facilities = new string[] { "f1", "f2" };
            hi.GLat = 123.231;
            hi.GLat = 30.23;
            hi.Id = 5;
            hi.InChina = true;
            hi.MaxPrice = 1000;
            hi.MinPrice = 100;
            hi.Name = Name;
            hi.OfficalPictureCount = 30;
            hi.Picture = "Picture";
            hi.PictureCount = 5;
            hi.Rank = 4;
            hi.Score = 5;
            hi.Star = 3;
            hi.Telephone = "telephone";
            hi.Types = new string[] { "t1", "t2" };

            return hi;
        }

        /// <summary>
        /// 获取酒店详情 为APP返回数据
        /// </summary>
        /// <param name="id">酒店id</param>
        /// <returns></returns>
        [HttpGet]
        public HJDAPI.Controllers.HotelAdapter.HotelItemResult Get(int id, string checkIn, string checkOut)
        {
            return Get(id, checkIn, checkOut, "app");
        }

        /// <summary>
        /// 获取酒店详情
        /// </summary>
        /// <param name="id">酒店id</param>
        /// <param name="sType">访问源类型：app, m=mobile</param>
        /// <returns></returns>
        [HttpGet]
        public HJDAPI.Controllers.HotelAdapter.HotelItem2Result Get2(int id, string checkIn, string checkOut, string sType, int themeid)
        {

            return new HotelAdapter().Get2(id, checkIn, checkOut, sType, themeid);

        }

        [HttpGet]
        public HotelItem3 Get3(int id, string checkIn, string checkOut, string sType, int interestid)
        {
            return new HotelAdapter().Get30(id, checkIn, checkOut, sType, interestid);
        }

        [HttpGet]
        public HotelItem3 Get30(int id, string checkIn, string checkOut, string sType, int interestid)
        {
            return new HotelAdapter().Get30(id, checkIn, checkOut, sType, interestid);
        }

        [HttpGet]
        public HotelItem4 Get40(int id, string checkIn, string checkOut, string sType, int interestid)
        {
            return new HotelAdapter().Get40(id, checkIn, checkOut, sType, interestid);
        }

        [HttpGet]
        public HotelItem5 Get50(int id, string checkIn, string checkOut, string sType, int interestid, long userID = 0, string machineNo = "", int pid = 0)
        {
            long appUserID = userID > 0 ? userID : AppUserID;
            try
            {
                if (appUserID != 0)
                {
                    var appType = AppType.ToLower();
                    int terminalType = appType.Contains("web") ? 1 : appType.Contains("ios") ? 2 : appType.Contains("android") ? 3 : 4;
                    string clientIP = terminalType == 2 || terminalType == 3 ? HttpRequestHelper.GetClientIp(this.ControllerContext.Request) : "";

                    HotelAdapter.InsertBrowsingRecord(new BrowsingRecordEntity()
                    {
                        BusinessID = id,
                        BusinessType = 2,
                        Visitor = appUserID,
                        TerminalType = terminalType,
                        ClientIP = clientIP,
                        SessionID = "",
                        OpenID = "",
                        AppVer = AppVer
                    });
                }
            }
            catch (Exception ex)
            {

            }

            return new HotelAdapter().Get50(id, checkIn, checkOut, sType, interestid, appUserID, AppVer, pid);
        }


        /// <summary>
        /// 获取酒店详情，app5.0开始使用
        /// </summary>
        /// <param name="id"></param>
        /// <param name="checkIn"></param>
        /// <param name="checkOut"></param>
        /// <param name="sType"></param>
        /// <param name="interestid"></param>
        /// <param name="userID"></param>
        /// <param name="machineNo"></param>
        /// <param name="pid"></param>
        /// <returns></returns>
        [HttpGet]
        public HotelItem6 GetHotelDetail(int id, string checkIn, string checkOut, string sType, int interestid, long userID = 0, string machineNo = "", int pid = 0, int commentid = 0)
        {
            if (userID > 0)
            {
                _ContextBasicInfo.AppUserID = userID;
            }

            try
            {
                if (_ContextBasicInfo.AppUserID > 0)
                {
                    int terminalType = _ContextBasicInfo.IsWeb ? 1 : _ContextBasicInfo.IsIOS ? 2 : _ContextBasicInfo.IsAndroid ? 3 : 4;
                    string clientIP = terminalType == 2 || terminalType == 3 ? HttpRequestHelper.GetClientIp(this.ControllerContext.Request) : "";

                    HotelAdapter.InsertBrowsingRecord(new BrowsingRecordEntity()
                    {
                        BusinessID = id,
                        BusinessType = 2,
                        Visitor = _ContextBasicInfo.AppUserID,
                        TerminalType = terminalType,
                        ClientIP = clientIP,
                        SessionID = "",
                        OpenID = "",
                        AppVer = _ContextBasicInfo.AppVer
                    });


                }
            }
            catch (Exception ex) { }

            var hotelEntity = new HotelAdapter().GetHotelDetailAdapter(_ContextBasicInfo, id, checkIn, checkOut, sType, interestid, pid, commentid, userID);
            //get
            //var hotelEntity = new HotelAdapter().Get60(_ContextBasicInfo, id, checkIn, checkOut, sType, interestid, pid, commentid);

            #region 行为记录

            try
            {
                var value = string.Format("{{\"hotelid\":\"{0}\",\"checkIn\":\"{1}\",\"checkOut\":\"{2}\",\"hotelName\":\"{3}\",\"sType\":\"{4}\",\"interestid\":\"{5}\",\"pid\":\"{6}\",\"commentid\":\"{7}\",\"machineNo\":\"{8}\"}}",
                    id, checkIn, checkOut, (hotelEntity != null ? hotelEntity.HotelName : ""), sType, interestid, pid, commentid, machineNo);
                RecordBehavior("AppHotelLoad", value);
            }
            catch (Exception ex) { }

            #endregion

            return hotelEntity;
        }
        
        /// <summary>
        /// Magicall获取酒店详情页发送图文信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="checkIn"></param>
        /// <param name="checkOut"></param>
        /// <param name="sType"></param>
        /// <param name="interestid"></param>
        /// <param name="userID"></param>
        /// <param name="machineNo"></param>
        /// <param name="pid"></param>
        /// <returns></returns>
        [HttpGet]
        public HotelItem6 GetSimpleHotelDetail(int id, string checkIn, string checkOut, string sType, int interestid, long userID = 0, string machineNo = "", int pid = 0, int commentid = 0)
        {
            long appUserID = userID > 0 ? userID : AppUserID;
            try
            {
                if (appUserID != 0)
                {
                    var appType = AppType.ToLower();
                    int terminalType = appType.Contains("web") ? 1 : appType.Contains("ios") ? 2 : appType.Contains("android") ? 3 : 4;
                    string clientIP = terminalType == 2 || terminalType == 3 ? HttpRequestHelper.GetClientIp(this.ControllerContext.Request) : "";

                    HotelAdapter.InsertBrowsingRecord(new BrowsingRecordEntity()
                    {
                        BusinessID = id,
                        BusinessType = 2,
                        Visitor = appUserID,
                        TerminalType = terminalType,
                        ClientIP = clientIP,
                        SessionID = "",
                        OpenID = "",
                        AppVer = AppVer
                    });
                }
            }
            catch (Exception ex)
            {

            }

            return new HotelAdapter().GetSimple60(id, checkIn, checkOut, sType, interestid, appUserID, AppVer, pid, commentid);
        }

        [HttpGet]
        public HotelItem5 Get50Test(int id, string checkIn, string checkOut, string sType, int interestid, long userID = 0, string machineNo = "")
        {
            return new HotelAdapter().Get50Test(id, checkIn, checkOut, sType, interestid);
        }

        [HttpGet]
        public bool ClareHotelInfoCache(int hotelid)
        {
            return new HotelAdapter().ClareHotelInfoCache(hotelid);
        }

        /// <summary>
        /// 获取酒店详情
        /// </summary>
        /// <param name="id">酒店id</param>
        /// <param name="sType">访问源类型：app, m=mobile</param>
        /// <returns></returns>
        [HttpGet]
        public HJDAPI.Controllers.HotelAdapter.HotelItemResult Get(int id, string checkIn, string checkOut, string sType)
        {
            return new HotelAdapter().Get(id, checkIn, checkOut, sType);
        }

        [HttpGet]
        public CrawlerHotel GetCtripHotel(string hotelId, string checkIn, string checkOut)
        {
            HotelRoomParams hotelRoomParams = new HotelRoomParams
            {
                HotelID = hotelId,
                CheckIn = checkIn,
                CheckOut = checkOut,
                OtaType = OtaType.Ctrip
            };

            return otaCrawlerService.GetCanSellHotel(hotelRoomParams);
        }

        [HttpGet]
        public string GetCtripHotelHtml(string hotelId, string checkIn, string checkOut)
        {
            HotelRoomParams hotelRoomParams = new HotelRoomParams
            {
                HotelID = hotelId,
                CheckIn = checkIn,
                CheckOut = checkOut,
                OtaType = OtaType.Ctrip
            };

            return otaCrawlerService.GetCanSellHotelHtml(hotelRoomParams);
        }

        private string GetOTAName(string otaename)
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
            }

            return otaName;
        }

        private int GetOTAChannelID(string otaename)
        {
            int otaChannelID = 0;
            switch (otaename.ToLower())
            {
                case "booking":
                    otaChannelID = 1;
                    break;
                case "ctrip":
                    otaChannelID = 2;
                    break;
                case "elong":
                    otaChannelID = 6;
                    break;
                case "tongcheng":
                    otaChannelID = 4;
                    break;
                case "zhuna":
                    otaChannelID = 11;
                    break;
            }

            return otaChannelID;
        }

        private string GetBookUrl(int hotelID, string otaEName, int otaHotelID, string sType)
        {
            return LocalCache.GetData<string>(string.Format("bookingurl:{0}:{1}:{2}", hotelID.ToString()
                , otaEName, sType), () =>
                {
                    return GenBookUrl(hotelID, otaEName, otaHotelID, sType);
                });
        }

        private string GenBookUrl(int hotelID, string otaEName, int otaHotelID, string sType)
        {
            string bookurl = "";


            bookurl = ResourceAdapter.GetOtaHotelUrl(otaEName, otaHotelID, "", sType);


            return bookurl;
        }

        [HttpGet]
        public string HotelTypes()
        {
            var res = HotelService.GetHotelAllClass();
            return "{" + string.Join(",", res.Select(p => p.ClassID + ":\"" + p.ClassName + "\"")) + "}";
        }

        [HttpGet]
        public ReviewResult GetComments([FromUri] ArguHotelReview p)//int hotel,  int start, int count)
        {
            return new HotelAdapter().GetHotelReviews(p);//hotel,  start, count);
        }


        /// <summary>
        /// 获取酒店的基础信息，如酒店名、点评数等 
        /// </summary>
        /// <param name="HotelID"></param>
        /// <returns></returns>
        [HttpGet]
        public HotelItem GetHotelBasicInfo(int HotelID)
        {
            return new HotelAdapter().GetHotelBasicInfo(HotelID);
        }


        [HttpGet]
        public ReviewResult2 GetComments2([FromUri] ReviewQueryParam p)//int hotel,  int start, int count)
        {
            return new HotelAdapter().GetHotelReviews2(p);//hotel,  start, count);
        }

        [HttpGet]
        public ReviewResult40 GetComments40([FromUri] ReviewQueryParam p)//int hotel,  int start, int count)
        {
            try
            {
                return new HotelAdapter().GetHotelReviews40(p, AppVer);//hotel,  start, count);
            }
            catch (Exception err)
            {
                return new ReviewResult40();
            }
        }

        [HttpGet]
        public ReviewResult40 GetComments50([FromUri] ReviewQueryParam p)//int hotel,  int start, int count)
        {
            try
            {
                return new HotelAdapter().GetHotelReviews50(p, AppVer);//hotel,  start, count);
            }
            catch (Exception err)
            {
                return new ReviewResult40();
            }
        }

        [HttpGet]
        public int GetHotelReviewCount(int hotelId)
        {
            return new HotelAdapter().GetHotelReviewCount(hotelId);
        }

        [HttpGet]
        public HotelPhotosEntity GetHotelPhotos(int HotelID)
        {
            return HotelAdapter.GetHotelPhotos(HotelID, 0);
        }

        [HttpGet]
        public HotelPhotosEntity GetHotelPhotos20(int HotelID, int InterestID)
        {
            return HotelAdapter.GetHotelPhotos(HotelID, InterestID);
        }

        [HttpGet]
        public List<CitySuggestItem> SuggestCity(string keyword, int count)
        {
            return HotelAdapter.SuggestCity(keyword, count);
        }

        [HttpGet]
        public List<QuickSearchSuggestItem> SuggestHotel(string keyword, int count)
        {
            return HotelAdapter.SuggestHotel(keyword, count);
        }

        //[HttpGet]
        //public List<QuickSearchSuggestItem> SuggestCityAndHotel(string keyword, int count)
        //{
        //    if (string.IsNullOrWhiteSpace(keyword))
        //    {
        //        return new List<QuickSearchSuggestItem>();
        //    }
        //    var list = SearchTipApiService.GetListSearchTipAllType(keyword, count);
        //    var res = (from d in list
        //               select new QuickSearchSuggestItem
        //               {
        //                   EName = d.EngName,
        //                   ParentName = d.DisName,
        //                   Name = d.CnName,
        //                   Type = d.Type,
        //                   Id = int.Parse(d.Id)
        //               }).ToList();
        //    return res;
        //}

        /// <summary>
        /// 搜索方法（目前该接口仅支持城市和酒店搜索）
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="cityCount"></param>
        /// <param name="hotelCount"></param>
        /// <returns></returns>
        [HttpGet]
        public List<QuickSearchSuggestItem> SuggestCityAndHotel(string keyword, int cityCount = 2, int hotelCount = 10)
        {
            var result = new List<QuickSearchSuggestItem>();

            if (cityCount < 0)
            {
                cityCount = 2;
            }

            if (hotelCount < 0)
            {
                hotelCount = 10;
            }

            if (cityCount > 0)
            {
                var originResult = HotelAdapter.SuggestCity(keyword, cityCount);
                if (originResult != null && originResult.Any())
                {
                    var districtList = destService.GetDistrictInfo(originResult.Select(_ => _.Id).ToList());//地区信息集合
                    foreach (var _ in originResult)
                    {
                        var item = new QuickSearchSuggestItem()
                        {
                            EName = _.EName,
                            Icon = PhotoAdapter.GenHotelPicUrl(Configs.icon4City, Enums.AppPhotoSize.small),
                            Id = _.Id,
                            Name = _.Name,
                            ParentName = districtList.FirstOrDefault(d => d.DistrictID == _.Id) != null ? districtList.First(d => d.DistrictID == _.Id).RootName : "",
                            Tag = "",
                            Type = "D",
                            ActionUrl = string.Format("whotelapp://www.zmjiudian.com/strategy/place?districtid={0}&title={1}", _.Id, _.Name)
                        };

                        var newName = string.Format("{0}及周边", item.Name);
                        var copy = new QuickSearchSuggestItem()
                        {
                            EName = newName,
                            Icon = item.Icon,
                            Id = _.Id,
                            Name = newName,
                            ParentName = "",
                            Tag = "",
                            Type = "D",
                            ActionUrl = string.Format("whotelapp://www.zmjiudian.com/strategy/place?title={0}&geoscopetype=3&districtName={1}", newName, item.Name)
                        };
                        result.Add(item);
                        result.Add(copy);
                    }
                }
            }

            if (hotelCount > 0)
            {
                result.AddRange(HotelAdapter.SuggestHotel(keyword, hotelCount).Select(_ => new QuickSearchSuggestItem()
                {
                    EName = _.EName,
                    Icon = PhotoAdapter.GenHotelPicUrl(Configs.icon4Hotel, Enums.AppPhotoSize.small),
                    Id = _.Id,
                    Name = _.Name,
                    ParentName = _.ParentName,
                    Tag = _.Tag,
                    Type = "H",
                    ActionUrl = string.Format("whotelapp://www.zmjiudian.com/hotel/{0}", _.Id)
                }));
            }

            #region 行为记录

            try
            {
                var value = string.Format("{{\"keyword\":\"{0}\",\"cityCount\":\"{1}\",\"hotelCount\":\"{2}\"}}", keyword.Replace("|", ""), cityCount, hotelCount);
                RecordBehavior("SearchCityAndHotel", value);
            }
            catch (Exception ex) { }

            #endregion

            return result;
        }

        /// <summary>
        /// 发布酒店点评
        /// </summary>
        /// <param name="id">酒店id</param>
        /// <param name="name">酒店名称</param>
        /// <param name="title">标题</param>
        /// <param name="text">内容</param>
        /// <param name="ratingRoom">房间卫生</param>
        /// <param name="ratingAtmosphere">周边环境</param>
        /// <param name="ratingService">酒店服务</param>
        /// <param name="ratingCostBenefit">设施设备</param>
        /// <param name="tripRadio">出游类型</param>
        /// <param name="identity"></param>
        /// <param name="recommend">是否推荐</param>
        /// <returns></returns>
        [AppAuthorize]
        [HttpPost]
        public OperationResult PostReview(HotelReviewItem r)
        {
            if (!Authorize())
            {
                return null;
            }

            var result = new OperationResult();



            HotelReviewEntity hr = new HotelReviewEntity();

            //    rate=0&ratingRoom={0}&ratingAtmosphere={1}&ratingService={2}&ratingCostBenefit={3}&tripRadio={4}&reviewTitle={5}&reviewContent={6}&resourceName={7}&resource={8}
            //&identitytxt={9}&recRadio={10}&uid={11}&key={12}&source={13}",
            hr.RatingRoom = r.ratingRoom;
            hr.RatingAtmosphere = r.ratingAtmosphere;
            hr.RatingService = r.ratingService;
            hr.RatingCostBenefit = r.ratingCostBenefit;
            hr.CommentSubject =
                hr.Title = r.title;// HttpUtility.UrlEncode(r.title, Encoding.GetEncoding("GB2312"));
            hr.Content = r.text;// HttpUtility.UrlEncode(r.text, Encoding.GetEncoding("GB2312"));
            hr.RoomName = r.name;// HttpUtility.UrlEncode(r.name, Encoding.GetEncoding("GB2312"));
            hr.Hotel = r.id;
            hr.WholeRate = r.wholeRate;
            hr.NewisCommand = r.recommend ? "T" : "F";
            hr.Uid = Uid;
            hr.UserID = AccountAdapter.GetCurrentUserID();
            hr.Source = r.source;
            hr.Deleted = "F";
            hr.Status = "P";
            hr.StatusDetail = 1;



            int user_Identity = 0;
            string identitytxt = "";
            switch (r.tripRadio)
            {
                case "10":
                    user_Identity = 10;
                    identitytxt = "商务出差";
                    break;
                case "20":
                    user_Identity = 20;
                    identitytxt = "带小孩出游";
                    break;
                case "30":
                    user_Identity = 30;
                    identitytxt = "和家人出游";
                    break;
                case "40":
                    user_Identity = 40;
                    identitytxt = "和朋友出游";
                    break;
                case "50":
                    user_Identity = 50;
                    identitytxt = "独自出游";
                    break;
                case "60":
                    user_Identity = 60;
                    identitytxt = "代人预订";
                    break;
                case "70":
                    user_Identity = 70;
                    identitytxt = "夫妇、情侣出游";
                    break;
                default:
                    user_Identity = 0;
                    identitytxt = r.identity != null && r.identity != "" ? r.identity : "其他";
                    break;
            }
            hr.User_Identity = user_Identity;
            hr.Identitytxt = identitytxt;

            HotelReviewSaveResult hrs = HotelService.HotelReviewSubmitToHotelDB(hr);

            if (hrs.Writing > 0)
            {
                result.Success = true;
                result.Data = hrs.Writing.ToString();
            }
            return result;
        }

        /// <summary>
        /// 获取目的地中档价格区间段(最低价和最高价）
        /// </summary>
        /// <param name="districtID"></param>
        /// <returns></returns>
        public List<int> GetPriceSectionMinMax(int districtID)
        {
            return LocalCache.GetData<List<int>>("PriceSecMM" + districtID.ToString(), () =>
                {
                    return HotelService.GetZhongdangPriceSection(districtID);
                });
        }

        //public int[] GetPriceSection(int type , int districtID)
        //{
        //   Dictionary<int,int> pl =  LocalCache.GetData < Dictionary<int, int>>("PriceSec" + districtID.ToString(), () =>
        //    {  
        //        HotelSearchParas p = new HotelSearchParas();
        //        p.DistrictID = districtID;
        //        p.ClassID = new int[1];
        //        p.ClassID[0] = 0;
        //        p.Valued = 0;
        //        p.MinPrice = 0;
        //        p.MaxPrice = 0;

        //        Dictionary<int,string> fp =   GetWapMenuEntity( p,needTagLength).filterPrice;
        //        Dictionary<int, int> lp = new Dictionary<int, int>();
        //        foreach (int k in fp.Keys)
        //        {
        //            lp.Add(k, int.Parse(fp[k].Split('|')[0]));
        //        }
        //        lp.Add(fp.Count + 1, int.Parse(fp[fp.Count].Split('|')[1]));
        //        return lp;
        //    });
        //    int minPrice = 0;
        //    int maxPrice = 0;
        //    switch (type)
        //    {
        //        case 200:
        //            minPrice = 0;
        //            maxPrice = pl[1];
        //            break;
        //        case 201:
        //            minPrice = pl[1];
        //            maxPrice = pl[pl.Count/2 + 1];
        //            break;
        //        case 202:
        //            minPrice = pl[pl.Count / 2 + 1];
        //            maxPrice = pl[pl.Count];
        //            break;
        //        case 203:
        //            minPrice = pl[pl.Count ];
        //            maxPrice = MaxHotelPrice;
        //            break;
        //    }

        //    return new int[] { minPrice, maxPrice };

        //}

        public AppMenuEntity GetWapMenuEntity(HotelSearchParas p, int needTagLength)
        {
            return LocalCache.GetData<AppMenuEntity>(string.Format("WapMenu{0}", p.Attraction), () =>
                {
                    AppMenuEntity menu = new AppMenuEntity();

                    HotelSearchParas tp = new HotelSearchParas();

                    tp.Attraction = p.Attraction;
                    tp.Type = 0;
                    tp.NeedFilterCol = true;
                    tp.NeedHotelID = false; //不需要返回酒店列表
                    tp.CheckInDate = DateTime.Now;
                    tp.CheckOutDate = DateTime.Now;


                    List<HJD.HotelServices.Contracts.FilterDicEntity> flist = HotelService.GetQueryHotelFilters(tp);

                    if (flist.Count > 0)
                        menu.TotalNum = flist.Where(o => o.Key == Struct.HotelListFilterPrefix.Attraction + p.Attraction).FirstOrDefault().Num;

                    List<HotelTypeDefine> hotelClassDefine = GetHotelClassDefine();

                    foreach (HotelTypeDefine hc in hotelClassDefine)
                    {
                        TypeItem item = new TypeItem();
                        tp.Type = hc.TypeID;
                        tp.ClassID = (from c in hc.SubHotelClass
                                      select c.Key).ToArray();

                        flist = HotelService.GetQueryHotelFilters(tp);



                        item.TypeName = hc.Name;
                        item.TypeID = hc.TypeID;
                        FilterDicEntity TotalNumItem = flist.Where(o => o.Key == Struct.HotelListFilterPrefix.Attraction + p.Attraction).FirstOrDefault();
                        if (TotalNumItem != null)
                        {
                            item.TotalNum = TotalNumItem.Num;

                            item.FeaturedList = (from b in flist
                                                 where b.Type == (int)(Struct.HotelListFilterPrefix.Featured / Struct.HotelListFilterPrefix.BaseOffset)
                                                 orderby b.Num descending
                                                 select b).Take(needTagLength).ToList();

                            menu.TypeItems.Add(item);
                        }
                    }

                    return menu;
                });
        }

        public List<HotelTypeDefine> GetHotelClassDefine()
        {
            return LocalCache.GetData<List<HotelTypeDefine>>("HotelClassDefine", () =>
                {
                    List<HotelTypeDefine> list = new List<HotelTypeDefine>();
                    foreach (string c in Configs.AppHotelClass.Split('|'))
                    {
                        HotelTypeDefine item = new HotelTypeDefine();
                        item.TypeID = int.Parse(c.Split(',')[0]);
                        item.Name = c.Split(',')[1];
                        item.SubHotelClass = new Dictionary<int, string>();
                        foreach (string sc in c.Split(',')[2].Split('+'))
                        {
                            item.SubHotelClass.Add(int.Parse(sc.Split(':')[0]), sc.Split(':')[1]);
                        }
                        list.Add(item);
                    }
                    return list;
                });
        }

        /// <summary>
        /// 转换货币代码与符号
        /// </summary>
        /// <param name="currency"></param>
        /// <returns></returns>
        private string TransCurrencyToSymbol(string currency)
        {

            return "￥";
        }

        //<summary>
        //将抓取的点评送往生产环境
        //</summary>
        [HttpPost]
        public string MoveReview_17U(MoveReview17U mr)
        {
            try
            {
                HotelService.MoveReview17U(mr);
            }
            catch (Exception e)
            {
                return e.Message;
            }
            return "Success";
        }

        #region Ctrip 抓取数据相关处理

        [HttpGet]
        public int UpdateHotelStatus(long hid, long realhotelid, int status, int crawlercount)
        {
            Hotels hotels = new Hotels
            {
                HID = hid,
                RealHotelID = realhotelid,
                Status = status,
                CrawlerCount = crawlercount
            };
            return otaCrawlerService.UpdateHotelStatus(hotels);
        }

        [HttpGet]
        public int InsertHistorys(long hid, int status, string proxy, string message)
        {
            Historys historys = new Historys
            {
                HID = hid,
                Status = status,
                Proxy = proxy,
                Message = message,
                CreateTime = DateTime.Now
            };
            return otaCrawlerService.InsertHistorys(historys);
        }

        [HttpPost]
        public string InsertCtripRoom(CrawlerHotelBaseRoom baseRoom)
        {
            return otaCrawlerService.InsertCtripRoom(baseRoom);
        }

        [HttpPost]
        public int InsertCtripPriceRate(CrawlerHotelRoom room)
        {
            return otaCrawlerService.InsertCtripPriceRate(room);
        }


        [HttpGet]
        public int InsertProxyData(string ipAddress, string ip, string port)
        {
            ProxyData proxy = new ProxyData
            {
                IpAddress = ipAddress,
                Ip = ip,
                Port = port
            };
            return otaCrawlerService.InsertProxyData(proxy);
        }

        [HttpGet]
        public int InsertOptimizeProxyData(string ipAddress, string ip, string port)
        {
            ProxyData proxy = new ProxyData
            {
                IpAddress = ipAddress,
                Ip = ip,
                Port = port
            };
            return otaCrawlerService.InsertOptimizeProxyData(proxy);
        }

        [HttpGet]
        public int PreHotelsData(string night, int groupId)
        {
            return otaCrawlerService.PreHotelsData(DateTime.Parse(night), groupId);
        }

        [HttpGet]
        public int UpdateRealHotelBase(string hid, string realhotelid)
        {
            return otaCrawlerService.UpdateRealHotelBase(hid, realhotelid);
        }

        [HttpPost]
        public int InsertProductCtripRoom(CrawlerHotelBaseRoom baseRoom)
        {
            return otaCrawlerService.InsertProductCtripRoom(baseRoom);
        }

        [HttpPost]
        public int InsertProductCtripPriceRate(CrawlerHotelRoom room)
        {
            return otaCrawlerService.InsertProductCtripPriceRate(room);
        }

        [HttpGet]
        public int InsertRoomMatch(long baseRoomID, string otaRoomName, long proomID, string pRoomCode, long hotelID, long hotelOriID, decimal matchRate, int matched, DateTime matchTime)
        {
            RoomMatch roomMatch = new RoomMatch
            {
                BaseRoomID = baseRoomID,
                OtaRoomName = otaRoomName,
                PRoomID = proomID,
                PRoomCode = pRoomCode,
                HotelID = hotelID,
                HotelOriID = hotelOriID,
                MatchRate = matchRate,
                Matched = matched,
                MatchTime = matchTime
            };
            return otaCrawlerService.InsertRoomMatch(roomMatch);
        }

        [HttpPost]
        public int InsertPackageMatch(PackageMatch packageMatch)
        {
            return otaCrawlerService.InsertPackageMatch(packageMatch);
        }

        [HttpGet]
        public int BindRoomMatchRate()
        {
            return otaCrawlerService.BindRoomMatchRate();
        }

        [HttpGet]
        public int BakTodayHotels()
        {
            return otaCrawlerService.BakTodayHotels();
        }

        [HttpGet]
        public int DeleteProxyData()
        {
            return otaCrawlerService.DeleteProxyData();
        }

        [HttpGet]
        public int UpdateCrawlerConfig(string code, string value)
        {
            //return 1;
            return otaCrawlerService.UpdateCrawlerConfig(code, value);
        }

        [HttpPost]
        public int InsertPricePlan(PricePlan pricePlan)
        {
            return otaCrawlerService.InertPricePlan(pricePlan);
        }

        [HttpGet]
        public int UpdateOldPriceRate(string night)
        {
            return otaCrawlerService.UpdateOldPriceRate(DateTime.Parse(night));
        }

        #endregion

        /// <summary>
        /// 选择可以显示的主题照片(酒店主题)
        /// </summary>
        /// <param name="interestHotelList"></param>
        /// <param name="useInterestImg"></param>
        /// <returns></returns>
        private List<int> RandomChooseInterestHotel(List<InterestEntity> interestHotelList, bool useInterestImg)
        {
            //1.处理打分相同的酒店 随机展示
            Dictionary<string, List<string>> dic = new Dictionary<string, List<string>>();//各主题 不同评分等级的酒店信息字典
            List<int> totalInterestIDs = new List<int>();
            foreach (InterestEntity ie in interestHotelList)
            {
                if (!string.IsNullOrEmpty(ie.HotelList))
                {
                    var hotelArray = ie.HotelList.Split(',');
                    int totalCount = hotelArray.Count();
                    for (int j = 0; j < totalCount; j++)
                    {
                        var hotelInfo = hotelArray[j];
                        var hotelDataArray = hotelInfo.Split(':');
                        var score = hotelDataArray[4];
                        //var key = ie.ID + ',' + score;//,ASCII码是几十 变成数字相加了
                        var key = ie.ID + "," + score;
                        if (!dic.ContainsKey(key))
                        {
                            dic.Add(key, new List<string>() { hotelInfo });
                        }
                        else
                        {
                            dic[key].Add(hotelInfo);
                        }

                        if (!totalInterestIDs.Contains(ie.ID))
                        {
                            totalInterestIDs.Add(ie.ID);
                        }
                    }
                }
            }
            //List<InterestEntity> resultThemeList = new List<InterestEntity>();
            //2.如果酒店的主题照片未上传 以酒店的默认照片代替
            //3.如果一家酒店对应多个主题 重复时调整
            List<int> existHotelIds = new List<int>();
            List<int> existThemeIds = new List<int>();
            foreach (var key in dic.Keys)
            {
                var interestID = Convert.ToInt32(key.Split(',')[0]);
                var ie = interestHotelList.FirstOrDefault(i => i.ID == interestID);//还是原序列的记录
                if (ie.HotelCount > 0 && !existThemeIds.Contains(interestID))
                {
                    List<string> list = dic[key];
                    Random rd = new Random();
                    int listLength = list.Count;
                    int index = rd.Next(listLength);

                    string hotelInfo = list[index];
                    var hotelDataArray = hotelInfo.Split(':');
                    var hotelID = Convert.ToInt32(hotelDataArray[1]);
                    var imgUrl = hotelDataArray[2];
                    imgUrl = ValidateImgExistance(imgUrl, hotelID);
                    if (!existHotelIds.Contains(hotelID) && !string.IsNullOrEmpty(imgUrl))
                    {
                        existHotelIds.Add(hotelID);
                        existThemeIds.Add(interestID);
                    }
                    else
                    {
                        hotelID = 0;
                        int maxLoopCount = listLength < 2 ? 0 : 10 * listLength;
                        Random rd2 = new Random();
                        //和最优先级平级的酒店信息 随机出现
                        while (maxLoopCount-- > 0)
                        {
                            index = rd2.Next(listLength);
                            hotelInfo = list[index];
                            hotelDataArray = hotelInfo.Split(':');
                            hotelID = Convert.ToInt32(hotelDataArray[1]);
                            imgUrl = hotelDataArray[2];
                            imgUrl = ValidateImgExistance(imgUrl, hotelID);
                            if (existHotelIds.Contains(hotelID) || string.IsNullOrEmpty(imgUrl))
                            {
                                hotelID = 0;
                                continue;
                            }
                            else
                            {
                                break;
                            }
                        }

                        if (hotelID > 0)
                        {
                            existHotelIds.Add(hotelID);
                            existThemeIds.Add(interestID);
                        }
                        else
                        {
                            continue;
                        }
                    }

                    //依次为酒店名称 酒店ID 图片短名称 一句话描述 评分
                    var hotelName = hotelDataArray[0];
                    var description = hotelDataArray[3];


                    //15-05-15 ToDel 发布时以下代码必须删除 wwb  15-08-28这段代码是为了后台更新主页推广酒店信息 可以立即看到最新内容
                    //List<HJD.HotelManagementCenter.Domain.HomeThemeHotelEntity> htheList = HotelAdapter.HMC_HotelService.GetHomeThemeHotelEntityList(hotelID, interestID).FindAll(_ => !_.IsDel);
                    //if (htheList != null && htheList.Count > 0)
                    //{
                    //    if(!string.IsNullOrWhiteSpace(htheList[0].ShortUrl)){
                    //        imgUrl = PhotoAdapter.GenHotelPicUrl(htheList[0].ShortUrl, Enums.AppPhotoSize.theme);
                    //    }
                    //    description = htheList[0].Description;
                    //}
                    //以上代码发布时必须删除

                    ie.ImageUrl = !useInterestImg || string.IsNullOrWhiteSpace(ie.ImageUrl) ? imgUrl : ie.ImageUrl.Replace("290x290s", "jupiter");
                    ie.ADDescription = description;
                    ie.HotelID = hotelID;
                    ie.HotelName = hotelName;
                }
            }
            return existThemeIds;
        }

        private static string ValidateImgExistance(string imgUrl, int hotelID)
        {
            if (string.IsNullOrEmpty(imgUrl) || imgUrl.Trim().Length == 0)
            {
                HotelPhotosEntity hps = HotelAdapter.GetHotelPhotos(hotelID, 0);
                if (hps.HPList.Count > 0)
                {
                    List<HotelPhotoEntity> photoList = hps.HPList.FindAll(i => !i.Deleted && !string.IsNullOrEmpty(i.SURL) && i.SURL.Trim().Length > 0).ToList();
                    HotelPhotoEntity hpe = photoList.FirstOrDefault(i => i.IsCover);
                    if (hpe != null && hpe.HotelID > 0)
                    {
                        imgUrl = PhotoAdapter.GenHotelPicUrl(hpe.SURL, Enums.AppPhotoSize.theme);
                    }
                    else if (photoList.Count > 0)
                    {
                        imgUrl = PhotoAdapter.GenHotelPicUrl(photoList[0].SURL, Enums.AppPhotoSize.theme);
                    }
                    else
                    {
                        imgUrl = null;
                    }
                }
                else
                {
                    imgUrl = null;
                }
            }
            else
            {
                imgUrl = PhotoAdapter.GenHotelPicUrl(imgUrl, Enums.AppPhotoSize.theme);
            }
            return imgUrl;
        }

        [HttpGet]
        public List<TopNPackageItem> GetTop20Package()
        {
            return HotelAdapter.GetTop20Package(25);
        }

        /// <summary>
        /// 获取所有的节假日数据
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public List<HJD.HotelManagementCenter.Domain.HolidayInfoEntity> GetHolidays()
        {
            return HotelAdapter.GetHolidays();
        }

        /// <summary>
        /// 酒店地图位置(国内国外都支持)信息
        /// </summary>
        /// <param name="hotelID">酒店ID</param>
        /// <returns></returns>
        [HttpGet]
        public HotelMapBasicInfo GetHotelMapInfo(int hotelID)
        {
            return HotelAdapter.GetHotelMapInfo(hotelID);
        }

        /// <summary>
        /// 获取周边的攻略列表
        /// </summary>
        /// <param name="lon"></param>
        /// <param name="lat"></param>
        /// <param name="districtName"></param>
        /// <param name="districtID"></param>
        /// <returns></returns>
        [HttpGet]
        public StrategyADResult GetStrategyADList(string districtName, float lon = 0, float lat = 0, int districtID = 0)
        {
            if (districtID == 0 && !string.IsNullOrWhiteSpace(districtName))
            {
                districtName = HttpUtility.UrlDecode(districtName);
                var targetCity = GetZMJDCityData2().Citys.FirstOrDefault(_ => _.Name.Trim() == districtName.Trim());
                districtID = targetCity != null ? targetCity.ID : 0;
            }

            return ADAdapter.GetStrategyADList(districtID, lon, lat);
        }


        [HttpPost]
        public bool CacheCrawlHotelPackage(CrawlerHotel result)
        {
            return otaCrawlerService.CacheCrawlHotelPackage(result);
        }

        #region 抓取与业务数据处理层相关操作【OTA借用下HotelController】

        [HttpGet]
        public int UpdatePackageSourceConfigValid(Int64 id, int valid)
        {
            return otaCrawlerService.UpdatePackageSourceConfigValid(id, valid) ? 1 : 0;
        }

        #endregion

        #region 微信地理位置

        [HttpGet]
        public WapInterestHotelsResult3 SearchInterestHotelWeixin(int districtId, int interestId = 0)
        {
            return HotelAdapter.SearchInterestHotelWeixin(districtId, interestId);
        }

        [HttpGet]
        public List<string> GetProvinceListByInterest(int interestId)
        {
            return HotelAdapter.GetProvinceListByInterest(interestId);
        }

        #endregion

        #region App Home H5新接口

        [HttpGet]
        public HomePageData30 GetAppHomePageData(long curUserID = 0)
        {
            long userId = curUserID == 0 ? AppUserID : curUserID;
            var dateNow = DateTime.Now.Date;

            var result = new HomePageData30();
            //获得某个状态下 某个活动类型的券
            int totalCount = 0;
            var originActivityList = CouponAdapter.couponSvc.GetCouponActivityList(
                new HJD.CouponService.Contracts.Entity.CouponActivityQueryParam()
            {
                stateArray = new int[] { 1 },
                PageSize = 20,
                PageIndex = 1,
                activityTypeArray = new int[] { 200, 300 },
                merchantCode = HJD.CouponService.Contracts.Entity.CouponActivityMerchant.zmjd
            }, out totalCount).FindAll(_ => _.SaleEndDate > dateNow);

            result.FlashDeals = CouponAdapter.TransRoomCouponActivity(originActivityList, 200);
            result.GroupDeals = CouponAdapter.TransRoomCouponActivity(originActivityList, 300);

            return result;
        }

        /// <summary>
        /// 获取闪购
        /// </summary>
        /// <param name="curUserID"></param>
        /// <returns></returns>
        [HttpGet]
        public HomePageData30 GetAppHomeFlashData(long curUserID = 0)
        {
            long userId = curUserID == 0 ? AppUserID : curUserID;

            var result = new HomePageData30();
            //获得某个状态下 某个活动类型的券
            int totalCount = 0;
            var originActivityList = CouponAdapter.couponSvc.GetCouponActivityList(
                new HJD.CouponService.Contracts.Entity.CouponActivityQueryParam()
                {
                    stateArray = new int[] { 1 },
                    PageSize = 10,
                    PageIndex = 1,
                    activityTypeArray = new int[] { 200 },
                    merchantCode = HJD.CouponService.Contracts.Entity.CouponActivityMerchant.zmjd
                }, out totalCount).FindAll(_ => _.IsValid && _.SaleEndDate > DateTime.Now.Date);

            result.FlashDeals = CouponAdapter.TransRoomCouponActivity(originActivityList, 200);
            result.GroupDeals = new List<RoomCouponActivityModel>();

            return result;
        }

        /// <summary>
        /// 获取团购
        /// </summary>
        /// <param name="curUserID"></param>
        /// <returns></returns>
        [HttpGet]
        public HomePageData30 GetAppHomeGroupData(long curUserID = 0)
        {
            long userId = curUserID == 0 ? AppUserID : curUserID;

            var result = new HomePageData30();
            //获得某个状态下 某个活动类型的券
            int totalCount = 0;
            var originActivityList = CouponAdapter.couponSvc.GetCouponActivityList(
                new HJD.CouponService.Contracts.Entity.CouponActivityQueryParam()
                {
                    stateArray = new int[] { 1 },
                    PageSize = 10,
                    PageIndex = 1,
                    activityTypeArray = new int[] { 300 },
                    merchantCode = HJD.CouponService.Contracts.Entity.CouponActivityMerchant.zmjd
                }, out totalCount).FindAll(_ => _.SaleEndDate > DateTime.Now.Date);

            result.FlashDeals = new List<RoomCouponActivityModel>();
            result.GroupDeals = CouponAdapter.TransRoomCouponActivity(originActivityList, 300);

            return result;
        }

        #endregion

        #region App4.3 版本 搜索标签块 搜索酒店列表接口

        /// <summary>
        /// （标签块）搜索酒店列表
        /// 变化 标签块调整
        /// 查找页 排序调整 需要输出排序选项
        /// 发现以及攻略页面进入列表页仅标签块变化
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        [HttpPost]
        public SearchHotelListResult SearchHotelList20(HotelListQueryParam20 p)
        {
            if (AppVer.CompareTo("4.1") >= 0)
            {
                p.hotelId = 0;//4.1版本之后 不再将精选页面某个主题排第一的酒店ID传到后台
            }

            if (p.geoScopeType == 3 && p.districtid > 0 && !string.IsNullOrWhiteSpace(p.districtName) && p.lat != 0 && p.lng != 0)
            {
                p.districtid = 0;//删掉一个过度的参数 2016—05-30 wwb
            }

            p.IsNeedHotelList = true;
            p.IsNeedFilterCol = false;

            if (p.sort == 20 && (p.FilterTags == null || p.FilterTags.Count == 0 || string.IsNullOrWhiteSpace(p.FilterTags[0].Name)))
            {
                p.sort = 0;//如果没有选择任何过滤标签 排序默认按照好评
            }

            p.SupportWebP = HotelAdapter.JudgeSupportWebP(AppVer, AppType);

            SearchHotelListResult result = HotelAdapter.SearchHotelList30(_ContextBasicInfo, p);
            result.FilterMenus = new HotelListMenu();

            result.FilterBlocks = new FilterBlockModel();
            if (p.interest != 0 || p.zoneId != 0 || (p.districtid > 0 && string.IsNullOrWhiteSpace(p.districtName)))
            {
                //影响搜索FilterTag出现的条件
                var p2 = new HotelListQueryParam20()
                {
                    IsNeedHotelList = false,
                    IsNeedFilterCol = true,
                    sort = p.districtid > 0 && string.IsNullOrWhiteSpace(p.districtName) ? 200 : 0,
                    order = 0,
                    districtid = p.districtid,
                    aroundCityId = p.aroundCityId,
                    districtName = p.districtName,
                    geoScopeType = p.geoScopeType,
                    interest = p.interest,
                    zoneId = p.zoneId,
                    lat = p.lat,
                    lng = p.lng,
                    distance = p.distance
                };

                var argus = HotelAdapter.TransHotelListQueryParam202HotelSearchParas(p2);
                result.FilterBlocks.TagBlocks = HotelAdapter.TransFilterCol2FilterTagBlock(argus);
                result.SortOptions = new List<HotelListSortOption>();

                if (p.interest > 0 && result.TotalCount > 0)
                {
                    //var interestName = result.Result20.First().HotelRelFilterTags.First(_ => _.BlockCategoryID == 2 && _.Value == p.interest.ToString()).Name;
                    var interestEntity = HotelAdapter.GetOneInterestEntity(p.interest);
                    var titleContent = string.Format("{0}{1}{2}酒店", p.districtName, p.geoScopeType == 3 ? "及周边" : "", interestEntity.Name);
                    var description = string.Format("快来看看这里{0}家好口碑的{1}酒店", result.TotalCount, interestEntity.Name);//ToDo web没有周边地区

                    //sharelink
                    var shareLink = string.Format("http://www.zmjiudian.com/city{0}/theme{1}", argus.DistrictID, p.interest);
                    if (p.geoScopeType == 3)
                    {
                        shareLink = string.Format("http://www.zmjiudian.com/zone{0}/theme{1}?userLat={2}&userLng={3}", argus.DistrictID, p.interest, p.districtLat, p.districtLng);
                    }

                    result.ShareModel = new CommentShareModel()
                    {
                        Content = description,
                        shareLink = shareLink,
                        notHotelNameTitle = "",
                        photoUrl = string.IsNullOrEmpty(interestEntity.ImageUrl) ? PhotoAdapter.GenHotelPicUrl(DescriptionHelper.defaultZMJDLogo, Enums.AppPhotoSize.small).Replace("_small", "_290x290s") : interestEntity.ImageUrl,
                        returnUrl = "",
                        title = titleContent
                    };
                }
            }
            else
            {
                result.SortOptions = new List<HotelListSortOption>(){
                    new HotelListSortOption(){
                        SortName = "默认排序",
                        Sort = 20
                    },
                    new HotelListSortOption(){
                        SortName = "好评优先",
                        Sort = 0
                    },
                    new HotelListSortOption(){
                        SortName = "价格最低",
                        Sort = 3
                    },
                    new HotelListSortOption(){
                        SortName = "价格最高",
                        Sort = 4
                    }
                };
            }

            if (result.ShareModel == null || string.IsNullOrWhiteSpace(result.ShareModel.title))
            {
                result.ShareModel = new CommentShareModel()
                {
                    Content = "",
                    shareLink = "",
                    notHotelNameTitle = "",
                    photoUrl = "",
                    returnUrl = "",
                    title = ""
                };
            }

            //对酒店列表中的酒店做后台列表价动态更新操作
            HotelAdapter.PublishUpdatePriceSlotTask(result, p.checkin);

            return result;
        }

        /// <summary>
        /// 搜索查询标签块
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        [HttpGet]
        public FilterBlockModel GetHotelSearchFilter([FromUri]HotelListQueryParam20 p)
        {
            if (AppVer.CompareTo("4.1") >= 0)
            {
                p.hotelId = 0;//4.1版本之后 不再将精选页面某个主题排第一的酒店ID传到后台
            }

            FilterBlockModel result = new FilterBlockModel();
            p.IsNeedHotelList = false;
            p.IsNeedFilterCol = true;

            var argus = HotelAdapter.TransHotelListQueryParam202HotelSearchParas(p);
            var newArgus = new HotelSearchParas()
            {
                DistrictID = argus.DistrictID,
                SortType = 0,
                CheckInDate = argus.CheckInDate,
                CheckOutDate = argus.CheckOutDate,
                Distance = 0,
                Lat = 0,
                Lng = 0,
                MinPrice = 0,
                MaxPrice = 0
            };

            var allTagBlocks = HotelAdapter.TransFilterCol2FilterTagBlock(newArgus);

            if (allTagBlocks != null && allTagBlocks.Count != 0)
            {
                allTagBlocks = allTagBlocks.FindAll(_ => _.BlockCategoryID != 5 && _.BlockCategoryID != 6 && _.BlockCategoryID != 7);
                allTagBlocks.ForEach((_) =>
                {
                    _.Tags.ForEach((j) => { j.PinyinName = PinYinConverter.GetAllPinyinStr(j.Name); });
                    result.TagBlocks.Add(_);

                    if (_.BlockCategoryID == 9) return;//其他标签 不显示
                    var tempTagBlock = new FilterTagBlock()
                    {
                        BlockCategoryID = _.BlockCategoryID,
                        BlockTitle = _.BlockTitle,
                        MinTags = 0,
                        MaxTags = 100,
                        Tags = HotelAdapter.GenHotFilterTagsByFilterBlockCategoryId(_.BlockCategoryID, _.Tags)
                    };//主题特色和酒店类型要求全部显示 设施默认11个 其他默认出现7个
                    result.HotTagBlocks.Add(tempTagBlock);
                });

                var districtHotFilterTags = HotelAdapter.GetDistrictHotFilterTagList(argus.DistrictID);//获取地区ID涉及的热门标签块集合
                if (districtHotFilterTags != null && districtHotFilterTags.Count != 0)
                {
                    var rightTags = (from items in districtHotFilterTags.GroupBy(_ => _.CategoryId)
                                     let blockCategoryId = HotelAdapter.MapTagBlockCategoryId(items.Key)
                                     let curCategoryTags = allTagBlocks.Find(_ => _.BlockCategoryID == blockCategoryId)
                                     select new FilterTagBlock()
                                     {
                                         BlockCategoryID = blockCategoryId,
                                         BlockTitle = curCategoryTags.BlockTitle,
                                         MaxTags = curCategoryTags.MaxTags,
                                         MinTags = curCategoryTags.MinTags,
                                         Tags = (from item in items
                                                 join i in curCategoryTags.Tags
                                                 on item.Value.ToString() equals i.Value
                                                 select new FilterTag()
                                                 {
                                                     BlockCategoryID = blockCategoryId,
                                                     HotelCount = item.HotelCount,
                                                     IsMatch = false,
                                                     Value = i.Value,
                                                     PinyinName = i.PinyinName,
                                                     Name = i.Name
                                                 }).ToList()
                                     }).ToList();
                    result.HotTagBlocks = rightTags;//把热门区域标签块 替换为数据库中现存的
                }
                else
                {
                    districtHotFilterTags = new List<DistrictHotFilterTagEntity>();
                    foreach (var tagBlock in result.HotTagBlocks)
                    {
                        districtHotFilterTags.AddRange(tagBlock.Tags.Select(_ => new DistrictHotFilterTagEntity()
                        {
                            CategoryId = HotelAdapter.MapCategoryId(_.BlockCategoryID),
                            Value = int.Parse(_.Value),
                            HotelCount = _.HotelCount,
                            DistrictId = argus.DistrictID
                        }));
                    }
                    HotelAdapter.UpsertDistrictHotFilterTagList(districtHotFilterTags);
                }
            }
            return result;
        }

        /// <summary>
        /// 酒店列表页查询【APP 5.0 USE】    sort等于30 按照套餐优先级匹配排序
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        [HttpPost]
        public SearchHotelListResult SearchHotelList30(HotelListQueryParam20 p)
        {
            if (p.geoScopeType == 3 && p.districtid > 0 && !string.IsNullOrWhiteSpace(p.districtName) && p.lat != 0 && p.lng != 0)
            {
                p.districtid = 0;//删掉一个过度的参数 2016—05-30 wwb
            }

            p.sort = 30;//2016.06.21 V4.6 默认排序方式 有我们相关优惠套餐的优先显示

            p.IsNeedHotelList = true;
            p.IsNeedFilterCol = false;

            //地图里面默认展示酒店数量  取四个角的中心点
            if (p.Top_Left_Lat != 0 || p.Top_Right_Lat != 0 || p.Bottom_Left_Lat != 0 || p.Bottom_Right_Lat != 0)
            {

                var centerLat = (p.Top_Left_Lat + p.Top_Right_Lat + p.Bottom_Left_Lat + p.Bottom_Right_Lat) / 4;
                var centerLng = (p.Top_Left_Lng + p.Top_Right_Lng + p.Bottom_Left_Lng + p.Bottom_Right_Lng) / 4;

                p.start = 0;
                p.count = 25;

                if (p.geoScopeType == 3 && p.interest > 0)
                {
                    p.districtid = 0;
                    p.districtName = "";

                    p.districtLat = centerLat;
                    p.districtLng = centerLng;
                    p.lat = centerLat;
                    p.lng = centerLng;
                }
            }
            TimeLog log = new TimeLog(string.Format("SearchHotelList30:checkin{0} checkout{1} districtid{2}  ", p.checkin, p.checkout, p.districtid)
, 1000, null);
            p.SupportWebP = HotelAdapter.JudgeSupportWebP(AppVer, AppType);
            SearchHotelListResult result = new SearchHotelListResult();//搜索酒店数据
            try
            {
                result = HotelAdapter.SearchHotelList30(_ContextBasicInfo, p);//搜索酒店数据

                log.AddLog("HotelAdapter.SearchHotelList30");
            }
            catch (Exception e)
            {
                Log.WriteLog("SearchHotelList30 error:" + e);
            }
            result.FilterMenus = new HotelListMenu();
            result.FilterBlocks = new FilterBlockModel();
            //p.interest != 0 || p.zoneId != 0 || (p.districtid > 0 && string.IsNullOrWhiteSpace(p.districtName))
            //if (true)
            //{
                //影响搜索FilterTag出现的条件
                var p2 = new HotelListQueryParam20()
                {
                    IsNeedHotelList = false,
                    IsNeedFilterCol = true,
                    sort = 30,//p.districtid > 0 && string.IsNullOrWhiteSpace(p.districtName) ? 30 : 0,
                    order = 0,
                    districtid = p.districtid,
                    aroundCityId = p.aroundCityId,
                    districtName = p.districtName,
                    geoScopeType = p.geoScopeType,
                    interest = p.interest,
                    zoneId = p.zoneId,
                    lat = p.lat,
                    lng = p.lng,
                    distance = p.distance
                };

                var argus = HotelAdapter.TransHotelListQueryParam202HotelSearchParas(p2);

                log.AddLog("HotelAdapter.TransHotelListQueryParam202HotelSearchParas");

                // 6.0版本以后 不需要改字段内容  -- 20180111
                if (!_ContextBasicInfo.IsThanVer6_0)
                {
                    result.FilterBlocks.TagBlocks = HotelAdapter.TransFilterCol2FilterTagBlock(argus);

                    log.AddLog("HotelAdapter.TransFilterCol2FilterTagBlock");
                }

                result.SortOptions = new List<HotelListSortOption>();

                if (p.interest > 0 && result.TotalCount > 0)
                {
                    var interestEntity = HotelAdapter.GetOneInterestEntity(p.interest);

                    log.AddLog("HotelAdapter.GetOneInterestEntity");
                    var titleContent = string.Format("{0}{1}{2}酒店", p.districtName, p.geoScopeType == 3 ? "及周边" : "", interestEntity.Name);
                    var description = string.Format("快来看看这里{0}家好口碑的{1}酒店", result.TotalCount, interestEntity.Name);

                    //sharelink
                    var shareLink = string.Format("http://www.zmjiudian.com/city{0}/theme{1}", argus.DistrictID, p.interest);
                    if (p.geoScopeType == 3)
                    {
                        shareLink = string.Format("http://www.zmjiudian.com/zone{0}/theme{1}?userLat={2}&userLng={3}", argus.DistrictID, p.interest, p.districtLat, p.districtLng);
                    }

                    result.ShareModel = new CommentShareModel()
                    {
                        Content = description,
                        shareLink = shareLink,
                        notHotelNameTitle = "",
                        photoUrl = string.IsNullOrEmpty(interestEntity.ImageUrl) ? PhotoAdapter.GenHotelPicUrl(DescriptionHelper.defaultZMJDLogo, Enums.AppPhotoSize.small).Replace("_small", "_290x290s") : interestEntity.ImageUrl,
                        returnUrl = "",
                        title = titleContent
                    };
                }

                log.AddLog("酒店列表搜索");

                log.Finish();
            //}
            //else
            //{
            //    result.SortOptions = new List<HotelListSortOption>() { };
            //}

            if (result.ShareModel == null || string.IsNullOrWhiteSpace(result.ShareModel.title))
            {
                result.ShareModel = new CommentShareModel()
                {
                    Content = "",
                    shareLink = "",
                    notHotelNameTitle = "",
                    photoUrl = "",
                    returnUrl = "",
                    title = ""
                };
            }

            //对酒店列表中的酒店做后台列表价动态更新操作
            HotelAdapter.PublishUpdatePriceSlotTask(result, p.checkin);

            if (p.districtid > 0)
            {
                //记录用户搜索
                CommonRecordParam recordParam = new CommonRecordParam()
                {
                    userID = AppUserID,
                    appVersion = AppVer,
                    clientIP = HttpRequestHelper.GetClientIp(this.ControllerContext.Request),
                    businessType = 3,
                    busniessId = p.districtid,
                    recordType = 3,
                    openID = "",
                    sessionID = "",
                    terminalType = CommMethods.GetTerminalId(AppType)
                };
                HotelAdapter.InsertUserRecord(recordParam);
            }

            #region 行为记录

            var value = string.Format("{{\"lat\":\"{0}\",\"lng\":\"{1}\",\"districtid\":\"{2}\",\"districtName\":\"{3}\",\"geoScopeType\":\"{4}\",\"checkIn\":\"{5}\",\"checkOut\":\"{6}\",\"aroundCityId\":\"{7}\",\"interest\":\"{8}\",\"zoneId\":\"{9}\",\"minPrice\":\"{10}\",\"maxPrice\":\"{11}\",\"sort\":\"{12}\",\"ResultCount\":\"{13}\"}}",
                p.lat, p.lng, p.districtid, p.districtName, p.geoScopeType, p.checkin, p.checkout, p.aroundCityId, p.interest, p.zoneId, p.minPrice, p.maxPrice, 30, (result != null && result.Result20 != null ? result.Result20.Count : 0));
            RecordBehavior("AppHotelListLoad", value);

            #endregion

            return result;
        }

        /// <summary>
        /// 指定条件下的酒店列表（30min cache）
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        [HttpPost]
        public SearchHotelListResult SearchHotelList30Cache(HotelListQueryParam20 p)
        {
            return LocalCache30Min.GetData<SearchHotelListResult>(string.Format("SearchHotelList30Cache:{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}{11}{12}{13}", p.aroundCityId, p.districtid, p.districtName, p.districtLat, p.districtLng, p.lat, p.lng, p.geoScopeType, p.interest, p.zoneId, p.checkin, p.checkout, p.star, p.count), () =>
            {
                return SearchHotelListForWellPage(p);
            });
            //return SearchHotelListForWellPage(p);
        }

        /// <summary>
        /// 给新推荐页面开放的主题酒店列表查询方法
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        [HttpPost]
        public SearchHotelListResult SearchHotelListForWellPage(HotelListQueryParam20 p)
        {
            if (p.geoScopeType == 3 && p.districtid > 0 && !string.IsNullOrWhiteSpace(p.districtName) && p.lat != 0 && p.lng != 0)
            {
                p.districtid = 0;//删掉一个过度的参数 2016—05-30 wwb
            }

            p.sort = 30;//2016.06.21 V4.6 默认排序方式 有我们相关优惠套餐的优先显示

            p.IsNeedHotelList = true;
            p.IsNeedFilterCol = false;

            //地图里面默认展示酒店数量  取四个角的中心点
            if (p.Top_Left_Lat != 0 || p.Top_Right_Lat != 0 || p.Bottom_Left_Lat != 0 || p.Bottom_Right_Lat != 0)
            {

                var centerLat = (p.Top_Left_Lat + p.Top_Right_Lat + p.Bottom_Left_Lat + p.Bottom_Right_Lat) / 4;
                var centerLng = (p.Top_Left_Lng + p.Top_Right_Lng + p.Bottom_Left_Lng + p.Bottom_Right_Lng) / 4;

                p.start = 0;
                p.count = 25;

                if (p.geoScopeType == 3 && p.interest > 0)
                {
                    p.districtid = 0;
                    p.districtName = "";

                    p.districtLat = centerLat;
                    p.districtLng = centerLng;
                    p.lat = centerLat;
                    p.lng = centerLng;
                }
            }

            p.SupportWebP = HotelAdapter.JudgeSupportWebP(AppVer, AppType);

            //搜索酒店数据
            SearchHotelListResult result = HotelAdapter.SearchHotelList30(_ContextBasicInfo, p);

            //削减返回数据量
            if (result != null && result.Result20 != null)
            {
                foreach (var item in result.Result20)
                {
                    item.InterestComment = "";
                    if (item.PictureList != null && item.PictureList.Count > 0)
                    {
                        item.PictureList = new List<string> { item.PictureList[0] };
                    }
                }
            }

            result.FilterMenus = new HotelListMenu();
            result.FilterBlocks = new FilterBlockModel();
            result.FilterBlocks.TagBlocks = new List<FilterTagBlock>();
            result.SortOptions = new List<HotelListSortOption>();
            result.ShareModel = new CommentShareModel();

            return result;
        }

        #endregion

        #region Agoda 相关接口处理

        [HttpGet]
        public int Agoda_UploadHotelUrl(string hotelOriId, string hotelUrl, string photoUrl)
        {
            return otaCrawlerService.Agoda_UploadHotelUrl(hotelOriId, hotelUrl, photoUrl);
        }

        #endregion

        #region 酒店列表价相关处理

        [HttpGet]
        public List<OTAPackageSourceConfig> GetOtaMinPriceHotels()
        {
            return otaCrawlerService.GetOtaMinPriceHotels();
        }

        [HttpGet]
        public int InsertPricePlanAndEx(int hotelId, DateTime date, int channelId, decimal price, string name, string brief)
        {
            return otaCrawlerService.InsertPricePlanAndEx(hotelId, date, channelId, price, name, brief);
        }

        [HttpPost]
        public int InsertHotelPriceSlot(HotelPriceSlot pslot)
        {
            return otaCrawlerService.InsertHotelPriceSlot(pslot);
        }

        #endregion

        #region App4.4版本 酒店详情页照片
        public HotelRelPicData GetHotelRelPicData(int hotelId, HJDAPI.Common.Helpers.Enums.HotelRelPicSource picSource, int start = 0, int count = 20)
        {
            var result = new HotelRelPicData();
            if (hotelId > 0)
            {
                var source = (int)picSource;

                var officialPicsCount = 0;
                var officialPics = HotelAdapter.GetHotelPicFromOfficial(hotelId, start, count, out officialPicsCount);
                result.officialPicCount = officialPicsCount;
                result.officialPics = source < 2 ? officialPics : new List<HotelPicInfo>();

                var customerPicsCount = 0;//取客户照片
                var customerPics = HotelAdapter.GetHotelPicFromCustomer(hotelId, start, count, out customerPicsCount);

                result.customerPicCount = customerPicsCount;
                result.customerPics = source % 2 == 0 ? customerPics : new List<HotelPicInfo>();
            }
            return result;
        }
        #endregion

        #region 更新酒店照片标题
        [HttpPost]
        public int UpdateHotelPhotoTitle(HotelPhotoEntity entity)
        {
            return PhotoAdapter.UpdateHotelPhotoTitle(entity.ID, entity.Title);
        }
        #endregion

        #region 特惠套餐详情
        [HttpGet]
        public RecommendPackageDetailResult GetPackageDetailResult(int pid, long userId = 0, string startDate = "", string endDate = "")
        {
            userId = userId == 0 ? AppUserID : userId;
            DateTime checkInDate = string.IsNullOrWhiteSpace(startDate) ? CommMethods.GetDefaultCheckIn() : DateTime.Parse(startDate);
            DateTime checkOutDate = string.IsNullOrWhiteSpace(endDate) ? CommMethods.GetDefaultCheckIn().AddDays(1) : DateTime.Parse(endDate);

            return HotelAdapter.GetPackageDetailResult(pid, userId, checkInDate, checkOutDate);
        }
        
        [HttpGet]
        public RecommendPackageDetailResult GetPackageDetailInfo(int pid, long userId = 0, string startDate = "", string endDate = "")
        {
            userId = userId == 0 ? AppUserID : userId;
            DateTime checkInDate = string.IsNullOrWhiteSpace(startDate) ? CommMethods.GetDefaultCheckIn() : DateTime.Parse(startDate);
            DateTime checkOutDate = string.IsNullOrWhiteSpace(endDate) ? CommMethods.GetDefaultCheckIn().AddDays(1) : DateTime.Parse(endDate);

            return HotelAdapter.GetPakcageDetailInfo(pid, userId, checkInDate, checkOutDate);
        }


        
        #endregion

        #region 获取某个地区或某个坐标附近的地区列表

        /// <summary>
        /// 附近城市及地区的城市列表
        /// </summary>
        /// <param name="districtName"></param>
        /// <param name="lon"></param>
        /// <param name="lat"></param>
        /// <param name="districtID"></param>
        /// <returns></returns>
        [HttpGet]
        public AroundCityList GetAroundCityList(string districtName = "", float lon = 0, float lat = 0, int districtID = 0)
        {
            var result = new AroundCityList();
            if (districtID == 0 && !string.IsNullOrWhiteSpace(districtName))
            {
                districtName = HttpUtility.UrlDecode(districtName);
                var targetCity = GetZMJDCityData2().Citys.FirstOrDefault(_ => _.Name.Trim() == districtName.Trim());
                districtID = targetCity != null ? targetCity.ID : 0;
            }

            var aroundCitys = HotelAdapter.CalculateNearDistrictByDistance(districtID, lat, lon).FindAll(_ => _.HotelCount > 0);//只出有酒店的地区
            if (aroundCitys != null && aroundCitys.Any())
            {
                result.cityList = aroundCitys.OrderByDescending(_ => _.HotelCount).Take(12).ToList();//按酒店数量从多到少排列出
                result.currentCityName = string.IsNullOrWhiteSpace(districtName) ? aroundCitys.OrderBy(_ => _.Distance).First().DistrictName : districtName;
                result.aroundCityCount = aroundCitys.Count;
            }
            else
            {
                result.cityList = new List<ArounDistrictEntity>();
                result.currentCityName = districtName;
                result.aroundCityCount = 0;
            }
            return result;
        }

        #endregion

        /// <summary>
        /// 加载搜索页面精选与周边数据（目前主要是APP和H5的搜索页面加载使用）
        /// </summary>
        /// <param name="districtName"></param>
        /// <param name="lon"></param>
        /// <param name="lat"></param>
        /// <param name="districtID"></param>
        /// <returns></returns>
        [HttpGet]
        public ShowCityInfo GetBoutiqueAndAroundCityList(string districtName = "", float lon = 0, float lat = 0, int districtID = 0)
        {

            ShowCityInfo result = new ShowCityInfo();
            result.AroundCityList = new List<AroundCityList>();
            result.UserDefinedList = new List<UserDefinedCity>();
            if (districtID == 0 && !string.IsNullOrWhiteSpace(districtName))
            {
                districtName = HttpUtility.UrlDecode(districtName);
                var targetCity = GetZMJDCityData2().Citys.FirstOrDefault(_ => _.Name.Trim() == districtName.Trim());
                districtID = targetCity != null ? targetCity.ID : 0;
            }
            //周边好地方
            List<ArounDistrictEntity> aroundCitys = HotelAdapter.CalculateNearDistrictByDistance(districtID, lat, lon).FindAll(_ => _.HotelCount > 0).OrderByDescending(_ => _.HotelCount).Take(7).ToList();//只出有酒店的地区
            if (aroundCitys != null && aroundCitys.Count > 0)
            {
                ArounDistrictEntity model = aroundCitys.OrderBy(_ => _.Distance).FirstOrDefault();
                aroundCitys.Remove(aroundCitys.OrderBy(_ => _.Distance).FirstOrDefault());
                aroundCitys.Insert(0, model);
                aroundCitys = aroundCitys.Select(_ => new ArounDistrictEntity()
                {
                    Distance = _.Distance,
                    DistrictID = _.DistrictID,
                    DistrictName = _.DistrictName,
                    GeoScopeType = 1,
                    HotelCount = _.HotelCount,
                    Icon = Configs.searchIcon4Lab,
                    Lat = _.Lat,
                    Lon = _.Lon,
                    Type = "D",
                    ActionUrl = string.Format("http://www.zmjiudian.com/App/Around?userid={{userid}}&districtId={0}&districtName={1}&lat={2}&lng={3}&geoScopeType=1&_newpage=1&_headSearch=1&_searchType=1", _.DistrictID, _.DistrictName, _.Lat,_.Lon)
                }).ToList();
                aroundCitys.Insert(1, new ArounDistrictEntity()
                {
                    Distance = 0,
                    DistrictName = model.DistrictName + "及周边",//(string.IsNullOrWhiteSpace(districtName) ? aroundCitys.OrderBy(_ => _.Distance).First().DistrictName : districtName) + "及周边",
                    DistrictID = model.DistrictID,//2,
                    GeoScopeType = 3,
                    Icon = Configs.searchIcon4Lab,
                    Lat = model.Lat,
                    Lon = model.Lon,
                    Type = "D",
                    ActionUrl = string.Format("http://www.zmjiudian.com/App/Around?userid={{userid}}&districtId=0&districtName={0}&lat={1}&lng={2}&geoScopeType=3&_newpage=1&_headSearch=1&_searchType=1", model.DistrictName, model.Lat, model.Lon)
                });
                if (aroundCitys != null && aroundCitys.Any())
                {
                    result.AroundCityList.Add(new AroundCityList()
                    {
                        currentCityName = "推荐目的地", // aroundCitys.First().DistrictName + "周边好地方",
                        cityList = aroundCitys
                    });

                }
            }
            var cachedResult = HotelAdapter.GetZMJDAllCityData();
            var BoutiqueCitylist = new CityList()
            {
                BoutiqueCity = cachedResult.BoutiqueCity
            };
            result.AroundCityList.Add(new AroundCityList()
            {
                currentCityName = "精选度假地",
                cityList = BoutiqueCitylist.BoutiqueCity.Select(_ => new ArounDistrictEntity()
                {
                    Distance = 0F,
                    DistrictName = _.Name,
                    DistrictID = _.ID,
                    HotelCount = 0,
                    GeoScopeType = 1,
                    Icon = Configs.searchIcon4Lab,
                    Lat = double.Parse(_.lat.ToString()),
                    Lon = double.Parse(_.lon.ToString()),
                    Type = "D",
                    ActionUrl = string.Format("http://www.zmjiudian.com/App/Around?userid={{userid}}&districtId={0}&districtName={1}&lat={2}&lng={3}&geoScopeType=1&_newpage=1&_headSearch=1&_searchType=1", _.ID, _.Name, double.Parse(_.lat.ToString()), double.Parse(_.lon.ToString()))
                }).ToList()
            });

            //if (aroundCitys != null && aroundCitys.Count>0 && aroundCitys.FirstOrDefault().DistrictName.Trim() == "上海")
            //{
            //    result.UserDefinedList.Add(new UserDefinedCity() { Title = "上海近郊", Link = "http://www.zmjiudian.com/package/collection/20?userid={userid}&userlat={userlat}&userlng={userlng}&_newpage=1" });
            //}

            #region 行为记录

            try
            {
                var value = string.Format("{{\"districtName\":\"{0}\",\"lon\":\"{1}\",\"lat\":\"{2}\",\"districtID\":\"{3}\"}}", districtName, lon, lat, districtID);
                RecordBehavior("GetBoutiqueAndAroundCityList", value);
            }
            catch (Exception ex) { }

            #endregion

            return result;

        }

        #region 获取指定日期的包房特惠酒店
        [HttpGet]
        public List<int> GetPackRoomHotelIdList(DateTime date)
        {
            return PackageAdapter.GetPackRoomHotelIdList(date);
        }
        #endregion

        #region 专辑详情页
        [HttpGet]
        public PackageAlbumDetail GetPackageAlbumDetail(int albumId, bool isNeedNotSale = false)
        {
            var albumEntity = HotelAdapter.GetOnePackageAlbums(albumId);
            albumEntity.CoverPicSUrl = !string.IsNullOrWhiteSpace(albumEntity.CoverPicSUrl) ? PhotoAdapter.GenHotelPicUrl(albumEntity.CoverPicSUrl, Enums.AppPhotoSize.theme) : "";
            var regex = new Regex("_.*$", RegexOptions.Compiled | RegexOptions.RightToLeft);

            List<TopNPackageItem> packageList = HotelAdapter.GetTop20Package(100, albumId,isNeedNotSale:isNeedNotSale);

            packageList = packageList.Where(_ => _.PackagePrice.Exists(j => j.Type == 0) && _.PackagePrice.Exists(j => j.Type == -1)).ToList();

            return new PackageAlbumDetail()
            {
                albumEntity = albumEntity,
                packageList = packageList.Select(_ => new RecommendHotelItem()
                {
                    HotelID = _.HotelID,
                    HotelName = _.HotelName,
                    HotelPicUrl = _.PicUrls.Any() ? regex.Replace(_.PicUrls.First(), "_theme") : "",
                    HotelPrice = _.PackagePrice.First(j => j.Type == 0).Price,
                    VIPPrice = _.PackagePrice.First(j => j.Type == -1).Price,
                    TotalHotelPrice = _.PackagePrice.First(j => j.Type == 0).Price * _.DayLimitMin,
                    TotalVIPPrice = _.PackagePrice.First(j => j.Type == -1).Price * _.DayLimitMin,
                    DayLimitMin = _.DayLimitMin,
                    ADDescription = string.IsNullOrWhiteSpace(_.Title) ? "" : _.Title,
                    MarketPrice = _.MarketPrice,
                    PackageBrief = _.PackageBrief,
                    PID = _.PackageID,
                    RecommendPicUrl = string.IsNullOrEmpty(_.RecomendPicShortNames) ? "" : ("http://whphoto.b0.upaiyun.com/" + _.RecomendPicShortNames + "_theme"),
                    RecommendPicUrl2 = string.IsNullOrEmpty(_.RecomendPicShortNames2) ? "" : ("http://whphoto.b0.upaiyun.com/" + _.RecomendPicShortNames2 + "_theme"),
                    HotelReviewCount = _.HotelReviewCount,
                    HotelScore = _.HotelScore,
                    RecomemndWord = _.RecomemndWord,
                    RecomemndWord2 = _.RecomemndWord2,
                    PackageName = _.PackageName,
                    DistrictId = _.DistrictId,
                    DistrictName = _.DistrictName,
                    DistrictEName = _.DistrictEName,
                    ProvinceName = _.ProvinceName,
                    InChina = _.InChina
                }).ToList(),
                shareModel = new CommentShareModel()
                {
                    Content = albumEntity.SubTitle,
                    notHotelNameTitle = "",
                    title = albumEntity.SubTitle,
                    shareLink = "",
                    photoUrl = regex.Replace(albumEntity.CoverPicSUrl, "_290x290s"),
                    returnUrl = ""
                }
            };
        }
        /// <summary>
        /// 获取分组套餐专辑列表, Package.PackageGroupName相同且不为空，只取价格最低的套餐
        /// </summary>
        /// <param name="albumId"></param>
        /// <param name="startDistrictId"></param>
        /// <param name="isNeedNotSale"></param>
        /// <returns></returns>

        [HttpGet]
        public PackageAlbumDetail GetPackageGroupAlbumDetail(int albumId, int startDistrictId=0,int start=0, int count=100,bool isNeedNotSale = false)
        {
            var redisPackageAlbumDetail = HotelHelper.GetPackageAlbumDetailFromRedis(albumId, start, count, startDistrictId);
            if (redisPackageAlbumDetail == null)
            {
                redisPackageAlbumDetail = HotelAdapter.GetPackageAlbumDetailList(albumId, startDistrictId, count, isNeedNotSale);
            }
            var albumEntity = HotelAdapter.GetOnePackageAlbums(albumId);
            albumEntity.CoverPicSUrl = !string.IsNullOrWhiteSpace(albumEntity.CoverPicSUrl) ? PhotoAdapter.GenHotelPicUrl(albumEntity.CoverPicSUrl, Enums.AppPhotoSize.jupiter) : "";
            var regex = new Regex("_.*$", RegexOptions.Compiled | RegexOptions.RightToLeft);
            CommentShareModel shareModel = new CommentShareModel()
            {
                Content = albumEntity.SubTitle,
                notHotelNameTitle = "",
                title = albumEntity.SubTitle,
                shareLink = "",
                photoUrl = regex.Replace(albumEntity.CoverPicSUrl, "_290x290s"),
                returnUrl = ""
            };
            redisPackageAlbumDetail.albumEntity = albumEntity;
            redisPackageAlbumDetail.shareModel = shareModel;
            return redisPackageAlbumDetail;
        }

        /// <summary>
        /// 更新redis中套餐专辑
        /// </summary>
        /// <param name="albumId"></param>
        /// <returns></returns>
        [HttpGet]
        public void UpdateAlbumPackageRedis()
        {

            List<PackageAlbumsEntity> packageAlbums = HotelAdapter.GetAllPackageAlbums();
            foreach (var albumItem in packageAlbums)
            {
                var redisPackageAlbumDetail = HotelHelper.GetPackageAlbumDetailFromRedis(albumItem.ID, 0, 10000, 0);
                if (redisPackageAlbumDetail != null && redisPackageAlbumDetail.packageList != null)
                {
                    List<RecommendHotelItem> redisPackages = redisPackageAlbumDetail.packageList;
                    foreach (var item in redisPackages)
                    {
                        UpdateAlbumRedisCache(albumItem.ID, item.PID);
                    }
                }
            }

        } 



        #endregion

        #region 专辑列表
        /// <summary>
        /// 默认按照排名升序 id降序排
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        public List<PackageAlbumsEntity> GetPackageAlbumsEntityList(int page, int pageSize)
        {
            page = page == 0 ? 1 : page;
            TopNPackageSearchParam param = new TopNPackageSearchParam() { Filter = new TopNPackageSearchFilter() { IsValid = true }, Start = (page - 1) * pageSize, PageSize = pageSize, Sort = new Dictionary<string, string>() };
            int count = 0;
            List<PackageAlbumsEntity> packageAlbumsList = new List<PackageAlbumsEntity>();
            packageAlbumsList = HotelService.GetPackageAlbumsList(param, out count).Where(p => p.ID != 1 && p.ID != 10 && p.ID != 12 && p.ID != 37).ToList(); //周末去哪模块需要把专辑ID为1、10、12这个三个专辑 去掉 //37最新推荐专辑也从专辑列表中筛选掉，后面会单独在首页一个栏目显示 20170616 haoy
            packageAlbumsList = packageAlbumsList.Select(_ => new PackageAlbumsEntity
                {
                    Author = _.Author,
                    CoverPicSUrl = String.IsNullOrEmpty(_.CoverPicSUrl) ? "" : ("http://whphoto.b0.upaiyun.com/" + _.CoverPicSUrl + "_theme"),
                    Date = _.Date,
                    Description = _.Description,
                    GroupNo = _.GroupNo,
                    ICON = _.ICON,
                    ID = _.ID,
                    IsValid = _.IsValid,
                    LabelPicSUrl = _.LabelPicSUrl,
                    Name = _.Name,
                    Rank = _.Rank,
                    ShowSortNo = _.ShowSortNo,
                    SubTitle = _.SubTitle,
                    Type = _.Type
                }).ToList();
            return packageAlbumsList;
        }
        #endregion

        #region 可售地区酒店
        [HttpGet]
        public CanSellDistrictHotelResult GetCanSellDistrictCheapHotelList(HJD.HotelServices.Contracts.HotelServiceEnums.HotelDistrictRange range)
        {
            var result = new CanSellDistrictHotelResult()
            {
                data = new Dictionary<string, Dictionary<string, List<CanSaleHotelInfoEntity>>>()
            };

            var hotels = HotelAdapter.GetCanSellDistrictCheapHotelList(range);
            if (hotels != null && hotels.Any())
            {
                if (range == HotelServiceEnums.HotelDistrictRange.JZH)
                {

                    foreach (var provinceName in new string[] { "浙江", "江苏", "上海" })
                    {
                        result.data.Add(provinceName, hotels.FindAll(_ => _.Province.Contains(provinceName)).GroupBy(x => x.City).OrderBy(_ => _.Key)
                                    .ToDictionary(gdc => gdc.Key, gdc => gdc.Select(y => new CanSaleHotelInfoEntity()
                                    {
                                        Businessprice = y.Price,
                                        HotelId = y.HotelId,
                                        HotelName = y.HotelName,
                                        PackageBrief = y.Brief,
                                        DataPathName = "",
                                        DistrictID = 0
                                    }).OrderBy(_ => _.Businessprice).ToList()));
                    }
                }
                else
                {
                    foreach (var item in hotels.GroupBy(_ => _.Province).OrderBy(_ => _.Key))
                    {
                        result.data.Add(item.Key, item.GroupBy(x => x.City).OrderBy(_ => _.Key)
                                 .ToDictionary(gdc => gdc.Key, gdc => gdc.Select(y => new CanSaleHotelInfoEntity()
                                 {
                                     Businessprice = y.Price,
                                     HotelId = y.HotelId,
                                     HotelName = y.HotelName,
                                     PackageBrief = y.Brief,
                                     DataPathName = "",
                                     DistrictID = 0
                                 }).OrderBy(_ => _.Businessprice).ToList()));
                    }
                }
            }
            return result;
        }
        #endregion

        /// <summary>
        /// 获取单套餐详情
        /// </summary>
        /// <param name="pId"></param>
        /// <returns></returns>
        [HttpGet]
        public HJD.HotelServices.Contracts.PackageEntity GetOnePackageEntity(int pId)
        {
            return PackageAdapter.GetOnePackageEntity(pId);
        }

        #region 4.6版App 新发现首页

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userLat"></param>
        /// <param name="userLng"></param>
        /// <param name="geoScopeType"></param>
        /// <param name="districtid"></param>
        /// <param name="distance"></param>
        /// <param name="onlySelected"></param>
        /// <param name="districtName"></param>
        /// <returns></returns>
        [HttpGet]
        public InterestModel3 GetHomePageData40(float userLat, float userLng, int geoScopeType, int districtid = 2, string districtName = null, int needICONCount = 0)
        {
            try
            {
                var titlePrefix = string.IsNullOrWhiteSpace(districtName) || string.IsNullOrWhiteSpace(districtName.Trim()) ? "" : geoScopeType == 3 ? districtName + "及周边/" : districtName + "/";

                InterestModel3 im3 = new InterestModel3();
                im3.districtid = districtid;
                im3.Name = "";
                im3.GLat = userLat;
                im3.GLon = userLng;

                im3.InspectorList = new List<RecommendedInspectorModel>();
                im3.ThemeInterestList = new List<InterestEntity>();

                Task<List<PackageAlbumsEntity>> packageAlbumsTask = Task.Factory.StartNew<List<PackageAlbumsEntity>>(() =>
                {
                    return HotelAdapter.GetPackageAlbumByGeoInfo(districtid, userLat, userLng);
                });

                Task<InterestHotelResult> interestHotelResultTask = Task.Factory.StartNew<InterestHotelResult>(() =>
                {
                    return ResourceAdapter.GetInterest4AD(districtid, userLat, userLng, geoScopeType, true);
                });

                Task<List<InterestAlbumICON>> tempInterestDataCopyCacheTask = interestHotelResultTask.ContinueWith<List<InterestAlbumICON>>((obj) =>
                {
                    return interestHotelResultTask.Result.interests.FindAll(_ => _.HotelCount > 0 && _.Type == 1).Select(_ => new InterestAlbumICON()
                    {
                        ADDescription = _.ADDescription,
                        DistrictID = _.DistrictID,
                        GLat = _.GLat,
                        GLon = _.GLon,
                        HotelCount = _.HotelCount,
                        HotelID = _.HotelID,
                        HotelList = _.HotelList,
                        HotelName = _.HotelName,
                        HotelPrice = _.HotelPrice,
                        ID = _.ID,
                        ImageUrl = _.ImageUrl,
                        InterestPlaceIDs = _.InterestPlaceIDs,
                        LogoBGColor = _.LogoBGColor,
                        LogoURL = string.IsNullOrWhiteSpace(_.LogoURL) ? "" : _.LogoURL,
                        Name = _.Name,
                        Type = _.Type,
                        ActionUrl = string.Format("whotelapp://www.zmjiudian.com/strategy/place?interest={0}&title={1}&districtlat={2}&districtlng={3}&geoscopetype={4}&districtid={5}&districtName={6}", _.ID, HttpUtility.UrlEncode(titlePrefix + _.Name), userLat, userLng, geoScopeType, districtid, HttpUtility.UrlEncode(districtName))
                    }).ToList();
                });

                Task.WaitAll();

                if (im3.AD == null)
                {
                    im3.AD = new Advertise();
                }

                InterestHotelResult interestHotelResult = interestHotelResultTask.Result;
                im3.TotalHotelNum = interestHotelResult.totalHotelCount;//酒店总数量

                im3.TotalICONNum = 0;
                var isHaveParentChild = false;

                im3.ICONList = new List<InterestAlbumICON>();
                List<InterestAlbumICON> tempInterestList = tempInterestDataCopyCacheTask.Result;

                if (tempInterestList != null && tempInterestList.Count != 0)
                {
                    var interestList = tempInterestList.OrderByDescending(_ => _.HotelCount).ToList();
                    var childInterest = interestList.FirstOrDefault(_ => _.ID == 12);//先找亲子主题 如果没有则算了

                    if (childInterest != null)
                    {
                        isHaveParentChild = true;
                        im3.ICONList.Add(childInterest);
                    }
                    foreach (var item in interestList)
                    {
                        if (item.ID != 12)
                        {
                            im3.ICONList.Add(item);
                        }
                    }
                }

                var albums = packageAlbumsTask.Result;
                if (albums != null && albums.Any())
                {
                    //有亲子度假主题 永远第一个 其他专辑排后面
                    im3.ICONList.InsertRange(isHaveParentChild ? 1 : 0, albums.Select(_ => new InterestAlbumICON()
                    {
                        ADDescription = "",
                        DistrictID = 0,
                        GLat = 0.0,
                        GLon = 0.0,
                        HotelCount = 0,
                        HotelID = 0,
                        HotelList = "",
                        HotelName = "",
                        HotelPrice = 0,
                        ID = _.ID,
                        ImageUrl = "",
                        InterestPlaceIDs = "",
                        LogoBGColor = "",
                        LogoURL = string.IsNullOrWhiteSpace(_.ICON) ? "" : PhotoAdapter.GenHotelPicUrl(_.ICON, Enums.AppPhotoSize.jupiter),
                        Name = _.Name,
                        Type = 0,
                        ActionUrl = string.Format("whotelapp://www.zmjiudian.com/gotopage?url={0}",
                        System.Web.HttpUtility.UrlEncode(string.Format("http://www.zmjiudian.com/package/collection/{0}?userid={3}&userlat={1}&userlng={2}", _.ID, userLat, userLng, AppUserID)))
                    }));
                }

                im3.TotalICONNum = im3.ICONList.Count;//总数

                if (needICONCount > 0 && im3.TotalICONNum > needICONCount)
                {
                    im3.ICONList = im3.ICONList.Take(needICONCount).ToList();
                }

                return im3;
            }
            catch (Exception ex)
            {
                Log.WriteLog("捕获异常：" + ex.Message + ex.StackTrace);
                throw;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="userLat"></param>
        /// <param name="userLng"></param>
        /// <param name="geoScopeType"></param>
        /// <param name="districtid"></param>
        /// <param name="distance"></param>
        /// <param name="onlySelected"></param>
        /// <param name="districtName"></param>
        /// <param name="needICONCount"></param>
        /// <param name="loadHotelList">是否加载每个主题下的酒店列表 1true 0false</param>
        /// <returns></returns>
        [HttpGet]
        public InterestModel3 GetThemeInterestList(float userLat, float userLng, int geoScopeType, int districtid = 2, string districtName = null, int needICONCount = 0, int loadHotelList = 1)
        {
            try
            {
                var titlePrefix = string.IsNullOrWhiteSpace(districtName) || string.IsNullOrWhiteSpace(districtName.Trim()) ? "" : geoScopeType == 3 ? districtName + "及周边/" : districtName + "/";

                InterestModel3 im3 = new InterestModel3();
                im3.districtid = districtid;
                im3.Name = "";
                im3.GLat = userLat;
                im3.GLon = userLng;

                im3.InspectorList = new List<RecommendedInspectorModel>();
                im3.ThemeInterestList = new List<InterestEntity>();


                Task<InterestHotelResult> interestHotelResultTask = Task.Factory.StartNew<InterestHotelResult>(() =>
                {
                    return ResourceAdapter.GetInterest4AD(districtid, userLat, userLng, geoScopeType, true);
                });

                Task<List<InterestAlbumICON>> tempInterestDataCopyCacheTask = interestHotelResultTask.ContinueWith<List<InterestAlbumICON>>((obj) =>
                {
                    return interestHotelResultTask.Result.interests.FindAll(_ => _.HotelCount > 0 && _.Type == 1).Select(_ => new InterestAlbumICON()
                    {
                        ADDescription = _.ADDescription,
                        DistrictID = _.DistrictID,
                        GLat = _.GLat,
                        GLon = _.GLon,
                        HotelCount = _.HotelCount,
                        HotelID = _.HotelID,
                        HotelList = "",
                        HotelName = _.HotelName,
                        HotelPrice = _.HotelPrice,
                        ID = _.ID,
                        ImageUrl = _.ImageUrl,
                        InterestPlaceIDs = _.InterestPlaceIDs,
                        LogoBGColor = _.LogoBGColor,
                        LogoURL = string.IsNullOrWhiteSpace(_.LogoURL) ? "" : _.LogoURL,
                        Name = _.Name,
                        Type = _.Type,
                        Sort = _.Sort,
                        ActionUrl = string.Format("whotelapp://www.zmjiudian.com/strategy/place?interest={0}&title={1}&districtlat={2}&districtlng={3}&geoscopetype={4}&districtid={5}&districtName={6}", _.ID, HttpUtility.UrlEncode(titlePrefix + _.Name), userLat, userLng, geoScopeType, districtid, HttpUtility.UrlEncode(districtName))
                    }).ToList();
                });

                Task.WaitAll();

                if (im3.AD == null)
                {
                    im3.AD = new Advertise();
                }

                InterestHotelResult interestHotelResult = interestHotelResultTask.Result;
                im3.TotalHotelNum = interestHotelResult.totalHotelCount;//酒店总数量

                im3.TotalICONNum = 0;
                //var isHaveParentChild = false;

                im3.ICONList = new List<InterestAlbumICON>();
                List<InterestAlbumICON> tempInterestList = tempInterestDataCopyCacheTask.Result.OrderBy(_ => _.Sort).ToList();

                if (tempInterestList != null && tempInterestList.Count != 0)
                {
                    //去掉按照主题下面酒店数量排序。sql中按照 sort排序
                    //var interestList = tempInterestList.OrderByDescending(_ => _.HotelCount).ToList();
                    //var childInterest = interestList.FirstOrDefault(_ => _.ID == 12);//先找亲子主题 如果没有则算了

                    //if (childInterest != null)
                    //{
                    //    childInterest.HotelListResult = (loadHotelList == 0 ? new SearchHotelListResult() : SearchHotelList30Cache(new HotelListQueryParam20 { aroundCityId = (geoScopeType == 3 ? districtid : 0), districtid = districtid, start = 0, count = 6, lat = userLat, lng = userLng, geoScopeType = geoScopeType, interest = childInterest.ID }));

                    //    //isHaveParentChild = true;
                    //    im3.ICONList.Add(childInterest);
                    //}

                    foreach (var item in tempInterestList)
                    {

                        item.HotelListResult = (loadHotelList == 0 ? new SearchHotelListResult() : SearchHotelList30Cache(new HotelListQueryParam20 { aroundCityId = (geoScopeType == 3 ? districtid : 0), districtid = districtid, start = 0, count = 6, lat = userLat, lng = userLng, geoScopeType = geoScopeType, interest = item.ID }));

                        im3.ICONList.Add(item);
                        //if (item.ID != 12)
                        //{
                        //    item.HotelListResult = (loadHotelList == 0 ? new SearchHotelListResult() : SearchHotelList30Cache(new HotelListQueryParam20 { aroundCityId = (geoScopeType == 3 ? districtid : 0), districtid = districtid, start = 0, count = 6, lat = userLat, lng = userLng, geoScopeType = geoScopeType, interest = item.ID }));

                        //    im3.ICONList.Add(item);
                        //}
                    }
                }

                im3.TotalICONNum = im3.ICONList.Count;//总数

                if (needICONCount > 0 && im3.TotalICONNum > needICONCount)
                {
                    im3.ICONList = im3.ICONList.Take(needICONCount).ToList();
                }

                return im3;
            }
            catch (Exception ex)
            {
                Log.WriteLog("捕获异常：" + ex.Message + ex.StackTrace);
                throw;
            }
        }

        [HttpGet]
        public RecommendHotelResult GetHotRecommendHotel([FromUri]BsicSearchParam param)
        {
            return HotelAdapter.GetHotRecommendHotel(param);
        }

        #endregion

        #region 4.7版本App 个性化首页
        /// <summary>
        /// 推荐酒店列表
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpGet]
        public RecommendHotelResult GetRecommendHotelListByFollowing([FromUri]RecommendCommentParam param)
        {
            return CommentAdapter.GetRecommendHotelListByFollowing(param);
        }

        /// <summary>
        /// 酒店浏览记录
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpGet]
        public RecommendHotelResult GetHotelBrowsingRecordList([FromUri]BsicSearchParam param)
        {
            return HotelAdapter.GetHotelBrowsingRecordList(param);
        }

        /// <summary>
        /// 浏览记录 包括酒店和sku
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        //[HttpGet]
        //public BrowsingRecordResult GetBrowsingRecordList([FromUri]BsicSearchParam param)
        //{
        //    return HotelAdapter.GetBrowsingRecordList(param);
        //}
        #region 获取300公里之内酒店（好去处）

        [HttpGet]
        public RecommendHotelResult GetHotelWithInDistance(int count, int start, float lat = 0, float lng = 0)
        {
            DestController dest = new DestController();
            SimpleCityInfo CityInfo = dest.GetCityInfoByName("", lat, lng);
            HotelListQueryParam20 param = new HotelListQueryParam20();
            param.districtid = CityInfo.DistrictId;
            param.districtName = CityInfo.Name;
            param.geoScopeType = 3;
            param.start = start;
            param.count = count;
            param.lat = lat;
            param.districtLat = lat;
            param.lng = lng;
            param.districtLng = lng;
            param.sType = "ios";
            param.sort = 20;
            param.JustMinPricePlan = true;

            SearchHotelListResult searchHotel = SearchHotelList30(param);
            RecommendHotelResult RecommendHotel = new RecommendHotelResult();
            try
            {
                if (searchHotel != null && searchHotel.Result20.Count > 0)
                {
                    foreach (ListHotelItemV43 item in searchHotel.Result20)
                    {
                        RecommendHotelItem Recoment = new RecommendHotelItem();
                        HotelItem hi = ResourceAdapter.GetHotel(item.Id, UserId);
                        Recoment.HotelID = item.Id;
                        Recoment.HotelName = item.Name;
                        Recoment.HotelPrice = Convert.ToInt32(item.MinPrice);
                        Recoment.VIPPrice = Convert.ToInt32(item.VIPPrice);
                        Recoment.HotelPicUrl = item.PictureList.FirstOrDefault();
                        Recoment.PackageBrief = item.PackageBrief;
                        Recoment.PID = item.PackageId;
                        Recoment.HotelScore = hi.Score;
                        Recoment.HotelReviewCount = hi.ReviewCount;
                        RecommendHotel.HotelList.Add(Recoment);
                    }
                    RecommendHotel.HotelTotalCount = searchHotel.TotalCount;
                }
            }
            catch (Exception e)
            {
                Log.WriteLog("周边300公里内" + e);
            }

            RecommendHotel.DistrictName = CityInfo.Name;
            RecommendHotel.HotelBlockTitle = "周边300公里内";


            return RecommendHotel;
            //return RecommendHotelResult HotelAdapter.GetHotelWithInDistance(lat, lng, count, start);
        }
        #endregion

        /// <summary>
        /// Html5页面显示最近搜索记录
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpGet]
        public RecommendHotelResult GetSearchRecordList([FromUri]CommonRecordQueryParam param)
        {
            return HotelAdapter.GetSearchRecordList(param);
        }

        /// <summary>
        /// App原生显示最近搜索记录
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpGet]
        public List<QuickSearchSuggestItem> GetSearchRecordList4App([FromUri]CommonRecordQueryParam param)
        {
            var result = new List<QuickSearchSuggestItem>();
            param.count = param.count == 0 ? 5 : param.count;//App count 等于0 默认为5

            var cityHistory = HotelAdapter.GetSearchRecordList(param, _ContextBasicInfo);

            if (cityHistory != null && cityHistory.HotelList != null && cityHistory.HotelList.Any())
            {
                result = cityHistory.HotelList.Select(_ => new QuickSearchSuggestItem()
                {
                    ActionUrl = _.ActionURL,
                    EName = "",
                    Icon = Configs.searchIcon4Old,
                    Id = _.HotelID,
                    Name = string.IsNullOrWhiteSpace(_.HotelName) ? "" : _.HotelName.Split(",，".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)[0],
                    ParentName = "",
                    Tag = "",
                    Type = "D",
                    Lat = _.Lat,
                    Lon = _.Lon,
                    GeoScopeType = 1
                }).ToList();
            }

            #region 行为记录

            try
            {
                var value = string.Format("{{\"ParamCount\":\"{0}\",\"ResultCount\":\"{1}\"}}", param.count, (result != null ? result.Count : 0));
                RecordBehavior("GetSearchRecordList4App", value);
            }
            catch (Exception ex) { }

            #endregion

            return result;
        }

        #endregion

        #region WHForHtml5 调用接口

        /// <summary>
        /// 获取指定日期的套餐价格
        /// </summary>
        /// <param name="hotelid"></param>
        /// <param name="pid"></param>
        /// <param name="checkIn"></param>
        /// <param name="checkOut"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        [HttpGet]
        public Dictionary<string, string> GetPackageCalendarPrice(int hotelid, int pid, string checkIn, string checkOut, int userid = 0)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();

            var checkInDate = DateTime.Now.Date;
            var checkOutDate = checkInDate.AddDays(1);
            try
            {
                checkInDate = DateTime.Parse(checkIn);
                checkOutDate = DateTime.Parse(checkOut);
            }
            catch (Exception ex)
            {

            }

            var price = 0;
            var vipPrice = 0;
            var nightCount = 1;

            //首先获取当前套餐的所有日期下的价格数据
            var calendar = new PriceController().GetOneHotelPackageCalendar(hotelid, DateTime.Now.Date.ToString("yyyy-MM-dd"), pid, userid);
            if (calendar != null && calendar.DayItems != null && calendar.DayItems.Count > 0)
            {
                var selCalendar = calendar.DayItems.Where(c => c.Day >= checkInDate && c.Day < checkOutDate).ToList();
                price = selCalendar.Sum(c => c.SellPrice);
                vipPrice = selCalendar.Sum(c => c.VipPrice);
                nightCount = selCalendar.Count;
            }

            dic["price"] = price.ToString();
            dic["vipPrice"] = vipPrice.ToString();
            dic["tip"] = string.Format("已选{0}月{1}号入住{2}月{3}日离店共{4}天", checkInDate.Month, checkInDate.Day, checkOutDate.Month, checkOutDate.Day, nightCount);
            dic["days"] = nightCount.ToString();

            return dic;
        }

        #endregion

        [HttpGet]
        public ActiveRuleGroupEntity GetActiveRuleList(int id)
        {
            return HotelAdapter.GetActiveRuleList(id);
        }

        /// <summary>
        /// 【直接触发&没有缓存时间】触发更新指定酒店指定日期的列表价队列任务
        /// </summary>
        /// <param name="hotelId"></param>
        /// <param name="checkIn"></param>
        /// <returns></returns>
        [HttpGet]
        public bool QuickPublishPriceSlotTask(int hotelId, DateTime checkIn)
        {
            HotelAdapter.PublishUpdatePriceSlotTask(hotelId, checkIn.ToString("yyyy-MM-dd"));
            return true;
        }

        [HttpGet]
         public   List<ChannelInfoEntity> GetAllChannelInfoList()
        {
            return HotelAdapter.GetAllChannelInfoList();
        }

        /// <summary>
        /// 分销酒店接口
        /// </summary>
        /// <param name="type">套餐类型 0：酒店 1：机酒 2：游轮</param>
        /// <param name="sort">排序规则</param>
        /// <param name="searchWord">搜索</param>
        /// <param name="start"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        [HttpPost]
        public RetailHotelEntity GetRetailHotelList(HJD.CouponService.Contracts.Entity.SearchProductRequestEntity param)
        {

            RetailHotelEntity RetailHotelList = new RetailHotelEntity();
            int type = param.Screen.ProductType.Count > 0 ? param.Screen.ProductType.First() : 0;
            RetailHotelList = HotelAdapter.GetRetailHotelList(type, param.Sort, param.SearchWord, param.Start, param.Count);
            List<int> hotelIdList = RetailHotelList.list.Select(_ => _.HotelId).ToList();
            var hotelPicsList = HotelAdapter.GetManyHotelPhotos(hotelIdList);
            foreach (RetailHotel item in RetailHotelList.list)
            {
                item.HotelPic = (hotelPicsList.FirstOrDefault(p => p.HotelId == item.HotelId) != null && hotelPicsList.First(p => p.HotelId == item.HotelId).HPList.Any()) ? PhotoAdapter.GenHotelPicUrl(hotelPicsList.First(p => p.HotelId == item.HotelId).HPList[0].SURL, Enums.AppPhotoSize.theme) : "";
            }

            RetailHotelList.ParamType = param.Screen.ProductType;
            return RetailHotelList;
            //return HotelAdapter.GetRetailHotelList(type, searchWord, start, count);
        }

        [HttpGet]
        public RetailHotelInfoEntity GetRetailHotelDetail(int hotelId)
        {
            RetailHotelInfoEntity result = HotelAdapter.GetRetailPackageList(hotelId);


            HotelPhotosEntity hps = HotelAdapter.GetHotelPhotos(result.HotelId, 0);
            result.HotelPicList = hps.HPList.Take(5).Select(p => PhotoAdapter.GenHotelPicUrl(p.SURL, Enums.AppPhotoSize.appdetail)).ToList();
            
            foreach (RetailPackageEntity item in result.RetailPackageList)
            {
                if (item.NightCount > 1)
                {
                    int _totalPrice = 0;
                    int _totalVipPrice = 0;
                    decimal _totalMamuCommission = 0m;
                    decimal _totalAutoCommission = 0m;
                    DateTime startTime = item.CheckIn;
                    PriceAdapter.GetManyDaysPackagePriceCached(result.HotelId, item.PID, Enums.CustomerType.vip, item.NightCount, out _totalPrice, out _totalVipPrice, out startTime, out _totalMamuCommission, out _totalAutoCommission);

                    if (_totalPrice <= 0) 
                    { 
                        item.NotVipPrice = item.NotVipPrice * item.NightCount;
                        item.CheckIn = item.CheckIn;
                        item.CheckOut = item.CheckIn.AddDays(item.NightCount);
                    }
                    else {
                        item.NotVipPrice = _totalPrice;
                        item.CheckIn = startTime;
                        item.CheckOut = item.CheckIn.AddDays(item.NightCount);
                    }
                    if (_totalVipPrice <= 0) { item.VipPrice = item.VipPrice * item.NightCount; }
                    else { item.VipPrice = _totalVipPrice; }

                    if (_totalAutoCommission == 0m) { item.AutoCommission = item.AutoCommission * item.NightCount; }
                    else { item.AutoCommission = _totalAutoCommission; }
                    if (_totalMamuCommission == 0m) { item.ManualCommission = item.ManualCommission * item.NightCount; }
                    else { item.ManualCommission = _totalMamuCommission; }

                }
            }
            List<Hotel3Entity> h3s = HotelService.GetHotel3(hotelId);
            result.Intro = new CommentTextAndUrlEx();
            foreach (Hotel3Entity h3 in h3s)
            {
                switch (h3.Type)
                {
                    case 1://推荐理由
                        HotelInfo his = new HotelInfo();
                        his = HotelAdapter.GenHotel3Info(h3);
                        if (!string.IsNullOrEmpty(his.Items[0].content))
                        {
                            result.Intro.Description = his.Items[0].content;
                            foreach (var item in his.Items[0].content.Split("\r\n".ToCharArray()))
                            {
                                result.Intro.Item.Add(item);
                            }
                        }
                        break;
                        #region 图文连接取hotelcontact表中的ActivePageId
                        //case 9: //微信分享全文链接
                        //case 10: //微信链接为空时 取本地全文链接
                        //    Hotel3Entity h9 = h3s.Where(_ => _.Type == 9).FirstOrDefault();
                        //    if (h9 != null)
                        //    {
                        //        HotelInfo hi9 = HotelAdapter.GenHotel3Info(h9);

                        //        if (hi9.Items.Count > 0)
                        //        {
                        //            string strUrl9 = hi9.Items[0].content.Contains("#") == true ? (hi9.Items[0].content.Insert(hi9.Items[0].content.LastIndexOf("#"), "&_isshare=1")) : (hi9.Items[0].content + "&_isshare=1");
                        //            result.Intro.ActionUrl = hi9.Items.Count == 0 ? "" : strUrl9;
                        //            result.Intro.Text = "全文";
                        //        }
                        //        else
                        //        {
                        //            HotelInfo h = new HotelInfo();
                        //            h = HotelAdapter.GenHotel3Info(h3);
                        //            result.Intro.ActionUrl = h.Items.Count == 0 ? "" : ("http://www.zmjiudian.com/active/activepage?midx=" + h.Items[0].content + "&_isshare=1");
                        //            result.Intro.Text = "全文";
                        //        }
                        //    }
                        //    else
                        //    {
                        //        HotelInfo h = new HotelInfo();
                        //        h = HotelAdapter.GenHotel3Info(h3);
                        //        result.Intro.ActionUrl = h.Items.Count == 0 ? "" : ("http://www.zmjiudian.com/active/activepage?midx=" + h.Items[0].content + "&_isshare=1");
                        //        result.Intro.Text = "全文";
                        //    }
                        //    break; 
                        #endregion

                }

            }
            //获取微信图文
            HJD.HotelServices.Contracts.HotelContacts hc = HotelService.GetHotelContacts(hotelId);
            if (hc != null & hc.ActivePageId > 0)
            {
                result.Intro.ActionUrl = "http://www.zmjiudian.com/active/activepage?pageid=" + hc.ActivePageId + "&_isshare=1";
                result.Intro.Text = "全文";
            }
            return result;
        }

        [HttpGet]
        public RetailPackageEntity GetRetailPackageInfo(DateTime checkIn, DateTime checkOut, int pid, long cid)
        {
            if (checkIn < DateTime.Now)
            {
                checkIn = DateTime.Now.AddDays(1);
                checkOut = checkOut.AddDays(1);
            }
            RetailPackageEntity result = HotelAdapter.GetRetailProductByID(checkIn, checkOut, pid, cid);
            HotelPhotosEntity hps = HotelAdapter.GetHotelPhotos(result.HotelId, 0);
            result.HotelPicList = hps.HPList.Take(5).Select(p => PhotoAdapter.GenHotelPicUrl(p.SURL, Enums.AppPhotoSize.appdetail)).ToList();
            return result;
        }

        /// <summary>
        /// 根据分组号获取专辑列表
        /// </summary>
        /// <param name="groupNo"></param>
        /// <returns></returns>
        [HttpGet]
        public List<PackageAlbumsEntity> GetPackageAlbumsByGroupNo(string groupNo)
        {
            return HotelAdapter.GetPackageAlbumsByGroupNo(groupNo);
        }
        /// <summary>
        /// 更新缓存
        /// </summary>
        /// <param name="albumId"></param>
        /// <param name="pid"></param>
        /// <returns></returns>
        [HttpGet]
        public bool UpdateAlbumRedisCache(int albumId, int pid)
        {
            return HotelHelper.UpdateAlbumRedisCache(albumId,pid);
        }
        /// <summary>
        /// 设置过了售卖时间套餐下线 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public bool OfflineOverTimePackage()
        {
            return HotelAdapter.OfflineOverTimePackage();
        }

    }

}