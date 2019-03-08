using HJD.AccountServices.Entity;
using HJD.HotelServices.Contracts;
using HJDAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WHotelSite.Params
{
    public class InspectorParam : ParamBase
    {
        public string Explain;
        public int UserId;

        public InspectorParam(Controller controller)
            : base(controller)
        {
            Explain = controller.Request.Form["explain"];

            var tempId = GetInt("user", 0);
            UserId = tempId == 0 ? GetIntFromQuery("user", 0) : tempId;
        }
    }

    public class InspectorRegisterParam : InspectorParam
    {
        public OperationResult RegisterResult;

        public InspectorRegisterParam(Controller controller)
            : base(controller)
        {

        }
    }

    public class InspectorHotelParam : InspectorParam
    {
        public InspectorHotelListResult HotelList;
        public InspectorHotel Hotel;
        public int Id;
        public int BS;
        public string CheckIn;
        public string CheckOut;

        public DateTime CheckInDate
        {
            get { return Utils.ParseDate(CheckIn, DateTime.Today); }
            set { CheckIn = Utils.FormatDate(value); }
        }

        public DateTime CheckOutDate
        {
            get { return Utils.ParseDate(CheckOut, CheckInDate.AddDays(1)); }
            set { CheckOut = Utils.FormatDate(value); }
        }

        public InspectorHotelParam(Controller controller)
            : base(controller)
        {
            int tempId = GetInt("hotel", 0);
            Id = tempId == 0 ? GetIntFromQuery("hotel", 0) : tempId;

            tempId = GetInt("bs", 0);
            BS = tempId == 0 ? GetIntFromQuery("bs", 0) : tempId;

            CheckIn = GetStringFromQuery("checkIn", "0");
            CheckOut = GetStringFromQuery("checkOut", "0");
        }
    }

    public class InspectorJumpParam : ParamBase
    {
        public string JumpUrl;

        public InspectorJumpParam(Controller controller)
            : base(controller)
        {
            JumpUrl = controller.Request.Form["jumpurl"];
        }
    }
}