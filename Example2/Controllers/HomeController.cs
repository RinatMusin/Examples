using CitystarCms.Models;
using CitystarCms.Services;
using CitystarCms.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace CitystarCms.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/
        private DataService dataService = new DataService();

        public string Order(string dateFrom, string dateTo, string phone, string email)
        {
            var order = new Order
            {
                DateFrom = dateFrom,
                DateTo = dateTo,
                Phone = phone,
                Email = email,
                CreateDate = DateTime.Now
            };
            dataService.SaveOrder(order);
            //отправить на почту заказ
            StringBuilder demail = new StringBuilder();
            demail.AppendLine(@"<!DOCTYPE html PUBLIC '-//W3C//DTD XHTML 1.0 Transitional//EN' 'http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd'>
<html lang='ru' xml:lang='ru' xmlns='http://www.w3.org/1999/xhtml'>

<head>
	<meta http-equiv='Content-Type' content='text/html; charset=UTF-8' />
	<title>Новое бронирование номера с сайта nadejda-bannoe.ru</title>
</head>

<body style='margin:0px; font: 12.8px Arial,Helvetica,sans-serif;'>");
            demail.AppendLine("<div>");
            demail.AppendLine("<h1>Забронирован номер</h1>");

            demail.AppendFormat("<div style='margin:10px 0;'>Дата бронирования: {0}</div>", order.CreateDate.ToShortDateString() + " " + order.CreateDate.ToShortTimeString());
            demail.AppendFormat("<div style='margin:10px 0;'>Дата заезда: {0}</div>", order.DateFrom);
            demail.AppendFormat("<div style='margin:10px 0;'>Дата выезда: {0}</div>", order.DateTo);
            demail.AppendFormat("<div style='margin:10px 0;'>Контактный номер: {0}</div>", order.Phone);
            demail.AppendFormat("<div style='margin:10px 0;'>Email: {0}</div>", order.Email);

            string siteUrl = string.Format("http://www.nadejda-bannoe.ru{0}", LinkBuilder.AdminOrders());
            demail.AppendFormat("<div style='margin:10px 0;'><a href='{0}'>Перейти к просмотру забронированных номеров</a></div>", siteUrl);
            demail.AppendLine("</div></body></html>");
            EmailService.SendNewOrderEvent("bannoe-nadejda@mail.ru", demail.ToString());
            //   EmailService.SendNewOrderEvent("testcitystar@mail.ru", demail.ToString());
            return "success";
        }

        public ActionResult Index()
        {
            var vm = new PageViewModel();
            vm.Page = dataService.GetPage("home");

            return View("HomePage", vm);
        }
        public ActionResult Contacts()
        {
            var vm = new PageViewModel();
            vm.Page = dataService.GetPage("kontakty");

            return View("ContactsPage", vm);
        }
        public ActionResult Information()
        {
            var vm = new PageViewModel();
            vm.Page = dataService.GetPage("informaciya");
            return View("InformationPage", vm);
        }
        /// <summary>
        /// Активный отдых
        /// </summary>
        /// <returns></returns>
        public ActionResult Active()
        {
            var vm = new ActiveViewModel();
            vm.Articles = dataService.GetArticles();
            return View("ActivePage", vm);
        }

        public ActionResult Progivanie()
        {
            var vm = new PageViewModel();
            vm.Page = dataService.GetPage("progivanie");
            return View("ProgivaniePage", vm);
        }


        public ActionResult Article(string articleUrl)
        {
            /*var artice = new Article
            {
                Title = "Горнолыжный центр ГЛЦ Металлург",
                ShortText = "На юге Урала, на склонах горного массива Яманкай располагается горнолыжный курорт «Металлург». Благоприятные рельефно – климатические условия и чистейшие воды озеро сделали этот район одним из самых популярных горнолыжных курортов России. Здоровый экологический фон и чистота атмосферы в этом районе являются одной из главных причин высокой посещаемости этого места.",
                Text = @"
            &nbsp;&nbsp;&nbsp;На юге Урала, на склонах горного массива Яманкай располагается горнолыжный курорт «Металлург». Благоприятные рельефно – климатические условия и чистейшие воды озеро сделали этот район одним из самых популярных горнолыжных курортов России. Здоровый экологический фон и чистота атмосферы в этом районе являются одной из главных причин высокой посещаемости этого места.<br /><br />

            &nbsp;&nbsp;&nbsp;Здесь среди Уральских лесов, по горным склонам тянутся к востоку нити пяти горнолыжных трасс. Эти трассы отличаются по протяженности, уклону и линейности и рассчитаны на лыжников различной степени подготовленности. Башкирия может по праву гордиться тем, что это единственный в РФ горнолыжный центр, оборудованный гондольным подъемником австрийского производства. Подъемник перемещает 38 кабинок по 8 мест каждая, по трассе с общей протяженностью свыше полутора километров. Здесь так же есть бугельный подъемник. Горнолыжный сезон здесь длится с середины ноября до конца мая благодаря итальянской системе искусственного заснеживания. Горнолыжный центр состоит из двух комплексов большого и малого. Трасса малого комплекса имеет протяженность 300м, ее ширина 40-50 м с перепадом высот 45м. На этой трасе хорошо начинать обучение катанию на лыжах. На трассе есть бугельный подъемник, а благодаря освещению кататься на ней можно даже ночью. Якты –куль - так издревле называлось озеро Банное, расположенное под горой Крых –Тау. Озеро находится всего в полукилометре от «Металлурга». Прогулки по озеру доставят массу удовольствия отдыхающим в горнолыжном центре. Любители рыбалки смогут насладиться любимым занятием на берегу озера. Вокруг озера открывается чудесная панорама -прекрасный вид на заснеженные Уральские горы и леса.<br /><br />

            &nbsp;&nbsp;&nbsp;Горнолыжный центр «металлург» имеет развитую туристическую инфраструктуру. К услугам туристов здесь функционирует ряд гостиниц, есть санатории, где можно отдохнуть всей семьей. Собираясь отдохнуть на горнолыжном курорте, не отчаивайтесь, что не приобрели лыжи, здесь к Вашим услугам прокат лыж, сноубордов, саней и прочих спортивных снарядов. Тут Вы сможете поиграть с друзьями в пейнтбол или пожарить шашлыки под открытым небом. Активный отдых это то зачем действительно стоит сюда ехать, ведь для этого здесь масса возможностей.<br /><br />

            &nbsp;&nbsp;&nbsp;На этот курорт можно поехать как в одиночестве, так и с веселой компанией друзей. В любом случае все отдыхающие останутся довольны поездкой. Для того чтобы Вы чувствовали себя комфортно в горнолыжном центре «Металлург» имеются все удобства. Здесь есть специально оборудованные площадки для пикников, ряд кафе, кемпинги, а по вечерам для молодежи открыты клубы и дискотеки.<br /><br />

            &nbsp;&nbsp;&nbsp;Если Вы еще не были на Урале, то советуем вам начать знакомство с этим удивительным краем именно с посещения горнолыжного центра. Побывав там однажды, вам непременно захочется приехать туда вновь."
            };
            */
            var article = dataService.GetArticle(articleUrl);

            var vm = new ArticleViewModel();
            if (article != null)
            {
                vm.Title = article.Title;
                vm.Text = article.Text;
            };

            return View("ArticlePage", vm);
        }
    }
}
