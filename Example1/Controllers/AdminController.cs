using BookMedica.Models;
using BookMedica.Repositories;
using BookMedica.ViewModels.Admin;
using CityStar.Meta.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace BookMedica.Controllers
{
    public class AdminController : Controller
    {
        Global G = Global.GetGlobal();
        /// <summary>
        /// Инициализация меню
        /// </summary>
        private void initMenu(AdminMenuType menuType)
        {
            var vm = new AdminLeftMenuViewModel();
            //колв-во неотмодерированных отзывов
            vm.NotModerationOpinionCount = AdminRepository.GetNotModerationOpinionCount();
            //колв-во неотмодерированных врачей
            vm.NotModerationDoctorCount = AdminRepository.GetNotModerationDoctorCount();
            //колв-во неотмодерированных клиник
            vm.NotModerationClinicCount = AdminRepository.GetNotModerationClinicCount();
            //кол-во неотмодерированных специализаций врача
            vm.NotModerationSpecializationCount = AdminRepository.GetNotModerationSpecializationCount();
            vm.MenuType = menuType;
            ViewBag.LeftMenuViewModel = vm;
        }

        private void addLog(string text)
        {
            CityStar.Core.Logger.Log.Info(text);
        }

        /// <summary>
        /// Проверка на права
        /// </summary>
        private bool checkRight()
        {
            //addLog("UserIsAuthenticated " + G.UserIsAuthenticated);
            var user = CityStar.Core.CurrentUser.GetInstance();
            //addLog(user.IsAuthenticated.ToString());
            //TODO сделать
            //если авторизован, но не модератор  -TODO ПРАВА
            if (user != null && user.IsAuthenticated)
            {
                //получить данные из БАЗЫ
                if (user.CurrentUserData.RoleID == (int)UserRoleType.Admin || user.CurrentUserData.RoleID == (int)UserRoleType.SuperAdmin)

                    return true;
                else
                    return false;

            }
            return false;

        }
        public ActionResult Logout()
        {
            //if (User.Identity.IsAuthenticated)
            {
                FormsAuthentication.SignOut();

            }
            return Redirect(AdminLinkBuilder.Home());
        }
        public ActionResult Login()
        {

            if (Request.HttpMethod == "POST")
            {
                var username = RequestHelper.GetRequestFormText(Request, "username");
                var password = RequestHelper.GetRequestFormText(Request, "password");
                string phone = BookMedica.Models.Utils.ClearPhone(username);

                var userData = CityStar.Core.UserData.GetByPhone(phone);
                if (userData != null && userData.Password == password)
                {
                    FormsAuthentication.SetAuthCookie(userData.id.ToString(), false, "/");
                    return Redirect(AdminLinkBuilder.Home());
                }
                else
                {
                    //result.Status = "error";
                }

            }
            return View("AdminLoginPage");
        }
        //
        // GET: /Admin/

        public ActionResult Index()
        {
            if (!checkRight())
                return Login();
            initMenu(AdminMenuType.None);


            return View("AdminPage");
        }
        /// <summary>
        /// Страница модерации отзывов
        /// </summary>
        /// <returns></returns>
        public ActionResult OpinionModeration()
        {

            if (!checkRight())
                return Login();
            initMenu(AdminMenuType.ModerationOpinion);


            //addLog("UserIsAuthenticated " + G.UserIsAuthenticated);
            var user = CityStar.Core.CurrentUser.GetInstance();
            //addLog(user.IsAuthenticated.ToString());


            int moderatorID = user.CurrentUserData.id;
            if (Request.HttpMethod == "POST")
            {

                //получить данные,
                int opinionID = RequestHelper.GetRequestFormInt(Request, "opinionid");
                int moderation = RequestHelper.GetRequestFormInt(Request, "moderation");
                var opinion = FormRepository.GetDoctorOpinion(opinionID);
                if (opinion == null)
                {
                    //ошибка

                }
                addLog("opinionID " + opinionID + " moderation " + moderation);
                //сохранить данные
                AdminRepository.UpdateOpinionModeration(opinionID, moderation, moderatorID);
                //пересчет статистики по рекомендациям
                AdminRepository.UpdateDoctorRecomendation(opinion.OnID);
                //пересчет рейтинга по врачу и клинике
                FormRepository.UpdateDoctorRating(opinion.OnID);

                //отправить смс автору отзывы, если приняли отзыв
                if (moderation == 2)
                {
                    string phone = UserRepository.GetUserPhone(opinion.UserID);
                    Utils.SendSMS(opinion.UserID, phone, "Ваш отзыв о враче успешно прошел модерацию и опубликован на BookMedica.ru. Спасибо, что вы помогли сделать мир лучше!", true);
                }
                //редирект на модерацию
                return Redirect(AdminLinkBuilder.OpinionModeration());


            }



            var vm = new AdminOpinionModerationViewModel
            {
                //получить список 5 неотмодерированных отзывов
                Opinion = AdminRepository.GetNotModerationOpinion(),
                //получить данные по формированию отзыва
                CriterionList = FormRepository.GetDoctorCriterionList()
            };

            //данные по врачу
            if (vm.Opinion != null && vm.Opinion.ID > 0)
            {
                vm.Doctor = FormRepository.GetDoctor(vm.Opinion.OnID);
                //надо проверить что клиника в отзыве есть у врача
                if (!FormRepository.CheckDoctorClinic(vm.Opinion.OnID, vm.Opinion.ClinicID))
                {
                    //клиники такой нет, 
                    vm.OthenClinic = FormRepository.GetClinic(vm.Opinion.ClinicID);
                }
            }


            return View("AdminOpinionModerationPage", vm);
        }

        /// <summary>
        /// Страница модерации клиник
        /// </summary>
        /// <returns></returns>
        public ActionResult ClinicModeration()
        {

            if (!checkRight())
                return Login();
            initMenu(AdminMenuType.ModerationClinic);


            //addLog("UserIsAuthenticated " + G.UserIsAuthenticated);
            var user = CityStar.Core.CurrentUser.GetInstance();
            //addLog(user.IsAuthenticated.ToString());


            int moderatorID = user.CurrentUserData.id;
            if (Request.HttpMethod == "POST")
            {

                //получить данные,
                int clinicID = RequestHelper.GetRequestFormInt(Request, "clinicid");
                int moderation = RequestHelper.GetRequestFormInt(Request, "moderation");
                int skip = RequestHelper.GetRequestFormInt(Request, "skip");
                var clinic = FormRepository.GetClinic(clinicID);
                if (clinic == null)
                {
                    //ошибка

                }
                addLog("clinicID " + clinicID + " moderation " + moderation + " skip " + skip);
                if (skip > 0)
                {
                    //пометить флагом пропуск модерации
                    AdminRepository.SkipClinicModeration(clinic.ID, skip);
                }
                else
                {
                    //сохранить данные
                    AdminRepository.UpdateClinicModeration(clinic.ID, moderation, moderatorID);
                    //пересчет статистики по врачам клиник
                    //AdminRepository.UpdateClinicDoctorCount(clinic.ID);
                    //пересчет рейтинга по клинике
                    //   AdminRepository.UpdateClinicRating(clinic.ID);
                    // Обновление статистики по городу
                    FormRepository.UpdateCityStatistic();

                    //отправить смс автору отзывы, если приняли отзыв
                    /*   if (moderation == 2)
                       {
                           string phone = UserRepository.GetUserPhone(opinion.UserID);
                           Utils.SendSMS(opinion.UserID, phone, "Ваш отзыв о враче успешно прошел модерацию и опубликован на BookMedica.ru. Спасибо, что вы помогли сделать мир лучше!", true);
                       }*/
                }
                //редирект на модерацию
                return Redirect(AdminLinkBuilder.ClinicModeration());


            }



            var vm = new AdminClinicModerationViewModel
            {
                //получить неотмодерированную клинику
                Clinic = AdminRepository.GetNotModerationClinic(),
            };
            if (vm.Clinic != null && vm.Clinic.ID > 0)
            {
                //фотоальбом
                vm.PhotoList = FormRepository.GetClinicPhotos(vm.Clinic.ID);
                //похожие клиники по названию и адресу
                vm.SimilarClinicList = AdminRepository.GetSimilarClinicList(vm.Clinic);
                vm.CityName = FormRepository.GetCity(vm.Clinic.CityID).CityName;
            }

            return View("AdminClinicModerationPage", vm);
        }

        public ActionResult DoctorModeration()
        {

            if (!checkRight())
                return Login();
            initMenu(AdminMenuType.ModerationDoctor);


            //addLog("UserIsAuthenticated " + G.UserIsAuthenticated);
            var user = CityStar.Core.CurrentUser.GetInstance();
            //addLog(user.IsAuthenticated.ToString());


            int moderatorID = user.CurrentUserData.id;
            if (Request.HttpMethod == "POST")
            {

                //получить данные,
                int doctorID = RequestHelper.GetRequestFormInt(Request, "doctorid");
                int moderation = RequestHelper.GetRequestFormInt(Request, "moderation");
                int skip = RequestHelper.GetRequestFormInt(Request, "skip");
                int joinDoctor = RequestHelper.GetRequestFormInt(Request, "join-doctor");

                var doctor = FormRepository.GetDoctor(doctorID);
                if (doctor == null)
                {
                    //ошибка

                }
                if (joinDoctor > 0)
                {
                    //объединение с другим врачом
                    //у врача ставится статус модерации = 5 объединен, отзывы переносятся на другого врача
                    //сохранить данные
                    AdminRepository.UpdateDoctorModeration(doctor.ID, Values.MODERATION_STATUS_JOIN, moderatorID);
                    //перенос отзывов
                    AdminRepository.TransferOpinions(joinDoctor, doctor.ID);

                    //пересчет кол-ва отзывов по врачу и его клиникам
                    FormRepository.UpdateDoctorRating(joinDoctor);


                }
                else
                {
                    if (skip > 0)
                    {
                        //пометить флагом пропуск модерации
                        AdminRepository.SkipDoctorModeration(doctor.ID, skip);
                    }
                    else
                    {
                        //  addLog("opinionID " + opinionID + " moderation " + moderation);
                        //сохранить данные
                        AdminRepository.UpdateDoctorModeration(doctor.ID, moderation, moderatorID);
                        //пересчет статистики по врачам клиник
                        AdminRepository.UpdateClinicDoctorCount(doctor.ClinicList);
                        //пересчет рейтинга по клинике
                        //   AdminRepository.UpdateClinicRating(clinic.ID);
                        // Обновление статистики по городу
                        FormRepository.UpdateCityStatistic();

                        //отправить смс автору отзывы, если приняли отзыв
                        /*   if (moderation == 2)
                           {
                               string phone = UserRepository.GetUserPhone(opinion.UserID);
                               Utils.SendSMS(opinion.UserID, phone, "Ваш отзыв о враче успешно прошел модерацию и опубликован на BookMedica.ru. Спасибо, что вы помогли сделать мир лучше!", true);
                           }*/
                    }
                }
                //редирект на модерацию
                return Redirect(AdminLinkBuilder.DoctorModeration());


            }



            var vm = new AdminDoctorModerationViewModel
            {
                //получить неотмодерированного врача
                Doctor = AdminRepository.GetNotModerationDoctor(),
            };

            if (vm.Doctor != null && vm.Doctor.ID > 0)
            {
                //сертификаты
                vm.CertificateList = FormRepository.GetDoctorCertificateList(vm.Doctor.ID);
                //похожие по фамилии врачи
                vm.SimilarDoctorList = AdminRepository.GetSimilarDoctorList(vm.Doctor);
                //колво отзывов у этого врача
                vm.OpinionCount = FormRepository.GetDoctorAllOpinionCount(vm.Doctor.ID);
                vm.CityName = FormRepository.GetCity(vm.Doctor.CityID).CityName;
            }


            return View("AdminDoctorModerationPage", vm);
        }

        /// <summary>
        /// Модерация специализаций
        /// </summary>
        /// <returns></returns>
        public ActionResult SpecializationModeration()
        {
            if (!checkRight())
                return Login();
            initMenu(AdminMenuType.ModerationSpecialization);


            //addLog("UserIsAuthenticated " + G.UserIsAuthenticated);
            var user = CityStar.Core.CurrentUser.GetInstance();
            //addLog(user.IsAuthenticated.ToString());


            int moderatorID = user.CurrentUserData.id;
            if (Request.HttpMethod == "POST")
            {

                //получить данные,
                int specializationID = RequestHelper.GetRequestFormInt(Request, "specializationid");
                int moderation = RequestHelper.GetRequestFormInt(Request, "moderation");


                var specialization = Valueset.GetDoctorSpecialization(specializationID);
                if (specialization == null)
                {
                    //ошибка
                }
                else
                {
                    //сохранить данные
                    AdminRepository.UpdateSpecializationModeration(specialization.ID, moderation, moderatorID);

                    //   AdminRepository.UpdateClinicRating(clinic.ID);
                    // Обновление статистики по специализациям в клинике
                    //TODO

                }

                //редирект на модерацию
                return Redirect(AdminLinkBuilder.SpecializationModeration());


            }



            var vm = new AdminSpecializationModerationViewModel
            {
                //получить неотмодерированные данные по специализации
                Specialization = Valueset.GetNotModerationDoctorSpecialization()
            };




            return View("AdminSpecializationModerationPage", vm);
        }

        #region ГОРОДА
        /// <summary>
        /// Список городов
        /// </summary>
        /// <returns></returns>
        public ActionResult CityList()
        {
            if (!checkRight())
                return Login();
            initMenu(AdminMenuType.CitySettings);


            //addLog("UserIsAuthenticated " + G.UserIsAuthenticated);
            // var user = CityStar.Core.CurrentUser.GetInstance();
            //addLog(user.IsAuthenticated.ToString());

            var vm = new AdminCityListViewModel();
            vm.Items = FormRepository.GetCityList();
            return View("AdminCityListPage", vm);
        }

        public ActionResult CityEdit(int id = 0)
        {
            if (!checkRight())
                return Login();
            initMenu(AdminMenuType.CitySettings);

            bool error = false;
            if (Request.HttpMethod == "POST")
            {
                string cityName = RequestHelper.GetRequestFormText(Request, "cityname");
                if (string.IsNullOrEmpty(cityName))
                {
                    //ошибка
                    error = true;
                }
                else
                {
                    if (id == 0)
                        FormRepository.AddCity(cityName);
                    else
                        FormRepository.UpdateCity(id, cityName);
                    return Redirect(AdminLinkBuilder.CityList());
                }
            }

            var vm = new AdminCityEditViewModel();
            vm.Error = error;
            vm.City = FormRepository.GetCity(id);
            vm.CityName = "";
            if (vm.City == null || vm.City.ID == 0)
                vm.IsNew = true;
            else
                if (!error)
                    vm.CityName = vm.City.CityName;
            return View("AdminCityEditPage", vm);
        }
        #endregion


        public ActionResult SearchOpinion(int page = 1)
        {
            if (!checkRight())
                return Login();
            initMenu(AdminMenuType.SearchOpinion);
            string searchText = "";

            if (Request.HttpMethod == "POST")
            {
                searchText = RequestHelper.GetRequestFormText(Request, "search-text");
            }
            else
            {
                searchText = RequestHelper.GetRequestText(Request, "search");
            }

            var vm = new AdminOpinionListViewModel();
            vm.Page = page;

            vm.ItemCount = AdminRepository.GetOpinionListCount(searchText);
            vm.TotalPage = vm.ItemCount / Values.OPINION_PAGE_SIZE;
            if (vm.TotalPage * Values.OPINION_PAGE_SIZE < vm.ItemCount)
                vm.TotalPage++;

            if (vm.Page > vm.TotalPage)
                vm.Page = 1;


            vm.Items = AdminRepository.GetOpinionList(vm.Page, searchText);
            vm.SearchText = searchText;
            if (!string.IsNullOrEmpty(searchText))
            {
                foreach (var item in vm.Items)
                {
                    item.LikeText = Utils.SelectSearchText(item.LikeText, searchText);
                    item.DislikeText = Utils.SelectSearchText(item.DislikeText, searchText);
                }
            }
            return View("AdminSearchOpinionPage", vm);
        }
    }
}
