using Microsoft.EntityFrameworkCore;
using WFConFin.Models;

namespace WFConFin.Dados
{
    public class WFConfinDBContext: DbContext
    {


        public WFConfinDBContext(DbContextOptions<WFConfinDBContext> options): base(options) { 
       
        
        
        }


        public DbSet<Cidade> Cidade { get; set; }
        public DbSet<Conta> Conta { get; set; }
        public DbSet<Estado> Estado { get; set; }
        public DbSet<Pessoa> Pessoa { get; set; }
        public DbSet<Usuario> Usuario { get; set; }














    }
}
