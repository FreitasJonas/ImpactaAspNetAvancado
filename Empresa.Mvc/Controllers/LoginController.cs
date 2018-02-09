using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Empresa.Mvc.ViewModels;
using Empresa.Repositorios.SqlServer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Empresa.Mvc.Controllers
{
    public class LoginController : Controller
    {
        private EmpresaDbContext _db;
        private IDataProtector _protectorProvider;
        private string _tipoAutenticacao;

        public LoginController(EmpresaDbContext _db, IDataProtectionProvider protectionProvider, IConfiguration configuration)
        {
            this._db = _db;
            _protectorProvider = protectionProvider.CreateProtector(configuration.GetSection("ChaveCriptografia").Value);
            _tipoAutenticacao = configuration.GetSection("TipoAuthenticacao").Value;
        }


        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(LoginViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }   
               
            var contato = _db.Contatos.Where(c => c.Email == viewModel.Email && _protectorProvider.Unprotect(c.Senha) == viewModel.Senha).SingleOrDefault();

            if (contato == null)
            {
                ModelState.AddModelError("", "Usuário/Senha incorreto");
                return View(viewModel);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, contato.Nome),
                new Claim(ClaimTypes.Email, contato.Email),

                new Claim(ClaimTypes.Role, "Admin"),
                new Claim(ClaimTypes.Role, "Vendedor"),
                new Claim("Contato", "Criar"),
            };

            var identidade = new ClaimsIdentity(claims, _tipoAutenticacao);
            var principal = new ClaimsPrincipal(identidade);
            HttpContext.Authentication.SignInAsync(_tipoAutenticacao, principal);
            
            return RedirectToAction("Index", "Login");
        }

        public IActionResult Logout()
        {
            HttpContext.Authentication.SignOutAsync(_tipoAutenticacao);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult AcessoNegado()
        {
            return View();
        }


    }
}
