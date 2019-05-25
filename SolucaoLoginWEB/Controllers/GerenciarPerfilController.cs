using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Mvc;
using SolucaoLoginWEB.Models;
using System.Threading.Tasks;
using System.IO;

namespace SolucaoLoginWEB.Controllers
{
    public class GerenciarPerfilController : Controller
    {
        // GET: GerenciarPerfil
        public ActionResult Index()
        {
            
            string access_token = Session["access_token"]?.ToString();

            //CASO USUARIO NÃO ESTEJA LOGADO DIRECIONA PARA O LOGIN
            //CASO ESTEJA LOGADO MOSTRA AS INFORMAÇÕES ATUAIS DE PERFIL
            if (access_token != null)
            {

                var emailPerfil = new Dictionary<string, string>
                {
                    {"", Session["Email_User"].ToString() }
                };

                var perfilAConsultar = new FormUrlEncodedContent(emailPerfil);

                using (var Requisicao = new HttpClient())
                {
                    Requisicao.BaseAddress = new Uri("http://localhost:49703");
                    Requisicao.DefaultRequestHeaders.Accept.Clear();
                    Requisicao.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", $"{access_token}");           
                    var Resposta = Requisicao.PostAsync("api/Perfil", perfilAConsultar).Result;

                    string res = "";


                    using (HttpContent conteudo = Resposta.Content)
                    {
                        Task<string> resultado = conteudo.ReadAsStringAsync();
                        res = resultado.Result;

                        dynamic Jperfil = JsonConvert.DeserializeObject(res);

                        //não dá pra converter direto porque se o aniversário estiver null
                        //quebra o programa
                        //PerfilModel perfil = JsonConvert.DeserializeObject<PerfilModel>(JsonConvert.SerializeObject(Jperfil[0]));
                        
                        var perfil = new PerfilModel()
                        {
                            EmailUser = Jperfil[0]["EmailUser"].ToString(),
                            Nome = Jperfil[0]["Nome"].ToString(),
                            Sobrenome = Jperfil[0]["Sobrenome"].ToString(),
                            PictureUrl = Jperfil[0]["PictureUrl"].ToString(),
                            AccountNumber = Jperfil[0]["AccountNumber"].ToString(),
                        };
                        if (Jperfil[0]["Aniversario"] != null)
                        {
                            perfil.Aniversario = Convert.ToDateTime(Jperfil[0]["Aniversario"].ToString());
                        }
                        
                        Session.Add("account_Id", perfil.AccountNumber);

                        return View(perfil);
                    }

                }
            }

            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

        [HttpPost]
        public async Task<ActionResult> Index(PerfilModel model)
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
                        using (var conteudo = new MultipartFormDataContent())
                        {
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
                            var resposta = await cliente.PostAsync("api/AlterarPerfil", conteudo);

                            string res = "";
                            var resultado = resposta.Content.ReadAsStringAsync();
                            using (HttpContent RespostaAPI = resposta.Content)
                            {
                                Task<string> resultadoAPI = RespostaAPI.ReadAsStringAsync();
                                res = resultado.Result;

                                dynamic Jperfil = JsonConvert.DeserializeObject(res);

                                var perfil = new PerfilModel()
                                {
                                    EmailUser = Jperfil[0]["EmailUser"].ToString(),
                                    Nome = Jperfil[0]["Nome"].ToString(),
                                    Sobrenome = Jperfil[0]["Sobrenome"].ToString(),
                                    PictureUrl = Jperfil[0]["PictureUrl"].ToString(),
                                    AccountNumber = Jperfil[0]["AccountNumber"].ToString(),
                                };
                                if (Jperfil[0]["Aniversario"] != null)
                                {
                                    perfil.Aniversario = Convert.ToDateTime(Jperfil[0]["Aniversario"].ToString());
                                }


                                return View(perfil);
                            }

                        }
                    }
                }
                return View(model);
            }

            /*
            string access_token = Session["access_token"]?.ToString();
            if (!string.IsNullOrEmpty(access_token))
            {
                //AUTENTICAÇÃO
                using (var cliente = new HttpClient())
                {
                    cliente.BaseAddress = new Uri("http://localhost:49703");
                    cliente.DefaultRequestHeaders.Accept.Clear();
                    cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", $"{access_token}");
                    var Resposta = cliente.PostAsJsonAsync("api/AlterarPerfil", model).Result;
                    string res = "";


                    //using (HttpContent conteudo = Resposta.Content)
                    using(var conteudo = new MultipartFormDataContent())
                    {

                        //var imagemPerfil = Request.Files;
                        HttpPostedFileBase arquivo = Request.Files[0];
                        Stream fs = arquivo.InputStream;

                        Task<string> resultado = conteudo.ReadAsStringAsync();
                        res = resultado.Result;

                        dynamic Jperfil = JsonConvert.DeserializeObject(res);

                        var perfil = new PerfilModel()
                        {
                            EmailUser = Jperfil[0]["EmailUser"].ToString(),
                            Nome = Jperfil[0]["Nome"].ToString(),
                            Sobrenome = Jperfil[0]["Sobrenome"].ToString(),
                            PictureUrl = Jperfil[0]["PictureUrl"].ToString(),
                            AccountNumber = Jperfil[0]["AccountNumber"].ToString(),
                        };
                        if (Jperfil[0]["Aniversario"] != null)
                        {
                            perfil.Aniversario = Convert.ToDateTime(Jperfil[0]["Aniversario"].ToString());
                        }


                        return View(perfil);
                    }
                }
            }
            else
            {
                return View("Error");
            }

            */
        }

        [HttpPost]
        public async Task<ActionResult> BuscaEmail(string email)
        {
            if (Session["Email_User"] != null && Session["Email_User"].ToString() != "")
            {
                if (email == Session["Email_User"].ToString())
                {
                    return View();
                }
            }

            var emailPerfil = new Dictionary<string, string>
                {
                    {"",email }
                };
            var perfilAConsultar = new FormUrlEncodedContent(emailPerfil);

            using (var Requisicao = new HttpClient())
            {
                Requisicao.BaseAddress = new Uri("http://localhost:49703");
                //Requisicao.DefaultRequestHeaders.Accept.Clear();
                //Requisicao.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", $"{access_token}");
                var Resposta = Requisicao.PostAsync("api/Perfil", perfilAConsultar).Result;

                string res = "";


                using (HttpContent conteudo = Resposta.Content)
                {
                    ViewBag.Mensagem = "";
                    Task<string> resultado = conteudo.ReadAsStringAsync();
                    res = resultado.Result;
                    if (res != "[]")
                    {
                        dynamic Jperfil = JsonConvert.DeserializeObject(res);

                        var perfil = new PerfilModel()
                        {
                            EmailUser = Jperfil[0]["EmailUser"].ToString(),
                            Nome = Jperfil[0]["Nome"].ToString(),
                            Sobrenome = Jperfil[0]["Sobrenome"].ToString(),
                            PictureUrl = Jperfil[0]["PictureUrl"].ToString(),
                            AccountNumber = Jperfil[0]["AccountNumber"].ToString(),
                        };
                        if (Jperfil[0]["Aniversario"] != null)
                        {
                            perfil.Aniversario = Convert.ToDateTime(Jperfil[0]["Aniversario"].ToString());
                        }

                        if (Session["Email_User"] != null && Session["Email_User"].ToString() != "")
                        {
                            var parametro = string.Format("api/Amigos/?email={0}", Session["Email_User"].ToString());
                            var AmigosAPI = await Requisicao.GetAsync(parametro);
                            var listadeamigos = await AmigosAPI.Content.ReadAsStringAsync();
                            dynamic jLista = JsonConvert.DeserializeObject(listadeamigos);

                            foreach (var item in jLista)
                            {
                                if (perfil.EmailUser.Replace(" ",string.Empty) == item.ToString())
                                {
                                    ViewBag.Amigo = true;
                                }
                            }

                            parametro = string.Format("api/ConvitesPendentes/?email={0}", Session["Email_User"].ToString());
                            AmigosAPI = Requisicao.GetAsync(parametro).Result;
                            listadeamigos = await AmigosAPI.Content.ReadAsStringAsync();
                            //var Jsolicitacao = JsonConvert.DeserializeObject(listadeamigos);
                            dynamic JAmigos = JsonConvert.DeserializeObject<List<SolicitacaoAmizade>>(listadeamigos);
                            foreach (var item in JAmigos)
                            {
                                if (Session["Email_User"].ToString() == item.SolicitacaoEnviada)
                                {
                                    ViewBag.Amigo = true;
                                }
                                if (Session["Email_User"].ToString() == item.SolicitacaoRecebida)
                                {
                                    ViewBag.Amigo = true;
                                }

                            }

                        }
                       
                        return View(perfil);
                    }
                    else
                    {
                        ViewBag.Mensagem = "Nada foi encontrado";
                        return View();
                    }
                }

            }
        }
    }

}