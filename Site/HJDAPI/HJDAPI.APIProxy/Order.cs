using HJD.HotelManagementCenter.Domain;
using HJD.HotelPrice.Contract.DataContract.Order;
using HJDAPI.Common.Helpers;
using HJDAPI.Controllers;
using HJDAPI.Controllers.Adapter;
using HJDAPI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.APIProxy
{
    public class Order : BaseProxy
    {

        public static int AddCommOrders(HJD.HotelPrice.Contract.DataContract.Order.CommOrderEntity commorders)
        {
            if (IsProductEvn)
            {
                return OrderAdapter.AddCommOrders(commorders);
            }
            else
            {
                string url = APISiteUrl + "api/order/AddCommOrders";

                CookieContainer cc = new CookieContainer();

                string json = HttpRequestHelper.PostJson(url, commorders, ref cc);

                return JsonConvert.DeserializeObject<int>(json);
            }
        }

        public HJD.HotelPrice.Contract.DataContract.Order.CommOrderEntity GetCommOrder(int OrderID)
        {
            if (IsProductEvn)
            {
                return OrderAdapter.GetCommOrder(OrderID);
            }
            else
            {
                string url = APISiteUrl + "api/order/GetCommOrder";

                CookieContainer cc = new CookieContainer();

                string json = HttpRequestHelper.PostJson(url, OrderID, ref cc);

                return JsonConvert.DeserializeObject<HJD.HotelPrice.Contract.DataContract.Order.CommOrderEntity>(json);
            }
        }


        public static bool CheckAlipaySecurity(AlipayCheckParams acp)
        {
            if (IsProductEvn)
            {
                return OrderAdapter.CheckAlipaySecurity(acp);
            }
            else
            {
                string url = APISiteUrl + "api/order/CheckAlipaySecurity";

                CookieContainer cc = new CookieContainer();

                string json = HttpRequestHelper.PostJson(url, acp, ref cc);

                return true;// JsonConvert.DeserializeObject<bool>(json);
            }
        }

        public static string GetAlipayOrderInfo(long orderid)
        {
            string url = "http://alipay.zmjiudian.com/GetOrderInfo.aspx";
            string pars = "OrderID=" + orderid.ToString();

            CookieContainer cc = new CookieContainer();

            string info = HttpRequestHelper.Get(url, pars, ref cc);

            return info;
        }

        public static OrderSubmitResult SubmitOrder(OrderEntity order)
        {
            if (IsProductEvn)
            {
                return OrderAdapter.SubmitOrder(order);
            }
            else
            {
                string url = APISiteUrl + "api/order/SubmitOrder";

                CookieContainer cc = new CookieContainer();

                string json = HttpRequestHelper.PostJson(url, order, ref cc);

                return JsonConvert.DeserializeObject<OrderSubmitResult>(json);
            }
        }

        public CheckSubmitOrderResonse CheckSubmitOrderBefore(int pid, long userid)
        {
            string url = APISiteUrl + "api/order/CheckSubmitOrderBefore";
            string postDataStr = string.Format("pid={0}&userid={1}", pid, userid);
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<CheckSubmitOrderResonse>(json);
        }

        public static OrderSubmitResult SubmitOrderV42(OrderEntity order)
        {
            if (IsProductEvn)
            {
                return OrderAdapter.SubmitOrder(order, "4.2");
            }
            else
            {
                string url = APISiteUrl + "api/order/SubmitOrderV42";

                CookieContainer cc = new CookieContainer();

                string json = HttpRequestHelper.PostJson(url, order, ref cc);

                return JsonConvert.DeserializeObject<OrderSubmitResult>(json);
            }
        }

        public static PackageOrderInfo GetPackageOrderInfo(long orderid, long userid)
        {
            if (IsProductEvn)
            {
                return OrderAdapter.GetPackageOrderInfo(orderid, userid);
            }
            else
            {

                string url = APISiteUrl + "api/order/GetPackageOrderInfo";
                string postDataStr = string.Format("orderid={0}&userid={1}"
                  , orderid, userid);

                CookieContainer cc = new CookieContainer();

                string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
                return JsonConvert.DeserializeObject<PackageOrderInfo>(json);
            }
        }

        public static PackageOrderInfo20 GetPackageOrderInfo20(long orderid, long userid)
        {
            if (IsProductEvn)
            {
                return OrderAdapter.GetPackageOrderInfo20(orderid, userid);
            }
            else
            {

                string url = APISiteUrl + "api/order/GetPackageOrderInfo20";
                string postDataStr = string.Format("orderid={0}&userid={1}"
                  , orderid, userid);

                CookieContainer cc = new CookieContainer();

                string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
                return JsonConvert.DeserializeObject<PackageOrderInfo20>(json);
            }
        }

        public static List<OrderListItem> GetUserOrderList(long userid, int start, int count)
        {
            if (IsProductEvn)
            {
                return OrderAdapter.GetUserOrderList(userid, start, count);
            }
            else
            {
                string url = APISiteUrl + "api/order/GetUserOrderList";
                string postDataStr = string.Format("userid={0}&start={1}&count={2}"
                  , userid, start, count);

                CookieContainer cc = new CookieContainer();

                string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
                return JsonConvert.DeserializeObject<List<OrderListItem>>(json);
            }
        }

        public static List<MenuItemEntity> GetOrderCountDisplayEntity(long userId)
        {
            string url = APISiteUrl + "api/order/GetOrderCountDisplayEntity";
            string postDataStr = string.Format("userid={0}", userId);

            CookieContainer cc = new CookieContainer();

            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<List<MenuItemEntity>>(json);
        }

        public static bool CancelAuthOrder(long orderid, long userid)
        {
            OrderCancelResult result = null;
            if (IsProductEvn)
            {
                result = OrderAdapter.CancelAuthOrder(orderid, userid);
            }
            else
            {
                string url = APISiteUrl + "api/order/CancelAuthOrder";

                CookieContainer cc = new CookieContainer();
                //string postDataStr = string.Format("orderid={0}&userid={1}", orderid, userid);
                CancelAuthOrderParams cancelParam = new CancelAuthOrderParams();
                cancelParam.orderid = orderid;
                cancelParam.userid = userid;
                string json = HttpRequestHelper.PostJson(url, cancelParam, ref cc);

                result = JsonConvert.DeserializeObject<OrderCancelResult>(json);
            }
            return result.success == 0;
        }

        public BaseResponse RequestOrderRebate(long orderId, int typeId)
        {
            string url = APISiteUrl + "api/order/RequestOrderRebate";
            CookieContainer cc = new CookieContainer();
            RequestOrderRebateRequestParams param = new RequestOrderRebateRequestParams() { orderID = orderId, type = typeId };
            string json = HttpRequestHelper.PostJson(url, param, ref cc);

            return JsonConvert.DeserializeObject<BaseResponse>(json);
        }

        public GetUserRebateListResponse GetUserRebateList(GetUserRebateListRequestParams p)
        {
            string url = APISiteUrl + "api/order/GetUserRebateList";
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.PostJson(url, p, ref cc);
            return JsonConvert.DeserializeObject<GetUserRebateListResponse>(json);
        }

        public CheckOrderBeforePayResponse CheckOrderBeforePay(CheckOrderBeforePayRequestParams p)
        {
            string url = APISiteUrl + "api/order/CheckOrderBeforePay";
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.PostJson(url, p, ref cc);
            return JsonConvert.DeserializeObject<CheckOrderBeforePayResponse>(json);
        }

        public WeixinPayReturnParam GetWeixinPayParam(WeixinPayRequestParam param)
        {
            string url = APISiteUrl + "api/order/GetWeixinPayParam";
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.PostJson(url, param, ref cc);
            return JsonConvert.DeserializeObject<WeixinPayReturnParam>(json);
        }

        /// <summary>
        /// 清除订单
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static OrderCancelResult DeleteOrder(CancelAuthOrderParams p)
        {
            OrderCancelResult result = null;
            if (IsProductEvn)
            {
                result = OrderAdapter.DeleteOrder(p);
            }
            else
            {
                string url = APISiteUrl + "api/order/DeleteOrder";
                CookieContainer cc = new CookieContainer();
                CancelAuthOrderParams cancelParam = p;
                string json = HttpRequestHelper.PostJson(url, cancelParam, ref cc);
                result = JsonConvert.DeserializeObject<OrderCancelResult>(json);
            }
            return result;
        }

        public static BoTaoOrderEntity GetBoTaoOrderInfo(long orderId)
        {
            BoTaoOrderEntity result = null;
            string url = APISiteUrl + "api/order/GetBoTaoOrderInfo";
            string postDataStr = string.Format("orderId={0}", orderId);
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<BoTaoOrderEntity>(json);
        }

        public static string GetOrderRelWHSaleInfo(int hotelid, int supplierid = 0)
        {
            BoTaoOrderEntity result = null;
            string url = APISiteUrl + "api/order/GetOrderRelWHSaleInfo";
            string postDataStr = string.Format("hotelid={0}&supplierid={1}", hotelid, supplierid);
            CookieContainer cc = new CookieContainer();
            return HttpRequestHelper.Get(url, postDataStr, ref cc);
        }

        public static string UpdateOrderRelWHSales(long oid, string RelWHSales)
        {
            BoTaoOrderEntity result = null;
            string url = APISiteUrl + "api/order/UpdateOrderRelWHSales";
            string postDataStr = string.Format("Oid={0}&RelWHSales={1}", oid, RelWHSales);
            CookieContainer cc = new CookieContainer();
            return HttpRequestHelper.Get(url, postDataStr, ref cc);
        }

        public static long BGSubimitOrder(OrderEntity order)
        {
            string url = APISiteUrl + "api/order/BGSubimitOrder";
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.PostJson(url, order, ref cc);
            return JsonConvert.DeserializeObject<long>(json);
        }
        public static bool UpdateOrderStateFromRedis(OrderCacheParam param)
        {
            string url = APISiteUrl + "api/order/UpdateOrderStateFromRedis";
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.PostJson(url, param, ref cc);
            return JsonConvert.DeserializeObject<bool>(json);
           
        }
        public static bool AddOrderToRedis(OrderCacheParam param)
        {
            string url = APISiteUrl + "api/order/AddOrderToRedis";
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.PostJson(url, param, ref cc);
            return JsonConvert.DeserializeObject<bool>(json);
        }
        /// <summary>
        /// 取消超时订单
        /// </summary>
        /// <returns></returns>
        public static bool CancelOverTimeOrder()
        {
            return OrderAdapter.CancelOverTimeOrder();
        }
    }
}