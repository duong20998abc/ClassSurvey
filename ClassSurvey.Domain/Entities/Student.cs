using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ClassSurvey.Domain.Entities
{
	public class Student : BaseEntity
	{
		[Required]
		[StringLength(256)]
		public string StudentCode { get; set; }

		[Required]
		[StringLength(256)]
		public string StudentName { get; set; }

		[StringLength(256)]
		public string Email { get; set; }

		[StringLength(100)]
		public string ClassByGrade { get; set; }
			
		[Required]
		[StringLength(50)]
		public string Username { get; set; }

		[Required]
		[StringLength(1000)]
		public string Password { get; set; }

		public virtual ICollection<StudentClass> StudentClasses { get; set; }

	}
}