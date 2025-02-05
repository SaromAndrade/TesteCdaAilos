using Questao5.Domain.Entities;

namespace Questao5.Domain.Abstractions
{
    public interface IIdempotenciaRepository
    {
        Task<IEnumerable<Idempotencia>> GetAllAsync();
        Task InsertAsync(Idempotencia idempotencia);
        Task UpdateAsync(Idempotencia idempotencia);
        Task<Idempotencia> GetByIdAsync(Guid chaveIdempotencia);
        Task DeleteAsync(Guid chaveIdempotencia);
    }
}
