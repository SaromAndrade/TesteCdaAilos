namespace Questao5.Domain.Entities
{
    public class ContaCorrente
    {
        public string IdContaCorrente { get; set; }  // ID da conta corrente (GUID como string)
        public int Numero { get; set; }  // Número da conta corrente
        public string Nome { get; set; }  // Nome do titular da conta
        public bool Ativo { get; set; }  // Indica se a conta está ativa (0 = inativa, 1 = ativa)
    }
}
