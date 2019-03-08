using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.Models
{
    public class GroupSerialItem
    {
        public GroupSerialItem() {
            SerialNo = "";
            SerialPackageList = new List<SameSerialPackageItem>();
        }

        public string SerialNo { get; set; }

        public List<SameSerialPackageItem> SerialPackageList { get; set; } 
    }
}
