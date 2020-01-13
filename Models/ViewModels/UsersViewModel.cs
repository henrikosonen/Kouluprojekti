using System.Collections.Generic;
//using Microsoft.Extensions.DependencyInjection;

namespace Apoa.Models.ViewModels
{

    public class UsersViewModel
    {

        public UsersViewModel()
        {
            Groups = new List<Group>();
            Users = new List<ApplicationUser>();

        }

        public ICollection<ApplicationUser> Users { get; set; }

        public virtual ICollection<Group> Groups { get; set; }

    }


}