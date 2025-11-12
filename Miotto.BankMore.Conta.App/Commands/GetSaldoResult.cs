namespace Miotto.BankMore.Conta.App.Commands
{
    public class GetSaldoResult
    {
        public int NumeroConta { get; set; }
        public string NomeTitular { get; set; }
        public DateTime DataConsulta { get; set; }
        public decimal SaldoAtual { get; set; }
    }
}
