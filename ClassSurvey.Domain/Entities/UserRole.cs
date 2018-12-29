using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClassSurvey.Domain.Entities
{
	//Roles of users (Entity)
	public class UserRole : BaseEntity
	{
		public string Position { get; set; }
		public string Area { get; set; }
		public string Controller { get; set; }
		public string Action { get; set; }
	}
}