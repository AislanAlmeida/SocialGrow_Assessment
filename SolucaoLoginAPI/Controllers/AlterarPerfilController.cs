using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
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
    public class AlterarPerfilController : ApiController
    {
        // GET: api/AlterarPerfil
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/AlterarPerfil/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/AlterarPerfil
        [Authorize]
        //public HttpResponseMessage Post([FromBody]Dictionary<string,string> value)
        public async Task<HttpResponseMessage> Post()
        {
            var resultado = await Request.Content.ReadAsMultipartAsync();
            var requisicaoJson = await resultado.Contents[0].ReadAsStringAsync();

            var modelo = JsonConvert.DeserializeObject<Table>(requisicaoJson);

            if (resultado.Contents.Count > 1)
            {
                modelo.PictureUrl = await CreateBlob(resultado.Contents[1], modelo.EmailUser.Replace("@","-"));
            }

            // A função aqui traz o perfil da pessoa e atualiza tudo que ela modificou
            PerfisEntities entities = new PerfisEntities();
            Table perfil = entities.Table.First(x => x.EmailUser == modelo.EmailUser);
            perfil.Nome = modelo.Nome;
            perfil.Sobrenome = modelo.Sobrenome;
            perfil.PictureUrl = modelo.PictureUrl;
            
            if (modelo.Aniversario != null)
            {
                perfil.Aniversario = Convert.ToDateTime(modelo.Aniversario.ToString());
            }
            entities.SaveChanges();

            //Adiciona a foto do upload à tabela Galeria > Album "PERFIL"
            Galeria galeria = new Galeria()
            {
                EmailUsuario = modelo.EmailUser,
                Album = "Perfil",
                Imagem = modelo.PictureUrl,
                CapaAlbum = modelo.PictureUrl
                
            };
            entities.Galeria.Add(galeria);

            entities.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.OK, entities.Table.Where(x => x.EmailUser == modelo.EmailUser).ToList());

            /*
            var emailvalue = value["EmailUser"].ToString();
            PerfisEntities entities = new PerfisEntities();
            Table perfil = entities.Table.First(x => x.EmailUser == emailvalue);
            perfil.Nome = value["Nome"];
            perfil.Sobrenome = value["Sobrenome"];
            perfil.PictureUrl = value["PictureUrl"];
            if (value["Aniversario"] !=null)
            {
                perfil.Aniversario = Convert.ToDateTime(value["Aniversario"].ToString());
            }
            entities.SaveChanges();


            return Request.CreateResponse(HttpStatusCode.OK, entities.Table.Where(x => x.EmailUser == emailvalue).ToList());
           */
        }

        private async Task<string> CreateBlob(HttpContent httpContent,string emailUsuario)
        {
            var storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
            var ContainerName = emailUsuario.ToLower().Replace(" ",string.Empty).Replace(".","");


            var blobCliente = storageAccount.CreateCloudBlobClient();
            var blobContainer = blobCliente.GetContainerReference(ContainerName);
            await blobContainer.CreateIfNotExistsAsync();

            await blobContainer.SetPermissionsAsync(
                new BlobContainerPermissions
                {
                    PublicAccess = BlobContainerPublicAccessType.Blob
                });

            var fileName = httpContent.Headers.ContentDisposition.FileName;
            var byteArray = await httpContent.ReadAsByteArrayAsync();

            var blob = blobContainer.GetBlockBlobReference(GetRandomBlobName(fileName));
            await blob.UploadFromByteArrayAsync(byteArray, 0, byteArray.Length);
            return blob.Uri.AbsoluteUri;
        }

        private string GetRandomBlobName(string filename)
        {
            string ext = Path.GetExtension(filename);
            return string.Format("{0:10}_{1}{2}", DateTime.Now.Ticks, Guid.NewGuid(), ext);
        }

        // PUT: api/AlterarPerfil/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/AlterarPerfil/5
        public void Delete(int id)
        {
        }
    }
}
