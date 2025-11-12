using Miotto.BankMore.Conta.Domain;
using Miotto.BankMore.Conta.Domain.Entities;
using System.Text;

namespace Miotto.BankMore.Conta.App.Entities
{
    public class ContaCorrente : BaseEntity
    {
        public string Nome { get; set; }
        public int Numero { get; set; }
        public string Cpf { get; set; }
        public string Senha { get; set; }
        public string Salt { get; set; }
        public decimal Saldo { get; set; }

        public virtual IEnumerable<Movimento> Movimentos { get; set; }

        public ContaCorrente()
        {
            
        }

        public ContaCorrente(string nome, string cpf, string senha)
        {
            var(pass, salt) = PasswordHasher.CreatePasswordHash(senha);

            Nome = nome;
            Cpf = cpf;
            Senha = pass;
            Salt = salt;
            Saldo = 0;
        }
    }
}
