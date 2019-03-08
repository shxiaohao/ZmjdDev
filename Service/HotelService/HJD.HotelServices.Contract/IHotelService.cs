using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using HJD.HotelServices.Contracts;
using HJD.HotelServices.Contracts.Comments;
//using Hotel.Supplier.WebServiceEntity.HotelReview.Entity;

namespace HJD.HotelServices.Contracts
{
    /// <summary>
    /// 酒店服务接口
    /// </summary>
    [ServiceContract(Namespace = "http://www.zmjiudian.com/")]
    public interface IHotelService
    {


        [System.ServiceModel.OperationContractAttribute()]
        bool ClearHotelCache(HJD.HotelServices.Contracts.HotelServiceEnums.HotelCacheType hotelCacheType, List<int> hotelIDlist);

        /// <summary>
        /// 获取酒店对应ota
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="otaHotelID"></param>
        /// <param name="sourcePageType"></param>
        /// <param name="sType"></param>
        /// <returns></returns>
        [System.ServiceModel.OperationContractAttribute()]
        string GetOtaHotelUrl(string channel, int otaHotelID, string sourcePageType, string sType);

        /// <summary>
        /// 获取所有OTA
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="otaHotelID"></param>
        /// <param name="sourcePageType"></param>
        /// <param name="sType"></param>
        /// <returns></returns>
        [System.ServiceModel.OperationContractAttribute()]
        List<HotelOTAEntity> GetOtaListByHotelID(int HotelID);

        [System.ServiceModel.OperationContractAttribute()]
        Dictionary<string, string> GetOtaUrlListByHotelID(int HotelID, string sourcePageType, string sType);
        /// <summary>
        /// 新增出行人信息
        /// </summary>
        /// <param name="travelperson"></param>
        /// <returns></returns>
        [System.ServiceModel.OperationContractAttribute()]
        int AddTravelPerson(TravelPersonEntity travelperson);

        /// <summary>
        /// 修改出行人信息
        /// </summary>
        /// <param name="travelperson"></param>
        /// <returns></returns>
        [System.ServiceModel.OperationContractAttribute()]
        int UpdateTravelPerson(TravelPersonEntity travelperson);

        /// <summary>
        /// 根据id获取出行人信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [System.ServiceModel.OperationContractAttribute()]
        TravelPersonEntity GetTravelPersonById(int id);


        [OperationContract]
        List<TravelPersonEntity> GetBookUserDateInfoByExchangCouponId(int ExchangCouponId);
        /// <summary>
        /// 根据用户Id获取出行人信息
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <returns></returns>
        [System.ServiceModel.OperationContractAttribute()]
        List<TravelPersonEntity> GetTravelPersonByUserId(long userId);


        /// <summary>
        /// 获取出行人信息
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <returns></returns>
        [System.ServiceModel.OperationContractAttribute()]
        List<TravelPersonEntity> GetTravelPersonByIds(string Ids);

        /// <summary>
        /// 删除出行人，更改static=2
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [System.ServiceModel.OperationContractAttribute()]
        bool DeleteTravelPerson(int id);

        /// <summary>
        /// 获取所有销售渠道信息
        /// </summary>
        /// <returns></returns>
        [System.ServiceModel.OperationContractAttribute()]
        List<ChannelInfoEntity> GetAllChannelInfoList();


        /// <summary>
        /// 返回酒店最优先展示的套餐信息
        /// </summary>
        /// <param name="HotelIDList">酒店列表</param>
        /// <param name="CheckIn">入住日期</param>
        /// <param name="CheckOut">离开日期</param>
        /// <returns></returns>
        [OperationContract]
        List<HotelTop1PackageInfoEntity> GetHotelTop1PackageInfo(IEnumerable<int> HotelIDList, DateTime CheckIn, DateTime CheckOut);

        /// <summary>
        /// 酒店套餐价格，按供应商出
        /// </summary>
        /// <param name="hotelid"></param>
        /// <param name="CheckIn"></param>
        /// <param name="CheckOut"></param>
        /// <returns></returns>
        [OperationContract]
        List<PackageRateEntity> GetHotelPackageRateList(int hotelid, DateTime CheckIn, DateTime CheckOut, HJD.HotelServices.Contracts.HotelServiceEnums.PricePolicyType ppt = HJD.HotelServices.Contracts.HotelServiceEnums.PricePolicyType.Default, int pid = 0);

        [OperationContract]
        PackageRateEntity GetCtripHotelPackageRateList(int hotelid, int pid, HJD.HotelServices.Contracts.HotelServiceEnums.PackageType packageType, DateTime CheckIn, DateTime CheckOut, HJD.HotelServices.Contracts.HotelServiceEnums.PricePolicyType ppt);

        [OperationContract]
        bool PublishUpdatePriceSlotTask(int hotelid, DateTime checkIn);

        [OperationContract]
        bool QuickPublishPriceSlotTask(int hotelid, DateTime checkIn);

        [OperationContract]
        bool QuickPublishPriceSlotTaskForBg(int hotelid, DateTime checkIn);

        [OperationContract]
        bool PublishUpdateMultiPriceSlotTask(List<int> hotellist, DateTime checkIn);

        [OperationContract]
        bool UpdateInspectorHotelOrder(InspectorRefHotel ihr);

        /// <summary>
        /// 获得评鉴酒店列表(App显示或后台显示)
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <returns></returns>
        [OperationContract]
        List<InspectorHotel> GetInspectorHotelList(InspectorHotelSearchParam param, out int count);

        [OperationContract]
        int InsertOrUpdateInspectorHotel(InspectorHotel eh);

        [OperationContract]
        int InsertOrUpdateInspectorRefHotel(InspectorRefHotel eh);

        [OperationContract]
        int UpdateInspectorRefHotel(int state, int vid, int hvid);

        [OperationContract]
        int UpdateInspectorRefHotel4Comment(int ehID, int commentID);

        [OperationContract]
        int UpdateInspectorRefHotel4NoticeComment(int ehID, bool hasSendWriteComment);

        [OperationContract]
        List<InspectorRefHotel> GetInspectorRefHotelList(long evaHotelID, long userID, int hvid = 0);
        #region 房券
        /// <summary>
        /// 得到房券数量和列表
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        [System.ServiceModel.OperationContractAttribute()]
        List<HotelVoucherEntity> GetUseHotelVoucher(out int count);
        /// <summary>
        /// 同一个房券是否参加过兑换
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="hvid">房券id</param>
        /// <returns></returns>
        [System.ServiceModel.OperationContractAttribute()]
        List<VoucherEntity> GetUseVoucherList(long userid, int hvid);

        /// <summary>
        /// 查询房券信息
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [System.ServiceModel.OperationContractAttribute()]
        List<HotelVoucherEntity> GetHotelVoucherByID(int ID);

        [OperationContract]
        List<InspectorRefHotel> GetInspectorRefHotelListByHVID(int hvid, long userID);
        #endregion

        [OperationContract]
        List<InspectorRefHotel> GetInspectorHotelOrderList(int OrderState, int lastID, int pageSize);

        //[OperationContract]
        //int AddInspectorRefHotel(long id, long userId, int hotelID, DateTime checkIn, DateTime checkOut);

        /// <summary>
        /// 申请品鉴酒店
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [OperationContract]
        int BookInspectorRefHotelEx(InspectorRefHotel data);

        /// <summary>
        /// 申请房券
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [OperationContract]
        string BookVoucherInspectorRefHotelEx(InspectorRefHotel data);

        [OperationContract]
        int CheckInspectorRefHotel(InspectorRefHotel eh);

        [OperationContract]
        int CheckInspectorRefHotelEx(InspectorRefHotel eh, HJD.HotelServices.Contracts.HotelServiceEnums.ConsumeUserPointsBizType typeID);

