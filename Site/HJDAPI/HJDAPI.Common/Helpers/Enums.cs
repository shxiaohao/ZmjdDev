using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.Common.Helpers
{
    public class Enums
    {
         
        /// <summary>
        /// 产品类型 0：普通产品，1：大团购产品.订金  
        /// </summary>
        public enum SubSKUType
        {
            Normal = 0,
            BigGroup_Deposit = 1
        }


        /// <summary>
        /// 字典活动枚举
        /// </summary>
        public enum ActivityRedCashCouponType
        {
            redcashcouponlistnewuser = 1 
        }
        public enum GEOScopeType
        {
            District = 1, //按目的地查
            AroundCity = 2,//查城市周边
            ArooundUser = 3//用户坐标附近
        }

        public enum OTAUserName
        {
            一位携程会员 = 2,
            一位同程会员 = 4,
            一位驴评会员 = 25
        }

        /// <summary>
        /// 系统消息推送类型
        /// 等于1 表示 评论，涉及数据在hoteldb..CommentReviews
        /// 等于2 表示 回复,   涉及数据在hoteldb..CommentReviews   
        /// 等于3表示 点评有帮助， 涉及数据在hotelbizdb..CommentUseful
        /// 等于4表示 新点评 涉及数据在 hoteldb..comments
        /// 等于5表示 新关注 涉及数据在 [accountdb].[dbo].[FollowerFollowingRel]
        /// 等于6表示 新注册 涉及数据在 [HotelBizDB].[dbo].[points]  TypeId=12
        /// 等于7表示 评论得到积分 涉及数据在 [HotelBizDB].[dbo].[points]  TypeId=1
        /// 等于8表示 上传头像得到积分 涉及数据在 [HotelBizDB].[dbo].[points]  TypeId=10
        /// 等于9表示 消费得到积分 涉及数据在 [HotelBizDB].[dbo].[points]  TypeId=13
        /// 10 即将过期积分 涉及数据在 [HotelBizDB].[dbo].[ExpirePoints] 
        /// </summary>
        public enum SysMessigeTypeDescribe
        { 
            评论 = 1,
            回复 = 2,
            点赞 = 3,
            新点评 = 4,
            新关注 = 5,
            注册 = 6,
            评论得到积分 = 7,
            上传头像得到积分 = 8,
            消费积分 = 9,
            即将过期积分 =10,
            东航里程积分=11
        }
        public enum SysMessigeType
        {
            Comment = 1,
            Reply = 2,
            Fabulous = 3,
            NewComment = 4,
            NewFollow = 5,
            Register = 6,
            CommentPoints = 7,
            UploadUserAvatar = 8, 
            UserPayPoints = 9,
            ExpirePoints = 10,
            EasternAirLinesPoints=11
        }

        public enum enumOTAName
        {
            周末酒店 = 0,
            Booking = 1,
            携程 = 2,
            同程 = 4,
            艺龙 = 6,
            住哪 = 11,
            驴评 = 25,
            去哪儿 = 26,
            Agoda = 27,
            周末 = 102,
            大都市 = 104
        }

        public enum enumCardType
        {
            身份证 = 1,
            护照 = 2,
            户口簿 = 3,
            港澳通行证 = 4,
            台胞证 = 5,
            其他 = 10

        }

        public enum AppPhotoSize { applist, applist2, appdetail, appview, interestHotelList, share, small, w320h230, theme, appdetail1, jupiter, shop };

        public enum JumpUrlName { commentdetail, commentsharedetail }

        /// <summary>
        /// inspector:品鉴师
        /// botao:博涛金融产品用户
        /// vip:特别用户
        /// general:一般用户
        /// vip199:金牌会员
        /// vip599:铂金会员
        /// </summary>
        public enum CustomerType { general = 0, botao = 1, vip = 2, inspector = 3, vip199 = 4, vip599 = 5 , vip3M = 6, vip6M = 7, vip199nr = 8}

        public enum CustomerTypeDescribe { 普通会员 = 0, 铂涛会员 = 1, VIP会员 = 2, 品鉴师 = 3, 金牌VIP会员 = 4, 铂金VIP会员 = 5, 金牌VIP3M = 6, 金牌VIP6M = 7, 金牌VIP_NR = 8}

        /// <summary>
        /// 房券类型，400：VIP
        /// 消费券    600
        /// </summary>
        public enum CouponType { VIP = 400,ProductCoupon=600 }

        public enum CouponState { submit = 1, paied = 2 , consumed = 3 , cancel = 4,  refund = 5 , over_time_paied = 6, expired = 8  }

        /// <summary>
        /// all:所有支付渠道
        /// alipay:支付宝
        /// upay:U付
        /// tenpay:微信支付
        /// cmbpay 招商一网通
        /// chinapay 银联支付
        /// </summary>
        public enum PayChannelType { alipay, upay, tenpay, botao, cmbpay, chinapay }

        /// <summary>
        /// writecomment: 写点评
        /// pay:支付
        /// cancel:取消
        /// cancelpay:取消支付
        /// delete:删除
        /// addpay:补款
        /// </summary>
        public enum OrderOptionButton { writecomment, pay, cancel, cancelpay, delete, addpay }

        /// <summary>
        /// umeng通知 在App端点击的后续动作
        /// </summary>
        public enum UmengMessageAfterOpen { go_app, go_url, go_activity, go_custom }

        /// <summary>
        /// umeng 推送消息方式
        /// </summary>
        public enum UmengMessagePushType { broadcast, unicast, listcast, filecast, groupcast, customizedcast }

        /// <summary>
        /// umeng 推送内容展示方式
        /// 通知栏还是普通消息
        /// </summary>
        public enum UmengMessageDisplayType { notification, message }

        /// <summary>
        /// 第三方商户合作代码
        /// </summary>
        public enum ThirdPartyMerchantCode
        {
            zmhotel,
            bohuijinrong
        }

        /// <summary>
        /// 酒店详情页照片来源
        /// </summary>
        public enum HotelRelPicSource
        {
            all = 0,
            official = 1,
            customer = 2
        }

        /// <summary>
        /// 弹出框出现的页面范围
        /// </summary>
        public enum PopBoxTarget { commentdetail = 1, homepage = 2, hoteldetail = 3 }

        /// <summary>
        /// 发票类型
        /// </summary>
        public enum InvoiceType
        {
            代订房费 = 0, 服务费 = 1, 会务费 = 2, 住宿费 = 3,  旅游费 =4
        }

        /// <summary>
        /// 配送方式
        /// </summary>
        public enum ShippingType { 顺丰到付 = 0, EMS = 1, 圆通快递 = 2 }

        public enum RoleType
        { 
            客服=1,
            管理=2,
            结算=3
        }
        /// <summary>
        /// 属性类型
        /// </summary>
        public enum PropertyType
        {
            普通 = 1,
            城市 = 2,
            日期 = 3,
            大人小孩 = 4,
            拼团 = 5
        }

        public enum GroupState
        {
            进行中 = 0,
            成功 = 1,
            失败 = 2,
            未支付 = 3,
            已取消 = 4
        }
        /// <summary>
        ///    DefaultSort = 默认排序
        ///    PriceAsc = 价格升序
        ///    PriceDesc = 价格降序
        ///    RewardAsc = 佣金升序
        ///    RewardDesc = 佣金降序
        /// </summary>
        public enum ShopSearchSort
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
        public enum ShopSearchScreen
        {
            ExchangeCoupon200 = 0,
            ExchangeCoupon600 = 1
        }

        public enum OrdersType
        {
            ExchangeOrder = 2,
            HotelOrder = 3
        }

        /// <summary>
        /// 支付订单类型
        /// 1:酒店订单  2:酒店订单线下支付  3:度假众筹。。  200:房券 300 400：会员  500：专辑房券 600：消费券 10000：纸质发票费用
        /// </summary>
        public enum CommOrderType
        {
            roomOrder = 1,
            roomOrderOfflinePay = 2,
            roomCoupon = 200,
            memberOrder = 400,
            roomCouponForList = 500,
            couponOrder = 600,
            InvocePay = 10000

        }
    }

}