using AutoMapper;
using HJD.Framework.Interface;
using HJD.Framework.WCF;
using HJD.HotelManagementCenter.Domain;
using HJD.HotelPrice.Contract.DataContract.Order;
using HJDAPI.Controllers.Adapter;
using HJDAPI.Controllers.Cache;
using HJDAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.Controllers.Common
{
    public  class OrderHelper
    {

  
        #region  Order Redis  相关方法
        /// <summary>
        /// 缓存用户对应的OrderID 的Key,{0}用UserId替换,{1}用订单列表类型替换, UserOrderID_1：4511973
        /// </summary>
        static string UserOrderID = "UserOrder:UserID_{0}:OrderType_{1}";

        /// <summary>
        /// 订单与订单明细关联有关系的K， ｛0｝为全局订单ID
        /// </summary>
        static string OrderIDDetailID = "OrderIDDetalID:OrderID_{0}";

        /// <summary>
        /// 用户数据是否缓存过
        /// </summary>
        static string UserOrderInit = "UserOrderInit:UserID_{0}";         

        /// <summary>
        /// 缓存订单的Key,{0}用DetailOrderId替换  ExchangeCoupon.ID or  Orders.OrderID
        /// </summary>
         static string prefixDetailOrderCacheKey = "userDetailOrder";
         
         /// <summary>
         /// key过期时间，单位分钟
         /// </summary>
         static double KeyExpireTime = 30;

        public static Dictionary<EnumHelper.OrderType, string > dicOrderTypeIcon = 
            new Dictionary<EnumHelper.OrderType,string>{
                   {EnumHelper.OrderType.FoodCouponOrder,"http://whfront.b0.upaiyun.com/app/img/order/order-logo-food.png"}, 
                   {EnumHelper.OrderType.LiuWaCouponOrder,"http://whfront.b0.upaiyun.com/app/img/order/order-logo-kids.png"}, 
                   {EnumHelper.OrderType.PlayCouponOrder,"http://whfront.b0.upaiyun.com/app/img/order/order-logo-play.png"}, 
                   {EnumHelper.OrderType.RoomCouponOrder,"http://whfront.b0.upaiyun.com/app/img/order/order-logo-room.png"}, 
                   {EnumHelper.OrderType.HotelOrder,"http://whfront.b0.upaiyun.com/app/img/order/order-logo-hotel.png"}, 
                   {EnumHelper.OrderType.JiJiuHotelOrder,"http://whfront.b0.upaiyun.com/app/img/order/order-logo-airtrip.png"}, 
                   {EnumHelper.OrderType.YouLunHotelOrder,"http://whfront.b0.upaiyun.com/app/img/order/order-logo-liner.png"}
            };

        //public static string LiuWaIcon = "http://whfront.b0.upaiyun.com/app/img/order/order-logo-kids.png";
        //public static string HotelOrderIcon = "http://whfront.b0.upaiyun.com/app/img/order/order-logo-hotel.png";
        //public static string FoodCouponIcon = "http://whfront.b0.upaiyun.com/app/img/order/order-logo-food.png";
        //public static string playCouponIcon = "http://whfront.b0.upaiyun.com/app/img/order/order-logo-play.png";
        //public static string RoomCouponIcon = "http://whfront.b0.upaiyun.com/app/img/order/order-logo-room.png";
        //public static string JiJiuOrderIcon = "http://whfront.b0.upaiyun.com/app/img/order/order-logo-airtrip.png";
        //public static string YouLunOrderIcon = "http://whfront.b0.upaiyun.com/app/img/order/order-logo-liner.png";

        #region 生成Key的规则
        private static string GenUserOrderIDKey(long userId, EnumHelper.OrderType ptype)
        {
            return string.Format(OrderHelper.UserOrderID, userId, (int)ptype);
        }
        private static string GenUserOrderIDKey_all(long userId)
        {
            return string.Format(OrderHelper.UserOrderID, userId, (int)EnumHelper.OrderType.AllOrder);
        }

        private static List<string> GenAllUserOrderIDKey(long userId)
        {
            List<string> allKeyList = new List<string>();
            foreach (EnumHelper.OrderType item in Enum.GetValues(typeof(EnumHelper.OrderType)))
            {
                allKeyList.Add(GenUserOrderIDKey(userId, item));
            }
            return allKeyList;
        }
        

        private static string GenUserOrderInitKey(long userID)
        {
            return string.Format(OrderHelper.UserOrderInit, userID);
        }
        
        private static List<string> GenOrderIDKeyList(List<string> OrderIDs)
        {
            return OrderIDs.Select(curCouponOrderID => string.Format(OrderHelper.OrderIDDetailID, curCouponOrderID)).ToList();
        }


        #endregion


        /// <summary>
        /// 把订单放入Redis缓存 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="orderType">订单类型</param>
        /// <param name="allOrderList"></param>
        /// <returns></returns>
        private static bool AddOrderToRedis(long userId, EnumHelper.OrderType orderType, List<OrderListItemEntity> allOrderList) //todo 打包产品
        {
            bool isSuccess = false;
            try
            {

                var curPTypeOrderList = allOrderList.Where(_ => _.OrderType == (int)orderType && _.IsPromotion == false);

                //用户订单缓存Key
                string userOrderIDKey = GenUserOrderIDKey(userId, orderType); 

                //用户全部订单缓存Key
                string userOrderIDKey_all = GenUserOrderIDKey_all( userId );


                foreach (var curCouponOrderID in curPTypeOrderList.Select(_ => _.CouponOrderId).Distinct())
                {
                    if (curCouponOrderID == 0) continue;
                    var curDetailOrderList = allOrderList.Where(_ => _.CouponOrderId == curCouponOrderID).OrderBy(_=>_.OrderId);
                    var firstItem =curDetailOrderList.First();

                    //添加用户订单缓存
                    RedisHelper.RedisConn.SortedSetAdd(userOrderIDKey, curCouponOrderID.ToString(), CommMethods.GetTimestamp(firstItem.SubmitOrderDate));
                    //添加用户全部订单ID缓存
                    RedisHelper.RedisConn.SortedSetAdd(userOrderIDKey_all, curCouponOrderID.ToString(), CommMethods.GetTimestamp(firstItem.SubmitOrderDate));
                       
                    string OrderIDDetailIDKey = string.Format(OrderHelper.OrderIDDetailID, curCouponOrderID);


                    //添加订单ID与明细实体关联
                    OrderIDDetailOrderIDRelEntity ddr = new OrderIDDetailOrderIDRelEntity
                    {
                        OrderID = curCouponOrderID,
                        DetailOrderIDList = string.Join(",", curDetailOrderList.Select(_ => _.OrderId).ToList())
                    };
                    RedisHelper.RedisConn.HashSetObject(OrderIDDetailIDKey, ddr);
                                    
                    //foreach (var item in curDetailOrderList)
                    //{   //添加订单实体缓存
                    //    UserDetailOrderMemCache.Set(item.OrderId.ToString(), MemCacheHelper.Prefix.UserDetailOrderCacheKey, item);
                     
                    //  //  RedisHelper.RedisConn.HashSetObject<OrderListItemEntity>(GenDetailOrderKey(item.OrderId), item);    
                    //}
                }
                isSuccess = true;
            }
            catch (Exception ex)
            {
                Log.WriteLog(string.Format("订单放入Redi缓存失败：UserId={0},OrderType={1},错误信息：{2}", userId, orderType, ex.Message + ex.StackTrace));
            }

            allOrderList.ForEach(order => ProductCache.RemoveUserDetailOrderCache(order.OrderId));
            return isSuccess;
        }
             
        /// <summary>
        /// 清除用户订单Redis数据已初始化标识，以便重新初始化数据
        /// </summary>
        /// <param name="userIdList"></param>        
        public static  void RemoveUserOrderRedisData(List<long> userIdList)
        {
            RedisHelper.RedisConn.KeyDelete(userIdList.Select(userId => GenUserOrderInitKey(userId)).ToList());
        }


        /// <summary>
        /// 初始化用户订单Redis数据
        /// </summary>
        /// <param name="userIdList">要初始化的数据的用户ID列表</param>
        private static  void InitUserOrderRedisData(List<long> userIdList)
        {
            userIdList.ForEach(userId =>
            {
                if (userId > 0)
                {
                    if (RedisHelper.RedisConn.StringGet(GenUserOrderInitKey(userId)) == null)
                    {
                        Log.WriteLog("InitUserOrderRedisData UserID:" + userId.ToString());
                        RedisHelper.RedisConn.KeyDelete(GenAllUserOrderIDKey(userId));

                        // 酒店
                        var hotelOrderList = OrderAdapter.GetAllHotelOrderList(userId);

                        EnumHelper.PackageOrderTypeList.ForEach(pType => OrderHelper.AddOrderToRedis(userId, pType, hotelOrderList));

                        //券订单
                        var couponOrderList = CouponAdapter.GetAllCouponOrderList(userId);
                        EnumHelper.CouponOrderTypeList.ForEach(pType => OrderHelper.AddOrderToRedis(userId, pType, couponOrderList));

                        RedisHelper.RedisConn.StringSet(GenUserOrderInitKey(userId), userId.ToString());
                    }
                }
            });
        } 
         




        /// <summary>
        /// 从Redi缓存获取订单
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="ptype"></param>
        /// <param name="start"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static List<OrderListItemEntity> GetOrderListFromRedis(long userId, EnumHelper.OrderType ptype, int start, int count)
        {
            StringBuilder sb = new StringBuilder();

            List<OrderListItemEntity> orderList = new List<OrderListItemEntity>();
            if (userId > 0)
            {
                try
                {
                    string userWHOrderIDKey = GenUserOrderIDKey(userId, ptype);
                    InitUserOrderRedisData(new List<long> { userId });

                    sb.AppendLine(userWHOrderIDKey);

                    //获取用户的OrderID
                    List<string> userWHOrderIds = RedisHelper.RedisConn.SortedSetRangeByRank<string>(userWHOrderIDKey, start, (start + count - 1));
                    sb.AppendLine("userWHOrderIds");
                    sb.AppendLine(string.Join(",",userWHOrderIds));
                    if (userWHOrderIds.Count > 0)
                    {
                        //获取订单关联子订单ID
                        List<OrderIDDetailOrderIDRelEntity> ddrList = RedisHelper.RedisConn.HashGetAllToList<OrderIDDetailOrderIDRelEntity>(GenOrderIDKeyList(userWHOrderIds));
                        //全局订单ID列表
                        List<long> WHOrderIDList = ddrList.Select(_ => _.OrderID).ToList();
                        sb.AppendLine("WHOrderIDList");
                        sb.AppendLine(string.Join(",", WHOrderIDList));
                        //子订单ID列表
                        List<long> detailOrderIDList = string.Join(",", ddrList.Select(_ => _.DetailOrderIDList)).Split(",".ToCharArray()).Select(strID => long.Parse(strID)).ToList();
                        sb.AppendLine("detailOrderIDList");
                        sb.AppendLine(string.Join(",", detailOrderIDList));

                        //根据userOrderIds,从缓存中获取订单列表
                        List<OrderListItemEntity> detailOrderList = ProductCache.GetOrderListItemEntityFromMemCache(detailOrderIDList);
                        sb.AppendLine("detailOrderList:" + detailOrderList.Count());
                        sb.AppendLine(string.Join(",", detailOrderIDList));

                        WHOrderIDList.ForEach(WHOrderID =>
                        {
                            var curOrderList = detailOrderList.Where(_ => _.CouponOrderId == WHOrderID && _.OrderId > 0);
                            sb.AppendLine(string.Format("{0} {1}",WHOrderID , curOrderList.Count()));

                            if (curOrderList.Count() > 0)
                            {
                                var firstDetailOrder = curOrderList.First();
                                //如果是大团购的定金产品，需要增加大团购数据                                      
                                if (firstDetailOrder.SubSkuType == (int)HJDAPI.Common.Helpers.Enums.SubSKUType.BigGroup_Deposit)
                                {
                                    firstDetailOrder.StepGroup = ProductCache.GetStepGroupCahceWithSKUID(firstDetailOrder.SKUID);
                                }

                                if (curOrderList.Count() > 1)
                                {

                                    List<OrderListItemEntity> subDetailList = new List<OrderListItemEntity>();
                                    //生成订单详情 
                                    var productType = (OrderHelper.ProductType)firstDetailOrder.ProductType;
                                    var firstOrderID = firstDetailOrder.OrderId;

                                    if (productType == OrderHelper.ProductType.Package)
                                    {
                                        //壳产品排在上面
                                        subDetailList = curOrderList.OrderByDescending(p => p.IsPackage).ThenBy(p => p.OrderState).ToList();
                                    }
                                    else if(productType == OrderHelper.ProductType.BigGroup) 
                                    {
                                        subDetailList = curOrderList.Where(_=>_.OrderState != 1 && _.OrderState != 4).OrderBy(p => p.SubmitOrderDate).ToList();  //大团购尾款订单末支付时不显示                                  
                                    }
                                    else
                                    {
                                        subDetailList = curOrderList.OrderBy(p => p.OrderState).ThenBy(p=> p.OrderId).ToList();
                                    }                                                                    

                                    firstDetailOrder.DetailOrderList = subDetailList.Select(d => Mapper.Map<OrderListItemEntity, DetailOrderListEntity>(d)).ToList();
                                    firstDetailOrder.Count = firstDetailOrder.DetailOrderList.Count;
                                    firstDetailOrder.TotalAmount =  firstDetailOrder.DetailOrderList.Where(_=>_.IsPromotion == false).Sum(_=>_.TotalAmount);
                                    firstDetailOrder.TotalPoints = firstDetailOrder.DetailOrderList.Where(_ => _.IsPromotion == false).Sum(_ => _.TotalPoints);
                                }

                                orderList.Add(firstDetailOrder);
                            }
                        });
                    }
                }
                catch (Exception ex)
                {
                    Log.WriteLog(string.Format("从Redi缓存获取订单失败：UserId={0},OrderType={1},错误信息：{2}", userId, ptype, ex.Message + ex.StackTrace));
                }
            }

            Log.WriteLog("GetOrderListFromRedis:" +sb.ToString());
            return orderList;
        }


        /// <summary>
        /// 批量更新Redis用户订单缴存 
        /// </summary>
        /// <param name="orderIdList">订单ID列表 </param>
        /// <returns></returns>
         public static bool AddOrderToRedisWithIDList(List<long> orderIdList )
        {
            orderIdList.Distinct().ToList().ForEach(orderID => AddOrderToRedis(orderID));
            return true;
        }
       

        /// <summary>
        /// 新增订单Redis缓存
        /// </summary>
        /// <param name="orderId"> orders.OrderID, ExchangeCoupon.CouponOrderID  </param> 
        /// <returns></returns>
        public static bool AddOrderToRedis(long orderId )
        {
            try
            {
                //酒店订单
                if (OrderHelper.IsCommOrder(orderId) == false)
                { 
                    OrderListItem item = OrderAdapter.GetUserOrderInfoByOrderID(orderId); 

                    List<OrderListItemEntity> orderList = OrderAdapter.TransOrderListItem2OrderListItemEntity( new List<OrderListItem>{item});
                    return OrderHelper.AddOrderToRedis(item.UserID, (EnumHelper.OrderType)item.PackageType, orderList);
                }
                else
                {
                    var couponOrderList = CouponAdapter.GetExchangCouponListByCouponOrderId(orderId);
                    if (couponOrderList.Count > 0)
                    {
                        var couponEntity = couponOrderList.First();
                        EnumHelper.OrderType orderType = (EnumHelper.OrderType)couponEntity.ParentID;
                        List<OrderListItemEntity> orderList = CouponAdapter.TransExchangeCouponEntityItem2OrderListItemEntity(couponOrderList);
                        return OrderHelper.AddOrderToRedis(couponEntity.UserID, orderType, orderList);
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.WriteLog("新增订单Redis缓存失败， OrderId="+orderId+",错误信息："+ ex.Message + ex.StackTrace);
                return false;
            }
        }
        /// <summary>
        /// 移除订单添加到缓存 只有酒店订单会有删除
        /// </summary>
        /// <param name="param">param.OrderId： 酒店订单的OrderID </param>
        /// <returns></returns>
        public static bool RemoveOrderFromRedis(long detailOrderID)
        {
            try
            {
                 OrderListItem item = OrderAdapter.GetUserOrderInfoByOrderID(detailOrderID);

                 RedisHelper.RedisConn.SortedSetRemove(GenUserOrderIDKey(item.UserID, (EnumHelper.OrderType)item.PackageType), detailOrderID.ToString());
                 RedisHelper.RedisConn.SortedSetRemove(GenUserOrderIDKey_all(item.UserID), detailOrderID.ToString());

                return true;
            }
            catch (Exception ex)
            {
                Log.WriteLog("移除订单缓存失败，OrderId=" + detailOrderID.ToString() + "错误信息：" + ex.Message + ex.StackTrace);
                return false;
            }
        }


        #endregion


        /// <summary>
        /// 判断是酒店订单还是券订单，OrderID大于100000000属于酒店订单
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns>券订单返回 true ,酒店订单返回 false </returns>
        public static bool IsCommOrder(long orderId)
        {
            return orderId < 100000000;
        }
        /// <summary>
        /// 获取订单状态名称
        /// </summary>
        /// <param name="orderState"></param>
        /// <returns></returns>
        public static string GetOrderStateName(int orderState)
        {
            switch (orderState)
            {
                case 1:
                case 2:
                    return "已提交未支付";
                case 3:
                    return "已提交待支付";
                case 4:
                case 5:
                    return "已取消";
                case 10:
                    return "已支付待确认";
                case 12: 
                    return "已确认";
                case 31:
                case 32:
                    return "已修改待确认";
                case 33:
                    return "待线下支付";
                case 34:
                    return "待取消";
                case 36:
                    return "已补汇款";
                default:
                    return "其它";
            }
        }
        /// <summary>
        /// 获取券订单状态名称
        /// </summary>
        /// <param name="couponState"></param>
        /// <returns></returns>
        public static string GetCouponStateName(int couponState)
        {
            switch (couponState)
            {
                case 1:
                    return "已提交";
                case 2:
                    return "已支付";
                case 3:
                    return "已消费";
                case 4:
                    return "已取消";
                case 5:
                    return "已退款";
                case 6:
                    return "超时支付";
                case 8:
                    return "已过期";
                default:
                    return "其它";
            }
        }
        public static string GetIcon(int type)
        {
            if ( dicOrderTypeIcon.ContainsKey((EnumHelper.OrderType)type) == false )
            {
                dicOrderTypeIcon.Add((EnumHelper.OrderType)type,"");
            }
            return dicOrderTypeIcon[(EnumHelper.OrderType)type];
            //string strReturn = "";
            //switch ((EnumHelper.OrderType)type)
            //{
            //    case EnumHelper.OrderType.FoodCouponOrder:
            //        strReturn = OrderHelper.FoodCouponIcon;
            //        break;
            //    case EnumHelper.OrderType.LiuWaCouponOrder:
            //        strReturn = OrderHelper.LiuWaIcon;
            //        break;
            //    case EnumHelper.OrderType.PlayCouponOrder:
            //        strReturn = OrderHelper.playCouponIcon;
            //        break;
            //    case EnumHelper.OrderType.RoomCouponOrder:
            //        strReturn = OrderHelper.RoomCouponIcon;
            //        break;
            //    case EnumHelper.OrderType.HotelOrder:
            //        strReturn = OrderHelper.HotelOrderIcon;
            //        break;
            //    case EnumHelper.OrderType.JiJiuHotelOrder:
            //        strReturn = OrderHelper.JiJiuOrderIcon;
            //        break;
            //    case EnumHelper.OrderType.YouLunHotelOrder:
            //        strReturn = OrderHelper.YouLunOrderIcon;
            //        break;
            //}
            //return strReturn;
        }
        /// <summary>
        /// 券产品类型
        /// </summary>
        public enum ProductType
        {
            /// <summary>
            /// 普通产品
            /// </summary>
            Normal = 0,
            /// <summary>
            /// 大团购产品
            /// </summary>
            BigGroup = 1,
            /// <summary>
            /// 打包产品
            /// </summary>
            Package = 2
        }

        /// <summary>
        ///  子产品 类型
        /// </summary>
        public enum SubProductType
        {
            /// <summary>
            /// 普通产品
            /// </summary>
            Normal = 0,
            /// <summary>
            /// 大团购产品。订金
            /// </summary>
            BigGroupSubscription = 1 
        }

        /// <summary>
        /// 取消用户与券的关联关系
        /// </summary>
        /// <param name="p"></param>
        /// <param name="deletedOrderIDList"></param>
        internal static void DeleteUserCouponOrderRelRedisData(long userID, List<long> deletedOrderIDList)
        {
            List<string> strDeletedOrderIDList = deletedOrderIDList.Select(_ => _.ToString()).ToList();

            EnumHelper.CouponOrderTypeList.ForEach(pType =>
            {
                RedisHelper.RedisConn.SortedSetRemove(GenUserOrderIDKey(userID, pType), strDeletedOrderIDList);
                RedisHelper.RedisConn.SortedSetRemove(GenUserOrderIDKey_all(userID), strDeletedOrderIDList);
            });
        }

        /// <summary>
        /// 取消用户与酒店订单的关联关系
        /// </summary>
        /// <param name="p"></param>
        /// <param name="deletedOrderIDList"></param>
        internal static void DeleteUserPackageOrderRelRedisData(long userID, List<long> deletedOrderIDList)
        {
            List<string> strDeletedOrderIDList = deletedOrderIDList.Select(_ => _.ToString()).ToList();

            EnumHelper.PackageOrderTypeList.ForEach(pType =>
            {
                RedisHelper.RedisConn.SortedSetRemove(GenUserOrderIDKey(userID, pType), strDeletedOrderIDList);
                RedisHelper.RedisConn.SortedSetRemove(GenUserOrderIDKey_all(userID), strDeletedOrderIDList);
            });
        }

    }
}
