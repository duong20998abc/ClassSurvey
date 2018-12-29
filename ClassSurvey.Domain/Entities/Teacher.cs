using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ClassSurvey.Domain.Entities
{
	//Teacher(Entity)
	public class Teacher : BaseEntity
	{
		//constructor
		public Teacher()
		{
			//default info
			Phone = "0123456789";
			Office = "404E3-UET-VNU";
		}
		
		[StringLength(256)]
		public string TeacherName { get; set; }

		[StringLength(100)]
		public string Email { get; set; }

		[StringLength(50)]
		public string Phone { get; set; }

		[StringLength(100)]
		public string Office { get; set; }

		[Required]
		[StringLength(100)]
		public string Username { get; set; }

		[Required]
		[MaxLength(1000)]
		public string Password { get; set; }

		public virtual ICollection<StudentClass> StudentClasses { get; set; }
	}
}