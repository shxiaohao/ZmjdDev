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
    public class ActiveRuleGroup
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

        private string title = "";
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 
        /// </summary>
        public string Title
        {
            get { return title; }
            set { title = value; }
        }

        private string subtitle = "";
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 
        /// </summary>
        public string SubTitle
        {
            get { return subtitle; }
            set { subtitle = value; }
        }

        private string picurl = "";
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 
        /// </summary>
        public string PicUrl
        {
            get { return picurl; }
            set { picurl = value; }
        }

        private string description = "";
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 
        /// </summary>
        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        private int activeid = 0;
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 
        /// </summary>
        public int ActiveId
        {
            get { return activeid; }
            set { activeid = value; }
        }

        private int type = 0;
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 
        /// </summary>
        public int Type
        {
            get { return type; }
            set { type = value; }
        }

        private DateTime updatetime = DateTime.Now;
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 
        /// </summary>
        public DateTime UpdateTime
        {
            get { return updatetime; }
            set { updatetime = value; }
        }
    }

    /// <summary>
    /// 活动扩展表
    /// </summary>
    [System.SerializableAttribute()]
    [System.Runtime.Serialization.DataContractAttribute()]
    [HJD.Framework.Entity.DefaultColumnAttribute()]
    public class ActiveRuleEx
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

        private int hotelid = 0;
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 
        /// </summary>
        public int HotelId
        {
            get { return hotelid; }
            set { hotelid = value; }
        }

        private int activeid = 0;
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 
        /// </summary>
        public int ActiveId
        {
            get { return activeid; }
            set { activeid = value; }
        }

        private int groupid = 0;
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 
        /// </summary>
        public int GroupId
        {
            get { return groupid; }
            set { groupid = value; }
        }

        private string title = "";
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 
        /// </summary>
        public string Title
        {
            get { return title; }
            set { title = value; }
        }
        
        private string subtitle = "";
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 
        /// </summary>
        public string SubTitle
        {
            get { return subtitle; }
            set { subtitle = value; }
        }

        private string picurl = "";
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 
        /// </summary>
        public string PicUrl
        {
            get { return picurl; }
            set { picurl = value; }
        }

        private string roominfo = "";
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 
        /// </summary>
        public string RoomInfo
        {
            get { return roominfo; }
            set { roominfo = value; }
        }

        private int offercount = 0;
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 
        /// </summary>
        public int OfferCount
        {
            get { return offercount; }
            set { offercount = value; }
        }

        private string description = "";
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 
        /// </summary>
        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        private int ordernum = 0;
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 
        /// </summary>
        public int OrderNum
        {
            get { return ordernum; }
            set { ordernum = value; }
        }

        private DateTime updatetime = DateTime.Now;
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 
        /// </summary>
        public DateTime UpdateTime
        {
            get { return updatetime; }
            set { updatetime = value; }
        }

    }


    /// <summary>
    /// 活动扩展表-投票活动使用
    /// </summary>
    [System.SerializableAttribute()]
    [System.Runtime.Serialization.DataContractAttribute()]
    [HJD.Framework.Entity.DefaultColumnAttribute()]
    public class ActiveRuleExForVote : ActiveRuleEx
    {
        private int votecount = 0;
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 投票数量
        /// </summary>
        public int VoteCount
        {
            get { return votecount; }
            set { votecount = value; }
        }

        private int rankNo = 0;
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 大使排名（在该item下的排名）
        /// </summary>
        public int RankNo
        {
            get { return rankNo; }
            set { rankNo = value; }
        }

        private DateTime lastCreateTime = DateTime.Parse("2018-11-01");
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 大使最后在该item下的投票时间
        /// </summary>
        public DateTime LastCreateTime
        {
            get { return lastCreateTime; }
            set { lastCreateTime = value; }
        }
    }

    /// <summary>
    /// 活动奖品对象
    /// </summary>
    [System.SerializableAttribute()]
    [System.Runtime.Serialization.DataContractAttribute()]
    [HJD.Framework.Entity.DefaultColumnAttribute()]
    public class ActiveRulePrize
    {
        private int id = 0;
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// ID
        /// </summary>
        public int ID
        {
            get { return id; }
            set { id = value; }
        }

        private string code = "";
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// Code
        /// </summary>
        public string Code
        {
            get { return code; }
            set { code = value; }
        }

        private string name = "";
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 奖品名称
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private string description = "";
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 奖品描述
        /// </summary>
        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        private string picture = "";
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 奖品图片地址
        /// </summary>
        public string Picture
        {
            get { return picture; }
            set { picture = value; }
        }

        private string tagName = "";
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 奖品标签
        /// </summary>
        public string TagName
        {
            get { return tagName; }
            set { tagName = value; }
        }

        private string levelName = "";
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 奖品等级名称
        /// </summary>
        public string LevelName
        {
            get { return levelName; }
            set { levelName = value; }
        }

        private int count = 0;
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 奖品数量
        /// </summary>
        public int Count
        {
            get { return count; }
            set { count = value; }
        }

        private decimal price = 0;
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 奖品价格
        /// </summary>
        public decimal Price
        {
            get { return price; }
            set { price = value; }
        }

        private int sourceId = 0;
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 奖品来源ID（比如ActiveRuleEx表的ID）
        /// </summary>
        public int SourceId
        {
            get { return sourceId; }
            set { sourceId = value; }
        }

        private int sourceType = 0;
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 奖品来源类型（比如ActiveRuleEx）
        /// </summary>
        public int SourceType
        {
            get { return sourceType; }
            set { sourceType = value; }
        }

        private int activeId = 0;
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 奖品关联归属的活动ID
        /// </summary>
        public int ActiveId
        {
            get { return activeId; }
            set { activeId = value; }
        }
    }

    /// <summary>
    /// 活动大使的关联记录
    /// </summary>
    [System.SerializableAttribute()]
    [System.Runtime.Serialization.DataContractAttribute()]
    [HJD.Framework.Entity.DefaultColumnAttribute()]
    public class ActiveRuleSpokesman
    {
        private int id = 0;
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// ID
        /// </summary>
        public int ID
        {
            get { return id; }
            set { id = value; }
        }

        private int activeDrawId = 0;
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 活动报名表主键ID
        /// </summary>
        public int ActiveDrawId
        {
            get { return activeDrawId; }
            set { activeDrawId = value; }
        }

        private int ruleExId = 0;
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// ActiveRuleExId
        /// </summary>
        public int RuleExId
        {
            get { return ruleExId; }
            set { ruleExId = value; }
        }

        private int activeId = 0;
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 归属的活动ID
        /// </summary>
        public int ActiveId
        {
            get { return activeId; }
            set { activeId = value; }
        }

        private DateTime createTime = DateTime.Now;
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime
        {
            get { return createTime; }
            set { createTime = value; }
        }
    }

    /// <summary>
    /// 活动大使的关联记录(包含大使报名信息)
    /// </summary>
    [System.SerializableAttribute()]
    [System.Runtime.Serialization.DataContractAttribute()]
    [HJD.Framework.Entity.DefaultColumnAttribute()]
    public class ActiveRuleSpokesmanAndDrawInfo : ActiveRuleSpokesman
    {
        private string nickName = "";
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 大使昵称
        /// </summary>
        public string NickName
        {
            get { return nickName; }
            set { nickName = value; }
        }

        private string headimgurl = "";
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 大使头像
        /// </summary>
        public string Headimgurl
        {
            get { return headimgurl; }
            set { headimgurl = value; }
        }

        private int voteCount = 0;
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 大使已获票数
        /// </summary>
        public int VoteCount
        {
            get { return voteCount; }
            set { voteCount = value; }
        }

        private int mineVoteCount = 0;
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 大使已获票数(根据指定ActiveRuleEx来源的票数)
        /// </summary>
        public int MineVoteCount
        {
            get { return mineVoteCount; }
            set { mineVoteCount = value; }
        }

        private int spokesCount = 0;
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 大使代言的数量
        /// </summary>
        public int SpokesCount
        {
            get { return spokesCount; }
            set { spokesCount = value; }
        }

        private DateTime lastCreateTime = DateTime.Parse("2018-11-01");
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 大使最后得票时间
        /// </summary>
        public DateTime LastCreateTime
        {
            get { return lastCreateTime; }
            set { lastCreateTime = value; }
        }
    }

    /// <summary>
    /// 活动的投票记录
    /// </summary>
    [System.SerializableAttribute()]
    [System.Runtime.Serialization.DataContractAttribute()]
    [HJD.Framework.Entity.DefaultColumnAttribute()]
    public class ActiveVoteRecord
    {
        private int id = 0;
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// ID
        /// </summary>
        public int ID
        {
            get { return id; }
            set { id = value; }
        }

        private string weixinAccount = "";
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 投票微信用户openid
        /// </summary>
        public string WeixinAccount
        {
            get { return weixinAccount; }
            set { weixinAccount = value; }
        }

        private Int64 userId = 0;
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 投票用户的zmjd的userid
        /// </summary>
        public Int64 UserId
        {
            get { return userId; }
            set { userId = value; }
        }

        private int sourceId = 0;
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 被投票对象的ID（SourceType=1时：ActiveRuleEx的ID；SourceType=2时：ActiveWeixinDraw的ID；）
        /// </summary>
        public int SourceId
        {
            get { return sourceId; }
            set { sourceId = value; }
        }

        private int sourceType = 0;
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 被投票对象的身份类型（1：ActiveRuleEx[如酒店大使活动的酒店] 2：ActiveWeixinDraw[如酒店大使活动的报名大使]）
        /// </summary>
        public int SourceType
        {
            get { return sourceType; }
            set { sourceType = value; }
        }

        private int reltionId = 0;
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 投票的关联ID
        /// 1：比如用户A给酒店A投票，但是用户A是通过用户B的拉票投的，那么这里记录下用户B的ActiveWeixinDraw ID
        /// 2：比如用户A给用户B投票，但是用户A是通过用户B给酒店A拉的票投的，那么这里记录下酒店A的ActiveRuleEx ID
        /// </summary>
        public int ReltionId
        {
            get { return reltionId; }
            set { reltionId = value; }
        }

        private int state = 0;
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 票的状态（1有效投票 0无效投票）
        /// </summary>
        public int State
        {
            get { return state; }
            set { state = value; }
        }

        private int activeId = 0;
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 主活动ID
        /// </summary>
        public int ActiveId
        {
            get { return activeId; }
            set { activeId = value; }
        }

        private DateTime createTime = DateTime.Now;
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime
        {
            get { return createTime; }
            set { createTime = value; }
        }
    }

    /// <summary>
    /// 活动的抽奖记录
    /// </summary>
    [System.SerializableAttribute()]
    [System.Runtime.Serialization.DataContractAttribute()]
    [HJD.Framework.Entity.DefaultColumnAttribute()]
    public class ActiveLuckDrawRecord 
    {
        private int id = 0;
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// ID
        /// </summary>
        public int ID
        {
            get { return id; }
            set { id = value; }
        }

        private int activeDrawId = 0;
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 活动用户报名ID
        /// </summary>
        public int ActiveDrawId
        {
            get { return activeDrawId; }
            set { activeDrawId = value; }
        }

        private int prizeId = 0;
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 奖品ID
        /// </summary>
        public int PrizeId
        {
            get { return prizeId; }
            set { prizeId = value; }
        }

        private string remark = "";
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 抽奖备注
        /// </summary>
        public string Remark
        {
            get { return remark; }
            set { remark = value; }
        }

        private int state = 0;
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 抽奖的状态（1有效 0无效）
        /// </summary>
        public int State
        {
            get { return state; }
            set { state = value; }
        }

        private int activeId = 0;
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 主活动ID
        /// </summary>
        public int ActiveId
        {
            get { return activeId; }
            set { activeId = value; }
        }

        private DateTime createTime = DateTime.Now;
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime
        {
            get { return createTime; }
            set { createTime = value; }
        }
    }

    /// <summary>
    /// 活动的抽奖记录（包含奖品信息字段）
    /// </summary>
    [System.SerializableAttribute()]
    [System.Runtime.Serialization.DataContractAttribute()]
    [HJD.Framework.Entity.DefaultColumnAttribute()]
    public class ActiveLuckDrawRecordContainPrize : ActiveRulePrize
    {
        private string recordRemark = "";
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 抽奖备注
        /// </summary>
        public string RecordRemark
        {
            get { return recordRemark; }
            set { recordRemark = value; }
        }

        private DateTime recordTime = DateTime.Now;
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 抽奖记录创建时间
        /// </summary>
        public DateTime RecordTime
        {
            get { return recordTime; }
            set { recordTime = value; }
        }
    }
}
