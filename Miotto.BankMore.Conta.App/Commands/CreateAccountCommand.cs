using MediatR;

namespace Miotto.BankMore.Conta.App.Commands
{
    public record CreateAccountCommand(string Cpf, string Nome, string Senha) : IRequest<int>;
}
