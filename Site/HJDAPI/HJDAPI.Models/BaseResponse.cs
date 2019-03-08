using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.Models
{
    [DataContract]
    public class BaseResponse
    {
        private ResponseSuccessState mSuccess = ResponseSuccessState.Success;
        private string mMessage = "成功！";

        public void SignError()
        {
            Success = ResponseSuccessState.SignError;
        }

        public void NoPhoneError()
        {
            Success = ResponseSuccessState.NoPhoneNum;

        }

        public bool IsSuccess
        {
            get
            {
                return Success ==  ResponseSuccessState.Success;
            }
        }

        [DataMember]
        public ResponseSuccessState Success
        {
            get
            {
                return mSuccess;
            }
            set
            {
                mSuccess = value;
                switch(value)
                {
                    case  ResponseSuccessState.Success:
                        mMessage = "成功！";
                        break;
                    case  ResponseSuccessState.SignError:
                        mMessage = "签名错误！";
                        break;
                    case  ResponseSuccessState.NoPhoneNum:
                        mMessage = "手机号为空，缺少手机号！";
                        break;
                }

            }
        }

        [DataMember]
        public string Message {
            get
            {
                return mMessage;
            }
            set
            {
                mMessage = value;
            }
        }

        public enum ResponseSuccessState
        {
            Success = 0,
            Failed = 1,   //删除失败

            //EACard
            EACard_NoCardInfoError = 2,//"暂无此卡号信息！"
            EACard_UserIdentityError = 3, //"用户身份验证失败！"
            EACard_RepeatAddCardError = 4,//"已经添加过东航卡！"
            EACard_InputValidateError = 5,//"姓名中不能包含中文！"

            NoPhoneNum = 80, //没有手机号
            SignError = 100, //签名错误
            ParamError = 200, //参数错误

            //券
            RelCouponHasUsed = 302, //"关联券码已使用，该券码不可单独作取消"
            Coupon_GiftCanBeRefundSolely = 303,  // "赠送券不能单独退款。"
            Coupon_NoInfo = 304,// "没有找到相应的券信息。"
            VIP_UsedCouponOnRefund = 305,//新VIP现金券礼已使用，不能取消VIP
            Coupon_SubCouponCannotRefund  = 306, //子产品不能退款

            //返现
            Rebat_NoOrderID = 400, //"您的订单号不存在，请重新输入。";
            Rebat_CannotRepeatApply = 402,// "该订单号已申请过，无需重复申请，谢谢！";
            Rebat_HasRebat = 403,//"您的订单已返现，请查收。";
            Rebat_NotPaied = 404,//"您的订单尚末付款，请付款后再申请返现，谢谢！";
            Rebat_NoRebat = 405,//"您好，你输入的订单没有返现，谢谢！";
            Rebat_OrderNotConfirmed = 406, //"您好，您的订单尚末完成酒店确认，请确认后再申请返现，谢谢！";

            //订单
            Order_CannotPay = 501, //  "订单不能支付！"

            //Points
            Points_ConsumFailed = 600,  //扣积分失败

            //现金券
            CashCoupon_PresentFailed = 700,


            //支付前检测
            BeforePay_Coupon_OverTime = 801,//                        res.Message = "支付时间已经超过10分钟！";
            BeforePay_Coupon_OverTime_StateChange = 802, //"支付时间超时，支付状态已改变！";
            BeforePay_Coupon_ParamsError = 803,//"参数错误！";
            BeforePay_Coupon_CashCouponError = 804,//"关联的现金券已不可用！";
            BeforePay_Coupon_FundError = 805,//"住基金不足！";
            BeforePay_Coupon_VoucherError = 806,//"关联的代金券已不可用！";
            BeforePay_Coupon_Other = 810, // "其他错误"; 
            BeforePay_Coupon_BookOver = 820, // "预约已满"; 
        }
    }
}