using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SolucaoLoginWEB.Models
{
    public class SolicitacaoAmizade
    {
        public int Id { get; set; }
        public string SolicitacaoEnviada { get; set; }
        public string SolicitacaoRecebida { get; set; }
        public string SolicitacaoAceita { get; set; }
    }
}