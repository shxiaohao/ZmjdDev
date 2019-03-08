using HJD.AccessService.Contract.Model;
using HJD.AccessService.Contract.Model.Acount;
using HJD.AccessService.Implement;
using HJD.AccessService.Implement.Helper;
using HJD.AccessServiceTask.Entity;
using HJD.AccessServiceTask.Job.Helper;
using HJDAPI.APIProxy;
using HtmlAgilityPack;
using MessageService.Contract;
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

namespace HJD.AccessServiceTask.Job.Acount
{
    /// <summary>
    /// 统计用户的推荐关联关系表
    /// </summary>
    public class GenUserRecommendRelJob : BaseJob
    {
        public GenUserRecommendRelJob()
            : base("GenUserRecommendRelJob")
        {
            Log(string.Format("Start Job [GenUserRecommendRelJob]"));

            try
            {
                RunJob();
            }
            catch (Exception ex)
            {
                Log("Job [GenUserRecommendRelJob] Error:" + ex.Message);
            }


            Log(string.Format("Stop Job [GenUserRecommendRelJob]"));
        }

        private void RunJob()
        {
            //test
            //SendFundRegistryNotice(4514792);

            //获取通过红包/现金券推荐的用户记录（已注册）
            var list = AcountHelper.GetRegisteredRecommendUserInfo();
            if (list != null && list.Count > 0)
            {
                Console.WriteLine("GetRegisteredRecommendUserInfo Count:" + list.Count);
                Log("GetRegisteredRecommendUserInfo Count:" + list.Count);

                //每天凌晨统计昨天注册的用户
                //var yesterday = DateTime.Now.AddDays(-1);
                var useLastRegTime = GetUseLastRegTime();
                list = list.Where(o => o.RegisterTime >= o.CreateTime && o.RegisterTime > useLastRegTime).OrderBy(o => o.RegisterTime).ToList();

                Console.WriteLine("useLastRegTime:" + useLastRegTime);
                Log("useLastRegTime:" + useLastRegTime);

                Console.WriteLine("RegisterTime > useLastRegTime Count:" + list.Count);
                Log("RegisterTime > useLastRegTime Count:" + list.Count);

                for (int i = 0; i < list.Count; i++)
                {
                    var oriCouponEntity = list[i];

                    Console.WriteLine(string.Format("--Entity:【{0}】:【{1}】:【{2}】:【{3}】:【{4}】:【{5}】", oriCouponEntity.Id, oriCouponEntity.SourceId, oriCouponEntity.UserId, oriCouponEntity.CreateTime, oriCouponEntity.RegisterTime, oriCouponEntity.TypeId));
                    Log(string.Format("--Entity:【{0}】:【{1}】:【{2}】:【{3}】:【{4}】:【{5}】", oriCouponEntity.Id, oriCouponEntity.SourceId, oriCouponEntity.UserId, oriCouponEntity.CreateTime, oriCouponEntity.RegisterTime, oriCouponEntity.TypeId));

                    //执行insert
                    var relObj = new UserRecommendRel();
                    relObj.UserId = oriCouponEntity.SourceId;
                    relObj.ReUserId = oriCouponEntity.UserId;
                    relObj.RecommendChannel = GetRecommendChannel(oriCouponEntity);     //定义：1 分享50元现金券；2 订单红包分享
                    relObj.RecommendDate = oriCouponEntity.CreateTime;
                    relObj.ReRegisterDate = oriCouponEntity.RegisterTime;

                    //基本规则，推荐人和被推荐人不能为同一人
                    if (relObj.UserId != relObj.ReUserId)
                    {
                        var add = AcountHelper.InsertUserRecommendRelInfo(relObj);

                        Console.WriteLine("InsertUserRecommendRelInfo:" + add);
                        Log("InsertUserRecommendRelInfo:" + add);

                        #region 当推荐人有新的被推荐用户注册，通知推荐人“你邀请的1位好友注册成功“，链接至”我的住基金”页面

                        //SendFundRegistryNotice(relObj.UserId);

                        #endregion
                    }
                    else 
                    {
                        Console.WriteLine(string.Format("【warning】UserId == ReUserId: {0} == {1}", relObj.UserId, relObj.ReUserId));
                        Log(string.Format("【warning】UserId == ReUserId: {0} == {1}", relObj.UserId, relObj.ReUserId));
                    }

                    //记录最后处理过的注册时间
                    var lastUseRegTime = oriCouponEntity.RegisterTime;
                    SetUseLastRegTime(lastUseRegTime);

                    Console.WriteLine("--【set lastUseRegTime】:" + lastUseRegTime);
                    Log("--【set lastUseRegTime】:" + lastUseRegTime);
                }
            }
            else
            {
                Console.WriteLine("GetRegisteredRecommendUserInfo is null or count = 0:");
                Log("GetRegisteredRecommendUserInfo is null or count = 0:");
            }
        }

