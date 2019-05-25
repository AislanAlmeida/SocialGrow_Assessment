using Newtonsoft.Json;
using SolucaoLoginAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace SolucaoLoginAPI.Controllers
{
    public class FotosController : ApiController
    {
        // GET: api/Fotos
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Fotos/5
        public string Get(string Album,string EmailUsuario)
        {


            return "value";
        }

        // POST: api/Fotos
        public async Task<HttpResponseMessage> Post()
        {
            //var teste = await Request.Content.ReadAsFormDataAsync();
            
            var request = await Request.Content.ReadAsStringAsync();

            var Jalbum = JsonConvert.DeserializeObject<Foto>(request);

            using (PerfisEntities entidade = new PerfisEntities())
            {
                List<string> fotos = new List<string>();
                var teste = entidade.Galeria.Where(x => x.EmailUsuario == Jalbum.Email).Where(y => y.Album == Jalbum.Album).ToList();

                foreach (var item in teste)
                {
                    fotos.Add(item.Imagem);
                }


                return Request.CreateResponse(HttpStatusCode.OK, fotos);
            }



        }

        // PUT: api/Fotos/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Fotos/5
        public void Delete(int id)
        {
        }
    }
}
