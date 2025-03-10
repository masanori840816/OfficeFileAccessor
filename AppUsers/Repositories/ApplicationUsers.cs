using Microsoft.EntityFrameworkCore;
using OfficeFileAccessor.AppUsers.DTO;
using OfficeFileAccessor.AppUsers.Entities;

namespace OfficeFileAccessor.AppUsers.Repositories;

public class ApplicationUsers(OfficeFileAccessorContext Context): IApplicationUsers
{
    public async Task<ApplicationUser?> GetByEmailForSignInAsync(string email)
    {
        return await Context.ApplicationUsers
            .FirstOrDefaultAsync(u => u.Email == email);
    }
    public async Task<List<SearchUser>> SearchUsersAsync(string? organization, string? userName,
        string? email, string? updateDateFrom, string? updateDateTo)
    {
        string sql = """
                SELECT auser.id AS "Id",
                auser.user_name AS "UserName",
                auser.organization AS "Organization",
                auser.mail AS "Email",
                COUNT(ofile.id) AS "UseCount",
                auser.last_update_date AS "LastUpdateDate"
                FROM application_user auser
                LEFT JOIN office_file ofile ON auser.id = ofile.register_user_id
                GROUP BY auser.id
            """;
        IQueryable<SearchUser> query = Context.SearchUsers.FromSqlRaw(sql);
        if(string.IsNullOrEmpty(organization) == false)
        {
            query = query.Where(u => u.Organization != null && u.Organization.Contains(organization));
        }
        if(string.IsNullOrEmpty(userName) == false)
        {
            query = query.Where(u => u.UserName != null && u.UserName.Contains(userName));
        }
        if(string.IsNullOrEmpty(email) == false)
        {
            query = query.Where(u => u.Email != null && u.Email.Contains(email));
        }
        if(string.IsNullOrEmpty(updateDateFrom) == false &&
            DateTime.TryParse($"{updateDateFrom} 00:00:00", out var updateFrom))
        {
            var uData = updateFrom.ToUniversalTime();
            query = query.Where(u => u.LastUpdateDate >= uData);
        }
        if(string.IsNullOrEmpty(updateDateTo) == false &&
            DateTime.TryParse($"{updateDateTo} 23:59:59", out var updateTo))
        {
            var uData = updateTo.ToUniversalTime();
            query = query.Where(u => u.LastUpdateDate <= uData);
        }
        return await query.ToListAsync();
    }
}