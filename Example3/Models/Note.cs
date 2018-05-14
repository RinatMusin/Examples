using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Crm.Models
{
    [Table("notes")]
    /// <summary>
    /// Заметка
    /// </summary>
    public class Note
    {
        [Column("data_property")]
        public string DataProperty { get; set; }
        [Column("id")]
        public int ID { get; set; }
        [Column("create_date")]
        public DateTime CreateDate { get; set; }
        [Column("update_date")]
        public DateTime UpdateDate { get; set; }
        [Column("user_id")]
        public int UserID { get; set; }
        [Column("party_id")]
        public int PartyID { get; set; }
        [Column("text")]
        public string Text { get; set; }
        [Column("object_type")]
        public string ObjectType { get; set; }
        [Column("object_id")]
        public int ObjectID { get; set; }
        [Column("is_public")]
        public int IsPublic { get; set; }
        [Column("project_id")]
        public int ProjectID { get; set; }

        //[ForeignKey("User")]
        public UserData User { get; set; }
    }
}