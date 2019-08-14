using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SmartHome.Model;
using SmartHome.Models.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace SmartHome.Models.Security
{
    public class SmartHomeUser : IdentityUser
    {
        // Profiles
        public string ActiveProfileName { get; set; }

        public List<string> AvailableProfiles { get; set; }

        [JsonIgnore]
        public List<UserProfile> Profiles { get; set; }
    }

    public class SmartHomeUserStore : IUserStore<SmartHomeUser>, IUserPasswordStore<SmartHomeUser>
    {
        private readonly LocalPasswordHasher _hasher = new LocalPasswordHasher();
        private List<SmartHomeUser> _users;

        public SmartHomeUserStore(IOptionsMonitor<List<SmartHomeUser>> users)
        {
            _users = users.CurrentValue;
        }

        public Task<IdentityResult> CreateAsync(SmartHomeUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> DeleteAsync(SmartHomeUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {

            //throw new NotImplementedException();
        }

        public Task<SmartHomeUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            var user = _users.FirstOrDefault(u => u.Id.Equals(userId));
            return Task.FromResult(user);
        }

        public Task<SmartHomeUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            //var user = new SmartHomeUser()
            //{
            //    NormalizedUserName = normalizedUserName,
            //    Id = Guid.NewGuid().ToString(),
            //    UserName = "John Smith"
            //};
            //user.PasswordHash = _hasher.HashPassword(user, "123");

            var user = _users.Where(u => u.NormalizedUserName == normalizedUserName).FirstOrDefault();

            if (user != null)
            {
                user.PasswordHash = _hasher.HashPassword(user, user.PasswordHash);
            }
            
            return Task.FromResult(user);
        }

        public Task<string> GetNormalizedUserNameAsync(SmartHomeUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.NormalizedUserName);
        }

        public Task<string> GetPasswordHashAsync(SmartHomeUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PasswordHash);
        }

        public Task<string> GetUserIdAsync(SmartHomeUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Id);
        }

        public Task<string> GetUserNameAsync(SmartHomeUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.UserName);
        }

        public Task<bool> HasPasswordAsync(SmartHomeUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PasswordHash != null);
        }

        public Task SetNormalizedUserNameAsync(SmartHomeUser user, string normalizedName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetPasswordHashAsync(SmartHomeUser user, string passwordHash, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetUserNameAsync(SmartHomeUser user, string userName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> UpdateAsync(SmartHomeUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }


        private class LocalPasswordHasher : PasswordHasher<SmartHomeUser>
        {

            public override string HashPassword(SmartHomeUser user, string password)
            {
                return base.HashPassword(user, password);
            }

            public override PasswordVerificationResult VerifyHashedPassword(SmartHomeUser user, string hashedPassword, string providedPassword)
            {
                return base.VerifyHashedPassword(user, hashedPassword, providedPassword);
            }

            
        }

    }



}
