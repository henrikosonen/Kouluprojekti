using System.Collections.Generic;
namespace Apoa.Models
{
    public class Group
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<UserGroup> GroupUsers { get; set; }

        public virtual ICollection<CategoryGroup> CategoryGroups { get; set; }


    }
}