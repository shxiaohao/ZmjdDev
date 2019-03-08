using HJD.AccessService.Contract.Model;
using HJD.AccessService.Contract.Params;
using HJD.EmailServices.Contracts;
using HJD.Framework.WCF;
using HJD.HotelManagementCenter.IServices;
using HJD.HotelServices.Contracts;
using HJD.WeixinServices.Contract;
using HJD.WeixinServices.Contracts;
using HJDAPI.Common.Helpers;
using HJDAPI.Controllers.Activity;
using HJDAPI.Controllers.Adapter;
using HJDAPI.Controllers.Common;
using HJDAPI.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Web.Security;
using System.Xml;

namespace HJDAPI.Controllers
{
    public class MailController : BaseApiController
    {

        public static ICommService commService = ServiceProxyFactory.Create<ICommService>("ICommService");
        public static IEmailService emailService = ServiceProxyFactory.Create<IEmailService>("BasicHttpBinding_IEmailService");

        private string logFile = Configs.LogPath + string.Format("EmailLog_{0}{1}{2}.txt", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

        [System.Web.Http.AcceptVerbs("Get")]
        [System.Web.Http.HttpGet]
        public int UpdateEdmUsersState(long idx, int state)
        {
            return emailService.UpdateEdmUsersState(idx, state);
        }
    }
}
