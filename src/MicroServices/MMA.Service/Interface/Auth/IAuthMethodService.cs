namespace MMA.Service
{
    public interface IAuthMethodService
    {
        Task<string> GenerateAccessTokenAsync(UserEntity user);
        Task<string> GenerateRefreshTokenAsync(UserEntity user);
    }
}