using System;
using HJDAPI.Common.Helpers;
using HJD.AccountServices.Contracts;
using HJD.AccountServices.Entity;
using HJD.Framework.WCF;
using System.Web;
using HJDAPI.Models;
using HJD.HotelManagementCenter.IServices;
using HJDAPI.Controllers.Common;
using System.Collections.Generic;
using System.Linq;
using HJD.Framework.Interface;
using HJD.CouponService.Contracts.Entity;
using HJDAPI.Controllers.Adapter;
using System.Linq;
using HJD.HotelServices.Contracts;
using MessageService.Contract;
using Newtonsoft.Json;

namespace HJDAPI.Controllers
{
    public class AccountAdapter
    {
        public static IAccountService AccService = ServiceProxyFactory.Create<IAccountService>("BasicHttpBinding_IAccountService");
        public static IPrivilegeManager PrivilegeService = ServiceProxyFactory.Create<IPrivilegeManager>("IPrivilegeManager");
        public static ISMSService SMSService = ServiceProxyFactory.Create<ISMSService>("ISMSService");
        public static ICacheProvider LocalCache10Min = CacheManagerFactory.Create("DynamicCacheFor10Min");
        public static HJD.HotelServices.Contracts.IHotelService HotelService = ServiceProxyFactory.Create<HJD.HotelServices.Contracts.IHotelService>("BasicHttpBinding_IHotelService");
        public static HJD.CouponService.Contracts.ICouponService couponService = ServiceProxyFactory.Create<HJD.CouponService.Contracts.ICouponService>("ICouponService");


        public static int AddUserChannelRelHistory(UserChannelRelHistoryEntity user_channelrelhistory)
        {
            return AccService.AddUserChannelRelHistory(user_channelrelhistory);
        }

        public static int AddReceivePeopleInformation(ReceivePeopleInformationEntity param)
        {
            return AccService.AddReceivePeopleInformation(param);
        }

        public static int UpdateReceivePeopleInformation(ReceivePeopleInformationEntity param)
        {
            return AccService.UpdateReceivePeopleInformation(param);
        }
        public static ReceivePeopleInformationEntity GetReceivePeopleInformationById(int id)
        {
            return AccService.GetReceivePeopleInformationById(id);
        }

        public static List<ReceivePeopleInformationEntity> GetReceivePeopleInformationByUserId(long userId)
        {
            return AccService.GetReceivePeopleInformationByUserId(userId).Where(_ => _.State == 0).ToList();
        }

        public static List<UserChannelRelHistoryEntity> GetChannelBelongToUserList(long cid)
        {
            return AccService. GetChannelBelongToUserList( cid);
        }

        public static List<UserBindTypeEntity> GetUserBindTypeList()
        {
            return AccService.GetUserBindTypeList();
        }

