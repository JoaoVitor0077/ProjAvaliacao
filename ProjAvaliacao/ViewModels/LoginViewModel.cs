using System.ComponentModel.DataAnnotations;

namespace ProjAvaliacao.ViewModels{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Senha { get; set; }

        public bool LembrarMe { get; set; }
    }
}
