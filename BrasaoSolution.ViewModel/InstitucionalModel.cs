using System;

namespace BrasaoSolution.ViewModel
{
    public class FuncionamentoEstabelecimentoViewModel
    {
        public int CodEmpresa { get; set; }
        public string NomeEmpresa { get; set; }
        public DateTime Abertura { get; set; }
        public string AberturaString { get; set; }
        public DateTime Fechamento { get; set; }
        public string FechamentoString { get; set; }
        public int DiaSemana { get; set; }
        public String DescricaoDiaSemana { get; set; }
        public bool TemDelivery { get; set; }
    }
}