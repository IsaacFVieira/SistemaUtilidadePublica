using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaUtilidadePublicaMaui.Features.Information.Models
{
    internal class Information
    {
        public int Id_Information {  get; set;}
        public int Id_Criador { get; set;}
        public int Id_Tipo_Information { get; set;}
        public string Title { get; set;}
        public string Text_Information { get; set;}
        public string Imagem {  get; set;}
        public string Video { get; set;}
        public string audio { get; set;}
        public DateTime Data_Criacao { get; set;}
        public DateTime Data_Exclusão { get; set; }
        public string Statu {  get; set;
        }
    }
}
