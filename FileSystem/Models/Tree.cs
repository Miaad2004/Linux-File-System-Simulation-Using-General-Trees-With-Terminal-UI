namespace FileSystem.Models
{
    public enum SearchAlgorithm
    {
        DFS,
        BFS
    }

    public class Node<T>
    {
        public static event EventHandler<NodeEventArgs<T>> NodeCreated;
        public static event EventHandler<NodeEventArgs<T>> NodeDisposed;

        public Guid Key { get; set; } = Guid.NewGuid();

        public T Data { get; set; }

        public Node<T>? Parent { get; set; } = null;

        public ICollection<Node<T>> Children { get; set; } = new List<Node<T>>();

        public Node(T value)
        {
            Data = value;
            NodeCreated?.Invoke(this, new NodeEventArgs<T>(Data));
        }

        ~Node()
        {
            NodeDisposed?.Invoke(this, new NodeEventArgs<T>(Data));
        }

        public Node<T> AddChild(T value)
        {
            var node = new Node<T>(value) { Parent = this };
            Children.Add(node);
            return node;
        }

        public Node<T> AddChild(Node<T> newNode)
        {
            Children.Add(newNode);
            return newNode;
        }

        public Node<T> Clone()
        {
            Node<T> clone = new(this.Data);

            foreach (var child in Children)
            {
                Node<T> childClone = child.Clone();
                childClone.Parent = clone;
                clone.Children.Add(childClone);
            }

            return clone;
        }

        public List<Node<T>> GetPathToRoot()
        {
            List<Node<T>> path = [];
            Node<T>? node = this;

            while (node != null)
            {
                path.Add(node);
                node = node.Parent;
            }

            path.Reverse();
            return path;
        }
    }


    public class Tree<T>
    {
        public Node<T> Root { get; set; }

        public Node<T> GetFirstCommonAncestor(Node<T> node1, Node<T> node2)
        {
            var path1 = node1.GetPathToRoot();
            var path2 = node2.GetPathToRoot();

            int i = 0;
            while (i < path1.Count && i < path2.Count && path1[i] == path2[i])
            {
                i++;
            }

            return path1[i - 1];
        }
    }
}
