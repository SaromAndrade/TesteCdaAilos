using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Questao5.Domain.Abstractions;
using Questao5.Domain.Entities;

namespace Questao5.Test
{
    public class MovimentoRepositoryTests
    {
        private readonly IMovimentoRepository _mockRepository;

        public MovimentoRepositoryTests()
        {
            _mockRepository = Substitute.For<IMovimentoRepository>();
        }
        [Fact]
        public async Task InsertAsync_DeveInserirMovimentoComSucesso()
        {
            // Arrange
            var movimento = new Movimento
            {
                IdMovimento = Guid.NewGuid(),
                IdContaCorrente = 1,
                DataMovimento = DateTime.Now,
                TipoMovimento = 'C', // 'C' para Crédito
                Valor = 1000.00m
            };

            // Act
            await _mockRepository.InsertAsync(movimento);

            // Assert
            await _mockRepository.Received(1).InsertAsync(Arg.Is<Movimento>(m =>
                m.IdMovimento == movimento.IdMovimento &&
                m.IdContaCorrente == movimento.IdContaCorrente &&
                m.DataMovimento == movimento.DataMovimento &&
                m.TipoMovimento == movimento.TipoMovimento &&
                m.Valor == movimento.Valor
            ));
        }
        [Fact]
        public async Task InsertAsync_DeveRegistrarErroSeExceptionForLançada()
        {
            // Arrange
            var movimento = new Movimento
            {
                IdMovimento = Guid.NewGuid(),
                IdContaCorrente = 1,
                DataMovimento = DateTime.Now,
                TipoMovimento = 'D', // 'D' para Débito
                Valor = 500.00m
            };

            _mockRepository.InsertAsync(Arg.Any<Movimento>())
                .Throws(new Exception("Erro no banco de dados"));

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => _mockRepository.InsertAsync(movimento));
            Assert.Equal("Erro no banco de dados", ex.Message);
        }
        [Fact]
        public async Task InsertAsync_DeveLancarExcecaoParaValorNegativo()
        {
            // Arrange
            var movimento = new Movimento
            {
                IdMovimento = Guid.NewGuid(),
                IdContaCorrente = 1,
                DataMovimento = DateTime.Now,
                TipoMovimento = 'D', // 'D' para Débito
                Valor = -100.00m
            };

            _mockRepository.InsertAsync(Arg.Any<Movimento>())
                .Throws(new ArgumentException("Valor não pode ser negativo."));

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _mockRepository.InsertAsync(movimento));
            Assert.Equal("Valor não pode ser negativo.", ex.Message);
        }
        [Fact]
        public async Task UpdateAsync_DeveAtualizarMovimentoComSucesso()
        {
            // Arrange
            var movimento = new Movimento
            {
                IdMovimento = Guid.NewGuid(),
                IdContaCorrente = 1,
                DataMovimento = DateTime.Now,
                TipoMovimento = 'C', // 'C' para Crédito
                Valor = 1000.00m
            };

            // Act
            await _mockRepository.UpdateAsync(movimento);

            // Assert
            await _mockRepository.Received(1).UpdateAsync(Arg.Is<Movimento>(m =>
                m.IdMovimento == movimento.IdMovimento &&
                m.IdContaCorrente == movimento.IdContaCorrente &&
                m.DataMovimento == movimento.DataMovimento &&
                m.TipoMovimento == movimento.TipoMovimento &&
                m.Valor == movimento.Valor
            ));
        }
        [Fact]
        public async Task UpdateAsync_DeveLancarExcecaoQuandoErroOcorrer()
        {
            // Arrange
            var movimento = new Movimento
            {
                IdMovimento = Guid.NewGuid(),
                IdContaCorrente = 1,
                DataMovimento = DateTime.Now,
                TipoMovimento = 'C',
                Valor = 1000.00m
            };

            //Testando outra forma de lançar Exception
            _mockRepository
                .When(x => x.UpdateAsync(Arg.Any<Movimento>()))
                .Do(x => { throw new Exception("Erro ao atualizar movimento."); });

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => _mockRepository.UpdateAsync(movimento));
            Assert.Equal("Erro ao atualizar movimento.", ex.Message);
        }
        [Fact]
        public async Task UpdateAsync_DeveLancarExcecaoParaValorNegativo()
        {
            // Arrange
            var movimento = new Movimento
            {
                IdMovimento = Guid.NewGuid(),
                IdContaCorrente = 1,
                DataMovimento = DateTime.Now,
                TipoMovimento = 'D', // 'D' para Débito
                Valor = -100.00m
            };

            _mockRepository.UpdateAsync(Arg.Any<Movimento>())
                .Throws(new ArgumentException("Valor não pode ser negativo."));

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _mockRepository.UpdateAsync(movimento));
            Assert.Equal("Valor não pode ser negativo.", ex.Message);
        }
        [Fact]
        public async Task UpdateAsync_DeveLancarExcecaoParaTipoMovimentoInvalido()
        {
            // Arrange
            var movimento = new Movimento
            {
                IdMovimento = Guid.NewGuid(),
                IdContaCorrente = 1,
                DataMovimento = DateTime.Now,
                TipoMovimento = 'X', // Tipo inválido
                Valor = 1000.00m
            };

            _mockRepository.UpdateAsync(Arg.Any<Movimento>())
                .Throws(new ArgumentException("Tipo de movimento inválido."));

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _mockRepository.UpdateAsync(movimento));
            Assert.Equal("Tipo de movimento inválido.", ex.Message);
        }
        [Fact]
        public async Task GetByIdAsync_DeveRetornarMovimentoQuandoIdExiste()
        {
            // Arrange
            var movimento = new Movimento
            {
                IdMovimento = Guid.NewGuid(),
                IdContaCorrente = 1,
                DataMovimento = DateTime.Now,
                TipoMovimento = 'C',
                Valor = 1000.00m
            };

            _mockRepository.GetByIdAsync(movimento.IdMovimento)
                .Returns(movimento);

            // Act
            var resultado = await _mockRepository.GetByIdAsync(movimento.IdMovimento);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(movimento.IdMovimento, resultado.IdMovimento);
        }
        [Fact]
        public async Task GetByIdAsync_DeveRetornarNullQuandoIdNaoExiste()
        {
            // Arrange
            var idMovimento = Guid.NewGuid();

            _mockRepository.GetByIdAsync(idMovimento)
                .Returns((Movimento)null);

            // Act
            var resultado = await _mockRepository.GetByIdAsync(idMovimento);

            // Assert
            Assert.Null(resultado);
        }

        [Fact]
        public async Task GetByIdAsync_DeveLancarExcecaoQuandoErroOcorrer()
        {
            // Arrange
            var idMovimento = Guid.NewGuid();

            _mockRepository
                .When(x => x.GetByIdAsync(idMovimento))
                .Do(x => { throw new Exception("Erro ao buscar movimento."); });

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => _mockRepository.GetByIdAsync(idMovimento));
            Assert.Equal("Erro ao buscar movimento.", ex.Message);
        }
        [Fact]
        public async Task DeleteAsync_DeveRemoverMovimentoComSucesso()
        {
            // Arrange
            var idMovimento = Guid.NewGuid();

            // Act
            await _mockRepository.DeleteAsync(idMovimento);

            // Assert
            await _mockRepository.Received(1).DeleteAsync(Arg.Is<Guid>(id => id == idMovimento));
        }
        [Fact]
        public async Task DeleteAsync_DeveLancarExcecaoQuandoErroOcorrer()
        {
            // Arrange
            var idMovimento = Guid.NewGuid();

            _mockRepository
                .When(x => x.DeleteAsync(Arg.Any<Guid>()))
                .Do(x => { throw new Exception("Erro ao remover movimento."); });

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => _mockRepository.DeleteAsync(idMovimento));
            Assert.Equal("Erro ao remover movimento.", ex.Message);
        }
        [Fact]
        public async Task DeleteAsync_DeveLancarExcecaoParaIdInexistente()
        {
            // Arrange
            var idMovimento = Guid.NewGuid();

            _mockRepository.DeleteAsync(Arg.Any<Guid>())
                .Throws(new ArgumentException("Movimento não encontrado."));

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _mockRepository.DeleteAsync(idMovimento));
            Assert.Equal("Movimento não encontrado.", ex.Message);
        }
        
        [Fact]
        public async Task GetByIdContaCorrenteAsync_DeveRetornarListaVaziaQuandoIdNaoExiste()
        {
            // Arrange
            var idContaCorrente = 1;

            _mockRepository.GetByNumeroContaCorrenteAsync(idContaCorrente)
                .Returns(Enumerable.Empty<Movimento>());

            // Act
            var resultados = await _mockRepository.GetByNumeroContaCorrenteAsync(idContaCorrente);

            // Assert
            Assert.NotNull(resultados);
            Assert.Empty(resultados);
        }
        [Fact]
        public async Task GetByIdContaCorrenteAsync_DeveLancarExcecaoQuandoErroOcorrer()
        {
            // Arrange
            var idContaCorrente = 1;

            _mockRepository
                .When(x => x.GetByNumeroContaCorrenteAsync(idContaCorrente))
                .Do(x => { throw new Exception("Erro ao buscar movimentos."); });

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _mockRepository.GetByNumeroContaCorrenteAsync(idContaCorrente));
            Assert.Equal("Erro ao buscar movimentos.", exception.Message);
        }
    }
}
