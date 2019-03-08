using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJD.AccessService.Contract.Model.Search
{
    public class QuestionEntity
    {
        public int ID { get; set; }
        public string question { get; set; }
        public QuestionContext context { get; set; }
    }
}
