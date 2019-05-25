using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SolucaoLoginAPI.Models
{
    public class Foto
    {
        public string Album { get; set; }
        public string Email { get; set; }
    }
    public class AlbumModel
    {
        public string EmailUsuario { get; set; }
        public string Albumfoto { get; set; }
        public string Imagem { get; set; }

    }
}