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
using BrasaoHamburgueria.Helpers;

namespace BrasaoHamburgueria.ServicosInternos.Business
{

    public class PedidoBusiness
    {
        private const int QtdMaximaCaracteresLinha = 48;

        public static string RetornaEspacosCompletar(string descricao, int tamanho, int qtdSubtrair)
        {
            int tamanhoDescricao = descricao.Length;
            int qtdEspacos = tamanho - tamanhoDescricao - qtdSubtrair;
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

        private string RetornaStringDeExtras(List<ExtraItemPedidoViewModel> extras, bool comValor)
        {
            string retorno = "";
            int i = 0;

            foreach (ExtraItemPedidoViewModel extra in extras)
            {
                retorno = retorno + extra.DescricaoOpcaoExtra;
                
                if (comValor)
                {
                    retorno = retorno + " (" + extra.Preco.ToString("C")  + ")";
                }
                
                if (i < extras.Count - 1)
                {
                    retorno = retorno + ", ";
                }
                i++;
            }

            return retorno;
        }

        private string FormataComEspacos(string texto, int tamanho)
        {
            string retorno = "";

            if (texto.Length > tamanho)
            {
                retorno = texto.Substring(0, tamanho);
            }
            else
            {
                retorno = texto + RetornaEspacosCompletar(texto, tamanho, 0);
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
            //declaração da variável para retorno das funções
            int iRetorno = 0;

            pedido.DescricaoFormaPagamento = Helper.Util.getDescricaoFormaPagamentoPedido(pedido.FormaPagamento);

            List<string> portasComanda = new List<string> { "192.168.1.201" };

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

                foreach (var portaComanda in portasComanda)
                {
                    bool sucesso = false;
                    int count = 0;
                    //Abrindo a porta
                    while (!sucesso && count < 3)
                    {
                        iRetorno = ImpressaoBematech4200.IniciaPorta(portaComanda);
                        if (iRetorno != 1)
                        {
                            //se falhou, aguarda 3 segundos para tentar novamente
                            System.Threading.Thread.Sleep(3000);
                        }
                        else
                        {
                            sucesso = true;
                        }
                        count++;
                    }

                    if (!sucesso)
                    {
                        throw new Exception("Falha ao abrir a porta de impressão de produção " + portaComanda + ".");
                    }

                    iRetorno = ImpressaoBematech4200.SelecionaQualidadeImpressao((int)ImpressaoBematech4200.QualidadeImpressao.Baixa);

                    switch (iRetorno)
                    {
                        case 0:
                            throw new Exception("Falha na comunicação com a impressora na porta " + portaComanda + " ao definir a qualidade da impressão.");
                            break;
                        case -4:
                            throw new Exception("Parâmetro inválido com a impressora na porta " + portaComanda + " ao definir a qualidade da impressão.");
                            break;
                        case -5:
                            throw new Exception("Modelo de impressora inválido na porta " + portaComanda + " ao definir a qualidade da impressão.");
                            break;
                    }

                    ImpressaoBematech4200.FormataTX("   RELATORIO GERENCIAL", (int)ImpressaoBematech4200.TipoLetraImpressao.Elite, (int)ImpressaoBematech4200.ItalicoImpressao.Desativado, (int)ImpressaoBematech4200.SublinhadoImpressao.Desativado, (int)ImpressaoBematech4200.ExpandidoImpressao.Ativado, (int)ImpressaoBematech4200.NegritoImpressao.Desativado).ValidaRetornoImpressora(portaComanda);
                    ImpressaoBematech4200.ComandoTX(comandoQuebraLinha, comandoQuebraLinha.Length).ValidaRetornoImpressora(portaComanda);
                    ImpressaoBematech4200.ComandoTX(comandoQuebraLinha, comandoQuebraLinha.Length).ValidaRetornoImpressora(portaComanda);

                    //CABECALHO
                    string texto = "================================================";
                    texto += comandoQuebraLinha;
                    texto += "            PEDIDO DELIVERY NR " + pedido.CodPedido;
                    texto += comandoQuebraLinha;
                    texto += "================================================";
                    texto += comandoQuebraLinha;
                    texto += DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                    texto += comandoQuebraLinha;
                    texto += "Cliente: " + pedido.DadosCliente.Nome;
                    texto += comandoQuebraLinha;
                    texto += "             NÃO É DOCUMENTO FISCAL             ";
                    texto += comandoQuebraLinha;
                    texto += "Endereco: " + pedido.DadosCliente.Logradouro + ", " + pedido.DadosCliente.Numero;
                    texto += comandoQuebraLinha;
                    if (!String.IsNullOrEmpty(pedido.DadosCliente.Complemento))
                    {
                        texto += "Complemento: " + pedido.DadosCliente.Complemento;
                        texto += comandoQuebraLinha;
                    }
                    texto += "Bairro: " + pedido.DadosCliente.Bairro;
                    texto += comandoQuebraLinha;
                    texto += "Telefone: " + pedido.DadosCliente.Telefone;
                    texto += comandoQuebraLinha;
                    if (!String.IsNullOrEmpty(pedido.DadosCliente.Referencia))
                    {
                        texto += pedido.DadosCliente.Referencia;
                        texto += comandoQuebraLinha;
                    }
                    texto += "------------------------------------------------";
                    texto += comandoQuebraLinha;
                    texto += "QTD  DESCRICAO               P.UNIT    TOTAL    ";
                    texto += comandoQuebraLinha;
                    texto += "------------------------------------------------";
                    texto += comandoQuebraLinha;

                    int i = 0;

                    //ITENS
                    foreach (ItemPedidoViewModel item in pedido.Itens)
                    {
                        texto += FormataComEspacos(item.Quantidade.ToString("00"), 5) + FormataComEspacos(item.DescricaoItem, 24) + FormataComEspacos(item.PrecoUnitario.ToString("C"), 10) + FormataComEspacos(item.ValorTotalItem.ToString("C"), 9);
                        texto += comandoQuebraLinha;

                        //ImpressaoBematech4200.FormataTX(item.DescricaoItem + RetornaEspacosCompletar(item.DescricaoItem) + item.Quantidade.ToString("00"), (int)ImpressaoBematech4200.TipoLetraImpressao.Normal, (int)ImpressaoBematech4200.ItalicoImpressao.Desativado, (int)ImpressaoBematech4200.SublinhadoImpressao.Desativado, (int)ImpressaoBematech4200.ExpandidoImpressao.Desativado, (int)ImpressaoBematech4200.NegritoImpressao.Desativado).ValidaRetornoImpressora(portaComanda);
                        //ImpressaoBematech4200.ComandoTX(comandoQuebraLinha, comandoQuebraLinha.Length).ValidaRetornoImpressora(portaComanda);
                        if (item.Obs != null && item.Obs.Count > 0)
                        {
                            var obsString = RetornaStringDeObs(item.Obs);
                            //ImpressaoBematech4200.FormataTX("OBSERVACAO: ", (int)ImpressaoBematech4200.TipoLetraImpressao.Normal, (int)ImpressaoBematech4200.ItalicoImpressao.Desativado, (int)ImpressaoBematech4200.SublinhadoImpressao.Desativado, (int)ImpressaoBematech4200.ExpandidoImpressao.Desativado, (int)ImpressaoBematech4200.NegritoImpressao.Desativado).ValidaRetornoImpressora(portaComanda);
                            //ImpressaoBematech4200.FormataTX(obsString, (int)ImpressaoBematech4200.TipoLetraImpressao.Normal, (int)ImpressaoBematech4200.ItalicoImpressao.Ativado, (int)ImpressaoBematech4200.SublinhadoImpressao.Desativado, (int)ImpressaoBematech4200.ExpandidoImpressao.Desativado, (int)ImpressaoBematech4200.NegritoImpressao.Desativado).ValidaRetornoImpressora(portaComanda);
                            //ImpressaoBematech4200.ComandoTX(comandoQuebraLinha, comandoQuebraLinha.Length).ValidaRetornoImpressora(portaComanda);

                            texto += "     OBSERVACAO: ";
                            texto += obsString;
                            texto += comandoQuebraLinha;
                        }
                        if (item.extras != null && item.extras.Count > 0)
                        {
                            var extrasString = RetornaStringDeExtras(item.extras, true);
                            //ImpressaoBematech4200.FormataTX("EXTRAS: ", (int)ImpressaoBematech4200.TipoLetraImpressao.Normal, (int)ImpressaoBematech4200.ItalicoImpressao.Desativado, (int)ImpressaoBematech4200.SublinhadoImpressao.Desativado, (int)ImpressaoBematech4200.ExpandidoImpressao.Desativado, (int)ImpressaoBematech4200.NegritoImpressao.Desativado).ValidaRetornoImpressora(portaComanda);
                            //ImpressaoBematech4200.FormataTX(extrasString, (int)ImpressaoBematech4200.TipoLetraImpressao.Normal, (int)ImpressaoBematech4200.ItalicoImpressao.Ativado, (int)ImpressaoBematech4200.SublinhadoImpressao.Desativado, (int)ImpressaoBematech4200.ExpandidoImpressao.Desativado, (int)ImpressaoBematech4200.NegritoImpressao.Desativado).ValidaRetornoImpressora(portaComanda);
                            //ImpressaoBematech4200.ComandoTX(comandoQuebraLinha, comandoQuebraLinha.Length).ValidaRetornoImpressora(portaComanda);

                            texto += "     EXTRAS: ";
                            texto += extrasString;
                            texto += comandoQuebraLinha;

                        }

                        if (i < pedido.Itens.Count)
                        {
                            //ImpressaoBematech4200.FormataTX("------------------------------------------------", (int)ImpressaoBematech4200.TipoLetraImpressao.Normal, (int)ImpressaoBematech4200.ItalicoImpressao.Desativado, (int)ImpressaoBematech4200.SublinhadoImpressao.Desativado, (int)ImpressaoBematech4200.ExpandidoImpressao.Desativado, (int)ImpressaoBematech4200.NegritoImpressao.Desativado).ValidaRetornoImpressora(portaComanda);
                            //ImpressaoBematech4200.ComandoTX(comandoQuebraLinha, comandoQuebraLinha.Length).ValidaRetornoImpressora(portaComanda);

                            texto += "------------------------------------------------";
                            texto += comandoQuebraLinha;
                        }

                        i = i + 1;
                    }

                    if (pedido.TaxaEntrega > 0)
                    {
                        var qtdTx = 1;
                        texto += FormataComEspacos(qtdTx.ToString("00"), 5) + FormataComEspacos("Taxa entrega", 24) + FormataComEspacos(pedido.TaxaEntrega.ToString("C"), 10) + FormataComEspacos(pedido.TaxaEntrega.ToString("C"), 9);
                        texto += comandoQuebraLinha;
                        texto += "------------------------------------------------";
                        texto += comandoQuebraLinha;
                    }

                    ImpressaoBematech4200.BematechTX(texto).ValidaRetornoImpressora(portaComanda);

                    ImpressaoBematech4200.FormataTX("     TOTAL: " + pedido.ValorTotal.ToString("C"), (int)ImpressaoBematech4200.TipoLetraImpressao.Elite, (int)ImpressaoBematech4200.ItalicoImpressao.Desativado, (int)ImpressaoBematech4200.SublinhadoImpressao.Desativado, (int)ImpressaoBematech4200.ExpandidoImpressao.Ativado, (int)ImpressaoBematech4200.NegritoImpressao.Desativado).ValidaRetornoImpressora(portaComanda);
                    ImpressaoBematech4200.ComandoTX(comandoQuebraLinha, comandoQuebraLinha.Length).ValidaRetornoImpressora(portaComanda);

                    if (pedido.ValorDesconto != null && pedido.ValorDesconto > 0)
                    {
                        texto += "Desconto: " + pedido.ValorDesconto.Value.ToString("C");
                        texto += comandoQuebraLinha;
                        var valorComDesconto = pedido.ValorTotal - pedido.ValorDesconto.Value;
                        texto += "Total c/ desconto: " + valorComDesconto.ToString("C");
                        texto += comandoQuebraLinha;
                    }

                    texto = "================================================";
                    texto += comandoQuebraLinha;
                    texto += "             NÃO É DOCUMENTO FISCAL             ";
                    texto += comandoQuebraLinha;
                    texto += "================================================";
                    texto += comandoQuebraLinha;
                    texto += "               FORMA DE PAGAMENTO               ";
                    texto += comandoQuebraLinha;
                    texto += pedido.DescricaoFormaPagamento;
                    if (!String.IsNullOrEmpty(pedido.BandeiraCartao))
                    {
                        texto += " - " + pedido.BandeiraCartao;
                        texto += comandoQuebraLinha;
                    }
                    if (pedido.FormaPagamento == "D" && pedido.Troco != null)
                    {
                        texto += " - Troco: " + pedido.Troco.Value.ToString("C");
                        texto += comandoQuebraLinha;
                    }
                    texto += "================================================";
                    texto += comandoQuebraLinha;

                    ImpressaoBematech4200.BematechTX(texto).ValidaRetornoImpressora(portaComanda);

                    ImpressaoBematech4200.FormataTX("     AGUADE A EMISSAO", (int)ImpressaoBematech4200.TipoLetraImpressao.Elite, (int)ImpressaoBematech4200.ItalicoImpressao.Desativado, (int)ImpressaoBematech4200.SublinhadoImpressao.Desativado, (int)ImpressaoBematech4200.ExpandidoImpressao.Ativado, (int)ImpressaoBematech4200.NegritoImpressao.Desativado).ValidaRetornoImpressora(portaComanda);
                    ImpressaoBematech4200.ComandoTX(comandoQuebraLinha, comandoQuebraLinha.Length).ValidaRetornoImpressora(portaComanda);
                    ImpressaoBematech4200.FormataTX("      DO CUPOM FISCAL", (int)ImpressaoBematech4200.TipoLetraImpressao.Elite, (int)ImpressaoBematech4200.ItalicoImpressao.Desativado, (int)ImpressaoBematech4200.SublinhadoImpressao.Desativado, (int)ImpressaoBematech4200.ExpandidoImpressao.Ativado, (int)ImpressaoBematech4200.NegritoImpressao.Desativado).ValidaRetornoImpressora(portaComanda);
                    ImpressaoBematech4200.ComandoTX(comandoQuebraLinha, comandoQuebraLinha.Length).ValidaRetornoImpressora(portaComanda);

                    //Aciona a guilhotina para cortar o papel
                    iRetorno = ImpressaoBematech4200.AcionaGuilhotina(1);

                    switch (iRetorno)
                    {
                        case 0:
                            throw new Exception("Falha na comunicação com a impressora na porta " + portaComanda + " ao acionar a guilhotina.");
                            break;
                        case -2:
                            throw new Exception("Parâmetro inválido com a impressora na porta " + portaComanda + " ao acionar a guilhotina.");
                            break;
                    }

                    //Fechar a porta utilizada
                    iRetorno = ImpressaoBematech4200.FechaPorta();

                    if (iRetorno != 1)
                    {
                        throw new Exception("Falha ao fechar a porta de impressão de produção " + portaComanda + ".");
                    }
                }
            }
            catch (Exception ex)
            {
                result.Succeeded = false;
                result.Errors.Add(ex.Message);
                ImpressaoBematech4200.FechaPorta();
            }

            return result;
        }

        private List<PortaPedido> DividePorPorta(PedidoViewModel pedido)
        {
            List<PortaPedido> retorno = new List<PortaPedido>();
            PortaPedido portaPedido = new PortaPedido();
            List<String> portas = new List<string>();

            foreach (var item in pedido.Itens)
            {
                foreach (var porta in item.PortasImpressaoProducao)
                {
                    if (!portas.Contains(porta))
                    {
                        portas.Add(porta);
                    }
                }
            }

            foreach (var porta in portas)
            {
                portaPedido = new PortaPedido();
                portaPedido.Porta = porta;
                portaPedido.Itens = new List<ItemPedidoViewModel>();

                foreach (var item in pedido.Itens)
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

                foreach (var portaPedido in portasPedido)
                {
                    bool sucesso = false;
                    int count = 0;
                    //Abrindo a porta
                    while (!sucesso && count < 3)
                    {
                        iRetorno = ImpressaoBematech4200.IniciaPorta(portaPedido.Porta);
                        if (iRetorno != 1)
                        {
                            //se falhou, aguarda 3 segundos para tentar novamente
                            System.Threading.Thread.Sleep(3000);
                        }
                        else
                        {
                            sucesso = true;
                        }
                        count++;
                    }

                    if (!sucesso)
                    {
                        throw new Exception("Falha ao abrir a porta de impressão de produção " + portaPedido.Porta + ".");
                    }

                    iRetorno = ImpressaoBematech4200.SelecionaQualidadeImpressao((int)ImpressaoBematech4200.QualidadeImpressao.Baixa);

                    switch (iRetorno)
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
                    string texto = "================================================";
                    texto += comandoQuebraLinha;
                    texto += "            PEDIDO DELIVERY NR " + pedido.CodPedido;
                    texto += comandoQuebraLinha;
                    texto += "================================================";
                    texto += comandoQuebraLinha;
                    texto += DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                    texto += comandoQuebraLinha;
                    texto += "------------------------------------------------";
                    texto += comandoQuebraLinha;
                    texto += "Produto                                      Qtd";
                    texto += comandoQuebraLinha;
                    texto += "------------------------------------------------";
                    texto += comandoQuebraLinha;

                    //ImpressaoBematech4200.FormataTX("================================================", (int)ImpressaoBematech4200.TipoLetraImpressao.Normal, (int)ImpressaoBematech4200.ItalicoImpressao.Desativado, (int)ImpressaoBematech4200.SublinhadoImpressao.Desativado, (int)ImpressaoBematech4200.ExpandidoImpressao.Desativado, (int)ImpressaoBematech4200.NegritoImpressao.Desativado).ValidaRetornoImpressora(portaPedido.Porta);
                    //ImpressaoBematech4200.ComandoTX(comandoQuebraLinha, comandoQuebraLinha.Length).ValidaRetornoImpressora(portaPedido.Porta);
                    //ImpressaoBematech4200.FormataTX("            PEDIDO DELIVERY NR " + pedido.CodPedido, (int)ImpressaoBematech4200.TipoLetraImpressao.Normal, (int)ImpressaoBematech4200.ItalicoImpressao.Desativado, (int)ImpressaoBematech4200.SublinhadoImpressao.Desativado, (int)ImpressaoBematech4200.ExpandidoImpressao.Desativado, (int)ImpressaoBematech4200.NegritoImpressao.Desativado).ValidaRetornoImpressora(portaPedido.Porta);
                    //ImpressaoBematech4200.ComandoTX(comandoQuebraLinha, comandoQuebraLinha.Length).ValidaRetornoImpressora(portaPedido.Porta);
                    //ImpressaoBematech4200.FormataTX("================================================", (int)ImpressaoBematech4200.TipoLetraImpressao.Normal, (int)ImpressaoBematech4200.ItalicoImpressao.Desativado, (int)ImpressaoBematech4200.SublinhadoImpressao.Desativado, (int)ImpressaoBematech4200.ExpandidoImpressao.Desativado, (int)ImpressaoBematech4200.NegritoImpressao.Desativado).ValidaRetornoImpressora(portaPedido.Porta);
                    //ImpressaoBematech4200.ComandoTX(comandoQuebraLinha, comandoQuebraLinha.Length).ValidaRetornoImpressora(portaPedido.Porta);
                    ////iRetorno = ImpressaoBematech4200.FormataTX("Restaurante                  " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), (int)ImpressaoBematech4200.TipoLetraImpressao.Normal, (int)ImpressaoBematech4200.ItalicoImpressao.Desativado, (int)ImpressaoBematech4200.SublinhadoImpressao.Desativado, (int)ImpressaoBematech4200.ExpandidoImpressao.Desativado, (int)ImpressaoBematech4200.NegritoImpressao.Desativado);
                    //ImpressaoBematech4200.FormataTX(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), (int)ImpressaoBematech4200.TipoLetraImpressao.Normal, (int)ImpressaoBematech4200.ItalicoImpressao.Desativado, (int)ImpressaoBematech4200.SublinhadoImpressao.Desativado, (int)ImpressaoBematech4200.ExpandidoImpressao.Desativado, (int)ImpressaoBematech4200.NegritoImpressao.Desativado).ValidaRetornoImpressora(portaPedido.Porta);
                    //ImpressaoBematech4200.ComandoTX(comandoQuebraLinha, comandoQuebraLinha.Length).ValidaRetornoImpressora(portaPedido.Porta);
                    ////iRetorno = ImpressaoBematech4200.FormataTX("Comanda: XXXX", (int)ImpressaoBematech4200.TipoLetraImpressao.Normal, (int)ImpressaoBematech4200.ItalicoImpressao.Desativado, (int)ImpressaoBematech4200.SublinhadoImpressao.Desativado, (int)ImpressaoBematech4200.ExpandidoImpressao.Ativado, (int)ImpressaoBematech4200.NegritoImpressao.Desativado);
                    ////iRetorno = ImpressaoBematech4200.ComandoTX(comandoQuebraLinha, comandoQuebraLinha.Length);
                    //ImpressaoBematech4200.FormataTX("------------------------------------------------", (int)ImpressaoBematech4200.TipoLetraImpressao.Normal, (int)ImpressaoBematech4200.ItalicoImpressao.Desativado, (int)ImpressaoBematech4200.SublinhadoImpressao.Desativado, (int)ImpressaoBematech4200.ExpandidoImpressao.Desativado, (int)ImpressaoBematech4200.NegritoImpressao.Desativado).ValidaRetornoImpressora(portaPedido.Porta);
                    //ImpressaoBematech4200.ComandoTX(comandoQuebraLinha, comandoQuebraLinha.Length).ValidaRetornoImpressora(portaPedido.Porta);

                    //ImpressaoBematech4200.FormataTX("Produto                                      Qtd", (int)ImpressaoBematech4200.TipoLetraImpressao.Normal, (int)ImpressaoBematech4200.ItalicoImpressao.Desativado, (int)ImpressaoBematech4200.SublinhadoImpressao.Desativado, (int)ImpressaoBematech4200.ExpandidoImpressao.Desativado, (int)ImpressaoBematech4200.NegritoImpressao.Desativado).ValidaRetornoImpressora(portaPedido.Porta);
                    //ImpressaoBematech4200.ComandoTX(comandoQuebraLinha, comandoQuebraLinha.Length).ValidaRetornoImpressora(portaPedido.Porta);
                    //ImpressaoBematech4200.FormataTX("------------------------------------------------", (int)ImpressaoBematech4200.TipoLetraImpressao.Normal, (int)ImpressaoBematech4200.ItalicoImpressao.Desativado, (int)ImpressaoBematech4200.SublinhadoImpressao.Desativado, (int)ImpressaoBematech4200.ExpandidoImpressao.Desativado, (int)ImpressaoBematech4200.NegritoImpressao.Desativado).ValidaRetornoImpressora(portaPedido.Porta);
                    //ImpressaoBematech4200.ComandoTX(comandoQuebraLinha, comandoQuebraLinha.Length).ValidaRetornoImpressora(portaPedido.Porta);

                    int i = 0;
                    //ITENS
                    foreach (ItemPedidoViewModel item in portaPedido.Itens)
                    {
                        texto += item.DescricaoItem + RetornaEspacosCompletar(item.DescricaoItem, PedidoBusiness.QtdMaximaCaracteresLinha, 2) + item.Quantidade.ToString("00");
                        texto += comandoQuebraLinha;

                        //ImpressaoBematech4200.FormataTX(item.DescricaoItem + RetornaEspacosCompletar(item.DescricaoItem) + item.Quantidade.ToString("00"), (int)ImpressaoBematech4200.TipoLetraImpressao.Normal, (int)ImpressaoBematech4200.ItalicoImpressao.Desativado, (int)ImpressaoBematech4200.SublinhadoImpressao.Desativado, (int)ImpressaoBematech4200.ExpandidoImpressao.Desativado, (int)ImpressaoBematech4200.NegritoImpressao.Desativado).ValidaRetornoImpressora(portaPedido.Porta);
                        //ImpressaoBematech4200.ComandoTX(comandoQuebraLinha, comandoQuebraLinha.Length).ValidaRetornoImpressora(portaPedido.Porta);
                        if (item.Obs != null && item.Obs.Count > 0)
                        {
                            var obsString = RetornaStringDeObs(item.Obs);
                            //ImpressaoBematech4200.FormataTX("OBSERVACAO: ", (int)ImpressaoBematech4200.TipoLetraImpressao.Normal, (int)ImpressaoBematech4200.ItalicoImpressao.Desativado, (int)ImpressaoBematech4200.SublinhadoImpressao.Desativado, (int)ImpressaoBematech4200.ExpandidoImpressao.Desativado, (int)ImpressaoBematech4200.NegritoImpressao.Desativado).ValidaRetornoImpressora(portaPedido.Porta);
                            //ImpressaoBematech4200.FormataTX(obsString, (int)ImpressaoBematech4200.TipoLetraImpressao.Normal, (int)ImpressaoBematech4200.ItalicoImpressao.Ativado, (int)ImpressaoBematech4200.SublinhadoImpressao.Desativado, (int)ImpressaoBematech4200.ExpandidoImpressao.Desativado, (int)ImpressaoBematech4200.NegritoImpressao.Desativado).ValidaRetornoImpressora(portaPedido.Porta);
                            //ImpressaoBematech4200.ComandoTX(comandoQuebraLinha, comandoQuebraLinha.Length).ValidaRetornoImpressora(portaPedido.Porta);

                            texto += "OBSERVACAO: ";
                            texto += obsString;
                            texto += comandoQuebraLinha;
                        }
                        if (item.extras != null && item.extras.Count > 0)
                        {
                            var extrasString = RetornaStringDeExtras(item.extras, false);
                            //ImpressaoBematech4200.FormataTX("EXTRAS: ", (int)ImpressaoBematech4200.TipoLetraImpressao.Normal, (int)ImpressaoBematech4200.ItalicoImpressao.Desativado, (int)ImpressaoBematech4200.SublinhadoImpressao.Desativado, (int)ImpressaoBematech4200.ExpandidoImpressao.Desativado, (int)ImpressaoBematech4200.NegritoImpressao.Desativado).ValidaRetornoImpressora(portaPedido.Porta);
                            //ImpressaoBematech4200.FormataTX(extrasString, (int)ImpressaoBematech4200.TipoLetraImpressao.Normal, (int)ImpressaoBematech4200.ItalicoImpressao.Ativado, (int)ImpressaoBematech4200.SublinhadoImpressao.Desativado, (int)ImpressaoBematech4200.ExpandidoImpressao.Desativado, (int)ImpressaoBematech4200.NegritoImpressao.Desativado).ValidaRetornoImpressora(portaPedido.Porta);
                            //ImpressaoBematech4200.ComandoTX(comandoQuebraLinha, comandoQuebraLinha.Length).ValidaRetornoImpressora(portaPedido.Porta);

                            texto += "EXTRAS: ";
                            texto += extrasString;
                            texto += comandoQuebraLinha;

                        }

                        if (i < pedido.Itens.Count)
                        {
                            //ImpressaoBematech4200.FormataTX("------------------------------------------------", (int)ImpressaoBematech4200.TipoLetraImpressao.Normal, (int)ImpressaoBematech4200.ItalicoImpressao.Desativado, (int)ImpressaoBematech4200.SublinhadoImpressao.Desativado, (int)ImpressaoBematech4200.ExpandidoImpressao.Desativado, (int)ImpressaoBematech4200.NegritoImpressao.Desativado).ValidaRetornoImpressora(portaPedido.Porta);
                            //ImpressaoBematech4200.ComandoTX(comandoQuebraLinha, comandoQuebraLinha.Length).ValidaRetornoImpressora(portaPedido.Porta);

                            texto += "------------------------------------------------";
                            texto += comandoQuebraLinha;
                        }

                        i = i + 1;
                    }

                    ImpressaoBematech4200.BematechTX(texto).ValidaRetornoImpressora(portaPedido.Porta);

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
                ImpressaoBematech4200.FechaPorta();
            }

            return result;
        }
    }
}