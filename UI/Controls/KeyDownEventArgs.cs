namespace UI.Controls
{
    public class KeyDownEventArgs(string inputText) : EventArgs
    {
        public string InputText { get; } = inputText;
    }
}
