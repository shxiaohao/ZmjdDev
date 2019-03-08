using HJD.CouponService.Contracts.Entity;
using HJDAPI.Controllers;
using HJDAPI.Controllers.Adapter;
using HJDAPI.Controllers.Common;
using HJDAPI.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProductService.Contracts.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.Tests.Controllers
{
    [TestClass]
    public class CouponControllerTest
    {

        [TestMethod]
        public void SubmitExchangeOrder()
        {
            SubmitCouponOrderModel scom = new SubmitCouponOrderModel { ActivityID = 101349, SKUID = 707, ActivityType = 200, UserID = 4511972, OrderItems = new List<ProductAndNum> { new ProductAndNum { Number = 2, Price = 250, SourceID = 2, Type = 200 } } };

            CouponOrderResult result = new CouponController().SubmitExchangeOrder(scom);
        }

        [TestMethod]
        public void GetCanUseVoucherInfoListForOrder()
        {
            var ui = AccountAdapter.GetOrRegistPhoneUser("18021036970");

            OrderUserCouponRequestParamsBase req = new OrderUserCouponRequestParamsBase()
            {
                 BuyCount = 1,
                  OrderSourceID = 1,
                   OrderTypeID = CashCouponOrderSorceType.sku,
                    UserID = ui.UserId,
                     SelectedDateFrom = DateTime.Now,
                      SelectedDateTo = DateTime.Now,
                       TotalOrderPrice = 200                        
                  
            };


            var  result = new CouponController().GetCanUseVoucherInfoListForOrder(req);
        }


        [TestMethod]
        public void GetCanUseCouponInfoListForOrder()
        {
            var ui = AccountAdapter.GetOrRegistPhoneUser("18021036970");

            OrderUserCouponRequestParams req = new OrderUserCouponRequestParams()
            {
                 BuyCount = 1,
                  OrderSourceID = 1,
                   OrderTypeID = CashCouponOrderSorceType.sku,
                    UserID = ui.UserId,
                     SelectedDateFrom = DateTime.Now,
                      SelectedDateTo = DateTime.Now,
                       TotalOrderPrice = 200                        
                  
            };


            var result = new CouponController().GetCanUseCouponInfoListForOrder(req);
        }

        [TestMethod]
        public void GroupSuccessSendMsg()
        {
            new CouponController().GroupSuccessSendMsg(222, "dddd", "ddss", 200);
        }

        [TestMethod]
        public void SubmitExchangeMemberOrder()
        {
            var ui = AccountAdapter.GetOrRegistPhoneUser("18021036970");

            SubmitCouponOrderModel scom = new SubmitCouponOrderModel { ActivityID = 100929, ActivityType = 400, UserID = ui.UserId, OrderItems = new List<ProductAndNum> { new ProductAndNum { Number =1, Price = 199, SourceID = 2, Type = 400 } } };

            CouponOrderResult result = new CouponController().SubmitMemberOrder(scom);
        }


        [TestMethod]
        public void CheckSelectedVoucherInfoForOrder()
        {
            OrderVoucherRequestParams req = new OrderVoucherRequestParams { SelectedVoucherIDs = new List<int> { 1, 2 } , UserID= 123, TotalOrderPrice = 20, MaxOrderCanUseVoucherAmount = 20, OrderTypeID = CashCouponOrderSorceType.sku };
            var result = new CouponController().CheckSelectedVoucherInfoForOrder(req );
        }



        [TestMethod]
        public void GetSKUCouponActivityDetail()
        {

            var result = new CouponController().GetSKUCouponActivityDetail(1);
        }
        [TestMethod]
        public void SubmitExchangeOrderForProduct()
        {
            // aid=100950&atype=600&skuid=2&paynum=1&userid=4511860&stype=1

         
            UseCashCouponItem cashCouponInfo = new UseCashCouponItem { CashCouponType = (int)UserCouponType.DiscountOverPrice, OrderType = (int)CashCouponOrderSorceType.sku, CashCouponID = 0, UseCashAmount = 0 };
            var bestCoupon = CouponAdapter.GetTheBestCouponInfoForOrder(new OrderUserCouponRequestParams { OrderTypeID = CashCouponOrderSorceType.sku, TotalOrderPrice = 60, UserID = 4512277 });
            if (bestCoupon.Success)
                {
                    cashCouponInfo.CashCouponID = bestCoupon.CashCouponInfo.IDX;
                    cashCouponInfo.UseCashAmount = bestCoupon.CashCouponInfo.OrderCanUseDiscountAmount;
                    cashCouponInfo.CashCouponType = bestCoupon.CashCouponInfo.UserCouponType;
                }

            SubmitCouponOrderModel scom = new SubmitCouponOrderModel
            {
                ActivityID = 102155,
                SKUID = 2229,
                ActivityType = 600,
                UserID = 4512277,
                UseCashCouponInfo = cashCouponInfo,
                        UserUseHousingFundAmount = 2,
                                                                       OrderItems = new List<ProductAndNum> { 
                                                                           new ProductAndNum { Number = 2, Points = 0, Price = 0, SourceID = 1242 ,
                                                                           Type = 1} 
                                                                       }  
                                                                   
            }; 

            var result = new CouponController().SubmitExchangeOrderForProduct(scom);

            scom.UseVoucherInfo = new UseVoucherInfoEntity() { UseVoucherAmount = 110, UseVoucherIDList = new List<int> { 1, 2, } };
            scom.UserUseHousingFundAmount = 2;
            scom.UseCashCouponInfo = cashCouponInfo;
             scom.OrderItems = new List<ProductAndNum> { 
                                                                           new ProductAndNum { Number = 2, Points = 0, Price = 0, SourceID = 1242, 
                                                                               RetailPrice = 9.9M,
                                                                           Type = 1} 
                                                                       }; 
            var result2 = new CouponController().SubmitExchangeOrderForProduct(scom);
        }

        [TestMethod]
        public void SubmitGroupOrderForProduct()
        {
            SubmitCouponOrderModel scom = new SubmitCouponOrderModel();//{ ActivityID = 100956, ActivityType = 200, UserID = 4512253, SKUID = 213 };
            scom.ActivityID = 102156;// 100961;
            scom.ActivityType = 200;
            scom.UserID = 4512652;
            scom.SKUID = 2230;// 213;   //ActivityID = 102156, SKUID = 2230
            scom.GroupId = 0;
            scom.PhoneNo = "15502126909";
            scom.OrderItems = new List<ProductAndNum>
            {
                new ProductAndNum
                {
                    SourceID=0,
                    Number=1,
                    Type=1
                }
            };
            scom.OpenId = "21212sddxcx";

            var result = new CouponController().SubmitGroupOrderForProduct(scom);
        }
  [TestMethod]
        public void CheckOrderBeforePay( )
        {
            CheckOrderBeforePayRequestParams p = new CheckOrderBeforePayRequestParams { OrderID = 1724, UserID = 0 };
         var result = new OrderController().CheckOrderBeforePay(p);
       
        }

        [TestMethod]
        public void CheckPromotionAndRemoveUserBuyFirstPackagePriviledge()
        {
            long UserID = 123;
            int PromotionID = 1;
            ProductAdapter.CheckPromotionAndRemoveUserBuyFirstPackagePriviledge(UserID, new List<int> { PromotionID });
        }

        [TestMethod]
        public void TestSubmitExchangeRoomOrder()
        {
            SubmitExchangeRoomOrderParam submitParam = new SubmitExchangeRoomOrderParam
            {
                channelID = 1,
                checkIn = DateTime.Now,
                contact = "test",
                contactPhone = "18021036971",
                exchangeNo = "B08F06JB165T",
                hotelID = 206056,
                nightCount = 1,
                note = "",
                packageID = 579,
                packageType = 1,
                roomCount = 1,
                terminalID = 1,
                useCouponNum = 0,
                userID = 4511973
            };
            ExchangeRoomOrderConfirmResult cm = CouponAdapter.IsExchangeNeedAddMoney(submitParam);
            SubmitExchangeRoomOrderResult m = CouponAdapter.SubmitExchangeRoomOrder(submitParam);

        }

        [TestMethod]
        public void TestGetSearchProductList()
        {
            SearchProductRequestEntity param = new SearchProductRequestEntity();
            param.Count = 3;
            param.Screen = new SearchProductScreenEntity() { ProductType = { 200, 600 } };
            param.SearchWord = "";
            param.Sort = 1;
            param.Start = 0;
            GetRetailProductListResult m = new CouponController().GetSearchRetailerProductList(param);
        }

        [TestMethod]
        public void TestInserBook()
        {
            BookPersonInfoEntity user = new BookPersonInfoEntity();
            //BookUserInfoEntity bookUser = new BookUserInfoEntity();

            BookUserDateInfoEntity bud = new BookUserDateInfoEntity();
            //bookUser.CardNo = "123654789";
            //bookUser.CardType = 1;
            //bookUser.ExchangCouponID = 1;
            //bookUser.PersonName = "teste";
            //bookUser.State = 1;

            //bud.BookDateId = 182;
            //bud.BookDetailId = 36;
            //bud.ExchangCouponId = 160;
            //bud.State = 0;
            
            //bud.TravelIDs = "2,3";


            user.skuid = 442;
            user.ExchangCouponIds = new List<int>(){1,2};
            user.TravelId = new List<int>() ;
            user.BookDateId = 182;
            user.BookDetailId = 36;
            //user.BookUserDate = bud;
            //user.BookUserList.Add(bookUser);
            //user.BookTempDate = "";

            new CouponController().SubmitBookInfo(user);
            //user.BookUserDateInfo = 
        }

        [TestMethod]
        public void GetBookDateList()
        {
            new CouponController().GetBookDateList(439);
        }

        [TestMethod]
        public void GetSKUTempSource()
        {
           string sss = CommMethods.GenShortenUrl("http://www.zmjiudian.com/coupon/product/4515?cid=4512316");
            //new CouponController().GetSKUTempSource(439); 
        }
    }
}
