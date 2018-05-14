using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CitystarCms.Models
{
    [Table("pages")]
    public class Page
    {
        [Column("title")]
        public string Title { get; set; }
        [Column("page_type")]
        public string PageType { get; set; }
        [Column("text")]
        public string Text { get; set; }
        [Column("url")]
        public string Url { get; set; }
        [Column("id")]
        public int ID { get; set; }
    }
}