        /// <summary>
        /// 更新品鉴酒店的审核状态
        /// </summary>
        /// <param name="eh"></param>
        /// <returns></returns>
        [OperationContract]
        int CheckInspectorHotelApply(InspectorRefHotel eh);

        /// <summary>
        /// 获得个人预订酒店数量
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="state"></param>
        /// <param name="isDistinct"></param>
        /// <returns></returns>
        [OperationContract]
        int GetOrderHotelCountByUserId(long userId, int state, bool isDistinct);

        /// <summary>
        /// 获得个人酒店点评数量
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="isDistinct"></param>
        /// <returns></returns>
        [OperationContract]
        int GetCommentHotelCountByUserId(long userId, bool isDistinct);

        [OperationContract]
        InspectorRefHotel GetInspectorRefHotelById(long id);

        [OperationContract]
        InspectorHotel GetInspectorHotelById(long id);

        [OperationContract]
        List<HotelOTAEntity> GetHotelOTAByCreateTime(DateTime CreateTime);

        [OperationContract]
        List<HotelTagInfoEntity> GetHotelShowTags(int hotelID);

        [OperationContract]
        List<HotelTagInfoEntity> GetHotelInterestTags(int hotelID);

        [OperationContract]
        bool RefreshHotelPhotos(int hotelID);

        [OperationContract]
        List<FeaturedCommentEntity> GetHotelFeaturedCommentInfo(int hotelid);

        [OperationContract]
        List<int> GetHotelFeaturedInfo(int hotelid);

        [OperationContract]
        List<SpecialDealPackageEntity> GetSpecialDealPackage();

        [OperationContract]
        List<CanSaleHotelInfoEntity> GetAllCanSellPackage(DateTime startDate, DateTime endDate, string tag = "");

        [OperationContract]
        bool CreateAllCanSellPackage(DateTime startDate, DateTime endDate, string tag = "");


        [OperationContract]
        List<RoomSouldCountEntity> GetHotelRoomSouldInfo(int hotelid, DateTime checkIn, DateTime checkOut);

        [OperationContract]
        List<Int32> GetHotelInterestIDs(int hotelID);

        [OperationContract]
        List<int> GetPackageHotelList();

        [OperationContract]
        List<AttractionCategoryRelEntity> GetAttractionCategoryRel();

        [OperationContract]
        List<int> GetPackagedInterestPlace();

        [OperationContract]
        InterestQueryEntity QueryInterestAndHotelCount(int districtid, float lat, float lng, int distance = 300000);

        /// <summary>
        /// 非精选 某地区所有酒店数量
        /// </summary>
        /// <param name="districtid"></param>
        /// <param name="lat"></param>
        /// <param name="lng"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        [OperationContract]
        int QueryInterestHotelCount(int districtid, float lat, float lng, int distance = 300000);

        /// <summary>
        /// 精选 某地区所有酒店数量
        /// </summary>
        /// <param name="districtid"></param>
        /// <param name="lat"></param>
        /// <param name="lng"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        [OperationContract]
        int QueryInterestHotelCountSelected(int districtid, float lat, float lng, int distance = 300000);

        [OperationContract]
        List<PackageInfoEntity> GetHotelPackageList(int hotelid, DateTime CheckIn, DateTime CheckOut, int terminalType = 1, HJD.HotelServices.Contracts.HotelServiceEnums.PricePolicyType ppt = HJD.HotelServices.Contracts.HotelServiceEnums.PricePolicyType.Default, int pid = 0);



        [OperationContract]
        List<PackageInfoEntity> GetHotelPackageListInfo(int hotelid, DateTime CheckIn, DateTime CheckOut, int terminalType = 1, HJD.HotelServices.Contracts.HotelServiceEnums.PricePolicyType ppt = HJD.HotelServices.Contracts.HotelServiceEnums.PricePolicyType.Default, int pid = 0);

        /// <summary>
        /// 根据packageid获取关联VIP套餐
        /// </summary>
        /// <param name="packageId"></param>
        /// <returns></returns>
        [OperationContract]
        PackageInfoEntity GetFirstVipPackageByPackageId(int pid, DateTime checkIn, DateTime checkOut);

        /// <summary>
        /// 获得酒店某一个符合条件的套餐，不存在返回null
        /// </summary>
        /// <param name="hotelid"></param>
        /// <param name="code"></param>
        /// <param name="pid"></param>
        /// <returns></returns>
        [OperationContract]
        PackageInfoEntity GetHotelPackageByCode(int hotelid, string code, int pid);

        [OperationContract]
        void BindUserAccountAndOrders(long userid, string phone);

        [OperationContract]
        List<SimplePackageEntity> GetSimplePackageInfo(string packageIDs);

        [OperationContract]
        List<FilterDicEntity> GetHotelFilters(HotelSearchParas p);

        [OperationContract]
        List<InterestEntity2> GetAllInterestList();

        [OperationContract]
        PackageEntity GetSharePckageInfoByPrice(int hotelid, int price);

        [OperationContract]
        List<HotelSightEntity> GetHotelSightList(int hotelid);

        [OperationContract]
        List<HotelRestaurantEntity> GetHotelRestaurantList(int hotelid);

        [OperationContract]
        List<PDayItem> GetHotelPackageCalendar(int hotelid, DateTime startDate, HJD.HotelServices.Contracts.HotelServiceEnums.PricePolicyType ppt = HJD.HotelServices.Contracts.HotelServiceEnums.PricePolicyType.Default);

        [OperationContract]
        List<PDayItem> GetJLHotelPackageCalendar(int hotelid, DateTime startDate, HJD.HotelServices.Contracts.HotelServiceEnums.PricePolicyType ppt = HJD.HotelServices.Contracts.HotelServiceEnums.PricePolicyType.Default);

        [OperationContract]
        List<PDayItem> GetHotelPackageCalendar30(int hotelid, DateTime startDate, int pid = 0, int channelId = 0, HJD.HotelServices.Contracts.HotelServiceEnums.PricePolicyType ppt = HJD.HotelServices.Contracts.HotelServiceEnums.PricePolicyType.Default);

        [OperationContract]
        List<PDayItem> GetHotelPackageCalendar30Cached(int hotelid, DateTime startDate, int pid = 0, int channelId = 0, HJD.HotelServices.Contracts.HotelServiceEnums.PricePolicyType ppt = HJD.HotelServices.Contracts.HotelServiceEnums.PricePolicyType.Default);

        [OperationContract]
        List<PDayItem> GenHotelVoucherCalendar(int hvid, DateTime startDate, DateTime endDate);

        [OperationContract]
        List<HotelInterestTagEntity> GetHotelInterestTag(int hotelID, int interestID);


        [OperationContract]
        List<RoomInfoEntity> GetRoomInfo(int hotelid);

        [OperationContract]
        HotelContactPackageEntity GetHotelContactPackageByHotelIDAndPid(int hotelid, int pid);

        [OperationContract]
        List<PackageInfoEntity> GetHotelPackages(int hotelid, int pid = 0, int terminalType = 1);

        [OperationContract]
        List<PackageInfoEntity> GetTopTownPackages(int hotelid, DateTime checkIn, DateTime checkOut, HotelServiceEnums.PricePolicyType ppt = HotelServiceEnums.PricePolicyType.Default);

        [OperationContract]
        List<PackageInfoEntity> GetJLHotelPackages(int hotelid, DateTime CheckIn, DateTime CheckOut, HJD.HotelServices.Contracts.HotelServiceEnums.PricePolicyType ppt = HJD.HotelServices.Contracts.HotelServiceEnums.PricePolicyType.Default);

        [OperationContract]
        List<Hotel3Entity> GetHotel3(int hotelid);