        public static bool IsVIPCustomer(HJDAPI.Common.Helpers.Enums.CustomerType type)
        {
            if (type == HJDAPI.Common.Helpers.Enums.CustomerType.vip
                || type == HJDAPI.Common.Helpers.Enums.CustomerType.vip199
                 || type == HJDAPI.Common.Helpers.Enums.CustomerType.vip199nr
                || type == HJDAPI.Common.Helpers.Enums.CustomerType.vip599
                 || type == HJDAPI.Common.Helpers.Enums.CustomerType.vip3M
                 || type == HJDAPI.Common.Helpers.Enums.CustomerType.vip6M)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static HJDAPI.Common.Helpers.Enums.CustomerType TransforCustomerTypeForShowVIPPrice(HJDAPI.Common.Helpers.Enums.CustomerType type)
        {
            if (type == HJDAPI.Common.Helpers.Enums.CustomerType.vip
                || type == HJDAPI.Common.Helpers.Enums.CustomerType.vip199
                || type == HJDAPI.Common.Helpers.Enums.CustomerType.vip199nr
                || type == HJDAPI.Common.Helpers.Enums.CustomerType.vip599
                || type == HJDAPI.Common.Helpers.Enums.CustomerType.vip3M
                || type == HJDAPI.Common.Helpers.Enums.CustomerType.vip6M)
            {
                return HJDAPI.Common.Helpers.Enums.CustomerType.vip;
            }
            else
            {
                return type;
            }
        }


        //  public enum UserPriviledge { BoTaoUser = 2001, MagiCallUser = 2002, MemberPrice = 2003, InnerTest = 2004 }
        public enum UserRoleEnum { VIP = 12, VIP199 = 16, VIP599 = 17, VIP3M = 18, VIP6M = 19, VIP199NR = 22 }

        //public enum UserRoleEnumFull
        //{
        //    普通 = 0,
        //    客服 = 1,
        //    管理员 = 2,
        //    结算 = 3,
        //    套餐管理 = 4,
        //    点评提炼 = 5,
        //    特色标签管理 = 6,
        //    品鉴师管理 = 7,
        //    编辑 = 8,
        //    酒店 = 9,
        //    点评 = 10,
        //    销售 = 11,
        //    VIP = 12,
        //    测试组 = 13,
        //    销售主管 = 14,
        //    财务主管 = 15,
        //    金牌VIP = 16,
        //    铂金VIP = 17,
        //    金牌VIP3M = 18,
        //    金牌VIP6M = 19,
        //    新金牌VIP = 22
        //}

        /// <summary>
        /// 根据渠道类型和渠道ID获取用户第三方绑定记录
        /// </summary>
        /// <param name="userchannelrel">（如微信：Channel=weixin Code=unionid）</param>
        /// <returns></returns>
        public static UserChannelRelEntity GetOneUserChannelRel(UserChannelRelEntity userchannelrel)
        {
            return AccService.GetOneUserChannelRel(userchannelrel);
        }

        /// <summary>
        /// 根据渠道类型和用户ID获取用户第三方绑定记录
        /// </summary>
        /// <param name="userchannelrel">（如微信：Channel=weixin）</param>
        /// <returns></returns>
        public static UserChannelRelEntity GetOneUserChannelRelByUID(UserChannelRelEntity userchannelrel)
        {
            return AccService.GetOneUserChannelRelByUID(userchannelrel);
        }

        /// <summary>
        /// 新增用户第三方绑定关系
        /// </summary>
        /// <param name="userchannelrel"></param>
        /// <returns></returns>
        public static int AddUserChannelRel(UserChannelRelEntity userchannelrel)
        {
            return AccService.AddUserChannelRel(userchannelrel);
        }

        public static bool AddUserCommInfo(UserCommInfoEntity info)
        {
            try
            {
                // Log.WriteLog("AddUserCommInfo");
                AccService.AddUserCommInfo(info);
            }
            catch (Exception err)
            {
                Log.WriteLog(err.Message + err.StackTrace);
                return false;
            }
            return true;
        }

        public static HJDAPI.Common.Helpers.Enums.CustomerType GetCustomerType(long userId)
        {
            Enums.CustomerType customerType = Enums.CustomerType.general;
            if (userId > 0)
            {
                if (AccountAdapter.HasUserPriviledge(userId, HJD.AccountServices.Entity.PrivilegeEnums.UserPrivilege.MemberPrice))
                {
                    var roleList = GetUserRole(userId);
                    if (roleList.Any(p => p.RoleID == (int)UserRoleEnum.VIP))
                    {
                        customerType = (Enums.CustomerType.vip);
                    }
                    else if (roleList.Any(p => p.RoleID == (int)UserRoleEnum.VIP199))
                    {
                        customerType = (Enums.CustomerType.vip199);
                    }
                    else if (roleList.Any(p => p.RoleID == (int)UserRoleEnum.VIP199NR))
                    {
                        customerType = (Enums.CustomerType.vip199nr);
                    }
                    else if (roleList.Any(p => p.RoleID == (int)UserRoleEnum.VIP599))
                    {
                        customerType = (Enums.CustomerType.vip599);
                    }
                    else if (roleList.Any(p => p.RoleID == (int)UserRoleEnum.VIP3M))
                    {
                        customerType = (Enums.CustomerType.vip3M);
                    }
                    else if (roleList.Any(p => p.RoleID == (int)UserRoleEnum.VIP6M))
                    {
                        customerType = (Enums.CustomerType.vip6M);
                    }
                }
                //else if (AccountAdapter.HasUserPriviledge(userId, HJD.AccountServices.Entity.PrivilegeEnums.UserPrivilege.BoTaoUser))
                //{
                //    customerType = Enums.CustomerType.botao;
                //}
                else if (AccountAdapter.IsInspector(userId))
                {
                    customerType = Enums.CustomerType.inspector;
                }
            }

            return customerType;
        }


        //public static List<HJDAPI.Common.Helpers.Enums.CustomerType> GetCustomerTypeList(long userId)
        //{
        //    List<Enums.CustomerType> customerTypeList = new List<Enums.CustomerType>();
        //    if (AccountAdapter.HasUserPriviledge(userId, AccountAdapter.UserPriviledge.MemberPrice))
        //    {
        //        var roleList = GetUserRole(userId);
        //        if (roleList.Any(p => p.RoleID == (int)UserRoleEnum.VIP))
        //        {
        //            customerTypeList.Add(Enums.CustomerType.vip);
        //        }
        //        else if (roleList.Any(p => p.RoleID == (int)UserRoleEnum.VIP199 ))
        //        {
        //            customerTypeList.Add(Enums.CustomerType.vip199);
        //        }
        //        else if (roleList.Any(p => p.RoleID == (int)UserRoleEnum.VIP599))
        //        {
        //            customerTypeList.Add(Enums.CustomerType.vip599);
        //        }
        //        else if (roleList.Any(p =>  p.RoleID == (int)UserRoleEnum.VIP3M  ))
        //        {
        //            customerTypeList.Add(Enums.CustomerType.vip3M);
        //        }
        //        else if (roleList.Any(p =>  p.RoleID == (int)UserRoleEnum.VIP6M  ))
        //        {
        //            customerTypeList.Add(Enums.CustomerType.vip6M);
        //        }
        //    }
        //    if (AccountAdapter.HasUserPriviledge(userId, AccountAdapter.UserPriviledge.BoTaoUser))
        //    {
        //        customerTypeList.Add(Enums.CustomerType.botao);
        //    }
        //    if (AccountAdapter.IsInspector(userId))
        //    {
        //        customerTypeList.Add(Enums.CustomerType.inspector);
        //    }

        //    return customerTypeList;
        //}


        public static bool HasUserPriviledge(long userid, HJD.AccountServices.Entity.PrivilegeEnums.UserPrivilege userPri)
        {
            var result = PrivilegeService.GetAllPrivilegeByUserId(userid);
            return result != null ? result.Any(p => p.Code == (int)userPri) : false;
        }

        public static bool HasUserRole(long userid, UserRoleEnum userRole)
        {
            var result = PrivilegeService.GetUserRoleRelByUserId(userid);
            return result != null ? result.Any(p => p.RoleID == (int)userRole) : false;
        }

        public static List<UserRole> GetUserRole(long userid)
        {
            return PrivilegeService.GetUserRoleRelByUserId(userid);
        }

        public static List<HJD.AccountServices.Entity.UserRole> GetUserRoleRelByUserId(long userid)
        {
            return PrivilegeService.GetUserRoleRelByUserId(userid);
        }

        public static int UserRoleRelDelete(long userID, int roleID)
        {
            return PrivilegeService.UserRoleRelDelete(userID, roleID);
        }

        /// <summary>
        /// 铂涛：2001 magiccall：2002
        /// isAdd: true增加 false: 删除
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="priviledgeID"></param>
        /// <param name="isAdd"></param>
        /// <returns></returns>
        public static bool InsertOrDeleteUserPrivilegeRel(long userid, int priviledgeID, bool isAdd, HJD.HotelManagementCenter.Domain.OpLogBizType bizType, long bizID, string mark)
        {
            PrivilegeService.UserPrivilegeRelInsertWithLog(userid, priviledgeID, isAdd, (int)bizType, bizID, mark);
            if (isAdd && priviledgeID == (int)PrivilegeEnums.UserPrivilege.CanBuyVIPFirstPackage) //新VIP发券
            {
                  CouponAdapter.PresentNewVIPGift(userid);        
            }           
            return true;
        }

        public static List<UserCommInfoEntity> GetUserCommInfo(GetUserCommInfoReqParm p)
        {

            long key = p.UserID;
            int IDType = 1;
            if (p.UserID == 0)
            {
                var u = GetPhoneUser(p.Phone); //手机  先试一下有没有对应的帐号，如果有帐号，用帐号来取
                if (u != null && u.UserId > 0)
                {
                    key = u.UserId;
                    IDType = 1;
                }
                else
                {
                    key = long.Parse(p.Phone);
                    IDType = 2;
                }
            }

            List<UserCommInfoEntity> list = AccService.GetUserCommInfo(key, IDType);

            if (p.InfoType == 1000) //用户发票信息
            {

                return list.Where(l => l.State != 3 && (l.InfoType == 1 || l.InfoType == 2 || l.InfoType == 3)).ToList();
            }
            else
            {
                return list;
            }
        }


        public static UserCommInvoiceInfoModel GetUserCommInvoiceInfo(GetUserCommInfoReqParm p, string AppVer = "")
        {

            long key = p.UserID;
            int IDType = 1;
            if (p.UserID == 0)
            {
                var u = GetPhoneUser(p.Phone); //手机  先试一下有没有对应的帐号，如果有帐号，用帐号来取
                if (u != null && u.UserId > 0)
                {
                    key = u.UserId;
                    IDType = 1;
                }
                else
                {
                    key = long.Parse(p.Phone);
                    IDType = 2;
                }
            }
            //List<UserCommInfoEntity> list = AccService.GetUserCommInfo(4511973, 1);

            List<UserCommInfoEntity> list = AccService.GetUserCommInfo(key, IDType);
            UserCommInvoiceInfoModel ucomm = new UserCommInvoiceInfoModel();

            ucomm.invoiceType = AccountAdapter.invoiceType(p.HotelId, p.Pid, AppVer, p.PackageType);
            ucomm.ShippingType = new List<string> { "圆通快递" };
            ucomm.Describe = "提示：发票金额不包括住基金和现金券抵扣部分。\n注：当前版本开发票无效，请使用最新版App(6.2.1)开发票";
            ucomm.InvoiceDateDescribe = "入住完成后，1-5个工作日开具。\n（规则适用于入住完成前发起开票申请，若入住完成后申请开具，则计提交申请后1-5个工作日开具。）";
            if (p.InfoType == 1000) //用户发票信息
            {
                foreach (var u in list.Where(l => l.State != 3 && (l.InfoType == 1 || l.InfoType == 2 || l.InfoType == 3 || l.InfoType == 6)).ToList())
                {
                    switch (u.InfoType)
                    {
                        case 1:
                            ucomm.Title.Add(u.Info);
                            break;
                        case 2:
                            ucomm.Address.Add(u.Info);
                            break;
                        case 3:
                            ucomm.ContactPeople.Add(u.Info);
                            break;
                        //case 4:
                        //    ucomm.ShippingType.Add(u.Info);
                        //    break;
                        case 5:
                            ucomm.TelPhone.Add(u.Info);
                            break;
                        case 6:
                            ucomm.TaxNumber.Add(u.Info);
                            break;
                    }
                }
                //list.Where(l => l.State != 3 && (l.InfoType == 1 || l.InfoType == 2 || l.InfoType == 3)).ToList();

                return ucomm;
            }
            else
            {
                foreach (var u in list)
                {
                    switch (u.InfoType)
                    {
                        case 1:
                            ucomm.Title.Add(u.Info);
                            break;
                        case 2:
                            ucomm.Address.Add(u.Info);
                            break;
                        case 3:
                            ucomm.ContactPeople.Add(u.Info);
                            break;
                        //case 4:
                        //    ucomm.ShippingType.Add(u.Info);
                        //    break;
                        case 5:
                            ucomm.TelPhone.Add(u.Info);
                            break;
                        case 6:
                            ucomm.TaxNumber.Add(u.Info);
                            break;
                    }
                }
                return ucomm;
            }
        }

        public static InvoiceParamEntity GetInvoiceInfo(GetUserCommInfoReqParm param, string appVer = "")
        {
            int needPoints = 50;//发票需要积分

            long key = param.UserID;
            int IDType = 1;
            if (param.UserID == 0)
            {
                var u = GetPhoneUser(param.Phone); //手机  先试一下有没有对应的帐号，如果有帐号，用帐号来取
                if (u != null && u.UserId > 0)
                {
                    key = u.UserId;
                    IDType = 1;
                }
                else
                {
                    key = long.Parse(param.Phone);
                    IDType = 2;
                }
            }

            List<UserCommInfoEntity> list = AccService.GetUserCommInfo(key, IDType);
            InvoiceParamEntity model = new InvoiceParamEntity();


            model.Description = "关于电子/纸质普通发票、专用发票的说明";
            model.DescriptionURL = "http://www.zmjiudian.com/active/activepage?pageid=133";//_isshare=1 右上角是否显示分享按钮
            model.PaperInvoicePoints = needPoints;
            model.PaperInvoicePointsDesc = "可用"+ needPoints + "积分抵扣";
            model.PaperInvoicePrice = 10;
            model.ShippingType = new List<string> { "圆通快递" };

            model.Title = new List<string>();
            model.InvoicePrintType = new List<TextValueEntity>();

            model.Tips = "提示：发票金额不包括住基金和现金券抵扣部分\n\n按照国税总局公告，自2017年7月1日起，企业索取的增值税普通发票需要填写纳税人识别号，不符合规定的发票，不得作为合法税收凭证。";


            model.InvoiceType = AccountAdapter.invoiceType(param.HotelId, param.Pid, appVer, param.PackageType);

            foreach (var u in list.Where(l => l.State != 3).ToList())
            {
                switch (u.InfoType)
                {
                    case 1:
                        model.Title.Add(u.Info);
                        break;
                }
            }
            
            List<PointsEntity> listPoints = PointsAdapter.GetUserPointsList(param.UserID);
            int canTotalPoints = listPoints.Sum(_ => _.LeavePoint);
            if (needPoints < canTotalPoints)
            {
                model.IsCanPoint = true;
            }

            //获取收件人信息
            List<ReceivePeopleInformationEntity> receivePeopleInformationList = AccountAdapter.GetReceivePeopleInformationByUserId(param.UserID);
            ReceivePeopleInformationEntity receivePeopleLastSelect = receivePeopleInformationList.Where(_ => _.IsLastSelected == true).FirstOrDefault();

            if (receivePeopleLastSelect == null)
            {
                receivePeopleLastSelect = receivePeopleInformationList.FirstOrDefault();
            }
            if (receivePeopleLastSelect != null)
            {
                model.Contact = receivePeopleLastSelect.ReceiveName;
                model.Address = receivePeopleLastSelect.ReceiveAddress;
                model.TelPhone = receivePeopleLastSelect.ReceivePhone;
            }

            TextValueEntity textvalue = new TextValueEntity() { Text = "增值税普通发票", TextValue = "1" };
            model.InvoicePrintType.Add(textvalue);

            //获取上次填写的邮箱

            var lastInvoice = SettlementAdapter.GetInvoiceEntityByUserId(param.UserID);
            if (lastInvoice != null && lastInvoice.Email != null)
            {
                model.Email = lastInvoice.Email;
            }


            return model;
        }


        public static List<InvoiceType> invoiceType(int hotelid, int pid, string appVer, int packageType = 1)
        {
            Dictionary<int, string> dicInvoice = new Dictionary<int, string>();
            if (dicInvoice.Count == 0)
            {
                if (string.IsNullOrWhiteSpace(appVer) || appVer.CompareTo("5.1") < 0)
                {
                    dicInvoice.Add((int)Enums.InvoiceType.代订房费, Enums.InvoiceType.代订房费.ToString());
                    dicInvoice.Add((int)Enums.InvoiceType.旅游费, Enums.InvoiceType.旅游费.ToString());
                    dicInvoice.Add((int)Enums.InvoiceType.会务费, Enums.InvoiceType.会务费.ToString());
                }
                else if (packageType == 4 || packageType == 6)//非周某酒店套餐默认开代订房费
                {
                    dicInvoice.Add((int)Enums.InvoiceType.代订房费, Enums.InvoiceType.代订房费.ToString());
                }
                else
                {
                    HotelContactPackageEntity hc = HotelService.GetHotelContactPackageByHotelIDAndPid(hotelid, pid);
                    if (hc.InvoiceType == 2 && hc.SupplierID == 0)
                    {
                        dicInvoice.Add((int)Enums.InvoiceType.代订房费, Enums.InvoiceType.代订房费.ToString());
                        dicInvoice.Add((int)Enums.InvoiceType.旅游费, Enums.InvoiceType.旅游费.ToString());
                        dicInvoice.Add((int)Enums.InvoiceType.会务费, Enums.InvoiceType.会务费.ToString());
                    }
                    else
                    {
                        dicInvoice.Add((int)Enums.InvoiceType.旅游费, Enums.InvoiceType.旅游费.ToString());
                        dicInvoice.Add((int)Enums.InvoiceType.代订房费, Enums.InvoiceType.代订房费.ToString());
                    }
                }
            }
            List<InvoiceType> invoicetype = new List<InvoiceType>();
            foreach (var c in dicInvoice)
            {
                InvoiceType invoiceEntity = new InvoiceType();
                invoiceEntity.TypeId = c.Key;
                invoiceEntity.TypeName = c.Value.ToString();
                if (string.IsNullOrWhiteSpace(appVer) || appVer.CompareTo("5.5") < 0)
                {
                    switch (c.Key)
                    {
                        case (int)Enums.InvoiceType.代订房费:
                        case (int)Enums.InvoiceType.会务费:
                        case (int)Enums.InvoiceType.旅游费:
                            invoiceEntity.TypeDescribe = "（1-5个工作日开具）";
                            break;
                        case (int)Enums.InvoiceType.住宿费:
                            invoiceEntity.TypeDescribe = "（需协助酒店开具，7-20个工作日开具）";
                            break;
                    }
                }
                invoicetype.Add(invoiceEntity);
            }

            return invoicetype;
        }

        //public static List<UserCommInfoEntity> GetUserInvoiceTitleAddreeInfo(long userID,int IDType)
        //{

        //    if (IDType == 2) //手机  先试一下有没有对应的帐号，如果有帐号，用帐号来取
        //    {
        //        var u = GetPhoneUser( userID.ToString());
        //        if (u != null && u.UserId > 0)
        //        {
        //            userID = u.UserId;
        //            IDType = 1;
        //        }
        //    }

        //    List<UserCommInfoEntity> list = AccService.GetUserCommInfo(userID, IDType); ;

        //   return list.Where(l => l.State != 3 && (l.InfoType == 1 || l.InfoType == 2 || l.InfoType == 3)).ToList();
        //}

        /// <summary>
        /// 昵称可用：没有人用过，昵称没有包括特殊词
        /// </summary>
        /// <param name="nickName"></param>
        /// <returns></returns>
        public static ResultEntity CheckNickName(string nickName, long userID)
        {
            ResultEntity r = new ResultEntity();
            if (string.IsNullOrWhiteSpace(nickName))
            {
                r.Success = 3;
                r.Message = "昵称不能为空";
            }
            else
            {
                bool b = AccService.CheckNickNameOrProfileUrlExists(nickName, userID);
                int maxWordLength = 15;
                if (b)
                {
                    r.Success = 1;
                    r.Message = "昵称不可用！";
                }
                else if (nickName.Length > maxWordLength)
                {
                    r.Success = 2;
                    r.Message = "昵称最多" + maxWordLength + "个字符";
                }
                else
                {
                    r.Success = 0;
                    r.Message = "昵称可用！";
                }
            }
            return r;
        }

        public static ResultEntity UpdateNickName(string nickName, long userID, string password)
        {
            ResultEntity r = new ResultEntity();

            r.Success = 0;
            r.Message = "更新成功！";

            //现在系统中增加了验证码登陆，客户端可能没有密码，这样密码的确证是会通不过的。。。在新的解决方案出来前，先去掉密码的验证环节，以签名来保证吧
            //if (!CheckUserIDAndPassword(userID, password))
            //{
            //    r.Success = 2;
            //    r.Message = "密码不正确！";
            //}
            //else
            {
                ResultEntity u = CheckNickName(nickName, userID);

                if (u.Success == 0)
                {
                    MemberProfileInfo mpi = GetCurrentUserInfo(userID);

                    SP_UpdateMemberProfileParam s = new SP_UpdateMemberProfileParam();
                    s.NickName = nickName;
                    s.UserID = userID;

                    s.MemberBrief = string.IsNullOrWhiteSpace(mpi.MemberBrief) ? "" : System.Web.HttpUtility.UrlEncode(mpi.MemberBrief.Trim(), System.Text.Encoding.UTF8);

                    int i = AccService.UpdateMemberProfile(s);
                    if (i != 0)
                    {
                        r.Success = 1;
                        r.Message = "昵称更新失败！";
                    }
                }
                else
                {
                    r.Success = u.Success;
                    r.Message = u.Message;
                }
            }

            return r;
        }

        /// <summary>
        /// 向手机发送验证短信
        /// </summary>
        /// <param name="phoneNum"></param>
        /// <returns></returns>
        public static bool sendConfirmSMS(string phoneNum)
        {
            return SMSService.sendConfirmSMS(phoneNum);
        }

        /// <summary>
        /// 向手机发送新密码
        /// </summary>
        /// <param name="phoneNum"></param>
        /// <returns></returns>
        public static bool sendNewPasswordSMS(string phoneNum)
        {
            return SMSService.sendNewPasswordSMS(phoneNum);
        }

        /// <summary>
        /// 验证手机验证码是否正确
        /// </summary>
        /// <param name="phoneNum"></param>
        /// <returns></returns>
        public static bool checkConfirmSMS(string phoneNum, string confirmCode)
        {
            return AccService.checkConfirmSMS(phoneNum, string.IsNullOrWhiteSpace(confirmCode) ? "" : confirmCode);
        }

        /// <summary>
        /// 验证邀请码是否正确
        /// </summary>
        /// <param name="phoneNum"></param>
        /// <returns></returns>
        public static bool checkInvitationCode(string InvitationCode)
        {
            return AccService.checkInvitationCode(InvitationCode);
        }

        /// <summary>
        /// 获取userid 根据 验证邀
        /// </summary>
        /// <param name="phoneNum"></param>
        /// <returns></returns>
        public static User_Info GetUserIDByInvitationCode(string InvitationCode)
        {
            return AccService.GetUserInfoByInvitationCode(InvitationCode);
        }

        #region 安缦渠道短信通道

        public static bool sendConfirmSMSAnman(string phoneNum)
        {
            return SMSService.sendConfirmSMSAnman(phoneNum);
        }

        public static bool checkConfirmSMSAnman(string phoneNum, string confirmCode)
        {
            return AccService.checkConfirmSMSAnman(phoneNum, string.IsNullOrWhiteSpace(confirmCode) ? "" : confirmCode);
        }

        #endregion

        public static OperationResult Login(AccountInfoItem ai)
        {
            var result = new OperationResult();

            try
            {
                IAccountService AccService = ServiceProxyFactory.Create<IAccountService>("BasicHttpBinding_IAccountService");

                HJD.AccountServices.Entity.LoginResult serviceReturn;
                LoginRequest regrequest = new LoginRequest();
                regrequest.LoginID = ai.Email;
                regrequest.Password = ai.Password;
                regrequest.ClientIP = ai.RemoteAddress;

                serviceReturn = AccService.Login(regrequest);

                result.Success = serviceReturn.RetCode.StartsWith("TS");
                result.Message = result.Success ? "" : serviceReturn.RetCode;
                result.Email = ai.Email;




                //var data = string.Format("EncEmail={0}&EncPassword={1}&RememberMe={2}&Key={3}",
                //        ai.Email,
                //        ai.Password,
                //        ai.AutoLogin,
                //        Configs.Key);
                //var cookies = new CookieContainer();
                //var json = AccountAdapter.Userlogin(ai.Email, ai.Password, ai.AutoLogin);  //HttpRequestHelper.Post(Configs.LoginUrl, data, ref cookies);
                //var res = JsonConvert.DeserializeObject<HJDAPI.Models.LoginResult>(json);
                //result.Success = res.Errormsg.StartsWith("T") || res.Errormsg.StartsWith("TA01|") || res.Errormsg.StartsWith("TA02|") || res.Errormsg.StartsWith("TA03|");
                //result.Message = res.Errormsg;

                if (result.Success)
                {
                    long userid = 0;
                    try
                    {
                        userid = long.Parse(serviceReturn.RetCode.Substring(serviceReturn.RetCode.LastIndexOf("|") + 1));
                    }
                    catch
                    {
                        userid = 0;
                    }
                    string nickname = string.Empty;
                    if (userid > 0)
                    {
                        var u = AccountAdapter.GetCurrentUserInfo(userid);

                        if (u != null)
                        {
                            nickname = u.NickName;
                            new BaseApiController().SetAuthorizeHead(userid, ai.Password, ai.Email);
                        }
                    }
                    string retcode = serviceReturn.RetCode;
                    string md5 = retcode.Substring(retcode.IndexOf("|") + 1, retcode.LastIndexOf("|") - retcode.IndexOf("|") - 1);
                    result.Email = ai.Email;
                    result.Data = userid + "|" + StringHelper.UidMask(nickname) + "|" + md5;   //使用正确的昵称
                }

            }
            catch (Exception e)
            {
                result.Data = e.Message;
            }
            return result;
        }

        public static OperationResult ModifyPassword(ModifyPasswordItem r)
        {
            if (r.newpassword.Length < 6)
            {
                return new OperationResult() { Success = false, Message = "密码长度不能小于6位。" };
            }
            else if (AccountAdapter.CheckUserIDAndPassword(r.userid, r.oldpassword))
            {
                AccountAdapter.UpdatePasswordByUserId(r.userid.ToString(), r.newpassword, r.updateIP);
                return new OperationResult() { Success = true, Message = "密码更新成功！" };
            }
            else
            {
                return new OperationResult() { Success = false, Message = "原密码错误" };
            }
        }

        public static bool CheckUserIDAndPassword(long userId, string password)
        {
            return AccService.CheckUserIDAndPassword(userId, password);

        }

        public static int UpdatePasswordByUserId(string userId, string password, string updateIp)
        {
            return AccService.UpdatePasswordByUserId(userId, password, updateIp);

        }

        public static int UpdateMobileByUserId(long userid, string newpohone)
        {
            Log.WriteLog(string.Format("UpdateMobileByUserId:{0} {1}", userid, newpohone));
            return AccService.UpdateMobileByUserId(userid, newpohone);
        }

        //public static User_Info GetOrRegisterPhoneUser(string nickName, string phone)
        //{
        //   User_Info ui = AccService.GetUserInfoByEMail(phone + PhoneUserEmail);

        //   if (ui == null)
        //   {
        //       AccountsController ac = new AccountsController();

        //       OperationResult r = RegisterPhoneUser(nickName, phone, GenPassword());

        //       ui = AccService.GetUserInfoByEMail(r.Email);
        //   }

        //   return ui;
        //}

        public static User_Info GetPhoneUser(string phone)
        {
            User_Info userInfo = AccService.GetUserInfoByMobile(phone);
            if (userInfo != null)
            {
                userInfo.AvatarUrl = string.IsNullOrWhiteSpace(userInfo.AvatarUrl) ? DescriptionHelper.defaultAvatar :
                        PhotoAdapter.GenHotelPicUrl(userInfo.AvatarUrl, Enums.AppPhotoSize.jupiter);
            }
            return userInfo;
        }

        /// <summary>
        /// 通过手机号获取用户信息，如果手机号对应的用户不存在，则相应的生成一个帐号，手机号为末验证状态。
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        public static User_Info GetOrRegistPhoneUser(string phone, long CID = 0)
        {
            User_Info ui = GetPhoneUser(phone);

            if (ui == null || ui.UserId == 0)
            {
                OperationResult u = RegisterPhoneUser(phone, "", 0, CID:CID);
                ui = GetPhoneUser(phone);
            }

            return ui;
        }

        public static string GenPassword()
        {
            PasswordGenerator pg = new PasswordGenerator();
            pg.Maximum = 10;
            pg.Minimum = 6;
            return pg.Generate();
        }


        public static string GenInvitationCode()
        {
            PasswordGenerator pg = new PasswordGenerator();
            pg.Maximum = 6;
            pg.Minimum = 4;
            return pg.Generate();
        }

        /// <summary>
        /// 用手机号注册用户
        /// 如果手机号已存在，那么只需要更新手机状态和密码
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="password"></param>
        /// <param name="mobileState"></param>
        /// <param name="InvitationCode"></param>
        /// <param name="NickName"></param>
        /// <param name="isTemporaryPWD"></param>
        /// <param name="CID"></param>
        /// <param name="unionid"></param>
        /// <returns></returns>
        public static OperationResult RegisterPhoneUser(string phone, string password, int mobileState, long InvitationCode = 0, string NickName = "", bool isTemporaryPWD = false, long CID = 0, string unionid = "")
        {
            User_Info ui = GetPhoneUser(phone);

            if (ui == null || ui.UserId == 0)
            {
                AccountInfoItem item = new AccountInfoItem();

                item.IsMobile = 1;
                item.Phone = phone;
                item.MobileState = mobileState;
                item.NickName = NickName;
                item.FriendUserId = InvitationCode;
                item.IsTemporaryPWD = isTemporaryPWD;
                item.CID = CID;
                item.Uid = unionid;

                if (password == "") //对于手机短信验证的用户密码会为空。 如果密码为空，那么给个随机密码
                {
                    password = GenPassword();
                }

                item.Password = password;

                return Register(item);
            }
            else //更新手机状态和密码
            {
                AccService.UpdateUserMobileState(ui.UserId, mobileState);
                AccService.UpdatePasswordByUserId(ui.UserId.ToString(), password, "", isTemporaryPWD);

                var result = new OperationResult(); // TODO
                result.Success = true;

                new BaseApiController().SetAuthorizeHead(ui.UserId, password, "");
                MemberProfileInfo member = GetCurrentUserInfo(ui.UserId);

                HJD.AccountServices.Entity.MobileLoginResult serviceReturn = AccService.MobileLogin(new LoginRequest { LoginID = phone, Password = password, AccountType = AccountType.WHMOBILE });
                string retcode = serviceReturn.RetCode;
                string md5 = "";
                try
                {
                    md5 = retcode.Substring(retcode.IndexOf("|") + 1, retcode.LastIndexOf("|") - retcode.IndexOf("|") - 1);
                }
                catch (Exception ex)
                {
                    Log.WriteLog("取md5值失败，" + ex.Message + ex.StackTrace);
                }

                result.Data = ui.UserId + "|" + StringHelper.UidMask(member.NickName) + "|" + md5;
                result.Email = "";

                result.HelloWord = "你好，" + StringHelper.UidMask(member.NickName);
                result.HelloTip = "旅行休闲，又好又划算";

                result.Avatar = member.AvatarUrl;//头像
                result.UserID = ui.UserId;
                result.IsTemporaryPWD = ui.IsTemporaryPWD;
                return result;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="mobileState"></param>
        /// <param name="CID"></param>
        /// <param name="unionid"></param>
        /// <returns></returns>
        public static OperationResult RegisterWithoutPassword(string phone, int mobileState = 1, long  CID = 0, string unionid = "")
        {
            User_Info ui = GetPhoneUser(phone);
            if (ui == null || ui.UserId == 0)
            {
                AccountInfoItem item = new AccountInfoItem();

                item.IsMobile = 1;
                item.Phone = phone;
                item.MobileState = mobileState;
                item.NickName = "";

                item.Password = GenPassword();
                item.CID = CID;
                item.Uid = unionid;
                return Register(item);
            }
            else
            {
                return new OperationResult()
                {
                    Mobile = phone,
                    UserID = ui.UserId,
                    Success = true,
                    Message = "",
                    Avatar = "",
                    Data = "",
                    Email = ""
                };
            }
        }

        public static OperationResult Register(AccountInfoItem item)
        {
            string email = item.Email;
            string mobile = item.Phone;
            int mobileState = item.MobileState;
            string nickname = item.NickName;
            string password = item.Password;
            string remoteAddress = item.RemoteAddress;
            var result = new OperationResult();

            string serviceReturn = "";
            RegisterRequest regrequest = new RegisterRequest();
            regrequest.Nickname = nickname;
            regrequest.Email = email;
            regrequest.Mobile = mobile;
            regrequest.MobileState = mobileState;
            regrequest.Password = password;
            regrequest.RemoteAddress = remoteAddress;
            regrequest.IsMobile = item.IsMobile == 1 ? true : false;
            regrequest.FriendUserId = item.FriendUserId;

            regrequest.IsTemporaryPWD = item.IsTemporaryPWD;

            serviceReturn = AccService.Register(regrequest);

            result.Success = serviceReturn.StartsWith("TA01");
            result.Message = result.Success ? "" : serviceReturn;

            long userid = 0;
            try
            {
                userid = long.Parse(serviceReturn.Substring(serviceReturn.LastIndexOf("|") + 1));
            }
            catch
            {
                userid = 0;
            }

            if (result.Success)
            {
                new BaseApiController().SetAuthorizeHead(userid, password, email);
                MemberProfileInfo member = GetCurrentUserInfo(userid);
                string keyId = serviceReturn.Substring(serviceReturn.IndexOf("|") + 1, serviceReturn.LastIndexOf("|") - serviceReturn.IndexOf("|") - 1);
                result.Data = userid + "|" + StringHelper.UidMask(member.NickName) + "|" + keyId;
                result.Email = email;
                result.UserID = userid;
                result.Avatar = member.AvatarUrl;
                result.IsTemporaryPWD = item.IsTemporaryPWD;

                result.HelloWord = "你好，" + StringHelper.UidMask(member.NickName);
                result.HelloTip = "旅行休闲，又好又划算";

                //插入积分
                RegisterInsertPonit(userid);

                #region 微信活动粉丝的CID关联

                //检查当前用户在注册之前是否有成为别人的活动粉丝，如果有，则设置为那个人的CID关系
                UserFansRel fansInfo = string.IsNullOrEmpty(item.Uid) ? null : GetOneFansRelByUnionid(item.Uid);
                if (fansInfo != null && fansInfo.UserID > 0)
                {
                    AddUserChannelRelHistory(new UserChannelRelHistoryEntity
                    {
                        ChangeType = (int)AccountEnums.ChangeType.Registe,
                        CID = fansInfo.UserID,
                        CreateTime = DateTime.Now,
                        UserID = userid,
                        RelBizInfo = ""
                    });
                }

                #endregion

                #region 常规CID或邀请注册的关联

                else
                {
                    AddUserChannelRelHistory(new UserChannelRelHistoryEntity
                    {
                        ChangeType = item.FriendUserId > 0 ? (int)AccountEnums.ChangeType.Invite : (int)AccountEnums.ChangeType.Registe,
                        CID = item.CID,
                        CreateTime = DateTime.Now,
                        UserID = userid,
                        RelBizInfo = item.FriendUserId > 0 ? item.FriendUserId.ToString() : ""
                    });
                }

                #endregion
            }

            return result;
        }

        //public OperationResult GenRegisterInfo()
        //{
        //    var result = new OperationResult();
        //    result.Success = true;
        //    new BaseApiController().SetAuthorizeHead(userid, password, email);
        //    MemberProfileInfo member = AccService.GetMemberProfileInfoByUserId(userid);
        //    string keyId = serviceReturn.Substring(serviceReturn.IndexOf("|") + 1, serviceReturn.LastIndexOf("|") - serviceReturn.IndexOf("|") - 1);
        //    result.Data = userid + "|" + StringHelper.UidMask(member.NickName) + "|" + keyId;
        //    result.Email = email;

        //    return result;
        //}

        public static long GetUserid(string uid)
        {
            if (string.IsNullOrEmpty(uid))
                return 0;
            var ub = AccService.GetUserBind(uid, 2);
            if (ub != null && ub.UserID > 0)
            {
                return ub.UserID;
            }
            long d;
            long.TryParse(uid.ToLower().Replace("_lp_", string.Empty), out d);

            return d;
        }

        //public static  OperationResult MobileLogin(AccountInfoItem ai)
        //{
        //    var result = new OperationResult();

        //    try
        //    {
        //        IAccountService AccService = ServiceProxyFactory.Create<IAccountService>("BasicHttpBinding_IAccountService");

        //        HJD.AccountServices.Entity.MobileLoginResult serviceReturn;
        //        LoginRequest regrequest = new LoginRequest();
        //        regrequest.LoginID = ai.Phone;
        //        regrequest.Password = ai.Password;
        //        regrequest.ClientIP = ai.RemoteAddress;

        //        serviceReturn = AccService.MobileLogin(regrequest);

        //        result.Success = serviceReturn.RetCode.StartsWith("TS");
        //        result.Message = result.Success ? "" : serviceReturn.Message;
        //        result.Mobile = ai.Phone;
        //        result.Email = "";


        //        if (result.Success)
        //        {
        //            long userid = 0;
        //            try
        //            {
        //                userid = long.Parse(serviceReturn.RetCode.Substring(serviceReturn.RetCode.LastIndexOf("|") + 1));
        //            }
        //            catch
        //            {
        //                userid = 0;
        //            }
        //            string nickname = string.Empty;
        //            if (userid > 0)
        //            {
        //                var u = AccountAdapter.GetCurrentUserInfo(userid);

        //                if (u != null)
        //                {
        //                    nickname = u.NickName;
        //                    SetAuthorizeHead(userid, ai.Password, ai.Email);
        //                }
        //            }
        //            string retcode = serviceReturn.RetCode;
        //            string md5 = retcode.Substring(retcode.IndexOf("|") + 1, retcode.LastIndexOf("|") - retcode.IndexOf("|") - 1);
        //            result.Email = "";
        //            result.Mobile = ai.Phone;
        //            result.Data = userid + "|" + StringHelper.UidMask(nickname) + "|" + md5;   //使用正确的昵称

        //            HotelAdapter.BindUserAccountAndOrders(userid, ai.Phone); //关联帐号与手机号下的订单，以便关联用户在退出状态下的订单
        //        }

        //    }
        //    catch (Exception e)
        //    {
        //        result.Data = e.Message;
        //    }
        //    return result;
        //}

        public static string GetNickName(long userId)
        {
            var m = AccService.GetMemberProfileInfoByUserId(userId);
            if (m == null || m.UserID < 0) return string.Empty;
            return StringHelper.UidMask(m.NickName);
        }

        public static bool NotRegistMobileAccount(string phone)
        {
            User_Info ui = GetPhoneUser(phone);

            return !(ui == null || ui.UserId == 0 || ui.MobileState == 0 || ui.MobileState == 1);
        }

        public static bool ExistsMobileAccount(string phone)
        {
            User_Info ui = GetPhoneUser(phone);

            return !(ui == null);
        }

        public static string GetNickName(string uid)
        {
            var userid = GetUserid(uid);
            return GetNickName(userid);
        }

        public static MemberProfileInfo GetCurrentUserInfo(long userid)
        {
            try
            {
                var m = AccService.GetMemberProfileInfoByUserId(userid);
                if (m != null && m.UserID > 0)
                {
                    m.AvatarUrl = string.IsNullOrWhiteSpace(m.AvatarUrl) ? DescriptionHelper.defaultAvatar :
                        PhotoAdapter.GenHotelPicUrl(m.AvatarUrl, Enums.AppPhotoSize.jupiter);

                    m.IsInspector = IsInspector(userid);

                    m.MemberBrief = System.Web.HttpUtility.UrlDecode(m.MemberBrief, System.Text.Encoding.UTF8);

                    m.ThemeCodeSN = string.IsNullOrWhiteSpace(m.ThemeCodeSN) ? "" : m.ThemeCodeSN;

                    return m;
                }
                else
                {
                    return new MemberProfileInfo();
                }
            }
            catch(Exception err)
            {
                Log.WriteLog("GetCurrentUserInfo:" + userid.ToString() + err.Message + err.StackTrace);
                return new MemberProfileInfo();
            }
        }

        //public static string Userlogin (string email, string password, bool autologin)
        //{            
        //    string returnvalues = "";
        //    HJD.AccountServices.Entity.LoginRequest lr = new LoginRequest();
        //    lr.AccountType = AccountType.WHOTEL;

        //    //设置登录通道
        //    if (email.IndexOf("@") == -1)
        //    {
        //        returnvalues = "{\"uid\":\"\",\"errormsg\":\"FA01|你的Email不符合条件\"}";
        //    }

        //    lr.LoginID = email;
        //    lr.Password = password;
        //    lr.ClientIP = "-1.-1.-1.-1";

        //    lr.Cookies = "";

        //    HJD.AccountServices.Entity.LoginResult lrt = new HJD.AccountServices.Entity.LoginResult();
        //    lrt = AccService.Login(lr);

        //    var returnCode = lrt.RetCode;
        //    var mpi = lrt.MemberProfileInfo;
        //    returnCode = string.IsNullOrEmpty(returnCode) ? "F" : returnCode;
        //    if (mpi == null || returnCode.StartsWith("F") || (returnCode.StartsWith("T") && mpi.UserID <= 0))
        //    {
        //        returnvalues = "{\"uid\":\"\",\"errormsg\":\"FA01|你的Email和密码不符，请再试一次\"}";
        //    }
        //    else
        //    {
        //        string bindingUid; 
        //        HJD.AccountServices.Entity.UserBind ub = new UserBind();
        //        ub = AccService.GetUserBindByUserIdAndType(mpi.UserID, 2);
        //        if (ub != null && ub.UserID > 0)
        //        {
        //            bindingUid = ub.BindUserName;
        //        }
        //        else
        //            bindingUid = "_lp_" + mpi.UserID.ToString();


        //        returnvalues = "{\"uid\":\""+ bindingUid +"\",\"errormsg\":\"TA01|\"}";
        //    }

        //    return returnvalues;
        //}

        /// <summary>
        /// 获取当前用户的帐号
        /// </summary>
        /// <returns></returns>
        public static long GetCurrentUserID()
        {
            if (HttpContext.Current.Request.Headers["WH-UserId"] != null)
                return long.Parse(HttpContext.Current.Request.Headers["WH-UserId"]);
            else
                return 0;
        }

        public static Inspector GetInspector(long userid, string identityCode, string mobilePhone = null)
        {
            return AccService.GetInspector(userid, identityCode, mobilePhone);
        }

        public static bool IsInspector(long userid)
        {
            Inspector ee = GetInspector(userid, null);
            if (ee.UserID > 0 && ee.State == 2)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// -1:没提交过;0:资格未通过;1:资格通过了
        /// 特殊状态:7  活动通过 虽然不是品鉴师但是运行申请品鉴酒店
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public static int IsInspectorEx(long userid)
        {
            Inspector ee = GetInspector(userid, null);
            if (ee == null || (ee != null && ee.UserID == 0))
            {
                return -1;
            }
            else if (ee.UserID > 0 && ee.State == 2)
            {
                return 1;
            }
            else if (ee.State == 7)
            {
                return 7;
            }
            else
            {
                return 0;
            }
        }

        public static List<User_Info> GetUserInfoZmjiudian()
        {
            return AccService.GetUserInfoZmjiudian();
        }

        public static List<UserRightRole> GetUserRightRole()
        {
            return PrivilegeService.GetUserRightRole();
        }

        /// <summary>
        /// 更新用户头像
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="avatarSUrl"></param>
        /// <returns></returns>
        public static int UploadUserAvatar(long userID, string avatarSUrl)
        {
            return AccService.UploadUserAvatar(userID, avatarSUrl);
        }

        /// <summary>
        /// 更新关注状态
        /// </summary>
        /// <param name="follower">粉丝UserID</param>
        /// <param name="following">被关注者UserID</param>
        /// <param name="IsValid">关注状态有效无效</param>
        /// <returns></returns>
        public static int UpdateFollowerFollowingRel(long follower, long following, bool isValid)
        {
            return AccService.UpdateFollowerFollowingRel(follower, following, isValid);
        }

        /// <summary>
        /// 新增关注
        /// </summary>
        /// <param name="follower">粉丝UserID</param>
        /// <param name="following">被关注者UserID</param>
        /// <returns></returns>
        public static int AddFollowerFollowingRel(long follower, long following)
        {
            return AccService.AddFollowerFollowingRel(follower, following);
        }

        public static int UpdatePersonalSignature(long userID, string personalSignature)
        {
            return AccService.UpdatePersonalSignature(userID, personalSignature);
        }

        /// <summary>
        /// 获取粉丝列表
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public static List<FollowerFollowingRelEntity> GetFollowersByUserID(long userID, int start, int count)
        {
            return AccService.GetFollowersByUserID(userID, start, count);
        }

        /// <summary>
        /// 获取关注列表
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public static List<FollowerFollowingRelEntity> GetFollowingsByUserID(long userID, int start, int count)
        {
            return AccService.GetFollowingsByUserID(userID, start, count);
        }

        /// <summary>
        /// 获取指定用户粉丝数量
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public static int GetFollowersCountByUserID(long userID)
        {
            return AccService.GetFollowersCountByUserID(userID);
        }

        /// <summary>
        /// 获取指定用户关注数目
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public static int GetFollowingsCountByUserID(long userID)
        {
            return AccService.GetFollowingsCountByUserID(userID);
        }

        /// <summary>
        /// 0：没关系 1：userID1是userID2的粉丝 2：userID2是userID1的粉丝 3：相互关注
        /// </summary>
        /// <param name="userID1"></param>
        /// <param name="userID2"></param>
        /// <returns></returns>
        public static int GetFollowFollowingRelState(long userID1, long userID2)
        {
            return AccService.GetFollowFollowingRelState(userID1, userID2);
        }

        /// <summary>
        /// 更新品鉴师描述和图片
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="desPicUrl"></param>
        /// <returns></returns>
        public static int UpdateInspectorDesPicSurl(long userID, string desPicUrl)
        {
            return AccService.UpdateInspectorDesPicSurl(userID, desPicUrl);
        }

        /// <summary>
        /// 获取某个状态的Inspector列表
        /// </summary>
        /// <returns></returns>
        private static List<Inspector> GetInspectorList(InspectorSearchParam param, out int count)
        {
            return AccService.GetInspectorList(param, out count);
        }

        /// <summary>
        /// 获取首页推荐关注的品鉴师列表 4.1版本
        /// </summary>
        /// <returns></returns>
        public static List<RecommendedInspectorModel> GetAppHomeInspectorList(InspectorSearchParam param, out int count, long curUserID)
        {
            List<Inspector> list = GetInspectorList(param, out count);
            if (list != null && list.Count != 0)
            {
                List<RecommendedInspectorModel> inspectorModelList = new List<RecommendedInspectorModel>();
                foreach (var item in list)
                {
                    MemberProfileInfo mpi = GetCurrentUserInfo(item.UserID);
                    if (mpi.AvatarUrl == DescriptionHelper.defaultAvatar)
                    {
                        mpi.AvatarUrl = DescriptionHelper.defaultInspectorAvatar;
                    }

                    inspectorModelList.Add(new RecommendedInspectorModel()
                    {
                        AvatarUrl = mpi.AvatarUrl,
                        Brief = mpi.MemberBrief,//item.Job,(使用个性签名)
                        CoverPicUrl = mpi.AvatarUrl,
                        FollowersCount = item.FollowersCount,//GetFollowersCountByUserID(item.UserID),
                        FollowingsCount = 0,//GetFollowingsCountByUserID(item.UserID),
                        FollowFollowingState = GetFollowFollowingRelState(item.UserID, curUserID),
                        NickName = mpi.NickName,
                        UserID = item.UserID
                    });
                }
                return inspectorModelList;
            }
            else
            {
                return new List<RecommendedInspectorModel>();
            }
        }

        /// <summary>
        /// 更新主题封面
        /// </summary>
        /// <param name="theme"></param>
        /// <returns></returns>
        public static int UpdateUserThemeCover(User_Info_Theme theme)
        {
            return AccService.UpdateUserThemeCover(theme);
        }

        public static User_Info GetUserInfoByUserId(long userID)
        {
            return AccService.GetUserInfoByUserId(userID);
        }

        public static IEnumerable<User_Info> GetUserBasicInfo(IEnumerable<long> userIdList)
        {
            return AccService.GetUserBasicInfo(userIdList);
        }

        public static IEnumerable<FollowerFollowingRelEntity> GetManyFollowerFollowings(IEnumerable<long> ffRelIds)
        {
            return AccService.GetManyFollowerFollowings(ffRelIds);
        }

        //public static BasePostResponse RegisterUser4BT(string mobile, int mobileState, string password = "")
        //{
        //    User_Info userInfo = AccService.GetUserInfoByMobile(mobile);
        //    if (userInfo == null || userInfo.UserId == 0)
        //    {
        //        AccountInfoItem item = new AccountInfoItem();

        //        item.IsMobile = 1;
        //        item.Phone = mobile;
        //        item.MobileState = mobileState;
        //        item.NickName = "";

        //        if (password == "")//对于手机短信验证的用户密码会为空。 如果密码为空，那么给个随机密码
        //        {
        //            password = GenPassword();
        //        }
        //        item.Password = password;

        //        OperationResult result = Register(item);

        //        //ToDo 增加userID来源的绑定


        //        return new BasePostResponse()
        //        {
        //            data = mobile,
        //            errorCode = "0",
        //            errormsg = "同步数据成功",
        //            result = true
        //        };
        //    }
        //    else
        //    {
        //        //ToDo 如果已经是我们的客户 判断是不是铂涛金融产品的用户来了

        //        return new BasePostResponse()
        //        {
        //            data = mobile,
        //            errorCode = "1000",
        //            errormsg = "已经同步过了",
        //            result = false
        //        };
        //    }
        //}

        #region 用户标签相关接口

        /// <summary>
        /// 增加tag记录
        /// </summary>
        /// <param name="userTags"></param>
        /// <returns></returns>
        public static int InsertUserTagRel(IEnumerable<UserTagRelEntity> userTags)
        {
            return AccService.InsertUserTagRel(userTags);
        }

        /// <summary>
        /// 获取用户的userTag(包括自定义和预定义的)
        /// </summary>
        /// <param name="userIds"></param>
        /// <returns></returns>
        public static List<UserTagRelEntity> GetUserTagRel(IEnumerable<long> userIds)
        {
            return AccService.GetUserTagRel(userIds);
        }

        /// <summary>
        /// 获得默认用户tag列表
        /// </summary>
        /// <returns></returns>
        public static List<UserTagOption> GetUserTagOptionList()
        {
            var userTagOption = new List<UserTagOption>();
            var defaultTags = GetDefaultUserTag();
            foreach (var item in defaultTags.GroupBy(_ => new { _.TypeId, _.TypeName }))
            {
                var typeId = item.Key.TypeId;
                var typeName = item.Key.TypeName;
                if (typeId == 3)
                {
                    userTagOption.Add(genUserTagOption(0, 3, typeId, typeName, item.ToList()));
                }
                else
                {
                    userTagOption.Add(genUserTagOption(0, typeId == 1 ? 1 : 3, typeId, typeName, item.ToList(), true));
                }
            }
            return userTagOption;
        }

        private static UserTagOption genUserTagOption(int minCount, int maxCount, int typeId, string typeName, List<UserTagRelEntity> tags, bool isAllowAdd = false)
        {
            if (isAllowAdd)
            {
                tags.Add(new UserTagRelEntity()
                {
                    TypeId = typeId,
                    TagID = 0,
                    TagName = "自定义",
                    TypeName = typeName,
                    UserID = 0
                });
            }

            return new UserTagOption()
            {
                MaxCount = maxCount,
                MinCount = minCount,
                TypeId = typeId,
                TypeName = typeName,
                Tags = tags
            };
        }

        /// <summary>
        /// 获取缺省的userTag
        /// </summary>
        /// <returns></returns>
        private static List<UserTagRelEntity> GetDefaultUserTag()
        {
            return AccService.GetDefaultUserTag();
        }

        #endregion

        #region 权限的获取

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="roleid"></param>
        /// <returns></returns>
        public static int UserRoleRelInsert(long userid, int roleid)
        {
            return PrivilegeService.UserRoleRelInsert(userid, roleid);
        }

        /// <summary>
        /// 批量增加/删除权限
        /// </summary>
        /// <param name="userIds"></param>
        /// <param name="privilegeId"></param>
        /// <param name="isAdd"></param>
        /// <returns></returns>
        public static int UpdateUserPrivilegeRel4ManyUserId(IEnumerable<long> userIds, int privilegeId, bool isAdd)
        {
            return PrivilegeService.UpdateUserPrivilegeRel4ManyUserId(userIds, privilegeId, isAdd);
        }

        /// <summary>
        /// 批量增加删除角色
        /// </summary>
        /// <param name="userIds"></param>
        /// <param name="roleId"></param>
        /// <param name="isAdd"></param>
        /// <returns></returns>
        public static int UpdateUserRoleRel4ManyUserId(IEnumerable<long> userIds, int roleId, bool isAdd)
        {
            return PrivilegeService.UpdateUserRoleRel4ManyUserId(userIds, roleId, isAdd);
        }

        #endregion

        #region 由用户权限列表 生成 权限code数组
        public static List<string> GetUserPrivilegeNames(IEnumerable<UserPrivilegeRel> privilegeRels)
        {
            if (privilegeRels != null && privilegeRels.Any())
            {
                return privilegeRels.Select(_ => _.PrivilegeName).ToList();
            }
            return new List<string>();
        }
        #endregion


        public static string GetCustomerTypeDescribe(int userType)
        {
            string CustomerType = "";
            switch (userType)
            {
                case (int)HJDAPI.Common.Helpers.Enums.CustomerType.vip:
                    CustomerType = "VIP会员";
                    break;
                case (int)HJDAPI.Common.Helpers.Enums.CustomerType.vip199:
                case (int)HJDAPI.Common.Helpers.Enums.CustomerType.vip199nr:
                case (int)HJDAPI.Common.Helpers.Enums.CustomerType.vip3M:
                case (int)HJDAPI.Common.Helpers.Enums.CustomerType.vip6M:
                    CustomerType = "金牌VIP会员";
                    break;
                case (int)HJDAPI.Common.Helpers.Enums.CustomerType.vip599:
                    CustomerType = " 铂金VIP会员";
                    break;
            }
            return CustomerType;
        }


        internal static Boolean IsVIPCustomer(long curUserID)
        {
            return (Boolean)LocalCache10Min.GetData<Object>("IsVIPCustomer:" + curUserID.ToString(), () =>
            {
                return IsVIPCustomer(GetCustomerType(curUserID));
            });
        }


        internal static UserInfoResult GetUserInfo(long userid)
        {
            var result = new UserInfoResult();
            string nickname = string.Empty;
            var u = AccountAdapter.GetCurrentUserInfo(userid);
            if (u != null)
            {
                result.UserID = userid;
                result.Email = u.EmailAccount == null ? "" : u.EmailAccount;

                nickname = u.NickName;
                //默认头像地址
                result.Avatar = u.AvatarUrl;
                result.NickName = u.NickName;
                result.FollowingsCount = AccountAdapter.GetFollowingsCountByUserID(userid);
                result.FollowersCount = AccountAdapter.GetFollowersCountByUserID(userid);
                result.PersonalSignature = u.MemberBrief;//个性签名
                //result.ThemeCodeSN = u.ThemeCodeSN;//个人主题
                //  result.Email = ""; ;

                result.RealName = u.RealName;//真实姓名
                result.InvitationCode = u.InvitationCode;//邀请码
                result.Mobile = u.MobileAccount;//手机号


                List<PointsEntity> pointsList = HotelController.HotelService.GetPointsEntity(userid);

                result.CanUsePoints = pointsList.Sum(i => i.LeavePoint);

                result.SaveMoney = u.SaveMoney;

                int CustomerType = (int)AccountAdapter.GetCustomerType(userid);
                result.CustomerTypeDescribe = AccountAdapter.GetCustomerTypeDescribe(result.CustomerType);
                switch (CustomerType)
                {
                    case (int)HJDAPI.Common.Helpers.Enums.CustomerType.vip:
                        result.CustomerTypeInterests = "终身VIP权益";
                        result.CustomerType = 2;
                        break;
                    case (int)HJDAPI.Common.Helpers.Enums.CustomerType.vip199:
                    case (int)HJDAPI.Common.Helpers.Enums.CustomerType.vip199nr:
                    case (int)HJDAPI.Common.Helpers.Enums.CustomerType.vip3M:
                    case (int)HJDAPI.Common.Helpers.Enums.CustomerType.vip6M:
                        result.CustomerTypeInterests = "VIP会员专区";
                        result.CustomerType = (int)HJDAPI.Common.Helpers.Enums.CustomerType.vip199nr;
                        break;
                    case (int)HJDAPI.Common.Helpers.Enums.CustomerType.vip599:
                        result.CustomerTypeInterests = "VIP会员专区";
                        result.CustomerType = (int)HJDAPI.Common.Helpers.Enums.CustomerType.vip599;
                        break;
                    default:
                        result.CustomerTypeInterests = "成为VIP";
                        result.CustomerType = (int)HJDAPI.Common.Helpers.Enums.CustomerType.general;
                        break;
                }

                result.StartVipTime = Convert.ToDateTime("1900-01-01");
                result.EndVipTime = Convert.ToDateTime("1900-01-01");

                //购买VIP时间
                List<ExchangeCouponEntity> exchangeVip = CouponAdapter.GetExchangeCouponEntityListByUser(userid, (int)(HJDAPI.Common.Helpers.Enums.CouponType.VIP));
                if (exchangeVip.Count > 0)
                {
                    //2表示VIP购买成功的状态
                    if (exchangeVip.Exists(_ => _.State == 2))
                    {
                        var buyVIPCoupon =  exchangeVip.Where(_ => _.State == 2).OrderByDescending(_ => _.CreateTime).FirstOrDefault();
                        result.StartVipTime = buyVIPCoupon.CreateTime;
                        result.VIPCID = buyVIPCoupon.CID;

                    }
                    //8是用户VIP过期的状态，当没有购买成功的VIP录时，则查询最后一条过期的记录
                    else if (exchangeVip.Exists(_ => _.State == 8))
                    {
                        result.StartVipTime = exchangeVip.Where(_ => _.State == 8).OrderByDescending(_ => _.CreateTime).FirstOrDefault().CreateTime;
                    }

                    switch (CustomerType)
                    {
                        case (int)HJDAPI.Common.Helpers.Enums.CustomerType.vip:
                            result.EndVipTime = Convert.ToDateTime("3000-01-01");
                            break;
                        case (int)HJDAPI.Common.Helpers.Enums.CustomerType.vip199:
                        case (int)HJDAPI.Common.Helpers.Enums.CustomerType.vip199nr:
                        case (int)HJDAPI.Common.Helpers.Enums.CustomerType.vip599:
                            result.EndVipTime = result.StartVipTime.AddYears(1);
                            break;
                        case (int)HJDAPI.Common.Helpers.Enums.CustomerType.vip3M:
                            result.EndVipTime = result.StartVipTime.AddMonths(3);
                            break;
                        case (int)HJDAPI.Common.Helpers.Enums.CustomerType.vip6M:
                            result.EndVipTime = result.StartVipTime.AddMonths(6);
                            break;
                        default:
                            result.EndVipTime = Convert.ToDateTime("1900-01-01");
                            break;
                    }
                }
            }
            return result;
        }

        internal static int UpdateRealName(string realName, long userId)
        {
            SP_UpdateMemberProfileParam spUserInfoEx = new SP_UpdateMemberProfileParam();
            spUserInfoEx.UserID = userId;
            spUserInfoEx.RealName = realName;
            return AccService.UpdateUserInfoExRealName(spUserInfoEx);
        }
        internal static void InvitationCodeJoinZmjdGetCoupon(long UserId, string phone = "", long sourceId = 0)
        {
            CouponTypeDefineEntity ctd = couponService.GetCouponTypeDefineByCode(CouponActivityCode.cashcoupon50);
            //发现金券（注册类型，50rmb）
            OriginCouponResult ocr = couponService.GenerateOriginCoupon3(UserId, ctd.Type, phone, sourceId);
            if (ocr.Success == 1)
            {
                Log.WriteLog(string.Format("发放现金券失败：UserId_{0},Type_{1},Phone_{2},sourceId_{3}", UserId, ctd.Type, phone, sourceId));
            }
        }
        internal static void InvitationCodeRegisterInsertPonit(long userid,int points,int type)
        {

            PointsEntity pe = new PointsEntity()
            {
                BusinessID = 0,
                LeavePoint = points,
                TotalPoint = points,
                TypeID = type,//12注册得到积分
                UserID = userid,
                Approver = 0
            };
            try
            {
                HotelAdapter.HotelService.InsertOrUpdatePoints(pe);
                MessageAdapter.InsertSysMessage(new SysMessageEnitity()
                {
                    state = 0,
                    businessID = userid,
                    businessType = (int)HJDAPI.Common.Helpers.Enums.SysMessigeType.Register,
                    receiver = userid
                });
                string ms="获邀注册成功，我们已将" + points + "奖励积分存放在您的钱包中。";
                if (type == 19)
                {
                    ms = "邀请注册成功，我们已将" + points + "奖励积分存放在您的钱包中。";
                }
                MessageAdapter.SendAppNotice(new SendNoticeEntity()
                {
                    actionUrl = "",
                    appType = 0,
                    from = "周末酒店",
                    msg = ms,
                    noticeType = ZMJDNoticeType.register,
                    title = "周末酒店",
                    userID = userid
                });
            }
            catch (Exception e)
            {
                Log.WriteLog("注册插入积分报错：userid" + userid + " " + e.Message + "\r\n" + e.StackTrace);
                throw e;
            }
        }

        internal static void RegisterInsertPonit(long userid)
        {

            PointsEntity pe = new PointsEntity()
            {
                BusinessID = 0,
                LeavePoint = 10,//注册由200积分改为10积分  20170829 by luoli
                TotalPoint = 10,
                TypeID = 12,//12注册得到积分
                UserID = userid,
                Approver = 0
            };
            try
            {
                HotelAdapter.HotelService.InsertOrUpdatePoints(pe);
                MessageAdapter.InsertSysMessage(new SysMessageEnitity()
                {
                    state = 0,
                    businessID = userid,
                    businessType = (int)HJDAPI.Common.Helpers.Enums.SysMessigeType.Register,
                    receiver = userid
                });
                MessageAdapter.SendAppNotice(new SendNoticeEntity()
                {
                    actionUrl = "",
                    appType = 0,
                    from = "周末酒店",
                    msg = "恭喜注册成功，我们已将10奖励积分存放在您的钱包中。",
                    noticeType = ZMJDNoticeType.register,
                    title = "周末酒店",
                    userID = userid
                });
            }
            catch (Exception e)
            {
                Log.WriteLog("注册插入积分报错：userid" + userid + " " + e.Message + "\r\n" + e.StackTrace);
                throw e;
            }
        }

        internal static List<UserRole> GetUserRoleRelByRoleID(int roleid)
        {
            return PrivilegeService.GetUserRoleRelByRoleID(roleid);
        }

        internal static List<User_Info> GetUserInfoByRoleID(int roleid)
        {
            return PrivilegeService.GetUserInfoByRoleID(roleid);
        }

        internal static User_Info GetUserInfoByMobile(string phoneNum)
        {
            return AccService.GetUserInfoByMobile(phoneNum);
        }

        internal static MobileLoginResult PhoneNumConfirmCodeLogin(LoginRequest request)
        {
            return AccService.PhoneNumConfirmCodeLogin(request);
        }

        internal static int EditUserInfoExAdd(long userId, string userRemark, string creator)
        {
            return AccService.EditUserInfoExAdd(userId, userRemark, creator);
        }

        internal static void CreateVIPRole(long userID, string phoneNum, int activityID, long CID, bool CanBuyVIPFirstPackage)
        {

            HJDAPI.Controllers.AccountAdapter.UserRoleEnum VIPRole = HJDAPI.Controllers.AccountAdapter.UserRoleEnum.VIP199NR;
            if (activityID == (int)HJDAPI.Controllers.EnumHelper.ActivityID.VIP599)
                VIPRole = HJDAPI.Controllers.AccountAdapter.UserRoleEnum.VIP599;

            var userRoleList = GetUserRoleRelByUserId(userID);

            if (userRoleList.Where(r => r.RoleID == (int)VIPRole).Count() == 0)
            {
                AccountAdapter.UserRoleRelInsert(userID, (int)VIPRole);

                if (CanBuyVIPFirstPackage)
                    AccountAdapter.InsertOrDeleteUserPrivilegeRel(userID, (int)PrivilegeEnums.UserPrivilege.CanBuyVIPFirstPackage, true, HJD.HotelManagementCenter.Domain.OpLogBizType.BuyVIP, (int)VIPRole, "CreateVIPRole");


                var smsMsg400 = CouponAdapter.GetVIPConfirmSMS(); 
                try
                {
                    SMServiceController.SendSMS(phoneNum, smsMsg400);
                }
                catch (Exception ex)
                {
                    Log.WriteLog("短信发送异常" + ex.Message);
                }


                try
                {
                    if (!AccountAdapter.NotRegistMobileAccount(phoneNum))
                    {
                        int r = new Random().Next(100000, 999999);
                        string passWord = r.ToString();
                        string phone = phoneNum;
                        AccountAdapter.RegisterPhoneUser(phone, passWord, 2, 0, "", true, CID);
                        string registMsg = string.Format(@"你的临时密码为：{0}。登录周末酒店APP，可在设置页面更改密码。点击下载周末酒店APP：http://app.zmjiudian.com", passWord);
                        SMServiceController.SendSMS(phone, registMsg);
                    }
                }
                catch (Exception ex)
                {
                    Log.WriteLog("未注册用户购买vip注册异常" + ex.Message);
                }

            }
            else
            {
                Log.WriteLog(string.Format("CreateVIPRole：用户已是vip! userid:{0}", userID));
            }

        }

        internal static int DeleteUserRight_UserPrivilegeRel(long couponUserID, PrivilegeEnums.UserPrivilege userPrivilege)
        {
            return PrivilegeService.DeleteUserRight_UserPrivilegeRel(couponUserID, (int)userPrivilege); 
        }

        internal static int DeleteUserRight_UserRoleRel(long UserID, UserRoleEnum userRole)
        {
            return PrivilegeService.UserRoleRelDelete(UserID, (int)userRole);
        }

        internal static List<MemberProfileInfo> GetMultiMemberProfileInfo(List<long> userIDList)
        {
            return AccService.GetMultiMemberProfileInfo(userIDList);
        }

        internal static List<PointsEntity> GetExpirePointsEntity(string userids = "")
        {
            DateTime startTime = Convert.ToDateTime(System.DateTime.Now.ToString("yyyy-MM-01"));
            DateTime endTime = Convert.ToDateTime(System.DateTime.Now.AddDays(1 - DateTime.Now.Day).AddMonths(4).AddDays(-1).ToString("yyyy-MM-dd"));
            return HotelService.GetExpirePointsEntity(startTime, endTime, userids);
        }


        public static int UpdateReceivePeopleInformation_UpdateIsLastSelected(ReceivePeopleInformationEntity param)
        {
            return AccService.UpdateReceivePeopleInformation_UpdateIsLastSelected(param);
        }

        public static List<User_Info> GetTestUserList()
        {

            return new List<User_Info>(){ 
            new User_Info{ UserId=4536291, RealName ="贝贝",Phone ="10012312309"},
new User_Info{ UserId=4536287, RealName ="草莓酱",Phone ="10012312305"},
new User_Info{ UserId=4536292, RealName ="等等在路上",Phone ="10012312310"},
new User_Info{ UserId=4536288, RealName ="Hupopo",Phone ="10012312306"},
new User_Info{ UserId=4536289, RealName ="秘密天使",Phone ="10012312307"},
new User_Info{ UserId=4536290, RealName ="猫小姐",Phone ="10012312308"},
new User_Info{ UserId=4536284, RealName ="麦小麦",Phone ="10012312302"},
new User_Info{ UserId=4536286, RealName ="TeresaQ",Phone ="10012312304"},
new User_Info{ UserId=4536283, RealName ="小满的天空",Phone ="10012312301"},
new User_Info{ UserId=4536285, RealName ="萱萱妈妈",Phone ="10012312303"} 
            };
        }

        #region 活动粉丝相关操作

        public static UserFansRel GetOneFansRelByUnionid(string unionid)
        {
            return AccService.GetOneFansRelByUnionid(unionid);
        }

        public static UserFansRel GetOneUserRelFans(long userId, string unionid)
        {
            return AccService.GetOneUserRelFans(userId, unionid);
        }

        public static int AddUserFansRel(UserFansRel userfansrel)
        {
            return AccService.AddUserFansRel(userfansrel);
        }


        public static List<UserFansRel> GetUserRelFans(long userId)
        {
            return AccService.GetUserRelFans(userId);
        }

        public static List<UserFansRelReport> GetUserRelFansReportBySourceId(int sourceType, int sourceId)
        {
            return AccService.GetUserRelFansReportBySourceId(sourceType, sourceId);
        }

        public static int GetUserRelFansCountBySourceId(int sourceType, int sourceId)
        {
            return AccService.GetUserRelFansCountBySourceId(sourceType, sourceId);
        }

        #endregion
    }
}