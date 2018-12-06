using System;
using System.Collections.Generic;
using CapstoneApi.Models;

namespace CapstoneApi.Services
{
    public interface IUserService
    {
        User Authenticate(string email, string password);
        IEnumerable<User> GetAll();
        User GetById(Guid id);
        User Create(User user, string password);
        void Update(User user, string currentPassword, string newPassword = null);
        void Delete(Guid id);
    }
}