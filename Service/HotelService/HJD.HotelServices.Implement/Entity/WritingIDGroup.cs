using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJD.HotelServices.Implement.Entity
{
    public class WritingIDGroup
    {
        public int writing { get; set; }
        public int type { get; set; }

        public override string ToString()
        {
            return string.Format("{0}:{1}",writing,type);
        }
    }
}
