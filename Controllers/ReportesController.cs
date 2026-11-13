using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ClosedXML.Excel;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using TechStore.Data;
using TechStore.Models;
using TechStore.Services;

namespace TechStore.Controllers;

[Authorize]
public class ReportesController : Controller
{
    private readonly TechStoreDbContext _context;
    private readonly ILogger<ReportesController> _logger;
    private readonly IRegistroEventoService _registro;

    public ReportesController(TechStoreDbContext context, ILogger<ReportesController> logger,
        IRegistroEventoService registro)
    {
        _context = context;
        _logger = logger;
        _registro = registro;
    }

    public IActionResult Index()
    {
        var categorias = _context.Categorias.OrderBy(c => c.Nombre).ToList();
        ViewBag.Categorias = categorias;
        return View();
    }

    public async Task<IActionResult> GenerarPDF(int? categoriaId)
    {
        var productos = await ObtenerProductos(categoriaId);
        var filtro = categoriaId.HasValue
            ? (await _context.Categorias.FindAsync(categoriaId.Value))?.Nombre ?? "Todos"
            : "Todos";

        var documento = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.Letter);
                page.Margin(40);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Header().Element(header =>
                {
                    header.Column(col =>
                    {
                        col.Item().Text("TechStore - Reporte de Productos")
                            .FontSize(18).Bold().FontColor(Colors.Blue.Darken3);
                        col.Item().Row(row =>
                        {
                            row.RelativeItem().Text($"Categoría: {filtro}")
                                .FontSize(9).FontColor(Colors.Grey.Darken1);
                            row.RelativeItem().AlignRight()
                                .Text($"Fecha: {DateTime.Now:dd/MM/yyyy HH:mm}")
                                .FontSize(9).FontColor(Colors.Grey.Darken1);
                        });
                        col.Item().PaddingTop(5).LineHorizontal(1).LineColor(Colors.Blue.Darken3);
                    });
                });

                page.Content().PaddingTop(10).Element(content =>
                {
                    content.Column(col =>
                    {
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(30);
                                columns.RelativeColumn(3);
                                columns.RelativeColumn(1.5f);
                                columns.ConstantColumn(60);
                                columns.ConstantColumn(45);
                                columns.ConstantColumn(55);
                            });

                            table.Header(h =>
                            {
                                var estilo = TextStyle.Default.FontSize(9).Bold().FontColor(Colors.White);
                                h.Cell().Background(Colors.Blue.Darken3).Padding(5).Text("ID").Style(estilo);
                                h.Cell().Background(Colors.Blue.Darken3).Padding(5).Text("Nombre").Style(estilo);
                                h.Cell().Background(Colors.Blue.Darken3).Padding(5).Text("Categoría").Style(estilo);
                                h.Cell().Background(Colors.Blue.Darken3).Padding(5).AlignRight().Text("Precio").Style(estilo);
                                h.Cell().Background(Colors.Blue.Darken3).Padding(5).AlignCenter().Text("Stock").Style(estilo);
                                h.Cell().Background(Colors.Blue.Darken3).Padding(5).AlignCenter().Text("Estado").Style(estilo);
                            });

                            var estiloFila = TextStyle.Default.FontSize(9);
                            var impar = true;

                            foreach (var p in productos)
                            {
                                var bg = impar ? Colors.White : Colors.Grey.Lighten4;
                                table.Cell().Background(bg).Padding(4).Text(p.Id.ToString()).Style(estiloFila);
                                table.Cell().Background(bg).Padding(4).Text(p.Nombre).Style(estiloFila);
                                table.Cell().Background(bg).Padding(4).Text(p.Categoria?.Nombre ?? "-").Style(estiloFila);
                                table.Cell().Background(bg).Padding(4).AlignRight().Text($"${p.Precio:N2}").Style(estiloFila);
                                table.Cell().Background(bg).Padding(4).AlignCenter().Text(p.Stock.ToString()).Style(estiloFila);
                                table.Cell().Background(bg).Padding(4).AlignCenter().Text(p.Estado ? "Activo" : "Inactivo").Style(estiloFila);
                                impar = !impar;
                            }
                        });

