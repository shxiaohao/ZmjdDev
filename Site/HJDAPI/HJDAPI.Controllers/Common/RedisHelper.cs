using HJD.Framework.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.Controllers.Common
{
    public class RedisHelper
    {
        /// <summary>
        /// redis连接
        /// </summary>
        public static readonly IRedisProvider RedisConn = RedisManagerFactory.Create("redisConn");

        /// <summary>
        /// 每次缓存数据条数 每个用户订单信息一次缓存的数量
        /// </summary>
        public static int CacheCount=100;

        
    }
}
