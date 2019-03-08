using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using HJDAPI.APIProxy;
using WHotelSite.ViewModels;
using HJD.AccessService.Contract.Model;
using HJD.HotelServices.Contracts;

namespace WHotelSite.Controllers
{
    public class PortalController : BaseController
    {
        protected readonly attraction AttractionProxy = new attraction();
     

        public ActionResult Aboutus()
        {
            return View();
        }

        public ActionResult OurTeam()
        {
            return View();
        }

        public ActionResult ContactUs()
        {
            return View();
        }

        // 网站首页
        public ActionResult Home(float userLat = 0, float userLng = 0, int district = 2, int geoScopeType = 1)
        {
            var isMobile = Utils.IsMobile();
            ViewBag.IsMobile = isMobile;

            //如果是移动端进来，则跳转至app版首页
            if (isMobile)
            {
                return Redirect(string.Format("/app/home?userid={0}&userlat={1}&userlng={2}", 0, userLat, userLng));
            }

            //geoScopeType 1城市 2 城市周围 3 坐标周围
            var homeData = Hotel.GetHomePageData(district, geoScopeType);
            if (homeData == null)
            {
                return HttpNotFound();
            }
            var ret = new HomeViewModel
            {
                DistrictId = district,
                TopHotPackage = Mapper.ConvertHotPackageInfoToHotPackageModel(homeData.topPackage),
                Districts = Mapper.ConvertCityEntityListToSimpleDistrictModelList(Hotel.GetZMJDCityData2()),
                HotPackages = Mapper.ConvertHotPackageInfoListToHotPackageModelList(homeData.Preferential),
                Sights = Mapper.ConvertInterestEntityListToInterestEntityModelList(homeData.InterestData != null ? homeData.InterestData.SightInterestList : null),
                Themes = Mapper.ConvertInterestEntityListToInterestEntityModelList(homeData.InterestData != null ? homeData.InterestData.ThemeInterestList : null),
                SightCategories = Mapper.ConvertSightCategoryListToSightCategoryModelList(homeData.InterestData != null ? homeData.InterestData.SightCategoryList : null)
            };

            var homeData30 = new Hotel().GetHomePageData30(userLat, userLng, geoScopeType, district);
            ViewBag.HotelData30 = homeData30;

            //记录网站首页加载
            RecordBehavior("HomeLoad");
            ViewBag.PageTag = "Home";

            return View(ret);
        }

        public ActionResult InterestList(int districtId = 2, float lat = 0, float lng = 0, string category = "zbzt")
        {
            //var data = Interest.QueryInterest20( category.StartsWith("d_")?districtId: 0, lat, lng);
            var data = Interest.QueryInterest20(districtId, lat, lng);
            if (data == null)
            {
                return HttpNotFound();
            }

            List<InterestEntity> themeList = data.ThemeInterestList;
            themeList = themeList.FindAll(_ => _.ID <= 100003);

            var viewModel = new InterestListViewModel
            {
                DistrictId = districtId,
                Interests = category.EndsWith("zbzt") ? Mapper.ConvertInterestEntityListToInterestEntityModelList(themeList)
                : Mapper.ConvertInterestEntityListToInterestEntityModelList(data.SightInterestList)
            };

            return View(viewModel);
        }

        public ActionResult GetDistrictSuggest()
        {
            var model = Utils.GetDistrictSuggest();
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult InterestListSelect(int districtId = 2, float lat = 0, float lng = 0)
        {
            var data = Interest.QueryInterest20(districtId, lat, lng);
            if (data == null)
            {
                return HttpNotFound();
            }
            
            //List<InterestEntity> themeList = data.ThemeInterestList.FindAll(_ => _.ID <= 100003);//由于不再维护图

            List<InterestEntity> themeList = data.ThemeInterestList;
            themeList = themeList.FindAll(_ => _.ID <= 100003);

            var ret = new InterestListSelectViewModel
            {
                Sights = Mapper.ConvertInterestEntityListToInterestEntityModelList(data.SightInterestList),
                Themes = Mapper.ConvertInterestEntityListToInterestEntityModelList(themeList)
            };
            return Json(ret,JsonRequestBehavior.AllowGet);
        }

        public ActionResult SearchList(string keyword, string page)
        {
            //新搜索
            var list = new App().Search(keyword, 20);   //.Where(s => s.Boost > );
            var ret = new SearchListViewModel
            {
                Hotels = Mapper.ConvertAppSearchHotelResultToHotelModelList(list),
                Keyword = keyword
            };

            ////原始搜索
            //var list = Hotel.SuggestHotel(keyword, 20);
            //var ret = new SearchListViewModel
            //{
            //    Hotels = Mapper.ConvertSuggestListToHotelModelList(list),
            //    Keyword = keyword
            //};

            //记录搜索行为
            RecordBehavior(page + "Search", keyword);

            return View(ret);
        }
    }
}
