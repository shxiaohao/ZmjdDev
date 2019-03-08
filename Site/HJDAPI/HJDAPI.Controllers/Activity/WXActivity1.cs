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
    public class WXActivity1
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
            DateTime endTime = DateTime.Parse("2014-5-11 20:00:00");

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

                    saveResult();
                }

                sb.AppendLine("最终投票结果：");
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

        string strVHL = @"D:\Log\API\VoteHotel.txt";
        string strVUL = @"D:\Log\API\VoteUser.txt";

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
                mVoteHotelsList.Add(new VoteHotels() { hotelcode = 1001, hotelname = "良渚君澜度假", voteCount = 0 });
                mVoteHotelsList.Add(new VoteHotels() { hotelcode = 1002, hotelname = "开元九龙湖度假村", voteCount = 0 });
                mVoteHotelsList.Add(new VoteHotels() { hotelcode = 1003, hotelname = "金鸡湖凯宾斯基", voteCount = 0 });
                mVoteHotelsList.Add(new VoteHotels() { hotelcode = 1004, hotelname = "世豪全套间", voteCount = 0 });
                mVoteHotelsList.Add(new VoteHotels() { hotelcode = 1005, hotelname = "上海怡沁园度假村", voteCount = 0 });
                mVoteHotelsList.Add(new VoteHotels() { hotelcode = 1006, hotelname = "千岛湖洲际度假", voteCount = 0 });
                mVoteHotelsList.Add(new VoteHotels() { hotelcode = 1007, hotelname = "常州万达喜来登", voteCount = 0 });
                mVoteHotelsList.Add(new VoteHotels() { hotelcode = 1008, hotelname = "阳澄湖费尔蒙", voteCount = 0 });
                mVoteHotelsList.Add(new VoteHotels() { hotelcode = 1009, hotelname = "同里湖度假村(二期)", voteCount = 0 });
                mVoteHotelsList.Add(new VoteHotels() { hotelcode = 1010, hotelname = "御庭精品(汤山店)", voteCount = 0 });
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
