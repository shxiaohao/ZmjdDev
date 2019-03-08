using HJD.Framework.Interface;
using Microsoft.Practices.EnterpriseLibrary.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.Controllers.Adapter
{
    public class CacheAdapter
    {
        public static ICacheProvider LocalCache10Min = HJD.Framework.Interface.CacheManagerFactory.Create("DynamicCacheFor10Min");//抢券活动缓存 十分钟

        public static readonly ICacheProvider LocalCache30Min = HJD.Framework.Interface.CacheManagerFactory.Create("DynamicCacheForADV"); 
        public static readonly ICacheProvider LocalCache5Min = HJD.Framework.Interface.CacheManagerFactory.Create("DynamicCacheFor5Min");
    }
}
