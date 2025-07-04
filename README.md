# PdfToVideo

This sample project is a .NET console application that extracts text and images from a PDF file, converts the text to speech, and creates a simple video with that audio. The project targets **.NET 8** and uses the following NuGet packages:

- `itext7` for parsing PDF contents
- `System.Speech` for text-to-speech
- `Xabe.FFmpeg` for creating a video using FFmpeg

## Prerequisites

1. Install the **.NET 8 SDK** from <https://dotnet.microsoft.com/download>.
2. Ensure an internet connection is available on first build so NuGet can restore the above packages.
3. FFmpeg binaries will be downloaded automatically at runtime by `Xabe.FFmpeg`.

## Building and Running

From the repository root:

```bash
# restore dependencies and build
cd PdfToVideo
 dotnet build PdfToVideo.sln

# run with a specific PDF
 dotnet run --project PdfToVideo/PdfToVideo.csproj <path or url to pdf>
```

If no argument is provided, the program uses the example PDF located in `pdfs/sample.pdf`.

