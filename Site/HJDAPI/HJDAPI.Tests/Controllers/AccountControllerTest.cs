using System;
using HJDAPI.Controllers;
using HJDAPI.Common.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HJDAPI.Models;
using HJD.HotelPrice.Contract.DataContract.Order;
using HJDAPI.Controllers.Adapter;
using HJD.HotelPrice.Contract;
using System.Collections.Generic;
using HJD.DestServices.Contract;
using System.Text.RegularExpressions;
using HJDAPI.Common.Security;
using HJD.AccountServices.Entity;
using System.Net;
using Newtonsoft.Json;
using HJD.CouponService.Contracts.Entity;

namespace HJDAPI.Tests.Controllers
{
    [TestClass]
    public class AccountControllerTest
    {
        [TestMethod]
        public void TestSubmitComment()
        {
            string ss = "1";
            int i = 0;
            SubmitCommentEntity c = new SubmitCommentEntity()
            {
                HotelID = 1,
                OrderID = 2,
                recommend = true,
                score = 4,
                UserID = 123,
                TagIDs = new List<int>() { 1, 2, 3, 4 },
                addInfos = new List<AddationalInfo>(){ new AddationalInfo(){ AddationalComment="test", CategoryID=1},
                     new AddationalInfo(){ AddationalComment="test1", CategoryID=2}}
            };
           SubmitCommentResultEntity r = CommentAdapter.SubmitComment(c);
        }

//        [TestMethod]
//        public void TestAlipayProxy()
//        {
//            string str  = HJDAPI.APIProxy.Order.GetAlipayOrderInfo(1626414033);
//        }
//        [TestMethod]
//        public void TestHotelProxy()
//        {
//         HotelItem3 hi =   HJDAPI.APIProxy.Hotel.Get3(206056, "0", "0", "wap", 0);

//        }
//  [TestMethod]
//        public void TestGetDistrictInfo()
//        {
//            List<DistrictInfoEntity> r = HJDAPI.APIProxy.Dest.GetDistrictInfo(new List<int>(){ 1,2 });
//        }
//  [TestMethod]
//  public void TestQueryInterestHotel()
//  {
//      HotelListQueryParam p = new HotelListQueryParam() { districtid = 2, star = 0, count = 10 };
//      InterestHotelsResult r  = HotelAdapter.QueryInterestHotel(p);
//      HotelListMenu menu = new HotelListMenu();
//       r = HJDAPI.APIProxy.Hotel.QueryInterestHotel(p);
//  }

//[TestMethod]
//  public void TestGetPackageOrderInfo20()
//  {

//      var r = HJDAPI.APIProxy.Order.GetPackageOrderInfo20(1721412516, 0);
//  }


//        [TestMethod]
//        public void TestPriceProxy()
//        {
//            OrderEntity order = GenOrder();
//            //OrderSubmit os = new OrderSubmit();
//            //os.packageID = 1;
//            //os.name = "hi";
//            order.package.RoomCount = 2;
         
//            OrderSubmitResult result = HJDAPI.APIProxy.Order.SubmitOrder(order);
//        }


        public OperationResult MobileLogin40()
        {
            AccountInfoItem ai = new AccountInfoItem();
            ai.Phone = "G0HHZuCQP4Lop9qp8BtdXg==";
            ai.Password = "mz3FVxmtkCs=";
            //MobileLogin40(ai);
            return new OperationResult();
        }


        private OrderEntity GenOrder()
        {
            OrderEntity order = new OrderEntity();
            OrderMainEntity main = new OrderMainEntity();
            OrderPackageEntity package = new OrderPackageEntity();
            SimpleInvoiceInfoEntity invoiceInfo = new SimpleInvoiceInfoEntity();

            main.Amount = 1100;// 800;
            main.Quantity = 1;
            main.Type = OrderType.Package;
            main.HotelID = 206056;



            package.CheckIn = DateTime.Now.AddDays(25); // DateTime.Parse("2013-03-01");// DateTime.Now.AddDays(5);
             package.NightCount = 1;

             package.Contact = "王二";
             package.ContactPhone = "15502622690";// "18021036971";

             package.PID = 1110;
      package.RoomCount = 1;
       
            package.Note = "好的";

           invoiceInfo.Title = "Title One";
            invoiceInfo.Address = "地址";
            invoiceInfo.Contact = "联系人";
            invoiceInfo.TaxNumber = "112212331221";

            order.main = main;
            order.package = package;
            order.invoiceInfo = invoiceInfo;

            return order;
        }
        [TestMethod]
        public void GetPointslistNumByUserIdAndTypeId()
        {
            //int o1 = HotelAdapter.GetPointslistNumByUserIdAndTypeId(4671798, 10) != null ? HotelAdapter.GetPointslistNumByUserIdAndTypeId(4671798, 10).Count : 0;
            int o= HotelAdapter.GetPointslistNumByUserIdAndTypeId(4671798, 10).Count;
            User_Info ui = AccountAdapter.GetOrRegistPhoneUser("18021036971");
        }
        [TestMethod]
        public void TestGetOrRegistPhoneUser()
        {
            User_Info ui = AccountAdapter.GetOrRegistPhoneUser("18021036971");
        }

