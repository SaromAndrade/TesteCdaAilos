using System.Globalization;

namespace Questao1
{
    class ContaBancaria {

        public int Numero { get; }
        public string Titular { get; set; }
        private double Saldo;
        private double TaxaSaque = 3.50;

        public ContaBancaria(int numero, string titular)
        {
            Numero = numero;
            Titular = titular;
            Saldo = 0.0;
        }
        public ContaBancaria(int numero, string titular, double depositoInicial) : this(numero, titular)
        {
            Deposito(depositoInicial);
        }
        public void Deposito(double quantia)
        {
            Saldo += quantia;
        }
        public void Saque(double quantia)
        {
            Saldo -= (quantia + TaxaSaque);
        }
        public void AlterarTitular(string novoTitular)
        {
            Titular = novoTitular;
        }
        public override string ToString()
        {
            return $"Conta {Numero}, Titular: {Titular}, Saldo: $ {Saldo.ToString("F2", CultureInfo.InvariantCulture)}";
        }
    }
}
