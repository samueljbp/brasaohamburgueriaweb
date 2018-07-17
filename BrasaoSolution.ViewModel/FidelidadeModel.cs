using System;
using System.Collections.Generic;

namespace BrasaoSolution.ViewModel
{
    public enum TipoPontuacaoProgramaFidelidadeEnum
    {
        PontuacaoDinheiro = 1
    }

    public class ProgramaFidelidadeUsuarioViewModel
    {
        public int? CodEmpresa { get; set; }
        public string NomeEmpresa { get; set; }
        public string LoginUsuario { get; set; }
        public int CodProgramaFidelidade { get; set; }
        public int CodTipoPontuacaoProgramaFidelidade { get; set; }
        public string DescricaoProgramaFidelidade { get; set; }
        public DateTime InicioVigencia { get; set; }
        public DateTime? TerminoVigencia { get; set; }
        public string TermosAceite { get; set; }
        public bool ProgramaAtivo { get; set; }
        public bool? TermosAceitos { get; set; }
        public DateTime? DataHoraAceite { get; set; }
        public decimal? Saldo { get; set; }
        public decimal PontosGanhosPorUnidadeMonetariaGasta { get; set; }
        public decimal ValorDinheiroPorPontoParaResgate { get; set; }
        public decimal QuantidadeMinimaPontosParaResgate { get; set; }
    }

    public class ExtratoProgramaFidelidadeViewModel
    {
        public string LoginUsuario { get; set; }
        public int CodProgramaFidelidade { get; set; }
        public DateTime DataHoraLancamento { get; set; }
        public string DescricaoLancamento { get; set; }
        public decimal ValorLancamento { get; set; }
        public decimal SaldoPosLancamento { get; set; }
        public int? CodPedido { get; set; }
    }
}
