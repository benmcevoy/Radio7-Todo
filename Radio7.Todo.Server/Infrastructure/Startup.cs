using System.Net.Http.Headers;
using System.Web.Http;
using Cormo.Injects;

namespace Radio7.Todo.Server.Infrastructure
{
    [Configuration]
    public class Startup
    {
        [Inject]
        void Register(HttpConfiguration config)
        {
            config.EnableCors();

            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));

            config.Filters.Add(new ApiSessionAuthorizationFilter());
        }
    }
}