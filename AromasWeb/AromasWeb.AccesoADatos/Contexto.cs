using AromasWeb.Abstracciones.ModeloUI;
using AromasWeb.AccesoADatos.Modelos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;

namespace AromasWeb.AccesoADatos
{
    public class Contexto : DbContext
    {
        private readonly IConfiguration _configuration;

        public Contexto()
        {
        }

        public Contexto(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Contexto(DbContextOptions<Contexto> options) : base(options)
        {
        }

        // DbSets de Insumos
        public DbSet<CategoriaInsumoAD> CategoriaInsumo { get; set; }
        public DbSet<InsumoAD> Insumo { get; set; }
        public DbSet<MovimientoInsumoAD> MovimientoInsumo { get; set; }

        // DbSets de Recetas
        public DbSet<CategoriaRecetaAD> CategoriaReceta { get; set; }
        public DbSet<RecetaAD> Receta { get; set; }
        public DbSet<RecetaInsumoAD> RecetaInsumo { get; set; }

        // DbSets de Promociones
        public DbSet<TipoPromocionAD> TipoPromocion { get; set; }
        public DbSet<PromocionAD> Promocion { get; set; }
        public DbSet<PromocionRecetaAD> PromocionReceta { get; set; }

        // DbSets de Clientes y Reservas
        public DbSet<ClienteAD> Cliente { get; set; }
        public DbSet<ReservaAD> Reserva { get; set; }

        // DbSets de Empleados y Roles
        public DbSet<RolAD> Rol { get; set; }
        public DbSet<EmpleadoAD> Empleado { get; set; }
        public DbSet<HistorialTarifaAD> HistorialTarifa { get; set; }

        public DbSet<AsistenciaAD> Asistencia { get; set; }
        public DbSet<SolicitudVacacionesAD> SolicitudVacaciones { get; set; }

        // DbSets de Planillas
        public DbSet<PlanillaAD> Planilla { get; set; }
        public DbSet<DetallePlanillaAD> DetallePlanilla { get; set; }

        // DbSets de Módulos y Permisos
        public DbSet<ModuloAD> Modulo { get; set; }
        public DbSet<PermisoAD> Permiso { get; set; }
        public DbSet<RolPermisoAD> RolPermiso { get; set; }

        // DbSets de Bitácora
        public DbSet<BitacoraAD> Bitacora { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                string connectionString = "Host=db.qtwtmksixuzgsjyqcqjn.supabase.co;Port=5432;Database=postgres;Username=postgres;Password=aromas.2025-;SSL Mode=Require;Trust Server Certificate=true;Timeout=30;Command Timeout=60;Pooling=true;Minimum Pool Size=0;Maximum Pool Size=100";

                try
                {
                    optionsBuilder.UseNpgsql(connectionString, npgsqlOptions =>
                    {
                        // Habilitar reintentos automáticos
                        npgsqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 3,
                            maxRetryDelay: TimeSpan.FromSeconds(10),
                            errorCodesToAdd: null);

                        // Timeout de comando
                        npgsqlOptions.CommandTimeout(60);
                    });

                    // Logging solo para errores
                    optionsBuilder.LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Error);

                    Console.WriteLine("[CONTEXTO] ✓ Configuración aplicada correctamente");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[CONTEXTO ERROR] ✗ {ex.Message}");
                    if (ex.InnerException != null)
                    {
                        Console.WriteLine($"[CONTEXTO ERROR INNER] ✗ {ex.InnerException.Message}");
                    }
                    throw;
                }
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configuración de CategoriaInsumo
            modelBuilder.Entity<CategoriaInsumoAD>(entity =>
            {
                entity.ToTable("categoriainsumo");
                entity.HasKey(e => e.IdCategoria);
                entity.Property(e => e.IdCategoria).HasColumnName("idcategoria").ValueGeneratedOnAdd();
                entity.Property(e => e.NombreCategoria).IsRequired().HasMaxLength(100).HasColumnName("nombrecategoria");
                entity.Property(e => e.Descripcion).HasMaxLength(500).HasColumnName("descripcion");
                entity.Property(e => e.Estado).IsRequired().HasColumnName("estado").HasDefaultValue(true);
            });

