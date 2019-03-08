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
using HJDAPI.Controllers.Adapter;
using HJD.HotelPrice.Contract.DataContract.Order;
using System.IO;
using Newtonsoft.Json;
using HJDAPI.Controllers.Common;
using PackageDailyEntity = HJD.HotelServices.Contracts.PackageDailyEntity;

namespace HJDAPI.Controllers
{
    public class PriceController : BaseApiController
    {
        [HttpGet]
        public Models.HotelPrice Get(int id, string checkIn, string checkOut, string sType)
        {
            return PriceAdapter.Get(id, checkIn, checkOut, sType);
        }

        [HttpGet]
        public Models.HotelPrice2 Get2(int id, string checkIn, string checkOut, string sType)
        {
            return PriceAdapter.Get2(id, checkIn, checkOut, sType);
        }

        [HttpGet]
        public Models.HotelPrice2 Get3(int id, string checkIn, string checkOut, string sType)
        {
            return PriceAdapter.GetHotelPackageList(id, checkIn, checkOut, sType, AppVer, AppUserID);
        }

        [HttpGet]
        public Models.HotelPrice2 Get32(int id, string checkIn, string checkOut, string sType, int updatePrice)
        {
            return PriceAdapter.Get32(id, checkIn, checkOut, sType, updatePrice == 1);
        }

        [HttpGet]
        public Models.HotelPrice2 Get4(int hotelId, string code, string sType, int pid)
        {
            return PriceAdapter.Get4(hotelId, code, sType, pid);
        }

        /// <summary>
        /// 获取酒店套餐可销售状态
        /// </summary>
        /// <param name="hotelid"></param>
        /// <param name="startDate"></param>
        /// <returns></returns>
        [HttpGet]
        public List<HJD.HotelServices.Contracts.PDayItem> GetHotelPackageCalendar(int hotelid, string startDate)
        {
            return PackageAdapter.GetHotelPackageCalendar(hotelid, CommMethods.TransSpecialDateFormat2StandardDate(startDate));
        }

        [HttpGet]
        public List<HJD.HotelServices.Contracts.PDayItem> GetHotelPackageCalendar30(int hotelid, string startDate)
        {
            var list  = PackageAdapter.GetHotelPackageCalendar30(hotelid, CommMethods.TransSpecialDateFormat2StandardDate(startDate));
            if( base.IsIOS && !base.IsThanVer5_7 ) //5.7版之前的IOS APP由于日历效率问题，只返回90天的
            {
                return list.Take(180).ToList();
            }
            else
            {
                return list;
            }
        }

        [HttpGet]
        public List<HJD.HotelServices.Contracts.PDayItem> GetOneHotelPackageCalendar30(int hotelid, string startDate, int pid)
        {
            if (AppType.ToLower().Contains("ios"))
            {
                startDate = "0";
            }
            return PackageAdapter.GetHotelPackageCalendar30(hotelid, CommMethods.TransSpecialDateFormat2StandardDate(startDate), pid);
        }

        /// <summary>
        /// 2016-02-23 去除套餐的Rates数组
        /// </summary>
        /// <param name="hotelid"></param>
        /// <param name="checkIn"></param>
        /// <param name="checkOut"></param>
        /// <param name="sType"></param>
        /// <returns></returns>
        [HttpGet]
        public HotelPrice3 Get5(int hotelid, string checkIn, string checkOut, string sType)
        {
            HotelPrice2 hp2 = PriceAdapter.Get3(hotelid, checkIn, checkOut, sType, true, AppVer, AppUserID);

            //处理套餐内容  没有Brief则取dailyItems第一条代替
            //没有serialNo 则以套餐的Code代替

            hp2.Packages.ForEach((_) =>
            {
                if (string.IsNullOrWhiteSpace(_.packageBase.Brief))
                {
                    string brief = _.DailyItems.First().Items.FindAll(i => i.Type == 1).First().Description;
                    _.packageBase.Brief = brief;
                }
                if (string.IsNullOrWhiteSpace(_.packageBase.SerialNO))
                {
                    _.packageBase.SerialNO = _.packageBase.Code;
                }
            });

            //修补ios Banner跳转到套餐列表不成功 增加Packages数据 2015-09030 wwb
            HotelPrice3 hp3 = new HotelPrice3()
            {
                CheckIn = hp2.CheckIn,
                CheckOut = hp2.CheckOut,
                DayLimitMax = hp2.DayLimitMax,
                DayLimitMin = hp2.DayLimitMin,
                HotelID = hp2.HotelID,
                Name = hp2.Name,
                JLPackages = hp2.JLPackages,
                OTAList = hp2.OTAList,
                PackageGroups = new List<PackageGroupItem>(),
                Packages = new List<PackageInfoEntity>()
                {
                    new PackageInfoEntity(){
                        ActiveRebate = 0,
                        Rebate = 0,
                        SellState = 0,
                        CanUseCashCoupon = 0
                    }
                }
            };
            if (hp2.Packages != null && hp2.Packages.Count > 0)
            {
                foreach (var tempList in hp2.Packages.GroupBy(_ => _.packageBase.SerialNO + _.PayType.ToString()))
                {
                    var packageInfoEntitys = tempList.ToList();
                    var roomCodes = packageInfoEntitys.Select(_ => _.Room.RoomCode).Distinct().ToList();
                    Dictionary<string, PackageInfoEntity> dic = new Dictionary<string, PackageInfoEntity>();
                    OrderPackageList( packageInfoEntitys);
                    foreach (var package in packageInfoEntitys)
                    {
                        var roomCode = package.Room.RoomCode;
                        if (!dic.ContainsKey(roomCode))
                        {
                            package.Rates = new List<PRateEntity>();//app页面不需要的数据
                            //所有套餐（包括携程） 增加此套餐不支持增值税发票开具  from tracy at 2016-07-27 
                          //  package.Items.Add(PriceAdapter.GenOnePItemEntity(hp3.CheckIn, hotelid, package.packageBase.ID));
                            dic.Add(roomCode, package);
                        }
                    }

                    hp3.PackageGroups.Add(new PackageGroupItem() { RoomCodes = roomCodes, dicRoomTypePackage = dic });
                }
                OrderPackageGroupList(hp3);
            }
            return hp3;
        }

