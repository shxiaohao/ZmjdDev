using HJD.CommentService.Contract;
using HJD.HotelServices.Contracts;
using HJDAPI.Common.Helpers;
using HJDAPI.Controllers;
using HJDAPI.Controllers.Common;
using HJDAPI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.APIProxy
{
    public class Comment : BaseProxy
    {
        public static CommentInfoModel GetOneComment(int CommentID)
        {
            string url = APISiteUrl + "api/Comment/GetOneComment";
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.Get(url, "CommentID=" + CommentID, ref cc);
            CommentInfoModel result = JsonConvert.DeserializeObject<CommentInfoModel>(json);
            return result;
        }

        public static UserCanWriteCommentResult GetUserCanWriteCommentOrderID(int hotelID, long userID)
        {
            string url = APISiteUrl + "api/Comment/GetUserCanWriteCommentOrderID";
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.Get(url, "hotelID=" + hotelID + "&userID=" + userID, ref cc);
            UserCanWriteCommentResult result = JsonConvert.DeserializeObject<UserCanWriteCommentResult>(json);
            return result;
        }

        public static CanWriteCommentResult GetUserCanWriteComment(int hotelID, long userID)
        {
            string url = APISiteUrl + "api/Comment/GetUserCanWriteComment";
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.Get(url, "hotelID=" + hotelID + "&userID=" + userID, ref cc);
            CanWriteCommentResult result = JsonConvert.DeserializeObject<CanWriteCommentResult>(json);
            return result;
        }

        public static CommentDefaultInfoModel GetCommentDefaultInfo30(long userid, int hotelid)
        {
            string url = APISiteUrl + "api/Comment/GetCommentDefaultInfo30";
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.Get(url, "hotelid=" + hotelid + "&userid=" + userid, ref cc);
            CommentDefaultInfoModel result = JsonConvert.DeserializeObject<CommentDefaultInfoModel>(json);
            return result;
        }

        public static UserCommentListModel GetUserCommentList(long userID)
        {
            string url = APISiteUrl + "api/Comment/GetUserCommentList";
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.Get(url, "userID=" + userID, ref cc);
            UserCommentListModel result = JsonConvert.DeserializeObject<UserCommentListModel>(json);
            return result;
        }

        public static CommentListModel20 GetUserCommentList30(long userID, int start, int count)
        {
            string url = APISiteUrl + "api/Comment/GetUserCommentList30";
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.Get(url, "userID=" + userID + "&start=" + start + "&count=" + count, ref cc);
            CommentListModel20 result = JsonConvert.DeserializeObject<CommentListModel20>(json);
            return result;
        }

        public static CommentDefaultInfoModel GetCommentDefaultInfo(int hotelid)
        {
            string url = APISiteUrl + "api/Comment/GetCommentDefaultInfo";
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.Get(url, "hotelid=" + hotelid, ref cc);
            CommentDefaultInfoModel result = JsonConvert.DeserializeObject<CommentDefaultInfoModel>(json);
            return result;
        }

        public static SubmitCommentResultEntity SubmitComment(SubmitCommentEntity c)
        {
            string url = APISiteUrl + "api/Comment/SubmitComment";
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.PostJson(url, c, ref cc);
            SubmitCommentResultEntity result = JsonConvert.DeserializeObject<SubmitCommentResultEntity>(json);
            return result;
        }

        public static CommentPhotoUploadResultEntity UploadCommnetPhoto(CommentPhotoUploadEntity photo)
        {
            string url = APISiteUrl + "api/Comment/UploadCommnetPhoto";
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.PostJson(url, photo, ref cc);
            CommentPhotoUploadResultEntity result = JsonConvert.DeserializeObject<CommentPhotoUploadResultEntity>(json);
            return result;
        }

        /// <summary>
        /// 查看点评
        /// </summary>
        /// <param name="CommentID"></param>
        /// <param name="TimeStamp"></param>
        /// <param name="SourceID"></param>
        /// <param name="RequestType"></param>
        /// <param name="sign"></param>
        /// <returns></returns>
        public static CommentInfoModel GetOneComment40(int CommentID, Int64 TimeStamp, int SourceID, string RequestType, string sign)
        {
            string url = APISiteUrl + "api/Comment/GetOneComment40";
            CookieContainer cc = new CookieContainer();
            var photo = new { CommentID = CommentID, TimeStamp = TimeStamp, SourceID = SourceID, RequestType = RequestType, sign = sign };
            string json = HttpRequestHelper.PostJson(url, photo, ref cc);
            CommentInfoModel result = JsonConvert.DeserializeObject<CommentInfoModel>(json);
            return result;
        }

        public static UserCanWriteCommentResult GetUserCanWriteCommentOrderID40(int hotelID, long userID, Int64 TimeStamp, int SourceID, string RequestType, string sign)
        {
            string url = APISiteUrl + "api/Comment/GetUserCanWriteCommentOrderID40";
            CookieContainer cc = new CookieContainer();
            var photo = new { hotelID = hotelID, userID=userID, TimeStamp = TimeStamp, SourceID = SourceID, RequestType = RequestType, sign = sign };
            string json = HttpRequestHelper.PostJson(url, photo, ref cc);
            UserCanWriteCommentResult result = JsonConvert.DeserializeObject<UserCanWriteCommentResult>(json);
            return result;
        }

        public static UserCommentListModel GetUserCommentList40(long userID, Int64 TimeStamp, int SourceID, string RequestType, string sign)
        {
            string url = APISiteUrl + "api/Comment/GetUserCommentList40";
            CookieContainer cc = new CookieContainer();
            var photo = new { userID = userID, TimeStamp = TimeStamp, SourceID = SourceID, RequestType = RequestType, sign = sign };
            string json = HttpRequestHelper.PostJson(url, photo, ref cc);
            UserCommentListModel result = JsonConvert.DeserializeObject<UserCommentListModel>(json);
            return result;
        }

        public static SubmitCommentResultEntity SubmitComment40(SubmitCommentEntity c)
        {
            string url = APISiteUrl + "api/Comment/SubmitComment40";
            CookieContainer cc = new CookieContainer();           
            string json = HttpRequestHelper.PostJson(url, c, ref cc);
            SubmitCommentResultEntity result = JsonConvert.DeserializeObject<SubmitCommentResultEntity>(json);
            return result;
        }

        public static CommentPhotoUploadResultEntity InsertCommnetPhoto40(CommentPhotoInsertEntity photo)
        {
            string url = APISiteUrl + "api/Comment/InsertCommnetPhoto40";
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.PostJson(url, photo, ref cc);
            CommentPhotoUploadResultEntity result = JsonConvert.DeserializeObject<CommentPhotoUploadResultEntity>(json);
            return result;
        }

        public static UserCommentListModel GetUserCommentList(long userID, int start, int count)
        {
            if(!IsProductEvn)
            {
                string url = APISiteUrl + "api/comment/GetUserCommentList";
                CookieContainer cc = new CookieContainer();
                string sstr = string.Format("userID={0}&start={1}&count={2}"
                      , userID,start,count);
                string json = HttpRequestHelper.Get(url, sstr, ref cc);
                UserCommentListModel model = JsonConvert.DeserializeObject<UserCommentListModel>(json);
                return model;
            }
            else{
                return CommentAdapter.GetUserCommentList(userID, start, count);
            }
        }

        public static CommentShareModel GetCommentShareInfo(int CommentID)
        {
            string url = APISiteUrl + "api/comment/GetCommentShareInfo";
            CookieContainer cc = new CookieContainer();
            string sstr = string.Format("CommentID={0}", CommentID);
            string json = HttpRequestHelper.Get(url, sstr, ref cc);
            CommentShareModel model = JsonConvert.DeserializeObject<CommentShareModel>(json);
            return model;
        }

        public static CommentDefaultInfoModel GetCommentDefaultInfoEx(int hotelid)
        {
            string url = APISiteUrl + "api/Comment/GetCommentDefaultInfoEx";
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.Get(url, "hotelid=" + hotelid, ref cc);
            CommentDefaultInfoModel result = JsonConvert.DeserializeObject<CommentDefaultInfoModel>(json);
            return result;
        }

        public static CommentInfoModel2 GetOneCommentEx(int CommentID)
        {
            string url = APISiteUrl + "api/Comment/GetOneCommentEx";
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.Get(url, "CommentID=" + CommentID, ref cc);
            CommentInfoModel2 result = JsonConvert.DeserializeObject<CommentInfoModel2>(json);
            return result;
        }

        public static CommentInfoModel3 GetOneComment50(Comment500Param param)
        {
            string url = APISiteUrl + "api/Comment/GetOneComment500";
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.Get(url, string.Format("commentID={0}&userID={1}&openID={2}&sessionID={3}&terminalType={4}&clientIP={5}&appVersion={6}", param.commentID, param.userID, param.openID, param.sessionID, param.terminalType, param.clientIP, param.appVersion), ref cc);
            CommentInfoModel3 result = JsonConvert.DeserializeObject<CommentInfoModel3>(json);
            result.commentContent = result.commentContent.Replace("\n", "<br/>").Replace("\r", "<br/>");
            return result;
        }

        public static CommentSharePageData GetCommentSharePageData(int commentID, long commentUserID, long orderID = 0, int hotelID = 0)
        {
            string url = APISiteUrl + "api/Comment/GetCommentSharePageData";
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.Get(url, string.Format("CommentID={0}&commentUserID={1}&orderID={2}&hotelID={3}", commentID, commentUserID, orderID, hotelID), ref cc);
            CommentSharePageData result = JsonConvert.DeserializeObject<CommentSharePageData>(json);
            return result;
        }

        public static BasePostResult InsertUserRecord(CommonRecordParam recordParam)
        {
            string url = APISiteUrl + "api/Comment/InsertUserRecord";
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.PostJson(url, recordParam, ref cc);
            BasePostResult result = JsonConvert.DeserializeObject<BasePostResult>(json);
            return result;
        }

        public static RecommendCommentListModel GetRecommendCommentListModel(RecommendCommentListQueryParam param)
        {
            string url = APISiteUrl + "api/Comment/GetRecommendCommentListModel";
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.Get(url, string.Format("start={0}&count={1}", param.start, param.count), ref cc);
            RecommendCommentListModel result = JsonConvert.DeserializeObject<RecommendCommentListModel>(json);
            return result;
        }

        public static TrackCodeData GenTrackCodeResult4Share(GenTrackCodeParam param)
        {
            string url = APISiteUrl + "api/Comment/GenTrackCodeResult4Share";
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.PostJson(url, param, ref cc);
            TrackCodeData result = JsonConvert.DeserializeObject<TrackCodeData>(json);
            return result;
        }

        public static ReviewResult40 GetCommentInfosByPId(int pid, int start = 0, int count = 3)
        {
            string url = APISiteUrl + "api/Comment/GetCommentInfosByPId";
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.Get(url, string.Format("pid={2}&start={0}&count={1}", start, count, pid), ref cc);
            ReviewResult40 result = JsonConvert.DeserializeObject<ReviewResult40>(json);
            return result;
        }

        public static RecommendCommentListModel GetPublishedCommentList(RecommendCommentParam param)
        {
            string url = APISiteUrl + "api/Comment/GetPublishedCommentList";
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.Get(url, string.Format("start={0}&count={1}&lat={2}&lng={3}&districtid={4}", param.start, param.count, param.lat, param.lng, param.districtid), ref cc);
            return JsonConvert.DeserializeObject<RecommendCommentListModel>(json);
        }
    }
}