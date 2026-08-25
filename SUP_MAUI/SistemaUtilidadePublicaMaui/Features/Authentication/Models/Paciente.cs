using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaUtilidadePublicaMaui.Features.Authentication.Models
{
    internal class Paciente
    {
        public int Id_Paciente { get; set; }
        public int Id_User { get; set; }
        public DateTime Data_Nascimento { get; set; }
        public string Genero { get; set; }
        public string Estado_Civil { get; set; }
        public string Morada { get; set; }
    }
}
