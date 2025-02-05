using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Questao5.Application.Handlers;
using Questao5.Application.Queries.Requests;
using Questao5.Domain.Abstractions;
using Questao5.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Questao5.Test
{
    public class ConsultarSaldoQueryHandlerTests
    {
        private readonly IContaCorrenteRepository _contaCorrenteRepository;
        private readonly IMovimentoRepository _movimentoRepository;

        public ConsultarSaldoQueryHandlerTests()
        {
            _contaCorrenteRepository = Substitute.For<IContaCorrenteRepository>();
            _movimentoRepository = Substitute.For<IMovimentoRepository>();
        }

        [Fact]
        public async Task Handle_DeveRetornarFalhaParaContaInvalida()
        {
            // Arrange
            var request = new ConsultarSaldoQuery { NumeroContaCorrente = 123 };

            _contaCorrenteRepository.GetByNumeroAsync(request.NumeroContaCorrente)
                .Returns(Task.FromResult<ContaCorrente>(null));

            var handler = new ConsultarSaldoQueryHandler(_contaCorrenteRepository, _movimentoRepository);

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("INVALID_ACCOUNT", result.Error);
        }
        [Fact]
        public async Task Handle_DeveRetornarFalhaParaContaInativa()
        {
            // Arrange
            var request = new ConsultarSaldoQuery { NumeroContaCorrente = 123 };
            var contaCorrente = new ContaCorrente { IdContaCorrente = Guid.NewGuid().ToString(), Ativo = false, Numero= request.NumeroContaCorrente };

            _contaCorrenteRepository.GetByNumeroAsync(request.NumeroContaCorrente)
                .Returns(Task.FromResult(contaCorrente));

            var handler = new ConsultarSaldoQueryHandler(_contaCorrenteRepository, _movimentoRepository);

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("INACTIVE_ACCOUNT", result.Error);
        }
        [Fact]
        public async Task Handle_DeveRetornarSaldoCalculadoCorretamente()
        {
            // Arrange
            var request = new ConsultarSaldoQuery { NumeroContaCorrente = 12345 };
            var contaCorrente = new ContaCorrente { IdContaCorrente = Guid.NewGuid().ToString(), Ativo = true, Nome = "Teste", Numero = 12345 };
            var movimentos = new List<Movimento>
            {
                new Movimento { TipoMovimento = 'C', Valor = 1000.00m },
                new Movimento { TipoMovimento = 'D', Valor = 500.00m }
            };

            _contaCorrenteRepository.GetByNumeroAsync(request.NumeroContaCorrente)
                .Returns(Task.FromResult(contaCorrente));

            _movimentoRepository.GetByNumeroContaCorrenteAsync(request.NumeroContaCorrente)
                .Returns(Task.FromResult((IEnumerable<Movimento>)movimentos));

            var handler = new ConsultarSaldoQueryHandler(_contaCorrenteRepository, _movimentoRepository);

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(500.00m, result.Value.Saldo);
            Assert.Equal("Teste", result.Value.NomeTitular);
            Assert.Equal("12345", result.Value.NumeroConta);
        }
        [Fact]
        public async Task Handle_DeveLancarExcecaoQuandoErroOcorrer()
        {
            // Arrange
            var request = new ConsultarSaldoQuery { NumeroContaCorrente = 12345 };

            _contaCorrenteRepository.GetByNumeroAsync(request.NumeroContaCorrente)
                .Throws(new Exception("Erro de banco de dados"));

            var handler = new ConsultarSaldoQueryHandler(_contaCorrenteRepository, _movimentoRepository);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => handler.Handle(request, CancellationToken.None));
            Assert.Equal("Erro de banco de dados", exception.Message);
        }
    }
}
