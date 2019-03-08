using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.Models
{
    [DataContract]
    public class CheckUserIDAndPasswordResponse
    {
        [DataMember]
        public bool UserIDAndPasswordCorrect { get; set; }
        [DataMember]

        public bool Success { get; set; }
        [DataMember]
        public string Message { get; set; }
    }
}

