using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SyndiiWWWMVC.Models;

namespace SyndiiWWWMVC.Controllers
{
    /// <summary>
    /// HomeController for home page, just return the view page of home
    /// </summary>
    public class HomeController : Controller
    {
        // const string SessionLoggedIn = "false";
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Contact Us";

            return View("Contact");
        }

        [HttpPost]
        public ActionResult Contact(string name, string email, string comment)
        {
            // Name and message populated with the values from the form

            // TODO: send an e-mail or store messages here

            return Content("Thanks for contacting us! We'll be in touch with you soon.");
        }

        public ActionResult Privacy()
        {
            ViewBag.Message = "Privacy Policy";

            return View("Privacy");
        }

        public ActionResult Terms()
        {
            ViewBag.Message = "Terms of Use";

            return View("Terms");
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


    }
}
