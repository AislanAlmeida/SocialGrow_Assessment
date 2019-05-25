using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SolucaoLoginAPI.Controllers
{
    public class GaleriaController : ApiController
    {
        // GET: api/Galeria
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Galeria/5
        public string Get(int id)
        {
            return "value";
        }


        // POST: api/Galeria

        public HttpResponseMessage Post([FromBody]string value)
        {
            using (PerfisEntities entidade = new PerfisEntities())
            {
                List<string> albuns = new List<string>();
                var teste = entidade.Galeria.Where(x => x.EmailUsuario == value).GroupBy(y => y.Album).ToList();

                foreach (var item in teste)
                {
                    albuns.Add(item.Key);
                }
                

                return Request.CreateResponse(HttpStatusCode.OK, albuns);
            }
        }

        // PUT: api/Galeria/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Galeria/5
        public void Delete(int id)
        {
        }
    }
}
