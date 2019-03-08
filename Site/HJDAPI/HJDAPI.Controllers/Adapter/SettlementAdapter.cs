using HJD.Framework.WCF;
using HJD.HotelManagementCenter.Domain.Settlement;
using HJD.HotelManagementCenter.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.Controllers.Adapter
{
    public class SettlementAdapter
    {
        public static ISettlementService SettlementService = ServiceProxyFactory.Create<ISettlementService>("ISettlementService");



        /// <summary>
        /// 券取消时相应的供应商进行扣款操作
        /// </summary>
        /// <param name="CouponIDList"></param>
        /// <returns></returns>
        public static bool UpdateSettlementForSupplierProductOnCouponRefund(List<int> CouponIDList)
        {
            return SettlementService.UpdateSettlementForSupplierProductOnCouponRefund(CouponIDList);
        }



        public static bool AddInvoiceInfo( InvoiceEntity invoice)
        {
            SettlementService.AddInvoice(invoice);

            return true;
        }

        public static bool AddInvoiceAndOrderRel(InvoiceEntity invoice)
        {
            SettlementService.AddInvoiceAndOrderRel(invoice);

            return true;
        }


        public static int AddInvoiceAndOrderRel_BackInt(InvoiceEntity invoice)
        {
            return SettlementService.AddInvoiceAndOrderRel(invoice);
        }

        public static InvoiceEntity GetInvoiceByID(int id)
        {
            return SettlementService.GetInvoiceByID(id);
        }
        public static int UpdateInvoice(InvoiceEntity param)
        {
            return SettlementService.UpdateInvoice(param);
        }

        public static InvoiceEntity GetInvoiceEntityByUserId(long userId)
        {
            return SettlementService.GetInvoiceEntityByUserId(userId);
        }

    }
}
