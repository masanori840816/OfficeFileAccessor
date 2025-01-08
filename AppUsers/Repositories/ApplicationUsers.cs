using Microsoft.EntityFrameworkCore;
using OfficeFileAccessor.AppUsers.Entities;

namespace OfficeFileAccessor.AppUsers.Repositories;

public class ApplicationUsers(OfficeFileAccessorContext context): IApplicationUsers
{
    public async Task<ApplicationUser?> GetByEmailForSigninAsync(string email)
    {
        return await context.ApplicationUsers
            .FirstOrDefaultAsync(u => u.Email == email);
    }
}