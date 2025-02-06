using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;

namespace Veb_projekat.Models
{
    public class Patient
    {
        public string Username { get;  set; }
        public string JMBG { get;  set; }
        public string Password { get;  set; }
        public string FirstName { get;   set; }
        public string LastName { get; set; }
        public string DateOfBirth { get; set; }
        public string Email { get;   set; }
        public List<string> ScheduledAppointments { get; set; }

        public Patient(string username, string jmbg, string password, string firstName, string lastName, string dateOfBirth, string email)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentException("Username cannot be empty.");
            }

            if (jmbg.Length != 13 || !Regex.IsMatch(jmbg, @"^\d{13}$"))
            {
                throw new ArgumentException("JMBG must be exactly 13 numeric characters.");
            }

            DateTime dob;
            if (!DateTime.TryParseExact(dateOfBirth, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dob))
            {
                throw new ArgumentException("Date of birth must be in the format dd/MM/yyyy.");
            }

            if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                throw new ArgumentException("Invalid email format.");
            }

            Username = username;
            JMBG = jmbg;
            Password = password;
            FirstName = firstName;
            LastName = lastName;
            DateOfBirth = dateOfBirth;
            Email = email;
            ScheduledAppointments = new List<string>();
        }

        public Patient() {
            ScheduledAppointments = new List<string>();
        }

        public void ScheduleAppointment(string appointment)
        {
            ScheduledAppointments.Add(appointment);
        }

        public void CancelAppointment(string appointment)
        {
            if (ScheduledAppointments.Contains(appointment))
            {
                ScheduledAppointments.Remove(appointment);
            }
            else
            {
                throw new ArgumentException("Appointment not found in the list of scheduled appointments.");
            }
        }

        public override string ToString()
        {
            return $"Username: {Username}\n" +
                   $"JMBG: {JMBG}\n" +
                   $"First Name: {FirstName}\n" +
                   $"Last Name: {LastName}\n" +
                   $"Date of Birth: {DateOfBirth}\n" +
                   $"Email: {Email}\n" +
                   $"Scheduled Appointments: {string.Join(", ", ScheduledAppointments)}";
        }
    }
}
