using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ClassSurvey.Domain.Entities
{
	public class User : BaseEntity
	{
		[StringLength(256)]
		[Required]
		public string Username { get; set; }

		[Required]
		public string Password { get; set; }

		[Required]
		public string Position { get; set; }

		public int? StudentId { get; set; }
		public int? TeacherId { get; set; }
		public int? AdminId { get; set; }
	}
}