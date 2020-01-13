using System.Collections.Generic;

namespace Apoa.Models.ViewModels
{
    public class UserResponseViewModel
    {

        public int Year { get; set; }
        public ApplicationUser User { get; set; }
        //Responseista collection
        public virtual ICollection<Response> Responses { get; set; }

        public virtual ICollection<Response> GroupResponses { get; set; }

        public virtual ICollection<Assessment> Assessments { get; set; }

        public virtual List<Category> Categories { get; set; }

    }

}