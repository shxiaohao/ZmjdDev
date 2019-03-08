using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Runtime.Serialization;
using HJD.Framework.Entity;
namespace HJD.HotelServices.Contracts
{
    [System.SerializableAttribute()]
    [System.Runtime.Serialization.DataContractAttribute()]
    [HJD.Framework.Entity.DefaultColumnAttribute()]
    public class HJDHotelReview
    {
        
        #region 映射字段
        private int writing = 0;

        private DateTime wdate = System.DateTime.Now;
        
        private string uid = "";
        
        private string title = "";
        
        private string content;
        
        private decimal rating = 0;
        
        private int hotelid = 0;
        
        private int user_identity = 0;
        
        private string identitytxt = "";
        
        private string commentsubject = "";
        
        private string writingtype;

        private DateTime operatetime = System.DateTime.Now;
        
        private long userid = 0;
        
        private string nickname = "";
        
        private string userurl = "";
        
        private int ratingroom = 0;
        
        private int ratingatmosphere = 0;
        
        private int ratingservice = 0;
        
        private int ratingcostbenefit = 0;

        private int votecount = 0;
        #endregion
        
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public int Writing
        {
            get
            {
                return this.writing;
            }
            set
            {
                this.writing = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public DateTime Wdate
        {
            get
            {
                return this.wdate;
            }
            set
            {
                this.wdate = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public string Uid
        {
            get
            {
                return this.uid;
            }
            set
            {
                this.uid = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public string Title
        {
            get
            {
                return this.title;
            }
            set
            {
                this.title = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public string Content
        {
            get
            {
                return this.content;
            }
            set
            {
                this.content = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public decimal Rating
        {
            get
            {
                return this.rating;
            }
            set
            {
                this.rating = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public int HotelId
        {
            get
            {
                return this.hotelid;
            }
            set
            {
                this.hotelid = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public int User_identity
        {
            get
            {
                return this.user_identity;
            }
            set
            {
                this.user_identity = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public string Identitytxt
        {
            get
            {
                return this.identitytxt;
            }
            set
            {
                this.identitytxt = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public string Commentsubject
        {
            get
            {
                return this.commentsubject;
            }
            set
            {
                this.commentsubject = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public string WritingType
        {
            get
            {
                return this.writingtype;
            }
            set
            {
                this.writingtype = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public DateTime OperateTime
        {
            get
            {
                return this.operatetime;
            }
            set
            {
                this.operatetime = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public long UserID
        {
            get
            {
                return this.userid;
            }
            set
            {
                this.userid = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public string Nickname
        {
            get
            {
                return this.nickname;
            }
            set
            {
                this.nickname = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public string UserUrl
        {
            get
            {
                return this.userurl;
            }
            set
            {
                this.userurl = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public int RatingRoom
        {
            get
            {
                return this.ratingroom;
            }
            set
            {
                this.ratingroom = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public int RatingAtmosphere
        {
            get
            {
                return this.ratingatmosphere;
            }
            set
            {
                this.ratingatmosphere = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public int RatingService
        {
            get
            {
                return this.ratingservice;
            }
            set
            {
                this.ratingservice = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public int RatingCostBenefit
        {
            get
            {
                return this.ratingcostbenefit;
            }
            set
            {
                this.ratingcostbenefit = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public int Votecount
        {
            get
            {
                return this.votecount;
            }
            set
            {
                this.votecount = value;
            }
        }
        
    }

}
