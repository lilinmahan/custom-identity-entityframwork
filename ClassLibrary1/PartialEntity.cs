using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace ClassLibrary1
{
    public partial class User:IUser<int>
    {
    }

    public partial class Role : IRole<int>
    {
    }

    public class UserRole : IdentityUserRole<int>
    {
    }
    public partial class UserLogin 
    {
        public UserLogin()
        {
        }
        public UserLogin(string loginProvider, string providerKey)
        {
            this.LoginProvider = loginProvider;
            this.ProviderKey = providerKey;
        }
    }
    public partial class UserClaim
    {
        public UserClaim()
        {
            
        }
        public UserClaim(Claim claim)
        {
            if (claim == null) throw new ArgumentNullException("claim");

            ClaimType = claim.Type;
            ClaimValue = claim.Value;
        }
    }
}
