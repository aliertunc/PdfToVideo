using PdfToVideo;

namespace PdfToVideo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: PdfToVideo <pdf-file-path>");
                return;
            }

            string pdfPath = args[0];

            try
            {
                PdfContent content = PdfParser.Extract(pdfPath);
                Console.WriteLine("Text extracted from PDF:\n");
                Console.WriteLine(content.Text);
                Console.WriteLine($"Total images extracted: {content.Images.Count}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to parse PDF: {ex.Message}");
            }
        }
    }
}
