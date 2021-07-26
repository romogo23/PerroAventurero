using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace PerroAventurero.Models
{
    public partial class PAContext : DbContext
    {
        public PAContext()
        {
        }

        public PAContext(DbContextOptions<PAContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Acompannante> Acompannantes { get; set; }
        public virtual DbSet<Afiliacion> Afiliacions { get; set; }
        public virtual DbSet<Cliente> Clientes { get; set; }
        public virtual DbSet<EmpresasAfiliada> EmpresasAfiliadas { get; set; }
        public virtual DbSet<Evento> Eventos { get; set; }
        public virtual DbSet<Reserva> Reservas { get; set; }
        public virtual DbSet<UsuarioAdministrador> UsuarioAdministradors { get; set; }
        public virtual DbSet<UsuarioComun> UsuarioComuns { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
//            if (!optionsBuilder.IsConfigured)
//            {
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
//                optionsBuilder.UseSqlServer("Data Source=PC_411764;Initial Catalog=PerroAventurero;Integrated Security=True;Connect Timeout=60;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
//            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Acompannante>(entity =>
            {
                //entity.HasNoKey();

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

                entity.HasOne(d => d.CodigoReservaNavigation)
                    .WithMany()
                    .HasForeignKey(d => d.CodigoReserva)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ACOMPANN_RESERVA_T_RESERVA");
            });

            modelBuilder.Entity<Afiliacion>(entity =>
            {
                entity.HasKey(e => e.Codigo)
                    .IsClustered(false);

                entity.ToTable("AFILIACION");

                entity.HasIndex(e => e.Cedula, "USUARIO_ADMI_VERIFICA_AFILIACION_FK");

                entity.HasIndex(e => e.CedulaCliente, "USUARIO_REALIZA_AFILIACION_FK");

                entity.Property(e => e.Codigo)
                    .ValueGeneratedNever()
                    .HasColumnName("CODIGO");

                entity.Property(e => e.Cedula)
                    .HasMaxLength(35)
                    .IsUnicode(false)
                    .HasColumnName("CEDULA");

                entity.Property(e => e.CedulaCliente)
                    .HasMaxLength(35)
                    .IsUnicode(false)
                    .HasColumnName("CEDULA_CLIENTE");

                entity.Property(e => e.ComprobantePago)
                    .HasColumnType("image")
                    .HasColumnName("COMPROBANTE_PAGO");

                entity.Property(e => e.EsAceptada).HasColumnName("ES_ACEPTADA");

                entity.Property(e => e.Fecha)
                    .HasColumnType("datetime")
                    .HasColumnName("FECHA");

                entity.HasOne(d => d.CedulaNavigation)
                    .WithMany(p => p.Afiliacions)
                    .HasForeignKey(d => d.Cedula)
                    .HasConstraintName("FK_AFILIACI_USUARIO_A_USUARIO_");

                entity.HasOne(d => d.CedulaClienteNavigation)
                    .WithMany(p => p.Afiliacions)
                    .HasForeignKey(d => d.CedulaCliente)
                    .HasConstraintName("FK_AFILIACI_USUARIO_R_USUARIO_");
            });

            modelBuilder.Entity<Cliente>(entity =>
            {
                entity.HasKey(e => e.CedulaCliente)
                    .IsClustered(false);

                entity.ToTable("CLIENTE");

                entity.Property(e => e.CedulaCliente)
                    .HasMaxLength(35)
                    .IsUnicode(false)
                    .HasColumnName("CEDULA_CLIENTE");

                entity.Property(e => e.Correo)
                    .HasMaxLength(320)
                    .IsUnicode(false)
                    .HasColumnName("CORREO");

                entity.Property(e => e.FechaNacimiento)
                    .HasColumnType("datetime")
                    .HasColumnName("FECHA_NACIMIENTO");

                entity.Property(e => e.Genero)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("GENERO")
                    .IsFixedLength(true);

                entity.Property(e => e.NombreCompleto)
                    .HasMaxLength(250)
                    .IsUnicode(false)
                    .HasColumnName("NOMBRE_COMPLETO");

                entity.Property(e => e.RecepcionAnuncios).HasColumnName("RECEPCION_ANUNCIOS");

                entity.Property(e => e.Telefono).HasColumnName("TELEFONO");
            });

            modelBuilder.Entity<EmpresasAfiliada>(entity =>
            {
                entity.HasKey(e => e.CodigoEmpresa)
                    .IsClustered(false);

                entity.ToTable("EMPRESAS_AFILIADAS");

                entity.HasIndex(e => e.Cedula, "USUARIO_ADMI_INGRESA_EMPRESA_FK");

                entity.Property(e => e.CodigoEmpresa).HasColumnName("CODIGO_EMPRESA");

                entity.Property(e => e.Categoria)
                    .IsRequired()
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("CATEGORIA")
                    .IsFixedLength(true);

                entity.Property(e => e.Cedula)
                    .HasMaxLength(35)
                    .IsUnicode(false)
                    .HasColumnName("CEDULA");

                entity.Property(e => e.Correo)
                    .HasMaxLength(320)
                    .IsUnicode(false)
                    .HasColumnName("CORREO");

                entity.Property(e => e.Logo)
                    .HasColumnType("image")
                    .HasColumnName("LOGO");

                entity.Property(e => e.NombreEmpresa)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("NOMBRE_EMPRESA");

                entity.Property(e => e.Telefono).HasColumnName("TELEFONO");

                entity.HasOne(d => d.CedulaNavigation)
                    .WithMany(p => p.EmpresasAfiliada)
                    .HasForeignKey(d => d.Cedula)
                    .HasConstraintName("FK_EMPRESAS_USUARIO_A_USUARIO_");
            });

            modelBuilder.Entity<Evento>(entity =>
            {
                entity.HasKey(e => e.CodigoEvento)
                    .IsClustered(false);

                entity.ToTable("EVENTO");

                entity.HasIndex(e => e.Cedula, "USUARIO_ADMI_CREA_EVENTO_FK");

                entity.Property(e => e.CodigoEvento).HasColumnName("CODIGO_EVENTO");

                entity.Property(e => e.CantidadAforo).HasColumnName("CANTIDAD_AFORO");

                entity.Property(e => e.CantidadGrupos).HasColumnName("CANTIDAD_GRUPOS");

                entity.Property(e => e.Cedula)
                    .HasMaxLength(35)
                    .IsUnicode(false)
                    .HasColumnName("CEDULA");

                entity.Property(e => e.Comentarios)
                    .HasMaxLength(250)
                    .IsUnicode(false)
                    .HasColumnName("COMENTARIOS");

                entity.Property(e => e.Direccion)
                    .IsRequired()
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("DIRECCION");

                entity.Property(e => e.EnvioAnuncios).HasColumnName("ENVIO_ANUNCIOS");

                entity.Property(e => e.Fecha)
                    .HasColumnType("datetime")
                    .HasColumnName("FECHA");

                entity.Property(e => e.HoraFinal)
                    .HasColumnType("datetime")
                    .HasColumnName("HORA_FINAL");

                entity.Property(e => e.HoraInicio)
                    .HasColumnType("datetime")
                    .HasColumnName("HORA_INICIO");

                entity.Property(e => e.Imagen).HasColumnType("image").HasColumnName("IMAGEN");

                entity.Property(e => e.Lugar)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("LUGAR");

                entity.Property(e => e.NombreEvento)
                    .IsRequired()
                    .HasMaxLength(80)
                    .IsUnicode(false)
                    .HasColumnName("NOMBRE_EVENTO");

                entity.Property(e => e.PrecioGeneral)
                    .HasColumnType("money")
                    .HasColumnName("PRECIO_GENERAL");

                entity.Property(e => e.PrecioNinno)
                    .HasColumnType("money")
                    .HasColumnName("PRECIO_NINNO");

                entity.HasOne(d => d.CedulaNavigation)
                    .WithMany(p => p.Eventos)
                    .HasForeignKey(d => d.Cedula)
                    .HasConstraintName("FK_EVENTO_USUARIO_A_USUARIO_");
            });

            modelBuilder.Entity<Reserva>(entity =>
            {
                entity.HasKey(e => e.CodigoReserva)
                    .IsClustered(false);

                entity.ToTable("RESERVA");

                entity.HasIndex(e => e.CodigoEvento, "EVENTO_TIENE_RESERVA_FK");

                entity.HasIndex(e => e.CedulaCliente, "RESERVA_PERTENECE_CLIENTE_FK");

                entity.HasIndex(e => e.Cedula, "USUARIO_ADMI_VERIFICA_RESERVA_FK");

                entity.Property(e => e.CodigoReserva).HasColumnName("CODIGO_RESERVA");

                entity.Property(e => e.Asistencia).HasColumnName("ASISTENCIA");

                entity.Property(e => e.Cedula)
                    .HasMaxLength(35)
                    .IsUnicode(false)
                    .HasColumnName("CEDULA");

                entity.Property(e => e.CedulaCliente)
                    .IsRequired()
                    .HasMaxLength(35)
                    .IsUnicode(false)
                    .HasColumnName("CEDULA_CLIENTE");

                entity.Property(e => e.CodigoEvento).HasColumnName("CODIGO_EVENTO");

                entity.Property(e => e.ComprobantePago)
                    .HasColumnType("image")
                    .HasColumnName("COMPROBANTE_PAGO");

                entity.Property(e => e.EntradasGenerales).HasColumnName("ENTRADAS_GENERALES");

                entity.Property(e => e.EntradasNinnos).HasColumnName("ENTRADAS_NINNOS");

                entity.Property(e => e.EsAceptada).HasColumnName("ES_ACEPTADA");

                entity.Property(e => e.FechaReserva)
                    .HasColumnType("datetime")
                    .HasColumnName("FECHA_RESERVA");

                entity.Property(e => e.Grupo).HasColumnName("GRUPO");

                entity.Property(e => e.HoraEntrada)
                    .HasColumnType("datetime")
                    .HasColumnName("HORA_ENTRADA");

                entity.Property(e => e.PrecioTotal)
                    .HasColumnType("money")
                    .HasColumnName("PRECIO_TOTAL");

                entity.HasOne(d => d.CedulaNavigation)
                    .WithMany(p => p.Reservas)
                    .HasForeignKey(d => d.Cedula)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RESERVA_USUARIO_A_USUARIO_");

                entity.HasOne(d => d.CedulaClienteNavigation)
                    .WithMany(p => p.Reservas)
                    .HasForeignKey(d => d.CedulaCliente)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RESERVA_RESERVA_P_CLIENTE");

                entity.HasOne(d => d.CodigoEventoNavigation)
                    .WithMany(p => p.Reservas)
                    .HasForeignKey(d => d.CodigoEvento)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RESERVA_EVENTO_TI_EVENTO");
            });

            modelBuilder.Entity<UsuarioAdministrador>(entity =>
            {
                entity.HasKey(e => e.Cedula)
                    .IsClustered(false);

                entity.ToTable("USUARIO_ADMINISTRADOR");

                entity.Property(e => e.Cedula)
                    .HasMaxLength(35)
                    .IsUnicode(false)
                    .HasColumnName("CEDULA");

                entity.Property(e => e.CodigoTemporal).HasColumnName("CODIGO_TEMPORAL");

                entity.Property(e => e.Contrasenna)
                    .IsRequired()
                    .HasMaxLength(11)
                    .IsUnicode(false)
                    .HasColumnName("CONTRASENNA");

                entity.Property(e => e.Correo)
                    .IsRequired()
                    .HasMaxLength(320)
                    .IsUnicode(false)
                    .HasColumnName("CORREO");

                entity.Property(e => e.Descripcion)
                    .HasMaxLength(250)
                    .IsUnicode(false)
                    .HasColumnName("DESCRIPCION");

                entity.Property(e => e.FechaCambioC)
                    .HasColumnType("datetime")
                    .HasColumnName("FECHA_CAMBIO_C");

                entity.Property(e => e.FechaNacimiento)
                    .HasColumnType("datetime")
                    .HasColumnName("FECHA_NACIMIENTO");

                entity.Property(e => e.Foto)
                    .HasColumnType("image")
                    .HasColumnName("FOTO");

                entity.Property(e => e.Genero)
                    .IsRequired()
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("GENERO")
                    .IsFixedLength(true);

                entity.Property(e => e.NombreCompleto)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false)
                    .HasColumnName("NOMBRE_COMPLETO");

                entity.Property(e => e.Telefono).HasColumnName("TELEFONO");
            });

            modelBuilder.Entity<UsuarioComun>(entity =>
            {
                entity.HasKey(e => e.CedulaCliente)
                    .IsClustered(false);

                entity.ToTable("USUARIO_COMUN");

                entity.Property(e => e.CedulaCliente)
                    .HasMaxLength(35)
                    .IsUnicode(false)
                    .HasColumnName("CEDULA_CLIENTE");

                entity.Property(e => e.CodigoTemporal).HasColumnName("CODIGO_TEMPORAL");

                entity.Property(e => e.Contrasenna)
                    .IsRequired()
                    .HasMaxLength(11)
                    .IsUnicode(false)
                    .HasColumnName("CONTRASENNA");

                entity.Property(e => e.Descripcion)
                    .HasMaxLength(250)
                    .IsUnicode(false)
                    .HasColumnName("DESCRIPCION");

                entity.Property(e => e.FechaCambioC)
                    .HasColumnType("datetime")
                    .HasColumnName("FECHA_CAMBIO_C");

                entity.Property(e => e.Foto)
                    .HasColumnType("image")
                    .HasColumnName("FOTO");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
