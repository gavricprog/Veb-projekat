using System;
using System.Linq;
using System.Web.Mvc;
using Veb_projekat.Helpers;
using Veb_projekat.Models;

namespace Veb_projekat.Controllers
{
    public class AdministratorController : Controller
    {
        private const string PatientsFilePath = "~/App_Data/Patients.xml";
        private const string TherapiesFilePath = "~/App_Data/Therapies.xml";
        private const string AppointmentsFilePath = "~/App_Data/Appointments.xml";
        private const string DoctorsFilePath = "~/App_Data/Doctors.xml";

        // GET: Administrator
        public ActionResult Index()
        {
            var pacijenti = XMLHelper.UcitajPacijente(Server.MapPath(PatientsFilePath));
            return View(pacijenti);
        }

        // GET: Administrator/Delete/{username}
        public ActionResult Delete(string username)
        {
            var pacijenti = XMLHelper.UcitajPacijente(Server.MapPath(PatientsFilePath));
            var pacijent = pacijenti.FirstOrDefault(p => p.Username == username);
            if (pacijent != null)
            {
                pacijenti.Remove(pacijent);
                XMLHelper.SacuvajPacijente(pacijenti, Server.MapPath(PatientsFilePath));

                ObrisiPovezanePodatke(pacijent.Username);
            }
            return RedirectToAction("Index");
        }

        private void ObrisiPovezanePodatke(string username)
        {
            var terapije = XMLHelper.UcitajTerapije(Server.MapPath(TherapiesFilePath));
            terapije.RemoveAll(t => t.PatientUsername == username);
            XMLHelper.SacuvajTerapije(terapije, Server.MapPath(TherapiesFilePath));

            var termini = XMLHelper.UcitajTermine(Server.MapPath(AppointmentsFilePath), XMLHelper.UcitajPacijente(Server.MapPath(PatientsFilePath)));
            termini.RemoveAll(a => a.Patient.Username == username);
            XMLHelper.SacuvajTermine(termini, Server.MapPath(AppointmentsFilePath));

            var lekari = XMLHelper.UcitajLekare(Server.MapPath(DoctorsFilePath));
            foreach (var doktor in lekari)
            {
                doktor.ScheduledAppointments.RemoveAll(sa => sa.Contains(username));
            }
            XMLHelper.SacuvajLekare(lekari, Server.MapPath(DoctorsFilePath));
        }

        // GET: Administrator/Edit/{username}
        public ActionResult Edit(string username)
        {
            var pacijenti = XMLHelper.UcitajPacijente(Server.MapPath(PatientsFilePath));
            var pacijent = pacijenti.FirstOrDefault(p => p.Username == username);
            if (pacijent == null)
            {
                return HttpNotFound("Pacijent nije pronađen.");
            }
            return View(pacijent);
        }

        // POST: Administrator/Edit/{username}
        [HttpPost]
        public ActionResult Edit(Patient updatedPatient)
        {
            var pacijenti = XMLHelper.UcitajPacijente(Server.MapPath(PatientsFilePath));
            var pacijent = pacijenti.FirstOrDefault(p => p.Username == updatedPatient.Username);
            if (pacijent != null)
            {
                pacijenti.Remove(pacijent);
                pacijenti.Add(updatedPatient);
                XMLHelper.SacuvajPacijente(pacijenti, Server.MapPath(PatientsFilePath));
            }
            return RedirectToAction("Index");
        }

        // GET: Administrator/Create
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Patient newPatient)
        {
            var pacijenti = XMLHelper.UcitajPacijente(Server.MapPath(PatientsFilePath));

            
            if (pacijenti.Any(p => p.JMBG == newPatient.JMBG))
            {
                ModelState.AddModelError("JMBG", "Pacijent sa ovim JMBG-om već postoji.");
                return View(newPatient);
            }

            if (pacijenti.Any(p => p.Email.Equals(newPatient.Email, StringComparison.OrdinalIgnoreCase)))
            {
                ModelState.AddModelError("Email", "Pacijent sa ovom email adresom već postoji.");
                return View(newPatient);
            }

            string baseUsername = newPatient.LastName.ToLower();
            int usernameSuffix = 1;
            string newUsername = baseUsername;
            while (pacijenti.Any(p => p.Username.Equals(newUsername, StringComparison.OrdinalIgnoreCase)))
            {
                newUsername = baseUsername + usernameSuffix.ToString();
                usernameSuffix++;
            }
            newPatient.Username = newUsername;

            pacijenti.Add(newPatient);

            XMLHelper.SacuvajPacijente(pacijenti, Server.MapPath(PatientsFilePath));

            return RedirectToAction("Index");
        }

    }
}
