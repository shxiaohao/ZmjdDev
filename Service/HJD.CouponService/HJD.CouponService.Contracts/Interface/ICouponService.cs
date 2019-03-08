using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using HJD.CouponService.Contracts.Entity;

namespace HJD.CouponService.Contracts
{
    [ServiceContract(Namespace = "http://www.zmjiudian.com/")]
    public interface ICouponService
    {
        #region 生成红包
        /// <summary>
        /// 返回原始红包ID
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="typeId"></param>
        /// <param name="sourceId"></param>
        /// <param name="totalMoney"></param>
        /// <param name="moneyStr"></param>
        /// <returns></returns>
        [OperationContract]
        OriginCouponResult GenerateOriginCoupon(long userId, int typeId, long sourceId, int totalMoney, int cashMoney, string moneyStr);
        /// <summary>
        /// 返回抢到的红包金额（单位分）
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="originId"></param>
        /// <returns></returns>
        [OperationContract]
        AcquiredCoupon GetAcquiredCoupon(long userId, string guid, string phoneNo);
        /// <summary>
        /// 查询当前红包对应的争抢记录 userId不为空
        /// </summary>
        /// <param name="originId"></param>
        /// <returns></returns>
        [OperationContract]
        List<AcquiredCoupon> GetAcquiredCouponList(string guid);

        /// <summary>
        /// 更新cashcoupon记录状态
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        OriginCouponResult UpdateCashCoupon(long id, string guid, CashCouponState state);

        /// <summary>
        /// 更新订单转发 送现金券活动状态 并插入一条现金券记录
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        OriginCouponResult UpdateCashCouponAndGenACouponRecord(long id, CashCouponState state, long userId, string phoneNo, int couponAmount);

        /// <summary>
        /// 通过组成字段更新
        /// </summary>
        /// <param name="id"></param>
        /// <param name="guid"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        [OperationContract]
        OriginCouponResult UpdateCashCouponBySourceId(long sourceId, int typeId, long userId, CashCouponState state);

        /// <summary>
        /// 活动的数据
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="typeId"></param>
        /// <param name="phoneNo"></param>
        /// <returns></returns>
        [OperationContract]
        OriginCouponResult GenerateOriginCoupon2(long userId, int typeId, string phoneNo);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="typeId"></param>
        /// <param name="phoneNo"></param>
        /// <param name="sourceId"></param>
        /// <returns></returns>
        [OperationContract]
        OriginCouponResult GenerateOriginCoupon3(long userId, int typeId, string phoneNo, long sourceId);

        /// <summary>
        /// 活动券分发
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="code">活动码</param>
        /// <param name="phoneNo">手机号</param>
        /// <returns></returns>
        [OperationContract]
        OriginCouponResult GenerateOriginCouponByActivity(long userId, CouponActivityCode code, string phoneNo);

        #endregion

        #region 使用红包
        /// <summary>
        /// 订单使用现金券记录
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="orderid"></param>
        /// <param name="couponAmount"></param>
        /// <returns></returns>
        [OperationContract]
        ReturnCouponResult SetOrderCoupon(long userid, long orderid, int couponAmount, bool isBudget);

        ///// <summary>
        ///// 客人退还现金券数量 优先补未过期 剩余注入已过期
        ///// </summary>
        ///// <param name="userId">用户Id</param>
        ///// <param name="CouponNum">退还现金券数量，以分为单位</param>
        ///// <returns></returns>
        //[OperationContract]
        //ReturnCouponResult ReturnCoupon(long userId, long orderId, int CouponNum);

        ///// <summary>
        ///// 扣现金券
        ///// </summary>
        ///// <param name="userId"></param>
        ///// <param name="orderId"></param>
        ///// <param name="CouponNum"></param>
        ///// <returns></returns>
        //[OperationContract]
        //ReturnCouponResult SubtractCoupon(long userId, long orderId, int CouponNum);

        /// <summary>
        /// 用户可用现金券总额
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [OperationContract]
        int GetUserCouponSum(long userId);

        /// <summary>
        /// 获得现金列表
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [OperationContract]
        List<OriginCoupon> GetCashCouponList(long userId);

        /// <summary>
        /// VIP续费
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        [OperationContract]
        bool ReNewVIPAfterPayment(long userID);

        /// <summary>
        /// 获得我的现金券列表
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [OperationContract]
        List<AcquiredCoupon> GetAcquiredCouponList2(long userId, bool? isExpired);
        #endregion

        #region 使用券

        [OperationContract]
        //        获取指定产品类型指定金额下，所有可用的券（订单确认页使用，暂时可以不分页）
        List<UserCouponItemInfoEntity> GetCanUseCouponInfoListForOrder(OrderUserCouponRequestParams req);
        //获取指定产品类型指定金额下，所有不可用的券（订单确认页使用，暂时可以不分页）
        [OperationContract]
        List<UserCouponItemInfoEntity> GetCannotUseCouponInfoListForOrder(OrderUserCouponRequestParams req);

        [OperationContract]
        //获取指定产品类型指定金额下，默认最优惠的券（订单确认页需要默认选择一个券）        
        UserCouponItemInfoEntity GetTheBestCouponInfoForOrder(OrderUserCouponRequestParams req);

        #endregion

        #region  使用代金券
        [OperationContract]
        List<UserCouponItemInfoEntity> GetCanUseVoucherInfoListForOrder(OrderUserCouponRequestParamsBase req);

