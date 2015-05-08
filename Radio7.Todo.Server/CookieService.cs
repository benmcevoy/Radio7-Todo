using System;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http.Controllers;

namespace Radio7.Todo.Server
{
    public class CookieService
    {
        private const string CookieName = "E2097506F1A7418BB379596BD0D17B48";
        private const string CookieItem = "D8DA259A422A4B0B890BF0CE6F4C1224";
        private static readonly string CookieValue = "189D480A98894A158E8AB1DF756CF08D";

        static CookieService()
        {
            CookieValue = ConfigurationManager.AppSettings["Radio7.Todo.Server.CookieService.CookieValue"];
        }

        public void Create(HttpContextBase context)
        {
            var cookie = new HttpCookie(CookieName);

            cookie[CookieItem] = CookieValue;
            cookie.Expires = DateTime.Now.AddMonths(1);

            context.Response.Cookies.Add(cookie);
        }

        public bool IsSet(HttpContextBase context)
        {
            if (context.Request.Cookies[CookieName] == null) return false;
            if (context.Request.Cookies[CookieName][CookieItem] == null) return false;

            return context.Request.Cookies[CookieName][CookieItem] == CookieValue;
        }

        public bool IsSet(HttpActionContext context)
        {
            var cookies = context.Request.Headers.GetCookies(CookieName);

            if (!cookies.Any()) return false;

            var cookie = cookies.First().Cookies;

            if (!cookie.Any()) return false;

            var securityCookie = cookie.FirstOrDefault(c => c.Name == CookieName);

            if (securityCookie == null) return false;

            return securityCookie.Value == CookieItem;
        }

        public void Remove(HttpContextBase context)
        {
            if (context.Request.Cookies[CookieName] == null) return;

            var cookie = new HttpCookie(CookieName) { Expires = DateTime.Now.AddDays(-2) };

            context.Response.Cookies.Add(cookie);
        }
    }
}