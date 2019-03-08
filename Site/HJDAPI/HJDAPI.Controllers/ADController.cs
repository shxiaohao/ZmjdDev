using System;
using System.Net;
using System.Web.Http;
using HJD.AccountServices.Contracts;
using HJD.AccountServices.Entity;
using HJD.Framework.WCF;
using HJDAPI.Common.Helpers;
using HJDAPI.Models;
using Newtonsoft.Json;
using HJD.HotelManagementCenter.Domain.Comm;
using HJD.HotelManagementCenter.IServices;
using HJDAPI.Common.Security;
using HJD.ADServices.Contract;
using System.Collections.Generic;
using HJDAPI.Controllers.Adapter;

namespace HJDAPI.Controllers
{
    public class ADController:BaseApiController
    {
        static IADService ADService = ServiceProxyFactory.Create<IADService>("IADService");

        [HttpGet]
         public  bool TempSendActiveRebate(long orderid, int activeRebateAmount, long CID =0 )
        {
            return OrderAdapter.TempSendActiveRebate(orderid, activeRebateAmount, CID);

        }

        [HttpGet]
        public List<ActiveUserCodeEntity> GetActiveCode(long userID, int activeID)
        {
            return ADAdapter.GetActiveCode(userID,  activeID);
        }

        [HttpPost]
        public bool AddActiveUserCodeRel(ActiveUserCodeRelEntity activeusercoderel)
        {
           return   ADAdapter.AddActiveUserCodeRel(activeusercoderel);
        }

      
    }
}