using HJD.CouponService.Contracts.Entity;
using HJD.HotelManagementCenter.Domain;
using HJDAPI.APIProxy;
using HJDAPI.Models;
using ProductService.Contracts.Entity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WHotelSite.Common;

namespace WHotelSite.Controllers
{
    public class ShopController : Controller
    {
        //
        // GET: /BusinessOperate/
        const string LogInURL = "/Shop/ShopLogin";
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ShopLogin(string autoLogin = "true")
        {
            if (autoLogin == "false")
            {
                AccountHelper.LoginOut();
            }
            if (AccountHelper.IsShopLogin())
            {
                return Redirect("/Shop/Coupon");
            }
            ViewBag.AutoLogin = autoLogin;
            return View();
        }


        public JsonResult Login(BusinessOperatorEntity param)
        {
            ExistsCodeResponse response = new ExistsCodeResponse();

            BusinessOperatorEntity result = new BusinessOperatorEntity();
            ShopLoginParam p = new ShopLoginParam();
            p.OperatorName = param.OperatorName;
            p.PassWord = param.PassWord;
            if (Shop.ExistsOperateName(param.OperatorName, 0) > 0)
            {
                result = Shop.ShopLogin(p);
                if (result != null)
                { 
                    AccountHelper.SetAuthorizeHead(result.ID, result.BizID, result.OperatorName,result.ShowName);
                    //FormsAuthentication.SetAuthCookie(param.OperatorName, true); 
                    return Json(new { Success = "1", id = result.ID, OperatorName = result.OperatorName, SupplierId = result.BizID, PassWord = result.PassWord });
                }
                return Json(new { Success = "0" });//密码错误
            }
            return Json(new { Success = "-1" });//账号不存在
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="type"></param>
        /// <param name="exno"></param>
        /// <returns></returns>
        public ActionResult Coupon(string startTime = "", string endTime = "", int type = 1, string exno = "")
        {
            if (!AccountHelper.IsShopLogin())
            {
                return Redirect(LogInURL);
            }
            DateTime endDate = System.DateTime.Now;
            DateTime startDate = endDate.AddDays(-7);
            List<UsedCouponProductEntity> usedCouponList = new List<UsedCouponProductEntity>();
            if (type == 1)
            {
                usedCouponList = Shop.GetUsedCouponProductByOperatorId(AccountHelper.OperatorID);
                ViewBag.SupplierName = Shop.GetSupplierById((long)AccountHelper.SupplierId).Name;
            }
            else if(type == 2)
            {
                if (endTime.Trim() != "")
                {
                    endDate = Convert.ToDateTime(Convert.ToDateTime(endTime).ToString("yyyy-MM-dd 23:59:59"));
                }
                if (startTime.Trim() != "")
                {
                    startDate = Convert.ToDateTime(Convert.ToDateTime(startTime).ToString("yyyy-MM-dd 00:00:00"));
                }
                UsedProductCouponEntity sumCoupon = Shop.GetSumCouponProductByOperatorId(AccountHelper.OperatorID, startDate, endDate,count:10000);
                ViewBag.SumCoupon = sumCoupon;
                //ViewBag.StartTime = startDate.ToString("yyyy-MM-dd");
                //ViewBag.EndTime = endDate.AddDays(-1).ToString("yyyy-MM-dd");

            }
            else if (type == 3)
            {
                startDate = System.DateTime.Now;
                endDate = DateTime.Now.AddDays(7);
                if (endTime.Trim() != "")
                {
                    endDate = Convert.ToDateTime(Convert.ToDateTime(endTime).ToString("yyyy-MM-dd 23:59:59"));
                }
                if (startTime.Trim() != "")
                {
                    startDate = Convert.ToDateTime(Convert.ToDateTime(startTime).ToString("yyyy-MM-dd 00:00:00"));
                }
                BookNoUsedExchangeCouponEntity bookNoUsedCoupon = Shop.GetBookNoUsedExchangeCouponBySupplierId(AccountHelper.SupplierId, startDate, endDate);
                ViewBag.BookNoUsedCoupon = bookNoUsedCoupon;
                //ViewBag.EndTime = endDate.ToString("yyyy-MM-dd");
            }
            ViewBag.EndTime = endDate.ToString("yyyy-MM-dd");
            ViewBag.StartTime = startDate.ToString("yyyy-MM-dd");
            ViewBag.Type = type;

            ViewBag.Exno = exno;
            ViewBag.OperatorID = AccountHelper.OperatorID;
            ViewBag.ShowName = AccountHelper.ShowName;

            return View(usedCouponList);
        }

        public JsonResult checkExchangeNo(string exchangeNo)
        {
            ExchangeCouponEntity exchangeCoupon = Shop.GetOneExchangeCouponInfo(0, exchangeNo);
            CouponProductEntity coupProduct = new CouponProductEntity();
            if (!AccountHelper.IsShopLogin())
            {
                coupProduct.State = 0;
                coupProduct.Msg = "未登录";
                return Json(coupProduct);
            }
            if (exchangeCoupon.SKUID == 0)
            {
                coupProduct.State = -1;
                coupProduct.Msg = "未找到消费券";
                return Json(coupProduct);
            }
            SKUInfoEntity skuInfo = Shop.GetSKUByID(exchangeCoupon.SKUID);
            UsedConsumerCouponInfoEntity usedCoupon = Shop.GetUsedCouponProductByExchangeNo(exchangeNo);
            if (skuInfo.SPU.SupplierID == AccountHelper.SupplierId)
            {
                coupProduct.Id = exchangeCoupon.ID;
                coupProduct.ExchangeNo = exchangeCoupon.ExchangeNo;
                coupProduct.SKUName = skuInfo.SKU.Name;
                coupProduct.SPUName = skuInfo.SPU.Name;
                coupProduct.SupplierId = skuInfo.SPU.SupplierID;
                coupProduct.State = exchangeCoupon.State;
                coupProduct.RefundState = exchangeCoupon.RefundState;
                coupProduct.Price = exchangeCoupon.Price;
                coupProduct.ConsumeTime = usedCoupon == null ? DateTime.Now : usedCoupon.CreateTime;
                coupProduct.PackageInfoList = string.IsNullOrWhiteSpace(exchangeCoupon.PackageInfo) ? new List<string>() : exchangeCoupon.PackageInfo.Trim().Split('\n').ToList();
                SupplierEntity supplierEntity = Shop.GetSupplierById(AccountHelper.SupplierId);
                coupProduct.SupplierName = supplierEntity.Name;
                coupProduct.CouponNote = exchangeCoupon.CouponNote;
                coupProduct.IsShowPrice = supplierEntity.IsShowPrice;
                coupProduct.OperationState = exchangeCoupon.OperationState;
                coupProduct.NeedPhoto = skuInfo.SKU.NeedPhoto;
                coupProduct.PhotoUrl = exchangeCoupon.PhotoUrl;
                coupProduct.UserID = exchangeCoupon.UserID;
                coupProduct.ExpireTime = (DateTime)exchangeCoupon.ExpireTime;
                
            }
            else
            {
                coupProduct.State = -1;
                coupProduct.Msg = "未找到消费券";
            }
            return Json(coupProduct);
        }

        public JsonResult UpdateCouponState(string exchangeNo)
        {
            if (string.IsNullOrWhiteSpace(exchangeNo))
            {
                return Json(-1);
            }
            else
            {
                if (!AccountHelper.IsShopLogin())
                {
                    return Json(0);
                }
                try
                {


                    ExchangeCouponEntity exchange = Shop.GetOneExchangeCouponInfo(0, exchangeNo);
                    SKUInfoEntity skuInfo = Shop.GetSKUByID(exchange.SKUID);
                    if (skuInfo.SKU.BookPosition != 0)
                    {
                        int bookCount = HJDAPI.APIProxy.Coupon.GetBookedUserInfoByExchangid(exchange.ID).Where(_ => _.State == 0).Count();
                        if (bookCount <= 0)
                        {
                            return Json(-3);
                        }
                    }


                    UsedConsumerCouponInfoEntity usedCoupon = Shop.GetUsedCouponProductByExchangeNo(exchangeNo);
                    if (usedCoupon == null || usedCoupon.ID == 0)
                    {
                        SPUEntity spuModel = HJDAPI.APIProxy.Coupon.GetSPUBySKUID(exchange.SKUID);

                        CouponActivityEntity ca = HJDAPI.APIProxy.Coupon.GetCouponActivityBySKUID(exchange.SKUID);
                        //周期卡处理：在有效期内核销了一张卡就发一张新卡，金额、结算都为0 ， promotionID = -1(用以标识生成的周期卡，不记在销售数量中)
                        if (spuModel.ProductCategoryID == 27 && ca.ExpireTime >= DateTime.Now)
                        {
                            HJDAPI.APIProxy.Coupon.CreateNewCouponForCycleCoupon(exchange.ID);
                        }

                        //更新ExchangeCoupone 状态
                        ExchangeCouponEntity exchangeCoupon = new ExchangeCouponEntity();
                        exchangeCoupon.ExchangeNo = exchangeNo;
                        exchangeCoupon.State = 3;//消费状态为3
                        Shop.UpdateExchangeState(exchangeCoupon);
                        //记录到消费券表
                        UsedConsumerCouponInfoEntity model = new UsedConsumerCouponInfoEntity();
                        model.ExchangeNo = exchangeNo;
                        model.OperatorId = AccountHelper.OperatorID;
                        model.SupplierId = AccountHelper.SupplierId;
                        model.CreateTime = DateTime.Now;
                        Shop.AddUsedConsumerCouponInfo(model);

                        LogHelper.WriteLog("消费券兑换操作人  OperatorID:" + AccountHelper.OperatorID + " SupplierId:" + exchangeNo + "消费券码：" + exchangeNo);
                        return Json(1);
                    }
                    else
                    {
                        LogHelper.WriteLog("消费券已核销：" + "消费券码：" + exchangeNo );
                        return Json(-2);
                    }
                }
                catch (Exception e)
                {
                    LogHelper.WriteLog("兑换房券更新出错UpdateCouponState：" + "消费券码：" + exchangeNo + "   异常信息" + e);
                    return Json(-1);
                }
            }
        }

        private static string GenintCheckCode(int Length)
        {
            char[] Pattern = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            string result = "";
            int n = Pattern.Length;
            System.Random random = new Random(~unchecked((int)DateTime.Now.Ticks));
            for (int i = 0; i < Length; i++)
            {
                int rnd = random.Next(0, n);
                result += Pattern[rnd];
            }
            return result;
        }
        public ActionResult CouponData(DateTime startTime, DateTime endTime)
        {

            UsedProductCouponEntity usedCoupon = Shop.GetSumCouponProductBySupplierId(AccountHelper.SupplierId, startTime, endTime);


            return View();
        }
        public string UEscape(string str)
        {
            StringBuilder s = new StringBuilder();
            int c = str.Length;
            int i = 0;
            while (i != c)
            {
                if (Uri.IsHexEncoding(str, i))
                {
                    s.Append(Uri.HexUnescape(str, ref i));
                }
                else
                {
                    s.Append(str[i++]);
                }
            }
            return s.ToString();

        }

        public FileResult ExceptUseCoupon(string startTime = "", string endTime = "")
        {
            if (!AccountHelper.IsShopLogin())
            {
                return null;
            }
            else
            {
                List<UsedCouponProductEntity> usedCouponList = Shop.GetUsedCouponProductByOperatorId(AccountHelper.OperatorID, startTime, endTime,0,1000000);
                string supplierName = Shop.GetSupplierById((long)AccountHelper.SupplierId).Name;
                //创建Excel文件的对象  
                NPOI.HSSF.UserModel.HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook();
                //添加一个sheet  
                NPOI.SS.UserModel.ISheet sheet1 = book.CreateSheet("Sheet1");

                //给sheet1添加第一行的头部标题  
                NPOI.SS.UserModel.IRow row1 = sheet1.CreateRow(0);
                row1.CreateCell(0).SetCellValue("产品名称");
                row1.CreateCell(1).SetCellValue("商户");
                row1.CreateCell(2).SetCellValue("交易时间");
                row1.CreateCell(3).SetCellValue("核销码");
                row1.CreateCell(4).SetCellValue("结算价");
                row1.CreateCell(5).SetCellValue("核销人");
                //将数据逐步写入sheet1各个行  
                for (int i = 0; i < usedCouponList.Count; i++)
                {
                    NPOI.SS.UserModel.IRow rowtemp = sheet1.CreateRow(i + 1);
                    rowtemp.CreateCell(0).SetCellValue(usedCouponList[i].SkuName.ToString());
                    rowtemp.CreateCell(1).SetCellValue(supplierName);
                    rowtemp.CreateCell(2).SetCellValue(usedCouponList[i].CreateTime.ToString("yyyy-MM-dd HH:mm:ss"));
                    rowtemp.CreateCell(3).SetCellValue(usedCouponList[i].ExchangeNo);
                    rowtemp.CreateCell(4).SetCellValue(usedCouponList[i].SettlePrice.ToString());
                    rowtemp.CreateCell(5).SetCellValue(usedCouponList[i].OperatorName);
                }
                // 写入到客户端   
                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                book.Write(ms); 
                ms.Seek(0, SeekOrigin.Begin);
                string fileName = "核销券码.xls";
                return File(ms, "application/vnd.ms-excel", fileName);
            }
        }


    }
}
