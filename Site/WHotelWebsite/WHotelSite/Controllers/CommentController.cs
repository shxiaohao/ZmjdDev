using HJD.HotelPrice.Contract.DataContract.Order;
using HJDAPI.APIProxy;
using HJDAPI.Common.Security;
using HJDAPI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WHotelSite.Common;
using WHotelSite.Models;
using WHotelSite.Params.Hotel;
using WHotelSite.Params.Order;
using WHotelSite.ViewModels;

namespace WHotelSite.Controllers
{
    public class CommentController : BaseController
    {
        public const string AccessProtocal_IsApp = "whotelapp://www.zmjiudian.com/";
        public const string AccessProtocalPage_IsApp = "whotelapp://gotopage?url=http://www.zmjiudian.com/";

        public const string AccessProtocal_UnApp = "/inspector/jump?jumpurl=whotelapp://www.zmjiudian.com/";  //"http://www.zmjiudian.com/hotel/";  //"http://m1.zmjiudian.com/";
        public const string AccessProtocalPage_UnApp = "/inspector/jump?jumpurl=whotelapp://www.zmjiudian.com/gotopage?url=http://www.zmjiudian.com/";
        //public const string AccessProtocalPage_UnApp = "/inspector/jump?jumpurl=whotelapp://www.zmjiudian.com/gotopage?url=http://192.168.1.11:8081/";

        //
        // GET: /Review/

        public ActionResult Index()
        {
            return View();
        }

        // 点评列表
        public ActionResult List()
        {
            if (!UserState.IsLogin)
            {
                return Json(new { Message = "登录后才能查看点评", Success = 1 }, JsonRequestBehavior.AllowGet);
            }
            OrderParam param = new OrderParam(this);
            ViewBag.isMobile = Utils.IsMobile();
            ViewBag.param = param;
            if (param.UserID == 0)
            {
                return null;
            }
            UserCommentListModel model = Comment.GetUserCommentList(param.UserID, param.Start, param.Count);

            CommentListModel20 commentListModel20 = Comment.GetUserCommentList30(param.UserID, param.Start, param.Count);
            
            Session["CommentInfoSessionKey"] = null;//清点评session

            ViewBag.DoneCommentList = commentListModel20.DoneCommentList;//已完成的点评内容

            return View("List", model);
        }

        // 点评详情
        public ActionResult Detail(int CommentID)
        {
            var isApp = IsApp();
            if (IsApp())
            {
                ViewBag.AccessProtocal = AccessProtocal_IsApp;
                ViewBag.AccessProtocalPage = AccessProtocalPage_IsApp;
            }
            else
            {
                ViewBag.AccessProtocal = AccessProtocal_UnApp;
                ViewBag.AccessProtocalPage = AccessProtocalPage_UnApp;
            }
            ViewBag.IsApp = isApp;

            //Weixin
            bool isInWeixin = Request.UserAgent.IndexOf("MicroMessenger") > 0 ? true : false;
            ViewBag.isInWeixin = isInWeixin;

            CommentInfoModel3 commentEntity = Comment.GetOneComment50(new Comment500Param() 
            {
                commentID = CommentID,
                userID = UserState.UserID,
                sessionID = UserState.RecordSessionId(), 
                busniessId = CommentID,
                clientIP = HttpContext.Request.UserHostAddress,
                recordType = 2,                                         //分享类型（1.分享 2.浏览）
                businessType = 1,                                       //业务类型（1.点评 2.酒店）
                terminalType = Utils.GetTerminalId(Request.UserAgent),  //终端类型（0.wap  1.web  2.iOS  3.android  4.weixin）
                appVersion = DeviceVer
            }) 
            ?? new CommentInfoModel3();

            ViewBag.CommentInfo = commentEntity;
            ViewBag.CmId = CommentID;

            //根据分享者获取分享者的信息
            var channelId = 0;
            if (Session["ChannelID"] != null)
            {
                try
                {
                    var SessionChannelID = Session["ChannelID"].ToString().Replace(" ", "+");
                    string originStr = HJDAPI.Common.Security.DES.Decrypt(SessionChannelID);
                    int.TryParse(!string.IsNullOrWhiteSpace(originStr) ? originStr.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)[1] : "0", out channelId);
                }
                catch (Exception ex)
                {

                }
            }
            HJD.AccountServices.Entity.MemberProfileInfo shareUserInfo = HJDAPI.APIProxy.account.GetCurrentUserInfo(channelId) ?? new HJD.AccountServices.Entity.MemberProfileInfo();
            ViewBag.ShareUserInfo = shareUserInfo;

