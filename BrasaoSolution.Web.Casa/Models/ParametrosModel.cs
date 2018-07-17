using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BrasaoSolution.Casa.Model
{
    [Table("PARAMETRO_SISTEMA")]
    public class ParametroSistema
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.None)]
        [Column("COD_PARAMETRO", Order = 1)]
        public int CodParametro { get; set; }

        [ForeignKey("Empresa")]
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