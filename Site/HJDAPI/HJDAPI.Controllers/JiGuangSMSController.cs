using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using HJDAPI.Models.JiGuangSMS;
using System.Web.Http;

namespace HJDAPI.Controllers
{
   public  class JiGuangSMSController : BaseApiController
    {

        public HttpClient httpClient;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="appKey"></param>
        /// <param name="masterSecret"></param>
        public JiGuangSMSController()
        {
           string appKey= "cf960a3c8f1b530a5feaac11";

           string masterSecret = "12bfd691be0f29b461e4dc10";


            if (string.IsNullOrEmpty(appKey))
                throw new ArgumentNullException(appKey);

            if (string.IsNullOrEmpty(masterSecret))
                throw new ArgumentNullException(masterSecret);

            httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://api.sms.jpush.cn/v1/")
            };
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var auth = Convert.ToBase64String(Encoding.UTF8.GetBytes(appKey + ":" + masterSecret));
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", auth);
        }


        #region 发送文本验证码

        /// <summary>
        /// 发送文本验证码短信
        /// </summary>
        public async Task<HttpResponse> SendCodeAsync(string mobile, int tempId)
        {
            if (string.IsNullOrEmpty(mobile))
                throw new ArgumentNullException(mobile);

            JObject json = new JObject
            {
                { "mobile", mobile },
                { "temp_id", tempId }
            };
            HttpContent httpContent = new StringContent(json.ToString(), Encoding.UTF8);
            HttpResponseMessage httpResponseMessage = await httpClient.PostAsync("codes", httpContent).ConfigureAwait(false);
            string httpResponseContent = await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
            return new HttpResponse(httpResponseMessage.StatusCode, httpResponseMessage.Headers, httpResponseContent);
        }

        /// <summary>
        /// 发送文本验证码短信。
        /// <see cref="https://docs.jiguang.cn/jsms/server/rest_api_jsms/#api_1"/>
        /// </summary>
        /// <param name="mobile">手机号码</param>
        /// <param name="tempId">模板 Id</param>
        public HttpResponse SendCode(string mobile, int tempId)
        {
            Task<HttpResponse> task = Task.Run(() => SendCodeAsync(mobile, tempId));
            task.Wait();
            return task.Result;
        }

        #endregion


        #region 发送语音验证码

        /// <summary>
        /// 发送语音验证码。
        /// </summary>
        /// <param name="jsonBody">消息体。</param>
        public async Task<HttpResponse> SendVoiceCodeAsync(string jsonBody)
        {
            if (string.IsNullOrEmpty(jsonBody))
                throw new ArgumentNullException(jsonBody.ToString());

            HttpContent httpContent = new StringContent(jsonBody, Encoding.UTF8);
            HttpResponseMessage httpResponseMessage = await httpClient.PostAsync("voice_codes", httpContent).ConfigureAwait(false);
            string httpResponseContent = await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
            return new HttpResponse(httpResponseMessage.StatusCode, httpResponseMessage.Headers, httpResponseContent);
        }

        /// <summary>
        /// <see cref="SendVoiceCode(VoiceCode)"/>
        /// </summary>
        public async Task<HttpResponse> SendVoiceCodeAsync(VoiceCode voiceCode)
        {
            if (voiceCode == null)
                throw new ArgumentNullException(voiceCode.ToString());

            string body = voiceCode.ToString();
            return await SendVoiceCodeAsync(body).ConfigureAwait(false);
        }

        /// <summary>
        /// 发送语音验证码。
        /// <see cref="https://docs.jiguang.cn/jsms/server/rest_api_jsms/#api_2"/>
        /// </summary>
        /// <param name="voiceCode">语音验证码对象。</param>
        public HttpResponse SendVoiceCode(VoiceCode voiceCode)
        {
            Task<HttpResponse> task = Task.Run(() => SendVoiceCodeAsync(voiceCode));
            task.Wait();
            return task.Result;
        }
        #endregion


        #region 判断验证码

