using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BrasaoHamburgueria.Web.Context;
using BrasaoHamburgueria.Model;
using System.Threading.Tasks;
using System.Globalization;

namespace BrasaoHamburgueria.Web.Repository
{
    public class InstitucionalRepository
    {
        private BrasaoContext _contexto = new BrasaoContext();

        #region Horário de funcionamento
        public async Task<List<FuncionamentoEstabelecimentoViewModel>> GetHorariosFuncionamento()
        {
            var lista = _contexto.FuncionamentosEstabelecimento.ToList();
            List<FuncionamentoEstabelecimentoViewModel> retorno = new List<FuncionamentoEstabelecimentoViewModel>();

            foreach (var item in lista)
            {
                FuncionamentoEstabelecimentoViewModel func = new FuncionamentoEstabelecimentoViewModel();
                func.DiaSemana = item.DiaSemana;
                func.DescricaoDiaSemana = new CultureInfo("pt-BR").DateTimeFormat.GetDayName((DayOfWeek)item.DiaSemana);
                func.Abertura = Convert.ToDateTime(DateTime.Now.ToString("dd/MM/yyyy") + " " + item.Abertura);
                func.Fechamento = Convert.ToDateTime(DateTime.Now.ToString("dd/MM/yyyy") + " " + item.Fechamento);
                func.TemDelivery = item.TemDelivery;
                retorno.Add(func);
            }

            return retorno;
        }

        public async Task<FuncionamentoEstabelecimentoViewModel> GravarFuncionamentoEstabelecimento(FuncionamentoEstabelecimentoViewModel funcionamento, String modoCadastro)
        {
            if (modoCadastro == "A") //alteração
            {
                var funcionamentoAlterar = _contexto.FuncionamentosEstabelecimento.Find(funcionamento.DiaSemana, funcionamento.Abertura.ToString("HH:mm"));

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

                var valida = _contexto.FuncionamentosEstabelecimento.Find(funcionamento.DiaSemana, funcionamento.Abertura.ToString("HH:mm"));

                if (valida != null)
                {
                    throw new Exception("Já existe um horário de funcionamento cadastrado para o dia " + funcionamento.DescricaoDiaSemana + " e abertura " + funcionamento.Abertura.ToString("HH:mm"));
                }

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
            var funcionamentoExcluir = await _contexto.FuncionamentosEstabelecimento.FindAsync(funcionamento.DiaSemana, funcionamento.Abertura.ToString("HH:mm"));

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