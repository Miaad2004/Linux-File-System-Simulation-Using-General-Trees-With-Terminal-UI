using System.Security.Cryptography;
using System.Text;

namespace FileSystem.Services.Authentication
{
    public class PasswordServiceSHA : IPasswordService
    {
        public string GetHash(string? password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException($"{nameof(password)} can't be empty.");
            }

            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

            SHA256 sha256 = SHA256.Create();
            byte[] hashBytes = sha256.ComputeHash(passwordBytes);

            return BitConverter.ToString(hashBytes).Replace("-", "");
        }

        public bool IsStrong(string? password)
        {
            return string.IsNullOrWhiteSpace(password)
                ? throw new ArgumentException($"{nameof(password)} can't be empty.")
                : password.Length >= 8 &&
                  password.Any(char.IsUpper) &&
                  password.Any(char.IsLower) &&
                  password.Any(char.IsDigit);
        }

        public bool IsValid(string? password,
                            string? passwordRepeat)
        {
            if (password is null) throw new ArgumentException(nameof(password));
            if (passwordRepeat is null) throw new ArgumentException(nameof(passwordRepeat));

            return password == passwordRepeat;
        }
    }
}
