using HJD.AccessService.Contract.Model.Dialog;
using HJD.Framework.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJD.AccessServer.DAL
{
    public class MagiCallDAL
    {
        private static IDatabaseManager database = DatabaseManagerFactory.Create("MagiCallDB");         

        public static List<MagiCallDialogItemsEntity> GetDialogItemsByIDList(List<long> itemIDList)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@ItemIDList", string.Join(",", itemIDList));
            return database.ExecuteSqlString<MagiCallDialogItemsEntity>("MagiCallDB.GetDialogItemsByIDList", parameters);
        } 
    }
}
