using Microsoft.Extensions.Caching.Distributed;
using WebProject.ExternalServices.Interfaces;

namespace WebProject.ExternalServices.Implements;

public class SessionService(IDistributedCache _cache) : ISessionService
{
    private const string Prefix = "Session:";

    public async Task CreateAsync(string userId, string sessionToken, CancellationToken ct = default)
    {
        await _cache.SetStringAsync(
            $"{Prefix}{userId}",
            sessionToken,
            new DistributedCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMinutes(10)
            }, ct);
    }

    public async Task<bool> IsActiveAsync(string userId, CancellationToken ct = default)
    {
        return null != await _cache.GetStringAsync($"{Prefix}{userId}", ct);
    }

    public async Task<bool> IsValidAsync(string userId, string sessionToken, CancellationToken ct = default)
    {
        return sessionToken == await _cache.GetStringAsync($"{Prefix}{userId}", ct);
    }

    public async Task RevokeAsync(string userId, CancellationToken ct = default)
    {
        await _cache.RemoveAsync($"{Prefix}{userId}", ct);
    }
}
