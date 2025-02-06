using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Veb_projekat.Models;
using Veb_projekat.Helpers;
using static Veb_projekat.Models.Appointment;

namespace Veb_projekat.Controllers
{
    public class PatientController : Controller
    {
        private const string PatientFilePath = "~/App_Data/Patients.xml";
        private const string AppointmentsFilePath = "~/App_Data/Appointments.xml";
        private const string TherapiesFilePath = "~/App_Data/Therapies.xml";

        public ActionResult Index()
        {
            var username = Session["Username"] as string;
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Home");
            }

            try
            {
                var patients = XMLHelper.UcitajPacijente(Server.MapPath(PatientFilePath));
                var appointments = XMLHelper.UcitajTermine(Server.MapPath(AppointmentsFilePath), patients);
                var therapies = XMLHelper.UcitajTerapije(Server.MapPath(TherapiesFilePath));

                var patient = patients.FirstOrDefault(p => p.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
                if (patient == null)
                {
                    return HttpNotFound("Pacijent nije pronađen.");
                }

                var patientAppointments = appointments
                    .Where(a => a.Patient != null && a.Patient.Username.Equals(username, StringComparison.OrdinalIgnoreCase))
                    .ToList();

               
                foreach (var appointment in patientAppointments)
                {
                    if (!string.IsNullOrWhiteSpace(appointment.TherapyDescription))
                    {
                        
                        if (!therapies.Any(t => t.Description == appointment.TherapyDescription && t.PatientUsername == username))
                        {
                            
                            DateTime scheduledDate;
                            if (DateTime.TryParse(appointment.DateTimeScheduled, out scheduledDate))
                            {
                                
                                var newTherapy = new Therapy
                                {
                                    Id = therapies.Count > 0 ? therapies.Max(t => t.Id) + 1 : 1,
                                    Description = appointment.TherapyDescription,
                                    StartDate = scheduledDate,
                                    EndDate = scheduledDate.AddDays(7), 
                                    PatientUsername = username
                                };
                                therapies.Add(newTherapy);
                            }
                            else
                            {
                                
                                throw new Exception("Invalid DateTime format in Appointment.DateTimeScheduled");
                            }
                        }
                    }
                }

                
                XMLHelper.SacuvajTerapije(therapies, Server.MapPath(TherapiesFilePath));

                var patientTherapies = therapies
                    .Where(t => t.PatientUsername.Equals(username, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                var model = new PatientDetailsViewModel
                {
                    Patient = patient,
                    Therapies = patientTherapies,
                    Appointments = patientAppointments
                };

                return View(model);
            }
            catch (Exception ex)
            {
                
                return new HttpStatusCodeResult(500, "Internal Server Error: " + ex.Message);
            }
        }

        public ActionResult Details(string jmbg)
        {
            try
            {
                var patients = XMLHelper.UcitajPacijente(Server.MapPath(PatientFilePath));
                var appointments = XMLHelper.UcitajTermine(Server.MapPath(AppointmentsFilePath), patients);
                var patient = patients.FirstOrDefault(p => p.JMBG == jmbg);

                if (patient == null)
                {
                    return HttpNotFound("Pacijent nije pronađen.");
                }

                var patientAppointments = appointments
                    .Where(a => a.Patient != null && a.Patient.JMBG == jmbg)
                    .ToList();

                var therapies = XMLHelper.UcitajTerapije(Server.MapPath(TherapiesFilePath))
                                          .Where(t => t.PatientUsername == patient.Username)
                                          .ToList();

                var model = new PatientDetailsViewModel
                {
                    Patient = patient,
                    Therapies = therapies,
                    Appointments = patientAppointments
                };

                return View(model);
            }
            catch (Exception ex)
            {
                
                return new HttpStatusCodeResult(500, "Internal Server Error: " + ex.Message);
            }
        }

        public ActionResult Therapies(string jmbg)
        {
            try
            {
                var patients = XMLHelper.UcitajPacijente(Server.MapPath(PatientFilePath));
                var appointments = XMLHelper.UcitajTermine(Server.MapPath(AppointmentsFilePath), patients);
                var patient = patients.FirstOrDefault(p => p.JMBG == jmbg);

                if (patient == null)
                {
                    return HttpNotFound("Pacijent nije pronađen.");
                }

                var patientAppointments = appointments
                    .Where(a => a.Patient != null && a.Patient.JMBG == jmbg)
                    .ToList();

                var therapies = XMLHelper.UcitajTerapije(Server.MapPath(TherapiesFilePath))
                                          .Where(t => t.PatientUsername == patient.Username)
                                          .ToList();

                return View(therapies);
            }
            catch (Exception ex)
            {
                
                return new HttpStatusCodeResult(500, "Internal Server Error: " + ex.Message);
            }
        }

        public ActionResult ScheduleAppointment()
        {
            try
            {
                var patients = XMLHelper.UcitajPacijente(Server.MapPath(PatientFilePath));
                var appointments = XMLHelper.UcitajTermine(Server.MapPath(AppointmentsFilePath), patients);

                
                var freeAppointments = appointments
                    .Where(a => a.Status == AppointmentStatus.Free) 
                    .ToList();

                
                var model = freeAppointments.Select(a => new ScheduleAppointmentViewModel
                {
                    AppointmentId = a.Id,
                    DoctorName = a.Doctor.FirstName + " " + a.Doctor.LastName,
                    DateTimeScheduled = a.DateTimeScheduled
                }).ToList();

                return View(model);
            }
            catch (Exception ex)
            {
                
                return new HttpStatusCodeResult(500, "Internal Server Error: " + ex.Message);
            }
        }


        [HttpPost]
        public ActionResult ScheduleAppointment(int appointmentId)
        {
            if (appointmentId <= 0)
            {
                
                return new HttpStatusCodeResult(400, "Invalid appointment ID.");
            }

            try
            {
                var username = Session["Username"] as string;
                if (string.IsNullOrEmpty(username))
                {
                    return RedirectToAction("Login", "Home");
                }

                var patients = XMLHelper.UcitajPacijente(Server.MapPath(PatientFilePath));
                var appointments = XMLHelper.UcitajTermine(Server.MapPath(AppointmentsFilePath), patients);
                var doctors = XMLHelper.UcitajLekare(Server.MapPath("~/App_Data/Doctors.xml"));

                var patient = patients.FirstOrDefault(p => p.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
                var appointment = appointments.FirstOrDefault(a => a.Id == appointmentId && a.Status == AppointmentStatus.Free);

                if (patient == null || appointment == null)
                {
                    return HttpNotFound("Pacijent ili termin nisu pronađeni.");
                }

                
                appointment.Status = AppointmentStatus.Scheduled;
                appointment.Patient = patient;

                
                var doctor = doctors.FirstOrDefault(d => d.Username.Equals(appointment.Doctor.Username, StringComparison.OrdinalIgnoreCase));
                if (doctor != null)
                {
                    if (doctor.ScheduledAppointments == null)
                    {
                        doctor.ScheduledAppointments = new List<string>();
                    }
                    doctor.ScheduledAppointments.Add(appointment.DateTimeScheduled);
                }

                
                if (patient.ScheduledAppointments == null)
                {
                    patient.ScheduledAppointments = new List<string>();
                }
                patient.ScheduledAppointments.Add(appointment.DateTimeScheduled);

                
                XMLHelper.SacuvajTermine(appointments, Server.MapPath(AppointmentsFilePath));
                XMLHelper.SacuvajPacijente(patients, Server.MapPath(PatientFilePath));
                XMLHelper.SacuvajLekare(doctors, Server.MapPath("~/App_Data/Doctors.xml"));

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(500, "Internal Server Error: " + ex.Message);
            }
        }




    }
}
