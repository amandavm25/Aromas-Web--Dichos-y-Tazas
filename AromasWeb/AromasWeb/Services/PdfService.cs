using AromasWeb.Abstracciones.ModeloUI;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AromasWeb.Services
{
    // Servicio de generación de PDFs con QuestPDF 2026.x
    public static class PdfService
    {
        // ─── Colores (mismos que el CSS de Aromas) ───
        private const string ColorVerde    = "#3E4A3A";
        private const string ColorOliva   = "#8F8E6A";
        private const string ColorOro     = "#B8A76A";
        private const string ColorCrema   = "#EFE7C8";
        private const string ColorTexto   = "#2B2B2B";
        private const string ColorGris    = "#95A5A6";
        private const string ColorSuccess = "#2F9E31";
        private const string ColorRojo    = "#E6210B";
        private const string ColorAmarillo = "#F39C12";
        private const string ColorBlanco  = "#FFFFFF";

        public static void Inicializar()
        {
            QuestPDF.Settings.License = LicenseType.Community;
        }

        // ═══════════════════════════════════════════════════════════
        // 1. HISTORIAL DE TARIFAS
        // ═══════════════════════════════════════════════════════════
        public static byte[] GenerarHistorialTarifas(Empleado empleado, List<HistorialTarifa> historial)
        {
            var tarifaActual = historial.FirstOrDefault(t => t.EstadoVigencia == "Vigente");
            var primeraTarifa = historial.OrderBy(t => t.FechaInicio).FirstOrDefault();
            var crecimiento = 0m;
            if (primeraTarifa != null && tarifaActual != null && primeraTarifa.TarifaHora > 0)
                crecimiento = ((tarifaActual.TarifaHora - primeraTarifa.TarifaHora) / primeraTarifa.TarifaHora) * 100;

            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.Letter);
                    page.Margin(1.5f, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontSize(10).FontColor(ColorTexto));

                    page.Header().Element(c => Encabezado(c,
                        "Historial de Tarifas",
                        $"{empleado.Nombre} {empleado.Apellidos}  •  {empleado.Cargo}",
                        DateTime.Now));

                    page.Content().PaddingVertical(10).Column(col =>
                    {
                        col.Item().Element(c => SeccionDatosEmpleado(c, empleado));
                        col.Item().Height(12);

                        col.Item().Row(row =>
                        {
                            TarjetaStat(row.RelativeItem(), "Tarifa Actual/h",
                                tarifaActual != null ? $"₡{tarifaActual.TarifaHora:N2}" : "Sin tarifa", ColorOliva);
                            row.ConstantItem(8);
                            TarjetaStat(row.RelativeItem(), "Salario Mensual Est.",
                                tarifaActual != null ? $"₡{tarifaActual.SalarioMensualEstimado:N0}" : "₡0", ColorVerde);
                            row.ConstantItem(8);
                            TarjetaStat(row.RelativeItem(), "Cambios de Tarifa",
                                historial.Count.ToString(), ColorOro);
                            row.ConstantItem(8);
                            TarjetaStat(row.RelativeItem(), "Crecimiento Salarial",
                                $"{crecimiento:N1}%", crecimiento >= 0 ? ColorSuccess : ColorRojo);
                        });

                        col.Item().Height(16);
                        col.Item().Text("Línea de Tiempo de Tarifas").Bold().FontSize(12).FontColor(ColorVerde);
                        col.Item().Height(8);
                        col.Item().Element(c => TablaTarifas(c, historial));
                    });

                    page.Footer().Element(Pie);
                });
            }).GeneratePdf();
        }

        // ═══════════════════════════════════════════════════════════
        // 2. PLANILLAS DEL EMPLEADO
        // ═══════════════════════════════════════════════════════════
        public static byte[] GenerarPlanillasEmpleado(Empleado empleado, List<Planilla> planillas)
        {
            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.Letter.Landscape());
                    page.Margin(1.5f, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontSize(9).FontColor(ColorTexto));

                    page.Header().Element(c => Encabezado(c,
                        "Historial de Planillas",
                        $"{empleado.Nombre} {empleado.Apellidos}  •  {empleado.Cargo}",
                        DateTime.Now));

                    page.Content().PaddingVertical(10).Column(col =>
                    {
                        col.Item().Element(c => SeccionDatosEmpleado(c, empleado));
                        col.Item().Height(12);

                        col.Item().Row(row =>
                        {
                            TarjetaStat(row.RelativeItem(), "Planillas Totales",
                                planillas.Count.ToString(), ColorVerde);
                            row.ConstantItem(8);
                            TarjetaStat(row.RelativeItem(), "Total Recibido",
                                $"₡{planillas.Sum(p => p.PagoNeto):N0}", ColorSuccess);
                            row.ConstantItem(8);
                            TarjetaStat(row.RelativeItem(), "Promedio Planilla",
                                planillas.Any() ? $"₡{planillas.Average(p => p.PagoNeto):N0}" : "₡0", ColorOliva);
                            row.ConstantItem(8);
                            TarjetaStat(row.RelativeItem(), "Total Horas",
                                $"{planillas.Sum(p => p.TotalHorasTrabajadas):N2}h", ColorOro);
                        });

                        col.Item().Height(16);
                        col.Item().Text("Detalle de Planillas").Bold().FontSize(12).FontColor(ColorVerde);
                        col.Item().Height(8);
                        col.Item().Element(c => TablaPlanillas(c, planillas));
                    });

                    page.Footer().Element(Pie);
                });
            }).GeneratePdf();
        }

        // ═══════════════════════════════════════════════════════════
        // 3. DETALLE DE PLANILLA (comprobante)
        // ═══════════════════════════════════════════════════════════
        public static byte[] GenerarDetallePlanilla(Planilla planilla, List<DetallePlanilla> detalles)
        {
            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.Letter);
                    page.Margin(1.5f, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontSize(10).FontColor(ColorTexto));

                    page.Header().Element(c => Encabezado(c,
                        "Comprobante de Planilla",
                        $"Período: {planilla.PeriodoFormateado}",
                        DateTime.Now));

                    page.Content().PaddingVertical(10).Column(col =>
                    {
                        // Info empleado + período
                        col.Item().Border(1).BorderColor(ColorCrema).Padding(10).Row(row =>
                        {
                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Text("DATOS DEL EMPLEADO").Bold().FontSize(9).FontColor(ColorOliva);
                                c.Item().Height(4);
                                FilaDato(c, "Nombre:", planilla.NombreEmpleado);
                                FilaDato(c, "Identificación:", planilla.IdentificacionEmpleado);
                                FilaDato(c, "Cargo:", planilla.CargoEmpleado);
                            });
                            row.ConstantItem(20);
                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Text("DATOS DE LA PLANILLA").Bold().FontSize(9).FontColor(ColorOliva);
                                c.Item().Height(4);
                                FilaDato(c, "Período:", planilla.PeriodoFormateado);
                                FilaDato(c, "Días del período:", planilla.DiasPeriodo.ToString());
                                FilaDato(c, "Tarifa por hora:", $"₡{planilla.TarifaHora:N2}");
                                FilaDato(c, "Estado:", planilla.Estado);
                            });
                        });

                        col.Item().Height(12);

                        col.Item().Row(row =>
                        {
                            TarjetaStat(row.RelativeItem(), "H. Regulares", $"{planilla.TotalHorasRegulares:N2}h", ColorVerde);
                            row.ConstantItem(8);
                            TarjetaStat(row.RelativeItem(), "H. Extras", $"{planilla.TotalHorasExtras:N2}h", ColorOro);
                            row.ConstantItem(8);
                            TarjetaStat(row.RelativeItem(), "Pago Bruto", $"₡{planilla.PagoBruto:N0}", ColorOliva);
                            row.ConstantItem(8);
                            TarjetaStat(row.RelativeItem(), "Pago Neto", $"₡{planilla.PagoNeto:N0}", ColorSuccess);
                        });

                        col.Item().Height(14);
                        col.Item().Text("Desglose Diario").Bold().FontSize(12).FontColor(ColorVerde);
                        col.Item().Height(6);
                        col.Item().Element(c => TablaDetalles(c, detalles));

                        col.Item().Height(14);

                        // Pagos y deducciones
                        col.Item().Row(row =>
                        {
                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Background(ColorVerde).Padding(8)
                                    .Text("PAGOS").Bold().FontSize(10).FontColor(ColorBlanco);
                                FilaResumen(c, "Pago Horas Regulares", $"₡{planilla.PagoHorasRegulares:N2}");
                                FilaResumen(c, "Pago Horas Extras (150%)", $"₡{planilla.PagoHorasExtras:N2}");
                                c.Item().Background(ColorCrema).Padding(8).Row(r =>
                                {
                                    r.RelativeItem().Text("Pago Bruto Total").Bold().FontColor(ColorVerde);
                                    r.AutoItem().Text($"₡{planilla.PagoBruto:N2}").Bold().FontColor(ColorVerde);
                                });
                            });
                            row.ConstantItem(10);
                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Background(ColorOro).Padding(8)
                                    .Text("DEDUCCIONES").Bold().FontSize(10).FontColor(ColorBlanco);
                                FilaResumen(c, "CCSS (10.67%)", $"₡{planilla.DeduccionCCSS:N2}");
                                FilaResumen(c, "Impuesto sobre la Renta", $"₡{planilla.ImpuestoRenta:N2}");
                                FilaResumen(c, "Otras Deducciones", $"₡{planilla.OtrasDeducciones:N2}");
                                c.Item().Background(ColorCrema).Padding(8).Row(r =>
                                {
                                    r.RelativeItem().Text("Total Deducciones").Bold().FontColor(ColorOro);
                                    r.AutoItem().Text($"₡{planilla.TotalDeducciones:N2}").Bold().FontColor(ColorOro);
                                });
                            });
                        });

                        col.Item().Height(10);

                        // Pago neto final
                        col.Item().Background(ColorVerde).Padding(14).Row(row =>
                        {
                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Text("PAGO NETO A RECIBIR").FontSize(11).Bold().FontColor(ColorCrema);
                                c.Item().Text($"₡{planilla.PagoNeto:N2}").FontSize(22).Bold().FontColor(ColorOro);
                            });
                            row.AutoItem().AlignMiddle()
                                .Text($"Deducciones: {planilla.PorcentajeDeducciones:N1}%")
                                .FontSize(9).FontColor(ColorCrema);
                        });
                    });

                    page.Footer().Element(Pie);
                });
            }).GeneratePdf();
        }

        // ─────────────────────────────────────────────────────────
        // COMPONENTES REUTILIZABLES
        // ─────────────────────────────────────────────────────────

        private static void Encabezado(IContainer container, string titulo,
            string subtitulo, DateTime fecha)
        {
            container.Column(col =>
            {
                col.Item().Row(row =>
                {
                    row.RelativeItem().Column(c =>
                    {
                        c.Item().Text("TSURÚ").Bold().FontSize(22).FontColor(ColorVerde);
                        c.Item().Text("Sistema de Gestión Empresarial").FontSize(8).FontColor(ColorGris);
                    });
                    row.RelativeItem().AlignRight().Column(c =>
                    {
                        c.Item().AlignRight().Text(titulo).Bold().FontSize(18).FontColor(ColorVerde);
                        c.Item().AlignRight().Text(subtitulo).FontSize(9).FontColor(ColorOliva);
                        c.Item().AlignRight().Text($"Generado: {fecha:dd/MM/yyyy HH:mm}").FontSize(8).FontColor(ColorGris);
                    });
                });
                col.Item().Height(4);
                col.Item().LineHorizontal(2).LineColor(ColorOliva);
                col.Item().Height(4);
                col.Item().LineHorizontal(0.5f).LineColor(ColorCrema);
                col.Item().Height(6);
            });
        }

        private static void Pie(IContainer container)
        {
            container.Column(col =>
            {
                col.Item().Height(4);
                col.Item().LineHorizontal(0.5f).LineColor(ColorCrema);
                col.Item().Height(4);
                col.Item().Row(row =>
                {
                    row.RelativeItem()
                        .Text("Aromas — Documento generado automáticamente por el sistema")
                        .FontSize(7).FontColor(ColorGris);
                    row.AutoItem().Text(text =>
                    {
                        text.Span("Página ").FontSize(7).FontColor(ColorGris);
                        text.CurrentPageNumber().FontSize(7).FontColor(ColorGris);
                        text.Span(" de ").FontSize(7).FontColor(ColorGris);
                        text.TotalPages().FontSize(7).FontColor(ColorGris);
                    });
                });
            });
        }

        private static void SeccionDatosEmpleado(IContainer container, Empleado emp)
        {
            container.Background(ColorCrema).Padding(10).Row(row =>
            {
                DatoInline(row, "Nombre:", $"{emp.Nombre} {emp.Apellidos}");
                DatoInline(row, "Identificación:", emp.Identificacion ?? "-");
                DatoInline(row, "Cargo:", emp.Cargo ?? "-");
                if (!string.IsNullOrEmpty(emp.FechaContratacionFormateada))
                    DatoInline(row, "Contratación:", emp.FechaContratacionFormateada);
                if (!string.IsNullOrEmpty(emp.AnosTrabajados))
                    DatoInline(row, "Antigüedad:", emp.AnosTrabajados);
            });
        }

        private static void DatoInline(RowDescriptor row, string label, string valor)
        {
            row.AutoItem().PaddingRight(16).Column(c =>
            {
                c.Item().Text(label).FontSize(7).FontColor(ColorGris);
                c.Item().Text(valor).Bold().FontSize(9).FontColor(ColorVerde);
            });
        }

        private static void TarjetaStat(IContainer container, string label, string valor, string color)
        {
            container.Border(1).BorderColor(color).Column(c =>
            {
                c.Item().Background(color).Padding(6).Text(label).FontSize(8).Bold().FontColor(ColorBlanco);
                c.Item().Padding(8).Text(valor).FontSize(14).Bold().FontColor(color);
            });
        }

        private static void FilaDato(ColumnDescriptor col, string label, string valor)
        {
            col.Item().Row(r =>
            {
                r.ConstantItem(110).Text(label).FontSize(9).FontColor(ColorGris);
                r.RelativeItem().Text(valor ?? "-").FontSize(9).Bold().FontColor(ColorTexto);
            });
            col.Item().Height(3);
        }

        private static void FilaResumen(ColumnDescriptor col, string label, string valor)
        {
            col.Item().BorderBottom(0.5f).BorderColor(ColorCrema).Padding(6).Row(r =>
            {
                r.RelativeItem().Text(label).FontSize(9).FontColor(ColorTexto);
                r.AutoItem().Text(valor).FontSize(9).Bold().FontColor(ColorTexto);
            });
        }

        // ─────────────────────────────────────────────────────────
        // TABLAS — API correcta para QuestPDF 2026.x
        // Header(handler) y Footer(handler) reciben un Action<>
        // ─────────────────────────────────────────────────────────

        private static void TablaTarifas(IContainer container, List<HistorialTarifa> historial)
        {
            container.Table(table =>
            {
                table.ColumnsDefinition(cols =>
                {
                    cols.ConstantColumn(30);   // #
                    cols.RelativeColumn(2);    // Tarifa/h
                    cols.RelativeColumn(2);    // Salario
                    cols.RelativeColumn(2);    // Inicio
                    cols.RelativeColumn(2);    // Fin
                    cols.RelativeColumn(1);    // Días
                    cols.RelativeColumn(1.5f); // Estado
                    cols.RelativeColumn(3);    // Motivo
                });

                // ── Encabezado ──
                string[] headers = { "#", "Tarifa/hora", "Salario Est.", "Inicio", "Fin", "Días", "Estado", "Motivo" };
                table.Header(header =>
                {
                    foreach (var h in headers)
                        header.Cell().Background(ColorVerde).Padding(6)
                              .Text(h).Bold().FontSize(8).FontColor(ColorBlanco);
                });

                // ── Filas ──
                int idx = 1;
                foreach (var t in historial.OrderByDescending(x => x.FechaInicio))
                {
                    string bg = idx % 2 == 0 ? ColorCrema : ColorBlanco;
                    string estadoColor = t.EstadoVigencia switch
                    {
                        "Vigente" => ColorSuccess,
                        "Vencida" => ColorGris,
                        "Futura"  => ColorAmarillo,
                        _         => ColorTexto
                    };

                    CeldaTabla(table, idx.ToString(), bg);
                    CeldaTabla(table, $"₡{t.TarifaHora:N2}", bg, bold: true, color: ColorVerde);
                    CeldaTabla(table, $"₡{t.SalarioMensualEstimado:N0}", bg);
                    CeldaTabla(table, t.FechaInicioFormateada, bg);
                    CeldaTabla(table, t.FechaFin.HasValue ? t.FechaFinFormateada : "Indefinido", bg);
                    CeldaTabla(table, t.DiasVigencia.ToString(), bg);
                    CeldaTabla(table, t.EstadoVigencia, bg, bold: true, color: estadoColor);
                    CeldaTabla(table, t.Motivo ?? "-", bg);

                    idx++;
                }
            });
        }

        private static void TablaPlanillas(IContainer container, List<Planilla> planillas)
        {
            container.Table(table =>
            {
                table.ColumnsDefinition(cols =>
                {
                    cols.RelativeColumn(3);
                    cols.RelativeColumn(1.5f);
                    cols.RelativeColumn(1.5f);
                    cols.RelativeColumn(2);
                    cols.RelativeColumn(2);
                    cols.RelativeColumn(2);
                    cols.RelativeColumn(1.5f);
                });

                string[] headers = { "Período", "H. Regulares", "H. Extras", "Pago Bruto", "Deducciones", "Pago Neto", "Estado" };
                table.Header(header =>
                {
                    foreach (var h in headers)
                        header.Cell().Background(ColorVerde).Padding(6)
                              .Text(h).Bold().FontSize(8).FontColor(ColorBlanco);
                });

                int idx = 1;
                foreach (var p in planillas)
                {
                    string bg = idx % 2 == 0 ? ColorCrema : ColorBlanco;
                    string estadoColor = p.Estado switch
                    {
                        "Pagado"    => ColorSuccess,
                        "Calculado" => ColorAmarillo,
                        "Anulado"   => ColorRojo,
                        _           => ColorTexto
                    };

                    CeldaTabla(table, p.PeriodoFormateado, bg, bold: true, color: ColorVerde);
                    CeldaTabla(table, $"{p.TotalHorasRegulares:N2}h", bg);
                    CeldaTabla(table, $"{p.TotalHorasExtras:N2}h", bg, color: ColorOro);
                    CeldaTabla(table, $"₡{p.PagoBruto:N0}", bg);
                    CeldaTabla(table, $"₡{p.TotalDeducciones:N0}", bg, color: ColorOro);
                    CeldaTabla(table, $"₡{p.PagoNeto:N0}", bg, bold: true, color: ColorSuccess);
                    CeldaTabla(table, p.Estado, bg, bold: true, color: estadoColor);

                    idx++;
                }

                // ── Footer con totales ──
                table.Footer(footer =>
                {
                    footer.Cell().Background(ColorVerde).Padding(6)
                          .Text("TOTALES").Bold().FontSize(8).FontColor(ColorBlanco);
                    footer.Cell().Background(ColorVerde).Padding(6)
                          .Text($"{planillas.Sum(p => p.TotalHorasRegulares):N2}h")
                          .Bold().FontSize(8).FontColor(ColorBlanco);
                    footer.Cell().Background(ColorVerde).Padding(6)
                          .Text($"{planillas.Sum(p => p.TotalHorasExtras):N2}h")
                          .Bold().FontSize(8).FontColor(ColorOro);
                    footer.Cell().Background(ColorVerde).Padding(6)
                          .Text($"₡{planillas.Sum(p => p.PagoBruto):N0}")
                          .Bold().FontSize(8).FontColor(ColorBlanco);
                    footer.Cell().Background(ColorVerde).Padding(6)
                          .Text($"₡{planillas.Sum(p => p.TotalDeducciones):N0}")
                          .Bold().FontSize(8).FontColor(ColorOro);
                    footer.Cell().Background(ColorVerde).Padding(6)
                          .Text($"₡{planillas.Sum(p => p.PagoNeto):N0}")
                          .Bold().FontSize(8).FontColor(ColorOro);
                    footer.Cell().Background(ColorVerde).Padding(6)
                          .Text(string.Empty).FontSize(8).FontColor(ColorBlanco);
                });
            });
        }

        private static void TablaDetalles(IContainer container, List<DetallePlanilla> detalles)
        {
            container.Table(table =>
            {
                table.ColumnsDefinition(cols =>
                {
                    cols.RelativeColumn(2);
                    cols.RelativeColumn(2);
                    cols.RelativeColumn(1.5f);
                    cols.RelativeColumn(1.5f);
                    cols.RelativeColumn(1.5f);
                    cols.RelativeColumn(1.5f);
                    cols.RelativeColumn(1.5f);
                    cols.RelativeColumn(1.5f);
                    cols.RelativeColumn(2);
                });

                string[] headers = { "Fecha", "Día", "Entrada", "Salida", "Almuerzo", "Total Hrs", "H. Reg.", "H. Extra", "Subtotal" };
                table.Header(header =>
                {
                    foreach (var h in headers)
                        header.Cell().Background(ColorVerde).Padding(5)
                              .Text(h).Bold().FontSize(8).FontColor(ColorBlanco);
                });

                int idx = 1;
                foreach (var d in detalles)
                {
                    string bg = d.EsFinDeSemana ? "#FFF9E6" : (idx % 2 == 0 ? ColorCrema : ColorBlanco);

                    CeldaTabla(table, d.FechaFormateada, bg, bold: true);
                    CeldaTabla(table, d.DiaSemanaCalculado, bg,
                        color: d.EsFinDeSemana ? ColorAmarillo : ColorTexto);
                    CeldaTabla(table, d.HoraEntradaFormateada, bg);
                    CeldaTabla(table, d.HoraSalidaFormateada, bg);
                    CeldaTabla(table, d.TiempoAlmuerzoFormateado, bg);
                    CeldaTabla(table, $"{d.TotalHorasDia:F2}h", bg, bold: true, color: ColorOliva);
                    CeldaTabla(table, $"{d.HorasRegulares:F2}h", bg, color: ColorVerde);
                    CeldaTabla(table, d.TieneHorasExtras ? $"{d.HorasExtras:F2}h" : "0.00h", bg,
                        color: d.TieneHorasExtras ? ColorOro : ColorGris,
                        bold: d.TieneHorasExtras);
                    CeldaTabla(table, $"₡{d.Subtotal:N2}", bg, bold: true, color: ColorOro);

                    idx++;
                }

                table.Footer(footer =>
                {
                    footer.Cell().ColumnSpan(5).Background(ColorVerde).Padding(5)
                          .AlignRight().Text("TOTALES:").Bold().FontSize(8).FontColor(ColorBlanco);
                    footer.Cell().Background(ColorVerde).Padding(5)
                          .Text($"{detalles.Sum(d => d.TotalHorasDia):F2}h")
                          .Bold().FontSize(8).FontColor(ColorBlanco);
                    footer.Cell().Background(ColorVerde).Padding(5)
                          .Text($"{detalles.Sum(d => d.HorasRegulares):F2}h")
                          .Bold().FontSize(8).FontColor(ColorBlanco);
                    footer.Cell().Background(ColorVerde).Padding(5)
                          .Text($"{detalles.Sum(d => d.HorasExtras):F2}h")
                          .Bold().FontSize(8).FontColor(ColorOro);
                    footer.Cell().Background(ColorVerde).Padding(5)
                          .Text($"₡{detalles.Sum(d => d.Subtotal):N2}")
                          .Bold().FontSize(8).FontColor(ColorOro);
                });
            });
        }

        // ── Helper: celda genérica ──
        private static void CeldaTabla(TableDescriptor table, string texto, string bg,
            bool bold = false, string color = null)
        {
            var cell = table.Cell()
                .Background(bg)
                .BorderBottom(0.5f).BorderColor(ColorCrema)
                .Padding(5);

            var txt = cell.Text(texto).FontSize(8).FontColor(color ?? ColorTexto);
            if (bold) txt.Bold();
        }
    }
}