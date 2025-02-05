using MediatR;
using Questao5.Application.Queries.Requests;
using Questao5.Application.Queries.Responses;
using Questao5.Domain.Abstractions;
using Questao5.Domain.Entities;

namespace Questao5.Application.Handlers
{
    public class ConsultarSaldoQueryHandler : IRequestHandler<ConsultarSaldoQuery, Result<SaldoResponse>>
    {
        private readonly IContaCorrenteRepository _contaCorrenteRepository;
        private readonly IMovimentoRepository _movimentoRepository;

        public ConsultarSaldoQueryHandler(IContaCorrenteRepository contaCorrenteRepository, IMovimentoRepository movimentoRepository)
        {
            _contaCorrenteRepository = contaCorrenteRepository;
            _movimentoRepository = movimentoRepository;
        }

        public async Task<Result<SaldoResponse>> Handle(ConsultarSaldoQuery request, CancellationToken cancellationToken)
        {
            var conta = await _contaCorrenteRepository.GetByNumeroAsync(request.NumeroContaCorrente);

            if (conta == null)
                return Result<SaldoResponse>.Failure("INVALID_ACCOUNT");

            if (!conta.Ativo)
                return Result<SaldoResponse>.Failure("INACTIVE_ACCOUNT");
            // Busca todos os movimentos da conta corrente
            var movimentos = await _movimentoRepository.GetByNumeroContaCorrenteAsync(request.NumeroContaCorrente);

            // Calcula o saldo
            decimal saldo = 0;
            foreach (var movimento in movimentos)
            {
                if (movimento.TipoMovimento == 'C') // Crédito
                    saldo += movimento.Valor;
                else if (movimento.TipoMovimento == 'D') // Débito
                    saldo -= movimento.Valor;
            }
            // Cria a resposta
            var response = new SaldoResponse
            {
                NumeroConta = conta.Numero.ToString(),
                NomeTitular = conta.Nome,
                DataHoraConsulta = DateTime.UtcNow,
                Saldo = saldo
            };
            return Result<SaldoResponse>.Success(response);
        }
    }
}
