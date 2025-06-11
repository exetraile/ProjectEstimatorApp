using System;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using ProjectEstimatorApp.Models;

namespace ProjectEstimatorApp.Helper
{
    public static class ExportHelper
    {
        public static void ExportToPdf(ProjectSummary summary, string filePath)
        {
            // Создаем директорию, если она не существует
            FileInfo fileInfo = new FileInfo(filePath);
            if (fileInfo.Directory != null && !fileInfo.Directory.Exists)
            {
                fileInfo.Directory.Create();
            }

            // Определяем шрифты
            BaseFont baseFont = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
            iTextSharp.text.Font titleFont = new iTextSharp.text.Font(baseFont, 20, iTextSharp.text.Font.BOLD);
            iTextSharp.text.Font headerFont = new iTextSharp.text.Font(baseFont, 14, iTextSharp.text.Font.BOLD);
            iTextSharp.text.Font textFont = new iTextSharp.text.Font(baseFont, 12, iTextSharp.text.Font.NORMAL);

            // Создаем документ
            using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
            using (iTextSharp.text.Document document = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4, 40, 40, 40, 40)) // margins
            using (PdfWriter writer = PdfWriter.GetInstance(document, fs))
            {
                document.Open();

                // Добавляем заголовок
                iTextSharp.text.Paragraph title = new iTextSharp.text.Paragraph("Project Summary", titleFont);
                title.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
                document.Add(title);

                document.Add(iTextSharp.text.Chunk.NEWLINE); // Add a blank line

                // Добавляем информацию о проекте
                document.Add(new iTextSharp.text.Paragraph($"Project Name: {summary.ProjectName}", headerFont));
                document.Add(new iTextSharp.text.Paragraph($"Total Works: {summary.TotalWorks}", textFont));
                document.Add(new iTextSharp.text.Paragraph($"Total Materials: {summary.TotalMaterials}", textFont));
                document.Add(new iTextSharp.text.Paragraph($"Overall Total: {summary.OverallTotal}", textFont));

                document.Add(iTextSharp.text.Chunk.NEWLINE);

                // Добавляем информацию о этажах
                if (summary.EstimateSummaries != null && summary.EstimateSummaries.Count > 0)
                {
                    document.Add(new iTextSharp.text.Paragraph("Floor Summaries", headerFont));
                    document.Add(iTextSharp.text.Chunk.NEWLINE);

                    // Создаем таблицу
                    PdfPTable table = new PdfPTable(4);
                    table.WidthPercentage = 100; // Table fills the page width
                    table.DefaultCell.Padding = 5;

                    // Добавляем заголовки таблицы
                    AddTableHeader(table, "Floor", baseFont);
                    AddTableHeader(table, "Works Total", baseFont);
                    AddTableHeader(table, "Materials Total", baseFont);
                    AddTableHeader(table, "Total", baseFont);

                    // Добавляем данные о этажах
                    foreach (var floor in summary.EstimateSummaries)
                    {
                        AddTableCell(table, floor.EstimateName ?? "", baseFont);
                        AddTableCell(table, floor.WorksTotal.ToString("N2"), baseFont);
                        AddTableCell(table, floor.MaterialsTotal.ToString("N2"), baseFont);
                        AddTableCell(table, floor.Total.ToString("N2"), baseFont);
                    }

                    document.Add(table);
                }

                document.Close();
            }
        }

        private static void AddTableHeader(PdfPTable table, string text, BaseFont baseFont)
        {
            PdfPCell cell = new PdfPCell(new iTextSharp.text.Phrase(text, new iTextSharp.text.Font(baseFont, 12, iTextSharp.text.Font.BOLD)));
            cell.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;
            table.AddCell(cell);
        }

        private static void AddTableCell(PdfPTable table, string text, BaseFont baseFont)
        {
            PdfPCell cell = new PdfPCell(new iTextSharp.text.Phrase(text, new iTextSharp.text.Font(baseFont, 10, iTextSharp.text.Font.NORMAL)));
            cell.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;
            table.AddCell(cell);
        }
    }
}
