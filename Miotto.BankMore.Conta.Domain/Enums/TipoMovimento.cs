using System.ComponentModel;

namespace Miotto.BankMore.Conta.Domain.Enums
{
    public enum TipoMovimento
    {
        [Description("D")]
        Debito = 0,

        [Description("C")]
        Credito = 1
    }
}
