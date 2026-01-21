using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace ProjAvaliacao.Models
{
    public class Avaliacao
    {
        public int Id { get; set; }

        // FK para a matrícula
        public int MatriculaId { get; set; }

        [ValidateNever]
        public Matricula Matricula { get; set; }

        // Nota da disciplina
        [Range(1, 5)]
        public int Nota { get; set; }

        // Comentário opcional
        [StringLength(1000)]
        public string Comentario { get; set; }

        // Indicador de recomendação
        public bool Recomendaria { get; set; }

        // Auditoria
        public DateTime DataAvaliacao { get; set; }
    }
}
