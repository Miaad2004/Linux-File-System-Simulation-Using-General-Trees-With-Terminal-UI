using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSystem.Models
{
    public class Ownership: Entity
    {
        User User { get; set; }
        File File { get; set; }
        AccessLevels AccessLevel { get; set; }

        public Ownership(User user, File file, AccessLevels accessLevel)
        {
            User = user;
            File = file;
            AccessLevel = accessLevel;
        }
    }
}