        [OperationContract]
        List<CityEntity> GetZMJDCityData();

        [OperationContract]
        List<CityEntity> GetZMJDSelectedCityData();

        [OperationContract]
        List<CityEntity> GetZMJDAllCityData();

        [OperationContract]
        List<CityEntity> GetZMJDLoveCityData();

        [OperationContract]
        List<SimpleHotelItem> GetSimpleHotelCore(List<int> hotelIDs);

        [OperationContract]
        List<InterestEntity> QueryInterest(int districtid, float lat, float lng, int distance = 300000);

        [OperationContract]
        List<InterestEntity> QueryInterest4AD(int districtid, float glat, float glon, int distance = 300000);

        [OperationContract]
        List<InterestEntity> QueryInterest4ADSelected(int districtid, float glat, float glon, int distance = 300000);

        [OperationContract]
        bool UpdateHotelInfo(HotelInfoEditEntity hi);

        //[OperationContract]
        //List<HotelTFTReviewEntity> GetHotelTFTReview(int hotelID);

        [OperationContract]
        List<HotelTFTRelItemEntity> GetHotelTFTRel(int hotelID);

        [OperationContract]
        List<HotelThemeEntity> GetAllHotelTheme();


        [OperationContract]
        List<HotelThemeEntity> GetHotelTheme(int districtid, float lat, float lng);


        [OperationContract]
        List<OTACodeEntity> GetOTAHotelCode(int channelid, List<int> otahotelid);

        /// <summary>
        /// 获取特色列表
        /// </summary>
        [OperationContract]
        List<FeaturedEntity> GetFeaturedList();

        /// <summary>
        /// 获取酒店照片列表
        /// </summary>
        [OperationContract]
        HotelPhotosEntity GetHotelPhotos(int hotelID);

        /// <summary>
        /// 批量获取酒店照片列表
        /// </summary>
        [OperationContract]
        List<HotelPhotosEntity> GetManyHotelPhotos(IEnumerable<int> hotelIds);

        /// <summary>
        /// 获取酒店信息
        /// </summary>
        [OperationContract]
        List<HotelInfoExEntity> GetHotelInfoEx(List<int> hotelID);

        /// <summary>
        /// 刷新酒店信息
        /// </summary>
        [OperationContract]
        void RefreshCacheHotelInfoEx(List<int> hotelID);

        [OperationContract]
        void RefreshCacheGetHotelKeyWordList(List<int> hotelID);

        [OperationContract]
        List<HotelKeyWordListEntity> GetHotelKeyWordList(List<int> hotelID);

        [OperationContract]
        List<CommentKeyWordListEntity> GetCommentKeyWordList(string keyword);

        [OperationContract]
        void SetGetCommentKeyWordList(string keyword);

        /// <summary>
        /// 获取酒店点评
        /// </summary>
        //[OperationContract]
        //List<HotelReviewExEntity> GetHotelReviewEx(List<int> writing);

        /// <summary>
        /// 获取商区筛选条件
        /// </summary>
        [OperationContract]
        List<ClassZoneEntity> GetClassZone(int district, List<int> classID);

        /// <summary>
        /// 刷新商区筛选条件
        /// </summary>
        [OperationContract]
        void SetClassZone(int district, List<int> classID);

        /// <summary>
        /// 获取品牌筛选条件
        /// </summary>
        /// <param name="district">目的地</param>
        /// <param name="classHotel">酒店类别</param>
        [OperationContract]
        List<ClassBrandEntity> GetClassBrand(int district, List<int> classID);

        /// <summary>
        /// 获取酒店分类
        /// </summary>       
        [OperationContract]
        List<HotelRankEntity> GetHotelAllClass();

        /// <summary>
        /// 获取分类筛选条件
        /// </summary>
        /// <param name="district">目的地</param>
        /// <returns></returns>
        [OperationContract]
        List<FilterDicEntity> GetHotelClassByDistrict(int district);

        /// <summary>
        /// 重新设机场火车站筛选条件
        /// </summary>
        /// <param name="district"></param>
        [OperationContract]
        void SetAirportTrain(int district);

        /// <summary>
        /// 获取机场火车站筛选条件
        /// </summary>
        /// <param name="district">目的地</param>
        /// <returns></returns>
        [OperationContract]
        List<DistrictAirportTrainstationEntity> GetAirportTrain(int district);

        /// <summary>
        /// 获取目的地地标
        /// </summary>
        /// <param name="district"></param>
        /// <param name="placeType"></param>
        /// <returns></returns>
        [OperationContract]
        List<DistrictAirportTrainstationEntity> GetPlaceMark(int district, PlaceType placeType);

        /// <summary>
        /// 重设目的地地标
        /// </summary>
        /// <param name="district"></param>
        /// <param name="placeType"></param>
        [OperationContract]
        void SetPlaceMark(int district, PlaceType placeType);

        /// <summary>
        /// 刷新品牌筛选条件
        /// </summary>
        [OperationContract]
        void SetClassBrand(int district, List<int> classID);

        /// <summary>
        /// 刷新分类筛选条件
        /// </summary>
        [OperationContract]
        void SetDistrictHotelClass(int district);

        /// <summary>
        /// 获取附近酒店
        /// </summary>
        [OperationContract]
        List<HotelDistanceEntity> GetNearbyHotel(int hotelID);

        /// <summary>
        /// 获取附近酒店
        /// </summary>
        [OperationContract]
        List<HotelDistanceEntity> GetNearbyHotelEx(int hotelID);

        /// <summary>
        /// 根据餐馆查找附近酒店
        /// </summary>
        /// <param name="restaurant"></param>
        /// <returns></returns>
        [OperationContract]
        List<HotelDistanceEntity> GetNearbyHotelByRestaurant(int restaurant, int district, double lat, double lon);

        /// <summary>
        /// 刷新附近酒店
        /// </summary>
        [OperationContract]
        void SetNearbyHotel(Dictionary<int, string> dic);

        /// <summary>
        /// 刷新附近酒店通过景点
        /// </summary>
        [OperationContract]
        void SetNearbySight(Dictionary<int, string> dic);

        /// <summary>
        /// 获取酒店点评汇总信息
        /// </summary>
        [OperationContract]
        List<HotelReviewSummaryEntity> GetHotelReviewSummary(List<int> hotelID);

        /// <summary>
        /// 获取酒店点评
        /// </summary>
        /// <param name="hotelID"></param>
        /// <param name="startIndex"></param>
        /// <param name="returnCount"></param>
        /// <param name="userID"></param>
        /// <param name="hideUserID"></param>
        /// <returns></returns>
        //[OperationContract]
        //List<HotelReviewExEntity> GetHotelReviewList(ArguHotelReview argu);

        [OperationContract]
        List<HotelInfoChannelExEntity> GetHotelChannelList(List<int> hotelID);

        [OperationContract]
        void SetHotelInfoChannelEx(List<int> hotelID);
        /// <summary>
        /// 更新酒店点评状态
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        //[OperationContract]
        //bool SetHotelReviewStatus(HotelReviewAuditEntity c);

        #region 酒店点评提交

        [OperationContract]
        string RefreshAllHotelCache();

        [OperationContract]
        string RefreshSomeHotelCache(List<int> hotelids);


        /// <summary>
        /// 提交酒店点评到Hoteldb
        /// </summary>
        /// <param name="hotelreviewinfo"></param>
        /// <returns></returns>
        [OperationContract]
        HotelReviewSaveResult HotelReviewSubmitToHotelDB(HotelReviewEntity hotelreviewinfo);

