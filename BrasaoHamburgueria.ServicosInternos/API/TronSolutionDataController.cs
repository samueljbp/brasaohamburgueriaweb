using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Threading.Tasks;
using BrasaoHamburgueria.Model;
using BrasaoHamburgueria.TronSolutionData;

namespace BrasaoHamburgueria.ServicosInternos
{
    public class TronSolutionDataController : ApiController
    {
        private Business.TronSolutionBusiness bo = new Business.TronSolutionBusiness();

        [Route("api/TronSolutionData/GetItemCardapio")]
        [HttpGet]
        public ServiceResultViewModel GetItemCardapio()
        {
            return bo.GetItensFromTron();
        }

        [Route("api/TronSolutionData/GetClassesItemCardapio")]
        [HttpGet]
        public ServiceResultViewModel GetClassesItemCardapio()
        {
            return bo.GetClassesFromTron();
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