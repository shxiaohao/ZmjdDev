using HJD.Framework.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.Controllers.Common
{
    public class MemCacheHelper
    {
        public static IMemcacheProvider memcached30min = MemcacheManagerFactory.Create("memcached30min");
        public static IMemcacheProvider memcached5min = MemcacheManagerFactory.Create("memcached5min");
        public static IMemcacheProvider memcached10min = MemcacheManagerFactory.Create("memcached10min");

        public class Prefix
        {
            /// <summary>
            /// 用户订单列表订单实体缓存前缀
            /// </summary>
            public const string UserDetailOrderCacheKey = "UserDetailOrder";
        }
    }
}
