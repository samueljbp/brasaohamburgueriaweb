using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BrasaoSolution.Casa.Model
{
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