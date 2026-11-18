using DevWork.API.Data;
using DevWork.Shared.DTOs;
using DevWork.Shared.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DevWork.API.Helpers
{
    public class UserHelper : IUserHelper
    {
        private readonly DataContext _context;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly SignInManager<User> _signInManager;

        public UserHelper(DataContext context, UserManager<User> userManager, RoleManager<Role> roleManager, SignInManager<User> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }

        public async Task<IdentityResult> AddUserAsync(User user, string password)
        {
            return await _userManager.CreateAsync(user, password);
        }

        public async Task<SignInResult> LoginAsync(LoginDTO model)
        {
            return await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, false);
        }

        public async Task<string?> GetRoleNameAsync(int roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId.ToString());
            return role?.Name;
        }

        public async Task AddUserToRoleAsync(User user, string roleName)
        {
            await CheckRoleAsync(roleName);
            await _userManager.AddToRoleAsync(user, roleName);
        }

        public async Task CheckRoleAsync(string roleName)
        {
            bool roleExists = await _roleManager.RoleExistsAsync(roleName);
            if (!roleExists)
            {
                await _roleManager.CreateAsync(new Role
                {
                    Name = roleName
                });
            }
        }

        public async Task<User?> GetUserAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<bool> IsUserInRoleAsync(User user, string roleName)
        {
            return await _userManager.IsInRoleAsync(user, roleName);
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<string> GenerateEmailConfirmationTokenAsync(User user)
        {
            return await _userManager.GenerateEmailConfirmationTokenAsync(user);
        }

        public async Task<IdentityResult> ConfirmEmailAsync(User user, string token)
        {
            return await _userManager.ConfirmEmailAsync(user, token);
        }

        public async Task<User?> GetUserAsync(int userId)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
        }
    }
}