        private static int GetRecommendChannel(OriginCoupon obj)
        {
            var channel = 0;

            switch (obj.TypeId)
            {
                    //50元现金券
                case 8:
                    {
                        channel = 1;
                        break;
                    }
                    //订单分享红包
                case 100:
                    {
                        channel = 2;
                        break;
                    }
            }

            return channel;
        }

        #region 最后使用的一个注册时间

        string useLastRegTimeFile = @"D:\Log\Config\Access\Acount\UseLastRegTime.txt";
        private DateTime GetUseLastRegTime()
        {
            var yesterday = DateTime.Now.AddDays(-1);
            var useLastRegTime = yesterday;

            if (File.Exists(useLastRegTimeFile))
            {
                try
                {
                    var fileInfo = File.ReadAllText(useLastRegTimeFile).Trim();
                    if (!string.IsNullOrEmpty(fileInfo))
                    {
                        useLastRegTime = DateTime.Parse(fileInfo);   
                    }
                    Console.WriteLine("【GetUseLastRegTime】:" + useLastRegTime);
                    Log("【GetUseLastRegTime】:" + useLastRegTime);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("读取NextOpenid错误：" + ex.Message);
                    Log("读取NextOpenid错误：" + ex.Message);
                }
            }

            return useLastRegTime;
        }

        private void SetUseLastRegTime(DateTime regTime)
        {
            File.WriteAllText(useLastRegTimeFile, regTime.ToString("yyyy-MM-dd HH:mm:ss.ff"));
            Console.WriteLine("SetUseLastRegTime已经存储至本地！" + regTime.ToString("yyyy-MM-dd HH:mm:ss.ff"));
            Log("SetUseLastRegTime已经存储至本地！" + regTime.ToString("yyyy-MM-dd HH:mm:ss.ff"));
        }

        #endregion

        #region 发送app系统消息推送

        private static void SendFundRegistryNotice(long userid)
        {
            SendNoticeEntity sendEntity = new SendNoticeEntity();
            sendEntity.userID = userid;
            sendEntity.appType = 0;
            sendEntity.title = "周末酒店";
            sendEntity.msg = "你邀请的1位好友注册成功";
            sendEntity.noticeType = ZMJDNoticeType.recommendregistry;

            string url = "http://api.zmjiudian.com/api/Notice/SendAppNotice";
            CookieContainer cc = new CookieContainer();
            string json = DownloadHelper.PostJson(url, sendEntity, ref cc);
            var back = JsonConvert.DeserializeObject<int>(json);

            Console.WriteLine(string.Format("[{0}] AddType:{1}  UserId:{2}  back:{3}", DateTime.Now, sendEntity.appType, userid, back));
            Log(string.Format("[{0}] AddType:{1}  UserId:{2}  back:{3}", DateTime.Now, sendEntity.appType, userid, back));
        }

        #endregion
    }
}
