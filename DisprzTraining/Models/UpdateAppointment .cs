using System.ComponentModel.DataAnnotations;

namespace DisprzTraining.Models
{
    public class UpdateAppointment
    {
        [Required]
        public Appointment Appointment { get; set; }
        [Required]
        public DateTime OldDate { get; set; }
    }
}