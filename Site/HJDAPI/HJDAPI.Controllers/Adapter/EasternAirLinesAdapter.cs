using HJD.CouponService.Contracts;
using HJD.Framework.WCF;
using HJD.HotelManagementCenter.Domain;
using HJD.HotelManagementCenter.IServices;
using HJDAPI.Controllers.Biz;
using ProductService.Contracts;
using ProductService.Contracts.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.Controllers.Adapter
{
    public class EasternAirLinesAdapter
    {
        private static IProductService productSvc = ServiceProxyFactory.Create<IProductService>("basicHttpBinding_IProductService");
        private static ICouponService couponSvc = ServiceProxyFactory.Create<ICouponService>("ICouponService");
        public static IOrderService orderSvc = ServiceProxyFactory.Create<IOrderService>("IOrderService");

        internal static bool PromotionEAPointsAdd(PromotionOrderInfo order, PromotionRuleEntity rule)
        {
            bool result = true;
            try
            {
                if (rule != null)
                {
                    if (order.UserID > 0)
                    {
                        string cardID = string.Empty;
                        EasternAirLinesCardEntity cardmodel = orderSvc.GetEACardByUid(order.UserID);//获取卡号ID
                        if (cardmodel != null)
                        {
                            cardID = cardmodel.ID.ToString();
                        }
                        else
                        {
                            cardID = "0";
                        }
                        EasternAirLinesRecordEntity recordmodel = new EasternAirLinesRecordEntity();
                        recordmodel.AccountNOID = cardID;
                        recordmodel.Points = double.Parse(rule.Discount.ToString());//赠送里程
                        recordmodel.ConsumeDate = DateTime.Now;
                        recordmodel.CreateTime = DateTime.Now;
                        recordmodel.DealState = 1;
                        recordmodel.Description = "";
                        recordmodel.Remark = "";
                        recordmodel.SourceType = 4;//7.8-8.31和心庭单人日料自助餐 买券送航空里程活动
                        recordmodel.States = 1;
                        recordmodel.UserID = order.UserID;
                        recordmodel.BusinessID = order.orderid;
                        orderSvc.AddEasternAirLinesRecord(recordmodel);
                    }
                }
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }

        internal static bool PromotionCouponEAPointsAdd(PromotionOrderInfo order, PromotionRuleEntity rule)
        {
            bool result = true;
            try
            {
                if (order.UserID > 0)
                {
                    string cardID = string.Empty;
                    EasternAirLinesCardEntity cardmodel = orderSvc.GetEACardByUid(order.UserID);//获取卡号ID
                    if (cardmodel != null)
                    {
                        cardID = cardmodel.ID.ToString();
                    }
                    else
                    {
                        cardID = "0";
                    }

                    decimal price = order.PayPrice;

                    if (price > 0)
                    {
                        EasternAirLinesRecordEntity recordmodel = new EasternAirLinesRecordEntity();
                        recordmodel.AccountNOID = cardID;
                        recordmodel.Points = (int)price;
                        recordmodel.ConsumeDate = DateTime.Now;
                        recordmodel.CreateTime = DateTime.Now;
                        recordmodel.DealState = 1;
                        recordmodel.Description = "";
                        recordmodel.Remark = "";
                        recordmodel.SourceType = 4;//7.8-8.31和心庭单人日料自助餐 买券核销后送航空里程活动
                        recordmodel.States = 1;
                        recordmodel.UserID = order.UserID;
                        recordmodel.BusinessID = order.orderid;
                        orderSvc.AddEasternAirLinesRecord(recordmodel);
                    }
                }
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }
    }
}
