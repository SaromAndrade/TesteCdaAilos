using MediatR;
using Questao5.Domain.Entities;

namespace Questao5.Application.Commands.Requests
{
    public class CriarMovimentoCommand : IRequest<Result<Guid>>
    {
        public Guid ChaveIdempotencia { get; set; }
        public int NumeroContaCorrente { get; set; }
        public decimal Valor { get; set; }
        public char TipoMovimento { get; set; }
    }
}
