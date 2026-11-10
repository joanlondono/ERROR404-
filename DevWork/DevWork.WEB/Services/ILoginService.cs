namespace DevWork.WEB.Services
{
    public class ILoginService
    {
        Task LoginAsync(string token);

        Task LogoutAsync();
    }
}
