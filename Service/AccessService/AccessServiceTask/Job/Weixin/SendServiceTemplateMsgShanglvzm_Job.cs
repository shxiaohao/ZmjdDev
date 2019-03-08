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
    /// 发送模板消息（尚旅周末服务号 尚旅主体）
    /// </summary>
    public class SendServiceTemplateMsgShanglvzm_Job : BaseJob
    {
        //尚旅周末
        static int weixinAcountId = 2;

        public SendServiceTemplateMsgShanglvzm_Job()
            : base("SendServiceTemplateMsgShanglvzm_Job")
        {
            Log(string.Format("Start Job [SendServiceTemplateMsgShanglvzm_Job]"));

            try
            {
                RunJob();
            }
            catch (Exception ex)
            {
                Log("Job [SendServiceTemplateMsgShanglvzm_Job] Error:" + ex.Message);
            }


            Log(string.Format("Stop Job [SendServiceTemplateMsgShanglvzm_Job]"));
        }

        private void RunJob()
        {
            var weixinApi = new Weixin();

            //模板消息
            WeixinTempMsgEntity msg = new WeixinTempMsgEntity();
            msg.WeixinAcount= weixinAcountId;
            msg.OpenId= "";

            #region 项目进度通知（招代理）

            //msg.TempId = "cHsVtD8UCnZBzZiit-RO_Ik3ilZux14NzO1AaAilIDU";     //项目进度通知
            //msg.Url = Config.WeixinTemplateMsgLink;
            //msg.First = Config.WeixinTemplateMsgTitle;
            //msg.Remark = Config.WeixinTemplateMsgRemark;
            //msg.List = new List<string>();
            //msg.List.Add(Config.WeixinTemplateMsgDesc);
            //msg.List.Add(DateTime.Now.ToString("yyyy年MM月dd日"));

            #endregion

            #region 待办事项（暑期尾巴200券）

            msg.TempId = "A4Uq0IkjnEUjMHCOByXWytYP9bjoQpis3FAEIW5dST8";     //待办事项
            msg.Url = Config.WeixinTemplateMsgLink;
            msg.First = Config.WeixinTemplateMsgTitle;
            msg.Remark = Config.WeixinTemplateMsgRemark;
            msg.List = new List<string>();
            msg.List.Add(Config.WeixinTemplateMsgName);
            msg.List.Add("待领取");
            msg.List.Add(DateTime.Now.ToString("yyyy年MM月dd日"));
            msg.List.Add(Config.WeixinTemplateMsgDesc);

            #endregion

            ////测试
            //msg.Url = "http://www.zmjiudian.com/channel/partners?cid=4512632 ";
            //msg.First = "【未创建店铺】周末酒店APP招募分销代理，开启会员自购省钱，分享赚钱";
            //msg.Remark = ">>点击申请";
            //msg.List = new List<string>();
            //msg.List.Add("度假伙伴");
            //msg.List.Add(DateTime.Now.ToString("yyyy-MM-dd"));

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

        string nextopenidFile = @"D:\Log\Config\Weixin\WeixinAllUser_NextOpenid_Shanglvzmjd.txt";
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