        [TestMethod]
        public void TestGetPackageList()
        {
            int hotelid = 206056;
            
            string checkIn = DateTime.Now.ToString("yyyy-MM-dd");
            string checkOut = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd");
            string stype = "www";

            new PriceController().Get7(hotelid, checkIn, checkOut, stype);
        }

        [TestMethod]
        public void PhoneNumLogin()
        {
            AccountInfoItem ai = new AccountInfoItem();
            ai.Phone = "15011110000";
            ai.ConfirmCode = "705882";
            new AccountsController().PhoneNumLogin(ai);
        }

        [TestMethod]
        public void PullUserInfo()
        {
            AccountInfoItem ai = new AccountInfoItem();
            ai.Phone = "15011110000";
            ai.ConfirmCode = "705882";
            new AccountsController().PullUserInfo(ai);
        }

       [TestMethod]
        public void TestOrder()
        {
        //    OrderSubmitResult SubmitOrder(OrderEntity order)
            for (int i = 1; i < 2; i++)
            {
                OrderEntity order = GenOrder();
                order.main.UserID = 0;
              //  order.package.PID = 1;
                order.package.PayType = 5;
                order.package.NightCount = 1;
                order.package.RoomCount = 2;
                order.package.ContactPhone = "18021036971";
                order.package.CheckIn = DateTime.Now.AddDays(1);
                order.package.Contact = "打包套餐" + (DateTime.Now.Millisecond%100).ToString();
                order.package.Note = "双床";// "大床房 无烟房 4间双床一间大床";
                order.main.Amount = 550 * order.package.NightCount * order.package.RoomCount;
                order.main.ChannelID = 2;
                order.main.TerminalID = 3;
                order.package.UserUseCashCouponAmount = 100;
                order.package.UserUseCashCouponAmount = 50;
                order.package.ActiveRebate = 50;
                order.package.CashCouponAmount = 500;
                order.package.Contact += i.ToString();
                order.package.RoomTypeID = 1;

                order.package.UserUseHousingFundAmount = 100;

           var bestCoupon = CouponAdapter.GetTheBestCouponInfoForOrder(new OrderUserCouponRequestParams {
               OrderTypeID =  CashCouponOrderSorceType.hotelPackage, 
               TotalOrderPrice = order.main.Amount ,
               UserID = 4512277 });
            if (bestCoupon.Success)
                {
                    //cashCouponInfo.CashCouponID = bestCoupon.CashCouponInfo.IDX;
                    //cashCouponInfo.UseCashAmount = bestCoupon.CashCouponInfo.OrderCanUseDiscountAmount;
                    //cashCouponInfo.CashCouponType = bestCoupon.CashCouponInfo.UserCouponType;  

                    order.package.UseCashCouponInfo = new HJD.HotelPrice.Contract.DataContract.Order.UseCashCouponItem { CashCouponID = bestCoupon.CashCouponInfo.IDX,  CashCouponType = bestCoupon.CashCouponInfo.UserCouponType, UseCashAmount = bestCoupon.CashCouponInfo.OrderCanUseDiscountAmount };
                }
             
                OrderSubmitResult r = OrderAdapter.SubmitOrder(order);

                Assert.AreEqual(OrderErrorCode.SUCCESS, r.ErrorCode);
            }
        }

