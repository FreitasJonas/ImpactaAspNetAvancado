using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Empresa.Repositorios.SqlServer;
using Empresa.Dominio;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Empresa.Mvc.Controllers
{
    public class ContatosController : Controller
    {
        private EmpresaDbContext _db;
        private IDataProtector _protectorProvider;

        public ContatosController(EmpresaDbContext _db, IDataProtectionProvider protectionProvider, IConfiguration configuration)
        {
            this._db = _db;
            _protectorProvider = protectionProvider.CreateProtector(configuration.GetSection("ChaveCriptografia").Value);
        }
        
        // GET: /<controller>/
        public IActionResult Index()
        {
            return View(_db.Contatos.OrderBy(c => c.Nome).ToList());
        }

        [Authorize(Roles = "Admin, Corretor")] //Admin ou Corretor
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        //[Authorize(Roles = "Corretor")] //Admin e corretor
        [Authorize(Roles = "Admin")]
        public IActionResult Create(Contato contato)
        {
            var podeCriar = User.HasClaim("Contato", "Criar");
            
            if (!podeCriar)
            {
                RedirectToAction("AcessoNegado", "Login");
            }

            contato.Senha = _protectorProvider.Protect(contato.Senha);
            _db.Contatos.Add(contato);
            _db.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
