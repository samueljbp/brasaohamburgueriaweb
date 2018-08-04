using System.Net;
using System.Net.Http;
using System.Web.Http;
using BrasaoSolution.ViewModel;
using BrasaoSolution.Repository;

namespace BrasaoHamburgueria.Web.API
{
    [RoutePrefix("api/comanda")]
    public class ComandaController : ApiController
    {
        [Route("GetComanda")]
        [Authorize]
        [HttpGet]
        public ComandaViewModel GetComanda(string numMesa)
        {
            return new ComandaViewModel();
        }

        [Route("RegistraPedido")]
        [Authorize]
        [HttpPost]
        public ComandaViewModel RegistraPedido([FromBody] ComandaViewModel comanda)
        {
            comanda.Itens.ForEach(x => { x.AcaoRegistro = (int)TipoAcaoRegistro.Nenhuma; });

            ItemComandaViewModel item = new ItemComandaViewModel();
            item.AcaoRegistro = (int)TipoAcaoRegistro.Nenhuma;
            item.CodCombo = null;
            item.CodItem = 99;
            item.CodPromocaoVenda = null;
            item.DescricaoCombo = null;
            item.DescricaoItem = "Item manual";
            item.Extras = null;
            item.Obs = null;
            item.ObservacaoLivre = "Manual";
            item.PercentualDesconto = 0;
            item.PrecoCombo = 0;
            item.PrecoUnitario = 23.80M;
            item.PrecoUnitarioComDesconto = 0;
            item.Quantidade = 1;
            item.SeqItem = 20;
            item.ValorDesconto = 0;
            item.ValorExtras = 0;
            item.ValorTotal = 23.80M;

            comanda.Itens.Add(item);

            //base._httpContext.Session.SetString("comanda", JsonConvert.SerializeObject(comanda));

            return comanda;
        }
    }
}