        /// <summary>
        /// 提交补充点评
        /// </summary>
        /// <param name="hotelreviewAddcontent"></param>
        /// <returns></returns>
        //[OperationContract]
        //HotelReviewSaveResult HotelReviewAddtionContentSubmit(HotelReviewAddtionEntity hotelreviewAddcontent);

        #endregion

        [OperationContract]
        List<HotelFacilityEntity> GetAllFacilityList();

        /// <summary>
        /// 获取某条点评条目数
        /// 返回第几天点评，按发表时间降序
        /// 0 没找到点评
        /// </summary>
        //[OperationContract]
        //long GetIndexHotelReview(int hotelID, int writing, long userID, long hideUserID);

        //[OperationContract]
        //List<HotelReviewExEntity> GetHotelReviewWritingListByUserid(long userid);

        [OperationContract]
        void ClearHJDHotelcommentCacheData(int hotelid);

        /// <summary>
        /// 跟新会有对点评是否有用评价
        /// </summary>
        /// <param name="writing"></param>
        /// <param name="uid"></param>
        /// <param name="ip"></param>
        /// <returns></returns>
        [OperationContract]
        string SetCommentWritingUseful(int writing, long userid, string ip);

        /// <summary>
        /// 酒店搜索
        /// </summary>
        /// <param name="districts">目的地id列表</param>
        /// <param name="type">酒店类型</param>
        /// <param name="sort">排序类型(排名r,价格j)</param>
        /// <param name="order">排序方式(desc,asc)</param>
        /// <param name="start"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        //[OperationContract]
        //List<HotelItem> SearchHotel(HotelSearchParas argu, out long intCount);//SearchHotelArgu argu);

        [OperationContract]
        void SetGetHotelSimilarCommentListByWriting(int writing);

        /// <summary>
        /// 获取点评相似点评
        /// </summary>
        /// <param name="writing"></param>
        /// <returns></returns>
        [OperationContract]
        List<int> GetHotelSimilarCommentListByWriting(int writing);

        /// <summary>
        /// 获取点评相似酒店列表
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        List<int> SimilarReviewHotelList();

        [OperationContract]
        void SetSimilarReviewHotelList();
        /// <summary>
        /// 酒店详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [OperationContract]
        HotelItem GetHotel(int id);

        /// <summary>
        /// 获取酒店点评
        /// </summary>
        /// <param name="userid">当前登录用户</param>
        /// <param name="hotel">酒店id</param>
        /// <param name="review">点评id</param>
        /// <param name="start"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        //[OperationContract]
        //List<HotelReviewExEntity> GetHotelReviews(long userid, int hotel, int review, int start, int count);

        [OperationContract]
        int GetHotelReviewCount(int hotel);

        [OperationContract]
        List<HotelItem> GetHotels(List<int> ids);

        [OperationContract]
        HotelReviewSaveResult SetHotelReviewWritingLog(HotelReviewWritingLog hcl);

        //[OperationContract]
        //List<HotelListInfoEntity> GetHotelListByQuery(HotelSearchParas p, out long count);

        /// <summary>
        /// 酒店与POI的距离
        /// </summary>
        /// <param name="hotelid"></param>
        /// <param name="poiid"></param>
        /// <returns></returns>
        [OperationContract]
        int GetHotelPOIDistanceByHotel(int hotelid, int poiid);

        [OperationContract]
        string GetBookingUrlByHotel(int hotelID);

        [OperationContract]
        string GetAgodaUrlByHotel(int hotelID);

        [OperationContract]
        int GetHotelIDByHotelOriID(int hotelOriID);

        [OperationContract]
        void UpdateHotelTagWithWriting(int writing);

        [OperationContract]
        void UpdateHotelTag(DateTime dt);
        [OperationContract]
        void UpdateHotelTag2(DateTime before, DateTime now);
        //[OperationContract]
        //void UpdateHotelSeoKeyword();
        /// <summary>
        /// 获取特色标签列表For酒店列表查询选项
        /// </summary>
        /// <param name="districtID">目的地编号</param>
        /// <param name="type">0：全部；1：中档；2：客栈；3：度假</param>
        /// <returns></returns>
        [OperationContract]
        List<TagEntity> GetTagParams4HotelList(int districtID, int type);

        /// <summary>
        /// 获取菜单和类别对应关系
        /// </summary>
        /// <param name="menuID">1:中档；2：客栈；3：中档</param>
        /// <returns></returns>
        [OperationContract]
        List<int> GetClassIDs(int menuID);

        [OperationContract]
        List<ValuedDistrictEntity> GetMostValuedDistrict();

        [OperationContract]
        List<UniqueDistrictEntity> GetMostUniqueDistrict();

        [OperationContract]
        List<TagNameEntity> GetTagList(List<int> tagIDs);


        /// <summary>
        /// 获取酒店列表价
        /// </summary>
        /// <param name="hotelIDs"></param>
        /// <param name="arrivalTime"></param>
        /// <param name="departureTime"></param>
        /// <param name="minPrice"></param>
        /// <param name="maxPrice"></param>
        /// <returns></returns>
        [OperationContract]
        List<HotelPrice> QueryHotelListPrice(List<int> hotelIDs, DateTime arrivalTime, DateTime departureTime, decimal minPrice = 0, decimal maxPrice = 1000000);


        /// <summary>
        /// 获取指定OTA的酒店列表价，如wap用携程的价格
        /// </summary>
        /// <param name="hotelIDs"></param>
        /// <param name="arrivalTime"></param>
        /// <param name="departureTime"></param>
        /// <param name="OTAID"></param>
        /// <returns></returns>
        [OperationContract]
        List<HotelPrice> QueryHotelListPriceWithOTAID(List<int> hotelIDs, DateTime arrivalTime, DateTime departureTime, int OTAID, decimal minPrice = 0, decimal maxPrice = 1000000);

        [OperationContract]
        List<FilterDicEntity> GetQueryHotelFilters(HotelSearchParas p);

        [OperationContract]
        WapMenuEntity QueryHotelWapMenu(HotelSearchParas p, int needTagLength);

        [OperationContract]
        QueryHotelResult2 QueryHotel2(HotelSearchParas p, int PriceWithOTAID = 0);

        [OperationContract]
        QueryHotelResult3 QueryHotel3(HotelSearchParas p, int PriceWithOTAID = 0);
        [OperationContract]
        QueryHotelResult3 QueryHotelForMagicall(HotelSearchParas p, int PriceWithOTAID = 0);

        [OperationContract]
        QueryHotelResult QueryHotel(HotelSearchParas p, int PriceWithOTAID = 0);

        [OperationContract]
        MenuEntity ParseHotelMenu(Dictionary<long, int> filterCount, bool channel = false, int districtID = 0);

        [OperationContract]
        MenuEntity InitHotelMenu(int districtID);

        [OperationContract]
        QueryReviewResult QueryReview(ArguHotelReview p);

        [OperationContract]
        List<HotelReviewExEntity> GetHotelReviewByCommentId(int commentId);


        [OperationContract]
        List<HotelReviewExEntity> QueryCommentReview(ArguHotelReview p);

        [OperationContract]
        List<TagEntity> GetHotelTag(int hotelID);

        [OperationContract]
        List<int> GetHotelTagReviewID(int tagID, int hotelID);

        /// <summary>
        /// 获取目的地中档价格区间
        /// </summary>
        /// <param name="DistrictID"></param>
        /// <returns></returns>
        [OperationContract]
        List<int> GetZhongdangPriceSection(int DistrictID);

        [OperationContract]
        List<HotelReviewEntity> GetReviewStatus(int hotelID, long userID);

        [OperationContract]
        List<CommDictEntity> GetCommDictList(int type);

        [OperationContract]
        void GetValuedHotel();

        [OperationContract]
        List<HotelSEOKeyword> GetHotelSEOKeyword(int hotelID);

