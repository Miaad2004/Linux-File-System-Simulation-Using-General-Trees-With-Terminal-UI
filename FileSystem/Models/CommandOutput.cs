namespace FileSystem.Models
{
    public class CommandOutput(bool HasFailed, Exception? exception = null)
    {
        public List<OutputTextDTO> OutputTexts { get; set; } = new List<OutputTextDTO>();
        public bool HasFailed { get; set; } = HasFailed;

        public Exception? exception = exception;
    }
}
