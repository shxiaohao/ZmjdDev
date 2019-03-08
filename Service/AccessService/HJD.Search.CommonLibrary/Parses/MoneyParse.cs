using HJD.AccessService.Contract.Model.Dialog;
using PanGu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJD.Search.CommonLibrary.Parses
{
    public class MoneyParse : ParseBase
    {

        private static string NotMoneyKeyWord = "年月日号路弄";
        internal static void GenMoneyOption(List<AccessService.Contract.Model.Dialog.UserWordOptionItem> uwl, List<PanGu.WordInfo> wordInfoList)
        {
            UserWordOptionItem option = new UserWordOptionItem { ItemType = MagiCallDialogItemType.UserWordOption, OptionType = MagiCallUserWordOptionType.Money };

            List<int> money = new List<int>();

            for(int i =0; i < wordInfoList.Count; i++)
            {
                if(wordInfoList[i].WordType == PanGu.WordType.Numeric)
                {
                    int m = 0;
                  if(  int.TryParse(wordInfoList[i].Word,out m) )
                  {
                      WordInfo nextWord = GetNextWord(wordInfoList, i);
                      if (nextWord.Word == "元")
                      {
                          money.Add(m);
                      }
                      else if (m > 100 && nextWord.Word.Length == 1 && NotMoneyKeyWord.IndexOf(nextWord.Word)== -1)
                      {
                          money.Add(m);
                      }
                  }
                }
            }          

            if(money.Count ==1 )
            {
                option.Text = string.Format("{0}元左右", money[0]);
                option.ActionParam = string.Format("Price={0}", money[0]);
                uwl.Add(option);
            }
            else if(money.Count > 1)
            {
                money.Sort();
                option.Text = string.Format("{0}元到{1}元", money.First(), money.Last());
                option.ActionParam = string.Format("MinPrice={0}, MaxPrice={1}", money.First(), money.Last());
                uwl.Add(option);
            }
            
        }


    }
}
