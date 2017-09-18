using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Impressao;
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

            ImpressaoBematech4200 MP2032 = new ImpressaoBematech4200();

            string porta = txtPorta.Text;
            string comandoQuebraLinha = "\r\n";

            try
            {
                //declaração da variável para retorno das funções
                int iRetorno = 0;

                //Função para configurar o modelo da impressora
                iRetorno = ImpressaoBematech4200.ConfiguraModeloImpressora((int)ImpressaoBematech4200.ModeloImpressora.MP4200TH);

                //Abrindo a porta
                iRetorno = ImpressaoBematech4200.IniciaPorta(porta);

                iRetorno = ImpressaoBematech4200.SelecionaQualidadeImpressao((int)ImpressaoBematech4200.QualidadeImpressao.Media);

                //CABECALHO
                iRetorno = ImpressaoBematech4200.FormataTX("================================================", (int)ImpressaoBematech4200.TipoLetraImpressao.Normal, (int)ImpressaoBematech4200.ItalicoImpressao.Desativado, (int)ImpressaoBematech4200.SublinhadoImpressao.Desativado, (int)ImpressaoBematech4200.ExpandidoImpressao.Desativado, (int)ImpressaoBematech4200.NegritoImpressao.Desativado);
                iRetorno = ImpressaoBematech4200.ComandoTX(comandoQuebraLinha, comandoQuebraLinha.Length);
                iRetorno = ImpressaoBematech4200.FormataTX("            PEDIDO DELIVERY NR " + ped.CodPedido, (int)ImpressaoBematech4200.TipoLetraImpressao.Normal, (int)ImpressaoBematech4200.ItalicoImpressao.Desativado, (int)ImpressaoBematech4200.SublinhadoImpressao.Desativado, (int)ImpressaoBematech4200.ExpandidoImpressao.Desativado, (int)ImpressaoBematech4200.NegritoImpressao.Desativado);
                iRetorno = ImpressaoBematech4200.ComandoTX(comandoQuebraLinha, comandoQuebraLinha.Length);
                iRetorno = ImpressaoBematech4200.FormataTX("================================================", (int)ImpressaoBematech4200.TipoLetraImpressao.Normal, (int)ImpressaoBematech4200.ItalicoImpressao.Desativado, (int)ImpressaoBematech4200.SublinhadoImpressao.Desativado, (int)ImpressaoBematech4200.ExpandidoImpressao.Desativado, (int)ImpressaoBematech4200.NegritoImpressao.Desativado);
                iRetorno = ImpressaoBematech4200.ComandoTX(comandoQuebraLinha, comandoQuebraLinha.Length);
                //iRetorno = ImpressaoBematech4200.FormataTX("Restaurante                  " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), (int)ImpressaoBematech4200.TipoLetraImpressao.Normal, (int)ImpressaoBematech4200.ItalicoImpressao.Desativado, (int)ImpressaoBematech4200.SublinhadoImpressao.Desativado, (int)ImpressaoBematech4200.ExpandidoImpressao.Desativado, (int)ImpressaoBematech4200.NegritoImpressao.Desativado);
                iRetorno = ImpressaoBematech4200.FormataTX(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), (int)ImpressaoBematech4200.TipoLetraImpressao.Normal, (int)ImpressaoBematech4200.ItalicoImpressao.Desativado, (int)ImpressaoBematech4200.SublinhadoImpressao.Desativado, (int)ImpressaoBematech4200.ExpandidoImpressao.Desativado, (int)ImpressaoBematech4200.NegritoImpressao.Desativado);
                iRetorno = ImpressaoBematech4200.ComandoTX(comandoQuebraLinha, comandoQuebraLinha.Length);
                //iRetorno = ImpressaoBematech4200.FormataTX("Comanda: XXXX", (int)ImpressaoBematech4200.TipoLetraImpressao.Normal, (int)ImpressaoBematech4200.ItalicoImpressao.Desativado, (int)ImpressaoBematech4200.SublinhadoImpressao.Desativado, (int)ImpressaoBematech4200.ExpandidoImpressao.Ativado, (int)ImpressaoBematech4200.NegritoImpressao.Desativado);
                //iRetorno = ImpressaoBematech4200.ComandoTX(comandoQuebraLinha, comandoQuebraLinha.Length);
                iRetorno = ImpressaoBematech4200.FormataTX("------------------------------------------------", (int)ImpressaoBematech4200.TipoLetraImpressao.Normal, (int)ImpressaoBematech4200.ItalicoImpressao.Desativado, (int)ImpressaoBematech4200.SublinhadoImpressao.Desativado, (int)ImpressaoBematech4200.ExpandidoImpressao.Desativado, (int)ImpressaoBematech4200.NegritoImpressao.Desativado);
                iRetorno = ImpressaoBematech4200.ComandoTX(comandoQuebraLinha, comandoQuebraLinha.Length);

                iRetorno = ImpressaoBematech4200.FormataTX("Produto                                      Qtd", (int)ImpressaoBematech4200.TipoLetraImpressao.Normal, (int)ImpressaoBematech4200.ItalicoImpressao.Desativado, (int)ImpressaoBematech4200.SublinhadoImpressao.Desativado, (int)ImpressaoBematech4200.ExpandidoImpressao.Desativado, (int)ImpressaoBematech4200.NegritoImpressao.Desativado);
                iRetorno = ImpressaoBematech4200.ComandoTX(comandoQuebraLinha, comandoQuebraLinha.Length);
                iRetorno = ImpressaoBematech4200.FormataTX("------------------------------------------------", (int)ImpressaoBematech4200.TipoLetraImpressao.Normal, (int)ImpressaoBematech4200.ItalicoImpressao.Desativado, (int)ImpressaoBematech4200.SublinhadoImpressao.Desativado, (int)ImpressaoBematech4200.ExpandidoImpressao.Desativado, (int)ImpressaoBematech4200.NegritoImpressao.Desativado);
                iRetorno = ImpressaoBematech4200.ComandoTX(comandoQuebraLinha, comandoQuebraLinha.Length);

                int i = 0;
                //ITENS
                foreach(ItemPedidoViewModel item in ped.Itens)
                {
                    iRetorno = ImpressaoBematech4200.FormataTX(item.DescricaoItem + RetornaEspacosCompletar(item.DescricaoItem) + item.Quantidade.ToString("00"), (int)ImpressaoBematech4200.TipoLetraImpressao.Normal, (int)ImpressaoBematech4200.ItalicoImpressao.Desativado, (int)ImpressaoBematech4200.SublinhadoImpressao.Desativado, (int)ImpressaoBematech4200.ExpandidoImpressao.Desativado, (int)ImpressaoBematech4200.NegritoImpressao.Desativado);
                    iRetorno = ImpressaoBematech4200.ComandoTX(comandoQuebraLinha, comandoQuebraLinha.Length);
                    if (item.Obs != null && item.Obs.Count > 0)
                    {
                        var obsString = this.RetornaStringDeObs(item.Obs);
                        iRetorno = ImpressaoBematech4200.FormataTX("OBSERVACAO: ", (int)ImpressaoBematech4200.TipoLetraImpressao.Normal, (int)ImpressaoBematech4200.ItalicoImpressao.Desativado, (int)ImpressaoBematech4200.SublinhadoImpressao.Desativado, (int)ImpressaoBematech4200.ExpandidoImpressao.Desativado, (int)ImpressaoBematech4200.NegritoImpressao.Desativado);
                        iRetorno = ImpressaoBematech4200.FormataTX(obsString, (int)ImpressaoBematech4200.TipoLetraImpressao.Normal, (int)ImpressaoBematech4200.ItalicoImpressao.Ativado, (int)ImpressaoBematech4200.SublinhadoImpressao.Desativado, (int)ImpressaoBematech4200.ExpandidoImpressao.Desativado, (int)ImpressaoBematech4200.NegritoImpressao.Desativado);
                        iRetorno = ImpressaoBematech4200.ComandoTX(comandoQuebraLinha, comandoQuebraLinha.Length);
                    }
                    if (item.extras != null && item.extras.Count > 0)
                    {
                        var extrasString = this.RetornaStringDeExtras(item.extras);
                        iRetorno = ImpressaoBematech4200.FormataTX("EXTRAS: ", (int)ImpressaoBematech4200.TipoLetraImpressao.Normal, (int)ImpressaoBematech4200.ItalicoImpressao.Desativado, (int)ImpressaoBematech4200.SublinhadoImpressao.Desativado, (int)ImpressaoBematech4200.ExpandidoImpressao.Desativado, (int)ImpressaoBematech4200.NegritoImpressao.Desativado);
                        iRetorno = ImpressaoBematech4200.FormataTX(extrasString, (int)ImpressaoBematech4200.TipoLetraImpressao.Normal, (int)ImpressaoBematech4200.ItalicoImpressao.Ativado, (int)ImpressaoBematech4200.SublinhadoImpressao.Desativado, (int)ImpressaoBematech4200.ExpandidoImpressao.Desativado, (int)ImpressaoBematech4200.NegritoImpressao.Desativado);
                        iRetorno = ImpressaoBematech4200.ComandoTX(comandoQuebraLinha, comandoQuebraLinha.Length);
                        
                    }

                    if (i < ped.Itens.Count)
                    {
                        iRetorno = ImpressaoBematech4200.FormataTX("------------------------------------------------", (int)ImpressaoBematech4200.TipoLetraImpressao.Normal, (int)ImpressaoBematech4200.ItalicoImpressao.Desativado, (int)ImpressaoBematech4200.SublinhadoImpressao.Desativado, (int)ImpressaoBematech4200.ExpandidoImpressao.Desativado, (int)ImpressaoBematech4200.NegritoImpressao.Desativado);
                        iRetorno = ImpressaoBematech4200.ComandoTX(comandoQuebraLinha, comandoQuebraLinha.Length);
                    }

                    i = i + 1;
                }

                

                //Função para envio de comandos //declararação da variável para receber o comando
                

                //Comando para salto de linha
                iRetorno = ImpressaoBematech4200.ComandoTX(comandoQuebraLinha, comandoQuebraLinha.Length);

                iRetorno = ImpressaoBematech4200.FormataTX("", 2, 0, 0, 1, 1);
                iRetorno = ImpressaoBematech4200.FormataTX("", 2, 0, 0, 1, 1);
                iRetorno = ImpressaoBematech4200.AcionaGuilhotina(1);

                //Fechar a porta utilizada
                iRetorno = ImpressaoBematech4200.FechaPorta();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
