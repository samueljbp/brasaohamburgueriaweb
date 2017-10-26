using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BrasaoHamburgueria.Web.Context;
using BrasaoHamburgueria.Model;
using System.Threading.Tasks;

namespace BrasaoHamburgueria.Web.Repository
{
    public class CadastrosRepository
    {
        private BrasaoContext _contexto = new BrasaoContext();


        #region Cadastros de opções extra
        public async Task<List<OpcaoExtraViewModel>> GetOpcoesExtra()
        {
            return _contexto.Extras.OrderBy(o => o.CodOpcaoExtra).Select(o => new OpcaoExtraViewModel { CodOpcaoExtra = o.CodOpcaoExtra, DescricaoOpcaoExtra = o.DescricaoOpcaoExtra, Preco = o.Preco }).ToList();
        }

        public async Task<OpcaoExtraViewModel> GravarOpcaoExtra(OpcaoExtraViewModel opcao, String modoCadastro)
        {
            if (modoCadastro == "A") //alteração
            {
                var opcaoAlterar = _contexto.Extras.Find(opcao.CodOpcaoExtra);

                if (opcaoAlterar != null)
                {
                    opcaoAlterar.DescricaoOpcaoExtra = opcao.DescricaoOpcaoExtra;
                    opcaoAlterar.Preco = opcao.Preco;

                    await _contexto.SaveChangesAsync();
                }

                return opcao;
            }
            else if (modoCadastro == "I") //inclusão
            {
                var opcaoIncluir = new OpcaoExtra();
                if (opcao.CodOpcaoExtra <= 0)
                {
                    opcaoIncluir.CodOpcaoExtra = 1;
                    var cod = _contexto.Extras.Max(o => o.CodOpcaoExtra);
                    if (cod != null)
                    {
                        opcaoIncluir.CodOpcaoExtra = cod + 1;
                    }
                    opcao.CodOpcaoExtra = opcaoIncluir.CodOpcaoExtra;
                }
                else
                {
                    var valida = _contexto.Extras.Find(opcao.CodOpcaoExtra);

                    if (valida != null)
                    {
                        throw new Exception("Já existe uma opção extra cadastrada com o código " + opcao.CodOpcaoExtra);
                    }

                    opcaoIncluir.CodOpcaoExtra = opcao.CodOpcaoExtra;
                }

                opcaoIncluir.DescricaoOpcaoExtra = opcao.DescricaoOpcaoExtra;
                opcaoIncluir.Preco = opcao.Preco;

                _contexto.Extras.Add(opcaoIncluir);

                await _contexto.SaveChangesAsync();

                return opcao;
            }

            return null;
        }

        public async Task<string> ExcluiOpcaoExtra(OpcaoExtraViewModel opcao)
        {
            if (_contexto.ExtrasItensPedidos.Where(i => i.CodOpcaoExtra == opcao.CodOpcaoExtra).Count() > 0)
            {
                return "Exclusão não permitida. Esta opção extra já foi utilizada em pedidos registrados na base.";
            }

            if (_contexto.ExtrasPermitidos.Where(i => i.CodOpcaoExtra == opcao.CodOpcaoExtra).Count() > 0)
            {
                return "Exclusão não permitida. Esta opção extra está associada a itens de cardápio.";
            }

            var opcaoExcluir = await _contexto.Extras.FindAsync(opcao.CodOpcaoExtra);

            if (opcaoExcluir != null)
            {
                _contexto.Extras.Remove(opcaoExcluir);
                await _contexto.SaveChangesAsync();
            }
            else
            {
                return "Registro não encontrado na base de dados.";
            }

            return "";
        }
        #endregion
        

        #region Cadastros de observações
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
                    obsIncluir.CodObservacao = 1;
                    var cod = _contexto.Extras.Max(o => o.CodOpcaoExtra);
                    if (cod != null)
                    {
                        obsIncluir.CodObservacao = cod + 1;
                    }
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
        #endregion

    }
}