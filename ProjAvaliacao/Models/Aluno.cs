using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace ProjAvaliacao.Models
{
    public class Aluno
    {
        public int Id { get; set; }

        // FK para Identity
        public string UsuarioId { get; set; }

        [ValidateNever]
        public Usuario Usuario { get; set; }

        public string RegistroAcademico { get; set; }
    }
}