        [OperationContract]
        List<HotelSEOKeyword> GetAllHotelSEOKeyword();

        //[OperationContract]
        //QueryReviewResult QuerySEOKeywordReview(ArguHotelSEOKeywordReview p);

        [OperationContract]
        void MoveReview17U(MoveReview17U mr);


        [OperationContract]
        void MoveReviewElong(MoveReviewElong mr);

        [OperationContract]
        void MoveReviewCtrip(MoveReviewCtrip mr);

        [OperationContract]
        int InitReviewOTA(MoveReviewOTA mr);

        [OperationContract]
        void InitReview17U(MoveReview17U mr);

        [OperationContract]
        void InitReviewElong(MoveReviewElong mr);

        [OperationContract]
        int InitReviewCtrip(MoveReviewCtrip mr);

        [OperationContract]
        int InitReviewBooking(MoveReviewBooking mr);

        [OperationContract]
        List<ListHotelItem2> GetCollectHotelList(List<int> hotelIdList);

        [OperationContract]
        List<VRateEntity> GetVRateByHVID(int HVID);
        //[OperationContract]
        //List<PackageEntity> GetPackegeListByHotelIDs(List<int> hotelIDs);

        [OperationContract]
        int UpdateInspectorRefHotelState();

        [OperationContract]
        List<InspectorHotel> GetInspectorHotelByHotelId(int hotelId);

        [OperationContract]
        int InsertOrUpdatePoints(PointsEntity point);

        [OperationContract]
        int DeletePoints(int id);

        [OperationContract]
        int InsertOrUpdatePointsConsume(PointsConsumeEntity point);

        [OperationContract]
        List<PointsEntity> GetPointsEntity(long userId);


        [OperationContract]
        List<PointsEntity> GetExpirePointsEntity(DateTime startTime, DateTime endTime, string userids = "");

        [OperationContract]
        List<PointsEntity> GetPointsEntityByToDayAndTypeId(int typeId);

        /// <summary>
        /// 即将过期积分
        /// </summary>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns></returns>
        [OperationContract]
        List<ExpirePointsEntity> GetExpirePoints(DateTime startTime, DateTime endTime);

        [OperationContract]
        List<PointsConsumeEntity> GetPointsConsumeEntity(long userId);

        [OperationContract]
        List<PointsTypeDefineEntity> GetPointsTypeDefineList(string code, int type);

        [OperationContract]
        List<SourceIDAndObjectNameEntity> GetObjectNamesByTypeCode(List<long> ids, string typeCode, int typeID);

        [OperationContract]
        int GetAvailablePointByUserID(long userID, int typeID);

        [OperationContract]
        PointsEntity GetPointsByIDOrTypeIDAndBusinessID(int id, int typeID, int businessID);

        [OperationContract]
        List<PointsEntity> GetPointslistNumByUserIdAndTypeId(long userId, int typeId);

        [OperationContract]
        int UpdateInspectorStateByPointsNum();

        [OperationContract]
        List<PackageInfoEntity> GetCtripHotelPackagesV42(int hotelid, DateTime CheckIn, DateTime CheckOut, bool updatePrice, HJD.HotelServices.Contracts.HotelServiceEnums.PricePolicyType ppt = HJD.HotelServices.Contracts.HotelServiceEnums.PricePolicyType.Default);

        [OperationContract]
        List<PackageInfoEntity> GetCtripHotelPackagesForApiV42(int hotelid, DateTime CheckIn, DateTime CheckOut, bool updatePrice, HJD.HotelServices.Contracts.HotelServiceEnums.PricePolicyType ppt = HJD.HotelServices.Contracts.HotelServiceEnums.PricePolicyType.Default);

        [OperationContract]
        List<PackageInfoEntity> GetCtripHotelPackages(int hotelid, DateTime CheckIn, DateTime CheckOut, bool updatePrice, HJD.HotelServices.Contracts.HotelServiceEnums.PricePolicyType ppt = HJD.HotelServices.Contracts.HotelServiceEnums.PricePolicyType.Default);

        [OperationContract]
        List<PDayItem> GenCtripHotelPackageCalendar(int hotelid, DateTime startDate, int pid = 0, int canlendarLength = 360, HJD.HotelServices.Contracts.HotelServiceEnums.PricePolicyType ppt = HJD.HotelServices.Contracts.HotelServiceEnums.PricePolicyType.Default);

        [OperationContract]
        List<PDayItem> GenHotelPackageCalendar(int hotelid, DateTime startDate, int pid = 0, int canlendarLength = 360, HJD.HotelServices.Contracts.HotelServiceEnums.PricePolicyType ppt = HJD.HotelServices.Contracts.HotelServiceEnums.PricePolicyType.Default);

        [OperationContract]
        bool NeedCtripPackage(int hotelid);

        [OperationContract]
        List<ListHotelItem3> GetListHotelItem3List(List<int> hotelIDs);

        [OperationContract]
        List<ListHotelItem3> GetUserRecommendHotelList(int hotelID, long userID, int interestID, int maxCount = 3, int distance = 200000);

        #region Top 20 套餐排序
        [OperationContract]
        List<TopNPackagesEntity> GetTopNPackagesEntityList(TopNPackageSearchParam param, out int count);

        [OperationContract]
        List<TopNPackagesEntity> GetTopNPackagesAddSearchEntityList(TopNPackageSearchParam param, out int count);

        [OperationContract]
        int UpdateTopNPackage(TopNPackagesEntity tnpe);

        [OperationContract]
        List<TopNPackageItem> GetPackageItemList(List<int> pids);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pids"></param>
        /// <param name="currentDate"></param>
        /// <param name="isOnlyOnlinePackage">只返回上线状态的套餐信息</param>
        /// <returns></returns>
        [OperationContract]
        List<TopNPackageItem> GetPackageItemList2(List<int> pids, DateTime? currentDate, bool isOnlyOnlinePackage = true);


        /// <summary>
        /// 获取某专辑内容
        /// </summary>
        /// <param name="isValid"></param>
        /// <param name="start"></param>
        /// <param name="count"></param>
        /// <param name="albumsId"></param>
        /// <returns></returns>
        [OperationContract]
        List<TopNPackageItem> GetTopNPackageList(bool isValid, int start, int count, int albumsId = 0, bool isShowVipFirstBuypackage = true, bool isNeedNotSale = false, string dateStr = "", int gotoDistrictID = 0, int startDistrictID = 0);
        /// <summary>
        /// 获取分组套餐专辑列表,Package.PackageGroupName相同且不为空，只取价格最低的套餐
        /// </summary>
        /// <param name="isValid"></param>
        /// <param name="start"></param>
        /// <param name="count"></param>
        /// <param name="albumsId"></param>
        /// <returns></returns>
        [OperationContract]
        List<TopNPackageItem> GetTopNGroupPackageList(bool isValid, int start, int count, int albumsId = 0, bool isShowVipFirstBuypackage = true, bool isNeedNotSale = false, string dateStr = "", int gotoDistrictID = 0, int startDistrictID = 0, int pid=0);


        [OperationContract]
        List<AlbumPackageSimpleEntity> GetTopNPackageScreenList(int albumsID);

