﻿namespace Questao5.Domain.Entities
{
    public class Idempotencia
    {
        public Guid ChaveIdempotencia { get; set; }
        public string Requisicao { get; set; }
        public string Resultado { get; set; }
        public string ChaveIdempotenciaString
        {
            get => ChaveIdempotencia.ToString();
            set => ChaveIdempotencia = Guid.Parse(value);
        }
    }
}
