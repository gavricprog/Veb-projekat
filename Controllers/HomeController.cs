using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Veb_projekat.Helpers; 
using Veb_projekat.Models;

namespace Veb_projekat.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home/Index
        public ActionResult Index()
        {
            return View();
        }

        // GET: Home/Login
        public ActionResult Login()
        {
            return View();
        }

        // POST: Home/Login
        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            List<Administrator> administratori = XMLHelper.UcitajAdministratore(Server.MapPath("~/App_Data/Administrators.xml"));
            List<Doctor> lekari = XMLHelper.UcitajLekare(Server.MapPath("~/App_Data/Doctors.xml"));
            List<Patient> pacijenti = XMLHelper.UcitajPacijente(Server.MapPath("~/App_Data/Patients.xml"));

            var admin = administratori.FirstOrDefault(a => a.Username == username && a.Password == password);
            if (admin != null)
            {
                Session["UserType"] = "Administrator";
                Session["Username"] = admin.Username;
                return RedirectToAction("Index", "Administrator");
            }

            var lekar = lekari.FirstOrDefault(l => l.Username == username && l.Password == password);
            if (lekar != null)
            {
                Session["UserType"] = "Doctor";
                Session["Username"] = lekar.Username; 
                return RedirectToAction("Index", "Doctor");
            }

            var pacijent = pacijenti.FirstOrDefault(p => p.Username == username && p.Password == password);
            if (pacijent != null)
            {
                Session["UserType"] = "Patient";
                Session["Username"] = pacijent.Username; 
                return RedirectToAction("Index", "Patient");
            }


            ViewBag.Message = "Pogrešno korisničko ime ili lozinka.";
            return View("Index");
        }



        // GET: Home/Logout
        public ActionResult Logout()
        {
            Session.Abandon(); 
            return RedirectToAction("Login");
        }

        public ActionResult About()
        {
            ViewBag.Message = "Everything you need to know about us.";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Our contact page.";
            return View();
        }
    }
}
