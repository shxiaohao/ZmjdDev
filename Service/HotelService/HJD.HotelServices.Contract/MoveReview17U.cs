using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using HJD.Framework.Entity;

namespace HJD.HotelServices.Contracts
{
    [Serializable]
    [DataContract]
    [DefaultColumn]
    public class MoveReview17U
    {
        [DataMember]
        [DBColumn]
        public int HAOJIUDIANHotelId { get; set; }
        [DataMember]
        public int ReviewID { get; set; }

        [DataMember]
        [DBColumn]
        public int HotelId { get; set; }

        [DataMember]
        [DBColumn]
        public string Creator { get; set; }

        [DataMember]
        [DBColumn]
        public DateTime CreateDate { get; set; }

        [DataMember]
        [DBColumn]
        public string Content { get; set; }

        [DataMember]
        [DBColumn]
        public int TuiJianHotel { get; set; }

        [DataMember]
        [DBColumn]
        public int Voucher { get; set; }

        [DataMember]
        [DBColumn]
        public int HOEVoucherFlag { get; set; }

        [DataMember]
        [DBColumn]
        public int HDMDType { get; set; }

        [DataMember]
        [DBColumn]
        public string RoomTypeName { get; set; }

        [DataMember]
        [DBColumn(Ignore = true)]
        public int OverRallRating { get; set; }

        [DataMember]
        [DBColumn]
        public int DealState { get; set; }

    }

    [Serializable]
    [DataContract]
    [DefaultColumn]
    public class MoveReviewElong
    {
        [DataMember]
        [DBColumn]
        public int HAOJIUDIANHotelId { get; set; }

        [DataMember]
        [DBColumn]
        public int ReviewID { get; set; }

        [DataMember]
        [DBColumn]
        public int HotelId { get; set; }

        [DataMember]
        [DBColumn]
        public string Creator { get; set; }

        [DataMember]
        [DBColumn]
        public DateTime CreateDate { get; set; }

        [DataMember]
        [DBColumn]
        public string Title { get; set; }

        [DataMember]
        [DBColumn]
        public string Content { get; set; }

        [DataMember]
        [DBColumn]
        public int RecommendType { get; set; }

        [DataMember]
        [DBColumn]
        public int UsefulTotal { get; set; }

        [DataMember]
        [DBColumn]
        public int UserRating { get; set; }

        [DataMember]
        [DBColumn]
        public int CommentTitularId { get; set; }

        [DataMember]
        [DBColumn]
        public int CommentSourceId { get; set; }

        [DataMember]
        [DBColumn]
        public int ParentReviewId { get; set; }

        [DataMember]
        [DBColumn]
        public int DealState { get; set; }
    }


    [Serializable]
    [DataContract]
    [DefaultColumn]
    public class MoveReviewOTA
    {  
        [DataMember]
        [DBColumn(IsOptional=true)]
        public int ID { get; set; }

        [DataMember]
        [DBColumn]
        public int HotelId { get; set; }

        [DataMember]
        [DBColumn]
        public int OTAReviewID { get; set; }

        [DataMember]
        [DBColumn]
        public int OTAHotelId { get; set; }

        [DataMember]
        [DBColumn]
        public string Creator { get; set; }

        [DataMember]
        [DBColumn]
        public DateTime CreateDate { get; set; }
 
        [DataMember]
        [DBColumn]
        public string Content { get; set; }

        [DataMember]
        [DBColumn]
        public int ChannelID { get; set; }

        [DataMember]
        [DBColumn]
        public int Score { get; set; }

        [DataMember]
        [DBColumn]
        public string CommentSubject { get; set; }


        [DataMember]
        [DBColumn]
        public long UserID { get; set; }        

        [DataMember]
        [DBColumn]
        public int DealState { get; set; }
    }

    [Serializable]
    [DataContract]
    [DefaultColumn]
    public class MoveReviewCtrip
    {
        [DataMember]
        [DBColumn(IsOptional=true)]
        public int Writing { get; set; }

        [DataMember]
        [DBColumn]
        public int HotelId { get; set; }

        [DataMember]
        [DBColumn]
        public int CtripHotelId { get; set; }

        [DataMember]
        [DBColumn]
        public string reviewType { get; set; }

        [DataMember]
        [DBColumn]
        public DateTime commentdate { get; set; }

        [DataMember]
        [DBColumn]
        public int ctripCommentId { get; set; }

        [DataMember]
        [DBColumn]
        public string roomType { get; set; }

        [DataMember]
        [DBColumn]
        public float wholerate { get; set; }

        [DataMember]
        [DBColumn]
        public int score_weisheng { get; set; }

        [DataMember]
        [DBColumn]
        public int score_fuwu { get; set; }

        [DataMember]
        [DBColumn]
        public int score_sheshi { get; set; }

        [DataMember]
        [DBColumn]
        public int score_weizhi { get; set; }

        [DataMember]
        [DBColumn]
        public string commentdetail { get; set; }

        [DataMember]
        [DBColumn]
        public string hotelfeedback { get; set; }

        [DataMember]
        [DBColumn(DefaultValue = "2000-01-01 00:00:00")]
        public DateTime hotelfeedbackdate { get; set; }

        [DataMember]
        [DBColumn]
        public DateTime updatetime { get; set; }

        [DataMember]
        [DBColumn]
        public string username { get; set; }

        [DataMember]
        [DBColumn]
        public int DealState { get; set; }

        [DataMember]
        [DBColumn]
        public string AddReview { get; set; }

        [DataMember]
        [DBColumn]
        public bool IsRecommand { get; set; }
    }

    [Serializable]
    [DataContract]
    [DefaultColumn]
    public class MoveReviewBooking
    {
        [DataMember]
        [DBColumn]
        public Int64 Id { get; set; }

        [DataMember]
        [DBColumn]
        public Int32 HotelId { get; set; }

        [DataMember]
        [DBColumn]
        public Int64 OtaHotelId { get; set; }

        [DataMember]
        [DBColumn]
        public Int64 ReviewId { get; set; }

        [DataMember]
        [DBColumn]
        public string Uid { get; set; }

        [DataMember]
        [DBColumn]
        public string UInfo { get; set; }

        [DataMember]
        [DBColumn]
        public string District { get; set; }

        [DataMember]
        [DBColumn]
        public string Title { get; set; }

        [DataMember]
        [DBColumn]
        public string ReviewContent { get; set; }

        [DataMember]
        [DBColumn]
        public string ContentNeg { get; set; }

        [DataMember]
        [DBColumn]
        public string ContentPos { get; set; }

        [DataMember]
        [DBColumn]
        public DateTime ReviewDate { get; set; }

        [DataMember]
        [DBColumn]
        public Int32 Score { get; set; }

        [DataMember]
        [DBColumn]
        public string Rating { get; set; }

        [DataMember]
        [DBColumn]
        public string Tags { get; set; }

        [DataMember]
        [DBColumn]
        public string Channel { get; set; }

        [DataMember]
        [DBColumn]
        public int DealState { get; set; }
    }
}
