using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;

namespace Veb_projekat.Models
{
    public class Administrator
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get;  set; }
        public string DateOfBirth { get; set; }

        public Administrator(string username, string password, string firstName, string lastName, string dateOfBirth)
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

            Username = username;
            Password = password;
            FirstName = firstName;
            LastName = lastName;
            DateOfBirth = dateOfBirth;
        }

        public Administrator() { }

        public override string ToString()
        {
            return $"Username: {Username}\n" +
                   $"First Name: {FirstName}\n" +
                   $"Last Name: {LastName}\n" +
                   $"Date of Birth: {DateOfBirth}";
        }
    }
}
