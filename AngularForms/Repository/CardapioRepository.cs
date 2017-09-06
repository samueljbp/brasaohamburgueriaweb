using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AngularForms.Model;
using System.Threading.Tasks;
using System.Data.Entity;

namespace AngularForms.Repository
{
    public class CardapioRepository
    {
        private BrasaoContext _contexto;

        public CardapioRepository()
        {
            _contexto = new BrasaoContext();
        }

        public List<ClasseItemCardapioViewModel> GetCardapio()
        {

            return _contexto.Classes
                .Include(c => c.Itens)
                .Include(c => c.Itens.Select(i => i.Classe))
                .Include(c => c.Itens.Select(i => i.Complemento))
                .Include(c => c.Itens.Select(i => i.ObservacoesPermitidas))
                .Include(c => c.Itens.Select(i => i.ObservacoesPermitidas.Select(o => o.ObservacaoProducao)))
                .Include(c => c.Itens.Select(i => i.ExtrasPermitidos))
                .Include(c => c.Itens.Select(i => i.ExtrasPermitidos.Select(e => e.OpcaoExtra)))
                .ToList()
                .Select(c =>
                new ClasseItemCardapioViewModel
                {
                    CodClasse = c.CodClasse,
                    DescricaoClasse = c.DescricaoClasse,
                    Itens = c.Itens.Select(i =>
                        new ItemCardapioViewModel
                        {
                            CodItemCardapio = i.CodItemCardapio,
                            Nome = i.Nome,
                            Preco = i.Preco,
                            ObservacoesPermitidas = (i.ObservacoesPermitidas != null ?
                                i.ObservacoesPermitidas.Select(o => new ObservacaoProducaoViewModel { CodObservacao = o.ObservacaoProducao.CodObservacao, DescricaoObservacao = o.ObservacaoProducao.DescricaoObservacao }).ToList() : null),
                            ExtrasPermitidos = (i.ExtrasPermitidos != null ?
                                i.ExtrasPermitidos.Select(e => new OpcaoExtraViewModel { CodOpcaoExtra = e.OpcaoExtra.CodOpcaoExtra, DescricaoOpcaoExtra = e.OpcaoExtra.DescricaoOpcaoExtra, Preco = e.OpcaoExtra.Preco }).ToList() : null),
                            Complemento = (i.Complemento != null ?
                                new ComplementoItemCardapioViewModel
                                {
                                    DescricaoLonga = i.Complemento.DescricaoLonga,
                                    Imagem = i.Complemento.Imagem
                                } : null)
                        }).ToList()
                }).ToList();
        }
    }
}