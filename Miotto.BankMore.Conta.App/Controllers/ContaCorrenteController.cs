using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Miotto.BankMore.Conta.App.Commands;
using System.Security.Claims;

namespace Miotto.BankMore.Conta.App.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ContaCorrenteController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ContaCorrenteController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [AllowAnonymous]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateAccountAsync([FromBody] CreateAccountCommand command)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var id = await _mediator.Send(command);

            return Created(nameof(CreateAccountAsync), new { id });
        }

        /// <summary>
        /// Inativa a conta vinculada ao usuário logado
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteAsync()
        {
            var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var command = new DeleteAccountCommand(Guid.Parse(id));

            await _mediator.Send(command);

            return NoContent();
        }


        [HttpGet("saldo")]
        public async Task<IActionResult> ObterSaldo()
        {
            var id = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var command = new GetSaldoCommand()
            {
                IdConta = Guid.Parse(id)
            };

            var saldo = await _mediator.Send(command);

            return Ok(saldo);
        }
    }
}
