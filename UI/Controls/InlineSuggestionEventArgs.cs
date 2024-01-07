namespace UI.Controls
{
    public class InlineSuggestionEventArgs(string inputText, string suggestion) : EventArgs
    {
        public string InputText { get; } = inputText;
        public string Suggestion { get; } = suggestion;
    }
}
