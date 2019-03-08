using HJD.AccountServices.Entity;
using HJD.Framework.Interface;
using HJD.Framework.WCF;
using HJD.HotelManagementCenter.Domain;
using HJD.HotelManagementCenter.IServices;
using HJD.CouponService.Contracts;
using HJDAPI.Common.Helpers;
using HJDAPI.Common.Security;
using HJDAPI.Controllers.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Security;
using System.Xml;
using HJD.CouponService.Contracts.Entity;
using HJD.WeixinServices.Contracts;
using HJD.HotelManagementCenter.Domain.Comm;
using HJD.HotelServices.Contracts;
using HJD.WeixinServices.Contract;
using HJDAPI.Models;
using HJD.AccessService.Contract.Model;
using HJD.AccessService.Contract.Params;
using HJD.WeixinService.Contract;
using System.Web;
using System.Configuration;

namespace HJDAPI.Controllers.Adapter
{
    public class WeiXinAdapter
    {
        public static ICacheProvider LocalCache = CacheManagerFactory.Create("DynamicCacheForType2");
     
        static HJD.HotelServices.Contracts.IHotelService hotelService = ServiceProxyFactory.Create<HJD.HotelServices.Contracts.IHotelService>("BasicHttpBinding_IHotelService");
        static ICommService commService = ServiceProxyFactory.Create<ICommService>("ICommService");
        static IWeixinService weixinService = ServiceProxyFactory.Create<IWeixinService>("IWeixinService");
        static ICouponService couponService = ServiceProxyFactory.Create<ICouponService>("ICouponService");
        
        private static Dictionary<string, string> dicResponseTemplate = new Dictionary<string, string>();
        private static Dictionary<String, String> rules = new Dictionary<string, string>();
        private static DateTime lastRuleUpdateTime = DateTime.Now.AddDays(-1);

        static readonly object _lock = new object();

        //private static string token = "";
        //private static DateTime tokenExpiresTime = DateTime.Now;

        //private static string ticket = "";
        //private static DateTime ticketExpiresTime = DateTime.Now;

        #region 微信相关活动使用变量（关键字、返回内容等）

        /// <summary>
        /// 【周末酒店】微信活动集合
        /// </summary>
        public static List<WeixinActivityEntity> weixinActivityList = new List<WeixinActivityEntity>();

        /// <summary>
        /// 【尚旅游】微信活动集合
        /// </summary>
        public static List<WeixinActivityEntity> weixinActivityList3 = new List<WeixinActivityEntity>();

        /// <summary>
        /// 【尚旅游成都】微信活动集合
        /// </summary>
        public static List<WeixinActivityEntity> weixinActivityList4 = new List<WeixinActivityEntity>();

        /// <summary>
        /// 【美味至尚】微信活动集合
        /// </summary>
        public static List<WeixinActivityEntity> weixinActivityList5 = new List<WeixinActivityEntity>();

        /// <summary>
        /// 【尚旅游北京】微信活动集合
        /// </summary>
        public static List<WeixinActivityEntity> weixinActivityList6 = new List<WeixinActivityEntity>();

        /// <summary>
        /// 【周末酒店服务号 浩颐】微信活动集合
        /// </summary>
        public static List<WeixinActivityEntity> weixinActivityList7 = new List<WeixinActivityEntity>();

        /// <summary>
        /// 【周末酒店苏州服务号】微信活动集合
        /// </summary>
        public static List<WeixinActivityEntity> weixinActivityList8 = new List<WeixinActivityEntity>();

        /// <summary>
        /// 【周末酒店成都服务号】微信活动集合
        /// </summary>
        public static List<WeixinActivityEntity> weixinActivityList9 = new List<WeixinActivityEntity>();

        /// <summary>
        /// 【周末酒店深圳服务号】微信活动集合
        /// </summary>
        public static List<WeixinActivityEntity> weixinActivityList10 = new List<WeixinActivityEntity>();

        /// <summary>
        /// 【遛娃指南服务号】微信活动集合
        /// </summary>
        public static List<WeixinActivityEntity> weixinActivityList11 = new List<WeixinActivityEntity>();

        /// <summary>
        /// 【遛娃指南南京服务号】微信活动集合
        /// </summary>
        public static List<WeixinActivityEntity> weixinActivityList13 = new List<WeixinActivityEntity>();

        /// <summary>
        /// 【遛娃指南无锡服务号】微信活动集合
        /// </summary>
        public static List<WeixinActivityEntity> weixinActivityList14 = new List<WeixinActivityEntity>();

        /// <summary>
        /// 【遛娃指南广州服务号】微信活动集合
        /// </summary>
        public static List<WeixinActivityEntity> weixinActivityList15 = new List<WeixinActivityEntity>();

        /// <summary>
        /// 【遛娃指南杭州服务号】微信活动集合
        /// </summary>
        public static List<WeixinActivityEntity> weixinActivityList16 = new List<WeixinActivityEntity>();

        /// <summary>
        /// 【遛娃指南深圳 订阅号】微信活动集合
        /// </summary>
        public static List<WeixinActivityEntity> weixinActivityList17 = new List<WeixinActivityEntity>();

        /// <summary>
        /// 【周末酒店Lite小程序】微信活动集合
        /// </summary>
        public static List<WeixinActivityEntity> weixinActivityList100 = new List<WeixinActivityEntity>();

        public static void LoadWeixinActivity()
        {
            weixinActivityList = new List<WeixinActivityEntity>();

            weixinActivityList = weixinService.GetWeixinActives();

            weixinActivityList = weixinActivityList.Where(w => w.WeixinAcountId == 1).ToList();

            #region 原来固定写死的活动对象(注释)

//            #region 猜一猜

//            weixinActivityList.Add(new WeixinActivityEntity
//            {
//                ActivityID = 19,
//                ActivityKeyWord = "猜一猜",
//                ActivityStartDateTime = DateTime.Parse("2015-06-12 00:00:00"),
//                ActivityFinishDateTime = DateTime.Parse("2015-06-15 16:00:00"),
//                ActivityNotStartWord = "",
//                ActivityFinishWord = "本期活动已结束",
//                ActivitySolutions = new List<string> { "长城脚下" },

//                TxtCanEmpty = true,
//                HasEnrollTxtMessage = "您已经参加过本次活动，不能重复参加哦",
//                EnrollTxtSuccess = @"恭喜您猜对了！ 
//1、转发此链接（http://mp.weixin.qq.com/s?__biz=MzA5NTIwODUzMQ==&mid=210067630&idx=1&sn=c17ac85c66c7a640319c594ea2776fc0#rd）至朋友圈； 
//2、截屏，回复本账号",
//                EnrollTxtAlert = "抱歉，您没有猜对哦，下次再试吧。",
                
//                HasPhotoStep = true,
//                DefaultPhotoSuccess = "截图收到！",
//                EnrollPhotoSuccess = @"截图收到！ 恭喜您获得1积分，累积满30积分，可兑换免费品鉴酒店的机会。 
//积分查询：用您的手机号码注册登录“周末酒店”APP，在“我的钱包”中查询。 
//积分积累：参与周末酒店微信互动活动，或在周末酒店APP上写点评，获得更多积分。
//点击（http://www.zmjiudian.com/inspector/hotellist）查看可供免费品鉴的酒店。"
//            });
//            #endregion

//            #region 反馈
//            weixinActivityList.Add(new WeixinActivityEntity
//            {
//                ActivityID = 18,
//                ActivityKeyWord = "反馈",
//                ActivityStartDateTime = DateTime.Parse("2015-05-30 00:00:00"),
//                ActivityFinishDateTime = DateTime.Parse("2016-06-01 16:00:00"),
//                ActivityNotStartWord = "",
//                ActivityFinishWord = "本期活动已结束",
//                ActivitySolutions = new List<string>(),
                
//                TxtCanEmpty = true,
//                HasEnrollTxtMessage = "您已经参加过本次活动，不能重复参加哦",
//                EnrollTxtSuccess = @"感谢您的反馈，我们的工作人员会仔细阅读。如您的建议被采纳，与您手机号码关联的账号会获得1积分，请到APP“我的钱包”中查询。累计满30积分，可免费品鉴酒店，点击链接（http://www.zmjiudian.com/inspector/hotellist） 查看可供免费品鉴的酒店",
//                EnrollTxtAlert = "请输入反馈内容哦。",
                
//                HasPhotoStep = false,
//                DefaultPhotoSuccess = "截图收到！",
//                EnrollPhotoSuccess = @"截图收到！"
//            });
//            #endregion

//            #region 爱的故事
//            weixinActivityList.Add(new WeixinActivityEntity
//            {
//                ActivityID = 20,
//                ActivityKeyWord = "晒恩爱",
//                ActivityStartDateTime = DateTime.Parse("2015-08-20 00:00:00"),
//                ActivityFinishDateTime = DateTime.Parse("2015-08-21 23:59:59"),
//                ActivityNotStartWord = "",
//                ActivityFinishWord = "本期活动已结束",
//                ActivitySolutions = new List<string>(),

//                TxtCanEmpty = true,
//                HasEnrollTxtMessage = "请说说你的故事，并post一张图片。",
//                EnrollTxtSuccess = "请说说你的故事，并post一张图片。",
//                EnrollTxtAlert = "请输入酒店名称哦。",

//                HasPhotoStep = false,
//                DefaultPhotoSuccess = "截图收到！",
//                EnrollPhotoSuccess = @"截图收到！ 恭喜您获得1积分，累积满30积分，可兑换免费品鉴酒店的机会。 
//积分查询：用您的手机号码注册登录“周末酒店”APP，在“我的钱包”中查询。 
//积分积累：参与周末酒店微信互动活动，或在周末酒店APP上写点评，获得更多积分。
//点击（http://www.zmjiudian.com/inspector/hotellist）查看可供免费品鉴的酒店。"
//            });
//            #endregion

//            #region 教师节
//            weixinActivityList.Add(new WeixinActivityEntity
//            {
//                ActivityID = 21,
//                ActivityKeyWord = "教师节",
//                ActivityStartDateTime = DateTime.Parse("2015-09-08 00:00:00"),
//                ActivityFinishDateTime = DateTime.Parse("2015-09-09 15:59:59"),
//                ActivityNotStartWord = "",
//                ActivityFinishWord = "本期活动已结束",
//                ActivitySolutions = new List<string>(),

//                TxtCanEmpty = true,
//                HasEnrollTxtMessage = "您已经参与成功。",
//                EnrollTxtSuccess = "感谢参与，提名已收到。",
//                EnrollTxtAlert = "请输入老师姓名+任教科目+学校名",

//                HasPhotoStep = false,
//                DefaultPhotoSuccess = "截图收到！",
//                EnrollPhotoSuccess = @"截图收到！ 恭喜您获得1积分，累积满30积分，可兑换免费品鉴酒店的机会。 
//积分查询：用您的手机号码注册登录“周末酒店”APP，在“我的钱包”中查询。 
//积分积累：参与周末酒店微信互动活动，或在周末酒店APP上写点评，获得更多积分。
//点击（http://www.zmjiudian.com/inspector/hotellist）查看可供免费品鉴的酒店。"
//            });
//            #endregion

//            #region 10.4-6月湖亲子节活动

//            weixinActivityList.Add(new WeixinActivityEntity
//            {
//                ActivityID = 22,
//                ActivityKeyWord = "现金券",
//                ActivityStartDateTime = DateTime.Parse("2015-10-04 00:00:00"),
//                ActivityFinishDateTime = DateTime.Parse("2015-10-06 23:59:59"),
//                ActivityNotStartWord = "您来早了，活动将在10月4号正式开始。",
//                ActivityFinishWord = "活动已过期，期待下次参与！",
//                ActivitySolutions = new List<string>(),

//                TxtCanEmpty = true,
//                HasEnrollTxtMessage = "您已经参与成功。",
//                EnrollTxtSuccess = "恭喜您已获得100现金券，请下载周末酒店APP，用您的手机号码注册登录，在“我的钱包”中查看。APP下载地址：http://app.zmjiudian.com/",
//                EnrollTxtAlert = "请输入现金券+手机号码",

//                HasPhotoStep = false,
//                DefaultPhotoSuccess = "",
//                EnrollPhotoSuccess = @""
//            });

//            #endregion

//            #region 偶萌

//            weixinActivityList.Add(new WeixinActivityEntity
//            {
//                ActivityID = 24,
//                ActivityKeyWord = "偶萌",
//                ActivityStartDateTime = DateTime.Parse("2015-10-04 00:00:00"),
//                ActivityFinishDateTime = DateTime.Parse("2017-10-06 23:59:59"),
//                ActivityNotStartWord = "您来早了，活动将在10月4号正式开始。",
//                ActivityFinishWord = "活动已过期，期待下次参与！",
//                ActivitySolutions = new List<string>(),

//                TxtCanEmpty = true,
//                HasEnrollTxtMessage = "此活动一个手机号只能参加一次，谢谢！",
//                EnrollTxtSuccess = "恭喜您已经获得100元现金券，请下载周末酒店APP http://app.zmjiudian.com ，用您提供的电话号码注册登录领取。",
//                EnrollTxtAlert = "",

//                HasPhotoStep = false,
//                DefaultPhotoSuccess = "",
//                EnrollPhotoSuccess = @""
//            });

//            #endregion

//            #region 10.6 免费品鉴酒店

//            weixinActivityList.Add(new WeixinActivityEntity
//            {
//                ActivityID = 23,
//                ActivityKeyWord = "品鉴酒店",
//                ActivityStartDateTime = DateTime.Parse("2015-10-06 00:00:00"),
//                ActivityFinishDateTime = DateTime.Parse("2015-11-13 23:59:59"),
//                ActivityNotStartWord = "您来早了！",
//                ActivityFinishWord = "活动已过期，期待下次参与！",
//                ActivitySolutions = new List<string>(),

//                TxtCanEmpty = true,
//                HasEnrollTxtMessage = @"欢迎参加活动！1、请打开页面（http://mp.weixin.qq.com/s?__biz=MzA5NTIwODUzMQ==&mid=400956354&idx=1&sn=c137cccc661ae384097eae37f867dfcf#rd），分享至微信朋友圈；2、截屏、回复本公众号。",
//                EnrollTxtSuccess = @"欢迎参加活动！1、请打开页面（http://mp.weixin.qq.com/s?__biz=MzA5NTIwODUzMQ==&mid=400956354&idx=1&sn=c137cccc661ae384097eae37f867dfcf#rd），分享至微信朋友圈；2、截屏、回复本公众号。",
//                EnrollTxtAlert = @"欢迎参加活动！1、请打开页面（http://mp.weixin.qq.com/s?__biz=MzA5NTIwODUzMQ==&mid=400956354&idx=1&sn=c137cccc661ae384097eae37f867dfcf#rd），分享至微信朋友圈；2、截屏、回复本公众号。",

//                HasPhotoStep = true,
//                DefaultPhotoSuccess = "截图收到！请点击 http://www.zmjiudian.com/inspector/register 报名成为品鉴师。",
//                EnrollPhotoSuccess = @"截图收到！请点击 http://www.zmjiudian.com/inspector/register 报名成为品鉴师。"
//            });

//            #endregion

//            #region 观影活动

//            weixinActivityList.Add(new WeixinActivityEntity
//            {
//                ActivityID = 25,
//                ActivityKeyWord = "观影",
//                ActivityStartDateTime = DateTime.Parse("2015-11-07 00:00:00"),
//                ActivityFinishDateTime = DateTime.Parse("2015-11-11 23:59:59"),
//                ActivityNotStartWord = "您来早了！",
//                ActivityFinishWord = "活动已过期，期待下次参与！",
//                ActivitySolutions = new List<string>(),

//                TxtCanEmpty = true,
//                HasEnrollTxtMessage = @"报名收到！我们将在活动截止日后公布获选名单，敬请期待。",
//                EnrollTxtSuccess = @"报名收到！我们将在活动截止日后公布获选名单，敬请期待。",
//                EnrollTxtAlert = @"请输入观影+姓名+手机号+出生年月日进行报名",

//                HasPhotoStep = false,
//                DefaultPhotoSuccess = "截图收到！",
//                EnrollPhotoSuccess = @"截图收到！"
//            });

//            #endregion

            #endregion
        }

