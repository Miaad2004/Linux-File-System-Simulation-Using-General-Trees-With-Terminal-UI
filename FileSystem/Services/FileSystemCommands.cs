using FileSystem.Exceptions;
using FileSystem.Models;

namespace FileSystem.Services
{
    public partial class FileSystemService
    {
        public void ChangeDirectory(string[] args)
        {
            var newPath = args[0];
            var currentNode = GetNodeByPath(newPath);

            if (!currentNode.Data.IsDirectory)
                throw new UIException("Can not cd to a file.");

            CurrentDirectory = currentNode;
        }

        public void MakeDirectory(string[] args)
        {
            var path = args[0];
            var targetNode = GetNodeByPath(GetPathWithoutFileName(path));

            if (!targetNode.Data.IsDirectory)
                throw new UIException("Can not create a folder in a file.");

            if (targetNode.Data.GetCurrentUserAccessLevel() < AccessLevels.Write && !CurrentUser.IsRoot)
                throw new UIException("You don't have permission to create a directory here");

            targetNode.AddChild(new File(GetFileNameFromPath(path), isDirectory: true));
        }

        public void RemoveDirectory(string[] args)
        {
            var path = args[0];

            var targetNode = GetNodeByPath(GetPathWithoutFileName(path));
            var dirName = GetFileNameFromPath(path);

            // check if it exists
            var child = targetNode.Children.FirstOrDefault(c => c.Data.Title == dirName && c.Data.IsDirectory)
                        ?? throw new UIException($"Directory {dirName} does not exist");

            if (child.Data.GetCurrentUserAccessLevel() < AccessLevels.Write && !CurrentUser.IsRoot)
                throw new UIException("You don't have permission to remove this directory");

            // check if it is empty
            if (child.Children.Count > 0)
                throw new UIException($"Directory {dirName} is not empty");

            targetNode.Children.Remove(child);
        }

        public void Touch(string[] args)
        {
            var path = args[0];

            var targetNode = GetNodeByPath(GetPathWithoutFileName(path));

            if (targetNode.Data.GetCurrentUserAccessLevel() < AccessLevels.Write && !CurrentUser.IsRoot)
                throw new UIException("You don't have permission to create a file here");

            targetNode.AddChild(new File(GetFileNameFromPath(path), isDirectory: false));
        }

        public void Remove(string[] args)
        {
            var path = args[0];

            var targetNode = GetNodeByPath(GetPathWithoutFileName(path));
            var fileName = GetFileNameFromPath(path);

            // check if it exists
            var child = targetNode.Children.FirstOrDefault(c => c.Data.Title == fileName && !c.Data.IsDirectory)
                        ?? throw new UIException($"File {fileName} does not exist");


            if (child.Data.GetCurrentUserAccessLevel() < AccessLevels.Write && !CurrentUser.IsRoot)
                throw new UIException("You don't have permission to remove this file");

            targetNode.Children.Remove(child);
        }

        public List<OutputTextDTO> ListFiles(string[] args)
        {
            string? path;

            if (args.Length == 0 || args[0] == "")
                path = null;

            else
                path = args[0];

            Node<File> targetNode;

            if (path == null)
                targetNode = CurrentDirectory;
            else
                targetNode = GetNodeByPath(path);

            List<OutputTextDTO> output = [];
            foreach (var child in targetNode.Children)
            {
                var DTO = new OutputTextDTO(child.Data.Title + '\n');
                if (child.Data.IsDirectory)
                    DTO.HexColor = "#53e0af"; // Green

                output.Add(DTO);
            }

            return output;
        }

        public void Copy(string[] args)
        {
            var sourcePath = args[0];
            var targetPath = args[1];

            var sourceNode = GetNodeByPath(sourcePath);
            var targetNode = GetNodeByPath(targetPath);

            if (targetNode.Data.GetCurrentUserAccessLevel() < AccessLevels.Write && !CurrentUser.IsRoot)
                throw new UIException("You don't have permission to write here");

            if (!targetNode.Data.IsDirectory)
                throw new UIException("Target must be a directory");


            targetNode.AddChild(sourceNode.Clone());
        }

        public void Move(string[] args)
        {
            var sourcePath = args[0];
            var targetPath = args[1];

            var sourceNode = GetNodeByPath(sourcePath);
            var targetNode = GetNodeByPath(targetPath);

            if (targetNode.Data.GetCurrentUserAccessLevel() < AccessLevels.Write && !CurrentUser.IsRoot)
                throw new UIException("You don't have permission to write here");

            if (!targetNode.Data.IsDirectory)
                throw new UIException("Target must be a directory");

            if (sourceNode.Parent == null)
                throw new UIException("Cannot move root directory");

            targetNode.AddChild(sourceNode);
            sourceNode.Parent.Children.Remove(sourceNode);
        }

        public OutputTextDTO Cat(string[] args)
        {
            var sourcePath = args[0];

            var node = GetNodeByPath(sourcePath);

            if (node.Data.IsDirectory)
                throw new UIException("Target must be a file");

            return new OutputTextDTO(node.Data.Content);
        }

