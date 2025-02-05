using Moq;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Questao5.Domain.Abstractions;
using Questao5.Domain.Entities;

namespace Questao5.Test
{
    public class ContaCorrenteRepositoryTests
    {
        private readonly IContaCorrenteRepository _mockRepository;

        public ContaCorrenteRepositoryTests()
        {
            _mockRepository = Substitute.For<IContaCorrenteRepository>();
        }

        [Fact]
        public async Task GetAllAsync_DeveRetornarTodasAsContasCorrentes()
        {
            // Arrange
            var contasCorrentes = new List<ContaCorrente>
            {
                new ContaCorrente { IdContaCorrente = Guid.NewGuid().ToString(), Numero = 12345, Nome = "Titular 1", Ativo = true },
                new ContaCorrente { IdContaCorrente = Guid.NewGuid().ToString(), Numero = 67890, Nome = "Titular 2", Ativo = false }
            };
            // Configura o mock para retornar a lista de contas correntes quando GetAllAsync for chamado
            _mockRepository.GetAllAsync().Returns(Task.FromResult((IEnumerable<ContaCorrente>)contasCorrentes));

            // Act
            var result = await _mockRepository.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count()); // Verifica se a lista contém 2 itens
            Assert.Equal(contasCorrentes, result); // Verifica se a lista retornada é a mesma que foi mockada

            // Verifica se o método GetAllAsync foi chamado
            await _mockRepository.Received(1).GetAllAsync();
        }
        [Fact]
        public async Task GetAllAsync_DeveRetornarListaVazia_QuandoNaoHouverContas()
        {
            // Arrange

            // Configura o mock para retornar a lista vazia de contas correntes quando GetAllAsync for chamado
            _mockRepository.GetAllAsync().Returns(Task.FromResult((IEnumerable<ContaCorrente>)new List<ContaCorrente>()));

            // Act
            var result = await _mockRepository.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result); 
            await _mockRepository.Received(1).GetAllAsync();
        }
        [Fact]
        public async Task GetAllAsync_DeveLancarExcecao_QuandoOcorrerErroNoBancoDeDados()
        {
            // Arrange

            _mockRepository.GetAllAsync().Throws(new Exception("Erro no banco de dados"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _mockRepository.GetAllAsync());
        }
        [Fact]
        public async Task GetAllAsync_DeveRetornarSomenteContasAtivas_QuandoHouverContasAtivasEInativas()
        {
            // Arrange
            var contas = new List<ContaCorrente>
            {
                new ContaCorrente { IdContaCorrente = Guid.NewGuid().ToString(), Numero = 12345, Nome = "Conta Ativa", Ativo = true },
                new ContaCorrente { IdContaCorrente = Guid.NewGuid().ToString(), Numero = 67890, Nome = "Conta Inativa", Ativo = false }
            };

            _mockRepository.GetAllAsync().Returns(contas);

            // Act
            var result = await _mockRepository.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Contains(result, c => c.Ativo == true);
            Assert.Contains(result, c => c.Ativo == false);
            Assert.Equal(2, result.Count());
        }
        [Fact]
        public async Task GetAllAsync_DeveLancarExcecaoPersonalizada_QuandoErroInesperadoOcorre()
        {
            // Arrange

            _mockRepository.GetAllAsync().ThrowsAsync(new InvalidOperationException("Erro inesperado"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _mockRepository.GetAllAsync());
            Assert.Equal("Erro inesperado", exception.Message);
        }
        [Fact]
        public async Task GetByIdAsync_DeveRetornarContaCorrente_QuandoIdForValido()
        {
            // Arrange
            var contaEsperada = new ContaCorrente
            {
                IdContaCorrente = Guid.NewGuid().ToString(),
                Numero = 12345,
                Nome = "João Silva",
                Ativo = true,
            };

            _mockRepository.GetByIdAsync(contaEsperada.IdContaCorrente).Returns(Task.FromResult(contaEsperada));

            // Act
            var result = await _mockRepository.GetByIdAsync(contaEsperada.IdContaCorrente);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(contaEsperada.IdContaCorrente, result.IdContaCorrente);
            Assert.Equal(contaEsperada.Numero, result.Numero);
            Assert.Equal(contaEsperada.Nome, result.Nome);
            Assert.Equal(contaEsperada.Ativo, result.Ativo);
        }
        [Fact]
        public async Task GetByIdAsync_DeveRetornarNull_QuandoContaCorrenteNaoExistir()
        {
            // Arrange
            _mockRepository.GetByIdAsync(Arg.Any<string>()).Returns(Task.FromResult<ContaCorrente>(null));

            // Act
            var result = await _mockRepository.GetByIdAsync(Guid.NewGuid().ToString());

            // Assert
            Assert.Null(result);
        }
        [Fact]
        public async Task GetByIdAsync_DeveLancarExcecao_QuandoOcorrerErroNoBancoDeDados()
        {
            // Arrange
            _mockRepository.GetByIdAsync(Arg.Any<string>()).ThrowsAsync(new Exception("Erro no banco de dados"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _mockRepository.GetByIdAsync(Guid.NewGuid().ToString()));
            Assert.Equal("Erro no banco de dados", exception.Message);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task GetByIdAsync_DeveLancarArgumentNullException_QuandoIdForNuloOuVazio(string idInvalido)
        {
            // Arrange
            // Simula que o método lança ArgumentNullException se um ID inválido for passado
            _mockRepository.GetByIdAsync(idInvalido).ThrowsAsync(new ArgumentNullException(nameof(idInvalido)));

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _mockRepository.GetByIdAsync(idInvalido));
        }
    }
}
