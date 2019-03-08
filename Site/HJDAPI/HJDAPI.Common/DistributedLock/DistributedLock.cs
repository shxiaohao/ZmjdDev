using HJD.Framework.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HJDAPI.Common.DistributedLock
{
    public class DistributedLock
    {
        /// <summary>
        /// 获取锁应该满足三点
        /// 1.同一时刻只有一个线程可以获得锁
        /// 2.获得锁的线程没有自动释放（删除相应key的缓存），其他线程有权在得知约定的锁过期后占有锁
        /// 3.如果一个锁不断尝试 无法获得锁 设置一个过期时间 这个时间段内没拿到锁则退出争夺
        /// </summary>
        /// <param name="keyName"></param>
        /// <param name="maxHoldTimeSpan"></param>
        /// <param name="maxTryTimeSpan"></param>
        /// </summary>
        /// <param name="keyName"></param>
        /// <param name="maxHoldTimeSpan"></param>
        /// <param name="maxTryTimeSpan"></param>
        /// <param name="retryFrequency">重试的频率时间 默认10 单位毫秒</param>
        /// <returns>bool值 是否获得锁</returns>
        public static bool AcquireLock(AcquireLockParam param,IMemcacheProvider memcacheProvider)
        {
            if (string.IsNullOrWhiteSpace(param.keyName) || param.maxHoldTimeSpan == null || param.maxTryTimeSpan == null)
            {
                return false;
            }

            DateTime baseDateTime = new DateTime(1970, 1, 1, 0, 0, 0);
            DateTime currentTime = DateTime.Now.ToUniversalTime();
            DateTime autoQuitTime = currentTime + param.maxTryTimeSpan;//到达此时刻且没有获得锁则返回false

            //ulong quitFlag = (ulong)Math.Ceiling((autoQuitTime - baseDateTime).TotalMilliseconds);

            IMemcacheProvider memcacheProvider2 = memcacheProvider;//MemcacheManagerFactory.Create("Type1");
            ulong unique = 0;
            do
            {
                bool isAcquired = false;
                ulong oldData = Convert.ToUInt64(memcacheProvider2.Gets(param.keyName, out unique));

                currentTime = DateTime.Now.ToUniversalTime();
                DateTime latestHoldTime = currentTime + param.maxHoldTimeSpan;

                ulong holdFlag = (ulong)Math.Ceiling((latestHoldTime - baseDateTime).TotalMilliseconds);

                if (oldData == 0)
                {
                    isAcquired = memcacheProvider2.AddNX(param.keyName, holdFlag);
                }
                else if ((ulong)Math.Ceiling((currentTime - baseDateTime).TotalMilliseconds) >= oldData)
                {
                    isAcquired = memcacheProvider2.CAS(param.keyName, holdFlag, unique);//持有锁的线程 出了意外 到时间没释放锁 当前线程有权去争取锁
                }

                if (isAcquired)
                {
                    return true;
                }

                Thread.Sleep(param.retryFrequency);
            }
            while ((autoQuitTime - DateTime.Now.ToUniversalTime()).TotalMilliseconds > 0);
            return false;
             
        }

        /// <summary>
        /// 释放锁
        /// </summary>
        /// <param name="keyName"></param>
        /// <returns></returns>
        public static bool ReleaseLock(string keyName, IMemcacheProvider memcacheProvider)
        {
            if (string.IsNullOrWhiteSpace(keyName))
            {
                return false;
            }
            return memcacheProvider.Delete(keyName);
        }
    }

    public class AcquireLockParam
    {
        public int retryFrequency { get; set; }

        public string keyName { get; set; }

        public TimeSpan maxHoldTimeSpan { get; set; }

        public TimeSpan maxTryTimeSpan { get; set; }
    }
}