        public static void LoadWeixinActivity3()
        {
            weixinActivityList3 = new List<WeixinActivityEntity>();

            weixinActivityList3 = weixinService.GetWeixinActives();

            weixinActivityList3 = weixinActivityList3.Where(w => w.WeixinAcountId == 3).ToList();
        }

        public static void LoadWeixinActivity4()
        {
            weixinActivityList4 = new List<WeixinActivityEntity>();
                              
            weixinActivityList4 = weixinService.GetWeixinActives();
                              
            weixinActivityList4 = weixinActivityList4.Where(w => w.WeixinAcountId == 4).ToList();
        }

        public static void LoadWeixinActivity5()
        {
            weixinActivityList5 = new List<WeixinActivityEntity>();

            weixinActivityList5 = weixinService.GetWeixinActives();

            weixinActivityList5 = weixinActivityList5.Where(w => w.WeixinAcountId == 5).ToList();
        }

        public static void LoadWeixinActivity6()
        {
            weixinActivityList6 = new List<WeixinActivityEntity>();

            weixinActivityList6 = weixinService.GetWeixinActives();

            weixinActivityList6 = weixinActivityList6.Where(w => w.WeixinAcountId == 6).ToList();
        }

        /// <summary>
        /// 周末酒店服务号 浩颐
        /// </summary>
        public static void LoadWeixinActivity7()
        {
            weixinActivityList7 = new List<WeixinActivityEntity>();

            weixinActivityList7 = weixinService.GetWeixinActives();

            weixinActivityList7 = weixinActivityList7.Where(w => w.WeixinAcountId == 7).ToList();
        }

        /// <summary>
        /// 周末酒店苏州服务号
        /// </summary>
        public static void LoadWeixinActivity8()
        {
            weixinActivityList8 = new List<WeixinActivityEntity>();

            weixinActivityList8 = weixinService.GetWeixinActives();

            weixinActivityList8 = weixinActivityList8.Where(w => w.WeixinAcountId == 8).ToList();
        }

        /// <summary>
        /// 周末酒店成都服务号
        /// </summary>
        public static void LoadWeixinActivity9()
        {
            weixinActivityList9 = new List<WeixinActivityEntity>();

            weixinActivityList9 = weixinService.GetWeixinActives();

            weixinActivityList9 = weixinActivityList9.Where(w => w.WeixinAcountId == 9).ToList();
        }

        /// <summary>
        /// 周末酒店深圳服务号
        /// </summary>
        public static void LoadWeixinActivity10()
        {
            weixinActivityList10 = new List<WeixinActivityEntity>();

            weixinActivityList10 = weixinService.GetWeixinActives();

            weixinActivityList10 = weixinActivityList10.Where(w => w.WeixinAcountId == 10).ToList();
        }

        /// <summary>
        /// 遛娃指南服务号
        /// </summary>
        public static void LoadWeixinActivity11()
        {
            weixinActivityList11 = new List<WeixinActivityEntity>();

            weixinActivityList11 = weixinService.GetWeixinActives();

            weixinActivityList11 = weixinActivityList11.Where(w => w.WeixinAcountId == 11).ToList();
        }

        /// <summary>
        /// 遛娃指南南京服务号
        /// </summary>
        public static void LoadWeixinActivity13()
        {
            weixinActivityList13 = new List<WeixinActivityEntity>();

            weixinActivityList13 = weixinService.GetWeixinActives();

            weixinActivityList13 = weixinActivityList13.Where(w => w.WeixinAcountId == 13).ToList();
        }

        /// <summary>
        /// 遛娃指南无锡服务号
        /// </summary>
        public static void LoadWeixinActivity14()
        {
            weixinActivityList14 = new List<WeixinActivityEntity>();

            weixinActivityList14 = weixinService.GetWeixinActives();

            weixinActivityList14 = weixinActivityList14.Where(w => w.WeixinAcountId == 14).ToList();
        }

        /// <summary>
        /// 遛娃指南广州服务号
        /// </summary>
        public static void LoadWeixinActivity15()
        {
            weixinActivityList15 = new List<WeixinActivityEntity>();

            weixinActivityList15 = weixinService.GetWeixinActives();

            weixinActivityList15 = weixinActivityList15.Where(w => w.WeixinAcountId == 15).ToList();
        }

        /// <summary>
        /// 遛娃指南杭州服务号
        /// </summary>
        public static void LoadWeixinActivity16()
        {
            weixinActivityList16 = new List<WeixinActivityEntity>();

            weixinActivityList16 = weixinService.GetWeixinActives();

            weixinActivityList16 = weixinActivityList16.Where(w => w.WeixinAcountId == 16).ToList();
        }

        /// <summary>
        /// 遛娃指南深圳 订阅号
        /// </summary>
        public static void LoadWeixinActivity17()
        {
            weixinActivityList17 = new List<WeixinActivityEntity>();

            weixinActivityList17 = weixinService.GetWeixinActives();

            weixinActivityList17 = weixinActivityList17.Where(w => w.WeixinAcountId == 17).ToList();
        }

        public static void LoadWeixinActivity100()
        {
            weixinActivityList100 = new List<WeixinActivityEntity>();

            weixinActivityList100 = weixinService.GetWeixinActives();

            weixinActivityList100 = weixinActivityList100.Where(w => w.WeixinAcountId == 100).ToList();
        }

        #endregion

        #region WeixinWebservice、WeixinController 返回结果中需要回调的事件

        public static string RequestRebate(RequestEntity request)
        {
            int rebateState = OrderAdapter.RequestOrderRebate(request.OrderID);
            string Content = "你好！" + rebateState.ToString();
            switch (rebateState)
            {
                case 0:
                    Content = "输入的订单号" + request.OrderID.ToString() + "不存在，请重新输入。";
                    break;
                case 1:
                    Content = "申请提交成功，您会在1-5个工作日内收到返现。返现金额将直接退至您的支付帐户中，请注意查收，谢谢！";
                    break;
                case 2:
                    Content = "该订单号已申请过，无需重复申请，谢谢！";
                    break;
                case 3:
                    Content = "您的订单已返现，请查收。";
                    break;
                case 4:
                    Content = "您的订单尚末付款，请付款后再申请返现，谢谢！";
                    break;
                case 5:
                    Content = "您好，你输入的订单没有返现，谢谢！";
                    break;
                case 6:
                    Content = "您好，您的订单尚末完成酒店确认，请确认后再申请返现，谢谢！";
                    break;
            }


            Content += "如果需要其它帮助，请输入#服务#，我们会及时回复您。";
            return Content;
        }

        public static string LogWeiXinInfo(RequestEntity request)
        {
            if (request.Content.StartsWith("zeta"))
            {
                string phone = request.Content.ToLower().Replace("zeta", "").Replace(" ", "");
                if (CommMethods.IsPhone(phone))
                {
                    User_Info ui = AccountAdapter.GetOrRegistPhoneUser(phone, 0);

                    CouponTypeDefineEntity ctd = couponService.GetCouponTypeDefineByCode(CouponActivityCode.zeta);


                    if (ctd.EndTime < DateTime.Now)
                    {
                        return "活动已结束，谢谢参与。";
                    }
                    else
                    {
                        List<OriginCoupon> lo = couponService.GetUserOrgCouponInfoByType(ui.UserId, ctd.Type);
                        if (lo.Count > 0)
                        {
                            return "此活动一个手机号只能参加一次，谢谢！";
                        }
                        else
                        {
                            OriginCouponResult ocr = couponService.GenerateOriginCoupon2(ui.UserId, ctd.Type, phone);
                        }
                    }

                }
                else
                {
                    return "手机号不正确，请确认后重新录入。";
                }
            }
            //else if (request.Content.StartsWith("偶萌"))
            //{
            //    string pattern = @"\D";
            //    Regex reg = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Multiline);   // MatchEvaluator myEvaluator = new MatchEvaluator(Match.;
            //    string phone = reg.Replace(request.Content.ToLower(), "");

            //   // string phone = request.Content.ToLower().Replace("现金券", "").Replace(" ", "");
            //    if (CommMethods.IsPhone(phone))
            //    {
            //        User_Info ui = AccountAdapter.GetOrRegistPhoneUser(phone);

            //        CouponTypeDefineEntity ctd = couponService.GetCouponTypeDefineByCode(CouponActivityCode.oumeng);


            //        if (ctd.EndTime < DateTime.Now)
            //        {
            //            return "活动已过期，期待下次参与！";
            //        }
            //        else
            //        {
            //            List<OriginCoupon> lo = couponService.GetUserOrgCouponInfoByType(ui.UserId, ctd.Type);
            //            if (lo.Count > 0)
            //            {
            //                return "此活动一个手机号只能参加一次，谢谢！";
            //            }
            //            else
            //            {
            //                OriginCouponResult ocr = couponService.GenerateOriginCoupon2(ui.UserId, ctd.Type, phone);

            //                return "恭喜您已经获得100元现金券，请下载周末酒店APP http://app.zmjiuidan.com ，用您提供的电话号码注册登录领取。";
            //            }
            //        }

            //    }
            //    else
            //    {
            //        return "手机号不正确，请确认后重新录入。";
            //    }
            //}
            return commService.LogWeiXinInfo(request.FromUserName, request.Content);
        }

