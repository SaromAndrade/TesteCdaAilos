using MediatR;
using Newtonsoft.Json;
using Questao5.Application.Commands.Requests;
using Questao5.Domain.Abstractions;
using Questao5.Domain.Entities;

namespace Questao5.Application.Handlers
{
    public class CriarMovimentoCommandHandler : IRequestHandler<CriarMovimentoCommand, Result<Guid>>
    {
        private readonly IContaCorrenteRepository _contaCorrenteRepository;
        private readonly IIdempotenciaRepository _idempotenciaRepository;
        private readonly IMovimentoRepository _movimentoRepository;
        public CriarMovimentoCommandHandler(IContaCorrenteRepository contaCorrenteRepository, IIdempotenciaRepository idempotenciaRepository, IMovimentoRepository movimentoRepository)
        {
            _contaCorrenteRepository = contaCorrenteRepository;
            _idempotenciaRepository = idempotenciaRepository;
            _movimentoRepository = movimentoRepository;
        }

        public async Task<Result<Guid>> Handle(CriarMovimentoCommand request, CancellationToken cancellationToken)
        {
            var idempotencia = await _idempotenciaRepository.GetByIdAsync(request.ChaveIdempotencia);
            if (idempotencia != null)
            {
                return Result<Guid>.Success(Guid.Parse(idempotencia.Resultado));
            }
            var conta = await _contaCorrenteRepository.GetByNumeroAsync(request.NumeroContaCorrente);

            if (conta == null)
                return Result<Guid>.Failure("INVALID_ACCOUNT");

            if (!conta.Ativo)
                return Result<Guid>.Failure("INACTIVE_ACCOUNT");

            if (request.Valor <= 0)
                return Result<Guid>.Failure("INVALID_VALUE");

            if (request.TipoMovimento != 'C' && request.TipoMovimento != 'D')
                return Result<Guid>.Failure("INVALID_TYPE");

            // Criar o movimento
            var movimento = new Movimento
            {
                IdMovimento = Guid.NewGuid(),
                IdContaCorrente = request.NumeroContaCorrente,
                DataMovimento = DateTime.UtcNow,
                TipoMovimento = request.TipoMovimento,
                Valor = request.Valor
            };
            await _movimentoRepository.InsertAsync(movimento);

            // Salva a requisição no banco para garantir idempotência
            var novoIdempotencia = new Idempotencia
            {
                ChaveIdempotencia = request.ChaveIdempotencia,
                Requisicao = JsonConvert.SerializeObject(request),
                Resultado = movimento.IdMovimento.ToString("D").ToUpper()
            };
            await _idempotenciaRepository.InsertAsync(novoIdempotencia);
            return Result<Guid>.Success(movimento.IdMovimento);
        }
    }
}
