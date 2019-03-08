using HJD.AccountServices.Entity;
using HJD.CouponService.Contracts.Entity;
using HJD.HotelManagementCenter.Domain;
using HJD.HotelManagementCenter.Domain.Fund;
using HJD.HotelManagementCenter.Domain.QRCode;
using HJD.HotelPrice.Contract.DataContract.Order;
using HJD.HotelServices.Contracts;
using HJDAPI.APIProxy;
using HJDAPI.Common.Security;
using HJDAPI.Models;
using Newtonsoft.Json;
using ProductService.Contracts.Entity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WHotelSite.Common;
using WHotelSite.Models;

namespace WHotelSite.Controllers
{
    /// <summary>
    /// 二维码处理
    /// </summary>
    public class QrcodeController : BaseController
    {
        /// <summary>
        /// 二维码识别入口
        /// </summary>
        /// <param name="qid"></param>
        /// <returns></returns>
        public ActionResult Entrance(int qid = 0)
        {
            var isApp = IsApp();
            ViewBag.IsApp = isApp;

            //isMobile
            var isMobile = Utils.IsMobile();
            ViewBag.IsMobile = isMobile;

            //Weixin
            bool isInWeixin = Request.UserAgent.IndexOf("MicroMessenger") > 0 ? true : false;
            ViewBag.isInWeixin = isInWeixin;

            var curUserID = Convert.ToInt64(UserState.UserID > 0 ? UserState.UserID : 0);
            if (curUserID <= 0)
            {
                curUserID = _ContextBasicInfo.AppUserID;
            }
            if (curUserID >= 0 && _ContextBasicInfo.AppUserID <= 0)
            {
                _ContextBasicInfo.AppUserID = curUserID;
            }
            ViewBag.UserId = curUserID;

            ViewBag.QID = qid;

            //new Qrcode
            QRCodeEntity qrcodeInfo = new QRCode().GetQRCodeByID(qid);
            ViewBag.QRCodeInfo = qrcodeInfo;

            if (qrcodeInfo != null && qrcodeInfo.ExpiryTime > DateTime.Now && !string.IsNullOrEmpty(qrcodeInfo.BusinessUrl))
            {
                return Redirect(qrcodeInfo.BusinessUrl);
            }

            return View(); 
        }
    }
}