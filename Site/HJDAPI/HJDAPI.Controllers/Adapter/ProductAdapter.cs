using HJD.AccountServices.Entity;
using HJD.CouponService.Contracts;
using HJD.CouponService.Contracts.Entity;
using HJD.Framework.Interface;
using HJD.Framework.WCF;
using HJDAPI.Controllers.Cache;
using HJDAPI.Controllers.Common;
using ProductService.Contracts;
using ProductService.Contracts.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.Controllers.Adapter
{
    public class ProductAdapter
    {
        private static IProductService productSvc = ServiceProxyFactory.Create<IProductService>("basicHttpBinding_IProductService");
        private static ICouponService couponSvc = ServiceProxyFactory.Create<ICouponService>("ICouponService");

        public static ICacheProvider LocalCache100Min = CacheManagerFactory.Create("DynamicCacheForType2");//抢券活动缓存 100分钟 




        #region 产品相关
        public static Boolean IsRetailerProduct(int SKUID)
        {
            return Convert.ToBoolean(LocalCache100Min.GetData<Object>(string.Format("Product.IsRetailerProduct:{0}", SKUID),
         () =>
         {
             var sku = productSvc.GetSKUInfoByID(SKUID);
             return sku.SPU.Name.StartsWith("[分销]");
         }));
        }

        public static SKUInfoEntity GetSKUInfoByID(int SKUID)
        {
            return productSvc.GetSKUInfoByID(SKUID);
        }

        /// <summary>
        /// 通过SKUID获取SKU数据（仅SKU表的数据）
        /// </summary>
        /// <param name="SKUID"></param>
        /// <returns></returns>
        public static SKUEntity GetSKUItemByID(int SKUID)
        {
            return productSvc.GetSKUByID(SKUID);
        }

        /// <summary>
        /// 通过SKUID获取SKU数据（SKU、sku_ex、sku_thirdPartyRel表的数据）
        /// </summary>
        /// <param name="SKUID"></param>
        /// <returns></returns>
        public static SKUEntity GetSKUEXEntityByID(int SKUID)
        {
            return productSvc.GetSKUEntityByID(SKUID);
        }

        public static List<BookDetailEntity> GetGroupBookDetailIdBySupplierId(int supplierId)
        {
            return productSvc.GetGroupBookDetailIdBySupplierId(supplierId);
        }

        public static SPUEntity GetSPUBySKUID(int skuid)
        {
            return productSvc.GetSPUBySKUID(skuid);
        }


        public static void CheckPromotionAndRemoveUserBuyFirstPackagePriviledge(long userID, List<int> promotionIDs)
        {
            if (IsPromotionNeedBuyFirstPackagePriviledge(promotionIDs))
            {
                AccountAdapter.InsertOrDeleteUserPrivilegeRel(userID, (int)PrivilegeEnums.UserPrivilege.CanBuyVIPFirstPackage, false, HJD.HotelManagementCenter.Domain.OpLogBizType.SKUPromotion, promotionIDs.First(), string.Join(",", promotionIDs) + " CheckPromotionAndRemoveUserBuyFirstPackagePriviledge");
            }
        }

        public static void CheckSKUAndRemoveUserBuyFirstPackagePriviledge(long userID, List<int> SKUIDs)
        {
            if (IsSKUNeedBuyFirstPackagePriviledge(SKUIDs))
            {
                AccountAdapter.InsertOrDeleteUserPrivilegeRel(userID, (int)PrivilegeEnums.UserPrivilege.CanBuyVIPFirstPackage, false, HJD.HotelManagementCenter.Domain.OpLogBizType.BuySKU, SKUIDs.First(), string.Join(",", SKUIDs) + "CheckSKUAndRemoveUserBuyFirstPackagePriviledge");
            }
        }

        public static void CheckPromotionAndAddUserBuyFirstPackagePriviledge(long userID, int promotionID)
        {
            if (IsPromotionNeedBuyFirstPackagePriviledge(new List<int> { promotionID }))
            {
                AccountAdapter.InsertOrDeleteUserPrivilegeRel(userID, (int)PrivilegeEnums.UserPrivilege.CanBuyVIPFirstPackage, true, HJD.HotelManagementCenter.Domain.OpLogBizType.SKUPromotion, promotionID, "CheckPromotionAndAddUserBuyFirstPackagePriviledge");
            }
        }

        public static void CheckSKUAndAddUserBuyFirstPackagePriviledge(long userID, int SKUID)
        {
            if (IsSKUNeedBuyFirstPackagePriviledge(new List<int> { SKUID }))
            {
                AccountAdapter.InsertOrDeleteUserPrivilegeRel(userID, (int)PrivilegeEnums.UserPrivilege.CanBuyVIPFirstPackage, true, HJD.HotelManagementCenter.Domain.OpLogBizType.BuySKU, SKUID, "CheckSKUAndAddUserBuyFirstPackagePriviledge");
            }
        }

        public static bool  StepGroupUpdateSKUPriceBySKUID(int skuid, int sellNull)
        {
            return   productSvc.StepGroupUpdateSKUPriceBySKUID(skuid, sellNull);
        }

        #endregion



        #region Product Promotion 优惠规则相关


        /// <summary>
        /// 优惠是否使用了购买首套VIP权限
        /// </summary>
        /// <param name="promotionID"></param>
        /// <returns></returns>
        public static bool IsPromotionNeedBuyFirstPackagePriviledge(List<int> promotionIDs)
        {
            return ProductAdapter.GetPromotionRuleListByIDList(promotionIDs).Where(r => r.PrivID.IndexOf(((int)PrivilegeEnums.UserPrivilege.CanBuyVIPFirstPackage).ToString()) >= 0).Count() > 0;
        }

        /// <summary>
        /// SKU是否标记了使用首套VIP权限
        /// </summary>
        /// <param name="promotionID"></param>
        /// <returns></returns>
        public static bool IsSKUNeedBuyFirstPackagePriviledge(List<int> SKUIDList)
        {
            var list = GetTagObjRelByIDs(ProductServiceEnums.TagObjectType.SKU, SKUIDList);
            return list.Where(l => l.TagID == (int)ProductServiceEnums.ProductTagType.NeedVIPFirstPayPriviledge).Count() > 0;
        }

        public static List<PromotionRuleEntity> GetPromotionRuleListByIDList(List<int> promotionIDList)
        {
            return productSvc.GetPromotionRuleListByIDList(promotionIDList);
        }

        public static List<TagObjRelEntity> GetTagObjRelByIDs(ProductService.Contracts.Entity.ProductServiceEnums.TagObjectType objType, List<int> IDList)
        {
            return productSvc.GetTagObjRelByIDs(objType, IDList);
        }

        /// <summary>
        /// 【消费券产品】根据指定用户的购买产品相关参数，检测当前享受的Promotion优惠策略
        /// </summary>
        /// <param name="skuid">SKUID</param>
        /// <param name="buynum">购买数量</param>
        /// <param name="userid">UserId</param>
        /// <param name="sType">购买场景（wap/web/app/weixin/wxapp）</param>
        /// <param name="usType">购买阶段</param>
        /// <returns></returns>
        public static PromotionCheckEntity CheckProductPromotionForCoupon(int skuid, int buynum, long userid, ProductServiceEnums.SceneType sType, ProductServiceEnums.PromotionUseSceneType usType = ProductServiceEnums.PromotionUseSceneType.Shopping)
        {
            var result = new PromotionCheckEntity
            {
                SKUID = skuid,
                PromotionRuleList = null,

                PromotionPriceItemList = new List<PromotionCheckItemPriceEntity>(),
                PromotionVIPPriceItemList = new List<PromotionCheckItemPriceEntity>(),
                PromotionPrice = 0,
                PromotionVipPrice = 0,

                SellPriceItemList = new List<PromotionCheckItemPriceEntity>(),
                SellVIPPriceItemList = new List<PromotionCheckItemPriceEntity>(),
                SellPrice = 0,
                SellVipPrice = 0,

                SellPoints = 0,
                SellPointsItemList = new List<PromotionCheckItemPointsEntity>()
            };

            if (skuid > 0 && buynum > 0)
            {
                try
                {
                    #region 获取消费券的信息（默认价格信息等）

                    //消费券基本信息
                    var productInfo = CouponAdapter.GetSKUCouponActivityDetail(skuid);

                    //购买数量
                    var buyCount = buynum;

                    //单价
                    decimal unitPrice = productInfo.SKUInfo.SKU.Price;
                    decimal vipUnitPrice = productInfo.SKUInfo.SKU.VIPPrice;
                    var unitPoints = productInfo.SKUInfo.SKU.Points;

                    //总价
                    decimal sellTotalPrice = unitPrice * buyCount;
                    decimal vipTotalPrice = vipUnitPrice * buyCount;
                    var sellTotalPoints = unitPoints * buyCount;

                    result.SellPrice = sellTotalPrice;
                    result.SellVipPrice = vipTotalPrice;
                    result.SellPoints = sellTotalPoints;

                    //价格list
                    for (int i = 0; i < buyCount; i++)
                    {
                        result.SellPriceItemList.Add(new PromotionCheckItemPriceEntity { Price = unitPrice, OriPrice = vipUnitPrice, PromotionID = 0 });
                        result.SellVIPPriceItemList.Add(new PromotionCheckItemPriceEntity { Price = vipUnitPrice, OriPrice = unitPrice, PromotionID = 0 });
                        result.SellPointsItemList.Add(new PromotionCheckItemPointsEntity { Points = unitPoints, PromotionID = 0 });
                    }

                    #endregion

                    #region 存在消费券享有的优惠策略

                    var promotionRuleList = productSvc.GetPromotionRuleListBySKU(skuid);
                    if (promotionRuleList != null && promotionRuleList.Count > 0)
                    {
                        result.PromotionRuleList = new List<PromotionRuleEntity>();

                        //过滤赠送优惠方式(赠送优惠放至订单提交后和支付后处理)
                        promotionRuleList = promotionRuleList.Where(_ =>
                            (ProductServiceEnums.PromotionRuleType)_.Type != ProductServiceEnums.PromotionRuleType.FreeCashCoupon
                            && (ProductServiceEnums.PromotionRuleType)_.Type != ProductServiceEnums.PromotionRuleType.FreeVIP199nr
                            && (ProductServiceEnums.PromotionRuleType)_.Type != ProductServiceEnums.PromotionRuleType.FreeVIP199nr_NoFistPackagePriviledge
                            && (ProductServiceEnums.PromotionRuleType)_.Type != ProductServiceEnums.PromotionRuleType.FreeVIP599
                            && (ProductServiceEnums.PromotionRuleType)_.Type != ProductServiceEnums.PromotionRuleType.FreeVIP599_NoFistPackagePriviledge
                            && (ProductServiceEnums.PromotionRuleType)_.Type != ProductServiceEnums.PromotionRuleType.FreeCashCoupon
                            && (ProductServiceEnums.PromotionRuleType)_.Type != ProductServiceEnums.PromotionRuleType.FreeCoupon
                            && (ProductServiceEnums.PromotionRuleType)_.Type != ProductServiceEnums.PromotionRuleType.FreeSKU
                            && (ProductServiceEnums.PromotionRuleType)_.Type != ProductServiceEnums.PromotionRuleType.EAPointsAfterPay
                            && (ProductServiceEnums.PromotionRuleType)_.Type != ProductServiceEnums.PromotionRuleType.EAPointsAfterCheck
                            && (ProductServiceEnums.PromotionRuleType)_.Type != ProductServiceEnums.PromotionRuleType.UserCashCoupon).ToList();

                        //获取用户信息
                        User_Info userInfo = AccountAdapter.GetUserInfoByUserId(userid);

                        //获取用户是否VIP
                        var isVip = AccountAdapter.IsVIPCustomer(AccountAdapter.GetCustomerType(userid));

                        //获取用户角色
                        var roleList = AccountAdapter.GetUserRoleRelByUserId(userid) ?? new List<UserRole>();

                        //获取用户权限
                        var privList = PrivilegeAdapter.GetAllPrivilegeByUserId(userid) ?? new List<UserPrivilegeRel>();

                        #region 规则处理

                        //现在暂时先不考虑叠加优惠规则的情况，先取第一条规则套用 2017.03.31 haoy
                        for (int rNum = 0; rNum < 1 && promotionRuleList.Count > 0; rNum++)
                        {
                            var ruleItem = promotionRuleList[rNum];

                            #region 套用规则，算出用户是否可以参与优惠

                            ruleItem.Valid = true;
                            ruleItem.PromotionUseState = ProductServiceEnums.PromotionUseState.Adopt;
                            ruleItem.RoleIdList = ruleItem.RoleIdList.Where(_ => _ > 0).ToList();
                            ruleItem.PrivIdList = ruleItem.PrivIdList.Where(_ => _ > 0).ToList();

                            //享用份数
                            var canUseTotalNumber = ruleItem.TotalNumber;

                            #region 规则在活动有效期内

                            var now = DateTime.Now;

                            if (now < ruleItem.StartTime || now > ruleItem.EndTime)
                            {
                                ruleItem.Valid = false;
                                ruleItem.PromotionUseState = ProductServiceEnums.PromotionUseState.Refuse_Overdue;
                                result.Message = "不在活动有效期内";
                            }

                            #endregion

                            #region 最小金额条件

                            else if (ruleItem.MinPrice > 0 && (isVip ? vipTotalPrice : sellTotalPrice) < ruleItem.MinPrice)
                            {
                                ruleItem.Valid = false;
                                ruleItem.PromotionUseState = ProductServiceEnums.PromotionUseState.Refuse_MinPrice;
                                result.Message = "不满足最小购买金额";
                            }

                            #endregion

                            #region 最小数量条件

                            else if (ruleItem.MinCount > 0 && buyCount < ruleItem.MinCount)
                            {
                                ruleItem.Valid = false;
                                ruleItem.PromotionUseState = ProductServiceEnums.PromotionUseState.Refuse_MinCount;
                                result.Message = "不满足最小购买数量";
                            }

                            #endregion

                            #region 产品范围/类型条件（券/抢购/团购/套餐组合等等）

                            ////暂未定义
                            //else if (true)
                            //{
                            //    ruleItem.Valid = false;
                            //    ruleItem.PromotionUseState = ProductServiceEnums.PromotionUseState.Refuse_ProductType;
                            //    result.Message = "该产品不参与活动";
                            //}

                            #endregion

                            #region 购买场景判断（wap/web/app/weixin等等）

                            else if (!ruleItem.SceneTypeList.Exists(_ => _ == Convert.ToInt32(sType)))
                            {
                                ruleItem.Valid = false;
                                ruleItem.PromotionUseState = ProductServiceEnums.PromotionUseState.Refuse_SceneType;
                                result.Message = "购买场景不满足条件";
                            }

                            #endregion

                            #region 用户角色条件（普通用户/199会员/新199会员/599会员等等）

                            else if (ruleItem.RoleIdList != null && ruleItem.RoleIdList.Count > 0 && roleList != null && !ruleItem.RoleIdList.Exists(_ => roleList.Exists(_r => _r.RoleID == _)))
                            {
                                ruleItem.Valid = false;
                                ruleItem.PromotionUseState = ProductServiceEnums.PromotionUseState.Refuse_UserRole;
                                result.Message = "用户角色不符合条件";
                            }

                            #endregion

                            #region 用户权限条件（购买爆款的权限 等等...）

                            else if (ruleItem.PrivIdList != null && ruleItem.PrivIdList.Count > 0 && privList != null && !ruleItem.PrivIdList.Exists(_ => privList.Exists(_p => _p.PrivID == _)))
                            {
                                ruleItem.Valid = false;
                                ruleItem.PromotionUseState = ProductServiceEnums.PromotionUseState.Refuse_UserPrivilege;
                                result.Message = "用户权限不符合条件";
                            }

                            #endregion

                            else
                            {
                                #region 最后计算用户可享受的优惠份数

                                if (canUseTotalNumber > 0)
                                {
                                    //查询当前用户已经购买过几份该sku产品
                                    var exchangeList = couponSvc.GetExchangeCouponListByUserSKU(userid, skuid, ruleItem.ID);
                                    if (exchangeList != null && exchangeList.Count > 0)
                                    {
                                        //只查询在SKU参与该优惠日起购买的有效房券（2可兑换 3已使用）
                                        exchangeList = exchangeList.Where(_ => (_.State == 2 || _.State == 3) && _.CreateTime > ruleItem.SKURelationTime && _.CreateTime < ruleItem.EndTime).ToList();

                                        //减去之前已经享受的次数
                                        canUseTotalNumber = canUseTotalNumber - exchangeList.Count;
                                    }

                                    //判断是否还剩余享受次数
                                    if (canUseTotalNumber <= 0)
                                    {
                                        //如果没有享受次数，则直接OVER
                                        ruleItem.Valid = false;
                                        ruleItem.PromotionUseState = ProductServiceEnums.PromotionUseState.Refuse_TotalNumber;
                                        result.Message = "该优惠的使用次数已达上限";

                                        canUseTotalNumber = ruleItem.TotalNumber;
                                    }
                                }

                                #endregion
                            }

                            #endregion

                            //计算出最终优惠金额
                            result.PromotionPriceItemList = GenPromotionPrice(sellTotalPrice, unitPrice, buyCount, canUseTotalNumber, ruleItem);
                            result.PromotionVIPPriceItemList = GenPromotionPrice(vipTotalPrice, vipUnitPrice, buyCount, canUseTotalNumber, ruleItem);

                            result.PromotionPrice = result.PromotionPriceItemList.Sum(_ => _.Price);
                            result.PromotionVipPrice = result.PromotionVIPPriceItemList.Sum(_ => _.Price);

                            //符合优惠规则
                            if (ruleItem.Valid)
                            {
                                result.SellPriceItemList = result.PromotionPriceItemList;
                                result.SellVIPPriceItemList = result.PromotionVIPPriceItemList;

                                result.SellPrice = result.PromotionPrice;
                                result.SellVipPrice = result.PromotionVipPrice;
                            }

                            //不符合优惠规则，目前也计算出“如果符合条件的优惠结果，方便前端做展示/提示等使用”
                            if (!ruleItem.Valid)
                            {
                                //如果是最小价格不符合，则以最低价去生成优惠价
                                if (ruleItem.PromotionUseState == ProductServiceEnums.PromotionUseState.Refuse_MinPrice)
                                {
                                    result.PromotionPriceItemList = GenPromotionPrice(ruleItem.MinPrice, unitPrice, buyCount, canUseTotalNumber, ruleItem);
                                    result.PromotionVIPPriceItemList = GenPromotionPrice(ruleItem.MinPrice, vipUnitPrice, buyCount, canUseTotalNumber, ruleItem);

                                    result.PromotionPrice = result.PromotionPriceItemList.Sum(_ => _.Price);
                                    result.PromotionVipPrice = result.PromotionVIPPriceItemList.Sum(_ => _.Price);
                                }
                                //如果是最小数量不符合，则以最小数量去生成优惠价
                                else if (ruleItem.PromotionUseState == ProductServiceEnums.PromotionUseState.Refuse_MinCount)
                                {
                                    result.PromotionPriceItemList = GenPromotionPrice((unitPrice * ruleItem.MinCount), unitPrice, buyCount, canUseTotalNumber, ruleItem);
                                    result.PromotionVIPPriceItemList = GenPromotionPrice((vipUnitPrice * ruleItem.MinCount), vipUnitPrice, buyCount, canUseTotalNumber, ruleItem);

                                    result.PromotionPrice = result.PromotionPriceItemList.Sum(_ => _.Price);
                                    result.PromotionVipPrice = result.PromotionVIPPriceItemList.Sum(_ => _.Price);
                                }
                            }

                            result.PromotionRuleList.Add(ruleItem);
                        }

                        #endregion
                    }

                    #endregion

                    #region SKU没有关联优惠策略

                    else
                    {
                        result.Message = "SKU没有促销优惠";
                    }

                    #endregion
                }
                catch (Exception ex)
                {
                    result.Message = ex.Message + ex.StackTrace;
                    return result;
                }
            }
            else
            {
                result.Message = "缺少关键参数";
            }

            return result;
        }

        /// <summary>
        /// 【Base】产品优惠策略检测机制
        /// </summary>
        /// <param name="productType">产品类型（房券/消费券/常规酒店产品）</param>
        /// <param name="objId">产品ID（目前可以是SKUID/CouponId）</param>
        /// <param name="buynum">购买数量</param>
        /// <param name="userid">UserId</param>
        /// <param name="sType">购买场景（wap/web/app/weixin/wxapp）</param>
        /// <param name="usType">购买阶段：买前/买中/买后</param>
        /// <returns></returns>
        public static PromotionCheckEntity CheckProductPromotion(ProductServiceEnums.ProductType productType, int objId, int buynum, long userid, ProductServiceEnums.SceneType sType, ProductServiceEnums.PromotionUseSceneType usType = ProductServiceEnums.PromotionUseSceneType.Shopping)
        {
            var result = new PromotionCheckEntity
            {
                ObjID = objId,
                PromotionRuleList = new List<PromotionRuleEntity>(),

                PromotionPriceItemList = new List<PromotionCheckItemPriceEntity>(),
                PromotionVIPPriceItemList = new List<PromotionCheckItemPriceEntity>(),
                PromotionPrice = 0,
                PromotionVipPrice = 0,

                SellPriceItemList = new List<PromotionCheckItemPriceEntity>(),
                SellVIPPriceItemList = new List<PromotionCheckItemPriceEntity>(),
                SellPrice = 0,
                SellVipPrice = 0
            };

            if (objId > 0 && buynum > 0)
            {
                try
                {
                    #region 获取当前券的信息（默认价格信息等）

                    //单价
                    Decimal unitPrice = 0;
                    Decimal vipUnitPrice = 0;

                    switch (productType)
                    {
                        case ProductServiceEnums.ProductType.ProductCoupon:
                            {
                                //消费券基本信息
                                var sku = ProductAdapter.GetSKUItemByID(objId);

                                //单价
                                unitPrice = sku.Price;
                                vipUnitPrice = sku.VIPPrice;

                                break;
                            }
                        case ProductServiceEnums.ProductType.RoomCoupon:
                            {
                                //房券基本信息
                                var couponInfo = CouponAdapter.GetCouponActivityDetail(objId, false);//10min缓存 //CouponAdapter.GetSKUCouponActivityDetail(couponid);

                                //设置单价
                                var pingri = couponInfo.package.PackagePrice.First(_ => _.Price > 0);
                                if (pingri != null && pingri.PID > 0)
                                {
                                    unitPrice = pingri.Price;
                                    vipUnitPrice = pingri.Price;
                                }

                                break;
                            }
                    }

                    //购买数量
                    var buyCount = buynum;

                    //总价
                    var sellTotalPrice = unitPrice * buyCount;
                    var vipTotalPrice = vipUnitPrice * buyCount;

                    result.SellPrice = sellTotalPrice;
                    result.SellVipPrice = vipTotalPrice;

                    //价格list
                    for (int i = 0; i < buyCount; i++)
                    {
                        result.SellPriceItemList.Add(new PromotionCheckItemPriceEntity { Price = unitPrice, PromotionID = 0 });
                        result.SellVIPPriceItemList.Add(new PromotionCheckItemPriceEntity { Price = vipUnitPrice, PromotionID = 0 });
                    }

                    #endregion

                    #region 存在当前券享有的优惠策略

                    var promotionRuleList = new List<PromotionRuleEntity>();

                    switch (productType)
                    {
                        case ProductServiceEnums.ProductType.ProductCoupon:
                            {
                                promotionRuleList = productSvc.GetPromotionRuleListBySKU(objId);

                                break;
                            }
                        case ProductServiceEnums.ProductType.RoomCoupon:
                            {
                                promotionRuleList = productSvc.GetPromotionRuleListByCouponID(objId);

                                break;
                            }
                    }

                    if (promotionRuleList != null && promotionRuleList.Count > 0)
                    {
                        result.PromotionRuleList = new List<PromotionRuleEntity>();

                        //根据不同购买阶段，选择性过滤一部分优惠方式（如买前只需要满减/一口价等；买中只需要优惠券使用/指定支付方式优惠等；买后只需要赠送VIP赠送产品等）
                        switch (usType)
                        {
                            //买前
                            case ProductServiceEnums.PromotionUseSceneType.Shopping:
                                {
                                    //过滤赠送优惠方式(赠送优惠放至订单提交后和支付后处理)
                                    promotionRuleList = promotionRuleList.Where(_ => IsShopingRule(
                                        (ProductServiceEnums.PromotionRuleType)_.Type)).ToList();
                                    break;
                                }
                            //买中
                            case ProductServiceEnums.PromotionUseSceneType.Submit:
                                {
                                    break;
                                }
                            //买后
                            case ProductServiceEnums.PromotionUseSceneType.PayComplate:
                                {
                                    //过滤满减/折扣/一口价
                                    promotionRuleList = promotionRuleList.Where(_ => IsPayComplateRule(
                                        (ProductServiceEnums.PromotionRuleType)_.Type)).ToList();
                                    break;
                                }
                        }

                        //获取用户信息
                        User_Info userInfo = AccountAdapter.GetUserInfoByUserId(userid);

                        //获取用户是否VIP
                        var isVip = AccountAdapter.IsVIPCustomer(AccountAdapter.GetCustomerType(userid));

                        //获取用户角色
                        var roleList = AccountAdapter.GetUserRoleRelByUserId(userid) ?? new List<UserRole>();

                        //获取用户权限
                        var privList = PrivilegeAdapter.GetAllPrivilegeByUserId(userid) ?? new List<UserPrivilegeRel>();

                        #region 规则处理

                        //现在暂时先不考虑叠加优惠规则的情况，先取第一条规则套用 2017.03.31 haoy
                        var forMinCount = 1;
                        if (usType == ProductServiceEnums.PromotionUseSceneType.PayComplate)
                        {
                            forMinCount = promotionRuleList.Count;
                        }
                        for (int rNum = 0; rNum < forMinCount && promotionRuleList.Count > 0; rNum++)
                        {
                            var ruleItem = promotionRuleList[rNum];

                            #region 套用规则，算出用户是否可以参与优惠

                            ruleItem.Valid = true;
                            ruleItem.PromotionUseState = ProductServiceEnums.PromotionUseState.Adopt;
                            ruleItem.RoleIdList = ruleItem.RoleIdList.Where(_ => _ > 0).ToList();
                            ruleItem.PrivIdList = ruleItem.PrivIdList.Where(_ => _ > 0).ToList();

                            //享用份数
                            var canUseTotalNumber = ruleItem.TotalNumber;

                            var now = DateTime.Now;

                            #region 如果当前用户已经是VIP，优惠方式又是赠送VIP，则不能享受

                            if (isVip && ((ProductServiceEnums.PromotionRuleType)ruleItem.Type == ProductServiceEnums.PromotionRuleType.FreeVIP199nr
                                || (ProductServiceEnums.PromotionRuleType)ruleItem.Type == ProductServiceEnums.PromotionRuleType.FreeVIP199nr_NoFistPackagePriviledge
                                || (ProductServiceEnums.PromotionRuleType)ruleItem.Type == ProductServiceEnums.PromotionRuleType.FreeVIP599
                                || (ProductServiceEnums.PromotionRuleType)ruleItem.Type == ProductServiceEnums.PromotionRuleType.FreeVIP599_NoFistPackagePriviledge))
                            {
                                ruleItem.Valid = false;
                                ruleItem.PromotionUseState = ProductServiceEnums.PromotionUseState.Refuse_TotalNumber;
                                result.Message = "当前用户已经是VIP，不能再次赠送VIP";
                            }

                            #endregion

                            #region 规则在活动有效期内

                            else if (now < ruleItem.StartTime || now > ruleItem.EndTime.Date.AddDays(1))
                            {
                                ruleItem.Valid = false;
                                ruleItem.PromotionUseState = ProductServiceEnums.PromotionUseState.Refuse_Overdue;
                                result.Message = "不在活动有效期内";
                            }

                            #endregion

                            #region 最小金额条件

                            else if (ruleItem.MinPrice > 0 && (isVip ? vipTotalPrice : sellTotalPrice) < ruleItem.MinPrice)
                            {
                                ruleItem.Valid = false;
                                ruleItem.PromotionUseState = ProductServiceEnums.PromotionUseState.Refuse_MinPrice;
                                result.Message = "不满足最小购买金额";
                            }

                            #endregion

                            #region 最小数量条件

                            else if (ruleItem.MinCount > 0 && buyCount < ruleItem.MinCount)
                            {
                                ruleItem.Valid = false;
                                ruleItem.PromotionUseState = ProductServiceEnums.PromotionUseState.Refuse_MinCount;
                                result.Message = "不满足最小购买数量";
                            }

                            #endregion

                            #region 产品范围/类型条件（券/抢购/团购/套餐组合等等）

                            ////暂未定义
                            //else if (true)
                            //{
                            //    ruleItem.Valid = false;
                            //    ruleItem.PromotionUseState = ProductServiceEnums.PromotionUseState.Refuse_ProductType;
                            //    result.Message = "该产品不能参与活动";
                            //}

                            #endregion

                            #region 购买场景判断（wap/web/app/weixin等等）

                            else if (!ruleItem.SceneTypeList.Exists(_ => _ == Convert.ToInt32(sType)))
                            {
                                ruleItem.Valid = false;
                                ruleItem.PromotionUseState = ProductServiceEnums.PromotionUseState.Refuse_SceneType;
                                result.Message = "购买场景不满足条件";
                            }

                            #endregion

                            #region 用户角色条件（普通用户/199会员/新199会员/599会员等等）

                            else if (ruleItem.RoleIdList != null && ruleItem.RoleIdList.Count > 0 && roleList != null && !ruleItem.RoleIdList.Exists(_ => roleList.Exists(_r => _r.RoleID == _)))
                            {
                                ruleItem.Valid = false;
                                ruleItem.PromotionUseState = ProductServiceEnums.PromotionUseState.Refuse_UserRole;
                                result.Message = "用户角色不符合条件";
                            }

                            #endregion

                            #region 用户权限条件（购买爆款的权限 等等...）

                            else if (ruleItem.PrivIdList != null && ruleItem.PrivIdList.Count > 0 && privList != null && !ruleItem.PrivIdList.Exists(_ => privList.Exists(_p => _p.PrivID == _)))
                            {
                                ruleItem.Valid = false;
                                ruleItem.PromotionUseState = ProductServiceEnums.PromotionUseState.Refuse_UserPrivilege;
                                result.Message = "用户权限不符合条件";
                            }

                            #endregion

                            else
                            {
                                #region 最后计算用户可享受的优惠份数

                                if (canUseTotalNumber > 0)
                                {
                                    //查询当前用户已经购买过几份该sku产品
                                    var exchangeList = new List<ExchangeCouponEntity>();
                                    if ((ProductServiceEnums.PromotionRuleType)ruleItem.Type != ProductServiceEnums.PromotionRuleType.FreeSKU)
                                    {
                                        exchangeList = couponSvc.GetExchangeCouponListByUserSKU(userid, ruleItem.TargetID, ruleItem.ID);
                                    }
                                    else
                                    {
                                        exchangeList = couponSvc.GetExchangeCouponListByUserSKU(userid, objId, ruleItem.ID);
                                    }

                                    if (exchangeList != null && exchangeList.Count > 0)
                                    {
                                        //只查询在SKU参与该优惠日起购买的有效房券（2可兑换 3已使用）
                                        exchangeList = exchangeList.Where(_ => (_.State == 2 || _.State == 3) && _.CreateTime > ruleItem.SKURelationTime && _.CreateTime < ruleItem.EndTime).ToList();

                                        //减去之前已经享受的次数
                                        canUseTotalNumber = canUseTotalNumber - exchangeList.Count;
                                    }

                                    //判断是否还剩余享受次数
                                    if (canUseTotalNumber <= 0)
                                    {
                                        //如果没有享受次数，则直接OVER
                                        ruleItem.Valid = false;
                                        ruleItem.PromotionUseState = ProductServiceEnums.PromotionUseState.Refuse_TotalNumber;
                                        result.Message = "该优惠的使用次数已达上限";

                                        canUseTotalNumber = ruleItem.TotalNumber;
                                    }
                                }

                                #endregion
                            }

                            #endregion

                            //计算出最终优惠金额
                            result.PromotionPriceItemList = GenPromotionPrice(sellTotalPrice, unitPrice, buyCount, canUseTotalNumber, ruleItem);
                            result.PromotionVIPPriceItemList = GenPromotionPrice(vipTotalPrice, vipUnitPrice, buyCount, canUseTotalNumber, ruleItem);

                            result.PromotionPrice = result.PromotionPriceItemList.Sum(_ => _.Price);
                            result.PromotionVipPrice = result.PromotionVIPPriceItemList.Sum(_ => _.Price);

                            //符合优惠规则
                            if (ruleItem.Valid)
                            {
                                result.SellPriceItemList = result.PromotionPriceItemList;
                                result.SellVIPPriceItemList = result.PromotionVIPPriceItemList;

                                result.SellPrice = result.PromotionPrice;
                                result.SellVipPrice = result.PromotionVipPrice;
                            }

                            //不符合优惠规则，目前也计算出“如果符合条件的优惠结果，方便前端做展示/提示等使用”
                            if (!ruleItem.Valid)
                            {
                                //如果是最小价格不符合，则以最低价去生成优惠价
                                if (ruleItem.PromotionUseState == ProductServiceEnums.PromotionUseState.Refuse_MinPrice)
                                {
                                    result.PromotionPriceItemList = GenPromotionPrice(ruleItem.MinPrice, unitPrice, buyCount, canUseTotalNumber, ruleItem);
                                    result.PromotionVIPPriceItemList = GenPromotionPrice(ruleItem.MinPrice, vipUnitPrice, buyCount, canUseTotalNumber, ruleItem);

                                    result.PromotionPrice = result.PromotionPriceItemList.Sum(_ => _.Price);
                                    result.PromotionVipPrice = result.PromotionVIPPriceItemList.Sum(_ => _.Price);
                                }
                                //如果是最小数量不符合，则以最小数量去生成优惠价
                                else if (ruleItem.PromotionUseState == ProductServiceEnums.PromotionUseState.Refuse_MinCount)
                                {
                                    result.PromotionPriceItemList = GenPromotionPrice((unitPrice * ruleItem.MinCount), unitPrice, buyCount, canUseTotalNumber, ruleItem);
                                    result.PromotionVIPPriceItemList = GenPromotionPrice((vipUnitPrice * ruleItem.MinCount), vipUnitPrice, buyCount, canUseTotalNumber, ruleItem);

                                    result.PromotionPrice = result.PromotionPriceItemList.Sum(_ => _.Price);
                                    result.PromotionVipPrice = result.PromotionVIPPriceItemList.Sum(_ => _.Price);
                                }
                            }

                            result.PromotionRuleList.Add(ruleItem);
                        }

                        #endregion
                    }

                    #endregion

                    #region SKU没有关联优惠策略

                    else
                    {
                        result.Message = "SKU没有促销优惠";
                    }

                    #endregion
                }
                catch (Exception ex)
                {
                    result.Message = ex.Message + ex.StackTrace;
                    return result;
                }
            }
            else
            {
                result.Message = "缺少关键参数";
            }

            return result;
        }


        private static bool IsShopingRule(ProductServiceEnums.PromotionRuleType Type)
        {
            return Type == ProductServiceEnums.PromotionRuleType.Discount ||
                                 Type == ProductServiceEnums.PromotionRuleType.FixedPrice ||
                                Type == ProductServiceEnums.PromotionRuleType.Markdown;
        }

        /// <summary>
        /// 兑换规则
        /// </summary>
        /// <param name="Type"></param>
        /// <returns></returns>
        private static bool IsExchangeRule(ProductServiceEnums.PromotionRuleType Type)
        {
            return Type == ProductServiceEnums.PromotionRuleType.EAPointsAfterCheck;
        }

        private static bool IsPayComplateRule(ProductServiceEnums.PromotionRuleType Type)
        {
            return Type == ProductServiceEnums.PromotionRuleType.FreeCashCoupon ||
                                Type == ProductServiceEnums.PromotionRuleType.FreeVIP199nr ||
                                Type == ProductServiceEnums.PromotionRuleType.FreeVIP199nr_NoFistPackagePriviledge ||
                                Type == ProductServiceEnums.PromotionRuleType.FreeVIP599 ||
                                Type == ProductServiceEnums.PromotionRuleType.FreeVIP599_NoFistPackagePriviledge ||
                                Type == ProductServiceEnums.PromotionRuleType.FreeCashCoupon ||
                                Type == ProductServiceEnums.PromotionRuleType.FreeCoupon ||
                                Type == ProductServiceEnums.PromotionRuleType.FreeSKU ||
                                Type == ProductServiceEnums.PromotionRuleType.EAPointsAfterPay||
                                Type == ProductServiceEnums.PromotionRuleType.UserCashCoupon;
        }

        /// <summary>
        /// 获取促销规则下的价格
        /// </summary>
        /// <param name="unitPrice"></param>
        /// <param name="buyCount"></param>
        /// <param name="canUseTotalNumber"></param>
        /// <param name="ruleItem"></param>
        private static List<PromotionCheckItemPriceEntity> GenPromotionPrice(decimal sellTotalPrice, decimal unitPrice, int buyCount, int canUseTotalNumber, PromotionRuleEntity ruleItem)
        {
            List<PromotionCheckItemPriceEntity> itemPriceList = new List<PromotionCheckItemPriceEntity>();

            for (int i = 0; i < buyCount; i++)
            {
                itemPriceList.Add(new PromotionCheckItemPriceEntity { Price = unitPrice, PromotionID = 0 });
            }

            #region 一口价

            if ((ProductServiceEnums.PromotionRuleType)ruleItem.Type == ProductServiceEnums.PromotionRuleType.FixedPrice)
            {
                for (int _bnum = 0; _bnum < buyCount; _bnum++)
                {
                    var _temSellPrice = (decimal)unitPrice;
                    if (canUseTotalNumber == 0 || _bnum < canUseTotalNumber)
                    {
                        _temSellPrice = (int)ruleItem.Discount;
                        itemPriceList[_bnum].Price = _temSellPrice;
                        itemPriceList[_bnum].PromotionID = ruleItem.ID;
                    }
                }
            }

            #endregion

            #region 立减

            else if ((ProductServiceEnums.PromotionRuleType)ruleItem.Type == ProductServiceEnums.PromotionRuleType.Markdown)
            {
                decimal unitDiscount = ruleItem.Discount / buyCount;
                decimal _tempTotalPrice = 0;
                for (int i = 0; i < buyCount - 1; i++)
                {
                    itemPriceList[i].Price = itemPriceList[i].Price - unitDiscount;
                    itemPriceList[i].PromotionID = ruleItem.ID;
                    _tempTotalPrice += itemPriceList[i].Price;
                }
                itemPriceList[buyCount - 1].Price = sellTotalPrice - ruleItem.Discount - _tempTotalPrice;
                itemPriceList[buyCount - 1].PromotionID = ruleItem.ID;
            }

            #endregion

            #region 折扣

            else if ((ProductServiceEnums.PromotionRuleType)ruleItem.Type == ProductServiceEnums.PromotionRuleType.Discount)
            {
                for (int i = 0; i < buyCount - 1; i++)
                {
                    itemPriceList[i].Price = itemPriceList[i].Price * ruleItem.Discount / 100;
                    itemPriceList[buyCount - 1].PromotionID = ruleItem.ID;
                }
            }

            #endregion

            return itemPriceList;
        }


        public static int AddGroupPurchase(GroupPurchaseEntity param)
        {
            return productSvc.AddGroupPurchase(param);
        }
        public static int AddGroupPurchaseDetail(GroupPurchaseDetailEntity param)
        {
            return productSvc.AddGroupPurchaseDetail(param);
        }

        public static List<ProductPropertyInfoEntity> GetProductPropertyInfoBySKU(string skuids)
        {
            return productSvc.GetProductPropertyInfoBySKU(skuids);
        }
        public static StepGroupEntity GetStepGroupBySPUID(int spuid)
        {
            return productSvc.GetStepGroupBySPUID(spuid);
        }
        public static List<GroupPurchaseInfoEntity> GetGroupPurchaseCountBySkuAndUserid(int skuid, long userid)
        {
            return productSvc.GetGroupPurchaseCountBySkuAndUserid(skuid, userid);
        }

        public static int GetGroupPurchaseDetailCountByGroupId(int groupid)
        {
            return productSvc.GetGroupPurchaseDetailCountByGroupId(groupid);
        }

        public static List<GroupPurchaseEntity> GetGroupPurchaseBySKUID(int SKUID, int state)
        {
            return productSvc.GetGroupPurchaseBySKUID(SKUID, state);
        }
        public static List<GroupPurchaseEntity> GetGroupPurchaseContainCABySKUID(int SKUID)
        {
            return productSvc.GetGroupPurchaseContainCABySKUID(SKUID);
        }

        

        public static int UpdateGroupPurchase(int id, int state)
        {
            int result= productSvc.UpdateGroupPurchase(id, state);
            //更新团状态Redis缓存
            try
            {
                var couponList = couponSvc.GetExchangeCouponEntityListByGroupId(id, 0);
                foreach (var item in couponList)
                {
                    ProductCache.RemoveUserDetailOrderCache(item.ID);
                }
            }
            catch (Exception ex)
            {
                Log.WriteLog("更新团状态Redis缓存失败，GroupID=" + id + "错误信息：" + ex.Message + ex.Source);
            }
            return result;
        }


        public static List<GroupPurchaseDetailEntity> GetGroupPurchaseDetailByGroupId(int groupid, int categoryId=0)
        {
            return productSvc.GetGroupPurchaseDetailByGroupId(groupid, categoryId);
        }

        /// <summary>
        /// 获取团状态信息
        /// </summary>
        /// <param name="groupid">团ID</param>
        /// <returns>如果没有对应的团，返回新的空对象</returns>
        public static GroupPurchaseEntity GetGroupPurchaseEntityByGroupID(int groupid)
        {
            var list = productSvc.GetGroupPurchaseEntity(groupid);
            if(list.Count == 0)
            {
                return new GroupPurchaseEntity();
            }
            else
            {
                return list.First();
            }
        }

        public static List<GroupPurchaseEntity> GetGroupPurchaseEntity(int groupid)
        {
            return productSvc.GetGroupPurchaseEntity(groupid);
        }

        public static List<GroupPurchaseEntity> GetGroupPurchaseByStateAndEndTime(int state, DateTime endTime)
        {
            return productSvc.GetGroupPurchaseByStateAndEndTime(state, endTime);
        }

        public static List<GroupPurchaseEntity> GetAutoGroupByTime()
        {
            return productSvc.GetAutoGroupByTime();
        }
        

        public static List<GroupPurchaseEntity> GetGroupPurchaseByOpenID(string openid,int skuid)
        {
            return productSvc.GetGroupPurchaseByOpenID(openid, skuid);
        }

        public static List<ProductAlbumSKUEntity> GetProductAlbumSkuByAlbumIds(string albumIds)
        {
            return productSvc.GetProductAlbumSkuByAlbumIds(albumIds);
        }
        #endregion

         /// <summary>
        /// 获取指定SKU享受的优惠规则
        /// </summary>
        /// <param name="SKUID"></param>
        /// <returns></returns>
        public static List<PromotionRuleEntity> GetPromotionRuleListBySKU(int SKUID)
        {
            return productSvc.GetPromotionRuleListBySKU(SKUID);
        }
        #region 预约相关

        public static int AddBookUserDateInfo(BookUserDateInfoEntity param)
        {
            return productSvc.AddBookUserDateInfo(param);
        }

        public static int UpdateBookUserDateInfo(BookUserDateInfoEntity param)
        {
            return productSvc.UpdateBookUserDateInfo(param);
        }

        public static BookUserDateInfoEntity GetBookedUserInfoById(int id)
        {
            return productSvc.GetBookedUserInfoById(id);
        }
        public static List<BookUserDateInfoEntity> GetBookUserDateInfoByExchangID(int id)
        {
            return productSvc.GetBookUserDateInfoByExchangID(id);
        }
        //public static int AddBookUserInfo(BookUserInfoEntity model)
        //{
        //    return productSvc.AddBookUserInfo(model);
        //}

        /// <summary>
        /// 购买前预约的订单 支付后更新预约信息的状态
        /// </summary>
        /// <param name="exchangeCouponId"></param>
        public static void UpdateCouponOrderBookUserDateByCouponId(int exchangeCouponId)
        {
            List<BookUserDateInfoEntity> list = productSvc.GetBookUserDateInfoByExchangID(exchangeCouponId);

            BookUserDateInfoEntity bud = list.Where(_ => _.State == 2).OrderByDescending(_ => _.ID).ToList().FirstOrDefault();
            if (bud != null && bud.ID > 0)
            {
               //BookUserDateInfoEntity upBud= productSvc.GetBookedUserInfoById(bud.ID);
                bud.State = 0;
                UpdateBookUserDateInfo(bud);
            }

        }

        public static List<BookDayItem> GetBookDateList(int bookId, int skuStockCount)
        {
            return productSvc.GetBookDateList(bookId, skuStockCount);
        }

        public static List<BookDayItem> GetBookDateListBySql(int bookId, int skuStockCount)
        {
            return productSvc.GetBookDateListBySql(bookId, skuStockCount);
        }


        public static List<BookDetailItem> GetBookDetailItemList(int bookId, int bookDateId, int skuStockCount)
        {
            return productSvc.GetBookDetailList(bookId, bookDateId, skuStockCount);
        }

        public static bool IsCanBookItem(int bookDateid, int bookDetailId, int skuStockCount)
        {
            return productSvc.IsCanBookItem(bookDateid, bookDetailId, skuStockCount);
        }


        public static List<int> GetDistinctSPUDistrictID(int parentCategoryID, int payType)
        {
            return productSvc.GetDistinctSPUDistrictID(parentCategoryID, payType);
        }

        public static List<int> GetDistinctSPUDistrictIDByAlbum(int albumID)
        {
            return productSvc.GetDistinctSPUDistrictIDByAlbum(albumID);
        }

        public static List<int> GetDistinctRetailSPUDistrict()
        {
            return productSvc.GetDistinctRetailSPUDistrict();
        }

        public static List<int> GetDistinctSPUDistrictIDByProductTagID(int productTagID)
        {
            return productSvc.GetDistinctSPUDistrictIDByProductTagID(productTagID);
        }
        
        public static List<ProcudtCategoryEntity> GetProductCategoryListByParentID(int parentID)
        {
            return productSvc.GetProductCategoryListByParentID(parentID);
        }
        public static List<ProcudtCategoryEntity> GetProductCategoryList(int id)
        {
            return productSvc.GetProductCategoryList(id);
        }
        public static List<int> GetSPUTagID(int districtID)
        {
            return productSvc.GetSPUTagID(districtID);
        }

        public static List<StepGroupEntity> GetStepGroupByState(int state)
        {
            return productSvc.GetStepGroupByState(state);
        }

        public static int UpdateStepGroup(StepGroupEntity param)
        {
            return productSvc.UpdateStepGroup(param);
        }

        public static SPUInfoEntity GetSPUInfoByID(int SPUID)
        {
            return productSvc.GetSPUInfoByID(SPUID);
        }
        #endregion
    }
}