            // Configuración de Insumo
            modelBuilder.Entity<InsumoAD>(entity =>
            {
                entity.ToTable("insumo");
                entity.HasKey(e => e.IdInsumo);
                entity.Property(e => e.IdInsumo).HasColumnName("idinsumo").ValueGeneratedOnAdd();
                entity.Property(e => e.NombreInsumo).IsRequired().HasMaxLength(200).HasColumnName("nombreinsumo");
                entity.Property(e => e.UnidadMedida).IsRequired().HasMaxLength(50).HasColumnName("unidadmedida");
                entity.Property(e => e.IdCategoria).IsRequired().HasColumnName("idcategoria");
                entity.Property(e => e.CostoUnitario).IsRequired().HasColumnType("decimal(10,2)").HasColumnName("costounitario");
                entity.Property(e => e.CantidadDisponible).IsRequired().HasColumnType("decimal(10,2)").HasColumnName("cantidaddisponible").HasDefaultValue(0);
                entity.Property(e => e.StockMinimo).IsRequired().HasColumnType("decimal(10,2)").HasColumnName("stockminimo").HasDefaultValue(0);
                entity.Property(e => e.Estado).IsRequired().HasColumnName("estado").HasDefaultValue(true);
                entity.Property(e => e.FechaCreacion).IsRequired().HasColumnName("fechacreacion").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.FechaActualizacion).IsRequired().HasColumnName("fechaactualizacion").HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(e => e.CategoriaInsumo)
                    .WithMany()
                    .HasForeignKey(e => e.IdCategoria)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configuración de CategoriaReceta
            modelBuilder.Entity<CategoriaRecetaAD>(entity =>
            {
                entity.ToTable("categoriareceta");
                entity.HasKey(e => e.IdCategoriaReceta);
                entity.Property(e => e.IdCategoriaReceta).HasColumnName("idcategoriareceta").ValueGeneratedOnAdd();
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100).HasColumnName("nombre");
                entity.Property(e => e.Descripcion).HasMaxLength(500).HasColumnName("descripcion");
                entity.Property(e => e.Estado).IsRequired().HasColumnName("estado").HasDefaultValue(true);
            });

