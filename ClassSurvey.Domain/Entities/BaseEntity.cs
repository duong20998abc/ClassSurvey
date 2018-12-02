using ClassSurvey.Domain.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ClassSurvey.Domain.Entities
{
	public class BaseEntity
	{
		public BaseEntity()
		{
			IsDeleted = false;
			Status = CommonStatus.Active;
		}

		[Key]
		public int Id { get; set; }
		public CommonStatus Status { get; set; }
		public bool IsDeleted { get; set; }
	}
}