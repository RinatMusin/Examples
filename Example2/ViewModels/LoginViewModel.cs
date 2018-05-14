using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CitystarCms.ViewModels
{
    public class LoginViewModel
    {
        public string ErrorMessage;
        [AllowHtml]
        public string Email { get; set; }
        public string Password { get; set; }
    }
}