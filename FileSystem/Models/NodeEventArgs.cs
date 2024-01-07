namespace FileSystem.Models
{
    public class NodeEventArgs<T>(T data) : EventArgs
    {
        public T Data = data;
    }
}
