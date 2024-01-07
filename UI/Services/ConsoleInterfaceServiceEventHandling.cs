using FileSystem.Models;
using System.Windows.Input;
using System.Windows.Threading;
using UI.Controls;
using WordPredictor;

namespace UI.Services
{
    public partial class ConsoleInterfaceService
    {
        private void InputControl_InlineSuggestionAccepted(object sender, InlineSuggestionEventArgs e)
        {
            CommandInputControl inputControl = (CommandInputControl)sender;
            inputControl.InputTextBox.Text += e.Suggestion;

            inputControl.Dispatcher.BeginInvoke(DispatcherPriority.Input,
                    new Action(delegate ()
                    {
                        inputControl.InputTextBox.Focus();         // Set Logical Focus
                        Keyboard.Focus(inputControl.InputTextBox); // Set Keyboard Focus
                    }));

            inputControl.InputTextBox.CaretIndex = inputControl.InputTextBox.Text.Length;

        }

        private async void InputControl_InlineSuggestionTriggred(object sender, InlineSuggestionEventArgs e)
        {
            CommandInputControl inputControl = (CommandInputControl)sender;
            string lastWord = GetLastWord(e.InputText);
            string prediction = await NextWordPredictor.Instance.Predict(lastWord, PredictionEngine.BST);

            if (!prediction.StartsWith(lastWord) || prediction.Length < lastWord.Length)
            {
                inputControl.InLineSuggestion = "";
                return;
            }

            string subtraction = prediction.Substring(lastWord.Length);
            inputControl.InLineSuggestion = subtraction;
        }

        public void InputControl_EnterPressed(object sender, KeyDownEventArgs e)
        {
            if (((CommandInputControl)sender).IsReadonly)
            {
                return;
            }

            ExecuteCommand(e.InputText);
        }

        private void Node_NodeDisposed(object sender, NodeEventArgs<File> e)
        {
            NextWordPredictor.Instance.RemoveContextWord(e.Data.Title);
        }

        private void Node_NodeCreated(object sender, NodeEventArgs<File> e)
        {
            NextWordPredictor.Instance.AddContextWord(e.Data.Title);
        }

        private static string GetLastWord(string text)
        {
            var words = text.Split(' ');
            return words[words.Length - 1].Trim();
        }
    }
}
