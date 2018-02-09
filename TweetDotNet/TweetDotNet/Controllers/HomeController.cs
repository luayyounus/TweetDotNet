using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TweetDotNet.Models;

namespace TweetDotNet.Controllers
{
    // main home controller provided by Asp.NET MVC that inherits from Controller
    public class HomeController : Controller
    {
        // Default index action that returns the index view
        public IActionResult Index()
        {
            return View();
        }

        // Default about action that return About view
        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        // Default contact me action with its view 
        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        // Error action returning the error view model with requested id or the unique id from logs
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
