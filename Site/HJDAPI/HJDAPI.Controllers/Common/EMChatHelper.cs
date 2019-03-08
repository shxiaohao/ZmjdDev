using HJD.AccountServices.Entity;
using HJD.Framework.WCF;
using HJD.HotelManagementCenter.Domain;
using HJD.HotelManagementCenter.IServices;
using HJDAPI.Controllers.Adapter;
using HJDAPI.Models;
using MagiCallService.Contracts.Model.Dialog;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.Controllers.Common
{
    public class EMChatHelper
    {
        public static EMChatAdapter ChatAdapter = new EMChatAdapter();
        public static IHotelService HotelService = ServiceProxyFactory.Create<IHotelService>("HMC_IHotelService");
        public static string SendTxtMessageToUser(MagiCallTxtMsgEntity m)
        {
            string to = GenUserID(m.userID);

            if (string.IsNullOrEmpty(m.from))
                m.from = "kevincai"; //默认 

            if (!m.msg.StartsWith("WHAPP:"))
            {
                m.msg = "WHAPP:" + m.msg;
            }

            return ChatAdapter.SendTxtMessageToUser("users", new List<string> { to }, m.msg, m.from, new Dictionary<string, string>());
        }

        public static string SendPicTxtMessageToUser(MagiCallTxtMsgEntity m)
        {
            string Advantage = string.Empty;//优点
            string ShortComming = string.Empty;//不足
            string PicUrl = string.Empty;//展示图片
            string Score = string.Empty;//得分
            string MinPrice = string.Empty;//最低价
            string[] arrMsg = m.msg.Split(',');
            string HotelName = string.Empty;
            string Currency = string.Empty;

            int hotelid = 0;
            if (arrMsg.Length > 0)
            {
                hotelid = int.Parse(arrMsg[1].ToString());
            }
            if (hotelid > 0)
            {
                HotelEditEntity model = HotelService.GetHotelEditInfo(hotelid);
                Advantage = model.Advantage.Replace("\n", "").Replace(" ","").Replace("\t","").Replace("\r","")?? "";
                ShortComming = model.ShortComming.Replace("\n", "").Replace(" ", "").Replace("\t", "").Replace("\r", "") ?? "";
            }
            HotelItem6 hoteldetailmodel = new HotelController().GetSimpleHotelDetail(hotelid, "", "", "ios", 0);
            int currentPrice = 0;
            if (hoteldetailmodel != null)
            {
                foreach (var item in hoteldetailmodel.PackageList)
                {
                    if (currentPrice > item.VIPPrice)
                    {
                        currentPrice = item.VIPPrice;
                    }
                }
                PicUrl = hoteldetailmodel.Pics.Count > 0 ? hoteldetailmodel.Pics[0].ToString() : "";
                Score = hoteldetailmodel.Score.ToString();
                MinPrice = currentPrice == 0 ? Math.Ceiling(hoteldetailmodel.MinPrice).ToString() : currentPrice.ToString();
                HotelName = hoteldetailmodel.HotelName;
                Currency = hoteldetailmodel.Currency;
            }
            List<string> list = arrMsg.ToList();
            list.Add(PicUrl);
            list.Add(Score);
            list.Add(MinPrice);
            list.Add(Currency);
            list.Add(Advantage);
            list.Add(ShortComming);
            //Advantage = "非常干净，酒店设施很齐全，装修很大气朋友推荐来的";
            //ShortComming = "非常不干净，酒店设施很不齐全，装修不大气朋友不推荐来的";
            //StringBuilder sb = new StringBuilder();
            //sb.AppendFormat("<table>");
            //sb.AppendFormat(@"<tr><tr><td width=30% valign=top><a class=a1 href={5}><img style=display:inline-block src={0} width=60px  height=60px/></a></td><td width=70% style=font-weight:500><a class=a1 href={5}><p>{1}</p><p class=linefont><span class=s1>{2}{3}</span> 起 <span>{4}/5分</span></p></a></td></tr>", PicUrl, HotelName, Currency, MinPrice, Score, "whotelapp://www.zmjiudian.com/hotel/" + hotelid.ToString() + "/packages");
            //if (!string.IsNullOrEmpty(Advantage) || !string.IsNullOrEmpty(ShortComming))
            //{
            //    Advantage = Advantage.Length > 17 ? Advantage.Substring(0, 17) + "..." : Advantage;
            //    ShortComming = ShortComming.Length > 17 ? ShortComming.Substring(0, 17) + "..." : ShortComming;
            //    sb.AppendFormat(@"<tr><td colspan=2><hr class=hr1></td></tr>", Advantage);
            //    if (!string.IsNullOrEmpty(Advantage))
            //    {
            //        sb.AppendFormat(@"<tr><td colspan=2><span class=zmjd-iconfont>&#xe631;</span>:{0}</td></tr>", Advantage);
            //    }
            //    if (!string.IsNullOrEmpty(ShortComming))
            //    {
            //        sb.AppendFormat(@"<tr><td colspan=2><span class=zmjd-iconfont>&#xe632;</span>:{0}</td></tr>", ShortComming);
            //    }
            //}
            //sb.Append("</table>");
            m.msg = string.Join(",", list);
            string to = GenUserID(m.userID);
            if (string.IsNullOrEmpty(m.from))
                m.from = "kevincai"; //默认 

            if (!m.msg.StartsWith("WHAPP:"))
            {
                m.msg = "WHAPP:" + m.msg;
            }

            return ChatAdapter.SendPicTxtMessageToUser("users", new List<string> { to }, m.msg, m.from, new Dictionary<string, string>());
        }



        static long mLastTimeStamp = 0;
        static string MagiCallLogPath = System.Configuration.ConfigurationManager.AppSettings["MagiCallLogPath"];
        static long lastTimeStamp
        {
            get
            {
                if (mLastTimeStamp == 0)
                {
                    mLastTimeStamp = long.Parse(JobAssistantAdapter.GetParameter("MagiCall").First().ConfigValue);
                    //if (File.Exists(lastTimeStampFilePath))
                    //{
                    //    mLastTimeStamp = long.Parse(File.ReadAllText(lastTimeStampFilePath));
                    //}
                }
                return mLastTimeStamp;

            }
            set
            {
                mLastTimeStamp = value;
                JobAssistantAdapter.SetParameter("MagiCall", "LastTimeStamp", value.ToString());
            }
        }

        public static string CheckChatLogs()
        {
            return "0"; //不再从“环信支持 APP 把聊天记录通过 REST 接口导出。”获取记录，因为这样没有客服的信息

            //string filePath = MagiCallLogPath;
            //string chatLogs = EMChatHelper.GetChatLogs(lastTimeStamp);
            //try
            //{

            //    if (chatLogs.IndexOf("\"count\" : 0") > 0 || chatLogs.IndexOf("Too Many Requests") > 0)
            //    {
            //        return "0";
            //    }
            //    else
            //    {

            //        File.WriteAllText(filePath + "Log" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt", chatLogs);

            //        HJDAPI.Models.MCChatMessage mc = JsonConvert.DeserializeObject<HJDAPI.Models.MCChatMessage>(chatLogs);


            //        if (chatLogs.Length > 0 && mc.Entities.Count() > 0)
            //        {
            //            long theLastTimeStamp = mc.Entities.Last().Timestamp;
            //            if (theLastTimeStamp > lastTimeStamp)
            //            {



            //                lastTimeStamp = theLastTimeStamp;

            //                MessageEntity entity = new MessageEntity();
            //                foreach (HJDAPI.Models.MCChatMessage.MsgEntity e in mc.Entities)
            //                {
            //                    if (e.To == "kevincai")
            //                    {
            //                        entity = new MessageEntity
            //                        {
            //                            messageType = MessageType.UserWord,
            //                            UserID = long.Parse(e.From.Replace("zmjdappuser", ""))   //"from" : "zmjdappuser4522272",
            //                        };
            //                    }
            //                    else
            //                    {
            //                        entity = new MessageEntity
            //                        {
            //                            messageType = MessageType.KeFuWord,
            //                            UserID = long.Parse(e.To.Replace("zmjdappuser", ""))   //"from" : "zmjdappuser4522272",
            //                        };
            //                    }

            //                    entity.msg = e.Payload.Bodies.First().Msg;
            //                    entity.CreateTime = CommMethods.GetTime(e.Created);

            //                    if (entity.msg == null || entity.msg.StartsWith("WHAPP"))  //对于服务器端发出的信息不需要处理。不然会出现重复的情况。
            //                    {
            //                        continue;
            //                    }
            //                    else
            //                    {
            //                        MagiCallAdapter.MagiCallClientMessage(entity);
            //                    }
            //                }

            //            }
            //        }

            //        return mc.Entities.Count().ToString();
            //    }
            //}
            //catch (Exception err)
            //{
            //    Log.WriteLog("CheckChatLogs:" + err.Message + err.StackTrace + chatLogs);
            //    return err.Message + err.StackTrace;
            //}
        }






        public static string GetChatLogs(long lastTimeStamp)
        {
            return ChatAdapter.GetChatLogs(lastTimeStamp);
        }


        public static string GenUserID(long userID)
        {
            return "zmjdappuser" + userID.ToString();
        }
        public static string GenUserPWD(long userID)
        {
            return "zmjdappuserpwd" + userID.ToString();
        }

        public static string AccountCreate(long userID)
        {
            return ChatAdapter.AccountCreate(GenUserID(userID), GenUserPWD(userID), GenMagicallUserNickName(userID));
        }

        public static string AccountUpdateNickName(long userID)
        {
            return ChatAdapter.AccountUpdateNickName(GenUserID(userID), GenMagicallUserNickName(userID));
        }



        private static string GenMagicallUserNickName(long userID)
        {
            User_Info ui = AccountAdapter.GetUserInfoByUserId(userID);

            return ui.NickName + ":" + ui.MobileAccount;
        }


    }
}
