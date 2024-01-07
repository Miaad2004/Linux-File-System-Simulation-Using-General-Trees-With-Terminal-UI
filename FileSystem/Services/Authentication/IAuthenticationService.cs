namespace FileSystem.Services.Authentication
{
    public interface IAuthenticationService
    {
        Task<bool> IsUsernameTakenAsync(string username);
        Task SignUpAsync(string username, string password, bool passMustBeString = true);
        Task SignUpAsync(string arg, bool passMustBeString = true);
        Task LoginAsync(string arg);
        Task LoginAsync(string username, string password);
        void Logout();
    }
}