        private static void OrderPackageGroupList(HotelPrice3 hp3)
        {
            hp3.PackageGroups = hp3.PackageGroups.OrderBy(_ => _.dicRoomTypePackage.First().Value.SellState).ThenByDescending(_ => _.dicRoomTypePackage.First().Value.packageBase.ForVIPFirstBuy ? 1 : 0).ThenBy(_ => _.dicRoomTypePackage.First().Value.Price).ToList();

        }

        private static  void OrderPackageList(List<PackageInfoEntity> packageInfoEntitys)
        {
          packageInfoEntitys =   packageInfoEntitys.OrderBy(_ => _.SellState).ThenByDescending(_ => _.packageBase.ForVIPFirstBuy?1:0).ThenBy(_ => _.Price).ToList();
        }
         

        /// <summary>
        /// 2016-02-23 去除套餐的Rates数组
        /// 统一使用Items绑定套餐显示内容和注意事项
        /// 后台算好几晚 房间
        /// </summary>
        /// <param name="hotelid"></param>
        /// <param name="checkIn"></param>
        /// <param name="checkOut"></param>
        /// <param name="sType"></param>
        /// <returns></returns>
        [HttpGet]
        public HotelPrice3 Get6(int hotelid, string checkIn, string checkOut, string sType, long userId = 0)
        {
            HotelPrice2 hp2 = PriceAdapter.Get3(hotelid, checkIn, checkOut, sType, true, ((string.IsNullOrWhiteSpace(AppVer) || AppVer == "0") ? "4.2" : AppVer), userId == 0 ? AppUserID : userId);

            //处理套餐内容  没有Brief则取dailyItems第一条代替
            //没有serialNo 则以套餐的Code代替
            //修补ios Banner跳转到套餐列表不成功 增加Packages数据 2015-09030 wwb
            HotelPrice3 hp3 = new HotelPrice3()
            {
                CheckIn = hp2.CheckIn,
                CheckOut = hp2.CheckOut,
                DayLimitMax = hp2.DayLimitMax,
                DayLimitMin = hp2.DayLimitMin,
                HotelID = hp2.HotelID,
                Name = hp2.Name,
                JLPackages = hp2.JLPackages ?? new List<PackageInfoEntity>(),
                OTAList = hp2.OTAList ?? new List<OTAInfo2>(),
                PackageGroups = new List<PackageGroupItem>(),
                Packages = new List<PackageInfoEntity>()
                {
                    new PackageInfoEntity(){
                        ActiveRebate = 0,
                        Rebate = 0,
                        SellState = 0,
                        CanUseCashCoupon = 0,
                        CancelPolicy = ""
                    }
                }
            };

            int nightCount = (hp3.CheckOut - hp3.CheckIn).Days;

            if (hp2.Packages != null && hp2.Packages.Count > 0)
            {
                hp2.Packages.ForEach((_) =>
                {
                    if (string.IsNullOrWhiteSpace(_.packageBase.Brief) && _.DailyItems != null && _.DailyItems.Count != 0)
                    {
                        var tempItem = _.DailyItems.First().Items.FindAll(i => i.Type == 1).FirstOrDefault();
                        string brief = tempItem != null ? tempItem.Description : "";
                        _.packageBase.Brief = brief;
                    }

                    if (string.IsNullOrWhiteSpace(_.packageBase.SerialNO))
                    {
                        _.packageBase.SerialNO = _.packageBase.Code;
                    }

                });

                //zmjiudian的套餐和其他渠道套餐分开
                var basePackageList = new List<List<PackageInfoEntity>>();
                var hotelPackageTypeNum = Convert.ToInt32(HJD.HotelServices.Contracts.HotelServiceEnums.PackageType.HotelPackage);
                if (hp2.Packages.Exists(p => p.PackageType == hotelPackageTypeNum)) basePackageList.Add(hp2.Packages.Where(p => p.PackageType == hotelPackageTypeNum).ToList());
                if (hp2.Packages.Exists(p => p.PackageType != hotelPackageTypeNum)) basePackageList.Add(hp2.Packages.Where(p => p.PackageType != hotelPackageTypeNum).ToList());

                #region 套餐分组排序

                for (int i = 0; i < basePackageList.Count; i++)
                {
                    var basePackages = basePackageList[i];
                    if (basePackages != null && basePackageList.Count > 0)
                    {
                        var packageGroups = new List<PackageGroupItem>();

                        //分组
                        var basePackageGroup = basePackages.GroupBy(_ => _.packageBase.SerialNO + _.PayType.ToString());
                        foreach (var tempList in basePackageGroup)
                        {
                            var packageInfoEntitys = tempList.ToList();
                            var roomCodes = packageInfoEntitys.Select(_ => _.Room.RoomCode).Distinct().ToList();
                            Dictionary<string, PackageInfoEntity> dic = new Dictionary<string, PackageInfoEntity>();
                            OrderPackageList(packageInfoEntitys);
                            foreach (var package in packageInfoEntitys) 
                            {
                                var roomCode = package.Room.RoomCode;
                                if (!dic.ContainsKey(roomCode))
                                {
                                    package.Rates = new List<PRateEntity>();//app页面不需要的数据
                                    package.DailyItems = new List<PackageDailyEntity>();//统一使用 Items绑定套餐内容 注意事项 不再使用DailyItems
                                    GenRoomDescription(nightCount, package, roomCode);
                                   
                                    //所有套餐（包括携程） 增加此套餐不支持增值税发票开具  from tracy at 2016-07-27 
                                  //  package.Items.Add(PriceAdapter.GenOnePItemEntity(hp3.CheckIn, hotelid, package.packageBase.ID));

                                    dic.Add(roomCode, package);
                                }
                            }

                            packageGroups.Add(new PackageGroupItem() { RoomCodes = roomCodes, dicRoomTypePackage = dic });
                        }
                        hp3.PackageGroups.AddRange(packageGroups);
                        //packageGroups.OrderBy(_ => _.dicRoomTypePackage.First().Value.SellState).ThenByDescending(_ => _.dicRoomTypePackage.First().Value.packageBase.ForVIPFirstBuy).ThenBy(_ => _.dicRoomTypePackage.First().Value.Price);
                        OrderPackageGroupList(hp3);
                    }
                }

                #endregion
            }

            //判断是不是会员
            var customerType = AccountAdapter.GetCustomerType(AppUserID);
            hp3.CustomerType = (int)customerType;

            //if (customerType == Enums.CustomerType.vip)
            //{
            //    hp3.Suggest = new BuyMembershipSuggest()
            //    {
            //        ActionUrl = "",
            //        Text = ""
            //    };
            //}
            //else
            //{
            //    //跟随非会员显示的会员价变动 如果不显示会员这里也不显示
            //    //var actionUrl = string.Format("whotelapp://www.zmjiudian.com/gotopage?url=http://www.zmjiudian.com/custom/shop/vip/{0}?userid={1}", 100250, AppUserID);
            //    //hp3.Suggest = new BuyMembershipSuggest()
            //    //{
            //    //    ActionUrl = actionUrl,
            //    //    Text = "现在去申请VIP >"
            //    //};
            //}

            PriceAdapter.GenPackageUpGradeInfo(hp3.Packages, userId, customerType);


            return hp3;
        }

