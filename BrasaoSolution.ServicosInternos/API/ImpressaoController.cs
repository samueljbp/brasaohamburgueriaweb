using System.Collections.Generic;
using System.Web.Http;
using BrasaoSolution.Model;
using Newtonsoft.Json.Linq;
using BrasaoSolution.ViewModel;

namespace BrasaoSolution.ServicosInternos
{
    public class ImpressaoController : ApiController
    {
        private Business.PedidoBusiness bo = new Business.PedidoBusiness();

        [Route("api/Impressao/ImprimePedido")]
        [HttpPost] // There are HttpGet, HttpPost, HttpPut, HttpDelete.
        public ServiceResultViewModel ImprimePedido([FromBody]JObject data)
        {
            PedidoViewModel pedido = data["pedido"].ToObject<PedidoViewModel>();
            bool imprimeComandaCozinha = data["imprimeComandaCozinha"].ToObject<bool>();
            string portaImpressoraCozinha = data["portaImpressoraCozinha"].ToObject<string>();

            var retorno = bo.ImprimeComandaPedido(pedido);

            if (retorno.Succeeded)
            {
                retorno = bo.ImprimeItensProducao(pedido, imprimeComandaCozinha, portaImpressoraCozinha);
            }

            return retorno;
        }

        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}