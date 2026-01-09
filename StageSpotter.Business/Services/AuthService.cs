using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using StageSpotter.Business.Interfaces;
using StageSpotter.Data.Interfaces;
using StageSpotter.Domain.Models;

namespace StageSpotter.Business.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly StageSpotter.Data.Interfaces.IBedrijfRepository _bedrijfRepository;
        private readonly Microsoft.IdentityModel.Tokens.SymmetricSecurityKey _signingKey;

        public AuthService(IUserRepository userRepository, StageSpotter.Data.Interfaces.IBedrijfRepository bedrijfRepository, Microsoft.IdentityModel.Tokens.SymmetricSecurityKey signingKey)
        {
            _userRepository = userRepository;
            _bedrijfRepository = bedrijfRepository;
            _signingKey = signingKey;
        }

        public Task<int> RegisterAsync(string email, string password)
        {
            var existing = _userRepository.GetUserByEmail(email);
            if (existing != null)
            {
                return Task.FromResult(0);
            }

            var hash = HashPassword(password);
            var user = new User { Email = email, PasswordHash = hash, Type = UserType.Student };
            var id = _userRepository.CreateUser(user);
            return Task.FromResult(id);
        }

        public Task<int> RegisterAsync(string email, string password, UserType type, StageSpotter.Data.DTOs.BedrijfDto? bedrijfDto = null)
        {
            var existing = _userRepository.GetUserByEmail(email);
            if (existing != null)
            {
                return Task.FromResult(0);
            }

            var hash = HashPassword(password);
            var user = new User { Email = email, PasswordHash = hash, Type = type };

            if (type == UserType.Bedrijf && bedrijfDto != null)
            {
                // create or find company
                var existingBedrijf = _bedrijfRepository.FindByName(bedrijfDto.Naam);
                if (existingBedrijf == null)
                {
                    existingBedrijf = _bedrijfRepository.Create(bedrijfDto);
                }
                user.BedrijfId = existingBedrijf.Id;
            }

            var id = _userRepository.CreateUser(user);
            return Task.FromResult(id);
        }

        public Task<string> LoginAsync(string email, string password)
        {
            var user = _userRepository.GetUserByEmail(email);
            if (user == null)
            {
                return Task.FromResult<string>(null);
            }

            var hash = HashPassword(password);
            if (user.PasswordHash != hash)
            {
                return Task.FromResult<string>(null);
            }

            var token = GenerateToken(user);
            return Task.FromResult(token);
        }

        private string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hashed = sha.ComputeHash(bytes);
            var sb = new StringBuilder();
            foreach (var b in hashed)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }

        private string GenerateToken(User user)
        {
            var credentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("usertype", user.Type == UserType.Bedrijf ? "bedrijf" : "student")
            };
            if (user.BedrijfId.HasValue)
            {
                claims.Add(new Claim("bedrijfId", user.BedrijfId.Value.ToString()));
            }

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