                        col.Item().PaddingTop(15).Row(row =>
                        {
                            row.RelativeItem().Background(Colors.Grey.Lighten3).Padding(8).Text(text =>
                            {
                                text.Span("Total productos: ").SemiBold();
                                text.Span(productos.Count.ToString());
                                text.Span("  |  Valor inventario: ").SemiBold();
                                text.Span($"${productos.Sum(p => p.Precio * p.Stock):N2}");
                            });
                        });
                    });
                });

                page.Footer().AlignCenter().Text(text =>
                {
                    text.Span("Página ").FontSize(8);
                    text.CurrentPageNumber().FontSize(8);
                    text.Span(" de ").FontSize(8);
                    text.TotalPages().FontSize(8);
                });
            });
        });

        _logger.LogInformation("Reporte PDF generado. Categoría: {Filtro}. Productos: {Total}.", filtro, productos.Count);
        await _registro.RegistrarAsync("Info", "Reportes/GenerarPDF",
            $"PDF generado ({productos.Count} productos, categoría: {filtro}).", User.Identity?.Name);

        var pdf = documento.GeneratePdf();
        return File(pdf, "application/pdf", "ReporteProductos.pdf");
    }

    public async Task<IActionResult> ExportarExcel(int? categoriaId)
    {
        var productos = await ObtenerProductos(categoriaId);
        var filtro = categoriaId.HasValue
            ? (await _context.Categorias.FindAsync(categoriaId.Value))?.Nombre ?? "Todos"
            : "Todos";

        using var workbook = new XLWorkbook();
        var hoja = workbook.Worksheets.Add("Productos");

        hoja.Cell("A1").Value = "TechStore - Reporte de Productos";
        hoja.Range("A1:F1").Merge().Style
            .Font.SetBold(true).Font.SetFontSize(14)
            .Font.SetFontColor(XLColor.White)
            .Fill.SetBackgroundColor(XLColor.DarkBlue)
            .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

        hoja.Cell("A2").Value = $"Categoría: {filtro}";
        hoja.Cell("D2").Value = $"Fecha: {DateTime.Now:dd/MM/yyyy HH:mm}";
        hoja.Range("A2:F2").Style.Font.SetFontSize(9).Font.SetItalic(true);

        string[] encabezados = { "ID", "Nombre", "Categoría", "Precio", "Stock", "Estado" };
        for (int i = 0; i < encabezados.Length; i++)
        {
            var celda = hoja.Cell(4, i + 1);
            celda.Value = encabezados[i];
            celda.Style.Font.SetBold(true).Font.SetFontColor(XLColor.White)
                .Fill.SetBackgroundColor(XLColor.DarkBlue)
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                .Border.SetOutsideBorder(XLBorderStyleValues.Thin);
        }

        var fila = 5;
        foreach (var p in productos)
        {
            hoja.Cell(fila, 1).Value = p.Id;
            hoja.Cell(fila, 2).Value = p.Nombre;
            hoja.Cell(fila, 3).Value = p.Categoria?.Nombre ?? "-";
            hoja.Cell(fila, 4).Value = p.Precio;
            hoja.Cell(fila, 5).Value = p.Stock;
            hoja.Cell(fila, 6).Value = p.Estado ? "Activo" : "Inactivo";

            var rango = hoja.Range(fila, 1, fila, 6);
            rango.Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                .Border.SetInsideBorder(XLBorderStyleValues.Thin);

            if (fila % 2 == 0)
                rango.Style.Fill.SetBackgroundColor(XLColor.LightGray);

            hoja.Cell(fila, 4).Style.NumberFormat.Format = "$#,##0.00";
            hoja.Cell(fila, 5).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            hoja.Cell(fila, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            fila++;
        }

        fila++;
        hoja.Cell(fila, 1).Value = "Total productos:";
        hoja.Cell(fila, 1).Style.Font.SetBold(true);
        hoja.Cell(fila, 2).Value = productos.Count;
        hoja.Cell(fila, 3).Value = "Valor inventario:";
        hoja.Cell(fila, 3).Style.Font.SetBold(true);
        hoja.Cell(fila, 4).Value = productos.Sum(p => p.Precio * p.Stock);
        hoja.Cell(fila, 4).Style.NumberFormat.Format = "$#,##0.00";

        hoja.Columns().AdjustToContents();

        _logger.LogInformation("Reporte Excel generado. Categoría: {Filtro}. Productos: {Total}.", filtro, productos.Count);
        await _registro.RegistrarAsync("Info", "Reportes/ExportarExcel",
            $"Excel generado ({productos.Count} productos, categoría: {filtro}).", User.Identity?.Name);

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return File(stream.ToArray(),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "ReporteProductos.xlsx");
    }

    private async Task<List<Producto>> ObtenerProductos(int? categoriaId)
    {
        var consulta = _context.Productos.Include(p => p.Categoria).AsQueryable();

        if (categoriaId.HasValue)
            consulta = consulta.Where(p => p.CategoriaId == categoriaId.Value);

        return await consulta.OrderBy(p => p.Nombre).ToListAsync();
    }
}
