using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BrasaoHamburgueria.Model;
using BrasaoHamburgueria.Web.Context;
using System.Threading.Tasks;
using System.Data.Entity;
using BrasaoHamburgueria.Helper;

namespace BrasaoHamburgueria.Web.Repository
{
    public class ProgramaFidelidadeRepository : IDisposable
    {
        private BrasaoContext _contexto;

        public void Dispose()
        {
            if (_contexto != null)
            {
                _contexto.Dispose();
            }
        }

        public ProgramaFidelidadeRepository()
        {
            _contexto = new BrasaoContext();
        }

        public async Task<List<ExtratoProgramaFidelidadeViewModel>> GetExtratoUsuarioProgramaRecompensa(string loginUsuario)
        {
            var extrato = await (from ext in _contexto.ExtratosUsuariosProgramasFidelidade
                                 where ext.LoginUsuario == loginUsuario
                                 orderby ext.DataHoraLancamento descending
                                 select new ExtratoProgramaFidelidadeViewModel
                                 {
                                     CodPedido = ext.CodPedido,
                                     CodProgramaFidelidade = ext.CodProgramaFidelidade,
                                     DataHoraLancamento = ext.DataHoraLancamento,
                                     DescricaoLancamento = ext.DescricaoLancamento,
                                     LoginUsuario = ext.LoginUsuario,
                                     SaldoPosLancamento = ext.SaldoPosLancamento,
                                     ValorLancamento = ext.ValorLancamento
                                 }
                                 ).ToListAsync();

            return extrato;
        }

        public ProgramaFidelidadeUsuarioViewModel GetProgramaFidelidadeUsuario(string loginUsuario)
        {
            var prog = (from progs in _contexto.ProgramasFidelidade
                        from usersProgs in _contexto.UsuariosParticipantesProgramaFidelidade.Where(u => progs.CodProgramaFidelidade == u.CodProgramaFidelidade && u.LoginUsuario == loginUsuario).DefaultIfEmpty()
                        from saldoProgs in _contexto.SaldosUsuariosProgramasFidelidade.Where(s => progs.CodProgramaFidelidade == s.CodProgramaFidelidade && s.LoginUsuario == loginUsuario).DefaultIfEmpty()
                        join din in _contexto.PontuacoesDinheiroProgramaFidelidade on progs.CodProgramaFidelidade equals din.CodProgramaFidelidade
                        where progs.ProgramaAtivo && progs.InicioVigencia <= DateTime.Now && (progs.TerminoVigencia == null || progs.TerminoVigencia.Value >= DateTime.Now)
                        select new ProgramaFidelidadeUsuarioViewModel
                        {
                            CodProgramaFidelidade = progs.CodProgramaFidelidade,
                            CodTipoPontuacaoProgramaFidelidade = progs.CodTipoPontuacaoProgramaFidelidade,
                            DataHoraAceite = usersProgs.DataHoraAceite,
                            DescricaoProgramaFidelidade = progs.DescricaoProgramaFidelidade,
                            InicioVigencia = progs.InicioVigencia,
                            LoginUsuario = usersProgs.LoginUsuario,
                            ProgramaAtivo = progs.ProgramaAtivo,
                            TerminoVigencia = progs.TerminoVigencia,
                            TermosAceite = progs.TermosAceite,
                            TermosAceitos = usersProgs.TermosAceitos,
                            Saldo = saldoProgs.Saldo,
                            PontosGanhosPorUnidadeMonetariaGasta = din.PontosGanhosPorUnidadeMonetariaGasta,
                            ValorDinheiroPorPontoParaResgate = din.ValorDinheiroPorPontoParaResgate,
                            QuantidadeMinimaPontosParaResgate = din.QuantidadeMinimaPontosParaResgate
                        }).FirstOrDefault();

            return prog;
        }

        public async Task RegistraAceite(ProgramaFidelidadeUsuarioViewModel programa)
        {
            var valida = _contexto.UsuariosParticipantesProgramaFidelidade.Where(u => u.CodProgramaFidelidade == programa.CodProgramaFidelidade && u.LoginUsuario == programa.LoginUsuario).FirstOrDefault();
            if (valida != null)
            {
                throw new Exception("O usuário já está inscrito neste programa de fidelidade.");
            }

            using (var dbContextTransaction = _contexto.Database.BeginTransaction())
            {
                try
                {
                    UsuarioParticipanteProgramaFidelidade part = new UsuarioParticipanteProgramaFidelidade();
                    part.CodProgramaFidelidade = programa.CodProgramaFidelidade;
                    part.LoginUsuario = programa.LoginUsuario;
                    part.TermosAceitos = programa.TermosAceitos.Value;
                    part.DataHoraAceite = DateTime.Now;

                    _contexto.UsuariosParticipantesProgramaFidelidade.Add(part);
                    await _contexto.SaveChangesAsync();

                    //inicia o saldo do participante
                    SaldoUsuarioProgramaFidelidade saldo = new SaldoUsuarioProgramaFidelidade();
                    saldo.LoginUsuario = programa.LoginUsuario;
                    saldo.CodProgramaFidelidade = programa.CodProgramaFidelidade;
                    saldo.Saldo = 0;
                    _contexto.SaldosUsuariosProgramasFidelidade.Add(saldo);
                    await _contexto.SaveChangesAsync();

                    ExtratoUsuarioProgramaFidelidade extrato = new ExtratoUsuarioProgramaFidelidade();
                    extrato.CodPedido = null;
                    extrato.CodProgramaFidelidade = programa.CodProgramaFidelidade;
                    extrato.DataHoraLancamento = DateTime.Now;
                    extrato.DescricaoLancamento = "Registro do usuário no programa de fidelidade.";
                    extrato.LoginUsuario = programa.LoginUsuario;
                    extrato.SaldoPosLancamento = 0;
                    extrato.ValorLancamento = 0;
                    _contexto.ExtratosUsuariosProgramasFidelidade.Add(extrato);
                    await _contexto.SaveChangesAsync();

                    dbContextTransaction.Commit();
                }
                catch(Exception ex)
                {
                    throw ex;
                }
            }
            
        }
    }
}