            return View("Detail", commentEntity);
        }

        public ActionResult ShareDetail(int CommentID)
        {
            var isApp = IsApp();
            if (IsApp())
            {
                ViewBag.AccessProtocal = AccessProtocal_IsApp;
                ViewBag.AccessProtocalPage = AccessProtocalPage_IsApp;
            }
            else
            {
                ViewBag.AccessProtocal = AccessProtocal_UnApp;
                ViewBag.AccessProtocalPage = AccessProtocalPage_UnApp;
            }
            ViewBag.IsApp = isApp;

            //Weixin
            bool isInWeixin = Request.UserAgent.IndexOf("MicroMessenger") > 0 ? true : false;
            ViewBag.isInWeixin = isInWeixin;

            CommentInfoModel3 commentEntity = Comment.GetOneComment50(new Comment500Param() 
            {
                commentID = CommentID, 
                userID = UserState.UserID,
                sessionID = UserState.RecordSessionId(), 
                busniessId = CommentID,
                clientIP = HttpContext.Request.UserHostAddress,
                recordType = 2,                                         //分享类型（1.分享 2.浏览）
                businessType = 1,                                       //业务类型（1.点评 2.酒店）
                terminalType = Utils.GetTerminalId(Request.UserAgent),  //终端类型（0.wap  1.web  2.iOS  3.android  4.weixin）
                appVersion = DeviceVer,
            }) 
            ?? new CommentInfoModel3();

            ViewBag.CommentInfo = commentEntity;
            ViewBag.CmId = CommentID;

            //根据点评和用户获取分享的配置参数
            CommentSharePageData sharePageData = Comment.GetCommentSharePageData(CommentID, (long)0);
            ViewBag.SharePageData = sharePageData;

            //根据分享者获取分享者的信息
            var channelId = 0;
            if (Session["ChannelID"] != null)
            {
                try
                {
                    var SessionChannelID = Session["ChannelID"].ToString().Replace(" ", "+");
                    string originStr = HJDAPI.Common.Security.DES.Decrypt(SessionChannelID);
                    int.TryParse(!string.IsNullOrWhiteSpace(originStr) ? originStr.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)[1] : "0", out channelId);   
                }
                catch (Exception ex)
                {
                    
                }
            }
            HJD.AccountServices.Entity.MemberProfileInfo shareUserInfo = HJDAPI.APIProxy.account.GetCurrentUserInfo(channelId) ?? new HJD.AccountServices.Entity.MemberProfileInfo();
            ViewBag.ShareUserInfo = shareUserInfo;

            ViewBag.BusniessId = CommentID;
            ViewBag.UserId = UserState.UserID;
            ViewBag.AppVersion = DeviceVer;

