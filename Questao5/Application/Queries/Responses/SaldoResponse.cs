namespace Questao5.Application.Queries.Responses
{
    public class SaldoResponse
    {
        public string NumeroConta { get; set; }
        public string NomeTitular { get; set; }
        public DateTime DataHoraConsulta { get; set; }
        public decimal Saldo { get; set; }
    }
}
