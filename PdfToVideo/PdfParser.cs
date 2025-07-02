using System.Text;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Kernel.Pdf.Xobject;
using iText.Kernel.Pdf.Colorspace;
using iText.Kernel.Pdf.Filters;

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
    }
}