        [OperationContract]
        List<UserCouponItemInfoEntity> GetCannotUseVoucherInfoListForOrder(OrderUserCouponRequestParamsBase req);

        #endregion

        [OperationContract]
        CouponActivitySKURelEntity GetCouponActivitySKURelBySKUID(int SKUID);

        /// <summary>
        /// 根据skuid 获取未关闭的房券
        /// </summary>
        /// <param name="SKUID"></param>
        /// <returns></returns>
        [OperationContract]
        CouponActivityEntity GetCouponActivityBySKUID(int SKUID);

        /// <summary>
        /// 根据sourceid 获取未关闭的房券
        /// </summary>
        /// <param name="SKUID"></param>
        /// <returns></returns>
        [OperationContract]
        List<CouponActivityEntity> GetCouponActivityBySourceID(int sourceID);


        [OperationContract]
        List<CouponActivitySKURelEntity> GetCouponActivitySKURelByActivityID(int ActivityID);


        [OperationContract]
        bool UpdOldVIPtoNewVIP(long userId);
        [OperationContract]
        bool Del6MVIP(long userId);

        [OperationContract]
        int UpdateExchangeCouponForSMSAlert(int ID, int SMSAlertType);

        [OperationContract]
        int UpdateExchangeCouponForPhotoUrl(int ID, string PhotoUrl);

        [OperationContract]
        List<ExchangeCouponEntity> GetExchangeCouponEntityListByIDList(List<int> IDList);

        [OperationContract]
        List<ExchangeCouponEntity> GetExchangeCouponEntityListBySKUID(int skuid, int state);

        [OperationContract]
        List<ExchangeCouponEntity> GetExchangeCouponEntityListNoJoinBookBySKUID(int skuid, int state);

        [OperationContract]
        AcquiredCoupon GetAcquiredCouponById(long id);

        [OperationContract]
        string GetOriginGUIDByOrderAndTypeId(long sourceId, long typeId);

        [OperationContract]
        int GetAcquiredCouponByPhone(string phoneNo, string guid);

        [OperationContract]
        OriginCoupon GetCashCoupon(long id, string guid);//获得原始红包

        //  [System.ServiceModel.OperationContractAttribute()]
        //int InsertCouponTypeDefine(CouponTypeDefineEntity p);

        //[System.ServiceModel.OperationContractAttribute()]
        //int UpdateCouponTypeDefine(CouponTypeDefineEntity p);   

        [System.ServiceModel.OperationContractAttribute()]
        List<CouponTypeDefineEntity> GetCouponTypeDefine();


        [System.ServiceModel.OperationContractAttribute()]
        CouponTypeDefineEntity GetCouponTypeDefineByID(int ID);

        [System.ServiceModel.OperationContractAttribute()]
        CouponTypeDefineEntity GetCouponTypeDefineByCode(CouponActivityCode code);

        [System.ServiceModel.OperationContractAttribute()]
        List<OriginCoupon> GetUserOrgCouponInfoByType(long userId, int typeID);

        [OperationContract]
        int InsertInspectorReward(InspectorRewardEntity ire);

        [OperationContract]
        int UpdateInspectorReward(InspectorRewardEntity ire);

        [OperationContract]
        List<InspectorRewardItem> GetInspectorRewardItemList(long userid);

        [OperationContract]
        List<AcquiredCoupon> GetAcquireCouponRecordByUserID(long userid);

        [OperationContract]
        List<UseCouponRecordEntity> GetUseCouponRecordByUserID(long userid);

        [OperationContract]
        OriginCouponResult GenerateOriginCouponEx(long userId, long sourceID, CouponActivityCode code, string phoneNo);

        [OperationContract]
        OriginCoupon GetOriginCouponByTypeAndSourceID(long sourceId, int typeId);

        [OperationContract]
        string SomeFailingOperation();

        [OperationContract]
        int InsertCouponActivity(CouponActivityEntity cae);

        [OperationContract]
        int UpdateCouponActivity(CouponActivityEntity cae);

        [OperationContract]
        int UpdateCouponActivityNoUpRel(CouponActivityEntity cae);

        [OperationContract]
        CouponActivityEntity GetOneCouponActivity(int id, bool isLock);

        [OperationContract]
        CouponActivityEntity GetCouponActivityByBizIdAndBizType(int bizId, int bizType);

        [OperationContract]
        List<CouponActivityEntity> GetCouponActivityList(CouponActivityQueryParam param, out int totalCount);

        /// <summary>
        /// 根据skuid 获取couponactivity 信息
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        List<CouponActivityEntity> GetCouponActivityBySKUIDSList(CouponActivityQueryParam param, out int totalCount);

        [OperationContract]
        List<CouponActivityEntity> MemberCouponActivityList(CouponActivityQueryParam param, out int totalCount);

        [OperationContract]
        int AdjustPriceForPackageSKU(int payid);


        [OperationContract]
        List<CouponActivityEntity> MemberRetailCouponActivityList(CouponActivityQueryParam param, out int totalCount);

        [OperationContract]
        CouponActivityEntity GetToDayCouponActivity();


        [OperationContract]
        CouponActivityEntity GetToDayCouponActivityAndSKU();

        [OperationContract]
        CouponActivityEntity GetOneCouponActivityAndSKU(int id, bool isLock);