        /// <summary>
        ///判断验证码是否有效。
        /// </summary>
        public async Task<HttpResponse> IsCodeValidAsync(string msgId, string code)
        {
            if (string.IsNullOrEmpty(msgId))
                throw new ArgumentNullException(msgId);

            if (string.IsNullOrEmpty(code))
                throw new ArgumentNullException(code);

            //var url = $"codes/{msgId}/valid";
            var url = string.Format("codes/{0}/valid", msgId);

            JObject json = new JObject
            {
                { "code", code }
            };
            HttpContent httpContent = new StringContent(json.ToString(), Encoding.UTF8);

            HttpResponseMessage httpResponseMessage = await httpClient.PostAsync(url, httpContent).ConfigureAwait(false);
            string httpResponseContent = await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
            return new HttpResponse(httpResponseMessage.StatusCode, httpResponseMessage.Headers, httpResponseContent);
        }

        /// <summary>
        /// 判断验证码是否有效。
        /// <see cref="https://docs.jiguang.cn/jsms/server/rest_api_jsms/#api_3"/>
        /// </summary>
        /// <param name="msgId">调用发送验证码短信 API 的返回值。</param>
        /// <param name="code">验证码。</param>
        public HttpResponse IsCodeValid(string msgId, string code)
        {
            Task<HttpResponse> task = Task.Run(() => IsCodeValidAsync(msgId, code));
            task.Wait();
            return task.Result;
        }


        #endregion



        #region 发送单条模板短信

        [HttpPost]
        public TemplateSendResult SendTemplateMessageAsync(TemplateMessage message)
        {

            HttpResponse response = SendTemplateMessage(message);

            TemplateSendResult result = Newtonsoft.Json.JsonConvert.DeserializeObject<TemplateSendResult>(response.Content);

            return result;
            
        }


        public async Task<HttpResponse> SendTemplateMessageAsyncTask(TemplateMessage message)
        {
            if (message == null)
                throw new ArgumentNullException(message.ToString());

            HttpContent httpContent = new StringContent(message.ToString(), Encoding.UTF8);
            HttpResponseMessage httpResponseMessage = await httpClient.PostAsync("messages", httpContent).ConfigureAwait(false);
            string httpResponseContent = await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
            return new HttpResponse(httpResponseMessage.StatusCode, httpResponseMessage.Headers, httpResponseContent);
        }

        /// <summary>
        /// 发送单条模板短信。
        /// <see cref="https://docs.jiguang.cn/jsms/server/rest_api_jsms/#api_4"/>
        /// </summary>
        /// <param name="message">模板短信对象。</param>
        public HttpResponse SendTemplateMessage(TemplateMessage message)
        {
            Task<HttpResponse> task = Task.Run(() => SendTemplateMessageAsyncTask(message));
            task.Wait();
            return task.Result;
        }

        #endregion



        #region 批量发送模板短信
        /// <summary>
        /// <see cref="SendTemplateMessage(List{TemplateMessage})"/>
        /// </summary>
        public async Task<HttpResponse> SendTemplateMessageListAsync(List<TemplateMessage> templateMessageList)
        {
            if (templateMessageList == null || templateMessageList.Count == 0)
                throw new ArgumentException(templateMessageList.ToString());

            int? tempId = templateMessageList[0].TemplateId;

            JArray recipients = new JArray();
            foreach (TemplateMessage msg in templateMessageList)
            {
                JObject item = new JObject
                {
                    { "mobile", msg.Mobile }
                };

                if (msg.TemplateParameters != null && msg.TemplateParameters.Count != 0)
                {
                    item.Add("temp_para", JObject.FromObject(msg.TemplateParameters));
                }

                recipients.Add(item);
            }

            JObject json = new JObject
            {
                { "temp_id", tempId },
                { "recipients", recipients }
            };

            HttpContent httpContent = new StringContent(json.ToString(), Encoding.UTF8);
            HttpResponseMessage httpResponseMessage = await httpClient.PostAsync("messages/batch", httpContent).ConfigureAwait(false);
            string httpResponseContent = await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
            return new HttpResponse(httpResponseMessage.StatusCode, httpResponseMessage.Headers, httpResponseContent);
        }

