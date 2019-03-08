using HJD.CouponService.Contracts;
using HJD.CouponService.Contracts.Entity;
using HJD.CouponService.Impl.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HJD.CouponService.Impl
{
    //[GlobalExceptionHandlerBehaviour(typeof(GlobalExceptionHandler))]
    public class CouponService : ICouponService
    {
        #region 生成红包
        public OriginCouponResult GenerateOriginCoupon(long userId, int typeId, long sourceId, int totalMoney, int cashMoney, string moneyStr)
        {
            OriginCouponResult result = new OriginCouponResult();
            string guid = System.Guid.NewGuid().ToString();
            result.GUID = guid;
            long originID = CouponDAL.GenerateOriginCoupon(userId, typeId, sourceId, totalMoney, cashMoney, guid);
            if (originID > 0 && !string.IsNullOrEmpty(moneyStr))
            {
                string[] numArray = moneyStr.Split(',');
                for (int i = 0; i < numArray.Length; i++)
                {
                    int money = int.Parse(numArray[i]);
                    AcquiredCoupon param = new AcquiredCoupon() { OriginId = originID, TotalMoney = money, RestMoney = money };
                    CouponDAL.UpdateAcquiredCoupon(param);
                }
                result.Message = "生成红包成功";
                result.Success = 0;
            }
            else
            {
                result.Message = "生成红包失败";
                result.Success = 1;
            }
            return result;
        }

        public AcquiredCoupon GetAcquiredCoupon(long userId, string guid, string phoneNo)
        {
            long originId = CouponDAL.GetOriginIdByGUID(guid);
            long acquiredId = CouponDAL.GetAcquiredCouponId(userId, originId, phoneNo);
            AcquiredCoupon ac = GetAcquiredCouponById(acquiredId);
            if (ac.ID == 0)
            {
                return ac;
            }
            int money = TransNum(ac.TotalMoney);
            ac.TotalMoney = ac.RestMoney = money;
            return ac;
        }

        public List<AcquiredCoupon> GetAcquiredCouponList(string guid)
        {
            long originId = CouponDAL.GetOriginIdByGUID(guid);
            List<AcquiredCoupon> list = CouponDAL.GetAcquiredCouponList(originId);

            if (list != null && list.Count != 0)
            {
                foreach (AcquiredCoupon temp in list)
                {
                    temp.TotalMoney = TransNum(temp.TotalMoney);
                    temp.ExpiredMoney = TransNum(temp.ExpiredMoney);
                    temp.RestMoney = TransNum(temp.RestMoney);
                }
                return list;
            }
            else
            {
                return new List<AcquiredCoupon>();
            }
        }

        /// <summary>
        /// 更新cashcoupon状态
        /// </summary>
        /// <returns></returns>
        public OriginCouponResult UpdateCashCoupon(long id, string guid, CashCouponState state)
        {
            try
            {
                long ss = CouponDAL.UpdateOriginCoupon(id, guid, (int)state);
                return new OriginCouponResult() { Message = "更新现金红包成功", Success = 0 };
            }
            catch (Exception e)
            {
                return new OriginCouponResult() { Message = "更新现金红包失败" + e.Message + e.StackTrace, Success = 1 };
            }
        }

        /// <summary>
        /// 更新订单转发 送现金券活动状态 并插入一条现金券记录
        /// </summary>
        /// <returns></returns>
        public OriginCouponResult UpdateCashCouponAndGenACouponRecord(long id, CashCouponState state, long userId, string phoneNo, int couponAmount)
        {
            try
            {
                long ss = CouponDAL.UpdateOriginCoupon(id, "", (int)state);
                DateTime dtNow = DateTime.Now;
                DateTime dtSecondYearLater = dtNow.AddYears(2);
                AcquiredCoupon ac = new AcquiredCoupon() { OriginId = id, TotalMoney = couponAmount, RestMoney = couponAmount, AcquiredTime = dtNow, PhoneNo = phoneNo, UserId = userId, ExpiredTime = dtSecondYearLater };
                CouponDAL.InsertAcquiredCoupon(ac);
                return new OriginCouponResult() { Message = "订单转发领取现金券成功", Success = 0 };
            }
            catch (Exception e)
            {
                return new OriginCouponResult() { Message = "订单转发领取现金券失败" + e.Message + e.StackTrace, Success = 1 };
            }
        }

        /// <summary>
        /// 通过组成字段更新
        /// </summary>
        /// <param name="id"></param>
        /// <param name="guid"></param>
        /// <param name="state"></param>
        /// <returns></returns>        
        public OriginCouponResult UpdateCashCouponBySourceId(long sourceId, int typeId, long userId, CashCouponState state)
        {
            try
            {
                long ss = CouponDAL.UpdateCashCouponByKeyColumn(sourceId, typeId, userId, (int)state);
                return new OriginCouponResult() { Message = "更新现金红包成功", Success = 0 };
            }
            catch (Exception e)
            {
                return new OriginCouponResult() { Message = "更新现金红包失败" + e.ToString(), Success = 1 };
            }
        }

        public List<OriginCoupon> GetUserOrgCouponInfoByType(long userId, int typeID)
        {
            return CouponDAL.GetUserOrgCouponInfoByType(userId, typeID);
        }

        /// <summary>
        /// 活动的数据
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="typeId"></param>
        /// <param name="phoneNo"></param>
        /// <returns></returns>
        public OriginCouponResult GenerateOriginCoupon2(long userId, int type, string phoneNo)
        {
            OriginCouponResult result = new OriginCouponResult();
            string guid = System.Guid.NewGuid().ToString();
            result.GUID = guid;
            CouponTypeDefineEntity define = GetCouponTypeDefineByType(type);

            long originID = CouponDAL.GenerateOriginCoupon(userId, define.Type, 0, define.CashCoupon, define.CashMoney, guid);
            if (originID > 0)
            {
                AcquiredCoupon param = new AcquiredCoupon() { OriginId = originID, TotalMoney = define.CashCoupon, RestMoney = define.CashCoupon, UserId = userId, PhoneNo = phoneNo, AcquiredTime = DateTime.Now, ExpiredTime = DateTime.Now.AddYears(2) };
                CouponDAL.InsertAcquiredCoupon(param);
                result.Message = "返券成功！";
                result.Success = 0;
            }
            else
            {
                result.Message = "返券失败！";
                result.Success = 1;
            }
            return result;
        }

        /// <summary>
        /// 记录活动数据
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="type"></param>
        /// <param name="phoneNo"></param>
        /// <param name="sourceId"></param>
        /// <returns></returns>
        public OriginCouponResult GenerateOriginCoupon3(long userId, int type, string phoneNo, long sourceId)
        {
            OriginCouponResult result = new OriginCouponResult();
            string guid = System.Guid.NewGuid().ToString();
            result.GUID = guid;
            CouponTypeDefineEntity define = GetCouponTypeDefineByType(type);

            long originID = CouponDAL.GenerateOriginCoupon(userId, define.Type, sourceId, define.CashCoupon, define.CashMoney, guid);
            if (originID > 0)
            {
                AcquiredCoupon param = new AcquiredCoupon() { OriginId = originID, TotalMoney = define.CashCoupon, RestMoney = define.CashCoupon, UserId = userId, PhoneNo = phoneNo, AcquiredTime = DateTime.Now, ExpiredTime = DateTime.Now.AddYears(2) };
                CouponDAL.InsertAcquiredCoupon(param);
                result.Message = "返券成功！";
                result.Success = 0;
            }
            else
            {
                result.Message = "返券失败！";
                result.Success = 1;
            }
            return result;
        }

        /// <summary>
        /// 活动券分发
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="code">活动码</param>
        /// <param name="phoneNo">手机号</param>
        /// <returns></returns>
        public OriginCouponResult GenerateOriginCouponByActivity(long userId, CouponActivityCode code, string phoneNo)
        {
            OriginCouponResult result = new OriginCouponResult();
            string guid = System.Guid.NewGuid().ToString();
            result.GUID = guid;
            CouponTypeDefineEntity define = CouponDAL.GetCouponTypeDefineByCode(code);

            long originID = CouponDAL.GenerateOriginCoupon(userId, define.Type, 0, define.CashCoupon, define.CashMoney, guid);
            if (originID > 0)
            {
                AcquiredCoupon param = new AcquiredCoupon() { OriginId = originID, TotalMoney = define.CashCoupon, RestMoney = define.CashCoupon, UserId = userId, PhoneNo = phoneNo, AcquiredTime = DateTime.Now, ExpiredTime = DateTime.Now.AddYears(2) };
                CouponDAL.InsertAcquiredCoupon(param);
                result.Message = "返券成功！";
                result.Success = 0;
            }
            else
            {
                result.Message = "返券失败！";
                result.Success = 1;
            }
            return result;
        }

        #endregion

        public bool UpdOldVIPtoNewVIP(long userId)
        {
            return CouponDAL.UpdOldVIPtoNewVIP(userId);
        }

        public bool Del6MVIP(long userId)
        {
            return CouponDAL.Del6MVIP(userId);
        }

        #region 使用红包

        /// <summary>
        /// 订单使用现金券记录
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="orderid"></param>
        /// <param name="couponAmount"></param>
        /// <returns></returns>
        public ReturnCouponResult SetOrderCoupon(long userid, long orderid, int planNum, bool isBudget)
        {
            int orderCoupon = CouponDAL.GetOrderRecord(orderid);
            ReturnCouponResult result = null;
            int transNum = 0;
            if (orderCoupon == 0 || orderCoupon - planNum < 0)
            {
                result = SubtractCoupon(userid, orderid, planNum, orderCoupon, isBudget);
                transNum = result.DiscountMoney;
            }
            else if (orderCoupon - planNum > 0)
            {
                result = ReturnCoupon(userid, orderid, planNum, orderCoupon, isBudget);
                transNum = result.DiscountMoney;
            }
            else if (orderCoupon - planNum == 0)
            {
                result = new ReturnCouponResult() { Message = "与已消费的现金券记录一样", Success = 0, DiscountMoney = planNum };
                transNum = planNum;
            }
            //ToDo 新增现金券消费记录表 实际金额的转换
            if (!isBudget && result != null)
            {
                CouponDAL.UpdateUseCouponRecord(orderid, result.DiscountMoney);
            }

            result.DiscountMoney = TransNum(transNum);
            return result;
        }

        /// <summary>
        /// 退现金券
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="orderId"></param>
        /// <param name="CouponNum"></param>
        /// <returns></returns>
        private ReturnCouponResult ReturnCoupon(long userId, long orderId, int planNum, int orderCoupon, bool isBudget)
        {
            int CouponNum = orderCoupon - planNum;
            if (userId == 0 || CouponNum == 0)
            {
                return new ReturnCouponResult() { Message = "缺少用户ID或现金券退扣数量", Success = 1 };
            }
            //ToDo 
            //1.订单抵扣记录更新值 减掉退还的现金券；如果大于等于抵扣前则更新抵扣值为0
            //2.找到用户未过期的现金券（且可用金额小于原始金额） 按照从早到晚的顺序依次补满。
            //2.1 能补买则操作完成；2.2剩余金额依次补入过期现金券剩余额度中（更新过期金额）

            List<AcquiredCoupon> unexpiredList = CouponDAL.GetAcquiredCouponList(userId, false);
            List<AcquiredCoupon> expiredList = CouponDAL.GetAcquiredCouponList(userId, true);
            List<AcquiredCoupon> updateList = new List<AcquiredCoupon>();
            int validMoney = 0;
            if (unexpiredList == null || unexpiredList.Count == 0)
            {
                if (expiredList != null && expiredList.Count != 0)
                {
                    foreach (AcquiredCoupon temp in expiredList)
                    {
                        if (temp.TotalMoney > temp.ExpiredMoney)
                        {
                            int margin = (int)(temp.TotalMoney - temp.ExpiredMoney);
                            if (margin < CouponNum)
                            {
                                CouponNum -= margin;
                                updateList.Add(new AcquiredCoupon() { ID = temp.ID, ExpiredMoney = temp.TotalMoney });
                                continue;
                            }
                            else
                            {
                                updateList.Add(new AcquiredCoupon() { ID = temp.ID, ExpiredMoney = temp.ExpiredMoney + CouponNum });
                                CouponNum = 0;
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                foreach (AcquiredCoupon temp in unexpiredList)
                {
                    if (temp.TotalMoney > temp.RestMoney)
                    {
                        int margin = (int)(temp.TotalMoney - temp.RestMoney);
                        if (margin < CouponNum)
                        {
                            validMoney += margin;
                            CouponNum -= margin;
                            updateList.Add(new AcquiredCoupon() { ID = temp.ID, RestMoney = temp.TotalMoney });
                            continue;
                        }
                        else
                        {
                            validMoney += CouponNum;
                            updateList.Add(new AcquiredCoupon() { ID = temp.ID, RestMoney = temp.RestMoney + CouponNum });
                            CouponNum = 0;
                            break;
                        }
                    }
                }
                //判断CouponNum的值 如果大于0则继续
                if (CouponNum > 0)
                {
                    if (expiredList != null && expiredList.Count != 0)
                    {
                        foreach (AcquiredCoupon temp in expiredList)
                        {
                            if (temp.TotalMoney > temp.ExpiredMoney)
                            {
                                int margin = (int)(temp.TotalMoney - temp.ExpiredMoney);
                                if (margin < CouponNum)
                                {
                                    CouponNum -= margin;
                                    updateList.Add(new AcquiredCoupon() { ID = temp.ID, ExpiredMoney = temp.TotalMoney });
                                    continue;
                                }
                                else
                                {
                                    updateList.Add(new AcquiredCoupon() { ID = temp.ID, ExpiredMoney = temp.ExpiredMoney + CouponNum });
                                    CouponNum = 0;
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            //更新涉及的红包列表
            if (!isBudget && updateList != null && updateList.Count != 0)
            {
                foreach (AcquiredCoupon temp in updateList)
                {
                    CouponDAL.UpdateAcquiredCoupon(temp);
                }
            }
            return new ReturnCouponResult() { Message = isBudget ? "【预估】现金券退款成功" : "现金券退款成功", DiscountMoney = planNum, Success = 0 };
        }

        /// <summary>
        /// 扣现金券
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="orderId"></param>
        /// <param name="CouponNum"></param>
        /// <returns></returns>
        private ReturnCouponResult SubtractCoupon(long userId, long orderId, int planNum, int orderCoupon, bool isBudget)
        {
            int CouponNum = planNum - orderCoupon;
            if (userId == 0 || CouponNum == 0)
            {
                return new ReturnCouponResult() { Message = "缺少用户ID或现金券退扣数量", Success = 1 };
            }
            //ToDo 
            //1.新建订单抵扣记录 
            //2.找到用户未过期的现金券（且可用金额大于0） 按照从早到晚的顺序依次扣去。

            List<AcquiredCoupon> unexpiredList = CouponDAL.GetAcquiredCouponList(userId, false);
            List<AcquiredCoupon> updateList = new List<AcquiredCoupon>();

            int validMoney = 0;
            if (unexpiredList != null && unexpiredList.Count != 0)
            {
                foreach (AcquiredCoupon temp in unexpiredList)
                {
                    if (temp.RestMoney > 0)
                    {
                        int margin = (int)temp.RestMoney;
                        if (margin < CouponNum)
                        {
                            CouponNum -= margin;
                            updateList.Add(new AcquiredCoupon() { ID = temp.ID, RestMoney = 0 });

                            //现金券额度不足 只会发生在符合此条件的循环中
                            validMoney += margin;
                            continue;
                        }
                        else
                        {
                            validMoney += CouponNum;
                            updateList.Add(new AcquiredCoupon() { ID = temp.ID, RestMoney = temp.RestMoney - CouponNum });
                            CouponNum = 0;
                            break;
                        }
                    }
                }
            }

            //更新涉及的红包列表
            if (!isBudget && updateList != null && updateList.Count != 0)
            {
                foreach (AcquiredCoupon temp in updateList)
                {
                    CouponDAL.UpdateAcquiredCoupon(temp);
                }
            }

            //判断CouponNum的值 如果大于0则继续
            if (CouponNum > 0)
            {
                return new ReturnCouponResult() { Message = isBudget ? "【预估】现金券可用金额不足" : "现金券可用金额不足", DiscountMoney = validMoney + orderCoupon, Success = 0 };
            }
            else
            {
                return new ReturnCouponResult() { Message = isBudget ? "【预估】现金券扣款成功" : "现金券扣款成功", DiscountMoney = planNum, Success = 0 };
            }
        }

        /// <summary>
        /// 用户可用现金券总额
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public int GetUserCouponSum(long userId)
        {
            return TransNum(CouponDAL.GetUserCouponSum(userId));
        }

         /// <summary>
        /// VIP续费
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public  bool ReNewVIPAfterPayment(long userID)
        {
            return CouponDAL.ReNewVIPAfterPayment(userID);
        }
        /// <summary>
        /// 获得现金列表
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<OriginCoupon> GetCashCouponList(long userId)
        {
            List<OriginCoupon> list = CouponDAL.GetCashCouponList(userId);
            if (list != null && list.Count != 0)
            {
                foreach (OriginCoupon temp in list)
                {
                    temp.TotalMoney = TransNum(temp.TotalMoney);
                    temp.CashMoney = TransNum(temp.CashMoney);
                }
                return list;
            }
            else
            {
                return new List<OriginCoupon>();
            }
        }

        /// <summary>
        /// 获得我的现金券列表
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<AcquiredCoupon> GetAcquiredCouponList2(long userId, bool? isExpired)
        {
            List<AcquiredCoupon> list = CouponDAL.GetAcquiredCouponList(userId, isExpired);
            if (list != null && list.Count != 0)
            {
                foreach (AcquiredCoupon temp in list)
                {
                    temp.TotalMoney = TransNum(temp.TotalMoney);
                    temp.ExpiredMoney = TransNum(temp.ExpiredMoney);
                    temp.RestMoney = TransNum(temp.RestMoney);
                }
                return list;
            }
            else
            {
                return new List<AcquiredCoupon>();
            }
        }

        #endregion

        public AcquiredCoupon GetAcquiredCouponById(long id)
        {
            AcquiredCoupon ac = CouponDAL.GetAcquiredCouponById(id);
            return ac == null ? new AcquiredCoupon() : ac;
        }

        public string GetOriginGUIDByOrderAndTypeId(long sourceId, long typeId)
        {
            return CouponDAL.GetOriginGUIDByOrderAndTypeId(sourceId, (int)typeId);
        }

        public int GetAcquiredCouponByPhone(string phoneNo, string guid)
        {
            return TransNum((decimal)CouponDAL.GetAcquiredCouponByPhone(phoneNo, guid));
        }

        public CouponActivitySKURelEntity GetCouponActivitySKURelBySKUID(int SKUID)
        {
            return CouponDAL.GetCouponActivitySKURelBySKUID(SKUID);
        }

        public CouponActivityEntity GetCouponActivityBySKUID(int SKUID)
        {
            var a = CouponDAL.GetCouponActivityBySKUID(SKUID);
            a.GenDicProperties();
            return a;
        }

        public List<CouponActivityEntity> GetCouponActivityBySourceID(int sourceID)
        {
            return CouponDAL.GetCouponActivityBySourceID(sourceID);
        }

        public List<CouponActivitySKURelEntity> GetCouponActivitySKURelByActivityID(int ActivityID)
        {
            return CouponDAL.GetCouponActivitySKURelByActivityID(ActivityID);
        }

        public OriginCoupon GetCashCoupon(long id, string guid)
        {
            OriginCoupon ss = CouponDAL.GetCashCouponAmount(id, guid);
            ss.TotalMoney = TransNum((decimal)ss.TotalMoney);
            ss.CashMoney = TransNum((decimal)ss.CashMoney);
            return ss;
        }

        /// <summary>
        /// 人民币 分转元
        /// </summary>
        /// <param name="dec"></param>
        /// <returns></returns>
        private int TransNum(decimal dec)
        {
            return (int)Math.Round(dec, 0);
        }
        public List<CouponTypeDefineEntity> GetCouponTypeDefine()
        {
            return CouponDAL.GetCouponTypeDefine();
        }
        public CouponTypeDefineEntity GetCouponTypeDefineByID(int ID)
        {
            return CouponDAL.GetCouponTypeDefineById(ID);
        }
        public CouponTypeDefineEntity GetCouponTypeDefineByType(int Type)
        {
            return CouponDAL.GetCouponTypeDefineByType(Type);
        }

        public CouponTypeDefineEntity GetCouponTypeDefineByCode(CouponActivityCode code)
        {
            return CouponDAL.GetCouponTypeDefineByCode(code);
        }

        public int InsertInspectorReward(InspectorRewardEntity ire)
        {
            CouponDAL.InsertOrUpdateInspectorReward(ire);
            return 0;
        }

        public int UpdateInspectorReward(InspectorRewardEntity ire)
        {
            CouponDAL.InsertOrUpdateInspectorReward(ire);
            return 0;
        }

        public List<InspectorRewardItem> GetInspectorRewardItemList(long userid)
        {
            return CouponDAL.GetInspectorRewardItemList(userid);
        }

        public List<AcquiredCoupon> GetAcquireCouponRecordByUserID(long userid)
        {
            return CouponDAL.GetAcquireCouponRecordByUserID(userid);
        }

        public List<UseCouponRecordEntity> GetUseCouponRecordByUserID(long userid)
        {
            return CouponDAL.GetUseCouponRecordByUserID(userid);
        }

        public OriginCouponResult GenerateOriginCouponEx(long userId, long sourceID, CouponActivityCode code, string phoneNo)
        {
            OriginCouponResult result = new OriginCouponResult();
            string guid = System.Guid.NewGuid().ToString();
            result.GUID = guid;
            CouponTypeDefineEntity define = CouponDAL.GetCouponTypeDefineByCode(code);

            if (define.EndTime <= DateTime.Now)
            {
                result.Message = "分享点评返现金券活动已结束！";
                result.Success = 3;
                return result;
            }

            OriginCoupon ocp = CouponDAL.GetOriginCouponByTypeAndSourceID(sourceID, define.Type);
            if (ocp != null && ocp.ID != 0)
            {
                result.Message = "已返券不能重复参加！";
                result.Success = 2;
            }
            else
            {
                long originID = CouponDAL.GenerateOriginCoupon(userId, define.Type, sourceID, define.CashCoupon, define.CashMoney, guid);
                if (originID > 0)
                {
                    AcquiredCoupon param = new AcquiredCoupon() { OriginId = originID, TotalMoney = define.CashCoupon, RestMoney = define.CashCoupon, UserId = userId, PhoneNo = phoneNo, AcquiredTime = DateTime.Now, ExpiredTime = DateTime.Now.AddYears(2) };
                    CouponDAL.InsertAcquiredCoupon(param);
                    result.Message = "返券成功！";
                    result.Success = 0;
                }
                else
                {
                    result.Message = "返券失败！";
                    result.Success = 1;
                }
            }
            return result;
        }

        public OriginCoupon GetOriginCouponByTypeAndSourceID(long sourceId, int typeId)
        {
            return CouponDAL.GetOriginCouponByTypeAndSourceID(sourceId, typeId);
        }

        public string SomeFailingOperation()
        {
            throw new Exception("Boom 异常处理炸弹");
        }

        public int InsertCouponActivity(CouponActivityEntity cae)
        {
            int ID = CouponDAL.InsertCouponActivity(cae);

            CouponDAL.AddCouponActivitySKURel(ID, cae.RelSKUIDs);
            return ID;
        }

        public int UpdateCouponActivity(CouponActivityEntity cae)
        {
            CouponDAL.AddCouponActivitySKURel(cae.ID, cae.RelSKUIDs);
            return CouponDAL.UpdateCouponActivity(cae);
        }

        public int UpdateCouponActivityNoUpRel(CouponActivityEntity cae)
        {
            return CouponDAL.UpdateCouponActivity(cae);
        }

        public int UpdateExchangeNO(ExchangeCouponEntity param)
        {
            return CouponDAL.UpdateExchangeNO(param);
        }

        public CouponActivityEntity GetOneCouponActivity(int id, bool isLock)
        {
            var a = CouponDAL.GetOneCouponActivity(id, isLock);
            a.GenDicProperties();
            return a;
        }

        public CouponActivityEntity GetOneCouponActivityAndSKU(int id, bool isLock)
        {
            var a = CouponDAL.GetOneCouponActivityAndSKU(id, isLock);
            a.GenDicProperties();
            return a;
        }

        public List<CouponActivityEntity> GetCouponActivityList(CouponActivityQueryParam param, out int totalCount)
        {
            totalCount = 0;
            var result = new List<CouponActivityEntity>();
            if (param == null || param.stateArray == null || param.activityTypeArray == null)
            {
                return result;
            }

            result = CouponDAL.GetCouponActivityList(param, out totalCount);
            return result;
        }

        public CouponActivityEntity GetCouponActivityByBizIdAndBizType(int bizId, int bizType)
        {
            return CouponDAL.GetCouponActivityByBizIdAndBizType(bizId, bizType);
        }

        public List<CouponActivityEntity> GetCouponActivityBySKUIDSList(CouponActivityQueryParam param, out int totalCount)
        {
            totalCount = 0;
            var result = new List<CouponActivityEntity>();
            if (param == null || param.stateArray == null || param.activityTypeArray == null)
            {
                return result;
            }

            result = CouponDAL.GetCouponActivityBySKUIDSList(param, out totalCount);
            return result;
        }


        /// <summary>
        /// 支付完成后根据打包产品SKU价格比例更新打包产品（赠送产品）价格。（以前赠送产品的价格都是0， 导致后面结算有问题）
        /// </summary>
        /// <param name="payid"></param>
        /// <returns></returns>
         public  int  AdjustPriceForPackageSKU(int payid)
        {
            return CouponDAL.SP4_ExchangeCoupon_AdjustPriceForPackageSKU(payid);
        }
     

        public List<CouponActivityEntity> MemberCouponActivityList(CouponActivityQueryParam param, out int totalCount)
        {
            totalCount = 0;
            var result = new List<CouponActivityEntity>();
            if (param == null || param.stateArray == null || param.activityTypeArray == null)
            {
                return result;
            }

            result = CouponDAL.MemberCouponActivityList(param, out totalCount);
            return result;
        }

        public List<CouponActivityEntity> MemberRetailCouponActivityList(CouponActivityQueryParam param, out int totalCount)
        {
            totalCount = 0;
            var result = new List<CouponActivityEntity>();
            if (param == null || param.stateArray == null)
            {
                return result;
            }

            result = CouponDAL.MemberRetailCouponActivityList(param, out totalCount);
            return result;
        }

        public CouponActivityEntity GetToDayCouponActivity()
        {
            return CouponDAL.GetToDayCouponActivity();
        }
        public CouponActivityEntity GetToDayCouponActivityAndSKU()
        {
            return CouponDAL.GetToDayCouponActivityAndSKU();
        }


        public List<ExchangeCouponEntity> GetExchangeCouponEntityListByUser(long userId, int activityType)
        {
            return CouponDAL.GetExchangeCouponEntityListByUser(userId, activityType);
        }

        public List<ExchangeCouponEntity> GetExchangCouponByCategoryId(long userId, int cParentId, int start, int count,int couponId)
        {
            return CouponDAL.GetExchangCouponByCategoryId(userId, cParentId, start, count,couponId);
        }

        public List<ExchangeCouponEntity> GetExchangeCouponEntityListByIDList(List<int> IDList)
        {
            return CouponDAL.GetExchangeCouponEntityListByIDList(IDList);
        }

        public List<ExchangeCouponEntity> GetExchangeCouponEntityListBySKUID(int skuid, int state)
        {
            return CouponDAL.GetExchangeCouponEntityListBySKUID(skuid, state);
        }

        public List<ExchangeCouponEntity> GetExchangeCouponEntityListNoJoinBookBySKUID(int skuid, int state)
        {
            return CouponDAL.GetExchangeCouponEntityListNoJoinBookBySKUID(skuid, state);
        }

        public List<ExchangeCouponEntity> GetExchangeCouponEntityListByPhone(string phone, int activityType)
        {
            return CouponDAL.GetExchangeCouponEntityListByPhone(phone, activityType);
        }

        public List<ExchangeCouponEntity> GetExchangeCouponEntityListByGroupId(int groupid, int activityType)
        {
            return CouponDAL.GetExchangeCouponEntityListByGroupId(groupid, activityType);
        }

        public int CancelUnPayExchangeCouponOrderByActivityIDAndUserID(long userId, int activityID, int SKUID)
        {
            return CouponDAL.CancelUnPayExchangeCouponOrderByActivityIDAndUserID(userId, activityID, SKUID);
        }

        /// <summary>
        /// 插入兑换券记录
        /// </summary>
        /// <param name="ece"></param>
        /// <returns></returns>
        public int InsertExchangeCoupon(ExchangeCouponEntity ece)
        {
            return CouponDAL.InsertExchangeCoupon(ece);
        }

        /// <summary>
        /// 更新兑换券记录
        /// </summary>
        /// <param name="ece"></param>
        /// <returns></returns>
        public int UpdateExchangeCoupon(ExchangeCouponEntity ece)
        {
            return CouponDAL.UpdateExchangeCoupon(ece);
        }

        public int AddCouponActivityBizRel(CouponActivityBizRelEntity couponactivitybizrel)
        {
            return CouponDAL.AddCouponActivityBizRel(couponactivitybizrel);
        }

        public int AddOrUpdateCouponActivityBizRel(CouponActivityBizRelEntity couponactivitybizrel)
        {
            return CouponDAL.AddOrUpdateCouponActivityBizRel(couponactivitybizrel);
        }

        public List<CouponActivityBizRelEntity> GetCouponActivityBizRelByCouponActivityIdOrBizID(int cid, int bizID, int bizType)
        {
            return CouponDAL.GetCouponActivityBizRelByCouponActivityIdOrBizID(cid, bizID, bizType);
        }

        public int UpdateCouponActivityBizRel(CouponActivityBizRelEntity couponactivitybizrel)
        {
            return CouponDAL.UpdateCouponActivityBizRel(couponactivitybizrel);
        }

        public int UpdateExchangeCouponForSMSAlert(int ID, int SMSAlertType)
        {
            return CouponDAL.UpdateExchangeCouponForSMSAlert(ID, SMSAlertType);
        }

        public int UpdateExchangeCouponForPhotoUrl(int ID, string PhotoUrl)
        {
            return CouponDAL.UpdateExchangeCouponForPhotoUrl(ID, PhotoUrl);
        }

        public List<ExchangeCouponEntity> GetExchangeCouponPageList(int activityID, int state, int pageSize, int pageIndex, out int total)
        {
            return CouponDAL.GetExchangeCouponPageList(activityID, state, pageSize, pageIndex, out total);
        }

        public List<ExchangeCouponEntity> GetExchangeOrderList(int pageSize, int pageIndex, int followOperation, int skuid, string skuName, string phoneNum, string supplierName, string thirdorderid, string exchangeNo, out int totalCount)
        {
            return CouponDAL.GetExchangeOrderList(pageSize, pageIndex, followOperation, skuid, skuName, phoneNum, supplierName, thirdorderid, exchangeNo, out  totalCount);
        }

        public CouponOrderOperationEntity GetCouponOderOperationByCouponId(int couponId)
        {
            return CouponDAL.GetCouponOderOperationByCouponId(couponId);
        }

        public List<ExchangeCouponEntity> GetExchangeCouponEntityListByActivity(int activityID)
        {
            return CouponDAL.GetExchangeCouponEntityListByActivity(activityID);
        }

        public List<ExchangeCouponEntity> GetExchangeCouponEntityListByActivityIDAndUserID(int activityID, long userID)
        {
            return CouponDAL.GetExchangeCouponEntityListByActivityIDAndUserID(activityID, userID);
        }

        public int InsertCommOrders(CommOrderEntity coe)
        {
            return CouponDAL.InsertCommOrders(coe);
        }

        public CommOrderEntity GetOneCommOrderEntity(int idx)
        {
            CommOrderEntity coe = CouponDAL.GetOneCommOrderEntity(idx);
            if (coe.IDX > 0)
            {
                coe.RelExchangeCoupon = GetExchangeCouponEntityByPayID(coe.IDX);
            }
            return coe;
        }

        public List<ExchangeCouponEntity> GetExchangeCouponEntityByPayID(int payID)
        {
            return CouponDAL.GetExchangeCouponEntityByPayID(payID);
        }
        public List<ExchangeCouponEntity> GetExchangeCouponEntityByCID(long CID)
        {
            return CouponDAL.GetExchangeCouponEntityByCID(CID);
        }

        public int BindOrderAndExchangeCoupon(long orderID, string exchangeNo, string phoneNo, long updator, int state = 3)
        {
            //ExchangeCouponEntity bindCoupon = CouponDAL.GetUsedCouponByOrderID(orderID);
            //if (bindCoupon.ID != 0)
            //{
            //    return 1;//订单已经绑定到其他券
            //}
            //else{
            ExchangeCouponEntity ece = CouponDAL.GetOneExchangeCouponByCouponNo(exchangeNo);
            if (ece.ID == 0)
            {
                return 2;//券码错误 找不到数据
            }
            else if (ece.State == 3)
            {
                return 3;//此券已兑换过
            }
            else if (ece.State == 4)
            {
                return 4;//券的状态为已取消 不能抵用
            }
            else if (ece.PhoneNum.Trim() != phoneNo)
            {
                return 5;//购券手机号与消费券提供的手机号码不一致
            }
            //}
            return CouponDAL.UpdateExchangeCoupon(new ExchangeCouponEntity() { ID = 0, ExchangeNo = exchangeNo, PayID = 0, ExchangeTargetID = orderID, ActivityID = 0, State = state, CancelTime = null, ExchangeTime = DateTime.Now, Updator = updator });
        }

        /// <summary>
        /// 获得订单使用的房券
        /// </summary>
        /// <param name="orderID">订单长ID</param>
        /// <returns></returns>
        public List<ExchangeCouponEntity> GetUsedCouponByOrderID(long orderID)
        {
            return CouponDAL.GetUsedCouponByOrderID(orderID);
        }

        public Int64 GetNextId(NextIdType tablename)
        {
            //return 0;
            return CouponDAL.GetNextId(tablename);
        }
        /// <summary>
        /// 各类券退款 只要状态未未消费 过期且可退
        /// </summary>
        /// <param name="couponID"></param>
        /// <returns></returns>
        public int CouponRefund(int couponID)
        {
            if (couponID > 0)
            {
                ExchangeCouponEntity ece = CouponDAL.GetOneExchangeCoupon(couponID);
                return CouponDAL.AddRefundCoupon(new RefundCouponsEntity() { CouponID = couponID, State = 1, Updator = ece.UserID, Creator = ece.UserID });
            }
            else
            {
                return 0;
            }
        }

        public List<ExchangeCouponEntity> GetExchangeCouponListByUserIDSourceID(Int64 UserID, Int64 SourceID)
        {
             return CouponDAL.GetExchangeCouponListByUserIDSourceID( UserID,  SourceID);
        }

        public List<ExchangeCouponEntity> GetWaitingRefundCouponList()
        {
            return CouponDAL.GetWaitingRefundCouponList();
        }

        public int ReturnOrderConsumedCoupon(long orderID, long updator)
        {
            return CouponDAL.ReturnOrderConsumedCoupon(orderID, updator);
        }

        public int UpdateCouponState4TimeOut(int activityID = 0)
        {
            return CouponDAL.UpdateCouponState4TimeOut(activityID);
        }
        public int GetActivityLockedCount(int activityID)
        {
            return CouponDAL.GetActivityLockedCount(activityID);
        }
        /// <summary>
        /// 支付类型和状态
        /// </summary>
        /// <param name="payType"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public List<RefundCouponsEntity> GetRefundCouponsList(RefundCouponsQueryParam param, out int count)
        {
            return CouponDAL.GetRefundCouponsList(param, out count);
        }

        public int UpdateRemark(int couponid, string remark)
        {
            return CouponDAL.UpdateRemark(couponid, remark);
        }
        public List<WillCloseProductGroupEntity> GetWillCloseProductGroup(int GroupCount, int SKUID, int Hour)
        {
            return CouponDAL.GetWillCloseProductGroup(GroupCount, SKUID, Hour);
        }
        /// <summary>
        /// 更新兑换券退款 
        /// rce.state=2表示支付宝直接确认退款完成;rce.state=3表示非支付宝支付确认 确认完成需要向hoteldb.refund表加一条退款记录， 同时将RefundID写回到refoundCoupon表中，以便关联
        /// </summary>
        /// <param name="rce"></param>
        /// <returns></returns>
        public int UpdateRefundCoupon(RefundCouponsEntity rce)
        {
            int flag = CouponDAL.UpdateRefundCoupon(rce);//更新refundcoupons表状态记录
            if (flag == 0)
            {
                //将要退券记录插入到总的退款表
                return CouponDAL.Insert2Refund(rce);
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        ///  更新部分退款状态，并插入数据到refund表中， 同时将RefundID写回到refoundCoupon表中，以便关联
        /// </summary>
        /// <param name="rce"></param>
        /// <returns></returns>
        public int UpdateRefundCouponForPartRefund(RefundCouponsEntity rce)
        {
            CouponDAL.UpdateRefundCouponForPartRefund( rce);
            return CouponDAL.Insert2Refund(rce);
        }
        public int UpdateThirdPartyRefundCoupon(int couponid, int thirdstate, int couponRefundstate)
        {
            return CouponDAL.UpdateThirdPartyRefundCoupon(couponid, thirdstate, couponRefundstate);//更新refundcoupons表状态记录
        }

        public int AddRefundCoupon(RefundCouponsEntity rce)
        {
            return CouponDAL.AddRefundCoupon(rce);
        }

        /// <summary>
        /// 主要添加待退券进入退款列表 如果支付类型是支付宝 无需插入HotelDB Refund表
        /// </summary>
        /// <param name="rce"></param>
        /// <returns></returns>
        public int CancelRefundCoupon(RefundCouponsEntity rce)
        {
            return CouponDAL.CancelRefundCoupon(rce);
        }

        /// <summary>
        /// 由券ID和券码 获得相关的券信息
        /// </summary>
        /// <param name="couponID">券ID</param>
        /// <param name="exchangeNo">券码</param>
        /// <returns></returns>
        public ExchangeCouponEntity GetOneExchangeCouponInfo(int couponID, string exchangeNo)
        {
            if (couponID > 0)
            {
                return CouponDAL.GetOneExchangeCoupon(couponID);
            }
            if (!string.IsNullOrWhiteSpace(exchangeNo))
            {
                return CouponDAL.GetOneExchangeCouponByCouponNo(exchangeNo);
            }
            return new ExchangeCouponEntity();
        }

        /// <summary>
        /// 获取房券的兑换价格列表
        /// </summary>
        /// <param name="couponActivity"></param>
        /// <returns></returns>
        public IEnumerable<CouponRateEntity> GetCouponRateEntityList(int couponActivity)
        {
            return CouponDAL.GetCouponRateEntityList(couponActivity);
        }

        /// <summary>
        /// 更新房券 兑换价格列表
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public int UpdateCouponRateEntity(IEnumerable<CouponRateEntity> param)
        {
            foreach (var item in param)
            {
                CouponDAL.UpdateCouponRateEntity(item);
            }
            return 0;
        }

        /// <summary>
        /// 删除房券价格
        /// </summary>
        /// <param name="cre"></param>
        /// <returns></returns>
        public int DeleteCouponRateEntity(CouponRateEntity cre)
        {
            return CouponDAL.DeleteCouponRateEntity(cre);
        }

        /// <summary>
        /// 由活动ID获取券ID(仅房券内容)
        /// </summary>
        /// <param name="activityID"></param>
        /// <returns></returns>
        //public IEnumerable<ExchangeCouponEntity> GetExchangeCouponList(int activityID)
        //{
        //    return CouponDAL.GetExchangeCouponList(activityID);
        //}

        /// <summary>
        /// 获取指定用户购买的指定SKU的券记录
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="skuid"></param>
        /// <param name="promotionId"></param>
        /// <returns></returns>
        public List<ExchangeCouponEntity> GetExchangeCouponListByUserSKU(long userId, int skuid, int promotionId)
        {
            return CouponDAL.GetExchangeCouponListByUserSKU(userId, skuid, promotionId);
        }

        /// <summary>
        /// 获取指定用户购买的指定SKU的券记录
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="skuid"></param>
        /// <param name="promotionId"></param>
        /// <returns></returns>
        public List<ExchangeCouponEntity> GetExchangeCouponListByUserIDAndSKU(long userId, int skuid)
        {
            return CouponDAL.GetExchangeCouponListByUserIDAndSKU(userId, skuid);
        }

        /// <summary>
        /// copy到某个区
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="updator"></param>
        /// <param name="merchantCode"></param>
        /// <returns></returns>
        public int CopyCouponActivity(int activityId, long updator, CouponActivityMerchant merchantCode)
        {
            return CouponDAL.CopyCouponActivity(activityId, updator, merchantCode);
        }

        /// <summary>
        /// 获取要结算的券
        /// </summary>
        /// <returns></returns>
        public List<ExchangeCouponEntity> GetNeedSettlementBotaoCouponList()
        {
            return CouponDAL.GetNeedSettlementBotaoCouponList();
        }

        /// <summary>
        /// 获得需要取消的成员列表
        /// </summary>
        /// <returns></returns>
        public List<long> GetNeedCancelMemberUserList()
        {
            return CouponDAL.GetNeedCancelMemberUserList();
        }

        /// <summary>
        /// 检查当前用户是否属于【预约 & [品鉴师|候选|有预订记录] & 非会员】的用户
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public int IsVipNoPayReserveUser(string userid)
        {
            return CouponDAL.IsVipNoPayReserveUser(userid);
        }

        /// <summary>
        /// 获取指定userid、typeid=8的originCoupon数据
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public OriginCoupon GetOriginCouponByUserIdForT8(long userid)
        {
            return CouponDAL.GetOriginCouponByUserIdForT8(userid);
        }
        public List<DistrictInfoForWxappEntity> GetRoomCouponDistrictInfoForWxapp(List<int> districtIds)
        {
            return CouponDAL.GetRoomCouponDistrictInfoForWxapp(districtIds);
        }
        public List<DistrictInfoForWxappEntity> GetRoomCouponDistrictInfoForLatLngWxapp(List<int> districtIds, double lat = 0, double lng = 0, int geoScopeType = 3, bool inchina = true)
        {
            return CouponDAL.GetRoomCouponDistrictInfoForLatLngWxapp(districtIds, lat, lng, geoScopeType, inchina);
        }

        public int AddVoucherChannel(VoucherChannelEntity voucherchannel)
        {
            return CouponDAL.AddVoucherChannel(voucherchannel);
        }

        /// <summary>
        /// 这里添加方法注释
        /// </summary>
        public int UpdateVoucherChannel(VoucherChannelEntity voucherchannel)
        {
            return CouponDAL.UpdateVoucherChannel(voucherchannel);
        }
        public int AddVoucherDefine(VoucherDefineEntity voucherdefine)
        {
            return CouponDAL.AddVoucherDefine(voucherdefine);
        }

        public int UpdateVoucherDefine(VoucherDefineEntity voucherdefine)
        {
            return CouponDAL.UpdateVoucherDefine(voucherdefine);
        }
        public int AddVoucherItems(VoucherItemsEntity voucheritems)
        {
            return CouponDAL.AddVoucherItems(voucheritems);
        }
        public int UpdateVoucherItems(VoucherItemsEntity voucheritems)
        {
            return CouponDAL.UpdateVoucherItems(voucheritems);
        }
        public List<VoucherDefineEntity> GetVoucherDefineList(int idx, string name)
        {
            return CouponDAL.GetVoucherDefineList(idx, name);
        }
        public List<VoucherChannelEntity> GetVoucherChanneList(int idx, string name, int defineid)
        {
            return CouponDAL.GetVoucherChanneList(idx, name, defineid);
        }

        public List<VoucherChannelEntity> GetVoucherChanneByCode(string code)
        {
            return CouponDAL.GetVoucherChanneByCode(code);
        }
        public List<VoucherItemsEntity> GetVoucherItemByVoucherChannelid(int channelid)
        {
            return CouponDAL.GetVoucherItemByVoucherChannelid(channelid);
        }

        public int AddUsedConsumerCouponInfo(UsedConsumerCouponInfoEntity usedconsumercouponinfo)
        {
            return CouponDAL.AddUsedConsumerCouponInfo(usedconsumercouponinfo);
        }

        public int UpdateUsedConsumerCouponInfo(UsedConsumerCouponInfoEntity usedconsumercouponinfo)
        {
            return CouponDAL.UpdateUsedConsumerCouponInfo(usedconsumercouponinfo);
        }
        public int DelUsedConsumerCouponInfo(int id)
        {
            return CouponDAL.DelUsedConsumerCouponInfo(id);
        }


        public int UpdateExchangeState(ExchangeCouponEntity param)
        {
            return CouponDAL.UpdateExchangeState(param);
        }
        public int UpdateOperationState(ExchangeCouponEntity param)
        {
            return CouponDAL.UpdateOperationState(param);
        }

        public int UpdateTravelIDs(ExchangeCouponEntity param)
        {
            return CouponDAL.UpdateTravelIDs(param);
        }

        public int UpdateOperationRemark(int ID, string Remark)
        {
            return CouponDAL.UpdateOperationRemark(ID,Remark);
        }
        public List<UsedCouponProductEntity> GetUsedCouponProductBySupplierId(int supplierId, DateTime startTime, DateTime endTime, int start = 0, int count = 20)
        {
            return CouponDAL.GetUsedCouponProductBySupplierId(supplierId, startTime, endTime, start, count);
        }

        public List<BookNoUsedExchangeCouponInfoEntity> GetBookNoUsedExchangeCouponBySupplierId(int supplierId, DateTime startTime, DateTime endTime, int state)
        {
            return CouponDAL.GetBookNoUsedExchangeCouponBySupplierId(supplierId, startTime, endTime, state);
        }


        public UsedConsumerCouponInfoEntity GetUsedCouponProductByExchangeNo(string exchangeNo)
        {
            return CouponDAL.GetUsedCouponProductByExchangeNo(exchangeNo);
        }

        public int GetUserCouponByCategoryParentId(long userId, int cParentId)
        {
            return CouponDAL.GetUserCouponByCategoryParentId(userId, cParentId);
        }

        /// <summary>
        /// 获取指定SKUID的消费券产品列表
        /// </summary>
        /// <param name="skuids">String可包含多个SKUID，英文逗号间隔</param>
        /// <returns></returns>
        public List<SKUCouponActivityEntity> GetSKUCouponActivityListBySKUIds(string skuids)
        {
            return CouponDAL.GetSKUCouponActivityListBySKUIds(skuids);
        }
        public List<CouponActivityWithSKUEntity> GetCouponActivityListBySKUIds(List<int> skuids)
        {
            return CouponDAL.GetCouponActivityListBySKUIds(skuids);
        }
        public List<SKUCouponActivityEntity> GetSKUCouponActivityListByAlbumId(int albumId, int start, int count, int districtID, out int totalCount)
        {
            return CouponDAL.GetSKUCouponActivityListByAlbumId(albumId, start, count, districtID, out  totalCount);
        }

        public List<SKUAlbumEntity> GetSKUAlbumEntityListByAlbumId(int albumId, int start, int count, out int totalCount)
        {
            List<SKUAlbumEntity> list = CouponDAL.GetSKUAlbumEntityListByAlbumId(albumId, start, count, out totalCount);
            return list;
        }

        public ProductAlbumSumEntity GetProductAlbumSum(int albumId)
        {
            return CouponDAL.GetProductAlbumSum(albumId);
        }

        public List<SKUCouponActivityEntity> GetOldVIPSKUCouponActivityListByAlbumId(int albumId, int start, int count, int districtID, out int totalCount)
        {
            return CouponDAL.GetOldVIPSKUCouponActivityListByAlbumId(albumId, start, count, districtID, out  totalCount);
        }

        public List<SKUCouponActivityEntity> SKUCouponActivityByCategory(int category = 14, int districtID = 2, double lat = 0, double lng = 0, int geoScopeType = 3, int start = 0, int count = 15, int sort = 0, int payType = 0, double locLat = 0, double locLng = 0)
        {
            return CouponDAL.SKUCouponActivityByCategory(category, districtID, lat, lng, geoScopeType, start, count, sort, payType, locLat, locLng);
        }

        public int SKUCouponActivityByCategoryCount(int category = 14, int districtID = 2, double lat = 0, double lng = 0, int geoScopeType = 3, int payType = 0)
        {
            return CouponDAL.SKUCouponActivityByCategoryCount(category, districtID, lat, lng, geoScopeType, payType);
        }

        public int UpdateExchangeCouponSettle(string ids, string code)
        {
            return CouponDAL.UpdateExchangeCouponSettle(ids, code);
        }

        public List<ExchangeCouponEntity> GetExchangeListByNO(string codes, int type)
        {
            return CouponDAL.GetExchangeListByNO(codes, type);
        }
        public List<ExchangeSettleModel> GetExchangeCheckByNO(string codes)
        {
            return CouponDAL.GetExchangeCheckByNO(codes);
        }
        public List<ExchangeCouponForSettleEntity> GetExchangeCouponSettleList(int AID, int state, string startdate, string enddate, int pageSize, int pageIndex, out int total)
        {
            return CouponDAL.GetExchangeCouponSettleList(AID, state, startdate, enddate, pageSize, pageIndex, out total);
        }

        public int AddRedShare(RedShareEntity redshare)
        {
            return CouponDAL.AddRedShare(redshare);
        }

        public int UpdateRedShare(RedShareEntity redshare)
        {
            return CouponDAL.UpdateRedShare(redshare);
        }
        public List<RedShareEntity> GetRedShareList(int pageIndex, int pageSize, out int total)
        {
            return CouponDAL.GetRedShareList(pageIndex, pageSize, out total);
        }
        public List<RedShareEntity> GetRedShareEntityByGUID(string guid)
        {
            return CouponDAL.GetRedShareEntityByGUID(guid);
        }

        public int AddRetailProduct(RetailProductEntity retailproduct)
        {
            return CouponDAL.AddRetailProduct(retailproduct);
        }


        public int UpdateRetailProduct(RetailProductEntity retailproduct)
        {
            return CouponDAL.UpdateRetailProduct(retailproduct);
        }

        public List<RetailProductEntity> GetRetailProductById(int id)
        {
            return CouponDAL.GetRetailProductById(id);
        }

        public List<CouponActivityRetailEntity> GetRetailProductList(int id, int relBizId, int state, int count, int start, out int totalCount)
        {
            return CouponDAL.GetRetailProductList(id, relBizId, state, count, start, out totalCount);
        }

        /// <summary>
        /// 获取sku是分销产品的列表
        /// </summary>
        /// <param name="count"></param>
        /// <param name="start"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public List<CouponActivityRetailEntity> GetSKURetailProductList(int count, int start, out int totalCount)
        {
            return CouponDAL.GetSKURetailProductList(count, start, out totalCount);
        }
        public CouponActivityRetailDetailEntity GetRetailProductInfoByIDAndCID(int ID, long CID)
        {
            return CouponDAL.GetRetailProductInfoByIDAndCID(ID, CID);
        }

        public List<SKUCouponActivityEntity> GetSKUListByActivityId(int activityId)
        {
            return CouponDAL.GetSKUListByActivityId(activityId);
        }

        public int CopyRetail(int id, long updator)
        {
            return CouponDAL.CopyRetail(id, updator);
        }
        public int ProductAndCouponCopyRetail(int id, long updator)
        {
            return CouponDAL.ProductAndCouponCopyRetail(id, updator);
        }
        public int AddRetailUrl(RetailUrlEntity retailurl)
        {
            return CouponDAL.AddRetailUrl(retailurl);
        }

        /// <summary>
        /// 这里添加方法注释
        /// </summary>
        public int AddSupplierCoupon(SupplierCouponEntity suppliercoupon)
        {
            return CouponDAL.AddSupplierCoupon(suppliercoupon);
        }

        /// <summary>
        /// 这里添加方法注释
        /// </summary>
        public int UpdateSupplierCoupon(SupplierCouponEntity suppliercoupon)
        {
            return CouponDAL.UpdateSupplierCoupon(suppliercoupon);
        }

        public List<SupplierCouponEntity> GetTopCountSupplierCouponBySupplierID(int count, int supplierId, int state)
        {
            return CouponDAL.GetTopCountSupplierCouponBySupplierID(count, supplierId, state);
        }
        public List<SupplierCouponEntity> GetSupplierCouponInfo(int supplierId)
        {
            return CouponDAL.GetSupplierCouponInfo(supplierId);
        }

        public List<CouponActivityRetailEntity> GetSearchProductList(SearchProductRequestEntity param)
        {

            return CouponDAL.GetSearchProductList(param);
        }

        public List<CouponActivityRetailEntity> GetSearchProductListByCategory(SearchProductRequestEntity param)
        {
            return CouponDAL.GetSearchProductListByCategory(param);
        }
        public int GetSearchProductListByCategoryCount(SearchProductRequestEntity param)
        {
            return CouponDAL.GetSearchProductListByCategoryCount(param);
        }
        public int GetSearchProductListCount(SearchProductRequestEntity param)
        {
            return CouponDAL.GetSearchProductListCount(param);
        }


        public int AddUserCouponConsumeLog(UserCouponConsumeLogEntity usercouponconsumelog)
        {
            return CouponDAL.AddUserCouponConsumeLog(usercouponconsumelog);
        }

        public int UpdateUserCouponConsumeLog(UserCouponConsumeLogEntity usercouponconsumelog)
        {
            return CouponDAL.UpdateUserCouponConsumeLog(usercouponconsumelog);
        }

        public int AddUserCouponDefine(UserCouponDefineEntity usercoupondefine)
        {
            return CouponDAL.AddUserCouponDefine(usercoupondefine);
        }

        public int UpdateUserCouponDefine(UserCouponDefineEntity usercoupondefine)
        {
            return CouponDAL.UpdateUserCouponDefine(usercoupondefine);
        }

        public int AddUserCouponItem(UserCouponItemEntity usercouponitem)
        {
            return CouponDAL.AddUserCouponItem(usercouponitem);
        }

        public int UpdateUserCouponItem(UserCouponItemEntity usercouponitem)
        {
            return CouponDAL.UpdateUserCouponItem(usercouponitem);
        }

        public int SendUserCouponItem(List<long> userIds, int couponDefineId, long curUserId)
        {
            int success = 1;
            try
            {
                UserCouponDefineEntity couponDefineEntity = CouponDAL.GetUserCouponDefineByID(couponDefineId);
                UserCouponItemEntity couponItem = new UserCouponItemEntity();
                couponItem.CouponDefineID = couponDefineId;
                couponItem.CreateTime = System.DateTime.Now;
                couponItem.Creator = curUserId;
                if (couponDefineEntity.ExpirationType == 0)
                {
                    couponItem.ExpiredDate = Convert.ToDateTime(System.DateTime.Now.AddDays(couponDefineEntity.ExpirationDay).ToString("yyyy-MM-dd 23:59:59"));
                    couponItem.StartDate = System.DateTime.Now;
                }
                else
                {
                    couponItem.ExpiredDate = Convert.ToDateTime(couponDefineEntity.ValidUntilDate.ToString("yyyy-MM-dd 23:59:59"));
                    couponItem.StartDate = couponDefineEntity.StartUseDate;
                }
                couponItem.RestAmount = couponDefineEntity.DiscountAmount;
                couponItem.State = 0;
                foreach (long userId in userIds)
                {
                    couponItem.UserID = userId;
                    CouponDAL.AddUserCouponItem(couponItem);
                }
            }
            catch (Exception e)
            {
                success = 0;
            }
            return success;
        }

        /// <summary>
        /// 为用户发券，但仅发一次
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="couponDefineId"></param>
        /// <param name="curUserId"></param>
        /// <returns></returns>
        public bool SendOneUserCouponItemOnlyOneTime(long userId, int couponDefineId, long curUserId)
        {
            bool bResult = true;

            int itemID =  GetUserCouponItemByUserIdAndCouponDefineId( userId,  couponDefineId);
            if( itemID == 0)
            {
                SendOneUserCouponItem(userId, couponDefineId, curUserId);
            }
            else
            {
                bResult = false;
            }

            return bResult;
        }

        public int SendOneUserCouponItem(long userId, int couponDefineId, long curUserId)
        {
            int success = 1;
            try
            {
                UserCouponDefineEntity couponDefineEntity = CouponDAL.GetUserCouponDefineByID(couponDefineId);
                UserCouponItemEntity couponItem = new UserCouponItemEntity();
                couponItem.CouponDefineID = couponDefineId;
                couponItem.CreateTime = System.DateTime.Now;
                couponItem.Creator = curUserId;
                if (couponDefineEntity.ExpirationType == 0)
                {
                    couponItem.ExpiredDate = Convert.ToDateTime(System.DateTime.Now.AddDays(couponDefineEntity.ExpirationDay).ToString("yyyy-MM-dd 23:59:59"));
                    couponItem.StartDate = System.DateTime.Now;
                }
                else
                {
                    couponItem.ExpiredDate = Convert.ToDateTime(couponDefineEntity.ValidUntilDate.ToString("yyyy-MM-dd 23:59:59"));
                    couponItem.StartDate = couponDefineEntity.StartUseDate;
                }
                couponItem.RestAmount = couponDefineEntity.DiscountAmount;
                couponItem.State = 0;
                couponItem.UserID = userId;
                success = CouponDAL.AddUserCouponItem(couponItem);
            }
            catch (Exception e)
            {
                success = 0;
            }
            return success;
        }


        public List<UserCouponDefineEntity> GetUserCouponDefineListByType(int type)
        {
            return CouponDAL.GetUserCouponDefineListByType(type);
        }

        public UserCouponDefineEntity GetUserCouponDefineByID(int id)
        {
            return CouponDAL.GetUserCouponDefineByID(id);
        }

        public List<UserCouponDefineEntity> GetUserCouponDefineListByIds(string ids, long userId)
        {
            return CouponDAL.GetUserCouponDefineListByIds(ids, userId);
        }

        public int GetUserCouponItemByUserId(long userId, int couponDefineId)
        {
            return CouponDAL.GetUserCouponItemByUserId(userId, couponDefineId);
        }

        public List<UserCouponItemInfoEntity> GetUserCouponInfoListByUserId(long userId, UserCouponState state)
        {
            List<UserCouponItemInfoEntity> cashCouponList = CouponDAL.GetUserCouponInfoListByUserId(userId, state);
            foreach (var item in cashCouponList)
            {
                item.IsShowExpireTips = (item.ExpiredDate - System.DateTime.Now).Days > 30 ? false : true;
                item.CashCouponName = GetCashCouponTypeName((UserCouponType)item.UserCouponType, item.RequireAmount, item.DiscountAmount);
            }
            return cashCouponList;
        }

        public List<UserCouponItemInfoEntity> GetNewVIPGiftUserCouponInfoListByUserId(long userId)
        {
            return CouponDAL.GetNewVIPGiftUserCouponInfoListByUserId(userId);
        }

        public bool UpdateUserCoupoinItemByIDs(List<Int32> IDs, Int32 CouponState)
        {
            return CouponDAL.UpdateUserCoupoinItemByIDs(IDs, CouponState);
        }

        public List<UserCouponDefineEntity> GetCouponDefineByIntval(int intval, CondationType condationtype)
        {
            return CouponDAL.GetCouponDefineByIntval(intval, condationtype);
        }

        public int GetUserCouponItemByUserIdAndCouponDefineId(long userId, int couponDefineId)
        {
            return CouponDAL.GetUserCouponItemByUserIdAndCouponDefineId(userId, couponDefineId);
        }

        public List<UserCouponItemInfoEntity> GetUserCouponInfoListByUserIdAndType(long userId, UserCouponState state, UserCouponType type)
        {
            List<UserCouponItemInfoEntity> cashCouponList = CouponDAL.GetUserCouponInfoListByUserIdAndType(userId, state, type);
            foreach (var item in cashCouponList)
            {
                item.IsShowExpireTips = (item.ExpiredDate - System.DateTime.Now).Days > 30 ? false : true;
                item.CashCouponName = GetCashCouponTypeName((UserCouponType)item.UserCouponType, item.RequireAmount, item.DiscountAmount);
                List<UserCouponUseCondationEntity> useCondationList = GetUserCouponUserCondationList(item.CouponDefineID);
                item.UseCondationList = useCondationList == null ? new List<UserCouponUseCondationEntity>() : useCondationList;
            }
            return cashCouponList;
        }

        public List<UserCouponUseCondationEntity> GetUserCouponUserCondationList(int couponDefineId)
        {
            return CouponDAL.GetUserCouponUserCondationList(couponDefineId);
        }
        public  List<UserCouponUseCondationEntity> GetUserCouponUserCondationListByUCDList(List<int> couponDefineIdList)
        {
            return CouponDAL.GetUserCouponUserCondationListByUCDList(couponDefineIdList);
        }
        public List<UserCouponUseCondationEntity> GetUserCouponUserCondationListByTypeAndSourceID(CondationType type, int SourceID)
        {
            return CouponDAL.GetUserCouponUserCondationListByTypeAndSourceID(type,  SourceID);
        }
        public string GetCashCouponTypeName(UserCouponType type, decimal RequireAmount, decimal DiscountAmount)
        {
            string CashCouponTypeName = "";
            switch (type)
            {
                case UserCouponType.DiscountOverPrice:
                    CashCouponTypeName = string.Format("满{0}减{1}", Convert.ToInt32(RequireAmount), Convert.ToInt32(DiscountAmount));
                    break;
                case UserCouponType.DiscountUnconditional:
                    CashCouponTypeName = "立减金";
                    break;
                case UserCouponType.DiscountVoucher:
                    CashCouponTypeName = "代金券";
                    break;
            }

            return CashCouponTypeName;
        }
        public int GetUserCouponInfoCountByUserId(int userId, UserCouponState state)
        {
            return CouponDAL.GetUserCouponInfoCountByUserId(userId, state);
        }
        public int GetUserCouponInfoCountByUserIdAndType(long userId, UserCouponState state, UserCouponType type)
        {
            return CouponDAL.GetUserCouponInfoCountByUserIdAndType(userId, state, type);
        }
        public UserCouponItemInfoEntity GetUserCouponItemInfoByID(int id)
        {
            return CouponDAL.GetUserCouponItemInfoByID(id);
        }

        public UserCouponItemEntity GetUserCouponItemByID(int id)
        {
            return CouponDAL.GetUserCouponItemByID(id);
        }

        public RequestResultEntity UseUserCouponInfoItem(UseCashCouponItem param)
        {
            RequestResultEntity result = new RequestResultEntity();
            UserCouponItemInfoEntity userCouponItemInfo = CouponDAL.GetUserCouponItemInfoByID(param.CashCouponID);
            if (userCouponItemInfo.State == 0)
            {
                if (userCouponItemInfo.RestAmount >= param.UseCashAmount)
                {
                    UserCouponItemEntity model = CouponDAL.GetUserCouponItemByID(param.CashCouponID);
                    //立减
                    if (userCouponItemInfo.UserCouponType == (int)UserCouponType.DiscountUnconditional)
                    {
                        model.RestAmount = model.RestAmount - param.UseCashAmount;
                        if (model.RestAmount == 0)
                        {
                            model.State = (int)UserCouponState.used;
                        }
                        CouponDAL.UpdateUserCouponItem(model);
                    }
                    else if (userCouponItemInfo.UserCouponType == (int)UserCouponType.DiscountOverPrice || userCouponItemInfo.UserCouponType == (int)UserCouponType.DiscountVoucher)
                    {
                        model.State = (int)UserCouponState.used;
                        CouponDAL.UpdateUserCouponItem(model);
                    }
                    // 写入使用明细表

                    UserCouponConsumeLogEntity logModel = new UserCouponConsumeLogEntity();
                    logModel.ConsumeAmount = param.UseCashAmount;
                    logModel.CreateTime = System.DateTime.Now;
                    logModel.OrderID = param.OrderID;
                    logModel.OrderType = param.OrderType;
                    logModel.UserCouponItemID = param.CashCouponID;
                    logModel.ConsumeType = 0;
                    CouponDAL.AddUserCouponConsumeLog(logModel);
                    result.Message = "已抵扣";
                    result.RetCode = "0";
                    return result;
                }
                else
                {
                    result.Message = "抵扣金额大于券剩余金额";
                    result.RetCode = "1";
                    return result;
                }
            }
            else if (userCouponItemInfo.State == 1)
            {
                result.Message = "该券已使用";
                result.RetCode = "1";
                return result;
            }
            else if (userCouponItemInfo.State == 2)
            {
                result.Message = "该券已过期";
                result.RetCode = "2";
                return result;
            }
            else if (userCouponItemInfo.State == 3)
            {
                result.Message = "该券已取消";
                result.RetCode = "3";
                return result;
            }

            return result;
        }

        public RequestResultEntity CancelUseUserCouponInfoItem(UseCashCouponItem param)
        {
            RequestResultEntity result = new RequestResultEntity();
            UserCouponItemInfoEntity userCouponItemInfo = CouponDAL.GetUserCouponItemInfoByID(param.CashCouponID);

            if (userCouponItemInfo.IDX > 0)
            {
                UserCouponItemEntity model = CouponDAL.GetUserCouponItemByID(param.CashCouponID);
                if (param.CashCouponType == (int)UserCouponType.DiscountUnconditional && (model.RestAmount + param.UseCashAmount) > userCouponItemInfo.DiscountAmount)
                {

                    result.Message = "取消失败，退款金额大于总金额";
                    result.RetCode = "2";
                    return result;
                }
                else
                {
                    model.State = (int)UserCouponState.log;
                    if (param.CashCouponType == (int)UserCouponType.DiscountUnconditional)
                    {
                        model.RestAmount = model.RestAmount + param.UseCashAmount;
                    }
                    CouponDAL.UpdateUserCouponItem(model);

                    // 写入使用明细表
                    UserCouponConsumeLogEntity logModel = new UserCouponConsumeLogEntity();
                    logModel.ConsumeAmount = param.UseCashAmount;
                    logModel.CreateTime = System.DateTime.Now;
                    logModel.OrderID = param.OrderID;
                    logModel.OrderType = param.OrderType;
                    logModel.UserCouponItemID = param.CashCouponID;
                    logModel.ConsumeType = 1;
                    CouponDAL.AddUserCouponConsumeLog(logModel);
                    result.Message = "取消成功";
                    result.RetCode = "0";
                    return result;
                }
            }
            else
            {
                result.Message = "未找到券";
                result.RetCode = "1";
                return result;
            }
        }

        public int AddUserCouponUseCondation(UserCouponUseCondationEntity usercouponusecondation)
        {
            return CouponDAL.AddUserCouponUseCondation(usercouponusecondation);
        }

        public int UpdateUserCouponUseCondation(UserCouponUseCondationEntity usercouponusecondation)
        {
            return CouponDAL.UpdateUserCouponUseCondation(usercouponusecondation);
        }

        public int DeleteUserCouponUseCondation(int idx)
        {
            return CouponDAL.DeleteUserCouponUseCondation(idx);
        }
        public List<UserCouponUseCondationEntity> GetCouponCondationByCouponDefineId(int couponDefineId)
        {
            return CouponDAL.GetCouponCondationByCouponDefineId(couponDefineId);
        }

        public List<UserCouponUseCondationEntity> GetCouponCondationByIntVal(int intVal, int condationType)
        {
            return CouponDAL.GetCouponCondationByIntVal(intVal, condationType);
        }

        public List<UserCouponDefineEntity> GetCouponDefineByIntVals(string intVals, int condationType, long userId)
        {
            return CouponDAL.GetCouponDefineByIntVals(intVals, condationType, userId);
        }

        public List<UserCouponConsumeLogEntity> GetUserCouponLogByCouponItemID(int idx)
        {
            return CouponDAL.GetUserCouponLogByCouponItemID(idx);
        }
        public bool GetProductAlbumSKUBySKUIDAndAlbumId(int skuid, int albumid)
        {
            return CouponDAL.GetProductAlbumSKUBySKUIDAndAlbumId(skuid, albumid);
        }

        public List<UserCouponItemInfoEntity> GetUserCouponInfoAllListByUserId(long userId)
        {
            List<UserCouponItemInfoEntity> cashCouponList = CouponDAL.GetUserCouponInfoAllListByUserId(userId);
            foreach (var item in cashCouponList)
            {
                item.IsShowExpireTips = (item.ExpiredDate - System.DateTime.Now).Days > 30 ? false : true;
                item.CashCouponName = GetCashCouponTypeName((UserCouponType)item.UserCouponType, item.RequireAmount, item.DiscountAmount);
            }
            return cashCouponList;
        }

        #region   使用代金券

        //        获取指定产品类型指定金额下，所有可用的券（订单确认页使用，暂时可以不分页）
        public List<UserCouponItemInfoEntity> GetCanUseVoucherInfoListForOrder(OrderUserCouponRequestParamsBase req)
        {
            var list = GetShortedVoucherInfoListForOrder(req);

            return list.Where(_ => _.State == (int)UserCouponState.log).ToList();
        }
        //获取指定产品类型指定金额下，所有不可用的券（订单确认页使用，暂时可以不分页）

        public List<UserCouponItemInfoEntity> GetCannotUseVoucherInfoListForOrder(OrderUserCouponRequestParamsBase req)
        {
            var list = GetShortedVoucherInfoListForOrder(req);
            return list.Where(_ => _.State != (int)UserCouponState.log).ToList();
        }

        IOrderedEnumerable<UserCouponItemInfoEntity> GetShortedVoucherInfoListForOrder(OrderUserCouponRequestParamsBase req)
        {
            var list = GetUserCouponInfoListByUserIdAndType(req.UserID, UserCouponState.log, UserCouponType.DiscountVoucher);

            //排序： 可用 、专用、 过期时间最近、优惠金额 
            int skuid = req.OrderTypeID == CashCouponOrderSorceType.sku ? req.OrderSourceID : 0;

            var canNotUseList = list.Where(_ => _.StartDate > DateTime.Now || _.ExpiredDate < DateTime.Now
                 || (_.UseCondationList.Count > 0 &&
                 _.UseCondationList.Where(c => c.CondationType == (int)CondationType.sku && c.IntVal == skuid).Count() == 0)
                 );

            foreach (var q in canNotUseList)
            {
                q.State = (int)UserCouponState.used;
            }


            //订单可用金额不大于订单金额
            //   list.ForEach(_ => _.OrderCanUseDiscountAmount = _.RestAmount > req.TotalOrderPrice ? req.TotalOrderPrice : _.RestAmount);

            return list.OrderBy(_ => _.State)
                .ThenBy(_ => _.UseCondationList.Count > 0 ? 0 : 1)
                  .ThenBy(_ => _.ExpiredDate.Date)
                  .ThenByDescending(_ => _.RestAmount);
        }



        #endregion

        #region 使用券
        //        获取指定产品类型指定金额下，所有可用的券（订单确认页使用，暂时可以不分页）
        public List<UserCouponItemInfoEntity> GetCanUseCouponInfoListForOrder(OrderUserCouponRequestParams req)
        {
            if (req.UserID > 0)
            {
                var list = GetShortedUseCouponInfoListForOrder(req);

                return list.Where(_ => _.State == (int)UserCouponState.log).ToList();
            }
            else
            {
                return new List<UserCouponItemInfoEntity>();
            }
        }
        //获取指定产品类型指定金额下，所有不可用的券（订单确认页使用，暂时可以不分页）

        public List<UserCouponItemInfoEntity> GetCannotUseCouponInfoListForOrder(OrderUserCouponRequestParams req)
        {         
            if (req.UserID > 0)
            {
                var list = GetShortedUseCouponInfoListForOrder(req);
                return list.Where(_ => _.State != (int)UserCouponState.log).ToList();
            }
            else
            {
                return new List<UserCouponItemInfoEntity>();
            }

        }

        //获取指定产品类型指定金额下，默认最优惠的券（订单确认页需要默认选择一个券）        
        public UserCouponItemInfoEntity GetTheBestCouponInfoForOrder(OrderUserCouponRequestParams req)
        {
            if (req.UserID > 0)
            {
                var list = GetShortedUseCouponInfoListForOrder(req).Where(_ => _.State == (int)UserCouponState.log);

                if (list.Count() > 0)
                {
                    return list.First();
                }
                else
                {
                    return new UserCouponItemInfoEntity();
                }
            }
            else
            {
                return new UserCouponItemInfoEntity();
            }
        }

        IOrderedEnumerable<UserCouponItemInfoEntity> GetShortedUseCouponInfoListForOrder(OrderUserCouponRequestParams req)
        {
         
            var list = GetUserCouponInfoListByUserId(req.UserID, UserCouponState.log);

            if (req.CanNotUseDiscountOverPrice == 1)//产品不可以使用满减券
            {
                foreach (var l in list.Where(l => l.UserCouponType == (int)UserCouponType.DiscountOverPrice))
                {
                    l.State = (int)UserCouponState.used;
                    l.Description += " *该产品不能使用满减券";
                }
            }
            else if (req.OrderTypeID == CashCouponOrderSorceType.sku || req.OrderTypeID == CashCouponOrderSorceType.hotelPackage)
            {
                   //当前请的的SKUID
                 int sourceID =  req.OrderSourceID  ;
                CondationType condationType =  (CashCouponOrderSorceType)req.OrderTypeID == CashCouponOrderSorceType.sku? CondationType.sku : CondationType.hotelPackage ;

                // 指定了SKU的券过滤
                var condationList = GetUserCouponUserCondationListByUCDList(list.Select(_ => _.CouponDefineID).Distinct().ToList());

                if (condationList.Count > 0)
                {                 
                    //不可用券ID列表
                    var cidlist = condationList.Select(_ => _.CouponDefineID).Distinct();
                    
                   //去除当前SKUID可用的券ID
                    var skuCanUserCouponDefineList = condationList.Where(_ => _.CondationType == (int)condationType && _.IntVal == sourceID);
                    if( skuCanUserCouponDefineList.Count()>0)
                    {
                        var skuCanUserCouponDefineIDList = skuCanUserCouponDefineList.Select(_=>_.CouponDefineID);
                        cidlist = cidlist.Where(_=>  !skuCanUserCouponDefineIDList.Contains(_));
                    }
                 

                    foreach (var l in list.Where(_ => cidlist.Contains(_.CouponDefineID)))
                    { 
                        l.State = (int)UserCouponState.used;
                        l.Description += " *该券只能使用在指定产品上";
                    }
                }
            }


            //排序： 可用  、过期时间最近、优惠金额、立减金、满减券
            foreach (var q in list.Where(_ => _.UserCouponType == (int)UserCouponType.DiscountOverPrice && _.RequireAmount > req.TotalOrderPrice))
            {
                q.State = (int)UserCouponState.used;
            }
            //订单可用金额不大于订单金额
            list.ForEach(_ => _.OrderCanUseDiscountAmount = _.RestAmount > req.TotalOrderPrice ? req.TotalOrderPrice : _.RestAmount);

            return list.OrderBy(_ => _.State)
                  .ThenBy(_ => _.ExpiredDate.Date)
                  .ThenByDescending(_ => _.RestAmount)
                  .ThenBy(_ => _.UserCouponType == (int)UserCouponType.DiscountUnconditional ? 0 : 1);
        }

        #endregion




        public int AddCouponOrderOperation(CouponOrderOperationEntity couponorderoperation)
        {
            return CouponDAL.AddCouponOrderOperation(couponorderoperation);
        }

        public int UpdateCouponOrderOperation(CouponOrderOperationEntity couponorderoperation)
        {
            return CouponDAL.UpdateCouponOrderOperation(couponorderoperation);
        }


        public int AddRedPool(RedPoolEntity redpool)
        {
            return CouponDAL.AddRedPool(redpool);
        }

        public int UpdateRedPool(RedPoolEntity redpool)
        {
            return CouponDAL.UpdateRedPool(redpool);
        }


        public int AddRedPoolDetail(RedPoolDetailEntity redpooldetail)
        {
            return CouponDAL.AddRedPoolDetail(redpooldetail);
        }

        public int UpdateRedPoolDetail(RedPoolDetailEntity redpooldetail)
        {
            return CouponDAL.UpdateRedPoolDetail(redpooldetail);
        }

        public int AddRedRecord(RedRecordEntity redrecord)
        {
            return CouponDAL.AddRedRecord(redrecord);
        }

        public int UpdateRedRecord(RedRecordEntity redrecord)
        {
            return CouponDAL.UpdateRedRecord(redrecord);
        }

        public int AddRedActivity(RedActivityEntity redactivity)
        {
            return CouponDAL.AddRedActivity(redactivity);
        }

        public int UpdateRedActivity(RedActivityEntity redactivity)
        {
            return CouponDAL.UpdateRedActivity(redactivity);
        }

        public List<RedPoolEntity> GetRedPoolListByType(RedPoolType type)
        {
            return CouponDAL.GetRedPoolListByType(type);
        }

        public RedPoolEntity GetRedPoolByID(int id)
        {
            return CouponDAL.GetRedPoolByID(id);
        }

        public List<RedPoolDetailEntity> GetRedDetailByPoolId(int poolId)
        {
            List<RedPoolDetailEntity> redPoolDetailList = CouponDAL.GetRedDetailByPoolId(poolId);
            foreach (RedPoolDetailEntity item in redPoolDetailList)
            {
                if (item.BizType == 0 || item.BizType == 1)
                {
                    item.BizName = CouponDAL.GetUserCouponDefineByID(item.BizID).Name;
                }
            }
            return redPoolDetailList;
        }

        public RedPoolDetailEntity GetRedDetailBylId(int Id)
        {
            return CouponDAL.GetRedDetailBylId(Id);
        }

        public List<RedActivityEntity> GetRedActivityListByType(RedActivityType type)
        {
            return CouponDAL.GetRedActivityListByType(type);
        }

        public RedActivityEntity GetRedActivityById(int id)
        {
            return CouponDAL.GetRedActivityById(id);
        }

        public RedActivityEntity GetRedActivityByGUID(string guid)
        {
            return CouponDAL.GetRedActivityByGUID(guid);
        }

        public RedRecordEntity GetRedRecordByRedActivityIdAndPhone(int activityId, string phone)
        {
            RedRecordEntity redRecord = CouponDAL.GetRedRecordByRedActivityIdAndPhone(activityId, phone);
            redRecord.CouponTypeName = GetRedRecordTypeName((RedRecordType)redRecord.BizType, redRecord.RequireAmount, redRecord.DiscountAmount);
            return redRecord;
        }

        public List<RedRecordEntity> GetRedRecordByActivityId(int activityId, int count, int start, out int totalCount)
        {
            List<RedRecordEntity> redRecordList = CouponDAL.GetRedRecordByActivityId(activityId, count, start, out totalCount);
            foreach (RedRecordEntity item in redRecordList)
            {
                item.CouponTypeName = GetRedRecordTypeName((RedRecordType)item.BizType, item.RequireAmount, item.DiscountAmount, 1);
                item.NickName = string.IsNullOrEmpty(item.NickName) ? (item.PhoneNum.Substring(0, 3) + "****" + item.PhoneNum.Substring(7)) : item.NickName;
            }
            return redRecordList;
        }

        public string GetRedRecordTypeName(RedRecordType type, decimal RequireAmount, decimal DiscountAmount, int redType = 0)
        {
            string RedRecordTypeName = "";
            if (redType == 0)
            {
                switch (type)
                {
                    case RedRecordType.DiscountOver:
                        RedRecordTypeName = string.Format("满{0}减{1}", Convert.ToInt32(RequireAmount), Convert.ToInt32(DiscountAmount));
                        break;
                    case RedRecordType.DiscountUnconditional:
                        RedRecordTypeName = "立减金 " + Convert.ToInt32(DiscountAmount) + "元";
                        break;
                    case RedRecordType.DiscountVoucher:
                        RedRecordTypeName = "代金券 " + Convert.ToInt32(DiscountAmount) + "元";
                        break;
                    case RedRecordType.ExchangeCoupon:
                        RedRecordTypeName = "消费券";
                        break;
                }
            }
            else
            {
                switch (type)
                {
                    case RedRecordType.DiscountOver:
                    case RedRecordType.DiscountUnconditional:
                    case RedRecordType.DiscountVoucher:
                        RedRecordTypeName = "¥" + Convert.ToInt32(DiscountAmount);
                        break;
                    case RedRecordType.ExchangeCoupon:
                        RedRecordTypeName = "消费券";
                        break;
                }
            }

            return RedRecordTypeName;
        }

        public List<RedPoolEntity> GetRedPoolByOrderPrice(decimal price)
        {
            return CouponDAL.GetRedPoolByOrderPrice(price);
        }

        public List<RedActivityEntity> GetRedActivityByBizIDAndBizType(long bizId, RedActivityType bizType)
        {
            return CouponDAL.GetRedActivityByBizIDAndBizType(bizId, bizType);
        }

        public RedRecordEntity GetRedRecordById(int id)
        {
            RedRecordEntity redRecord = CouponDAL.GetRedRecordById(id);
            redRecord.CouponTypeName = GetRedRecordTypeName((RedRecordType)redRecord.BizType, redRecord.RequireAmount, redRecord.DiscountAmount);
            return redRecord;
        }


        public int UpdateExchangeSettlePrice(decimal SettlePrice, int ExchangeID, long curuserID, string OperationRemark = "", int supplierID = 0)
        {
            return CouponDAL.UpdateExchangeSettlePrice(SettlePrice, ExchangeID, curuserID, OperationRemark, supplierID);
        }

        public string GetThirdCouponOrderID(int couponid, int type)
        {
            return CouponDAL.GetThirdCouponOrderID(couponid, type);
        }
        public int Insert2Refund(RefundCouponsEntity rce)
        {
            return CouponDAL.Insert2Refund(rce);
        }


        public int UpdateCouponActivityPicPath(int activityid, string picpath)
        {
            return CouponDAL.UpdateCouponActivityPicPath(activityid, picpath);
        }

        public string GetCouponActivityPicBySPUID(int spuid)
        {
            return CouponDAL.GetCouponActivityPicBySPUID(spuid);
        }
        public List<ExchangeCouponEntity> GetExchangeOrderListBySupplierID(int supplierID)
        {
            return CouponDAL.GetExchangeOrderListBySupplierID(supplierID);
        }
       
        public ExchangeCouponEntity GetExchangCouponByCouponId(int couponId)
        {
            return CouponDAL.GetExchangCouponByCouponId(couponId);
        }

        public List<ExchangeCouponEntity> GetExchangCouponByCouponOrderID(long couponOrderID)
        {
            return new List<ExchangeCouponEntity>();
            return CouponDAL.GetExchangCouponByCouponOrderID(couponOrderID);
        }
        public int GetExchangCouponCountByCouponOrderID(long couponOrderID, int state)
        {
            //return new List<ExchangeCouponEntity>();
            return CouponDAL.GetExchangCouponCountByCouponOrderID(couponOrderID, state);
        }
        /// <summary>
        /// 获取过期不退款券数据
        /// </summary>
        /// <returns></returns>
        public List<ExchangeCouponEntity> GetExpiredNotRefundExchangeCouponList()
        {
            return CouponDAL.GetExpiredNotRefundExchangeCouponList();
        }
        /// <summary>
        /// 更新过期不退款券数据
        /// </summary>
        /// <returns></returns>
        public int  UpdateExchangeCouponExpiredNotRefund()
        {
            return CouponDAL.UpdateExchangeCouponExpiredNotRefund();
        }
        /// <summary>
        /// 根据couponId修改备注
        /// </summary>
        /// <param name="couponId"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        public int UpdateOperationRemarkByCouponId(int couponId, string remark)
        {
            return CouponDAL.UpdateOperationRemarkByCouponId(couponId, remark);
        }
        public List<SKUCouponActivityEntity> SKUCouponActivityByID(int ID = 0, int districtID = 2, double lat = 0, double lng = 0, int geoScopeType = 3, int start = 0, int count = 15, int sort = 0, int payType = 0, double locLat = 0, double locLng = 0)
        {
            return CouponDAL.SKUCouponActivityByID(ID, districtID, lat, lng, geoScopeType, start, count, sort, payType, locLat, locLng);
        }
        /// <summary>
        /// 修改第三方券码状态
        /// </summary>
        /// <param name="suppliercoupon"></param>
        /// <returns></returns>
        public int UpdateSupplierCouponState(SupplierCouponEntity suppliercoupon, int couponID=0)
        {
            return CouponDAL.UpdateSupplierCouponState(suppliercoupon,couponID);
        }

        public UserCouponUseCondationEntity GetUserCouponUseCondationEntity(int idx)
        {
            return CouponDAL.GetUserCouponUseCondationEntity(idx);
        }
        public List<UserCouponDefineEntity> GetCouponCondationByMarketingType(int intval, int condationtype, int marketingType)
        {
            return CouponDAL.GetCouponCondationByMarketingType(intval, condationtype, marketingType);
        }

    }


}