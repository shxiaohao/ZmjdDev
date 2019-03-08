using HJD.AccessService.Contract.Model;
using HJD.AccessService.Contract.Params;
using HJD.AccessService.Implement;
using HJD.AccessService.Implement.Helper;
using HJD.AccessServiceTask.Entity;
using HJD.AccessServiceTask.Helper;
using HJD.AccessServiceTask.Job.Helper;
using HJD.WeixinService.Contract;
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

namespace HJD.AccessServiceTask.Job
{
    /// <summary>
    /// 发送模板消息（遛娃指南无锡服务号）
    /// </summary>
    public class SendServiceTemplateMsgLiuwaWX_Job : BaseJob
    {
        //遛娃指南无锡服务号
        static int weixinAcountId = 14;

        public SendServiceTemplateMsgLiuwaWX_Job()
            : base("SendServiceTemplateMsgLiuwaWX_Job")
        {
            Log(string.Format("Start Job [SendServiceTemplateMsgLiuwaWX_Job]"));

            try
            {
                RunJob();
            }
            catch (Exception ex)
            {
                Log("Job [SendServiceTemplateMsgLiuwaWX_Job] Error:" + ex.Message);
            }


            Log(string.Format("Stop Job [SendServiceTemplateMsgLiuwaWX_Job]"));
        }

        private void RunJob()
        {
            var weixinApi = new Weixin();

            //模板消息
            WeixinTempMsgEntity msg = new WeixinTempMsgEntity();
            msg.WeixinAcount= weixinAcountId;
            msg.OpenId= "";

            #region 活动状态变更通知（产品XXXX）

            //msg.TempId = "DLZiRIC57xYsf7FnszOOlqxJx0s5GtYFfuRIzMGMZtU";     //待办事项
            //msg.Url = Config.WeixinTemplateMsgLink;
            //msg.First = Config.WeixinTemplateMsgTitle;
            //msg.Remark = Config.WeixinTemplateMsgRemark;
            //msg.List = new List<string>();
            //msg.List.Add(Config.WeixinTemplateMsgName);
            //msg.List.Add("新店开业");
            //msg.List.Add(Config.WeixinTemplateMsgDesc);
            //msg.List.Add(Config.WeixinTemplateMsgDesc2);

            #endregion

            #region 待办事项状态变更通知（产品XXXX）

            msg.TempId = "kHqp6Xyp7ctAs4lYB5gz2LLES8f0ZfyOLC3jWgV-TPc";     //待办事项
            msg.Url = Config.WeixinTemplateMsgLink;
            msg.First = Config.WeixinTemplateMsgTitle;
            msg.Remark = Config.WeixinTemplateMsgRemark;
            msg.List = new List<string>();
            msg.List.Add(Config.WeixinTemplateMsgName);
            //msg.List.Add("团购中");
            msg.List.Add("库存有限，先到先得");
            msg.List.Add(Config.WeixinTemplateMsgDesc);
            msg.List.Add(Config.WeixinTemplateMsgDesc2);

            #endregion

            if (!string.IsNullOrEmpty(Config.WeixinTemplateMsgTestOpenid))
            {
                Console.WriteLine("指定了测试Openid：" + Config.WeixinTemplateMsgTestOpenid);
                Log("指定了测试Openid：" + Config.WeixinTemplateMsgTestOpenid);

                msg.OpenId = Config.WeixinTemplateMsgTestOpenid;//"oHGzlw-sdix9G__-S4IzfTsYRqC8";
                var _send = weixinApi.SendTemplateMessage(msg);

                Console.WriteLine(string.Format("结果：{0}", _send));
                Log(string.Format("结果：{0}", _send));
            }
            else
            {
                var num = 0;

                //下载尚旅周末的所有关注用户信息
                var nextOpenid = GetNextOpenid();

                var weixinAllUser = WeixinApiHelper.GetAllWeixinSubscribeUser(weixinApi.GetWeixinTokenByCode(weixinAcountId), nextOpenid);

                while (weixinAllUser != null && weixinAllUser.Data != null && weixinAllUser.Data.Openid != null && weixinAllUser.Data.Openid.Length > 0)
                {
                    Console.WriteLine(string.Format("【{0}】", num));
                    Log(string.Format("【{0}】", num));

                    Console.WriteLine(string.Format("GetAllWeixinSubscribeUser Count:{0}", weixinAllUser.Data.Openid.Length));
                    Log(string.Format("GetAllWeixinSubscribeUser Count:{0}", weixinAllUser.Data.Openid.Length));

                    for (int i = 0; i < weixinAllUser.Data.Openid.Length; i++)
                    {
                        var openidObj = weixinAllUser.Data.Openid[i];
                        if (!string.IsNullOrEmpty(openidObj))
                        {
                            try
                            {
                                msg.OpenId = openidObj;// "oHGzlw-sdix9G__-S4IzfTsYRqC8";// openidObj;
                                var _send = weixinApi.SendTemplateMessage(msg);

                                Console.WriteLine(string.Format("{0} 账号：{1} 结果：{2}", num, msg.OpenId, _send));
                                Log(string.Format("{0} 账号：{1} 结果：{2}", num, msg.OpenId, _send));

                                num++;
                            }
                            catch (Exception exsend)
                            {

                            }
                        }
                    }

                    SetNextOpenid(weixinAllUser.NextOpenid);
                    nextOpenid = GetNextOpenid();
                    weixinAllUser = WeixinApiHelper.GetAllWeixinSubscribeUser(weixinApi.GetWeixinTokenByCode(weixinAcountId), nextOpenid);
                }

                Console.WriteLine("GetAllWeixinSubscribeUser is null:nextOpenid:" + nextOpenid);
                Log("GetAllWeixinSubscribeUser is null:nextOpenid:" + nextOpenid);   
            }
        }

        string nextopenidFile = @"D:\Log\Config\Weixin\WeixinAllUser_NextOpenid_liuwazhinan_wx.txt";
        private string GetNextOpenid()
        {
            var nextopenid = "";
            if (File.Exists(nextopenidFile))
            {
                try
                {
                    nextopenid = File.ReadAllText(nextopenidFile);
                    Console.WriteLine("【NextOpenid】:" + nextopenid);
                    Log("【NextOpenid】:" + nextopenid);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("读取NextOpenid错误：" + ex.Message);
                    Log("读取NextOpenid错误：" + ex.Message);
                }
            }

            return nextopenid;
        }

        private void SetNextOpenid(string nexropenid)
        {
            File.WriteAllText(nextopenidFile, nexropenid);
            Console.WriteLine("NextOpenid已经存储至本地！" + nexropenid);
            Log("NextOpenid已经存储至本地！" + nexropenid);
        }
    }
}