        /// <summary>
        /// 由用户ID 获得指定类型的券
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="activityType"></param>
        /// <returns></returns>
        [OperationContract]
        List<ExchangeCouponEntity> GetExchangeCouponEntityListByUser(long userId, int activityType);

        /// <summary>
        /// 根据用户id，和产品类型parentid获取订单列表
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="cParentId"></param>
        /// <param name="start"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        [OperationContract]
        List<ExchangeCouponEntity> GetExchangCouponByCategoryId(long userId, int cParentId, int start, int count, int couponId = 0);

        /// <summary>
        /// 由用户手机号 获得指定类型的券
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="activityType"></param>
        /// <returns></returns>
        [OperationContract]
        List<ExchangeCouponEntity> GetExchangeCouponEntityListByPhone(string phone, int activityType);

        /// <summary>
        /// 根据groupid获取 指定类型的券
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="activityType"></param>
        /// <returns></returns>
        [OperationContract]
        List<ExchangeCouponEntity> GetExchangeCouponEntityListByGroupId(int groupid, int activityType);


        /// <summary>
        /// 取消末支付的券。 使用场景之一：用户第二次提交购买券，但前一次的同类券末支付时
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="activityType"></param>
        /// <returns></returns>  
        [OperationContract]
        int CancelUnPayExchangeCouponOrderByActivityIDAndUserID(long userId, int activityID, int SKUID);

        /// <summary>
        /// 插入兑换券记录
        /// </summary>
        /// <param name="ece"></param>
        /// <returns></returns>
        [OperationContract]
        int InsertExchangeCoupon(ExchangeCouponEntity ece);

        /// <summary>
        /// 更新兑换券记录
        /// </summary>
        /// <param name="ece"></param>
        /// <returns></returns>
        [OperationContract]
        int UpdateExchangeCoupon(ExchangeCouponEntity ece);
        /// <summary>
        /// CouponActivity与BizId的关系表
        /// </summary>
        /// <param name="couponactivitybizrel"></param>
        /// <returns></returns>
        [OperationContract]
        int AddCouponActivityBizRel(CouponActivityBizRelEntity couponactivitybizrel);

        [OperationContract]
        int AddOrUpdateCouponActivityBizRel(CouponActivityBizRelEntity couponactivitybizrel);

        /// <summary>
        /// 根据CouponActivityId 、BizID、 BitType 获取 活动关联表
        /// </summary>
        /// <param name="cid">CouponActivityId</param>
        /// <param name="bizID">BizID</param>
        /// <param name="bizType">BitType  1、对应的spuid</param>
        /// <returns></returns>
        [OperationContract]

        List<CouponActivityBizRelEntity> GetCouponActivityBizRelByCouponActivityIdOrBizID(int cid, int bizID, int bizType);

        /// <summary>
        /// CouponActivity与BizId的关系表
        /// </summary>
        /// <param name="couponactivitybizrel"></param>
        /// <returns></returns>
        [OperationContract]
        int UpdateCouponActivityBizRel(CouponActivityBizRelEntity couponactivitybizrel);

        [OperationContract]
        List<ExchangeCouponEntity> GetExchangeCouponPageList(int activityID, int state, int pageSize, int pageIndex, out int total);

        [OperationContract]
        List<ExchangeCouponEntity> GetExchangeOrderList(int pageSize, int pageIndex, int followOperation, int skuid, string skuName, string phoneNum, string supplierName, string thirdorderid, string exchangeNo, out int totalCount);

        [OperationContract]
        CouponOrderOperationEntity GetCouponOderOperationByCouponId(int couponId);

        [OperationContract]
        List<ExchangeCouponEntity> GetExchangeCouponEntityListByActivity(int activityID);

        [OperationContract]
        List<ExchangeCouponEntity> GetExchangeCouponEntityListByActivityIDAndUserID(int activityID, long userID);

        [OperationContract]
        int InsertCommOrders(CommOrderEntity coe);

        [OperationContract]
        CommOrderEntity GetOneCommOrderEntity(int idx);

        [OperationContract]
        List<ExchangeCouponEntity> GetExchangeCouponEntityByPayID(int payID);

        [OperationContract]
        List<ExchangeCouponEntity> GetExchangeCouponEntityByCID(long CID);


        [OperationContract]
        int BindOrderAndExchangeCoupon(long orderID, string exchangeNo, string phoneNo, long updator, int state = 3);

        /// <summary>
        /// 获得订单使用的房券
        /// </summary>
        /// <param name="orderID">订单长ID</param>
        /// <returns></returns>
        [OperationContract]
        List<ExchangeCouponEntity> GetUsedCouponByOrderID(long orderID);

        /// <summary>
        /// 获取不重复的id
        /// </summary>
        /// <param name="tablename"></param>
        /// <returns></returns>
        [OperationContract]
        Int64 GetNextId(NextIdType tablename);
        /// <summary>
        /// 更新exchangeno
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [OperationContract]
        int UpdateExchangeNO(ExchangeCouponEntity param);

        /// <summary>
        /// 自动退钱 券过期了
        /// </summary>
        /// <param name="couponID"></param>
        /// <returns></returns>
        [OperationContract]
        int CouponRefund(int couponID);

        /// <summary>
        /// 获取用户某个套餐可用的房券列表
        /// </summary>
        /// <param name="couponID"></param>
        /// <returns></returns> 
        [OperationContract]
        List<ExchangeCouponEntity> GetExchangeCouponListByUserIDSourceID(Int64 UserID, Int64 SourceID);

