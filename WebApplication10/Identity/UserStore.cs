using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using ClassLibrary1;
using Microsoft.AspNet.Identity;

namespace WebApplication10.Identity
{
    public class UserStore :
        IUserStore<User, int>,
        IUserPasswordStore<User, int>,
        IUserLockoutStore<User, int>,
        IUserTwoFactorStore<User, int>,
        IUserRoleStore<User, int>,
        IUserClaimStore<User, int>,
        IUserLoginStore<User, int>
    {
        private readonly PocId2Entities _dbContext;

        public UserStore(PocId2Entities dbContext)
        {
            _dbContext = dbContext;
        }

        #region USER STORE

        public System.Threading.Tasks.Task CreateAsync(User user)
        {
            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();
            return Task.FromResult(user);
        }

        public System.Threading.Tasks.Task DeleteAsync(User user)
        {
            _dbContext.Users.Remove(user);
            return _dbContext.SaveChangesAsync();
        }

        public System.Threading.Tasks.Task<User> FindByIdAsync(int userId)
        {
            return _dbContext.Users.SingleOrDefaultAsync(f => f.Id == userId);
        }

        public System.Threading.Tasks.Task<User> FindByNameAsync(string userName)
        {
            return _dbContext.Users.SingleOrDefaultAsync(f => f.UserName == userName);
        }

        public System.Threading.Tasks.Task UpdateAsync(User user)
        {
            return _dbContext.SaveChangesAsync();
        }

        public void Dispose()
        {
            this._dbContext.Dispose();
        }

        #endregion

        #region PASSWORD STORE

        public System.Threading.Tasks.Task<string> GetPasswordHashAsync(User user)
        {
            return Task.FromResult(user.PasswordHash);
        }

        public System.Threading.Tasks.Task<bool> HasPasswordAsync(User user)
        {
            return Task.FromResult(user.PasswordHash != null);
        }

        public System.Threading.Tasks.Task SetPasswordHashAsync(User user, string passwordHash)
        {
            user.PasswordHash = passwordHash;
            return Task.FromResult(0);
        }

        #endregion

        #region LOCKOUT STORE

        public Task<int> GetAccessFailedCountAsync(User user)
        {
            return Task.FromResult(user.AccessFailedCount);
        }

        public Task<bool> GetLockoutEnabledAsync(User user)
        {
            return Task.FromResult(user.LockoutEnabled);
        }

        public Task<DateTimeOffset> GetLockoutEndDateAsync(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            if (!user.LockoutEndDateUtc.HasValue)
            {
                throw new InvalidOperationException("LockoutEndDate has no value.");
            }

            return
                Task.FromResult(user.LockoutEndDateUtc.HasValue
                    ? new DateTimeOffset(DateTime.SpecifyKind(user.LockoutEndDateUtc.Value, DateTimeKind.Utc))
                    : new DateTimeOffset());
        }

        public Task<int> IncrementAccessFailedCountAsync(User user)
        {
            user.AccessFailedCount++;
            return Task.FromResult(user.AccessFailedCount);
        }

        public Task ResetAccessFailedCountAsync(User user)
        {
            user.AccessFailedCount = 0;
            return Task.FromResult(0);
        }

        public Task SetLockoutEnabledAsync(User user, bool enabled)
        {
            user.LockoutEnabled = enabled;
            return Task.FromResult(0);
        }

        public Task SetLockoutEndDateAsync(User user, DateTimeOffset lockoutEnd)
        {
            user.LockoutEndDateUtc = lockoutEnd == DateTimeOffset.MinValue ? (DateTime?)null : lockoutEnd.UtcDateTime;
            return Task.FromResult(0);
        }

        #endregion

        #region TWO FACTOR

        public Task<bool> GetTwoFactorEnabledAsync(User user)
        {
            return Task.FromResult(user.TwoFactorEnabled);
        }

        public Task SetTwoFactorEnabledAsync(User user, bool enabled)
        {
            user.TwoFactorEnabled = enabled;
            return Task.FromResult(0);
        }

        #endregion

        #region USERS - ROLES STORE

        public Task AddToRoleAsync(User user, string roleName)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (string.IsNullOrEmpty(roleName))
            {
                throw new ArgumentNullException("role");
            }

            var role = _dbContext.Roles.SingleOrDefault(f => f.Name == roleName);

            if (role == null)
            {
                throw new KeyNotFoundException("role");
            }

            if (user.Roles != null && user.Roles.All(r => r.Name != roleName))
            {
                user.Roles.Add(role);
            }

            return Task.FromResult(0);
        }

