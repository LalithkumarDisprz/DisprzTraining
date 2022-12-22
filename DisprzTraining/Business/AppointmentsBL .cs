using DisprzTraining.Models;
using DisprzTraining.DataAccess;
using DisprzTraining.Data;

namespace DisprzTraining.Business
{
    public class AppointmentsBL:IAppointmentsBL
    {
        private readonly IAppointmentsDAL _appointmentsDAL;

        public AppointmentsBL(IAppointmentsDAL appointmentsDAL)
        {
            _appointmentsDAL = appointmentsDAL;
        }

        public async Task<bool> CreateNewAppointment(AddNewAppointment data)
        {
            
            bool NoConflict=await CheckConflict(data);
            if(NoConflict==true)
            {
                return (await _appointmentsDAL.CreateNewAppointments(data));
            }
            else
            {
                return NoConflict;
            }
            
        }

        public async Task<List<Appointment>> GetAllAddedAppointments()
        {
            return await _appointmentsDAL.GetAllAppointments();
        }
        public async Task<List<Appointment>> GetAppointments(string date)
        {
            return await _appointmentsDAL.GetAppointmentsByDate(date);
        }

        public async Task<bool> RemoveAppointments(Guid id)
        {
            return await _appointmentsDAL.RemoveAppointmentsById(id);
        }

        public async Task<bool> UpdateAppointments(Appointment data)
        {
            
            bool NoConflict=await CheckUpdateConflict(data);
            if(NoConflict==true)
            {
              return await _appointmentsDAL.UpdateAppointmentById(data);
            }
            else
              return NoConflict;
        }
        private async Task<bool> CheckConflict(AddNewAppointment data)
        {
            if(data.StartTime==data.EndTime||data.EndTime<data.StartTime)
            {
                return false;
            }
            bool isAnyList=DataStore.newList.Count!=0;
            if(isAnyList)
            {
                foreach(var item in DataStore.newList)
                {
                     if((data.StartTime<item.EndTime)&&(item.StartTime<data.EndTime))
                     {
                        return false;  
                     }
                }
             return true; 
            }
         return true;
        }

        private async Task<bool> CheckUpdateConflict(Appointment data)
        {
            if(data.StartTime==data.EndTime||data.EndTime<data.StartTime)
            {
                return false;
            }
            
            
            
            // if(isAnyList==true)
            // {
                foreach(var item in DataStore.newList)
                {
                    if(item.Id==data.Id)
                    {
                        continue;
                    }
                    if((data.StartTime<item.EndTime)&&(item.StartTime<data.EndTime))
                     {
                        return false;  
                     }   
                }
             return true; 
            // }
            // else
            // {
            //   return false;
            // }
        }
        
    }
}