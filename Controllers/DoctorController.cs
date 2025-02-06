using System;
using System.Linq;
using System.Web.Mvc;
using Veb_projekat.Models;
using Veb_projekat.Helpers;
using System.Collections.Generic;

namespace Veb_projekat.Controllers
{
    public class DoctorController : Controller
    {
        private const string AppointmentFilePath = "C:\\Users\\HP\\Desktop\\Veb-projekat\\App_Data\\Appointments.xml";
        private const string PatientFilePath = "~/App_Data/Patients.xml";
        private const string TherapiesFilePath = "~/App_Data/Therapies.xml";
        private const string AppointmentsFilePath = "~/App_Data/Appointments.xml";


        public ActionResult Index()
        {
            var doctorUsername = Session["Username"] as string; 
            if (string.IsNullOrEmpty(doctorUsername))
            {
                return new HttpStatusCodeResult(401, "Korisnik nije prijavljen.");
            }

            var patients = XMLHelper.UcitajPacijente(Server.MapPath("~/App_Data/Patients.xml"));
            var appointments = XMLHelper.UcitajTermine(Server.MapPath("~/App_Data/Appointments.xml"), patients);
            if (appointments == null)
            {
                return new HttpStatusCodeResult(500, "Greška u učitavanju termina.");
            }

          
            var doctorAppointments = appointments.Where(a => a.Doctor != null && a.Doctor.Username.Equals(doctorUsername, StringComparison.OrdinalIgnoreCase)).ToList();
            return View(doctorAppointments);
        }

