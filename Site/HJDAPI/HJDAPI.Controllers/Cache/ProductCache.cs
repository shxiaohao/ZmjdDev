using HJD.CouponService.Contracts;
using HJD.CouponService.Contracts.Entity;
using HJD.Framework.Interface;
using HJD.Framework.WCF;
using HJDAPI.Common.Helpers;
using HJDAPI.Controllers.Adapter;
using HJDAPI.Controllers.Common;
using HJDAPI.Models;
using HJDAPI.Models.Coupon;
using ProductService.Contracts.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.Controllers.Cache
{
    public class ProductCache
    {
        public static ICouponService couponSvc = ServiceProxyFactory.Create<ICouponService>("ICouponService");
        public static IMemcacheProvider ActivityCouponSellNumCache = MemCacheHelper.memcached30min;//抢券活动缓存 30分钟

        private static IMemcacheProvider UserDetailOrderMemCache = MemCacheHelper.memcached30min;

       
        public static IMemcacheProvider StepGroupCache = MemCacheHelper.memcached30min;//抢券活动缓存 30分钟


           #region Key Gen
        internal static string GenStepGroupCahceKey(int SPUID)
        {
            return "StepGroupCahceKey:" + SPUID.ToString();
        }
        internal static string GenStepGroupWithSKUIDCahceKey(int SKUID)
        {
            return "StepGroupWithSKUIDCahceKey:" + SKUID.ToString();
        }
        internal static string GenCouponActivitySKUSPURelCacheKey(int activityID)
        {
            return "CouponActivitySKUSPURelCacheKey:" + activityID.ToString();
        }


        const string ActivityLockNumFromMemcachePreKey = "GetActivityLockNumFromMemcache";

        public static string GenActivityLockNumCacheKey(int activityID)
        {
           return  string.Format("{0}:{1}", ActivityLockNumFromMemcachePreKey, activityID);
        }

        public static string GenCurrentActivityNumMemKey(int activityID)
        {
            return "CurrentActivityNumMem:" + activityID.ToString();
        }

         
        internal static string GenCouponSellNumCacheKey(int activityID)
        {
            return "CouponSellNumCacheKey:" + activityID.ToString();
        }


        const string CouponUserLockNumFromMemcachePreKey = "GetCouponUserLockNumFromMemcacheV1";

        internal static string GenCouponUserLockNumCacheKey(int activityID, long userID)
        {
            return string.Format("{0}:{1}_{2}", CouponUserLockNumFromMemcachePreKey, activityID, userID);
        }

        #endregion


        #region OrderListItemEntity

        /// <summary>
        /// 从缓存中获取订单详情
        /// </summary>
        /// <param name="detailOrderIDs"></param>
        /// <returns></returns>
        public static List<OrderListItemEntity> GetOrderListItemEntityFromMemCache(List<long> detailOrderIDs)
        {
            return UserDetailOrderMemCache.GetMultiDataEx<long, OrderListItemEntity>(detailOrderIDs, MemCacheHelper.Prefix.UserDetailOrderCacheKey,
                 nolist =>
                 {  //获取缓存中不存在的对象列表
                     List<OrderListItemEntity> list = new List<OrderListItemEntity>();
                     var packageOrderList = nolist.Where(_ => OrderHelper.IsCommOrder(_) == false).ToList();
                     if (packageOrderList.Count > 0)
                     {
                         list = OrderAdapter.GetUserOrderInfoEntityByOrderIDList(packageOrderList);
                     }

                     var couponOrderList = nolist.Where(_ => OrderHelper.IsCommOrder(_)).ToList();
                     if (couponOrderList.Count > 0)
                     {
                         list.AddRange(CouponAdapter.GetUserOrderInfoEntityByOrderIDList(couponOrderList));
                     }

                     return list;
                 },
                 holl => holl.OrderId, //nolist返回对象的缓存主键是什么，
                //如果数据。返回什么
                 new OrderListItemEntity { OrderId = 0 }
         );
        }

        /// <summary>
        /// 批量清除订单状态缓存
        /// </summary>
        /// <param name="DetailOrderIdList">订单明细ID 券： exchangeCoupon.ID 酒店订单：orders.orderid </param>
        /// <returns></returns>
        public static bool RemoveUserDetailOrderCacheWithOrderIDList(List<long> DetailOrderIdList)
        {
            DetailOrderIdList.Distinct().ToList().ForEach(DetailOrderId => RemoveUserDetailOrderCache(DetailOrderId)); 
            return true;
        }



        /// <summary>
        /// 清除订单状态缓存
        /// </summary>
        /// <param name="DetailOrderId">订单明细ID 券： exchangeCoupon.ID 酒店订单：orders.orderid </param>
        /// <returns></returns>
        public static bool RemoveUserDetailOrderCache(long DetailOrderId)
        {
            UserDetailOrderMemCache.Remove(DetailOrderId.ToString(), MemCacheHelper.Prefix.UserDetailOrderCacheKey);
            return true;
        }

        #endregion

        #region UserActivityNum
        public static CouponUserLockNumResult GetCouponUserLockNumFromMemcache(int activityID, long userID)
        {
            return MemCacheHelper.memcached5min.GetData<CouponUserLockNumResult>(GenCouponUserLockNumCacheKey(activityID, userID), () =>
            {
                var ret = GenCouponUserLockNum(activityID, userID);//取最新的券活动购买数据
                //memcached5min.Set(keyName, ret);
                return ret;
            });
        }
        public static void RemoveCouponUserLockNumCache(int activityID, long userID)
        {
            MemCacheHelper.memcached5min.Remove(GenCouponUserLockNumCacheKey(activityID, userID));
        }


        public static void SetCurrentUserLockNum(int activityID, long userID, CouponUserLockNumResult result)
        {
            MemCacheHelper.memcached5min.Set(GenCouponUserLockNumCacheKey(activityID, userID), result);
        }


        /// <summary>
        /// 获取用户券已购买份数
        /// </summary>
        /// <param name="activityID"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        private static CouponUserLockNumResult GenCouponUserLockNum(int activityID, long userID)
        {

            var result = new CouponUserLockNumResult();

            //获得支付链接的 用户 并未付款成功 此时会一直锁定 状态 1.已提交（点了购买按钮 待支付） 2.支付完成（未消费） 3.已消费 4.已取消 5.已退款 6.支付过期
            if (userID > 0)
            {
                var userCouponList = couponSvc.GetExchangeCouponEntityListByActivityIDAndUserID(activityID, userID);
                var notPayList = userCouponList.FindAll(_ => _.State == 1);//未支付
                var hasPayList = userCouponList.FindAll(_ => _.State == 2 || _.State == 3);//已支付或已兑换

                if (userCouponList != null && userCouponList.Count != 0)
                {
                    result.HasLockedNum = userCouponList.Where(_ => _.State == (int)ExchangeCouponState.exchanged
                        || _.State == (int)ExchangeCouponState.paied
                        || _.State == (int)ExchangeCouponState.paiedOverTime
                        || _.State == (int)ExchangeCouponState.submit
                        || _.State == (int)ExchangeCouponState.refund).Count()
                        -
                        userCouponList.Where(_ => _.RefundState > 0).Count() //已退款或退款中的不退库存                        
                        ;
                }

                if (notPayList != null && notPayList.Count != 0)
                {
                    result.OrderID = notPayList.First().PayID;//支付单号
                    result.NotPaiedCount = notPayList.Count();
                }
                else if (hasPayList != null && hasPayList.Count != 0)
                {
                    result.OrderID = hasPayList.First().PayID;//支付单号
                    result.PayType = hasPayList.First().PayType;//支付方式
                }
            }
            return result;
        }

        #endregion


        #region CouponSellNum
        
        /// <summary>
        /// 获取一个券产品的销售数量
        /// </summary>
        /// <param name="activityID"></param>
        /// <returns></returns>
        public static CouponActivitySellNumEntity GetOneActivityCouponSellNumCache(int activityID)
        {
            return ActivityCouponSellNumCache.GetData<CouponActivitySellNumEntity>(GenCouponSellNumCacheKey(activityID), () =>
            {
                var list = couponSvc.GetCouponActivitySellNumByActivityIDList(new List<int> { activityID });
                if (list.Count == 1)
                {
                    return list.First();
                }
                else
                {
                    return new CouponActivitySellNumEntity { ActivityID = activityID, SellNum = 0 };
                }
            });
        }


        /// <summary>
        /// 移除券售卖数量缓存
        /// </summary>
        /// <param name="activityID"></param>
        public static void RemoveCouponSellNumCache(int activityID)
        {
            ActivityCouponSellNumCache.Remove(GenCouponSellNumCacheKey(activityID));
        }


        /// <summary>
        /// 获取一个券产品的销售数量
        /// </summary>
        /// <param name="activityID"></param>
        /// <returns></returns>
        public static int GetOneActivityCouponSellNum(int activityID)
        {
            return GetOneActivityCouponSellNumCache(activityID).SellNum;
        }




        #endregion


        #region ActivityCouponLockNum
        public static CouponLockNumResult GetCouponLockNumFromMemcache(int activityID)
        {
             return MemCacheHelper.memcached5min.GetData<CouponLockNumResult>(GenActivityLockNumCacheKey(activityID), () =>
            {
                var ret = GetActivityLockNum(activityID);//取最新的券活动购买数据
                //memcached5min.Set(keyName, ret);
                return ret;
            });
        }
        public static void SetCouponLockNum(int activityID, CouponLockNumResult result)
        {
            MemCacheHelper.memcached5min.Set(GenActivityLockNumCacheKey(activityID), result);
        }

        public static void RemoveCouponLockNumCache(int activityID)
        {
            MemCacheHelper.memcached5min.Remove(GenActivityLockNumCacheKey(activityID));
        }     

        private static CouponLockNumResult GetActivityLockNum(int activityID)
        {
            var result = new CouponLockNumResult();

            result.ActivityLockNum = couponSvc.GetActivityLockedCount(activityID);
            return result;
        }

        /// <summary>
        /// 根据最新的已卖出(加索定的）券数量计算券状态 
        /// </summary>
        /// <param name="activityID"></param>
        /// <param name="result"></param>
        public static void SetBuyCouponCheckNumActivityState(int activityID, BuyCouponCheckNumResult result)
        {
            result.ActivityLockNum = GetCouponLockNumFromMemcache(activityID).ActivityLockNum;
            int canSellNum = result.TotalNum - result.ActivityLockNum;//后台设置的初始卖出数量 开始限制已卖出量 - cae.ManuSellNum
            result.ActivityNoLockNum = canSellNum > 0 ? canSellNum : 0;

            if (canSellNum <= 0)
            {
                result.ActivityState = 0;//已售完
            }
        }

        #endregion


        #region CouponSellInfo

        //主动更新Memcached缓存
        public static void SetCouponSellInfoMem(int activityID, BuyCouponCheckNumResult bccr)
        {
            MemCacheHelper.memcached5min.Set(GenCurrentActivityNumMemKey(activityID), bccr);
        }
        public static void RemoveCouponSellInfoMem(int activityID)
        {
            MemCacheHelper.memcached5min.Remove(GenCurrentActivityNumMemKey(activityID));
        }


        public static BuyCouponCheckNumResult CouponSellInfoMem(int activityID)
        {
            return MemCacheHelper.memcached5min.GetData<BuyCouponCheckNumResult>(GenCurrentActivityNumMemKey(activityID), () =>
            {
                couponSvc.UpdateCouponState4TimeOut(activityID);//主动更新 超时未支付的订单为已取消
                return GenCouponSellInfo(activityID);//取最新的券活动购买数据
            });
        }




        /// <summary>
        /// 是否存在缓存，如果缓存不存在也就不需要后续的更新
        /// </summary>
        /// <param name="activityID"></param>
        /// <returns></returns>
        public static bool HasCouponSellInfoMem(int activityID)
        {
            return MemCacheHelper.memcached5min.Contains(GenCurrentActivityNumMemKey(activityID));
        }


        public static BuyCouponCheckNumResult GenCouponSellInfo(int activityID)
        {
            BuyCouponCheckNumResult result = new BuyCouponCheckNumResult();

            CouponActivityEntity cae = GetOneCouponActivity(activityID, true);

            result.SellNum = GetOneActivityCouponSellNum(activityID);//不包括锁定的数量 算上手动设置的起始数量
            result.TotalNum = cae.TotalNum;
            result.EffectiveTime = cae.EffectiveTime;
            result.ManuSellNum = cae.ManuSellNum;
            result.Price = cae.Price;
            result.SourceID = cae.SourceID;
            result.MaxBuyNum = cae.SingleBuyNum;
            result.MinBuyNum = cae.MinBuyNum;
            result.SaleEndDate = cae.SaleEndDate;

            if (cae.ExpireTime <= DateTime.Now || cae.State == 0)
            {
                result.ActivityState = 2;//活动关闭 结束
            }
            else
            {
                if (cae.EffectiveTime > DateTime.Now)
                {
                    result.ActivityState = 3;//活动未开始
                }
                else
                {
                    result.ActivityState = 1;//抢购中
                }
            }

            SetBuyCouponCheckNumActivityState(activityID, result);

            return result;
        }
      


        #endregion

        #region CouponActivitySKUSPURel

        /// <summary>
        /// 通过 activityID 获取与其关联的SKUID信息
        /// </summary>
        /// <param name="activityID"></param>
        /// <returns></returns>
        public static CouponActivitySKURelEntity GetCouponActivitySKUSPURelCache(int activityID)
        {
            return StepGroupCache.GetData<CouponActivitySKURelEntity>(GenCouponActivitySKUSPURelCacheKey(activityID), () =>
            {
                var list = couponSvc.GetCouponActivitySKURelByActivityID(activityID);
               if (list.Count > 0)
                {
                    return  list.First() ; 
                }
                else
                {
                    return new CouponActivitySKURelEntity {CouponActivityID = activityID, SKUID = -1 ,IDX = 0}; 
                }
            });
        }

        #endregion


        #region CouponActivity
        public static CouponActivityEntity GetOneCouponActivity(int id)
        {
            return couponSvc.GetOneCouponActivity(id, true);
        }

        public static CouponActivityEntity GetOneCouponActivity(int id, bool isLock)
        {
            var entity = couponSvc.GetOneCouponActivity(id, isLock);
            entity.PicList = new List<string>();
            if (entity.PicPath != null)
            {
                entity.PicPath.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList().ForEach(surl =>
                {
                    entity.PicList.Add(PhotoAdapter.GenHotelPicUrl(surl, Enums.AppPhotoSize.appdetail1));
                });
            }

            return entity;
        }

        #endregion


        #region 大团购
         

        public static  StepGroupEntity GetStepGroupCahceWithSKUID(int SKUID)
        {
            return StepGroupCache.GetData<StepGroupEntity>(GenStepGroupWithSKUIDCahceKey(SKUID), () =>
            {
                var skuItem = ProductAdapter.GetSKUItemByID(SKUID);
                if(skuItem.SPUID > 0)
                {
                    return ProductAdapter.GetStepGroupBySPUID(skuItem.SPUID);
                }
                else
                {
                    return new StepGroupEntity();
                }               
            });
        }

        public static void RemoveStepGroupCahceWithSKUID(int SKUID)
        {
            if (SKUID > 0)
            {
                StepGroupCache.Remove(GenStepGroupWithSKUIDCahceKey(SKUID));

                //移除购买SKU的用户订单数据缓存
                List<ExchangeCouponIDEntity> list = CouponAdapter.GetExchangeCouponIDListBySKUID(SKUID);
                RemoveUserDetailOrderCacheWithOrderIDList(list.Select(_ => (long)_.ID).ToList());
            }
        }


        #endregion


    }
}
