using System.Threading.Tasks;
using DevWork.Shared.Models;
using DevWork.Shared.DTOs;
using Microsoft.AspNetCore.Identity;

namespace DevWork.API.Helpers
{
    public interface IUserHelper
    {
        Task<User?> GetUserAsync(string email);

        Task<IdentityResult> AddUserAsync(User user, string password);

        Task<SignInResult> LoginAsync(LoginDTO model);

        Task<string?> GetRoleNameAsync(int roleId);

        Task CheckRoleAsync(string roleName);

        Task AddUserToRoleAsync(User user, string roleName);

        Task<bool> IsUserInRoleAsync(User user, string roleName);

        Task<IdentityResult> UpdateUserAsync(User user);

        Task LogoutAsync();

        Task<string> GenerateEmailConfirmationTokenAsync(User user);

        Task<IdentityResult> ConfirmEmailAsync(User user, string token);

        Task<User?> GetUserAsync(int userId);
    }
}