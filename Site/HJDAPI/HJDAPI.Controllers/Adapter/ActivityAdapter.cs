using HJD.ADServices.Contract;
using HJD.Framework.WCF;
using HJDAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.Controllers.Adapter
{
    public class ActivityAdapter
    {
        static IADService ADService = ServiceProxyFactory.Create<IADService>("IADService");
        public static string GetInvitedCode(ActivityParam p)
        {
            try
            {
                //ToDo
                if (p.activityID == 0)
                {
                    p.activityID = 100;
                }
                List<ActiveUserCodeEntity> list = ADService.GetActiveCode(p.userID, p.activityID);

                return list != null ? list[0].IDX.ToString() : "0";
            }
            catch
            {
                return "0";
            }
        }


        public static ActivePageEntity GetActivePageByIDX(int idx)
        {
            return ADService.GetActivePageByIDX(idx);
        }

        public static ActivePageTemplateEntity GetActivePageTemplateByID(int id)
        {
            return ADService.GetActivePageTemplateByID(id);
        }

    }
}