        /// <summary>
        /// 根据目的获取专辑列表
        /// </summary>
        /// <param name="isValid"></param>
        /// <param name="start"></param>
        /// <param name="count"></param>
        /// <param name="albumsId"></param>
        /// <param name="DistrictIds"></param>
        /// <returns></returns>
        [OperationContract]
        List<TopNPackageItem> GetTopNPackageListByDistrictIds(bool isValid, int start, int count, int albumsId = 0, string districtIds = "", bool isShowVipFirstBuypackage = true, string dateStr = "", int gotoDistrictID = 0);
        /// <summary>
        /// 根据目的获取分组套餐专辑列表,Package.PackageGroupName相同且不为空，只取价格最低的套餐
        /// </summary>
        /// <param name="isValid"></param>
        /// <param name="start"></param>
        /// <param name="count"></param>
        /// <param name="albumsId"></param>
        /// <param name="DistrictIds"></param>
        /// <returns></returns>
        [OperationContract]
        List<TopNPackageItem> GetTopNGroupPackageListByDistrictIds(bool isValid, int start, int count, int albumsId = 0, string districtIds = "", bool isShowVipFirstBuypackage = true, string dateStr = "", int gotoDistrictID = 0, int startDistrictID = 0);

        [OperationContract]
        List<TopNPackageItem> GetTopNPackageAddSearchList(bool isValid, int start, int count, int albumsId = 0, float lat = 0, float lng = 0, int geoScopeType = 0, int districtID = 0, bool isShowVipFirstBuypackage = true);

        /// <summary>
        /// 专辑对应的城市
        /// </summary>
        /// <param name="lat"></param>
        /// <param name="lng"></param>
        /// <returns></returns>
        [OperationContract]
        List<HotelDestnInfo> GetHotelDestnInfo(int albumsID);

        /// <summary>
        /// 专辑对应 附近城市
        /// </summary>
        /// <param name="lat"></param>
        /// <param name="lng"></param>
        /// <returns></returns>
        [OperationContract]
        List<HotelDestnInfo> GetHotelDestWithIn(int albumsID, float lat, float lng);

        /// <summary>
        /// 发布OR下线
        /// </summary>
        /// <param name="IsValid">True则发布top20;false则下线top20</param>
        /// <param name="Updator"></param>
        /// <returns></returns>
        [OperationContract]
        int UpdateTopNPackageBatch(bool IsValid, long Updator);

        [OperationContract]
        int DeleteTopNPackage(int id);

        [OperationContract]
        List<TypeAndPrice> GetPackageTypeAndPriceList(List<int> pids);

        /// <summary>
        /// 计算两个地区之间的距离  单位米
        /// </summary>
        /// <param name="districtID1"></param>
        /// <param name="districtID2"></param>
        /// <returns></returns>
        [OperationContract]
        int CalculateDistrictDistance(int districtID1, int districtID2);

        /// <summary>
        /// 通过用户经纬度 计算与某个地区的距离
        /// </summary>
        /// <param name="userLat"></param>
        /// <param name="userLon"></param>
        /// <param name="districtID"></param>
        /// <returns></returns>
        [OperationContract]
        int CalculateUserDistrictDistance(double userLat, double userLon, int districtID);

        /// <summary>
        /// 地区ID 经纬度 半径范围内的地区ID集合
        /// </summary>
        /// <param name="districtID"></param>
        /// <param name="lat"></param>
        /// <param name="lon"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        [OperationContract]
        List<ArounDistrictEntity> CalculateNearDistrictByDistance(int districtID, float lat, float lon, int distance = 300000);

        /// <summary>
        /// 获取酒店包房出售情况
        /// </summary>
        /// <param name="hotelid"></param>
        /// <param name="checkIn"></param>
        /// <param name="checkOut"></param>
        /// <returns></returns>
        [OperationContract]
        IEnumerable<RoomSouldCountEntity> GetHotelPackRoomSouldInfo(int hotelid, DateTime checkIn, DateTime checkOut);

        /// <summary>
        /// 通过用户备注分析用户预订的大床与双床数
        /// </summary>
        /// <param name="hotelID"></param>
        /// <param name="checkIn"></param>
        /// <param name="nightCount"></param>
        /// <param name="roomCount"></param>
        /// <returns>数组： [0]：大床数 [1]：双床数</returns>
        [OperationContract]
        List<int> ParseBedCountWithNote(int pID, string note, int roomCount);

        /// <summary>
        /// 套餐、酒店ID入住日期范围 获得可用的包房数量和价格
        /// </summary>
        /// <param name="hotelID"></param>
        /// <param name="checkIn"></param>
        /// <param name="nightCount"></param>
        /// <param name="roomCount"></param>
        /// <param name="pid"></param>
        /// <returns></returns>
        [OperationContract]
        List<PackRoomBedStateEntity20> GetCanUsePackRoomSupplier(int hotelID, DateTime checkIn, int nightCount, int pid, int BigBedCount, int TwinBedCount, int roomSupplierID = 0);

        /// <summary>
        /// 判断套餐缺省的房间供应商是不是包房供应商
        /// </summary>
        /// <param name="hotelID"></param>
        /// <param name="pid"></param>
        /// <returns></returns>
        [OperationContract]
        bool IsPackRoomSupplierByHotelID_PID(int hotelID, int pid);

        /// <summary>
        /// 批量获取酒店列表过滤标签名称ID
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [OperationContract]
        List<FilterDicEntity> GetHotelListFilterTagInfos(SearchHotelListFilterTagInfoParam param);

        /// <summary>
        /// 由名称生成一条客户添加酒店的新纪录 如果名称存在则返回记录的ID
        /// 如果commentID不为0那么新增一条点评记录与用户添加酒店的对应关系
        /// </summary>
        /// <param name="hotelName"></param>
        /// <param name="commentID"></param>
        /// <returns></returns>
        [OperationContract]
        int InsertUserAddHotels(string hotelName, long userID, int commentID = 0);

        /// <summary>
        /// 由点评ID获得新增酒店的名称
        /// </summary>
        /// <param name="commentID"></param>
        /// <returns></returns>
        [OperationContract]
        List<CommentAddHotelEntity> GetUserAddHotelByComment(int commentID, long userID = 0);

        /// <summary>
        /// 酒店名称 用户ID 验证是否提交过该名称的点评 防止重复点评
        /// </summary>
        /// <param name="hotelName"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        [OperationContract]
        int GetUserAddHotelForComment(string hotelName, long userID);
        #endregion

        /// <summary>
        /// 更新酒店坐标数据集合
        /// </summary>
        /// <param name="zoneID"></param>
        /// <returns></returns>
        [OperationContract]
        int UpdateHotelDistrictZoneRel(int zoneID);

        /// <summary>
        /// 绑定区域和酒店
        /// </summary>
        /// <param name="districtZoneID"></param>
        /// <param name="districtZoneName"></param>
        /// <param name="bindType"></param>
        /// <returns></returns>
        [OperationContract]
        int BindZoneHotelRel(int zoneID, string districtZoneName, BindZoneHotelRelType bindType);

        /// <summary>
        /// 获得酒店的设施集合
        /// </summary>
        /// <param name="hotelID"></param>
        /// <returns></returns>
        [OperationContract]
        List<HotelFacilityEntity> GetHotelFacilitysByHotelID(int hotelID);

        /// <summary>
        /// 主题特色关联酒店数量（默认按酒店数量降序排列）
        /// </summary>
        /// <param name="interestType"></param>
        /// <returns></returns>
        [OperationContract]
        IEnumerable<InterestHotelCountEntity> GetInterestHotelCountList(int interestType);

        /// <summary>
        /// 批量获取酒店基本信息
        /// </summary>
        /// <param name="hotelIDs"></param>
        /// <returns></returns>
        [OperationContract]
        IEnumerable<HotelBasicInfo> GetHotelBasicInfoList(IEnumerable<int> hotelIDs);

        /// <summary>
        /// 消费用户积分 
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="requiredPoints"></param>
        /// <param name="typeID"></param>
        /// <param name="businessID"></param>
        /// <returns></returns>
        [OperationContract]
        int ConsumeUserPoints(long userID, int requiredPoints, HJD.HotelServices.Contracts.HotelServiceEnums.ConsumeUserPointsBizType typeID, long businessID);


