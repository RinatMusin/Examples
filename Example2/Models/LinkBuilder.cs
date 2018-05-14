using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class LinkBuilder
{
    public static string AdminRooms()
    {
        return "/admin/rooms";
    }
    public static string AdminRoom(int id)
    {
        return "/admin/roomedit/" + id;
    }
    public static string AdminOrders()
    {
        return "/admin/Orders/";
    }

    public static string AdminArticleDelete(int id)
    {
        return "/admin/articledelete/" + id;
    }
    public static string AdminArticles()
    {
        return "/admin/articles";
    }
    public static string AdminArticle(int id = 0)
    {
        string res = "/admin/articleedit";
        if (id > 0)
            res += "/" + id;
        return res;
    }

    public static string AdminPages()
    {
        return "/admin/pages";
    }
    public static string AdminPage(int id)
    {
        return "/admin/pageedit/" + id;

    }




    public static string Article(string url)
    {
        return "/article/" + url + ".html";
    }

    public static string Contacts()
    {
        return "/kontakty.html";
    }
    public static string Active()
    {
        return "/activniy_otdyx.html";
    }
    public static string Progivanie()
    {
        return "/progivanie.html";
    }
    public static string Home()
    {
        return "/";
    }
    public static string Information()
    {
        return "/informaciya.html";
    }

    public static string Login(string returnUrl = "")
    {
        string res = "/admin/login";
        if (!string.IsNullOrEmpty(returnUrl))
            res += "?url=" + returnUrl;
        return res;
    }
}
