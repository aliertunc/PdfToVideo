using System;
using System.IO;
using System.Speech.Synthesis;
using System.Threading.Tasks;
using Xabe.FFmpeg;
using Xabe.FFmpeg.Downloader;

namespace PdfToVideo
{
    public static class MediaConverter
    {
        public static Task<string> TextToSpeechAsync(string text, string outputPath)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);
            using var synth = new SpeechSynthesizer();
            synth.SetOutputToWaveFile(outputPath);
            synth.Speak(text);
            return Task.FromResult(outputPath);
        }

        public static async Task<string> AudioToVideoAsync(string audioPath, string outputPath)
        {
            await FFmpegDownloader.GetLatestVersion(FFmpegVersion.Official);
            var info = await FFmpeg.GetMediaInfo(audioPath);
            string colorFilter = $"color=c=black:s=1280x720:d={info.Duration.TotalSeconds}";
            var conversion = FFmpeg.Conversions.New()
                .AddParameter("-f lavfi", ParameterPosition.PreInput)
                .AddParameter($"-i {colorFilter}")
                .AddParameter($"-i \"{audioPath}\"")
                .AddParameter("-shortest")
                .SetOutput(outputPath)
                .SetOverwriteOutput(true);
            await conversion.Start();
            return outputPath;
        }
    }
}
