using System;
using System.Collections.Generic;

namespace BrasaoSolution.ViewModel
{
    public enum TipoAcaoRegistro
    {
        Incluir = 0,
        Nenhuma = 1,
        Alterar = 2,
        Cancelar = 3,
        EmInclusao = 4
    }

    public class ComandaViewModel
    {
        public int CodEmpresa { get; set; }
        public string NomeEmpresa { get; set; }
        public int CodComanda { get; set; }
        public int NumMesa { get; set; }
        public DateTime DataPedido { get; set; }
        public int CodFormaPagamento { get; set; }
        public string DescricaoFormaPagamento { get; set; }
        public decimal TrocoPara { get; set; }
        public decimal Troco { get; set; }
        public int CodBandeiraCartao { get; set; }
        public string DescricaoBandeiraCartao { get; set; }
        public decimal ValorTotal { get; set; }
        public int Situacao { get; set; }
        public string DescricaoSituacao { get; set; }
        public decimal PercentualDesconto { get; set; }
        public decimal ValorDesconto { get; set; }
        public int MotivoDesconto { get; set; }
        public List<ItemComandaViewModel> Itens { get; set; }
    }

    public class ItemComandaViewModel
    {
        public int SeqItem { get; set; }
        public int CodItem { get; set; }
        public string DescricaoItem { get; set; }
        public string ObservacaoLivre { get; set; }
        public int Quantidade { get; set; }
        public decimal PrecoUnitario { get; set; }
        public decimal PrecoUnitarioComDesconto { get; set; }
        public decimal ValorExtras { get; set; }
        public decimal ValorTotal { get; set; }
        public int? CodPromocaoVenda { get; set; }
        public decimal PercentualDesconto { get; set; }
        public decimal ValorDesconto { get; set; }
        public int? CodCombo { get; set; }
        public decimal PrecoCombo { get; set; }
        public string DescricaoCombo { get; set; }
        public int AcaoRegistro { get; set; }
        public List<ObservacaoItemComandaViewModel> Obs { get; set; }
        public List<ExtraItemComandaViewModel> Extras { get; set; }
    }
}

public class ObservacaoItemComandaViewModel
{
    public int CodObservacao { get; set; }
    public string DescricaoObservacao { get; set; }
}

public class ExtraItemComandaViewModel
{
    public int CodOpcaoExtra { get; set; }
    public string DescricaoOpcaoExtra { get; set; }
    public decimal Preco { get; set; }
}