using System;
using System.Collections.Generic;

namespace BrasaoSolution.ViewModel
{
    public class EmpresaViewModel
    {
        public int CodEmpresa { get; set; }
        public bool EhFilial { get; set; }
        public int? CodEmpresaMatriz { get; set; }
        public string RazaoSocial { get; set; }
        public string NomeFantasia { get; set; }
        public string CNPJ { get; set; }
        public string InscricaoEstadual { get; set; }
        public string Estado { get; set; }
        public int CodCidade { get; set; }
        public string NomeCidade { get; set; }
        public int CodBairro { get; set; }
        public string NomeBairro { get; set; }
        public String Logradouro { get; set; }
        public String Numero { get; set; }
        public String Complemento { get; set; }
        public string Telefone { get; set; }
        public string Logomarca { get; set; }
        public string LogomarcaMini { get; set; }
        public string Email { get; set; }
        public string Facebook { get; set; }
        public string ImagemBackgroundPublica { get; set; }
        public string ImagemBackgroundPublicaMini { get; set; }
        public string ImagemBackgroundAutenticada { get; set; }
        public string ImagemBackgroundAutenticadaMini { get; set; }
        public bool EmpresaAtiva { get; set; }
        public string CorPrincipal { get; set; }
        public string CorSecundaria { get; set; }
        public string CorPrincipalContraste { get; set; }
        public string CorDestaque { get; set; }
        public string TextoInstitucional { get; set; }
        public string UrlSite { get; set; }
        public string CasaAberta { get; set; }

        public List<ImagemInstitucionalViewModel> ImagensInstitucionais { get; set; }
    }

    public class ImagemInstitucionalViewModel
    {
        public string Imagem { get; set; }
        public string ImagemMini { get; set; }
    }

    public class EstadoViewModel
    {
        public string Sigla { get; set; }
        public string Nome { get; set; }
    }

    public class CidadeViewModel
    {
        public int CodCidade { get; set; }
        public string Nome { get; set; }
        public string Estado { get; set; }
    }

    public class BairroViewModel
    {
        public int CodBairro { get; set; }
        public string Nome { get; set; }
        public int CodCidade { get; set; }
    }
}