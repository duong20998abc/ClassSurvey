using System.Web.Mvc;

namespace ClassSurvey.Areas.Member
{
    public class MemberAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Member";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Member_default",
                "Member/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );

			context.MapRoute(
				"StudentIndex",
				"student/home/{id}",
				new {controller = "Student", action = "Index", id = UrlParameter.Optional }
			);

			context.MapRoute(
				"StudentInfo",
				"Student/info/{id}",
				new { controller = "Student", action = "ShowStudentInfo", id = UrlParameter.Optional }
			);

			context.MapRoute(
				"StudentClasses",
				"Student/list-classes/{id}",
				new { controller = "Student", action = "ShowListClass", id = UrlParameter.Optional }
			);

			context.MapRoute(
				"Survey",
				"Student/survey/{id}",
				new { controller = "Student", action = "DoSurvey", id = UrlParameter.Optional }
			);

			context.MapRoute(
				"TeacherIndex",
				"Teacher/Index/{id}",
				new { controller = "Teacher", action = "Index", id = UrlParameter.Optional }
			);

			context.MapRoute(
				"TeacherInfo",
				"Teacher/info/{id}",
				new { controller = "Teacher", action = "ShowTeacherInfo", id = UrlParameter.Optional }
			);

			context.MapRoute(
				"TeacherListClasses",
				"Teacher/list-classes/{id}",
				new { controller = "Teacher", action = "DoSurvey", id = UrlParameter.Optional }
			);

			context.MapRoute(
				"SurveyResults",
				"Teacher/survey-results/{id}",
				new { controller = "Teacher", action = "ShowSurveyResult", id = UrlParameter.Optional }
			);

			context.MapRoute(
				"TeacherListStudents",
				"Teacher/list-students/{id}",
				new { controller = "Teachers", action = "GetStudentsInClass", id = UrlParameter.Optional }
			);
		}
    }
}