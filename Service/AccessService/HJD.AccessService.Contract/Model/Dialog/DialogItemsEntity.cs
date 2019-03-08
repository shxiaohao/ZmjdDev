﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJD.AccessService.Contract.Model.Dialog
{
   ﻿//------------------------------------------------------------------------------
    // <auto-generated>
    //     此代码由工具生成。
    //     运行时版本:4.0.30319.18408
    //
    //     对此文件的更改可能会导致不正确的行为，并且如果
    //     重新生成代码，这些更改将会丢失。
    // </auto-generated>
    //------------------------------------------------------------------------------


    [System.SerializableAttribute()]
    [System.Runtime.Serialization.DataContractAttribute()]
    [HJD.Framework.Entity.DefaultColumnAttribute()]
    public sealed class MagiCallDialogItemsEntity
    {
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public Int64 UserID { get; set; }

        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public DateTime DialogCreateTime { get; set; }

        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public string Creator { get; set; }

        #region 映射字段
        private Int64 idx = 0;

        private Int32 dialogid = 0;

        private Int32 itemtype = 0;

        private String text = "";

        private String @params = "";

        private DateTime createtime = System.DateTime.Now;
        #endregion

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public Int64 IDX
        {
            get
            {
                return this.idx;
            }
            set
            {
                this.idx = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public Int32 DialogID
        {
            get
            {
                return this.dialogid;
            }
            set
            {
                this.dialogid = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public Int32 ItemType
        {
            get
            {
                return this.itemtype;
            }
            set
            {
                this.itemtype = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public String Text
        {
            get
            {
                return this.text;
            }
            set
            {
                this.text = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public String Params
        {
            get
            {
                return this.@params;
            }
            set
            {
                this.@params = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public DateTime CreateTime
        {
            get
            {
                return this.createtime;
            }
            set
            {
                this.createtime = value;
            }
        }
    }
}
