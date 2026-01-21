using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace ProjAvaliacao.Models
{
    public class Professor
    {
        public int Id { get; set; }
        // FK para o Identity
        public string UsuarioId { get; set; }

        [ValidateNever]
        public Usuario Usuario { get; set; }

        public string Nome { get; set; }
        public string Email { get; set; }
    }
}
