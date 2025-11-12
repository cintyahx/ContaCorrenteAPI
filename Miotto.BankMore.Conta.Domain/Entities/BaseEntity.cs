using Miotto.BankMore.Conta.Domain.Interfaces;

namespace Miotto.BankMore.Conta.Domain.Entities
{
    public abstract class BaseEntity : IEntity
    {
        public Guid Id { get; protected set; }
        public bool IsActive { get; set; } = true;

        private DateTime? _createAt;
        public DateTime? CreateAt
        {
            get { return _createAt; }
            set { _createAt = value == null ? DateTime.UtcNow : value; }
        }

        public DateTime? UpdateAt { get; set; }
    }
}
