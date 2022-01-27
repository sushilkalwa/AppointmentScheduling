using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AppointmentScheduling.Models.ViewModels
{
	public class RegisterViewModel
	{
		[Required]
		public string Name { get; set; }

		[Required]
		[EmailAddress]
		public string Email { get; set; }

		[Required]
		[DataType(DataType.Password)]
		[StringLength(10,ErrorMessage ="The {0} must be at least {2} character long.",MinimumLength =6)]
		public string Password { get; set; }

		[Display(Name ="Confirm Password")]
		[DataType(DataType.Password)]
		[Compare("Password",ErrorMessage ="The Password and Confirmation Password do not match.")]
		public string ConfirmPassword { get; set; }

		[Required]
		[Display(Name ="Role Name")]
		public string RoleName { get; set; }
	}
}
