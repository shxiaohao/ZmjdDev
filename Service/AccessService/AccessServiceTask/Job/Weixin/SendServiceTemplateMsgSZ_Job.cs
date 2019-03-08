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
    /// 下载指定微信号的所有关注用户信息
    /// </summary>
    public class SendServiceTemplateMsgSZ_Job : BaseJob
    {
        //苏州服务号
        static int weixinAcountId = 8;

        public SendServiceTemplateMsgSZ_Job()
            : base("SendServiceTemplateMsgSZ_Job")
        {
            Log(string.Format("Start Job [SendServiceTemplateMsgSZ_Job]"));

            try
            {
                RunJob();
            }
            catch (Exception ex)
            {
                Log("Job [SendServiceTemplateMsgSZ_Job] Error:" + ex.Message);
            }


            Log(string.Format("Stop Job [SendServiceTemplateMsgSZ_Job]"));
        }

        private void RunJob()
        {
            var weixinApi = new Weixin();

            //模板消息
            WeixinTempMsgEntity msg = new WeixinTempMsgEntity();
            msg.WeixinAcount= weixinAcountId;
            msg.OpenId= "";
            msg.TempId = "GfDrV68IpStyasY5UidIMDzUCe4U4mValpZIFiUQ-Sw";
            msg.Url = Config.WeixinTemplateMsgLink;         // "http://www.zmjiudian.com/coupon/product/2405?CID=210";
            msg.First = Config.WeixinTemplateMsgTitle;      // "您有一项粉丝福利待处理！";
            msg.Remark = Config.WeixinTemplateMsgRemark;    // ">>快戳这里，马上处理！";
            msg.List = new List<string>();
            msg.List.Add(Config.WeixinTemplateMsgDesc);     //msg.List.Add("苏州乐园四季悦温泉仅￥29（原价198元）！");
            msg.List.Add(DateTime.Now.ToString("yyyy-MM-dd"));

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

                //下载周末酒店zmjiudian的所有关注用户信息
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

        string nextopenidFile = @"D:\Log\Config\Weixin\WeixinAllUser_NextOpenid_SZ.txt";
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
