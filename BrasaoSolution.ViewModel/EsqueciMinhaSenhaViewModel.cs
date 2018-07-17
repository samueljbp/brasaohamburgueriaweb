using System.ComponentModel.DataAnnotations;

namespace BrasaoSolution.ViewModel
{
    public class EsqueciMinhaSenhaViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}