using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http.Headers;

namespace HJDAPI.Models.JiGuangSMS
{
    public class HttpResponse
    {

        public HttpStatusCode StatusCode { get; set; }
        public HttpResponseHeaders Headers { get; set; }
        public string Content { get; set; }

        public HttpResponse(HttpStatusCode statusCode, HttpResponseHeaders headers, string content)
        {
            StatusCode = statusCode;
            Headers = headers;
            Content = content;
        }
    }
}
