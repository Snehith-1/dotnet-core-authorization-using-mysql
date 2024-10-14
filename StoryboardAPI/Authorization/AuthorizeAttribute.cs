using System.Web.Http.Controllers;

namespace StoryboardAPI.Authorization
{
    public class AuthorizeAttribute : System.Web.Http.AuthorizeAttribute
    {
        private readonly IHttpContextAccessor _contextAccessor;
        public AuthorizeAttribute(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        protected override void HandleUnauthorizedRequest(HttpActionContext actionContext)
        {
            var contextAccessor = _contextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;
            if (!contextAccessor)
            {
                base.HandleUnauthorizedRequest(actionContext);
            }
            else
            {
                actionContext.Response = new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.Forbidden);
            }
        }
    }
}