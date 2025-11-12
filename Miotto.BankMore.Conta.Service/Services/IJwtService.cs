namespace Miotto.BankMore.Conta.App.Services
{
    public interface IJwtService
    {
        string GenerateToken(Guid contaCorrenteId);
    }
}
