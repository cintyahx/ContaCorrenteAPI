using MediatR;

namespace Miotto.BankMore.Conta.App.Commands
{
    public record DeleteAccountCommand(Guid Id) : IRequest;
}
