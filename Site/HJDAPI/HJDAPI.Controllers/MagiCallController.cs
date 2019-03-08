using HJD.AccessService.Contract;
using HJD.AccessService.Contract.Model;
using HJD.AccessService.Contract.Model.Dialog;
using HJD.Framework.Interface;
using HJD.Framework.WCF;
using HJD.HotelManagementCenter.IServices;
using HJD.WeixinService.Contract;
using HJDAPI.Controllers.Activity;
using HJDAPI.Controllers.Adapter;
using HJDAPI.Controllers.Common;
using HJDAPI.Models;
using MagiCallService.Contracts.Model.Dialog;
using MagiCallService.Contracts.Model.Dialog.EasemobPushEvents;
using MessageService.Contract;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Security;
using System.Xml;

namespace HJDAPI.Controllers
{
    public class MagiCallController : BaseApiController
    {
        public static ICacheProvider LocalCache5Min = CacheManagerFactory.Create("DynamicCacheFor5Min");
        public static IAccessService AccessService = ServiceProxyFactory.Create<IAccessService>("IAccessService");
        public static MagiCallService.Contracts.IMagiCallService MagiCallService = ServiceProxyFactory.Create<MagiCallService.Contracts.IMagiCallService>("IMagiCallService");

        public static Dictionary<long, DateTime> dicMagiCallUserHeart = new Dictionary<long, DateTime>();

        private const string MagiCallKeFuUserName = "kevincai";
        private const string MagiCallWeiXinChannnelUserName = "kefu001_callback";  //走微信通道时用的代理名


        private static string easemobEvents_ReturnFormat = "{{\"callId\":\"{0}\",\"accept\":\"true\",\"reason\":\"\",\"security\":\"{1}\"}}";//签名。格式如下: MD5（callId+约定的key+"true"），约定key为654321 

        //回调密钥为zmjd10001!，APP响应的密钥为ZMJD10001!

        private static string easemobEvents_ReceiveKey = "zmjd10001!" ;//"123456";
        private static string easemobEvents_ReturnKey = "ZMJD10001!";// "654321";

        private const string FastReplyWords = "收到，请稍等...";

        /// <summary>
        /// 快速回复用户
        /// </summary> 
        /// <returns></returns>
        [System.Web.Http.HttpGet]
        public int  FastReply( )
        {

            string autoReplay = FastReplyWords;



            List<int> dialogIDList =  MagiCallService.GetNeedFastReplyDialogIDList( );
            foreach(int dialogID in dialogIDList)
            {
                try
                {
                    DialogEntity dialog = MagiCallService.GetDialogByID(dialogID);

                    //根据用户录入信息做不同的反馈
                    //var lastItem = dialog.ItemList.Last();
                    //if (lastItem.ItemType == 0)
                    //{
                    //    var userWord = lastItem.Text.Split("\r\n ".ToCharArray())[0];
                    //    if (userWord =="立秀宝" )
                    //    {
                    //        autoReplay = "您好，请提供手机号码，核实后为您处理退款。 由于目前咨询人数较多，请留言，客服将尽快操作。谢谢您的理解和支持...";
                    //    }
                    //    else if (CommMethods.IsPhone(userWord)) {
                    //        if( dialog.ItemList.Where(_=>_.Text.StartsWith("立秀宝")).Count() > 0)
                    //        {
                    //            autoReplay = "您好，已经记录，我们将在核实之后立即为您申请立秀宝退款，款项将在1-3个工作日内原路退回，请注意查收...";
                    //        }
                    //    }

                    //}

                    //Log.WriteMagiCallLog("FastReply", string.Format("FastReply:{0},{1},{2},{3}  ", lastItem.ItemType, lastItem.Text, dialog.UserID, autoReplay));

                    if (dialog.UserID > 0)
                    {
                        MagiCallTxtMsgEntity m = new MagiCallTxtMsgEntity { appType=0, from="", msg = MagiCallAdapter.FormatHTMLMsg(autoReplay), userID = dialog.UserID };
                        EMChatHelper.SendTxtMessageToUser(m);
                    }
                    else
                    {
                        //微信回复
                        string[] SessionInfo = dialog.SessionID.Split(':');
                        if (SessionInfo.Length == 2)
                        {
                            WeiXinAdapter.SentTextMsgToWeiXinUser(SessionInfo[0], autoReplay, SessionInfo[1]);
                        }

                    }
                }
                catch(Exception err)
                {
                    Log.WriteLog(string.Format("FastReply:{0} {1} ", dialogID, err.Message + err.StackTrace));
                }
            }

            return dialogIDList.Count;
        }



