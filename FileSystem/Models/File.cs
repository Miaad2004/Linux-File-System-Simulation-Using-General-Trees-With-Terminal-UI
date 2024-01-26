using FileSystem.Services.Authentication;
using System.Text;

namespace FileSystem.Models
{
    public class File : Entity
    {
        private string _title = "";

        public string Title
        {
            get => _title;

            protected set
            {
                // Common FileName checks

                if (value.Length < 3)
                    throw new Exception("File name must be at least 3 characters long");

                if (value.Length > 20)
                    throw new Exception("File name must be at most 20 characters long");

                if (value.Any(c => !char.IsLetterOrDigit(c)))
                    throw new Exception("File name must contain only letters and digits");

                _title = value;
            }
        }

        public string Content { get; set; } = "";
        public DateTime CreationDate { get; private set; }
        public bool IsDirectory { get; private set; }
        public ICollection<Ownership> OwnerShips { get; protected set; } = [];

        public long Size => Encoding.UTF8.GetBytes(Content).Length;
        private User? CurrentUser => SessionManager.CurrentSession?.User;

        public File(string title, bool isDirectory)
        {
            if (CurrentUser == null && title != "root")
                throw new Exception("No user is logged in");

            OwnerShips.Add(new Ownership(CurrentUser, this, AccessLevels.ReadWrite));
            Title = title;
            IsDirectory = isDirectory;
            CreationDate = DateTime.Now;
        }

        public File(string title, bool isDirectory, User owner)
        {
            OwnerShips.Add(new Ownership(owner, this, AccessLevels.ReadWrite));
            Title = title;
            IsDirectory = isDirectory;
            CreationDate = DateTime.Now;
        }

        public File()
        {

        }

        public AccessLevels GetCurrentUserAccessLevel()
        {
            User? currentUser = SessionManager.CurrentSession?.User;

            if (currentUser == null)
                return AccessLevels.None;

            Ownership? ownership = this.OwnerShips.FirstOrDefault(ow => ow.User == currentUser);

            if (ownership == null)
                return AccessLevels.None;

            return ownership.AccessLevel;
        }

        public void SetCurrentUserAccessLevel(AccessLevels accessLevel)
        {
            User? currentUser = (SessionManager.CurrentSession?.User) ?? throw new Exception("No user is logged in");

            Ownership ownership = new(currentUser, this, accessLevel);

            OwnerShips.Add(ownership);
        }
    }
}
