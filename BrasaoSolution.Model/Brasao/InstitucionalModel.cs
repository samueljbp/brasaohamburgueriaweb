using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;

namespace BrasaoSolution.Model
{
    public class FuncionamentoEstabelecimentoViewModel
    {
        public int CodEmpresa { get; set; }
        public string NomeEmpresa { get; set; }
        public DateTime Abertura { get; set; }
        public string AberturaString { get; set; }
        public DateTime Fechamento { get; set; }
        public string FechamentoString { get; set; }
        public int DiaSemana { get; set; }
        public String DescricaoDiaSemana { get; set; }
        public bool TemDelivery { get; set; }
    }

    [Table("FUNCIONAMENTO_ESTABELECIMENTO")]
    public class FuncionamentoEstabelecimento
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.None)]
        [Column("DIA_SEMANA", Order=1)]
        public int DiaSemana { get; set; }

        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.None)]
        [Column("ABERTURA", Order = 2)]
        public string Abertura { get; set; }

        [Key, ForeignKey("Empresa")]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.None)]
        [Column("COD_EMPRESA", Order = 3)]
        public int CodEmpresa { get; set; }

        [Required]
        [Column("FECHAMENTO")]
        public string Fechamento { get; set; }

        [Required]
        [Column("TEM_DELIVERY")]
        public bool TemDelivery { get; set; }

        public virtual Empresa Empresa { get; set; }
    }
}