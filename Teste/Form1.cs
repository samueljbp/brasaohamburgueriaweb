using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ImpressaoBematech;
using BrasaoHamburgueria.Model;

namespace Teste
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private const int QtdMaximaCaracteresLinha = 48;

        private PedidoViewModel MontaPedidoTeste()
        {
            PedidoViewModel ped = new PedidoViewModel();
            ped.CodPedido = 1234;
            ped.Itens = new List<ItemPedidoViewModel>();

            ItemPedidoViewModel item = new ItemPedidoViewModel();
            item.CodItem = 1;
            item.DescricaoItem = "Roberto Carlos";
            item.Quantidade = 2;
            item.Obs = new List<ObservacaoItemPedidoViewModel>();
            ObservacaoItemPedidoViewModel obs = new ObservacaoItemPedidoViewModel();
            obs.CodObservacao = 1;
            obs.DescricaoObservacao = "Ponto da casa";
            item.Obs.Add(obs);
            obs = new ObservacaoItemPedidoViewModel();
            obs.CodObservacao = 2;
            obs.DescricaoObservacao = "Mal passado mesmo";
            item.Obs.Add(obs);
            item.extras = new List<ExtraItemPedidoViewModel>();
            ExtraItemPedidoViewModel extra = new ExtraItemPedidoViewModel();
            extra.CodOpcaoExtra= 1;
            extra.DescricaoOpcaoExtra = "Bacon extra";
            item.extras.Add(extra);
            ped.Itens.Add(item);

            item = new ItemPedidoViewModel();
            item.CodItem = 2;
            item.DescricaoItem = "Coca cola lata";
            item.Quantidade = 1;
            ped.Itens.Add(item);

            return ped;
        }

        private string RetornaEspacosCompletar(string descricao)
        {
            int tamanhoDescricao = descricao.Length;
            int qtdEspacos = Form1.QtdMaximaCaracteresLinha - tamanhoDescricao - 2; //2 é o tamanho da quantidade
            string retorno = "";

            for (int i = 0; i < qtdEspacos; i++)
            {
                retorno = retorno + " ";
            }

            return retorno;
        }

        private string RetornaStringDeObs(List<ObservacaoItemPedidoViewModel> obss)
        {
            string retorno = "";
            int i = 0;

            foreach(ObservacaoItemPedidoViewModel obs in obss)
            {
                retorno = retorno + obs.DescricaoObservacao;
                if (i < obss.Count - 1)
                {
                    retorno = retorno + ", ";
                }
                i++;
            }

            return retorno;
        }

        private string RetornaStringDeExtras(List<ExtraItemPedidoViewModel> extras)
        {
            string retorno = "";
            int i = 0;

            foreach (ExtraItemPedidoViewModel extra in extras)
            {
                retorno = retorno + extra.DescricaoOpcaoExtra;
                if (i < extras.Count - 1)
                {
                    retorno = retorno + ", ";
                }
                i++;
            }

            return retorno;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            PedidoViewModel ped = this.MontaPedidoTeste();
            ped.PortasImpressaoComandaEntrega.Add(txtPorta.Text);
            BrasaoHamburgueria.ServicosInternos.Business.PedidoBusiness bo = new BrasaoHamburgueria.ServicosInternos.Business.PedidoBusiness();

            var result = bo.ImprimeItensProducao(ped);
            
            if (result.Succeeded)
            {
                MessageBox.Show("Sucesso na impressão!");
            }
            else
            {
                MessageBox.Show("Erro: " + result.Errors[0].ToString());
            }
        }
    }
}
