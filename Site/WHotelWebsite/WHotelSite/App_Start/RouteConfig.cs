using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace WHotelSite
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            /////
            // Portal
            routes.MapRoute(
                name: "网站首页",
                url: "Home{district}",
                defaults: new { controller = "Portal", action = "Home" },
                constraints: new { district = @"\d+" }
            );

            routes.MapRoute(
                name: "小屏幕网站首页",
                url: "HomeCity{district}/{geoScopeType}",
                defaults: new { controller = "Portal", action = "Home" },
                constraints: new { district = @"\d+", geoScopeType = @"\d+" }
            );

            routes.MapRoute(
                name: "关于我们",
                url: "aboutus",
                defaults: new { controller = "Portal", action = "Aboutus" }
            );

            routes.MapRoute(
               name: "我们的团队",
               url: "ourteam",
               defaults: new { controller = "Portal", action = "OurTeam" }
            );

            routes.MapRoute(
                name: "联系我们",
                url: "contactus",
                defaults: new { controller = "Portal", action = "Contactus" }
            );

            // Hotel
            routes.MapRoute(
                name: "酒店列表-城市及周边",
                url: "zone{city}",
                defaults: new { controller = "Hotel", action = "List", sctype = 3 },
                constraints: new { city = @"\d+" }
            );

            routes.MapRoute(
                name: "酒店列表-城市区域",
                url: "region{zoneid}",
                defaults: new { controller = "Hotel", action = "List", sctype = 1 },
                constraints: new { zoneid = @"\d+" }
            );

            routes.MapRoute(
                name: "酒店列表-城市",
                url: "city{city}",
                defaults: new { controller = "Hotel", action = "List", sctype = 1 },
                constraints: new { city = @"\d+" }
            );

            routes.MapRoute(
                name: "主题-酒店列表-城市及周边",
                url: "zone{city}/theme{interest}",
                defaults: new { controller = "Hotel", action = "List", sctype = 3 },
                constraints: new { city = @"\d+", interest = @"\d+" }
            );

            routes.MapRoute(
                name: "主题-酒店列表-城市区域",
                url: "region{zoneid}/theme{interest}",
                defaults: new { controller = "Hotel", action = "List", sctype = 1 },
                constraints: new { zoneid = @"\d+", interest = @"\d+" }
            );

            routes.MapRoute(
                name: "主题-酒店列表-城市",
                url: "city{city}/theme{interest}",
                defaults: new { controller = "Hotel", action = "List", sctype = 1 },
                constraints: new { city = @"\d+", interest = @"\d+" }
            );

            routes.MapRoute(
                name: "景区-酒店列表-城市及周边",
                url: "zone{city}/sight{interest}",
                defaults: new { controller = "Hotel", action = "List", sctype = 3 },
                constraints: new { city = @"\d+", interest = @"\d+" }
            );

            routes.MapRoute(
                name: "景区-酒店列表-城市区域",
                url: "region{zoneid}/sight{interest}",
                defaults: new { controller = "Hotel", action = "List", sctype = 1 },
                constraints: new { zoneid = @"\d+", interest = @"\d+" }
            );

            routes.MapRoute(
                name: "景区-酒店列表-城市",
                url: "city{city}/sight{interest}",
                defaults: new { controller = "Hotel", action = "List", sctype = 1 },
                constraints: new { city = @"\d+", interest = @"\d+" }
            );

            routes.MapRoute(
                name: "酒店列表-AroundCity",
                url: "cityaround{aroundcity}",
                defaults: new { controller = "Hotel", action = "List", sctype = 3 },
                constraints: new { aroundcity = @"\d+" }
            );

            routes.MapRoute(
                name: "主题-酒店列表-AroundCity",
                url: "cityaround{aroundcity}/theme{interest}",
                defaults: new { controller = "Hotel", action = "List", sctype = 3 },
                constraints: new { aroundcity = @"\d+", interest = @"\d+" }
            );

            routes.MapRoute(
                name: "主题-酒店列表-AroundCity 2",
                url: "wx/cityaround{aroundcity}/theme{interest}",
                defaults: new { controller = "Hotel", action = "List2", sctype = 3 },
                constraints: new { aroundcity = @"\d+", interest = @"\d+" }
            );

            routes.MapRoute(
                name: "景区-酒店列表-AroundCity",
                url: "aroundcity{aroundcity}/sight{interest}",
                defaults: new { controller = "Hotel", action = "List", sctype = 3 },
                constraints: new { aroundcity = @"\d+", interest = @"\d+" }
            );

            routes.MapRoute(
                name: "m-城市-主题-酒店列表",
                url: "m/city{city}",
                defaults: new { controller = "Hotel", action = "ListMobile" },
                constraints: new { city = @"\d+" }
            );

            routes.MapRoute(
                name: "m-国内外-地域-酒店列表",
                url: "m/inchina{inchina}",
                defaults: new { controller = "Hotel", action = "ListZoneMobile" },
                constraints: new { inchina = @"\d+" }
            );

            routes.MapRoute(
                name: "酒店详情",
                url: "hotel/{hotel}",
                defaults: new { controller = "Hotel", action = "Detail" },
                constraints: new { hotel = @"\d+" }
            );

            routes.MapRoute(
                name: "酒店照片",
                url: "hotel/{hotel}/pic",
                defaults: new { controller = "Hotel", action = "Photos" },
                constraints: new { hotel = @"\d+" }
            );

            routes.MapRoute(
                name: "酒店地图",
                url: "hotel/{hotel}/map",
                defaults: new { controller = "Hotel", action = "Map" },
                constraints: new { hotel = @"\d+" }
            );

            routes.MapRoute(
                name: "酒店攻略",
                url: "strategy{did}",
                defaults: new { controller = "Hotel", action = "Strategy" },
                constraints: new { did = @"\d+" }
            );

            routes.MapRoute(
                name: "酒店点评",
                url: "hotel/{hotel}/allreviews",
                defaults: new { controller = "Hotel", action = "Reviews" },
                constraints: new { hotel = @"\d+" }
            );

            routes.MapRoute(
                name: "酒店点评-特色点评",
                url: "hotel/{hotel}/f{feature}reviews",
                defaults: new { controller = "Hotel", action = "Reviews" },
                constraints: new { hotel = @"\d+" }
            );

            routes.MapRoute(
                name: "酒店套餐",
                url: "hotel/{hotel}/packages",
                defaults: new { controller = "Hotel", action = "Packages" },
                constraints: new { hotel = @"\d+" }
            );

            routes.MapRoute(
                name: "酒店套餐详情",
                url: "hotel/package/{pid}",
                defaults: new { controller = "Hotel", action = "Package" },
                constraints: new { pid = @"\d+" }
            );

            routes.MapRoute(
                name: "酒店专辑列表",
                url: "hotel/collection/{cid}",
                defaults: new { controller = "Hotel", action = "CollectionHotel" },
                constraints: new { cid = @"\d+" }
            );

            routes.MapRoute(
                name: "普通套餐专辑列表",
                url: "package/collection/{cid}",
                defaults: new { controller = "Hotel", action = "CollectionPackage" },
                constraints: new { cid = @"\d+" }
            );

            routes.MapRoute(
                name: "可兑换套餐专辑列表",
                url: "exchange/packages/{cid}",
                defaults: new { controller = "Coupon", action = "ExchangePackages" },
                constraints: new { cid = @"\d+" }
            );

            routes.MapRoute(
                name: "节假日套餐专辑列表",
                url: "package/collection2/{cid}",
                defaults: new { controller = "Hotel", action = "CollectionPackageForHoliday" },
                constraints: new { cid = @"\d+" }
            );

            routes.MapRoute(
                url: "hotel/{hotel}/package-calendar",
                name: "套餐日历",
                defaults: new { controller = "Hotel", action = "PackageCalendar" },
                constraints: new { hotel = @"\d+" }
            );

            routes.MapRoute(
                name: "酒店预订",
                url: "hotel/{hotel}/book",
                defaults: new { controller = "Hotel", action = "Book" },
                constraints: new { hotel = @"\d+" }
            );

            routes.MapRoute(
                name: "酒店提交订单",
                url: "hotel/{hotel}/submit",
                defaults: new { controller = "Hotel", action = "Submit" },
                constraints: new { hotel = @"\d+" }
            );

            /////
            // Order

            //routes.MapRoute(
            //    name: "订单列表",
            //    url: "order/list",
            //    defaults: new { controller = "Order", action = "List" }
            //);

            //routes.MapRoute(
            //    name: "订单详情",
            //    url: "order/{order}",
            //    defaults: new { controller = "Order", action = "Detail" },
            //    constraints: new { order = @"\d+" }
            //);

            /////
            // Payment

            routes.MapRoute(
                name: "渠道选择",
                url: "payment/pay/{order}",
                defaults: new { controller = "Payment", action = "Choose" },
                constraints: new { order = @"\d+" }
            );

            routes.MapRoute(
                name: "渠道跳转",
                url: "payment/direct/{channel}/{order}",
                defaults: new { controller = "Payment", action = "Direct" },
                constraints: new { order = @"\d+" }
            );

            routes.MapRoute(
                name: "支付完成",
                url: "payment/complete/{channel}/{order}",
                defaults: new { controller = "Payment", action = "Complete" },
                constraints: new { order = @"\d+" }
            );

            routes.MapRoute(
                name: "支付完成2",
                url: "pay/complete",
                defaults: new { controller = "Payment", action = "Complete" }
            );

            routes.MapRoute(
                name: "支付取消",
                url: "payment/cancel/{channel}/{order}",
                defaults: new { controller = "Payment", action = "Cancel" },
                constraints: new { order = @"\d+" }
            );

            //个人中心
            routes.MapRoute(
                name: "我的订单",
                url: "personal/order",
                defaults: new { controller = "Order", action = "List" }
            );
            routes.MapRoute(
                name: "订单详情",
                url: "personal/order/{order}",
                defaults: new { controller = "Order", action = "Detail" },
                constraints: new { order = @"\d+" }
            );
            routes.MapRoute(
                name: "我的点评",
                url: "personal/comment",
                defaults: new { controller = "Comment", action = "List" }
            );
            routes.MapRoute(
                name: "点评详情",
                url: "personal/comments/{CommentID}",
                defaults: new { controller = "Comment", action = "Detail" },
                constraints: new { CommentID = @"\d+" }
            );
            routes.MapRoute(
                name: "我的收藏",
                url: "personal/collection",
                defaults: new { controller = "Collection", action = "List" }
            );
            routes.MapRoute(
                name: "收藏详情",
                url: "personal/collection/{collection}",
                defaults: new { controller = "Hotel", action = "Detail" },
                constraints: new { collection = @"\d+" }
            );
            routes.MapRoute(
                name: "个人信息",
                url: "personal/info",
                defaults: new { controller = "Account", action = "Info" }
            );
            routes.MapRoute(
                name: "写点评",
                url: "personal/comment/section",
                defaults: new { controller = "Section", action = "Comment" }
            );
            routes.MapRoute(
                name: "我的钱包",
                url: "personal/wallet/{id}",
                defaults: new { controller = "Coupon", action = "Wallet" },
                constraints: new { id = @"\d+" }
            );
            routes.MapRoute(
                name: "钱包信息",
                url: "personal/wallet/{id}/{tag}",
                defaults: new { controller = "Coupon", action = "Wallet" },
                constraints: new { id = @"\d+" }
            );
            routes.MapRoute(
                name: "钱包信息明细",
                url: "personal/wallet/{id}/{tag}/{mode}",
                defaults: new { controller = "Coupon", action = "Wallet" },
                constraints: new { id = @"\d+" }
            );
            routes.MapRoute(
                name: "限时抢购",
                url: "coupon/shop/{id}",
                defaults: new { controller = "Coupon", action = "CouponShop" },
                constraints: new { id = @"\d+" }
            );
            routes.MapRoute(
                name: "主题团购",
                url: "coupon/shop/group/{id}",
                defaults: new { controller = "Coupon", action = "CouponShopForGroup" },
                constraints: new { id = @"\d+" }
            );
            routes.MapRoute(
                name: "VIP卡片购买列表",
                url: "custom/shop/vip/list",
                defaults: new { controller = "Coupon", action = "VipCartList" }
            );
            routes.MapRoute(
                name: "VIP购买",
                url: "custom/shop/vip/{id}",
                defaults: new { controller = "Coupon", action = "CouponShopForVip" },
                constraints: new { id = @"\d+" }
            );
            routes.MapRoute(
                name: "VIP预约",
                url: "custom/reserve/vip/{id}",
                defaults: new { controller = "Coupon", action = "CouponShopForVip2" },
                constraints: new { id = @"\d+" }
            );
            routes.MapRoute(
                name: "天天果园套餐购买",
                url: "custom/shop/ftd/{id}",
                defaults: new { controller = "Coupon", action = "CouponShopForFruitDay" },
                constraints: new { id = @"\d+" }
            );
            routes.MapRoute(
                name: "消费券购买",
                url: "coupon/product/{skuid}",
                defaults: new { controller = "Coupon", action = "CouponShopForProduct" },
                constraints: new { skuid = @"\d+" }
            );
            routes.MapRoute(
                name: "手拉手购买",
                url: "coupon/group/product/{skuid}/{groupid}",
                defaults: new { controller = "Coupon", action = "CouponShopForGroupProduct" },
                constraints: new { skuid = @"\d+", groupid = @"\d+" }
            );
            routes.MapRoute(
                name: "大团购",
                url: "coupon/stepgroup/product/{skuid}",
                defaults: new { controller = "Coupon", action = "CouponShopForStepGroup" },
                constraints: new { skuid = @"\d+" }
            );
            routes.MapRoute(
                name: "积攒拼团购买",
                url: "coupon/group/tree/{skuid}/{groupid}",
                defaults: new { controller = "Coupon", action = "GroupProductForTree" },
                constraints: new { skuid = @"\d+", groupid = @"\d+" }
            );
            //routes.MapRoute(
            //    name: "机酒购买",
            //    url: "packageproduct/{skuid}",
            //    defaults: new { controller = "Product", action = "PackageProduct" },
            //    constraints: new { skuid = @"\d+" }
            //);
            routes.MapRoute(
                name: "限时抢购列表页",
                url: "coupon/shoplist/{advID}/{groupNo}",
                defaults: new { controller = "Coupon", action = "CouponShopList" },
                constraints: new { advID = @"\d+", groupNo = @"\d+" }
            );
            routes.MapRoute(
                 name: "限时抢购列表页V1",
                 url: "coupon/shoplist",
                 defaults: new { controller = "Coupon", action = "CouponShopList" }
             );
            routes.MapRoute(
              name: "通用订单支付完成",
              url: "order/paycomplete/{channel}/{order}",
              defaults: new { controller = "Order", action = "PayComplete" },
              constraints: new { order = @"\d+" }
          );
            routes.MapRoute(
                name: "房券支付完成",
                url: "coupon/paycomplete/{channel}/{order}/{_t}",
                defaults: new { controller = "Coupon", action = "PayComplete" },
                constraints: new { order = @"\d+", _t = @"\d+" }
            );
            routes.MapRoute(
                name: "房券兑换",
                url: "coupon/exchange/{id}",
                defaults: new { controller = "Coupon", action = "CouponExchange" },
                constraints: new { id = @"\d+" }
            );

            routes.MapRoute(
                name: "房券兑换酒店套餐详情",
                url: "coupon/exchangepackage/{pid}",
                defaults: new { controller = "Coupon", action = "ExchangePackage" },
                constraints: new { pid = @"\d+" }
            );

            routes.MapRoute(
                name: "会员购买成功",
                url: "vip/paycomplete/{channel}/{order}",
                defaults: new { controller = "Coupon", action = "PayCompleteForVip" },
                constraints: new { order = @"\d+" }
            );

            routes.MapRoute(
                name: "天天果园套餐购买成功",
                url: "ftd/paycomplete/{channel}/{order}",
                defaults: new { controller = "Coupon", action = "PayCompleteForFruitDay" },
                constraints: new { order = @"\d+" }
            );

            routes.MapRoute(
                name: "发票支付完成",
                url: "invoice/paycomplete/{channel}/{order}",
                defaults: new { controller = "Coupon", action = "PayCompleteInvoice" },
                constraints: new { order = @"\d+" }
            );

            routes.MapRoute(
                name: "套餐专辑列表",
                url: "product/collection/{cid}",
                defaults: new { controller = "Hotel", action = "CollectionPackage" },
                constraints: new { cid = @"\d+" }
            );

            routes.MapRoute(
                name: "消费券专辑列表",
                url: "Coupon/MoreList/{grid}/{isdouble11}/{cid}",
                defaults: new { controller = "Coupon", action = "MoreProductList" },
                constraints: new { grid = @"\d+", isdouble11 = @"\d+", cid = @"\d+" }
            );

            routes.MapRoute(
                name: "领券活动中心",
                url: "Coupon/ActiveCenter/{grid}/{cid}",
                defaults: new { controller = "Coupon", action = "CouponActiveCenter" },
                constraints: new { grid = @"\d+", cid = @"\d+" }
            );

            routes.MapRoute(
                name: "领券活动中心-指定模板",
                url: "Coupon/ActiveCenter/{grid}/{cid}/{tempid}",
                defaults: new { controller = "Coupon", action = "CouponActiveCenter" },
                constraints: new { grid = @"\d+", cid = @"\d+", tempid = @"\d+" }
            );

            routes.MapRoute(
              name: "活动页",
              url: "active/{action}",
              defaults: new { controller = "Active", action = "index" }
          );

            // 默认
            routes.MapRoute(
                name: "默认",
                url: "{controller}/{action}",
                defaults: new { controller = "Portal", action = "Home" }
            );

            //品鉴师
            routes.MapRoute(
                name: "品鉴师",
                url: "{controller}/{action}",
                defaults: new { controller = "Inspector", action = "Explain" }
            );
            //品鉴师条件
            routes.MapRoute(
                name: "品鉴师条件与福利",
                url: "{controller}/{action}",
                defaults: new { controller = "Inspector", action = "Rules" }
            );
            //品鉴师注册
            routes.MapRoute(
               name: "品鉴师注册",
               url: "{controller}/{action}",
               defaults: new { controller = "Inspector", action = "Register" }
           );
            //品鉴酒店列表
            routes.MapRoute(
                name: "免费品鉴酒店",
                url: "{controller}/{action}",
                defaults: new { controller = "Inspector", action = "HotelList" }
            );
            //品鉴酒店入住
            routes.MapRoute(
                name: "酒店入住",
                url: "{controller}/{action}/{hotel}/{user}",
                defaults: new { controller = "Inspector", action = "Hotel" },
                constraints: new { hotel = @"\d+", user = @"\d+" }
            );
            //打开APP中转页面
            routes.MapRoute(
               name: "周末酒店",
               url: "{controller}/{action}",
               defaults: new { controller = "Inspector", action = "Jump" }
           );


            //打开APP中转页面
            routes.MapRoute(
               name: "MagiCall",
               url: "{controller}/{action}",
               defaults: new { controller = "MagiCall", action = "MagiCallClient" }
           );

            #region 微信内活动页

            //活动页
            routes.MapRoute(
            name: "微信活动Signup",
            url: "active/Weixin_SignupActive/{aidsharer}",
            defaults: new { controller = "Active", action = "Weixin_SignupActive" },
            constraints: new { aidsharer = @"\d+" }
           );

            //活动结束页
            routes.MapRoute(
           name: "微信活动Weixin_SignupActive_End",
           url: "active/Weixin_SignupActive_End/{activeid}",
           defaults: new { controller = "Active", action = "Weixin_SignupActive_End" },
           constraints: new { activeid = @"\d+" }
          );

            //活动报名页
            routes.MapRoute(
           name: "微信活动Weixin_SignupActive_Reg",
           url: "active/Weixin_SignupActive_Reg/{activeid}",
           defaults: new { controller = "Active", action = "Weixin_SignupActive_Reg" },
           constraints: new { activeid = @"\d+" }
          );

            //活动报名成功页
            routes.MapRoute(
           name: "微信活动Weixin_SignupActive_RegDone",
           url: "active/Weixin_SignupActive_RegDone/{activeid}",
           defaults: new { controller = "Active", action = "Weixin_SignupActive_RegDone" },
           constraints: new { activeid = @"\d+" }
          );

            //活动分享成功页
            routes.MapRoute(
           name: "微信活动Weixin_SignupActive_ShareDone",
           url: "active/Weixin_SignupActive_ShareDone/{activeid}",
           defaults: new { controller = "Active", action = "Weixin_SignupActive_ShareDone" },
           constraints: new { activeid = @"\d+" }
          );

            //活动邀请更多朋友页
            routes.MapRoute(
           name: "微信活动Weixin_SignupActive_SendMoreFd",
           url: "active/Weixin_SignupActive_SendMoreFd/{activeid}",
           defaults: new { controller = "Active", action = "Weixin_SignupActive_SendMoreFd" },
           constraints: new { activeid = @"\d+" }
          );

            routes.MapRoute(
              name: "微信活动Xmas2015",
              url: "active/weixin_xmas2015/{sharer}",
              defaults: new { controller = "Active", action = "Weixin_Xmas2015" },
              constraints: new { sharer = @"\d+" }
             );

            routes.MapRoute(
             name: "微信活动VillaSpring",
             url: "active/weixin_villaspring/{sharer}",
             defaults: new { controller = "Active", action = "Weixin_VillaSpring" },
             constraints: new { sharer = @"\d+" }
            );

            routes.MapRoute(
            name: "微信活动HeartValley",
            url: "active/weixin_heartvalley/{sharer}",
            defaults: new { controller = "Active", action = "Weixin_HeartValley" },
            constraints: new { sharer = @"\d+" }
           );

            #endregion

            #region 微信抽奖活动【最新】

            //【报名页】
            routes.MapRoute(
           name: "微信活动Weixin_LuckActive_Reg",
           url: "active/Weixin_LuckActive_Reg/{activeid}",
           defaults: new { controller = "Active", action = "Weixin_LuckActive_Reg" },
           constraints: new { activeid = @"\d+" }
          );

            //【报名页2】
            routes.MapRoute(
           name: "微信活动Weixin_LuckActive_Reg2",
           url: "wx/active/reg/{partnerid}/{activeid}",
           defaults: new { controller = "Active", action = "Weixin_LuckActive_Reg" },
           constraints: new { partnerid = @"\d+", activeid = @"\d+" }
          );

            //【报名页3 完成支付】
            routes.MapRoute(
           name: "微信活动Weixin_LuckActive_Reg3",
           url: "wx/active/regpayok/{partnerid}/{activeid}",
           defaults: new { controller = "Active", action = "Weixin_LuckActive_Reg", ip = 200 },
           constraints: new { partnerid = @"\d+", activeid = @"\d+" }
          );

            //【分享页】
            routes.MapRoute(
            name: "微信活动Weixin_LuckActive",
            url: "active/Weixin_LuckActive/{aidsharer}",
            defaults: new { controller = "Active", action = "Weixin_LuckActive" },
            constraints: new { aidsharer = @"\d+" }
           );

            //【报名成功页】
            routes.MapRoute(
           name: "微信活动Weixin_LuckActive_RegDone",
           url: "active/Weixin_LuckActive_RegDone/{activeid}",
           defaults: new { controller = "Active", action = "Weixin_LuckActive_RegDone" },
           constraints: new { activeid = @"\d+" }
          );

            //【分享成功页】
            routes.MapRoute(
           name: "微信活动Weixin_LuckActive_ShareDone",
           url: "active/Weixin_LuckActive_ShareDone/{activeid}",
           defaults: new { controller = "Active", action = "Weixin_LuckActive_ShareDone" },
           constraints: new { activeid = @"\d+" }
          );

            //【微信活动伙伴列表页】
            routes.MapRoute(
           name: "微信活动Weixin_LuckActive_SharePartner",
           url: "active/Weixin_LuckActive_SharePartner/{activeid}",
           defaults: new { controller = "Active", action = "Weixin_LuckActive_SharePartner" },
           constraints: new { activeid = @"\d+" }
          );

            //【微信活动伙伴关注奖励页】
            routes.MapRoute(
           name: "微信活动Weixin_LuckActive_PartnerLuck",
           url: "wx/active/partner/{partnerid}/{activeid}",
           defaults: new { controller = "Active", action = "Weixin_LuckActive_PartnerLuck" },
           constraints: new { partnerid = @"\d+", activeid = @"\d+" }
          );

            //【微信活动酒店列表页】
            routes.MapRoute(
           name: "微信活动Weixin_LuckActive_HotelList",
           url: "wx/active/hotellist/{partnerid}/{groupid}",
           defaults: new { controller = "Active", action = "Weixin_LuckActive_HotelList" },
           constraints: new { partnerid = @"\d+", groupid = @"\d+" }
          );

            #endregion

            #region 微信投票活动【公众号回复投票机制】

            //【微信投票活动结果页】
            routes.MapRoute(
               name: "微信投票活动结果页",
               url: "wx/active/voteresult/{activeId}/{voteId}",
               defaults: new { controller = "Active", action = "VoteResult" },
               constraints: new { activeId = @"\d+", voteId = @"\d+" }
            );

            #endregion

            #region 微信投票活动【大使代言投票机制】

            //【微信大投票主页】
            routes.MapRoute(
               name: "微信大投票主页",
               url: "wx/active/supervote/{activeId}/{urlfrommine}",
               defaults: new { controller = "Active", action = "SuperVote" },
               constraints: new { activeId = @"\d+", urlfrommine = @"\d+" }
            );

            //【大投票活动的被投票者主页】
            routes.MapRoute(
               name: "大投票活动的被投票者主页",
               url: "wx/active/supervoteitem/{activeId}/{id}/{sourceDrawid}/{urlfrommine}",
               defaults: new { controller = "Active", action = "SuperVoteItem" },
               constraints: new { activeId = @"\d+", id = @"\d+", sourceDrawid = @"\d+", urlfrommine = @"\d+" }
            );

            //【大投票活动的大使用户主页】
            routes.MapRoute(
               name: "大投票活动的大使用户主页",
               url: "wx/active/supervoteuser/{activeId}/{ruleExId}",
               defaults: new { controller = "Active", action = "SuperVoteUser" },
               constraints: new { activeId = @"\d+", ruleExId = @"\d+" }
            );

            //【大投票活动的大使用户注册页面】
            routes.MapRoute(
               name: "大投票活动的大使用户注册页面",
               url: "wx/active/supervoteuserreg/{activeId}/{exid}/{urlfrommine}",
               defaults: new { controller = "Active", action = "SuperVoteUserReg" },
               constraints: new { activeId = @"\d+", exid = @"\d+", urlfrommine = @"\d+" }
            );

            //【大投票活动的大使抽奖历史页面】
            routes.MapRoute(
               name: "大投票活动的大使抽奖历史页面",
               url: "wx/active/supervoteluckrecord/{activeId}",
               defaults: new { controller = "Active", action = "SuperVoteLuckRecord" },
               constraints: new { activeId = @"\d+" }
            );

            #endregion

            #region 红包联合推广活动

            //【领取落地页】
            routes.MapRoute(
           name: "红包联合推广活动RedpackUnionHome",
           url: "wx/active/redpackunionhome/{partnerid}/{activeid}",
           defaults: new { controller = "Active", action = "RedpackUnionHome" },
           constraints: new { activeid = @"\d+", partnerid = @"\d+" }
          );

            //【分享页】
            routes.MapRoute(
            name: "红包联合推广活动RedpackUnionShare",
            url: "wx/active/redpackunionshare/{aidsharer}",
            defaults: new { controller = "Active", action = "RedpackUnionShare" },
            constraints: new { aidsharer = @"\d+" }
           );

            #endregion

            #region 自定义活动

            routes.MapRoute(
          name: "自定义活动CustomActive_Reg1",
          url: "custom/active/{aid}",
          defaults: new { controller = "Active", action = "CustomActive_Reg" },
          constraints: new { aid = @"\d+" }
         );

            routes.MapRoute(
          name: "自定义活动CustomActive_Reg3",
          url: "custom/active/{aid}/{phone}/{showtype}",
          defaults: new { controller = "Active", action = "CustomActive_Reg" },
          constraints: new { aid = @"\d+", phone = @"\d+", showtype = @"\d+" }
         );

            #endregion

            #region App

            routes.MapRoute(
         name: "订优惠",
         url: "app/dingyouhui",
         defaults: new { controller = "App", action = "DiscountCollection" }
        );

            routes.MapRoute(
                 name: "机酒度假",
                 url: "App/AirHoliday",
                 defaults: new { controller = "App", action = "AlbumGroup" }
            );
            #endregion

            #region 红包分享

            routes.MapRoute(
                name: "红包分享",
                url: "coupon/redcashcoupon/{key}/{userid}",
                defaults: new { controller = "Coupon", action = "RedCashCoupon" },
                constraints: new { key = @"\w+", userid = @"\d+" }
            );

            #endregion
        }
    }
}