using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Miotto.BankMore.Conta.App.Services
{
    public class JwtService : IJwtService
    {
        private readonly string _key;

        public JwtService(string key)
        {
            _key = key;
        }

        public string GenerateToken(Guid contaCorrenteId)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, contaCorrenteId.ToString())
            };

            var credentials = BuildCredentials();
            var tokenDescriptor = CreateSecurityToken(claims, credentials);
            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }

        private JwtSecurityToken CreateSecurityToken(List<Claim> claims, SigningCredentials credentials)
        {
            return new JwtSecurityToken(
                null,
                null,
                claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: credentials
            );
        }

        private SigningCredentials BuildCredentials()
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
            return credentials;
        }
    }
}
