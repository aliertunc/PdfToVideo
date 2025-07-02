namespace PdfToVideo
{
    public class PdfContent
    {
        public string Text { get; set; }
        public List<byte[]> Images { get; set; }

        public PdfContent(string text, List<byte[]> images)
        {
            Text = text;
            Images = images;
        }
    }
}