        [System.Web.Http.HttpGet]
        public string GetGreeting(long userID, bool isVIPUser)
        { 
            return MagiCallService.GetGreeting(userID, isVIPUser);
        }

        [System.Web.Http.HttpPost]
        public bool UserOptionClickEvent(UserOptionClickEventEntity e)
        {
           var response =  MagiCallService.UserOptionClickEvent(e);

            //switch( e.optionType )
            //{
            //    case  MagiCallService.Contracts.Model.Dialog.UserWordOptionType.FAQChoices:

            //        break;
            //    default:

            //        break;
            //}

            return true;
        }

        /// <summary>
        /// 接收环信微信消息，不包括客服消息
        /// </summary>
        /// <returns></returns>
        [System.Web.Http.HttpPost]
        public string   easemobEvents()
        {

            HttpContent content = Request.Content;

            string json = content.ReadAsStringAsync().Result;

            if (json.Length == 0)
            {
                json = "test!";
            }
            //do something
            Log.WriteMagiCallLog("W",json);

            return DealWeiXinEasemobEvents(json);       
        }


        /// <summary>
        /// 接收环信消息，不包括客服消息
        /// </summary>
        /// <returns></returns>
        [System.Web.Http.HttpPost]
        public string easemobMsgEvents()
        {

            HttpContent content = Request.Content;

            string json = content.ReadAsStringAsync().Result;

            if (json.Length == 0)
            {
                json = "test!";
            }
            //do something
            Log.WriteMagiCallLog("E", json);

            return DealEasemobEvents(json);
        }


        [HttpGet]
        public UserCustomerCareInfoEntity GetLastUserCustomerCareInfo(long userID)
        {
            return new MagiCallAdapter().GetLastUserCustomerCareInfo(userID);
        }


        [HttpGet]
        public CustomerCareEntity GetCustomerCareByUserID(long UserID)
        {
            return new MagiCallAdapter().GetCustomerCareByUserID(UserID);
        }

        public static string DealWeiXinEasemobEvents(string json)
        {
            string callID = "";
            try
            {
                EasemobEventsEntity e = JsonConvert.DeserializeObject<EasemobEventsEntity>(json);
                foreach (var msg in e.payload.bodies)
                {
                    switch (msg.type)
                    {
                        case "txt":
                            #region 新消息（ServiceSessionMessageEvent）事件
                            //                            {
                            //  "timestamp": 1438837020808,
                            //  "chat_type": "chat",
                            //  "from": "kefu001", //在环信移动客服中设置的关联IM账号
                            //  "to": "kefu001_callback", //用于开通实时消息回调功能的IM账号
                            //  "payload": {
                            //      "bodies": [
                            //          {
                            //              "msg": "座席回复的消息",
                            //              "type": "txt"
                            //          }
                            //      ],
                            //      "ext": {
                            //          "weichat": {
                            //              "visitor": {
                            //                  "mp": “gh_9898dc96a1ea",
                            //                  "openid": "oAGnbt89fTgfzOlybTmAh_7s3Z_g",
                            //                  "userNickname": "微信粉丝昵称",
                            //                  "source": "weixin"
                            //              }
                            //          }
                            //      }
                            //  },
                            //  "msg_id": "91363492593926564",
                            //  "callId": "easemob-demo#app_91363492593926564",
                            //  "eventType": "chat",
                            //  "security": “1d9b3a27574989826a034f02ece89161"  //回调签名，见环信回调签名认证方式
                            //}
                            #endregion
                            StringBuilder sb = new StringBuilder();
                            foreach (var body in e.payload.bodies)
                            {
                                sb.AppendLine(body.msg);
                            }
 

                            WeiXinAdapter.SentTextMsgToWeiXinUser(e.payload.ext.weichat.visitor.mp, sb.ToString(), e.payload.ext.weichat.visitor.openid);

                            break;
                        case "img":
                            #region 图片消息
                            //                            {
                            //  "timestamp": 1438837366089,
                            //  "chat_type": "chat",
                            //  "from": "kefu001", //在环信移动客服中设置的关联IM账号
                            //  "to": "kefu001_callback", //用于开通实时消息回调功能的IM账号
                            //  "payload": {
                            //      "bodies": [
                            //          {
                            //              "filename": "8899a801f289.jpg",
                            //              "size": {
                            //                  "width": 750,
                            //                  "height": 500
                            //              },
                            //              "secret": "",
                            //              "type": "img",
                            //              "url": “https://a1.easemob.com/easemob-demo/xuzhengli/chatfiles/5bd56cb0-3bf8-11e5-b668-431280c88f48"  //坐席回复的图片地址
                            //          }
                            //      ],
                            //      "ext": {
                            //          "weichat": {
                            //              "visitor": {
                            //                  "mp": "gh_9898dc86a1ea",
                            //                  "openid": "oAGnbt89fTgfzOlybTmAh_7s3Z_g",
                            //                  "userNickname": "微信粉丝昵称",
                            //                  "source": "weixin"
                            //              }
                            //          }
                            //      }
                            //  },
                            //  "msg_id": "91364975632712188",
                            //  "callId": "easemob-demo#xuzhengli_91364975632712188",
                            //  "eventType": "chat",
                            //  "security": "27db399e77d4f422e50f5b529820ef64"
                            //}
                            #endregion

                            break;
                    }
                }
                callID = e.callId;
            }
            catch (Exception err)
            {
                Log.WriteMagiCallLog("W", "easemobEvents:" + err.Message + err.StackTrace);
                return "fail";
            }


            string sign = GenSignature(string.Format("{0}{1}true", callID, easemobEvents_ReturnKey));
            return string.Format(easemobEvents_ReturnFormat, callID, sign);
        }

        


