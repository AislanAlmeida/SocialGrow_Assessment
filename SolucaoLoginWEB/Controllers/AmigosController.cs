using Newtonsoft.Json;
using SolucaoLoginWEB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SolucaoLoginWEB.Controllers
{
    public class AmigosController : Controller
    {
        public async Task<ActionResult> AceitaConvite(string email)
        {
            SolicitacaoAmizade solicitacao = new SolicitacaoAmizade()
            {
                SolicitacaoRecebida = Session["Email_user"].ToString(),
                SolicitacaoEnviada = email.Replace(" ", string.Empty)
            };

            using (var requisicao = new HttpClient())
            {
                requisicao.BaseAddress = new Uri("http://localhost:49703");

                // var content = JsonConvert.SerializeObject(fotos);
                //var Resposta = requisicao.PostAsync("api/Galeria/FotosDoAlbum", perfilAConsultar).Result;
                var Resposta = await requisicao.PutAsJsonAsync("api/Amigos", solicitacao);
                //var Resposta = await requisicao.PostAsJsonAsync("api/Amigos", solicitacao);

                if (Resposta.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index","Amigos");
                }
                else
                {
                    return null;
                }
            }
        }

        public async Task<ActionResult> Index()
        {
            
            using (var requisicao = new HttpClient())
            {

                List<string> solicitacoes = new List<string>();
                List<string> amigos = new List<string>();

                List<PerfilModel> perfisSolicitacoes = new List<PerfilModel>();
                List<PerfilModel> perfisAmigos = new List<PerfilModel>();

                requisicao.BaseAddress = new Uri("http://localhost:49703");

                if (Session["Email_User"] != null)
                {
                    //preciso trazer todas as solicitações pendentes
                    var parametro = string.Format("api/ConvitesPendentes/?email={0}", Session["Email_User"].ToString());

                    var AmigosAPI = await requisicao.GetAsync(parametro);

                    var SAmigos = await AmigosAPI.Content.ReadAsStringAsync();
                    dynamic JAmigos = JsonConvert.DeserializeObject<List<SolicitacaoAmizade>>(SAmigos);
                    
                    foreach (var item in JAmigos)
                    {
                        if (Session["Email_User"].ToString() == item.SolicitacaoEnviada)
                        {
                            //solicitacoes.Add(item.SolicitacaoRecebida);
                        }
                        else
                        {

                            solicitacoes.Add(item.SolicitacaoEnviada);
                        }
                    }
                    
                    //preciso trazer todos os amigos que ja confirmaram
                    parametro = string.Format("api/Amigos/?email={0}", Session["Email_User"].ToString());

                    AmigosAPI = await requisicao.GetAsync(parametro);
                    SAmigos = await AmigosAPI.Content.ReadAsStringAsync();
                    JAmigos = JsonConvert.DeserializeObject<List<string>>(SAmigos);

                    foreach (var item in JAmigos)
                    {
                        amigos.Add(item);
                    }

                    //BUSCAR O PERFIL DE CADA EMAIL RECEBIDO - PRECISA SER MELHORADO
                    //-------------------------------------
                    
                    foreach (var item in solicitacoes)
                    {
                        var emailPerfil = new Dictionary<string, string>
                        {
                            {"",item }
                        };
                        var perfilAConsultar = new FormUrlEncodedContent(emailPerfil);

                        using (var Requisicao = new HttpClient())
                        {
                            Requisicao.BaseAddress = new Uri("http://localhost:49703");
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
                                    perfisSolicitacoes.Add(perfil);
                                }
                            }
                        }
                    }

                    foreach (var item in amigos)
                    {
                        var emailPerfil = new Dictionary<string, string>
                        {
                            {"",item }
                        };
                        var perfilAConsultar = new FormUrlEncodedContent(emailPerfil);

                        using (var Requisicao = new HttpClient())
                        {
                            Requisicao.BaseAddress = new Uri("http://localhost:49703");
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

                                    perfisAmigos.Add(perfil);
                                }
                            }
                        }
                    }

                    //-------------------------------------


                    ViewBag.Amigos = perfisAmigos;
                    ViewBag.Solicitacoes = perfisSolicitacoes;

                    return View();
                }
                else
                {
                    return RedirectToAction("Index", "Login");
                }

            }

        }

        [HttpPost]
        public async Task<ActionResult> Index(string email)
        {
            SolicitacaoAmizade solicitacao = new SolicitacaoAmizade()
            {
                SolicitacaoEnviada = Session["Email_user"].ToString(),
                SolicitacaoRecebida = email.Replace(" ", string.Empty)
            };
            using (var requisicao = new HttpClient())
            {
                requisicao.BaseAddress = new Uri("http://localhost:49703");

                // var content = JsonConvert.SerializeObject(fotos);
                //var Resposta = requisicao.PostAsync("api/Galeria/FotosDoAlbum", perfilAConsultar).Result;

                var Resposta = await requisicao.PostAsJsonAsync("api/Amigos", solicitacao);

                if (Resposta.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index","Amigos");
                }
                /*
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

                    return View();
                }
                */
            }
            return View();
        }
    }
}