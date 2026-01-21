namespace ProjAvaliacao.ViewModels
{
    public class ProfessorDashboardViewModel
    {
        public string Disciplina { get; set; }
        public int Ano { get; set; }
        public int Semestre { get; set; }

        public int TotalAvaliacoes { get; set; }
        public double MediaNota { get; set; }
        public double PercentualRecomendacao { get; set; }
    }
}
