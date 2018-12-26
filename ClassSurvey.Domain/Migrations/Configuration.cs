namespace ClassSurvey.Domain.Migrations
{
	using ClassSurvey.Domain.Entities;
	using System;
	using System.Collections.Generic;
	using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<ClassSurvey.Domain.ClassSurveyDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(ClassSurvey.Domain.ClassSurveyDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.

			//tao 1 admin moi neu chua co admin nao ton tai
			if(!context.Users.Any(u => u.Position == "Admin")) {
				context.Users.Add(new Entities.User {
					Username = "admin",
					Password = "000000",
					Position = "Admin"
				});
			}
			if(context.UserRoles.ToList().Count() == 0)
			{
				List<UserRole> roles = new List<UserRole>
				{
					new UserRole{Position = "Admin", Area = "Admin", Controller = "Home", Action = "Index"},
					new UserRole{Position = "Admin", Area = "Admin", Controller = "Students", Action = "Index"},
					new UserRole{Position = "Admin", Area = "Admin", Controller = "Students", Action = "Details"},
					new UserRole{Position = "Admin", Area = "Admin", Controller = "Students", Action = "Create"},
					new UserRole{Position = "Admin", Area = "Admin", Controller = "Students", Action = "ImportStudentFromExcel"},
					new UserRole{Position = "Admin", Area = "Admin", Controller = "Students", Action = "Edit"},
					new UserRole{Position = "Admin", Area = "Admin", Controller = "Students", Action = "Delete"},
					new UserRole{Position = "Admin", Area = "Admin", Controller = "Teachers", Action = "Index"},
					new UserRole{Position = "Admin", Area = "Admin", Controller = "Teachers", Action = "Details"},
					new UserRole{Position = "Admin", Area = "Admin", Controller = "Teachers", Action = "Create"},
					new UserRole{Position = "Admin", Area = "Admin", Controller = "Teachers", Action = "Edit"},
					new UserRole{Position = "Admin", Area = "Admin", Controller = "Teachers", Action = "Delete"},
					new UserRole{Position = "Admin", Area = "Admin", Controller = "Teachers", Action = "ImportTeacherFromExcel"},
					new UserRole{Position = "Admin", Area = "Admin", Controller = "Classes", Action = "Index"},
					new UserRole{Position = "Admin", Area = "Admin", Controller = "Classes", Action = "Details"},
					new UserRole{Position = "Admin", Area = "Admin", Controller = "Classes", Action = "Create"},
					new UserRole{Position = "Admin", Area = "Admin", Controller = "Classes", Action = "Delete"},
					new UserRole{Position = "Admin", Area = "Admin", Controller = "Classes", Action = "ImportClassFromExcel"},
					new UserRole{Position = "Admin", Area = "Admin", Controller = "Classes", Action = "ShowSurveyResult"},
					new UserRole{Position = "Admin", Area = "Admin", Controller = "Classes", Action = "ShowStudentsInClass"},
					new UserRole{Position = "Admin", Area = "Admin", Controller = "Classes", Action = "ShowSurveyResultByStudent"},
					new UserRole{Position = "Admin", Area = "Admin", Controller = "Surveys", Action = "Index"},
					new UserRole{Position = "Admin", Area = "Admin", Controller = "Surveys", Action = "Create"},
					new UserRole{Position = "Admin", Area = "Admin", Controller = "Surveys", Action = "Edit"},
					new UserRole{Position = "Admin", Area = "Admin", Controller = "Surveys", Action = "Delete"},
					new UserRole{Position = "Student", Area = "Member", Controller = "Student", Action = "Index"},
					new UserRole{Position = "Student", Area = "Member", Controller = "Student", Action = "ShowStudentInfo"},
					new UserRole{Position = "Student", Area = "Member", Controller = "Student", Action = "ShowListClass"},
					new UserRole{Position = "Student", Area = "Member", Controller = "Student", Action = "DoSurvey"},
					new UserRole{Position = "Teacher", Area = "Member", Controller = "Teacher", Action = "Index"},
					new UserRole{Position = "Teacher", Area = "Member", Controller = "Teacher", Action = "ShowTeacherInfo"},
					new UserRole{Position = "Teacher", Area = "Member", Controller = "Teacher", Action = "ShowListClasses"},
					new UserRole{Position = "Teacher", Area = "Member", Controller = "Teacher", Action = "GetStudentsInClass"},
					new UserRole{Position = "Teacher", Area = "Member", Controller = "Teacher", Action = "ShowSurveyResult"}
				};
				context.UserRoles.AddRange(roles);
				context.SaveChanges();
			}
        }
    }
}
