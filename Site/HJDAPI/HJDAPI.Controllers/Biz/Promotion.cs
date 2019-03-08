using HJD.AccountServices.Entity;
using HJDAPI.Controllers.Adapter;
using ProductService.Contracts.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.Controllers.Biz
{
    /// <summary>
    /// 优惠策略处理
    /// </summary>
    public class Promotion
    {
        public static bool DoPromotionForPaied(PromotionRuleEntity rule, PromotionOrderInfo order)
        {
            bool result = true;

            switch ((ProductServiceEnums.PromotionRuleType)rule.Type)
            {
                case ProductServiceEnums.PromotionRuleType.FreeSKU:
                    FreeSKU(rule, order);
                    break;
                //case ProductServiceEnums.PromotionRuleType.FreeCoupon:
                //    FreeCoupon(rule, order);
                //    break;
                case ProductServiceEnums.PromotionRuleType.UserCashCoupon:
                    UserCashCoupon(rule, order);
                    break; 
                case ProductServiceEnums.PromotionRuleType.FreeVIP199nr:
                    FreeVIP199nr(rule, order);
                    break;
                case ProductServiceEnums.PromotionRuleType.FreeVIP199nr_NoFistPackagePriviledge:
                    FreeVIP199nr_NoFistPackagePriviledge(rule, order);
                    break;
                case ProductServiceEnums.PromotionRuleType.FreeVIP599:
                    FreeVIP599(rule, order);
                    break;
                case ProductServiceEnums.PromotionRuleType.FreeVIP599_NoFistPackagePriviledge:
                    FreeVIP599_NoFistPackagePriviledge(rule, order);
                    break;
                case ProductServiceEnums.PromotionRuleType.EAPointsAfterPay:
                    EAPointsAdd(rule, order);
                    break;
            }

            return result;

        }

      

        /// <summary>
        /// 核销过后
        /// </summary>
        /// <param name="rule"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public static bool DoPromotionForChecked(PromotionRuleEntity rule, PromotionOrderInfo order)
        {
            bool result = true;

            switch ((ProductServiceEnums.PromotionRuleType)rule.Type)
            {
                case ProductServiceEnums.PromotionRuleType.EAPointsAfterCheck://7.8-8.31和心庭单人日料自助餐 买券送航空里程活动
                    PromotionCouponEAPointsAdd(rule, order);
                    break;
            }
            return result;

        }
        private static void FreeSKU(PromotionRuleEntity rule, PromotionOrderInfo order)
        {
            int skuID = rule.TargetID;

            int activityID = CouponAdapter.GetSKUCouponActivityDetail(skuID).activity.ID;

            CouponAdapter.PromotionPresent(order, rule.ID, activityID, skuID);
        }

        private static void FreeVIP599_NoFistPackagePriviledge(PromotionRuleEntity rule, PromotionOrderInfo order)
        {
            CouponAdapter.PromotionPresent(order, rule.ID, (int)HJDAPI.Controllers.EnumHelper.ActivityID.VIP599);
            bool CanBuyVIPFirstPackage = false;
            AccountAdapter.CreateVIPRole(order.UserID, order.PhoneNum, (int)HJDAPI.Controllers.EnumHelper.ActivityID.VIP599, order.CID, CanBuyVIPFirstPackage);
        }

        private static void FreeVIP599(PromotionRuleEntity rule, PromotionOrderInfo order)
        {
            CouponAdapter.PromotionPresent(order, rule.ID, (int)HJDAPI.Controllers.EnumHelper.ActivityID.VIP599);
            bool CanBuyVIPFirstPackage = true;
            AccountAdapter.CreateVIPRole(order.UserID, order.PhoneNum, (int)HJDAPI.Controllers.EnumHelper.ActivityID.VIP599, order.CID, CanBuyVIPFirstPackage);
        }

        private static void FreeVIP199nr_NoFistPackagePriviledge(PromotionRuleEntity rule, PromotionOrderInfo order)
        {
            CouponAdapter.PromotionPresent(order, rule.ID, (int)HJDAPI.Controllers.EnumHelper.ActivityID.VIP199_NR);
            bool CanBuyVIPFirstPackage = false;
            AccountAdapter.CreateVIPRole(order.UserID, order.PhoneNum, (int)HJDAPI.Controllers.EnumHelper.ActivityID.VIP199_NR, order.CID, CanBuyVIPFirstPackage);
            try
            {
                AccountAdapter.EditUserInfoExAdd(order.UserID, "SKU赠送VIP", "订单PayID ：" + order.PayID + "Promotion ：" + order.promotionID);
            }
            catch (Exception e)
            {

            }
        }

        private static void FreeVIP199nr(PromotionRuleEntity rule, PromotionOrderInfo order)
        {
            CouponAdapter.PromotionPresent(order, rule.ID, (int)HJDAPI.Controllers.EnumHelper.ActivityID.VIP199_NR);
            bool CanBuyVIPFirstPackage = true;
            AccountAdapter.CreateVIPRole(order.UserID, order.PhoneNum, (int)HJDAPI.Controllers.EnumHelper.ActivityID.VIP199_NR, order.CID, CanBuyVIPFirstPackage);
            try
            {
                AccountAdapter.EditUserInfoExAdd(order.UserID, "SKU赠送VIP", "订单PayID ：" + order.PayID + "Promotion ：" + order.promotionID);
            }
            catch (Exception e)
            { 
            
            }
        }

        //private static void FreeCoupon(PromotionRuleEntity rule, PromotionOrderInfo order)
        //{
        //    CouponAdapter.PromotionPresent(order, rule.ID, rule.TargetID);
        //}

        private static void UserCashCoupon(PromotionRuleEntity rule, PromotionOrderInfo order)
        {
            CouponAdapter.PresentUserCoupon(order, rule.ID, rule.TargetID);
        }

        private static void EAPointsAdd(PromotionRuleEntity rule, PromotionOrderInfo order)
        {
            EasternAirLinesAdapter.PromotionEAPointsAdd(order, rule);
        }

        private static void PromotionCouponEAPointsAdd(PromotionRuleEntity rule, PromotionOrderInfo order)
        {
            EasternAirLinesAdapter.PromotionCouponEAPointsAdd(order, rule);
        }
    }

    public class PromotionOrderInfo
    {
        public long UserID { get; set; }
        public string PhoneNum { get; set; }
        public long orderid { get; set; }
        public long CID { get; set; }
        public int PayID { get; set; }
        public int SKUID { get; set; }
        public int promotionID { get; set; }
        public decimal PayPrice { get; set; }
        public int CustomerType { get; set; }
        public int InnerBuyGroup { get; set; }

        public long CouponOrderId { get; set; }
    }
}
