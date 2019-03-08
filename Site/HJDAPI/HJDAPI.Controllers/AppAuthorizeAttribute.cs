using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using HJDAPI.Common.Helpers;

namespace HJDAPI.Controllers
{
    public class AppAuthorizeAttribute:AuthorizeAttribute
    {
        public override void OnAuthorization(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            var encrypte = HttpContext.Current.Request.Headers["WH-Encrypte"];
            var userid = HttpContext.Current.Request.Headers["WH-Userid"];
            if (string.IsNullOrEmpty(userid) || string.IsNullOrEmpty(encrypte))
            {
                actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                return;
            }

            var encrypteString = SecurityHelper.DecryptDES(encrypte, Configs.WHMo);
            if (userid != encrypteString)
            {
                actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
            }
        }
    }
}
