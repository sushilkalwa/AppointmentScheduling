using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppointmentScheduling.Models.ViewModels
{
	public class CommonResponse<T>
	{
		public int Status { get; set; }
		public string message { get; set; }
		public T dataenum { get; set; }
	}
}
