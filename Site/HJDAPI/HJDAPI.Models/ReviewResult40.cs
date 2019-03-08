using System.Collections.Generic;
using System.Runtime.Serialization;
using HJD.HotelServices.Contracts;
using HJD.HotelServices;

namespace HJDAPI.Models
{
    [DataContract]
    public class ReviewResult40
    {
        public ReviewResult40()
        {
            TFTList = new List<HotelTFTRelItemEntity>();
            ParentTFTList = new List<HotelTFTRelItemEntity>();
            Result = new List<CommentItem>();
            //RoomPicAndPicDescriptionList = new List<RoomPicAndPicDescription>();
        }

        [DataMember]
        public int HotelID { get; set; }

        [DataMember]
        public int Start { get; set; }

        /// <summary>
        /// 如玩点的所有点评数
        /// </summary>
        [DataMember]
        public int GroupTotalCount { get; set; }

        [DataMember]
        public int TotalCount { get; set; }

        [DataMember]
        public int InterestID { get; set; }

        [DataMember]
        public int TFTType { get; set; }

        [DataMember]
        public int TFTID { get; set; }

        [DataMember]
        public int RatingType { get; set; }

        [DataMember]
        public string RatingPercent { get; set; }
        
        [DataMember]
        public decimal Score { get; set; }

        [DataMember]
        public IEnumerable<CommentItem> Result { get; set; }

        //[DataMember]
        //public List<OTAInfo> OTAInfos;

        [DataMember]
        public List<HotelTFTRelItemEntity> TFTList { get; set; }

        [DataMember]
        public List<HotelTFTRelItemEntity> ParentTFTList { get; set; }

        /// <summary>
        /// 酒店的所有点评数;而TotalCount表示相关的特色或主题的酒店点评数量
        /// </summary>
        [DataMember]
        public int AllReviewCount { get; set; }
        /// <summary>
        /// 房型图片
        /// </summary>
        [DataMember]
        public List<string> PicUrl { get; set; }

        /// <summary>
        /// 房型图片描述
        /// </summary>
        [DataMember]
        public string PicUrlDescription { get; set; }

        //public List<RoomPicAndPicDescription> RoomPicAndPicDescriptionList { get; set; }
    }

    //public class RoomPicAndPicDescription
    //{
    //    public RoomPicAndPicDescription()
    //    {
    //        RoomCode = "";
    //        PicUrl = new List<string>();
    //        PicUrlDescription = "";
    //    }
    //    /// <summary>
    //    /// 房型名称
    //    /// </summary>
    //    [DataMember]
    //    public string RoomCode { get; set; }
    //    /// <summary>
    //    /// 房型图片
    //    /// </summary>
    //    [DataMember]
    //    public List<string> PicUrl { get; set; }
    //    /// <summary>
    //    /// 房型图片描述
    //    /// </summary>
    //    [DataMember]
    //    public string PicUrlDescription { get; set; }
    //}
}