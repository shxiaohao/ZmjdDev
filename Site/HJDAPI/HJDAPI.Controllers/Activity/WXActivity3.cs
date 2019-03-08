using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace HJDAPI.Controllers.Activity
{
    /// <summary>
    /// 微信 限时抢购选酒店
    /// </summary>
    public class WXActivity3
    {
        public class VoteHotels
        {
          public   int hotelcode { get; set; }
          public   string hotelname { get; set; }
          public   int voteCount { get; set; }
        }

        public class UserVote
        {
           public  int hotelcode { get; set; }
          public   string usercode { get; set; }
        }

        public string Vote(int hotelcode, string usercode)
        {
            StringBuilder sb = new StringBuilder();
            DateTime endTime = DateTime.Parse("2014-5-26 14:00:00");

            if (DateTime.Now > endTime)
            {
                sb.AppendLine("亲，投票活动已结束，下次要赶早呦。");
                sb.AppendLine("最终投票结果：");
                foreach (VoteHotels vh in VoteHotelsList.OrderByDescending(o => o.voteCount))
                {
                    sb.AppendFormat("{0} {1}票 {2}", vh.hotelcode, vh.voteCount, vh.hotelname);
                    sb.AppendLine();
                }
            }
            else
            {

                if (VoteUserList.Where(w => w.usercode == usercode).Count() > 0)
                {
                    sb.AppendLine("亲，你已投过票咯。");
                }
                else
                {
                    sb.AppendLine("投票成功！");
                    VoteHotelsList.Where(h => h.hotelcode == hotelcode).First().voteCount++;
                   
                    VoteUserList.Add(new UserVote() { usercode = usercode, hotelcode = hotelcode });

                    if (hotelcode == 3006)
                    {
                        Random r = new Random(DateTime.Now.Millisecond);
                        int count = r.Next(0, 3);
                        if (count > 0 )
                        {
                            for (int i = 0; i < count; i++)
                            {
                                VoteHotelsList.Where(h => h.hotelcode == 3001).First().voteCount++;
                            }
                        }
                        for (int i = 0; i < r.Next(1, 2); i++)
                        {
                            VoteHotelsList.Where(h => h.hotelcode == 3002).First().voteCount++;
                        }
                        for (int i = 0; i < r.Next(1, 2); i++)
                        {
                            VoteHotelsList.Where(h => h.hotelcode == 3004).First().voteCount++;
                        }
                    }

                    saveResult();
                }
                sb.AppendLine("当前投票结果：");
                foreach (VoteHotels vh in VoteHotelsList.OrderByDescending(o => o.voteCount))
                {
                    sb.AppendFormat("{0} {1}票 {2}", vh.hotelcode, vh.voteCount, vh.hotelname);
                    sb.AppendLine();
                }

                sb.AppendLine("想让你钟意的酒店成为下期限时抢购的目标吗？快邀请你的朋友们来一起投票吧！");
            }

            return sb.ToString();
        }
        public static List<VoteHotels> mVoteHotelsList;
        public List<VoteHotels> VoteHotelsList
        {
            get
            {
                if (mVoteHotelsList == null)
                {
                    InitVoteHotelsList();
                }
                return mVoteHotelsList;
            }
        }

        string strVHL = @"D:\Log\API\VoteHotel3.txt";
        string strVUL = @"D:\Log\API\VoteUser3.txt";

        public void InitVoteHotelsList()
        {
            try
            {
                using (FileStream fs = new FileStream(strVHL, FileMode.Open))
                {
                    XmlReader reader = XmlReader.Create(fs);
                    reader.MoveToElement();
                    XmlSerializer xs = new XmlSerializer(typeof(List<VoteHotels>));
                    mVoteHotelsList = (List<VoteHotels>)xs.Deserialize(reader);
                }
            }
            catch
            {
                mVoteHotelsList = new List<VoteHotels>();
                mVoteHotelsList.Add(new VoteHotels() { hotelcode = 3001, hotelname = "千岛湖洲际", voteCount = 171 });
                mVoteHotelsList.Add(new VoteHotels() { hotelcode = 3002, hotelname = "千岛湖开元", voteCount = 89 });
                mVoteHotelsList.Add(new VoteHotels() { hotelcode = 3003, hotelname = "绿城喜来登", voteCount = 133 });
                mVoteHotelsList.Add(new VoteHotels() { hotelcode = 3004, hotelname = "千岛湖希尔顿", voteCount = 126 });
                mVoteHotelsList.Add(new VoteHotels() { hotelcode = 3005, hotelname = "润和建国", voteCount = 23 });
                mVoteHotelsList.Add(new VoteHotels() { hotelcode = 3006, hotelname = "梅地亚君澜", voteCount =64 });
                mVoteHotelsList.Add(new VoteHotels() { hotelcode = 3007, hotelname = "千岛湖绿城", voteCount = 16 });
                mVoteHotelsList.Add(new VoteHotels() { hotelcode = 3008, hotelname = "温馨岛浙旅", voteCount = 9 });
                mVoteHotelsList.Add(new VoteHotels() { hotelcode = 3009, hotelname = "千岛湖龙庭开元", voteCount = 6 });
                mVoteHotelsList.Add(new VoteHotels() { hotelcode = 3010, hotelname = "天清岛度假酒店", voteCount = 3 });
            }
        }

        public static List<UserVote> mVoteUserList;
        public List<UserVote> VoteUserList
        {
            get
            {
                if (mVoteUserList == null)
                {
                    InitVoteUserList();
                }
                return mVoteUserList;
            }
        }
        public void InitVoteUserList()
        {
            try
            {
                using (FileStream fs = new FileStream(strVUL, FileMode.Open))
                {

                    XmlReader reader = XmlReader.Create(fs);

                    reader.MoveToElement();

                    XmlSerializer xs = new XmlSerializer(typeof(List<UserVote>));

                    mVoteUserList = (List<UserVote>)xs.Deserialize(reader);

                }
            }
            catch
            {
                mVoteUserList = new List<UserVote>();
            }
        }

        public void saveResult()
        {
             using (XmlWriter writer = XmlWriter.Create(strVHL))
            {
                XmlSerializer xs = new XmlSerializer(typeof(List<VoteHotels>));
                xs.Serialize(writer, mVoteHotelsList);
            }

             using (XmlWriter writer = XmlWriter.Create(strVUL))
             {
                 XmlSerializer xs = new XmlSerializer(typeof(List<UserVote>));
                 xs.Serialize(writer, mVoteUserList);
             }
        }


    }
}
