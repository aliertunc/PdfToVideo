using System.Text;
using System.Collections.Generic;
using System.IO;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Kernel.Pdf.Xobject;
using iText.Kernel.Pdf.Colorspace;
using iText.Kernel.Pdf.Filters;
using System.Net.Http;

namespace PdfToVideo
{
    public static class PdfParser
    {
        public static PdfContent Extract(string pdfPath)
        {
            var textBuilder = new StringBuilder();
            var images = new List<byte[]>();

            using var reader = new PdfReader(pdfPath);
            using var document = new PdfDocument(reader);

            for (int i = 1; i <= document.GetNumberOfPages(); i++)
            {
                var page = document.GetPage(i);

                // Extract text
                var strategy = new SimpleTextExtractionStrategy();
                textBuilder.AppendLine(PdfTextExtractor.GetTextFromPage(page, strategy));

                // Extract images
                var resources = page.GetResources();
                var xObjects = resources.GetResource(PdfName.XObject);
                if (xObjects != null)
                {
                    foreach (var name in xObjects.KeySet())
                    {
                        var obj = xObjects.GetAsStream(name);
                        var subtype = obj.GetAsName(PdfName.Subtype);
                        if (subtype != null && subtype.Equals(PdfName.Image))
                        {
                            var bytes = obj.GetBytes();
                            images.Add(bytes);
                        }
                    }
                }
            }

            return new PdfContent(textBuilder.ToString(), images);
        }

        public static PdfContent Extract(Uri pdfUri)
        {
            if (pdfUri.IsFile)
            {
                return Extract(pdfUri.LocalPath);
            }

            using var client = new HttpClient();
            byte[] data = client.GetByteArrayAsync(pdfUri).Result;
            string tmp = Path.GetTempFileName();
            File.WriteAllBytes(tmp, data);
            try
            {
                return Extract(tmp);
            }
            finally
            {
                try { File.Delete(tmp); } catch { }
            }
        }
    }
}
