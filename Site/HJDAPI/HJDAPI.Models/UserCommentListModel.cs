using HJD.CommentService.Contract;
using HJD.HotelPrice.Contract.DataContract.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.Models
{
    public class UserCommentListModel
    {
        public List<CommentItemEntity> CommentList { get; set; }

        public List<CommentInfoModel> CommentDteailList { get; set; }
        
        public List<CommentItemEntity> NoCommentList { get; set; }

        public List<OrderListItem> OrderList { get; set; }
    }

    [System.Runtime.Serialization.DataContract]
    public class CommentListModel
    {
        /// <summary>
        /// 是否显示添加酒店标志
        /// </summary>
        [System.Runtime.Serialization.DataMember]
        public bool IsShowAddHotel { get;set;}
        /// <summary>
        /// 点评列表 混合
        /// </summary>
        [System.Runtime.Serialization.DataMember]
        public List<UserCommentListItemEntity> CommentList { get; set; }
    }

    [System.Runtime.Serialization.DataContract]
    public class CommentListModel20
    {
        /// <summary>
        /// 点评列表 写好的
        /// </summary>
        [System.Runtime.Serialization.DataMember]
        public List<UserCommentListItemEntity> DoneCommentList { get; set; }

        /// <summary>
        /// 点评列表 待写的
        /// </summary>
        [System.Runtime.Serialization.DataMember]
        public List<UserCommentListItemEntity> UnDoneCommentList { get; set; }
    }
}