using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;
using TP2_ASP.Data;
using TP2_ASP.Models.PaiementViewModels;

namespace TP2_ASP.Controllers
{
    public class PaiementController : Controller
    {
        private readonly DbContextOptions<ApplicationDbContext> context;
        public PaiementController(DbContextOptions<ApplicationDbContext> option)
        {
            context = option;
        }

        [Route("/paiement/paiementReussi")]
        public IActionResult Charge(string stripeEmail, string stripeToken,string nombreTelecopies, string montant)
        {
            string idUser = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var customers = new CustomerService();
            var charges = new ChargeService();

            var customer = customers.Create(new CustomerCreateOptions
            {
                Email = stripeEmail,
                SourceToken = stripeToken
            });

            var charge = charges.Create(new ChargeCreateOptions
            {
                Amount =  Convert.ToInt32(montant),
                Description = "Sample Charge",
                Currency = "cad",
                CustomerId = customer.Id
            });

            PaiementViewModel.ajouterSolde(context, idUser, Convert.ToInt32(nombreTelecopies));

            return View("Charge");
        }

        [Route("paiement")]
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }

    }
}