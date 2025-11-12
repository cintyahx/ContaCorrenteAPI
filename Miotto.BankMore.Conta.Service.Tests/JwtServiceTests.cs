using Microsoft.IdentityModel.Tokens;
using Miotto.BankMore.Conta.App.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Miotto.BankMore.Conta.Service.Tests.Services
{
    public class JwtServiceTests
    {
        [Fact]
        public void GenerateToken_ContainsNameIdentifierClaim()
        {
            // Arrange
            var key = "test_secret_key_123!";
            var service = new JwtService(key);
            var expectedId = Guid.NewGuid();

            // Act
            var token = service.GenerateToken(expectedId);
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
            var claim = jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

            // Assert
            Assert.NotNull(claim);
            Assert.Equal(expectedId.ToString(), claim!.Value);
        }

        [Fact]
        public void GenerateToken_ValidatesWithSameKey()
        {
            // Arrange
            var key = "another_secret_key_456!";
            var service = new JwtService(key);
            var expectedId = Guid.NewGuid();
            var token = service.GenerateToken(expectedId);

            var handler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false, // lifetime is not relevant for signature test
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
            };

            // Act
            var principal = handler.ValidateToken(token, validationParameters, out var validatedToken);

            // Assert
            Assert.NotNull(principal);
            var nameId = principal.FindFirst(ClaimTypes.NameIdentifier);
            Assert.NotNull(nameId);
            Assert.Equal(expectedId.ToString(), nameId!.Value);
            Assert.IsType<JwtSecurityToken>(validatedToken);
        }

        [Fact]
        public void GenerateToken_InvalidWithDifferentKey_ThrowsSignatureException()
        {
            // Arrange
            var key1 = "key_one_!@#";
            var key2 = "different_key_two_$%^";
            var service = new JwtService(key1);
            var token = service.GenerateToken(Guid.NewGuid());

            var handler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key2))
            };

            // Act & Assert
            Assert.Throws<Microsoft.IdentityModel.Tokens.SecurityTokenInvalidSignatureException>(() =>
                handler.ValidateToken(token, validationParameters, out _));
        }

        [Fact]
        public void GenerateToken_ExpirationApproximatelyOneHour()
        {
            // Arrange
            var key = "expiry_test_key_789!";
            var service = new JwtService(key);
            var token = service.GenerateToken(Guid.NewGuid());

            // Act
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
            var timeUntilExpiry = jwt.ValidTo - DateTime.Now;

            // Assert: approx 60 minutes (allow small clock drift)
            Assert.InRange(timeUntilExpiry.TotalMinutes, 59, 61);
        }

        [Fact]
        public void GenerateToken_WithNullKey_ThrowsArgumentNullException()
        {
            // Arrange
            JwtService service = new JwtService(null!);

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => service.GenerateToken(Guid.NewGuid()));
        }
    }
}