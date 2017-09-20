using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace ImpressaoBematech
{
    public class ImpressaoBematech4200
    {
        public enum ModeloImpressora
        {
            MP20TH_MP2000_CI_MP2000TH  = 0,
            MP20MI_MP20CI_MP20S = 1,
            BlocosTermicosSerialDTRDSR = 2,
            Bloco112mm = 3,
            ThermalKiosk = 4,
            MP4000TH = 5,
            MP4200TH = 7

        }

        public enum QualidadeImpressao
        {
            Baixa = 0,
            Media = 1,
            Normal = 2,
            Alta = 3,
            Altissima = 4

        }

        public enum TipoLetraImpressao
        {
            Comprimido = 1,
            Normal = 2,
            Elite = 3
        }

        public enum ItalicoImpressao
        {
            Ativado = 1,
            Desativado = 0
        }

        public enum SublinhadoImpressao
        {
            Ativado = 1,
            Desativado = 0
        }

        public enum ExpandidoImpressao
        {
            Ativado = 1,
            Desativado = 0
        }

        public enum NegritoImpressao
        {
            Ativado = 1,
            Desativado = 0
        }

        //Configura o modelo da impressora
        [DllImport("MP2032.dll")]
        public static extern int ConfiguraModeloImpressora(int modelo);

        //Inicia Porta
        [DllImport("MP2032.dll")]
        public static extern int IniciaPorta(String porta);

        //Enviar texto formatado
        [DllImport("MP2032.dll")]
        public static extern int FormataTX(String texto, int TipoLetra, int italico, int sublinhado, int expandido, int enfatizado);

        [DllImport("MP2032.dll")]
        public static extern int BematechTX(String texto);

        [DllImport("MP2032.dll")]
        public static extern int AcionaGuilhotina(int valor);

        [DllImport("MP2032.dll")]
        public static extern int SelecionaQualidadeImpressao(int qualidade);

        //Enviar comandos para a impressora
        [DllImport("MP2032.dll")]
        public static extern int ComandoTX(String comando, int tComando);

        //Fecha a porta
        [DllImport("MP2032.dll")]
        public static extern int FechaPorta();
    }
}
