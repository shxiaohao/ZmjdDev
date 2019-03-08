using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using HJD.DestServices.Contract;
using HJD.HotelServices.Contracts;
using HJDAPI.Models;
using System.Runtime.Serialization;
using HJD.CommentService.Contract;
using System.Net.Http;
using Newtonsoft.Json;
using HJDAPI.Controllers.Common;
using HJDAPI.Common.Security;
using HJD.PhotoServices.Entity;
using HJD.AccountServices.Entity;
using HJD.Framework.WCF;
using HJD.AccountServices.Contracts;
using HJD.HotelPrice.Contract;
using HJDAPI.Common.Helpers;
using HJDAPI.Controllers.Adapter;
using System.Transactions;


namespace HJDAPI.Controllers
{
    public class InspectorController : BaseApiController
    {
        public static HJD.HotelManagementCenter.IServices.IVoucherService voucherService = ServiceProxyFactory.Create<HJD.HotelManagementCenter.IServices.IVoucherService>("IVoucherService");
        public static IHotelService hotelService = ServiceProxyFactory.Create<IHotelService>("BasicHttpBinding_IHotelService");
        public static IAccountService AccService = ServiceProxyFactory.Create<IAccountService>("BasicHttpBinding_IAccountService");
        public static int orderStatus = 12;
        public static int minCount = 3;
        public static bool isNoRepeat = true;
        private readonly object _lockObj = new object();

        const string noticeCheckInspectorHotelTemplate =
            @"免费品鉴酒店有新增待审核记录，详情查看 http://bg.zmjiudian.com/Inspector/InspectorRefList?id={0} ";

        [HttpPost]
        //检查品鉴师申请人资格
        public ResultEntity CheckInspectorAccess(OperationResult item)
        {
            User_Info info = null;
            if (!string.IsNullOrEmpty(item.Mobile))
            {
                info = AccountAdapter.GetOrRegistPhoneUser(item.Mobile, 0);
            }
            if (info != null && info.UserId != 0)
            {
                Inspector instor = AccountAdapter.GetInspector(info.UserId, null);
                //&& instor.State != -1 需求变动 不符合资格直接可进入后台待审核-不符合资格列表
                if (instor != null && instor.UserID != 0)
                {
                    return new ResultEntity() { Message = "您的报名信息已收到，无需重复提交", Success = 4 };
                }

                ResultEntity result = new ResultEntity() { Message = "", Success = 0 };
                int hotelCount = hotelService.GetOrderHotelCountByUserId(info.UserId, orderStatus, isNoRepeat);//12是已确认订单 hard code
                int accumulativePoints = hotelService.GetPointsEntity(info.UserId).Sum(i => i.TotalPoint);

                if (hotelCount < minCount)
                {
                    result = new ResultEntity() { Message = string.Format("您入住酒店的次数不足{0}次", minCount), Success = 2 };
                }
                //int commentCount = hotelService.GetCommentHotelCountByUserId(info.UserId, isNoRepeat);
                //if (commentCount < minCount)
                //{
                //    return new ResultEntity() { Message = string.Format("您写点评的次数不足{0}次", minCount), Success = 3 };
                //}                
                else if (accumulativePoints < 30)
                {
                    result = new ResultEntity() { Message = "您累计获得的点评奖励积分为" + accumulativePoints + "分，小于30分", Success = 6 };
                }

                //保存符合要求者的个人信息到品鉴师表 包括基本参数的设置
                Inspector ee = new Inspector()
                {
                    UserID = info.UserId,
                    TrueName = item.TrueName,
                    Mail = string.IsNullOrEmpty(item.Email) ? " " : item.Email,
                    MobilePhone = item.Mobile,
                    CreateTime = DateTime.Now,
                    UpdateTime = DateTime.Now,
                    State = result.Success == 0 ? 1 : -1,
                    CommissionRatio = 1,
                    IdentityCode = "",
                    SourceType = 3
                };
                try
                {
                   // Log.WriteLog("InsertInspector:" + info.UserId.ToString());
                    AccService.InsertInspector(ee);//自带更新
                }
                catch (Exception ex)
                {
                    Log.WriteLog(ex.Message);
                    result = new ResultEntity() { Message = "数据发生异常,请联系管理员", Success = 5 };
                }

                return result;
            }
            return new ResultEntity() { Message = "您还未成功预订并且入住过酒店", Success = 1 };
        }

