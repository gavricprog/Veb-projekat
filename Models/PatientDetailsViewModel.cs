using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Veb_projekat.Models
{
    public class PatientDetailsViewModel
    {
        public Patient Patient { get; set; }
        public List<Therapy> Therapies { get; set; }
        public List<Appointment> Appointments { get; set; }
    }
}