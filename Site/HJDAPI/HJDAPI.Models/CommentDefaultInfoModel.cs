using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.Models
{
    public class CommentDefaultInfoModel
    {
        public CommentDefaultInfoModel()
        {
            AlertResult = new AlertPointsRuleResult();
            BlockInfo = new List<CommentBlockInfo>();
        }

        public List<CommentBlockInfo> BlockInfo { get; set; }

        public AlertPointsRuleResult AlertResult { get; set; }
    }

    public class AlertPointsRuleResult
    {
        public AlertPointsRuleResult()
        {
            IsNeed = false;
            Message = "";
        }

        public bool IsNeed { get; set; }
        public string Message { get; set; }
    }

    public class CommentBlockInfo
    {
        public CommentBlockInfo()
        {
            BlockCategoryName = "";
            additionTips = "";
            TagBlockList = new List<CommentTagBlock>();
        }

        public int BlockCategory { get; set; } //模块ID，与主CommentTagBlock ID保持一致，以便AddInfo与CommentTagBlock 关联
        public string BlockCategoryName { get; set; }
        public List<CommentTagBlock> TagBlockList { get; set; }
        public string additionTips { get; set; }
    }

    public class CommentTagBlock
    {
        public CommentTagBlock()
        {
            CategoryName = "";
            CategoryTitle = "";
            CommentTagList = new List<CommentTag>();
            additionTips = "";
            addScores = new List<AdditionalScore>();
        }

        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public string CategoryTitle { get; set; }
        /// <summary>
        /// 需要显示的标签块,如果数量大于0则显示,没有则不显示
        /// </summary>
        public List<CommentTag> CommentTagList { get; set; }
        /// <summary>
        /// 最多选几个标签
        /// </summary>
        public int MaxTags { get; set; }
        /// <summary>
        /// 最少选几个标签
        /// </summary>
        public int MinTags { get; set; }
        /// <summary>
        /// 最多可上传的照片数量
        /// </summary>
        public int PicCount { get; set; }
        /// <summary>
        /// 最多可写多少字数
        /// </summary>
        public int MaxLength { get; set; }
        /// <summary>
        /// 最少可写多少字数
        /// </summary>
        public int MinLength { get; set; }
        /// <summary>
        /// 默认显示内容块的行数（包括标签块 以及 文本框的内容）
        /// </summary>
        public int DefaultRowCount { get; set; }
        /// <summary>
        /// 文本内容 非标签的内容 如果值非空则代表需要显示文本框
        /// </summary>
        public string additionTips { get; set; }
        /// <summary>
        /// 某一类内容下属需要打分的清单 数组长度大于0则显示
        /// </summary>
        public List<AdditionalScore> addScores { get; set; }
    }

    /// <summary>
    /// 打分项
    /// </summary>
    [DataContract]
    public class AdditionalScore
    {
        [DataMember]
        public int CommentID { get; set; }
        /// <summary>
        /// 标明哪一类打分
        /// </summary>
        [DataMember]
        public int ScoreType { get; set; }
        [DataMember]
        public string ScoreName { get; set; }
        /// <summary>
        /// 得分 可写小数
        /// </summary>
        [DataMember]
        public float Score { get; set; }
    }

    /// <summary>
    /// 点评片段
    /// </summary>
    [DataContract]
    public class AdditionalSection
    {
        /// <summary>
        /// ID
        /// </summary>
        [DataMember]
        public int ID { get; set; }
        /// <summary>
        /// 点评ID
        /// </summary>
        [DataMember]
        public int CommentID { get; set; }
        /// <summary>
        /// 照片
        /// </summary>
        [DataMember]
        public string PicSUrl { get; set; }
        /// <summary>
        /// 照片的链接
        /// </summary>
        [DataMember]
        public string PicUrl { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        [DataMember]
        public string Brief { get; set; }
        /// <summary>
        /// SequenceNo
        /// </summary>
        [DataMember]
        public Int32 SequenceNo { get; set; }
        /// <summary>
        /// 比例 高度/宽度
        /// </summary>
        [DataMember]
        public double Ratio { get; set; }
    }

    public class CommentTag
    {
        public CommentTag()
        {
            Tag = "";
        }

        public int TagID { get; set; }
        public string Tag { get; set; }
        public AddInfo addInfo { get; set; }
    }

    public class AddInfo
    {
        public AddInfo()
        {
            Tips = "";
        }

        public int TypeID { get; set; }
        public string Tips { get; set; }
    }
}