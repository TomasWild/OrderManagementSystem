using OrderManagementSystem.Models;

namespace OrderManagementSystem.Services.Interfaces;

public interface ITokenService
{
    string GenerateToken(ApplicationUser applicationUser, IList<string> roles);
}