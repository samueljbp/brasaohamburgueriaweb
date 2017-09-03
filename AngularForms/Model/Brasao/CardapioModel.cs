using System;
using System.Collections.Generic;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Security;
using Newtonsoft.Json;

namespace AngularForms.Model
{
    [Table("CLASSE_ITEM_CARDAPIO")]
    public class ClasseItemCardapio
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.None)]
        [Column("COD_CLASSE")]
        public int CodClasse { get; set; }

        [Required]
        [Column("DESCRICAO_CLASSE")]
        public String DescricaoClasse { get; set; }

        [Column("IMAGEM")]
        public String Imagem { get; set; }

        [Column("ORDEM_EXIBICAO")]
        public int OrdemExibicao { get; set; }

        [InverseProperty("Classe")]
        public virtual List<ItemCardapio> Itens { get; set; }
    }

    [Table("ITEM_CARDAPIO")]
    public class ItemCardapio
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.None)]
        [Column("COD_ITEM_CARDAPIO")]
        public int CodItemCardapio { get; set; }

        [Required]
        [Column("COD_CLASSE")]
        [ForeignKey("Classe")]
        public int CodClasse { get; set; }

        [Required]
        [Column("NOME")]
        public String Nome { get; set; }

        [Required]
        [Column("PRECO")]
        public double Preco { get; set; }

        public virtual ClasseItemCardapio Classe { get; set; }

        [InverseProperty("Item")]
        public virtual ComplementoItemCardapio Complemento { get; set; }

        [InverseProperty("Item")]
        public virtual List<ObservacaoProducaoPermitidaItemCardapio> ObservacoesPermitidas { get; set; }

        [InverseProperty("Item")]
        public virtual List<OpcaoExtraItemCardapio> ExtrasPermitidos { get; set; }

        [InverseProperty("ItemCardapio")]
        public virtual List<ItemPedido> ItensPedidos { get; set; }
    }

    [Table("COMPLEMENTO_ITEM_CARDAPIO")]
    public class ComplementoItemCardapio
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.None)]
        [Key, ForeignKey("Item")]
        [Column("COD_ITEM_CARDAPIO")]
        public int CodItemCardapio { get; set; }

        [Required]
        [Column("DESCRICAO")]
        public String DescricaoLonga { get; set; }

        [Column("IMAGEM")]
        public String Imagem { get; set; }

        [Column("ORDEM_EXIBICAO")]
        public int OrdemExibicao { get; set; }

        public virtual ItemCardapio Item { get; set; }
    }

    [Table("OBSERVACAO_PRODUCAO")]
    public class ObservacaoProducao
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.None)]
        [Key]
        [Column("COD_OBSERVACAO")]
        public int CodObservacao { get; set; }

        [Required]
        [Column("DESCRICAO_OBSERVACAO")]
        public String DescricaoObservacao { get; set; }

        [InverseProperty("ObservacaoProducao")]
        public virtual List<ObservacaoProducaoPermitidaItemCardapio> ItensAssociados { get; set; }

        [InverseProperty("Observacao")]
        public virtual List<ObservacaoItemPedido> ObservacoesPedidos { get; set; }
    }

    [Table("OBSERVACAO_PRODUCAO_PERMITIDA_ITEM_CARDAPIO")]
    public class ObservacaoProducaoPermitidaItemCardapio
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.None)]
        [Key, ForeignKey("Item")]
        [Column("COD_ITEM_CARDAPIO", Order=1)]
        public int CodItemCardapio { get; set; }

        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.None)]
        [Key, ForeignKey("ObservacaoProducao")]
        [Column("COD_OBSERVACAO", Order=2)]
        public int CodObservacao { get; set; }

        public virtual ObservacaoProducao ObservacaoProducao { get; set; }

        public virtual ItemCardapio Item { get; set; }
    }

    [Table("OPCAO_EXTRA")]
    public class OpcaoExtra
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.None)]
        [Key]
        [Column("COD_OPCAO_EXTRA")]
        public int CodOpcaoExtra { get; set; }

        [Required]
        [Column("DESCRICAO_OPCAO_EXTRA")]
        public String DescricaoOpcaoExtra { get; set; }

        [Required]
        [Column("PRECO")]
        public double Preco { get; set; }

        [InverseProperty("OpcaoExtra")]
        public virtual List<OpcaoExtraItemCardapio> ItensAssociados { get; set; }

        [InverseProperty("OpcaoExtra")]
        public virtual List<ExtraItemPedido> ExtrasPedidos { get; set; }
    }

    [Table("OPCAO_EXTRA_ITEM_CARDAPIO")]
    public class OpcaoExtraItemCardapio
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.None)]
        [Key, ForeignKey("Item")]
        [Column("COD_ITEM_CARDAPIO", Order = 1)]
        public int CodItemCardapio { get; set; }

        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.None)]
        [Key, ForeignKey("OpcaoExtra")]
        [Column("COD_OPCAO_EXTRA", Order = 2)]
        public int CodOpcaoExtra { get; set; }

        public virtual OpcaoExtra OpcaoExtra { get; set; }

        public virtual ItemCardapio Item { get; set; }
    }
}