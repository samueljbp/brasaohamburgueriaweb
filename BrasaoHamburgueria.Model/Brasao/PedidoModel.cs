﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;

namespace BrasaoHamburgueria.Model
{
    public enum SituacaoPedidoEnum
    {
        EmDigitacao = 0,
        AguardandoConfirmacao = 1,
        Confirmado = 2,
        EmPreparacao = 3,
        EmProcessoEntrega = 4,
        Concluido = 5,
        Cancelado = 9
    }
    #region "Pedido View Model"

    public class PedidoViewModel
    {
        public int CodPedido { get; set; }
        public DateTime DataPedido { get; set; }
        public string FormaPagamento { get; set; }
        public string DescricaoFormaPagamento { get; set; }
        public double TaxaEntrega { get; set; }
        public double? TrocoPara { get; set; }
        public double? Troco { get; set; }
        public string BandeiraCartao { get; set; }
        public double ValorTotal { get; set; }
        public int Situacao { get; set; }
        public string Usuario { get; set; }
        public string DescricaoSituacao { get; set; }
        public bool PedidoExterno { get; set; }
        public bool Alterar { get; set; }
        public string MotivoCancelamento { get; set; }
        public string FeedbackCliente { get; set; }
        public string EstiloLinhaPorTempo { get; set; }
        public double? PercentualDesconto { get; set; }
        public double? ValorDesconto { get; set; }
        public string MotivoDesconto { get; set; }
        public int TempoMedioEspera { get; set; }
        public bool UsaSaldoProgramaFidelidade { get; set; }
        public decimal PontosAUtilizarProgramaRecompensa { get; set; }
        public decimal DinheiroAUtilizarProgramaRecompensa { get; set; }
        public DadosClientePedidoViewModel DadosCliente { get; set; }
        public List<ItemPedidoViewModel> Itens { get; set; }
        public string PortaImpressaoComandaEntrega { get; set; }
    }

    public class DadosClientePedidoViewModel
    {
        public int Id { get; set; }
        public String Email { get; set; }
        public String Nome { get; set; }
        public String Telefone { get; set; }
        public String Sexo { get; set; }
        public String DataNascimento { get; set; }
        public String Estado { get; set; }
        public String Cidade { get; set; }
        public String Logradouro { get; set; }
        public String Numero { get; set; }
        public String Complemento { get; set; }
        public String Bairro { get; set; }
        public String Referencia { get; set; }
        public bool Salvar { get; set; }
        public bool ClienteNovo { get; set; }
    }

    public class ItemPedidoViewModel
    {
        public int SeqItem { get; set; }
        public int CodItem { get; set; }
        public string DescricaoItem { get; set; }
        public List<ObservacaoItemPedidoViewModel> Obs { get; set; }
        public string ObservacaoLivre { get; set; }
        public List<ExtraItemPedidoViewModel> extras { get; set; }
        public int Quantidade { get; set; }
        public double PrecoUnitario { get; set; }
        public double ValorExtras { get; set; }
        public double ValorTotalItem { get; set; }
        public int AcaoRegistro { get; set; }
        public List<String> PortasImpressaoProducao { get; set; }
    }

    public class ObservacaoItemPedidoViewModel
    {
        public int CodObservacao { get; set; }
        public string DescricaoObservacao { get; set; }
    }

    public class ExtraItemPedidoViewModel
    {
        public int CodOpcaoExtra { get; set; }
        public string DescricaoOpcaoExtra { get; set; }
        public double Preco { get; set; }
    }
    #endregion

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
        [Column("DATA_HORA")]
        public DateTime DataHora { get; set; }

        [Required]
        [Column("TAXA_ENTREGA")]
        public double TaxaEntrega { get; set; }

        [Required]
        [Column("FORMA_PAGAMENTO")]
        public string FormaPagamento { get; set; }

        [Column("TROCO_PARA")]
        public double? TrocoPara { get; set; }

        [Column("TROCO")]
        public double? Troco { get; set; }

        [Column("BANDEIRA_CARTAO")]
        public string BandeiraCartao { get; set; }

        [Required]
        [Column("VALOR_TOTAL")]
        public double ValorTotal { get; set; }

        [Required]
        [Column("NOME_CLIENTE")]
        public string NomeCliente { get; set; }

        [Required]
        [Column("TELEFONE_CLIENTE")]
        public string TelefoneCliente { get; set; }

        [Required]
        [Column("UF_ENTREGA")]
        public string UFEntrega { get; set; }

        [Required]
        [Column("CIDADE_ENTREGA")]
        public string CidadeEntrega { get; set; }

        [Required]
        [Column("LOGRADOURO_ENTREGA")]
        public string LogradouroEntrega { get; set; }

        [Required]
        [Column("NUMERO_ENTREGA")]
        public string NumeroEntrega { get; set; }

        [Required]
        [Column("COMPLEMENTO_ENTREGA")]
        public string ComplementoEntrega { get; set; }

        [Required]
        [Column("BAIRRO_ENTREGA")]
        public string BairroEntrega { get; set; }

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

        [InverseProperty("Pedido")]
        public virtual List<ExtratoUsuarioProgramaFidelidade> LancamentoExtrato { get; set; }

        [InverseProperty("Pedido")]
        public virtual List<ItemPedido> Itens { get; set; }
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
}