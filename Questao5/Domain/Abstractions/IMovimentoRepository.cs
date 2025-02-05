using Questao5.Domain.Entities;

namespace Questao5.Domain.Abstractions
{
    public interface IMovimentoRepository
    {
        Task InsertAsync(Movimento movimento);
        Task UpdateAsync(Movimento movimento);
        Task<Movimento> GetByIdAsync(Guid idMovimento);
        Task DeleteAsync(Guid idMovimento);
        Task<IEnumerable<Movimento>> GetByNumeroContaCorrenteAsync(int numeroContaCorrente);
    }
}
