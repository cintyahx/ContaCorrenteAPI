using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Miotto.BankMore.Conta.App.Commands;
using System.Security.Claims;

namespace Miotto.BankMore.Conta.App.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/movimento")]
    public class MovimentoContaController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MovimentoContaController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Registra a movimentação de valores de uma conta
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> NewTransactionAsync([FromBody] NewTransactionRequest request)
        {
            var idConta = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var command = new NewTransactionCommand(Guid.Parse(idConta), request.NumeroContaDestino, request.Tipo, request.Valor);

            await _mediator.Send(command);

            return NoContent();
        }
    }
}
