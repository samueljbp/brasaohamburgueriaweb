using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Threading.Tasks;
using BrasaoHamburgueria.Model;

namespace BrasaoHamburgueria.ServicosInternos
{
    public class ImpressaoController : ApiController
    {
        private Business.PedidoBusiness bo = new Business.PedidoBusiness();

        [Route("api/Impressao/ImprimeItensProducao")]
        [HttpPost] // There are HttpGet, HttpPost, HttpPut, HttpDelete.
        public ServiceResultViewModel ImprimeItensProducao(PedidoViewModel model)
        {
            return bo.ImprimeItensProducao(model);
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