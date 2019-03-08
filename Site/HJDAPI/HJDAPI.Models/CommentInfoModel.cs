using HJD.CommentService.Contract;
using HJD.PhotoServices.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.Models
{
    public class CommentInfoModel
    {
        public CommentInfoEntity commentInfo { get; set; }
        public List<string> photoInfo { get; set; }
        /// <summary>
        /// APP 显示的点评HTML
        /// </summary>
        public string commentHTML { get; set; }
        public CommentItemEntity commentItem { get; set; }
        public string hotelPic { get; set; }
        public int hotelStar { get; set; }
        public decimal hotelScore { get; set; }
        public string hotelName { get; set; }
        public string nickName { get; set; }
        public CommentShareModel shareModel { get; set; }
    }

    public class CommentInfoModel2
    {
        public CommentInfoModel commentInfoModel { get; set; }

        public List<CategoryPhotosEntity> categoryPhotoList { get; set; }
    }

    [DataContract]
    public class CommentInfoModel3
    {
        [DataMember]
        public int commentID { get; set; }
        [DataMember]
        public List<string> commentPics { get; set; }
        [DataMember]
        public List<string> bigCommentPics { get; set; }
        [DataMember]
        public string hotelName { get; set; }
        [DataMember]
        public int hotelID { get; set; }
        [DataMember]
        public decimal hotelScore { get; set; }
        [DataMember]
        public string hotelPic { get; set; }
        [DataMember]
        public int minPrice { get; set; }
        [DataMember]
        public string packageBrief { get; set; }
        [DataMember]
        public string nickName { get; set; }
        /// <summary>
        /// 阿凡达 头像化身
        /// </summary>
        [DataMember]
        public string avatar { get; set; }
        [DataMember]
        public string advantage { get; set; }
        [DataMember]
        public string disAdvantage { get; set; }
        /// <summary>
        /// 用户自己一句话的总结
        /// </summary>
        [DataMember]
        public string commentTitle { get; set; }
        /// <summary>
        /// 用户详细的点评内容
        /// </summary>
        [DataMember]
        public string commentContent { get; set; }
        /// <summary>
        /// 追加点评内容
        /// </summary>
        [DataMember]
        public string additionalContent { get; set; }
        [DataMember]
        public string roomType { get; set; }
        [DataMember]
        public string tripType { get; set; }
        [DataMember]
        public bool isRecommend { get; set; }
        [DataMember]
        public float commentScore { get; set; }
        [DataMember]
        public int usefulCount { get; set; }
        [DataMember]
        public bool hasClickUseful { get; set; }
        [DataMember]
        public List<ClickUsefulPeopleItem> clickUsefulList { get; set; }
        [DataMember]
        public CommentShareModel shareModel { get; set; }
        /// <summary>
        /// 写点评时间
        /// </summary>
        [DataMember]
        public DateTime writeTime { get; set; }
        /// <summary>
        /// 点评作者UserID
        /// </summary>
        [DataMember]
        public long authorUserID { get; set; }
        /// <summary>
        /// 评论列表
        /// </summary>
        [DataMember]
        public List<CommentReviewsItem> reviewItems { get; set; }
        /// <summary>
        /// 评论数量
        /// </summary>
        [DataMember]
        public int reviewCount { get; set; }
        /// <summary>
        /// 浏览数量
        /// </summary>
        [DataMember]
        public int visitCount { get; set; }
        /// <summary>
        /// 是否显示写评论
        /// </summary>
        [DataMember]
        public bool isCanWriteReview { get; set; }
        /// <summary>
        /// 点评详情页 关注按钮 需要知道当前关系
        /// </summary>
        [DataMember]
        public int followState { get; set; }
        /// <summary>
        /// 点评片段
        /// </summary>
        [DataMember]
        public List<AdditionalSection> addSections { get; set; }
        /// <summary>
        /// 弹出框显示内容
        /// </summary>
        [DataMember]
        public PopupBoxData boxData { get; set; }
        /// <summary>
        /// 能否追加点评
        /// </summary>
        [DataMember]
        public bool IsCanAddContent { get; set; }
    }

    [DataContract]
    public class PopupBoxData
    {
        public PopupBoxData()
        {
            showUrl = "";
            widthRatio = 0.8f;
            lazyLoadTime = 0.2f;
            widthHeightRatio = 0.6f;
            isShow = true;
            boxType = 0;
        }

        /// <summary>
        /// 弹出框显示的内容页面
        /// </summary>
        [DataMember]
        public string showUrl { get; set; }

        /// <summary>
        /// 宽度占屏幕宽度的比例
        /// </summary>
        [DataMember]
        public float widthRatio { get; set; }

        /// <summary>
        /// 宽高比
        /// </summary>
        [DataMember]
        public float widthHeightRatio { get; set; }

        /// <summary>
        /// lazyLoadTime 单位秒
        /// </summary>
        [DataMember]
        public float lazyLoadTime { get; set; }

        /// <summary>
        /// 是否显示
        /// </summary>
        [DataMember]
        public bool isShow { get; set; }

        /// <summary>
        /// 弹出最大次数
        /// </summary>
        [DataMember]
        public int frequency { get; set; }

        /// <summary>
        /// 框id
        /// </summary>
        [DataMember]
        public string boxId { get; set; }

        /// <summary>
        /// 框id
        /// </summary>
        [DataMember]
        public int boxType { get; set; }
    }

    [DataContract]
    public class ClickUsefulPeopleItem{

        /// <summary>
        /// 点赞者昵称
        /// </summary>
        [DataMember]
        public string NickName { get; set; }

        /// <summary>
        /// 点赞者头像
        /// </summary>
        [DataMember]
        public string AvatarUrl { get; set; }

        /// <summary>
        /// 跳转到主页
        /// </summary>
        [DataMember]
        public long UserID { get; set; }
    }

    [DataContract]
    public class CommentReviewsItem
    {
        /// <summary>
        /// 该条评论或回复 的 头像链接
        /// </summary>
        [DataMember]
        public string AvatarUrl { get; set; }

        /// <summary>
        /// 该条评论或回复 的 作者
        /// </summary>
        [DataMember]
        public long UserID { get; set; }

        /// <summary>
        /// 该条评论或回复 的 昵称
        /// </summary>
        [DataMember]
        public string NickName { get; set; }

        /// <summary>
        /// 该条评论或回复 的 内容
        /// </summary>
        [DataMember]
        public string Content { get; set; }

        /// <summary>
        /// 该条评论或回复 的 时间描述 可扩展
        /// </summary>
        [DataMember]
        public string TimeDesc { get; set; }

        /// <summary>
        /// 写的时间
        /// </summary>
        [DataMember]
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 该条评论或回复ID
        /// </summary>
        [DataMember]
        public long ReviewId { get; set; }

        /// <summary>
        /// 针对该条评论的评论（或回复） 需要附属的上级评论（或回复）ID
        /// </summary>
        [DataMember]
        public long ParentReviewId { get; set; }

        /// <summary>
        /// 此条内容是否可回复
        /// </summary>
        [DataMember]
        public bool IsCanReply { get; set; }

        /// <summary>
        /// 下级的评论或回复
        /// </summary>
        [DataMember]
        public List<CommentReviewsItem> subItems { get; set; }
    }

    [DataContract]
    public class CategoryPhotosEntity
    {
        [DataMember]
        public int CommentID { get; set; }

        [DataMember]
        public int CategoryID { get; set; }
        
        [DataMember]
        public string CategoryName { get; set; }
        
        [DataMember]
        public List<PicShortInfo> PhotoUrls { get; set; }
        
        [DataMember]
        public List<PicShortInfo> BigPhotoUrls { get; set; }
    }

    [DataContract]
    public class PicShortInfo
    {
        /// <summary>
        /// 图片高度
        /// </summary>
        [DataMember]
        public int Height { get; set; }

        /// <summary>
        /// 图片宽度
        /// </summary>
        [DataMember]
        public int Width { get; set; }

        /// <summary>
        /// 图片可访问连接
        /// </summary>
        [DataMember]
        public string PicUrl { get; set; }
    }

    [DataContract]
    public class CommentUsefulParam
    {
        /// <summary>
        /// 文章
        /// </summary>
        [DataMember]
        public int CommentID { get; set; }

        /// <summary>
        /// 被点赞的人
        /// </summary>
        [DataMember]
        public int ReceiverUserID { get; set; }

        /// <summary>
        /// 点赞的人
        /// </summary>
        [DataMember]
        public long UserID { get; set; }
        
        /// <summary>
        /// 渠道ID
        /// </summary>
        [DataMember]
        public int ChannelID { get; set; }

        /// <summary>
        /// 是否有帮助
        /// </summary>
        [DataMember]
        public bool IsUseful { get; set; }
    }

    [DataContract]
    public class PersonalHomePageModel
    {
        public PersonalHomePageModel()
        {
            homeUserData = new UserData();
            commentData = new CommentData();
            NoCommentHotelDescribe = "";
            NoCommentHotelName = "";
        }

        [DataMember]
        public UserData homeUserData { get; set; }

        /// <summary>
        /// 最近未点评酒店描述
        /// </summary>
        [DataMember]
        public string NoCommentHotelDescribe { get; set; }
        /// <summary>
        /// 酒店id
        /// </summary>
        [DataMember]
        public int NoCommentHotelID { get; set; }
        /// <summary>
        /// 酒店名称
        /// </summary>
        [DataMember]
        public string NoCommentHotelName { get; set; }
        /// <summary>
        /// 订单id
        /// </summary>
        [DataMember]
        public long OrderId { get; set; }

        /// <summary>
        /// 点评数据
        /// </summary>
        [DataMember]
        public CommentData commentData { get; set; }
    }

    [DataContract]
    public class PersonalHomePageModel20
    {
        public PersonalHomePageModel20()
        {
            homeUserData = new UserData();
            recommendCommentData = new CommentDataGroupByDistrict();
            notRecommendCommentData = new CommentDataGroupByDistrict();
        }

        [DataMember]
        public UserData homeUserData { get; set; }

        /// <summary>
        /// 推荐的点评数据
        /// </summary>
        [DataMember]
        public CommentDataGroupByDistrict recommendCommentData { get; set; }

        /// <summary>
        /// 不推荐的点评数据
        /// </summary>
        [DataMember]
        public CommentDataGroupByDistrict notRecommendCommentData { get; set; }
    }
    
    [DataContract]
    public class UserData
    {
        /// <summary>
        /// 昵称
        /// </summary>
        [DataMember]
        public string NickName { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        [DataMember]
        public string AvatarUrl { get; set; }

        /// <summary>
        /// 主页用户ID
        /// </summary>
        [DataMember]
        public long HomeUserID { get; set; }

        /// <summary>
        /// 是否为品鉴师
        /// </summary>
        [DataMember]
        public bool IsInspector { get; set; }

        /// <summary>
        /// 当前App用户粉丝数
        /// </summary>
        [DataMember]
        public int FollowersCount { get; set; }

        /// <summary>
        /// 当前App用户关注数
        /// </summary>
        [DataMember]
        public int FollowingsCount { get; set; }

        /// <summary>
        /// 0 没有关系 1 我的粉丝 2 我的关注 3 相互关注
        /// </summary>
        [DataMember]
        public int FollowState { get; set; }

        /// <summary>
        /// 个性签名
        /// </summary>
        [DataMember]
        public string PersonalSignature { get; set; }

        /// <summary>
        /// 主题
        /// </summary>
        [DataMember]
        public string ThemeCodeSN { get; set; }

        /// <summary>
        /// 客户类型
        /// </summary>
        [DataMember]
        public int CustomerType { get; set; }
    }

    [DataContract]
    public class CommentData
    {
        public CommentData()
        {
            commentList = new List<HomePageComemntItem>();
        }

        /// <summary>
        /// 点评总数
        /// </summary>
        [DataMember]
        public int TotalCount { get; set; }
        
        /// <summary>
        /// 点评列表
        /// </summary>
        [DataMember]
        public List<HomePageComemntItem> commentList { get; set; }
    }

    [DataContract]
    public class CommentDataGroupByDistrict
    {
        public CommentDataGroupByDistrict()
        {
            commentGroup = new Dictionary<string, List<HomePageComemntItem>>();
        }

        /// <summary>
        /// 点评总数（推荐或不推荐标签下面）
        /// </summary>
        [DataMember]
        public int TotalCount { get; set; }

        /// <summary>
        /// 地区名称对应的点评组
        /// </summary>
        [DataMember]
        public Dictionary<string, List<HomePageComemntItem>> commentGroup { get; set; }
    }

    [DataContract]
    public class HomePageComemntItem
    {
        [DataMember]
        public List<string> commentPics { get; set; }
        [DataMember]
        public List<string> bigCommentPics { get; set; }
        [DataMember]
        public string hotelName { get; set; }
        [DataMember]
        public int hotelID { get; set; }
        [DataMember]
        public string hotelPic { get; set; }
        /// <summary>
        /// 用户自己一句话的总结
        /// </summary>
        [DataMember]
        public string commentTitle { get; set; }
        /// <summary>
        /// 用户详细的点评内容
        /// </summary>
        [DataMember]
        public string commentContent { get; set; }
        /// <summary>
        /// 用户点评日期
        /// </summary>
        [DataMember]
        public DateTime commentDate { get; set; }
        /// <summary>
        /// 日期描述
        /// </summary>
        [DataMember]
        public string TimeDesc { get; set; }

        /// <summary>
        /// 价格
        /// </summary>
        [DataMember]
        public decimal MinPrice { get; set; }

        /// <summary>
        /// 价格类型
        /// </summary>
        [DataMember]
        public int PriceType { get; set; }


        [DataMember]
        public bool isRecommend { get; set; }
        [DataMember]
        public float commentScore { get; set; }
        [DataMember]
        public int commentID { get; set; }
        /// <summary>
        /// 点评的评论数
        /// </summary>
        [DataMember]
        public int reviewCount { get; set; }
        /// <summary>
        /// 有帮助的数量
        /// </summary>
        [DataMember]
        public int helpfulCount { get; set; }
        /// <summary>
        /// 是否点过有用
        /// </summary>
        [DataMember]
        public bool hasClickUseful { get; set; }
        /// <summary>
        /// 房型
        /// </summary>
        [DataMember]
        public string roomType { get; set; }
        /// <summary>
        /// 出游类型
        /// </summary>
        [DataMember]
        public string tripType { get; set; }
    }

    [DataContract]
    public class CommentReviewsResult
    {
        /// <summary>
        /// 回复或点评总数（应该是genji）
        /// </summary>
        [DataMember]
        public int reviewsCount { get; set; }

        [DataMember]
        public List<CommentReviewsItem> reviewItems { get; set; }
    }

    [DataContract]
    public class CommentReviewsParam:BaseParam
    {
        /// <summary>
        /// 点评ID
        /// </summary>
        [DataMember]
        public int commentId { get; set; }
        
        [DataMember]
        public long commentUserID { get; set; }

        [DataMember]
        public long curUserID { get; set; }

        [DataMember]
        public int start { get; set; }

        [DataMember]
        public int count { get; set; }
    }
}