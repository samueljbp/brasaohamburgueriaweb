using System;
using System.Collections.Generic;

namespace BrasaoSolution.ViewModel
{

    public enum TipoAplicacaoDescontoEnum
    {
        DescontoPorItem = 1,
        DescontoPorClasse = 2
    }

    public class PromocaoVendaViewModel
    {
        public int? CodEmpresa { get; set; }
        public string NomeEmpresa { get; set; }
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
}
