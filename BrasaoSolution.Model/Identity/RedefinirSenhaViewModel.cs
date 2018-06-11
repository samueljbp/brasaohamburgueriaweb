using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BrasaoSolution.Model
{
    public class RedefinirSenhaViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Senha { get; set; }

        [DataType(DataType.Password)]
        public string SenhaConfirmada { get; set; }

        public string Code { get; set; }
    }
}