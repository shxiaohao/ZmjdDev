using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.Models.Tags
{ 
        [System.Runtime.Serialization.DataContractAttribute] 
        public class CommTagEntity
        { 

            [System.Runtime.Serialization.DataMemberAttribute] 
            public int ID { get; set; }
            [System.Runtime.Serialization.DataMemberAttribute] 
            public string Name { get; set; }
            [System.Runtime.Serialization.DataMemberAttribute] 
            public string Description { get; set; }
        }
    }
