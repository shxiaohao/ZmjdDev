using HJD.HotelServices.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.Controllers.Adapter
{
    public  class ChannelAdapter
    {
        private static Dictionary<long, ChannelInfoEntity> dicChannelInfo = new Dictionary<long, ChannelInfoEntity>();
        private static Dictionary<string, ChannelInfoEntity> dicInvitationChannelInfo = new Dictionary<string, ChannelInfoEntity>();

        public static bool RemoveChannelInfoCache(long CID)
        {
            if (dicChannelInfo.ContainsKey(CID))
            {
                dicChannelInfo.Remove(CID);
                return true;
            }
            else
            {
                return false;
            }
        }

        public static ChannelInfoEntity GetChannelInfo(long CID)
        {
            if (dicChannelInfo.Count == 0)
            {
                try
                {
                    List<ChannelInfoEntity> list = HotelAdapter.GetAllChannelInfoList();
                    list.ForEach(_ =>
                    {
                        if (!dicChannelInfo.ContainsKey(_.IDX))
                        {
                            dicChannelInfo.Add(_.IDX, _);
                        }
                    });
                }
                catch (Exception e)
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

        public static ChannelInfoEntity GetChannelInfoByInvitationCode(string invitationCode)
        {
            if (dicInvitationChannelInfo.Count == 0)
            {
                try
                {
                    List<ChannelInfoEntity> list = HotelAdapter.GetAllChannelInfoList();
                    list.ForEach(_ =>
                    {
                        if (!dicChannelInfo.ContainsKey(_.IDX))
                        {
                            dicChannelInfo.Add(_.IDX, _);
                        }
                        if(_.InviteCode != null && _.InviteCode.Length > 0)
                        {
                            if( !dicInvitationChannelInfo.ContainsKey(_.InviteCode.ToUpper()))
                            {
                                dicInvitationChannelInfo.Add(_.InviteCode.ToUpper(), _);
                            }
                        }
                    });
                }
                catch (Exception e)
                {
                    string msg = e.Message;
                }
            }

            if (!dicInvitationChannelInfo.ContainsKey(invitationCode.ToUpper()))
            {
                return new ChannelInfoEntity();
            }
            else
            {
                return dicInvitationChannelInfo[invitationCode.ToUpper()];
            }
             
        }
    }
}