       [TestMethod]
       public void TestPackageOrder()
       {
           OrderEntity order = GenOrder();
           order.main.UserID = 0;
           order.package.PID = 1110;
           order.package.PayType = 5;
           order.package.NightCount = 2;
           order.package.RoomCount = 5;
           order.package.ContactPhone = "18021036971";
           order.package.CheckIn = DateTime.Now.AddDays(1);
           order.package.Contact = "打包套餐" + (DateTime.Now.Millisecond % 100).ToString();
           order.package.Note = "双床";// "大床房 无烟房 4间双床一间大床";
           order.main.Amount = 280 * order.package.NightCount * order.package.RoomCount;
           order.main.ChannelID = 2;
           order.main.TerminalID = 3;
           order.package.UserUseCashCouponAmount = 100;
           order.package.UserUseCashCouponAmount = 50;
           order.package.ActiveRebate = 50;
           order.package.CashCouponAmount = 500;
           order.package.Contact = "22";
           order.package.RoomTypeID = 1;
           //order.TravelId = new List<int> { 1, 2 };

           OrderSubmitResult r = OrderAdapter.SubmitOrder(order);

           Assert.AreEqual(OrderErrorCode.SUCCESS, r.ErrorCode);
       }


       [TestMethod]
       public void TestParseRoomInfo()
       {
           List<string> notelist = new List<string>(){"双床房","双床房 无烟房 一间双床房，两间大床房，",
              "2间双床房，1大床",
              "大床房 无烟房 三间双床，二间大床",
           "无烟 两个房型的房间各订一间。两件房间相邻。",
           "双床房 无烟房 大床房和双床房各一间",
           "大床房 1间双床，其余大床房，谢谢！韩雪梅的这间大床房取消"};

           Dictionary<string, string> numDic = new Dictionary<string, string>() { { "一", "1" } ,
                { "二", "2" },
                { "两", "2" },
                { "三", "3" },
                { "四", "4" },
                { "五", "5" },
                { "六", "6" },
                { "七", "7" },
                { "八", "8" },
                { "九", "9" }};

           int roomCount = 5;

           foreach (string note in notelist)
           {

               string newNote = note;
               foreach (string key in numDic.Keys)
               {
                   newNote = newNote.Replace(key, numDic[key]);
               }

               Console.WriteLine(newNote);

               int BigBedCount = 0;
               int TwinBedCount = 0;

               Regex BigBegreg = new Regex("(\\d)间?大床");
               Regex TwinBegreg = new Regex("(\\d)间?双床");

               Match m = BigBegreg.Match(newNote);

               if (m.Success)
               {
                   BigBedCount = int.Parse( m.Groups[1].ToString());
               }

               m = TwinBegreg.Match(newNote);

               if (m.Success)
               {
                   TwinBedCount = int.Parse(m.Groups[1].ToString());
               }

               if (TwinBedCount == 0 && BigBedCount == 0)
               {
                   if (newNote.IndexOf("双床") >= 0)
                   {
                       TwinBedCount = roomCount;
                   }
                   else if (newNote.IndexOf("大床") >= 0)
                   {
                       BigBedCount = roomCount;
                   }
               }
               else
               {
                   if (TwinBedCount == 0)
                   {
                       TwinBedCount = roomCount - BigBedCount;
                   }
                   else if (BigBedCount == 0)
                   {
                       BigBedCount = roomCount - TwinBedCount;
                   }
               }

               Console.WriteLine(string.Format("{0}大床 {1}双床", BigBedCount, TwinBedCount));

           }
       }


        [TestMethod]
        public void TestRegister()
        {
            AccountsController ac = new AccountsController();
           HJDAPI.Models.AccountInfoItem item = new Models.AccountInfoItem();

           item.Email = "w2Cai2@gmail.com";
           item.IsMobile = 1;
           item.NickName = "w2Cai21";
           item.Password = "kevincai";

           OperationResult r = ac.Register(item);
        }  
        
        [TestMethod]
        public void TestPhoneUserModifyPassword()
        {
            AccountsController ac = new AccountsController();

            string oldpassword = "123456";// AccountAdapter.GenPassword();
            string newpassword = "654321";
            long userid = 4511960;
            string phone = "18021036979";

            ModifyPasswordItem r = new ModifyPasswordItem();
            r.oldpassword = oldpassword;
            r.newpassword = newpassword;
            r.userid = userid;


            OperationResult result = ac.ModifyPassword(r);
        }
        [TestMethod]
        public void TestModifyPassword()
        {
            ModifyPasswordItem r = new ModifyPasswordItem();
            r.userid = 4511973;
            r.oldpassword = "314592";
            r.newpassword = "314592";
            r.updateIP = "";
            OperationResult or =    AccountAdapter.ModifyPassword(r);
        }

  
        [TestMethod]
        public void TestBindUserAccountAndOrders()
        {
            HotelAdapter.BindUserAccountAndOrders( 2 , "18021036971");
        }

