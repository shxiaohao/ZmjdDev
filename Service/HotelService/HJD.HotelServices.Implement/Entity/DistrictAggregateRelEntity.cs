using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Runtime.Serialization;
using HJD.Framework.Entity;


namespace HJD.HotelServices
{

    [HJD.Framework.Entity.DefaultColumnAttribute()]
    public sealed class DistrictAggregateRelEntity
    {

        #region 映射字段
        private Int32 districtid = 0;

        private Int32 subdistrictid = 0;
        #endregion

        /// <summary>
        /// 
        /// </summary>
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public Int32 DistrictID
        {
            get
            {
                return this.districtid;
            }
            set
            {
                this.districtid = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public Int32 SubDistrictID
        {
            get
            {
                return this.subdistrictid;
            }
            set
            {
                this.subdistrictid = value;
            }
        }
    }
}
