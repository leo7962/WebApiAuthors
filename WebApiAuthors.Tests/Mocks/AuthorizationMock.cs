using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace WebApiAuthors.Tests.Mocks;

public class AuthorizationMock : IAuthorizationService
{
    public AuthorizationResult Result { get; set; }

    public async Task<AuthorizationResult> AuthorizeAsync(ClaimsPrincipal user, object? resource,
        IEnumerable<IAuthorizationRequirement> requirements)
    {
        return await Task.FromResult(Result);
    }

    public async Task<AuthorizationResult> AuthorizeAsync(ClaimsPrincipal user, object? resource, string policyName)
    {
        return await Task.FromResult(Result);
    }
}