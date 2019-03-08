using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HJD.Search.CommonLibrary.Model
{
    /// <summary>
    /// 酒店索引库返回模型
    /// </summary>
    public class HotelIndexResult
    {
        public HotelIndexResult()
        {
            HotelIndexResultEntity hEntity = new HotelIndexResultEntity();
            Type t = hEntity.GetType();
            FieldInfos = t.GetFields(BindingFlags.Public);
        }

        public List<HotelIndexResultEntity> HotelEntityList = new List<HotelIndexResultEntity>();

        public FieldInfo[] FieldInfos;

    }
}
