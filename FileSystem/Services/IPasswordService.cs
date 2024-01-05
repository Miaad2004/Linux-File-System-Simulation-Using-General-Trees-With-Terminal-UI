using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSystem.Services
{
    public interface IPasswordService
    {
        string GetHash(string? password);
        bool IsStrong(string? password);
        bool IsValid(string? password, string? passwordRepeat);
    }
}
