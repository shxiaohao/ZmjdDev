using HJD.AccessServer.DAL;
using HJD.AccessService.Contract.Model.Dialog; 
using HJD.Framework.Interface;
using HJD.Search.CommonLibrary.Helper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace HJD.Search.CommonLibrary.Engine
{
    public class MagiCallEngine
    { 
        public static List<MagiCallDialogItemsEntity> GetDialogItemsByIDList(List<long> itemIDList)
        {
            return MagiCallDAL.GetDialogItemsByIDList(itemIDList);
        }         

    }
}
