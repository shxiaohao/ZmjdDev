using System;
using System.Linq;
using System.Net;
using System.Web.Http;
using HJD.AccountServices.Contracts;
using HJD.AccountServices.Entity;
using HJD.Framework.WCF;
using HJDAPI.Common.Helpers;
using HJDAPI.Models;
using Newtonsoft.Json;
using HJD.HotelManagementCenter.Domain.Comm;
using HJD.HotelManagementCenter.IServices;
using HJDAPI.Common.Security;
using System.Collections.Generic;
using HJDAPI.Controllers.Adapter;
using System.IO;
using HJD.Framework.Interface;
using HJD.CouponService.Contracts.Entity;
using System.Web;
using Com.Ctrip.Framework.Apollo;

namespace HJDAPI.Controllers
{
    public class AppController:BaseApiController
    {
        static ICommService commService = ServiceProxyFactory.Create<ICommService>("ICommService");


        public static ICacheProvider LocalCache10Min = CacheManagerFactory.Create("DynamicCacheFor10Min");

        /// <summary>
        /// 记录APP 的错误日志
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        public BaseResult LogAppErrorLog(AppErrorParam param)
        {
            if (Signature.IsRightSignature(param.TimeStamp, param.SourceID, param.RequestType, param.Sign))
            {
                LogBase.WriteAppErrorLog(param.ErrorMessage);
                return new BaseResult()
                {
                    RetCode = "0",
                    Message = "提交成功"
                };
            }
            else
            {
                return new BaseResult()
                {
                    RetCode = "1",
                    Message = "签名错误，无法提交"
                };
            }
        }


        /// <summary>
        /// 是否需要额外的验证机制， 如极验等的第三方验证
        /// 系统根据客户端请求数据和来进行判断。 需要统一的APPFunctionID, 以便根据不同的应用场景做不同的判断
        /// </summary> 
        /// <param name="APPFunctionID">调用者当前功能ID  1:注册 2：登陆 3：忘记密码</param>
        /// <returns> RetCode = "0": 需要， RetCode="1" : 不需要 </returns>
        [HttpGet]
        public ResponseBase IsNeedAddationalCheck(int APPFunctionID)
        {

            string apiName = "IsNeedAddationalCheck";
            if (APPFunctionID > 3)
            {
                return new ResponseBase( ResponseBaseCode.noAppFunctionID, apiName);
            }
            else
            {
                return new ResponseBase( ResponseBaseCode.success, apiName)
                {
                    Data = new { NeedAddationalCheck = Configs.IsNeedAddationalCheck  }
                };
            }
        }

        /// <summary>
        /// 获取APP配置
        /// </summary>
        /// <param name="AppVer"></param>
        /// <param name="AppType"></param>
        /// <param name="appH5ZipVer"></param>
        /// <returns></returns>
        [HttpGet]
        public Dictionary<string, string> GetAppConfig(string AppVer, int AppType, int appH5ZipVer = 0)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();

            //app配置所在的namespace
            var _appNamespaceName = "app";

            //获取所有配置Key
            var _configNames = Apollo.GetAppConfig(_appNamespaceName).GetPropertyNames();
            if (_configNames != null && _configNames.Count > 0)
            {
                var _configNameList = _configNames.ToList();

                //遍历所有key
                foreach (var _configName in _configNameList)
                {
                    //获取指定key的值
                    var _configValue = Apollo.Get(_configName, "", _appNamespaceName);

                    dic.Add(_configName, _configValue);
                }
            }

            dic.Add("H5ZIPVer", GetH5ZIPVerConfig(appH5ZipVer));

            //config在App端的更新周期（app端会跟该time比较时间差，目前>=5分钟会执行更新，5分钟是在app写死的 2019.03.06 haoy）
            dic.Add("ServiceTime", Signature.GenTimeStamp().ToString());

            return dic;
        }

        public string GetH5ZIPVerConfig(int appH5ZipVer)
        {
          // return GenCurH5ZIPVerConfig(appH5ZipVer);
            return LocalCache10Min.GetData<String>("LastH5ZipVer:" + appH5ZipVer.ToString(), () =>
                    {
                        return GenCurH5ZIPVerConfig(appH5ZipVer);
                    });
        }

