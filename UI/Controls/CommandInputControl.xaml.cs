using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace UI.Controls
{
    /// <summary>
    /// Interaction logic for CommandInputControl.xaml
    /// </summary>
    public partial class CommandInputControl : UserControl
    {
        public event EventHandler<KeyDownEventArgs> OnEnterPressed;
        public event EventHandler<InlineSuggestionEventArgs> InlineSuggestionTriggred;
        public event EventHandler<InlineSuggestionEventArgs> InlineSuggestionAccepted;



        public static readonly DependencyProperty IsReadonlyProerty = DependencyProperty.Register("IsReadonly", typeof(bool), typeof(CommandInputControl));
        public bool IsReadonly
        {
            get { return (bool)GetValue(IsReadonlyProerty); }
            set { SetValue(IsReadonlyProerty, value); }
        }

        public static readonly DependencyProperty UsernameProperty = DependencyProperty.Register("Username", typeof(string), typeof(CommandInputControl));
        public string Username
        {
            get { return (string)GetValue(UsernameProperty); }
            set { SetValue(UsernameProperty, value); }
        }

        public static readonly DependencyProperty InLineSuggestionProperty = DependencyProperty.Register("InLineSuggestion", typeof(string), typeof(CommandInputControl));
        public string InLineSuggestion
        {
            get { return (string)GetValue(InLineSuggestionProperty); }
            set { SetValue(InLineSuggestionProperty, value); }
        }

        public static readonly DependencyProperty CurrentDirectoryProperty = DependencyProperty.Register("CurrentDirectory", typeof(string), typeof(CommandInputControl));
        public string CurrentDirectory
        {
            get { return (string)GetValue(CurrentDirectoryProperty); }
            set { SetValue(CurrentDirectoryProperty, value); }
        }

        public CommandInputControl()
        {
            InitializeComponent();

            IsReadonly = false;
            InputTextBox.Focus();
        }

        public string GetInputText()
        {
            return InputTextBox.Text;
        }

        private void InputTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && !this.IsReadonly)
            {
                OnEnterPressed?.Invoke(this, new KeyDownEventArgs(InputTextBox.Text));
            }

            else if (e.Key == Key.Tab && !this.IsReadonly)
            {
                InlineSuggestionAccepted?.Invoke(this, new InlineSuggestionEventArgs(InputTextBox.Text, InLineSuggestion));
            }
        }

        private void InputTextBox_Loaded(object sender, RoutedEventArgs e)
        {
            Keyboard.Focus((TextBox)sender);
            InputTextBox.CaretIndex = InputTextBox.Text.Length;
        }

        private void InputTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!this.IsReadonly)
            {
                InlineSuggestionTriggred?.Invoke(this, new InlineSuggestionEventArgs(InputTextBox.Text, InLineSuggestion));
            }
        }
    }
}
