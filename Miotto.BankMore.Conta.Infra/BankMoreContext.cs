using Microsoft.EntityFrameworkCore;
using Miotto.BankMore.Conta.Infra.Mappings;

namespace Miotto.BankMore.Conta.Infra
{
    public class BankMoreContext : DbContext
    {
        protected BankMoreContext() { }

        public BankMoreContext(DbContextOptions<BankMoreContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ContaCorrenteMapping());
            modelBuilder.ApplyConfiguration(new MovimentoMapping());

            base.OnModelCreating(modelBuilder);
        }
    }
}
