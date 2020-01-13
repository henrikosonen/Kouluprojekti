using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Apoa.Models.ViewModels
{
    public class HomeViewModel
    {

        public IdentityUser User { get; set; }
        public List<Group> Groups { get; set; }
        
        public int Year {get; set;}
        public virtual List<Category> Categories { get; set; }

        public HomeViewModel()
        {
            Groups = new List<Group>();
            Categories = new List<Category>();
        }
        

    }

}