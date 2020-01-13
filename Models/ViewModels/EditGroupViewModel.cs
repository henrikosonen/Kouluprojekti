using System.Collections.Generic;

namespace Apoa.Models.ViewModels
{
    public class EditGroupViewModel
    {

        public Group Group { get; set; }
        public virtual ICollection<UserGroup> UserGroups { get; set; }


        public string Name { get; set; }

        public int Id { get; set; }

        public ICollection<int> GroupIds { set; get; }
        public ICollection<int> UserIds { set; get; }

        public EditGroupViewModel()
        {
            GroupIds = new List<int>();

        }

    }

}