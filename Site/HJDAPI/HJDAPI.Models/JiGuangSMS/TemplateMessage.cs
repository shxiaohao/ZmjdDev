﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace HJDAPI.Models.JiGuangSMS
{
    public class TemplateMessage
    {

        /// <summary>
        /// 手机号码。
        /// </summary>
        [JsonProperty("mobile")]
        public string Mobile { get; set; }

        /// <summary>
        /// 模板 Id。
        /// </summary>
        [JsonProperty("temp_id")]
        public int? TemplateId { get; set; }

        /// <summary>
        /// 模板参数，需要替换的键值对。
        /// </summary>
        [JsonProperty("temp_para", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, string> TemplateParameters { get; set; }



        // 以下属性为创建、修改模板短信时所需。

        /// <summary>
        /// 模板内容。注意：根据运营商规定下发短信的内容不能超过 350 字符。
        /// <para>当类型为 1 （验证码）时，可用 {{code}} 作为验证码的占位符。</para>
        /// <para>例如：您好，您的验证码是{{code}}，5分钟内有效。</para>
        /// </summary>
        [JsonProperty("template", NullValueHandling = NullValueHandling.Ignore)]
        public string Content { get; set; }

        /// <summary>
        /// 模板类型。1 为验证码类，2 为通知类，3 为营销类。
        /// </summary>
        [JsonProperty("type")]
        public int Type { get; set; }

        /// <summary>
        /// 验证码有效期，单位为秒（当模板类型为 1 时必传）。
        /// </summary>
        [JsonProperty("ttl")]
        public int ValidDuration { get; set; }

        [JsonProperty("remark", NullValueHandling = NullValueHandling.Ignore)]
        public string Remark { get; set; }



        public override string ToString()
        {
            return GetJson();
        }

        private string GetJson()
        {
            return JsonConvert.SerializeObject(this, new JsonSerializerSettings
            {
                DefaultValueHandling = DefaultValueHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore
            });
        }

    }

    /// <summary>
    /// 创建短信模板返回结果
    /// </summary>
    public class CrateTemplateResult {

        public int temp_id { get; set; }

        public TemplateErrorInfo error { get; set; }





    }

    /// <summary>
    /// 模板错误信息
    /// </summary>

    public class TemplateErrorInfo {

        public string code { get; set; }

        public string message { get; set; }

    }



    /// <summary>
    /// 查询短信模板返回结果
    /// </summary>
    public class QueryTemplateResult {

        public int temp_id { get; set; }

        public string template { get; set; }


        public int type { get; set; }

        public int ttl { get; set; }

        public string remark { get; set; }

        public int status { get; set; }


        public TemplateErrorInfo error { get; set; }




    }





}
