using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public DateTime CreationDate { get; private set; }
        public DateTime? LastAccessedDate { get; private set; } = null;
        public bool IsReadOnly { get; private set; } = false;
        public bool IsHidden { get; private set; } = false;
        public bool IsDirectory { get; private set; }
        public ICollection<Ownership> OwnerShips { get; protected set; }

        public File(string title, bool isDirectory)
        {
            Title = title;
            IsDirectory = isDirectory;
            CreationDate = DateTime.Now;
        }



    }
}
