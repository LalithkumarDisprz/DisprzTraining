using DisprzTraining.Models;

namespace DisprzTraining.Business
{
    public interface IAppointmentsBL
    {
        bool CreateNewAppointment(AddNewAppointment data);

        List<Appointment> GetAppointmentsForSelectedDate(DateTime date);

        List<Appointment> GetRangedList(DateTime startDate,DateTime endDate);
        bool RemoveAppointment(Guid id, DateTime date);

        bool UpdateAppointment(UpdateAppointment data);
    }
}