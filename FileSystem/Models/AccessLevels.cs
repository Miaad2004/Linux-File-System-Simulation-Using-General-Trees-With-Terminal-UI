namespace FileSystem.Models
{
    public enum AccessLevels
    {
        // File Access Levels
        None = 0,
        Read = 1,
        Write = 2,
        ReadWrite = Read | Write,
    }
}
