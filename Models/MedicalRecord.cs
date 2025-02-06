using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Veb_projekat.Models
{
    public class MedicalRecord
    {
        public List<Appointment> Appointments { get;  set; }
        public Patient Patient { get;  set; }

        public MedicalRecord(Patient patient)
        {
            Patient = patient;
            Appointments = new List<Appointment>();
        }

        public MedicalRecord() { }

        public void AddAppointment(Appointment appointment)
        {
            Appointments.Add(appointment);
        }

        public void RemoveAppointment(Appointment appointment)
        {
            if (Appointments.Contains(appointment))
            {
                Appointments.Remove(appointment);
            }
            else
            {
                throw new ArgumentException("Appointment not found in the medical record.");
            }
        }

        public override string ToString()
        {
            return $"Patient: {Patient.FirstName} {Patient.LastName}\n" +
                   $"Number of Appointments: {Appointments.Count}\n" +
                   $"Appointments:\n" +
                   string.Join("\n", Appointments.Select(a => a.ToString()));
        }
    }
}
