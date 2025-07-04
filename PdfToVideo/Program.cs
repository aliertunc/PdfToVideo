using System.Globalization;
using System.Speech.Synthesis;
using System.Text;
using Xabe.FFmpeg;

namespace PdfToVideo
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

            string input = args.Length > 0 ? args[0] : Path.Combine("pdfs", "p1.pdf");
            Console.WriteLine($"Using input: {input}");

            string outputDir = Path.Combine(Path.GetDirectoryName(input) ?? string.Empty, "output");
            Directory.CreateDirectory(outputDir);

            string srtPath = Path.Combine(outputDir, "subtitles.srt");
            string audioPath = Path.Combine(outputDir, "audio.wav");
            string videoPath = Path.Combine(outputDir, "video.mp4");

            try
            {
                // 1. PDF metnini oku
                PdfContent content = PdfParser.Extract(input);

                // 2. Cümlelere ayır
                var sentences = content.Text
                    .Split(new[] { ".", "!", "?" }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Trim())
                    .Where(s => s.Length > 0)
                    .ToList();

                // 3. Her cümle için süreyi tahmin ederek SRT oluştur
                double currentTime = 0;
                int index = 1;
                StringBuilder srtBuilder = new StringBuilder();

                foreach (string sentence in sentences)
                {
                    double duration = EstimateSpeechDuration(sentence); // kelime sayısına göre süre tahmini
                    var start = TimeSpan.FromSeconds(currentTime);
                    var end = TimeSpan.FromSeconds(currentTime + duration);

                    srtBuilder.AppendLine(index.ToString());
                    srtBuilder.AppendLine($"{FormatTime(start)} --> {FormatTime(end)}");
                    srtBuilder.AppendLine(sentence);
                    srtBuilder.AppendLine();

                    currentTime += duration;
                    index++;
                }

                File.WriteAllText(srtPath, srtBuilder.ToString());

                // 4. Tüm metni tek bir .wav olarak üret
                var synth = new SpeechSynthesizer();
                synth.SetOutputToWaveFile(audioPath);
                synth.Speak(content.Text);
                synth.SetOutputToNull();
                synth.Rate = -2; // Hızı biraz yavaşlat, normalden hızlı konuşuyor

                // 5. FFmpeg ile altyazı + ses birleştir
                FFmpeg.SetExecutablesPath("ffmpeg");
                string srtPathFull = Path.GetFullPath(srtPath).Replace("\\", "/");
                string audioPathFull = Path.GetFullPath(audioPath).Replace("\\", "/");
                string videoPathFull = Path.GetFullPath(videoPath).Replace("\\", "/");

                // FFmpeg parametrelerinde tam yol + tırnak!
                var conversion = FFmpeg.Conversions.New()
                    .AddParameter($"-f lavfi -i color=c=black:s=1280x720:d={currentTime.ToString(CultureInfo.InvariantCulture)}", ParameterPosition.PreInput)
                    .AddParameter($"-i \"{audioPathFull}\"", ParameterPosition.PreInput)
                    .AddParameter($"-vf subtitles='{srtPathFull}'") // tek tırnak dene, çalışmazsa çift tırnak
                    .SetOutput($"\"{videoPathFull}\"");
                await conversion.Start();
                // Proje çıktısında ffmpeg klasörü var mı kontrol et


                string ffmpegBinPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ffmpeg", "bin");
                string ffmpegExePath = Path.Combine(ffmpegBinPath, "ffmpeg.exe");
                if (!File.Exists(ffmpegExePath))
                {
                    throw new FileNotFoundException($"FFmpeg bulunamadı: {ffmpegExePath}\nffmpeg.exe dosyasını bu klasöre koy: {ffmpegBinPath}");
                }
                FFmpeg.SetExecutablesPath(ffmpegBinPath);

                await conversion.Start();

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        static double GetWavDuration(string path)
        {
            using var reader = new NAudio.Wave.AudioFileReader(path);
            return reader.TotalTime.TotalSeconds;
        }

        static string FormatTime(TimeSpan time)
        {
            return time.ToString(@"hh\:mm\:ss\,fff").Replace(".", ",");
        }
        static double EstimateSpeechDuration(string sentence)
        {
            int wordCount = sentence.Split(' ').Length;
            return wordCount / 2.5; // ortalama 2.5 kelime/sn okuma hızı
        }

    }
}
