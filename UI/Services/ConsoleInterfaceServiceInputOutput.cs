using FileSystem.Models;
using FileSystem.Services;
using FileSystem.Services.Authentication;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using UI.Controls;

namespace UI.Services
{
    public partial class ConsoleInterfaceService
    {
        private readonly List<OutputTextDTO> _textBuffer = [];
        private static Node<File> CurrentDir => FileSystemService.Instance.CurrentDirectory;
        private static string CurrentDirString => FileSystemService.Instance.GetCurrentDirString();
        private static User? CurrentUser => SessionManager.CurrentSession?.User;


        public void BufferOutputText(OutputTextDTO textObj)
        {
            _textBuffer.Add(textObj);
        }

        public void BufferOutputText(string text)
        {
            OutputTextDTO textDTO = new(text);
            BufferOutputText(textDTO);
        }

        private void ClearBuffer()
        {
            var outputControl = new RichTextBox
            {
                Background = new SolidColorBrush(Colors.Transparent),
                Foreground = new SolidColorBrush(Colors.White),
                FontSize = 17,
                IsReadOnly = true,
                BorderThickness = new Thickness(0),
                Margin = new Thickness(0),
                Padding = new Thickness(0)
            };

            outputControl.Document.Blocks.Clear();

            var paragraph = new Paragraph
            {
                Margin = new Thickness(0) // Set margin to remove any extra spacing
            };

            foreach (var textObj in _textBuffer)
            {
                var run = new Run(textObj.Text)
                {
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(textObj.HexColor)),
                    FontWeight = textObj.IsBold ? FontWeights.Bold : FontWeights.Normal
                };

                paragraph.Inlines.Add(run);
            }

            outputControl.Document.Blocks.Add(paragraph);

            stackPanel.Children.Add(outputControl);

            _textBuffer.Clear();
        }

        private CommandInputControl AddInputControl(string username)
        {
            var inputControl = new CommandInputControl
            {
                Username = username,
                CurrentDirectory = CurrentDirString
            };

            inputControl.OnEnterPressed += InputControl_EnterPressed;
            inputControl.InlineSuggestionTriggred += InputControl_InlineSuggestionTriggred;
            inputControl.InlineSuggestionAccepted += InputControl_InlineSuggestionAccepted;
            stackPanel.Children.Add(inputControl);

            ScrollDown();

            return inputControl;
        }
        private CommandInputControl AddInputControl()
        {
            if (CurrentUser == null)
                return AddInputControl("Authenticater");

            else
                return AddInputControl(CurrentUser.Username);

        }

        private void DisablePreviousInputs()
        {
            Keyboard.ClearFocus();

            foreach (var control in stackPanel.Children)
            {
                if (control is CommandInputControl)
                {
                    ((CommandInputControl)control).IsReadonly = true;
                    ((CommandInputControl)control).InLineSuggestion = "";
                }
            }
        }

        public void ClearConsole()
        {
            stackPanel.Children.Clear();
            AddInputControl();
        }

        private void ScrollDown()
        {
            mainView.scrollViewer.ScrollToBottom();
        }
    }
}
