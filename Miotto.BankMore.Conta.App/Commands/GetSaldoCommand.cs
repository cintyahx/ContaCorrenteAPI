using MediatR;

namespace Miotto.BankMore.Conta.App.Commands
{
    public class GetSaldoCommand : IRequest<GetSaldoResult>
    {
        public required Guid IdConta { get; set; }
    }
}
