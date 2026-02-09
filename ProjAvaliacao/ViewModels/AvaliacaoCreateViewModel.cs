using System.ComponentModel.DataAnnotations;

namespace ProjAvaliacao.ViewModels
{
    public class AvaliacaoCreateViewModel
    {
        // Identificador da matrícula (oculto na tela)
        [Required]
        public int MatriculaId { get; set; }

        // Informações apenas para exibição
        public string Disciplina { get; set; }
        public string Professor { get; set; }
        public string Periodo { get; set; }

        // Campos preenchidos pelo aluno
        [Required]
        [Range(1, 5)]
        public int Nota { get; set; }

        [StringLength(1000)]
        public string Comentario { get; set; }

        public bool Recomendaria { get; set; }
    }
}
