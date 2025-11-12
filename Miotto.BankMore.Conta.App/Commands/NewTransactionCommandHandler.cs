using MediatR;
using Miotto.BankMore.Conta.App.Entities;
using Miotto.BankMore.Conta.Domain.Entities;
using Miotto.BankMore.Conta.Domain.Enums;
using Miotto.BankMore.Conta.Domain.Interfaces;

namespace Miotto.BankMore.Conta.App.Commands
{
    public class NewTransactionCommandHandler : IRequestHandler<NewTransactionCommand>
    {
        private readonly IContaCorrenteRepository _contaCorrenteRepository;
        private readonly IMovimentoRepository _movimentoRepository;

        public NewTransactionCommandHandler(IContaCorrenteRepository contaCorrenteRepository,
            IMovimentoRepository movimentoRepository)
        {
            _contaCorrenteRepository = contaCorrenteRepository;
            _movimentoRepository = movimentoRepository;
        }

        public async Task Handle(NewTransactionCommand command, CancellationToken cancellationToken)
        {
            ContaCorrente? contaDestino;

            if (command.NumeroContaDestino > 0)
                contaDestino = await _contaCorrenteRepository.GetByNumeroAsync(command.NumeroContaDestino);
            else
                contaDestino = await _contaCorrenteRepository.GetByIdAsync(command.IdContaToken);

            if (contaDestino is null)
                throw new InvalidOperationException("INVALID_ACCOUNT");

            var tipoMovimento = Enum.Parse<TipoMovimento>(command.Tipo);

            if (tipoMovimento == TipoMovimento.Debito && !command.NumeroContaDestino.Equals(contaDestino.Numero))
                throw new InvalidOperationException("INVALID_TYPE");

            var movimento = new Movimento()
            {
                Tipo = tipoMovimento,
                Valor = command.Valor,
                ContaCorrenteId = contaDestino.Id,

            };

            await _movimentoRepository.CreateAsync(movimento);
            await _contaCorrenteRepository.AtualizarSaldoAsync(contaDestino, command.Valor, tipoMovimento);
        }
    }
}
