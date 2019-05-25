using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace SolucaoLoginAPI.Controllers
{
    public class PerfilController : ApiController
    {
        // GET: api/Perfil
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Perfil/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Perfil
  
        public HttpResponseMessage Post([FromBody]string value)
        {
            using (PerfisEntities entidade = new PerfisEntities())
            {
                var perfis = entidade.Table.Where(x => x.EmailUser == value).ToList();
                return Request.CreateResponse(HttpStatusCode.OK, perfis);
            }
        }

        
        // PUT: api/Perfil/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Perfil/5
        public void Delete(int id)
        {
        }
    }
}
