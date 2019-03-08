using HJD.HotelManagementCenter.Domain.QRCode;
using HJDAPI.Common.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.APIProxy
{
    public class QRCode : BaseProxy
    {
        /// <summary>
        /// 获取二维码信息
        /// </summary>
        /// <param name="SKUID"></param>
        /// <returns></returns>
        public QRCodeEntity GetQRCodeByID(int id)
        {
            string url = APISiteUrl + "api/QRCode/GetQRCodeInfoById";
            string postDataStr = "id=" + id;

            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<QRCodeEntity>(json);
        }
    }
}
