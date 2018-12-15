using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TP2_ASP.Data;
using TP2_ASP.Models.SendViewModels;
using Microsoft.AspNetCore.Hosting;

namespace TP2_ASP.Controllers
{
    [Route("Send/[action]/{name?}")]
    [Authorize]
    public class SendController : Controller
    {
        private readonly DbContextOptions<ApplicationDbContext> context;
        private readonly ILogger _logger;

        [Route("/send")]
        [HttpGet]
        public IActionResult Index()
        {
            var model = new List<SendViewModel>();
            foreach (SendViewModel item in SendViewModel.getTelecopies(context, User.FindFirstValue(ClaimTypes.NameIdentifier)))
            {
                model.Add(item);
            }
            
            return View("Index", model);
        }

        // GET send/create
        public IActionResult Create()
        {
            return View("Create");
        }

        public SendController(IBackgroundTaskQueue queue, DbContextOptions<ApplicationDbContext> option, ILogger<SendController> logger)
        {
            Queue = queue;
            _logger = logger;
            context = option;
        }

        public IBackgroundTaskQueue Queue { get; }

        [HttpPost]
        public IActionResult OnPostAddTask(string nom, string adresse, string NuméroTelecopieur)
        {
            BlobClass blob = new BlobClass();
            string fileName = string.Empty;
            if (HttpContext.Request.Form.Files != null)
            {
                var files = HttpContext.Request.Form.Files;

                foreach (var file in files)
                {
                    if (file.Length > 0)
                    {
                        fileName = file.FileName;
                        blob.Post(file);
                    }
                }
            }
            string idUser = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int id = SendViewModel.insert(context, nom, adresse, NuméroTelecopieur, fileName, "En cours", idUser);
            string view;
            CustomUser utilisateur;
            using (ApplicationDbContext database = new ApplicationDbContext(context))
            {
                utilisateur = database.Users.First(u => u.Id == idUser);
                if (utilisateur.Solde != 0)
                {
                    utilisateur.Solde -= 1;
                    database.SaveChanges();
                    Queue.QueueBackgroundWorkItem(async token =>
                    {
                        await EnvoieAsync(id);
                    });

                    var model = new List<SendViewModel>();
                    foreach (SendViewModel item in SendViewModel.getTelecopies(context, User.FindFirstValue(ClaimTypes.NameIdentifier)))
                    {
                        model.Add(item);
                    }

                    return View("Index", model);
                }
                else
                    return View("soldeInsufisant");
            }
        }

        private async Task EnvoieAsync(int id)
        {
            int tentativeEnvoie = 0;
            bool reussite = false;
            string idUser = User.FindFirstValue(ClaimTypes.NameIdentifier);
            while (!reussite)
            {
                if (estValide())
                {
                    SendViewModel.SetStatus(context, id, "Réussis");
                    reussite = true;
                }
                else
                {
                    tentativeEnvoie++;
                    SendViewModel.SetStatus(context, id, "En cours, Tentative " + tentativeEnvoie);
                }

                if (tentativeEnvoie == 3)
                {
                    SendViewModel.SetStatus(context, id, "Échec et Remboursé");
                    CustomUser utilisateur;
                    using (ApplicationDbContext database = new ApplicationDbContext(context))
                    {
                        utilisateur = database.Users.First(u => u.Id == idUser);
                        utilisateur.Solde += 1;
                        database.SaveChanges();
                    }
                    reussite = true;
                }
                Task.Delay(tempsAttente());
            }
        }

        [Route("/soldeInsufisant")]
        // GET send/create
        public IActionResult soldeInsufisant()
        {
            return View("soldeInsufisant");
        }

        public IActionResult retry(int id)
        {
            string idUser = User.FindFirstValue(ClaimTypes.NameIdentifier);
            CustomUser utilisateur;
            using (ApplicationDbContext database = new ApplicationDbContext(context))
            {
                utilisateur = database.Users.First(u => u.Id == idUser);
                if (utilisateur.Solde != 0)
                {
                    utilisateur.Solde -= 1;
                    database.SaveChanges();
                    Queue.QueueBackgroundWorkItem(async token =>
                    {
                        await EnvoieAsync(id);
                    });

                    return Redirect("/send");
                }
                else
                    return Redirect("/soldeInsufisant");
            }
        }

        private bool estValide()
        {
            Random r = new Random();
            int resultInt;
            resultInt = r.Next(100);
            if (resultInt > 60)
                return true;
            else
                return false;
        }

        private int tempsAttente()
        {
            Random r = new Random();
            int resultInt;
            return resultInt = r.Next(30000);
        }
    }
}