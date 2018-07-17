using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using BrasaoSolution.Repository.Context;
using BrasaoSolution.Model;
using System.Threading.Tasks;
using System.Globalization;
using BrasaoSolution.ViewModel;

namespace BrasaoSolution.Repository
{
    public class InstitucionalRepository
    {
        private BrasaoContext _contexto = new BrasaoContext();

        #region Horário de funcionamento
        public async Task<List<FuncionamentoEstabelecimentoViewModel>> GetHorariosFuncionamento(int codEmpresa, bool ehAdmin)
        {
            var retorno = await _contexto.FuncionamentosEstabelecimento.Include(e => e.Empresa)
                .Where(o => o.CodEmpresa == codEmpresa && ((SessionData.EmpresasInt.Contains(o.CodEmpresa)) || ehAdmin))
                .OrderBy(o => o.DiaSemana)
                .Select(o => new FuncionamentoEstabelecimentoViewModel
            {
                CodEmpresa = o.CodEmpresa,
                NomeEmpresa = o.Empresa.NomeFantasia,
                DiaSemana = o.DiaSemana,
                AberturaString = o.Abertura,
                FechamentoString = o.Fechamento,
                TemDelivery = o.TemDelivery
            }).ToListAsync();

            foreach(var item in retorno)
            {
                item.DescricaoDiaSemana = new CultureInfo("pt-BR").DateTimeFormat.GetDayName((DayOfWeek)item.DiaSemana);
                item.Abertura = Convert.ToDateTime(DateTime.Now.ToString("dd/MM/yyyy") + " " + item.AberturaString);
                item.Fechamento = Convert.ToDateTime(DateTime.Now.ToString("dd/MM/yyyy") + " " + item.FechamentoString);
            }

            return retorno;
        }

        public async Task<FuncionamentoEstabelecimentoViewModel> GravarFuncionamentoEstabelecimento(FuncionamentoEstabelecimentoViewModel funcionamento, String modoCadastro, int codEmpresa)
        {
            if (modoCadastro == "A") //alteração
            {
                var funcionamentoAlterar = _contexto.FuncionamentosEstabelecimento.Find(funcionamento.DiaSemana, funcionamento.Abertura.ToString("HH:mm"), codEmpresa);

                if (funcionamentoAlterar != null)
                {
                    funcionamentoAlterar.Abertura = funcionamento.Abertura.ToString("HH:mm");
                    funcionamentoAlterar.Fechamento = funcionamento.Fechamento.ToString("HH:mm");
                    funcionamentoAlterar.TemDelivery = funcionamento.TemDelivery;

                    await _contexto.SaveChangesAsync();
                }

                return funcionamento;
            }
            else if (modoCadastro == "I") //inclusão
            {
                funcionamento.DescricaoDiaSemana = new CultureInfo("pt-BR").DateTimeFormat.GetDayName((DayOfWeek)funcionamento.DiaSemana);
                var funcionamentoIncluir = new FuncionamentoEstabelecimento();

                var valida = _contexto.FuncionamentosEstabelecimento.Find(funcionamento.DiaSemana, funcionamento.Abertura.ToString("HH:mm"), codEmpresa);

                if (valida != null)
                {
                    throw new Exception("Já existe um horário de funcionamento cadastrado para o dia " + funcionamento.DescricaoDiaSemana + " e abertura " + funcionamento.Abertura.ToString("HH:mm"));
                }

                funcionamentoIncluir.CodEmpresa = codEmpresa;
                funcionamentoIncluir.DiaSemana = funcionamento.DiaSemana;
                funcionamentoIncluir.Abertura = funcionamento.Abertura.ToString("HH:mm");
                funcionamentoIncluir.Fechamento = funcionamento.Fechamento.ToString("HH:mm");
                funcionamentoIncluir.TemDelivery = funcionamento.TemDelivery;

                _contexto.FuncionamentosEstabelecimento.Add(funcionamentoIncluir);

                await _contexto.SaveChangesAsync();

                return funcionamento;
            }

            return null;
        }

        public async Task<string> ExcluiFuncionamentoEstabelecimento(FuncionamentoEstabelecimentoViewModel funcionamento)
        {
            var funcionamentoExcluir = await _contexto.FuncionamentosEstabelecimento.FindAsync(funcionamento.DiaSemana, funcionamento.Abertura.ToString("HH:mm"), funcionamento.CodEmpresa);

            if (funcionamentoExcluir != null)
            {
                _contexto.FuncionamentosEstabelecimento.Remove(funcionamentoExcluir);
                await _contexto.SaveChangesAsync();
            }
            else
            {
                return "Registro não encontrado na base de dados.";
            }

            return "";
        }
        #endregion
    }
}