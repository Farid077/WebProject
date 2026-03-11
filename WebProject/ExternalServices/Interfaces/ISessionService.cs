namespace WebProject.ExternalServices.Interfaces;

public interface ISessionService
{
    Task CreateAsync(string userId, string sessionToken, CancellationToken ct = default);
    Task<bool> IsActiveAsync(string userId, CancellationToken ct = default);
    Task<bool> IsValidAsync(string userId, string sessionToken, CancellationToken ct = default);
    Task RevokeAsync(string userId, CancellationToken ct = default);
}
