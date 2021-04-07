using System;
using System.Collections.Generic;

using System.Text;

namespace AmplaCore.Models
{
    public class Student : Profile
    {
        public int UniversityID { get; set; }


        public string UniversityName { get; set; }
        public string UniversityImage { get; set; }
        public string UniversityImageLogo { get; set; }
        public string UniversityCityName { get; set; }

        public int FriendCount { get; set; }
        public int FriendInviteStatus { get; set; }
    }
}
