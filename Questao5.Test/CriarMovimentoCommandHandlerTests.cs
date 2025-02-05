using NSubstitute;
using Questao5.Application.Commands.Requests;
using Questao5.Application.Handlers;
using Questao5.Domain.Abstractions;
using Questao5.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Questao5.Test
{
    public class CriarMovimentoCommandHandlerTests
    {
        private readonly IContaCorrenteRepository _contaCorrenteRepository;
        private readonly IMovimentoRepository _movimentoRepository;
        private readonly IIdempotenciaRepository _idempotenciaRepository;

        public CriarMovimentoCommandHandlerTests()
        {
            _contaCorrenteRepository = Substitute.For<IContaCorrenteRepository>();
            _movimentoRepository = Substitute.For<IMovimentoRepository>();
            _idempotenciaRepository = Substitute.For<IIdempotenciaRepository>();
        }

        [Fact]
        public async Task Handle_DeveRetornarResultadoIdempotenteSeChaveIdempotenciaExistir()
        {
            // Arrange
            var request = new CriarMovimentoCommand
            {
                ChaveIdempotencia = Guid.NewGuid(),
                NumeroContaCorrente = 1,    
                Valor = 1000.00m,
                TipoMovimento = 'C'
            };
            var idempotencia = new Idempotencia
            {
                ChaveIdempotencia = request.ChaveIdempotencia,
                Resultado = Guid.NewGuid().ToString()
            };

            _idempotenciaRepository.GetByIdAsync(request.ChaveIdempotencia)
                .Returns(Task.FromResult(idempotencia));

            var handler = new CriarMovimentoCommandHandler(_contaCorrenteRepository, _idempotenciaRepository, _movimentoRepository);

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(Guid.Parse(idempotencia.Resultado), result.Value);
        }
        [Fact]
        public async Task Handle_DeveRetornarFalhaParaContaInvalida()
        {
            // Arrange
            var request = new CriarMovimentoCommand
            {
                ChaveIdempotencia = Guid.NewGuid(),
                NumeroContaCorrente = 1,
                Valor = 1000.00m,
                TipoMovimento = 'C'
            };

            _contaCorrenteRepository.GetByNumeroAsync(request.NumeroContaCorrente)
                .Returns(Task.FromResult<ContaCorrente>(null));

            var handler = new CriarMovimentoCommandHandler(_contaCorrenteRepository, _idempotenciaRepository, _movimentoRepository);

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
            var request = new CriarMovimentoCommand
            {
                ChaveIdempotencia = Guid.NewGuid(),
                NumeroContaCorrente = 1,
                Valor = 1000.00m,
                TipoMovimento = 'C'
            };
            var contaCorrente = new ContaCorrente { IdContaCorrente = Guid.NewGuid().ToString(), Ativo = false, Numero = request.NumeroContaCorrente };

            _contaCorrenteRepository.GetByNumeroAsync(request.NumeroContaCorrente)
                .Returns(Task.FromResult(contaCorrente));

            var handler = new CriarMovimentoCommandHandler(_contaCorrenteRepository, _idempotenciaRepository, _movimentoRepository);

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("INACTIVE_ACCOUNT", result.Error);
        }
        [Fact]
        public async Task Handle_DeveRetornarFalhaParaValorInvalido()
        {
            // Arrange
            var request = new CriarMovimentoCommand
            {
                ChaveIdempotencia = Guid.NewGuid(),
                NumeroContaCorrente = 1,
                Valor = -100.00m,
                TipoMovimento = 'C'
            };

            var contaCorrente = new ContaCorrente { IdContaCorrente = Guid.NewGuid().ToString(), Ativo = true, Numero = request.NumeroContaCorrente };

            _contaCorrenteRepository.GetByNumeroAsync(request.NumeroContaCorrente)
                .Returns(Task.FromResult(contaCorrente));

            var handler = new CriarMovimentoCommandHandler(_contaCorrenteRepository, _idempotenciaRepository, _movimentoRepository);

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("INVALID_VALUE", result.Error);
        }
        [Fact]
        public async Task Handle_DeveRetornarFalhaParaTipoMovimentoInvalido()
        {
            // Arrange
            var request = new CriarMovimentoCommand
            {
                ChaveIdempotencia = Guid.NewGuid(),
                NumeroContaCorrente = 1,
                Valor = 1000.00m,
                TipoMovimento = 'X'
            };

            var contaCorrente = new ContaCorrente { IdContaCorrente = Guid.NewGuid().ToString(), Ativo = true, Numero = request.NumeroContaCorrente };

            _contaCorrenteRepository.GetByNumeroAsync(request.NumeroContaCorrente)
                .Returns(Task.FromResult(contaCorrente));

            var handler = new CriarMovimentoCommandHandler(_contaCorrenteRepository, _idempotenciaRepository, _movimentoRepository);

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("INVALID_TYPE", result.Error);
        }
        [Fact]
        public async Task Handle_DeveCriarMovimentoComSucesso()
        {
            // Arrange
            var request = new CriarMovimentoCommand
            {
                ChaveIdempotencia = Guid.NewGuid(),
                NumeroContaCorrente = 1,
                Valor = 1000.00m,
                TipoMovimento = 'C'
            };

            var contaCorrente = new ContaCorrente { IdContaCorrente = Guid.NewGuid().ToString(), Ativo = true, Numero = request.NumeroContaCorrente };

            _contaCorrenteRepository.GetByNumeroAsync(request.NumeroContaCorrente)
                .Returns(Task.FromResult(contaCorrente));

            _idempotenciaRepository.GetByIdAsync(request.ChaveIdempotencia)
                .Returns(Task.FromResult<Idempotencia>(null));

            var handler = new CriarMovimentoCommandHandler(_contaCorrenteRepository, _idempotenciaRepository, _movimentoRepository);

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotEqual(Guid.Empty, result.Value);
            await _movimentoRepository.Received(1).InsertAsync(Arg.Any<Movimento>());
            await _idempotenciaRepository.Received(1).InsertAsync(Arg.Any<Idempotencia>());
        }
    }
}
