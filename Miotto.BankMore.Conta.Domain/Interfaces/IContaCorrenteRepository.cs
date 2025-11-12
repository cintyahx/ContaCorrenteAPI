using Miotto.BankMore.Conta.App.Entities;
using Miotto.BankMore.Conta.Domain.Enums;

namespace Miotto.BankMore.Conta.Domain.Interfaces
{
    public interface IContaCorrenteRepository : IRepository<ContaCorrente>
    {
        Task<ContaCorrente?> GetByIdentificationAsync(string identification);
        Task<ContaCorrente?> GetByCpfAsync(string cpf);
        Task<ContaCorrente?> GetByNumeroAsync(int numero);
        Task<ContaCorrente?> GetByIdAsync(Guid id);
        Task AtualizarSaldoAsync(ContaCorrente conta, decimal valor, TipoMovimento tipoMovimento);
    }
}
