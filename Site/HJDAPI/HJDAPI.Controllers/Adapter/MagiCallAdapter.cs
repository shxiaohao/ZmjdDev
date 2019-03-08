using HJD.Framework.WCF;
using HJDAPI.Controllers.Common;
using HJDAPI.Models;
using MagiCallService.Contracts.Model.Dialog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.Controllers.Adapter
{
    public class MagiCallAdapter
    {
        public static MagiCallService.Contracts.IMagiCallService MagiCallService = ServiceProxyFactory.Create<MagiCallService.Contracts.IMagiCallService>("IMagiCallService");


        /// <summary>
        /// 包装成HTML格式的消息
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static string FormatHTMLMsg(string msg)
        {
            return "html" + "," + msg;
        }

        public static bool MagiCallClientMessage(MessageEntity message)
        {
            try
            {
                FeedbackMessageBaseEntity feedback = MagiCallService.MagiCallClientMessage(message);

                Log.WriteMagiCallLog("MagiCallClientMessage", string.Format("MagiCall :{0} {1}", feedback.UserID, message.msg, FeedbackMessageType.Text, feedback.msg));
                switch (feedback.feedbackType)
                {
                    case FeedbackMessageType.Text:
                        if (message.UserID > 0)
                        {
                            feedback.msg = feedback.msg.Replace("\r\n", "<BR/>").Replace("\n", "<BR/>").Replace("&lt;", "<").Replace("&gt;", ">").Replace("\"", "\\\"");
                            string feedBack = EMChatHelper.SendTxtMessageToUser(new MagiCallTxtMsgEntity { appType = 0, from = "kevincai", msg = "WHAPP:" + feedback.msg, userID = feedback.UserID });
                        }
                        break;
                }
            }
            catch (Exception err)
            {
                Log.WriteMagiCallLog("MagiCallClientMessage", err.Message + err.StackTrace);
            }

            return true;
        }


        public UserCustomerCareInfoEntity GetLastUserCustomerCareInfo(long userID)
        {
            return MagiCallService.GetLastUserCustomerCareInfo(userID);
        }

        public CustomerCareEntity GetCustomerCareByUserID(long UserID)
        {
            return MagiCallService.GetCustomerCareByUserID(UserID);
        }

        //internal void MagiCallClientMessage(MessageEntity entity)
        //{
        //      MagiCallService.MagiCallClientMessage(entity);
        //}
    }
}
