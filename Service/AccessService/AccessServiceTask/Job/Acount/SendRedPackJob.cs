using HJD.AccessService.Contract.Model;
using HJD.AccessService.Contract.Model.Acount;
using HJD.AccessService.Contract.Model.Fund;
using HJD.AccessService.Implement;
using HJD.AccessService.Implement.Helper;
using HJD.AccessServiceTask.Entity;
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

namespace HJD.AccessServiceTask.Job.Fund
{
    /// <summary>
    /// 统计红包发送分配 2018-01-23 haoy
    /// </summary>
    public class SendRedPackJob : BaseJob
    {
        public SendRedPackJob()
            : base("SendRedPackJob")
        {
            Log(string.Format("Start Job [SendRedPackJob]"));

            try
            {
                RunJob();
            }
            catch (Exception ex)
            {
                Log("Job [SendRedPackJob] Error:" + ex.Message);
            }


            Log(string.Format("Stop Job [SendRedPackJob]"));
        }

        private void RunJob()
        {
            Console.WriteLine("准备开始发送微信红包奖励");
            Log("准备开始发送微信红包奖励");

            //获取小于当前时间&没有发送的红包奖励记录
            var willSendList = HJD.AccessServiceTask.Job.Helper.WeixinHelper.GetWeixinRewardWillSendRecord();
            if (willSendList != null && willSendList.Count > 0)
            {
                Console.WriteLine(string.Format("待发送红包奖励记录：{0}", willSendList.Count));
                Log(string.Format("待发送红包奖励记录：{0}", willSendList.Count));

                for (int i = 0; i < willSendList.Count; i++)
                {
                    var willSendEntity = willSendList[i];

                    Console.WriteLine(string.Format("【新发送】当前发送给：{0} 金额：{1} 祝福语：{2} 活动：{3} 发送方：{4}", willSendEntity.ReOpenid, willSendEntity.Amount, willSendEntity.Wishing, willSendEntity.ActiveName, willSendEntity.SendName));
                    Log(string.Format("【新发送】当前发送给：{0} 金额：{1} 祝福语：{2} 活动：{3} 发送方：{4}", willSendEntity.ReOpenid, willSendEntity.Amount, willSendEntity.Wishing, willSendEntity.ActiveName, willSendEntity.SendName));

                    //执行红包发送
                    try
                    {
                        string sendUrl = "http://tenpay.zmjiudian.com/api/tenpay/SendRedPack";

                        //判断如果是SourceType=3的红包记录（红包联合活动），则用尚旅的商户号发送（暂排除498新春红包活动，这期活动当时还是用浩颐服务号收集的openid）
                        if (willSendEntity.SourceType == 3 && willSendEntity.ActiveId != 498)
                        {
                            sendUrl = "http://tenpay.zmjiudian.com/api/tenpay/SendRedPackByShangLv";

                            Console.WriteLine("【尚旅商户发送】");
                            Log("【尚旅商户发送】");
                        }

                        string sendParam = string.Format("id={0}", willSendEntity.ID);
                        CookieContainer cc = new CookieContainer();
                        string json = DownloadHelper.Get(sendUrl, sendParam, ref cc);
                        var sendBack = JsonConvert.DeserializeObject<string>(json);

                        //发送成功
                        if (sendBack.Contains("发放成功"))
                        {
                            Console.WriteLine("红包发送成功");
                            Log("红包发送成功");

                            //修改待发记录的状态为1
                            var updateState = HJD.AccessServiceTask.Job.Helper.WeixinHelper.UpdateWeixinRewardRecordState(willSendEntity.ID, 1);
                            Console.WriteLine("待发状态已更新为1：" + updateState);
                            Log("待发状态已更新为1：" + updateState);
                            Log("成功记录：" + sendBack);

                            //发送成功后，处理相关业务规则
                            try
                            {
                                RedpackSendSuccessRules(willSendEntity);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("发送成功后的业务处理报错：" + ex.Message);
                                Log("发送成功后的业务处理报错：" + ex.Message);
                            }

                        }
                        else
                        {
                            Console.WriteLine("红包发送失败");
                            Log("红包发送失败");

                            var updateState = HJD.AccessServiceTask.Job.Helper.WeixinHelper.UpdateWeixinRewardRecordState(willSendEntity.ID, -1);
                            Console.WriteLine("失败！待发状态已更新为-1：" + updateState);
                            Log("失败！待发状态已更新为-1：" + updateState);
                            Log("失败记录：" + sendBack);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("发送异常：" + ex.Message);
                        Log("发送异常：" + ex.Message);
                    }

                    Console.WriteLine("-----------------------------------");
                    Log("-----------------------------------");

                }
            }
            else
            {
                Console.WriteLine("【没有待发送红包奖励记录】");
                Log("【没有待发送红包奖励记录】");
            }
        }

        /// <summary>
        /// rules
        /// </summary>
        /// <param name="sendEntity"></param>
        private void RedpackSendSuccessRules(WeixinRewardRecord sendEntity)
        {
            Console.WriteLine("正在处理红包发送完成后的业务处理～");
            Log("正在处理红包发送完成后的业务处理～");

            switch (sendEntity.SourceType)
            {
                case 3:
                    #region 微信红包大联欢活动的红包发送
                    {
                        Console.WriteLine("SourceType 3：红包大联欢活动");
                        Log("SourceType 3：红包大联欢活动");

                        if (sendEntity.Amount > 0)
                        {
                            Console.WriteLine("SourceId:" + sendEntity.SourceId);
                            Log("SourceId:" + sendEntity.SourceId);

                            Console.WriteLine("sendEntity.Amount(单位 分):" + sendEntity.Amount);
                            Log("sendEntity.Amount(单位 分):" + sendEntity.Amount);

                            decimal _amount = Convert.ToDecimal(Convert.ToDecimal(sendEntity.Amount) / Convert.ToDecimal(100));

                            Console.WriteLine("sendEntity.Amount(转换为元):" + _amount);
                            Log("sendEntity.Amount(转换为元):" + _amount);

                            ////红包发送成功后，需要扣除相关合作伙伴的指定资金
                            //var update = HJD.AccessServiceTask.Job.Helper.WeixinHelper.UpdateWxPartnerActiveFund(sendEntity.SourceId, _amount);

                            //Console.WriteLine("扣除指定资金：" + update);
                            //Log("扣除指定资金：" + update);
                        }
                        else
                        {
                            Console.WriteLine("sendEntity.Amount <= 0");
                            Log("sendEntity.Amount <= 0");
                        }

                        break;
                    }
                    #endregion
                default:
                    Console.WriteLine("没有业务处理");
                    Log("没有业务处理");
                    break;
            }
        }
    }
}
