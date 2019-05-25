using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SolucaoLoginWEB.Models
{
    public class PerfilModel
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Sobrenome { get; set; }
        public string EmailUser { get; set; }
        public string PictureUrl { get; set; }
        public DateTime Aniversario { get; set; }
        public string AccountNumber { get; set; }
    }
}