using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Crm.Helpers
{
    public class RequestHelper
    {
        public static string GetRequestText(HttpRequestBase request, string param)
        {
            return GetRequestText(request, param, "");
            /*string res = "";
            try
            {
                res = request[param];
            }
            catch
            {
            }
            return res;*/
        }
        public static string GetRequestText(HttpRequestBase request, string param, string defaultValue)
        {
            string res = defaultValue;
            try
            {
                res = request[param];
            }
            catch
            {
            }
            if (string.IsNullOrEmpty(res))
                res = defaultValue;
            return res;
        }

        public static string GetRequestText(HttpRequest request, string param, string defaultValue = "")
        {
            string res = defaultValue;
            try
            {
                res = request[param];
            }
            catch
            {
            }
            if (string.IsNullOrEmpty(res))
                res = defaultValue;
            return res;
        }
    }
}