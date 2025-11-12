using MediatR;
using Miotto.BankMore.Conta.App.Services;
using Miotto.BankMore.Conta.Domain;
using Miotto.BankMore.Conta.Domain.Interfaces;

namespace Miotto.BankMore.Conta.App.Commands
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, string?>
    {
        private readonly IJwtService _jwtService;
        private readonly IContaCorrenteRepository _contaCorrenteRepository;

        public LoginCommandHandler(IJwtService jwtService,
            IContaCorrenteRepository usuarioRepository)
        {
            _jwtService = jwtService;
            _contaCorrenteRepository = usuarioRepository;
        }

        public async Task<string?> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var conta = await _contaCorrenteRepository.GetByIdentificationAsync(request.Identification);

            if (conta is null)
                return string.Empty;

            bool valid = PasswordHasher.VerifyPassword(request.Password, conta.Senha, conta.Salt);

            if (!valid)
                return string.Empty;

            return _jwtService.GenerateToken(conta.Id);
        }
    }
}