        /// <summary>
        /// 获取所有过期待取消并退款的券列表
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        List<ExchangeCouponEntity> GetWaitingRefundCouponList();

        /// <summary>
        /// 归还订单消费的券
        /// </summary>
        /// <param name="orderID"></param>
        /// <returns></returns>
        [OperationContract]
        int ReturnOrderConsumedCoupon(long orderID, long updator);

        /// <summary>
        /// 更新券的状态 由于支付超时
        /// </summary>
        /// <param name="activityID"></param>
        /// <returns></returns>
        [OperationContract]
        int UpdateCouponState4TimeOut(int activityID = 0);

        /// <summary>
        /// 从数据库获取券已锁定数量，用于计算还可购买数量
        /// </summary>
        /// <param name="activityID"></param>
        /// <returns></returns>
        [OperationContract]
        int GetActivityLockedCount(int activityID);

        /// <summary>
        /// 支付类型和状态
        /// </summary>
        /// <param name="payType"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        [OperationContract]
        List<RefundCouponsEntity> GetRefundCouponsList(RefundCouponsQueryParam param, out int count);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hour"></param>
        /// <returns></returns>
        [OperationContract]
        List<WillCloseProductGroupEntity> GetWillCloseProductGroup(int GroupCount, int SKUID, int Hour);

        [OperationContract]
        int UpdateRemark(int couponid, string remark);
        [System.ServiceModel.OperationContractAttribute()]
        List<DistrictInfoForWxappEntity> GetRoomCouponDistrictInfoForWxapp(List<int> districtIds);

        [System.ServiceModel.OperationContractAttribute()]
        List<DistrictInfoForWxappEntity> GetRoomCouponDistrictInfoForLatLngWxapp(List<int> districtIds, double lat = 0, double lng = 0, int geoScopeType = 3, bool inchina = true);

        /// <summary>
        /// 主要更新待退券的后台状态为已退款
        /// </summary>
        /// <param name="rce"></param>
        /// <returns></returns>
        [OperationContract]
        int UpdateRefundCoupon(RefundCouponsEntity rce);


        [OperationContract]
        int UpdateRefundCouponForPartRefund(RefundCouponsEntity rce);

        /// <summary>
        /// 主要添加待退券进入退款列表 如果支付类型是支付宝 无需插入HotelDB Refund表
        /// </summary>
        /// <param name="rce"></param>
        /// <returns></returns>
        [OperationContract]
        int AddRefundCoupon(RefundCouponsEntity rce);

        /// <summary>
        /// 主要添加待退券进入退款列表 如果支付类型是支付宝 无需插入HotelDB Refund表
        /// </summary>
        /// <param name="rce"></param>
        /// <returns></returns>
        [OperationContract]
        int CancelRefundCoupon(RefundCouponsEntity rce);

        /// <summary>
        /// 由券ID和券码 获得相关的券信息
        /// </summary>
        /// <param name="couponID">券ID</param>
        /// <param name="exchangeNo">券码</param>
        /// <returns></returns>
        [OperationContract]
        ExchangeCouponEntity GetOneExchangeCouponInfo(int couponID, string exchangeNo);

        /// <summary>
        /// 获取房券的兑换价格列表
        /// </summary>
        /// <param name="couponActivity"></param>
        /// <returns></returns>
        [OperationContract]
        IEnumerable<CouponRateEntity> GetCouponRateEntityList(int couponActivity);

        /// <summary>
        /// 更新房券 兑换价格列表
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [OperationContract]
        int UpdateCouponRateEntity(IEnumerable<CouponRateEntity> param);

        /// <summary>
        /// 删除房券价格
        /// </summary>
        /// <param name="cre"></param>
        /// <returns></returns>
        [OperationContract]
        int DeleteCouponRateEntity(CouponRateEntity cre);

        /// <summary>
        /// 由活动ID获取券ID(仅房券内容)
        /// </summary>
        /// <param name="activityID"></param>
        /// <returns></returns>
        //[OperationContract]
        //IEnumerable<ExchangeCouponEntity> GetExchangeCouponList(int activityID);

        /// <summary>
        /// 获取指定用户购买的指定SKU的券记录
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="skuid"></param>
        /// <param name="promotionId"></param>
        /// <returns></returns>
        [OperationContract]
        List<ExchangeCouponEntity> GetExchangeCouponListByUserSKU(long userId, int skuid, int promotionId);


        /// <summary>
        /// 获取指定用户购买的指定SKU的券记录
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="skuid"></param>
        /// <returns></returns>
        [OperationContract]
        List<ExchangeCouponEntity> GetExchangeCouponListByUserIDAndSKU(long userId, int skuid);

        /// <summary>
        /// copy到某个房券信息
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="updator"></param>
        /// <param name="merchantCode"></param>
        /// <returns></returns>
        [OperationContract]
        int CopyCouponActivity(int activityId, long updator, CouponActivityMerchant merchantCode);

        /// <summary>
        /// 获取要结算的券
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        List<ExchangeCouponEntity> GetNeedSettlementBotaoCouponList();

        /// <summary>
        /// 获得需要取消的成员列表
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        List<long> GetNeedCancelMemberUserList();

        [OperationContract]
        int IsVipNoPayReserveUser(string userid);

