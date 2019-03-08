using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJD.Search.CommonLibrary.Model
{
    public class DocumentData
    {
        public List<FieldData> FieldDataList = new List<FieldData>();
        public float Boost = 1F;
    }

    public class FieldData
    {
        public string Column { get; set; }
        public string Value { get; set; }
        public float Weight { get; set; }
        public bool IsStore { get; set; }
        public bool IsSort { get; set; }

        public DataTypes DataType { get; set; }

    }

    public enum DataTypes
    {
        String = 0,
        Int = 1,
        DateTime = 2,
        Other = 3
    }
}
