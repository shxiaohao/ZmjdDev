using HJD.HotelServices.Contracts;
using HJDAPI.APIProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WHotelSite.Common
{
    public class HotelHelper
    {
        private static Dictionary<long, ChannelInfoEntity> dicChannelInfo = new Dictionary<long , ChannelInfoEntity>();

        public static  ChannelInfoEntity GetChannelInfo(long  CID)
        {
            if (dicChannelInfo.Count == 0)
            {
                try
                {
                    List<ChannelInfoEntity> list = Hotel.GetAllChannelInfoList();
                    list.ForEach(_ =>
                    {
                        if (!dicChannelInfo.ContainsKey(_.IDX))
                        {
                            dicChannelInfo.Add(_.IDX, _);
                        }
                    });
                }
                catch( Exception e)
                {
                    string msg = e.Message;
                }
            }

            if (!dicChannelInfo.ContainsKey(CID))
            {
                dicChannelInfo.Add(CID, new ChannelInfoEntity { IDX = CID, NeedVerify = false, CanBuyFisrtVIPPackage = true, Code = CID.ToString(), Name = CID.ToString() });
            }

            return dicChannelInfo[CID];
        }
    }
}