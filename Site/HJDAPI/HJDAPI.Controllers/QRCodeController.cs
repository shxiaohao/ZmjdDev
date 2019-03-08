using HJD.Framework.WCF;
using HJD.HotelManagementCenter.Domain.QRCode;
using HJD.HotelManagementCenter.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace HJDAPI.Controllers
{
    public class QRCodeController : BaseApiController
    {
        private static IQRCodeService qrcodeservice = ServiceProxyFactory.Create<IQRCodeService>("IQRCodeService");

        /// <summary>
        /// 根据id 获取二维码信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public QRCodeEntity GetQRCodeInfoById(int id)
        {
            return qrcodeservice.GetQRCodeByID(id);
        }
    }
}
