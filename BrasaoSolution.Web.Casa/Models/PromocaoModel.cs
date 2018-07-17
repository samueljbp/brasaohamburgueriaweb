using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BrasaoSolution.Casa.Model
{
    [Table("PROMOCAO_VENDA")]
    public class PromocaoVenda
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.None)]
        [Column("COD_PROMOCAO_VENDA")]
        public int CodPromocaoVenda { get; set; }

        [ForeignKey("Empresa")]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.None)]
        [Column("COD_EMPRESA")]
        public int? CodEmpresa { get; set; }

        [Required]
        [Column("DESCRICAO_PROMOCAO")]
        public string DescricaoPromocao { get; set; }

        [Required]
        [Column("DATA_INICIO")]
        public DateTime DataHoraInicio { get; set; }

        [Required]
        [Column("DATA_FIM")]
        public DateTime DataHoraFim { get; set; }

        [Required]
        [Column("COD_TIPO_APLICACAO_DESCONTO")]
        [ForeignKey("TipoAplicacaoDesconto")]
        public int CodTipoAplicacaoDesconto { get; set; }

        [Required]
        [Column("PERCENTUAL_DESCONTO")]
        public decimal PercentualDesconto { get; set; }

        [Required]
        [Column("PROMOCAO_ATIVA")]
        public bool PromocaoAtiva { get; set; }

        [InverseProperty("Promocoes")]
        public virtual TipoAplicacaoDescontoPromocao TipoAplicacaoDesconto { get; set; }

        [InverseProperty("PromocaoVenda")]
        public virtual List<ClasseItemCardapioPromocaoVenda> ClassesAssociadas { get; set; }

        [InverseProperty("PromocaoVenda")]
        public virtual List<ItemCardapioPromocaoVenda> ItensAssociados { get; set; }

        [InverseProperty("PromocaoVenda")]
        public virtual List<DiaSemanaPromocaoVenda> DiasAssociados { get; set; }

        public virtual Empresa Empresa { get; set; }
    }

    [Table("TIPO_APLICACAO_DESCONTO_PROMOCAO")]
    public class TipoAplicacaoDescontoPromocao
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.None)]
        [Column("COD_TIPO_APLICACAO_DESCONTO")]
        public int CodTipoAplicacaoDesconto { get; set; }

        [Required]
        [Column("DESCRICAO")]
        public string Descricao { get; set; }

        [InverseProperty("TipoAplicacaoDesconto")]
        public virtual List<PromocaoVenda> Promocoes { get; set; }
    }

    [Table("CLASSE_ITEM_CARDAPIO_PROMOCAO_VENDA")]
    public class ClasseItemCardapioPromocaoVenda
    {
        [ForeignKey("PromocaoVenda")]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.None)]
        [Column("COD_PROMOCAO_VENDA", Order = 1)]
        public int CodPromocaoVenda { get; set; }

        [ForeignKey("Classe")]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.None)]
        [Column("COD_CLASSE", Order = 2)]
        public int CodClasse { get; set; }

        public virtual ClasseItemCardapio Classe { get; set; }

        public virtual PromocaoVenda PromocaoVenda { get; set; }
    }

    [Table("ITEM_CARDAPIO_PROMOCAO_VENDA")]
    public class ItemCardapioPromocaoVenda
    {
        [ForeignKey("PromocaoVenda")]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.None)]
        [Column("COD_PROMOCAO_VENDA", Order = 1)]
        public int CodPromocaoVenda { get; set; }

        [ForeignKey("Item")]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.None)]
        [Column("COD_ITEM_CARDAPIO", Order = 2)]
        public int CodItemCardapio { get; set; }

        public virtual ItemCardapio Item { get; set; }

        public virtual PromocaoVenda PromocaoVenda { get; set; }
    }

    [Table("DIA_SEMANA_PROMOCAO_VENDA")]
    public class DiaSemanaPromocaoVenda
    {
        [ForeignKey("PromocaoVenda")]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.None)]
        [Column("COD_PROMOCAO_VENDA", Order = 1)]
        public int CodPromocaoVenda { get; set; }

        [Required]
        [Column("DIA_SEMANA", Order = 2)]
        public int DiaSemana { get; set; }

        public virtual PromocaoVenda PromocaoVenda { get; set; }
    }
}
