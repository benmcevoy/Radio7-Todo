using System.Web.Http;

namespace Radio7.Todo.Server.Infrastructure
{
    public class ApiSessionAuthorizationFilter : AuthorizeAttribute
    {
        protected override bool IsAuthorized(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            return new CookieService().IsSet(actionContext);
        }
    }
}