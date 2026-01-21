using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace ProjAvaliacao.Models
{
    public enum StatusMatricula
    {
        Ativa = 1,
        Trancada = 2,
        Cancelada = 3,
        Concluida = 4
    }

    public class Matricula
    {
        public int Id { get; set; }
        public int AlunoId { get; set; }
        [ValidateNever]
        public Aluno Aluno { get; set; }
        public int DisciplinaOfertadaId { get; set; }  // FK
        [ValidateNever]
        public DisciplinaOfertada DisciplinaOfertada { get; set; }

        // Estado da matrícula
        public StatusMatricula Status { get; set; }

        // Contexto temporal
        public DateTime DataMatricula { get; set; }
    }
}
