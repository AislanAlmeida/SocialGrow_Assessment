using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace SolucaoLoginAPI.Controllers
{
    public class ConvitesPendentesController : ApiController
    {
        // GET: api/ConvitesPendentes
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/ConvitesPendentes/5
        public async Task<HttpResponseMessage> Get(string email)
        {
            List<Amigos> amigos = new List<Amigos>();
            using (var Contexto = new PerfisEntities())
            {
                var pendentesRecebidos = Contexto.Amigos.Where(x => x.SolicitacaoRecebida == email).Where(x => x.SolicitacaoAceita != "S").ToList();
                var pendentesEnviados = Contexto.Amigos.Where(x => x.SolicitacaoEnviada == email).Where(x => x.SolicitacaoAceita != "S").ToList();
                foreach (var item in pendentesRecebidos)
                {
                    amigos.Add(item);
                }
                foreach (var item in pendentesEnviados)
                {
                    amigos.Add(item);
                }
                return Request.CreateResponse(HttpStatusCode.OK,amigos);
            }
        }

        // POST: api/ConvitesPendentes
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/ConvitesPendentes/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/ConvitesPendentes/5
        public void Delete(int id)
        {
        }
    }
}
