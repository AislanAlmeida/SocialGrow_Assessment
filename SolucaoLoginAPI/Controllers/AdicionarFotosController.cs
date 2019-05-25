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
using SolucaoLoginAPI.Models;

namespace SolucaoLoginAPI.Controllers
{
    public class AdicionarFotosController : ApiController
    {
        // GET: api/AdicionarFotos
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/AdicionarFotos/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/AdicionarFotos
        [Authorize]
        public async Task<HttpResponseMessage> Post()
        {
            var resultado = await Request.Content.ReadAsMultipartAsync();
            var requisicaoJson = await resultado.Contents[0].ReadAsStringAsync();
            var modelo = JsonConvert.DeserializeObject<AlbumModel>(requisicaoJson);

            if (resultado.Contents.Count > 1)
            {
                modelo.Imagem = await CreateBlob(resultado.Contents[1], modelo.EmailUsuario.Replace("@", "-"));
            }

            PerfisEntities entities = new PerfisEntities();
            //Adiciona a foto do upload à tabela Galeria > Album "PERFIL"
            Galeria galeria = new Galeria()
            {
                EmailUsuario = modelo.EmailUsuario,
                Album = modelo.Albumfoto,
                Imagem = modelo.Imagem,
                CapaAlbum = modelo.Imagem

            };
            entities.Galeria.Add(galeria);

            entities.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.OK);

        }

        private async Task<string> CreateBlob(HttpContent httpContent, string emailUsuario)
        {
            var storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
            var ContainerName = emailUsuario.ToLower().Replace(" ", string.Empty).Replace(".", "");


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

    }
}
