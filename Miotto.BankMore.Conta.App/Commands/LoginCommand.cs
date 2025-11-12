using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Miotto.BankMore.Conta.App.Commands
{
    public record LoginCommand : IRequest<string?>
    {
        [Required]
        public required string Identification { get; set; }
        [Required]
        public required string Password { get; set; }
    }
}
