using UserMicroService.Models; 
using UserMicroService.Repositories;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using StackExchange.Redis;

namespace UserMicroService.Services
{
    public class UserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IDatabase _redisCache;
        private readonly byte[] _key;
        private readonly int _cacheExpiryTime=30;
        private readonly IConfiguration _configuration;
    private readonly PublisherService _publisherService;

        public UserService(IUserRepository userRepository, IDatabase redisCache, IConfiguration configuration, PublisherService publisherService)
        {
            _userRepository = userRepository;
            _redisCache=redisCache;
            _key = Encoding.UTF8.GetBytes(configuration["Jwt:Key"]);
            _configuration = configuration;
            _publisherService = publisherService;
        }

        public async Task<User> GetUserAsync(Guid id) =>
            await _userRepository.GetUserByIdAsync(id);

        public async Task<IEnumerable<User>> GetAllUsersAsync() =>
            await _userRepository.GetAllUsersAsync();

        public async Task AddUserAsync(User user){
            var password=user.PasswordHash;
            user.PasswordHash = HashPassword(user.PasswordHash);

            await _userRepository.AddUserAsync(user);

            // Publish a message to create a new UserPortfolio
            _publisherService.PublishUserPortfolioCreation(user.Id);

            await LoginAsync(user.Username, password);
        }

        private string HashPassword(string password)
        {
            using (var hmac = new HMACSHA256(_key))
            {
                var passwordBytes = Encoding.UTF8.GetBytes(password);
                var hashedBytes = hmac.ComputeHash(passwordBytes);
                return Convert.ToBase64String(hashedBytes);
            }
        }

        public async Task UpdateUserAsync(User user) =>
            await _userRepository.UpdateUserAsync(user);

        public async Task DeleteUserAsync(Guid id){
            await _userRepository.DeleteUserAsync(id);

            // Publish a message to Delete the UserPortfolio
            _publisherService.PublishUserPortfolioDeletion(id);

            await _redisCache.KeyDeleteAsync($"token:{id}");
        }

        public async Task<Guid?> LoginAsync(string username, string password){
            var user = await _userRepository.GetAllUsersAsync();

            var existingUser = user.FirstOrDefault(u => u.Username == username);
            if (existingUser == null) return null;

            if (VerifyPasswordHash(password, existingUser.PasswordHash)){
                var token=GenerateJwtToken(existingUser);
                await _redisCache.StringSetAsync($"token:{existingUser.Id}", token, TimeSpan.FromMinutes(_cacheExpiryTime));
                return existingUser.Id;
            }

            return null;
        }

        public async Task LogoutAsync(Guid userId)
        {
            await _redisCache.KeyDeleteAsync($"token:{userId}");
        }

        private bool VerifyPasswordHash(string password, string storedHash){
            using (var hmac = new HMACSHA256(_key)){
                var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                var hash = Convert.ToBase64String(hashBytes);
                return hash == storedHash;
            }
        }

        public string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("id", user.Id.ToString())
            };

            var creds = new SigningCredentials(new SymmetricSecurityKey(_key), SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(_cacheExpiryTime),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<string> GetTokenAsync(Guid userId){
            var token = await _redisCache.StringGetAsync($"token:{userId}");
            
            return token.IsNullOrEmpty ? null : token.ToString();
        }

    }
}