        /// <summary>
        /// 一般处理微信活动中的文字信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static string AddActiveLuckyDraw(RequestEntity request)
        {
            //如果是 投票活动，则做投票活动的特殊处理
            if (request.EnrollActivity.Type == 4)
            {
                var luckyDrawInfo = commService.GetLuckyDrawUserInfo(request.EnrollActivity.ActivityID, request.FromUserName);
                if (luckyDrawInfo != null && luckyDrawInfo.ID > 0)
                {
                    //已经投过票了
                    return request.EnrollActivity.HasEnrollTxtMessage;
                }
                else
                { 
                    //第一次投票
                    return WeixinVoteActiveAction(request);
                }
            }
            else 
            {
                #region 指定活动需要首先验证手机号

                string phone = request.OrderID.ToString();

                //偶萌/现金券
                if (request.Content.StartsWith("偶萌") || request.Content.StartsWith("现金券") || request.Content.StartsWith("1933") || request.Content.StartsWith("蒲蒲兰") || request.Content.StartsWith("猜一猜"))
                {
                    if (!CommMethods.IsPhone(phone))
                    {
                        return "手机号不正确，请确认后重新录入。";
                    }
                }

                #endregion

                string hotelname = request.Content.Replace(request.EnrollActivity.ActivityKeyWord, "").Replace(request.OrderID.ToString(), "").Replace(" ", "");
                var back = commService.AddActiveLuckyDraw(request.EnrollActivity.ActivityID, request.FromUserName, request.OrderID.ToString(), hotelname, request.Content);
                if (back == "sign")
                {
                    #region 偶萌活动 的回复后操作

                    if (request.Content.StartsWith("偶萌"))
                    {
                        User_Info ui = AccountAdapter.GetOrRegistPhoneUser(phone, 0);

                        CouponTypeDefineEntity ctd = couponService.GetCouponTypeDefineByCode(CouponActivityCode.oumeng);

                        List<OriginCoupon> lo = couponService.GetUserOrgCouponInfoByType(ui.UserId, ctd.Type);
                        if (lo.Count > 0)
                        {
                            return "此活动一个手机号只能参加一次，谢谢！";
                        }
                        else
                        {
                            OriginCouponResult ocr = couponService.GenerateOriginCoupon2(ui.UserId, ctd.Type, phone);
                        }
                    }

                    #endregion

                    #region 现金券活动 的回复后操作

                    else if (request.Content.StartsWith("现金券"))
                    {
                        User_Info ui = AccountAdapter.GetOrRegistPhoneUser(phone, 0);

                        CouponTypeDefineEntity ctd = couponService.GetCouponTypeDefineByCode(CouponActivityCode.cashcoupon);

                        List<OriginCoupon> lo = couponService.GetUserOrgCouponInfoByType(ui.UserId, ctd.Type);
                        if (lo.Count > 0)
                        {
                            return "此活动一个手机号只能参加一次，谢谢！";
                        }
                        else
                        {
                            OriginCouponResult ocr = couponService.GenerateOriginCoupon2(ui.UserId, ctd.Type, phone);
                        }
                    }

                    #endregion

                    #region 1933活动 的回复后操作

                    else if (request.Content.StartsWith("1933"))
                    {
                        User_Info ui = AccountAdapter.GetOrRegistPhoneUser(phone, 0);

                        CouponTypeDefineEntity ctd = couponService.GetCouponTypeDefineByCode(CouponActivityCode.cashcoupon);

                        List<OriginCoupon> lo = couponService.GetUserOrgCouponInfoByType(ui.UserId, ctd.Type);
                        if (lo.Count > 0)
                        {
                            return "此活动一个手机号只能参加一次，谢谢！";
                        }
                        else
                        {
                            OriginCouponResult ocr = couponService.GenerateOriginCoupon2(ui.UserId, ctd.Type, phone);
                        }
                    }

                    #endregion

                    #region 蒲蒲兰活动 的回复后操作

                    else if (request.Content.StartsWith("蒲蒲兰"))
                    {
                        User_Info ui = AccountAdapter.GetOrRegistPhoneUser(phone, 0);

                        CouponTypeDefineEntity ctd = couponService.GetCouponTypeDefineByCode(CouponActivityCode.cashcoupon);

                        List<OriginCoupon> lo = couponService.GetUserOrgCouponInfoByType(ui.UserId, ctd.Type);
                        if (lo.Count > 0)
                        {
                            return "此活动一个手机号只能参加一次，谢谢！";
                        }
                        else
                        {
                            OriginCouponResult ocr = couponService.GenerateOriginCoupon2(ui.UserId, ctd.Type, phone);
                        }
                    }

                    #endregion

                    #region 12.12协和抽奖活动 的回复后操作

                    else if (request.Content.ToLower().Contains("suis") || request.Content.Contains("协和"))
                    {
                        var addUserInfo = commService.GetLuckyDrawUserInfo(request.EnrollActivity.ActivityID, request.FromUserName);
                        if (addUserInfo != null)
                        {
                            return string.Format(request.EnrollActivity.EnrollTxtSuccess, addUserInfo.ID);
                        }
                    }

                    #endregion

                    #region 猜一猜 活动追加答案

                    if (request.Content.ToLower().Contains("猜一猜"))
                    {
                        request.EnrollActivity.ActivitySolutions = new List<string>();
                        request.EnrollActivity.ActivitySolutions.Add("太阳湾柏悦");
                    }

                    #endregion

                    if (request.EnrollActivity.TxtCanEmpty || !string.IsNullOrEmpty(hotelname))
                    {
                        //判断是否有活动答案，有则需要判断是否包含（比如猜一猜活动，需要猜对活动答案是哪一家酒店）
                        if (request.EnrollActivity.ActivitySolutions != null && request.EnrollActivity.ActivitySolutions.Count > 0)
                        {
                            //只要包含其中一个答案即可
                            if (request.EnrollActivity.ActivitySolutions.Exists(solution => hotelname.Contains(solution)))
                            {
                                return request.EnrollActivity.EnrollTxtSuccess;
                            }
                        }
                        else
                        {
                            return request.EnrollActivity.EnrollTxtSuccess;
                        }
                    }

                    //Fail
                    return request.EnrollActivity.EnrollTxtAlert;
                }
                else
                {
                    return request.EnrollActivity.HasEnrollTxtMessage;
                }
            }
        }

        /// <summary>
        /// 一般处理微信活动中的图片信息
        /// </summary>
        /// <param name="activeCode"></param>
        /// <param name="WXAccount"></param>
        /// <param name="PicUrl"></param>
        /// <returns></returns>
        public static string AddActiveLuckyDrawSharePhoto(WeixinActivityEntity activity, string WXAccount, string PicUrl)
        {
            var response = commService.AddActiveLuckyDrawSharePhoto(activity.ActivityID, WXAccount, PicUrl);
            switch (response.Success)
            {
                case 1:
                    {
                        //参加成功，则给当前手机号用户增加1积分奖励
                        if (response.ActvieLuckyDraw != null)
                        {
                            try
                            {
                               // Log.WriteLog(string.Format("AddActiveLuckyDrawSharePhoto GenPoint: {0} {1} {2}",
                                    //activity.ActivityID, response.ActvieLuckyDraw.Phone, 5));

                                //PointsEntity pe = new PointsEntity { BusinessID = activity.ActivityID, PhoneNo = response.ActvieLuckyDraw.Phone, TypeID = 5 };
                                //var add = new CouponController().GenPoint(pe);
                            }
                            catch (Exception ex)
                            {
                                Log.WriteLog(string.Format("GenPoint Error: {0}", ex.Message));
                            }
                        }

                        return activity.EnrollPhotoSuccess;
                    }
                case 2:break;
                default: break;
            }

            return activity.DefaultPhotoSuccess;
        }

        public static int GetLuckyDrawVoteTotalCount(int activeCode)
        {
            return commService.GetLuckyDrawVoteTotalCount(activeCode);
        }

        /// <summary>
        /// 微信投票活动的处理方法
        /// </summary>
        /// <param name="requestEntity">当前请求对象（包含当前活动对象和请求内容）</param>
        /// <returns></returns>
        public static string WeixinVoteActiveAction(RequestEntity requestEntity) 
        {
            //默认回复内容
            var responseText = "投票失败，请确认投票的正确方式哦：）";  //requestEntity.EnrollActivity.EnrollTxtSuccess;

            //当前活动ID
            var activeId = requestEntity.EnrollActivity.ActivityID;

            //解析出用户投票的ID（例：宝贝投票+1，其中“宝贝投票+”为活动关键字，需要将“宝贝投票+”去除，剩余的“1”则为投票ID）
            var voteId = 0;

            //当前活动关键字
            var activeKeyword = requestEntity.EnrollActivity.ActivityKeyWord;

            //用户回复内容
            var callInfo = requestEntity.Content;

            //替换关键字获取投票ID
            if (callInfo.ToLower().Contains(activeKeyword.ToLower()))
            {
                try
                {
                    voteId = Convert.ToInt32(Regex.Replace(callInfo.ToLower(), activeKeyword.ToLower(), "").Trim());
                    if (voteId > 0)
                    {
                        //获取当前活动的所有参与者
                        var activeRuleExs = weixinService.GetActiveRuleExsByActiveId(activeId);
                        if (activeRuleExs != null && activeRuleExs.Count > 0 && activeRuleExs.Exists(_ => _.HotelId == voteId))
                        {
                            //当前被投票者信息
                            var voteEntity = activeRuleExs.Find(_ => _.HotelId == voteId);

                            //更新当前投票ID
                            var update = weixinService.AddUpActiveRuleExOfferCountById(voteEntity.ID);

                            //记录投票记录
                            string hotelname = requestEntity.Content.Replace(requestEntity.EnrollActivity.ActivityKeyWord, "").Replace(requestEntity.OrderID.ToString(), "").Replace(" ", "");
                            var addLuckyDraw = commService.AddActiveLuckyDraw(requestEntity.EnrollActivity.ActivityID, requestEntity.FromUserName, requestEntity.OrderID.ToString(), hotelname, requestEntity.Content);

                            //返回投票成功提示及查看投票结果页链接
                            responseText = string.Format("你已成功为{2}号投票，感谢参与。<a href='http://www.shang-ke.cn/wx/active/voteresult/{0}/{1}'>点击查看投票结果>></a>", activeId, voteId, voteId);
                        }
                        else
                        {
                            //返回投票失败提示（ID有误）
                            responseText = "投票号码查询失败，请检查";
                        }
                    }
                    else
                    {
                        responseText = "无效的投票号码，请检查";
                    }

                }
                catch (Exception ex)
                {
                    responseText = "投票失败，请联系管理员";
                }
            }

            return responseText;
        }

        #endregion

        #region Weixin JsApi 相关

        /// <summary>
        /// 获取Weixin Api的调用配置
        /// </summary>
        /// <param name="config2"></param>
        /// <param name="weixinAcountId">1周末酒店 3尚旅游 4尚旅游成都 5美味至尚</param>
        /// <returns></returns>
        public static WeixinConfig GetWeixinApiConfig(WeixinConfig config2, int weixinAcountId)
        {
            WeixinConfig config = new WeixinConfig();

            lock (_lock)
            {
                config.appId = Configs.WeiXinAPPID;
                switch (weixinAcountId)
                {
                    case 1: { config.appId = Configs.WeiXinAPPID; break; }
                    case 3: { config.appId = Configs.WeiXinAPPID; break; }
                    case 4: { config.appId = Configs.WeiXinAPPIDShanglvcd; break; }
                    case 5: { config.appId = Configs.WeiXinAPPIDMeiweizhishang; break; }
                }

                //根据是否生产环境调配
                config.debug = config2.debug;

                DateTime baseTime = Convert.ToDateTime("1970-1-1 00:00:00");
                TimeSpan ts = DateTime.Now - baseTime;
                long intervel = (long)ts.TotalSeconds;
                config.timestamp = intervel;

                //生成随机字符串 8到30位的字母数字组合
                config.nonceStr = DescriptionHelper.GenerateRandomStr();

                //计算出的签名
                config.signature = SignatureWeixinJSSDK(config.nonceStr, config.timestamp, config2.url);

                config.jsApiList = config2.jsApiList;// "onMenuShareTimeline,chooseWXPay";//ToDo wwb需要注册使用的js API列表 逗号相隔 挪到配置文件中？    
            }

            return config;
        }

        #endregion

        public static string SentTextMsgToWeiXinUser(string weixincode, string msg, string ToUser)
        {
            string ACCESS_TOKEN = weixinService.GetWeixinTokenByStrCode(weixincode, false);

            string result = SentTextMsgToWeiXinUserWithTOKEN(ACCESS_TOKEN, msg, ToUser);

            if (result.IndexOf("40001") > 0) //token不正确
            {
                ACCESS_TOKEN = weixinService.GetWeixinTokenByStrCode(weixincode, true);

                result = SentTextMsgToWeiXinUserWithTOKEN(ACCESS_TOKEN, msg, ToUser);
            }
         //   Log.WriteMagiCallLog("W", string.Format("{0} {1} {2} {3}",weixincode, msg,ToUser, result));
            return result;
        }

        private static string SentTextMsgToWeiXinUserWithTOKEN(string ACCESS_TOKEN, string msg, string ToUser)
        {
            string url = "https://api.weixin.qq.com/cgi-bin/message/custom/send?access_token=" + ACCESS_TOKEN;
            //发送文本消息
            string json = string.Format("{{\"touser\":\"{0}\",\"msgtype\":\"text\",\"text\":{{\"content\":\"{1}\"}}", ToUser, msg);

            string result = HttpHelper.HttpPostForJson(url, json);

            //Log.WriteLog(string.Format("SentTextMsgToWeiXinUser:{0} {1} {2}", url, json, result));

            return result;
        }

        #region 服务号二维码&模板消息等接口处理

        /// <summary>
        /// 创建指定公众号指定参数的二维码配置
        /// </summary>
        /// <param name="weixinAcount"></param>
        /// <param name="actionName"></param>
        /// <param name="sceneKey"></param>
        /// <returns></returns>
        public static string CreateAccountQrcode(int weixinAcount, int expires, string actionName, int sceneId = 0, string sceneStr = "")
        {
            string ACCESS_TOKEN = weixinService.GetWeixinTokenByCode((WeiXinChannelCode)weixinAcount);

            string result = CreateQrcode(ACCESS_TOKEN, expires, actionName, sceneId, sceneStr);

            if (result.IndexOf("40001") > 0) //token不正确
            {
                ACCESS_TOKEN = weixinService.GetWeixinTokenByCode((WeiXinChannelCode)weixinAcount);

                result = CreateQrcode(ACCESS_TOKEN, expires, actionName, sceneId, sceneStr);

                if (result.IndexOf("40001") > 0) //token不正确
                {
                    ACCESS_TOKEN = weixinService.GetWeixinTokenByCode((WeiXinChannelCode)weixinAcount);

                    result = CreateQrcode(ACCESS_TOKEN, expires, actionName, sceneId, sceneStr);
                }
            }

            //返回结果，如：
            //{"ticket":"gQEt8DwAAAAAAAAAAS5odHRwOi8vd2VpeGluLnFxLmNvbS9xLzAyRm90cGdzc0lmQjMxMDAwMHcwN2wAAgREV8NZAwQAAAAA","url":"http:\/\/weixin.qq.com\/q\/02FotpgssIfB310000w07l"}

            //然后通过showqrcode请求生成二维码图片，如：
            //https://mp.weixin.qq.com/cgi-bin/showqrcode?ticket=gQEt8DwAAAAAAAAAAS5odHRwOi8vd2VpeGluLnFxLmNvbS9xLzAyRm90cGdzc0lmQjMxMDAwMHcwN2wAAgREV8NZAwQAAAAA

            //Log.WriteMagiCallLog("W", string.Format("{0} {1} {2} {3}",weixincode, msg,ToUser, result));

            return result;
        }

        public static string CreateQrcode(string token, int expires, string actionName, int sceneId = 0, string sceneStr = "")
        {
            string url = "https://api.weixin.qq.com/cgi-bin/qrcode/create?access_token=" + token;

            string json_id = string.Format("{{\"expire_seconds\":{0},\"action_name\":\"QR_SCENE\",\"action_info\":{{\"scene\":{{\"scene_id\":{1}}}}}}}", expires, sceneId);
            string json_str = string.Format("{{\"expire_seconds\":{0},\"action_name\":\"QR_STR_SCENE\",\"action_info\":{{\"scene\":{{\"scene_str\":\"{1}\"}}}}}}", expires, sceneStr);
            string json_limit_str = string.Format("{{\"expire_seconds\":{0},\"action_name\":\"QR_LIMIT_STR_SCENE\",\"action_info\":{{\"scene\":{{\"scene_str\":\"{1}\"}}}}}}", expires, sceneStr);

            string result = "";
            if (actionName == "QR_SCENE")
	        {
		        result = HttpHelper.HttpPostForJson(url, json_id);
	        }
            else if (actionName == "QR_STR_SCENE")
            {
                result = HttpHelper.HttpPostForJson(url, json_str);
            }
            else
            {
                result = HttpHelper.HttpPostForJson(url, json_limit_str);
            }

            //Log.WriteLog(string.Format("SentTextMsgToWeiXinUser:{0} {1} {2}", url, json, result));

            return result;
        }

