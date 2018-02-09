using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Empresa.Mvc.ViewModels;
using Empresa.Repositorios.SqlServer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Empresa.Mvc.Controllers
{
    public class LoginController : Controller
    {
        private EmpresaDbContext _db;
        private IDataProtector _protectorProvider;

        public LoginController(EmpresaDbContext _db, IDataProtectionProvider protectionProvider, IConfiguration configuration)
        {
            this._db = _db;
            _protectorProvider = protectionProvider.CreateProtector(configuration.GetSection("ChaveCriptografia").Value);
        }


        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(LoginViewModel viewModel)
        {            
            var contato = _db.Contatos.Where(c => c.Email == viewModel.Email && _protectorProvider.Unprotect(c.Senha) == viewModel.Senha).SingleOrDefault();

            if (contato == null)
            {
                ModelState.AddModelError("", "Usuário/Senha incorreto");
                return View(viewModel);
            }
            else
            {
                                    
            }


            return RedirectToAction("Index", "Login");
        }
    }
}
