using HJD.AccountServices.Contracts;
using HJD.AccountServices.Entity;
using HJD.Framework.WCF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.Controllers.Adapter
{
    public class PrivilegeAdapter
    {
        public static IPrivilegeManager PrivilegeService = ServiceProxyFactory.Create<IPrivilegeManager>("IPrivilegeManager");

        public static  List<UserPrivilegeRel> GetAllPrivilegeByUserId(long userid)
        {
            return PrivilegeService.GetAllPrivilegeByUserId(userid);
        }

        public static void RemoveUserPrivilegeCache(long userid)
        {
            PrivilegeService.RemoveUserPrivilegeCache(userid);
        }
    }
}