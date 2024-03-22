using System.Security.Claims;
using ALIVEMusicAPI.Models.Entities;
using CollaborateMusicAPI.Contexts;
using Microsoft.AspNetCore.Identity;

namespace ALIVEMusicAPI.SeedData;

public class RoleSeeder
{
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public RoleSeeder(RoleManager<IdentityRole<Guid>> roleManager, UserManager<ApplicationUser> userManager)
    {
        _roleManager = roleManager;
        _userManager = userManager;
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

        var adminUser = new ApplicationUser
        {
            UserName = "hannah@domain.com",
            FullName = "Hannah Ekström",
            Email = "hannah@domain.com",
            EmailConfirmed = true,
        };

        var user = await _userManager.FindByNameAsync(adminUser.UserName);
        if (user == null)
        {
            var result = await _userManager.CreateAsync(adminUser, "BytMig123!");
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(adminUser, UserRoles.Admin);
            }
        }

        var adminUser2 = new ApplicationUser
        {
            UserName = "svartkorp@hotmail.com",
            FullName = "Leah Saxe",
            Email = "svartkorp@hotmail.com",
            EmailConfirmed = true,
        };

        var user2 = await _userManager.FindByNameAsync(adminUser2.UserName);
        if (user2 == null)
        {
            var result = await _userManager.CreateAsync(adminUser2, "BytMig123!");
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(adminUser2, UserRoles.Admin);
            }
        }
    }

}