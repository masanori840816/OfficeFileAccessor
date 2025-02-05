using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace OfficeFileAccessor.AppUsers.Entities;

[Table("application_user")]
public class ApplicationUser: IdentityUser<int>
{
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public override int Id { get; set; }
    [Required]
    [Column("user_name")]
    public override string? UserName { get; set; } = null!;
    [Column("organization")]
    public string? Organization { get; set; }
    [Required]
    [Column("mail")]
    public override string? Email { get; set; } = null!;
    [Required]
    [Column("password")]
    public override string? PasswordHash { get; set; } = null!;
    [Required]
    [Column("last_update_date", TypeName = "timestamp with time zone")]
    public DateTime LastUpdateDate { get; set; }
    [NotMapped]
    public override bool EmailConfirmed { get; set; }
    [NotMapped]
    public override string? NormalizedUserName {
        get
        {
            return UserName?.ToUpper();
        }
        set { /* DO nothing*/ }
    }
    [NotMapped]
    public override string? NormalizedEmail {
        get
        {
            return Email?.ToUpper();
        }
        set { /* DO nothing*/ }
    }
    [NotMapped]
    public override bool LockoutEnabled { get; set; }
    [NotMapped]
    public override int AccessFailedCount { get; set; }
    [NotMapped]
    public override string? PhoneNumber { get; set; }
    [NotMapped]
    public override string? ConcurrencyStamp { get; set; }
    [NotMapped]
    public override string? SecurityStamp { get; set; }
    [NotMapped]
    public override DateTimeOffset? LockoutEnd { get; set; }
    [NotMapped]
    public override bool TwoFactorEnabled { get; set; }
    [NotMapped]
    public override bool PhoneNumberConfirmed { get; set; }

    public static ApplicationUser Create(string userName, string email, string password, int? id = null)
    {
        var result = new ApplicationUser
        {
            UserName = userName,
            Email = email,
        };
        if(id != null)
        {
            result.Id = id.Value;
        }
        result.PasswordHash = new PasswordHasher<ApplicationUser>()
            .HashPassword(result, password);
        return result;
    }
    public static ApplicationUser Copy(ApplicationUser user)
    {
        return new ApplicationUser
        {
            Id = user.Id,
            UserName = user.UserName,
            Organization = user.Organization,
            Email = user.Email,
            LastUpdateDate = DateTime.Now.ToUniversalTime(),
        };
    }    
    public void Update(ApplicationUser user)
    {
        this.UserName = user.UserName;
        this.Organization = user.Organization;
        this.Email = user.Email;
        this.PasswordHash = user.PasswordHash;
        this.LastUpdateDate = DateTime.Now.ToUniversalTime();
    }
    public void Update(string userName, string organization,
        string email, string password)
    {
        this.UserName = userName;
        this.Organization = organization;
        this.Email = email;
        // set hashed password text to PasswordHash.
        this.PasswordHash = new PasswordHasher<ApplicationUser>()
            .HashPassword(this, password);
        this.LastUpdateDate = DateTime.Now.ToUniversalTime();
    }
    public string? Validate()
    {
        if(string.IsNullOrEmpty(UserName))
        {
            return "UserName is required";
        }
        if(string.IsNullOrEmpty(Email))
        {
            return "E-Mail address is required";
        }
        if(string.IsNullOrEmpty(PasswordHash))
        {
            return "Password is required";
        }
        return null;
    }
}
