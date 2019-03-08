using HJD.Framework.WCF;
using HJD.HotelManagementCenter.Domain;
using HJD.HotelManagementCenter.IServices;
using HJDAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace HJDAPI.Controllers
{
    public class EasternAirLinesController : BaseApiController
    {
        static IOrderService OrderService = ServiceProxyFactory.Create<IOrderService>("IOrderService");

        [System.Web.Http.HttpGet]
        public int GetEasternAirLinesPoints(string userID)
        {
            if (!string.IsNullOrEmpty(userID))
            {
                EasternAirLinesCardEntity model = OrderService.GetPointsPreview(userID);
                if (model != null)
                {
                    return int.Parse(model.Points.ToString());
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return 0;
            }
        }

        [System.Web.Http.HttpGet]
        public EasternAirLinesCardEntity GetPointsPreview(string userID)
        {
            if (!string.IsNullOrEmpty(userID))
            {
                EasternAirLinesCardEntity model = OrderService.GetPointsPreview(userID);
                if (model != null)
                {
                    return model;
                }
                else
                {
                    return new EasternAirLinesCardEntity();
                }
            }
            else
            {
                return new EasternAirLinesCardEntity();
            }
        }

        [System.Web.Http.HttpGet]
        public List<EasternAirLinesRecordEntity> GetPointsList(string userID)
        {
            if (!string.IsNullOrEmpty(userID))
            {
                List<EasternAirLinesRecordEntity> List = OrderService.GetPointsList(userID);
                if (List != null && List.Count > 0)
                {
                    return List;
                }
                else
                {
                    return new List<EasternAirLinesRecordEntity>();
                }
            }
            else
            {
                return new List<EasternAirLinesRecordEntity>();
            }
        }

        //[HttpPost]
        [System.Web.Http.HttpPost]
        public BaseResponse AddEACard(EasternAirLinesCardParam modelAdd)
        {
            BaseResponse response = new BaseResponse();
            if (modelAdd.UserID <= 0)
            {
                response.Success = HJDAPI.Models.BaseResponse.ResponseSuccessState.EACard_UserIdentityError;
                response.Message = "用户身份验证失败！";
            }
            else
            {
                if (!ContainChinese(modelAdd.PreName) && !ContainChinese(modelAdd.OtherName))
                {
                    if (OrderService.CheckExistCardByUserID(modelAdd.UserID) == 0)//判断是否已经有卡
                    {
                        if (!string.IsNullOrEmpty(modelAdd.AccountNO) && (CheckCardNO(modelAdd.AccountNO).Success == BaseResponse.ResponseSuccessState.Success))
                        {
                            EasternAirLinesCardEntity model = new EasternAirLinesCardEntity();
                            model.AccountNO = modelAdd.AccountNO.Trim();
                            model.AccountType = 1;//默认卡种
                            model.CreateTime = DateTime.Now;
                            model.Description = "";
                            model.PreName = modelAdd.PreName.Trim();
                            model.OtherName = modelAdd.OtherName.Trim();
                            model.States = 0;//未校验   1：校验中   2：校验通过   3：校验失败   4：删除
                            model.UserID = long.Parse(modelAdd.UserID.ToString());
                            int optResult = OrderService.AddEasternAirLinesCard(model);
                            if (optResult >= 0)
                            {
                                response.Success = HJDAPI.Models.BaseResponse.ResponseSuccessState.Success;// 1;
                                response.Message = "添加成功！";
                                int res = OrderService.UpdateEARecordAccountNOByID(optResult.ToString(), modelAdd.UserID);
                            }
                            else
                            {
                                response.Success = HJDAPI.Models.BaseResponse.ResponseSuccessState.Failed;// 0;
                                response.Message = "添加失败！";
                            }
                        }
                        else
                        {
                            response.Success = HJDAPI.Models.BaseResponse.ResponseSuccessState.EACard_NoCardInfoError;// 2;
                            response.Message = "卡号格式不正确！";
                        }
                    }
                    else
                    {
                        response.Success = HJDAPI.Models.BaseResponse.ResponseSuccessState.EACard_RepeatAddCardError;// 4;
                        response.Message = "已经添加过东航卡！";
                    }
                }
                else
                {
                    response.Success = HJDAPI.Models.BaseResponse.ResponseSuccessState.EACard_InputValidateError;// 5;
                    response.Message = "姓名中不能包含中文！";
                }
            }
            return response;
        }


        public static bool ContainChinese(string input)
        {
            string pattern = "[\u4e00-\u9fbb]";
            return Regex.IsMatch(input, pattern);
        }

        [System.Web.Http.HttpGet]
        public BaseResponse ChangeEACard(string ID)
        {
            BaseResponse response = new BaseResponse();
            EasternAirLinesCardEntity modelold = OrderService.FindAllEasternAirLinesCardByID(int.Parse(ID));
            if (modelold != null)
            {
                EasternAirLinesCardEntity model = new EasternAirLinesCardEntity();
                model.ID = modelold.ID;
                model.AccountNO = modelold.AccountNO;
                model.AccountType = modelold.AccountType;
                model.CreateTime = modelold.CreateTime;
                model.Description = modelold.Description ?? "";
                model.PreName = modelold.PreName;
                model.OtherName = modelold.OtherName;
                model.States = 4;//删除
                model.UserID = modelold.UserID;
                model.UpdateTime = DateTime.Now;
                int optResult = OrderService.UpdateEasternAirLinesCard(model);
                if (optResult >= 0)
                {
                    response.Success = HJDAPI.Models.BaseResponse.ResponseSuccessState.Success;// 1;
                    response.Message = "删除成功！";
                }
                else
                {
                    response.Success = HJDAPI.Models.BaseResponse.ResponseSuccessState.Failed; // 0;
                    response.Message = "删除失败！";
                }
            }
            else
            {
                response.Success = HJDAPI.Models.BaseResponse.ResponseSuccessState.EACard_NoCardInfoError;
                response.Message = "暂无此卡号信息！";
            }
            return response;
        }

        [System.Web.Http.HttpGet]
        public BaseResponse CheckCardNO(string cardNO)
        {
            BaseResponse response = new BaseResponse();
            bool checkRes = true;
            string checkkey = cardNO.Substring(1, 1);
            string cardcheck = cardNO.Substring(0, 1) + "0" + cardNO.Substring(2);
            if (!string.IsNullOrEmpty(cardNO))
            {
                if (cardNO.Length != 12)
                {
                    response.Success = 0;
                    response.Message = "卡号为12位数字！";
                    checkRes = false;
                }
                else if (cardNO.Substring(0, 1) != "6")
                {
                    response.Success = 0;
                    response.Message = "卡号为数字6开头！";
                    checkRes = false;
                }

                else if (long.Parse(cardcheck) % 7 < 7 && (long.Parse(cardcheck) % 7).ToString() != checkkey)
                {
                    response.Success = 0;
                    response.Message = "校验不通过！";
                    checkRes = false;
                }
            }
            if (checkRes)
            {
                response.Success = BaseResponse.ResponseSuccessState.Success;// 1;
                response.Message = "校验通过！";
            }
            return response;
        }
    }
}
