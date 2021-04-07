using System;
using System.Collections.Generic;
using System.Text;

namespace AmplaCore.Models
{
    public class Comment
    {
        public int ID { get; set; }
        public DateTime CreateDate { get; set; }
        public string Text { get; set; }

        public string ObjectType { get; set; }
        public int ObjectID { get; set; }
        public int ProfileID { get; set; }


        public string ProfileImage { get; set; }
        public string ProfileName { get; set; }
    }
}
