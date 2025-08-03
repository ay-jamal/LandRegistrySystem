using LandRegistrySystem_Domain.Entities;
using QuestPDF.Infrastructure;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using System;
using System.Collections.Generic;
using SkiaSharp;
using ZXing;


namespace LandRegistrySystem_Domain.Reports
{
    public class FarmReportDocument : IDocument
    {
        public Farm Farm { get; set; }
        public List<FarmDocument> Documents { get; set; }
        public OrganizationInfo Organization { get; set; }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        [Obsolete]
        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(30);
                page.DefaultTextStyle(x => x.FontSize(12).FontFamily("Arial").DirectionFromRightToLeft());

                // ✅ Header يحتوي شعار المؤسسة + QR على اليسار
                page.Header().PaddingBottom(15).Grid(grid =>
                {
                    grid.Columns(3);

                    // ✅ العمود الأول (يسار) QR Code
                    grid.Item().AlignLeft().Column(col =>
                    {
                        if (Farm != null)
                        {
                            string qrContent = $"{Farm.Project?.City?.CityNumber}{Farm.Project?.ProjectNumber}{Farm.FarmNumber}";
                            var qrBytes = GenerateQrCode(qrContent);
                            col.Item().AlignLeft().Height(100).Width(100).Image(qrBytes);
                        }
                    });

                    // ✅ العمود الثاني (يمين) الشعار والنصوص
                    grid.Item().AlignCenter().Column(col =>
                    {
                        col.Spacing(10);
                        if (Organization?.Logo != null && Organization.Logo.Length > 0)
                        {
                            col.Item().AlignCenter().Height(100).Width(100).Image(Organization.Logo);
                        }
                        col.Item().Text(Organization?.Name ?? "اسم المؤسسة غير متوفر").Bold().FontSize(20).AlignCenter();
                        col.Item().Text("تقرير بيانات المزرعة").Bold().FontSize(16).AlignCenter();
                        col.Item().Text($"تاريخ التقرير: {DateTime.Now:yyyy/MM/dd HH:mm}")
                            .FontSize(10).AlignCenter().FontColor(Colors.Grey.Medium);
                    });
                });

                // ✅ Content
                page.Content().AlignTop().Column(column =>
                {
                    column.Spacing(5);

                    column.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(1);
                        });

                        void AddRow(string label, string value)
                        {
                            table.Cell().Element(CellStyle).AlignRight().Text(value ?? "-");
                            table.Cell().Element(CellStyle).AlignRight().Text(label).Bold();
                        }

                        AddRow(":رقم المزرعة ", Farm.FarmNumber);
                        AddRow(":المساحة", $"{Farm.Area} م²");
                        AddRow(":اسم المشروع", Farm.Project?.Name);
                        AddRow(":الحدود", $"شمال: {Farm.Boundaries?.North ?? "-"} | جنوب: {Farm.Boundaries?.South ?? "-"} | شرق: {Farm.Boundaries?.East ?? "-"} | غرب: {Farm.Boundaries?.West ?? "-"}");
                        AddRow(":اسم المالك", Farm.Owner?.FullName);
                        AddRow(": رقم هاتف المالك", Farm.Owner?.PhoneNumber);
                        AddRow(": الرقم الوطني", Farm.Owner?.NationalId);
                        AddRow(":تاريخ الإنشاء", Farm.CreatedAt.ToString("yyyy/MM/dd"));
                        AddRow(":آخر تعديل", Farm.UpdatedAt?.ToString("yyyy/MM/dd") ?? "-");
                        AddRow(":تم التعديل بواسطة", Farm.UpdatedByUserName ?? "-");

                        IContainer CellStyle(IContainer container) =>
                            container.PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Grey.Lighten2);
                    });

                    column.Item().Text(":الملفات المرتبطة").Bold().AlignRight().FontSize(14);
                    column.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(3);
                            columns.RelativeColumn(2);
                            columns.ConstantColumn(40);
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

                // ✅ Footer
                page.Footer().AlignCenter().Text(x =>
                {
                    x.Span("تقرير نظام السجل العقاري - سرت ").FontSize(10);
                    x.Span($" | {DateTime.Now:yyyy/MM/dd HH:mm}");
                });
            });
        }

        // ✅ دالة توليد QR كـ byte[]
        private byte[] GenerateQrCode(string content)
        {
            var writer = new ZXing.BarcodeWriter<SKBitmap>
            {
                Format = BarcodeFormat.CODE_128,
                Options = new ZXing.Common.EncodingOptions
                {
                    Width = 200,
                    Height = 100,
                    Margin = 2
                },
                Renderer = new ZXing.SkiaSharp.Rendering.SKBitmapRenderer()
            };

            using (var bitmap = writer.Write(content))
            using (var image = SKImage.FromBitmap(bitmap))
            using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
            {
                return data.ToArray();
            }
        }

    }
}
