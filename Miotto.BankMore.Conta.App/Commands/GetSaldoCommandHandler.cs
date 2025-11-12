using MediatR;
using Miotto.BankMore.Conta.Domain.Interfaces;

namespace Miotto.BankMore.Conta.App.Commands
{
    public class GetSaldoCommandHandler : IRequestHandler<GetSaldoCommand, GetSaldoResult>
    {
        private readonly IContaCorrenteRepository _contaCorrenteRepository;

        public GetSaldoCommandHandler(IContaCorrenteRepository contaCorrenteRepository)
        {
            _contaCorrenteRepository = contaCorrenteRepository;
        }

        public async Task<GetSaldoResult> Handle(GetSaldoCommand request, CancellationToken cancellationToken)
        {
            var conta = await _contaCorrenteRepository.GetAsync(request.IdConta);

            if (conta is null)
                throw new InvalidOperationException("INVALID_ACCOUNT");

            if (!conta.IsActive)
                throw new InvalidOperationException("INACTIVE_ACCOUNT");

            var result = new GetSaldoResult()
            {
                NumeroConta = conta.Numero,
                NomeTitular = conta.Nome,
                DataConsulta = DateTime.Now,
                SaldoAtual = conta.Saldo
            };

            return result;
        }
    }
}