        public OutputTextDTO Find(string[] args)
        {
            var pathToSearch = args[0];
            var type = args[1];
            var targetFileName = args[2];
            var searchMethod = args[3];

            if (type != "d" && type != "f")
                throw new UIException("Invalid type");

            if (searchMethod != "bfs" && searchMethod != "dfs")
                throw new UIException("Invalid search method");

            var searchStartNode = GetNodeByPath(pathToSearch);
            bool isTargetDirectory = type == "d";
            SearchAlgorithm searchAlgorithm = searchMethod == "bfs" ? SearchAlgorithm.BFS : SearchAlgorithm.DFS;

            Node<File>? searchResult = SearchTree(targetFileName, searchStartNode, isTargetDirectory, searchAlgorithm);

            if (searchResult == null)
                return new OutputTextDTO("Not found");

            return new OutputTextDTO(GetPathByNode(searchResult));
        }

        private Node<File>? SearchTree(string targetFileName, Node<File>? searchStartNode, bool isTargetDirectory, SearchAlgorithm searchAlgorithm)
        {
            searchStartNode ??= Tree.Root;

            return searchAlgorithm switch
            {
                SearchAlgorithm.BFS => SearchBFS(targetFileName, searchStartNode, isTargetDirectory),
                SearchAlgorithm.DFS => SearchDFS(targetFileName, searchStartNode, isTargetDirectory),
                _ => throw new UIException("Invalid search algorithm"),
            };
        }

        private Node<File>? SearchDFS(string targetFileName, Node<File> searchStartNode, bool isTargetDirectory)
        {
            Stack<Node<File>> stack = new();
            HashSet<Node<File>> visited = [];

            stack.Push(searchStartNode);

            while (stack.Count > 0)
            {
                Node<File> currentNode = stack.Pop();

                if (currentNode.Data.Title == targetFileName && currentNode.Data.IsDirectory == isTargetDirectory)
                    return currentNode;

                visited.Add(currentNode);

                foreach (Node<File> neighbor in currentNode.Children.Where(n => !visited.Contains(n)))
                    stack.Push(neighbor);
            }

            // Target not found
            return null;
        }

        private Node<File>? SearchBFS(string targetFileName, Node<File> searchStartNode, bool isTargetDirectory)
        {
            Queue<Node<File>> queue = new();
            HashSet<Node<File>> visited = [];

            queue.Enqueue(searchStartNode);

            while (queue.Count > 0)
            {
                Node<File> currentNode = queue.Dequeue();

                if (currentNode.Data.Title == targetFileName && currentNode.Data.IsDirectory == isTargetDirectory)
                    return currentNode;


                visited.Add(currentNode);

                foreach (Node<File> neighbor in currentNode.Children.Where(n => !visited.Contains(n)))
                    queue.Enqueue(neighbor);
            }

            // Target not found
            return null;
        }


        public OutputTextDTO LCA(string[] args)
        {
            var path1 = args[0];
            var path2 = args[1];

            var node1 = GetNodeByPath(path1);
            var node2 = GetNodeByPath(path2);
            Node<File> commonAncestor = Tree.GetFirstCommonAncestor(node1, node2);

            return new OutputTextDTO(GetPathByNode(commonAncestor));
        }

        private static long Size(Node<File> node)
        {
            if (!node.Data.IsDirectory)
                return node.Data.Size;

            else
            {
                long totalSize = 0;
                foreach (var child in node.Children)
                {
                    totalSize += Size(child);
                }

                return totalSize;
            }
        }

        public OutputTextDTO Size(string[] args)
        {
            var path = args[0];

            var node = GetNodeByPath(path);
            return new OutputTextDTO($"{Size(node)} Bytes");
        }

        public void Chmod(string[] args)
        {
            string accessLevel = args[0];
            string path = args[1];

            var node = GetNodeByPath(path);

            switch (accessLevel)
            {
                case "r":
                    node.Data.SetCurrentUserAccessLevel(AccessLevels.Read);
                    break;

                case "rw":
                    node.Data.SetCurrentUserAccessLevel(AccessLevels.ReadWrite);
                    break;

                default:
                    throw new UIException("Invalid access level");
            }

        }

        public OutputTextDTO PWD()
        {
            var pathToRoot = CurrentDirectory.GetPathToRoot().Select(node => node.Data.Title);
            return new OutputTextDTO($"/{string.Join('/', pathToRoot)}");

        }

        public void WriteToFile(string[] args)
        {
            var targetPath = args[1].Trim();

            var targetNode = GetNodeByPath(targetPath);

            if (targetNode.Data.GetCurrentUserAccessLevel() < AccessLevels.Write && !CurrentUser.IsRoot)
                throw new UIException("You don't have permission to write here");

            if (targetNode.Data.IsDirectory)
                throw new UIException("Target must be a file");

            string content = args[0].Trim();
            targetNode.Data.Content += content;
        }
    }
}
