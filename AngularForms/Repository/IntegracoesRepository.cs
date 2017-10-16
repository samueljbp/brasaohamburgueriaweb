using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BrasaoHamburgueria.Model;
using BrasaoHamburgueria.Web.Context;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Configuration;

namespace BrasaoHamburgueria.Web.Repository
{
    public class IntegracoesRepository
    {
        private BrasaoContext _contexto = new BrasaoContext();
        static HttpClient client = new HttpClient();

        public async Task<List<String>> ExecutaIntegracaoTronSolution()
        {
            String urlBase = "";
            if (ConfigurationManager.AppSettings["URLServicosInternos"] != null)
            {
                urlBase = ConfigurationManager.AppSettings["URLServicosInternos"].ToString();
            }

            List<String> lista = new List<string>();

            HttpResponseMessage responseClasses = await client.GetAsync(urlBase + "api/TronSolutionData/GetClassesItemCardapio");
            var totalAlteracoes = 0;

            if (responseClasses.IsSuccessStatusCode)
            {
                ServiceResultViewModel result = await responseClasses.Content.ReadAsAsync<ServiceResultViewModel>();
                List<ClasseItemCardapioViewModel> classesTron = JsonConvert.DeserializeObject<List<ClasseItemCardapioViewModel>>(result.data.ToString());

                List<ClasseItemCardapioViewModel> classesBrasao = _contexto.Classes.Select(i => new ClasseItemCardapioViewModel { CodClasse = i.CodClasse, DescricaoClasse = i.DescricaoClasse }).ToList();

                List<ClasseItemCardapioViewModel> classesNovas = new List<ClasseItemCardapioViewModel>();
                List<ClasseItemCardapioViewModel> classesAlterar = new List<ClasseItemCardapioViewModel>();
                List<ClasseItemCardapioViewModel> classesInativar = new List<ClasseItemCardapioViewModel>();

                classesNovas = (from t in classesTron
                              where !classesBrasao.Any(b => (b.CodClasse == t.CodClasse))
                              select t).ToList();

                classesInativar = (from b in classesBrasao
                                   where !classesTron.Any(t => (b.CodClasse == t.CodClasse))
                                 select b).ToList();

                classesAlterar = (from t in classesTron
                                  where classesTron.Any(b => (b.CodClasse == t.CodClasse && b.DescricaoClasse.Trim().ToUpper() != t.DescricaoClasse.Trim().ToUpper()))
                                select t).ToList();

                if (classesNovas.Count > 0)
                {
                    foreach(var nova in classesNovas)
                    {
                        var classe = new ClasseItemCardapio();
                        classe.CodClasse = nova.CodClasse;
                        classe.DescricaoClasse = nova.DescricaoClasse;
                        _contexto.Classes.Add(classe);
                    }

                    _contexto.SaveChanges();
                    totalAlteracoes += 1;
                    lista.Add(classesNovas.Count + " classe(s) de cardápio incluídas.");
                }

                if (classesAlterar.Count > 0)
                {
                    foreach(var diferente in classesAlterar)
                    {
                        var classe = _contexto.Classes.Find(diferente.CodClasse);
                        if (classe != null)
                        {
                            classe.DescricaoClasse = diferente.DescricaoClasse;
                        }
                    }

                    _contexto.SaveChanges();
                    totalAlteracoes += 1;
                    lista.Add(classesAlterar.Count + " classe(s) de cardápio alteradas.");
                }

                
                //lista.Add(classesInativar.Count + " classe(s) de cardápio inativadas.");
                
            }

            HttpResponseMessage responseItensCardapio = await client.GetAsync(urlBase + "api/TronSolutionData/GetItemCardapio");

            if (responseItensCardapio.IsSuccessStatusCode)
            {
                ServiceResultViewModel result = await responseItensCardapio.Content.ReadAsAsync<ServiceResultViewModel>();
                List<ItemCardapioViewModel> itensTron = JsonConvert.DeserializeObject<List<ItemCardapioViewModel>>(result.data.ToString());

                List<ItemCardapioViewModel> itensBrasao = _contexto.ItensCardapio.Select(i => new ItemCardapioViewModel { Ativo = i.Ativo, CodItemCardapio = i.CodItemCardapio, CodClasse = i.CodClasse, Nome = i.Nome, Preco = i.Preco }).ToList();

                List<ItemCardapioViewModel> itensNovos = new List<ItemCardapioViewModel>();
                List<ItemCardapioViewModel> itensAlterar = new List<ItemCardapioViewModel>();
                List<ItemCardapioViewModel> itensInativar = new List<ItemCardapioViewModel>();

                itensNovos = (from t in itensTron
                              where !itensBrasao.Any(b => (b.CodItemCardapio == t.CodItemCardapio))
                              select t).ToList();

                itensInativar = (from b in itensBrasao
                                 where b.Ativo && !itensTron.Any(t => (b.CodItemCardapio == t.CodItemCardapio))
                                 select b).ToList();

                itensInativar.AddRange((from b in itensBrasao
                                        where itensTron.Any(t => (b.CodItemCardapio == t.CodItemCardapio && !t.Ativo && b.Ativo))
                                        select b).ToList());

                itensAlterar = (from t in itensTron
                                where t.Ativo && itensTron.Any(b => (b.CodItemCardapio == t.CodItemCardapio && b.Ativo && (b.Nome.Trim().ToUpper() != t.Nome.Trim().ToUpper() || b.CodClasse != t.CodClasse || b.Preco != t.Preco)))
                                select t).ToList();

                if (itensNovos.Count > 0)
                {
                    foreach(var novo in itensNovos)
                    {
                        var item = new ItemCardapio();
                        item.CodItemCardapio = novo.CodItemCardapio;
                        item.Ativo = true;
                        item.CodClasse = novo.CodClasse;
                        item.Nome = novo.Nome;
                        item.Preco = novo.Preco;
                        _contexto.ItensCardapio.Add(item);
                    }
                    _contexto.SaveChanges();
                    totalAlteracoes += 1;
                    lista.Add(itensNovos.Count + " iten(s) de cardápio incluídos.");
                }

                if (itensInativar.Count > 0)
                {
                    foreach(var inativo in itensInativar)
                    {
                        var item = _contexto.ItensCardapio.Find(inativo.CodItemCardapio);
                        if (item != null)
                        {
                            item.Ativo = false;
                        }
                    }

                    _contexto.SaveChanges();
                    totalAlteracoes += 1;
                    lista.Add(itensInativar.Count + " iten(s) de cardápio inativados.");
                }

                
                if (itensAlterar.Count > 0)
                {
                    foreach(var alterado in itensAlterar)
                    {
                        var item = _contexto.ItensCardapio.Find(alterado.CodItemCardapio);
                        if (item != null)
                        {
                            item.CodItemCardapio = alterado.CodItemCardapio;
                            item.CodClasse = alterado.CodClasse;
                            item.Nome = alterado.Nome;
                            item.Preco = alterado.Preco;
                        }
                    }

                    _contexto.SaveChanges();
                    totalAlteracoes += 1;
                    lista.Add(itensAlterar.Count + " iten(s) de cardápio alterados.");
                }
            }

            if (totalAlteracoes == 0)
            {
                lista.Add("Nenhuma alteração foi necessária.");
            }

            _contexto.Dispose();
            _contexto = null;

            return lista;
        }
    }
}