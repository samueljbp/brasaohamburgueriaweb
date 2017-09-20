using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BrasaoHamburgueria.Model
{
    public static class UsuarioCopy
    {
        public static void ViewModelToDB(UsuarioViewModel viewModel, Usuario usuario)
        {
            usuario.Bairro = viewModel.Bairro;
            usuario.Cidade = viewModel.Cidade;
            usuario.Complemento = viewModel.Complemento;
            usuario.DataNascimento = viewModel.DataNascimento;
            usuario.Email = viewModel.Email;
            usuario.Estado = viewModel.Estado;
            usuario.Logradouro = viewModel.Logradouro;
            usuario.Nome = viewModel.Nome;
            usuario.Numero = viewModel.Numero;
            usuario.Referencia = viewModel.Referencia;
            usuario.Sexo = viewModel.Sexo;
            usuario.Telefone = viewModel.Telefone;
            usuario.UsuarioExterno = viewModel.ClienteNovo;
        }

        public static void DBToViewModel(Usuario usuario, UsuarioViewModel viewModel)
        {
            viewModel.Bairro = usuario.Bairro;
            viewModel.Cidade = usuario.Cidade;
            viewModel.ClienteNovo = false;
            viewModel.Complemento = usuario.Complemento;
            viewModel.DataNascimento = usuario.DataNascimento;
            viewModel.Email = usuario.Email;
            viewModel.Estado = usuario.Estado;
            viewModel.Id = usuario.Id;
            viewModel.Logradouro = usuario.Logradouro;
            viewModel.Nome = usuario.Nome;
            viewModel.Numero = usuario.Numero;
            viewModel.Referencia = usuario.Referencia;
            viewModel.Salvar = false;
            viewModel.Sexo = usuario.Sexo;
            viewModel.Telefone = usuario.Telefone;
        }
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
        public String Cidade { get; set; }
        public String Logradouro { get; set; }
        public String Numero { get; set; }
        public String Complemento { get; set; }
        public String Bairro { get; set; }
        public String Referencia { get; set; }
        public bool Salvar { get; set; }
        public bool ClienteNovo { get; set; }
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
        public String Cidade { get; set; }
        public String Logradouro { get; set; }
        public String Numero { get; set; }
        public String Complemento { get; set; }
        public String Bairro { get; set; }
        public String Referencia { get; set; }
    }
}