        /// <summary>
        /// 获取指定userid、typeid=8的originCoupon数据
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        [OperationContract]
        OriginCoupon GetOriginCouponByUserIdForT8(long userid);


        [OperationContract]
        int AddVoucherChannel(VoucherChannelEntity voucherchannel);

        [OperationContract]
        int UpdateVoucherChannel(VoucherChannelEntity voucherchannel);

        [System.ServiceModel.OperationContractAttribute()]
        int AddVoucherDefine(VoucherDefineEntity voucherdefine);

        [System.ServiceModel.OperationContractAttribute()]
        int UpdateVoucherDefine(VoucherDefineEntity voucherdefine);

        [System.ServiceModel.OperationContractAttribute()]
        int AddVoucherItems(VoucherItemsEntity voucheritems);

        [System.ServiceModel.OperationContractAttribute()]
        int UpdateVoucherItems(VoucherItemsEntity voucheritems);

        [System.ServiceModel.OperationContractAttribute()]
        List<VoucherDefineEntity> GetVoucherDefineList(int idx, string name);

        [System.ServiceModel.OperationContractAttribute()]
        List<VoucherChannelEntity> GetVoucherChanneList(int idx, string name, int defineid);

        [System.ServiceModel.OperationContractAttribute()]
        List<VoucherChannelEntity> GetVoucherChanneByCode(string code);

        [System.ServiceModel.OperationContractAttribute()]
        List<VoucherItemsEntity> GetVoucherItemByVoucherChannelid(int channelid);

        [System.ServiceModel.OperationContractAttribute()]
        int AddUsedConsumerCouponInfo(UsedConsumerCouponInfoEntity usedconsumercouponinfo);

        [System.ServiceModel.OperationContractAttribute()]
        int UpdateUsedConsumerCouponInfo(UsedConsumerCouponInfoEntity usedconsumercouponinfo);

        [System.ServiceModel.OperationContractAttribute()]
        int DelUsedConsumerCouponInfo(int id);

        [System.ServiceModel.OperationContractAttribute()]
        int UpdateExchangeState(ExchangeCouponEntity param);

        [System.ServiceModel.OperationContractAttribute()]
        int UpdateOperationState(ExchangeCouponEntity param);

        [System.ServiceModel.OperationContractAttribute()]
        int UpdateTravelIDs(ExchangeCouponEntity param);

        [System.ServiceModel.OperationContractAttribute()]
        int UpdateOperationRemark(int ID, string Remark);

        [System.ServiceModel.OperationContractAttribute()]
        List<UsedCouponProductEntity> GetUsedCouponProductBySupplierId(int supplierId, DateTime startTime, DateTime endTime, int start = 0, int count = 20);

        /// <summary>
        /// 查询已预约未核销的信息
        /// </summary>
        /// <param name="supplierId">供应商id</param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="state">券的状态</param>
        /// <returns></returns>
        [System.ServiceModel.OperationContractAttribute()]
        List<BookNoUsedExchangeCouponInfoEntity> GetBookNoUsedExchangeCouponBySupplierId(int supplierId, DateTime startTime, DateTime endTime, int state);

        [System.ServiceModel.OperationContractAttribute()]
        UsedConsumerCouponInfoEntity GetUsedCouponProductByExchangeNo(string exchangeNo);

        [System.ServiceModel.OperationContractAttribute()]
        int GetUserCouponByCategoryParentId(long userId, int cParentId);

        /// <summary>
        /// 获取指定SKUID的消费券产品列表
        /// </summary>
        /// <param name="skuids">String可包含多个SKUID，英文逗号间隔</param>
        /// <returns></returns>
        [System.ServiceModel.OperationContractAttribute()]
        List<SKUCouponActivityEntity> GetSKUCouponActivityListBySKUIds(string skuids);


        [System.ServiceModel.OperationContractAttribute()]
        List<CouponActivityWithSKUEntity> GetCouponActivityListBySKUIds(List<int> skuids);


        /// <summary>
        /// 获取指定产品专辑的消费券列表
        /// </summary>
        /// <param name="albumid"></param>
        /// <param name="start"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        [System.ServiceModel.OperationContractAttribute()]
        List<SKUCouponActivityEntity> GetSKUCouponActivityListByAlbumId(int albumid, int start, int count, int districtID, out int totalCount);


        /// <summary>
        /// 遛娃小程序首页产品列表
        /// </summary>
        /// <param name="albumid"></param>
        /// <param name="start"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        [System.ServiceModel.OperationContractAttribute()]
        List<SKUAlbumEntity> GetSKUAlbumEntityListByAlbumId(int albumId, int start, int count, out int totalCount);


        [System.ServiceModel.OperationContractAttribute()]
        ProductAlbumSumEntity GetProductAlbumSum(int albumid);

        /// <summary>
        /// 用户没有购买首单权限时 获取指定产品专辑的消费券列表
        /// </summary>
        /// <param name="albumid"></param>
        /// <param name="start"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        [System.ServiceModel.OperationContractAttribute()]
        List<SKUCouponActivityEntity> GetOldVIPSKUCouponActivityListByAlbumId(int albumid, int start, int count, int districtID, out int totalCount);


        [System.ServiceModel.OperationContractAttribute()]
        List<SKUCouponActivityEntity> SKUCouponActivityByCategory(int category = 14, int districtID = 2, double lat = 0, double lng = 0, int geoScopeType = 3, int start = 0, int count = 15, int sort = 0, int payType = 0, double locLat = 0, double locLng = 0);

