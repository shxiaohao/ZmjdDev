using System;
using System.Runtime.Serialization;
using HJD.Framework.Entity;
using System.Collections.Generic;
namespace HJD.HotelServices.Contracts
{
     [Serializable]
    [DataContract]
    public class CommentKeyWordListEntity
    {
      
        /// <summary>
        /// keyword
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "0", IsOptional = true)]
        public string KeyWord { get; set; }

        /// <summary>
        /// keywordext
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "0", IsOptional = true)]
        public List<string> KeyWordExt { get; set; }

    }
}