        /// <summary>
        /// 微信发送模板信息
        /// </summary>
        /// <param name="weixinAcount"></param>
        /// <param name="openid"></param>
        /// <param name="tempid"></param>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string SendTemplateMessage(int weixinAcount, string openid, string tempid, string url, string first, string remark, List<string> list)
        {
            var _msgEntity = new WeixinTemplateMessageEntity
            {
                WeixinAccount = (WeiXinChannelCode)weixinAcount,
                ToOpenId = openid,
                TemplateId = tempid,
                TemplateUrl = url,
                MiniProgram = "",
                DataFirst = first,
                DataRemark = remark,
                DataKeywords = list
            }; 

            return SendTemplateMessage(_msgEntity);
        }
        public static string SendTemplateMessage(WeiXinChannelCode weixinAcount, string openid, string tempid, string url, string first, string remark, List<string> list)
        {
            var _msgEntity = new WeixinTemplateMessageEntity
            {
                WeixinAccount = weixinAcount,
                ToOpenId = openid,
                TemplateId = tempid,
                TemplateUrl = url,
                MiniProgram = "",
                DataFirst = first,
                DataRemark = remark,
                DataKeywords = list
            };

            return SendTemplateMessage(_msgEntity);
        }
        public static string SendTemplateMessage(WeixinTemplateMessageEntity tempMsgEntity)
        {
            string ACCESS_TOKEN = "VzCczBV7QlvpPJ_0bf1kx2MWu_4JbNCBXw_qlCa_MZL_SdzlXCMZFV_TgNbkO-HfEZWP71xsMOHnAwWRvR8cIIVkPmTXTDUlPTt8xFmxMCtbwFBd1LEYM18jwmE2UUSvDGKjAHAGJN";
            ACCESS_TOKEN = weixinService.GetWeixinTokenByCode(tempMsgEntity.WeixinAccount);

            string api_url = "https://api.weixin.qq.com/cgi-bin/message/template/send?access_token=" + ACCESS_TOKEN;

            string json_parameter = "{ \"touser\":\"[OPENID]\", \"template_id\":\"[TEMPID]\", \"url\":\"[URL]\", \"miniprogram\":{}, \"data\":[DATA] }";

            json_parameter = json_parameter.Replace("[OPENID]", tempMsgEntity.ToOpenId);
            json_parameter = json_parameter.Replace("[TEMPID]", tempMsgEntity.TemplateId);
            json_parameter = json_parameter.Replace("[URL]", tempMsgEntity.TemplateUrl);
            json_parameter = json_parameter.Replace("[DATA]", GetDataByTemplateId(tempMsgEntity.TemplateId, tempMsgEntity.DataFirst, tempMsgEntity.DataRemark, tempMsgEntity.DataKeywords));

            string result = HttpHelper.HttpPostForJson(api_url, json_parameter);

            //Log.WriteLog(string.Format("SendTemplateMessage:{0} {1} {2}", url, json, result));

            return result;
        }

        /// <summary>
        /// 根据模板ID获取模板消息数据
        /// </summary>
        /// <param name="tempId"></param>
        /// <param name="first"></param>
        /// <param name="remark"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public static string GetDataByTemplateId(string tempId, string first, string remark, List<string> list)
        {
            var dataStr = "{}";

            switch (tempId)
            {
                //团购订单状态提醒
                case "8Ctx7mex5dalgAE_cu0GC12v2fwPs8Szx2vn_1Ri2oM": //周末酒店服务号浩颐下的id
                case "Vjtz_ENJc8bJIkdVz3tGLCcjgXaxFjsyODkGwEo-bPc": //遛娃指南苏州服务号下的id
                case "1tY-ArY11_vaNMGQ_P1z0cATZBv_7rkSV6ngaUIwLDY": //遛娃指南成都服务号下的id
                case "leGThrNm9C1j_RsgaoVqwso1hyD7utCyRaTCkcNmmKE": //遛娃指南深圳服务号下的id
                case "M_ZSUolM0yuKdZKHYakAO8J_KkEcQPqx58fv_hooSgw": //遛娃指南服务号下的id
                case "WYbP8jXvU-wh1LsQETtRgf2oJ99Vp1hpq99F8L6_rQk": //遛娃指南南京服务号下的id
                //case "KmQWuI4mkf2gUuGR79PnWD4v2sBcVHlcpVMHxr30ap4": //遛娃指南无锡服务号下的id（deleted 2018.09.03 haoy）
                case "kxA6PDXdHY-JZX9s643sKZcEeU6_tKPJU7Y5Pd6LSYQ": //遛娃指南无锡服务号下的id（new 2018.09.03 haoy）
                case "kLAbgRrvtQpkNmmd0V94MoatJUA-GXBU1VUgkSuHoV4": //遛娃指南广州服务号下的id
                case "AFB7Q8HvXIUaDpWntXkAXxns8U2-3HCSqAmEL2SjLT0": //遛娃指南杭州服务号下的id
                    {
                        dataStr = "{ \"first\": { \"value\":\"[FIRST]\", \"color\":\"#fe8000\"},";
                        dataStr += "\"HotelName\":{ \"value\":\"[K1]\", \"color\":\"#555555\" },";
                        dataStr += "\"ProductName\":{ \"value\":\"[K2]\", \"color\":\"#555555\" },";
                        dataStr += "\"remark\":{ \"value\":\"[REMARK]\", \"color\":\"#3E9EC0\" } }";
                        break;
                    }
                //待办事项状态变更通知【新 20180802 haoy】
                case "1Et5PC_DqlQyGbYx7ERsqcl-f8ArPOogLfg3WgGl0p0": //周末酒店服务号浩颐下的id
                case "Se17X3ma49wa4T7gpD9liN2eo0d5fHlea57xVWv0zv8": //遛娃指南服务号下的id
                case "kHqp6Xyp7ctAs4lYB5gz2LLES8f0ZfyOLC3jWgV-TPc": //遛娃指南无锡服务号下的id
                case "A4Uq0IkjnEUjMHCOByXWytYP9bjoQpis3FAEIW5dST8": //尚旅周末服务号的id
                    {
                        dataStr = "{ \"first\": { \"value\":\"[FIRST]\", \"color\":\"#fe8000\"},";
                        dataStr += "\"keyword1\":{ \"value\":\"[K1]\", \"color\":\"#555555\" },";
                        dataStr += "\"keyword2\":{ \"value\":\"[K2]\", \"color\":\"#555555\" },";
                        dataStr += "\"keyword3\":{ \"value\":\"[K3]\", \"color\":\"#555555\" },";
                        dataStr += "\"keyword4\":{ \"value\":\"[K4]\", \"color\":\"#555555\" },";
                        dataStr += "\"remark\":{ \"value\":\"[REMARK]\", \"color\":\"#3E9EC0\" } }";
                        break;
                    }
                //查询结果通知【20181228 haoy】
                case "2oKigs7BZY8BSMNntmEx1OaKTJAvqnNBDLAujKbAyVg": //周末酒店服务号浩颐下的id
                    {
                        dataStr = "{ \"first\": { \"value\":\"[FIRST]\", \"color\":\"#fe8000\"},";
                        dataStr += "\"keyword1\":{ \"value\":\"[K1]\", \"color\":\"#555555\" },";
                        dataStr += "\"keyword2\":{ \"value\":\"[K2]\", \"color\":\"#555555\" },";
                        dataStr += "\"keyword3\":{ \"value\":\"[K3]\", \"color\":\"#555555\" },";
                        dataStr += "\"remark\":{ \"value\":\"[REMARK]\", \"color\":\"#3E9EC0\" } }";
                        break;
                    }
                //待办事项提醒【旧】
                case "-1gNmtaQN_fWIPzlArhUpSCleXb28cC8-obqP4-80wM": //周末酒店服务号浩颐下的id
                case "GfDrV68IpStyasY5UidIMDzUCe4U4mValpZIFiUQ-Sw": //周末酒店苏州服务号下的id
                    {
                        dataStr = "{ \"first\": { \"value\":\"[FIRST]\", \"color\":\"#3E9EC0\"},";
                        dataStr += "\"keyword1\":{ \"value\":\"[K1]\", \"color\":\"#000000\" },";
                        dataStr += "\"keyword2\":{ \"value\":\"[K2]\", \"color\":\"#000000\" },";
                        dataStr += "\"remark\":{ \"value\":\"[REMARK]\", \"color\":\"#fe8000\" } }";
                        break;
                    }
                //资料完善提醒
                case "R2aaurSTbP0OOSXd-qKzLH1x4PWhyEeBT1d9Te-y6Jw": //尚旅周末服务号下的id（主要分销使用）
                    {
                        dataStr = "{ \"first\": { \"value\":\"[FIRST]\", \"color\":\"#fe8000\"},";
                        dataStr += "\"keyword1\":{ \"value\":\"[K1]\", \"color\":\"#555555\" },";
                        dataStr += "\"keyword2\":{ \"value\":\"[K2]\", \"color\":\"#555555\" },";
                        dataStr += "\"remark\":{ \"value\":\"[REMARK]\", \"color\":\"#3E9EC0\" } }";
                        break;
                    }
                //项目进度通知
                case "cHsVtD8UCnZBzZiit-RO_Ik3ilZux14NzO1AaAilIDU": //尚旅周末服务号下的id（主要分销使用）
                    {
                        dataStr = "{ \"first\": { \"value\":\"[FIRST]\", \"color\":\"#fe8000\"},";
                        dataStr += "\"keyword1\":{ \"value\":\"[K1]\", \"color\":\"#555555\" },";
                        dataStr += "\"keyword2\":{ \"value\":\"[K2]\", \"color\":\"#555555\" },";
                        dataStr += "\"remark\":{ \"value\":\"[REMARK]\", \"color\":\"#3E9EC0\" } }";
                        break;
                    }
                //活动状态变更通知
                case "111111": //周末酒店服务号浩颐下的id
                case "qR6NqzH2pz3LXo-dCU3bsBAnH6kRWajXlIHzsOp5X-8": //遛娃指南服务号下的id
                case "DLZiRIC57xYsf7FnszOOlqxJx0s5GtYFfuRIzMGMZtU": //遛娃指南无锡服务号下的id
                case "444444": //遛娃指南苏州服务号下的id
                case "555555": //遛娃指南杭州服务号下的id
                case "rAbn5PFrlAp7qFSoEdvYrznHpj0WjkboqASebf7mj_w": //尚旅周末服务号下的id
                    {
                        dataStr = "{ \"first\": { \"value\":\"[FIRST]\", \"color\":\"#fe8000\"},";
                        dataStr += "\"keyword1\":{ \"value\":\"[K1]\", \"color\":\"#555555\" },";
                        dataStr += "\"keyword2\":{ \"value\":\"[K2]\", \"color\":\"#555555\" },";
                        dataStr += "\"keyword3\":{ \"value\":\"[K3]\", \"color\":\"#555555\" },";
                        dataStr += "\"keyword4\":{ \"value\":\"[K4]\", \"color\":\"#555555\" },";
                        dataStr += "\"remark\":{ \"value\":\"[REMARK]\", \"color\":\"#3E9EC0\" } }";
                        break;
                    }
                //分销订单通知
                case "57GrHgxxc7FYIQoaHMQs0UquzMGN_JtpfflahNQ5rMc": //尚旅周末服务号
                    {
                        dataStr = "{ \"first\": { \"value\":\"[FIRST]\", \"color\":\"#fe8000\"},";
                        dataStr += "\"keyword1\":{ \"value\":\"[K1]\", \"color\":\"#555555\" },";   //订单编号
                        dataStr += "\"keyword2\":{ \"value\":\"[K2]\", \"color\":\"#555555\" },";   //商品名称
                        dataStr += "\"keyword3\":{ \"value\":\"[K3]\", \"color\":\"#555555\" },";   //下单时间
                        dataStr += "\"keyword4\":{ \"value\":\"[K4]\", \"color\":\"#555555\" },";   //下单金额
                        dataStr += "\"keyword5\":{ \"value\":\"[K5]\", \"color\":\"#555555\" },";   //分销商名称
                        dataStr += "\"remark\":{ \"value\":\"[REMARK]\", \"color\":\"#3E9EC0\" } }";
                        break;
                    }
                //成为分销商通知
                case "ldn16SUidzjp34rk8fL6ehT4Kdfcj-nAf8lFiFZ-Cuc": //尚旅周末服务号
                    {
                        dataStr = "{ \"first\": { \"value\":\"[FIRST]\", \"color\":\"#fe8000\"},";
                        dataStr += "\"keyword1\":{ \"value\":\"[K1]\", \"color\":\"#555555\" },";
                        dataStr += "\"keyword2\":{ \"value\":\"[K2]\", \"color\":\"#555555\" },";
                        dataStr += "\"remark\":{ \"value\":\"[REMARK]\", \"color\":\"#3E9EC0\" } }";
                        break;
                    }
            }

            dataStr = dataStr.Replace("[FIRST]", first);
            if (list != null && list.Count > 0)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    var _k = list[i];
                    dataStr = dataStr.Replace(string.Format("[K{0}]", (i + 1)), _k);
                }
            }
            dataStr = dataStr.Replace("[REMARK]", remark);

