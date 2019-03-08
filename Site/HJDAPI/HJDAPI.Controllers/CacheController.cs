using System;
using System.Linq;
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
using System.Collections.Generic;
using HJDAPI.Controllers.Adapter;
using System.IO;
using HJD.Framework.Interface;
using HJD.CouponService.Contracts.Entity;

namespace HJDAPI.Controllers
{
    public class CacheController : BaseApiController
    {
        static ICommService commService = ServiceProxyFactory.Create<ICommService>("ICommService");


        public static ICacheProvider LocalCache10Min = CacheManagerFactory.Create("DynamicCacheFor10Min");

        [HttpGet]
        public bool ClearCommDicKeyValueCache(int typeid, int key)
        {
            CommAdapter.ClearCommDicKeyValueCache(typeid, key);
            return true;
        }
    }
}