namespace FileSystem.Models
{
    public class Ownership : Entity
    {
        public User User { get; set; }
        public File File { get; set; }
        public AccessLevels AccessLevel { get; set; }

        public Ownership(User user, File file, AccessLevels accessLevel)
        {
            User = user;
            File = file;
            AccessLevel = accessLevel;
        }

        public Ownership()
        {

        }
    }
}