        [HttpPost]
        public ResultEntity BookInspectorHotel(BookInspectorHotelParam param)
        {
            ResultEntity re = new ResultEntity();
            DateTime checkIn = DateTime.MinValue;
            DateTime checkOut = DateTime.MinValue;
            try
            {
                checkIn = DateTime.Parse(param.checkin);
                checkOut = DateTime.Parse(param.checkout);
            }
            catch
            {
                return new ResultEntity() { Success = 2, Message = "日期格式错误" };
            }
            //只限制 通过缓存控制剩余数量的申请名额
            lock (_lockObj)
            {
                re.Success = hotelService.BookInspectorRefHotelEx(new InspectorRefHotel()
                {
                    State = 1,
                    CreateTime = DateTime.Now,
                    UpdateTime = DateTime.Now,
                    CheckInDate = checkIn,
                    CheckOutDate = checkOut,
                    Inspector = param.userid,
                    InspectorHotel = (int)param.id,
                    HotelID = param.hotelid,
                    CheckTime = DateTime.Now
                });
            }
            re.Message = re.Success == 0 ? "" : "名额已满";
            if (re.Success != 0) return re;

            string sms = string.Format(noticeCheckInspectorHotelTemplate, param.id);
            SMServiceController.SendSMS("18602189876", sms);
            SMServiceController.SendSMS("18001801688", sms);

            return re;
            //re.Success = hotelService.AddInspectorRefHotel(param.id, param.userid, param.hotelid, checkIn, checkOut);
        }

        //[HttpGet]
        //public List<EvaluationerHotel> GetEvaluationerHotelList(long evaHotelID, long userID)
        //{
        //    return hotelService.GetEvaluationerHotelList(evaHotelID, userID);
        //}

        /// <summary>
        /// 申请房券
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        public ResultEntity BookVoucherHotel(BookInspectorHotelParam param)
        {
            ResultEntity re = new ResultEntity();
            DateTime checkIn = DateTime.MinValue;
            DateTime checkOut = DateTime.MinValue;
            try
            {
                checkIn = DateTime.Parse(param.checkin);
                checkOut = DateTime.Parse(param.checkout);
            }
            catch
            {
                return new ResultEntity() { Success = 2, Message = "日期格式错误" };
            }
            //只限制 通过缓存控制剩余数量的申请名额
            lock (_lockObj)
            {
                re.Message = hotelService.BookVoucherInspectorRefHotelEx(new InspectorRefHotel()
                {
                    State = 1,
                    CreateTime = DateTime.Now,
                    UpdateTime = DateTime.Now,
                    CheckInDate = checkIn,
                    CheckOutDate = checkOut,
                    Inspector = param.userid,
                    InspectorHotel = param.HVID,
                    HotelID = param.hotelid,
                    CheckTime = DateTime.Now,
                    HVID = param.HVID,
                    VID= param.VID
                });
            }
            re.Success = re.Message == "" ? 0 : 1;
            if (re.Success != 0) return re;

            string sms = string.Format(noticeCheckInspectorHotelTemplate, param.id);
            SMServiceController.SendSMS("18602189876", sms);
            SMServiceController.SendSMS("18001801688", sms);

            return re;
            //re.Success = hotelService.AddInspectorRefHotel(param.id, param.userid, param.hotelid, checkIn, checkOut);
        }

