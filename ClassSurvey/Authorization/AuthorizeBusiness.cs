using ClassSurvey.Domain;
using ClassSurvey.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ClassSurvey.Authorization
{
	//authorize function
	public class AuthorizeBusiness : ActionFilterAttribute
	{
		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			base.OnActionExecuting(filterContext);
			//get user
			User user = HttpContext.Current.Session["User"] as User;
			//if user not exists
			if(user == null)
			{
				filterContext.Result = new RedirectResult("/home");
				return;
			}

			//get token
			var token = HttpContext.Current.Request.RequestContext.RouteData.DataTokens;
			//get routeValues
			var routeValues = HttpContext.Current.Request.RequestContext.RouteData.Values;
			//get action
			string action = routeValues.ContainsKey("action") ? (string)routeValues["action"] : string.Empty;
			//get controller
			string controller = routeValues.ContainsKey("controller") ? (string)routeValues["controller"] : string.Empty;
			//get area
			string area = token.ContainsKey("area") ? (string)token["area"] : string.Empty;
			ClassSurveyDbContext db = new ClassSurveyDbContext();
			//check to authorize user
			bool check = db.UserRoles.Any(s => s.Position == user.Position && s.Area == area && s.Controller == controller && s.Action == action);
			if(!check)
			{
				//if not match the roles, cant get in the page -> redirect to Not Authorize
				filterContext.Result = new RedirectResult("/notauthorized");
				return;
			}
		}
	}
}