using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJD.Search.CommonLibrary.Model
{
    public class SearchData
    {
        /// <summary>
        /// 列名
        /// </summary>
        public string Column { get; set; }

        /// <summary>
        /// 操作类型
        /// </summary>
        public Operator OperatorType { get; set; }

        /// <summary>
        /// 值
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// 逻辑运算符，and or
        /// </summary>
        public Logic Logic { get; set; }

        public SearchData Clone()
        {
            SearchData obj = new SearchData();

            obj.Column = this.Column;

            obj.Value = this.Value;

            obj.Logic = this.Logic;

            obj.OperatorType = this.OperatorType;

            return obj;
        }

    }

    public enum Logic
    {
        and,
        or
    }

    public enum Operator
    {
        Equal,
        NotEqual,
        Like,
        StartLike,
        EndLike,
        NotLike,
        In,
        NotIn,
        GreaterThan,
        GreaterThanOrEqual,
        LessThan,
        LessThanOrEqual
    }

    public class OrderBy
    {
        public string Coloumn { get; set; }
        public bool IsDesc { get; set; }
    }
}
