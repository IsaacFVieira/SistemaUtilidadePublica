using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaUtilidadePublicaAPI.Models
{
    
    public class User
    {
        public int Id_User{ get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Telefone { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; }
        public string PasswordHash { get; set; } = string.Empty;
        public string Salt { get; set; }
        public string? ImagemPerfil { get; set; } 
        public DateTime Create_Date { get; set; }

    }
}
