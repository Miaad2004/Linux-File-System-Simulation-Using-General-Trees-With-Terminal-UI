using FileSystem.Models;
using FileSystem.Services;
using FileSystem.Services.Authentication;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Controls;
using UI.Views;
using WordPredictor;

namespace UI.Services
{


    public partial class ConsoleInterfaceService
    {
        private static readonly string[] commands = ["ls", "pwd", "mkdir", "rmdir", "rm", "find", "logout", "useradd", "su", "help", "touch", "find", "size", "ls", ">>", "chmod", "lca", "dfs", "bfs"];

        private readonly MainView mainView;
        private readonly StackPanel stackPanel;
        private readonly IAuthenticationService _authenticationService;

        private static readonly object lockObject = new();
        private static ConsoleInterfaceService instance;

        public static ConsoleInterfaceService Instance
        {
            get
            {
                lock (lockObject)
                {
                    instance ??= new ConsoleInterfaceService();
                    return instance;
                }
            }
        }

        private ConsoleInterfaceService()
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainView = (MainView)mainWindow.CurrentView;
            stackPanel = (StackPanel)mainView.scrollViewer.Content;

            // Dependency Injection
            var serviceProvider = (Application.Current as App)?.ServiceProvider;
            _authenticationService = serviceProvider.GetRequiredService<IAuthenticationService>();

            Node<File>.NodeCreated += Node_NodeCreated;
            Node<File>.NodeDisposed += Node_NodeDisposed;
        }


        public async void ExecuteCommand(string command)
        {
            DisablePreviousInputs();

            command = command.Trim();

            if (command == "clear")
            {
                ClearConsole();
                return;
            }

            else if (command == "exit")
            {
                Application.Current.Shutdown();
                return;
            }

            else if (command == "logout")
            {
                _authenticationService.Logout();
                FileSystemService.Instance.Logout();
                AddInputControl();
                return;
            }

            else if (command.StartsWith("useradd"))
            {
                try
                {
                    string arg = command.Split(' ').Skip(1).ToArray()[0];
                    await _authenticationService.SignUpAsync(arg);
                }

                catch (Exception ex)
                {
                    BufferOutputText(new OutputTextDTO($"{ex.Message}", hexColor: "#e053a1"));
                    ClearBuffer();
                }

                AddInputControl();
                return;
            }

            else if (command.StartsWith("su"))
            {
                try
                {
                    string arg = command.Split(' ').Skip(1).ToArray()[0];
                    await _authenticationService.LoginAsync(arg);
                }

                catch (Exception ex)
                {
                    BufferOutputText(new OutputTextDTO($"{ex.Message}", hexColor: "#e053a1"));
                    ClearBuffer();
                }

                AddInputControl();
                return;
            }


            if (SessionManager.CurrentSession?.User == null)
            {
                BufferOutputText(new OutputTextDTO("Access denied!", hexColor: "#e053a1"));
                ClearBuffer();
            }

            else
            {

                CommandOutput output = await ConsoleService.Execute(command);

                if (output.HasFailed)
                {
                    BufferOutputText(new OutputTextDTO($"{output?.exception?.Message}", hexColor: "#e053a1"));
                    ClearBuffer();
                }

                else
                {
                    foreach (OutputTextDTO text in output.OutputTexts)
                    {
                        BufferOutputText(text);
                    }

                    ClearBuffer();
                }
            }


            AddInputControl();
        }

        public async void OnInitialized()
        {
            if (!await _authenticationService.IsUsernameTakenAsync("root"))
            {
                await _authenticationService.SignUpAsync("root", "root", passMustBeString: false);

            }

            foreach (string command in commands)
            {
                NextWordPredictor.Instance.AddContextWord(command);
            }

            BufferOutputText(new OutputTextDTO("Welcome to the Linux File System Simulation\n", "Gray", false));
            BufferOutputText(new OutputTextDTO($"Current time is: {DateTime.Now}", "Gray", false));
            ClearBuffer();
            AddInputControl();
        }



    }
}
