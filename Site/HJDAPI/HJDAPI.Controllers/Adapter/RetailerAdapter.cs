using HJD.Framework.WCF;
using HJD.HotelManagementCenter.Domain;
using HJDAPI.Common.Helpers;
using HJDAPI.Controllers.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.Controllers.Adapter
{
    public class RetailerAdapter
    {
        public static HJD.HotelManagementCenter.IServices.IRetailerService RetailerService = ServiceProxyFactory.Create<HJD.HotelManagementCenter.IServices.IRetailerService>("HMC_IRetailerService");
        public static RetailerShopEntity GetRetailerShopByCID(long cid)
        {
            RetailerShopEntity retailerShop = RetailerService.GetRetailerShopByCID(cid);
            retailerShop = retailerShop == null ? new RetailerShopEntity() : retailerShop;
            retailerShop.AvatarUrl = string.IsNullOrWhiteSpace(retailerShop.AvatarUrl) ? "http://whfront.b0.upaiyun.com/app/img/home/zmjd-logo-256x256.png" :
                PhotoAdapter.GenHotelPicUrl(retailerShop.AvatarUrl, Enums.AppPhotoSize.shop);

            retailerShop.ShopName = string.IsNullOrEmpty(retailerShop.ShopName) ? "周末酒店APP" : retailerShop.ShopName;

            return retailerShop;

        }
        public static int AddRetailerShop(RetailerShopEntity retailershop)
        {
            return RetailerService.AddRetailerShop(retailershop);
        }
        public static int AddOrUpdateRetailerShop(RetailerShopEntity retailershop)
        {
            return RetailerService.AddOrUpdateRetailerShop(retailershop);
        }

        public static int UpdateAvatarUrl(long CID, string avatarUrl)
        {
            return RetailerService.AddOrUpdateRetailerShopAvatarUrl(CID, avatarUrl);
        }

        public static int UpdateRetailerShop(RetailerShopEntity retailershop)
        {
            return RetailerService.UpdateRetailerShop(retailershop);
        }

        public static RetailerInvateEntity GetRetailerInvateByUserID(long userId)
        {
            return RetailerService.GetRetailerInvateByUserID(userId).First();
        }
    }
}