        //{"timestamp":1479462618459,"chat_type":"chat","from":"zmjdappuser4679536","to":"kevincai","payload":{"bodies":[{"type":"txt","msg":"特阿泰"}],"ext":{"weichat":{"originType":"webim"}}},"msg_id":"265849105899585940","callId":"zmjiudian#zmjd100_265849105899585940","eventType":"chat","security":"081f5c3f4c6599f557ff1f4658282887"}
        //{"timestamp":1479462679195,"chat_type":"chat","from":"kefu001_callback","to":"kevincai","payload":{"bodies":[{"type":"txt","msg":"小测一下"}],"ext":{"weichat":{"visitor":{"source":"weixin","msgId":"6354243809503604298","openid":"oUyJ9uPSfK5EyDonHpbZ9WtWDqGM","mp":"gh_00367eba4731","userNickname":"周末酒店订阅号 蔡春来。。 oUyJ9uPSfK5EyDonHpbZ9WtWDqGM"}}}},"msg_id":"265849366810464896","callId":"zmjiudian#zmjd100_265849366810464896","eventType":"chat","security":"6b3ddeea5570ec784d5b97d51d1e95dd"} 
        //{"timestamp":1483611517192,"chat_type":"chat","from":"kevincai","to":"kefu001_callback","payload":{"bodies":[{"msg":"这就试一下","type":"txt"}],"ext":{"weichat":{"msgId":"cd6a1c3d-7110-4f8b-8f7b-b27df7517a58","originType":null,"visitor":{"source":"weixin","openid":"oUyJ9uPSfK5EyDonHpbZ9WtWDqGM","mp":"gh_00367eba4731","userNickname":"周末酒店订阅号 蔡春来。。 oUyJ9uPSfK5EyDonHpbZ9WtWDqGM","trueName":null,"sex":null,"qq":null,"email":null,"phone":null,"companyName":null,"description":null,"weixin":null,"tags":null,"callback_user":"kefu001_callback","gr_user_id":null},"agent":{"avatar":"//kefu-prod-avatar.img-cn-hangzhou.aliyuncs.com/avatar/10249/fb77be42-1fa8-46db-83b1-21c1e0797d23@40-35-300-300a|300h_300w|.png","userNickname":null},"queueId":null,"queueName":null,"agentUsername":null,"ctrlType":null,"ctrlArgs":null,"event":null,"metadata":null,"callcenter":null,"language":null,"service_session":null,"html_safe_body":{"type":"txt","msg":"这就试一下"},"msg_id_for_ack":null,"ack_for_msg_id":null}}},"msg_id":"283668490328474724","callId":"zmjiudian#zmjd100_283668490328474724","eventType":"chat","security":"9a6fd4d3d467dc5ffe1fcafa5fce0f93"}
        public static string DealEasemobEvents(string json)
        {
            string callID = "";
            try
            {
               EasemobEventsEntity e = JsonConvert.DeserializeObject< EasemobEventsEntity>(json);
               if (e.from == "kefu001_callback" || e.to == "kefu001_callback")
               {
                   //微信对话
                   foreach (var msg in e.payload.bodies)
                   {

                       switch (msg.type)
                       {
                           case "txt":
                               #region 新消息（ServiceSessionMessageEvent）事件 
                               #endregion
                               StringBuilder sb = new StringBuilder();
                               foreach (var body in e.payload.bodies)
                               {
                                   sb.AppendLine(body.msg);
                               }

                               MessageEntity entity = new MessageEntity
                                   {
                                       messageType = TransEasemobMsgType(e.from, e.to),
                                       SourceType = "app",
                                       Service_Session_ID = e.payload.ext.weichat.visitor.mp + ":" + e.payload.ext.weichat.visitor.openid,
                                       UserID = 0,
                                       FromName = e.from,
                                       msg = sb.ToString(),
                                       CreateTime = DateTime.Now
                                   };

                                   if (!(entity.msg == null || entity.msg.StartsWith("WHAPP")))  //对于服务器端发出的信息不需要处理。不然会出现重复的情况。
                                   {
                                       MagiCallAdapter.MagiCallClientMessage(entity);
                                   } 

                               break;
                           case "img":
                               #region 图片消息
                               #endregion

                               break;
                       }
                   }
               }
               else
               {
                   foreach (var msg in e.payload.bodies)
                   {

                       switch (msg.type)
                       {
                           case "txt":
                               #region 新消息（ServiceSessionMessageEvent）事件
                             //{
                             //   "timestamp":1479462618459,
                             //   "chat_type":"chat",
                             //   "from":"zmjdappuser4679536",
                             //   "to":"kevincai",
                             //   "payload":{
                             //       "bodies":[
                             //           {
                             //               "type":"txt",
                             //               "msg":"特阿泰"
                             //               }],
                             //       "ext":{
                             //           "weichat":{
                             //               "originType":"webim"
                             //               }}},
                             //       "msg_id":"265849105899585940",
                             //       "callId":"zmjiudian#zmjd100_265849105899585940",
                             //       "eventType":"chat",
                             //       "security":"081f5c3f4c6599f557ff1f4658282887"
                             //   }
                               #endregion
                               StringBuilder sb = new StringBuilder();
                               foreach (var body in e.payload.bodies)
                               {
                                   sb.AppendLine(body.msg);
                               }

                                  long tempUserID = ParseUserID( e.from, e.to );

                               if( tempUserID > 0)
                               {
                                   MessageEntity entity = new MessageEntity
                                   {
                                       messageType = TransEasemobMsgType(e.from, e.to ),
                                       SourceType ="app",
                                       Service_Session_ID = "",
                                       UserID = tempUserID,
                                       FromName = e.from,
                                       msg = sb.ToString() ,
                                       CreateTime = DateTime.Now
                                   };

                                   if (!(entity.msg == null || entity.msg.StartsWith("WHAPP")))  //对于服务器端发出的信息不需要处理。不然会出现重复的情况。
                                   {
                                       MagiCallAdapter.MagiCallClientMessage(entity);
                                   }
                               }


                            ///   {"timestamp":1480566088006,
                            ///   "chat_type":"chat",
                            ///   "from":"kevincai",
                            ///   "to":"zmjdappuser4680206",
                            ///   "payload":{"bodies":[
                            ///     {"msg":"我其","type":"txt"}],
                            ///     "ext":{
                            ///         "weichat":{
                            ///                 "msgId":"8cee341b-aa59-4836-b2de-fa6bef83a476",
                            ///                 "originType":"webim",
                            ///                 "visitor":null,
                            ///                 "agent":{"avatar":"//kefu-prod-avatar.img-cn-hangzhou.aliyuncs.com/avatar/10249/fb77be42-1fa8-46db-83b1-21c1e0797d23@40-35-300-300a|300h_300w|.png","userNickname":null},"queueId":null,"queueName":null,"agentUsername":null,"ctrlType":null,"ctrlArgs":null,"event":null,"metadata":null,"callcenter":null,"language":null,"service_session":null,"html_safe_body":{"type":"txt","msg":"我其"},"msg_id_for_ack":null,"ack_for_msg_id":null}}},"msg_id":"270588471586849860","callId":"zmjiudian#zmjd100_270588471586849860","eventType":"chat","security":"09387c06afbea6901d2a4159d027ab7c"}

                               break;
                           case "img":
                               #region 图片消息
                               //                            {
                               //  "timestamp": 1438837366089,
                               //  "chat_type": "chat",
                               //  "from": "kefu001", //在环信移动客服中设置的关联IM账号
                               //  "to": "kefu001_callback", //用于开通实时消息回调功能的IM账号
                               //  "payload": {
                               //      "bodies": [
                               //          {
                               //              "filename": "8899a801f289.jpg",
                               //              "size": {
                               //                  "width": 750,
                               //                  "height": 500
                               //              },
                               //              "secret": "",
                               //              "type": "img",
                               //              "url": “https://a1.easemob.com/easemob-demo/xuzhengli/chatfiles/5bd56cb0-3bf8-11e5-b668-431280c88f48"  //坐席回复的图片地址
                               //          }
                               //      ],
                               //      "ext": {
                               //          "weichat": {
                               //              "visitor": {
                               //                  "mp": "gh_9898dc86a1ea",
                               //                  "openid": "oAGnbt89fTgfzOlybTmAh_7s3Z_g",
                               //                  "userNickname": "微信粉丝昵称",
                               //                  "source": "weixin"
                               //              }
                               //          }
                               //      }
                               //  },
                               //  "msg_id": "91364975632712188",
                               //  "callId": "easemob-demo#xuzhengli_91364975632712188",
                               //  "eventType": "chat",
                               //  "security": "27db399e77d4f422e50f5b529820ef64"
                               //}
                               #endregion

                               break;
                       }
                   }
               }
                callID = e.callId;
            }
            catch (Exception err)
            {
                Log.WriteMagiCallLog("E", "easemobEvents:" + err.Message + err.StackTrace );
                return "fail";
            }


            string sign = GenSignature(string.Format("{0}{1}true", callID, easemobEvents_ReturnKey));
            return string.Format(easemobEvents_ReturnFormat, callID, sign);
        }

