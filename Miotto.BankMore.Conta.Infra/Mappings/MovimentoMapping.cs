using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Miotto.BankMore.Conta.Domain.Entities;
using Miotto.BankMore.Conta.Domain.Enums;

namespace Miotto.BankMore.Conta.Infra.Mappings
{
    public class MovimentoMapping : IEntityTypeConfiguration<Movimento>
    {
        public void Configure(EntityTypeBuilder<Movimento> builder)
        {
            builder.Property(x => x.Id).IsRequired();
            builder.Property(x => x.Data).HasDefaultValue(DateOnly.FromDateTime(DateTime.Now));
            builder.Property(x => x.Valor).HasColumnType("DECIMAL(10, 2)");

            builder.Property(x => x.Tipo)
                .HasConversion(new EnumToStringConverter<TipoMovimento>())
                .IsRequired()
                .HasMaxLength(1);

            builder.Property(x => x.ContaCorrenteId).IsRequired();
            builder.HasOne(x => x.Conta)
                .WithMany(x => x.Movimentos)
                .HasForeignKey(x => x.ContaCorrenteId);
        }
    }
}
