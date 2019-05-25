using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SolucaoLoginWEB.Models;

namespace SolucaoLoginWEB.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var dados = new Dictionary<string,string>
                {
                    {"grant_type", "password" },
                    {"password",model.Senha },
                    {"username",model.Email }
                };

                using (var cliente = new HttpClient())
                {
                    cliente.BaseAddress = new Uri("http://localhost:49703");

                    using (var conteudoRequest = new FormUrlEncodedContent(dados))
                    {
                        var resposta = await cliente.PostAsync("/Token", conteudoRequest);

                        if (resposta.IsSuccessStatusCode)
                        {
                            var conteudoResposta = await resposta.Content.ReadAsStringAsync();
                            var tokenData = JObject.Parse(conteudoResposta);
                            Session.Add("access_token", tokenData["access_token"]);
                            Session.Add("Email_User", model.Email);

                            //---
                            var emailPerfil = new Dictionary<string, string>
                                {
                                    {"",model.Email }
                                };
                            var perfilAConsultar = new FormUrlEncodedContent(emailPerfil);
                            var teste = cliente.PostAsync("api/Perfil", perfilAConsultar).Result;
                            using (HttpContent conteudo = teste.Content)
                            {
                                Task<string> resultado = conteudo.ReadAsStringAsync();
                                var res = resultado.Result;
                                dynamic Jperfil = JsonConvert.DeserializeObject(res);
                                Session.Add("PictureUrl", Jperfil[0]["PictureUrl"].ToString());
                            }
                            //---
                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            ModelState.AddModelError("", "Tentativa de Login Inválida.");
                            return View(model);
                        }
                    }
                }
            }

            ModelState.AddModelError("", "Alguma coisa deu errado, confira seus dados e tente novamente.");
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> LogOut()
        {

            Session.Clear();
            Session.Add("Email_User", "");
            return RedirectToAction("Index", "Home");
        }


    }
}