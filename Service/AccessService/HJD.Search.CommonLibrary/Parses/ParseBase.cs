using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJD.Search.CommonLibrary.Parses
{
    public class ParseBase
    {
       public static PanGu.WordInfo  GetNextWord(List<PanGu.WordInfo> wordInfoList,int curIndex, int skip=0)
        {
           if( wordInfoList.Count > curIndex +1 + skip )
           {
               return wordInfoList[curIndex + 1 + skip];
           }
           else
           {
               return new PanGu.WordInfo();
           }
        }
    }
}
