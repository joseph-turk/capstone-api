using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using CapstoneApi.Models;
using CapstoneApi.Helpers;

namespace CapstoneApi.Services
{
    public class UserService : IUserService
    {
        private CapstoneContext _context;

        public UserService(CapstoneContext context)
        {
            _context = context;
        }

        public User Authenticate(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                return null;
            }

            User user = _context.Users.SingleOrDefault(x => x.Email.Equals(email));

            // Make sure user exists in DB
            if (user == null) return null;

            // Verify password is correct
            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            {
                return null;
            }

            return user;
        }

        public IEnumerable<User> GetAll()
        {
            return _context.Users;
        }

        public User GetById(Guid id)
        {
            return _context.Users.Where(u => u.Id == id)
                .Include(u => u.Events)
                .ThenInclude(e => e.Registrations)
                .First();
        }

        public User Create(User user, string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new AppException("Password is required");
            }

            if (_context.Users.Any(x => x.Email.Equals(user.Email)))
            {
                throw new AppException("Email is already in use");
            }

            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            _context.Users.Add(user);
            _context.SaveChanges();

            return user;
        }

        public void Update(User userParam, string currentPassword, string newPassword = null)
        {
            User user = _context.Users.Find(userParam.Id);

            if (user == null) throw new AppException("User not found");

            if (!userParam.Email.Equals(user.Email))
            {
                // Check if new username is available
                if (_context.Users.Any(x => x.Email.Equals(userParam.Email)))
                {
                    throw new AppException("Email is already in use");
                }
            }

            // Verify old password is correct
            if (newPassword != null && currentPassword != null)
            {
                if (!VerifyPasswordHash(currentPassword, user.PasswordHash, user.PasswordSalt))
                {
                    throw new AppException("Old password is incorrect");
                }
            }

            user.Email = userParam.Email;
            user.IsAdmin = userParam.IsAdmin;

            // Update password if it was entered
            if (!string.IsNullOrWhiteSpace(newPassword))
            {
                byte[] passwordHash, passwordSalt;
                CreatePasswordHash(newPassword, out passwordHash, out passwordSalt);

                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
            }

            _context.Users.Update(user);
            _context.SaveChanges();
        }

        public void Delete(Guid id)
        {
            User user = _context.Users.Find(id);

            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
            }
        }

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Value cannot be empty or whitespace-only string.", "password");
            }

            using (HMACSHA512 hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        private static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Value cannot be empty or whitespace-only string.", "password");
            }

            if (storedHash.Length != 64)
            {
                throw new ArgumentException("Invalid length of password hash (64 bytes expcected).", "storedHash");
            }

            if (storedSalt.Length != 128)
            {
                throw new ArgumentException("Invalid length of password salt (128 bytes expected).", "storedSalt");
            }

            using (HMACSHA512 hmac = new HMACSHA512(storedSalt))
            {
                byte[] computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != storedHash[i]) return false;
                }
            }

            return true;
        }
    }
}