        public string GenCurH5ZIPVerConfig(int appH5ZipVer)
        {
            string ZipVerPackageList = GetLastH5ZipVerPackageList();

            string zipUrlBase = "http://resource.zmjiudian.com/h5zip/";

            string strReturn = "";

            int  CurVer = 0;
          
            string [] lines = ZipVerPackageList.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            CurVer = int.Parse(lines[0].Split(',')[1]);

            if(appH5ZipVer == 30) //因为IOS的BUG引入的临时处理
            {
                appH5ZipVer = 0;
            }

            if (appH5ZipVer < CurVer )
            {  
                foreach (string line in ZipVerPackageList.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
                {



                    if (line.Split(',')[0] == appH5ZipVer.ToString() || line.Split(',')[0] == "0")//"0"为全量包。也就是找不到增量包就用全量包来更新
                    {
                        strReturn = string.Format("{0},{1},{2}", line.Split(',')[0], line.Split(',')[1], zipUrlBase + line.Split(',')[2]);
                        break;
                    }

                }
            }

            return strReturn;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public String GetLastH5ZipVerPackageList()
        {
          //  return File.ReadAllText(@"D:\WHAPPTools\H5AppPub\Data\zipVerPackageList.txt");
            return LocalCache10Min.GetData<String>("H5ZipVerPackageList:", () =>
            {
                return File.ReadAllText(@"D:\WHAPPTools\H5AppPub\Data\zipVerPackageList.txt");
            });
        }

        [HttpPost]
        public bool AddDeviceInfo(DeviceInfoModel di)
        {
            DeviceInfoEntity deviceinfo = new DeviceInfoEntity() { AppVer = di.AppVer, AppType = di.AppType, UserID = di.UserID, OS = di.OS, DeviceInfo = di.DeviceInfo, DeviceID = di.DeviceID };
            deviceinfo.IsValid = true;
             commService.AddDeviceInfo(deviceinfo) ;
             return true;
        }

        [HttpGet]
        public string TestSecurity(int TimeStamp, int sourceID , string data, string RequestType, string sign)
        {
            string MD5Key = "zmjd0001";  // Configs.MD5Key; //"zmjd0001";
                
            if (sourceID < 0)
            {
                return Signature.GetTestSignature(-sourceID, MD5Key, data, RequestType);
            }
            else
            {
                try
                {
                    // switch (sourceID)
                    //{
                    //    case 1://"ios":
                    //    case 2://"android":
                    //    case 3://"web":
                    //    case 4://"msite":
                    //        MD5Key = "zmjd0001";
                    //        break;
                    //}
                    bool isRightSignature = Signature.IsRightSignature(TimeStamp, sourceID, MD5Key, RequestType, sign);

                    string decodeData = DES.Decrypt(data, MD5Key);
                    return string.Format("签名结果：{0} 加密内容：{1}", (isRightSignature ? "正确" : "错误"), decodeData);
                }
                catch (Exception err)
                {
                    return err.Message;
                }
            }
        }
        
        [HttpPost]
        public BaseSubmitResult AddSuggesstions(SuggesstionParam param)
        {
            if(Signature.IsRightSignature(param.TimeStamp, param.SourceID, param.RequestType, param.Sign))
            {
                commService.AddUserFeedbackEntity(new HJD.HotelManagementCenter.Domain.UserFeedbackEntity()
                {
                    terminalType = (int)param.SourceID,
                    text = param.Content,
                    userID = param.UserID,
                    type = param.SuggestionType
                });
                return new BaseSubmitResult()
                {
                    Success = true,
                    Message = "您的反馈我们已经收到，非常感谢您的宝贵建议。"
                };
            }
            else{
                return new BaseSubmitResult()
                {
                    Success = false,
                    Message = "签名错误，无法提交"
                };
            }
        }
        
        [HttpGet]
        public PreLoadAppData GetPreLoadAppData(PreLoadAppDataParam param)
        {
            int givenAdvId = 77;//固定了一个广告 专门用来显示App查找页面的信息
            HJD.ADServices.Contract.AdvBaseInfoEntity avInfo = ADAdapter.GetAdvBaseInfoByID(givenAdvId);
            if (avInfo != null && avInfo.ID > 0)
            {
                return new PreLoadAppData()
                {
                    SearchHotelAdv = new SearchHotelAdv()
                    {
                        ActionUrl = ADAdapter.FormatActionURL(avInfo.ContentUrl),
                        HotelBGP = avInfo.BgImgUrl,
                        HotelID = 0,//设成0 不影响App使用
                        HotelName = string.IsNullOrWhiteSpace(avInfo.Name) ? "" : avInfo.Name
                    }
                };
            }

            //App默认查找页面广告背景数据
            return new PreLoadAppData()
            {
                SearchHotelAdv = new SearchHotelAdv()
                {
                    ActionUrl = "whotelapp://www.zmjiudian.com/hotel/213676",
                    HotelBGP = "http://whphoto.b0.upaiyun.com/115MOsC04B_jupiter",
                    HotelID = 213676,
                    HotelName = "溧阳涵田度假村酒店"
                }
            };
        }

        #region App弹窗处理

        /// <summary>
        /// 获取首页的弹窗信息
        /// </summary>
        /// <param name="curUserID"></param>
        /// <returns></returns>
        [HttpGet]
        public HomeDataModel20 GetAppAlertBoxData(long curUserID = 0)
        {
            var returnObj = new HomeDataModel20();

            /*
             * 1:推荐朋友 
             * 2:VIP剩余2天提示 
             * 3:VIP剩余1天提示 
             * 4:VIP剩余0天提示
             */
            //首先查询用户的VIP状态(目前分别在VIP到期时间还有2天1天0天的时候，给出弹窗提示)
            //暂时只给过期VIP提示，那么就只需要给 userInfo.CustomerType == 0 做弹窗判断
            var userInfo = AccountAdapter.GetUserInfo(curUserID);
            if (userInfo != null)// || userInfo.CustomerType == 4 || userInfo.CustomerType == 5 || userInfo.CustomerType == 6 || userInfo.CustomerType == 7))
            {
                returnObj = new HomeDataModel20()
                {
                    BoxData = new PopupBoxData()
                    {
                        isShow = false,
                        showUrl = "",
                        lazyLoadTime = 0.2f,
                        widthRatio = 0.8f,
                        frequency = 0,
                        boxId = "-1",
                        boxType = 0,
                        widthHeightRatio = 0.6f
                    }
                };

                //2019春节500红包弹窗
                if (DateTime.Now <= DateTime.Parse("2019-02-19 23:59"))
                {
                    var _geted = new CouponController().CheckUserCouponItemByUserIdAndCouponDefineIdlist(curUserID, "735");
                    if (!_geted)
                    {
                        returnObj.BoxData.boxType = 9;
                        returnObj.BoxData.boxId = DateTime.Now.ToString("yyyyMM") + curUserID;
                        var _showUrl = "http://www.zmjiudian.com/ad/adbox?userid={userid}&type=9";
                        returnObj.BoxData.showUrl = _showUrl;
                        returnObj.BoxData.isShow = true;
                        return returnObj;
                    }
                }
                else
                {
                    //定期红包活动弹窗提示(目前是所有用户都弹)
                    if (DateTime.Now > DateTime.Parse("2018-04-12 11:00") && DateTime.Now <= DateTime.Parse("2018-04-30 23:59"))
                    {
                        var _hasGift = new CouponController().IsUserHasGetNewVIPGift(curUserID);
                        if (!_hasGift)
                        {
                            returnObj.BoxData.boxType = 7;
                            returnObj.BoxData.boxId = DateTime.Now.Date.ToString("yyyyMMdd") + curUserID;
                            var _showUrl = "http://www.zmjiudian.com/ad/adbox?userid={userid}&type=7";
                            returnObj.BoxData.showUrl = _showUrl;
                            returnObj.BoxData.isShow = true;
                            return returnObj;
                        }
                    }
                    //暑期大促 首页弹窗暂时拿掉 2018.08.16 haoy
                    //else if ((DateTime.Now > DateTime.Parse("2018-07-16 19:30") || curUserID == 4514792 || curUserID == 4848910 || curUserID == 4862114) && DateTime.Now <= DateTime.Parse("2018-08-31 12:00"))
                    //{
                    //    var _hasGift = new CouponController().IsUserHasGetNewVIPGift(curUserID);
                    //    if (!_hasGift)
                    //    {
                    //        returnObj.BoxData.boxType = 8;
                    //        returnObj.BoxData.boxId = DateTime.Now.Date.ToString("yyyyMMdd") + curUserID;
                    //        var _showUrl = "http://www.zmjiudian.com/ad/adbox?userid={userid}&type=8";
                    //        returnObj.BoxData.showUrl = _showUrl;
                    //        returnObj.BoxData.isShow = true;
                    //        return returnObj;
                    //    }
                    //}

                    //非VIP判断
                    if (userInfo.CustomerType == 0)
                    {
                        //购买VIP时间
                        List<ExchangeCouponEntity> exchangeVip = CouponAdapter.GetExchangeCouponEntityListByUser(curUserID, (int)(HJDAPI.Common.Helpers.Enums.CouponType.VIP));
                        if (exchangeVip != null && exchangeVip.Count > 0)
                        {
                            //非VIP用户&存在VIP过期记录，则告知用户去购买VIP的提示（剩余0天）
                            if (exchangeVip.Exists(_ => _.State == 8))
                            {
                                returnObj.BoxData.boxId = DateTime.Now.Date.ToString("yyyy") + "0000";
                                returnObj.BoxData.boxType = 4;
                                returnObj.BoxData.showUrl = "http://www.zmjiudian.com/ad/adbox?userid={userid}&type=4";

                                //非VIP暂时不弹出任何提示 2018-03-27 haoy
                                returnObj.BoxData.isShow = false;
                            }
                        }
                    }
                    //VIP判断
                    else
                    {
                        var time1 = DateTime.Now;
                        var time2 = userInfo.EndVipTime;

                        //计算VIP剩余天数
                        TimeSpan ts1 = new TimeSpan(time1.Ticks);
                        TimeSpan ts2 = new TimeSpan(time2.Ticks);
                        TimeSpan ts = ts1.Subtract(ts2).Duration();

                        //当VIP的到期天数小于等于30天的时候，弹出“即将到期”的提示
                        if (ts.Days <= 30)
                        {
                            returnObj.BoxData.isShow = true;
                            returnObj.BoxData.boxType = 5;
                            var _showUrl = "http://www.zmjiudian.com/ad/adbox?userid={userid}&type=5";
                            if (userInfo.CustomerType == (int)HJDAPI.Common.Helpers.Enums.CustomerType.vip599)
                            {
                                //599暂时不提示续费了，暂时开始停止发展599会员 2018.04.24 haoy
                                returnObj.BoxData.isShow = false;
                                returnObj.BoxData.boxType = 6;
                                _showUrl = "http://www.zmjiudian.com/ad/adbox?userid={userid}&type=6";
                            }

                            returnObj.BoxData.boxId = DateTime.Now.Date.ToString("yyyyMM") + "00" + curUserID;
                            returnObj.BoxData.showUrl = _showUrl;

                        }
                        else
                        {
                            //大于30天的不提示 2018-03-27 haoy
                            //switch (ts.Days)
                            //{
                            //    case 2:
                            //        {
                            //            returnObj.BoxData.boxType = 2;
                            //            returnObj.BoxData.boxId = DateTime.Now.Date.ToString("yyyy") + "0002";
                            //            returnObj.BoxData.showUrl = "http://www.zmjiudian.com/ad/adbox?userid={userid}&type=2";
                            //            returnObj.BoxData.isShow = true;
                            //            break;
                            //        }
                            //    case 1:
                            //        {
                            //            returnObj.BoxData.boxType = 3;
                            //            returnObj.BoxData.boxId = DateTime.Now.Date.ToString("yyyy") + "0001";
                            //            returnObj.BoxData.showUrl = "http://www.zmjiudian.com/ad/adbox?userid={userid}&type=3";
                            //            returnObj.BoxData.isShow = true;
                            //            break;
                            //        }
                            //    case 0:
                            //        {
                            //            returnObj.BoxData.boxType = 4;
                            //            returnObj.BoxData.boxId = DateTime.Now.Date.ToString("yyyy") + "0000";
                            //            returnObj.BoxData.showUrl = "http://www.zmjiudian.com/ad/adbox?userid={userid}&type=4";
                            //            returnObj.BoxData.isShow = true;
                            //            break;
                            //        }
                            //}
                        }
                    }
                }

                if (returnObj.BoxData.isShow)
                {
                    return returnObj;
                }
            }

            //常规弹窗提示
            var boxDataList = ADAdapter.GetPopBoxConfigEntityList((int)HJDAPI.Common.Helpers.Enums.PopBoxTarget.homepage);
            var boxData = boxDataList.FirstOrDefault();
            returnObj = new HomeDataModel20()
            {
                BoxData = new PopupBoxData()
                {
                    isShow = boxData != null ? boxData.isShow : false,
                    showUrl = boxData != null ? boxData.showUrl : "",
                    lazyLoadTime = boxData != null ? boxData.lazyLoadTime : 0.2f,
                    widthRatio = boxData != null ? boxData.widthRatio : 0.8f,
                    frequency = boxData != null ? boxData.frequency : 0,
                    boxId = boxData != null ? boxData.boxId : "",
                    boxType = 0,
                    widthHeightRatio = boxData != null ? boxData.widthHeightRatio : 0.6f
                }
            };

            //暂不显示常规弹窗
            returnObj.BoxData.isShow = false;

            return returnObj;
        }

        #endregion


        /// <summary>
        /// 检查app更新
        /// </summary>
        [HttpGet]
        public AppUpdateEntity CheckAPPUpdate()
        {
            AppUpdateEntity result = new AppUpdateEntity();
            result.Success = true;
            result.URL = "";
            if (_ContextBasicInfo.IsIOS)
            {
                result.MandatoryVersion = Configs.IOSMandatoryVersion;
                result.ProposedVersion = Configs.IOSProposedVersion;
            }
            else
            {
                result.MandatoryVersion = Configs.AndroidMandatoryVersion;
                result.ProposedVersion = Configs.AndroidProposedVersion;
                result.URL = Configs.APPUpdateandroidURL;

            }
            result.Days = Configs.APPUpdateDays;
            result.Title = Configs.APPUpdateTitle;
            result.Tips = Configs.APPUpdateTips;
            return result;

        }
    }
}