using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;

namespace Veb_projekat.Models
{
    public class Doctor
    {
        public string Username { get;   set; }
        public string Password { get;   set; }
        public string FirstName { get;  set; }
        public string LastName { get;  set; }
        public string DateOfBirth { get;  set; }
        public string Email { get;   set; }
        public List<string> ScheduledAppointments { get;   set; }
        public List<string> AvailableAppointments { get;   set; }

        public Doctor(string username, string password, string firstName, string lastName, string dateOfBirth, string email)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentException("Username cannot be empty.");
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
            Password = password;
            FirstName = firstName;
            LastName = lastName;
            DateOfBirth = dateOfBirth;
            Email = email;
            ScheduledAppointments = new List<string>();
            AvailableAppointments = new List<string>();
        }

        public Doctor() { }

        public void ScheduleAppointment(string appointment)
        {
            if (AvailableAppointments.Contains(appointment))
            {
                AvailableAppointments.Remove(appointment);
                ScheduledAppointments.Add(appointment);
            }
            else
            {
                throw new ArgumentException("Appointment is not available.");
            }
        }

        public void CancelAppointment(string appointment)
        {
            if (ScheduledAppointments.Contains(appointment))
            {
                ScheduledAppointments.Remove(appointment);
                AvailableAppointments.Add(appointment);
            }
            else
            {
                throw new ArgumentException("Appointment not found in the list of scheduled appointments.");
            }
        }

        public override string ToString()
        {
            return $"Username: {Username}\n" +
                   $"First Name: {FirstName}\n" +
                   $"Last Name: {LastName}\n" +
                   $"Date of Birth: {DateOfBirth}\n" +
                   $"Email: {Email}\n" +
                   $"Scheduled Appointments: {string.Join(", ", ScheduledAppointments)}\n" +
                   $"Available Appointments: {string.Join(", ", AvailableAppointments)}";
        }
    }
}
