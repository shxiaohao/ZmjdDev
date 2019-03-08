using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJD.Search.CommonLibrary.Model
{
    ///// <summary>
    ///// 快搜智能提示项的类型
    ///// </summary>
    //public enum SearchType
    //{
    //    /// <summary>
    //    /// Demo
    //    /// </summary>
    //    Demo,

    //    /// <summary>
    //    /// 酒店
    //    /// </summary>
    //    Hotel,

    //    /// <summary>
    //    /// 景点
    //    /// </summary>
    //    Spot,

    //    /// <summary>
    //    /// 城市
    //    /// </summary>
    //    City
    //}

    /// <summary>
    /// 搜索器类型
    /// </summary>
    public enum SearcherType
    {
        /// <summary>
        /// 单索引搜索
        /// </summary>
        Single,

        /// <summary>
        /// 多索引搜索
        /// </summary>
        Multi,

        /// <summary>
        /// 多索引并行搜索
        /// </summary>
        ParallelMulti
    }

    /// <summary>
    /// 分词词性
    /// </summary>
    public enum AnalyzerType
    { 
        
    }
}
