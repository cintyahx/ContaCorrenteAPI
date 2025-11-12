using MediatR;
using Miotto.BankMore.Conta.App.Entities;
using Miotto.BankMore.Conta.Domain.Interfaces;

namespace Miotto.BankMore.Conta.App.Commands
{
    public class CreateAccountCommandHandler : IRequestHandler<CreateAccountCommand, int>
    {
        private readonly IContaCorrenteRepository _contaCorrenteRepository;

        public CreateAccountCommandHandler(IContaCorrenteRepository contaCorrenteRepository)
        {
            _contaCorrenteRepository = contaCorrenteRepository;
        }

        public async Task<int> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
        {
            var conta = new ContaCorrente(request.Nome, request.Cpf, request.Senha);

            await _contaCorrenteRepository.CreateAsync(conta);
            return conta.Numero;
        }
    }
}
