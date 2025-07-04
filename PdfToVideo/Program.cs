using PdfToVideo;
using System;
using System.IO;
using System.Threading.Tasks;

namespace PdfToVideo
{
    internal class Program
    {
        static async Task Main(string[] args)
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
                Console.WriteLine("Text extracted from PDF:\n");
                Console.WriteLine(content.Text);
                Console.WriteLine($"Total images extracted: {content.Images.Count}");

                string outputDir = Path.Combine(Path.GetDirectoryName(input) ?? string.Empty, "output");
                Directory.CreateDirectory(outputDir);
                string audioPath = Path.Combine(outputDir, "audio.wav");
                string videoPath = Path.Combine(outputDir, "video.mp4");

                await MediaConverter.TextToSpeechAsync(content.Text, audioPath);
                await MediaConverter.AudioToVideoAsync(audioPath, videoPath);

                Console.WriteLine($"Audio saved to {audioPath}");
                Console.WriteLine($"Video saved to {videoPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to parse PDF: {ex.Message}");
            }
        }
    }
}
