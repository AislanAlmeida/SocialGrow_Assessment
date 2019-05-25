using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using SolucaoLoginWEB.Models;

namespace SolucaoLoginWEB.Controllers
{
    public class RegistroController : Controller
    {
        // GET: Registro
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(RegistroModel model)
        {
            if (ModelState.IsValid)
            {
                using (var requisicao = new HttpClient())
                {
                    requisicao.BaseAddress = new Uri("http://localhost:49703");
                    
                    //AQUI PRECISO CONVERTER O NOME DOS CAMPOS PARA A API ACEITAR OS VALORES
                    var modelconvertido = new Dictionary<string, string>
                    {
                        {"Email",model.Email },
                        {"Password",model.Senha },
                        {"ConfirmPassword",model.ConfirmacaoSenha }
                    };

                    var resposta = await requisicao.PostAsJsonAsync("api/Account/Register", modelconvertido);

                    if (resposta.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Index", "Login");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Não foi possível cadastrar este email.");
                        return View();
                    }
                }
            }

            ModelState.AddModelError("", "Algo está errado, confira seus dados e tente novamente.");
            return View(model);
        }
    }
}