        [System.ServiceModel.OperationContractAttribute()]
        int SKUCouponActivityByCategoryCount(int category = 14, int districtID = 2, double lat = 0, double lng = 0, int geoScopeType = 3, int payType = 0);

        [System.ServiceModel.OperationContractAttribute()]
        int UpdateExchangeCouponSettle(string ids, string code);

        [System.ServiceModel.OperationContractAttribute()]
        List<ExchangeCouponEntity> GetExchangeListByNO(string codes, int type);


        [System.ServiceModel.OperationContractAttribute()]
        List<ExchangeSettleModel> GetExchangeCheckByNO(string codes);

        [System.ServiceModel.OperationContractAttribute()]
        List<ExchangeCouponForSettleEntity> GetExchangeCouponSettleList(int AID, int state, string startdate, string enddate, int pageSize, int pageIndex, out int total);

        [System.ServiceModel.OperationContractAttribute()]
        int AddRedShare(RedShareEntity redshare);

        [System.ServiceModel.OperationContractAttribute()]
        int UpdateRedShare(RedShareEntity redshare);

        [System.ServiceModel.OperationContractAttribute()]
        List<RedShareEntity> GetRedShareList(int pageIndex, int pageSize, out int TotalCount);

        [System.ServiceModel.OperationContractAttribute()]
        List<RedShareEntity> GetRedShareEntityByGUID(string guid);

        [System.ServiceModel.OperationContractAttribute()]
        int AddRetailProduct(RetailProductEntity retailproduct);

        [System.ServiceModel.OperationContractAttribute()]
        int UpdateRetailProduct(RetailProductEntity retailproduct);

        [System.ServiceModel.OperationContractAttribute()]
        List<RetailProductEntity> GetRetailProductById(int id);

        [System.ServiceModel.OperationContractAttribute()]
        List<CouponActivityRetailEntity> GetRetailProductList(int id, int relBizId, int state, int count, int start, out int totalCount);

        [System.ServiceModel.OperationContractAttribute()]
        List<CouponActivityRetailEntity> GetSKURetailProductList(int count, int start, out int totalCount);

        [System.ServiceModel.OperationContractAttribute()]
        CouponActivityRetailDetailEntity GetRetailProductInfoByIDAndCID(int ID, long CID);

        [System.ServiceModel.OperationContractAttribute()]
        List<SKUCouponActivityEntity> GetSKUListByActivityId(int activityId);

        [System.ServiceModel.OperationContractAttribute()]
        int CopyRetail(int id, long updator);

        [System.ServiceModel.OperationContractAttribute()]
        int ProductAndCouponCopyRetail(int id, long updator);

        [System.ServiceModel.OperationContractAttribute()]
        int AddRetailUrl(RetailUrlEntity retailurl);

        [System.ServiceModel.OperationContractAttribute()]
        int AddSupplierCoupon(SupplierCouponEntity suppliercoupon);

        [System.ServiceModel.OperationContractAttribute()]
        int UpdateSupplierCoupon(SupplierCouponEntity suppliercoupon);

        [System.ServiceModel.OperationContractAttribute()]
        List<SupplierCouponEntity> GetTopCountSupplierCouponBySupplierID(int count, int supplierID, int state);

        [System.ServiceModel.OperationContractAttribute()]
        List<SupplierCouponEntity> GetSupplierCouponInfo(int supplierid);


        [System.ServiceModel.OperationContractAttribute()]
        List<CouponActivityRetailEntity> GetSearchProductList(SearchProductRequestEntity param);

        [System.ServiceModel.OperationContractAttribute()]
        List<CouponActivityRetailEntity> GetSearchProductListByCategory(SearchProductRequestEntity param);

        [System.ServiceModel.OperationContractAttribute()]
        int GetSearchProductListByCategoryCount(SearchProductRequestEntity param);


        [System.ServiceModel.OperationContractAttribute()]
        int GetSearchProductListCount(SearchProductRequestEntity param);


        [System.ServiceModel.OperationContractAttribute()]
        int AddUserCouponConsumeLog(UserCouponConsumeLogEntity usercouponconsumelog);

        [System.ServiceModel.OperationContractAttribute()]
        int UpdateUserCouponConsumeLog(UserCouponConsumeLogEntity usercouponconsumelog);

        [System.ServiceModel.OperationContractAttribute()]
        int AddUserCouponDefine(UserCouponDefineEntity usercoupondefine);

        [System.ServiceModel.OperationContractAttribute()]
        int UpdateUserCouponDefine(UserCouponDefineEntity usercoupondefine);

        [System.ServiceModel.OperationContractAttribute()]
        int AddUserCouponItem(UserCouponItemEntity usercouponitem);

        [System.ServiceModel.OperationContractAttribute()]
        int UpdateUserCouponItem(UserCouponItemEntity usercouponitem);

        [System.ServiceModel.OperationContractAttribute()]
        int SendUserCouponItem(List<long> userIds, int couponDefineId, long curUserId);

        [System.ServiceModel.OperationContractAttribute()]
        bool SendOneUserCouponItemOnlyOneTime(long userId, int couponDefineId, long curUserId);

        [System.ServiceModel.OperationContractAttribute()]
        int SendOneUserCouponItem(long userId, int couponDefineId, long curUserId);

        [System.ServiceModel.OperationContractAttribute()]
        List<UserCouponDefineEntity> GetUserCouponDefineListByType(int type);

