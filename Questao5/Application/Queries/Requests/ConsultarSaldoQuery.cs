using MediatR;
using Questao5.Application.Queries.Responses;
using Questao5.Domain.Entities;

namespace Questao5.Application.Queries.Requests
{
    public class ConsultarSaldoQuery : IRequest<Result<SaldoResponse>>
    {
        public int NumeroContaCorrente { get; set; }
    }
}
