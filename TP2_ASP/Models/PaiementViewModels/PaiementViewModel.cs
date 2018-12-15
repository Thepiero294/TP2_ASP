using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TP2_ASP.Data;

namespace TP2_ASP.Models.PaiementViewModels
{
    public class PaiementViewModel
    {
        [BindProperty]
        public string StripeToken { get; set; }
        [BindProperty]
        public string StripeTokenType { get; set; }
        [BindProperty]
        public string StripeEmail { get; set; }

        public static void ajouterSolde(DbContextOptions<ApplicationDbContext> option, string id,int solde)
        {
            CustomUser utilisateur;

            using (ApplicationDbContext context = new ApplicationDbContext(option))
            {

                utilisateur = context.Users.First(u => u.Id == id);
                utilisateur.Solde += solde;
                context.SaveChanges();
            }
        }
    }
}
