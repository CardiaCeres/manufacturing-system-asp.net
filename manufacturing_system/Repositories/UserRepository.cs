using ManufacturingSystem.Data;
using ManufacturingSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ManufacturingSystem.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;
        public UserRepository(AppDbContext context) => _context = context;

        public async Task<User?> GetByUsernameAsync(string username) =>
            await _context.Users.FirstOrDefaultAsync(u => u.Username == username);

        public async Task<User?> GetByEmailAsync(string email) =>
            await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

        public async Task<User?> GetByResetTokenAsync(string resetToken) =>
            await _context.Users.FirstOrDefaultAsync(u => u.ResetToken == resetToken);

        public async Task<User> AddOrUpdateAsync(User user)
        {
            if (user.Id == 0) _context.Users.Add(user);
            else _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return user;
        }
    }
}
