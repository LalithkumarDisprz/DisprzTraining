using System.ComponentModel.DataAnnotations;

namespace DisprzTraining.Models
{
    public class AppointmentsBase 
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please select a date")]
        public DateTime Date { get; set; }
        [Required]
        public string Title { get; set; }
        public string? Description { get; set; }
        public string Type { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Please select start-time")]
        public DateTime StartTime { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please select end-time")]
        public DateTime EndTime { get; set; }
       
       public Attachment? AppointmentAttachment { get; set; }
    }
}