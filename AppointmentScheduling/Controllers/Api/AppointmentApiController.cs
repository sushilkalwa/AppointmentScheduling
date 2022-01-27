using AppointmentScheduling.Models.ViewModels;
using AppointmentScheduling.Services;
using AppointmentScheduling.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AppointmentScheduling.Controllers.Api
{
	[Route("api/Appointment")]
	[ApiController]
	public class AppointmentApiController : Controller
	{
		private readonly IAppointmentService _appointmentService;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly string loginUserId;
		private readonly string role;

		
		public AppointmentApiController(IAppointmentService appointmentService, IHttpContextAccessor httpContextAccessor)
		{
			_appointmentService = appointmentService;
			_httpContextAccessor = httpContextAccessor;
			loginUserId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
			role = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role);
		}
		[HttpPost]
		[Route("SaveCalenderData")]
		public IActionResult SaveCalenderData(AppointmentVM data)
		{
			CommonResponse<int> commonResponse = new CommonResponse<int>();
			try {
				commonResponse.Status = _appointmentService.AddUpdate(data).Result;
				if (commonResponse.Status == 1)
					commonResponse.message = Helper.appointmentUpdated;
				if(commonResponse.Status == 2)
					commonResponse.message = Helper.appointmentAdded;
			}
			catch(Exception e) {
				commonResponse.message = e.Message;
				commonResponse.Status = Helper.failure_code;
			}
			return Ok(commonResponse);
		}

		[HttpGet]
		[Route("GetCalenderData")]
		public IActionResult GetCalenderData(string doctorId)
		{
			CommonResponse<List<AppointmentVM>> commonResponse = new CommonResponse<List<AppointmentVM>>();
			try
			{
				if (role == Helper.Patient)
				{
					commonResponse.dataenum = _appointmentService.PatientEventsById(loginUserId);
					commonResponse.Status = Helper.success_code;
				}
				else if (role == Helper.Doctor)
				{
					commonResponse.dataenum = _appointmentService.DoctorsEventsById(loginUserId);
					commonResponse.Status = Helper.success_code;
				}
				else
				{
					commonResponse.dataenum = _appointmentService.DoctorsEventsById(doctorId);
					commonResponse.Status = Helper.success_code;
				}
			}
			catch(Exception e)
			{
				commonResponse.message = e.Message;
				commonResponse.Status = Helper.failure_code;
			}
			return Ok(commonResponse);
		}

		[HttpGet]
		[Route("GetCalendarDataById/{id}")]
		public IActionResult GetCalendarDataById(int id)
		{
			CommonResponse<AppointmentVM> commonResponse = new CommonResponse<AppointmentVM>();
			try
			{

				commonResponse.dataenum = _appointmentService.GetById(id);
				commonResponse.Status = Helper.success_code;

			}
			catch (Exception e)
			{
				commonResponse.message = e.Message;
				commonResponse.Status = Helper.failure_code;
			}
			return Ok(commonResponse);
		}

		[HttpGet]
		[Route("DeleteAppointment/{id}")]
		public async Task<IActionResult> DeleteAppointment(int id)
		{
			CommonResponse<int> commonResponse = new CommonResponse<int>();
			try
			{
				commonResponse.Status = await _appointmentService.Delete(id);
				commonResponse.message = commonResponse.Status == 1 ? Helper.appointmentDeleted : Helper.somethingWentWrong;

			}
			catch (Exception e)
			{
				commonResponse.message = e.Message;
				commonResponse.Status = Helper.failure_code;
			}
			return Ok(commonResponse);
		}

		[HttpGet]
		[Route("ConfirmEvent/{id}")]
		public IActionResult ConfirmEvent(int id)
		{
			CommonResponse<int> commonResponse = new CommonResponse<int>();
			try
			{
				var result =  _appointmentService.ConfirmEvent(id).Result;
				if (result > 0)
				{
					commonResponse.Status = Helper.success_code;
					commonResponse.message = Helper.meetingConfirm;
				}
				else
				{
					commonResponse.Status = Helper.success_code;
					commonResponse.message = Helper.meetingConfirmError;
				}

			}
			catch (Exception e)
			{
				commonResponse.message = e.Message;
				commonResponse.Status = Helper.failure_code;
			}
			return Ok(commonResponse);
		}
	}
}
