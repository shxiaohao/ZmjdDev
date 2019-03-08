using HJD.AccessService.Contract.Model;
using HJD.Framework.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJD.AccessServer.DAL
{
    public class DestDBDAL
    {
        private static IDatabaseManager database = DatabaseManagerFactory.Create("DestDB");

        public static List<DistrictInfoAroundEntity> GetAroundDistrict(int DistrictID, int Distance)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@DistrictID", DistrictID);
            parameters.Add("@Distance", Distance);
            return database.ExecuteSproc<DistrictInfoAroundEntity>("sp4_DistrictInfo_Around", parameters);
        }
    }
}
