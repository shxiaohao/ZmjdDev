using HJD.Framework.WCF;
using HJD.HotelManagementCenter.IServices;
using HJD.HotelServices.Contracts;
using HJD.WeixinService.Contract;
using HJD.WeixinServices.Contract;
using HJD.WeixinServices.Contracts;
using HJDAPI.Common.Helpers;
using HJDAPI.Controllers.Activity;
using HJDAPI.Controllers.Adapter;
using HJDAPI.Controllers.Common;
using HJDAPI.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Security;
using System.Xml;

namespace HJDAPI.Controllers
{
    public class WeixinBaseController : Controller
    {

        public static ICommService commService = ServiceProxyFactory.Create<ICommService>("ICommService");
        public static IWeixinService weixinService = ServiceProxyFactory.Create<IWeixinService>("IWeixinService");

        public static Dictionary<string, DateTime> dicInDialogUsers = new Dictionary<string, DateTime>();//记录对话中的用户

        protected string logFile = Configs.LogPath + string.Format("WeiXinLog_{0}{1}{2}.txt", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

     //   static Dictionary<string, DateTime> dicUserLastTalkTime = new Dictionary<string, DateTime>();


        protected WeiXinChannelCode ChannelCode = WeiXinChannelCode.UNKNOW;
        protected string subscribeEventCode = "subscribe";

        public virtual List<WeixinActivityEntity> GetWeixinActivityList()
        {
            WeiXinAdapter.LoadWeixinActivity();
           
            return WeiXinAdapter.weixinActivityList.Where(a => a.ActivityFinishDateTime > DateTime.Now).ToList();
        }
         


       public string ValidData(string signature, string timestamp, string nonce, string echostr)
        {
            if (string.IsNullOrEmpty(signature) ||
                string.IsNullOrEmpty(timestamp) ||
                string.IsNullOrEmpty(nonce) ||
                string.IsNullOrEmpty(echostr))
                return null;
            List<string> list = new List<string> { timestamp, nonce, "ZMJiuDian2014" };
            list.Sort();
            string tmp = string.Join(string.Empty, list.ToArray());
            //var file = System.IO.File.AppendText(@"D:\Work\TFS\Dev\Site\HJDAPI\HJDAPI\bin\a.txt");
            //file.WriteLine(tmp);
            //file.Close();

            string tmpStr = FormsAuthentication.HashPasswordForStoringInConfigFile(tmp, "SHA1").ToLower();

            if (Configs.WeiXinLog == "open")
            {
                try
                {
                    System.IO.File.AppendAllText(logFile, "\r\n" + DateTime.Now.ToString() + "\r\n" + signature + ":" + timestamp + ":" + nonce + ":" + echostr + ":" + tmpStr);
                }
                catch { }
            }

            if (tmpStr == signature.ToLower())
            {
                return echostr;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string DataAction()
        {
            var inputStream = Request.InputStream;
            var strLen = Convert.ToInt32(inputStream.Length);

            if (strLen == 0)
                return null;
            try
            {
                var strArr = new byte[strLen];
                inputStream.Read(strArr, 0, strLen);
                var requestMes = Encoding.UTF8.GetString(strArr);

                if (Configs.WeiXinLog == "open")
                {
                   WriteLog( requestMes);
                }

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(requestMes);
                XmlElement rootElement = xmlDoc.DocumentElement;
                XmlNode MsgType = rootElement.SelectSingleNode("MsgType");

                RequestEntity requestXML = new RequestEntity();
                requestXML.ToUserName = rootElement.SelectSingleNode("ToUserName").InnerText;
                requestXML.FromUserName = rootElement.SelectSingleNode("FromUserName").InnerText;
                requestXML.CreateTime = rootElement.SelectSingleNode("CreateTime").InnerText;
                requestXML.MsgType = MsgType.InnerText; 


                switch (requestXML.MsgType)
                {
                    case "location":
                        requestXML.Location_X = rootElement.SelectSingleNode("Location_X").InnerText;
                        requestXML.Location_Y = rootElement.SelectSingleNode("Location_Y").InnerText;
                        requestXML.Scale = rootElement.SelectSingleNode("Scale").InnerText;
                        requestXML.Label = rootElement.SelectSingleNode("Label").InnerText; break;
                    case "image":
                        requestXML.PicUrl = rootElement.SelectSingleNode("PicUrl").InnerText;
                        requestXML.MsgID = rootElement.SelectSingleNode("MsgId").InnerText.Trim();
                        break;
                    case "text":
                        requestXML.Content = rootElement.SelectSingleNode("Content").InnerText.Trim();
                        requestXML.MsgID = rootElement.SelectSingleNode("MsgId").InnerText.Trim();
                        break;
                    case "event":
                        requestXML.Event = rootElement.SelectSingleNode("Event").InnerText.Trim();
                        requestXML.EventKey = rootElement.SelectSingleNode("EventKey").InnerText.Trim();
                        break;
                }

                //解析微信请求对象后，处理这些请求
                return HandleWeixinRequest(requestXML);
            }
            catch (Exception ex)
            {
                WriteLog(ex.Message + ex.StackTrace);
                return null;
            }
        }

         public void WriteLog(string msg)
        {
            try
            {
                System.IO.File.AppendAllText(logFile, "\r\n" + DateTime.Now.ToString() + "\r\n" + msg);
            }
            catch { }
        }

        public string DataTest(RequestEntity requestXML)
        {
            //return "";
            return HandleWeixinRequest(requestXML);
        }

        /// <summary>
        /// 处理微信的请求
        /// </summary>
        /// <param name="requestXML"></param>
        /// <param name="weixinAcountId">1周末酒店  2周末酒店订阅号  3尚旅游</param>
        /// <returns></returns>
        private string HandleWeixinRequest(RequestEntity requestXML)
        {
            #region 初始微信活动配置

            //微信活动信息绑定
            requestXML.ActivityList = GetWeixinActivityList();

            #endregion

            string toUser = requestXML.FromUserName;
            string fromuserName = requestXML.ToUserName;
            string createTime = ConvertDateTimeInt(DateTime.Now).ToString();
            //string MsgId = "5837017832671832047";

            #region 处理文本内容

            if (requestXML.MsgType == "text")
            {
                //调取webservice，执行相关业务处理
                ResponseEntity responseEntity = weixinService.GetWeixinResponseText(requestXML);
                var responseContent = "";

                try
                {
                    //System.IO.File.AppendAllText(logFile, "\r\n" + DateTime.Now.ToString() + "\r\n" + responseEntity.RequestEntity.Content + responseEntity.RequestEntity.ActiveID + responseEntity.RequestEntity.OrderID + responseEntity.RequestEntity.FromUserName);

                    //如果返回中有需要执行事件，则首先执行事件
                    if (!string.IsNullOrEmpty(responseEntity.ResponseEvent))
                    {
                        responseContent = RunWeiXinAdapterMethod(responseEntity.ResponseEvent, responseEntity.RequestEntity).ToString();
                        if (!string.IsNullOrEmpty(responseContent) && string.IsNullOrEmpty(responseEntity.ResponseContent))
                        {
                            responseEntity.ResponseContent = responseContent;
                        }
                    }
                }
                catch (Exception ex)
                {
                   WriteLog(ex.Message);
                } 


                //客人在晚上12点到早上8点之间发的消息，如果是非标，我们自动回复”您好，我们的微信客服时间是每天早上8点至晚上12点，您的需求已经收到，我们会在工作时间尽快回复您“

                //if (string.IsNullOrEmpty(responseEntity.ResponseContent))
                //{
                //    if( DateTime.Now.Hour >23 || DateTime.Now.Hour < 8)
                //    {
                //        responseEntity.ResponseContent = "您好，我们的微信客服时间是每天早上8点至晚上12点，您的需求已经收到，我们会在工作时间尽快回复您。";
                //    }
                //}

                WriteLog("responseEntity.ResponseContent:" + responseEntity.ResponseContent);

                //返回最终content
                if (!string.IsNullOrEmpty(responseEntity.ResponseContent))
                {
                    if (responseEntity.ResponseContent.IndexOf("transfer_customer_service") > 0)
                    {
                        return GenWeiXinTextForTransfer_Customer_Service(requestXML, createTime);
                    }
                    else
                    {                        
                        CheckContentFormat(ref responseEntity, responseEntity.RequestEntity);

                        if (IsInDialogUserList(requestXML.FromUserName)) //如果用户已在对话中，那么需要将用户的消息转发给客服，同时回复用户信息
                        {
                            return GenWeiXinTextForTransfer_Customer_Service(requestXML, createTime , responseEntity.ResponseContent);
                        }
                        else
                        {
                            return responseEntity.ResponseContent;
                        }
                    }
                }



                return GenWeiXinTextForTransfer_Customer_Service(requestXML , createTime );
            }

            #endregion

            #region 其他类型处理

            else
            {

                XmlDocument responseXML = new XmlDocument();
                var root = responseXML.AppendChild(responseXML.CreateElement("xml"));
                var node = responseXML.CreateElement("ToUserName");
                var CData = responseXML.CreateCDataSection(toUser);
                node.AppendChild(CData);
                root.AppendChild(node);

                node = responseXML.CreateElement("FromUserName");
                CData = responseXML.CreateCDataSection(fromuserName);
                node.AppendChild(CData);
                root.AppendChild(node);

                node = responseXML.CreateElement("CreateTime");
                node.InnerText = createTime;
                root.AppendChild(node);

                node = responseXML.CreateElement("MsgType");
                CData = responseXML.CreateCDataSection("text");
                node.AppendChild(CData);
                root.AppendChild(node);

                node = responseXML.CreateElement("FuncFlag");
                node.InnerText = "0";
                root.AppendChild(node);

                node = responseXML.CreateElement("Content");

                switch (requestXML.MsgType)
                {
                    #region Location

                    case "location":
                        break;

                    #endregion

                    #region 图片信息处理

                    case "image":
                        {
                            string feedBack = "";

                            ResponseEntity responseEntity = weixinService.GetWeixinResponseImage(requestXML);
                            if (!string.IsNullOrEmpty(responseEntity.ResponseContent))
                            {
                                feedBack = responseEntity.ResponseContent;
                            }
                            else
                            {
                                //得到唯一一个会有图片交互的活动（暂时一次只能设置一个有图片交互的活动，因为如果有多个不好判断接收的图片归属哪个活动）
                                var potoActivity = requestXML.ActivityList.Find(a => a.HasPhotoStep && DateTime.Now < a.ActivityFinishDateTime);

                                if (potoActivity != null)  //DateTime.Now >= requestXML.ActiveStartDateTime && 
                                {
                                    feedBack = WeiXinAdapter.AddActiveLuckyDrawSharePhoto(potoActivity, toUser, requestXML.PicUrl);
                                }
                                else
                                {
                                    feedBack = "照片已收到";
                                }
                            }
                            GenWeiXinTextForTransfer_Customer_Service(requestXML, createTime);//将图片发给环信客服
                            return GenWeiXinTextReturn(feedBack, toUser, fromuserName, createTime);
                        }

                    #endregion

                    #region 事件处理

                    case "event":
                        {
                            return DealEventTypeEvent(requestXML, toUser, fromuserName, createTime);
                        }

                    #endregion
                }


                node.AppendChild(CData);
                root.AppendChild(node);
                return responseXML.OuterXml;
            }

            #endregion
        }

        /// <summary>
        /// 处理微信公众号的事件
        /// </summary>
        /// <param name="requestXML"></param>
        /// <param name="toUser"></param>
        /// <param name="fromuserName"></param>
        /// <param name="createTime"></param>
        /// <returns></returns>
        public  virtual string DealEventTypeEvent(RequestEntity requestXML, string toUser, string fromuserName, string createTime)
        {
            if (!string.IsNullOrEmpty(requestXML.EventKey))
            {
                //subscribe--oHGzlw64Od16EpBke0PUojcPJEC0--gh_7622f728d6c1--GROUPTREE_SKUID_ACTIVEID_GROUPID_USERID
                //SCAN--oHGzlw64Od16EpBke0PUojcPJEC0--gh_7622f728d6c1--GROUPTREE_SKUID_ACTIVEID_GROUPID_USERID

                //WriteLog(string.Format("{0}--{1}--{2}--{3}", requestXML.Event, toUser, fromuserName, requestXML.EventKey));

                //对EventKey做处理（自定义业务处理）
                var _backContent = WeiXinAdapter.ProcessEventKeyInput(requestXML.Event, requestXML.EventKey, toUser, fromuserName, createTime, ChannelCode);

                if (!string.IsNullOrEmpty(_backContent))
                {
                    if (!_backContent.ToLower().Contains("<xml>"))
                    {
                        return GenWeiXinTextReturn(_backContent, toUser, fromuserName, createTime);
                    }
                    else
                    {
                        return _backContent;
                    }
                }
                else
                {
                    return WeiXinAdapter.getResponseForTextInput2(requestXML.EventKey, toUser, fromuserName, createTime);
                }
            }
            else
            {
                if (requestXML.Event == "subscribe")
                {
                    return WeiXinAdapter.getResponseForTextInput2(subscribeEventCode, toUser, fromuserName, createTime);
                }
                else
                {
                    return WeiXinAdapter.getResponseForTextInput2(requestXML.EventKey, toUser, fromuserName, createTime);
                }   
            }
        }

        public object RunWeiXinAdapterMethod(string eventStr, RequestEntity request)
        {
            WeiXinAdapter weixinAd = new WeiXinAdapter();
            Type type = typeof(WeiXinAdapter);
            return type.GetMethod(eventStr).Invoke(weixinAd, new object[1] { request });
        }

        //检查当前
        private void CheckContentFormat(ref ResponseEntity responseEntity, RequestEntity request)
        {
            if (!responseEntity.ResponseContent.ToLower().Contains("<xml>"))
            {
                responseEntity.ResponseContent = GenWeiXinTextReturn(responseEntity.ResponseContent, request.FromUserName, request.ToUserName, request.CreateTime);
            }
        }

        public static string GenWeiXinTextReturn(string Content, string ToUserName, string FromUserName, string CreateTime)
        {
            string textFormat = @"<xml>
 <ToUserName><![CDATA[{ToUserName}]]></ToUserName>
 <FromUserName><![CDATA[{FromUserName}]]></FromUserName>
 <CreateTime>{CreateTime}</CreateTime>
 <MsgType><![CDATA[text]]></MsgType>
 <Content><![CDATA[{Content}]]></Content>
</xml>";

            return textFormat.Replace("{Content}", Content)
               .Replace("{ToUserName}", ToUserName)
                                               .Replace("{FromUserName}", FromUserName)
                                               .Replace("{CreateTime}", CreateTime);
        }

        public string GenWeiXinTextForTransfer_Customer_Service(RequestEntity requestXML, string CreateTime, string toUserMsg = "")
        {
            string ToUserName = requestXML.FromUserName;
            string FromUserName = requestXML.ToUserName;
            //            string textFormat = @"<xml>
            //<ToUserName><![CDATA[{ToUserName}]]></ToUserName>
            //<FromUserName><![CDATA[{FromUserName}]]></FromUserName>
            //<CreateTime>{CreateTime}</CreateTime>
            //<MsgType><![CDATA[transfer_customer_service]]></MsgType>
            //</xml>";

            //            return textFormat
            //               .Replace("{ToUserName}", ToUserName)
            //                                               .Replace("{FromUserName}", FromUserName)
            //                                               .Replace("{CreateTime}", CreateTime);

            string msgType = requestXML.MsgType;
            WeixinUser wu = weixinService.GetWeixinUserSubscribeInfo(requestXML.FromUserName, ChannelCode);
            string userNickName = string.Format("{0} {1} {2}", ChannelCode, wu.Nickname, requestXML.FromUserName);
            string picUrl = "";
            string msg = "";

            switch (msgType)
            {
                case "text":
                    msg = requestXML.Content;

                    if (string.IsNullOrEmpty(toUserMsg))
                    {
                        //如果用户在晚上11点到第二天8点之间发送服务信息，则返回用户友情提示，同时需要进入多客服
                        if (DateTime.Now.Hour > 23 || DateTime.Now.Hour < 8)
                        {
                            toUserMsg = "您好，我们的微信客服时间是每天早上8点至晚上12点，您的需求已经收到，我们会在工作时间尽快回复您。";
                        }
                    }

                    break;
                case "image":
                    picUrl = requestXML.PicUrl;
                    break;
            }

            if (IsNeedIntercept(msg) == false)
            {
                string result = new EMChatAdapter().SendWeixinMsgToKeFu(msg, requestXML.MsgID, requestXML.FromUserName, requestXML.ToUserName, userNickName, msgType, picUrl);
            }
     //      WriteLog(string.Format("{0}:{1}:{2}:{3}:{4}", this.ChannelCode.ToString(), requestXML.FromUserName, requestXML.ToUserName, msg, result));

            AddOrUpdateInDialogUserList(requestXML.FromUserName);
            return toUserMsg;
        }

        /// <summary>
        /// 不进MagiCall微信关键字 过滤
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        private bool IsNeedIntercept(string msg)
        {
            Log.WriteLog("IsNeedIntercept:" + msg + ":" + string.Join(",", CommAdapter.GetCommDicKeyValueWidthCache(10008, 6)));
            if (CommAdapter.GetCommDicKeyValueWidthCache(10008, 6).Where(_ => _.Equals(msg)).Count() > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        private void AddOrUpdateInDialogUserList(string userKey)
        {
            if (dicInDialogUsers.ContainsKey(userKey))
            {
                dicInDialogUsers[userKey] = DateTime.Now;
            }
            else
            {
                dicInDialogUsers.Add(userKey, DateTime.Now);
            }
        }

        /// <summary>
        /// 如果最近2小时在对话中，那么认为是在对话中
        /// </summary>
        /// <param name="userKey"></param>
        /// <returns></returns>
        private bool IsInDialogUserList(string userKey)
        {
              if (dicInDialogUsers.ContainsKey(userKey))
              {
                  return dicInDialogUsers[userKey].AddHours(2) > DateTime.Now;
              }
              else
              {
                  return false;
              }
        }
        private int ConvertDateTimeInt(System.DateTime time)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            return (int)(time - startTime).TotalSeconds;
        }
    }
}
