using HJD.AccessService.Contract;
using HJD.Search.CommonLibrary.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJD.Search.CommonLibrary.Config
{
    public class IndexConfig
    {

        public static string SearchIndexRootPath = System.Configuration.ConfigurationManager.AppSettings["SearchIndexRootPath"];

        /// <summary>
        /// 根据索引类型返回该索引库的存放路径
        /// </summary>
        /// <returns></returns>
        public static string GetIndexPathByType(SearchType type)
        { 
            var indexPath = "";

            switch (type)
            {
                case SearchType.Demo:
                    indexPath = SearchIndexRootPath + @"\Index\Demo";
                    break;
                case SearchType.Hotel:
                    indexPath = SearchIndexRootPath + @"\Index\Hotel";
                    break;
                case SearchType.Spot:
                    indexPath = SearchIndexRootPath + @"\Index\Spot";
                    break;
                case SearchType.City:
                    indexPath = SearchIndexRootPath + @"\Index\City";
                    break;
                case SearchType.QaHotel:
                    indexPath = SearchIndexRootPath + @"\Index\QaHotel";
                    break;
            }

            return indexPath;
        }

        public static string[] GetIndexPathsByTypes(List<SearchType> types)
        {
            if (types == null) return new string[0];

            var paths = new string[types.Count];

            for (int i = 0; i < types.Count; i++)
            {
                var t = types[i];
                paths[i] = GetIndexPathByType(t);
            }

            return paths;
        }
    }
}
