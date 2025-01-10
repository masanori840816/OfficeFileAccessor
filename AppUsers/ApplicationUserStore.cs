using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OfficeFileAccessor.AppUsers.Entities;

namespace OfficeFileAccessor.AppUsers;
public class ApplicationUserStore(OfficeFileAccessorContext Context): IUserPasswordStore<ApplicationUser>
{
    public async Task<IdentityResult> CreateAsync(ApplicationUser user,
            CancellationToken cancellationToken)
        {
            // validation
            string? validationError = user.Validate();
            if(string.IsNullOrEmpty(validationError) == false)
            {
                return IdentityResult.Failed(new IdentityError { Description = validationError });
            }
            using var transaction = await Context.Database.BeginTransactionAsync();
            
            if(await Context.ApplicationUsers
                .AnyAsync(u => u.Email == user.Email,
                cancellationToken))
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Description = "Your e-mail address is already used"
                });
            }
            var newUser = new ApplicationUser();
            newUser.Update(user);
            await Context.ApplicationUsers.AddAsync(newUser, cancellationToken);
            await Context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return IdentityResult.Success;
        }
        public async Task<IdentityResult> DeleteAsync(ApplicationUser user,
            CancellationToken cancellationToken)
        {
            ApplicationUser? target = await Context.ApplicationUsers
                .FirstOrDefaultAsync(u => u.Id == user.Id,
                cancellationToken);
            if(target != null)
            {
                Context.ApplicationUsers.Remove(target);
                await Context.SaveChangesAsync(cancellationToken);
            }
            return IdentityResult.Success;
        }
        public void Dispose() { /* do nothing */ }
        public async Task<ApplicationUser?> FindByIdAsync(string userId,
            CancellationToken cancellationToken)
        {
            if(int.TryParse(userId, out var id) == false)
            {
                return new ApplicationUser();
            }
            var result = await Context.ApplicationUsers
                .FirstOrDefaultAsync(u => u.Id == id,
                cancellationToken);
            return result ?? new ApplicationUser();
        }
        public async Task<ApplicationUser?> FindByNameAsync(string normalizedUserName,
            CancellationToken cancellationToken)
        {
            var result = await Context.ApplicationUsers
                .FirstOrDefaultAsync(u => u != null &&
                    string.IsNullOrEmpty(u.UserName) == false &&
                    u.UserName.ToUpper() == normalizedUserName,
                cancellationToken);
            return result ?? new ApplicationUser();
        }
        public async Task<string?> GetNormalizedUserNameAsync(ApplicationUser user,
            CancellationToken cancellationToken)
        {
            return await Task.FromResult(user.NormalizedUserName);
        }
        public async Task<string?> GetPasswordHashAsync(ApplicationUser user,
            CancellationToken cancellationToken)
        {
            Console.WriteLine("GetPASSWORD " + user.PasswordHash);
            return await Task.FromResult(user.PasswordHash);
        }
        public async Task<string> GetUserIdAsync(ApplicationUser user,
            CancellationToken cancellationToken)
        {
            return await Task.FromResult(user.Id.ToString());
        }
        public async Task<string?> GetUserNameAsync(ApplicationUser user,
            CancellationToken cancellationToken)
        {
            return await Task.FromResult(user.UserName);
        }
        public async Task<bool> HasPasswordAsync(ApplicationUser user,
            CancellationToken cancellationToken)
        {
            return await Task.FromResult(true);
        }
        public async Task SetNormalizedUserNameAsync(ApplicationUser user,
            string? normalizedName, CancellationToken cancellationToken)
        {
            // do nothing
            await Task.Run(() => {});
        }
        public async Task SetPasswordHashAsync(ApplicationUser user, string? passwordHash,
            CancellationToken cancellationToken)
        {
            using var transaction = await Context.Database.BeginTransactionAsync();
            var target = await Context.ApplicationUsers
                    .FirstOrDefaultAsync(u => u.Id == user.Id,
                cancellationToken);
            if(target != null)
            {
                target.PasswordHash = passwordHash;
                // validation
                await Context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }           
        }
        public async Task SetUserNameAsync(ApplicationUser user, string? userName, CancellationToken cancellationToken)
        {
            using var transaction = await Context.Database.BeginTransactionAsync();
            
            var target = await Context.ApplicationUsers
                    .FirstOrDefaultAsync(u => u.Id == user.Id,
                cancellationToken);
            if(target != null)
            {
                target.UserName = userName;
                // validation
                await Context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }
        }
        public async Task<IdentityResult> UpdateAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            var validationError = user.Validate();
            if(string.IsNullOrEmpty(validationError) == false)
            {
                return IdentityResult.Failed(new IdentityError { Description = validationError });
            }
            using var transaction = await Context.Database.BeginTransactionAsync(cancellationToken);

            var target = await Context.ApplicationUsers
                .FirstOrDefaultAsync(u => u.Id == user.Id,
                cancellationToken);
            if(target == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "Target user was not found" });
            }
            // validation
            target.Update(user);
            await Context.SaveChangesAsync(cancellationToken);
            transaction.Commit();
            return IdentityResult.Success;
        }
}