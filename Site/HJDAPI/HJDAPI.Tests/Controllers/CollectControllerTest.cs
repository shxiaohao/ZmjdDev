using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PersonalServices.Contract;
using HJDAPI.Controllers;
using PersonalServices.Contracts.Entity;
using HJDAPI.Models;
using System.Collections.Generic;
using System.Net;
using HJDAPI.Common.Helpers;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Web.Helpers;

namespace HJDAPI.Tests.Controllers
{
    [TestClass]
    public class CollectControllerTest
    {
        [TestMethod]
        public void TestAddCollect()
        {
            //CollectParam cp = new CollectParam();
            //cp.UserID = 4513339;
            //cp.IsCollect = true;
            //cp.Items = new List<CollectParamItem>();
            //cp.Items.Add(new CollectParamItem() { HotelID = 592915, InterestID = 1 });

            //CollectOptionResult result = CollectAdapter.Add(cp);
            CollectParam cp = new CollectParam();
            cp.UserID = 4513339;
            cp.IsCollect = true;
            cp.Items = new List<CollectParamItem>();
            cp.Items.Add(new CollectParamItem() { HotelID = 592915, InterestID = 1 });
            string url = "http://192.168.2.100:500/api/Collect/Add";
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.PostJson(url, cp, ref cc);
            CollectOptionResult result = JsonConvert.DeserializeObject<CollectOptionResult>(json);
        }

        //[TestMethod]
        //public void TestAddCollectProxy()
        //{
        //    CollectParam cp = new CollectParam();
        //    cp.UserID = 4513339;
        //    cp.IsCollect = true;
        //    cp.Items = new List<CollectParamItem>();
        //    cp.Items.Add(new CollectParamItem() { HotelID = 592915, InterestID = 1 });
        //    CollectOptionResult result = Collect.Add(cp);
        //}

        //[TestMethod]
        //public void TestRemoveCollectProxy()
        //{
        //    CollectParam cp = new CollectParam();
        //    cp.UserID = 4513339;
        //    cp.IsCollect = true;
        //    cp.Items = new List<CollectParamItem>();
        //    cp.Items.Add(new CollectParamItem() { HotelID = 592915, InterestID = 1 });
        //    CollectOptionResult result = Collect.Remove(cp);
        //}

        [TestMethod]
        public void TestBookVoucherHotel()
        {
            BookInspectorHotelParam param = new BookInspectorHotelParam();
            param.checkin = "2016-09-22";
            param.checkout = "2016-09-23";
            param.hotelid = 5425;
            param.HVID = 6;
            //param.VID = 5;

            param.userid = 4512106;
            InspectorController insp = new InspectorController();
            ResultEntity rEntirty = insp.BookVoucherHotel(param);

        }


        [TestMethod]
        public void TestGetCollectHotel()
        {
            CollectParam cp = new CollectParam();
            cp.UserID = 4513339;

            CollectHotelResult result = new CollectHotelResult();

            List<long> hotelIdList = CollectAdapter.GetCollectIdList(cp.UserID);
            if (hotelIdList != null)
            {
                //result.Hotels = HotelAdapter.GetCollectHotelList(hotelIdList.ConvertAll<int>(i => (int)i));
                result.TotalCount = 0;
            }
        }

        [TestMethod]
        public void TestGetCollectHotel2()
        {
            CollectParam cp = new CollectParam();
            cp.UserID = 4512004;//4513339 4512004;

            //CollectHotelResult result = new CollectHotelResult();

            //result = Collect.GetCollectHotelList(cp);            

            string url = "http://192.168.2.100:500/api/Collect/GetCollectHotelList";
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.PostJson(url, cp, ref cc);
            CollectHotelResult result = JsonConvert.DeserializeObject<CollectHotelResult>(json);
        }

        [TestMethod]
        public void SyncCollectListTest()
        {
            CollectParam cp = new CollectParam();
            cp.UserID = 4512622;
            cp.IsCollect = true;
            cp.Items = new List<CollectParamItem>();
            cp.Items.Add(new CollectParamItem() { HotelID = 320020, InterestID = 100002 });
            cp.Items.Add(new CollectParamItem() { HotelID = 498513, InterestID = 100002 });

            CollectHotelResult result = new CollectHotelResult();
            if (cp.UserID == 0)
            {
                result.Message = "用户ID不能为空";
                result.Success = 1;
            }
            if (cp.Items == null || cp.Items.Count == 0)
            {
                result.Message = "没有酒店ID";
                result.Success = 2;
            }

            //CollectOptionResult r1 = CollectAdapter.Add(cp);
            string url = "http://192.168.2.100:500/api/Collect/SyncCollectList";
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.PostJson(url, cp, ref cc);
            result = JsonConvert.DeserializeObject<CollectHotelResult>(json);
        }

        [TestMethod]
        public void AttrituteTest()
        {
            CollectParam param = new CollectParam() { };
            var a = param.UserID;
            Debug.WriteLine(a);
        }

        [TestMethod]
        public void TestNewParam() {
            string url = "http://192.168.2.100:500/api/Collect/SyncCollectList";
            CookieContainer cc = new CookieContainer();

            CollectParamModel cp = new CollectParamModel();
            cp.UserID = 4512622;
            cp.IsCollect = true;
            cp.Items = new List<CollectParamItemModel>();
            cp.Items.Add(new CollectParamItemModel() { HotelID = 320020, InterestID = 100002 });
            cp.Items.Add(new CollectParamItemModel() { HotelID = 498513, InterestID = 100002 });
                        
            var a = new {UserID=cp.UserID,Items = cp.Items };
            string json = HttpRequestHelper.PostJson(url, cp, ref cc);
            CollectHotelResult result = JsonConvert.DeserializeObject<CollectHotelResult>(json);
        }

        [TestMethod]
        public void TestGetCollectHotelList()
        {
            string url = "http://192.168.2.100:500/" + "api/Collect/GetCollectHotelList";
            CookieContainer cc = new CookieContainer();
            CollectParamModel model = new CollectParamModel();
            model.UserID = 4512622;
            string json = HttpRequestHelper.PostJson(url, model, ref cc);
            CollectHotelResult result = JsonConvert.DeserializeObject<CollectHotelResult>(json);
        }
        [TestMethod]
        public void IsCollect()
        {
            string url = "http://192.168.2.100:500/" + "api/Collect/IsCollect";
            CookieContainer cc = new CookieContainer();
            var model = new { UserID = 45004, HotelID = 498513 };
            long UserID = 4512004, HotelID = 595106;
            string ss = string.Format("UserID={0}&HotelID={1}", UserID, HotelID);            
            string json = HttpRequestHelper.Get(url, ss, ref cc);
            CollectHotelResult result = JsonConvert.DeserializeObject<CollectHotelResult>(json);
        }
    }
}
