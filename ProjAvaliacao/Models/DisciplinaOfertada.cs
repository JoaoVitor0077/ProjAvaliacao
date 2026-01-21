using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace ProjAvaliacao.Models
{
    public class DisciplinaOfertada
    {
        public int Id { get; set; }

        public int DisciplinaId { get; set; }
        [ValidateNever]
        public Disciplina Disciplina { get; set; }

        public int ProfessorId { get; set; }
        [ValidateNever]
        public Professor Professor { get; set; }

        [Range(2000, 2100)]
        public int Ano { get; set; }

        [Range(1, 2)]
        public int Semestre { get; set; }

        public bool Ativa { get; set; }

        // RELAÇÃO 1:N COM MATRÍCULA
        [ValidateNever]
        public ICollection<Matricula> Matriculas { get; set; }

    }
}
