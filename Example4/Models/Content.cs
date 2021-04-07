using System;
using System.Collections.Generic;
using System.Text;

namespace AmplaCore.Models
{
    public class Content
    {
        public int ID { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public DateTime PublishDate { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public int ProfileID { get; set; }
        public string Image { get; set; }

        public string ProfileImage { get; set; }
        public string ProfileName { get; set; }
        public int CommentCount { get; set; }
    }
}
