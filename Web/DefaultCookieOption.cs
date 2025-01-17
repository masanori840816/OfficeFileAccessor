namespace OfficeFileAccessor.Web;

public static class DefaultCookieOption
{
    public static CookieOptions Get()
    {
        return new ()
        {
            Expires = DateTime.Now.AddDays(7),
            Path = "/",
            HttpOnly = true,
            //Secure = true
        };
    }
}