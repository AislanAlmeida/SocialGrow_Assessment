using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace SolucaoLoginAPI.Controllers
{
    public class AmigosController : ApiController
    {
        // GET: api/Amigos
        public IEnumerable<string> Get()
        {
            
            return new string[] { "value1", "value2" };
        }

        // GET: api/Amigos/5
        public async Task<HttpResponseMessage> Get(string email)
        {
            using (var Contexto = new PerfisEntities())
            {
                List<string> amigos = new List<string>();
                var amigosquesolicitei = Contexto.Amigos.Where(x => x.SolicitacaoEnviada == email).Where(x => x.SolicitacaoAceita == "S").Select(x => x.SolicitacaoRecebida).ToList();
                var convitesqueaceitei = Contexto.Amigos.Where(x => x.SolicitacaoRecebida == email).Where(x => x.SolicitacaoAceita == "S").Select(x => x.SolicitacaoEnviada).ToList();
                //var convitesqueaceitei = Contexto.Amigos.Where(x => x.SolicitacaoRecebida == email).Select(x => x.SolicitacaoEnviada).ToList();

                foreach (var item in amigosquesolicitei)
                {
                    amigos.Add(item);
                }
                foreach (var item in convitesqueaceitei)
                {
                    amigos.Add(item);
                }

                return Request.CreateResponse(HttpStatusCode.OK, amigos);
            }
        }

        // POST: api/Amigos
        public async Task<HttpResponseMessage> Post()
        {
            var requisicao = await Request.Content.ReadAsStringAsync();
            var JSolicitacao = JsonConvert.DeserializeObject<Amigos>(requisicao);

            using (var Contexto = new PerfisEntities())
            {
                //var ultid = Contexto.Amigos.Max(x => x.Id);

                Amigos Solicitacao = new Amigos()
                {
                    SolicitacaoEnviada = JSolicitacao.SolicitacaoEnviada,
                    SolicitacaoRecebida = JSolicitacao.SolicitacaoRecebida,
                };

                //antes de adicionar devo consultar se ja recebi uma solicitação deste email
                try
                {
                    var teste = Contexto.Amigos.Where(x => x.SolicitacaoRecebida == Solicitacao.SolicitacaoEnviada).Where(x => x.SolicitacaoEnviada == Solicitacao.SolicitacaoRecebida).Single();
                    var teste2 = Contexto.Amigos.Where(x => x.SolicitacaoEnviada == Solicitacao.SolicitacaoEnviada).Where(x => x.SolicitacaoRecebida == Solicitacao.SolicitacaoRecebida).Single();
                }
                catch
                {

                    Contexto.Amigos.Add(Solicitacao);
                    await Contexto.SaveChangesAsync();
                }
                

            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        // PUT: api/Amigos/5
        public async Task<HttpResponseMessage> Put()
        {
            var requisicao = await Request.Content.ReadAsStringAsync();
            var JSolicitacao = JsonConvert.DeserializeObject<Amigos>(requisicao);

            using (var Contexto = new PerfisEntities())
            {
                //var ultid = Contexto.Amigos.Max(x => x.Id);

                Amigos Solicitacao = new Amigos()
                {
                    SolicitacaoEnviada = JSolicitacao.SolicitacaoEnviada,
                    SolicitacaoRecebida = JSolicitacao.SolicitacaoRecebida,
                };
                var solicitacao = Contexto.Amigos.Where(x => x.SolicitacaoEnviada == Solicitacao.SolicitacaoEnviada).Where(x => x.SolicitacaoRecebida == Solicitacao.SolicitacaoRecebida).Single();
                solicitacao.SolicitacaoAceita = "S";
                Contexto.SaveChanges();
                await Contexto.SaveChangesAsync();
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        // DELETE: api/Amigos/5
        public void Delete(int id)
        {
        }
    }
}
