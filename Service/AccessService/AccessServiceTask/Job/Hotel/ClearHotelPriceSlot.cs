using HJD.AccessService.Contract;
using HJD.AccessService.Contract.Model;
using HJD.AccessService.Contract.Model.Hotel;
using HJD.AccessService.Implement;
using HJD.AccessService.Implement.Helper;
using HJD.AccessServiceTask.Entity;
using HJD.AccessServiceTask.Helper;
using HJD.AccessServiceTask.Job.Helper;
using HJD.HotelServices.Contracts;
using HJD.Search.CommonLibrary;
using HJD.Search.CommonLibrary.Engine;
using HJD.Search.CommonLibrary.Model;
using HJDAPI.APIProxy;
using HJDAPI.Models;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace HJD.AccessServiceTask.Job.Search
{
    /// <summary>
    /// 
    /// </summary>
    public class ClearHotelPriceSlot : BaseJob
    {
        //public static string HotelDbConn = "Data Source=192.168.1.113;Initial Catalog=HotelDB;Persist Security Info=True;User ID=sa;Password=password01!";
        public static string HotelDbConn = "Server=rdss9am4323erjo9qjsx.sqlserver.rds.aliyuncs.com,3433;UID=appuser;password=C1g8oC__JcW_C1g8oC__JcW_;database=HotelDB;";

        public ClearHotelPriceSlot()
            : base("ClearHotelPriceSlot")
        {
            Log(string.Format("Start Job [ClearHotelPriceSlot]"));

            try
            {
                RunJob();
            }
            catch (Exception ex)
            {
                Log("Job [ClearHotelPriceSlot] Error:" + ex.Message);
            }

            Log(string.Format("Stop Job [ClearHotelPriceSlot]"));
        }

        private void RunJob()
        {
            DBHelper.HotelDbConn = HotelDbConn;

            var deleteDate = DateTime.Now.Date;

            Log("Delete Date:" + deleteDate);

            //一次1w条，删除50次
            for (int i = 0; i < 50; i++)
            {
                var del = ClearExpireHotelPriceSlot(deleteDate);
                Log(string.Format("Delete {0}: {1}", i, del));
            }

            Log("Delete End");
        }

        private int ClearExpireHotelPriceSlot(DateTime date)
        {
            var sql = string.Format(@"
delete HotelDB.dbo.HotelPriceSlot where id in
(select top 10000 id from HotelDB.dbo.HotelPriceSlot where night < '{0}')
", date.ToString("yyyy-MM-dd"));

            return DBHelper.ExecuteNonQuery(HotelDbConn, System.Data.CommandType.Text, sql);
        }
    }
}
