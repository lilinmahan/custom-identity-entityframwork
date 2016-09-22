using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using ClassLibrary1;
using Microsoft.AspNet.Identity;

namespace WebApplication10.Identity
{
    public class RoleStore : IRoleStore<Role, int>, IQueryableRoleStore<Role, int>
    {
        private readonly PocId2Entities _dbContext = null;

        public RoleStore()
        {
        }

        public RoleStore(PocId2Entities dbContext)
        {
            _dbContext = dbContext;
        }

        public Task CreateAsync(Role role)
        {
            return Task.FromResult(role);
        }

        public Task DeleteAsync(Role role)
        {
            throw new NotImplementedException();
        }

        public Task<Role> FindByIdAsync(int roleId)
        {
            Role role = null;
            
            return Task.FromResult(role);
        }

        public Task<Role> FindByNameAsync(string roleName)
        {
            Role role = null;

            return Task.FromResult(role);
        }

        public Task UpdateAsync(Role role)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            this._dbContext.Dispose();
        }

        public IQueryable<Role> Roles
        {
            get { return null; }
        }
    }
}