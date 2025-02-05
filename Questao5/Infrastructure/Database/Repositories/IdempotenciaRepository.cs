using Dapper;
using Questao5.Domain.Abstractions;
using Questao5.Domain.Entities;

namespace Questao5.Infrastructure.Database.Repositories
{
    public class IdempotenciaRepository : IIdempotenciaRepository
    {
        private readonly DatabaseContext _databaseContext;
        private readonly ILogger<IdempotenciaRepository> _logger;

        public IdempotenciaRepository(DatabaseContext databaseContext, ILogger<IdempotenciaRepository> logger)
        {
            _databaseContext = databaseContext;
            _logger = logger;

        }
        public async Task<IEnumerable<Idempotencia>> GetAllAsync()
        {
            try
            {
                using var connection = _databaseContext.CreateConnection();
                return await connection.QueryAsync<Idempotencia>("SELECT * FROM idempotencia");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar todos os registros de idempotencia.");
                throw;
            }
        }

        public async Task InsertAsync(Idempotencia idempotencia)
        {
            try
            {
                string query = @"
                INSERT INTO idempotencia (chave_idempotencia, requisicao, resultado)
                VALUES (@ChaveIdempotencia, @Requisicao, @Resultado);";
                using var connection = _databaseContext.CreateConnection();
                await connection.ExecuteAsync(query, new { idempotencia.ChaveIdempotencia, idempotencia.Requisicao, idempotencia.Resultado });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao inserir registro de idempotência. Chave: {ChaveIdempotencia}", idempotencia.ChaveIdempotencia);
                throw;
            }
        }
        public async Task UpdateAsync(Idempotencia idempotencia)
        {
            try
            {
                string query = @"
                UPDATE idempotencia
                SET Requisicao = @Requisicao, Resultado = @Resultado
                WHERE ChaveIdempotencia = @ChaveIdempotencia;";

                using var connection = _databaseContext.CreateConnection();
                await connection.ExecuteAsync(query, new
                {
                    idempotencia.ChaveIdempotencia,
                    idempotencia.Requisicao,
                    idempotencia.Resultado
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar registro de idempotência. Chave: {ChaveIdempotencia}", idempotencia.ChaveIdempotencia);
                throw;
            }
        }
        public async Task<Idempotencia> GetByIdAsync(Guid chaveIdempotencia)
        {
            try
            {
                string query = @"
                SELECT 
                    chave_idempotencia AS ChaveIdempotenciaString, 
                    requisicao, 
                    resultado
                    FROM idempotencia
                    WHERE chave_idempotencia = @ChaveIdempotencia;";

                using var connection = _databaseContext.CreateConnection();
                return await connection.QueryFirstOrDefaultAsync<Idempotencia>(
                    query,
                    new { ChaveIdempotencia = chaveIdempotencia.ToString("D").ToUpper()});
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar registro de idempotência. Chave: {ChaveIdempotencia}", chaveIdempotencia);
                throw;
            }
        }
        public async Task DeleteAsync(Guid chaveIdempotencia)
        {
            try
            {
                string query = @"
                DELETE FROM Idempotencia
                WHERE ChaveIdempotencia = @ChaveIdempotencia;";

                using var connection = _databaseContext.CreateConnection();
                await connection.ExecuteAsync(
                    query,
                    new { ChaveIdempotencia = chaveIdempotencia.ToString() });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao remover registro de idempotência. Chave: {ChaveIdempotencia}", chaveIdempotencia);
                throw;
            }
        }
    }
}
