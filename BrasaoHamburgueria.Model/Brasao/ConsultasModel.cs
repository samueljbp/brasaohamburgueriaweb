using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrasaoHamburgueria.Model
{
    public class ProdutosVendidosViewModel
    {
        public int CodItemCardapio { get; set; }
        public string Nome { get; set; }
        public int CodClasse { get; set; }
        public string DescricaoClasse { get; set; }
        public int Quantidade { get; set; }
        public double ValorTotal { get; set;  }
    }
}
