using FileSystem.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace FileSystem.Models
{
    public enum SearchAlgorithm
    {
        DFS,
        BFS
    }

    public class Node<T>
    {
        public Guid Key { get; set; } = Guid.NewGuid();
        public T Data { get; set; }
        public Node<T>? Parent { get; set; } = null;
        public ICollection<Node<T>> Children { get; set; } = new List<Node<T>>();

        public Node(T value)
        {
            Data = value;
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
            Node<T> clone = new Node<T>(this.Data);

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
            return Root;
        }

        private Node<T> DFS(Node<T> startNode, string path)
        {
            return Root;
        }

        private Node<T> BFS(Node<T> startNode, string path)
        {
            return Root;
        }

        public Node<T> FindNodeByPath(string path, Node<T>? startNode=null, SearchAlgorithm searchAlgorithm=SearchAlgorithm.DFS)
        {
            startNode ??= Root;

            if (searchAlgorithm == SearchAlgorithm.DFS)
            {
                return DFS(startNode, path);
            }

            else
            {
                return BFS(startNode, path);
            }
        }
    }
}
