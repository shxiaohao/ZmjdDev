
using HJD.Framework.WCF;
using HJDAPI.Controllers.Common;
using MagiCallService.Contracts;
using MagiCallService.Contracts.Model.Dialog;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.Tests.Controllers
{
    [TestClass]
    public  class MagiCallTest
    {
      //  public static IAccessService AccessService = ServiceProxyFactory.Create<IAccessService>("IAccessService");
        public static  IMagiCallService MagiCallService = ServiceProxyFactory.Create<MagiCallService.Contracts.IMagiCallService>("IMagiCallService");


        [TestMethod]
        public void GetChatLogsTest()
        {
            GetTime(1462430092801);
            string filePath = @"D:\LOG\magiCall\";
            string chatLogs = EMChatHelper.GetChatLogs(lastTimeStamp);

           HJDAPI.Models.MCChatMessage mc =  JsonConvert.DeserializeObject<HJDAPI.Models.MCChatMessage>(chatLogs);


           if (chatLogs.Length > 0)
           {
                MessageEntity entity = new MessageEntity();
               foreach (HJDAPI.Models.MCChatMessage.MsgEntity e in mc.Entities)
               {

                   if (e.To == "kevincai")
                   {
                       entity = new MessageEntity
                       {
                           messageType = MessageType.UserWord,
                           UserID = long.Parse(e.From.Replace("zmjdappuser", ""))   //"from" : "zmjdappuser4522272",
                       };
                   }
                   else
                   {
                       entity = new MessageEntity
                       {
                           messageType = MessageType.KeFuWord,
                           UserID = long.Parse(e.To.Replace("zmjdappuser", ""))   //"from" : "zmjdappuser4522272",
                       };
                   }

                   entity.msg = e.Payload.Bodies.First().Msg;
                   entity.CreateTime = GetTime(e.Created);
             //      AccessService.MagiCallClientMessage(entity);
               }


              long theLastTimeStamp = mc.Entities.Last().Timestamp;
              if (theLastTimeStamp > lastTimeStamp)
              {
                  File.WriteAllText(filePath + "log" + theLastTimeStamp.ToString() + ".txt", chatLogs);

                  lastTimeStamp = theLastTimeStamp;
              }

            
           }
        }

        static long mLastTimeStamp = 0;
        static string lastTimeStampFilePath = @"D:\Log\MagiCall\lastTimeStamp.txt";
        static long lastTimeStamp
        {
            get
            {
                if(mLastTimeStamp == 0)
                {
                    if(File.Exists(lastTimeStampFilePath ) )
                    {
                        mLastTimeStamp = long.Parse(File.ReadAllText(lastTimeStampFilePath));
                    }
                }
                return mLastTimeStamp;

            }
            set
            {
                mLastTimeStamp = value;
                File.WriteAllText(lastTimeStampFilePath, value.ToString());
            }
        }


        /// <summary>  
        /// 时间戳转为C#格式时间  
        /// </summary>  
        /// <param name="timeStamp">Unix时间戳格式</param>  
        /// <returns>C#格式时间</returns>  
        public static DateTime GetTime(long timeStamp)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));

            return dtStart.AddMilliseconds(timeStamp);
            //long lTime = timeStamp * 10000000;
            //TimeSpan toNow = new TimeSpan(lTime);
            //return dtStart.Add(toNow);
        }  


    }
}
