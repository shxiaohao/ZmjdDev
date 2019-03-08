using HJD.Framework.WCF;
using HJD.HotelManagementCenter.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.Controllers.Adapter
{
    public class SupplierAdapter
    {
        public static HJD.HotelManagementCenter.IServices.ISupplierService SupplierService = ServiceProxyFactory.Create<HJD.HotelManagementCenter.IServices.ISupplierService>("HMC_ISupplierService");


        public static SupplierEntity GetSupplierById(int supplierId)
        {
            return SupplierService.GetSupplierById(supplierId);
        }

        public static BusinessOperatorEntity GetBusinessOperatorByID(int id)
        {
            return SupplierService.GetBusinessOperatorByID(id);
        }
    }
}
