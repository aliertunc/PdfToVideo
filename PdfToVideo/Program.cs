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
                input = Path.GetFullPath(Path.Combine("pdfs", "sample.pdf"));
                Console.WriteLine($"No input provided. Using default: {input}");
            }

            try
            {
                PdfContent content = PdfParser.Extract(input); 
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
