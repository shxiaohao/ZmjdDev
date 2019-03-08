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
    public class DownLoadWexinUserInfoJob : BaseJob
    {
        //周末酒店订阅号
        static string weixinAcount = "zmjiudian";
        
        //周末酒店服务号 浩颐
        static int weixinAcountId = 7;

        public DownLoadWexinUserInfoJob()
            : base("DownLoadWexinUserInfoJob")
        {
            Log(string.Format("Start Job [DownLoadWexinUserInfoJob]"));

            try
            {
                RunJob();
            }
            catch (Exception ex)
            {
                Log("Job [DownLoadWexinUserInfoJob] Error:" + ex.Message);
            }


            Log(string.Format("Stop Job [DownLoadWexinUserInfoJob]"));
        }

        private void RunJob()
        {
            //模板消息
            WeixinTempMsgEntity msg = new WeixinTempMsgEntity();
            msg.WeixinAcount= weixinAcountId;
            msg.OpenId= "";
            //msg.TempId = "-1gNmtaQN_fWIPzlArhUpSCleXb28cC8-obqP4-80wM";
            //msg.TempId = "-Bj4Q7AhRsusvsfHXeepCYfpt49Iocdj84RR6GmXysc";
            //msg.TempId = "1Et5PC_DqlQyGbYx7ERsqcl-f8ArPOogLfg3WgGl0p0";
            msg.TempId = "2oKigs7BZY8BSMNntmEx1OaKTJAvqnNBDLAujKbAyVg"; //查询结果通知【20181228 haoy】
            msg.Url = Config.WeixinTemplateMsgLink;         // "http://dm12.me/sEkR";
            msg.First = Config.WeixinTemplateMsgTitle;      // "#暑期大促#";
            msg.Remark = Config.WeixinTemplateMsgRemark;    // ">>快戳这里，马上领取！";
            msg.List = new List<string>();
            msg.List.Add(Config.WeixinTemplateMsgName);     // "500元红包限时领"
            msg.List.Add(DateTime.Now.ToString("yyyy年MM月dd日"));
            msg.List.Add(Config.WeixinTemplateMsgDesc);     // "TOP五星亲子度假酒店低至￥349"

            var weixinApi = new Weixin();

            if (!string.IsNullOrEmpty(Config.WeixinTemplateMsgTestOpenid))
            {
                Console.WriteLine("指定了测试Openid：" + Config.WeixinTemplateMsgTestOpenid);
                Log("指定了测试Openid：" + Config.WeixinTemplateMsgTestOpenid);

                msg.OpenId = Config.WeixinTemplateMsgTestOpenid; // "oHGzlw-sdix9G__-S4IzfTsYRqC8";
                var _send = weixinApi.SendTemplateMessage(msg);

                Console.WriteLine(string.Format("结果：{0}", _send));
                Log(string.Format("结果：{0}", _send));
            }
            else
            {
                var num = 0;
                //下载周末酒店zmjiudian的所有关注用户信息
                var nextOpenid = GetNextOpenid();

                //var weixinAllUser = WeixinApiHelper.GetAllWeixinSubscribeUser(weixinApi.GetWeixinToken(weixinAcount), nextOpenid);
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

                            try
                            {
                                //var weixinUser = weixinApi.GetWeixinUserSubscribeInfo(openidObj);
                                //if (weixinUser != null)
                                //{
                                //    weixinUser.WeixinAcount = weixinAcount;
                                //    weixinUser.CreateTime = DateTime.Now;
                                //    if (weixinUser.Subscribe == 0 || weixinUser.SubscribeTime < DateTime.Parse("2000-01-01"))
                                //    {
                                //        weixinUser.SubscribeTime = DateTime.Now;
                                //    }

                                //    if (!string.IsNullOrEmpty(weixinUser.Openid) && !string.IsNullOrEmpty(weixinUser.Unionid))
                                //    {
                                //        //添加用户
                                //        var add = weixinApi.UpdateWeixinUserSubscribe(weixinUser);

                                //        Console.WriteLine("UpdateWeixinUserSubscribe:" + openidObj + ":" + add);
                                //        Log("UpdateWeixinUserSubscribe:" + openidObj + ":" + add);
                                //    }
                                //    else
                                //    {
                                //        Console.WriteLine("weixin user unionid is null:" + openidObj);
                                //        Log("weixin user unionid is null:" + openidObj);
                                //    }
                                //}
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(string.Format("【Err】【{0}】：{1}", openidObj, ex.Message));
                                Log(string.Format("【Err】【{0}】：{1}", openidObj, ex.Message));
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

        string nextopenidFile = @"D:\Log\Config\Weixin\WeixinAllUser_NextOpenid_HAOYI.txt";
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