        private static long ParseUserID(string from, string to)
        {
            long tempUserID = 0;

            if (from == MagiCallKeFuUserName)
            {
                long.TryParse(to.Replace("zmjdappuser", ""), out tempUserID);
            }
            else
            {
                long.TryParse(from.Replace("zmjdappuser", ""), out tempUserID);

            }
            return tempUserID;
        }

        public static MessageType TransEasemobMsgType(string from, string to)
        {
            MessageType msgType = MessageType.UserWord;
            if (from == MagiCallKeFuUserName || to == MagiCallWeiXinChannnelUserName)
                    msgType = MessageType.KeFuWord; 

            return msgType;
        }

        //   //签名。格式如下: MD5（callId+约定的key+"true"），约定key为654321 

        public static string GenSignature(string toSign)
        {
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] data = System.Text.Encoding.UTF8.GetBytes(toSign);
            //加密Byte[]数组
            byte[] result = md5.ComputeHash(data);
            //将加密后的数组转化为字段
            //return  System.Text.Encoding.Unicode.GetString(result);
            return Convert.ToBase64String(result);
        }

        /// <summary>
        /// 接收环信推送信息，包括客服状态待
        /// </summary>
        /// <returns></returns>
        [System.Web.Http.HttpPost]
        [System.Web.Http.HttpGet]
        public string easemobMsgPush()
        {

            HttpContent content = Request.Content;

            string str = content.ReadAsStringAsync().Result;

            //if (str.Length == 0)
            //{
            //    str = "test!";
            //}

            Log.WriteMagiCallLog("P", str);


            MagiCallService.easemobMsgPush(str);

            return "success";
        }
   

