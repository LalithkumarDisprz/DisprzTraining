using DisprzTraining.Models;
using Microsoft.AspNetCore.Mvc;
using DisprzTraining.Business;
using System.ComponentModel.DataAnnotations;
using DisprzTraining.Data;
using DisprzTraining.CustomErrorCodes;
using DisprzTraining.Models.CustomCodeModel;
using Newtonsoft.Json;

namespace DisprzTraining.Controllers
{
    [Route("api/appointments")]
    [ApiController]
    public class AppointmentsController : Controller
    {
        private readonly IAppointmentsBL _appointmentsBL;

        public AppointmentsController(IAppointmentsBL appointmentsBL)
        {
            _appointmentsBL = appointmentsBL;
        }

        //design - GET /api/appointments
        //- POST /api/appointments
        //- DELETE /api/appointments
        // public static List<Appointment> list=new List<Appointment>();
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(bool))]
        [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(CustomCodes))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(CustomCodes))]
        public IActionResult CreateAppointment(AddNewAppointment data)
        {
            try
            {
                bool postCreated = _appointmentsBL.CreateNewAppointment(data);
                if (postCreated == true)
                {
                    return Created("~api/appointments", postCreated);
                }
                else
                {
                    return Conflict(JsonConvert.SerializeObject(CustomErrorCodeMessages.meetingIsAlreadyAssigned));
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("{date}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<Appointment>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<List<Appointment>> GetAppointmentsByDate(DateTime date)
        {

            return Ok(_appointmentsBL.GetAppointmentsForSelectedDate(date));
        }

        [HttpGet("range/{startDate}/{endDate}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<Appointment>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest,Type= typeof(CustomCodes))]
        public ActionResult<List<Appointment>> GetListByRange(DateTime startDate,DateTime endDate)
        {
            try
            {
                return Ok(_appointmentsBL.GetRangedList(startDate, endDate));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete("{id}/{date}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(CustomCodes))]

        public IActionResult Remove(Guid id,DateTime date)
        {
            bool idIsThere = _appointmentsBL.RemoveAppointment(id, date);
            if (idIsThere == true)
            {
                return NoContent();
            }
            else
            {
                return NotFound(JsonConvert.SerializeObject(CustomErrorCodeMessages.idIsInvalid));
            }
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(CustomCodes))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(CustomCodes))]
        public IActionResult Update([FromBody] UpdateAppointment data)
        {
            try
            {
                bool eventIsUpdated = _appointmentsBL.UpdateAppointment(data);
                if (eventIsUpdated == true)
                {
                    return Ok(eventIsUpdated);
                }
                else
                {
                    return Conflict(JsonConvert.SerializeObject(CustomErrorCodeMessages.meetingIsAlreadyAssigned));
                }
            }
            catch (ArgumentException e)
            {
                return NotFound(e.Message);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }
    }
}
