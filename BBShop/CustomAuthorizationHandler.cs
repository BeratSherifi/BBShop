using System.Security.Claims;
using BBShop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace BBShop;

public class CustomAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, User>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
        OperationAuthorizationRequirement requirement,
        User resource)
    {
        if (context.User.IsInRole("admin") || 
            (context.User.IsInRole("buyer") && context.User.FindFirst(ClaimTypes.NameIdentifier).Value == resource.Id) ||
            (context.User.IsInRole("seller") && context.User.FindFirst(ClaimTypes.NameIdentifier).Value == resource.Id))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}