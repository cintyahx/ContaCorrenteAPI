using Microsoft.EntityFrameworkCore;
using Miotto.BankMore.Conta.App.Entities;
using Miotto.BankMore.Conta.Domain.Enums;
using Miotto.BankMore.Conta.Domain.Interfaces;

namespace Miotto.BankMore.Conta.Infra.Repositories
{
    public class ContaCorrenteRepository : RepositoryBase<ContaCorrente>, IContaCorrenteRepository
    {
        public ContaCorrenteRepository(BankMoreContext dbContext) : base(dbContext)
        {
        }

        public Task<ContaCorrente?> GetByCpfAsync(string cpf)
        {
            return Set.Where(x => x.Cpf == cpf && x.IsActive).FirstOrDefaultAsync();
        }

        public Task<ContaCorrente?> GetByIdAsync(Guid id)
        {
            return Set.Where(x => x.Id == id && x.IsActive).FirstOrDefaultAsync();
        }

        public Task<ContaCorrente?> GetByIdentificationAsync(string identification)
        {
            return Set.Where(x => (x.Cpf == identification || x.Numero == Convert.ToInt64(identification)) && x.IsActive).FirstOrDefaultAsync();
        }

        public Task<ContaCorrente?> GetByNumeroAsync(int numero)
        {
            return Set.Where(x => x.Numero == numero && x.IsActive).FirstOrDefaultAsync();
        }

        public async Task AtualizarSaldoAsync(ContaCorrente conta, decimal valor, TipoMovimento tipoMovimento)
        {
            if (tipoMovimento == TipoMovimento.Debito)
            {
                conta.Saldo -= valor;
            }
            else
            {
                conta.Saldo += valor;
            }

            await UpdateAsync(conta);
        }
    }
}
