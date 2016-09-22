using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ClassLibrary1;
using Microsoft.AspNet.Identity;

namespace WebApplication10.Identity
{
    public class RoleManager : RoleManager<Role, int>
    {
        public RoleManager(IRoleStore<Role, int> store) : base(store)
        {
            this.RoleValidator = new RoleValidator<Role, int>(this);
        }
    }
}