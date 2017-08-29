using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AngularForms.Model
{
    public class EsqueciMinhaSenhaViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}