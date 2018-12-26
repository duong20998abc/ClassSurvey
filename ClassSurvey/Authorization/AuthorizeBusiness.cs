using ClassSurvey.Domain;
using ClassSurvey.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ClassSurvey.Authorization
{
	public class AuthorizeBusiness : ActionFilterAttribute
	{
		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			base.OnActionExecuting(filterContext);
			//lay ra user theo session
			User user = HttpContext.Current.Session["User"] as User;
			//check user co ton tai trong he thong hay khong
			if(user == null)
			{
				filterContext.Result = new RedirectResult("/home");
				return;
			}

			var token = HttpContext.Current.Request.RequestContext.RouteData.DataTokens;
			var routeValues = HttpContext.Current.Request.RequestContext.RouteData.Values;
			string action = routeValues.ContainsKey("action") ? (string)routeValues["action"] : string.Empty;
			string controller = routeValues.ContainsKey("controller") ? (string)routeValues["controller"] : string.Empty;
			string area = token.ContainsKey("area") ? (string)token["area"] : string.Empty;
			ClassSurveyDbContext db = new ClassSurveyDbContext();
			bool check = db.UserRoles.Any(s => s.Position == user.Position && s.Area == area && s.Controller == controller && s.Action == action);
			if(!check)
			{
				filterContext.Result = new RedirectResult("/notauthorized");
				return;
			}
		}
	}
}