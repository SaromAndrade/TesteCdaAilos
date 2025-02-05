using Dapper;
using Questao5.Domain.Abstractions;
using Questao5.Domain.Entities;

namespace Questao5.Infrastructure.Database.Repositories
{
    public class MovimentoRepository : IMovimentoRepository
    {
        private readonly DatabaseContext _databaseContext;
        private readonly ILogger<IdempotenciaRepository> _logger;

        public MovimentoRepository(DatabaseContext databaseContext, ILogger<IdempotenciaRepository> logger)
        {
            _databaseContext = databaseContext;
            _logger = logger;
        }
        public async Task InsertAsync(Movimento movimento)
        {
            try
            {
                string query = @"
                INSERT INTO Movimento (IdMovimento, IdContaCorrente, DataMovimento, TipoMovimento, Valor)
                VALUES (@IdMovimento, @IdContaCorrente, @DataMovimento, @TipoMovimento, @Valor);";

                using var connection = _databaseContext.CreateConnection();
                await connection.ExecuteAsync(query, new
                {
                    movimento.IdMovimento,
                    movimento.IdContaCorrente,
                    DataMovimento = movimento.DataMovimento.ToString("yyyy-MM-dd HH:mm:ss"),
                    movimento.TipoMovimento,
                    movimento.Valor
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao inserir movimento. IdMovimento: {IdMovimento}", movimento.IdMovimento);
                throw;
            }
        }
        public async Task UpdateAsync(Movimento movimento)
        {
            try
            {
                string query = @"
                UPDATE Movimento
                SET IdContaCorrente = @IdContaCorrente,
                    DataMovimento = @DataMovimento,
                    TipoMovimento = @TipoMovimento,
                    Valor = @Valor
                WHERE IdMovimento = @IdMovimento;";

                using var connection = _databaseContext.CreateConnection();
                await connection.ExecuteAsync(query, new
                {
                    movimento.IdMovimento,
                    movimento.IdContaCorrente,
                    DataMovimento = movimento.DataMovimento.ToString("yyyy-MM-dd HH:mm:ss"),
                    movimento.TipoMovimento,
                    movimento.Valor
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar movimento. IdMovimento: {IdMovimento}", movimento.IdMovimento);
                throw;
            }
        }
        public async Task<Movimento> GetByIdAsync(Guid idMovimento)
        {
            try
            {
                string query = @"
                SELECT * FROM Movimento
                WHERE IdMovimento = @IdMovimento;";

                using var connection = _databaseContext.CreateConnection();
                var resultado = await connection.QueryFirstOrDefaultAsync<Movimento>(
                    query,
                    new { IdMovimento = idMovimento.ToString() });

                if (resultado != null)
                {
                    // Converte a string DataMovimento de volta para DateTime
                    resultado.DataMovimento = DateTime.Parse(resultado.DataMovimento.ToString());
                }

                return resultado;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar movimento. IdMovimento: {IdMovimento}", idMovimento);
                throw;
            }
        }
        public async Task DeleteAsync(Guid idMovimento)
        {
            try
            {
                string query = @"
                DELETE FROM Movimento
                WHERE IdMovimento = @IdMovimento;";

                using var connection = _databaseContext.CreateConnection();
                await connection.ExecuteAsync(
                    query,
                    new { IdMovimento = idMovimento.ToString() });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao remover movimento. IdMovimento: {IdMovimento}", idMovimento);
                throw;
            }
        }
        public async Task<IEnumerable<Movimento>> GetByNumeroContaCorrenteAsync(int numeroContaCorrente)
        {
            try
            {
                string query = @"
                 SELECT 
                        idmovimento AS IdMovimentoString, 
                        idcontacorrente, 
                        datamovimento, 
                        tipomovimento, 
                        valor
                        FROM movimento
                        WHERE idcontacorrente = @NumeroContaCorrente;";

                using var connection = _databaseContext.CreateConnection();
                var resultados = await connection.QueryAsync<Movimento>(
                    query,
                    new { NumeroContaCorrente = numeroContaCorrente });

                // Converte as strings DataMovimento de volta para DateTime
                foreach (var movimento in resultados)
                {
                    movimento.DataMovimento = DateTime.Parse(movimento.DataMovimento.ToString());
                }

                return resultados;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar movimentos por conta corrente. NumeroContaCorrente: {NumeroContaCorrente}", numeroContaCorrente);
                throw;
            }
        }
    }
}
