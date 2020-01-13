using System.Collections.Generic;

namespace Apoa.Models.ViewModels
{
    public class EditCategoryViewModel
    {

        public Category Category { get; set; }
        public virtual ICollection<Group> Groups { get; set; }

        // Alla olevat laitetaan post formille

        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<int> GroupIds { get; set; }

        public EditCategoryViewModel()
        {
            GroupIds = new List<int>();
        }
    }

}