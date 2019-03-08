using HJD.AccessService.Contract.Model;
using HJD.AccessService.Contract.Params;
using HJD.AccessService.Implement;
using HJD.AccessService.Implement.Helper;
using HJD.AccessServiceTask.Entity;
using HJD.AccessServiceTask.Helper;
using HJD.AccessServiceTask.Job.Helper;
using HJD.WeixinServices.Contracts;
using HJDAPI.APIProxy;
using HtmlAgilityPack;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace HJD.AccessServiceTask.Job
{
    /// <summary>
    /// 自定义短信批量发送
    /// </summary>
    public class CustomSmsSendJob : BaseJob
    {
        RabbitMqContext rbmqContext;
        static string queueKey = "CustomSmsSendJob";

        public CustomSmsSendJob()
            : base("CustomSmsSendJob")
        {
            Log(string.Format("Start Job [CustomSmsSendJob]"));

            try
            {
                RunJob();
            }
            catch (Exception ex)
            {
                Log("Job [CustomSmsSendJob] Error:" + ex.Message);
            }


            Log(string.Format("Stop Job [CustomSmsSendJob]"));
        }

        private void RunJob()
        {
            var weixinApi = new Weixin();
            var accessApi = new Access();

            //读取数据
            Console.WriteLine("读取数据：" + @"D:\Log\temp\customSms.txt");
            Log("读取数据：" + @"D:\Log\temp\customSms.txt");
            var smsInfo = File.ReadAllLines(@"D:\Log\temp\customSms.txt", Encoding.UTF8);

            //解析并发送
            Console.WriteLine("准备开始解析&发送操作");
            Log("准备开始解析&发送操作");
            if (smsInfo != null && smsInfo.Length > 0)
            {
                Console.WriteLine("smsInfo.Length:" + smsInfo.Length);
                Log("smsInfo.Length:" + smsInfo.Length);
                for (int i = 0; i < smsInfo.Length; i++)
                {
                    Console.WriteLine(string.Format("【{0}】", i));
                    Log(string.Format("【{0}】", i));

                    var sms = smsInfo[i];
                    Console.WriteLine("Get:" + sms);
                    Log("Get:" + sms);

                    if (!string.IsNullOrEmpty(sms))
                    {
                        var smsList = Regex.Split(sms, "\t");
                        if (smsList.Length > 2)
                        {
                            var smsSnedUrl = "http://bg.zmjiudian.com/api/sms/SendSMS";
                            var phone = smsList[0];
                            var msg = smsList[1];
                            var url = smsList[2];

                            try
                            {
                                //生成短链接
                                Console.WriteLine("生成短链接");
                                var s_url = accessApi.GenShortUrl(-3, HttpUtility.UrlEncode(url));
                                msg = string.Format(msg, s_url);

                                //发送
                                Console.WriteLine("准备发送");
                                var smsObj = new SMSSendEntity();
                                smsObj.phone = phone;
                                smsObj.msg = msg;

                                CookieContainer cc = new CookieContainer();
                                string json = HJD.AccessService.Implement.Helper.DownloadHelper.PostJson(smsSnedUrl, smsObj, ref cc);
                                var back = JsonConvert.DeserializeObject<bool>(json);
                                if (back)
                                {
                                    Console.WriteLine("发送成功：" + phone + "：" + msg);
                                    Log("发送成功：" + phone + "：" + msg);
                                }
                                else
                                {
                                    Console.WriteLine("失败：" + phone + "：" + msg);
                                    Log("失败：" + phone + "：" + msg);
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("报错：" + phone + "：" + ex.Message);
                                Log("报错：" + phone + "：" + ex.Message);
                            }
                        }
                        else
                        {
                            Console.WriteLine("smsList.Length <= 2");
                            Log("smsList.Length <= 2");
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("没有找到待发送数据");
                Log("没有找到待发送数据");
            }
        }
    }

    public class SMSSendEntity
    {
        public string phone { get; set; }
        public string msg { get; set; }
    }
}
