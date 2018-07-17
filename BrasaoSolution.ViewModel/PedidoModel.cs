using System;
using System.Collections.Generic;

namespace BrasaoSolution.ViewModel
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

    public class PedidoViewModel
    {
        public int CodEmpresa { get; set; }
        public string NomeEmpresa { get; set; }
        public int CodPedido { get; set; }
        public DateTime DataPedido { get; set; }
        public string CodFormaPagamento { get; set; }
        public string DescricaoFormaPagamento { get; set; }
        public double TaxaEntrega { get; set; }
        public int? CodEntregador { get; set; }
        public string NomeEntregador { get; set; }
        public double? TrocoPara { get; set; }
        public double? Troco { get; set; }
        public int? CodBandeiraCartao { get; set; }
        public string DescricaoBandeiraCartao { get; set; }
        public double ValorTotal { get; set; }
        public int Situacao { get; set; }
        public string Usuario { get; set; }
        public string DescricaoSituacao { get; set; }
        public bool PedidoExterno { get; set; }
        public bool RetirarNaCasa { get; set; }
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
        public int? CodCidade { get; set; }
        public string NomeCidade { get; set; }
        public int? CodBairro { get; set; }
        public string NomeBairro { get; set; }
        public String Logradouro { get; set; }
        public String Numero { get; set; }
        public String Complemento { get; set; }
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
        public double PrecoUnitarioComDesconto { get; set; }
        public double ValorExtras { get; set; }
        public double ValorTotalItem { get; set; }
        public int? CodPromocaoVenda { get; set; }
        public double PercentualDesconto { get; set; }
        public double ValorDesconto { get; set; }
        public int? CodCombo { get; set; }
        public double PrecoCombo { get; set; }
        public string DescricaoCombo { get; set; }
        public int AcaoRegistro { get; set; }
        public List<String> PortasImpressaoProducao { get; set; }
    }

    public class HistoricoPedidoViewModel
    {
        public int CodPedido { get; set; }
        public DateTime DataHora { get; set; }
        public int? CodSituacao { get; set; }
        public string DescricaoSituacao { get; set; }
        public string Usuario { get; set; }
        public string Descricao { get; set; }
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

    public class EntregadorViewModel
    {
        public int? CodEmpresa { get; set; }
        public string NomeEmpresa { get; set; }
        public int CodEntregador { get; set; }
        public string Nome { get; set; }
        public string Sexo { get; set; }
        public string TelefoneFixo { get; set; }
        public string TelefoneCelular { get; set; }
        public string Email { get; set; }
        public string EnderecoCompleto { get; set; }
        public string CPF { get; set; }
        public string Observacao { get; set; }
        public int OrdemAcionamento { get; set; }
        public decimal? ValorPorEntrega { get; set; }
    }

    public class FormaPagamentoViewModel
    {
        public string CodFormaPagamento { get; set; }
        public string DescricaoFormaPagamento { get; set; }
    }

    public class BandeiraCartaoViewModel
    {
        public int CodBandeiraCartao { get; set; }
        public string DescricaoBandeiraCartao { get; set; }
    }
}