using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Miotto.BankMore.Conta.App.Entities;

namespace Miotto.BankMore.Conta.Infra.Mappings
{
    public class ContaCorrenteMapping : IEntityTypeConfiguration<ContaCorrente>
    {
        public void Configure(EntityTypeBuilder<ContaCorrente> builder)
        {
            builder.Property(x => x.Id).IsRequired();
            builder.Property(x => x.Numero).HasMaxLength(10).ValueGeneratedOnAdd();
            builder.Property(x => x.Nome).HasMaxLength(100);
            builder.Property(x => x.Cpf).HasMaxLength(11);
            builder.Property(x => x.Senha).HasMaxLength(100);
            builder.Property(x => x.Salt).HasMaxLength(100);
            builder.Property(x => x.Saldo).HasColumnType("DECIMAL(10, 2)");
        }
    }
}
