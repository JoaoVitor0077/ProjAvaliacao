using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProjAvaliacao.Models;

namespace ProjAvaliacao.Data
{
    public class ApplicationDbContext : IdentityDbContext<Usuario>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ProjAvaliacao.Models.Avaliacao> Avaliacoes { get; set; }
        public DbSet<ProjAvaliacao.Models.Matricula> Matriculas { get; set; }
        public DbSet<ProjAvaliacao.Models.DisciplinaOfertada> DisciplinaOfertadas { get; set; }
        public DbSet<ProjAvaliacao.Models.Disciplina> Disciplinas { get; set; }
        public DbSet<ProjAvaliacao.Models.Professor> Professores { get; set; }
        public DbSet<ProjAvaliacao.Models.Aluno> Alunos { get; set; }
        
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Aluno>()
                .HasOne(a => a.Usuario)
                .WithMany()
                .HasForeignKey(a => a.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Professor>()
                .HasOne(a => a.Usuario)
                .WithMany()
                .HasForeignKey(a => a.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
