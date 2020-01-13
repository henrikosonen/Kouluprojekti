using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Apoa.Models
{
    public class ApplicationUser : IdentityUser
    {
        public virtual ICollection<UserGroup> UserGroups { get; set; }

        //public virtual ICollection<IdentityRole> Roles {get; set;}

        public string GetUserRole() {
            var parts = this.UserName.Split("@");
            return parts[1].ToLower() == "savonia.fi" || parts[0].ToLower() == "antti.nikku" ? "Opettaja" : "Opiskelija"; 
        }

        public ApplicationUser()
        {
            UserGroups = new List<UserGroup>();
        }

    }
}