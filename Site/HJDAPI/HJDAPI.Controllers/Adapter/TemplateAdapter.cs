using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HJD.Framework.WCF;
using HJDAPI.Models;
using HJDAPI.Controllers.Common;
using Newtonsoft.Json;

namespace HJDAPI.Controllers.Adapter
{
    public class TemplateAdapter
    {
        public static HJD.HotelManagementCenter.IServices.ISMSService SMSService = ServiceProxyFactory.Create<HJD.HotelManagementCenter.IServices.ISMSService>("ISMSService");

        /// <summary>
        /// 更新审核状态
        /// </summary>
        /// <param name="tempId"></param>
        /// <param name="status"></param>
        /// <param name="refuseReason"></param>
        /// <returns></returns>
        public static int IsThirdPartSMSTemplatePass(int tempId, int status, string refuseReason) {

            Log.WriteLog("模板审核状态更新");
           return  SMSService.IsThirdPartSMSTemplatePass(tempId, status, refuseReason);


        }

        /// <summary>
        /// 更新下行短信状态
        /// </summary>
        /// <param name="tempId"></param>
        /// <param name="status"></param>
        /// <param name="refuseReason"></param>
        /// <returns></returns>
        public static int UpdateThirdTemplateSMSCallMessage(string msgid, string mterrcode)
        {

            Log.WriteLog("下行短信状态更新");
            return SMSService.UpdateThirdTemplateSMSCallMessage(msgid, mterrcode);


        }

        public static HJD.HotelManagementCenter.Domain.TemplateDataEntity GetTempSource(int id)
        {
            HJD.HotelManagementCenter.Domain.TemplateSourceEntity tempSource = HotelAdapter.GetTempSourceById(id);
            HJD.HotelManagementCenter.Domain.TemplateDataEntity tempData = new HJD.HotelManagementCenter.Domain.TemplateDataEntity();
            if (tempSource != null && tempSource.ID > 0)
            {
                tempData.TemplateID = tempSource.ID;
                tempData.TemplateItem = tempSource.TemplateContent;
                tempData.BizType = 3;
                List<HJD.HotelManagementCenter.Domain.TemplateContent> tList = JsonConvert.DeserializeObject<List<HJD.HotelManagementCenter.Domain.TemplateContent>>(tempSource.TemplateContent);
                if (tList.Count > 0 && tList != null)
                {
                    tempData.ContentList = tList.OrderBy(_ => _.IDX).ToList();
                }
            }
            return tempData;
        }



        /// <summary>
        /// 极光上行短信回复
        /// </summary>
        /// <param name="nonce"></param>
        /// <param name="content"></param>
        /// <param name="replyTime"></param>
        /// <param name="phone"></param>
        /// <returns></returns>
        public static int UpdateThirdTemplateReply(string nonce, string content, string replyTime, string phone) {

            return SMSService.JiGuangTemplateReply(nonce, content, replyTime, phone);

        }

    }
}
