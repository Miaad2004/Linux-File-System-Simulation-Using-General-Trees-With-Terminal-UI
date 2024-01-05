using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSystem.Models
{
    public class User: Entity
    {
        private string _username;
        
        public string Username
        {
            get => _username;
            protected set
            {
                // Username checks
                if (value.Length < 3)
                    throw new Exception("Username must be at least 3 characters long");

                if (value.Length > 20)
                    throw new Exception("Username must be at most 20 characters long");

                if (value.Any(c => !char.IsLetterOrDigit(c)))
                    throw new Exception("Username must contain only letters and digits");

                _username = value;
            }
        }


        public string PasswordHash { get; protected set; }

        public DateTime CreationDate { get; private set; }

        public DateTime? LastAccessedDate { get; private set; } = null;

        public bool IsRoot { get; private set; } = false;

        public ICollection<Ownership> OwnerShips { get; protected set; }

        public User(string username, string passwordHash)
        {
            Username = username;
            PasswordHash = passwordHash;
            CreationDate = DateTime.Now;
        }

    }
}
