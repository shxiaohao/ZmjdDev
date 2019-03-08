using System;
using HJD.Framework.Entity;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace HJD.HotelServices.Entity
{
    [Serializable]
    [DataContract]
    [DefaultColumn]
    public class SOAHotelReviewEntity
    {
        int _writing;
        /// <summary>
        /// 酒店点评ID
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "0")]
        public int Writing
        {
            get { return _writing; }
            set { _writing = value; }
        }
        DateTime _wdate;
        /// <summary>
        /// 点评日期
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(IsOptional = true)]
        public DateTime Wdate
        {
            get { return _wdate; }
            set { _wdate = value; }
        }
        string _uid;
        /// <summary>
        /// 用户UID
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(IsOptional = true)]
        public string Uid
        {
            get { return _uid; }
            set { _uid = value; }
        }
        string _content;
        /// <summary>
        /// 点评内容
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(IsOptional = true)]
        public string Content
        {
            get { return _content; }
            set { _content = value; }
        }
        int _resource;
        /// <summary>
        /// 点评酒店
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "0")]
        public int Resource
        {
            get { return _resource; }
            set { _resource = value; }
        }
        int _fbWriting;
        /// <summary>
        /// 酒店点评的反馈ID
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "0", IsOptional = true)]
        public int FbWriting
        {
            get { return _fbWriting; }
            set { _fbWriting = value; }
        }
        string _nickname;
        /// <summary>
        /// 用户的昵称
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(IsOptional = true)]
        public string Nickname
        {
            get { return _nickname; }
            set { _nickname = value; }
        }

        string _writingType;

        /// <summary>
        /// 点评类型 C携程点评 L驴评开发点评
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute( IsOptional = true)]
        public string WritingType
        {
            get { return _writingType; }
            set { _writingType = value; }
        }

        string _title;
        /// <summary>
        /// 点评标题
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(IsOptional = true)]
        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        string identitytxt;

        /// <summary>
        /// 点评原因
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(IsOptional = true)]
        public string Identitytxt
        {
            get { return identitytxt; }
            set { identitytxt = value; }
        }

        int user_identity;

        /// <summary>
        /// 点评原因ID
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "0")]
        public int User_identity
        {
            get { return user_identity; }
            set { user_identity = value; }
        }


        string _hotelUrl;

        [DataMemberAttribute()]
        [DBColumnAttribute(IsOptional=true)]
        public string HotelUrl
        {
            get { return _hotelUrl; }
            set { _hotelUrl = value; }
        }

        string _hotelReviewUrl;

        [DataMemberAttribute()]
        [DBColumnAttribute( IsOptional = true)]
        public string HotelReviewUrl
        {
            get { return _hotelReviewUrl; }
            set { _hotelReviewUrl = value; }
        }

        string _hotelOtherUrl;

        [DataMemberAttribute()]
        [DBColumnAttribute(IsOptional=true)]
        public string HotelOtherUrl
        {
            get { return _hotelOtherUrl; }
            set { _hotelOtherUrl = value; }
        }

        string _fbcontent;

        [DataMemberAttribute()]
        [DBColumnAttribute( IsOptional = true)]
        public string Fbcontent
        {
            get { return _fbcontent; }
            set { _fbcontent = value; }
        }
        string _addconent;

        [DataMemberAttribute()]
        [DBColumnAttribute( IsOptional = true)]
        public string Addconent
        {
            get { return _addconent; }
            set { _addconent = value; }
        }
        
    }
}
