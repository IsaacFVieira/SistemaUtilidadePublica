using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaUtilidadePublicaMaui.Features.Authentication.Models
{
    
    internal class User
    {
        public int Id_User{ get; set; }
        public string Name { get; set; }
        public string Telefone { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ImagemPerfil { get; set; } 

    }
}
