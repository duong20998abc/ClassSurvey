using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ClassSurvey.Domain.Entities
{
	public class Admin : BaseEntity
	{
		[Required]
		[StringLength(256)]
		public string Name { get; set; }

		[Required]
		[StringLength(256)]
		public string Username { get; set; }

		[Required]
		[StringLength(256)]
		public string Password { get; set; }

		[StringLength(500)]
		public string Logo { get; set; }
	}
}