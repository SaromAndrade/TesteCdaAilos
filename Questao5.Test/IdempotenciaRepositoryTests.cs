using Moq;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Questao5.Domain.Abstractions;
using Questao5.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Questao5.Test
{
    public class IdempotenciaRepositoryTests
    {
        private readonly IIdempotenciaRepository _mockRepository;

        public IdempotenciaRepositoryTests()
        {
            _mockRepository = Substitute.For<IIdempotenciaRepository>(); ;
        }

        [Fact]
        public async Task GetAllAsync_DeveRetornarListaDeIdempotencia_QuandoExistiremRegistros()
        {
            // Arrange
            var idempotenciasEsperadas = new List<Idempotencia>
            {
            new Idempotencia { ChaveIdempotencia = Guid.NewGuid(), Requisicao = "Req1", Resultado = "Res1" },
            new Idempotencia { ChaveIdempotencia = Guid.NewGuid(), Requisicao = "Req2", Resultado = "Res2" }
            };

            _mockRepository.GetAllAsync().Returns(idempotenciasEsperadas);

            // Act
            var resultado = await _mockRepository.GetAllAsync();

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(idempotenciasEsperadas.Count, resultado.Count());
            Assert.Equal(idempotenciasEsperadas, resultado);
        }
        [Fact]
        public async Task GetAllAsync_DeveRetornarListaVazia_QuandoNaoHouverRegistros()
        {
            // Arrange
            _mockRepository.GetAllAsync().Returns(new List<Idempotencia>());

            // Act
            var resultado = await _mockRepository.GetAllAsync();

            // Assert
            Assert.NotNull(resultado);
            Assert.Empty(resultado);
        }
        [Fact]
        public async Task GetAllAsync_DeveLancarExcecao_QuandoOcorrerErroNoBancoDeDados()
        {
            // Arrange
            _mockRepository.GetAllAsync().Throws(new Exception("Erro no banco de dados"));

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => _mockRepository.GetAllAsync());
            Assert.Equal("Erro no banco de dados", ex.Message);
        }
        [Fact]
        public async Task InsertAsync_DeveInserirRegistroComSucesso()
        {
            // Arrange
            var idempotencia = new Idempotencia
            {
                ChaveIdempotencia = Guid.NewGuid(),
                Requisicao = "Teste de Requisição",
                Resultado = "Teste de Resultado"
            };

            // Act
            await _mockRepository.InsertAsync(idempotencia);

            // Assert
            await _mockRepository.Received(1).InsertAsync(Arg.Is<Idempotencia>(i =>
                i.ChaveIdempotencia == idempotencia.ChaveIdempotencia &&
                i.Requisicao == idempotencia.Requisicao &&
                i.Resultado == idempotencia.Resultado
            ));
        }
        [Fact]
        public async Task InsertAsync_DeveLancarExcecao_QuandoOcorrerErroNoBancoDeDados()
        {
            // Arrange
            var idempotencia = new Idempotencia
            {
                ChaveIdempotencia = Guid.NewGuid(),
                Requisicao = "Teste de Requisição",
                Resultado = "Teste de Resultado"
            };

            _mockRepository.InsertAsync(Arg.Any<Idempotencia>())
                .Throws(new Exception("Erro no banco de dados"));

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => _mockRepository.InsertAsync(idempotencia));
            Assert.Equal("Erro no banco de dados", ex.Message);
        }
        [Fact]
        public async Task UpdateAsync_DeveAtualizarRegistroComSucesso()
        {
            // Arrange
            var idempotencia = new Idempotencia
            {
                ChaveIdempotencia = Guid.NewGuid(),
                Requisicao = "Requisição Atualizada",
                Resultado = "Resultado Atualizado"
            };

            // Act
            await _mockRepository.UpdateAsync(idempotencia);

            // Assert
            await _mockRepository.Received(1).UpdateAsync(Arg.Is<Idempotencia>(i =>
                i.ChaveIdempotencia == idempotencia.ChaveIdempotencia &&
                i.Requisicao == idempotencia.Requisicao &&
                i.Resultado == idempotencia.Resultado
            ));
        }
        [Fact]
        public async Task UpdateAsync_DeveLancarExcecao_QuandoOcorrerErroNoBancoDeDados()
        {
            // Arrange
            var idempotencia = new Idempotencia
            {
                ChaveIdempotencia = Guid.NewGuid(),
                Requisicao = "Requisição com erro",
                Resultado = "Resultado com erro"
            };

            _mockRepository.UpdateAsync(Arg.Any<Idempotencia>())
                .Throws(new Exception("Erro no banco de dados"));

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => _mockRepository.UpdateAsync(idempotencia));
            Assert.Equal("Erro no banco de dados", ex.Message);
        }
        [Fact]
        public async Task GetByIdAsync_DeveRetornarIdempotencia_QuandoRegistroExistir()
        {
            // Arrange
            var chaveIdempotencia = Guid.NewGuid();
            var idempotenciaEsperada = new Idempotencia
            {
                ChaveIdempotencia = chaveIdempotencia,
                Requisicao = "Requisição de Teste",
                Resultado = "Resultado de Teste"
            };

            _mockRepository.GetByIdAsync(chaveIdempotencia).Returns(idempotenciaEsperada);

            // Act
            var resultado = await _mockRepository.GetByIdAsync(chaveIdempotencia);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(idempotenciaEsperada.ChaveIdempotencia, resultado.ChaveIdempotencia);
            Assert.Equal(idempotenciaEsperada.Requisicao, resultado.Requisicao);
            Assert.Equal(idempotenciaEsperada.Resultado, resultado.Resultado);
        }
        [Fact]
        public async Task GetByIdAsync_DeveRetornarNulo_QuandoRegistroNaoExistir()
        {
            // Arrange
            var chaveIdempotencia = Guid.NewGuid();
            _mockRepository.GetByIdAsync(chaveIdempotencia).Returns((Idempotencia)null);

            // Act
            var resultado = await _mockRepository.GetByIdAsync(chaveIdempotencia);

            // Assert
            Assert.Null(resultado);
        }
        [Fact]
        public async Task GetByIdAsync_DeveLancarExcecao_QuandoOcorrerErroNoBancoDeDados()
        {
            // Arrange
            var chaveIdempotencia = Guid.NewGuid();
            _mockRepository.GetByIdAsync(Arg.Any<Guid>())
                .Throws(new Exception("Erro no banco de dados"));

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => _mockRepository.GetByIdAsync(chaveIdempotencia));
            Assert.Equal("Erro no banco de dados", ex.Message);
        }
        [Fact]
        public async Task DeleteAsync_DeveExcluirRegistro_QuandoChaveExistir()
        {
            // Arrange
            var chaveIdempotencia = Guid.NewGuid();

            // Act
            await _mockRepository.DeleteAsync(chaveIdempotencia);

            // Assert
            await _mockRepository.Received(1).DeleteAsync(chaveIdempotencia);
        }
        [Fact]
        public async Task DeleteAsync_DeveLancarExcecao_QuandoOcorrerErroNoBancoDeDados()
        {
            // Arrange
            var chaveIdempotencia = Guid.NewGuid();
            _mockRepository.DeleteAsync(Arg.Any<Guid>())
                .Throws(new Exception("Erro no banco de dados"));

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => _mockRepository.DeleteAsync(chaveIdempotencia));
            Assert.Equal("Erro no banco de dados", ex.Message);
        }
    }
}
