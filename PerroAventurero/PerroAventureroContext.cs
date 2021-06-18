using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace PerroAventurero
{
    public partial class PerroAventureroContext : DbContext
    {
        public PerroAventureroContext()
        {
        }

        public PerroAventureroContext(DbContextOptions<PerroAventureroContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Acompannante> Acompannantes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Data Source=PC_411764;Initial Catalog=PerroAventurero;Integrated Security=True;Connect Timeout=60;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Acompannante>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("ACOMPANNANTE");

                entity.HasIndex(e => e.CodigoReserva, "RESERVA_TIENE_ACOMPANNANTE_FK");

                entity.Property(e => e.Asistencia).HasColumnName("ASISTENCIA");

                entity.Property(e => e.CodigoReserva).HasColumnName("CODIGO_RESERVA");

                entity.Property(e => e.Edad).HasColumnName("EDAD");

                entity.Property(e => e.Genero)
                    .IsRequired()
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("GENERO")
                    .IsFixedLength(true);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
