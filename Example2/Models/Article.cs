using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CitystarCms.Models
{
    [Table("articles")]
    public class Article
    {
        [Column("title")]
        public string Title { get; set; }
        [Column("short_text")]
        public string ShortText { get; set; }
        [Column("text")]
        public string Text { get; set; }
        [Column("url")]
        public string Url { get; set; }
        [Column("id")]
        public int ID { get; set; }
    }
}