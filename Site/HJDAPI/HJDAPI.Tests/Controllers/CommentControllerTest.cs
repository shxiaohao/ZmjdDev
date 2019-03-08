using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HJDAPI.Controllers;
using HJDAPI.Models;
using System.Net;
using HJDAPI.Common.Helpers;
using Newtonsoft.Json;
using HJD.CouponService.Contracts.Entity;

namespace HJDAPI.Tests.Controllers
{
    [TestClass]
    public class CommentControllerTest
    {
        //[TestMethod]
        //public void GetUserCommentListTest()
        //{
        //   UserCommentListModel model = Comment.GetUserCommentList(4513339);           
        //}

        //[TestMethod]
        //public void GetOneCommentTest()
        //{
        //    CommentInfoModel model = Comment.GetOneComment(4513339);
        //}

        //[TestMethod]
        //public void  GetUserCanWriteCommentOrderIDTest()
        //{
        //    UserCanWritingCommentResult result = Comment.GetUserCanWriteCommentOrderID(8137,4513339);
        //}

        //[TestMethod]
        //public void GetCommentDefaultInfo()
        //{
        //    CommentDefaultInfoModel model = Comment.GetCommentDefaultInfo(8137);
        //}

        //[TestMethod]
        //public void SubmitComment()
        //{
        //    SubmitCommentEntity c = new SubmitCommentEntity();
        //    c.UserID = 4513339;
        //    Comment.SubmitComment(c);
        //}

        //[TestMethod]
        //public void UploadCommnetPhoto()
        //{
        //    CommentPhotoUploadEntity photo = new CommentPhotoUploadEntity();
        //    photo.CommentID = 1;
        //    Comment.UploadCommnetPhoto(photo);
        //}

        [TestMethod]
        public void GetHomePageDataTest()
        {
            if (false)
                 HotelAdapter.GetHomePageData(2);
            else
            {
                string url = "http://192.168.1.113:8000/" + "api/hotel/GetHomePageData";
                string postDataStr = string.Format("districtid={0}"
                  , 2);

                CookieContainer cc = new CookieContainer();

                string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
                JsonConvert.DeserializeObject<HomePageData>(json);
            }
        }
        [TestMethod]
        public void TestGetCommentList() {
            string url = "http://192.168.2.100:500/" + "api/comment/GetUserCommentList";
            long userID = 4512004;
            CookieContainer cc = new CookieContainer();
            int start = 0;
            int count = 0;
            var ss = new { userID = userID, start = start,count = count };
            //string sstr = JsonConvert.SerializeObject(ss);
            string sstr = string.Format("userID={0}&start={1}&count={2}"
                  , userID,start,count);
            //string postDataStr = string.Format("userID={0}", userID);
            string json = HttpRequestHelper.Get(url, sstr, ref cc);
            JsonConvert.DeserializeObject<UserCommentListModel>(json);
        }

        [TestMethod]
        public void GetCommentShareInfo()
        {
            string url = "http://192.168.2.100:500/" + "api/comment/GetCommentShareInfo";
            CookieContainer cc = new CookieContainer();
            string sstr = string.Format("CommentID={0}", 365);
            string json = HttpRequestHelper.Get(url, sstr, ref cc);
            CommentShareModel model = JsonConvert.DeserializeObject<CommentShareModel>(json);
        }

        [TestMethod]
        public void TestXXXX()
        {
            string url = "http://192.168.1.100:500/" + "api/Comment/GetUserCommentList";
            CookieContainer cc = new CookieContainer();
            long userID = 4512004;
            string json = HttpRequestHelper.Get(url, "userID=" + userID, ref cc);
            UserCommentListModel result = JsonConvert.DeserializeObject<UserCommentListModel>(json);
            if (result.CommentDteailList != null)
            {
                //result.CommentDteailList.ForEach(i => i.shareModel = Comment.GetCommentShareInfo(i.commentItem.CommentID));
            }
        }

        [TestMethod]
        public void TestYYYYY()
        {
            string url = "http://192.168.1.20:500/" + "api/Coupon/UpdateCashCoupon";
            CookieContainer cc = new CookieContainer();
            var xx = new { id = 4, userId = 4512004, typeId = 100, sourceId = 1, state = 1 };
            string xxx = JsonConvert.SerializeObject(xx,Formatting.Indented);
            string json = HttpRequestHelper.PostJson(url, xxx, ref cc);
            OriginCouponResult ss = JsonConvert.DeserializeObject<OriginCouponResult>(json);
        }

        [TestMethod]
        public void TestGetUserCanWriteCommentOrderID()
        {
            string url = "http://api.zmjiudian.com/" + "api/Comment/GetUserCanWriteCommentOrderID";
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.Get(url, "hotelID=" + 597435 + "&userID=" + 4513693, ref cc);
            UserCanWriteCommentResult result = JsonConvert.DeserializeObject<UserCanWriteCommentResult>(json);
        }
    }
}
