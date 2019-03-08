namespace HJDAPI.Common.Helpers
{
    public static class GeoHelper
    {
        /// <summary>
        /// 距离转换成经纬度偏移,1米约等于0.000007
        /// </summary>
        /// <param name="distance">距离，单位米</param>
        /// <returns></returns>
        public static decimal DistanceToOffset(int distance)
        {
            const decimal d = 0.000007m;
            return d * distance;
        }
    }
}