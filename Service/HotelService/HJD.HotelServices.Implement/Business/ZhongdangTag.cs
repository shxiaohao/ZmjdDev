using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections.Concurrent;
using System.Threading;
using HJD.HotelServices.Contracts;
using System.Text.RegularExpressions;
using HJD.HotelServices.Implement.Entity;

namespace HJD.HotelServices
{
    class ZhongdangTag
    {
        static ConcurrentQueue<HotelReview4TagKeywordEntity> queue = null;

        static Dictionary<string, int> dic = null;
        static List<ZhongdangHotelRuleEntity> ruleList = null;

        static ZhongdangTag()
        {
            ruleList = HotelDAL.GetZhongdangRuleList();
        }

        public static void UpdateHotelTag(int writing)
        {
            while (true)
            {
                List<HotelReview4TagKeywordEntity> list = HotelDAL.GetOldHotelReviewData(writing);
                if (list == null || list.Count == 0)
                    break;
                if (queue == null || queue.Count == 0)
                {
                    queue = new ConcurrentQueue<HotelReview4TagKeywordEntity>(list.ToArray());
                    for (int i = 0; i < 40; i++)
                    {
                        new Thread(new ThreadStart(Do)).Start();
                    }
                }
                else
                {
                    list.AsParallel().ForAll(l => queue.Enqueue(l));
                }
                writing = list.Last().Writing;
              //  Thread.Sleep(1000 * 55);
            }
        }

        public static void UpdateZhongdangTag(List<HotelReview4TagKeywordEntity> list)
        {
            queue = new ConcurrentQueue<HotelReview4TagKeywordEntity>(list.ToArray());
            for (int i = 0; i < 40; i++)
            {
                new Thread(new ThreadStart(Do)).Start();
            }

        }

        private static void Do()
        {
            HotelReview4TagKeywordEntity entity = null;
            while (true)
            {
                if (queue.Count == 0)
                    return;
                if (queue.TryDequeue(out entity))
                {
                    ISet<int> tagset = GetZhongdangTag(entity);
                    UpdateDB(tagset, entity);
                }
            }
        }

        private static void UpdateDB(ISet<int> tagset, HotelReview4TagKeywordEntity he)
        {
            
            foreach(int tagId in tagset)
            {
                //int num = HotelDAL.GetZhongdangTagNum(he.Resource, tagId);

                HotelDAL.AddZhongdangTag(he.Resource, he.Writing, tagId);
            }
            
        }

        private static ISet<int> GetZhongdangTag(HotelReview4TagKeywordEntity he)
        {
            ISet<int> tagSet = new HashSet<int>();

            string content = he.Content;
            string[] sentences = content.Split(',', '，', '.', '。', ';', '；', '！', '!', '?', '？');
            
            foreach (ZhongdangHotelRuleEntity jr in ruleList) //检查每一条规则
            {
                foreach (string sentence in sentences) //检查每一个分句
                {
                    if (string.IsNullOrEmpty(sentence.Trim()))
                    {
                        continue;
                    }
                    #region 判断该分句是否可以打标签
                   
                    if (!string.IsNullOrEmpty(jr.Must))  //判断must条件
                    {
                        string[] musts = jr.Must.Split('+');
                        bool iftag = true;
                        //
                        iftag = musts.ToList().Find(must => !sentence.Contains(must)) == null;
                        if(!iftag)
                            continue;
                    }

                    if(!string.IsNullOrEmpty(jr.Mustnot)) //判断mustnot条件
                    {
                        string[] mustnots = jr.Mustnot.Split('+');
                        bool iftag = true;
                        foreach (string mustnot in mustnots)
                        {
                            if(mustnot.Contains("-"))
                            {
                                string[] sec = mustnot.Split('-');
                                int totalcount = GetCount(sentence, sec[0]);
                                int count = 0;
                                for (int i = 1; i < sec.Length; i++ )
                                {
                                    count += GetCount(sentence, sec[i]);
                                }
                                if(totalcount > count)
                                {
                                    iftag = false;
                                    break;
                                }
                            }
                            else 
                            {
                                if (sentence.Contains(mustnot))
                                {
                                    iftag = false;
                                    break;
                                }
                            }
                        }
                        if (!iftag)
                            continue;
                    }


                    if(!string.IsNullOrEmpty(jr.Should))  //判断should条件  A|b+c|d+e*f+g+h|i+j-k|l+m_n   |:或，并列组条件  *：与，组内被*分割条件的与 +：或，组内或  _:与，被+分割后的与  -：非，被+分割后的非 
                    {
                        string[] sections = jr.Should.Split('|'); //分组， 组间只有一个条件成立就算成立

                        bool bSection = false;
                        //Should1 Should2 ...
                        foreach (string section in sections)
                        {
                            string[] ands = section.Split('*');
                            bool bAnd = true;
                            foreach (string and in ands)
                            {

                                #region 每一个Should
                                string[] shoulds = and.Split('+');
                                bool bShould = false;
                                foreach (string should in shoulds)
                                {
                                    if (should.Contains("_"))
                                    {
                                        bool ifhavethisword = true;
                                        string[] temp = should.Split('_');
                                        foreach (string word in temp)
                                        {
                                            if (!sentence.Contains(word))
                                            {
                                                ifhavethisword = false;
                                                break;
                                            }
                                        }
                                        if (ifhavethisword)
                                        {
                                            bShould = true;
                                            break;
                                        }
                                    }
                                    else if (should.Contains("-"))
                                    {
                                        bool ifhavethisword = true;
                                        string[] temp = should.Split('-');
                                        if (!sentence.Contains(temp[0]))
                                        {
                                            ifhavethisword = false;
                                        }
                                        else
                                        {
                                            for (int i = 1; i < temp.Length; i++)
                                            {
                                                if (sentence.Contains(temp[i]))
                                                {
                                                    ifhavethisword = false;
                                                    break;
                                                }
                                            }
                                        }
                                        if (ifhavethisword) bShould = true;
                                    }
                                    else
                                    {
                                        if (sentence.Contains(should))
                                        {
                                            bShould = true;
                                            break;
                                        }
                                    }
                                }
                           #endregion     
                                
                                 if (bShould == false) //如果为假，那么AND为假，退出
                                {
                                    bAnd = false;
                                    break;
                                }
                                
                              
                            }
                            if (bAnd == true) //如果为真，那么section为真，退出
                            {
                                bSection = true;
                                break;
                            }
                        }
                        if (bSection == false) //如果条件不符合，那么跳出，继续分析下一条点评
                            continue;
                    }


                    tagSet.Add(jr.ID);
                    break;
                    #endregion
                }
            }

            Filter(tagSet, he);

            return tagSet;
        }

        private static void Filter(ISet<int> set, HotelReview4TagKeywordEntity he)
        {
            if(he.Resource == 79768)
            {
                set.Remove(62);//去除该酒店的国宾馆标签
            }
        }

        private static int GetCount(string str, string flag)
        {
            int count = 0;
            Match match = Regex.Match(str, flag);
            while (match.Success)
            {
                count++;
                match = match.NextMatch();
            }
            return count;
        }
    }
}
