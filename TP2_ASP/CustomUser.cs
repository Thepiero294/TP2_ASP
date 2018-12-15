using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TP2_ASP
{
    public class CustomUser : IdentityUser
    {
        public string Nom { get; set; }
        public int Solde{ get; set; }
    }
}
