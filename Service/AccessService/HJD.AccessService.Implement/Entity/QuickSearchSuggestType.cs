using HJD.AccessService.Contract.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace HJD.AccessService.Implement.Entity
{
    /// <summary>
    /// 快搜智能提示项的类型
    /// </summary>
    public enum QuickSearchSuggestType
    {
        /// <summary>
        /// 酒店
        /// </summary>
        Hotel,

        /// <summary>
        /// 景点
        /// </summary>
        Spot,

        /// <summary>
        /// 城市
        /// </summary>
        City
    }
}
