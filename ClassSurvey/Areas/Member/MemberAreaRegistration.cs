using System.Web.Mvc;

namespace ClassSurvey.Areas.Member
{
	//Route Config for Member Page
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
				"StudentIndex",
				"student/home/{id}",
				new {controller = "Student", action = "Index", id = UrlParameter.Optional }
			);

			context.MapRoute(
				"StudentInfo",
				"student/info/{id}",
				new { controller = "Student", action = "ShowStudentInfo", id = UrlParameter.Optional }
			);

			context.MapRoute(
				"StudentClasses",
				"student/list-classes/{id}",
				new { controller = "Student", action = "ShowListClass", id = UrlParameter.Optional }
			);

			context.MapRoute(
				"Survey",
				"student/survey/{id}",
				new { controller = "Student", action = "DoSurvey", id = UrlParameter.Optional }
			);

			context.MapRoute(
				"TeacherIndex",
				"teacher/home/{id}",
				new { controller = "Teacher", action = "Index", id = UrlParameter.Optional }
			);

			context.MapRoute(
				"TeacherInfo",
				"teacher/info/{id}",
				new { controller = "Teacher", action = "ShowTeacherInfo", id = UrlParameter.Optional }
			);

			context.MapRoute(
				"TeacherListClasses",
				"teacher/list-classes/{id}",
				new { controller = "Teacher", action = "ShowListClasses", id = UrlParameter.Optional }
			);

			context.MapRoute(
				"SurveyResults",
				"teacher/survey-results/{id}",
				new { controller = "Teacher", action = "ShowSurveyResult", id = UrlParameter.Optional }
			);

			context.MapRoute(
				"TeacherListStudents",
				"teacher/list-students/{id}",
				new { controller = "Teacher", action = "GetStudentsInClass", id = UrlParameter.Optional }
			);

			context.MapRoute(
				"Member_default",
				"Member/{controller}/{action}/{id}",
				new { action = "Index", id = UrlParameter.Optional }
			);
		}
    }
}