using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BrasaoHamburgueria.Model;
using BrasaoHamburgueria.Web.Context;
using System.Threading.Tasks;
using System.Data.Entity;
using BrasaoHamburgueria.Helper;
using BrasaoHamburgueria.Web.Helpers;

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

        public async Task<List<ProgramaFidelidadeUsuarioViewModel>> GetProgramasFidelidade(int? codEmpresa)
        {
            var programas = await (from progs in _contexto.ProgramasFidelidade
                                   from emps in _contexto.Empresas.Where(c => c.CodEmpresa == progs.CodEmpresa).DefaultIfEmpty()
                                   join pont in _contexto.PontuacoesDinheiroProgramaFidelidade on progs.CodProgramaFidelidade equals pont.CodProgramaFidelidade
                                   where (progs.CodEmpresa != null ? progs.CodEmpresa : codEmpresa) == codEmpresa && (SessionData.EmpresasInt.Contains(progs.CodEmpresa != null ? progs.CodEmpresa.Value : 0))
                                   orderby progs.CodProgramaFidelidade
                                   select new ProgramaFidelidadeUsuarioViewModel
                                   {
                                       CodEmpresa = progs.CodEmpresa,
                                       NomeEmpresa = emps.NomeFantasia,
                                       CodProgramaFidelidade = progs.CodProgramaFidelidade,
                                       DescricaoProgramaFidelidade = progs.DescricaoProgramaFidelidade,
                                       InicioVigencia = progs.InicioVigencia,
                                       TerminoVigencia = progs.TerminoVigencia,
                                       ProgramaAtivo = progs.ProgramaAtivo,
                                       TermosAceite = progs.TermosAceite,
                                       CodTipoPontuacaoProgramaFidelidade = progs.CodTipoPontuacaoProgramaFidelidade,
                                       PontosGanhosPorUnidadeMonetariaGasta = pont.PontosGanhosPorUnidadeMonetariaGasta,
                                       QuantidadeMinimaPontosParaResgate = pont.QuantidadeMinimaPontosParaResgate,
                                       ValorDinheiroPorPontoParaResgate = pont.ValorDinheiroPorPontoParaResgate
                                   }).ToListAsync();

            return programas;
        }

        public async Task<ProgramaFidelidadeUsuarioViewModel> GravarProgramaFidelidade(ProgramaFidelidadeUsuarioViewModel prog, String modoCadastro)
        {
            if (modoCadastro == "I" && prog.InicioVigencia <= DateTime.Now)
            {
                throw new Exception("A data de início da vigência não pode ser retroativa.");
            }

            var validaData = _contexto.ProgramasFidelidade.Where(p => p.CodProgramaFidelidade != prog.CodProgramaFidelidade && p.ProgramaAtivo && ((p.TerminoVigencia == null && p.InicioVigencia <= prog.InicioVigencia) || (p.TerminoVigencia != null && p.TerminoVigencia.Value >= prog.InicioVigencia))).Count();
            if (validaData > 0)
            {
                throw new Exception("Já existe outro programa de fidelidade cadastrado ativo para o período selecionado.");
            }

            if (modoCadastro == "A") //alteração
            {
                var progAlterar = _contexto.ProgramasFidelidade.Find(prog.CodProgramaFidelidade);

                if (progAlterar != null)
                {
                    progAlterar.CodEmpresa = prog.CodEmpresa;
                    progAlterar.DescricaoProgramaFidelidade = prog.DescricaoProgramaFidelidade;
                    progAlterar.InicioVigencia = prog.InicioVigencia;
                    progAlterar.TermosAceite = prog.TermosAceite.RemoveLineEndings();
                    progAlterar.ProgramaAtivo = prog.ProgramaAtivo;

                    progAlterar.TerminoVigencia = null;
                    if (prog.TerminoVigencia != null)
                    {
                        progAlterar.TerminoVigencia = prog.TerminoVigencia;
                    }

                    var pontuacaoDinheiroAlterar = _contexto.PontuacoesDinheiroProgramaFidelidade.Find(prog.CodProgramaFidelidade);

                    if (pontuacaoDinheiroAlterar != null)
                    {
                        pontuacaoDinheiroAlterar.PontosGanhosPorUnidadeMonetariaGasta = prog.PontosGanhosPorUnidadeMonetariaGasta;
                        pontuacaoDinheiroAlterar.QuantidadeMinimaPontosParaResgate = prog.QuantidadeMinimaPontosParaResgate;
                        pontuacaoDinheiroAlterar.ValorDinheiroPorPontoParaResgate = prog.ValorDinheiroPorPontoParaResgate;
                    }

                    await _contexto.SaveChangesAsync();
                }

                return prog;
            }
            else if (modoCadastro == "I") //inclusão
            {
                var progIncluir = new ProgramaFidelidade();
                if (prog.CodProgramaFidelidade <= 0)
                {
                    progIncluir.CodProgramaFidelidade = 1;
                    var cod = _contexto.ProgramasFidelidade.Max(o => o.CodProgramaFidelidade);
                    if (cod != null)
                    {
                        progIncluir.CodProgramaFidelidade = cod + 1;
                    }
                    prog.CodProgramaFidelidade = progIncluir.CodProgramaFidelidade;
                }
                else
                {
                    var valida = _contexto.ProgramasFidelidade.Find(prog.CodProgramaFidelidade);

                    if (valida != null)
                    {
                        throw new Exception("Já existe um programa de fidelidade cadastrado com o código " + prog.CodProgramaFidelidade);
                    }

                    progIncluir.CodProgramaFidelidade = prog.CodProgramaFidelidade;
                }

                progIncluir.CodEmpresa = prog.CodEmpresa;
                progIncluir.CodTipoPontuacaoProgramaFidelidade = (int)TipoPontuacaoProgramaFidelidadeEnum.PontuacaoDinheiro;
                progIncluir.DescricaoProgramaFidelidade = prog.DescricaoProgramaFidelidade;
                progIncluir.InicioVigencia = prog.InicioVigencia;
                progIncluir.TermosAceite = prog.TermosAceite.RemoveLineEndings();
                progIncluir.ProgramaAtivo = prog.ProgramaAtivo;

                progIncluir.TerminoVigencia = null;
                if (prog.TerminoVigencia != null)
                {
                    progIncluir.TerminoVigencia = prog.TerminoVigencia;
                }

                PontuacaoDinheiroProgramaFidelidade pontuacaoDinheiroIncluir = new PontuacaoDinheiroProgramaFidelidade();

                pontuacaoDinheiroIncluir.CodProgramaFidelidade = prog.CodProgramaFidelidade;
                pontuacaoDinheiroIncluir.PontosGanhosPorUnidadeMonetariaGasta = prog.PontosGanhosPorUnidadeMonetariaGasta;
                pontuacaoDinheiroIncluir.QuantidadeMinimaPontosParaResgate = prog.QuantidadeMinimaPontosParaResgate;
                pontuacaoDinheiroIncluir.ValorDinheiroPorPontoParaResgate = prog.ValorDinheiroPorPontoParaResgate;


                _contexto.ProgramasFidelidade.Add(progIncluir);
                _contexto.PontuacoesDinheiroProgramaFidelidade.Add(pontuacaoDinheiroIncluir);

                await _contexto.SaveChangesAsync();

                return prog;
            }

            return null;
        }

        public async Task<string> ExcluiProgramaFidelidade(ProgramaFidelidadeUsuarioViewModel prog)
        {
            if (_contexto.UsuariosParticipantesProgramaFidelidade.Where(i => i.CodProgramaFidelidade == prog.CodProgramaFidelidade).Count() > 0)
            {
                return "Exclusão não permitida. Este programa de fidelidade possui usuários associados. Como alternativa desative o programa.";
            }

            var progExcluir = await _contexto.ProgramasFidelidade.FindAsync(prog.CodProgramaFidelidade);

            if (progExcluir != null)
            {
                var pontuacaoExcluir = _contexto.PontuacoesDinheiroProgramaFidelidade.Find(prog.CodProgramaFidelidade);

                if (pontuacaoExcluir != null)
                {
                    _contexto.PontuacoesDinheiroProgramaFidelidade.Remove(pontuacaoExcluir);
                }

                _contexto.ProgramasFidelidade.Remove(progExcluir);
                await _contexto.SaveChangesAsync();
            }
            else
            {
                return "Registro não encontrado na base de dados.";
            }

            return "";
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

        public ProgramaFidelidadeUsuarioViewModel GetProgramaFidelidadeUsuario(string loginUsuario, int codEmpresa)
        {
            var prog = (from progs in _contexto.ProgramasFidelidade
                        from emps in _contexto.Empresas.Where(u => progs.CodEmpresa == u.CodEmpresa).DefaultIfEmpty()
                        from usersProgs in _contexto.UsuariosParticipantesProgramaFidelidade.Where(u => progs.CodProgramaFidelidade == u.CodProgramaFidelidade && u.LoginUsuario == loginUsuario).DefaultIfEmpty()
                        from saldoProgs in _contexto.SaldosUsuariosProgramasFidelidade.Where(s => progs.CodProgramaFidelidade == s.CodProgramaFidelidade && s.LoginUsuario == loginUsuario).DefaultIfEmpty()
                        join din in _contexto.PontuacoesDinheiroProgramaFidelidade on progs.CodProgramaFidelidade equals din.CodProgramaFidelidade
                        where progs.ProgramaAtivo && progs.InicioVigencia <= DateTime.Now && (progs.TerminoVigencia == null || progs.TerminoVigencia.Value >= DateTime.Now) && (progs.CodEmpresa != null ? progs.CodEmpresa : codEmpresa) == codEmpresa
                        select new ProgramaFidelidadeUsuarioViewModel
                        {
                            CodEmpresa = progs.CodEmpresa,
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
                catch (Exception ex)
                {
                    throw ex;
                }
            }

        }
    }
}