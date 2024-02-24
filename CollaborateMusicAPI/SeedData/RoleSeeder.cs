using System.Security.Claims;
using ALIVEMusicAPI.Models.Entities;
using Microsoft.AspNetCore.Identity;

namespace ALIVEMusicAPI.SeedData;

public class RoleSeeder
{
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;

    public RoleSeeder(RoleManager<IdentityRole<Guid>> roleManager)
    {
        _roleManager = roleManager;
    }

    public async Task SeedRoles()
    {
        string[] roleNames = { UserRoles.Admin, UserRoles.Manager, UserRoles.Employee, UserRoles.SubscribingMember, UserRoles.NonPayingMember };

        foreach (var roleName in roleNames)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role == null)
            {
                //create the roles and seed them to the database
                role = new IdentityRole<Guid>(roleName);
                await _roleManager.CreateAsync(role);
            }

            // Add claims to the Admin role
            if (roleName == UserRoles.Admin)
            {
                var claim = new Claim("Permission", "Edit");
                var claims = await _roleManager.GetClaimsAsync(role);
                if (!claims.Any(c => c.Type == claim.Type && c.Value == claim.Value))
                {
                    await _roleManager.AddClaimAsync(role, claim);
                }
            }
        }
    }

}