            return View("ShareDetail", commentEntity);
        }

        public ActionResult RecordCommentShareInfo(int busniessId = 0, int userid = 0, int recordType = 1, string appVersion = "0")
        {
            string userAgent = Request.UserAgent;

            var recordEntity = new CommonRecordParam();
            recordEntity.busniessId = busniessId;
            recordEntity.userID = userid;
            recordEntity.sessionID = UserState.RecordSessionId();
            recordEntity.clientIP = HttpContext.Request.UserHostAddress;
            recordEntity.businessType = 1;          //业务类型（1.点评 2.酒店）
            recordEntity.recordType = recordType;   //分享类型（1.分享 2.浏览）
            recordEntity.terminalType = Utils.GetTerminalId(userAgent);  //终端类型（0.wap  1.web  2.iOS  3.android  4.weixin）
            recordEntity.appVersion = appVersion;   //平台版本
            Comment.InsertUserRecord(recordEntity);

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        // 点评详情
        public ActionResult Section()
        {
            if (!UserState.IsLogin)
            {
                return Json(new { Message = "必须登录后才能写点评", Success = 1 }, JsonRequestBehavior.AllowGet);
            }

            //string userAgent = this.Request.UserAgent;
            //double version = double.Parse(this.Request.Browser.Version);
            //if (userAgent.IndexOf("MSIE ") > -1 && version < 10)
            //{
            //    return Json(new { Message = "您当前使用的IE浏览器版本过低，使用点评功能需要安装最新版的IE或其他浏览器", Success = 2 }, JsonRequestBehavior.AllowGet);
            //}

            int hotelID = 0;
            int section = 0;
            long orderID = 0;

            int.TryParse(this.Request.Params["hotel"],out hotelID);
            int.TryParse(this.Request.Params["section"],out section);
            long.TryParse(this.Request.Params["order"],out orderID);
            
            if(hotelID <= 0){
                return Json(new { Message = "缺少酒店ID无法写点评,请联系管理员！", Success = 2 }, JsonRequestBehavior.AllowGet);
            }

            CommentDefaultInfoModel infoModel = Comment.GetCommentDefaultInfo30(UserState.UserID, hotelID);

            ViewBag.SectionNum = section;
            ViewBag.OrderID = orderID;
            ViewBag.HotelID = hotelID;
            ViewBag.BlockInfoCount = infoModel.BlockInfo != null ? infoModel.BlockInfo.Count : 0;

            PackageOrderInfo20 info20 = null;
            if(orderID != 0){
                info20 = Order.GetPackageOrderInfo20(orderID, UserState.UserID);
            }

            CommentViewModel comment = new CommentViewModel { CommentInfoModel = infoModel, OrderInfo20 = info20 };

            return View(comment);
        }

        // 点评详情
        public ActionResult SubmitSection()
        {
            int score = 0;
            bool recommend = false;
            List<int> tagIDs = null;
            List<CommentTagAddInfo> tagAddInfos = null;

            int.TryParse(this.Request.Params["score"], out score);
            bool.TryParse(this.Request.Params["recommend"], out recommend);            

            if (this.Request.Params["tagIDs"] != null)
            {
                tagIDs = JsonConvert.DeserializeObject<List<int>>(this.Request.Params["tagIDs"]);//this.Request.Params["tagIDs"].Split(',')
            }

            if (this.Request.Params["tagAddInfos"] != null)
            {
                tagAddInfos = JsonConvert.DeserializeObject<List<CommentTagAddInfo>>(this.Request.Params["tagAddInfos"]);
            }
            
            int hotelID = 0;
            int section = 0;
            long orderID = 0;
            bool isSubmit = false;//提交和上一步按钮在一起 很容易产生同样的效果 要区分
            
            int.TryParse(this.Request.Params["hotel"], out hotelID);
            int.TryParse(this.Request.Params["section"], out section);
            long.TryParse(this.Request.Params["order"], out orderID);
            bool.TryParse(this.Request.Params["isSubmit"], out isSubmit);

            if(hotelID <= 0){
                return Json(new OperationResult() { Success = false, Message = "缺少酒店ID无法提交点评内容，请联系管理员！" });
            }

            CommentDefaultInfoModel infoModel = Comment.GetCommentDefaultInfo(hotelID);
            int blockInfoCount = infoModel.BlockInfo != null ? infoModel.BlockInfo.Count : 0;

            /*session存客户需求*/
            SubmitCommentEntity commentEntity = Session["CommentInfoSessionKey"] != null ? (SubmitCommentEntity)Session["CommentInfoSessionKey"] : null;
            if (section < blockInfoCount && !isSubmit)
            {
                CommentBlockInfo tempData = infoModel.BlockInfo[section];//section即数组索引
                int blockCategory = tempData.BlockCategory;
                if (true)
                {
                    Session["CommentInfoSessionKey"] = null;
                    commentEntity = new SubmitCommentEntity();

                    commentEntity.HotelID = hotelID;
                    commentEntity.OrderID = orderID;
                    commentEntity.UserID = UserState.UserID;


                    commentEntity.TagIDs = new List<int>();
                    if (tagIDs != null && tagIDs.Count != 0)
                    {                        
                        foreach (int i in tagIDs)
                        {
                            commentEntity.TagIDs.Add(i);
                        }
                    }
                    commentEntity.addInfos = new List<AddationalInfo>();
                    
                    if (tagAddInfos != null && tagAddInfos.Count != 0)
                    {
                        foreach (CommentTagAddInfo addInfo in tagAddInfos)
                        {
                            commentEntity.addInfos.Add(new AddationalInfo() { CategoryID = addInfo.catID, AddationalComment = addInfo.info });
                        }
                    }

                    Session["CommentInfoSessionKey"] = commentEntity;
                }
            }
            else
            {
                if (score != 0)
                {
                    commentEntity.score = score;
                    commentEntity.recommend = recommend;

                    //40版本App 改动 导致最后一页打分出现标签块 需调整
                    if (tagIDs != null && tagIDs.Count > 0)
                    {
                        foreach (var tagID in tagIDs)
                        {
                            if (!commentEntity.TagIDs.Contains(tagID))
                            {
                                commentEntity.TagIDs.AddRange(tagIDs);
                            }
                        }
                    }

                    if (tagAddInfos != null && tagAddInfos.Count > 0)
                    {
                        foreach (CommentTagAddInfo addInfo in tagAddInfos)
                        {
                            //ToDo 验证多次提交 最后一页 ToSolove
                            if (!commentEntity.addInfos.Exists(_=>_.CategoryID == addInfo.catID))
                            {
                                commentEntity.addInfos.Add(new AddationalInfo() { CategoryID = addInfo.catID, AddationalComment = addInfo.info });
                            }
                        }
                    }

                    Session["CommentInfoSessionKey"] = commentEntity;
                }
            }
            /**/
            //最低一分 为0说明是正常返回view视图 非0 则需要最终提交点评结果了
            if (score != 0 && isSubmit)
            {
                return Json(Comment.SubmitComment(commentEntity));
            }
            else
            {
                return Json(new OperationResult() { Success = true, Message = "" });
            }
        }

        public ActionResult InsertCommnetPhoto40(CommentPhotoInsertEntity photo)
        {
            CommentPhotoUploadResultEntity result = new CommentPhotoUploadResultEntity();
            if (photo.CommentID == 0)
            {
                return Json(new { Message = "没有点评ID", Success = 1});
            }
            else if (string.IsNullOrEmpty(photo.PhotoSURL))
            {
                return Json(new { Message = "没有照片链接", Success = 2 });
            }
            photo.Sign = Signature.GenSignature(photo.TimeStamp, photo.SourceID, Config.MD5Key, photo.RequestType);
            result = Comment.InsertCommnetPhoto40(photo);
            return Json(result);
        }
        
        public  ActionResult GetUserCanWriteCommentOrderID(int hotelID)
        {
            UserCanWriteCommentResult result = null;
            if (hotelID == 0)
            {
                result = new UserCanWriteCommentResult();
                result.msg = "参数错误";
                result.orderID = 0;
            }
            else if(UserState.UserID == 0){
                result = new UserCanWriteCommentResult();
                result.msg = "请先登录";
                result.orderID = 0;
            }
            else
            {
                result = Comment.GetUserCanWriteCommentOrderID(hotelID, UserState.UserID);
            }
            return Json(result,JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetUserCanWriteComment(int hotelID)
        {
            CanWriteCommentResult result = null;
            if (hotelID == 0)
            {
                result = new CanWriteCommentResult();
                result.Message = "酒店参数错误";
                result.orderID = 0;
            }
            else if (UserState.UserID == 0)
            {
                result = new CanWriteCommentResult();
                result.Message = "请先登录";
                result.orderID = 0;
            }
            else
            {
                result = Comment.GetUserCanWriteComment(hotelID, UserState.UserID);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetCommentShareInfo(int CommentID)
        {
            return Json(Comment.GetCommentShareInfo(CommentID));
        }

        public ActionResult Finish(int CommentID)
        {
            ViewBag.CommentID = CommentID;
            CommentShareModel shareInfo = Comment.GetCommentShareInfo(CommentID);
            ViewBag.CommentShareModel = shareInfo;
            return View();
        }
        
        /// <summary>
        /// 积分规则
        /// </summary>
        /// <returns></returns>
        public ActionResult PointsRule()
        {
            return View();
        }

        /// <summary>
        /// 点评规则
        /// </summary>
        /// <returns></returns>
        public ActionResult CommentRule()
        {
            return View();
        }

        /// <summary>
        /// 点评已提交页面（目前APP在用）
        /// </summary>
        /// <returns></returns>
        public ActionResult CommentSubCompleted(int commentId, int userid = 0, int share = 0)
        {
            var curUserID = Convert.ToInt64(UserState.UserID > 0 ? UserState.UserID : userid);
            if (curUserID <= 0)
            {
                curUserID = _ContextBasicInfo.AppUserID;
            }
            if (curUserID >= 0 && _ContextBasicInfo.AppUserID <= 0)
            {
                _ContextBasicInfo.AppUserID = curUserID;
            }
            ViewBag.UserId = curUserID;
            userid = Convert.ToInt32(curUserID);

            //根据点评和用户获取分享的配置参数
            CommentSharePageData sharePageData = Comment.GetCommentSharePageData(commentId, (long)userid);
            ViewBag.SharePageData = sharePageData;

            //分享链接
            var shareLink = "whfriend://comment?title={0}&photoUrl={1}&shareLink={2}&nextUrl={3}&content={4}&shareType={5}";
            shareLink = string.Format(shareLink, HttpUtility.UrlEncode(sharePageData.commentShare.title), HttpUtility.UrlEncode(sharePageData.commentShare.photoUrl), HttpUtility.UrlEncode(sharePageData.commentShare.shareLink), HttpUtility.UrlEncode(sharePageData.commentShare.returnUrl), HttpUtility.UrlEncode(sharePageData.commentShare.Content), "{0}");

            //微信好友
            var shareLink_WeixinFriend = string.Format(shareLink, 1);
            ViewBag.ShareLink_WeixinFriend = shareLink_WeixinFriend;

            //微信朋友圈
            var shareLink_WeixinLoop = string.Format(shareLink, 2);
            ViewBag.ShareLink_WeixinLoop = shareLink_WeixinLoop;

            //QQ好友
            var shareLink_QqFriend = string.Format(shareLink, 5);
            ViewBag.ShareLink_QqFriend = shareLink_QqFriend;

            //QQ空间
            var shareLink_QqZone = string.Format(shareLink, 3);
            ViewBag.ShareLink_QqZone = shareLink_QqZone;

            //新浪
            var shareLink_Sina = string.Format(shareLink, 4);
            ViewBag.ShareLink_Sina = shareLink_Sina;

            //复制链接
            var shareLink_CopyLink = string.Format(shareLink, 6);
            ViewBag.ShareLink_CopyLink = shareLink_CopyLink;

            //更多
            var shareLink_More = string.Format(shareLink, 7);
            ViewBag.ShareLink_More = shareLink_More;

            //打开原生分享bar
            var shareLink_Native = string.Format(shareLink, 0);
            ViewBag.ShareLink_Native = shareLink_Native;

            //当前系统环境（ios | android）
            ViewBag.AppType = AppType();

            //检查当前版本是否大于等于4.4版本
            ViewBag.IsThanVer4_4 = IsThanVer4_4();

            //分享成功，则更新状态
            if (share == 1)
            {
                try
                {
                    string userAgent = Request.UserAgent;

                    var recordEntity = new CommonRecordParam();
                    recordEntity.busniessId = commentId;
                    recordEntity.userID = userid;
                    recordEntity.sessionID = userid.ToString();
                    recordEntity.clientIP = HttpContext.Request.UserHostAddress;
                    recordEntity.businessType = 1;  //业务类型（1.点评 2.酒店）
                    recordEntity.recordType = 1;    //分享类型（1.分享 2.浏览）
                    recordEntity.terminalType = Utils.GetTerminalId(userAgent);  //终端类型（0.wap  1.web  2.iOS  3.android  4.weixin）
                    recordEntity.appVersion = "";   //平台版本
                    Comment.InsertUserRecord(recordEntity);
                }
                catch (Exception ex)
                {
                    
                }
            }

            return View();
        }
    }
}