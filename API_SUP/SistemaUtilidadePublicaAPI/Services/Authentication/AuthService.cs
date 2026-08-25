using SistemaUtilidadePublicaAPI.Common.Exceptions;
using SistemaUtilidadePublicaAPI.Data.Repositories;
using SistemaUtilidadePublicaAPI.DTOs.Authentication;
using SistemaUtilidadePublicaAPI.Models;
using System.Security.Cryptography;
using System.Text;

namespace SistemaUtilidadePublicaAPI.Services.Authentication;

public class AuthService
{
    private readonly UserRepository _userRepository;

    public AuthService(UserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserResponseDto> RegisterAsync(RegisterUserDto dto)
    {
        var email = dto.Email.Trim().ToLowerInvariant();

        var emailExists = await _userRepository.EamilExistsAsync(email);

        if (emailExists)
        {
            throw new EmailAlreadyExistsException();
        }

        var salt = RandomNumberGenerator.GetBytes(16);

        var passwordHash = HashPassword(
            dto.Password,
            salt);

        var user = new User
        {
            Name = dto.Name.Trim(),
            Telefone = dto.telephone?.Trim(),
            Email = email,
            PasswordHash = passwordHash,
            Salt = Convert.ToBase64String(salt),
            ImagemPerfil = null,
            Create_Date = DateTime.UtcNow
        };

        var userId = await _userRepository.CreateAsync(user);

        return new UserResponseDto
        {
            Id_User = userId,
            Name = user.Name,
            Telephone = user.Telefone,
            Email = user.Email,
            ImagemPerfil = user.ImagemPerfil,
            Create_Data = user.Create_Date
        };
    }

    private static string HashPassword(
        string password,
        byte[] salt)
    {
        var passwordBytes =
            Encoding.UTF8.GetBytes(password);

        var hash = Rfc2898DeriveBytes.Pbkdf2(
            passwordBytes,
            salt,
            100_000,
            HashAlgorithmName.SHA256,
            32);

        return Convert.ToBase64String(hash);
    }

    public async Task<UserResponseDto> LoginService(LoginUserDto dto)
    {
       User? result = await _userRepository.LoginAsync(dto);

        if (result == null)
        { 
            throw new EmailNotExistsException();
        
        }
       string passwordHash = HashPassword(dto.Password, Convert.FromBase64String(result.Salt));
       
       if (passwordHash != result.PasswordHash)
       {
           throw new InvalidPasswordException();
       }

        return new UserResponseDto
        {
            Id_User = result.Id_User,
            Name = result.Name,
            Email = result.Email,
            Telephone = result.Telefone,
            Create_Data = result.Create_Date,
            ImagemPerfil = result.ImagemPerfil,
        };
    }

}