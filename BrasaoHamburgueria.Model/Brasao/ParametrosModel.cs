using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;

namespace BrasaoHamburgueria.Model
{
    public static class CodigosParametros
    {
        public const int COD_PARAMETRO_TAXA_ENTREGA = 1;
    }

    public static class Constantes
    {
        public const string ROLE_ADMIN = "Administradores";
    }

    public class HorarioFuncionamento
    {
        public DateTime Abertura { get; set;  }
        public DateTime Fechamento { get; set; }
        public String DiaSemana { get; set; }
    }

    [Table("PARAMETRO_SISTEMA")]
    public class ParametrosSistema
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.None)]
        [Column("COD_PARAMETRO")]
        public int CodParametro { get; set; }

        [Required]
        [Column("DESCRICAO_PARAMETRO")]
        public string DescricaoParametro { get; set; }

        [Required]
        [Column("VALOR_PARAMETRO")]
        public string ValorParametro { get; set; }
    }
}