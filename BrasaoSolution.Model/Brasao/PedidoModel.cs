using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;

namespace BrasaoSolution.Model
{
    [Table("SITUACAO_PEDIDO")]
    public class SituacaoPedido
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.None)]
        [Key]
        [Column("COD_SITUACAO")]
        public int CodSituacao { get; set; }

        [Required]
        [Column("DESCRICAO")]
        public string Descricao { get; set; }
    }

    [Table("PEDIDO")]
    public class Pedido
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Key]
        [Column("COD_PEDIDO")]
        public int CodPedido { get; set; }

        [ForeignKey("Empresa")]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.None)]
        [Column("COD_EMPRESA")]
        public int CodEmpresa { get; set; }

        [ForeignKey("Situacao")]
        [Column("COD_SITUACAO")]
        public int CodSituacao { get; set; }

        [Required]
        [Column("USUARIO")]
        public string Usuario { get; set; }

        [Required]
        [Column("PEDIDO_EXTERNO")]
        public bool PedidoExterno { get; set; }

        [Required]
        [Column("RETIRAR_NA_CASA")]
        public bool RetirarNaCasa { get; set; }

        [Required]
        [Column("DATA_HORA")]
        public DateTime DataHora { get; set; }

        [Required]
        [Column("TAXA_ENTREGA")]
        public double TaxaEntrega { get; set; }

        [ForeignKey("Entregador")]
        [Column("COD_ENTREGADOR")]
        public int? CodEntregador { get; set; }

        [ForeignKey("FormaPagamentoRef")]
        [Column("COD_FORMA_PAGAMENTO")]
        public string CodFormaPagamento { get; set; }

        [ForeignKey("BandeiraCartaoRef")]
        [Column("COD_BANDEIRA_CARTAO")]
        public int? CodBandeiraCartao { get; set; }

        [Column("TROCO_PARA")]
        public double? TrocoPara { get; set; }

        [Column("TROCO")]
        public double? Troco { get; set; }

        [Required]
        [Column("VALOR_TOTAL")]
        public double ValorTotal { get; set; }

        [Required]
        [Column("NOME_CLIENTE")]
        public string NomeCliente { get; set; }

        [Required]
        [Column("TELEFONE_CLIENTE")]
        public string TelefoneCliente { get; set; }

        [ForeignKey("Bairro")]
        [Column("COD_BAIRRO")]
        public int? CodBairro { get; set; }

        [Column("LOGRADOURO_ENTREGA")]
        public string LogradouroEntrega { get; set; }

        [Column("NUMERO_ENTREGA")]
        public string NumeroEntrega { get; set; }

        [Column("COMPLEMENTO_ENTREGA")]
        public string ComplementoEntrega { get; set; }

        [Column("REFERENCIA_ENTREGA")]
        public string ReferenciaEntrega { get; set; }

        [Column("MOTIVO_CANCELAMENTO")]
        public string MotivoCancelamento { get; set; }

        [Column("FEEDBACK_CLIENTE")]
        public string FeedbackCliente { get; set; }

        [Column("PERCENTUAL_DESCONTO")]
        public double? PercentualDesconto { get; set; }

        [Column("VALOR_DESCONTO")]
        public double? ValorDesconto { get; set; }

        [Column("MOTIVO_DESCONTO")]
        public string MotivoDesconto { get; set; }

        public virtual SituacaoPedido Situacao { get; set; }

        public virtual Entregador Entregador { get; set; }

        public virtual FormaPagamento FormaPagamentoRef { get; set; }

        public virtual BandeiraCartao BandeiraCartaoRef { get; set; }

        [InverseProperty("Pedido")]
        public virtual List<ExtratoUsuarioProgramaFidelidade> LancamentoExtrato { get; set; }

        [InverseProperty("Pedido")]
        public virtual List<ItemPedido> Itens { get; set; }

        [InverseProperty("Pedido")]
        public virtual List<HistoricoPedido> Historicos { get; set; }

        public virtual Empresa Empresa { get; set; }

        public virtual Bairro Bairro { get; set; }
    }

    [Table("ITEM_PEDIDO")]
    public class ItemPedido
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.None)]
        [Key, ForeignKey("Pedido")]
        [Column("COD_PEDIDO", Order = 1)]
        public int CodPedido { get; set; }

        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.None)]
        [Column("SEQ_ITEM", Order = 2)]
        public int SeqItem { get; set; }

        [ForeignKey("ItemCardapio")]
        [Column("COD_ITEM_CARDAPIO")]
        public int CodItemCardapio { get; set; }

        [Column("OBSERVACAO_LIVRE")]
        public string ObservacaoLivre { get; set; }

        [Required]
        [Column("QUANTIDADE")]
        public int Quantidade { get; set; }

        [Required]
        [Column("PRECO_UNITARIO")]
        public double PrecoUnitario { get; set; }

        [Required]
        [Column("VALOR_EXTRAS")]
        public double ValorExtras { get; set; }

        [Column("COD_PROMOCAO_VENDA")]
        public int? CodPromocaoVenda { get; set; }

        [Column("PERCENTUAL_DESCONTO")]
        public double PercentualDesconto { get; set; }

        [Column("VALOR_DESCONTO")]
        public double ValorDesconto { get; set; }

        [Column("COD_COMBO")]
        public int? CodCombo { get; set; }

        [Column("PRECO_COMBO")]
        public double PrecoCombo { get; set; }

        [Required]
        [Column("VALOR_TOTAL")]
        public double ValorTotal { get; set; }

        [Required]
        [Column("CANCELADO")]
        public bool Cancelado { get; set; }

        [Column("MOTIVO_CANCELAMENTO")]
        public string MotivoCancelamento { get; set; }

        public virtual Pedido Pedido { get; set; }

        public virtual ItemCardapio ItemCardapio { get; set; }

        [InverseProperty("ItemPedido")]
        public virtual List<ObservacaoItemPedido> Observacoes { get; set; }

        [InverseProperty("ItemPedido")]
        public virtual List<ExtraItemPedido> Extras { get; set; }
    }

    [Table("HISTORICO_PEDIDO")]
    public class HistoricoPedido
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.None)]
        [Key, ForeignKey("Pedido")]
        [Column("COD_PEDIDO", Order = 1)]
        public int CodPedido { get; set; }

        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.None)]
        [Key]
        [Column("DATA_HORA", Order = 2)]
        public DateTime DataHora { get; set; }

        [Column("COD_SITUACAO")]
        [ForeignKey("Situacao")]
        public int? CodSituacao { get; set; }

        [Required]
        [Column("USUARIO")]
        public string Usuario { get; set; }

        [Required]
        [Column("DESCRICAO")]
        public string Descricao { get; set; }

        public virtual Pedido Pedido { get; set; }

        public virtual SituacaoPedido Situacao { get; set; }
    }
    

    [Table("OBSERVACAO_PRODUCAO_ITEM_PEDIDO")]
    public class ObservacaoItemPedido
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.None)]
        [Key, ForeignKey("ItemPedido")]
        [Column("COD_PEDIDO", Order = 1)]
        public int CodPedido { get; set; }

        [Key, ForeignKey("ItemPedido")]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.None)]
        [Column("SEQ_ITEM", Order = 2)]
        public int SeqItem { get; set; }

        [Key, ForeignKey("Observacao")]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.None)]
        [Column("COD_OBSERVACAO", Order = 3)]
        public int CodObservacao { get; set; }

        public virtual ItemPedido ItemPedido { get; set; }

        public virtual ObservacaoProducao Observacao { get; set; }
    }

    [Table("OPCAO_EXTRA_ITEM_PEDIDO")]
    public class ExtraItemPedido
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.None)]
        [Key, ForeignKey("ItemPedido")]
        [Column("COD_PEDIDO", Order = 1)]
        public int CodPedido { get; set; }

        [Key, ForeignKey("ItemPedido")]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.None)]
        [Column("SEQ_ITEM", Order = 2)]
        public int SeqItem { get; set; }

        [Key, ForeignKey("OpcaoExtra")]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.None)]
        [Column("COD_OPCAO_EXTRA", Order = 3)]
        public int CodOpcaoExtra { get; set; }

        [Required]
        [Column("PRECO")]
        public double Preco { get; set; }

        public virtual ItemPedido ItemPedido { get; set; }

        public virtual OpcaoExtra OpcaoExtra { get; set; }
    }

    [Table("ENTREGADOR")]
    public class Entregador
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.None)]
        [Key]
        [Column("COD_ENTREGADOR")]
        public int CodEntregador { get; set; }

        [ForeignKey("Empresa")]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.None)]
        [Column("COD_EMPRESA")]
        public int? CodEmpresa { get; set; }

        [Required]
        [Column("NOME")]
        public string Nome { get; set; }

        [Required]
        [Column("SEXO")]
        public string Sexo { get; set; }

        [Column("TELEFONE_FIXO")]
        public string TelefoneFixo { get; set; }

        [Column("TELEFONE_CELULAR")]
        public string TelefoneCelular { get; set; }

        [Column("EMAIL")]
        public string Email { get; set; }

        [Column("ENDERECO_COMPLETO")]
        public string EnderecoCompleto { get; set; }

        [Column("CPF")]
        public string CPF { get; set; }

        [Column("OBSERVACAO")]
        public string Observacao { get; set; }

        [Required]
        [Column("ORDEM_ACIONAMENTO")]
        public int OrdemAcionamento { get; set; }

        [Column("VALOR_POR_ENTREGA")]
        public decimal? ValorPorEntrega { get; set; }

        public virtual Empresa Empresa { get; set; }
    }

    [Table("FORMA_PAGAMENTO")]
    public class FormaPagamento
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.None)]
        [Key]
        [Column("COD_FORMA_PAGAMENTO")]
        public string CodFormaPagamento { get; set; }

        [Required]
        [Column("DESCRICAO")]
        public string DescricaoFormaPagamento { get; set; }
    }

    [Table("BANDEIRA_CARTAO")]
    public class BandeiraCartao
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.None)]
        [Key]
        [Column("COD_BANDEIRA_CARTAO")]
        public int CodBandeiraCartao { get; set; }

        [Required]
        [Column("DESCRICAO")]
        public string DescricaoBandeiraCartao { get; set; }
    }
}