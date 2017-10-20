using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;

namespace BrasaoHamburgueria.Model
{
    public class DadosItemCardapioViewModel
    {
        public int CodItemCardapio { get; set; }
        public List<ObservacaoProducaoViewModel> Observacoes { get; set; }
        public List<OpcaoExtraViewModel> Extras { get; set; }
    }

    public class ClasseItemCardapioViewModel
    {
        public int CodClasse { get; set; }
        public string DescricaoClasse { get; set; }
        public bool Sincronizar { get; set; }
        public List<ItemCardapioViewModel> Itens { get; set; }
    }

    public class ItemCardapioViewModel
    {
        public int CodItemCardapio { get; set; }
        public int CodClasse { get; set; }
        public string Nome { get; set; }
        public double Preco { get; set; }
        public bool Ativo { get; set; }
        public bool Sincronizar { get; set; }
        public ComplementoItemCardapioViewModel Complemento { get; set; }
        public List<ObservacaoProducaoViewModel> ObservacoesPermitidas { get; set; }
        public List<OpcaoExtraViewModel> ExtrasPermitidos { get; set; }
    }

    public class ObservacaoProducaoViewModel
    {
        public int CodObservacao { get; set; }
        public string DescricaoObservacao { get; set; }
    }

    public class OpcaoExtraViewModel
    {
        public int CodOpcaoExtra { get; set; }
        public string DescricaoOpcaoExtra { get; set; }
        public double Preco { get; set; }
    }

    public class ComplementoItemCardapioViewModel
    {
        public string DescricaoLonga { get; set; }
        public string Imagem { get; set; }
    }

    [Table("CLASSE_ITEM_CARDAPIO")]
    public class ClasseItemCardapio
    {
        public ClasseItemCardapio()
        {
            Sincronizar = true;
        }

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

        [Column("SINCRONIZAR")]
        public bool Sincronizar { get; set; }

        [Column("COD_IMPRESSORA_PADRAO")]
        [ForeignKey("ImpressoraPadrao")]
        public int? CodImpressoraPadrao { get; set; }

        [InverseProperty("Classe")]
        public virtual List<ItemCardapio> Itens { get; set; }

        public virtual ImpressoraProducao ImpressoraPadrao { get; set; }
    }

    [Table("ITEM_CARDAPIO")]
    public class ItemCardapio
    {
        public ItemCardapio()
        {
            Ativo = true;
            Sincronizar = true;
        }

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

        [Required]
        [Column("ATIVO")]
        public bool Ativo { get; set; }

        [Column("SINCRONIZAR")]
        public bool Sincronizar { get; set; }

        public virtual ClasseItemCardapio Classe { get; set; }

        [InverseProperty("Item")]
        public virtual ComplementoItemCardapio Complemento { get; set; }

        [InverseProperty("Item")]
        public virtual List<ObservacaoProducaoPermitidaItemCardapio> ObservacoesPermitidas { get; set; }

        [InverseProperty("Item")]
        public virtual List<OpcaoExtraItemCardapio> ExtrasPermitidos { get; set; }

        [InverseProperty("Item")]
        public virtual List<ItemCardapioImpressora> ImpressorasAssociadas { get; set; }

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
        [Column("COD_ITEM_CARDAPIO", Order = 1)]
        public int CodItemCardapio { get; set; }

        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.None)]
        [Key, ForeignKey("ObservacaoProducao")]
        [Column("COD_OBSERVACAO", Order = 2)]
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

    [Table("IMPRESSORA_PRODUCAO")]
    public class ImpressoraProducao
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Key]
        [Column("COD_IMPRESSORA")]
        public int CodImpressora { get; set; }

        [Required]
        [Column("DESCRICAO")]
        public string Descricao { get; set; }

        [Required]
        [Column("PORTA")]
        public string Porta { get; set; }

        [InverseProperty("ImpressoraProducao")]
        public virtual List<ItemCardapioImpressora> ItensAssociados { get; set; }

        [InverseProperty("ImpressoraPadrao")]
        public virtual List<ClasseItemCardapio> ClassesAssociadas { get; set; }
    }

    [Table("ITEM_CARDAPIO_IMPRESSORA_PRODUCAO")]
    public class ItemCardapioImpressora
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.None)]
        [Key, ForeignKey("Item")]
        [Column("COD_ITEM_CARDAPIO", Order = 1)]
        public int CodItemCardapio { get; set; }

        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.None)]
        [Key, ForeignKey("ImpressoraProducao")]
        [Column("COD_IMPRESSORA", Order = 2)]
        public int CodImpressora { get; set; }

        public virtual ImpressoraProducao ImpressoraProducao { get; set; }

        public virtual ItemCardapio Item { get; set; }
    }
}