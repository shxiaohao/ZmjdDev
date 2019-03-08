using HJD.HotelServices.Implement.Entity;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace HJD.HotelServices
{
    class SeoKeyword
    {
        static ConcurrentQueue<HotelReview4TagKeywordEntity> queue = null;

        static List<SeoKeywordRuleEntity> ruleList = null;

        static SeoKeyword()
        {
            ruleList = HotelDAL.GetSeoKeywordRuleList();
        }

        public static void UpdateSeoKeyword(int writing)
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
                writing = list[list.Count - 1].Writing;
                Thread.Sleep(1000 * 55);
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
                    ISet<int> keywordset = GetSeoKeyword(entity);
                    UpdateDB(keywordset, entity);
                }
            }
        }

        private static void UpdateDB(ISet<int> keywordset, HotelReview4TagKeywordEntity he)
        {
            
            foreach(int keywordId in keywordset)
            {
                //int num = HotelDAL.GetZhongdangTagNum(he.Resource, tagId);

                HotelDAL.AddSeoKeyword(he.Resource, he.Writing, keywordId);
            }
            
        }

        private static ISet<int> GetSeoKeyword(HotelReview4TagKeywordEntity he)
        {
            ISet<int> tagSet = new HashSet<int>();
                        
            foreach (SeoKeywordRuleEntity jr in ruleList)
            {
                if (string.IsNullOrEmpty(he.Content.Trim()) || !he.Content.Contains(jr.Must))
                    continue;
                tagSet.Add(jr.ID);
            }

            //Filter(tagSet, he);

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
