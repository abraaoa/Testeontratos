using Microsoft.EntityFrameworkCore;
using Contratos.Models;
namespace Contratos.Data
{
    public class DataContext: DbContext
    {

        public DataContext(DbContextOptions options) : base(options) { }
        public DbSet<Contrato> Contratos { get; set; }
        public DbSet<Prestacao> Prestacoes_Contrato { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Contrato>()
                .HasMany(c => c.Prestacoes)
                .WithOne(e => e.Contrato);
        }
    }
}
