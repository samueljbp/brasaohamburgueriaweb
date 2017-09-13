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

    public class LoginExternoViewModel
    {
        public string ReturnURL { get; set; }
    }

    public class ConfirmacaoLoginExternoViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }

        public string Provider { get; set; }
    }
}