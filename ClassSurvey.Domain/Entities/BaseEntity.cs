using ClassSurvey.Domain.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ClassSurvey.Domain.Entities
{
	//BaseEntity
	//All column share BaseEntity Components
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