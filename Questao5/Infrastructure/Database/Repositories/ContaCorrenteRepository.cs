using Dapper;
using Questao5.Domain.Abstractions;
using Questao5.Domain.Entities;

namespace Questao5.Infrastructure.Database.Repositories
{
    public class ContaCorrenteRepository : IContaCorrenteRepository
    {
        private readonly DatabaseContext _databaseContext;
        private readonly ILogger<ContaCorrenteRepository> _logger;

        public ContaCorrenteRepository(DatabaseContext databaseContext, ILogger<ContaCorrenteRepository> logger)
        {
            _databaseContext = databaseContext;
            _logger = logger;
        }
        public async Task<IEnumerable<ContaCorrente>> GetAllAsync()
        {
            try
            {
                using var connection = _databaseContext.CreateConnection();
                return await connection.QueryAsync<ContaCorrente>("SELECT * FROM contacorrente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao GetAllAsync");
                throw;
            }
        }
        public async Task<ContaCorrente> GetByIdAsync(string id)
        {
            try
            {
                using var connection = _databaseContext.CreateConnection();
                return await connection.QueryFirstOrDefaultAsync<ContaCorrente>(
                    "SELECT * FROM contacorrente WHERE idcontacorrente = @Id",
                    new { Id = id }
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar registro de contacorrente. Chave: {id}", id);
                throw;
            }
        }
        public async Task<ContaCorrente> GetByNumeroAsync(int numero)
        {
            try
            {
                using var connection = _databaseContext.CreateConnection();
                return await connection.QueryFirstOrDefaultAsync<ContaCorrente>(
                    "SELECT * FROM contacorrente WHERE numero = @Numero",
                    new { Numero = numero }
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar registro de contacorrente. Número: {numero}", numero);
                throw;
            }
        }

    }
}
