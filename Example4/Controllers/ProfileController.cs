using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AmplaApp.FormModels;
using AmplaApp.Services;
using AmplaApp.Utils;
using AmplaApp.ViewModels.Profile;
using AmplaCore.Models;
using AmplaCore.ViewModels;
using AmplaCore.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AmplaApp.ViewModels;
using AmplaCore.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace AmplaApp.Controllers
{
    /// <summary>
    /// Профили на сайте, списки, карточки, добавление, удаление, редактирование.
    /// </summary>
    public class ProfileController : Controller
    {
        private readonly LocalizationService _localizationService;
        private readonly StatusService _statusService;
        private readonly LocalizationRepository _localizationRepository;
        private readonly ILogger<HomeController> _logger;
        private readonly ProfileRepository _profileRepository;
        private readonly ContentRepository _contentRepository;
        private readonly AccountRepository _accountRepository;
        private readonly ViewFieldsRepository _viewFieldsRepository;
        private readonly ProfileService _profileService;
        private readonly EmailService _emailService;
        private readonly CityRepository _cityRepository;
        private readonly FileStorageService _fileStorageService;
        private readonly IHostingEnvironment _environment;

        public ProfileController(ILogger<HomeController> logger,
            LocalizationService localizationService,
            StatusService statusService,
            ProfileRepository profileRepository,
            ContentRepository contentRepository,
            AccountRepository accountRepository,
            FileStorageService fileStorageService,
            IHostingEnvironment environment,
            LocalizationRepository localizationRepository,
            ViewFieldsRepository viewFieldsRepository,
            ProfileService profileService,
            EmailService emailService,
            CityRepository cityRepository

           )
        {
            _logger = logger;
            _localizationService = localizationService;
            _statusService = statusService;
            _profileRepository = profileRepository;
            _contentRepository = contentRepository;
            _accountRepository = accountRepository;
            _fileStorageService = fileStorageService;
            _localizationRepository = localizationRepository;
            _environment = environment;
            _viewFieldsRepository = viewFieldsRepository;
            _profileService = profileService;
            _emailService = emailService;
            _cityRepository = cityRepository;


        }
      

        /// <summary>
        /// Список приглашений профиля.
        /// </summary>
        /// <param name="profileID"></param>
        /// <param name="page"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        [Route("~/profile/invites/{profileID}")]
        public ActionResult Invites(int profileID, int page = 1, int count = 20)
        {
            //TODO проверка авторизация и права доступа.
            ProfileInviteListViewModel vm = new ProfileInviteListViewModel();
            

            vm.ProfileList = _profileRepository.GetInviteProfileList(profileID);//TODO листование
            vm.ProfileID = profileID;
            //TODO vm.ProfileList если нет данных, то 404
            
            return View(vm);
        }
        /// <summary>
        /// Список специальностей
        /// </summary>
        /// <param name="page"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        [Route("~/profile/list/specialty")]
        public ActionResult SpecialtyList(int specialtyID = 0, string name = "", string cities = "", int page = 1, int count = 20)
        {
            SpecialtyListViewModel vm = new SpecialtyListViewModel
            {
                CityList = _cityRepository.GetCityList(),
                Search = name,
                Cities = cities
            };
            //    count = 3;

            var profiles = _profileRepository.GetUniversityListBySpecialty((int)ProfileTypes.university, name, cities, specialtyID, page, count);

            //TODO vm.ProfileList если нет данных, то 404
            int totalCount = _profileRepository.GetUniversityListBySpecialtyTotalCount((int)ProfileTypes.university, name, cities, specialtyID);
            if (totalCount > count)
                vm.Pagination = new PaginationData(_localizationService, LinkBuilder.Profile.UniversityEducationSpecialtyList(), Utils.RequestUtils.GetRequesValues(Request), page, count, totalCount);
            vm.Count = count;


            List<EducationSpecialty> specialtyList = null;
            // По списку универов получить специальности.
            if (profiles != null && profiles.Count > 0)
                specialtyList = _profileRepository.GetEducationSpecialtiesByUniversities(profiles.Select(u => u.ID).ToList());

            //Список популярных специальностей.
            vm.TopSpecialtyList = _profileRepository.GetTopSpecialties(5);

            // Формирование данных.
            vm.PrepareData(profiles, specialtyList);


            // Проверка, какой возвращать результат html или json
            if (CheckAcceptJson())
            {
                // результат json
                return Json(new
                {
                    profileList = vm.ProfileList,
                    pagination = vm.Pagination
                });
            }
            //  результат html
            return View("UniversityEducationSpecialtyList", vm);
        }
        
        
        /// <summary>
        /// API данных по студенту.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="relation"></param>
        /// <param name="relationSelect"></param>
        /// <param name="ctype"></param>
        /// <param name="page"></param>
        /// <param name="count"></param>
        /// <param name="confirmed"></param>
        /// <param name="rejected"></param>
        /// <returns></returns>
        [Route("~/profile/card/student")]
        public ActionResult StudentCard(int id, int page = 1, int count = 20, string token ="")
        {
            
            var profile = _profileRepository.GetStudent(id);
            if (profile == null)
            {
                return NotFound();
            }
            if (!string.IsNullOrEmpty(token))
                _profileService.GetCurrentAccountByToken(token);
            int currentProfileID = 0;
            StudentCardViewModel vm = new StudentCardViewModel();
            if (_profileService.CurrentProfile != null && _profileService.CurrentProfile.ID == profile.ID)
            {
                currentProfileID = _profileService.CurrentProfile.ID;
                vm.IsOwner = true;
                vm.CurrentProfileID = currentProfileID;
            }
            vm.Student = new StudentViewModel(currentProfileID,profile);

            // Данные об образовании.
            vm.StudentEducation =new StudentEducationViewModel(  _profileRepository.GetStudentEducation(id));
            


            // Получить список контента (записей в ленту).
            var contentList = _contentRepository.GetContentList(id,page, count);
            // Получить последние комментарии для записей в ленту.
            List<Comment> contentCommentList = null;
            if (contentList != null && contentList.Count > 0)
                contentCommentList = _contentRepository.GetLastContentCommentList(contentList.Select(u => u.ID).ToList());
            vm.PrepareContentData(contentList, contentCommentList);

            int totalContentCount = _contentRepository.GetContentListTotalCount(id);
            
            vm.Pagination = new PaginationData(_localizationService, LinkBuilder.Profile.Card(id), Utils.RequestUtils.GetRequesValues(Request), page, count, totalContentCount);


            // Возможно вы знакомы.
            vm.PreparePossibleFriends ( _profileRepository.GetPossibleFriends(currentProfileID));
            
            // Работодатели.
            vm.PrepareEmployers ( _profileRepository.GetRandEmployers());
            
            // Список друзей (не более 9)                
            vm.PrepareFriends(_profileRepository.GetFriendProfileList(id, 2, 1, 9));
            
            
            /*
            // Подписчики (не более 9)
            vm.SubscriberList = _profileRepository.GetRelationProfileList(id, "subscribers", 2, 1, 9);

            // Список последних записей всех профилей TODO потом точно будет известно как надо.
            vm.LastContentList = _contentRepository.GetLastContentList();
            */
            return Json(vm);
        }
     
    }
}
