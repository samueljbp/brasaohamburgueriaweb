using System;
using System.Collections.Generic;

namespace BrasaoSolution.ViewModel
{
    public class ObservacaoProducaoItemCardapioViewModel
    {
        public int CodItemCardapio { get; set; }
        public string Descricao { get; set; }
        public List<ObservacaoProducaoViewModel> Observacoes { get; set; }
    }

    public class ImpressoraProducaoViewModel
    {
        public int? CodEmpresa { get; set; }
        public string NomeEmpresa { get; set; }
        public int CodImpressora { get; set; }
        public string Descricao { get; set; }
        public string Porta { get; set; }
    }

    public class DadosItemCardapioViewModel
    {
        public int CodItemCardapio { get; set; }
        public string Nome { get; set; }
        public List<ObservacaoProducaoViewModel> Observacoes { get; set; }
        public List<OpcaoExtraViewModel> Extras { get; set; }
    }

    public class ClasseItemCardapioViewModel
    {
        public int CodClasse { get; set; }
        public string DescricaoClasse { get; set; }
        public bool Sincronizar { get; set; }
        public int? CodImpressoraPadrao { get; set; }
        public string DescricaoImpressoraPadrao { get; set; }
        public string Imagem { get; set; }
        public string ImagemMini { get; set; }
        public int OrdemExibicao { get; set; }
        public List<ItemCardapioViewModel> Itens { get; set; }
    }

    public class ItemCardapioViewModel
    {
        public int? CodEmpresa { get; set; }
        public string NomeEmpresa { get; set; }
        public int CodItemCardapio { get; set; }
        public int CodClasse { get; set; }
        public string DescricaoClasse { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public double Preco { get; set; }
        public int? CodPromocaoVenda { get; set; }
        public double PercentualDesconto { get; set; }
        public double PrecoComDesconto { get; set; }
        public int? CodCombo { get; set; }
        public double PrecoCombo { get; set; }
        public bool Ativo { get; set; }
        public bool Sincronizar { get; set; }
        public int? CodImpressoraProducao { get; set; }
        public string DescricaoImpressoraProducao { get; set; }
        public ComplementoItemCardapioViewModel Complemento { get; set; }
        public List<ObservacaoProducaoViewModel> ObservacoesPermitidas { get; set; }
        public List<OpcaoExtraViewModel> ExtrasPermitidos { get; set; }
    }

    public class ComboViewModel : ItemCardapioViewModel
    {
        public DateTime DataHoraInicio { get; set; }
        public DateTime DataHoraFim { get; set; }
        public string DataInicio { get; set; }
        public string DataFim { get; set; }
        public string HoraInicio { get; set; }
        public string HoraFim { get; set; }
        public List<ComboItemCardapioViewModel> Itens { get; set; }
        public List<DiaSemanaViewModel> DiasAssociados { get; set; }
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
        public string ImagemMini { get; set; }
        public int? OrdemExibicao { get; set; }
    }

    public class ComboItemCardapioViewModel
    {
        public int CodCombo { get; set; }
        public int CodItemCardapio { get; set; }
        public string Nome { get; set; }
        public int Quantidade { get; set; }
    }
}