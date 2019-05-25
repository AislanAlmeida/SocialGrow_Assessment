using Newtonsoft.Json;
using SolucaoLoginWEB.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SolucaoLoginWEB.Controllers
{
    public class GaleriaController : Controller
    {

        List<string> ConsultaAlbuns(string email)
        {
            var emailPerfil = new Dictionary<string, string>
                {
                    {"", email }
                };

            var perfilAConsultar = new FormUrlEncodedContent(emailPerfil);

            using (var Requisicao = new HttpClient())
            {
                Requisicao.BaseAddress = new Uri("http://localhost:49703");
                
                //NAO PRECISO ME AUTORIZAR
                //-------------------------
                //Requisicao.DefaultRequestHeaders.Accept.Clear();
                //Requisicao.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", $"{access_token}");
                var Resposta = Requisicao.PostAsync("api/Galeria", perfilAConsultar).Result;

                string res = "";

                using (HttpContent conteudo = Resposta.Content)
                {

                    Task<string> resultado = conteudo.ReadAsStringAsync();
                    res = resultado.Result;

                    dynamic Jperfil = JsonConvert.DeserializeObject(res);                    

                    List<string> albuns = new List<string>();
                    foreach (var item in Jperfil)
                    {
                        albuns.Add(item.ToString());
                    }

                    return albuns;
                }
            }
        }

        public ActionResult Index()
        {
            //Consultar minhas galerias de fotos

            string access_token = Session["access_token"]?.ToString();

            //CASO USUARIO NÃO ESTEJA LOGADO DIRECIONA PARA O LOGIN
            //CASO ESTEJA LOGADO MOSTRA AS INFORMAÇÕES ATUAIS DE PERFIL

            if (Session["Email_User"] == null || Session["Email_User"].ToString() == "")
            {
                return RedirectToAction("Index", "Login");
            }
            else
            {
                ViewBag.Albuns = ConsultaAlbuns(Session["Email_User"].ToString());
                ViewBag.Email = Session["Email_User"].ToString();
                //TempData.Keep("EmailPesquisado");
                return View();
            }
            /*
            if (TempData["EmailPesquisado"] != null)
            {
                ViewBag.Albuns = ConsultaAlbuns(TempData["EmailPesquisado"].ToString());
                ViewBag.Email = TempData["EmailPesquisado"].ToString();
                TempData.Keep("EmailPesquisado");
                return View();
            }
            else
            {
                if (access_token != null)
                {

                    ViewBag.Albuns = ConsultaAlbuns(Session["Email_User"].ToString());
                    ViewBag.Email = Session["Email_User"].ToString();
                    return View();
                }
                else
                {
                    return RedirectToAction("Index", "Login");
                }
            }
            */
        }

        [HttpPost]
        public ActionResult Index(string email)
        {
            ViewBag.Email = email;
            ViewBag.Albuns = ConsultaAlbuns(email);
            return View();            
        }

        [HttpGet]
        public ActionResult FotosdoAlbum()
        {
            return RedirectToAction("Index", "Galeria");
        }

        [HttpPost]
        public async Task<ActionResult> FotosDoAlbum(string AlbumPost, string Emailpost)
        {
           
            Fotos fotos = new Fotos()
            {
                Email = Emailpost,
                Album = AlbumPost
            };

            using (var requisicao = new HttpClient())
            {
                
                requisicao.BaseAddress = new Uri("http://localhost:49703");

                var Resposta = await requisicao.PostAsJsonAsync("api/Fotos", fotos);
                var res = "";
                using (HttpContent conteudo = Resposta.Content)
                {
                    Task<string> resultado = conteudo.ReadAsStringAsync();
                    res = resultado.Result;

                    dynamic imagens = JsonConvert.DeserializeObject(res);
                    List<string> ListaFotos = new List<string>();

                    foreach (var item in imagens)
                    {
                        ListaFotos.Add(item.ToString());
                    }

                    ViewBag.Fotos = ListaFotos;
                    ViewBag.Email = fotos.Email;
                    ViewBag.Album = fotos.Album;

                    return View();
                }
                
            }
        }

        [HttpGet]
        public ActionResult CriarAlbum(string album = "")
        {
            ViewBag.nomeAlbum = album;

            string access_token = Session["access_token"]?.ToString();

            if (access_token == null)
            {
                return RedirectToAction("Index", "Login");
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public async Task<ActionResult> CriarAlbum(AlbumModel model)
        {
            string access_token = Session["access_token"]?.ToString();
            if (string.IsNullOrEmpty(access_token))
            {
                return RedirectToAction("Error");
            }
            else
            {
                if (ModelState.IsValid)
                {
                    using (var cliente = new HttpClient())
                    {

                        model.EmailUsuario = Session["Email_User"].ToString();
                        using (var conteudo = new MultipartFormDataContent())
                        {
                            model.Imagem = "ImagemUpload";
                            cliente.BaseAddress = new Uri("http://localhost:49703");
                            cliente.DefaultRequestHeaders.Accept.Clear();
                            cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", $"{access_token}");
                            conteudo.Add(new StringContent(JsonConvert.SerializeObject(model)));
                            if (Request.Files.Count > 0)
                            {
                                byte[] filebytes;
                                using (var inputStream = Request.Files[0].InputStream)
                                {
                                    var memoryStream = inputStream as MemoryStream;
                                    if (memoryStream == null)
                                    {
                                        memoryStream = new MemoryStream();
                                        inputStream.CopyTo(memoryStream);
                                    }
                                    filebytes = memoryStream.ToArray();
                                }

                                var fileContent = new ByteArrayContent(filebytes);

                                fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                                fileContent.Headers.ContentDisposition.FileName = Request.Files[0].FileName.Split('\\').Last();

                                conteudo.Add(fileContent);
                            }
                            
                            var resposta = await cliente.PostAsync("api/AdicionarFotos", conteudo);

                            if (resposta.IsSuccessStatusCode)
                            {
                                return RedirectToAction("Index", "Galeria");
                            }
                            else
                            {
                                return RedirectToAction("Error");
                            }
                            

                        }
                    }
                }
                return View(model);
            }

        }
    }
}