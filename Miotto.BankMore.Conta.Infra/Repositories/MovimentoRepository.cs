using Miotto.BankMore.Conta.App.Entities;
using Miotto.BankMore.Conta.Domain.Entities;
using Miotto.BankMore.Conta.Domain.Interfaces;

namespace Miotto.BankMore.Conta.Infra.Repositories
{
    public class MovimentoRepository : RepositoryBase<Movimento>, IMovimentoRepository
    {
        public MovimentoRepository(BankMoreContext tasksContext) : base(tasksContext)
        {
        }
    }
}
