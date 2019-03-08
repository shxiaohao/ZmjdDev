using System;
using System.Runtime.Serialization;
using HJD.Framework.Entity;
using System.Collections.Generic;
namespace HJD.HotelServices.Contracts
{
    [Serializable]
    [DataContract]
    public class CommentKeyWordEntity
    {

        /// <summary>
        /// keyword
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "0", IsOptional = true)]
        public string KeyWord { get; set; }

        /// <summary>
        /// keywordExt
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "0", IsOptional = true)]
        public string KeyWordExt { get; set; }


        /// <summary>
        /// Prop
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "0", IsOptional = true)]
        public string Prop { get; set; }

    }
}
