using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BrasaoHamburgueria.Model;
using ImpressaoBematech;
using System.Runtime.CompilerServices;
using ExtensionMethods;
using System.Web.Http;
using System.Threading.Tasks;

namespace BrasaoHamburgueria.ServicosInternos.Business
{
    
    public class PedidoBusiness
    {
        private const int QtdMaximaCaracteresLinha = 48;

        public static string RetornaEspacosCompletar(string descricao)
        {
            int tamanhoDescricao = descricao.Length;
            int qtdEspacos = PedidoBusiness.QtdMaximaCaracteresLinha - tamanhoDescricao - 2; //2 é o tamanho da quantidade
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

            foreach (ObservacaoItemPedidoViewModel obs in obss)
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

        private class PortaPedido
        {
            public string Porta { get; set; }
            public List<ItemPedidoViewModel> Itens { get; set; }
        }

        //Este método recebe o pedido completo para impressão da comanda
        public ServiceResultViewModel ImprimeComandaPedido(PedidoViewModel pedido)
        {
            ServiceResultViewModel result = new ServiceResultViewModel { Succeeded = true, Errors = new List<string>(), data = null };

            return result;
        }

        private List<PortaPedido> DividePorPorta(PedidoViewModel pedido)
        {
            List<PortaPedido> retorno = new List<PortaPedido>();
            PortaPedido portaPedido = new PortaPedido();
            List<String> portas = new List<string>();

            foreach(var item in pedido.Itens)
            {
                foreach(var porta in item.PortasImpressaoProducao)
                {
                    if (!portas.Contains(porta))
                    {
                        portas.Add(porta);
                    }
                }
            }

            foreach(var porta in portas)
            {
                portaPedido = new PortaPedido();
                portaPedido.Porta = porta;
                portaPedido.Itens = new List<ItemPedidoViewModel>();

                foreach(var item in pedido.Itens)
                {
                    if (item.PortasImpressaoProducao.Contains(porta))
                    {
                        portaPedido.Itens.Add(item);
                    }
                }

                retorno.Add(portaPedido);
            }

            return retorno;
        }

        //Este método já recebe o pedido com os itens filtrados somente para a porta desejada
        public ServiceResultViewModel ImprimeItensProducao(PedidoViewModel pedido)
        {
            //declaração da variável para retorno das funções
            int iRetorno = 0;

            List<PortaPedido> portasPedido = DividePorPorta(pedido);
            
            ServiceResultViewModel result = new ServiceResultViewModel { Succeeded = true, Errors = new List<string>(), data = null };

            ImpressaoBematech4200 MP2032 = new ImpressaoBematech4200();

            string comandoQuebraLinha = "\r\n";

            //imprime os pedidos nas impressoras de produção
            try
            {
                //Função para configurar o modelo da impressora
                iRetorno = ImpressaoBematech4200.ConfiguraModeloImpressora((int)ImpressaoBematech4200.ModeloImpressora.MP4200TH);

                if (iRetorno != 1)
                {
                    throw new Exception("Parâmetro de modelo de impressora inválido. ");
                }

                foreach(var portaPedido in portasPedido)
                {
                    //Abrindo a porta
                    iRetorno = ImpressaoBematech4200.IniciaPorta(portaPedido.Porta);

                    if (iRetorno != 1)
                    {
                        throw new Exception("Falha ao abrir a porta de impressão de produção " + portaPedido.Porta + ".");
                    }

                    iRetorno = ImpressaoBematech4200.SelecionaQualidadeImpressao((int)ImpressaoBematech4200.QualidadeImpressao.Media);

                    switch(iRetorno)
                    {
                        case 0:
                            throw new Exception("Falha na comunicação com a impressora na porta " + portaPedido.Porta + " ao definir a qualidade da impressão.");
                            break;
                        case -4:
                            throw new Exception("Parâmetro inválido com a impressora na porta " + portaPedido.Porta + " ao definir a qualidade da impressão.");
                            break;
                        case -5:
                            throw new Exception("Modelo de impressora inválido na porta " + portaPedido.Porta + " ao definir a qualidade da impressão.");
                            break;
                    }

                    //CABECALHO
                    ImpressaoBematech4200.FormataTX("================================================", (int)ImpressaoBematech4200.TipoLetraImpressao.Normal, (int)ImpressaoBematech4200.ItalicoImpressao.Desativado, (int)ImpressaoBematech4200.SublinhadoImpressao.Desativado, (int)ImpressaoBematech4200.ExpandidoImpressao.Desativado, (int)ImpressaoBematech4200.NegritoImpressao.Desativado).ValidaRetornoImpressora(portaPedido.Porta);
                    ImpressaoBematech4200.ComandoTX(comandoQuebraLinha, comandoQuebraLinha.Length).ValidaRetornoImpressora(portaPedido.Porta);
                    ImpressaoBematech4200.FormataTX("            PEDIDO DELIVERY NR " + pedido.CodPedido, (int)ImpressaoBematech4200.TipoLetraImpressao.Normal, (int)ImpressaoBematech4200.ItalicoImpressao.Desativado, (int)ImpressaoBematech4200.SublinhadoImpressao.Desativado, (int)ImpressaoBematech4200.ExpandidoImpressao.Desativado, (int)ImpressaoBematech4200.NegritoImpressao.Desativado).ValidaRetornoImpressora(portaPedido.Porta);
                    ImpressaoBematech4200.ComandoTX(comandoQuebraLinha, comandoQuebraLinha.Length).ValidaRetornoImpressora(portaPedido.Porta);
                    ImpressaoBematech4200.FormataTX("================================================", (int)ImpressaoBematech4200.TipoLetraImpressao.Normal, (int)ImpressaoBematech4200.ItalicoImpressao.Desativado, (int)ImpressaoBematech4200.SublinhadoImpressao.Desativado, (int)ImpressaoBematech4200.ExpandidoImpressao.Desativado, (int)ImpressaoBematech4200.NegritoImpressao.Desativado).ValidaRetornoImpressora(portaPedido.Porta);
                    ImpressaoBematech4200.ComandoTX(comandoQuebraLinha, comandoQuebraLinha.Length).ValidaRetornoImpressora(portaPedido.Porta);
                    //iRetorno = ImpressaoBematech4200.FormataTX("Restaurante                  " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), (int)ImpressaoBematech4200.TipoLetraImpressao.Normal, (int)ImpressaoBematech4200.ItalicoImpressao.Desativado, (int)ImpressaoBematech4200.SublinhadoImpressao.Desativado, (int)ImpressaoBematech4200.ExpandidoImpressao.Desativado, (int)ImpressaoBematech4200.NegritoImpressao.Desativado);
                    ImpressaoBematech4200.FormataTX(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), (int)ImpressaoBematech4200.TipoLetraImpressao.Normal, (int)ImpressaoBematech4200.ItalicoImpressao.Desativado, (int)ImpressaoBematech4200.SublinhadoImpressao.Desativado, (int)ImpressaoBematech4200.ExpandidoImpressao.Desativado, (int)ImpressaoBematech4200.NegritoImpressao.Desativado).ValidaRetornoImpressora(portaPedido.Porta);
                    ImpressaoBematech4200.ComandoTX(comandoQuebraLinha, comandoQuebraLinha.Length).ValidaRetornoImpressora(portaPedido.Porta);
                    //iRetorno = ImpressaoBematech4200.FormataTX("Comanda: XXXX", (int)ImpressaoBematech4200.TipoLetraImpressao.Normal, (int)ImpressaoBematech4200.ItalicoImpressao.Desativado, (int)ImpressaoBematech4200.SublinhadoImpressao.Desativado, (int)ImpressaoBematech4200.ExpandidoImpressao.Ativado, (int)ImpressaoBematech4200.NegritoImpressao.Desativado);
                    //iRetorno = ImpressaoBematech4200.ComandoTX(comandoQuebraLinha, comandoQuebraLinha.Length);
                    ImpressaoBematech4200.FormataTX("------------------------------------------------", (int)ImpressaoBematech4200.TipoLetraImpressao.Normal, (int)ImpressaoBematech4200.ItalicoImpressao.Desativado, (int)ImpressaoBematech4200.SublinhadoImpressao.Desativado, (int)ImpressaoBematech4200.ExpandidoImpressao.Desativado, (int)ImpressaoBematech4200.NegritoImpressao.Desativado).ValidaRetornoImpressora(portaPedido.Porta);
                    ImpressaoBematech4200.ComandoTX(comandoQuebraLinha, comandoQuebraLinha.Length).ValidaRetornoImpressora(portaPedido.Porta);

                    ImpressaoBematech4200.FormataTX("Produto                                      Qtd", (int)ImpressaoBematech4200.TipoLetraImpressao.Normal, (int)ImpressaoBematech4200.ItalicoImpressao.Desativado, (int)ImpressaoBematech4200.SublinhadoImpressao.Desativado, (int)ImpressaoBematech4200.ExpandidoImpressao.Desativado, (int)ImpressaoBematech4200.NegritoImpressao.Desativado).ValidaRetornoImpressora(portaPedido.Porta);
                    ImpressaoBematech4200.ComandoTX(comandoQuebraLinha, comandoQuebraLinha.Length).ValidaRetornoImpressora(portaPedido.Porta);
                    ImpressaoBematech4200.FormataTX("------------------------------------------------", (int)ImpressaoBematech4200.TipoLetraImpressao.Normal, (int)ImpressaoBematech4200.ItalicoImpressao.Desativado, (int)ImpressaoBematech4200.SublinhadoImpressao.Desativado, (int)ImpressaoBematech4200.ExpandidoImpressao.Desativado, (int)ImpressaoBematech4200.NegritoImpressao.Desativado).ValidaRetornoImpressora(portaPedido.Porta);
                    ImpressaoBematech4200.ComandoTX(comandoQuebraLinha, comandoQuebraLinha.Length).ValidaRetornoImpressora(portaPedido.Porta);

                    int i = 0;
                    //ITENS
                    foreach (ItemPedidoViewModel item in portaPedido.Itens)
                    {
                        ImpressaoBematech4200.FormataTX(item.DescricaoItem + RetornaEspacosCompletar(item.DescricaoItem) + item.Quantidade.ToString("00"), (int)ImpressaoBematech4200.TipoLetraImpressao.Normal, (int)ImpressaoBematech4200.ItalicoImpressao.Desativado, (int)ImpressaoBematech4200.SublinhadoImpressao.Desativado, (int)ImpressaoBematech4200.ExpandidoImpressao.Desativado, (int)ImpressaoBematech4200.NegritoImpressao.Desativado).ValidaRetornoImpressora(portaPedido.Porta);
                        ImpressaoBematech4200.ComandoTX(comandoQuebraLinha, comandoQuebraLinha.Length).ValidaRetornoImpressora(portaPedido.Porta);
                        if (item.Obs != null && item.Obs.Count > 0)
                        {
                            var obsString = RetornaStringDeObs(item.Obs);
                            ImpressaoBematech4200.FormataTX("OBSERVACAO: ", (int)ImpressaoBematech4200.TipoLetraImpressao.Normal, (int)ImpressaoBematech4200.ItalicoImpressao.Desativado, (int)ImpressaoBematech4200.SublinhadoImpressao.Desativado, (int)ImpressaoBematech4200.ExpandidoImpressao.Desativado, (int)ImpressaoBematech4200.NegritoImpressao.Desativado).ValidaRetornoImpressora(portaPedido.Porta);
                            ImpressaoBematech4200.FormataTX(obsString, (int)ImpressaoBematech4200.TipoLetraImpressao.Normal, (int)ImpressaoBematech4200.ItalicoImpressao.Ativado, (int)ImpressaoBematech4200.SublinhadoImpressao.Desativado, (int)ImpressaoBematech4200.ExpandidoImpressao.Desativado, (int)ImpressaoBematech4200.NegritoImpressao.Desativado).ValidaRetornoImpressora(portaPedido.Porta);
                            ImpressaoBematech4200.ComandoTX(comandoQuebraLinha, comandoQuebraLinha.Length).ValidaRetornoImpressora(portaPedido.Porta);
                        }
                        if (item.extras != null && item.extras.Count > 0)
                        {
                            var extrasString = RetornaStringDeExtras(item.extras);
                            ImpressaoBematech4200.FormataTX("EXTRAS: ", (int)ImpressaoBematech4200.TipoLetraImpressao.Normal, (int)ImpressaoBematech4200.ItalicoImpressao.Desativado, (int)ImpressaoBematech4200.SublinhadoImpressao.Desativado, (int)ImpressaoBematech4200.ExpandidoImpressao.Desativado, (int)ImpressaoBematech4200.NegritoImpressao.Desativado).ValidaRetornoImpressora(portaPedido.Porta);
                            ImpressaoBematech4200.FormataTX(extrasString, (int)ImpressaoBematech4200.TipoLetraImpressao.Normal, (int)ImpressaoBematech4200.ItalicoImpressao.Ativado, (int)ImpressaoBematech4200.SublinhadoImpressao.Desativado, (int)ImpressaoBematech4200.ExpandidoImpressao.Desativado, (int)ImpressaoBematech4200.NegritoImpressao.Desativado).ValidaRetornoImpressora(portaPedido.Porta);
                            ImpressaoBematech4200.ComandoTX(comandoQuebraLinha, comandoQuebraLinha.Length).ValidaRetornoImpressora(portaPedido.Porta);

                        }

                        if (i < pedido.Itens.Count)
                        {
                            ImpressaoBematech4200.FormataTX("------------------------------------------------", (int)ImpressaoBematech4200.TipoLetraImpressao.Normal, (int)ImpressaoBematech4200.ItalicoImpressao.Desativado, (int)ImpressaoBematech4200.SublinhadoImpressao.Desativado, (int)ImpressaoBematech4200.ExpandidoImpressao.Desativado, (int)ImpressaoBematech4200.NegritoImpressao.Desativado).ValidaRetornoImpressora(portaPedido.Porta);
                            ImpressaoBematech4200.ComandoTX(comandoQuebraLinha, comandoQuebraLinha.Length).ValidaRetornoImpressora(portaPedido.Porta);
                        }

                        i = i + 1;
                    }

                    //Aciona a guilhotina para cortar o papel
                    iRetorno = ImpressaoBematech4200.AcionaGuilhotina(1);

                    switch (iRetorno)
                    {
                        case 0:
                            throw new Exception("Falha na comunicação com a impressora na porta " + portaPedido.Porta + " ao acionar a guilhotina.");
                            break;
                        case -2:
                            throw new Exception("Parâmetro inválido com a impressora na porta " + portaPedido.Porta + " ao acionar a guilhotina.");
                            break;
                    }

                    //Fechar a porta utilizada
                    iRetorno = ImpressaoBematech4200.FechaPorta();

                    if (iRetorno != 1)
                    {
                        throw new Exception("Falha ao fechar a porta de impressão de produção " + portaPedido.Porta + ".");
                    }
                }
            }
            catch (Exception ex)
            {
                result.Succeeded = false;
                result.Errors.Add(ex.Message);
            }

            return result;
        }
    }
}