            /*
             {
                    \""first\"": {
                        \""value\"":\""你的好友小兵兵帮你助力成功！\"",
                        \""color\"":\""#fe8000\""
                    },
                    \""HotelName\"":{
                        \""value\"":\""周末酒店\"",
                        \""color\"":\""#2c2c2c\""
                    },
                    \""ProductName\"": {
                        \""value\"":\""一元助力产品\"",
                        \""color\"":\""#2c2c2c\""
                    },
                    \""remark\"":{
                        \""value\"":\"">>点击详情查看助力动态！\"",
                        \""color\"":\""#173177\""
                    }
            }
             */

            return dataStr;
        }

        #endregion

        public static string GoodDealVote(int activeCode, string userFrom)
        {
           return  commService.ActiveVote(activeCode, userFrom);
        }

        public static string GetToken()
        {
            return weixinService.GetToken();
        }

        public static Dictionary<string,string> weixinDict;

        /// <summary>
        /// 微信公众号事件参数自定义处理
        /// </summary>
        /// <param name="eventName">事件名称key</param>
        /// <param name="eventKey">事件包含的自定义的参数值</param>
        /// <param name="openId">当前关注者openid</param>
        /// <param name="fromuserName"></param>
        /// <param name="createTime"></param>
        /// <param name="channelCode">当前公众号枚举类型</param>
        /// <returns></returns>
        public static string ProcessEventKeyInput(string eventName, string eventKey, string openId, string fromuserName, string createTime, WeiXinChannelCode channelCode)
        {
            var _response = "";

            //subscribe--oHGzlw64Od16EpBke0PUojcPJEC0--gh_7622f728d6c1--GROUPTREE_SKUID_ACTIVEID_GROUPID_USERID
            //SCAN--oHGzlw64Od16EpBke0PUojcPJEC0--gh_7622f728d6c1--GROUPTREE_SKUID_ACTIVEID_GROUPID_USERID

            /*
             * GROUPTREE 团购助力
             * ACTIVEFANS 分销活动粉丝
             * ACTIVESUPERVOTE 酒店大使投票活动
             * ACTIVELUCKCORD 免费住翻倍卡扫码关注
             * COUPONPRODUCT 消费券产品推送
             *
             * 关注事件的key里面包含:
             * qrscene_GROUPTREE_1549_102176_4576_0
             * qrscene_ACTIVEFANS_4514792_11
             * qrscene_ACTIVESUPERVOTE_786_85
             * qrscene_ACTIVELUCKCORD_829~3~openid
             * qrscene_COUPONPRODUCT_14175_4514792
             *
             */
            eventKey = eventKey.Replace("qrscene_", "");

            //解析EventKey
            var _keyList = Regex.Split(eventKey, "_");
            var _type = _keyList[0];

            switch (_type)
            {
                case "GROUPTREE":
                    #region
                    {

                        var _skuId = 0;
                        var _activeId = 0;
                        var _groupId = 0;
                        var _userId = 0;

                        try
                        {
                            if (_keyList.Length > 1) _skuId = Convert.ToInt32(_keyList[1]);
                            if (_keyList.Length > 2) _activeId = Convert.ToInt32(_keyList[2]);
                            if (_keyList.Length > 3) _groupId = Convert.ToInt32(_keyList[3]);
                            if (_keyList.Length > 4) _userId = Convert.ToInt32(_keyList[4]);

                            if (_skuId > 0 && _activeId > 0 && _groupId > 0)
                            {
                                var _result = new CouponController().SubmitSetLike(openId, _groupId, _activeId, 0);
                                if (_result != null)
                                {
                                    if (_result.RetCode == "1")
                                    {
                                        _response = @"你已帮助好友助力成功！我们也为你准备了免费的礼品【点击领取】";

                                        //获取当前团信息
                                        var _groupSKUDetail = CouponAdapter.GetGroupSKUCouponActivity(_skuId, 0, _groupId, openId: openId);

                                        //获取团长信息
                                        if (_groupSKUDetail != null && _groupSKUDetail.GroupPurchase != null && _groupSKUDetail.GroupPurchase.GroupPeople != null && _groupSKUDetail.GroupPurchase.GroupPeople.Exists(_ => _.IsSponsor))
                                        {
                                            var _sPeople = _groupSKUDetail.GroupPurchase.GroupPeople.Find(_ => _.IsSponsor);
                                            _response = string.Format(@"你已帮助好友{0}助力成功！我们也为你准备了免费的礼品【点击领取】", _sPeople.NickName);

                                            //产品图url
                                            var _pUrl = "http://whfront.b0.upaiyun.com/app/img/coupon/groupproductfortree/group-process-img.png";
                                            //if (_groupSKUDetail.activity.PicList != null && _groupSKUDetail.activity.PicList.Count > 0 && !string.IsNullOrEmpty(_groupSKUDetail.activity.PicList[0]))
                                            //{
                                            //    _pUrl = _groupSKUDetail.activity.PicList[0].Replace("_appdetail1", "_640x360");
                                            //}

                                            //给助力者的推送信息配置
                                            var _resTitle = _response;
                                            var _resPicUrl = _pUrl;
                                            var _resLink = string.Format("http://www.zmjiudian.com/coupon/group/product/{0}/0", _skuId);

                                            //判断当前产品是否配置了自定义推送信息
                                            if (_groupSKUDetail.SKUInfo != null && _groupSKUDetail.SKUInfo.SKU != null)
                                            {
                                                try
                                                {
                                                    if (!string.IsNullOrEmpty(_groupSKUDetail.SKUInfo.SKU.CustomTitle.Trim()))
                                                    {
                                                        _resTitle = _groupSKUDetail.SKUInfo.SKU.CustomTitle;
                                                    }

                                                    if (!string.IsNullOrEmpty(_groupSKUDetail.SKUInfo.SKU.CustomPhoto.Trim()) && _groupSKUDetail.SKUInfo.SKU.CustomPhoto.ToLower().Contains("http"))
                                                    {
                                                        _resPicUrl = _groupSKUDetail.SKUInfo.SKU.CustomPhoto.Replace("_640x426", "_640x360").Replace("_jupiter", "_640x360").Replace("_appdetail1", "_640x360");
                                                    }

                                                    if (!string.IsNullOrEmpty(_groupSKUDetail.SKUInfo.SKU.CustomLink.Trim()) && _groupSKUDetail.SKUInfo.SKU.CustomLink.ToLower().Contains("http"))
                                                    {
                                                        _resLink = _groupSKUDetail.SKUInfo.SKU.CustomLink;
                                                    }
                                                }
                                                catch (Exception ex)
                                                {
                                                    
                                                }
                                            }

                                            //组合图文推送信息
                                            _response = GenWeiXinTextForLinkItem(_resTitle, "", _resPicUrl, _resLink);
                                            _response = GenWeiXinTextForLinkArticleReturn(_response, openId, fromuserName, createTime, 1);

                                            #region 给团长推送模板消息

                                            if (!string.IsNullOrEmpty(_sPeople.OpenId))
                                            {
                                                try
                                                {
                                                    var weixinAcount = _groupSKUDetail.activity.WeixinAcountId > 0 ? _groupSKUDetail.activity.WeixinAcountId : 7;
                                                    var weixinAcountName = "周末酒店";

                                                    //团购订单状态提醒
                                                    var tempId = "8Ctx7mex5dalgAE_cu0GC12v2fwPs8Szx2vn_1Ri2oM";
                                                    switch ((WeiXinChannelCode)weixinAcount)
                                                    {
                                                        case WeiXinChannelCode.周末酒店服务号_皓颐:
                                                            weixinAcountName = "周末酒店";
                                                            tempId = "8Ctx7mex5dalgAE_cu0GC12v2fwPs8Szx2vn_1Ri2oM";
                                                            break;
                                                        case WeiXinChannelCode.周末酒店苏州服务号_皓颐:
                                                            weixinAcountName = "周末酒店";
                                                            tempId = "Vjtz_ENJc8bJIkdVz3tGLCcjgXaxFjsyODkGwEo-bPc";
                                                            break;
                                                        case WeiXinChannelCode.周末酒店成都服务号_皓颐:
                                                            weixinAcountName = "周末酒店";
                                                            tempId = "1tY-ArY11_vaNMGQ_P1z0cATZBv_7rkSV6ngaUIwLDY";
                                                            break;
                                                        case WeiXinChannelCode.周末酒店深圳服务号_皓颐:
                                                            weixinAcountName = "周末酒店";
                                                            tempId = "leGThrNm9C1j_RsgaoVqwso1hyD7utCyRaTCkcNmmKE";
                                                            break;
                                                        case WeiXinChannelCode.遛娃指南服务号_皓颐:
                                                            weixinAcountName = "遛娃指南";
                                                            tempId = "M_ZSUolM0yuKdZKHYakAO8J_KkEcQPqx58fv_hooSgw";
                                                            break;
                                                        case WeiXinChannelCode.遛娃指南南京服务号_皓颐:
                                                            weixinAcountName = "遛娃指南";
                                                            tempId = "WYbP8jXvU-wh1LsQETtRgf2oJ99Vp1hpq99F8L6_rQk";
                                                            break;
                                                        case WeiXinChannelCode.遛娃指南无锡服务号_皓颐:
                                                            weixinAcountName = "遛娃指南";
                                                            //tempId = "KmQWuI4mkf2gUuGR79PnWD4v2sBcVHlcpVMHxr30ap4";
                                                            tempId = "kxA6PDXdHY-JZX9s643sKZcEeU6_tKPJU7Y5Pd6LSYQ";
                                                            break;
                                                        case WeiXinChannelCode.遛娃指南广州服务号_皓颐:
                                                            weixinAcountName = "遛娃指南";
                                                            tempId = "kLAbgRrvtQpkNmmd0V94MoatJUA-GXBU1VUgkSuHoV4";
                                                            break;
                                                        case WeiXinChannelCode.遛娃指南杭州服务号_皓颐:
                                                            weixinAcountName = "遛娃指南";
                                                            tempId = "AFB7Q8HvXIUaDpWntXkAXxns8U2-3HCSqAmEL2SjLT0";
                                                            break;
                                                    }

                                                    //点击文字
                                                    var clickTxt = ">>点击继续邀请好友";

                                                    //团状态url
                                                    var groupTreeUrl = string.Format("http://www.zmjiudian.com/coupon/group/tree/{0}/{1}", _skuId, _groupId);

                                                    //获取当前助力者信息
                                                    var first = "你有新的好友助力！";
                                                    if (_groupSKUDetail.GroupPurchase.GroupPeople.Exists(_ => _.OpenId == openId))
                                                    {
                                                        var sendPeople = _groupSKUDetail.GroupPurchase.GroupPeople.Find(_ => _.OpenId == openId);
                                                        first = string.Format("你的好友{0}已为你助力！", sendPeople.NickName);
                                                    }

                                                    //小团还差几人
                                                    var _rCount = _groupSKUDetail.activity.GroupCount - _groupSKUDetail.GroupPurchase.GroupPeople.Count;

                                                    //超过成团人数时，不再给发起者重复推送成功提示
                                                    if (_rCount >= 0)
                                                    {
                                                        //小团成功
                                                        if (_groupSKUDetail.GroupPurchase.State == 1)
                                                        {
                                                            clickTxt = ">>点击去使用";

                                                            //groupTreeUrl = string.Format("http://www.zmjiudian.com/app/order?userid={0}&specifyuid=1", _sPeople.UserId);
                                                            //groupTreeUrl = GetUserWalletUrl(_sPeople.UserId, "productcoupon", "detail", newpage: true, dorpdown: true);
                                                            groupTreeUrl = "http://www.zmjiudian.com/Account/WxMenuTransfer?menu=10";

                                                            first = string.Format("恭喜助力成功，马上去领！", _groupSKUDetail.activity.PageTitle);
                                                        }
                                                        else
                                                        {
                                                            first = first + string.Format("还差{0}人就能领了！", _rCount, _groupSKUDetail.activity.PageTitle);
                                                        }

                                                        //做换行处理
                                                        first = string.Format(@"{0}\r\n", first);
                                                        clickTxt = string.Format(@"\r\n{0}", clickTxt);

                                                        //data list
                                                        var dataList = new List<string>();
                                                        dataList.Add(weixinAcountName);
                                                        dataList.Add(_groupSKUDetail.activity.PageTitle);

                                                        SendTemplateMessage(weixinAcount, _sPeople.OpenId, tempId, groupTreeUrl, first, clickTxt, dataList);   

                                                        ////暂时改为只有团成功时才会给发起者推送模板消息（无锡号总是被举报，安全起见助力只在发起和成功的时候推送模板消息 haoy 20180905）
                                                        //if (_groupSKUDetail.GroupPurchase.State == 1)
                                                        //{
                                                        //    SendTemplateMessage(weixinAcount, _sPeople.OpenId, tempId, groupTreeUrl, first, clickTxt, dataList);   
                                                        //}
                                                    }
                                                }
                                                catch (Exception ex)
                                                {

                                                }
                                            }

                                            #endregion

                                        }
                                    }
                                    else
                                    {
                                        _response = _result.Message;
                                    }
                                }
                                else
                                {
                                    _response = "抱歉，参与失败，请重试";
                                }
                            }
                            else
                            {
                                _response = "操作异常：参数错误";
                            }
                        }
                        catch (Exception ex)
                        {
                            _response = "操作异常";
                        }

                        break;
                    }
                    #endregion
                case "ACTIVEFANS":
                    #region
                    {
                        try
                        {
                            var _channelCodeNum = Convert.ToInt32(channelCode);

                            var _fromCID = 0;
                            var _fromAID = 0;
                            if (_keyList.Length > 1) _fromCID = Convert.ToInt32(_keyList[1]);   //当前二维码的分享者CID
                            if (_keyList.Length > 2) _fromAID = Convert.ToInt32(_keyList[2]);   //当前活动的ID

                            //获取当前活动信息
                            var _activeEntity = new ActivityController().GetChannelActiveQueryByID(_fromAID);
                            if (_activeEntity != null && !string.IsNullOrEmpty(_activeEntity.ActiveLink))
                            {
                                _response = string.Format(@"为你推荐：{0}", _activeEntity.PosterDesc);

                                //活动图url
                                var _pUrl = "";
                                if (!string.IsNullOrEmpty(_activeEntity.PosterBannerUrl))
                                {
                                    _pUrl = _activeEntity.PosterBannerUrl.Replace("_jupiter", "_640x360").Replace("_appdetail1", "_640x360");
                                }

                                //追加CID
                                if (!_activeEntity.ActiveLink.ToLower().Contains("cid="))
                                {
                                    _activeEntity.ActiveLink += _activeEntity.ActiveLink.Contains("?") ? string.Format("&CID={0}", _fromCID) : string.Format("?CID={0}", _fromCID);   
                                }

                                //组合图文推送信息
                                _response = GenWeiXinTextForLinkItem(_response, "", _pUrl, _activeEntity.ActiveLink);
                                _response = GenWeiXinTextForLinkArticleReturn(_response, openId, fromuserName, createTime, 1);
                            }
                            else
                            {
                                _response = @"感谢关注！活动已结束或已下线~";
                            }

                            //Log.WriteLog(string.Format("Poster _fromCID: {0}", _fromCID));

                            //处理关联关系
                            if (_fromCID > 0)
                            {
                                //Log.WriteLog(string.Format("Poster GetWeixinUserSubscribeInfo: {0} {1}", openId, _channelCodeNum));

                                //获取当前关注者的信息
                                WeixinUser wu = weixinService.GetWeixinUserSubscribeInfo2(openId, _channelCodeNum);
                                if (wu != null && !string.IsNullOrEmpty(wu.Unionid))
                                {
                                    //Log.WriteLog(string.Format("Poster wu.Unionid: {0}", wu.Unionid));

                                    //只有未关联过zmjd账号 & 没有关联为别人的粉丝，才会绑定为分享者的活动粉丝
                                    var isNewUser = false;
                                    var noRelFans = false;

                                    #region 1 检查当前用户是否有关联zmjd账号

                                    var _userChannelRel = new UserChannelRelEntity
                                    {
                                        Channel = "weixin",
                                        Code = wu.Unionid,
                                        UserId = 0,
                                        Tag = "",
                                        CreateTime = DateTime.Now
                                    };

                                    var _rel = AccountAdapter.GetOneUserChannelRel(_userChannelRel);
                                    if (_rel != null && _rel.UserId > 0)
                                    {
                                        //Log.WriteLog(string.Format("Poster isNewUser = false"));

                                        //到这里说明当前微信已经关联zmjd账号，视为老用户...
                                        isNewUser = false;
                                    }
                                    else
                                    {
                                        //Log.WriteLog(string.Format("Poster isNewUser = true"));

                                        //没有关联过zmjd账号，为新用户
                                        isNewUser = true;
                                    }

                                    #endregion

                                    #region 2 检查是否已成为别人的活动粉丝

                                    var fansInfo = AccountAdapter.GetOneFansRelByUnionid(wu.Unionid);
                                    if (fansInfo != null && fansInfo.UserID > 0)
                                    {
                                        //Log.WriteLog(string.Format("Poster noRelFans = false"));

                                        //已经是别人粉丝了
                                        noRelFans = false;
                                    }
                                    else
                                    {
                                        //Log.WriteLog(string.Format("Poster noRelFans = true"));

                                        //无粉丝关联记录
                                        noRelFans = true;
                                    }

                                    #endregion

                                    //没有关联zmjd账号也没有成为别人粉丝，则绑定为当前分享者粉丝
                                    if (isNewUser && noRelFans)
                                    {
                                        //Log.WriteLog(string.Format("Poster AddUserFansRel: {0} {1} {2} {3}", openId, wu.Unionid, _fromCID, Convert.ToInt32(channelCode).ToString()));

                                        var _fansEntity = new UserFansRel { Openid = openId, Unionid = wu.Unionid, UserID = _fromCID, WeixinAcount = Convert.ToInt32(channelCode).ToString(), SourceType = 1, SourceId = _fromAID, CreateTime = DateTime.Now };
                                        var _addFans = AccountAdapter.AddUserFansRel(_fansEntity);

                                        //Log.WriteLog(string.Format("Poster AddUserFansRel ：{0}", _addFans));

                                        #region 存储当前用户信息

                                        wu.WeixinAcount = "weixinservice_haoyi";
                                        switch (channelCode)
                                        {
                                            case WeiXinChannelCode.周末酒店服务号_皓颐:
                                                wu.WeixinAcount = "weixinservice_haoyi";
                                                break;
                                            case WeiXinChannelCode.遛娃指南服务号_皓颐:
                                                wu.WeixinAcount = "weixinservice_haoyi_liuwa";
                                                break;
                                            case WeiXinChannelCode.周末酒店苏州服务号_皓颐:
                                                wu.WeixinAcount = "weixinservice_haoyi_sz";
                                                break;
                                            case WeiXinChannelCode.周末酒店成都服务号_皓颐:
                                                wu.WeixinAcount = "weixinservice_haoyi_cd";
                                                break;
                                            case WeiXinChannelCode.周末酒店深圳服务号_皓颐:
                                                wu.WeixinAcount = "weixinservice_haoyi_shenz";
                                                break;
                                            case WeiXinChannelCode.遛娃指南南京服务号_皓颐:
                                                wu.WeixinAcount = "weixinservice_haoyi_liuwa_nj";
                                                break;
                                            case WeiXinChannelCode.遛娃指南无锡服务号_皓颐:
                                                wu.WeixinAcount = "weixinservice_haoyi_liuwa_wx";
                                                break;
                                            case WeiXinChannelCode.遛娃指南广州服务号_皓颐:
                                                wu.WeixinAcount = "weixinservice_haoyi_liuwa_gz";
                                                break;
                                            case WeiXinChannelCode.遛娃指南杭州服务号_皓颐:
                                                wu.WeixinAcount = "weixinservice_haoyi_liuwa_hzh";
                                                break;
                                        }

                                        var _updateWeixinUser = weixinService.UpdateWeixinUserInfo(wu);

                                        #endregion

                                        #region 给分享者推送模板消息

                                        //未处理...

                                        #endregion
                                    }
                                }
                                else
                                {
                                    Log.WriteLog(string.Format("Poster GetWeixinUserSubscribeInfo: is null: {0}", openId));
                                }
                            }
                            
                        }
                        catch (Exception ex)
                        {
                            Log.WriteLog(string.Format("Poster error: {0}", ex.Message));
                        }
                        break;
                    }
                    #endregion
                case "ACTIVESUPERVOTE":
                    #region
                    {
                        var _channelCodeNum = Convert.ToInt32(channelCode);

                        var _activeId = 0;
                        var _ruleExId = 0;
                        var _goVoteDrawId = 0;
                        if (_keyList.Length > 1) _activeId = Convert.ToInt32(_keyList[1]);   //当前主活动的ID
                        if (_keyList.Length > 2) _ruleExId = Convert.ToInt32(_keyList[2]);   //带入的酒店ID，直接生成指定酒店的海报时使用
                        if (_keyList.Length > 3) _goVoteDrawId = Convert.ToInt32(_keyList[3]);   //被投票用户的报名ID（如果有该ID传入，说明是投票行为）

                        //默认是进入个人主页的推送
                        var _tit = string.Format(@"Hello亲子大使～点击打开个人主页，获取海报/天天抽奖");
                        var _pUrl = "http://whphoto.b0.upaiyun.com/118LQzU0_jupiter";
                        var _link = string.Format("http://www.zmjiudian.com/wx/active/supervoteuser/{0}/{1}", _activeId, _ruleExId);

                        //如果有被投票者ID，则是投票成功推送
                        if (_goVoteDrawId > 0)
                        {
                            if (DateTime.Now > DateTime.Parse("2018-12-10 23:59:59"))
                            {
                                _tit = string.Format(@"活动已截止，感谢您的关注与参与！");
                                _pUrl = "http://whphoto.b0.upaiyun.com/118LQzU0_jupiter";
                                _link = string.Format("http://www.zmjiudian.com/wx/active/supervoteuser/{0}/{1}", _activeId, _ruleExId);
                            }
                            else
                            {
                                //是否可投票
                                var _canGoVote = true;

                                //获取当前用户的今日投票记录
                                var _todayVoteRecord = new WeixinApiController().GetActiveVoteRecordForType1ByWxAccount(_activeId, openId, 1);
                                if (_todayVoteRecord != null)
                                {
                                    //今日次数已达5次
                                    if (_todayVoteRecord.Count >= 10)
                                    {
                                        _tit = string.Format(@"你今天的投票次数已用完，明天再来哦~【点击进入活动主场】");
                                        _pUrl = "http://whphoto.b0.upaiyun.com/118ONeR0_jupiter";
                                        _link = string.Format("http://www.zmjiudian.com/wx/active/supervote/{0}/0", _activeId);

                                        _canGoVote = false;
                                    }
                                    else
                                    {
                                        //一家酒店一天只能投一次
                                        if (_todayVoteRecord.Exists(_ => _.SourceId == _ruleExId))
                                        {
                                            //并检测今天是否通过当前drawid（包括自己）投过该酒店
                                            var _todayGoVotesForDrawid = new WeixinApiController().GetActiveVoteRecordForType2BySourceIdAndReltionId(_activeId, _goVoteDrawId, _ruleExId, openId, 1);
                                            if (_todayGoVotesForDrawid != null && _todayGoVotesForDrawid.Count > 0)
                                            {
                                                _tit = string.Format(@"你今天已帮好友的这次打榜投过票啦~【点击进入活动主场】");
                                                _pUrl = "http://whphoto.b0.upaiyun.com/118ONeR0_jupiter";
                                                _link = string.Format("http://www.zmjiudian.com/wx/active/supervote/{0}/0", _activeId);

                                                _canGoVote = false;
                                            }
                                        }
                                    }
                                }

                                if (_canGoVote)
                                {
                                    var _goVoteEntityForUser = new ActiveVoteRecord
                                    {
                                        ID = 0,
                                        WeixinAccount = openId,
                                        UserId = 0,
                                        SourceId = _goVoteDrawId,
                                        SourceType = 2,
                                        ReltionId = _ruleExId,
                                        State = 1,
                                        ActiveId = _activeId
                                    };

                                    var _add = weixinService.AddActiveVoteRecord(_goVoteEntityForUser);
                                    if (_add == -1)
                                    {
                                        return "";
                                    }
                                    else
                                    {
                                        //给酒店投
                                        var goVoteEntityForHotel = new ActiveVoteRecord
                                        {
                                            ID = 0,
                                            WeixinAccount = openId,
                                            UserId = 0,
                                            SourceId = _ruleExId,
                                            SourceType = 1,
                                            ReltionId = _goVoteDrawId,
                                            State = 1,
                                            ActiveId = _activeId
                                        };
                                        _add = weixinService.AddActiveVoteRecord(goVoteEntityForHotel);

                                        _tit = string.Format(@"投票成功！我们也为你准备了全国50+酒店免费住和现金红包【点击进入】");
                                        _pUrl = "http://whphoto.b0.upaiyun.com/118ONeR0_jupiter";
                                        _link = string.Format("http://www.zmjiudian.com/wx/active/supervote/{0}/0", _activeId);
                                    }
                                }
                            }
                        }

                        //组合图文推送信息
                        _response = GenWeiXinTextForLinkItem(_tit, "", _pUrl, _link);
                        _response = GenWeiXinTextForLinkArticleReturn(_response, openId, fromuserName, createTime, 1);

                        break;
                    }
                #endregion
                case "ACTIVELUCKCORD":
                    #region
                    {
                        if (_keyList.Length > 1)
                        {
                            var _channelCodeNum = Convert.ToInt32(channelCode);

                            var _shareInfo = _keyList[1] + (_keyList.Length > 2 ? "_" + _keyList[2] : "") + (_keyList.Length > 3 ? "_" + _keyList[3] : "") + (_keyList.Length > 4 ? "_" + _keyList[4] : "") + (_keyList.Length > 5 ? "_" + _keyList[5] : "");
                            var _shareInfoList = Regex.Split(_shareInfo, "~");

                            var _activeId = 0;
                            var _getLuckCount = 0;
                            var _sharerOpenid = "";
                            if (_shareInfoList.Length > 0) _activeId = Convert.ToInt32(_shareInfoList[0]);      //当前主活动的ID
                            if (_shareInfoList.Length > 1) _getLuckCount = Convert.ToInt32(_shareInfoList[1]);  //需要奖励的抽奖码个数
                            if (_shareInfoList.Length > 2) _sharerOpenid = _shareInfoList[2];                   //当前分享者的openid
                            if (!string.IsNullOrEmpty(_sharerOpenid) && _getLuckCount > 0)
                            {
                                //获取当前活动信息
                                var weixinActiveEntity = weixinService.GetOneWeixinActive(_activeId);
                                if (weixinActiveEntity != null)
                                {
                                    //抽奖码标签
                                    var _luckCodeTagName = "翻倍卡助力";

                                    //提示文案
                                    var _title2 = "我们也为你准备了免费住活动【点击查看】";
                                    switch (_activeId)
                                    {
                                        case 829:
                                            _title2 = "我们也为你准备了一场免费年会团建【点击查看】";
                                            break;
                                    }

                                    //提供关注者帮助成功，点击是进入当前免费住活动推文
                                    var _tit = string.Format(@"助力成功！{0}", _title2);
                                    var _pUrl = weixinActiveEntity.WeixinSignUpTopBannerUrl.Replace("_jupiter", "_640x360");
                                    var _link = weixinActiveEntity.WeixinSignUpShareLink;

                                    //检查当前用户的抽奖码
                                    var thisActiveLuckCodeList = weixinService.GetActiveWeixinLuckCodeInfoByTagNameAndSource(_activeId, _luckCodeTagName, openId) ?? new List<ActiveWeixinLuckCode>();

                                    //首先检查当前用户是否已经通过翻倍卡投过票
                                    if (thisActiveLuckCodeList.Count > 0)
                                    {
                                        _getLuckCount = 0;

                                        //已投过
                                        _tit = string.Format(@"你已经助力过哦~{0}", _title2);
                                    }
                                    else
                                    {
                                        //如果当前活动有限制每个人的抽奖码数量，则需要验证
                                        if (weixinActiveEntity.PersonMaxLucks > 0)
                                        {
                                            //检查当前用户的抽奖码
                                            var thisUserLuckCodeList = weixinService.GetActiveWeixinLuckCodeInfo(_activeId, _sharerOpenid) ?? new List<ActiveWeixinLuckCode>();

                                            //如果有剩余，则可以奖励
                                            if (weixinActiveEntity.PersonMaxLucks - thisUserLuckCodeList.Count <= 0)
                                            {
                                                _getLuckCount = 0;

                                                //超出个人可获得抽奖码总数
                                                _tit = string.Format(@"助力成功。{0}", _title2);
                                            }
                                            else
                                            {
                                                //如果有剩余，则可以奖励；如果剩余不足本次奖励个数，则以剩余为准
                                                if (weixinActiveEntity.PersonMaxLucks - thisUserLuckCodeList.Count < _getLuckCount)
                                                {
                                                    _getLuckCount = weixinActiveEntity.PersonMaxLucks - thisUserLuckCodeList.Count;
                                                }
                                            }
                                        }
                                    }

                                    if (_getLuckCount > 0)
                                    {
                                        //奖励指定数量抽奖码
                                        for (int i = 0; i < _getLuckCount; i++)
                                        {
                                            var aLuck = new ActiveWeixinLuckCode();
                                            aLuck.ActiveId = _activeId;
                                            aLuck.Openid = _sharerOpenid;
                                            aLuck.PartnerId = _channelCodeNum;
                                            aLuck.TagName = "翻倍卡助力";
                                            aLuck.SourceOpenid = openId;
                                            aLuck.CreateTime = DateTime.Now;
                                            var genluckcode = weixinService.PublishWeixinLuckCodeTask(aLuck);
                                        }
                                    }

                                    //组合图文推送信息
                                    _response = GenWeiXinTextForLinkItem(_tit, "", _pUrl, _link);
                                    _response = GenWeiXinTextForLinkArticleReturn(_response, openId, fromuserName, createTime, 1);
                                }
                            }
                        }

                        break;
                    }
                #endregion
                case "COUPONPRODUCT":
                    #region
                    {
                        var _channelCodeNum = Convert.ToInt32(channelCode);

                        var _SKUID = 0;
                        var _fromCID = 0;
                        if (_keyList.Length > 1) _SKUID = Convert.ToInt32(_keyList[1]);   //产品SKUID
                        if (_keyList.Length > 2) _fromCID = Convert.ToInt32(_keyList[2]); //当前二维码的分享者CID

                        //获取当前产品信息
                        var _productDetail = CouponAdapter.GetSKUCouponActivityDetail(_SKUID);
                        if (_productDetail != null && _productDetail.activity != null && _productDetail.SKUInfo != null)
                        {
                            //提供关注者帮助成功，点击是进入当前免费住活动推文
                            var _tit = !string.IsNullOrEmpty(_productDetail.SKUInfo.SKU.ShareTitle) ? _productDetail.SKUInfo.SKU.ShareTitle : _productDetail.activity.PageTitle;
                            _tit = "为你推荐：" + _tit;

                            var _desc = !string.IsNullOrEmpty(_productDetail.SKUInfo.SKU.ShareDescription) ? _productDetail.SKUInfo.SKU.ShareDescription : _productDetail.activity.Tags;
                            var _pUrl = "http://whfront.b0.upaiyun.com/app/img/pic-def-3x2.png";
                            if (_productDetail.activity.PicList != null && _productDetail.activity.PicList.Count > 0)
                            {
                                _pUrl = _productDetail.activity.PicList[0].Replace("_jupiter", "350X350");
                            }

                            //sku_wx_poster_follow 数据统计：微信海报关注推送
                            var _link = string.Format("http://www.zmjiudian.com/coupon/product/{0}?CID={1}&_sourcekey=sku_wx_poster_follow", _SKUID, _fromCID);

                            //组合图文推送信息
                            _response = GenWeiXinTextForLinkItem(_tit, _desc, _pUrl, _link);
                            _response = GenWeiXinTextForLinkArticleReturn(_response, openId, fromuserName, createTime, 1);
                        }

                        break;
                    }
                    #endregion
                case "Test":
                    #region
                    {
                        var _skuId = "1484";

                        _response = GenWeiXinTextForLinkItem("你已帮助好友助力成功！我们也为你准备了免费的礼品，点击领取！", "", "http://whfront.b0.upaiyun.com/app/img/coupon/groupproductfortree/group-process-img.png", string.Format("http://www.zmjiudian.com/coupon/group/product/{0}/0", _skuId));

                        _response = GenWeiXinTextForLinkArticleReturn(_response, openId, fromuserName, createTime, 1);

                        break;
                    }
                    #endregion
            }

            return _response;
        }

        /// <summary>
        /// 格式化微信返回内容(链接模块的方式)
        /// </summary>
        /// <param name="Content">返回内容</param>
        /// <param name="ToUserName"></param>
        /// <param name="FromUserName"></param>
        /// <param name="CreateTime"></param>
        /// <returns></returns>
        public static string GenWeiXinTextForLinkArticleReturn(string Content, string ToUserName, string FromUserName, string CreateTime, int ArticleCount)
        {
            string textFormat = @"<xml>
    <ToUserName><![CDATA[{ToUserName}]]></ToUserName>
    <FromUserName><![CDATA[{FromUserName}]]></FromUserName>
    <CreateTime>{CreateTime}</CreateTime>
    <MsgType><![CDATA[news]]></MsgType>
    <Content><![CDATA[]]></Content>
    <ArticleCount>{ArticleCount}</ArticleCount>
    <Articles>
        {Content}
    </Articles>
</xml>";

            return textFormat
                .Replace("{Content}", Content)
                .Replace("{ArticleCount}", ArticleCount.ToString())
                .Replace("{ToUserName}", ToUserName)
                .Replace("{FromUserName}", FromUserName)
                .Replace("{CreateTime}", CreateTime);
        }

        public static string GenWeiXinTextForLinkItem(string Title, string Description, string PicUrl, string Url)
        {
            string textFormat = @"<item>
            <Title><![CDATA[{Title}]]></Title>
            <Description><![CDATA[{Description}]]></Description>
            <PicUrl><![CDATA[{PicUrl}]]></PicUrl>
            <Url><![CDATA[{Url}]]></Url>
        </item>";

            return textFormat
                .Replace("{Title}", Title)
                .Replace("{Description}", Description)
                .Replace("{PicUrl}", PicUrl)
                .Replace("{Url}", Url);
        }

        /// <summary>
        /// 获取订单&钱包的相关页面url
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="tag"></param>
        /// <param name="mode"></param>
        /// <param name="newtitle"></param>
        /// <param name="newpage"></param>
        /// <param name="dorpdown"></param>
        /// <returns></returns>
        public static string GetUserWalletUrl(long userId, string tag, string mode = "", bool newtitle = false, bool newpage = false, bool dorpdown = false)
        {
            Int64 url_TimeStamp = Signature.GenTimeStamp();
            int url_SourceID = 100;
            string webSiteUrl = "http://www.zmjiudian.com/";
            //webSiteUrl = "http://192.168.1.22:8081/";
            //webSiteUrl = "http://localhost:8780/";
            //webSiteUrl = "http://www.tst.zmjd001.com/";
            string url_RequestType = String.Format("{4}personal/wallet/{0}/{3}{5}?TimeStamp={1}&SourceID={2}", userId, url_TimeStamp, url_SourceID, tag, webSiteUrl, (string.IsNullOrEmpty(mode) ? "" : "/" + mode));
            string MD5Key = ConfigurationManager.AppSettings["MD5Key"];
            string Sign = Signature.GenSignature(url_TimeStamp, url_SourceID, MD5Key, url_RequestType);
            string UserInfoUrl = String.Format("{0}&Sign={1}{2}{3}{4}", url_RequestType, Sign, (newtitle ? "&_newtitle=1" : ""), (newpage ? "&_newpage=1" : ""), (dorpdown ? "&_dorpdown=1" : ""));
            return UserInfoUrl;
        }

        public static string getResponseForTextInput2(string input , string ToUserName , string FromUserName , string CreateTime)
        {
            if (lastRuleUpdateTime < DateTime.Now.AddMinutes(-10)) //每十分钟更新一次
            {
                lastRuleUpdateTime = DateTime.Now;
                InitWeiXinDict();
            }

            if (weixinDict.ContainsKey(input))
            {
                return weixinDict[input].Replace("{ToUserName}", ToUserName)
                                              .Replace("{FromUserName}", FromUserName)
                                              .Replace("{CreateTime}", CreateTime);
            }
            else
            {
                return "";
            }
        }

        public static  void InitWeiXinDict()
        {
            List<HJD.HotelManagementCenter.Domain.CommDictEntity> lcd = commService.GetCommDictList(302);
            weixinDict = new Dictionary<string, string>();
            foreach (HJD.HotelManagementCenter.Domain.CommDictEntity d in lcd)
            {
                weixinDict.Add(d.DicValue, d.Descript);
            }
        }

        public static string ParseText(string input)
        {
            Regex regex = null;
            Match match;

            foreach (string key in GetRules().Keys)
            {

                regex = new Regex(rules[key], RegexOptions.Singleline);


                match = regex.Match(input);

                if (match.Success)
                {
                    return key;
                }
            }

            return "default";
        }

        public static string SignatureWeixinJSSDK(string noncestr, Int64 timestamp, string url)
        {
            string jsapi_ticket = GetTicket();
            string tmp = string.Format("jsapi_ticket={0}&noncestr={1}&timestamp={2}&url={3}",jsapi_ticket,noncestr,timestamp,url);
            //string tmp = string.Format("url={0}&timestamp={1}&noncestr={2}&jsapi_ticket={3}", url, timestamp, noncestr, jsapi_ticket);
            //ToDo 修改加密方法 wwb
            return FormsAuthentication.HashPasswordForStoringInConfigFile(tmp, "SHA1");
        }

        private static string GetTicket()
        {
            return weixinService.GetTicket();
        }

        private static Dictionary<String, String> GetRules()
        {

            if (lastRuleUpdateTime < DateTime.Now.AddMinutes(-10))
            {
                lastRuleUpdateTime = DateTime.Now;

                try
                {
                    Dictionary<string, string> tmpRule = new Dictionary<string, string>();
                    string[] strRules = File.ReadAllLines(Configs.WeiXinTemplatePath + "rules.txt");

                    foreach (string rule in strRules)
                    {
                        tmpRule.Add(rule.Split(':')[0], rule.Split(':')[1]);
                    }
                    
                    rules = new Dictionary<string, string>();

                    foreach (string rule in strRules)
                    {
                        rules.Add(rule.Split(':')[0], rule.Split(':')[1]);
                    }
                }
                catch{}
            
            }
            return rules;
        }

        public static string Menu(string menuInfo)
        {
          //  string menuInfo = System.IO.File.ReadAllText(@"D:\Template\weixin\menu.txt");



            string token = WeiXinAdapter.GetToken();

            string url = "https://api.weixin.qq.com/cgi-bin/menu/create?access_token=" + token;

            CookieContainer cc = new CookieContainer();

            string json = HttpRequestHelper.PostJson(url, menuInfo, ref cc);

            return json +  menuInfo;
        }

        public static WeixinPrePayResult GetWeixinUnifiedOrder(string body, string attach, string out_trade_no, int total_fee, string spbill_create_ip, string notify_url, string trade_type, string openid)
        {
            string APPID = Configs.WeiXinAPPID2;
            string WeiXinSecret = Configs.WeiXinSecret2;
            string url = "https://api.mch.weixin.qq.com/pay/unifiedorder";
            string mch_id = "1306853401";   // "1218698601";

            string nonce_str = DescriptionHelper.GenerateRandomStr();
            string sign = Signature.WeixinPayUnifiedorderSignature(APPID, WeiXinSecret, mch_id, nonce_str, body, attach, out_trade_no, total_fee, spbill_create_ip, notify_url, trade_type, openid);
            //StringBuilder strBuilder = new StringBuilder();
            //strBuilder.Append("<xml>");
            //strBuilder.Append("<appid>" + APPID + "</appid>");
            //strBuilder.Append("<mch_id>" + "1218698601"  + "</mch_id>");
            //strBuilder.Append("<nonce_str>" + noncestr + "</nonce_str>");
            //strBuilder.Append("<sign>" + sign + "</sign>");
            //strBuilder.Append("<body>" + body + "</body>");
            //strBuilder.Append("<out_trade_no>" + out_trade_no + "</out_trade_no>");
            //strBuilder.Append("<total_fee>" + total_fee.ToString() + "</total_fee>");
            //strBuilder.Append("<spbill_create_ip>" + spbill_create_ip + "</spbill_create_ip>");
            //strBuilder.Append("<notify_url>" + notify_url + "</notify_url>");
            //strBuilder.Append("<trade_type>" + trade_type + "</trade_type>");
            //strBuilder.Append("<openid>" + openid + "</openid>");
            //strBuilder.Append("</xml>");

            //string ss = strBuilder.ToString();

            XmlDocument responseXML = new XmlDocument();
            var root = responseXML.AppendChild(responseXML.CreateElement("xml"));
            
            //公众号
            var node = responseXML.CreateElement("appid");
            node.InnerText = APPID;
            root.AppendChild(node);

            //商户号
            node = responseXML.CreateElement("mch_id");
            node.InnerText = mch_id;
            root.AppendChild(node);

            //随机字符串
            node = responseXML.CreateElement("nonce_str");
            node.InnerText = nonce_str;
            root.AppendChild(node);

            //商品描述
            node = responseXML.CreateElement("body");
            var CData = responseXML.CreateCDataSection("body");
            node.AppendChild(CData);
            root.AppendChild(node);

            //签名
            node = responseXML.CreateElement("sign");
            CData = responseXML.CreateCDataSection("sign");
            node.AppendChild(CData);
            root.AppendChild(node);

            //商户订单号
            node = responseXML.CreateElement("out_trade_no");
            node.InnerText = out_trade_no;
            root.AppendChild(node);            

            //总金额
            node = responseXML.CreateElement("total_fee");
            node.InnerText = total_fee.ToString();
            root.AppendChild(node);

            //终端IP
            node = responseXML.CreateElement("spbill_create_ip");
            node.InnerText = spbill_create_ip;
            root.AppendChild(node);

            //通知地址
            node = responseXML.CreateElement("notify_url");
            node.InnerText = notify_url;
            root.AppendChild(node);

            //交易类型 
            node = responseXML.CreateElement("trade_type");
            node.InnerText = trade_type;
            root.AppendChild(node);

            //用户标识 jssdk必传
            node = responseXML.CreateElement("openid");
            node.InnerText = openid;
            root.AppendChild(node);
            
            string ss = responseXML.OuterXml;

            string result = HttpRequestHelper.PostXml(url, ss);

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(result);

            XmlElement rootElement = xmlDoc.DocumentElement;
            XmlNode flag1 = rootElement.SelectSingleNode("return_code");
            
            string return_code = GetXMLNodeValue(flag1);
            if(!return_code.Equals("SUCCESS"))
            {
                XmlNode flag = rootElement.SelectSingleNode("return_msg");
                string return_msg = GetXMLNodeValue(flag);

                return new WeixinPrePayResult(){return_code = return_code, return_msg = return_msg};
            }

            XmlNode flag2 = rootElement.SelectSingleNode("result_code");
            string result_code = GetXMLNodeValue(flag2);
            if(!result_code.Equals("SUCCESS")){
                XmlNode err_codeNode = rootElement.SelectSingleNode("err_code");
                XmlNode err_code_desNode = rootElement.SelectSingleNode("err_code_des");
                string err_code = GetXMLNodeValue(err_codeNode);
                string err_code_des = GetXMLNodeValue(err_code_desNode);

                return new WeixinPrePayResult(){result_code = result_code, err_code = err_code,err_code_des = err_code_des};
            }

            if (return_code.Equals("SUCCESS") && result_code.Equals("SUCCESS"))
            {
                XmlNode prepay_idNode = rootElement.SelectSingleNode("prepay_id");
                string prepay_id = GetXMLNodeValue(prepay_idNode);
                return new WeixinPrePayResult(){prepay_id = prepay_id };
            }

            return new WeixinPrePayResult();
        }

        /// <summary>
        /// 【高端酒店特价】微信小程序统一下单api调用（for Tenpay project）
        /// </summary>
        /// <param name="body"></param>
        /// <param name="attach"></param>
        /// <param name="out_trade_no"></param>
        /// <param name="total_fee"></param>
        /// <param name="spbill_create_ip"></param>
        /// <param name="notify_url"></param>
        /// <param name="trade_type"></param>
        /// <param name="openid"></param>
        /// <returns></returns>
        public static WxAppPayConfig GetWxAppUnifiedOrder(WxAppPayRequestParam p)
        {
            var orderid = p.orderid;
            var clientIP = "127.0.0.1";
            try
            {
                clientIP = HttpContext.Current.Request.UserHostAddress;
            }
            catch (Exception ex)
            {
                
            }
            var openid = p.openid;

            var param = string.Format("orderid={0}&clientIP={1}&openid={2}", orderid, clientIP, openid);
            var pre_pay_url = string.Format("http://tenpay.zmjiudian.com/api/tenpay/pay_req_forWxApp?{0}", param);

            var xml = HttpHelper.Get(pre_pay_url, "utf-8");

            string ns = @"xmlns:i=""http://www.w3.org/2001/XMLSchema-instance"" xmlns=""http://schemas.datacontract.org/2004/07/Tenpay.Models""";
            string nil = @"i:nil=""true""";
            xml = xml.Replace(ns, "").Replace(nil, "");

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);
            XmlElement rootElement = xmlDoc.DocumentElement;

            var _p = GetXMLNodeValue(rootElement, "package");

            var result = new WxAppPayConfig
            {
                package = GetXMLNodeValue(rootElement, "package"),
                nonceStr = GetXMLNodeValue(rootElement, "nonceStr"),
                paySign = GetXMLNodeValue(rootElement, "paySign"),
                signType = GetXMLNodeValue(rootElement, "signType"),
                timeStamp = int.Parse(GetXMLNodeValue(rootElement, "timeStamp")),
                openid = openid,
                orderid = orderid,
                err_code = GetXMLNodeValue(rootElement, "err_code"),
                err_code_des = GetXMLNodeValue(rootElement, "err_code_des"),
                prepay_id = GetXMLNodeValue(rootElement, "prepay_id"),
                result_code = GetXMLNodeValue(rootElement, "result_code"),
                return_code = GetXMLNodeValue(rootElement, "return_code"),
                return_msg = GetXMLNodeValue(rootElement, "return_msg")
            };

            //小程序支付签名生成


            return result;
        }

        /// <summary>
        /// 【周末酒店Lite】微信小程序统一下单api调用（for Tenpay project）
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static WxAppPayConfig GetWxAppUnifiedOrderForZmjdLite(WxAppPayRequestParam p)
        {
            var orderid = p.orderid;
            var clientIP = "127.0.0.1";
            try
            {
                clientIP = HttpContext.Current.Request.UserHostAddress;
            }
            catch (Exception ex)
            {

            }
            var openid = p.openid;

            var param = string.Format("orderid={0}&clientIP={1}&openid={2}", orderid, clientIP, openid);
            var pre_pay_url = string.Format("http://tenpay.zmjiudian.com/api/tenpay/pay_req_forWxApp_Lite?{0}", param);

            var xml = HttpHelper.Get(pre_pay_url, "utf-8");

            string ns = @"xmlns:i=""http://www.w3.org/2001/XMLSchema-instance"" xmlns=""http://schemas.datacontract.org/2004/07/Tenpay.Models""";
            string nil = @"i:nil=""true""";
            xml = xml.Replace(ns, "").Replace(nil, "");

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);
            XmlElement rootElement = xmlDoc.DocumentElement;

            var _p = GetXMLNodeValue(rootElement, "package");

            var result = new WxAppPayConfig
            {
                package = GetXMLNodeValue(rootElement, "package"),
                nonceStr = GetXMLNodeValue(rootElement, "nonceStr"),
                paySign = GetXMLNodeValue(rootElement, "paySign"),
                signType = GetXMLNodeValue(rootElement, "signType"),
                timeStamp = int.Parse(GetXMLNodeValue(rootElement, "timeStamp")),
                openid = openid,
                orderid = orderid,
                err_code = GetXMLNodeValue(rootElement, "err_code"),
                err_code_des = GetXMLNodeValue(rootElement, "err_code_des"),
                prepay_id = GetXMLNodeValue(rootElement, "prepay_id"),
                result_code = GetXMLNodeValue(rootElement, "result_code"),
                return_code = GetXMLNodeValue(rootElement, "return_code"),
                return_msg = GetXMLNodeValue(rootElement, "return_msg")
            };

            //小程序支付签名生成


            return result;
        }

        /// <summary>
        /// 【遛娃指南Lite】微信小程序统一下单api调用（for Tenpay project）
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static WxAppPayConfig GetWxAppUnifiedOrderForLiuwaLite(WxAppPayRequestParam p)
        {
            var orderid = p.orderid;
            var clientIP = "127.0.0.1";
            try
            {
                clientIP = HttpContext.Current.Request.UserHostAddress;
            }
            catch (Exception ex)
            {

            }
            var openid = p.openid;

            var param = string.Format("orderid={0}&clientIP={1}&openid={2}", orderid, clientIP, openid);
            var pre_pay_url = string.Format("http://tenpay.zmjiudian.com/api/tenpay/pay_req_forWxApp_LiuwaLite?{0}", param);

            var xml = HttpHelper.Get(pre_pay_url, "utf-8");

            string ns = @"xmlns:i=""http://www.w3.org/2001/XMLSchema-instance"" xmlns=""http://schemas.datacontract.org/2004/07/Tenpay.Models""";
            string nil = @"i:nil=""true""";
            xml = xml.Replace(ns, "").Replace(nil, "");

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);
            XmlElement rootElement = xmlDoc.DocumentElement;

            var _p = GetXMLNodeValue(rootElement, "package");

            var result = new WxAppPayConfig
            {
                package = GetXMLNodeValue(rootElement, "package"),
                nonceStr = GetXMLNodeValue(rootElement, "nonceStr"),
                paySign = GetXMLNodeValue(rootElement, "paySign"),
                signType = GetXMLNodeValue(rootElement, "signType"),
                timeStamp = int.Parse(GetXMLNodeValue(rootElement, "timeStamp")),
                openid = openid,
                orderid = orderid,
                err_code = GetXMLNodeValue(rootElement, "err_code"),
                err_code_des = GetXMLNodeValue(rootElement, "err_code_des"),
                prepay_id = GetXMLNodeValue(rootElement, "prepay_id"),
                result_code = GetXMLNodeValue(rootElement, "result_code"),
                return_code = GetXMLNodeValue(rootElement, "return_code"),
                return_msg = GetXMLNodeValue(rootElement, "return_msg")
            };

            //小程序支付签名生成


            return result;
        }

        public static string GetXMLNodeValue(XmlNode rootElement, string nodeName)
        {
            try
            {
                XmlNode flag2 = rootElement.SelectSingleNode(nodeName);

                return GetXMLNodeValue(flag2);
            }
            catch
            {
                return "";
            }
        }

        public static string GetXMLNodeValue(XmlNode node)
        {
            if(node != null)
            {
                try
                {
                    XmlCDataSection cdata = (XmlCDataSection)node.FirstChild;
                    if (cdata != null)
                    {
                        return cdata.InnerText;
                    }
                    else
                    {
                        return node.InnerText;
                    }
                }
                catch (Exception ex)
                {
                    if (node.FirstChild != null && !string.IsNullOrEmpty(node.FirstChild.InnerText))
                    {
                        return node.FirstChild.InnerText;
                    }
                }
            }
            else
            {
                return "";
            }

            return "";
        }

        public static WeixinChatRecordResult GetWeixinChatRecord(WeixinChatRecordParams rp)
        {
            string token = WeiXinAdapter.GetToken();    //"J-Fvg3dg0WIj_MzpDlFoymctLx-k2LuNVoZTDWCivJCY4UeMA3CgndDeQN7Fm2vxEOs7RSk8r5jP2A8QK3OsS00wPzkEkoOqIEmEOuDXW9U";// 
            string url = "https://api.weixin.qq.com/cgi-bin/customservice/getrecord?access_token=" + token;

            CookieContainer cc = new CookieContainer();

            string json = HttpRequestHelper.PostJson(url, rp, ref cc);

            return JsonConvert.DeserializeObject<WeixinChatRecordResult>(json);
        }

        /// <summary>
        /// 获取指定UserId和指定微信账号的微信用户信息
        /// </summary>
        /// <param name="uid">UserId</param>
        /// <param name="weixinAcountStr">WeixinUser表的WeixinAccount字段（如 shanglvzhoumo “尚旅周末服务号”的微信用户信息记录）</param>
        /// <returns></returns>
        public static WeixinUser GetOpenidByUid(long uid, string weixinAcountStr)
        {
            var _wxUser = new WeixinUser();

            try
            {
                var _userChannelRel = new UserChannelRelEntity
                {
                    IDX = 0,
                    PhoneNum = "",
                    Channel = "weixin",
                    Code = "",
                    UserId = uid,
                    Tag = "",
                    CreateTime = DateTime.Now
                };

                //首先查询指定UID的微信关联unionid
                var _relEntity = AccountAdapter.GetOneUserChannelRelByUID(_userChannelRel);
                if (_relEntity != null && !string.IsNullOrEmpty(_relEntity.Code))
                {
                    //根据unionid查询指定账号的openid信息
                    _wxUser = weixinService.GetWeixinUserByUnionidAndAccount(_relEntity.Code, weixinAcountStr);
                }
            }
            catch (Exception ex)
            {
                Log.WriteLog("GetOpenidByUid[1]:" + ex.Message);
                Log.WriteLog("GetOpenidByUid[2]:" + ex.StackTrace);
            }

            return _wxUser;
        }
    }

    public class TockenClass
    {
        public string access_token{get;set;}
        public int expires_in{get;set;}
        public string errcode{get;set;}
        public string errmsg{get;set;}
    }

    [DataContract]
    [Serializable]
    public class WeixinPayRequestParam
    {
        /// <summary>
        /// 订单描述
        /// </summary>
        [DataMember]
        public string body { get; set; }
        /// <summary>
        /// 商户内部订单号
        /// </summary>
        [DataMember]
        public string out_trade_no { get; set; }
        /// <summary>
        /// 总费用（单位分）
        /// </summary>
        [DataMember]
        public int total_fee { get; set; }
        /// <summary>
        /// 终端IP
        /// </summary>
        [DataMember]
        public string spbill_create_ip { get; set; }
        /// <summary>
        /// 通知地址
        /// </summary>
        public string notify_url { get; set; }
        /// <summary>
        /// 交易类型
        /// </summary>
        [DataMember]
        public string trade_type { get; set; }
        /// <summary>
        /// 获得openId需要的Code（回调时可以获得）
        /// </summary>
        [DataMember]
        public string code { get; set; }
    }

    [DataContract]
    [Serializable]
    public class WeixinPrePayResult
    {
        /// <summary>
        /// 签名结果
        /// </summary>
        [DataMember]
        public string return_code { get; set; }
        /// <summary>
        /// 签名结果描述
        /// </summary>
        [DataMember]
        public string return_msg { get; set; }
        /// <summary>
        /// 微信支付预付Id
        /// </summary>
        [DataMember]
        public string prepay_id { get; set; }
        /// <summary>
        /// 预付结果
        /// </summary>
        [DataMember]
        public string result_code { get; set; }
        /// <summary>
        /// 预付结果代码
        /// </summary>
        [DataMember]
        public string err_code { get; set; }
        /// <summary>
        /// 预付结果代码描述
        /// </summary>
        [DataMember]
        public string err_code_des { get; set; }
    }
}
