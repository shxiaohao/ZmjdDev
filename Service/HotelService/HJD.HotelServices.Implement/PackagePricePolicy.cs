using HJD.HotelServices.Contracts;
using HJD.HotelServices.Implement.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJD.HotelServices.Implement
{
    public class PackagePricePolicy
    {
        private static HotelService hotelSvc = new HotelService();

        public static List<PackageInfoEntity> UseBotaoPricePolicyForWH(List<PackageInfoEntity> plist, List<CanSellCheapHotelPackageEntity> cheapPackageForBotao)
        {
            //铂涛只显示部分预付套餐
            if (plist != null && plist.Count != 0)
            {
                plist = plist.FindAll(_ => _.PayType == 3);//预付

                var list = from p in plist
                           join c in cheapPackageForBotao on p.packageBase.ID equals c.PId
                           select p;

                plist = list.ToList();              
            }

            return plist;
        }

        public static void UseBotaoPriceRatePolicyForWH(List<PackageRateEntity> plist)
        {
            plist.ForEach(_ =>
            {
                _.DailyList.ForEach(d =>
                {
                    d.ItemList.ForEach(i =>
                    {
                        if (i.SupplierType == (int)HJD.HotelServices.Contracts.HotelServiceEnums.SupplierType.RoomSupplier)
                        {
                            i.NotVIPPrice = i.Price;
                            //i.Price = i.Price - 5;//铂韬用户一致减去5元 待定  //2016.06.22 wwb 博涛不再提前减去价格
                            i.VIPPrice = i.Price;
                            i.CanUseCashCoupon = 0;//博涛用户不能用券
                        }
                    });
                    d.NotVIPPrice = d.ItemList.Sum(i => i.NotVIPPrice);
                    d.Price = d.ItemList.Sum(i => i.Price);
                    d.VIPPrice = d.ItemList.Sum(i => i.VIPPrice);
                    d.CanUseCashCoupon = 0;//博涛用户不能用券

                });

                _.NotVIPPrice = _.DailyList.Sum(d => d.NotVIPPrice);
                _.Price = _.DailyList.Sum(d => d.Price);
                _.VIPPrice = _.DailyList.Sum(d => d.VIPPrice);
                _.CanUseCashCoupon = 0;//博涛用户不能用券
            });
        }

           /// <summary>
        /// 周末酒店套餐的会员价
        /// </summary>
        /// <param name="plist"></param>
        internal static List<PackageInfoEntity> UseVIPPricePolicyForWH(List<PackageInfoEntity> plist, HotelServiceEnums.PricePolicyType ppt)
        {
            // VIP专享套餐只能VIP看到
            if(ppt != HotelServiceEnums.PricePolicyType.VIP)
            {
                plist = plist.Where(p => p.packageBase.OnlyForVIP == false).ToList();
            }

            return plist;
        }

        /// <summary>
        /// 周末酒店套餐的会员价
        /// </summary>
        /// <param name="plist"></param>
        internal static void UseVIPPriceRatePolicyForWH(List<PackageRateEntity> plist, HotelServiceEnums.PricePolicyType ppt)
        {
            plist.ForEach(_ =>
            {

                _.DailyList.ForEach(d =>
                {
                    d.ItemList.ForEach(i =>
                    {  
                        i.NotVIPPrice = i.Price; 

                        if (i.SupplierType == (int)HJD.HotelServices.Contracts.HotelServiceEnums.SupplierType.RoomSupplier)
                        {                          
                            if (ppt == HotelServiceEnums.PricePolicyType.VIP)
                            {
                                i.Price = i.VIPPrice;
                            }

                        }
                    });
                    d.NotVIPPrice = d.ItemList.Sum(i => i.NotVIPPrice);
                    d.Price = d.ItemList.Sum(i => i.Price);
                    d.VIPPrice = d.ItemList.Sum(i => i.VIPPrice);
                });

                _.NotVIPPrice = _.DailyList.Sum(d => d.NotVIPPrice);
                _.Price = _.DailyList.Sum(d => d.Price);
                _.VIPPrice = _.DailyList.Sum(d => d.VIPPrice);
            });
             
        }

        private static int CalPriceForCtrip(int hotelCtripPricePolicy, int price, int PayType)
        {
            //return price;
            //如果是前台现付 或 担保的，那么VIP也不享受优惠
            if (PayType == 1 || PayType == 2)
            {
                return price;
            }
            else
            {
                // 携程对接酒店= 返现后携程价格*0.96 或 (携程价格-30)，两者价格中高的一个价格  
                double price1 ;
                double price2;
                //if (hotelCtripPricePolicy == 0) //0:可以显示最低价  1：不显示最低价
                //{
                //    price1 = price * 0.92;
                //    price2 = price - 100;
                //}
                //else
                //{
                //    price1 = price * 0.96;
                //    price2 = price - 100;
                //}   
                
                price1 = price * 0.95;
                price2 = price - 50;
              
                return (int)((price1 > price2 ? price1 : price2));
            }
        }

        private static int CalRebatForCtrip(int price, int PayType)
        {

            //不再反现 2016/11/15
            ////如果是前台现付，那么VIP也不享受优惠
            //if (PayType == (int) HJD.HotelServices.Contracts.HotelServiceEnums.PackagePayType.FrontPay)
            //{
            //    // 携程对接酒店= 返现后携程价格*0.96 或 (携程价格-30)，两者价格中高的一个价格  
            //    double price1 = price * 0.96;
            //    double price2 = price - 24;

            //    return (int)( price - (price1 > price2 ? price1 : price2));
            //}
            //else
            //{
            //    return 0;
            //}
            return 0;
        }

        internal static List<PackageInfoEntity> UseVIPPricePolicyForCtrip(List<PackageInfoEntity> plist, HotelServiceEnums.PricePolicyType ppt)
        {
           
            StringBuilder sb = new StringBuilder();
            sb.Append("UseVIPPricePolicyForCtrip PPT:" + ppt.ToString());
            int hotelCtripPricePolicy = plist.Count == 0 ? 0 : hotelSvc.GetHotelCtripPricePolicy(plist.First().packageBase.HotelID);

            plist.ForEach(_ =>
            {
                _.DailyItems.ForEach(d =>
                {                          
                    //VIP价格策略
                    d.NotVIPPrice = d.Price;
                    d.VIPPrice = CalPriceForCtrip(hotelCtripPricePolicy, d.Price, d.PayType); //去掉携程vip价 yyb 20161213
                    if (ppt == HotelServiceEnums.PricePolicyType.VIP)
                    {
                        d.Price = d.VIPPrice;
                        d.Rebate = CalRebatForCtrip(d.Price, d.PayType);
                    }
                    sb.AppendFormat("Date:{0} notVIP:{1}  VIP:{2}", d.Day, d.NotVIPPrice, d.VIPPrice);
                });

                _.NotVIPPrice = _.Price;  //携程套餐在构造时没有按每天来构造DailyItems, 所以对于多天用Sum的方式会出错，这里采用直接处理的方式
                _.VIPPrice =  _.DailyItems.Sum(d => d.VIPPrice);// CalPriceForCtrip(_.Price, _.PayType); 去掉携程VIP价yyb20161213
                if (ppt == HotelServiceEnums.PricePolicyType.VIP)
                {
                    _.Price = _.VIPPrice;
                    _.Rebate = _.DailyItems.Sum(d => d.Rebate); //CalRebatForCtrip(_.Price, _.PayType);
                }

               _.CanUseCoupon = _.PayType == 3; //携程预付套餐可用现金券    
            });

      //      LogHelper.WriteLog(sb.ToString());

            return plist;
        }

     

        internal static void UseVIPPriceRatePolicyForCtrip(List<PackageRateEntity> plist, HotelServiceEnums.PricePolicyType ppt)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("UseVIPPriceRatePolicyForCtrip PPT:" + ppt.ToString());

            int hotelCtripPricePolicy = plist.Count == 0 ? 0 : hotelSvc.GetHotelCtripPricePolicy(plist.First().HotelID);
            plist.ForEach(_ =>
            {

                _.DailyList.ForEach(d =>
                {
                    d.ItemList.ForEach(i =>
                    {
                        if (i.SupplierType == (int)HJD.HotelServices.Contracts.HotelServiceEnums.SupplierType.RoomSupplier)
                        {
                            
                            i.NotVIPPrice = i.Price;
                            i.VIPPrice = CalPriceForCtrip(hotelCtripPricePolicy, i.Price, i.PayType);

                            if (ppt == HotelServiceEnums.PricePolicyType.VIP)
                            {
                                i.Price = i.VIPPrice;
                                i.Rebate = CalRebatForCtrip(i.Price, i.PayType);
                            }

                            sb.AppendFormat("Date:{0} notVIP:{1}  VIP:{2}", d.Day, i.NotVIPPrice, i.VIPPrice);
                        }
                    });
                    d.NotVIPPrice = d.ItemList.Sum(i => i.NotVIPPrice);
                    d.Price = d.ItemList.Sum(i => i.Price);
                    d.VIPPrice = d.ItemList.Sum(i => i.VIPPrice);

                    d.Rebate = d.ItemList.Sum(i => i.Rebate);
                });

                _.NotVIPPrice = _.DailyList.Sum(d => d.NotVIPPrice);
                _.Price = _.DailyList.Sum(d => d.Price);
                _.VIPPrice = _.DailyList.Sum(d => d.VIPPrice);

                _.Rebate = _.DailyList.Sum(d => d.Rebate);
            });

         //   LogHelper.WriteLog(sb.ToString());
        }

    }
}
