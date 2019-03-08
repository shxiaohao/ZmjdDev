using HJD.Framework.WCF;
using HJD.HotelManagementCenter.Domain.Fund;
using HJD.HotelManagementCenter.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.Controllers.Adapter
{
    public  class FundAdapter
    {
        public static IFundService fundSvc = ServiceProxyFactory.Create<IFundService>("IFundService");


        //  住基金明细类型定义：
        //1、用户注册（被推荐人注册）
        //2、订单奖励（被推荐人下单）
        //3、订单扣款
        //4、订单退款
        //5、现付返现
        //6、1来5往
        //public enum FundType 
        //{
        //    UserRegister = 1,
        //    OrderAward = 2,
        //    OrderDeduct = 3,
        //    OrderRefund = 4,
        //    FontPayRebat = 5,
        //    Active1to5 = 6
        //}


        public static int AddUserFund(UserFundIncomeDetail fund)
        {
            return fundSvc.AddUserFund(fund);
        }

        public static int ReduceUserFund(UserFundExpendDetailEntity fund)
        {
            return fundSvc.ReduceUserFund(fund);
        }


        /// <summary>
        /// 获得userId的住房基金
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
   
        public static UserFundEntity GetUserFundInfo(Int64 userId)
        {
            return fundSvc.GetUserFundInfo(userId);
        }

        public static List<UserFundIncomeStatEntity> GetUserFundIncomeStat(Int64 userId)
        {
            return fundSvc.GetUserFundIncomeStat(userId);
        }

        public static List<UserFundExpendDetailEntity> GetUserFundExpendDetail(Int64 userId)
        {
            return fundSvc.GetUserFundExpendDetail(userId);
        }
         
    }
}
