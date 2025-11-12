using Miotto.BankMore.Conta.App.Entities;
using Miotto.BankMore.Conta.Domain.Enums;

namespace Miotto.BankMore.Conta.Domain.Entities
{
    public class Movimento : BaseEntity
    {
        public DateOnly Data { get; set; } = DateOnly.FromDateTime(DateTime.Now);
        public TipoMovimento Tipo { get; set; }
        public decimal Valor { get; set; }

        public virtual Guid ContaCorrenteId { get; set; }
        public virtual ContaCorrente Conta { get; set; }
    }
}
