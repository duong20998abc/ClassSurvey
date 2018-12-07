using ClassSurvey.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace ClassSurvey.Domain
{
	public class ClassSurveyDbContext : DbContext 
	{
		public ClassSurveyDbContext() : base("ClassSurveyDbContext")
		{

		}

		public DbSet<Admin> Admins { get; set; }
		public DbSet<Class> Classes { get; set; }
		public DbSet<Student> Students { get; set; }
		public DbSet<StudentClass> StudentClasses { get; set; }
		public DbSet<Teacher> Teachers { get; set; }
		public DbSet<Survey> Surveys { get; set; }
		public DbSet<SurveyQuestion> SurveyQuestions { get; set; }
		public DbSet<User> Users { get; set; }
		public DbSet<UserRole> UserRoles { get; set; }
	}
}