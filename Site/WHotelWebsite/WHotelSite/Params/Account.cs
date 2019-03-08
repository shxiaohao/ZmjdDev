using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WHotelSite.Params.Account
{
    public class VerifyParam : ParamBase
    {
        public string Action;
        public string Number;
        public string Code;
        public string InvitationCode;
        public string Password;
        public bool IsSaveCookie;
        public bool IsFollow;
        public long FollowUserId;
        public string Callback;
        public long CID;
        public string Unionid;

        public string Geetest_Challenge;
        public string Geetest_Seccode;
        public string Geetest_Validate;

        public VerifyParam(Controller controller) : base(controller)
        {
            Callback = controller.Request.Form["callback"];
            Action = controller.Request.Form["action"];
            Number = controller.Request.Form["number"];
            Code = controller.Request.Form["code"];
            InvitationCode = controller.Request.Form["invCode"];
            Password = controller.Request.Form["password"];
            bool.TryParse(controller.Request.Form["isSaveCookie"], out IsSaveCookie);
            bool.TryParse(controller.Request.Form["isFollow"], out IsFollow);
            Int64.TryParse(controller.Request.Form["followUserId"], out FollowUserId);
            Int64.TryParse(controller.Request.Form["CID"], out CID);
            Unionid = controller.Request.Form["unionid"];
            Geetest_Challenge= controller.Request.Form["geetest_challenge"];
            Geetest_Seccode = controller.Request.Form["geetest_seccode"];
            Geetest_Validate = controller.Request.Form["geetest_validate"];
        }
    }
}