        /// <summary>
        /// 退用户积分
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="typeID"></param>
        /// <param name="businessID"></param>
        /// <returns>0：成功  其它：失败</returns>
        [OperationContract]
        int RefundUserPoints(long userID, HJD.HotelServices.Contracts.HotelServiceEnums.ConsumeUserPointsBizType typeID, int businessID);

        /// <summary>
        /// 获取积分
        /// </summary>
        /// <param name="typeid"></param>
        /// <param name="ffRelIds"></param>
        /// <returns></returns>
        [OperationContract]
        List<PointsEntity> GetPointListByTypeIDAndUserID(int typeid, IEnumerable<long> ffRelIds);

        /// <summary>
        /// 获取积分
        /// </summary>
        /// <param name="typeid"></param>
        /// <param name="ffRelIds"></param>
        /// <returns></returns>
        [OperationContract]
        List<PointsEntity> GetPointListByTypeIDAndBusinessID(int typeid, IEnumerable<long> ffRelIds);


        /// <summary>
        /// 退还用户积分
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="typeID"></param>
        /// <param name="businessID"></param>
        /// <returns></returns>
        //[OperationContract]
        //int ReturnUserPoints(long userID, int typeID, int businessID);

        /// <summary>
        /// 酒店相关主题 已发布的
        /// </summary>
        /// <param name="hotelID"></param>
        /// <returns></returns>
        [OperationContract]
        IEnumerable<InterestEntity> GetInterestListByHotel(int hotelID);

        /// <summary>
        /// 回收哪类积分 默认所有过期的积分
        /// </summary>
        /// <param name="typeID"></param>
        /// <returns></returns>
        [OperationContract]
        int PointsRecycle(int typeID = 0);

        /// <summary>
        /// 自入住日期开始的最晚写点评的天数
        /// </summary>
        /// <param name="maxDay"></param>
        /// <returns></returns>
        [OperationContract]
        IEnumerable<InspectorRefHotel> GetNeedWriteCommentInspectorRefHotel(int maxDay = 7);

        /// <summary>
        /// 更新指定的品鉴酒店提醒写点评状态
        /// </summary>
        /// <param name="inspectorRefHotelIDs"></param>
        /// <returns></returns>
        [OperationContract]
        int UpdateNoticeWriteCommentState4InspectorRefHotel(IEnumerable<int> inspectorRefHotelIDs);

        /// <summary>
        /// 获得酒店地图信息
        /// </summary>
        /// <param name="hotelID"></param>
        /// <returns></returns>
        [OperationContract]
        HotelMapBasicInfo GetHotelMapInfo(int hotelID);

        /// <summary>
        /// 获取用户某个状态的品鉴记录
        /// 如果state=0则是全部状态的品鉴记录 1.待审核 2.审核通过 3.审核不通过
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        [OperationContract]
        IEnumerable<InspectorRefHotel> GetInspectorRefHotelByUserID(long userID, int state = 0);

        /// <summary>
        /// 获取地区对应攻略目的地的照片
        /// </summary>
        /// <param name="districtId"></param>
        /// <returns></returns>
        [OperationContract]
        string GetDistrictZonePicSUrl(int districtId);

        [OperationContract]
        QueryHotelResult3 QueryHotelByDistrictInterest(int districtId, int interestId = 0);

        [OperationContract]
        QueryHotelResult3 QueryHotelByHids(string hotelids);

        [OperationContract]
        List<string> GetProvinceListByInterest(int interestId);

        /// <summary>
        /// 插入浏览记录 返回记录ID
        /// </summary>
        /// <param name="browsing"></param>
        /// <returns></returns>
        [System.ServiceModel.OperationContract]
        int InsertBrowsingRecord(BrowsingRecordEntity browsing);

        /// <summary>
        /// 插入搜索记录  返回记录ID
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [System.ServiceModel.OperationContract]
        int InsertSearchRecord(SearchRecordEntity search);

        /// <summary>
        /// 插入分享记录  返回记录ID
        /// </summary>
        /// <param name="share"></param>
        /// <returns></returns>
        [System.ServiceModel.OperationContract]
        int InsertShareRecord(ShareRecordEntity share);

        /// <summary>
        /// 插入分享记录
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [System.ServiceModel.OperationContract]
        List<ShareRecordEntity> GetShareRecordList(CommonRecordQueryParam param);

        /// <summary>
        /// 批量获取酒店的filterCol
        /// </summary>
        /// <param name="hotelIds"></param>
        /// <returns></returns>
        [System.ServiceModel.OperationContract]
        List<HotelFilterColEntity> GetManyHotelFilterCols(IEnumerable<int> hotelIds);

        /// <summary>
        /// 获取点评各渠道的浏览数量
        /// </summary>
        /// <param name="commentId"></param>
        /// <param name="terminalTypeArray"></param>
        /// <returns></returns>
        [System.ServiceModel.OperationContract]
        int GetBrowseringCountOneComment(int commentId, int[] terminalTypeArray);

        /// <summary>
        /// 获取某个地区热门搜索Tag
        /// </summary>
        /// <param name="districtId"></param>
        /// <returns></returns>
        [System.ServiceModel.OperationContract]
        List<DistrictHotFilterTagEntity> GetDistrictHotFilterTagList(int districtId);

        /// <summary>
        /// 获取某个地区热门搜索Tag
        /// </summary>
        /// <param name="districtId"></param>
        /// <returns></returns>
        [System.ServiceModel.OperationContract]
        int UpsertDistrictHotFilterTagList(IEnumerable<DistrictHotFilterTagEntity> hotTags);

        /// <summary>
        /// 根据Id获取主题信息
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [System.ServiceModel.OperationContract]
        InterestEntity GetOneInterestEntity(int Id);

