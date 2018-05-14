using CitystarCms.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CitystarCms.Services
{
    public class DataService : BaseService
    {
 

        #region Статьи
        public Article GetArticle(string url)
        {
            return db.Articles.Where(a => a.Url.ToLower() == url.ToLower()).FirstOrDefault();
        }

        public List<Article> GetArticles()
        {
            return db.Articles.OrderByDescending(a => a.ID).ToList();
        }

        internal Article GetArticle(int id)
        {
            return db.Articles.Where(a => a.ID == id).FirstOrDefault();
        }

        internal void SaveArticle(Article article)
        {
            if (article.ID == 0)
            {
                //добавление
                db.Articles.Add(article);
            }

            db.SaveChanges();
        }

        internal void DeleteArticle(Article article)
        {
            db.Articles.Remove(article);
            db.SaveChanges();
        }
        #endregion



        internal UserData GetUserByEmail(string login)
        {
            return db.Users.Where(u => u.Email.ToLower() == login.ToLower()).FirstOrDefault();
        }
    }
}