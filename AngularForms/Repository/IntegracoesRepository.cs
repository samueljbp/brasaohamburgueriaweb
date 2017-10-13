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

namespace BrasaoHamburgueria.Web.Repository
{
    public class IntegracoesRepository
    {
        private BrasaoContext _contexto = new BrasaoContext();
        static HttpClient client = new HttpClient();

        public async Task<List<String>> ExecutaIntegracaoTronSolution()
        {
            HttpResponseMessage response = await client.GetAsync("http://localhost:62993/api/TronSolutionData/GetItemCardapio");

            List<String> lista = new List<string>();

            if (response.IsSuccessStatusCode)
            {
                ServiceResultViewModel result = await response.Content.ReadAsAsync<ServiceResultViewModel>();
                List<ItemCardapioViewModel> itensTron = JsonConvert.DeserializeObject<List<ItemCardapioViewModel>>(result.data.ToString());
                
                List<ItemCardapioViewModel> itensBrasao = _contexto.ItensCardapio.Select(i => new ItemCardapioViewModel { Ativo = i.Ativo, CodItemCardapio = i.CodItemCardapio, CodClasse = i.CodClasse, Nome = i.Nome, Preco = i.Preco }).ToList();

                List<ItemCardapioViewModel> itensNovos = new List<ItemCardapioViewModel>();
                List<ItemCardapioViewModel> itensAlterar = new List<ItemCardapioViewModel>();
                List<ItemCardapioViewModel> itensInativar = new List<ItemCardapioViewModel>();

                itensNovos = (from t in itensTron
                              where !itensBrasao.Any(b=>(b.CodItemCardapio==t.CodItemCardapio))
                              select t).ToList();

                itensInativar = (from t in itensBrasao
                                 where !itensTron.Any(b => (b.CodItemCardapio == t.CodItemCardapio))
                                 select t).ToList();

                itensInativar.AddRange((from t in itensBrasao
                                        where itensTron.Any(b => (b.CodItemCardapio == t.CodItemCardapio && !t.Ativo && b.Ativo))
                                        select t).ToList());

                itensAlterar = (from t in itensBrasao
                                where itensTron.Any(b => (b.CodItemCardapio == t.CodItemCardapio && (b.Nome.ToUpper() != t.Nome.ToUpper() || b.Preco != t.Preco || b.CodClasse != t.CodClasse)))
                                select t).ToList();

                lista.Add(itensNovos.Count + " iten(s) de cardápio incluídos.");
                lista.Add(itensInativar.Count + " iten(s) de cardápio inativados.");
                lista.Add(itensAlterar.Count + " iten(s) de cardápio alterados.");
            }

            return lista;
        }
    }
}