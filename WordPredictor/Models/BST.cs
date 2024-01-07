namespace WordPredictor.Models
{
    public sealed class Node(string key, string data)
    {
        public string Key = key;
        public string Data = data;
        public Node? Left = null;
        public Node? Right = null;
        public Node? Parent = null;
    }

    public sealed class BST
    {
        public Node? Root = null;

        private static Node InsertRecursive(Node? root, string key, string data = "")
        {
            if (root == null)
                return new Node(key, data);

            else
            {
                if (String.Compare(root.Key, key, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    root.Data = data;
                    return root;
                }

                else if (String.Compare(key, root.Key, StringComparison.OrdinalIgnoreCase) < 0)
                {
                    root.Left = InsertRecursive(root.Left, key, data);
                    root.Left.Parent = root;
                }

                else
                {
                    root.Right = InsertRecursive(root.Right, key, data);
                    root.Right.Parent = root;
                }
            }

            return root;
        }

        private static Node? RemoveRecursive(Node? root, string key)
        {
            if (root == null)
                return null;

            int compareResult = String.Compare(key, root.Key, StringComparison.OrdinalIgnoreCase);

            if (compareResult < 0)
                root.Left = RemoveRecursive(root.Left, key);

            else if (compareResult > 0)
                root.Right = RemoveRecursive(root.Right, key);

            else // Found
            {
                // Case 1: No children
                if (root.Left == null && root.Right == null)
                {
                    root = null;
                }

                // Case 2: One child
                else if (root.Left == null)
                    root = root.Right;

                else if (root.Right == null)
                    root = root.Left;

                // Case 3: Two children
                else
                {
                    var minRight = GetMin(root.Right);
                    root.Key = minRight.Key;
                    root.Data = minRight.Data;
                    root.Right = RemoveRecursive(root.Right, minRight.Key);
                }
            }

            return root;
        }

        private static Node? SearchRecursive(Node? root, string key)
        {
            if (root == null)
                return null;

            if (String.Compare(root.Key, key, StringComparison.OrdinalIgnoreCase) == 0)
                return root;

            else if (String.Compare(key, root.Key, StringComparison.OrdinalIgnoreCase) < 0)
                return SearchRecursive(root.Left, key);

            else
                return SearchRecursive(root.Right, key);
        }

        public string PredictiveSearch(string prefix)
        {
            string bestMatch = "";
            PredictiveSearchRecursive(Root, prefix, ref bestMatch);
            return bestMatch;
        }

        private static void PredictiveSearchRecursive(Node? root, string prefix, ref string bestMatch)
        {
            if (root == null)
                return;

            int compareResult = String.Compare(root.Key, prefix, StringComparison.OrdinalIgnoreCase);

            if (root.Key.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            {
                if (root.Key.Length > bestMatch.Length)
                    bestMatch = root.Key;
            }

            if (compareResult >= 0)
                PredictiveSearchRecursive(root.Left, prefix, ref bestMatch);

            if (compareResult <= 0)
                PredictiveSearchRecursive(root.Right, prefix, ref bestMatch);
        }


        public void Insert(string key, string data = "")
        {
            Root = InsertRecursive(Root, key, data);
        }

        public Node? Remove(string key)
        {
            Root = RemoveRecursive(Root, key);
            return Root;
        }

        public Node? Search(string key)
        {
            return SearchRecursive(Root, key);
        }

        public void Clear()
        {
            Root = null;
        }

        private static Node? GetMin(Node? root)
        {
            if (root == null)
                return null;

            while (root.Left != null)
                root = root.Left;

            return root;
        }

        private static void PrintInorder(Node? root)
        {
            if (root == null)
                return;

            PrintInorder(root.Left);
            Console.Write($"{root.Key} ");
            PrintInorder(root.Right);
        }
    }
}
