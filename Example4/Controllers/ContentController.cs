using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AmplaApp.Services;
using AmplaApp.Utils;
using AmplaApp.ViewModels.Content;
using AmplaCore.Models;
using AmplaCore.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using AmplaCore.Services;
using AmplaApp.ViewModels.Profile;
using AmplaCore.ViewModels;

namespace AmplaApp.Controllers
{
    public class ContentController : Controller
    {
        private readonly LocalizationService _localizationService;

        private readonly ProfileRepository _profileRepository;
        private readonly ContentRepository _contentRepository;
        private readonly AccountRepository _accountRepository;
        private readonly FileStorageService _fileStorageService;
        private readonly IHostingEnvironment _environment;
        private readonly ProfileService _profileService;

        public ContentController(LocalizationService localizationService,
            ContentRepository contentRepository,
            AccountRepository accountRepository,
            ProfileRepository profileRepository,
            FileStorageService fileStorageService,
            IHostingEnvironment environment,
            ProfileService profileService
           )
        {
            _localizationService = localizationService;
            _contentRepository = contentRepository;
            _accountRepository = accountRepository;
            _profileRepository = profileRepository;
            _fileStorageService = fileStorageService;
            _environment = environment;
            _profileService = profileService;
        }
        [HttpPost]
        public JsonResult Add()
        {
            ContentAddViewModel vm = new ContentAddViewModel();
            vm.Status = "error";
            // vm.WebRootPath = _environment.WebRootPath;
            string title = RequestUtils.GetFormText(Request, "title");
            string text = RequestUtils.GetFormText(Request, "text");
            string token = RequestUtils.GetFormText(Request, "token");

            // Валидация.
            if (string.IsNullOrEmpty(title))
                vm.ErrorTitle = _localizationService.GetValue("Input post title");
            if (string.IsNullOrEmpty(text))
                vm.ErrorText = _localizationService.GetValue("Input post text");
            if (!string.IsNullOrEmpty(token))
            {
                _profileService.GetCurrentAccountByToken(token);
            }
            /* var account = _accountRepository.GetAccountByEmail(User.Identity.Name);
             if (account == null)
             {
                 //return 404;
                 vm.Result = "account == null";
             }*/
            if (string.IsNullOrEmpty(vm.ErrorTitle) && string.IsNullOrEmpty(vm.ErrorText) && _profileService.CurrentProfile != null)
            {
                var content = new Content
                {
                    ProfileID = _profileService.CurrentProfile.ID,// Указываем профиль из запроса
                    Title = title,
                    Text = text,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                    PublishDate = DateTime.Now
                };
                content.ID = _contentRepository.AddContent(content);

                vm.Status = "success";
                vm.Title = content.Title;
                vm.Text = content.Text;
                // Формирование даты для вывода.
                vm.Date = content.CreateDate.ToString("dd MMMM yyyy") + " в " + content.CreateDate.ToString("HH:mm");
                string image = "";
                var files = _fileStorageService.SaveFiles(_environment.WebRootPath, Request.Form.Files, content.ID, "content");
                if (files != null && files.Count > 0)
                {
                    image = files[0].Filename;
                    content.Image = image;
                    _contentRepository.UpdateContentImage(content);
                }
                // TODO для отрисовки ссылка на картинку
                vm.ImageLink = LinkBuilder.Image(image, true);

            }
            else
            {

            }

            return Json(vm);
        }


        //TODO пока комментарии только для записей в ленте.
        /// <summary>
        /// Список комментариев.
        /// </summary>
        /// <param name="contentID"></param>
        /// <returns></returns>
        public IActionResult GetComments(int objectID, int lastID = 0)
        {
            //TODO через viewmodel
            var comments = _contentRepository.GetContentCommentList(objectID, lastID);
            var res = new List<CommentViewModel>();
            if (comments != null)
            {
                foreach (var comment in comments)
                {
                    res.Add(new CommentViewModel(comment));
                }
            }
            return Json(new
            {
                comments = res
            });
        }
        [HttpPost]
        public JsonResult AddComment()
        {
            CommentAddViewModel vm = new CommentAddViewModel();
            string text = RequestUtils.GetFormText(Request, "text");
            int contentID = RequestUtils.GetFormInt(Request, "objectid");

            if (_profileService.CurrentProfile != null && !string.IsNullOrEmpty(text) && contentID > 0)
            {

                {

                    var comment = new Comment
                    {
                        ProfileID = _profileService.CurrentProfile.ID,
                        Text = text,
                        CreateDate = DateTime.Now,
                        ObjectID = contentID,
                        ObjectType = "content",
                        ProfileName = _profileService.CurrentProfile.Name,
                        ProfileImage = _profileService.CurrentProfile.Image
                    };
                    comment.ID = _contentRepository.AddComment(comment);
                    vm.Status = "success";

                    var content = _contentRepository.GetContent(comment.ObjectID);
                    if (content != null)
                        vm.CommentCount = content.CommentCount;
                    //     vm.Comment = comment;
                    // Формирование даты для вывода.
                    //vm.Date = content.CreateDate.ToString("dd MMMM yyyy") + " в " + content.CreateDate.ToString("HH:mm");                    

                    vm.Comment = new CommentViewModel(comment);

                }




            }
            return Json(vm);
        }

        /// <summary>
        /// Получить список записей для ленты с листованием
        /// </summary>
        /// <param name="profileID"></param>
        /// <param name="page"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public IActionResult GetContents(int profileID, int page, int count)
        {
            var vm = new ProfileCardViewModel();
            if (profileID > 0)
            {
                if (count < 1)
                    count = 20;
                if (page < 1)
                    page = 1;
                // Получить список контента (записей в ленту)
                var contentList = _contentRepository.GetContentList(profileID, page, count);
                // Получить последние комментарии для записей в ленту.
                List<Comment> contentCommentList = null;
                if (contentList != null && contentList.Count > 0)
                {
                    contentCommentList = _contentRepository.GetLastContentCommentList(contentList.Select(u => u.ID).ToList());
                    vm.PrepareContentData(contentList, contentCommentList);
                    int totalContentCount = _contentRepository.GetContentListTotalCount(profileID);
                    vm.Pagination = new PaginationData(_localizationService, LinkBuilder.Profile.Card(profileID), Utils.RequestUtils.GetRequesValues(Request), page, count, totalContentCount);
                    

                }
            }

            return Json(new
            {
                vm.ContentList,
                vm.Pagination

            }
                );


        }
    }
}