        [System.ServiceModel.OperationContractAttribute()]
        UserCouponDefineEntity GetUserCouponDefineByID(int id);

        [System.ServiceModel.OperationContractAttribute()]
        List<UserCouponDefineEntity> GetUserCouponDefineListByIds(string ids, long userId);

        [System.ServiceModel.OperationContractAttribute()]
        int GetUserCouponItemByUserId(long userId, int couponDefineId);
        /// <summary>
        /// 只适用满减券和立减券（即：type=0 or type = 1）
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        [System.ServiceModel.OperationContractAttribute()]
        List<UserCouponItemInfoEntity> GetUserCouponInfoListByUserId(long userId, UserCouponState state);


        /// <summary>
        /// 用户新VIP欢迎礼券
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [System.ServiceModel.OperationContractAttribute()]
        List<UserCouponItemInfoEntity> GetNewVIPGiftUserCouponInfoListByUserId(long userId);

        /// <summary>
        /// 批量更新现金券状态
        /// </summary>
        /// <param name="IDs"></param>
        /// <param name="CouponState"></param>
        /// <returns></returns>
        [System.ServiceModel.OperationContractAttribute()]
        bool UpdateUserCoupoinItemByIDs(List<Int32> IDs, Int32 CouponState);

        [System.ServiceModel.OperationContractAttribute()]
        List<UserCouponDefineEntity> GetCouponDefineByIntval(int intval, CondationType condationtype);


        [System.ServiceModel.OperationContractAttribute()]
        int GetUserCouponItemByUserIdAndCouponDefineId(long userId, int couponDefineId);

        /// <summary>
        /// 只适用满减券和立减券（即：type=0 or type = 1）
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        [System.ServiceModel.OperationContractAttribute()]
        List<UserCouponItemInfoEntity> GetUserCouponInfoAllListByUserId(long userId);

        [System.ServiceModel.OperationContractAttribute()]
        int GetUserCouponInfoCountByUserId(int userId, UserCouponState state);

        [System.ServiceModel.OperationContractAttribute()]
        UserCouponItemInfoEntity GetUserCouponItemInfoByID(int id);

        [System.ServiceModel.OperationContractAttribute()]
        UserCouponItemEntity GetUserCouponItemByID(int id);

        [System.ServiceModel.OperationContractAttribute()]
        RequestResultEntity UseUserCouponInfoItem(UseCashCouponItem param);

        [System.ServiceModel.OperationContractAttribute()]
        RequestResultEntity CancelUseUserCouponInfoItem(UseCashCouponItem param);

        [System.ServiceModel.OperationContractAttribute()]
        List<UserCouponConsumeLogEntity> GetUserCouponLogByCouponItemID(int idx);

        [System.ServiceModel.OperationContractAttribute()]
        bool GetProductAlbumSKUBySKUIDAndAlbumId(int skuid, int albumid);

        [System.ServiceModel.OperationContractAttribute()]
        int AddUserCouponUseCondation(UserCouponUseCondationEntity usercouponusecondation);

        [System.ServiceModel.OperationContractAttribute()]
        int UpdateUserCouponUseCondation(UserCouponUseCondationEntity usercouponusecondation);

        [System.ServiceModel.OperationContractAttribute()]
        int DeleteUserCouponUseCondation(int idx);

        [OperationContract]
        List<UserCouponUseCondationEntity> GetCouponCondationByCouponDefineId(int CouponDefineId);

        [OperationContract]
        List<UserCouponUseCondationEntity> GetCouponCondationByIntVal(int intVal, int condationType);

        [OperationContract]
        List<UserCouponDefineEntity> GetCouponDefineByIntVals(string intVals, int condationType, long userId);

        [OperationContract]
        List<UserCouponItemInfoEntity> GetUserCouponInfoListByUserIdAndType(long userId, UserCouponState state, UserCouponType type);

        [OperationContract]
        int GetUserCouponInfoCountByUserIdAndType(long userId, UserCouponState state, UserCouponType type);

        [System.ServiceModel.OperationContractAttribute()]
        int AddCouponOrderOperation(CouponOrderOperationEntity couponorderoperation);

        [System.ServiceModel.OperationContractAttribute()]
        int UpdateCouponOrderOperation(CouponOrderOperationEntity couponorderoperation);

        [System.ServiceModel.OperationContractAttribute()]
        int AddRedPool(RedPoolEntity redpool);

        [System.ServiceModel.OperationContractAttribute()]
        int UpdateRedPool(RedPoolEntity redpool);

        [System.ServiceModel.OperationContractAttribute()]
        int AddRedPoolDetail(RedPoolDetailEntity redpooldetail);

        [System.ServiceModel.OperationContractAttribute()]
        int UpdateRedPoolDetail(RedPoolDetailEntity redpooldetail);

        [System.ServiceModel.OperationContractAttribute()]
        int AddRedActivity(RedActivityEntity redactivity);

        [System.ServiceModel.OperationContractAttribute()]
        int UpdateRedActivity(RedActivityEntity redactivity);

        [System.ServiceModel.OperationContractAttribute()]
        int AddRedRecord(RedRecordEntity redrecord);

        [System.ServiceModel.OperationContractAttribute()]
        int UpdateRedRecord(RedRecordEntity redrecord);

        [System.ServiceModel.OperationContractAttribute()]
        List<RedPoolEntity> GetRedPoolListByType(RedPoolType type);

