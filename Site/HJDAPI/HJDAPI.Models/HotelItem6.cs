using HJD.HotelServices.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.Models
{
    public class HotelItem6 : HotelBaseItem
    {
        public HotelItem6()
        {
            RoomComment = new RoomTypeComment();
            RoomTypeData = new List<RoomTypeCommentItem>();
            FoodDescription = new HotelInfo();
            RoomDescription = new HotelInfo();
            FacilitieDescription = new HotelInfo();
            NewComment = new CommentItem();
            Suggest = new BuyMembershipSuggest();
            CommentForId = new CommentItem();
            IntroNew = new CommentTextAndUrl_ex();
            FriendComment = new CommentItem();
        }
        /// <summary>
        /// 房间评论老字段
        /// </summary>
        public RoomTypeComment RoomComment { get; set; }

        public List<RoomTypeCommentItem> RoomTypeData { get; set; }
        /// <summary>
        /// 娱乐设施字段
        /// </summary>
        public List<FeaturedCommentEntity> EntertainmentList { get; set; }

        public DateTime PriceCinDate { get; set; }

        public DateTime PriceCouDate { get; set; }
        /// <summary>
        /// 美食描述
        /// </summary>
        //public FeaturedCommentEntity FoodDescription { get; set; }
        public HotelInfo FoodDescription { get; set; }
        //public string FoodDescription { get; set; } 
        /// <summary>
        /// 房间描述
        /// </summary>
        //public string RoomDescription { get; set; }
        //public FeaturedCommentEntity RoomDescription { get; set; }
        public HotelInfo RoomDescription { get; set; }
        /// <summary>
        /// 设施描述
        /// </summary>
        //public string FacilitieDescription { get; set; }
        //public FeaturedCommentEntity FacilitieDescription { get; set; }
        public HotelInfo FacilitieDescription { get; set; }
        /// <summary>
        /// 最新评论
        /// </summary>
        //public string NewComment { get; set; }
        public CommentItem NewComment { get; set; }

        /// <summary>
        /// 好友最新评论
        /// </summary>
        //public string NewComment { get; set; }
        public CommentItem FriendComment { get; set; }

        /// <summary>
        /// 展示购买VIP
        /// </summary>
        //public string NewComment { get; set; }
        public BuyMembershipSuggest Suggest { get; set; }


        /// <summary>
        /// 推荐理由 5.1版本开始使用
        /// </summary>
        //public string NewComment { get; set; }
        public CommentTextAndUrl_ex IntroNew { get; set; }

        /// <summary>
        /// 是否收藏
        /// </summary>
        public bool IsCollection { get; set; }

        /// <summary>
        /// 评论
        /// </summary>
        public CommentItem CommentForId { get; set; }

        /// <summary>
        /// 图片宽高比的宽
        /// </summary>
        public int PicWidth { get; set; }

        /// <summary>
        /// 图片宽高比的高
        /// </summary>
        public int PicHeight { get; set; }
    }

    [Serializable]
    [DataContract]
    public class CommentTextAndUrl_ex : CommentTextAndUrl
    {
        public CommentTextAndUrl_ex()
        {
            Item = new List<string>();
            PageMaxHeight = 1300;
            PagePaddingRight = 15;
            PagePaddingLeft = 15;
        }

        [DataMember]
        public List<string> Item { get; set; }

        [DataMember]
        public int PageMaxHeight { get; set; }

        [DataMember]
        public int PagePaddingLeft { get; set; }

        [DataMember]
        public int PagePaddingRight { get; set; }
    }
}
