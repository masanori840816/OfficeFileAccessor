namespace OfficeFileAccessor.AppUsers.DTO;

public record UpdateUser(int? Id, string UserName, string? Organization, string Email, string Password);