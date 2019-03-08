using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using HJD.Framework.Entity;


namespace HJD.HotelServices.Contracts
{
    /// <summary>
    /// 酒店点评提交后返回状态数据
    /// </summary>
    [Serializable]
    [DataContract]
    public class HotelReviewSaveResult
    {
        /// <summary>
        /// 保存返回的状态 0 保存成功
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(IsOptional = true, DefaultValue = "-1")]
        public int Retcode { get; set; }

        /// <summary>
        /// 返回的点评ID
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(IsOptional = true, DefaultValue = "0")]
        public int Writing { get; set; }

        /// <summary>
        /// CommDB 如果已经点评的话，返回已经点评的ID
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(IsOptional = true, DefaultValue = "0")]
        public int WritingReturn { get; set; }
    }
}