            // Configuración de Receta
            modelBuilder.Entity<RecetaAD>(entity =>
            {
                entity.ToTable("receta");
                entity.HasKey(e => e.IdReceta);
                entity.Property(e => e.IdReceta).HasColumnName("idreceta").ValueGeneratedOnAdd();
                entity.Property(e => e.IdCategoriaReceta).IsRequired().HasColumnName("idcategoriareceta");
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(200).HasColumnName("nombre");
                entity.Property(e => e.Descripcion).HasMaxLength(1000).HasColumnName("descripcion");
                entity.Property(e => e.CantidadPorciones).IsRequired().HasColumnName("cantidadporciones");
                entity.Property(e => e.PasosPreparacion).IsRequired().HasColumnName("pasospreparacion");
                entity.Property(e => e.PrecioVenta).HasColumnType("decimal(10,2)").HasColumnName("precioventa");
                entity.Property(e => e.CostoTotal).IsRequired().HasColumnType("decimal(10,2)").HasColumnName("costototal").HasDefaultValue(0);
                entity.Property(e => e.CostoPorcion).IsRequired().HasColumnType("decimal(10,2)").HasColumnName("costoporcion").HasDefaultValue(0);
                entity.Property(e => e.GananciaNeta).IsRequired().HasColumnType("decimal(10,2)").HasColumnName("ganancianeta").HasDefaultValue(0);
                entity.Property(e => e.PorcentajeMargen).IsRequired().HasColumnType("decimal(10,2)").HasColumnName("porcentajemargen").HasDefaultValue(0);
                entity.Property(e => e.Disponibilidad).IsRequired().HasColumnName("disponibilidad").HasDefaultValue(true);

                // Relación con CategoriaReceta
                entity.HasOne(e => e.CategoriaReceta)
                    .WithMany()
                    .HasForeignKey(e => e.IdCategoriaReceta)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configuración de RecetaInsumo
            modelBuilder.Entity<RecetaInsumoAD>(entity =>
            {
                entity.ToTable("recetainsumo");
                entity.HasKey(e => e.IdRecetaInsumo);
                entity.Property(e => e.IdRecetaInsumo).HasColumnName("idrecetainsumo").ValueGeneratedOnAdd();
                entity.Property(e => e.IdReceta).IsRequired().HasColumnName("idreceta");
                entity.Property(e => e.IdInsumo).IsRequired().HasColumnName("idinsumo");
                entity.Property(e => e.CantidadUtilizada).IsRequired().HasColumnType("decimal(10,2)").HasColumnName("cantidadutilizada");
                entity.Property(e => e.CostoUnitario).IsRequired().HasColumnType("decimal(10,2)").HasColumnName("costounitario");
                entity.Property(e => e.CostoTotalIngrediente).IsRequired().HasColumnType("decimal(10,2)").HasColumnName("costototalingrediente").HasDefaultValue(0);

                // Relaciones
                entity.HasOne(e => e.Receta)
                    .WithMany(r => r.RecetaInsumos)
                    .HasForeignKey(e => e.IdReceta)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Insumo)
                    .WithMany()
                    .HasForeignKey(e => e.IdInsumo)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configuración de TipoPromocion
            modelBuilder.Entity<TipoPromocionAD>(entity =>
            {
                entity.ToTable("tipopromocion");
                entity.HasKey(e => e.IdTipoPromocion);
                entity.Property(e => e.IdTipoPromocion).HasColumnName("idtipopromocion").ValueGeneratedOnAdd();
                entity.Property(e => e.NombreTipo).IsRequired().HasMaxLength(100).HasColumnName("nombre");
                entity.Property(e => e.Descripcion).HasMaxLength(300).HasColumnName("descripcion");
                entity.Property(e => e.Estado).IsRequired().HasColumnName("estado").HasDefaultValue(true);
            });

            // Configuración de Promocion
            modelBuilder.Entity<PromocionAD>(entity =>
            {
                entity.ToTable("promocion");
                entity.HasKey(e => e.IdPromocion);
                entity.Property(e => e.IdPromocion).HasColumnName("idpromocion").ValueGeneratedOnAdd();
                entity.Property(e => e.IdTipoPromocion).IsRequired().HasColumnName("idtipopromocion");
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(200).HasColumnName("nombre");
                entity.Property(e => e.Descripcion).HasMaxLength(500).HasColumnName("descripcion");
                entity.Property(e => e.PorcentajeDescuento).IsRequired().HasColumnType("decimal(5,2)").HasColumnName("porcentajedescuento");
                entity.Property(e => e.FechaInicio).IsRequired().HasColumnName("fechainicio");
                entity.Property(e => e.FechaFin).IsRequired().HasColumnName("fechafin");
                entity.Property(e => e.Estado).IsRequired().HasColumnName("estado").HasDefaultValue(true);

                // Relación con TipoPromocion
                entity.HasOne(e => e.TipoPromocion)
                    .WithMany()
                    .HasForeignKey(e => e.IdTipoPromocion)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configuración de PromocionReceta
            modelBuilder.Entity<PromocionRecetaAD>(entity =>
            {
                entity.ToTable("promocionreceta");
                entity.HasKey(e => e.IdPromocionReceta);
                entity.Property(e => e.IdPromocionReceta).HasColumnName("idpromocionreceta").ValueGeneratedOnAdd();
                entity.Property(e => e.IdPromocion).IsRequired().HasColumnName("idpromocion");
                entity.Property(e => e.IdReceta).IsRequired().HasColumnName("idreceta");
                entity.Property(e => e.PrecioPromocional).IsRequired().HasColumnType("decimal(18,2)").HasColumnName("preciopromocional");
                entity.Property(e => e.PorcentajeDescuento).IsRequired().HasColumnType("decimal(5,2)").HasColumnName("porcentajedescuento");
                entity.Property(e => e.Estado).IsRequired().HasColumnName("estado").HasDefaultValue(true);

                // Relaciones
                entity.HasOne(e => e.Promocion)
                    .WithMany()
                    .HasForeignKey(e => e.IdPromocion)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Receta)
                    .WithMany()
                    .HasForeignKey(e => e.IdReceta)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configuración de Cliente
            modelBuilder.Entity<ClienteAD>(entity =>
            {
                entity.ToTable("cliente");
                entity.HasKey(e => e.IdCliente);
                entity.Property(e => e.IdCliente).HasColumnName("idcliente").ValueGeneratedOnAdd();
                entity.Property(e => e.Identificacion).IsRequired().HasMaxLength(20).HasColumnName("identificacion");
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(200).HasColumnName("nombre");
                entity.Property(e => e.Apellidos).IsRequired().HasMaxLength(200).HasColumnName("apellidos");
                entity.Property(e => e.Correo).IsRequired().HasMaxLength(100).HasColumnName("correo");
                entity.Property(e => e.Telefono).IsRequired().HasMaxLength(20).HasColumnName("telefono");
                entity.Property(e => e.Contrasena).HasMaxLength(255).HasColumnName("contrasena");
                entity.Property(e => e.Estado).IsRequired().HasColumnName("estado").HasDefaultValue(true);
                entity.Property(e => e.FechaRegistro).IsRequired().HasColumnName("fecharegistro").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.UltimaReserva).HasColumnName("ultimareserva");

                // Índices
                entity.HasIndex(e => e.Identificacion).IsUnique();
                entity.HasIndex(e => e.Correo).IsUnique();
            });

            // Configuración de Reserva
            modelBuilder.Entity<ReservaAD>(entity =>
            {
                entity.ToTable("reserva");
                entity.HasKey(e => e.IdReserva);
                entity.Property(e => e.IdReserva).HasColumnName("idreserva").ValueGeneratedOnAdd();
                entity.Property(e => e.IdCliente).IsRequired().HasColumnName("idcliente");
                entity.Property(e => e.CantidadPersonas).IsRequired().HasColumnName("cantidadpersonas");
                entity.Property(e => e.Fecha).IsRequired().HasColumnName("fecha");
                entity.Property(e => e.Hora).IsRequired().HasColumnName("hora");
                entity.Property(e => e.Estado).IsRequired().HasMaxLength(20).HasColumnName("estado").HasDefaultValue("Pendiente");
                entity.Property(e => e.Observaciones).HasMaxLength(500).HasColumnName("observaciones");
                entity.Property(e => e.FechaCreacion).IsRequired().HasColumnName("fechacreacion").HasDefaultValueSql("CURRENT_TIMESTAMP");

                // Relación con Cliente
                entity.HasOne(e => e.Cliente)
                    .WithMany()
                    .HasForeignKey(e => e.IdCliente)
                    .OnDelete(DeleteBehavior.Restrict);

                // Índices
                entity.HasIndex(e => new { e.Fecha, e.Hora });
                entity.HasIndex(e => e.Estado);
            });

            // Configuración de MovimientoInsumo
            modelBuilder.Entity<MovimientoInsumoAD>(entity =>
            {
                entity.ToTable("movimientoinsumo");
                entity.HasKey(e => e.IdMovimiento);
                entity.Property(e => e.IdMovimiento).HasColumnName("idmovimiento").ValueGeneratedOnAdd();
                entity.Property(e => e.IdInsumo).IsRequired().HasColumnName("idinsumo");
                entity.Property(e => e.TipoMovimiento).IsRequired().HasMaxLength(1).HasColumnName("tipomovimiento");
                entity.Property(e => e.Cantidad).IsRequired().HasColumnType("decimal(18,2)").HasColumnName("cantidad");
                entity.Property(e => e.Motivo).IsRequired().HasMaxLength(500).HasColumnName("motivo");
                entity.Property(e => e.CostoUnitario).IsRequired().HasColumnType("decimal(18,2)").HasColumnName("costounitario");
                entity.Property(e => e.IdEmpleado).IsRequired().HasColumnName("idempleado");
                entity.Property(e => e.FechaMovimiento).IsRequired().HasColumnName("fechamovimiento").HasDefaultValueSql("CURRENT_TIMESTAMP");

                // Relación con Insumo
                entity.HasOne(e => e.Insumo)
                    .WithMany()
                    .HasForeignKey(e => e.IdInsumo)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Empleado)
                    .WithMany()
                    .HasForeignKey(e => e.IdEmpleado)
                    .OnDelete(DeleteBehavior.Restrict);

                // Índices
                entity.HasIndex(e => e.IdInsumo);
                entity.HasIndex(e => e.FechaMovimiento);
            });

            // Configuración de Rol
            modelBuilder.Entity<RolAD>(entity =>
            {
                entity.ToTable("rol");
                entity.HasKey(e => e.IdRol);
                entity.Property(e => e.IdRol).HasColumnName("idrol").ValueGeneratedOnAdd();
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100).HasColumnName("nombre");
                entity.Property(e => e.Descripcion).HasMaxLength(500).HasColumnName("descripcion");
                entity.Property(e => e.Estado).IsRequired().HasColumnName("estado").HasDefaultValue(true);

                // Índices
                entity.HasIndex(e => e.Nombre).IsUnique();
            });

            // Configuración de Empleado
            modelBuilder.Entity<EmpleadoAD>(entity =>
            {
                entity.ToTable("empleado");
                entity.HasKey(e => e.IdEmpleado);
                entity.Property(e => e.IdEmpleado).HasColumnName("idempleado").ValueGeneratedOnAdd();
                entity.Property(e => e.IdRol).IsRequired().HasColumnName("idrol");
                entity.Property(e => e.Identificacion).IsRequired().HasMaxLength(20).HasColumnName("identificacion");
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(200).HasColumnName("nombre");
                entity.Property(e => e.Apellidos).IsRequired().HasMaxLength(200).HasColumnName("apellidos");
                entity.Property(e => e.Correo).IsRequired().HasMaxLength(100).HasColumnName("correo");
                entity.Property(e => e.Telefono).IsRequired().HasMaxLength(20).HasColumnName("telefono");
                entity.Property(e => e.Cargo).IsRequired().HasMaxLength(100).HasColumnName("cargo");
                entity.Property(e => e.FechaContratacion).IsRequired().HasColumnName("fechacontratacion");
                entity.Property(e => e.Contrasena).HasMaxLength(255).HasColumnName("contrasena");
                entity.Property(e => e.Estado).IsRequired().HasColumnName("estado").HasDefaultValue(true);
                entity.Property(e => e.ContactoEmergencia).HasMaxLength(200).HasColumnName("contactoemergencia");
                entity.Property(e => e.Alergias).HasMaxLength(500).HasColumnName("alergias");
                entity.Property(e => e.Medicamentos).HasMaxLength(500).HasColumnName("medicamentos");

                // Relación con Rol
                entity.HasOne(e => e.Rol)
                     .WithMany()
                     .HasForeignKey(e => e.IdRol)
                     .OnDelete(DeleteBehavior.Restrict);

                // Índices
                entity.HasIndex(e => e.Identificacion).IsUnique();
                entity.HasIndex(e => e.Correo).IsUnique();
            });

            // Configuración de HistorialTarifa
            modelBuilder.Entity<HistorialTarifaAD>(entity =>
            {
                entity.ToTable("historialtarifa");
                entity.HasKey(e => e.IdHistorialTarifa);
                entity.Property(e => e.IdHistorialTarifa).HasColumnName("idhistorialtarifa").ValueGeneratedOnAdd();
                entity.Property(e => e.IdEmpleado).IsRequired().HasColumnName("idempleado");
                entity.Property(e => e.TarifaHora).IsRequired().HasColumnType("decimal(10,2)").HasColumnName("tarifahora");
                entity.Property(e => e.Motivo).IsRequired().HasMaxLength(500).HasColumnName("motivo");
                entity.Property(e => e.FechaInicio).IsRequired().HasColumnName("fechainicio");
                entity.Property(e => e.FechaFin).HasColumnName("fechafin");
                entity.Property(e => e.FechaRegistro).IsRequired().HasColumnName("fecharegistro").HasDefaultValueSql("CURRENT_TIMESTAMP");

                // Relación con Empleado
                entity.HasOne(e => e.Empleado)
                    .WithMany()
                    .HasForeignKey(e => e.IdEmpleado)
                    .OnDelete(DeleteBehavior.Restrict);

                // Índices
                entity.HasIndex(e => e.IdEmpleado);
                entity.HasIndex(e => new { e.FechaInicio, e.FechaFin });
            });

            // Configuración de Planilla
            modelBuilder.Entity<PlanillaAD>(entity =>
            {
                entity.ToTable("planilla");
                entity.HasKey(e => e.IdPlanilla);
                entity.Property(e => e.IdPlanilla).HasColumnName("idplanilla").ValueGeneratedOnAdd();
                entity.Property(e => e.IdEmpleado).IsRequired().HasColumnName("idempleado");
                entity.Property(e => e.PeriodoInicio).IsRequired().HasColumnName("periodoinicio");
                entity.Property(e => e.PeriodoFin).IsRequired().HasColumnName("periodofin");
                entity.Property(e => e.TarifaHora).IsRequired().HasColumnType("decimal(10,2)").HasColumnName("tarifahora");
                entity.Property(e => e.TotalHorasRegulares).HasColumnType("decimal(10,2)").HasColumnName("totalhorasregulares").HasDefaultValue(0);
                entity.Property(e => e.TotalHorasExtras).HasColumnType("decimal(10,2)").HasColumnName("totalhorasextras").HasDefaultValue(0);
                entity.Property(e => e.PagoHorasRegulares).HasColumnType("decimal(10,2)").HasColumnName("pagohorasregulares").HasDefaultValue(0);
                entity.Property(e => e.PagoHorasExtras).HasColumnType("decimal(10,2)").HasColumnName("pagohorasextras").HasDefaultValue(0);
                entity.Property(e => e.PagoBruto).HasColumnType("decimal(10,2)").HasColumnName("pagobruto").HasDefaultValue(0);
                entity.Property(e => e.DeduccionCCSS).HasColumnType("decimal(10,2)").HasColumnName("deduccionccss").HasDefaultValue(0);
                entity.Property(e => e.ImpuestoRenta).HasColumnType("decimal(10,2)").HasColumnName("impuestorenta").HasDefaultValue(0);
                entity.Property(e => e.OtrasDeducciones).HasColumnType("decimal(10,2)").HasColumnName("otrasdeducciones").HasDefaultValue(0);
                entity.Property(e => e.TotalDeducciones).HasColumnType("decimal(10,2)").HasColumnName("totaldeducciones").HasDefaultValue(0);
                entity.Property(e => e.PagoNeto).HasColumnType("decimal(10,2)").HasColumnName("pagoneto").HasDefaultValue(0);
                entity.Property(e => e.Estado).IsRequired().HasMaxLength(20).HasColumnName("estado").HasDefaultValue("Calculado");

                // Relación con Empleado
                entity.HasOne(e => e.Empleado)
                    .WithMany()
                    .HasForeignKey(e => e.IdEmpleado)
                    .OnDelete(DeleteBehavior.Restrict);

                // Índices
                entity.HasIndex(e => e.IdEmpleado);
                entity.HasIndex(e => e.Estado);
                entity.HasIndex(e => new { e.PeriodoInicio, e.PeriodoFin });
            });

            // Configuración de DetallePlanilla
            modelBuilder.Entity<DetallePlanillaAD>(entity =>
            {
                entity.ToTable("detalleplanilla");
                entity.HasKey(e => e.IdDetallePlanilla);
                entity.Property(e => e.IdDetallePlanilla).HasColumnName("iddetalleplanilla").ValueGeneratedOnAdd();
                entity.Property(e => e.IdPlanilla).IsRequired().HasColumnName("idplanilla");
                entity.Property(e => e.IdAsistencia).IsRequired().HasColumnName("idasistencia");
                entity.Property(e => e.Fecha).IsRequired().HasColumnName("fecha");
                entity.Property(e => e.HorasRegulares).HasColumnType("decimal(5,2)").HasColumnName("horasregulares").HasDefaultValue(0);
                entity.Property(e => e.HorasExtras).HasColumnType("decimal(5,2)").HasColumnName("horasextras").HasDefaultValue(0);
                entity.Property(e => e.Subtotal).HasColumnType("decimal(10,2)").HasColumnName("subtotal").HasDefaultValue(0);

                // Relación con Planilla
                entity.HasOne(e => e.Planilla)
                    .WithMany()
                    .HasForeignKey(e => e.IdPlanilla)
                    .OnDelete(DeleteBehavior.Cascade);

                // Relación con Asistencia
                entity.HasOne(e => e.Asistencia)
                    .WithMany()
                    .HasForeignKey(e => e.IdAsistencia)
                    .OnDelete(DeleteBehavior.Restrict);

                // Índices
                entity.HasIndex(e => e.IdPlanilla);
                entity.HasIndex(e => e.IdAsistencia);
                entity.HasIndex(e => e.Fecha);
            });

            // Configuración de Asistencia
            modelBuilder.Entity<AsistenciaAD>(entity =>
            {
                entity.ToTable("asistencia");
                entity.HasKey(e => e.IdAsistencia);
                entity.Property(e => e.IdAsistencia).HasColumnName("idasistencia").ValueGeneratedOnAdd();
                entity.Property(e => e.IdEmpleado).IsRequired().HasColumnName("idempleado");
                entity.Property(e => e.Fecha).IsRequired().HasColumnName("fecha");
                entity.Property(e => e.HoraEntrada).IsRequired().HasColumnName("horaentrada");
                entity.Property(e => e.HoraSalida).HasColumnName("horasalida");
                entity.Property(e => e.TiempoAlmuerzo).IsRequired().HasColumnName("tiempoalmuerzo").HasDefaultValue(0);
                entity.Property(e => e.HorasRegulares).HasColumnType("decimal(10,2)").HasColumnName("horasregulares").HasDefaultValue(0);
                entity.Property(e => e.HorasExtras).HasColumnType("decimal(10,2)").HasColumnName("horasextras").HasDefaultValue(0);
                entity.Property(e => e.HorasTotales).HasColumnType("decimal(10,2)").HasColumnName("horastotales").HasDefaultValue(0);

                // Relación con Empleado
                entity.HasOne(e => e.Empleado)
                    .WithMany()
                    .HasForeignKey(e => e.IdEmpleado)
                    .OnDelete(DeleteBehavior.Restrict);

                // Índices
                entity.HasIndex(e => e.IdEmpleado);
                entity.HasIndex(e => e.Fecha);
                entity.HasIndex(e => new { e.IdEmpleado, e.Fecha });
            });

            // Configuración de SolicitudVacaciones
            modelBuilder.Entity<SolicitudVacacionesAD>(entity =>
            {
                entity.ToTable("solicitudvacaciones");
                entity.HasKey(e => e.IdSolicitud);
                entity.Property(e => e.IdSolicitud).HasColumnName("idsolicitud").ValueGeneratedOnAdd();
                entity.Property(e => e.IdEmpleado).IsRequired().HasColumnName("idempleado");
                entity.Property(e => e.FechaSolicitud).IsRequired().HasColumnName("fechasolicitud");
                entity.Property(e => e.FechaInicio).IsRequired().HasColumnName("fechainicio");
                entity.Property(e => e.FechaFin).IsRequired().HasColumnName("fechafin");
                entity.Property(e => e.DiasSolicitados).IsRequired().HasColumnName("diassolicitados");
                entity.Property(e => e.Estado).IsRequired().HasMaxLength(50).HasColumnName("estado").HasDefaultValue("Pendiente");
                entity.Property(e => e.FechaRespuesta).HasColumnName("fecharespuesta");

                // Relación con Empleado
                entity.HasOne(e => e.Empleado)
                   .WithMany()
                   .HasForeignKey(e => e.IdEmpleado)
                   .OnDelete(DeleteBehavior.Restrict);

                // Índices
                entity.HasIndex(e => e.IdEmpleado);
                entity.HasIndex(e => e.Estado);
                entity.HasIndex(e => e.FechaSolicitud);
            });

            // Configuración de Modulo
            modelBuilder.Entity<ModuloAD>(entity =>
            {
                entity.ToTable("modulo");
                entity.HasKey(e => e.IdModulo);
                entity.Property(e => e.IdModulo).HasColumnName("idmodulo").ValueGeneratedOnAdd();
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100).HasColumnName("nombre");
                entity.Property(e => e.Descripcion).HasMaxLength(500).HasColumnName("descripcion");
                entity.Property(e => e.Estado).IsRequired().HasColumnName("estado").HasDefaultValue(true);

                // Índices
                entity.HasIndex(e => e.Nombre).IsUnique();
                entity.HasIndex(e => e.Estado);
            });

            // Configuración de Permiso
            modelBuilder.Entity<PermisoAD>(entity =>
            {
                entity.ToTable("permiso");
                entity.HasKey(e => e.IdPermiso);
                entity.Property(e => e.IdPermiso).HasColumnName("idpermiso").ValueGeneratedOnAdd();
                entity.Property(e => e.IdModulo).IsRequired().HasColumnName("idmodulo");
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100).HasColumnName("nombre");

                // Relación con Modulo
                entity.HasOne(e => e.Modulo)
                    .WithMany()
                    .HasForeignKey(e => e.IdModulo)
                    .OnDelete(DeleteBehavior.Restrict);

                // Índices
                entity.HasIndex(e => e.IdModulo);
                entity.HasIndex(e => new { e.IdModulo, e.Nombre }).IsUnique();
            });

            // Configuración de RolPermiso
            modelBuilder.Entity<RolPermisoAD>(entity =>
            {
                entity.ToTable("rolpermiso");
                entity.HasKey(e => e.IdRolPermiso);
                entity.Property(e => e.IdRolPermiso).HasColumnName("idrolpermiso").ValueGeneratedOnAdd();
                entity.Property(e => e.IdRol).IsRequired().HasColumnName("idrol");
                entity.Property(e => e.IdPermiso).IsRequired().HasColumnName("idpermiso");

                // Relación con Rol
                entity.HasOne(e => e.Rol)
                    .WithMany()
                    .HasForeignKey(e => e.IdRol)
                    .OnDelete(DeleteBehavior.Cascade);

                // Relación con Permiso
                entity.HasOne(e => e.Permiso)
                    .WithMany()
                    .HasForeignKey(e => e.IdPermiso)
                    .OnDelete(DeleteBehavior.Cascade);

                // Índices
                entity.HasIndex(e => new { e.IdRol, e.IdPermiso }).IsUnique();
                entity.HasIndex(e => e.IdRol);
                entity.HasIndex(e => e.IdPermiso);
            });

            // Configuración de Bitacora
            modelBuilder.Entity<BitacoraAD>(entity =>
            {
                entity.ToTable("bitacora");
                entity.HasKey(e => e.IdBitacora);
                entity.Property(e => e.IdBitacora).HasColumnName("idbitacora").ValueGeneratedOnAdd();
                entity.Property(e => e.IdEmpleado).IsRequired().HasColumnName("idempleado");
                entity.Property(e => e.IdModulo).IsRequired().HasColumnName("idmodulo");
                entity.Property(e => e.Accion).IsRequired().HasMaxLength(200).HasColumnName("accion");
                entity.Property(e => e.TablaAfectada).HasMaxLength(100).HasColumnName("tablaafectada");
                entity.Property(e => e.Descripcion).HasMaxLength(1000).HasColumnName("descripcion");
                entity.Property(e => e.DatosAnteriores).HasColumnName("datosanteriores");
                entity.Property(e => e.DatosNuevos).HasColumnName("datosnuevos");
                entity.Property(e => e.Fecha).IsRequired().HasColumnName("fecha").HasDefaultValueSql("CURRENT_TIMESTAMP");

                // Relación con Empleado
                entity.HasOne(e => e.Empleado)
                    .WithMany()
                    .HasForeignKey(e => e.IdEmpleado)
                    .OnDelete(DeleteBehavior.Restrict);

                // Relación con Módulo
                entity.HasOne(e => e.Modulo)
                    .WithMany()
                    .HasForeignKey(e => e.IdModulo)
                    .OnDelete(DeleteBehavior.Restrict);

                // Índices
                entity.HasIndex(e => e.IdEmpleado);
                entity.HasIndex(e => e.IdModulo);
                entity.HasIndex(e => e.Fecha);
                entity.HasIndex(e => e.Accion);
                entity.HasIndex(e => new { e.IdEmpleado, e.Fecha });
                entity.HasIndex(e => new { e.IdModulo, e.Fecha });
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}