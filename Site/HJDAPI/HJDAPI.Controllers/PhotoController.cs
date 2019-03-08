using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using HJDAPI.Common.Helpers;
using HJDAPI.Models;
using HJD.DestServices.Contract;
using HJD.Framework.WCF;
using HJD.HotelServices.Contracts;
using HJD.HotelServices;
using HJD.Search.Contracts;
using HJD.Search.Entity;
using HJD.PhotoServices.Contracts;
using HJDAPI.Controllers.Common;
using HJD.PhotoServices.Entity;
using HJD.PhotoSyncServices.Contracts;
using System.Net.Http;
using System.Net;
using System.Drawing;
using System.IO;
using System.Net.Http.Headers;

namespace HJDAPI.Controllers
{
    public class PhotoController : BaseApiController
    {
        public static IPhotoService PhotoService = ServiceProxyFactory.Create<IPhotoService>("BasicHttpBinding_IPhotoService");
        public static IPhotoSyncService photoSyncService = ServiceProxyFactory.Create<IPhotoSyncService>("IPhotoSyncService");
        public static IDestService DestService = ServiceProxyFactory.Create<IDestService>("BasicHttpBinding_IDestService");
        public static IHotelService HotelService = ServiceProxyFactory.Create<IHotelService>("BasicHttpBinding_IHotelService");
        public static ISearchTipAPI SearchTipApiService = ServiceProxyFactory.Create<ISearchTipAPI>("wsHttpBinding_ISearchTipAPI");

       

        /// <summary>
        /// 获取酒店详情
        /// </summary>
        /// <param name="id">酒店id</param>
        /// <returns></returns>
        [HttpGet]
        public System.Web.Mvc.RedirectResult Get(int id)
        {
          //  RedirectToRouteResult re = new RedirectToRouteResult();
            System.Web.Mvc.RedirectResult r = new System.Web.Mvc.RedirectResult("http://p3.lvpingphoto.com/B6Uf7YDW_wood");
            return r;
        }

        [HttpGet]
        public bool Del(int id)
        {
            bool bResult = PhotoService.DelHotelPhoto(id);
            HotelService.RefreshHotelPhotos(id);

            return bResult;
        }

        [HttpGet]
        public bool SetCover(int id)
        {
            bool bResult = PhotoService.SetHotelCoverPhoto(id);
            HotelService.RefreshHotelPhotos(id);
            return bResult;
        }

        /// <summary>
        /// 上传图片链接至upai
        /// </summary>
        /// <param name="picUrls"></param>
        /// <returns></returns>
        [HttpPost]
        public List<int> PhotoUploadWithUrls(string[] picUrls)
        {
            var _ids = new List<int>();

            var _typeId = 1;
            var _appId = 1;
            var _bSync = true;
            _ids = photoSyncService.PhotoUploadWithPicUrls(picUrls, _typeId, _appId, _bSync);

            return _ids;
        }

        /// <summary>
        /// 返回图片上传后生成的paths
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpPost]
        public List<PHSPhotoInfoEntity> GenPhotoPaths(List<int> ids)
        {
            var _pathObjs = new List<PHSPhotoInfoEntity>();

            if (ids != null && ids.Count > 0)
            {
                _pathObjs = PhotoService.GetPhotoInfo(ids);
            }

            return _pathObjs;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetZmjdImgByUrl(string url)
        {
            Uri myUri = new Uri(url);
            WebRequest webRequest = WebRequest.Create(myUri);
            WebResponse webResponse = webRequest.GetResponse();
            Bitmap myImage = new Bitmap(webResponse.GetResponseStream());


            MemoryStream ms = new MemoryStream();
            myImage.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);

            //var resp = new HttpResponseMessage(HttpStatusCode.OK)
            //{
            //    Content = new StreamContent(new MemoryStream(ms.ToArray()))
            //};
            //resp.Content.Headers.ContentType = new MediaTypeHeaderValue("image/jpg");

            var resp = Request.CreateResponse();
            resp.StatusCode = HttpStatusCode.OK;
            resp.Content = new StreamContent(new MemoryStream(ms.ToArray()));
            resp.Content.Headers.ContentType = new MediaTypeHeaderValue("image/jpg");
            resp.Headers.CacheControl = new CacheControlHeaderValue()
            {
                Public = true,
                MaxAge = new TimeSpan(1, 0, 0, 0)
            };

            return resp;
        }
    }
}