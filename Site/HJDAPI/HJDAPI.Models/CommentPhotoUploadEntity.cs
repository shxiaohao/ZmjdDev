using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.Models
{
    public class CommentPhotoUploadEntity
    {
        public int AppID { get; set; }
        public int CommentID { get; set; }
        public string PhotoData { get; set; }
        public int TagBlockCategory { get; set; }
    }
}
