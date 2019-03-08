using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace HJD.CouponService.Contracts.Entity
{
    public enum ExchangeCouponState : sbyte
    {
        submit = 1,//: 已提交未支付 
        paied = 2,//：已支付未兑换 
        exchanged = 3, //: 已兑换 
        cancel = 4, //：已取消 
        refund = 5,//：已退款 
        paiedOverTime = 6,//：超时支付 
        refunding = 7 //：处于待退款状态
    }
    public enum OpLogBizType
    {
        HotelInfo = 1,
        sightInfo = 2,
        HotelSightRel = 3,
        RestaurantInfo = 4,
        HotelRestRel = 5,
        HotelContacts = 6,
        PackProduct = 9,
        TravelPerson = 10,
        PackageInfo = 11,
        PrePay = 12,
        RoomInfo = 13,
        PackageItem = 14,
        PRate = 15,
        RetailRate = 16,
        CanSaleDate = 17,
        RoomBedState = 18,
        EnsureRoom = 19,   // 包房
        DicCode = 20,
        CouponActivity200 = 21,  //抢券活动
        CouponActivity300 = 22,    //团券活动
        CouponActivity600 = 23   //消费券活动
    }
    public enum OpLogBizOpType
    {
        Add = 1,
        Update = 2,
        Del = 3
    }

    public enum CouponRefundState
    {
        NoRefund = 0, //无退款
        Log = 1,
        Done = 2
    }

    public enum CouponActivityBizRelBizType
    {
        SPUID = 1,
        ProductAlbum = 2
    }

    /// <summary>
    ///    DefaultSort = 默认排序
    ///    PriceAsc = 价格升序
    ///    PriceDesc = 价格降序
    ///    RewardAsc = 佣金升序
    ///    RewardDesc = 佣金降序
    /// </summary>
    public enum ProductSearchSort
    {
        DefaultSort = 0,
        ManuSellNum = 1,
        PriceAsc = 2,
        PriceDesc = 3,
        RewardAsc = 4,
        RewardDesc = 5
    }

    /// <summary>
    /// 筛选类型
    /// ExchangeCoupon200 = 房券
    /// ExchangeCoupon600 = 消费券
    /// </summary>
    public enum ProductSearchScreen
    {
        ExchangeCoupon200 = 0,
        ExchangeCoupon600 = 1
    }


    public enum CouponActivityType
    {
        闪购房券 = 200,
        //    团购房券 = 300,
        专辑房券 = 500,
        消费券 = 600
    }

    public enum MemberType
    {
        会员 = 400
    }

    /// <summary>
    /// 现金券类型
    /// </summary>
    public enum CouponActivityCode
    {
        zeta = 1,
        regist = 2,
        zmjd50 = 3,
        sharecomment = 4,
        yuehu = 5,
        oumeng = 6,
        cashcoupon = 7,
        cashcoupon50 = 8,
        cashcoupon200 = 12,
        upvip = 13,
        newVIPPresent = 17,  //新VIP礼
        redbag = 100
    }

    public enum CouponActivityMerchant
    {
        all,
        zmjd,
        bohuijinrong,
        retailer
    }

    public enum UserCouponState
    {
        //0： 记录 1：已使用 2： 过期 3：取消
        log = 0,
        used = 1,
        expired = 2,
        canceled = 3

    }

    public enum UserCouponType
    {
        DiscountOverPrice = 0,//满减
        DiscountUnconditional = 1, //立减
        DiscountVoucher = 2  //代金券
    }


    /// <summary>
    /// 现金券使用订单类型
    /// </summary>
    ///    
    [DataContract]
    public enum CashCouponOrderSorceType
    {
        [EnumMember]
        unknow = 0,
        [EnumMember]
        hotelPackage = 1,
        [EnumMember]
        sku = 2
    }

    /// <summary>
    /// 限制使用券类型
    /// </summary>
    public enum CondationType
    {
        sku = 1,
        hotelPackage = 2
    }

    public enum RedPoolType
    {
        Order = 1,   //订单
        NoOrder = 2  //非订单
    }
    public enum RedActivityType
    {
        all = 0,
        activityRed = 1,  //活动红包
        ExchangeOrderRed = 2, //券订单红包
        HotelOrderRed = 3      //酒店订单红包
    }

    public enum RedRecordType
    {
        DiscountOver = 0,//满减
        DiscountUnconditional = 1,//立减
        DiscountVoucher = 2,//代金券
        ExchangeCoupon = 3 //房券
    }

    public enum ExchangeOperationState
    {
        Over = 0,//完成并核销或不需要待处理
        TodoExchange = 1,//待处理券
        DateTodoExchange = 3, //指定日期进入待处理
        BookTodoExchange = 4, //预约后进入待处理
        DoneNotWriteOff = 20 //完成待核销
    }

    public enum NextIdType
    {
        OrderID = 0
    }

}