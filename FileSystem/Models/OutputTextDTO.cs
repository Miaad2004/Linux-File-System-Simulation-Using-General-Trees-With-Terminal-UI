namespace FileSystem.Models
{
    public class OutputTextDTO(string text, string hexColor = "#FFFF", bool isBold = false)
    {
        public string Text { get; set; } = text;
        public string HexColor { get; set; } = hexColor;
        public bool IsBold { get; set; } = isBold;
    }
}
