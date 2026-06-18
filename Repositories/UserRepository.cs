using Microsoft.EntityFrameworkCore;
using Zariya.Data;
using Zariya.Interfaces;
using Zariya.Models;

namespace Zariya.Repositories;

public sealed class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(ZariyaDbContext context) : base(context)
    {
    }

    public Task<User?> GetByEmailAsync(string email)
    {
        return Context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }
}