        [HttpGet]
        public HotelPrice2 GetOtaList(int id, string checkIn, string checkOut, string sType)
        {
            return PriceAdapter.GetOtaList(id, checkIn, checkOut, sType);
        }

        [HttpGet]
        public HotelPrice2 GetV42(int id, string checkIn, string checkOut, string sType, long userID = 0, bool needNotSalePackage=false)
        {
            return PriceAdapter.GetHotelPackageList(id, checkIn, checkOut, sType, "4.2", userID == 0 ? AppUserID : userID, needNotSalePackage: needNotSalePackage);
        }

        [HttpGet]
        public HotelPackageCalendar GetOneHotelPackageCalendar(int hotelid, string startDate, int pid, long userId = 0)
        {
            userId = userId == 0 ? AppUserID : userId;
            HJDAPI.Common.Helpers.Enums.CustomerType customerType = AccountAdapter.GetCustomerType(userId);

            return GetOneHotelPackageCalendarWithCustomerType(hotelid, startDate, pid, customerType);
        }

        [HttpGet]
        public HotelPackageCalendar GetOneHotelPackageCalendarWithCustomerType(int hotelid, string startDate, int pid, HJDAPI.Common.Helpers.Enums.CustomerType customerType)
        {
             var hp = new HotelPrice2();
            var packagePrice = new List<PackageInfoEntity>() { new PackageAdapter().GetHotelPackageByCode(hotelid, "", pid) };

            PriceAdapter.SetDayLimit(hp, packagePrice);
            var list = PackageAdapter.GetHotelPackageCalendarWithCustomerType(hotelid, CommMethods.TransSpecialDateFormat2StandardDate(startDate),customerType,pid:pid);

            return new HotelPackageCalendar()
            {
                DayLimitMin = hp.DayLimitMin,
                DayLimitMax = hp.DayLimitMax,
                DayItems = list
            };
        }


