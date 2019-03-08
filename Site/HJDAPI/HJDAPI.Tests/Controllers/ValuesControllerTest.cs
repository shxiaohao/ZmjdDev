using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HJDAPI;
using HJDAPI.Controllers;
using HJDAPI.Common.Security;

namespace HJDAPI.Tests.Controllers
{
    [TestClass]
    public class ValuesControllerTest
    {

        [TestMethod]
        public void TestDES()
        {
            string toSign = "CID=150&CUserID=12312312&TimeStamp=100000";
            string sign = "wlNwGjglynUOdOayE7GMmeL8aRaHYTJL5NAAZ7ZyFp+eu9R4Ow8lb8gaX3Ag1zV3";
            string deSign = DES.DecryptString("EOTYF68H5DYQW69A3F2DEI54WBCJSJ1Y", sign);// DES.bohuijinrongDESKey);
            Assert.AreEqual(toSign, deSign);
        }

        [TestMethod]
        public void Get()
        {
            // 排列
            ValuesController controller = new ValuesController();

            // 操作
            IEnumerable<string> result = controller.Get();

            // 断言
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
            Assert.AreEqual("value1", result.ElementAt(0));
            Assert.AreEqual("value2", result.ElementAt(1));
        }

        [TestMethod]
        public void GetById()
        {
            // 排列
            ValuesController controller = new ValuesController();

            // 操作
            string result = controller.Get(5);

            // 断言
            Assert.AreEqual("value", result);
        }

        [TestMethod]
        public void Post()
        {
            // 排列
            ValuesController controller = new ValuesController();

            // 操作
            controller.Post("value");

            // 断言
        }

        [TestMethod]
        public void Put()
        {
            // 排列
            ValuesController controller = new ValuesController();

            // 操作
            controller.Put(5, "value");

            // 断言
        }

        [TestMethod]
        public void Delete()
        {
            // 排列
            ValuesController controller = new ValuesController();

            // 操作
            controller.Delete(5);

            // 断言
        }

        [TestMethod]
        [HttpPost]
        public void YAXsx(string param)
        {
            //string moid = "79a11e15-5363-4a0f-b3f0-46240bd3cea6";
            //string mobile = "13838838438";
            //string content = "短信上行1";
            //string sign = "云之讯";
            //string extend = "00";
            //string reply_time = "2016-04-02 17:52:15";

            //string data = "{\"moid\":\"" + moid + "\""
            //            + ",\"mobile\":\"" + mobile + "\""
            //            + ",\"content\":\"" + content + "\""
            //            + ",\"sign\":\"" + sign + "\""
            //            + ",\"extend\":\"" + extend + "\""
            //            + ",\"reply_time\":\"" + reply_time + "\""
            //            + "}";

            //SMServiceController.YZXSMSsx(param);


        }



    }
}
