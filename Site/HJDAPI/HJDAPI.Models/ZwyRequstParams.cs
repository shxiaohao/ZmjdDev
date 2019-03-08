using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace HJDAPI.Models
{
    public class ZwyRequstParams
    {



            public requestHeadZ requestHead { get; set; }

            public requestBodyZ requestBody { get; set; }


    }


    public class requestHeadZ {



    }


    public class requestBodyZ 
    {

        public string apikey { get; set; }

        public string cust_id { get; set; }

        public string order_id { get; set; }


        public string ticket_num { get; set; }

        public string order_money { get; set; }


    }

}
