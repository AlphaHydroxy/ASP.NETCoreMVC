using DotNet.Models;
using DotNet.Services;
using DotNet.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNet.Controllers.Web
{
    public class AppController : Controller
    {
        private IMailService _mailService;
        private IConfigurationRoot _config;
        private IWorldRepository _repository;

        public AppController(IMailService mailService, IConfigurationRoot config, IWorldRepository repository)
        {
            _mailService = mailService;
            _config = config;
            _repository = repository;
        }

        public IActionResult Index()
        {
            //fetches a list of all the trips
            var data = _repository.GetAllTrips(); //query
            return View(data);
        }

        public IActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Contact(ContactViewModel model)
        {
            //add own validation
            //if (model.Email.Contains("aol.com"))
            //{
            //    ModelState.AddModelError("Email", "We don't support AOL addresses");
            //}

            if (ModelState.IsValid)//serverside validation
            {
            _mailService.SendMail(_config["MailSettings:ToAddress"], model.Email, "From the App", model.Message);

                //clears form after submission
                ModelState.Clear();

                //shows message confirmation from Contact.cshtml
                ViewBag.UserMessage = "Message Sent!";
            }

            return View();
        }

        public IActionResult About()
        {
            return View();
        }
    }
}
