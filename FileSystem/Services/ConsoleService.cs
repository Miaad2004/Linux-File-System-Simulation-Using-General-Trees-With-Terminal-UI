using FileSystem.Models;
using WordPredictor;

namespace FileSystem.Services
{
    public class ConsoleService
    {
        private static ConsoleService _instance;

        public static ConsoleService Instance
        {
            get
            {
                _instance ??= new ConsoleService();
                return _instance;
            }
        }

        public static async Task<CommandOutput> Execute(string command)
        {
            OutputTextDTO result;
            var output = new CommandOutput(HasFailed: false);

            try
            {
                if (command.Contains(">>"))
                {
                    string[] args = command.Split(">>");
                    FileSystemService.Instance.WriteToFile(args);
                    return output;
                }

                var commandParts = command.Split(' ');
                var commandName = commandParts[0];
                var commandArgs = commandParts.Skip(1).ToArray();

                switch (commandName)
                {
                    case "cd":
                        FileSystemService.Instance.ChangeDirectory(commandArgs);
                        break;

                    case "ls":
                        List<OutputTextDTO> fileNames = FileSystemService.Instance.ListFiles(commandArgs);
                        output.OutputTexts = fileNames;
                        break;

                    case "mkdir":
                        FileSystemService.Instance.MakeDirectory(commandArgs);
                        break;

                    case "rmdir":
                        FileSystemService.Instance.RemoveDirectory(commandArgs);
                        break;

                    case "touch":
                        FileSystemService.Instance.Touch(commandArgs);
                        break;

                    case "rm":
                        FileSystemService.Instance.Remove(commandArgs);
                        break;

                    case "mv":
                        FileSystemService.Instance.Move(commandArgs);
                        break;

                    case "cp":
                        FileSystemService.Instance.Copy(commandArgs);
                        break;

                    case "cat":
                        output.OutputTexts.Add(FileSystemService.Instance.Cat(commandArgs));
                        break;

                    case "find":
                        output.OutputTexts.Add(FileSystemService.Instance.Find(commandArgs));
                        break;

                    case "lca":
                        output.OutputTexts.Add(FileSystemService.Instance.LCA(commandArgs));
                        break;

                    case "sz":
                        result = FileSystemService.Instance.Size(commandArgs);
                        output.OutputTexts.Add(result);
                        break;

                    case "chmod":
                        FileSystemService.Instance.Chmod(commandArgs);
                        break;

                    case "pwd":
                        result = FileSystemService.Instance.PWD();
                        output.OutputTexts.Add(result);
                        break;

                    case "help":
                        string response = await LLM.GetResponse(command);
                        output.OutputTexts.Add(new OutputTextDTO(response));
                        break;

                    default:
                        throw new Exception("Command not found");
                }
            }

            catch (Exception e)
            {
                output.HasFailed = true;
                output.exception = e;
            }

            return output;
        }
    }
}
