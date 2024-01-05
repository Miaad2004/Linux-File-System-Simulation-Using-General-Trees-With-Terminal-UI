using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSystem.Models
{
    public enum AccessLevels
    {
        // File Access Levels
        Read = 1,
        Write = 2,
        ReadWrite = Read | Write,
    }
}
