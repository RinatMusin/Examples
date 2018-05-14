using Crm.Helpers;
using Crm.Models;
using Crm.Models.Core;
using Crm.Models.Report;
using Crm.Services;
using Crm.ViewModels;
using Crm.ViewModels.CardHolders;
using Crm.ViewModels.Chat;
using Crm.ViewModels.Core;
using Crm.ViewModels.Files;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Text;
using System.Web;
using System.Web.Mvc;


namespace Crm.Controllers
{
    public class CrmController : BaseApiController
    {

        public string RenderPartialViewToString(string viewName, object model)
        {
            if (string.IsNullOrEmpty(viewName))
            {
                viewName = this.ControllerContext.RouteData.GetRequiredString("action");
            }

            this.ViewData.Model = model;

            using (var sw = new StringWriter())
            {
                // Find the partial view by its name and the current controller context.
                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(this.ControllerContext, viewName);

                // Create a view context.
                var viewContext = new ViewContext(this.ControllerContext, viewResult.View, this.ViewData, this.TempData, sw);

                // Render the view using the StringWriter object.
                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }
        }


        [HttpPost]
        public ContentResult Remind(RemindViewModel form)
        {
            if (string.IsNullOrEmpty(form.Login))
            {
                //   Response.StatusCode = (int)HttpStatusCode.BadRequest;

                return JsonErrorMessage("E-mail не указан");
            }

            var user = userService.GetByEmail(form.Login);
            if (user == null)
            {
                //     Response.StatusCode = (int)HttpStatusCode.BadRequest;

                return JsonErrorMessage("E-mail не найден");
            }
            ///Отправка письма
            var res = EmailService.SendRemind(form.Login);
            if (!res)
            {
                //  Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                return JsonErrorMessage("Не удалось отправить данные на указанный E-mail");
            }
            return JsonSuccessMessage("Ваш пароль был выслан на указанный E-mail");
        }



        [HttpPost]
        public ContentResult Login(LoginViewModel form)
        {

            UserData user = null;

            //  if (form.Type == "email")сейчас через почту только
            if (!string.IsNullOrEmpty(form.Login) && !string.IsNullOrEmpty(form.Password))
            {
                user = userService.GetByEmail(form.Login);

            }

            if (user == null || user.Password != form.Password)
            {
                //Response.StatusCode = (int)HttpStatusCode.BadRequest;

                return JsonErrorMessage("Неправильный логин или идентификатор пользователя");
            }
            else
            {
                //System.Web.Security.FormsAuthentication.SetAuthCookie(user.ID.ToString(), false);

                //проверка, если token пустой
                if (string.IsNullOrEmpty(user.Token))
                {
                    user.Token = Guid.NewGuid().ToString();
                    userService.SaveShanges();
                }


                int status = 0;
                var party = partyService.GetByUser(user.ID, G.CurrentProjectID);
                /*  потом вернуться  if (party == null)
                    {
                        var r = ProjectAccessRequest.GetByProjectIDAndUserID(G.CurrentProjectID, user.id);
                        if (r != null)
                        {
                            status = r.Status;
                        }
                    }
                    else
                    {
                        status = (int)ProjectAccessRequestStatus.Yes;
                    }*/

                return JsonResult(new
                {
                    status = "success",
                    user = UserDataViewModel.GetMap(user),
                    Token = user.Token
                });
            }
        }
        public ContentResult Logout()
        {
            System.Web.Security.FormsAuthentication.SignOut();
            return JsonResult(new { });
        }
        //
        // GET: /Crm/


        public ContentResult GetCurrentUser(string token)
        {
            var user = userService.GetByToken(token);

            return JsonResult(new
            {
                user = UserDataViewModel.GetMap(user, true),
                //                IsAuthenticated = User.Identity.IsAuthenticated

            });
        }
        //список активных пользователей
        public ContentResult GetActivePeoples(string token)
        {
            var user = userService.GetByToken(token);

            return JsonResult(new
            {
                parties = partyService.GetActivePeoples(14)
                //                IsAuthenticated = User.Identity.IsAuthenticated

            });
        }

