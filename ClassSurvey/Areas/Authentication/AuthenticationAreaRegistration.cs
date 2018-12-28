using System.Web.Mvc;

namespace ClassSurvey.Areas.Authentication
{
    public class AuthenticationAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Authentication";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
			context.MapRoute(
                "Home",
                "home",
                new { controller = "Authentication", action = "Home", id = UrlParameter.Optional }
            );

			context.MapRoute(
				"notfound",
				"notfound",
				new {controller = "Authentication", action = "Page404", id = UrlParameter.Optional}
			);

			context.MapRoute(
				"notauthorized",
				"notauthorized",
				new { controller = "Authentication", action = "NotAuthorized", id = UrlParameter.Optional }
			);

			context.MapRoute(
				"Authentication_default",
				"Authentication/{controller}/{action}/{id}",
				new { action = "Index", id = UrlParameter.Optional }
			);
		}
    }
}