        /// <summary>
        /// 房券列表
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        [HttpGet]
        public List<HotelVoucherAndInspectorHotel> GetHotelVoucherList(long userid,int start=1,int counts=10)
        {

            List<HotelVoucherAndInspectorHotel> voucherAndInspectorList = new List<HotelVoucherAndInspectorHotel>();

            //房券信息

            HotelVoucherListResult HotelVoucherResult = new HotelVoucherListResult()
            {
                currentResult = new HotelVoucherList(),
                pastResult = new HotelVoucherList()
            };

            ////已报名过的酒店
            //List<VoucherEntity> voucherList = new List<VoucherEntity>();
            //if (userid != 0)
            //{
            //    voucherList = hotelService.GetUseVoucherList(userid, 0);
            //}
            //List<long> voucherIDList = new List<long>();
            //if (voucherList != null && voucherList.Count != 0)
            //{
            //    voucherIDList = (from i in voucherList select (long)i.HVID).ToList<long>();
            //}

            int count = 0;
            //得到品鉴酒店
            List<HotelVoucherEntity> HotelVoucherList = hotelService.GetUseHotelVoucher(out count);
            HotelVoucherResult.currentResult.count=count;
            
            List<int> hotelIdList1 = new List<int>();
            if (HotelVoucherList != null && HotelVoucherList.Count != 0)
            {
                hotelIdList1 = (from i in HotelVoucherList select i.HotelID).ToList<int>();
            }
            if (hotelIdList1 != null && hotelIdList1.Count != 0)
            {
                List<ListHotelItem2> hotelList = HotelAdapter.GetCollectHotelList(hotelIdList1);//hotelIdList.ConvertAll<int>(i => (int)i)             

                DateTime arrivalTime = CommMethods.GetDefaultCheckIn();
                DateTime departureTime = arrivalTime.AddDays(1);

                hotelList.ForEach(h => h.Picture = PhotoAdapter.GenHotelPicUrl(h.PicSURL, Enums.AppPhotoSize.appdetail));//拼图片链接

                HotelVoucherResult.currentResult.items = new List<HotelVoucherResult>();
                
                foreach (HotelVoucherEntity hv in HotelVoucherList)
                {
                    HotelVoucherAndInspectorHotel item = new HotelVoucherAndInspectorHotel();
                    item.BS = 1;
                    item.HVID = hv.ID;
                    item.HotelName = hv.HotelName;
                    item.HotelID = hv.HotelID;
                    item.Description = hv.RoomCode + " " + ((hv.BreakfastCount == 1 ? "单" : hv.BreakfastCount == 2 ? "双" : hv.BreakfastCount == 0 ? "无" : hv.BreakfastCount.ToString()) + "早");
                    item.hotelItem = hotelList.FirstOrDefault<ListHotelItem2>(i => i.Id == hv.HotelID);
                    item.RequiredPoint = HotelAdapter.GetVRateByHVID(hv.ID).FirstOrDefault() != null ? HotelAdapter.GetVRateByHVID(hv.ID).Where(_ => _.Integral != 0).OrderBy(_ => _.Integral).FirstOrDefault().Integral : 0;//hv.Integral;
                    item.IsShow = hv.IsShow;
                    voucherAndInspectorList.Add(item);
                }
            }
            #region 去掉旧的品鉴酒店列表

            ////old 品鉴酒店信息
            ////限制三个月内的品鉴酒店
            //DateTime maxLimitTime = DateTime.Parse("2014-1-1");//Kevincai 取消限制  DateTime.Now.AddMonths(-3);

            //InspectorHotelListResult result = new InspectorHotelListResult()
            //{
            //    currentResult = new HotelListResult(),
            //    pastResult = new HotelListResult()
            //};
            ////已报名过的酒店
            //List<InspectorRefHotel> evaluationerHotelList = new List<InspectorRefHotel>();
            //if (userid != 0)
            //{
            //    evaluationerHotelList = hotelService.GetInspectorRefHotelList(0, userid);
            //}
            //List<long> evaluationHotelIDList = new List<long>();
            //if (evaluationerHotelList != null && evaluationerHotelList.Count != 0)
            //{
            //    evaluationHotelIDList = (from i in evaluationerHotelList select (long)i.InspectorHotel).ToList<long>();
            //}

            //int count1 = 0;
            //Dictionary<string, string> sortDic = new Dictionary<string, string>();
            ////sortDic.Add("Rank", "asc");//按照排名出现
            //InspectorHotelSearchParam param = new InspectorHotelSearchParam()
            //{
            //    PageIndex = 1,
            //    PageSize = 100,
            //    Filter = new InspectorHotelFilter()
            //    {
            //        IsExpired = false,
            //        IsValid = true,
            //        MaxLimitTime = maxLimitTime,
            //    },
            //    Sort = sortDic
            //};
            //List<InspectorHotel> list1 = hotelService.GetInspectorHotelList(param, out count1);
            //result.currentResult.count = count1;

            //List<int> hotelIdList = new List<int>();
            //if (list1 != null && list1.Count != 0)
            //{
            //    hotelIdList = (from i in list1 select i.HotelID).ToList<int>();
            //}

            //if (hotelIdList != null && hotelIdList.Count != 0)
            //{
            //    List<ListHotelItem2> hotelList = HotelAdapter.GetCollectHotelList(hotelIdList);//hotelIdList.ConvertAll<int>(i => (int)i)             

            //    //string hotelIdListaa = "";
            //    //foreach (ListHotelItem2 i in hotelList)
            //    //{
            //    //    hotelIdListaa += i.Id + ","+i.Name+"||||";
            //    //}
            //    //Log.WriteLog("hotelIdList:" + hotelIdListaa);

            //    DateTime arrivalTime = CommMethods.GetDefaultCheckIn();
            //    DateTime departureTime = arrivalTime.AddDays(1);

            //    hotelList.ForEach(h => h.Picture = PhotoAdapter.GenHotelPicUrl(h.PicSURL, Enums.AppPhotoSize.appdetail));//拼图片链接

            //    result.currentResult.items = new List<InspectorHotelResult>();
            //    foreach (InspectorHotel es in list1)
            //    {
            //        HotelVoucherAndInspectorHotel item = new HotelVoucherAndInspectorHotel();
            //        item.BS = 2;
            //        item.InspectorHotelID = es.ID;
            //        item.HotelName = es.HotelName;
            //        item.HotelID = es.HotelID;
            //        item.Description = es.Description;
            //        item.hotelItem = hotelList.FirstOrDefault<ListHotelItem2>(i => i.Id == es.HotelID);
            //        item.IsExpired = es.IsExpired;
            //        item.IsValid = es.IsValid;
            //        item.RequiredPoint = es.RequiredPoint;
            //        voucherAndInspectorList.Add(item);
            //    }
            //} 
            #endregion
            start = start == 0 ? 1 : start;
            return voucherAndInspectorList.OrderBy(_ => _.HotelName).Skip((start - 1) * counts).Take(counts).ToList(); 
        }
        
        




