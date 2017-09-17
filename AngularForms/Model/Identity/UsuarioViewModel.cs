using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AngularForms.Model
{
    public class Usuario
    {
        public int Id { get; set; }
        public bool Salvar { get; set; }
        public String Email { get; set; }
        public String Nome { get; set; }
        public String Telefone { get; set; }
        public String Sexo { get; set; }
        public String DataNascimento { get; set; }
        public String Estado { get; set; }
        public String Cidade { get; set; }
        public String Logradouro { get; set; }
        public String Numero { get; set; }
        public String Complemento { get; set; }
        public String Bairro { get; set; }
        public String Referencia { get; set; }
    }
}