using System;
using System.Collections.Generic;
using System.Text;

using AmplaCore.Services;

namespace AmplaCore.Models
{
    /// <summary>
    /// Профиль.
    /// </summary>
    public class Profile
    {
        public int ID { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public DateTime ActivityDate { get; set; }
        public string Name { get; set; }
        public int ProfileTypeID { get; set; }
        public string ProfileType { get; set; }
        public int AccountID { get; set; }
        public string Description { get; set; }
        public string ShortDescription { get; set; }
        public string Image { get; set; }
        public string ImageLogo { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public int InviteCount { get; set; }
    }
}