        [HttpGet]
        public InspectorHotelListResult GetInspectorHotelsList(long userid, long timeStamp, int sourceID, string requestType, string sign)
        {
            //限制三个月内的品鉴酒店
            DateTime maxLimitTime = DateTime.Parse("2014-1-1");//Kevincai 取消限制  DateTime.Now.AddMonths(-3);

            InspectorHotelListResult result = new InspectorHotelListResult()
            {
                currentResult = new HotelListResult(),
                pastResult = new HotelListResult()
            };
            //已报名过的酒店
            List<InspectorRefHotel> evaluationerHotelList = new List<InspectorRefHotel>();
            if (userid != 0)
            {
                evaluationerHotelList = hotelService.GetInspectorRefHotelList(0, userid);
            }
            List<long> evaluationHotelIDList = new List<long>();
            if (evaluationerHotelList != null && evaluationerHotelList.Count != 0)
            {
                evaluationHotelIDList = (from i in evaluationerHotelList select (long)i.InspectorHotel).ToList<long>();
            }

            int count = 0;
            Dictionary<string, string> sortDic = new Dictionary<string, string>();
            sortDic.Add("Rank", "asc");//按照排名出现
            InspectorHotelSearchParam param = new InspectorHotelSearchParam()
            {
                PageIndex = 1,
                PageSize = 100,
                Filter = new InspectorHotelFilter()
                {
                    IsExpired = false,
                    IsValid = true,
                    MaxLimitTime = maxLimitTime,
                },
                Sort = sortDic
            };
            List<InspectorHotel> list1 = hotelService.GetInspectorHotelList(param, out count);
            result.currentResult.count = count;

            List<int> hotelIdList = new List<int>();
            if (list1 != null && list1.Count != 0)
            {
                hotelIdList = (from i in list1 select i.HotelID).ToList<int>();
            }

            if (hotelIdList != null && hotelIdList.Count != 0)
            {
                List<ListHotelItem2> hotelList = HotelAdapter.GetCollectHotelList(hotelIdList);//hotelIdList.ConvertAll<int>(i => (int)i)             

                DateTime arrivalTime = CommMethods.GetDefaultCheckIn();
                DateTime departureTime = arrivalTime.AddDays(1);

                hotelList.ForEach(h => h.Picture = PhotoAdapter.GenHotelPicUrl(h.PicSURL, Enums.AppPhotoSize.appdetail));//拼图片链接

                result.currentResult.items = new List<InspectorHotelResult>();
                foreach (InspectorHotel es in list1)
                {
                    InspectorHotelResult item = new InspectorHotelResult()
                    {
                        inspectorHotel = es,
                        hotelItem = hotelList.FirstOrDefault<ListHotelItem2>(i => i.Id == es.HotelID),
                        isEnrolled = evaluationHotelIDList.Contains(es.ID)
                    };
                    result.currentResult.items.Add(item);
                }
            }

            return result;
        }

        [HttpGet]
        public int IsInspector(long userid, string identityCode, long timeStamp, int sourceID, string requestType, string sign)
        {
            return AccountAdapter.IsInspectorEx(userid);
        }

