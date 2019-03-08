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

namespace HJD.AccessServiceTask.Job
{
    /// <summary>
    /// 监听“发布增加宝石的队列任务”
    /// </summary>
    public class GenActiveGemListener : BaseJob
    {
        RabbitMqContext rbmqContext;
        static string queueKey = "PublishWeixinGemTask";

        public GenActiveGemListener()
            : base("GenActiveGemListener")
        {
            Log(string.Format("Start Job [GenActiveGemListener]"));

            try
            {
                RunJob();
            }
            catch (Exception ex)
            {
                Log("Job [GenActiveGemListener] Error:" + ex.Message);
            }


            Log(string.Format("Stop Job [GenActiveGemListener]"));
        }

        private void RunJob()
        {
            //weixin api
            var weixinApi = new Weixin();

            #region 准备队列

            rbmqContext = new RabbitMqContext(queueKey, Config.RabbitmqHostName, Config.RabbitmqUserName, Config.RabbitmqPassword);

            #endregion

            #region 读取队列

            rbmqContext.OnLitening();

            var ea = (BasicDeliverEventArgs)rbmqContext.BasicConsumer.Queue.Dequeue();
            while (ea.Body.Length > 0)
            {
                try
                {
                    var item = ea.Body;
                    var taskStr = Encoding.UTF8.GetString(item);
                    var task = JsonConvert.DeserializeObject<ActiveWeixinLuckCode>(taskStr);
                    if (task != null)
                    {
                        Console.WriteLine(string.Format("{0} 新的任务：{1}：{2}：{3}：{4}", DateTime.Now, task.ActiveId, task.Openid, task.PartnerId, task.TagName));
                        Log(string.Format("{0} 新的任务：{1}：{2}：{3}：{4}", DateTime.Now, task.ActiveId, task.Openid, task.PartnerId, task.TagName));

                        if (!string.IsNullOrEmpty(task.Openid))
                        {
                            Console.WriteLine("执行抽奖码生成~");

                            #region 没有PartnerId

                            if (task.PartnerId == 0)
                            {
                                //if (task.ActiveId == 498)
                                {
                                    Console.WriteLine("【红包大联欢，直接单次奖励宝石】");

                                    #region 微信红包大联欢活动

                                    //将LuckCode转换成FicMoney
                                    var taskFicMoney = new ActiveWeixinFicMoney
                                    {
                                        Id = task.Id,
                                        ActiveId = task.ActiveId,
                                        Openid = task.Openid,
                                        PartnerId = task.PartnerId,
                                        Value = task.LuckCode,
                                        Remark = task.TagName,
                                        CreateTime = task.CreateTime
                                    };

                                    //检查当前用户Openid还可以生成几个宝石（规则：阅读数满1/满3/满5/满n次获得一个抽奖码）
                                    var genCount = HJD.AccessServiceTask.Job.Helper.WeixinHelper.GetWeixinUserHaveLuckCodeCountForRedpack(task);
                                    Console.WriteLine("【宝石】GetWeixinUserHaveLuckCodeCountForRedpack:" + genCount);
                                    Log("【宝石】GetWeixinUserHaveLuckCodeCountForRedpack:" + genCount);

                                    if (genCount > 0)
                                    {
                                        for (int i = 0; i < genCount; i++)
                                        {
                                            Console.WriteLine("【宝石】gen:" + i);

                                            try
                                            {
                                                var add = weixinApi.AddActiveWeixinFicMoney(taskFicMoney);
                                                Console.WriteLine("【宝石】AddActiveWeixinFicMoney:" + add);
                                            }
                                            catch (Exception ex_add)
                                            {
                                                Console.WriteLine("【宝石】[ex_add error]:" + ex_add.Message);
                                                Log("【宝石】[ex_add error]:" + ex_add.Message);
                                            }
                                        }
                                    }

                                    #endregion
                                }
                                //else
                                //{
                                //    Console.WriteLine("【无合作者，按次奖励抽奖码】");

                                //    #region 常规免费住活动

                                //    //检查当前用户Openid还可以生成几个抽奖码（规则：阅读数满3/满5/满n次获得一个抽奖码）
                                //    var genCount = HJD.AccessServiceTask.Job.Helper.WeixinHelper.GetWeixinUserHaveLuckCodeCount(task);
                                //    Console.WriteLine("GetWeixinUserHaveLuckCodeCount:" + genCount);
                                //    Log("GetWeixinUserHaveLuckCodeCount:" + genCount);

                                //    if (genCount > 0)
                                //    {
                                //        for (int i = 0; i < genCount; i++)
                                //        {
                                //            Console.WriteLine("gen:" + i);

                                //            try
                                //            {
                                //                var add = weixinApi.AddActiveWeixinLuckCode(task);
                                //                Console.WriteLine("AddActiveWeixinLuckCode:" + add);
                                //            }
                                //            catch (Exception ex_add)
                                //            {
                                //                Console.WriteLine("[ex_add error]:" + ex_add.Message);
                                //                Log("[ex_add error]:" + ex_add.Message);
                                //            }
                                //        }
                                //    }

                                //    #endregion
                                //}
                            }

                            #endregion

                            #region 有PartnerId

                            else
                            {
                                Console.WriteLine("【有合作者】");

                                //if (task.ActiveId == 498)
                                {
                                    Console.WriteLine("【红包大联欢，奖励宝石】");

                                    try
                                    {
                                        //将LuckCode转换成FicMoney
                                        var taskFicMoney = new ActiveWeixinFicMoney
                                        {
                                            Id = task.Id,
                                            ActiveId = task.ActiveId,
                                            Openid = task.Openid,
                                            PartnerId = task.PartnerId,
                                            Value = task.LuckCode,
                                            Remark = task.TagName,
                                            CreateTime = task.CreateTime
                                        };

                                        var add = weixinApi.AddActiveWeixinFicMoney(taskFicMoney);
                                        Console.WriteLine("【宝石】AddActiveWeixinFicMoney:" + add);
                                    }
                                    catch (Exception ex_add)
                                    {
                                        Console.WriteLine("【宝石】[ex_add error]:" + ex_add.Message);
                                        Log("【宝石】[ex_add error]:" + ex_add.Message);
                                    }
                                }
                                //else
                                //{
                                //    Console.WriteLine("【常规免费住，奖励抽奖码】");

                                //    try
                                //    {
                                //        var add = weixinApi.AddActiveWeixinLuckCode(task);
                                //        Console.WriteLine("AddActiveWeixinLuckCode:" + add);
                                //    }
                                //    catch (Exception ex_add)
                                //    {
                                //        Console.WriteLine("[ex_add error]:" + ex_add.Message);
                                //        Log("[ex_add error]:" + ex_add.Message);
                                //    }
                                //}
                            }

                            #endregion
                        }
                        else
                        {
                            Console.WriteLine(string.Format("{0} task.Openid is null", DateTime.Now));
                            Log(string.Format("{0} task.Openid is null", DateTime.Now));
                        }
                    }
                    else
                    {
                        Console.WriteLine(string.Format("{0} task is null", DateTime.Now));
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("【Error】:" + ex.Message);
                    Log("【Error】:" + ex.Message);
                }

                rbmqContext.ListenChannel.BasicAck(ea.DeliveryTag, false);

                Console.WriteLine(string.Format("-_- 等待任务中 .. "));
                ea = (BasicDeliverEventArgs)rbmqContext.BasicConsumer.Queue.Dequeue();
            }

            #endregion
        }
    }
}
