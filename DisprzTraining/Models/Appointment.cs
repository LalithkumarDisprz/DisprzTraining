using System.ComponentModel.DataAnnotations;

namespace DisprzTraining.Models
{
    public class Appointment : AppointmentsBase
    {
        [Required]
        public Guid Id { get; set; }
    }
}
