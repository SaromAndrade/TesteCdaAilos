using Questao5.Domain.Entities;

namespace Questao5.Domain.Abstractions
{
    public interface IContaCorrenteRepository
    {
        Task<IEnumerable<ContaCorrente>> GetAllAsync();
        Task<ContaCorrente> GetByIdAsync(string id);
        Task<ContaCorrente> GetByNumeroAsync(int numero);
    }
}
