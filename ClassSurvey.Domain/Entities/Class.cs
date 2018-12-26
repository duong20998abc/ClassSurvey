using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ClassSurvey.Domain.Entities
{
	public class Class : BaseEntity
	{
		[Required]
		[StringLength(256)]
		public string ClassName { get; set; }

		[Required]
		[StringLength(256)]
		public string ClassCode { get; set; }

		//[Required]
		//[ForeignKey("Teacher")]
		//public int TeacherId { get; set; }
		//public virtual Teacher Teacher { get; set; }

		public int Semester { get; set; }
		public int NumberOfDegrees { get; set; }

		public virtual ICollection<StudentClass> StudentClasses { get; set; }
	}
}