using HJD.AccessService.Contract;
using HJD.AccessService.Contract.Model;
using HJD.AccessService.Implement;
using HJD.AccessService.Implement.Helper;
using HJD.AccessServiceTask.Entity;
using HJD.AccessServiceTask.Job.Helper;
using HJD.Search.CommonLibrary;
using HJD.Search.CommonLibrary.Engine;
using HJD.Search.CommonLibrary.Model;
using HJDAPI.APIProxy;
using HtmlAgilityPack;
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
    /// 生成酒店相关索引库
    /// </summary>
    public class BuildHotelIndexJob : BaseJob
    {
        public BuildHotelIndexJob()
            : base("BuildHotelIndexJob")
        {
            Log(string.Format("Start Job [BuildHotelIndexJob]"));

            try
            {
                RunJob();
            }
            catch (Exception ex)
            {
                Log("Job [BuildHotelIndexJob] Error:" + ex.Message);
            }


            Log(string.Format("Stop Job [BuildHotelIndexJob]"));

            //Console.ReadKey();
        }

        private void RunJob()
        {
            //demo
            //BuildDemo();
            
            //hotel
            BuildHotel();
        }

        #region hotel

        private void BuildHotel()
        {
            var indexEngine = new IndexEngine(SearchType.Hotel);

            var writeDatas = GetHotelDatas();
            if (writeDatas == null)
            {
                Log("Doc Is Null");
                return;
            }

            Log("Doc Count:" + writeDatas.Count());

            var writeResult = indexEngine.WriteIndex(writeDatas);
            foreach (var item in writeResult)
            {
                Console.WriteLine(item);
            }
            Console.WriteLine("over");
            //Console.ReadKey();
        }

        private IEnumerable<DocumentData> GetHotelDatas()
        {
            var docDataList = IndexEngine.GetIndexDocsById(SearchType.Hotel, null);
            //var docDataList = IndexEngine.GetIndexDocsById(SearchType.Hotel, "45574,480425,242061");

            return docDataList;
        }

        #endregion

        #region demo

        private void BuildDemo()
        {

            //var indexEngine = new IndexEngine(SearchType.Demo);

            //var writeDatas = GetDemoDatas();

            //var writeResult = indexEngine.WriteIndex(writeDatas);

            //foreach (var item in writeResult)
            //{
            //    Console.WriteLine(item);
            //}
            //Console.WriteLine("over");
            //Console.ReadKey();
        }

        private IEnumerable<FieldData[]> GetDemoDatas()
        {
            var conn = "Data Source=;Initial Catalog=LuceneDB;Persist Security Info=True;User ID=sa;Password=sa";

            using (var reader = SqlHelper.ExecuteReader(conn, System.Data.CommandType.Text, "select id,title,datacontent from DataObject"))
            {
                while (reader.Read())
                {
                    var objs = new List<FieldData>();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        objs.Add(new FieldData()
                        {
                            Column = reader.GetName(i),
                            DataType = reader.GetName(i).ToLower() == "id" ? DataTypes.Int : DataTypes.String,
                            IsSort = false,
                            IsStore = reader.GetName(i).ToLower() == "id" ? true : false,
                            Value = reader[i].ToString(),
                            Weight = reader.GetName(i).ToLower() == "title" ? 30 : 1
                        });
                    }
                    yield return objs.ToArray();
                }
            }
        }

        #endregion
    }
}