        [HttpGet]
        public string GetEasemobToken()
        {
            return new EMChatAdapter().token;
        }

        
        [HttpPost]
        public int MagiCallClientHeart( MessageEntity message)
        {

            if (dicMagiCallUserHeart.ContainsKey(message.UserID))
            {
                dicMagiCallUserHeart[message.UserID] = DateTime.Now;
            }
            else
            {
                dicMagiCallUserHeart.Add(message.UserID, DateTime.Now);
            }

            MagiCallService.MagiCallClientHeart(message.UserID);

            return 0;
        }


        static long mLastDialogItemID = 0;
        static long LastDialogItemID
        {
            get
            {
                if (mLastDialogItemID == 0)
                {
                    mLastDialogItemID = long.Parse(JobAssistantAdapter.GetParameter("MagiCallAppNotice").First().ConfigValue);
                }
                return mLastDialogItemID;
            }
            set
            {
                mLastDialogItemID = value;
                JobAssistantAdapter.SetParameter("MagiCallAppNotice", "LastDialogItemID", value.ToString());
            }
        }

        [System.Web.Http.HttpGet]
        public string AutoSendNoticeToUser()
        {
            List<DialogItemsEntity> itemList = MagiCallService.GetLastDialogItems(LastDialogItemID);

            if (itemList.Count > 0)
            {
                LastDialogItemID = itemList.Last().IDX;

                List<long> hadSendNoticeUserList = new List<long>();

                foreach ( DialogItemsEntity item in itemList)
                {
                   // if (item.UserID != 4539575) continue;
                    if (item.UserID == 0) continue;
                    if (item.ItemType == (int)DialogItemType.KeFuWord)
                    {
                        if (MagiCallService.IsMagiCallUserOnLine(item.UserID) == false)
                        {
                            if (hadSendNoticeUserList.Contains(item.UserID) == false)
                            {
                                hadSendNoticeUserList.Add(item.UserID);

                                string deaultMsg = "MagiCall有新消息，请点击查看。";
                                string defaultFrom = "周末酒店";

                                var magicallActionUrl = "http://www.zmjiudian.com/MagiCall/MagiCallClient";

                                string gotoUrl = "whotelapp://www.zmjiudian.com/gotopage?url=" + System.Web.HttpUtility.UrlEncode(magicallActionUrl + (magicallActionUrl.IndexOf("?") >= 0 ? "&" : "?") + "userid={userid}&realuserid=1");


                                MessageAdapter.SendAppNotice(new SendNoticeEntity()
                                {
                                    actionUrl = gotoUrl,
                                    title = "周末酒店MagiCall",
                                    msg = deaultMsg,
                                    appType = 0,
                                    from = defaultFrom,
                                    noticeType = ZMJDNoticeType.magicall,
                                    userID = item.UserID
                                });

                                MagiCallService.MagiCallClientMessage(new  MessageEntity { CreateTime = DateTime.Now, messageType = MessageType.SystemAction, msg = "系统发送APP提醒", UserID = item.UserID });
                            }
                        }
                    }
                }
            }

            return itemList.Count.ToString();
        }


