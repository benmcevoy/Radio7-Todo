using Radio7.Unity.Decorators;
using System.Configuration;

namespace Radio7.Todo.Server.Infrastructure
{
    [Singleton]
    public class Producers
    {
        public string CreateCookieValue()
        {
            return ConfigurationManager.AppSettings["CookieValue"];
        }
    }
}