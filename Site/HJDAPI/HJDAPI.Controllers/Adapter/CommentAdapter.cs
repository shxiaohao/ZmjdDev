
using HJD.CommentService.Contract;
using HJD.Framework.WCF;
using HJD.HotelPrice.Contract;
using HJD.HotelPrice.Contract.DataContract;
using HJD.HotelPrice.Contract.DataContract.Order;
using HJD.HotelServices.Contracts;
using HJD.PhotoServices.Contracts;
using HJD.PhotoServices.Entity;
using HJDAPI.Common.Helpers;
using HJDAPI.Controllers.Adapter;
using HJDAPI.Controllers.Common;
using HJDAPI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc.Html;

namespace HJDAPI.Controllers
{
    public class CommentAdapter
    {
        public static IHotelPriceService PriceService = ServiceProxyFactory.Create<IHotelPriceService>("BasicHttpBinding_IHotelPriceService");
        public static ICommentService commentService = ServiceProxyFactory.Create<ICommentService>("ICommentService");
        public static IHotelService hotelService = ServiceProxyFactory.Create<IHotelService>("BasicHttpBinding_IHotelService");
        public static IPhotoService photoService = ServiceProxyFactory.Create<IPhotoService>("BasicHttpBinding_IPhotoService");

        public static List<InterestCommentEntity> GetHotelInterestComment(int featuredID, List<int> hotelIDs)
        {
            return commentService.GetHotelInterestComment(featuredID, hotelIDs);
        }

        public static CommentInfoModel GetOneComment(int ID, long appUserID = 0)
        {
            CommentInfoModel m = new CommentInfoModel();
            m.commentInfo = commentService.GetOneCommentInfo(ID);
            m.commentHTML = GenWHHotelCommentDes(m.commentInfo);
            m.photoInfo = PhotoAdapter.GetWHHotelCommentPhotoByWritings(new List<int> { ID }).Select(p => p.PhotoUrl[PhotoSizeType.small]).ToList();

            int hotelID = m.commentInfo.Comment.HotelID;

            DateTime CheckInDate = DateTime.Now.AddDays(1);
            DateTime CheckOutDate = DateTime.Now.AddDays(2);
            string CheckIn = CheckInDate.ToString("yyyy-MM-dd");
            string CheckOut = CheckOutDate.ToString("yyyy-MM-dd");

            if (hotelID > 0)
            {
                HotelItem hi = ResourceAdapter.GetHotel(hotelID, 0);//;//new HotelAdapter().Get3(hotelID, CheckIn, CheckOut, "www", 0);//interestID暂时get30没有用
                m.hotelPic = hi.Picture;//hi.Pics != null && hi.Pics.Count > 0 ? hi.Pics[0] : "";
                m.hotelStar = hi.Star;
                m.hotelName = hi.Name;
            }
            else
            {
                m.hotelPic = "";
                m.hotelStar = 0;
                List<CommentAddHotelEntity> addHotellist = HotelAdapter.HotelService.GetUserAddHotelByComment(ID);//获得用户添加酒店名称
                if (addHotellist != null && addHotellist.Count != 0)
                {
                    m.hotelName = addHotellist.First().HotelName;
                }
            }

            long userId = m.commentInfo.Comment.UserID;
            string nickName = AccountAdapter.GetNickName(userId);
            m.nickName = string.IsNullOrWhiteSpace(nickName) ? "匿名" : nickName;

            m.shareModel = DescriptionHelper.GetShareModel(m, appUserID == 0 ? userId : appUserID);
            return m;
        }

        public static CommentInfoEntity GenWHHotelcommentDescription(int ID)
        {
            return commentService.GenWHHotelcommentDescription(ID);
        }

        public static List<CommentInfoEntity> GetComments(List<int> writingList, TimeLog parentLog = null)
        {
            TimeLog log = new TimeLog("GetComments", 300, parentLog);
            try
            {
                if (writingList.Count == 0)
                {
                    return new List<CommentInfoEntity>();
                }
                List<PHSPhotoInfoEntity> pl = PhotoAdapter.GetWHHotelCommentPhotoByWritings(writingList);
                log.AddLog("GetWHHotelCommentPhotoByWritings");
                List<CommentInfoEntity> cl = commentService.GetComments(writingList);
                log.AddLog("GetComments");
               
                string regStr = @"_.*$";
                Regex regx = new Regex(regStr, RegexOptions.IgnoreCase | RegexOptions.RightToLeft);

                foreach (var c in cl)
                {
                    IEnumerable<PHSPhotoInfoEntity> plList = pl.Where(p => p.BusinessID == c.Comment.ID);
                    c.PhotoIDs = plList.Select(p => p.PHSID).ToList();
                    c.BigCommentPics = plList.Select(p => p.PhotoUrl[PhotoSizeType.appview]).ToList();
                    c.CommentPics = plList.Select(p => regx.Replace(p.PhotoUrl[PhotoSizeType.small], "_290x290s")).ToList();
                }


                log.Finish();
                return cl;
            }
            catch
            {
                return new List<CommentInfoEntity>();
            }

        }

        public static CommentInfoEntity GetOneCommentNoPhoto(int ID)
        {
            return commentService.GetOneCommentInfo(ID);
        }

        public static UserCanWriteCommentResult GetUserCanWriteCommentOrderID(int hotelID, long userID)
        {
            UserCanWriteCommentResult r = new UserCanWriteCommentResult();
            if (userID == 0)
            {
                r.canWrite = false;
                r.orderID = 0;
                r.msg = "请登录!";
            }
            else
            {
                if (Configs.CanWriteCommentUserIDs.Contains(userID))
                {
                    r.canWrite = true;
                    r.orderID = 0;
                    r.msg = "用户为可直接写点评用户。";
                }
                else
                {
                    //区分品鉴酒店 非品鉴酒店 写点评的要求
                    //品鉴酒店可以没有订单
                    //非品鉴酒店必须有订单
                    //品鉴师点评
                    DateTime dtNow = DateTime.Now;
                    List<InspectorRefHotel> refHotelList = HotelAdapter.HotelService.GetInspectorRefHotelList(0, userID).FindAll(i => i.State == 3 && i.HotelID == hotelID && i.CommentID == 0).ToList();
                    DateTime dt2 = DateTime.MaxValue;
                    dt2 = refHotelList != null && refHotelList.Count != 0 ? refHotelList.Min(i => i.CheckInDate) : dt2;//未点评的品鉴酒店 最近的checkin时间
                    if (refHotelList != null && refHotelList.Count != 0 && dt2 <= dtNow)
                    {
                        r.canWrite = true;
                        r.orderID = 0;
                        r.msg = "您可以点评该酒店,提交品鉴报告。";
                    }
                    //int commentedCount = commentedRefHotels.Count;
                    //int unCommentedCount = unCommentedRefHotels.Count;
                    //if (unCommentedCount != 0 && commentedCount == 0)
                    //{
                    //    r.canWrite = true;
                    //    r.orderID = 0;
                    //    r.msg = "品鉴酒店写点评";
                    //}
                    //else if (unCommentedCount != 0 && commentedCount != 0)
                    //{
                    //    InspectorRefHotel lastCommentedRefHotel = commentedRefHotels[0];
                    //    DateTime lastCheckIn = lastCommentedRefHotel.UpdateTime;
                    //    if ((dtNow - lastCheckIn).TotalDays <= 90)
                    //    {
                    //        r.canWrite = false;
                    //        r.orderID = 0;
                    //        r.msg = "抱歉，您已经在三个月之内提交过这家酒店的品鉴报告了。";
                    //    }
                    //    else
                    //    {
                    //        r.canWrite = true;
                    //        r.orderID = 0;
                    //        r.msg = "品鉴酒店写点评";
                    //    }
                    //}
                    else
                    {
                        List<UserHotelCommentsEntity> list = commentService.GetUserHotelComments(hotelID, userID);//用户订单相关的点评（已写未写）
                        List<UserHotelCommentsEntity> list2 = list.FindAll(i => i.CommentID == 0);//未写点评
                        DateTime dt = DateTime.MaxValue;
                        dt = list2 != null && list2.Count != 0 ? list2.Max(i => i.CheckIn) : dt;

                        if (list.Count == 0)
                        {
                            r.canWrite = false;
                            r.orderID = 0;
                            r.msg = "抱歉，您还没有入住过该酒店，不能点评!";
                        }
                        else if (list2.Count == 0)
                        {
                            r.canWrite = false;
                            r.orderID = 0;
                            r.msg = "您已点评过该酒店了，谢谢!";
                        }
                        //else if (list.FindAll(i => i.CommentID != 0).Count() > 0)
                        //{
                        //    r.canWrite = false;
                        //    r.orderID = 0;
                        //    r.msg = "您已点评过该酒店了，谢谢!";
                        //}
                        else if (!list2.Exists(i => i.CheckIn <= dtNow))
                        {
                            r.canWrite = false;
                            r.orderID = 0;
                            r.msg = "客官别急，入住之后再来写点评吧。";
                        }
                        //else if ((dtNow - dt).TotalDays > 90)
                        //{
                        //    r.canWrite = false;
                        //    r.orderID = 0;
                        //    r.msg = "抱歉，您入住时间已超过三个月，无法点评这家酒店了。";
                        //}
                        else
                        {
                            r.canWrite = true;
                            r.orderID = list.Where(l => l.CommentID == 0).First().OrderID;
                            r.msg = "您可以点评该酒店。";
                        }
                    }
                }
            }
            return r;
        }

        /// <summary>
        /// 已经删除或重复删除的开放点评 不受3个月时间限制
        /// </summary>
        /// <param name="hotelID"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        public static CanWriteCommentResult GetUserCanWriteComment(int hotelID, long userID)
        {
            CanWriteCommentResult r = new CanWriteCommentResult();
            if (userID == 0)
            {
                r.canWrite = false;
                r.orderID = 0;
                r.Message = "请登录!";
            }
            else
            {
                if (Configs.CanWriteCommentUserIDs.Contains(userID))
                {
                    r.canWrite = true;
                    r.orderID = 0;
                    r.Message = "用户为可直接写点评用户。";
                }
                else
                {
                    //是否有订单 订单处于可点评状态
                    List<UserHotelCommentsEntity> list = commentService.GetUserHotelComments(hotelID, userID);//订单列表(包括已经点评过的和待点评的订单)
                    List<UserHotelCommentsEntity> list2 = list.FindAll(i => i.CommentID == 0);//未点评的订单列表

                    if (list2.Count == 0)
                    {
                        //开放点评 存在品鉴订单
                        List<InspectorRefHotel> refHotelList = HotelAdapter.HotelService.GetInspectorRefHotelList(0, userID).FindAll(i => i.State == 3 && i.HotelID == hotelID && i.CommentID == 0).ToList();
                        if (refHotelList != null && refHotelList.Count != 0)
                        {
                            r.canWrite = true;
                            r.orderID = 0;
                            r.Message = "您可以点评该酒店,提交品鉴报告。";
                        }
                        else
                        {
                            //无订单的点评只能点评一次
                            DateTime date3MonthsAgo = DateTime.Now.AddDays(-90).Date;
                            List<UserCommentListItemEntity> clWrite = commentService.GetUserCommentList20(userID);//已写好的点评（包括订单和开放点评）
                            if (clWrite == null || clWrite.Count == 0 || !clWrite.Exists(_ => _.OrderID == 0 && _.HotelID == hotelID && _.CommentDateTime.Date > date3MonthsAgo && _.State != 6))
                            {
                                r.canWrite = true;
                                r.orderID = 0;
                                r.Message = "您可以写该点评";//&& _.State != 2 && _.State != 3  删除和重复状态的点评暂时算在不可写 主动删除的可认为可继续写
                            }
                            else
                            {
                                r.canWrite = false;
                                r.orderID = 0;
                                r.Message = "同一家酒店的开放点评三个月内只能写一次";
                            }
                        }
                    }
                    else
                    {
                        //订单点评直接可以写 但积分三个月内只积分一次
                        r.canWrite = true;
                        r.orderID = list2.First().OrderID;
                        r.Message = "您可以点评该酒店。";
                    }
                }
            }
            r.msg = r.Message;//兼容android4.2版本 bug 2016-02-16
            return r;
        }

