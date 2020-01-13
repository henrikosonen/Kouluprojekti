using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Apoa.Models.ViewModels
{
    public class GroupViewModel
    {

        public Group Group { get; set; }

        public List<Category> Categories { get; set; }
        public GroupViewModel()
        {
            Categories = new List<Category>();
        }      
        public int Year { get; set; }
    }

}