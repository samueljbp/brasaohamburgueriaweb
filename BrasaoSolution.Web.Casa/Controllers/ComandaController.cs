using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BrasaoSolution.Casa.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace BrasaoSolution.Web.Casa.Controllers
{
    [Route("api/[controller]")]
    public class ComandaController : BaseController
    {
        public ComandaController(BrasaoSolutionContext brasaoContext, IHttpContextAccessor httpContextAccessor) : base(brasaoContext, httpContextAccessor)
        {
        }

        [HttpGet("[action]")]
        public ComandaViewModel GetComanda(string numMesa)
        {
            return JsonConvert.DeserializeObject<ComandaViewModel>(_httpContext.Session.GetString("comanda"));
        }

        [HttpPost("[action]")]
        public ComandaViewModel RegistraPedido([FromBody] ComandaViewModel comanda)
        {
            BrasaoSolutionRepository rep = new BrasaoSolutionRepository(this._brasaoContext);

            comanda.Itens[0].AcaoRegistro = (int)TipoAcaoRegistro.Nenhuma;

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

            base._httpContext.Session.SetString("comanda", JsonConvert.SerializeObject(comanda));

            return comanda;
        }
    }
}