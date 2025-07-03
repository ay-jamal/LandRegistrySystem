using LandRegistrySystem_Domain.Entities;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuestPDF.Fluent;
using QuestPDF.Helpers;

namespace LandRegistrySystem_Domain.Reports
{
    public class FarmReportDocument : IDocument
    {
        public Farm Farm { get; set; }
        public List<FarmDocument> Documents { get; set; }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(30);
                page.DefaultTextStyle(x => x.FontSize(12));

                page.Header().Text($"تقرير بيانات المزرعة").Bold().FontSize(18).AlignCenter();
                page.Content().Column(column =>
                {
                    column.Spacing(10);

                    column.Item().Text($"رقم المزرعة: {Farm.FarmNumber}");
                    column.Item().Text($"اسم المشروع: {Farm.Project?.Name ?? "غير محدد"}");
                    column.Item().Text($"المساحة: {Farm.Area}");
                    column.Item().Text($"اسم المالك: {Farm.Owner?.FullName ?? "غير محدد"}");
                    column.Item().Text($"الحدود:");
                    column.Item().Text(
                        $"شمال: {Farm.Boundaries?.North ?? "-"}  | جنوب: {Farm.Boundaries?.South ?? "-"}  | شرق: {Farm.Boundaries?.East ?? "-"}  | غرب: {Farm.Boundaries?.West ?? "-"}"
                    );

                    column.Item().Text("الملفات المرتبطة:").Bold();

                    column.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(40);
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(CellStyle).Text("#");
                            header.Cell().Element(CellStyle).Text("اسم الملف");
                            header.Cell().Element(CellStyle).Text("المسار");
                        });

                        int i = 1;
                        foreach (var doc in Documents)
                        {
                            table.Cell().Element(CellStyle).Text(i++);
                            table.Cell().Element(CellStyle).Text(doc.FileName);
                            table.Cell().Element(CellStyle).Text(doc.FilePath);
                        }

                        IContainer CellStyle(IContainer container) =>
                            container.BorderBottom(1).PaddingVertical(5).AlignMiddle();
                    });
                });
                page.Footer().AlignCenter().Text(x =>
                {
                    x.Span("تقرير نظام السجل العقاري - ").FontSize(10);
                    x.Span($"{DateTime.Now:yyyy/MM/dd HH:mm}");
                });
            });
        }
    }
}
