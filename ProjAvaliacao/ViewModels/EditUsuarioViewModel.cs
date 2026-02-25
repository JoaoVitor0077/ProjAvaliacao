using System.ComponentModel.DataAnnotations;

namespace ProjAvaliacao.ViewModels
{
    public class EditUsuarioViewModel
    {
        [Required]
        public string Id { get; set; }

        [Required]
        [Display(Name = "Nome")]
        public string Nome { get; set; }

        [Required]
        [Display(Name = "Sobrenome")]
        public string Sobrenome { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Display(Name = "Role")]
        public string Role { get; set; }
    }
}
