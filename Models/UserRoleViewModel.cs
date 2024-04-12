using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace FPTBook.Models
{
    public class UserRoleViewModel
    {
        internal List<string?> Role;

        public IdentityUser User { get; set; }
        public List<string> Roles { get; set; }
    }
}