using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BrasaoSolution.Casa.Model
{
    [Table("FUNCIONAMENTO_ESTABELECIMENTO")]
    public class FuncionamentoEstabelecimento
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.None)]
        [Column("DIA_SEMANA", Order=1)]
        public int DiaSemana { get; set; }

        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.None)]
        [Column("ABERTURA", Order = 2)]
        public string Abertura { get; set; }

        [ForeignKey("Empresa")]
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