        [System.ServiceModel.OperationContractAttribute()]
        RedPoolEntity GetRedPoolByID(int id);

        [System.ServiceModel.OperationContractAttribute()]
        List<RedPoolDetailEntity> GetRedDetailByPoolId(int poolId);

        [System.ServiceModel.OperationContractAttribute()]
        RedPoolDetailEntity GetRedDetailBylId(int Id);

        [System.ServiceModel.OperationContractAttribute()]
        List<RedActivityEntity> GetRedActivityListByType(RedActivityType type);

        [System.ServiceModel.OperationContractAttribute()]
        RedActivityEntity GetRedActivityById(int id);


        [System.ServiceModel.OperationContractAttribute()]
        RedActivityEntity GetRedActivityByGUID(string guid);


        [System.ServiceModel.OperationContractAttribute()]
        RedRecordEntity GetRedRecordByRedActivityIdAndPhone(int activityId, string phone);


        [System.ServiceModel.OperationContractAttribute()]
        List<RedRecordEntity> GetRedRecordByActivityId(int activityId, int count, int start, out int totalCount);

        [System.ServiceModel.OperationContractAttribute()]
        RedRecordEntity GetRedRecordById(int id);

        [System.ServiceModel.OperationContractAttribute()]
        List<RedPoolEntity> GetRedPoolByOrderPrice(decimal price);


        [System.ServiceModel.OperationContractAttribute()]
        List<RedActivityEntity> GetRedActivityByBizIDAndBizType(long bizId, RedActivityType bizType);

        [System.ServiceModel.OperationContractAttribute()]
        int UpdateExchangeSettlePrice(decimal SettlePrice, int ExchangeID, long curuserID, string OperationRemark = "", int supplierID = 0);



        [System.ServiceModel.OperationContractAttribute()]
        string GetThirdCouponOrderID(int couponid, int type);

        [System.ServiceModel.OperationContractAttribute()]
        int Insert2Refund(RefundCouponsEntity rce);


        [System.ServiceModel.OperationContractAttribute()]
        int UpdateThirdPartyRefundCoupon(int couponid, int thirdstate, int couponRefundstate);

        [System.ServiceModel.OperationContractAttribute()]
        int UpdateCouponActivityPicPath(int activityid, string picpath);

        [System.ServiceModel.OperationContractAttribute()]
        string GetCouponActivityPicBySPUID(int spuid);

        [OperationContract]
        List<ExchangeCouponEntity> GetExchangeOrderListBySupplierID(int supplierID);
        /// <summary>
        /// 根据CouponId，获取订单列表
        /// </summary>
        /// <param name="couponId"></param>
        [OperationContract]
        ExchangeCouponEntity GetExchangCouponByCouponId(int couponId);

        /// <summary>
        /// 根据券订单id，获取订单
        /// </summary>
        /// <param name="couponOrderID"></param>
        /// <returns></returns>
        [OperationContract]
        List<ExchangeCouponEntity> GetExchangCouponByCouponOrderID(long couponOrderID);

        /// <summary>
        /// 根据couponOrderId和状态获取订单数量
        /// </summary>
        /// <param name="couponOrderID"></param>
        /// <param name="state">订单状态</param>
        /// <returns></returns>
        [OperationContract]
        int GetExchangCouponCountByCouponOrderID(long couponOrderID, int state);
        /// <summary>
        /// 获取过期不退款券数据
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        List<ExchangeCouponEntity> GetExpiredNotRefundExchangeCouponList();
        /// <summary>
        /// 更新过期不退款券数据
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        int UpdateExchangeCouponExpiredNotRefund();
        /// <summary>
        /// 根据couponId修改备注
        /// </summary>
        /// <param name="couponId"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        [System.ServiceModel.OperationContractAttribute()]
        int UpdateOperationRemarkByCouponId(int couponId, string remark);

        /// <summary>
        /// 根据产品所属分类标签ProductTagID，获取消费券
        /// </summary>
        /// <param name="category"></param>
        /// <param name="districtID"></param>
        /// <param name="lat"></param>
        /// <param name="lng"></param>
        /// <param name="geoScopeType"></param>
        /// <param name="start"></param>
        /// <param name="count"></param>
        /// <param name="sort"></param>
        /// <param name="payType"></param>
        /// <param name="locLat"></param>
        /// <param name="locLng"></param>
        /// <returns></returns>
        [System.ServiceModel.OperationContractAttribute()]
        List<SKUCouponActivityEntity> SKUCouponActivityByID(int ID = 0, int districtID = 2, double lat = 0, double lng = 0, int geoScopeType = 3, int start = 0, int count = 15, int sort = 0, int payType = 0, double locLat = 0, double locLng = 0);
        /// <summary>
        /// 修改第三方券码状态
        /// </summary>
        /// <param name="suppliercoupon"></param>
        /// <returns></returns>
        [System.ServiceModel.OperationContractAttribute()]
        int UpdateSupplierCouponState(SupplierCouponEntity suppliercoupon, int couponID = 0);

        [System.ServiceModel.OperationContractAttribute()]
        UserCouponUseCondationEntity GetUserCouponUseCondationEntity(int idx);

        [System.ServiceModel.OperationContractAttribute()]
        List<UserCouponDefineEntity> GetCouponCondationByMarketingType(int intval, int condationtype, int marketingType);
    }
}