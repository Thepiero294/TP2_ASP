using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using TP2_ASP.Data;

namespace TP2_ASP.Models.SendViewModels
{
    public class SendViewModel
    {
        [Key]
        public int Id { get; set; }
        public string Nom { get; set; }
        public string Adresse { get; set; }
        [Required]
        public string NuméroTelecopieur { get; set; }
        [Required]
        public string Fichier { get; set; }
        public string Status { get; set; }
        [Required]
        public virtual string Sender { get; set; }

        private SendViewModel() { }

        private SendViewModel(string nom, string adresse, string numéroTelecopieur, string fichier, string status, string sender)
        {
            Nom = nom;
            Adresse = adresse;
            NuméroTelecopieur = numéroTelecopieur;
            Fichier = fichier;
            Status = status;
            Sender = sender;
        }


        public static int insert(DbContextOptions<ApplicationDbContext> option, string nom, string adresse, string numeroTelecopieur, string fichier, string status, string sender)
        {
            int id;
            using (ApplicationDbContext context = new ApplicationDbContext(option))
            {
                SendViewModel telecopie = new SendViewModel(nom,  adresse, numeroTelecopieur, fichier, status, sender);
                context.SendViewModels.Add(telecopie);
                context.SaveChanges();
                return telecopie.Id; ;
            }
            
        }

        public static IQueryable<SendViewModel> getTelecopies(DbContextOptions<ApplicationDbContext> option, string sender)
        {
            IQueryable<SendViewModel> listTelecopies;

            using (ApplicationDbContext context = new ApplicationDbContext(option))
            {
                listTelecopies = (from t in context.SendViewModels where t.Sender == sender select t);
            }
            return listTelecopies;
        }

        public static void SetStatus(DbContextOptions<ApplicationDbContext> option,int id, string status)
        {
            SendViewModel Telecopie;

            using (ApplicationDbContext context = new ApplicationDbContext(option))
            {

                Telecopie = context.SendViewModels.First(u => u.Id == id );
                Telecopie.Status = status;
                context.SaveChanges();
            }
        }
    }
}