        /// <summary>
        /// 批量发送模板短信。模板 Id 必须一致。
        /// <see cref="https://docs.jiguang.cn/jsms/server/rest_api_jsms/#api_5"/>
        /// </summary>
        /// <param name="templateMessageList">模板短信对象列表。</param>
        public HttpResponse SendTemplateMessage(List<TemplateMessage> templateMessageList)
        {
            Task<HttpResponse> task = Task.Run(() => SendTemplateMessageListAsync(templateMessageList));
            task.Wait();
            return task.Result;
        }

        #endregion



        #region 创建短信模板

        /// <summary>
        /// <seealso cref="CreateMessageTemplate(TemplateMessage)"/>
        /// </summary>
        public async Task<HttpResponse> CreateMessageTemplateAsync(TemplateMessage template)
        {
            HttpContent httpContent = new StringContent(template.ToString(), Encoding.UTF8);
            HttpResponseMessage httpResponseMessage = await httpClient.PostAsync("templates", httpContent).ConfigureAwait(false);
            string httpResponseContent = await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
            return new HttpResponse(httpResponseMessage.StatusCode, httpResponseMessage.Headers, httpResponseContent);
        }

        /// <summary>
        /// 创建短信模板。
        /// <para><see cref="https://docs.jiguang.cn/jsms/server/rest_api_jsms_templates/#api_1"/></para>
        /// </summary>
        /// <param name="template">短信模板对象。</param>
        public HttpResponse CreateMessageTemplate(TemplateMessage template)
        {
            Task<HttpResponse> task = Task.Run(() => CreateMessageTemplateAsync(template));
            task.Wait();
            return task.Result;
        }


        #endregion


        #region 修改短信模板
        /// <summary>
        /// <seealso cref="UpdateMessageTemplate(TemplateMessage)"/>
        /// </summary>
        public async Task<HttpResponse> UpdateMessageTemplateAsync(TemplateMessage template)
        {
            if (template.TemplateId == null)
                throw new ArgumentNullException(template.TemplateId.ToString());

            HttpContent httpContent = new StringContent(template.ToString(), Encoding.UTF8);
            HttpResponseMessage httpResponseMessage = await httpClient.PutAsync(string.Format("templates/{0}", template.TemplateId), httpContent).ConfigureAwait(false);
            string httpResponseContent = await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
            return new HttpResponse(httpResponseMessage.StatusCode, httpResponseMessage.Headers, httpResponseContent);
        }

        /// <summary>
        /// 更新短信模板。
        /// <para><see cref="https://docs.jiguang.cn/jsms/server/rest_api_jsms_templates/#api_2"/></para>
        /// </summary>
        /// <param name="template">短信模板对象。</param>
        public HttpResponse UpdateMessageTemplate(TemplateMessage template)
        {
            Task<HttpResponse> task = Task.Run(() => UpdateMessageTemplateAsync(template));
            task.Wait();
            return task.Result;
        }

        #endregion



        #region 查询短信模板

        /// <summary>
        /// <seealso cref="QueryMessageTemplate(int)"/>
        /// </summary>
        public async Task<HttpResponse> QueryMessageTemplateAsync(int tempId)
        {
            HttpResponseMessage httpResponseMessage = await httpClient.GetAsync(string.Format("templates/{0}", tempId)).ConfigureAwait(false);
            string httpResponseContent = await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
            return new HttpResponse(httpResponseMessage.StatusCode, httpResponseMessage.Headers, httpResponseContent);
        }

        /// <summary>
        /// 查询短信模板。
        /// <para><see cref="https://docs.jiguang.cn/jsms/server/rest_api_jsms_templates/#api_3"/></para>
        /// </summary>
        public HttpResponse QueryMessageTemplate(int tempId)
        {
            Task<HttpResponse> task = Task.Run(() => QueryMessageTemplateAsync(tempId));
            task.Wait();
            return task.Result;
        }
        #endregion


    }



}
