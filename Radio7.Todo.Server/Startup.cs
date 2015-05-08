using System.Net.Http.Headers;
using System.Web.Http;
using Cormo.Injects;

namespace Radio7.Todo.Server
{
    [Configuration]
    public class Startup
    {
        [Inject]
        void Register(HttpConfiguration config)
        {
            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));
        }
    }
}