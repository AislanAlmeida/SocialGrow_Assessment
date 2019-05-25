using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SolucaoLoginWEB.Models
{
    public class RegistroModel
    {
        public string Email { get; set; }
        public string Senha { get; set; }
        public string ConfirmacaoSenha { get; set; }
    }
}