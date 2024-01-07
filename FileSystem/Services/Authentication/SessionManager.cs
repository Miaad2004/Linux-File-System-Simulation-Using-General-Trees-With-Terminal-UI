using FileSystem.Models;

namespace FileSystem.Services.Authentication
{
    public static class SessionManager
    {
        public static Session? CurrentSession { get; set; } = null;

        public static void Revoke()
        {
            CurrentSession = null;
        }

    }
}
