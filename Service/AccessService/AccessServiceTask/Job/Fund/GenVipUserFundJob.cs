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
    /// 统计VIP用户基金获得
    /// </summary>
    public class GenVipUserFundJob : BaseJob
    {
        public GenVipUserFundJob()
            : base("GenVipUserFundJob")
        {
            Log(string.Format("Start Job [GenVipUserFundJob]"));

            try
            {
                RunJob();
            }
            catch (Exception ex)
            {
                Log("Job [GenVipUserFundJob] Error:" + ex.Message);
            }


            Log(string.Format("Stop Job [GenVipUserFundJob]"));
        }

        private void RunJob()
        {
            //查询出还没有奖励1%住基金的铂金会员的所有已完成的订单
            var orderInfoList = AcountHelper.GetCheckInOrderInfoByVipUser();
            if (orderInfoList != null && orderInfoList.Count > 0)
            {
                Console.WriteLine("GetCheckInOrderInfoByVipUser Count:" + orderInfoList.Count);
                Log("GetCheckInOrderInfoByVipUser Count:" + orderInfoList.Count);

                for (int oNum = 0; oNum < orderInfoList.Count; oNum++)
                {
                    var orderInfo = orderInfoList[oNum];

                    var orderFund = Convert.ToDecimal(orderInfo.Amount * Convert.ToDecimal(0.01));

                    var userFundIncomeDetail = new UserFundIncomeDetail();
                    userFundIncomeDetail.UserId = orderInfo.UserID;
                    userFundIncomeDetail.TypeId = 9;    //1用户注册（被推荐人注册）  2订单奖励（被推荐人下单）  3订单扣除  4订单取消  9VIP订单奖励
                    userFundIncomeDetail.Fund = orderFund;
                    userFundIncomeDetail.Label = "VIP订单奖励";
                    userFundIncomeDetail.RelationUserId = 0;
                    userFundIncomeDetail.OriginOrderId = orderInfo.OrderID;
                    userFundIncomeDetail.OriginAmount = orderInfo.Amount;
                    userFundIncomeDetail.OriCreateTime = orderInfo.CheckIn.AddDays(orderInfo.NightCount);
                    userFundIncomeDetail.CreateTime = DateTime.Now;

                    //add fund
                    var add = AcountHelper.AddUserFund(userFundIncomeDetail);

                    Console.WriteLine(string.Format("InsertUserFundIncomeDetail【{0}】【{1}】【{2}】【{3}】:{4}", userFundIncomeDetail.UserId, userFundIncomeDetail.OriginOrderId, userFundIncomeDetail.OriginAmount, userFundIncomeDetail.Fund, add));
                    Log(string.Format("InsertUserFundIncomeDetail【{0}】【{1}】【{2}】【{3}】:{4}", userFundIncomeDetail.UserId, userFundIncomeDetail.OriginOrderId, userFundIncomeDetail.OriginAmount, userFundIncomeDetail.Fund, add));
                }

            }
            else
            {
                Console.WriteLine("GetCheckInOrderInfoByVipUser is null~");
                Log("GetCheckInOrderInfoByVipUser is null~");
            }
        }
    }
}
