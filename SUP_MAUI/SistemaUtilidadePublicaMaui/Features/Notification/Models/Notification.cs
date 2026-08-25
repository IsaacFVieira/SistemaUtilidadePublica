using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaUtilidadePublicaMaui.Features.Notification.Models
{
    internal class Notification
    {
        public int Id_Notification { get; set; }
        public DateTime Data_Envio { get; set; }
        public int Id_Tipo_Notification { get; set; }
        public int Id_Remetente { get; set; }
        public string Text_Notification { get; set; }

    }
}
