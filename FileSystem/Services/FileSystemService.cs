using FileSystem.Exceptions;
using FileSystem.Models;
using FileSystem.Services.Authentication;

namespace FileSystem.Services
{
    public partial class FileSystemService
    {

        private static User CurrentUser => SessionManager.CurrentSession?.User ?? throw new ArgumentNullException();

        public Tree<File> Tree { get; } = new Tree<File>();

        public Node<File> CurrentDirectory { get; set; }

        // Singeltone
        private static FileSystemService _instance;

        public static FileSystemService Instance
        {
            get
            {
                _instance ??= new FileSystemService();
                return _instance;
            }
        }

        private FileSystemService()
        {
            var file = new File("root", isDirectory: true);
            CurrentDirectory = Tree.Root = new Node<File>(file);
        }

        public static string GetPathByNode(Node<File> node)
        {
            var path = node.Data.Title;

            while (node.Parent != null)
            {
                node = node.Parent;
                path = node.Data.Title + "/" + path;
            }

            return "/" + path;
        }

        public string GetCurrentDirString()
        {
            return GetPathByNode(CurrentDirectory);
        }

        public Node<File> GetNodeByPath(string path)
        {
            Node<File> node;

            var pathParts = SplitPath(path);

            // Handling absolute paths 
            if (path.StartsWith("/"))
            {
                node = Tree.Root;
                pathParts.RemoveAt(0);
                pathParts.RemoveAt(0);
            }

            else
                node = CurrentDirectory;


            // Traversing the tree
            foreach (var part in pathParts)
            {
                if (part == "")
                    continue;

                else if (part == "..")
                {
                    if (node?.Parent == null)
                        return node;

                    if (node.Parent?.Data.GetCurrentUserAccessLevel() >= AccessLevels.Read || CurrentUser.IsRoot)
                        node = node.Parent;
                }

                else
                {
                    var child = node.Children.FirstOrDefault(node => node.Data.Title == part);

                    if (node.Parent?.Data.GetCurrentUserAccessLevel() < AccessLevels.Read && !CurrentUser.IsRoot)
                        throw new Exception("Access Denied!");

                    if (child != null)
                        node = child;

                    else
                        throw new UIException($"Directory {part} does not exist");
                }
            }

            return node;
        }

        private static List<string> SplitPath(string path)
        {
            return path.Split('/').ToList();
        }

        private static string GetFileNameFromPath(string path)
        {
            return SplitPath(path).Last();
        }

        private static string GetPathWithoutFileName(string path)
        {
            List<string> pathParts = SplitPath(path);

            return path[..^pathParts.Last().Length];
        }

        public void Logout()
        {
            CurrentDirectory = Tree.Root;
        }

    }
}
