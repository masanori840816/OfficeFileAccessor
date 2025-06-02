using OfficeFileAccessor.Apps;

namespace OfficeFileAccessor.AppUsers.DTO;

public record SignInResult(ApplicationResult Result, DisplayUser? User);