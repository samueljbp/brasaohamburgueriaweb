﻿namespace BrasaoSolution.ViewModel
{
    public class DiaSemanaViewModel
    {
        public DiaSemanaViewModel(int num, string nome)
        {
            this.NumDiaSemana = num;
            this.NomeDiaSemana = nome;
        }

        public DiaSemanaViewModel()
        {
        }

        public int NumDiaSemana { get; set; }
        public string NomeDiaSemana { get; set; }
    }

    public static class CodigosParametros
    {
        public const int COD_PARAMETRO_TAXA_ENTREGA = 1;
        public const int COD_PARAMETRO_CODIGO_IMPRESSORA_COMANDA = 2;
        //public const int COD_PARAMETRO_CASA_ABERTA = 3;
        public const int COD_PARAMETRO_TEMPO_MEDIO_ESPERA = 4;
        public const int COD_PARAMETRO_PORTA_IMPRESSORA_COZINHA = 5;
        public const int COD_PARAMETRO_IMPRIME_COMANDA_COZINHA = 6;
    }

    public static class Constantes
    {
        public const string ROLE_MASTER = "Master";
        public const string ROLE_ADMIN = "Administradores";
        public const string ROLE_COZINHA = "Cozinha";
        public const string ROLE_CLIENTE = "Clientes";
    }

    public class ParametroSistemaViewModel
    {
        public int? CodEmpresa { get; set; }
        public string NomeEmpresa { get; set; }
        public int CodParametro { get; set; }
        public string DescricaoParametro { get; set; }
        public string ValorParametro { get; set; }
    }
}