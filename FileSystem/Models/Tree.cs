using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSystem.Models
{
    public class Node<T>
    {
        public Guid Key { get; } = Guid.NewGuid();
        public T Data { get; set; }
        public Node<T>? Parent { get; set; } = null;
        public ICollection<Node<T>> Children { get; set; } = new List<Node<T>>();

        public Node(T value)
        {
            Value = value;
        }

        public Node<T> AddChild(T value)
        {
            var node = new Node<T>(value) { Parent = this };
            Children.Add(node);
            return node;
        }
    }
    public class Tree<T>
    {
        public Node<T> Root { get; set; }




    
    }
}
