using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BrasaoHamburgueria.Model
{
    public class PerfilViewModel
    {
        public string id { get; set; }
        public string nome { get; set; }
    }

    public class UsuarioPerfisViewModel
    {
        public string Id { get; set; }
        public String Email { get; set; }
        public String Nome { get; set; }
        public List<PerfilViewModel> Perfis { get; set; }
    }

    public class UsuarioViewModel
    {
        public int Id { get; set; }
        public String Email { get; set; }
        public String Nome { get; set; }
        public String Telefone { get; set; }
        public String Sexo { get; set; }
        public String DataNascimento { get; set; }
        public String Estado { get; set; }
        public int CodCidade { get; set; }
        public string NomeCidade { get; set; }
        public int CodBairro { get; set; }
        public string NomeBairro { get; set; }
        public String Logradouro { get; set; }
        public String Numero { get; set; }
        public String Complemento { get; set; }
        public String Referencia { get; set; }
        public bool Salvar { get; set; }
        public bool ClienteNovo { get; set; }
        public int? CodEmpresaPreferencial { get; set; }
    }

    public class Usuario
    {
        public int Id { get; set; }
        public bool UsuarioExterno { get; set; }
        public String Email { get; set; }
        public String Nome { get; set; }
        public String Telefone { get; set; }
        public String Sexo { get; set; }
        public String DataNascimento { get; set; }
        public String Estado { get; set; }
        public int CodCidade { get; set; }
        public int CodBairro { get; set; }
        public String Logradouro { get; set; }
        public String Numero { get; set; }
        public String Complemento { get; set; }
        public String Referencia { get; set; }
        public int? CodEmpresaPreferencial { get; set; }
    }
}