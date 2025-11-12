namespace Miotto.BankMore.Conta.App.Commands
{
    public class NewTransactionRequest
    {
        public int NumeroContaDestino { get; set; }
        public string Tipo { get; set; }
        public decimal Valor { get; set; }
    }
}
