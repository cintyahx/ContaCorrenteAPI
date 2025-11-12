using MediatR;
using Miotto.BankMore.Conta.Domain.Interfaces;

namespace Miotto.BankMore.Conta.App.Commands
{
    public class DeleteAccountCommandHandler : IRequestHandler<DeleteAccountCommand>
    {
        private readonly IContaCorrenteRepository _contaCorrenteRepository;

        public DeleteAccountCommandHandler(IContaCorrenteRepository contaCorrenteRepository)
        {
            _contaCorrenteRepository = contaCorrenteRepository;
        }

        public Task Handle(DeleteAccountCommand request, CancellationToken cancellationToken)
        {
            return _contaCorrenteRepository.DeleteAsync(request.Id);
        }
    }
}