        public ContentResult GetNotes(int id, string objectType, int limit = 20, int offset = 0, string token = "")
        {
            DateTime startDate = DateTime.Now;
            var user = userService.GetByToken(token);
            if (string.IsNullOrEmpty(objectType))
                return JsonMessage("Не указан тип объекта");
            if (id <= 0)
                return JsonMessage("Не указан ID объекта");
            DateTime endDate1 = new DateTime(DateTime.Now.Ticks - startDate.Ticks);
            var notes = noteService.GetByObject(objectType, id, user != null ? user.ID : 0, limit, offset);
            var vm = new List<NoteViewModel>();
            //сформировать список заметок
            //List<int> idList=new List<int>();
            foreach (var note in notes)
            {

                vm.Add(NoteViewModel.GetMap(note));
            }


            //long ticks = msg.StopDateTime.Ticks - msg.StartDateTime.Ticks;
            DateTime endDate = new DateTime(DateTime.Now.Ticks - startDate.Ticks);

            //отметить как просмотренные
            dbService.UpdateObjectLastVisit(user.ActiveProfileID, notes.Select(s => s.ID).ToList(), "note");

            SocketService socketService = new SocketService();
            socketService.SendShowObject(G.CurrentProjectID, user.ID, user.ActiveProfileID, id, objectType);

            return JsonResult(new
            {
                notes = vm,
                endDate1 = endDate1.Ticks / TimeSpan.TicksPerMillisecond,
                endDate = endDate.Ticks / TimeSpan.TicksPerMillisecond
                //long ticks = msg.StopDateTime.Ticks - msg.StartDateTime.Ticks;


            });
        }
        [HttpPost]
        public ContentResult AddNote(NoteFormModel form, string token)
        {
            // Response.AddHeader("Access-Control-Allow-Origin", "*");
            var user = userService.GetByToken(token);
            if (user == null || user.ID == 0)
                return JsonMessage("Ошибка! Пользователь не авторизован!");

            //получение параметров
            if (string.IsNullOrEmpty(form.Text))
                return JsonMessage("Не задан текст заметки");

            if (string.IsNullOrEmpty(form.ObjectType))
                return JsonMessage("Не задан тип объекта для заметки");
            if (form.ObjectID <= 0)
                return JsonMessage("Не задан идентификатор объекта для заметки");

            var party = partyService.GetByUser(user.ID, G.CurrentProjectID);
            //сформировать заметку
            //по идее надо проверять что заметка создается на существующий объект (это на будущее проверку)
            Note note = new Note
            {
                IsPublic = form.IsPublic,
                ObjectID = form.ObjectID,
                ObjectType = form.ObjectType,
                Text = form.Text,
                UserID = user.ID,
                PartyID = party != null ? party.ID : 0,
                ProjectID = G.CurrentProjectID
            };

            //2018 временно эксперимент
            List<DataPropertyValue> props = new List<DataPropertyValue>();
            props.Add(new DataPropertyValue
            {
                Key = "1",
                Value = "ds"
            });
            props.Add(new DataPropertyValue
            {
                Key = "werret1",
                Value = "daawrweews"
            });
            props.Add(new DataPropertyValue
            {
                Key = "1dfgdg",
                Value = "dsgfdg"
            });
            note.DataProperty = Newtonsoft.Json.JsonConvert.SerializeObject(props, new Newtonsoft.Json.JsonSerializerSettings { ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver() });
            noteService.AddNote(note);
            //      note = noteService.GetByID(note.ID);
            var vm = NoteViewModel.GetMap(note);

            //сохранить в иерархию объектов
            dbService.AddObjectHierarchy(new ObjectHierarchy { ObjectType = "note", ObjectID = note.ID, ProjectID = G.CurrentProjectID, ParentObjectID = form.ObjectID, ParentObjectType = form.ObjectType, IsCompleteEdit = 1, IsPublic = form.IsPublic });
            //отметить что это заметку прочитал
            dbService.AddObjectLastVisit(user.ActiveProfileID, note.ID, "note");

            //отправить уведомление клиентам об изменении данных
            SocketService socketService = new SocketService();
            var totalCountData = (sqlDataService.GetObjectStatistic(G.CurrentProjectID, note.ObjectID, note.ObjectType));
            var newCountData = (sqlDataService.GetNewObjectStatistic(G.CurrentProjectID, note.ObjectID, note.ObjectType, user.ActiveProfileID));  //профиль=1

            socketService.SendAddNewObject(G.CurrentProjectID, note.ObjectID, note.ObjectType, "note", totalCountData, newCountData);

            return JsonResult(new
            {
                message = "Заметка сохранена!",
                note = vm
            });

        }


    }
}
