using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ClassSurvey.Domain.Entities
{
	public class SurveyQuestion : BaseEntity
	{
		[Required]
		[StringLength(2048)]
		public string Content { get; set; }
		
		//1 bo cac survey co cac cau hoi nhu trong bo surveyquestion
		public ICollection<Survey> Surveys { get; set; }
	}
}