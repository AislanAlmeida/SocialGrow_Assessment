using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SolucaoLoginWEB.Models
{
    public class AlbumModel
    {
        public string EmailUsuario { get; set; }
        public string Albumfoto { get; set; }
        public string Imagem { get; set; }

    }
    public class Fotos
    {
        public string Album { get; set; }
        public string Email { get; set; }
    }
}