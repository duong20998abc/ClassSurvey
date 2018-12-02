using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ClassSurvey.Domain.Entities
{
	public class Survey : BaseEntity
	{
		// 1 bo cau hoi cho nhieu survey
		[ForeignKey("SurveyQuestion")]
		public int SurveyQuestionId { get; set; }
		public virtual SurveyQuestion SurveyQuestion { get; set; }

		[ForeignKey("StudentClass")]
		public int StudentClassId { get; set; }
		public virtual StudentClass StudentClass { get; set; }

		public int Result { get; set; }
	}
}