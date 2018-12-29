using ClassSurvey.Domain.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ClassSurvey.Domain.Entities
{
	//Column between student and class, can get teacher here too
	public class StudentClass : BaseEntity
	{
		public int? StudentId { get; set; }
		public virtual Student Student { get; set; }

		public int? ClassId { get; set; }
		public virtual Class Class { get; set; }

		public int? TeacherId { get; set; }
		public virtual Teacher Teacher { get; set; }

		public virtual ICollection<Survey> Surveys { get; set; }

	}
}