        public Task<IList<string>> GetRolesAsync(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            return Task.FromResult<IList<string>>(user.Roles.Select(r=>r.Name).ToList());
        }

        public Task<bool> IsInRoleAsync(User user, string roleName)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (string.IsNullOrEmpty(roleName))
            {
                throw new ArgumentNullException("role");
            }

            return Task.FromResult(user.Roles.Any(r=>r.Name==roleName));
        }

        public Task RemoveFromRoleAsync(User user, string roleName)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            var role = _dbContext.Roles.Single(r => r.Name == roleName);
            user.Roles.Remove(role);

            return Task.FromResult(0);
        }

        #endregion

        #region USERS - CLAIM STORE

        public Task AddClaimAsync(User user, System.Security.Claims.Claim claim)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (claim == null)
            {
                throw new ArgumentNullException("claim");
            }

            if (user.UserClaims != null && user.UserClaims.Any(f => f.ClaimValue == claim.Value))
            {
                user.UserClaims.Add(new UserClaim(claim));
            }

            return Task.FromResult(0);
        }

        public Task<IList<System.Security.Claims.Claim>> GetClaimsAsync(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            return Task.FromResult<IList<System.Security.Claims.Claim>>(user.UserClaims.Select(clm => new System.Security.Claims.Claim(clm.ClaimType, clm.ClaimValue)).ToList());
        }

        public Task RemoveClaimAsync(User user, System.Security.Claims.Claim claim)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (claim == null)
            {
                throw new ArgumentNullException("user");
            }

            var userClaim =
                _dbContext.UserClaims.Single(uc => uc.ClaimType == claim.Type && uc.ClaimValue == claim.Value);

            user.UserClaims.Remove(userClaim);

            return Task.FromResult(0);
        }

        #endregion

        #region USER - LOGINS

        public Task AddLoginAsync(User user, UserLoginInfo login)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            if (login == null)
            {
                throw new ArgumentNullException("user");
            }

            if (!user.UserLogins.Any(x => x.LoginProvider == login.LoginProvider && x.ProviderKey == login.ProviderKey))
            {
                var userLogin = _dbContext.UserLogins.SingleOrDefault(ul => ul.UserId == user.Id);
                if (userLogin != null)
                {
                    userLogin.LoginProvider = login.LoginProvider;
                    userLogin.ProviderKey = login.ProviderKey;
                }
                else
                {
                    user.UserLogins.Add(new UserLogin() {UserId = user.Id,LoginProvider = login.LoginProvider,ProviderKey = login.ProviderKey});
                }
                return _dbContext.SaveChangesAsync();
            }

            return Task.FromResult(true);
        }

        public Task<User> FindAsync(UserLoginInfo login)
        {
            var loginId = GetLoginId(login);
            var user = _dbContext.Users.SingleOrDefault(f => f.Id == int.Parse(loginId));
            return Task.FromResult(user);
        }

        public virtual async Task<IList<UserLoginInfo>> GetLoginsAsync(User user)
        {
            var list = user.UserLogins.Select(ul => new UserLoginInfo(ul.LoginProvider, ul.LoginProvider));
            //return Task.FromResult(list);
            return list.ToList();
        }

        public Task RemoveLoginAsync(User user, UserLoginInfo login)
        {
            throw new NotImplementedException();
        }

        #endregion

        private string GetLoginId(UserLoginInfo login)
        {
            using (var sha = new SHA1CryptoServiceProvider())
            {
                var clearBytes = Encoding.UTF8.GetBytes(login.LoginProvider + "|" + login.ProviderKey);
                var hashBytes = sha.ComputeHash(clearBytes);
                return ToHex(hashBytes);
            }
        }

        private static string ToHex(byte[] bytes)
        {
            var sb = new StringBuilder(bytes.Length * 2);
            for (int i = 0; i < bytes.Length; i++)
                sb.Append(i.ToString("x2"));
            return sb.ToString();
        }
    }
}