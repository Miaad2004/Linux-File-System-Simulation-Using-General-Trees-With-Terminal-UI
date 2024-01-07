using FileSystem.Data;
using FileSystem.Exceptions;
using FileSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Authentication;

namespace FileSystem.Services.Authentication
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly LinuxDbContext _dbContext;
        private readonly IPasswordService _passwordService;

        public AuthenticationService(LinuxDbContext dbContext,
                                     IPasswordService passwordService)
        {
            _dbContext = dbContext;
            _passwordService = passwordService;
        }

        public async Task<bool> IsUsernameTakenAsync(string username)
        {
            return await _dbContext.Users.AnyAsync(u => u.Username == username);
        }

        public async Task SignUpAsync(string username, string password, bool passMustBeString = true)
        {
            if (username != "root" && (SessionManager.CurrentSession?.User?.IsRoot == false || SessionManager.CurrentSession == null))
            {
                throw new UIException("Only root can create new users.");
            }

            if (await IsUsernameTakenAsync(username))
            {
                throw new UIException($"Username {username} is already taken.");
            }

            if (!_passwordService.IsStrong(password) && passMustBeString)
            {
                throw new UIException("Password must be at least 8 characters and contain upper and lower letters.");
            }

            string passwordHash = _passwordService.GetHash(password);

            bool isRoot = username == "root";
            User user = new(username, passwordHash, isRoot);

            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();
        }

        public async Task LoginAsync(string username, string password)
        {
            string passwordHash = _passwordService.GetHash(password);

            Session? session = SessionManager.CurrentSession;
            if (session != null)
            {
                throw new UIException("There is an active session. Please logout first.");
            }

            User user = await _dbContext.Users.SingleOrDefaultAsync(u => u.Username == username && u.PasswordHash == passwordHash)
                            ?? throw new InvalidCredentialException("Wrong username or password!");

            SessionManager.CurrentSession = new Session(user);
        }

        public async Task LoginAsync(string arg)
        {
            string[] args = arg.Split('@');
            await LoginAsync(args[0], args[1]);
        }

        public async Task SignUpAsync(string arg, bool passMustBeString = true)
        {
            string[] args = arg.Split('@');
            await SignUpAsync(args[0], args[1], passMustBeString);
        }

        public void Logout()
        {
            SessionManager.Revoke();
        }
    }
}
