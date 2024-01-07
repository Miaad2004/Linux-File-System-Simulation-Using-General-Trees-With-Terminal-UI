namespace FileSystem.Models
{
    public sealed class Session(User user) : Entity
    {
        public User User { get; private set; } = user;
    }
}
