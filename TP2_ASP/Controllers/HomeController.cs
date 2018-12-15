using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TP2_ASP.Data;
using TP2_ASP.Models;
using TP2_ASP.Models.SendViewModels;

namespace TP2_ASP.Controllers
{
    public class HomeController : Controller
    {
        private readonly DbContextOptions<ApplicationDbContext> context;

        public HomeController(DbContextOptions<ApplicationDbContext> option)
        {
            context = option;
        }

        public IActionResult Index()
        {
            var model = new List<SendViewModel>();
            int nb = 0;
            foreach (SendViewModel item in SendViewModel.getTelecopies(context, User.FindFirstValue(ClaimTypes.NameIdentifier)))
            {
                model.Add(item);
            }

            return View(model);
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