        // GET: Doctor/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Doctor/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Appointment appointment)
        {
            var doctorUsername = Session["Username"] as string;
            if (string.IsNullOrEmpty(doctorUsername))
            {
                return new HttpStatusCodeResult(401, "Korisnik nije prijavljen.");
            }

            var doctor = XMLHelper.UcitajDoktoraPoUsername(doctorUsername);
            if (doctor == null)
            {
                return new HttpStatusCodeResult(500, "Greška u učitavanju podataka o doktoru.");
            }

            try
            {
                var appointments = XMLHelper.UcitajTermine(Server.MapPath("~/App_Data/Appointments.xml")) ?? new List<Appointment>();

                appointment.Id = appointments.Any() ? appointments.Max(a => a.Id) + 1 : 1;

                appointment.Doctor = doctor;
                appointment.Status = Appointment.AppointmentStatus.Free;

                appointment.TherapyDescription = string.Empty;

                appointments.Add(appointment);

                XMLHelper.SacuvajTermine(appointments, Server.MapPath("~/App_Data/Appointments.xml"));

                return RedirectToAction("Index");
            }
            catch
            {
                return View(appointment);
            }
        }




        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult Details(int id)
        {
            var patients = XMLHelper.UcitajPacijente(Server.MapPath("~/App_Data/Patients.xml"));
            var appointments = XMLHelper.UcitajTermine(Server.MapPath("~/App_Data/Appointments.xml"), patients);

            var appointment = appointments.FirstOrDefault(a => a.Id == id);

            if (appointment == null || appointment.Patient == null)
            {
                return HttpNotFound("Termin ili pacijent nije pronađen.");
            }



            
            var doctorUsername = Session["Username"] as string;
            

            return View(appointment);
        }


        
        public ActionResult CreateTherapy(int appointmentId)
        {
            var doctorUsername = Session["Username"] as string;
            if (string.IsNullOrEmpty(doctorUsername))
            {
                return new HttpStatusCodeResult(401, "Korisnik nije prijavljen.");
            }

            var patients = XMLHelper.UcitajPacijente(Server.MapPath("~/App_Data/Patients.xml"));
            var appointments = XMLHelper.UcitajTermine(Server.MapPath("~/App_Data/Appointments.xml"), patients);

            if (appointments == null || patients == null)
            {
                return new HttpStatusCodeResult(500, "Greška u učitavanju podataka.");
            }

            var appointment = appointments.FirstOrDefault(a => a.Id == appointmentId &&
                                                               a.Doctor != null && a.Doctor.Username == doctorUsername);

            if (appointment == null)
            {
                return HttpNotFound("Termin nije pronađen ili ne pripada trenutnom doktoru.");
            }

            return View(appointment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateTherapy(int appointmentId, string therapyDescription)
        {
            var patients = XMLHelper.UcitajPacijente(Server.MapPath("~/App_Data/Patients.xml"));
            var appointments = XMLHelper.UcitajTermine(Server.MapPath("~/App_Data/Appointments.xml"), patients);

            if (appointments == null || patients == null)
            {
                return new HttpStatusCodeResult(500, "Greška u učitavanju podataka.");
            }

            var appointment = appointments.FirstOrDefault(a => a.Id == appointmentId);

            if (appointment == null)
            {
                return HttpNotFound("Termin nije pronađen.");
            }

            appointment.TherapyDescription = therapyDescription;

            XMLHelper.SacuvajTermine(appointments, Server.MapPath("~/App_Data/Appointments.xml"));

            return RedirectToAction("Index");
        }

        
        public ActionResult CreateTherapyForPatient(string patientId)
        {
            var doctorUsername = Session["Username"] as string;
            if (string.IsNullOrEmpty(doctorUsername))
            {
                return new HttpStatusCodeResult(401, "Korisnik nije prijavljen.");
            }

            var patients = XMLHelper.UcitajPacijente(Server.MapPath("~/App_Data/Patients.xml"));
            if (patients == null)
            {
                return new HttpStatusCodeResult(500, "Greška u učitavanju podataka o pacijentima.");
            }

            var patient = patients.FirstOrDefault(p => p.Username == patientId);
            if (patient == null)
            {
                return HttpNotFound("Pacijent nije pronađen.");
            }

            return View(patient);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateTherapyForPatient(string patientId, string therapyDescription)
        {
            var patients = XMLHelper.UcitajPacijente(Server.MapPath("~/App_Data/Patients.xml"));
            var appointments = XMLHelper.UcitajTermine(Server.MapPath("~/App_Data/Appointments.xml"), patients);

            if (appointments == null || patients == null)
            {
                return new HttpStatusCodeResult(500, "Greška u učitavanju podataka.");
            }

            
            var scheduledAppointments = appointments.Where(a => a.Patient != null &&
                                                                a.Patient.Username == patientId &&
                                                                a.Status == Appointment.AppointmentStatus.Scheduled).ToList();

            if (!scheduledAppointments.Any())
            {
                return HttpNotFound("Nijedan zakazan termin nije pronađen za datog pacijenta.");
            }

           
            foreach (var appointment in scheduledAppointments)
            {
                appointment.TherapyDescription = therapyDescription;
            }

            XMLHelper.SacuvajTermine(appointments, Server.MapPath("~/App_Data/Appointments.xml"));

            return RedirectToAction("Index");
        }


        public ActionResult PrescribeTherapy()
        {
            var doctorUsername = Session["Username"] as string;
            if (string.IsNullOrEmpty(doctorUsername))
            {
                return new HttpStatusCodeResult(401, "Korisnik nije prijavljen.");
            }

            var patients = XMLHelper.UcitajPacijente(Server.MapPath("~/App_Data/Patients.xml"));
            var appointments = XMLHelper.UcitajTermine(Server.MapPath("~/App_Data/Appointments.xml"), patients);
            var therapies = XMLHelper.UcitajTerapije(Server.MapPath("~/App_Data/Therapies.xml"));

            if (appointments == null || therapies == null || patients == null)
            {
                return new HttpStatusCodeResult(500, "Greška u učitavanju podataka.");
            }

            var pastPatients = appointments
                .Where(a => a.Doctor != null && a.Doctor.Username.Equals(doctorUsername, StringComparison.OrdinalIgnoreCase) && a.Status == Appointment.AppointmentStatus.Scheduled)
                .Select(a => a.Patient)
                .Distinct()
                .ToList();

            foreach (var patient in pastPatients)
            {
                var patientAppointments = appointments
                    .Where(a => a.Patient != null && a.Patient.Username == patient.Username)
                    .ToList();

                foreach (var appointment in patientAppointments)
                {
                    if (!string.IsNullOrWhiteSpace(appointment.TherapyDescription))
                    {
                        if (!therapies.Any(t => t.Description == appointment.TherapyDescription && t.PatientUsername == patient.Username))
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
                                    PatientUsername = patient.Username
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
            }

            // Save updated therapies
            XMLHelper.SacuvajTerapije(therapies, Server.MapPath("~/App_Data/Therapies.xml"));

            return View(pastPatients);
        }


        public ActionResult PatientTherapies(string jmbg)
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


    }
}