        [HttpPost]
        public bool IsMagiCallUserOnLine( MessageEntity message)
        {
            return MagiCallService.IsMagiCallUserOnLine(message.UserID);
        }

        [HttpPost]
        public bool MagiCallClientMessage( MessageEntity message)
        {
            return MagiCallAdapter.MagiCallClientMessage(message);
        }

        [HttpGet]
        public bool IsMagiCallUser(long userId)
        {
            bool isMagiCallUser = userId == 0 ? false : AccountAdapter.HasUserPriviledge(userId, HJD.AccountServices.Entity.PrivilegeEnums.UserPrivilege.MagiCallUser);
            return isMagiCallUser;
        }

        /// <summary>
        /// 发送magic消息
        /// </summary>
        /// <param name="txtEntity"></param>
        /// <returns></returns>
        [HttpPost]
        public int SendMagicallNotice(MagiCallTxtMsgEntity txtEntity)
        {
            var isTest = AccountAdapter.HasUserPriviledge(txtEntity.userID, HJD.AccountServices.Entity.PrivilegeEnums.UserPrivilege.InnerTest);
            if (isTest)
            {
                var magicallActionUrl = "http://www.zmjiudian.com/MagiCall/MagiCallClient";
                var iOSActionUrl = "whotelapp://www.zmjiudian.com/gotopage?url=" + magicallActionUrl + (magicallActionUrl.IndexOf("?") >= 0 ? "&" : "?") + "userid={userid}&realuserid=1";
                var androidActionUrl = ADAdapter.FormatActionURL(magicallActionUrl);
                var umengExtra = new UmengMessageExtra()
                {
                    action = txtEntity.appType == 1 ? iOSActionUrl : txtEntity.appType == 2 ? androidActionUrl : "",
                    type = "comment",
                    bgColor = "red",
                    btnName = "去看看",
                    isShowInApp = true,
                    timeElapse = 15f
                };
                var title = "你收到一条来自客服的Magicall";
                switch (txtEntity.appType)
                {
                    case 1:
                        MessageAdapter.SendMsgByUmeng(title, txtEntity.msg, txtEntity.userID, umengExtra, false);
                        break;
                    case 2:
                        MessageAdapter.SendMsgByUmeng(title, txtEntity.msg, txtEntity.userID, umengExtra, true);
                        break;
                    default:
                        MessageAdapter.SendMsgByUmeng(title, txtEntity.msg, txtEntity.userID, umengExtra, false);
                        MessageAdapter.SendMsgByUmeng(title, txtEntity.msg, txtEntity.userID, umengExtra, true);
                        break;
                }
            }
            return 0;
        }
    }
}