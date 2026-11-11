namespace DevWork.WEB.Services
{
    public interface ILoginService
    {
        Task LoginAsync(string token);

        Task LogoutAsync();
    }
}
