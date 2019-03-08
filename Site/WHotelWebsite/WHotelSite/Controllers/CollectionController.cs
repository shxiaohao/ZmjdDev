using HJDAPI.APIProxy;
using HJDAPI.Models;
using PersonalServices.Contract;
using PersonalServices.Contracts.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WHotelSite.Params.Order;

namespace WHotelSite.Controllers
{
    public class CollectionController : BaseController
    {
        //
        // GET: /Collection/

        public ActionResult Index()
        {
            return View();
        }

        // 收藏列表
        public ActionResult List()
        {
            if (!UserState.IsLogin)
            {
                return Json(new { Message = "必须登录后才能查看", Success = 1 }, JsonRequestBehavior.AllowGet);
            }
            CollectParam param = new CollectParam();
            long userID;
            long.TryParse(this.Request.Form["user"],out userID);            
            param.UserID = userID;

            if (param.UserID == 0 && UserState.UserID != 0)
            {
                param.UserID = UserState.UserID;
            }
            CollectHotelResult result = Collect.GetCollectHotelList(param.UserID);
            ViewBag.totalCount = result.TotalCount;
            return View("List", result);
        }

        // 添加收藏
        public ActionResult Add()
        {
            CollectParamModel param = new CollectParamModel();
            long userID, hotelID;
            int interestID;
            long.TryParse(this.Request.Form["user"],out userID);
            long.TryParse(this.Request.Form["hotel"],out hotelID);
            int.TryParse(this.Request.Form["interest"], out interestID);
            
            param.UserID = userID;
            param.Items = new List<CollectParamItemModel>();
            param.Items.Add(new CollectParamItemModel() { HotelID = hotelID, InterestID = interestID });

            if (userID == 0 && UserState.UserID != 0)
            {
                param.UserID = UserState.UserID;
            }
            
            return Json(Collect.Add(param));
        }

        // 移除收藏
        public ActionResult Remove()
        {
            CollectParamModel param = new CollectParamModel();
            long userID, hotelID;
            long.TryParse(this.Request.Form["user"], out userID);
            long.TryParse(this.Request.Form["hotel"], out hotelID);

            param.UserID = userID; 
            param.Items = new List<CollectParamItemModel>();
            param.Items.Add(new CollectParamItemModel() { HotelID = hotelID });

            if (userID == 0 && UserState.UserID != 0)
            {
                param.UserID = UserState.UserID;
            }
            return Json(Collect.Remove(param));
        }
    }
}
