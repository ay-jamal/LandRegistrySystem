using LandRegistrySystem_Domain.Entities;
using QuestPDF.Infrastructure;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using System;
using System.Collections.Generic;

namespace LandRegistrySystem_Domain.Reports
{
    public class FarmReportDocument : IDocument
    {
        public Farm Farm { get; set; }
        public List<FarmDocument> Documents { get; set; }
        public OrganizationInfo Organization { get; set; }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(30);
                page.DefaultTextStyle(x => x.FontSize(12).FontFamily("Arial").DirectionFromRightToLeft());


                // Header
                page.Header().Column(col =>
                {
                    col.Item().Text(Organization?.Name ?? "اسم المؤسسة غير متوفر").Bold().FontSize(20).AlignCenter();
                    col.Item().Text("تقرير بيانات المزرعة").Bold().FontSize(16).AlignCenter();
                    col.Item().Text($"تاريخ التقرير: {DateTime.Now:yyyy/MM/dd HH:mm}")
                        .FontSize(10).AlignCenter().FontColor(Colors.Grey.Medium);
                });

                // Content
                page.Content().AlignMiddle().Column(column =>
                {
                    column.Spacing(5);

                    // Farm Info Table with reversed columns and alignment
                    column.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(2); // Value - left side
                            columns.RelativeColumn(1); // Label - right side
                        });

                        void AddRow(string label, string value)
                        {
                            table.Cell().Element(CellStyle).AlignRight().Text(value ?? "-");
                            table.Cell().Element(CellStyle).AlignRight().Text(label).Bold();
                        }

                        AddRow(":رقم المزرعة ", Farm.FarmNumber);
                        AddRow(":المساحة", $"{Farm.Area} م²");
                        AddRow(":اسم المشروع", Farm.Project?.Name);
                        AddRow(":اسم المالك", Farm.Owner?.FullName);
                        AddRow(":الحدود", $"شمال: {Farm.Boundaries?.North ?? "-"} | جنوب: {Farm.Boundaries?.South ?? "-"} | شرق: {Farm.Boundaries?.East ?? "-"} | غرب: {Farm.Boundaries?.West ?? "-"}");
                        AddRow(":تاريخ الإنشاء", Farm.CreatedAt.ToString("yyyy/MM/dd"));
                        AddRow(":آخر تعديل", Farm.UpdatedAt?.ToString("yyyy/MM/dd") ?? "-");
                        AddRow(":تم التعديل بواسطة", Farm.UpdatedByUserName ?? "-");

                        IContainer CellStyle(IContainer container) =>
                            container.PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Grey.Lighten2);
                    });

                    // Documents Table with reversed columns and alignment
                    column.Item().Text(":الملفات المرتبطة").Bold().AlignRight().FontSize(14);
                    column.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(3); // File Path - left side
                            columns.RelativeColumn(2); // File Name - middle
                            columns.ConstantColumn(40); // # - right side
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(CellHeaderStyle).AlignRight().Text("المسار");
                            header.Cell().Element(CellHeaderStyle).AlignRight().Text("اسم الملف");
                            header.Cell().Element(CellHeaderStyle).AlignRight().Text("#");
                        });

                        int i = 1;
                        foreach (var doc in Documents)
                        {
                            table.Cell().Element(CellStyle).AlignRight().Text(doc.FilePath);
                            table.Cell().Element(CellStyle).AlignRight().Text(doc.FileName);
                            table.Cell().Element(CellStyle).AlignRight().Text(i++.ToString());
                        }

                        IContainer CellHeaderStyle(IContainer container) =>
                            container.Padding(5).Background(Colors.Grey.Lighten3).BorderBottom(1);

                        IContainer CellStyle(IContainer container) =>
                            container.PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Grey.Lighten1);
                    });
                });

                // Footer
                page.Footer().AlignCenter().Text(x =>
                {
                    x.Span("تقرير نظام السجل العقاري - سرت ").FontSize(10);
                    x.Span($" | {DateTime.Now:yyyy/MM/dd HH:mm}");
                });
            });
        }
    }
}
