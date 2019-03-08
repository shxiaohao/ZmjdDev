using HJD.AccessService.Contract.Model;
using HJD.AccessService.Contract.Params;
using HJD.AccountServices.Entity;
using HJD.CouponService.Contracts.Entity;
using HJD.Framework.Encrypt;
using HJD.OtaCrawlerService.Contract;
using HJD.OtaCrawlerService.Contract.Params;
using HJD.WeixinServices.Contracts;
using HJDAPI.Common.Helpers;
using HJDAPI.Common.Security;
using HJDAPI.Controllers;
using HJDAPI.Controllers.Activity;
using HJDAPI.Controllers.Adapter;
using HJDAPI.Controllers.Common;
using HJDAPI.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.Tests.Controllers
{
    [TestClass]
    public class WeixinTest
    {
        public static string WeixinOpenidEncode(string openid)
        {
            return openid.Replace("-", "zmjd");
        }

        public static string WeixinOpenidDecode(string openid)
        {
            return openid.Replace("zmjd", "-");
        }

        [TestMethod]
        public void GetWeixin()
        {
            var testStr = "GROUPTREE_123__234";
            var _keyList = testStr.Split('_');

            var _shareInfo = _keyList[1] + (_keyList.Length > 2 ? "_" + _keyList[2] : "") + (_keyList.Length > 3 ? "_" + _keyList[3] : "") + (_keyList.Length > 4 ? "_" + _keyList[4] : "");

            var first = "Wow! 你有一位好友已成功帮你助力哦！";

            //data list
            var dataList = new List<string>();
            dataList.Add("周末酒店");
            dataList.Add("测试产品");

            WeiXinAdapter.SendTemplateMessage(7, "oHGzlw-sdix9G__-S4IzfTsYRqC8", "8Ctx7mex5dalgAE_cu0GC12v2fwPs8Szx2vn_1Ri2oM", "http://www.zmjiudian.com/coupon/group/tree/1484/4564", first, ">>点击详情查看助力最新动态", dataList);

            return;

            var configInfo = new OrderController().WxAppPay(new WxAppPayRequestParam { orderid = "1234567891", openid = "oDWPq0Fd0N0Ica7xcXO5htud6JgA" });

            var phoneList = GetWeixinSignUpPhoneList();
            foreach (var phone in phoneList)
            {
                if (CommMethods.IsPhone(phone))
                {
                    User_Info ui = AccountAdapter.GetOrRegistPhoneUser(phone);
                }
            }
            return;

            var reads = new WeixinApiController().GetActiveWeixinShareReadList(30, "okg6-uBqijun__Frr_dJfV8m6Tjs");

            var openid = WeixinOpenidEncode("okg6-uBqijun__Frr_dJfV8m6Tjs");
            var wu = new WeixinApiController().GetActiveWeixinUser(openid);

            //var wrp = new WeixinChatRecordParams {
            //    starttime = ConvertDateTimeInt(DateTime.Parse("2015-05-27")),
            //    endtime = ConvertDateTimeInt(DateTime.Parse("2015-05-27 23:59:59")),
            //    openid = "",
            //    pagesize = 1000,
            //    pageindex = 1
            //};

            //var recordList = WeiXinAdapter.GetWeixinChatRecord(wrp);

            //return;

            //var order = OrderAdapter.GetPackageOrderInfo20(4513537097, 4512064);

            //var pxList = new OtaCrawlerController().GetProxyDataList();

            //RequestEntity request = new RequestEntity { Content = "猜一猜富春山居15001966513", OrderID = 15001966513, ActiveID = 15, FromUserName = "shyuanhao",  };
            //var back = WeiXinAdapter.AddActiveLuckyDraw(request);


            //var list = new PriceController().Get3(360023, "2015-08-11", "2015-08-12", "www");

            //return;

            //HotelItem4 hotel = new HotelController().Get40(297562, "2015-05-01", "2015-05-02", "www", 0);

            //HotelRoomParams hotelRoomParams = new HotelRoomParams
            //{
            //    HotelID = "436187",
            //    CheckIn = "2015-04-01",
            //    CheckOut = "2015-04-02",
            //    OtaType = OtaType.Ctrip,
            //};
            //var hotel = new OtaCrawlerAdapter().GetCtripCanSellHotel(hotelRoomParams);
            //if (hotel != null && hotel.HotelBaseRoomList != null)
            //{
            //    foreach (var baseRoom in hotel.HotelBaseRoomList)
            //    {
            //      new HotelController().InsertCtripRoom(baseRoom.HotelID, baseRoom.BaseRoomID, baseRoom.Name, baseRoom.Area, baseRoom.Floor, baseRoom.BedType, baseRoom.NonSmokingRoom, baseRoom.MaxGuests);
            //    }

            //}
        }

        public List<string> GetWeixinSignUpPhoneList()
        {
            var list = new List<string>();

            var txtList = File.ReadAllLines(@"D:\Me\Task\2015-12-24\phone.txt");
            list = txtList.ToList();

            return list;
        }

        [TestMethod]
        public void TestWeixinData()
        {
            //var sendsms = AccessController.SendSMS("15001966513","hello");
            //return;

            RequestEntity requestXML = new RequestEntity();
            requestXML.ToUserName = "zmjiudian";
            requestXML.FromUserName = "test1";
            requestXML.CreateTime = DateTime.Now.ToString("yyyy-mm-dd");
            requestXML.MsgType = "image";     //location //image //text //event

            switch (requestXML.MsgType)
            {
                case "image":
                    requestXML.PicUrl = "test image url 1";
                    break;
                case "text":
                    requestXML.Content = "爱的故事凯宾斯基大酒店15001966513"; //"反馈15001966513不错不错不错"; //猜一猜千岛湖洲际15001966513
                    break;
            }

            var get = new WeixinController().DataTest(requestXML);
        }

        [TestMethod]
        public void TestWinXinRule()
        {
            CookieContainer cc = new CookieContainer();

            var url = "http://api.zmjiudian.com/api/hotel/SearchInterestHotel40?districtid=2&lat=0&lng=0&distance=0&hotelid=0&type=0&sort=0&order=0&start=0&count=15&checkin=&checkout=&nLat=0&nLng=0&sLat=0&sLng=0&valued=0&tag=&minPrice=0&maxPrice=0&location=&zone=&brand=&attraction=0&featured=&Interest=0&geoScopeType=1";

            string json = HttpRequestHelper.Get(url, "", ref cc);
            var result3 = JsonConvert.DeserializeObject<WapInterestHotelsResult3>(json);

            return;

            string input = "民宿";

            string inputs = "南京,天津,上海,苏州";

            string toUser = "oIDrpjqASyTPnxRmpS9O_ruZGsfk";
            string fromuserName = "gh_680bdefc8c5d";
            string createTime = "1359011899";
            string MsgId = "5837017832671832047";
            string str = WeiXinAdapter.getResponseForTextInput2(input, toUser, fromuserName, createTime);



            string rule = "suzhou:^苏州$;history:历史;";


            // string result =    WeiXinAdapter.ParseText("苏州市", rule);




            string result = WeiXinAdapter.getResponseForTextInput2(input, toUser, fromuserName, createTime);

            foreach (string i in inputs.Split(','))
            {
                result = WeiXinAdapter.getResponseForTextInput2(i, toUser, fromuserName, createTime);

            }
        }
        [TestMethod]
        public void TestWXActivity1()
        {
            WXActivity3 wx = new WXActivity3();

            for (int i = 0; i < 20; i++)
            {
                string str = wx.Vote(3001, "kevincai");
                str = wx.Vote(3001, "kevincai2");
                str = wx.Vote(3002, "kevincai3");
                wx.saveResult();
            }
        }
        [TestMethod]
        public void TestWXActivity3()
        {
            WXActivity3 wx = new WXActivity3();

            for (int i = 0; i < 20; i++)
            {
                string str = wx.Vote(1001, "kevincai");
                str = wx.Vote(1001, "kevincai2");
                str = wx.Vote(1002, "kevincai3");
                wx.saveResult();
            }
        }
        [TestMethod]
        public void TestGenerateRandomStr()
        {
            string ss = DescriptionHelper.GenerateRandomStr();
        }

        [TestMethod]
        public void TestGetWalletList()
        {
            string url = "http://192.168.1.20:500/" + "api/Coupon/GetWalletList";
            CookieContainer cc = new CookieContainer();
            string postDataStr = string.Format("userId={0}", "4512004");
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            WalletResult result = JsonConvert.DeserializeObject<WalletResult>(json);
        }

        [TestMethod]
        public void TestGetMethod()
        {
            string url = "http://192.168.1.20:500/" + "api/Coupon/GetCashCoupon";
            CookieContainer cc = new CookieContainer();
            string postDataStr = string.Format("id={0}&guid={1}", 0, "a22fa624-3211-4557-ad88-beff11d29a94");
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            OriginCoupon result = JsonConvert.DeserializeObject<OriginCoupon>(json);
        }

        [TestMethod]
        public void TestGetMethod2222()
        {
            string ss = Signature.GenSignature(1422957761, 2, Configs.MD5Key, "http://web.shanglv.net.cn:500/Coupon/DashList?userId=4512016&TimeStamp=1422957761&SourceID=2");
            bool isT = Signature.IsRightSignature(1422957761, 2, "http://web.shanglv.net.cn:500/Coupon/DashList?userId=4512016&TimeStamp=1422957761&SourceID=2", "Jgc9fnfZ4CT4t+pLRzoY3w==");
        }

        [TestMethod]
        public void TestWeixinPay()
        {
            string nonce_str = "wwb3345";
            string body = "124哈哈";
            string out_trade_no = "1234567";
            //string appid = "wxb79a37b190594d96";
            //string mch_id = "123456";
            int total_fee = 1000;
            string spbill_create_ip = "192.168.1.9";
            string ss = Signature.WeixinPayUnifiedorderSignature("", "", "", nonce_str, body, "", out_trade_no, total_fee, spbill_create_ip, "http://www.baidu.com", "JSAPI", "1");
        }

        [TestMethod]
        public void TestweixinPayMethod()
        {
            string s2 = "body=124&nonce_str=wwb3345&key=1";
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] data = System.Text.Encoding.UTF8.GetBytes(s2);
            //加密Byte[]数组
            byte[] result = md5.ComputeHash(data);
            string s3 = System.Text.Encoding.UTF8.GetString(result).ToUpper();//这样出来是乱码

            byte[] result2 = MD5.ComputeHash(data);

            StringBuilder buf = new StringBuilder();
            foreach (byte b in result2)
            {
                buf.Append(b.ToString("X2"));//这样就对了
            }
            string s4 = buf.ToString();
        }

        private int ConvertDateTimeInt(System.DateTime time)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            return (int)(time - startTime).TotalSeconds;
        }
    }
}