        /// <summary>
        /// 获取指定日期的日历信息和指定入住日期的套餐总价
        /// </summary>
        /// <param name="hotelid"></param>
        /// <param name="startDate"></param>
        /// <param name="pid"></param>
        /// <param name="userId"></param>
        /// <param name="checkIn"></param>
        /// <param name="nightCount"></param>
        /// <returns></returns>
        [HttpGet]
        public HotelPackagePriceCalendarEntity GetOneHotelPackageCalendarForWxapp(int hotelid, string startDate, int pid, long userId = 0, string checkIn = "", int nightCount = 1)
        {
            var priceCalendarEntity = new HotelPackagePriceCalendarEntity();

            userId = userId == 0 ? AppUserID : userId;
            var hp = new HotelPrice2();
            var packagePrice = new List<PackageInfoEntity>() { new PackageAdapter().GetHotelPackageByCode(hotelid, "", pid) };

            PriceAdapter.SetDayLimit(hp, packagePrice);
            var list = PackageAdapter.GetHotelPackageCalendar30(hotelid, CommMethods.TransSpecialDateFormat2StandardDate(startDate), pid, userId);

            //calendar
            var calendar = new HotelPackageCalendar()
            {
                DayLimitMin = hp.DayLimitMin,
                DayLimitMax = hp.DayLimitMax,
                DayItems = list
            };
            priceCalendarEntity.Calendar = calendar;

            //price
            var totalPrice = 0;
            var totalVipPrice = 0;
            var totalNormalPrice = 0;

            var checkInDate = DateTime.Now.Date;
            var checkOutDate = checkInDate.AddDays(1);
            try
            {
                checkInDate = DateTime.Parse(checkIn);
                checkOutDate = checkInDate.AddDays(Convert.ToDouble(nightCount));
            }
            catch (Exception ex) { }

            //首先获取当前套餐的所有日期下的价格数据
            if (calendar != null && calendar.DayItems != null && calendar.DayItems.Count > 0)
            {
                var selCalendar = calendar.DayItems.Where(c => c.Day >= checkInDate && c.Day < checkOutDate).ToList();
                totalPrice = selCalendar.Sum(c => c.SellPrice);
                totalVipPrice = selCalendar.Sum(c => c.VipPrice);
                totalNormalPrice = selCalendar.Sum(c => c.NormalPrice);
                nightCount = selCalendar.Count;
            }

            priceCalendarEntity.TotalPrice = totalPrice;
            priceCalendarEntity.TotalVipPrice = totalVipPrice;
            priceCalendarEntity.TotalNormalPrice = totalNormalPrice;
            priceCalendarEntity.CheckIn = checkInDate;
            priceCalendarEntity.CheckOut = checkOutDate;
            priceCalendarEntity.NightCount = nightCount;

            return priceCalendarEntity;
        }

