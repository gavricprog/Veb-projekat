using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Veb_projekat.Models
{
    public class ScheduleAppointmentViewModel
    {
        public int AppointmentId { get; set; }
        public string DoctorName { get; set; }
        public string DateTimeScheduled { get; set; }
    }
}