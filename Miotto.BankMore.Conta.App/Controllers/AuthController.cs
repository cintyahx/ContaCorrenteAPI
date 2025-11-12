using MediatR;
using Microsoft.AspNetCore.Mvc;
using Miotto.BankMore.Conta.App.Commands;

namespace Miotto.BankMore.Conta.App.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> LoginAsync([FromBody] LoginCommand command)
        {
            var token = await _mediator.Send(command);

            return string.IsNullOrWhiteSpace(token)
                        ? Unauthorized(new
                        {
                            message = "Usuário ou senha incorretos",
                            type = "USER_UNAUTHORIZED"
                        })
                        : Ok(new { token });
        }
    }
}
