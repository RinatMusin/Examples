using Crm.Models;
using Crm.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Crm.Controllers
{
    [InitSPASession]
    public class BaseApiController : Controller
    {
        protected Global G = Global.GetInstance();
        protected ProjectService projectService = new ProjectService();
        protected UserService userService = new UserService();
        protected PartyService partyService = new PartyService();
        protected SqlDataService sqlDataService = new SqlDataService();
        protected NoteService noteService = new NoteService();
        protected FileService fileService = new FileService();
        protected ChatService chatService = new ChatService();
        protected DbService dbService = new DbService();
        //
        // GET: /Base/

        protected ContentResult JsonResult(object obj)
        {

            return new ContentResult
            {
                ContentType = "application/json",
                Content = JsonConvert.SerializeObject(obj, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }),
                ContentEncoding = System.Text.Encoding.UTF8,

            };
        }
        protected ContentResult JsonMessage(string mes)
        {
            return JsonResult(new
            {
                message = mes
            });
        }

        protected ContentResult JsonErrorMessage(string mes)
        {
            return JsonResult(new
            {
                status = "error",
                message = mes
            });
        }
        protected ContentResult JsonSuccessMessage(string mes)
        {
            return JsonResult(new
            {
                status = "success",
                message = mes
            });
        }
    }
}
