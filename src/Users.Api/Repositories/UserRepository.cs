using Microsoft.EntityFrameworkCore;
using Users.Api.Context;
using Users.Api.Models;

namespace Users.Api.Repositories
{
    public sealed class UserRepository(AppDbContext context) : IUserRepository
    {
        public async Task<bool> CreateAsync(User user, CancellationToken cancellationToken = default)
        {
            await context.AddAsync(user,cancellationToken);
            var result = await context.SaveChangesAsync();
            return result > 0 ? true : false;
        }

        public async Task<bool> DeleteAsync(User user, CancellationToken cancellationToken = default)
        {
            context.Remove(user);
            var result = await context.SaveChangesAsync();
            return result > 0 ? true : false;
        }

        public async Task<List<User>> GetAllAsync(CancellationToken cancellationToken = default) =>
            await context.Users.ToListAsync(cancellationToken);

        public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
            await context.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

        public async Task<bool> IsNameExist(string fullName, CancellationToken cancellationToken = default)
        {
            return await context.Users.AnyAsync(u => u.FullName == fullName, cancellationToken);
        }

        public Task<bool> UpdateAsync(Guid id, User user, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }


}
