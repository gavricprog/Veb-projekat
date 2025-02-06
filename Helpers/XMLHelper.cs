using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Veb_projekat.Models;

namespace Veb_projekat.Helpers
{
    public class XMLHelper
    {
        public static List<Administrator> UcitajAdministratore(string filePath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<Administrator>), new XmlRootAttribute("Administrators"));
            using (StreamReader reader = new StreamReader(filePath))
            {
                return (List<Administrator>)serializer.Deserialize(reader);
            }
        }

        public static List<Doctor> UcitajLekare(string filePath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<Doctor>), new XmlRootAttribute("Doctors"));
            using (StreamReader reader = new StreamReader(filePath))
            {
                return (List<Doctor>)serializer.Deserialize(reader);
            }
        }

        public static List<Patient> UcitajPacijente(string filePath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<Patient>), new XmlRootAttribute("Patients"));
            using (StreamReader reader = new StreamReader(filePath))
            {
                return (List<Patient>)serializer.Deserialize(reader);
            }
        }

        public static List<Appointment> UcitajTermine(string filePath, List<Patient> patients)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<Appointment>), new XmlRootAttribute("Appointments"));
                using (StreamReader reader = new StreamReader(filePath))
                {
                    var appointments = (List<Appointment>)serializer.Deserialize(reader);

                    // Dodeljivanje pacijenata terminima
                    foreach (var appointment in appointments)
                    {
                        if (appointment.Status == Appointment.AppointmentStatus.Scheduled)
                        {
                            var patient = patients.FirstOrDefault(p => p.ScheduledAppointments.Contains(appointment.DateTimeScheduled));
                            if (patient != null)
                            {
                                appointment.Patient = patient;
                            }
                        }
                    }

                    foreach (var appointment in appointments)
                    {
                        if (appointment.Patient == null)
                        {
                            Console.WriteLine($"Pacijent nije učitan za termin sa ID-om: {appointment.Id}");
                        }
                        else
                        {
                            Console.WriteLine($"Pacijent {appointment.Patient.FirstName} {appointment.Patient.LastName} je učitan za termin sa ID-om: {appointment.Id}");
                        }
                    }

                    return appointments;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Greška prilikom deserializacije termina: {ex.Message}");
                return new List<Appointment>();
            }
        }


        public static List<Appointment> UcitajTermine(string filePath)
        {
            try
            {
                List<Patient> pacijenti = UcitajPacijente("C:\\Users\\HP\\Desktop\\Veb-projekat - Radna Verzija\\App_Data\\Patients.xml");

                XmlSerializer serializer = new XmlSerializer(typeof(List<Appointment>), new XmlRootAttribute("Appointments"));
                using (StreamReader reader = new StreamReader(filePath))
                {
                    var appointments = (List<Appointment>)serializer.Deserialize(reader);

                    foreach (var appointment in appointments)
                    {
                        if (appointment.Status == Appointment.AppointmentStatus.Scheduled)
                        {
                            foreach (var patient in pacijenti)
                            {
                                if (patient.ScheduledAppointments.Contains(appointment.DateTimeScheduled))
                                {
                                    appointment.Patient = patient;
                                    break; 
                                }
                            }
                        }

                        if (appointment.Patient == null)
                        {
                            Console.WriteLine($"Pacijent nije učitan za termin sa ID-om: {appointment.Id}");
                        }
                        else
                        {
                            Console.WriteLine($"Pacijent {appointment.Patient.FirstName} {appointment.Patient.LastName} je učitan za termin sa ID-om: {appointment.Id}");
                        }
                    }
                    return appointments;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Greška prilikom deserializacije: {ex.Message}");
                return null;
            }
        }

        public static void SacuvajAdministratore(List<Administrator> administratori, string filePath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<Administrator>), new XmlRootAttribute("Administrators"));
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                serializer.Serialize(writer, administratori);
            }
        }

        public static void SacuvajLekare(List<Doctor> lekari, string filePath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<Doctor>), new XmlRootAttribute("Doctors"));
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                serializer.Serialize(writer, lekari);
            }
        }

        public static void SacuvajPacijente(List<Patient> pacijenti, string filePath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<Patient>), new XmlRootAttribute("Patients"));
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                serializer.Serialize(writer, pacijenti);
            }
        }

        public static void SacuvajTermine(List<Appointment> termini, string filePath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<Appointment>), new XmlRootAttribute("Appointments"));
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                serializer.Serialize(writer, termini);
            }
        }

        public static Doctor UcitajDoktoraPoUsername(string username)
        {
            var doctors = UcitajLekare("C:\\Users\\HP\\Desktop\\Veb-projekat\\App_Data\\Doctors.xml"); 
            return doctors.FirstOrDefault(d => d.Username.Equals(username, System.StringComparison.OrdinalIgnoreCase));
        }

        public static Patient UcitajPacijentaPoUsername(string username)
        {
            var patients = UcitajPacijente("C:\\Users\\HP\\Desktop\\Veb-projekat\\App_Data\\Patients.xml"); 
            return patients.FirstOrDefault(p => p.Username.Equals(username, System.StringComparison.OrdinalIgnoreCase));
        }

        public static List<Therapy> UcitajTerapije(string filePath)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<Therapy>), new XmlRootAttribute("Therapies"));
                using (StreamReader reader = new StreamReader(filePath))
                {
                    return (List<Therapy>)serializer.Deserialize(reader);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Greška prilikom deserializacije terapija: {ex.Message}");
                return new List<Therapy>();
            }
        }

        public static void SacuvajTerapije(List<Therapy> terapije, string filePath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<Therapy>), new XmlRootAttribute("Therapies"));
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                serializer.Serialize(writer, terapije);
            }
        }

    }
}
