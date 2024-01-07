namespace FileSystem.Models
{
    public abstract class Entity
    {
        public Guid Id { get; protected set; } = Guid.NewGuid();

        public Entity()
        {

        }
    }
}
