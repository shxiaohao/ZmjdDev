using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using HJD.Framework.Entity;

namespace HJD.WeixinServices.Contracts
{
    [System.SerializableAttribute()]
    [System.Runtime.Serialization.DataContractAttribute()]
    [HJD.Framework.Entity.DefaultColumnAttribute()]
    //微信活动配置类
    public class WeixinActivityEntity
    {
        private int id = 0;
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 
        /// </summary>
        public int ID
        {
            get { return id; }
            set { id = value; }
        }

        private int activityID = 0;
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 
        /// </summary>
        public int ActivityID
        {
            get { return activityID; }
            set { activityID = value; }
        }

        private string activityKeyWord = "";
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 
        /// </summary>
        public string ActivityKeyWord
        {
            get { return activityKeyWord; }
            set { activityKeyWord = value; }
        }

        private string activityNotStartWord = "";
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 
        /// </summary>
        public string ActivityNotStartWord
        {
            get { return activityNotStartWord; }
            set { activityNotStartWord = value; }
        }

        private string activityFinishWord = "";
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 
        /// </summary>
        public string ActivityFinishWord
        {
            get { return activityFinishWord; }
            set { activityFinishWord = value; }
        }

        private DateTime activityStartDateTime = DateTime.Parse("2000-01-01");
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 
        /// </summary>
        public DateTime ActivityStartDateTime
        {
            get { return activityStartDateTime; }
            set { activityStartDateTime = value; }
        }

        private DateTime activityFinishDateTime = DateTime.Parse("2000-01-01");
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 
        /// </summary>
        public DateTime ActivityFinishDateTime
        {
            get { return activityFinishDateTime; }
            set { activityFinishDateTime = value; }
        }

        private DateTime activeEndTime = DateTime.Parse("2000-01-01");
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 
        /// </summary>
        public DateTime ActiveEndTime
        {
            get { return activeEndTime; }
            set { activeEndTime = value; }
        }

        private bool txtCanEmpty = false;
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 
        /// </summary>
        public bool TxtCanEmpty
        {
            get { return txtCanEmpty; }
            set { txtCanEmpty = value; }
        }

        private string hasEnrollTxtMessage = "";
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 
        /// </summary>
        public string HasEnrollTxtMessage
        {
            get { return hasEnrollTxtMessage; }
            set { hasEnrollTxtMessage = value; }
        }

        private string enrollTxtSuccess = "";
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 
        /// </summary>
        public string EnrollTxtSuccess
        {
            get { return enrollTxtSuccess; }
            set { enrollTxtSuccess = value; }
        }

        private string enrollTxtAlert = "";
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 
        /// </summary>
        public string EnrollTxtAlert
        {
            get { return enrollTxtAlert; }
            set { enrollTxtAlert = value; }
        }

        private bool hasPhotoStep = false;
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 
        /// </summary>
        public bool HasPhotoStep
        {
            get { return hasPhotoStep; }
            set { hasPhotoStep = value; }
        }

        private string defaultPhotoSuccess = "";
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 
        /// </summary>
        public string DefaultPhotoSuccess
        {
            get { return defaultPhotoSuccess; }
            set { defaultPhotoSuccess = value; }
        }

        private string enrollPhotoSuccess = "";
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 
        /// </summary>
        public string EnrollPhotoSuccess
        {
            get { return enrollPhotoSuccess; }
            set { enrollPhotoSuccess = value; }
        }

