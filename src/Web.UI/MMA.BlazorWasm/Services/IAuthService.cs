namespace MMA.BlazorWasm
{
    public interface IAuthService
    {
        Task<bool> LoginAsync(string email, string password);
        Task LogoutAsync();
    }
}