        [TestMethod]
        public void TestRegisterPhoneUser()
        {
            AccountsController ac = new AccountsController();

            string password = "304030";// AccountAdapter.GenPassword();
            string phone = "10021036801";

            RegistPhoneUserItem rp = new RegistPhoneUserItem();
            rp.Phone = phone;
            rp.ConfirmPassword = password;
            rp.Password = password;

            OperationResult r = ac.RegistPhoneUser(rp);
        }

        [TestMethod]
        public void TestPhoneUserLogin()
        {
            AccountsController ac = new AccountsController();

            AccountInfoItem ai = new AccountInfoItem();
            ai.Phone = "15502126909";
            ai.IsMobile = 1;
            ai.Password = "111111";

            OperationResult r = ac.MobileLogin(ai);
        }

        [TestMethod]
        public void TestRestPhoneUserPassword()
        {
            AccountsController ac = new AccountsController();

            ResetPasswordItem ai = new ResetPasswordItem();
            ai.Phone = "18021036971";
            ai.confirmCode = "405185";
            ai.newpassword = "314592";
            OperationResult r = ac.ResetPasswordWithPhone(ai);
        }

        [TestMethod]
        public void TestDecryptDES()
        {
            SecurityHelper sh = new SecurityHelper();
            string key = "haohotel";
            long userid = 4511730;

            string e = SecurityHelper.EncryptDES(userid.ToString(), key);

            string userID =  SecurityHelper.DecryptDES(e, key);

        }

        [TestMethod]
        public void TestSignCal()
        {
            //string url = "http://www.zmjiudian.com/personal/wallet/4515263/cash?TimeStamp=1426695667&SourceID=1&sign=JGzGxWN6kj+71uVVNkvbAw==";
            string url_Sign = "JGzGxWN6kj+71uVVNkvbAw==";//url请求的sign
            long url_TimeStamp = 1426695667;
            string url_RequestType = "http://www.zmjiudian.com/personal/wallet/4515263/cash?TimeStamp=1426695667&SourceID=1";
            int url_SourceID = 1;
            string sscode = HJDAPI.Common.Security.Signature.GenSignature(url_TimeStamp, url_SourceID, Configs.MD5Key, url_RequestType);
            string final = "JGzGxWN6kj+71uVVNkvbAw==";
            bool isEqual = final.Equals(sscode);
            //string ss = Signature.GenSignature(100000,0,Configs.MD5Key,"TestSecurity");
        }

        //[TestMethod]
        //public void TestCheckInspectorAccess()
        //{
        //    string url = "http://192.168.1.20:500/" + "api/Inspector/CheckInspectorAccess";
        //    CookieContainer cc = new CookieContainer();
        //    OperationResult item = new OperationResult() { Mobile = "15001966513" };
        //    string json = HttpRequestHelper.PostJson(url, item, ref cc);
        //    ResultEntity re =JsonConvert.DeserializeObject<ResultEntity>(json);
        //}

        [TestMethod]
        public void TestRegistPhoneUser()
        {
            CouponAdapter.GiveCouponByActivityType(4512004, CouponActivityCode.regist, "15850636791");
        }

        [TestMethod]
        public void TestRequestOrderRebate()
        {
            string url = "www.zmjiudian.com/" + "api/order/RequestOrderRebate";
            CookieContainer cc = new CookieContainer();
            RequestOrderRebateRequestParams param = new RequestOrderRebateRequestParams() { orderID = 4512004, type = 2 };
            string json = HttpRequestHelper.PostJson(url, param, ref cc);

            JsonConvert.DeserializeObject<BaseResponse>(json);
        }

        [TestMethod]
        public void TestCheckInspectorAccess()
        {
            string url = "http://192.168.1.113:8000/" + "api/Inspector/CheckInspectorAccess";
            CookieContainer cc = new CookieContainer();
            OperationResult item = new OperationResult()
            {
                Mobile = "15001966513",
                TrueName = "yh"
            };
            string json = HttpRequestHelper.PostJson(url, item, ref cc);
            JsonConvert.DeserializeObject<ResultEntity>(json);
        }

        [TestMethod]
        public void TestGetShareCommentCoupon()
        {
            string url = "http://192.168.1.20:500/" + "api/Coupon/GetShareCommentCoupon";
            CookieContainer cc = new CookieContainer();
            ShareCommentCouponParam param = new ShareCommentCouponParam() { userId = 4512016, sourceId = 558 };
            string json = HttpRequestHelper.PostJson(url, param, ref cc);
            OriginCouponResult result = JsonConvert.DeserializeObject<OriginCouponResult>(json);
        }
    }
}
