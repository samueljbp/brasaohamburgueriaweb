﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BrasaoSolution.Casa.Model
{
    [Table("TIPO_PONTUACAO_PROGRAMA_FIDELIDADE")]
    public class TipoPontuacaoProgramaFidelidade
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.None)]
        [Column("COD_TIPO_PONTUACAO_PROGRAMA_FIDELIDADE")]
        [Key]
        public int CodTipoPontuacaoProgramaFidelidade { get; set; }

        [Required]
        [Column("DESCRICAO_TIPO_PONTUACAO_PROGRAMA_FIDELIDADE")]
        public string DescricaoTipoPontuacaoProgramaFidelidade { get; set; }
    }

    [Table("PROGRAMA_FIDELIDADE")]
    public class ProgramaFidelidade
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.None)]
        [Column("COD_PROGRAMA_FIDELIDADE")]
        [Key]
        public int CodProgramaFidelidade { get; set; }

        [ForeignKey("Empresa")]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.None)]
        [Column("COD_EMPRESA")]
        public int? CodEmpresa { get; set; }

        [Required]
        [Column("COD_TIPO_PONTUACAO_PROGRAMA_FIDELIDADE")]
        [ForeignKey("TipoPontuacao")]
        public int CodTipoPontuacaoProgramaFidelidade { get; set; }

        [Required]
        [Column("DESCRICAO_PROGRAMA_FIDELIDADE")]
        public string DescricaoProgramaFidelidade { get; set; }

        [Required]
        [Column("INICIO_VIGENCIA")]
        public DateTime InicioVigencia { get; set; }

        [Column("TERMINO_VIGENCIA")]
        public DateTime? TerminoVigencia { get; set; }

        [Required]
        [Column("TERMOS_ACEITE")]
        public string TermosAceite { get; set; }

        [Required]
        [Column("PROGRAMA_ATIVO")]
        public bool ProgramaAtivo { get; set; }

        public virtual TipoPontuacaoProgramaFidelidade TipoPontuacao { get; set; }

        [InverseProperty("ProgramaFidelidade")]
        public virtual PontuacaoDinheiroProgramaFidelidade PontuacaoDinheiro { get; set; }

        [InverseProperty("ProgramaFidelidade")]
        public virtual List<UsuarioParticipanteProgramaFidelidade> UsuariosParticipantes { get; set; }

        [InverseProperty("ProgramaFidelidade")]
        public virtual List<SaldoUsuarioProgramaFidelidade> SaldosUsuarios { get; set; }

        [InverseProperty("ProgramaFidelidade")]
        public virtual List<ExtratoUsuarioProgramaFidelidade> ExtratosUsuarios { get; set; }

        public virtual Empresa Empresa { get; set; }
    }

    [Table("PONTUACAO_DINHEIRO_PROGRAMA_FIDELIDADE")]
    public class PontuacaoDinheiroProgramaFidelidade
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.None)]
        [Column("COD_PROGRAMA_FIDELIDADE")]
        [Key]
        [ForeignKey("ProgramaFidelidade")]
        public int CodProgramaFidelidade { get; set; }

        [Required]
        [Column("PONTOS_GANHOS_POR_UNIDADE_MONETARIA_GASTA")]
        public decimal PontosGanhosPorUnidadeMonetariaGasta { get; set; }

        [Required]
        [Column("VALOR_DINHEIRO_POR_PONTO_PARA_RESGATE")]
        public decimal ValorDinheiroPorPontoParaResgate { get; set; }

        [Required]
        [Column("QUANTIDADE_MINIMA_PONTOS_PARA_RESGATE")]
        public decimal QuantidadeMinimaPontosParaResgate { get; set; }

        public virtual ProgramaFidelidade ProgramaFidelidade { get; set; }
    }

    [Table("USUARIO_PARTICIPANTE_PROGRAMA_FIDELIDADE")]
    public class UsuarioParticipanteProgramaFidelidade
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.None)]
        [Column("LOGIN_USUARIO", Order = 1)]
        public string LoginUsuario { get; set; }

        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.None)]
        [Column("COD_PROGRAMA_FIDELIDADE", Order = 2)]
        [ForeignKey("ProgramaFidelidade")]
        public int CodProgramaFidelidade { get; set; }

        [Required]
        [Column("TERMOS_ACEITOS")]
        public bool TermosAceitos { get; set; }

        [Required]
        [Column("DATA_HORA_ACEITE")]
        public DateTime DataHoraAceite { get; set; }

        public virtual ProgramaFidelidade ProgramaFidelidade { get; set; }
    }

    [Table("SALDO_USUARIO_PROGRAMA_FIDELIDADE")]
    public class SaldoUsuarioProgramaFidelidade
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.None)]
        [Column("LOGIN_USUARIO", Order = 1)]
        public string LoginUsuario { get; set; }

        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.None)]
        [Column("COD_PROGRAMA_FIDELIDADE", Order = 2)]
        [ForeignKey("ProgramaFidelidade")]
        public int CodProgramaFidelidade { get; set; }

        [Required]
        [Column("SALDO")]
        public decimal Saldo { get; set; }

        public virtual ProgramaFidelidade ProgramaFidelidade { get; set; }
    }

    [Table("EXTRATO_USUARIO_PROGRAMA_FIDELIDADE")]
    public class ExtratoUsuarioProgramaFidelidade
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.None)]
        [Column("LOGIN_USUARIO", Order = 1)]
        public string LoginUsuario { get; set; }

        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.None)]
        [Column("COD_PROGRAMA_FIDELIDADE", Order = 2)]
        [ForeignKey("ProgramaFidelidade")]
        public int CodProgramaFidelidade { get; set; }

        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.None)]
        [Column("DATA_HORA_LANCAMENTO", Order = 3)]
        public DateTime DataHoraLancamento { get; set; }

        [Required]
        [Column("DESCRICAO_LANCAMENTO")]
        public string DescricaoLancamento { get; set; }

        [Required]
        [Column("VALOR_LANCAMENTO")]
        public decimal ValorLancamento { get; set; }

        [Required]
        [Column("SALDO_POS_LANCAMENTO")]
        public decimal SaldoPosLancamento { get; set; }

        [Column("COD_PEDIDO")]
        [ForeignKey("Pedido")]
        public int? CodPedido { get; set; }

        public virtual ProgramaFidelidade ProgramaFidelidade { get; set; }

        public virtual Pedido Pedido { get; set; }
    }
}
