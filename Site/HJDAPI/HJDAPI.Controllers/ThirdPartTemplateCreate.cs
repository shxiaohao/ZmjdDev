using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HJDAPI.Controllers;
using HJDAPI.Models.JiGuangSMS;
using Newtonsoft.Json;
using HJD.Framework.WCF;

namespace HJDAPI.Controllers
{





    public class ThirdPartTemplateCreate
    {
        public static HJD.HotelManagementCenter.IServices.ISMSService SMSService = ServiceProxyFactory.Create<HJD.HotelManagementCenter.IServices.ISMSService>("ISMSService");
        /// <summary>
        /// 创建短信模板
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        public static int CreateMessageTemplate(int dbtempid, int templateType, string templateContent)
        {


            TemplateMessage template = new TemplateMessage();
            template.Content = templateContent;
            template.Type = templateType;


            JiGuangSMSController js = new JiGuangSMSController();

            HttpResponse httpResponse = js.CreateMessageTemplate(template);


            CrateTemplateResult tempid = JsonConvert.DeserializeObject<CrateTemplateResult>(httpResponse.Content);//拿到返回的模板ID

            if (tempid.temp_id != 0)//接收到短信模板id
            {

                var result = SMSService.ThirdPartSMSTemplatetempID(tempid.temp_id, dbtempid, templateType);
                return result;//进入审核，返回插入第三方表ID

            }
            else
            {

                return 0;//创建短信模板失败

            }



        }


        /// <summary>
        /// 审核不通过，修改后重新提交
        /// </summary>
        /// <returns></returns>
        public static int AlterMessageTemplate(int tempid, int templateType, string templateContent)
        {

            TemplateMessage template = new TemplateMessage();
            template.TemplateId = tempid;
            template.Content = templateContent;
            template.Type = templateType;


            JiGuangSMSController js = new JiGuangSMSController();

            HttpResponse httpResponse = js.UpdateMessageTemplate(template);


            CrateTemplateResult dbtempid = JsonConvert.DeserializeObject<CrateTemplateResult>(httpResponse.Content);//拿到返回的模板ID

            if (dbtempid.temp_id != 0)//说明已经请求成功，等待审核
            {
                var result = SMSService.IsThirdPartSMSTemplatePass(dbtempid.temp_id, 0, "待审核");//重置审核状态
                return result;
            }
            else
            {
                var result = SMSService.IsThirdPartSMSTemplatePass(tempid, 0, "待审核");//重置审核状态
                return 0;
            }

        }





        /// <summary>
        /// 手动查询短信模板审核状态
        /// </summary>
        /// <returns></returns>
        public static int QueryMessageTemplate(int tempid) {

            JiGuangSMSController js = new JiGuangSMSController();

            HttpResponse httpResponse = js.QueryMessageTemplate(tempid);
            QueryTemplateResult dbtempid = JsonConvert.DeserializeObject<QueryTemplateResult>(httpResponse.Content);//拿到返回的模板ID

            int result = 0;//待审核
            if (dbtempid.temp_id == tempid)
            {

                if (dbtempid.status == 1)
                {

                    result = SMSService.IsThirdPartSMSTemplatePass(dbtempid.temp_id, 1, "审核通过");//重置审核状态
                    return 1;
                }
                if (dbtempid.status == 2)
                {

                    result = SMSService.IsThirdPartSMSTemplatePass(dbtempid.temp_id, 2, "审核不通过");//重置审核状态
                    return 2;
                }

                return 0;



            }
            else {

                return -1;
            }



        }





    }

}
