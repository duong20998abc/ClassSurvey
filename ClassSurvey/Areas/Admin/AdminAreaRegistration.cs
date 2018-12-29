using System.Web.Mvc;

namespace ClassSurvey.Areas.Admin
{
	//RouteConfig For Admin Page
    public class AdminAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Admin";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {

			context.MapRoute(
				"AdminIndex",
				"admin/home/{id}",
				new {controller = "Home", action = "Index", Id = UrlParameter.Optional}
			);

			context.MapRoute(
				"AdminListStudents",
				"student/list-students",
				new { controller = "Students", action = "Index", Id = UrlParameter.Optional }
			);

			context.MapRoute(
				"CreateNewStudent",
				"student/create/{id}",
				new { controller = "Students", action = "Create", Id = UrlParameter.Optional }
			);

			context.MapRoute(
				"AdminDetailStudents",
				"student/detail/{id}",
				new { controller = "Students", action = "Details", Id = UrlParameter.Optional }
			);

			context.MapRoute(
				"ImportStudentFromExcel",
				"student/import/{id}",
				new { controller = "Students", action = "ImportStudentFromExcel", Id = UrlParameter.Optional }
			);

			context.MapRoute(
				"EditStudent",
				"student/edit/{id}",
				new { controller = "Students", action = "Edit", Id = UrlParameter.Optional }
			);

			context.MapRoute(
				"DeleteStudent",
				"student/delete/{id}",
				new { controller = "Students", action = "Delete", Id = UrlParameter.Optional }
			);

			context.MapRoute(
				"AdminListTeachers",
				"teacher/list-teachers",
				new { controller = "Teachers", action = "Index", Id = UrlParameter.Optional }
			);

			context.MapRoute(
				"CreateNewTeacher",
				"teacher/create/{id}",
				new { controller = "Teachers", action = "Create", Id = UrlParameter.Optional }
			);

			context.MapRoute(
				"AdminDetailTeacher",
				"teacher/detail/{id}",
				new { controller = "Teachers", action = "Details", Id = UrlParameter.Optional }
			);

			context.MapRoute(
				"ImportTeacherFromExcel",
				"teacher/import/{id}",
				new { controller = "Teachers", action = "ImportTeacherFromExcel", Id = UrlParameter.Optional }
			);

			context.MapRoute(
				"EditTeacher",
				"teacher/edit/{id}",
				new { controller = "Teachers", action = "Edit", Id = UrlParameter.Optional }
			);


			context.MapRoute(
				"DeleteTeacher",
				"teacher/delete/{id}",
				new { controller = "Teachers", action = "Delete", Id = UrlParameter.Optional }
			);

			context.MapRoute(
				"AdminListClasses",
				"class/list-classes",
				new { controller = "Classes", action = "Index", Id = UrlParameter.Optional }
			);

			context.MapRoute(
				"ImportClassFromExcel",
				"class/import/{id}",
				new { controller = "Classes", action = "ImportClassFromExcel", Id = UrlParameter.Optional }
			);

			context.MapRoute(
				"AdminShowSurveyResult",
				"class/survey-result/{id}",
				new { controller = "Classes", action = "ShowSurveyResult", Id = UrlParameter.Optional }
			);

			context.MapRoute(
				"StudentsInClass",
				"class/list-students/{id}",
				new { controller = "Classes", action = "ShowStudentsInClass", Id = UrlParameter.Optional }
			);

			context.MapRoute(
				"SurveyByStudent",
				"class/student-survey/{id}",
				new { controller = "Classes", action = "ShowSurveyResultByStudent", Id = UrlParameter.Optional }
			);

			context.MapRoute(
				"AdminSurveyList",
				"survey/list-criteria",
				new { controller = "Surveys", action = "Index", Id = UrlParameter.Optional }
			);

			context.MapRoute(
				"AddSurveyQuestion",
				"survey/create/{id}",
				new { controller = "Surveys", action = "Create", Id = UrlParameter.Optional }
			);

			context.MapRoute(
				"EditSurveyQuestion",
				"survey/edit/{id}",
				new { controller = "Surveys", action = "Edit", Id = UrlParameter.Optional }
			);

			context.MapRoute(
				"DeleteSurveyQuestion",
				"survey/delete/{id}",
				new { controller = "Surveys", action = "Delete", Id = UrlParameter.Optional }
			);

			context.MapRoute(
				"Admin_default",
				"Admin/{controller}/{action}/{id}",
				new { action = "Index", id = UrlParameter.Optional }
			);
		}
    }
}