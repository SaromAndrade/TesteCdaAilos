namespace Questao5.Domain.Entities
{
    public class Movimento
    {
        public Guid IdMovimento { get; set; }
        public int IdContaCorrente { get; set; }
        public DateTime DataMovimento { get; set; }
        public char TipoMovimento { get; set; } // 'C' ou 'D' / C = Credito, D = Débito
        public decimal Valor { get; set; }
        public string IdMovimentoString
        {
            get => IdMovimento.ToString();
            set => IdMovimento = Guid.Parse(value);
        }
    }
}
