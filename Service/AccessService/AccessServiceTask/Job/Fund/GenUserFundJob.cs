using HJD.AccessService.Contract.Model;
using HJD.AccessService.Contract.Model.Acount;
using HJD.AccessService.Contract.Model.Fund;
using HJD.AccessService.Implement;
using HJD.AccessService.Implement.Helper;
using HJD.AccessServiceTask.Entity;
using HJD.AccessServiceTask.Job.Helper;
using HJDAPI.APIProxy;
using HtmlAgilityPack;
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
    /// 统计用户推荐奖励基金
    /// </summary>
    public class GenUserFundJob : BaseJob
    {
        public GenUserFundJob()
            : base("GenUserFundJob")
        {
            Log(string.Format("Start Job [GenUserFundJob]"));

            try
            {
                RunJob();
            }
            catch (Exception ex)
            {
                Log("Job [GenUserFundJob] Error:" + ex.Message);
            }


            Log(string.Format("Stop Job [GenUserFundJob]"));
        }

        private void RunJob()
        {
            var yesterday = DateTime.Now.AddDays(-1);
            Console.WriteLine("yesterday:" + yesterday);
            Log("yesterday:" + yesterday);

            #region 查询出还有哪些推荐人没有创建用户基金记录

            var recUserList = AcountHelper.GetNoFundRecommendUsers();
            if (recUserList != null && recUserList.Count > 0)
            {
                Console.WriteLine("GetNoFundRecommendUsers Count:" + recUserList.Count);
                Log("GetNoFundRecommendUsers Count:" + recUserList.Count);

                for (int i = 0; i < recUserList.Count; i++)
                {
                    var userEntity = recUserList[i];

                    Console.WriteLine("No Fund User:" + userEntity.UserId);
                    Log("No Fund User:" + userEntity.UserId);

                    var userFund = new UserFund();
                    userFund.UserId = userEntity.UserId;
                    userFund.TotalFund = 0;
                    userFund.CreateTime = DateTime.Now;
                    userFund.UpdateTime = DateTime.Now;
                    var add = AcountHelper.InsertUserFundInfo(userFund);

                    Console.WriteLine("InsertUserFundInfo:" + add);
                    Log("InsertUserFundInfo:" + add);
                }
            }
            else
            {
                Console.WriteLine("GetNoFundRecommendUsers is null or count = 0");
                Log("GetNoFundRecommendUsers is null or count = 0");
            }

            #endregion

            //产生新的基金奖励的用户
            var newUserList = new List<Int64>();

            #region 统计所有推荐人的基金奖励明细

            //获取所有的被推荐人
            var list = AcountHelper.GetAllRecommendRelList();
            if (list != null && list.Count > 0)
            {
                Console.WriteLine("GetAllRecommendRelList Count:" + list.Count);
                Log("GetAllRecommendRelList Count:" + list.Count);

                for (int i = 0; i < list.Count; i++)
                {
                    var relEntity = list[i];
                    Console.WriteLine("rel recommend:" + relEntity.UserId + ":" + relEntity.ReUserId);

                    var userFundIncomeDetail = new UserFundIncomeDetail();

                    var isnew = false;

                    #region 注册奖励基金

                    //注册时间是今天的，增加一条基金奖励为0的奖励明细
                    if (relEntity.ReRegisterDate.Day == yesterday.Day)
                    {
                        userFundIncomeDetail = new UserFundIncomeDetail();
                        userFundIncomeDetail.UserId = relEntity.UserId;
                        userFundIncomeDetail.TypeId = 1;    //1用户注册（被推荐人注册）  2订单奖励（被推荐人下单）  3订单扣除  4订单取消 9VIP订单奖励 
                        userFundIncomeDetail.Fund = 0;
                        userFundIncomeDetail.Label = "您推荐的1位朋友已注册成功";
                        userFundIncomeDetail.RelationUserId = relEntity.ReUserId;
                        userFundIncomeDetail.OriginOrderId = 0;
                        userFundIncomeDetail.OriginAmount = 0;
                        userFundIncomeDetail.OriCreateTime = relEntity.ReRegisterDate;
                        userFundIncomeDetail.CreateTime = DateTime.Now;

                        //铁规:推荐人和被推荐人不能同一人
                        if (userFundIncomeDetail.UserId != userFundIncomeDetail.RelationUserId)
                        {
                            var add1 = AcountHelper.InsertUserFundIncomeDetail(userFundIncomeDetail);

                            Console.WriteLine("InsertUserFundIncomeDetail【Register】:" + add1);
                            Log("InsertUserFundIncomeDetail【Register】:" + add1);

                            isnew = true;
                        }
                        else
                        {
                            Console.WriteLine(string.Format("【warning】UserId == ReUserId: {0} == {1}", userFundIncomeDetail.UserId, userFundIncomeDetail.RelationUserId));
                            Log(string.Format("【warning】UserId == ReUserId: {0} == {1}", userFundIncomeDetail.UserId, userFundIncomeDetail.RelationUserId));
                        }
                    }

                    #endregion

                    #region 成交订单的奖励基金

                    //根据当前被推荐人找出其昨天成交的所有的订单数据，统计出其推荐人应得的推荐奖励基金
                    var orderInfoList = AcountHelper.GetCheckInOrderInfoByUserAndDate(yesterday, relEntity.ReUserId);
                    if (orderInfoList != null && orderInfoList.Count > 0)
                    {
                        Console.WriteLine("GetCheckInOrderInfoByUserAndDate Count:" + orderInfoList.Count);
                        Log("GetCheckInOrderInfoByUserAndDate Count:" + orderInfoList.Count);

                        for (int onum = 0; onum < orderInfoList.Count; onum++)
                        {
                            var orderInfo = orderInfoList[onum];

                            var orderFund = Convert.ToDecimal(orderInfo.Amount * Convert.ToDecimal(0.01));

                            userFundIncomeDetail = new UserFundIncomeDetail();
                            userFundIncomeDetail.UserId = relEntity.UserId;
                            userFundIncomeDetail.TypeId = 2;    //1用户注册（被推荐人注册）  2订单奖励（被推荐人下单）  3订单扣除  4订单取消 9VIP订单奖励
                            userFundIncomeDetail.Fund = orderFund;
                            userFundIncomeDetail.Label = "" + orderInfo.HotelName;
                            userFundIncomeDetail.RelationUserId = relEntity.ReUserId;
                            userFundIncomeDetail.OriginOrderId = orderInfo.OrderID;
                            userFundIncomeDetail.OriginAmount = orderInfo.Amount;
                            userFundIncomeDetail.OriCreateTime = orderInfo.CheckIn.AddDays(orderInfo.NightCount);
                            userFundIncomeDetail.CreateTime = DateTime.Now;

                            //铁规:推荐人和被推荐人不能同一人
                            if (userFundIncomeDetail.UserId != userFundIncomeDetail.RelationUserId)
                            {
                                var add2 = AcountHelper.InsertUserFundIncomeDetail(userFundIncomeDetail);

                                Console.WriteLine("InsertUserFundIncomeDetail【Order】:" + add2);
                                Log("InsertUserFundIncomeDetail【Order】:" + add2);

                                isnew = true;
                            }
                            else
                            {
                                Console.WriteLine(string.Format("【warning】UserId == ReUserId: {0} == {1}", userFundIncomeDetail.UserId, userFundIncomeDetail.RelationUserId));
                                Log(string.Format("【warning】UserId == ReUserId: {0} == {1}", userFundIncomeDetail.UserId, userFundIncomeDetail.RelationUserId));
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("GetCheckInOrderInfoByUserAndDate is null or count = 0");
                        Log("GetCheckInOrderInfoByUserAndDate is null or count = 0");
                    }

                    #endregion

                    //标识有基金变更的用户
                    if (isnew && !newUserList.Contains(relEntity.UserId))
                    {
                        newUserList.Add(relEntity.UserId);
                    }
                }
            }
            else
            {
                Console.WriteLine("GetAllRecommendRelList is null or count = 0:");
                Log("GetAllRecommendRelList is null or count = 0:");
            }

            #endregion

            #region 统计每一个产生基金奖励用户的奖励统计信息

            if (newUserList.Count > 0)
            {
                Console.WriteLine("newUserList count:" + newUserList.Count);
                Log("newUserList count:" + newUserList.Count);

                for (int i = 0; i < newUserList.Count; i++)
                {
                    var user = newUserList[i];

                    //获取该用户本次统计日的所有的基金变更
                    var incomeDetails = AcountHelper.GetFundIncomeDetailByUser(yesterday, user);
                    if (incomeDetails != null && incomeDetails.Count > 0)
                    {
                        Console.WriteLine("GetFundIncomeDetailByUser count:" + incomeDetails.Count);
                        Log("GetFundIncomeDetailByUser count:" + incomeDetails.Count);

                        //直接触发更新typeid=1的注册统计
                        var funIncomeStat = new UserFundIncomeStat();
                        funIncomeStat.UserId = user;
                        funIncomeStat.TypeId = 1;
                        funIncomeStat.Day = DateTime.Now;
                        funIncomeStat.Fund = 0;
                        funIncomeStat.Label = "";
                        funIncomeStat.CreateTime = DateTime.Now;
                        var updateRegStat = AcountHelper.InsertOrUpdateFundIncomeStat_1(funIncomeStat);
                        Console.WriteLine("InsertOrUpdateFundIncomeStat_1:" + updateRegStat);
                        Log("InsertOrUpdateFundIncomeStat_1:" + updateRegStat);

                        //获取typeid=2的订单奖励
                        var list2 = incomeDetails.Where(id => id.TypeId == 2).ToList();
                        var orderSumFund = list2.Sum(l2 => l2.Fund);
                        if (orderSumFund > 0)
                        {
                            funIncomeStat = new UserFundIncomeStat();
                            funIncomeStat.UserId = user;
                            funIncomeStat.TypeId = 2;
                            funIncomeStat.Day = yesterday.Date;
                            funIncomeStat.Label = "推荐朋友奖励";
                            funIncomeStat.Fund = orderSumFund;
                            funIncomeStat.CreateTime = DateTime.Now;
                            var updateOrderStat = AcountHelper.InsertOrUpdateFundIncomeStat_2(funIncomeStat);
                            Console.WriteLine("InsertOrUpdateFundIncomeStat_2:" + updateOrderStat);
                            Log("InsertOrUpdateFundIncomeStat_2:" + updateOrderStat);   
                        }
                    }
                    else
                    {
                        Console.WriteLine("GetFundIncomeDetailByUser is null or count = 0");
                        Log("GetFundIncomeDetailByUser is null or count = 0");
                    }

                    //统计计算完以后，重新计算当前用户的基金记录（总基金等信息重算）
                    var reset = AcountHelper.ResetUserFund(user);
                    Console.WriteLine("ResetUserFund:" + reset);
                    Log("ResetUserFund:" + reset);
                }
            }
            else
            {
                Console.WriteLine("newUserList count = 0");
                Log("newUserList count = 0");
            }

            #endregion
        }
    }
}
