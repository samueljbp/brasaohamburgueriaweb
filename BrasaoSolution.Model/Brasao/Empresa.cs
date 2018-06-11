using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;

namespace BrasaoSolution.Model
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

    [Table("CIDADE")]
    public class Cidade
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.None)]
        [Key]
        [Column("COD_CIDADE")]
        public int CodCidade { get; set; }

        [Required]
        [Column("NOME")]
        public string Nome { get; set; }

        [Required]
        [Column("ESTADO")]
        public string Estado { get; set; }
    }

    [Table("BAIRRO")]
    public class Bairro
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.None)]
        [Key]
        [Column("COD_BAIRRO")]
        public int CodBairro { get; set; }

        [Required]
        [Column("NOME")]
        public string Nome { get; set; }

        [Required]
        [Column("CIDADE")]
        [ForeignKey("Cidade")]
        public int CodCidade { get; set; }

        public virtual Cidade Cidade { get; set; }
    }

    [Table("EMPRESA")]
    public class Empresa
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.None)]
        [Key]
        [Column("COD_EMPRESA")]
        public int CodEmpresa { get; set; }

        [Column("COD_EMPRESA_MATRIZ")]
        public int? CodEmpresaMatriz { get; set; }

        [Required]
        [Column("RAZAO_SOCIAL")]
        public string RazaoSocial { get; set; }

        [Required]
        [Column("NOME_FANTASIA")]
        public string NomeFantasia { get; set; }

        [Required]
        [Column("CNPJ")]
        public string CNPJ { get; set; }

        [Column("INSCRICAO_ESTADUAL")]
        public string InscricaoEstadual { get; set; }

        [Required]
        [ForeignKey("Bairro")]
        [Column("COD_BAIRRO")]
        public int CodBairro { get; set; }

        [Required]
        [Column("LOGRADOURO")]
        public String Logradouro { get; set; }

        [Required]
        [Column("NUMERO")]
        public String Numero { get; set; }

        [Column("COMPLEMENTO")]
        public String Complemento { get; set; }

        [Required]
        [Column("TELEFONE")]
        public string Telefone { get; set; }

        [Column("EMAIL")]
        public string Email { get; set; }

        [Column("FACEBOOK")]
        public string Facebook { get; set; }

        [Column("TEXTO_INSTITUCIONAL")]
        public string TextoInstitucional { get; set; }

        [Column("LOGOMARCA")]
        public string Logomarca { get; set; }

        [Column("IMAGEM_BACKGROUND_PUBLICA")]
        public string ImagemBackgroundPublica { get; set; }

        [Column("IMAGEM_BACKGROUND_AUTENTICADA")]
        public string ImagemBackgroundAutenticada { get; set; }

        [Required]
        [Column("COR_PRINCIPAL")]
        public string CorPrincipal { get; set; }

        [Required]
        [Column("COR_SECUNDARIA")]
        public string CorSecundaria { get; set; }

        [Required]
        [Column("COR_PRINCIPAL_CONTRASTE")]
        public string CorPrincipalContraste { get; set; }

        [Required]
        [Column("COR_DESTAQUE")]
        public string CorDestaque { get; set; }

        [Column("URL_SITE")]
        public string UrlSite { get; set; }

        [Required]
        [Column("EMPRESA_ATIVA")]
        public bool EmpresaAtiva { get; set; }

        [Required]
        [Column("CASA_ABERTA")]
        public string CasaAberta { get; set; }

        public virtual Bairro Bairro { get; set; }

    }
}