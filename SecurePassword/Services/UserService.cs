using System.Security.Cryptography;
using System.Text;
using SecurePassword.DAL;
using SecurePassword.Models.DTO;
using SecurePassword.Models.Entity;

namespace SecurePassword.Services;

public class UserService
{
    /// <summary>
    ///     Singleton instance of the DatabaseManager.
    /// </summary>
    private readonly DatabaseManager _databaseManager;

    /// <summary>
    ///     Constructor for the UserService.
    /// </summary>
    /// <param name="databaseManager">Our DatabaseManager singleton, automatically passed using dependency injection</param>
    public UserService(DatabaseManager databaseManager)
    {
        _databaseManager = databaseManager;
    }

    /// <summary>
    ///     Checks if a user exists.
    /// </summary>
    /// <param name="email">The email to check if exists in the database</param>
    /// <returns>The user or null</returns>
    private User? UserExists(string email)
    {
        return _databaseManager.GetOne<User>(user => user.Email == email);
    }

    /// <summary>
    ///     Returns a user if the email and password match.
    /// </summary>
    /// <param name="email">The email to find</param>
    /// <param name="password">The plaintext password</param>
    /// <returns>The user or null, if the user doesn't exist or the values don't match</returns>
    private User? GetUser(string email, string password)
    {
        // First we check if the user exists
        var user = UserExists(email);

        // If the user doesn't exist, we return null
        if (user == null) return null;
        
        // Then we convert the password to bytes
        var passwordBytes = Encoding.UTF8.GetBytes(password);
        
        // Then we get the salt bytes from the user
        var saltBytes = Convert.FromBase64String(user.Salt);

        // Then we use SHA512 to hash the password bytes and the salt bytes
        var hashedPasswordBytes = SHA512.HashData(passwordBytes.Concat(saltBytes).ToArray());

        // Then we convert the hashed password bytes to a Base64 string
        var hashedPassword = Convert.ToBase64String(hashedPasswordBytes);
        
        // Then we check if the hashed password matches the user's password
        return hashedPassword == user.PasswordHash ? user : null;
    }

    /// <summary>
    ///     Uses the underlying GetUser method to find a user by email and password from a LoginRequest.
    /// </summary>
    /// <param name="loginRequest">The LoginRequest DTO</param>
    /// <returns>The user or null, if the user doesn't exist or the values don't match</returns>
    public User? GetUser(LoginRequest loginRequest)
    {
        // We use the underlying GetUser method to find a user by email and password
        return GetUser(loginRequest.Email, loginRequest.Password);
    }

    /// <summary>
    ///     Creates a new user.
    /// </summary>
    /// <param name="registerRequest">The registration request DTO</param>
    /// <returns>The new user or null</returns>
    public User? CreateUser(RegisterRequest registerRequest)
    {
        // First we check if the user already exists
        var user = UserExists(registerRequest.Email);

        // If the user exists, we return null
        if (user != null) return null;

        // Then we generate a random salt
        using var rng = RandomNumberGenerator.Create();
        var saltBytes = new byte[32];
        rng.GetBytes(saltBytes);

        // Then we convert the salt to a Base64 string
        var salt = Convert.ToBase64String(saltBytes);

        // Then we convert the password to bytes
        var passwordBytes = Encoding.UTF8.GetBytes(registerRequest.Password);

        // Then we use SHA512 to hash the password bytes and the salt bytes
        var hashedPasswordBytes = SHA512.HashData(passwordBytes.Concat(saltBytes).ToArray());

        // Then we convert the hashed password bytes to a Base64 string
        var hashedPassword = Convert.ToBase64String(hashedPasswordBytes);

        // Then we create a new user
        user = new User
        {
            Email = registerRequest.Email,
            PasswordHash = hashedPassword,
            Salt = salt
        };
        
        // Then we add the user to the database
        _databaseManager.Add(user);
        
        // Then we return the user
        return user;
    }
}