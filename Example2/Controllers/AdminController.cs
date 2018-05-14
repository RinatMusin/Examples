using CitystarCms.Models;
using CitystarCms.Services;
using CitystarCms.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace CitystarCms.Controllers
{
    public class AdminController : Controller
    {
        private DataService dataService = new DataService();

        protected override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            //проверка на авторизацию

            if (!User.Identity.IsAuthenticated && Request.RawUrl.ToLower().IndexOf("login") < 0)
            {
                context.HttpContext.Response.Redirect(LinkBuilder.Login(Request.RawUrl));

            }

        }
        [HttpGet]
        public ActionResult Login()
        {
            var vm = new LoginViewModel();

            return View(vm);
        }
        [HttpPost]
        public ActionResult Login(LoginViewModel form, string url)
        {
            bool valid = true;
            if (string.IsNullOrEmpty(form.Email) ||
            string.IsNullOrEmpty(form.Password))
                valid = false;
            else
            {
                var user = dataService.GetUserByEmail(form.Email);
                //проверка на соответствие паролю
                if (user != null && user.Password == form.Password)
                {
                    //авторизация
                    FormsAuthentication.SetAuthCookie(user.Email, true);
                }
                else
                    valid = false;

            }

            if (!valid)
            {
                var vm = new LoginViewModel();
                vm.ErrorMessage = "Ошибка авторизации";
                return View(vm);
            }
            else
            {
                if (string.IsNullOrEmpty(url))
                    url = LinkBuilder.AdminOrders();
                return Redirect(url);
            }


        }
        public ActionResult Logout()
        {
            if (User.Identity.IsAuthenticated)
            {
                FormsAuthentication.SignOut();
            }
            return Redirect(LinkBuilder.Home());
        }

        protected void setActiveMenuIndex(int index)
        {
            ViewBag.ActiveMenuIndex = index;
        }
        public ActionResult Index()
        {
            return Redirect(LinkBuilder.AdminOrders());
            return View("AdminPage");
        }

        public ActionResult Orders(int id = 1)
        {
            int page = id;
            int pageSize = 20;
            setActiveMenuIndex(4);
            var vm = new AdminOrdersViewModel();
            vm.Orders = dataService.GetOrders(page, pageSize);
            vm.UsePaging = true;
            vm.BaseUrl = LinkBuilder.AdminOrders();
            vm.CurrentPage = page;
            vm.PageSize = pageSize;
            vm.OrdersCount = dataService.GetOrdersCount();


            return View("Orders", vm);
        }


        public ActionResult Pages()
        {
            setActiveMenuIndex(2);
            var vm = new AdminPagesViewModel();
            vm.Pages = dataService.GetPages();
            return View("Pages", vm);
        }
        public ActionResult PageEdit(int id = 0)
        {
            setActiveMenuIndex(2);
            var vm = new AdminPageViewModel();
            if (id > 0)
            {
                var page = dataService.GetPage(id);
                vm.Title = page.Title;
                vm.Text = page.Text;
                vm.PageType = page.PageType;
                vm.Url = page.Url;
                vm.ID = page.ID;
            }
            return View("Page", vm);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult PageEdit(AdminArticleViewModel form, int id)
        {
            setActiveMenuIndex(2);
            //сохранение
            Page page = dataService.GetPage(id);
            if (page != null)
            {

                page.Text = form.Text;
                page.Title = form.Title;

                dataService.SavePage(page);
                return Redirect(LinkBuilder.AdminPages());
            }

            return View("Page", form);
        }


        public ActionResult Articles()
        {
            setActiveMenuIndex(1);
            var vm = new AdminArticlesViewModel();
            vm.Articles = dataService.GetArticles();
            return View("ArticlesPage", vm);
        }
        public ActionResult ArticleDelete(int id)
        {

            if (id > 0)
            {
                var article = dataService.GetArticle(id);
                dataService.DeleteArticle(article);

            }
            return Redirect(LinkBuilder.AdminArticles());
        }
        public ActionResult ArticleEdit(int id = 0)
        {
            setActiveMenuIndex(1);
            var vm = new AdminArticleViewModel();
            if (id > 0)
            {
                var article = dataService.GetArticle(id);
                vm.Title = article.Title;
                vm.Text = article.Text;
                vm.ShortText = article.ShortText;
                vm.Url = article.Url;
                vm.ID = article.ID;
            }
            return View("ArticlePage", vm);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ArticleEdit(AdminArticleViewModel form, int id = 0)
        {
            setActiveMenuIndex(1);
            //сохранение
            Article article = id == 0 ? new Article() : dataService.GetArticle(id);
            if (article != null)
            {
                article.ShortText = form.ShortText;
                article.Text = form.Text;
                article.Title = form.Title;
                //если нет URL то прописываю
                if (string.IsNullOrEmpty(article.Url))
                    article.Url = TextHelper.Translit(article.Title);

                dataService.SaveArticle(article);
                return Redirect(LinkBuilder.AdminArticles());
            }

            return View("ArticlePage", form);
        }


    }
}