        [HttpGet]
        public PackageInfoEntity GetOnePackageInfoEntity(int pId, int hotelId, DateTime checkIn, DateTime checkOut, long userId = 0)
        {
            HotelPrice2 price = PriceAdapter.GetHotelPackageList(hotelId, checkIn.Date.ToString(), checkOut.Date.ToString(), "www", string.IsNullOrWhiteSpace(AppVer) ? "4.5" : AppVer, userId == 0 ? AppUserID : userId);
            PackageInfoEntity result = null;
            if (price != null && price.Packages != null && price.Packages.Count != 0)
            {
                result = price.Packages.FirstOrDefault(_ => _.packageBase.ID == pId);
                if (result != null)
                {
                    result.HotelName = price.Name;
                    result.CheckIn = checkIn;
                    result.CheckOut = checkOut;
                    result.InChina = price.InChina;

                    if (result.PayType == 3)
                    {
                        result.CanUseCoupon = (result.PackageType == (int)HotelServiceEnums.PackageType.CtripPackage || result.PackageType == (int)HotelServiceEnums.PackageType.CtripPackageByApi || result.PackageType == (int)HotelServiceEnums.PackageType.CtripPackageForHotel) ? true : result.CanUseCoupon;
                        result.packageBase.CanUseCoupon = result.CanUseCoupon;
                    }
                }
            }
            return result ?? new PackageInfoEntity(); ;
        }

