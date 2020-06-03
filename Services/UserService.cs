using System;
using System.Collections.Generic;
using System.Linq;
using backend.Data;
using backend.Helpers;
using backend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace backend.Services
{
    public interface IUserService
    {
        ApplicationUser Authenticate(string username, string password);
        IEnumerable<ApplicationUser> GetAll();
        ApplicationUser GetById(string id);
        ApplicationUser Create(ApplicationUser user, string password);
        void Update(ApplicationUser user, string oldPassword = null, string newPassword = null);
        void Delete(string id);
    }

    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public ILogger<UserService> Logger;

        public UserService(ApplicationDbContext context, UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager, ILogger<UserService> logger)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            Logger = logger;
        }

        public ApplicationUser Authenticate(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return null;

            var user = _context.Users.SingleOrDefault(u => u.UserName == username);
            
            // check if username exists
            if (user == null)
                return null;

            // check if password is correct
            if (!_signInManager.PasswordSignInAsync(user.UserName, password, true, false).Result.Succeeded)
                return null;

            // authentication successful
            return user;
        }
        
        public IEnumerable<ApplicationUser> GetAll()
        {
            return _context.Users;
        }

        public ApplicationUser GetById(string id)
        {
            return _context.Users.Find(id);
        }

        public ApplicationUser Create(ApplicationUser user, string password)
        {
            user.UserName = user.UserName.ToLower();
            
            // validation
            if (string.IsNullOrWhiteSpace(password))
                throw new AppException("Password is required");

            if (_context.Users.Any(x => x.UserName == user.UserName))
                throw new AppException("Username \"" + user.UserName + "\" is already taken");
            
            if (_context.Users.Any(x => x.Email == user.Email))
                throw new AppException("Email \"" + user.Email + "\" is already taken");
            
            _userManager.CreateAsync(user, password).Wait();
            _context.SaveChanges();

            return user;
        }

        public void Update(ApplicationUser userParam, string oldpassword = null, string newpassword = null)
        {
            var user = _context.Users.Find(userParam.Id);

            if (user == null)
                throw new AppException("User not found");

            // update username if it has changed
            if (!string.IsNullOrWhiteSpace(userParam.UserName) && userParam.UserName != user.UserName)
            {
                // throw error if the new username is already taken
                if (_context.Users.Any(x => x.UserName == userParam.UserName))
                    throw new AppException("Username " + userParam.UserName + " is already taken");
            }

            if (!string.IsNullOrWhiteSpace(userParam.Email) && userParam.Email != user.Email)
            {
                if (_context.Users.Any(x => x.Email == userParam.Email))
                    throw new AppException("Email " + userParam.Email + " is already taken");
            }

            if (!string.IsNullOrWhiteSpace(userParam.Role))
                user.Role = userParam.Role;
            
            // update user properties if provided
            if (!string.IsNullOrWhiteSpace(userParam.FirstName))
                user.FirstName = userParam.FirstName;

            if (!string.IsNullOrWhiteSpace(userParam.LastName))
                user.LastName = userParam.LastName;
            
            if (!string.IsNullOrWhiteSpace(user.Email))
                user.Email = userParam.Email;

            // update password if provided
            if (!string.IsNullOrWhiteSpace(oldpassword) && !string.IsNullOrWhiteSpace(newpassword))
            {
                var res = _userManager.ChangePasswordAsync(user, oldpassword, newpassword);
                if (res.Result.Errors.Any())
                    throw new AppException(res.Result.ToString());
            }

            _context.Users.Update(user);
            _context.SaveChanges();
        }

        public void Delete(string id)
        {
            var user = _context.Users.Find(id);
            if (user == null) return;
            
            _context.Users.Remove(user);
            _context.SaveChanges();
        }
    }
}