        [HttpGet]
        public InspectorHotel GetInspectorHotelById(long id, long timeStamp, int sourceID, string requestType, string sign)
        {
            return hotelService.GetInspectorHotelById(id);
        }

        /// <summary>
        /// 查询房券信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public HotelVoucherEntity GetHotelVoucherById(int id)
        {
            HotelVoucherEntity hotelVoucher = hotelService.GetHotelVoucherByID(id).FirstOrDefault();
            hotelVoucher.PDayItem = PackageAdapter.GetHotelVoucherCalendar(hotelVoucher.ID, hotelVoucher.StartDate, hotelVoucher.EndDate);
            return hotelVoucher;
        }


        [HttpGet]
        public bool HasBookedInspectorHotel(long id, long userid, long timeStamp, int sourceID, string requestType, string sign)
        {
            if (id == 0 || userid == 0)
            {
                return false;
            }
            List<InspectorRefHotel> inspectorHotelList = hotelService.GetInspectorRefHotelList(id, userid);
            if (inspectorHotelList != null && inspectorHotelList.Count != 0)
            {
                InspectorRefHotel ss = inspectorHotelList.Find(i => i.InspectorHotel == id && i.Inspector == userid);
                return ss != null ? true : false;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// 是否申请过房券
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public bool HasBookedVoucherHotel(int id, long userid, long timeStamp, int sourceID, string requestType, string sign)
        {
            if (id == 0 || userid == 0)
            {
                return false;
            }
            List<InspectorRefHotel> VoucherList = hotelService.GetInspectorRefHotelListByHVID(id, userid);
            if (VoucherList != null && VoucherList.Count != 0)
            {
                InspectorRefHotel ss = VoucherList.Find(i => i.HVID == id && i.Inspector == userid);
                return ss != null ? true : false;
            }
            else
            {
                return false;
            }
        }


        [HttpGet]
        public int GetAvailablePointByUserID(long userid, int typeid, long timeStamp, int sourceID, string requestType, string sign)
        {
            return hotelService.GetAvailablePointByUserID(userid, typeid);
        }

        /// <summary>
        /// 1.没有提交过申请 2.提交过但是没有写过简历 3.提交过页写过简历
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public int IsUserHasApplyInspector(InspectorApplyData data)
        {
            Inspector inspector = AccountAdapter.GetInspector(data.UserID, null, data.MobilePhone);
            if (inspector == null || inspector.UserID == 0)
            {
                return 1;
            }
            else if (string.IsNullOrWhiteSpace(inspector.Job) &&
                string.IsNullOrWhiteSpace(inspector.JobExperience) &&
                string.IsNullOrWhiteSpace(inspector.JobSpecialty))
            {
                return 2;
            }
            else
            {
                return 3;
            }
        }

        /// <summary>
        /// 提交报名品鉴师个人数据 已经提交过的更新 第一次提交的新增
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [HttpPost]
        public ResultEntity SubmitInspectorApplyData(InspectorApplyData item)
        {
            ResultEntity result = new ResultEntity();
            User_Info info = null;
            if (!string.IsNullOrEmpty(item.MobilePhone))
            {
                info = AccountAdapter.GetOrRegistPhoneUser(item.MobilePhone,0);

                result.Success = 0;
                result.Message = "您已报名成功！";
            }
            else
            {
                result.Success = 1;
                result.Message = "手机号不能为空！";
            }

            if (info != null && info.UserId != 0)
            {
                Inspector instor = AccountAdapter.GetInspector(info.UserId, null);

                int hotelCount = hotelService.GetOrderHotelCountByUserId(info.UserId, orderStatus, isNoRepeat);//12是已确认订单 hard code
                int accumulativePoints = hotelService.GetPointsEntity(info.UserId).Sum(i => i.TotalPoint);

                if (hotelCount < minCount)
                {
                    result = new ResultEntity() { Message = string.Format("您入住酒店的次数不足{0}次", minCount), Success = 2 };
                }
                else if (accumulativePoints < 30)
                {
                    result = new ResultEntity() { Message = "您累计获得的点评奖励积分为" + accumulativePoints + "分，小于30分", Success = 3 };
                }

                if (instor != null && instor.UserID != 0)
                {
                    //2016-01-18 wwb 为了方便品鉴师重新申请 更改真实姓名和其他相关描述
                    if (true || !string.IsNullOrWhiteSpace(item.Job) ||
                            !string.IsNullOrWhiteSpace(item.JobExperience) ||
                            !string.IsNullOrWhiteSpace(item.JobSpecialty))
                    {
                        //更新品鉴师工作信息
                        Inspector ee = new Inspector()
                        {
                            UserID = instor.UserID,
                            TrueName = string.IsNullOrWhiteSpace(item.TrueName) ? instor.TrueName : item.TrueName,
                            MobilePhone = instor.MobilePhone,
                            Mail = string.IsNullOrWhiteSpace(instor.Mail) ? "" : instor.Mail,
                            CreateTime = instor.CreateTime,
                            UpdateTime = DateTime.Now,
                            State = (instor.State == 2 || instor.State == 6) ? instor.State : result.Success == 0 ? 1 : -1,
                            //instor.State,
                            CommissionRatio = instor.CommissionRatio,
                            IdentityCode = instor.IdentityCode,
                            Job = string.IsNullOrWhiteSpace(item.Job) ? instor.Job : item.Job,
                            JobExperience = instor.JobExperience,
                            JobSpecialty = instor.State.ToString(),//重新申请 需要记录当时的状态
                            SourceType = instor.SourceType == 0 ? 3 : instor.SourceType,
                            TerminalType = StringHelper.TransAppTypeHeaderToAppType(AppType)
                        };

                        try
                        {
                            //Log.WriteLog("UpdateInspector:" + info.UserId.ToString());
                            AccService.UpdateInspector(ee);//自带更新
                        }
                        catch (Exception ex)
                        {
                            Log.WriteLog(ex.Message + ex.StackTrace);
                            throw;
                        }
                    }
                }
                else
                {
                    Inspector ee = new Inspector()
                    {
                        UserID = info.UserId,
                        TrueName = item.TrueName,
                        MobilePhone = item.MobilePhone,
                        Mail = "",
                        CreateTime = DateTime.Now,
                        UpdateTime = DateTime.Now,
                        State = result.Success == 0 ? 1 : -1,
                        CommissionRatio = 1,
                        IdentityCode = "",
                        Job = item.Job,
                        JobExperience = item.JobExperience,
                        JobSpecialty = item.JobSpecialty,
                        SourceType = 3,
                        TerminalType = StringHelper.TransAppTypeHeaderToAppType(AppType)
                    };

                    try
                    {
                        //Log.WriteLog("InsertInspector:" + info.UserId.ToString());
                        AccService.InsertInspector(ee);//自带更新
                    }
                    catch (Exception ex)
                    {
                        Log.WriteLog(ex.Message + ex.StackTrace);
                        throw;
                    }
                }
            }

            return result.Success == 1 ? result : new ResultEntity() { Success = 0, Message = "报名成功!" };
        }

        /// <summary>
        /// 回收过期积分
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        [HttpGet]
        public int PointsRecycle(int type = 0)
        {
            HotelAdapter.HotelService.PointsRecycle(type);
            return 0;
        }

        [HttpGet]
        public int ConsumeUserPoints(long userID, int requiredPoints, HJD.HotelServices.Contracts.HotelServiceEnums.ConsumeUserPointsBizType typeID, long businessID, string token)
        {
            if (token == "zmjd01!")
            {
                return PointsAdapter.ConsumeUserPoints(new ConsumeUserPointsParam { userID = userID, requiredPoints = requiredPoints, typeID = typeID, businessID = businessID });
            }
            else
            {
                return 100;
            }
        }

        [HttpGet]
        public InspectorListResult GetRecommendedInspectorList([FromUri]RecommendedInspectorParam param)
        {
            int totalCount = 0;
            List<RecommendedInspectorModel> inspectorList = AccountAdapter.GetAppHomeInspectorList(
                new HJD.AccountServices.Entity.InspectorSearchParam()
                {
                    Start = param.start,
                    PageSize = param.count,
                    Filter = new HJD.AccountServices.Entity.InspectorFilter()
                    {
                        State = new List<int>() { 2, 6 }
                    }
                }, out totalCount, param.curUserID);//前N个申请的 正式 品鉴师

            return new InspectorListResult()
            {
                Items = inspectorList,
                TotalCount = totalCount,
                page3 = 3,
                page4 = 4,
                page5 = 5,
                page6 = 6,
                page7 = 7,
                page8 = 8,
                page9 = 9,
                page10 = 10
            };
        }
        
        #region 获取申请品鉴师所需选择的tag
        [HttpGet]
        public List<UserTagOption> GetUserTagOptionList()
        {
            return AccountAdapter.GetUserTagOptionList();
        }
        #endregion
    }
}