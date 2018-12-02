using ClassSurvey.Domain.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ClassSurvey.Domain.Entities
{
	public class StudentClass : BaseEntity
	{
		[ForeignKey("Student")]
		public int StudentId { get; set; }
		public virtual Student Student { get; set; }

		[ForeignKey("Class")]
		public int ClassId { get; set; }
		public virtual Class Class { get; set; }

		[ForeignKey("Teacher")]
		public int TeacherId { get; set; }
		public virtual Teacher Teacher { get; set; }

		public ICollection<Survey> Surveys { get; set; }

	}
}