using HJD.CouponService.Contracts;
using HJD.CouponService.Contracts.Entity;
using HJD.Framework.WCF;
using HJD.HotelManagementCenter.Domain;
using HJDAPI.Controllers.Adapter;
using HJDAPI.Controllers.Biz;
using HJDAPI.Models;
using ProductService.Contracts;
using ProductService.Contracts.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace HJDAPI.Controllers
{
    public class ShopController : BaseApiController
    {
        public static HJD.HotelManagementCenter.IServices.ISupplierService SupplierService = ServiceProxyFactory.Create<HJD.HotelManagementCenter.IServices.ISupplierService>("HMC_ISupplierService");
        private static IProductService productSvc = ServiceProxyFactory.Create<IProductService>("basicHttpBinding_IProductService");
        private static ICouponService couponSvc = ServiceProxyFactory.Create<ICouponService>("ICouponService");

        [HttpPost]
        public BusinessOperatorEntity ShopLogin(ShopLoginParam param)
        {
            BusinessOperatorEntity result = new BusinessOperatorEntity();

            result = SupplierService.ExistsOperate(param.OperatorName, param.PassWord);

            return result;
        }
        [HttpPost]
        public BusinessOperatorEntity ChannelLogin(ChannelLoginParam param)
        {
            BusinessOperatorEntity result = new BusinessOperatorEntity();
            if (param.CType == 2)
            {
                result = SupplierService.ExistsOperate(param.OperatorName, param.PassWord);
            }
            else if (param.CType == 1)
            {
                OperationResult resultOpt = new OperationResult();
                resultOpt = new AccountsController().MobileLogin(new AccountInfoItem { Phone = param.OperatorName, Password = param.PassWord });
                if (resultOpt != null && resultOpt.Success == true)
                {
                    result.Phone = long.Parse(resultOpt.Mobile);
                    string[] resData = resultOpt.Data.Split('|');
                    result.UserID = long.Parse(resData[0].ToString());
                    result.OperatorName = resData[1].ToString();
                }
                else
                {
                    return new BusinessOperatorEntity();
                }
            }
            return result;
        }
        [HttpPost]
        public BusinessOperatorEntity PhoneNumChannelLogin(ChannelLoginParam param)
        {
            BusinessOperatorEntity result = new BusinessOperatorEntity();

            OperationResult resultOpt = new OperationResult();
            resultOpt = new AccountsController().PhoneNumLogin(new AccountInfoItem { Phone = param.PhoneNum, ConfirmCode = param.ConfirmCode, TimeStamp = param.TimeStamp, Sign = param.Sign, RequestType = param.RequestType, SourceID = param.SourceID });
            if (resultOpt != null && resultOpt.Success == true)
            {
                result.Phone = long.Parse(resultOpt.Mobile);
                string[] resData = resultOpt.Data.Split('|');
                result.UserID = long.Parse(resData[0].ToString());
                result.OperatorName = resData[1].ToString();
                result.Message = resultOpt.Message;
            }
            else
            {
                result.Message = resultOpt.Message ;
                return result;
                //return new BusinessOperatorEntity() { Message = resultOpt.Message };
            }
            return result;
        }
        [HttpGet]
        public int ExistsOperateName(string operatorName, int id)
        {
            return SupplierService.ExistsOperateName(operatorName, id);
        }
        //[HttpGet]
        //public CouponInfoEntity GetCouponInfoByExchangeNo(string exchangeNo)
        //{
        //    return CouponAdapter.GetCouponInfoByExchangeNo(exchangeNo);
        //}

        [HttpPost]
        public int UpdateExchangeState(ExchangeCouponEntity param)
        {

            return CouponAdapter.UpdateExchangeState(param);
        }

        [HttpPost]
        public int AddUsedConsumerCouponInfo(UsedConsumerCouponInfoEntity param)
        {
            int res = CouponAdapter.AddUsedConsumerCouponInfo(param);
            AddEAPointsAfterCheck(param);
            return res;
        }
        [HttpPost]
        public int AddEAPointsAfterCheck(UsedConsumerCouponInfoEntity param)
        {
            int res = 0;
            ExchangeCouponEntity ec = couponSvc.GetExchangeListByNO(param.ExchangeNo, 0).FirstOrDefault();
            if (ec != null)
            {
                List<PromotionRuleEntity> prlist = productSvc.GetPromotionRuleListBySKU(ec.SKUID);
                foreach (var item in prlist)
                {
                    PromotionOrderInfo pinfo = new PromotionOrderInfo();
                    pinfo.orderid = ec.ID;
                    pinfo.UserID = ec.UserID;
                    pinfo.PayPrice = ec.Price;
                    Promotion.DoPromotionForChecked(item, pinfo);
                }
            }
            return res;
        }
        [HttpGet]
        public List<UsedCouponProductEntity> GetUsedCouponProductBySupplierId(int supplierId, string startTime = "2000-01-01", string endTime = "2100-01-01", int start = 0, int count = 20)
        {
            if (string.IsNullOrWhiteSpace(startTime))
            {
                startTime = "2000-01-01";
            }
            if (string.IsNullOrWhiteSpace(endTime))
            {
                endTime = "2100-01-01";
            }

            //DateTime startTime = Convert.ToDateTime("2000-01-01");
            //DateTime endTime = Convert.ToDateTime("2100-01-01");
            return CouponAdapter.GetUsedCouponProductBySupplierId(supplierId, Convert.ToDateTime(startTime), Convert.ToDateTime(endTime).AddDays(1), start, count);
        }


        [HttpGet]
        public List<UsedCouponProductEntity> GetUsedCouponProductByOperatorId(int operatorId, string startTime = "2000-01-01", string endTime = "2100-01-01", int start = 0, int count = 20)
        {
            if (string.IsNullOrWhiteSpace(startTime))
            {
                startTime = "2000-01-01";
            }
            if (string.IsNullOrWhiteSpace(endTime))
            {
                endTime = "2100-01-01";
            }
            BusinessOperatorEntity bo = SupplierAdapter.GetBusinessOperatorByID(operatorId);
            if (bo.Grade == 1)
            {
                return CouponAdapter.GetUsedCouponProductBySupplierId(bo.BizID, Convert.ToDateTime(startTime), Convert.ToDateTime(endTime).AddDays(1), start, count);
            }
            else
            {
                return CouponAdapter.GetUsedCouponProductByOperatorId(operatorId, Convert.ToDateTime(startTime), Convert.ToDateTime(endTime).AddDays(1), start, count);
            }


            //DateTime startTime = Convert.ToDateTime("2000-01-01");
            //DateTime endTime = Convert.ToDateTime("2100-01-01");
        }

        [HttpGet]
        public UsedProductCouponEntity GetSumCouponProductBySupplierId(int supplierId, DateTime startTime, DateTime endTime, int start = 0, int count = 10000)
        {
            List<UsedCouponProductEntity> usedCoupon = CouponAdapter.GetUsedCouponProductBySupplierId(supplierId, startTime, endTime, start, count);
            UsedProductCouponEntity usedProduct = new UsedProductCouponEntity();
            if (usedCoupon != null && usedCoupon.Count > 0)
            {
                usedProduct.TotalCount = usedCoupon.Count;
                usedProduct.TotalAmount = usedCoupon.Sum(_ => _.SettlePrice);
                IEnumerable<IGrouping<string, UsedCouponProductEntity>> groupCoupon = usedCoupon.GroupBy(_ => _.SkuName);
                foreach (var item in usedCoupon.GroupBy(_ => _.SkuName))
                {
                    UsedDetailProductCouponEntity coupon = new UsedDetailProductCouponEntity();
                    coupon.SKUName = item.Key;
                    coupon.Count = item.Count();
                    coupon.Amount = item.Sum(_ => _.SettlePrice);
                    usedProduct.usedDetailProductCoupon.Add(coupon);
                }
            }
            return usedProduct;
        }


        [HttpGet]
        public UsedProductCouponEntity GetSumCouponProductByOperatorId(int operatorId, DateTime startTime, DateTime endTime, int start = 0, int count = 100000)
        {
            List<UsedCouponProductEntity> usedCoupon = new List<UsedCouponProductEntity>();
            BusinessOperatorEntity bo = SupplierAdapter.GetBusinessOperatorByID(operatorId);
            if (bo.Grade == 1)
            {
                usedCoupon = CouponAdapter.GetUsedCouponProductBySupplierId(bo.BizID, startTime, endTime, start, count);
            }
            else
            {
                usedCoupon = CouponAdapter.GetUsedCouponProductByOperatorId(operatorId, startTime, endTime, start, count);
            }
            //List<UsedCouponProductEntity> usedCoupon = CouponAdapter.GetUsedCouponProductBySupplierId(supplierId, startTime, endTime, start, count);
            UsedProductCouponEntity usedProduct = new UsedProductCouponEntity();
            if (usedCoupon != null && usedCoupon.Count > 0)
            {
                usedProduct.TotalCount = usedCoupon.Count;
                usedProduct.TotalAmount = usedCoupon.Sum(_ => _.SettlePrice);
                IEnumerable<IGrouping<string, UsedCouponProductEntity>> groupCoupon = usedCoupon.GroupBy(_ => _.SkuName);
                foreach (var item in usedCoupon.GroupBy(_ => _.SkuName))
                {
                    UsedDetailProductCouponEntity coupon = new UsedDetailProductCouponEntity();
                    coupon.SKUName = item.Key;
                    coupon.Count = item.Count();
                    coupon.Amount = item.Sum(_ => _.SettlePrice);
                    usedProduct.usedDetailProductCoupon.Add(coupon);
                }
            }
            return usedProduct;
        }

        [HttpGet]
        public BookNoUsedExchangeCouponEntity GetBookNoUsedExchangeCouponBySupplierId(int supplierId, DateTime startTime, DateTime endTime)
        {
            BookNoUsedExchangeCouponEntity result = new BookNoUsedExchangeCouponEntity();
            List<BookDetailEntity> bookDetailModel = ProductAdapter.GetGroupBookDetailIdBySupplierId(supplierId);
            List<BookNoUsedExchangeCouponInfoEntity> bookNoUsedList = CouponAdapter.GetBookNoUsedExchangeCouponBySupplierId(supplierId, startTime, endTime, 2);
            result.TotalCount = bookNoUsedList.Count;
            foreach (BookDetailEntity item in bookDetailModel)
            {
                List<BookNoUsedExchangeCouponInfoEntity> bookNoUsedInfoList = bookNoUsedList.Where(_ => _.BookDetailId == item.ID).ToList();
                BookNoUsedExchangeCouponDetailEntity bookDetail = new BookNoUsedExchangeCouponDetailEntity();
                if (bookNoUsedInfoList.Count > 0)
                {
                    bookDetail.BookDetailName = item.NumPlayName;
                    bookDetail.PeopleCount = bookNoUsedInfoList.Count;
                }
                else
                {
                    bookDetail.BookDetailName = item.NumPlayName;
                    bookDetail.PeopleCount = 0;
                }
                result.BookNoUsedList.Add(bookDetail);
            }
            return result;
        }

        [HttpGet]
        public SupplierEntity GetSupplierById(long supplierId)
        {
            return SupplierService.GetSupplierById(supplierId);
        }

        [HttpGet]
        public ExchangeCouponEntity GetOneExchangeCouponInfo(int couponID, string exchangeNo)
        {
            if (couponID == 0)
            {
                return CouponAdapter.GetOneExchangeCouponInfoByExchangeNo( exchangeNo);
            }
            else
            {

                return CouponAdapter.GetOneExchangeCouponInfoByCouponID(couponID);
            }
        }

        [HttpGet]
        public SKUInfoEntity GetSKUByID(int SKUID)
        {
            return ProductAdapter.GetSKUInfoByID(SKUID);
        }

        [HttpGet]
        public UsedConsumerCouponInfoEntity GetUsedCouponProductByExchangeNo(string exchangeNo)
        {
            return CouponAdapter.GetUsedCouponProductByExchangeNo(exchangeNo);
        }

        [HttpGet]
        public RetailerShopEntity GetRetailerShopByCID(long CID)
        {

            return RetailerAdapter.GetRetailerShopByCID(CID);

        }

        [HttpPost]
        public int AddRetailerShop(RetailerShopEntity retailershop)
        {
            return RetailerAdapter.AddRetailerShop(retailershop);
        }

        [HttpPost]
        public int AddOrUpdateRetailerShop(RetailerShopEntity retailershop)
        {
            return RetailerAdapter.AddOrUpdateRetailerShop(retailershop);
        }

        [HttpGet]
        public int UpdateAvatarUrl(long CID, string avatarUrl)
        {
            return RetailerAdapter.UpdateAvatarUrl(CID, avatarUrl);
        }

        [HttpPost]
        public int UpdateRetailerShop(RetailerShopEntity retailershop)
        {
            return RetailerAdapter.UpdateRetailerShop(retailershop);
        }
    }
}