        private string solutions = "";
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 
        /// </summary>
        public string Solutions
        {
            get { return solutions; }
            set { solutions = value; }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        /// <summary>
        /// 
        /// </summary>
        public List<string> ActivitySolutions;

        private int type = 1;
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 活动类型
        /// </summary>
        public int Type
        {
            get { return type; }
            set { type = value; }
        }

        #region 微信报名活动关联配置

        private int haveSignUp;
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 
        /// </summary>
        public int HaveSignUp
        {
            get { return haveSignUp; }
            set { haveSignUp = value; }
        }

        private string weixinSignUpTopBannerUrl = "";
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 
        /// </summary>
        public string WeixinSignUpTopBannerUrl
        {
            get { return weixinSignUpTopBannerUrl; }
            set { weixinSignUpTopBannerUrl = value; }
        }

        private string weixinSignUpTopBannerTitle = "";
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 
        /// </summary>
        public string WeixinSignUpTopBannerTitle
        {
            get { return weixinSignUpTopBannerTitle; }
            set { weixinSignUpTopBannerTitle = value; }
        }

        private string weixinSignUpTopBannerTitle2 = "";
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 
        /// </summary>
        public string WeixinSignUpTopBannerTitle2
        {
            get { return weixinSignUpTopBannerTitle2; }
            set { weixinSignUpTopBannerTitle2 = value; }
        }

        private int weixinSignUpTopBannerTitleAlign = 0;
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 
        /// </summary>
        public int WeixinSignUpTopBannerTitleAlign
        {
            get { return weixinSignUpTopBannerTitleAlign; }
            set { weixinSignUpTopBannerTitleAlign = value; }
        }

        private string weixinSignUpShareTitle = "";
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 
        /// </summary>
        public string WeixinSignUpShareTitle
        {
            get { return weixinSignUpShareTitle; }
            set { weixinSignUpShareTitle = value; }
        }

        private string weixinSignUpShareLink = "";
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 
        /// </summary>
        public string WeixinSignUpShareLink
        {
            get { return weixinSignUpShareLink; }
            set { weixinSignUpShareLink = value; }
        }

        private string weixinSignUpShareImgUrl = "";
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 
        /// </summary>
        public string WeixinSignUpShareImgUrl
        {
            get { return weixinSignUpShareImgUrl; }
            set { weixinSignUpShareImgUrl = value; }
        }

        private string weixinSignUpShareTip = "";
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 
        /// </summary>
        public string WeixinSignUpShareTip
        {
            get { return weixinSignUpShareTip; }
            set { weixinSignUpShareTip = value; }
        }

        private string weixinSignUpResultLink = "";
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 
        /// </summary>
        public string WeixinSignUpResultLink
        {
            get { return weixinSignUpResultLink; }
            set { weixinSignUpResultLink = value; }
        }

        private int weixinAcountId = 1;
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 活动归属的微信账号
        /// </summary>
        public int WeixinAcountId
        {
            get { return weixinAcountId; }
            set { weixinAcountId = value; }
        }

        private string relPartnerIds = "";
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 当前活动关联合作伙伴的ID（可多个，英文逗号分开）
        /// </summary>
        public string RelPartnerIds
        {
            get { return relPartnerIds; }
            set { relPartnerIds = value; }
        }

        private int needPaySign = 0;
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 是否需要支付报名
        /// </summary>
        public int NeedPaySign
        {
            get { return needPaySign; }
            set { needPaySign = value; }
        }

        private decimal payPrice = 1;
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 需支付金额(单位元)
        /// </summary>
        public decimal PayPrice
        {
            get { return payPrice; }
            set { payPrice = value; }
        }

        private decimal returnPrice = 5;
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 返还金额(单位元，一般为住基金)
        /// </summary>
        public decimal ReturnPrice
        {
            get { return returnPrice; }
            set { returnPrice = value; }
        }

        private int isInvite = 0;
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 是否为邀请制免费住
        /// </summary>
        public int IsInvite
        {
            get { return isInvite; }
            set { isInvite = value; }
        }

        private int personMaxLucks = 0;
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 每人最多可获抽奖码总数（该字段的初衷是为避免微信用户刷阅读量的情况 2018.06.26）
        /// </summary>
        public int PersonMaxLucks
        {
            get { return personMaxLucks; }
            set { personMaxLucks = value; }
        }

        #endregion
    }
}