        /// <summary>
        /// 获得酒店套餐列表
        /// </summary>
        /// <param name="hotelid"></param>
        /// <param name="checkIn"></param>
        /// <param name="checkOut"></param>
        /// <param name="sType"></param>
        /// <returns></returns>
        [HttpGet]
        public HotelPrice3 Get7(int hotelid, string checkIn, string checkOut, string sType, HJD.HotelServices.Contracts.HotelServiceEnums.PackageType packageType = HJD.HotelServices.Contracts.HotelServiceEnums.PackageType.HotelPackage, long userId = 0, bool isNotSale = false)
        {
            var curUserID = userId == 0 ? AppUserID : userId;

            HotelPrice2 hp2 = PriceAdapter.GetHotelPackageList(hotelid, checkIn, checkOut, sType, true, ((string.IsNullOrWhiteSpace(AppVer) || AppVer == "0") ? "4.5" : AppVer), curUserID, packageType != HJD.HotelServices.Contracts.HotelServiceEnums.PackageType.HotelPackage);

            TimeSpan timespan = hp2.CheckOut - hp2.CheckIn;

            //处理套餐内容  没有Brief则取dailyItems第一条代替
            //没有serialNo 则以套餐的Code代替
            //修补ios Banner跳转到套餐列表不成功 增加Packages数据 2015-09030 wwb
            HotelPrice3 hp3 = new HotelPrice3()
            {
                CheckIn = hp2.CheckIn,
                CheckOut = hp2.CheckOut,
                DayLimitMax = hp2.DayLimitMax,
                DayLimitMin = hp2.DayLimitMin,
                HotelID = hp2.HotelID,
                Name = hp2.Name,
                JLPackages = hp2.JLPackages ?? new List<PackageInfoEntity>(),
                OTAList = hp2.OTAList ?? new List<OTAInfo2>(),
                PackageGroups = new List<PackageGroupItem>(),
                Packages = new List<PackageInfoEntity>() { },
                HaveNotZMJDPackages = false,
                NightCount = Convert.ToInt32(timespan.TotalDays)
            };

            var isRealExistsCtripPackage = false;
            int nightCount = (hp3.CheckOut - hp3.CheckIn).Days;
            if (hp2.Packages != null && hp2.Packages.Count > 0)
            {              
                 
                //判断是不是会员
                var customerType = AccountAdapter.GetCustomerType(curUserID);
                bool isVIP = AccountAdapter.IsVIPCustomer(customerType);
                hp3.CustomerType =  isVIP? (int)Enums.CustomerType.vip :(int)customerType;//特殊处理，将VIP199、VIP599转换成VIP

                //用户是VIP，但不能够卖爆款时，不显示这些爆款
                if( isVIP  && !AccountAdapter.HasUserPriviledge(curUserID, HJD.AccountServices.Entity.PrivilegeEnums.UserPrivilege.CanBuyVIPFirstPackage))
                {
                    hp2.Packages = hp2.Packages.Where(_ => _.packageBase.ForVIPFirstBuy == false).ToList();
                }

                PriceAdapter.GenPackageUpGradeInfo(hp2.Packages, curUserID, customerType);

                //非会员的情况下  vip和非vip价格差大于200提示成为会员
                if (customerType == (int)HJDAPI.Common.Helpers.Enums.CustomerType.general || customerType == HJDAPI.Common.Helpers.Enums.CustomerType.inspector)
                {
                    hp2.Packages.ForEach(p =>
                    {
                        if (p.NotShowVIPPrice != 1)
                        {
                            p.Suggest.ActionUrl = "http://www.zmjiudian.com/Coupon/VipShopInfo?userid={userid}&_newpage=1&_isoneoff=1";
                            p.Suggest.Text = "成为VIP>>";
                            p.Suggest.Description = "付199元年费可";

                            p.OrderPageShowBuyVIP.ActionUrl = "";
                            p.OrderPageShowBuyVIP.Text = "";

                        }
                        //if (p.NotVIPPrice - p.VIPPrice >= 50)
                        //{去掉价差超过50加成为会员标识
                        //} 
                    });
                }

                var basePackageList = new List<List<PackageInfoEntity>>();
                //var hotelPackageType = (int)packageType;
                var hotelPackageTypeNum = (int)HJD.HotelServices.Contracts.HotelServiceEnums.PackageType.HotelPackage;

                #region 套餐分类型  zmjiudian的套餐和其他渠道套餐分开

                hp3.HaveNotZMJDPackages = hp2.Packages.Exists(p => p.PackageType == hotelPackageTypeNum);//如果有周末酒店套餐 正常会有携程套餐。 默认大部分情况都有非周末酒店直采套餐， 如果没有周末酒店套餐默认取携程套餐 因而此处为false App端无需显示加载更多。

                if (hp2.Packages.Exists(p => p.PackageType == hotelPackageTypeNum))
                {
                    basePackageList.Add(hp2.Packages.Where(p => p.PackageType == hotelPackageTypeNum && p.IsNotSale == isNotSale).ToList());//周末酒店的其他套餐
                }
                if (hp2.Packages.Exists(p => p.PackageType != hotelPackageTypeNum))
                {
                    isRealExistsCtripPackage = true;
                    basePackageList.Add(hp2.Packages.Where(p => p.PackageType != hotelPackageTypeNum).ToList());//非周末酒店的其他套餐
                }

                #endregion

                #region 初始化套餐描述 和 房型照片
                if (basePackageList.Any())
                {
                    //取整个酒店维护的房型信息
                    var pRoomInfoList = HotelAdapter.GetProomInfoList(hotelid);

                    //按房型ID找照片
                    var roomIds = basePackageList[0].Select(_ => _.Room.ID).Distinct();
                    var dicRoomAndPic = HotelAdapter.GenRoomAndPicDic(roomIds);

                    basePackageList[0].ForEach((_) =>
                    {
                        if (string.IsNullOrWhiteSpace(_.packageBase.Brief) && _.DailyItems != null && _.DailyItems.Count != 0)
                        {
                            var tempItem = _.DailyItems.First().Items.FindAll(i => i.Type == 1).FirstOrDefault();
                            string brief = tempItem != null ? tempItem.Description : "";
                            _.packageBase.Brief = brief;
                        }

                        if (string.IsNullOrWhiteSpace(_.packageBase.SerialNO))
                        {
                            _.packageBase.SerialNO = _.packageBase.Code;
                        }

                        var roomInfo = _.Room;
                        if (!isRealExistsCtripPackage)
                        {
                            var roomId = roomInfo.ID;
                            roomInfo.PicShortNames = dicRoomAndPic.ContainsKey(roomId) ? dicRoomAndPic[roomId].First() : "";
                        }
                        else if (pRoomInfoList != null && pRoomInfoList.FirstOrDefault(p => p.RoomCode == roomInfo.RoomCode) != null)
                        {
                            roomInfo.PicShortNames = HotelAdapter.genHotelRoomTypePicCompleteUrl(pRoomInfoList.First(p => p.RoomCode == roomInfo.RoomCode).PicShortNames).FirstOrDefault();
                        }
                    });
                }
                #endregion


                #region 套餐分组排序

                int canUserCashcoupon = 0;

                for (int i = 0; i < basePackageList.Count; i++)
                {
                    var basePackages = basePackageList[i];
                    if (basePackages != null && basePackageList.Count > 0)
                    {
                        var packageGroups = new List<PackageGroupItem>();

                        //分组
                        var basePackageGroup = basePackages.GroupBy(_ => _.packageBase.SerialNO + _.PayType.ToString());
                        foreach (var tempList in basePackageGroup)
                        {

                            var packageInfoEntitys = tempList.OrderByDescending(_=>_.packageBase.IsMainPush).ToList();
                            var roomCodes = packageInfoEntitys.Select(_ => _.Room.RoomCode).Distinct().ToList();
                            Dictionary<string, PackageInfoEntity> dic = new Dictionary<string, PackageInfoEntity>();
                            OrderPackageList(packageInfoEntitys);
                            List<HJD.AccountServices.Entity.User_Info> userList = AccountAdapter.GetUserInfoByRoleID((int)Enums.RoleType.客服);
                            foreach (var package in packageInfoEntitys)
                            {
                                if (package.packageBase.IsAskPrice == true)
                                {
                                    if (userList.Select(_ => _.UserId).ToList().Contains(curUserID))
                                    {
                                        package.packageBase.IsAskPrice = false;
                                    }
                                    else
                                    {
                                        //package.VIPPrice = 0; 
                                        string askPriceDescription = "价格变动较快，请询价后购买";
                                        package.CustomerAskPrice = AskPrice(DateTime.Parse(checkIn), DateTime.Parse(checkOut), package.packageBase.SerialNO, package.NotVIPPrice, package.VIPPrice, package.packageBase.HotelID, package.PayType, package.CancelPolicy, askPriceDescription, "");

                                        package.Suggest.ActionUrl = "";
                                        package.Suggest.Text = "";
                                        package.Suggest.Description = "";
                                        //package.BenefitPolicy = "";
                                    }
                                }
                                var roomCode = package.Room.RoomCode;
                                if (!dic.ContainsKey(roomCode))
                                {
                                    package.Rates = new List<PRateEntity>();//app页面不需要的数据
                                    package.DailyItems = new List<PackageDailyEntity>();//统一使用 Items绑定套餐内容 注意事项 不再使用DailyItems
                                    GenRoomDescription(nightCount, package, roomCode);

                                    //所有套餐（包括携程） 增加此套餐不支持增值税发票开具  from tracy at 2016-07-27 
                                  //  package.Items.Add(PriceAdapter.GenOnePItemEntity(hp3.CheckIn, hotelid, package.packageBase.ID));
                                    if (package.BenefitPolicy != "")
                                    {
                                        canUserCashcoupon++;
                                    }
                                    dic.Add(roomCode, package);
                                }
                            }

                            string Brief = "";
                            string SerialNo="";
                            if (base.IsThanVer5_7)
                            {
                                if ((int)packageType == 1 && dic.FirstOrDefault().Value.PackageType == 1)
                                {
                                    Brief = dic.FirstOrDefault().Value.packageBase.Brief;
                                    SerialNo = dic.FirstOrDefault().Value.packageBase.SerialNO;
                                }
                            }
                            else
                            {
                                //5.0版本以后 携程套餐Brief、SerialNo设为空
                                if (!string.IsNullOrWhiteSpace(AppVer) && AppVer.CompareTo("5.0") >= 0)
                                {
                                    if ((int)packageType == 1 && dic.FirstOrDefault().Value.PackageType == 1)
                                    {

                                        Brief = dic.FirstOrDefault().Value.packageBase.Brief.Replace(dic.FirstOrDefault().Key.Trim(), "房间");
                                        SerialNo = dic.FirstOrDefault().Value.packageBase.SerialNO;
                                    }
                                }
                                else
                                {
                                    //Log.WriteLog("PackageType11:" + dic.FirstOrDefault().Value.PackageType +":"+ AppVer.CompareTo("5.0"));
                                    Brief = dic.FirstOrDefault().Value.packageBase.Brief.Replace(dic.FirstOrDefault().Key.Trim(), "房间");
                                    SerialNo = dic.FirstOrDefault().Value.packageBase.SerialNO;
                                }
                            }

                            #region 判断系列号下套餐可售状态

                            //序列Group的状态值 0：可预订 1：查看可售日 2：已售完
                            int seriaNoState = 0;

                            //序列Group的描述文字显示 如“当前日期不可售，请查看其它日期”
                            string seriaNoDesc = "";

                            /*
                             * 目前只对zmjd自己的套餐做可售/不可售/查看可售日等逻辑处理 2019.02.26 haoy
                             *  _.PackageType == 1为zmjd自己的套餐，非1为其它渠道套餐 如携程 
                             */
                            if (dic.Values.ToList().Exists(_ => _.PackageType == 1))
                            {
                                //存在可售的
                                var existsCanSell = dic.Values.ToList().Exists(_ => _.packageBase.IsSellOut == false && _.SellState == 1);
                                if (existsCanSell)
                                {
                                    seriaNoState = 0;
                                    seriaNoDesc = "";
                                }
                                else
                                {
                                    //存在没有售空，但是并非可预订（其它售卖状态 如“查看可售日”）的套餐
                                    var existsNoSellOut = dic.Values.ToList().Exists(_ => _.packageBase.IsSellOut == false && _.SellState != 1);
                                    if (existsNoSellOut)
                                    {
                                        //需要查看可售日
                                        seriaNoState = 1;

                                        //如果全部套餐_.SellState == 2，才显示 当前日期不可售 的提示（_.SellState == 3，则会显示预订按钮，但是预订过程中提示更换日期）
                                        if (dic.Values.Where(_ => _.packageBase.IsSellOut == false && _.SellState == 2).Count() == dic.Values.Count)
                                        {
                                            seriaNoDesc = "当前日期不可售，请查看其它日期";
                                        }
                                        else
                                        {
                                            seriaNoDesc = "";
                                        }
                                    }
                                    else
                                    {
                                        //全部售完
                                        seriaNoState = 2;
                                        seriaNoDesc = "该套餐已售完";
                                    }
                                }
                            }

                            #endregion

                            int hasVIPFirstBuyCount = dic.Values.Where(_ => _.packageBase.ForVIPFirstBuy == true).ToList().Count;
                            List<string> groupLables = new List<string>();
                            bool hasVipFirstBuy = false;
                            if (hasVIPFirstBuyCount > 0)
                            {
                                hasVipFirstBuy = true;
                                groupLables.Add("http://whfront.b0.upaiyun.com/app/img/icon/new_vip.png");
                            }
                            
                            int ShowCount = dic.Values.Where(_ => _.SellState == 1).Count() >= 1 ? 2 : 1;

                            packageGroups.Add(new PackageGroupItem() { RoomCodes = roomCodes,
                                                                       dicRoomTypePackage = dic,
                                                                       ShowPackageGroupCount = ShowCount,
                                                                       Brief = Brief,
                                                                       SerialNo = SerialNo,
                                                                       GroupItemLables = groupLables,
                                                                       HasForVIPFirstBuy = hasVipFirstBuy,
                                                                       SeriaNoDesc = seriaNoDesc,
                                                                       SeriaNoState = seriaNoState
                            });
                        }
                    //    packageGroups.OrderBy(_ => _.dicRoomTypePackage.First().Value.SellState).ThenByDescending(_ => _.dicRoomTypePackage.First().Value.packageBase.ForVIPFirstBuy).ThenBy(_ => _.dicRoomTypePackage.First().Value.Price);
                        
                        hp3.PackageGroups.AddRange(packageGroups);
                        OrderPackageGroupList(hp3);
                        //hp3.PackageGroups. = ShowCount >= 1 ? 2 : 1;
                    }
                }
                // 拿掉送现金券的功能     2017-11-03
                //if (canUserCashcoupon > 0)
                //{
                //    hp3.HotelBenefitPolicy.ActionUrl = "http://www.zmjiudian.com/fund/userrecommend2?realuserid=1&userid={userid}";
                //    hp3.HotelBenefitPolicy.Description = "该酒店含可用现金券的套餐，";
                //    hp3.HotelBenefitPolicy.Text = "邀请好友得现金券";
                //}

                #endregion

            }


            //Log.WriteLog(string.Format("Get7已经返回结果，hotelID:{0}", hotelid));
            return hp3;
        }
        
