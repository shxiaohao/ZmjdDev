using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.Models
{
    public class ResponseBase
    {
        public ResponseBase(ResponseBaseCode code,string apiName)
        {
            Code = code;
            Msg = code.ToString();
            APIName = apiName;
        }

        /// <summary>
        /// 返回状态码
        /// </summary>
        public  ResponseBaseCode Code { get; set; }

        /// <summary>
        /// 返回消息
        /// </summary>
        public string Msg { get; set; }



        /// <summary>
        /// 接口名
        /// </summary>
        public string APIName { get; set; }

        /// <summary>
        /// 接口签名
        /// </summary>
        public string Sign { get; set; }

         //业务返回数据
        public object Data { get; set; }

    }

    public enum ResponseBaseCode
    {
        success = 0,
        noAppFunctionID = 1
    }
}