        public static SubmitCommentResultEntity SubmitComment(SubmitCommentEntity c)
        {
            SubmitCommentResultEntity r = new SubmitCommentResultEntity();
            r.Success = 0;
            r.Message = @"<div style=""margin-top:5em""><div style=""height:1.5em;line-height:1.5em;font-size: 1.15em; color: #333;text-align:center;"">点评提交成功</div><div style=""height: 1.4em; line-height: 1.4em; font-size: 0.9em; color: #888; text-align: center;"">待审核，1-3个工作日后发表</div></div>";
            r.DetailUrl = "";
            r.NextUrl = "";

            try
            {
                if (c.UserID == 0)
                {
                    r.Success = 1;
                    r.Message = "用户信息为空，请先登录！";
                    return r;
                }
                else
                {
                    string nextUrl = "";
                    if (c.OrderID != 0)
                    {
                        //判断该订单是否提交过点评 已提交的不能重复提交点评内容
                        List<UserHotelCommentsEntity> list = commentService.GetUserHotelComments(c.HotelID, c.UserID);
                        if (list != null && list.Count > 0 && list.Exists(_ => _.OrderID == c.OrderID && _.CommentID != 0))
                        {
                            r.CommentID = list.First(_ => _.OrderID == c.OrderID && _.CommentID != 0).CommentID;
                            r.Success = 3;
                            r.Message = "点评已提交成功，无需重复提交！";
                            r.ShareCoupon = 2000;
                            r.DetailUrl = DescriptionHelper.GenAppJumpUrl(HJDAPI.Common.Helpers.Enums.JumpUrlName.commentdetail, r.CommentID);
                            nextUrl = string.Format("{0}/Comment/CommentSubCompleted?commentid={1}&userid={2}", Configs.WWWURL, r.CommentID, c.UserID);
                            r.NextUrl = "whotelapp://www.zmjiudian.com/gotopage?url=" + System.Web.HttpUtility.UrlEncode(nextUrl);
                            return r;
                        }
                    }
                    else
                    {
                        List<UserHotelCommentsEntity> list = commentService.GetUserHotelComments(c.HotelID, c.UserID);//订单列表(包括已经点评过的和待点评的订单)
                        List<UserHotelCommentsEntity> list2 = list.FindAll(i => i.CommentID == 0);//未点评的订单列表
                        if (list2.Count != 0)
                        {
                            c.OrderID = list2.First().OrderID;
                            r.ShareCoupon = 2000;//单位为分
                        }
                    }

                    HJD.CommentService.Contract.CommentEntity ce = new HJD.CommentService.Contract.CommentEntity()
                    {
                        CreateTime = DateTime.Now,
                        HotelID = c.HotelID,
                        OrderID = c.OrderID,
                        Recommned = c.recommend,
                        Score = c.score,
                        UserID = c.UserID,
                        SourceType = (int)c.SourceID,
                        AppVer = c.appVer
                    };

                    ce.ID = AddComment(ce);//保存点评

                    AddCommentsTag(ce.ID, c.TagIDs);//保存标签块

                    AddCommentsAddInfo(ce.ID, c.addInfos);//保存补充文字信息

                    if (c.addScores != null && c.addScores.Count != 0)
                    {
                        AddCommentScore(c.addScores.Select(_ => new CommentScoreEntity()
                        {
                            CommentID = ce.ID,
                            ScoreType = _.ScoreType,
                            ScoreVal = _.Score
                        }));
                    }

                    //在Comments表中生成点评描述，以便后台审核
                    CommentAdapter.GenWHHotelcommentDescription(ce.ID);

                    //如果提交时有待点评的品鉴酒店  酒店id一致 则同时更新获得的品鉴酒店commentID
                    if (c.OrderID == 0)
                    {
                        DateTime dtNow = DateTime.Now;
                        List<InspectorRefHotel> refHotelList = HotelAdapter.HotelService.GetInspectorRefHotelList(0, c.UserID).FindAll(i => i.HotelID == c.HotelID && i.State == 3).ToList();
                        List<InspectorRefHotel> unCommentedRefHotels = refHotelList.FindAll(i => i.CommentID == 0 && i.CheckInDate <= dtNow).OrderByDescending(i => i.CheckInDate).ToList();//依赖申请品鉴酒店的入住日期

                        if (unCommentedRefHotels != null && unCommentedRefHotels.Count != 0)
                        {
                            int irhID = unCommentedRefHotels[0].ID;
                            HotelAdapter.HotelService.UpdateInspectorRefHotel4Comment(irhID, ce.ID);//更新品鉴酒店相关的点评
                        }
                    }

                    r.CommentID = ce.ID;
                    r.DetailUrl = DescriptionHelper.GenAppJumpUrl(HJDAPI.Common.Helpers.Enums.JumpUrlName.commentdetail, r.CommentID);
                    nextUrl = string.Format("{0}/Comment/CommentSubCompleted?commentid={1}&userid={2}", Configs.WWWURL, r.CommentID, c.UserID);
                    r.NextUrl = "whotelapp://www.zmjiudian.com/gotopage?url=" + System.Web.HttpUtility.UrlEncode(nextUrl);

                    if (c.photoInfos != null && c.photoInfos.Any())
                    {
                        foreach (var photo in c.photoInfos)
                        {
                            photo.AppVer = c.appVer;
                            photo.CommentID = ce.ID;
                            CommentAdapter.InsertCommentPhotoUpload(photo);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.WriteLog("SubmitCommnet ERR:" + e.Message + e.StackTrace);
                r.Message = e.Message;
                r.Success = 2;
                r.CommentID = 0;
                r.ShareCoupon = 0;
                r.DetailUrl = "";
            }
            return r;
        }

        /// <summary>
        /// 开放点评
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static SubmitCommentResultEntity SubmitComment4NewHotel(SubmitCommentEntity c)
        {
            SubmitCommentResultEntity r = new SubmitCommentResultEntity();
            r.Success = 0;
            r.Message = @"<div style=""margin-top:5em""><div style=""height:1.5em;line-height:1.5em;font-size: 1.15em; color: #333;text-align:center;"">点评提交成功</div><div style=""height: 1.4em; line-height: 1.4em; font-size: 0.9em; color: #888; text-align: center;"">待审核，1-3个工作日后发表</div></div>";
            r.DetailUrl = "";
            r.NextUrl = "";

            try
            {
                if (c.UserID == 0)
                {
                    r.Success = 1;
                    r.Message = "用户信息为空，请先登录！";
                    return r;
                }
                else
                {
                    HJD.CommentService.Contract.CommentEntity ce = new HJD.CommentService.Contract.CommentEntity()
                    {
                        CreateTime = DateTime.Now,
                        HotelID = c.HotelID,
                        OrderID = 0,
                        Recommned = c.recommend,
                        Score = c.score,
                        UserID = c.UserID,
                        SourceType = (int)c.SourceID,
                        AppVer = c.appVer
                    };

                    ce.ID = AddComment(ce);//保存点评

                    AddCommentsTag(ce.ID, c.TagIDs);

                    AddCommentsAddInfo(ce.ID, c.addInfos);

                    if (c.addScores != null && c.addScores.Count != 0)
                    {
                        AddCommentScore(c.addScores.Select(_ => new CommentScoreEntity()
                        {
                            CommentID = ce.ID,
                            ScoreType = _.ScoreType,
                            ScoreVal = _.Score
                        }));
                    }

                    //在Comments表中生成点评描述，以便后台审核
                    CommentAdapter.GenWHHotelcommentDescription(ce.ID);

                    int addHotelID = HotelAdapter.HotelService.InsertUserAddHotels(c.HotelName.Trim(), ce.UserID, ce.ID);//清除传入酒店名称的两端空格                    

                    r.CommentID = ce.ID;
                    r.ShareCoupon = 0;//单位为分 新酒店的点评不再享有分享后获得现金券

                    string nextUrl = string.Format("{0}/Comment/CommentSubCompleted?commentid={1}&userid={2}", Configs.WWWURL, r.CommentID, c.UserID);
                    r.NextUrl = "whotelapp://www.zmjiudian.com/gotopage?url=" + System.Web.HttpUtility.UrlEncode(nextUrl);

                    if (c.photoInfos != null && c.photoInfos.Any())
                    {
                        foreach (var photo in c.photoInfos)
                        {
                            photo.AppVer = c.appVer;
                            photo.CommentID = ce.ID;
                            CommentAdapter.InsertCommentPhotoUpload(photo);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.WriteLog("SubmitCommnet ERR:" + e.Message + e.StackTrace);
                r.Message = e.Message;
                r.Success = 2;
                r.CommentID = 0;
                r.ShareCoupon = 0;
            }
            return r;
        }

        public static CommentPhotoUploadResultEntity InsertCommentPhotoUpload(CommentPhotoInsertEntity photo)
        {
            int appID = photo.AppID == 0 ? (photo.SourceID == 0 ? 3 : (int) photo.SourceID) : photo.AppID;//1.iOS 2.android 3.web
            int typeID = 1;

            CommentPhotoUploadResultEntity r = new CommentPhotoUploadResultEntity();
            r.Success = 0;
            r.Message = "添加成功！";
            try
            {
                CommentInfoEntity c = CommentAdapter.GetOneCommentNoPhoto(photo.CommentID);
                if (c != null && c.Comment != null && c.Comment.State != 0)
                {
                    CommentAdapter.commentService.UpdateWHComments(photo.CommentID, 0, c.Comment.UserID);//判断点评状态  非待审核的点评全部更新成待审核状态
                }

                //增加点评的section内容
                if (!string.IsNullOrWhiteSpace(photo.AppVer) && photo.AppVer.CompareTo("4.2") >= 0)
                {
                    var toAddSection = new List<CommentSectionEntity>();
                    toAddSection.Add(new CommentSectionEntity()
                    {
                        Brief = string.IsNullOrWhiteSpace(photo.PicBrief) ? "" : photo.PicBrief,
                        CommentID = photo.CommentID,
                        SequenceNo = photo.SequenceNo,
                        PicSUrl = photo.PhotoSURL,
                        Batch = photo.IsAdditionalComment ? 2 : 1
                    });
                    CommentAdapter.AddCommentSection(toAddSection);
                }

                int phsid = PhotoAdapter.AddPHSUploadInfoWithYupooInfo(appID, typeID, photo.PhotoSize, photo.PhotoSURL, photo.PhotoSecret, photo.PhotoType, photo.PhotoWidth, photo.PhotoHeight);
                int type = (int)PHSPhotoBizType.周末酒店点评; //酒店点评ID

                if (c.Comment.HotelID == 0)
                {
                    List<CommentAddHotelEntity> addHotellist = HotelAdapter.HotelService.GetUserAddHotelByComment(c.Comment.ID);//获得用户添加酒店名称
                    if (addHotellist != null && addHotellist.Count != 0)
                    {
                        PhotoAdapter.InsertPhotoInfo(phsid, type, photo.TagBlockCategory == 0 || (photo.AppVer.CompareTo("4.2") >= 0 && photo.TagBlockCategory == 1) ? -1 : photo.TagBlockCategory, c.Comment.UserID, 0, addHotellist.First().HotelID, photo.CommentID, "");
                    }
                    else
                    {
                        PhotoAdapter.InsertPhotoInfo(phsid, type, photo.TagBlockCategory == 0 || (photo.AppVer.CompareTo("4.2") >= 0 && photo.TagBlockCategory == 1) ? -1 : photo.TagBlockCategory, c.Comment.UserID, 0, 0, photo.CommentID, "");
                    }
                }
                else
                {
                    var hotelList = HotelAdapter.GetHotelBasicInfoList(new List<int>() { c.Comment.HotelID });
                    HotelBasicInfo hotelInfo = hotelList != null && hotelList.Count() > 0 ? hotelList.First() : new HotelBasicInfo();
                    PhotoAdapter.InsertPhotoInfo(phsid, type, photo.TagBlockCategory == 0 || (photo.AppVer.CompareTo("4.2") >= 0 && photo.TagBlockCategory == 1) ? -1 : photo.TagBlockCategory, c.Comment.UserID, hotelInfo.DistrictID, hotelInfo.HotelID, photo.CommentID, "");
                }
                r.phsid = phsid;
            }
            catch (Exception e)
            {
                r.Success = 1;
                r.Message = e.Message + e.StackTrace;
                r.phsid = 0;
            }
            return r;
        }

        public static UserCommentListModel GetUserCommentList(long userID, int start, int count)
        {
            List<CommentItemEntity> cl = commentService.GetUserCommentList(userID);
            UserCommentListModel m = new UserCommentListModel();

            m.CommentList = cl.Where(c => c.CommentID > 0 && c.CommentState != 6).ToList();//完全跟订单有关的点评            

            List<int> hotelIDList = m.CommentList.Select(i => i.HotelID).Distinct().ToList();

            List<CommentItemEntity> pageCommentList = count == 0 ? m.CommentList : (from n
                     in m.CommentList.Skip<CommentItemEntity>(start).Take<CommentItemEntity>(count)
                                                                                    select n).ToList();

            m.CommentDteailList = new List<CommentInfoModel>();

            foreach (CommentItemEntity c in pageCommentList)
            {
                CommentInfoModel infoModel = GetOneComment(c.CommentID);

                infoModel.commentItem = c;
                infoModel.shareModel = DescriptionHelper.GetShareModel(infoModel);

                m.CommentDteailList.Add(infoModel);
            }

            DateTime dtNow = DateTime.Now;
            //1)点评时间不到的不能点评;
            m.NoCommentList = cl.Where(c => c.CommentID == 0 && c.Checkin <= dtNow).ToList();

            List<CommentItemEntity> pageUnCommentList = count == 0 ? m.NoCommentList : (from n
                in m.NoCommentList.Skip<CommentItemEntity>(start).Take<CommentItemEntity>(count)
                                                                                        select n).ToList();

            m.OrderList = OrderAdapter.GetPackageOrderInfos(pageUnCommentList.Select(o => o.OID).ToList());

            return m;
        }

        /// <summary>
        /// 未点评和已经点评的合并在一个列表 点评状态为5(不符合规则)的点评可以修改 增加图片和内容
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="start"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static CommentListModel GetUserCommentList20(long userID, int start, int count, string appVer = null)
        {
            List<UserCommentListItemEntity> resultList = new List<UserCommentListItemEntity>();

            List<CommentItemEntity> clNoWrite = commentService.GetUserCommentList(userID).FindAll(_ => _.CommentID == 0);//代写点评的订单
            List<UserCommentListItemEntity> clWrite = commentService.GetUserCommentList20(userID).FindAll(_ => _.State != 6);//已写好的点评(排除主动删除的)
            int curLength = start + count;
            int leaveCount = 0;
            //代写订单点评
            if (clNoWrite != null && clNoWrite.Count != 0)
            {
                int clNoWriteCount = clNoWrite.Count;
                if (count != 0)
                {
                    int needCount = clNoWriteCount >= curLength ? count : clNoWriteCount >= start ? clNoWrite.Count - start : 0;
                    if (needCount > 0)
                    {
                        resultList.AddRange(clNoWrite.OrderByDescending(_ => _.Checkin).Select(_ => new UserCommentListItemEntity()
                        {
                            HotelScore = _.HotelScore,
                            CommentScore = _.Score,
                            CheckIn = _.Checkin,
                            CommentDateTime = DateTime.MinValue,
                            CommentID = _.CommentID,
                            HotelID = _.HotelID,
                            HotelName = _.Hotelname,
                            HotelPics = new List<string>(),
                            OrderID = _.OrderID,
                            IsCanAddContent = false
                        }).Skip(start).Take(needCount));
                    }
                    leaveCount = count - needCount;
                }
                else
                {
                    resultList.AddRange(clNoWrite.OrderByDescending(_ => _.Checkin).Select(_ => new UserCommentListItemEntity()
                    {
                        HotelScore = _.HotelScore,
                        CommentScore = _.Score,
                        CheckIn = _.Checkin,
                        CommentDateTime = DateTime.MinValue,
                        CommentID = _.CommentID,
                        HotelID = _.HotelID,
                        HotelName = _.Hotelname,
                        HotelPics = new List<string>(),
                        OrderID = _.OrderID,
                        IsCanAddContent = false
                    }));
                }
            }

            //写好的点评（订单点评和开放点评）
            if (clWrite != null && clWrite.Count != 0)
            {
                int clWriteCount = clWrite.Count;
                int needCount = count != 0 && leaveCount > 0 ? leaveCount : 0;
                if (needCount > 0)
                {
                    int nextStart = clWriteCount >= start ? (count - needCount) : 0;
                    if (nextStart > 0)
                    {
                        clWrite = clWrite.Skip(nextStart).Take(needCount).ToList();
                    }
                    else
                    {
                        clWrite = new List<UserCommentListItemEntity>();
                    }
                }

                if (clWrite.Count > 0)
                {
                    var canWriteCommentIds = GenCanWriteMoreContentCommentList(clWrite.Select(_ => _.CommentID));
                    clWrite.ForEach((_) =>
                    {
                        _.IsCanAddContent = (!string.IsNullOrWhiteSpace(appVer) && appVer.StartsWith("4.1") || !string.IsNullOrWhiteSpace(_.AppVer) && _.AppVer.CompareTo("4.2") >= 0) && canWriteCommentIds.Contains(_.CommentID);//
                    });//判断是否追加过点评
                    resultList.AddRange(clWrite.OrderByDescending(_ => _.CommentID));//写好的点评降序排列
                }
            }

            if (resultList != null && resultList.Count != 0)
            {
                genHotelPicOrDetailUrlOfComment(resultList);
            }

            return new CommentListModel() { CommentList = resultList, IsShowAddHotel = true };
        }

        /// <summary>
        /// 网站 我的点评列表
        /// </summary>
        /// <returns></returns>
        public static CommentListModel20 GetUserCommentList30(long userID, int start, int count, string appVer = null)
        {
            CommentListModel20 result = new CommentListModel20()
            {
                DoneCommentList = new List<UserCommentListItemEntity>(),
                UnDoneCommentList = new List<UserCommentListItemEntity>()
            };

            List<CommentItemEntity> clNoWrite = commentService.GetUserCommentList(userID).FindAll(_ => _.CommentID == 0);//待写点评的订单
            List<UserCommentListItemEntity> clWrite = commentService.GetUserCommentList20(userID).FindAll(_ => _.State != 6);//已写好的点评(排除主动删除的点评)

            //待写订单点评
            if (clNoWrite != null && clNoWrite.Count != 0)
            {
                result.UnDoneCommentList.AddRange(clNoWrite.OrderByDescending(_ => _.Checkin).Select(_ => new UserCommentListItemEntity()
                {
                    HotelScore = _.HotelScore,
                    CommentScore = _.Score,
                    CheckIn = _.Checkin,
                    CommentDateTime = DateTime.MinValue,
                    CommentID = _.CommentID,
                    HotelID = _.HotelID,
                    HotelName = _.Hotelname,
                    HotelPics = new List<string>(),
                    OrderID = _.OrderID,
                    IsCanAddContent = false
                }));
            }

            //写好的点评（订单点评和开放点评）
            if (clWrite != null && clWrite.Count != 0)
            {
                var canWriteCommentIds = GenCanWriteMoreContentCommentList(clWrite.Select(_ => _.CommentID));
                clWrite.ForEach((_) =>
                {
                    _.IsCanAddContent = (!string.IsNullOrWhiteSpace(appVer) && appVer.StartsWith("4.1") || !string.IsNullOrWhiteSpace(_.AppVer) && _.AppVer.CompareTo("4.2") >= 0) && canWriteCommentIds.Contains(_.CommentID);//
                });//判断是否追加过点评
                result.DoneCommentList.AddRange(clWrite.OrderByDescending(_ => _.CommentID));//写好的点评降序排列
            }

            if (result.UnDoneCommentList != null && result.UnDoneCommentList.Count != 0)
            {
                genHotelPicOrDetailUrlOfComment(result.UnDoneCommentList);
            }

            if (result.DoneCommentList != null && result.DoneCommentList.Count != 0)
            {
                genHotelPicOrDetailUrlOfComment(result.DoneCommentList);
            }

            return result;
        }

        private static void genHotelPicOrDetailUrlOfComment(List<UserCommentListItemEntity> userCommentList)
        {
            if (userCommentList == null || userCommentList.Count == 0)
            {
                return;
            }

            //批量获取点评涉及酒店照片Url集合
            var hotelIds = userCommentList.Select(_ => _.HotelID).Distinct();
            var hotelPicsList = HotelAdapter.GetManyHotelPhotos(hotelIds);

            var finalHasPicHotel = new Dictionary<int, List<HJD.HotelServices.Contracts.HotelPhotoEntity>>();
            foreach (var item in hotelPicsList)
            {
                HJD.HotelServices.Contracts.HotelPhotoEntity photo = item.HPList.FirstOrDefault();
                if (photo != null && photo.HotelID > 0)
                {
                    finalHasPicHotel.Add(photo.HotelID, item.HPList);
                }
            }

            //绑定照片
            userCommentList.ForEach((_) =>
            {
                if (finalHasPicHotel.ContainsKey(_.HotelID))
                {
                    var photos = finalHasPicHotel[_.HotelID];
                    var coverPic = photos.FirstOrDefault(j => j.IsCover == true);
                    if (coverPic != null)
                    {
                        _.HotelPics = new List<string>() { PhotoAdapter.GenHotelPicUrl(coverPic.SURL, HJDAPI.Common.Helpers.Enums.AppPhotoSize.applist) };
                    }
                    else
                    {
                        _.HotelPics = new List<string>() { PhotoAdapter.GenHotelPicUrl(photos[0].SURL, HJDAPI.Common.Helpers.Enums.AppPhotoSize.applist) };
                    }
                }
                else
                {
                    _.HotelPics = new List<string>();
                }
                if (_.CommentID > 0)
                {
                    _.DetailUrl = DescriptionHelper.GenAppJumpUrl(HJDAPI.Common.Helpers.Enums.JumpUrlName.commentdetail, _.CommentID);
                }
                else
                {
                    _.DetailUrl = "";
                }
            });
        }

        public static int AddComment(CommentEntity c)
        {
            return commentService.AddComments(c);
        }

        public static bool AddCommentsTag(int CommentID, List<int> tags)
        {
            CommentTagEntity c = new CommentTagEntity();
            c.CommentID = CommentID;
            foreach (int i in tags.Distinct())
            {
                c.TagID = i;
                commentService.AddCommentsTag(c);
            }
            return true;
        }

        public static bool AddCommentsAddInfo(int CommentID, List<HJDAPI.Models.AddationalInfo> infos)
        {
            CommentAddInfoEntity c = new CommentAddInfoEntity();
            c.CommentID = CommentID;

            foreach (HJDAPI.Models.AddationalInfo info in infos)
            {
                c.CategoryID = info.CategoryID;
                c.Content = info.AddationalComment;
                commentService.AddCommentsAddInfo(c);
            }

            return true;
        }

        public static CommentDefaultInfoModel GetCommentDefaultInfo(int hotelid)
        {
            CommentDefaultInfoModel m = GetCommentDefaultInfo();

            List<CommentTagDefineEntity> tl = commentService.GetOneHotelCommentTags(hotelid);

            foreach (var b in m.BlockInfo)
            {
                foreach (var tb in b.TagBlockList)
                {
                    tb.CommentTagList.AddRange(tl.Where(o => o.Type == tb.CategoryID).Select(t => new CommentTag { TagID = t.TagID, Tag = t.Name }));

                    if (tb.CommentTagList.Count == 0)
                    {
                        if (DicDefaultTagBlock.ContainsKey(tb.CategoryID))
                        {
                            tb.CommentTagList.AddRange(DicDefaultTagBlock[tb.CategoryID]);
                        }
                        else // 如果没有房间数据，那么去掉
                        {

                        }
                    }

                    //处理房型信息
                    if (tb.CategoryID == 2)
                    {
                        //if(tb.CommentTagList.Count > 0)
                        //{
                        //    tb.CommentTagList.Add(GetOtherRoomTag());
                        //}
                        tb.CommentTagList.Add(GetOtherRoomTag());//为了公开点评  如果没有找到任何房型 则将房型信息为自定义
                    }
                }
            }
            return m;
        }

        /// <summary>
        /// -100 小文本框填写的房型标签
        /// </summary>
        /// <returns></returns>
        private static CommentTag GetOtherRoomTag()
        {
            return new CommentTag { Tag = "+", TagID = -100, addInfo = new AddInfo { TypeID = -100, Tips = "请填写您入住的房型：" } };
        }

        /// <summary>
        /// -200 小文本框填写的主题标签
        /// </summary>
        /// <returns></returns>
        private static CommentTag GetOtherInterestTag()
        {
            return new CommentTag { Tag = "+", TagID = -200, addInfo = new AddInfo { TypeID = -200, Tips = "请填写您觉得更好的特色标签：" } };
        }

        /// <summary>
        /// -300 小文本框填写的出游类型
        /// </summary>
        /// <returns></returns>
        private static CommentTag GetOtherTripTypeTag()
        {
            return new CommentTag { Tag = "+", TagID = -300, addInfo = new AddInfo { TypeID = -300, Tips = "请填写您此次出游的类型：" } };
        }

        /// <summary>
        /// -400 小文本框填写的酒店位置
        /// </summary>
        /// <returns></returns>
        private static CommentTag GetOtherHotelLocationTag()
        {
            return new CommentTag { Tag = "+", TagID = -400, addInfo = new AddInfo { TypeID = -400, Tips = "请填写酒店位置：" } };
        }

        public static CommentDefaultInfoModel GetCommentDefaultInfo()
        {

            CommentDefaultInfoModel m = new CommentDefaultInfoModel();

            m.BlockInfo = new List<CommentBlockInfo>();

            CommentBlockInfo block = new CommentBlockInfo();
            CommentTagBlock c = new CommentTagBlock();

            /////////////////////////////////////

            block = new CommentBlockInfo();
            block.BlockCategory = 3;
            block.BlockCategoryName = "Room";
            block.additionTips = "说说你对住房的感受吧，会对大家都有帮助哦";//藏式风格很有特色

            c = new CommentTagBlock();
            c.CategoryName = "我住的房型是";
            c.CategoryID = 2;
            c.CommentTagList = new List<CommentTag>();
            c.MaxTags = 1;
            c.MinTags = 1;

            block.TagBlockList.Add(c);

            c = new CommentTagBlock();
            c.CategoryName = "对“房间”的评价";//对房间设施的评价
            c.CategoryID = 3;
            c.CommentTagList = new List<CommentTag>();
            c.MaxTags = 1000;
            c.MinTags = 1;
            block.TagBlockList.Add(c);

            m.BlockInfo.Add(block);

            /////////////////////////////////////

            block = new CommentBlockInfo();
            block.BlockCategory = 4;
            block.BlockCategoryName = "Interest";
            block.additionTips = "说说你对玩点的评价吧，会对大家都有帮助哦";//儿童乐园有点小，设施也有点旧了。

            c = new CommentTagBlock();
            c.CategoryName = "对“玩点”的评价";//有那些好玩的
            c.CategoryID = 4;
            c.CommentTagList = new List<CommentTag>();
            block.TagBlockList.Add(c);
            c.MaxTags = 1000;
            c.MinTags = 1;

            m.BlockInfo.Add(block);

            /////////////////////

            block = new CommentBlockInfo();
            block.BlockCategory = 5;
            block.BlockCategoryName = "Food";
            block.additionTips = "说说你对美食的评价吧，会对大家都有帮助哦";//儿童乐园有点小，设施也有点旧了。
            //block.TagBlockList = new List<CommentTagBlock>();
            c = new CommentTagBlock();
            c.CategoryName = "对“美食”的评价";//有哪些好吃的
            c.CategoryID = 5;
            c.CommentTagList = new List<CommentTag>();
            c.MaxTags = 1000;
            c.MinTags = 1;

            block.TagBlockList.Add(c);
            m.BlockInfo.Add(block);

            ///////////////////

            block = new CommentBlockInfo();
            block.BlockCategory = 6;
            block.BlockCategoryName = "Service";
            block.additionTips = "说说你对酒店服务的感受吧，会对大家都有帮助哦";//儿童乐园有点小，设施也有点旧了。
            //block.TagBlockList = new List<CommentTagBlock>();
            c = new CommentTagBlock();
            c.CategoryName = "对“服务”的评价";
            c.CategoryID = 6;
            c.CommentTagList = new List<CommentTag>();
            c.MaxTags = 1000;
            c.MinTags = 1;

            block.TagBlockList.Add(c);
            m.BlockInfo.Add(block);

            ///////////////////

            block = new CommentBlockInfo();
            block.BlockCategory = 1;
            block.BlockCategoryName = "OverAll";
            block.additionTips = "说说你赞美或吐槽的理由吧，会对大家都有帮助哦";//藏式风格很有特色
            //block.TagBlockList = new List<CommentTagBlock>();

            c = new CommentTagBlock();
            c.CategoryName = "酒店的主要特点是";//对酒店的总体印象是
            c.CategoryID = 1;
            c.CommentTagList = new List<CommentTag>();
            c.MaxTags = 1000;
            c.MinTags = 1;

            block.TagBlockList.Add(c);
            m.BlockInfo.Add(block);

            ///////////////////

            block = new CommentBlockInfo();
            block.BlockCategory = 7;
            block.BlockCategoryName = "Problem";
            block.additionTips = "说说酒店不足的方面吧，会对大家都有帮助哦";//儿童乐园有点小，设施也有点旧了。
            //block.TagBlockList = new List<CommentTagBlock>();
            c = new CommentTagBlock();
            c.CategoryName = "酒店的不足是";
            c.CategoryID = 7;
            c.CommentTagList = new List<CommentTag>();
            c.MaxTags = 1000;
            c.MinTags = 0;

            block.TagBlockList.Add(c);
            m.BlockInfo.Add(block);

            ///////////////////


            return m;
        }

        private static Dictionary<int, List<CommentTag>> _DicDefaultTagBlock;

        private static Dictionary<int, List<CommentTag>> DicDefaultTagBlock
        {
            get
            {
                if (_DicDefaultTagBlock == null)
                {
                    _DicDefaultTagBlock = new Dictionary<int, List<CommentTag>>();

                    List<CommentTagDefineEntity> tl = commentService.GetDefaultHotelCommentTags();
                    foreach (var t in tl)
                    {
                        if (!_DicDefaultTagBlock.ContainsKey(t.Type))
                        {
                            _DicDefaultTagBlock[t.Type] = new List<CommentTag>();
                        }
                        _DicDefaultTagBlock[t.Type].Add(new CommentTag() { Tag = t.Name, TagID = t.TagID });
                    }
                }
                return _DicDefaultTagBlock;
            }
        }

        internal static string GenWHHotelCommentDes(CommentInfoEntity c)
        {
            return commentService.GenWHHotelCommentDes(c, "commentdetail");
        }

        internal static CommentInfoEntity GenWHHotelCommentDescription(int ID)
        {
            return commentService.GenWHHotelcommentDescription(ID);
        }

        #region 写点评调整
        private static CommentDefaultInfoModel GenCommentDefaultInfoEx()
        {
            CommentDefaultInfoModel m = new CommentDefaultInfoModel();

            m.BlockInfo = new List<CommentBlockInfo>();

            CommentBlockInfo block = new CommentBlockInfo();
            CommentTagBlock c = new CommentTagBlock();

            /////////////////////////////////////

            block = new CommentBlockInfo();
            block.BlockCategory = 3;
            block.BlockCategoryName = "roomtype&outting";
            block.additionTips = "";//
            //block.TagBlockList = new List<CommentTagBlock>();
            c = new CommentTagBlock();
            c.CategoryName = "我住的房型是";
            c.CategoryTitle = "房型";
            c.CategoryID = 2;
            c.CommentTagList = new List<CommentTag>();
            c.MaxTags = 1;
            c.MinTags = 1;
            //c.additionTips = "填写其他房型";
            //ToDo 我的房型 增加其他
            block.TagBlockList.Add(c);

            c = new CommentTagBlock();
            c.CategoryName = "出游类型";
            c.CategoryID = 8;
            c.CategoryTitle = "出游";
            c.CommentTagList = new List<CommentTag>();
            c.MaxTags = 1;
            c.MinTags = 1;
            block.TagBlockList.Add(c);
            m.BlockInfo.Add(block);


            ///////////////////
            block = new CommentBlockInfo();
            block.BlockCategory = 1;
            block.BlockCategoryName = "OverAll";
            //block.TagBlockList = new List<CommentTagBlock>();

            c = new CommentTagBlock();
            c.CategoryName = "酒店的优点";//对酒店的总体印象是
            c.CategoryID = 1;
            c.CategoryTitle = "优点";
            c.additionTips = "说说酒店不错的方面吧，对大家都有帮助哦";//藏式风格很有特色
            c.CommentTagList = new List<CommentTag>();
            c.MaxTags = 1000;
            c.MinTags = 1;
            c.PicCount = 0;
            block.TagBlockList.Add(c);

            c = new CommentTagBlock();
            c.CategoryName = "酒店的不足";
            c.CategoryID = 7;
            c.CategoryTitle = "不足";
            c.CommentTagList = new List<CommentTag>();
            c.additionTips = "说说酒店不足的方面吧，对大家都有帮助哦";//儿童乐园有点小，设施也有点旧了。
            c.MaxTags = 1000;
            c.MinTags = 0;
            c.PicCount = 0;
            block.TagBlockList.Add(c);

            m.BlockInfo.Add(block);

            /////////////////////////////////////
            block = new CommentBlockInfo();
            block.BlockCategory = 8;
            block.BlockCategoryName = "Room";
            //block.additionTips = "说说你对住房的感受吧，会对大家都有帮助哦";//藏式风格很有特色
            //block.TagBlockList = new List<CommentTagBlock>();

            c = new CommentTagBlock();
            c.CategoryName = "房间的评价";//对房间设施的评价
            c.CategoryID = 3;
            c.CategoryTitle = "房间";
            c.CommentTagList = new List<CommentTag>();
            c.MaxTags = 1000;
            c.MinTags = 1;
            c.additionTips = "说说你对房间的感受吧，对大家都有帮助哦";
            c.PicCount = 8;
            block.TagBlockList.Add(c);
            m.BlockInfo.Add(block);


            /////////////////////

            block = new CommentBlockInfo();
            block.BlockCategory = 5;
            block.BlockCategoryName = "Food";
            //block.additionTips = "说说你对美食的评价吧，会对大家都有帮助哦";//儿童乐园有点小，设施也有点旧了。
            //block.TagBlockList = new List<CommentTagBlock>();
            c = new CommentTagBlock();
            c.CategoryName = "美食的评价";//有哪些好吃的
            c.CategoryID = 5;
            c.CategoryTitle = "美食";
            c.CommentTagList = new List<CommentTag>();
            c.MaxTags = 1000;
            c.MinTags = 1;
            c.additionTips = "说说你对美食的评价吧，对大家都有帮助哦";
            c.PicCount = 8;
            block.TagBlockList.Add(c);
            m.BlockInfo.Add(block);

            /////////////////////////////////////

            block = new CommentBlockInfo();
            block.BlockCategory = 4;
            block.BlockCategoryName = "Interest";
            //block.additionTips = "说说你对玩点的评价吧，会对大家都有帮助哦";//儿童乐园有点小，设施也有点旧了。
            //block.TagBlockList = new List<CommentTagBlock>();
            c = new CommentTagBlock();
            c.CategoryName = "玩点的评价";//有那些好玩的
            c.CategoryID = 4;
            c.CategoryTitle = "玩点";
            c.CommentTagList = new List<CommentTag>();
            c.MaxTags = 1000;
            c.MinTags = 1;
            c.additionTips = "说说你对玩点的评价吧，对大家都有帮助哦";
            c.PicCount = 8;
            block.TagBlockList.Add(c);
            m.BlockInfo.Add(block);

            ///////////////////

            block = new CommentBlockInfo();
            block.BlockCategory = 6;
            block.BlockCategoryName = "Service";
            //block.additionTips = "说说你对酒店服务的感受吧，会对大家都有帮助哦";//儿童乐园有点小，设施也有点旧了。
            //block.TagBlockList = new List<CommentTagBlock>();
            c = new CommentTagBlock();
            c.CategoryName = "服务的评价";
            c.CategoryTitle = "服务";
            c.CategoryID = 6;
            c.CommentTagList = new List<CommentTag>();
            c.MaxTags = 1000;
            c.MinTags = 1;
            c.PicCount = 0;
            c.additionTips = "说说你对酒店服务的感受吧，对大家都有帮助哦";
            block.TagBlockList.Add(c);
            m.BlockInfo.Add(block);

            /////////////////////////////////////////////////

            return m;
        }

        public static CommentDefaultInfoModel GetCommentDefaultInfoEx(int hotelid)
        {
            CommentDefaultInfoModel m = GenCommentDefaultInfoEx();
            List<CommentTagDefineEntity> tl = commentService.GetOneHotelRoomTypeTags(hotelid);
            foreach (var b in m.BlockInfo)
            {
                if (b.BlockCategory != 3)
                {
                    continue;
                }
                foreach (var tb in b.TagBlockList)
                {
                    //只有房型需要接口获得  出游类型是默认生成的几个选项
                    if (tb.CategoryID == 8)
                    {
                        if (DicDefaultTagBlock.ContainsKey(tb.CategoryID))
                        {
                            tb.CommentTagList.AddRange(DicDefaultTagBlock[tb.CategoryID]);//插入缺省的出游类型
                        }
                        continue;
                    }
                    else if (tb.CategoryID != 2)
                    {
                        continue;
                    }

                    if (tl != null && tl.Count > 0)
                    {
                        tb.CommentTagList.AddRange(tl.Where(o => o.Type == tb.CategoryID).Select(t => new CommentTag { TagID = t.TagID, Tag = t.Name }));
                    }

                    if (tb.CommentTagList.Count == 0)
                    {
                        if (DicDefaultTagBlock.ContainsKey(tb.CategoryID))
                        {
                            tb.CommentTagList.AddRange(DicDefaultTagBlock[tb.CategoryID]);
                        }
                    }

                    //处理房型信息
                    if (tb.CategoryID == 2)
                    {
                        if (tb.CommentTagList.Count > 0)
                        {
                            tb.CommentTagList.Add(GetOtherRoomTag());
                        }
                    }
                }
            }

            return m;
        }

        public static CommentInfoModel2 GetOneCommentEx(int ID, bool needNickNameAndShareModel = true, long userID = 0)
        {
            CommentInfoModel2 m2 = new CommentInfoModel2();//CommentInfoModel2
            m2.commentInfoModel = new CommentInfoModel();
            m2.commentInfoModel.commentInfo = commentService.GetOneCommentInfo(ID, true, true);
            if (needNickNameAndShareModel)
            {
                m2.commentInfoModel.commentHTML = GenWHHotelCommentDes(m2.commentInfoModel.commentInfo);//App用
            }
            else
            {
                m2.commentInfoModel.commentHTML = "";
            }
            //变化之一就是把照片按点评内容区分  变化之二多出了一个出游类型 变化之三图片排列
            m2.commentInfoModel.photoInfo = new List<string>();
            if (needNickNameAndShareModel)
            {
                List<PHSPhotoInfoEntity> photoList = PhotoAdapter.GetWHHotelCommentPhotoByWritings(new List<int> { ID });
                m2.categoryPhotoList = getCategoryPhotosList(photoList);//点评内容分类照片列表
            }
            else
            {
                m2.categoryPhotoList = new List<CategoryPhotosEntity>();
            }

            int hotelID = m2.commentInfoModel.commentInfo.Comment.HotelID;
            if (hotelID > 0)
            {
                HotelItem hi = ResourceAdapter.GetHotel(hotelID, 0);////new HotelAdapter().GetHotelItem3FromCache(hotelID, 0);//new HotelAdapter().Get3(hotelID, CheckIn, CheckOut, "www", 0);//interestID暂时get30没有用
                m2.commentInfoModel.hotelPic = hi.Picture;//hi.Pics != null && hi.Pics.Count > 0 ? hi.Pics[0] : "";
                m2.commentInfoModel.hotelStar = hi.Star;
                m2.commentInfoModel.hotelScore = hi.Score;//增加酒店点评得分
                m2.commentInfoModel.hotelName = hi.Name;
            }
            else
            {
                m2.commentInfoModel.hotelPic = "";
                m2.commentInfoModel.hotelStar = 0;
                List<CommentAddHotelEntity> addHotellist = HotelAdapter.HotelService.GetUserAddHotelByComment(ID);//获得用户添加酒店名称
                if (addHotellist != null && addHotellist.Count != 0)
                {
                    m2.commentInfoModel.hotelName = addHotellist.First().HotelName;
                }
            }

            long userId = m2.commentInfoModel.commentInfo.Comment.UserID;
            if (needNickNameAndShareModel)
            {
                string nickName = AccountAdapter.GetNickName(userId);
                m2.commentInfoModel.nickName = string.IsNullOrWhiteSpace(nickName) ? "匿名" : nickName;
            }
            else
            {
                m2.commentInfoModel.nickName = "";
            }

            m2.commentInfoModel.shareModel = needNickNameAndShareModel ? DescriptionHelper.GetShareModel(m2.commentInfoModel, userID) : new CommentShareModel()
            {
                shareLink = "",
                Content = "",
                notHotelNameTitle = "",
                photoUrl = "",
                title = ""
            };
            return m2;
        }

        /// <summary>
        /// 返给App 点评详情信息
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="userID">查看点评内容的用户ID 与作者可能不一样</param>
        /// <returns></returns>
        public static CommentInfoModel3 GetOneComment50(int ID, long userID = 0)
        {
            //点评详情
            CommentInfoModel2 m2 = GetOneCommentEx(ID, true, userID);
            var commentInfoData = m2.commentInfoModel.commentInfo;
            CommentEntity ce = commentInfoData.Comment;
            int hotelId = ce.HotelID;//酒店ID
            long commentUser = ce.UserID;

            HJD.AccountServices.Entity.MemberProfileInfo memberInfo = AccountAdapter.GetCurrentUserInfo(commentUser);
            string avatarUrl = memberInfo != null ? memberInfo.AvatarUrl : DescriptionHelper.defaultAvatar;//默认头像地址;

            List<string> commentPics = new List<string>();
            List<string> bigCommentPics = new List<string>();
            Dictionary<string, double> dicPicHeightWidth = new Dictionary<string, double>();//照片对应的高宽比

            var commentSections = commentInfoData.CommentSections;

            if (m2.categoryPhotoList != null && m2.categoryPhotoList.Count != 0)
            {
                Regex getPicNameReg = new Regex(@"(?<=/).*?(?=_)", RegexOptions.Compiled);
                m2.categoryPhotoList.ForEach((_) =>
                {
                    commentPics.AddRange(_.PhotoUrls.Select(o => o.PicUrl));
                    bigCommentPics.AddRange(_.BigPhotoUrls.Select(o => o.PicUrl));

                    if (commentSections != null && commentInfoData.CommentSections.Count != 0)
                    {
                        foreach (var bigPhoto in _.BigPhotoUrls)
                        {
                            var photoShortName = getPicNameReg.Match(bigPhoto.PicUrl, 10).Value;//避开Http://的干扰
                            if (!dicPicHeightWidth.ContainsKey(photoShortName))
                            {
                                dicPicHeightWidth.Add(photoShortName, bigPhoto.Width * bigPhoto.Height == 0 ? 0.618 : Math.Round((bigPhoto.Height + 0.0001) / bigPhoto.Width, 3));
                            }
                        }
                    }
                });
            }

            string commentTitle = DescriptionHelper.CommentInfoConcat(9, commentInfoData);//4.0版本新加的一句话点评
            string commentDetail = DescriptionHelper.CommentInfoConcat(10, commentInfoData);//4.0版本新加的详细点评
            string additionalContent = DescriptionHelper.CommentInfoConcat(11, commentInfoData);//4.1版本新加的追加点评

            commentDetail = string.IsNullOrWhiteSpace(commentDetail) || commentDetail.Length <= 1 ? DescriptionHelper.GenCommentContent(commentInfoData) : commentDetail;

            string tripType = DescriptionHelper.CommentInfoConcat(8, commentInfoData);
            string roomType = commentInfoData.RoomInfo != null && commentInfoData.RoomInfo.Count > 0
                ? commentInfoData.RoomInfo[0].TagName : "";

            //酒店套餐详情
            PackagePriceInfo packageData = hotelId > 0 ? HotelAdapter.GetHotelPriceInfo(userID, hotelId, DateTime.MinValue, DateTime.MinValue) : null;

            string firstPackageBrief = packageData != null && packageData.PackageList != null && packageData.PackageList.Count > 0 ?
                packageData.PackageList.First().Brief : "";

            int minPrice = packageData != null ? (int)packageData.MinPrice : 0;
            int usefulCount = ce.UsefulCount;

            //if (userID != 0 && m2.commentInfoModel.shareModel != null)
            //{
            //    //替换分享点评链接的加密查询字符串 替换为新的
            //    string curShareLink = m2.commentInfoModel.shareModel.shareLink;

            //    Regex newRegex = new Regex("CID=.*$", RegexOptions.RightToLeft | RegexOptions.IgnoreCase);
            //    //追踪码生成 点评ID + 英文半角逗号 + 分享者UserID DES加密后传输 防止url篡改
            //    string desBase64Str = HJDAPI.Common.Security.DES.Encrypt(string.Format("{0},{1}", ID, userID));

            //    m2.commentInfoModel.shareModel.shareLink = newRegex.Replace(curShareLink, string.Format("CID={0}", desBase64Str));//更换为当前用户的分享追踪
            //}
            
            //判断能否追加点评
            List<int> clWrite = new List<int>();
            clWrite.Add(ce.ID);
            var canWriteCommentIds = GenCanWriteMoreContentCommentList(clWrite);
            bool IsCanAddContent = canWriteCommentIds.Contains(ce.ID);

            //判断当前是否点过帮助
            bool hasClickUseful = false;
            List<int> commentIds = commentService.GetUserClickUsefulComment(userID);
            hasClickUseful = commentIds != null && commentIds.Contains(ID) ? true : false;

            var reviewResult = GetCommentReviewsItemList(ID, commentUser, userID);

            return new CommentInfoModel3()
            {
                hotelID = hotelId,
                hotelName = m2.commentInfoModel.hotelName,
                hotelScore = m2.commentInfoModel.hotelScore,
                hotelPic = m2.commentInfoModel.hotelPic,
                commentPics = commentPics,
                bigCommentPics = bigCommentPics,
                commentScore = ce.Score,
                isRecommend = ce.Recommned,
                nickName = m2.commentInfoModel.nickName,
                minPrice = minPrice,
                packageBrief = firstPackageBrief,
                roomType = roomType,
                tripType = string.IsNullOrWhiteSpace(tripType) ? "" : tripType.Trim(".。".ToCharArray()),
                hasClickUseful = hasClickUseful,
                usefulCount = usefulCount,
                advantage = "",
                disAdvantage = "",
                commentContent = commentDetail,
                commentTitle = commentTitle,
                additionalContent = additionalContent,
                shareModel = m2.commentInfoModel.shareModel,
                avatar = avatarUrl,
                writeTime = ce.CreateTime.Date,
                commentID = ce.ID,
                authorUserID = ce.UserID,
                reviewItems = reviewResult,
                reviewCount = ce.ReviewsCount,
                visitCount = (int)ce.BrowsingCount2,//以运营浏览数量替代实际浏览数量
                isCanWriteReview = commentUser == userID ? false : true,
                followState = AccountAdapter.GetFollowFollowingRelState(commentUser, userID),
                IsCanAddContent = IsCanAddContent,
                clickUsefulList = GetCommentUsefulList(ID).Take(20).Select(_ => new ClickUsefulPeopleItem()
                {
                    UserID = _.UserID,
                    NickName = _.UserNickName,
                    AvatarUrl = string.IsNullOrWhiteSpace(_.UserAvatar) ? DescriptionHelper.defaultAvatar : PhotoAdapter.GenHotelPicUrl(_.UserAvatar, Enums.AppPhotoSize.jupiter)
                }).ToList(),
                addSections = commentSections.Select(_ => new AdditionalSection()
                {
                    Brief = _.Brief,
                    CommentID = _.CommentID,
                    SequenceNo = _.SequenceNo,
                    PicSUrl = _.PicSUrl,
                    PicUrl = PhotoAdapter.GenHotelPicUrl(_.PicSUrl, Enums.AppPhotoSize.appview),//appview不会更改原图比例
                    ID = _.ID,
                    Ratio = dicPicHeightWidth.ContainsKey(_.PicSUrl) ? dicPicHeightWidth[_.PicSUrl] : 0.618
                }).ToList()
            };
        }

        /// <summary>
        /// 生成点评的评论或回复列表
        /// 目前添加在点评详情GetOneComment50的接口内部
        /// 一旦某些点评回复过多 需要分页 并且接口要独立出来
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="commentUserID"></param>
        /// <param name="curUserID"></param>
        /// <returns></returns>
        public static List<CommentReviewsItem> GetCommentReviewsItemList(int ID, long commentUserID, long curUserID)
        {
            List<CommentReviewsEntity> commentReviews = GetCommentReviews(ID);

            if (commentReviews != null && commentReviews.Count() != 0)
            {
                var rootCommentReviewList = commentReviews.FindAll(_ => _.Parent == 0).ToList();
                var subCommentReviewList = commentReviews.FindAll(_ => _.Parent != 0).ToList();

                return rootCommentReviewList.OrderByDescending(_ => _.CreateTime).Select(_ => new CommentReviewsItem()
                {
                    AvatarUrl = String.IsNullOrWhiteSpace(_.SendUserAvatar) ? DescriptionHelper.defaultAvatar : PhotoAdapter.GenHotelPicUrl(_.SendUserAvatar, Enums.AppPhotoSize.jupiter),
                    UserID = _.SendUser,
                    NickName = commentUserID == _.SendUser ? _.SendUserNickName + "(作者)" : _.SendUserNickName,
                    Content = System.Web.HttpUtility.UrlDecode(_.ReviewContent, System.Text.Encoding.UTF8),//_.ReviewContent,
                    TimeDesc = genReviewTimeSpan(_.CreateTime),
                    CreateTime = _.CreateTime,
                    ReviewId = _.ID,
                    ParentReviewId = 0,
                    IsCanReply = commentUserID != _.SendUser && commentUserID == curUserID ? true : false,
                    subItems = subCommentReviewList.FindAll(j => j.Parent == _.ID).OrderBy(j => j.CreateTime).Select(j =>
                    new CommentReviewsItem()
                    {
                        AvatarUrl = String.IsNullOrWhiteSpace(j.SendUserAvatar) ? DescriptionHelper.defaultAvatar : PhotoAdapter.GenHotelPicUrl(j.SendUserAvatar, Enums.AppPhotoSize.jupiter),
                        UserID = j.SendUser,
                        NickName = commentUserID == j.SendUser ? "作者回复" : j.SendUserNickName,
                        Content = System.Web.HttpUtility.UrlDecode(j.ReviewContent, System.Text.Encoding.UTF8),//j.ReviewContent,
                        TimeDesc = genReviewTimeSpan(j.CreateTime),
                        CreateTime = j.CreateTime,
                        ReviewId = j.ID,
                        ParentReviewId = _.ID,
                        IsCanReply = commentUserID != j.SendUser && commentUserID == curUserID ? true : false,
                        subItems = new List<CommentReviewsItem>()
                    }).ToList()
                }).ToList();
            }

            return new List<CommentReviewsItem>();
        }

        /// <summary>
        /// 获得某个点评评论的数量（根级别回复数量）
        /// </summary>
        /// <param name="commentId"></param>
        /// <returns></returns>
        internal static int genCommentReviewsCount(int commentId)
        {
            List<CommentReviewsEntity> commentReviews = GetCommentReviews(commentId);
            return commentReviews != null ? commentReviews.FindAll(_ => _.Parent == 0).Count : 0;
        }

        internal static string genReviewTimeSpan(DateTime writeDate)
        {
            DateTime dtNow = DateTime.Now;
            TimeSpan timeSpan = dtNow - writeDate;
            string timeDesc = "刚刚";
            //先是年
            if (timeSpan.TotalDays >= 365)
            {
                int years = (int)Math.Floor(timeSpan.TotalDays / 365);
                timeDesc = years + "年前";
            }
            //再是月
            else if (timeSpan.TotalDays >= 30)
            {
                int months = (int)Math.Floor(timeSpan.TotalDays / 30);
                timeDesc = months + "个月前";
            }
            //后是天
            else if (timeSpan.TotalDays >= 1)
            {
                int days = (int)Math.Floor(timeSpan.TotalDays);
                timeDesc = days + "天前";
            }
            //继而小时
            else if (timeSpan.TotalHours >= 1)
            {
                int hours = (int)Math.Floor(timeSpan.TotalHours);
                timeDesc = hours + "小时前";
            }
            //后面分钟 刚刚(小于5分钟) 15分钟 30分钟 45分钟
            else if (timeSpan.TotalMinutes >= 45)
            {
                timeDesc = 45 + "分钟前";
            }
            else if (timeSpan.TotalMinutes >= 30)
            {
                timeDesc = 30 + "分钟前";
            }
            else if (timeSpan.TotalMinutes >= 15)
            {
                timeDesc = 15 + "分钟前";
            }
            return timeDesc;
        }

        public static CommentData GenUserCommentData(long userID)
        {
            //生成
            return new CommentData();
        }

        public static List<CategoryPhotosEntity> getCategoryPhotosList(List<PHSPhotoInfoEntity> photoList)
        {
            List<CategoryPhotosEntity> finalList = new List<CategoryPhotosEntity>();
            if (photoList != null && photoList.Count != 0)
            {
                int commentID = photoList.First().BusinessID;
                string regStr = @"_.*$";
                Regex regx = new Regex(regStr, RegexOptions.IgnoreCase | RegexOptions.RightToLeft | RegexOptions.Compiled);//正则表达式 多次使用需编译

                List<PHSPhotoInfoEntity> roomList = photoList.FindAll(i => i.SubType == 3);
                List<PHSPhotoInfoEntity> playList = photoList.FindAll(i => i.SubType == 4);
                List<PHSPhotoInfoEntity> foodList = photoList.FindAll(i => i.SubType == 5);
                List<PHSPhotoInfoEntity> otherList = photoList.FindAll(i => i.SubType == -1 || i.SubType == 0);//追加的照片跟没有分类的照片一起显示
                List<PHSPhotoInfoEntity> appendList = photoList.FindAll(i => i.SubType == 14);//追加的照片跟没有分类的照片一起显示

                if (roomList != null && roomList.Count != 0)
                {
                    finalList.Add(new CategoryPhotosEntity()
                    {
                        CommentID = commentID,
                        CategoryID = 3,
                        CategoryName = "房间",
                        BigPhotoUrls = roomList.Select(p => new PicShortInfo()
                        {
                            Height = p.Height,
                            Width = p.Width,
                            PicUrl = p.PhotoUrl[PhotoSizeType.appview]
                        }).ToList(),
                        PhotoUrls = roomList.Select(p => new PicShortInfo()
                        {
                            Height = p.Height,
                            Width = p.Width,
                            PicUrl = regx.Replace(p.PhotoUrl[PhotoSizeType.small], "_290x290s")
                        }).ToList()
                    });
                }
                if (foodList != null && foodList.Count != 0)
                {
                    finalList.Add(new CategoryPhotosEntity()
                    {
                        CommentID = commentID,
                        CategoryID = 5,
                        CategoryName = "美食",
                        BigPhotoUrls = foodList.Select(p => new PicShortInfo()
                        {
                            Height = p.Height,
                            Width = p.Width,
                            PicUrl = p.PhotoUrl[PhotoSizeType.appview]
                        }).ToList(),
                        PhotoUrls = foodList.Select(p => new PicShortInfo()
                        {
                            Height = p.Height,
                            Width = p.Width,
                            PicUrl = regx.Replace(p.PhotoUrl[PhotoSizeType.small], "_290x290s")
                        }).ToList()
                    });
                }
                if (playList != null && playList.Count != 0)
                {
                    finalList.Add(new CategoryPhotosEntity()
                    {
                        CommentID = commentID,
                        CategoryID = 4,
                        CategoryName = "玩点",
                        BigPhotoUrls = playList.Select(p => new PicShortInfo()
                        {
                            Height = p.Height,
                            Width = p.Width,
                            PicUrl = p.PhotoUrl[PhotoSizeType.appview]
                        }).ToList(),
                        PhotoUrls = playList.Select(p => new PicShortInfo()
                        {
                            Height = p.Height,
                            Width = p.Width,
                            PicUrl = regx.Replace(p.PhotoUrl[PhotoSizeType.small], "_290x290s")
                        }).ToList()
                    });
                }
                if (otherList != null && otherList.Count != 0)
                {
                    finalList.Add(new CategoryPhotosEntity()
                    {
                        CommentID = commentID,
                        CategoryID = -1,
                        CategoryName = "更多照片",
                        BigPhotoUrls = otherList.Select(p => new PicShortInfo()
                        {
                            Height = p.Height,
                            Width = p.Width,
                            PicUrl = p.PhotoUrl[PhotoSizeType.appview]
                        }).ToList(),
                        PhotoUrls = otherList.Select(p => new PicShortInfo()
                        {
                            Height = p.Height,
                            Width = p.Width,
                            PicUrl = regx.Replace(p.PhotoUrl[PhotoSizeType.small], "_290x290s")
                        }).ToList()
                    });
                }
                if (appendList != null && appendList.Count != 0)
                {
                    finalList.Add(new CategoryPhotosEntity()
                    {
                        CommentID = commentID,
                        CategoryID = 14,
                        CategoryName = "追加照片",
                        BigPhotoUrls = appendList.Select(p => new PicShortInfo()
                        {
                            Height = p.Height,
                            Width = p.Width,
                            PicUrl = p.PhotoUrl[PhotoSizeType.appview]
                        }).ToList(),
                        PhotoUrls = appendList.Select(p => new PicShortInfo()
                        {
                            Height = p.Height,
                            Width = p.Width,
                            PicUrl = regx.Replace(p.PhotoUrl[PhotoSizeType.small], "_290x290s")
                        }).ToList()
                    });
                }
            }
            return finalList;
        }

        /// <summary>
        /// 如果是三个月内非首次对该酒店的点评(包括品鉴报告)则提示
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="hotelID"></param>
        /// <returns></returns>
        public static AlertPointsRuleResult getAlertPointsRuleResult(long userID, int hotelID)
        {
            AlertPointsRuleResult ar = new AlertPointsRuleResult()
                {
                    IsNeed = false,
                    Message = ""
                };

            if (hotelID > 0)
            {


                DateTime dtNow = DateTime.Now;
                List<UserHotelCommentsEntity> list = commentService.GetUserHotelComments20(hotelID, userID, 0).FindAll(i => i.CommentID != 0 && (dtNow - i.CheckIn).TotalDays <= 90).ToList();
                List<InspectorRefHotel> commentedRefHotels = HotelAdapter.HotelService.GetInspectorRefHotelList(0, userID).FindAll(i => i.State == 3 && i.HotelID == hotelID && i.CommentID != 0 && (dtNow - i.CheckInDate).TotalDays <= 90).ToList();
                if (list != null && list.Count != 0 || commentedRefHotels != null && commentedRefHotels.Count != 0)
                {
                    ar = new AlertPointsRuleResult()
                    {
                        IsNeed = true,
                        Message = "您入住后三个月内已经点评过同一家酒店"
                    };
                }
                else
                {
                    ar = new AlertPointsRuleResult()
                    {
                        IsNeed = false,
                        Message = ""
                    };
                }

                //开放点评不需要提示积分规则。 没有积分
                CanWriteCommentResult r = GetUserCanWriteComment(hotelID, userID);
                ar.IsNeed = r.orderID > 0;
            }


            return ar;

        }

        /// <summary>
        /// APP 4.0版本调整 开放点评
        /// </summary>
        /// <returns></returns>
        private static CommentDefaultInfoModel GenCommentDefaultInfo30()
        {
            CommentDefaultInfoModel m = new CommentDefaultInfoModel();

            m.BlockInfo = new List<CommentBlockInfo>();

            CommentBlockInfo block = new CommentBlockInfo();
            CommentTagBlock c = new CommentTagBlock();

            /////////////////////////////////////
            block = new CommentBlockInfo();
            block.BlockCategory = 3;
            block.BlockCategoryName = "roomtype&outting";
            block.additionTips = "";
            //block.TagBlockList = new List<CommentTagBlock>();

            block.TagBlockList.Add(genCommentTagBlock("入住房型", "房型", 2, "", 1, 1, 0, 0, 2, 0));
            block.TagBlockList.Add(genCommentTagBlock("出游类型", "出游", 8, "", 1, 1, 0, 0, 2, 0));

            m.BlockInfo.Add(block);

            ///////////////////
            block = new CommentBlockInfo();
            block.BlockCategory = 1;
            block.BlockCategoryName = "OverAll";
            //block.TagBlockList = new List<CommentTagBlock>();

            block.TagBlockList.Add(genCommentTagBlock("点评标题", "点评标题", 9, "一句话说说您的感受", 1, 20, 1, 30, 3, 0));
            block.TagBlockList.Add(genCommentTagBlock("详细点评", "详细点评", 10, "请输入详细的点评内容", 1, 20, 1, 30000, 6, 0));

            m.BlockInfo.Add(block);

            /////////////////////////////////////
            block = new CommentBlockInfo();
            block.BlockCategory = 12;//上传照片单独一页
            block.BlockCategoryName = "Pics";
            //block.TagBlockList = new List<CommentTagBlock>();

            block.TagBlockList.Add(genCommentTagBlock("添加照片", "添加照片", -1, "", 0, 0, 0, 0, 1, 30));

            m.BlockInfo.Add(block);

            /////////////////////////////////////
            //ToDo 主题数据来自哪里呢 主题不断增加 如果酒店有主题则输出 如果没有则输出所有主题还是?
            block = new CommentBlockInfo();
            block.BlockCategory = 13;
            block.BlockCategoryName = "Interest";
            //block.TagBlockList = new List<CommentTagBlock>();

            block.TagBlockList.Add(genCommentTagBlock("主题特色", "主题特色", 12, "", 0, 3, 0, 0, 3, 0));

            m.BlockInfo.Add(block);

            return m;
        }

        /// <summary>
        /// 点评默认的标签块结构数据
        /// </summary>
        /// <param name="hotelid"></param>
        /// <returns></returns>
        public static CommentDefaultInfoModel GetCommentDefaultInfo30(int hotelid)
        {
            
            CommentDefaultInfoModel m = GenCommentDefaultInfo30();
            try
            {
                foreach (var b in m.BlockInfo)
                {
                    //需要补充tag标签组的内容
                    if (b.BlockCategory == 3 || b.BlockCategory == 13)
                    {
                        foreach (var tb in b.TagBlockList)
                        {
                            //出游类型是默认生成的几个选项
                            if (tb.CategoryID == 8)
                            {
                                if (DicDefaultTagBlock.ContainsKey(tb.CategoryID))
                                {
                                    tb.CommentTagList.AddRange(DicDefaultTagBlock[tb.CategoryID]);//插入缺省的出游类型
                                }
                                tb.CommentTagList.Add(GetOtherTripTypeTag());//自定义出游类型
                            }
                            else if (tb.CategoryID == 12)
                            {
                                List<CommentTagDefineEntity> interestTags = commentService.GetCommentTagDefineList(tb.CategoryID);
                                if (interestTags != null && interestTags.Count != 0)
                                {
                                    IEnumerable<InterestEntity> interestList = hotelService.GetInterestListByHotel(hotelid);

                                    var tempTags = from tag in interestTags
                                                   join j in interestList on tag.Name equals j.Name
                                                   where j.Type == 1
                                                   group tag by tag.TagID into g
                                                   select new CommentTagDefineEntity
                                                   {
                                                       TagID = g.Key,
                                                       IsDefaultTag = g.ToList().First().IsDefaultTag,
                                                       Name = g.ToList().First().Name,
                                                       Type = g.ToList().First().Type
                                                   };//求两个集合的交集

                                    List<CommentTag> finalCommentTagList = new List<CommentTag>();
                                    if (tempTags != null && tempTags.Count() > 0)
                                    {
                                        int tempCount = tempTags.Count();
                                        if (tempCount >= 5)
                                        {
                                            finalCommentTagList = tempTags.Take(5).Select(t =>
                                                    new CommentTag { TagID = t.TagID, Tag = t.Name }).ToList();//随机五个
                                        }
                                        else
                                        {
                                            finalCommentTagList.AddRange(tempTags.Select(t =>
                                                    new CommentTag { TagID = t.TagID, Tag = t.Name }));
                                            int leftCount = 5 - tempCount;

                                            IEnumerable<InterestHotelCountEntity> interestHotelCount = hotelService.GetInterestHotelCountList(1);
                                            finalCommentTagList.AddRange(interestTags.Select(_ =>
                                                new
                                                {
                                                    TagID = _.TagID,
                                                    TagName = _.Name,
                                                    HotelCount = interestHotelCount.FirstOrDefault(
                                                        j => j.interestName.Equals(_.Name)) != null ?
                                                        interestHotelCount.First(j => j.interestName.Equals(_.Name)).hotelCount : 0
                                                }).OrderByDescending(_ =>
                                                _.HotelCount).Take(leftCount).Select(t =>
                                                    new CommentTag { TagID = t.TagID, Tag = t.TagName }));
                                        }
                                        tb.CommentTagList.AddRange(finalCommentTagList);
                                    }
                                    else
                                    {
                                        IEnumerable<InterestHotelCountEntity> interestHotelCount = hotelService.GetInterestHotelCountList(1);
                                        //排序 查询各个主题对应的酒店数量 降序排列
                                        tb.CommentTagList.AddRange(interestTags.Select(_ =>
                                            new
                                            {
                                                TagID = _.TagID,
                                                TagName = _.Name,
                                                HotelCount = interestHotelCount.FirstOrDefault(
                                                    j => j.interestName.Equals(_.Name)) != null ?
                                                    interestHotelCount.First(j => j.interestName.Equals(_.Name)).hotelCount : 0
                                            }).OrderByDescending(_ =>
                                                _.HotelCount).Take(5).Select(t =>
                                                    new CommentTag { TagID = t.TagID, Tag = t.TagName }));
                                    }
                                }

                                //if (tb.CommentTagList.Count == 0 && DicDefaultTagBlock.ContainsKey(tb.CategoryID))
                                //{
                                //    tb.CommentTagList.AddRange(DicDefaultTagBlock[tb.CategoryID]);//插入缺省的主题标签
                                //}

                                tb.CommentTagList = (from tag in tb.CommentTagList
                                                     group tag by tag.TagID
                                                         into g
                                                         select new CommentTag()
                                                         {
                                                             TagID = g.Key,
                                                             Tag = g.ToList().First().Tag,
                                                             addInfo = g.ToList().First().addInfo
                                                         }).ToList();//去掉重复的

                                tb.CommentTagList.Add(GetOtherInterestTag());//默认自定义主题
                            }
                            else if (tb.CategoryID == 2)
                            {
                                List<CommentTagDefineEntity> tl = commentService.GetOneHotelRoomTypeTags(hotelid);
                                if (tl != null && tl.Count > 0)
                                {
                                    tb.CommentTagList.AddRange(tl.Where(o => o.Type == tb.CategoryID).Select(t => new CommentTag { TagID = t.TagID, Tag = t.Name }));
                                }
                                if (tb.CommentTagList.Count == 0 && DicDefaultTagBlock.ContainsKey(tb.CategoryID))
                                {
                                    tb.CommentTagList.AddRange(DicDefaultTagBlock[tb.CategoryID]);
                                }
                                tb.CommentTagList.Add(GetOtherRoomTag());//自定义房型
                            }
                        }
                    }
                }
            }
            catch(Exception err)
            {
                Log.WriteLog("GetCommentDefaultInfo30 Error:" + err.Message + err.StackTrace);
            }

            return m;
        }

        /// <summary>
        /// 点击有帮助 如果没点过 插入一条有帮助记录；点过了则更新当前状态
        /// 新建帮助记录 返回ID
        /// 更新记录返回0
        /// </summary>
        /// <param name="commentID"></param>
        /// <param name="userID"></param>
        /// <param name="channelID"></param>
        /// <returns></returns>
        public static int InsertOrUpdateCommentUseful(int commentID, long userID, int channelID)
        {
            return commentService.InsertOrUpdateCommentUseful(commentID, userID, channelID);
        }

        /// <summary>
        /// 获得个人主页数据
        /// </summary>
        /// <param name="homeUserID"></param>
        /// <param name="curUserID"></param>
        /// <returns></returns>
        public static PersonalHomePageModel GetPersonalPageModel(long homeUserID, long curUserID, int start = 0, int count = 1000)
        {
            var result = new PersonalHomePageModel();
            if (homeUserID > 0)
            {
                Task<HJD.AccountServices.Entity.MemberProfileInfo> memberInfoTask = Task.Factory.StartNew<HJD.AccountServices.Entity.MemberProfileInfo>(() =>
                {
                    return AccountAdapter.GetCurrentUserInfo(homeUserID);
                });

                Task<List<int>> followTask = Task.Factory.StartNew<List<int>>(() =>
                {
                    int followersCount = AccountAdapter.GetFollowersCountByUserID(homeUserID);
                    int followingsCount = AccountAdapter.GetFollowingsCountByUserID(homeUserID);
                    int followState = AccountAdapter.GetFollowFollowingRelState(homeUserID, curUserID);

                    return new List<int> { followersCount, followingsCount, followState };
                });

                //点评数据
                int totalCount = 0;
                Task homePageComemntItemTask = Task.Factory.StartNew(() =>
                {
                    IEnumerable<CommentInfoEntity> returnList = null;
                    if (homeUserID == curUserID)
                    {
                        returnList = CommentAdapter.GetCommentInfoList20(homeUserID, new int[] { 0, 1 }, out totalCount, -1, start, count);
                    }
                    else
                    {
                        returnList = CommentAdapter.GetCommentInfoList20(homeUserID, new int[] { 1 }, out totalCount, -1, start, count);
                    }
                     


                    if (returnList != null && returnList.Count() != 0)
                    {
                        //获取价格
                        DateTime arrivalTime = CommMethods.GetDefaultCheckIn();
                        DateTime departureTime = arrivalTime.AddDays(1);

                        List<HotelPriceEntity> hpl = HotelAdapter.QueryHotelListPrice(returnList.Select(h => h.Comment.HotelID).ToList(), arrivalTime, departureTime);
                        foreach (var commentInfo in returnList)
                        {
                            decimal MinPrice = 0;
                            int PriceType = 0;
                            var thp = hpl.Where(hp => hp.HotelId == commentInfo.Comment.HotelID);

                            if (thp.Count() == 1 && thp.First().PriceList.Count() > 0)
                            {
                                if (thp.First().PriceList.Where(pr => pr.ChannelID == 100).Count() > 0) //有套餐
                                {
                                    MinPrice = (int)thp.First().PriceList.Where(pr => pr.ChannelID == 100).FirstOrDefault().Price;
                                    PriceType = 3;

                                }
                                else if (thp.First().PriceList.Where(pr => pr.ChannelID == 102).Count() > 0) //JLTour
                                {
                                    MinPrice = (int)thp.First().PriceList.Where(pr => pr.ChannelID == 102).FirstOrDefault().Price;
                                    PriceType = 2;

                                }
                                else
                                {
                                    MinPrice = (int)thp.First().PriceList.Min(P => P.Price);
                                    PriceType = 1;
                                }
                            }
                            else
                            {
                                MinPrice = 0;
                                PriceType = 0;
                            }


                            CommentEntity commentEntity = commentInfo.Comment;
                            string commentTitle = DescriptionHelper.CommentInfoConcat(9, commentInfo);//4.0版本新加的一句话点评
                            string commentDetail = DescriptionHelper.CommentInfoConcat(10, commentInfo);//4.0版本新加的详细点评
                            commentDetail = string.IsNullOrWhiteSpace(commentDetail) || commentDetail.Length <= 1 ? DescriptionHelper.GenCommentContent(commentInfo) : commentDetail;

                            string tripType = DescriptionHelper.CommentInfoConcat(8, commentInfo);
                            string roomType = commentInfo.RoomInfo != null && commentInfo.RoomInfo.Count > 0
                                ? commentInfo.RoomInfo[0].TagName : "";

                            result.commentData.commentList.Add(new HomePageComemntItem()
                            {
                                bigCommentPics = new List<string>(),
                                commentPics = new List<string>(),
                                hotelID = commentEntity.HotelID,
                                hotelName = commentEntity.HotelName,
                                hotelPic = ResourceAdapter.GetHotel(commentEntity.HotelID, 0) != null ? ResourceAdapter.GetHotel(commentEntity.HotelID, 0).Picture : "",
                                commentScore = commentEntity.Score,
                                commentTitle = commentTitle,
                                commentContent = commentDetail,
                                isRecommend = commentEntity.Recommned,
                                commentDate = commentEntity.CreateTime,
                                commentID = commentEntity.ID,
                                reviewCount = commentEntity.ReviewsCount,
                                helpfulCount = commentEntity.UsefulCount,
                                hasClickUseful = false,
                                roomType = roomType,
                                tripType = tripType,
                                TimeDesc = CommentAdapter.genReviewTimeSpan(commentEntity.CreateTime),
                                MinPrice = MinPrice,
                                PriceType = PriceType
                            });
                        }
                    }
                });

                Task<List<CategoryPhotosEntity>> commentPicTask = homePageComemntItemTask.ContinueWith<List<CategoryPhotosEntity>>(obj =>
                {
                    List<CategoryPhotosEntity> photoResult = new List<CategoryPhotosEntity>();
                    var returnList = result.commentData.commentList;
                    if (returnList != null && returnList.Count() != 0)
                    {
                        List<PHSPhotoInfoEntity> photoList = PhotoAdapter.GetWHHotelCommentPhotoByWritings(returnList.Select(_ => _.commentID).ToList());

                        foreach (var groupPhoto in photoList.GroupBy(_ => _.BusinessID))
                        {
                            photoResult.AddRange(getCategoryPhotosList(groupPhoto.ToList()));//点评内容分类照片列表
                        }
                    }
                    return photoResult;
                });

                Task.WaitAll(new Task[] { memberInfoTask, followTask, homePageComemntItemTask, commentPicTask });

                //合并所有点评的照片
                result.commentData.commentList.ForEach(i =>
                {
                    var commentPics = commentPicTask.Result.FindAll(j => j.CommentID == i.commentID);
                    commentPics.ForEach(_ =>
                    {
                        i.bigCommentPics.AddRange(_.BigPhotoUrls.Select(o => o.PicUrl));//大照片
                        i.commentPics.AddRange(_.PhotoUrls.Select(o => o.PicUrl));//小照片
                    });
                });

                result.commentData.TotalCount = totalCount;//点评总数量

                HJD.AccountServices.Entity.MemberProfileInfo memberInfo = memberInfoTask.Result;//AccountAdapter.GetCurrentUserInfo(homeUserID);//成员信息                
                //主页私人数据
                result.homeUserData = new UserData
                {
                    AvatarUrl = memberInfo.AvatarUrl,
                    HomeUserID = homeUserID,
                    IsInspector = memberInfo.IsInspector,
                    CustomerType = ((int)AccountAdapter.GetCustomerType(memberInfo.UserID) == 6 || (int)AccountAdapter.GetCustomerType(memberInfo.UserID) == 7) ? 4 : (int)AccountAdapter.GetCustomerType(memberInfo.UserID),
                    NickName = memberInfo.NickName,
                    FollowersCount = followTask.Result[0],//AccountAdapter.GetFollowersCountByUserID(homeUserID),
                    FollowingsCount = followTask.Result[1],//AccountAdapter.GetFollowingsCountByUserID(homeUserID),
                    FollowState = followTask.Result[2],//AccountAdapter.GetFollowFollowingRelState(homeUserID, curUserID),//followers.Exists(_ => _.Follower == curUserID) ? (followings.Exists(_ => _.Following == curUserID) ? 3 : 2) : (followings.Exists(_ => _.Following == curUserID) ? 1 : 0)
                    PersonalSignature = memberInfo.MemberBrief,
                    ThemeCodeSN = memberInfo.ThemeCodeSN
                };
                var list = PriceService.GetUserOrderList(curUserID, 0, 1000);
                OrderListItem order = list.Where(_ => _.State == 12 && _.CommentID == 0).OrderByDescending(_ => _.CheckIn).FirstOrDefault();
                if (order != null)
                {
                    result.NoCommentHotelName = order.HotelName;
                    result.NoCommentHotelDescribe = "您" + order.CheckIn.ToString("yyyy年MM月dd日") + "入住的" + order.HotelName + "尚未点评";
                    result.NoCommentHotelID = order.HotelId;
                    result.OrderId = order.Orderid;
                }
            }
            return result;
        }
        #endregion

        /// <summary>
        /// 获取用户相关状态点评详细内容
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="stateList"></param>
        /// <param name="totalCount"></param>
        /// <param name="recommendCondition"></param>
        /// <param name="start"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        internal static IEnumerable<CommentInfoEntity> GetCommentInfoList20(long userID, IEnumerable<int> stateList, out int totalCount, int recommendCondition = -1, int start = 0, int count = 1000)
        {
            return commentService.GetCommentInfoList20(userID, stateList, out totalCount, recommendCondition, start, count);
        }

        /// <summary>
        /// 获取用户相关状态点评详细内容
        /// 按地区合并 地区内点评按提交时间倒序 地区之间按各地区最晚的点评时间倒序
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="stateList"></param>
        /// <param name="totalCount"></param>
        /// <param name="recommendCondition"></param>
        /// <param name="start"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        internal static IEnumerable<CommentInfoEntity> GetCommentInfoList30(long userID, IEnumerable<int> stateList, out int totalCount, int recommendCondition = -1, int start = 0, int count = 1000)
        {
            return commentService.GetCommentInfoList30(userID, stateList, out totalCount, recommendCondition, start, count);
        }

        /// <summary>
        /// 获得某个用户关注人写的点评（默认已发表）
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="hotelId"></param>
        /// <returns></returns>
        internal static List<CommentInfoEntity> GetCommentInfosByFollowing(long userId, int hotelId, IEnumerable<int> stateList, out int totalCount, int start = 0, int count = 4)
        {
            totalCount = 0;
            if (userId != 0 && hotelId != 0 && stateList != null && stateList.Any())
            {
                //Log.WriteLog("userID ：" + userId + ", hotelid：" + hotelId + ", totalCount :" + totalCount + ",stateList" + string.Join(",", stateList));
                return commentService.GetCommentInfosByFollowing(userId, hotelId, stateList, out totalCount, start, count);
            }
            else
            {
                return new List<CommentInfoEntity>();
            }
        }

        /// <summary>
        /// 获得品鉴师写的点评
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="hotelId"></param>
        /// <param name="stateList"></param>
        /// <param name="totalCount"></param>
        /// <param name="start"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        internal static List<CommentInfoEntity> GetCommentInfosByInspector(int hotelId, IEnumerable<int> stateList, out int totalCount, int start = 0, int count = 4)
        {
            totalCount = 0;
            if (hotelId != 0 && stateList != null && stateList.Any())
            {
                try
                {
                    return commentService.GetCommentInfosByInspector(hotelId, stateList, out totalCount, start, count);
                }
                catch (Exception err)
                {
                    return new List<CommentInfoEntity>();
                }
            }
            else
            {
                return new List<CommentInfoEntity>();
            }
        }

        /// <summary>
        /// 获取点评清单集合
        /// </summary>
        /// <param name="comments"></param>
        /// <returns></returns>
        internal static List<CommentItem> GenCommentItems(IEnumerable<CommentInfoEntity> comments)
        {
            List<CommentItem> commentItems = new List<CommentItem>();

            var commentIds = comments.Select(_ => _.Comment.ID).Distinct();//点评ID集合
            var userIds = comments.Select(_ => _.Comment.UserID).Distinct();//点评UserID集合

            //点评照片
            var pl = PhotoAdapter.GetWHHotelCommentPhotoByWritings(commentIds);
            string regStr = @"_.*$";
            Regex regx = new Regex(regStr, RegexOptions.IgnoreCase | RegexOptions.RightToLeft);

            //作者昵称 头像
            var userInfos = AccountAdapter.GetUserBasicInfo(userIds);

            foreach (CommentInfoEntity c in comments)
            {
                CommentItem commentItem = new CommentItem();
                commentItem.Id = c.Comment.ID;

                commentItem.CommentTitle = DescriptionHelper.CommentInfoConcat(9, c);
                if (string.IsNullOrWhiteSpace(commentItem.CommentTitle))
                {
                    commentItem.Text = HotelAdapter.GetOrGenWHHotelCommentDescription(c);
                }
                else
                {
                    commentItem.Text = DescriptionHelper.CommentInfoConcat(10, c);//点评详细内容
                    commentItem.AdditionalText = DescriptionHelper.CommentInfoConcat(11, c);//补充点评内容
                }

                commentItem.RoomInfo = string.Join("、", c.RoomInfo.Select(o => o.TagName));

                var tripResult = c.TagInfo.FindAll(_ => _.CategoryID == 8).FirstOrDefault();
                if (tripResult != null && tripResult.CategoryID > 0 && tripResult.Tags.Count > 0)
                {
                    commentItem.TripInfo = string.Join("、", tripResult.Tags.Select(_ => _.TagName));
                }

                //还要有帮助数量
                commentItem.UsefulCount = c.Comment.UsefulCount;
                commentItem.HasClickUseful = false;
                commentItem.Score = (decimal)c.Comment.Score;
                commentItem.Time = c.Comment.CreateTime.ToString("yyyy-MM-dd");

                IEnumerable<PHSPhotoInfoEntity> plList = pl.Where(p => p.BusinessID == c.Comment.ID);
                //还要小图 和 大图链接的数组
                commentItem.CommentPics = plList.Select(p => regx.Replace(p.PhotoUrl[PhotoSizeType.small], "_290x290s")).ToList();
                commentItem.BigCommentPics = plList.Select(p => p.PhotoUrl[PhotoSizeType.appview]).ToList();
                commentItem.PhotoCount = plList.Count();

                long author = c.Comment.UserID;
                var authorInfo = userInfos.First(_ => _.UserId == author);
                commentItem.AvatarUrl = string.IsNullOrWhiteSpace(authorInfo.AvatarUrl) ? DescriptionHelper.defaultAvatar : PhotoAdapter.GenHotelPicUrl(authorInfo.AvatarUrl, Enums.AppPhotoSize.jupiter);
                commentItem.Author = authorInfo.NickName;
                commentItem.AuthorUserID = author;

                commentItems.Add(commentItem);
            }

            return commentItems;
        }

        /// <summary>
        /// 点评的评论列表
        /// </summary>
        /// <param name="commentID"></param>
        /// <returns></returns>
        internal static List<CommentReviewsEntity> GetCommentReviews(int commentID)
        {
            return commentService.GetCommentReviews(commentID).ToList();
        }

        /// <summary>
        /// 获得点评的评论数
        /// </summary>
        /// <param name="commentID"></param>
        /// <returns></returns>
        internal static int GetCommentReviewsCount(int commentID)
        {
            return commentService.GetCommentReviewsCount(commentID);
        }

        /// <summary>
        /// 添加恢复或评论
        /// </summary>
        /// <param name="cre"></param>
        /// <returns></returns>
        internal static long AddReview4Comment(CommentReviewsEntity cre)
        {
            return commentService.AddReview4Comment(cre);
        }

        /// <summary>
        /// 获得某条评论（回复）信息
        /// </summary>
        /// <param name="commentReviewId"></param>
        /// <returns></returns>
        internal static CommentReviewsEntity GetOneCommentReviewsEntity(long commentReviewId)
        {
            return commentService.GetOneCommentReviewsEntity(commentReviewId);
        }

        /// <summary>
        /// 多个点评内容的来源实体
        /// </summary>
        /// <param name="commentReviewIds"></param>
        /// <returns></returns>
        internal static IEnumerable<CommentReviewsEntity> GetManyCommentReviewsEntity(IEnumerable<long> commentReviewIds)
        {
            return commentService.GetManyCommentReviewsEntity(commentReviewIds);
        }

        /// <summary>
        /// 追加点评内容
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        internal static SubmitCommentResultEntity AddCommentContent(SubmitCommentEntity c)
        {
            SubmitCommentResultEntity r = new SubmitCommentResultEntity();
            r.Success = 0;
            r.Message = @"<div style=""margin-top:5em""><div style=""height:1.5em;line-height:1.5em;font-size: 1.15em; color: #333;text-align:center;"">点评提交成功</div><div style=""height: 1.4em; line-height: 1.4em; font-size: 0.9em; color: #888; text-align: center;"">待审核，1-3个工作日后发表</div></div>";
            try
            {
                if (c.UserID == 0)
                {
                    r.Success = 1;
                    r.Message = "用户信息为空，请先登录再提交！";
                    return r;
                }
                else
                {
                    if (c.addInfos != null && c.addInfos.Count != 0)
                    {
                        AddCommentsAddInfo(c.CommentID, c.addInfos);
                    }

                    //查看补充点评是否已经领取过分享现金券 还有是不是应该获得现金券
                    List<HJD.CouponService.Contracts.Entity.AcquiredCoupon> couponlist = CouponAdapter.couponSvc.GetAcquireCouponRecordByUserID(c.UserID);
                    HJD.CouponService.Contracts.Entity.AcquiredCoupon commentCoupon = couponlist != null && couponlist.Count != 0 ?
                        couponlist.FirstOrDefault(_ => _.SourceID == c.CommentID
                        && _.TypeCode.Equals(HJD.CouponService.Contracts.Entity.CouponActivityCode.sharecomment.ToString())) : null;

                    if (c.OrderID == 0 || commentCoupon != null)
                    {
                        r.ShareCoupon = 0;//单位为分
                    }
                    else
                    {
                        r.ShareCoupon = 2000;//单位为分
                    }

                    r.CommentID = c.CommentID;
                    r.DetailUrl = DescriptionHelper.GenAppJumpUrl(HJDAPI.Common.Helpers.Enums.JumpUrlName.commentdetail, r.CommentID);

                    if (c.photoInfos != null && c.photoInfos.Any())
                    {
                        foreach (var photo in c.photoInfos)
                        {
                            photo.AppVer = c.appVer;
                            photo.CommentID = c.CommentID;
                            CommentAdapter.InsertCommentPhotoUpload(photo);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.WriteLog("AddCommentContent ERR:" + e.Message + e.StackTrace);
                r.Message = e.Message;
                r.Success = 2;
                r.CommentID = 0;
                r.ShareCoupon = 0;
                r.DetailUrl = "";
            }
            return r;
        }

        /// <summary>
        /// 补充点评内容的模板
        /// </summary>
        /// <returns></returns>
        internal static CommentDefaultInfoModel GetAddCommentContentDefaultInfo(int commentID)
        {
            CommentDefaultInfoModel m = new CommentDefaultInfoModel();

            m.BlockInfo = new List<CommentBlockInfo>();

            CommentBlockInfo block = null;
            CommentTagBlock c = null;

            ///////////////////补充点评///////////////////
            block = new CommentBlockInfo();
            block.BlockCategory = 11;
            block.BlockCategoryName = "appendComment";
            //block.TagBlockList = new List<CommentTagBlock>();

            block.TagBlockList.Add(genCommentTagBlock("补充点评", "补充点评", 11, "说说您的感受", 0, 1, 0, 500, 6, 0));

            m.BlockInfo.Add(block);

            /////////////////////补充照片////////////////
            block = new CommentBlockInfo();
            block.BlockCategory = 14;//上传照片单独一页
            block.BlockCategoryName = "Pics";
            ////block.TagBlockList = new List<CommentTagBlock>();

            block.TagBlockList.Add(genCommentTagBlock("添加照片", "添加照片", 14, "", 0, 0, 0, 200, 1, 30));

            m.BlockInfo.Add(block);

            m.AlertResult = new AlertPointsRuleResult()
            {
                IsNeed = false,
                Message = ""
            };

            return m;
        }

        internal static CommentDefaultInfoModel GetAddCommentContentDefaultInfo20(int commentID)
        {
            CommentDefaultInfoModel m = new CommentDefaultInfoModel();

            m.BlockInfo = new List<CommentBlockInfo>();

            CommentBlockInfo block = null;
            CommentTagBlock c = null;

            ///////////////////补充点评///////////////////
            block = new CommentBlockInfo();
            block.BlockCategory = 20;
            block.BlockCategoryName = "appendCommentAndPhoto";
            ////block.TagBlockList = new List<CommentTagBlock>();

            block.TagBlockList.Add(genCommentTagBlock("补充点评", "补充点评", 11, "说说您的感受", 0, 1, 0, 500, 6, 0));

            /////////////////////补充照片////////////////
            block.TagBlockList.Add(genCommentTagBlock("添加照片", "添加照片", 14, "添加描述...", 0, 0, 0, 200, 1, 30));

            m.BlockInfo.Add(block);

            m.AlertResult = new AlertPointsRuleResult()
            {
                IsNeed = false,
                Message = ""
            };

            return m;
        }

        /// <summary>
        /// 判断点评是否已追加过内容或图片
        /// </summary>
        /// <param name="commentID"></param>
        /// <returns></returns>
        internal static bool HasCommentAddedContentOrPic(int commentID)
        {
            CommentInfoEntity commentInfo = commentService.GetOneCommentInfo(commentID);//获取点评内容信息
            string additionalContent = DescriptionHelper.CommentInfoConcat(11, commentInfo);//4.1版本新加的追加点评
            if (!string.IsNullOrWhiteSpace(additionalContent))
            {
                return true;
            }
            else
            {
                List<PHSPhotoInfoEntity> photoList = PhotoAdapter.GetWHHotelCommentPhotoByWritings(new List<int> { commentID });//获取点评照片
                if (photoList != null && photoList.Count != 0 && photoList.Exists(_ => _.SubType == 14))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        internal static List<int> GenCanWriteMoreContentCommentList(IEnumerable<int> commentIds)
        {
            //批量获取补充点评的内容
            //1. 补充点评正文  补充点评配图的说明文字 有任何补充文字也不能继续写
            //2. 补充点评的配图 有配图不能继续写
            //3. 在4.2版本上,之前版本写的任何点评 都不能写补充内容？？？
            List<int> result = new List<int>();
            if (commentIds != null && commentIds.Count() != 0)
            {

                List<CommentAddInfoEntity> addInfos = commentService.GetCommentAddInfos(new CommentAddInfoParam()
                {
                    UserID = 0,
                    CommentIdList = commentIds,
                    CatIdList = new int[] { 11 }
                });
                var addInfoCommentIds = addInfos.Select(_ => _.CommentID);

                List<PHSPhotoInfoEntity> photoList = PhotoAdapter.GetWHHotelCommentPhotoByWritings(commentIds);//获取点评照片
                var photoCommentIds = photoList.FindAll(_ => _.SubType == 14).Select(_ => _.BusinessID);

                foreach (var commentId in commentIds)
                {
                    if (!addInfoCommentIds.Contains(commentId) && !photoCommentIds.Contains(commentId))
                    {
                        result.Add(commentId);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 获取一条帮助记录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        internal static CommentUsefulEntity GetOneCommentUseful(int id)
        {
            return commentService.GetOneCommentUseful(id);
        }

        /// <summary>
        /// 获取有帮助记录
        /// </summary>
        /// <param name="commentID"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        internal static IEnumerable<CommentUsefulEntity> GetManyCommentUseful(IEnumerable<int> ids)
        {
            return commentService.GetManyCommentUseful(ids);
        }

        /// <summary>
        /// 获取点评详情
        /// </summary>
        /// <param name="commentId"></param>
        /// <returns></returns>
        internal static CommentInfoEntity GetOneCommentInfoEntity(int commentId)
        {
            return commentService.GetOneCommentInfo(commentId);
        }

        /// <summary>
        /// 按点评ID 获取赞列表
        /// 按时间降序
        /// </summary>
        /// <param name="commentID"></param>
        /// <returns></returns>
        internal static IEnumerable<CommentUsefulEntity> GetCommentUsefulList(int commentID)
        {
            var list = commentService.GetCommentUsefulList(commentID);
            return list != null && list.Count() != 0 ? (from i in list where i.IsUseful == true select i).OrderByDescending(_ => _.ID).ToList() : new List<CommentUsefulEntity>();
        }

        #region 缺省点评信息设置

        /// <summary>
        /// APP 4.2版本调整 点评默认模板
        /// </summary>
        /// <returns></returns>
        private static CommentDefaultInfoModel GenCommentDefaultInfo40()
        {
            List<CommentScoreDefineEntity> defineList = GetManyCommentScoreDefineEntity(new List<int>());
            bool isHaveDefineList = defineList == null || defineList.Count == 0 ? false : true;

            CommentDefaultInfoModel m = new CommentDefaultInfoModel();

            m.BlockInfo = new List<CommentBlockInfo>();
            CommentBlockInfo block = null;

            ////////////////////第一页/////////////////
            block = new CommentBlockInfo();
            block.BlockCategory = 3;
            block.BlockCategoryName = "roomtype&outting";
            block.additionTips = "";

            block.TagBlockList.Add(genCommentTagBlock("入住房型", "房型", 2, "", 1, 1, 0, 0, 3, 0));
            block.TagBlockList.Add(genCommentTagBlock("出游类型", "出游", 8, "", 1, 1, 0, 0, 3, 0));
            block.TagBlockList.Add(genCommentTagBlock("特色", "特色", 12, "", 1, 10, 0, 0, 3, 0));

            m.BlockInfo.Add(block);

            ///////////////第二页/////////////////
            block = new CommentBlockInfo();
            block.BlockCategory = 1;
            block.BlockCategoryName = "OverAll";

            block.TagBlockList.Add(genCommentTagBlock("点评标题", "点评标题", 9, "点评标题", 0, 0, 1, 30, 3, 0));
            //详细点评限制由2000字改为20000字
            block.TagBlockList.Add(genCommentTagBlock("详细点评", "详细点评", 10, "请输入详细的点评内容", 0, 0, 1, 20000, 6, 0));
            block.TagBlockList.Add(genCommentTagBlock("添加照片", "添加照片", -1, "添加描述.....", 0, 0, 0, 200, 3, 30));

            m.BlockInfo.Add(block);

            ///////////////第三页//////////////
            block = new CommentBlockInfo();
            block.BlockCategory = 16;
            block.BlockCategoryName = "moreContent";

            CommentTagBlock c = genCommentTagBlock("酒店环境", "酒店环境", 0, "", 0, 0, 0, 0, 1, 0);
            if (isHaveDefineList)
            {
                c.addScores.AddRange(defineList.FindAll(_ => _.TypeID == 1 || _.TypeID == 14).Select(_ => new AdditionalScore()
                {
                    ScoreName = _.Name,
                    ScoreType = _.TypeID,
                    CommentID = 0,
                    Score = 0f
                }));
            }
            block.TagBlockList.Add(c);

            c = genCommentTagBlock("酒店位置", "酒店位置", 13, "", 0, 0, 0, 0, 3, 0);
            block.TagBlockList.Add(c);

            c = genCommentTagBlock("酒店房间", "酒店房间", 0, "", 0, 0, 0, 0, 3, 0);
            if (isHaveDefineList)
            {
                c.addScores.AddRange(defineList.FindAll(_ => _.TypeID == 2 || _.TypeID == 3 || _.TypeID == 4 || _.TypeID == 15).Select(_ => new AdditionalScore()
                {
                    ScoreName = _.Name,
                    ScoreType = _.TypeID,
                    CommentID = 0,
                    Score = 0f
                }));
            }
            block.TagBlockList.Add(c);

            c = genCommentTagBlock("WIFI", "WIFI", 14, "", 0, 0, 0, 0, 3, 0);
            if (isHaveDefineList)
            {
                c.addScores.AddRange(defineList.FindAll(_ => _.TypeID == 5).Select(_ => new AdditionalScore()
                {
                    ScoreName = _.Name,
                    ScoreType = _.TypeID,
                    CommentID = 0,
                    Score = 0f
                }));
            }
            block.TagBlockList.Add(c);

            c = genCommentTagBlock("活动设施", "活动设施", 0, "", 0, 0, 0, 0, 5, 0);
            if (isHaveDefineList)
            {
                c.addScores.AddRange(defineList.FindAll(_ => _.TypeID == 6 || _.TypeID == 7 || _.TypeID == 8 || _.TypeID == 9 || _.TypeID == 10).Select(_ => new AdditionalScore()
                {
                    ScoreName = _.Name,
                    ScoreType = _.TypeID,
                    CommentID = 0,
                    Score = 0f
                }));
            }
            block.TagBlockList.Add(c);

            c = genCommentTagBlock("餐饮", "餐饮", 15, "", 0, 1, 0, 0, 2, 0);
            if (isHaveDefineList)
            {
                c.addScores.AddRange(defineList.FindAll(_ => _.TypeID == 11).Select(_ => new AdditionalScore()
                {
                    ScoreName = _.Name,
                    ScoreType = _.TypeID,
                    CommentID = 0,
                    Score = 0f
                }));
            }
            block.TagBlockList.Add(c);

            c = genCommentTagBlock("酒店服务", "酒店服务", 0, "", 0, 0, 0, 0, 1, 0);
            if (isHaveDefineList)
            {
                c.addScores.AddRange(defineList.FindAll(_ => _.TypeID == 18 || _.TypeID == 19 || _.TypeID == 20 || _.TypeID == 21).Select(_ => new AdditionalScore()
                {
                    ScoreName = _.Name,
                    ScoreType = _.TypeID,
                    CommentID = 0,
                    Score = 0f
                }));
            }
            block.TagBlockList.Add(c);

            m.BlockInfo.Add(block);

            return m;
        }

        private static CommentTagBlock genCommentTagBlock(string categoryName, string categoryTitle, int categoryID, string additionTipsPlaceHolder, int minTags, int maxTags, int minLength, int maxLength, int defaultRowCount, int picCount)
        {
            CommentTagBlock c = new CommentTagBlock();
            c.CategoryName = categoryName ?? "";
            c.CategoryID = categoryID;
            c.CategoryTitle = categoryTitle ?? "";
            c.additionTips = additionTipsPlaceHolder ?? "";
            c.CommentTagList = new List<CommentTag>();
            c.MaxTags = maxTags;
            c.MinTags = minTags;
            c.MaxLength = maxLength;
            c.MinLength = minLength;
            c.DefaultRowCount = defaultRowCount;
            c.PicCount = picCount;
            c.addScores = new List<AdditionalScore>();
            return c;
        }

        /// <summary>
        /// 点评默认的标签块结构数据
        /// </summary>
        /// <param name="hotelid"></param>
        /// <returns></returns>
        public static CommentDefaultInfoModel GetCommentDefaultInfo40(int hotelid)
        {
            CommentDefaultInfoModel m = GenCommentDefaultInfo40();
            foreach (var b in m.BlockInfo)
            {
                //需要补充tag标签组的内容
                if (b.BlockCategory == 3 || b.BlockCategory == 16)
                {
                    foreach (var tb in b.TagBlockList)
                    {
                        //出游类型是默认生成的几个选项
                        if (tb.CategoryID == 8)
                        {
                            if (DicDefaultTagBlock.ContainsKey(tb.CategoryID))
                            {
                                tb.CommentTagList.AddRange(DicDefaultTagBlock[tb.CategoryID]);//插入缺省的出游类型
                            }
                            tb.CommentTagList.Add(GetOtherTripTypeTag());//自定义出游类型
                        }
                        //主题特色
                        else if (tb.CategoryID == 12)
                        {
                            List<CommentTagDefineEntity> interestTags = commentService.GetCommentTagDefineList(tb.CategoryID);
                            if (interestTags != null && interestTags.Count != 0)
                            {
                                IEnumerable<InterestEntity> interestList = hotelService.GetInterestListByHotel(hotelid);

                                var tempTags = from tag in interestTags
                                               join j in interestList on tag.Name equals j.Name
                                               where j.Type == 1
                                               group tag by tag.TagID into g
                                               select new CommentTagDefineEntity
                                               {
                                                   TagID = g.Key,
                                                   IsDefaultTag = g.ToList().First().IsDefaultTag,
                                                   Name = g.ToList().First().Name,
                                                   Type = g.ToList().First().Type
                                               };//求两个集合的交集

                                List<CommentTag> finalCommentTagList = new List<CommentTag>();
                                if (tempTags != null && tempTags.Count() > 0)
                                {
                                    int tempCount = tempTags.Count();
                                    if (tempCount >= 5)
                                    {
                                        finalCommentTagList = tempTags.Take(5).Select(t =>
                                                new CommentTag { TagID = t.TagID, Tag = t.Name }).ToList();//随机五个
                                    }
                                    else
                                    {
                                        finalCommentTagList.AddRange(tempTags.Select(t =>
                                                new CommentTag { TagID = t.TagID, Tag = t.Name }));
                                        int leftCount = 5 - tempCount;

                                        IEnumerable<InterestHotelCountEntity> interestHotelCount = hotelService.GetInterestHotelCountList(1);
                                        finalCommentTagList.AddRange(interestTags.Select(_ =>
                                            new
                                            {
                                                TagID = _.TagID,
                                                TagName = _.Name,
                                                HotelCount = interestHotelCount.FirstOrDefault(
                                                    j => j.interestName.Equals(_.Name)) != null ?
                                                    interestHotelCount.First(j => j.interestName.Equals(_.Name)).hotelCount : 0
                                            }).OrderByDescending(_ =>
                                            _.HotelCount).Take(leftCount).Select(t =>
                                                new CommentTag { TagID = t.TagID, Tag = t.TagName }));
                                    }
                                    tb.CommentTagList.AddRange(finalCommentTagList);
                                }
                                else
                                {
                                    IEnumerable<InterestHotelCountEntity> interestHotelCount = hotelService.GetInterestHotelCountList(1);
                                    //排序 查询各个主题对应的酒店数量 降序排列
                                    tb.CommentTagList.AddRange(interestTags.Select(_ =>
                                        new
                                        {
                                            TagID = _.TagID,
                                            TagName = _.Name,
                                            HotelCount = interestHotelCount.FirstOrDefault(
                                                j => j.interestName.Equals(_.Name)) != null ?
                                                interestHotelCount.First(j => j.interestName.Equals(_.Name)).hotelCount : 0
                                        }).OrderByDescending(_ =>
                                            _.HotelCount).Take(5).Select(t =>
                                                new CommentTag { TagID = t.TagID, Tag = t.TagName }));
                                }
                            }

                            tb.CommentTagList = (from tag in tb.CommentTagList
                                                 group tag by tag.TagID
                                                     into g
                                                     select new CommentTag()
                                                     {
                                                         TagID = g.Key,
                                                         Tag = g.ToList().First().Tag,
                                                         addInfo = g.ToList().First().addInfo
                                                     }).ToList();//去掉重复的

                            tb.CommentTagList.Add(GetOtherInterestTag());//默认自定义主题
                        }
                        //房型
                        else if (tb.CategoryID == 2)
                        {
                            List<CommentTagDefineEntity> tl = commentService.GetOneHotelRoomTypeTags(hotelid);
                            if (tl != null && tl.Count > 0)
                            {
                                tb.CommentTagList.AddRange(tl.Where(o => o.Type == tb.CategoryID).Select(t => new CommentTag { TagID = t.TagID, Tag = t.Name }));
                            }
                            if (tb.CommentTagList.Count == 0 && DicDefaultTagBlock.ContainsKey(tb.CategoryID))
                            {
                                tb.CommentTagList.AddRange(DicDefaultTagBlock[tb.CategoryID]);
                            }
                            tb.CommentTagList.Add(GetOtherRoomTag());//自定义房型
                        }
                        //酒店位置
                        else if (tb.CategoryID == 13)
                        {
                            if (DicDefaultTagBlock.ContainsKey(tb.CategoryID))
                            {
                                tb.CommentTagList.AddRange(DicDefaultTagBlock[tb.CategoryID]);//插入缺省的酒店位置
                            }
                            tb.CommentTagList.Add(GetOtherHotelLocationTag());//自定义酒店位置
                        }
                        //酒店WIFI
                        else if (tb.CategoryID == 14)
                        {
                            if (DicDefaultTagBlock.ContainsKey(tb.CategoryID))
                            {
                                tb.CommentTagList.AddRange(DicDefaultTagBlock[tb.CategoryID]);//插入缺省的酒店WIFI评价
                            }
                        }
                        //酒店早餐
                        else if (tb.CategoryID == 15)
                        {
                            if (DicDefaultTagBlock.ContainsKey(tb.CategoryID))
                            {
                                tb.CommentTagList.AddRange(DicDefaultTagBlock[tb.CategoryID]);//插入缺省的酒店早餐评价
                            }
                        }
                    }
                }
            }

            return m;
        }
        #endregion

        #region 个人主页点评列表展示
        internal static PersonalHomePageModel20 GetPersonalPageModel20(long homeUserID, long curUserID, int isRecommend = 1, int start = 0, int count = 1000)
        {
            var result = new PersonalHomePageModel20();
            if (homeUserID > 0)
            {
                Task<HJD.AccountServices.Entity.MemberProfileInfo> memberInfoTask = Task.Factory.StartNew<HJD.AccountServices.Entity.MemberProfileInfo>(() =>
                {
                    return AccountAdapter.GetCurrentUserInfo(homeUserID);
                });

                Task<List<int>> followTask = Task.Factory.StartNew<List<int>>(() =>
                {
                    int followersCount = AccountAdapter.GetFollowersCountByUserID(homeUserID);
                    int followingsCount = AccountAdapter.GetFollowingsCountByUserID(homeUserID);
                    int followState = AccountAdapter.GetFollowFollowingRelState(homeUserID, curUserID);

                    return new List<int> { followersCount, followingsCount, followState };
                });

                Task<List<int>> homePageComemntItemTask = Task.Factory.StartNew<List<int>>(() =>
                {
                    List<int> commentIds = new List<int>();
                    int recommendTotalCount = 0;//推荐点评数据
                    IEnumerable<CommentInfoEntity> recommendList = CommentAdapter.GetCommentInfoList30(homeUserID, new int[] { 1 }, out recommendTotalCount, 1, start, count);
                    if (recommendList != null && recommendList.Count() != 0)
                    {
                        foreach (var commentInfo in recommendList)
                        {
                            CommentEntity commentEntity = commentInfo.Comment;
                            string commentTitle = DescriptionHelper.CommentInfoConcat(9, commentInfo);//4.0版本新加的一句话点评
                            string commentDetail = DescriptionHelper.CommentInfoConcat(10, commentInfo);//4.0版本新加的详细点评
                            commentDetail = string.IsNullOrWhiteSpace(commentDetail) || commentDetail.Length <= 1 ? DescriptionHelper.GenCommentContent(commentInfo) : commentDetail;

                            string tripType = DescriptionHelper.CommentInfoConcat(8, commentInfo);
                            string roomType = commentInfo.RoomInfo != null && commentInfo.RoomInfo.Count > 0
                                ? commentInfo.RoomInfo[0].TagName : "";

                            result.recommendCommentData.TotalCount = recommendTotalCount;
                            string districtKey = commentInfo.Comment.DistrictName;
                            if (!result.recommendCommentData.commentGroup.ContainsKey(districtKey))
                            {
                                result.recommendCommentData.commentGroup[districtKey] = new List<HomePageComemntItem>();
                                result.recommendCommentData.commentGroup[districtKey].Add(new HomePageComemntItem()
                                        {
                                            bigCommentPics = new List<string>(),
                                            commentPics = new List<string>(),
                                            hotelID = commentEntity.HotelID,
                                            hotelName = commentEntity.HotelName,
                                            hotelPic = "",
                                            commentScore = commentEntity.Score,
                                            commentTitle = commentTitle,
                                            commentContent = commentDetail,
                                            isRecommend = commentEntity.Recommned,
                                            commentDate = commentEntity.CreateTime,
                                            commentID = commentEntity.ID,
                                            reviewCount = commentEntity.ReviewsCount,
                                            helpfulCount = commentEntity.UsefulCount,
                                            hasClickUseful = false,
                                            roomType = roomType,
                                            tripType = tripType
                                        });
                            }
                            else
                            {
                                result.recommendCommentData.commentGroup[districtKey].Add(new HomePageComemntItem()
                                        {
                                            bigCommentPics = new List<string>(),
                                            commentPics = new List<string>(),
                                            hotelID = commentEntity.HotelID,
                                            hotelName = commentEntity.HotelName,
                                            hotelPic = "",
                                            commentScore = commentEntity.Score,
                                            commentTitle = commentTitle,
                                            commentContent = commentDetail,
                                            isRecommend = commentEntity.Recommned,
                                            commentDate = commentEntity.CreateTime,
                                            commentID = commentEntity.ID,
                                            reviewCount = commentEntity.ReviewsCount,
                                            helpfulCount = commentEntity.UsefulCount,
                                            hasClickUseful = false,
                                            roomType = roomType,
                                            tripType = tripType
                                        });
                            }

                            commentIds.Add(commentEntity.ID);//累计点评ID
                        }
                    }

                    int notRecommendTotalCount = 0;//不推荐点评数据
                    IEnumerable<CommentInfoEntity> notRecommendList = CommentAdapter.GetCommentInfoList30(homeUserID, new int[] { 1 }, out notRecommendTotalCount, 0, start, count);
                    if (notRecommendList != null && notRecommendList.Count() != 0)
                    {
                        foreach (var commentInfo in notRecommendList)
                        {
                            CommentEntity commentEntity = commentInfo.Comment;
                            string commentTitle = DescriptionHelper.CommentInfoConcat(9, commentInfo);//4.0版本新加的一句话点评
                            string commentDetail = DescriptionHelper.CommentInfoConcat(10, commentInfo);//4.0版本新加的详细点评
                            commentDetail = string.IsNullOrWhiteSpace(commentDetail) || commentDetail.Length <= 1 ? DescriptionHelper.GenCommentContent(commentInfo) : commentDetail;

                            string tripType = DescriptionHelper.CommentInfoConcat(8, commentInfo);
                            string roomType = commentInfo.RoomInfo != null && commentInfo.RoomInfo.Count > 0
                                ? commentInfo.RoomInfo[0].TagName : "";

                            result.notRecommendCommentData.TotalCount = notRecommendTotalCount;
                            string districtKey = commentInfo.Comment.DistrictName;
                            if (!result.notRecommendCommentData.commentGroup.ContainsKey(districtKey))
                            {
                                result.notRecommendCommentData.commentGroup[districtKey] = new List<HomePageComemntItem>();
                                result.notRecommendCommentData.commentGroup[districtKey].Add(new HomePageComemntItem()
                                {
                                    bigCommentPics = new List<string>(),
                                    commentPics = new List<string>(),
                                    hotelID = commentEntity.HotelID,
                                    hotelName = commentEntity.HotelName,
                                    hotelPic = "",
                                    commentScore = commentEntity.Score,
                                    commentTitle = commentTitle,
                                    commentContent = commentDetail,
                                    isRecommend = commentEntity.Recommned,
                                    commentDate = commentEntity.CreateTime,
                                    commentID = commentEntity.ID,
                                    reviewCount = commentEntity.ReviewsCount,
                                    helpfulCount = commentEntity.UsefulCount,
                                    hasClickUseful = false,
                                    roomType = roomType,
                                    tripType = tripType
                                });
                            }
                            else
                            {
                                result.notRecommendCommentData.commentGroup[districtKey].Add(new HomePageComemntItem()
                                {
                                    bigCommentPics = new List<string>(),
                                    commentPics = new List<string>(),
                                    hotelID = commentEntity.HotelID,
                                    hotelName = commentEntity.HotelName,
                                    hotelPic = "",
                                    commentScore = commentEntity.Score,
                                    commentTitle = commentTitle,
                                    commentContent = commentDetail,
                                    isRecommend = commentEntity.Recommned,
                                    commentDate = commentEntity.CreateTime,
                                    commentID = commentEntity.ID,
                                    reviewCount = commentEntity.ReviewsCount,
                                    helpfulCount = commentEntity.UsefulCount,
                                    hasClickUseful = false,
                                    roomType = roomType,
                                    tripType = tripType
                                });
                            }

                            commentIds.Add(commentEntity.ID);//累计点评ID
                        }
                    }

                    return commentIds;
                });

                Task<List<CategoryPhotosEntity>> commentPicTask = homePageComemntItemTask.ContinueWith<List<CategoryPhotosEntity>>(obj =>
                {
                    List<CategoryPhotosEntity> photoResult = new List<CategoryPhotosEntity>();
                    var commentIdList = homePageComemntItemTask.Result;
                    if (commentIdList != null && commentIdList.Count() != 0)
                    {
                        List<PHSPhotoInfoEntity> photoList = PhotoAdapter.GetWHHotelCommentPhotoByWritings(commentIdList);

                        foreach (var groupPhoto in photoList.GroupBy(_ => _.BusinessID))
                        {
                            photoResult.AddRange(getCategoryPhotosList(groupPhoto.ToList()));//点评内容分类照片列表
                        }
                    }
                    return photoResult;
                });

                Task.WaitAll(new Task[] { memberInfoTask, followTask, homePageComemntItemTask, commentPicTask });

                //合并所有点评的照片
                foreach (var item in result.recommendCommentData.commentGroup.Values)
                {
                    item.ForEach(i =>
                    {
                        var commentPics = commentPicTask.Result.FindAll(j => j.CommentID == i.commentID);
                        commentPics.ForEach(_ =>
                        {
                            i.bigCommentPics.AddRange(_.BigPhotoUrls.Select(o => o.PicUrl));//大照片
                            i.commentPics.AddRange(_.PhotoUrls.Select(o => o.PicUrl));//小照片
                        });
                    });
                }

                foreach (var item in result.notRecommendCommentData.commentGroup.Values)
                {
                    item.ForEach(i =>
                    {
                        var commentPics = commentPicTask.Result.FindAll(j => j.CommentID == i.commentID);
                        commentPics.ForEach(_ =>
                        {
                            i.bigCommentPics.AddRange(_.BigPhotoUrls.Select(o => o.PicUrl));//大照片
                            i.commentPics.AddRange(_.PhotoUrls.Select(o => o.PicUrl));//小照片
                        });
                    });
                }

                HJD.AccountServices.Entity.MemberProfileInfo memberInfo = memberInfoTask.Result;
                //主页私人数据
                result.homeUserData = new UserData
                {
                    AvatarUrl = memberInfo.AvatarUrl,
                    HomeUserID = homeUserID,
                    IsInspector = memberInfo.IsInspector,
                    NickName = memberInfo.NickName,
                    FollowersCount = followTask.Result[0],
                    FollowingsCount = followTask.Result[1],
                    FollowState = followTask.Result[2],
                    PersonalSignature = memberInfo.MemberBrief,
                    ThemeCodeSN = memberInfo.ThemeCodeSN
                };
            }

            return result;
        }
        #endregion

        public static IEnumerable<CommentEntity> GetManyCommentBasicInfoEntity(IEnumerable<int> commentIds)
        {
            return commentService.GetManyCommentBasicInfoEntity(commentIds);
        }

        private static List<CommentScoreDefineEntity> GetManyCommentScoreDefineEntity(IEnumerable<int> defineIds)
        {
            return commentService.GetManyCommentScoreDefineEntity(defineIds).ToList();
        }

        public static int AddCommentScore(IEnumerable<CommentScoreEntity> scores)
        {
            return commentService.AddCommentScore(scores);
        }

        public static int AddCommentSection(IEnumerable<CommentSectionEntity> sections)
        {
            return commentService.AddCommentSection(sections);
        }

        #region 计算点评的积分
        /// <summary>
        /// 计算点评可以积多少分
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="commentID"></param>
        /// <param name="orderID"></param>
        /// <returns></returns>
        public static int CalculateCommentPoints(long userId, int commentID, long orderID = 0)
        {
            List<PHSPhotoInfoExt> photos = photoService.GetBusinessIDAndPHSIDByUserID(userId, commentID);//发布的点评 与订单无关
            List<CommentAddInfoEntity> addInfos = commentService.GetAddInfoByUserID(userId, commentID);//发布的点评 与订单无关
            List<CommentTagInfoEntity> tagInfos = commentService.GetTagInfoByUserID(userId, commentID);//发布的点评 与订单无关
            List<CommentSectionEntity> sections = commentService.GetSections(commentID);

            int CustomerType = (int)AccountAdapter.GetCustomerType(userId);

            var dic = new Dictionary<int, string>();
            foreach (var g in addInfos.GroupBy(x => x.CommentID))
            {
                string ss = string.Join("", g.Select(i => i.Content));
                dic.Add(g.Key, ss);
            }

            foreach (var g in tagInfos.GroupBy(x => x.CommentID))
            {
                string ss = string.Join("", g.Select(i => i.TagName));
                if (dic.ContainsKey(g.Key))
                {
                    dic[g.Key] += ss;
                }
                else
                {
                    dic.Add(g.Key, ss);
                }
            }

            foreach (var g in sections.GroupBy(x => x.CommentID))
            {
                string ss = string.Join("", g.Select(i => i.Brief));
                if (dic.ContainsKey(g.Key))
                {
                    dic[g.Key] += ss;
                }
                else
                {
                    dic.Add(g.Key, ss);
                }
            }

            var dic2 = new Dictionary<int, int>();
            foreach (var g in photos.GroupBy(x => (int)x.BusinessID))
            {
                int photoLength = g.Select(i => i.PHSID).Count();
                if (dic.ContainsKey(g.Key))
                {
                    dic2.Add(g.Key, photoLength);
                }
            }
            foreach (int key in dic.Keys)
            {
                if (dic[key].Length >= 300 && dic2.ContainsKey(key) && dic2[key] >= 10)
                {
                    if (orderID != 0)
                    {
                        return 40;
                    }
                    else if (orderID == 0)
                    {
                        return 20;
                    }
                }
                else if (dic[key].Length >= 80  && dic2.ContainsKey(key) && dic2[key] >= 3 )
                {
                    if (orderID != 0)
                    {
                        return 20;
                    }
                    else if (orderID == 0)
                    {
                        return 10;
                    }
                }
            }

            return 0;
        }

        /// <summary>
        /// 判断点评是否能享受积分
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="hotelID"></param>
        /// <param name="commentUserId"></param>
        /// <param name="orderID"></param>
        /// <returns></returns>
        public static bool CanAcquireCommentPoint(int ID, int hotelID, long commentUserId, long orderID)
        {
            bool isExist = true;
            //Log.WriteLog(string.Format("orderID:{0},hotelID：{1}.commentUserId：{2},orderID：{3}", orderID, hotelID, commentUserId, orderID));
            if (orderID == 0 && hotelID > 0)
            {
                //先看品鉴酒店 2015.10.16 tracy QQ消息 品鉴师参与品鉴酒店写的点评不积分
                List<InspectorRefHotel> refHotelList = hotelService.GetInspectorRefHotelList(0, commentUserId).FindAll(i => i.State == 3 && i.HotelID == hotelID && i.CommentID == ID).ToList();//品鉴酒店点评不受三个月限制
                //Log.WriteLog(string.Format("commentUserId:{0},refHotelList.Count：{1}", commentUserId, refHotelList.Count));
                //去掉品鉴师才能获得积分  20161104
                if (refHotelList == null || refHotelList.Count == 0)
                {
                    isExist = false;
                }
                //if ((refHotelList == null || refHotelList.Count == 0) && AccountAdapter.IsInspector(commentUserId))
                //{
                //    isExist = false;//2016.01.05 这里没有验证三个月 因为App或网页 点击写点评会限制三个月内开放点评只能写一篇
                //}
            }
            else if (orderID > 0)
            {
                DateTime dtNow = DateTime.Now;
                List<PointsEntity> commentPointsList = hotelService.GetPointsEntity(commentUserId).FindAll(i => i.TypeID == 1);//已获得点评积分

                //2015-12-15 为了防止iOS点评订单出现错误的问题 影响到判断入住三个月内同一家酒店的订单点评不得重复积分
                //弃用commentService.GetUserHotelComments20(hotelID, commentUserId, state) 
                //List<CommentItemEntity> canWriteOrderList = commentService.GetUserCommentList(commentUserId).FindAll(_ => _.HotelID == hotelID && (dtNow - _.Checkin).TotalDays <= 90);//和该点评同一个酒店ID 且是三个月之内入住的订单

                var commentInfos = commentService.GetCommentInfoList(commentUserId, new int[] { 1 });
                List<CommentInfoEntity> orderCommentsWithSameHotel = (from i in commentInfos where i.Comment.ID != ID && i.Comment.HotelID == hotelID && i.Comment.OrderID > 0 && (dtNow - i.Comment.CreateTime).TotalDays <= 90 select i).ToList();//如果订单是三个月内的 其相对应的点评也该是三个月内写的
                //List<UserHotelCommentsEntity> list = commentService.GetUserHotelComments20(hotelID, commentUserId, state).FindAll(i => i.CommentID != 0 && i.CommentID != ID && (dtNow - i.CheckIn).TotalDays <= 90).OrderBy(i => i.CheckIn).ToList();//完整的已发布点评列表 入住三个月内的

                Log.WriteLog(string.Format("commentPointsList:{0}", string.Join(",", orderCommentsWithSameHotel.Select(_ => _.Comment.ID))));
                if (orderCommentsWithSameHotel != null && orderCommentsWithSameHotel.Count != 0 && commentPointsList != null && commentPointsList.Count != 0)
                {
                    foreach (CommentInfoEntity commentEntity in orderCommentsWithSameHotel)
                    {
                        isExist = commentPointsList.Exists(i => i.BusinessID == commentEntity.Comment.ID);
                        if (isExist)
                        {
                            break;
                        }
                    }
                }
                else
                {
                    isExist = false;
                }
            }

            return !isExist;
        }

        #endregion

        #region 推荐的点评列表
        public static RecommendCommentListModel GetRecommendCommentListModel(RecommendCommentListQueryParam param)
        {
            int totalCount = 0;
            List<RecommendCommentEntity> list = commentService.GetRecommendCommentList(new RecommendCommentParam()
            {
                start = param.start,
                count = param.count,
                startTime = null,
                endTime = null,
                stateArray = new int[] { 1 }
            }, out totalCount);

            if (list != null && list.Count > 0)
            {
                var writingList = list.Select(_ => _.CommentID).ToList();//点评ID集合
                List<PHSPhotoInfoEntity> pl = PhotoAdapter.GetWHHotelCommentPhotoByWritings(writingList);

                return new RecommendCommentListModel()
                {
                    CommentList = list.Select(_ => new RecommendCommentModel()
                    {
                        CommentID = _.CommentID,
                        AuthorUserID = _.UserID,
                        PhotoUrl = pl.Exists(j => _.CommentID == j.BusinessID) ? pl.First(j => _.CommentID == j.BusinessID).PhotoUrl[PhotoSizeType.appview].Replace("appview", "290x290") : "",//pl.First(j => _.CommentID == j.BusinessID).PhotoUrl[PhotoSizeType.]
                        AvatarUrl = String.IsNullOrWhiteSpace(_.AvatarSUrl) ? DescriptionHelper.defaultAvatar : PhotoAdapter.GenHotelPicUrl(_.AvatarSUrl, HJDAPI.Common.Helpers.Enums.AppPhotoSize.small),
                        HotelName = _.HotelName ?? "",
                        NickName = _.NickName ?? "",
                        Title = _.CommentTitle ?? ""
                    }).ToList(),
                    CommentTotalCount = totalCount
                };
            }
            else
            {
                return new RecommendCommentListModel();
            }
        }
        #endregion

        #region 点评浏览量计算
        public static int GenCommentPageView(int actualNum, DateTime createTime, bool isRecommend, DateTime recommendTime, int usefulCount, int picCount, int wordCount, bool isInspector)
        {
            decimal factor = 1;
            DateTime curTime = DateTime.Now;
            //time affection
            //if (createTime - )
            //{

            //}
            //content affection
            if (true)
            {

            }
            //usefulCount affection
            if (true)
            {

            }

            return (int)Math.Ceiling(actualNum * factor);
        }

        private double getTimeFactor(DateTime dtTime1, DateTime dtiTime2)
        {
            TimeSpan timeSpan = dtiTime2 - dtTime1;
            if (timeSpan.TotalHours < 1)
            {

            }
            else if (timeSpan.TotalHours < 3)
            {

            }
            else if (timeSpan.TotalHours < 3)
            {

            }
            return 1;
        }
        #endregion

        #region 精彩点评获取
        public static RecommendCommentListModel GetWonderfulCommentListModel(RecommendCommentListQueryParam param)
        {
            int totalCount = 0;
            List<RecommendCommentEntity> list = commentService.GetWonderfulCommentList(new RecommendCommentParam()
            {
                start = param.start,
                count = param.count,
                startTime = null,
                endTime = null,
                stateArray = new int[] { 1 }
            }, out totalCount);

            if (list != null && list.Count > 0)
            {
                var writingList = list.Select(_ => _.CommentID).ToList(); //点评ID集合
                List<PHSPhotoInfoEntity> pl = PhotoAdapter.GetWHHotelCommentPhotoByWritings(writingList);

                return new RecommendCommentListModel()
                {
                    CommentList = list.Select(_ => new RecommendCommentModel()
                    {
                        CommentID = _.CommentID,
                        AuthorUserID = _.UserID,
                        PhotoUrl = string.IsNullOrWhiteSpace(_.DefaultPicSUrl) ? pl.First(j => _.CommentID == j.BusinessID).PhotoUrl[PhotoSizeType.appview].Replace(
                                    "appview", "290x290") : PhotoAdapter.GenHotelPicUrl(_.DefaultPicSUrl, Enums.AppPhotoSize.appview).Replace("appview", "290x290"),
                        PhotoUrls = pl.FindAll(j => _.CommentID == j.BusinessID).Take(9).Select(j => j.PhotoUrl[PhotoSizeType.appview].Replace(
                        "appview", "290x290")).ToList(),
                        AvatarUrl =
                            String.IsNullOrWhiteSpace(_.AvatarSUrl)
                                ? DescriptionHelper.defaultAvatar
                                : PhotoAdapter.GenHotelPicUrl(_.AvatarSUrl,
                                    HJDAPI.Common.Helpers.Enums.AppPhotoSize.small),
                        HotelName = _.HotelName ?? "",
                        HotelId = _.HotelID,
                        NickName = _.NickName ?? "",
                        Title = _.CommentTitle ?? "",
                        Content = !string.IsNullOrWhiteSpace(_.CommentContent) ? _.CommentContent.Replace("\r", "").Replace("\n", "").Trim() : "",
                        RecommendDesc = "点评并推荐了酒店",
                        RoleDesc = _.InspectorState == 2 ? "(品鉴师)" : _.InspectorState == 6 ? "" : "",
                        TimeDesc = CommentAdapter.genReviewTimeSpan(_.CreateTime)
                    }).ToList(),
                    CommentTotalCount = totalCount
                };
            }
            else
            {
                return new RecommendCommentListModel();
            }
        }
        #endregion

        #region 套餐关联的点评
        public static ReviewResult40 GetCommentInfosByPId(int pid, int start = 0, int count = 3)
        {
            var reviews = new ReviewResult40()
            {
                HotelID = 0,
                Start = start
            };
            if (pid > 0)
            {
                float score = 4.5f;
                int totalCount = 0;
                List<CommentInfoEntity> commentList = GetCommentInfosByPId(pid, score, new int[] { 1 }, out totalCount, start, count);
                if (commentList != null && commentList.Count != 0)
                {
                    reviews.HotelID = commentList.First().Comment.HotelID;
                    reviews.TotalCount = totalCount;
                    reviews.AllReviewCount = totalCount;
                    reviews.GroupTotalCount = totalCount;
                    reviews.Start = start;

                    reviews.Result = CommentAdapter.GenCommentItems(commentList);

                    //var likeCommentIds = CommentAdapter.commentService.GetUserClickUsefulComment(userId);
                    //if (likeCommentIds != null && likeCommentIds.Count > 0)
                    //{
                    //    foreach (CommentItem item in reviews.Result)
                    //    {
                    //        item.HasClickUseful = likeCommentIds.Contains(item.Id) ? true : false;
                    //    }
                    //}
                }
            }
            return reviews;
        }

        /// <summary>
        /// 获取特惠套餐相关的点评列表
        /// </summary>
        /// <param name="pId"></param>
        /// <param name="hotelId"></param>
        /// <param name="score"></param>
        /// <param name="stateList"></param>
        /// <param name="totalCount"></param>
        /// <param name="start"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        private static List<CommentInfoEntity> GetCommentInfosByPId(int pId, float score, IEnumerable<int> stateList, out int totalCount, int start = 0, int count = 4)
        {
            return commentService.GetCommentInfosByPId(pId, score, stateList, out totalCount, start, count);
        }
        #endregion

        #region 一段时间内发布的点评

        public static RecommendCommentListModel GetPublishedCommentList(RecommendCommentParam param)
        {
            DateTime arrivalTime = CommMethods.GetDefaultCheckIn();

            int totalCount = 0;
            List<RecommendCommentEntity> list = commentService.GetPublishedCommentList(new RecommendCommentParam()
            {
                start = param.start,
                count = param.count,
                startTime = null,
                endTime = null,
                stateArray = new int[] { 1 },
                districtid = param.districtid,
                lat = param.lat,
                lng = param.lng
            }, out totalCount);

            if (list != null && list.Count > 0)
            {
                var writingList = list.Select(_ => _.CommentID).ToList();   //点评ID集合
                var hotelIdList = list.Select(_ => _.HotelID).ToList();     //酒店ID集合

                var pl = PhotoAdapter.GetWHHotelCommentPhotoByWritings(writingList);    //取照片 价格 点评分 推荐数量
                var hotelPicsList = HotelAdapter.GetManyHotelPhotos(hotelIdList);

                //获取当前用户遵循的列表价类型
                var minPriceTypes = PriceAdapter.GetHotelMinPriceTypeByUser(param.userId);

                //获取HotelMinPrice列表价
                var hotelMinPriceList = PriceService.GetHotelMinPriceList(hotelIdList, arrivalTime) ?? new List<HotelMinPriceEntity>();
                var pricePlanList = new List<PricePlanEx>();

                if (hotelMinPriceList == null || hotelMinPriceList.Count == 0)
                {
                    //获取PricePlan列表价
                    pricePlanList = PriceService.GetPricePlanExList(hotelIdList, arrivalTime) ?? new List<PricePlanEx>();
                }

                ////查询酒店列表价
                //var pricePlanList = PriceAdapter.PriceService.GetPricePlanExList(hotelIdList, DateTime.Now.Date) ?? new List<HJD.HotelPrice.Contract.DataContract.PricePlanEx>();

                return new RecommendCommentListModel()
                {
                    CommentList = list.Select(_ => new RecommendCommentModel()
                    {
                        CommentID = _.CommentID,
                        AuthorUserID = _.UserID,
                        PhotoUrl = pl.FirstOrDefault(j => _.CommentID == j.BusinessID) != null ? pl.First(j => _.CommentID == j.BusinessID).PhotoUrl[PhotoSizeType.appview].Replace("appview", "290x290") : "",
                        PhotoUrls = pl.FirstOrDefault(j => _.CommentID == j.BusinessID) != null ? pl.FindAll(j => _.CommentID == j.BusinessID).Take(9).Select(j => j.PhotoUrl[PhotoSizeType.appview].Replace("appview", "290x290")).ToList() : new List<string>(),
                        AvatarUrl = String.IsNullOrWhiteSpace(_.AvatarSUrl) ? DescriptionHelper.defaultAvatar : PhotoAdapter.GenHotelPicUrl(_.AvatarSUrl, HJDAPI.Common.Helpers.Enums.AppPhotoSize.small),
                        HotelName = _.HotelName ?? "",
                        HotelId = _.HotelID,
                        NickName = _.NickName ?? "",
                        Title = _.CommentTitle ?? "",
                        Content = !string.IsNullOrWhiteSpace(_.CommentContent) ? _.CommentContent.Replace("\r", "").Replace("\n", "").Trim() : "",
                        RecommendDesc = _.IsRecommend ? "点评并推荐了酒店" : "",
                        RoleDesc = _.InspectorState == 2 ? "(品鉴师)" : _.InspectorState == 6 ? "" : "",
                        TimeDesc = CommentAdapter.genReviewTimeSpan(_.CreateTime),
                        HotelScore = _.HotelScore,
                        HotelPicUrl = hotelPicsList.FirstOrDefault(p => p.HotelId == _.HotelID) != null && hotelPicsList.First(p => p.HotelId == _.HotelID).HPList.Any() ? PhotoAdapter.GenHotelPicUrl(hotelPicsList.First(p => p.HotelId == _.HotelID).HPList[0].SURL, Enums.AppPhotoSize.appview).Replace("appview", "290x290") : "",
                        HotelPrice = PriceAdapter.GetHotelPricePlan(_.HotelID, pricePlanList, hotelMinPriceList, minPriceTypes, arrivalTime, false).Price,
                        HotelRecommendedCount = _.RecommendedCount,
                        CustomerType = (int)AccountAdapter.GetCustomerType(_.UserID)
                    }).ToList(),
                    CommentTotalCount = totalCount,
                    BlockTitle = "大家正在推荐"
                };
            }
            else
            {
                return new RecommendCommentListModel();
            }
        }

        #endregion

        #region 推荐

        public static RecommendCommentListModel GetPublishedCommentsList(RecommendCommentParam param)
        {
            var result = new RecommendCommentListModel();

            DateTime arrivalTime = CommMethods.GetDefaultCheckIn();

            int totalCount = 0;
            List<RecommendCommentEntity> list = commentService.GetPublishedCommentListByStr(new RecommendCommentParam()
            {
                start = param.start,
                count = param.count,
                startTime = null,
                endTime = null,
                stateArray = new int[] { 1 },
                userId = param.userId,
                isFollow = param.isFollow
            }, 
            out totalCount);
            if (list != null && list.Count > 0)
            {
                var writingList = list.Select(_ => _.CommentID).ToList(); //点评ID集合
                var hotelIdList = list.Select(_ => _.HotelID).ToList();//酒店ID集合

                var pl = PhotoAdapter.GetWHHotelCommentPhotoByWritings(writingList);//取照片 价格 点评分 推荐数量
                var hotelPicsList = HotelAdapter.GetManyHotelPhotos(hotelIdList);

                //获取当前用户遵循的列表价类型
                var minPriceTypes = PriceAdapter.GetHotelMinPriceTypeByUser(param.userId);

                //获取HotelMinPrice列表价
                var hotelMinPriceList = PriceService.GetHotelMinPriceList(hotelIdList, arrivalTime) ?? new List<HotelMinPriceEntity>();
                var pricePlanList = new List<PricePlanEx>();

                if (hotelMinPriceList == null || hotelMinPriceList.Count == 0)
                {
                    //获取PricePlan列表价
                    pricePlanList = PriceService.GetPricePlanExList(hotelIdList, arrivalTime) ?? new List<PricePlanEx>();
                }

                ////查询酒店
                //var pricePlanList = PriceAdapter.PriceService.GetPricePlanExList(hotelIdList, DateTime.Now.Date) ?? new List<HJD.HotelPrice.Contract.DataContract.PricePlanEx>();
                
                try { 
                    result = new RecommendCommentListModel()
                    {
                        CommentList = list.Select(_ => new RecommendCommentModel()
                        {
                            CommentID = _.CommentID,
                            AuthorUserID = _.UserID,
                            PhotoUrl = pl.FirstOrDefault(j => _.CommentID == j.BusinessID) != null ? pl.First(j => _.CommentID == j.BusinessID).PhotoUrl[PhotoSizeType.appview].Replace("appview", "290x290") : "",
                            PhotoUrls = pl.FirstOrDefault(j => _.CommentID == j.BusinessID) != null ? pl.FindAll(j => _.CommentID == j.BusinessID).Take(9).Select(j => j.PhotoUrl[PhotoSizeType.appview].Replace("appview", "290x290")).ToList() : new List<string>(),
                            AvatarUrl = String.IsNullOrWhiteSpace(_.AvatarSUrl) ? DescriptionHelper.defaultAvatar : PhotoAdapter.GenHotelPicUrl(_.AvatarSUrl, HJDAPI.Common.Helpers.Enums.AppPhotoSize.small),
                            HotelName = _.HotelName ?? "",
                            HotelId = _.HotelID,
                            NickName = _.NickName ?? "",
                            Title = _.CommentTitle ?? "",
                            Content = !string.IsNullOrWhiteSpace(_.CommentContent) ? _.CommentContent.Replace("\r", "").Replace("\n", "").Trim() : "",
                            RecommendDesc = _.IsRecommend ? "点评并推荐了酒店" : "",
                            RoleDesc = _.InspectorState == 2 ? "(品鉴师)" : _.InspectorState == 6 ? "" : "", 
                            TimeDesc = CommentAdapter.genReviewTimeSpan(_.CreateTime),
                            HotelScore = _.HotelScore,
                            HotelPicUrl = hotelPicsList.FirstOrDefault(p => p.HotelId == _.HotelID) != null && hotelPicsList.First(p => p.HotelId == _.HotelID).HPList.Any() ? PhotoAdapter.GenHotelPicUrl(hotelPicsList.First(p => p.HotelId == _.HotelID).HPList[0].SURL, Enums.AppPhotoSize.appview).Replace("appview", "290x290") : "",
                            HotelPrice = PriceAdapter.GetHotelPricePlan(_.HotelID, pricePlanList, hotelMinPriceList, minPriceTypes, arrivalTime, false).Price,
                            HotelRecommendedCount = _.RecommendedCount,
                            CustomerType = (int)AccountAdapter.GetCustomerType(_.UserID),
                            Score = _.Score,
                            followState = AccountAdapter.GetFollowFollowingRelState(_.UserID, param.userId)
                        }).ToList(),
                    };
                }
                catch (Exception e)
                {
                    Log.WriteLog("GetPublishedCommentsList:" + e); 
                }
            }

            result.FollowingsCount = AccountAdapter.GetFollowingsCountByUserID(param.userId);
            result.FollowersCount = AccountAdapter.GetFollowersCountByUserID(param.userId);
            result.CommentTotalCount = totalCount;
            result.BlockTitle = "大家正在推荐";
            return result;
        }

        #endregion

        #region 朋友推荐的点评

        /// <summary>
        /// 获取朋友推荐的酒店
        /// </summary>
        /// <param name="param"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public static RecommendHotelResult GetRecommendHotelListByFollowing(RecommendCommentParam param)
        {
            DateTime arrivalTime = CommMethods.GetDefaultCheckIn();

            var result = new RecommendHotelResult()
            {
                HotelBlockTitle = "朋友推荐酒店",
                HotelTotalCount = 0,
                HotelList = new List<RecommendHotelItem>()
            };

            int totalCount = 0;
            List<RecommendCommentEntity> recommendCommentList = commentService.GetRecommendHotelListByFollowing(param, out totalCount);
            if (totalCount > 0 && recommendCommentList.Any())
            {
                var hotelIdList = recommendCommentList.Select(_ => _.HotelID).ToList();//酒店ID集合
                var hotelPicsList = HotelAdapter.GetManyHotelPhotos(hotelIdList);

                //获取当前用户遵循的列表价类型
                var minPriceTypes = PriceAdapter.GetHotelMinPriceTypeByUser(param.userId);

                //获取HotelMinPrice列表价
                var hotelMinPriceList = PriceService.GetHotelMinPriceList(hotelIdList, arrivalTime) ?? new List<HotelMinPriceEntity>();
                var pricePlanList = new List<PricePlanEx>();

                if (hotelMinPriceList == null || hotelMinPriceList.Count == 0)
                {
                    //获取PricePlan列表价
                    pricePlanList = PriceService.GetPricePlanExList(hotelIdList, arrivalTime) ?? new List<PricePlanEx>();
                }

                ////查询酒店列表价
                //pricePlanList = PriceAdapter.PriceService.GetPricePlanExList(hotelIdList, arrivalTime) ?? new List<HJD.HotelPrice.Contract.DataContract.PricePlanEx>();

                result.HotelTotalCount = totalCount;
                result.HotelList = new List<RecommendHotelItem>();
                if (recommendCommentList != null && recommendCommentList.Count > 0)
                {
                    foreach (var _ in recommendCommentList)
                    {
                        var _pricePlanItem = PriceAdapter.GetHotelPricePlan(_.HotelID, pricePlanList, hotelMinPriceList, minPriceTypes, arrivalTime, false);

                        HotelItem hi = ResourceAdapter.GetHotel(_.HotelID, 0);
                        var hotelItem = new RecommendHotelItem();
                        hotelItem.ADDescription = "";
                        hotelItem.CustomerType = 0;
                        hotelItem.HotelID = _.HotelID;
                        hotelItem.HotelName = _.HotelName;
                        hotelItem.HotelPicUrl = hotelPicsList.FirstOrDefault(p => p.HotelId == _.HotelID) != null && hotelPicsList.First(p => p.HotelId == _.HotelID).HPList.Any() ? PhotoAdapter.GenHotelPicUrl(hotelPicsList.First(p => p.HotelId == _.HotelID).HPList[0].SURL, Enums.AppPhotoSize.theme) : "";
                        hotelItem.HotelPrice = _pricePlanItem.Price;
                        hotelItem.HotelReviewCount = hi.ReviewCount;
                        hotelItem.HotelScore = hi.Score;
                        hotelItem.MarketPrice = 0;
                        hotelItem.NotVIPPrice = 0;
                        hotelItem.PackageBrief = string.IsNullOrWhiteSpace(_.CommentContent) ? "" : _.CommentContent.Trim().Length >= 30 ? _.CommentContent.Trim().Substring(0, 30) : _.CommentContent.Trim().Substring(0, _.CommentContent.Trim().Length);
                        hotelItem.packageContent = new List<string>();
                        hotelItem.PackageName = "";
                        hotelItem.packageNotice = new List<string>();
                        hotelItem.PID = 0;
                        hotelItem.RecomemndWord = "";
                        hotelItem.RecommendCount = 0;
                        hotelItem.RecommendPicUrl = "";
                        hotelItem.VIPPrice = _pricePlanItem.VipPrice;
                        hotelItem.AvatarUrl = string.IsNullOrWhiteSpace(_.AvatarSUrl) ? DescriptionHelper.defaultAvatar : PhotoAdapter.GenHotelPicUrl(_.AvatarSUrl, Enums.AppPhotoSize.jupiter).Replace("jupiter", "120x120");

                        result.HotelList.Add(hotelItem);
                    }
                }
            }
            return result;
        }

        #endregion
    }
}