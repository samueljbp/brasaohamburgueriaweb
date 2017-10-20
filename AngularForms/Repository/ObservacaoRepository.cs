using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BrasaoHamburgueria.Web.Context;
using BrasaoHamburgueria.Model;
using System.Threading.Tasks;

namespace BrasaoHamburgueria.Web.Repository
{
    public class ObservacaoRepository
    {
        private BrasaoContext _contexto = new BrasaoContext();

        public async Task<List<ObservacaoProducaoViewModel>> GetObservacoes()
        {
            return _contexto.ObservacoesProducao.OrderBy(o => o.CodObservacao).Select(o => new ObservacaoProducaoViewModel { CodObservacao = o.CodObservacao, DescricaoObservacao = o.DescricaoObservacao }).ToList();
        }

        public async Task<ObservacaoProducaoViewModel> GravarObservacao(ObservacaoProducaoViewModel obs, String modoCadastro)
        {
            if (modoCadastro == "A") //alteração
            {
                var obsAlterar = _contexto.ObservacoesProducao.Find(obs.CodObservacao);

                if (obsAlterar != null)
                {
                    obsAlterar.DescricaoObservacao = obs.DescricaoObservacao;

                    await _contexto.SaveChangesAsync();
                }

                return obs;
            }
            else if (modoCadastro == "I") //inclusão
            {
                var obsIncluir = new ObservacaoProducao();
                if (obs.CodObservacao <= 0)
                {
                    obsIncluir.CodObservacao = _contexto.ObservacoesProducao.Max(o => o.CodObservacao) + 1;
                    obs.CodObservacao = obsIncluir.CodObservacao;
                }
                else
                {
                    var valida = _contexto.ObservacoesProducao.Find(obs.CodObservacao);

                    if (valida != null)
                    {
                        throw new Exception("Já existe uma observação cadastrada com o código " + obs.CodObservacao);
                    }

                    obsIncluir.CodObservacao = obs.CodObservacao;
                }
                obsIncluir.DescricaoObservacao = obs.DescricaoObservacao;

                _contexto.ObservacoesProducao.Add(obsIncluir);

                await _contexto.SaveChangesAsync();

                return obs;
            }

            return null;
        }

        public async Task<string> ExcluiObservacao(ObservacaoProducaoViewModel obs)
        {
            if (_contexto.ObservacoesItensPedidos.Where(i => i.CodObservacao == obs.CodObservacao).Count() > 0)
            {
                return "Exclusão não permitida. Esta observação já foi utilizada em pedidos registrados na base.";
            }

            if (_contexto.ObservacoesPermitidas.Where(i => i.CodObservacao == obs.CodObservacao).Count() > 0)
            {
                return "Exclusão não permitida. Esta observação está associada a itens de cardápio.";
            }

            var obsExcluir = await _contexto.ObservacoesProducao.FindAsync(obs.CodObservacao);

            if (obsExcluir != null)
            {
                _contexto.ObservacoesProducao.Remove(obsExcluir);
                await _contexto.SaveChangesAsync();
            }
            else
            {
                return "Registro não encontrado na base de dados.";
            }

            return "";
        }
    }
}