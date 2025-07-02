using PdfToVideo;
using System;
using System.IO;

namespace PdfToVideo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string input = args.Length > 0 ? args[0] : null;
            if (string.IsNullOrWhiteSpace(input))
            {
                string defaultPath = "C:\\Users\\GamePc\\OneDrive\\Masaüstü\\AI Projesi\\NetProjects\\PdfToVideo\\pdfs\\hp.pdf";
                //Path.GetFullPath(Path.Combine("pdfs", "s.pdf"));

                input = new Uri(defaultPath).AbsoluteUri;
                Console.WriteLine($"No input provided. Using default: {input}");
            }

            Uri pdfUri = new Uri(input);

            try
            {
                PdfContent content = PdfParser.Extract(pdfUri);
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
