using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJD.HotelServices.Contracts
{
    public class ChaoZhiPriceSection
    {
        public double Max { get; set; }
        public double Min { get; set; }
        public Dictionary<int, List<ValuedHotelEntity>> Dic { get; set; }

        public ChaoZhiPriceSection(double min, double max)
        {
            this.Max = max;
            this.Min = min;
            this.Dic = new Dictionary<int, List<ValuedHotelEntity>>();
        }

        public ChaoZhiPriceSection()
        {
        }
    }
}
