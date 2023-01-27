using DisprzTraining.Models;
using DisprzTraining.DataAccess;
using DisprzTraining.Data;
using System.Globalization;
using Newtonsoft.Json;
using DisprzTraining.CustomErrorCodes;
using System.Text.RegularExpressions;

namespace DisprzTraining.Business
{
    public class AppointmentsBL : IAppointmentsBL
    {
        private readonly IAppointmentsDAL _appointmentsDAL;

        public AppointmentsBL(IAppointmentsDAL appointmentsDAL)
        {
            _appointmentsDAL = appointmentsDAL;
        }
        public bool CreateNewAppointment(AddNewAppointment data)
        {
            bool base64Format = CheckBase64(data.AppointmentAttachment);
            if (base64Format)
            {
                CheckInputTime(data);
                return (_appointmentsDAL.CreateNewAppointments(data));
            }
            else
            {
                throw new Exception(JsonConvert.SerializeObject(CustomErrorCodeMessages.invalidAttachment));
            }
        }

        public List<Appointment> GetAppointmentsForSelectedDate(DateTime date)
        {

            return _appointmentsDAL.GetAppointmentsByDate(date);

        }

        public List<Appointment> GetRangedList(DateTime startDate,DateTime endDate)
        {
            DateTime startRange = startDate.Date;
            DateTime endRange = endDate.Date;
            if(endRange<startRange)
            {
                throw new Exception(JsonConvert.SerializeObject(CustomErrorCodeMessages.invalidRange));
            }
            else
            return _appointmentsDAL.GetRangedList(startRange,endRange);
        }

        public bool RemoveAppointment(Guid id, DateTime date)
        {
            return _appointmentsDAL.RemoveAppointmentById(id, date);
        }

        public bool UpdateAppointment(UpdateAppointment data)
        {
            DateTime convertedDate = data.Appointment.Date.Date;
            DateTime OldDate = (data.OldDate).Date;
            bool base64Format = CheckBase64(data.Appointment.AppointmentAttachment);
            AddNewAppointment dataToBeUpdated = new AddNewAppointment()
            {
                Date = data.Appointment.Date,
                Title = data.Appointment.Title,
                Description = data.Appointment.Description,
                Type = data.Appointment.Type,
                StartTime = data.Appointment.StartTime,
                EndTime = data.Appointment.EndTime,
                AppointmentAttachment = new Attachment()
                {
                    Content = data.Appointment.AppointmentAttachment.Content,
                    ContentName = data.Appointment.AppointmentAttachment.ContentName,
                    ContentType = data.Appointment.AppointmentAttachment.ContentType,
                }
            };
            if (base64Format)
            {
                CheckInputTime(dataToBeUpdated);
                if (_appointmentsDAL.CheckForId(data.Appointment.Id, OldDate))
                {
                    if (convertedDate == OldDate)
                    {
                        return (_appointmentsDAL.UpdateAppointmentById(data.Appointment));
                    }
                    else
                    {
                        var updatedAppointment = _appointmentsDAL.CreateNewAppointments(dataToBeUpdated);
                        if (updatedAppointment)
                        {
                            return _appointmentsDAL.RemoveAppointmentById(data.Appointment.Id, OldDate);
                        }
                        else
                        {
                            return updatedAppointment;
                        }
                    }
                }
                else
                {
                    throw new ArgumentException(JsonConvert.SerializeObject(CustomErrorCodeMessages.idIsInvalid));
                }
            }
            else
            {
                throw new Exception(JsonConvert.SerializeObject(CustomErrorCodeMessages.invalidAttachment));
            }
        }
        private void CheckInputTime(AddNewAppointment data)
        {
            if (data.StartTime == data.EndTime)
            {
                throw new Exception(JsonConvert.SerializeObject(CustomErrorCodeMessages.startAndEndTimeAreSame));
            }
            if (data.EndTime < data.StartTime)
            {
                throw new Exception(JsonConvert.SerializeObject(CustomErrorCodeMessages.startTimeGreaterThanEndTime));
            }
            int compare = DateTime.Compare(data.StartTime, DateTime.Now.AddMinutes(-1));
            if (compare == -1)
            {
                throw new Exception(JsonConvert.SerializeObject(CustomErrorCodeMessages.tryingToAddMeetingInPastDate));
            }
        }
        private bool CheckBase64(Attachment attachedData)
        {
            string base64 = attachedData.Content;
            if (String.IsNullOrWhiteSpace(base64))
            {
                return true;
            }
            else if (String.IsNullOrWhiteSpace(attachedData.ContentName) || String.IsNullOrWhiteSpace(attachedData.ContentType))
            {
                return false;
            }
            else if (base64.Contains(","))
            {
                var splittedString = base64.Split(new char[] { ',' }, StringSplitOptions.None);
                base64 = splittedString[1].Trim();
                if ((base64.Length % 4 == 0) && Regex.IsMatch(base64, @"^[a-zA-Z0-9\+/]*={0,3}$", RegexOptions.None))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if ((base64.Length % 4 == 0) && Regex.IsMatch(base64, @"^[a-zA-Z0-9\+/]*={0,3}$", RegexOptions.None))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}