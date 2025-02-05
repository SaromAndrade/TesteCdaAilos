using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Questao5.Application.Commands.Requests;
using Questao5.Application.Queries.Requests;
using Questao5.Application.Queries.Responses;

namespace Questao5.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovimentoController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MovimentoController(IMediator mediator)
        {
            _mediator = mediator;
        }
        /// <summary>
        /// Consulta o saldo da conta corrente.
        /// </summary>
        /// <param name="idContaCorrente">ID da conta corrente</param>
        /// <returns>Retorna o saldo da conta corrente</returns>
        [HttpGet("saldo/{numeroContaCorrente}")]
        [ProducesResponseType(typeof(SaldoResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ConsultarSaldo(int numeroContaCorrente)
        {
            var query = new ConsultarSaldoQuery { NumeroContaCorrente = numeroContaCorrente };
            var result = await _mediator.Send(query);
            if (!result.IsSuccess)
            {
                return BadRequest(result.Error);
            }

            return Ok(result.Value);
        }

        /// <summary>
        /// Cria um novo movimento na conta corrente.
        /// </summary>
        /// <param name="command">Dados do movimento a ser criado</param>
        /// <returns>Retorna o ID do movimento criado</returns>
        [HttpPost("movimento")]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CriarMovimento([FromBody] CriarMovimentoCommand command)
        {
            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
            {
                return BadRequest(result.Error);
            }

            return Ok(result.Value);
        }
    }
}