        [HttpGet]
        public PackageInfoEntity GetFirstVIPPackageByPackageId(int pid, string checkIn, string checkOut)
        {
            DateTime arrivalTime = CommMethods.GetDefaultCheckIn();
            DateTime departureTime = arrivalTime.AddDays(1);
            if (!string.IsNullOrWhiteSpace(checkIn) && !string.IsNullOrWhiteSpace(checkOut))
            {
                arrivalTime = DateTime.Parse(checkIn);
                departureTime = DateTime.Parse(checkOut);
            }
            return PackageAdapter.GetFirstVIPPackageByPackageId(pid, arrivalTime, departureTime);
        }


        //public 

        private static void GenRoomDescription(int nightCount, PackageInfoEntity package, string roomCode)
        {
              if (package.packageBase.WHPackageType == (int)HJD.HotelServices.Contracts.HotelServiceEnums.WHPackageType.AirHotel)
            {
                package.Room.Description =  string.IsNullOrWhiteSpace(package.Room.Description) ? roomCode : package.Room.Description;//设置显示的房型数据
            }
            else
            {
                package.Room.Description = nightCount + "晚 " + (string.IsNullOrWhiteSpace(package.Room.Description) ? roomCode : package.Room.Description);//设置显示的房型数据
            }
        }
        public static CommentTextAndUrl AskPrice(DateTime checkIn, DateTime checkOut, string serialNo, int price, int vipPrice, int hoteId, int payType, string cancelPolicy, string description, string text)
        {
            CommentTextAndUrl askContent = new CommentTextAndUrl();
            askContent.ActionUrl = "http://www.zmjiudian.com/MagiCall/MagiCallClient?checkIn=" + checkIn + "&checkOut=" + checkOut + "&PCode=" + serialNo + "&price=" + price + "&VIPPrice=" + vipPrice + "&hotelId=" + hoteId + "&payType=" + payType + "&cancelPolicy=" + cancelPolicy + "&userid={userid}&realuserid=1"; 
            //string s = "http://www.zmjiudian.com/MagiCall/MagiCallClient?checkIn=" + checkIn + "&checkOut=" + checkOut + "&PCode=" + serialNo + "&price=" + price + "&VIPPrice=" + vipPrice + "&hotelId=" + hoteId + "&payType=" + payType + "&cancelPolicy=" + cancelPolicy + "&userid={userid}&realuserid=1"; 
            askContent.Description = description;
            askContent.Text = text;
            return askContent;
        }
    }
}