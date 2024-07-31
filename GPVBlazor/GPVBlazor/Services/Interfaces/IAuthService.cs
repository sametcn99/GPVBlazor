namespace GPVBlazor.Services.Interfaces
{
    public interface IAuthService
    {
        Task<bool> IsTokenValidAsync(string token);
    }
}
