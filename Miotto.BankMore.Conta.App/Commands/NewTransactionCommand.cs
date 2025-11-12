using MediatR;

namespace Miotto.BankMore.Conta.App.Commands
{
    public record NewTransactionCommand(Guid IdContaToken, int NumeroContaDestino, string Tipo, decimal Valor) : IRequest;
}
