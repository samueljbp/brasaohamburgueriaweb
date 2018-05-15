using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;

namespace BrasaoHamburgueria.Model
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
        public const int COD_PARAMETRO_CASA_ABERTA = 3;
        public const int COD_PARAMETRO_TEMPO_MEDIO_ESPERA = 4;
        public const int COD_PARAMETRO_PORTA_IMPRESSORA_COZINHA = 5;
        public const int COD_PARAMETRO_IMPRIME_COMANDA_COZINHA = 6;
    }

    public static class Constantes
    {
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

    [Table("PARAMETRO_SISTEMA")]
    public class ParametroSistema
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.None)]
        [Column("COD_PARAMETRO", Order = 1)]
        public int CodParametro { get; set; }

        [Key, ForeignKey("Empresa")]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.None)]
        [Column("COD_EMPRESA", Order = 2)]
        public int? CodEmpresa { get; set; }

        [Required]
        [Column("DESCRICAO_PARAMETRO")]
        public string DescricaoParametro { get; set; }

        [Required]
        [Column("VALOR_PARAMETRO")]
        public string ValorParametro { get; set; }

        public virtual Empresa Empresa { get; set; }
    }
}