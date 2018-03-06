﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;

namespace BrasaoHamburgueria.Model
{
    #region Promocao viewmodel

    public enum TipoAplicacaoDescontoEnum
    {
        DescontoPorItem = 1,
        DescontoPorClasse = 2
    }

    public class PromocaoVendaViewModel
    {
        public int CodPromocaoVenda { get; set; }
        public string DescricaoPromocao { get; set; }
        public DateTime DataHoraInicio { get; set; }
        public DateTime DataHoraFim { get; set; }
        public string DataInicio { get; set; }
        public string DataFim { get; set; }
        public string HoraInicio { get; set; }
        public string HoraFim { get; set; }
        public int CodTipoAplicacaoDesconto { get; set; }
        public string DescricaoTipoAplicacaoDesconto { get; set; }
        public decimal PercentualDesconto { get; set; }
        public bool PromocaoAtiva { get; set; }
        public List<ClasseItemCardapioPromocaoVendaViewModel> ClassesAssociadas { get; set; }
        public List<ItemCardapioPromocaoVendaViewModel> ItensAssociados { get; set; }
        public List<DiaSemanaViewModel> DiasAssociados { get; set; }
    }

    public class ClasseItemCardapioPromocaoVendaViewModel
    {
        public int CodPromocaoVenda { get; set; }
        public int CodClasse { get; set; }
        public string DescricaoClasse { get; set; }
    }

    public class ItemCardapioPromocaoVendaViewModel
    {
        public int CodPromocaoVenda { get; set; }
        public int CodItemCardapio { get; set; }
        public string Nome { get; set; }
    }

    public class TipoAplicacaoDescontoPromocaoViewModel
    {
        public int CodTipoAplicacaoDesconto { get; set; }
        public string Descricao { get; set; }
    }

    #endregion

    [Table("PROMOCAO_VENDA")]
    public class PromocaoVenda
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.None)]
        [Column("COD_PROMOCAO_VENDA")]
        public int CodPromocaoVenda { get; set; }

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
        [Key, ForeignKey("PromocaoVenda")]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.None)]
        [Column("COD_PROMOCAO_VENDA", Order = 1)]
        public int CodPromocaoVenda { get; set; }

        [Key, ForeignKey("Classe")]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.None)]
        [Column("COD_CLASSE", Order = 2)]
        public int CodClasse { get; set; }

        public virtual ClasseItemCardapio Classe { get; set; }

        public virtual PromocaoVenda PromocaoVenda { get; set; }
    }

    [Table("ITEM_CARDAPIO_PROMOCAO_VENDA")]
    public class ItemCardapioPromocaoVenda
    {
        [Key, ForeignKey("PromocaoVenda")]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.None)]
        [Column("COD_PROMOCAO_VENDA", Order = 1)]
        public int CodPromocaoVenda { get; set; }

        [Key, ForeignKey("Item")]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.None)]
        [Column("COD_ITEM_CARDAPIO", Order = 2)]
        public int CodItemCardapio { get; set; }

        public virtual ItemCardapio Item { get; set; }

        public virtual PromocaoVenda PromocaoVenda { get; set; }
    }

    [Table("DIA_SEMANA_PROMOCAO_VENDA")]
    public class DiaSemanaPromocaoVenda
    {
        [Key, ForeignKey("PromocaoVenda")]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.None)]
        [Column("COD_PROMOCAO_VENDA", Order = 1)]
        public int CodPromocaoVenda { get; set; }

        [Key]
        [Required]
        [Column("DIA_SEMANA", Order = 2)]
        public int DiaSemana { get; set; }

        public virtual PromocaoVenda PromocaoVenda { get; set; }
    }
}
