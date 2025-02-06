using System;

namespace Veb_projekat.Models
{
    public class Appointment
    {
        public enum AppointmentStatus
        {
            Free,
            Scheduled
        }

        public int Id { get; set; }

        public Patient Patient { get; set; }

        public Doctor Doctor { get; set; }
        public string DoctorUsername
        {
            get { return Doctor?.Username; }
            set
            {
                if (Doctor == null)
                {
                    Doctor = new Doctor { Username = value };
                }
                else
                {
                    Doctor.Username = value;
                }
            }
        }
        public AppointmentStatus Status { get; set; }
        public string DateTimeScheduled { get; set; }
        public string TherapyDescription { get; set; }

        public Appointment(Doctor doctor, Patient patient, AppointmentStatus status, string dateTimeScheduled, string therapyDescription)
        {
            DateTime dt;
            if (!DateTime.TryParseExact(dateTimeScheduled, "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out dt))
            {
                throw new ArgumentException("Date and time must be in the format dd/MM/yyyy HH:mm.");
            }

            Doctor = doctor;
            Patient = patient; 
            Status = status;
            DateTimeScheduled = dateTimeScheduled;
            TherapyDescription = therapyDescription;
        }


        public Appointment() { }

        public void UpdateStatus(AppointmentStatus newStatus)
        {
            Status = newStatus;
        }

        public override string ToString()
        {
            return $"Doctor: {Doctor.FirstName} {Doctor.LastName}\n" +
                   $"Status: {Status}\n" +
                   $"Date and Time: {DateTimeScheduled}\n" +
                   $"Therapy Description: {TherapyDescription}";
        }
    }
}
