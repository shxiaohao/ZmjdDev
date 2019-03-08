using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using HJDAPI.Models.JiGuangSMS;
using HJDAPI.Controllers;
using System.Collections.Generic;

namespace HJDAPI.Tests.Controllers
{
    [TestClass]
    public class JSMSTest
    {

        static JiGuangSMSController jsmsClient = new JiGuangSMSController();

        [TestMethod]
        public  void Main() {

            SendCode();//运行结果：{"msg_id":"670400304711"}
                       //SendVoiceCode();//发送语音验证码，余额不足
                       //IsCodeValid();
                       //SendTemplateMessage();
                       /*SendTemplateMessageList();*///{"failure_count":0,"recipients":[{"mobile":"15000970557","msg_id":"670613652208"},{"mobile":"18721025207","msg_id":"670613652209"}],"success_count":2}
            Console.ReadLine();
        }



        /// <summary>
        /// 发送文本验证码短信。
        /// <see cref="https://docs.jiguang.cn/jsms/server/rest_api_jsms/#api_1"/>
        /// </summary>
        private static void SendCode()
        {
            HttpResponse httpResponse = jsmsClient.SendCode("18721025207", 159299);
            Console.WriteLine(httpResponse.Content);
        }

        /// <summary>
        /// 发送语音验证码短信。
        /// </summary>
        private static void SendVoiceCode()
        {
            VoiceCode vc = new VoiceCode();
            vc.Mobile = "18721025207";
            vc.Code = 12345;
            vc.Languarge = 0;
            vc.Life = 120;

            HttpResponse httpResponse = jsmsClient.SendVoiceCode(vc);
            Console.WriteLine(httpResponse.Content);
        }


        /// <summary>
        /// 判断验证码是否有效
        /// </summary>
        private static void IsCodeValid()
        {

            string msgId = "670400304711";
            string code = "804822";

            HttpResponse httpResponse = jsmsClient.IsCodeValid(msgId, code);
            Console.WriteLine(httpResponse.Content);

        }

        /// <summary>
        /// 发送单条模板短信
        /// </summary>
        private static void SendTemplateMessage()
        {

            HttpResponse httpResponse = jsmsClient.SendTemplateMessage(new TemplateMessage
            {
                Mobile = "18721025207",
                TemplateId = 159352,
                TemplateParameters = new Dictionary<string, string> {

                    { "Name","吕勇路"},
                    { "TypeName","lv"},
                    { "Count","10"},
                    { "No","123456"}
                }
            });
            Console.WriteLine(httpResponse.Content);
        }


        /// <summary>
        /// 批量发送模板短信
        /// </summary>
        private static void SendTemplateMessageList()
        {


            HttpResponse httpResponse = jsmsClient.SendTemplateMessage(new List<TemplateMessage>
            {
                new TemplateMessage{
                Mobile= "15000970557",
                TemplateId = 159352,
                TemplateParameters = new Dictionary<string, string> {

                    { "Name","吕勇路"},
                    { "TypeName","lv"},
                    { "Count","10"},
                    { "No","123456"}
                }

                },

                new TemplateMessage{
                Mobile= "18721025207",
                TemplateId = 159352,
                TemplateParameters = new Dictionary<string, string> {

                    { "Name","英雄"},
                    { "TypeName","jq"},
                    { "Count","10"},
                    { "No","123456"}
                }

                }

            });
            Console.WriteLine(httpResponse.Content);

        }







    }
}
