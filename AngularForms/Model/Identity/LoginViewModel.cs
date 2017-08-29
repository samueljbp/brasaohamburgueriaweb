using System.ComponentModel.DataAnnotations;

namespace AngularForms.Model
{
    public class LoginViewModel
    {
        [Required]
        public string Usuario { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Senha { get; set; }

        public bool LembrarMe { get; set; }

        public string ReturnUrl { get; set; }
    }
}