        /// <summary>
        /// 绑定点评的订单ID
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="commentId"></param>
        /// <returns></returns>
        [System.ServiceModel.OperationContract]
        int UpdateCommentForOrderId(long orderId, int commentId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hotelId"></param>
        /// <returns></returns>
        [System.ServiceModel.OperationContract]
        List<HotelFilterColEntity> GetHotelDisplayFilterTagList(int hotelId);

        [System.ServiceModel.OperationContract]
        List<HotelRoomTypeFilterTagEntity> GetHotelRoomTypeFilterTagList(int hotelId);

        [System.ServiceModel.OperationContract]
        List<PRoomInfoEntity> GetProomInfoList(int hotelId);

        #region 各种优惠套餐
        [System.ServiceModel.OperationContract]
        List<CanSellCheapHotelPackageEntity> GetCheapHotelPackage4Botao(DateTime startTime);
        #endregion

        #region 铂韬结算订单数据

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [System.ServiceModel.OperationContract]
        int InsertBotaoSettlementRecord(BotaoSettlementEntity entity);

        /// <summary>
        /// 铂涛待结算订单列表
        /// </summary>
        /// <returns></returns>
        [System.ServiceModel.OperationContract]
        List<BotaoSettleOrderEntity> GetBotaoSettleOrderList();

        #endregion

        #region 计算包房酒店列表 某个日期有房的
        [System.ServiceModel.OperationContract]
        List<int> GetPackRoomHotelIdList(DateTime date);
        #endregion

        #region 根据酒店名字获取酒店 用于点评上传找到对应的酒店ID
        [System.ServiceModel.OperationContract]
        int GetHotelIdByName(string hotelName);
        #endregion

        [OperationContract]
        List<NearbyHotelEntity> GetHotelListNearPOI(int poiId);

        /// <summary>
        /// 获取与某个套餐ID相同序列的套餐列表
        /// </summary>
        /// <param name="pId"></param>
        /// <returns></returns>
        [OperationContract]
        List<SameSerialPackageEntity> GetSameSerialPackageEntityList(int pId);


        /// <summary>
        /// 获取与某个套餐ID相同组号的套餐列表
        /// </summary>
        /// <param name="pId"></param>
        /// <returns></returns>
        [OperationContract]
        List<SameSerialPackageEntity> GetSerialPackageItemListByPid(int pId, DateTime currentDate,int hotelId);

        #region 套餐专辑列表

        /// <summary>
        /// 专辑列表
        /// </summary>
        /// <param name="param"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        [OperationContract]
        List<PackageAlbumsEntity> GetPackageAlbumsList(TopNPackageSearchParam param, out int totalCount);

        /// <summary>
        /// 获取所有专辑列表
        /// </summary>
        /// <param name="param"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        [OperationContract]
        List<PackageAlbumsEntity> GetAllPackageAlbums();

        /// <summary>
        /// 根据专辑组号获取 专辑
        /// </summary>
        /// <param name="param"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        [OperationContract]
        List<PackageAlbumsEntity> GetPackageAlbumsByGroupNo(string groupNo);

        /// <summary>
        /// 单张专辑信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [OperationContract]
        PackageAlbumsEntity GetOnePackageAlbums(int id);

        /// <summary>
        /// 删掉专辑
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [OperationContract]
        int DelPackageAlbums(int id);

        /// <summary>
        /// 新增或更新专辑
        /// </summary>
        /// <param name="album"></param>
        /// <returns></returns>
        [OperationContract]
        int InsertOrUpdatePackageAlbums(PackageAlbumsEntity album);

        /// <summary>
        /// 获取酒店类列表
        /// </summary>
        /// <param name="param"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        [OperationContract]
        List<TopNPackagesEntity> GetTopNPackagesEntityList4HotelAlbums(TopNPackageSearchParam param, out int count);
        #endregion

        #region 批量获取房型列表
        /// <summary>
        /// 获取房间列表
        /// </summary>
        /// <param name="roomIds"></param>
        /// <returns></returns>
        [OperationContract]
        List<PRoomInfoEntity> GetPRoomInfoEntityList(IEnumerable<int> roomIds);
        #endregion

        /// <summary>
        /// 由酒店Id或套餐Id获得绑定的专辑
        /// </summary>
        /// <param name="hotelId"></param>
        /// <param name="pId"></param>
        /// <returns></returns>
        [OperationContract]
        List<RelPackageAlbumsEntity> GetRelPackageAlbums(IEnumerable<int> hotelIds, IEnumerable<int> pIds);

        /// <summary>
        /// 获取区域内的可售优惠酒店
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        [OperationContract]
        List<CanSellDistrictCheapHotel> GetCanSellDistrictCheapHotelList(HJD.HotelServices.Contracts.HotelServiceEnums.HotelDistrictRange range);

        /// <summary>
        /// 获取指定套餐在指定范围内的数据
        /// </summary>
        /// <param name="pIds"></param>
        /// <param name="checkIn"></param>
        /// <param name="checkOut"></param>
        /// <returns></returns>
        [OperationContract]
        List<HotelTop1PackageInfoEntity> GetHotelPackageFirstAvailablePriceByPIds(IEnumerable<int> pIds, DateTime? checkIn, DateTime? checkOut);

        /// <summary>
        /// 获取单个套餐的数据
        /// </summary>
        /// <param name="pId"></param>
        /// <returns></returns>
        [OperationContract]
        PackageEntity GetOnePackageEntity(int pId);

        /// <summary>
        /// “大家都说好”酒店列表 App V4.6版启用
        /// </summary>
        /// <param name="param"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        [OperationContract]
        List<TopScoreHotelEntity> GetTopScoreHotelList(BsicSearchParam param, out int totalCount);

        /// <summary>
        /// 获取附近地区的套餐专辑
        /// </summary>
        /// <param name="districtID"></param>
        /// <param name="lat"></param>
        /// <param name="lng"></param>
        /// <returns></returns>
        [OperationContract]
        List<PackageAlbumsEntity> GetPackageAlbumByGeoInfo(int districtID, float lat, float lng);

        /// <summary>
        /// 更新排名 排名数字越大越靠前
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="RankScore"></param>
        /// <returns></returns>
        [OperationContract]
        int UpdateTopScoreHotel(int Id, float RankScore);

        /// <summary>
        /// 批量获取酒店是否可显示携程价格
        /// </summary>
        /// <param name="hotelIds"></param>
        /// <returns></returns>
        [System.ServiceModel.OperationContract]
        Dictionary<int, bool> CanShowCtripPrice(IEnumerable<int> hotelIds);

        /// <summary>
        /// 获取酒店浏览记录
        /// </summary>
        /// <param name="param"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        [System.ServiceModel.OperationContract]
        List<TopScoreHotelEntity> GetHotelBrowsingRecordList(BsicSearchParam param, out int totalCount);


        /// <summary>
        /// 获取浏览记录 包括酒店 和 sku
        /// </summary>
        /// <param name="param"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        [System.ServiceModel.OperationContract]
        List<BrowsingRecordEntity> GetBrowsingRecordList(BsicSearchParam param, out int totalCount);

        /// <summary>
        /// 获取300KM之内酒店
        /// </summary>
        /// <param name="param"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        [System.ServiceModel.OperationContract]
        List<TopScoreHotelEntity> GetHotelWithInDistance(float lat, float lng, int count, int start, out int totalCount);
        /// <summary>
        /// 获取最近搜索记录
        /// </summary>
        /// <param name="param"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        [System.ServiceModel.OperationContract]
        List<SearchRecordEntity> GetSearchRecordList(CommonRecordQueryParam param, out int totalCount);

        /// <summary>
        /// 查询指定酒店的基于OTA数据的房态数据
        /// </summary>
        /// <param name="hotelId"></param>
        /// <returns></returns>
        [System.ServiceModel.OperationContract]
        List<OtaRoomBedState> GetOtaRoomBedStateByHid(int hotelId);

        /// <summary>
        /// 获取有合作的酒店的房态汇总信息
        /// </summary>
        /// <returns></returns>
        [System.ServiceModel.OperationContract]
        List<OtaHotelRoomState> GetOtaHotelRoomStates();

        /// <summary>
        /// 【触发检查指定酒店房态】
        /// </summary>
        /// <param name="hotelid"></param>
        /// <param name="checkIn"></param>
        /// <returns></returns>
        [OperationContract]
        bool PublishCheckHotelRoomStateTask(int hotelid, DateTime date);


        [OperationContract]
        List<ActiveRuleGroupEntity> GetWXActiveRuleGroupList(int id);

        [OperationContract]
        List<ActiveRuleExEntity> GetWXActiveRuleExList(int groupId);

        [OperationContract]
        List<SimpleHotelEntity> GetHotelPackageDistrictInfo(double lat = 0, double lng = 0, int geoScopeType = 0);

        [OperationContract]
        List<PackageItemEntity> GetHotelPackageByDistrictId(int distictId, DateTime checkIn, DateTime checkOut, int pageIndex, int pageSize, out int totalCount);

        [OperationContract]
        RetailHotelEntity GetRetailHotelList(int packageType, int sort, string searchWord = "", int start = 0, int count = 10);

        [OperationContract]
        RetailHotelInfoEntity GetRetailHotelInfo(int hotelId);


        [OperationContract]
        RetailPackageEntity GetRetailPackageInfo(DateTime CheckIn, DateTime CheckOut, int pid, long cid);

        [OperationContract]
        List<TopNPackagesEntity> GetTopNPackagesListByAlbumIdOrPID(int albumId = 0, int pid = 0);
        
         [OperationContract]
        int UpdateTopNPackageTitle(int pid , string title);
        
    }
}