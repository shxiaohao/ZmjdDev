﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HJD.AccessService.Contract.Model
{
    [Serializable]
    [DataContract]
    /// <summary>
    /// 目的地数据模型
    /// </summary>
    public class DistrictInfoEntity
    {
        [DataMemberAttribute()]
        public int DistrictId;

        [DataMemberAttribute()]
        public string Name;

        [DataMemberAttribute()]
        public string EName;

        [DataMemberAttribute()]
        public int DistrictType;
    }
}
