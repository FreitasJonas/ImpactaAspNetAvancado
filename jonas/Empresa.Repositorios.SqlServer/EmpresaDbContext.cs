using Empresa.Dominio;
using Microsoft.EntityFrameworkCore;

namespace Empresa.Repositorios.SqlServer
{
    public class EmpresaDbContext : DbContext
    {
        public DbSet<Contato> Contatos { get; set; }

        public EmpresaDbContext(DbContextOptions options) : base(options)
        {
            Database.EnsureCreated();

        }
    }
}
